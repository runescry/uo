using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Server.Services.LLM
{
    /// <summary>
    /// Simple in-memory fallback for memory/relationship storage when SQLite unavailable
    /// Data is lost on server restart but provides basic functionality
    /// </summary>
    public static class InMemoryFallbackStore
    {
        private static Dictionary<string, List<Memory>> m_Memories = new Dictionary<string, List<Memory>>();
        private static Dictionary<string, Relationship> m_Relationships = new Dictionary<string, Relationship>();
        private static Dictionary<string, List<PersistentConversationMessage>> m_Conversations = new Dictionary<string, List<PersistentConversationMessage>>();
        private static readonly object m_Lock = new object();

        private static bool m_IsActive = false;

        public static void Activate()
        {
            m_IsActive = true;
            Console.WriteLine("[InMemoryFallbackStore] In-memory fallback store activated");
            Console.WriteLine("[InMemoryFallbackStore] WARNING: Memories will be lost on server restart!");
        }

        public static bool IsActive => m_IsActive;

        #region Memories

        public static Task<List<Memory>> GetMemoriesAsync(int npcSerial, string playerName, int limit = 10)
        {
            lock (m_Lock)
            {
                string key = GetMemoryKey(npcSerial, playerName);

                if (m_Memories.TryGetValue(key, out var memories))
                {
                    // Update last accessed time
                    foreach (var mem in memories)
                        mem.LastAccessed = DateTime.UtcNow;

                    return Task.FromResult(memories
                        .OrderByDescending(m => m.Importance)
                        .ThenByDescending(m => m.LastAccessed)
                        .Take(limit)
                        .ToList());
                }

                return Task.FromResult(new List<Memory>());
            }
        }
        
        public static Task<List<Memory>> GetMemoriesAsync(string npcName, string playerName, int limit = 10) // Legacy support
        {
            return GetMemoriesAsync(0, playerName, limit); // Will return empty for legacy calls
        }

        public static Task SaveMemoryAsync(Memory memory)
        {
            lock (m_Lock)
            {
                string key = GetMemoryKey(memory.NpcSerial, memory.PlayerName);

                if (!m_Memories.ContainsKey(key))
                    m_Memories[key] = new List<Memory>();

                // Check for duplicates (same content)
                var existing = m_Memories[key].FirstOrDefault(m => m.Content == memory.Content);
                if (existing != null)
                {
                    // Update existing memory
                    existing.LastAccessed = DateTime.UtcNow;
                    existing.Importance = Math.Max(existing.Importance, memory.Importance);
                }
                else
                {
                    m_Memories[key].Add(memory);
                }

                // Prune old memories (keep max 50 per NPC/player pair)
                if (m_Memories[key].Count > 50)
                {
                    m_Memories[key] = m_Memories[key]
                        .OrderByDescending(m => m.Importance)
                        .ThenByDescending(m => m.LastAccessed)
                        .Take(50)
                        .ToList();
                }
            }

            // SQLite writes are handled directly by LLMMemoryService
            return Task.CompletedTask;
        }

        private static string GetMemoryKey(int npcSerial, string playerName)
        {
            return $"{npcSerial}:{playerName}";
        }
        
        private static string GetMemoryKey(string npcName, string playerName) // Legacy support
        {
            return $"{npcName}:{playerName}";
        }

        #endregion

        #region Relationships

        public static Task<Relationship> GetRelationshipAsync(int npcSerial, string playerName)
        {
            lock (m_Lock)
            {
                string key = GetRelationshipKey(npcSerial, playerName);

                if (m_Relationships.TryGetValue(key, out var relationship))
                {
                    return Task.FromResult(relationship);
                }

                return Task.FromResult<Relationship>(null);
            }
        }
        
        public static Task<Relationship> GetRelationshipAsync(string npcName, string playerName) // Legacy support
        {
            return Task.FromResult<Relationship>(null);
        }

        public static Task SaveRelationshipAsync(Relationship relationship)
        {
            lock (m_Lock)
            {
                string key = GetRelationshipKey(relationship.NpcSerial, relationship.PlayerName);
                m_Relationships[key] = relationship;
            }

            // SQLite writes are handled directly by LLMMemoryService
            return Task.CompletedTask;
        }

        private static string GetRelationshipKey(int npcSerial, string playerName)
        {
            return $"{npcSerial}:{playerName}";
        }
        
        private static string GetRelationshipKey(string npcName, string playerName) // Legacy support
        {
            return $"{npcName}:{playerName}";
        }

        #endregion

        #region Conversations

        public static Task SaveConversationAsync(string npcName, string playerName, string sessionId, List<ConversationMessage> messages)
        {
            lock (m_Lock)
            {
                string key = GetConversationKey(npcName, playerName);

                if (!m_Conversations.ContainsKey(key))
                    m_Conversations[key] = new List<PersistentConversationMessage>();

                // Convert and add messages
                foreach (var msg in messages)
                {
                    var persistentMsg = PersistentConversationMessage.FromConversationMessage(msg, npcName, playerName, sessionId);
                    m_Conversations[key].Add(persistentMsg);
                }

                // Keep last 100 messages per NPC/player pair
                if (m_Conversations[key].Count > 100)
                {
                    m_Conversations[key] = m_Conversations[key]
                        .OrderByDescending(m => m.Timestamp)
                        .Take(100)
                        .ToList();
                }
            }

            return Task.CompletedTask;
        }

        public static Task<List<PersistentConversationMessage>> LoadRecentConversationAsync(string npcName, string playerName, int limit = 10)
        {
            lock (m_Lock)
            {
                string key = GetConversationKey(npcName, playerName);

                if (m_Conversations.TryGetValue(key, out var messages))
                {
                    return Task.FromResult(messages
                        .OrderByDescending(m => m.Timestamp)
                        .Take(limit)
                        .OrderBy(m => m.Timestamp)
                        .ToList());
                }

                return Task.FromResult(new List<PersistentConversationMessage>());
            }
        }

        private static string GetConversationKey(string npcName, string playerName)
        {
            return $"{npcName}:{playerName}";
        }

        #endregion

        #region Statistics

        public static string GetStatistics()
        {
            lock (m_Lock)
            {
                int totalMemories = m_Memories.Values.Sum(list => list.Count);
                int totalRelationships = m_Relationships.Count;
                int totalConversations = m_Conversations.Values.Sum(list => list.Count);

                return $"In-Memory Store Stats:\n" +
                       $"  Memories: {totalMemories} ({m_Memories.Count} NPC/Player pairs)\n" +
                       $"  Relationships: {totalRelationships}\n" +
                       $"  Conversation messages: {totalConversations}";
            }
        }

        #endregion
        
        /// <summary>
        /// Clear all data from in-memory fallback store
        /// </summary>
        public static void ClearAll()
        {
            lock (m_Lock)
            {
                m_Memories.Clear();
                m_Relationships.Clear();
                m_Conversations.Clear();
                Console.WriteLine("[InMemoryFallbackStore] All data cleared");
            }
        }
    }
}
