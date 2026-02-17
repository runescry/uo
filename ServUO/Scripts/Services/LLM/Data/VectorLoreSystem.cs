using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Server;

namespace Server.Services.LLM
{
    /// <summary>
    /// Enhanced lore system using vector embeddings for semantic search (Level 2 RAG)
    /// Falls back to keyword search if embeddings unavailable
    /// </summary>
    public static class VectorLoreSystem
    {
        private static Dictionary<string, float[]> m_LoreEmbeddings;
        private static Dictionary<string, LoreEntry> m_LoreByID;
        private static bool m_Initialized = false;
        private static bool m_VectorSearchEnabled = false;
        private static readonly string EmbeddingCachePath = Path.Combine(Core.BaseDirectory.Directory, "Data", "Lore", "lore_embeddings.cache");

        // Query embedding cache - speeds up repeated/similar queries
        private static Dictionary<string, CachedQueryEmbedding> m_QueryCache = new Dictionary<string, CachedQueryEmbedding>();
        private static readonly int MaxQueryCacheSize = 100;
        private static readonly TimeSpan QueryCacheTTL = TimeSpan.FromMinutes(5);

        private class CachedQueryEmbedding
        {
            public float[] Embedding { get; set; }
            public DateTime Timestamp { get; set; }
        }

        /// <summary>
        /// Initialize the vector lore system
        /// </summary>
        public static void Initialize()
        {
            if (m_Initialized)
            {
                return;
            }

            try
            {
                // Initialize embedding service
                EmbeddingService.Initialize();

                // Initialize base lore system
                // Console.WriteLine("[VectorLoreSystem] Initializing knowledge base (RAG)...");
                SimpleLoreSystem.Initialize();

                // Build lore lookup by ID
                // Console.WriteLine("[VectorLoreSystem] Building lore lookup index...");
                m_LoreByID = new Dictionary<string, LoreEntry>();
                var allLore = SimpleLoreSystem.GetAllLore();
                foreach (var entry in allLore)
                {
                    if (!string.IsNullOrEmpty(entry.ID))
                    {
                        m_LoreByID[entry.ID] = entry;
                    }
                }
                // Console.WriteLine($"[VectorLoreSystem] Indexed {m_LoreByID.Count} lore entries by ID");

                // Try to load cached embeddings
                // Console.WriteLine("[VectorLoreSystem] Loading vector embeddings...");
                if (LoadEmbeddingCache())
                {
                    m_VectorSearchEnabled = true;
                    // Console.WriteLine($"[VectorLoreSystem] Loaded {m_LoreEmbeddings.Count} cached embeddings (VECTOR SEARCH: ENABLED)");
                }
                else if (EmbeddingService.IsAvailable())
                {
                    // Console.WriteLine("[VectorLoreSystem] No cached embeddings found (optional - keyword search works fine)");
                    // Console.WriteLine("[VectorLoreSystem] To enable vector search, run [GenerateEmbeddings command in-game");
                    // Console.WriteLine("[VectorLoreSystem] Using keyword-based RAG (fully functional)");
                    m_VectorSearchEnabled = false;
                }
                else
                {
                    // Console.WriteLine("[VectorLoreSystem] Vector search unavailable (Ollama not running)");
                    // Console.WriteLine("[VectorLoreSystem] Using keyword-based RAG (fully functional - no action needed)");
                    // Console.WriteLine("[VectorLoreSystem] Note: Vector search is optional. Keyword search works great!");
                    m_VectorSearchEnabled = false;
                }

                m_Initialized = true;
                // Console.WriteLine($"[VectorLoreSystem] RAG System initialized: {(m_VectorSearchEnabled ? "Vector + Keyword" : "Keyword-only")}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[VectorLoreSystem] ERROR during initialization: {ex.Message}");
                Console.WriteLine($"[VectorLoreSystem] Stack trace: {ex.StackTrace}");
            }
        }

        /// <summary>
        /// Search for relevant lore using vector similarity or keyword fallback
        /// Fast mode: Returns keyword results immediately if embedding not cached (2-3 seconds faster!)
        /// </summary>
        public static async Task<List<LoreEntry>> SearchAsync(string query, int maxResults = 3, bool fastMode = true)
        {
            DateTime searchStart = DateTime.UtcNow;

            if (!m_Initialized)
            {
                Console.WriteLine("[VectorLoreSystem] Not initialized, initializing now...");
                Initialize();
            }

            // Try vector search if available
            if (m_VectorSearchEnabled && EmbeddingService.IsAvailable())
            {
                // FAST MODE: Check if embedding is cached
                if (fastMode)
                {
                    string cacheKey = query.ToLower().Trim();
                    if (m_QueryCache.ContainsKey(cacheKey))
                    {
                        var cached = m_QueryCache[cacheKey];
                        if (DateTime.UtcNow - cached.Timestamp < QueryCacheTTL)
                        {
                            // Cached! Use vector search (instant)
                            Console.WriteLine("[VectorLoreSystem] Using cached query embedding");
                            var results = await VectorSearch(query, maxResults);
                            TimeSpan elapsed = DateTime.UtcNow - searchStart;
                            Console.WriteLine($"[VectorLoreSystem] [TIMING] Search time: {elapsed.TotalMilliseconds:F0}ms (cached)");
                            return results;
                        }
                    }

                    // Not cached - use keyword search immediately, cache embedding for next time
                    Console.WriteLine("[VectorLoreSystem] Fast mode: Using keyword search, caching embedding for next query");
                    var keywordResults = SimpleLoreSystem.Search(query, maxResults);
                    TimeSpan keywordTime = DateTime.UtcNow - searchStart;
                    Console.WriteLine($"[VectorLoreSystem] [TIMING] Search time: {keywordTime.TotalMilliseconds:F0}ms (keyword + background cache)");

                    // Pre-cache embedding in background (fire-and-forget)
                    _ = Task.Run(async () => // Intentionally not awaited
                    {
                        try
                        {
                            var embedding = await EmbeddingService.GenerateEmbeddingAsync(query);
                            if (embedding != null)
                            {
                                lock (m_QueryCache)
                                {
                                    if (m_QueryCache.Count >= MaxQueryCacheSize)
                                    {
                                        var oldest = m_QueryCache.OrderBy(kvp => kvp.Value.Timestamp).First();
                                        m_QueryCache.Remove(oldest.Key);
                                    }
                                    m_QueryCache[cacheKey] = new CachedQueryEmbedding
                                    {
                                        Embedding = embedding,
                                        Timestamp = DateTime.UtcNow
                                    };
                                }
                                Console.WriteLine("[VectorLoreSystem] Embedding cached for future queries");
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"[VectorLoreSystem] Error caching embedding: {ex.Message}");
                        }
                    });

                    return keywordResults;
                }
                else
                {
                    // Slow mode: Always wait for vector search
                    return await VectorSearch(query, maxResults);
                }
            }
            else
            {
                // Fallback to keyword search
                Console.WriteLine("[VectorLoreSystem] Using keyword search fallback");
                return SimpleLoreSystem.Search(query, maxResults);
            }
        }

        /// <summary>
        /// Perform vector-based semantic search
        /// </summary>
        private static async Task<List<LoreEntry>> VectorSearch(string query, int maxResults)
        {
            try
            {
                // Check query cache first
                float[] queryEmbedding = null;
                string cacheKey = query.ToLower().Trim();

                if (m_QueryCache.ContainsKey(cacheKey))
                {
                    var cached = m_QueryCache[cacheKey];
                    if (DateTime.UtcNow - cached.Timestamp < QueryCacheTTL)
                    {
                        queryEmbedding = cached.Embedding;
                        Console.WriteLine("[VectorLoreSystem] Using cached query embedding");
                    }
                    else
                    {
                        // Expired, remove it
                        m_QueryCache.Remove(cacheKey);
                    }
                }

                // Generate embedding if not cached
                if (queryEmbedding == null)
                {
                    queryEmbedding = await EmbeddingService.GenerateEmbeddingAsync(query);

                    if (queryEmbedding == null)
                    {
                        Console.WriteLine("[VectorLoreSystem] Failed to generate query embedding, falling back to keyword search");
                        return SimpleLoreSystem.Search(query, maxResults);
                    }

                    // Cache the query embedding
                    if (m_QueryCache.Count >= MaxQueryCacheSize)
                    {
                        // Remove oldest entry
                        var oldest = m_QueryCache.OrderBy(kvp => kvp.Value.Timestamp).First();
                        m_QueryCache.Remove(oldest.Key);
                    }
                    m_QueryCache[cacheKey] = new CachedQueryEmbedding
                    {
                        Embedding = queryEmbedding,
                        Timestamp = DateTime.UtcNow
                    };
                }

                // Calculate similarity scores for all lore entries
                var scores = new List<(LoreEntry entry, float similarity)>();

                foreach (var kvp in m_LoreEmbeddings)
                {
                    string loreID = kvp.Key;
                    float[] loreEmbedding = kvp.Value;

                    if (m_LoreByID.ContainsKey(loreID))
                    {
                        float similarity = EmbeddingService.CosineSimilarity(queryEmbedding, loreEmbedding);
                        scores.Add((m_LoreByID[loreID], similarity));
                    }
                }

                // Sort by similarity and take top results
                var results = scores
                    .OrderByDescending(s => s.similarity)
                    .Take(maxResults)
                    .Select(s => s.entry)
                    .ToList();

                Console.WriteLine($"[VectorLoreSystem] Vector search found {results.Count} results for query: {query}");
                if (results.Count > 0)
                {
                    Console.WriteLine($"[VectorLoreSystem] Top result: {results[0].Title} (similarity: {scores.OrderByDescending(s => s.similarity).First().similarity:F3})");
                }

                return results;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[VectorLoreSystem] Error in vector search: {ex.Message}");
                Console.WriteLine("[VectorLoreSystem] Falling back to keyword search");
                return SimpleLoreSystem.Search(query, maxResults);
            }
        }

        /// <summary>
        /// Generate embeddings for all lore entries
        /// </summary>
        public static async Task GenerateAllEmbeddings()
        {
            if (!EmbeddingService.IsAvailable())
            {
                Console.WriteLine("[VectorLoreSystem] ERROR: Ollama not available. Cannot generate embeddings.");
                return;
            }

            Console.WriteLine("[VectorLoreSystem] Starting embedding generation for all lore entries...");

            var allLore = SimpleLoreSystem.GetAllLore();
            m_LoreEmbeddings = new Dictionary<string, float[]>();

            int successCount = 0;
            int failCount = 0;

            for (int i = 0; i < allLore.Count; i++)
            {
                var entry = allLore[i];

                try
                {
                    // Create embedding text from title and content
                    string embeddingText = $"{entry.Title}. {entry.Content}";

                    // Generate embedding
                    float[] embedding = await EmbeddingService.GenerateEmbeddingAsync(embeddingText);

                    if (embedding != null)
                    {
                        m_LoreEmbeddings[entry.ID] = embedding;
                        successCount++;
                    }
                    else
                    {
                        failCount++;
                        Console.WriteLine($"[VectorLoreSystem] Failed to generate embedding for: {entry.Title}");
                    }

                    // Progress update
                    if ((i + 1) % 10 == 0 || i == allLore.Count - 1)
                    {
                        Console.WriteLine($"[VectorLoreSystem] Progress: {i + 1}/{allLore.Count} ({successCount} success, {failCount} failed)");
                    }

                    // Small delay to avoid overwhelming Ollama
                    await Task.Delay(200);
                }
                catch (Exception ex)
                {
                    failCount++;
                    Console.WriteLine($"[VectorLoreSystem] Exception generating embedding for {entry.Title}: {ex.Message}");
                }
            }

            Console.WriteLine($"[VectorLoreSystem] Embedding generation complete: {successCount}/{allLore.Count} successful");

            // Save to cache
            if (successCount > 0)
            {
                SaveEmbeddingCache();
                m_VectorSearchEnabled = true;
                Console.WriteLine("[VectorLoreSystem] Vector search ENABLED");
            }
        }

        /// <summary>
        /// Save embeddings to cache file
        /// </summary>
        private static void SaveEmbeddingCache()
        {
            try
            {
                // Create directory if needed
                string dir = Path.GetDirectoryName(EmbeddingCachePath);
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }

                using (FileStream fs = File.Create(EmbeddingCachePath))
                using (BinaryWriter writer = new BinaryWriter(fs))
                {
                    // Write version
                    writer.Write(1);

                    // Write count
                    writer.Write(m_LoreEmbeddings.Count);

                    // Write each embedding
                    foreach (var kvp in m_LoreEmbeddings)
                    {
                        // Write ID
                        writer.Write(kvp.Key);

                        // Write embedding dimension
                        writer.Write(kvp.Value.Length);

                        // Write embedding values
                        foreach (float value in kvp.Value)
                        {
                            writer.Write(value);
                        }
                    }
                }

                Console.WriteLine($"[VectorLoreSystem] Saved {m_LoreEmbeddings.Count} embeddings to cache: {EmbeddingCachePath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[VectorLoreSystem] Error saving embedding cache: {ex.Message}");
            }
        }

        /// <summary>
        /// Load embeddings from cache file
        /// </summary>
        private static bool LoadEmbeddingCache()
        {
            try
            {
                if (!File.Exists(EmbeddingCachePath))
                {
                    // Don't log this as an error - cache is optional, keyword search works fine
                    return false;
                }

                m_LoreEmbeddings = new Dictionary<string, float[]>();

                using (FileStream fs = File.OpenRead(EmbeddingCachePath))
                using (BinaryReader reader = new BinaryReader(fs))
                {
                    // Read version
                    int version = reader.ReadInt32();

                    // Read count
                    int count = reader.ReadInt32();

                    // Read each embedding
                    for (int i = 0; i < count; i++)
                    {
                        // Read ID
                        string id = reader.ReadString();

                        // Read dimension
                        int dimension = reader.ReadInt32();

                        // Read embedding values
                        float[] embedding = new float[dimension];
                        for (int j = 0; j < dimension; j++)
                        {
                            embedding[j] = reader.ReadSingle();
                        }

                        m_LoreEmbeddings[id] = embedding;
                    }
                }

                Console.WriteLine($"[VectorLoreSystem] Loaded {m_LoreEmbeddings.Count} embeddings from cache");
                return m_LoreEmbeddings.Count > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[VectorLoreSystem] Error loading embedding cache: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Get statistics
        /// </summary>
        public static string GetStats()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("=== Vector Lore System Statistics ===");
            sb.AppendLine($"Initialized: {m_Initialized}");
            sb.AppendLine($"Vector Search: {(m_VectorSearchEnabled ? "ENABLED" : "DISABLED")}");
            sb.AppendLine($"Ollama Available: {EmbeddingService.IsAvailable()}");
            sb.AppendLine($"Cached Embeddings: {m_LoreEmbeddings?.Count ?? 0}");
            sb.AppendLine($"Total Lore Entries: {m_LoreByID?.Count ?? 0}");

            if (m_LoreEmbeddings != null && m_LoreEmbeddings.Count > 0)
            {
                var firstEmbedding = m_LoreEmbeddings.Values.First();
                sb.AppendLine($"Embedding Dimension: {firstEmbedding.Length}");
            }

            return sb.ToString();
        }
    }

    /// <summary>
    /// Commands to manage vector lore system
    /// </summary>
    public class VectorLoreCommands
    {
        public static void Initialize()
        {
            Server.Commands.CommandSystem.Register("GenerateEmbeddings", AccessLevel.Administrator, GenerateEmbeddings_OnCommand);
            Server.Commands.CommandSystem.Register("VectorLoreStats", AccessLevel.GameMaster, VectorLoreStats_OnCommand);
            Server.Commands.CommandSystem.Register("TestVectorSearch", AccessLevel.GameMaster, TestVectorSearch_OnCommand);
        }

        [Usage("GenerateEmbeddings")]
        [Description("Generate vector embeddings for all lore entries (requires Ollama)")]
        private static async void GenerateEmbeddings_OnCommand(Server.Commands.CommandEventArgs e)
        {
            Mobile from = e.Mobile;
            from.SendMessage("Starting embedding generation. This will take several minutes...");
            from.SendMessage("Check console for progress updates.");

            await VectorLoreSystem.GenerateAllEmbeddings();

            from.SendMessage("Embedding generation complete! Check console for details.");
        }

        [Usage("VectorLoreStats")]
        [Description("Display vector lore system statistics")]
        private static void VectorLoreStats_OnCommand(Server.Commands.CommandEventArgs e)
        {
            Mobile from = e.Mobile;
            string stats = VectorLoreSystem.GetStats();
            string[] lines = stats.Split('\n');
            foreach (string line in lines)
            {
                from.SendMessage(line);
            }
        }

        [Usage("TestVectorSearch <query>")]
        [Description("Test vector search with a query")]
        private static async void TestVectorSearch_OnCommand(Server.Commands.CommandEventArgs e)
        {
            Mobile from = e.Mobile;

            if (e.Length < 1)
            {
                from.SendMessage("Usage: [TestVectorSearch <query>");
                from.SendMessage("Example: [TestVectorSearch evil wizard");
                return;
            }

            string query = e.ArgString;
            from.SendMessage($"Searching for: '{query}'");

            var results = await VectorLoreSystem.SearchAsync(query, 5);

            if (results.Count == 0)
            {
                from.SendMessage("No results found.");
                return;
            }

            from.SendMessage($"Found {results.Count} results:");
            foreach (var entry in results)
            {
                from.SendMessage($"--- {entry.Title} ---");
                from.SendMessage($"Category: {entry.Category}, Importance: {entry.Importance}");
                from.SendMessage($"Content: {entry.Content.Substring(0, Math.Min(150, entry.Content.Length))}...");
                from.SendMessage("");
            }
        }
    }
}
