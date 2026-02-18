using System;
using System.Collections.Generic;
using System.Linq;
using Server.Mobiles;
using Server.Engines.PartySystem;

namespace Server.Services.UnifiedQuestSystem
{
    /// <summary>
    /// Basic validation strategy for all quest types
    /// </summary>
    public class BasicValidationStrategy : IQuestValidationStrategy
    {
        public string StrategyName => "basic";
        public ValidationType[] SupportedTypes => new[] { ValidationType.Basic };

        public ValidationResult Validate(UnifiedQuestData quest, ValidationContext context)
        {
            var result = new ValidationResult { Result = ValidationStatus.Valid };

            // Validate basic quest properties
            ValidateBasicProperties(quest, result);
            
            // Validate quest metadata
            ValidateMetadata(quest, result);
            
            // Validate timestamps
            ValidateTimestamps(quest, result);
            
            // Validate owner/creator
            ValidateOwnership(quest, result);

            return result;
        }

        private void ValidateBasicProperties(UnifiedQuestData quest, ValidationResult result)
        {
            // Check quest ID
            if (quest.QuestId <= 0)
            {
                result.Issues.Add(new ValidationIssue
                {
                    Code = "INVALID_QUEST_ID",
                    Description = "Quest ID must be greater than 0",
                    Severity = "Error",
                    Recommendation = "Assign a valid quest ID"
                });
                result.Result = ValidationStatus.Invalid;
            }

            // Check title
            if (string.IsNullOrWhiteSpace(quest.Title))
            {
                result.Issues.Add(new ValidationIssue
                {
                    Code = "MISSING_TITLE",
                    Description = "Quest title is required",
                    Severity = "Error",
                    Recommendation = "Provide a descriptive quest title"
                });
                result.Result = ValidationStatus.Invalid;
            }
            else if (quest.Title.Length > 100)
            {
                result.Issues.Add(new ValidationIssue
                {
                    Code = "TITLE_TOO_LONG",
                    Description = "Quest title exceeds maximum length (100 characters)",
                    Severity = "Warning",
                    Recommendation = "Consider shortening the quest title"
                });
                if (result.Result == ValidationStatus.Valid)
                    result.Result = ValidationStatus.ValidWithWarnings;
            }

            // Check description
            if (string.IsNullOrWhiteSpace(quest.Description))
            {
                result.Issues.Add(new ValidationIssue
                {
                    Code = "MISSING_DESCRIPTION",
                    Description = "Quest description is required",
                    Severity = "Error",
                    Recommendation = "Provide a quest description"
                });
                result.Result = ValidationStatus.Invalid;
            }
            else if (quest.Description.Length < 50)
            {
                result.Issues.Add(new ValidationIssue
                {
                    Code = "DESCRIPTION_TOO_SHORT",
                    Description = "Quest description is too short (minimum 50 characters)",
                    Severity = "Warning",
                    Recommendation = "Provide more detailed quest description"
                });
                if (result.Result == ValidationStatus.Valid)
                    result.Result = ValidationStatus.ValidWithWarnings;
            }
        }

        private void ValidateMetadata(UnifiedQuestData quest, ValidationResult result)
        {
            // Validate tags
            if (quest.Tags != null && quest.Tags.Count > 10)
            {
                result.Issues.Add(new ValidationIssue
                {
                    Code = "TOO_MANY_TAGS",
                    Description = "Too many quest tags (maximum 10)",
                    Severity = "Warning",
                    Recommendation = "Reduce the number of quest tags"
                });
                if (result.Result == ValidationStatus.Valid)
                    result.Result = ValidationStatus.ValidWithWarnings;
            }

            // Validate metadata
            if (quest.Metadata != null && quest.Metadata.Count > 50)
            {
                result.Issues.Add(new ValidationIssue
                {
                    Code = "TOO_MANY_METADATA",
                    Description = "Too many metadata entries (maximum 50)",
                    Severity = "Warning",
                    Recommendation = "Reduce the amount of metadata"
                });
                if (result.Result == ValidationStatus.Valid)
                    result.Result = ValidationStatus.ValidWithWarnings;
            }
        }

        private void ValidateTimestamps(UnifiedQuestData quest, ValidationResult result)
        {
            // Check creation time
            if (quest.CreatedAt == default)
            {
                result.Issues.Add(new ValidationIssue
                {
                    Code = "INVALID_CREATED_AT",
                    Description = "Quest creation time is not set",
                    Severity = "Error",
                    Recommendation = "Set the quest creation time"
                });
                result.Result = ValidationStatus.Invalid;
            }

            // Check update time
            if (quest.UpdatedAt == default)
            {
                result.Issues.Add(new ValidationIssue
                {
                    Code = "INVALID_UPDATED_AT",
                    Description = "Quest update time is not set",
                    Severity = "Warning",
                    Recommendation = "Set the quest update time"
                });
                if (result.Result == ValidationStatus.Valid)
                    result.Result = ValidationStatus.ValidWithWarnings;
            }

            // Check time logic
            if (quest.CreatedAt > quest.UpdatedAt)
            {
                result.Issues.Add(new ValidationIssue
                {
                    Code = "INVALID_TIME_LOGIC",
                    Description = "Creation time cannot be after update time",
                    Severity = "Error",
                    Recommendation = "Correct the timestamp logic"
                });
                result.Result = ValidationStatus.Invalid;
            }
        }

        private void ValidateOwnership(UnifiedQuestData quest, ValidationResult result)
        {
            // Check owner
            if (quest.Owner == null)
            {
                result.Issues.Add(new ValidationIssue
                {
                    Code = "MISSING_OWNER",
                    Description = "Quest owner is required",
                    Severity = "Error",
                    Recommendation = "Assign a quest owner"
                });
                result.Result = ValidationStatus.Invalid;
            }
            else if (quest.Owner.Deleted)
            {
                result.Issues.Add(new ValidationIssue
                {
                    Code = "DELETED_OWNER",
                    Description = "Quest owner has been deleted",
                    Severity = "Critical",
                    Recommendation = "Assign a new quest owner or remove the quest"
                });
                result.Result = ValidationStatus.Critical;
            }

            // Check creator
            if (quest.Creator == null)
            {
                result.Issues.Add(new ValidationIssue
                {
                    Code = "MISSING_CREATOR",
                    Description = "Quest creator is required",
                    Severity = "Warning",
                    Recommendation = "Assign a quest creator"
                });
                if (result.Result == ValidationStatus.Valid)
                    result.Result = ValidationStatus.ValidWithWarnings;
            }
        }
    }

    /// <summary>
    /// Multiplayer validation strategy for multiplayer quests
    /// </summary>
    public class MultiplayerValidationStrategy : IQuestValidationStrategy
    {
        public string StrategyName => "multiplayer";
        public ValidationType[] SupportedTypes => new[] { ValidationType.Multiplayer };

        public ValidationResult Validate(UnifiedQuestData quest, ValidationContext context)
        {
            var result = new ValidationResult { Result = ValidationStatus.Valid };

            // Only validate multiplayer quests
            if (quest.Type != QuestType.Multiplayer)
            {
                result.Issues.Add(new ValidationIssue
                {
                    Code = "NOT_MULTIPLAYER_QUEST",
                    Description = "Quest is not a multiplayer quest",
                    Severity = "Warning",
                    Recommendation = "Use basic validation strategy for non-multiplayer quests"
                });
                result.Result = ValidationStatus.ValidWithWarnings;
                return result;
            }

            // Validate multiplayer-specific properties
            ValidateMultiplayerData(quest, result);
            ValidatePartyRequirements(quest, result);
            ValidateCooperativeObjectives(quest, result);
            ValidateRewardDistribution(quest, result);

            return result;
        }

        private void ValidateMultiplayerData(UnifiedQuestData quest, ValidationResult result)
        {
            if (quest.MultiplayerData == null)
            {
                result.Issues.Add(new ValidationIssue
                {
                    Code = "MISSING_MULTIPLAYER_DATA",
                    Description = "Multiplayer quest data is required",
                    Severity = "Error",
                    Recommendation = "Set multiplayer quest data"
                });
                result.Result = ValidationStatus.Invalid;
                return;
            }

            // Validate party
            if (quest.MultiplayerData.Party == null)
            {
                result.Issues.Add(new ValidationIssue
                {
                    Code = "MISSING_PARTY",
                    Description = "Multiplayer quest requires a party",
                    Severity = "Error",
                    Recommendation = "Assign a party to the quest"
                });
                result.Result = ValidationStatus.Invalid;
            }
            else if (quest.MultiplayerData.Party.Members.Count < 2)
            {
                result.Issues.Add(new ValidationIssue
                {
                    Code = "INSUFFICIENT_PARTY_SIZE",
                    Description = "Multiplayer quest requires at least 2 party members",
                    Severity = "Error",
                    Recommendation = "Ensure party has sufficient members"
                });
                result.Result = ValidationStatus.Invalid;
            }

            // Validate settings
            if (quest.MultiplayerData.Settings == null)
            {
                result.Issues.Add(new ValidationIssue
                {
                    Code = "MISSING_SETTINGS",
                    Description = "Multiplayer quest settings are required",
                    Severity = "Warning",
                    Recommendation = "Set multiplayer quest settings"
                });
                if (result.Result == ValidationStatus.Valid)
                    result.Result = ValidationStatus.ValidWithWarnings;
            }
        }

        private void ValidatePartyRequirements(UnifiedQuestData quest, ValidationResult result)
        {
            if (quest.MultiplayerData?.Settings == null)
                return;

            var settings = quest.MultiplayerData.Settings;

            // Validate party size limits
            if (settings.MinimumPartySize < 2)
            {
                result.Issues.Add(new ValidationIssue
                {
                    Code = "INVALID_MIN_PARTY_SIZE",
                    Description = "Minimum party size must be at least 2",
                    Severity = "Error",
                    Recommendation = "Set minimum party size to 2 or more"
                });
                result.Result = ValidationStatus.Invalid;
            }

            if (settings.MaximumPartySize < settings.MinimumPartySize)
            {
                result.Issues.Add(new ValidationIssue
                {
                    Code = "INVALID_MAX_PARTY_SIZE",
                    Description = "Maximum party size cannot be less than minimum",
                    Severity = "Error",
                    Recommendation = "Set maximum party size greater than or equal to minimum"
                });
                result.Result = ValidationStatus.Invalid;
            }

            if (settings.MaximumPartySize > 8)
            {
                result.Issues.Add(new ValidationIssue
                {
                    Code = "MAX_PARTY_SIZE_TOO_LARGE",
                    Description = "Maximum party size exceeds recommended limit (8)",
                    Severity = "Warning",
                    Recommendation = "Consider reducing maximum party size for better performance"
                });
                if (result.Result == ValidationStatus.Valid)
                    result.Result = ValidationStatus.ValidWithWarnings;
            }
        }

        private void ValidateCooperativeObjectives(UnifiedQuestData quest, ValidationResult result)
        {
            if (quest.CooperativeObjectives == null || quest.CooperativeObjectives.Count == 0)
            {
                result.Issues.Add(new ValidationIssue
                {
                    Code = "NO_COOPERATIVE_OBJECTIVES",
                    Description = "Multiplayer quest should have cooperative objectives",
                    Severity = "Warning",
                    Recommendation = "Add cooperative objectives for better multiplayer experience"
                });
                if (result.Result == ValidationStatus.Valid)
                    result.Result = ValidationStatus.ValidWithWarnings;
                return;
            }

            // Validate each cooperative objective
            foreach (var objective in quest.CooperativeObjectives)
            {
                if (string.IsNullOrWhiteSpace(objective.Description))
                {
                    result.Issues.Add(new ValidationIssue
                    {
                        Code = "MISSING_OBJECTIVE_DESCRIPTION",
                        Description = $"Cooperative objective {objective.ObjectiveId} is missing description",
                        Severity = "Error",
                        Recommendation = "Provide description for all cooperative objectives"
                    });
                    result.Result = ValidationStatus.Invalid;
                }

                if (objective.RequiredCount <= 0)
                {
                    result.Issues.Add(new ValidationIssue
                    {
                        Code = "INVALID_REQUIRED_COUNT",
                        Description = $"Cooperative objective {objective.ObjectiveId} has invalid required count",
                        Severity = "Error",
                        Recommendation = "Set required count greater than 0"
                    });
                    result.Result = ValidationStatus.Invalid;
                }

                if (objective.CurrentProgress < 0)
                {
                    result.Issues.Add(new ValidationIssue
                    {
                        Code = "INVALID_PROGRESS",
                        Description = $"Cooperative objective {objective.ObjectiveId} has negative progress",
                        Severity = "Error",
                        Recommendation = "Set progress to 0 or greater"
                    });
                    result.Result = ValidationStatus.Invalid;
                }
            }
        }

        private void ValidateRewardDistribution(UnifiedQuestData quest, ValidationResult result)
        {
            if (quest.MultiplayerData?.Settings == null)
                return;

            var settings = quest.MultiplayerData.Settings;

            // Validate reward distribution settings
            if (!settings.ShareRewardsEqually && !settings.AllowIndividualCompletion)
            {
                result.Issues.Add(new ValidationIssue
                {
                    Code = "INCONSISTENT_REWARD_SETTINGS",
                    Description = "Reward distribution settings may be inconsistent",
                    Severity = "Warning",
                    Recommendation = "Review reward distribution settings for consistency"
                });
                if (result.Result == ValidationStatus.Valid)
                    result.Result = ValidationStatus.ValidWithWarnings;
            }

            // Validate role-based requirements
            if (quest.CooperativeObjectives.Any(obj => obj.Type == CooperativeObjectiveType.RoleBased) && 
                quest.CooperativeObjectives.All(obj => obj.RequiredRoles.Count == 0))
            {
                result.Issues.Add(new ValidationIssue
                {
                    Code = "MISSING_ROLE_REQUIREMENTS",
                    Description = "Role-based objectives require role definitions",
                    Severity = "Error",
                    Recommendation = "Define required roles for role-based objectives"
                });
                result.Result = ValidationStatus.Invalid;
            }
        }
    }

    /// <summary>
    /// Variety validation strategy for quest variety system
    /// </summary>
    public class VarietyValidationStrategy : IQuestValidationStrategy
    {
        public string StrategyName => "variety";
        public ValidationType[] SupportedTypes => new[] { ValidationType.Variety };

        public ValidationResult Validate(UnifiedQuestData quest, ValidationContext context)
        {
            var result = new ValidationResult { Result = ValidationStatus.Valid };

            // Validate variety-specific data
            ValidateSimilarityData(quest, result);
            ValidateQualityData(quest, result);
            ValidateThemeData(quest, result);
            ValidatePreferenceData(quest, result);

            return result;
        }

        private void ValidateSimilarityData(UnifiedQuestData quest, ValidationResult result)
        {
            if (quest.SimilarityData == null)
            {
                result.Issues.Add(new ValidationIssue
                {
                    Code = "MISSING_SIMILARITY_DATA",
                    Description = "Quest similarity data is recommended for variety tracking",
                    Severity = "Warning",
                    Recommendation = "Add similarity data for better quest variety management"
                });
                if (result.Result == ValidationStatus.Valid)
                    result.Result = ValidationStatus.ValidWithWarnings;
                return;
            }

            // Validate similarity score
            if (quest.SimilarityData.SimilarityScore < 0 || quest.SimilarityData.SimilarityScore > 1)
            {
                result.Issues.Add(new ValidationIssue
                {
                    Code = "INVALID_SIMILARITY_SCORE",
                    Description = "Similarity score must be between 0 and 1",
                    Severity = "Error",
                    Recommendation = "Normalize similarity score to 0-1 range"
                });
                result.Result = ValidationStatus.Invalid;
            }

            // Validate repetition threshold
            if (quest.SimilarityData.SimilarityScore > quest.SimilarityData.RepetitionThreshold)
            {
                result.Issues.Add(new ValidationIssue
                {
                    Code = "QUEST_TOO_SIMILAR",
                    Description = "Quest is too similar to existing quests",
                    Severity = "Warning",
                    Recommendation = "Consider modifying quest to increase variety"
                });
                if (result.Result == ValidationStatus.Valid)
                    result.Result = ValidationStatus.ValidWithWarnings;
            }
        }

        private void ValidateQualityData(UnifiedQuestData quest, ValidationResult result)
        {
            if (quest.QualityData == null)
            {
                result.Issues.Add(new ValidationIssue
                {
                    Code = "MISSING_QUALITY_DATA",
                    Description = "Quest quality data is recommended for quality tracking",
                    Severity = "Warning",
                    Recommendation = "Add quality data for better quest quality management"
                });
                if (result.Result == ValidationStatus.Valid)
                    result.Result = ValidationStatus.ValidWithWarnings;
                return;
            }

            // Validate quality scores
            if (quest.QualityData.OverallScore < 0 || quest.QualityData.OverallScore > 1)
            {
                result.Issues.Add(new ValidationIssue
                {
                    Code = "INVALID_QUALITY_SCORE",
                    Description = "Quality scores must be between 0 and 1",
                    Severity = "Error",
                    Recommendation = "Normalize quality scores to 0-1 range"
                });
                result.Result = ValidationStatus.Invalid;
            }

            // Check minimum quality threshold
            if (quest.QualityData.OverallScore < 0.3 && !quest.QualityData.MeetsMinimumQuality)
            {
                result.Issues.Add(new ValidationIssue
                {
                    Code = "LOW_QUALITY_QUEST",
                    Description = "Quest quality is below minimum threshold",
                    Severity = "Warning",
                    Recommendation = "Improve quest quality before deployment"
                });
                if (result.Result == ValidationStatus.Valid)
                    result.Result = ValidationStatus.ValidWithWarnings;
            }
        }

        private void ValidateThemeData(UnifiedQuestData quest, ValidationResult result)
        {
            if (quest.ThemeData == null)
            {
                result.Issues.Add(new ValidationIssue
                {
                    Code = "MISSING_THEME_DATA",
                    Description = "Quest theme data is recommended for theme tracking",
                    Severity = "Warning",
                    Recommendation = "Add theme data for better quest variety management"
                });
                if (result.Result == ValidationStatus.Valid)
                    result.Result = ValidationStatus.ValidWithWarnings;
                return;
            }

            // Validate theme weight
            if (quest.ThemeData.ThemeWeight <= 0)
            {
                result.Issues.Add(new ValidationIssue
                {
                    Code = "INVALID_THEME_WEIGHT",
                    Description = "Theme weight must be greater than 0",
                    Severity = "Warning",
                    Recommendation = "Set positive theme weight"
                });
                if (result.Result == ValidationStatus.Valid)
                    result.Result = ValidationStatus.ValidWithWarnings;
            }

            // Check theme cooldown
            if (quest.ThemeData.CooldownRemaining > TimeSpan.FromHours(24))
            {
                result.Issues.Add(new ValidationIssue
                {
                    Code = "LONG_THEME_COOLDOWN",
                    Description = "Theme cooldown is unusually long",
                    Severity = "Warning",
                    Recommendation = "Review theme cooldown settings"
                });
                if (result.Result == ValidationStatus.Valid)
                    result.Result = ValidationStatus.ValidWithWarnings;
            }
        }

        private void ValidatePreferenceData(UnifiedQuestData quest, ValidationResult result)
        {
            if (quest.PreferenceData == null)
            {
                result.Issues.Add(new ValidationIssue
                {
                    Code = "MISSING_PREFERENCE_DATA",
                    Description = "Player preference data is recommended for personalization",
                    Severity = "Info",
                    Recommendation = "Add preference data for better player experience"
                });
                return;
            }

            // Validate preference scores
            if (quest.PreferenceData.PreferenceScore < 0 || quest.PreferenceData.PreferenceScore > 1)
            {
                result.Issues.Add(new ValidationIssue
                {
                    Code = "INVALID_PREFERENCE_SCORE",
                    Description = "Preference score must be between 0 and 1",
                    Severity = "Warning",
                    Recommendation = "Normalize preference score to 0-1 range"
                });
                if (result.Result == ValidationStatus.Valid)
                    result.Result = ValidationStatus.ValidWithWarnings;
            }
        }
    }

    /// <summary>
    /// Persistence validation strategy for quest persistence
    /// </summary>
    public class PersistenceValidationStrategy : IQuestValidationStrategy
    {
        public string StrategyName => "persistence";
        public ValidationType[] SupportedTypes => new[] { ValidationType.Persistence };

        public ValidationResult Validate(UnifiedQuestData quest, ValidationContext context)
        {
            var result = new ValidationResult { Result = ValidationStatus.Valid };

            // Validate persistence-specific data
            ValidatePersistenceData(quest, result);
            ValidateSerialization(quest, result);

            return result;
        }

        private void ValidatePersistenceData(UnifiedQuestData quest, ValidationResult result)
        {
            if (quest.PersistenceData == null)
            {
                result.Issues.Add(new ValidationIssue
                {
                    Code = "MISSING_PERSISTENCE_DATA",
                    Description = "Quest persistence data is required for persistent quests",
                    Severity = "Warning",
                    Recommendation = "Add persistence data for quest persistence"
                });
                if (result.Result == ValidationStatus.Valid)
                    result.Result = ValidationStatus.ValidWithWarnings;
                return;
            }

            // Validate save version
            if (string.IsNullOrWhiteSpace(quest.PersistenceData.SaveVersion))
            {
                result.Issues.Add(new ValidationIssue
                {
                    Code = "MISSING_SAVE_VERSION",
                    Description = "Save version is required for persistence",
                    Severity = "Warning",
                    Recommendation = "Set save version for compatibility tracking"
                });
                if (result.Result == ValidationStatus.Valid)
                    result.Result = ValidationStatus.ValidWithWarnings;
            }

            // Check backup locations
            if (quest.PersistenceData.BackupLocations.Count == 0 && quest.IsPersistent)
            {
                result.Issues.Add(new ValidationIssue
                {
                    Code = "NO_BACKUP_LOCATIONS",
                    Description = "Persistent quests should have backup locations",
                    Severity = "Warning",
                    Recommendation = "Add backup locations for data safety"
                });
                if (result.Result == ValidationStatus.Valid)
                    result.Result = ValidationStatus.ValidWithWarnings;
            }
        }

        private void ValidateSerialization(UnifiedQuestData quest, ValidationResult result)
        {
            // Test serialization by attempting to serialize key data
            try
            {
                // This would test actual serialization in a real implementation
                if (quest.QuestId == 0)
                {
                    result.Issues.Add(new ValidationIssue
                    {
                        Code = "SERIALIZATION_TEST_FAILED",
                        Description = "Quest data may not serialize properly",
                        Severity = "Error",
                        Recommendation = "Check quest data structure for serialization issues"
                    });
                    result.Result = ValidationStatus.Invalid;
                }
            }
            catch (Exception ex)
            {
                result.Issues.Add(new ValidationIssue
                {
                    Code = "SERIALIZATION_ERROR",
                    Description = $"Serialization error: {ex.Message}",
                    Severity = "Critical",
                    Recommendation = "Fix serialization issues before persistence"
                });
                result.Result = ValidationStatus.Critical;
            }
        }
    }

    /// <summary>
    /// Journal validation strategy for quest journal system
    /// </summary>
    public class JournalValidationStrategy : IQuestValidationStrategy
    {
        public string StrategyName => "journal";
        public ValidationType[] SupportedTypes => new[] { ValidationType.Journal };

        public ValidationResult Validate(UnifiedQuestData quest, ValidationContext context)
        {
            var result = new ValidationResult { Result = ValidationStatus.Valid };

            // Validate journal-specific data
            ValidateJournalData(quest, result);

            return result;
        }

        private void ValidateJournalData(UnifiedQuestData quest, ValidationResult result)
        {
            if (quest.JournalData == null)
            {
                result.Issues.Add(new ValidationIssue
                {
                    Code = "MISSING_JOURNAL_DATA",
                    Description = "Quest journal data is recommended for journal display",
                    Severity = "Warning",
                    Recommendation = "Add journal data for better quest journal experience"
                });
                if (result.Result == ValidationStatus.Valid)
                    result.Result = ValidationStatus.ValidWithWarnings;
                return;
            }

            // Validate journal category
            if (string.IsNullOrWhiteSpace(quest.JournalData.JournalCategory))
            {
                result.Issues.Add(new ValidationIssue
                {
                    Code = "MISSING_JOURNAL_CATEGORY",
                    Description = "Journal category is required for proper organization",
                    Severity = "Warning",
                    Recommendation = "Set journal category for quest organization"
                });
                if (result.Result == ValidationStatus.Valid)
                    result.Result = ValidationStatus.ValidWithWarnings;
            }

            // Validate journal entries
            if (quest.JournalData.JournalEntries.Count == 0 && quest.Status == QuestStatus.Active)
            {
                result.Issues.Add(new ValidationIssue
                {
                    Code = "NO_JOURNAL_ENTRIES",
                    Description = "Active quests should have journal entries",
                    Severity = "Info",
                    Recommendation = "Add journal entries for better player experience"
                });
            }
        }
    }
}
