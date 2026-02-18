using System;
using System.Collections.Generic;
using System.Linq;
using Server.Mobiles;
using Server.Custom.VystiaClasses.Quests;
using Server.Services.QuestVariety;
using Server.Services.LLM;

namespace Server.Services.UnifiedQuestSystem
{
    /// <summary>
    /// Integration manager for LLM Quest Generation and Quest Variety System
    /// Bridges the gap between AI-generated quests and variety tracking
    /// </summary>
    public static class LLMVarietyIntegration
    {
        private static readonly Dictionary<string, IQuestIntegrationStrategy> s_IntegrationStrategies;
        private static readonly object s_Lock = new object();
        private static bool s_Initialized = false;
        private static int s_IntegrationCount = 0;
        private static int s_QualityImprovements = 0;
        private static int s_VarietyEnhancements = 0;

        static LLMVarietyIntegration()
        {
            s_IntegrationStrategies = new Dictionary<string, IQuestIntegrationStrategy>();
        }

        /// <summary>
        /// Initialize the LLM-Variety integration system
        /// </summary>
        public static void Initialize()
        {
            if (s_Initialized)
                return;

            s_Initialized = true;

            // Register built-in integration strategies
            RegisterIntegrationStrategy("automatic", new AutomaticIntegrationStrategy());
            RegisterIntegrationStrategy("manual", new ManualIntegrationStrategy());
            RegisterIntegrationStrategy("quality", new QualityIntegrationStrategy());
            RegisterIntegrationStrategy("variety", new VarietyIntegrationStrategy());

            Console.WriteLine("[LLMVarietyIntegration] Initialized LLM-Variety integration system");
            Console.WriteLine($"[LLMVarietyIntegration] Registered {s_IntegrationStrategies.Count} integration strategies");
        }

        /// <summary>
        /// Process LLM-generated quest through variety system
        /// </summary>
        public static IntegrationResult ProcessGeneratedQuest(UnifiedQuestData quest, IntegrationContext context = null)
        {
            if (quest == null)
                return new IntegrationResult { Success = false, Error = "Quest data is null" };

            lock (s_Lock)
            {
                try
                {
                    s_IntegrationCount++;

                    // Use default context if none provided
                    context = context ?? new IntegrationContext();

                    // Select integration strategy
                    var strategy = SelectIntegrationStrategy(quest, context);
                    if (strategy == null)
                    {
                        return new IntegrationResult { Success = false, Error = "No integration strategy available" };
                    }

                    // Process the quest
                    var result = strategy.ProcessQuest(quest, context);

                    // Update statistics
                    if (result.Success)
                    {
                        if (result.QualityImproved)
                            s_QualityImprovements++;
                        if (result.VarietyEnhanced)
                            s_VarietyEnhancements++;
                    }

                    return result;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[LLMVarietyIntegration] Error processing quest {quest.QuestId}: {ex.Message}");
                    return new IntegrationResult { Success = false, Error = ex.Message };
                }
            }
        }

        /// <summary>
        /// Enhance quest generation with variety system feedback
        /// </summary>
        public static QuestGenerationFeedback GetGenerationFeedback(UnifiedQuestData quest, GenerationContext context)
        {
            lock (s_Lock)
            {
                try
                {
                    var feedback = new QuestGenerationFeedback();

                    // Analyze quest variety
                    var varietyAnalysis = AnalyzeQuestVariety(quest);
                    feedback.VarietyScore = varietyAnalysis.OverallScore;
                    feedback.SimilarityAlerts = varietyAnalysis.SimilarityAlerts;
                    feedback.Recommendations = varietyAnalysis.Recommendations;

                    // Analyze quest quality
                    var qualityAnalysis = AnalyzeQuestQuality(quest);
                    feedback.QualityScore = qualityAnalysis.OverallScore;
                    feedback.QualityIssues = qualityAnalysis.Issues;
                    feedback.ImprovementSuggestions = qualityAnalysis.Suggestions;

                    // Generate integration suggestions
                    feedback.IntegrationSuggestions = GenerateIntegrationSuggestions(quest, context);

                    return feedback;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[LLMVarietyIntegration] Error generating feedback: {ex.Message}");
                    return new QuestGenerationFeedback();
                }
            }
        }

        /// <summary>
        /// Register an integration strategy
        /// </summary>
        public static void RegisterIntegrationStrategy(string strategyName, IQuestIntegrationStrategy strategy)
        {
            lock (s_Lock)
            {
                s_IntegrationStrategies[strategyName] = strategy;
                Console.WriteLine($"[LLMVarietyIntegration] Registered integration strategy: {strategyName}");
            }
        }

        /// <summary>
        /// Get integration statistics
        /// </summary>
        public static IntegrationStatistics GetStatistics()
        {
            lock (s_Lock)
            {
                return new IntegrationStatistics
                {
                    TotalIntegrations = s_IntegrationCount,
                    QualityImprovements = s_QualityImprovements,
                    VarietyEnhancements = s_VarietyEnhancements,
                    SuccessRate = s_IntegrationCount > 0 ? (double)(s_QualityImprovements + s_VarietyEnhancements) / s_IntegrationCount : 0.0,
                    RegisteredStrategies = s_IntegrationStrategies.Count,
                    LastIntegration = DateTime.UtcNow
                };
            }
        }

        /// <summary>
        /// Reset integration statistics
        /// </summary>
        public static void ResetStatistics()
        {
            lock (s_Lock)
            {
                s_IntegrationCount = 0;
                s_QualityImprovements = 0;
                s_VarietyEnhancements = 0;
                Console.WriteLine("[LLMVarietyIntegration] Statistics reset");
            }
        }

        /// <summary>
        /// Select appropriate integration strategy
        /// </summary>
        private static IQuestIntegrationStrategy SelectIntegrationStrategy(UnifiedQuestData quest, IntegrationContext context)
        {
            // Use specified strategy if provided
            if (!string.IsNullOrEmpty(context.StrategyName) && s_IntegrationStrategies.TryGetValue(context.StrategyName, out var specifiedStrategy))
            {
                return specifiedStrategy;
            }

            // Auto-select based on quest type and context
            switch (quest.Type)
            {
                case QuestType.Dynamic:
                return s_IntegrationStrategies.GetValueOrDefault("automatic", s_IntegrationStrategies.GetValueOrDefault("quality"));
                
                case QuestType.Vystia:
                    return s_IntegrationStrategies.GetValueOrDefault("variety", s_IntegrationStrategies.GetValueOrDefault("quality"));
                
                default:
                    return s_IntegrationStrategies.GetValueOrDefault("manual", s_IntegrationStrategies.GetValueOrDefault("automatic"));
            }
        }

        /// <summary>
        /// Analyze quest variety
        /// </summary>
        private static VarietyAnalysis AnalyzeQuestVariety(UnifiedQuestData quest)
        {
            var analysis = new VarietyAnalysis();

            // Check similarity to existing quests
            var similarityScore = CalculateSimilarityScore(quest);
            analysis.OverallScore = similarityScore;
            analysis.SimilarityAlerts = similarityScore > 0.8 ? 
                new List<string> { $"Quest is {similarityScore:P1} similar to existing quests" } : 
                new List<string>();

            // Check theme variety
            var themeScore = CalculateThemeVariety(quest);
            analysis.ThemeScore = themeScore;

            // Check objective variety
            var objectiveScore = CalculateObjectiveVariety(quest);
            analysis.ObjectiveScore = objectiveScore;

            // Generate recommendations
            analysis.Recommendations = GenerateVarietyRecommendations(quest, analysis);

            return analysis;
        }

        /// <summary>
        /// Analyze quest quality
        /// </summary>
        private static QualityAnalysis AnalyzeQuestQuality(UnifiedQuestData quest)
        {
            var analysis = new QualityAnalysis();

            // Check structural quality
            var structuralScore = CalculateStructuralQuality(quest);
            analysis.StructuralScore = structuralScore;

            // Check content quality
            var contentScore = CalculateContentQuality(quest);
            analysis.ContentScore = contentScore;

            // Check coherence
            var coherenceScore = CalculateCoherence(quest);
            analysis.CoherenceScore = coherenceScore;

            // Calculate overall score
            analysis.OverallScore = (structuralScore + contentScore + coherenceScore) / 3.0;

            // Identify issues
            analysis.Issues = IdentifyQualityIssues(quest);

            // Generate suggestions
            analysis.Suggestions = GenerateQualitySuggestions(quest, analysis);

            return analysis;
        }

        /// <summary>
        /// Generate integration suggestions
        /// </summary>
        private static List<string> GenerateIntegrationSuggestions(UnifiedQuestData quest, GenerationContext context)
        {
            var suggestions = new List<string>();

            // Analyze quest characteristics
            if (quest.Description?.Length < 100)
            {
                suggestions.Add("Consider expanding quest description for better variety");
            }

            if (quest.CooperativeObjectives?.Count == 0)
            {
                suggestions.Add("Add cooperative objectives for multiplayer engagement");
            }

            if (string.IsNullOrEmpty(quest.Theme))
            {
                suggestions.Add("Assign a theme for better quest categorization");
            }

            // Check for improvement opportunities
            if (quest.QualityData?.OverallScore < 0.5)
            {
                suggestions.Add("Review quest quality metrics for improvement");
            }

            if (quest.SimilarityData?.SimilarityScore > 0.7)
            {
                suggestions.Add("Modify quest to increase variety and reduce similarity");
            }

            return suggestions;
        }

        /// <summary>
        /// Calculate similarity score
        /// </summary>
        private static double CalculateSimilarityScore(UnifiedQuestData quest)
        {
            // This would integrate with QuestSimilarityDetector
            // For now, return a placeholder score
            if (quest.SimilarityData != null)
            {
                return quest.SimilarityData.SimilarityScore;
            }

            // Calculate based on title and description
            var titleWords = quest.Title?.Split(' ').Length ?? 0;
            var descriptionWords = quest.Description?.Split(' ').Length ?? 0;
            
            // Simple heuristic: longer descriptions are more likely to be similar
            var complexity = (titleWords + descriptionWords) / 100.0;
            return Math.Min(complexity, 1.0);
        }

        /// <summary>
        /// Calculate theme variety score
        /// </summary>
        private static double CalculateThemeVariety(UnifiedQuestData quest)
        {
            if (string.IsNullOrEmpty(quest.Theme))
                return 0.0;

            // Check theme frequency (would integrate with QuestThemeRotation)
            var commonThemes = new[] { "combat", "exploration", "collection", "rescue", "delivery" };
            var isCommonTheme = commonThemes.Contains(quest.Theme.ToLower());

            return isCommonTheme ? 0.3 : 0.8; // Higher score for unique themes
        }

        /// <summary>
        /// Calculate objective variety score
        /// </summary>
        private static double CalculateObjectiveVariety(UnifiedQuestData quest)
        {
            if (quest.CooperativeObjectives == null || quest.CooperativeObjectives.Count == 0)
                return 0.0;

            var objectiveTypes = quest.CooperativeObjectives.Select(o => o.Type).Distinct().Count();
            var maxTypes = Enum.GetValues(typeof(CooperativeObjectiveType)).Length;

            return (double)objectiveTypes / maxTypes;
        }

        /// <summary>
        /// Calculate structural quality score
        /// </summary>
        private static double CalculateStructuralQuality(UnifiedQuestData quest)
        {
            var score = 0.0;

            // Check for essential components
            if (!string.IsNullOrEmpty(quest.Title))
                score += 0.3;
            if (!string.IsNullOrEmpty(quest.Description))
                score += 0.3;
            if (quest.Owner != null)
                score += 0.2;
            if (quest.Creator != null)
                score += 0.2;

            return Math.Min(score, 1.0);
        }

        /// <summary>
        /// Calculate content quality score
        /// </summary>
        private static double CalculateContentQuality(UnifiedQuestData quest)
        {
            var score = 0.0;

            // Check description quality
            if (quest.Description?.Length > 50)
                score += 0.3;
            if (quest.Description?.Length > 200)
                score += 0.2;

            // Check objectives
            if (quest.CooperativeObjectives?.Count > 0)
                score += 0.3;
            if (quest.CooperativeObjectives?.All(o => !string.IsNullOrEmpty(o.Description)))
                score += 0.2;

            return Math.Min(score, 1.0);
        }

        /// <summary>
        /// Calculate coherence score
        /// </summary>
        private static double CalculateCoherence(UnifiedQuestData quest)
        {
            var score = 0.0;

            // Check title-description coherence
            if (quest.Title != null && quest.Description != null)
            {
                var titleWords = quest.Title.ToLower().Split(' ');
                var descriptionWords = quest.Description.ToLower().Split(' ');
                var commonWords = titleWords.Intersect(descriptionWords).Count;
                var totalWords = Math.Max(titleWords.Length, descriptionWords.Length);
                
                if (totalWords > 0)
                {
                    score = (double)commonWords / totalWords;
                }
            }

            return Math.Min(score, 0.5); // Cap at 0.5 for coherence
        }

        /// <summary>
        /// Identify quality issues
        /// </summary>
        private static List<string> IdentifyQualityIssues(UnifiedQuestData quest)
        {
            var issues = new List<string>();

            if (string.IsNullOrEmpty(quest.Title))
                issues.Add("Missing quest title");
            if (string.IsNullOrEmpty(quest.Description))
                issues.Add("Missing quest description");
            if (quest.Owner == null)
                issues.Add("Missing quest owner");
            if (quest.CooperativeObjectives?.Count == 0)
                issues.Add("No objectives defined");
            if (quest.QuestId <= 0)
                issues.Add("Invalid quest ID");

            return issues;
        }

        /// <summary>
        /// Generate quality suggestions
        /// </summary>
        private static List<string> GenerateQualitySuggestions(UnifiedQuestData quest, QualityAnalysis analysis)
        {
            var suggestions = new List<string>();

            if (analysis.StructuralScore < 0.5)
                suggestions.Add("Improve quest structure with title, description, and objectives");
            if (analysis.ContentScore < 0.5)
                suggestions.Add("Enhance content quality with more detailed descriptions");
            if (analysis.CoherenceScore < 0.3)
                suggestions.Add("Improve coherence between title and description");

            return suggestions;
        }

        /// <summary>
        /// Generate variety recommendations
        /// </summary>
        private static List<string> GenerateVarietyRecommendations(UnifiedQuestData quest, VarietyAnalysis analysis)
        {
            var recommendations = new List<string>();

            if (analysis.SimilarityAlerts.Count > 0)
            {
                recommendations.AddRange(analysis.SimilarityAlerts);
                recommendations.Add("Consider modifying quest elements to increase uniqueness");
            }

            if (analysis.ThemeScore < 0.5)
            {
                recommendations.Add("Consider using a more unique theme for better variety");
            }

            if (analysis.ObjectiveScore < 0.5)
            {
                recommendations.Add("Add more diverse objective types for better engagement");
            }

            return recommendations;
        }
    }

    /// <summary>
    /// Integration result
    /// </summary>
    public class IntegrationResult
    {
        public bool Success { get; set; }
        public string Error { get; set; }
        public List<string> Messages { get; set; }
        public bool QualityImproved { get; set; }
        public bool VarietyEnhanced { get; set; }
        public UnifiedQuestData EnhancedQuest { get; set; }
        public Dictionary<string, object> Metadata { get; set; }

        public IntegrationResult()
        {
            Messages = new List<string>();
            Metadata = new Dictionary<string, object>();
        }
    }

    /// <summary>
    /// Quest generation feedback
    /// </summary>
    public class QuestGenerationFeedback
    {
        public double VarietyScore { get; set; }
        public double QualityScore { get; set; }
        public List<string> SimilarityAlerts { get; set; }
        public List<string> QualityIssues { get; set; }
        public List<string> ImprovementSuggestions { get; set; }
        public List<string> IntegrationSuggestions { get; set; }

        public QuestGenerationFeedback()
        {
            SimilarityAlerts = new List<string>();
            QualityIssues = new List<string>();
            ImprovementSuggestions = new List<string>();
            IntegrationSuggestions = new List<string>();
        }
    }

    /// <summary>
    /// Integration context
    /// </summary>
    public class IntegrationContext
    {
        public string StrategyName { get; set; }
        public PlayerMobile Requester { get; set; }
        public Party Party { get; set; }
        public Dictionary<string, object> Parameters { get; set; }
        public bool AutoEnhance { get; set; }
        public bool ForceIntegration { get; set; }
        public IntegrationPriority Priority { get; set; }

        public IntegrationContext()
        {
            Parameters = new Dictionary<string, object>();
            AutoEnhance = true;
            ForceIntegration = false;
            Priority = IntegrationPriority.Normal;
        }
    }

    /// <summary>
    /// Integration priority levels
    /// </summary>
    public enum IntegrationPriority
    {
        Low,
        Normal,
        High,
        Critical
    }

    /// <summary>
    /// Variety analysis results
    /// </summary>
    public class VarietyAnalysis
    {
        public double OverallScore { get; set; }
        public double SimilarityScore { get; set; }
        public double ThemeScore { get; set; }
        public double ObjectiveScore { get; set; }
        public List<string> SimilarityAlerts { get; set; }
        public List<string> Recommendations { get; set; }

        public VarietyAnalysis()
        {
            SimilarityAlerts = new List<string>();
            Recommendations = new List<string>();
        }
    }

    /// <summary>
    /// Quality analysis results
    /// </summary>
    public class QualityAnalysis
    {
        public double OverallScore { get; set; }
        public double StructuralScore { get; set; }
        public double ContentScore { get; set; }
        public double CoherenceScore { get; set; }
        public List<string> Issues { get; set; }
        public List<string> Suggestions { get; set; }

        public QualityAnalysis()
        {
            Issues = new List<string>();
            Suggestions = new List<string>();
        }
    }

    /// <summary>
    /// Integration statistics
    /// </summary>
    public class IntegrationStatistics
    {
        public int TotalIntegrations { get; set; }
        public int QualityImprovements { get; set; }
        public int VarietyEnhancements { get; set; }
        public double SuccessRate { get; set; }
        public int RegisteredStrategies { get; set; }
        public DateTime LastIntegration { get; set; }
    }

    /// <summary>
    /// Interface for integration strategies
    /// </summary>
    public interface IQuestIntegrationStrategy
    {
        IntegrationResult ProcessQuest(UnifiedQuestData quest, IntegrationContext context);
        string StrategyName { get; }
        bool SupportsQuestType(QuestType questType);
        IntegrationPriority Priority { get; }
    }

    /// <summary>
    /// Generation context for quest generation
    /// </summary>
    public class GenerationContext
    {
        public string Theme { get; set; }
        public PlayerMobile Player { get; set; }
        public Party Party { get; set; }
        public Dictionary<string, object> Parameters { get; set; }
        public bool ForceVariety { get; set; }
        public bool ForceQuality { get; set; }

        public GenerationContext()
        {
            Parameters = new Dictionary<string, object>();
        }
    }

    /// <summary>
    /// Extension methods for LLM-Variety integration
    /// </summary>
    public static class LLMVarietyExtensions
    {
        /// <summary>
        Process LLM-generated quest through variety system
        /// </summary>
        public static IntegrationResult ProcessThroughVariety(this UnifiedQuestData quest, IntegrationContext context = null)
        {
            return LLMVarietyIntegration.ProcessGeneratedQuest(quest, context);
        }

        /// <summary>
        Get variety feedback for quest generation
        /// </summary>
        public static QuestGenerationFeedback GetVarietyFeedback(this UnifiedQuestData quest, GenerationContext context = null)
        {
            return LLMVarietyIntegration.GetGenerationFeedback(quest, context);
        }

        /// <summary>
        Check if quest needs variety enhancement
        /// </summary>
        public static bool NeedsVarietyEnhancement(this UnifiedQuestData quest)
        {
            var feedback = quest.GetVarietyFeedback(null);
            return feedback.VarietyScore < 0.5 || feedback.SimilarityAlerts.Count > 0;
        }

        /// <summary>
        Check if quest needs quality improvement
        /// </summary>
        public static bool NeedsQualityImprovement(this UnifiedQuestData quest)
        {
            var feedback = quest.GetVarietyFeedback(null);
            return feedback.QualityScore < 0.5 || feedback.QualityIssues.Count > 0;
        }
    }
}
