using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Server.Mobiles;
using Server.Custom.VystiaClasses.Quests;
using Server.Custom.VystiaClasses.Quests.Generation;

namespace Server.Services.QuestVariety
{
    /// <summary>
    /// Detects similarity between quests to prevent repetitive content
    /// </summary>
    public static class QuestSimilarityDetector
    {
        private static readonly Dictionary<string, List<string>> s_Synonyms = new Dictionary<string, List<string>>
        {
            ["kill"] = new List<string> { "defeat", "slay", "eliminate", "destroy", "vanquish" },
            ["collect"] = new List<string> { "gather", "find", "acquire", "obtain", "retrieve" },
            ["deliver"] = new List<string> { "bring", "transport", "carry", "take", "return" },
            ["explore"] = new List<string> { "investigate", "search", "scout", "survey", "discover" },
            ["protect"] = new List<string> { "guard", "defend", "shield", "safeguard", "watch" },
            ["rescue"] = new List<string> { "save", "free", "liberate", "help", "assist" }
        };

        /// <summary>
        /// Calculate similarity between two quests
        /// </summary>
        public static double CalculateSimilarity(DynamicQuest quest1, DynamicQuest quest2)
        {
            if (quest1 == null || quest2 == null)
                return 0.0;

            var metrics1 = ExtractQuestMetrics(quest1);
            var metrics2 = ExtractQuestMetrics(quest2);

            return CalculateSimilarity(metrics1, metrics2);
        }

        /// <summary>
        /// Calculate similarity between two quest metrics
        /// </summary>
        public static double CalculateSimilarity(QuestSimilarityMetrics metrics1, QuestSimilarityMetrics metrics2)
        {
            if (metrics1 == null || metrics2 == null)
                return 0.0;

            double similarity = 0.0;
            int factors = 0;

            // Title similarity (30% weight)
            double titleSim = CalculateTextSimilarity(metrics1.Title, metrics2.Title);
            similarity += titleSim * 0.3;
            factors++;

            // Description similarity (25% weight)
            double descSim = CalculateTextSimilarity(metrics1.Description, metrics2.Description);
            similarity += descSim * 0.25;
            factors++;

            // Theme similarity (20% weight)
            double themeSim = metrics1.Theme == metrics2.Theme ? 1.0 : 0.0;
            similarity += themeSim * 0.2;
            factors++;

            // Location similarity (15% weight)
            double locSim = CalculateTextSimilarity(metrics1.Location, metrics2.Location);
            similarity += locSim * 0.15;
            factors++;

            // Objectives similarity (10% weight)
            double objSim = CalculateListSimilarity(metrics1.Objectives, metrics2.Objectives);
            similarity += objSim * 0.1;
            factors++;

            return similarity;
        }

        /// <summary>
        /// Extract quest metrics for similarity analysis
        /// </summary>
        public static QuestSimilarityMetrics ExtractQuestMetrics(DynamicQuest quest)
        {
            if (quest == null)
                return null;

            var objectives = quest.GetObjectives()?.Keys.ToList() ?? new List<string>();
            var rewards = ExtractRewardKeywords(quest);

            return new QuestSimilarityMetrics
            {
                QuestId = quest.QuestID.ToString(),
                Title = quest.Title ?? "",
                Description = quest.Description ?? "",
                Theme = DetectQuestTheme(quest),
                Location = DetectQuestLocation(quest),
                QuestType = "Dynamic",
                Objectives = objectives,
                Rewards = rewards,
                GeneratedAt = DateTime.UtcNow,
                FeatureVector = CreateFeatureVector(quest)
            };
        }

        /// <summary>
        /// Check if a quest is too similar to recently generated quests
        /// </summary>
        public static bool IsTooSimilar(DynamicQuest quest, List<QuestSimilarityMetrics> recentQuests, double threshold = 0.8)
        {
            var newMetrics = ExtractQuestMetrics(quest);
            
            foreach (var recent in recentQuests)
            {
                double similarity = CalculateSimilarity(newMetrics, recent);
                if (similarity >= threshold)
                    return true;
            }
            
            return false;
        }

        /// <summary>
        /// Find similar quests in the quest history
        /// </summary>
        public static List<QuestSimilarityMetrics> FindSimilarQuests(DynamicQuest quest, List<QuestSimilarityMetrics> questHistory, double minSimilarity = 0.5)
        {
            var newMetrics = ExtractQuestMetrics(quest);
            var similarQuests = new List<QuestSimilarityMetrics>();

            foreach (var historical in questHistory)
            {
                double similarity = CalculateSimilarity(newMetrics, historical);
                if (similarity >= minSimilarity)
                {
                    historical.SimilarityScore = similarity;
                    similarQuests.Add(historical);
                }
            }

            return similarQuests.OrderByDescending(q => q.SimilarityScore).ToList();
        }

        /// <summary>
        /// Calculate text similarity using Jaccard similarity and keyword matching
        /// </summary>
        private static double CalculateTextSimilarity(string text1, string text2)
        {
            if (string.IsNullOrEmpty(text1) && string.IsNullOrEmpty(text2))
                return 1.0;
            
            if (string.IsNullOrEmpty(text1) || string.IsNullOrEmpty(text2))
                return 0.0;

            var words1 = TokenizeAndNormalize(text1);
            var words2 = TokenizeAndNormalize(text2);

            // Jaccard similarity
            var intersection = words1.Intersect(words2).ToList();
            var union = words1.Union(words2).ToList();
            
            double jaccardSim = union.Count > 0 ? (double)intersection.Count / union.Count : 0.0;

            // Keyword similarity bonus
            double keywordBonus = CalculateKeywordSimilarity(words1, words2);

            return (jaccardSim * 0.7) + (keywordBonus * 0.3);
        }

        /// <summary>
        /// Calculate similarity between two lists of strings
        /// </summary>
        private static double CalculateListSimilarity(List<string> list1, List<string> list2)
        {
            if (list1 == null || list2 == null || list1.Count == 0 || list2.Count == 0)
                return 0.0;

            var intersection = list1.Intersect(list2).ToList();
            var union = list1.Union(list2).ToList();
            
            return union.Count > 0 ? (double)intersection.Count / union.Count : 0.0;
        }

        /// <summary>
        /// Tokenize and normalize text for comparison
        /// </summary>
        private static HashSet<string> TokenizeAndNormalize(string text)
        {
            var tokens = new HashSet<string>();
            
            if (string.IsNullOrEmpty(text))
                return tokens;

            var words = text.ToLower()
                .Replace(@"/[^\w\s]/g", "")
                .Split(new[] { ' ', '\t', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var word in words)
            {
                tokens.Add(word);
                
                // Add synonyms
                if (s_Synonyms.TryGetValue(word, out var synonyms))
                {
                    foreach (var synonym in synonyms)
                    {
                        tokens.Add(synonym);
                    }
                }
            }

            return tokens;
        }

        /// <summary>
        /// Calculate keyword similarity bonus
        /// </summary>
        private static double CalculateKeywordSimilarity(HashSet<string> words1, HashSet<string> words2)
        {
            var actionWords1 = words1.Where(w => IsActionWord(w)).ToList();
            var actionWords2 = words2.Where(w => IsActionWord(w)).ToList();
            
            if (actionWords1.Count == 0 || actionWords2.Count == 0)
                return 0.0;

            var commonActions = actionWords1.Intersect(actionWords2).ToList();
            return (double)commonActions.Count / Math.Max(actionWords1.Count, actionWords2.Count);
        }

        /// <summary>
        /// Check if a word is an action word
        /// </summary>
        private static bool IsActionWord(string word)
        {
            var actionWords = new HashSet<string>
            {
                "kill", "defeat", "slay", "collect", "gather", "find", "deliver", "bring",
                "explore", "investigate", "protect", "guard", "rescue", "save", "destroy",
                "eliminate", "retrieve", "obtain", "acquire", "return", "transport", "carry"
            };
            
            return actionWords.Contains(word);
        }

        /// <summary>
        /// Detect quest theme based on keywords
        /// </summary>
        private static string DetectQuestTheme(DynamicQuest quest)
        {
            var text = $"{quest.Title} {quest.Description}".ToLower();
            
            var themes = new Dictionary<string, List<string>>
            {
                ["combat"] = new List<string> { "kill", "defeat", "battle", "fight", "war", "combat", "enemy" },
                ["exploration"] = new List<string> { "explore", "discover", "find", "search", "investigate", "map" },
                ["collection"] = new List<string> { "collect", "gather", "find", "acquire", "obtain", "retrieve" },
                ["delivery"] = new List<string> { "deliver", "bring", "carry", "transport", "return", "take" },
                ["protection"] = new List<string> { "protect", "guard", "defend", "shield", "watch", "safeguard" },
                ["rescue"] = new List<string> { "rescue", "save", "free", "liberate", "help", "assist" },
                ["mystery"] = new List<string> { "mystery", "secret", "hidden", "unknown", "investigate", "solve" },
                ["crafting"] = new List<string> { "craft", "create", "build", "make", "forge", "construct" }
            };

            string bestTheme = "general";
            int maxMatches = 0;

            foreach (var theme in themes)
            {
                int matches = theme.Value.Count(keyword => text.Contains(keyword));
                if (matches > maxMatches)
                {
                    maxMatches = matches;
                    bestTheme = theme.Key;
                }
            }

            return bestTheme;
        }

        /// <summary>
        /// Detect quest location based on keywords
        /// </summary>
        private static string DetectQuestLocation(DynamicQuest quest)
        {
            var text = $"{quest.Title} {quest.Description}".ToLower();
            
            var locations = new Dictionary<string, List<string>>
            {
                ["forest"] = new List<string> { "forest", "woods", "trees", "jungle", "grove" },
                ["dungeon"] = new List<string> { "dungeon", "cave", "crypt", "tomb", "underground" },
                ["city"] = new List<string> { "city", "town", "village", "settlement", "urban" },
                ["mountain"] = new List<string> { "mountain", "peak", "summit", "hill", "cliff" },
                ["water"] = new List<string> { "water", "lake", "river", "ocean", "sea", "beach" },
                ["castle"] = new List<string> { "castle", "fortress", "keep", "tower", "stronghold" },
                ["ruins"] = new List<string> { "ruins", "ancient", "old", "forgotten", "abandoned" }
            };

            string bestLocation = "unknown";
            int maxMatches = 0;

            foreach (var location in locations)
            {
                int matches = location.Value.Count(keyword => text.Contains(keyword));
                if (matches > maxMatches)
                {
                    maxMatches = matches;
                    bestLocation = location.Key;
                }
            }

            return bestLocation;
        }

        /// <summary>
        /// Extract reward keywords from quest
        /// </summary>
        private static List<string> ExtractRewardKeywords(DynamicQuest quest)
        {
            var rewards = new List<string>();
            var text = quest.Description.ToLower();
            
            if (text.Contains("gold") || text.Contains("money") || text.Contains("coin"))
                rewards.Add("gold");
            
            if (text.Contains("item") || text.Contains("equipment") || text.Contains("gear"))
                rewards.Add("items");
            
            if (text.Contains("experience") || text.Contains("xp") || text.Contains("level"))
                rewards.Add("experience");
            
            if (text.Contains("fame") || text.Contains("reputation") || text.Contains("honor"))
                rewards.Add("reputation");
            
            return rewards;
        }

        /// <summary>
        /// Create feature vector for machine learning analysis
        /// </summary>
        private static Dictionary<string, double> CreateFeatureVector(DynamicQuest quest)
        {
            var vector = new Dictionary<string, double>();
            var text = $"{quest.Title} {quest.Description}".ToLower();
            
            // Action type features
            vector["has_kill"] = text.ContainsAny(new[] { "kill", "defeat", "slay" }) ? 1.0 : 0.0;
            vector["has_collect"] = text.ContainsAny(new[] { "collect", "gather", "find" }) ? 1.0 : 0.0;
            vector["has_deliver"] = text.ContainsAny(new[] { "deliver", "bring", "carry" }) ? 1.0 : 0.0;
            vector["has_explore"] = text.ContainsAny(new[] { "explore", "discover", "search" }) ? 1.0 : 0.0;
            vector["has_protect"] = text.ContainsAny(new[] { "protect", "guard", "defend" }) ? 1.0 : 0.0;
            
            // Complexity features
            vector["word_count"] = text.Split(' ').Length;
            vector["sentence_count"] = text.Split('.').Length;
            vector["unique_words"] = TokenizeAndNormalize(text).Count;
            
            // Theme features
            vector["is_combat"] = DetectQuestTheme(quest) == "combat" ? 1.0 : 0.0;
            vector["is_exploration"] = DetectQuestTheme(quest) == "exploration" ? 1.0 : 0.0;
            vector["is_collection"] = DetectQuestTheme(quest) == "collection" ? 1.0 : 0.0;
            
            return vector;
        }
    }

    /// <summary>
    /// Extension methods for string operations
    /// </summary>
    public static class StringExtensions
    {
        public static bool ContainsAny(this string text, string[] substrings)
        {
            return substrings.Any(substring => text.Contains(substring));
        }
    }
}
