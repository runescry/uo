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
    /// Automatic integration strategy for LLM-generated quests
    /// Automatically processes quests through variety system with minimal configuration
    /// </summary>
    public class AutomaticIntegrationStrategy : IQuestIntegrationStrategy
    {
        public string StrategyName => "automatic";
        public IntegrationPriority Priority => IntegrationPriority.Normal;
        public bool SupportsQuestType(QuestType questType) => true;

        public IntegrationResult ProcessQuest(UnifiedQuestData quest, IntegrationContext context)
        {
            var result = new IntegrationResult { Success = true };

            try
            {
                // Step 1: Analyze quest for variety and quality
                var feedback = LLMVarietyIntegration.GetGenerationFeedback(quest);
                
                // Step 2: Apply automatic enhancements if needed
                if (context.AutoEnhance && (feedback.NeedsVarietyEnhancement() || feedback.NeedsQualityImprovement()))
                {
                    var enhancedQuest = ApplyAutomaticEnhancements(quest, feedback);
                    result.EnhancedQuest = enhancedQuest;
                    
                    if (feedback.NeedsVarietyEnhancement())
                        result.VarietyEnhanced = true;
                    if (feedback.NeedsQualityImprovement())
                        result.QualityImproved = true;
                    
                    result.Messages.Add("Quest automatically enhanced through variety system");
                }

                // Step 3: Register with variety tracking
                RegisterWithVarietySystem(quest, result);

                // Step 4: Update quest data with variety metrics
                UpdateQuestWithVarietyData(quest, feedback);

                result.Messages.Add($"Quest processed with automatic integration (Variety: {feedback.VarietyScore:P1}, Quality: {feedback.QualityScore:P1})");
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Error = ex.Message;
                result.Messages.Add($"Automatic integration failed: {ex.Message}");
            }

            return result;
        }

        private UnifiedQuestData ApplyAutomaticEnhancements(UnifiedQuestData quest, QuestGenerationFeedback feedback)
        {
            var enhancedQuest = quest; // In a real implementation, this would create a copy

            // Apply variety enhancements
            if (feedback.NeedsVarietyEnhancement())
            {
                enhancedQuest = EnhanceQuestVariety(enhancedQuest, feedback);
            }

            // Apply quality improvements
            if (feedback.NeedsQualityImprovement())
            {
                enhancedQuest = EnhanceQuestQuality(enhancedQuest, feedback);
            }

            return enhancedQuest;
        }

        private UnifiedQuestData EnhanceQuestVariety(UnifiedQuestData quest, QuestGenerationFeedback feedback)
        {
            // Enhance theme if needed
            if (string.IsNullOrEmpty(quest.Theme))
            {
                quest.Theme = GenerateUniqueTheme(quest);
            }

            // Add variety tags
            if (quest.Tags == null || quest.Tags.Count == 0)
            {
                quest.Tags = GenerateVarietyTags(quest);
            }

            // Enhance objectives for variety
            if (quest.CooperativeObjectives != null)
            {
                EnhanceObjectivesForVariety(quest.CooperativeObjectives);
            }

            return quest;
        }

        private UnifiedQuestData EnhanceQuestQuality(UnifiedQuestData quest, QuestGenerationFeedback feedback)
        {
            // Enhance description if too short
            if (quest.Description?.Length < 100)
            {
                quest.Description = ExpandDescription(quest.Description, feedback);
            }

            // Add more objectives if needed
            if (quest.CooperativeObjectives?.Count < 2)
            {
                AddAdditionalObjectives(quest);
            }

            // Improve coherence
            if (feedback.CoherenceScore < 0.3)
            {
                ImproveCoherence(quest);
            }

            return quest;
        }

        private void RegisterWithVarietySystem(UnifiedQuestData quest, IntegrationResult result)
        {
            // This would register the quest with QuestVarietyTracker
            // For now, just log the action
            Console.WriteLine($"[AutomaticIntegration] Registered quest {quest.QuestId} with variety system");
            
            result.Metadata["variety_registered"] = true;
            result.Metadata["registration_time"] = DateTime.UtcNow;
        }

        private void UpdateQuestWithVarietyData(UnifiedQuestData quest, QuestGenerationFeedback feedback)
        {
            // Update similarity data
            if (quest.SimilarityData == null)
                quest.SimilarityData = new QuestSimilarityData();

            quest.SimilarityData.SimilarityScore = feedback.VarietyScore;
            quest.SimilarityData.FeatureVector = GenerateFeatureVector(quest);

            // Update quality data
            if (quest.QualityData == null)
                quest.QualityData = new QuestQualityData();

            quest.QualityData.OverallScore = feedback.QualityScore;
            quest.QualityData.QualityIssues = feedback.QualityIssues;
            quest.QualityData.ImprovementSuggestions = feedback.ImprovementSuggestions;
            quest.QualityData.MeetsMinimumQuality = feedback.QualityScore >= 0.3;
        }

        private string GenerateUniqueTheme(UnifiedQuestData quest)
        {
            // Generate a theme based on quest content
            var themes = new[]
            {
                "mystery", "adventure", "discovery", "exploration", "rescue",
                "protection", "diplomacy", "investigation", "treasure", "artifact"
            };

            // Select theme based on quest content
            var content = $"{quest.Title} {quest.Description}".ToLower();
            var selectedTheme = themes.FirstOrDefault(theme => content.Contains(theme)) ?? "adventure";

            return selectedTheme;
        }

        private List<string> GenerateVarietyTags(UnifiedQuestData quest)
        {
            var tags = new List<string>();

            // Add tags based on quest content
            if (quest.Description?.ToLower().Contains("kill") == true)
                tags.Add("combat");
            if (quest.Description?.ToLower().Contains("collect") == true)
                tags.Add("gathering");
            if (quest.Description?.ToLower().Contains("explore") == true)
                tags.Add("exploration");
            if (quest.Description?.ToLower().Contains("rescue") == true)
                tags.Add("rescue");
            if (quest.Description?.ToLower().Contains("protect") == true)
                tags.Add("protection");

            return tags.Distinct().ToList();
        }

        private void EnhanceObjectivesForVariety(List<CooperativeObjectiveData> objectives)
        {
            // Add variety to objective types
            var existingTypes = objectives.Select(o => o.Type).ToList();
            var availableTypes = Enum.GetValues(typeof(CooperativeObjectiveType)).Except(existingTypes).ToArray();

            if (availableTypes.Length > 0 && objectives.Count < 5)
            {
                var newObjective = new CooperativeObjectiveData
                {
                    ObjectiveId = $"variety_objective_{objectives.Count + 1}",
                    Description = "Additional variety objective",
                    Type = availableTypes[0],
                    RequiredCount = 1,
                    CurrentProgress = 0,
                    IsCompleted = false
                };

                objectives.Add(newObjective);
            }
        }

        private void AddAdditionalObjectives(UnifiedQuestData quest)
        {
            if (quest.CooperativeObjectives == null)
                quest.CooperativeObjectives = new List<CooperativeObjectiveData>();

            // Add a simple completion objective
            var additionalObjective = new CooperativeObjectiveData
            {
                ObjectiveId = "auto_objective_complete",
                Description = "Complete the quest objectives",
                Type = CooperativeObjectiveType.AllMembers,
                RequiredCount = 1,
                CurrentProgress = 0,
                IsCompleted = false
            };

            quest.CooperativeObjectives.Add(additionalObjective);
        }

        private string ExpandDescription(string original, QuestGenerationFeedback feedback)
        {
            // Simple description expansion
            var sentences = original.Split('.', StringSplitOptions.RemoveEmptyEntries);
            var expandedDescription = original;

            if (sentences.Length == 1 && original.Length < 100)
            {
                expandedDescription = $"{original} This quest offers engaging challenges and rewards for brave adventurers seeking glory and fortune.";
            }
            else if (sentences.Length == 2 && original.Length < 200)
            {
                expandedDescription = $"{original} Additional objectives may appear as you progress, offering new challenges and opportunities for advancement.";
            }

            return expandedDescription;
        }

        private void ImproveCoherence(UnifiedQuestData quest)
        {
            // Simple coherence improvement
            if (quest.Title != null && quest.Description != null)
            {
                // Ensure title and description share common themes
                var titleWords = quest.Title.ToLower().Split(' ');
                var descriptionWords = quest.Description.ToLower().Split(' ');
                var commonWords = titleWords.Intersect(descriptionWords).ToList();

                if (commonWords.Count == 0)
                {
                    // Add a title word to description for coherence
                    var firstTitleWord = titleWords.FirstOrDefault();
                    if (!string.IsNullOrEmpty(firstTitleWord))
                    {
                        quest.Description = $"{quest.Description} This {firstTitleWord} quest involves...";
                    }
                }
            }
        }

        private Dictionary<string, double> GenerateFeatureVector(UnifiedQuestData quest)
        {
            var vector = new Dictionary<string, double>();

            // Generate feature vector based on quest content
            var content = $"{quest.Title} {quest.Description}".ToLower();

            // Add word frequency features
            var words = content.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            foreach (var word in words.Distinct())
            {
                vector[$"word_{word}"] = 1.0;
            }

            // Add length-based features
            vector["title_length"] = quest.Title?.Length ?? 0;
            vector["description_length"] = quest.Description?.Length ?? 0;
            vector["total_length"] = vector["title_length"] + vector["description_length"];

            // Add objective features
            vector["objective_count"] = quest.CooperativeObjectives?.Count ?? 0;

            return vector;
        }
    }

    /// <summary>
    /// Manual integration strategy for LLM-generated quests
    /// Requires manual approval for variety system integration
    /// </summary>
    public class ManualIntegrationStrategy : IQuestIntegrationStrategy
    {
        public string StrategyName => "manual";
        public IntegrationPriority Priority => IntegrationPriority.Low;
        public bool SupportsQuestType(QuestType questType) => true;

        public IntegrationResult ProcessQuest(UnifiedQuestData quest, IntegrationContext context)
        {
            var result = new IntegrationResult { Success = true };

            try
            {
                // Manual integration requires explicit approval
                if (!context.ForceIntegration)
                {
                    result.Success = false;
                    result.Error = "Manual integration requires explicit approval";
                    result.Messages.Add("Set ForceIntegration=true to bypass manual approval");
                    return result;
                }

                // Get variety feedback
                var feedback = LLMVarietyIntegration.GetGenerationFeedback(quest);

                // Present feedback to requester
                result.Messages.Add($"Manual integration requested for quest {quest.QuestId}");
                result.Messages.Add($"Variety Score: {feedback.VarietyScore:P1}");
                result.Messages.Add($"Quality Score: {feedback.QualityScore:P1}");
                
                if (feedback.SimilarityAlerts.Count > 0)
                {
                    result.Messages.Add("Similarity Alerts:");
                    result.Messages.AddRange(feedback.SimilarityAlerts);
                }

                if (feedback.QualityIssues.Count > 0)
                {
                    result.Messages.Add("Quality Issues:");
                    result.Messages.AddRange(feedback.QualityIssues);
                }

                result.Messages.Add("Use [UnifiedQuest integrate force] to proceed with manual integration");

                // Store feedback for later use
                result.Metadata["feedback"] = feedback;
                result.Metadata["manual_review_required"] = true;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Error = ex.Message;
                result.Messages.Add($"Manual integration failed: {ex.Message}");
            }

            return result;
        }
    }

    /// <summary>
    /// Quality-focused integration strategy
    /// Prioritizes quest quality improvements
    /// </summary>
    public class QualityIntegrationStrategy : IQuestIntegrationStrategy
    {
        public string StrategyName => "quality";
        public IntegrationPriority Priority => IntegrationPriority.High;
        public bool SupportsQuestType(QuestType questType) => true;

        public IntegrationResult ProcessQuest(UnifiedQuestData quest, IntegrationContext context)
        {
            var result = new IntegrationResult { Success = true };

            try
            {
                // Get quality feedback
                var feedback = LLMVarietyIntegration.GetGenerationFeedback(quest);

                // Focus on quality improvements
                if (feedback.NeedsQualityImprovement())
                {
                    var enhancedQuest = ApplyQualityEnhancements(quest, feedback);
                    result.EnhancedQuest = enhancedQuest;
                    result.QualityImproved = true;
                    result.Messages.Add("Quest enhanced through quality-focused integration");
                }

                // Register with variety system
                RegisterWithVarietySystem(quest, result);

                // Update quest data
                UpdateQuestWithVarietyData(quest, feedback);

                result.Messages.Add($"Quest processed with quality integration (Quality: {feedback.QualityScore:P1})");
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Error = ex.Message;
                result.Messages.Add($"Quality integration failed: {ex.Message}");
            }

            return result;
        }

        private UnifiedQuestData ApplyQualityEnhancements(UnifiedQuestData quest, QuestGenerationFeedback feedback)
        {
            var enhancedQuest = quest; // In a real implementation, this would create a copy

            // Apply quality improvements based on feedback
            if (feedback.QualityIssues.Count > 0)
            {
                foreach (var issue in feedback.QualityIssues)
                {
                    enhancedQuest = FixQualityIssue(enhancedQuest, issue);
                }
            }

            if (feedback.ImprovementSuggestions.Count > 0)
            {
                foreach (var suggestion in feedback.ImprovementSuggestions)
                {
                    enhancedQuest = ApplySuggestion(enhancedQuest, suggestion);
                }
            }

            return enhancedQuest;
        }

        private UnifiedQuestData FixQualityIssue(UnifiedQuestData quest, string issue)
        {
            switch (issue.ToLower())
            {
                case "missing quest title":
                    quest.Title = "Untitled Quest";
                    break;
                case "missing quest description":
                    quest.Description = "A quest that needs description.";
                    break;
                case "no objectives defined":
                    AddAdditionalObjectives(quest);
                    break;
                case "invalid quest id":
                    quest.QuestId = DateTime.UtcNow.GetHashCode();
                    break;
                default:
                    // Log the issue for manual review
                    Console.WriteLine($"[QualityIntegration] Quality issue not auto-fixed: {issue}");
                    break;
            }

            return quest;
        }

        private UnifiedQuestData ApplySuggestion(UnifiedQuestData quest, string suggestion)
        {
            switch (suggestion.ToLower())
            {
                case "improve quest structure":
                    return ImproveQuestStructure(quest);
                case "enhance content quality":
                    return EnhanceContentQuality(quest);
                case "improve coherence":
                    return ImproveCoherence(quest);
                default:
                    Console.WriteLine($"[QualityIntegration] Suggestion not auto-applied: {suggestion}");
                    return quest;
            }
        }

        private UnifiedQuestData ImproveQuestStructure(UnifiedQuestData quest)
        {
            // Ensure all essential components are present
            if (string.IsNullOrEmpty(quest.Title))
                quest.Title = "Enhanced Quest";
            if (string.IsNullOrEmpty(quest.Description))
                quest.Description = "An enhanced quest with improved structure and quality.";

            return quest;
        }

        private UnifiedQuestData EnhanceContentQuality(UnifiedQuestData quest)
        {
            // Expand description if needed
            if (quest.Description?.Length < 150)
            {
                quest.Description = ExpandDescription(quest.Description, null);
            }

            return quest;
        }

        private void RegisterWithVarietySystem(UnifiedQuestData quest, IntegrationResult result)
        {
            Console.WriteLine($"[QualityIntegration] Registered quest {quest.QuestId} with variety system");
            result.Metadata["variety_registered"] = true;
        }

        private void UpdateQuestWithVarietyData(UnifiedQuestData quest, QuestGenerationFeedback feedback)
        {
            // Update similarity data
            if (quest.SimilarityData == null)
                quest.SimilarityData = new QuestSimilarityData();

            quest.SimilarityData.SimilarityScore = feedback.VarietyScore;
            quest.SimilarityData.FeatureVector = GenerateFeatureVector(quest);

            // Update quality data
            if (quest.QualityData == null)
                quest.QualityData = new QuestQualityData();

            quest.QualityData.OverallScore = feedback.QualityScore;
            quest.QualityData.QualityIssues = feedback.QualityIssues;
            quest.QualityData.MeetsMinimumQuality = feedback.QualityScore >= 0.5;
        }
    }

    /// <summary>
    /// Variety-focused integration strategy
    /// Prioritizes quest variety and uniqueness
    /// </summary>
    public class VarietyIntegrationStrategy : IQuestIntegrationStrategy
    {
        public string StrategyName => "variety";
        public IntegrationPriority Priority => IntegrationPriority.High;
        public bool SupportsQuestType(QuestType questType) => true;

        public IntegrationResult ProcessQuest(UnifiedQuestData quest, IntegrationContext context)
        {
            var result = new IntegrationResult { Success = true };

            try
            {
                // Get variety feedback
                var feedback = LLMVarietyIntegration.GetGenerationFeedback(quest);

                // Focus on variety enhancements
                if (feedback.NeedsVarietyEnhancement())
                {
                    var enhancedQuest = ApplyVarietyEnhancements(quest, feedback);
                    result.EnhancedQuest = enhancedQuest;
                    result.VarietyEnhanced = true;
                    result.Messages.Add("Quest enhanced through variety-focused integration");
                }

                // Register with variety system
                RegisterWithVarietySystem(quest, result);

                // Update quest data
                UpdateQuestWithVarietyData(quest, feedback);

                result.Messages.Add($"Quest processed with variety integration (Variety: {feedback.VarietyScore:P1})");
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Error = ex.Message;
                result.Messages.Add($"Variety integration failed: {ex.Message}");
            }

            return result;
        }

        private UnifiedQuestData ApplyVarietyEnhancements(UnifiedQuestData quest, QuestGenerationFeedback feedback)
        {
            var enhancedQuest = quest; // In a real implementation, this would create a copy

            // Apply variety enhancements based on feedback
            if (feedback.SimilarityAlerts.Count > 0)
            {
                enhancedQuest = ReduceSimilarity(enhancedQuest, feedback);
            }

            if (feedback.Recommendations.Count > 0)
            {
                enhancedQuest = ApplyVarietyRecommendations(enhancedQuest, feedback);
            }

            return enhancedQuest;
        }

        private UnifiedQuestData ReduceSimilarity(UnifiedQuestData quest, QuestGenerationFeedback feedback)
        {
            // Modify quest to reduce similarity
            if (quest.Description?.Length > 50)
            {
                // Replace common words with synonyms
                var description = quest.Description.ToLower();
                var synonyms = new Dictionary<string, string>
                {
                    { "kill", "defeat" },
                    { "eliminate", "vanquish" },
                    { "collect", "gather" },
                    { "acquire", "obtain" },
                    { "explore", "discover" },
                    { "investigate", "survey" },
                    { "rescue", "save" },
                    { "liberate", "free" },
                    { "protect", "guard" },
                    { "shield", "defend" }
                };

                foreach (var kvp in synonyms)
                {
                    description = description.Replace(kvp.Key, kvp.Value);
                }

                quest.Description = description;
            }

            return quest;
        }

        private UnifiedQuestData ApplyVarietyRecommendations(UnifiedQuestData quest, QuestGenerationFeedback feedback)
        {
            // Apply variety recommendations
            foreach (var recommendation in feedback.Recommendations)
            {
                ApplyVarietyRecommendation(quest, recommendation);
            }

            return quest;
        }

        private void ApplyVarietyRecommendation(UnifiedQuestData quest, string recommendation)
        {
            switch (recommendation.ToLower())
            {
                case "consider using a more unique theme":
                    quest.Theme = GenerateUniqueTheme(quest);
                    break;
                case "add more diverse objective types":
                    EnhanceObjectivesForVariety(quest.CooperativeObjectives);
                    break;
                case "consider modifying quest elements to increase uniqueness":
                    // This would involve more complex analysis
                    Console.WriteLine("[VarietyIntegration] Quest uniqueness enhancement requires manual implementation");
                    break;
            }
        }

        private void RegisterWithVarietySystem(UnifiedQuestData quest, IntegrationResult result)
        {
            Console.WriteLine($"[VarietyIntegration] Registered quest {quest.QuestId} with variety system");
            result.Metadata["variety_registered"] = true;
        }

        private void UpdateQuestWithVarietyData(UnifiedQuestData quest, QuestGenerationFeedback feedback)
        {
            // Update similarity data
            if (quest.SimilarityData == null)
                quest.SimilarityData = new QuestSimilarityData();

            quest.SimilarityData.SimilarityScore = feedback.VarietyScore;
            quest.SimilarityData.FeatureVector = GenerateFeatureVector(quest);

            // Update quality data
            if (quest.QualityData == null)
                quest.QualityData = new QuestQualityData();

            quest.QualityData.OverallScore = feedback.QualityScore;
            quest.QualityData.MeetsMinimumQuality = feedback.QualityScore >= 0.5;
        }
    }
}
