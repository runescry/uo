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
    /// Automatic journal integration strategy
    /// Automatically syncs multiplayer quest progress with journal system
    /// </summary>
    public class AutomaticJournalStrategy : IJournalIntegrationStrategy
    {
        public string StrategyName => "automatic";
        public JournalSyncPriority Priority => JournalSyncPriority.Normal;
        public bool SupportsQuestType(QuestType questType) => questType == QuestType.Multiplayer;

        public JournalSyncResult SyncQuest(UnifiedQuestData quest, JournalSyncData syncData, JournalSyncContext context)
        {
            var result = new JournalSyncResult { Success = true };

            try
            {
                // Check if sync is needed
                if (!ShouldSync(quest, syncData))
                {
                    result.Messages.Add("Sync not needed - conditions not met");
                    return result;
                }

                // Sync current progress
                var progressUpdates = GetCurrentProgressUpdates(quest, syncData);
                foreach (var update in progressUpdates)
                {
                    var updateResult = MultiplayerJournalIntegration.UpdateJournalEntries(quest, update);
                    if (updateResult.Success)
                    {
                        result.JournalUpdates += updateResult.JournalUpdates;
                        result.Messages.Add($"Synced {updateResult.JournalUpdates} journal entries");
                    }
                    else
                    {
                        result.Messages.Add($"Failed to sync journal entries: {updateResult.Error}");
                    }
                }

                // Update sync data
                syncData.LastSync = DateTime.UtcNow;
                result.SyncTime = DateTime.UtcNow;

                result.Messages.Add($"Automatic sync completed for quest {quest.QuestId}");
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Error = ex.Message;
                result.Messages.Add($"Automatic sync failed: {ex.Message}");
            }

            return result;
        }

        private bool ShouldSync(UnifiedQuestData quest, JournalSyncData syncData)
        {
            // Check if sync is enabled
            if (!syncData.IsEnabled)
                return false;

            // Check if enough time has passed
            var timeSinceLastSync = DateTime.UtcNow - syncData.LastSync;
            if (timeSinceLastSync < syncData.SyncFrequency)
                return false;

            // Check if there's progress to sync
            return HasProgressToSync(quest, syncData);
        }

        private bool HasProgressToSync(UnifiedQuestData quest, JournalSyncData syncData)
        {
            // Check if there are recent progress events
            var recentProgress = quest.ProgressData?.ProgressHistory
                ?.Where(e => e.Timestamp > syncData.LastSync)
                ?.ToList() ?? new List<ProgressEvent>();

            return recentProgress.Count > 0;
        }

        private List<ProgressUpdate> GetCurrentProgressUpdates(UnifiedQuestData quest, JournalSyncData syncData)
        {
            var updates = new List<ProgressUpdate>();

            // Get recent progress events
            var recentProgress = quest.ProgressData?.ProgressHistory
                ?.Where(e => e.Timestamp > syncData.LastSync)
                ?.ToList() ?? new List<ProgressEvent>();

            // Convert progress events to updates
            foreach (var progressEvent in recentProgress)
            {
                var update = new ProgressUpdate
                {
                    ProgressType = progressEvent.EventType,
                    Amount = progressEvent.Amount,
                    Description = progressEvent.Description,
                    Source = progressEvent.PlayerSerial != 0 ? World.FindMobile(progressEvent.PlayerSerial) as PlayerMobile : null,
                    ObjectiveId = progressEvent.ObjectiveId,
                    Metadata = progressEvent.Metadata
                };

                updates.Add(update);
            }

            return updates;
        }
    }

    /// <summary>
    /// Manual journal integration strategy
    /// Requires manual approval for journal sync operations
    /// </summary>
    public class ManualJournalStrategy : IJournalIntegrationStrategy
    {
        public string StrategyName => "manual";
        public JournalSyncPriority Priority => JournalSyncPriority.Low;
        public bool SupportsQuestType(QuestType questType) => questType == QuestType.Multiplayer;

        public JournalSyncResult SyncQuest(UnifiedQuestData quest, JournalSyncData syncData, JournalSyncContext context)
        {
            var result = new JournalSyncResult { Success = true };

            try
            {
                // Manual sync requires explicit approval
                if (!context.ForceSync)
                {
                    result.Success = false;
                    result.Error = "Manual sync requires explicit approval";
                    result.Messages.Add("Set ForceSync=true to bypass manual approval");
                    return result;
                }

                // Get pending updates
                var pendingUpdates = GetPendingUpdates(quest, syncData);
                if (pendingUpdates.Count == 0)
                {
                    result.Messages.Add("No pending updates to sync");
                    return result;
                }

                // Present pending updates for manual review
                result.Messages.Add($"Manual sync requested for quest {quest.QuestId}");
                result.Messages.Add($"Pending updates: {pendingUpdates.Count}");
                
                foreach (var update in pendingUpdates.Take(5))
                {
                    result.Messages.Add($"  - {update.Description}");
                }

                if (pendingUpdates.Count > 5)
                {
                    result.Messages.Add($"  ... and {pendingUpdates.Count - 5} more updates");
                }

                result.Messages.Add("Use [UnifiedQuest journal sync force] to proceed with manual sync");

                // Store pending updates for later use
                result.Metadata["pending_updates"] = pendingUpdates;
                result.Metadata["manual_review_required"] = true;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Error = ex.Message;
                result.Messages.Add($"Manual sync failed: {ex.Message}");
            }

            return result;
        }

        private List<ProgressUpdate> GetPendingUpdates(UnifiedQuestData quest, JournalSyncData syncData)
        {
            var updates = new List<ProgressUpdate>();

            // Get progress events since last sync
            var recentProgress = quest.ProgressData?.ProgressHistory
                ?.Where(e => e.Timestamp > syncData.LastSync)
                ?.ToList() ?? new List<ProgressEvent>();

            // Convert to progress updates
            foreach (var progressEvent in recentProgress)
            {
                var update = new ProgressUpdate
                {
                    ProgressType = progressEvent.EventType,
                    Amount = progressEvent.Amount,
                    Description = progressEvent.Description,
                    Source = progressEvent.PlayerSerial != 0 ? World.FindMobile(progressEvent.PlayerSerial) as PlayerMobile : null,
                    ObjectiveId = progressEvent.ObjectiveId,
                    Metadata = progressEvent.Metadata
                };

                updates.Add(update);
            }

            return updates;
        }
    }

    /// <summary>
    /// Real-time journal integration strategy
    /// Syncs progress immediately as it happens
    /// </summary>
    public class RealtimeJournalStrategy : IJournalIntegrationStrategy
    {
        public string StrategyName => "realtime";
        public JournalSyncPriority Priority => JournalSyncPriority.High;
        public bool SupportsQuestType(QuestType questType) => questType == QuestType.Multiplayer;

        public JournalSyncResult SyncQuest(UnifiedQuestData quest, JournalSyncData syncData, JournalSyncContext context)
        {
            var result = new JournalSyncResult { Success = true };

            try
            {
                // Real-time sync processes all pending updates immediately
                var allUpdates = GetAllPendingUpdates(quest, syncData);
                
                foreach (var update in allUpdates)
                {
                    var updateResult = MultiplayerJournalIntegration.UpdateJournalEntries(quest, update);
                    if (updateResult.Success)
                    {
                        result.JournalUpdates += updateResult.JournalUpdates;
                        result.MultiplayerUpdates += 1; // Each update affects multiplayer system
                    }
                    else
                    {
                        result.Messages.Add($"Real-time sync failed for update: {updateResult.Error}");
                    }
                }

                // Update sync data
                syncData.LastSync = DateTime.UtcNow;
                syncData.SyncFrequency = TimeSpan.FromSeconds(30); // More frequent for real-time
                result.SyncTime = DateTime.UtcNow;

                result.Messages.Add($"Real-time sync completed: {result.JournalUpdates} journal entries, {allUpdates.Count} updates processed");
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Error = ex.Message;
                result.Messages.Add($"Real-time sync failed: {ex.Message}");
            }

            return result;
        }

        private List<ProgressUpdate> GetAllPendingUpdates(UnifiedQuestData quest, JournalSyncData syncData)
        {
            var updates = new List<ProgressUpdate>();

            // Get all progress events
            var allProgress = quest.ProgressData?.ProgressHistory ?? new List<ProgressEvent>();

            // Convert to progress updates
            foreach (var progressEvent in allProgress)
            {
                var update = new ProgressUpdate
                {
                    ProgressType = progressEvent.EventType,
                    Amount = progressEvent.Amount,
                    Description = progressEvent.Description,
                    Source = progressEvent.PlayerSerial != 0 ? World.FindMobile(progressEvent.PlayerSerial) as PlayerMobile : null,
                    ObjectiveId = progressEvent.ObjectiveId,
                    Metadata = progressEvent.Metadata
                };

                updates.Add(update);
            }

            return updates;
        }
    }

    /// <summary>
    /// Batch journal integration strategy
    /// Processes updates in batches for efficiency
    /// </summary>
    public class BatchJournalStrategy : IJournalIntegrationStrategy
    {
        public string StrategyName => "batch";
        public JournalSyncPriority Priority => JournalSyncPriority.Normal;
        public bool SupportsQuestType(QuestType questType) => questType == QuestType.Multiplayer;

        public JournalSyncResult SyncQuest(UnifiedQuestData quest, JournalSyncData syncData, JournalSyncContext context)
        {
            var result = new JournalSyncResult { Success = true };

            try
            {
                // Batch processing for efficiency
                var batchSize = GetOptimalBatchSize(quest, syncData);
                var allUpdates = GetAllPendingUpdates(quest, syncData);
                
                var batches = allUpdates
                    .Select((update, index) => new { update, index })
                    .GroupBy(x => x.index / batchSize)
                    .Select(g => g.Select(x => x.update).ToList())
                    .ToList();

                foreach (var batch in batches)
                {
                    var batchResult = ProcessBatch(quest, batch, syncData);
                    
                    if (batchResult.Success)
                    {
                        result.JournalUpdates += batchResult.JournalUpdates;
                        result.MultiplayerUpdates += batch.Count;
                        result.Messages.Add($"Processed batch of {batch.Count} updates");
                    }
                    else
                    {
                        result.Messages.Add($"Batch processing failed: {batchResult.Error}");
                    }
                }

                // Update sync data
                syncData.LastSync = DateTime.UtcNow;
                syncData.SyncFrequency = TimeSpan.FromMinutes(10); // Less frequent for batch
                result.SyncTime = DateTime.UtcNow;

                result.Messages.Add($"Batch sync completed: {batches.Count} batches, {allUpdates.Count} total updates");
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Error = ex.Message;
                result.Messages.Add($"Batch sync failed: {ex.Message}");
            }

            return result;
        }

        private int GetOptimalBatchSize(UnifiedQuestData quest, JournalSyncData syncData)
        {
            // Calculate optimal batch size based on party size and update frequency
            var partySize = quest.MultiplayerData?.Party?.Members.Count ?? 1;
            var baseSize = 10;
            
            // Larger parties benefit from larger batches
            return Math.Max(baseSize, partySize * 2);
        }

        private JournalSyncResult ProcessBatch(UnifiedQuestData quest, List<ProgressUpdate> batch, JournalSyncData syncData)
        {
            var result = new JournalSyncResult { Success = true };

            foreach (var update in batch)
            {
                var updateResult = MultiplayerJournalIntegration.UpdateJournalEntries(quest, update);
                if (!updateResult.Success)
                {
                    result.Success = false;
                    result.Error = updateResult.Error;
                    return result;
                }

                result.JournalUpdates += updateResult.JournalUpdates;
            }

            return result;
        }

        private List<ProgressUpdate> GetAllPendingUpdates(UnifiedQuestData quest, JournalSyncData syncData)
        {
            var updates = new List<ProgressUpdate>();

            // Get all progress events
            var allProgress = quest.ProgressData?.ProgressHistory ?? new List<ProgressEvent>();

            // Convert to progress updates
            foreach (var progressEvent in allProgress)
            {
                var update = new ProgressUpdate
                {
                    ProgressType = progressEvent.EventType,
                    Amount = progressEvent.Amount,
                    Description = progressEvent.Description,
                    Source = progressEvent.PlayerSerial != 0 ? World.FindMobile(progressEvent.PlayerSerial) as PlayerMobile : null,
                    ObjectiveId = progressEvent.ObjectiveId,
                    Metadata = progressEvent.Metadata
                };

                updates.Add(update);
            }

            return updates;
        }
    }
}
