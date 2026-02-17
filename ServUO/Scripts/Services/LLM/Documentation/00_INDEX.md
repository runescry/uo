# LLM NPC System - Documentation Index

**Current Version**: 1.6.0 (Production Ready)
**Last Updated**: 2025-11-20

---

## Quick Navigation

📖 **[README.md](README.md)** - Complete feature overview and setup guide
⚡ **[QUICKSTART.txt](QUICKSTART.txt)** - Get started in 5 minutes

---

## Project Structure

```
Scripts/Services/LLM/
├── Core/                           # Core NPC implementations
│   ├── LLMNpc.cs                  # Main conversational NPC (1,445 lines)
│   ├── LLMQuester.cs              # Quest-giving NPC (454 lines)
│   ├── ConversationContext.cs     # In-memory conversation history
│   ├── LLMInitializer.cs          # System initialization
│   ├── CacheCleanupTimer.cs       # Automatic cache cleanup
│   └── ExampleLLMNpcs.cs          # Example NPC spawners
│
├── Services/                       # LLM API integrations
│   ├── UnifiedLLMService.cs       # Smart routing (OpenAI + Ollama)
│   ├── LLMService.cs              # OpenAI GPT-4o-mini integration
│   ├── LocalLLMService.cs         # Ollama local LLM integration
│   ├── LLMMemoryService.cs        # Memory coordinator (SQLite)
│   ├── SQLiteMemoryDatabase.cs    # Persistent memory storage (file-based)
│   ├── InMemoryFallbackStore.cs   # Emergency fallback storage
│   ├── LLMMemoryConfig.cs         # Memory system configuration
│   ├── LLMResponseCache.cs        # Response caching (semantic matching)
│   ├── LLMErrorHandler.cs         # Error handling and logging
│   └── MemoryHelpers.cs           # Memory extraction utilities
│
├── Data/                           # Data models and knowledge systems
│   ├── NPCPersonalities.cs        # 54 personality archetypes + location system
│   ├── NPCPersonalityDatabase.cs  # Personality database
│   ├── NPCPersonalityMapper.cs    # Personality mapping utilities
│   ├── NPCKnowledgeSystem.cs      # Proactive RAG (role-based knowledge)
│   ├── NPCLocationDatabase.cs     # Static NPC location database (NEW)
│   ├── SimpleLoreSystem.cs        # Keyword-based lore search (Level 1)
│   ├── VectorLoreSystem.cs        # Vector semantic search (Level 2)
│   ├── LoreEntry.cs               # Lore data model
│   ├── MemoryModels.cs            # Memory and relationship models
│   ├── EmbeddingService.cs        # Vector embedding generation
│   └── NPCNames.cs                # Random name generation
│
├── Commands/                       # In-game commands
│   ├── SpawnPersonalityNPC.cs     # Spawn NPCs by personality type
│   ├── SpawnNPCGroups.cs          # Spawn pre-configured NPC groups
│   ├── SpawnLLMQuester.cs         # Spawn quest-giving NPCs
│   ├── LLMMenuCommand.cs          # Interactive spawner menu
│   ├── MemoryCommands.cs          # Memory management commands
│   ├── CacheCommands.cs           # Cache management commands
│   ├── KnowledgeTestCommand.cs    # Knowledge system testing (NEW)
│   ├── LocationTestCommand.cs    # Location-based response testing (NEW)
│   └── SpawnNPCWithHistory.cs     # Spawn NPCs with memory history
│
├── UI/                             # User interfaces
│   ├── LLMNpcGump.cs              # Visual NPC spawner GUI
│   └── PersonalitySpawner.cs.old  # Legacy spawner (deprecated)
│
├── Tests/                          # Testing suites
│   ├── AutomatedKnowledgeTest.cs  # Comprehensive knowledge testing (NEW)
│   ├── LongTermMemoryTests.cs     # Memory system validation
│   ├── LocationBasedResponseTests.cs # Location-based response testing (NEW)
│   ├── StandaloneMemoryTest.cs    # Isolated memory testing
│   ├── IntegrationTests.cs        # End-to-end conversation testing
│   ├── PerformanceTests.cs        # Performance benchmarks
│   └── ThreeMonthSimulation.cs    # Long-running simulation
│
├── Examples/                       # Example implementations
│   └── SimpleGatherQuest.cs       # Sample quest integration
│
├── Database/                       # Database schemas (legacy)
│   └── schema.sql                 # Legacy PostgreSQL schema (not used - SQLite now)
│
├── Documentation/                  # This directory
│   ├── 00_INDEX.md                # This file
│   ├── README.md                  # Main documentation
│   ├── QUICKSTART.txt             # Quick reference
│   ├── NPC_IMPLEMENTATION_TEMPLATE.md # Complete NPC LLM integration guide (NEW - 2025-12-08)
│   ├── PERSONALITY_GUIDE.md       # Personality system (comprehensive)
│   ├── LOCAL_LLM_SETUP.md         # Ollama setup instructions
│   ├── MEMORY_ARCHITECTURE.md     # Memory system design
│   ├── RAG_LORE_ARCHITECTURE.md   # RAG and knowledge base
│   ├── NPC_KNOWLEDGE_BOUNDARIES.md # Knowledge boundaries system
│   ├── NPC_KNOWLEDGE_TESTING.md   # Knowledge system testing
│   ├── WANDERING_NPC_TRACKING_ROADMAP.md # Future wandering NPC tracking (NEW)
│   ├── VENDOR_SETUP.md            # Vendor integration guide
│   ├── NPC_TITLES_REFERENCE.md    # Title-to-personality mappings
│   ├── PROMPT_EXAMPLES.md         # Example prompts
│   └── Vystia.pdf                 # World lore document
│
├── LLMConversationPlugin.cs       # Global enable/disable plugin
├── ILLMConversational.cs          # Conversational interface
└── LLMConversationHelper.cs       # Conversation processing helper (1000+ lines)

Config/                             # Configuration files (created at runtime)
├── LLM.cfg                        # OpenAI API key
├── LocalLLM.cfg                   # Ollama endpoint and model
└── LLMMemory.cfg                  # Memory system settings

Data/Lore/                          # Lore database
├── sosaria_lore.json              # 47+ lore entries (keyword search)
└── lore_embeddings.cache          # Pre-computed vector embeddings
```

---

## Implementation Status

### ✅ Fully Implemented (Production Ready)

**Core Features:**
- Natural language conversation system
- OpenAI GPT-4o-mini integration
- Ollama local LLM integration (phi3:mini, llama3.1, mistral)
- Smart provider routing (auto-select best LLM)
- 45+ personality archetypes
- 6 speech patterns
- Request queuing and throttling (max 5 per NPC)
- Response chunking with delays
- Meta-commentary stripping
- Active conversation tracking (2-minute timeout)
- Context menu integration

**Memory System:**
- SQLite persistent storage (file-based, no server needed)
- In-memory fallback storage
- Memory importance scoring (1-10)
- Relationship tracking (-100 to +100 reputation)
- Conversation history persistence
- Automatic memory extraction from conversations
- Relationship decay (configurable)

**Knowledge System (RAG):**
- Proactive RAG with pre-loaded knowledge
- 412+ Sosaria lore entries (expanded)
- Role-based knowledge filtering (16 roles)
- Location-based knowledge loading
- Keyword search (SimpleLoreSystem)
- Vector semantic search (VectorLoreSystem)
- Embedding cache with TTL
- Comprehensive testing for all 54 personality types

**Location & Referral System:**
- Static NPC location database (auto-populated)
- Region-based NPC lookup with parent region fallback
- Cross-town referrals when local NPCs unavailable
- Distance descriptions ("nearby", "a short walk", "across town")
- Multiple NPC options (can mention 2-3 nearby NPCs)
- NPC name mentions in directions
- 30-second cache TTL (optimized for town NPCs)
- Movement tracking (updates on >5 tile movement)

**Vendor Integration:**
- Natural language buy/sell commands
- Intent detection from LLM response
- Automatic vendor gump opening
- Personality-based inventory assignment
- SBInfo integration

**Quest System:**
- LLMQuester class for quest NPCs
- LLM-powered quest dialogue
- Quest state awareness
- Traditional ServUO quest integration
- Acceptance/decline detection

**Testing:**
- Memory system tests
- Integration tests
- Performance benchmarks
- Three-month simulation
- Standalone memory testing

### 📋 Planned (Phase 2)

**Autonomous NPCs:**
- Self-directed movement and behavior
- Regional intelligence (batch decision making)
- NPC-to-NPC conversations
- Event-driven behaviors
- Emergent quest generation
- Dynamic economy participation

**Advanced Features:**
- Personality evolution over time
- Faction emergence
- Advanced procedural storytelling
- Meta-learning from players

---

## Commands Reference

| Command | Description |
|---------|-------------|
| `[spawnllmnpc` | Open visual NPC spawner gump |
| `[SpawnPersonalityNPC <type> <speech>` | Spawn NPC by personality type and speech pattern |
| `[SpawnTownNPCs` | Spawn town NPC group (Blacksmith, Guard, etc.) |
| `[SpawnMagicNPCs` | Spawn magic NPC group (Archmage, Mystic, etc.) |
| `[SpawnAdventurerNPCs` | Spawn adventurer group (Warrior, Ranger, etc.) |
| `[LLMConfig <setting> <value>` | Configure LLM provider settings |
| `[MemoryStats` | Display memory system statistics |
| `[ViewMemories <npc> <player>` | View memories for NPC-player pair |
| `[ViewRelationship <npc> <player>` | Display relationship status |
| `[ClearMemories <npc> <player>` | Clear memories (admin only) |
| `[CacheStats` | Display response cache statistics |
| `[ClearCache` | Clear response cache |
| `[LoreStats` | View lore database statistics |
| `[KnowledgeTest` | Run comprehensive knowledge and referral tests |
| `[LocationTest` | Run location-based response tests |
| `[MemoryTest` | Run memory system validation tests |
| `[IntegrationTest` | Run integration tests |
| `[PerformanceTest` | Run performance benchmarks |
| `[MemorySystemTest` | Test memory system components |

---

## Configuration Files

### LLM.cfg (OpenAI)
```
# OpenAI API Configuration
# Get your API key from: https://platform.openai.com/api-keys
ApiKey=sk-your-actual-api-key-here
```

### LocalLLM.cfg (Ollama)
```
# Local LLM Configuration (Ollama)
Endpoint=http://localhost:11434
Model=phi3:mini

# Available models (requires 'ollama pull <model>' first):
# - phi3:mini (fastest, 1-2 sec, 3.8B params, recommended)
# - llama3.1:8b (balanced, 2-3 sec, 8B params)
# - llama3.1:70b (best quality, 5-10 sec, 70B params, requires 48GB RAM)
# - mistral:7b (quality, 2-4 sec, 7B params)
```

### LLMMemory.cfg (Memory System)
```
# Memory System Configuration
LLMMemory.MemorySystem.Enabled=true

# SQLite Database (file-based, no server needed)
LLMMemory.MemoryDatabase.Provider=SQLite
LLMMemory.MemoryDatabase.ConnectionString=  # Optional, defaults to Data/llm_memories.db

# Memory Limits
LLMMemory.MemorySystem.MaxMemoriesPerNPC=50
LLMMemory.MemorySystem.MemoryImportanceThreshold=3

# Auto Cleanup
LLMMemory.MemorySystem.AutoCleanupEnabled=true
LLMMemory.MemorySystem.CleanupIntervalHours=24

# Relationship Decay
LLMMemory.MemorySystem.RelationshipDecayEnabled=true
LLMMemory.MemorySystem.RelationshipDecayDays=7
LLMMemory.MemorySystem.RelationshipDecayAmount=1
```

---

## Performance Characteristics

### Response Times

| Provider | Model | Response Time | Cost | Quality |
|----------|-------|---------------|------|---------|
| OpenAI | gpt-4o-mini | 1-3 seconds | ~$0.001/conv | Excellent |
| Ollama | phi3:mini | 1-2 seconds | Free | Very Good |
| Ollama | llama3.1:8b | 2-3 seconds | Free | Excellent |
| Ollama | mistral:7b | 2-4 seconds | Free | Excellent |

### System Overhead

- **RAM per NPC**: ~65 KB
- **Database per NPC**: ~330 KB (long-term)
- **Cache hit rate**: ~85% (semantic matching)
- **Response cache**: 100 entries, 24-hour TTL
- **Conversation history**: Last 10 messages
- **Knowledge base per NPC**: Up to 25 lore entries
- **Queue limit**: 5 pending requests per NPC
- **Request timeout**: 30 seconds

---

## Requirements

### Minimum
- ServUO (latest version)
- .NET Framework 4.8+ or .NET 6+
- OpenAI API key OR Ollama installed
- 4GB RAM

### Recommended
- 8GB+ RAM
- Ollama with phi3:mini or llama3.1:8b
- SQLite (embedded, no installation needed)
- SSD storage

### Optional
- SQLite: Persistent memory across restarts (file-based)
- Ollama: Free local LLM (no API costs)
- nomic-embed-text model: Semantic lore search

---

## Development Roadmap

### ✅ Phase 1: Conversational NPCs (COMPLETE)
- Natural language conversations
- OpenAI + Ollama integration
- Personality system (45+ types)
- Vendor functionality
- Quest integration
- Response caching

### ✅ Phase 1.5: Memory & Knowledge (COMPLETE)
- SQLite memory architecture (file-based, zero external dependencies)
- Relationship tracking
- Proactive RAG system
- Keyword + semantic lore search
- Memory importance scoring
- Automatic cleanup

### ✅ Phase 1.6: Location & Testing (COMPLETE)
- Static NPC location database
- Location-based NPC referrals
- Comprehensive knowledge testing (all 54 personality types)
- Location-based response testing
- Performance optimizations
- Movement tracking for wandering NPCs

### 📋 Phase 2: Autonomous NPCs (PLANNED)
- Self-directed movement
- Regional intelligence
- NPC-to-NPC interactions
- Event-driven behaviors
- Emergent quests
- Dynamic economy

### 🚀 Phase 3: Advanced Features (FUTURE)
- Personality evolution
- Faction politics
- Voice integration
- Multi-shard sync
- Procedural storytelling

---

## Vystia Custom Content Integration

### Custom NPCs with LLM Integration (NEW - 2025-12-08)

**Complete Implementation Template:** See `NPC_IMPLEMENTATION_TEMPLATE.md`

**Status:** ✅ Full template and working reference implementation (Emperor Garrick Steelarm)

**Custom Personalities Added:**
- Emperor (Imperial ruler, strategic visionary)
- Chieftain (Warrior leader, gruff and honorable)
- Elder (Ancient wise leader, protective of nature)
- Sultan (Diplomatic merchant prince, shrewd and neutral)
- FactionLeader (General faction leader role)

**Custom NPCRoles Added:**
- Emperor → Expert in History, Geography, Law, Trade
- Chieftain → Expert in Combat, Monsters, Nature
- Elder → Expert in History, Nature, Religion
- Sultan → Expert in Trade, Geography, Diplomacy
- Archmage → Expert in Magic, History, Dungeons

**Working Example:**
- `Scripts/Mobiles/Vystia/NPCs/FactionLeaders/EmperorGarrickSteelarm.cs`
- Fully tested with OpenAI GPT-4o-mini
- Speaks correctly about Vystia (not Britannia)
- Has complete Emperor knowledge domain
- Uses formal speech pattern with imperial authority

**Implementation Checklist:**
1. Add PersonalityType to NPCPersonalities.cs
2. Add domain knowledge and base personality descriptions
3. Add NPCRole to NPCKnowledgeSystem.cs
4. Map PersonalityType → NPCRole
5. Add knowledge domain in KnowledgeDomain.cs
6. Create NPC class implementing ILLMConversational
7. Set PersonalityType, SpeechPattern, HearingRange
8. Implement HandleConversation calling LLMConversationHelper
9. Add serialization for LLM properties
10. Test with server restart and fresh NPC spawn

**Next Faction Leaders to Implement:**
- Chieftain Bjorn Frostbeard (Frosthold)
- Elder Seraphina Leafwhisper (Verdantpeak)
- Sultan Azir al-Rashid (Desert of Surya)
- Archmage Pyrus Ashborn (Emberlands)

All core system files already support these personalities - just need NPC class files!

---

## Support

### Getting Help
- Check console logs for detailed error messages
- Use `[IntegrationTest` to validate installation
- Verify configuration files in `Config/` directory
- Review README.md for troubleshooting

### Contributing
- Follow ServUO coding standards
- Test with provided test suites (`[MemorySystemTest`, `[PerformanceTest`)
- Document new features
- Performance test before submitting

---

## Credits

**LLM Providers:**
- OpenAI (GPT-4o-mini)
- Ollama (phi3:mini, llama3.1, mistral)

**Database:**
- SQLite (file-based, no external dependencies)

**ServUO Integration:**
- BaseVendor for vendor functionality
- BaseQuester for quest system
- Quest system for traditional quests

**Architecture:**
- Proactive RAG design
- Hybrid memory system (three-tier)
- Smart provider routing

---

**Version**: 1.7.0 (Vystia Integration)
**Status**: Production Ready
**License**: Free for ServUO projects
**Last Updated**: 2025-12-08
