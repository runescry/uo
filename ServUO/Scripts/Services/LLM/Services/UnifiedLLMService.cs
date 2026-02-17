using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Server;

namespace Server.Services.LLM
{
    /// <summary>
    /// Unified LLM service that routes requests to OpenAI or Local LLM
    /// </summary>
    public static class UnifiedLLMService
    {
        public enum LLMProvider
        {
            Auto,      // Automatically choose based on context
            OpenAI,    // Always use OpenAI (cloud, high quality)
            Local      // Always use local LLM (fast, free)
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
            
            // Default back to OpenAI for better performance and natural responses
            defaultProvider = LLMProvider.Auto;
            preferLocal = false; // Changed back to false - prefer OpenAI
            
            Console.WriteLine($"[UnifiedLLM] Static constructor set defaults: defaultProvider={defaultProvider}, preferLocal={preferLocal}");
            
            // Log default configuration on first access
            Console.WriteLine($"[UnifiedLLM] ========================================");
            Console.WriteLine($"[UnifiedLLM] LLM Provider Configuration:");
            Console.WriteLine($"[UnifiedLLM]   Default Provider: {defaultProvider} (Auto=smart routing)");
            Console.WriteLine($"[UnifiedLLM]   Prefer Local: {preferLocal} (OpenAI preferred for performance)");
            Console.WriteLine($"[UnifiedLLM]   Local LLM: Available as fallback only");
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
                        // Use proactive RAG with OpenAI
                        // For quest generation, use higher token limit
                        if (requestType == RequestType.QuestDialogue)
                        {
                            // Quest generation needs more tokens - use 2000 for complete JSON
                            return await LLMService.GetResponseAsyncForQuest(npcName, npcPersonality, conversationHistory, playerMessage, playerName, preloadedKnowledge);
                        }
                        return await LLMService.GetResponseAsync(npcName, npcPersonality, conversationHistory, playerMessage, playerName, preloadedKnowledge, isVendor, isFirstConversation);

                    case LLMProvider.Local:
                        // Add timeout for LocalLLM to prevent long waits
                        var localTask = LocalLLMService.GetResponseAsync(npcName, npcPersonality, conversationHistory, playerMessage, playerName, preloadedKnowledge, isVendor, isFirstConversation);
                        var timeoutTask = Task.Delay(TimeSpan.FromSeconds(5)); // 5 second timeout
                        
                        var completedTask = await Task.WhenAny(localTask, timeoutTask);
                        
                        if (completedTask == timeoutTask)
                        {
                            Console.WriteLine("[UnifiedLLM] LocalLLM timeout (5s) - falling back to OpenAI");
                            // Fall back to OpenAI for faster response
                            if (requestType == RequestType.QuestDialogue)
                            {
                                return await LLMService.GetResponseAsyncForQuest(npcName, npcPersonality, conversationHistory, playerMessage, playerName, preloadedKnowledge);
                            }
                            return await LLMService.GetResponseAsync(npcName, npcPersonality, conversationHistory, playerMessage, playerName, preloadedKnowledge, isVendor, isFirstConversation);
                        }
                        
                        return await localTask;

                    default:
                        return "Error: Invalid provider selection.";
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[UnifiedLLM] Error with {selectedProvider} in GetResponseAsync (Proactive): {ex.Message}");

                // Fallback to other provider if one fails (parity with legacy overload)
                try
                {
                    if (selectedProvider == LLMProvider.Local)
                    {
                        Console.WriteLine("[UnifiedLLM] Falling back to OpenAI (Proactive)...");

                        if (requestType == RequestType.QuestDialogue)
                        {
                            return await LLMService.GetResponseAsyncForQuest(npcName, npcPersonality, conversationHistory, playerMessage, playerName, preloadedKnowledge);
                        }

                        return await LLMService.GetResponseAsync(npcName, npcPersonality, conversationHistory, playerMessage, playerName, preloadedKnowledge, isVendor, isFirstConversation);
                    }

                    Console.WriteLine("[UnifiedLLM] Falling back to Local LLM (Proactive)...");
                    return await LocalLLMService.GetResponseAsync(npcName, npcPersonality, conversationHistory, playerMessage, playerName, preloadedKnowledge, isVendor, isFirstConversation);
                }
                catch (Exception fallbackEx)
                {
                    Console.WriteLine($"[UnifiedLLM] Fallback failed in GetResponseAsync (Proactive): {fallbackEx.Message}");
                    return "I seem to be having trouble thinking right now...";
                }
            }
        }

        /// <summary>
        /// Get LLM response with REACTIVE RAG (legacy - per-query searches)
        /// </summary>
        public static async Task<string> GetResponseAsync(
            string npcName,
            string npcPersonality,
            List<ConversationMessage> conversationHistory,
            string playerMessage,
            string playerName,
            RequestType requestType = RequestType.PlayerConversation,
            LLMProvider providerOverride = LLMProvider.Auto,
            bool isVendor = false)
        {
            LLMProvider selectedProvider = ChooseProvider(requestType, providerOverride);

            Console.WriteLine($"[UnifiedLLM] Request type: {requestType}, Using provider: {selectedProvider}");

            try
            {
                switch (selectedProvider)
                {
                    case LLMProvider.OpenAI:
                        return await LLMService.GetResponseAsync(npcName, npcPersonality, conversationHistory, playerMessage, playerName, isVendor);

                    case LLMProvider.Local:
                        return await LocalLLMService.GetResponseAsync(npcName, npcPersonality, conversationHistory, playerMessage, playerName, isVendor);

                    default:
                        return "Error: Invalid provider selection.";
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[UnifiedLLM] Error with {selectedProvider}: {ex.Message}");

                // Fallback to other provider if one fails
                if (selectedProvider == LLMProvider.Local)
                {
                    Console.WriteLine("[UnifiedLLM] Falling back to OpenAI...");
                    return await LLMService.GetResponseAsync(npcName, npcPersonality, conversationHistory, playerMessage, playerName, isVendor);
                }
                else
                {
                    Console.WriteLine("[UnifiedLLM] Falling back to Local LLM...");
                    return await LocalLLMService.GetResponseAsync(npcName, npcPersonality, conversationHistory, playerMessage, playerName, isVendor);
                }
            }
        }

        /// <summary>
        /// Choose which provider to use based on request type
        /// </summary>
        private static LLMProvider ChooseProvider(RequestType requestType, LLMProvider providerOverride)
        {
            // Explicit override
            if (providerOverride != LLMProvider.Auto)
            {
                Console.WriteLine($"[UnifiedLLM] Using explicit provider override: {providerOverride}");
                return providerOverride;
            }

            // Use default if not auto (defaultProvider is OpenAI)
            if (defaultProvider != LLMProvider.Auto)
            {
                Console.WriteLine($"[UnifiedLLM] Using default provider: {defaultProvider} (preferLocal={preferLocal})");
                return defaultProvider;
            }

            // Smart routing based on request type (only if defaultProvider is Auto)
            LLMProvider selected;
            switch (requestType)
            {
                case RequestType.PlayerConversation:
                    // Player conversations - use OpenAI for quality (unless preferLocal is true)
                    selected = preferLocal ? LLMProvider.Local : LLMProvider.OpenAI;
                    Console.WriteLine($"[UnifiedLLM] PlayerConversation routing: preferLocal={preferLocal}, selected={selected}");
                    return selected;

                case RequestType.QuestDialogue:
                    // Important dialogue - use OpenAI for best quality
                    Console.WriteLine($"[UnifiedLLM] QuestDialogue routing: Using OpenAI");
                    return LLMProvider.OpenAI;

                case RequestType.NPCDecision:
                case RequestType.SimpleGreeting:
                case RequestType.AutonomousBehavior:
                    // Fast decisions - always use local
                    Console.WriteLine($"[UnifiedLLM] {requestType} routing: Using Local for speed");
                    return LLMProvider.Local;

                default:
                    selected = preferLocal ? LLMProvider.Local : LLMProvider.OpenAI;
                    Console.WriteLine($"[UnifiedLLM] Default routing for {requestType}: preferLocal={preferLocal}, selected={selected}");
                    return selected;
            }
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
                from.SendMessage("=== LLM Configuration ===");
                from.SendMessage("Usage: [LLMConfig <setting> <value>");
                from.SendMessage("");
                from.SendMessage("Settings:");
                from.SendMessage("  provider <auto|openai|local> - Set default provider");
                from.SendMessage("  preferlocal <true|false> - Prefer local LLM when auto");
                from.SendMessage("  test <openai|local> - Test a provider");
                from.SendMessage("");
                from.SendMessage("Examples:");
                from.SendMessage("  [LLMConfig provider local");
                from.SendMessage("  [LLMConfig preferlocal true");
                from.SendMessage("  [LLMConfig test local");
                return;
            }

            string setting = e.GetString(0).ToLower();

            switch (setting)
            {
                case "provider":
                    if (e.Length < 2)
                    {
                        from.SendMessage("Usage: [LLMConfig provider <auto|openai|local>");
                        return;
                    }

                    string provider = e.GetString(1).ToLower();
                    switch (provider)
                    {
                        case "auto":
                            UnifiedLLMService.SetDefaultProvider(UnifiedLLMService.LLMProvider.Auto);
                            from.SendMessage("Default provider set to: Auto (smart routing)");
                            break;
                        case "openai":
                            UnifiedLLMService.SetDefaultProvider(UnifiedLLMService.LLMProvider.OpenAI);
                            from.SendMessage("Default provider set to: OpenAI");
                            break;
                        case "local":
                            UnifiedLLMService.SetDefaultProvider(UnifiedLLMService.LLMProvider.Local);
                            from.SendMessage("Default provider set to: Local LLM");
                            break;
                        default:
                            from.SendMessage("Invalid provider. Use: auto, openai, or local");
                            break;
                    }
                    break;

                case "preferlocal":
                    if (e.Length < 2)
                    {
                        from.SendMessage("Usage: [LLMConfig preferlocal <true|false>");
                        return;
                    }

                    string preferStr = e.GetString(1).ToLower();
                    bool prefer = preferStr == "true" || preferStr == "1" || preferStr == "yes";
                    UnifiedLLMService.SetPreferLocal(prefer);
                    from.SendMessage($"Prefer local LLM set to: {prefer}");
                    break;

                case "test":
                    if (e.Length < 2)
                    {
                        from.SendMessage("Usage: [LLMConfig test <openai|local>");
                        return;
                    }

                    string testProvider = e.GetString(1).ToLower();
                    from.SendMessage($"Testing {testProvider} provider...");
                    TestProvider(from, testProvider);
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
                string response;

                if (provider == "openai")
                {
                    response = await LLMService.GetResponseAsync(
                        "Test NPC",
                        testPrompt,
                        new List<ConversationMessage>(),
                        testMessage,
                        from.Name
                    );
                }
                else if (provider == "local")
                {
                    response = await LocalLLMService.GetResponseAsync(
                        "Test NPC",
                        testPrompt,
                        new List<ConversationMessage>(),
                        testMessage,
                        from.Name
                    );
                }
                else
                {
                    from.SendMessage("Invalid provider. Use: openai or local");
                    return;
                }

                from.SendMessage($"Test successful! Response: {response}");
            }
            catch (Exception ex)
            {
                from.SendMessage($"Test failed: {ex.Message}");
            }
        }
    }
}
