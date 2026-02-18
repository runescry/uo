using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Server;
using Server.Mobiles;

namespace Server.Services.LLM
{
    /// <summary>
    /// Performance monitoring system for LLM quest generation
    /// Tracks metrics and provides performance insights
    /// </summary>
    public static class QuestPerformanceMonitor
    {
        private static readonly ConcurrentDictionary<string, PerformanceMetrics> s_Metrics = new ConcurrentDictionary<string, PerformanceMetrics>();
        private static readonly TimeSpan s_ReportingInterval = TimeSpan.FromMinutes(5);
        private static readonly TimeSpan s_WarningThreshold = TimeSpan.FromSeconds(2);
        private static readonly TimeSpan s_CriticalThreshold = TimeSpan.FromSeconds(5);
        private static readonly object s_Lock = new object();

        // Global metrics
        private static long s_TotalRequests = 0;
        private static long s_TotalResponseTime = 0;
        private static long s_FailedRequests = 0;
        private static DateTime s_LastReport = DateTime.UtcNow;

        static QuestPerformanceMonitor()
        {
            // Start reporting task
            Task.Run(async () =>
            {
                while (true)
                {
                    await Task.Delay(s_ReportingInterval);
                    GeneratePerformanceReport(null);
                }
            });
        }

        /// <summary>
        /// Record quest generation performance
        /// </summary>
        public static void RecordPerformance(string npcName, TimeSpan responseTime, bool success, bool fromCache = false)
        {
            var key = npcName ?? "Unknown";
            
            var metrics = s_Metrics.GetOrAdd(key, _ => new PerformanceMetrics());
            
            lock (metrics.Lock)
            {
                metrics.TotalRequests++;
                metrics.TotalResponseTime += (long)responseTime.TotalMilliseconds;
                metrics.LastResponseTime = responseTime;
                metrics.LastRequestTime = DateTime.UtcNow;
                
                if (success)
                {
                    metrics.SuccessfulRequests++;
                    if (fromCache)
                    {
                        metrics.CacheHits++;
                    }
                }
                else
                {
                    metrics.FailedRequests++;
                }
                
                // Update min/max response times
                if (responseTime < metrics.MinResponseTime || metrics.MinResponseTime == TimeSpan.Zero)
                {
                    metrics.MinResponseTime = responseTime;
                }
                
                if (responseTime > metrics.MaxResponseTime)
                {
                    metrics.MaxResponseTime = responseTime;
                }
            }

            // Update global metrics
            Interlocked.Increment(ref s_TotalRequests);
            Interlocked.Add(ref s_TotalResponseTime, (long)responseTime.TotalMilliseconds);
            
            if (!success)
            {
                Interlocked.Increment(ref s_FailedRequests);
            }

            // Log warnings for slow responses
            if (responseTime > s_CriticalThreshold)
            {
                Console.WriteLine($"[QuestPerformanceMonitor] CRITICAL: Slow quest generation for {npcName}: {responseTime.TotalSeconds:F2}s");
            }
            else if (responseTime > s_WarningThreshold)
            {
                Console.WriteLine($"[QuestPerformanceMonitor] WARNING: Slow quest generation for {npcName}: {responseTime.TotalSeconds:F2}s");
            }
        }

        /// <summary>
        /// Get performance metrics for an NPC
        /// </summary>
        public static PerformanceMetrics GetMetrics(string npcName)
        {
            return s_Metrics.ContainsKey(npcName) ? s_Metrics[npcName] : new PerformanceMetrics();
        }

        /// <summary>
        /// Get all performance metrics
        /// </summary>
        public static Dictionary<string, PerformanceMetrics> GetAllMetrics()
        {
            return new Dictionary<string, PerformanceMetrics>(s_Metrics);
        }

        /// <summary>
        /// Get global performance statistics
        /// </summary>
        public static GlobalStats GetGlobalStats()
        {
            var cacheStats = QuestCache.GetStats();
            var queueStats = QuestGenerationQueue.GetStats();
            var memoryStats = QuestMemoryManager.GetStats();

            return new GlobalStats
            {
                TotalRequests = s_TotalRequests,
                FailedRequests = s_FailedRequests,
                AverageResponseTime = s_TotalRequests > 0 ? TimeSpan.FromMilliseconds(s_TotalResponseTime / s_TotalRequests) : TimeSpan.Zero,
                SuccessRate = s_TotalRequests > 0 ? (double)(s_TotalRequests - s_FailedRequests) / s_TotalRequests : 0,
                CacheHitRatio = cacheStats.HitRatio,
                QueueLength = queueStats.QueueLength,
                AverageQueueProcessingTime = TimeSpan.FromMilliseconds(queueStats.AverageProcessingTime),
                MemoryUsage = memoryStats.TotalInstances,
                LastReport = s_LastReport
            };
        }

        /// <summary>
        /// Reset all metrics
        /// </summary>
        public static void ResetMetrics()
        {
            lock (s_Lock)
            {
                s_Metrics.Clear();
                s_TotalRequests = 0;
                s_TotalResponseTime = 0;
                s_FailedRequests = 0;
                s_LastReport = DateTime.UtcNow;
            }
        }

        /// <summary>
        /// Generate performance report
        /// </summary>
        private static void GeneratePerformanceReport(object state)
        {
            try
            {
                var globalStats = GetGlobalStats();
                var allMetrics = GetAllMetrics();

                Console.WriteLine("=== Quest Performance Report ===");
                Console.WriteLine($"Total Requests: {globalStats.TotalRequests}");
                Console.WriteLine($"Success Rate: {globalStats.SuccessRate:P2}");
                Console.WriteLine($"Average Response Time: {globalStats.AverageResponseTime.TotalSeconds:F2}s");
                Console.WriteLine($"Cache Hit Ratio: {globalStats.CacheHitRatio:P2}");
                Console.WriteLine($"Queue Length: {globalStats.QueueLength}");
                Console.WriteLine($"Memory Usage: {globalStats.MemoryUsage} instances");
                Console.WriteLine($"Failed Requests: {globalStats.FailedRequests}");

                // Show top 10 NPCs by request count
                var topNpcs = allMetrics.OrderByDescending(kvp => kvp.Value.TotalRequests).Take(10);
                Console.WriteLine("\nTop NPCs by Request Count:");
                foreach (var kvp in topNpcs)
                {
                    var metrics = kvp.Value;
                    var avgTime = metrics.TotalRequests > 0 ? TimeSpan.FromMilliseconds(metrics.TotalResponseTime / metrics.TotalRequests) : TimeSpan.Zero;
                    var successRate = metrics.TotalRequests > 0 ? (double)metrics.SuccessfulRequests / metrics.TotalRequests : 0;
                    
                    Console.WriteLine($"  {kvp.Key}: {metrics.TotalRequests} requests, {avgTime.TotalSeconds:F2}s avg, {successRate:P2} success");
                }

                // Show slow NPCs
                var slowNpcs = allMetrics.Where(kvp => kvp.Value.AverageResponseTime > s_WarningThreshold).OrderByDescending(kvp => kvp.Value.AverageResponseTime).Take(5);
                if (slowNpcs.Any())
                {
                    Console.WriteLine("\nSlow NPCs (avg > 2s):");
                    foreach (var kvp in slowNpcs)
                    {
                        var metrics = kvp.Value;
                        Console.WriteLine($"  {kvp.Key}: {metrics.AverageResponseTime.TotalSeconds:F2}s avg");
                    }
                }

                Console.WriteLine("=== End Report ===");
                s_LastReport = DateTime.UtcNow;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[QuestPerformanceMonitor] Error generating report: {ex.Message}");
            }
        }

        /// <summary>
        /// Performance metrics for an NPC
        /// </summary>
        public class PerformanceMetrics
        {
            public int TotalRequests { get; set; }
            public int SuccessfulRequests { get; set; }
            public int FailedRequests { get; set; }
            public int CacheHits { get; set; }
            public long TotalResponseTime { get; set; } // in milliseconds
            public TimeSpan MinResponseTime { get; set; }
            public TimeSpan MaxResponseTime { get; set; }
            public TimeSpan LastResponseTime { get; set; }
            public DateTime LastRequestTime { get; set; }
            public object Lock { get; } = new object();

            public TimeSpan AverageResponseTime => TotalRequests > 0 ? TimeSpan.FromMilliseconds((double)TotalResponseTime / TotalRequests) : TimeSpan.Zero;
            public double SuccessRate => TotalRequests > 0 ? (double)SuccessfulRequests / TotalRequests : 0;
            public double CacheHitRatio => TotalRequests > 0 ? (double)CacheHits / TotalRequests : 0;
        }

        /// <summary>
        /// Global performance statistics
        /// </summary>
        public class GlobalStats
        {
            public long TotalRequests { get; set; }
            public long FailedRequests { get; set; }
            public TimeSpan AverageResponseTime { get; set; }
            public double SuccessRate { get; set; }
            public double CacheHitRatio { get; set; }
            public int QueueLength { get; set; }
            public TimeSpan AverageQueueProcessingTime { get; set; }
            public int MemoryUsage { get; set; }
            public DateTime LastReport { get; set; }
        }
    }
}
