using System;
using System.Collections.Generic;
using System.Linq;
using Server.Mobiles;
using Server.Engines.PartySystem;
using Server.Custom.VystiaClasses.Quests;
using Server.Services.QuestJournal;
using Server.Services.MultiplayerQuests;

namespace Server.Services.UnifiedQuestSystem
{
    /// <summary>
    /// Integration manager for Multiplayer Quest System and Quest Journal System
    /// Bridges the gap between multiplayer quest tracking and journal display
    /// </summary>
    public static class MultiplayerJournalIntegration
    {
        private static readonly Dictionary<string, IJournalIntegrationStrategy> s_IntegrationStrategies;
        private static readonly Dictionary<int, JournalSyncData> s_QuestJournalSync;
        private static readonly object s_Lock = new object();
        private static bool s_Initialized = false;
        private static int s_SyncOperations = 0;
        private static int s_JournalUpdates = 0;
        private static int s_MultiplayerUpdates = 0;

        static MultiplayerJournalIntegration()
        {
            s_IntegrationStrategies = new Dictionary<string, IJournalIntegrationStrategy>();
            s_QuestJournalSync = new Dictionary<int, JournalSyncData>();
        }

        /// <summary>
        /// Initialize the Multiplayer-Journal integration system
        /// </summary>
        public static void Initialize()
        {
            if (s_Initialized)
                return;

            s_Initialized = true;

            // Register built-in integration strategies
            RegisterIntegrationStrategy("automatic", new AutomaticJournalStrategy());
            RegisterIntegrationStrategy("manual", new ManualJournalStrategy());
            RegisterIntegrationStrategy("realtime", new RealtimeJournalStrategy());
            RegisterIntegrationStrategy("batch", new BatchJournalStrategy());

            Console.WriteLine("[MultiplayerJournalIntegration] Initialized Multiplayer-Journal integration system");
            Console.WriteLine($"[MultiplayerJournalIntegration] Registered {s_IntegrationStrategies.Count} integration strategies");
        }

        /// <summary>
        /// Sync multiplayer quest progress with journal system
        /// </summary>
        public static JournalSyncResult SyncQuestWithJournal(UnifiedQuestData quest, JournalSyncContext context = null)
        {
            if (quest == null || !quest.IsMultiplayer)
                return new JournalSyncResult { Success = false, Error = "Quest is null or not multiplayer" };

            lock (s_Lock)
            {
                try
                {
                    s_SyncOperations++;

                    // Use default context if none provided
                    context = context ?? new JournalSyncContext();

                    // Select integration strategy
                    var strategy = SelectIntegrationStrategy(quest, context);
                    if (strategy == null)
                    {
                        return new JournalSyncResult { Success = false, Error = "No integration strategy available" };
                    }

                    // Get or create sync data
                    var syncData = GetOrCreateSyncData(quest);

                    // Perform sync operation
                    var result = strategy.SyncQuest(quest, syncData, context);

                    // Update statistics
                    if (result.Success)
                    {
                        s_JournalUpdates += result.JournalUpdates;
                        s_MultiplayerUpdates += result.MultiplayerUpdates;
                    }

                    return result;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[MultiplayerJournalIntegration] Error syncing quest {quest.QuestId}: {ex.Message}");
                    return new JournalSyncResult { Success = false, Error = ex.Message };
                }
            }
        }

        /// <summary>
        /// Update journal entries for multiplayer quest progress
        /// </summary>
        public static JournalUpdateResult UpdateJournalEntries(UnifiedQuestData quest, ProgressUpdate update, JournalUpdateContext context = null)
        {
            if (quest == null || !quest.IsMultiplayer)
                return new JournalUpdateResult { Success = false, Error = "Quest is null or not multiplayer" };

            lock (s_Lock)
            {
                try
                {
                    s_JournalUpdates++;

                    // Get sync data
                    var syncData = GetOrCreateSyncData(quest);

                    // Create journal entries for progress update
                    var entries = CreateJournalEntries(quest, update, context);

                    // Update journal system
                    var result = UpdateJournalSystem(quest, entries, syncData);

                    // Notify party members
                    if (result.Success && quest.MultiplayerData?.Party != null)
                    {
                        NotifyPartyMembers(quest, entries, syncData);
                    }

                    return result;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[MultiplayerJournalIntegration] Error updating journal entries: {ex.Message}");
                    return new JournalUpdateResult { Success = false, Error = ex.Message };
                }
            }
        }

        /// <summary>
        /// Get journal sync status for a quest
        /// </summary>
        public static JournalSyncData GetSyncStatus(int questId)
        {
            lock (s_Lock)
            {
                return s_QuestJournalSync.GetValueOrDefault(questId);
            }
        }

        /// <summary>
        /// Register an integration strategy
        /// </summary>
        public static void RegisterIntegrationStrategy(string strategyName, IJournalIntegrationStrategy strategy)
        {
            lock (s_Lock)
            {
                s_IntegrationStrategies[strategyName] = strategy;
                Console.WriteLine($"[MultiplayerJournalIntegration] Registered integration strategy: {strategyName}");
            }
        }

        /// <summary>
        /// Get integration statistics
        /// </summary>
        public static JournalIntegrationStatistics GetStatistics()
        {
            lock (s_Lock)
            {
                return new JournalIntegrationStatistics
                {
                    TotalSyncOperations = s_SyncOperations,
                    JournalUpdates = s_JournalUpdates,
                    MultiplayerUpdates = s_MultiplayerUpdates,
                    ActiveSyncData = s_QuestJournalSync.Count,
                    RegisteredStrategies = s_IntegrationStrategies.Count,
                    LastSync = DateTime.UtcNow
                };
            }
        }

        /// <summary>
        /// Reset integration statistics
        /// </summary>
        public static void ResetStatistics()
        {
            lock (s_Lock)
            {
                s_SyncOperations = 0;
                s_JournalUpdates = 0;
                s_MultiplayerUpdates = 0;
                Console.WriteLine("[MultiplayerJournalIntegration] Statistics reset");
            }
        }

        /// <summary>
        /// Clear all sync data
        /// </summary>
        public static void ClearAllSyncData()
        {
            lock (s_Lock)
            {
                s_QuestJournalSync.Clear();
                Console.WriteLine("[MultiplayerJournalIntegration] All sync data cleared");
            }
        }

        /// <summary>
        /// Select appropriate integration strategy
        /// </summary>
        private static IJournalIntegrationStrategy SelectIntegrationStrategy(UnifiedQuestData quest, JournalSyncContext context)
        {
            // Use specified strategy if provided
            if (!string.IsNullOrEmpty(context.StrategyName) && s_IntegrationStrategies.TryGetValue(context.StrategyName, out var specifiedStrategy))
            {
                return specifiedStrategy;
            }

            // Auto-select based on quest properties
            if (quest.MultiplayerData?.Settings?.RealtimeJournalSync == true)
            {
                return s_IntegrationStrategies.GetValueOrDefault("realtime", s_IntegrationStrategies.GetValueOrDefault("automatic"));
            }

            if (quest.MultiplayerData?.Party?.Members.Count > 4)
            {
                return s_IntegrationStrategies.GetValueOrDefault("batch", s_IntegrationStrategies.GetValueOrDefault("automatic"));
            }

            return s_IntegrationStrategies.GetValueOrDefault("automatic");
        }

        /// <summary>
        /// Get or create sync data for quest
        /// </summary>
        private static JournalSyncData GetOrCreateSyncData(UnifiedQuestData quest)
        {
            if (!s_QuestJournalSync.TryGetValue(quest.QuestId, out var syncData))
            {
                syncData = new JournalSyncData
                {
                    QuestId = quest.QuestId,
                    QuestTitle = quest.Title,
                    Party = quest.MultiplayerData?.Party,
                    SyncStrategy = "automatic",
                    LastSync = DateTime.UtcNow,
                    IsEnabled = true,
                    SyncFrequency = TimeSpan.FromMinutes(5),
                    JournalEntries = new List<JournalEntry>(),
                    MemberSyncStatus = new Dictionary<Serial, MemberSyncStatus>()
                };

                // Initialize member sync status
                if (quest.MultiplayerData?.Party?.Members != null)
                {
                    foreach (var member in quest.MultiplayerData.Party.Members)
                    {
                        syncData.MemberSyncStatus[member.Serial] = new MemberSyncStatus
                        {
                            Member = member,
                            IsSynced = false,
                            LastSync = DateTime.MinValue,
                            SyncCount = 0
                        };
                    }
                }

                s_QuestJournalSync[quest.QuestId] = syncData;
            }

            return syncData;
        }

        /// <summary>
        /// Create journal entries for progress update
        /// </summary>
        private static List<JournalEntry> CreateJournalEntries(UnifiedQuestData quest, ProgressUpdate update, JournalUpdateContext context)
        {
            var entries = new List<JournalEntry>();

            // Create main progress entry
            var mainEntry = new JournalEntry
            {
                Timestamp = DateTime.UtcNow,
                EntryType = "Progress",
                Message = GenerateProgressMessage(quest, update),
                Player = update.Source,
                QuestId = quest.QuestId,
                Category = "Multiplayer",
                IsPublic = true,
                Metadata = new Dictionary<string, object>
                {
                    { "progress_type", update.ProgressType },
                    { "amount", update.Amount },
                    { "objective_id", update.ObjectiveId },
                    { "party_id", quest.MultiplayerData?.Party?.Id }
                }
            };

            entries.Add(mainEntry);

            // Create individual member entries if configured
            if (context?.CreateIndividualEntries == true && quest.MultiplayerData?.Party?.Members != null)
            {
                foreach (var member in quest.MultiplayerData.Party.Members)
                {
                    if (member != update.Source)
                    {
                        var memberEntry = new JournalEntry
                        {
                            Timestamp = DateTime.UtcNow,
                            EntryType = "PartyProgress",
                            Message = GeneratePartyProgressMessage(quest, update, member),
                            Player = member,
                            QuestId = quest.QuestId,
                            Category = "Multiplayer",
                            IsPublic = false,
                            Metadata = new Dictionary<string, object>
                            {
                                { "progress_type", update.ProgressType },
                                { "amount", update.Amount },
                                { "objective_id", update.ObjectiveId },
                                { "source_player", update.Source?.Name },
                                { "target_player", member.Name }
                            }
                        };

                        entries.Add(memberEntry);
                    }
                }
            }

            return entries;
        }

        /// <summary>
        /// Update journal system with entries
        /// </summary>
        private static JournalUpdateResult UpdateJournalSystem(UnifiedQuestData quest, List<JournalEntry> entries, JournalSyncData syncData)
        {
            var result = new JournalUpdateResult { Success = true };

            try
            {
                // This would integrate with the actual journal system
                // For now, simulate the update
                foreach (var entry in entries)
                {
                    // Add to sync data
                    syncData.JournalEntries.Add(entry);

                    // Update member sync status
                    if (entry.Player != null && syncData.MemberSyncStatus.TryGetValue(entry.Player.Serial, out var memberStatus))
                    {
                        memberStatus.IsSynced = true;
                        memberStatus.LastSync = entry.Timestamp;
                        memberStatus.SyncCount++;
                    }

                    result.UpdatedEntries.Add(entry);
                }

                // Limit journal entries
                if (syncData.JournalEntries.Count > 1000)
                {
                    var entriesToRemove = syncData.JournalEntries.Take(100).ToList();
                    foreach (var entry in entriesToRemove)
                    {
                        syncData.JournalEntries.Remove(entry);
                    }
                }

                result.JournalUpdates = entries.Count;
                result.Message = $"Updated {entries.Count} journal entries for quest {quest.QuestId}";
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Error = ex.Message;
            }

            return result;
        }

        /// <summary>
        /// Notify party members of journal updates
        /// </summary>
        private static void NotifyPartyMembers(UnifiedQuestData quest, List<JournalEntry> entries, JournalSyncData syncData)
        {
            if (quest.MultiplayerData?.Party?.Members == null)
                return;

            foreach (var member in quest.MultiplayerData.Party.Members)
            {
                try
                {
                    // This would send notifications to party members
                    // For now, just log the notification
                    var memberEntries = entries.Where(e => e.Player == member).ToList();
                    if (memberEntries.Count > 0)
                    {
                        Console.WriteLine($"[MultiplayerJournalIntegration] Notified {member.Name} of {memberEntries.Count} journal updates for quest {quest.QuestId}");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[MultiplayerJournalIntegration] Error notifying {member.Name}: {ex.Message}");
                }
            }
        }

        /// <summary>
        /// Generate progress message
        /// </summary>
        private static string GenerateProgressMessage(UnifiedQuestData quest, ProgressUpdate update)
        {
            var playerName = update.Source?.Name ?? "Unknown";
            var action = GetProgressAction(update.ProgressType);
            var objective = !string.IsNullOrEmpty(update.ObjectiveId) ? $"objective {update.ObjectiveId}" : "quest objectives";

            return $"{playerName} {action} {objective} (+{update.Amount ?? 1})";
        }

        /// <summary>
        /// Generate party progress message
        /// </summary>
        private static string GeneratePartyProgressMessage(UnifiedQuestData quest, ProgressUpdate update, PlayerMobile member)
        {
            var sourcePlayer = update.Source?.Name ?? "Unknown";
            var action = GetProgressAction(update.ProgressType);
            var objective = !string.IsNullOrEmpty(update.ObjectiveId) ? $"objective {update.ObjectiveId}" : "quest objectives";

            return $"Party member {sourcePlayer} {action} {objective} (+{update.Amount ?? 1})";
        }

        /// <summary>
        /// Get progress action description
        /// </summary>
        private static string GetProgressAction(string progressType)
        {
            switch (progressType?.ToLower())
            {
                case "kill":
                    return "defeated";
                case "collect":
                    return "collected";
                case "explore":
                    return "discovered";
                case "deliver":
                    return "delivered";
                case "protect":
                    return "protected";
                case "rescue":
                    return "rescued";
                default:
                    return "progressed";
            }
        }
    }

    /// <summary>
    /// Journal sync result
    /// </summary>
    public class JournalSyncResult
    {
        public bool Success { get; set; }
        public string Error { get; set; }
        public List<string> Messages { get; set; }
        public int JournalUpdates { get; set; }
        public int MultiplayerUpdates { get; set; }
        public DateTime SyncTime { get; set; }
        public Dictionary<string, object> Metadata { get; set; }

        public JournalSyncResult()
        {
            Messages = new List<string>();
            Metadata = new Dictionary<string, object>();
            SyncTime = DateTime.UtcNow;
        }
    }

    /// <summary>
    /// Journal update result
    /// </summary>
    public class JournalUpdateResult
    {
        public bool Success { get; set; }
        public string Error { get; set; }
        public string Message { get; set; }
        public List<JournalEntry> UpdatedEntries { get; set; }
        public int JournalUpdates { get; set; }
        public DateTime UpdateTime { get; set; }

        public JournalUpdateResult()
        {
            UpdatedEntries = new List<JournalEntry>();
            UpdateTime = DateTime.UtcNow;
        }
    }

    /// <summary>
    /// Journal sync context
    /// </summary>
    public class JournalSyncContext
    {
        public string StrategyName { get; set; }
        public PlayerMobile Requester { get; set; }
        public Party Party { get; set; }
        public Dictionary<string, object> Parameters { get; set; }
        public bool ForceSync { get; set; }
        public bool IncludePrivateEntries { get; set; }
        public JournalSyncPriority Priority { get; set; }

        public JournalSyncContext()
        {
            Parameters = new Dictionary<string, object>();
            ForceSync = false;
            IncludePrivateEntries = false;
            Priority = JournalSyncPriority.Normal;
        }
    }

    /// <summary>
    /// Journal update context
    /// </summary>
    public class JournalUpdateContext
    {
        public bool CreateIndividualEntries { get; set; }
        public bool NotifyPartyMembers { get; set; }
        public bool UpdatePrivateJournals { get; set; }
        public JournalEntryVisibility DefaultVisibility { get; set; }

        public JournalUpdateContext()
        {
            CreateIndividualEntries = true;
            NotifyPartyMembers = true;
            UpdatePrivateJournals = false;
            DefaultVisibility = JournalEntryVisibility.Party;
        }
    }

    /// <summary>
    /// Journal sync priority levels
    /// </summary>
    public enum JournalSyncPriority
    {
        Low,
        Normal,
        High,
        Critical
    }

    /// <summary>
    /// Journal entry visibility
    /// </summary>
    public enum JournalEntryVisibility
    {
        Private,
        Party,
        Public
    }

    /// <summary>
    /// Journal sync data
    /// </summary>
    public class JournalSyncData
    {
        public int QuestId { get; set; }
        public string QuestTitle { get; set; }
        public Party Party { get; set; }
        public string SyncStrategy { get; set; }
        public DateTime LastSync { get; set; }
        public bool IsEnabled { get; set; }
        public TimeSpan SyncFrequency { get; set; }
        public List<JournalEntry> JournalEntries { get; set; }
        public Dictionary<Serial, MemberSyncStatus> MemberSyncStatus { get; set; }

        public JournalSyncData()
        {
            JournalEntries = new List<JournalEntry>();
            MemberSyncStatus = new Dictionary<Serial, MemberSyncStatus>();
            IsEnabled = true;
            SyncFrequency = TimeSpan.FromMinutes(5);
        }
    }

    /// <summary>
    /// Member sync status
    /// </summary>
    public class MemberSyncStatus
    {
        public PlayerMobile Member { get; set; }
        public bool IsSynced { get; set; }
        public DateTime LastSync { get; set; }
        public int SyncCount { get; set; }
        public JournalEntryVisibility DefaultVisibility { get; set; }

        public MemberSyncStatus()
        {
            DefaultVisibility = JournalEntryVisibility.Party;
        }
    }

    /// <summary>
    /// Journal entry
    /// </summary>
    public class JournalEntry
    {
        public DateTime Timestamp { get; set; }
        public string EntryType { get; set; }
        public string Message { get; set; }
        public PlayerMobile Player { get; set; }
        public int QuestId { get; set; }
        public string Category { get; set; }
        public bool IsPublic { get; set; }
        public Dictionary<string, object> Metadata { get; set; }

        public JournalEntry()
        {
            Metadata = new Dictionary<string, object>();
            Timestamp = DateTime.UtcNow;
        }
    }

    /// <summary>
    /// Journal integration statistics
    /// </summary>
    public class JournalIntegrationStatistics
    {
        public int TotalSyncOperations { get; set; }
        public int JournalUpdates { get; set; }
        public int MultiplayerUpdates { get; set; }
        public int ActiveSyncData { get; set; }
        public int RegisteredStrategies { get; set; }
        public DateTime LastSync { get; set; }
    }

    /// <summary>
    /// Interface for journal integration strategies
    /// </summary>
    public interface IJournalIntegrationStrategy
    {
        JournalSyncResult SyncQuest(UnifiedQuestData quest, JournalSyncData syncData, JournalSyncContext context);
        string StrategyName { get; }
        bool SupportsQuestType(QuestType questType);
        JournalSyncPriority Priority { get; }
    }

    /// <summary>
    /// Extension methods for Multiplayer-Journal integration
    /// </summary>
    public static class MultiplayerJournalExtensions
    {
        /// <summary>
        /// Sync multiplayer quest with journal system
        /// </summary>
        public static JournalSyncResult SyncWithJournal(this UnifiedQuestData quest, JournalSyncContext context = null)
        {
            return MultiplayerJournalIntegration.SyncQuestWithJournal(quest, context);
        }

        /// <summary>
        /// Update journal entries for progress
        /// </summary>
        public static JournalUpdateResult UpdateJournal(this UnifiedQuestData quest, ProgressUpdate update, JournalUpdateContext context = null)
        {
            return MultiplayerJournalIntegration.UpdateJournalEntries(quest, update, context);
        }

        /// <summary>
        /// Check if quest has journal sync
        /// </summary>
        public static bool HasJournalSync(this UnifiedQuestData quest)
        {
            var syncData = MultiplayerJournalIntegration.GetSyncStatus(quest.QuestId);
            return syncData != null && syncData.IsEnabled;
        }

        /// <summary>
        /// Get journal sync status
        /// </summary>
        public static JournalSyncData GetJournalSyncStatus(this UnifiedQuestData quest)
        {
            return MultiplayerJournalIntegration.GetSyncStatus(quest.QuestId);
        }
    }
}
