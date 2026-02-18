using System;
using System.Collections.Generic;
using System.Linq;
using Server.Mobiles;
using Server.Custom.VystiaClasses.Quests;
using Server.Engines.PartySystem;
using Server.Engines.Quests;

namespace Server.Services.UnifiedQuestSystem
{
    /// <summary>
    /// Unified quest data model that consolidates all quest information across systems
    /// Replaces QuestSimilarityMetrics, SharedQuestInfo, QuestObjectiveInfo, and other quest data models
    /// </summary>
    public class UnifiedQuestData
    {
        // Core Quest Information
        public int QuestId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public QuestType Type { get; set; }
        public QuestStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public PlayerMobile Owner { get; set; }
        public PlayerMobile Creator { get; set; }

        // Quest Content Information
        public string Theme { get; set; }
        public string Location { get; set; }
        public int DifficultyLevel { get; set; }
        public TimeSpan TimeLimit { get; set; }
        public List<string> Tags { get; set; }
        public Dictionary<string, object> Metadata { get; set; }

        // Variety System Data
        public QuestSimilarityData SimilarityData { get; set; }
        public QuestQualityData QualityData { get; set; }
        public QuestThemeData ThemeData { get; set; }
        public PlayerPreferenceData PreferenceData { get; set; }

        // Multiplayer System Data
        public SharedQuestData MultiplayerData { get; set; }
        public List<CooperativeObjectiveData> CooperativeObjectives { get; set; }
        public Party Party { get; set; }

        // Progress Tracking Data
        public QuestProgressData ProgressData { get; set; }
        public Dictionary<Serial, PlayerProgressData> PlayerProgress { get; set; }

        // Persistence Data
        public QuestPersistenceData PersistenceData { get; set; }
        public bool IsPersistent { get; set; }
        public DateTime LastPersisted { get; set; }

        // Journal Data
        public QuestJournalData JournalData { get; set; }
        public bool IsInJournal { get; set; }
        public DateTime AddedToJournal { get; set; }

        // Validation Data
        public QuestValidationData ValidationData { get; set; }
        public bool IsValid { get; set; }
        public List<string> ValidationErrors { get; set; }

        // Communication Data
        public QuestCommunicationData CommunicationData { get; set; }
        public List<QuestMessage> MessageHistory { get; set; }

        // Constructor
        public UnifiedQuestData()
        {
            // Initialize collections
            Tags = new List<string>();
            Metadata = new Dictionary<string, object>();
            CooperativeObjectives = new List<CooperativeObjectiveData>();
            PlayerProgress = new Dictionary<Serial, PlayerProgressData>();
            ValidationErrors = new List<string>();
            MessageHistory = new List<QuestMessage>();

            // Initialize sub-data objects
            SimilarityData = new QuestSimilarityData();
            QualityData = new QuestQualityData();
            ThemeData = new QuestThemeData();
            PreferenceData = new PlayerPreferenceData();
            MultiplayerData = new SharedQuestData();
            ProgressData = new QuestProgressData();
            PersistenceData = new QuestPersistenceData();
            JournalData = new QuestJournalData();
            ValidationData = new QuestValidationData();
            CommunicationData = new QuestCommunicationData();

            // Set defaults
            Status = QuestStatus.Created;
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
            IsPersistent = true;
            IsValid = true;
        }

        // Helper Methods
        public bool IsMultiplayer => MultiplayerData.IsActive;
        public bool IsCompleted => Status == QuestStatus.Completed;
        public bool IsActive => Status == QuestStatus.Active;
        public bool HasTimeLimit => TimeLimit > TimeSpan.Zero;
        public int ParticipantCount => PlayerProgress.Count;

        public void UpdateTimestamp()
        {
            UpdatedAt = DateTime.UtcNow;
        }

        public void AddValidationError(string error)
        {
            ValidationErrors.Add(error);
            IsValid = false;
        }

        public void ClearValidationErrors()
        {
            ValidationErrors.Clear();
            IsValid = true;
        }
    }

    /// <summary>
    /// Quest similarity data from variety system
    /// </summary>
    public class QuestSimilarityData
    {
        public double SimilarityScore { get; set; }
        public Dictionary<string, double> FeatureVector { get; set; }
        public List<string> SimilarQuests { get; set; }
        public bool WouldBeRepetitive { get; set; }
        public double RepetitionThreshold { get; set; }

        public QuestSimilarityData()
        {
            FeatureVector = new Dictionary<string, double>();
            SimilarQuests = new List<string>();
            RepetitionThreshold = 0.7;
        }
    }

    /// <summary>
    /// Quest quality data from variety system
    /// </summary>
    public class QuestQualityData
    {
        public double OverallScore { get; set; }
        public double OriginalityScore { get; set; }
        public double ComplexityScore { get; set; }
        public double EngagementScore { get; set; }
        public double CoherenceScore { get; set; }
        public List<string> QualityIssues { get; set; }
        public List<string> ImprovementSuggestions { get; set; }
        public bool MeetsMinimumQuality { get; set; }

        public QuestQualityData()
        {
            QualityIssues = new List<string>();
            ImprovementSuggestions = new List<string>();
            MeetsMinimumQuality = true;
        }
    }

    /// <summary>
    /// Quest theme data from variety system
    /// </summary>
    public class QuestThemeData
    {
        public string ThemeId { get; set; }
        public string ThemeName { get; set; }
        public double ThemeWeight { get; set; }
        public bool IsActive { get; set; }
        public DateTime LastUsed { get; set; }
        public TimeSpan CooldownRemaining { get; set; }

        public QuestThemeData()
        {
            IsActive = true;
            ThemeWeight = 1.0;
        }
    }

    /// <summary>
    /// Player preference data from variety system
    /// </summary>
    public class PlayerPreferenceData
    {
        public Dictionary<string, double> ThemePreferences { get; set; }
        public Dictionary<string, double> DifficultyPreferences { get; set; }
        public Dictionary<string, double> LocationPreferences { get; set; }
        public Dictionary<string, double> QuestTypePreferences { get; set; }
        public double PreferenceScore { get; set; }
        public bool MatchesPlayerPreferences { get; set; }

        public PlayerPreferenceData()
        {
            ThemePreferences = new Dictionary<string, double>();
            DifficultyPreferences = new Dictionary<string, double>();
            LocationPreferences = new Dictionary<string, double>();
            QuestTypePreferences = new Dictionary<string, double>();
            PreferenceScore = 0.0;
            MatchesPlayerPreferences = true;
        }
    }

    /// <summary>
    /// Shared quest data from multiplayer system
    /// </summary>
    public class SharedQuestData
    {
        public bool IsActive { get; set; }
        public bool IsShared { get; set; }
        public PlayerMobile SharedBy { get; set; }
        public DateTime SharedAt { get; set; }
        public Party Party { get; set; }
        public SharedQuestSettings Settings { get; set; }
        public bool RequiresAllAcceptance { get; set; }
        public bool AllowLateJoiners { get; set; }

        public SharedQuestData()
        {
            Settings = new SharedQuestSettings();
            RequiresAllAcceptance = false;
            AllowLateJoiners = true;
        }
    }

    /// <summary>
    /// Cooperative objective data from multiplayer system
    /// </summary>
    public class CooperativeObjectiveData
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

        public CooperativeObjectiveData()
        {
            RequiredRoles = new List<string>();
            RoleContributions = new Dictionary<string, int>();
        }
    }

    /// <summary>
    /// Quest progress data for unified tracking
    /// </summary>
    public class QuestProgressData
    {
        public double OverallProgress { get; set; }
        public int CompletedObjectives { get; set; }
        public int TotalObjectives { get; set; }
        public DateTime StartedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public TimeSpan TimeSpent { get; set; }
        public Dictionary<string, int> ObjectiveProgress { get; set; }
        public List<ProgressEvent> ProgressHistory { get; set; }

        public QuestProgressData()
        {
            ObjectiveProgress = new Dictionary<string, int>();
            ProgressHistory = new List<ProgressEvent>();
            StartedAt = DateTime.UtcNow;
        }

        public void UpdateProgress()
        {
            if (TotalObjectives > 0)
            {
                OverallProgress = (double)CompletedObjectives / TotalObjectives;
            }
            
            if (CompletedAt.HasValue)
            {
                TimeSpent = CompletedAt.Value - StartedAt;
            }
            else
            {
                TimeSpent = DateTime.UtcNow - StartedAt;
            }
        }
    }

    /// <summary>
    /// Individual player progress data
    /// </summary>
    public class PlayerProgressData
    {
        public PlayerMobile Player { get; set; }
        public bool HasAccepted { get; set; }
        public DateTime AcceptedAt { get; set; }
        public bool HasCompleted { get; set; }
        public DateTime? CompletedAt { get; set; }
        public int ContributionScore { get; set; }
        public Dictionary<string, int> ObjectiveProgress { get; set; }
        public List<string> ActionsPerformed { get; set; }
        public DateTime LastActivity { get; set; }

        public PlayerProgressData()
        {
            ObjectiveProgress = new Dictionary<string, int>();
            ActionsPerformed = new List<string>();
            LastActivity = DateTime.UtcNow;
        }
    }

    /// <summary>
    /// Quest persistence data
    /// </summary>
    public class QuestPersistenceData
    {
        public bool IsPersisted { get; set; }
        public DateTime LastSaved { get; set; }
        public string SaveVersion { get; set; }
        public Dictionary<string, object> SerializedData { get; set; }
        public List<string> BackupLocations { get; set; }

        public QuestPersistenceData()
        {
            SerializedData = new Dictionary<string, object>();
            BackupLocations = new List<string>();
            SaveVersion = "1.0";
        }
    }

    /// <summary>
    /// Quest journal data
    /// </summary>
    public class QuestJournalData
    {
        public bool IsVisible { get; set; }
        public QuestDisplayPriority Priority { get; set; }
        public string JournalCategory { get; set; }
        public List<QuestJournalEntry> JournalEntries { get; set; }
        public DateTime LastViewed { get; set; }

        public QuestJournalData()
        {
            JournalEntries = new List<QuestJournalEntry>();
            Priority = QuestDisplayPriority.Normal;
            JournalCategory = "General";
            IsVisible = true;
        }
    }

    /// <summary>
    /// Quest validation data
    /// </summary>
    public class QuestValidationData
    {
        public ValidationResult Result { get; set; }
        public DateTime LastValidated { get; set; }
        public List<ValidationIssue> Issues { get; set; }
        public string ValidatorVersion { get; set; }
        public Dictionary<string, object> ValidationMetadata { get; set; }

        public QuestValidationData()
        {
            Result = ValidationResult.Valid;
            Issues = new List<ValidationIssue>();
            ValidatorVersion = "1.0";
            ValidationMetadata = new Dictionary<string, object>();
        }
    }

    /// <summary>
    /// Quest communication data
    /// </summary>
    public class QuestCommunicationData
    {
        public bool CommunicationEnabled { get; set; }
        public List<QuestMessageType> EnabledMessageTypes { get; set; }
        public Dictionary<Serial, DateTime> LastCommunication { get; set; }
        public int MessageCount { get; set; }

        public QuestCommunicationData()
        {
            EnabledMessageTypes = new List<QuestMessageType>();
            LastCommunication = new Dictionary<Serial, DateTime>();
            CommunicationEnabled = true;
        }
    }

    // Supporting Enums and Classes

    public enum QuestType
    {
        Dynamic,
        Vystia,
        Traditional,
        Mondain,
        Heritage,
        Multiplayer
    }

    public enum QuestStatus
    {
        Created,
        Active,
        Completed,
        Failed,
        Expired,
        Cancelled
    }

    public enum CooperativeObjectiveType
    {
        IndividualContribution,
        GroupContribution,
        RoleBased,
        MajorityVote,
        AllMembers,
        LeaderOnly
    }

    public enum QuestDisplayPriority
    {
        Low,
        Normal,
        High,
        Critical
    }

    public enum ValidationResult
    {
        Valid,
        ValidWithWarnings,
        Invalid,
        Critical
    }

    public class SharedQuestSettings
    {
        public bool AllowLateJoiners { get; set; } = true;
        public bool RequireAllAcceptance { get; set; } = false;
        public bool ShareRewardsEqually { get; set; } = true;
        public bool AllowIndividualCompletion { get; set; } = false;
        public bool SyncProgress { get; set; } = true;
        public int MinimumPartySize { get; set; } = 2;
        public int MaximumPartySize { get; set; } = 8;
    }

    public class QuestJournalEntry
    {
        public DateTime Timestamp { get; set; }
        public string EntryType { get; set; }
        public string Message { get; set; }
        public PlayerMobile Player { get; set; }

        public QuestJournalEntry()
        {
            Timestamp = DateTime.UtcNow;
        }
    }

    public class ValidationIssue
    {
        public string Code { get; set; }
        public string Description { get; set; }
        public string Severity { get; set; }
        public string Recommendation { get; set; }

        public ValidationIssue()
        {
            Severity = "Warning";
        }
    }

    public class QuestMessage
    {
        public DateTime Timestamp { get; set; }
        public QuestMessageType Type { get; set; }
        public PlayerMobile Sender { get; set; }
        public string Content { get; set; }
        public List<PlayerMobile> Recipients { get; set; }

        public QuestMessage()
        {
            Timestamp = DateTime.UtcNow;
            Recipients = new List<PlayerMobile>();
        }
    }

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
}
