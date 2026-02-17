# Core AI Architecture (Abstracted)

This document describes the core AI architecture in game-agnostic terms. It focuses on the actual loops, control flow, and system boundaries used for LLM-driven NPCs and autonomous sidekicks.

## LLM NPC Architecture

### 1. Capability Contract
- LLM-capable NPCs implement a conversation interface with:
  - `LLMConversationEnabled` toggle
  - `PersonalityType` and `SpeechPattern`
  - `HearingRange`
  - `ShouldHandleConversation(...)` and `HandleConversation(...)`

### 2. Conversation Routing
- Speech events are routed through a shared helper that:
  - Verifies capability + enabled state
  - Verifies hearing range
  - Resolves which NPC should respond using name targeting and active-conversation state
  - Avoids crosstalk when multiple LLM NPCs are nearby

### 3. Queueing + Throttling
- Each NPC has a per-NPC queue and processing lock.
- If a player is already being processed, duplicate requests are dropped.
- Queue size and timeouts enforce backpressure and prevent stalls.

### 4. Context Assembly
- The response prompt is assembled from:
  - Personality prompt (archetype + speech pattern)
  - Local knowledge base (preloaded RAG)
  - Conversation history
  - Optional memory/relationship context

### 5. Provider Routing
- A unified service selects the provider based on request type:
  - High-quality (player conversation / quest dialogue) -> cloud provider
  - Fast/low-stakes -> local provider
- Supports fallback to alternate provider on error.

### 6. Response Post-Processing
- Filters/strips meta commentary when required.
- Detects intent tags (e.g., vendor actions) and triggers corresponding actions.
- Sends final response to the player and persists memory.

### 7. System Boundaries
- Global plugin flag can disable LLM NPC conversations entirely.
- Background services run for cache cleanup and memory housekeeping.

## LLM NPC Loop (High-Level)
1. Speech event arrives.
2. `ShouldHandleConversation(...)` gating passes.
3. Request enqueued for NPC.
4. Context assembled (personality, history, RAG, memory).
5. Provider selected and LLM called.
6. Response filtered and delivered.
7. Memory/relationship updated.

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
