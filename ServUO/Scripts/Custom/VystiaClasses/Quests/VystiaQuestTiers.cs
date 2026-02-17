using System;
using Server.Mobiles;

namespace Server.Custom.VystiaClasses.Quests
{
    /// <summary>
    /// Quest Tier System
    /// Defines quest tiers, gold rewards, and tier-based mechanics
    /// </summary>
    public static class VystiaQuestTiers
    {
        #region Quest Tier Gold Rewards

        /// <summary>
        /// Get gold reward range for a quest tier
        /// </summary>
        public static (int min, int max) GetGoldRewardRange(QuestTier tier)
        {
            switch (tier)
            {
                case QuestTier.Initiation:   // Tier 1: Level 1-30
                    return (100, 500);
                case QuestTier.Apprentice:   // Tier 2: Level 30-60
                    return (500, 2000);
                case QuestTier.Journeyman:   // Tier 3: Level 60-90
                    return (2000, 5000);
                case QuestTier.Master:      // Tier 4: Level 90+
                    return (5000, 15000);
                default:
                    return (100, 500);
            }
        }

        /// <summary>
        /// Get random gold reward for a quest tier
        /// </summary>
        public static int GetRandomGoldReward(QuestTier tier)
        {
            var (min, max) = GetGoldRewardRange(tier);
            return Utility.RandomMinMax(min, max);
        }

        /// <summary>
        /// Get daily quest gold reward range
        /// </summary>
        public static (int min, int max) GetDailyQuestGoldRange()
        {
            return (200, 800);
        }

        /// <summary>
        /// Get faction quest gold reward range
        /// </summary>
        public static (int min, int max) GetFactionQuestGoldRange()
        {
            return (300, 2500);
        }

        #endregion

        #region Daily Quest System

        /// <summary>
        /// Check if player can accept a daily quest
        /// </summary>
        public static bool CanAcceptDailyQuest(PlayerMobile pm, int questID)
        {
            var tracker = VystiaQuestTracker.GetTracker(pm);
            if (tracker == null)
                return true;

            // Check if quest was completed today
            var lastCompletion = tracker.GetDailyQuestLastCompletion(questID);
            if (lastCompletion == DateTime.MinValue)
                return true;

            // Daily quests reset after 24 hours
            return DateTime.UtcNow - lastCompletion >= TimeSpan.FromHours(24);
        }

        /// <summary>
        /// Mark a daily quest as completed
        /// </summary>
        public static void MarkDailyQuestCompleted(PlayerMobile pm, int questID)
        {
            var tracker = VystiaQuestTracker.GetOrCreateTracker(pm);
            tracker.SetDailyQuestLastCompletion(questID, DateTime.UtcNow);
        }

        #endregion

        #region Faction Quest System

        /// <summary>
        /// Get faction quest reputation reward based on tier
        /// Uses the same tier system as regular quests
        /// </summary>
        public static int GetFactionQuestReputationReward(QuestTier tier)
        {
            // Faction quests use the same tier-based reputation rewards
            switch (tier)
            {
                case QuestTier.Initiation:
                    return Server.Custom.VystiaClasses.Factions.ReputationRewards.SmallQuestReward; // 50
                case QuestTier.Apprentice:
                    return Server.Custom.VystiaClasses.Factions.ReputationRewards.MediumQuestReward; // 150
                case QuestTier.Journeyman:
                    return Server.Custom.VystiaClasses.Factions.ReputationRewards.LargeQuestReward; // 350
                case QuestTier.Master:
                    return Server.Custom.VystiaClasses.Factions.ReputationRewards.EpicQuestReward; // 500
                default:
                    return Server.Custom.VystiaClasses.Factions.ReputationRewards.SmallQuestReward;
            }
        }

        #endregion
    }
}
