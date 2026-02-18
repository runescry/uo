using System;
using System.Collections.Generic;
using System.Linq;
using Server.Mobiles;
using Server.Custom.VystiaClasses.Quests;
using Server.Services.QuestVariety;
using Server.Services.MultiplayerQuests;
using Server.Services.QuestJournal;
using Server.Services.QuestPersistence;

namespace Server.Services.UnifiedQuestSystem
{
    /// <summary>
    /// Data migration utilities for converting existing quest data models to unified format
    /// Handles backward compatibility and seamless data migration
    /// </summary>
    public static class DataMigrationUtilities
    {
        private static readonly Dictionary<Type, Func<object, UnifiedQuestData>> s_MigrationStrategies;
        private static readonly object s_Lock = new object();
        private static int s_MigrationCount = 0;
        private static int s_ErrorCount = 0;

        static DataMigrationUtilities()
        {
            s_MigrationStrategies = new Dictionary<Type, Func<object, UnifiedQuestData>>
            {
                { typeof(DynamicQuest), MigrateDynamicQuest },
                { typeof(VystiaQuest), MigrateVystiaQuest },
                { typeof(BaseQuest), MigrateBaseQuest },
                { typeof(SharedQuestInfo), MigrateSharedQuestInfo },
                { typeof(QuestSimilarityMetrics), MigrateQuestSimilarityMetrics }
            };
        }

        /// <summary>
        /// Migrate any quest data object to unified format
        /// </summary>
        public static UnifiedQuestData MigrateToUnified(object sourceData)
        {
            if (sourceData == null)
                return null;

            lock (s_Lock)
            {
                try
                {
                    var sourceType = sourceData.GetType();
                    
                    // Check if we have a direct migration strategy
                    if (s_MigrationStrategies.TryGetValue(sourceType, out var migrationStrategy))
                    {
                        var result = migrationStrategy(sourceData);
                        s_MigrationCount++;
                        return result;
                    }

                    // Try to find a compatible migration strategy
                    var compatibleStrategy = FindCompatibleMigrationStrategy(sourceType);
                    if (compatibleStrategy != null)
                    {
                        var result = compatibleStrategy(sourceData);
                        s_MigrationCount++;
                        return result;
                    }

                    Console.WriteLine($"[DataMigration] No migration strategy found for type: {sourceType.Name}");
                    s_ErrorCount++;
                    return CreateDefaultUnifiedQuestData(sourceData);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[DataMigration] Error migrating {sourceData.GetType().Name}: {ex.Message}");
                    s_ErrorCount++;
                    return CreateDefaultUnifiedQuestData(sourceData);
                }
            }
        }

        /// <summary>
        /// Migrate DynamicQuest to UnifiedQuestData
        /// </summary>
        private static UnifiedQuestData MigrateDynamicQuest(object source)
        {
            var dynamicQuest = source as DynamicQuest;
            if (dynamicQuest == null)
                return null;

            var unifiedData = new UnifiedQuestData
            {
                QuestId = dynamicQuest.QuestId,
                Title = dynamicQuest.Title,
                Description = dynamicQuest.Description,
                Type = QuestType.Dynamic,
                Status = MapQuestStatus(dynamicQuest.Status),
                CreatedAt = dynamicQuest.CreatedAt,
                UpdatedAt = dynamicQuest.ModifiedAt,
                Owner = dynamicQuest.Owner,
                Creator = dynamicQuest.Creator,
                Theme = ExtractTheme(dynamicQuest.Description),
                Location = ExtractLocation(dynamicQuest.Description),
                DifficultyLevel = EstimateDifficulty(dynamicQuest),
                Tags = ExtractTags(dynamicQuest.Description)
            };

            // Migrate progress data
            unifiedData.ProgressData = MigrateProgressData(dynamicQuest);
            unifiedData.PlayerProgress[dynamicQuest.Owner.Serial] = MigratePlayerProgress(dynamicQuest.Owner, dynamicQuest);

            // Migrate journal data
            unifiedData.JournalData.IsInJournal = true;
            unifiedData.JournalData.AddedToJournal = DateTime.UtcNow;

            // Migrate persistence data
            unifiedData.PersistenceData.IsPersisted = true;
            unifiedData.PersistenceData.LastSaved = DateTime.UtcNow;

            return unifiedData;
        }

        /// <summary>
        /// Migrate VystiaQuest to UnifiedQuestData
        /// </summary>
        private static UnifiedQuestData MigrateVystiaQuest(object source)
        {
            var vystiaQuest = source as VystiaQuest;
            if (vystiaQuest == null)
                return null;

            var unifiedData = new UnifiedQuestData
            {
                QuestId = vystiaQuest.QuestId,
                Title = vystiaQuest.Title,
                Description = vystiaQuest.Description,
                Type = QuestType.Vystia,
                Status = MapQuestStatus(vystiaQuest.Status),
                CreatedAt = vystiaQuest.CreatedAt,
                UpdatedAt = vystiaQuest.ModifiedAt,
                Owner = vystiaQuest.Owner,
                Creator = vystiaQuest.Creator,
                Theme = vystiaQuest.Theme,
                Location = vystiaQuest.Location,
                DifficultyLevel = vystiaQuest.DifficultyLevel,
                Tags = vystiaQuest.Tags?.ToList() ?? new List<string>()
            };

            // Migrate objectives
            if (vystiaQuest.Objectives != null)
            {
                unifiedData.CooperativeObjectives = vystiaQuest.Objectives.Select(obj => new CooperativeObjectiveData
                {
                    ObjectiveId = obj.Id,
                    Description = obj.Description,
                    Type = MapObjectiveType(obj.Type),
                    RequiredCount = obj.RequiredCount,
                    CurrentProgress = obj.CurrentProgress,
                    IsCompleted = obj.IsCompleted
                }).ToList();

                unifiedData.ProgressData.TotalObjectives = unifiedData.CooperativeObjectives.Count;
                unifiedData.ProgressData.CompletedObjectives = unifiedData.CooperativeObjectives.Count(o => o.IsCompleted);
            }

            // Migrate player progress
            if (vystiaQuest.Owner != null)
            {
                unifiedData.PlayerProgress[vystiaQuest.Owner.Serial] = MigratePlayerProgress(vystiaQuest.Owner, vystiaQuest);
            }

            return unifiedData;
        }

        /// <summary>
        /// Migrate BaseQuest to UnifiedQuestData
        /// </summary>
        private static UnifiedQuestData MigrateBaseQuest(object source)
        {
            var baseQuest = source as BaseQuest;
            if (baseQuest == null)
                return null;

            var unifiedData = new UnifiedQuestData
            {
                QuestId = baseQuest.QuestId,
                Title = baseQuest.Title,
                Description = baseQuest.Description,
                Type = QuestType.Traditional,
                Status = MapQuestStatus(baseQuest.Status),
                CreatedAt = baseQuest.CreatedAt,
                UpdatedAt = baseQuest.ModifiedAt,
                Owner = baseQuest.Owner,
                Creator = baseQuest.Creator,
                Theme = "Traditional",
                Location = baseQuest.Location?.ToString() ?? "Unknown",
                DifficultyLevel = baseQuest.DifficultyLevel,
                Tags = new List<string> { "Traditional", "Classic" }
            };

            // Traditional quests may have limited data, set defaults
            unifiedData.ProgressData.StartedAt = baseQuest.StartedAt;
            unifiedData.ProgressData.CompletedAt = baseQuest.CompletedAt;

            return unifiedData;
        }

        /// <summary>
        /// Migrate SharedQuestInfo to UnifiedQuestData
        /// </summary>
        private static UnifiedQuestData MigrateSharedQuestInfo(object source)
        {
            var sharedQuestInfo = source as SharedQuestInfo;
            if (sharedQuestInfo == null)
                return null;

            var unifiedData = new UnifiedQuestData
            {
                QuestId = sharedQuestInfo.QuestId,
                Title = sharedQuestInfo.QuestTitle,
                Description = sharedQuestInfo.QuestDescription,
                Type = QuestType.Multiplayer,
                Status = sharedQuestInfo.IsCompleted ? QuestStatus.Completed : QuestStatus.Active,
                CreatedAt = sharedQuestInfo.SharedAt,
                UpdatedAt = DateTime.UtcNow,
                Owner = sharedQuestInfo.SharedBy,
                Creator = sharedQuestInfo.SharedBy,
                Party = sharedQuestInfo.Party
            };

            // Migrate multiplayer-specific data
            unifiedData.MultiplayerData.IsActive = sharedQuestInfo.IsActive;
            unifiedData.MultiplayerData.IsShared = true;
            unifiedData.MultiplayerData.SharedBy = sharedQuestInfo.SharedBy;
            unifiedData.MultiplayerData.SharedAt = sharedQuestInfo.SharedAt;
            unifiedData.MultiplayerData.Party = sharedQuestInfo.Party;
            unifiedData.MultiplayerData.Settings = sharedQuestInfo.Settings;

            // Migrate cooperative objectives
            if (sharedQuestInfo.CooperativeObjectives != null)
            {
                unifiedData.CooperativeObjectives = sharedQuestInfo.CooperativeObjectives.Select(obj => new CooperativeObjectiveData
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

            // Migrate member progress
            if (sharedQuestInfo.MemberProgress != null)
            {
                foreach (var memberProgress in sharedQuestInfo.MemberProgress)
                {
                    if (memberProgress.Member != null)
                    {
                        unifiedData.PlayerProgress[memberProgress.Member.Serial] = new PlayerProgressData
                        {
                            Player = memberProgress.Member,
                            HasAccepted = memberProgress.HasAccepted,
                            AcceptedAt = memberProgress.AcceptedAt,
                            HasCompleted = memberProgress.HasCompleted,
                            CompletedAt = memberProgress.CompletedAt,
                            ContributionScore = memberProgress.ContributionScore,
                            ObjectiveProgress = memberProgress.ObjectiveProgress?.ToDictionary(kvp => kvp.Key, kvp => kvp.Value) ?? new Dictionary<string, int>(),
                            ActionsPerformed = memberProgress.ActionsPerformed?.ToList() ?? new List<string>(),
                            LastActivity = DateTime.UtcNow
                        };
                    }
                }
            }

            return unifiedData;
        }

        /// <summary>
        /// Migrate QuestSimilarityMetrics to UnifiedQuestData
        /// </summary>
        private static UnifiedQuestData MigrateQuestSimilarityMetrics(object source)
        {
            var similarityMetrics = source as QuestSimilarityMetrics;
            if (similarityMetrics == null)
                return null;

            var unifiedData = new UnifiedQuestData
            {
                QuestId = int.TryParse(similarityMetrics.QuestId, out var id) ? id : 0,
                Title = similarityMetrics.Title,
                Description = similarityMetrics.Description,
                Type = QuestType.Dynamic, // Default type
                Status = QuestStatus.Created,
                CreatedAt = similarityMetrics.GeneratedAt,
                UpdatedAt = DateTime.UtcNow,
                Theme = similarityMetrics.Theme,
                Location = similarityMetrics.Location
            };

            // Migrate similarity data
            unifiedData.SimilarityData.SimilarityScore = similarityMetrics.SimilarityScore;
            unifiedData.SimilarityData.FeatureVector = similarityMetrics.FeatureVector?.ToDictionary(kvp => kvp.Key, kvp => kvp.Value) ?? new Dictionary<string, double>();
            unifiedData.SimilarityData.SimilarQuests = similarityMetrics.SimilarQuests?.ToList() ?? new List<string>();
            unifiedData.SimilarityData.WouldBeRepetitive = unifiedData.SimilarityData.SimilarityScore > 0.7;

            return unifiedData;
        }

        /// <summary>
        /// Create default UnifiedQuestData when no specific migration is available
        /// </summary>
        private static UnifiedQuestData CreateDefaultUnifiedQuestData(object source)
        {
            var unifiedData = new UnifiedQuestData
            {
                Type = QuestType.Traditional,
                Status = QuestStatus.Created,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                Title = $"Migrated Quest {s_MigrationCount}",
                Description = $"Automatically migrated from {source.GetType().Name}"
            };

            // Try to extract basic information using reflection
            try
            {
                var sourceType = source.GetType();
                
                // Try to get QuestId
                var questIdProperty = sourceType.GetProperty("QuestId");
                if (questIdProperty != null)
                {
                    var value = questIdProperty.GetValue(source);
                    if (value is int id)
                        unifiedData.QuestId = id;
                }

                // Try to get Title
                var titleProperty = sourceType.GetProperty("Title");
                if (titleProperty != null)
                {
                    var value = titleProperty.GetValue(source);
                    if (value is string title)
                        unifiedData.Title = title;
                }

                // Try to get Description
                var descriptionProperty = sourceType.GetProperty("Description");
                if (descriptionProperty != null)
                {
                    var value = descriptionProperty.GetValue(source);
                    if (value is string description)
                        unifiedData.Description = description;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[DataMigration] Error extracting data via reflection: {ex.Message}");
            }

            return unifiedData;
        }

        /// <summary>
        /// Find compatible migration strategy for similar types
        /// </summary>
        private static Func<object, UnifiedQuestData> FindCompatibleMigrationStrategy(Type sourceType)
        {
            // Check for inheritance relationships
            foreach (var strategy in s_MigrationStrategies)
            {
                if (strategy.Key.IsAssignableFrom(sourceType))
                {
                    return strategy.Value;
                }
            }

            return null;
        }

        /// <summary>
        /// Map quest status from various quest systems to unified status
        /// </summary>
        private static QuestStatus MapQuestStatus(object sourceStatus)
        {
            if (sourceStatus == null)
                return QuestStatus.Created;

            // Handle different status types
            if (sourceStatus is string statusString)
            {
                switch (statusString.ToLower())
                {
                    case "active":
                    case "inprogress":
                    case "ongoing":
                        return QuestStatus.Active;
                    case "completed":
                    case "finished":
                    case "done":
                        return QuestStatus.Completed;
                    case "failed":
                    case "abandoned":
                        return QuestStatus.Failed;
                    case "expired":
                    case "timeout":
                        return QuestStatus.Expired;
                    case "cancelled":
                    case "canceled":
                        return QuestStatus.Cancelled;
                    default:
                        return QuestStatus.Created;
                }
            }

            // Handle enum status types
            if (sourceStatus is int statusInt)
            {
                switch (statusInt)
                {
                    case 0:
                        return QuestStatus.Created;
                    case 1:
                        return QuestStatus.Active;
                    case 2:
                        return QuestStatus.Completed;
                    case 3:
                        return QuestStatus.Failed;
                    case 4:
                        return QuestStatus.Expired;
                    case 5:
                        return QuestStatus.Cancelled;
                    default:
                        return QuestStatus.Created;
                }
            }

            return QuestStatus.Created;
        }

        /// <summary>
        /// Map objective type to cooperative objective type
        /// </summary>
        private static CooperativeObjectiveType MapObjectiveType(object sourceType)
        {
            if (sourceType == null)
                return CooperativeObjectiveType.IndividualContribution;

            if (sourceType is string typeString)
            {
                switch (typeString.ToLower())
                {
                    case "individual":
                    case "solo":
                        return CooperativeObjectiveType.IndividualContribution;
                    case "group":
                    case "party":
                        return CooperativeObjectiveType.GroupContribution;
                    case "role":
                    case "rolebased":
                        return CooperativeObjectiveType.RoleBased;
                    case "all":
                    case "everyone":
                        return CooperativeObjectiveType.AllMembers;
                    case "leader":
                    case "leadersonly":
                        return CooperativeObjectiveType.LeaderOnly;
                    default:
                        return CooperativeObjectiveType.IndividualContribution;
                }
            }

            return CooperativeObjectiveType.IndividualContribution;
        }

        /// <summary>
        /// Migrate progress data from quest source
        /// </summary>
        private static QuestProgressData MigrateProgressData(object questSource)
        {
            var progressData = new QuestProgressData();

            try
            {
                var sourceType = questSource.GetType();

                // Try to extract progress information
                var progressProperty = sourceType.GetProperty("Progress");
                if (progressProperty != null)
                {
                    var value = progressProperty.GetValue(questSource);
                    if (value is double progress)
                        progressData.OverallProgress = progress;
                }

                // Try to extract objectives count
                var objectivesProperty = sourceType.GetProperty("Objectives");
                if (objectivesProperty != null)
                {
                    var value = objectivesProperty.GetValue(questSource);
                    if (value is System.Collections.ICollection objectives)
                    {
                        progressData.TotalObjectives = objectives.Count;
                        // Count completed objectives
                        var completedCount = 0;
                        foreach (var obj in objectives)
                        {
                            var objType = obj.GetType();
                            var completedProperty = objType.GetProperty("IsCompleted");
                            if (completedProperty != null && completedProperty.GetValue(obj) is bool completed && completed)
                                completedCount++;
                        }
                        progressData.CompletedObjectives = completedCount;
                    }
                }

                progressData.UpdateProgress();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[DataMigration] Error migrating progress data: {ex.Message}");
            }

            return progressData;
        }

        /// <summary>
        /// Migrate player progress from quest source
        /// </summary>
        private static PlayerProgressData MigratePlayerProgress(PlayerMobile player, object questSource)
        {
            var playerProgress = new PlayerProgressData
            {
                Player = player,
                HasAccepted = true,
                AcceptedAt = DateTime.UtcNow,
                LastActivity = DateTime.UtcNow
            };

            try
            {
                var sourceType = questSource.GetType();

                // Try to extract player-specific progress
                var playerProgressProperty = sourceType.GetProperty("PlayerProgress");
                if (playerProgressProperty != null)
                {
                    var value = playerProgressProperty.GetValue(questSource);
                    if (value is Dictionary<string, int> progress)
                    {
                        playerProgress.ObjectiveProgress = progress;
                    }
                }

                // Try to extract contribution score
                var contributionProperty = sourceType.GetProperty("ContributionScore");
                if (contributionProperty != null)
                {
                    var value = contributionProperty.GetValue(questSource);
                    if (value is int contribution)
                        playerProgress.ContributionScore = contribution;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[DataMigration] Error migrating player progress: {ex.Message}");
            }

            return playerProgress;
        }

        /// <summary>
        /// Extract theme from description using simple heuristics
        /// </summary>
        private static string ExtractTheme(string description)
        {
            if (string.IsNullOrEmpty(description))
                return "Unknown";

            var descriptionLower = description.ToLower();
            
            // Theme keywords
            var themes = new Dictionary<string, string[]>
            {
                { "Combat", new[] { "fight", "battle", "defeat", "kill", "combat", "war" } },
                { "Exploration", new[] { "explore", "discover", "find", "search", "investigate" } },
                { "Collection", new[] { "collect", "gather", "find", "obtain", "acquire" } },
                { "Delivery", new[] { "deliver", "bring", "carry", "transport", "escort" } },
                { "Protection", new[] { "protect", "guard", "defend", "shield", "safeguard" } },
                { "Rescue", new[] { "rescue", "save", "free", "liberate", "help" } }
            };

            foreach (var theme in themes)
            {
                if (theme.Value.Any(keyword => descriptionLower.Contains(keyword)))
                {
                    return theme.Key;
                }
            }

            return "General";
        }

        /// <summary>
        /// Extract location from description using simple heuristics
        /// </summary>
        private static string ExtractLocation(string description)
        {
            if (string.IsNullOrEmpty(description))
                return "Unknown";

            // Simple location extraction - look for location patterns
            var locationPatterns = new[]
            {
                @"in (\w+)",
                @"at (\w+)",
                @"near (\w+)",
                @"around (\w+)",
                @"to (\w+)"
            };

            foreach (var pattern in locationPatterns)
            {
                var match = System.Text.RegularExpressions.Regex.Match(description, pattern, System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                if (match.Success && match.Groups.Count > 1)
                {
                    return match.Groups[1].Value;
                }
            }

            return "Unknown";
        }

        /// <summary>
        /// Estimate difficulty level from quest content
        /// </summary>
        private static int EstimateDifficulty(object questSource)
        {
            try
            {
                var sourceType = questSource.GetType();
                
                // Try to get explicit difficulty
                var difficultyProperty = sourceType.GetProperty("Difficulty");
                if (difficultyProperty != null)
                {
                    var value = difficultyProperty.GetValue(questSource);
                    if (value is int difficulty)
                        return difficulty;
                    if (value is string difficultyString && int.TryParse(difficultyString, out var parsedDifficulty))
                        return parsedDifficulty;
                }

                // Estimate from description length and complexity
                var descriptionProperty = sourceType.GetProperty("Description");
                if (descriptionProperty != null)
                {
                    var value = descriptionProperty.GetValue(questSource);
                    if (value is string description)
                    {
                        // Simple heuristic: longer descriptions = higher difficulty
                        if (description.Length < 100)
                            return 1;
                        else if (description.Length < 300)
                            return 2;
                        else if (description.Length < 500)
                            return 3;
                        else
                            return 4;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[DataMigration] Error estimating difficulty: {ex.Message}");
            }

            return 2; // Default medium difficulty
        }

        /// <summary>
        /// Extract tags from description using simple heuristics
        /// </summary>
        private static List<string> ExtractTags(string description)
        {
            var tags = new List<string>();

            if (string.IsNullOrEmpty(description))
                return tags;

            var descriptionLower = description.ToLower();

            // Tag keywords
            var tagKeywords = new Dictionary<string, string[]>
            {
                { "Combat", new[] { "fight", "battle", "combat", "defeat" } },
                { "Magic", new[] { "magic", "spell", "arcane", "mystical" } },
                { "Exploration", new[] { "explore", "discover", "investigate" } },
                { "Collection", new[] { "collect", "gather", "find", "obtain" } },
                { "Delivery", new[] { "deliver", "bring", "carry", "escort" } },
                { "Protection", new[] { "protect", "guard", "defend" } },
                { "Rescue", new[] { "rescue", "save", "free", "help" } },
                { "Puzzle", new[] { "puzzle", "solve", "riddle", "mystery" } },
                { "Stealth", new[] { "stealth", "sneak", "hidden", "secret" } }
            };

            foreach (var tag in tagKeywords)
            {
                if (tag.Value.Any(keyword => descriptionLower.Contains(keyword)))
                {
                    tags.Add(tag.Key);
                }
            }

            return tags.Distinct().ToList();
        }

        /// <summary>
        /// Get migration statistics
        /// </summary>
        public static MigrationStatistics GetStatistics()
        {
            lock (s_Lock)
            {
                return new MigrationStatistics
                {
                    TotalMigrations = s_MigrationCount,
                    ErrorCount = s_ErrorCount,
                    SuccessRate = s_MigrationCount > 0 ? (double)(s_MigrationCount - s_ErrorCount) / s_MigrationCount : 0.0,
                    LastMigration = DateTime.UtcNow,
                    AvailableStrategies = s_MigrationStrategies.Count
                };
            }
        }

        /// <summary>
        /// Reset migration statistics
        /// </summary>
        public static void ResetStatistics()
        {
            lock (s_Lock)
            {
                s_MigrationCount = 0;
                s_ErrorCount = 0;
            }
        }
    }

    /// <summary>
    /// Migration statistics
    /// </summary>
    public class MigrationStatistics
    {
        public int TotalMigrations { get; set; }
        public int ErrorCount { get; set; }
        public double SuccessRate { get; set; }
        public DateTime LastMigration { get; set; }
        public int AvailableStrategies { get; set; }
    }
}
