using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Server;
using Server.Mobiles;
using Server.Custom.VystiaClasses.Quests;
using Server.Custom.VystiaClasses;
using Newtonsoft.Json;

namespace Server.Services.QuestPersistence
{
    /// <summary>
    /// SQLite implementation of unified quest persistence
    /// Provides fast, reliable storage for all quest types
    /// </summary>
    public class SQLiteQuestPersistence : IQuestPersistence
    {
        private static string m_ConnectionString;
        private static bool m_Initialized = false;
        private static string m_DatabasePath;

        public void Initialize()
        {
            if (m_Initialized)
                return;

            try
            {
                var dataDir = Path.Combine(Core.BaseDirectory.Directory, "Data");
                m_DatabasePath = Path.Combine(dataDir, "unified_quests.db");

                m_ConnectionString = $"Data Source={m_DatabasePath};Version=3;";

                // Create database file if it doesn't exist
                if (!File.Exists(m_DatabasePath))
                {
                    SQLiteConnection.CreateFile(m_DatabasePath);
                }

                // Initialize schema
                InitializeSchema();

                m_Initialized = true;
                Console.WriteLine("[SQLiteQuestPersistence] Initialized successfully");
                Console.WriteLine($"[SQLiteQuestPersistence] Database: {m_DatabasePath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[SQLiteQuestPersistence] Initialization failed: {ex.Message}");
                throw;
            }
        }

        public async Task SaveQuestAsync(PlayerMobile player, QuestData questData)
        {
            if (!m_Initialized)
                return;

            try
            {
                using (var conn = new SQLiteConnection(m_ConnectionString))
                {
                    await conn.OpenAsync();

                    var sql = @"INSERT OR REPLACE INTO unified_quests
                                (player_serial, player_name, quest_id, quest_type, title, description, required_class, tier, 
                                 prerequisite_quest_id, created_at, last_accessed, completed_at, is_active, is_completed,
                                 metadata, objectives, progress, tags, json_data)
                                VALUES (@playerSerial, @playerName, @questId, @questType, @title, @description, @requiredClass, @tier,
                                        @prerequisiteQuestId, @createdAt, @lastAccessed, @completedAt, @isActive, @isCompleted,
                                        @metadata, @objectives, @progress, @tags, @jsonData)";

                    using (var cmd = new SQLiteCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@playerSerial", player.Serial);
                        cmd.Parameters.AddWithValue("@playerName", player.Name);
                        cmd.Parameters.AddWithValue("@questId", questData.QuestId);
                        cmd.Parameters.AddWithValue("@questType", questData.QuestType);
                        cmd.Parameters.AddWithValue("@title", questData.Title);
                        cmd.Parameters.AddWithValue("@description", questData.Description);
                        cmd.Parameters.AddWithValue("@requiredClass", questData.RequiredClass.ToString());
                        cmd.Parameters.AddWithValue("@tier", questData.Tier.ToString());
                        cmd.Parameters.AddWithValue("@prerequisiteQuestId", questData.PrerequisiteQuestId);
                        cmd.Parameters.AddWithValue("@createdAt", questData.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss"));
                        cmd.Parameters.AddWithValue("@lastAccessed", questData.LastAccessed.ToString("yyyy-MM-dd HH:mm:ss"));
                        cmd.Parameters.AddWithValue("@completedAt", questData.CompletedAt?.ToString("yyyy-MM-dd HH:mm:ss") ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@isActive", questData.IsActive);
                        cmd.Parameters.AddWithValue("@isCompleted", questData.IsCompleted);
                        cmd.Parameters.AddWithValue("@metadata", questData.Metadata != null ? JsonConvert.SerializeObject(questData.Metadata) : (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@objectives", questData.Objectives != null ? JsonConvert.SerializeObject(questData.Objectives) : (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@progress", questData.Progress != null ? JsonConvert.SerializeObject(questData.Progress) : (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@tags", questData.Tags != null ? JsonConvert.SerializeObject(questData.Tags) : (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@jsonData", JsonConvert.SerializeObject(questData));

                        await cmd.ExecuteNonQueryAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[SQLiteQuestPersistence] Error saving quest {questData.QuestId} for {player.Name}: {ex.Message}");
            }
        }

        public async Task<List<QuestData>> LoadQuestsAsync(PlayerMobile player)
        {
            if (!m_Initialized)
                return new List<QuestData>();

            try
            {
                using (var conn = new SQLiteConnection(m_ConnectionString))
                {
                    await conn.OpenAsync();

                    var sql = @"SELECT quest_id, quest_type, title, description, required_class, tier, prerequisite_quest_id,
                                     created_at, last_accessed, completed_at, is_active, is_completed, metadata, objectives, progress, tags, json_data
                              FROM unified_quests
                              WHERE player_serial = @playerSerial
                              ORDER BY last_accessed DESC";

                    using (var cmd = new SQLiteCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@playerSerial", player.Serial);

                        var quests = new List<QuestData>();

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                var questData = new QuestData
                                {
                                    QuestId = reader.GetInt32("quest_id"),
                                    QuestType = reader.GetString("quest_type"),
                                    Title = reader.GetString("title"),
                                    Description = reader.GetString("description"),
                                    RequiredClass = Enum.TryParse<PlayerClassTypeV2>(reader.GetString("required_class"), out var classType) ? classType : PlayerClassTypeV2.None,
                                    Tier = Enum.TryParse<QuestTier>(reader.GetString("tier"), out var tier) ? tier : QuestTier.Initiation,
                                    PrerequisiteQuestId = reader.GetInt32("prerequisite_quest_id"),
                                    CreatedAt = DateTime.Parse(reader.GetString("created_at")),
                                    LastAccessed = DateTime.Parse(reader.GetString("last_accessed")),
                                    CompletedAt = reader.IsDBNull("completed_at") ? (DateTime?)null : DateTime.Parse(reader.GetString("completed_at")),
                                    IsActive = reader.GetBoolean("is_active"),
                                    IsCompleted = reader.GetBoolean("is_completed"),
                                    Metadata = reader.IsDBNull("metadata") ? null : JsonConvert.DeserializeObject<Dictionary<string, object>>(reader.GetString("metadata")),
                                    Objectives = reader.IsDBNull("objectives") ? null : JsonConvert.DeserializeObject<Dictionary<string, int>>(reader.GetString("objectives")),
                                    Progress = reader.IsDBNull("progress") ? null : JsonConvert.DeserializeObject<Dictionary<string, object>>(reader.GetString("progress")),
                                    Tags = reader.IsDBNull("tags") ? null : JsonConvert.DeserializeObject<List<string>>(reader.GetString("tags"))
                                };

                                quests.Add(questData);
                            }
                        }

                        return quests;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[SQLiteQuestPersistence] Error loading quests for {player.Name}: {ex.Message}");
                return new List<QuestData>();
            }
        }

        public async Task DeleteQuestAsync(PlayerMobile player, int questId)
        {
            if (!m_Initialized)
                return;

            try
            {
                using (var conn = new SQLiteConnection(m_ConnectionString))
                {
                    await conn.OpenAsync();

                    var sql = @"DELETE FROM unified_quests 
                              WHERE player_serial = @playerSerial AND quest_id = @questId";

                    using (var cmd = new SQLiteCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@playerSerial", player.Serial);
                        cmd.Parameters.AddWithValue("@questId", questId);

                        await cmd.ExecuteNonQueryAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[SQLiteQuestPersistence] Error deleting quest {questId} for {player.Name}: {ex.Message}");
            }
        }

        public async Task UpdateQuestProgressAsync(PlayerMobile player, int questId, QuestProgress progress)
        {
            if (!m_Initialized)
                return;

            try
            {
                using (var conn = new SQLiteConnection(m_ConnectionString))
                {
                    await conn.OpenAsync();

                    var sql = @"UPDATE unified_quests 
                              SET progress = @progress, last_accessed = @lastAccessed
                              WHERE player_serial = @playerSerial AND quest_id = @questId";

                    using (var cmd = new SQLiteCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@playerSerial", player.Serial);
                        cmd.Parameters.AddWithValue("@questId", questId);
                        cmd.Parameters.AddWithValue("@progress", JsonConvert.SerializeObject(progress));
                        cmd.Parameters.AddWithValue("@lastAccessed", DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"));

                        await cmd.ExecuteNonQueryAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[SQLiteQuestPersistence] Error updating progress for quest {questId} for {player.Name}: {ex.Message}");
            }
        }

        public async Task<QuestProgress> GetQuestProgressAsync(PlayerMobile player, int questId)
        {
            if (!m_Initialized)
                return null;

            try
            {
                using (var conn = new SQLiteConnection(m_ConnectionString))
                {
                    await conn.OpenAsync();

                    var sql = @"SELECT progress FROM unified_quests 
                              WHERE player_serial = @playerSerial AND quest_id = @questId";

                    using (var cmd = new SQLiteCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@playerSerial", player.Serial);
                        cmd.Parameters.AddWithValue("@questId", questId);

                        var result = await cmd.ExecuteScalarAsync();
                        if (result != null && result != DBNull.Value)
                        {
                            return JsonConvert.DeserializeObject<QuestProgress>(result.ToString());
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[SQLiteQuestPersistence] Error getting progress for quest {questId} for {player.Name}: {ex.Message}");
                return null;
            }
        }

        public async Task MarkQuestCompletedAsync(PlayerMobile player, int questId)
        {
            if (!m_Initialized)
                return;

            try
            {
                using (var conn = new SQLiteConnection(m_ConnectionString))
                {
                    await conn.OpenAsync();

                    var sql = @"UPDATE unified_quests 
                              SET is_completed = 1, completed_at = @completedAt, last_accessed = @lastAccessed
                              WHERE player_serial = @playerSerial AND quest_id = @questId";

                    using (var cmd = new SQLiteCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@playerSerial", player.Serial);
                        cmd.Parameters.AddWithValue("@questId", questId);
                        cmd.Parameters.AddWithValue("@completedAt", DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"));
                        cmd.Parameters.AddWithValue("@lastAccessed", DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"));

                        await cmd.ExecuteNonQueryAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[SQLiteQuestPersistence] Error marking quest {questId} completed for {player.Name}: {ex.Message}");
            }
        }

        public async Task<bool> HasCompletedQuestAsync(PlayerMobile player, int questId)
        {
            if (!m_Initialized)
                return false;

            try
            {
                using (var conn = new SQLiteConnection(m_ConnectionString))
                {
                    await conn.OpenAsync();

                    var sql = @"SELECT is_completed FROM unified_quests 
                              WHERE player_serial = @playerSerial AND quest_id = @questId";

                    using (var cmd = new SQLiteCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@playerSerial", player.Serial);
                        cmd.Parameters.AddWithValue("@questId", questId);

                        var result = await cmd.ExecuteScalarAsync();
                        return result != null && Convert.ToBoolean(result);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[SQLiteQuestPersistence] Error checking completion for quest {questId} for {player.Name}: {ex.Message}");
                return false;
            }
        }

        public async Task<List<int>> GetActiveQuestsAsync(PlayerMobile player)
        {
            if (!m_Initialized)
                return new List<int>();

            try
            {
                using (var conn = new SQLiteConnection(m_ConnectionString))
                {
                    await conn.OpenAsync();

                    var sql = @"SELECT quest_id FROM unified_quests 
                              WHERE player_serial = @playerSerial AND is_active = 1
                              ORDER BY last_accessed DESC";

                    using (var cmd = new SQLiteCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@playerSerial", player.Serial);

                        var questIds = new List<int>();

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                questIds.Add(reader.GetInt32("quest_id"));
                            }
                        }

                        return questIds;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[SQLiteQuestPersistence] Error getting active quests for {player.Name}: {ex.Message}");
                return new List<int>();
            }
        }

        public async Task<List<int>> GetCompletedQuestsAsync(PlayerMobile player)
        {
            if (!m_Initialized)
                return new List<int>();

            try
            {
                using (var conn = new SQLiteConnection(m_ConnectionString))
                {
                    await conn.OpenAsync();

                    var sql = @"SELECT quest_id FROM unified_quests 
                              WHERE player_serial = @playerSerial AND is_completed = 1
                              ORDER BY completed_at DESC";

                    using (var cmd = new SQLiteCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@playerSerial", player.Serial);

                        var questIds = new List<int>();

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                questIds.Add(reader.GetInt32("quest_id"));
                            }
                        }

                        return questIds;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[SQLiteQuestPersistence] Error getting completed quests for {player.Name}: {ex.Message}");
                return new List<int>();
            }
        }

        public async Task<ValidationResult> ValidateQuestDataAsync(PlayerMobile player)
        {
            var result = new ValidationResult();

            if (!m_Initialized)
            {
                result.AddError("Persistence system not initialized");
                return result;
            }

            try
            {
                var quests = await LoadQuestsAsync(player);
                
                // Check for duplicate quest IDs
                var duplicateIds = quests.GroupBy(q => q.QuestId).Where(g => g.Count() > 1).Select(g => g.Key).ToList();
                foreach (var duplicateId in duplicateIds)
                {
                    result.AddError($"Duplicate quest ID: {duplicateId}");
                }

                // Check for invalid quest types
                var invalidTypes = quests.Where(q => !IsValidQuestType(q.QuestType)).ToList();
                foreach (var invalidType in invalidTypes)
                {
                    result.AddError($"Invalid quest type: {invalidType.QuestType}");
                }

                // Check for missing required fields
                var incompleteQuests = quests.Where(q => string.IsNullOrWhiteSpace(q.Title) || string.IsNullOrWhiteSpace(q.Description)).ToList();
                foreach (var incompleteQuest in incompleteQuests)
                {
                    result.AddError($"Incomplete quest data for quest {incompleteQuest.QuestId}");
                }

                // Check for corrupted JSON data
                var corruptedQuests = quests.Where(q => q.JsonData == null).ToList();
                foreach (var corruptedQuest in corruptedQuests)
                {
                    result.AddWarning($"Quest {corruptedQuest.QuestId} has corrupted JSON data");
                }

                if (result.Errors.Count == 0)
                {
                    result.IsValid = true;
                }
            }
            catch (Exception ex)
            {
                result.AddError($"Validation failed: {ex.Message}");
                result.IsValid = false;
            }

            return result;
        }

        public async Task BackupQuestDataAsync(PlayerMobile player)
        {
            if (!m_Initialized)
                return;

            try
            {
                var backupPath = Path.Combine(QuestPersistenceManager.GetBackupDirectory(), $"{player.Serial}_{DateTime.Now:yyyyMMdd_HHmmss}.json");
                
                var quests = await LoadQuestsAsync(player);
                var backupData = new
                {
                    PlayerName = player.Name,
                    PlayerSerial = player.Serial,
                    BackupDate = DateTime.UtcNow,
                    Quests = quests
                };

                string json = JsonConvert.SerializeObject(backupData, Formatting.Indented);
                File.WriteAllText(backupPath, json);

                Console.WriteLine($"[SQLiteQuestPersistence] Backed up {quests.Count} quests for {player.Name} to {backupPath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[SQLiteQuestPersistence] Error backing up quest data for {player.Name}: {ex.Message}");
            }
        }

        public async Task RestoreQuestDataAsync(PlayerMobile player, DateTime backupDate)
        {
            if (!m_Initialized)
                return;

            try
            {
                var backupPath = Path.Combine(QuestPersistenceManager.GetBackupDirectory(), $"{player.Serial}_{backupDate:yyyyMMdd_HHmmss}.json");
                
                if (!File.Exists(backupPath))
                {
                    Console.WriteLine($"[SQLiteQuestPersistence] No backup found for {player.Name} at {backupPath}");
                    return;
                }

                string json = File.ReadAllText(backupPath);
                var backupData = JsonConvert.DeserializeObject<BackupData>(json);

                // Delete existing quests for this player
                var existingQuests = await LoadQuestsAsync(player);
                foreach (var quest in existingQuests)
                {
                    await DeleteQuestAsync(player, quest.QuestId);
                }

                // Restore from backup
                foreach (var questData in backupData.Quests)
                {
                    await SaveQuestAsync(player, questData);
                }

                Console.WriteLine($"[SQLiteQuestPersistence] Restored {backupData.Quests.Count} quests for {player.Name} from {backupPath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[SQLiteQuestPersistence] Error restoring quest data for {player.Name}: {ex.Message}");
            }
        }

        public async Task<List<DateTime>> GetBackupDatesAsync(PlayerMobile player)
        {
            if (!m_Initialized)
                return new List<DateTime>();

            try
            {
                var backupFiles = Directory.GetFiles(QuestPersistenceManager.GetBackupDirectory(), $"{player.Serial}_*.json");
                var dates = new List<DateTime>();

                foreach (var file in backupFiles)
                {
                    var fileName = Path.GetFileNameWithoutExtension(file);
                    if (DateTime.TryParse(fileName.Split('_').Last(), out DateTime date))
                    {
                        dates.Add(date);
                    }
                }

                return dates.OrderByDescending(d => d).ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[SQLiteQuestPersistence] Error getting backup dates for {player.Name}: {ex.Message}");
                return new List<DateTime>();
            }
        }

        #region Private Methods

        private void InitializeSchema()
        {
            try
            {
                using (var conn = new SQLiteConnection(m_ConnectionString))
                {
                    conn.Open();

                    // Create unified quests table
                    var createQuestsTable = @"
                        CREATE TABLE IF NOT EXISTS unified_quests (
                            id INTEGER PRIMARY KEY AUTOINCREMENT,
                            player_serial INTEGER NOT NULL,
                            player_name TEXT NOT NULL,
                            quest_id INTEGER NOT NULL,
                            quest_type TEXT NOT NULL,
                            title TEXT NOT NULL,
                            description TEXT NOT NULL,
                            required_class TEXT NOT NULL,
                            tier TEXT NOT NULL,
                            prerequisite_quest_id INTEGER DEFAULT 0,
                            created_at TEXT NOT NULL,
                            last_accessed TEXT NOT NULL,
                            completed_at TEXT,
                            is_active BOOLEAN DEFAULT 1,
                            is_completed BOOLEAN DEFAULT 0,
                            metadata TEXT,
                            objectives TEXT,
                            progress TEXT,
                            tags TEXT,
                            json_data TEXT
                        )";

                    using (var cmd = new SQLiteCommand(createQuestsTable, conn))
                    {
                        cmd.ExecuteNonQuery();
                    }

                    // Create indexes for performance
                    var createIndexes = @"
                        CREATE INDEX IF NOT EXISTS idx_unified_quests_player ON unified_quests(player_serial);
                        CREATE INDEX IF NOT EXISTS idx_unified_quests_quest_id ON unified_quests(quest_id);
                        CREATE INDEX IF NOT EXISTS idx_unified_quests_player_quest ON unified_quests(player_serial, quest_id);
                        CREATE INDEX IF NOT EXISTS idx_unified_quests_last_accessed ON unified_quests(last_accessed DESC);
                        CREATE INDEX IF NOT EXISTS idx_unified_quests_completed_at ON unified_quests(completed_at DESC);
                    ";

                    using (var cmd = new SQLiteCommand(createIndexes, conn))
                    {
                        cmd.ExecuteNonQuery();
                    }

                    Console.WriteLine("[SQLiteQuestPersistence] Database schema initialized");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[SQLiteQuestPersistence] Error initializing schema: {ex.Message}");
                throw;
            }
        }

        private bool IsValidQuestType(string questType)
        {
            return questType == "Vystia" || questType == "Dynamic" || questType == "Traditional";
        }

        #endregion
    }

    /// <summary>
    /// Data structure for quest backups
    /// </summary>
    public class BackupData
    {
        public string PlayerName { get; set; }
        public int PlayerSerial { get; set; }
        public DateTime BackupDate { get; set; }
        public List<QuestData> Quests { get; set; }
    }
}
