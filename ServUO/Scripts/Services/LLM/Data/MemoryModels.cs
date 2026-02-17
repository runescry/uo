using System;
using System.Collections.Generic;

namespace Server.Services.LLM
{
    /// <summary>
    /// Represents a memory that an NPC has about a player
    /// </summary>
    public class Memory
    {
        public int Id { get; set; }
        public int NpcSerial { get; set; } // Unique NPC identifier (Mobile.Serial)
        public string NpcName { get; set; } // For display/search purposes
        public string PlayerName { get; set; }
        public MemoryType Type { get; set; }
        public string Content { get; set; }
        public int Importance { get; set; } // 1-10
        public Dictionary<string, object> Context { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime LastAccessed { get; set; }
        public DateTime? ExpiresAt { get; set; }

        public Memory()
        {
            Context = new Dictionary<string, object>();
            CreatedAt = DateTime.UtcNow;
            LastAccessed = DateTime.UtcNow;
        }
    }

    /// <summary>
    /// Types of memories NPCs can have
    /// </summary>
    public enum MemoryType
    {
        Conversation,
        Event,
        Fact,
        Preference,
        Relationship
    }

    /// <summary>
    /// Extended conversation message for database storage
    /// (ConversationMessage class already exists in LLMService.cs for in-memory use)
    /// </summary>
    public class PersistentConversationMessage
    {
        public int Id { get; set; }
        public string NpcName { get; set; }
        public string PlayerName { get; set; }
        public string SessionId { get; set; }
        public string Message { get; set; }
        public bool IsPlayerMessage { get; set; }
        public DateTime Timestamp { get; set; }
        public Dictionary<string, object> Metadata { get; set; }

        public PersistentConversationMessage()
        {
            Metadata = new Dictionary<string, object>();
            Timestamp = DateTime.UtcNow;
        }

        /// <summary>
        /// Converts from in-memory ConversationMessage to persistent format
        /// </summary>
        public static PersistentConversationMessage FromConversationMessage(ConversationMessage msg, string npcName, string playerName, string sessionId)
        {
            return new PersistentConversationMessage
            {
                NpcName = npcName,
                PlayerName = playerName,
                SessionId = sessionId,
                Message = msg.Message,
                IsPlayerMessage = msg.IsPlayer,
                Timestamp = msg.Timestamp
            };
        }

        /// <summary>
        /// Converts from persistent format back to in-memory ConversationMessage
        /// </summary>
        public ConversationMessage ToConversationMessage()
        {
            return new ConversationMessage(IsPlayerMessage, Message)
            {
                Timestamp = Timestamp
            };
        }
    }

    /// <summary>
    /// Represents the relationship between an NPC and a player
    /// </summary>
    public class Relationship
    {
        public int Id { get; set; }
        public int NpcSerial { get; set; } // Unique NPC identifier (Mobile.Serial)
        public string NpcName { get; set; } // For display/search purposes
        public string PlayerName { get; set; }
        public RelationshipType Type { get; set; }
        public int Score { get; set; } // -100 to +100
        public string Summary { get; set; }
        public DateTime FirstMet { get; set; }
        public DateTime LastInteraction { get; set; }
        public int InteractionCount { get; set; }
        public Dictionary<string, object> Metadata { get; set; }

        public Relationship()
        {
            Type = RelationshipType.Stranger;
            Score = 0;
            Metadata = new Dictionary<string, object>();
            FirstMet = DateTime.UtcNow;
            LastInteraction = DateTime.UtcNow;
            InteractionCount = 0;
        }
    }

    /// <summary>
    /// Types of relationships
    /// </summary>
    public enum RelationshipType
    {
        Stranger,
        Acquaintance,
        Friend,
        CloseFriend,
        Ally,
        Enemy,
        Rival
    }
}

