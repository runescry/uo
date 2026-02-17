# Exploring the Deep Service

## Overview
Exploring the Deep is a quest chain that takes players on an underwater adventure, featuring unique NPCs, items, and dungeon encounters.

## Era
- **Expansion:** High Seas+ (`Core.HS`)
- **Type:** Quest chain

## Files
| File | Description |
|------|-------------|
| `ExploringTheDeepQuestChain.cs` | Main quest chain logic |
| `Generate.cs` | Quest area generation |
| `Regions.cs` | Underwater region definitions |
| `Addons/` | Quest-related addons and decorations |
| `Items/` | Quest items and rewards |
| `Mobiles/` | Quest NPCs and creatures |
| `Questers/` | Quest giver NPCs |

## Functionality
A multi-part quest chain centered around underwater exploration, featuring:
- Unique diving mechanics
- Underwater encounters
- Special quest items
- Story-driven progression

### Quest Chain Structure
1. **Introduction** - Meet quest starter NPC
2. **Investigation** - Gather clues and items
3. **Preparation** - Obtain diving equipment
4. **Exploration** - Explore underwater areas
5. **Finale** - Confront final challenge

## How it Works for Players

### Starting the Quest
1. Find the quest giver NPC
2. Accept the "Exploring the Deep" quest
3. Receive initial instructions and items
4. Begin the investigation phase

### Quest Progression
1. Follow quest objectives
2. Collect required items
3. Speak with quest NPCs
4. Complete each quest phase
5. Advance to next phase

### Underwater Exploration
- Obtain breathing apparatus
- Navigate underwater regions
- Fight aquatic creatures
- Discover hidden areas

### Completing the Chain
1. Complete all quest phases
2. Receive final rewards
3. Unlock any follow-up content

## Configuration
```csharp
// ExploringTheDeepQuestChain.cs
public class ExploringTheDeepQuestChain : QuestChain
{
    public override QuestChain ChainID { get; }
    public override Type[] TypeReferenceTable { get; }
}

// Generate.cs
public static void Generate()
{
    // Spawn quest NPCs and items
}
```

## GM Commands
```
[ExploringTheDeepGen   - Generate quest content
[add [QuestItem]       - Add quest items
```

## Quest NPCs
| NPC | Role |
|-----|------|
| Quest Starter | Initiates quest chain |
| Investigator | Provides clues |
| Diver | Diving equipment |
| (Various) | Quest progression |

## Quest Items
| Item | Purpose |
|------|---------|
| Diving Gear | Underwater breathing |
| Quest Clues | Story progression |
| Special Keys | Access locked areas |
| Rewards | Completion items |

## Quest Regions
Special underwater areas:
- Submerged ruins
- Underwater caves
- Deep sea trenches
- Ancient structures

## FAQ

**Q: Where do I start this quest?**
A: Find the quest giver NPC in a coastal city.

**Q: Do I need special equipment?**
A: Yes, you'll obtain diving gear during the quest.

**Q: Can I do this quest in a group?**
A: Quest is designed for individual completion but group play possible.

**Q: What are the rewards?**
A: Unique items, gold, and possibly access to new content.

**Q: How long does the quest take?**
A: Multiple sessions depending on play style.

**Q: Can I abandon and restart?**
A: Yes, but progress may be lost.

## Rewards
| Reward Type | Description |
|-------------|-------------|
| Equipment | Underwater gear |
| Decorations | House items |
| Gold | Currency reward |
| Titles | Achievement titles |

## Related Systems
- Mondain's Legacy Quests (`../MondainsLegacyQuests/`)
- Quests (`../Quests/`)
- High Seas Expansion (`../Expansions/High Seas/`)
