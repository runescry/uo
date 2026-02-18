using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Server;
using Server.Mobiles;
using Newtonsoft.Json;

namespace Server.Services.LLM
{
    /// <summary>
    /// High-performance caching system for LLM quest generation results
    /// Implements TTL-based caching with memory management
    /// </summary>
    public static class QuestCache
    {
        private static readonly ConcurrentDictionary<string, CacheEntry> s_Cache = new ConcurrentDictionary<string, CacheEntry>();
        private static readonly TimeSpan s_DefaultTTL = TimeSpan.FromMinutes(30);
        private static readonly TimeSpan s_CleanupInterval = TimeSpan.FromMinutes(5);
        private static readonly int s_MaxCacheSize = 1000;
        private static readonly object s_Lock = new object();

        // Performance metrics
        private static long s_Hits = 0;
        private static long s_Misses = 0;
        private static long s_Evictions = 0;

        static QuestCache()
        {
            // Start cleanup task to remove expired entries
            Task.Run(async () =>
            {
                while (true)
                {
                    await Task.Delay(s_CleanupInterval);
                    CleanupExpiredEntries(null);
                }
            });
        }

        /// <summary>
        /// Get cached quest generation result
        /// </summary>
        public static async Task<string> GetAsync(string cacheKey)
        {
            if (s_Cache.TryGetValue(cacheKey, out CacheEntry entry))
            {
                if (entry.ExpiresAt > DateTime.UtcNow)
                {
                    Interlocked.Increment(ref s_Hits);
                    return entry.Value;
                }
                else
                {
                    // Entry expired, remove it
                    s_Cache.TryRemove(cacheKey, out _);
                    Interlocked.Increment(ref s_Misses);
                }
            }

            Interlocked.Increment(ref s_Misses);
            return null;
        }

        /// <summary>
        /// Set cached quest generation result
        /// </summary>
        public static async Task SetAsync(string cacheKey, string value, TimeSpan? ttl = null)
        {
            var expiresAt = DateTime.UtcNow.Add(ttl ?? s_DefaultTTL);
            
            // Check cache size limit
            if (s_Cache.Count >= s_MaxCacheSize)
            {
                await EvictOldestEntriesAsync(s_MaxCacheSize / 10); // Evict 10% of entries
            }

            var entry = new CacheEntry
            {
                Value = value,
                ExpiresAt = expiresAt,
                CreatedAt = DateTime.UtcNow,
                LastAccessed = DateTime.UtcNow
            };

            s_Cache[cacheKey] = entry;
        }

        /// <summary>
        /// Get or create cached quest generation result
        /// </summary>
        public static async Task<string> GetOrCreateAsync(string cacheKey, Func<Task<string>> factory, TimeSpan? ttl = null)
        {
            var cached = await GetAsync(cacheKey);
            if (cached != null)
            {
                return cached;
            }

            var result = await factory();
            await SetAsync(cacheKey, result, ttl);
            return result;
        }

        /// <summary>
        /// Invalidate cache entry
        /// </summary>
        public static void Invalidate(string cacheKey)
        {
            s_Cache.TryRemove(cacheKey, out _);
        }

        /// <summary>
        /// Clear all cache entries
        /// </summary>
        public static void Clear()
        {
            lock (s_Lock)
            {
                s_Cache.Clear();
            }
        }

        /// <summary>
        /// Get cache statistics
        /// </summary>
        public static CacheStats GetStats()
        {
            lock (s_Lock)
            {
                return new CacheStats
                {
                    TotalEntries = s_Cache.Count,
                    Hits = s_Hits,
                    Misses = s_Misses,
                    HitRatio = s_Hits + s_Misses > 0 ? (double)s_Hits / (s_Hits + s_Misses) : 0.0,
                    Evictions = s_Evictions
                };
            }
        }

        /// <summary>
        /// Generate cache key for quest generation
        /// </summary>
        public static string GenerateCacheKey(string npcName, string npcPersonality, string playerMessage, string playerName, string preloadedKnowledge)
        {
            // Create a deterministic cache key based on input parameters
            var keyData = $"{npcName}|{npcPersonality}|{playerMessage}|{playerName}|{preloadedKnowledge}";
            return keyData.GetHashCode().ToString("X");
        }

        /// <summary>
        /// Generate cache key for quest generation with conversation history
        /// </summary>
        public static string GenerateCacheKey(string npcName, string npcPersonality, List<ConversationMessage> conversationHistory, string playerMessage, string playerName, string preloadedKnowledge)
        {
            var historyHash = conversationHistory != null ? string.Join("|", conversationHistory.Select(m => $"{(m.IsPlayer ? "user" : "assistant")}:{m.Message}")) : "";
            var keyData = $"{npcName}|{npcPersonality}|{historyHash}|{playerMessage}|{playerName}|{preloadedKnowledge}";
            return keyData.GetHashCode().ToString("X");
        }

        /// <summary>
        /// Cleanup expired entries
        /// </summary>
        private static void CleanupExpiredEntries(object state)
        {
            var now = DateTime.UtcNow;
            var expiredKeys = new List<string>();

            foreach (var kvp in s_Cache)
            {
                if (kvp.Value.ExpiresAt <= now)
                {
                    expiredKeys.Add(kvp.Key);
                }
            }

            foreach (var key in expiredKeys)
            {
                if (s_Cache.TryRemove(key, out _))
                {
                    Interlocked.Increment(ref s_Evictions);
                }
            }
        }

        /// <summary>
        /// Evict oldest entries to maintain cache size limit
        /// </summary>
        private static async Task EvictOldestEntriesAsync(int count)
        {
            var entries = s_Cache.OrderBy(kvp => kvp.Value.LastAccessed).Take(count).ToList();
            
            foreach (var kvp in entries)
            {
                s_Cache.TryRemove(kvp.Key, out _);
                Interlocked.Increment(ref s_Evictions);
            }
        }

        /// <summary>
        /// Cache entry with TTL
        /// </summary>
        private class CacheEntry
        {
            public string Value { get; set; }
            public DateTime ExpiresAt { get; set; }
            public DateTime CreatedAt { get; set; }
            public DateTime LastAccessed { get; set; }
        }

        /// <summary>
        /// Cache statistics
        /// </summary>
        public class CacheStats
        {
            public int TotalEntries { get; set; }
            public long Hits { get; set; }
            public long Misses { get; set; }
            public double HitRatio { get; set; }
            public long Evictions { get; set; }
        }
    }
}
