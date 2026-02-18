using System;
using System.Collections.Generic;
using System.Linq;
using Server.Mobiles;
using Server.Items;
using Server.Gumps;
using Server.Network;
using Server.Commands;
using Server.Engines.Quests;
using Server.Services.UnifiedQuestSystem;
using Server.Custom.VystiaClasses.Quests;

namespace Server.Services.UnifiedQuestSystem
{
    /// <summary>
    /// Vystia Quest System - Enhanced Unified Quest System with Vystia naming and content
    /// Maintains unified quest infrastructure while using Vystia names and lore
    /// </summary>
    public static class VystiaQuestSystem
    {
        private static readonly Dictionary<string, VystiaQuestDefinition> s_QuestDefinitions;
        private static readonly Dictionary<Mobile, VystiaQuestPlayerData> s_PlayerData;
        private static readonly object s_Lock = new object();
        private static bool s_Initialized = false;
        private static int s_TotalQuestsCreated = 0;
        private static int s_TotalQuestsCompleted = 0;

        static VystiaQuestSystem()
        {
            s_QuestDefinitions = new Dictionary<string, VystiaQuestDefinition>();
            s_PlayerData = new Dictionary<Mobile, VystiaQuestPlayerData>();
            InitializeQuests();
        }

        /// <summary>
        /// Initialize the Vystia Quest System
        /// </summary>
        public static void Initialize()
        {
            if (s_Initialized)
                return;

            s_Initialized = true;

            // Initialize unified quest system first
            UnifiedQuestSystem.Initialize();

            // Register Vystia-specific commands
            CommandSystem.Register("VystiaQuest", AccessLevel.Player, VystiaQuest_OnCommand);
            CommandSystem.Register("VQ", AccessLevel.Player, VystiaQuest_OnCommand);

            Console.WriteLine("[VystiaQuestSystem] Initialized Vystia Quest System");
            Console.WriteLine($"[VystiaQuestSystem] Registered {s_QuestDefinitions.Count} Vystia quest types");
        }

        /// <summary>
        /// Create a Vystia quest
        /// </summary>
        public static UnifiedQuestData CreateVystiaQuest(PlayerMobile owner, string questType, string title, string description)
        {
            if (owner == null || string.IsNullOrEmpty(questType))
                return null;

            lock (s_Lock)
            {
                var questDef = GetQuestDefinition(questType);
                if (questDef == null)
                {
                    owner.SendMessage($"Unknown quest type: {questType}");
                    return null;
                }

                var quest = new UnifiedQuestData
                {
                    QuestId = DateTime.UtcNow.GetHashCode(),
                    Title = title,
                    Description = description,
                    Type = QuestType.Vystia,
                    Owner = owner,
                    Creator = owner,
                    StartedAt = DateTime.UtcNow,
                    ExpiresAt = DateTime.UtcNow.AddDays(questDef.DefaultDuration),
                    Objectives = CreateQuestObjectives(questDef),
                    Rewards = CreateQuestRewards(questDef),
                    Metadata = new Dictionary<string, object>
                    {
                        { "vystia_quest_type", questType },
                        { "vystia_quest_tier", questDef.DefaultTier },
                        { "vystia_quest_difficulty", questDef.DefaultDifficulty }
                    }
                };

                s_TotalQuestsCreated++;
                owner.SendMessage($"Vystia quest created: {title}");
                
                return quest;
            }
        }

        /// <summary>
        /// Get quest definition by quest type
        /// </summary>
        public static VystiaQuestDefinition GetQuestDefinition(string questType)
        {
            return s_QuestDefinitions.GetValueOrDefault(questType);
        }

        /// <summary>
        /// Get player quest data
        /// </summary>
        public static VystiaQuestPlayerData GetPlayerData(Mobile mobile)
        {
            if (mobile == null)
                return null;

            lock (s_Lock)
            {
                if (!s_PlayerData.ContainsKey(mobile))
                {
                    s_PlayerData[mobile] = new VystiaQuestPlayerData(mobile);
                }

                return s_PlayerData[mobile];
            }
        }

        /// <summary>
        /// Complete a Vystia quest
        /// </summary>
        public static bool CompleteVystiaQuest(Mobile mobile, UnifiedQuestData quest)
        {
            if (mobile == null || quest == null)
                return false;

            lock (s_Lock)
            {
                var playerData = GetPlayerData(mobile);
                if (playerData.ActiveQuests.Contains(quest.QuestId))
                {
                    playerData.ActiveQuests.Remove(quest.QuestId);
                    playerData.CompletedQuests.Add(quest.QuestId);
                    s_TotalQuestsCompleted++;

                    // Give rewards
                    foreach (var reward in quest.Rewards)
                    {
                        GiveReward(mobile, reward);
                    }

                    mobile.SendMessage($"Vystia quest completed: {quest.Title}");
                    mobile.SendMessage($"Rewards: {string.Join(", ", quest.Rewards.Select(r => r.Description))}");

                    // Update unified quest system
                    UnifiedProgressTracker.TrackProgress(quest, new ProgressUpdate
                    {
                        ProgressType = "vystia_quest_complete",
                        Amount = 1,
                        Description = $"Completed Vystia quest: {quest.Title}",
                        Source = mobile,
                        QuestId = quest.QuestId
                    });

                    return true;
                }
            }

            return false;
        }

        /// <handle_quest_progress>
        /// Handle Vystia quest progress
        /// </summary>
        public static void HandleQuestProgress(Mobile mobile, int questId, string progressType, int amount, string description)
        {
            lock (s_Lock)
            {
                var playerData = GetPlayerData(mobile);
                if (playerData.ActiveQuests.ContainsKey(questId))
                {
                    var quest = playerData.ActiveQuests[questId];
                    
                    // Update quest objectives
                    foreach (var objective in quest.Objectives)
                    {
                        if (objective.ObjectiveId == progressType)
                        {
                            objective.CurrentProgress = amount;
                            objective.IsCompleted = amount >= objective.RequiredAmount;
                        }
                    }

                    // Check if quest is complete
                    if (quest.Objectives.All(o => o.IsCompleted))
                    {
                        CompleteVystiaQuest(mobile, quest);
                    }

                    // Update unified quest system
                    UnifiedProgressTracker.TrackProgress(quest, new ProgressUpdate
                    {
                        ProgressType = progressType,
                        Amount = amount,
                        Description = description,
                        Source = mobile,
                        QuestId = questId
                    });
                }
            }
        }

        /// <summary>
        /// Get available Vystia quests for a player
        /// </summary>
        public static List<UnifiedQuestData> GetAvailableQuests(PlayerMobile player)
        {
            var availableQuests = new List<UnifiedQuestData>();

            lock (s_Lock)
            {
                var playerData = GetPlayerData(player);
                
                // Generate quests based on player's class and level
                foreach (var questDef in s_QuestDefinitions.Values)
                {
                    if (CanPlayerAcceptQuest(player, questDef))
                    {
                        var quest = CreateVystiaQuest(player, questDef.Type, 
                            $"Vystia {questDef.Name}", 
                            questDef.Description);
                        
                        if (quest != null)
                        {
                            availableQuests.Add(quest);
                        }
                    }
                }

                // Add Vystia faction quests if player is in a faction
                var faction = VystiaFactionSystem.GetPlayerFaction(player);
                if (faction != VystiaFaction.None)
                {
                    var factionQuest = CreateFactionQuest(player, faction);
                    if (factionQuest != null)
                    {
                        availableQuests.Add(factionQuest);
                    }
                }

                // Add Vystia religion quests if player follows a religion
                var religion = VystiaReligionSystem.GetPlayerReligion(player);
                if (religion != VystiaReligion.None)
                {
                    var religionQuest = CreateReligionQuest(player, religion);
                    if (religionQuest != null)
                    {
                        availableQuests.Add(religionQuest);
                    }
                }
            }

            return availableQuests;
        }

        /// <summary>
        /// Create faction quest
        /// </summary>
        private static UnifiedQuestData CreateFactionQuest(PlayerMobile player, VystiaFaction faction)
        {
            var factionDef = VystiaFactionSystem.GetFactionDefinition(faction);
            if (factionDef == null)
                return null;

            return CreateVystiaQuest(player, "faction", 
                $"{factionDef.Name} Quest", 
                $"Complete tasks for the {factionDef.Name} faction.");
        }

        /// <summary>
        /// Create religion quest
        /// </summary>
        private static UnifiedQuestData CreateReligionQuest(PlayerMobile player, VystiaReligion religion)
        {
            var religionDef = VystiaReligionSystem.GetReligionDefinition(religion);
            if (religionDef == null)
                return null;

            return CreateVystiaQuest(player, "religion", 
                $"{religionDef.Name} Quest", 
                $"Complete religious tasks for the {religionDef.Name} religion.");
        }

        /// <summary>
        /// Check if player can accept quest
        /// </summary>
        private static bool CanPlayerAcceptQuest(PlayerMobile player, VystiaQuestDefinition questDef)
        {
            var playerData = GetPlayerData(player);
            var playerClass = VystiaClassSystem.GetPlayerClass(player);
            var playerLevel = playerData.Level;

            // Check class requirements
            if (questDef.RequiredClass != VystiaClass.None && questDef.RequiredClass != playerClass)
                return false;

            // Check level requirements
            if (playerLevel < questDef.MinimumLevel)
                return false;

            // Check active quest limit
            if (playerData.ActiveQuests.Count >= questDef.MaxActiveQuests)
                return false;

            return true;
        }

        /// <summary>
        /// Create quest objectives
        /// </summary>
        private static List<QuestObjective> CreateQuestObjectives(VystiaQuestDefinition questDef)
        {
            var objectives = new List<QuestObjective>();

            // Create objectives based on quest type
            switch (questDef.Type.ToLower())
            {
                case "combat":
                    objectives.Add(new QuestObjective
                    {
                        ObjectiveId = "kill_enemies",
                        Description = $"Defeat {questDef.TargetCount} {questDef.TargetType}",
                        Type = ObjectiveType.Kill,
                        RequiredAmount = questDef.TargetCount,
                        CurrentProgress = 0,
                        IsCompleted = false,
                        Metadata = new Dictionary<string, object>
                        {
                            { "target_type", questDef.TargetType },
                            { "target_count", questDef.TargetCount }
                        }
                    });
                    break;

                case "collection":
                    objectives.Add(new QuestObjective
                    {
                        ObjectiveId = "collect_items",
                        Description = $"Collect {questDef.TargetCount} {questDef.TargetItem}",
                        Type = ObjectiveType.Collect,
                        RequiredAmount = questDef.TargetCount,
                        CurrentProgress = 0,
                        IsCompleted = false,
                        Metadata = new Dictionary<string, object>
                        {
                            { "target_item", questDef.TargetItem },
                            { "target_count", questDef.TargetCount }
                        }
                    });
                    break;

                case "exploration":
                    objectives.Add(new QuestObjective
                    {
                        ObjectiveId = "explore_locations",
                        Description = $"Explore {questDef.TargetCount} locations",
                        Type = ObjectiveType.Explore,
                        RequiredAmount = questDef.TargetCount,
                        CurrentProgress = 0,
                        IsCompleted = false,
                        Metadata = new Dictionary<string, object>
                        {
                            { "exploration_count", questDef.TargetCount }
                        }
                    });
                    break;

                case "crafting":
                    objectives.Add(new QuestObjective
                    {
                        ObjectiveId = "craft_items",
                        Description = $"Craft {questDef.TargetCount} {questDef.TargetItem}",
                        Type = ObjectiveType.Craft,
                        RequiredAmount = questDef.TargetCount,
                        CurrentProgress = 0,
                        IsCompleted = false,
                        Metadata = new Dictionary<string, object>
                        {
                            { "craft_item", questDef.TargetItem },
                            { "craft_count", questDef.TargetCount }
                        }
                    });
                    break;
            }

            return objectives;
        }

        /// <summary>
        /// Create quest rewards
        /// </summary>
        private static List<QuestReward> CreateQuestRewards(VystiaQuestDefinition questDef)
        {
            var rewards = new List<QuestReward>();

            // Base gold reward
            rewards.Add(new QuestReward
            {
                Type = RewardType.Gold,
                Amount = questDef.BaseGoldReward,
                Description = $"{questDef.BaseGoldReward} gold"
            });

            // Experience reward
            rewards.Add(new QuestReward
            {
                Type = RewardType.Experience,
                Amount = questDef.ExperienceReward,
                Description = $"{questDef.ExperienceReward} experience"
            });

            // Vystia-specific rewards
            if (!string.IsNullOrEmpty(questDef.VystiaReward))
            {
                rewards.Add(new QuestReward
                {
                    Type = RewardType.Item,
                    Amount = 1,
                    Description = questDef.VystiaReward
                });
            }

            return rewards;
        }

        /// <summary>
        /// Give reward to player
        /// </summary>
        private static void GiveReward(Mobile mobile, QuestReward reward)
        {
            switch (reward.Type)
            {
                case RewardType.Gold:
                    mobile.AddToBackpack(new Gold(reward.Amount));
                    break;
                case RewardType.Experience:
                    if (mobile is PlayerMobile player)
                    {
                        VystiaClassSystem.AddExperience(player, reward.Amount);
                    }
                    break;
                case RewardType.Item:
                    // This would give the specific Vystia item
                    mobile.SendMessage($"Item reward: {reward.Description}");
                    break;
            }
        }

        /// <summary>
        /// Handle Vystia quest command
        /// </summary>
        private static void VystiaQuest_OnCommand(CommandEventArgs e)
        {
            var from = e.Mobile as PlayerMobile;
            if (from == null)
                return;

            if (e.Length == 0)
            {
                ShowQuestInfo(from);
                return;
            }

            switch (e.GetString(0).ToLower())
            {
                case "list":
                    ShowQuestList(from);
                    break;
                case "accept":
                    AcceptQuest(from, e);
                    break;
                case "abandon":
                    AbandonQuest(from, e);
                    break;
                case "progress":
                    ShowQuestProgress(from);
                    break;
                case "info":
                    ShowQuestInfo(from);
                    break;
                default:
                    ShowQuestInfo(from);
                    break;
            }
        }

        /// <summary>
        /// Show quest information
        /// </summary>
        private static void ShowQuestInfo(Mobile from)
        {
            from.SendMessage("=== VYSTIA QUEST SYSTEM ===");
            from.SendMessage("Commands:");
            from.SendMessage("  [VystiaQuest list] - Show available quests");
            from.SendMessage("  [VystiaQuest accept <questId>] - Accept a quest");
            from.SendMessage("  [VystiaQuest abandon <questId>] - Abandon a quest");
            from.SendMessage("  [VystiaQuest progress] - Show quest progress");
            from.SendMessage("  [VystiaQuest info] - Show this help");
            from.SendMessage("");
            from.SendMessage("Available Quest Types:");
            
            foreach (var questDef in s_QuestDefinitions.Values)
            {
                from.SendMessage($"  {questDef.Type} - {questDef.Name}");
            }
        }

        /// <summary>
        /// Show quest list
        /// </summary>
        private static void ShowQuestList(Mobile from)
        {
            var availableQuests = GetAvailableQuests(from as PlayerMobile);
            
            from.SendMessage($"=== AVAILABLE VYSTIA QUESTS ===");
            from.SendMessage($"Total Available: {availableQuests.Count}");
            
            if (availableQuests.Count > 0)
            {
                from.SendMessage("Quests:");
                for (int i = 0; i < availableQuests.Count; i++)
                {
                    var quest = availableQuests[i];
                    from.SendMessage($"  [{i+1}] {quest.Title} (ID: {quest.QuestId})");
                    from.SendMessage($"      {quest.Description}");
                }
            }
            else
            {
                from.SendMessage("No available quests. Try joining a class, faction, or religion.");
            }
        }

        /// <summary>
        /// Accept a quest
        /// </summary>
        private static void AcceptQuest(Mobile from, CommandEventArgs e)
        {
            if (e.Length < 2)
            {
                from.SendMessage("Usage: [VystiaQuest accept <questId>]");
                return;
            }

            if (!int.TryParse(e.GetString(1), out int questId))
            {
                from.SendMessage("Invalid quest ID format.");
                return;
            }

            // This would accept the quest from the unified quest system
            from.SendMessage($"Quest {questId} accepted!");
        }

        /// <summary>
        /// Abandon a quest
        /// </summary>
        private static void AbandonQuest(Mobile from, CommandEventArgs e)
        {
            if (e.Length < 2)
            {
                from.SendMessage("Usage: [VystiaQuest abandon <questId>]");
                return;
            }

            if (!int.TryParse(e.GetString(1), out int questId))
            {
                from.SendMessage("Invalid quest ID format.");
                return;
            }

            // This would abandon the quest from the unified quest system
            from.SendMessage($"Quest {questId} abandoned!");
        }

        /// <summary>
        /// Show quest progress
        /// </summary>
        private static void ShowQuestProgress(Mobile from)
        {
            var playerData = GetPlayerData(from);
            
            from.SendMessage("=== QUEST PROGRESS ===");
            from.SendMessage($"Active Quests: {playerData.ActiveQuests.Count}");
            from.SendMessage($"Completed Quests: {playerData.CompletedQuests.Count}");
            
            if (playerData.ActiveQuests.Count > 0)
            {
                from.SendMessage("Active Quests:");
                foreach (var questId in playerData.ActiveQuests.Keys)
                {
                    from.SendMessage($"  Quest {questId}");
                }
            }
        }

        /// <summary>
        /// Initialize quest definitions
        /// </summary>
        private static void InitializeQuests()
        {
            // Combat quests
            s_QuestDefinitions["combat"] = new VystiaQuestDefinition
            {
                Type = "combat",
                Name = "Combat Quest",
                Description = "Defeat enemies and complete combat objectives",
                DefaultDuration = 7,
                DefaultTier = "Normal",
                DefaultDifficulty = "Medium",
                TargetType = "enemies",
                TargetCount = 10,
                MinimumLevel = 1,
                MaxActiveQuests = 3,
                BaseGoldReward = 1000,
                ExperienceReward = 500,
                VystiaReward = "Combat Token"
            };

            // Collection quests
            s_QuestDefinitions["collection"] = new VystiaQuestDefinition
            {
                Type = "collection",
                Name = "Collection Quest",
                Description = "Collect items and resources",
                DefaultDuration = 5,
                DefaultTier = "Normal",
                DefaultDifficulty = "Easy",
                TargetItem = "resources",
                TargetCount = 20,
                MinimumLevel = 1,
                MaxActiveQuests = 5,
                BaseGoldReward = 500,
                ExperienceReward = 250,
                VystiaReward = "Collection Token"
            };

            // Exploration quests
            s_QuestDefinitions["exploration"] = new VystiaQuestDefinition
            {
                Type = "exploration",
                Name = "Exploration Quest",
                Description = "Explore locations and discover new areas",
                DefaultDuration = 10,
                DefaultTier = "Normal",
                DefaultDifficulty = "Medium",
                TargetCount = 5,
                MinimumLevel = 1,
                MaxActiveQuests = 4,
                BaseGoldReward = 750,
                ExperienceReward = 400,
                VystiaReward = "Explorer's Compass"
            };

            // Crafting quests
            s_QuestDefinitions["crafting"] = new VystiaQuestDefinition
            {
                Type = "crafting",
                Name = "Crafting Quest",
                Description = "Craft items and equipment",
                DefaultDuration = 3,
                DefaultTier = "Normal",
                DefaultDifficulty = "Medium",
                TargetItem = "equipment",
                TargetCount = 15,
                MinimumLevel = 1,
                MaxActiveQuests = 3,
                BaseGoldReward = 800,
                ExperienceReward = 300,
                VystiaReward = "Crafting Token"
            };

            // Faction quests
            s_QuestDefinitions["faction"] = new VystiaQuestDefinition
            {
                Type = "faction",
                Name = "Faction Quest",
                Description = "Complete tasks for your faction",
                DefaultDuration = 7,
                DefaultTier = "Normal",
                DefaultDifficulty = "Medium",
                TargetType = "enemies",
                TargetCount = 15,
                MinimumLevel = 5,
                MaxActiveQuests = 2,
                BaseGoldReward = 1500,
                ExperienceReward = 750,
                VystiaReward = "Faction Token"
            };

            // Religion quests
            s_QuestDefinitions["religion"] = new VystiaQuestDefinition
            {
                Type = "religion",
                Name = "Religion Quest",
                Description = "Complete religious tasks for your religion",
                DefaultDuration = 14,
                DefaultTier = "Normal",
                DefaultDifficulty = "Easy",
                TargetType = "pilgrimage",
                TargetCount = 1,
                MinimumLevel = 1,
                MaxActiveQuests = 2,
                BaseGoldReward = 1000,
                ExperienceReward = 500,
                VystiaReward = "Devotion Token"
            };
        }

        /// <summary>
        /// Get all quest definitions
        /// </summary>
        public static Dictionary<string, VystiaQuestDefinition> GetAllQuestDefinitions()
        {
            return new Dictionary<string, VystiaQuestDefinition>(s_QuestDefinitions);
        }

        /// <summary>
        /// Get system statistics
        /// </summary>
        public static VystiaQuestStatistics GetStatistics()
        {
            lock (s_Lock)
            {
                var stats = new VystiaQuestStatistics
                {
                    TotalQuestsCreated = s_TotalQuestsCreated,
                    TotalQuestsCompleted = s_TotalQuestsCompleted,
                    ActiveQuests = s_PlayerData.Values.Sum(pd => pd.ActiveQuests.Count),
                    CompletedQuests = s_PlayerData.Values.Sum(pd => pd.CompletedQuests.Count),
                    QuestTypeDistribution = new Dictionary<string, int>(),
                    AverageQuestsPerPlayer = 0.0,
                    LastUpdate = DateTime.UtcNow
                };

                // Count quest type distribution
                foreach (var playerData in s_PlayerData.Values)
                {
                    foreach (var questId in playerData.ActiveQuests)
                    {
                        var quest = UnifiedQuestSystem.GetQuest(questId);
                        if (quest != null)
                        {
                            var questType = quest.Metadata?.GetValueOrDefault("vystia_quest_type", "unknown") as string;
                            if (stats.QuestTypeDistribution.ContainsKey(questType))
                                stats.QuestTypeDistribution[questType]++;
                            else
                                stats.QuestTypeDistribution[questType] = 1;
                        }
                    }
                }

                // Calculate average quests per player
                if (stats.TotalPlayers > 0)
                {
                    stats.AverageQuestsPerPlayer = (double)stats.ActiveQuests / stats.TotalPlayers;
                }

                return stats;
            }
        }
    }

    /// <summary>
    /// Vystia quest definition
    /// </summary>
    public class VystiaQuestDefinition
    {
        public string Type { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int DefaultDuration { get; set; }
        public string DefaultTier { get; set; }
        public string DefaultDifficulty { get; set; }
        public string TargetType { get; set; }
        public int TargetCount { get; set; }
        public int MinimumLevel { get; set; }
        public int MaxActiveQuests { get; set; }
        public int BaseGoldReward { get; set; }
        public int ExperienceReward { get; set; }
        public string VystiaReward { get; set; }
    }

    /// <summary>
    /// Vystia quest player data
    /// </summary>
    public class VystiaQuestPlayerData
    {
        public Mobile Player { get; set; }
        public HashSet<int> ActiveQuests { get; set; }
        public HashSet<int> CompletedQuests { get; set; }
        public int Level { get; set; }
        public DateTime LastQuestUpdate { get; set; }

        public VystiaQuestPlayerData(Mobile player)
        {
            Player = player;
            ActiveQuests = new HashSet<int>();
            CompletedQuests = new HashSet<int>();
            Level = 1;
            LastQuestUpdate = DateTime.UtcNow;
        }
    }

    /// <summary>
    /// Vystia quest statistics
    /// </summary>
    public class VystiaQuestStatistics
    {
        public int TotalQuestsCreated { get; set; }
        public int TotalQuestsCompleted { get; set; }
        public int ActiveQuests { get; set; }
        public int CompletedQuests { get; set; }
        public Dictionary<string, int> QuestTypeDistribution { get; set; }
        public double AverageQuestsPerPlayer { get; set; }
        public DateTime LastUpdate { get; set; }
    }
}
