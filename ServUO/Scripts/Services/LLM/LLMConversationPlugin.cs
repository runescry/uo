using System;

namespace Server.Services.LLM
{
    /// <summary>
    /// Global plugin configuration for LLM conversation integration
    /// Allows easy enable/disable of LLM conversation for all NPCs
    /// </summary>
    public static class LLMConversationPlugin
    {
        private static bool m_Enabled = true; // Default to enabled

        /// <summary>
        /// Global enable/disable switch for LLM conversation plugin
        /// When disabled, no NPCs will use LLM conversation regardless of individual settings
        /// </summary>
        [CommandProperty(AccessLevel.Administrator)]
        public static bool Enabled
        {
            get { return m_Enabled; }
            set
            {
                m_Enabled = value;
                Console.WriteLine($"[LLMConversationPlugin] Plugin {(value ? "ENABLED" : "DISABLED")}");
            }
        }

        /// <summary>
        /// Check if the plugin is enabled and should process conversations
        /// </summary>
        public static bool IsEnabled()
        {
            return m_Enabled;
        }
    }
}

