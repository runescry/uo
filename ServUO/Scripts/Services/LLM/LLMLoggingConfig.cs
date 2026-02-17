using System;

namespace ServUO.Scripts.Services.LLM
{
    /// <summary>
    /// Configuration for LLM logging verbosity levels
    /// </summary>
    public static class LLMLoggingConfig
    {
        // Logging level categories
        public static bool EnableStartupLogs = true;
        public static bool EnableTimingLogs = false;  // Most verbose - turn off by default
        public static bool EnableMemoryLogs = true;   // Important for debugging memory system
        public static bool EnableConversationLogs = false;  // Very verbose
        public static bool EnableErrorLogs = true;    // Always keep error logs
        public static bool EnableDebugLogs = false;   // Development only
        
        // Specific component logging
        public static bool EnableUnifiedLLMLogs = false;        // Provider routing (verbose)
        public static bool EnableLLMServiceLogs = false;         // API calls (verbose)
        public static bool EnableLLMConversationHelperLogs = false;  // Reduce to essential only
        public static bool EnableMemoryDatabaseLogs = false;    // Database operations (verbose)
        public static bool EnableNPCPersonalitiesLogs = false;   // Personality system (verbose)
        public static bool EnableVectorLoreSystemLogs = false;   // Knowledge base search (verbose)
        
        /// <summary>
        /// Logs a message if the specified category is enabled
        /// </summary>
        public static void Log(string category, string message, bool force = false)
        {
            if (force || IsCategoryEnabled(category))
            {
                Console.WriteLine(message);
            }
        }
        
        /// <summary>
        /// Checks if a logging category is enabled
        /// </summary>
        private static bool IsCategoryEnabled(string category)
        {
            switch (category)
            {
                case "STARTUP": return EnableStartupLogs;
                case "TIMING": return EnableTimingLogs;
                case "MEMORY": return EnableMemoryLogs;
                case "CONVERSATION": return EnableConversationLogs;
                case "ERROR": return EnableErrorLogs;
                case "DEBUG": return EnableDebugLogs;
                case "UNIFIED_LLM": return EnableUnifiedLLMLogs;
                case "LLM_SERVICE": return EnableLLMServiceLogs;
                case "CONVERSATION_HELPER": return EnableLLMConversationHelperLogs;
                case "MEMORY_DATABASE": return EnableMemoryDatabaseLogs;
                case "NPC_PERSONALITIES": return EnableNPCPersonalitiesLogs;
                case "VECTOR_LORE": return EnableVectorLoreSystemLogs;
                default: return false;
            }
        }
        
        /// <summary>
        /// Quick log methods for common categories
        /// </summary>
        public static void LogStartup(string message) => Log("STARTUP", message);
        public static void LogTiming(string message) => Log("TIMING", message);
        public static void LogMemory(string message) => Log("MEMORY", message);
        public static void LogConversation(string message) => Log("CONVERSATION", message);
        public static void LogError(string message) => Log("ERROR", message, true); // Always log errors
        public static void LogDebug(string message) => Log("DEBUG", message);
        public static void LogUnifiedLLM(string message) => Log("UNIFIED_LLM", message);
        public static void LogLLMService(string message) => Log("LLM_SERVICE", message);
        public static void LogConversationHelper(string message) => Log("CONVERSATION_HELPER", message);
        public static void LogMemoryDatabase(string message) => Log("MEMORY_DATABASE", message);
        public static void LogNPCPersonalities(string message) => Log("NPC_PERSONALITIES", message);
        public static void LogVectorLore(string message) => Log("VECTOR_LORE", message);
    }
}
