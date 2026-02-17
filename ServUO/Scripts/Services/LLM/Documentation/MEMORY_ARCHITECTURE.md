# LLM Memory System Architecture

The memory system enables NPCs to remember players, build relationships, and recall past interactions across server sessions.

---

## Overview

The memory system uses a **two-tier architecture** with SQLite for persistence:

```
┌─────────────────────────────────────────────────────────────┐
│                     PLAYER INTERACTION                       │
└─────────────────────┬───────────────────────────────────────┘
                      │
                      ▼
┌─────────────────────────────────────────────────────────────┐
│                  TIER 1: IN-MEMORY CACHE                     │
│  - Active conversation history (last 10 messages)            │
│  - Request queue and processing state                        │
│  - Timeout: 2 minutes conversation, 10 minutes history       │
│  - Speed: Instant access                                     │
│  - Storage: RAM only                                         │
└─────────────────────┬───────────────────────────────────────┘
                      │
                      ▼
┌─────────────────────────────────────────────────────────────┐
│                 TIER 2: SQLITE DATABASE                      │
│  - All memories (permanent storage)                          │
│  - All relationships (complete history)                      │
│  - Conversation history (full sessions)                      │
│  - Persistence: Forever (file-based)                        │
│  - Speed: 5-20ms access (direct file I/O)                   │
│  - Storage: Data/llm_memories.db                             │
└─────────────────────────────────────────────────────────────┘
```

**Key Benefits:**
- ✅ **Zero External Dependencies** - No database server required
- ✅ **Simple Deployment** - Single `.db` file
- ✅ **Fast Performance** - Direct file access, no network overhead
- ✅ **Persistent Storage** - Data survives server restarts
- ✅ **Easy Backup** - Just copy the `.db` file

---

## Data Models

### Memory

Represents a single memory that an NPC has about a player.

```csharp
public class Memory
{
    public int Id { get; set; }                    // Unique identifier
    public string NpcName { get; set; }            // NPC who has this memory
    public string PlayerName { get; set; }         // Player this memory is about
    public MemoryType Type { get; set; }           // Type of memory
    public string Content { get; set; }            // Memory description
    public int Importance { get; set; }            // 1-10 importance score
    public Dictionary<string, object> Context { get; set; }  // Additional metadata
    public DateTime CreatedAt { get; set; }        // When memory was created
    public DateTime LastAccessed { get; set; }     // Last time memory was recalled
    public DateTime? ExpiresAt { get; set; }       // Optional expiration
}
```

### Memory Types

```csharp
public enum MemoryType
{
    Conversation,   // Dialogue exchanges
    Event,          // Significant events
    Fact,           // Learned facts about player
    Preference,     // Player preferences
    Relationship    // Relationship changes
}
```

### Relationship

Tracks the relationship between an NPC and player.

```csharp
public class Relationship
{
    public int Id { get; set; }                    // Unique identifier
    public string NpcName { get; set; }            // NPC in relationship
    public string PlayerName { get; set; }         // Player in relationship
    public RelationshipType Type { get; set; }     // Relationship type
    public int Score { get; set; }                 // -100 to +100
    public string Summary { get; set; }            // Text summary
    public DateTime FirstMet { get; set; }         // First interaction
    public DateTime LastInteraction { get; set; }  // Last interaction
    public int InteractionCount { get; set; }      // Total interactions
}
```

### Relationship Types

```csharp
public enum RelationshipType
{
    Stranger,       // Never met or minimal interaction (score: -10 to 10)
    Acquaintance,   // Some familiarity (score: 11 to 40)
    Friend,         // Friendly relationship (score: 41 to 60)
    CloseFriend,    // Close bond (score: 61 to 80)
    Ally,           // Strong alliance (score: 81 to 100)
    Enemy,          // Hostile (score: -100 to -11)
    Rival,          // Competitive (varies)
}
```

---

## Storage Layers

### Layer 1: In-Memory (ConversationContext)

**File**: `Core/ConversationContext.cs`

**Purpose**: Fast access to active conversation history

**Implementation**:
```csharp
// Key format: "npcSerial_playerSerial"
Dictionary<string, ConversationHistory> m_Conversations

public class ConversationHistory
{
    public List<ConversationMessage> Messages { get; set; }  // Last 10 messages
    public DateTime LastActivity { get; set; }
}
```

**Lifecycle**:
- Created when conversation starts
- Updated on each message
- Cleaned up after 10 minutes of inactivity
- Lost on server restart (acceptable for active conversations)

**Performance**: Instant (RAM access)

---

### Layer 2: SQLite Database (SQLiteMemoryDatabase)

**File**: `Services/SQLiteMemoryDatabase.cs`

**Purpose**: Permanent storage of all memories and relationships

**Database Location**: `ServUO/Data/llm_memories.db` (auto-created)

**Schema**:
```sql
-- Memories table
CREATE TABLE llm_npc_memories (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    npc_serial INTEGER NOT NULL,
    npc_name TEXT NOT NULL,
    player_name TEXT NOT NULL,
    memory_type TEXT NOT NULL,
    content TEXT NOT NULL,
    importance INTEGER NOT NULL DEFAULT 5,
    context TEXT,
    created_at TEXT NOT NULL,
    last_accessed TEXT NOT NULL,
    expires_at TEXT
);

CREATE INDEX idx_llm_npc_memories_npc_player 
    ON llm_npc_memories (npc_serial, player_name);

-- Relationships table
CREATE TABLE llm_relationships (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    npc_serial INTEGER NOT NULL,
    npc_name TEXT NOT NULL,
    player_name TEXT NOT NULL,
    relationship_type TEXT NOT NULL,
    relationship_score INTEGER NOT NULL DEFAULT 0,
    summary TEXT,
    first_met TEXT NOT NULL,
    last_interaction TEXT NOT NULL,
    interaction_count INTEGER NOT NULL DEFAULT 0,
    UNIQUE(npc_serial, player_name)
);

-- Conversations table
CREATE TABLE llm_conversations (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    npc_name TEXT NOT NULL,
    player_name TEXT NOT NULL,
    session_id TEXT NOT NULL,
    message TEXT NOT NULL,
    is_player_message INTEGER NOT NULL,
    timestamp TEXT NOT NULL
);
```

**Features**:
- Automatic schema creation on first initialization
- Direct file I/O (no network overhead)
- Async operations (non-blocking)
- Importance-based queries
- Time-range filtering
- Batch operations for efficiency

**Performance**: 5-20ms per query (file-based, very fast)

**Configuration**:
```
Config/LLMMemory.cfg:
LLMMemory.MemoryDatabase.Provider=SQLite
LLMMemory.MemoryDatabase.ConnectionString=  # Optional, defaults to Data/llm_memories.db
```

---

## Memory Flow

### Saving a Memory

```
Player talks to NPC
    │
    ▼
Conversation processed
    │
    ▼
Memory extracted (MemoryHelpers)
    │
    ├─→ Importance scored (1-10)
    │
    ▼
LLMMemoryService.SaveMemoryAsync()
    │
    └─→ Write to SQLite (async, direct)
        │
        └─→ Update last_accessed timestamp
```

### Loading Memories

```
NPC needs memories for conversation
    │
    ▼
LLMMemoryService.GetMemoriesAsync()
    │
    └─→ Load from SQLite
        │
        ├─→ Query by npc_serial + player_name
        │
        ├─→ Sort by importance DESC
        │
        ├─→ Limit to top 10
        │
        └─→ Update last_accessed timestamps
```

**No caching layer needed** - SQLite is fast enough for direct access!

---

## Memory Extraction

### Automatic Extraction

**File**: `Services/MemoryHelpers.cs`

**Process**:
```csharp
After each conversation:
    1. Analyze conversation history
    2. Extract significant facts/events
    3. Calculate importance (1-10)
    4. Create Memory objects
    5. Save asynchronously (non-blocking)
```

**Importance Scoring**:
```csharp
// Examples of importance scores:
Quest accepted/completed: 9-10
Combat/danger mentioned: 7-8
Personal information shared: 6-7
Item traded: 5-6
General conversation: 3-4
Small talk: 1-2
```

**Extraction Rules**:
- **Conversation**: Every 5th exchange
- **Event**: Quest starts, combat, trades
- **Fact**: Player reveals information
- **Preference**: Player likes/dislikes
- **Relationship**: Reputation change

---

## Relationship Management

### Reputation Changes

**Common Modifiers**:
```csharp
Helped in combat: +15
Gave valuable gift: +10
Pleasant conversation: +5
Completed quest: +20
Ignored greeting: -3
Rude behavior: -10
Attacked NPC: -50
Attacked NPC's friend: -30
Killed NPC's friend: -100
Saved NPC's life: +25
Regular customer: +2 (per visit)
Fair trade: +5
Unfair trade: -15
```

### Relationship Decay

**Configuration**:
```
LLMMemory.MemorySystem.RelationshipDecayEnabled=true
LLMMemory.MemorySystem.RelationshipDecayDays=7
LLMMemory.MemorySystem.RelationshipDecayAmount=1
```

**Process**:
- If no interaction for `RelationshipDecayDays`, decay `RelationshipDecayAmount` points
- Prevents permanent relationships from single interaction
- Decays toward 0 (neutral)

### Relationship Types by Score

```csharp
Score >= 81:  Ally
Score >= 61:  CloseFriend
Score >= 41:  Friend
Score >= 21:  Acquaintance
Score >= -10: Stranger
Score >= -60: Unfriendly (not yet implemented as enum value)
Score >= -80: Enemy
Score <= -81: Nemesis (not yet implemented)
```

---

## In-Memory Fallback

**File**: `Services/InMemoryFallbackStore.cs`

**Purpose**: Emergency storage when SQLite unavailable

**Features**:
- Dictionary-based storage
- TTL expiration
- Statistics tracking
- Thread-safe operations

**Limitations**:
- Lost on server restart
- No persistence
- Limited to RAM capacity

**Usage**:
```csharp
// Automatically used when:
if (!SQLiteMemoryDatabase.IsAvailable())
{
    // Falls back to InMemoryFallbackStore
    InMemoryFallbackStore.Activate();
}
```

**When Fallback Activates**:
- SQLite DLLs missing
- Database file locked
- Disk full
- Permissions issue

---

## Memory Integration with Conversations

### Context Building

When NPC converses with player:

```csharp
// 1. Load recent memories (top 10 by importance)
var memories = await LLMMemoryService.GetMemoriesAsync(npcName, playerName, 10);

// 2. Load relationship
var relationship = await LLMMemoryService.GetRelationshipAsync(npcName, playerName);

// 3. Build context for LLM
string context = $@"
You are {npcName}.

Your relationship with {playerName}:
- Type: {relationship.Type}
- Reputation: {relationship.Score}/100
- First met: {relationship.FirstMet}
- Total interactions: {relationship.InteractionCount}

Things you remember about {playerName}:
";

foreach (var memory in memories.OrderByDescending(m => m.Importance))
{
    context += $"- [{memory.Type}] {memory.Content} (importance: {memory.Importance}/10)\n";
}

// 4. Add to LLM prompt
// Now NPC knows the player and their history!
```

**Example Context**:
```
You are Gareth the Blacksmith.

Your relationship with Runescry:
- Type: Friend
- Reputation: 52/100
- First met: 2025-01-10
- Total interactions: 8

Things you remember about Runescry:
- [Quest] Runescry accepted my quest to gather iron ore (importance: 9/10)
- [Event] Runescry helped defend the town from orcs (importance: 8/10)
- [Fact] Runescry is interested in learning blacksmithing (importance: 7/10)
- [Conversation] Discussed the properties of valorite (importance: 5/10)
- [Preference] Runescry prefers longswords over maces (importance: 4/10)
```

---

## Configuration

### LLMMemory.cfg

```ini
# Enable/Disable Memory System
LLMMemory.MemorySystem.Enabled=true

# SQLite Database
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

# Batch Operations
LLMMemory.MemoryDatabase.BatchSize=100
LLMMemory.MemoryDatabase.FlushIntervalSeconds=10
```

---

## Performance Considerations

### Memory Usage

**Per NPC with active conversations:**
- In-memory conversation: ~10 KB (10 messages)
- SQLite database: ~50 KB (100 memories + relationship + conversation history)

**100 NPCs**: ~5 MB RAM, ~5 MB database file

### Query Performance

**SQLite Direct Access**:
- Latency: 5-20ms (file I/O)
- Throughput: 1,000+ ops/sec
- No network overhead
- Optimized with indexes

### Optimization Tips

1. **Limit memory queries**: Top 10 by importance only
2. **Batch writes**: Use batch operations for multiple saves
3. **Index properly**: SQLite automatically indexes (npc_serial, player_name)
4. **Clean old data**: Periodic cleanup of low-importance memories
5. **Regular VACUUM**: SQLite can benefit from periodic VACUUM (not yet implemented)

---

## Testing

### Memory System Tests

**Command**: `[MemoryStats` (if available)

**Manual Testing**:
```
1. Spawn NPC:
[SpawnPersonalityNPC Blacksmith Archaic

2. Talk to NPC multiple times

3. Check database file exists:
Data/llm_memories.db

4. Restart server

5. Talk to NPC again - should remember you!
```

---

## Troubleshooting

### Memories Not Saving

**Check**:
1. `Config/LLMMemory.cfg` - `Enabled=true`
2. SQLite DLLs present: `System.Data.SQLite.dll`, `SQLite.Interop.dll`
3. Console for database errors
4. Database file permissions
5. Disk space available

### Memories Not Loading

**Check**:
1. SQLite database file exists: `Data/llm_memories.db`
2. Console for query errors
3. Memory importance scores (might be too low)
4. Database file integrity

### High Memory Usage

**Solutions**:
1. Reduce conversation history
2. Limit memories per NPC: `MaxMemoriesPerNPC=50`
3. Clean up old, low-importance memories
4. Enable auto-cleanup

### Slow Queries

**Solutions**:
1. Verify SQLite indexes exist (auto-created)
2. Reduce `MaxMemoriesPerNPC`
3. Check disk I/O performance
4. Consider SSD for database file location

### SQLite DLL Issues

**Symptoms**: "SQLite not available" in logs

**Solutions**:
1. Ensure `System.Data.SQLite.dll` and `SQLite.Interop.dll` are in ServUO root
2. Check DLL architecture matches (x64 vs x86)
3. Run `build-and-copy.ps1` to copy DLLs automatically
4. See [SQLITE_DLL_REQUIREMENTS.md](SQLITE_DLL_REQUIREMENTS.md)

---

## Backup and Maintenance

### Backup

**Simple**: Just copy the database file
```powershell
Copy-Item Data\llm_memories.db Data\llm_memories.db.backup
```

**Recommended**: Regular backups before server updates

### Maintenance

**Auto Cleanup**: Enabled by default
- Removes low-importance memories
- Runs every 24 hours (configurable)
- Respects `MaxMemoriesPerNPC` limit

**Manual Cleanup**: (Not yet implemented)
- SQLite VACUUM command
- Rebuild indexes
- Analyze query performance

---

## Future Enhancements

### Planned Features

**Memory Consolidation**:
- Merge similar memories
- Summarize long conversation chains
- Keep only most important details

**Shared Memories**:
- NPCs share knowledge with each other
- Gossip system
- Faction-wide awareness

**Emotional Context**:
- Track emotional state during memory creation
- Memories tied to emotions (fear, joy, anger)
- Emotional influence on relationships

**Performance Optimizations**:
- In-memory cache layer (optional)
- Query result caching
- Batch operation improvements

---

## Best Practices

1. **Enable Auto Cleanup**: Prevents database bloat
2. **Set Appropriate Limits**: Balance memory quality vs quantity
3. **Monitor Database Size**: Check `Data/llm_memories.db` file size
4. **Regular Backups**: Before major updates
5. **Test After Changes**: Verify memories persist after restart
6. **Check Logs**: Monitor for SQLite errors

---

---

**Related Documentation:**
- [README.md](README.md) - Main documentation
- [00_INDEX.md](00_INDEX.md) - Documentation index
- [SQLITE_DLL_REQUIREMENTS.md](SQLITE_DLL_REQUIREMENTS.md) - SQLite DLL setup
- [RAG_LORE_ARCHITECTURE.md](RAG_LORE_ARCHITECTURE.md) - Knowledge system
