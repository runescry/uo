using System;
using Server;
using Server.Commands;
using Server.Mobiles;
using Server.Services.LLM;
using Server.Custom.VystiaClasses.Quests.Generation;

namespace Server.Services.LLM.Commands
{
    /// <summary>
    /// Commands for testing and monitoring LLM fallback systems
    /// </summary>
    public static class LLMFallbackCommands
    {
        public static void Initialize()
        {
            CommandSystem.Register("TestLLMFallback", AccessLevel.GameMaster, TestLLMFallback_OnCommand);
            CommandSystem.Register("LLMStatus", AccessLevel.GameMaster, LLMStatus_OnCommand);
            CommandSystem.Register("ResetCircuitBreaker", AccessLevel.GameMaster, ResetCircuitBreaker_OnCommand);
        }

        [Usage("TestLLMFallback")]
        [Description("Test LLM fallback system by simulating API failures")]
        private static void TestLLMFallback_OnCommand(CommandEventArgs e)
        {
            Mobile from = e.Mobile;

            from.SendMessage(68, "[LLM Fallback] Testing LLM fallback system...");

            // Test circuit breaker status
            var circuitState = CircuitBreaker.GetState();
            from.SendMessage(68, $"[LLM Fallback] Circuit Breaker State: {circuitState}");
            from.SendMessage(68, $"[LLM Fallback] {CircuitBreaker.GetStats()}");

            // Test template library
            try
            {
                if (from is PlayerMobile pm)
                {
                    var template = QuestTemplateLibrary.GetRandomTemplate(pm);
                    from.SendMessage(68, $"[LLM Fallback] Template Test: {template.Title}");
                    from.SendMessage(68, $"[LLM Fallback] Theme: {template.Theme}, Tier: {template.Tier}");
                    from.SendMessage(68, $"[LLM Fallback] Waypoints: {template.Waypoints.Length}");
                }
            }
            catch (Exception ex)
            {
                from.SendMessage(38, $"[LLM Fallback] Template test failed: {ex.Message}");
            }

            // Test error classification
            try
            {
                var testError = new System.Net.WebException("Connection timeout");
                var errorType = ErrorClassifier.ClassifyError(testError);
                var friendlyMessage = ErrorClassifier.GetUserFriendlyMessage(errorType);
                from.SendMessage(68, $"[LLM Fallback] Error Classification Test: {errorType}");
                from.SendMessage(68, $"[LLM Fallback] Friendly Message: {friendlyMessage}");
            }
            catch (Exception ex)
            {
                from.SendMessage(38, $"[LLM Fallback] Error classification test failed: {ex.Message}");
            }

            from.SendMessage(68, "[LLM Fallback] Test completed. Check console for detailed logs.");
        }

        [Usage("LLMStatus")]
        [Description("Show current LLM service status and fallback system health")]
        private static void LLMStatus_OnCommand(CommandEventArgs e)
        {
            Mobile from = e.Mobile;

            from.SendMessage(68, "=== LLM Service Status ===");
            
            // Service availability
            bool llmAvailable = LLMService.IsAvailable();
            from.SendMessage(llmAvailable ? 68 : 38, $"[LLM Service] OpenAI API: {(llmAvailable ? "Available" : "Unavailable")}");

            // Circuit breaker status
            var circuitState = CircuitBreaker.GetState();
            from.SendMessage(circuitState == CircuitBreaker.CircuitState.Closed ? 68 : 38, $"[Circuit Breaker] State: {circuitState}");
            from.SendMessage(68, $"[Circuit Breaker] {CircuitBreaker.GetStats()}");

            // Template library status
            var templates = QuestTemplateLibrary.GetAllTemplates();
            from.SendMessage(68, $"[Template Library] {templates.Count} templates available");

            // Recent error patterns (if any)
            from.SendMessage(68, "[Recent Errors] Check console logs for detailed error patterns");
            
            from.SendMessage(68, "=== End Status ===");
        }

        [Usage("ResetCircuitBreaker")]
        [Description("Reset the circuit breaker to Closed state")]
        private static void ResetCircuitBreaker_OnCommand(CommandEventArgs e)
        {
            Mobile from = e.Mobile;

            CircuitBreaker.Reset();
            from.SendMessage(68, "[Circuit Breaker] Reset to Closed state");
            from.SendMessage(68, "[Circuit Breaker] LLM API calls will now be attempted again");
        }
    }
}
