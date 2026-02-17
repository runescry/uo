# RAG & Lore System Architecture

The RAG (Retrieval-Augmented Generation) system provides NPCs with world knowledge, enabling them to answer questions about lore, locations, and history.

---

## What is RAG?

**RAG (Retrieval Augmented Generation)** means the LLM gets relevant knowledge from your lore database before answering questions. This makes NPCs smarter and more accurate.

### Two Search Modes

**1. Keyword Search (Always Available)** ✅
- Searches lore entries for matching keywords
- Fast (instant, no external dependencies)
- Works great for most queries
- No setup required
- **This is fully functional and sufficient for most use cases!**

**2. Vector Search (Optional Enhancement)** 🚀
- Uses AI embeddings to understand meaning (semantic search)
- Can match synonyms and related concepts
- Example: "dragon" matches "wyrm", "drake", "serpent"
- Requires Ollama running + one-time embedding generation
- **Optional!** Keyword search works great without it.

---

## Overview

The system uses **Proactive RAG** instead of reactive queries:

**Traditional RAG (Reactive)**:
```
Player asks question → Search lore database → Send to LLM → Generate response
Problem: Adds 200-500ms latency per query
```

**Proactive RAG (Our Approach)**:
```
NPC spawns → Load role-specific knowledge → Cache knowledge → Ready for questions
Benefit: Zero latency penalty, knowledge already in context
```

---

## Architecture

```
┌─────────────────────────────────────────────────────────────┐
│                        NPC SPAWNS                            │
└─────────────────────┬───────────────────────────────────────┘
                      │
                      ▼
┌─────────────────────────────────────────────────────────────┐
│                  KNOWLEDGE LOADING                           │
│  1. Determine NPC role (from personality)                    │
│  2. Load role-specific lore                                  │
│  3. Load location-specific lore                              │
│  4. Format for LLM prompt                                    │
│  5. Cache formatted knowledge                                │
└─────────────────────┬───────────────────────────────────────┘
                      │
                      ▼
┌─────────────────────────────────────────────────────────────┐
│                    NPC READY                                 │
│  - Knowledge pre-loaded in memory                            │
│  - No database queries during conversation                   │
│  - Instant responses with lore awareness                     │
└─────────────────────────────────────────────────────────────┘
```

---

## Components

### 1. SimpleLoreSystem (Keyword Search)

**File**: `Data/SimpleLoreSystem.cs`

**Purpose**: Fast keyword-based lore retrieval (Level 1)

**Data Source**: `Data/Lore/sosaria_lore.json`

**Features**:
- Loads 47+ lore entries at startup
- Keyword indexing for fast search
- Category filtering (History, Location, Dungeon, etc.)
- Importance scoring (1-10)
- Max 3 results per query

**Lore Entry Structure**:
```json
{
  "id": "britannia",
  "title": "Kingdom of Britannia",
  "category": "Location",
  "content": "Britannia is the central kingdom of Sosaria, ruled by Lord British...",
  "tags": ["britannia", "kingdom", "lord british", "capital"],
  "importance": 10,
  "relatedEntries": ["lordbritish", "castle_britannia"]
}
```

**Categories**:
- History: Historical events and timelines
- NPC: Named characters and their backgrounds
- Location: Cities, towns, landmarks
- Dungeon: Dungeons and dangerous areas
- Monster: Creatures and threats
- Magic: Spells, reagents, magical lore
- Crafting: Crafting-related knowledge
- Reagent: Reagent information

**Search Example**:
```csharp
var results = SimpleLoreSystem.SearchLore("britain", maxResults: 3);
// Returns: Kingdom of Britannia, Lord British, Castle Britannia
```

---

### 2. VectorLoreSystem (Semantic Search)

**File**: `Data/VectorLoreSystem.cs`

**Purpose**: AI-powered semantic lore search (Level 2)

**How It Works**:
1. Generate embeddings for all lore entries (one-time)
2. Cache embeddings to disk
3. For queries, generate query embedding
4. Find most similar lore via cosine similarity
5. Return top N results

**Embedding Model**: `nomic-embed-text` (Ollama)

**Features**:
- Semantic understanding ("dragon" matches "wyrm", "drake")
- Embedding cache (`Data/Lore/lore_embeddings.cache`)
- Query embedding cache (100 queries, 5-minute TTL)
- Fast mode: Fallback to keyword search if embeddings not ready
- Automatic cache generation on first use

**Cache Structure**:
```csharp
Dictionary<string, float[]> embeddings
// Key: lore ID (e.g., "britannia")
// Value: 768-dimensional vector
```

**Performance**:
- Embedding generation: ~100ms per entry (one-time)
- Query embedding: ~100ms (cached)
- Similarity search: ~10ms for 47 entries
- Total: <150ms with cache, fallback to keyword if needed

**Search Example**:
```csharp
var results = await VectorLoreSystem.SearchLoreAsync("tell me about dragons", maxResults: 3);
// Returns: Dragons, Destard (dragon dungeon), Drake (related creature)
```

---

### 3. NPCKnowledgeSystem (Role-Based Loading)

**File**: `Data/NPCKnowledgeSystem.cs`

**Purpose**: Pre-load knowledge based on NPC role

**NPC Roles**:
```csharp
public enum NPCRole
{
    Guard,           // Threats, law, defense
    Blacksmith,      // Crafting, weapons, armor
    Mage,            // Magic, spells, reagents
    Merchant,        // Trade, economy, goods
    Scholar,         // All knowledge (broad)
    Healer,          // Medicine, healing, reagents
    Ranger,          // Wilderness, nature, tracking
    Noble,           // Politics, history, etiquette
    Commoner,        // Daily life, gossip, simple matters
    Sage,            // Ancient lore, philosophy
    Warrior,         // Combat, tactics, weapons
    Thief,           // Shadows, secrets, underground
    Bard,            // Stories, songs, entertainment
    Farmer,          // Agriculture, weather, seasons
    Fisher,          // Fishing, waters, tides
    Innkeeper        // Hospitality, local area, gossip
}
```

**Knowledge Filtering by Role**:

```csharp
// Blacksmith knowledge
Categories: Crafting, Reagent (for ores)
Importance: >= 6
Specific tags: "forge", "metal", "weapon", "armor", "ore"
Town locations: All
Dungeons: Only those with ore veins

// Guard knowledge
Categories: Monster, Dungeon, NPC (threats)
Importance: >= 7
Specific tags: "threat", "danger", "criminal", "law"
Town locations: All
Dungeons: All (need to know dangers)

// Merchant knowledge
Categories: Location (trade routes), NPC (trading partners)
Importance: >= 6
Specific tags: "trade", "economy", "market", "goods"
Town locations: All
Dungeons: None (not relevant)

// Scholar knowledge
Categories: ALL
Importance: >= 5
Specific tags: ALL
Everything: Yes (scholars know broadly)
```

**Location-Based Knowledge**:
```csharp
// Load dungeons within 200 tiles
var nearbyDungeons = allLore
    .Where(l => l.Category == "Dungeon")
    .Where(l => Distance(npcLocation, dungeonLocation) <= 200)
    .ToList();

// Load important locations within 100 tiles
var nearbyLocations = allLore
    .Where(l => l.Category == "Location")
    .Where(l => l.Importance >= 8)
    .Where(l => Distance(npcLocation, locationLocation) <= 100)
    .ToList();
```

**Knowledge Loading Process**:
```csharp
public static List<LoreEntry> GetNPCKnowledge(NPCRole role, string locationName, Point3D location, Map map)
{
    1. Load ALL lore from SimpleLoreSystem
    2. Filter by role (role-specific categories and tags)
    3. Filter by importance (role-specific threshold)
    4. Add location-based knowledge (nearby dungeons/locations)
    5. Add historical knowledge (top 25 by importance)
    6. Deduplicate by ID
    7. Return combined knowledge
}
```

**Example for Blacksmith**:
```csharp
Role: Blacksmith
Location: Britain Forge (1420, 1690, Map.Felucca)

Loaded Knowledge:
- [Crafting] "The Art of Blacksmithing" (importance: 9)
- [Crafting] "Properties of Valorite" (importance: 8)
- [Reagent] "Iron Ore Veins" (importance: 7)
- [Location] "Britain" (importance: 10) - nearby
- [Dungeon] "Shame" (importance: 8) - nearby, has ore
- [History] "The First Blacksmiths" (importance: 7)
- [NPC] "Lord British" (importance: 10) - historical
...
Total: 18 entries
```

---

## Knowledge Format for LLM

**File**: `Data/NPCKnowledgeSystem.cs` → `FormatKnowledgeForPrompt()`

**Formatted Output**:
```
WORLD KNOWLEDGE:

[History] The Fall of Mondain (Importance: 10)
In the year 1355, the evil wizard Mondain was defeated by a great hero...

[Location] Kingdom of Britannia (Importance: 10)
Britannia is the central kingdom of Sosaria, ruled by the virtuous Lord British...

[Dungeon] Covetous (Importance: 8)
A treacherous dungeon filled with creatures drawn to wealth and treasure...

[Crafting] The Art of Blacksmithing (Importance: 9)
Blacksmithing requires knowledge of metals, heat, and the rhythmic strike of hammer...

[Monster] Dragons (Importance: 9)
Ancient and powerful creatures, dragons are among the most fearsome beasts in Sosaria...
```

**Prompt Integration**:
```csharp
string fullPrompt = $@"
{personalityPrompt}

{contextualInfo}

{formattedKnowledge}

Player: {playerMessage}
";
```

---

## Lore Database

### Current Lore Entries (47+)

**File**: `Data/Lore/sosaria_lore.json`

**Coverage**:

**History** (10 entries):
- Fall of Mondain
- Exodus and the Second Age
- Lord British's Rise
- The Virtues
- And more...

**Locations** (12 entries):
- Britannia (kingdom)
- Britain (city)
- Trinsic
- Moonglow
- Vesper
- Skara Brae
- And more...

**Dungeons** (8 entries):
- Despise
- Destard (dragons)
- Covetous
- Shame
- Wrong
- Deceit
- And more...

**NPCs** (6 entries):
- Lord British
- Mondain
- Minax
- Exodus
- And more...

**Monsters** (5 entries):
- Dragons
- Orcs
- Liches
- Elementals
- Daemons

**Magic** (4 entries):
- Reagents
- Spellcasting
- Runes
- Magic items

**Crafting** (2 entries):
- Blacksmithing
- Alchemy

---

## Adding New Lore

### 1. Edit Lore JSON

**File**: `Data/Lore/sosaria_lore.json`

```json
{
  "id": "new_dungeon",
  "title": "The Abyss",
  "category": "Dungeon",
  "content": "Deep beneath Britannia lies the Abyss, an eight-level dungeon representing the antithesis of the Eight Virtues. Each level tests explorers with increasing danger...",
  "tags": ["abyss", "dungeon", "virtue", "eight levels", "deep"],
  "importance": 9,
  "relatedEntries": ["virtues", "britannia"]
}
```

### 2. Reload Lore

Restart server or use:
```
[LoreStats
```

Check output:
```
Lore System Statistics:
Total entries: 48
Categories: History (10), Location (12), Dungeon (9), ...
```

### 3. Regenerate Embeddings (Optional)

For semantic search:
```
Delete: Data/Lore/lore_embeddings.cache
Restart server (will regenerate automatically)
```

---

## Performance Optimization

### Proactive RAG Benefits

**Without Proactive RAG** (per conversation):
```
1. Player asks question (0ms)
2. Analyze question for keywords (50ms)
3. Query lore database (100-200ms)
4. Generate embeddings for query (100ms)
5. Find similar lore (50ms)
6. Send to LLM with lore (1000-3000ms)
Total: 1300-3400ms
```

**With Proactive RAG** (per conversation):
```
1. Player asks question (0ms)
2. Send to LLM with pre-loaded knowledge (1000-3000ms)
Total: 1000-3000ms
Savings: 300-400ms per query, no database hits
```

### Caching Strategy

**Lore Loading** (at NPC spawn):
```csharp
// Cached per NPC instance
private List<LoreEntry> m_KnowledgeBase;
private string m_FormattedKnowledge;

// Only loaded once when NPC spawns
LoadKnowledgeBase();
```

**Benefits**:
- Zero per-conversation latency
- No database queries during conversation
- Knowledge stays in RAM with NPC
- Format happens once, not per conversation

---

## Integration with Conversations

### Example: Blacksmith NPC

**NPC Spawns**:
```csharp
LLMNpc blacksmith = new LLMNpc("Gareth", PersonalityType.Blacksmith, SpeechPattern.Archaic);
// Automatically loads:
// - Blacksmithing lore
// - Crafting lore
// - Ore/reagent info
// - Nearby locations
// - Important history
```

**Player Asks Question**:
```
Player: "Tell me about valorite"
```

**Context Sent to LLM**:
```
You are Gareth, a skilled blacksmith...

WORLD KNOWLEDGE:
[Crafting] Properties of Valorite (Importance: 8)
Valorite is the rarest and strongest metal in Sosaria. It resists magic and holds
enchantments better than any other ore. Only the most skilled smiths can work it...

[Reagent] Ore Veins of Sosaria (Importance: 7)
Valorite can be found in the deepest parts of dungeons...

Player: Tell me about valorite
```

**LLM Response**:
```
Ah, valorite! The rarest and finest metal known to smith! 'Tis stronger than any
other ore and resists magic like no other. I've worked it a few times - requires
tremendous heat and a masterful hand. Ye can find it in the deepest dungeons,
if ye dare venture there. Would ye like me to craft something from valorite?
```

**Without Knowledge System**:
```
Player: Tell me about valorite
NPC: I'm not familiar with valorite. Perhaps you could tell me more about it?
```

---

## Testing

### Test Lore Loading

```
1. Spawn NPC:
[SpawnPersonalityNPC Blacksmith Archaic

2. Check console:
[LLMNpc] Gareth (Blacksmith) knowledge base loaded: 18 entries
[LLMNpc] Formatted knowledge length: 4521 chars
```

### Test Lore Search

```
1. Check lore stats:
[LoreStats

Output:
Lore System Statistics:
Total entries: 47
Categories: History (10), Location (12), Dungeon (8), ...
Vector embeddings: AVAILABLE
Cached embeddings: 47/47

2. Test search (console):
SimpleLoreSystem.SearchLore("dragon", 3)
→ Returns: Dragons, Destard, Drake
```

### Test Semantic Search

```
Requires Ollama with nomic-embed-text:
ollama pull nomic-embed-text

Then:
VectorLoreSystem.SearchLoreAsync("flying creatures", 3)
→ Returns: Dragons, Gargoyles, Drakes (semantic match!)
```

---

## Troubleshooting

### Knowledge Not Loading

**Symptom**: NPC doesn't know lore

**Check**:
```
Console output on NPC spawn:
[LLMNpc] WARNING: Knowledge base is EMPTY for <NPC>
```

**Solutions**:
1. Verify `Data/Lore/sosaria_lore.json` exists
2. Check JSON syntax (use JSON validator)
3. Verify SimpleLoreSystem initialized: `[LoreStats`
4. Check console for loading errors

### Embeddings Not Working

**Symptom**: Semantic search falls back to keyword

**Check**:
```
[LoreStats
Vector embeddings: NOT AVAILABLE
```

**Solutions**:
1. Install Ollama: https://ollama.com
2. Pull embedding model: `ollama pull nomic-embed-text`
3. Verify Ollama running: `ollama list`
4. Delete cache and regenerate: Delete `Data/Lore/lore_embeddings.cache`

### NPC Knows Wrong Things

**Symptom**: Blacksmith talks about magic instead of forging

**Check**:
```
Role inference:
[LLMNpc] Inferring role from personality: Blacksmith → Blacksmith role
```

**Solutions**:
1. Verify personality type matches intended role
2. Check `NPCKnowledgeSystem.InferRoleFromPersonality()`
3. Add custom role mapping if needed

---

## Future Enhancements

### Planned Features

**Dynamic Lore Updates**:
- NPCs learn new lore from events
- Player-contributed lore
- Lore spreading between NPCs (gossip)

**Contextual Filtering**:
- Time-based lore (day/night different knowledge)
- Faction-based lore (NPCs know faction secrets)
- Quest-based lore (reveal lore during quests)

**Enhanced Semantic Search**:
- Better embedding models
- Multi-language support
- Image-based lore (visual references)

**Lore Generation**:
- LLM-generated lore expansion
- Procedural history generation
- Dynamic world events creating lore

---

## Best Practices

1. **Keep Lore Concise**: 2-3 paragraphs max per entry
2. **Use Importance Wisely**: 10 = critical, 1 = trivial
3. **Tag Thoroughly**: Multiple tags improve search
4. **Link Related Entries**: Use relatedEntries field
5. **Test After Adding**: Use `[LoreStats` and test NPC conversations
6. **Balance Knowledge**: Don't overload NPCs (max 25 entries)
7. **Role-Appropriate**: Blacksmiths shouldn't know magic lore

---

**Related Documentation:**
- [README.md](README.md) - Main documentation
- [MEMORY_ARCHITECTURE.md](MEMORY_ARCHITECTURE.md) - Memory system
- [PERSONALITY_GUIDE.md](PERSONALITY_GUIDE.md) - Personality system
