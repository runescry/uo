# LLM Dynamic Quest Generation (No-GM) — Spec & Implementation Path

**Status:** Phase 1 MVP — Infrastructure Complete, LLM Planner Pending  
**Scope:** Generate **DynamicQuests** and **questlines** via LLM with **temporary spawns** and **per-player/per-party instances**.  
**Source of truth:** The LLM is trained on Vystia lore; the server provides **hard constraints + POI mapping** to ensure outputs are spawnable and non-hallucinated.

---

## Quick Status Summary

**Phase:** Phase 1 MVP — ✅ **COMPLETE**  
**Status:** Production Ready (with LLM integration)  
**Testable:** Yes — `[GenLLMQuest]` works with LLM generation  
**Player Access:** Yes — `Chronicler` NPC available for players

**Key Metrics:**
- ✅ 9/9 core components implemented
- ✅ 20 POIs registered
- ✅ 194 NPC templates registered
- ✅ Full pipeline tested end-to-end
- ✅ LLM planner wired to `UnifiedLLMService`
- ✅ All 10 bosses allowlisted
- ✅ NPC template avatar spawning implemented
- ✅ Player entry point (Chronicler NPC) created

---

## Current Implementation Status (2025-12-12)

### ✅ Phase 1 MVP — Infrastructure Complete

**Completed Components:**

1. **✅ Plan Models & JSON Parsing** (`DynamicQuestPlanModels.cs`)
   - Full JSON schema for `DynamicQuestPlan` and `DynamicQuestWaypointPlan`
   - Serialization/deserialization via `DynamicQuestPlanJson`
   - Supports all waypoint types and conditions

2. **✅ POI Registry** (`VystiaPoiRegistry.cs` + `Data/Vystia/poi_registry.json`)
   - 20 POIs registered covering all major Vystia regions
   - POI lookup by `poiId` with map/X/Y/Z/radius
   - Tags for filtering and LLM context

3. **✅ NPC Template Registry** (`Data/Vystia/npc_templates.json`)
   - **194 entries** including:
     - 20 Faction Leaders (all implemented)
     - 14 Ancient Beings
     - 10 Bosses
     - 3 Quest Givers
     - 18 Vendors
     - 25 Trainers
     - 121 Regional Creatures
   - All entries have `mobileTypeName` matching C# class names

4. **✅ Validator** (`QuestPlanValidator.cs`)
   - Hard validation for POI references
   - Allowlist for waypoint conditions and types
   - Boss type allowlist (expandable)
   - Safety checks for bounds and content

5. **✅ Compiler & Spawner** (`QuestPlanCompilerAndSpawner.cs`)
   - Converts validated plan → `DynamicQuest` + `QuestWaypoint`s
   - Spawns `QuestNPC`s with LLM context
   - Spawns boss creatures (allowlisted types)
   - Links NPCs to waypoints
   - Marks quests as ephemeral

6. **✅ Instance Tracker** (`GeneratedQuestInstanceAttachment.cs`)
   - `XmlAttachment` for per-player/per-party tracking
   - Tracks quest ID, spawned serials, expiry time
   - Persists across server restarts

7. **✅ Cleanup System** (`GeneratedQuestCleanup.cs`)
   - Timer-based cleanup (runs every 2 minutes)
   - Deletes expired spawned NPCs/bosses
   - Unregisters ephemeral quests
   - Handles party disband scenarios

8. **✅ Orchestration Service** (`LLMQuestGenerationService.cs`)
   - End-to-end pipeline: JSON → Validate → Compile → Spawn → Track → Offer
   - Modular design (swappable validators/compilers)
   - Demo plan generator for testing

9. **✅ GM Command** (`GenLLMQuestCommand.cs`)
   - `[GenLLMQuest <poiId>]` for testing
   - Uses demo plan (2-waypoint quest: TalkToNPC → DefeatBoss)

**What Works Now:**
- ✅ Full pipeline from JSON plan to playable quest
- ✅ Temporary spawns with automatic cleanup
- ✅ POI-based location grounding
- ✅ NPC template selection (via registry)
- ✅ Per-player instance tracking
- ✅ Quest offer gump integration

**What's Complete:**
- ✅ **LLM Planner Integration** — `GeneratePlanJsonAsync()` wired to `UnifiedLLMService`
- ✅ **NPC Template Avatar Spawning** — Spawns Vystia NPC avatars from registry
- ✅ **Expanded Boss Allowlist** — All 10 bosses allowlisted
- ✅ **Player Entry Point** — `Chronicler` NPC created for player-accessible quest generation

**What's Missing (Future Phases):**
- ❌ **Questline Support** — Only single quests work (Phase 2)
- ❌ **Party Instance Sharing** — Currently per-player only (Phase 2)

---

## Next Steps

### ✅ Phase 1 Complete (2025-12-12)
1. ✅ **LLM Planner Wired** — `GeneratePlanJsonAsync()` connects to `UnifiedLLMService`
2. ✅ **NPC Template Spawning** — `VystiaNpcTemplateRegistry` loads templates, compiler spawns avatars
3. ✅ **Boss Allowlist Expanded** — All 10 bosses added to validator
4. ✅ **Player Entry Point** — `Chronicler` NPC created in `NPCs/QuestGivers/Chronicler.cs`

**How to Use:**
- **GM Testing:** `[GenLLMQuest [theme]]` - Generates quest using LLM (or demo plan if POI ID provided)
- **Player Access:** Spawn `Chronicler` NPC, say "quest" or "adventure" to generate a personalized quest

### Phase 2 (Questlines)
5. **Multi-Quest Arc Support** — `DynamicQuestlinePlan` implementation
6. **Arc Progress Tracking** — Track completion across quest chain
7. **Auto-Offer Next Quest** — When previous quest completes

### Phase 3 (World Events)
8. **Scheduled Regional Events** — Time-based quest generation
9. **Shared Event Seeds** — Reduce LLM calls for popular events

---

## Goals
- **No GM involvement** for day-to-day quest creation.
- Generate **questlines** (3–7 quests) or **single quests** (quick jobs).
- Spawn required **QuestNPCs / Vystia NPC templates** and **boss creatures** automatically.
- Use existing quest infrastructure:
  - `DynamicQuest`, `QuestWaypoint`, `VystiaQuestSystem`, waypoint detectors, `QuestNPC` LLM context.
- Ensure generated objectives are:
  - grounded in **known POIs** (Vystia names mapped to standard UO map coords)
  - safe and balanced (anti-exploit, no player-kill objectives, bounded rewards).

---

## Key requirements (confirmed)
- **Spawn lifetime**: **temporary** (cleaned up after expiry / abandonment).
- **Instance model**: **per-player / per-party** (no global shared arcs by default).
- **POIs**: you will provide `X/Y` for POIs; the system uses **Vystia names** but resolves them to standard UO maps/locations (e.g., “Ironclad” → Britain).
- **NPC access**: LLM may choose from **all Vystia NPCs** (leaders, ancient beings, etc.), implemented as **template-driven “avatars”** (not moving canonical world NPCs).
- **Modular & expandable**: must support new waypoint conditions, new NPC template packs, new reward policies, new planners (OpenAI/local), and new world event modes later.

---

## Architecture (modular pipeline)

### 1) Planner (LLM → structured plan)
**Component:** `ILLMQuestPlanner`  
Input:
- Player context (class, tier, region, party size)
- Optional theme request (e.g., “Ice Island”, “Shadowfen mystery”)
- Allowed constraints (POI ids, allowed creature types, allowed waypoint conditions)
Output:
- **Strict JSON**: `DynamicQuestPlan` (single quest) or `DynamicQuestlinePlan` (multi-quest)

LLM prompt rules:
- Must reference POIs by **`poiId`** from a provided list (or `poiName` from a canonical mapping list).
- Must select NPCs from **template ids** (leaders, ancient beings, etc.) or request a generic QuestNPC.
- Must populate per-waypoint:
  - `NPCDialogueContext` (what to say at this step)
  - `PlayerLocationHint` (short, cool, non-numeric hint language)

### 2) Validator (server-side hard validation)
**Component:** `IQuestPlanValidator`
- Reject plans that:
  - reference unknown POIs / maps
  - request disallowed NPC/boss types
  - exceed bounds (waypoints, rewards, spawn counts)
  - contain unsafe instructions (e.g., “kill players”, slurs, exploit loops)
- Optionally auto-correct minor issues (e.g., missing radius) and re-validate.

### 3) Compiler (plan → `DynamicQuest` objects)
**Component:** `IQuestPlanCompiler`
- Converts plan → `DynamicQuest` + `QuestWaypoint`s
- Registers quest (as **ephemeral**) via `DynamicQuestManager.RegisterDynamicQuest`
- Sets:
  - waypoint objective keys
  - conditions (`TalkToNPC`, `ReachLocation`, `DefeatBoss`, etc.)
  - per-waypoint `NPCDialogueContext`, `PlayerLocationHint`

### 4) Spawner (temporary NPC/boss spawns)
**Component:** `IQuestInstanceSpawner`
- Spawns:
  - origin NPC (quest giver)
  - waypoint NPCs
  - boss creature for `DefeatBoss`
- Uses:
  - `QuestNPC` for generic + LLM-driven quest roles
  - optional **Vystia NPC template avatars** (leaders/ancients) when requested
- Links NPCs to waypoints (`QuestNPC.LinkToWaypoint(questId, waypointId)`).

### 5) Instance tracker + cleanup (per-player/per-party)
**Component:** `GeneratedQuestInstanceAttachment` (XmlSpawner2 `XmlAttachment`)
- Stored on:
  - party leader (preferred) or each player (fallback)
- Tracks:
  - `questId`
  - party members covered
  - spawned serials (NPCs/boss)
  - expiry time
- Cleanup rules:
  - On expiry: delete spawned mobiles + unregister quest
  - If party disbands: keep for leader or convert to solo instance
  - If server restart: cleanup still works (attachment + ephemeral quest skip-save)

---

## Data requirements: POI + NPC template registries

### POI Registry

**Purpose:** hard grounding. LLM can say “Ironclad” (Vystia), but the server must spawn at **real UO coordinates**.

**Storage (proposed):** `Data/Vystia/poi_registry.json`  
**Lookup key:** `poiId` (stable id); also supports `poiName` (display).

Minimum fields:
- `poiId` (string, unique, stable)
- `poiName` (string, Vystia-facing name)
- `map` (string, e.g. `Felucca`, `Trammel`, `Ilshenar`, etc.)
- `x`, `y`, `z` (int; `z` optional if you want auto-grounding)
- `radius` (int; used for `ReachLocation`)
- `tags` (string[]; optional; e.g. `["town","ironclad","britain"]`)

Example:

```json
[
  {
    "poiId": "IRONCLAD_BRITAIN_CENTER",
    "poiName": "Ironclad (Britain)",
    "map": "Felucca",
    "x": 1496,
    "y": 1629,
    "z": 20,
    "radius": 20,
    "tags": ["ironclad","britain","town","hub"]
  }
]
```

### NPC Template Registry (Vystia NPC access)

**Goal:** “Players should have access to all Vystia NPCs” without moving canonical world actors.

**Approach:** template-driven **avatars**:
- The generator chooses a `templateId`
- The server spawns an avatar with:
  - type (optional, if you have a specific Mobile class)
  - name/title
  - LLM personality/speech defaults
  - equipment pack
  - knowledge tags (for RAG, optional)

**Storage (proposed):** `Data/Vystia/npc_templates.json`

Example:

```json
[
  {
    "templateId": "FACTION_LEADER_IRONCLAD",
    "displayName": "Ironclad Faction Leader",
    "mobileTypeName": "QuestNPC",
    "name": "High Marshal Kael",
    "title": "Leader of Ironclad",
    "defaultPersonality": "Authoritative",
    "defaultSpeechPattern": "Formal",
    "knowledgeTags": ["ironclad","leadership","war","britain"]
  }
]
```

---

## Plan schema (LLM output contract)

### `DynamicQuestPlan` (single quest)
Required:
- `title`, `description`, `tier`
- `expiresMinutes` (for temporary instances)
- `waypoints[]` (must include: Origin + ≥1 objective + Completion)

Waypoint fields:
- `type`: `Origin | Waypoint | BossCompletion | NPCCompletion`
- `condition`: `TalkToNPC | ReachLocation | DefeatBoss | CollectItems | RecruitSidekick | Custom`
- `poiId` (required for any location-based spawn; may be omitted for “spawn at player”)
- `npcTemplateId` (optional; if omitted, spawn generic `QuestNPC`)
- `bossTypeName` (for `DefeatBoss`, allowlisted)
- `npcDialogueContext`, `playerLocationHint`

### `DynamicQuestlinePlan` (multi-quest)
- `arcTitle`, `arcSummary`
- `quests[]` (each is a `DynamicQuestPlan`)
- `progressionRules` (linear by default)

---

## Player/Party instance semantics

### Instance ownership
- If player is in a party: **party leader owns the instance** (attachment stored on leader).
- All party members may accept the quest:
  - starting the quest calls `VystiaQuestSystem.StartQuest` per player
  - but **spawns are shared** (one set of NPCs/boss for the party instance)

### Expiry / cleanup
- Default expiry: 60–240 minutes (configurable per tier).
- Cleanup triggers:
  - expiry time reached
  - party abandoned (no member has the quest active) for N minutes
  - explicit abandon command (future)
- Cleanup actions:
  - delete spawned NPCs/bosses
  - unregister the ephemeral `DynamicQuest`

### Persistence rules
- Instances persist across server restarts via **XmlAttachment** state.
- Generated quests are marked **ephemeral** and **excluded from `Data/VystiaQuests.xml`** to avoid world bloat.

---

## Safety / abuse prevention
- **Allowlists** for:
  - boss types
  - item rewards
  - NPC templates usable by generation
- **Hard bounds**:
  - waypoints per quest, quests per arc
  - spawn count per instance
  - reward caps per tier
- **Cooldowns**:
  - per player/party generation cooldown
  - per POI spawn density (optional)
- **Content filters**:
  - reject profanity / harassment / player-kill objectives
  - reject “infinite farm loops”

---

## Implementation path (recommended phases)

### Phase 1 — MVP “single quest per player/party”
- Add:
  - plan models + JSON parsing (`Newtonsoft.Json`)
  - POI registry loader
  - validator
  - compiler + spawner
  - instance attachment + cleanup timer
- Add a **single entrypoint**:
  - dev command `[GenLLMQuest <theme?>]` (GM) for rapid iteration
  - production entry: a “Chronicler / Quest Board” NPC (player accessible)

### Phase 2 — Questlines
- Generate 3–7 quests in an arc.
- Track arc progress in attachment.
- Offer next quest automatically when previous quest completes.

### Phase 3 — World events (optional)
- Scheduled arcs per region.
- Per-party instancing still supported; “event seed” can be shared to reduce LLM calls.

---

## Integration points (existing code)
- **Quest data**: `Server.Custom.VystiaClasses.Quests.DynamicQuest`
- **Progress tracking**: `VystiaQuestSystem` + `VystiaQuestTracker` attachment
- **NPC dialogue**: `QuestNPC.BuildQuestAwareContext()` and per-waypoint fields
- **Boss kill detection**: `QuestKillWaypointDetector`
- **Quest log**: `XMLQuestLogGump`


