using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Server;
using Newtonsoft.Json;

namespace Server.Services.LLM
{
    /// <summary>
    /// Simple keyword-based lore retrieval system (Level 1 RAG)
    /// </summary>
    public static class SimpleLoreSystem
    {
        private static List<LoreEntry> m_AllLore;
        private static Dictionary<string, List<LoreEntry>> m_KeywordIndex;
        private static bool m_Initialized = false;

        /// <summary>
        /// Initialize the lore system by loading from JSON
        /// </summary>
        public static void Initialize()
        {
            if (m_Initialized)
            {
                return;
            }

            try
            {
                m_AllLore = new List<LoreEntry>();
                string loreDir = Path.Combine(Core.BaseDirectory, "Data", "Lore");

                // Load from multiple domain-specific JSON files for better organization
                // Vystia custom world lore files
                string[] loreFiles = {
                    "vystia_general.json",         // Vystia regions, cities, factions, history (replaces britannia_general)
                    "religion_domain.json",        // Vystia religions and deities
                    "class_domain.json",           // Vystia 25 character classes
                    "magic_domain.json",           // Vystia 12 magic schools
                    "creatures_domain.json",       // Vystia creatures and monsters
                    "equipment_domain.json",       // Vystia weapons, armor, materials
                    "npc_domain.json",             // Vystia NPCs and vendors
                    "crafting_domain.json",        // Vystia crafting (blacksmithing, alchemy, carpentry)
                    "combat_domain.json",          // Vystia combat mechanics
                    "healing_domain.json",         // Vystia healing arts
                    "trade_domain.json",           // Vystia trade and economy
                    "hospitality_domain.json",     // Vystia inns and taverns
                    "finance_domain.json",         // Vystia banking
                    "animal_domain.json",          // Vystia animal training
                    "food_domain.json",            // Vystia cuisine
                    "resource_domain.json",        // Vystia mining and lumberjacking
                };

                int loadedCount = 0;
                int failedCount = 0;
                var failedFiles = new List<string>();

                foreach (string loreFile in loreFiles)
                {
                    string fullPath = Path.Combine(loreDir, loreFile);
                    if (!File.Exists(fullPath))
                    {
                        Console.WriteLine($"[LoreSystem] ERROR: {loreFile} not found at {fullPath}");
                        failedCount++;
                        failedFiles.Add(loreFile);
                        continue;
                    }

                    try
                    {
                        string json = File.ReadAllText(fullPath);
                        var entries = ParseLoreJsonFast(json);
                        if (entries != null && entries.Count > 0)
                        {
                            m_AllLore.AddRange(entries);
                            loadedCount++;
                        }
                        else
                        {
                            Console.WriteLine($"[LoreSystem] WARNING: {loreFile} parsed but contains no entries");
                            failedCount++;
                            failedFiles.Add(loreFile);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"[LoreSystem] ERROR: Failed to load {loreFile}: {ex.Message}");
                        failedCount++;
                        failedFiles.Add(loreFile);
                    }
                }

                // Summary
                // Console.WriteLine($"[LoreSystem] Loaded {loadedCount} lore files ({m_AllLore.Count} total entries)");
                if (failedCount > 0)
                {
                    Console.WriteLine($"[LoreSystem] WARNING: {failedCount} file(s) failed to load: {string.Join(", ", failedFiles)}");
                }

                if (m_AllLore == null || m_AllLore.Count == 0)
                {
                    Console.WriteLine("[LoreSystem] ERROR: Failed to parse lore JSON");
                    m_AllLore = new List<LoreEntry>();
                    m_KeywordIndex = new Dictionary<string, List<LoreEntry>>();
                    return;
                }

                // Build keyword index
                BuildIndex();

                m_Initialized = true;
                // Console.WriteLine($"[LoreSystem] Built keyword index with {m_KeywordIndex.Count} unique keywords");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[LoreSystem] ERROR during initialization: {ex.Message}");
                Console.WriteLine($"[LoreSystem] Stack trace: {ex.StackTrace}");
                m_AllLore = new List<LoreEntry>();
                m_KeywordIndex = new Dictionary<string, List<LoreEntry>>();
            }
        }

        /// <summary>
        /// Build keyword index for fast searching
        /// </summary>
        private static void BuildIndex()
        {
            m_KeywordIndex = new Dictionary<string, List<LoreEntry>>(StringComparer.OrdinalIgnoreCase);

            foreach (var entry in m_AllLore)
            {
                if (entry == null)
                    continue;

                // Index by tags
                if (entry.Tags != null)
                {
                    foreach (string tag in entry.Tags)
                    {
                        if (string.IsNullOrWhiteSpace(tag))
                            continue;

                        string key = tag.ToLower().Trim();
                        if (!m_KeywordIndex.ContainsKey(key))
                            m_KeywordIndex[key] = new List<LoreEntry>();

                        if (!m_KeywordIndex[key].Contains(entry))
                            m_KeywordIndex[key].Add(entry);
                    }
                }

                // Index by words in title (skip common words)
                if (!string.IsNullOrEmpty(entry.Title))
                {
                    string[] titleWords = entry.Title.ToLower().Split(new[] { ' ', '-', '_' }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (string word in titleWords)
                    {
                        if (word.Length > 2 && !IsCommonWord(word)) // Skip short and common words
                        {
                            if (!m_KeywordIndex.ContainsKey(word))
                                m_KeywordIndex[word] = new List<LoreEntry>();

                            if (!m_KeywordIndex[word].Contains(entry))
                                m_KeywordIndex[word].Add(entry);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Check if a word is too common to be useful for indexing
        /// </summary>
        private static bool IsCommonWord(string word)
        {
            string[] commonWords = { "the", "and", "of", "to", "a", "in", "is", "for", "on", "with", "by", "at", "from" };
            return commonWords.Contains(word.ToLower());
        }

        /// <summary>
        /// Search for relevant lore entries based on a query
        /// </summary>
        /// <param name="query">The search query (player's question)</param>
        /// <param name="maxResults">Maximum number of results to return</param>
        /// <returns>List of relevant lore entries, sorted by relevance</returns>
        public static List<LoreEntry> Search(string query, int maxResults = 3)
        {
            if (!m_Initialized)
            {
                Console.WriteLine("[LoreSystem] Warning: Search called before initialization, initializing now...");
                Initialize();
            }

            if (string.IsNullOrWhiteSpace(query))
                return new List<LoreEntry>();

            // Track entries and their match scores
            var results = new Dictionary<LoreEntry, int>();

            // Extract keywords from query
            string[] keywords = query.ToLower()
                .Split(new[] { ' ', ',', '.', '?', '!' }, StringSplitOptions.RemoveEmptyEntries)
                .Where(w => w.Length > 2 && !IsCommonWord(w))
                .ToArray();

            // Search for each keyword
            foreach (string keyword in keywords)
            {
                if (m_KeywordIndex.ContainsKey(keyword))
                {
                    foreach (var entry in m_KeywordIndex[keyword])
                    {
                        if (!results.ContainsKey(entry))
                            results[entry] = 0;
                        results[entry]++;
                    }
                }
            }

            // Return top results sorted by:
            // 1. Match count (how many keywords matched)
            // 2. Importance (lore entry importance rating)
            return results
                .OrderByDescending(kvp => kvp.Value) // Most keyword matches first
                .ThenByDescending(kvp => kvp.Key.Importance) // Then by importance
                .Take(maxResults)
                .Select(kvp => kvp.Key)
                .ToList();
        }

        /// <summary>
        /// Get statistics about the lore system
        /// </summary>
        public static string GetStats()
        {
            if (!m_Initialized)
                return "Lore system not initialized.";

            return $"Lore Entries: {m_AllLore.Count}, Indexed Keywords: {m_KeywordIndex.Count}";
        }

        /// <summary>
        /// Reload the lore database from disk
        /// </summary>
        public static void Reload()
        {
            m_Initialized = false;
            Initialize();
        }

        /// <summary>
        /// Get all lore entries (for debugging/admin purposes)
        /// </summary>
        public static List<LoreEntry> GetAllLore()
        {
            if (!m_Initialized)
                Initialize();

            return m_AllLore;
        }

        /// <summary>
        /// Fast JSON parser using Newtonsoft.Json (10-100x faster than custom parser)
        /// </summary>
        private static List<LoreEntry> ParseLoreJsonFast(string json)
        {
            try
            {
                return JsonConvert.DeserializeObject<List<LoreEntry>>(json) ?? new List<LoreEntry>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[LoreSystem] JSON Parse Error (Newtonsoft): {ex.Message}");
                // Fallback to custom parser for malformed JSON
                return ParseLoreJsonLegacy(json);
            }
        }

        /// <summary>
        /// Legacy custom JSON parser (fallback only, much slower)
        /// </summary>
        private static List<LoreEntry> ParseLoreJsonLegacy(string json)
        {
            var entries = new List<LoreEntry>();

            try
            {
                Console.WriteLine($"[LoreSystem] Parsing JSON, length: {json.Length}");

                // Find all object boundaries by counting braces
                int depth = 0;
                int objectStart = -1;
                List<string> objects = new List<string>();

                for (int i = 0; i < json.Length; i++)
                {
                    if (json[i] == '{')
                    {
                        if (depth == 0)
                            objectStart = i;
                        depth++;
                    }
                    else if (json[i] == '}')
                    {
                        depth--;
                        if (depth == 0 && objectStart >= 0)
                        {
                            // Extract complete object
                            string obj = json.Substring(objectStart + 1, i - objectStart - 1);
                            objects.Add(obj);
                            objectStart = -1;
                        }
                    }
                }

                Console.WriteLine($"[LoreSystem] Found {objects.Count} objects");

                // Parse each object
                int successCount = 0;
                foreach (string objContent in objects)
                {
                    var entry = new LoreEntry();

                    // Parse each field
                    entry.ID = ExtractJsonString(objContent, "id");
                    entry.Category = ExtractJsonString(objContent, "category");
                    entry.Title = ExtractJsonString(objContent, "title");
                    entry.Content = ExtractJsonString(objContent, "content");
                    entry.Source = ExtractJsonString(objContent, "source");
                    entry.Importance = ExtractJsonInt(objContent, "importance");
                    entry.Tags = ExtractJsonArray(objContent, "tags");

                    if (!string.IsNullOrEmpty(entry.ID))
                    {
                        entries.Add(entry);
                        successCount++;
                    }
                    else
                    {
                        Console.WriteLine($"[LoreSystem] Skipped entry with no ID");
                    }
                }

                Console.WriteLine($"[LoreSystem] Successfully parsed {successCount} entries");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[LoreSystem] JSON Parse Error: {ex.Message}");
                Console.WriteLine($"[LoreSystem] Stack trace: {ex.StackTrace}");
            }

            return entries;
        }

        private static string ExtractJsonString(string json, string key)
        {
            // Try with spaces around the colon (formatted JSON)
            string pattern1 = "\"" + key + "\": \"";
            string pattern2 = "\"" + key + "\":\"";

            int startIndex = json.IndexOf(pattern1);
            int patternLength = pattern1.Length;

            if (startIndex < 0)
            {
                startIndex = json.IndexOf(pattern2);
                patternLength = pattern2.Length;
            }

            if (startIndex < 0) return "";

            startIndex += patternLength;
            int endIndex = startIndex;

            // Find the closing quote (accounting for escaped quotes)
            while (endIndex < json.Length)
            {
                if (json[endIndex] == '\"' && (endIndex == 0 || json[endIndex - 1] != '\\'))
                    break;
                endIndex++;
            }

            if (endIndex >= json.Length) return "";

            return json.Substring(startIndex, endIndex - startIndex)
                .Replace("\\\"", "\"")
                .Replace("\\n", "\n")
                .Replace("\\\\", "\\");
        }

        private static int ExtractJsonInt(string json, string key)
        {
            string pattern1 = "\"" + key + "\": ";
            string pattern2 = "\"" + key + "\":";

            int startIndex = json.IndexOf(pattern1);
            int patternLength = pattern1.Length;

            if (startIndex < 0)
            {
                startIndex = json.IndexOf(pattern2);
                patternLength = pattern2.Length;
            }

            if (startIndex < 0) return 0;

            startIndex += patternLength;

            // Skip whitespace
            while (startIndex < json.Length && char.IsWhiteSpace(json[startIndex]))
                startIndex++;

            int endIndex = startIndex;

            // Find the next comma or closing brace
            while (endIndex < json.Length && char.IsDigit(json[endIndex]))
                endIndex++;

            if (endIndex > startIndex)
            {
                string numStr = json.Substring(startIndex, endIndex - startIndex);
                int result;
                if (int.TryParse(numStr, out result))
                    return result;
            }

            return 0;
        }

        private static List<string> ExtractJsonArray(string json, string key)
        {
            var result = new List<string>();
            string pattern1 = "\"" + key + "\": [";
            string pattern2 = "\"" + key + "\":[";

            int startIndex = json.IndexOf(pattern1);
            int patternLength = pattern1.Length;

            if (startIndex < 0)
            {
                startIndex = json.IndexOf(pattern2);
                patternLength = pattern2.Length;
            }

            if (startIndex < 0) return result;

            startIndex += patternLength;
            int endIndex = json.IndexOf("]", startIndex);
            if (endIndex < 0) return result;

            string arrayContent = json.Substring(startIndex, endIndex - startIndex);

            // Split by commas (simple approach)
            string[] items = arrayContent.Split(',');
            foreach (string item in items)
            {
                string cleanItem = item.Trim().Trim('\"');
                if (!string.IsNullOrEmpty(cleanItem))
                    result.Add(cleanItem);
            }

            return result;
        }
    }

    /// <summary>
    /// Command to test and manage the lore system
    /// </summary>
    public class LoreSystemCommands
    {
        public static void Initialize()
        {
            Server.Commands.CommandSystem.Register("LoreStats", AccessLevel.GameMaster, LoreStats_OnCommand);
            Server.Commands.CommandSystem.Register("LoreSearch", AccessLevel.GameMaster, LoreSearch_OnCommand);
            Server.Commands.CommandSystem.Register("LoreReload", AccessLevel.Administrator, LoreReload_OnCommand);
        }

        [Usage("LoreStats")]
        [Description("Display lore system statistics")]
        private static void LoreStats_OnCommand(Server.Commands.CommandEventArgs e)
        {
            Mobile from = e.Mobile;
            from.SendMessage($"=== Lore System Statistics ===");
            from.SendMessage(SimpleLoreSystem.GetStats());

            var allLore = SimpleLoreSystem.GetAllLore();
            if (allLore.Count > 0)
            {
                from.SendMessage($"Top 5 most important entries:");
                var topEntries = allLore.OrderByDescending(l => l.Importance).Take(5);
                foreach (var entry in topEntries)
                {
                    from.SendMessage($"  [{entry.Importance}] {entry.Title} ({entry.Category})");
                }
            }
        }

        [Usage("LoreSearch <query>")]
        [Description("Test lore search with a query")]
        private static void LoreSearch_OnCommand(Server.Commands.CommandEventArgs e)
        {
            Mobile from = e.Mobile;

            if (e.Length < 1)
            {
                from.SendMessage("Usage: [LoreSearch <query>");
                from.SendMessage("Example: [LoreSearch britain");
                return;
            }

            string query = e.ArgString;
            from.SendMessage($"Searching for: '{query}'");

            var results = SimpleLoreSystem.Search(query, 5);

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
                from.SendMessage($"Content: {entry.Content}");
                from.SendMessage($"Tags: {string.Join(", ", entry.Tags)}");
                from.SendMessage("");
            }
        }

        [Usage("LoreReload")]
        [Description("Reload the lore database from disk")]
        private static void LoreReload_OnCommand(Server.Commands.CommandEventArgs e)
        {
            Mobile from = e.Mobile;
            from.SendMessage("Reloading lore database...");
            SimpleLoreSystem.Reload();
            from.SendMessage("Lore database reloaded.");
            from.SendMessage(SimpleLoreSystem.GetStats());
        }
    }
}
