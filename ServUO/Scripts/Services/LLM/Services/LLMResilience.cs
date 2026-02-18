using System;
using System.Threading;
using System.Threading.Tasks;
using Server;

namespace Server.Services.LLM
{
    /// <summary>
    /// Circuit breaker pattern for LLM API calls to prevent cascading failures
    /// </summary>
    public static class CircuitBreaker
    {
        private static int s_FailureCount = 0;
        private static DateTime s_LastFailureTime = DateTime.MinValue;
        private static readonly TimeSpan s_ResetTimeout = TimeSpan.FromMinutes(5);
        private static readonly int s_FailureThreshold = 3;
        private static CircuitState s_State = CircuitState.Closed;

        public enum CircuitState
        {
            Closed,    // Normal operation
            Open,      // Failing, reject calls
            HalfOpen   // Testing if service recovered
        }

        /// <summary>
        /// Execute an action with circuit breaker protection
        /// </summary>
        public static async Task<T> ExecuteAsync<T>(Func<Task<T>> action, Func<T> fallback)
        {
            if (s_State == CircuitState.Open)
            {
                if (DateTime.UtcNow - s_LastFailureTime > s_ResetTimeout)
                {
                    s_State = CircuitState.HalfOpen;
                    Console.WriteLine("[CircuitBreaker] Transitioning to HalfOpen state");
                }
                else
                {
                    Console.WriteLine("[CircuitBreaker] Circuit is Open, using fallback");
                    return fallback();
                }
            }

            try
            {
                var result = await action();
                
                if (s_State == CircuitState.HalfOpen)
                {
                    s_State = CircuitState.Closed;
                    s_FailureCount = 0;
                    Console.WriteLine("[CircuitBreaker] Circuit recovered, transitioning to Closed state");
                }
                
                return result;
            }
            catch (Exception ex)
            {
                RecordFailure(ex);
                Console.WriteLine($"[CircuitBreaker] Action failed: {ex.Message}");
                return fallback();
            }
        }

        /// <summary>
        /// Record a failure and potentially open the circuit
        /// </summary>
        private static void RecordFailure(Exception ex)
        {
            s_FailureCount++;
            s_LastFailureTime = DateTime.UtcNow;

            if (s_FailureCount >= s_FailureThreshold)
            {
                s_State = CircuitState.Open;
                Console.WriteLine($"[CircuitBreaker] Circuit opened after {s_FailureCount} failures");
            }
        }

        /// <summary>
        /// Get current circuit state for monitoring
        /// </summary>
        public static CircuitState GetState()
        {
            return s_State;
        }

        /// <summary>
        /// Reset circuit breaker to closed state
        /// </summary>
        public static void Reset()
        {
            s_State = CircuitState.Closed;
            s_FailureCount = 0;
            s_LastFailureTime = DateTime.MinValue;
            Console.WriteLine("[CircuitBreaker] Circuit manually reset to Closed state");
        }

        /// <summary>
        /// Get circuit breaker statistics
        /// </summary>
        public static string GetStats()
        {
            return $"State: {s_State}, Failures: {s_FailureCount}, Last Failure: {(s_LastFailureTime == DateTime.MinValue ? "Never" : s_LastFailureTime.ToString())}";
        }
    }

    /// <summary>
    /// Retry logic with exponential backoff for LLM API calls
    /// </summary>
    public static class RetryPolicy
    {
        private const int MaxRetries = 3;
        private static readonly TimeSpan[] s_BackoffDelays = 
        {
            TimeSpan.FromSeconds(1),
            TimeSpan.FromSeconds(2),
            TimeSpan.FromSeconds(4)
        };

        /// <summary>
        /// Execute an action with retry logic and exponential backoff
        /// </summary>
        public static async Task<T> ExecuteAsync<T>(Func<Task<T>> action, Func<T> fallback)
        {
            Exception lastException = null;

            for (int attempt = 0; attempt <= MaxRetries; attempt++)
            {
                try
                {
                    if (attempt > 0)
                    Console.WriteLine($"[RetryPolicy] Attempt {attempt + 1} of {MaxRetries + 1}");

                    var result = await action();
                    
                    if (attempt > 0)
                        Console.WriteLine($"[RetryPolicy] Success on attempt {attempt + 1}");
                    
                    return result;
                }
                catch (Exception ex)
                {
                    lastException = ex;
                    Console.WriteLine($"[RetryPolicy] Attempt {attempt + 1} failed: {ex.Message}");

                    if (attempt < MaxRetries)
                    {
                        var delay = s_BackoffDelays[Math.Min(attempt, s_BackoffDelays.Length - 1)];
                        Console.WriteLine($"[RetryPolicy] Waiting {delay.TotalSeconds} seconds before retry...");
                        await Task.Delay(delay);
                    }
                }
            }

            Console.WriteLine($"[RetryPolicy] All {MaxRetries + 1} attempts failed, using fallback");
            return fallback();
        }

        /// <summary>
        /// Execute synchronous action with retry logic
        /// </summary>
        public static T Execute<T>(Func<T> action, Func<T> fallback)
        {
            Exception lastException = null;

            for (int attempt = 0; attempt <= MaxRetries; attempt++)
            {
                try
                {
                    if (attempt > 0)
                        Console.WriteLine($"[RetryPolicy] Attempt {attempt + 1} of {MaxRetries + 1}");

                    var result = action();
                    
                    if (attempt > 0)
                        Console.WriteLine($"[RetryPolicy] Success on attempt {attempt + 1}");
                    
                    return result;
                }
                catch (Exception ex)
                {
                    lastException = ex;
                    Console.WriteLine($"[RetryPolicy] Attempt {attempt + 1} failed: {ex.Message}");

                    if (attempt < MaxRetries)
                    {
                        var delay = s_BackoffDelays[Math.Min(attempt, s_BackoffDelays.Length - 1)];
                        Console.WriteLine($"[RetryPolicy] Waiting {delay.TotalSeconds} seconds before retry...");
                        Thread.Sleep(delay);
                    }
                }
            }

            Console.WriteLine($"[RetryPolicy] All {MaxRetries + 1} attempts failed, using fallback");
            return fallback();
        }
    }

    /// <summary>
    /// Error categorization for better handling and logging
    /// </summary>
    public static class ErrorClassifier
    {
        public enum ErrorType
        {
            NetworkError,        // Connection issues, timeouts
            APIError,           // API service errors
            ValidationError,    // Invalid responses, parsing errors
            AuthenticationError, // API key issues
            RateLimitError,     // Too many requests
            UnknownError        // Unclassified errors
        }

        /// <summary>
        /// Categorize an exception for appropriate handling
        /// </summary>
        public static ErrorType ClassifyError(Exception ex)
        {
            if (ex == null)
                return ErrorType.UnknownError;

            string message = ex.Message.ToLower();
            string type = ex.GetType().Name.ToLower();

            // Network-related errors
            if (message.Contains("timeout") || message.Contains("connection") || 
                message.Contains("network") || message.Contains("host") ||
                type.Contains("http") || type.Contains("web"))
            {
                return ErrorType.NetworkError;
            }

            // Authentication errors
            if (message.Contains("unauthorized") || message.Contains("forbidden") ||
                message.Contains("api key") || message.Contains("authentication"))
            {
                return ErrorType.AuthenticationError;
            }

            // Rate limiting
            if (message.Contains("rate limit") || message.Contains("too many requests") ||
                message.Contains("quota") || message.Contains("limit"))
            {
                return ErrorType.RateLimitError;
            }

            // API service errors
            if (message.Contains("api") || message.Contains("service") ||
                type.Contains("api"))
            {
                return ErrorType.APIError;
            }

            // Validation/parsing errors
            if (message.Contains("parse") || message.Contains("json") ||
                message.Contains("format") || message.Contains("invalid") ||
                type.Contains("json"))
            {
                return ErrorType.ValidationError;
            }

            return ErrorType.UnknownError;
        }

        /// <summary>
        /// Get user-friendly error message based on error type
        /// </summary>
        public static string GetUserFriendlyMessage(ErrorType errorType)
        {
            switch (errorType)
            {
                case ErrorType.NetworkError:
                    return "I'm having trouble connecting to my knowledge sources. Let me try a different approach.";
                case ErrorType.APIError:
                    return "My storytelling service seems to be having issues. Let me craft you an adventure from my archives.";
                case ErrorType.ValidationError:
                    return "I received some confusing information. Let me create a quest from my collection instead.";
                case ErrorType.AuthenticationError:
                    return "My access to the grand archives is temporarily blocked. Let me share a classic tale with you.";
                case ErrorType.RateLimitError:
                    return "Too many adventurers are seeking tales at once! Let me tell you a story I've prepared.";
                default:
                    return "The threads of fate are unclear at the moment. Let me weave you a tale from my memory.";
            }
        }

        /// <summary>
        /// Get whether retry is recommended for this error type
        /// </summary>
        public static bool ShouldRetry(ErrorType errorType)
        {
            switch (errorType)
            {
                case ErrorType.NetworkError:
                case ErrorType.APIError:
                    return true;
                case ErrorType.ValidationError:
                case ErrorType.AuthenticationError:
                case ErrorType.RateLimitError:
                    return false;
                default:
                    return true;
            }
        }
    }
}
