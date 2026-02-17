using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Server.Services.LLM
{
    /// <summary>
    /// Handles errors, retries, and circuit breaking for LLM services
    /// </summary>
    public static class LLMErrorHandler
    {
        // Circuit breaker state
        private static bool m_CircuitOpen = false;
        private static DateTime m_CircuitOpenTime = DateTime.MinValue;
        private static int m_ConsecutiveFailures = 0;
        private static readonly int MaxConsecutiveFailures = 5;
        private static readonly TimeSpan CircuitBreakerTimeout = TimeSpan.FromMinutes(2);
        private static readonly object m_CircuitLock = new object();

        // Retry configuration
        public static readonly int MaxRetries = 3;
        private static readonly int BaseRetryDelayMs = 500; // Start with 500ms

        /// <summary>
        /// Executes an LLM operation with retry logic and circuit breaker
        /// </summary>
        public static async Task<T> ExecuteWithRetryAsync<T>(
            Func<Task<T>> operation,
            Func<T> fallback,
            string operationName = "LLM operation")
        {
            // Check circuit breaker
            if (IsCircuitOpen())
            {
                Console.WriteLine($"[LLMErrorHandler] Circuit breaker OPEN, using fallback for {operationName}");
                return fallback();
            }

            Exception lastException = null;

            // Retry loop
            for (int attempt = 1; attempt <= MaxRetries; attempt++)
            {
                try
                {
                    T result = await operation();
                    
                    // Success - reset failure counter
                    lock (m_CircuitLock)
                    {
                        m_ConsecutiveFailures = 0;
                        if (m_CircuitOpen)
                        {
                            Console.WriteLine($"[LLMErrorHandler] Circuit breaker CLOSED after successful operation");
                            m_CircuitOpen = false;
                        }
                    }

                    return result;
                }
                catch (TaskCanceledException ex)
                {
                    lastException = ex;
                    Console.WriteLine($"[LLMErrorHandler] Timeout on attempt {attempt}/{MaxRetries} for {operationName}");
                    
                    if (attempt < MaxRetries)
                    {
                        int delayMs = BaseRetryDelayMs * (int)Math.Pow(2, attempt - 1); // Exponential backoff
                        Console.WriteLine($"[LLMErrorHandler] Retrying in {delayMs}ms...");
                        await Task.Delay(delayMs);
                    }
                }
                catch (HttpRequestException ex)
                {
                    lastException = ex;
                    Console.WriteLine($"[LLMErrorHandler] HTTP error on attempt {attempt}/{MaxRetries} for {operationName}: {ex.Message}");
                    
                    if (attempt < MaxRetries)
                    {
                        int delayMs = BaseRetryDelayMs * (int)Math.Pow(2, attempt - 1);
                        Console.WriteLine($"[LLMErrorHandler] Retrying in {delayMs}ms...");
                        await Task.Delay(delayMs);
                    }
                }
                catch (Exception ex)
                {
                    lastException = ex;
                    Console.WriteLine($"[LLMErrorHandler] Error on attempt {attempt}/{MaxRetries} for {operationName}: {ex.Message}");
                    
                    // Don't retry on non-transient errors (like invalid API key)
                    if (ex.Message.Contains("401") || ex.Message.Contains("Unauthorized") || ex.Message.Contains("403"))
                    {
                        Console.WriteLine($"[LLMErrorHandler] Non-retryable error detected, using fallback");
                        break;
                    }
                    
                    if (attempt < MaxRetries)
                    {
                        int delayMs = BaseRetryDelayMs * (int)Math.Pow(2, attempt - 1);
                        Console.WriteLine($"[LLMErrorHandler] Retrying in {delayMs}ms...");
                        await Task.Delay(delayMs);
                    }
                }
            }

            // All retries failed - update circuit breaker
            lock (m_CircuitLock)
            {
                m_ConsecutiveFailures++;
                Console.WriteLine($"[LLMErrorHandler] Operation failed after {MaxRetries} attempts. Consecutive failures: {m_ConsecutiveFailures}/{MaxConsecutiveFailures}");
                
                if (m_ConsecutiveFailures >= MaxConsecutiveFailures)
                {
                    m_CircuitOpen = true;
                    m_CircuitOpenTime = DateTime.UtcNow;
                    Console.WriteLine($"[LLMErrorHandler] Circuit breaker OPENED after {m_ConsecutiveFailures} consecutive failures");
                }
            }

            // Use fallback
            Console.WriteLine($"[LLMErrorHandler] Using fallback response for {operationName}");
            return fallback();
        }

        /// <summary>
        /// Checks if circuit breaker is open
        /// </summary>
        private static bool IsCircuitOpen()
        {
            lock (m_CircuitLock)
            {
                if (!m_CircuitOpen)
                    return false;

                // Check if timeout has passed
                if (DateTime.UtcNow - m_CircuitOpenTime > CircuitBreakerTimeout)
                {
                    Console.WriteLine($"[LLMErrorHandler] Circuit breaker timeout expired, attempting to close");
                    m_CircuitOpen = false;
                    m_ConsecutiveFailures = 0;
                    return false;
                }

                return true;
            }
        }

        /// <summary>
        /// Resets the circuit breaker (manual override)
        /// </summary>
        public static void ResetCircuitBreaker()
        {
            lock (m_CircuitLock)
            {
                m_CircuitOpen = false;
                m_ConsecutiveFailures = 0;
                Console.WriteLine("[LLMErrorHandler] Circuit breaker manually reset");
            }
        }

        /// <summary>
        /// Gets circuit breaker status
        /// </summary>
        public static string GetCircuitBreakerStatus()
        {
            lock (m_CircuitLock)
            {
                if (m_CircuitOpen)
                {
                    TimeSpan timeRemaining = CircuitBreakerTimeout - (DateTime.UtcNow - m_CircuitOpenTime);
                    return $"OPEN (closes in {timeRemaining.TotalSeconds:F0}s, failures: {m_ConsecutiveFailures})";
                }
                return $"CLOSED (failures: {m_ConsecutiveFailures}/{MaxConsecutiveFailures})";
            }
        }
    }
}

