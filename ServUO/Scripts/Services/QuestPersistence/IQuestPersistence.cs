using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Server.Mobiles;
using Server.Engines.Quests;
using Server.Custom.VystiaClasses.Quests;
using Server.Custom.VystiaClasses;
using Newtonsoft.Json;

namespace Server.Services.QuestPersistence
{
    /// <summary>
    /// Unified interface for quest persistence across all quest systems
    /// Provides consistent storage and retrieval regardless of quest type
    /// </summary>
    public interface IQuestPersistence
    {
        /// <summary>
        /// Save quest data for a player
        /// </summary>
        Task SaveQuestAsync(PlayerMobile player, QuestData questData);

        /// <summary>
        /// Load quest data for a player
        /// </summary>
        Task<List<QuestData>> LoadQuestsAsync(PlayerMobile player);

        /// <summary>
        /// Delete quest data for a player
        /// </summary>
        Task DeleteQuestAsync(PlayerMobile player, int questId);

        /// <summary>
        /// Update quest progress for a player
        /// </summary>
        Task UpdateQuestProgressAsync(PlayerMobile player, int questId, QuestProgress progress);

        /// <summary>
        /// Get quest progress for a player
        /// </summary>
        Task<QuestProgress> GetQuestProgressAsync(PlayerMobile player, int questId);

        /// <summary>
        /// Mark quest as completed for a player
        /// </summary>
        Task MarkQuestCompletedAsync(PlayerMobile player, int questId);

        /// <summary>
        /// Check if player has completed a quest
        /// </summary>
        Task<bool> HasCompletedQuestAsync(PlayerMobile player, int questId);

        /// <summary>
        /// Get all active quests for a player
        /// </summary>
        Task<List<int>> GetActiveQuestsAsync(PlayerMobile player);

        /// <summary>
        /// Get all completed quests for a player
        /// </summary>
        Task<List<int>> GetCompletedQuestsAsync(PlayerMobile player);

        /// <summary>
        /// Validate quest data integrity
        /// </summary>
        Task<ValidationResult> ValidateQuestDataAsync(PlayerMobile player);

        /// <summary>
        /// Backup quest data for a player
        /// </summary>
        Task BackupQuestDataAsync(PlayerMobile player);

        /// <summary>
        /// Restore quest data from backup for a player
        /// </summary>
        Task RestoreQuestDataAsync(PlayerMobile player, DateTime backupDate);

        /// <summary>
        /// Get available backup dates for a player
        /// </summary>
        Task<List<DateTime>> GetBackupDatesAsync(PlayerMobile player);
    }

    /// <summary>
    /// Unified quest data structure that works across all quest systems
    /// </summary>
    public class QuestData
    {
        public int QuestId { get; set; }
        public string QuestType { get; set; } // "Vystia", "Dynamic", "Traditional"
        public string Title { get; set; }
        public string Description { get; set; }
        public PlayerClassTypeV2 RequiredClass { get; set; }
        public QuestTier Tier { get; set; }
        public int PrerequisiteQuestId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime LastAccessed { get; set; }
        public DateTime? CompletedAt { get; set; }
        public bool IsActive { get; set; }
        public bool IsCompleted { get; set; }
        public Dictionary<string, object> Metadata { get; set; } = new Dictionary<string, object>();
        public Dictionary<string, int> Objectives { get; set; } = new Dictionary<string, int>();
        public Dictionary<string, object> Progress { get; set; } = new Dictionary<string, object>();
        public List<string> Tags { get; set; } = new List<string>();
        public string JsonData { get; set; } // For serialization

        /// <summary>
        /// Create quest data from Vystia quest
        /// </summary>
        public static QuestData FromVystiaQuest(VystiaQuest quest, PlayerMobile player)
        {
            var tracker = VystiaQuestTracker.GetTracker(player);
            var progress = tracker?.GetProgress(quest.QuestID);
            
            var questData = new QuestData
            {
                QuestId = quest.QuestID,
                QuestType = "Vystia",
                Title = quest.Title,
                Description = quest.Description,
                RequiredClass = quest.RequiredClass,
                Tier = quest.Tier,
                PrerequisiteQuestId = quest.PrerequisiteQuestID,
                CreatedAt = DateTime.UtcNow,
                LastAccessed = DateTime.UtcNow,
                IsActive = tracker?.IsActive(quest.QuestID) ?? false,
                IsCompleted = tracker?.HasCompleted(quest.QuestID) ?? false,
                CompletedAt = tracker?.HasCompleted(quest.QuestID) == true ? DateTime.UtcNow : (DateTime?)null,
                Objectives = quest.GetObjectives(),
                Progress = progress?.ToDictionary() ?? new Dictionary<string, object>(),
                Tags = new List<string> { "Vystia", quest.RequiredClass.ToString(), quest.Tier.ToString() }
            };
            
            // Store JSON data for serialization
            questData.JsonData = JsonConvert.SerializeObject(questData);
            
            return questData;
        }

        /// <summary>
        /// Create quest data from Dynamic quest
        /// </summary>
        public static QuestData FromDynamicQuest(DynamicQuest quest, PlayerMobile player)
        {
            var tracker = VystiaQuestTracker.GetTracker(player);
            var progress = tracker?.GetProgress(quest.QuestID);
            
            var questData = new QuestData
            {
                QuestId = quest.QuestID,
                QuestType = "Dynamic",
                Title = quest.Title,
                Description = quest.Description,
                RequiredClass = quest.RequiredClass,
                Tier = quest.Tier,
                PrerequisiteQuestId = quest.PrerequisiteQuestID,
                CreatedAt = DateTime.UtcNow,
                LastAccessed = DateTime.UtcNow,
                IsActive = tracker?.IsActive(quest.QuestID) ?? false,
                IsCompleted = tracker?.HasCompleted(quest.QuestID) ?? false,
                CompletedAt = tracker?.HasCompleted(quest.QuestID) == true ? DateTime.UtcNow : (DateTime?)null,
                Objectives = quest.GetObjectives(),
                Progress = progress?.ToDictionary() ?? new Dictionary<string, object>(),
                Tags = new List<string> { "Dynamic", quest.RequiredClass.ToString(), quest.Tier.ToString(), quest.IsEphemeral ? "Ephemeral" : "Persistent" },
                Metadata = new Dictionary<string, object>
                {
                    ["IsEphemeral"] = quest.IsEphemeral,
                    ["Waypoints"] = quest.Waypoints?.Count ?? 0,
                    ["Rewards"] = quest.Rewards?.Count ?? 0
                }
            };
            
            // Store JSON data for serialization
            questData.JsonData = JsonConvert.SerializeObject(questData);
            
            return questData;
        }

        /// <summary>
        /// Create quest data from traditional BaseQuest
        /// </summary>
        public static QuestData FromBaseQuest(BaseQuest quest, PlayerMobile player)
        {
            // Traditional quests use ServUO's built-in quest system
            // We'll need to extract data from the player's quest context
            return new QuestData
            {
                QuestId = quest.GetHashCode(), // Use hash as ID since BaseQuest may not have one
                QuestType = "Traditional",
                Title = quest.GetType().Name,
                Description = "A traditional quest from the ServUO quest system",
                RequiredClass = PlayerClassTypeV2.None,
                Tier = QuestTier.Initiation,
                PrerequisiteQuestId = 0,
                CreatedAt = DateTime.UtcNow,
                LastAccessed = DateTime.UtcNow,
                IsActive = false, // Traditional quests use different tracking
                IsCompleted = false, // Traditional quests use different completion tracking
                Objectives = new Dictionary<string, int>(),
                Progress = new Dictionary<string, object>(),
                Tags = new List<string> { "Traditional" },
                Metadata = new Dictionary<string, object>
                {
                    ["QuestType"] = quest.GetType().Name,
                    ["AllObjectives"] = quest.AllObjectives,
                    ["DoneOnce"] = quest.DoneOnce,
                    ["ForceRemember"] = quest.ForceRemember
                }
            };
        }

        /// <summary>
        /// Convert quest data back to appropriate quest type
        /// </summary>
        public object ToQuest()
        {
            switch (QuestType)
            {
                case "Vystia":
                    return ToVystiaQuest();
                case "Dynamic":
                    return ToDynamicQuest();
                case "Traditional":
                    return ToBaseQuest();
                default:
                    return null;
            }
        }

        private VystiaQuest ToVystiaQuest()
        {
            // This would need to be implemented to recreate the VystiaQuest from data
            // For now, return null as this would require access to the quest registry
            return null;
        }

        private DynamicQuest ToDynamicQuest()
        {
            // This would need to be implemented to recreate the DynamicQuest from data
            // For now, return null as this would require access to quest templates
            return null;
        }

        private BaseQuest ToBaseQuest()
        {
            // Traditional quests can't be recreated from data without the original class
            return null;
        }
    }

    /// <summary>
    /// Validation result for quest data integrity
    /// </summary>
    public class ValidationResult
    {
        public bool IsValid { get; set; }
        public List<string> Errors { get; set; } = new List<string>();
        public List<string> Warnings { get; set; } = new List<string>();
        public DateTime ValidatedAt { get; set; } = DateTime.UtcNow;

        public void AddError(string error)
        {
            Errors.Add(error);
            IsValid = false;
        }

        public void AddWarning(string warning)
        {
            Warnings.Add(warning);
        }
    }
}
