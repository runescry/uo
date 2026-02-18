using System;
using System.Collections.Generic;
using System.Linq;
using Server.Mobiles;
using Server.Custom.VystiaClasses.Quests;
using Server.Engines.PartySystem;
using Server.Services.UnifiedQuestSystem;

namespace Server.Services.MultiplayerQuests
{
    /// <summary>
    /// Quest objective information for validation
    /// [OBSOLETE] Use UnifiedQuestData.CooperativeObjectives instead
    /// This class is maintained for backward compatibility during migration
    /// </summary>
    [Obsolete("Use UnifiedQuestData.CooperativeObjectives instead. This class will be removed in a future version.")]
    public class QuestObjectiveInfo
    {
        public string ObjectiveId { get; set; }
        public string Description { get; set; }
        public int RequiredCount { get; set; }
        public bool IsSinglePlayerOnly { get; set; }
        public bool RequiresSoloCompletion { get; set; }
        public string Type { get; set; }
        public Dictionary<string, object> Metadata { get; set; }

        // Migration methods
        /// <summary>
        /// Convert to UnifiedQuestData CooperativeObjectiveData format
        /// </summary>
        public CooperativeObjectiveData ToCooperativeObjective()
        {
            return new CooperativeObjectiveData
            {
                ObjectiveId = this.ObjectiveId,
                Description = this.Description,
                RequiredCount = this.RequiredCount,
                Type = ParseObjectiveType(this.Type),
                CurrentProgress = 0,
                IsCompleted = false
            };
        }

        /// <summary>
        /// Create from UnifiedQuestData CooperativeObjectiveData
        /// </summary>
        public static QuestObjectiveInfo FromCooperativeObjective(CooperativeObjectiveData cooperativeObjective)
        {
            if (cooperativeObjective == null)
                return null;

            return new QuestObjectiveInfo
            {
                ObjectiveId = cooperativeObjective.ObjectiveId,
                Description = cooperativeObjective.Description,
                RequiredCount = cooperativeObjective.RequiredCount,
                Type = cooperativeObjective.Type.ToString(),
                Metadata = cooperativeObjective.RoleContributions?.ToDictionary(kvp => kvp.Key, (object)kvp.Value) ?? new Dictionary<string, object>()
            };
        }

        private static CooperativeObjectiveType ParseObjectiveType(string type)
        {
            if (string.IsNullOrEmpty(type))
                return CooperativeObjectiveType.IndividualContribution;

            switch (type.ToLower())
            {
                case "individual":
                return CooperativeObjectiveType.IndividualContribution;
                case "group":
                    return CooperativeObjectiveType.GroupContribution;
                case "role":
                    return CooperativeObjectiveType.RoleBased;
                case "all":
                    return CooperativeObjectiveType.AllMembers;
                case "leader":
                    return CooperativeObjectiveType.LeaderOnly;
                default:
                    return CooperativeObjectiveType.IndividualContribution;
            }
        }
    }

    /// <summary>
    /// Quest reward information for validation
    /// </summary>
    public class QuestRewardInfo
    {
        public string RewardId { get; set; }
        public string Description { get; set; }
        public int Amount { get; set; }
        public bool IsSinglePlayerOnly { get; set; }
        public bool IsUnique { get; set; }
        public string Type { get; set; }
        public Dictionary<string, object> Metadata { get; set; }
    }

    /// <summary>
    /// Shared quest information for multiplayer parties
    /// [OBSOLETE] Use UnifiedQuestData instead
    /// This class is maintained for backward compatibility during migration
    /// </summary>
    [Obsolete("Use UnifiedQuestData instead. This class will be removed in a future version.")]
    public class SharedQuestInfo
    {
        public int QuestId { get; set; }
        public string QuestTitle { get; set; }
        public string QuestDescription { get; set; }
        public DateTime SharedAt { get; set; }
        public PlayerMobile SharedBy { get; set; }
        public Party Party { get; set; }
        public bool IsActive { get; set; }
        public bool IsCompleted { get; set; }
        public DateTime CompletedAt { get; set; }
        public List<PartyMemberProgress> MemberProgress { get; set; }
        public List<CooperativeObjective> CooperativeObjectives { get; set; }
        public SharedQuestSettings Settings { get; set; }

        // Migration methods
        /// <summary>
        /// Convert to UnifiedQuestData format
        /// </summary>
        public UnifiedQuestData ToUnified()
        {
            return DataMigrationUtilities.MigrateToUnified(this);
        }

        /// <summary>
        /// Create from UnifiedQuestData
        /// </summary>
        public static SharedQuestInfo FromUnified(UnifiedQuestData unifiedData)
        {
            if (unifiedData?.MultiplayerData == null)
                return null;

            var sharedQuestInfo = new SharedQuestInfo
            {
                QuestId = unifiedData.QuestId,
                QuestTitle = unifiedData.Title,
                QuestDescription = unifiedData.Description,
                SharedAt = unifiedData.CreatedAt,
                SharedBy = unifiedData.Creator,
                Party = unifiedData.Party,
                IsActive = unifiedData.Status == QuestStatus.Active,
                IsCompleted = unifiedData.Status == QuestStatus.Completed,
                CompletedAt = unifiedData.ProgressData.CompletedAt ?? DateTime.MinValue,
                Settings = unifiedData.MultiplayerData.Settings
            };

            // Convert cooperative objectives
            if (unifiedData.CooperativeObjectives != null)
            {
                sharedQuestInfo.CooperativeObjectives = unifiedData.CooperativeObjectives.Select(obj => new CooperativeObjective
                {
                    ObjectiveId = obj.ObjectiveId,
                    Description = obj.Description,
                    Type = obj.Type,
                    RequiredCount = obj.RequiredCount,
                    CurrentProgress = obj.CurrentProgress,
                    IsCompleted = obj.IsCompleted,
                    CompletedAt = obj.CompletedAt,
                    RequiredRoles = obj.RequiredRoles?.ToList() ?? new List<string>(),
                    RoleContributions = obj.RoleContributions?.ToDictionary(kvp => kvp.Key, kvp => kvp.Value) ?? new Dictionary<string, int>()
                }).ToList();
            }

            // Convert member progress
            if (unifiedData.PlayerProgress != null)
            {
                sharedQuestInfo.MemberProgress = unifiedData.PlayerProgress.Values.Select(progress => new PartyMemberProgress
                {
                    Member = progress.Player,
                    JoinedAt = progress.AcceptedAt,
                    HasAccepted = progress.HasAccepted,
                    AcceptedAt = progress.AcceptedAt,
                    HasCompleted = progress.HasCompleted,
                    CompletedAt = progress.CompletedAt,
                    ContributionScore = progress.ContributionScore,
                    ObjectiveProgress = progress.ObjectiveProgress?.ToDictionary(kvp => kvp.Key, kvp => kvp.Value) ?? new Dictionary<string, int>(),
                    ActionsPerformed = progress.ActionsPerformed?.ToList() ?? new List<string>()
                }).ToList();
            }

            return sharedQuestInfo;
        }
    }

    /// <summary>
    /// Individual party member's progress on a shared quest
    /// </summary>
    public class PartyMemberProgress
    {
        public PlayerMobile Member { get; set; }
        public DateTime JoinedAt { get; set; }
        public bool HasAccepted { get; set; }
        public DateTime AcceptedAt { get; set; }
        public Dictionary<string, int> ObjectiveProgress { get; set; }
        public bool HasCompleted { get; set; }
        public DateTime CompletedAt { get; set; }
        public int ContributionScore { get; set; }
        public List<string> ActionsPerformed { get; set; }
    }

    /// <summary>
    /// Cooperative objective that requires party coordination
    /// </summary>
    public class CooperativeObjective
    {
        public string ObjectiveId { get; set; }
        public string Description { get; set; }
        public CooperativeObjectiveType Type { get; set; }
        public int RequiredCount { get; set; }
        public int CurrentProgress { get; set; }
        public bool IsCompleted { get; set; }
        public DateTime CompletedAt { get; set; }
        public List<string> RequiredRoles { get; set; }
        public Dictionary<string, int> RoleContributions { get; set; }
        public CooperativeObjectiveSettings Settings { get; set; }
    }

    /// <summary>
    /// Types of cooperative objectives
    /// </summary>
    public enum CooperativeObjectiveType
    {
        IndividualContribution,  // Each member must contribute
        GroupContribution,      // Party as a whole must contribute
        RoleBased,             // Specific roles required
        MajorityVote,           // Majority must agree
        AllMembers,            // All members must complete
        LeaderOnly             // Only party leader can complete
    }

    /// <summary>
    /// Settings for shared quests
    /// </summary>
    public class SharedQuestSettings
    {
        public bool AllowLateJoiners { get; set; } = true;
        public bool RequireAllAcceptance { get; set; } = false;
        public bool ShareRewardsEqually { get; set; } = true;
        public bool AllowIndividualCompletion { get; set; } = false;
        public bool SyncProgress { get; set; } = true;
        public int MinimumPartySize { get; set; } = 2;
        public int MaximumPartySize { get; set; } = 8;
        public RewardDistributionMethod RewardMethod { get; set; } = RewardDistributionMethod.Equal;
        public TimeSpan AutoCompleteDelay { get; set; } = TimeSpan.FromMinutes(5);
    }

    /// <summary>
    /// Settings for cooperative objectives
    /// </summary>
    public class CooperativeObjectiveSettings
    {
        public bool RequireAllMembers { get; set; } = false;
        public bool AllowPartialCredit { get; set; } = true;
        public double MinimumContributionRatio { get; set; } = 0.5;
        public bool ShowProgressToAll { get; set; } = true;
        public bool AllowRoleSwitching { get; set; } = true;
        public TimeSpan ContributionTimeout { get; set; } = TimeSpan.FromMinutes(10);
    }

    /// <summary>
    /// Methods for distributing rewards among party members
    /// </summary>
    public enum RewardDistributionMethod
    {
        Equal,              // Everyone gets the same rewards
        ContributionBased,  // Based on individual contribution
        Random,             // Random selection of recipients
        LeaderBonus,        // Leader gets bonus, others equal
        RoleBased,          // Based on role requirements
        NeedBased           // Based on member needs/level
    }

    /// <summary>
    /// Party quest communication message
    /// </summary>
    public class QuestCommunication
    {
        public int SharedQuestId { get; set; }
        public PlayerMobile Sender { get; set; }
        public string Message { get; set; }
        public QuestMessageType Type { get; set; }
        public DateTime SentAt { get; set; }
        public List<PlayerMobile> Recipients { get; set; }
        public Dictionary<string, object> Metadata { get; set; }
    }

    /// <summary>
    /// Types of quest communication messages
    /// </summary>
    public enum QuestMessageType
    {
        QuestShared,
        QuestAccepted,
        QuestDeclined,
        ObjectiveProgress,
        ObjectiveCompleted,
        QuestCompleted,
        MemberJoined,
        MemberLeft,
        RoleAssignment,
        CoordinationRequest,
        StatusUpdate,
        RewardDistributed
    }

    /// <summary>
    /// Party quest validation result
    /// </summary>
    public class MultiplayerQuestValidation
    {
        public bool IsValid { get; set; }
        public List<string> Errors { get; set; }
        public List<string> Warnings { get; set; }
        public List<ValidationIssue> Issues { get; set; }
        public ValidationResult Severity { get; set; }
    }

    /// <summary>
    /// Individual validation issue
    /// </summary>
    public class ValidationIssue
    {
        public string Code { get; set; }
        public string Description { get; set; }
        public string MemberName { get; set; }
        public ValidationSeverity Severity { get; set; }
        public string Recommendation { get; set; }
    }

    /// <summary>
    /// Validation severity levels
    /// </summary>
    public enum ValidationSeverity
    {
        Info,
        Warning,
        Error,
        Critical
    }

    /// <summary>
    /// Overall validation result
    /// </summary>
    public enum ValidationResult
    {
        Valid,
        ValidWithWarnings,
        Invalid,
        Critical
    }

    /// <summary>
    /// Party quest statistics
    /// </summary>
    public class PartyQuestStatistics
    {
        public int TotalSharedQuests { get; set; }
        public int CompletedQuests { get; set; }
        public int ActiveQuests { get; set; }
        public double CompletionRate { get; set; }
        public TimeSpan AverageCompletionTime { get; set; }
        public Dictionary<string, int> ObjectiveTypeCounts { get; set; }
        public Dictionary<RewardDistributionMethod, int> RewardMethodUsage { get; set; }
        public List<PlayerMobile> MostActiveMembers { get; set; }
        public DateTime LastActivity { get; set; }
    }
}
