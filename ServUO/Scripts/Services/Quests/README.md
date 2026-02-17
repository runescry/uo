# Quests Service

## Overview
The Quests service provides the classic quest framework for creating and managing quests with objectives, conversations, and rewards.

## Era
- **Expansion:** All
- **Status:** Core system (ML quests in separate folder)

## Files
| File | Description |
|------|-------------|
| `QuestSystem.cs` | Core quest management |
| `BaseQuester.cs` | Base quest giver NPC |
| `QuestObjective.cs` | Objective definitions |
| `QuestConversation.cs` | NPC dialog system |
| `QuestItemInfo.cs` | Quest item definitions |
| `QuestRestartInfo.cs` | Quest retry information |
| `QuestSerializer.cs` | Quest data persistence |
| `QuestCallbackEntry.cs` | Event callbacks |
| `Items/` | Quest-specific items |
| `Regions/` | Quest regions |

## Functionality
Framework for creating quests with multiple objectives, NPC conversations, and rewards.

### Quest Components
| Component | Purpose |
|-----------|---------|
| QuestSystem | Overall quest management |
| Objectives | What player must do |
| Conversations | NPC dialog |
| Rewards | Completion benefits |

### Objective Types
- Kill objectives
- Collect objectives
- Escort objectives
- Exploration objectives
- Custom objectives

## How it Works for Players

### Finding Quests
1. Locate quest giver NPC
2. Speak with them
3. Read quest description
4. Accept if interested

### Completing Objectives
1. Follow quest instructions
2. Complete each objective
3. Track progress in quest log
4. Return when complete

### Claiming Rewards
1. Return to quest giver
2. Confirm completion
3. Receive rewards
4. Quest marked complete

## Configuration
```csharp
// QuestSystem.cs
public abstract class QuestSystem
{
    public abstract object Name { get; }
    public abstract object OfferMessage { get; }
    public abstract bool IsTutorial { get; }
}

// QuestObjective.cs
public abstract class QuestObjective
{
    public abstract object Message { get; }
    public abstract bool Completed { get; set; }
}
```

## GM Commands
```
[add [QuestGiverType]
[QuestInfo [player]     - View player quests
```

## Quest Types
| Type | Description |
|------|-------------|
| Tutorial | New player guidance |
| Story | Lore-driven quests |
| Repeatable | Can be done multiple times |
| Chain | Multi-part storylines |

## Quest Conversations
NPCs can have branching dialog:
```csharp
public class QuestConversation
{
    public int Message { get; }
    public string[] Responses { get; }
    public bool Logged { get; }
}
```

## FAQ

**Q: How many quests can I have active?**
A: Typically unlimited, but some may conflict.

**Q: Can I abandon quests?**
A: Yes, most quests can be abandoned.

**Q: Do quest items persist?**
A: Usually deleted when quest ends.

**Q: Can I repeat quests?**
A: Depends on quest - some are repeatable.

**Q: Where do I track quest progress?**
A: Quest log in your paperdoll/status.

**Q: What if I lose a quest item?**
A: May need to restart or continue without.

## Quest Regions
Special areas for quests:
- Tutorial regions
- Quest-specific zones
- Protected areas
- Spawn regions

## Quest Items
Items specific to quests:
- Objective items (collectables)
- Key items (progression)
- Reward items (completion)

## Related Systems
- Mondain's Legacy Quests (`../MondainsLegacyQuests/`) - Modern quests
- Exploring the Deep (`../ExploringTheDeep/`) - Quest chain
- NPC Dialog System
