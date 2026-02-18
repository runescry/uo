using System;
using Server;
using Server.Commands;
using Server.Mobiles;
using Server.Services.LLM;

namespace Server.Services.QuestVariety
{
    /// <summary>
    /// Initializes the quest variety and content management system
    /// </summary>
    public static class QuestVarietyInitializer
    {
        private static bool s_Initialized = false;

        /// <summary>
        /// Initialize the quest variety system
        /// </summary>
        public static void Initialize()
        {
            if (s_Initialized)
                return;

            s_Initialized = true;

            // Initialize the quest variety system
            QuestContentManager.Initialize();

            // Register administrative commands
            CommandSystem.Register("QuestVariety", AccessLevel.Administrator, QuestVariety_OnCommand);
            CommandSystem.Register("QV", AccessLevel.Administrator, QuestVariety_OnCommand);
            CommandSystem.Register("QuestVarietyReport", AccessLevel.Administrator, QuestVarietyReport_OnCommand);

            Console.WriteLine("[QuestVariety] Quest variety and content management system initialized");
            Console.WriteLine("[QuestVariety] Administrative commands:");
            Console.WriteLine("  [QuestVariety or [QV] - Quest variety management commands");
            Console.WriteLine("  [QuestVarietyReport] - Generate variety report");
        }

        /// <summary>
        /// Main quest variety command handler
        /// </summary>
        [Usage("QuestVariety")]
        [Description("Administrative commands for quest variety management")]
        private static void QuestVariety_OnCommand(CommandEventArgs e)
        {
            Mobile from = e.Mobile;
            
            if (from.AccessLevel < AccessLevel.Administrator)
            {
                from.SendMessage("This command requires administrator access.");
                return;
            }

            if (e.Length == 0)
            {
                ShowQuestVarietyHelp(from);
                return;
            }

            string subCommand = e.GetString(0).ToLower();

            switch (subCommand)
            {
                case "stats":
                    ShowQuestVarietyStats(from);
                    break;

                case "report":
                    GenerateVarietyReport(from);
                    break;

                case "reset":
                    ResetQuestVarietyStats(from);
                    break;

                case "variety":
                    ShowVarietyAnalysis(from);
                    break;

                case "quality":
                    ShowQualityStats(from);
                    break;

                case "themes":
                    ShowThemeStats(from);
                    break;

                case "preferences":
                    ShowPreferenceStats(from);
                    break;

                default:
                    ShowQuestVarietyHelp(from);
                    break;
            }
        }

        /// <summary>
        /// Quest variety report command handler
        /// </summary>
        [Usage("QuestVarietyReport")]
        [Description("Generate comprehensive quest variety report")]
        private static void QuestVarietyReport_OnCommand(CommandEventArgs e)
        {
            Mobile from = e.Mobile;
            
            if (from.AccessLevel < AccessLevel.Administrator)
            {
                from.SendMessage("This command requires administrator access.");
                return;
            }

            try
            {
                var report = QuestContentManager.GenerateVarietyReport();
                from.SendMessage("=== QUEST VARIETY REPORT ===");
                from.SendMessage(report);
            }
            catch (Exception ex)
            {
                from.SendMessage($"Error generating report: {ex.Message}");
            }
        }

        /// <summary>
        /// Show quest variety help
        /// </summary>
        private static void ShowQuestVarietyHelp(Mobile from)
        {
            from.SendMessage("=== QUEST VARIETY COMMANDS ===");
            from.SendMessage("Usage: [QuestVariety <command>");
            from.SendMessage("");
            from.SendMessage("Available commands:");
            from.SendMessage("  stats     - Show quest variety statistics");
            from.SendMessage("  report    - Generate comprehensive variety report");
            from.SendMessage("  reset     - Reset all statistics");
            from.SendMessage("  variety   - Show variety analysis");
            from.SendMessage("  quality   - Show quality statistics");
            from.SendMessage("  themes    - Show theme usage statistics");
            from.SendMessage("  preferences - Show player preference statistics");
            from.SendMessage("");
            from.SendMessage("Examples:");
            from.SendMessage("  [QuestVariety stats] - Show current variety metrics");
            from.SendMessage("  [QuestVarietyReport] - Generate detailed variety report");
            from.SendMessage("  [QuestVariety reset] - Reset all tracking data");
        }

        /// <summary>
        /// Show quest variety statistics
        /// </summary>
        private static void ShowQuestVarietyStats(Mobile from)
        {
            try
            {
                var stats = QuestContentManager.GetStatistics();
                
                from.SendMessage("=== QUEST VARIETY STATISTICS ===");
                from.SendMessage($"Generated: {stats.LastUpdated:yyyy-MM-dd HH:mm:ss}");
                from.SendMessage("");
                from.SendMessage("OVERALL:");
                from.SendMessage($"  Total Quests Generated: {stats.TotalQuestsGenerated}");
                from.SendMessage($"  Quests Accepted: {stats.QuestsAccepted}");
                from.SendMessage($"  Quests Rejected for Similarity: {stats.QuestsRejectedForSimilarity}");
                from.SendMessage($"  Quests Rejected for Quality: {stats.QuestsRejectedForQuality}");
                from.SendMessage($"  Acceptance Rate: {stats.AcceptanceRate:P1}");
                from.SendMessage("");
                from.SendMessage("VARIETY ANALYSIS:");
                from.SendMessage($"  Overall Variety Score: {stats.VarietyAnalysis.OverallVarietyScore:P1}");
                from.SendMessage($"  Repetition Rate: {stats.VarietyAnalysis.RepetitionRate:P1}");
                from.SendMessage("");
                from.SendMessage("QUALITY METRICS:");
                from.SendMessage($"  Average Quality: {stats.QualityStatistics.AverageOverallQuality:P1}");
                from.SendMessage($"  High Quality Quests: {stats.QualityStatistics.HighQualityQuests}");
                from.SendMessage($"  Low Quality Quests: {stats.QualityStatistics.LowQualityQuests}");
                from.SendMessage($"  Quality Trend: {stats.QualityStatistics.QualityTrend:+0.00}");
                from.SendMessage("");
                from.SendMessage("THEME USAGE:");
                foreach (var kvp in stats.ThemeStats.OrderByDescending(x => x.Value.UsageCount))
                {
                    from.SendMessage($"  {kvp.Value.Name}: {kvp.Value.UsageCount} uses");
                }
            }
            catch (Exception ex)
            {
                from.SendMessage($"Error retrieving stats: {ex.Message}");
            }
        }

        /// <summary>
        /// Show quality statistics
        /// </summary>
        private static void ShowQualityStats(Mobile from)
        {
            try
            {
                var stats = QuestContentManager.GetStatistics().QualityStatistics;
                
                from.SendMessage("=== QUALITY STATISTICS ===");
                from.SendMessage($"Generated: {QuestContentManager.GetStatistics().LastUpdated:yyyy-MM-dd HH:mm:ss}");
                from.SendMessage("");
                from.SendMessage("QUALITY METRICS:");
                from.SendMessage($"  Total Quests Assessed: {stats.TotalQuests}");
                from.SendMessage($"  Average Overall Quality: {stats.AverageOverallQuality:P1}");
                from.SendMessage($"  Average Originality: {stats.AverageOriginality:P1}");
                from.SendMessage($"  Average Complexity: {stats.AverageComplexity:P1}");
                from.SendMessage($"  Average Engagement: {stats.AverageEngagement:P1}");
                from.SendMessage($"  Average Coherence: {stats.AverageCoherence:P1}");
                from.SendMessage("");
                from.SendMessage("QUALITY DISTRIBUTION:");
                from.SendMessage($"  High Quality (≥0.8): {stats.HighQualityQuests} ({(double)stats.HighQualityQuests / stats.TotalQuests:P1})");
                from.SendMessage($"  Low Quality (<0.4): {stats.LowQualityQuests} ({(double)stats.LowQualityQuests / stats.TotalQuests:P1})");
                from.SendMessage($"  Medium Quality: {stats.TotalQuests - stats.HighQualityQuests - stats.LowQualityQuests} ({(double)(stats.TotalQuests - stats.HighQualityQuests - stats.LowQualityQuests) / stats.TotalQuests:P1})");
                from.SendMessage($"  Quality Trend: {stats.QualityTrend:+0.00}");
            }
            catch (Exception ex)
            {
                from.SendMessage($"Error retrieving quality stats: {ex.Message}");
            }
        }

        /// <summary>
        /// Show theme usage statistics
        /// </summary>
        private static void ShowThemeStats(Mobile from)
        {
            try
            {
                var stats = QuestContentManager.GetStatistics().ThemeStats;
                
                from.SendMessage("=== THEME USAGE STATISTICS ===");
                from.SendMessage($"Generated: {QuestContentManager.GetStatistics().LastUpdated:yyyy-MM-dd HH:mm:ss}");
                from.SendMessage("");
                from.SendMessage("THEME USAGE (Last 30 days):");
                
                foreach (var kvp in stats.OrderByDescending(x => x.Value.UsageCount))
                {
                    var cooldown = QuestThemeRotation.GetThemeCooldown(kvp.Key);
                    var cooldownText = cooldown > TimeSpan.Zero ? $" ({cooldown.Days:F1} days)" : "Available";
                    
                    from.SendMessage($"  {kvp.Value.Name}: {kvp.Value.UsageCount} uses");
                    from.SendMessage($"    Last Used: {kvp.Value.LastUsed:yyyy-MM-dd HH:mm:ss}");
                    from.SendMessage($"    Cooldown: {cooldownText}");
                }
            }
            catch (Exception ex)
            {
                from.SendMessage($"Error retrieving theme stats: {ex.Message}");
            }
        }

        /// <summary>
        /// Show player preference statistics
        /// </summary>
        private static void ShowPreferenceStats(Mobile from)
        {
            try
            {
                var stats = QuestContentManager.GetStatistics().PreferenceStats;
                
                from.SendMessage("=== PLAYER PREFERENCE STATISTICS ===");
                from.SendMessage($"Generated: {QuestContentManager.GetStatistics().LastUpdated:yyyy-MM-dd HH:mm:ss}");
                from.SendMessage("");
                from.SendMessage("PLAYER PREFERENCES:");
                from.SendMessage($"  Total Players: {stats.TotalPlayers}");
                from.SendMessage($"  Average Quests Completed: {stats.AverageQuestsCompleted:F1}");
                from.SendMessage($"  Average Rating: {stats.AverageRating:F2}");
                from.SendMessage("");
                from.SendMessage("MOST COMMON THEMES:");
                foreach (var kvp in stats.MostCommonThemes.Take(5))
                {
                    from.SendMessage($"  {kvp.Key}: {kvp.Value} players");
                }
                from.SendMessage("");
                from.SendMessage("MOST COMMON LOCATIONS:");
                foreach (var kvp in stats.MostCommonLocations.Take(5))
                {
                    from.SendMessage($"  {kvp.Key}: {kvp.Value} players");
                }
                from.SendMessage("");
                from.SendMessage("PREFERENCE ENGAGEMENT:");
                from.SendMessage($"  Total Disliked Quests: {stats.TotalDislikedQuests}");
                from.SendMessage($"  Total Preferred Quests: {stats.TotalPreferredQuests}");
            }
            catch (Exception ex)
            {
                from.SendMessage($"Error retrieving preference stats: {ex.Message}");
            }
        }

        /// <summary>
        /// Show variety analysis
        /// </summary>
        private static void ShowVarietyAnalysis(Mobile from)
        {
            try
            {
                var analysis = QuestContentManager.GetVarietyAnalysis();
                
                from.SendMessage("=== VARIETY ANALYSIS ===");
                from.SendMessage($"Generated: {analysis.AnalyzedAt:yyyy-MM-dd HH:mm:ss}");
                from.SendMessage("");
                from.SendMessage("VARIETY SCORES:");
                from.SendMessage($"  Overall Variety: {analysis.OverallVarietyScore:P1}");
                from.SendMessage($"  Theme Variety: {analysis.ThemeDistribution.Values.Average():P1}");
                from.SendMessage($"  Location Variety: {analysis.LocationDistribution.Values.Average():P1}");
                from.SendMessage($"  Objective Variety: {analysis.ObjectiveVariety:P1}");
                from.SendMessage($"  Temporal Variety: {analysis.TemporalVariety:P1}");
                from.SendMessage("");
                from.SendMessage("REPETITION ANALYSIS:");
                from.SendMessage($"  Repetition Rate: {analysis.RepetitionRate:P1}");
                from.SendMessage($"  Threshold: {0.8:P1}");
                from.SendMessage("");
                from.SendMessage("RECOMMENDATIONS:");
                foreach (var recommendation in analysis.Recommendations)
                {
                    from.SendMessage($"  • {recommendation}");
                }
            }
            catch (Exception ex)
            {
                from.SendMessage($"Error retrieving variety analysis: {ex.Message}");
            }
        }

        /// <summary>
        /// Generate variety report
        /// </summary>
        private static void GenerateVarietyReport(Mobile from)
        {
            try
            {
                var stats = QuestContentManager.GetStatistics();
                
                from.SendMessage("=== QUEST VARIETY REPORT ===");
                from.SendMessage($"Generated: {stats.LastUpdated:yyyy-MM-dd HH:mm:ss}");
                from.SendMessage("");
                from.SendMessage("OVERALL STATISTICS:");
                from.SendMessage($"  Total Quests Generated: {stats.TotalQuestsGenerated}");
                from.SendMessage($"  Quests Accepted: {stats.QuestsAccepted}");
                from.SendMessage($"  Quests Rejected for Similarity: {stats.QuestsRejectedForSimilarity}");
                from.SendMessage($"  Quests Rejected for Quality: {stats.QuestsRejectedForQuality}");
                from.SendMessage($"  Acceptance Rate: {stats.AcceptanceRate:P1}");
                from.SendMessage("");
                from.SendMessage("VARIETY ANALYSIS:");
                from.SendMessage($"  Overall Variety Score: {stats.VarietyAnalysis.OverallVarietyScore:P1}");
                from.SendMessage($"  Repetition Rate: {stats.VarietyAnalysis.RepetitionRate:P1}");
                from.SendMessage("");
                from.SendMessage("QUALITY STATISTICS:");
                from.SendMessage($"  Average Quality: {stats.QualityStatistics.AverageOverallQuality:P1}");
                from.SendMessage($"  High Quality Quests: {stats.QualityStatistics.HighQualityQuests}");
                from.SendMessage($"  Low Quality Quests: {stats.QualityStatistics.LowQualityQuests}");
                from.SendMessage($"  Quality Trend: {stats.QualityStatistics.QualityTrend:+0.00}");
                from.SendMessage("");
                from.SendMessage("THEME DISTRIBUTION:");
                foreach (var kvp in stats.VarietyAnalysis.ThemeDistribution.OrderByDescending(x => x.Value))
                {
                    from.SendMessage($"  {kvp.Key}: {kvp.Value:P1}");
                }
                from.SendMessage("");
                from.SendMessage("RECOMMENDATIONS:");
                foreach (var recommendation in stats.VarietyAnalysis.Recommendations)
                {
                    from.SendMessage($"  • {recommendation}");
                }
            }
            catch (Exception ex)
            {
                from.SendMessage($"Error generating report: {ex.Message}");
            }
        }

        /// <summary>
        /// Reset all quest variety statistics
        /// </summary>
        private static void ResetQuestVarietyStats(Mobile from)
        {
            QuestContentManager.ResetStatistics();
            
            from.SendMessage("[QuestVariety] All quest variety statistics have been reset.");
        }
    }
}
