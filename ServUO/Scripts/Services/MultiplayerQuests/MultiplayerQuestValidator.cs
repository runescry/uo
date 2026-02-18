using System;
using System.Collections.Generic;
using System.Linq;
using Server.Mobiles;
using Server.Custom.VystiaClasses.Quests;
using Server.Engines.PartySystem;
using Server.Engines.Quests;

namespace Server.Services.MultiplayerQuests
{
    /// <summary>
    /// Validates multiplayer quests for consistency and compliance
    /// </summary>
    public static class MultiplayerQuestValidator
    {
        private static readonly Dictionary<string, IQuestValidator> s_Validators = new Dictionary<string, IQuestValidator>();
        private static readonly object s_Lock = new object();

        /// <summary>
        /// Initialize the multiplayer quest validator
        /// </summary>
        public static void Initialize()
        {
            // Register quest validators
            RegisterQuestValidator("dynamic", new DynamicQuestValidator());
            RegisterQuestValidator("vystia", new VystiaQuestValidator());
            RegisterQuestValidator("traditional", new TraditionalQuestValidator());

            Console.WriteLine("[MultiplayerQuestValidator] Initialized with {s_Validators.Count} quest validators");
        }

        /// <summary>
        /// Register a quest validator
        /// </summary>
        public static void RegisterQuestValidator(string questType, IQuestValidator validator)
        {
            lock (s_Lock)
            {
                s_Validators[questType] = validator;
            }
        }

        /// <summary>
        /// Validate a quest for multiplayer compatibility
        /// </summary>
        public static MultiplayerQuestValidation ValidateQuestForMultiplayer(object quest, Party party, SharedQuestSettings settings = null)
        {
            if (quest == null || party == null)
            {
                return new MultiplayerQuestValidation
                {
                    IsValid = false,
                    Errors = new List<string> { "Quest or party is null" },
                    Severity = ValidationResult.Critical
                };
            }

            var validation = new MultiplayerQuestValidation
            {
                IsValid = true,
                Errors = new List<string>(),
                Warnings = new List<string>(),
                Issues = new List<ValidationIssue>(),
                Severity = ValidationResult.Valid
            };

            // Get quest type
            string questType = GetQuestType(quest);
            var validator = s_Validators.GetValueOrDefault(questType);
            
            if (validator == null)
            {
                validation.Errors.Add($"No validator found for quest type: {questType}");
                validation.Severity = ValidationResult.Critical;
                validation.IsValid = false;
                return validation;
            }

            // Perform validation checks
            ValidatePartySize(party, settings, validation);
            ValidateQuestType(quest, validation);
            ValidateQuestDifficulty(quest, party, validation);
            ValidateQuestObjectives(quest, validation);
            ValidateQuestRewards(quest, validation);
            ValidateQuestLocation(quest, party, validation);
            ValidateQuestTimeLimit(quest, validation);
            ValidatePartyMemberRequirements(party, validation);
            ValidateQuestState(quest, validation);

            // Check for specific multiplayer issues
            CheckMultiplayerCompatibility(quest, party, validation);

            // Determine overall result
            validation.Severity = DetermineValidationSeverity(validation);

            return validation;
        }

        /// <summary>
        /// Validate party size requirements
        /// </summary>
        private static void ValidatePartySize(Party party, SharedQuestSettings settings, MultiplayerQuestValidation validation)
        {
            int partySize = party.Members.Count;
            int minSize = settings?.MinimumPartySize ?? 2;
            int maxSize = settings?.MaximumPartySize ?? 8;

            if (partySize < minSize)
            {
                validation.Errors.Add($"Party size {partySize} is below minimum requirement of {minSize}");
                validation.Issues.Add(new ValidationIssue
                {
                    Code = "PARTY_SIZE_TOO_SMALL",
                    Description = $"Party has {partySize} members but requires at least {minSize}",
                    Severity = ValidationSeverity.Error,
                    Recommendation = "Add more party members or adjust minimum party size settings"
                });
            }

            if (partySize > maxSize)
            {
                validation.Errors.Add($"Party size {partySize} exceeds maximum limit of {maxSize}");
                validation.Issues.Add(new ValidationIssue
                {
                    Code = "PARTY_SIZE_TOO_LARGE",
                    Description = $"Party has {partySize} members but maximum allowed is {maxSize}",
                    Severity = ValidationSeverity.Error,
                    Recommendation = "Remove party members or adjust maximum party size settings"
                });
            }

            // Check for inactive members
            var inactiveMembers = party.Members.Count(m => m.Mobile == null || m.Mobile.Deleted);
            if (inactiveMembers > 0)
            {
                validation.Warnings.Add($"Party has {inactiveMembers} inactive members");
                validation.Issues.Add(new ValidationIssue
                {
                    Code = "INACTIVE_PARTY_MEMBERS",
                    Description = $"Party has {inactiveMembers} inactive or deleted members",
                    Severity = ValidationSeverity.Warning,
                    Recommendation = "Remove inactive party members"
                });
            }
        }

        /// <summary>
        /// Validate quest type compatibility
        /// </summary>
        private static void ValidateQuestType(object quest, MultiplayerQuestValidation validation)
        {
            string questType = GetQuestType(quest);
            
            // Check if quest type supports multiplayer
            var supportedTypes = new[] { "dynamic", "vystia", "traditional" };
            
            if (!supportedTypes.Contains(questType))
            {
                validation.Errors.Add($"Quest type '{questType}' is not supported for multiplayer");
                validation.Issues.Add(new ValidationIssue
                {
                    Code = "UNSUPPORTED_QUEST_TYPE",
                    Description = $"Quest type {questType} does not support multiplayer features",
                    Severity = ValidationSeverity.Critical,
                    Recommendation = "Use a supported quest type or implement multiplayer support"
                });
            }
        }

        /// <summary>
        /// Validate quest difficulty for party
        /// </summary>
        private static void ValidateQuestDifficulty(object quest, Party party, MultiplayerQuestValidation validation)
        {
            // This would be implemented based on the quest's difficulty
            // For now, assume all difficulties are supported
            
            // Check if difficulty is appropriate for party size
            int partySize = party.Members.Count;
            
            // High difficulty quests with large parties might be too easy
            // This would need to be implemented based on actual quest difficulty data
        }

        /// <summary>
        /// Validate quest objectives for multiplayer
        /// </summary>
        private static void ValidateQuestObjectives(object quest, MultiplayerQuestValidation validation)
        {
            // Get quest objectives
            var objectives = GetQuestObjectives(quest);
            
            if (objectives == null || !objectives.Any())
            {
                validation.Errors.Add("Quest has no objectives");
                validation.Issues.Add(new ValidationIssue
                {
                    Code = "NO_OBJECTIVES",
                    Description = "Quest must have at least one objective",
                    Severity = ValidationSeverity.Error,
                    Recommendation = "Add objectives to the quest"
                });
                return;
            }

            // Check if objectives are suitable for multiplayer
            foreach (var objective in objectives)
            {
                if (objective.IsSinglePlayerOnly)
                {
                    validation.Warnings.Add($"Objective '{objective.Description}' is single-player only");
                    validation.Issues.Add(new ValidationIssue
                    {
                        Code = "SINGLE_PLAYER_OBJECTIVE",
                        Description = $"Objective '{objective.Description}' cannot be completed by multiple players",
                        Severity = ValidationSeverity.Warning,
                        Recommendation = "Modify objective to support multiplayer or mark as optional"
                    });
                }

                if (objective.RequiresSoloCompletion)
                {
                    validation.Warnings.Add($"Objective '{objective.Description}' requires solo completion");
                    validation.Issues.Add(new ValidationIssue
                    {
                        Code = "SOLO_COMPLETION_REQUIRED",
                        Description = $"Objective '{objective.Description}' must be completed by a single player",
                        Severity = ValidationSeverity.Warning,
                        Recommendation = "Allow solo completion or modify objective for group play"
                    });
                }
            }
        }

        /// <summary>
        /// Validate quest rewards for multiplayer
        /// </summary>
        private static void ValidateQuestRewards(object quest, MultiplayerQuestValidation validation)
        {
            // Get quest rewards
            var rewards = GetQuestRewards(quest);
            
            if (rewards == null || !rewards.Any())
            {
                validation.Warnings.Add("Quest has no rewards");
                validation.Issues.Add(new ValidationIssue
                {
                    Code = "NO_REWARDS",
                    Description = "Quest has no rewards to distribute",
                    Severity = ValidationSeverity.Warning,
                    Recommendation = "Add rewards to the quest"
                });
                return;
            }

            // Check if rewards are suitable for distribution
            foreach (var reward in rewards)
            {
                if (reward.IsSinglePlayerOnly)
                {
                    validation.Warnings.Add($"Reward '{reward.Description}' is single-player only");
                    validation.Issues.Add(new ValidationIssue
                    {
                        Code = "SINGLE_PLAYER_REWARD",
                        Description = $"Reward '{reward.Description}' cannot be distributed to multiple players",
                        Severity = ValidationSeverity.Warning,
                        Recommendation = "Modify reward to support multiplayer or mark as optional"
                    });
                }

                if (reward.IsUnique && party.Members.Count > 1)
                {
                    validation.Warnings.Add($"Unique reward '{reward.Description}' with multiple players");
                    validation.Issues.Add(new ValidationIssue
                    {
                        Code = "UNIQUE_REWARD_MULTIPLE_PLAYERS",
                        Description = $"Unique reward '{reward.Description}' cannot be given to multiple players",
                        Severity = ValidationSeverity.Warning,
                        Recommendation = "Add multiple unique rewards or use random distribution"
                    });
                }
            }
        }

        /// <summary>
        /// Validate quest location for party
        /// </summary>
        private static void ValidateQuestLocation(object quest, Party party, MultiplayerQuestValidation validation)
        {
            // Check if quest location can accommodate party
            var location = GetQuestLocation(quest);
            
            if (string.IsNullOrEmpty(location))
            {
                validation.Warnings.Add("Quest has no specified location");
                validation.Issues.Add(new ValidationIssue
                {
                    Code = "NO_LOCATION",
                    Description = "Quest location is not specified",
                    Severity = ValidationSeverity.Info,
                    Recommendation = "Add a location to the quest"
                });
                return;
            }

            // Check if location is suitable for party size
            int partySize = party.Members.Count;
            
            // Small locations might be crowded for large parties
            if (partySize > 6 && IsSmallLocation(location))
            {
                validation.Warnings.Add($"Location '{location}' might be crowded for {partySize} players");
                validation.Issues.Add(new ValidationIssue
                {
                    Code = "CROWDED_LOCATION",
                    Description = $"Location '{location}' may be too small for {partySize} players",
                    Severity = ValidationSeverity.Warning,
                    Recommendation = "Consider a larger location or reduce party size"
                });
            }
        }

        /// <summary>
        /// Validate quest time limit for party
        /// </summary>
        private static void ValidateQuestTimeLimit(object quest, MultiplayerQuestValidation validation)
        {
            // Get quest time limit
            var timeLimit = GetQuestTimeLimit(quest);
            
            if (timeLimit == TimeSpan.Zero)
            {
                return; // No time limit is fine
            }

            // Check if time limit is reasonable for party
            int partySize = party.Members.Count;
            TimeSpan adjustedTimeLimit = timeLimit;
            
            // Larger parties might need more time
            if (partySize > 4)
            {
                adjustedTimeLimit = TimeSpan.FromMinutes(timeLimit.TotalMinutes * 1.5);
            }

            if (adjustedTimeLimit > TimeSpan.FromHours(24))
            {
                validation.Warnings.Add($"Time limit {timeLimit.TotalMinutes} minutes might be too short for {partySize} players");
                validation.Issues.Add(new ValidationIssue
                {
                    Code = "SHORT_TIME_LIMIT",
                    Description = $"Time limit {timeLimit.TotalMinutes} minutes may be insufficient for {partySize} players",
                    Severity = ValidationSeverity.Warning,
                    Recommendation = "Extend time limit or reduce party size"
                });
            }
        }

        /// <summary>
        /// Validate party member requirements
        /// </summary>
        private static void ValidatePartyMemberRequirements(Party party, MultiplayerQuestValidation validation)
        {
            // Check if all party members meet requirements
            foreach (var member in party.Members)
            {
                if (member.Mobile == null || member.Mobile.Deleted)
                {
                    continue; // Skip invalid members
                }

                // Check member level requirements
                var memberLevel = member.Mobile.Level;
                if (memberLevel < 10)
                {
                    validation.Warnings.Add($"Party member {member.Mobile.Name} is level {memberLevel} (minimum recommended: 10)");
                    validation.Issues.Add(new ValidationIssue
                    {
                        Code = "LOW_MEMBER_LEVEL",
                        Description = $"Party member {member.Mobile.Name} is level {memberLevel}",
                        Severity = ValidationSeverity.Warning,
                        Recommendation = "Ensure all party members meet minimum level requirements"
                    });
                }

                // Check member skill requirements
                // This would be implemented based on quest requirements
            }
        }

        /// <summary>
        /// Validate quest state
        /// </summary>
        private static void ValidateQuestState(object quest, MultiplayerQuestValidation validation)
        {
            // Check if quest is in a state that supports sharing
            if (IsQuestCompleted(quest))
            {
                validation.Errors.Add("Quest is already completed");
                validation.Issues.Add(new ValidationIssue
                {
                    Code = "QUEST_ALREADY_COMPLETED",
                    Description = "Quest cannot be shared as it is already completed",
                    Severity = ValidationSeverity.Error,
                    Recommendation = "Share quests before completion"
                });
            }

            if (IsQuestExpired(quest))
            {
                validation.Errors.Add("Quest has expired");
                validation.Issues.Add(new ValidationIssue
                {
                    Code = "QUEST_EXPIRED",
                    Description = "Quest cannot be shared as it has expired",
                    Severity = ValidationSeverity.Error,
                    Recommendation = "Share quests before expiration or extend time limit"
                });
            }

            if (IsQuestFailed(quest))
            {
                validation.Errors.Add("Quest has failed");
                validation.Issues.Add(new ValidationIssue
                {
                    Code = "QUEST_FAILED",
                    Description = "Quest cannot be shared as it has failed",
                    Severity = ValidationSeverity.Error,
                    Recommendation = "Reset quest state or create a new quest"
                });
            }
        }

        /// <summary>
        /// Check for specific multiplayer compatibility issues
        /// </summary>
        private static void CheckMultiplayerCompatibility(object quest, Party party, MultiplayerQuestValidation validation)
        {
            // Check for quest features that don't work well with multiplayer
            var incompatibleFeatures = GetIncompatibleFeatures(quest);
            
            foreach (var feature in incompatibleFeatures)
            {
                validation.Warnings.Add($"Quest feature '{feature}' may not work well with multiplayer");
                validation.Issues.Add(new ValidationIssue
                {
                    Code = "INCOMPATIBLE_FEATURE",
                    Description = $"Feature '{feature}' may have issues in multiplayer",
                    Severity = ValidationSeverity.Warning,
                    Recommendation = "Test feature thoroughly or disable for multiplayer quests"
                });
            }
        }

        /// <summary>
        /// Determine validation severity
        /// </summary>
        private static ValidationResult DetermineValidationSeverity(MultiplayerQuestValidation validation)
        {
            if (validation.Errors.Any())
            {
                return ValidationResult.Critical;
            }
            else if (validation.Warnings.Any())
            {
                return ValidationResult.ValidWithWarnings;
            }
            else if (validation.Issues.Any(i => i.Severity == ValidationSeverity.Error))
            {
                return ValidationResult.Invalid;
            }
            
            return ValidationResult.Valid;
        }

        /// <summary>
        /// Get quest type
        /// </summary>
        private static string GetQuestType(object quest)
        {
            if (quest is DynamicQuest)
                return "dynamic";
            
            if (quest is VystiaQuest)
                return "vystia";
            
            if (quest is BaseQuest)
                return "traditional";
            
            return "unknown";
        }

        /// <summary>
        /// Get quest objectives
        /// </summary>
        private static List<QuestObjectiveInfo> GetQuestObjectives(object quest)
        {
            var objectives = new List<QuestObjectiveInfo>();
            
            // This would be implemented based on the specific quest system
            // For now, return empty list
            
            return objectives;
        }

        /// <summary>
        /// Get quest rewards
        /// </summary>
        private static List<QuestRewardInfo> GetQuestRewards(object quest)
        {
            var rewards = new List<QuestRewardInfo>();
            
            // This would be implemented based on the specific quest system
            // For now, return empty list
            
            return rewards;
        }

        /// <summary>
        /// Get quest location
        /// </summary>
        private static string GetQuestLocation(object quest)
        {
            // This would be implemented based on the specific quest system
            return "Unknown location";
        }

        /// <summary>
        /// Get quest time limit
        /// </summary>
        private static TimeSpan GetQuestTimeLimit(object quest)
        {
            // This would be implemented based on the specific quest system
            return TimeSpan.Zero;
        }

        /// <summary>
        /// Check if quest is completed
        /// </summary>
        private static bool IsQuestCompleted(object quest)
        {
            // This would be implemented based on the specific quest system
            return false;
        }

        /// <summary>
        /// Check if quest is expired
        /// </summary>
        private static bool IsQuestExpired(object quest)
        {
            // This would be implemented based on the specific quest system
            return false;
        }

        /// <summary>
        /// Check if quest has failed
        /// </summary>
        private static bool IsQuestFailed(object quest)
        {
            // This would be implemented based on the specific quest system
            return false;
        }

        /// <summary>
        /// Get incompatible features
        /// </summary>
        private static List<string> GetIncompatibleFeatures(object quest)
        {
            var features = new List<string>();
            
            // This would be implemented based on the specific quest system
            // For now, return empty list
            
            return features;
        }

        /// <summary>
        /// Check if location is small
        /// </summary>
        private static bool IsSmallLocation(string location)
        {
            // This would be implemented based on location data
            return false;
        }
    }

    /// <summary>
    /// Interface for quest validators
    /// </summary>
    public interface IQuestValidator
    {
        bool CanBeShared(object quest, Party party);
        List<string> GetValidationWarnings(object quest, Party party);
        bool SupportsMultiplayer(object quest);
    }

    /// <summary>
    /// Base quest validator
    /// </summary>
    public abstract class BaseQuestValidator : IQuestValidator
    {
        public virtual bool CanBeShared(object quest, Party party)
        {
            return SupportsMultiplayer(quest) && party != null && party.Members.Count >= 2;
        }

        public virtual List<string> GetValidationWarnings(object quest, Party party)
        {
            return new List<string>();
        }

        public abstract bool SupportsMultiplayer(object quest);
    }

    /// <summary>
    /// Dynamic quest validator
    /// </summary>
    public class DynamicQuestValidator : BaseQuestValidator
    {
        public override bool SupportsMultiplayer(object quest)
        {
            return quest is DynamicQuest;
        }

        public override List<string> GetValidationWarnings(object quest, Party party)
        {
            var warnings = new List<string>();
            
            // Dynamic quests generally support multiplayer
            // Add any specific warnings for dynamic quests here
            
            return warnings;
        }
    }

    /// <summary>
    /// Vystia quest validator
    /// </summary>
    public class VystiaQuestValidator : BaseQuestValidator
    {
        public override bool SupportsMultiplayer(object quest)
        {
            return quest is VystiaQuest;
        }

        public override List<string> GetValidationWarnings(object quest, Party party)
        {
            var warnings = new List<string>();
            
            // Vystia quests may have specific multiplayer limitations
            // Add any specific warnings for Vystia quests here
            
            return warnings;
        }
    }

    /// <summary>
    /// Traditional quest validator
    /// </summary>
    public class TraditionalQuestValidator : BaseQuestValidator
    {
        public override bool SupportsMultiplayer(object quest)
        {
            return quest is BaseQuest;
        }

        public override List<string> GetValidationWarnings(object quest, Party party)
        {
            var warnings = new List<string>();
            
            // Traditional quests may not be designed for multiplayer
            warnings.Add("Traditional quests may not support multiplayer features");
            warnings.Add("Consider using dynamic or Vystia quests for better multiplayer support");
            
            return warnings;
        }
    }
}
