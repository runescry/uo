using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Server;
using Server.Mobiles;
using Server.Custom.VystiaClasses.Quests;
using Server.Custom.VystiaClasses.Quests.Generation;

namespace Server.Services.LLM
{
    /// <summary>
    /// Memory management system for dynamic quest instances
    /// Provides cleanup and monitoring for quest memory usage
    /// </summary>
    public static class QuestMemoryManager
    {
        private static readonly TimeSpan s_InstanceTTL = TimeSpan.FromHours(24); // 24 hours TTL for quest instances
        private static readonly int s_MaxInstancesPerPlayer = 10;
        private static readonly int s_MaxTotalInstances = 1000;

        // Performance metrics
        private static int s_TotalInstances = 0;
        private static int s_CleanedInstances = 0;
        private static readonly Dictionary<int, int> s_PlayerInstanceCounts = new Dictionary<int, int>();

        static QuestMemoryManager()
        {
            // Start cleanup task
            Task.Run(async () =>
            {
                while (true)
                {
                    await Task.Delay(TimeSpan.FromMinutes(10));
                    CleanupExpiredInstances(null);
                }
            });
            
            Console.WriteLine($"[QuestMemoryManager] Initialized with cleanup interval: {TimeSpan.FromMinutes(10)}");
        }

        /// <summary>
        /// Register a quest instance for tracking
        /// </summary>
        public static void RegisterInstance(PlayerMobile player, int questId)
        {
            if (player == null || player.Deleted)
                return;

            lock (s_PlayerInstanceCounts)
            {
                s_PlayerInstanceCounts[player.Serial] = s_PlayerInstanceCounts.ContainsKey(player.Serial) ? s_PlayerInstanceCounts[player.Serial] + 1 : 1;
                s_TotalInstances++;
            }
        }

        /// <summary>
        /// Unregister a quest instance
        /// </summary>
        public static void UnregisterInstance(PlayerMobile player, int questId)
        {
            if (player == null || player.Deleted)
                return;

            lock (s_PlayerInstanceCounts)
            {
                if (s_PlayerInstanceCounts.ContainsKey(player.Serial))
                {
                    s_PlayerInstanceCounts[player.Serial]--;
                    if (s_PlayerInstanceCounts[player.Serial] <= 0)
                    {
                        s_PlayerInstanceCounts.Remove(player.Serial);
                    }
                    s_TotalInstances--;
                }
            }
        }

        /// <summary>
        /// Check if player can create more instances
        /// </summary>
        public static bool CanCreateInstance(PlayerMobile player)
        {
            if (player == null || player.Deleted)
                return false;

            lock (s_PlayerInstanceCounts)
            {
                var playerCount = s_PlayerInstanceCounts.ContainsKey(player.Serial) ? s_PlayerInstanceCounts[player.Serial] : 0;
                return playerCount < s_MaxInstancesPerPlayer && s_TotalInstances < s_MaxTotalInstances;
            }
        }

        /// <summary>
        /// Get player instance count
        /// </summary>
        public static int GetPlayerInstanceCount(PlayerMobile player)
        {
            if (player == null || player.Deleted)
                return 0;

            lock (s_PlayerInstanceCounts)
            {
                return s_PlayerInstanceCounts.ContainsKey(player.Serial) ? s_PlayerInstanceCounts[player.Serial] : 0;
            }
        }

        /// <summary>
        /// Get total instance count
        /// </summary>
        public static int GetTotalInstanceCount()
        {
            return s_TotalInstances;
        }

        /// <summary>
        /// Get memory statistics
        /// </summary>
        public static MemoryStats GetStats()
        {
            lock (s_PlayerInstanceCounts)
            {
                return new MemoryStats
                {
                    TotalInstances = s_TotalInstances,
                    CleanedInstances = s_CleanedInstances,
                    ActivePlayers = s_PlayerInstanceCounts.Count,
                    AverageInstancesPerPlayer = s_PlayerInstanceCounts.Count > 0 ? (double)s_TotalInstances / s_PlayerInstanceCounts.Count : 0,
                    MaxInstancesPerPlayer = s_MaxInstancesPerPlayer,
                    MaxTotalInstances = s_MaxTotalInstances
                };
            }
        }

        /// <summary>
        /// Force cleanup of expired instances
        /// </summary>
        public static void ForceCleanup()
        {
            CleanupExpiredInstances(null);
        }

        /// <summary>
        /// Cleanup expired quest instances
        /// </summary>
        private static void CleanupExpiredInstances(object state)
        {
            try
            {
                var cleanedCount = 0;
                var now = DateTime.UtcNow;

                // Check all players for expired instances
                foreach (var player in World.Mobiles.Values.OfType<PlayerMobile>())
                {
                    var attachment = GeneratedQuestInstanceAttachment.Get(player);
                    if (attachment != null)
                    {
                        var instances = attachment.Instances.ToList();
                        foreach (var instance in instances)
                        {
                            // Check if instance is expired (24 hours TTL)
                            if (now - instance.ExpiresAtUtc > s_InstanceTTL)
                            {
                                // Remove expired instance
                                attachment.RemoveInstancesForQuest(instance.QuestId);
                                cleanedCount++;
                            }
                        }
                    }
                }

                if (cleanedCount > 0)
                {
                    s_CleanedInstances += cleanedCount;
                    Console.WriteLine($"[QuestMemoryManager] Cleaned {cleanedCount} expired quest instances");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[QuestMemoryManager] Error during cleanup: {ex.Message}");
            }
        }

        /// <summary>
        /// Monitor memory usage and log warnings
        /// </summary>
        public static void MonitorMemoryUsage()
        {
            var stats = GetStats();
            
            // Log warnings if approaching limits
            if (stats.TotalInstances > stats.MaxTotalInstances * 0.8)
            {
                Console.WriteLine($"[QuestMemoryManager] WARNING: Quest instance count ({stats.TotalInstances}) approaching limit ({stats.MaxTotalInstances})");
            }

            if (stats.AverageInstancesPerPlayer > stats.MaxInstancesPerPlayer * 0.8)
            {
                Console.WriteLine($"[QuestMemoryManager] WARNING: Average instances per player ({stats.AverageInstancesPerPlayer:F2}) approaching limit ({stats.MaxInstancesPerPlayer})");
            }

            // Log periodic statistics
            Console.WriteLine($"[QuestMemoryManager] Memory Stats - Total: {stats.TotalInstances}, Cleaned: {stats.CleanedInstances}, Active Players: {stats.ActivePlayers}, Avg/Player: {stats.AverageInstancesPerPlayer:F2}");
        }

        /// <summary>
        /// Memory statistics
        /// </summary>
        public class MemoryStats
        {
            public int TotalInstances { get; set; }
            public int CleanedInstances { get; set; }
            public int ActivePlayers { get; set; }
            public double AverageInstancesPerPlayer { get; set; }
            public int MaxInstancesPerPlayer { get; set; }
            public int MaxTotalInstances { get; set; }
        }
    }
}
