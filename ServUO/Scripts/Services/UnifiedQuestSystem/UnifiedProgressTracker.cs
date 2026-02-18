using System;
using System.Collections.Generic;
using System.Linq;
using Server.Mobiles;
using Server.Engines.PartySystem;
using Server.Custom.VystiaClasses.Quests;

namespace Server.Services.UnifiedQuestSystem
{
    /// <summary>
    /// Unified quest progress tracking system that consolidates all progress tracking
    /// Replaces SharedProgressTracker and individual progress tracking mechanisms
    /// </summary>
    public static class UnifiedProgressTracker
    {
        private static readonly Dictionary<int, UnifiedQuestProgress> s_QuestProgress;
        private static readonly Dictionary<Serial, Dictionary<int, PlayerQuestProgress>> s_PlayerProgress;
        private static readonly Dictionary<string, IProgressHandler> s_ProgressHandlers;
        private static readonly object s_Lock = new object();
        private static bool s_Initialized = false;
        private static int s_ProgressUpdates = 0;
        private static int s_SynchronizationEvents = 0;

        static UnifiedProgressTracker()
        {
            s_QuestProgress = new Dictionary<int, UnifiedQuestProgress>();
            s_PlayerProgress = new Dictionary<Serial, Dictionary<int, PlayerQuestProgress>>();
            s_ProgressHandlers = new Dictionary<string, IProgressHandler>();
        }

        /// <summary>
        /// Initialize the unified progress tracking system
        /// </summary>
        public static void Initialize()
        {
            if (s_Initialized)
                return;

            s_Initialized = true;

            // Register built-in progress handlers
            RegisterProgressHandler("kill", new KillProgressHandler());
            RegisterProgressHandler("collect", new CollectProgressHandler());
            RegisterProgressHandler("explore", new ExploreProgressHandler());
            RegisterProgressHandler("deliver", new DeliverProgressHandler());
            RegisterProgressHandler("protect", new ProtectProgressHandler());
            RegisterProgressHandler("rescue", new RescueProgressHandler());

            Console.WriteLine("[UnifiedProgressTracker] Initialized unified progress tracking system");
            Console.WriteLine($"[UnifiedProgressTracker] Registered {s_ProgressHandlers.Count} progress handlers");
        }

        /// <summary>
        /// Update quest progress for a player
        /// </summary>
        public static ProgressUpdateResult UpdateProgress(UnifiedQuestData quest, PlayerMobile player, ProgressUpdate update)
        {
            if (quest == null || player == null || update == null)
                return new ProgressUpdateResult { Success = false, Error = "Invalid parameters" };

            lock (s_Lock)
            {
                try
                {
                    s_ProgressUpdates++;

                    // Get or create quest progress
                    var questProgress = GetOrCreateQuestProgress(quest);
                    var playerProgress = GetOrCreatePlayerProgress(quest, player);

                    // Apply progress update
                    var result = ApplyProgressUpdate(quest, questProgress, playerProgress, update);

                    // Synchronize with other systems
                    if (result.Success && quest.MultiplayerData?.Settings?.SyncProgress == true)
                    {
                        SynchronizeProgress(quest, questProgress, playerProgress, update);
                    }

                    // Update overall progress
                    UpdateOverallProgress(questProgress);

                    // Check for completion
                    CheckQuestCompletion(quest, questProgress);

                    return result;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[UnifiedProgressTracker] Error updating progress: {ex.Message}");
                    return new ProgressUpdateResult { Success = false, Error = ex.Message };
                }
            }
        }

        /// <summary>
        /// Get quest progress
        /// </summary>
        public static UnifiedQuestProgress GetQuestProgress(int questId)
        {
            lock (s_Lock)
            {
                return s_QuestProgress.GetValueOrDefault(questId);
            }
        }

        /// <summary>
        /// Get player progress for a quest
        /// </summary>
        public static PlayerQuestProgress GetPlayerProgress(int questId, Serial playerSerial)
        {
            lock (s_Lock)
            {
                if (s_PlayerProgress.TryGetValue(playerSerial, out var playerQuests))
                {
                    return playerQuests.GetValueOrDefault(questId);
                }
                return null;
            }
        }

        /// <summary>
        /// Get all progress for a player
        /// </summary>
        public static Dictionary<int, PlayerQuestProgress> GetAllPlayerProgress(Serial playerSerial)
        {
            lock (s_Lock)
            {
                return s_PlayerProgress.GetValueOrDefault(playerSerial, new Dictionary<int, PlayerQuestProgress>());
            }
        }

        /// <summary>
        /// Synchronize progress across party members
        /// </summary>
        public static void SynchronizePartyProgress(UnifiedQuestData quest, PlayerMobile sourcePlayer, ProgressUpdate update)
        {
            if (quest?.MultiplayerData?.Party == null)
                return;

            lock (s_Lock)
            {
                s_SynchronizationEvents++;

                var questProgress = GetQuestProgress(quest.QuestId);
                if (questProgress == null)
                    return;

                // Synchronize with all party members
                foreach (var member in quest.MultiplayerData.Party.Members)
                {
                    if (member == sourcePlayer)
                        continue; // Skip source player

                    var memberProgress = GetPlayerProgress(quest.QuestId, member.Serial);
                    if (memberProgress != null)
                    {
                        // Apply synchronization based on quest settings
                        ApplyProgressSynchronization(quest, memberProgress, update);
                    }
                }
            }
        }

        /// <summary>
        /// Register a progress handler
        /// </summary>
        public static void RegisterProgressHandler(string progressType, IProgressHandler handler)
        {
            lock (s_Lock)
            {
                s_ProgressHandlers[progressType] = handler;
                Console.WriteLine($"[UnifiedProgressTracker] Registered progress handler: {progressType}");
            }
        }

        /// <summary>
        /// Get progress statistics
        /// </summary>
        public static ProgressStatistics GetStatistics()
        {
            lock (s_Lock)
            {
                var stats = new ProgressStatistics
                {
                    TotalTrackedQuests = s_QuestProgress.Count,
                    ActiveQuests = s_QuestProgress.Values.Count(p => !p.IsCompleted),
                    CompletedQuests = s_QuestProgress.Values.Count(p => p.IsCompleted),
                    TotalPlayerProgress = s_PlayerProgress.Values.Sum(dict => dict.Count),
                    ProgressUpdates = s_ProgressUpdates,
                    SynchronizationEvents = s_SynchronizationEvents,
                    CompletionRate = s_QuestProgress.Count > 0 ? (double)s_QuestProgress.Values.Count(p => p.IsCompleted) / s_QuestProgress.Count : 0.0,
                    LastActivity = DateTime.UtcNow
                };

                return stats;
            }
        }

        /// <summary>
        /// Reset progress statistics
        /// </summary>
        public static void ResetStatistics()
        {
            lock (s_Lock)
            {
                s_ProgressUpdates = 0;
                s_SynchronizationEvents = 0;
                Console.WriteLine("[UnifiedProgressTracker] Statistics reset");
            }
        }

        /// <summary>
        /// Clear all progress data
        /// </summary>
        public static void ClearAllProgress()
        {
            lock (s_Lock)
            {
                s_QuestProgress.Clear();
                s_PlayerProgress.Clear();
                Console.WriteLine("[UnifiedProgressTracker] All progress data cleared");
            }
        }

        /// <summary>
        /// Get or create quest progress
        /// </summary>
        private static UnifiedQuestProgress GetOrCreateQuestProgress(UnifiedQuestData quest)
        {
            if (!s_QuestProgress.TryGetValue(quest.QuestId, out var questProgress))
            {
                questProgress = new UnifiedQuestProgress
                {
                    QuestId = quest.QuestId,
                    QuestTitle = quest.Title,
                    StartedAt = DateTime.UtcNow,
                    IsActive = true,
                    IsCompleted = false,
                    ObjectiveProgress = new Dictionary<string, ObjectiveProgress>(),
                    ProgressHistory = new List<ProgressEvent>()
                };

                // Initialize objective progress
                if (quest.CooperativeObjectives != null)
                {
                    foreach (var objective in quest.CooperativeObjectives)
                    {
                        questProgress.ObjectiveProgress[objective.ObjectiveId] = new ObjectiveProgress
                        {
                            ObjectiveId = objective.ObjectiveId,
                            Description = objective.Description,
                            RequiredCount = objective.RequiredCount,
                            CurrentCount = 0,
                            IsCompleted = false,
                            StartedAt = DateTime.UtcNow
                        };
                    }
                }

                s_QuestProgress[quest.QuestId] = questProgress;
            }

            return questProgress;
        }

        /// <summary>
        /// Get or create player progress
        /// </summary>
        private static PlayerQuestProgress GetOrCreatePlayerProgress(UnifiedQuestData quest, PlayerMobile player)
        {
            if (!s_PlayerProgress.TryGetValue(player.Serial, out var playerQuests))
            {
                playerQuests = new Dictionary<int, PlayerQuestProgress>();
                s_PlayerProgress[player.Serial] = playerQuests;
            }

            if (!playerQuests.TryGetValue(quest.QuestId, out var playerProgress))
            {
                playerProgress = new PlayerQuestProgress
                {
                    PlayerSerial = player.Serial,
                    PlayerName = player.Name,
                    QuestId = quest.QuestId,
                    JoinedAt = DateTime.UtcNow,
                    HasAccepted = true,
                    IsActive = true,
                    IsCompleted = false,
                    ContributionScore = 0,
                    ObjectiveProgress = new Dictionary<string, int>(),
                    ProgressHistory = new List<ProgressEvent>()
                };

                // Initialize objective progress
                if (quest.CooperativeObjectives != null)
                {
                    foreach (var objective in quest.CooperativeObjectives)
                    {
                        playerProgress.ObjectiveProgress[objective.ObjectiveId] = 0;
                    }
                }

                playerQuests[quest.QuestId] = playerProgress;
            }

            return playerProgress;
        }

        /// <summary>
        /// Apply progress update
        /// </summary>
        private static ProgressUpdateResult ApplyProgressUpdate(UnifiedQuestData quest, UnifiedQuestProgress questProgress, PlayerQuestProgress playerProgress, ProgressUpdate update)
        {
            var result = new ProgressUpdateResult { Success = true };

            try
            {
                // Get progress handler
                var handler = s_ProgressHandlers.GetValueOrDefault(update.ProgressType.ToLower());
                if (handler == null)
                {
                    result.Success = false;
                    result.Error = $"No progress handler for type: {update.ProgressType}";
                    return result;
                }

                // Validate update
                var validationResult = handler.ValidateUpdate(quest, questProgress, playerProgress, update);
                if (!validationResult.IsValid)
                {
                    result.Success = false;
                    result.Error = validationResult.ErrorMessage;
                    return result;
                }

                // Apply the update
                handler.ApplyUpdate(quest, questProgress, playerProgress, update);

                // Update contribution score
                playerProgress.ContributionScore += update.ContributionValue ?? 1;

                // Add to history
                var progressEvent = new ProgressEvent
                {
                    Timestamp = DateTime.UtcNow,
                    EventType = update.ProgressType,
                    Description = update.Description,
                    PlayerSerial = playerProgress.PlayerSerial,
                    PlayerName = playerProgress.PlayerName,
                    Amount = update.Amount,
                    ObjectiveId = update.ObjectiveId,
                    Metadata = update.Metadata
                };

                questProgress.ProgressHistory.Add(progressEvent);
                playerProgress.ProgressHistory.Add(progressEvent);

                // Limit history size
                if (questProgress.ProgressHistory.Count > 1000)
                {
                    questProgress.ProgressHistory.RemoveAt(0);
                }

                if (playerProgress.ProgressHistory.Count > 500)
                {
                    playerProgress.ProgressHistory.RemoveAt(0);
                }

                result.UpdatedProgress = questProgress;
                return result;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Error = ex.Message;
                return result;
            }
        }

        /// <summary>
        /// Synchronize progress with other systems
        /// </summary>
        private static void SynchronizeProgress(UnifiedQuestData quest, UnifiedQuestProgress questProgress, PlayerQuestProgress playerProgress, ProgressUpdate update)
        {
            // Update quest data progress
            if (quest.ProgressData != null)
            {
                quest.ProgressData.UpdateProgress();
                quest.ProgressData.ProgressHistory.Add(new ProgressEvent
                {
                    Timestamp = DateTime.UtcNow,
                    EventType = update.ProgressType,
                    Description = update.Description,
                    Amount = update.Amount,
                    Metadata = update.Metadata
                });
            }

            // Notify communication system
            if (quest.CommunicationData?.CommunicationEnabled == true)
            {
                // This would notify the communication system of progress updates
                // Implementation would go here
            }
        }

        /// <summary>
        /// Update overall progress
        /// </summary>
        private static void UpdateOverallProgress(UnifiedQuestProgress questProgress)
        {
            var totalObjectives = questProgress.ObjectiveProgress.Count;
            var completedObjectives = questProgress.ObjectiveProgress.Values.Count(op => op.IsCompleted);

            questProgress.OverallProgress = totalObjectives > 0 ? (double)completedObjectives / totalObjectives : 0.0;
            questProgress.CompletedObjectives = completedObjectives;
            questProgress.TotalObjectives = totalObjectives;
            questProgress.LastUpdated = DateTime.UtcNow;
        }

        /// <summary>
        /// Check for quest completion
        /// </summary>
        private static void CheckQuestCompletion(UnifiedQuestData quest, UnifiedQuestProgress questProgress)
        {
            if (questProgress.IsCompleted)
                return;

            // Check if all objectives are completed
            var allObjectivesCompleted = questProgress.ObjectiveProgress.Values.All(op => op.IsCompleted);
            
            if (allObjectivesCompleted)
            {
                questProgress.IsCompleted = true;
                questProgress.CompletedAt = DateTime.UtcNow;
                questProgress.IsActive = false;

                // Update quest status
                quest.Status = QuestStatus.Completed;
                quest.ProgressData.CompletedAt = DateTime.UtcNow;
                quest.ProgressData.UpdateProgress();

                Console.WriteLine($"[UnifiedProgressTracker] Quest {quest.QuestId} ({quest.Title}) completed!");
            }
        }

        /// <summary>
        /// Apply progress synchronization
        /// </summary>
        private static void ApplyProgressSynchronization(UnifiedQuestData quest, PlayerQuestProgress memberProgress, ProgressUpdate update)
        {
            // Apply synchronization based on quest type and settings
            switch (quest.Type)
            {
                case QuestType.Multiplayer:
                    // For multiplayer quests, sync based on objective type
                    if (quest.CooperativeObjectives != null)
                    {
                        var objective = quest.CooperativeObjectives.FirstOrDefault(o => o.ObjectiveId == update.ObjectiveId);
                        if (objective != null)
                        {
                            switch (objective.Type)
                            {
                                case CooperativeObjectiveType.GroupContribution:
                                    // All members see group progress
                                    memberProgress.ObjectiveProgress[update.ObjectiveId] = update.Amount ?? 0;
                                    break;

                                case CooperativeObjectiveType.AllMembers:
                                    // Each member must complete individually
                                    // Don't sync - each member must complete separately
                                    break;

                                case CooperativeObjectiveType.IndividualContribution:
                                    // Individual contribution only
                                    // Don't sync - progress is individual
                                    break;

                                default:
                                    // Default: show progress notification
                                    break;
                            }
                        }
                    }
                    break;

                default:
                    // For non-multiplayer quests, no synchronization needed
                    break;
            }
        }
    }

    /// <summary>
    /// Unified quest progress data
    /// </summary>
    public class UnifiedQuestProgress
    {
        public int QuestId { get; set; }
        public string QuestTitle { get; set; }
        public DateTime StartedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public DateTime LastUpdated { get; set; }
        public bool IsActive { get; set; }
        public bool IsCompleted { get; set; }
        public double OverallProgress { get; set; }
        public int CompletedObjectives { get; set; }
        public int TotalObjectives { get; set; }
        public Dictionary<string, ObjectiveProgress> ObjectiveProgress { get; set; }
        public List<ProgressEvent> ProgressHistory { get; set; }

        public UnifiedQuestProgress()
        {
            ObjectiveProgress = new Dictionary<string, ObjectiveProgress>();
            ProgressHistory = new List<ProgressEvent>();
            StartedAt = DateTime.UtcNow;
            LastUpdated = DateTime.UtcNow;
            IsActive = true;
            IsCompleted = false;
        }
    }

    /// <summary>
    /// Player quest progress data
    /// </summary>
    public class PlayerQuestProgress
    {
        public Serial PlayerSerial { get; set; }
        public string PlayerName { get; set; }
        public int QuestId { get; set; }
        public DateTime JoinedAt { get; set; }
        public bool HasAccepted { get; set; }
        public bool IsActive { get; set; }
        public bool IsCompleted { get; set; }
        public DateTime? CompletedAt { get; set; }
        public int ContributionScore { get; set; }
        public Dictionary<string, int> ObjectiveProgress { get; set; }
        public List<ProgressEvent> ProgressHistory { get; set; }

        public PlayerQuestProgress()
        {
            ObjectiveProgress = new Dictionary<string, int>();
            ProgressHistory = new List<ProgressEvent>();
            JoinedAt = DateTime.UtcNow;
            HasAccepted = true;
            IsActive = true;
            IsCompleted = false;
        }
    }

    /// <summary>
    /// Objective progress data
    /// </summary>
    public class ObjectiveProgress
    {
        public string ObjectiveId { get; set; }
        public string Description { get; set; }
        public int RequiredCount { get; set; }
        public int CurrentCount { get; set; }
        public bool IsCompleted { get; set; }
        public DateTime StartedAt { get; set; }
        public DateTime? CompletedAt { get; set; }

        public double ProgressPercentage => RequiredCount > 0 ? (double)CurrentCount / RequiredCount : 0.0;

        public void UpdateProgress(int amount)
        {
            CurrentCount = Math.Min(CurrentCount + amount, RequiredCount);
            IsCompleted = CurrentCount >= RequiredCount;
            
            if (IsCompleted && !CompletedAt.HasValue)
            {
                CompletedAt = DateTime.UtcNow;
            }
        }
    }

    /// <summary>
    /// Progress update data
    /// </summary>
    public class ProgressUpdate
    {
        public string ProgressType { get; set; }
        public string ObjectiveId { get; set; }
        public int? Amount { get; set; }
        public string Description { get; set; }
        public int? ContributionValue { get; set; }
        public PlayerMobile Source { get; set; }
        public Dictionary<string, object> Metadata { get; set; }

        public ProgressUpdate()
        {
            Metadata = new Dictionary<string, object>();
        }
    }

    /// <summary>
    /// Progress update result
    /// </summary>
    public class ProgressUpdateResult
    {
        public bool Success { get; set; }
        public string Error { get; set; }
        public UnifiedQuestProgress UpdatedProgress { get; set; }
        public List<string> Messages { get; set; }

        public ProgressUpdateResult()
        {
            Messages = new List<string>();
        }
    }

    /// <summary>
    /// Progress event data
    /// </summary>
    public class ProgressEvent
    {
        public DateTime Timestamp { get; set; }
        public string EventType { get; set; }
        public string Description { get; set; }
        public Serial PlayerSerial { get; set; }
        public string PlayerName { get; set; }
        public int? Amount { get; set; }
        public string ObjectiveId { get; set; }
        public Dictionary<string, object> Metadata { get; set; }

        public ProgressEvent()
        {
            Timestamp = DateTime.UtcNow;
            Metadata = new Dictionary<string, object>();
        }
    }

    /// <summary>
    /// Progress statistics
    /// </summary>
    public class ProgressStatistics
    {
        public int TotalTrackedQuests { get; set; }
        public int ActiveQuests { get; set; }
        public int CompletedQuests { get; set; }
        public int TotalPlayerProgress { get; set; }
        public int ProgressUpdates { get; set; }
        public int SynchronizationEvents { get; set; }
        public double CompletionRate { get; set; }
        public DateTime LastActivity { get; set; }
    }

    /// <summary>
    /// Interface for progress handlers
    /// </summary>
    public interface IProgressHandler
    {
        ValidationResult ValidateUpdate(UnifiedQuestData quest, UnifiedQuestProgress questProgress, PlayerQuestProgress playerProgress, ProgressUpdate update);
        void ApplyUpdate(UnifiedQuestData quest, UnifiedQuestProgress questProgress, PlayerQuestProgress playerProgress, ProgressUpdate update);
        string HandlerType { get; }
    }

    /// <summary>
    /// Validation result for progress updates
    /// </summary>
    public class ValidationResult
    {
        public bool IsValid { get; set; }
        public string ErrorMessage { get; set; }
        public List<string> Warnings { get; set; }

        public ValidationResult()
        {
            Warnings = new List<string>();
        }
    }

    /// <summary>
    /// Extension methods for progress tracking
    /// </summary>
    public static class ProgressExtensions
    {
        /// <summary>
        /// Update quest progress
        /// </summary>
        public static ProgressUpdateResult UpdateProgress(this UnifiedQuestData quest, PlayerMobile player, string progressType, int amount, string objectiveId = null)
        {
            var update = new ProgressUpdate
            {
                ProgressType = progressType,
                Amount = amount,
                ObjectiveId = objectiveId,
                Description = $"{progressType} progress: +{amount}",
                Source = player
            };

            return UnifiedProgressTracker.UpdateProgress(quest, player, update);
        }

        /// <summary>
        /// Get progress percentage
        /// </summary>
        public static double GetProgressPercentage(this UnifiedQuestData quest)
        {
            var progress = UnifiedProgressTracker.GetQuestProgress(quest.QuestId);
            return progress?.OverallProgress ?? 0.0;
        }

        /// <summary>
        /// Check if quest is completed
        /// </summary>
        public static bool IsCompleted(this UnifiedQuestData quest)
        {
            var progress = UnifiedProgressTracker.GetQuestProgress(quest.QuestId);
            return progress?.IsCompleted ?? false;
        }
    }
}
