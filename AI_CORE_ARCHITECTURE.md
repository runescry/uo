# AI Core Architecture: Vystia LLM NPC & Sidekick Systems

**Project:** Vystia (ServUO — Ultima Online private server)  
**Language:** C# (.NET 6, ServUO framework)  
**AI Stack:** OpenAI GPT-4o-mini (cloud) + Ollama/phi3:mini (local)  
**Last Updated:** 2025-12  

---

## Table of Contents

1. [System Overview](#1-system-overview)
2. [LLM NPC Architecture](#2-llm-npc-architecture)
   - 2.1 [Class Hierarchy](#21-class-hierarchy)
   - 2.2 [Proactive RAG: Knowledge Pre-loading](#22-proactive-rag-knowledge-pre-loading)
   - 2.3 [Speech Pipeline & Request Queue](#23-speech-pipeline--request-queue)
   - 2.4 [Provider Routing (UnifiedLLMService)](#24-provider-routing-unifiedllmservice)
   - 2.5 [OpenAI Integration (LLMService)](#25-openai-integration-llmservice)
   - 2.6 [Local Inference (LocalLLMService)](#26-local-inference-localllmservice)
   - 2.7 [Persistent Memory System](#27-persistent-memory-system)
   - 2.8 [Conversation Context (In-Memory)](#28-conversation-context-in-memory)
   - 2.9 [Response Caching](#29-response-caching)
   - 2.10 [Personality System](#210-personality-system)
3. [Lore & Knowledge Systems](#3-lore--knowledge-systems)
   - 3.1 [Lore Domain Files](#31-lore-domain-files)
   - 3.2 [SimpleLoreSystem vs VectorLoreSystem](#32-simpleloressystem-vs-vectorloressystem)
   - 3.3 [NPCKnowledgeSystem (Proactive RAG Engine)](#33-npcknowledgesystem-proactive-rag-engine)
4. [AI Sidekick Architecture](#4-ai-sidekick-architecture)
   - 4.1 [Class Hierarchy](#41-class-hierarchy)
   - 4.2 [BaseSidekick](#42-basesidekick)
   - 4.3 [AutonomousSidekick](#43-autonomoussidekick)
   - 4.4 [SidekickAI: Decision Controller](#44-sidekickai-decision-controller)
   - 4.5 [Combat Archetype Modules](#45-combat-archetype-modules)
   - 4.6 [SidekickGoalSystem](#46-sidekickgoalsystem)
5. [Data Flows](#5-data-flows)
   - 5.1 [NPC Conversation Flow](#51-npc-conversation-flow)
   - 5.2 [Sidekick Combat Decision Flow](#52-sidekick-combat-decision-flow)
6. [Configuration Reference](#6-configuration-reference)
7. [Architectural Decisions & Tradeoffs](#7-architectural-decisions--tradeoffs)

---

## 1. System Overview

The Vystia AI system consists of two independent but complementary subsystems:

| System | Purpose | AI Backend |
|--------|---------|-----------|
| **LLM NPC** | Intelligent NPC dialogue with persistent memory and world-aware RAG | GPT-4o-mini (primary), Ollama (fallback/background) |
| **AI Sidekick** | Player companion creature with archetype-specific combat behaviour | Rule-based state machine with goal scoring (no LLM at runtime) |

The LLM NPC system is cloud-first for player-facing interactions and local-first for background decisions. The Sidekick system is fully deterministic — LLM personality fields exist in the data model but are currently disabled at runtime.

**Directory layout:**
```
ServUO/Scripts/Services/LLM/
  Core/
    LLMNpc.cs                    # Primary NPC class (1545 lines)
    ConversationContext.cs        # In-memory conversation history
  Services/
    UnifiedLLMService.cs          # Provider router
    LLMService.cs                 # OpenAI integration
    LocalLLMService.cs            # Ollama integration
    LLMMemoryService.cs           # Memory orchestration
    SQLiteMemoryDatabase.cs       # SQLite persistence
    LLMResponseCache.cs           # Semantic + exact cache
  Data/
    NPCKnowledgeSystem.cs         # Proactive RAG engine
    NPCPersonalities.cs           # Personality + speech patterns
    NPCLocationDatabase.cs        # Static NPC position registry
    SimpleLoreSystem.cs           # Keyword lore search
    VectorLoreSystem.cs           # Semantic lore search (Ollama)
    Lore/                         # 16 JSON domain files

ServUO/Scripts/Services/AISidekicks/
  Core/
    BaseSidekick.cs               # Base class (156 lines)
    AutonomousSidekick.cs         # Full companion (1696 lines)
  AI/
    SidekickAI.cs                 # Decision controller (3287 lines)
    SidekickGoalSystem.cs         # Goal scoring (340 lines)
    MageCombatAI.cs               # Kiting + spell combos
    WarriorCombatAI.cs            # Melee tank
    ArcherCombatAI.cs             # Ranged + retreat
    TamerCombatAI.cs              # Pet commands + vet
    HealerCombatAI.cs             # Party healing priority
    PaladinCombatAI.cs            # Hybrid melee + chivalry
    NecromancerCombatAI.cs        # Dark magic + raise dead
    ThiefCombatAI.cs              # Flank + poison
    PlayerLikeCombatAI.cs         # Generic fallback
  Simulation/
    full_combat_simulator.py      # Headless combat testing
    parameter_optimizer.py        # Parameter tuning
```

---

## 2. LLM NPC Architecture

### 2.1 Class Hierarchy

```
Server.Mobiles.Mobile
  └── Server.Mobiles.BaseCreature
        └── Server.Mobiles.BaseVendor
              └── Server.Mobiles.LLMNpc          ← primary NPC class
                    (LLMConversationHelper)       ← static routing helper
```

`LLMNpc` inherits `BaseVendor` so it can buy/sell items while also engaging in LLM conversations. The two modes coexist: standard UO double-click trade UI and natural language speech in the game chat window.

**Key fields on `LLMNpc`:**

| Field | Type | Purpose |
|-------|------|---------|
| `m_Personality` | `string` | Free-form system prompt fragment |
| `m_PersonalityType` | `NPCPersonalities.PersonalityType` | Enum for 20+ archetypes |
| `m_SpeechPattern` | `SpeechPattern` | Medieval / Modern / Poetic / Rustic |
| `m_HearingRange` | `int` | Tiles within which speech is heard (default 10) |
| `m_MaxChunkLength` | `int` | Characters per output chunk (default 100) |
| `m_DelayBetweenChunks` | `int` | ms between chat line emissions |
| `m_ProcessingPlayers` | `HashSet<Mobile>` | Players with in-flight requests |
| `m_RequestQueue` | `Queue<SpeechRequest>` | Per-NPC request queue (max 5) |
| `m_IsProcessingRequest` | `bool` | Processing lock flag |
| `m_QueueLock` | `object` | Thread-safety lock for queue ops |
| `m_KnowledgeBase` | `List<LoreEntry>` | Pre-loaded lore entries (set at spawn) |
| `m_FormattedKnowledge` | `string` | Prompt-ready knowledge block (cached) |

---

### 2.2 Proactive RAG: Knowledge Pre-loading

This is the central architectural insight of the NPC system. Instead of performing a vector search for every player message (reactive RAG), each NPC loads its relevant world knowledge **once at spawn time** and caches it as a pre-formatted string.

```
NPC Spawn
   │
   ▼
LoadKnowledgeBase()
   │
   ├─► NPCKnowledgeSystem.InferRoleFromPersonality(m_PersonalityType)
   │       Returns: Merchant / Guard / Mage / Healer / Scholar / etc.
   │
   ├─► NPCPersonalities.GetLocationName(this)
   │       Returns: town/region name from NPC's map coordinates
   │
   ├─► NPCKnowledgeSystem.GetNPCKnowledge(role, locationName, location, map)
   │       Returns: List<LoreEntry> filtered by:
   │         - Role expertise (Expert/Proficient domains only)
   │         - Location-specific entries for this NPC's town
   │         - Top 25 by importance score
   │
   └─► NPCKnowledgeSystem.FormatKnowledgeForPrompt(m_KnowledgeBase)
           Returns: m_FormattedKnowledge (injected into every system prompt)
```

**Cache invalidation:** If an NPC moves more than 5 tiles (`OnLocationChange` → `HandleLocationChange`), the knowledge base is reloaded and the regional cache is invalidated. This handles wandering NPCs without constant recalculation. Small wandering within `RangeHome` (5–10 tiles) is ignored.

**Edge case — Internal map:** If the NPC spawns on `Map.Internal` (in transit), only role-based knowledge is loaded; location-based knowledge is deferred until the NPC is placed on a real map.

**Why proactive RAG?**
ServUO runs on the player's gaming PC. Embedding generation for per-query vector search adds 200–800ms per message and saturates the CPU during active combat. Pre-loading at spawn amortises this cost over the NPC's lifetime — typically hours or days. The formatted string is injected into the system prompt synchronously before the async LLM call, adding zero per-query latency.

---

### 2.3 Speech Pipeline & Request Queue

```
Player types message in chat
          │
          ▼
   LLMConversationHelper.OnSpeech()                [LLMConversationHelper.cs]
          │
          ├─ Check hearing range (≤ m_HearingRange tiles)
          ├─ Check name targeting (message contains NPC name, or NPC is last speaker)
          ├─ Anti-crosstalk: ignore if multiple LLM NPCs are nearby and this one isn't targeted
          │
          ▼
   LLMNpc.HandleSpeech(player, text)
          │
          ├─ Player already being processed? → EnqueueRequest()
          │       Queue size ≥ MaxQueueSize (5)? → discard silently
          │       Duplicate text within 30s (QueueTimeout)? → discard silently
          │
          └─ Otherwise → ProcessSpeechAsync(player, text)  [Task.Run, fire-and-forget]
                  │
                  ├─ LLMMemoryService.LoadMemories(npc.Serial, player.Name)
                  ├─ ConversationContext.GetHistory(npc.Serial, player.Serial)
                  ├─ Build system prompt (personality + knowledge + memories)
                  ├─ UnifiedLLMService.GetResponseAsync(...)
                  ├─ ChunkAndEmitResponse()
                  │     → Splits on '. ' and '?' sentence boundaries
                  │     → Timer emits one chunk every m_DelayBetweenChunks ms
                  ├─ ConversationContext.AddTurn(user, assistant)
                  ├─ Task.Run: ExtractAndSaveMemories()  [fire-and-forget]
                  └─ Dequeue and process next request if any
```

**Request queue design:**
Each NPC has its own `Queue<SpeechRequest>` and `object m_QueueLock`. The queue prevents message flooding and maintains FIFO ordering. Requests older than `QueueTimeout` (30s) are silently dropped when dequeued — players won't receive stale responses after walking away.

**Chunked response delivery:**
UO's chat system allows one line of ~100 characters per emission. Long LLM responses are split at sentence boundaries into `m_MaxChunkLength`-character chunks and emitted over a `Timer`, mimicking natural speech cadence and preventing wall-of-text chat spam.

---

### 2.4 Provider Routing (UnifiedLLMService)

`UnifiedLLMService` is a static routing layer with no persistent state. It selects a provider based on `RequestType`:

```csharp
public enum RequestType
{
    PlayerConversation,    // → OpenAI (quality required)
    QuestDialogue,         // → OpenAI (2000 token limit for structured JSON)
    NPCDecision,           // → Local/Ollama (latency sensitive)
    SimpleGreeting,        // → Local/Ollama (latency sensitive)
    AutonomousBehavior     // → Local/Ollama (latency sensitive)
}
```

**Selection logic (`ChooseProvider`):**
1. If `providerOverride != Auto` → use the override
2. If `preferLocal == true` (runtime toggle) → prefer Local for all types
3. Otherwise → map `RequestType` to provider per table above
4. If Local is selected but Ollama is unreachable → fall back to OpenAI

**Default state:**
```csharp
defaultProvider = LLMProvider.OpenAI;
preferLocal = false;
```
Both fields are logged at server startup. The GM command `[llmconfig` can toggle `preferLocal` at runtime without restarting.

---

### 2.5 OpenAI Integration (LLMService)

**File:** `Scripts/Services/LLM/Services/LLMService.cs` (592 lines)

| Setting | Value |
|---------|-------|
| Model | `gpt-4o-mini` |
| Endpoint | `https://api.openai.com/v1/chat/completions` |
| Auth | API key from `Config/LLM.cfg` |
| Token limit (normal) | 500 |
| Token limit (quest JSON) | 2000 |
| Temperature (conversation) | 0.8 |
| Temperature (quest) | 0.5 |
| Retry | Exponential backoff × 3 on HTTP 429 / 5xx |

**System prompt structure (per request):**
```
You are {npcName}, a {personalityDescription} in the world of Vystia.

WORLD KNOWLEDGE:
{m_FormattedKnowledge}    ← proactive RAG content injected here

MEMORIES OF {playerName}:
{memorySummary}

RELATIONSHIP: {relationshipLevel} ({interactionCount} interactions)

Respond in character. Keep responses concise (2-3 sentences).
Speech style: {speechPattern}
```

**Quest dialogue path (`GetResponseAsyncForQuest`):**
Uses 2000 max tokens and temperature 0.5 to produce well-formed JSON quest objects. A separate method exists because the parser downstream expects strict JSON — lower temperature reduces hallucinated structure.

**Conversation history:** The `conversationHistory` parameter (last 10 turns from `ConversationContext`) is passed as OpenAI `messages[]` entries with `user` / `assistant` roles, preceding the new user message. This gives GPT-4o-mini full session context within the UO conversation.

---

### 2.6 Local Inference (LocalLLMService)

**File:** `Scripts/Services/LLM/Services/LocalLLMService.cs` (1248 lines)

| Setting | Value |
|---------|-------|
| API | Ollama REST (`/api/chat`) |
| Default endpoint | `http://localhost:11434` |
| Default model | `phi3:mini` |
| Config | `Config/LocalLLM.cfg` |

**Available models:**

| Model | Parameters | VRAM | Latency | Use case |
|-------|-----------|------|---------|---------|
| `phi3:mini` | 3.8B | ~8GB | 1–2s | Default for background decisions |
| `llama3.1:8b` | 8B | ~16GB | 2–4s | Better reasoning, mid-range machines |
| `mistral:7b` | 7B | ~16GB | 2–4s | Alternative to llama |
| `llama3.1:70b` | 70B | 48GB+ | 10–30s | High-end machines only |

**Why Ollama for background tasks?**
Player machines are gaming rigs — RTX 4080/4090 with 16–24GB VRAM. Ollama runs natively on CUDA and serves phi3:mini in 1–2s at zero marginal cost, with no network round trip. This is used for: NPC-to-NPC social interactions, greetings from distant NPCs, autonomous patrol decision-making — tasks where quality is less critical than speed and cost.

**Health check:** `LocalLLMService` pings `/api/tags` on startup and caches availability. If Ollama is not running (common if the player hasn't started it), `ChooseProvider()` silently falls back to OpenAI for all request types.

---

### 2.7 Persistent Memory System

**Orchestrator:** `LLMMemoryService.cs` (301 lines)  
**Storage:** `SQLiteMemoryDatabase.cs` (546 lines) → `Data/llm_memories.db`  
**Fallback:** `InMemoryFallbackStore` (in-process, lost on server restart)

```
LLMMemoryService
    │
    ├─► SQLiteMemoryDatabase (primary)
    │       Tables:
    │         memories       (id, npc_serial, player_name, content, importance, timestamp)
    │         relationships  (npc_serial, player_name, level, interaction_count, last_seen)
    │         conversations  (id, npc_serial, player_name, role, content, timestamp)
    │
    └─► InMemoryFallbackStore (activated if SQLite unavailable)
```

**Key design choices:**

- **Keyed by `(npc_serial, player_name)`** not `(npc_name, player_name)`. NPC serial numbers are stable across server restarts (persisted in save files), while names can change. Multiple NPCs can share the same name (e.g., three "Town Guard" entries in different towns) — serial prevents memory bleed across them.
- **Max 50 memories per NPC**: LRU eviction drops the lowest-importance entry when the limit is reached.
- **Importance threshold = 3**: Only memories with `importance >= 3` (scale 1–10) are retrieved for prompt injection. Importance is assigned by a secondary LLM call during async extraction.
- **Relationship decay = 7 days**: `RelationshipDecayDays=7` — relationship level decreases if no interaction in 7 days, simulating social forgetting.
- **Async extraction**: `ExtractAndSaveMemories()` fires as a background `Task` after each conversation, using a secondary LLM call to extract and score memorable facts. This keeps the response path fast.

**Memory retrieval (per conversation turn):**
```
LoadPlayerMemories(npc.Serial, player.Name)
    → Query SQLite WHERE npc_serial=? AND player_name=? AND importance >= 3
    → ORDER BY importance DESC, timestamp DESC
    → LIMIT 10
    → Format as bullet list → injected into system prompt
```

---

### 2.8 Conversation Context (In-Memory)

**File:** `Core/ConversationContext.cs` (170 lines)

```csharp
private static Dictionary<string, PlayerConversation> m_Conversations;
// Key: $"{npc.Serial}_{player.Serial}"
```

Each `PlayerConversation` contains:
- `List<ConversationMessage>` — capped at last 10 turns (role + content)
- `DateTime LastActivity` — for 10-minute idle timeout

**Timeout cleanup:** A background `Timer` sweeps the dictionary every 5 minutes and removes entries where `LastActivity` is older than 10 minutes. Prevents unbounded memory growth on busy servers.

**Interaction with OpenAI:** The conversation list is passed directly as the `messages[]` array to the chat completions API, maintaining true multi-turn context within a session. At session end, memory extraction (§2.7) promotes salient facts from this ephemeral list into the durable SQLite store.

---

### 2.9 Response Caching

**File:** `Services/LLMResponseCache.cs` (591 lines)

Two-tier cache sits between `UnifiedLLMService` and the provider calls:

| Tier | Key | Eviction | Use |
|------|-----|----------|-----|
| Exact match | SHA256(npcName + full prompt) | TTL + LRU count | Identical repeated questions |
| Semantic match | Cosine similarity of keyword fingerprint | TTL + LRU count | Near-duplicate questions |

**Semantic cache hit threshold:** 0.85 similarity score. Below this, the request goes to the provider.

**Real-world hit rates:**
- High for static world-knowledge questions ("Where is the blacksmith?") — same answer regardless of who asks
- Low for personal/relationship-dependent questions ("Do you remember me?")

The cache reduces API costs significantly during server load testing and for common NPC questions that many players ask.

---

### 2.10 Personality System

**File:** `Data/NPCPersonalities.cs`

**20+ personality archetypes (enum `PersonalityType`):**
Merchant, Guard, Mage, Healer, Scholar, Blacksmith, Innkeeper, Farmer, Priest, Noble, Thief, Bard, Ranger, Alchemist, Necromancer, Paladin, Druid, Oracle, BountyHunter, Artificer, Monk, Templar, Summoner, Witch, Warlock, IceMage, Barbarian, Cleric, Knight

**4 speech patterns (`SpeechPattern`):**
| Pattern | Characteristics |
|---------|----------------|
| `Medieval` | forsooth, thee, thou, hath, dost |
| `Modern` | plain contemporary English |
| `Poetic` | rhyme/meter tendencies, lyrical phrasing |
| `Rustic` | contracted words, regional dialect, colloquial |

**Auto-inference on world load:** If a serialized NPC has no explicit personality set, `InferPersonalityFromName(npcName)` uses keyword matching on the NPC's name and title. The static counter `m_ReInferredCount` tracks how many NPCs required re-inference and is logged at startup.

**Regional hue assignment:** Personality type determines clothing colour based on the NPC's home region:

| Region | Hue | Personality examples |
|--------|-----|---------------------|
| Frosthold | 1150–1153 | Barbarian, IceMage, Beastmaster |
| Emberlands | 1358 | Sorcerer |
| Desert | 1719 | Ranger, Illusionist |
| Shadowfen | 2073 | Witch |
| ShadowVoid | 1109 | Warlock, Necromancer |
| Verdantpeak | 2010 | Druid, Alchemist |
| Crystal Barrens | 1154 | Wizard, Oracle |
| Ironclad | 2305 | Artificer, Fighter, Monk, Templar |
| Multi-Regional | 1153 | Cleric, Paladin, BountyHunter, Knight |

---

## 3. Lore & Knowledge Systems

### 3.1 Lore Domain Files

**Location:** `Scripts/Services/LLM/Data/Lore/`  
**Format:** JSON arrays of `LoreEntry` objects

| File | Entries | Content |
|------|---------|---------|
| `vystia_general.json` | — | World overview, geography, history |
| `religion_domain.json` | — | Gods, temples, rituals |
| `class_domain.json` | 25 | Player classes and abilities |
| `magic_domain.json` | 12 | Schools of magic |
| `creatures_domain.json` | 131 | Creature descriptions and behaviours |
| `equipment_domain.json` | — | Weapons, armour, enchantments |
| `npc_domain.json` | — | Notable NPCs, factions |
| `crafting_domain.json` | — | Professions, recipes |
| `combat_domain.json` | — | Combat rules, tactics |
| `healing_domain.json` | — | Medicine, potions, spells |
| `trade_domain.json` | — | Markets, currency, trade routes |
| `hospitality_domain.json` | — | Inns, taverns, food culture |
| `finance_domain.json` | — | Banking, guilds, taxes |
| `animal_domain.json` | — | Tameable animals, behaviour |
| `food_domain.json` | — | Cuisine, agriculture |
| `resource_domain.json` | — | Mining, forestry, materials |

**`LoreEntry` schema:**
```json
{
  "id": "magic_001",
  "category": "Magic",
  "subcategory": "Schools",
  "title": "School of Elementalism",
  "content": "Fire mages channel raw elemental energy...",
  "importance": 8,
  "relevantRoles": ["Mage", "Scholar", "Healer"],
  "tags": ["fire", "water", "earth", "air"]
}
```

`importance` ranges 1–10. `relevantRoles` gates which NPC types see this entry during proactive RAG selection.

---

### 3.2 SimpleLoreSystem vs VectorLoreSystem

**SimpleLoreSystem** (keyword matching, always available):
- Loads all 16 JSON files on startup into `List<LoreEntry>`
- `GetAllLore()` returns the full flat list — the data source for proactive RAG
- `SearchLore(query)` does case-insensitive keyword matching on title + content
- Zero infrastructure dependencies

**VectorLoreSystem** (semantic search, requires Ollama):
- Uses `nomic-embed-text` model via Ollama to generate 768-dim embeddings
- Builds an in-memory vector index of all lore entries at startup
- `SearchSemantic(query, topK)` returns entries by cosine similarity
- Falls back to `SimpleLoreSystem` if Ollama is unavailable
- Used for per-query retrieval when an NPC is asked about something outside its pre-loaded domain

The default path is proactive RAG (SimpleLoreSystem → NPCKnowledgeSystem → `m_FormattedKnowledge`). VectorLoreSystem activates only for reactive fallback queries.

---

### 3.3 NPCKnowledgeSystem (Proactive RAG Engine)

**File:** `Data/NPCKnowledgeSystem.cs` (470 lines)

Selection and formatting layer between lore JSON and the NPC system prompt.

**`GetNPCKnowledge(role, locationName, location, map)`:**
```
1. Load all lore via SimpleLoreSystem.GetAllLore()
2. Filter by role expertise:
   - Expert roles → all entries in that domain
   - Proficient roles → entries with importance >= 5 only
   - No relevance → skipped entirely
3. Add location-specific entries:
   - Any entry tagged with the NPC's town/region
   - Dungeon entries (category = "Dungeon")
   - High-importance location entries (importance >= 8)
4. Deduplicate by entry ID (GroupBy → First)
5. Sort by importance DESC
6. Take top 25
```

**`FormatKnowledgeForPrompt(entries)`:**
Produces a compact multi-line block injected into the system prompt:
```
WORLD KNOWLEDGE:
[Magic - Schools] School of Elementalism: Fire mages channel raw elemental energy...
[Trade - Routes] Ember Road: The primary trade route between Ironclad and Verdantpeak...
[Religion - Temples] Temple of Shiral: Located in the market district of Crossroads...
```

**Role expertise mapping (representative):**

| Role | Expert domains | Proficient domains |
|------|---------------|-------------------|
| Mage | magic, creatures | religion, combat |
| Merchant | trade, finance | crafting, hospitality |
| Healer | healing, religion | creatures, magic |
| Guard | combat, creatures | trade, local geography |
| Scholar | all (importance >= 5) | — |
| Innkeeper | hospitality, food | trade, local knowledge |
| Blacksmith | crafting, equipment | resource, combat |

---

## 4. AI Sidekick Architecture

### 4.1 Class Hierarchy

```
Server.Mobiles.Mobile
  └── Server.Mobiles.BaseCreature
        └── Server.Services.AISidekicks.BaseSidekick
              └── Server.Services.AISidekicks.AutonomousSidekick
                      ▲
                      │ controlled by
                      │
              Server.Services.AISidekicks.SidekickAI  (extends BaseAI)
                      │
                      └─ [one archetype module, selected at load time]
                              MageCombatAI / WarriorCombatAI / ArcherCombatAI
                              TamerCombatAI / HealerCombatAI / PaladinCombatAI
                              NecromancerCombatAI / ThiefCombatAI
                              PlayerLikeCombatAI  (generic fallback)
```

---

### 4.2 BaseSidekick

**File:** `Core/BaseSidekick.cs` (156 lines)

Minimal base class. Key additions over `BaseCreature`:

```csharp
public class BaseSidekick : BaseCreature
{
    public BaseSidekick(AIType ai, FightMode mode, ...) : base(...)
    {
        IsBonded = true;    // prevents deletion on death
    }
}
```

**`IsBonded = true`:** The critical death-handling flag. Standard UO creatures are deleted on death. Bonded pets instead enter `IsDeadBondedPet = true` state — they persist but cannot act. The owner can resurrect them with Veterinary skill or Chivalry spells. This allows persistent sidekick progression without losing creature state.

**AI type forcing:** ServUO caches AI type per creature class. `BaseSidekick` forces a cache bypass so `SidekickAI` is always selected regardless of the declared `AIType` field, preventing ServUO from substituting its default `HumanAI` or `MageAI`.

---

### 4.3 AutonomousSidekick

**File:** `Core/AutonomousSidekick.cs` (1696 lines)

The full companion implementation. Key systems:

**Identity and archetype fields:**
```csharp
public string SidekickName             // Persistent custom name
public SidekickArchetypeType ArchetypeType  // Warrior, Mage, Archer, Tamer...
public string CombatStyle              // "melee", "ranged", "magic"
public Mobile Owner                    // Owning player mobile
```

**LLM personality fields (data model exists, runtime-disabled):**
```csharp
public string LLMPersonality           // System prompt fragment
public string BackstoryText            // Narrative background
public bool UseLLMDialogue             // Feature flag — currently false
```
These fields are serialized so they survive server restarts. The infrastructure for LLM sidekick dialogue is built; the runtime hook is gated behind `UseLLMDialogue`.

**Archetype initialization (`InitializeArchetype`):**
Called on first creation. Assigns base stats, skills, and equipment:
- **Warrior** → plate armour, heater shield, weapon matching skill spread
- **Mage** → robe, spellbook loaded with memorized spells, reagents in backpack
- **Archer** → leather armour, composite bow, quiver of arrows
- **Tamer** → bonded pet reference stored in `m_BondedPet`, vet supplies
- **Paladin** → chain/plate armour, one-handed weapon + shield, chivalry spellbook
- **Necromancer** → dark robes, necromancy spellbook, reagents

**Serialization:** Full `Serialize`/`Deserialize` override persists all custom fields (equipment state, owner serial, archetype, goal state, LLM fields) to the ServUO save file. Sidekick state survives server restarts.

---

### 4.4 SidekickAI: Decision Controller

**File:** `AI/SidekickAI.cs` (3287 lines)

Extends `BaseAI`, replacing the standard creature AI tick with a custom decision loop. The `BaseAI` timer fires every ~250ms.

**Top-level decision tree (per tick):**
```
Think()
  │
  ├─ m_Sidekick null or deleted? → return false (deactivate AI)
  │
  ├─ Owner null, dead, or logged out?
  │     → Passive: stand idle, return true
  │
  ├─ Combatant != null (in combat)?
  │     → DoCombat()  ← delegates to active archetype module
  │
  ├─ Follow owner
  │     ├─ Distance > 2 tiles → MoveTo(Owner)
  │     └─ Stuck: m_ConsecutiveRetreatFailures > 3 → Teleport to owner
  │
  └─ Idle maintenance
        ├─ HP < threshold → use bandage/heal spell on self
        └─ Face owner direction
```

**`InitializeCombatAI()` (called after archetype load):**
```csharp
switch (archetypeType)
{
    case Warrior:     m_WarriorCombatAI = new WarriorCombatAI(m_Sidekick); break;
    case Mage:
    case Battlemage:
    case Druid:       m_MageCombatAI = new MageCombatAI(m_Sidekick); break;
    case Necromancer: m_NecromancerCombatAI = new NecromancerCombatAI(m_Sidekick); break;
    case Archer:
    case Ranger:      m_ArcherCombatAI = new ArcherCombatAI(m_Sidekick); break;
    case Thief:       m_ThiefCombatAI = new ThiefCombatAI(m_Sidekick); break;
    case Paladin:     m_PaladinCombatAI = new PaladinCombatAI(m_Sidekick); break;
    case Healer:      m_HealerCombatAI = new HealerCombatAI(m_Sidekick); break;
    case Tamer:       m_TamerCombatAI = new TamerCombatAI(m_Sidekick); break;
    default:          m_CombatAI = new PlayerLikeCombatAI(m_Sidekick); break;
}
```

Exactly one module is instantiated. The others remain `null` and are never allocated.

**Stuck detection:**
`m_ConsecutiveRetreatFailures` increments each tick that the sidekick fails to gain distance during a retreat (common in tight dungeon corridors). After 3 consecutive failures, the sidekick teleports to the owner rather than pathfinding indefinitely. `m_LastRetreatDistance` tracks the previous retreat tile to detect true stalls vs. legitimate slow progress.

---

### 4.5 Combat Archetype Modules

Each module exposes a `DoAction()` method called from `SidekickAI.DoCombat()`. Modules read from `AutonomousSidekick` skill values and equipment to make decisions.

**MageCombatAI** (most complex — kiting mage):

| Parameter | Default | Description |
|-----------|---------|-------------|
| `MIN_RETREAT_DISTANCE` | 4 | Retreat if enemy within X tiles |
| `SPELL_RELEASE_RANGE_MAX` | 14 | Maximum casting distance |
| `LOW_HEALTH` | 0.70 | HP% → begin self-healing |
| `CRITICAL_HEALTH` | 0.29 | HP% → emergency actions |

Decision flow:
1. If HP < `CRITICAL_HEALTH` → cast Greater Heal on self immediately
2. If enemy within `MIN_RETREAT_DISTANCE` → kite (move away while casting)
3. If enemy outside `SPELL_RELEASE_RANGE_MAX` → close distance to 10 tiles
4. Otherwise → select spell from combo rotation, cast if `m_NextCastTime` elapsed

Spell selection is HP-percentage-based: opens with damage spells (Fireball, Lightning), transitions to debuffs (Paralyze, Curse) as target weakens, finishes with damage-over-time (Poison) when target is near death.

**Parameter tuning via simulation:**
`Simulation/parameter_optimizer.py dual` runs a dual-objective optimization (win rate vs. HP remaining) over the four parameters above using headless Python combat simulations. This is how parameters were tuned without needing a live server session.

**WarriorCombatAI:**
- Maintains melee range (0–1 tiles)
- Tracks swing timer from UO's weapon speed formula; swings only when ready
- Below `LOW_HEALTH` → applies bandage via `Bandage.StartHealing()`
- Detects opponent swing timer for parry timing (ServUO's `BaseWeapon.GetDelay`)

**ArcherCombatAI:**
- Maintains 5–10 tile range; retreats if enemy closes to melee
- Tracks arrow count; equips from backpack when quiver depletes
- Leads target position for moving enemies (crude ballistic offset)

**TamerCombatAI:**
- Maintains reference to bonded pet (`m_BondedPet`)
- Issues `PetCommand.Attack` to pet when target is set
- Monitors pet HP; casts Veterinary spells below threshold
- Stays near pet rather than owner during combat

**HealerCombatAI:**
- Priority queue of heal targets ordered by HP deficit (owner > party > self)
- Will not attack enemies directly unless attacked (`AggressorList` check)
- Casts `GreaterHeal` when target HP < 40%, `Heal` when HP < 70%
- Falls back to bandages if spell mana is exhausted

**PaladinCombatAI:**
- Hybrid melee + Chivalry
- Primary rotation: `CloseWounds` (self-heal), `ConsecratWeapon` (damage bonus), `DivineFury`
- `Smite` (enemy damage debuff) on high-HP targets opening phase
- Melee attack fills gaps between Chivalry cast timer

**NecromancerCombatAI:**
- Mid-range (3–6 tiles); kites but less aggressively than mage
- Maintains `LichForm` for stat bonus when Necromancy >= 95
- Rotation: `PainSpike` → `CorpseSkin` (defence debuff) → `Wither` (AoE)
- Raises nearby corpses with `AnimateDead` when available (adds skeleton ally)

**ThiefCombatAI:**
- Attempts to flank (path to opponent's rear tile before each swing)
- Applies poison via `BasePoison.Level2` if weapon has poison attribute
- `StealthMove` when undetected; breaks stealth for first strike bonus

---

### 4.6 SidekickGoalSystem

**File:** `AI/SidekickGoalSystem.cs` (340 lines)

Goal-scoring layer for autonomous non-combat behaviour. Evaluates candidate goals every ~1s (not every 250ms tick) to avoid state thrashing.

**Goals and score contributors:**

| Goal | Base | Modifiers |
|------|------|-----------|
| `AttackEnemy` | 0 | +40 if owner is in combat, +20 per threatening enemy nearby |
| `FollowOwner` | 30 | +20 if distance > 5 tiles, −10 if already adjacent (distance ≤ 1) |
| `HealSelf` | 0 | +50 if HP < 50%, +70 if HP < 25% (overrides follow) |
| `Rest` | 0 | +20 if mana < 20% AND no enemies in range |

The highest-scoring goal is stored in `m_CurrentGoal` on `AutonomousSidekick` and remains active until:
- A different goal scores higher on next evaluation, OR
- The current goal reaches its terminal state (e.g., `HealSelf` completes when HP returns to full)

**Integration with `SidekickAI`:** When `SidekickAI.Think()` finds no `Combatant`, it calls `SidekickGoalSystem.EvaluateGoals(sidekick)` and executes the returned goal. Goals that involve movement (`FollowOwner`) delegate back to `SidekickAI`'s movement methods.

---

## 5. Data Flows

### 5.1 NPC Conversation Flow

```
┌─────────────────────────────────────────────────────────────────────┐
│                        UO Game Server (ServUO)                       │
│                                                                       │
│  Player types: "Aldric, where is the nearest healer?"               │
│        │                                                              │
│        ▼                                                              │
│  LLMConversationHelper.OnSpeech()                                    │
│    ├─ Hearing range check (within 10 tiles?)                         │
│    ├─ Name targeting ("Aldric" matches NPC name?)                    │
│    └─ Routes to LLMNpc.HandleSpeech(player, text)                   │
│        │                                                              │
│        ▼                                                              │
│  Request queue check                                                  │
│    ├─ Queue full (>= 5)? → discard silently                          │
│    ├─ Duplicate within 30s? → discard silently                       │
│    └─ Enqueue or begin processing immediately                        │
│        │                                          [fire-and-forget]  │
│        └────────────────────────────────────────────────────────►   │
│                                                                       │
│  ProcessSpeechAsync()                                                │
│    ├─ SQLite: LoadMemories(serial=4721, player="Marcus")             │
│    │     → ["Helped player find the forge", "Distrusts mages"]      │
│    ├─ ConversationContext: GetHistory(npc=4721, player=9823)         │
│    │     → [user: "hello", assistant: "Well met, traveller!"]       │
│    └─ Build system prompt                                            │
│          [personality: Blacksmith, speech: Medieval]                 │
│          [knowledge: crafting, equipment, trade, local geography]    │
│          [memories: 2 facts about Marcus]                            │
│          [relationship: Friendly, 3 interactions]                    │
└────────────────────────────────────────┬────────────────────────────┘
                                         │ HTTPS
                               ┌─────────▼──────────────┐
                               │     OpenAI API           │
                               │     gpt-4o-mini          │
                               │  /v1/chat/completions    │
                               │  max_tokens=500          │
                               │  temperature=0.8         │
                               └─────────┬──────────────-┘
                                         │
┌────────────────────────────────────────▼────────────────────────────┐
│                                                                       │
│  response: "Aye, the Temple of Shiral lies two roads east..."        │
│                                                                       │
│  ChunkAndEmitResponse()                                              │
│    → NPC says: "Aye, the Temple of Shiral lies two roads east..."   │
│    → [250ms delay]                                                   │
│    → NPC says: "...of the market square. Seek Sister Elowen."       │
│                                                                       │
│  ConversationContext.AddTurn(user, assistant)                        │
│                                                                       │
│  ExtractAndSaveMemories() [background Task]                          │
│    → LLM: extract facts from this conversation                       │
│    → SQLite: INSERT memory ("Player asked about healers", imp=4)    │
└─────────────────────────────────────────────────────────────────────┘
```

---

### 5.2 Sidekick Combat Decision Flow

```
ServUO AI Timer fires every ~250ms
          │
          ▼
SidekickAI.Think()
          │
          ├─[owner absent/dead]─────────────────────► Stand idle
          │
          ├─[Combatant != null]
          │        │
          │        ▼
          │  DoCombat()
          │        │
          │        ├─[Mage archetype]──► MageCombatAI.DoAction()
          │        │                          ├─ HP < 0.29? → Greater Heal
          │        │                          ├─ Enemy < 4 tiles? → kite + cast
          │        │                          └─ else → spell rotation
          │        │
          │        ├─[Warrior archetype]─► WarriorCombatAI.DoAction()
          │        │                          ├─ HP < 0.70? → bandage
          │        │                          └─ else → swing (weapon timer)
          │        │
          │        ├─[Tamer archetype]──► TamerCombatAI.DoAction()
          │        │                          ├─ Pet HP low? → vet spell
          │        │                          └─ else → PetCommand.Attack(target)
          │        │
          │        └─[other archetypes...]
          │
          └─[no Combatant]
                   │
                   ▼
            SidekickGoalSystem.EvaluateGoals()   (every ~1s)
                   │
                   ├─ Score all goals (Combat/Follow/Heal/Rest)
                   └─ Execute highest-score goal:
                         HealSelf → cast/bandage
                         FollowOwner → MoveTo(owner)
                         Rest → stand, regenerate
```

---

## 6. Configuration Reference

### `Config/LLM.cfg`
```ini
ApiKey=sk-proj-...
Model=gpt-4o-mini
MaxTokens=500
QuestMaxTokens=2000
Temperature=0.8
```

### `Config/LocalLLM.cfg`
```ini
Endpoint=http://localhost:11434
Model=phi3:mini
```

### `Config/LLMMemory.cfg`
```ini
LLMMemory.MemorySystem.Enabled=true
LLMMemory.MemoryDatabase.Provider=SQLite
LLMMemory.MemorySystem.MaxMemoriesPerNPC=50
LLMMemory.MemorySystem.MemoryImportanceThreshold=3
LLMMemory.MemorySystem.RelationshipDecayDays=7
```

### Runtime GM Commands
```
[llmconfig provider openai     # Force all requests to OpenAI
[llmconfig provider local      # Force all requests to Ollama
[llmconfig provider auto       # Restore automatic routing
[llmdebug on/off               # Toggle performance timing logs
[spawnvystia                   # GM gump: spawn AI NPCs, sidekicks
[st / [SpawnMage               # Spawn sidekick at cursor
```

### Key constants in source
| Constant | Value | Location |
|----------|-------|----------|
| `MaxQueueSize` | 5 | `LLMNpc.cs` |
| `QueueTimeout` | 30s | `LLMNpc.cs` |
| `ConversationHistoryMax` | 10 turns | `ConversationContext.cs` |
| `ConversationTimeout` | 10 min | `ConversationContext.cs` |
| `MaxMemoriesPerNPC` | 50 | `LLMMemoryService.cs` |
| `KnowledgeBaseMax` | 25 entries | `NPCKnowledgeSystem.cs` |
| `LocationChangeTrigger` | 5 tiles | `LLMNpc.cs` |
| `SemanticCacheThreshold` | 0.85 | `LLMResponseCache.cs` |
| `MageCombatAI.MIN_RETREAT_DISTANCE` | 4 | `MageCombatAI.cs` |
| `MageCombatAI.LOW_HEALTH` | 0.70 | `MageCombatAI.cs` |
| `MageCombatAI.CRITICAL_HEALTH` | 0.29 | `MageCombatAI.cs` |

---

## 7. Architectural Decisions & Tradeoffs

### 7.1 Proactive RAG vs Reactive RAG

**Decision:** Pre-load knowledge at NPC spawn, not per query.

**Why:** The deployment target is a player-hosted gaming PC running the UO server locally. Embedding generation for per-query vector search adds 200–800ms per message and saturates the CPU during combat. Pre-loading at spawn amortises this cost over the NPC's lifetime — typically hours or days. The formatted string is injected synchronously before the async LLM call, adding zero per-query overhead.

**Trade-off:** The NPC's knowledge snapshot is taken at spawn time. World events that occur after spawn are not reflected until the NPC is respawned or moves significantly (>5 tiles triggers a reload). For a static fantasy game world this is acceptable — lore doesn't change at runtime.

**Alternative present in codebase:** `VectorLoreSystem` with `nomic-embed-text` embeddings exists as a reactive fallback for queries outside an NPC's pre-loaded domain. Not the default path.

---

### 7.2 OpenAI for Players, Ollama for Background

**Decision:** Split by `RequestType`, not by NPC type.

**Why:** Player-facing conversations require GPT-4o-mini quality — nuanced in-character responses, accurate world knowledge, appropriate tone. Background decisions (patrol choices, inter-NPC greetings, autonomous behaviour) are invisible to players and don't justify API cost. Ollama/phi3:mini handles these at zero marginal cost with 1–2s latency, acceptable for background tasks.

**Why GPT-4o-mini specifically?** Evaluated:
- GPT-4o: too slow for interactive chat (3–8s P95)
- GPT-3.5-turbo: insufficient character depth, breaks persona
- Local llama3.1:8b: acceptable quality but 4–8s on an actively-loaded gaming GPU
- GPT-4o-mini: <2s P95 latency, strong role-playing, ~$0.001 per conversation

---

### 7.3 SQLite for Memory Storage

**Decision:** SQLite, not a vector database or Redis.

**Why:** The server runs on a gamer's PC. Adding a separate vector database process (Chroma, Qdrant) or a Redis instance adds deployment complexity for a single-player or small-group game. SQLite has zero infrastructure overhead, lives next to existing save files, and is sufficient for the query pattern (50 memories per NPC, filtered by importance score, no full-text search required).

**Trade-off:** Semantic memory search is not possible with SQLite alone. Memories are retrieved by `(npc_serial, player_name)` key and sorted by importance — not by relevance to the current query topic. A future enhancement could add embedding vectors to the `memories` table (pgvector-style) for relevance-ranked retrieval.

---

### 7.4 Rule-Based Sidekick Combat, Not LLM

**Decision:** Sidekick combat AI is deterministic rule-based, not LLM-driven.

**Why:** Combat decisions must resolve in <50ms per tick (250ms AI tick budget, potentially multiple sidekicks active simultaneously). LLM inference at 1–2s (local) or 300–500ms (cloud) makes the sidekick non-responsive in fast-paced UO combat. Rule-based archetype modules achieve sub-millisecond decision time.

**LLM infrastructure is built but gated:** The `UseLLMDialogue`, `LLMPersonality`, and `BackstoryText` fields on `AutonomousSidekick` are serialized and ready. When non-combat companion dialogue is added, the `SidekickAI` will route speech events to `LLMService` using `RequestType.AutonomousBehavior` (→ Ollama) for casual comments and `RequestType.PlayerConversation` (→ OpenAI) for story-significant dialogue.

---

### 7.5 Serial-keyed Memory

**Decision:** Key memories by `npc.Serial` (int), not `npc.Name` (string).

**Why:** Multiple NPCs can share the same name (three "Town Guard" in different towns). Name-keyed memory would merge their histories. ServUO serial numbers are unique per creature and stable across server restarts. 

**Trade-off:** If an NPC is deleted and re-spawned, it receives a new serial and its memories are orphaned (the old rows remain in SQLite until manual cleanup). This is acceptable — NPC re-spawning is a rare deliberate GM action, not a routine event.

---

*Source references: `ServUO/Scripts/Services/LLM/` and `ServUO/Scripts/Services/AISidekicks/`*
