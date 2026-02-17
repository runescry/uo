/*
 * Vystia Class System v2.0
 * Resource Manager
 *
 * Attaches to PlayerMobile to manage all secondary resources.
 * Handles tick updates, serialization, and UI notifications.
 */

using System;
using System.Collections.Generic;
using Server;
using Server.Mobiles;
using Server.Network;
using Server.Commands;
using Server.Custom.VystiaClasses.Skills;

namespace Server.Custom.VystiaClasses.Systems
{
    /// <summary>
    /// Manages all secondary resources for a player
    /// </summary>
    public class VystiaResourceManager
    {
        private static readonly Dictionary<PlayerMobile, VystiaResourceManager> s_Managers =
            new Dictionary<PlayerMobile, VystiaResourceManager>();

        private PlayerMobile m_Owner;
        private Dictionary<ResourceType, ISecondaryResource> m_Resources;
        private ResourceType m_PrimaryResource;
        private ResourceType m_SecondaryResource;
        private Timer m_TickTimer;
        private bool m_InCombat;
        private DateTime m_LastCombatAction;
        private bool m_LastBardStatusEnabled;
        private int m_LastConcentration;
        private int m_LastConcentrationMax;
        private int m_LastCrescendo;
        private int m_LastCrescendoMax;

        public const double TICK_INTERVAL = 1.0; // 1 second ticks
        public const double COMBAT_TIMEOUT = 10.0; // Seconds after last action before "out of combat"

        #region Static Access

        /// <summary>
        /// Get or create a resource manager for a player
        /// </summary>
        public static VystiaResourceManager GetManager(PlayerMobile pm)
        {
            if (pm == null)
                return null;

            if (!s_Managers.TryGetValue(pm, out var manager))
            {
                manager = new VystiaResourceManager(pm);
                s_Managers[pm] = manager;
            }

            return manager;
        }

        public static void SendBardStatus(PlayerMobile pm)
        {
            if (pm == null || pm.NetState == null)
                return;

            bool isBard = pm.VystiaClassV2 == Server.Custom.VystiaClasses.PlayerClassTypeV2.Bard;

            if (!isBard)
            {
                pm.NetState.Send(new Server.Custom.VystiaClasses.Network.VystiaBardStatusPacket(false, 0, 0, 0, 0));
                return;
            }

            int concentration = pm.Concentration;
            int concentrationMax = pm.MaxConcentration;

            int crescendo = 0;
            int crescendoMax = 0;
            if (GetResource(pm, ResourceType.Crescendo) is CrescendoResource resource)
            {
                crescendo = resource.Current;
                crescendoMax = resource.Maximum;
            }

            pm.NetState.Send(new Server.Custom.VystiaClasses.Network.VystiaBardStatusPacket(
                true,
                concentration,
                concentrationMax,
                crescendo,
                crescendoMax));
        }

        /// <summary>
        /// Remove manager when player is deleted
        /// </summary>
        public static void RemoveManager(PlayerMobile pm)
        {
            if (pm != null && s_Managers.TryGetValue(pm, out var manager))
            {
                manager.Cleanup();
                s_Managers.Remove(pm);
            }
        }

        /// <summary>
        /// Get a specific resource for a player
        /// </summary>
        public static T GetResource<T>(PlayerMobile pm) where T : class, ISecondaryResource
        {
            var manager = GetManager(pm);
            if (manager == null)
                return null;

            foreach (var resource in manager.m_Resources.Values)
            {
                if (resource is T typed)
                    return typed;
            }

            return null;
        }

        /// <summary>
        /// Get a resource by type for a player
        /// </summary>
        public static ISecondaryResource GetResource(PlayerMobile pm, ResourceType type)
        {
            var manager = GetManager(pm);
            if (manager == null)
                return null;

            manager.m_Resources.TryGetValue(type, out var resource);
            return resource;
        }

        #endregion

        #region Constructor

        private VystiaResourceManager(PlayerMobile owner)
        {
            m_Owner = owner;
            m_Resources = new Dictionary<ResourceType, ISecondaryResource>();
            m_PrimaryResource = ResourceType.None;
            m_SecondaryResource = ResourceType.None;
            m_InCombat = false;
            m_LastCombatAction = DateTime.MinValue;

            StartTickTimer();
        }

        #endregion

        #region Resource Management

        /// <summary>
        /// Set the player's class resources
        /// </summary>
        public void SetClassResources(ResourceType primary, ResourceType secondary = ResourceType.None)
        {
            // Clear existing resources
            m_Resources.Clear();

            m_PrimaryResource = primary;
            m_SecondaryResource = secondary;

            // Create primary resource
            if (primary != ResourceType.None)
            {
                var resource = SecondaryResourceFactory.Create(primary, m_Owner);
                if (resource != null)
                    m_Resources[primary] = resource;
            }

            // Create secondary resource if different
            if (secondary != ResourceType.None && secondary != primary)
            {
                var resource = SecondaryResourceFactory.Create(secondary, m_Owner);
                if (resource != null)
                    m_Resources[secondary] = resource;
            }

            RefreshUI();
        }

        /// <summary>
        /// Add a resource type (for classes with multiple resources)
        /// </summary>
        public void AddResource(ResourceType type)
        {
            if (type == ResourceType.None || m_Resources.ContainsKey(type))
                return;

            var resource = SecondaryResourceFactory.Create(type, m_Owner);
            if (resource != null)
            {
                m_Resources[type] = resource;
                RefreshUI();
            }
        }

        /// <summary>
        /// Remove a resource type
        /// </summary>
        public void RemoveResource(ResourceType type)
        {
            if (m_Resources.Remove(type))
                RefreshUI();
        }

        /// <summary>
        /// Get a resource by type
        /// </summary>
        public ISecondaryResource GetResource(ResourceType type)
        {
            m_Resources.TryGetValue(type, out var resource);
            return resource;
        }

        /// <summary>
        /// Get the primary resource
        /// </summary>
        public ISecondaryResource PrimaryResource =>
            m_PrimaryResource != ResourceType.None ? GetResource(m_PrimaryResource) : null;

        /// <summary>
        /// Get the secondary resource
        /// </summary>
        public ISecondaryResource SecondaryResource =>
            m_SecondaryResource != ResourceType.None ? GetResource(m_SecondaryResource) : null;

        /// <summary>
        /// Check if player has a specific resource type
        /// </summary>
        public bool HasResource(ResourceType type) => m_Resources.ContainsKey(type);

        /// <summary>
        /// Get all active resources
        /// </summary>
        public IEnumerable<ISecondaryResource> AllResources => m_Resources.Values;

        #endregion

        #region Combat Hooks

        /// <summary>
        /// Called when combat action occurs
        /// </summary>
        public void OnCombatAction()
        {
            m_LastCombatAction = DateTime.UtcNow;

            if (!m_InCombat)
            {
                m_InCombat = true;
                foreach (var resource in m_Resources.Values)
                    resource.OnCombatStart();
            }
        }

        /// <summary>
        /// Called when player deals damage
        /// </summary>
        public void OnDamageDealt(Mobile target, int damage, bool isCrit)
        {
            OnCombatAction();

            // Trigger class skill gain from combat damage
            TriggerCombatSkillGain(target, damage, isCrit);

            foreach (var resource in m_Resources.Values)
            {
                switch (resource)
                {
                    case FuryResource fury:
                        fury.OnDamageDealt(damage);
                        break;

                    case SoulShardsResource shards when isCrit:
                        shards.OnCriticalHit();
                        break;

                    case ZealResource zeal:
                        if (isCrit)
                            zeal.OnCriticalHit();
                        else
                            zeal.OnHit();
                        break;

                    case SteamResource steam:
                        steam.OnGadgetHit();
                        break;

                    case PursuitResource pursuit when target == pursuit.MarkedTarget:
                        if (isCrit)
                            pursuit.OnCritMarked();
                        else
                            pursuit.OnHitMarked();
                        break;

                    case VirtueStacksResource virtues:
                        virtues.OnDamageToEvil(target);
                        break;
                }
            }
        }

        /// <summary>
        /// Called when player takes damage
        /// </summary>
        public void OnDamageTaken(Mobile attacker, int damage)
        {
            OnCombatAction();

            foreach (var resource in m_Resources.Values)
            {
                switch (resource)
                {
                    case FuryResource fury:
                        fury.OnDamageTaken(damage);
                        break;

                    case FortitudeResource fortitude:
                        fortitude.OnDamageTaken(damage);
                        break;
                }
            }
        }

        /// <summary>
        /// Called when player blocks an attack
        /// </summary>
        public void OnBlock()
        {
            OnCombatAction();

            if (GetResource(ResourceType.Fortitude) is FortitudeResource fortitude)
                fortitude.OnBlock();
        }

        /// <summary>
        /// Triggers class skill gain from combat damage dealt
        /// </summary>
        private void TriggerCombatSkillGain(Mobile target, int damage, bool isCrit)
        {
            if (m_Owner == null || target == null || damage <= 0)
                return;

            // Get the player's class skill
            SkillName? classSkill = VystiaSkillCheck.GetClassSkill(m_Owner);
            if (!classSkill.HasValue) // No class assigned
                return;

            // Calculate difficulty based on target
            double difficulty = VystiaSkillCheck.GetCreatureDifficulty(target);

            // Bonus difficulty for crits (harder to gain from easy kills)
            if (isCrit)
                difficulty += 5.0;

            // Damage scaling - bigger hits = higher chance
            // Scale: 10 damage = base, 50+ damage = +25% difficulty
            double damageBonus = Math.Min(25.0, damage / 2.0);
            difficulty += damageBonus;

            // Cap difficulty (Vystia: 100 is GM, no power scrolls)
            difficulty = Math.Min(100.0, difficulty);

            // Only trigger gain check ~25% of the time per hit to avoid spam
            if (Utility.RandomDouble() < 0.25)
            {
                VystiaSkillCheck.TriggerGainCheck(m_Owner, difficulty);
            }
        }

        /// <summary>
        /// Called when player kills a target
        /// </summary>
        public void OnKill(Mobile victim)
        {
            OnCombatAction();

            foreach (var resource in m_Resources.Values)
            {
                switch (resource)
                {
                    case SoulShardsResource shards:
                        shards.OnKill(victim);
                        break;

                    case FuryResource fury:
                        fury.OnKill(victim);
                        break;

                    case ZealResource zeal:
                        zeal.OnKill(victim);
                        break;

                    case VirtueStacksResource virtues:
                        virtues.OnKillCriminal(victim);
                        break;
                }
            }

            // Also notify nearby necromancers
            NotifyNearbyDeaths(victim);
        }

        /// <summary>
        /// Called when player heals a target
        /// </summary>
        public void OnHeal(Mobile target, int amount, bool isCrit)
        {
            foreach (var resource in m_Resources.Values)
            {
                switch (resource)
                {
                    case FaithResource faith:
                        if (isCrit)
                            faith.OnCriticalHeal();
                        else
                            faith.OnHeal(amount);
                        break;

                    case VirtueStacksResource virtues:
                        virtues.OnHealAlly(target);
                        break;
                }
            }
        }

        /// <summary>
        /// Called when player resurrects someone
        /// </summary>
        public void OnResurrection(Mobile target)
        {
            if (GetResource(ResourceType.Faith) is FaithResource faith)
                faith.OnResurrection();
        }

        /// <summary>
        /// Notify nearby necromancers of death
        /// </summary>
        private void NotifyNearbyDeaths(Mobile victim)
        {
            if (victim == null || victim.Map == null)
                return;

            IPooledEnumerable eable = victim.Map.GetMobilesInRange(victim.Location, 10);

            foreach (Mobile m in eable)
            {
                if (m is PlayerMobile pm && pm != m_Owner)
                {
                    var manager = GetManager(pm);
                    if (manager?.GetResource(ResourceType.LifeForce) is LifeForceResource lifeForce)
                    {
                        int dist = (int)pm.GetDistanceToSqrt(victim.Location);
                        lifeForce.OnNearbyDeath(victim, dist);
                    }
                }
            }

            eable.Free();
        }

        #endregion

        #region Tick System

        private void StartTickTimer()
        {
            m_TickTimer?.Stop();
            m_TickTimer = Timer.DelayCall(
                TimeSpan.FromSeconds(TICK_INTERVAL),
                TimeSpan.FromSeconds(TICK_INTERVAL),
                OnTick
            );
        }

        private void OnTick()
        {
            if (m_Owner == null || m_Owner.Deleted)
            {
                Cleanup();
                return;
            }

            // Check combat timeout
            if (m_InCombat && (DateTime.UtcNow - m_LastCombatAction).TotalSeconds > COMBAT_TIMEOUT)
            {
                m_InCombat = false;
                foreach (var resource in m_Resources.Values)
                    resource.OnCombatEnd();
            }

            // Tick all resources
            foreach (var resource in m_Resources.Values)
                resource.OnTick();

            // Update UI if any resource changed
            RefreshUI();
        }

        #endregion

        #region UI

        /// <summary>
        /// Refresh the resource display UI
        /// </summary>
        public void RefreshUI()
        {
            if (m_Owner == null || m_Owner.NetState == null)
                return;

            // Update the resource bar gump if enabled
            if (m_Owner is PlayerMobile pm)
            {
                Gumps.ResourceDisplayGump.Refresh(pm);
#if VYSTIA_CRESCENDO
                Gumps.CrescendoTrackerGump.Refresh(pm);
#endif
                SendBardStatusPacket(pm);
            }
        }

        private void SendBardStatusPacket(PlayerMobile pm)
        {
            if (pm == null || pm.NetState == null)
                return;

            bool isBard = pm.VystiaClassV2 == Server.Custom.VystiaClasses.PlayerClassTypeV2.Bard;

            if (!isBard)
            {
                if (m_LastBardStatusEnabled)
                {
                    pm.NetState.Send(new Server.Custom.VystiaClasses.Network.VystiaBardStatusPacket(false, 0, 0, 0, 0));
                }

                m_LastBardStatusEnabled = false;
                m_LastConcentration = 0;
                m_LastConcentrationMax = 0;
                m_LastCrescendo = 0;
                m_LastCrescendoMax = 0;
                return;
            }

            int concentration = pm.Concentration;
            int concentrationMax = pm.MaxConcentration;

            int crescendo = 0;
            int crescendoMax = 0;
            if (GetResource(pm, ResourceType.Crescendo) is CrescendoResource resource)
            {
                crescendo = resource.Current;
                crescendoMax = resource.Maximum;
            }

            if (!m_LastBardStatusEnabled ||
                concentration != m_LastConcentration ||
                concentrationMax != m_LastConcentrationMax ||
                crescendo != m_LastCrescendo ||
                crescendoMax != m_LastCrescendoMax)
            {
                pm.NetState.Send(new Server.Custom.VystiaClasses.Network.VystiaBardStatusPacket(
                    true,
                    concentration,
                    concentrationMax,
                    crescendo,
                    crescendoMax));

                m_LastBardStatusEnabled = true;
                m_LastConcentration = concentration;
                m_LastConcentrationMax = concentrationMax;
                m_LastCrescendo = crescendo;
                m_LastCrescendoMax = crescendoMax;
            }
        }

        /// <summary>
        /// Send resource info to player
        /// </summary>
        public void SendResourceInfo()
        {
            if (m_Owner == null)
                return;

            m_Owner.SendMessage(0x35, "=== Your Resources ===");

            foreach (var resource in m_Resources.Values)
            {
                if (resource is PerTargetResourceBase perTarget)
                {
                    if (m_Owner.Combatant is Mobile target)
                    {
                        m_Owner.SendMessage(0x35, "{0}: {1}/{2} (on {3})",
                            resource.Name, perTarget.GetStacks(target), resource.Maximum, target.Name);
                    }
                    else
                    {
                        m_Owner.SendMessage(0x35, "{0}: No target", resource.Name);
                    }
                }
                else
                {
                    m_Owner.SendMessage(0x35, "{0}: {1}/{2}",
                        resource.Name, resource.Current, resource.Maximum);
                }
            }
        }

        #endregion

        #region Serialization

        public void Serialize(GenericWriter writer)
        {
            writer.Write(0); // Version

            writer.Write((int)m_PrimaryResource);
            writer.Write((int)m_SecondaryResource);
            writer.Write(m_InCombat);
            writer.Write(m_LastCombatAction);

            writer.Write(m_Resources.Count);
            foreach (var kvp in m_Resources)
            {
                writer.Write((int)kvp.Key);
                kvp.Value.Serialize(writer);
            }
        }

        public void Deserialize(GenericReader reader)
        {
            int version = reader.ReadInt();

            m_PrimaryResource = (ResourceType)reader.ReadInt();
            m_SecondaryResource = (ResourceType)reader.ReadInt();
            m_InCombat = reader.ReadBool();
            m_LastCombatAction = reader.ReadDateTime();

            int count = reader.ReadInt();
            for (int i = 0; i < count; i++)
            {
                ResourceType type = (ResourceType)reader.ReadInt();
                var resource = SecondaryResourceFactory.Create(type, m_Owner);
                if (resource != null)
                {
                    resource.Deserialize(reader);
                    m_Resources[type] = resource;
                }
            }

            StartTickTimer();
        }

        #endregion

        #region Cleanup

        public void Cleanup()
        {
            m_TickTimer?.Stop();
            m_TickTimer = null;
            m_Resources.Clear();
        }

        #endregion
    }

    #region GM Commands

    public static class VystiaResourceCommands
    {
        public static void Initialize()
        {
            CommandSystem.Register("SetResource", AccessLevel.GameMaster, SetResource_OnCommand);
            CommandSystem.Register("GetResources", AccessLevel.GameMaster, GetResources_OnCommand);
            CommandSystem.Register("ResetResources", AccessLevel.GameMaster, ResetResources_OnCommand);
            CommandSystem.Register("TestResource", AccessLevel.GameMaster, TestResource_OnCommand);
        }

        [Usage("SetResource <type> <amount>")]
        [Description("Set a secondary resource value for yourself or targeted player.")]
        private static void SetResource_OnCommand(CommandEventArgs e)
        {
            if (e.Arguments.Length < 2)
            {
                e.Mobile.SendMessage("Usage: SetResource <type> <amount>");
#if VYSTIA_CRESCENDO
                e.Mobile.SendMessage("Types: SoulShards, LifeForce, ChillStacks, Fury, Chi, ComboPoints, Focus, Zeal, Fortitude, Pursuit, Steam, Charges, Faith, Crescendo, Virtues");
#else
                e.Mobile.SendMessage("Types: SoulShards, LifeForce, ChillStacks, Fury, Chi, ComboPoints, Focus, Zeal, Fortitude, Pursuit, Steam, Charges, Faith, Virtues");
#endif
                return;
            }

            if (!Enum.TryParse<ResourceType>(e.Arguments[0], true, out var type))
            {
                e.Mobile.SendMessage("Invalid resource type: {0}", e.Arguments[0]);
                return;
            }

            if (!int.TryParse(e.Arguments[1], out int amount))
            {
                e.Mobile.SendMessage("Invalid amount: {0}", e.Arguments[1]);
                return;
            }

            PlayerMobile target = e.Mobile as PlayerMobile;

            // Target selection
            if (e.Mobile.Target != null)
            {
                e.Mobile.SendMessage("Please target a player.");
                return;
            }

            var manager = VystiaResourceManager.GetManager(target);
            if (manager == null)
            {
                e.Mobile.SendMessage("Failed to get resource manager.");
                return;
            }

            // Ensure resource exists
            if (!manager.HasResource(type))
                manager.AddResource(type);

            var resource = manager.GetResource(type);
            if (resource != null)
            {
                resource.Current = amount;
                e.Mobile.SendMessage("{0} set to {1}/{2}", type, resource.Current, resource.Maximum);
            }
        }

        [Usage("GetResources")]
        [Description("Display all resource values.")]
        private static void GetResources_OnCommand(CommandEventArgs e)
        {
            if (e.Mobile is PlayerMobile pm)
            {
                var manager = VystiaResourceManager.GetManager(pm);
                manager?.SendResourceInfo();
            }
        }

        [Usage("ResetResources")]
        [Description("Reset all resources to zero.")]
        private static void ResetResources_OnCommand(CommandEventArgs e)
        {
            if (e.Mobile is PlayerMobile pm)
            {
                var manager = VystiaResourceManager.GetManager(pm);
                if (manager != null)
                {
                    foreach (var resource in manager.AllResources)
                        resource.Reset();

                    e.Mobile.SendMessage("All resources reset.");
                }
            }
        }

        [Usage("TestResource <type>")]
        [Description("Add a resource type for testing.")]
        private static void TestResource_OnCommand(CommandEventArgs e)
        {
            if (e.Arguments.Length < 1)
            {
                e.Mobile.SendMessage("Usage: TestResource <type>");
#if VYSTIA_CRESCENDO
                e.Mobile.SendMessage("Types: SoulShards, LifeForce, ChillStacks, Fury, Chi, ComboPoints, Focus, Zeal, Fortitude, Pursuit, Steam, Charges, Faith, Crescendo, Virtues");
#else
                e.Mobile.SendMessage("Types: SoulShards, LifeForce, ChillStacks, Fury, Chi, ComboPoints, Focus, Zeal, Fortitude, Pursuit, Steam, Charges, Faith, Virtues");
#endif
                return;
            }

            if (!Enum.TryParse<ResourceType>(e.Arguments[0], true, out var type))
            {
                e.Mobile.SendMessage("Invalid resource type: {0}", e.Arguments[0]);
                return;
            }

            if (e.Mobile is PlayerMobile pm)
            {
                var manager = VystiaResourceManager.GetManager(pm);
                if (manager != null)
                {
                    manager.AddResource(type);
                    e.Mobile.SendMessage("Added resource: {0}", type);
                    manager.SendResourceInfo();
                }
            }
        }
    }

    #endregion
}
