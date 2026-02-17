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
        /// Formats memories into a prompt-friendly string with enhanced context
        /// </summary>
        public static string FormatMemoriesForPrompt(List<Memory> memories, int maxMemories = 5)
        {
            if (memories == null || memories.Count == 0)
                return "";

            var sb = new StringBuilder();
            sb.AppendLine("## CONVERSATION HISTORY & EMOTIONAL CONTEXT:");
            sb.AppendLine("CRITICAL: This conversation has emotional weight. Stay consistent with the established tone.");
            sb.AppendLine();
            
            // Sort by importance and recency, take top N
            var topMemories = memories
                .OrderByDescending(m => m.Importance)
                .ThenByDescending(m => m.LastAccessed)
                .Take(maxMemories);

            foreach (var memory in topMemories)
            {
                // Add importance indicator with emotional context
                string importanceIndicator = GetImportanceIndicator(memory.Importance);
                string emotionalWeight = GetEmotionalWeight(memory.Importance);
                
                sb.AppendLine($"- {importanceIndicator} [{memory.Type}] {memory.Content}");
                
                if (memory.Importance >= 7)
                {
                    sb.AppendLine($"  EMOTIONAL WEIGHT: {emotionalWeight} - This is emotionally significant!");
                }
                
                if (memory.Context != null && memory.Context.Count > 0)
                {
                    foreach (var kvp in memory.Context)
                    {
                        sb.AppendLine($"  Context: {kvp.Key} = {kvp.Value}");
                    }
                }
                
                // Add time context for better NPC memory recall
                if (memory.LastAccessed != default(DateTime))
                {
                    string timeAgo = GetTimeAgoDescription(memory.LastAccessed);
                    sb.AppendLine($"  Time: {timeAgo}");
                }
            }

            return sb.ToString();
        }

        /// <summary>
        /// Gets a visual indicator for memory importance
        /// </summary>
        private static string GetImportanceIndicator(float importance)
        {
            if (importance >= 0.8f) return "🔥"; // Very important
            if (importance >= 0.6f) return "⭐"; // Important
            if (importance >= 0.4f) return "📝"; // Moderately important
            if (importance >= 0.2f) return "💭"; // Slightly important
            return "•";       // Basic memory
        }

        /// <summary>
        /// Gets emotional weight description for memory importance
        /// </summary>
        private static string GetEmotionalWeight(int importance)
        {
            switch (importance)
            {
                case 9: return "CRITICAL - Life/death situation or major quest";
                case 8: return "URGENT - Serious matter requiring immediate attention";
                case 7: return "HIGH - Emotionally significant personal matter";
                case 6: return "IMPORTANT - Significant personal information";
                case 5: return "MODERATE - Meaningful personal context";
                case 4: return "NOTABLE - Worth remembering";
                case 3: return "MINOR - Slightly interesting";
                case 2: return "BASIC - Casual conversation";
                case 1: return "TRIVIAL - Minimal importance";
                default: return "UNKNOWN";
            }
        }

        /// <summary>
        /// Gets a natural language description of how long ago something happened
        /// </summary>
        private static string GetTimeAgoDescription(DateTime time)
        {
            var timeSpan = DateTime.UtcNow - time;
            
            if (timeSpan.TotalMinutes < 60)
                return "just recently";
            else if (timeSpan.TotalHours < 24)
                return $"{(int)timeSpan.TotalHours} hours ago";
            else if (timeSpan.TotalDays < 7)
                return $"{(int)timeSpan.TotalDays} days ago";
            else if (timeSpan.TotalDays < 30)
                return $"{(int)timeSpan.TotalDays / 7} weeks ago";
            else if (timeSpan.TotalDays < 365)
                return $"{(int)timeSpan.TotalDays / 30} months ago";
            else
                return "a long time ago";
        }

        /// <summary>
        /// Formats relationship information for prompts with enhanced descriptions
        /// </summary>
        public static string FormatRelationshipForPrompt(Relationship relationship)
        {
            if (relationship == null)
                return "\n## Relationship: Stranger (no prior interactions)";

            var sb = new StringBuilder();
            sb.AppendLine($"\n## Relationship with this player:");
            
            // Add relationship level with descriptive text
            string relationshipDesc = GetRelationshipDescription(relationship.Score);
            sb.AppendLine($"- Level: {relationshipDesc} (Score: {relationship.Score}/100)");
            
            // Add interaction context
            sb.AppendLine($"- Total Interactions: {relationship.InteractionCount}");
            
            // Add relationship type with personality context
            string typeDesc = GetRelationshipTypeDescription(relationship.Type);
            sb.AppendLine($"- Relationship Type: {typeDesc}");
            
            // Add summary if available
            if (!string.IsNullOrEmpty(relationship.Summary))
            {
                sb.AppendLine($"- Summary: {relationship.Summary}");
            }

            // Add time context with natural language
            if (relationship.FirstMet != default(DateTime))
            {
                var daysAgo = (DateTime.UtcNow - relationship.FirstMet).Days;
                string timeDescription = GetRelationshipTimeDescription(daysAgo);
                sb.AppendLine($"- Known them for: {timeDescription}");
            }

            // Add behavioral guidance for LLM
            sb.AppendLine($"- Behavioral Guidance: {GetBehavioralGuidance(relationship.Score)}");

            return sb.ToString();
        }

        /// <summary>
        /// Gets a descriptive relationship level
        /// </summary>
        private static string GetRelationshipDescription(int score)
        {
            if (score >= 80) return "Best Friend (very close bond)";
            if (score >= 60) return "Good Friend (warm, positive relationship)";
            if (score >= 40) return "Friend (friendly and familiar)";
            if (score >= 20) return "Acquaintance (casual acquaintance)";
            if (score >= 0) return "Stranger (new or neutral relationship)";
            if (score >= -20) return "Uneasy (slight tension or distrust)";
            if (score >= -40) return "Disliked (negative relationship)";
            return "Enemy (hostile relationship)";
        }

        /// <summary>
        /// Gets a description of the relationship type
        /// </summary>
        private static string GetRelationshipTypeDescription(RelationshipType type)
        {
            switch (type)
            {
                case RelationshipType.Friend:
                    return "Friendly and supportive";
                case RelationshipType.CloseFriend:
                    return "Very close friend and trusted ally";
                case RelationshipType.Ally:
                    return "Trusted ally and companion";
                case RelationshipType.Rival:
                    return "Competitive but respectful";
                case RelationshipType.Enemy:
                    return "Hostile and adversarial";
                case RelationshipType.Acquaintance:
                    return "Casual acquaintance";
                case RelationshipType.Stranger:
                    return "New or unknown relationship";
                default:
                    return "Undefined relationship";
            }
        }

        /// <summary>
        /// Gets natural language description of relationship duration
        /// </summary>
        private static string GetRelationshipTimeDescription(int daysAgo)
        {
            if (daysAgo < 1) return "just met today";
            if (daysAgo < 7) return "a few days";
            if (daysAgo < 14) return "about a week";
            if (daysAgo < 30) return "a couple of weeks";
            if (daysAgo < 60) return "about a month";
            if (daysAgo < 120) return "a few months";
            if (daysAgo < 365) return "less than a year";
            if (daysAgo < 730) return "about a year";
            return "many years";
        }

        /// <summary>
        /// Gets behavioral guidance for the LLM based on relationship level
        /// </summary>
        private static string GetBehavioralGuidance(int score)
        {
            if (score >= 80) return "Very warm, personal, use their name, share personal thoughts, offer help freely";
            if (score >= 60) return "Warm and friendly, remember past interactions, be helpful and supportive";
            if (score >= 40) return "Friendly and polite, recognize them, be helpful but maintain professional distance";
            if (score >= 20) return "Polite and courteous, recognize them if you have recent interactions";
            if (score >= 0) return "Neutral and polite, treat as new acquaintance until you know them better";
            if (score >= -20) return "Cautious and reserved, be polite but maintain distance";
            if (score >= -40) return "Cold and brief, be dismissive but not openly hostile";
            return "Hostile and confrontational, express displeasure, may refuse interaction";
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
        /// Determines if a message should be saved as a memory based on importance and content
        /// </summary>
        public static bool ShouldSaveMemory(string message, string npcResponse, out int importance)
        {
            importance = 1; // Default low importance

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

            // Low importance - basic interactions (but still save for relationship building)
            if (lowerMessage.Contains("hello") || lowerMessage.Contains("hi") || lowerMessage.Contains("hey") ||
                lowerMessage.Contains("goodbye") || lowerMessage.Contains("bye") || lowerMessage.Contains("thanks") ||
                lowerMessage.Contains("thank you") || lowerMessage.Contains("please") || lowerMessage.Contains("help"))
            {
                importance = 2;
                return true;
            }

            // Very low importance - any conversation (for relationship tracking)
            if (message.Length > 5 && !lowerMessage.Contains("?") && !lowerMessage.Contains("!"))
            {
                importance = 1;
                return true;
            }

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

            // Only process the last 10 messages to avoid re-processing old conversations
            // This prevents duplicate memory extraction on every conversation
            var recentMessages = conversation
                .OrderByDescending(msg => msg.Timestamp)
                .Take(10)
                .Reverse() // Process in chronological order
                .ToList();

            // Analyze recent conversation for extractable memories
            // Look at pairs of player-NPC messages for better context
            for (int i = 0; i < recentMessages.Count; i++)
            {
                var currentMsg = recentMessages[i];
                var nextMsg = i < recentMessages.Count - 1 ? recentMessages[i + 1] : null;
                
                // Create context from current message and next message (if available)
                string context = currentMsg.Message;
                if (nextMsg != null)
                {
                    context += " [Response: " + nextMsg.Message + "]";
                }

                // Check if this should be saved as a memory
                if (ShouldSaveMemory(context, currentMsg.IsPlayer ? "" : currentMsg.Message, out int importance))
                {
                    var memory = new Memory
                    {
                        NpcName = npcName,
                        PlayerName = playerName,
                        Type = DetermineMemoryType(context),
                        Content = currentMsg.Message,
                        Importance = importance,
                        CreatedAt = currentMsg.Timestamp,
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

