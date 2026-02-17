using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Server;

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
            Console.WriteLine("[UnifiedLLM] Static constructor START");
            
            // Simplified: OpenAI-only architecture
            defaultProvider = LLMProvider.Auto;
            preferLocal = false; // Deprecated - OpenAI only
            
            Console.WriteLine($"[UnifiedLLM] Static constructor set defaults: defaultProvider={defaultProvider}");
            
            // Log configuration
            Console.WriteLine($"[UnifiedLLM] ========================================");
            Console.WriteLine($"[UnifiedLLM] LLM Provider Configuration:");
            Console.WriteLine($"[UnifiedLLM]   Provider: OpenAI-only (simplified architecture)");
            Console.WriteLine($"[UnifiedLLM]   RAG Strategy: Proactive only (no reactive)");
            Console.WriteLine($"[UnifiedLLM] ========================================");
            Console.WriteLine("[UnifiedLLM] Static constructor END");
        }

        /// <summary>
        /// Set the default provider preference
        /// </summary>
        public static void SetDefaultProvider(LLMProvider provider)
        {
            defaultProvider = provider;
            Console.WriteLine($"[UnifiedLLM] Default provider set to: {provider}");
        }

        /// <summary>
        /// Set whether to prefer local LLM when available
        /// </summary>
        public static void SetPreferLocal(bool prefer)
        {
            preferLocal = prefer;
            Console.WriteLine($"[UnifiedLLM] Prefer local LLM: {prefer}");
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
            RequestType requestType = RequestType.PlayerConversation,
            LLMProvider providerOverride = LLMProvider.Auto,
            bool isVendor = false,
            bool isFirstConversation = false)
        {
            LLMProvider selectedProvider = ChooseProvider(requestType, providerOverride);

            Console.WriteLine($"[UnifiedLLM] Request type: {requestType}, Using provider: {selectedProvider} (Proactive RAG)");

            try
            {
                switch (selectedProvider)
                {
                    case LLMProvider.OpenAI:
                    case LLMProvider.Auto: // Auto now routes to OpenAI
                        // Use proactive RAG with OpenAI
                        // For quest generation, use higher token limit
                        if (requestType == RequestType.QuestDialogue)
                        {
                            // Quest generation needs more tokens - use 2000 for complete JSON
                            return await LLMService.GetResponseAsyncForQuest(npcName, npcPersonality, conversationHistory, playerMessage, playerName, preloadedKnowledge);
                        }
                        return await LLMService.GetResponseAsync(npcName, npcPersonality, conversationHistory, playerMessage, playerName, preloadedKnowledge, isVendor, isFirstConversation);

                    default:
                        return "Error: Invalid provider selection.";
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[UnifiedLLM] Error with OpenAI in GetResponseAsync (Proactive): {ex.Message}");
                return "I seem to be having trouble thinking right now...";
            }
        }

        /// <summary>
        /// Choose which provider to use based on request type
        /// </summary>
        private static LLMProvider ChooseProvider(RequestType requestType, LLMProvider providerOverride)
        {
            // All providers route to OpenAI now - simplified architecture
            LLMProvider selectedProvider = providerOverride == LLMProvider.Auto ? LLMProvider.Auto : providerOverride;
            
            Console.WriteLine($"[UnifiedLLM] Routing to OpenAI (provider: {selectedProvider})");
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
