using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ServUO.Scripts.Services.LLM;
using Server;
using Server.Mobiles;
using System.Diagnostics;
using System.Threading;

namespace Server.Services.LLM
{
    /// <summary>
    /// Unified LLM service that routes requests to OpenAI
    /// </summary>
    public static class UnifiedLLMService
    {
        public enum LLMProvider
        {
            Auto,    // Smart routing (now OpenAI-only)
            OpenAI   // Direct OpenAI
        }

        public enum RequestType
        {
            PlayerConversation,    // High quality needed - use OpenAI
            NPCDecision,           // Fast needed - use Local
            SimpleGreeting,        // Fast needed - use Local
            QuestDialogue,         // High quality needed - use OpenAI
            AutonomousBehavior     // Fast needed - use Local
        }

        private static LLMProvider defaultProvider = LLMProvider.Auto; // Smart routing by default
        private static bool preferLocal = false; // Prefer OpenAI for better performance and natural responses
        
        static UnifiedLLMService()
        {
            LLMLoggingConfig.LogStartup("[UnifiedLLM] Static constructor START");
            
            // Simplified: OpenAI-only architecture
            defaultProvider = LLMProvider.Auto;
            preferLocal = false; // Deprecated - OpenAI only
            
            LLMLoggingConfig.LogStartup($"[UnifiedLLM] Static constructor set defaults: defaultProvider={defaultProvider}");
            
            // Log configuration
            LLMLoggingConfig.LogStartup("[UnifiedLLM] ========================================");
            LLMLoggingConfig.LogStartup("[UnifiedLLM] LLM Provider Configuration:");
            LLMLoggingConfig.LogStartup("[UnifiedLLM]   Provider: OpenAI-only (simplified architecture)");
            LLMLoggingConfig.LogStartup("[UnifiedLLM]   RAG Strategy: Proactive only (no reactive)");
            LLMLoggingConfig.LogStartup("[UnifiedLLM] ========================================");
            LLMLoggingConfig.LogStartup("[UnifiedLLM] Static constructor END");
        }

        /// <summary>
        /// Set the default provider preference
        /// </summary>
        public static void SetDefaultProvider(LLMProvider provider)
        {
            defaultProvider = provider;
            LLMLoggingConfig.LogDebug($"[UnifiedLLM] Default provider set to: {provider}");
        }

        /// <summary>
        /// Set whether to prefer local LLM when available
        /// </summary>
        public static void SetPreferLocal(bool prefer)
        {
            preferLocal = prefer;
            LLMLoggingConfig.LogDebug($"[UnifiedLLM] Prefer local LLM: {prefer}");
        }

        /// <summary>
        /// Get response with automatic provider selection
        /// </summary>
        /// <summary>
        /// Get LLM response with PROACTIVE RAG (pre-loaded knowledge)
        /// </summary>
        public static async Task<string> GetResponseAsync(
            string npcName,
            string npcPersonality,
            List<ConversationMessage> conversationHistory,
            string playerMessage,
            string playerName,
            string preloadedKnowledge,
            bool isVendor = false,
            bool isFirstConversation = false,
            RequestType requestType = RequestType.PlayerConversation)
        {
            var stopwatch = Stopwatch.StartNew();
            bool success = false;
            bool fromCache = false;
            string response = null;

            try
            {
                // Generate cache key
                var cacheKey = QuestCache.GenerateCacheKey(npcName, npcPersonality, conversationHistory, playerMessage, playerName, preloadedKnowledge);

                // Check cache first for non-quest requests
                if (requestType != RequestType.QuestDialogue)
                {
                    response = await QuestCache.GetAsync(cacheKey);
                    if (response != null)
                    {
                        fromCache = true;
                        success = true;
                        return response;
                    }
                }

                // For quest generation, use the queue for load balancing
                if (requestType == RequestType.QuestDialogue)
                {
                    var queueRequest = new QuestGenerationRequest
                    {
                        NpcName = npcName,
                        NpcPersonality = npcPersonality,
                        ConversationHistory = conversationHistory,
                        PlayerMessage = playerMessage,
                        PlayerName = playerName,
                        PreloadedKnowledge = preloadedKnowledge,
                        IsVendor = isVendor,
                        IsFirstConversation = isFirstConversation
                    };

                    response = await QuestGenerationQueue.EnqueueAsync(queueRequest);
                    success = true;

                    // Cache the result for future requests
                    await QuestCache.SetAsync(cacheKey, response, TimeSpan.FromMinutes(15));
                }
                else
                {
                    // For non-quest requests, use direct async call
                    response = await GetResponseDirectAsync(npcName, npcPersonality, conversationHistory, playerMessage, playerName, preloadedKnowledge, isVendor, isFirstConversation, requestType);
                    success = true;

                    // Cache the result
                    await QuestCache.SetAsync(cacheKey, response, TimeSpan.FromMinutes(30));
                }

                return response;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[UnifiedLLMService] Error getting response for {npcName}: {ex.Message}");
                return GetFallbackResponse(requestType);
            }
            finally
            {
                stopwatch.Stop();
                QuestPerformanceMonitor.RecordPerformance(npcName, stopwatch.Elapsed, success, fromCache);
            }
        }

        /// <summary>
        /// Get response directly without queue (for non-quest requests)
        /// </summary>
        private static async Task<string> GetResponseDirectAsync(string npcName, string npcPersonality, List<ConversationMessage> conversationHistory, string playerMessage, string playerName, string preloadedKnowledge, bool isVendor, bool isFirstConversation, RequestType requestType)
        {
            // Select provider based on request type
            var selectedProvider = SelectProvider(requestType);

            try
            {
                switch (selectedProvider)
                {
                    case LLMProvider.OpenAI:
                    case LLMProvider.Auto: // Auto now routes to OpenAI
                        // Use proactive RAG with OpenAI
                        if (requestType == RequestType.QuestDialogue)
                        {
                            // Use retry logic for quest generation
                            return await RetryPolicy.ExecuteAsync(
                                async () => await LLMService.GetResponseAsyncForQuest(npcName, npcPersonality, conversationHistory, playerMessage, playerName, preloadedKnowledge),
                                () => GetFallbackResponse(requestType)
                            );
                        }
                        
                        return await RetryPolicy.ExecuteAsync(
                            async () => await LLMService.GetResponseAsync(npcName, npcPersonality, conversationHistory, playerMessage, playerName, preloadedKnowledge, isVendor, isFirstConversation),
                            () => GetFallbackResponse(requestType)
                        );

                    default:
                        return "Error: Invalid provider selection.";
                }
            }
            catch (Exception ex)
            {
                var errorType = ErrorClassifier.ClassifyError(ex);
                Console.WriteLine($"[UnifiedLLM] {errorType} error with OpenAI in GetResponseAsync (Proactive): {ex.Message}");
                return GetFallbackResponse(requestType);
            }
        }

        /// <summary>
        /// Select which provider to use based on request type
        /// </summary>
        private static LLMProvider SelectProvider(RequestType requestType)
        {
            // All providers route to OpenAI now - simplified architecture
            LLMLoggingConfig.LogDebug($"[UnifiedLLM] Routing to OpenAI (request type: {requestType})");
            return LLMProvider.OpenAI;
        }

        /// <summary>
        /// Get appropriate fallback response based on request type
        /// </summary>
        private static string GetFallbackResponse(RequestType requestType)
        {
            switch (requestType)
            {
                case RequestType.QuestDialogue:
                    return "I'm having trouble accessing the grand archives at the moment. Let me craft you a tale from my memory instead.";
                case RequestType.PlayerConversation:
                    return "I seem to be having trouble connecting to my knowledge sources. Let me respond from my experience.";
                case RequestType.NPCDecision:
                    return "My decision-making process seems clouded. Let me choose the most reasonable option.";
                case RequestType.SimpleGreeting:
                    return "Greetings! I'm having a bit of trouble with my thoughts, but I'm pleased to meet you.";
                case RequestType.AutonomousBehavior:
                    return "I'm experiencing some confusion. Let me continue with my usual routine.";
                default:
                    return "I seem to be having trouble thinking right now. Let me try a different approach.";
            }
        }

        /// <summary>
        /// Choose which provider to use based on request type
        /// </summary>
        private static LLMProvider ChooseProvider(RequestType requestType, LLMProvider providerOverride)
        {
            // All providers route to OpenAI now - simplified architecture
            LLMProvider selectedProvider = providerOverride == LLMProvider.Auto ? LLMProvider.Auto : providerOverride;
            
            LLMLoggingConfig.LogDebug($"[UnifiedLLM] Routing to OpenAI (provider: {selectedProvider})");
            return LLMProvider.OpenAI;
        }

        /// <summary>
        /// Get statistics about LLM usage
        /// </summary>
        public static string GetUsageStats()
        {
            // TODO: Track usage statistics
            return "Usage statistics not yet implemented.";
        }
    }

    /// <summary>
    /// Command to configure LLM providers
    /// </summary>
    public class LLMConfigCommand
    {
        public static void Initialize()
        {
            Server.Commands.CommandSystem.Register("LLMConfig", Server.AccessLevel.Administrator, LLMConfig_OnCommand);
        }

        private static void LLMConfig_OnCommand(Server.Commands.CommandEventArgs e)
        {
            Mobile from = e.Mobile;

            if (e.Length == 0)
            {
                from.SendMessage("=== LLM Configuration (OpenAI Only) ===");
                from.SendMessage("Usage: [LLMConfig <setting> <value>");
                from.SendMessage("");
                from.SendMessage("Settings:");
                from.SendMessage("  provider <auto|openai> - Set default provider");
                from.SendMessage("  test - Test OpenAI provider");
                from.SendMessage("");
                from.SendMessage("Examples:");
                from.SendMessage("  [LLMConfig provider openai");
                from.SendMessage("  [LLMConfig test");
                return;
            }

            string setting = e.GetString(0).ToLower();

            switch (setting)
            {
                case "provider":
                    if (e.Length < 2)
                    {
                        from.SendMessage("Usage: [LLMConfig provider <auto|openai>");
                        return;
                    }

                    string provider = e.GetString(1).ToLower();
                    switch (provider)
                    {
                        case "auto":
                            UnifiedLLMService.SetDefaultProvider(UnifiedLLMService.LLMProvider.Auto);
                            from.SendMessage("Default provider set to: Auto (routes to OpenAI)");
                            break;
                        case "openai":
                            UnifiedLLMService.SetDefaultProvider(UnifiedLLMService.LLMProvider.OpenAI);
                            from.SendMessage("Default provider set to: OpenAI");
                            break;
                        default:
                            from.SendMessage("Invalid provider. Use: auto or openai");
                            break;
                    }
                    break;

                case "test":
                    from.SendMessage("Testing OpenAI provider...");
                    TestProvider(from, "openai");
                    break;

                default:
                    from.SendMessage($"Unknown setting: {setting}");
                    from.SendMessage("Use [LLMConfig for help.");
                    break;
            }
        }

        private static async void TestProvider(Mobile from, string provider)
        {
            try
            {
                string testPrompt = "You are a test NPC.";
                string testMessage = "Hello, please respond with a brief greeting.";
                
                from.SendMessage("Testing OpenAI provider...");
                string response = await LLMService.GetResponseAsync(
                    "Test NPC",
                    testPrompt,
                    new List<ConversationMessage>(),
                    testMessage,
                    from.Name
                );

                from.SendMessage($"Test successful! Response: {response}");
            }
            catch (Exception ex)
            {
                from.SendMessage($"Test failed: {ex.Message}");
            }
        }
    }
}
