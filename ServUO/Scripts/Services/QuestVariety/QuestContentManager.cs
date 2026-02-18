using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Server.Mobiles;
using Server.Custom.VystiaClasses.Quests;
using Server.Custom.VystiaClasses.Quests.Generation;
using Server.Services.LLM;

namespace Server.Services.QuestVariety
{
    /// <summary>
    /// Main coordinator for quest variety and content management
    /// </summary>
    public static class QuestContentManager
    {
        private static readonly object s_Lock = new object();
        private static bool s_Initialized = false;

        // Configuration
        private static readonly double s_MinimumQualityThreshold = 0.4;
        private static readonly double s_MaximumSimilarityThreshold = 0.8;
        private static readonly int s_MaxRecentQuestsForComparison = 20;

        // Statistics
        private static int s_TotalQuestsGenerated = 0;
        private static int s_QuestsRejectedForSimilarity = 0;
        private static int s_QuestsRejectedForQuality = 0;
        private static int s_QuestsAccepted = 0;

        /// <summary>
        /// Initialize the quest content management system
        /// </summary>
        public static void Initialize()
        {
            lock (s_Lock)
            {
                if (s_Initialized)
                    return;

                // Initialize subsystems
                QuestThemeRotation.Initialize();
                PlayerPreferenceLearning.Initialize();
                QuestVarietyTracker.Initialize();

                s_Initialized = true;
                Console.WriteLine("[QuestContentManager] Initialized quest variety and content management system");
            }
        }

        /// <summary>
        /// Generate a quest with variety and quality controls
        /// </summary>
        public static async Task<DynamicQuest> GenerateQuestWithVarietyControls(
            PlayerMobile player, 
            string npcName, 
            string npcPersonality, 
            List<string> conversationHistory, 
            string playerMessage, 
            string playerName, 
            string preloadedKnowledge)
        {
            if (!s_Initialized)
                Initialize();

            DynamicQuest quest = null;
            int attempts = 0;
            const int maxAttempts = 5;

            lock (s_Lock)
            {
                s_TotalQuestsGenerated++;
            }

            try
            {
                while (attempts < maxAttempts)
                {
                    attempts++;

                    // Get player preferences for personalization
                    var preferences = PlayerPreferenceLearning.GetPlayerPreferences(player);
                    var generationParams = PlayerPreferenceLearning.GetGenerationParameters(player);

                    // Select appropriate theme
                    var excludedThemes = GetExcludedThemes(player);
                    var theme = QuestThemeRotation.SelectTheme(player, excludedThemes);

                    // Generate quest with theme and preference parameters
                    quest = await GenerateQuestWithTheme(
                        theme,
                        generationParams,
                        npcName,
                        npcPersonality,
                        conversationHistory,
                        playerMessage,
                        playerName,
                        preloadedKnowledge
                    );

                    if (quest == null)
                    {
                        Console.WriteLine($"[QuestContentManager] Quest generation attempt {attempts} failed");
                        continue;
                    }

                    // Assess quality
                    var qualityAssessor = new QuestQualityMetrics();
                    var qualityMetrics = qualityAssessor.AssessQuestQuality(quest);

                    // Check quality standards
                    if (!QuestQualityMetrics.MeetsMinimumStandards(qualityMetrics))
                    {
                        lock (s_Lock)
                        {
                            s_QuestsRejectedForQuality++;
                        }
                        
                        Console.WriteLine($"[QuestManager] Quest rejected for low quality (score: {qualityMetrics.OverallQuality:F2})");
                        
                        // Get improvement recommendations
                        var recommendations = QuestQualityMetrics.GetImprovementRecommendations(qualityMetrics);
                        if (recommendations.Any())
                        {
                            Console.WriteLine($"[QuestManager] Quality recommendations: {string.Join("; ", recommendations)}");
                        }
                        
                        continue;
                    }

                    // Check for repetition
                    if (QuestVarietyTracker.WouldBeRepetitive(quest, s_MaximumSimilarityThreshold))
                    {
                        lock (s_Lock)
                        {
                            s_QuestsRejectedForSimilarity++;
                        }
                        
                        Console.WriteLine($"[QuestManager] Quest rejected for similarity (threshold: {s_MaximumSimilarityThreshold})");
                        
                        // Find similar quests for reference
                        var similarQuests = QuestVarietyTracker.FindSimilarQuests(quest, 3);
                        if (similarQuests.Any())
                        {
                            Console.WriteLine($"[QuestManager] Similar quests found: {string.Join(", ", similarQuests.Select(q => q.Title))}");
                        }
                        
                        continue;
                    }

                    // Quest passed all checks
                    lock (s_Lack)
                    {
                        s_QuestsAccepted++;
                    }

                    Console.WriteLine($"[QuestContentManager] Quest generated successfully (attempts: {attempts}, quality: {qualityMetrics.OverallQuality:F2})");
                    break;
                }

                // Track the successful quest
                if (quest != null)
                {
                    QuestVarietyTracker.TrackQuest(quest, qualityMetrics);
                    
                    // Update player preferences if we have rating data
                    // This would be called separately when player rates the quest
                    // PlayerPreferenceLearning.UpdatePreferencesFromQuest(player, quest);
                }

                return quest;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[QuestContentManager] Error generating quest: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Generate a quest with specific theme and parameters
        /// </summary>
        private static async Task<DynamicQuest> GenerateQuestWithTheme(
            QuestTheme theme,
            Dictionary<string, object> parameters,
            string npcName,
            string npcPersonality,
            List<string> conversationHistory,
            string playerMessage,
            string playerName,
            string preloadedKnowledge)
        {
            // This would integrate with the LLM quest generation system
            // For now, create a mock quest with theme-based content
            
            var quest = new DynamicQuest();
            
            // Set basic properties
            quest.SetTitle(GenerateThemeBasedTitle(theme, npcName));
            quest.SetDescription(GenerateThemeBasedDescription(theme, npcName, playerMessage));
            
            // Set theme-based objectives
            var objectives = GenerateThemeBasedObjectives(theme);
            foreach (var objective in objectives)
            {
                quest.SetObjective(objective.Key, objective.Value);
            }

            // Set theme-based rewards
            var rewards = GenerateThemeBasedRewards(theme);
            quest.SetRewards(rewards);

            // Apply player preference adjustments
            if (parameters != null)
            {
                ApplyPreferenceAdjustments(quest, parameters);
            }

            return quest;
        }

        /// <summary>
        /// Generate a theme-based title
        /// </summary>
        private static string GenerateThemeBasedTitle(QuestTheme theme, string npcName)
        {
            var titles = new Dictionary<string, List<string>>
            {
                ["combat_basic"] = new List<string>
                    { $"{npcName}'s Challenge", "Battle for {npcName}", "The {npcName} Problem", "Defeat {npcName}'s Foes"
                },
                ["exploration_mystery"] = new List<string>
                    { "The Lost Artifact", "Mystery of {npcName}", "Discover the Truth", "Investigate the Area"
                },
                ["collection_gathering"] = new List<string>
                    { "Resource Collection", "Gathering Task", "Supply Run", "Material Acquisition"
                },
                ["delivery_transport"] = new List<string>
                    { "Urgent Delivery", "Package Transport", "Message Courier", "Supply Run"
                },
                ["protection_guard"] = new List<string>
                    { "Protection Duty", "Guard Assignment", "Defense Mission", "Shield {npcName}"
                },
                ["rescue_aid"] = new List<string>
                    { "Rescue Mission", "Help Request", "Save Operation", "Aid Required"
                },
                ["political_intrigue"] = new List<string>
                    { "Political Matter", "Diplomatic Task", "Secret Mission", "Covert Operation"
                },
                ["magical_mystical"] = new List<string>
                    { "Magical Investigation", "Arcane Discovery", "Mystical Quest", "Spellbound Task"
                }
            };

            var themeTitles = titles.GetValueOrDefault(theme.ThemeId, titles["combat_basic"]);
            var title = themeTitles[s_Random.Next(themeTitles.Count)];

            // Add variation
            var variations = new[]
            {
                " - Part 1", " - Chapter 1", " - The Beginning", " - First Step",
                " - A New Hope", " - Rising Action", " - Call to Arms", " - The Task"
            };

            var variation = variations[s_Random.Next(variations.Length)];
            
            return title + variation;
        }

        /// <summary>
        /// Generate a theme-based description
        /// </summary>
        private static string GenerateThemeBasedDescription(QuestTheme theme, string npcName, string playerMessage)
        {
            var descriptions = new Dictionary<string, string>
            {
                ["combat_basic"] = $"{npcName} needs your help with a combat situation. {playerMessage} This is a basic combat quest that will test your fighting skills and bravery.",
                ["exploration_mystery"] = $"{npcName} has discovered something mysterious that needs investigation. {playerMessage} This exploration quest will take you to uncharted territories and ancient ruins.",
                ["collection_gathering"] = $"{npcName} requires certain resources for an important task. {playerMessage} This collection quest will test your gathering skills and knowledge of the land.",
                ["delivery_transport"] = $"{npcName} has an urgent delivery that needs your attention. {playerMessage} This delivery quest requires speed and reliability.",
                ["protection_guard"] = $"{npcName} is in danger and needs protection. {playerMessage} This protection quest will test your defensive capabilities and loyalty.",
                ["rescue_aid"] = $"{npcName} or someone they know needs immediate assistance. {playerMessage} This rescue mission requires courage and quick thinking.",
                ["political_intrigue"] = $"{npcName} has a delicate political matter that requires discretion. {playerMessage} This intrigue quest demands subtlety and diplomacy.",
                ["magical_mystical"] = $"{npcName} has encountered something magical that defies explanation. {playerMessage} This mystical quest will test your understanding of the arcane."
            };

            return descriptions.GetValueOrDefault(theme.ThemeId, descriptions["combat_basic"]);
        }

        /// <summary>
        /// Generate theme-based objectives
        /// </summary>
        private static Dictionary<string, int> GenerateThemeBasedObjectives(QuestTheme theme)
        {
            var objectives = new Dictionary<string, int>();

            switch (theme.ThemeId)
            {
                case "combat_basic":
                    objectives["defeat enemies"] = s_Random.Next(5, 15);
                    objectives["clear area"] = 1;
                    break;
                case "exploration_mystery":
                    objectives["explore location"] = 1;
                    objectives["find clues"] = s_Random.Next(3, 8);
                    objectives["solve mystery"] = 1;
                    break;
                case "collection_gathering":
                    objectives["collect items"] = s_Random.Next(5, 15);
                    objectives["gather resources"] = s_Random.Next(3, 10);
                    break;
                case "delivery_transport":
                    objectives["deliver package"] = 1;
                    objectives["reach destination"] = 1;
                    break;
                case "protection_guard":
                    objectives["protect npc"] = 1;
                    objectives["defend area"] = 1;
                    objectives["eliminate threats"] = s_Random.Next(2, 8);
                    break;
                case "rescue_aid":
                    objectives["rescue person"] = 1;
                    objectives["reach victim"] = 1;
                    objectives["ensure safety"] = 1;
                    break;
                case "political_intrigue":
                    objectives["gather information"] = 1;
                    objectives["make contact"] = 1;
                    objectives["complete mission"] = 1;
                    break;
                case "magical_mystical":
                    objectives["investigate phenomenon"] = 1;
                    objectives["find magical item"] = 1;
                    objectives["solve magical puzzle"] = 1;
                    break;
            }

            return objectives;
        }

        /// <summary>
        /// Generate theme-based rewards
        /// </summary>
        private static List<string> GenerateThemeBasedRewards(QuestTheme theme)
        {
            var rewards = new List<string>();

            switch (theme.ThemeId)
            {
                case "combat_basic":
                    rewards.Add("experience");
                    rewards.Add("gold");
                    rewards.Add("combat reputation");
                    break;
                case "exploration_mystery":
                    rewards.Add("knowledge");
                    rewards.Add("ancient artifacts");
                    rewards.Add("discovery reputation");
                    break;
                case "collection_gathering":
                    rewards.Add("collected items");
                    rewards.Add("crafting materials");
                    rewards.Add("payment");
                    break;
                case "delivery_transport":
                    rewards.Add("delivery payment");
                    rewards.Add("transport reputation");
                    rewards.Add("favors");
                    break;
                case "protection_guard":
                    rewards.Add("protection payment");
                    rewards.Add("honor");
                    rewards.Add("guard reputation");
                    break;
                case "rescue_aid":
                    rewards.Add("gratitude");
                    rewards.Add("rescue reward");
                    rewards.Add("alliance");
                    break;
                case "political_intrigue":
                    rewards.Add("political favor");
                    rewards.Add("influence");
                    rewards.Add("secret information");
                    break;
                case "magical_mystical":
                    rewards.Add("magical items");
                    rewards.Add("arcane knowledge");
                    rewards.Add("mystical reputation");
                    break;
            }

            return rewards;
        }

        /// <        /// Apply preference adjustments to quest
        /// </summary>
        private static void ApplyPreferenceAdjustments(DynamicQuest quest, Dictionary<string, object> parameters)
        {
            // Apply theme bias
            if (parameters.TryGetValue("theme_bias", out var themeBias))
            {
                // Adjust quest content based on preferred theme
                // This would modify the quest generation to align with the theme
            }

            // Apply difficulty adjustment
            if (parameters.TryGetValue("difficulty_adjustment", out var difficultyAdjustment))
            {
                // Adjust objective counts based on difficulty preference
                var objectives = quest.GetObjectives();
                var adjustedObjectives = new Dictionary<string, int>();

                foreach (var objective in objectives)
                {
                    int adjustedCount = objective.Value;
                    
                    if (difficultyAdjustment.ToString() == "harder")
                    {
                        adjustedCount = (int)(adjustedCount * 1.5);
                    }
                    else if (difficultyAdjustment.ToString() == "easier")
                    {
                        adjustedCount = Math.Max(1, (int)(adjustedCount * 0.7));
                    }

                    adjustedObjectives[objective.Key] = adjustedCount;
                }

                // Update quest objectives
                foreach (var kvp in adjustedObjectives)
                {
                    quest.SetObjective(kvp.Key, kvp.Value);
                }
            }

            // Apply location preference
            if (parameters.TryGetValue("location_preference", out var locationPreference))
            {
                // Adjust quest location based on preference
                // This would modify the quest to use the preferred location
            }

            // Avoid disliked themes
            if (parameters.TryGetValue("avoid_themes", out var avoidThemes))
            {
                // Ensure quest doesn't use disliked themes
                // This would check and modify the quest content
            }
        }

        /// <summary>
        /// Get excluded themes for a player
        /// </summary>
        private static List<string> GetExcludedThemes(PlayerMobile player)
        {
            var excludedThemes = new List<string>();
            
            var preferences = PlayerPreferenceLearning.GetPlayerPreferences(player);
            if (preferences != null)
            {
                // Add themes that the player has disliked
                excludedThemes.AddRange(preferences.DislikedQuests);
                
                // Add themes that are on cooldown
                var themeStats = QuestThemeRotation.GetThemeStats();
                foreach (var stat in themeStats.Values)
                {
                    if (stat.CooldownRemaining > TimeSpan.Zero)
                    {
                        excludedThemes.Add(stat.ThemeId);
                    }
                }
            }

            return excludedThemes.Distinct().ToList();
        }

        /// <        /// Get comprehensive statistics
        /// </summary>
        public static ContentManagerStatistics GetStatistics()
        {
            lock (s_Lock)
            {
                var varietyAnalysis = QuestVarietyTracker.GetVarietyAnalysis();
                var qualityStats = QuestVarietyTracker.GetQualityStatistics();
                var themeStats = QuestThemeRotation.GetThemeStats();
                var locationStats = QuestVarietyTracker.GetLocationUsageStats();
                var preferenceStats = PlayerPreferenceLearning.GetPreferenceStatistics();

                return new ContentManagerStatistics
                {
                    TotalQuestsGenerated = s_TotalQuestsGenerated,
                    QuestsAccepted = s_QuestsAccepted,
                    QuestsRejectedForSimilarity = s_QuestsRejectedForSimilarity,
                    QuestsRejectedForQuality = s_QuestsRejectedForQuality,
                    AcceptanceRate = s_TotalQuestsGenerated > 0 ? (double)s_QuestsAccepted / s_TotalQuestsGenerated : 0.0,
                    VarietyAnalysis = varietyAnalysis,
                    QualityStatistics = qualityStats,
                    ThemeStats = themeStats,
                    LocationStats = locationStats,
                    PreferenceStats = preferenceStats,
                    LastUpdated = DateTime.UtcNow
                };
            }
        }

        /// <        /// Generate variety report
        /// </summary>
        public static string GenerateVarietyReport()
        {
            var stats = GetStatistics();
            var report = new System.Text.StringBuilder();

            report.AppendLine("=== QUEST VARIETY REPORT ===");
            report.AppendLine($"Generated: {stats.LastUpdated:yyyy-MM-dd HH:mm:ss}");
            report.AppendLine();

            report.AppendLine("OVERALL STATISTICS:");
            report.AppendLine($"  Total Quests Generated: {stats.TotalQuestsGenerated}");
            report.AppendLine($"  Quests Accepted: {stats.QuestsAccepted}");
            report.AppendLine($"  Quests Rejected for Similarity: {stats.QuestsRejectedForSimilarity}");
            report.AppendLine($"  Quests Rejected for Quality: {stats.QuestsRejectedForQuality}");
            report.AppendLine($"  Acceptance Rate: {stats.AcceptanceRate:P1}");
            report.AppendLine();

            report.AppendLine("VARIETY ANALYSIS:");
            report.AppendLine($"  Overall Variety Score: {stats.VarietyAnalysis.OverallVarietyScore:P1}");
            report.AppendLine($"  Repetition Rate: {stats.VarietyAnalysis.RepetitionRate:P1}");
            report.AppendLine();

            report.AppendLine("QUALITY STATISTICS:");
            report.AppendLine($"  Average Quality: {stats.QualityStatistics.AverageOverallQuality:P1}");
            report.AppendLine($"  High Quality Quests: {stats.QualityStatistics.HighQualityQuests}");
            report.AppendLine($"  Low Quality Quests: {stats.QualityStatistics.LowQualityQuests}");
            report.AppendLine($"  Quality Trend: {stats.QualityStatistics.QualityTrend:+0.00}");
            report.AppendLine();

            report.AppendLine("THEME DISTRIBUTION:");
            foreach (var kvp in stats.VarietyAnalysis.ThemeDistribution.OrderByDescending(x => x.Value))
            {
                report.AppendLine($"  {kvp.Key}: {kvp.Value:P1}");
            }
            report.AppendLine();

            report.AppendLine("RECOMMENDATIONS:");
            foreach (var recommendation in stats.VarietyAnalysis.Recommendations)
            {
                report.AppendLine($"  • {recommendation}");
            }

            return report.ToString();
        }

        /// <        /// Reset statistics
        /// </summary>
        public static void ResetStatistics()
        {
            lock (s_Lock)
            {
                s_TotalQuestsGenerated = 0;
                s_QuestsAccepted = 0;
                s_QuestsRejectedForSimilarity = 0;
                s_QuestsRejectedForQuality = 0;
            }
        }
    }

    /// <summary>
    /// Comprehensive statistics for the quest content management system
    /// </summary>
    public class ContentManagerStatistics
    {
        public int TotalQuestsGenerated { get; set; }
        public int QuestsAccepted { get; set; }
        public int QuestsRejectedForSimilarity { get; set; }
        public int QuestsRejectedForQuality { get; set; }
        public double AcceptanceRate { get; set; }
        public VarietyAnalysis VarietyAnalysis { get; set; }
        public QualityStatistics QualityStatistics { get; set; }
        public Dictionary<string, ThemeUsageStats> ThemeStats { get; set; }
        public Dictionary<string, LocationUsageStats> LocationStats { get; set; }
        public PreferenceStatistics PreferenceStats { get; set; }
        public DateTime LastUpdated { get; set; }
    }
}
