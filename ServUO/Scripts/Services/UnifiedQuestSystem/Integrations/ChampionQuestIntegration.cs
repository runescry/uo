using System;
using System.Collections.Generic;
using System.Linq;
using Server.Mobiles;
using Server.Items;
using Server.Engines.CannedEvil;
using Server.Services.UnifiedQuestSystem;

namespace Server.Services.UnifiedQuestSystem.Integrations
{
    /// <summary>
    /// Integration bridge between Unified Quest System and Champion System
    /// Connects quests with champion spawns and progression
    /// </summary>
    public static class ChampionQuestIntegration
    {
        private static readonly Dictionary<PlayerMobile, List<ChampionQuestLink>> s_ActiveLinks;
        private static readonly Dictionary<ChampionSpawn, List<UnifiedQuestData>> s_ChampionQuests;
        private static readonly object s_Lock = new object();
        private static bool s_Initialized = false;
        private static int s_TotalLinksCreated = 0;
        private static int s_ChampionQuestsCompleted = 0;

        static ChampionQuestIntegration()
        {
            s_ActiveLinks = new Dictionary<PlayerMobile, List<ChampionQuestLink>>();
            s_ChampionQuests = new Dictionary<ChampionSpawn, List<UnifiedQuestData>>();
        }

        /// <summary>
        /// Initialize the Champion-Quest integration
        /// </summary>
        public static void Initialize()
        {
            if (s_Initialized)
                return;

            s_Initialized = true;

            // Register event handlers
            EventSink.OnChampionSpawnCreated += OnChampionSpawnCreated;
            EventSink.OnChampionSpawnKilled += OnChampionSpawnKilled;
            EventSink.OnChampionLevelChanged += OnChampionLevelChanged;

            Console.WriteLine("[ChampionQuestIntegration] Initialized Champion-Quest integration");
        }

        /// <summary>
        /// Create a quest link for champion participation
        /// </summary>
        public static ChampionQuestLink CreateQuestLink(PlayerMobile player, ChampionSpawn spawn, UnifiedQuestData quest)
        {
            if (player == null || spawn == null || quest == null)
                return null;

            lock (s_Lock)
            {
                var link = new ChampionQuestLink
                {
                    Player = player,
                    ChampionSpawn = spawn,
                    Quest = quest,
                    JoinedAt = DateTime.UtcNow,
                    IsActive = true,
                    Kills = 0,
                    Level = spawn.Level
                };

                // Add to active links
                if (!s_ActiveLinks.ContainsKey(player))
                    s_ActiveLinks[player] = new List<ChampionQuestLink>();

                s_ActiveLinks[player].Add(link);
                s_TotalLinksCreated++;

                // Update quest with champion objectives
                UpdateQuestWithChampion(quest, spawn);

                Console.WriteLine($"[ChampionQuestIntegration] Created quest link for {player.Name}: {quest.Title}");

                return link;
            }
        }

        /// <summary>
        /// Generate champion quests for a spawn
        /// </summary>
        public static List<UnifiedQuestData> GenerateChampionQuests(ChampionSpawn spawn)
        {
            var quests = new List<UnifiedQuestData>();

            lock (s_Lock)
            {
                if (!s_ChampionQuests.ContainsKey(spawn))
                    s_ChampionQuests[spawn] = new List<UnifiedQuestData>();

                // Create different types of champion quests
                quests.Add(CreateParticipationQuest(spawn));
                quests.Add(CreateKillQuest(spawn));
                quests.Add(CreateLevelQuest(spawn));

                // Store quests for this spawn
                s_ChampionQuests[spawn].AddRange(quests);
            }

            return quests;
        }

        /// <summary>
        /// Get available champion quests for a player
        /// </summary>
        public static List<UnifiedQuestData> GetAvailableQuests(PlayerMobile player)
        {
            var availableQuests = new List<UnifiedQuestData>();

            lock (s_Lock)
            {
                // Get quests from nearby champion spawns
                var nearbySpawns = GetNearbyChampionSpawns(player, 20);
                
                foreach (var spawn in nearbySpawns)
                {
                    if (s_ChampionQuests.ContainsKey(spawn))
                    {
                        var spawnQuests = s_ChampionQuests[spawn];
                        availableQuests.AddRange(spawnQuests);
                    }
                }
            }

            return availableQuests;
        }

        /// <summary>
        /// Get integration statistics
        /// </summary>
        public static ChampionQuestStatistics GetStatistics()
        {
            lock (s_Lock)
            {
                return new ChampionQuestStatistics
                {
                    TotalLinksCreated = s_TotalLinksCreated,
                    ChampionQuestsCompleted = s_ChampionQuestsCompleted,
                    ActiveLinks = s_ActiveLinks.Values.Sum(list => list.Count),
                    ActivePlayers = s_ActiveLinks.Count,
                    ActiveChampions = s_ChampionQuests.Count,
                    LastActivity = DateTime.UtcNow
                };
            }
        }

        /// <summary>
        /// Handle champion spawn creation
        /// </summary>
        private static void OnChampionSpawnCreated(ChampionSpawnCreatedEventArgs e)
        {
            // Generate quests for the new champion spawn
            var quests = GenerateChampionQuests(e.Spawn);
            
            Console.WriteLine($"[ChampionQuestIntegration] Generated {quests.Count} quests for champion spawn at {e.Spawn.Location}");
        }

        /// <summary>
        /// Handle champion spawn killed
        /// </summary>
        private static void OnChampionSpawnKilled(ChampionSpawnKilledEventArgs e)
        {
            if (e.Killer is PlayerMobile player)
            {
                var links = GetActiveLinks(player);
                var relevantLinks = links.Where(l => l.ChampionSpawn == e.Spawn && l.IsActive).ToList();

                foreach (var link in relevantLinks)
                {
                    // Update quest progress
                    UpdateQuestProgress(link, "champion_killed", 1);
                    
                    // Complete kill quests
                    if (link.Quest.Objectives.Any(o => o.Type == ObjectiveType.Kill))
                    {
                        CompleteQuest(link);
                        s_ChampionQuestsCompleted++;
                    }
                }
            }
        }

        /// <summary>
        /// Handle champion level change
        /// </summary>
        private static void OnChampionLevelChanged(ChampionLevelChangedEventArgs e)
        {
            var links = GetAllActiveLinks()
                .Where(l => l.ChampionSpawn == e.Spawn && l.IsActive)
                .ToList();

            foreach (var link in links)
            {
                link.Level = e.NewLevel;
                
                // Update level-based quest progress
                UpdateQuestProgress(link, "champion_level", e.NewLevel);
                
                // Check if level quest is complete
                if (IsLevelQuestComplete(link))
                {
                    CompleteQuest(link);
                    s_ChampionQuestsCompleted++;
                }
            }
        }

        /// <summary>
        /// Update quest with champion objectives
        /// </summary>
        private static void UpdateQuestWithChampion(UnifiedQuestData quest, ChampionSpawn spawn)
        {
            var objective = new QuestObjective
            {
                ObjectiveId = $"champion_{spawn.GetHashCode()}",
                Description = GenerateChampionDescription(spawn),
                Type = ObjectiveType.Kill,
                RequiredAmount = 1,
                CurrentProgress = 0,
                IsCompleted = false,
                Metadata = new Dictionary<string, object>
                {
                    { "champion_type", spawn.ChampionType.ToString() },
                    { "champion_level", spawn.Level },
                    { "spawn_location", spawn.Location.ToString() },
                    { "spawn_map", spawn.Map.ToString() }
                }
            };

            if (quest.Objectives == null)
                quest.Objectives = new List<QuestObjective>();

            quest.Objectives.Add(objective);
        }

        /// <summary>
        /// Generate description for champion objective
        /// </summary>
        private static string GenerateChampionDescription(ChampionSpawn spawn)
        {
            var championType = spawn.ChampionType.ToString();
            var level = spawn.Level;
            var location = spawn.Location;

            return $"Defeat the {championType} Champion (Level {level}) at {location}";
        }

        /// <summary>
        /// Create participation quest
        /// </summary>
        private static UnifiedQuestData CreateParticipationQuest(ChampionSpawn spawn)
        {
            var quest = new UnifiedQuestData
            {
                QuestId = DateTime.UtcNow.GetHashCode(),
                Title = $"Champion Participation: {spawn.ChampionType}",
                Description = $"Participate in the {spawn.ChampionType} Champion spawn event.",
                Type = QuestType.Champion,
                StartedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddHours(4),
                Objectives = new List<QuestObjective>(),
                Rewards = GenerateParticipationRewards(spawn)
            };

            var objective = new QuestObjective
            {
                ObjectiveId = $"champion_participation_{spawn.GetHashCode()}",
                Description = $"Participate in {spawn.ChampionType} Champion event",
                Type = ObjectiveType.Participate,
                RequiredAmount = 1,
                CurrentProgress = 0,
                IsCompleted = false,
                Metadata = new Dictionary<string, object>
                {
                    { "champion_type", spawn.ChampionType.ToString() },
                    { "spawn_location", spawn.Location.ToString() }
                }
            };

            quest.Objectives.Add(objective);

            return quest;
        }

        /// <summary>
        /// Create kill quest
        /// </summary>
        private static UnifiedQuestData CreateKillQuest(ChampionSpawn spawn)
        {
            var quest = new UnifiedQuestData
            {
                QuestId = DateTime.UtcNow.GetHashCode() + 1,
                Title = $"Champion Slayer: {spawn.ChampionType}",
                Description = $"Defeat the {spawn.ChampionType} Champion.",
                Type = QuestType.Champion,
                StartedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddHours(6),
                Objectives = new List<QuestObjective>(),
                Rewards = GenerateKillRewards(spawn)
            };

            UpdateQuestWithChampion(quest, spawn);

            return quest;
        }

        /// <summary>
        /// Create level quest
        /// </summary>
        private static UnifiedQuestData CreateLevelQuest(ChampionSpawn spawn)
        {
            var quest = new UnifiedQuestData
            {
                QuestId = DateTime.UtcNow.GetHashCode() + 2,
                Title = $"Champion Level: {spawn.ChampionType}",
                Description = $"Help reach level {spawn.Level + 1} in the {spawn.ChampionType} Champion spawn.",
                Type = QuestType.Champion,
                StartedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddHours(8),
                Objectives = new List<QuestObjective>(),
                Rewards = GenerateLevelRewards(spawn)
            };

            var objective = new QuestObjective
            {
                ObjectiveId = $"champion_level_{spawn.GetHashCode()}",
                Description = $"Reach level {spawn.Level + 1} in {spawn.ChampionType} Champion spawn",
                Type = ObjectiveType.Level,
                RequiredAmount = spawn.Level + 1,
                CurrentProgress = spawn.Level,
                IsCompleted = false,
                Metadata = new Dictionary<string, object>
                {
                    { "champion_type", spawn.ChampionType.ToString() },
                    { "current_level", spawn.Level },
                    { "target_level", spawn.Level + 1 }
                }
            };

            quest.Objectives.Add(objective);

            return quest;
        }

        /// <summary>
        /// Generate participation rewards
        /// </summary>
        private static List<QuestReward> GenerateParticipationRewards(ChampionSpawn spawn)
        {
            var rewards = new List<QuestReward>();

            // Base gold reward
            rewards.Add(new QuestReward
            {
                Type = RewardType.Gold,
                Amount = 500,
                Description = "500 gold"
            });

            // Fame reward
            rewards.Add(new QuestReward
            {
                Type = RewardType.Fame,
                Amount = 100,
                Description = "100 fame"
            });

            return rewards;
        }

        /// <summary>
        /// Generate kill rewards
        /// </summary>
        private static List<QuestReward> GenerateKillRewards(ChampionSpawn spawn)
        {
            var rewards = new List<QuestReward>();

            // Gold reward based on champion type
            var goldAmount = CalculateChampionGoldReward(spawn);
            rewards.Add(new QuestReward
            {
                Type = RewardType.Gold,
                Amount = goldAmount,
                Description = $"{goldAmount} gold"
            });

            // Fame reward
            rewards.Add(new QuestReward
            {
                Type = RewardType.Fame,
                Amount = 500,
                Description = "500 fame"
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
        /// Generate level rewards
        /// </summary>
        private static List<QuestReward> GenerateLevelRewards(ChampionSpawn spawn)
        {
            var rewards = new List<QuestReward>();

            // Gold reward
            rewards.Add(new QuestReward
            {
                Type = RewardType.Gold,
                Amount = 1000,
                Description = "1000 gold"
            });

            // Fame reward
            rewards.Add(new QuestReward
            {
                Type = RewardType.Fame,
                Amount = 300,
                Description = "300 fame"
            });

            // Special reward
            rewards.Add(new QuestReward
            {
                Type = RewardType.Special,
                Amount = 1,
                Description = "Champion participation token"
            });

            return rewards;
        }

        /// <summary>
        /// Calculate gold reward based on champion
        /// </summary>
        private static int CalculateChampionGoldReward(ChampionSpawn spawn)
        {
            var baseAmount = 2000;
            var typeBonus = GetChampionTypeBonus(spawn.ChampionType);
            var levelBonus = spawn.Level * 100;

            return baseAmount + typeBonus + levelBonus;
        }

        /// <summary>
        /// Get champion type bonus
        /// </summary>
        private static int GetChampionTypeBonus(ChampionSpawnType type)
        {
            switch (type)
            {
                case ChampionSpawnType.Abyss: return 1000;
                case ChampionSpawnType.Arachnid: return 800;
                case ChampionSpawnType.ColdBlood: return 600;
                case ChampionSpawnType.ForestLord: return 700;
                case ChampionSpawnType.VerminHorde: return 500;
                case ChampionSpawnType.UnholyTerror: return 900;
                case ChampionSpawnType.PainElemental: return 750;
                default: return 0;
            }
        }

        /// <summary>
        /// Get nearby champion spawns
        /// </summary>
        private static List<ChampionSpawn> GetNearbyChampionSpawns(PlayerMobile player, int range)
        {
            var nearbySpawns = new List<ChampionSpawn>();

            foreach (var spawn in ChampionSystem.AllSpawns)
            {
                if (spawn.Map == player.Map && spawn.InRange(player.Location, range))
                {
                    nearbySpawns.Add(spawn);
                }
            }

            return nearbySpawns;
        }

        /// <summary>
        /// Get active quest links for a player
        /// </summary>
        public static List<ChampionQuestLink> GetActiveLinks(PlayerMobile player)
        {
            lock (s_Lock)
            {
                return s_ActiveLinks.GetValueOrDefault(player, new List<ChampionQuestLink>());
            }
        }

        /// <summary>
        /// Get all active links
        /// </summary>
        private static List<ChampionQuestLink> GetAllActiveLinks()
        {
            lock (s_Lock)
            {
                return s_ActiveLinks.Values.SelectMany(list => list).ToList();
            }
        }

        /// <summary>
        /// Update quest progress
        /// </summary>
        private static void UpdateQuestProgress(ChampionQuestLink link, string progressType, int amount)
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
        /// Check if level quest is complete
        /// </summary>
        private static bool IsLevelQuestComplete(ChampionQuestLink link)
        {
            return link.Quest.Objectives?.Any(o => o.Type == ObjectiveType.Level && o.IsCompleted) == true;
        }

        /// <summary>
        /// Complete the quest
        /// </summary>
        private static void CompleteQuest(ChampionQuestLink link)
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
                case RewardType.Fame:
                    player.Fame += reward.Amount;
                    break;
                case RewardType.PowerScroll:
                    // Add power scroll to backpack
                    player.SendMessage($"Power scroll awarded!");
                    break;
                case RewardType.Special:
                    // Add special reward
                    player.SendMessage($"Special reward: {reward.Description}");
                    break;
            }
        }

        /// <summary>
        /// Remove quest link
        /// </summary>
        private static void RemoveLink(ChampionQuestLink link)
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

                Console.WriteLine($"[ChampionQuestIntegration] Cleared {completedLinks.Count} completed links");
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
                s_ChampionQuestsCompleted = 0;
                Console.WriteLine("[ChampionQuestIntegration] Statistics reset");
            }
        }
    }

    /// <summary>
    /// Links a champion spawn with a quest
    /// </summary>
    public class ChampionQuestLink
    {
        public PlayerMobile Player { get; set; }
        public ChampionSpawn ChampionSpawn { get; set; }
        public UnifiedQuestData Quest { get; set; }
        public DateTime JoinedAt { get; set; }
        public bool IsActive { get; set; }
        public int Kills { get; set; }
        public int Level { get; set; }
    }

    /// <summary>
    /// Champion-Quest integration statistics
    /// </summary>
    public class ChampionQuestStatistics
    {
        public int TotalLinksCreated { get; set; }
        public int ChampionQuestsCompleted { get; set; }
        public int ActiveLinks { get; set; }
        public int ActivePlayers { get; set; }
        public int ActiveChampions { get; set; }
        public DateTime LastActivity { get; set; }
    }

    /// <summary>
    /// Event arguments for champion spawn created
    /// </summary>
    public class ChampionSpawnCreatedEventArgs : EventArgs
    {
        public ChampionSpawn Spawn { get; set; }
    }

    /// <summary>
    /// Event arguments for champion spawn killed
    /// </summary>
    public class ChampionSpawnKilledEventArgs : EventArgs
    {
        public Mobile Killer { get; set; }
        public ChampionSpawn Spawn { get; set; }
    }

    /// <summary>
    /// Event arguments for champion level changed
    /// </summary>
    public class ChampionLevelChangedEventArgs : EventArgs
    {
        public ChampionSpawn Spawn { get; set; }
        public int OldLevel { get; set; }
        public int NewLevel { get; set; }
    }
}
