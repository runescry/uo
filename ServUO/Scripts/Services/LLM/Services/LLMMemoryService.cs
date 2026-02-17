using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Server.Services.LLM;

namespace Server.Services.LLM
{
    /// <summary>
    /// Main service for managing NPC memories and relationships
    /// Uses SQLite database for persistent storage
    /// </summary>
    public static class LLMMemoryService
    {
        private static bool m_Initialized = false;

        public static void Initialize()
        {
            if (m_Initialized)
                return;

            m_Initialized = true;

            if (!LLMMemoryConfig.Enabled)
            {
                Console.WriteLine("[LLMMemoryService] Memory system disabled in configuration");
                return;
            }

            // Initialize database layer (SQLite - no external dependencies)
            // SQLite initialization logs its own status
            SQLiteMemoryDatabase.Initialize();

            // Log final status after initialization
            if (SQLiteMemoryDatabase.IsAvailable())
            {
                Console.WriteLine("[LLMMemoryService] Memory system initialized");
            }
            else
            {
                Console.WriteLine("[LLMMemoryService] WARNING: Memory system initialized (using in-memory fallback)");
            }
        }

        public static bool IsAvailable()
        {
            // Memory system is available if enabled and SQLite is available, or fallback
            if (!LLMMemoryConfig.Enabled)
                return false;
            
            // If SQLite available, use it
            if (SQLiteMemoryDatabase.IsAvailable())
                return true;
            
            // Otherwise, activate and use in-memory fallback
            if (!InMemoryFallbackStore.IsActive)
            {
                InMemoryFallbackStore.Activate();
            }
            return true;
        }

        public static bool IsUsingFallback()
        {
            return LLMMemoryConfig.Enabled && !SQLiteMemoryDatabase.IsAvailable();
        }

        /// <summary>
        /// Gets memories for an NPC about a player (cache-first, then database, then fallback)
        /// </summary>
        public static async Task<List<Memory>> GetMemoriesAsync(int npcSerial, string playerName, int limit = 10)
        {
            if (!LLMMemoryConfig.Enabled)
                return new List<Memory>();

            try
            {
                // Try SQLite database first (if available)
                if (SQLiteMemoryDatabase.IsAvailable())
                {
                    var memories = await SQLiteMemoryDatabase.GetMemoriesAsync(npcSerial, playerName, limit);
                    if (memories.Count > 0)
                        return memories;
                }

                // Fallback to in-memory store
                if (IsUsingFallback())
                {
                    if (!InMemoryFallbackStore.IsActive)
                    {
                        InMemoryFallbackStore.Activate();
                    }
                    return await InMemoryFallbackStore.GetMemoriesAsync(npcSerial, playerName, limit);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[LLMMemoryService] Error getting memories: {ex.Message}");
            }

            return new List<Memory>();
        }

        /// <summary>
        /// Fallback method to get memories by NPC name and location when serial changes after server restart
        /// </summary>
        public static async Task<List<Memory>> GetMemoriesByNameAndLocationAsync(string npcName, string playerName, Point3D location, Map map, int limit = 10)
        {
            if (!LLMMemoryConfig.Enabled)
                return new List<Memory>();

            try
            {
                Console.WriteLine($"[LLMMemoryService] Fallback: Searching memories for NPC '{npcName}' at {location} on {map.Name} for player '{playerName}'");
                
                // Try SQLite database first (if available)
                if (SQLiteMemoryDatabase.IsAvailable())
                {
                    var memories = await SQLiteMemoryDatabase.GetMemoriesByNameAndLocationAsync(npcName, playerName, location, map, limit);
                    if (memories.Count > 0)
                    {
                        Console.WriteLine($"[LLMMemoryService] Fallback: Found {memories.Count} memories for NPC '{npcName}' by name+location lookup");
                        return memories;
                    }
                }

                Console.WriteLine($"[LLMMemoryService] Fallback: No memories found for NPC '{npcName}' by name+location lookup");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[LLMMemoryService] Error getting memories by name and location: {ex.Message}");
            }

            return new List<Memory>();
        }

        /// <summary>
        /// Saves a memory (writes to cache and database, or fallback store)
        /// </summary>
        public static async Task SaveMemoryAsync(int npcSerial, string npcName, string playerName, Memory memory, Point3D location, Map map)
        {
            if (!LLMMemoryConfig.Enabled)
                return;

            try
            {
                memory.NpcSerial = npcSerial;
                memory.NpcName = npcName;
                memory.PlayerName = playerName;

                // Write to SQLite database (async, non-blocking)
                if (SQLiteMemoryDatabase.IsAvailable())
                {
                    _ = Task.Run(async () =>
                    {
                        await SQLiteMemoryDatabase.SaveMemoryAsync(memory, location, map);
                    });
                }
                
                // Also save to fallback store if using it
                if (IsUsingFallback())
                {
                    if (!InMemoryFallbackStore.IsActive)
                    {
                        InMemoryFallbackStore.Activate();
                    }
                    await InMemoryFallbackStore.SaveMemoryAsync(memory);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[LLMMemoryService] Error saving memory: {ex.Message}");
            }
        }

        /// <summary>
        /// Gets the relationship between an NPC and player
        /// </summary>
        public static async Task<Relationship> GetRelationshipAsync(int npcSerial, string playerName)
        {
            if (!LLMMemoryConfig.Enabled)
                return null;

            try
            {
                // Load from SQLite database
                if (SQLiteMemoryDatabase.IsAvailable())
                {
                    var relationship = await SQLiteMemoryDatabase.LoadRelationshipAsync(npcSerial, playerName);
                    return relationship;
                }
                
                // Fallback to in-memory store if SQLite not available
                if (IsUsingFallback())
                {
                    if (!InMemoryFallbackStore.IsActive)
                    {
                        InMemoryFallbackStore.Activate();
                    }
                    return await InMemoryFallbackStore.GetRelationshipAsync(npcSerial, playerName);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[LLMMemoryService] Error loading relationship: {ex.Message}");
            }

            return null;
        }

        /// <summary>
        /// Updates relationship score (increments/decrements)
        /// </summary>
        public static async Task UpdateRelationshipAsync(int npcSerial, string npcName, string playerName, int delta)
        {
            if (!LLMMemoryConfig.Enabled)
                return;

            try
            {
                var relationship = await GetRelationshipAsync(npcSerial, playerName);
                
                if (relationship == null)
                {
                    relationship = new Relationship
                    {
                        NpcSerial = npcSerial,
                        NpcName = npcName,
                        PlayerName = playerName,
                        Score = delta
                    };
                }
                else
                {
                    relationship.Score = Math.Max(-100, Math.Min(100, relationship.Score + delta));
                }

                relationship.LastInteraction = DateTime.UtcNow;
                relationship.InteractionCount++;

                // Update relationship type based on score
                relationship.Type = GetRelationshipTypeFromScore(relationship.Score);

                // Save to SQLite database
                if (SQLiteMemoryDatabase.IsAvailable())
                {
                    _ = Task.Run(async () =>
                    {
                        await SQLiteMemoryDatabase.SaveRelationshipAsync(relationship);
                    });
                }
                
                // Also save to fallback store if using it
                if (IsUsingFallback())
                {
                    if (!InMemoryFallbackStore.IsActive)
                    {
                        InMemoryFallbackStore.Activate();
                    }
                    await InMemoryFallbackStore.SaveRelationshipAsync(relationship);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[LLMMemoryService] Error updating relationship: {ex.Message}");
            }
        }

        /// <summary>
        /// Saves conversation history
        /// </summary>
        public static Task SaveConversationAsync(string npcName, string playerName, string sessionId, List<ConversationMessage> messages)
        {
            if (!IsAvailable())
                return Task.CompletedTask;

            try
            {
                // Save to SQLite database (fire and forget)
                if (SQLiteMemoryDatabase.IsAvailable())
                {
                    _ = SQLiteMemoryDatabase.SaveConversationAsync(npcName, playerName, sessionId, messages);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[LLMMemoryService] Error saving conversation: {ex.Message}");
            }
            
            return Task.CompletedTask;
        }

        /// <summary>
        /// Loads recent conversation history
        /// </summary>
        public static async Task<List<PersistentConversationMessage>> LoadRecentConversationAsync(string npcName, string playerName, int limit = 10)
        {
            if (!IsAvailable())
                return new List<PersistentConversationMessage>();

            try
            {
                if (SQLiteMemoryDatabase.IsAvailable())
                {
                    return await SQLiteMemoryDatabase.LoadRecentConversationAsync(npcName, playerName, limit);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[LLMMemoryService] Error loading conversation: {ex.Message}");
            }

            return new List<PersistentConversationMessage>();
        }

        /// <summary>
        /// Determines relationship type from score
        /// </summary>
        private static RelationshipType GetRelationshipTypeFromScore(int score)
        {
            if (score >= 81)
                return RelationshipType.Ally;
            else if (score >= 61)
                return RelationshipType.CloseFriend;
            else if (score >= 41)
                return RelationshipType.Friend;
            else if (score >= 21)
                return RelationshipType.Acquaintance;
            else if (score <= -10)
                return RelationshipType.Enemy;
            else
                return RelationshipType.Stranger;
        }
    }
}

