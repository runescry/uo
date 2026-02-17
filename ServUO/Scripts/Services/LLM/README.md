# LLM Service

## Overview
The LLM (Large Language Model) service provides AI-powered NPC conversations using OpenAI or local LLM backends. NPCs can engage in dynamic, context-aware dialogue with players, featuring persistent memory, personality systems, and knowledge-based responses.

## Era
- **Expansion:** Custom (All eras)
- **Type:** Custom ServUO feature

## Files
| File/Folder | Description |
|-------------|-------------|
| `Core/LLMNpc.cs` | Base LLM-enabled NPC class |
| `Core/LLMQuester.cs` | Quest-giving LLM NPC |
| `Core/ConversationContext.cs` | Conversation state management |
| `Core/CacheCleanupTimer.cs` | Response cache maintenance |
| `Core/LLMInitializer.cs` | System initialization |
| `Core/ExampleLLMNpcs.cs` | Example NPC implementations |
| `Services/UnifiedLLMService.cs` | Provider routing (OpenAI/Local) |
| `Services/LLMService.cs` | OpenAI integration |
| `Services/LocalLLMService.cs` | Local LLM integration (Ollama) |
| `Services/LLMResponseCache.cs` | Response caching |
| `Services/LLMMemoryService.cs` | Long-term memory storage |
| `Services/LLMMemoryConfig.cs` | Memory configuration |
| `Services/SQLiteMemoryDatabase.cs` | SQLite memory persistence |
| `Services/InMemoryFallbackStore.cs` | Fallback memory store |
| `Services/MemoryHelpers.cs` | Memory utility functions |
| `Services/LLMErrorHandler.cs` | Error handling |
| `Data/NPCPersonalities.cs` | NPC personality definitions |
| `Data/NPCPersonalityMapper.cs` | Personality assignment |
| `Data/NPCPersonalityDatabase.cs` | Personality storage |
| `Data/NPCKnowledgeSystem.cs` | Knowledge base system |
| `Data/NPCLocationDatabase.cs` | Location-aware responses |
| `Data/NPCNames.cs` | NPC name generation |
| `Data/MemoryModels.cs` | Memory data models |
| `Data/LoreEntry.cs` | Lore entry definitions |
| `Data/KnowledgeDomain.cs` | Knowledge domain definitions |
| `Data/SimpleLoreSystem.cs` | Simple lore retrieval |
| `Data/VectorLoreSystem.cs` | Vector-based lore search |
| `Data/EmbeddingService.cs` | Text embedding service |
| `UI/LLMNpcGump.cs` | Conversation interface |
| `Commands/` | GM commands for LLM system |
| `Examples/` | Example quests and NPCs |
| `Tests/` | Automated tests |
| `ILLMConversational.cs` | Conversational interface |
| `LLMConversationPlugin.cs` | Conversation plugin system |
| `LLMConversationHelper.cs` | Conversation utilities |

## Functionality
NPCs powered by LLM can have natural conversations with players.

### LLM Providers
| Provider | Quality | Speed | Cost |
|----------|---------|-------|------|
| OpenAI (gpt-4o-mini) | High | Moderate | API costs |
| Local (Ollama) | Variable | Fast | Free |

### Request Types
| Type | Provider | Use Case |
|------|----------|----------|
| PlayerConversation | OpenAI | High quality needed |
| NPCDecision | Local | Fast response needed |
| SimpleGreeting | Local | Basic interactions |
| QuestDialogue | OpenAI | Story-driven content |
| AutonomousBehavior | Local | Background AI actions |

### Features
- **Dynamic Dialogue** - Context-aware responses
- **Personality System** - Unique NPC personalities
- **Long-term Memory** - NPCs remember past interactions
- **Knowledge Base** - Lore-aware responses
- **Location Awareness** - Context from NPC location
- **Response Caching** - Performance optimization
- **RAG (Retrieval-Augmented Generation)** - Knowledge injection

## How it Works for Players

### Talking to NPCs
1. Approach LLM-enabled NPC
2. Click to open conversation gump
3. Type message in text field
4. Receive AI-generated response
5. Continue conversation naturally

### Memory System
- NPCs remember your name
- Past conversations influence responses
- Relationship builds over time
- Important events are recalled

### Quest Dialogue
1. Quest NPCs use LLM for dialogue
2. Natural language quest descriptions
3. Dynamic hint system
4. Completion acknowledgment

## Configuration
```csharp
// UnifiedLLMService.cs
public enum LLMProvider
{
    Auto,      // Automatically choose based on context
    OpenAI,    // Always use OpenAI (cloud, high quality)
    Local      // Always use local LLM (fast, free)
}

// Default configuration
defaultProvider = LLMProvider.OpenAI;
preferLocal = false;

// LLMMemoryConfig.cs
public static bool MemoryEnabled = true;
public static int MaxMemoryEntries = 100;
public static TimeSpan MemoryRetention = TimeSpan.FromDays(30);
```

## GM Commands
```
[LLMMenu              - Open LLM management menu
[SpawnLLMQuester      - Spawn LLM quest NPC
[SpawnPersonalityNPC  - Spawn NPC with personality
[SpawnNPCGroups       - Spawn group of NPCs
[ClearMemory          - Clear NPC memory
[MemoryTest           - Test memory system
[KnowledgeTest        - Test knowledge retrieval
[LocationTest         - Test location awareness
[CacheCommands        - Manage response cache
```

## Personality System
NPCs can have distinct personalities affecting their responses:
- Speech patterns
- Knowledge domains
- Emotional responses
- Behavioral tendencies

## Knowledge System
### Knowledge Domains
- Britannia lore
- City-specific information
- Trade and crafting
- Combat and magic
- Quest-related knowledge

### RAG (Proactive Knowledge)
```csharp
// Pre-loaded knowledge injection
return await LLMService.GetResponseAsync(
    npcName,
    npcPersonality,
    conversationHistory,
    playerMessage,
    playerName,
    preloadedKnowledge,  // RAG context
    isVendor,
    isFirstConversation
);
```

## Memory Models
```csharp
// MemoryModels.cs
public class MemoryEntry
{
    public string PlayerId { get; set; }
    public string NpcId { get; set; }
    public string Content { get; set; }
    public DateTime Timestamp { get; set; }
    public float Importance { get; set; }
}
```

## FAQ

**Q: Which LLM provider should I use?**
A: OpenAI for quality, Local for cost savings and speed.

**Q: Do NPCs remember between sessions?**
A: Yes, with SQLite or memory persistence enabled.

**Q: How do I set up local LLM?**
A: Install Ollama and pull a model (e.g., mistral-small).

**Q: Is an API key required?**
A: Only for OpenAI; local LLM is free.

**Q: How does knowledge injection work?**
A: Relevant lore is fetched and prepended to prompts.

**Q: Can I customize NPC personalities?**
A: Yes, through the personality database system.

**Q: What happens if LLM is unavailable?**
A: Fallback to default NPC dialogue.

## Local LLM Setup (Ollama)
```bash
# Install Ollama
# Pull a model
ollama pull mistral-small
ollama pull nomic-embed-text  # For embeddings

# Verify
ollama --version
ollama ps
```

## Related Systems
- Quest System (`../Quests/`)
- NPC Speech System
- Town Cryer (`../Town Cryer/`)
- Vendor System
