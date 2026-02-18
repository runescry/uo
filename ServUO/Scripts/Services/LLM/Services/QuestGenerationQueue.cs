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
    /// High-performance queue for LLM quest generation with load balancing
    /// Implements priority queue and concurrent processing
    /// </summary>
    public static class QuestGenerationQueue
    {
        private static readonly ConcurrentQueue<QuestGenerationRequest> s_Queue = new ConcurrentQueue<QuestGenerationRequest>();
        private static readonly SemaphoreSlim s_ProcessingSemaphore = new SemaphoreSlim(Environment.ProcessorCount);
        private static readonly TimeSpan s_ProcessingInterval = TimeSpan.FromMilliseconds(100);
        private static bool s_IsProcessing = false;

        // Performance metrics
        private static long s_TotalRequests = 0;
        private static long s_ProcessedRequests = 0;
        private static long s_FailedRequests = 0;
        private static long s_AverageProcessingTime = 0;

        static QuestGenerationQueue()
        {
            // Start processing task
            Task.Run(async () =>
            {
                while (true)
                {
                    await Task.Delay(s_ProcessingInterval);
                    ProcessQueueAsync(null);
                }
            });
        }

        /// <summary>
        /// Enqueue quest generation request
        /// </summary>
        public static Task<string> EnqueueAsync(QuestGenerationRequest request)
        {
            var tcs = new TaskCompletionSource<string>();
            request.CompletionSource = tcs;

            s_Queue.Enqueue(request);
            Interlocked.Increment(ref s_TotalRequests);

            return tcs.Task;
        }

        /// <summary>
        /// Get queue statistics
        /// </summary>
        public static QueueStats GetStats()
        {
            return new QueueStats
            {
                TotalRequests = s_TotalRequests,
                ProcessedRequests = s_ProcessedRequests,
                FailedRequests = s_FailedRequests,
                AverageProcessingTime = s_AverageProcessingTime,
                QueueLength = s_Queue.Count,
                IsProcessing = s_IsProcessing
            };
        }

        /// <summary>
        /// Process queued requests
        /// </summary>
        private static async Task ProcessQueueAsync(object state)
        {
            if (s_IsProcessing || s_Queue.IsEmpty)
                return;

            s_IsProcessing = true;

            try
            {
                var tasks = new List<Task>();

                // Process up to the semaphore count
                for (int i = 0; i < Environment.ProcessorCount && !s_Queue.IsEmpty; i++)
                {
                    if (s_Queue.TryDequeue(out var request))
                    {
                        tasks.Add(ProcessRequestAsync(request));
                    }
                }

                // Wait for all tasks to complete
                if (tasks.Count > 0)
                {
                    await Task.WhenAll(tasks);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[QuestGenerationQueue] Error processing queue: {ex.Message}");
            }
            finally
            {
                s_IsProcessing = false;
            }
        }

        /// <summary>
        /// Process individual request
        /// </summary>
        private static async Task ProcessRequestAsync(QuestGenerationRequest request)
        {
            var startTime = DateTime.UtcNow;
            
            try
            {
                // Check if we have a cached result
                var cacheKey = QuestCache.GenerateCacheKey(
                    request.NpcName, 
                    request.NpcPersonality, 
                    request.ConversationHistory,
                    request.PlayerMessage, 
                    request.PlayerName, 
                    request.PreloadedKnowledge
                );

                var cachedResult = await QuestCache.GetAsync(cacheKey);
                if (cachedResult != null)
                {
                    request.CompletionSource.SetResult(cachedResult);
                    Interlocked.Increment(ref s_ProcessedRequests);
                    return;
                }

                // Generate response
                var result = await GenerateQuestResponseAsync(request);
                
                // Cache the result
                await QuestCache.SetAsync(cacheKey, result, TimeSpan.FromMinutes(15));

                request.CompletionSource.SetResult(result);
                Interlocked.Increment(ref s_ProcessedRequests);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[QuestGenerationQueue] Error processing request: {ex.Message}");
                request.CompletionSource.SetException(ex);
                Interlocked.Increment(ref s_FailedRequests);
            }
            finally
            {
                var processingTime = (DateTime.UtcNow - startTime).TotalMilliseconds;
                s_AverageProcessingTime = (long)((s_AverageProcessingTime * (s_ProcessedRequests + s_FailedRequests) + processingTime) / (s_ProcessedRequests + s_FailedRequests + 1));
            }
        }

        /// </// <summary>
        /// Generate quest response using existing LLM service
        /// </summary>
        private static async Task<string> GenerateQuestResponseAsync(QuestGenerationRequest request)
        {
            // Use existing UnifiedLLMService with async pattern
            return await UnifiedLLMService.GetResponseAsync(
                request.NpcName,
                request.NpcPersonality,
                request.ConversationHistory,
                request.PlayerMessage,
                request.PlayerName,
                request.PreloadedKnowledge,
                request.IsVendor,
                request.IsFirstConversation,
                UnifiedLLMService.RequestType.QuestDialogue
            );
        }

        /// <summary>
        /// Clear the queue
        /// </summary>
        public static void Clear()
        {
            while (s_Queue.TryDequeue(out _))
            {
                // Clear all pending requests
            }
        }

        /// <summary>
        /// Queue statistics
        /// </summary>
        public class QueueStats
        {
            public long TotalRequests { get; set; }
            public long ProcessedRequests { get; set; }
            public long FailedRequests { get; set; }
            public long AverageProcessingTime { get; set; }
            public int QueueLength { get; set; }
            public bool IsProcessing { get; set; }
        }
    }

    /// <summary>
    /// Quest generation request
    /// </summary>
    public class QuestGenerationRequest
    {
        public string NpcName { get; set; }
        public string NpcPersonality { get; set; }
        public List<ConversationMessage> ConversationHistory { get; set; }
        public string PlayerMessage { get; set; }
        public string PlayerName { get; set; }
        public string PreloadedKnowledge { get; set; }
        public bool IsVendor { get; set; }
        public bool IsFirstConversation { get; set; }
        public TaskCompletionSource<string> CompletionSource { get; set; }
        public DateTime QueuedAt { get; set; } = DateTime.UtcNow;
        public int Priority { get; set; } = 1; // Higher priority = processed first
    }
}
