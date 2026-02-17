# Core AI Architecture (Abstracted)

This document describes the core AI architecture in game-agnostic terms. It focuses on the actual loops, control flow, and system boundaries used for LLM-driven NPCs and autonomous sidekicks.

## LLM NPC Architecture

### 1. Capability Contract
- LLM-capable NPCs implement the `ILLMConversational` interface with:
  - `LLMConversationEnabled` toggle
  - `PersonalityType` and `SpeechPattern`
  - `HearingRange`
  - `ShouldHandleConversation(...)` and `HandleConversation(...)` methods

### 2. Unified Conversation Pipeline
- ALL conversation processing goes through `LLMConversationHelper`:
  - LLMNpc delegates to the helper (no duplicate internal processing)
  - BaseVendor, Mage, Actor, QuestNPC all delegate to the helper
  - Single source of truth for queueing, throttling, and context assembly

### 3. Queueing + Throttling
- Per-NPC conversation state managed by the helper:
  - Request queue with size limits (5 requests max)
  - Processing lock to prevent concurrent requests per player
  - Request timeouts (30 seconds) and active conversation tracking (2 minutes)
  - Crosstalk avoidance when multiple LLM NPCs are nearby

### 4. Context Assembly
The response prompt is assembled from:
- Personality prompt (archetype + speech pattern)
- Proactively loaded knowledge base (cached per NPC)
- Conversation history (last 10 messages, 10-minute timeout)
- Memory and relationship context (if available, NPC-specific by serial)

### 5. Provider Routing (Ollama-First)
- `UnifiedLLMService` routes requests with Ollama-first policy:
  - Default: Auto with preferLocal=true (tries Ollama first, falls back to OpenAI)
  - Quest dialogue: Always OpenAI (higher quality requirements)
  - Fallback on error: Automatically switches to alternate provider

### 6. Response Post-Processing
- Structured vendor action parsing:
  - Primary: JSON schema (`{"action": "buy|sell", "reason": "...", "metadata": {...}}`)
  - Fallback: Legacy tag parsing (`[vendor_buy]`, `[vendor_sell]`)
  - Strips JSON blocks and meta-commentary from display text
- Vendor UX special cases:
  - SELL action with no sellable items → opens sell gump directly (no LLM speech)

### 7. Memory System (Unified)
- All NPCs use `LLMMemoryService` with SQLite persistence:
  - Memories keyed by (npc_serial, player_name) for NPC-specific recall
  - Automatic memory extraction from conversations
  - Relationship tracking with defensive validation
  - In-memory fallback if SQLite unavailable

### 8. Knowledge Base (Unified)
- Proactive loading at spawn time via `LLMConversationHelper.ProactivelyLoadKnowledgeBase()`:
  - Role-based knowledge + location-specific lore
  - Shared per-NPC cache (no duplicate knowledge storage)
  - Lazy loading fallback for NPCs without proactive initialization
  - Reload on personality re-inference or location changes

### 9. System Boundaries
- Global plugin (`LLMConversationPlugin`) can disable all LLM conversations
- Per-NPC enable/disable via `ILLMConversational.LLMConversationEnabled`
- Background services handle cache cleanup and memory housekeeping

## LLM NPC Loop (Unified Pipeline)
1. Speech event arrives at NPC.
2. NPC delegates to `ILLMConversational.ShouldHandleConversation()` → `LLMConversationHelper`.
3. Helper validates: enabled state, range, name targeting, active conversations.
4. Request enqueued in per-NPC queue (throttling prevents duplicates).
5. `ProcessSpeechAsync()` assembles context:
   - Personality prompt + speech pattern
   - Cached knowledge base (proactively loaded or lazy-loaded)
   - Conversation history + memory/relationship context
6. `UnifiedLLMService` routes to provider (Ollama-first with fallback).
7. LLM response parsed for structured vendor actions (JSON → tags fallback).
8. Response delivered (chunked if long) + vendor actions executed.
9. Memory extraction and relationship update (async, non-blocking).

---

## Sidekick AI Architecture

### 1. Core Structure
- Sidekicks are full autonomous companions implemented as controlled entities.
- They inherit from the core creature class to gain command compatibility.
- AI is driven by the base AI timer and state machine.

### 2. AI Brain
- A custom AI controller (`SidekickAI`) extends the base AI and runs the decision loop.
- Archetype-specific combat AI modules are initialized once the archetype is known.
- Combat modules include mage, warrior, healer, necromancer, tamer, etc.

### 3. Command Layer
- Standard control commands (follow, stay, attack, guard, release) are honored.
- Owner relationship is set through the control master and control order.

### 4. Combat Loop
- For each AI tick:
  - Determine current state (combat vs follow vs idle).
  - If combat: select target and delegate to the archetype-specific combat module.
  - If support archetype: prioritize healing/positioning instead of direct combat.
  - Use stuck detection to adapt distance and positioning.

### 5. Recovery Loop
- When combat ends:
  - Check for remaining aggressors; if found, continue combat.
  - Otherwise, clear combat state and return to follow owner.
  - Trigger recovery behaviors (healing, mana regen, etc.).

### 6. Persistence + Death Handling
- Sidekicks serialize like other controlled entities.
- Bonded-pet semantics prevent deletion on death and allow resurrection.

## Sidekick AI Loop (High-Level)
1. AI tick fires.
2. State evaluation (combat / follow / idle).
3. If combat:
   - Acquire target.
   - Delegate to archetype combat AI.
   - Apply movement, cast, heal, retreat decisions.
4. If not combat:
   - Follow owner and perform maintenance behaviors.
5. Persist state via serialization and bonded-death handling.

---

## Usage Examples

### Creating an LLM-Enabled NPC
```csharp
public class MyCustomNpc : BaseCreature, ILLMConversational
{
    // ILLMConversational implementation
    public bool LLMConversationEnabled { get; set; } = true;
    public NPCPersonalities.PersonalityType PersonalityType { get; set; } = NPCPersonalities.PersonalityType.Commoner;
    public NPCPersonalities.SpeechPattern SpeechPattern { get; set; } = NPCPersonalities.SpeechPattern.Modern;
    public int HearingRange { get; set; } = 10;

    public bool ShouldHandleConversation(SpeechEventArgs e) => 
        LLMConversationHelper.ShouldHandleConversation(this, e.Mobile, e.Speech);
    
    public void HandleConversation(SpeechEventArgs e) => 
        LLMConversationHelper.ProcessConversation(this, e.Mobile, e.Speech);

    // Optional: Proactive knowledge loading for faster startup
    public override void OnSpawn()
    {
        base.OnSpawn();
        LLMConversationHelper.ProactivelyLoadKnowledgeBase(this, this);
    }
}
```

### Creating an LLM Vendor
```csharp
public class MyVendor : BaseVendor, ILLMConversational
{
    // ILLMConversational implementation (same as above)
    // Vendor actions automatically detected from LLM responses:
    // - JSON: {"action": "buy"} or {"action": "sell"}
    // - Tags: [vendor_buy] or [vendor_sell]
}
```

### Structured Vendor Action Response
LLMs can return structured JSON for reliable vendor intent detection:
```json
{
  "action": "sell",
  "reason": "Player wants to sell their loot",
  "metadata": {
    "items": ["sword", "armor"],
    "confidence": 0.95
  }
}
```

---

## Integration Points

### Quest System Integration
- A quest waypoint condition can depend on recruiting a sidekick.
- The sidekick system triggers a quest hook on recruitment, completing that waypoint.

### LLM + Sidekick Integration
- Sidekicks contain LLM personality fields, but LLM conversation is currently disabled.
- The LLM conversation helper can be reused if sidekick LLM is re-enabled.

---

## Notes
- This document abstracts out all game-specific naming and mechanics.
- It preserves the real control flow and system dependencies used by the codebase.
