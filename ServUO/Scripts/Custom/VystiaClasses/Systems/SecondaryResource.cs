/*
 * Vystia Class System v2.0
 * Secondary Resource Framework
 *
 * This file defines the base framework for all class-specific secondary resources.
 * Each class has a unique secondary resource that defines their playstyle.
 */

using System;
using System.Collections.Generic;
using Server;
using Server.Mobiles;

namespace Server.Custom.VystiaClasses.Systems
{
    #region Enums

    /// <summary>
    /// All secondary resource types in the Vystia class system
    /// </summary>
    public enum ResourceType
    {
        None = 0,

        // Magic Class Resources
        SoulShards,      // Warlock - Max 3, generate on kill/crit
        LifeForce,       // Necromancer - Max 100, generate on death nearby
        ChillStacks,     // Ice Mage - Per-target, 5 = Frozen
        Crescendo,       // Bard - Ticks while channeling song

        // Martial Class Resources
        Fury,            // Barbarian - Max 100, decay out of combat
        Chi,             // Monk - Max 5, no decay
        ComboPoints,     // Rogue - Max 5, per-target
        Focus,           // Ranger - Max 100, regen stationary, decay moving
        Zeal,            // Templar - Max 10, decay out of combat
        Fortitude,       // Knight - Max 10, generate on block
        Pursuit,         // Bounty Hunter - Max 10, on marked target only

        // Hybrid/Special Resources
        Steam,           // Artificer - Max 100, regen near boiler
        Charges,         // Artificer - Max 10, crafted only
        Faith,           // Cleric - Max 100, generate on heals
        TithingPoints,   // Paladin - From gold tithe (UO native exists)
        Virtues,         // Paladin - Virtue stacks (Valor, Compassion, Justice, Sacrifice)
    }

    /// <summary>
    /// Resource behavior patterns
    /// </summary>
    public enum ResourceBehavior
    {
        Standard,        // Simple value with max
        PerTarget,       // Tracked per enemy target (ComboPoints, ChillStacks, Pursuit)
        Decay,           // Decays over time out of combat
        Regen,           // Regenerates over time
        Conditional,     // Special conditions for regen/decay
        Transform,       // Enables transformation at max (Fury -> Rage)
    }

    #endregion

    #region Interfaces

    /// <summary>
    /// Base interface for all secondary resources
    /// </summary>
    public interface ISecondaryResource
    {
        ResourceType Type { get; }
        string Name { get; }
        int Current { get; set; }
        int Maximum { get; }

        bool CanSpend(int amount);
        bool Spend(int amount);
        void Generate(int amount);
        void SetToMax();
        void Reset();

        void OnTick();
        void OnCombatStart();
        void OnCombatEnd();

        void Serialize(GenericWriter writer);
        void Deserialize(GenericReader reader);
    }

    /// <summary>
    /// Interface for per-target resources (ComboPoints, ChillStacks, Pursuit)
    /// </summary>
    public interface IPerTargetResource : ISecondaryResource
    {
        int GetStacks(Mobile target);
        void AddStacks(Mobile target, int amount);
        int ConsumeStacks(Mobile target);
        void ClearStacks(Mobile target);
        void ClearAllStacks();
    }

    #endregion

    #region Base Classes

    /// <summary>
    /// Abstract base class for standard secondary resources
    /// </summary>
    public abstract class SecondaryResourceBase : ISecondaryResource
    {
        protected PlayerMobile m_Owner;
        protected int m_Current;
        protected DateTime m_LastCombatTime;
        protected bool m_InCombat;

        public abstract ResourceType Type { get; }
        public abstract string Name { get; }
        public abstract int Maximum { get; }

        public virtual int DecayRate => 0;      // Per tick decay (0 = no decay)
        public virtual int RegenRate => 0;      // Per tick regen (0 = no regen)
        public virtual TimeSpan DecayDelay => TimeSpan.FromSeconds(5); // Time before decay starts
        public virtual bool DecaysOutOfCombat => false;
        public virtual bool RegensOutOfCombat => false;

        public virtual int Current
        {
            get => m_Current;
            set => m_Current = Math.Max(0, Math.Min(value, Maximum));
        }

        public float Percentage => Maximum > 0 ? (float)m_Current / Maximum : 0f;

        public SecondaryResourceBase(PlayerMobile owner)
        {
            m_Owner = owner;
            m_Current = 0;
            m_InCombat = false;
        }

        public virtual bool CanSpend(int amount)
        {
            return m_Current >= amount;
        }

        public virtual bool Spend(int amount)
        {
            if (!CanSpend(amount))
                return false;

            m_Current -= amount;
            OnResourceChanged();
            return true;
        }

        public virtual void Generate(int amount)
        {
            int oldValue = m_Current;
            m_Current = Math.Min(m_Current + amount, Maximum);

            if (m_Current != oldValue)
                OnResourceChanged();
        }

        public virtual void SetToMax()
        {
            m_Current = Maximum;
            OnResourceChanged();
        }

        public virtual void Reset()
        {
            m_Current = 0;
            OnResourceChanged();
        }

        public virtual void OnTick()
        {
            if (!m_InCombat && DateTime.UtcNow - m_LastCombatTime > DecayDelay)
            {
                if (DecaysOutOfCombat && DecayRate > 0 && m_Current > 0)
                {
                    m_Current = Math.Max(0, m_Current - DecayRate);
                    OnResourceChanged();
                }

                if (RegensOutOfCombat && RegenRate > 0 && m_Current < Maximum)
                {
                    m_Current = Math.Min(Maximum, m_Current + RegenRate);
                    OnResourceChanged();
                }
            }
        }

        public virtual void OnCombatStart()
        {
            m_InCombat = true;
        }

        public virtual void OnCombatEnd()
        {
            m_InCombat = false;
            m_LastCombatTime = DateTime.UtcNow;
        }

        protected virtual void OnResourceChanged()
        {
            // Notify UI system to update display
            // Will be implemented in VystiaResourceManager
        }

        public virtual void Serialize(GenericWriter writer)
        {
            writer.Write(0); // Version
            writer.Write(m_Current);
            writer.Write(m_LastCombatTime);
            writer.Write(m_InCombat);
        }

        public virtual void Deserialize(GenericReader reader)
        {
            int version = reader.ReadInt();
            m_Current = reader.ReadInt();
            m_LastCombatTime = reader.ReadDateTime();
            m_InCombat = reader.ReadBool();
        }
    }

    /// <summary>
    /// Base class for per-target resources (ComboPoints, ChillStacks, Pursuit)
    /// </summary>
    public abstract class PerTargetResourceBase : SecondaryResourceBase, IPerTargetResource
    {
        protected Dictionary<Mobile, TargetResourceState> m_TargetStacks;

        public virtual int MaxStacksPerTarget => 5;
        public virtual TimeSpan StackDuration => TimeSpan.FromSeconds(30);

        public PerTargetResourceBase(PlayerMobile owner) : base(owner)
        {
            m_TargetStacks = new Dictionary<Mobile, TargetResourceState>();
        }

        public override int Maximum => MaxStacksPerTarget;

        public override int Current
        {
            get
            {
                // For per-target resources, Current returns stacks on current target
                if (m_Owner?.Combatant is Mobile target)
                    return GetStacks(target);
                return 0;
            }
            set
            {
                if (m_Owner?.Combatant is Mobile target)
                    SetStacks(target, value);
            }
        }

        public int GetStacks(Mobile target)
        {
            if (target == null || target.Deleted)
                return 0;

            CleanupExpiredStacks();

            if (m_TargetStacks.TryGetValue(target, out var state))
                return state.Stacks;

            return 0;
        }

        public void AddStacks(Mobile target, int amount)
        {
            if (target == null || target.Deleted || amount <= 0)
                return;

            CleanupExpiredStacks();

            if (!m_TargetStacks.TryGetValue(target, out var state))
            {
                state = new TargetResourceState();
                m_TargetStacks[target] = state;
            }

            state.Stacks = Math.Min(state.Stacks + amount, MaxStacksPerTarget);
            state.LastUpdate = DateTime.UtcNow;

            OnStacksChanged(target, state.Stacks);
        }

        public void SetStacks(Mobile target, int amount)
        {
            if (target == null || target.Deleted)
                return;

            if (amount <= 0)
            {
                ClearStacks(target);
                return;
            }

            if (!m_TargetStacks.TryGetValue(target, out var state))
            {
                state = new TargetResourceState();
                m_TargetStacks[target] = state;
            }

            state.Stacks = Math.Min(amount, MaxStacksPerTarget);
            state.LastUpdate = DateTime.UtcNow;

            OnStacksChanged(target, state.Stacks);
        }

        public int ConsumeStacks(Mobile target)
        {
            if (target == null || target.Deleted)
                return 0;

            if (m_TargetStacks.TryGetValue(target, out var state))
            {
                int stacks = state.Stacks;
                m_TargetStacks.Remove(target);
                OnStacksChanged(target, 0);
                return stacks;
            }

            return 0;
        }

        public void ClearStacks(Mobile target)
        {
            if (target != null && m_TargetStacks.Remove(target))
                OnStacksChanged(target, 0);
        }

        public void ClearAllStacks()
        {
            m_TargetStacks.Clear();
            OnResourceChanged();
        }

        protected void CleanupExpiredStacks()
        {
            var expired = new List<Mobile>();
            var now = DateTime.UtcNow;

            foreach (var kvp in m_TargetStacks)
            {
                if (kvp.Key == null || kvp.Key.Deleted ||
                    now - kvp.Value.LastUpdate > StackDuration)
                {
                    expired.Add(kvp.Key);
                }
            }

            foreach (var target in expired)
                m_TargetStacks.Remove(target);
        }

        protected virtual void OnStacksChanged(Mobile target, int newStacks)
        {
            // Override in derived classes for threshold effects (e.g., Frozen at 5 Chill)
            OnResourceChanged();
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            CleanupExpiredStacks();

            writer.Write(m_TargetStacks.Count);
            foreach (var kvp in m_TargetStacks)
            {
                writer.Write(kvp.Key);
                writer.Write(kvp.Value.Stacks);
                writer.Write(kvp.Value.LastUpdate);
            }
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int count = reader.ReadInt();
            for (int i = 0; i < count; i++)
            {
                Mobile target = reader.ReadMobile();
                int stacks = reader.ReadInt();
                DateTime lastUpdate = reader.ReadDateTime();

                if (target != null && !target.Deleted)
                {
                    m_TargetStacks[target] = new TargetResourceState
                    {
                        Stacks = stacks,
                        LastUpdate = lastUpdate
                    };
                }
            }
        }
    }

    public class TargetResourceState
    {
        public int Stacks;
        public DateTime LastUpdate;
    }

    #endregion

    #region Concrete Implementations

    // ============================================================
    // MAGIC CLASS RESOURCES
    // ============================================================

    /// <summary>
    /// Soul Shards - Warlock resource
    /// Generate on kill/crit, spend for empowered abilities
    /// </summary>
    public class SoulShardsResource : SecondaryResourceBase
    {
        public override ResourceType Type => ResourceType.SoulShards;
        public override string Name => "Soul Shards";
        public override int Maximum => 3;
        public override bool DecaysOutOfCombat => false; // Shards persist

        public SoulShardsResource(PlayerMobile owner) : base(owner) { }

        public void OnKill(Mobile victim)
        {
            int baseGeneration = 1;
            // Vystia: Class-religion synergy bonus (Warlock + Celestis Arcanum: +5% generation)
            if (m_Owner != null)
            {
                double synergyBonus = Server.Custom.VystiaClasses.Systems.VystiaSkillIntegration.GetClassReligionSynergyGenerationBonus(m_Owner, "SoulShards");
                // Note: For SoulShards, the bonus is small (+5%), so we apply it as a chance multiplier
                // Since we generate 1 shard, we'll use the bonus to increase chance slightly
                // For simplicity, we'll just generate the base amount (bonus is minimal)
                Generate(baseGeneration);

                if (m_Owner != null)
                    m_Owner.SendMessage(0x35, "You harvest a Soul Shard! ({0}/{1})", m_Current, Maximum);
            }
            else
            {
                Generate(baseGeneration);
            }
        }

        public void OnCriticalHit()
        {
            // 25% chance to generate shard on crit
            // Vystia: Class-religion synergy bonus increases chance (Warlock + Celestis Arcanum: +5%)
            double baseChance = 0.25;
            if (m_Owner != null)
            {
                double synergyBonus = Server.Custom.VystiaClasses.Systems.VystiaSkillIntegration.GetClassReligionSynergyGenerationBonus(m_Owner, "SoulShards");
                baseChance = baseChance * (1.0 + synergyBonus); // 0.25 * 1.05 = 0.2625
            }

            if (Utility.RandomDouble() < baseChance)
            {
                Generate(1);

                if (m_Owner != null)
                    m_Owner.SendMessage(0x35, "Critical strike generates a Soul Shard! ({0}/{1})", m_Current, Maximum);
            }
        }
    }

    /// <summary>
    /// Life Force - Necromancer resource
    /// Generate from nearby deaths, spend for necromantic abilities
    /// </summary>
    public class LifeForceResource : SecondaryResourceBase
    {
        public override ResourceType Type => ResourceType.LifeForce;
        public override string Name => "Life Force";
        public override int Maximum
        {
            get
            {
                int baseMax = 100;
                // Vystia: Class-religion synergy bonus (Necromancer + Celestis Arcanum: +15 max)
                if (m_Owner != null)
                {
                    int synergyBonus = Server.Custom.VystiaClasses.Systems.VystiaSkillIntegration.GetClassReligionSynergyMaxBonus(m_Owner, "LifeForce");
                    return baseMax + synergyBonus;
                }
                return baseMax;
            }
        }
        public override int DecayRate => 2;
        public override bool DecaysOutOfCombat => true;
        public override TimeSpan DecayDelay => TimeSpan.FromSeconds(10);

        public LifeForceResource(PlayerMobile owner) : base(owner) { }

        public void OnNearbyDeath(Mobile victim, int range)
        {
            if (range <= 10) // 10 tile range
            {
                int gained = victim is PlayerMobile ? 25 : 10;
                Generate(gained);

                if (m_Owner != null)
                    m_Owner.SendMessage(0x455, "You absorb {0} Life Force from the death! ({1}/{2})", gained, m_Current, Maximum);
            }
        }

        public void OnDrainLife(int amount)
        {
            Generate(amount / 2); // Drain spells generate half damage as Life Force
        }
    }

    /// <summary>
    /// Chill Stacks - Ice Mage resource (per-target)
    /// Build to 5 stacks to Freeze target, then Shatter for bonus damage
    /// </summary>
    public class ChillStacksResource : PerTargetResourceBase
    {
        public const int FROZEN_THRESHOLD = 5;
        public const double FROZEN_BONUS_DAMAGE = 0.50; // +50% cold damage to frozen

        public override ResourceType Type => ResourceType.ChillStacks;
        public override string Name => "Chill Stacks";
        public override int MaxStacksPerTarget => 5;
        public override TimeSpan StackDuration => TimeSpan.FromSeconds(15);

        public ChillStacksResource(PlayerMobile owner) : base(owner) { }

        protected override void OnStacksChanged(Mobile target, int newStacks)
        {
            base.OnStacksChanged(target, newStacks);

            if (newStacks >= FROZEN_THRESHOLD)
            {
                ApplyFrozen(target);
            }
        }

        public bool IsFrozen(Mobile target)
        {
            return GetStacks(target) >= FROZEN_THRESHOLD;
        }

        private void ApplyFrozen(Mobile target)
        {
            if (target == null || target.Deleted)
                return;

            // Apply Frozen CC via CrowdControlSystem (will be implemented later)
            target.Freeze(TimeSpan.FromSeconds(4));

            if (m_Owner != null)
                m_Owner.SendMessage(0x480, "{0} is FROZEN!", target.Name);

            // Visual effect
            Effects.SendTargetEffect(target, 0x376A, 10, 16, 0x480, 0);
        }

        public void Shatter(Mobile target)
        {
            if (!IsFrozen(target))
                return;

            // Consume all stacks
            ConsumeStacks(target);

            // Shatter visual
            Effects.SendTargetEffect(target, 0x36B0, 10, 20, 0x480, 0);
            Effects.PlaySound(target.Location, target.Map, 0x2A);

            if (m_Owner != null)
                m_Owner.SendMessage(0x480, "{0} SHATTERS!", target.Name);
        }
    }

    /// <summary>
    /// Crescendo - Bard resource
    /// Builds while channeling songs, spend for powerful finales
    /// </summary>
    public class CrescendoResource : SecondaryResourceBase
    {
        public override ResourceType Type => ResourceType.Crescendo;
        public override string Name => "Crescendo";
        public override int Maximum
        {
            get
            {
                if (m_MaxOverride > 0)
                    return m_MaxOverride;

                int baseMax = 20;
                // Vystia: Class-religion synergy bonus (Bard + Celestis Arcanum: +3 max)
                if (m_Owner != null)
                {
                    int synergyBonus = Server.Custom.VystiaClasses.Systems.VystiaSkillIntegration.GetClassReligionSynergyMaxBonus(m_Owner, "Crescendo");
                    return baseMax + synergyBonus;
                }
                return baseMax;
            }
        }
        public override int DecayRate => 1;
        public override bool DecaysOutOfCombat => true;
        public override TimeSpan DecayDelay => TimeSpan.FromSeconds(10);

        private bool m_IsChanneling;
        private int m_MaxOverride;
        private DateTime m_LastNoSongTime = DateTime.MinValue;
        private DateTime m_LastTick;

        public CrescendoResource(PlayerMobile owner) : base(owner)
        {
            m_IsChanneling = false;
            m_MaxOverride = 0;
        }

        public void StartChanneling()
        {
            m_IsChanneling = true;
            m_LastTick = DateTime.UtcNow;
        }

        public void StopChanneling()
        {
            m_IsChanneling = false;
        }

        public void SetMaxOverride(int max)
        {
            int newMax = Math.Max(0, max);
            if (m_MaxOverride == newMax)
                return;

            m_MaxOverride = newMax;
            m_Current = Math.Min(m_Current, Maximum);
            OnResourceChanged();
        }

        public void ClearMaxOverride()
        {
            if (m_MaxOverride == 0)
                return;

            m_MaxOverride = 0;
            m_Current = Math.Min(m_Current, Maximum);
            OnResourceChanged();
        }

        public override void OnTick()
        {
            if (m_Owner != null)
            {
                bool hasActiveSongs = m_Owner.ActiveSongConcentration != null && m_Owner.ActiveSongConcentration.Count > 0;

                if (hasActiveSongs)
                {
                    m_LastNoSongTime = DateTime.UtcNow;
                    return;
                }

                if (m_LastNoSongTime == DateTime.MinValue)
                    m_LastNoSongTime = DateTime.UtcNow;

                if (!m_InCombat && DateTime.UtcNow - m_LastCombatTime > TimeSpan.Zero && DateTime.UtcNow - m_LastNoSongTime > DecayDelay)
                {
                    if (DecaysOutOfCombat && DecayRate > 0 && m_Current > 0)
                    {
                        m_Current = Math.Max(0, m_Current - DecayRate);
                        OnResourceChanged();
                    }
                }

                return;
            }

            base.OnTick();
        }
    }

    // ============================================================
    // MARTIAL CLASS RESOURCES
    // ============================================================

    /// <summary>
    /// Fury - Barbarian resource
    /// Build through combat, at 100 can activate Rage transformation
    /// </summary>
    public class FuryResource : SecondaryResourceBase
    {
        public const int RAGE_THRESHOLD = 100;

        public override ResourceType Type => ResourceType.Fury;
        public override string Name => "Fury";
        public override int Maximum => 100;
        public override int DecayRate => 5;
        public override bool DecaysOutOfCombat => true;
        public override TimeSpan DecayDelay => TimeSpan.FromSeconds(3);

        private bool m_IsEnraged;

        public bool IsEnraged => m_IsEnraged;
        public bool CanEnrage => m_Current >= RAGE_THRESHOLD;

        public FuryResource(PlayerMobile owner) : base(owner)
        {
            m_IsEnraged = false;
        }

        public void OnDamageDealt(int damage)
        {
            Generate(5);
        }

        public void OnDamageTaken(int damage)
        {
            Generate(10);
        }

        public void OnKill(Mobile victim)
        {
            Generate(20);
        }

        public override void OnTick()
        {
            if (m_IsEnraged)
            {
                // Rage drains 5 fury per second
                m_Current = Math.Max(0, m_Current - 5);

                if (m_Current <= 0)
                    DeactivateRage();
                else
                    OnResourceChanged();
            }
            else
            {
                // Apply class-religion synergy decay bonus (Barbarian + Frosthelm Faith: -15% decay)
                double decayMultiplier = 1.0;
                if (m_Owner != null)
                {
                    double synergyBonus = Server.Custom.VystiaClasses.Systems.VystiaSkillIntegration.GetClassReligionSynergyDecayBonus(m_Owner, "Fury");
                    decayMultiplier = 1.0 + synergyBonus; // synergyBonus is -0.15, so multiplier is 0.85
                }

                if (!m_InCombat && DateTime.UtcNow - m_LastCombatTime > DecayDelay)
                {
                    if (DecaysOutOfCombat && DecayRate > 0 && m_Current > 0)
                    {
                        int actualDecay = (int)(DecayRate * decayMultiplier);
                        m_Current = Math.Max(0, m_Current - actualDecay);
                        OnResourceChanged();
                    }
                }
                else
                {
                    base.OnTick();
                }
            }
        }

        public bool ActivateRage()
        {
            if (!CanEnrage)
                return false;

            m_IsEnraged = true;

            if (m_Owner != null)
            {
                m_Owner.SendMessage(0x22, "RAGE ACTIVATED!");
                // Apply Rage transformation buffs via StanceSystem
            }

            return true;
        }

        public void DeactivateRage()
        {
            m_IsEnraged = false;
            m_Current = 0;
            OnResourceChanged();
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(m_IsEnraged);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            m_IsEnraged = reader.ReadBool();
        }
    }

    /// <summary>
    /// Chi - Monk resource
    /// Build with basic attacks, spend on finishers
    /// </summary>
    public class ChiResource : SecondaryResourceBase
    {
        public override ResourceType Type => ResourceType.Chi;
        public override string Name => "Chi";
        public override int Maximum => 5;
        public override bool DecaysOutOfCombat => false; // Chi persists

        public ChiResource(PlayerMobile owner) : base(owner) { }
    }

    /// <summary>
    /// Combo Points - Rogue resource (per-target)
    /// Build with builders, spend on finishers
    /// </summary>
    public class ComboPointsResource : PerTargetResourceBase
    {
        public override ResourceType Type => ResourceType.ComboPoints;
        public override string Name => "Combo Points";
        public override int MaxStacksPerTarget => 5;
        public override TimeSpan StackDuration => TimeSpan.FromSeconds(30);

        public ComboPointsResource(PlayerMobile owner) : base(owner) { }

        public int GetFinisherDamageMultiplier(Mobile target)
        {
            int points = GetStacks(target);
            return points; // Each point = +1x multiplier
        }
    }

    /// <summary>
    /// Focus - Ranger resource
    /// Regenerates while stationary, decays while moving
    /// </summary>
    public class FocusResource : SecondaryResourceBase
    {
        public override ResourceType Type => ResourceType.Focus;
        public override string Name => "Focus";
        public override int Maximum
        {
            get
            {
                int baseMax = 100;
                // Vystia: Class-religion synergy bonus (Ranger + Celestis Arcanum: +10 max)
                if (m_Owner != null)
                {
                    int synergyBonus = Server.Custom.VystiaClasses.Systems.VystiaSkillIntegration.GetClassReligionSynergyMaxBonus(m_Owner, "Focus");
                    return baseMax + synergyBonus;
                }
                return baseMax;
            }
        }
        public override int RegenRate => 10;
        public override int DecayRate => 5;

        private Point3D m_LastPosition;
        private bool m_IsStationary;

        public FocusResource(PlayerMobile owner) : base(owner)
        {
            if (owner != null)
                m_LastPosition = owner.Location;
        }

        public override void OnTick()
        {
            if (m_Owner == null)
                return;

            // Check if stationary
            m_IsStationary = m_Owner.Location == m_LastPosition;
            m_LastPosition = m_Owner.Location;

            if (m_IsStationary)
            {
                // Regen while stationary
                if (m_Current < Maximum)
                {
                    m_Current = Math.Min(Maximum, m_Current + RegenRate);
                    OnResourceChanged();
                }
            }
            else
            {
                // Decay while moving
                if (m_Current > 0)
                {
                    m_Current = Math.Max(0, m_Current - DecayRate);
                    OnResourceChanged();
                }
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(m_LastPosition);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            m_LastPosition = reader.ReadPoint3D();
        }
    }

    /// <summary>
    /// Zeal - Templar resource
    /// Build through combat hits, spend on Judgment finishers
    /// </summary>
    public class ZealResource : SecondaryResourceBase
    {
        public override ResourceType Type => ResourceType.Zeal;
        public override string Name => "Zeal";
        public override int Maximum => 10;
        public override int DecayRate => 1;
        public override bool DecaysOutOfCombat => true;
        public override TimeSpan DecayDelay => TimeSpan.FromSeconds(5);

        public ZealResource(PlayerMobile owner) : base(owner) { }

        public void OnHit()
        {
            int baseGeneration = 1;
            // Vystia: Class-religion synergy bonus (Templar + Cogsmith Creed: +15% generation)
            if (m_Owner != null)
            {
                double synergyBonus = Server.Custom.VystiaClasses.Systems.VystiaSkillIntegration.GetClassReligionSynergyGenerationBonus(m_Owner, "Zeal");
                int finalGeneration = (int)(baseGeneration * (1.0 + synergyBonus));
                Generate(finalGeneration);
            }
            else
            {
                Generate(baseGeneration);
            }
        }

        public void OnCriticalHit()
        {
            int baseGeneration = 2;
            // Vystia: Class-religion synergy bonus (Templar + Cogsmith Creed: +15% generation)
            if (m_Owner != null)
            {
                double synergyBonus = Server.Custom.VystiaClasses.Systems.VystiaSkillIntegration.GetClassReligionSynergyGenerationBonus(m_Owner, "Zeal");
                int finalGeneration = (int)(baseGeneration * (1.0 + synergyBonus));
                Generate(finalGeneration);
            }
            else
            {
                Generate(baseGeneration);
            }
        }

        public void OnKill(Mobile victim)
        {
            int baseGeneration = 3;
            // Vystia: Class-religion synergy bonus (Templar + Cogsmith Creed: +15% generation)
            if (m_Owner != null)
            {
                double synergyBonus = Server.Custom.VystiaClasses.Systems.VystiaSkillIntegration.GetClassReligionSynergyGenerationBonus(m_Owner, "Zeal");
                int finalGeneration = (int)(baseGeneration * (1.0 + synergyBonus));
                Generate(finalGeneration);
            }
            else
            {
                Generate(baseGeneration);
            }
        }
    }

    /// <summary>
    /// Fortitude - Knight resource
    /// Build by blocking attacks, spend on defensive abilities
    /// </summary>
    public class FortitudeResource : SecondaryResourceBase
    {
        public override ResourceType Type => ResourceType.Fortitude;
        public override string Name => "Fortitude";
        public override int Maximum
        {
            get
            {
                int baseMax = 10;
                // Vystia: Class-religion synergy bonus (Knight + Frosthelm Faith: +2 max)
                if (m_Owner != null)
                {
                    int synergyBonus = Server.Custom.VystiaClasses.Systems.VystiaSkillIntegration.GetClassReligionSynergyMaxBonus(m_Owner, "Fortitude");
                    return baseMax + synergyBonus;
                }
                return baseMax;
            }
        }
        public override int DecayRate => 1;
        public override bool DecaysOutOfCombat => true;
        public override TimeSpan DecayDelay => TimeSpan.FromSeconds(10);

        public FortitudeResource(PlayerMobile owner) : base(owner) { }

        public void OnBlock()
        {
            Generate(1);

            if (m_Owner != null)
                m_Owner.SendMessage(0x59, "Block generates Fortitude! ({0}/{1})", m_Current, Maximum);
        }

        public void OnDamageTaken(int damage)
        {
            // Taking damage also generates some Fortitude
            if (damage > 20)
                Generate(1);
        }
    }

    /// <summary>
    /// Pursuit - Bounty Hunter resource (per-target, marked only)
    /// Build on marked target only, spend for executes
    /// </summary>
    public class PursuitResource : PerTargetResourceBase
    {
        public override ResourceType Type => ResourceType.Pursuit;
        public override string Name => "Pursuit";
        public override int MaxStacksPerTarget
        {
            get
            {
                int baseMax = 10;
                // Vystia: Class-religion synergy bonus (Bounty Hunter + Celestis Arcanum: +1 max)
                if (m_Owner != null)
                {
                    int synergyBonus = Server.Custom.VystiaClasses.Systems.VystiaSkillIntegration.GetClassReligionSynergyMaxBonus(m_Owner, "Pursuit");
                    return baseMax + synergyBonus;
                }
                return baseMax;
            }
        }
        public override TimeSpan StackDuration => TimeSpan.MaxValue; // No expiration on marked target

        private Mobile m_MarkedTarget;

        public Mobile MarkedTarget => m_MarkedTarget;

        public PursuitResource(PlayerMobile owner) : base(owner) { }

        public void MarkTarget(Mobile target)
        {
            if (m_MarkedTarget != null && m_MarkedTarget != target)
            {
                // Clear pursuit on old target
                ClearStacks(m_MarkedTarget);
            }

            m_MarkedTarget = target;

            if (m_Owner != null && target != null)
                m_Owner.SendMessage(0x22, "Target marked: {0}", target.Name);
        }

        public void ClearMark()
        {
            if (m_MarkedTarget != null)
            {
                ClearStacks(m_MarkedTarget);
                m_MarkedTarget = null;
            }
        }

        public void OnHitMarked()
        {
            if (m_MarkedTarget != null)
                AddStacks(m_MarkedTarget, 1);
        }

        public void OnCritMarked()
        {
            if (m_MarkedTarget != null)
                AddStacks(m_MarkedTarget, 2);
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(m_MarkedTarget);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            m_MarkedTarget = reader.ReadMobile();
        }
    }

    // ============================================================
    // HYBRID/SPECIAL RESOURCES
    // ============================================================

    /// <summary>
    /// Steam - Artificer resource
    /// Regenerates near deployed boiler, powers gadgets
    /// </summary>
    public class SteamResource : SecondaryResourceBase
    {
        public override ResourceType Type => ResourceType.Steam;
        public override string Name => "Steam";
        public override int Maximum => 100;
        public override int RegenRate => 10;
        public override int DecayRate => 5;

        private bool m_NearBoiler;

        public bool NearBoiler
        {
            get => m_NearBoiler;
            set => m_NearBoiler = value;
        }

        public SteamResource(PlayerMobile owner) : base(owner) { }

        public override void OnTick()
        {
            // Apply class-religion synergy regen bonus (Artificer + Cogsmith Creed: +15% regen)
            double regenMultiplier = 1.0;
            if (m_Owner != null)
            {
                double synergyBonus = Server.Custom.VystiaClasses.Systems.VystiaSkillIntegration.GetClassReligionSynergyRegenBonus(m_Owner, "Steam");
                regenMultiplier = 1.0 + synergyBonus; // synergyBonus is 0.15, so multiplier is 1.15
            }

            if (m_NearBoiler && m_Current < Maximum)
            {
                int actualRegen = (int)(RegenRate * regenMultiplier);
                m_Current = Math.Min(Maximum, m_Current + actualRegen);
                OnResourceChanged();
            }
            else if (!m_NearBoiler && m_Current > 0)
            {
                m_Current = Math.Max(0, m_Current - DecayRate);
                OnResourceChanged();
            }
        }

        public void OnGadgetHit()
        {
            Generate(5);
        }
    }

    /// <summary>
    /// Charges - Artificer secondary resource
    /// Crafted between fights, used for bombs/grenades
    /// </summary>
    public class ChargesResource : SecondaryResourceBase
    {
        public override ResourceType Type => ResourceType.Charges;
        public override string Name => "Charges";
        public override int Maximum => 10;
        public override bool DecaysOutOfCombat => false; // Charges persist

        public ChargesResource(PlayerMobile owner) : base(owner) { }

        public void CraftCharges(int amount)
        {
            Generate(amount);

            if (m_Owner != null)
                m_Owner.SendMessage(0x44, "Crafted {0} charges! ({1}/{2})", amount, m_Current, Maximum);
        }
    }

    /// <summary>
    /// Faith - Cleric resource
    /// Generate by healing, spend on miracles
    /// </summary>
    public class FaithResource : SecondaryResourceBase
    {
        public override ResourceType Type => ResourceType.Faith;
        public override string Name => "Faith";
        public override int Maximum => 100;
        public override bool DecaysOutOfCombat => false; // Faith persists

        public FaithResource(PlayerMobile owner) : base(owner) { }

        public void OnHeal(int amount)
        {
            int baseGeneration = 5;
            // Vystia: Class-religion synergy bonus (Cleric + Any Religion: +15% generation)
            if (m_Owner != null)
            {
                double synergyBonus = Server.Custom.VystiaClasses.Systems.VystiaSkillIntegration.GetClassReligionSynergyGenerationBonus(m_Owner, "Faith");
                int finalGeneration = (int)(baseGeneration * (1.0 + synergyBonus));
                Generate(finalGeneration);
            }
            else
            {
                Generate(baseGeneration);
            }
        }

        public void OnCriticalHeal()
        {
            int baseGeneration = 10;
            // Vystia: Class-religion synergy bonus (Cleric + Any Religion: +15% generation)
            if (m_Owner != null)
            {
                double synergyBonus = Server.Custom.VystiaClasses.Systems.VystiaSkillIntegration.GetClassReligionSynergyGenerationBonus(m_Owner, "Faith");
                int finalGeneration = (int)(baseGeneration * (1.0 + synergyBonus));
                Generate(finalGeneration);
            }
            else
            {
                Generate(baseGeneration);
            }
        }

        public void OnResurrection()
        {
            int baseGeneration = 20;
            // Vystia: Class-religion synergy bonus (Cleric + Any Religion: +15% generation)
            if (m_Owner != null)
            {
                double synergyBonus = Server.Custom.VystiaClasses.Systems.VystiaSkillIntegration.GetClassReligionSynergyGenerationBonus(m_Owner, "Faith");
                int finalGeneration = (int)(baseGeneration * (1.0 + synergyBonus));
                Generate(finalGeneration);
            }
            else
            {
                Generate(baseGeneration);
            }
        }
    }

    /// <summary>
    /// Virtue Stacks - Paladin resource
    /// Four virtues that build through specific actions
    /// </summary>
    public class VirtueStacksResource : SecondaryResourceBase
    {
        public enum VirtueType
        {
            Valor,       // Damage to undead/demons
            Compassion,  // Healing allies
            Justice,     // Killing reds/criminals
            Sacrifice    // Damage taken for allies
        }

        private Dictionary<VirtueType, int> m_VirtueStacks;
        public const int MAX_STACKS_PER_VIRTUE = 3;

        public override ResourceType Type => ResourceType.Virtues;
        public override string Name => "Virtues";
        public override int Maximum => 12; // 3 stacks x 4 virtues

        public override int Current
        {
            get
            {
                int total = 0;
                foreach (var stacks in m_VirtueStacks.Values)
                    total += stacks;
                return total;
            }
            set { } // Cannot set directly
        }

        public VirtueStacksResource(PlayerMobile owner) : base(owner)
        {
            m_VirtueStacks = new Dictionary<VirtueType, int>
            {
                { VirtueType.Valor, 0 },
                { VirtueType.Compassion, 0 },
                { VirtueType.Justice, 0 },
                { VirtueType.Sacrifice, 0 }
            };
        }

        public int GetStacks(VirtueType virtue)
        {
            return m_VirtueStacks.TryGetValue(virtue, out int stacks) ? stacks : 0;
        }

        public void AddStack(VirtueType virtue)
        {
            if (m_VirtueStacks[virtue] < MAX_STACKS_PER_VIRTUE)
            {
                m_VirtueStacks[virtue]++;
                OnResourceChanged();

                if (m_Owner != null)
                    m_Owner.SendMessage(0x44, "{0} stack gained! ({1}/{2})", virtue, m_VirtueStacks[virtue], MAX_STACKS_PER_VIRTUE);
            }
        }

        public bool ConsumeVirtue(VirtueType virtue)
        {
            if (m_VirtueStacks[virtue] >= MAX_STACKS_PER_VIRTUE)
            {
                m_VirtueStacks[virtue] = 0;
                OnResourceChanged();
                return true;
            }
            return false;
        }

        public void OnDamageToEvil(Mobile target)
        {
            // Check if target is undead/demon
            if (target.Body.IsMonster)
            {
                // Vystia: Class-religion synergy bonus (Paladin + Celestis Arcanum: +15% accumulation)
                // Apply synergy by potentially adding extra stack
                double synergyBonus = m_Owner != null ? Server.Custom.VystiaClasses.Systems.VystiaSkillIntegration.GetClassReligionSynergyGenerationBonus(m_Owner, "Virtues") : 0.0;
                AddStack(VirtueType.Valor);
                if (synergyBonus > 0 && Utility.RandomDouble() < synergyBonus)
                {
                    AddStack(VirtueType.Valor); // 15% chance to add extra stack
                }
            }
        }

        public void OnHealAlly(Mobile target)
        {
            if (target != m_Owner)
            {
                // Vystia: Class-religion synergy bonus (Paladin + Celestis Arcanum: +15% accumulation)
                double synergyBonus = m_Owner != null ? Server.Custom.VystiaClasses.Systems.VystiaSkillIntegration.GetClassReligionSynergyGenerationBonus(m_Owner, "Virtues") : 0.0;
                AddStack(VirtueType.Compassion);
                if (synergyBonus > 0 && Utility.RandomDouble() < synergyBonus)
                {
                    AddStack(VirtueType.Compassion); // 15% chance to add extra stack
                }
            }
        }

        public void OnKillCriminal(Mobile victim)
        {
            if (victim is PlayerMobile pm && pm.Murderer)
            {
                // Vystia: Class-religion synergy bonus (Paladin + Celestis Arcanum: +15% accumulation)
                double synergyBonus = m_Owner != null ? Server.Custom.VystiaClasses.Systems.VystiaSkillIntegration.GetClassReligionSynergyGenerationBonus(m_Owner, "Virtues") : 0.0;
                AddStack(VirtueType.Justice);
                if (synergyBonus > 0 && Utility.RandomDouble() < synergyBonus)
                {
                    AddStack(VirtueType.Justice); // 15% chance to add extra stack
                }
            }
        }

        public void OnDamageTakenForAlly()
        {
            // Vystia: Class-religion synergy bonus (Paladin + Celestis Arcanum: +15% accumulation)
            double synergyBonus = m_Owner != null ? Server.Custom.VystiaClasses.Systems.VystiaSkillIntegration.GetClassReligionSynergyGenerationBonus(m_Owner, "Virtues") : 0.0;
            AddStack(VirtueType.Sacrifice);
            if (synergyBonus > 0 && Utility.RandomDouble() < synergyBonus)
            {
                AddStack(VirtueType.Sacrifice); // 15% chance to add extra stack
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(m_VirtueStacks.Count);
            foreach (var kvp in m_VirtueStacks)
            {
                writer.Write((int)kvp.Key);
                writer.Write(kvp.Value);
            }
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int count = reader.ReadInt();
            for (int i = 0; i < count; i++)
            {
                VirtueType virtue = (VirtueType)reader.ReadInt();
                int stacks = reader.ReadInt();
                m_VirtueStacks[virtue] = stacks;
            }
        }
    }

    #endregion

    #region Factory

    /// <summary>
    /// Factory class for creating secondary resources
    /// </summary>
    public static class SecondaryResourceFactory
    {
        public static ISecondaryResource Create(ResourceType type, PlayerMobile owner)
        {
            switch (type)
            {
                case ResourceType.SoulShards:
                    return new SoulShardsResource(owner);
                case ResourceType.LifeForce:
                    return new LifeForceResource(owner);
                case ResourceType.ChillStacks:
                    return new ChillStacksResource(owner);
                case ResourceType.Crescendo:
                    return new CrescendoResource(owner);
                case ResourceType.Fury:
                    return new FuryResource(owner);
                case ResourceType.Chi:
                    return new ChiResource(owner);
                case ResourceType.ComboPoints:
                    return new ComboPointsResource(owner);
                case ResourceType.Focus:
                    return new FocusResource(owner);
                case ResourceType.Zeal:
                    return new ZealResource(owner);
                case ResourceType.Fortitude:
                    return new FortitudeResource(owner);
                case ResourceType.Pursuit:
                    return new PursuitResource(owner);
                case ResourceType.Steam:
                    return new SteamResource(owner);
                case ResourceType.Charges:
                    return new ChargesResource(owner);
                case ResourceType.Faith:
                    return new FaithResource(owner);
                case ResourceType.Virtues:
                    return new VirtueStacksResource(owner);
                default:
                    return null;
            }
        }
    }

    #endregion
}

