using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Server;

namespace Server.Services.LLM
{
    /// <summary>
    /// Service for generating and managing vector embeddings using Ollama
    /// </summary>
    public static class EmbeddingService
    {
        private static readonly HttpClient client;
        private static string endpoint = "http://localhost:11434";
        private static string modelName = "nomic-embed-text";
        private static bool isAvailable = false;
        private static bool initialized = false;

        static EmbeddingService()
        {
            // Initialize HttpClient with timeout before any use
            client = new HttpClient();
            client.Timeout = TimeSpan.FromSeconds(60);
        }

        public static void Initialize()
        {
            if (initialized)
                return;

            initialized = true;
            // Console.WriteLine($"[EmbeddingService] Initialized with model: {modelName}");

            // Test if Ollama is available (synchronous check for immediate result)
            TestAvailabilitySync();
        }

        /// <summary>
        /// Test if Ollama embedding service is available (synchronous for immediate check)
        /// </summary>
        private static void TestAvailabilitySync()
        {
            try
            {
                // Use a short timeout for quick check
                var testClient = new HttpClient();
                testClient.Timeout = TimeSpan.FromSeconds(3);
                
                var task = testClient.GetAsync($"{endpoint}/api/tags");
                bool completed = task.Wait(TimeSpan.FromSeconds(3));
                
                if (completed && !task.IsFaulted && !task.IsCanceled && task.Result.IsSuccessStatusCode)
                {
                    isAvailable = true;
                    // Console.WriteLine("[EmbeddingService] Ollama is available for embeddings");
                }
                else
                {
                    isAvailable = false;
                    Console.WriteLine("[EmbeddingService] WARNING: Ollama not responding (will retry on first use)");
                }
                
                testClient.Dispose();
            }
            catch (Exception ex)
            {
                isAvailable = false;
                Console.WriteLine($"[EmbeddingService] WARNING: Could not connect to Ollama: {ex.Message}");
                Console.WriteLine("[EmbeddingService] Will retry on first embedding request. Keyword search works fine.");
            }
        }

        /// <summary>
        /// Check if embedding service is available
        /// </summary>
        public static bool IsAvailable()
        {
            return isAvailable;
        }

        /// <summary>
        /// Generate embedding vector for a text string
        /// </summary>
        public static async Task<float[]> GenerateEmbeddingAsync(string text)
        {
            // Retry availability check if not yet available (Ollama might have started after init)
            if (!isAvailable)
            {
                // Quick retry check
                try
                {
                    var testClient = new HttpClient();
                    testClient.Timeout = TimeSpan.FromSeconds(2);
                    var response = await testClient.GetAsync($"{endpoint}/api/tags");
                    testClient.Dispose();
                    
                    if (response.IsSuccessStatusCode)
                    {
                        isAvailable = true;
                        Console.WriteLine("[EmbeddingService] Ollama now available (detected on first use)");
                    }
                }
                catch
                {
                    // Still not available, will use keyword search
                }
            }
            
            if (!isAvailable)
            {
                Console.WriteLine("[EmbeddingService] Service not available, cannot generate embedding");
                return null;
            }

            try
            {
                // Log stack trace to identify caller (for performance debugging)
                System.Diagnostics.StackTrace stackTrace = new System.Diagnostics.StackTrace();
                string caller = "Unknown";
                if (stackTrace.FrameCount > 1)
                {
                    var frame = stackTrace.GetFrame(1);
                    if (frame != null)
                    {
                        var method = frame.GetMethod();
                        if (method != null)
                        {
                            caller = $"{method.DeclaringType?.Name}.{method.Name}";
                        }
                    }
                }
                
                // Build request for Ollama embeddings API
                string requestBody = BuildEmbeddingRequest(text);
                var content = new StringContent(requestBody, Encoding.UTF8, "application/json");

                // Only log verbose details if not a vendor template pre-computation (reduce startup noise)
                bool isVendorTemplate = caller.Contains("PrecomputeVendorQueryEmbeddingsAsync");
                if (!isVendorTemplate)
                {
                    // Console.WriteLine($"[EmbeddingService] Generating embedding for: {text.Substring(0, Math.Min(50, text.Length))}... (Called from: {caller})");
                }

                HttpResponseMessage response = await client.PostAsync($"{endpoint}/api/embeddings", content);
                string responseBody = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    float[] embedding = ParseEmbeddingResponse(responseBody);
                    if (!isVendorTemplate)
                    {
                        Console.WriteLine($"[EmbeddingService] Generated embedding with {embedding?.Length ?? 0} dimensions");
                    }
                    return embedding;
                }
                else
                {
                    Console.WriteLine($"[EmbeddingService] Error: {response.StatusCode} - {responseBody}");
                    return null;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[EmbeddingService] Exception generating embedding: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Build Ollama embedding request JSON
        /// </summary>
        private static string BuildEmbeddingRequest(string text)
        {
            // Escape text for JSON
            string escapedText = text
                .Replace("\\", "\\\\")
                .Replace("\"", "\\\"")
                .Replace("\n", "\\n")
                .Replace("\r", "\\r")
                .Replace("\t", "\\t");

            return $"{{" +
                   $"\"model\":\"{modelName}\"," +
                   $"\"prompt\":\"{escapedText}\"" +
                   $"}}";
        }

        /// <summary>
        /// Parse embedding vector from Ollama response
        /// </summary>
        private static float[] ParseEmbeddingResponse(string json)
        {
            try
            {
                // Find "embedding":[...] in response
                int embeddingIndex = json.IndexOf("\"embedding\":[");
                if (embeddingIndex == -1)
                {
                    Console.WriteLine("[EmbeddingService] Could not find embedding array in response");
                    return null;
                }

                int startIndex = json.IndexOf("[", embeddingIndex);
                int endIndex = json.IndexOf("]", startIndex);

                if (startIndex == -1 || endIndex == -1)
                {
                    Console.WriteLine("[EmbeddingService] Could not parse embedding array bounds");
                    return null;
                }

                string arrayContent = json.Substring(startIndex + 1, endIndex - startIndex - 1);
                string[] values = arrayContent.Split(',');

                float[] embedding = new float[values.Length];
                for (int i = 0; i < values.Length; i++)
                {
                    if (float.TryParse(values[i].Trim(), System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out float value))
                    {
                        embedding[i] = value;
                    }
                    else
                    {
                        Console.WriteLine($"[EmbeddingService] Warning: Could not parse value at index {i}: {values[i]}");
                        embedding[i] = 0f;
                    }
                }

                return embedding;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[EmbeddingService] Error parsing embedding: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Calculate cosine similarity between two embedding vectors
        /// </summary>
        public static float CosineSimilarity(float[] a, float[] b)
        {
            if (a == null || b == null || a.Length != b.Length)
            {
                Console.WriteLine("[EmbeddingService] Invalid vectors for cosine similarity");
                return 0f;
            }

            float dotProduct = 0f;
            float magnitudeA = 0f;
            float magnitudeB = 0f;

            for (int i = 0; i < a.Length; i++)
            {
                dotProduct += a[i] * b[i];
                magnitudeA += a[i] * a[i];
                magnitudeB += b[i] * b[i];
            }

            magnitudeA = (float)Math.Sqrt(magnitudeA);
            magnitudeB = (float)Math.Sqrt(magnitudeB);

            if (magnitudeA == 0f || magnitudeB == 0f)
            {
                return 0f;
            }

            return dotProduct / (magnitudeA * magnitudeB);
        }

        /// <summary>
        /// Batch generate embeddings for multiple texts
        /// </summary>
        public static async Task<Dictionary<string, float[]>> GenerateBatchEmbeddingsAsync(List<string> texts)
        {
            var embeddings = new Dictionary<string, float[]>();

            // Console.WriteLine($"[EmbeddingService] Generating {texts.Count} embeddings in batch...");

            for (int i = 0; i < texts.Count; i++)
            {
                string text = texts[i];
                float[] embedding = await GenerateEmbeddingAsync(text);

                if (embedding != null)
                {
                    embeddings[text] = embedding;
                }

                // Progress update every 10 items
                if ((i + 1) % 10 == 0)
                {
                    Console.WriteLine($"[EmbeddingService] Progress: {i + 1}/{texts.Count}");
                }

                // Small delay to avoid overwhelming Ollama
                await Task.Delay(100);
            }

            Console.WriteLine($"[EmbeddingService] Batch complete. Generated {embeddings.Count}/{texts.Count} embeddings");
            return embeddings;
        }
    }
}
