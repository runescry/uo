using System;
using System.Collections.Generic;
using Server.Mobiles;

namespace Server.Services.LLM
{
    /// <summary>
    /// Manages conversation history between players and LLM NPCs
    /// </summary>
    public class ConversationContext
    {
        private static readonly Dictionary<string, PlayerConversation> conversations = new Dictionary<string, PlayerConversation>();
        private static readonly TimeSpan ConversationTimeout = TimeSpan.FromMinutes(10);
        private static readonly int MaxHistoryLength = 10; // Keep last 10 messages to avoid token limits

        /// <summary>
        /// Gets or creates a conversation context for a player-NPC pair
        /// </summary>
        public static List<ConversationMessage> GetHistory(Mobile npc, Mobile player)
        {
            string key = GetKey(npc, player);

            if (conversations.TryGetValue(key, out PlayerConversation conv))
            {
                // Check if conversation has timed out
                if (DateTime.UtcNow - conv.LastActivity > ConversationTimeout)
                {
                    // Conversation expired, start fresh
                    conversations.Remove(key);
                    return new List<ConversationMessage>();
                }

                return conv.Messages;
            }

            return new List<ConversationMessage>();
        }

        /// <summary>
        /// Adds a player message to the conversation history
        /// </summary>
        public static void AddPlayerMessage(Mobile npc, Mobile player, string message)
        {
            string key = GetKey(npc, player);

            if (!conversations.TryGetValue(key, out PlayerConversation conv))
            {
                conv = new PlayerConversation();
                conversations[key] = conv;
            }

            conv.Messages.Add(new ConversationMessage(true, message));
            conv.LastActivity = DateTime.UtcNow;

            // Trim history if it gets too long
            TrimHistory(conv);
        }

        /// <summary>
        /// Adds an NPC response to the conversation history
        /// </summary>
        public static void AddNpcMessage(Mobile npc, Mobile player, string message)
        {
            string key = GetKey(npc, player);

            if (!conversations.TryGetValue(key, out PlayerConversation conv))
            {
                conv = new PlayerConversation();
                conversations[key] = conv;
            }

            conv.Messages.Add(new ConversationMessage(false, message));
            conv.LastActivity = DateTime.UtcNow;

            // Trim history if it gets too long
            TrimHistory(conv);
        }

        /// <summary>
        /// Clears conversation history for a player-NPC pair
        /// </summary>
        public static void ClearHistory(Mobile npc, Mobile player)
        {
            string key = GetKey(npc, player);
            conversations.Remove(key);
        }

        /// <summary>
        /// Clears all expired conversations (called periodically)
        /// </summary>
        public static void CleanupExpired()
        {
            List<string> toRemove = new List<string>();

            foreach (var kvp in conversations)
            {
                if (DateTime.UtcNow - kvp.Value.LastActivity > ConversationTimeout)
                {
                    toRemove.Add(kvp.Key);
                }
            }

            foreach (string key in toRemove)
            {
                conversations.Remove(key);
            }

            if (toRemove.Count > 0)
            {
                Console.WriteLine($"[ConversationContext] Cleaned up {toRemove.Count} expired conversations.");
            }
        }

        /// <summary>
        /// Generates a unique key for a player-NPC conversation
        /// </summary>
        private static string GetKey(Mobile npc, Mobile player)
        {
            return $"{npc.Serial}_{player.Serial}";
        }

        /// <summary>
        /// Keeps conversation history within limits
        /// </summary>
        private static void TrimHistory(PlayerConversation conv)
        {
            while (conv.Messages.Count > MaxHistoryLength)
            {
                conv.Messages.RemoveAt(0);
            }
        }

        /// <summary>
        /// Internal class to track a conversation
        /// </summary>
        private class PlayerConversation
        {
            public List<ConversationMessage> Messages { get; set; }
            public DateTime LastActivity { get; set; }

            public PlayerConversation()
            {
                Messages = new List<ConversationMessage>();
                LastActivity = DateTime.UtcNow;
            }
        }
    }

    /// <summary>
    /// Timer to periodically clean up expired conversations
    /// </summary>
    public class ConversationCleanupTimer : Timer
    {
        public ConversationCleanupTimer() : base(TimeSpan.FromMinutes(5), TimeSpan.FromMinutes(5))
        {
            Priority = TimerPriority.FiveSeconds;
        }

        protected override void OnTick()
        {
            ConversationContext.CleanupExpired();
        }

        public static void Initialize()
        {
            new ConversationCleanupTimer().Start();
        }
    }
}
