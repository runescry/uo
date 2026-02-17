using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Server.Services.LLM;
using Newtonsoft.Json;
using Server;

namespace Server.Services.LLM
{
    /// <summary>
    /// SQLite-based memory database implementation
    /// Simple, file-based, no external dependencies beyond System.Data.SQLite
    /// </summary>
    public class SQLiteMemoryDatabase
    {
        private static bool m_Initialized = false;
        private static bool m_Available = false;
        private static string m_ConnectionString;
        private static string m_DatabasePath;

        public static void Initialize()
        {
            if (m_Initialized)
                return;

            m_Initialized = true;

            if (!LLMMemoryConfig.Enabled || LLMMemoryConfig.DatabaseProvider != "SQLite")
            {
                Console.WriteLine("[SQLiteMemoryDatabase] SQLite database disabled in configuration");
                return;
            }

            try
            {
                // Default database path if not specified
                if (string.IsNullOrEmpty(LLMMemoryConfig.DatabaseConnectionString))
                {
                    var dataDir = Path.Combine(Core.BaseDirectory.Directory, "Data");
                    if (!Directory.Exists(dataDir))
                        Directory.CreateDirectory(dataDir);
                    
                    m_DatabasePath = Path.Combine(dataDir, "llm_memories.db");
                }
                else
                {
                    m_DatabasePath = LLMMemoryConfig.DatabaseConnectionString;
                }

                m_ConnectionString = $"Data Source={m_DatabasePath};Version=3;";
                
                // Console.WriteLine("[SQLiteMemoryDatabase] Initializing SQLite database...");
                // Console.WriteLine($"[SQLiteMemoryDatabase] Database path: {m_DatabasePath}");

                // Create database file if it doesn't exist
                if (!File.Exists(m_DatabasePath))
                {
                    SQLiteConnection.CreateFile(m_DatabasePath);
                    // Console.WriteLine("[SQLiteMemoryDatabase] Created new database file");
                }

                // Initialize schema
                InitializeSchema();

                // Test connection
                using (var conn = new SQLiteConnection(m_ConnectionString))
                {
                    conn.Open();
                    m_Available = true;
                    // Console.WriteLine("[SQLiteMemoryDatabase] Successfully connected to SQLite");
                }
            }
            catch (Exception ex)
            {
                m_Available = false;
                Console.WriteLine($"[SQLiteMemoryDatabase] ERROR: SQLite initialization failed: {ex.Message}");
                Console.WriteLine($"[SQLiteMemoryDatabase] ERROR: Exception type: {ex.GetType().Name}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"[SQLiteMemoryDatabase] ERROR: Inner exception: {ex.InnerException.Message}");
                }
                Console.WriteLine("[SQLiteMemoryDatabase] WARNING: Using in-memory fallback (data will not persist)");
            }
        }

        private static void InitializeSchema()
        {
            try
            {
                using (var conn = new SQLiteConnection(m_ConnectionString))
                {
                    conn.Open();

                    // Create memories table
                    var createMemoriesTable = @"
                        CREATE TABLE IF NOT EXISTS llm_npc_memories (
                            id INTEGER PRIMARY KEY AUTOINCREMENT,
                            npc_serial INTEGER NOT NULL,
                            npc_name TEXT NOT NULL,
                            player_name TEXT NOT NULL,
                            memory_type TEXT NOT NULL,
                            content TEXT NOT NULL,
                            importance INTEGER NOT NULL DEFAULT 5,
                            context TEXT,
                            created_at TEXT NOT NULL,
                            last_accessed TEXT NOT NULL,
                            expires_at TEXT,
                            location_x INTEGER,
                            location_y INTEGER,
                            location_z INTEGER,
                            map_name TEXT
                        )";

                    using (var cmd = new SQLiteCommand(createMemoriesTable, conn))
                    {
                        cmd.ExecuteNonQuery();
                    }

                    // Create indexes
                    var createIndexes = @"
                        CREATE INDEX IF NOT EXISTS idx_memories_npc_player ON llm_npc_memories(npc_serial, player_name);
                        CREATE INDEX IF NOT EXISTS idx_memories_importance ON llm_npc_memories(importance DESC);
                        CREATE INDEX IF NOT EXISTS idx_memories_last_accessed ON llm_npc_memories(last_accessed DESC);
                        CREATE INDEX IF NOT EXISTS idx_memories_name_location ON llm_npc_memories(npc_name, location_x, location_y, location_z, map_name);
                    ";

                    using (var cmd = new SQLiteCommand(createIndexes, conn))
                    {
                        cmd.ExecuteNonQuery();
                    }

                    // Create relationships table
                    var createRelationshipsTable = @"
                        CREATE TABLE IF NOT EXISTS llm_relationships (
                            id INTEGER PRIMARY KEY AUTOINCREMENT,
                            npc_serial INTEGER NOT NULL,
                            npc_name TEXT NOT NULL,
                            player_name TEXT NOT NULL,
                            relationship_type TEXT NOT NULL,
                            relationship_score INTEGER NOT NULL DEFAULT 0,
                            summary TEXT,
                            first_met TEXT NOT NULL,
                            last_interaction TEXT NOT NULL,
                            interaction_count INTEGER NOT NULL DEFAULT 0,
                            UNIQUE(npc_serial, player_name)
                        )";

                    using (var cmd = new SQLiteCommand(createRelationshipsTable, conn))
                    {
                        cmd.ExecuteNonQuery();
                    }

                    // Create conversations table
                    var createConversationsTable = @"
                        CREATE TABLE IF NOT EXISTS llm_conversations (
                            id INTEGER PRIMARY KEY AUTOINCREMENT,
                            npc_name TEXT NOT NULL,
                            player_name TEXT NOT NULL,
                            session_id TEXT,
                            message TEXT NOT NULL,
                            is_player_message INTEGER NOT NULL DEFAULT 0,
                            timestamp TEXT NOT NULL
                        )";

                    using (var cmd = new SQLiteCommand(createConversationsTable, conn))
                    {
                        cmd.ExecuteNonQuery();
                    }

                    // Console.WriteLine("[SQLiteMemoryDatabase] Database schema initialized");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[SQLiteMemoryDatabase] Error initializing schema: {ex.Message}");
                throw;
            }
        }

        public static bool IsAvailable()
        {
            return m_Available && m_Initialized;
        }

        public static async Task<List<Memory>> GetMemoriesAsync(int npcSerial, string playerName, int limit = 10)
        {
            if (!IsAvailable())
                return new List<Memory>();

            try
            {
                using (var conn = new SQLiteConnection(m_ConnectionString))
                {
                    await conn.OpenAsync();

                    var sql = @"SELECT id, npc_serial, npc_name, player_name, memory_type, content, importance, 
                                     created_at, last_accessed, context, expires_at
                              FROM llm_npc_memories
                              WHERE npc_serial = @npcSerial AND player_name = @playerName
                              ORDER BY importance DESC, last_accessed DESC
                              LIMIT @limit";

                    using (var cmd = new SQLiteCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@npcSerial", npcSerial);
                        cmd.Parameters.AddWithValue("@playerName", playerName);
                        cmd.Parameters.AddWithValue("@limit", limit);

                        var memories = new List<Memory>();

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                var memory = new Memory
                                {
                                    Id = reader.GetInt32(0),
                                    NpcSerial = reader.GetInt32(1),
                                    NpcName = reader.GetString(2),
                                    PlayerName = reader.GetString(3),
                                    Type = Enum.TryParse<MemoryType>(reader.GetString(4), out var memType) ? memType : MemoryType.Conversation,
                                    Content = reader.GetString(5),
                                    Importance = reader.GetInt32(6),
                                    CreatedAt = DateTime.Parse(reader.GetString(7)),
                                    LastAccessed = DateTime.Parse(reader.GetString(8))
                                };

                                // Parse context if present
                                if (!reader.IsDBNull(9))
                                {
                                    string contextJson = reader.GetString(9);
                                    if (!string.IsNullOrEmpty(contextJson))
                                    {
                                        memory.Context = JsonConvert.DeserializeObject<Dictionary<string, object>>(contextJson);
                                    }
                                }

                                // Parse expiry if present
                                if (!reader.IsDBNull(10))
                                {
                                    memory.ExpiresAt = DateTime.Parse(reader.GetString(10));
                                }

                                memories.Add(memory);
                            }
                        }

                        return memories;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[SQLiteMemoryDatabase] Error loading memories: {ex.Message}");
                return new List<Memory>();
            }
        }

        /// <summary>
        /// Fallback method to get memories by NPC name and location when serial changes after server restart
        /// </summary>
        public static async Task<List<Memory>> GetMemoriesByNameAndLocationAsync(string npcName, string playerName, Point3D location, Map map, int limit = 10)
        {
            if (!IsAvailable())
                return new List<Memory>();

            try
            {
                using (var conn = new SQLiteConnection(m_ConnectionString))
                {
                    await conn.OpenAsync();

                    // Search for memories with same name and nearby location (within 50 tiles)
                    var sql = @"SELECT id, npc_serial, npc_name, player_name, memory_type, content, importance, 
                                     created_at, last_accessed, context, expires_at
                              FROM llm_npc_memories
                              WHERE npc_name = @npcName AND player_name = @playerName 
                                AND map_name = @mapName
                                AND ABS(location_x - @locationX) <= 50 
                                AND ABS(location_y - @locationY) <= 50
                              ORDER BY importance DESC, last_accessed DESC
                              LIMIT @limit";

                    using (var cmd = new SQLiteCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@npcName", npcName);
                        cmd.Parameters.AddWithValue("@playerName", playerName);
                        cmd.Parameters.AddWithValue("@locationX", location.X);
                        cmd.Parameters.AddWithValue("@locationY", location.Y);
                        cmd.Parameters.AddWithValue("@mapName", map?.Name ?? "");
                        cmd.Parameters.AddWithValue("@limit", limit);

                        var memories = new List<Memory>();

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                var memory = new Memory
                                {
                                    Id = reader.GetInt32(0),
                                    NpcSerial = reader.GetInt32(1),
                                    NpcName = reader.GetString(2),
                                    PlayerName = reader.GetString(3),
                                    Type = Enum.TryParse<MemoryType>(reader.GetString(4), out var memType) ? memType : MemoryType.Conversation,
                                    Content = reader.GetString(5),
                                    Importance = reader.GetInt32(6),
                                    CreatedAt = DateTime.Parse(reader.GetString(7)),
                                    LastAccessed = DateTime.Parse(reader.GetString(8))
                                };

                                // Parse context if present
                                if (!reader.IsDBNull(9))
                                {
                                    string contextJson = reader.GetString(9);
                                    if (!string.IsNullOrEmpty(contextJson))
                                    {
                                        memory.Context = JsonConvert.DeserializeObject<Dictionary<string, object>>(contextJson);
                                    }
                                }

                                // Parse expiry if present
                                if (!reader.IsDBNull(10))
                                {
                                    memory.ExpiresAt = DateTime.Parse(reader.GetString(10));
                                }

                                memories.Add(memory);
                            }
                        }

                        return memories;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[SQLiteMemoryDatabase] Error loading memories by name and location: {ex.Message}");
                return new List<Memory>();
            }
        }

        public static async Task SaveMemoryAsync(Memory memory, Point3D location, Map map)
        {
            if (!IsAvailable())
                return;

            try
            {
                using (var conn = new SQLiteConnection(m_ConnectionString))
                {
                    await conn.OpenAsync();

                    var sql = @"INSERT INTO llm_npc_memories
                                (npc_serial, npc_name, player_name, memory_type, content, importance, created_at, last_accessed, context, expires_at, location_x, location_y, location_z, map_name)
                                VALUES (@npcSerial, @npcName, @playerName, @memoryType, @content, @importance, @createdAt, @lastAccessed, @context, @expiresAt, @locationX, @locationY, @locationZ, @mapName)";

                    using (var cmd = new SQLiteCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@npcSerial", memory.NpcSerial);
                        cmd.Parameters.AddWithValue("@npcName", memory.NpcName);
                        cmd.Parameters.AddWithValue("@playerName", memory.PlayerName);
                        cmd.Parameters.AddWithValue("@memoryType", memory.Type.ToString());
                        cmd.Parameters.AddWithValue("@content", memory.Content);
                        cmd.Parameters.AddWithValue("@importance", memory.Importance);
                        cmd.Parameters.AddWithValue("@createdAt", memory.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss"));
                        cmd.Parameters.AddWithValue("@lastAccessed", memory.LastAccessed.ToString("yyyy-MM-dd HH:mm:ss"));
                        cmd.Parameters.AddWithValue("@context", memory.Context != null ? JsonConvert.SerializeObject(memory.Context) : (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@expiresAt", memory.ExpiresAt.HasValue ? memory.ExpiresAt.Value.ToString("yyyy-MM-dd HH:mm:ss") : (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@locationX", location.X);
                        cmd.Parameters.AddWithValue("@locationY", location.Y);
                        cmd.Parameters.AddWithValue("@locationZ", location.Z);
                        cmd.Parameters.AddWithValue("@mapName", map?.Name ?? "");

                        await cmd.ExecuteNonQueryAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[SQLiteMemoryDatabase] Error saving memory: {ex.Message}");
            }
        }

        public static async Task<Relationship> LoadRelationshipAsync(int npcSerial, string playerName)
        {
            if (!IsAvailable())
                return null;

            try
            {
                using (var conn = new SQLiteConnection(m_ConnectionString))
                {
                    await conn.OpenAsync();

                    var sql = @"SELECT id, npc_serial, npc_name, player_name, relationship_type, relationship_score, summary,
                                       first_met, last_interaction, interaction_count
                                FROM llm_relationships
                                WHERE npc_serial = @npcSerial AND player_name = @playerName";

                    using (var cmd = new SQLiteCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@npcSerial", npcSerial);
                        cmd.Parameters.AddWithValue("@playerName", playerName);

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                var relationship = new Relationship
                                {
                                    Id = reader.GetInt32(0),
                                    NpcSerial = reader.GetInt32(1),
                                    NpcName = reader.GetString(2),
                                    PlayerName = reader.GetString(3),
                                    Type = (RelationshipType)Enum.Parse(typeof(RelationshipType), reader.GetString(4)),
                                    Score = reader.GetInt32(5),
                                    Summary = reader.IsDBNull(6) ? null : reader.GetString(6),
                                    FirstMet = DateTime.Parse(reader.GetString(7)),
                                    LastInteraction = DateTime.Parse(reader.GetString(8)),
                                    InteractionCount = reader.GetInt32(9)
                                };

                                return relationship;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[SQLiteMemoryDatabase] Error loading relationship: {ex.Message}");
            }

            return null;
        }

        public static async Task SaveRelationshipAsync(Relationship relationship)
        {
            if (!IsAvailable())
                return;

            try
            {
                using (var conn = new SQLiteConnection(m_ConnectionString))
                {
                    await conn.OpenAsync();

                    var sql = @"INSERT OR REPLACE INTO llm_relationships
                                (npc_serial, npc_name, player_name, relationship_type, relationship_score, summary, first_met, last_interaction, interaction_count)
                                VALUES (@npcSerial, @npcName, @playerName, @type, @score, @summary, @firstMet, @lastInteraction, @interactionCount)";

                    using (var cmd = new SQLiteCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@npcSerial", relationship.NpcSerial);
                        cmd.Parameters.AddWithValue("@npcName", relationship.NpcName);
                        cmd.Parameters.AddWithValue("@playerName", relationship.PlayerName);
                        cmd.Parameters.AddWithValue("@type", relationship.Type.ToString());
                        cmd.Parameters.AddWithValue("@score", relationship.Score);
                        cmd.Parameters.AddWithValue("@summary", (object)relationship.Summary ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@firstMet", relationship.FirstMet.ToString("yyyy-MM-dd HH:mm:ss"));
                        cmd.Parameters.AddWithValue("@lastInteraction", relationship.LastInteraction.ToString("yyyy-MM-dd HH:mm:ss"));
                        cmd.Parameters.AddWithValue("@interactionCount", relationship.InteractionCount);

                        await cmd.ExecuteNonQueryAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[SQLiteMemoryDatabase] Error saving relationship: {ex.Message}");
            }
        }

        public static Task SaveConversationAsync(string npcName, string playerName, string sessionId, List<ConversationMessage> messages)
        {
            if (!IsAvailable())
                return Task.CompletedTask;

            // Fire and forget - don't block
            _ = Task.Run(async () =>
            {
                try
                {
                    using (var conn = new SQLiteConnection(m_ConnectionString))
                    {
                        await conn.OpenAsync();

                        using (var transaction = conn.BeginTransaction())
                        {
                            try
                            {
                                var sql = @"INSERT INTO llm_conversations
                                            (npc_name, player_name, session_id, message, is_player_message, timestamp)
                                            VALUES (@npcName, @playerName, @sessionId, @message, @isPlayer, @timestamp)";

                                foreach (var msg in messages)
                                {
                                    using (var cmd = new SQLiteCommand(sql, conn, transaction))
                                    {
                                        cmd.Parameters.AddWithValue("@npcName", npcName);
                                        cmd.Parameters.AddWithValue("@playerName", playerName);
                                        cmd.Parameters.AddWithValue("@sessionId", sessionId ?? "default");
                                        cmd.Parameters.AddWithValue("@message", msg.Message);
                                        cmd.Parameters.AddWithValue("@isPlayer", msg.IsPlayer ? 1 : 0);
                                        cmd.Parameters.AddWithValue("@timestamp", msg.Timestamp.ToString("yyyy-MM-dd HH:mm:ss"));

                                        await cmd.ExecuteNonQueryAsync();
                                    }
                                }

                                transaction.Commit();
                            }
                            catch
                            {
                                transaction.Rollback();
                                throw;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[SQLiteMemoryDatabase] Error saving conversation: {ex.Message}");
                }
            });

            return Task.CompletedTask;
        }

        public static async Task<List<PersistentConversationMessage>> LoadRecentConversationAsync(string npcName, string playerName, int limit = 10)
        {
            if (!IsAvailable())
                return new List<PersistentConversationMessage>();

            var messages = new List<PersistentConversationMessage>();

            try
            {
                using (var conn = new SQLiteConnection(m_ConnectionString))
                {
                    await conn.OpenAsync();

                    var sql = @"SELECT id, npc_name, player_name, session_id, message, is_player_message, timestamp
                                FROM llm_conversations
                                WHERE npc_name = @npcName AND player_name = @playerName
                                ORDER BY timestamp DESC
                                LIMIT @limit";

                    using (var cmd = new SQLiteCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@npcName", npcName);
                        cmd.Parameters.AddWithValue("@playerName", playerName);
                        cmd.Parameters.AddWithValue("@limit", limit);

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                var msg = new PersistentConversationMessage
                                {
                                    Id = reader.GetInt32(0),
                                    NpcName = reader.GetString(1),
                                    PlayerName = reader.GetString(2),
                                    SessionId = reader.IsDBNull(3) ? null : reader.GetString(3),
                                    Message = reader.GetString(4),
                                    IsPlayerMessage = reader.GetInt32(5) == 1,
                                    Timestamp = DateTime.Parse(reader.GetString(6))
                                };

                                messages.Add(msg);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[SQLiteMemoryDatabase] Error loading conversation: {ex.Message}");
            }

            return messages;
        }

        public static async Task FlushAll()
        {
            if (!IsAvailable())
                return;

            try
            {
                using (var conn = new SQLiteConnection(m_ConnectionString))
                {
                    await conn.OpenAsync();
                    
                    using (var transaction = conn.BeginTransaction())
                    {
                        try
                        {
                            // Delete all memories
                            using (var cmd = new SQLiteCommand("DELETE FROM llm_npc_memories", conn, transaction))
                            {
                                int memoriesDeleted = await cmd.ExecuteNonQueryAsync();
                                Console.WriteLine($"[SQLiteMemoryDatabase] Deleted {memoriesDeleted} memories");
                            }
                            
                            // Delete all relationships
                            using (var cmd = new SQLiteCommand("DELETE FROM llm_relationships", conn, transaction))
                            {
                                int relationshipsDeleted = await cmd.ExecuteNonQueryAsync();
                                Console.WriteLine($"[SQLiteMemoryDatabase] Deleted {relationshipsDeleted} relationships");
                            }
                            
                            // Delete all conversations
                            using (var cmd = new SQLiteCommand("DELETE FROM llm_conversations", conn, transaction))
                            {
                                int conversationsDeleted = await cmd.ExecuteNonQueryAsync();
                                Console.WriteLine($"[SQLiteMemoryDatabase] Deleted {conversationsDeleted} conversation messages");
                            }
                            
                            transaction.Commit();
                            Console.WriteLine("[SQLiteMemoryDatabase] All data flushed successfully");
                        }
                        catch
                        {
                            transaction.Rollback();
                            throw;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[SQLiteMemoryDatabase] Error flushing data: {ex.Message}");
            }
        }
    }
}

