using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Server;

namespace Server.Services.LLM
{
    /// <summary>
    /// Service for integrating with local LLM (Ollama)
    /// </summary>
    public static class LocalLLMService
    {
        private static readonly HttpClient client = new HttpClient();
        private static bool initialized = false;
        private static string endpoint = "http://localhost:11434";
        private static string modelName = "phi3:mini";

        // Configuration
        private static readonly string ConfigPath = Path.Combine(Core.BaseDirectory.Directory, "Config", "LocalLLM.cfg");

        public static void Initialize()
        {
            if (initialized)
                return;

            // Load configuration
            if (File.Exists(ConfigPath))
            {
                try
                {
                    string[] lines = File.ReadAllLines(ConfigPath);
                    foreach (string line in lines)
                    {
                        if (line.StartsWith("Endpoint="))
                        {
                            endpoint = line.Substring(9).Trim();
                        }
                        else if (line.StartsWith("Model="))
                        {
                            modelName = line.Substring(6).Trim();
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[LocalLLM] Error loading config: {ex.Message}");
                }
            }
            else
            {
                // Create default config
                try
                {
                    string configDir = Path.GetDirectoryName(ConfigPath);
                    if (!Directory.Exists(configDir))
                        Directory.CreateDirectory(configDir);

                    File.WriteAllText(ConfigPath,
                        "# Local LLM Configuration (Ollama)\n" +
                        "# Install: https://ollama.ai\n" +
                        "# Download model: ollama pull phi3:mini\n" +
                        "# Start server: ollama serve\n" +
                        "\n" +
                        "Endpoint=http://localhost:11434\n" +
                        "Model=phi3:mini\n" +
                        "\n" +
                        "# Available models:\n" +
                        "# phi3:mini (FASTEST, 8GB RAM, 1-2s responses) - RECOMMENDED\n" +
                        "# llama3.1:8b (good quality, 16GB RAM, 8-10s responses)\n" +
                        "# llama3.1:70b (best quality, 48GB+ RAM, very slow)\n" +
                        "# mistral:7b (fast, 16GB RAM)\n");

                    Console.WriteLine($"[LocalLLM] Created config file at: {ConfigPath}");
                    Console.WriteLine("[LocalLLM] Please install Ollama and download a model.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[LocalLLM] Error creating config: {ex.Message}");
                }
            }

            // Set timeout for local LLM (CPU needs more time)
            client.Timeout = TimeSpan.FromSeconds(60);

            initialized = true;
            Console.WriteLine($"[LocalLLM] Initialized (FALLBACK ONLY) - Endpoint: {endpoint}, Model: {modelName}");
            Console.WriteLine($"[LocalLLM] Note: OpenAI is the default provider. Local LLM is available as fallback.");
            Console.WriteLine($"[LocalLLM] Error handling: Retries={LLMErrorHandler.MaxRetries}, Circuit breaker enabled");

            // Pre-compute vendor query template embeddings in background (non-blocking)
            Task.Run(async () => await PrecomputeVendorQueryEmbeddingsAsync());

            // Test connection
            TestConnection();
        }

        /// <summary>
        /// Tests connection to local LLM server
        /// </summary>
        private static async void TestConnection()
        {
            try
            {
                var response = await client.GetAsync($"{endpoint}/api/tags");
                if (response.IsSuccessStatusCode)
                {
                    // Console.WriteLine("[LocalLLM] Successfully connected to Ollama server!");

                    // Check GPU status
                    CheckGPUStatus();
                }
                else
                {
                    Console.WriteLine("[LocalLLM] Warning: Ollama server not responding. Make sure it's running.");
                    Console.WriteLine("[LocalLLM] Install: https://ollama.ai");
                    Console.WriteLine($"[LocalLLM] Then run: ollama pull {modelName}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("[LocalLLM] Warning: Could not connect to Ollama server.");
                Console.WriteLine($"[LocalLLM] Error: {ex.Message}");
                Console.WriteLine("[LocalLLM] Make sure Ollama is installed and running.");
            }
        }

        /// <summary>
        /// Generates a response from the local LLM
        /// </summary>
        public static async Task<string> GetResponseAsync(string systemPrompt, string userMessage, int maxTokens = 100)
        {
            if (!initialized)
            {
                Console.WriteLine("[LocalLLM] Service not initialized!");
                return "Local LLM service is not available.";
            }

            try
            {
                // Build the full prompt
                string fullPrompt = $"{systemPrompt}\n\nUser: {userMessage}\n\nAssistant:";

                // Build request body for Ollama
                string requestBody = BuildOllamaRequest(fullPrompt, maxTokens);

                var content = new StringContent(requestBody, Encoding.UTF8, "application/json");

                Console.WriteLine($"[LocalLLM] Sending request to {endpoint}/api/generate");

                // Make the API call
                HttpResponseMessage response = await client.PostAsync($"{endpoint}/api/generate", content);
                string responseBody = await response.Content.ReadAsStringAsync();

                Console.WriteLine($"[LocalLLM] Response status: {response.StatusCode}");

                if (response.IsSuccessStatusCode)
                {
                    // Parse Ollama response
                    string llmResponse = ExtractOllamaResponse(responseBody);
                    Console.WriteLine($"[LocalLLM] Generated response: '{llmResponse.Substring(0, Math.Min(50, llmResponse.Length))}...'");
                    return llmResponse;
                }
                else
                {
                    Console.WriteLine($"[LocalLLM] Error: {response.StatusCode} - {responseBody}");
                    return "I seem to be having trouble thinking right now...";
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[LocalLLM] Exception: {ex.Message}");
                return "I apologize, I'm feeling confused at the moment.";
            }
        }

        /// <summary>
        /// Generates a response with conversation history and PRE-LOADED knowledge (Proactive RAG)
        /// </summary>
        public static async Task<string> GetResponseAsync(string npcName, string npcPersonality, List<ConversationMessage> conversationHistory, string playerMessage, string playerName, string preloadedKnowledge, bool isVendor = false, bool isFirstConversation = false)
        {
            try
            {
                // Build conversation context
                StringBuilder contextBuilder = new StringBuilder();
                
                // Clean personality text - remove example speech to prevent template imitation
                string cleanPersonality = npcPersonality;
                int exampleIndex = npcPersonality.IndexOf("Example speech:");
                if (exampleIndex >= 0)
                {
                    cleanPersonality = npcPersonality.Substring(0, exampleIndex).Trim();
                }
                
                contextBuilder.AppendLine($"You are {npcName}. {cleanPersonality}");
                contextBuilder.AppendLine($"You are talking to {playerName}.");
                contextBuilder.AppendLine();

                // Add pre-loaded knowledge BEFORE rules (so it's part of character definition)
                // For simple greetings on first conversation, use minimal knowledge to speed up response
                bool isSimpleGreeting = isFirstConversation && IsSimpleGreeting(playerMessage);
                
                if (!string.IsNullOrEmpty(preloadedKnowledge))
                {
                    if (isSimpleGreeting)
                    {
                        // For simple greetings, use a condensed version (just the header, no entries)
                        // This reduces prompt size significantly for "hi", "hello", etc.
                        Console.WriteLine($"[LocalLLM] Simple greeting detected - using minimal knowledge (0 chars) for {npcName}");
                        // Skip knowledge for simple greetings - NPC can respond naturally without lore context
                    }
                    else
                    {
                        Console.WriteLine($"[LocalLLM] Adding preloaded knowledge ({preloadedKnowledge.Length} chars) to prompt for {npcName}");
                        Console.WriteLine($"[LocalLLM] Knowledge preview (first 200 chars): {preloadedKnowledge.Substring(0, Math.Min(200, preloadedKnowledge.Length))}...");
                        // FormatKnowledgeForPrompt already includes "YOUR BACKGROUND KNOWLEDGE:" header, so just add it directly
                        contextBuilder.AppendLine(preloadedKnowledge);
                    }
                }
                else
                {
                    Console.WriteLine($"[LocalLLM] WARNING: No preloaded knowledge provided for {npcName} - NPC will not have lore context");
                }

                contextBuilder.AppendLine("RULES:");
                contextBuilder.AppendLine("1. Keep responses brief (1-2 sentences)");
                contextBuilder.AppendLine("2. ONLY your words, never the player's");
                contextBuilder.AppendLine("3. Use EXACT name '" + playerName + "'");
                contextBuilder.AppendLine("4. No meta-commentary, explanations, or markdown");
                contextBuilder.AppendLine("5. Speak naturally as " + npcName);
                contextBuilder.AppendLine();

                // Skip vendor commands for non-vendor queries
                // Use passed isFirstConversation flag (checked BEFORE adding message to history)
                DateTime vendorQueryStart = DateTime.UtcNow;
                bool isVendorQuery = false;
                
                if (isFirstConversation)
                {
                    // Fast keyword-only check (no embeddings) for first conversation
                    string lower = playerMessage.ToLower();
                    isVendorQuery = IsVendorQueryByKeywords(lower);
                    long vendorQueryTime = (long)(DateTime.UtcNow - vendorQueryStart).TotalMilliseconds;
                    Console.WriteLine($"[LocalLLM] [TIMING] Vendor query check (keyword-only): {vendorQueryTime}ms");
                }
                else
                {
                    // Full check (including semantic) for subsequent conversations
                    isVendorQuery = await IsVendorQueryAsync(playerMessage);
                    long vendorQueryTime = (long)(DateTime.UtcNow - vendorQueryStart).TotalMilliseconds;
                    Console.WriteLine($"[LocalLLM] [TIMING] Vendor query check (full): {vendorQueryTime}ms");
                }
                
                Console.WriteLine($"[LocalLLM] IsVendorQuery('{playerMessage}'): {isVendorQuery}, isFirstConversation: {isFirstConversation}");

                // REACTIVE RAG: For non-greeting, non-vendor queries, search for query-specific lore
                // This ensures NPCs can answer specific questions even if the entry wasn't in the top 5 preloaded entries
                if (!isSimpleGreeting && !isVendorQuery && !isFirstConversation)
                {
                    DateTime reactiveRAGStart = DateTime.UtcNow;
                    try
                    {
                        // Use keyword search (fast, no embedding generation) for reactive RAG
                        // This finds entries like "Blackthorn" when asked about it
                        var reactiveLore = SimpleLoreSystem.Search(playerMessage, maxResults: 3);
                        long reactiveRAGTime = (long)(DateTime.UtcNow - reactiveRAGStart).TotalMilliseconds;
                        
                        if (reactiveLore != null && reactiveLore.Count > 0)
                        {
                            Console.WriteLine($"[LocalLLM] [TIMING] Reactive RAG search: {reactiveRAGTime}ms, found {reactiveLore.Count} entries");
                            
                            // Add query-specific knowledge (don't duplicate entries already in preloaded knowledge)
                            contextBuilder.AppendLine();
                            contextBuilder.AppendLine("RELEVANT KNOWLEDGE FOR THIS QUESTION:");
                            foreach (var entry in reactiveLore)
                            {
                                // Full content for query-specific entries (not truncated)
                                contextBuilder.AppendLine($"- {entry.Title}: {entry.Content}");
                            }
                            contextBuilder.AppendLine();
                        }
                        else
                        {
                            Console.WriteLine($"[LocalLLM] [TIMING] Reactive RAG search: {reactiveRAGTime}ms, no additional entries found");
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"[LocalLLM] Error in reactive RAG search: {ex.Message}");
                        // Continue without reactive RAG - preloaded knowledge should be sufficient
                    }
                }

                // Check cache first (only if conversation history is minimal - don't cache context-dependent queries)
                // Skip semantic search on first conversation to avoid embedding generation delay (~2-5 seconds)
                DateTime cacheStart = DateTime.UtcNow;
                bool useCache = conversationHistory == null || conversationHistory.Count <= 2;
                
                if (useCache)
                {
                    // On first conversation, only do exact match (fast, no embedding generation)
                    // On subsequent conversations, do full semantic search
                    string cachedResponse;
                    if (isFirstConversation)
                    {
                        cachedResponse = LLMResponseCache.GetCachedResponse(playerMessage, npcName, playerName, isVendorQuery);
                        long cacheTime = (long)(DateTime.UtcNow - cacheStart).TotalMilliseconds;
                        Console.WriteLine($"[LocalLLM] [TIMING] Cache lookup (exact match only): {cacheTime}ms");
                    }
                    else
                    {
                        cachedResponse = await LLMResponseCache.GetCachedResponseAsync(playerMessage, npcName, playerName, isVendorQuery, isFirstConversation);
                        long cacheTime = (long)(DateTime.UtcNow - cacheStart).TotalMilliseconds;
                        Console.WriteLine($"[LocalLLM] [TIMING] Cache lookup (full): {cacheTime}ms");
                    }
                    
                    if (cachedResponse != null)
                    {
                        Console.WriteLine($"[LocalLLM] Using cached response (saved LLM call)");
                        return cachedResponse;
                    }
                }

                // Only include vendor commands if this NPC is actually a vendor AND it's a vendor query
                if (isVendor && isVendorQuery)
                {
                    contextBuilder.AppendLine("VENDOR COMMANDS:");
                    contextBuilder.AppendLine("CRITICAL: You are a vendor. When the player wants to BUY or SELL, you MUST add a vendor command at the END of your response.");
                    contextBuilder.AppendLine();
                    contextBuilder.AppendLine("BUY = Player wants to BUY FROM YOU (you sell to them):");
                    contextBuilder.AppendLine("  - 'buy', 'sell me', 'sell me your goods', 'what do you have?', 'show me your wares', 'what's for sale?', 'i want to buy', 'let me buy', 'procure', 'acquire', 'your goods', 'your wares'");
                    contextBuilder.AppendLine("  → Add [VENDOR_BUY] at the END");
                    contextBuilder.AppendLine();
                    contextBuilder.AppendLine("SELL = Player wants to SELL TO YOU (you buy from them):");
                    contextBuilder.AppendLine("  - 'i want to sell', 'will you buy this?', 'can i sell to you?', 'buy from me', 'i have [item] to sell'");
                    contextBuilder.AppendLine("  → Add [VENDOR_SELL] at the END");
                    contextBuilder.AppendLine();
                    contextBuilder.AppendLine($"IMPORTANT: The player just said: \"{playerMessage}\"");
                    contextBuilder.AppendLine("  - If they said 'sell me' or 'your goods' → They want to BUY from you → Use [VENDOR_BUY]");
                    contextBuilder.AppendLine("  - If they said 'I want to sell' or 'will you buy' → They want to SELL to you → Use [VENDOR_SELL]");
                    contextBuilder.AppendLine();
                }

                // Add conversation history
                if (conversationHistory != null && conversationHistory.Count > 0)
                {
                    contextBuilder.AppendLine("Previous conversation:");
                    foreach (var msg in conversationHistory)
                    {
                        string role = msg.IsPlayer ? playerName : npcName;
                        contextBuilder.AppendLine($"{role}: {msg.Message}");
                    }
                    contextBuilder.AppendLine();
                }

                // Add current message
                contextBuilder.AppendLine($"{playerName} just said: \"{playerMessage}\"");
                contextBuilder.AppendLine();
                contextBuilder.AppendLine($"Your response as {npcName} (STOP after your response - do NOT write \"{playerName}:\" or continue the conversation):");

                // Build Ollama request (reduced tokens for faster responses)
                DateTime llmCallStart = DateTime.UtcNow;
                string requestBody = BuildOllamaRequest(contextBuilder.ToString(), 100);
                var content = new StringContent(requestBody, Encoding.UTF8, "application/json");

                Console.WriteLine($"[LocalLLM] [TIMING] Prompt size: {contextBuilder.Length} chars");

                // Make the API call
                HttpResponseMessage response = await client.PostAsync($"{endpoint}/api/generate", content);
                string responseBody = await response.Content.ReadAsStringAsync();
                long llmCallTime = (long)(DateTime.UtcNow - llmCallStart).TotalMilliseconds;
                Console.WriteLine($"[LocalLLM] [TIMING] LLM API call: {llmCallTime}ms");

                if (response.IsSuccessStatusCode)
                {
                    string llmResponse = ExtractOllamaResponse(responseBody);

                    // Strip any hallucinated player dialogue that appears after NPC response
                    llmResponse = StripHallucinatedDialogue(llmResponse, playerName, npcName);
                    
                    // Fix player name variations (e.g., "Runescrary" -> "Runescry")
                    llmResponse = FixPlayerName(llmResponse, playerName);

                    // Validate and fix vendor commands if needed
                    if (isVendor && isVendorQuery)
                    {
                        string lowerResponse = llmResponse.ToLower();
                        string lowerMessage = playerMessage.ToLower();
                        
                        // Determine what the correct command should be
                        bool isSellRequest = lowerMessage.Contains("i want to sell") ||
                                            lowerMessage.Contains("i'd like to sell") ||
                                            lowerMessage.Contains("can i sell") ||
                                            lowerMessage.Contains("will you buy") ||
                                            lowerMessage.Contains("buy from me") ||
                                            lowerMessage.Contains("purchase from me") ||
                                            (lowerMessage.Contains("i have") && (lowerMessage.Contains("to sell") || lowerMessage.Contains("for sale"))) ||
                                            lowerMessage.Contains("take this") ||
                                            (lowerMessage.Contains("buy this") && !lowerMessage.Contains("sell me"));
                        
                        bool isBuyRequest = lowerMessage.Contains("sell me") ||
                                           lowerMessage.Contains("i want to buy") ||
                                           lowerMessage.Contains("i'd like to buy") ||
                                           lowerMessage.Contains("let me buy") ||
                                           lowerMessage.Contains("i need") ||
                                           (lowerMessage.Contains("i want") && !lowerMessage.Contains("to sell")) ||
                                           lowerMessage.Contains("show me") ||
                                           lowerMessage.Contains("what do you have") ||
                                           lowerMessage.Contains("your goods") ||
                                           lowerMessage.Contains("your wares");
                        
                        bool hasBuyCommand = lowerResponse.Contains("[vendor_buy]") || lowerResponse.Contains("(vendor_buy)");
                        bool hasSellCommand = lowerResponse.Contains("[vendor_sell]") || lowerResponse.Contains("(vendor_sell)");
                        
                        // Determine correct command (default to BUY if ambiguous)
                        bool shouldBeBuy = isBuyRequest || (!isSellRequest && !isBuyRequest);
                        bool shouldBeSell = isSellRequest && !isBuyRequest;
                        
                        // Fix incorrect commands
                        if (hasBuyCommand && shouldBeSell)
                        {
                            // Wrong command - replace BUY with SELL
                            llmResponse = System.Text.RegularExpressions.Regex.Replace(llmResponse, @"\[VENDOR_BUY\]|\(VENDOR_BUY\)", "[VENDOR_SELL]", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                        }
                        else if (hasSellCommand && shouldBeBuy)
                        {
                            // Wrong command - replace SELL with BUY
                            llmResponse = System.Text.RegularExpressions.Regex.Replace(llmResponse, @"\[VENDOR_SELL\]|\(VENDOR_SELL\)", "[VENDOR_BUY]", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                        }
                        else if (!hasBuyCommand && !hasSellCommand)
                        {
                            // No command - add correct one
                            if (shouldBeSell)
                            {
                                llmResponse += " [VENDOR_SELL]";
                            }
                            else
                            {
                                llmResponse += " [VENDOR_BUY]";
                            }
                        }
                    }

                    string finalResponse = llmResponse.Trim();

                    // Cache the response (only if conversation history is minimal)
                    if (useCache)
                    {
                        LLMResponseCache.CacheResponse(playerMessage, finalResponse, npcName, playerName, isVendorQuery);
                    }

                    return finalResponse;
                }
                else
                {
                    Console.WriteLine($"[LocalLLM] Error: {response.StatusCode} - {responseBody}");
                    return "...";
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[LocalLLM] Exception: {ex.Message}");
                return "...";
            }
        }

        /// <summary>
        /// Generates a response with conversation history (LEGACY - uses reactive RAG)
        /// </summary>
        public static async Task<string> GetResponseAsync(string npcName, string npcPersonality, List<ConversationMessage> conversationHistory, string playerMessage, string playerName, bool isVendor = false)
        {
            try
            {
                // Build conversation context
                StringBuilder contextBuilder = new StringBuilder();
                
                // Clean personality text - remove example speech to prevent template imitation
                string cleanPersonality = npcPersonality;
                int exampleIndex = npcPersonality.IndexOf("Example speech:");
                if (exampleIndex >= 0)
                {
                    cleanPersonality = npcPersonality.Substring(0, exampleIndex).Trim();
                }
                
                contextBuilder.AppendLine($"You are {npcName}. {cleanPersonality}");
                contextBuilder.AppendLine($"You are talking to {playerName}.");
                contextBuilder.AppendLine();
                contextBuilder.AppendLine("RULES:");
                contextBuilder.AppendLine("1. Keep responses brief (1-2 sentences)");
                contextBuilder.AppendLine("2. ONLY your words, never the player's");
                contextBuilder.AppendLine("3. Use EXACT name '" + playerName + "'");
                contextBuilder.AppendLine("4. No meta-commentary, explanations, or markdown");
                contextBuilder.AppendLine("5. Speak naturally as " + npcName);
                contextBuilder.AppendLine();

                // Skip lore search for vendor transactions (huge performance boost!)
                // On first conversation, use fast keyword-only check to avoid embedding generation
                bool isFirstConversation = conversationHistory == null || conversationHistory.Count == 0;
                bool isVendorQuery = false;
                
                if (isFirstConversation)
                {
                    // Fast keyword-only check (no embeddings) for first conversation
                    string lower = playerMessage.ToLower();
                    isVendorQuery = IsVendorQueryByKeywords(lower);
                }
                else
                {
                    // Full check (including semantic) for subsequent conversations
                    isVendorQuery = await IsVendorQueryAsync(playerMessage);
                }
                
                List<LoreEntry> relevantLore = null;

                Console.WriteLine($"[LocalLLM] IsVendorQuery('{playerMessage}'): {isVendorQuery}");

                // Check cache first (only if conversation history is minimal - don't cache context-dependent queries)
                // Skip semantic search on first conversation to avoid embedding generation delay (~2-5 seconds)
                bool useCache = conversationHistory == null || conversationHistory.Count <= 2;
                
                if (useCache)
                {
                    // On first conversation, only do exact match (fast, no embedding generation)
                    // On subsequent conversations, do full semantic search
                    string cachedResponse;
                    if (isFirstConversation)
                    {
                        cachedResponse = LLMResponseCache.GetCachedResponse(playerMessage, npcName, playerName, isVendorQuery);
                    }
                    else
                    {
                        cachedResponse = await LLMResponseCache.GetCachedResponseAsync(playerMessage, npcName, playerName, isVendorQuery, isFirstConversation);
                    }
                    
                    if (cachedResponse != null)
                    {
                        Console.WriteLine($"[LocalLLM] Using cached response (saved LLM call)");
                        return cachedResponse;
                    }
                }

                if (!isVendorQuery)
                {
                    // Retrieve relevant lore using vector search (with keyword fallback)
                    // Prioritize current message, but include last message for context if relevant
                    string searchQuery = playerMessage;
                    if (conversationHistory != null && conversationHistory.Count > 0)
                    {
                        var playerMessages = conversationHistory.Where(m => m.IsPlayer).ToList();
                        if (playerMessages.Count > 0)
                        {
                            string lastMessage = playerMessages[playerMessages.Count - 1].Message;
                            // Only append context if current message is very short (likely a follow-up)
                            if (playerMessage.Split(' ').Length <= 3 && lastMessage.Split(' ').Length > 2)
                            {
                                searchQuery = lastMessage + " " + playerMessage;
                            }
                        }
                    }

                    relevantLore = await VectorLoreSystem.SearchAsync(searchQuery, maxResults: 3, fastMode: false);
                }
                if (relevantLore != null && relevantLore.Count > 0)
                {
                    contextBuilder.AppendLine("=== WORLD KNOWLEDGE ===");
                    contextBuilder.AppendLine("You have knowledge about:");
                    foreach (var lore in relevantLore)
                    {
                        contextBuilder.AppendLine();
                        contextBuilder.AppendLine($"{lore.Title}:");
                        contextBuilder.AppendLine(lore.Content);
                    }
                    contextBuilder.AppendLine();
                    contextBuilder.AppendLine("Use this knowledge naturally in your responses when relevant.");
                    contextBuilder.AppendLine();
                }

                // Only include vendor commands if this NPC is actually a vendor AND it's a vendor query
                if (isVendor && isVendorQuery)
                {
                    contextBuilder.AppendLine("VENDOR COMMANDS:");
                    contextBuilder.AppendLine("CRITICAL: You are a vendor. When the player wants to BUY or SELL, you MUST add a vendor command at the END of your response.");
                    contextBuilder.AppendLine();
                    contextBuilder.AppendLine("BUY = Player wants to BUY FROM YOU (you sell to them):");
                    contextBuilder.AppendLine("  - 'buy', 'sell me', 'sell me your goods', 'what do you have?', 'show me your wares', 'what's for sale?', 'i want to buy', 'let me buy', 'procure', 'acquire', 'your goods', 'your wares'");
                    contextBuilder.AppendLine("  → Add [VENDOR_BUY] at the END");
                    contextBuilder.AppendLine();
                    contextBuilder.AppendLine("SELL = Player wants to SELL TO YOU (you buy from them):");
                    contextBuilder.AppendLine("  - 'i want to sell', 'will you buy this?', 'can i sell to you?', 'buy from me', 'i have [item] to sell'");
                    contextBuilder.AppendLine("  → Add [VENDOR_SELL] at the END");
                    contextBuilder.AppendLine();
                    contextBuilder.AppendLine($"IMPORTANT: The player just said: \"{playerMessage}\"");
                    contextBuilder.AppendLine("  - If they said 'sell me' or 'your goods' → They want to BUY from you → Use [VENDOR_BUY]");
                    contextBuilder.AppendLine("  - If they said 'I want to sell' or 'will you buy' → They want to SELL to you → Use [VENDOR_SELL]");
                    contextBuilder.AppendLine();
                }

                // Add conversation history
                if (conversationHistory != null && conversationHistory.Count > 0)
                {
                    contextBuilder.AppendLine("Previous conversation:");
                    foreach (var msg in conversationHistory)
                    {
                        string role = msg.IsPlayer ? playerName : npcName;
                        contextBuilder.AppendLine($"{role}: {msg.Message}");
                    }
                    contextBuilder.AppendLine();
                }

                // Add current message
                contextBuilder.AppendLine($"{playerName} just said: \"{playerMessage}\"");
                contextBuilder.AppendLine();
                contextBuilder.AppendLine($"Your response as {npcName} (STOP after your response - do NOT write \"{playerName}:\" or continue the conversation):");

                // Build Ollama request (reduced tokens for faster responses)
                DateTime llmCallStart = DateTime.UtcNow;
                string requestBody = BuildOllamaRequest(contextBuilder.ToString(), 100);
                var content = new StringContent(requestBody, Encoding.UTF8, "application/json");

                Console.WriteLine($"[LocalLLM] [TIMING] Prompt size: {contextBuilder.Length} chars");

                // Make the API call
                HttpResponseMessage response = await client.PostAsync($"{endpoint}/api/generate", content);
                string responseBody = await response.Content.ReadAsStringAsync();
                long llmCallTime = (long)(DateTime.UtcNow - llmCallStart).TotalMilliseconds;
                Console.WriteLine($"[LocalLLM] [TIMING] LLM API call: {llmCallTime}ms");

                if (response.IsSuccessStatusCode)
                {
                    string llmResponse = ExtractOllamaResponse(responseBody);

                    // Strip any hallucinated player dialogue that appears after NPC response
                    llmResponse = StripHallucinatedDialogue(llmResponse, playerName, npcName);
                    
                    // Fix player name variations (e.g., "Runescrary" -> "Runescry")
                    llmResponse = FixPlayerName(llmResponse, playerName);

                    // Validate and fix vendor commands if needed
                    if (isVendor && isVendorQuery)
                    {
                        string lowerResponse = llmResponse.ToLower();
                        string lowerMessage = playerMessage.ToLower();
                        
                        // Determine what the correct command should be
                        bool isSellRequest = lowerMessage.Contains("i want to sell") ||
                                            lowerMessage.Contains("i'd like to sell") ||
                                            lowerMessage.Contains("can i sell") ||
                                            lowerMessage.Contains("will you buy") ||
                                            lowerMessage.Contains("buy from me") ||
                                            lowerMessage.Contains("purchase from me") ||
                                            (lowerMessage.Contains("i have") && (lowerMessage.Contains("to sell") || lowerMessage.Contains("for sale"))) ||
                                            lowerMessage.Contains("take this") ||
                                            (lowerMessage.Contains("buy this") && !lowerMessage.Contains("sell me"));
                        
                        bool isBuyRequest = lowerMessage.Contains("sell me") ||
                                           lowerMessage.Contains("i want to buy") ||
                                           lowerMessage.Contains("i'd like to buy") ||
                                           lowerMessage.Contains("let me buy") ||
                                           lowerMessage.Contains("i need") ||
                                           (lowerMessage.Contains("i want") && !lowerMessage.Contains("to sell")) ||
                                           lowerMessage.Contains("show me") ||
                                           lowerMessage.Contains("what do you have") ||
                                           lowerMessage.Contains("your goods") ||
                                           lowerMessage.Contains("your wares");
                        
                        bool hasBuyCommand = lowerResponse.Contains("[vendor_buy]") || lowerResponse.Contains("(vendor_buy)");
                        bool hasSellCommand = lowerResponse.Contains("[vendor_sell]") || lowerResponse.Contains("(vendor_sell)");
                        
                        // Determine correct command (default to BUY if ambiguous)
                        bool shouldBeBuy = isBuyRequest || (!isSellRequest && !isBuyRequest);
                        bool shouldBeSell = isSellRequest && !isBuyRequest;
                        
                        // Fix incorrect commands
                        if (hasBuyCommand && shouldBeSell)
                        {
                            // Wrong command - replace BUY with SELL
                            llmResponse = System.Text.RegularExpressions.Regex.Replace(llmResponse, @"\[VENDOR_BUY\]|\(VENDOR_BUY\)", "[VENDOR_SELL]", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                        }
                        else if (hasSellCommand && shouldBeBuy)
                        {
                            // Wrong command - replace SELL with BUY
                            llmResponse = System.Text.RegularExpressions.Regex.Replace(llmResponse, @"\[VENDOR_SELL\]|\(VENDOR_SELL\)", "[VENDOR_BUY]", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                        }
                        else if (!hasBuyCommand && !hasSellCommand)
                        {
                            // No command - add correct one
                            if (shouldBeSell)
                            {
                                llmResponse += " [VENDOR_SELL]";
                            }
                            else
                            {
                                llmResponse += " [VENDOR_BUY]";
                            }
                        }
                    }

                    string finalResponse = llmResponse.Trim();

                    // Cache the response (only if conversation history is minimal)
                    if (useCache)
                    {
                        LLMResponseCache.CacheResponse(playerMessage, finalResponse, npcName, playerName, isVendorQuery);
                    }

                    return finalResponse;
                }
                else
                {
                    Console.WriteLine($"[LocalLLM] Error: {response.StatusCode} - {responseBody}");
                    return "...";
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[LocalLLM] Exception: {ex.Message}");
                return "...";
            }
        }

        /// <summary>
        /// Builds Ollama API request JSON
        /// </summary>
        private static string BuildOllamaRequest(string prompt, int maxTokens)
        {
            // Escape the prompt for JSON
            string escapedPrompt = prompt
                .Replace("\\", "\\\\")
                .Replace("\"", "\\\"")
                .Replace("\n", "\\n")
                .Replace("\r", "\\r")
                .Replace("\t", "\\t");

            return $"{{" +
                   $"\"model\":\"{modelName}\"," +
                   $"\"prompt\":\"{escapedPrompt}\"," +
                   $"\"stream\":false," +
                   $"\"options\":{{" +
                   $"\"num_predict\":{maxTokens}," +
                   $"\"temperature\":0.7," +
                   $"\"top_p\":0.9" +
                   $"}}" +
                   $"}}";
        }

        /// <summary>
        /// Extracts response text from Ollama JSON response
        /// </summary>
        private static string ExtractOllamaResponse(string json)
        {
            try
            {
                // Ollama returns: {"model":"...","response":"text here",...}
                int responseIndex = json.IndexOf("\"response\":\"");
                if (responseIndex == -1)
                    return "...";

                int startIndex = responseIndex + 12; // Length of "response":"
                int endIndex = startIndex;
                bool escaped = false;

                // Find the end of the response string
                while (endIndex < json.Length)
                {
                    char c = json[endIndex];

                    if (escaped)
                    {
                        escaped = false;
                    }
                    else if (c == '\\')
                    {
                        escaped = true;
                    }
                    else if (c == '\"')
                    {
                        break;
                    }

                    endIndex++;
                }

                string response = json.Substring(startIndex, endIndex - startIndex);

                // Unescape sequences
                response = response.Replace("\\n", "\n");
                response = response.Replace("\\r", "\r");
                response = response.Replace("\\t", "\t");
                response = response.Replace("\\\"", "\"");
                response = response.Replace("\\\\", "\\");

                return response.Trim();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[LocalLLM] Error parsing response: {ex.Message}");
                return "...";
            }
        }

        /// <summary>
        /// Strip hallucinated player dialogue from LLM response
        /// phi3:mini tends to continue conversations - cut it off
        /// </summary>
        private static string StripHallucinatedDialogue(string response, string playerName, string npcName)
        {
            if (string.IsNullOrEmpty(response))
                return response;

            // Look for patterns like "PlayerName:" or "[VENDOR_OFFER]\nPlayerName:"
            string[] lines = response.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
            List<string> validLines = new List<string>();

            foreach (string line in lines)
            {
                string trimmed = line.Trim();

                // Stop if we hit player dialogue (at start of line)
                if (trimmed.StartsWith(playerName + ":", StringComparison.OrdinalIgnoreCase) ||
                    trimmed.StartsWith("Player:", StringComparison.OrdinalIgnoreCase))
                {
                    break; // Cut off everything after this
                }

                // Also check for embedded conversation format within the line
                int playerDialogIndex = trimmed.IndexOf(playerName + ":", StringComparison.OrdinalIgnoreCase);
                int npcDialogIndex = trimmed.IndexOf(npcName + ":", StringComparison.OrdinalIgnoreCase);
                
                if (playerDialogIndex >= 0)
                {
                    // Cut off at the player dialogue, keep only the part before it
                    trimmed = trimmed.Substring(0, playerDialogIndex).Trim();
                }
                else if (npcDialogIndex > 0) // NPC name appearing mid-line is also bad
                {
                    // Cut off at the NPC dialogue, keep only the part before it
                    trimmed = trimmed.Substring(0, npcDialogIndex).Trim();
                }

                // Also stop if we see obvious continuation markers
                if (trimmed.StartsWith("[VENDOR_OFFER]") || trimmed.StartsWith("(VENDOR_OFFER)"))
                {
                    continue; // Skip this line but keep going
                }

                if (!string.IsNullOrEmpty(trimmed))
                {
                    validLines.Add(trimmed);
                }
            }

            return string.Join("\n", validLines).Trim();
        }

        /// <summary>
        /// Fixes player name variations in LLM responses
        /// Common issues: "Runescry" -> "Runescrary", extra letters, etc.
        /// </summary>
        private static string FixPlayerName(string response, string correctPlayerName)
        {
            if (string.IsNullOrEmpty(response) || string.IsNullOrEmpty(correctPlayerName) || correctPlayerName.Length < 3)
                return response;

            string fixedResponse = response;
            string lowerCorrectName = correctPlayerName.ToLower();
            
            // Find all words in the response that might be name variations
            // Use word boundaries to avoid partial matches
            Regex wordRegex = new Regex(@"\b\w+\b");
            
            fixedResponse = wordRegex.Replace(fixedResponse, (match) =>
            {
                string word = match.Value;
                string lowerWord = word.ToLower();
                
                // Skip if it's already the correct name
                if (lowerWord == lowerCorrectName)
                    return word;
                
                // Check if this word is a variation of the player name
                // Must start with same first 3+ characters and be similar length
                int minPrefix = Math.Min(3, lowerCorrectName.Length);
                if (lowerWord.Length >= minPrefix &&
                    lowerWord.StartsWith(lowerCorrectName.Substring(0, minPrefix)) &&
                    Math.Abs(word.Length - correctPlayerName.Length) <= 4) // Allow up to 4 char difference
                {
                    // Preserve original capitalization pattern
                    if (char.IsUpper(word[0]))
                        return correctPlayerName.Substring(0, 1).ToUpper() + (correctPlayerName.Length > 1 ? correctPlayerName.Substring(1).ToLower() : "");
                    else
                        return correctPlayerName.ToLower();
                }
                
                return word; // Keep original if not a match
            });

            return fixedResponse;
        }

        /// <summary>
        /// Check if local LLM is available
        /// </summary>
        public static async Task<bool> IsAvailableAsync()
        {
            try
            {
                var response = await client.GetAsync($"{endpoint}/api/tags");
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        // Cached vendor query template embeddings for semantic matching (pre-computed at startup)
        private static Dictionary<string, float[]> m_VendorQueryEmbeddings = null;
        private static bool m_VendorEmbeddingsReady = false;
        private static readonly object m_VendorEmbeddingsLock = new object();
        private static readonly string[] VendorQueryTemplates = new[]
        {
            "I want to buy something",
            "show me your wares",
            "what do you have for sale",
            "I need to purchase",
            "can I buy",
            "do you sell",
            "I would like to procure",
            "show your goods",
            "what items do you have",
            "I want to acquire"
        };

        // Cache for message embeddings (LRU-style, common queries)
        private static Dictionary<string, float[]> m_MessageEmbeddingCache = new Dictionary<string, float[]>();
        private static readonly int MaxMessageCacheSize = 50;
        private static readonly TimeSpan MessageCacheTTL = TimeSpan.FromMinutes(10);

        /// <summary>
        /// Detect if a query is a vendor transaction using three-tier approach:
        /// 1. Fast keyword matching (<1ms)
        /// 2. Pre-computed semantic similarity (<50ms, if embeddings ready)
        /// 3. Let LLM handle naturally (fallback)
        /// </summary>
        public static async Task<bool> IsVendorQueryAsync(string message)
        {
            if (string.IsNullOrEmpty(message))
                return false;

            string lower = message.ToLower();

            // Tier 1: Fast keyword check (covers 95%+ of cases, <1ms)
            if (IsVendorQueryByKeywords(lower))
                return true;

            // Skip semantic check for simple greetings/common phrases (no embedding generation needed)
            // These are clearly not vendor queries and don't need expensive semantic matching
            string[] simpleGreetings = { "hi", "hello", "hey", "greetings", "good morning", "good afternoon", "good evening", 
                                         "how are you", "what's up", "howdy", "salutations", "farewell", "goodbye", "bye",
                                         "thanks", "thank you", "who are you", "what is your name", "tell me about yourself" };
            
            if (simpleGreetings.Any(g => lower == g || lower.StartsWith(g + " ") || lower.EndsWith(" " + g) || lower.Contains(" " + g + " ")))
            {
                return false; // Skip semantic check for greetings - clearly not vendor queries
            }

            // Tier 2: Semantic similarity (only if embeddings are ready and message has request intent)
            // This catches semantic variations that keyword matching missed
            // SKIP on first conversation to avoid embedding generation delay
            if (m_VendorEmbeddingsReady && HasRequestIntent(lower))
            {
                bool semanticResult = await IsVendorQueryBySemantics(message);
                if (semanticResult)
                    return true;
            }

            // Tier 3: Let LLM handle it naturally (vendor commands in prompt will catch it)
            // This is the fallback - LLM will include [VENDOR_BUY] if appropriate
            return false;
        }

        /// <summary>
        /// Fast keyword-based vendor query detection (comprehensive synonym list)
        /// </summary>
        private static bool IsVendorQueryByKeywords(string lower)
        {
            // Strong vendor keywords - comprehensive synonym list
            // Purchase verbs
            if (lower.Contains("buy") || lower.Contains("sell") ||
                lower.Contains("purchase") || lower.Contains("trade") ||
                lower.Contains("procure") || lower.Contains("acquire") || 
                lower.Contains("obtain") || lower.Contains("secure") ||
                lower.Contains("get") || lower.Contains("fetch") ||
                lower.Contains("pick up") || lower.Contains("pickup"))
            {
                Console.WriteLine($"[LocalLLM] Keyword match found in purchase verbs for: '{lower}'");
                return true;
            }

            // Price/cost related
            if (lower.Contains("cost") || lower.Contains("price") ||
                lower.Contains("how much") || lower.Contains("gold") ||
                lower.Contains("coins") || lower.Contains("payment") ||
                lower.Contains("afford") || lower.Contains("charge"))
                return true;

            // Merchandise/inventory related
            if (lower.Contains("wares") || lower.Contains("goods") ||
                lower.Contains("merchandise") || lower.Contains("stock") ||
                lower.Contains("inventory") || lower.Contains("items") ||
                lower.Contains("for sale") || lower.Contains("shop") ||
                lower.Contains("store") || lower.Contains("market"))
                return true;

            // Common vendor phrases
            if (lower.Contains("what do you have") || lower.Contains("show me") ||
                lower.Contains("do you sell") || lower.Contains("can i buy") ||
                lower.Contains("what's for sale") || lower.Contains("what is for sale") ||
                lower.Contains("what can i buy") || lower.Contains("what can i get") ||
                lower.Contains("display") || lower.Contains("show your"))
                return true;

            // Common item requests to vendors - expanded patterns
            // Check for request patterns + item types
            bool hasRequestPattern = lower.Contains("i need") || 
                                    lower.Contains("i want") || 
                                    lower.Contains("i would like") ||
                                    lower.Contains("i'd like") ||
                                    lower.Contains("i would want") ||
                                    lower.Contains("get me") ||
                                    lower.Contains("i am here to") ||  // "i am here to procure"
                                    lower.Contains("i'm here to") ||
                                    (lower.Contains("i would") && (lower.Contains("like") || lower.Contains("want") || lower.Contains("need")));
            
            bool hasItemType = lower.Contains("sword") || 
                              lower.Contains("blade") || 
                              lower.Contains("weapon") ||
                              lower.Contains("armor") || 
                              lower.Contains("armour") ||
                              lower.Contains("shield") || 
                              lower.Contains("potion") ||
                              lower.Contains("reagent") || 
                              lower.Contains("item") ||
                              lower.Contains("tool") ||
                              lower.Contains("equipment") ||
                              lower.Contains("gear");

            if (hasRequestPattern && hasItemType)
            {
                Console.WriteLine($"[LocalLLM] Keyword match found: request pattern + item type for: '{lower}'");
                return true;
            }

            // Also check if message contains both a request verb and an item type (more flexible)
            // This catches "procure a weapon", "acquire a sword", etc.
            if ((lower.Contains("procure") || lower.Contains("acquire") || lower.Contains("obtain") || 
                 lower.Contains("get") || lower.Contains("need") || lower.Contains("want")) && hasItemType)
            {
                Console.WriteLine($"[LocalLLM] Keyword match found: purchase verb + item type for: '{lower}'");
                return true;
            }

            return false;
        }

        /// <summary>
        /// Check if message is a simple greeting (hi, hello, etc.)
        /// Used to skip knowledge base for faster responses
        /// </summary>
        private static bool IsSimpleGreeting(string message)
        {
            if (string.IsNullOrEmpty(message))
                return false;

            string lower = message.ToLower().Trim();
            
            // Common greetings that don't need lore context
            string[] greetings = { "hi", "hello", "hey", "greetings", "good morning", "good afternoon", 
                                   "good evening", "how are you", "what's up", "howdy", "salutations",
                                   "farewell", "goodbye", "bye", "thanks", "thank you" };
            
            return greetings.Any(g => lower == g || lower.StartsWith(g + " ") || lower.EndsWith(" " + g) || lower.Contains(" " + g + " "));
        }

        /// <summary>
        /// Check if message has request intent (wants something, asking about items, etc.)
        /// </summary>
        private static bool HasRequestIntent(string lower)
        {
            // Check for request patterns
            if (lower.Contains("i ") && (lower.Contains("want") || lower.Contains("need") || lower.Contains("like") || lower.Contains("would")))
                return true;
            
            // Check for question patterns about items/merchandise
            if (lower.Contains("what") || lower.Contains("show") || lower.Contains("have") || lower.Contains("can i"))
                return true;

            // Check for action verbs that might indicate purchasing
            string[] actionVerbs = { "get", "find", "look", "see", "check", "view" };
            foreach (var verb in actionVerbs)
            {
                if (lower.Contains(verb))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Pre-compute vendor query template embeddings at server startup (non-blocking)
        /// </summary>
        private static async Task PrecomputeVendorQueryEmbeddingsAsync()
        {
            if (!EmbeddingService.IsAvailable())
            {
                Console.WriteLine("[LocalLLM] Embedding service not available, skipping vendor query template pre-computation");
                return;
            }

            try
            {
                var embeddings = new Dictionary<string, float[]>();
                int successCount = 0;
                int failCount = 0;

                foreach (var template in VendorQueryTemplates)
                {
                    try
                    {
                        var embedding = await EmbeddingService.GenerateEmbeddingAsync(template);
                        if (embedding != null)
                        {
                            embeddings[template] = embedding;
                            successCount++;
                        }
                        else
                        {
                            failCount++;
                        }
                    }
                    catch (Exception ex)
                    {
                        failCount++;
                        Console.WriteLine($"[LocalLLM] Error generating embedding for vendor template '{template}': {ex.Message}");
                    }
                }

                lock (m_VendorEmbeddingsLock)
                {
                    m_VendorQueryEmbeddings = embeddings;
                    m_VendorEmbeddingsReady = embeddings.Count > 0;
                }

                if (successCount > 0)
                {
                    Console.WriteLine($"[LocalLLM] Pre-computed {successCount}/{VendorQueryTemplates.Length} vendor query template embeddings");
                }
                if (failCount > 0)
                {
                    Console.WriteLine($"[LocalLLM] WARNING: Failed to generate {failCount} vendor query template embeddings");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[LocalLLM] Error pre-computing vendor query embeddings: {ex.Message}");
            }
        }

        /// <summary>
        /// Use semantic similarity to detect vendor queries (catches synonyms and variations)
        /// Only called if embeddings are pre-computed and ready
        /// </summary>
        private static async Task<bool> IsVendorQueryBySemantics(string message)
        {
            if (!m_VendorEmbeddingsReady || m_VendorQueryEmbeddings == null || m_VendorQueryEmbeddings.Count == 0)
                return false;

            try
            {
                // Check message embedding cache first
                string cacheKey = message.ToLower().Trim();
                float[] messageEmbedding = null;

                lock (m_VendorEmbeddingsLock)
                {
                    // Simple cache lookup (could be improved with LRU)
                    if (m_MessageEmbeddingCache.ContainsKey(cacheKey))
                    {
                        messageEmbedding = m_MessageEmbeddingCache[cacheKey];
                    }
                }

                // Generate embedding if not cached
                if (messageEmbedding == null)
                {
                    messageEmbedding = await EmbeddingService.GenerateEmbeddingAsync(message);
                    if (messageEmbedding == null)
                        return false;

                    // Cache it (simple implementation - could use LRU)
                    lock (m_VendorEmbeddingsLock)
                    {
                        if (m_MessageEmbeddingCache.Count >= MaxMessageCacheSize)
                        {
                            // Clear oldest entries (simple approach)
                            m_MessageEmbeddingCache.Clear();
                        }
                        m_MessageEmbeddingCache[cacheKey] = messageEmbedding;
                    }
                }

                // Check similarity against vendor query templates
                float maxSimilarity = 0f;
                lock (m_VendorEmbeddingsLock)
                {
                    foreach (var kvp in m_VendorQueryEmbeddings)
                    {
                        float similarity = EmbeddingService.CosineSimilarity(messageEmbedding, kvp.Value);
                        if (similarity > maxSimilarity)
                            maxSimilarity = similarity;
                    }
                }

                // Threshold: if similarity > 0.7, it's likely a vendor query
                // This catches semantic variations like "procure" = "buy", "acquire" = "purchase", etc.
                bool isVendorQuery = maxSimilarity > 0.7f;
                
                if (isVendorQuery)
                {
                    Console.WriteLine($"[LocalLLM] Semantic vendor query detected (similarity: {maxSimilarity:F2}): '{message}'");
                }

                return isVendorQuery;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[LocalLLM] Error in semantic vendor query detection: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Check if Ollama is using GPU acceleration
        /// </summary>
        private static async void CheckGPUStatus()
        {
            try
            {
                // Wait a moment for models to potentially load
                await Task.Delay(500);

                // Ollama exposes GPU info in the /api/ps endpoint
                var response = await client.GetAsync($"{endpoint}/api/ps");
                if (response.IsSuccessStatusCode)
                {
                    string body = await response.Content.ReadAsStringAsync();

                    // Check for GPU indicators in the JSON response
                    // Look for patterns like "100% GPU" or GPU layer offloading
                    if (body.Contains("\"gpu_layers\"") ||
                        body.Contains("GPU") && body.Contains("\"processor\""))
                    {
                        Console.WriteLine("[LocalLLM] GPU ACCELERATION: ENABLED");
                        Console.WriteLine("[LocalLLM] Expected response times: 1-2 seconds with phi3:mini");
                    }
                    else
                    {
                        // No models loaded yet, check if CUDA libraries exist
                        Console.WriteLine("[LocalLLM] GPU status will be confirmed when first model loads");
                        Console.WriteLine("[LocalLLM] Run 'ollama ps' to verify GPU acceleration");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[LocalLLM] Could not check GPU status: {ex.Message}");
            }
        }
    }
}
