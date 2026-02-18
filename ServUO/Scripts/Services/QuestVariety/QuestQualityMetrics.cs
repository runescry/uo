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
    /// Assesses quest quality and provides improvement recommendations
    /// </summary>
    public class QuestQualityMetrics
    {
        /// <summary>
        /// Assess the quality of a generated quest
        /// </summary>
        public QuestQualityMetrics AssessQuestQuality(DynamicQuest quest)
        {
            if (quest == null)
                return null;

            var metrics = new QuestQualityMetrics
            {
                QuestId = quest.QuestID.ToString(),
                QualityIssues = new List<string>(),
                Strengths = new List<string>(),
                AssessedAt = DateTime.UtcNow
            };

            // Assess different quality aspects
            metrics.OriginalityScore = AssessOriginality(quest, metrics);
            metrics.ComplexityScore = AssessComplexity(quest, metrics);
            metrics.EngagementScore = AssessEngagement(quest, metrics);
            metrics.CoherenceScore = AssessCoherence(quest, metrics);

            // Calculate overall quality
            metrics.OverallQuality = (metrics.OriginalityScore * 0.25) +
                                    (metrics.ComplexityScore * 0.25) +
                                    (metrics.EngagementScore * 0.25) +
                                    (metrics.CoherenceScore * 0.25);

            return metrics;
        }

        /// <summary>
        /// Assess quest originality
        /// </summary>
        private double AssessOriginality(DynamicQuest quest, QuestQualityMetrics metrics)
        {
            double score = 0.8; // Base score
            var text = $"{quest.Title} {quest.Description}".ToLower();

            // Check for common quest templates
            var commonTemplates = new[]
            {
                "kill 10 rats", "collect 5 herbs", "deliver package", "explore cave",
                "rescue prisoner", "defeat dragon", "find artifact", "protect village"
            };

            foreach (var template in commonTemplates)
            {
                if (text.Contains(template))
                {
                    score -= 0.3;
                    metrics.QualityIssues.Add($"Uses common template: {template}");
                }
            }

            // Check for unique combinations
            var uniqueElements = new[]
            {
                "time limit", "weather condition", "moral dilemma", "political intrigue",
                "ancient prophecy", "magical artifact", "hidden treasure", "secret society"
            };

            foreach (var element in uniqueElements)
            {
                if (text.Contains(element))
                {
                    score += 0.1;
                    metrics.Strengths.Add($"Contains unique element: {element}");
                }
            }

            // Check title creativity
            if (quest.Title != null)
            {
                var titleWords = quest.Title.Split(' ').Length;
                if (titleWords > 3)
                {
                    score += 0.1;
                    metrics.Strengths.Add("Creative title length");
                }

                if (quest.Title.Contains(":") || quest.Title.Contains("-"))
                {
                    score += 0.05;
                    metrics.Strengths.Add("Structured title format");
                }
            }

            return Math.Max(0, Math.Min(1, score));
        }

        /// <summary>
        /// Assess quest complexity
        /// </summary>
        private static double AssessComplexity(DynamicQuest quest, QuestQualityMetrics metrics)
        {
            double score = 0.5; // Base score
            var text = quest.Description ?? "";

            // Count objectives
            var objectives = quest.GetObjectives();
            int objectiveCount = objectives?.Count ?? 0;

            if (objectiveCount == 0)
            {
                score -= 0.3;
                metrics.QualityIssues.Add("No clear objectives");
            }
            else if (objectiveCount == 1)
            {
                score += 0.1;
                metrics.Strengths.Add("Simple, focused objective");
            }
            else if (objectiveCount <= 3)
            {
                score += 0.2;
                metrics.Strengths.Add("Balanced objective count");
            }
            else if (objectiveCount > 5)
            {
                score -= 0.2;
                metrics.QualityIssues.Add("Too many objectives may overwhelm player");
            }

            // Assess difficulty progression
            var difficultyKeywords = new[]
            { "easy", "simple", "medium", "hard", "difficult", "challenging", "expert" };
            int difficultyMatches = difficultyKeywords.Count(keyword => text.Contains(keyword));

            if (difficultyMatches == 0)
            {
                score -= 0.1;
                metrics.QualityIssues.Add("No difficulty indication");
            }
            else if (difficultyMatches == 1)
            {
                score += 0.1;
                metrics.Strengths.Add("Clear difficulty indication");
            }

            // Check for multi-stage complexity
            var stageKeywords = new[]
            { "first", "then", "after", "finally", "next", "step", "phase", "part" };
            bool hasStages = stageKeywords.Any(keyword => text.Contains(keyword));

            if (hasStages && objectiveCount > 1)
            {
                score += 0.15;
                metrics.Strengths.Add("Multi-stage quest structure");
            }

            // Assess time complexity
            var timeKeywords = new[]
            { "quickly", "soon", "immediately", "eventually", "finally", "wait", "time" };
            bool hasTimeElement = timeKeywords.Any(keyword => text.Contains(keyword));

            if (hasTimeElement)
            {
                score += 0.05;
                metrics.Strengths.Add("Includes time-based elements");
            }

            return Math.Max(0, Math.Min(1, score));
        }

        /// <summary>
        /// Assess quest engagement potential
        /// </summary>
        private static double AssessEngagement(DynamicQuest quest, QuestQualityMetrics metrics)
        {
            double score = 0.6; // Base score
            var text = $"{quest.Title} {quest.Description}".ToLower();

            // Check for engaging elements
            var engagingElements = new[]
            {
                "mystery", "secret", "hidden", "ancient", "legendary", "rare", "unique",
                "dangerous", "perilous", "urgent", "critical", "important", "special"
            };

            int engagingMatches = engagingElements.Count(element => text.Contains(element));
            score += Math.Min(0.3, engagingMatches * 0.1);

            if (engagingMatches > 0)
            {
                metrics.Strengths.Add($"Contains engaging elements: {engagingMatches} found");
            }

            // Check for player agency
            var agencyKeywords = new[]
            { "choose", "decide", "option", "choice", "path", "way", "alternative" };
            bool hasAgency = agencyKeywords.Any(keyword => text.Contains(keyword));

            if (hasAgency)
            {
                score += 0.15;
                metrics.Strengths.Add("Offers player choice/agency");
            }

            // Check for emotional appeal
            var emotionalKeywords = new[]
            { "help", "save", "rescue", "protect", "avenge", "justice", "honor", "love", "fear" };
            int emotionalMatches = emotionalKeywords.Count(keyword => text.Contains(keyword));
            score += Math.Min(0.2, emotionalMatches * 0.05);

            if (emotionalMatches > 0)
            {
                metrics.Strengths.Add($"Emotional appeal: {emotionalMatches} elements");
            }

            // Check for reward appeal
            var rewardKeywords = new[]
            { "reward", "prize", "treasure", "artifact", "gold", "experience", "reputation", "honor" };
            int rewardMatches = rewardKeywords.Count(keyword => text.Contains(keyword));
            score += Math.Min(0.15, rewardMatches * 0.05);

            if (rewardMatches > 0)
            {
                metrics.Strengths.Add($"Reward motivation: {rewardMatches} rewards mentioned");
            }

            // Check for length (too short = less engaging, too long = overwhelming)
            int wordCount = text.Split(' ').Length;
            if (wordCount < 20)
            {
                score -= 0.2;
                metrics.QualityIssues.Add("Quest description too short");
            }
            else if (wordCount > 200)
            {
                score -= 0.1;
                metrics.QualityIssues.Add("Quest description too long");
            }
            else if (wordCount >= 50 && wordCount <= 100)
            {
                score += 0.1;
                metrics.Strengths.Add("Optimal description length");
            }

            return Math.Max(0, Math.Min(1, score));
        }

        /// <summary>
        /// Assess quest coherence and logical consistency
        /// </summary>
        private static double AssessCoherence(DynamicQuest quest, QuestQualityMetrics metrics)
        {
            double score = 0.7; // Base score
            var text = quest.Description ?? "";

            // Check for logical flow
            var flowIndicators = new[]
            { "because", "since", "due to", "therefore", "thus", "consequently", "however", "although" };
            bool hasLogicalFlow = flowIndicators.Any(indicator => text.Contains(indicator));

            if (hasLogicalFlow)
            {
                score += 0.1;
                metrics.Strengths.Add("Logical narrative flow");
            }

            // Check for consistency
            var title = quest.Title ?? "";
            var description = quest.Description ?? "";

            // Title-description consistency
            if (!string.IsNullOrEmpty(title) && !string.IsNullOrEmpty(description))
            {
                var titleWords = Tokenize(title);
                var descWords = Tokenize(description);
                var commonWords = titleWords.Intersect(descWords).ToList();

                if (commonWords.Count > 0)
                {
                    score += 0.1;
                    metrics.Strengths.Add($"Title-description consistency: {commonWords.Count} common words");
                }
                else if (titleWords.Count > 2 && descWords.Count > 10)
                {
                    score -= 0.15;
                    metrics.QualityIssues.Add("Title and description seem unrelated");
                }
            }

            // Check for contradictory statements
            var contradictions = new[]
            {
                "easy but difficult", "quick but slow", "safe but dangerous",
                "simple but complex", "near but far", "small but large"
            };

            foreach (var contradiction in contradictions)
            {
                if (text.Contains(contradiction))
                {
                    score -= 0.2;
                    metrics.QualityIssues.Add($"Contradictory statement: {contradiction}");
                }
            }

            // Check for vague statements
            var vagueWords = new[]
            { "something", "somewhere", "someone", "somehow", "sometime", "somewhat" };
            int vagueCount = vagueWords.Count(word => text.Contains(word));

            if (vagueCount > 3)
            {
                score -= 0.1;
                metrics.QualityIssues.Add($"Too many vague statements: {vagueCount}");
            }
            else if (vagueCount == 0)
            {
                score += 0.05;
                metrics.Strengths.Add("Specific and clear language");
            }

            // Check for grammar and spelling (basic checks)
            if (text.Contains("..") || text.Contains("  "))
            {
                score -= 0.05;
                metrics.QualityIssues.Add("Potential formatting issues");
            }

            return Math.Max(0, Math.Min(1, score));
        }

        /// <summary>
        /// Get quality improvement recommendations
        /// </summary>
        public static List<string> GetImprovementRecommendations(QuestQualityMetrics metrics)
        {
            var recommendations = new List<string>();

            if (metrics.OriginalityScore < 0.5)
            {
                recommendations.Add("Consider adding unique elements or creative twists to make the quest more original");
            }

            if (metrics.ComplexityScore < 0.5)
            {
                recommendations.Add("Add more depth to objectives or include multi-stage progression");
            }

            if (metrics.EngagementScore < 0.5)
            {
                recommendations.Add("Include more engaging elements like mysteries, choices, or emotional appeal");
            }

            if (metrics.CoherenceScore < 0.5)
            {
                recommendations.Add("Improve narrative flow and ensure title-description consistency");
            }

            if (metrics.QualityIssues.Contains("No clear objectives"))
            {
                recommendations.Add("Clearly define what the player needs to accomplish");
            }

            if (metrics.QualityIssues.Contains("Quest description too short"))
            {
                recommendations.Add("Expand the description to provide more context and motivation");
            }

            return recommendations;
        }

        /// <summary>
        /// Check if quest meets minimum quality standards
        /// </summary>
        public static bool MeetsMinimumStandards(QuestQualityMetrics metrics)
        {
            if (metrics == null)
                return false;

            return metrics.OverallQuality >= 0.4 &&
                   metrics.CoherenceScore >= 0.5 &&
                   !metrics.QualityIssues.Contains("No clear objectives");
        }

        /// <summary>
        /// Tokenize text into words
        /// </summary>
        private static HashSet<string> Tokenize(string text)
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
            }

            return tokens;
        }
    }
}
