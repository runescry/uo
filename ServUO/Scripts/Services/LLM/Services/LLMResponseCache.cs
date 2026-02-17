using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace Server.Services.LLM
{
    /// <summary>
    /// Caches LLM responses for common queries to improve performance
    /// </summary>
    public static class LLMResponseCache
    {
        private static Dictionary<string, CachedResponse> m_Cache = new Dictionary<string, CachedResponse>();
        private static readonly object m_CacheLock = new object();
        
        // Configuration
        private static readonly int MaxCacheSize = 100;
        private static readonly TimeSpan CacheTTL = TimeSpan.FromHours(24); // Cache for 24 hours
        private static readonly int MinQueryLength = 3; // Don't cache very short queries

        /// <summary>
        /// Cached response entry
        /// </summary>
        private class CachedResponse
        {
            public string Response { get; set; }
            public DateTime CachedAt { get; set; }
            public DateTime LastAccessed { get; set; }
            public int AccessCount { get; set; }
            public string QueryHash { get; set; }
            public string OriginalQuery { get; set; } // Store original query for semantic matching
            public float[] QueryEmbedding { get; set; } // Semantic embedding (computed lazily)
            public string NpcName { get; set; } // Store context for semantic matching
            public string PlayerName { get; set; }
            public bool IsVendorQuery { get; set; }

            public CachedResponse(string response, string queryHash, string originalQuery, string npcName, string playerName, bool isVendorQuery)
            {
                Response = response;
                CachedAt = DateTime.UtcNow;
                LastAccessed = DateTime.UtcNow;
                AccessCount = 1;
                QueryHash = queryHash;
                OriginalQuery = originalQuery;
                QueryEmbedding = null; // Will be computed on-demand
                NpcName = npcName;
                PlayerName = playerName;
                IsVendorQuery = isVendorQuery;
            }

            public bool IsExpired()
            {
                return DateTime.UtcNow - CachedAt > CacheTTL;
            }

            /// <summary>
            /// Checks if this cached entry matches the given context
            /// </summary>
            public bool MatchesContext(string npcName, string playerName, bool isVendorQuery)
            {
                // For semantic matching, we want to match same NPC and vendor status
                // Player name can be different (same question from different players should match)
                return (this.NpcName?.ToLower() == npcName?.ToLower()) &&
                       (this.IsVendorQuery == isVendorQuery);
            }
        }

        // Semantic matching configuration
        private static readonly float SemanticSimilarityThreshold = 0.85f; // 85% similarity required

        /// <summary>
        /// Normalizes a query for caching (removes case, extra spaces, etc.)
        /// </summary>
        private static string NormalizeQuery(string query)
        {
            if (string.IsNullOrEmpty(query))
                return "";

            // Convert to lowercase and trim
            string normalized = query.ToLower().Trim();

            // Remove extra whitespace
            normalized = System.Text.RegularExpressions.Regex.Replace(normalized, @"\s+", " ");

            // Remove punctuation variations that don't affect meaning
            // Keep essential punctuation but normalize
            normalized = normalized.Replace("?", "").Replace("!", "").Replace(".", "");

            return normalized;
        }

        /// <summary>
        /// Generates a hash for a query (for exact matching)
        /// </summary>
        private static string GenerateQueryHash(string normalizedQuery, string npcName, string playerName, bool isVendorQuery)
        {
            // Include NPC name and vendor status for context-specific caching
            // Same query to different NPCs or vendor vs non-vendor should be different
            string cacheKey = $"{normalizedQuery}|{npcName?.ToLower() ?? ""}|{playerName?.ToLower() ?? ""}|{isVendorQuery}";
            
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(cacheKey));
                return Convert.ToBase64String(hashBytes).Substring(0, 16); // Use first 16 chars
            }
        }

        /// <summary>
        /// Checks if a query should be cached (common patterns)
        /// </summary>
        private static bool ShouldCache(string normalizedQuery, bool isVendorQuery)
        {
            // Don't cache very short queries
            if (normalizedQuery.Length < MinQueryLength)
                return false;

            // Always cache vendor queries (they're repetitive and not location-dependent)
            if (isVendorQuery)
                return true;

            // DON'T cache location-dependent queries (they change based on where NPC/player is)
            string[] locationDependentQueries = {
                "where am i", "where are we", "where is this", "where are you",
                "what is this place", "what place is this", "what city", "what town",
                "what location", "where do we stand", "where are we standing",
                "what region", "what area", "where have we", "where did we"
            };

            foreach (string locationQuery in locationDependentQueries)
            {
                if (normalizedQuery.Contains(locationQuery))
                {
                    Console.WriteLine($"[LLMResponseCache] Skipping cache for location-dependent query: '{normalizedQuery}'");
                    return false;
                }
            }

            // Cache common greetings (not location-dependent)
            string[] commonGreetings = {
                "hello", "hi", "greetings", "hey", "good morning", "good afternoon",
                "good evening", "howdy", "salutations"
            };

            foreach (string greeting in commonGreetings)
            {
                if (normalizedQuery.Contains(greeting) && normalizedQuery.Split(' ').Length <= 5)
                    return true;
            }

            // Cache simple identity questions (not location-dependent)
            string[] identityQuestions = {
                "who are you", "what are you", "what is your name", "tell me about yourself",
                "what do you do", "what is your profession", "what is your trade", "what is your job"
            };

            foreach (string question in identityQuestions)
            {
                if (normalizedQuery.Contains(question))
                    return true;
            }

            // DON'T cache time-dependent queries (they change throughout the day)
            string[] timeDependentQueries = {
                "what time is it", "what time", "is it morning", "is it afternoon",
                "is it evening", "is it night", "what time of day", "when is"
            };

            foreach (string timeQuery in timeDependentQueries)
            {
                if (normalizedQuery.Contains(timeQuery))
                {
                    Console.WriteLine($"[LLMResponseCache] Skipping cache for time-dependent query: '{normalizedQuery}'");
                    return false;
                }
            }

            // DON'T cache player-specific/memory queries (depends on conversation history)
            string[] playerSpecificQueries = {
                "do you remember", "remember me", "what did we", "what did you tell me",
                "what did i say", "what did i ask", "have we met", "do you know me",
                "have we talked", "what was our", "earlier you", "you said"
            };

            foreach (string playerQuery in playerSpecificQueries)
            {
                if (normalizedQuery.Contains(playerQuery))
                {
                    Console.WriteLine($"[LLMResponseCache] Skipping cache for player-specific query: '{normalizedQuery}'");
                    return false;
                }
            }

            // DON'T cache quest-related queries (depends on player's quest state)
            string[] questQueries = {
                "do you have a quest", "what quest", "any quest", "give me a quest",
                "have a task", "need help", "can you help", "do you need"
            };

            foreach (string questQuery in questQueries)
            {
                if (normalizedQuery.Contains(questQuery))
                {
                    Console.WriteLine($"[LLMResponseCache] Skipping cache for quest-related query: '{normalizedQuery}'");
                    return false;
                }
            }

            // DON'T cache dynamic/event queries (depends on recent world state)
            string[] dynamicQueries = {
                "what happened", "did you see", "what did you see", "what was that",
                "did that happen", "what is going on", "what is happening", "what occurred",
                "recently", "just now", "a moment ago", "earlier today"
            };

            foreach (string dynamicQuery in dynamicQueries)
            {
                if (normalizedQuery.Contains(dynamicQuery))
                {
                    Console.WriteLine($"[LLMResponseCache] Skipping cache for dynamic/event query: '{normalizedQuery}'");
                    return false;
                }
            }

            // Cache static lore questions (factual knowledge that doesn't change)
            // These are about game world facts, not current state
            string[] loreQuestionPatterns = {
                "tell me about", "what do you know about", "what is", "who is",
                "what are", "explain", "describe", "what can you tell me about"
            };

            bool isLoreQuestion = false;
            foreach (string pattern in loreQuestionPatterns)
            {
                if (normalizedQuery.Contains(pattern))
                {
                    isLoreQuestion = true;
                    break;
                }
            }

            // If it's a lore question AND doesn't contain location/time/player-specific keywords, cache it
            if (isLoreQuestion)
            {
                // Double-check it's not asking about current location/time
                bool hasContextualKeywords = normalizedQuery.Contains("here") || 
                                           normalizedQuery.Contains("this place") ||
                                           normalizedQuery.Contains("now") ||
                                           normalizedQuery.Contains("current");
                
                if (!hasContextualKeywords)
                {
                    return true; // Cache static lore questions
                }
            }

            // Cache game mechanics questions (how things work, not current state)
            string[] mechanicsQuestions = {
                "how do i", "how does", "how can i", "how to", "how is",
                "what are reagents", "what are spell circles", "how does magic work",
                "how do i craft", "how do i use", "what is magic", "what are moongates"
            };

            foreach (string mechanicsQuery in mechanicsQuestions)
            {
                if (normalizedQuery.Contains(mechanicsQuery))
                    return true;
            }

            // Don't cache very long or complex queries
            if (normalizedQuery.Split(' ').Length > 15)
                return false;

            return false; // Default: don't cache unless it matches patterns above
        }

        /// <summary>
        /// Gets a cached response if available (exact match first, then semantic match) - ASYNC VERSION
        /// </summary>
        public static async Task<string> GetCachedResponseAsync(string query, string npcName, string playerName, bool isVendorQuery, bool isFirstConversation = false)
        {
            if (string.IsNullOrEmpty(query))
                return null;

            string normalizedQuery = NormalizeQuery(query);
            string queryHash = GenerateQueryHash(normalizedQuery, npcName, playerName, isVendorQuery);

            lock (m_CacheLock)
            {
                // Try exact match first
                if (m_Cache.TryGetValue(queryHash, out CachedResponse cached))
                {
                    // Check if expired
                    if (cached.IsExpired())
                    {
                        m_Cache.Remove(queryHash);
                    }
                    else
                    {
                        // Update access stats
                        cached.LastAccessed = DateTime.UtcNow;
                        cached.AccessCount++;

                        Console.WriteLine($"[LLMResponseCache] Cache HIT (exact match) for query hash {queryHash} (accessed {cached.AccessCount} times)");
                        return cached.Response;
                    }
                }
            }

            // SKIP semantic matching on first conversation to avoid embedding generation delay
            if (isFirstConversation)
            {
                Console.WriteLine($"[LLMResponseCache] Skipping semantic search on first conversation (no embeddings)");
                return null;
            }

            // Try semantic matching if exact match failed and embeddings are available (async, non-blocking)
            if (EmbeddingService.IsAvailable())
            {
                DateTime semanticStart = DateTime.UtcNow;
                string semanticMatch = await GetCachedResponseBySemanticsAsync(query, npcName, playerName, isVendorQuery);
                if (semanticMatch != null)
                {
                    return semanticMatch;
                }
            }

            return null;
        }

        /// <summary>
        /// Gets a cached response if available (exact match only - synchronous, non-blocking)
        /// </summary>
        public static string GetCachedResponse(string query, string npcName, string playerName, bool isVendorQuery)
        {
            if (string.IsNullOrEmpty(query))
                return null;

            string normalizedQuery = NormalizeQuery(query);
            string queryHash = GenerateQueryHash(normalizedQuery, npcName, playerName, isVendorQuery);

            lock (m_CacheLock)
            {
                // Try exact match only (skip semantic search to avoid blocking)
                if (m_Cache.TryGetValue(queryHash, out CachedResponse cached))
                {
                    // Check if expired
                    if (cached.IsExpired())
                    {
                        m_Cache.Remove(queryHash);
                    }
                    else
                    {
                        // Update access stats
                        cached.LastAccessed = DateTime.UtcNow;
                        cached.AccessCount++;

                        Console.WriteLine($"[LLMResponseCache] Cache HIT (exact match) for query hash {queryHash} (accessed {cached.AccessCount} times)");
                        return cached.Response;
                    }
                }
            }

            return null; // Skip semantic search in sync version to avoid blocking
        }

        /// <summary>
        /// Gets cached response using semantic similarity matching (async version)
        /// </summary>
        private static async Task<string> GetCachedResponseBySemanticsAsync(string query, string npcName, string playerName, bool isVendorQuery)
        {
            try
            {
                // Generate embedding for the query (async, non-blocking)
                float[] queryEmbedding = await EmbeddingService.GenerateEmbeddingAsync(query);
                
                if (queryEmbedding == null)
                {
                    Console.WriteLine("[LLMResponseCache] Failed to generate embedding for semantic search");
                    return null;
                }
                float bestSimilarity = 0f;
                CachedResponse bestMatch = null;

                // Search through cached entries for semantic matches
                // Limit to most recently accessed entries for performance (check top 20)
                List<CachedResponse> candidates;
                lock (m_CacheLock)
                {
                    // Get most recently accessed entries (more likely to be relevant)
                    candidates = m_Cache.Values
                        .Where(c => !c.IsExpired() && c.MatchesContext(npcName, playerName, isVendorQuery))
                        .OrderByDescending(c => c.LastAccessed)
                        .Take(20) // Check top 20 most recently accessed entries
                        .ToList();
                }

                // Process candidates outside of lock (async operations)
                foreach (CachedResponse cached in candidates)
                {
                    // Get or compute embedding for cached query (async, non-blocking)
                    if (cached.QueryEmbedding == null && !string.IsNullOrEmpty(cached.OriginalQuery))
                    {
                        try
                        {
                            cached.QueryEmbedding = await EmbeddingService.GenerateEmbeddingAsync(cached.OriginalQuery);
                            if (cached.QueryEmbedding == null)
                            {
                                // Skip if embedding generation fails
                                continue;
                            }
                        }
                        catch
                        {
                            // Skip if embedding generation fails
                            continue;
                        }
                    }

                    if (cached.QueryEmbedding != null)
                    {
                        float similarity = EmbeddingService.CosineSimilarity(queryEmbedding, cached.QueryEmbedding);
                        
                        if (similarity > bestSimilarity && similarity >= SemanticSimilarityThreshold)
                        {
                            bestSimilarity = similarity;
                            bestMatch = cached;
                        }
                    }
                }

                if (bestMatch != null)
                {
                    // Update access stats
                    bestMatch.LastAccessed = DateTime.UtcNow;
                    bestMatch.AccessCount++;
                    
                    Console.WriteLine($"[LLMResponseCache] Cache HIT (semantic match, similarity: {bestSimilarity:F2}) for query: '{query}' matched '{bestMatch.OriginalQuery}'");
                    return bestMatch.Response;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[LLMResponseCache] Error in semantic matching: {ex.Message}");
            }

            return null;
        }

        /// <summary>
        /// Caches a response for future use
        /// </summary>
        public static void CacheResponse(string query, string response, string npcName, string playerName, bool isVendorQuery)
        {
            if (string.IsNullOrEmpty(query) || string.IsNullOrEmpty(response))
                return;

            string normalizedQuery = NormalizeQuery(query);
            
            // Check if we should cache this query
            if (!ShouldCache(normalizedQuery, isVendorQuery))
                return;

            string queryHash = GenerateQueryHash(normalizedQuery, npcName, playerName, isVendorQuery);

            lock (m_CacheLock)
            {
                // Check if already cached (update if exists)
                if (m_Cache.ContainsKey(queryHash))
                {
                    m_Cache[queryHash].Response = response;
                    m_Cache[queryHash].CachedAt = DateTime.UtcNow;
                    m_Cache[queryHash].LastAccessed = DateTime.UtcNow;
                    // Update original query in case it changed slightly
                    if (m_Cache[queryHash].OriginalQuery != query)
                    {
                        m_Cache[queryHash].OriginalQuery = query;
                        m_Cache[queryHash].QueryEmbedding = null; // Reset embedding to recompute
                    }
                    // Update context in case NPC/player changed
                    m_Cache[queryHash].NpcName = npcName;
                    m_Cache[queryHash].PlayerName = playerName;
                    m_Cache[queryHash].IsVendorQuery = isVendorQuery;
                    return;
                }

                // Check cache size and evict if needed
                if (m_Cache.Count >= MaxCacheSize)
                {
                    EvictOldestEntry();
                }

                // Add to cache (store original query and context for semantic matching)
                m_Cache[queryHash] = new CachedResponse(response, queryHash, query, npcName, playerName, isVendorQuery);
                Console.WriteLine($"[LLMResponseCache] Cached response for query hash {queryHash} (cache size: {m_Cache.Count}/{MaxCacheSize})");
            }
        }

        /// <summary>
        /// Evicts the least recently used entry
        /// </summary>
        private static void EvictOldestEntry()
        {
            if (m_Cache.Count == 0)
                return;

            // Find entry with oldest last access time
            var oldest = m_Cache.OrderBy(kvp => kvp.Value.LastAccessed).First();
            m_Cache.Remove(oldest.Key);
            Console.WriteLine($"[LLMResponseCache] Evicted oldest cache entry (hash: {oldest.Key}, accessed: {oldest.Value.AccessCount} times)");
        }

        /// <summary>
        /// Cleans up expired entries
        /// </summary>
        public static void CleanupExpired()
        {
            lock (m_CacheLock)
            {
                List<string> toRemove = new List<string>();

                foreach (var kvp in m_Cache)
                {
                    if (kvp.Value.IsExpired())
                    {
                        toRemove.Add(kvp.Key);
                    }
                }

                foreach (string key in toRemove)
                {
                    m_Cache.Remove(key);
                }

                if (toRemove.Count > 0)
                {
                    Console.WriteLine($"[LLMResponseCache] Cleaned up {toRemove.Count} expired cache entries");
                }
            }
        }

        /// <summary>
        /// Clears the entire cache
        /// </summary>
        public static void ClearCache()
        {
            lock (m_CacheLock)
            {
                int count = m_Cache.Count;
                m_Cache.Clear();
                Console.WriteLine($"[LLMResponseCache] Cleared {count} cache entries");
            }
        }

        /// <summary>
        /// Gets cache statistics
        /// </summary>
        public static string GetCacheStats()
        {
            lock (m_CacheLock)
            {
                int totalEntries = m_Cache.Count;
                int expiredEntries = m_Cache.Count(kvp => kvp.Value.IsExpired());
                int totalAccesses = m_Cache.Sum(kvp => kvp.Value.AccessCount);
                int avgAccesses = totalEntries > 0 ? totalAccesses / totalEntries : 0;

                var topEntries = m_Cache.OrderByDescending(kvp => kvp.Value.AccessCount).Take(5);

                StringBuilder stats = new StringBuilder();
                stats.AppendLine($"Cache Statistics:");
                stats.AppendLine($"  Total entries: {totalEntries}/{MaxCacheSize}");
                stats.AppendLine($"  Expired entries: {expiredEntries}");
                stats.AppendLine($"  Total accesses: {totalAccesses}");
                stats.AppendLine($"  Average accesses per entry: {avgAccesses}");
                stats.AppendLine($"  Cache TTL: {CacheTTL.TotalHours} hours");
                stats.AppendLine();
                stats.AppendLine("Top 5 most accessed entries:");
                int rank = 1;
                foreach (var entry in topEntries)
                {
                    stats.AppendLine($"  {rank}. Hash: {entry.Key.Substring(0, 8)}... Accesses: {entry.Value.AccessCount}, Age: {(DateTime.UtcNow - entry.Value.CachedAt).TotalHours:F1}h");
                    rank++;
                }

                return stats.ToString();
            }
        }
    }
}

