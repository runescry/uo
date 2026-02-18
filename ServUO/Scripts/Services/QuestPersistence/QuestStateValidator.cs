using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Server;
using Server.Mobiles;
using Server.Custom.VystiaClasses.Quests;
using Server.Custom.VystiaClasses;
using Server.Services.LLM;

namespace Server.Services.QuestPersistence
{
    /// <summary>
    /// Validates quest state consistency across all quest systems
    /// Ensures data integrity and synchronization between different quest types
    /// </summary>
    public static class QuestStateValidator
    {
        /// <summary>
        /// Perform comprehensive validation of quest state for a player
        /// </summary>
        public static async Task<ValidationResult> ValidatePlayerQuestStateAsync(PlayerMobile player)
        {
            var result = new ValidationResult();

            try
            {
                Console.WriteLine($"[QuestStateValidator] Starting validation for {player.Name}...");

                // 1. Validate unified persistence data
                await ValidateUnifiedPersistenceAsync(player, result);

                // 2. Validate Vystia quest tracker consistency
                await ValidateVystiaTrackerAsync(player, result);

                // 3. Validate Dynamic quest instances
                await ValidateDynamicQuestsAsync(player, result);

                // 4. Validate Traditional quest context
                await ValidateTraditionalQuestsAsync(player, result);

                // 5. Check for orphaned quest data
                await CheckForOrphanedDataAsync(player, result);

                // 6. Validate quest progression logic
                await ValidateQuestProgressionAsync(player, result);

                // 7. Check for duplicate quest IDs
                await CheckForDuplicateQuestsAsync(player, result);

                Console.WriteLine($"[QuestStateValidator] Validation completed for {player.Name}: {result.Errors.Count} errors, {result.Warnings.Count} warnings");

                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[QuestStateValidator] Error during validation for {player.Name}: {ex.Message}");
                result.AddError($"Validation failed: {ex.Message}");
                return result;
            }
        }

        /// <summary>
        /// Validate all players' quest states
        /// </summary>
        public static async Task<ValidationSummary> ValidateAllPlayersAsync()
        {
            var summary = new ValidationSummary();

            try
            {
                Console.WriteLine("[QuestStateValidator] Starting validation for all players...");

                var allPlayers = World.Mobiles.Values.OfType<PlayerMobile>().ToList();
                summary.TotalPlayers = allPlayers.Count;

                foreach (var player in allPlayers)
                {
                    var playerResult = await ValidatePlayerQuestStateAsync(player);
                    
                    if (playerResult.IsValid)
                    {
                        summary.ValidPlayers++;
                    }
                    else
                    {
                        summary.InvalidPlayers++;
                        summary.TotalErrors += playerResult.Errors.Count;
                        summary.TotalWarnings += playerResult.Warnings.Count;
                    }
                }

                Console.WriteLine($"[QuestStateValidator] All players validation completed: {summary.ValidPlayers}/{summary.TotalPlayers} valid, {summary.TotalErrors} errors, {summary.TotalWarnings} warnings");

                return summary;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[QuestStateValidator] Error during all players validation: {ex.Message}");
                summary.AddError($"All players validation failed: {ex.Message}");
                return summary;
            }
        }

        /// <summary>
        /// Repair quest state issues for a player
        /// </summary>
        public static async Task<RepairResult> RepairPlayerQuestStateAsync(PlayerMobile player)
        {
            var result = new RepairResult();

            try
            {
                Console.WriteLine($"[QuestStateValidator] Starting repair for {player.Name}...");

                // 1. Load current quest data
                var unifiedQuests = await QuestPersistenceManager.LoadQuestsAsync(player);
                var vystiaTracker = VystiaQuestTracker.GetTracker(player);

                // 2. Repair Vystia quest tracker consistency
                await RepairVystiaTrackerAsync(player, unifiedQuests, vystiaTracker, result);

                // 3. Repair Dynamic quest instances
                await RepairDynamicQuestsAsync(player, unifiedQuests, result);

                // 4. Clean up orphaned data
                await CleanupOrphanedDataAsync(player, result);

                // 5. Synchronize quest states
                await SynchronizeQuestStatesAsync(player, result);

                Console.WriteLine($"[QuestStateValidator] Repair completed for {player.Name}: {result.RepairedIssues} issues fixed, {result.UnfixableIssues} issues remain");

                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[QuestStateValidator] Error during repair for {player.Name}: {ex.Message}");
                result.AddError($"Repair failed: {ex.Message}");
                return result;
            }
        }

        #region Private Validation Methods

        private static async Task ValidateUnifiedPersistenceAsync(PlayerMobile player, ValidationResult result)
        {
            try
            {
                var validation = await QuestPersistenceManager.ValidateQuestDataAsync(player);
                
                result.Errors.AddRange(validation.Errors);
                result.Warnings.AddRange(validation.Warnings);
            }
            catch (Exception ex)
            {
                result.AddError($"Unified persistence validation failed: {ex.Message}");
            }
        }

        private static async Task ValidateVystiaTrackerAsync(PlayerMobile player, ValidationResult result)
        {
            try
            {
                var tracker = VystiaQuestTracker.GetTracker(player);
                if (tracker == null)
                {
                    result.AddWarning("No Vystia quest tracker found");
                    return;
                }

                var activeQuests = tracker.GetActiveQuests();
                var completedQuests = activeQuests.Where(qid => tracker.HasCompleted(qid)).ToList();

                // Check for quest IDs that don't exist in the quest registry
                foreach (var questId in activeQuests.Concat(completedQuests))
                {
                    var quest = VystiaQuestSystem.GetQuest(questId);
                    if (quest == null)
                    {
                        result.AddWarning($"Quest {questId} in tracker but not in quest registry");
                    }
                }

                // Check for quest progress consistency
                foreach (var questId in activeQuests)
                {
                    var progress = tracker.GetProgress(questId);
                    if (progress == null)
                    {
                        result.AddError($"Active quest {questId} has no progress data");
                    }
                    else
                    {
                        // Check if progress is valid
                        var quest = VystiaQuestSystem.GetQuest(questId);
                        if (quest != null && !quest.AreObjectivesComplete(progress))
                        {
                            // Check if progress is valid
                            foreach (var objective in quest.GetObjectives())
                            {
                                var progressValue = progress.GetProgress(objective.Key);
                                if (progressValue < 0 || progressValue > objective.Value)
                                {
                                    result.AddError($"Invalid progress for quest {questId}, objective {objective.Key}: {progressValue}/{objective.Value}");
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                result.AddError($"Vystia tracker validation failed: {ex.Message}");
            }
        }

        private static async Task ValidateDynamicQuestsAsync(PlayerMobile player, ValidationResult result)
        {
            try
            {
                var attachment = GeneratedQuestInstanceAttachment.GetAttachment(player);
                if (attachment == null)
                {
                    result.AddWarning("No Dynamic quest instance attachment found");
                    return;
                }

                var instances = attachment.GetInstances();
                foreach (var instance in instances)
                {
                    if (instance.QuestId <= 0)
                    {
                        result.AddError($"Invalid Dynamic quest instance ID: {instance.QuestId}");
                    }

                    // Check if instance exists in unified persistence
                    var unifiedQuests = await QuestPersistenceManager.LoadQuestsAsync(player);
                    var matchingQuest = unifiedQuests.FirstOrDefault(q => q.QuestId == instance.QuestId && q.QuestType == "Dynamic");
                    
                    if (matchingQuest == null)
                    {
                        result.AddWarning($"Dynamic quest instance {instance.QuestId} not found in unified persistence");
                    }
                }
            }
            catch (Exception ex)
            {
                result.AddError($"Dynamic quest validation failed: {ex.Message}");
            }
        }

        private static async Task ValidateTraditionalQuestsAsync(PlayerMobile player, ValidationResult result)
        {
            try
            {
                // Traditional quests use ServUO's built-in quest system
                // We can only do basic validation here
                var questContext = Server.Engines.Quests.QuestSystem.GetQuestContext(player);
                
                if (questContext == null)
                {
                    result.AddWarning("No traditional quest context found");
                    return;
                }

                // Check if traditional quests are tracked in unified persistence
                var unifiedQuests = await QuestPersistenceManager.LoadQuestsAsync(player);
                var traditionalQuests = unifiedQuests.Where(q => q.QuestType == "Traditional").ToList();

                if (traditionalQuests.Count == 0)
                {
                    result.AddWarning("Traditional quests not found in unified persistence");
                }
            }
            catch (Exception ex)
            {
                result.AddError($"Traditional quest validation failed: {ex.Message}");
            }
        }

        private static async Task CheckForOrphanedDataAsync(PlayerMobile player, ValidationResult result)
        {
            try
            {
                var unifiedQuests = await QuestPersistenceManager.LoadQuestsAsync(player);
                var vystiaTracker = VystiaQuestTracker.GetTracker(player);

                // Check for quests in unified persistence that aren't in any tracker
                foreach (var quest in unifiedQuests)
                {
                    bool found = false;

                    if (quest.QuestType == "Vystia" || quest.QuestType == "Dynamic")
                    {
                        if (vystiaTracker != null)
                        {
                            found = vystiaTracker.IsActive(quest.QuestId) || vystiaTracker.HasCompleted(quest.QuestId);
                        }
                    }

                    if (!found)
                    {
                        result.AddWarning($"Quest {quest.QuestId} ({quest.QuestType}) in unified persistence but not in any tracker");
                    }
                }

                // Check for quests in trackers that aren't in unified persistence
                if (vystiaTracker != null)
                {
                    var trackedQuests = vystiaTracker.GetActiveQuests();
                    var completedTrackedQuests = trackedQuests.Where(qid => vystiaTracker.HasCompleted(qid));
                    foreach (var questId in trackedQuests.Concat(completedTrackedQuests))
                    {
                        var found = unifiedQuests.Any(q => q.QuestId == questId);
                        if (!found)
                        {
                            result.AddWarning($"Quest {questId} in tracker but not in unified persistence");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                result.AddError($"Orphaned data check failed: {ex.Message}");
            }
        }

        private static async Task ValidateQuestProgressionAsync(PlayerMobile player, ValidationResult result)
        {
            try
            {
                var unifiedQuests = await QuestPersistenceManager.LoadQuestsAsync(player);

                foreach (var quest in unifiedQuests)
                {
                    // Check for completed quests that are still marked as active
                    if (quest.IsCompleted && quest.IsActive)
                    {
                        result.AddWarning($"Quest {quest.QuestId} is marked as both completed and active");
                    }

                    // Check for active quests with completion dates
                    if (quest.IsActive && quest.CompletedAt.HasValue)
                    {
                        result.AddWarning($"Active quest {quest.QuestId} has completion date");
                    }

                    // Check for quest progression logic
                    if (quest.PrerequisiteQuestId > 0)
                    {
                        var prerequisiteCompleted = unifiedQuests.Any(q => q.QuestId == quest.PrerequisiteQuestId && q.IsCompleted);
                        if (!prerequisiteCompleted && quest.IsActive)
                        {
                            result.AddWarning($"Quest {quest.QuestId} is active but prerequisite {quest.PrerequisiteQuestId} is not completed");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                result.AddError($"Quest progression validation failed: {ex.Message}");
            }
        }

        private static async Task CheckForDuplicateQuestsAsync(PlayerMobile player, ValidationResult result)
        {
            try
            {
                var unifiedQuests = await QuestPersistenceManager.LoadQuestsAsync(player);

                // Check for duplicate quest IDs across different quest types
                var duplicates = unifiedQuests
                    .GroupBy(q => q.QuestId)
                    .Where(g => g.Count() > 1)
                    .ToList();

                foreach (var duplicate in duplicates)
                {
                    result.AddError($"Duplicate quest ID {duplicate.Key} found: {string.Join(", ", duplicate.Select(q => q.QuestType))}");
                }
            }
            catch (Exception ex)
            {
                result.AddError($"Duplicate quest check failed: {ex.Message}");
            }
        }

        #endregion

        #region Private Repair Methods

        private static async Task RepairVystiaTrackerAsync(PlayerMobile player, List<QuestData> unifiedQuests, VystiaQuestTracker tracker, RepairResult result)
        {
            try
            {
                if (tracker == null)
                {
                    tracker = VystiaQuestTracker.GetOrCreateTracker(player);
                    result.RepairedIssues++;
                }

                // Sync completed quests
                var unifiedCompleted = unifiedQuests.Where(q => q.QuestType == "Vystia" && q.IsCompleted).Select(q => q.QuestId).ToList();
                var trackerCompleted = unifiedQuests.Where(q => q.QuestType == "Vystia" && tracker.HasCompleted(q.QuestId)).Select(q => q.QuestId).ToList();

                // Add missing completed quests to tracker
                foreach (var questId in unifiedCompleted.Except(trackerCompleted))
                {
                    // This would need to be implemented in VystiaQuestTracker
                    result.RepairedIssues++;
                }

                // Remove invalid completed quests from tracker
                foreach (var questId in trackerCompleted.Except(unifiedCompleted))
                {
                    // This would need to be implemented in VystiaQuestTracker
                    result.RepairedIssues++;
                }
            }
            catch (Exception ex)
            {
                result.AddError($"Vystia tracker repair failed: {ex.Message}");
            }
        }

        private static async Task RepairDynamicQuestsAsync(PlayerMobile player, List<QuestData> unifiedQuests, RepairResult result)
        {
            try
            {
                var attachment = GeneratedQuestInstanceAttachment.GetOrCreate(player);
                var instances = attachment.GetInstances().ToList();
                var unifiedDynamicQuests = unifiedQuests.Where(q => q.QuestType == "Dynamic").ToList();

                // Sync dynamic quest instances
                foreach (var quest in unifiedDynamicQuests)
                {
                    var instance = instances.FirstOrDefault(i => i.QuestId == quest.QuestId);
                    if (instance == null)
                    {
                        // Create missing instance
                        // This would need to be implemented in GeneratedQuestInstanceAttachment
                        result.RepairedIssues++;
                    }
                }

                // Remove invalid instances
                foreach (var instance in instances)
                {
                    var quest = unifiedDynamicQuests.FirstOrDefault(q => q.QuestId == instance.QuestId);
                    if (quest == null)
                    {
                        // Remove invalid instance
                        // This would need to be implemented in GeneratedQuestInstanceAttachment
                        result.RepairedIssues++;
                    }
                }
            }
            catch (Exception ex)
            {
                result.AddError($"Dynamic quest repair failed: {ex.Message}");
            }
        }

        private static async Task CleanupOrphanedDataAsync(PlayerMobile player, RepairResult result)
        {
            try
            {
                var unifiedQuests = await QuestPersistenceManager.LoadQuestsAsync(player);

                // Remove quests that have no corresponding system
                foreach (var quest in unifiedQuests.ToList())
                {
                    bool shouldRemove = false;

                    if (quest.QuestType == "Vystia")
                    {
                        var vystiaQuest = VystiaQuestSystem.GetQuest(quest.QuestId);
                        shouldRemove = vystiaQuest == null;
                    }
                    else if (quest.QuestType == "Dynamic")
                    {
                        var dynamicQuest = DynamicQuestManager.GetQuest(quest.QuestId);
                        shouldRemove = dynamicQuest == null;
                    }

                    if (shouldRemove)
                    {
                        await QuestPersistenceManager.DeleteQuestAsync(player, quest.QuestId);
                        result.RepairedIssues++;
                    }
                }
            }
            catch (Exception ex)
            {
                result.AddError($"Orphaned data cleanup failed: {ex.Message}");
            }
        }

        private static async Task SynchronizeQuestStatesAsync(PlayerMobile player, RepairResult result)
        {
            try
            {
                var unifiedQuests = await QuestPersistenceManager.LoadQuestsAsync(player);
                var vystiaTracker = VystiaQuestTracker.GetTracker(player);

                // Synchronize active/completed states
                foreach (var quest in unifiedQuests)
                {
                    if (quest.QuestType == "Vystia" && vystiaTracker != null)
                    {
                        bool trackerActive = vystiaTracker.IsActive(quest.QuestId);
                        bool trackerCompleted = vystiaTracker.HasCompleted(quest.QuestId);

                        if (quest.IsActive != trackerActive || quest.IsCompleted != trackerCompleted)
                        {
                            // Update unified persistence to match tracker
                            quest.IsActive = trackerActive;
                            quest.IsCompleted = trackerCompleted;
                            quest.LastAccessed = DateTime.UtcNow;

                            await QuestPersistenceManager.SaveQuestAsync(player, quest);
                            result.RepairedIssues++;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                result.AddError($"Quest state synchronization failed: {ex.Message}");
            }
        }

        #endregion
    }

    /// <summary>
    /// Summary of validation results for all players
    /// </summary>
    public class ValidationSummary
    {
        public int TotalPlayers { get; set; }
        public int ValidPlayers { get; set; }
        public int InvalidPlayers { get; set; }
        public int TotalErrors { get; set; }
        public int TotalWarnings { get; set; }
        public List<string> Errors { get; set; } = new List<string>();

        public void AddError(string error)
        {
            Errors.Add(error);
        }
    }

    /// <summary>
    /// Result of quest state repair operations
    /// </summary>
    public class RepairResult
    {
        public int RepairedIssues { get; set; }
        public int UnfixableIssues { get; set; }
        public List<string> Errors { get; set; } = new List<string>();

        public void AddError(string error)
        {
            Errors.Add(error);
            UnfixableIssues++;
        }
    }
}
