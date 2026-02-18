using System;
using System.Collections.Generic;
using System.Linq;
using Server.Mobiles;
using Server.Services.UnifiedQuestSystem.Integrations;

namespace Server.Services.UnifiedQuestSystem.Integrations
{
    /// <summary>
    /// Integration manager for all UO system integrations
    /// Coordinates and manages all quest system integrations
    /// </summary>
    public static class UOSystemIntegrationManager
    {
        private static readonly Dictionary<string, IUOSystemIntegration> s_Integrations;
        private static readonly object s_Lock = new object();
        private static bool s_Initialized = false;
        private static DateTime s_LastSync = DateTime.UtcNow;

        static UOSystemIntegrationManager()
        {
            s_Integrations = new Dictionary<string, IUOSystemIntegration>();
        }

        /// <summary>
        /// Initialize all UO system integrations
        /// </summary>
        public static void Initialize()
        {
            if (s_Initialized)
                return;

            s_Initialized = true;

            // Register all integrations
            RegisterIntegration("BulkOrder", new BulkOrderSystemIntegration());
            RegisterIntegration("Champion", new ChampionSystemIntegration());
            RegisterIntegration("CityLoyalty", new CityLoyaltySystemIntegration());
            RegisterIntegration("SeasonalEvents", new SeasonalEventSystemIntegration());

            // Initialize all integrations
            foreach (var integration in s_Integrations.Values)
            {
                integration.Initialize();
            }

            Console.WriteLine("[UOSystemIntegrationManager] Initialized {0} UO system integrations", s_Integrations.Count);
        }

        /// <summary>
        /// Get integration statistics for all systems
        /// </summary>
        public static UOSystemIntegrationStatistics GetAllStatistics()
        {
            lock (s_Lock)
            {
                var stats = new UOSystemIntegrationStatistics
                {
                    TotalIntegrations = s_Integrations.Count,
                    IntegrationStatistics = new Dictionary<string, object>(),
                    LastSync = s_LastSync
                };

                foreach (var integration in s_Integrations)
                {
                    stats.IntegrationStatistics[integration.Key] = integration.GetStatistics();
                }

                return stats;
            }
        }

        /// <summary>
        /// Get available quests from all integrations
        /// </summary>
        public static List<UnifiedQuestData> GetAllAvailableQuests(PlayerMobile player)
        {
            var allQuests = new List<UnifiedQuestData>();

            lock (s_Lock)
            {
                foreach (var integration in s_Integrations.Values)
                {
                    try
                    {
                        var quests = integration.GetAvailableQuests(player);
                        allQuests.AddRange(quests);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"[UOSystemIntegrationManager] Error getting quests from {integration.GetType().Name}: {ex.Message}");
                    }
                }
            }

            return allQuests;
        }

        /// <summary>
        /// Sync all integrations
        /// </summary>
        public static void SyncAllIntegrations()
        {
            lock (s_Lock)
            {
                foreach (var integration in s_Integrations.Values)
                {
                    try
                    {
                        integration.Sync();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"[UOSystemIntegrationManager] Error syncing {integration.GetType().Name}: {ex.Message}");
                    }
                }

                s_LastSync = DateTime.UtcNow;
                Console.WriteLine("[UOSystemIntegrationManager] Synced all integrations");
            }
        }

        /// <summary>
        /// Get integration by name
        /// </summary>
        public static IUOSystemIntegration GetIntegration(string name)
        {
            lock (s_Lock)
            {
                return s_Integrations.GetValueOrDefault(name);
            }
        }

        /// <summary>
        /// Register an integration
        /// </summary>
        private static void RegisterIntegration(string name, IUOSystemIntegration integration)
        {
            lock (s_Lock)
            {
                s_Integrations[name] = integration;
                Console.WriteLine($"[UOSystemIntegrationManager] Registered integration: {name}");
            }
        }

        /// <summary>
        /// Reset all integration statistics
        /// </summary>
        public static void ResetAllStatistics()
        {
            lock (s_Lock)
            {
                foreach (var integration in s_Integrations.Values)
                {
                    try
                    {
                        integration.ResetStatistics();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"[UOSystemIntegrationManager] Error resetting statistics for {integration.GetType().Name}: {ex.Message}");
                    }
                }

                Console.WriteLine("[UOSystemIntegrationManager] Reset all integration statistics");
            }
        }

        /// <summary>
        /// Clear completed links from all integrations
        /// </summary>
        public static void ClearAllCompletedLinks()
        {
            lock (s_Lock)
            {
                foreach (var integration in s_Integrations.Values)
                {
                    try
                    {
                        integration.ClearCompletedLinks();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"[UOSystemIntegrationManager] Error clearing links for {integration.GetType().Name}: {ex.Message}");
                    }
                }

                Console.WriteLine("[UOSystemIntegrationManager] Cleared all completed links");
            }
        }
    }

    /// <summary>
    /// Interface for UO system integrations
    /// </summary>
    public interface IUOSystemIntegration
    {
        void Initialize();
        List<UnifiedQuestData> GetAvailableQuests(PlayerMobile player);
        object GetStatistics();
        void Sync();
        void ResetStatistics();
        void ClearCompletedLinks();
    }

    /// <summary>
    /// Bulk Order System integration
    /// </summary>
    public class BulkOrderSystemIntegration : IUOSystemIntegration
    {
        public void Initialize()
        {
            BulkOrderQuestIntegration.Initialize();
        }

        public List<UnifiedQuestData> GetAvailableQuests(PlayerMobile player)
        {
            return BulkOrderQuestIntegration.GenerateCraftingQuests(player);
        }

        public object GetStatistics()
        {
            return BulkOrderQuestIntegration.GetStatistics();
        }

        public void Sync()
        {
            BulkOrderQuestIntegration.ClearCompletedLinks();
        }

        public void ResetStatistics()
        {
            BulkOrderQuestIntegration.ResetStatistics();
        }

        public void ClearCompletedLinks()
        {
            BulkOrderQuestIntegration.ClearCompletedLinks();
        }
    }

    /// <summary>
    /// Champion System integration
    /// </summary>
    public class ChampionSystemIntegration : IUOSystemIntegration
    {
        public void Initialize()
        {
            ChampionQuestIntegration.Initialize();
        }

        public List<UnifiedQuestData> GetAvailableQuests(PlayerMobile player)
        {
            return ChampionQuestIntegration.GetAvailableQuests(player);
        }

        public object GetStatistics()
        {
            return ChampionQuestIntegration.GetStatistics();
        }

        public void Sync()
        {
            ChampionQuestIntegration.ClearCompletedLinks();
        }

        public void ResetStatistics()
        {
            ChampionQuestIntegration.ResetStatistics();
        }

        public void ClearCompletedLinks()
        {
            ChampionQuestIntegration.ClearCompletedLinks();
        }
    }

    /// <summary>
    /// City Loyalty System integration
    /// </summary>
    public class CityLoyaltySystemIntegration : IUOSystemIntegration
    {
        public void Initialize()
        {
            CityLoyaltyQuestIntegration.Initialize();
        }

        public List<UnifiedQuestData> GetAvailableQuests(PlayerMobile player)
        {
            return CityLoyaltyQuestIntegration.GetAvailableQuests(player);
        }

        public object GetStatistics()
        {
            return CityLoyaltyQuestIntegration.GetStatistics();
        }

        public void Sync()
        {
            CityLoyaltyQuestIntegration.ClearCompletedLinks();
        }

        public void ResetStatistics()
        {
            CityLoyaltyQuestIntegration.ResetStatistics();
        }

        public void ClearCompletedLinks()
        {
            CityLoyaltyQuestIntegration.ClearCompletedLinks();
        }
    }

    /// <summary>
    /// Seasonal Events System integration
    /// </summary>
    public class SeasonalEventSystemIntegration : IUOSystemIntegration
    {
        public void Initialize()
        {
            SeasonalEventQuestIntegration.Initialize();
        }

        public List<UnifiedQuestData> GetAvailableQuests(PlayerMobile player)
        {
            return SeasonalEventQuestIntegration.GetAvailableQuests(player);
        }

        public object GetStatistics()
        {
            return SeasonalEventQuestIntegration.GetStatistics();
        }

        public void Sync()
        {
            SeasonalEventQuestIntegration.ClearCompletedLinks();
        }

        public void ResetStatistics()
        {
            SeasonalEventQuestIntegration.ResetStatistics();
        }

        public void ClearCompletedLinks()
        {
            SeasonalEventQuestIntegration.ClearCompletedLinks();
        }
    }

    /// <summary>
    /// UO System Integration Statistics
    /// </summary>
    public class UOSystemIntegrationStatistics
    {
        public int TotalIntegrations { get; set; }
        public Dictionary<string, object> IntegrationStatistics { get; set; }
        public DateTime LastSync { get; set; }
    }
}
