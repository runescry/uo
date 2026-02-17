// Vystia PvP Kill Faction Rewards
// Grants reputation rewards for PvP kills in contested/lawless zones

using System;
using System.Collections.Generic;
using Server;
using Server.Items;
using Server.Mobiles;
using Server.Custom.VystiaClasses.Zones;

namespace Server.Custom.VystiaClasses.Factions
{
    #region PvP Reward System

    /// <summary>
    /// System for granting faction rewards for PvP kills
    /// </summary>
    public static class VystiaPvPRewards
    {
        // Cooldown tracking to prevent reward farming
        private static readonly Dictionary<PlayerMobile, Dictionary<PlayerMobile, DateTime>> s_KillCooldowns = new Dictionary<PlayerMobile, Dictionary<PlayerMobile, DateTime>>();

        // Cooldown between rewards from same player (30 minutes)
        private static readonly TimeSpan KillCooldown = TimeSpan.FromMinutes(30);

        /// <summary>
        /// Initialize the PvP reward system
        /// </summary>
        public static void Initialize()
        {
            EventSink.PlayerDeath += OnPlayerDeath;
            Console.WriteLine("[Vystia] PvP Faction Rewards system initialized.");
        }

        /// <summary>
        /// Handle player death event
        /// </summary>
        private static void OnPlayerDeath(PlayerDeathEventArgs e)
        {
            if (e.Mobile == null || !(e.Mobile is PlayerMobile victim))
                return;

            // Get the killer
            Mobile lastKiller = victim.LastKiller;

            if (lastKiller == null)
                return;

            PlayerMobile killer = null;

            if (lastKiller is PlayerMobile pm)
            {
                killer = pm;
            }
            else if (lastKiller is BaseCreature bc && bc.ControlMaster is PlayerMobile master)
            {
                killer = master;
            }

            if (killer == null || killer == victim)
                return;

            // Check zone type - only reward in PvP zones
            var zoneType = VystiaZoneSystem.GetZoneType(killer);

            if (zoneType == VystiaZoneType.Sanctuary)
            {
                // No rewards in safe zones
                return;
            }

            // Check cooldown
            if (IsOnCooldown(killer, victim))
            {
                killer.SendMessage(0x22, "You've killed this player recently. No bonus rewards.");
                return;
            }

            // Grant rewards based on zone type
            GrantPvPRewards(killer, victim, zoneType);

            // Record cooldown
            RecordKill(killer, victim);
        }

        /// <summary>
        /// Grant PvP kill rewards
        /// </summary>
        private static void GrantPvPRewards(PlayerMobile killer, PlayerMobile victim, VystiaZoneType zoneType)
        {
            // Base rewards
            int baseReputation = 25;
            int baseTokens = 1;

            // Zone multipliers
            double zoneMultiplier = 1.0;
            switch (zoneType)
            {
                case VystiaZoneType.Contested:
                    zoneMultiplier = 1.0;
                    break;
                case VystiaZoneType.Lawless:
                    zoneMultiplier = 1.5;
                    break;
                case VystiaZoneType.Extreme:
                    zoneMultiplier = 2.0;
                    break;
            }

            // Skill difference bonus (more reward for killing higher-skilled opponents)
            double skillRatio = GetTotalSkills(victim) / Math.Max(1, GetTotalSkills(killer));
            double skillMultiplier = Math.Min(2.0, Math.Max(0.5, skillRatio));

            // Calculate final rewards
            int reputation = (int)(baseReputation * zoneMultiplier * skillMultiplier);
            int tokens = (int)(baseTokens * zoneMultiplier);

            // Determine which faction to reward based on location
            VystiaFaction faction = DetermineFactionFromLocation(killer);

            if (faction != VystiaFaction.None)
            {
                VystiaReputation.AddReputation(killer, faction, reputation, "PvP kill");
                Server.Items.Vystia.FactionTokenSystem.GiveTokens(killer, faction, tokens);

                var info = FactionData.GetInfo(faction);
                killer.SendMessage(info?.Hue ?? 0, "You gain {0} {1} reputation for your victory!", reputation, info?.Name);
            }
            else
            {
                // No specific faction - grant to killer's primary faction
                VystiaFaction primaryFaction = GetPrimaryFaction(killer);
                if (primaryFaction != VystiaFaction.None)
                {
                    VystiaReputation.AddReputation(killer, primaryFaction, reputation, "PvP kill");
                    Server.Items.Vystia.FactionTokenSystem.GiveTokens(killer, primaryFaction, tokens);

                    var info = FactionData.GetInfo(primaryFaction);
                    killer.SendMessage(info?.Hue ?? 0, "You gain {0} {1} reputation for your victory!", reputation, info?.Name);
                }
            }

            // Bonus for killing enemies of your faction
            VystiaFaction killerFaction = GetPrimaryFaction(killer);
            VystiaFaction victimFaction = GetPrimaryFaction(victim);

            if (killerFaction != VystiaFaction.None && victimFaction != VystiaFaction.None)
            {
                var killerInfo = FactionData.GetInfo(killerFaction);
                if (killerInfo != null && killerInfo.EnemyFaction == victimFaction)
                {
                    // Bonus for killing enemy faction member
                    int bonusRep = reputation / 2;
                    VystiaReputation.AddReputation(killer, killerFaction, bonusRep, "enemy faction kill");
                    killer.SendMessage(0x35, "Bonus +{0} reputation for defeating an enemy of your faction!", bonusRep);
                }
            }

            // Visual effect
            killer.FixedParticles(0x376A, 9, 32, 5030, EffectLayer.Waist);
            killer.PlaySound(0x1F2);
        }

        /// <summary>
        /// Get total skills for a player (for difficulty comparison)
        /// </summary>
        private static double GetTotalSkills(PlayerMobile pm)
        {
            if (pm == null)
                return 0;

            double total = 0;
            foreach (var skill in pm.Skills)
            {
                total += skill.Base;
            }
            return total;
        }

        /// <summary>
        /// Get player's primary faction (highest reputation)
        /// </summary>
        private static VystiaFaction GetPrimaryFaction(PlayerMobile pm)
        {
            if (pm == null)
                return VystiaFaction.None;

            VystiaFaction bestFaction = VystiaFaction.None;
            int bestRep = 0;

            foreach (VystiaFaction faction in Enum.GetValues(typeof(VystiaFaction)))
            {
                if (faction == VystiaFaction.None)
                    continue;

                int rep = VystiaReputation.GetFactionReputation(pm, faction);
                if (rep > bestRep)
                {
                    bestRep = rep;
                    bestFaction = faction;
                }
            }

            return bestFaction;
        }

        /// <summary>
        /// Determine faction based on location
        /// </summary>
        private static VystiaFaction DetermineFactionFromLocation(Mobile creature)
        {
            if (creature.Map == null || creature.Region == null)
                return VystiaFaction.None;

            string regionName = creature.Region.Name?.ToLower() ?? "";

            if (regionName.Contains("frost") || regionName.Contains("ice"))
                return VystiaFaction.Frostguard;
            if (regionName.Contains("ember") || regionName.Contains("fire") || regionName.Contains("volcano"))
                return VystiaFaction.FlameLegion;
            if (regionName.Contains("forest") || regionName.Contains("verdant") || regionName.Contains("grove"))
                return VystiaFaction.Greenward;
            if (regionName.Contains("crystal") || regionName.Contains("arcane"))
                return VystiaFaction.ArcaneConclave;
            if (regionName.Contains("iron") || regionName.Contains("tech") || regionName.Contains("gear"))
                return VystiaFaction.Technoguild;
            if (regionName.Contains("desert") || regionName.Contains("sand"))
                return VystiaFaction.Sandwalkers;
            if (regionName.Contains("void") || regionName.Contains("shadow"))
                return VystiaFaction.Voidborn;

            return VystiaFaction.None;
        }

        /// <summary>
        /// Check if killer is on cooldown for victim
        /// </summary>
        private static bool IsOnCooldown(PlayerMobile killer, PlayerMobile victim)
        {
            if (!s_KillCooldowns.TryGetValue(killer, out var victimCooldowns))
                return false;

            if (!victimCooldowns.TryGetValue(victim, out var lastKill))
                return false;

            return DateTime.UtcNow - lastKill < KillCooldown;
        }

        /// <summary>
        /// Record a kill for cooldown tracking
        /// </summary>
        private static void RecordKill(PlayerMobile killer, PlayerMobile victim)
        {
            if (!s_KillCooldowns.TryGetValue(killer, out var victimCooldowns))
            {
                victimCooldowns = new Dictionary<PlayerMobile, DateTime>();
                s_KillCooldowns[killer] = victimCooldowns;
            }

            victimCooldowns[victim] = DateTime.UtcNow;

            // Clean up old entries
            var toRemove = new List<PlayerMobile>();
            foreach (var kvp in victimCooldowns)
            {
                if (DateTime.UtcNow - kvp.Value > KillCooldown)
                    toRemove.Add(kvp.Key);
            }

            foreach (var pm in toRemove)
                victimCooldowns.Remove(pm);
        }
    }

    #endregion
}
