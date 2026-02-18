using System;
using System.Collections.Generic;
using System.Linq;
using Server.Mobiles;
using Server.Custom.VystiaClasses.Quests;
using Server.Custom.VystiaClasses.Quests.Generation;
using Server.Services.QuestPersistence;
using Server.Engines.Quests;

namespace Server.Services.QuestJournal
{
    /// <summary>
    /// Adapter for Vystia quest system
    /// </summary>
    public class VystiaQuestAdapter : IQuestDisplayAdapter
    {
        public string GetTitle(object quest)
        {
            if (quest is VystiaQuest vystiaQuest)
                return vystiaQuest.Title;
            return "Unknown Vystia Quest";
        }

        public string GetDescription(object quest)
        {
            if (quest is VystiaQuest vystiaQuest)
                return vystiaQuest.Description;
            return "Quest description not available.";
        }

        public string GetQuestType(object quest)
        {
            return "Vystia";
        }

        public int GetQuestId(object quest)
        {
            if (quest is VystiaQuest vystiaQuest)
                return vystiaQuest.QuestID;
            return 0;
        }

        public QuestProgressInfo GetProgress(object quest, PlayerMobile player)
        {
            if (quest is VystiaQuest vystiaQuest && player != null)
            {
                var tracker = VystiaQuestTracker.GetTracker(player);
                var progress = tracker?.GetProgress(vystiaQuest.QuestID);
                
                if (progress != null)
                {
                    var objectives = vystiaQuest.GetObjectives();
                    var progressDict = progress.ToDictionary();
                    var totalRequired = objectives.Values.Aggregate(0, (sum, val) => sum + val);
                    var totalCompleted = progressDict.Values.Aggregate(0, (sum, val) => sum + Convert.ToInt32(val));
                    
                    return new QuestProgressInfo
                    {
                        Description = "Overall quest progress",
                        CurrentProgress = totalCompleted,
                        RequiredProgress = totalRequired,
                        IsCompleted = vystiaQuest.AreObjectivesComplete(progress)
                    };
                }
            }
            
            return new QuestProgressInfo
            {
                Description = "No progress available",
                CurrentProgress = 0,
                RequiredProgress = 1
            };
        }

        public bool IsActive(object quest, PlayerMobile player)
        {
            if (quest is VystiaQuest vystiaQuest && player != null)
                return VystiaQuestSystem.HasActiveQuest(player, vystiaQuest.QuestID);
            return false;
        }

        public bool IsCompleted(object quest, PlayerMobile player)
        {
            if (quest is VystiaQuest vystiaQuest && player != null)
                return VystiaQuestSystem.HasCompletedQuest(player, vystiaQuest.QuestID);
            return false;
        }

        public List<QuestObjectiveInfo> GetObjectives(object quest, PlayerMobile player)
        {
            var objectives = new List<QuestObjectiveInfo>();
            
            if (quest is VystiaQuest vystiaQuest && player != null)
            {
                var tracker = VystiaQuestTracker.GetTracker(player);
                var progress = tracker?.GetProgress(vystiaQuest.QuestID);
                
                if (progress != null)
                {
                    var questObjectives = vystiaQuest.GetObjectives();
                    var progressDict = progress.ToDictionary();
                    
                    foreach (var kvp in questObjectives)
                    {
                        var currentProgress = progressDict.ContainsKey(kvp.Key) ? (int)progressDict[kvp.Key] : 0;
                        objectives.Add(new QuestObjectiveInfo
                        {
                            Description = kvp.Key,
                            IsCompleted = currentProgress >= kvp.Value,
                            ProgressText = $"{currentProgress}/{kvp.Value}",
                            CurrentProgress = currentProgress,
                            RequiredProgress = kvp.Value
                        });
                    }
                }
            }
            
            return objectives;
        }

        public List<QuestRewardInfo> GetRewards(object quest)
        {
            var rewards = new List<QuestRewardInfo>();
            
            if (quest is VystiaQuest vystiaQuest)
            {
                // Add standard rewards based on quest tier
                switch (vystiaQuest.Tier)
                {
                    case QuestTier.Initiation:
                        rewards.Add(new QuestRewardInfo { Description = "Gold reward", Type = "Gold", Amount = 100 });
                        rewards.Add(new QuestRewardInfo { Description = "Experience", Type = "Experience", Amount = 500 });
                        break;
                    case QuestTier.Apprentice:
                        rewards.Add(new QuestRewardInfo { Description = "Gold reward", Type = "Gold", Amount = 250 });
                        rewards.Add(new QuestRewardInfo { Description = "Experience", Type = "Experience", Amount = 1000 });
                        break;
                    case QuestTier.Journeyman:
                        rewards.Add(new QuestRewardInfo { Description = "Gold reward", Type = "Gold", Amount = 500 });
                        rewards.Add(new QuestRewardInfo { Description = "Experience", Type = "Experience", Amount = 2000 });
                        break;
                    case QuestTier.Master:
                        rewards.Add(new QuestRewardInfo { Description = "Gold reward", Type = "Gold", Amount = 1000 });
                        rewards.Add(new QuestRewardInfo { Description = "Experience", Type = "Experience", Amount = 5000 });
                        break;
                }
            }
            
            return rewards;
        }

        public string GetDifficulty(object quest)
        {
            if (quest is VystiaQuest vystiaQuest)
                return vystiaQuest.Tier.ToString();
            return "Unknown";
        }

        public string GetLocation(object quest)
        {
            if (quest is VystiaQuest vystiaQuest)
            {
                // For now, return a generic location since GetWaypoints is not available
                return "Vystia Region";
            }
            return "Unknown location";
        }

        public TimeSpan? GetTimeLimit(object quest)
        {
            // Vystia quests typically don't have time limits
            return null;
        }

        public bool CanAbandon(object quest, PlayerMobile player)
        {
            // Most Vystia quests can be abandoned unless they're critical
            // For now, return true since IsEphemeral is not available
            return true;
        }
    }

    /// <summary>
    /// Adapter for Dynamic quest system
    /// </summary>
    public class DynamicQuestAdapter : IQuestDisplayAdapter
    {
        public string GetTitle(object quest)
        {
            if (quest is DynamicQuest dynamicQuest)
                return dynamicQuest.Title;
            return "Unknown Dynamic Quest";
        }

        public string GetDescription(object quest)
        {
            if (quest is DynamicQuest dynamicQuest)
                return dynamicQuest.Description;
            return "Quest description not available.";
        }

        public string GetQuestType(object quest)
        {
            return "Dynamic";
        }

        public int GetQuestId(object quest)
        {
            if (quest is DynamicQuest dynamicQuest)
                return dynamicQuest.QuestID;
            return 0;
        }

        public QuestProgressInfo GetProgress(object quest, PlayerMobile player)
        {
            if (quest is DynamicQuest dynamicQuest && player != null)
            {
                var attachment = GeneratedQuestInstanceAttachment.Get(player);
                if (attachment != null)
                {
                    var instance = attachment.Instances.FirstOrDefault(i => i.QuestId == dynamicQuest.QuestID);
                    if (instance != null)
                    {
                        // Calculate progress based on objectives
                        var objectives = dynamicQuest.GetObjectives();
                        var totalRequired = objectives.Values.Aggregate(0, (sum, val) => sum + val);
                        var totalCompleted = objectives.Count(kvp => kvp.Value <= 0); // Assuming negative means completed
                        
                        return new QuestProgressInfo
                        {
                            Description = "Dynamic quest progress",
                            CurrentProgress = totalCompleted,
                            RequiredProgress = totalRequired,
                            IsCompleted = totalCompleted >= totalRequired
                        };
                    }
                }
            }
            
            return new QuestProgressInfo
            {
                Description = "No progress available",
                CurrentProgress = 0,
                RequiredProgress = 1
            };
        }

        public bool IsActive(object quest, PlayerMobile player)
        {
            if (quest is DynamicQuest dynamicQuest && player != null)
            {
                var attachment = GeneratedQuestInstanceAttachment.Get(player);
                return attachment?.Instances.Any(i => i.QuestId == dynamicQuest.QuestID) == true;
            }
            return false;
        }

        public bool IsCompleted(object quest, PlayerMobile player)
        {
            if (quest is DynamicQuest dynamicQuest && player != null)
            {
                var attachment = GeneratedQuestInstanceAttachment.Get(player);
                var instance = attachment?.Instances.FirstOrDefault(i => i.QuestId == dynamicQuest.QuestID);
                return instance?.ExpiresAtUtc <= DateTime.UtcNow;
            }
            return false;
        }

        public List<QuestObjectiveInfo> GetObjectives(object quest, PlayerMobile player)
        {
            var objectives = new List<QuestObjectiveInfo>();
            
            if (quest is DynamicQuest dynamicQuest)
            {
                var questObjectives = dynamicQuest.GetObjectives();
                foreach (var kvp in questObjectives)
                {
                    objectives.Add(new QuestObjectiveInfo
                    {
                        Description = kvp.Key,
                        IsCompleted = kvp.Value <= 0,
                        ProgressText = kvp.Value > 0 ? $"{kvp.Value} remaining" : "Completed",
                        CurrentProgress = kvp.Value <= 0 ? 1 : 0,
                        RequiredProgress = 1
                    });
                }
            }
            
            return objectives;
        }

        public List<QuestRewardInfo> GetRewards(object quest)
        {
            var rewards = new List<QuestRewardInfo>();
            
            if (quest is DynamicQuest dynamicQuest)
            {
                // Dynamic quests typically have variable rewards
                rewards.Add(new QuestRewardInfo { Description = "Dynamic rewards", Type = "Variable", Amount = 0 });
            }
            
            return rewards;
        }

        public string GetDifficulty(object quest)
        {
            if (quest is DynamicQuest dynamicQuest)
                return dynamicQuest.Tier.ToString();
            return "Unknown";
        }

        public string GetLocation(object quest)
        {
            if (quest is DynamicQuest dynamicQuest)
            {
                var waypoints = dynamicQuest.Waypoints;
                if (waypoints != null && waypoints.Count > 0)
                {
                    var origin = waypoints.FirstOrDefault();
                    if (origin != null)
                        return $"Near {origin.Location}";
                }
            }
            return "Unknown location";
        }

        public TimeSpan? GetTimeLimit(object quest)
        {
            if (quest is DynamicQuest dynamicQuest)
            {
                // Dynamic quests may have expiration times
                return TimeSpan.FromHours(24); // Default 24 hours
            }
            return null;
        }

        public bool CanAbandon(object quest, PlayerMobile player)
        {
            // Dynamic quests can typically be abandoned
            return true;
        }
    }

    /// <summary>
    /// Adapter for Traditional quest system
    /// </summary>
    public class TraditionalQuestAdapter : IQuestDisplayAdapter
    {
        public string GetTitle(object quest)
        {
            if (quest is BaseQuest baseQuest)
                return baseQuest.GetType().Name.Replace("Quest", "");
            return "Unknown Traditional Quest";
        }

        public string GetDescription(object quest)
        {
            if (quest is BaseQuest baseQuest)
                return "Traditional quest from the Ultima Online quest system.";
            return "Quest description not available.";
        }

        public string GetQuestType(object quest)
        {
            return "Traditional";
        }

        public int GetQuestId(object quest)
        {
            // Traditional quests don't have consistent IDs, use hash
            return quest?.GetHashCode() ?? 0;
        }

        public QuestProgressInfo GetProgress(object quest, PlayerMobile player)
        {
            // Traditional quests use different progress tracking
            return new QuestProgressInfo
            {
                Description = "Traditional quest progress",
                CurrentProgress = 0,
                RequiredProgress = 1,
                IsCompleted = IsCompleted(quest, player)
            };
        }

        public bool IsActive(object quest, PlayerMobile player)
        {
            // Traditional quests use ServUO's quest context
            // This would need to be implemented based on the actual quest system
            return false;
        }

        public bool IsCompleted(object quest, PlayerMobile player)
        {
            // Traditional quests use ServUO's quest context
            // This would need to be implemented based on the actual quest system
            return false;
        }

        public List<QuestObjectiveInfo> GetObjectives(object quest, PlayerMobile player)
        {
            // Traditional quests may have objectives, but they're not standardized
            return new List<QuestObjectiveInfo>();
        }

        public List<QuestRewardInfo> GetRewards(object quest)
        {
            var rewards = new List<QuestRewardInfo>();
            
            if (quest is BaseQuest baseQuest)
            {
                // Add standard traditional quest rewards
                rewards.Add(new QuestRewardInfo { Description = "Traditional quest rewards", Type = "Variable", Amount = 0 });
            }
            
            return rewards;
        }

        public string GetDifficulty(object quest)
        {
            // Traditional quests don't have consistent difficulty levels
            return "Unknown";
        }

        public string GetLocation(object quest)
        {
            // Traditional quests don't have consistent location tracking
            return "Unknown location";
        }

        public TimeSpan? GetTimeLimit(object quest)
        {
            // Traditional quests typically don't have time limits
            return null;
        }

        public bool CanAbandon(object quest, PlayerMobile player)
        {
            // Traditional quests can typically be abandoned
            return true;
        }
    }
}
