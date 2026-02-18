using System;
using System.Collections.Generic;
using System.Linq;
using Server.Mobiles;
using Server.Items;
using Server.Gumps;
using Server.Network;
using Server.Commands;
using Server.Services.UnifiedQuestSystem;
using Server.Services.UnifiedQuestSystem.Integrations;

namespace Server.Services.UnifiedQuestSystem
{
    /// <summary>
    /// Vystia Integration Manager - Central coordination for all Vystia system integrations
    /// Coordinates and manages all quest system integrations with Vystia naming and content
    /// </summary>
    public static class VystiaIntegrationManager
    {
        private static readonly Dictionary<string, IVystiaSystemIntegration> s_Integrations;
        private static readonly Dictionary<string, VystiaSystemStatistics> s_SystemStatistics;
        private static readonly object s_Lock = new object();
        private static bool s_Initialized = false;
        private static DateTime s_LastSync = DateTime.UtcNow;

        static VystiaIntegrationManager()
        {
            s_Integrations = new Dictionary<string, IVystiaSystemIntegration>();
            s_SystemStatistics = new Dictionary<string, VystiaSystemStatistics>();
        }

        /// <summary>
        /// Initialize the Vystia Integration Manager
        /// </summary>
        public static void Initialize()
        {
            if (s_Initialized)
                return;

            s_Initialized = true;

            // Register Vystia-specific integrations
            RegisterIntegration("VystiaFaction", new VystiaFactionIntegration());
            RegisterIntegration("VystiaReligion", new VystiaReligionIntegration());
            RegisterIntegration("VystiaClass", new VystiaClassIntegration());
            RegisterIntegration("VystiaQuest", new VystiaQuestIntegration());

            Console.WriteLine("[VystiaIntegrationManager] Initialized Vystia Integration Manager");
            Console.WriteLine($"[VystiaIntegrationManager] Registered {s_Integrations.Count} Vystia integrations");
        }

        /// <summary>
        /// Get integration statistics for all Vystia systems
        /// </summary>
        public static VystiaSystemStatistics GetAllStatistics()
        {
            lock (s_Lock)
            {
                var stats = new VystiaSystemStatistics
                {
                    TotalIntegrations = s_Integrations.Count,
                    IntegrationStatistics = new Dictionary<string, VystiaSystemStatistics>(s_SystemStatistics),
                    LastSync = s_LastSync
                };

                // Collect statistics from all integrations
                foreach (var integration in s_Integrations.Values)
                {
                    var integrationStats = integration.GetStatistics();
                    stats.IntegrationStatistics[integration.Name] = integrationStats;
                }

                return stats;
            }
        }

        /// <summary>
        /// Get available quests from all Vystia integrations
        /// </summary>
        public static List<UnifiedQuestData> GetAllAvailableQuests(PlayerMobile player)
        {
            var allQuests = new List<UnifiedQuestData>();

            lock (s_Lock)
            {
                // Get quests from all integrations
                foreach (var integration in s_Integrations.Values)
                {
                    try
                    {
                        var integrationQuests = integration.GetAvailableQuests(player);
                        if (integrationQuests != null)
                        allQuests.AddRange(integrationQuests);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"[ VystiaIntegrationManager] Error getting quests from {integration.GetType().Name}: {ex.Message}");
                    }
                }
            }

            return allQuests;
        }

        /// <summary>
        /// Sync all Vystia integrations
        /// </summary>
        public static void SyncAllIntegrations()
        {
            lock (s_Lock)
            {
                foreach (var integration in s_Integrations.Values)
                {
                    try
                    integration.Sync();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"[ VystiaIntegrationManager] Error syncing {integration.GetType().Name}: {ex.Message}");
                    }
                }

                s_LastSync = DateTime.UtcNow;
                Console.WriteLine("[VystiaIntegrationManager] Synced all Vystia integrations");
            }
        }

        /// <summary>
        /// Register a Vystia system integration
        /// </summary>
        public static void RegisterIntegration(string name, IVystiaSystemIntegration integration)
        {
            lock (s_Lock)
            {
                s_Integrations[name] = integration;
                s_SystemStatistics[name] = new VystiaSystemStatistics();
                Console.WriteLine($"[ VystiaIntegrationManager] Registered Vystia integration: {name}");
            }
        }

        /// <summary>
        /// Get integration by name
        /// </summary>
        public static IVystiaSystemIntegration GetIntegration(string name)
        {
            lock (s_Lock)
            {
                return s_Integrations.GetValueOrDefault(name);
            }
        }

        /// <summary>
        /// Reset all integration statistics
        /// </summary>
        public static void ResetAllStatistics()
        {
            lock (s_Lock)
            {
                foreach (var stats in s_SystemStatistics.Values)
                {
                    stats.TotalOperations = 0;
                    stats.SuccessfulOperations = 0;
                    stats.FailedOperations = 0;
                    stats.LastSync = DateTime.UtcNow;
                }

                Console.WriteLine("[VystiaIntegrationManager] Reset all integration statistics");
            }
        }

        /// <summary>
        /// Clear all integration data
        /// </summary>
        public static void ClearAllIntegrationData()
        {
            lock (s_Lock)
            {
                foreach (var integration in s_Integrations.Values)
                {
                    integration.ClearData();
                }

                Console.WriteLine("[VystiaIntegrationManager] Cleared all integration data");
            }
        }

        /// <summary>
        /// Get system statistics
        /// </summary>
        public static VystiaSystemStatistics GetSystemStatistics()
        {
            lock (s_Lock)
            {
                return new VystiaSystemStatistics
                {
                    TotalIntegrations = s_Integrations.Count,
                    IntegrationStatistics = new Dictionary<string, VystiaSystemStatistics>(s_SystemStatistics),
                    LastSync = s_LastSync
                };
            }
        }
    }

    /// <summary>
    /// Interface for Vystia system integrations
    /// </summary>
    public interface IVystiaSystemIntegration
    {
        void Initialize();
        List<UnifiedQuestData> GetAvailableQuests(PlayerMobile player);
        VystiaSystemStatistics GetStatistics();
        void Sync();
        void ClearData();
    }

    /// <summary>
    /// Vystia Faction Integration
    /// </summary>
    public class VystiaFactionIntegration : IVystiaSystemIntegration
    {
        public string Name => "VystiaFaction";
        public bool IsInitialized() => true;

        public void Initialize()
        {
            // Initialize Vystia faction system
            VystiaFactionSystem.Initialize();
        }

        public List<UnifiedQuestData> GetAvailableQuests(PlayerMobile player)
        {
            var quests = new List<UnifiedQuestData>();
            
            // Get player's faction
            var faction = VystiaFactionSystem.GetPlayerFaction(player);
            if (faction == VystiaFaction.None)
                return quests;

            // Generate faction-specific quests
            var factionDef = VystiaFactionSystem.GetFactionDefinition(faction);
            if (factionDef != null)
            {
                var quest = VystiaQuestSystem.CreateVystiaQuest(player, "faction", 
                    $"{factionDef.Name} Quest", 
                    $"Complete tasks for the {factionDef.Name} faction.");
                
                if (quest != null)
                    quests.Add(quest);
            }

            return quests;
        }

        public VystiaSystemStatistics GetStatistics()
        {
            var stats = VystiaFactionSystem.GetStatistics();
            return new VystiaSystemStatistics
            {
                TotalOperations = stats.TotalOperations,
                TotalPlayers = stats.TotalPlayers,
                ActiveQuests = stats.ActiveQuests,
                CompletedQuests = stats.CompletedQuests,
                AverageRank = stats.AverageRank,
                LastUpdate = stats.LastUpdate
            };
        }

        public void Sync()
        {
            // Sync Vystia faction data with unified quest system
            var allQuests = UnifiedQuestSystem.GetAllQuests();
            
            lock (s_Lock)
            {
                var playerData = VystiaFactionSystem.GetPlayerData(null);
                foreach (var quest in allQuests)
                {
                    if (quest.Type == QuestType.Vystia)
                    {
                        // Update faction progress for Vystia quests
                        var questDef = VystiaQuestSystem.GetQuestDefinition(
                            quest.Metadata?.GetValueOrDefault("vystia_quest_type", "unknown") as string
                        );
                        
                        if (questDef?.Type == "faction")
                        {
                            // Handle faction-specific quest progress
                            var playerData = VystiaFactionSystem.GetPlayerData(quest.Owner);
                            if (playerData.Faction == faction)
                            {
                                // Update faction progress
                                HandleFactionQuestProgress(player, quest, questDef);
                            }
                        }
                    }
                }
            }
        }

        public void ClearData()
        {
            // Clear Vystia faction data
            VystiaFactionSystem.ClearAllData();
        }

        /// <summary>
        /// Handle faction quest progress
        /// </summary>
        private void HandleFactionQuestProgress(Mobile player, UnifiedQuestData quest, VystiaQuestDefinition questDef)
        {
            // Handle faction-specific quest progress
            switch (questDef.Type.ToLower())
            {
                case "faction":
                    // Update faction reputation
                    var playerData = VystiaFactionSystem.GetPlayerData(player);
                    if (playerData.Faction == faction)
                    {
                        playerData.Silver += 10;
                        playerData.Kills++;
                        
                        // Update quest progress
                        HandleQuestProgress(player, quest, questDef);
                    }
                    break;
            }
        }

        /// <summary>
        /// Handle quest progress
        /// </summary>
        private static void HandleQuestProgress(Mobile player, UnifiedQuestData quest, VystiaQuestDefinition questDef)
        {
            // Update quest objectives based on quest type
            switch (questDef.Type.ToLower())
            {
                case "combat":
                    // Handle combat quest progress
                    foreach (var objective in quest.Objectives)
                    {
                        if (objective.Type == ObjectiveType.Kill)
                        {
                            var playerData = VystiaFactionSystem.GetPlayerData(player);
                            if (playerData.Faction == quest.Metadata?.GetValueOrDefault("vystia_quest_type", "unknown") as string)
                            {
                                // Update kill count
                                playerData.Kills++;
                                playerData.Silver += 5;
                            }
                        }
                    }
                    break;
                case "collection":
                    // Handle collection quest progress
                    foreach (var objective in quest.Objectives)
                    {
                        if (objective.Type == ObjectiveType.Collect)
                        {
                            var playerData = VystiaFactionSystem.GetPlayerData(player);
                            if (playerData.Faction == quest.Metadata?.GetValueOrDefault("vystia_quest_type", "unknown") as string)
                            {
                                // Update collection count
                                playerData.Silver += 3;
                            }
                        }
                    }
                    break;
            }
        }
    }

        /// <summary>
        /// Vystia Religion Integration
        /// </summary>
    public class VystiaReligionIntegration : IVystiaSystemIntegration
    {
        public string Name => "VystiaReligion";
        public bool IsInitialized() => true;

        public void Initialize()
        {
            // Initialize Vystia religion system
            VystiaReligionSystem.Initialize();
        }

        public List<UnifiedQuestData> GetAvailableQuests(PlayerMobile player)
        {
            var quests = new List<UnifiedQuestData>();
            
            // Get player's religion
            var religion = VystiaReligionSystem.GetPlayerReligion(player);
            if (religion == VystiaReligion.None)
                return quests;

            // Generate religion-specific quests
            var religionDef = VystiaReligionSystem.GetReligionDefinition(religion);
            if (religionDef != null)
            {
                var quest = VystiaQuestSystem.CreateVystiaQuest(player, "religion", 
                    $"{religion.Name} Quest", 
                    $"Complete religious tasks for the {religion.Name} religion.");
                
                if (quest != null)
                    quests.Add(quest);
            }

            return quests;
        }

        public VystiaSystemStatistics GetStatistics()
        {
            var stats = VystiaReligionSystem.GetStatistics();
            return new VystiaSystemStatistics
            {
                TotalOperations = stats.TotalOperations,
                TotalPlayers = stats.TotalPlayers,
                ActiveQuests = stats.ActiveQuests,
                CompletedQuests = stats.CompletedQuests,
                AveragePiety = stats.AveragePiety,
                LastPrayer = stats.LastPrayer,
                LastPilgrimage = stats.LastPilgrimage,
                LastTithe = stats.LastTithe
            };
        }

        public void Sync()
        {
            // Sync Vystia religion data with unified quest system
            var allQuests = UnifiedQuestSystem.GetAllQuests();
            
            lock (s_Lock)
            {
                var playerData = VystiaReligionSystem.GetPlayerData(null);
                foreach (var quest in allQuests)
                {
                    if (quest.Type == QuestType.Vystia)
                    {
                        // Update religion progress
                        var questDef = VystiaReligionSystem.GetReligionDefinition(
                            quest.Metadata?.GetValueOrDefault("vystia_quest_type", "unknown") as string
                        );
                        
                        if (questDef?.Type == "religion")
                        {
                            // Handle religion-specific quest progress
                            var playerData = VystiaReligionSystem.GetPlayerData(quest.Owner);
                            if (playerData.Religion == religion)
                            {
                                // Update piety
                                playerData.Piety += 5;
                                playerData.LastPrayer = DateTime.UtcNow;
                                
                                // Update quest progress
                                HandleReligionQuestProgress(player, quest, questDef);
                            }
                        }
                    }
                }
            }
        }

        public void ClearData()
        {
            // Clear Vystia religion data
            VystiaReligionSystem.ClearAllData();
        }

        /// <summary>
        /// Handle religion quest progress
        /// </summary>
        private static void HandleReligionQuestProgress(Mobile player, UnifiedQuestData quest, VystiaQuestDefinition questDef)
        {
            // Handle religion-specific quest progress
            switch (questDef.Type.ToLower())
            {
                case "religion":
                    // Update piety
                    var playerData = VystiaReligionSystem.GetPlayerData(player);
                    if (playerData.Religion == religion)
                    {
                        playerData.Piety += 5;
                        playerData.LastPrayer = DateTime.UtcNow;
                        
                        // Update quest progress
                        HandleQuestProgress(player, quest, questDef);
                    }
                    break;
            }
        }

        /// <summary>
        /// Handle quest progress
        /// </summary>
        private static void HandleQuestProgress(Mobile player, UnifiedQuestData quest, VystiaQuestDefinition questDef)
        {
            // Update quest objectives based on quest type
            switch (questDef.Type.ToLower())
            {
                case "religion":
                    // Update piety
                    var playerData = VystiaReligionSystem.GetPlayerData(player);
                    if (playerData.Religion == religion)
                    {
                        playerData.Piety += 5;
                        playerData.LastPrayer = DateTime.UtcNow;
                        
                        // Update quest progress
                        HandleReligionQuestProgress(player, quest, questDef);
                    }
                    break;
            }
        }
    }

        /// <summary>
        /// Vystia Class Integration
        /// </summary>
    public class VystiaClassIntegration : IVystiaSystemIntegration
        {
        public string Name => "VystiaClass";
        public bool IsInitialized() => true;

        public void Initialize()
        {
            // Initialize Vystia class system
            VystiaClassSystem.Initialize();
        }

        public List<UnifiedQuestData> GetAvailableQuests(PlayerMobile player)
        {
            var quests = new List<UnifiedQuestData>();
            
            // Get player's class
            var playerClass = VystiaClassSystem.GetPlayerClass(player);
            if (playerClass == VystiaClass.None)
                return quests;

            // Generate class-specific quests
            var classDef = VystiaClassSystem.GetClassDefinition(playerClass);
            if (classDef != null)
            {
                var quest = VystiaQuestSystem.CreateVystiaQuest(player, "class", 
                    $"{classDef.Name} Quest", 
                    $"Complete class-specific tasks for the {classDef.Name} class.");
                
                if (quest != null)
                    quests.Add(quest);
            }

            return quests;
        }

        public VystiaSystemStatistics GetStatistics()
        {
            var stats = VystiaClassSystem.GetStatistics();
            return new VystiaSystemStatistics
            {
                TotalOperations = stats.TotalOperations,
                TotalPlayers = stats.TotalPlayers,
                ActiveQuests = stats.ActiveQuests,
                CompletedQuests = stats.CompletedQuests,
                AverageLevel = stats.AverageLevel,
                ExperienceEarned = stats.TotalExperience,
                LastLevelUp = stats.LastLevelUp,
                TotalQuestsCompleted = stats.TotalQuestsCompleted,
                ClassDistribution = stats.ClassDistribution,
                LastUpdate = stats.LastUpdate
            };
        }

        public void Sync()
        {
            // Sync Vystia class data with unified quest system
            var allQuests = UnifiedQuestSystem.GetAllQuests();
            
            lock (s_Lock)
            {
                var playerData = VystiaClassSystem.GetPlayerData(null);
                foreach (var quest in allQuests)
                {
                    if (quest.Type == QuestType.Vystia)
                    {
                        // Update class progress
                        var playerData = VystiaClassSystem.GetPlayerData(quest.Owner);
                        if (playerData.Class == VystiaClassSystem.GetPlayerClass(quest.Owner))
                        {
                            // Update experience
                            playerData.Experience += 50;
                            
                            // Update quest progress
                            HandleClassQuestProgress(quest, classDef);
                        }
                    }
                }
            }
        }

        public void ClearData()
        {
            // Clear Vystia class data
            VystiaClassSystem.ClearAllData();
        }

        /// <summary>
        /// Handle class quest progress
        /// </summary>
        private static void HandleClassQuestProgress(UnifiedQuestData quest, VystiaClassDefinition classDef)
        {
            // Update class experience
            var playerData = VystiaClassSystem.GetPlayerData(quest.Owner);
            if (playerData.Class == VystiaClassSystem.GetPlayerClass(quest.Owner))
            {
                playerData.Experience += 25;
                
                // Update quest progress
                UpdateClassQuestProgress(quest, classDef);
            }
        }

        /// <summary>
        /// Update class quest progress
        /// </summary>
        private static void UpdateClassQuestProgress(UnifiedQuestData quest, VystiaClassDefinition classDef)
        {
            // Update quest objectives based on class type
            switch (classDef.Type.ToLower())
            {
                case "class":
                    // Update skill experience
                    var playerData = VystiaClassSystem.GetPlayerData(quest.Owner);
                    if (playerData.Class == VystiaClassSystem.GetPlayerClass(quest.Owner))
                    {
                        // Update primary skill experience
                        var primarySkill = classDef.PrimarySkill;
                        var currentSkill = quest.Owner.Skills.PrimarySkill.BaseSkill.BaseSkill;
                        var skillValue = (double)currentSkill.Value / 1000.0;
                        
                        if (skillValue > 0)
                        {
                            var gain = (int)(skillValue * 0.1);
                            quest.Owner.Skills.PrimarySkill.Base += gain;
                            player.SendMessage($"Your {primarySkill} increased by {gain} points!");
                        }
                    }
                    
                    // Update secondary skills
                    if (classDef.SecondarySkills != null)
                    {
                        foreach (var skill in classDef.SecondarySkills)
                        {
                            var currentSkill = quest.Owner.Skills.GetSkill(skill);
                            var skillValue = (double)currentSkill.Value / 1000.0;
                            
                            if (skillValue > 0)
                            {
                                var gain = (int)(skillValue * 0.05);
                                quest.Owner.Skills.GetSkill(skill).Base += gain;
                                player.SendMessage($"Your {skill} increased by {gain} points!");
                            }
                        }
                    }
                    break;
            }
        }
    }

    /// <summary>
    /// Vystia Quest Integration
    /// </summary>
    public class VystiaQuestIntegration : IVystiaSystemIntegration
    {
        public string Name => "VystiaQuest";
        public bool IsInitialized() => true;

        public void Initialize()
        {
            // Initialize unified quest system first
            UnifiedQuestSystem.Initialize();
        }

        public List<UnifiedQuestData> GetAvailableQuests(PlayerMobile player)
        {
            // Get all available quests from unified system
            var unifiedQuests = UnifiedQuestSystem.GetAllQuests(player);
            
            // Filter for Vystia quests
            var vystiaQuests = unifiedQuests.Where(q => q.Type == QuestType.Vystia).ToList();

            // Add Vystia-specific quests from integrations
            var integrationQuests = GetAllAvailableQuests(player);
            vystiaQuests.AddRange(integrationQuests);

            return vystiaQuests;
        }

        public VystiaSystemStatistics GetStatistics()
        {
            var stats = UnifiedQuestSystem.GetSystemStatistics();
            var vystiaStats = VystiaQuestSystem.GetStatistics();

            return new VystiaSystemStatistics
            {
                TotalQuestsCreated = s_TotalQuestsCreated,
                TotalQuestsCompleted = s_TotalQuestsCompleted,
                ActiveQuests = vystiaStats.ActiveQuests,
                CompletedQuests = vystiaStats.CompletedQuests,
                AverageQuestsPerPlayer = vystiaStats.AverageQuestsPerPlayer,
                QuestTypeDistribution = new Dictionary<string, int>(),
                LastUpdate = vystiaStats.LastUpdate
            };
        }

        public void Sync()
        {
            // Sync all Vystia quest data with unified quest system
            var allQuests = UnifiedQuestSystem.GetAllQuests();
            
            lock (s_Lock)
            {
                var vystiaQuests = allQuests.Where(q => q.Type == QuestType.Vystia).ToList();
                
                foreach (var quest in vystiaQuests)
                {
                    // Update unified quest data with Vystia context
                    quest.Metadata["vystia_quest_sync"] = true;
                    
                    // Update unified quest progress
                    UnifiedProgressTracker.TrackProgress(quest, new ProgressUpdate
                    {
                        ProgressType = "vystia_quest_sync",
                        Amount = 1,
                        Description = $"Vystia quest sync: {quest.Title}",
                        Source = quest.Owner,
                        QuestId = quest.QuestId
                    });
                }
            }
        }

        public void ClearData()
        {
            // Clear all Vystia quest data
            lock (s_Lock)
            {
                // Clear unified quest data
                var allQuests = UnifiedQuestSystem.GetAllQuests();
                
                foreach (var quest in allQuests.Where(q => q.Type == QuestType.Vystia))
                {
                    UnifiedQuestSystem.DeleteQuest(quest.QuestId);
                }
            }
        }
    }
}
