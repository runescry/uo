using System;
using System.Collections.Generic;
using System.Linq;
using Server.Mobiles;
using Server.Engines.PartySystem;

namespace Server.Services.UnifiedQuestSystem
{
    /// <summary>
    /// Basic quest validator
    /// </summary>
    public class BasicQuestValidator : IQuestValidator
    {
        public string ValidatorName => "BasicQuestValidator";
        public ValidationType ValidationType => ValidationType.Basic;

        public ValidationResult Validate(UnifiedQuestData quest, ValidationContext context)
        {
            var result = new ValidationResult { Result = ValidationStatus.Valid };

            // Validate quest ID
            if (quest.QuestId <= 0)
            {
                result.Issues.Add(new ValidationIssue
                {
                    Code = "BASIC_INVALID_QUEST_ID",
                    Description = "Quest ID must be greater than 0",
                    Severity = "Error",
                    Category = "Basic"
                });
                result.Result = ValidationStatus.Invalid;
            }

            // Validate title
            if (string.IsNullOrWhiteSpace(quest.Title))
            {
                result.Issues.Add(new ValidationIssue
                {
                    Code = "BASIC_MISSING_TITLE",
                    Description = "Quest title is required",
                    Severity = "Error",
                    Category = "Basic"
                });
                result.Result = ValidationStatus.Invalid;
            }

            // Validate description
            if (string.IsNullOrWhiteSpace(quest.Description))
            {
                result.Issues.Add(new ValidationIssue
                {
                    Code = "BASIC_MISSING_DESCRIPTION",
                    Description = "Quest description is required",
                    Severity = "Error",
                    Category = "Basic"
                });
                result.Result = ValidationStatus.Invalid;
            }

            return result;
        }
    }

    /// <summary>
    /// Multiplayer quest validator
    /// </summary>
    public class MultiplayerQuestValidator : IQuestValidator
    {
        public string ValidatorName => "MultiplayerQuestValidator";
        public ValidationType ValidationType => ValidationType.Multiplayer;

        public ValidationResult Validate(UnifiedQuestData quest, ValidationContext context)
        {
            var result = new ValidationResult { Result = ValidationStatus.Valid };

            // Only validate multiplayer quests
            if (quest.Type != QuestType.Multiplayer)
                return result;

            // Validate party requirements
            if (quest.MultiplayerData?.Party == null)
            {
                result.Issues.Add(new ValidationIssue
                {
                    Code = "MP_MISSING_PARTY",
                    Description = "Multiplayer quest requires a party",
                    Severity = "Error",
                    Category = "Multiplayer"
                });
                result.Result = ValidationStatus.Invalid;
            }
            else if (quest.MultiplayerData.Party.Members.Count < 2)
            {
                result.Issues.Add(new ValidationIssue
                {
                    Code = "MP_INSUFFICIENT_PARTY_SIZE",
                    Description = "Multiplayer quest requires at least 2 party members",
                    Severity = "Error",
                    Category = "Multiplayer"
                });
                result.Result = ValidationStatus.Invalid;
            }

            // Validate cooperative objectives
            if (quest.CooperativeObjectives == null || quest.CooperativeObjectives.Count == 0)
            {
                result.Issues.Add(new ValidationIssue
                {
                    Code = "MP_NO_OBJECTIVES",
                    Description = "Multiplayer quest should have cooperative objectives",
                    Severity = "Warning",
                    Category = "Multiplayer"
                });
                if (result.Result == ValidationStatus.Valid)
                    result.Result = ValidationStatus.ValidWithWarnings;
            }

            return result;
        }
    }

    /// <summary>
    /// Quest variety validator
    /// </summary>
    public class QuestVarietyValidator : IQuestValidator
    {
        public string ValidatorName => "QuestVarietyValidator";
        public ValidationType ValidationType => ValidationType.Variety;

        public ValidationResult Validate(UnifiedQuestData quest, ValidationContext context)
        {
            var result = new ValidationResult { Result = ValidationStatus.Valid };

            // Validate similarity data
            if (quest.SimilarityData != null && quest.SimilarityData.SimilarityScore > 0.8)
            {
                result.Issues.Add(new ValidationIssue
                {
                    Code = "VARIETY_HIGH_SIMILARITY",
                    Description = "Quest is very similar to existing quests",
                    Severity = "Warning",
                    Category = "Variety"
                });
                if (result.Result == ValidationStatus.Valid)
                    result.Result = ValidationStatus.ValidWithWarnings;
            }

            // Validate quality data
            if (quest.QualityData != null && quest.QualityData.OverallScore < 0.3)
            {
                result.Issues.Add(new ValidationIssue
                {
                    Code = "VARIETY_LOW_QUALITY",
                    Description = "Quest quality is below recommended threshold",
                    Severity = "Warning",
                    Category = "Variety"
                });
                if (result.Result == ValidationStatus.Valid)
                    result.Result = ValidationStatus.ValidWithWarnings;
            }

            // Validate theme
            if (string.IsNullOrWhiteSpace(quest.Theme))
            {
                result.Issues.Add(new ValidationIssue
                {
                    Code = "VARIETY_MISSING_THEME",
                    Description = "Quest theme is recommended for variety tracking",
                    Severity = "Info",
                    Category = "Variety"
                });
            }

            return result;
        }
    }

    /// <summary>
    /// Quest persistence validator
    /// </summary>
    public class QuestPersistenceValidator : IQuestValidator
    {
        public string ValidatorName => "QuestPersistenceValidator";
        public ValidationType ValidationType => ValidationType.Persistence;

        public ValidationResult Validate(UnifiedQuestData quest, ValidationContext context)
        {
            var result = new ValidationResult { Result = ValidationStatus.Valid };

            // Validate persistence requirements
            if (quest.IsPersistent && quest.PersistenceData == null)
            {
                result.Issues.Add(new ValidationIssue
                {
                    Code = "PERSISTENCE_MISSING_DATA",
                    Description = "Persistent quest requires persistence data",
                    Severity = "Error",
                    Category = "Persistence"
                });
                result.Result = ValidationStatus.Invalid;
            }

            // Validate save version
            if (quest.PersistenceData != null && string.IsNullOrWhiteSpace(quest.PersistenceData.SaveVersion))
            {
                result.Issues.Add(new ValidationIssue
                {
                    Code = "PERSISTENCE_MISSING_VERSION",
                    Description = "Save version is required for persistence",
                    Severity = "Warning",
                    Category = "Persistence"
                });
                if (result.Result == ValidationStatus.Valid)
                    result.Result = ValidationStatus.ValidWithWarnings;
            }

            // Validate backup locations
            if (quest.IsPersistent && (quest.PersistenceData?.BackupLocations.Count == 0))
            {
                result.Issues.Add(new ValidationIssue
                {
                    Code = "PERSISTENCE_NO_BACKUPS",
                    Description = "Persistent quest should have backup locations",
                    Severity = "Warning",
                    Category = "Persistence"
                });
                if (result.Result == ValidationStatus.Valid)
                    result.Result = ValidationStatus.ValidWithWarnings;
            }

            return result;
        }
    }

    /// <summary>
    /// Quest journal validator
    /// </summary>
    public class QuestJournalValidator : IQuestValidator
    {
        public string ValidatorName => "QuestJournalValidator";
        public ValidationType ValidationType => ValidationType.Journal;

        public ValidationResult Validate(UnifiedQuestData quest, ValidationContext context)
        {
            var result = new ValidationResult { Result = ValidationStatus.Valid };

            // Validate journal data
            if (quest.IsInJournal && quest.JournalData == null)
            {
                result.Issues.Add(new ValidationIssue
                {
                    Code = "JOURNAL_MISSING_DATA",
                    Description = "Quest in journal requires journal data",
                    Severity = "Error",
                    Category = "Journal"
                });
                result.Result = ValidationStatus.Invalid;
            }

            // Validate journal category
            if (quest.JournalData != null && string.IsNullOrWhiteSpace(quest.JournalData.JournalCategory))
            {
                result.Issues.Add(new ValidationIssue
                {
                    Code = "JOURNAL_MISSING_CATEGORY",
                    Description = "Journal category is required for organization",
                    Severity = "Warning",
                    Category = "Journal"
                });
                if (result.Result == ValidationStatus.Valid)
                    result.Result = ValidationStatus.ValidWithWarnings;
            }

            // Validate journal entries for active quests
            if (quest.Status == QuestStatus.Active && quest.JournalData?.JournalEntries.Count == 0)
            {
                result.Issues.Add(new ValidationIssue
                {
                    Code = "JOURNAL_NO_ENTRIES",
                    Description = "Active quest should have journal entries",
                    Severity = "Info",
                    Category = "Journal"
                });
            }

            return result;
        }
    }

    /// <summary>
    /// Performance validator for quest performance metrics
    /// </summary>
    public class QuestPerformanceValidator : IQuestValidator
    {
        public string ValidatorName => "QuestPerformanceValidator";
        public ValidationType ValidationType => ValidationType.Performance;

        public ValidationResult Validate(UnifiedQuestData quest, ValidationContext context)
        {
            var result = new ValidationResult { Result = ValidationStatus.Valid };

            // Validate quest complexity
            var complexity = CalculateQuestComplexity(quest);
            if (complexity > 100)
            {
                result.Issues.Add(new ValidationIssue
                {
                    Code = "PERFORMANCE_HIGH_COMPLEXITY",
                    Description = "Quest complexity may impact performance",
                    Severity = "Warning",
                    Category = "Performance",
                    Recommendation = "Consider simplifying quest structure"
                });
                if (result.Result == ValidationStatus.Valid)
                    result.Result = ValidationStatus.ValidWithWarnings;
            }

            // Validate memory usage
            var estimatedMemory = EstimateMemoryUsage(quest);
            if (estimatedMemory > 1024 * 1024) // 1MB
            {
                result.Issues.Add(new ValidationIssue
                {
                    Code = "PERFORMANCE_HIGH_MEMORY",
                    Description = "Quest may use excessive memory",
                    Severity = "Warning",
                    Category = "Performance",
                    Recommendation = "Optimize quest data structure"
                });
                if (result.Result == ValidationStatus.Valid)
                    result.Result = ValidationStatus.ValidWithWarnings;
            }

            return result;
        }

        private int CalculateQuestComplexity(UnifiedQuestData quest)
        {
            var complexity = 0;
            
            // Base complexity
            complexity += 10;
            
            // Add complexity for objectives
            complexity += (quest.CooperativeObjectives?.Count ?? 0) * 5;
            
            // Add complexity for metadata
            complexity += (quest.Metadata?.Count ?? 0) * 2;
            
            // Add complexity for player progress
            complexity += (quest.PlayerProgress?.Count ?? 0) * 3;
            
            // Add complexity for multiplayer features
            if (quest.IsMultiplayer)
                complexity += 20;
            
            return complexity;
        }

        private long EstimateMemoryUsage(UnifiedQuestData quest)
        {
            var memory = 0;
            
            // Base memory usage
            memory += 1024; // 1KB base
            
            // Add memory for collections
            memory += (quest.Tags?.Count ?? 0) * 50;
            memory += (quest.Metadata?.Count ?? 0) * 100;
            memory += (quest.CooperativeObjectives?.Count ?? 0) * 200;
            memory += (quest.PlayerProgress?.Count ?? 0) * 500;
            
            // Add memory for strings
            memory += quest.Title?.Length ?? 0;
            memory += quest.Description?.Length ?? 0;
            memory += quest.Theme?.Length ?? 0;
            memory += quest.Location?.Length ?? 0;
            
            return memory;
        }
    }

    /// <summary>
    /// Security validator for quest security checks
    /// </summary>
    public class QuestSecurityValidator : IQuestValidator
    {
        public string ValidatorName => "QuestSecurityValidator";
        public ValidationType ValidationType => ValidationType.Security;

        public ValidationResult Validate(UnifiedQuestData quest, ValidationContext context)
        {
            var result = new ValidationResult { Result = ValidationStatus.Valid };

            // Validate for potential security issues
            ValidateForSecurityIssues(quest, result);
            ValidateForExploits(quest, result);
            ValidateForPermissions(quest, result);

            return result;
        }

        private void ValidateForSecurityIssues(UnifiedQuestData quest, ValidationResult result)
        {
            // Check for suspicious content in title and description
            var suspiciousPatterns = new[]
            {
                "javascript:", "script:", "eval(", "exec(", "system(",
                "<script", "</script>", "onclick", "onload"
            };

            var content = $"{quest.Title} {quest.Description}".ToLower();
            
            foreach (var pattern in suspiciousPatterns)
            {
                if (content.Contains(pattern))
                {
                    result.Issues.Add(new ValidationIssue
                    {
                        Code = "SECURITY_SUSPICIOUS_CONTENT",
                        Description = "Quest content contains potentially suspicious patterns",
                        Severity = "Critical",
                        Category = "Security",
                        Recommendation = "Review quest content for security issues"
                    });
                    result.Result = ValidationStatus.Critical;
                    return;
                }
            }
        }

        private void ValidateForExploits(UnifiedQuestData quest, ValidationResult result)
        {
            // Check for potential exploits in rewards
            if (quest.CooperativeObjectives != null)
            {
                foreach (var objective in quest.CooperativeObjectives)
                {
                    if (objective.RequiredCount < 0 || objective.CurrentProgress < 0)
                    {
                        result.Issues.Add(new ValidationIssue
                        {
                            Code = "SECURITY_POTENTIAL_EXPLOIT",
                            Description = "Objective values may allow exploits",
                            Severity = "Error",
                            Category = "Security",
                            Recommendation = "Validate objective values for security"
                        });
                        result.Result = ValidationStatus.Invalid;
                    }
                }
            }
        }

        private void ValidateForPermissions(UnifiedQuestData quest, ValidationResult result)
        {
            // Check for permission issues
            if (quest.Owner == null)
            {
                result.Issues.Add(new ValidationIssue
                {
                    Code = "SECURITY_NO_OWNER",
                    Description = "Quest without owner may cause security issues",
                    Severity = "Warning",
                    Category = "Security",
                    Recommendation = "Assign quest owner for proper permissions"
                });
                if (result.Result == ValidationStatus.Valid)
                    result.Result = ValidationStatus.ValidWithWarnings;
            }
        }
    }

    /// <summary>
    /// Compatibility validator for quest compatibility checks
    /// </summary>
    public class QuestCompatibilityValidator : IQuestValidator
    {
        public string ValidatorName => "QuestCompatibilityValidator";
        public ValidationType ValidationType => ValidationType.Compatibility;

        public ValidationResult Validate(UnifiedQuestData quest, ValidationContext context)
        {
            var result = new ValidationResult { Result = ValidationStatus.Valid };

            // Validate compatibility with different systems
            ValidateWithLLMSystem(quest, result);
            ValidateWithPartySystem(quest, result);
            ValidateWithJournalSystem(quest, result);

            return result;
        }

        private void ValidateWithLLMSystem(UnifiedQuestData quest, ValidationResult result)
        {
            // Check compatibility with LLM quest generation
            if (quest.Type == QuestType.Dynamic || quest.Type == QuestType.Vystia)
            {
                if (string.IsNullOrWhiteSpace(quest.Theme))
                {
                    result.Issues.Add(new ValidationIssue
                    {
                        Code = "COMPATIBILITY_LLM_THEME",
                        Description = "LLM-generated quests should have themes",
                        Severity = "Warning",
                        Category = "Compatibility",
                        Recommendation = "Add theme for better LLM integration"
                    });
                    if (result.Result == ValidationStatus.Valid)
                        result.Result = ValidationStatus.ValidWithWarnings;
                }
            }
        }

        private void ValidateWithPartySystem(UnifiedQuestData quest, ValidationResult result)
        {
            // Check compatibility with party system
            if (quest.IsMultiplayer && quest.MultiplayerData?.Party == null)
            {
                result.Issues.Add(new ValidationIssue
                {
                    Code = "COMPATIBILITY_PARTY_SYSTEM",
                    Description = "Multiplayer quest requires party system integration",
                    Severity = "Error",
                    Category = "Compatibility",
                    Recommendation = "Integrate with party system"
                });
                result.Result = ValidationStatus.Invalid;
            }
        }

        private void ValidateWithJournalSystem(UnifiedQuestData quest, ValidationResult result)
        {
            // Check compatibility with journal system
            if (quest.IsInJournal && quest.JournalData == null)
            {
                result.Issues.Add(new ValidationIssue
                {
                    Code = "COMPATIBILITY_JOURNAL_SYSTEM",
                    Description = "Quest in journal requires journal system integration",
                    Severity = "Warning",
                    Category = "Compatibility",
                    Recommendation = "Integrate with journal system"
                });
                if (result.Result == ValidationStatus.Valid)
                    result.Result = ValidationStatus.ValidWithWarnings;
            }
        }
    }
}
