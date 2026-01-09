# Vystia Multi-Chain Quest System - Complete Implementation

**Status:** ✅ **Active** (2025-12-12)
**Build Status:** ✅ All code compiles successfully
**Note:** This document describes the **current wizard-first workflow** and core systems. Older legacy gumps are considered **retired** (no GM commands registered).

---

## Overview

The Vystia Quest System is a GM-friendly quest creation + management system featuring:

- **Multi-chain quests** with sequential waypoints
- **LLM-powered quest NPCs** with context-aware dialogue
- **Quest Wizard** (step-by-step) for creating quests without coding
- **Inline QuestNPC spawning** for TalkToNPC steps (no detours to other gumps)
- **Waypoint detection**:
  - ReachLocation (movement)
  - TalkToNPC (dialogue)
  - DefeatBoss (kill event)
- **Sample Quest generator** (one-click demo quest, fully wired)
- **XML persistence** - all quests save to `Data/VystiaQuests.xml`

---

## Architecture

### Core Components (Current)

| Component | File | LOC | Purpose |
|-----------|------|-----|---------|
| **Waypoint Data** | QuestWaypoint.cs | - | Waypoint structure with XML + world serialization |
| **Dynamic Quest + Manager** | DynamicQuest.cs | - | Runtime-editable quests, registry, XML persistence |
| **Quest System** | VystiaQuestSystem.cs | - | Quest tracking, completion, waypoint helpers |
| **Quest NPC** | QuestNPC.cs | - | LLM quest NPC with keyword handling + waypoint integration |
| **Quest Wizard** | VystiaQuestWizardGump.cs | - | Primary GM UX (step-by-step quest building) |
| **Picker + Confirm** | VystiaQuestWizardPickerGump.cs / VystiaQuestWizardConfirmGump.cs | - | “Dropdown-like” selection + confirmations |
| **Movement Detector** | QuestWaypointDetector.cs | - | Auto-complete ReachLocation waypoints |
| **Kill Detector** | QuestKillWaypointDetector.cs | - | Auto-complete DefeatBoss waypoints |
| **GM Commands** | AddQuestNPCCommand.cs | - | `[aqn]` routes into the wizard’s NPC-linking step |

**Total:** ~3,033 LOC

---

## Waypoint Types

### WaypointType Enum
```csharp
public enum WaypointType
{
    Origin,         // Starting point - where player accepts quest
    Waypoint,       // Intermediate step
    BossCompletion, // Defeat a boss to complete
    NPCCompletion   // Talk to final NPC to complete
}
```

### WaypointCondition Enum
```csharp
public enum WaypointCondition
{
    TalkToNPC,      // Must speak to assigned NPC
    ReachLocation,  // Must enter region/coordinates
    DefeatBoss,     // Must kill specific creature
    CollectItems,   // Must gather items
    CastSpell,      // Must cast specific spell
    Custom,         // Custom objective key
    RecruitSidekick // Recruit an AI sidekick companion (optional archetype match)
}
```

### Recruit Sidekick Waypoint (NEW)

- **Use case**: “Recruit a companion before continuing.”
- **How to configure**:
  - Set waypoint **Condition** to `RecruitSidekick`
  - Optionally set **TargetTypeName** to a sidekick archetype (example: `Mage`, `Healer`, `Warrior`). If blank, **any** recruited sidekick completes it.
- **How it completes**:
  - When the player creates (or is assigned) a sidekick via the AISidekicks system, the quest system auto-completes the current `RecruitSidekick` waypoint if it matches.

---

## QuestWaypoint Data Structure

### Properties

| Property | Type | Purpose |
|----------|------|---------|
| `WaypointID` | int | Unique ID within quest |
| `Name` | string | Display name |
| `Description` | string | Player-facing description |
| `Type` | WaypointType | Origin/Waypoint/Completion |
| `Condition` | WaypointCondition | How to complete |
| `Location` | Point3D | Map coordinates |
| `Map` | Map | Which map (Felucca/Trammel) |
| `Radius` | int | Detection radius for location |
| `RegionName` | string | Alternate: region-based detection |
| `AssignedNPCSerial` | Serial | Linked NPC |
| `NPCTypeName` | string | NPC class name |
| `NPCDialogueContext` | string | LLM context for NPC |
| `ObjectiveKey` | string | Progress tracking key |
| `RequiredAmount` | int | How many to complete |
| `OrderIndex` | int | Sequential order (0, 1, 2...) |
| `IsOptional` | bool | Can be skipped? |
| `LLMPersonality` | string | Personality type |
| `LLMSpeechPattern` | string | Speech pattern |

### Serialization

- **XML:** `SaveXml()` / `LoadXml()` for quest data files
- **Binary:** `Serialize()` / `Deserialize()` for world saves

---

## DynamicQuest Extensions

### New Methods

```csharp
// Waypoint management
public void AddWaypoint(QuestWaypoint waypoint)
public void RemoveWaypoint(int waypointId)
public QuestWaypoint GetWaypoint(int waypointId)
public QuestWaypoint GetWaypointByOrder(int orderIndex)

// Waypoint queries
public QuestWaypoint GetOrigin()
public QuestWaypoint GetCompletion()
public IEnumerable<QuestWaypoint> GetIntermediateWaypoints()
public QuestWaypoint GetCurrentWaypoint(QuestProgress progress)

// Waypoint status
public bool IsWaypointComplete(QuestWaypoint waypoint, QuestProgress progress)

// Waypoint ordering
public void MoveWaypointUp(int waypointId)
public void MoveWaypointDown(int waypointId)
public void ClearWaypoints()
```

### DynamicQuestManager Extensions

```csharp
public static DynamicQuest GetQuest(int questId)
public static DynamicQuest GetQuestByTitle(string title)
```

---

## QuestNPC - LLM Integration

### Key Features

1. **Implements ILLMConversational** - Full LLM dialogue support
2. **Quest-Aware Context** - Builds context from quest + player progress
3. **Automatic Waypoint Completion** - Completes TalkToNPC waypoints on greeting
4. **Keyword Responses** - Handles accept/complete/quest status
5. **Serialization** - Persists quest/waypoint links across restarts

### Context Building

```csharp
public string BuildQuestAwareContext(PlayerMobile pm)
```

Generates context like:
```
You are Elder Frostweave, the Quest Giver.
You are assigned to the quest "Ice Mage Initiation".
This is an Origin waypoint named "Quest Start".
Waypoint purpose: Talk to this NPC to begin the quest.

The player has NOT started this quest.
You should offer the quest to them.

Quest description: Prove your mastery of ice magic by completing...
```

### Keyword Handling

| Keyword | Action |
|---------|--------|
| accept, yes, begin | Start quest (Origin waypoints) |
| complete, done, finished | Complete quest (Completion waypoints) |
| quest, task, job | Show quest status |
| hello, hail, greetings | Complete TalkToNPC waypoints |

---

## Quest Creation UX (Wizard-First)

The original Quest Editor worked, but felt **clunky** because it exposed multiple unrelated modes/buttons.

The default GM workflow is a **single step-by-step wizard** with **Next/Back** navigation and **“dropdown-like” pickers** (UO gumps don’t have native dropdown controls).

### Commands

| Command | Purpose |
|--------|---------|
| `[QE]` / `[QuestEditor]` | **Opens the Quest Wizard** (recommended) |
| `[QW]` / `[QuestWizard]` | Opens the Quest Wizard |
| `[aqn]` / `[addquestNPC]` | Opens the Quest Wizard directly into **Quest NPC linking** |

### Quest Wizard Steps

1. **Select/Create Quest**
2. **Quest Details**
3. **Origin Waypoint**
4. **Intermediate Waypoints** (add as many as you want)
5. **Completion Waypoint**
6. **Objectives** (optional)
7. **Rewards** (optional)
8. **NPCs** (optional: spawn/link QuestNPCs for TalkToNPC waypoints)

### One-Click Demo

On Step 1 (quest list), use:
- **Create Sample Quest (Demo)**: creates a fully wired quest, spawns/links 3 TalkToNPC QuestNPCs, and adds a kill-creature finish.

---

## Legacy Notes

- The older multi-mode quest editor + standalone NPC wizard are **retired** (no registered GM command entrypoints).
- All supported GM workflows go through the **Quest Wizard**.

## Waypoint Detector

### Purpose
Auto-detect when players enter location-based waypoints and mark them complete.

### Detection Method
- Hooks into `EventSink.Movement`
- Throttled to check every 2 seconds
- Checks all active quests for player
- Only processes `WaypointCondition.ReachLocation` waypoints

### Detection Logic

**By Region:**
```csharp
if (!string.IsNullOrEmpty(waypoint.RegionName))
{
    // Check if player's region matches
    if (pm.Region.Name.Contains(waypoint.RegionName, StringComparison.OrdinalIgnoreCase))
        return true;
}
```

**By Radius:**
```csharp
if (waypoint.Location != Point3D.Zero)
{
    double distance = pm.GetDistanceToSqrt(waypoint.Location);
    return distance <= waypoint.Radius;
}
```

### GM Tools

```csharp
QuestWaypointDetector.ForceCheck(pm);         // Force immediate check
QuestWaypointDetector.GetDebugInfo(pm);       // Get debug info string
```

---

## VystiaQuestSystem Extensions

### New Methods

```csharp
// Check all objectives including waypoints
public static bool AreAllObjectivesComplete(VystiaQuest quest, QuestProgress progress)

// Get current waypoint for player
public static QuestWaypoint GetCurrentWaypoint(PlayerMobile pm, int questID)

// Check waypoint completion
public static bool IsWaypointComplete(PlayerMobile pm, int questID, int waypointID)

// Complete waypoint
public static void CompleteWaypoint(PlayerMobile pm, int questID, int waypointID)
```

### Waypoint Progress Tracking

Waypoints use special objective keys:
```csharp
string key = waypoint.GetObjectiveKeyForProgress();
// Returns: "waypoint_1", "waypoint_2", etc.
```

Progress is tracked via `QuestProgress.GetProgress("waypoint_1")`
Required amount is always 1 for waypoints.

---

## Quest Flow Example

### Creating a Multi-Chain Quest

1. GM uses `[QE]` to open Quest Editor
2. Creates "Ice Mage Initiation" quest
3. Clicks **[Waypoints]** button
4. Clicks **[Add Origin]** → Creates "Quest Start" waypoint
5. Clicks **[Edit]** on waypoint:
   - Name: "Speak to Elder Frostweave"
   - Type: Origin
   - Condition: TalkToNPC
   - Description: "Talk to the elder to begin your journey"
   - LLM Context: "You are the wise elder of the ice mages..."
6. Clicks **[Add Waypoint]** → Creates intermediate waypoint
7. Edits waypoint:
   - Name: "Reach the Frozen Caves"
   - Type: Waypoint
   - Condition: ReachLocation
   - Sets location to cave entrance
   - Radius: 10
8. Clicks **[Add Waypoint]** → Creates second intermediate
9. Edits waypoint:
   - Name: "Defeat Frost Giant"
   - Type: Waypoint
   - Condition: DefeatBoss
   - Target: FrostGiant
   - Required: 1
10. Clicks **[Add Finish]** → Creates completion waypoint
11. Edits waypoint:
    - Name: "Return to Elder"
    - Type: NPCCompletion
    - Condition: TalkToNPC

### Spawning Quest NPCs

1. GM stands at town center
2. Uses `[aqn]`
3. Selects "Ice Mage Initiation" quest
4. Selects "Speak to Elder Frostweave" waypoint
5. Configures NPC:
   - Name: "Elder Frostweave"
   - Title: "the Ice Sage"
   - Personality: Sage
   - Speech: Formal
6. Clicks **[SPAWN NPC]**
7. Elder spawns at GM location

8. GM teleports to cave entrance
9. Uses `[aqn]` again
10. Selects same quest
11. Selects "Return to Elder" waypoint (completion)
12. Configures different personality if needed
13. Spawns completion NPC

### Player Experience

1. Player approaches Elder Frostweave
2. Says "Hello"
3. Elder: "Hail, PlayerName. I am Elder Frostweave. What brings you before me?"
4. Player says "quest"
5. Elder: "I have a quest for you: Ice Mage Initiation"
6. Elder: "Prove your mastery of ice magic by completing these trials."
7. Elder: "Say 'accept' if you wish to begin."
8. Player says "accept"
9. Quest starts! Visual effect plays.
10. Player travels to Frozen Caves
11. **Automatically completes** "Reach the Frozen Caves" waypoint when entering radius
12. Message: "Location reached: Reach the Frozen Caves"
13. Player kills Frost Giant
14. (Boss kill logic completes "Defeat Frost Giant" waypoint)
15. Player returns to Elder
16. Says "complete"
17. Elder: "Well done! You have completed the quest. Here is your reward."
18. Quest completes, rewards given!

---

## Serialization & Persistence

### Quest Data Files

**Location:** `D:\UO\ServUO\Data\VystiaQuests.xml`

**Format:**
```xml
<?xml version="1.0"?>
<VystiaQuests>
  <Quest id="1">
    <Title>Ice Mage Initiation</Title>
    <Description>Prove your mastery...</Description>
    <RequiredClass>2</RequiredClass>
    <PrerequisiteQuestID>0</PrerequisiteQuestID>
    <Tier>0</Tier>

    <Objectives>
      <Objective key="kill_frostgiant" amount="1" display="Defeat Frost Giant" />
    </Objectives>

    <Rewards>
      <GoldReward amount="500" />
      <SkillReward skill="Cryomancy" amount="5.0" />
    </Rewards>

    <Waypoints>
      <Waypoint id="1" order="0" type="0" condition="0">
        <Name>Quest Start</Name>
        <Description>Talk to the elder</Description>
        <Location x="1234" y="5678" z="0" map="Felucca" radius="5" />
        <NPC serial="123456" typeName="QuestNPC">
          <DialogueContext>You are the wise elder...</DialogueContext>
          <Personality>Sage</Personality>
          <SpeechPattern>Formal</SpeechPattern>
        </NPC>
        <Objective key="waypoint_1" amount="1" targetType="" optional="false" />
      </Waypoint>
      <!-- More waypoints... -->
    </Waypoints>
  </Quest>
</VystiaQuests>
```

### Auto-Save

Quests are automatically saved:
- When quests/waypoints are edited in the Quest Wizard
- When QuestNPCs are spawned/linked in the Quest Wizard
- On world save (EventSink.WorldSave)

### Manual Save

```csharp
DynamicQuestManager.Save();
```

---

## GM Commands Reference

| Command | Shortcut | Purpose |
|---------|----------|---------|
| `[QuestWizard]` | `[QW]` | Open Quest Wizard |
| `[QuestEditor]` | `[QE]` | Alias for Quest Wizard |
| `[addquestNPC]` | `[aqn]` | Jump into Quest Wizard’s **Quest NPC linking** step |

---

## Integration Points

### With Existing Systems

1. **VystiaQuestSystem** - Core quest tracking
2. **PlayerMobile** - Quest progress attachment
3. **LLM System** - ILLMConversational interface
4. **EventSink** - Movement detection for locations
5. **Serialization** - World saves and XML persistence

### With Future Systems

- **Boss Encounters** - Kill detection for `DefeatBoss` waypoints is implemented (EventSink.OnKilledBy)
- **Item Collection** - `CollectItems` condition ready
- **Spell Casting** - `CastSpell` condition ready
- **Custom Objectives** - `Custom` condition for scripted objectives

---

## Technical Notes

### Namespace Conflict Resolution

ServUO has a built-in `Server.WaypointType` enum that conflicts with our `Server.Custom.VystiaClasses.Quests.WaypointType`.

**Solution:** Always use fully-qualified names:
```csharp
Server.Custom.VystiaClasses.Quests.WaypointType.Origin
```

### Serial Storage

Waypoints store NPC serials as `Serial` type (struct, not class):
```csharp
public Serial AssignedNPCSerial { get; set; }
```

**Check if linked:**
```csharp
if (waypoint.AssignedNPCSerial.Value != -1)
    // NPC is linked
```

**Set to unlinked:**
```csharp
waypoint.AssignedNPCSerial = -1;
```

### C# 7.3 Compatibility

ServUO uses C# 7.3, which doesn't support:
- Switch expressions (`switch { }`)
- Recursive patterns

**Use traditional switch instead:**
```csharp
// ❌ Don't use:
int color = type switch { WaypointType.Origin => 0x00FF00, _ => 0xFFFFFF };

// ✅ Use:
int color;
switch (type)
{
    case WaypointType.Origin:
        color = 0x00FF00;
        break;
    default:
        color = 0xFFFFFF;
        break;
}
```

---

## Testing Checklist

### Quest Creation
- [ ] Create quest via Quest Wizard
- [ ] Add Origin waypoint
- [ ] Add intermediate waypoints
- [ ] Add completion waypoint
- [ ] Save and verify XML file created

### NPC Spawning
- [ ] Spawn/link QuestNPC at Origin (TalkToNPC)
- [ ] Spawn/link QuestNPC for at least 1 intermediate TalkToNPC step

### Quest Flow
- [ ] Player talks to Origin NPC
- [ ] Player accepts quest
- [ ] Quest starts, visual effects play
- [ ] Player completes waypoints in order
- [ ] Location waypoints auto-complete
- [ ] Talk waypoints complete on dialogue
- [ ] DefeatBoss waypoints complete on kill

### Persistence
- [ ] Create quest and NPCs
- [ ] Restart server
- [ ] Verify quest data loads
- [ ] Verify NPCs still linked
- [ ] Verify player progress persists

---

## Known Limitations

1. **LLM Context Injection** - Currently uses LLMConversationHelper.ProcessConversation without custom context injection. Full LLM context support requires LLM helper updates.

2. **Item Collection** - `CollectItems` condition requires backpack scanning logic (not yet implemented).

3. **Spell Casting** - `CastSpell` condition requires spell cast event hooks (not yet implemented).

4. **Quest Offer UI** - Quest acceptance currently uses keyword dialogue (“quest/accept”). A dedicated “quest offer gump” is not implemented yet.

5. **Quest Chaining UI (Player)** - No dedicated player quest log / chain UI (text-based only).

---

## Future Enhancements

### Short Term
1. Add item collection detection
2. Add spell cast detection
3. Enhance LLM context injection

### Medium Term
1. Quest chain visualization gump for players
2. Quest log UI with waypoint progress
3. Map markers for waypoint locations
4. Quest completion statistics

### Long Term
1. Branching quest paths (choice-based)
2. Dynamic quest generation via LLM
3. Quest achievements and titles
4. Guild/party quest sharing

---

## File Locations

```
ServUO/Scripts/Custom/VystiaClasses/
├── Quests/
│   ├── QuestWaypoint.cs              - Waypoint data structure
│   ├── DynamicQuest.cs               - DynamicQuest + DynamicQuestManager (XML persistence)
│   ├── QuestNPC.cs                   - Quest-linked LLM NPC
│   ├── QuestWaypointDetector.cs      - ReachLocation detection (movement)
│   ├── QuestKillWaypointDetector.cs  - DefeatBoss detection (kill event)
│   └── VystiaQuestSystem.cs          - Quest tracking + helpers
├── Gumps/
│   ├── VystiaQuestWizardGump.cs      - Primary GM UX
│   ├── VystiaQuestWizardPickerGump.cs - Picker (“dropdown-like”) gump
│   ├── VystiaQuestWizardConfirmGump.cs - Confirm dialogs (delete quest)
│   └── VystiaNPCSpawnPickerGump.cs   - GM “spawn Vystia NPC” picker (world content)
└── Commands/
    └── AddQuestNPCCommand.cs         - [aqn] routes into wizard’s NPC-linking step
```

**Data:**
```
ServUO/Data/
└── VystiaQuests.xml                  - Quest persistence
```

---

## Build Status

✅ **All code compiles successfully**

**Note:** If you see "Build FAILED" with error MSB3027, it means ServUO is running and has locked Scripts.dll. This is expected. Stop the server, rebuild, then restart.

---

## Changelog

**2025-12-12:**
- ✅ Initial implementation complete
- ✅ All 8 files created/modified
- ✅ Build errors resolved
- ✅ Documentation written

---

*Quest System Implementation by Claude Sonnet 4.5*
*Part of the Vystia UO Shard Project*
