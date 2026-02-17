using Server;
using Server.Mobiles;

namespace Server.Services.LLM
{
    /// <summary>
    /// Interface for NPCs that support LLM-powered conversation
    /// </summary>
    public interface ILLMConversational
    {
        /// <summary>
        /// Whether this NPC has LLM conversation enabled
        /// </summary>
        bool LLMConversationEnabled { get; set; }

        /// <summary>
        /// The personality type for this NPC
        /// </summary>
        NPCPersonalities.PersonalityType PersonalityType { get; set; }

        /// <summary>
        /// The speech pattern for this NPC
        /// </summary>
        NPCPersonalities.SpeechPattern SpeechPattern { get; set; }

        /// <summary>
        /// Hearing range for this NPC (how far they can hear player speech)
        /// </summary>
        int HearingRange { get; set; }

        /// <summary>
        /// Determines if this NPC should handle the conversation
        /// </summary>
        bool ShouldHandleConversation(SpeechEventArgs e);

        /// <summary>
        /// Handles the conversation using LLM
        /// </summary>
        void HandleConversation(SpeechEventArgs e);
    }
}

