# LLM-Enabled NPCs for ServUO

This system allows you to create NPCs that use Large Language Models (OpenAI GPT or local Ollama) to generate conversational responses in real-time.

## Features

- **Conversational NPCs**: NPCs that respond naturally to player speech
- **45+ Personality Types**: From merchants and guards to actors and gypsies
- **6 Speech Patterns**: Modern, Formal, OldEnglish, Cryptic, Casual, Archaic
- **Dual LLM Support**: OpenAI (cloud) or Ollama (local, free)
- **Smart Vendor Integration**: Natural language buying/selling commands
- **Quest NPCs**: LLM-powered quest dialogue with traditional quest mechanics
- **Persistent Memory**: NPCs remember players across sessions (SQLite database)
- **Relationship System**: Reputation tracking from -100 to +100
- **Knowledge Base**: 412+ lore entries with keyword and semantic search
- **Location-Based Referrals**: NPCs provide directions to nearby NPCs
- **Static Location Database**: Fast lookups for town NPCs
- **Proactive RAG**: Role-based knowledge pre-loaded at spawn (no per-query searches)
- **Response Caching**: Semantic cache for common queries (24-hour TTL)
- **Request Throttling**: Queue management with max 5 pending per NPC
- **Context Menu**: Reset conversation, view paperdoll
- **Automatic Cleanup**: Old conversations cleaned up after timeout

## Setup Instructions

### 1. Choose Your LLM Provider

**Option A: OpenAI (Cloud - Paid)**
1. Visit https://platform.openai.com/api-keys
2. Sign up or log in to your OpenAI account
3. Create a new API key
4. Copy the key (starts with `sk-`)

**Option B: Ollama (Local - Free)**
1. Install Ollama from https://ollama.com
2. Pull a model: `ollama pull phi3:mini` (fastest, 1-2 sec responses)
3. Verify: `ollama list`

### 2. Configure the API Key

After first server startup, config files will be created:

**For OpenAI** - Edit `Config/LLM.cfg`:
```
# OpenAI API Configuration
# Get your API key from: https://platform.openai.com/api-keys
ApiKey=sk-your-actual-api-key-here
```

**For Ollama** - Edit `Config/LocalLLM.cfg`:
```
# Local LLM Configuration (Ollama)
Endpoint=http://localhost:11434
Model=phi3:mini

# Available models (run 'ollama pull <model>' first):
# - phi3:mini (fastest, 1-2 sec, 3.8B params)
# - llama3.1:8b (balanced, 2-3 sec, 8B params)
# - mistral:7b (quality, 2-4 sec, 7B params)
```

### 3. Restart the Server

Restart ServUO for configuration changes to take effect.

## Usage

### Spawning NPCs with UI Gump

Use `[spawnllmnpc` to open the visual spawner:
- Click category buttons (Town NPCs, Magic & Mystical, Adventurers, etc.)
- Select individual NPCs or spawn entire groups
- Click to place NPC at target location

### Spawning NPCs by Command

**Spawn by Personality Type:**
```
[SpawnPersonalityNPC Blacksmith Archaic
[SpawnPersonalityNPC Mage Formal
[SpawnPersonalityNPC Merchant Casual
```

**Spawn Pre-configured Groups:**
```
[SpawnTownNPCs        # Blacksmith, Guard, Herbalist, Tavernkeeper, Librarian
[SpawnMagicNPCs       # Archmage, Mystic, Necromancer, Druid
[SpawnAdventurerNPCs  # Warrior, Treasure Hunter, Bard, Ranger
```

### Talking to NPCs

Simply type near an LLM NPC (within 10 tiles by default). The NPC will:
1. Show `*ponders*` while thinking
2. Call the LLM API (1-3 seconds)
3. Respond with contextual message

**Conversation Tips:**
- Say the NPC's name to start a conversation
- After naming them once, continue talking without repeating their name
- Conversation expires after 2 minutes of silence
- Right-click NPC and select "Reset" to clear history

### Vendor Commands

LLM NPCs with vendor inventory understand natural language:
- "I'd like to buy a sword"
- "Show me your wares"
- "Can I sell some ore?"
- "What do you have for sale?"

The NPC will respond naturally and open the appropriate vendor gump.

## File Structure

```
Scripts/Services/LLM/
├── Core/                    # Core NPC classes
│   ├── LLMNpc.cs           # Main conversational NPC
│   ├── LLMQuester.cs       # Quest-giving NPC
│   └── ConversationContext.cs  # Conversation memory
│
├── Services/                # LLM API integrations
│   ├── UnifiedLLMService.cs    # Smart routing
│   ├── LLMService.cs           # OpenAI integration
│   ├── LocalLLMService.cs      # Ollama integration
│   ├── LLMMemoryService.cs     # Memory coordinator
│   └── SQLiteMemoryDatabase.cs  # Persistent storage (file-based)
│
├── Data/                    # Data models and knowledge
│   ├── NPCPersonalities.cs # 45+ personality types
│   ├── NPCKnowledgeSystem.cs  # Role-based knowledge
│   ├── SimpleLoreSystem.cs    # Keyword lore search
│   ├── VectorLoreSystem.cs    # Semantic search
│   └── MemoryModels.cs        # Memory data models
│
├── Commands/                # In-game commands
│   ├── SpawnPersonalityNPC.cs
│   ├── SpawnNPCGroups.cs
│   ├── MemoryCommands.cs
│   └── CacheCommands.cs
│
├── UI/                      # User interfaces
│   └── LLMNpcGump.cs       # Visual spawner GUI
│
└── Documentation/           # This directory
    ├── README.md           # This file
    ├── QUICKSTART.txt      # Quick reference
    ├── 00_INDEX.md         # Documentation index
    └── ...
```

## Configuration Files

### LLM.cfg (OpenAI)
```
ApiKey=your_openai_api_key_here
```

### LocalLLM.cfg (Ollama)
```
Endpoint=http://localhost:11434
Model=phi3:mini
```

### LLMMemory.cfg (Memory System)
```
Enabled=true
# SQLite - no connection string needed (uses Data/llm_memories.db by default)
# Or specify custom path: LLMMemory.MemoryDatabase.ConnectionString=C:\Path\To\database.db
```

## Cost Considerations

### OpenAI (Cloud)
- **Model**: gpt-4o-mini
- **Input**: ~$0.15 per 1M tokens
- **Output**: ~$0.60 per 1M tokens
- **Average conversation cost**: ~$0.001 (less than a penny)
- **1000 conversations**: ~$0.50-$1.00

### Ollama (Local)
- **Cost**: Free
- **Requirements**: 8GB+ RAM for 8B models
- **Response time**: 1-2 seconds (phi3:mini), 2-4 seconds (llama3.1:8b)
- **Quality**: Very good with llama3.1:8b

### Recommended Setup
- **Player Conversations**: Local LLM (fast, free)
- **Complex Reasoning**: OpenAI (occasional, high quality)
- **Vendor Actions**: Local LLM (simple, fast)

## Commands Reference

| Command | Description |
|---------|-------------|
| `[spawnllmnpc` | Open visual NPC spawner gump |
| `[SpawnPersonalityNPC <type> <speech>` | Spawn NPC by personality type |
| `[SpawnTownNPCs` | Spawn town NPC group |
| `[SpawnMagicNPCs` | Spawn magic NPC group |
| `[SpawnAdventurerNPCs` | Spawn adventurer NPC group |
| `[LLMConfig` | Configure LLM provider settings |
| `[MemoryStats` | View memory system statistics |
| `[ViewMemories <npc> <player>` | View memories for NPC-player pair |
| `[CacheStats` | View response cache statistics |
| `[ClearCache` | Clear response cache |
| `[KnowledgeTest` | Run comprehensive knowledge and referral tests |
| `[LocationTest` | Run location-based response tests |
| `[MemoryTest` | Run memory system validation tests |
| `[IntegrationTest` | Run integration tests |
| `[PerformanceTest` | Run performance benchmarks |

## Personality Types (45+)

### Generic Types
- Merchant, Guard, Noble, Sage, Commoner, Villain, Hermit, Healer, Warrior, Mage

### Profession Types
- Actor, Artist, Gypsy, Bard, Blacksmith, Tailor, Alchemist, Banker, InnKeeper, Barkeeper
- Cook, Farmer, Fisherman, Miner, Carpenter, Tinker, Scribe, Jeweler, LeatherWorker
- Bowyer, Weaponsmith, Armorer, Provisioner, AnimalTrainer, HairStylist, Herbalist
- Veterinarian, Shipwright, Mapmaker, RealEstateBroker, TownCrier, Vagabond, Peasant
- And more...

### Speech Patterns
- **Modern**: Contemporary English
- **Formal**: Elevated, proper English
- **OldEnglish**: Thee, thou, dost, hath
- **Cryptic**: Mysterious, riddles, vague
- **Casual**: Relaxed, contractions, slang
- **Archaic**: Medieval fantasy speech

## Troubleshooting

### NPCs say "I cannot speak right now..."
- Check that your API key is correctly configured in `Config/LLM.cfg`
- Verify the API key is valid at https://platform.openai.com/api-keys
- For Ollama: Verify service is running: `ollama list`
- Restart the server after adding the API key

### NPCs say "I seem to be having trouble thinking..."
- Check the server console for error messages
- Verify you have internet connectivity (OpenAI)
- Check your OpenAI account has available credits
- For Ollama: Verify model is pulled: `ollama list`

### No response from NPC
- Make sure you're within hearing range (default: 10 tiles)
- Say the NPC's name to start a conversation
- Check the server console for errors
- Verify the NPC is an `LLMNpc` (check with `[props`)

### Slow responses
- OpenAI: 1-3 seconds is normal
- Ollama with phi3:mini: 1-2 seconds is normal
- Ollama with larger models: 2-4+ seconds is normal
- Check network latency (OpenAI) or CPU usage (Ollama)

### Memory system not working
- Check `Config/LLMMemory.cfg` - set `Enabled=true`
- Verify SQLite database file exists at `Data/llm_memories.db`
- Check console logs for SQLite initialization messages
- Use `[MemoryStats` to check system status

## Advanced Features

### Memory System
NPCs can remember players across sessions:
- **Memories**: Stored with importance scores (1-10)
- **Relationships**: Reputation from -100 to +100
- **Conversation History**: Full session storage
- **Storage**: SQLite (persistent, file-based) + In-Memory (fallback)

### Knowledge Base (RAG)
NPCs have access to world lore:
- **412+ lore entries**: Sosaria history, locations, dungeons, NPCs, virtues, principles
- **Role-based knowledge**: Blacksmiths know about forging, guards about threats
- **Location awareness**: NPCs know about nearby places
- **Search modes**: Keyword (fast) + Semantic (accurate)
- **Comprehensive testing**: All 54 personality types validated

### Location-Based NPC Referrals
NPCs can help players find other NPCs:
- **"Where is the nearest healer?"** - NPCs provide accurate directions
- **Static database**: Fast lookups for town NPCs (they don't wander far)
- **Distance descriptions**: "nearby", "a short walk", "across town"
- **Multiple options**: Can mention 2-3 nearby NPCs of same type
- **Cross-town referrals**: Suggests other towns when local NPCs unavailable
- **NPC names**: Includes NPC names in directions when available

### Response Caching
Common queries are cached:
- **Cache size**: 100 entries
- **TTL**: 24 hours
- **Semantic matching**: 85% similarity threshold
- **Context-aware**: Per-NPC, vendor-aware

## Security Notes

- Never commit your API key to version control
- Add `Config/LLM.cfg` to your `.gitignore`
- Keep your API key secure
- Monitor your OpenAI usage at https://platform.openai.com/usage

## Future Enhancements

### Phase 2 (Planned)
- Autonomous NPCs (self-directed movement)
- Regional intelligence (batch decision making)
- NPC-to-NPC interactions
- Event-driven behaviors
- Emergent quest generation
- Dynamic economy participation

### Phase 3 (Future)
- Personality evolution over time
- Faction emergence and politics
- Voice/speech integration
- Multi-shard knowledge synchronization
- Procedural storytelling

## Support

For issues or questions:
1. Check the ServUO forums
2. Review the server console for error messages
3. Verify your API configuration
4. Check OpenAI API status at https://status.openai.com/
5. For Ollama issues, check https://github.com/ollama/ollama

## Credits

- **LLM Integration**: OpenAI GPT-4o-mini, Ollama (phi3, llama3.1, mistral)
- **Database**: SQLite (System.Data.SQLite)
- **UO Server**: ServUO
- **Architecture**: Proactive RAG, SQLite Memory, Smart Routing
