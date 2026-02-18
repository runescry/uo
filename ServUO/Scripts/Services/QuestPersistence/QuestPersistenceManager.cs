using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Server;
using Server.Mobiles;
using Server.Engines.Quests;
using Server.Custom.VystiaClasses.Quests;
using Server.Services.LLM;
using Newtonsoft.Json;

namespace Server.Services.QuestPersistence
{
    /// <summary>
    /// Unified quest persistence manager that coordinates all quest systems
    /// Provides consistent storage and retrieval across Vystia, Dynamic, and Traditional quests
    /// </summary>
    public static class QuestPersistenceManager
    {
        private static IQuestPersistence s_PersistenceProvider;
        private static bool s_Initialized = false;
        private static readonly string s_BackupDirectory = Path.Combine(Core.BaseDirectory, "Data", "QuestBackups");
        private static readonly string s_DataDirectory = Path.Combine(Core.BaseDirectory, "Data", "UnifiedQuests");

        /// <summary>
        /// Initialize the unified quest persistence system
        /// </summary>
        public static void Initialize()
        {
            if (s_Initialized)
                return;

            s_Initialized = true;

            try
            {
                // Ensure data directories exist
                Directory.CreateDirectory(s_DataDirectory);
                Directory.CreateDirectory(s_BackupDirectory);

                // Use SQLite as the default persistence provider
                s_PersistenceProvider = new SQLiteQuestPersistence();

                Console.WriteLine("[QuestPersistenceManager] Initialized with SQLite persistence provider");
                Console.WriteLine($"[QuestPersistenceManager] Data directory: {s_DataDirectory}");
                Console.WriteLine($"[QuestPersistenceManager] Backup directory: {s_BackupDirectory}");

                // Initialize the provider
                if (s_PersistenceProvider is SQLiteQuestPersistence sqliteProvider)
                {
                    sqliteProvider.Initialize();
                }

                Console.WriteLine("[QuestPersistenceManager] Ready for unified quest persistence");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[QuestPersistenceManager] Error initializing: {ex.Message}");
                Console.WriteLine("[QuestPersistenceManager] Falling back to null provider - quest persistence will be limited");
                s_PersistenceProvider = new NullQuestPersistence();
            }
        }

        /// <summary>
        /// Save quest data for a player
        /// </summary>
        public static async Task SaveQuestAsync(PlayerMobile player, QuestData questData)
        {
            if (player == null || player.Deleted || questData == null)
                return;

            try
            {
                await s_PersistenceProvider.SaveQuestAsync(player, questData);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[QuestPersistenceManager] Error saving quest {questData.QuestId} for {player.Name}: {ex.Message}");
            }
        }

        /// <summary>
        /// Load quest data for a player
        /// </summary>
        public static async Task<List<QuestData>> LoadQuestsAsync(PlayerMobile player)
        {
            if (player == null || player.Deleted)
                return new List<QuestData>();

            try
            {
                return await s_PersistenceProvider.LoadQuestsAsync(player);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[QuestPersistenceManager] Error loading quests for {player.Name}: {ex.Message}");
                return new List<QuestData>();
            }
        }

        /// <summary>
        /// Delete quest data for a player
        /// </summary>
        public static async Task DeleteQuestAsync(PlayerMobile player, int questId)
        {
            if (player == null || player.Deleted)
                return;

            try
            {
                await s_PersistenceProvider.DeleteQuestAsync(player, questId);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[QuestPersistenceManager] Error deleting quest {questId} for {player.Name}: {ex.Message}");
            }
        }

        /// <summary>
        /// Update quest progress for a player
        /// </summary>
        public static async Task UpdateQuestProgressAsync(PlayerMobile player, int questId, QuestProgress progress)
        {
            if (player == null || player.Deleted || progress == null)
                return;

            try
            {
                await s_PersistenceProvider.UpdateQuestProgressAsync(player, questId, progress);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[QuestPersistenceManager] Error updating progress for quest {questId} for {player.Name}: {ex.Message}");
            }
        }

        /// <summary>
        /// Get quest progress for a player
        /// </summary>
        public static async Task<QuestProgress> GetQuestProgressAsync(PlayerMobile player, int questId)
        {
            if (player == null || player.Deleted)
                return null;

            try
            {
                return await s_PersistenceProvider.GetQuestProgressAsync(player, questId);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[QuestPersistenceManager] Error getting progress for quest {questId} for {player.Name}: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Mark quest as completed for a player
        /// </summary>
        public static async Task MarkQuestCompletedAsync(PlayerMobile player, int questId)
        {
            if (player == null || player.Deleted)
                return;

            try
            {
                await s_PersistenceProvider.MarkQuestCompletedAsync(player, questId);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[QuestPersistenceManager] Error marking quest {questId} completed for {player.Name}: {ex.Message}");
            }
        }

        /// <summary>
        /// Check if player has completed a quest
        /// </summary>
        public static async Task<bool> HasCompletedQuestAsync(PlayerMobile player, int questId)
        {
            if (player == null || player.Deleted)
                return false;

            try
            {
                return await s_PersistenceProvider.HasCompletedQuestAsync(player, questId);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[QuestPersistenceManager] Error checking completion for quest {questId} for {player.Name}: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Get all active quests for a player
        /// </summary>
        public static async Task<List<int>> GetActiveQuestsAsync(PlayerMobile player)
        {
            if (player == null || player.Deleted)
                return new List<int>();

            try
            {
                return await s_PersistenceProvider.GetActiveQuestsAsync(player);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[QuestPersistenceManager] Error getting active quests for {player.Name}: {ex.Message}");
                return new List<int>();
            }
        }

        /// <summary>
        /// Get all completed quests for a player
        /// </summary>
        public static async Task<List<int>> GetCompletedQuestsAsync(PlayerMobile player)
        {
            if (player == null || player.Deleted)
                return new List<int>();

            try
            {
                return await s_PersistenceProvider.GetCompletedQuestsAsync(player);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[QuestPersistenceManager] Error getting completed quests for {player.Name}: {ex.Message}");
                return new List<int>();
            }
        }

        /// <summary>
        /// Validate quest data integrity for a player
        /// </summary>
        public static async Task<ValidationResult> ValidateQuestDataAsync(PlayerMobile player)
        {
            if (player == null || player.Deleted)
            {
                return new ValidationResult { IsValid = false, Errors = { "Invalid player" } };
            }

            try
            {
                return await s_PersistenceProvider.ValidateQuestDataAsync(player);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[QuestPersistenceManager] Error validating quest data for {player.Name}: {ex.Message}");
                return new ValidationResult { IsValid = false, Errors = { ex.Message } };
            }
        }

        /// <summary>
        /// Backup quest data for a player
        /// </summary>
        public static async Task BackupQuestDataAsync(PlayerMobile player)
        {
            if (player == null || player.Deleted)
                return;

            try
            {
                await s_PersistenceProvider.BackupQuestDataAsync(player);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[QuestPersistenceManager] Error backing up quest data for {player.Name}: {ex.Message}");
            }
        }

        /// <summary>
        /// Restore quest data from backup for a player
        /// </summary>
        public static async Task RestoreQuestDataAsync(PlayerMobile player, DateTime backupDate)
        {
            if (player == null || player.Deleted)
                return;

            try
            {
                await s_PersistenceProvider.RestoreQuestDataAsync(player, backupDate);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[QuestPersistenceManager] Error restoring quest data for {player.Name}: {ex.Message}");
            }
        }

        /// <summary>
        /// Get available backup dates for a player
        /// </summary>
        public static async Task<List<DateTime>> GetBackupDatesAsync(PlayerMobile player)
        {
            if (player == null || player.Deleted)
                return new List<DateTime>();

            try
            {
                return await s_PersistenceProvider.GetBackupDatesAsync(player);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[QuestPersistenceManager] Error getting backup dates for {player.Name}: {ex.Message}");
                return new List<DateTime>();
            }
        }

        /// <summary>
        /// Migrate quest data from legacy systems to unified format
        /// </summary>
        public static async Task<MigrationResult> MigrateLegacyQuestDataAsync(PlayerMobile player)
        {
            var result = new MigrationResult();

            try
            {
                Console.WriteLine($"[QuestPersistenceManager] Starting migration for {player.Name}...");

                // Migrate Vystia quests from XmlAttachment
                await MigrateVystiaQuestsAsync(player, result);

                // Migrate Dynamic quests from memory (if any)
                await MigrateDynamicQuestsAsync(player, result);

                // Migrate Traditional quests from ServUO system
                await MigrateTraditionalQuestsAsync(player, result);

                Console.WriteLine($"[QuestPersistenceManager] Migration completed for {player.Name}: {result.TotalMigrated} quests migrated, {result.Errors.Count} errors");

                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[QuestPersistenceManager] Error during migration for {player.Name}: {ex.Message}");
                result.AddError($"Migration failed: {ex.Message}");
                return result;
            }
        }

        /// <summary>
        /// Get statistics about quest persistence
        /// </summary>
        public static async Task<QuestPersistenceStats> GetStatsAsync()
        {
            var stats = new QuestPersistenceStats();

            try
            {
                // Count total players with quest data
                var allPlayers = World.Mobiles.Values.OfType<PlayerMobile>().ToList();
                stats.TotalPlayers = allPlayers.Count;
                stats.PlayersWithQuests = 0;
                stats.TotalQuests = 0;

                foreach (var player in allPlayers)
                {
                    var quests = await LoadQuestsAsync(player);
                    if (quests.Count > 0)
                    {
                        stats.PlayersWithQuests++;
                        stats.TotalQuests += quests.Count;
                    }
                }

                stats.ActiveQuests = await CountActiveQuestsAsync();
                stats.CompletedQuests = await CountCompletedQuestsAsync();
                stats.BackupCount = Directory.GetFiles(s_BackupDirectory, "*.json").Length;

                return stats;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[QuestPersistenceManager] Error getting stats: {ex.Message}");
                return new QuestPersistenceStats();
            }
        }

        /// <summary>
        /// Get the backup directory path
        /// </summary>
        public static string GetBackupDirectory()
        {
            return s_BackupDirectory;
        }

        /// <summary>
        /// Get the data directory path
        /// </summary>
        public static string GetDataDirectory()
        {
            return s_DataDirectory;
        }

        /// <summary>
        /// Set the persistence provider (for testing)
        /// </summary>
        public static void SetProvider(IQuestPersistence provider)
        {
            s_PersistenceProvider = provider ?? new NullQuestPersistence();
        }

        #region Private Migration Methods

        private static async Task MigrateVystiaQuestsAsync(PlayerMobile player, MigrationResult result)
        {
            try
            {
                var tracker = VystiaQuestTracker.GetTracker(player);
                if (tracker == null)
                    return;

                var activeQuests = tracker.GetActiveQuests();
                var completedQuests = activeQuests.Where(qid => tracker.HasCompleted(qid)).ToList();

                foreach (var questId in completedQuests.Concat(activeQuests))
                {
                    var quest = VystiaQuestSystem.GetQuest(questId);
                    if (quest != null)
                    {
                        var questData = QuestData.FromVystiaQuest(quest, player);
                        await SaveQuestAsync(player, questData);
                        result.VystiaQuestsMigrated++;
                    }
                }
            }
            catch (Exception ex)
            {
                result.AddError($"Vystia quest migration failed: {ex.Message}");
            }
        }

        private static async Task MigrateDynamicQuestsAsync(PlayerMobile player, MigrationResult result)
        {
            try
            {
                var tracker = VystiaQuestTracker.GetTracker(player);
                if (tracker == null)
                    return;

                var activeQuests = tracker.GetActiveQuests();
                var completedQuests = activeQuests.Where(qid => tracker.HasCompleted(qid)).ToList();

                foreach (var questId in completedQuests.Concat(activeQuests))
                {
                    var quest = DynamicQuestManager.GetQuest(questId);
                    if (quest != null)
                    {
                        var questData = QuestData.FromDynamicQuest(quest, player);
                        await SaveQuestAsync(player, questData);
                        result.DynamicQuestsMigrated++;
                    }
                }
            }
            catch (Exception ex)
            {
                result.AddError($"Dynamic quest migration failed: {ex.Message}");
            }
        }

        private static async Task MigrateTraditionalQuestsAsync(PlayerMobile player, MigrationResult result)
        {
            try
            {
                // Traditional quests use ServUO's built-in quest system
                // We can extract basic information but full migration would require
                // access to the quest registry or serialization system
                // TODO: Implement proper traditional quest migration when API is available
                result.AddWarning("Traditional quest migration not yet implemented");
            }
            catch (Exception ex)
            {
                result.AddError($"Traditional quest migration failed: {ex.Message}");
            }
        }

        private static async Task<int> CountActiveQuestsAsync()
        {
            int count = 0;
            foreach (var player in World.Mobiles.Values.OfType<PlayerMobile>())
            {
                var activeQuests = await GetActiveQuestsAsync(player);
                count += activeQuests.Count;
            }
            return count;
        }

        private static async Task<int> CountCompletedQuestsAsync()
        {
            int count = 0;
            foreach (var player in World.Mobiles.Values.OfType<PlayerMobile>())
            {
                var completedQuests = await GetCompletedQuestsAsync(player);
                count += completedQuests.Count;
            }
            return count;
        }

        #endregion
    }

    /// <summary>
    /// Statistics about quest persistence
    /// </summary>
    public class QuestPersistenceStats
    {
        public int TotalPlayers { get; set; }
        public int PlayersWithQuests { get; set; }
        public int TotalQuests { get; set; }
        public int ActiveQuests { get; set; }
        public int CompletedQuests { get; set; }
        public int BackupCount { get; set; }
    }

    /// <summary>
    /// Result of quest data migration
    /// </summary>
    public class MigrationResult
    {
        public int VystiaQuestsMigrated { get; set; }
        public int DynamicQuestsMigrated { get; set; }
        public int TraditionalQuestsMigrated { get; set; }
        public int TotalMigrated => VystiaQuestsMigrated + DynamicQuestsMigrated + TraditionalQuestsMigrated;
        public List<string> Errors { get; set; } = new List<string>();

        public void AddError(string error)
        {
            Errors.Add(error);
        }

        public void AddWarning(string warning)
        {
            // Warnings are just logged but don't count as errors
            Console.WriteLine($"[MigrationResult] Warning: {warning}");
        }
    }

    /// <summary>
    /// Null implementation for when no persistence is available
    /// </summary>
    public class NullQuestPersistence : IQuestPersistence
    {
        public Task SaveQuestAsync(PlayerMobile player, QuestData questData) => Task.CompletedTask;
        public Task<List<QuestData>> LoadQuestsAsync(PlayerMobile player) => Task.FromResult(new List<QuestData>());
        public Task DeleteQuestAsync(PlayerMobile player, int questId) => Task.CompletedTask;
        public Task UpdateQuestProgressAsync(PlayerMobile player, int questId, QuestProgress progress) => Task.CompletedTask;
        public Task<QuestProgress> GetQuestProgressAsync(PlayerMobile player, int questId) => Task.FromResult<QuestProgress>(null);
        public Task MarkQuestCompletedAsync(PlayerMobile player, int questId) => Task.CompletedTask;
        public Task<bool> HasCompletedQuestAsync(PlayerMobile player, int questId) => Task.FromResult(false);
        public Task<List<int>> GetActiveQuestsAsync(PlayerMobile player) => Task.FromResult(new List<int>());
        public Task<List<int>> GetCompletedQuestsAsync(PlayerMobile player) => Task.FromResult(new List<int>());
        public Task<ValidationResult> ValidateQuestDataAsync(PlayerMobile player) => Task.FromResult(new ValidationResult { IsValid = true });
        public Task BackupQuestDataAsync(PlayerMobile player) => Task.CompletedTask;
        public Task RestoreQuestDataAsync(PlayerMobile player, DateTime backupDate) => Task.CompletedTask;
        public Task<List<DateTime>> GetBackupDatesAsync(PlayerMobile player) => Task.FromResult(new List<DateTime>());
    }
}
