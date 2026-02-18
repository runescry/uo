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
    /// Compatibility layer that maintains backward compatibility during quest system migration
    /// Provides adapters and converters for existing quest systems to work with unified data model
    /// </summary>
    public static class CompatibilityLayer
    {
        private static readonly Dictionary<Type, IQuestAdapter> s_Adapters;
        private static readonly object s_Lock = new object();
        private static bool s_Initialized = false;
        private static readonly Dictionary<int, UnifiedQuestData> s_QuestCache = new Dictionary<int, UnifiedQuestData>();

        static CompatibilityLayer()
        {
            s_Adapters = new Dictionary<Type, IQuestAdapter>
            {
                { typeof(DynamicQuest), new DynamicQuestAdapter() },
                { typeof(VystiaQuest), new VystiaQuestAdapter() },
                { typeof(BaseQuest), new BaseQuestAdapter() },
                { typeof(SharedQuestInfo), new SharedQuestAdapter() }
            };
        }

        /// <summary>
        /// Initialize the compatibility layer
        /// </summary>
        public static void Initialize()
        {
            if (s_Initialized)
                return;

            s_Initialized = true;
            Console.WriteLine("[CompatibilityLayer] Initialized backward compatibility layer");
            Console.WriteLine($"[CompatibilityLayer] Registered {s_Adapters.Count} quest adapters");
        }

        /// <summary>
        /// Get unified quest data from any quest object
        /// </summary>
        public static UnifiedQuestData GetUnifiedQuestData(object quest)
        {
            if (quest == null)
                return null;

            lock (s_Lock)
            {
                // Check cache first
                if (TryGetFromCache(quest, out var cachedData))
                {
                    return cachedData;
                }

                // Try to get adapter for the quest type
                var questType = quest.GetType();
                if (s_Adapters.TryGetValue(questType, out var adapter))
                {
                    var unifiedData = adapter.ConvertToUnified(quest);
                    CacheQuestData(quest, unifiedData);
                    return unifiedData;
                }

                // Try to find compatible adapter
                var compatibleAdapter = FindCompatibleAdapter(questType);
                if (compatibleAdapter != null)
                {
                    var unifiedData = compatibleAdapter.ConvertToUnified(quest);
                    CacheQuestData(quest, unifiedData);
                    return unifiedData;
                }

                Console.WriteLine($"[CompatibilityLayer] No adapter found for quest type: {questType.Name}");
                return null;
            }
        }

        /// <summary>
        /// Convert unified quest data back to original format
        /// </summary>
        public static T ConvertFromUnified<T>(UnifiedQuestData unifiedData) where T : class, new()
        {
            if (unifiedData == null)
                return null;

            lock (s_Lock)
            {
                var targetType = typeof(T);
                
                // Try to get adapter for the target type
                if (s_Adapters.TryGetValue(targetType, out var adapter))
                {
                    return adapter.ConvertFromUnified<T>(unifiedData);
                }

                Console.WriteLine($"[CompatibilityLayer] No adapter found for target type: {targetType.Name}");
                return null;
            }
        }

        /// <summary>
        /// Check if a quest type is supported by the compatibility layer
        /// </summary>
        public static bool IsSupported(Type questType)
        {
            lock (s_Lock)
            {
                return s_Adapters.ContainsKey(questType) || FindCompatibleAdapter(questType) != null;
            }
        }

        /// <summary>
        /// Get all supported quest types
        /// </summary>
        public static List<Type> GetSupportedTypes()
        {
            lock (s_Lock)
            {
                return s_Adapters.Keys.ToList();
            }
        }

        /// <summary>
        /// Register a new quest adapter
        /// </summary>
        public static void RegisterAdapter(Type questType, IQuestAdapter adapter)
        {
            lock (s_Lock)
            {
                s_Adapters[questType] = adapter;
                Console.WriteLine($"[CompatibilityLayer] Registered adapter for {questType.Name}");
            }
        }

        /// <summary>
        /// Clear the quest cache
        /// </summary>
        public static void ClearCache()
        {
            lock (s_Lock)
            {
                s_QuestCache.Clear();
                Console.WriteLine("[CompatibilityLayer] Quest cache cleared");
            }
        }

        /// <summary>
        /// Get cache statistics
        /// </summary>
        public static CacheStatistics GetCacheStatistics()
        {
            lock (s_Lock)
            {
                return new CacheStatistics
                {
                    CachedQuests = s_QuestCache.Count,
                    MemoryUsage = EstimateMemoryUsage(),
                    LastCleanup = DateTime.UtcNow
                };
            }
        }

        /// <summary>
        /// Try to get quest data from cache
        /// </summary>
        private static bool TryGetFromCache(object quest, out UnifiedQuestData unifiedData)
        {
            // Try to extract quest ID
            var questId = ExtractQuestId(quest);
            if (questId > 0 && s_QuestCache.TryGetValue(questId, out unifiedData))
            {
                // Check if the cached data is still valid
                if (IsCacheDataValid(quest, unifiedData))
                {
                    return true;
                }
                else
                {
                    // Remove stale cache entry
                    s_QuestCache.Remove(questId);
                }
            }

            unifiedData = null;
            return false;
        }

        /// <summary>
        /// Cache quest data
        /// </summary>
        private static void CacheQuestData(object quest, UnifiedQuestData unifiedData)
        {
            var questId = ExtractQuestId(quest);
            if (questId > 0)
            {
                s_QuestCache[questId] = unifiedData;
                
                // Limit cache size
                if (s_QuestCache.Count > 1000)
                {
                    // Remove oldest entries (simple LRU simulation)
                    var keysToRemove = s_QuestCache.Keys.Take(100).ToList();
                    foreach (var key in keysToRemove)
                    {
                        s_QuestCache.Remove(key);
                    }
                }
            }
        }

        /// <summary>
        /// Extract quest ID from quest object
        /// </summary>
        private static int ExtractQuestId(object quest)
        {
            try
            {
                var questType = quest.GetType();
                var idProperty = questType.GetProperty("QuestId");
                if (idProperty != null)
                {
                    var value = idProperty.GetValue(quest);
                    if (value is int id)
                        return id;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[CompatibilityLayer] Error extracting quest ID: {ex.Message}");
            }

            return 0;
        }

        /// <summary>
        /// Check if cached data is still valid
        /// </summary>
        private static bool IsCacheDataValid(object quest, UnifiedQuestData cachedData)
        {
            try
            {
                var questType = quest.GetType();
                
                // Check if quest was modified
                var modifiedProperty = questType.GetProperty("ModifiedAt") ?? questType.GetProperty("UpdatedAt");
                if (modifiedProperty != null)
                {
                    var value = modifiedProperty.GetValue(quest);
                    if (value is DateTime modifiedTime)
                    {
                        return modifiedTime <= cachedData.UpdatedAt;
                    }
                }

                return true; // Assume valid if we can't check
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[CompatibilityLayer] Error validating cache data: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Find compatible adapter for similar quest types
        /// </summary>
        private static IQuestAdapter FindCompatibleAdapter(Type questType)
        {
            foreach (var adapter in s_Adapters)
            {
                if (adapter.Key.IsAssignableFrom(questType))
                {
                    return adapter.Value;
                }
            }

            return null;
        }

        /// <summary>
        /// Estimate memory usage of cache
        /// </summary>
        private static long EstimateMemoryUsage()
        {
            // Rough estimation: each cached quest ~1KB
            return s_QuestCache.Count * 1024;
        }
    }

    /// <summary>
    /// Interface for quest adapters
    /// </summary>
    public interface IQuestAdapter
    {
        UnifiedQuestData ConvertToUnified(object quest);
        T ConvertFromUnified<T>(UnifiedQuestData unifiedData) where T : class, new();
        bool CanConvert(Type questType);
    }

    /// <summary>
    /// Adapter for DynamicQuest objects
    /// </summary>
    public class DynamicQuestAdapter : IQuestAdapter
    {
        public UnifiedQuestData ConvertToUnified(object quest)
        {
            return DataMigrationUtilities.MigrateToUnified(quest);
        }

        public T ConvertFromUnified<T>(UnifiedQuestData unifiedData) where T : class, new()
        {
            if (typeof(T) != typeof(DynamicQuest))
                return null;

            var dynamicQuest = new DynamicQuest();
            
            // Convert unified data back to DynamicQuest
            dynamicQuest.QuestId = unifiedData.QuestId;
            dynamicQuest.Title = unifiedData.Title;
            dynamicQuest.Description = unifiedData.Description;
            dynamicQuest.Owner = unifiedData.Owner;
            dynamicQuest.Creator = unifiedData.Creator;
            dynamicQuest.CreatedAt = unifiedData.CreatedAt;
            dynamicQuest.ModifiedAt = unifiedData.UpdatedAt;

            return dynamicQuest as T;
        }

        public bool CanConvert(Type questType)
        {
            return questType == typeof(DynamicQuest);
        }
    }

    /// <summary>
    /// Adapter for VystiaQuest objects
    /// </summary>
    public class VystiaQuestAdapter : IQuestAdapter
    {
        public UnifiedQuestData ConvertToUnified(object quest)
        {
            return DataMigrationUtilities.MigrateToUnified(quest);
        }

        public T ConvertFromUnified<T>(UnifiedQuestData unifiedData) where T : class, new()
        {
            if (typeof(T) != typeof(VystiaQuest))
                return null;

            var vystiaQuest = new VystiaQuest();
            
            // Convert unified data back to VystiaQuest
            vystiaQuest.QuestId = unifiedData.QuestId;
            vystiaQuest.Title = unifiedData.Title;
            vystiaQuest.Description = unifiedData.Description;
            vystiaQuest.Owner = unifiedData.Owner;
            vystiaQuest.Creator = unifiedData.Creator;
            vystiaQuest.CreatedAt = unifiedData.CreatedAt;
            vystiaQuest.ModifiedAt = unifiedData.UpdatedAt;
            vystiaQuest.Theme = unifiedData.Theme;
            vystiaQuest.Location = unifiedData.Location;
            vystiaQuest.DifficultyLevel = unifiedData.DifficultyLevel;
            vystiaQuest.Tags = unifiedData.Tags?.ToArray();

            // Convert objectives
            if (unifiedData.CooperativeObjectives != null)
            {
                vystiaQuest.Objectives = unifiedData.CooperativeObjectives.Select(obj => new VystiaQuestObjective
                {
                    Id = obj.ObjectiveId,
                    Description = obj.Description,
                    Type = obj.Type.ToString(),
                    RequiredCount = obj.RequiredCount,
                    CurrentProgress = obj.CurrentProgress,
                    IsCompleted = obj.IsCompleted
                }).ToList();
            }

            return vystiaQuest as T;
        }

        public bool CanConvert(Type questType)
        {
            return questType == typeof(VystiaQuest);
        }
    }

    /// <summary>
    /// Adapter for BaseQuest objects
    /// </summary>
    public class BaseQuestAdapter : IQuestAdapter
    {
        public UnifiedQuestData ConvertToUnified(object quest)
        {
            return DataMigrationUtilities.MigrateToUnified(quest);
        }

        public T ConvertFromUnified<T>(UnifiedQuestData unifiedData) where T : class, new()
        {
            if (typeof(T) != typeof(BaseQuest))
                return null;

            var baseQuest = new BaseQuest();
            
            // Convert unified data back to BaseQuest
            baseQuest.QuestId = unifiedData.QuestId;
            baseQuest.Title = unifiedData.Title;
            baseQuest.Description = unifiedData.Description;
            baseQuest.Owner = unifiedData.Owner;
            baseQuest.Creator = unifiedData.Creator;
            baseQuest.CreatedAt = unifiedData.CreatedAt;
            baseQuest.ModifiedAt = unifiedData.UpdatedAt;
            baseQuest.DifficultyLevel = unifiedData.DifficultyLevel;

            return baseQuest as T;
        }

        public bool CanConvert(Type questType)
        {
            return questType == typeof(BaseQuest);
        }
    }

    /// <summary>
    /// Adapter for SharedQuestInfo objects
    /// </summary>
    public class SharedQuestAdapter : IQuestAdapter
    {
        public UnifiedQuestData ConvertToUnified(object quest)
        {
            return DataMigrationUtilities.MigrateToUnified(quest);
        }

        public T ConvertFromUnified<T>(UnifiedQuestData unifiedData) where T : class, new()
        {
            if (typeof(T) != typeof(SharedQuestInfo))
                return null;

            var sharedQuestInfo = new SharedQuestInfo();
            
            // Convert unified data back to SharedQuestInfo
            sharedQuestInfo.QuestId = unifiedData.QuestId;
            sharedQuestInfo.QuestTitle = unifiedData.Title;
            sharedQuestInfo.QuestDescription = unifiedData.Description;
            sharedQuestInfo.SharedAt = unifiedData.CreatedAt;
            sharedQuestInfo.SharedBy = unifiedData.Creator;
            sharedQuestInfo.Party = unifiedData.Party;
            sharedQuestInfo.IsActive = unifiedData.Status == QuestStatus.Active;
            sharedQuestInfo.IsCompleted = unifiedData.Status == QuestStatus.Completed;
            sharedQuestInfo.CompletedAt = unifiedData.ProgressData.CompletedAt ?? DateTime.MinValue;

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

            return sharedQuestInfo as T;
        }

        public bool CanConvert(Type questType)
        {
            return questType == typeof(SharedQuestInfo);
        }
    }

    /// <summary>
    /// Cache statistics
    /// </summary>
    public class CacheStatistics
    {
        public int CachedQuests { get; set; }
        public long MemoryUsage { get; set; }
        public DateTime LastCleanup { get; set; }
    }

    /// <summary>
    /// Extension methods for compatibility layer
    /// </summary>
    public static class CompatibilityExtensions
    {
        /// <summary>
        /// Convert any quest to unified format
        /// </summary>
        public static UnifiedQuestData ToUnified(this object quest)
        {
            return CompatibilityLayer.GetUnifiedQuestData(quest);
        }

        /// <summary>
        /// Convert unified data back to specific quest type
        /// </summary>
        public static T FromUnified<T>(this UnifiedQuestData unifiedData) where T : class, new()
        {
            return CompatibilityLayer.ConvertFromUnified<T>(unifiedData);
        }

        /// <summary>
        /// Check if quest type is supported
        /// </summary>
        public static bool IsSupported(this Type questType)
        {
            return CompatibilityLayer.IsSupported(questType);
        }
    }
}
