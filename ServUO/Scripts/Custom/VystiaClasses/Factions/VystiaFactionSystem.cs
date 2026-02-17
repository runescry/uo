/*
 * Vystia Faction Reputation System
 *
 * 7 factions with reputation tiers (Hostile to Exalted, -3000 to 15000)
 * - Reputation gains: Quests (+50-500), Boss kills (+100), Donations (+50/1000g)
 * - Vendor discounts: 5-15% based on tier
 * - Faction tokens for currency
 */

using System;
using System.Collections.Generic;
using Server;
using Server.Mobiles;
using Server.Items;

namespace Server.Custom.VystiaClasses.Factions
{
    /// <summary>
    /// Initialize faction system and test commands
    /// </summary>
    public static class VystiaFactionSystemInit
    {
        public static void Initialize()
        {
            // Initialize test item commands
            FactionStoneCommands.Initialize();
        }
    }
    #region Enums

    /// <summary>
    /// The seven major factions of Vystia
    /// </summary>
    public enum VystiaFaction
    {
        None = 0,

        /// <summary>
        /// Frosthold faction - The Frostguard
        /// </summary>
        Frostguard = 1,

        /// <summary>
        /// Emberlands faction - The Flame Legion
        /// </summary>
        FlameLegion = 2,

        /// <summary>
        /// Verdantpeak faction - The Greenward
        /// </summary>
        Greenward = 3,

        /// <summary>
        /// Crystal Barrens faction - The Arcane Conclave
        /// </summary>
        ArcaneConclave = 4,

        /// <summary>
        /// Ironclad faction - The Technoguild
        /// </summary>
        Technoguild = 5,

        /// <summary>
        /// Desert faction - The Sandwalkers
        /// </summary>
        Sandwalkers = 6,

        /// <summary>
        /// ShadowVoid faction - The Voidborn
        /// </summary>
        Voidborn = 7
    }

    /// <summary>
    /// Reputation tier thresholds (matches design document)
    /// </summary>
    public enum ReputationTier
    {
        Hostile = 0,        // -3000 to -1500
        Unfriendly = 1,     // -1499 to -1
        Neutral = 2,        // 0
        Friendly = 3,       // 1 to 1,500
        Allied = 4,         // 1,501 to 4,500
        Honored = 5,        // 4,501 to 9,000
        Exalted = 6         // 9,001 to 15,000
    }

    #endregion

    #region Faction Data

    /// <summary>
    /// Static data about each faction
    /// </summary>
    public static class FactionData
    {
        private static readonly Dictionary<VystiaFaction, FactionInfo> s_FactionInfo = new Dictionary<VystiaFaction, FactionInfo>
        {
            { VystiaFaction.Frostguard, new FactionInfo(
                "The Frostguard",
                "Protectors of Frosthold and keepers of the ancient ice magic",
                1150, // Ice blue hue
                VystiaFaction.FlameLegion // Enemy faction
            )},
            { VystiaFaction.FlameLegion, new FactionInfo(
                "The Flame Legion",
                "Warriors of the Emberlands who wield the power of fire",
                1358, // Fire red hue
                VystiaFaction.Frostguard
            )},
            { VystiaFaction.Greenward, new FactionInfo(
                "The Greenward",
                "Druids and rangers who protect the forests of Verdantpeak",
                2010, // Forest green hue
                VystiaFaction.Voidborn
            )},
            { VystiaFaction.ArcaneConclave, new FactionInfo(
                "The Arcane Conclave",
                "Mages and scholars who study the crystal magic of the Barrens",
                1154, // Crystal blue hue
                VystiaFaction.Technoguild
            )},
            { VystiaFaction.Technoguild, new FactionInfo(
                "The Technoguild",
                "Engineers and artificers of the Ironclad Wastes",
                2305, // Metallic hue
                VystiaFaction.ArcaneConclave
            )},
            { VystiaFaction.Sandwalkers, new FactionInfo(
                "The Sandwalkers",
                "Nomadic traders and warriors of the Desert of Surya",
                1719, // Sand/tan hue
                VystiaFaction.None
            )},
            { VystiaFaction.Voidborn, new FactionInfo(
                "The Voidborn",
                "Dark practitioners who draw power from the ShadowVoid",
                1109, // Dark purple hue
                VystiaFaction.Greenward
            )}
        };

        public static FactionInfo GetInfo(VystiaFaction faction)
        {
            if (s_FactionInfo.TryGetValue(faction, out var info))
                return info;
            return null;
        }

        /// <summary>
        /// Get the reputation tier for a given reputation value
        /// </summary>
        public static ReputationTier GetTier(int reputation)
        {
            if (reputation >= 9001) return ReputationTier.Exalted;      // 9,001 to 15,000
            if (reputation >= 4501) return ReputationTier.Honored;      // 4,501 to 9,000
            if (reputation >= 1501) return ReputationTier.Allied;       // 1,501 to 4,500
            if (reputation >= 1) return ReputationTier.Friendly;       // 1 to 1,500
            if (reputation == 0) return ReputationTier.Neutral;         // 0
            if (reputation > -1500) return ReputationTier.Unfriendly;  // -1,499 to -1
            return ReputationTier.Hostile;                               // -3,000 to -1,500
        }

        /// <summary>
        /// Get the minimum reputation required for a tier
        /// </summary>
        public static int GetTierThreshold(ReputationTier tier)
        {
            switch (tier)
            {
                case ReputationTier.Exalted: return 9001;      // 9,001 to 15,000
                case ReputationTier.Honored: return 4501;      // 4,501 to 9,000
                case ReputationTier.Allied: return 1501;       // 1,501 to 4,500
                case ReputationTier.Friendly: return 1;        // 1 to 1,500
                case ReputationTier.Neutral: return 0;         // 0
                case ReputationTier.Unfriendly: return -1500;  // -1,500 to -1
                case ReputationTier.Hostile: return -3000;     // -3,000 to -1,500
                default: return 0;
            }
        }

        /// <summary>
        /// Get vendor discount percentage for a reputation tier
        /// </summary>
        public static int GetVendorDiscount(ReputationTier tier)
        {
            switch (tier)
            {
                case ReputationTier.Exalted: return 15;   // 15% discount
                case ReputationTier.Honored: return 12;   // 12% discount
                case ReputationTier.Allied: return 10;    // 10% discount
                case ReputationTier.Friendly: return 5;    // 5% discount
                default: return 0;
            }
        }

        /// <summary>
        /// Get the tier color for display
        /// </summary>
        public static int GetTierHue(ReputationTier tier)
        {
            switch (tier)
            {
                case ReputationTier.Exalted: return 0x35;  // Gold
                case ReputationTier.Honored: return 0x3B;  // Green
                case ReputationTier.Allied: return 0x59;   // Blue
                case ReputationTier.Friendly: return 0x59; // Blue
                case ReputationTier.Neutral: return 0x3B2; // Gray
                case ReputationTier.Unfriendly: return 0x26; // Orange
                case ReputationTier.Hostile: return 0x22;  // Red
                default: return 0;
            }
        }
    }

    /// <summary>
    /// Information about a specific faction
    /// </summary>
    public class FactionInfo
    {
        public string Name { get; }
        public string Description { get; }
        public int Hue { get; }
        public VystiaFaction EnemyFaction { get; }

        public FactionInfo(string name, string description, int hue, VystiaFaction enemyFaction)
        {
            Name = name;
            Description = description;
            Hue = hue;
            EnemyFaction = enemyFaction;
        }
    }

    #endregion

    #region Player Reputation Storage

    /// <summary>
    /// Manages faction reputation for players
    /// </summary>
    public static class VystiaReputation
    {
        private static readonly Dictionary<PlayerMobile, ReputationData> s_ReputationData = new Dictionary<PlayerMobile, ReputationData>();

        /// <summary>
        /// Get or create reputation data for a player
        /// </summary>
        public static ReputationData GetReputation(PlayerMobile pm)
        {
            if (pm == null)
                return null;

            if (!s_ReputationData.TryGetValue(pm, out var data))
            {
                data = new ReputationData();
                s_ReputationData[pm] = data;
            }

            return data;
        }

        /// <summary>
        /// Get player's reputation with a specific faction
        /// </summary>
        public static int GetFactionReputation(PlayerMobile pm, VystiaFaction faction)
        {
            var data = GetReputation(pm);
            if (data == null)
                return 0;

            return data.GetReputation(faction);
        }

        /// <summary>
        /// Get player's reputation tier with a specific faction
        /// </summary>
        public static ReputationTier GetFactionTier(PlayerMobile pm, VystiaFaction faction)
        {
            return FactionData.GetTier(GetFactionReputation(pm, faction));
        }

        /// <summary>
        /// Add reputation to a specific faction
        /// </summary>
        public static void AddReputation(PlayerMobile pm, VystiaFaction faction, int amount, string source)
        {
            if (faction == VystiaFaction.None)
                return;

            var data = GetReputation(pm);
            if (data == null)
                return;

            int oldRep = data.GetReputation(faction);
            data.AddReputation(faction, amount);
            int newRep = data.GetReputation(faction);

            if (newRep != oldRep)
            {
                var oldTier = FactionData.GetTier(oldRep);
                var newTier = FactionData.GetTier(newRep);
                var info = FactionData.GetInfo(faction);
                int hue = FactionData.GetTierHue(newTier);

                if (amount > 0)
                    pm.SendMessage(hue, "You gain {0} reputation with {1}. ({2})", amount, info?.Name ?? faction.ToString(), newTier);
                else
                    pm.SendMessage(hue, "You lose {0} reputation with {1}. ({2})", -amount, info?.Name ?? faction.ToString(), newTier);

                // Notify on tier change
                if (newTier > oldTier)
                {
                    pm.SendMessage(0x35, "Your standing with {0} has improved to {1}!", info?.Name, newTier);
                    pm.FixedParticles(0x376A, 9, 32, 5030, info?.Hue ?? 0, 0, EffectLayer.Waist);
                    pm.PlaySound(0x1F2);
                }
                else if (newTier < oldTier)
                {
                    pm.SendMessage(0x22, "Your standing with {0} has fallen to {1}.", info?.Name, newTier);
                }

                // Handle enemy faction penalty
                if (amount > 0 && info?.EnemyFaction != VystiaFaction.None)
                {
                    int penaltyAmount = amount / 2; // Lose half as much with enemy
                    data.AddReputation(info.EnemyFaction, -penaltyAmount);
                    var enemyInfo = FactionData.GetInfo(info.EnemyFaction);
                    pm.SendMessage(0x22, "You lose {0} reputation with {1}.", penaltyAmount, enemyInfo?.Name);
                }
            }
        }

        /// <summary>
        /// Calculate vendor price with faction discount
        /// </summary>
        public static int ApplyFactionDiscount(PlayerMobile pm, VystiaFaction faction, int basePrice)
        {
            int discount = FactionData.GetVendorDiscount(GetFactionTier(pm, faction));
            return basePrice - (basePrice * discount / 100);
        }
    }

    /// <summary>
    /// Reputation data for a single player
    /// </summary>
    public class ReputationData
    {
        private readonly Dictionary<VystiaFaction, int> m_Reputation;

        public ReputationData()
        {
            m_Reputation = new Dictionary<VystiaFaction, int>();

            // Initialize all factions to neutral (0)
            foreach (VystiaFaction faction in Enum.GetValues(typeof(VystiaFaction)))
            {
                if (faction != VystiaFaction.None)
                    m_Reputation[faction] = 0;
            }
        }

        public int GetReputation(VystiaFaction faction)
        {
            if (m_Reputation.TryGetValue(faction, out int rep))
                return rep;
            return 0;
        }

        public void SetReputation(VystiaFaction faction, int value)
        {
            m_Reputation[faction] = Math.Max(-3000, Math.Min(21000, value)); // Cap at -3000 to 21000
        }

        public void AddReputation(VystiaFaction faction, int amount)
        {
            int current = GetReputation(faction);
            SetReputation(faction, current + amount);
        }
    }

    #endregion

    #region Reputation Rewards

    /// <summary>
    /// Standard reputation rewards for various actions
    /// </summary>
    public static class ReputationRewards
    {
        // Quest rewards
        public const int SmallQuestReward = 50;
        public const int MediumQuestReward = 150;
        public const int LargeQuestReward = 350;
        public const int EpicQuestReward = 500;

        // Boss kills
        public const int MiniBossReward = 50;
        public const int RegionalBossReward = 100;
        public const int WorldBossReward = 250;

        // Donations (per 1000g)
        public const int DonationPer1000Gold = 50;

        // Kills (enemy faction members)
        public const int EnemyKillReward = 25;

        /// <summary>
        /// Award reputation for completing a quest
        /// </summary>
        public static void AwardQuestReputation(PlayerMobile pm, VystiaFaction faction, int tier)
        {
            int reward;
            switch (tier)
            {
                case 1: reward = SmallQuestReward; break;
                case 2: reward = MediumQuestReward; break;
                case 3: reward = LargeQuestReward; break;
                case 4: reward = EpicQuestReward; break;
                default: reward = SmallQuestReward; break;
            }

            VystiaReputation.AddReputation(pm, faction, reward, "quest completion");
        }

        /// <summary>
        /// Award reputation for killing a boss
        /// </summary>
        public static void AwardBossReputation(PlayerMobile pm, VystiaFaction faction, bool isWorldBoss)
        {
            int reward = isWorldBoss ? WorldBossReward : RegionalBossReward;
            VystiaReputation.AddReputation(pm, faction, reward, "boss kill");
        }

        /// <summary>
        /// Award reputation for gold donation
        /// </summary>
        public static void AwardDonationReputation(PlayerMobile pm, VystiaFaction faction, int goldAmount)
        {
            if (goldAmount < 1000)
            {
                pm.SendMessage("Donations must be at least 1,000 gold.");
                return;
            }

            // Check if player has the gold
            if (!Banker.Withdraw(pm, goldAmount))
            {
                pm.SendMessage("You don't have enough gold to donate.");
                return;
            }

            int reputationGained = (goldAmount / 1000) * DonationPer1000Gold;
            VystiaReputation.AddReputation(pm, faction, reputationGained, "donation");

            pm.PlaySound(0x2E6); // Coin sound
        }
    }

    #endregion
}
