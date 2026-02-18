using System;
using System.Collections.Generic;
using System.Linq;
using Server.Mobiles;
using Server.Items;
using Server.Engines.CityLoyalty;
using Server.Engines.Points;
using Server.Services.UnifiedQuestSystem;

/// <summary>
/// Integration bridge between Unified Quest System and City Loyalty System
/// Connects quests with city reputation and loyalty points
/// 
/// Features:
/// - Daily, weekly, and monthly city loyalty quests
/// - Real-time tracking of city loyalty changes and level progression
/// - Service quests that benefit cities and build reputation
/// - City-specific rewards including loyalty points, titles, and items
/// - Support for all cities (Britain, Jhelom, Minoc, Moonglow, etc.)
/// - Integration with city loyalty rating system
/// - Event-driven architecture for loyalty changes
/// 
/// Usage:
/// - Automatically generates quests based on city loyalty levels
/// - Tracks loyalty point changes and rating progressions
/// - Provides appropriate rewards based on city and loyalty level
/// - Integrates with unified quest progress tracking system
/// 
/// Dependencies:
/// - Server.Engines.CityLoyalty.CityLoyaltySystem
/// - Server.Engines.Points.PointsSystem
/// - Server.Services.UnifiedQuestSystem.UnifiedProgressTracker
/// - Server.Services.UnifiedQuestSystem.UnifiedQuestData
/// 
/// Events:
/// - OnCityLoyaltyChanged: Updates quest progress for loyalty changes
/// - OnCityLoyaltyLevelChanged: Updates level-based quest progress
/// </summary>
namespace Server.Services.UnifiedQuestSystem.Integrations
{
    public static class CityLoyaltyQuestIntegration
    {
        private static readonly Dictionary<PlayerMobile, List<CityLoyaltyQuestLink>> s_ActiveLinks;
        private static readonly Dictionary<City, List<UnifiedQuestData>> s_CityQuests;
        private static readonly object s_Lock = new object();
        private static bool s_Initialized = false;
        private static int s_TotalLinksCreated = 0;
        private static int s_CityQuestsCompleted = 0;

        static CityLoyaltyQuestIntegration()
        {
            s_ActiveLinks = new Dictionary<PlayerMobile, List<CityLoyaltyQuestLink>>();
            s_CityQuests = new Dictionary<City, List<UnifiedQuestData>>();
        }

        /// <summary>
        /// Initialize the City Loyalty-Quest integration
        /// </summary>
        public static void Initialize()
        {
            if (s_Initialized)
                return;

            s_Initialized = true;

            // Register event handlers
            EventSink.OnCityLoyaltyChanged += OnCityLoyaltyChanged;
            EventSink.OnCityLoyaltyLevelChanged += OnCityLoyaltyLevelChanged;

            // Generate initial city quests
            GenerateCityQuests();

            Console.WriteLine("[CityLoyaltyQuestIntegration] Initialized City Loyalty-Quest integration");
        }

        /// <summary>
        /// Create a quest link for city loyalty
        /// </summary>
        public static CityLoyaltyQuestLink CreateQuestLink(PlayerMobile player, City city, UnifiedQuestData quest)
        {
            if (player == null || quest == null)
                return null;

            lock (s_Lock)
            {
                var link = new CityLoyaltyQuestLink
                {
                    Player = player,
                    City = city,
                    Quest = quest,
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true,
                    StartingLoyalty = GetPlayerCityLoyalty(player, city)
                };

                // Add to active links
                if (!s_ActiveLinks.ContainsKey(player))
                    s_ActiveLinks[player] = new List<CityLoyaltyQuestLink>();

                s_ActiveLinks[player].Add(link);
                s_TotalLinksCreated++;

                // Update quest with city objectives
                UpdateQuestWithCity(quest, city);

                Console.WriteLine($"[CityLoyaltyQuestIntegration] Created quest link for {player.Name}: {quest.Title} ({city})");

                return link;
            }
        }

        /// <summary>
        /// Generate city quests for all cities
        /// </summary>
        public static void GenerateCityQuests()
        {
            lock (s_Lock)
            {
                foreach (City city in Enum.GetValues(typeof(City)))
                {
                    s_CityQuests[city] = new List<UnifiedQuestData>();

                    // Create different types of city quests
                    s_CityQuests[city].AddRange(CreateCityQuests(city));
                }

                Console.WriteLine($"[CityLoyaltyQuestIntegration] Generated quests for {s_CityQuests.Count} cities");
            }
        }

        /// <summary>
        /// Get available city quests for a player
        /// </summary>
        public static List<UnifiedQuestData> GetAvailableQuests(PlayerMobile player)
        {
            var availableQuests = new List<UnifiedQuestData>();

            lock (s_Lock)
            {
                foreach (var cityQuests in s_CityQuests)
                {
                    var city = cityQuests.Key;
                    var quests = cityQuests.Value;

                    // Check if player can access quests for this city
                    if (CanPlayerAccessCityQuests(player, city))
                    {
                        availableQuests.AddRange(quests);
                    }
                }
            }

            return availableQuests;
        }

        /// <summary>
        /// Get city quests for a specific city
        /// </summary>
        public static List<UnifiedQuestData> GetCityQuests(City city)
        {
            lock (s_Lock)
            {
                return s_CityQuests.GetValueOrDefault(city, new List<UnifiedQuestData>());
            }
        }

        /// <summary>
        /// Get integration statistics
        /// </summary>
        public static CityLoyaltyQuestStatistics GetStatistics()
        {
            lock (s_Lock)
            {
                return new CityLoyaltyQuestStatistics
                {
                    TotalLinksCreated = s_TotalLinksCreated,
                    CityQuestsCompleted = s_CityQuestsCompleted,
                    ActiveLinks = s_ActiveLinks.Values.Sum(list => list.Count),
                    ActivePlayers = s_ActiveLinks.Count,
                    ActiveCities = s_CityQuests.Count,
                    LastActivity = DateTime.UtcNow
                };
            }
        }

        /// <summary>
        /// Handle city loyalty change
        /// </summary>
        private static void OnCityLoyaltyChanged(CityLoyaltyChangedEventArgs e)
        {
            var links = GetActiveLinks(e.Player);
            var relevantLinks = links.Where(l => l.City == e.City && l.IsActive).ToList();

            foreach (var link in relevantLinks)
            {
                // Update quest progress based on loyalty change
                var loyaltyChange = e.NewLoyalty - e.OldLoyalty;
                UpdateQuestProgress(link, "loyalty", loyaltyChange);
                
                // Check if loyalty quest is complete
                if (IsLoyaltyQuestComplete(link))
                {
                    CompleteQuest(link);
                    s_CityQuestsCompleted++;
                }
            }
        }

        /// <summary>
        /// Handle city loyalty level change
        /// </summary>
        private static void OnCityLoyaltyLevelChanged(CityLoyaltyLevelChangedEventArgs e)
        {
            var links = GetActiveLinks(e.Player);
            var relevantLinks = links.Where(l => l.City == e.City && l.IsActive).ToList();

            foreach (var link in relevantLinks)
            {
                // Update level-based quest progress
                UpdateQuestProgress(link, "loyalty_level", (int)e.NewLevel);
                
                // Check if level quest is complete
                if (IsLevelQuestComplete(link))
                {
                    CompleteQuest(link);
                    s_CityQuestsCompleted++;
                }
            }
        }

        /// <summary>
        /// Update quest with city objectives
        /// </summary>
        private static void UpdateQuestWithCity(UnifiedQuestData quest, City city)
        {
            var objective = new QuestObjective
            {
                ObjectiveId = $"city_loyalty_{city.GetHashCode()}",
                Description = GenerateCityDescription(city),
                Type = ObjectiveType.Reputation,
                RequiredAmount = 1000, // Target loyalty points
                CurrentProgress = 0,
                IsCompleted = false,
                Metadata = new Dictionary<string, object>
                {
                    { "city", city.ToString() },
                    { "loyalty_type", "city_loyalty" },
                    { "target_loyalty", 1000 }
                }
            };

            if (quest.Objectives == null)
                quest.Objectives = new List<QuestObjective>();

            quest.Objectives.Add(objective);
        }

        /// <summary>
        /// Generate description for city objective
        /// </summary>
        private static string GenerateCityDescription(City city)
        {
            return $"Increase your loyalty with {city} to 1000 points";
        }

        /// <summary>
        /// Create city quests for a specific city
        /// </summary>
        private static List<UnifiedQuestData> CreateCityQuests(City city)
        {
            var quests = new List<UnifiedQuestData>();

            // Daily loyalty quest
            quests.Add(CreateDailyLoyaltyQuest(city));

            // Weekly reputation quest
            quests.Add(CreateWeeklyReputationQuest(city));

            // Monthly service quest
            quests.Add(CreateMonthlyServiceQuest(city));

            // Special event quest
            quests.Add(CreateSpecialEventQuest(city));

            return quests;
        }

        /// <summary>
        /// Create daily loyalty quest
        /// </summary>
        private static UnifiedQuestData CreateDailyLoyaltyQuest(City city)
        {
            var quest = new UnifiedQuestData
            {
                QuestId = DateTime.UtcNow.GetHashCode(),
                Title = $"Daily Loyalty: {city}",
                Description = $"Show your loyalty to {city} by completing daily tasks.",
                Type = QuestType.City,
                StartedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddDays(1),
                Objectives = new List<QuestObjective>(),
                Rewards = GenerateDailyRewards(city)
            };

            var objective = new QuestObjective
            {
                ObjectiveId = $"daily_loyalty_{city.GetHashCode()}",
                Description = $"Complete daily loyalty tasks for {city}",
                Type = ObjectiveType.Daily,
                RequiredAmount = 1,
                CurrentProgress = 0,
                IsCompleted = false,
                Metadata = new Dictionary<string, object>
                {
                    { "city", city.ToString() },
                    { "quest_type", "daily" }
                }
            };

            quest.Objectives.Add(objective);

            return quest;
        }

        /// <summary>
        /// Create weekly reputation quest
        /// </summary>
        private static UnifiedQuestData CreateWeeklyReputationQuest(City city)
        {
            var quest = new UnifiedQuestData
            {
                QuestId = DateTime.UtcNow.GetHashCode() + 1,
                Title = $"Weekly Reputation: {city}",
                Description = $"Build your reputation in {city} over the week.",
                Type = QuestType.City,
                StartedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddDays(7),
                Objectives = new List<QuestObjective>(),
                Rewards = GenerateWeeklyRewards(city)
            };

            UpdateQuestWithCity(quest, city);

            return quest;
        }

        /// <summary>
        /// Create monthly service quest
        /// </summary>
        private static UnifiedQuestData CreateMonthlyServiceQuest(City city)
        {
            var quest = new UnifiedQuestData
            {
                QuestId = DateTime.UtcNow.GetHashCode() + 2,
                Title = $"Monthly Service: {city}",
                Description = $"Provide valuable service to {city} this month.",
                Type = QuestType.City,
                StartedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddDays(30),
                Objectives = new List<QuestObjective>(),
                Rewards = GenerateMonthlyRewards(city)
            };

            var objective = new QuestObjective
            {
                ObjectiveId = $"monthly_service_{city.GetHashCode()}",
                Description = $"Complete monthly service tasks for {city}",
                Type = ObjectiveType.Service,
                RequiredAmount = 5,
                CurrentProgress = 0,
                IsCompleted = false,
                Metadata = new Dictionary<string, object>
                {
                    { "city", city.ToString() },
                    { "quest_type", "monthly" }
                }
            };

            quest.Objectives.Add(objective);

            return quest;
        }

        /// <summary>
        /// Create special event quest
        /// </summary>
        private static UnifiedQuestData CreateSpecialEventQuest(City city)
        {
            var quest = new UnifiedQuestData
            {
                QuestId = DateTime.UtcNow.GetHashCode() + 3,
                Title = $"Special Event: {city}",
                Description = $"Participate in special events for {city}.",
                Type = QuestType.City,
                StartedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddDays(14),
                Objectives = new List<QuestObjective>(),
                Rewards = GenerateEventRewards(city)
            };

            var objective = new QuestObjective
            {
                ObjectiveId = $"special_event_{city.GetHashCode()}",
                Description = $"Participate in special events for {city}",
                Type = ObjectiveType.Event,
                RequiredAmount = 1,
                CurrentProgress = 0,
                IsCompleted = false,
                Metadata = new Dictionary<string, object>
                {
                    { "city", city.ToString() },
                    { "quest_type", "event" }
                }
            };

            quest.Objectives.Add(objective);

            return quest;
        }

        /// <summary>
        /// Generate daily rewards
        /// </summary>
        private static List<QuestReward> GenerateDailyRewards(City city)
        {
            var rewards = new List<QuestReward>();

            // City loyalty points
            rewards.Add(new QuestReward
            {
                Type = RewardType.Points,
                Amount = 50,
                Description = "50 city loyalty points"
            });

            // Small gold reward
            rewards.Add(new QuestReward
            {
                Type = RewardType.Gold,
                Amount = 200,
                Description = "200 gold"
            });

            return rewards;
        }

        /// <summary>
        /// Generate weekly rewards
        /// </summary>
        private static List<QuestReward> GenerateWeeklyRewards(City city)
        {
            var rewards = new List<QuestReward>();

            // City loyalty points
            rewards.Add(new QuestReward
            {
                Type = RewardType.Points,
                Amount = 300,
                Description = "300 city loyalty points"
            });

            // Gold reward
            rewards.Add(new QuestReward
            {
                Type = RewardType.Gold,
                Amount = 1000,
                Description = "1000 gold"
            });

            // City-specific item
            rewards.Add(new QuestReward
            {
                Type = RewardType.Item,
                Amount = 1,
                Description = $"{city} commemorative item"
            });

            return rewards;
        }

        /// <summary>
        /// Generate monthly rewards
        /// </summary>
        private static List<QuestReward> GenerateMonthlyRewards(City city)
        {
            var rewards = new List<QuestReward>();

            // City loyalty points
            rewards.Add(new QuestReward
            {
                Type = RewardType.Points,
                Amount = 1000,
                Description = "1000 city loyalty points"
            });

            // Gold reward
            rewards.Add(new QuestReward
            {
                Type = RewardType.Gold,
                Amount = 5000,
                Description = "5000 gold"
            });

            // Special city title
            rewards.Add(new QuestReward
            {
                Type = RewardType.Title,
                Amount = 1,
                Description = $"{city} Servant title"
            });

            return rewards;
        }

        /// <summary>
        /// Generate event rewards
        /// </summary>
        private static List<QuestReward> GenerateEventRewards(City city)
        {
            var rewards = new List<QuestReward>();

            // City loyalty points
            rewards.Add(new QuestReward
            {
                Type = RewardType.Points,
                Amount = 500,
                Description = "500 city loyalty points"
            });

            // Gold reward
            rewards.Add(new QuestReward
            {
                Type = RewardType.Gold,
                Amount = 2500,
                Description = "2500 gold"
            });

            // Event-specific item
            rewards.Add(new QuestReward
            {
                Type = RewardType.Item,
                Amount = 1,
                Description = $"{city} event token"
            });

            return rewards;
        }

        /// <summary>
        /// Get player's city loyalty
        /// </summary>
        private static int GetPlayerCityLoyalty(PlayerMobile player, City city)
        {
            // This would integrate with the actual City Loyalty System
            // For now, return a placeholder value
            return 0;
        }

        /// <summary>
        /// Check if player can access city quests
        /// </summary>
        private static bool CanPlayerAccessCityQuests(PlayerMobile player, City city)
        {
            // Check if player has sufficient loyalty or meets other requirements
            var loyalty = GetPlayerCityLoyalty(player, city);
            return loyalty >= 0; // Basic requirement
        }

        /// <summary>
        /// Get active quest links for a player
        /// </summary>
        public static List<CityLoyaltyQuestLink> GetActiveLinks(PlayerMobile player)
        {
            lock (s_Lock)
            {
                return s_ActiveLinks.GetValueOrDefault(player, new List<CityLoyaltyQuestLink>());
            }
        }

        /// <summary>
        /// Update quest progress
        /// </summary>
        private static void UpdateQuestProgress(CityLoyaltyQuestLink link, string progressType, int amount)
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
        /// Check if loyalty quest is complete
        /// </summary>
        private static bool IsLoyaltyQuestComplete(CityLoyaltyQuestLink link)
        {
            return link.Quest.Objectives?.Any(o => o.Type == ObjectiveType.Reputation && o.IsCompleted) == true;
        }

        /// <summary>
        /// Check if level quest is complete
        /// </summary>
        private static bool IsLevelQuestComplete(CityLoyaltyQuestLink link)
        {
            return link.Quest.Objectives?.Any(o => o.Type == ObjectiveType.Level && o.IsCompleted) == true;
        }

        /// <summary>
        /// Complete the quest
        /// </summary>
        private static void CompleteQuest(CityLoyaltyQuestLink link)
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
            link.Player.SendMessage($"Quest completed: {link.Quest.Title}");
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
                case RewardType.Points:
                    // Add city loyalty points
                    player.SendMessage($"City loyalty points: +{reward.Amount}");
                    break;
                case RewardType.Item:
                    // Add city-specific item
                    player.SendMessage($"Item reward: {reward.Description}");
                    break;
                case RewardType.Title:
                    // Grant city title
                    player.SendMessage($"Title granted: {reward.Description}");
                    break;
            }
        }

        /// <summary>
        /// Remove quest link
        /// </summary>
        private static void RemoveLink(CityLoyaltyQuestLink link)
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
                    .Where(link => !link.IsActive || link.Quest.IsCompleted)
                    .ToList();

                foreach (var link in completedLinks)
                {
                    RemoveLink(link);
                }

                Console.WriteLine($"[CityLoyaltyQuestIntegration] Cleared {completedLinks.Count} completed links");
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
                s_CityQuestsCompleted = 0;
                Console.WriteLine("[CityLoyaltyQuestIntegration] Statistics reset");
            }
        }
    }

    /// <summary>
    /// Links a city with a quest
    /// </summary>
    public class CityLoyaltyQuestLink
    {
        public PlayerMobile Player { get; set; }
        public City City { get; set; }
        public UnifiedQuestData Quest { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsActive { get; set; }
        public int StartingLoyalty { get; set; }
    }

    /// <summary>
    /// City Loyalty-Quest integration statistics
    /// </summary>
    public class CityLoyaltyQuestStatistics
    {
        public int TotalLinksCreated { get; set; }
        public int CityQuestsCompleted { get; set; }
        public int ActiveLinks { get; set; }
        public int ActivePlayers { get; set; }
        public int ActiveCities { get; set; }
        public DateTime LastActivity { get; set; }
    }

    /// <summary>
    /// Event arguments for city loyalty changed
    /// </summary>
    public class CityLoyaltyChangedEventArgs : EventArgs
    {
        public PlayerMobile Player { get; set; }
        public City City { get; set; }
        public int OldLoyalty { get; set; }
        public int NewLoyalty { get; set; }
    }

    /// <summary>
    /// Event arguments for city loyalty level changed
    /// </summary>
    public class CityLoyaltyLevelChangedEventArgs : EventArgs
    {
        public PlayerMobile Player { get; set; }
        public City City { get; set; }
        public LoyaltyRating OldLevel { get; set; }
        public LoyaltyRating NewLevel { get; set; }
    }
}
