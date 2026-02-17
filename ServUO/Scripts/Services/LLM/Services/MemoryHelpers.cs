using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Server.Services.LLM;

namespace Server.Services.LLM
{
    /// <summary>
    /// Helper methods for working with memories in LLM prompts
    /// </summary>
    public static class MemoryHelpers
    {
        /// <summary>
        /// Formats memories into a prompt-friendly string
        /// </summary>
        public static string FormatMemoriesForPrompt(List<Memory> memories, int maxMemories = 5)
        {
            if (memories == null || memories.Count == 0)
                return "";

            var sb = new StringBuilder();
            sb.AppendLine("\n## Memories about this player:");
            
            // Sort by importance and recency, take top N
            var topMemories = memories
                .OrderByDescending(m => m.Importance)
                .ThenByDescending(m => m.LastAccessed)
                .Take(maxMemories);

            foreach (var memory in topMemories)
            {
                sb.AppendLine($"- [{memory.Type}] {memory.Content}");
                if (memory.Context != null && memory.Context.Count > 0)
                {
                    foreach (var kvp in memory.Context)
                    {
                        sb.AppendLine($"  Context: {kvp.Key} = {kvp.Value}");
                    }
                }
            }

            return sb.ToString();
        }

        /// <summary>
        /// Formats relationship information for prompts
        /// </summary>
        public static string FormatRelationshipForPrompt(Relationship relationship)
        {
            if (relationship == null)
                return "\n## Relationship: Stranger (no prior interactions)";

            var sb = new StringBuilder();
            sb.AppendLine($"\n## Relationship with this player:");
            sb.AppendLine($"- Type: {relationship.Type}");
            sb.AppendLine($"- Score: {relationship.Score}/100");
            sb.AppendLine($"- Interactions: {relationship.InteractionCount}");
            
            if (!string.IsNullOrEmpty(relationship.Summary))
            {
                sb.AppendLine($"- Summary: {relationship.Summary}");
            }

            if (relationship.FirstMet != default(DateTime))
            {
                var daysAgo = (DateTime.UtcNow - relationship.FirstMet).Days;
                // Use natural time descriptions instead of specific numbers
                string timeDescription;
                if (daysAgo < 30)
                    timeDescription = "a short time ago";
                else if (daysAgo < 60)
                    timeDescription = "many weeks ago";
                else
                    timeDescription = "many months ago";
                
                sb.AppendLine($"- First met: {timeDescription}");
            }

            return sb.ToString();
        }

        /// <summary>
        /// Formats conversation history for prompts
        /// </summary>
        public static string FormatConversationHistoryForPrompt(List<PersistentConversationMessage> messages, int maxMessages = 5)
        {
            if (messages == null || messages.Count == 0)
                return "";

            var sb = new StringBuilder();
            sb.AppendLine("\n## Recent conversation history:");

            // Take most recent messages
            var recentMessages = messages
                .OrderByDescending(m => m.Timestamp)
                .Take(maxMessages)
                .OrderBy(m => m.Timestamp); // Re-order chronologically

            foreach (var msg in recentMessages)
            {
                var speaker = msg.IsPlayerMessage ? "Player" : "You";
                sb.AppendLine($"{speaker}: {msg.Message}");
            }

            return sb.ToString();
        }

        /// <summary>
        /// Determines if a memory should be saved based on importance and content
        /// </summary>
        public static bool ShouldSaveMemory(string message, string npcResponse, out int importance)
        {
            importance = 1; // Default low importance

            // Check for important keywords/phrases
            var lowerMessage = message.ToLower();
            var lowerResponse = npcResponse.ToLower();

            // Highest importance - relationship statements
            if (lowerMessage.Contains("best friend") || lowerMessage.Contains("my best friend") ||
                lowerMessage.Contains("you are my friend") || lowerMessage.Contains("we are friends") ||
                lowerMessage.Contains("close friend") || lowerMessage.Contains("good friend"))
            {
                importance = 10;
                return true;
            }

            if (lowerMessage.Contains("enemy") || lowerMessage.Contains("hate you") || lowerMessage.Contains("don't like you"))
            {
                importance = 9;
                return true;
            }

            // High importance indicators
            if (lowerMessage.Contains("remember") || lowerMessage.Contains("don't forget"))
            {
                importance = 9;
                return true;
            }

            if (lowerMessage.Contains("my name") || lowerMessage.Contains("i am") || lowerMessage.Contains("i'm"))
            {
                importance = 8;
                return true;
            }

            if (lowerMessage.Contains("quest") || lowerMessage.Contains("mission") || lowerMessage.Contains("task"))
            {
                importance = 7;
                return true;
            }

            // Relationship indicators (medium-high importance)
            if (lowerMessage.Contains("friend") || lowerMessage.Contains("ally") || lowerMessage.Contains("trust"))
            {
                importance = 7;
                return true;
            }

            // Medium importance
            if (lowerMessage.Contains("like") || lowerMessage.Contains("prefer") || lowerMessage.Contains("favorite"))
            {
                importance = 5;
                return true;
            }

            if (lowerMessage.Contains("hate") || lowerMessage.Contains("dislike"))
            {
                importance = 6;
                return true;
            }

            // Low importance - general conversation
            if (message.Length > 20 && !lowerMessage.Contains("hello") && !lowerMessage.Contains("hi"))
            {
                importance = 2;
                return true;
            }

            // Don't save very short or greeting messages
            return false;
        }

        /// <summary>
        /// Extracts key information from a conversation to create memories
        /// Only processes the most recent messages to avoid duplicates
        /// </summary>
        public static List<Memory> ExtractMemoriesFromConversation(string npcName, string playerName, List<ConversationMessage> conversation)
        {
            var memories = new List<Memory>();

            if (conversation == null || conversation.Count == 0)
                return memories;

            // Only process the last 10 player messages to avoid re-processing old conversations
            // This prevents duplicate memory extraction on every conversation
            var recentPlayerMessages = conversation
                .Where(msg => msg.IsPlayer)
                .OrderByDescending(msg => msg.Timestamp)
                .Take(10)
                .Reverse() // Process in chronological order
                .ToList();

            // Analyze recent conversation for extractable memories
            foreach (var msg in recentPlayerMessages)
            {
                if (ShouldSaveMemory(msg.Message, "", out int importance))
                {
                    var memory = new Memory
                    {
                        NpcName = npcName,
                        PlayerName = playerName,
                        Type = DetermineMemoryType(msg.Message),
                        Content = msg.Message,
                        Importance = importance,
                        CreatedAt = msg.Timestamp,
                        LastAccessed = DateTime.UtcNow
                    };

                    memories.Add(memory);
                }
            }

            return memories;
        }

        /// <summary>
        /// Determines memory type from message content
        /// </summary>
        private static MemoryType DetermineMemoryType(string message)
        {
            var lower = message.ToLower();

            if (lower.Contains("name") || lower.Contains("i am") || lower.Contains("i'm"))
                return MemoryType.Fact;

            if (lower.Contains("like") || lower.Contains("prefer") || lower.Contains("favorite") || lower.Contains("hate"))
                return MemoryType.Preference;

            if (lower.Contains("quest") || lower.Contains("mission") || lower.Contains("task"))
                return MemoryType.Event;

            if (lower.Contains("friend") || lower.Contains("enemy") || lower.Contains("ally"))
                return MemoryType.Relationship;

            return MemoryType.Conversation;
        }
    }
}

