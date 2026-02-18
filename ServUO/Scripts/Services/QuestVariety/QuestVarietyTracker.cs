using System;
using System.Collections.Generic;
using System.Linq;
using Server.Mobiles;
using Server.Custom.VystiaClasses.Quests;
using Server.Custom.VystiaClasses.Quests.Generation;
using Server.Services.QuestPersistence;

namespace Server.Services.QuestVariety
{
    /// <summary>
    /// Tracks quest variety metrics and provides analysis
    /// </summary>
    public static class QuestVarietyTracker
    {
        private static readonly List<QuestSimilarityMetrics> s_QuestHistory = new List<QuestSimilarityMetrics>();
        private static readonly List<QuestQualityMetrics> s_QualityHistory = new List<QuestQualityMetrics>();
        private static readonly Dictionary<string, List<DateTime>> s_ThemeUsageHistory = new Dictionary<string, List<DateTime>>();
        private static readonly Dictionary<string, List<DateTime>> s_LocationUsageHistory = new Dictionary<string, List<DateTime>>();
        private static readonly object s_Lock = new object();

        // Configuration
        private static readonly int s_MaxHistorySize = 1000;
        private static readonly TimeSpan s_AnalysisPeriod = TimeSpan.FromHours(24);
        private static readonly double s_VarietyThreshold = 0.7;

        /// <summary>
        /// Initialize the variety tracker
        /// </summary>
        public static void Initialize()
        {
            Console.WriteLine("[QuestVarietyTracker] Initialized");
        }

        /// <summary>
        /// Track a newly generated quest
        /// </summary>
        public static void TrackQuest(DynamicQuest quest, QuestQualityMetrics qualityMetrics = null)
        {
            if (quest == null)
                return;

            lock (s_Lock)
            {
                // Extract similarity metrics
                var similarityMetrics = QuestSimilarityDetector.ExtractQuestMetrics(quest);
                if (similarityMetrics != null)
                {
                    s_QuestHistory.Add(similarityMetrics);
                    
                    // Update theme usage history
                    var theme = similarityMetrics.Theme;
                    if (!s_ThemeUsageHistory.ContainsKey(theme))
                    {
                        s_ThemeUsageHistory[theme] = new List<DateTime>();
                    }
                    s_ThemeUsageHistory[theme].Add(DateTime.UtcNow);

                    // Update location usage history
                    var location = similarityMetrics.Location;
                    if (!s_LocationUsageHistory.ContainsKey(location))
                    {
                        s_LocationUsageHistory[location] = new List<DateTime>();
                    }
                    s_LocationUsageHistory[location].Add(DateTime.UtcNow);

                    // Maintain history size
                    if (s_QuestHistory.Count > s_MaxHistorySize)
                    {
                        s_QuestHistory.RemoveAt(0);
                    }
                }

                // Track quality metrics
                if (qualityMetrics != null)
                {
                    s_QualityHistory.Add(qualityMetrics);
                    
                    if (s_QualityHistory.Count > s_MaxHistorySize)
                    {
                        s_QualityHistory.RemoveAt(0);
                    }
                }
            }
        }

        /// <summary>
        /// Get current variety analysis
        /// </summary>
        public static VarietyAnalysis GetVarietyAnalysis()
        {
            lock (s_Lock)
            {
                var analysis = new VarietyAnalysis
                {
                    AnalyzedAt = DateTime.UtcNow
                };

                // Calculate overall variety score
                analysis.OverallVarietyScore = CalculateOverallVarietyScore();

                // Calculate theme distribution
                analysis.ThemeDistribution = CalculateThemeDistribution();

                // Calculate location distribution
                analysis.LocationDistribution = CalculateLocationDistribution();

                // Calculate difficulty distribution
                analysis.DifficultyDistribution = CalculateDifficultyDistribution();

                // Calculate repetition rate
                analysis.RepetitionRate = CalculateRepetitionRate();

                // Generate recommendations
                analysis.Recommendations = GenerateRecommendations(analysis);

                return analysis;
            }
        }

        /// <summary>
        /// Get recent quest history
        /// </summary>
        public static List<QuestSimilarityMetrics> GetRecentQuests(int count = 50)
        {
            lock (s_Lock)
            {
                return s_QuestHistory.TakeLast(count).ToList();
            }
        }

        /// <summary>
        /// Get quality metrics history
        /// </summary>
        public static List<QuestQualityMetrics> GetQualityHistory(int count = 50)
        {
            lock (s_Lock)
            {
                return s_QualityHistory.TakeLast(count).ToList();
            }
        }

        /// <summary>
        /// Check if a quest would be too repetitive
        /// </summary>
        public static bool WouldBeRepetitive(DynamicQuest quest, double threshold = 0.8)
        {
            var recentQuests = GetRecentQuests(20);
            return QuestSimilarityDetector.IsTooSimilar(quest, recentQuests, threshold);
        }

        /// <summary>
        /// Get similarity score for a quest against recent history
        /// </summary>
        public static double GetSimilarityScore(DynamicQuest quest)
        {
            var recentQuests = GetRecentQuests(20);
            var newMetrics = QuestSimilarityDetector.ExtractQuestMetrics(quest);
            
            if (newMetrics == null || !recentQuests.Any())
                return 0.0;

            var similarities = recentQuests.Select(q => QuestSimilarityDetector.CalculateSimilarity(newMetrics, q));
            return similarities.Any() ? similarities.Average() : 0.0;
        }

        /// <summary>
        /// Find similar quests in history
        /// </summary>
        public static List<QuestSimilarityMetrics> FindSimilarQuests(DynamicQuest quest, int maxResults = 10)
        {
            var questHistory = GetRecentQuests(s_MaxHistorySize);
            return QuestSimilarityDetector.FindSimilarQuests(quest, questHistory, 0.5)
                .Take(maxResults)
                .ToList();
        }

        /// <summary>
        /// Get theme usage statistics
        /// </summary>
        public static Dictionary<string, ThemeUsageStats> GetThemeUsageStats()
        {
            lock (s_Lock)
            {
                var stats = new Dictionary<string, ThemeUsageStats>();

                foreach (var kvp in s_ThemeUsageHistory)
                {
                    var themeId = kvp.Key;
                    var usageDates = kvp.Value;

                    stats[themeId] = new ThemeUsageStats
                    {
                        ThemeId = themeId,
                        UsageCount = usageDates.Count,
                        LastUsed = usageDates.LastOrDefault(),
                        UsageFrequency = CalculateUsageFrequency(usageDates),
                        AverageTimeBetweenUses = CalculateAverageTimeBetweenUses(usageDates)
                    };
                }

                return stats;
            }
        }

        /// <summary>
        /// Get location usage statistics
        /// </summary>
        public static Dictionary<string, LocationUsageStats> GetLocationUsageStats()
        {
            lock (s_Lock)
            {
                var stats = new Dictionary<string, LocationUsageStats>();

                foreach (var kvp in s_LocationUsageHistory)
                {
                    var locationId = kvp.Key;
                    var usageDates = kvp.Value;

                    stats[locationId] = new LocationUsageStats
                    {
                        LocationId = locationId,
                        UsageCount = usageDates.Count,
                        LastUsed = usageDates.LastOrDefault(),
                        UsageFrequency = CalculateUsageFrequency(usageDates),
                        AverageTimeBetweenUses = CalculateAverageTimeBetweenUses(usageDates)
                    };
                }

                return stats;
            }
        }

        /// <summary>
        /// Get quality statistics
        /// </summary>
        public static QualityStatistics GetQualityStatistics()
        {
            lock (s_Lock)
            {
                if (!s_QualityHistory.Any())
                {
                    return new QualityStatistics();
                }

                var qualities = s_QualityHistory.ToList();

                return new QualityStatistics
                {
                    TotalQuests = qualities.Count,
                    AverageOverallQuality = qualities.Average(q => q.OverallQuality),
                    AverageOriginality = qualities.Average(q => q.OriginalityScore),
                    AverageComplexity = qualities.Average(q => q.ComplexityScore),
                    AverageEngagement = qualities.Average(q => q.EngagementScore),
                    AverageCoherence = qualities.Average(q => q.CoherenceScore),
                    HighQualityQuests = qualities.Count(q => q.OverallQuality >= 0.8),
                    LowQualityQuests = qualities.Count(q => q.OverallQuality < 0.4),
                    QualityTrend = CalculateQualityTrend()
                };
            }
        }

        /// <summary>
        /// Calculate overall variety score
        /// </summary>
        private static double CalculateOverallVarietyScore()
        {
            if (s_QuestHistory.Count < 10)
                return 1.0; // Not enough data

            var recentQuests = s_QuestHistory.TakeLast(50).ToList();
            
            // Calculate variety metrics
            double themeVariety = CalculateThemeVariety(recentQuests);
            double locationVariety = CalculateLocationVariety(recentQuests);
            double objectiveVariety = CalculateObjectiveVariety(recentQuests);
            double temporalVariety = CalculateTemporalVariety(recentQuests);

            // Weighted average
            return (themeVariety * 0.4) + (locationVariety * 0.3) + (objectiveVariety * 0.2) + (temporalVariety * 0.1);
        }

        /// <summary>
        /// Calculate theme variety
        /// </summary>
        private static double CalculateThemeVariety(List<QuestSimilarityMetrics> quests)
        {
            var themes = quests.Select(q => q.Theme).Distinct().ToList();
            var themeCounts = themes.ToDictionary(theme => theme, theme => quests.Count(q => q.Theme == theme));
            
            // Calculate entropy (higher entropy = more variety)
            double totalQuests = quests.Count;
            double entropy = 0;
            
            foreach (var count in themeCounts.Values)
            {
                double probability = count / totalQuests;
                if (probability > 0)
                {
                    entropy -= probability * Math.Log(probability);
                }
            }

            // Normalize to 0-1 scale
            double maxEntropy = Math.Log(themes.Count);
            return maxEntropy > 0 ? entropy / maxEntropy : 1.0;
        }

        /// <summary>
        /// Calculate location variety
        /// </summary>
        private static double CalculateLocationVariety(List<QuestSimilarityMetrics> quests)
        {
            var locations = quests.Select(q => q.Location).Distinct().ToList();
            var locationCounts = locations.ToDictionary(location => location, quests.Count(q => q.Location == location));
            
            double totalQuests = quests.Count;
            double entropy = 0;
            
            foreach (var count in locationCounts.Values)
            {
                double probability = count / totalQuests;
                if (probability > 0)
                {
                    entropy -= probability * Math.Log(probability);
                }
            }

            double maxEntropy = Math.Log(locations.Count);
            return maxEntropy > 0 ? entropy / maxEntropy : 1.0;
        }

        /// <summary>
        /// Calculate objective variety
        /// </summary>
        private static double CalculateObjectiveVariety(List<QuestSimilarityMetrics> quests)
        {
            var allObjectives = quests.SelectMany(q => q.Objectives).ToList();
            var uniqueObjectives = allObjectives.Distinct().ToList();
            
            // Variety based on unique objectives vs total objectives
            return uniqueObjectives.Count > 0 ? (double)uniqueObjectives.Count / allObjectives.Count : 1.0;
        }

        /// <summary>
        /// Calculate temporal variety (time-based variety)
        /// </summary>
        private static double CalculateTemporalVariety(List<QuestSimilarityMetrics> quests)
        {
            if (quests.Count < 2)
                return 1.0;

            var timeSpans = new List<TimeSpan>();
            
            for (int i = 1; i < quests.Count; i++)
            {
                timeSpans.Add(quests[i].GeneratedAt - quests[i - 1].GeneratedAt);
            }

            // Calculate average time between quests
            var averageTimeSpan = TimeSpan.FromTicks((long)timeSpans.Average(ts => ts.Ticks));
            
            // Higher variety = more spread out in time
            // Normalize based on a 24-hour period
            double maxTimeSpan = TimeSpan.FromHours(24).TotalMilliseconds;
            return Math.Min(1.0, averageTimeSpan.TotalMilliseconds / maxTimeSpan);
        }

        /// <summary>
        /// Calculate theme distribution
        /// </summary>
        private static Dictionary<string, double> CalculateThemeDistribution()
        {
            var distribution = new Dictionary<string, double>();
            var totalQuests = s_QuestHistory.Count;

            if (totalQuests == 0)
                return distribution;

            var themeCounts = s_QuestHistory
                .GroupBy(q => q.Theme)
                .ToDictionary(g => g.Key, g.Count());

            foreach (var kvp in themeCounts)
            {
                distribution[kvp.Key] = (double)kvp.Value / totalQuests;
            }

            return distribution;
        }

        /// <summary>
        /// Calculate location distribution
        /// </summary>
        private static Dictionary<string, double> CalculateLocationDistribution()
        {
            var distribution = new Dictionary<string, double>();
            var totalQuests = s_QuestHistory.Count;

            if (totalQuests == 0)
                return distribution;

            var locationCounts = s_QuestHistory
                .GroupBy(q => q.Location)
                .ToDictionary(g => g.Key, g.Count());

            foreach (var kvp in locationCounts)
            {
                distribution[kvp.Key] = (double)kvp.Value / totalQuests;
            }

            return distribution;
        }

        /// <summary>
        /// Calculate difficulty distribution
        /// </summary>
        private static Dictionary<string, double> CalculateDifficultyDistribution()
        {
            var distribution = new Dictionary<string, double>();
            var totalQuests = s_QuestHistory.Count;

            if (totalQuests == 0)
                return distribution;

            // This would need to be implemented based on actual difficulty detection
            // For now, return a placeholder distribution
            distribution["easy"] = 0.4;
            distribution["medium"] = 0.4;
            distribution["hard"] = 0.2;

            return distribution;
        }

        /// <summary>
        /// Calculate repetition rate
        /// </summary>
        private static double CalculateRepetitionRate()
        {
            if (s_QuestHistory.Count < 10)
                return 0.0;

            var recentQuests = s_QuestHistory.TakeLast(50).ToList();
            var totalComparisons = 0;
            var similarCount = 0;

            for (int i = 0; i < recentQuests.Count; i++)
            {
                for (int j = i + 1; j < recentQuests.Count; j++)
                {
                    var similarity = QuestSimilarityDetector.CalculateSimilarity(recentQuests[i], recentQuests[j]);
                    if (similarity > 0.7)
                    {
                        similarCount++;
                    }
                    totalComparisons++;
                }
            }

            return totalComparisons > 0 ? (double)similarCount / totalComparisons : 0.0;
        }

        /// <summary>
        /// Generate recommendations based on analysis
        /// </summary>
        private static List<string> GenerateRecommendations(VarietyAnalysis analysis)
        {
            var recommendations = new List<string>();

            // Low variety recommendations
            if (analysis.OverallVarietyScore < s_VarietyThreshold)
            {
                recommendations.Add("Overall variety is low - consider diversifying themes and locations");
            }

            // High repetition rate recommendations
            if (analysis.RepetitionRate > 0.3)
            {
                recommendations.Add("High repetition rate detected - increase cooldowns for similar quests");
            }

            // Theme imbalance recommendations
            var maxThemePercentage = analysis.ThemeDistribution.Values.Max();
            if (maxThemePercentage > 0.5)
            {
                var dominantTheme = analysis.ThemeDistribution.FirstOrDefault(kvp => kvp.Value == maxThemePercentage).Key;
                recommendations.Add($"Theme imbalance detected - {dominantTheme} is used {maxThemePercentage:P0} of the time");
            }

            // Location imbalance recommendations
            var maxLocationPercentage = analysis.LocationDistribution.Values.Max();
            if (maxLocationPercentage > 0.5)
            {
                var dominantLocation = analysis.LocationDistribution.FirstOrDefault(kvp => kvp.Value == maxLocationPercentage).Key;
                recommendations.Add($"Location imbalance detected - {dominantLocation} is used {maxLocationPercentage:P0} of the time");
            }

            // Quality recommendations
            var qualityStats = GetQualityStatistics();
            if (qualityStats.AverageOverallQuality < 0.6)
            {
                recommendations.Add("Average quest quality is below target - consider improving quality standards");
            }

            return recommendations;
        }

        /// <summary>
        /// Calculate usage frequency for a theme/location
        /// </summary>
        private static double CalculateUsageFrequency(List<DateTime> usageDates)
        {
            if (usageDates.Count < 2)
                return 0.0;

            var timeSpan = usageDates.Last() - usageDates.First();
            var days = timeSpan.TotalDays;

            return days > 0 ? (double)usageDates.Count / days : 0.0;
        }

        /// <summary>
        /// Calculate average time between uses
        /// </summary>
        private static TimeSpan CalculateAverageTimeBetweenUses(List<DateTime> usageDates)
        {
            if (usageDates.Count < 2)
                return TimeSpan.Zero;

            var timeSpans = new List<TimeSpan>();
            for (int i = 1; i < usageDates.Count; i++)
            {
                timeSpans.Add(usageDates[i] - usageDates[i - 1]);
            }

            return TimeSpan.FromTicks((long)timeSpans.Average(ts => ts.Ticks));
        }

        /// <summary>
        /// Calculate quality trend
        /// </summary>
        private static double CalculateQualityTrend()
        {
            if (s_QualityHistory.Count < 10)
                return 0.0;

            var recentQuests = s_QualityHistory.TakeLast(10).ToList();
            var olderQuests = s_QualityHistory.SkipLast(10).Take(10).ToList();

            if (!olderQuests.Any())
                return 0.0;

            var recentAverage = recentQuests.Average(q => q.OverallQuality);
            var olderAverage = olderQuests.Average(q => q.OverallQuality);

            return recentAverage - olderAverage;
        }
    }
}
