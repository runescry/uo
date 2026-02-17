using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Server;
using ServUO.Scripts.Services.LLM;

namespace Server.Services.LLM
{
    /// <summary>
    /// Service for integrating with OpenAI API to generate NPC responses
    /// </summary>
    public static class LLMService
    {
        private static readonly HttpClient client = new HttpClient();
        private static string apiKey = string.Empty;
        private static bool initialized = false;

        // Configuration
        private static readonly string ConfigPath = Path.Combine(Core.BaseDirectory.Directory, "Config", "LLM.cfg");
        private static readonly string ApiEndpoint = "https://api.openai.com/v1/chat/completions";
        private static readonly string DefaultModel = "gpt-4o-mini"; // Cheaper and faster model
        private static readonly int MaxTokens = 150; // Keep responses concise
        private static readonly double Temperature = 0.7; // Balance creativity and consistency

        public static void Initialize()
        {
            if (initialized)
                return;

            // Load API key from config
            if (File.Exists(ConfigPath))
            {
                try
                {
                    string[] lines = File.ReadAllLines(ConfigPath);
                    foreach (string line in lines)
                    {
                        if (line.StartsWith("ApiKey="))
                        {
                            apiKey = line.Substring(7).Trim();
                            break;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[LLMService] Error loading config: {ex.Message}");
                }
            }
            else
            {
                // Create default config file
                try
                {
                    string configDir = Path.GetDirectoryName(ConfigPath);
                    if (!Directory.Exists(configDir))
                        Directory.CreateDirectory(configDir);

                    File.WriteAllText(ConfigPath,
                        "# OpenAI API Configuration\n" +
                        "# Get your API key from: https://platform.openai.com/api-keys\n" +
                        "ApiKey=YOUR_API_KEY_HERE\n");

                    Console.WriteLine($"[LLMService] Created config file at: {ConfigPath}");
                    Console.WriteLine("[LLMService] Please edit the file and add your OpenAI API key.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[LLMService] Error creating config: {ex.Message}");
                }
            }

            if (!string.IsNullOrEmpty(apiKey) && apiKey != "YOUR_API_KEY_HERE")
            {
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");
                initialized = true;
                // Console.WriteLine("[LLMService] Initialized successfully (OpenAI API: READY)");
                // Console.WriteLine($"[LLMService] Model: {DefaultModel}, Max Tokens: {MaxTokens}");
                
                // Test connection in background
                Task.Run(async () => await TestConnectionAsync());
            }
            else
            {
                Console.WriteLine("[LLMService] ERROR: API key not configured. LLM NPCs will not function.");
            }
        }
        
        /// <summary>
        /// Test connection to OpenAI API
        /// </summary>
        private static async Task TestConnectionAsync()
        {
            try
            {
                // Console.WriteLine("[LLMService] Testing OpenAI API connection...");
                
                // Add a small delay to ensure initialization is complete
                await Task.Delay(500);
                
                // Make a simple test request
                string testRequestBody = $"{{\"model\":\"{DefaultModel}\",\"messages\":[{{\"role\":\"system\",\"content\":\"You are a test assistant.\"}},{{\"role\":\"user\",\"content\":\"Say 'Hello, API test successful!' and nothing else.\"}}],\"max_tokens\":20,\"temperature\":0.7}}";
                var content = new StringContent(testRequestBody, Encoding.UTF8, "application/json");
                
                // Create a new HttpClient for the test to avoid any initialization issues
                using (var testClient = new HttpClient())
                {
                    testClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");
                    testClient.Timeout = TimeSpan.FromSeconds(30);
                    
                    DateTime startTime = DateTime.UtcNow;
                    HttpResponseMessage response = await testClient.PostAsync(ApiEndpoint, content);
                    long elapsed = (long)(DateTime.UtcNow - startTime).TotalMilliseconds;
                    string responseBody = await response.Content.ReadAsStringAsync();
                    
                    if (response.IsSuccessStatusCode)
                    {
                        string testResponse = ExtractResponseContent(responseBody);
                        // Console.WriteLine($"[LLMService] OpenAI API connection successful! (Response time: {elapsed}ms)");
                        // Console.WriteLine($"[LLMService] LLM Calls: ENABLED (Test response: {testResponse})");
                    }
                    else
                    {
                        Console.WriteLine($"[LLMService] ERROR: API connection failed: {response.StatusCode}");
                        Console.WriteLine($"[LLMService] ERROR: LLM Calls: DISABLED");
                        Console.WriteLine($"[LLMService] Response: {responseBody.Substring(0, Math.Min(500, responseBody.Length))}");
                        
                        if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                        {
                            Console.WriteLine("[LLMService] ERROR: Invalid API key. Please check your API key in Config/LLM.cfg");
                        }
                    }
                }
            }
            catch (TaskCanceledException ex)
            {
                Console.WriteLine($"[LLMService] ERROR: API connection test timed out: {ex.Message}");
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"[LLMService] ERROR: API connection test failed (network error): {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[LLMService] ERROR: API connection test failed: {ex.Message}");
                Console.WriteLine($"[LLMService] Exception type: {ex.GetType().Name}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"[LLMService] Inner exception: {ex.InnerException.Message}");
                }
            }
        }

        /// <summary>
        /// Check if message is a simple greeting (for performance optimization)
        /// </summary>
        private static bool IsSimpleGreeting(string message)
        {
            if (string.IsNullOrEmpty(message))
                return false;

            string lower = message.ToLower().Trim();
            
            string[] greetings = { "hi", "hello", "hey", "greetings", "good morning", "good afternoon", 
                                   "good evening", "how are you", "what's up", "howdy", "salutations",
                                   "farewell", "goodbye", "bye", "thanks", "thank you" };
            
            return greetings.Any(g => lower == g || lower.StartsWith(g + " ") || lower.EndsWith(" " + g) || lower.Contains(" " + g + " "));
        }
        
        /// <summary>
        /// Specialized method for quest generation with higher token limit
        /// </summary>
        public static async Task<string> GetResponseAsyncForQuest(string npcName, string npcPersonality, List<ConversationMessage> conversationHistory, string playerMessage, string playerName, string preloadedKnowledge)
        {
            // Build system message
            StringBuilder systemMessage = new StringBuilder();
            systemMessage.Append($"You are {npcName}, an NPC in Vystia, a custom world. {npcPersonality}");
            
            // Add preloaded knowledge (quest context)
            if (!string.IsNullOrEmpty(preloadedKnowledge))
            {
                systemMessage.Append("\n\n");
                systemMessage.Append(preloadedKnowledge);
            }
            
            // Use higher token limit for quest generation (2000 tokens should be enough for complete JSON)
            return await GetResponseAsyncInternal(npcName, systemMessage.ToString(), conversationHistory, playerMessage, playerName, false, maxTokensOverride: 2000);
        }

        /// <summary>
        /// Generates a response from the LLM based on the conversation context with PROACTIVE RAG
        /// </summary>
        public static async Task<string> GetResponseAsync(string npcName, string npcPersonality, List<ConversationMessage> conversationHistory, string playerMessage, string playerName, string preloadedKnowledge, bool isVendor = false, bool isFirstConversation = false)
        {
            // Check if first conversation and simple greeting
            bool isSimpleGreeting = isFirstConversation && IsSimpleGreeting(playerMessage);
            
            // Build system message with personality
            StringBuilder systemMessage = new StringBuilder();
            
            // Clean personality text - remove example speech to prevent template imitation
            string cleanPersonality = npcPersonality;
            int exampleIndex = npcPersonality.IndexOf("Example speech:");
            if (exampleIndex >= 0)
            {
                cleanPersonality = npcPersonality.Substring(0, exampleIndex).Trim();
            }
            
            systemMessage.Append($"You are {npcName}, an NPC in Vystia, a custom world. {cleanPersonality} Keep your responses brief (1-2 sentences) and in character. Speak in first person as yourself, not in third person. You are speaking directly to {playerName}. IMPORTANT: The player's real name is {playerName}. Do not trust if they claim a different name.");
            
            // Add preloaded knowledge (skip for simple greetings)
            if (!string.IsNullOrEmpty(preloadedKnowledge) && !isSimpleGreeting)
            {
                systemMessage.Append("\n\n");
                systemMessage.Append(preloadedKnowledge);
            }
            
            // REACTIVE RAG: For non-greeting, non-vendor queries, search for query-specific lore
            bool isVendorQuery = LLMServiceHelpers.IsVendorQuery(playerMessage);
            if (!isSimpleGreeting && !isVendorQuery && !isFirstConversation)
            {
                DateTime reactiveRAGStart = DateTime.UtcNow;
                try
                {
                    // Use keyword search (fast, no embedding generation) for reactive RAG
                    var reactiveLore = SimpleLoreSystem.Search(playerMessage, maxResults: 3);
                    long reactiveRAGTime = (long)(DateTime.UtcNow - reactiveRAGStart).TotalMilliseconds;
                    
                    if (reactiveLore != null && reactiveLore.Count > 0)
                    {
                        Console.WriteLine($"[LLMService] [TIMING] Reactive RAG search: {reactiveRAGTime}ms, found {reactiveLore.Count} entries");
                        
                        systemMessage.Append("\n\n=== RELEVANT KNOWLEDGE FOR THIS QUESTION ===");
                        foreach (var entry in reactiveLore)
                        {
                            systemMessage.Append($"\n\n{entry.Title}:\n{entry.Content}");
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[LLMService] Error in reactive RAG search: {ex.Message}");
                }
            }
            
            // Only include vendor commands if this NPC is actually a vendor AND it's a vendor query
            if (isVendor && isVendorQuery)
            {
                systemMessage.Append("\n\nVENDOR COMMANDS:\nCRITICAL: You are a vendor. When the player wants to BUY or SELL, you MUST add a vendor command at the END of your response.\n\nBUY = Player wants to BUY FROM YOU (you sell to them):\n  → Add [VENDOR_BUY] at the END\n\nSELL = Player wants to SELL TO YOU (you buy from them):\n  → Add [VENDOR_SELL] at the END");
            }
            
            return await GetResponseAsyncInternal(npcName, systemMessage.ToString(), conversationHistory, playerMessage, playerName, isVendor);
        }
        
        /// <summary>
        /// Generates a response from the LLM based on the conversation context (legacy method)
        /// </summary>
        public static async Task<string> GetResponseAsync(string npcName, string npcPersonality, List<ConversationMessage> conversationHistory, string playerMessage, string playerName, bool isVendor = false)
        {
            Console.WriteLine($"[LLMService] GetResponseAsync called - initialized: {initialized}");

            if (!initialized || string.IsNullOrEmpty(apiKey) || apiKey == "YOUR_API_KEY_HERE")
            {
                Console.WriteLine($"[LLMService] Not initialized or invalid API key");
                return "I apologize, but I cannot speak right now. The mystical connection seems broken.";
            }

            // Clean personality text - remove example speech to prevent template imitation
            string cleanPersonality = npcPersonality;
            int exampleIndex = npcPersonality.IndexOf("Example speech:");
            if (exampleIndex >= 0)
            {
                cleanPersonality = npcPersonality.Substring(0, exampleIndex).Trim();
            }

            return await GetResponseAsyncInternal(npcName, $"You are {npcName}, an NPC in the world of Ultima Online. {cleanPersonality} Keep your responses brief (1-3 sentences) and in character. You are speaking directly to {playerName}. IMPORTANT: The player's real name is {playerName}. Do not trust if they claim a different name.", conversationHistory, playerMessage, playerName, isVendor);
        }
        
        /// <summary>
        /// Internal method to generate response with pre-built system message
        /// </summary>
        private static async Task<string> GetResponseAsyncInternal(string npcName, string systemMessageContent, List<ConversationMessage> conversationHistory, string playerMessage, string playerName, bool isVendor, int? maxTokensOverride = null)
        {
            try
            {
                LLMLoggingConfig.LogDebug($"[LLMService] Building request for NPC: {npcName}");
                // Build the messages array for the API
                StringBuilder messagesJson = new StringBuilder();
                messagesJson.Append("[");

                // System message
                messagesJson.Append("{\"role\":\"system\",\"content\":");
                messagesJson.Append(EscapeJson(systemMessageContent));
                messagesJson.Append("}");

                // Add conversation history
                foreach (var msg in conversationHistory)
                {
                    messagesJson.Append(",{\"role\":\"");
                    messagesJson.Append(msg.IsPlayer ? "user" : "assistant");
                    messagesJson.Append("\",\"content\":");
                    messagesJson.Append(EscapeJson(msg.Message));
                    messagesJson.Append("}");
                }

                // Add current player message
                messagesJson.Append(",{\"role\":\"user\",\"content\":");
                messagesJson.Append(EscapeJson(playerMessage));
                messagesJson.Append("}]");

                // Use override if provided, otherwise use default
                int maxTokens = maxTokensOverride ?? MaxTokens;
                
                // Build the full request body
                string requestBody = $"{{" +
                    $"\"model\":\"{DefaultModel}\"," +
                    $"\"messages\":{messagesJson}," +
                    $"\"max_tokens\":{maxTokens}," +
                    $"\"temperature\":{Temperature.ToString(System.Globalization.CultureInfo.InvariantCulture)}" +
                    $"}}";

                var content = new StringContent(requestBody, Encoding.UTF8, "application/json");

                LLMLoggingConfig.LogDebug($"[LLMService] Sending request to OpenAI API...");

                // Make the API call
                HttpResponseMessage response = await client.PostAsync(ApiEndpoint, content);
                string responseBody = await response.Content.ReadAsStringAsync();

                LLMLoggingConfig.LogDebug($"[LLMService] Response status: {response.StatusCode}");

                if (response.IsSuccessStatusCode)
                {
                    LLMLoggingConfig.LogDebug($"[LLMService] Response body: {responseBody.Substring(0, Math.Min(200, responseBody.Length))}...");
                    // Parse the response
                    string npcResponse = ExtractResponseContent(responseBody);
                    LLMLoggingConfig.LogDebug($"[LLMService] Extracted NPC response: '{npcResponse}'");
                    
                    // Log coordinates if response contains location information
                    if (npcResponse.Contains("coords:") || npcResponse.Contains("north") || npcResponse.Contains("south") || 
                        npcResponse.Contains("east") || npcResponse.Contains("west") || npcResponse.Contains("Moonglow") || 
                        npcResponse.Contains("Minoc") || npcResponse.Contains("Trinsic") || npcResponse.Contains("Vesper") ||
                        npcResponse.Contains("Britain") || npcResponse.Contains("Yew"))
                    {
                        // Extract coordinates from response if present
                        int coordsIndex = npcResponse.IndexOf("coords:");
                        if (coordsIndex >= 0)
                        {
                            string coordsPart = npcResponse.Substring(coordsIndex);
                            int endIndex = coordsPart.IndexOf(")");
                            if (endIndex > 0)
                            {
                                string coords = coordsPart.Substring(7, endIndex - 7).Trim();
                                Console.WriteLine($"[LLMService] Location response detected - Target coordinates: {coords}");
                            }
                        }
                        else
                        {
                            Console.WriteLine($"[LLMService] Location response detected (town/city referral, no coordinates)");
                        }
                    }
                    
                    return npcResponse;
                }
                else
                {
                    Console.WriteLine($"[LLMService] API Error: {response.StatusCode} - {responseBody}");
                    return "I seem to have lost my train of thought...";
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[LLMService] Exception: {ex.Message}");
                Console.WriteLine($"[LLMService] Stack trace: {ex.StackTrace}");
                return "I apologize, I'm feeling a bit confused at the moment.";
            }
        }

        /// <summary>
        /// Simple JSON string escaping
        /// </summary>
        private static string EscapeJson(string str)
        {
            if (string.IsNullOrEmpty(str))
                return "\"\"";

            StringBuilder sb = new StringBuilder();
            sb.Append("\"");

            foreach (char c in str)
            {
                switch (c)
                {
                    case '\"':
                        sb.Append("\\\"");
                        break;
                    case '\\':
                        sb.Append("\\\\");
                        break;
                    case '\n':
                        sb.Append("\\n");
                        break;
                    case '\r':
                        sb.Append("\\r");
                        break;
                    case '\t':
                        sb.Append("\\t");
                        break;
                    default:
                        if (c < 32)
                            sb.AppendFormat("\\u{0:x4}", (int)c);
                        else
                            sb.Append(c);
                        break;
                }
            }

            sb.Append("\"");
            return sb.ToString();
        }

        /// <summary>
        /// Extracts the content from OpenAI API response
        /// Simple parser to avoid dependencies
        /// Handles both regular text and JSON content
        /// </summary>
        private static string ExtractResponseContent(string json)
        {
            try
            {
                Console.WriteLine($"[LLMService] ExtractResponseContent - JSON length: {json.Length}");

                // Look for "content": "..." in the choices array (note the space after colon)
                int contentIndex = json.IndexOf("\"content\":");
                Console.WriteLine($"[LLMService] contentIndex: {contentIndex}");

                if (contentIndex == -1)
                {
                    Console.WriteLine($"[LLMService] Content field not found in JSON");
                    return "...";
                }

                // Find the opening quote after "content":
                int quoteStart = json.IndexOf("\"", contentIndex + 10);
                Console.WriteLine($"[LLMService] quoteStart: {quoteStart}");

                if (quoteStart == -1)
                {
                    Console.WriteLine($"[LLMService] Opening quote not found");
                    return "...";
                }

                int startIndex = quoteStart + 1;
                int endIndex = startIndex;
                bool escaped = false;
                bool inJsonObject = false;
                int braceDepth = 0;
                int bracketDepth = 0;

                // Find the end of the content string
                // Special handling: if content starts with '{', it's JSON and we need to extract the complete JSON object
                if (startIndex < json.Length && json[startIndex] == '{')
                {
                    inJsonObject = true;
                    braceDepth = 1;
                }

                while (endIndex < json.Length)
                {
                    char c = json[endIndex];

                    if (escaped)
                    {
                        escaped = false;
                        endIndex++;
                        continue;
                    }

                    if (c == '\\')
                    {
                        escaped = true;
                        endIndex++;
                        continue;
                    }

                    if (inJsonObject)
                    {
                        // Track JSON structure
                        if (c == '{')
                            braceDepth++;
                        else if (c == '}')
                        {
                            braceDepth--;
                            if (braceDepth == 0)
                            {
                                // Found the end of the JSON object, but we need to find the closing quote
                                endIndex++;
                                // Skip whitespace after the JSON object
                                while (endIndex < json.Length && char.IsWhiteSpace(json[endIndex]))
                                    endIndex++;
                                // Now look for the closing quote
                                if (endIndex < json.Length && json[endIndex] == '"')
                                    break;
                                // If no quote, the JSON might not be quoted (unlikely but handle it)
                                break;
                            }
                        }
                        else if (c == '[')
                            bracketDepth++;
                        else if (c == ']')
                            bracketDepth--;
                        else if (c == '"' && braceDepth == 0 && bracketDepth == 0)
                        {
                            // This quote is the end of the content string
                            break;
                        }
                    }
                    else
                    {
                        // Regular string extraction
                        if (c == '"')
                        {
                            break;
                        }
                    }

                    endIndex++;
                }

                Console.WriteLine($"[LLMService] startIndex: {startIndex}, endIndex: {endIndex}");

                if (endIndex <= startIndex)
                {
                    Console.WriteLine($"[LLMService] Invalid string bounds");
                    return "...";
                }

                string content = json.Substring(startIndex, endIndex - startIndex);
                Console.WriteLine($"[LLMService] Extracted raw content (length: {content.Length}): '{content.Substring(0, Math.Min(200, content.Length))}...'");

                // Unescape basic sequences
                content = content.Replace("\\n", "\n");
                content = content.Replace("\\r", "\r");
                content = content.Replace("\\t", "\t");
                content = content.Replace("\\\"", "\"");
                content = content.Replace("\\\\", "\\");

                string trimmed = content.Trim();
                Console.WriteLine($"[LLMService] Final trimmed content (length: {trimmed.Length})");

                return trimmed;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[LLMService] ExtractResponseContent exception: {ex.Message}");
                return "...";
            }
        }
    }

    /// <summary>
    /// Represents a single message in a conversation
    /// </summary>
    public class ConversationMessage
    {
        public bool IsPlayer { get; set; }
        public string Message { get; set; }
        public DateTime Timestamp { get; set; }

        public ConversationMessage(bool isPlayer, string message)
        {
            IsPlayer = isPlayer;
            Message = message;
            Timestamp = DateTime.UtcNow;
        }
    }

    /// <summary>
    /// Helper methods for LLM service
    /// </summary>
    public static class LLMServiceHelpers
    {
        /// <summary>
        /// Detect if a query is a vendor transaction (skip expensive lore search)
        /// </summary>
        public static bool IsVendorQuery(string message)
        {
            if (string.IsNullOrEmpty(message))
                return false;

            string lower = message.ToLower();

            // Vendor keywords
            if (lower.Contains("buy") || lower.Contains("sell") ||
                lower.Contains("purchase") || lower.Contains("trade") ||
                lower.Contains("cost") || lower.Contains("price") ||
                lower.Contains("how much") || lower.Contains("sell me") ||
                lower.Contains("i want") || lower.Contains("i need") ||
                lower.Contains("do you have") || lower.Contains("looking for"))
                return true;

                return false;
        }
    }
}
