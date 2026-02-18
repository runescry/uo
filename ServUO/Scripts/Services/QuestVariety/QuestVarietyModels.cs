using System;
using System.Collections.Generic;
using System.Linq;
using Server.Mobiles;
using Server.Custom.VystiaClasses.Quests;
using Server.Custom.VystiaClasses.Quests.Generation;

namespace Server.Services.QuestVariety
{
    /// <summary>
    /// Quest similarity metrics for content variety tracking
    /// </summary>
    public class QuestSimilarityMetrics
    {
        public string QuestId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Theme { get; set; }
        public string Location { get; set; }
        public string QuestType { get; set; }
        public List<string> Objectives { get; set; }
        public List<string> Rewards { get; set; }
        public DateTime GeneratedAt { get; set; }
        public double SimilarityScore { get; set; }
        public Dictionary<string, double> FeatureVector { get; set; }
    }

    /// <summary>
    /// Player preference data for personalized quest generation
    /// </summary>
    public class PlayerPreferences
    {
        public PlayerMobile Player { get; set; }
        public Dictionary<string, double> ThemePreferences { get; set; }
        public Dictionary<string, double> DifficultyPreferences { get; set; }
        public Dictionary<string, double> LocationPreferences { get; set; }
        public Dictionary<string, double> QuestTypePreferences { get; set; }
        public List<string> DislikedQuests { get; set; }
        public List<string> PreferredQuests { get; set; }
        public double AverageQuestRating { get; set; }
        public int TotalQuestsCompleted { get; set; }
        public DateTime LastUpdated { get; set; }
    }

    /// <summary>
    /// Quest theme information for rotation management
    /// </summary>
    public class QuestTheme
    {
        public string ThemeId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public List<string> Keywords { get; set; }
        public List<string> Objectives { get; set; }
        public List<string> Locations { get; set; }
        public List<string> Rewards { get; set; }
        public double Weight { get; set; }
        public int CooldownPeriod { get; set; }
        public DateTime LastUsed { get; set; }
        public int UsageCount { get; set; }
        public bool IsActive { get; set; }
    }

    /// <summary>
    /// Content variety tracking and analysis
    /// </summary>
    public class VarietyAnalysis
    {
        public double OverallVarietyScore { get; set; }
        public Dictionary<string, double> ThemeDistribution { get; set; }
        public Dictionary<string, double> LocationDistribution { get; set; }
        public Dictionary<string, double> DifficultyDistribution { get; set; }
        public double RepetitionRate { get; set; }
        public List<string> Recommendations { get; set; }
        public DateTime AnalyzedAt { get; set; }
    }

    /// <summary>
    /// Theme usage statistics
    /// </summary>
    public class ThemeUsageStats
    {
        public string ThemeId { get; set; }
        public string Name { get; set; }
        public int UsageCount { get; set; }
        public DateTime LastUsed { get; set; }
        public TimeSpan CooldownRemaining { get; set; }
        public double Weight { get; set; }
        public bool IsActive { get; set; }
    }

    /// <summary>
    /// Location usage statistics
    /// </summary>
    public class LocationUsageStats
    {
        public string LocationId { get; set; }
        public int UsageCount { get; set; }
        public DateTime LastUsed { get; set; }
        public double UsageFrequency { get; set; }
        public TimeSpan AverageTimeBetweenUses { get; set; }
    }

    /// <summary>
    /// Quality statistics
    /// </summary>
    public class QualityStatistics
    {
        public int TotalQuests { get; set; }
        public double AverageOverallQuality { get; set; }
        public double AverageOriginality { get; set; }
        public double AverageComplexity { get; set; }
        public double AverageEngagement { get; set; }
        public double AverageCoherence { get; set; }
        public int HighQualityQuests { get; set; }
        public int LowQualityQuests { get; set; }
        public double QualityTrend { get; set; }
    }
}
