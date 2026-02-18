using System;
using System.Collections.Generic;
using Server.Mobiles;
using Server.Custom.VystiaClasses.Quests;
using Server.Engines.Quests;

namespace Server.Services.QuestJournal
{
    /// <summary>
    /// Interface for quest display adapters to standardize quest information across different quest systems
    /// </summary>
    public interface IQuestDisplayAdapter
    {
        /// <summary>
        /// Get quest title for display
        /// </summary>
        string GetTitle(object quest);

        /// <summary>
        /// Get quest description for display
        /// </summary>
        string GetDescription(object quest);

        /// <summary>
        /// Get quest type identifier
        /// </summary>
        string GetQuestType(object quest);

        /// <summary>
        /// Get quest ID for tracking
        /// </summary>
        int GetQuestId(object quest);

        /// <summary>
        /// Get current progress information
        /// </summary>
        QuestProgressInfo GetProgress(object quest, PlayerMobile player);

        /// <summary>
        /// Check if quest is active for the player
        /// </summary>
        bool IsActive(object quest, PlayerMobile player);

        /// <summary>
        /// Check if quest is completed by the player
        /// </summary>
        bool IsCompleted(object quest, PlayerMobile player);

        /// <summary>
        /// Get quest objectives for display
        /// </summary>
        List<QuestObjectiveInfo> GetObjectives(object quest, PlayerMobile player);

        /// <summary>
        /// Get quest rewards for display
        /// </summary>
        List<QuestRewardInfo> GetRewards(object quest);

        /// <summary>
        /// Get quest difficulty level
        /// </summary>
        string GetDifficulty(object quest);

        /// <summary>
        /// Get quest location information
        /// </summary>
        string GetLocation(object quest);

        /// <summary>
        /// Get quest time limit (if any)
        /// </summary>
        TimeSpan? GetTimeLimit(object quest);

        /// <summary>
        /// Check if quest can be abandoned
        /// </summary>
        bool CanAbandon(object quest, PlayerMobile player);
    }

    /// <summary>
    /// Standardized quest progress information
    /// </summary>
    public class QuestProgressInfo
    {
        public string Description { get; set; }
        public int CurrentProgress { get; set; }
        public int RequiredProgress { get; set; }
        public double ProgressPercentage => RequiredProgress > 0 ? (double)CurrentProgress / RequiredProgress : 0;
        public bool IsCompleted { get; set; }
        public string ProgressText => $"{CurrentProgress}/{RequiredProgress}";
    }

    /// <summary>
    /// Standardized quest objective information
    /// </summary>
    public class QuestObjectiveInfo
    {
        public string Description { get; set; }
        public bool IsCompleted { get; set; }
        public string ProgressText { get; set; }
        public int CurrentProgress { get; set; }
        public int RequiredProgress { get; set; }
    }

    /// <summary>
    /// Standardized quest reward information
    /// </summary>
    public class QuestRewardInfo
    {
        public string Description { get; set; }
        public string Type { get; set; } // "Gold", "Item", "Experience", "Fame", "Karma"
        public int Amount { get; set; }
        public string ItemName { get; set; }
        public int ItemHue { get; set; }
    }

    /// <summary>
    /// Quest filter options
    /// </summary>
    public enum QuestFilterType
    {
        All,
        Active,
        Completed,
        Vystia,
        Dynamic,
        Traditional,
        ByDifficulty,
        ByLocation,
        ByType
    }

    /// <summary>
    /// Quest sort options
    /// </summary>
    public enum QuestSortType
    {
        ByName,
        ByDifficulty,
        ByProgress,
        ByType,
        ByLocation,
        ByTimeRemaining,
        ByDateStarted
    }

    /// <summary>
    /// Unified quest information for display
    /// </summary>
    public class UnifiedQuestInfo
    {
        public int QuestId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string QuestType { get; set; }
        public string Difficulty { get; set; }
        public string Location { get; set; }
        public bool IsActive { get; set; }
        public bool IsCompleted { get; set; }
        public DateTime? StartedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public TimeSpan? TimeLimit { get; set; }
        public TimeSpan? TimeRemaining { get; set; }
        public QuestProgressInfo Progress { get; set; }
        public List<QuestObjectiveInfo> Objectives { get; set; }
        public List<QuestRewardInfo> Rewards { get; set; }
        public bool CanAbandon { get; set; }
        public object OriginalQuest { get; set; }
        public IQuestDisplayAdapter Adapter { get; set; }
    }
}
