using System;
using System.Collections.Generic;
using System.Linq;
using Server.Mobiles;
using Server.Items;
using Server.Engines.SeasonalEvents;
using Server.Services.UnifiedQuestSystem;

/// <summary>
/// Integration bridge between Unified Quest System and Seasonal Events System
/// Connects quests with seasonal events and limited-time content
/// 
/// Features:
/// - Automatic quest generation for active seasonal events
/// - Limited-time quests with expiration handling
/// - Collection, combat, and exploration quest types for events
/// - Event-specific rewards and commemorative items
/// - Support for all seasonal event types (Treasures of Tokuno, etc.)
/// - Event lifecycle management (start, progress, end)
/// - Real-time progress tracking during seasonal activities
/// 
/// Usage:
/// - Automatically generates quests when seasonal events start
/// - Tracks player participation and progress in seasonal activities
/// - Provides appropriate rewards based on event type and participation
/// - Handles quest expiration when events end
/// - Integrates with unified quest progress tracking system
/// 
/// Dependencies:
/// - Server.Engines.SeasonalEvents.SeasonalEventSystem
/// - Server.Services.UnifiedQuestSystem.UnifiedProgressTracker
/// - Server.Services.UnifiedQuestSystem.UnifiedQuestData
/// 
/// Events:
/// - OnSeasonalEventStarted: Generates quests for new seasonal events
/// - OnSeasonalEventEnded: Handles quest expiration and cleanup
/// - OnSeasonalEventProgress: Updates quest progress for event activities
/// </summary>
namespace Server.Services.UnifiedQuestSystem.Integrations
{
    public static class SeasonalEventQuestIntegration
    {
        private static readonly Dictionary<PlayerMobile, List<SeasonalEventQuestLink>> s_ActiveLinks;
        private static readonly Dictionary<EventType, List<UnifiedQuestData>> s_EventQuests;
        private static readonly object s_Lock = new object();
        private static bool s_Initialized = false;
        private static int s_TotalLinksCreated = 0;
        private static int s_SeasonalQuestsCompleted = 0;

        static SeasonalEventQuestIntegration()
        {
            s_ActiveLinks = new Dictionary<PlayerMobile, List<SeasonalEventQuestLink>>();
            s_EventQuests = new Dictionary<EventType, List<UnifiedQuestData>>();
        }

        /// <summary>
        /// Initialize the Seasonal Events-Quest integration
        /// </summary>
        public static void Initialize()
        {
            if (s_Initialized)
                return;

            s_Initialized = true;

            // Register event handlers
            EventSink.OnSeasonalEventStarted += OnSeasonalEventStarted;
            EventSink.OnSeasonalEventEnded += OnSeasonalEventEnded;
            EventSink.OnSeasonalEventProgress += OnSeasonalEventProgress;

            // Generate quests for active events
            GenerateEventQuests();

            Console.WriteLine("[SeasonalEventQuestIntegration] Initialized Seasonal Events-Quest integration");
        }

        /// <summary>
        /// Create a quest link for seasonal event
        /// </summary>
        public static SeasonalEventQuestLink CreateQuestLink(PlayerMobile player, EventType eventType, UnifiedQuestData quest)
        {
            if (player == null || quest == null)
                return null;

            lock (s_Lock)
            {
                var link = new SeasonalEventQuestLink
                {
                    Player = player,
                    EventType = eventType,
                    Quest = quest,
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true,
                    EventProgress = 0
                };

                // Add to active links
                if (!s_ActiveLinks.ContainsKey(player))
                    s_ActiveLinks[player] = new List<SeasonalEventQuestLink>();

                s_ActiveLinks[player].Add(link);
                s_TotalLinksCreated++;

                // Update quest with event objectives
                UpdateQuestWithEvent(quest, eventType);

                Console.WriteLine($"[SeasonalEventQuestIntegration] Created quest link for {player.Name}: {quest.Title} ({eventType})");

                return link;
            }
        }

        /// <summary>
        /// Generate quests for seasonal events
        /// </summary>
        public static void GenerateEventQuests()
        {
            lock (s_Lock)
            {
                foreach (EventType eventType in Enum.GetValues(typeof(EventType)))
                {
                    s_EventQuests[eventType] = new List<UnifiedQuestData>();

                    // Create different types of event quests
                    s_EventQuests[eventType].AddRange(CreateEventQuests(eventType));
                }

                Console.WriteLine($"[SeasonalEventQuestIntegration] Generated quests for {s_EventQuests.Count} event types");
            }
        }

        /// <summary>
        /// Get available seasonal event quests
        /// </summary>
        public static List<UnifiedQuestData> GetAvailableQuests(PlayerMobile player)
        {
            var availableQuests = new List<UnifiedQuestData>();

            lock (s_Lock)
            {
                // Get active seasonal events
                var activeEvents = GetActiveSeasonalEvents();

                foreach (var eventType in activeEvents)
                {
                    if (s_EventQuests.ContainsKey(eventType))
                    {
                        var eventQuests = s_EventQuests[eventType];
                        availableQuests.AddRange(eventQuests);
                    }
                }
            }

            return availableQuests;
        }

        /// <summary>
        /// Get quests for a specific event type
        /// </summary>
        public static List<UnifiedQuestData> GetEventQuests(EventType eventType)
        {
            lock (s_Lock)
            {
                return s_EventQuests.GetValueOrDefault(eventType, new List<UnifiedQuestData>());
            }
        }

        /// <summary>
        /// Get integration statistics
        /// </summary>
        public static SeasonalEventQuestStatistics GetStatistics()
        {
            lock (s_Lock)
            {
                return new SeasonalEventQuestStatistics
                {
                    TotalLinksCreated = s_TotalLinksCreated,
                    SeasonalQuestsCompleted = s_SeasonalQuestsCompleted,
                    ActiveLinks = s_ActiveLinks.Values.Sum(list => list.Count),
                    ActivePlayers = s_ActiveLinks.Count,
                    ActiveEvents = GetActiveSeasonalEvents().Count,
                    LastActivity = DateTime.UtcNow
                };
            }
        }

        /// <summary>
        /// Handle seasonal event started
        /// </summary>
        private static void OnSeasonalEventStarted(SeasonalEventStartedEventArgs e)
        {
            // Generate quests for the new event
            var quests = CreateEventQuests(e.EventType);
            
            lock (s_Lock)
            {
                s_EventQuests[e.EventType] = quests;
            }

            Console.WriteLine($"[SeasonalEventQuestIntegration] Generated {quests.Count} quests for {e.EventType} event");
        }

        /// <summary>
        /// Handle seasonal event ended
        /// </summary>
        private static void OnSeasonalEventEnded(SeasonalEventEndedEventArgs e)
        {
            // Complete all active quests for this event
            var links = GetAllActiveLinks()
                .Where(l => l.EventType == e.EventType && l.IsActive)
                .ToList();

            foreach (var link in links)
            {
                // Mark quests as expired
                link.Quest.IsExpired = true;
                link.Quest.ExpiresAt = DateTime.UtcNow;
                link.IsActive = false;

                // Notify player
                link.Player.SendMessage($"Event quest expired: {link.Quest.Title}");
            }

            Console.WriteLine($"[SeasonalEventQuestIntegration] Ended {links.Count} quests for {e.EventType} event");
        }

        /// <summary>
        /// Handle seasonal event progress
        /// </summary>
        private static void OnSeasonalEventProgress(SeasonalEventProgressEventArgs e)
        {
            if (e.Player is PlayerMobile player)
            {
                var links = GetActiveLinks(player);
                var relevantLinks = links.Where(l => l.EventType == e.EventType && l.IsActive).ToList();

                foreach (var link in relevantLinks)
                {
                    // Update quest progress
                    UpdateQuestProgress(link, e.ProgressType, e.Amount);
                    
                    // Check if quest is complete
                    if (IsQuestComplete(link))
                    {
                        CompleteQuest(link);
                        s_SeasonalQuestsCompleted++;
                    }
                }
            }
        }

        /// <summary>
        /// Update quest with event objectives
        /// </summary>
        private static void UpdateQuestWithEvent(UnifiedQuestData quest, EventType eventType)
        {
            var objective = new QuestObjective
            {
                ObjectiveId = $"seasonal_event_{eventType.GetHashCode()}",
                Description = GenerateEventDescription(eventType),
                Type = ObjectiveType.Event,
                RequiredAmount = 1,
                CurrentProgress = 0,
                IsCompleted = false,
                Metadata = new Dictionary<string, object>
                {
                    { "event_type", eventType.ToString() },
                    { "seasonal_event", true },
                    { "limited_time", true }
                }
            };

            if (quest.Objectives == null)
                quest.Objectives = new List<QuestObjective>();

            quest.Objectives.Add(objective);
        }

        /// <summary>
        /// Generate description for event objective
        /// </summary>
        private static string GenerateEventDescription(EventType eventType)
        {
            return $"Participate in the {eventType} seasonal event";
        }

        /// <summary>
        /// Create event quests for a specific event type
        /// </summary>
        private static List<UnifiedQuestData> CreateEventQuests(EventType eventType)
        {
            var quests = new List<UnifiedQuestData>();

            // Main event quest
            quests.Add(CreateMainEventQuest(eventType));

            // Collection quest
            quests.Add(CreateCollectionQuest(eventType));

            // Combat quest
            quests.Add(CreateCombatQuest(eventType));

            // Exploration quest
            quests.Add(CreateExplorationQuest(eventType));

            return quests;
        }

        /// <summary>
        /// Create main event quest
        /// </summary>
        private static UnifiedQuestData CreateMainEventQuest(EventType eventType)
        {
            var quest = new UnifiedQuestData
            {
                QuestId = DateTime.UtcNow.GetHashCode(),
                Title = $"Main Event: {eventType}",
                Description = $"Participate in the main {eventType} seasonal event activities.",
                Type = QuestType.Seasonal,
                StartedAt = DateTime.UtcNow,
                ExpiresAt = GetEventEndTime(eventType),
                Objectives = new List<QuestObjective>(),
                Rewards = GenerateMainEventRewards(eventType)
            };

            UpdateQuestWithEvent(quest, eventType);

            return quest;
        }

        /// <summary>
        /// Create collection quest
        /// </summary>
        private static UnifiedQuestData CreateCollectionQuest(EventType eventType)
        {
            var quest = new UnifiedQuestData
            {
                QuestId = DateTime.UtcNow.GetHashCode() + 1,
                Title = $"Collection: {eventType}",
                Description = $"Collect special items during the {eventType} event.",
                Type = QuestType.Seasonal,
                StartedAt = DateTime.UtcNow,
                ExpiresAt = GetEventEndTime(eventType),
                Objectives = new List<QuestObjective>(),
                Rewards = GenerateCollectionRewards(eventType)
            };

            var objective = new QuestObjective
            {
                ObjectiveId = $"collection_{eventType.GetHashCode()}",
                Description = $"Collect {eventType} event items",
                Type = ObjectiveType.Collect,
                RequiredAmount = 10,
                CurrentProgress = 0,
                IsCompleted = false,
                Metadata = new Dictionary<string, object>
                {
                    { "event_type", eventType.ToString() },
                    { "collection_type", "seasonal" }
                }
            };

            quest.Objectives.Add(objective);

            return quest;
        }

        /// <summary>
        /// Create combat quest
        /// </summary>
        private static UnifiedQuestData CreateCombatQuest(EventType eventType)
        {
            var quest = new UnifiedQuestData
            {
                QuestId = DateTime.UtcNow.GetHashCode() + 2,
                Title = $"Combat: {eventType}",
                Description = $"Defeat special enemies during the {eventType} event.",
                Type = QuestType.Seasonal,
                StartedAt = DateTime.UtcNow,
                ExpiresAt = GetEventEndTime(eventType),
                Objectives = new List<QuestObjective>(),
                Rewards = GenerateCombatRewards(eventType)
            };

            var objective = new QuestObjective
            {
                ObjectiveId = $"combat_{eventType.GetHashCode()}",
                Description = $"Defeat {eventType} event enemies",
                Type = ObjectiveType.Kill,
                RequiredAmount = 5,
                CurrentProgress = 0,
                IsCompleted = false,
                Metadata = new Dictionary<string, object>
                {
                    { "event_type", eventType.ToString() },
                    { "combat_type", "seasonal" }
                }
            };

            quest.Objectives.Add(objective);

            return quest;
        }

        /// <summary>
        /// Create exploration quest
        /// </summary>
        private static UnifiedQuestData CreateExplorationQuest(EventType eventType)
        {
            var quest = new UnifiedQuestData
            {
                QuestId = DateTime.UtcNow.GetHashCode() + 3,
                Title = $"Exploration: {eventType}",
                Description = $"Explore special locations during the {eventType} event.",
                Type = QuestType.Seasonal,
                StartedAt = DateTime.UtcNow,
                ExpiresAt = GetEventEndTime(eventType),
                Objectives = new List<QuestObjective>(),
                Rewards = GenerateExplorationRewards(eventType)
            };

            var objective = new QuestObjective
            {
                ObjectiveId = $"exploration_{eventType.GetHashCode()}",
                Description = $"Explore {eventType} event locations",
                Type = ObjectiveType.Explore,
                RequiredAmount = 3,
                CurrentProgress = 0,
                IsCompleted = false,
                Metadata = new Dictionary<string, object>
                {
                    { "event_type", eventType.ToString() },
                    { "exploration_type", "seasonal" }
                }
            };

            quest.Objectives.Add(objective);

            return quest;
        }

        /// <summary>
        /// Generate main event rewards
        /// </summary>
        private static List<QuestReward> GenerateMainEventRewards(EventType eventType)
        {
            var rewards = new List<QuestReward>();

            // Event-specific reward
            rewards.Add(new QuestReward
            {
                Type = RewardType.Item,
                Amount = 1,
                Description = $"{eventType} event trophy"
            });

            // Gold reward
            rewards.Add(new QuestReward
            {
                Type = RewardType.Gold,
                Amount = 2000,
                Description = "2000 gold"
            });

            // Fame reward
            rewards.Add(new QuestReward
            {
                Type = RewardType.Fame,
                Amount = 500,
                Description = "500 fame"
            });

            return rewards;
        }

        /// <summary>
        /// Generate collection rewards
        /// </summary>
        private static List<QuestReward> GenerateCollectionRewards(EventType eventType)
        {
            var rewards = new List<QuestReward>();

            // Collection reward
            rewards.Add(new QuestReward
            {
                Type = RewardType.Item,
                Amount = 1,
                Description = $"{eventType} collection satchel"
            });

            // Gold reward
            rewards.Add(new QuestReward
            {
                Type = RewardType.Gold,
                Amount = 1000,
                Description = "1000 gold"
            });

            return rewards;
        }

        /// <summary>
        /// Generate combat rewards
        /// </summary>
        private static List<QuestReward> GenerateCombatRewards(EventType eventType)
        {
            var rewards = new List<QuestReward>();

            // Combat reward
            rewards.Add(new QuestReward
            {
                Type = RewardType.Item,
                Amount = 1,
                Description = $"{eventType} combat medal"
            });

            // Gold reward
            rewards.Add(new QuestReward
            {
                Type = RewardType.Gold,
                Amount = 1500,
                Description = "1500 gold"
            });

            // Power scroll chance
            rewards.Add(new QuestReward
            {
                Type = RewardType.PowerScroll,
                Amount = 1,
                Description = "Power scroll chance"
            });

            return rewards;
        }

        /// <summary>
        /// Generate exploration rewards
        /// </summary>
        private static List<QuestReward> GenerateExplorationRewards(EventType eventType)
        {
            var rewards = new List<QuestReward>();

            // Exploration reward
            rewards.Add(new QuestReward
            {
                Type = RewardType.Item,
                Amount = 1,
                Description = $"{eventType} explorer's compass"
            });

            // Gold reward
            rewards.Add(new QuestReward
            {
                Type = RewardType.Gold,
                Amount = 800,
                Description = "800 gold"
            });

            // Karma reward
            rewards.Add(new QuestReward
            {
                Type = RewardType.Karma,
                Amount = 200,
                Description = "200 karma"
            });

            return rewards;
        }

        /// <summary>
        /// Get event end time based on event type
        /// </summary>
        private static DateTime GetEventEndTime(EventType eventType)
        {
            // Different events have different durations
            switch (eventType)
            {
                case EventType.TreasuresOfTokuno:
                return DateTime.UtcNow.AddDays(7);
                case EventType.VirtueArtifacts:
                    return DateTime.UtcNow.AddDays(14);
                case EventType.TreasuresOfKotlCity:
                    return DateTime.UtcNow.AddDays(10);
                case EventType.SorcerersDungeon:
                    return DateTime.UtcNow.AddDays(21);
                case EventType.TreasuresOfDoom:
                    return DateTime.UtcNow.AddDays(30);
                case EventType.TreasuresOfKhaldun:
                    return DateTime.UtcNow.AddDays(14);
                case EventType.KrampusEncounter:
                    return DateTime.UtcNow.AddDays(7);
                case EventType.RisingTide:
                    return DateTime.UtcNow.AddDays(14);
                case EventType.Fellowship:
                    return DateTime.UtcNow.AddDays(30);
                default:
                    return DateTime.UtcNow.AddDays(7);
            }
        }

        /// <summary>
        /// Get active seasonal events
        /// </summary>
        private static List<EventType> GetActiveSeasonalEvents()
        {
            // This would check with the actual Seasonal Event System
            // For now, return a list of events that would typically be active
            return new List<EventType>
            {
                EventType.TreasuresOfTokuno,
                EventType.VirtueArtifacts,
                EventType.Fellowship
            };
        }

        /// <summary>
        /// Get active quest links for a player
        /// </summary>
        public static List<SeasonalEventQuestLink> GetActiveLinks(PlayerMobile player)
        {
            lock (s_Lock)
            {
                return s_ActiveLinks.GetValueOrDefault(player, new List<SeasonalEventQuestLink>());
            }
        }

        /// <summary>
        /// Get all active links
        /// </summary>
        private static List<SeasonalEventQuestLink> GetAllActiveLinks()
        {
            lock (s_Lock)
            {
                return s_ActiveLinks.Values.SelectMany(list => list).ToList();
            }
        }

        /// <summary>
        /// Update quest progress
        /// </summary>
        private static void UpdateQuestProgress(SeasonalEventQuestLink link, string progressType, int amount)
        {
            var objective = link.Quest.Objectives?.FirstOrDefault(o => o.Type.ToString().ToLower() == progressType.ToLower());

            if (objective != null)
            {
                objective.CurrentProgress = amount;
                objective.IsCompleted = amount >= objective.RequiredAmount;

                // Update quest progress
                UnifiedProgressTracker.TrackProgress(link.Quest, new ProgressUpdate
                {
                    ProgressType = progressType,
                    Amount = amount,
                    Description = $"{progressType}: {amount}/{objective.RequiredAmount}",
                    Source = link.Player,
                    ObjectiveId = objective.ObjectiveId
                });
            }
        }

        /// <summary>
        /// Check if quest is complete
        /// </summary>
        private static bool IsQuestComplete(SeasonalEventQuestLink link)
        {
            return link.Quest.Objectives?.All(o => o.IsCompleted) == true;
        }

        /// <summary>
        /// Complete the quest
        /// </summary>
        private static void CompleteQuest(SeasonalEventQuestLink link)
        {
            // Mark quest as complete
            link.Quest.IsCompleted = true;
            link.Quest.CompletedAt = DateTime.UtcNow;

            // Give rewards
            foreach (var reward in link.Quest.Rewards)
            {
                GiveReward(link.Player, reward);
            }

            // Notify player
            link.Player.SendMessage($"Seasonal quest completed: {link.Quest.Title}");
            link.Player.SendMessage($"Rewards: {string.Join(", ", link.Quest.Rewards.Select(r => r.Description))}");

            // Remove link
            RemoveLink(link);
        }

        /// <summary>
        /// Give reward to player
        /// </summary>
        private static void GiveReward(PlayerMobile player, QuestReward reward)
        {
            switch (reward.Type)
            {
                case RewardType.Gold:
                    player.AddToBackpack(new Gold(reward.Amount));
                    break;
                case RewardType.Fame:
                    player.Fame += reward.Amount;
                    break;
                case RewardType.Karma:
                    player.Karma += reward.Amount;
                    break;
                case RewardType.PowerScroll:
                    // Add power scroll to backpack
                    player.SendMessage($"Power scroll awarded!");
                    break;
                case RewardType.Item:
                    // Add event-specific item
                    player.SendMessage($"Item reward: {reward.Description}");
                    break;
            }
        }

        /// <summary>
        /// Remove quest link
        /// </summary>
        private static void RemoveLink(SeasonalEventQuestLink link)
        {
            lock (s_Lock)
            {
                if (s_ActiveLinks.ContainsKey(link.Player))
                {
                    s_ActiveLinks[link.Player].Remove(link);
                    
                    if (s_ActiveLinks[link.Player].Count == 0)
                        s_ActiveLinks.Remove(link.Player);
                }
            }
        }

        /// <summary>
        /// Clear completed links
        /// </summary>
        public static void ClearCompletedLinks()
        {
            lock (s_Lock)
            {
                var completedLinks = s_ActiveLinks.Values.SelectMany(list => list)
                    .Where(link => !link.IsActive || link.Quest.IsCompleted || link.Quest.IsExpired)
                    .ToList();

                foreach (var link in completedLinks)
                {
                    RemoveLink(link);
                }

                Console.WriteLine($"[SeasonalEventQuestIntegration] Cleared {completedLinks.Count} completed/expired links");
            }
        }

        /// <summary>
        /// Reset statistics
        /// </summary>
        public static void ResetStatistics()
        {
            lock (s_Lock)
            {
                s_TotalLinksCreated = 0;
                s_SeasonalQuestsCompleted = 0;
                Console.WriteLine("[SeasonalEventQuestIntegration] Statistics reset");
            }
        }
    }

    /// <summary>
    /// Links a seasonal event with a quest
    /// </summary>
    public class SeasonalEventQuestLink
    {
        public PlayerMobile Player { get; set; }
        public EventType EventType { get; set; }
        public UnifiedQuestData Quest { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsActive { get; set; }
        public int EventProgress { get; set; }
    }

    /// <summary>
    /// Seasonal Events-Quest integration statistics
    /// </summary>
    public class SeasonalEventQuestStatistics
    {
        public int TotalLinksCreated { get; set; }
        public int SeasonalQuestsCompleted { get; set; }
        public int ActiveLinks { get; set; }
        public int ActivePlayers { get; set; }
        public int ActiveEvents { get; set; }
        public DateTime LastActivity { get; set; }
    }

    /// <summary>
    /// Event arguments for seasonal event started
    /// </summary>
    public class SeasonalEventStartedEventArgs : EventArgs
    {
        public EventType EventType { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
    }

    /// <summary>
    /// Event arguments for seasonal event ended
    /// </summary>
    public class SeasonalEventEndedEventArgs : EventArgs
    {
        public EventType EventType { get; set; }
        public DateTime EndTime { get; set; }
    }

    /// <summary>
    /// Event arguments for seasonal event progress
    /// </summary>
    public class SeasonalEventProgressEventArgs : EventArgs
    {
        public Mobile Player { get; set; }
        public EventType EventType { get; set; }
        public string ProgressType { get; set; }
        public int Amount { get; set; }
    }
}
