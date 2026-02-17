# Mondain's Legacy Quests Service

## Overview
The Mondain's Legacy quest system provides the modern quest framework with multi-objective quests, escort missions, tiered rewards, and complex dialog trees.

## Era
- **Expansion:** Mondain's Legacy+ (`Core.ML`)

## Files
| File | Description |
|------|-------------|
| `MondainQuester.cs` | Base quest giver NPC |
| `HeritageQuester.cs` | Heritage/legacy quest giver |
| `BaseEscort.cs` | Escort NPC base class |
| `BaseQuestItem.cs` | Quest item base class |
| `BaseReward.cs` | Reward base class |
| `QuestObjectives.cs` | Objective type definitions |
| `SimpleObjective.cs` | Basic objective implementation |
| `QuestChains.cs` | Chain quest definitions |
| `QuestHintItem.cs` | Items that hint at quests |
| `TierQuest.cs` | Tiered difficulty quests |
| `Q&AEntry.cs` | Dialog tree entries |
| `ApprenticeRegion.cs` | Tutorial/apprentice areas |
| `Gumps/` | Quest UI gumps |
| `Helpers/` | Quest helper utilities |

## Functionality
A comprehensive quest system with multiple objective types, conversation trees, and reward tiers.

### Quest Types
| Type | Description |
|------|-------------|
| Kill Quest | Kill specific creatures |
| Collect Quest | Gather specific items |
| Escort Quest | Protect and guide NPC |
| Exploration Quest | Visit specific locations |
| Chain Quest | Multi-part storyline |
| Tier Quest | Repeatable at higher difficulties |

### Objective System
Quests can have multiple objectives:
- Primary objectives (required)
- Bonus objectives (optional)
- Hidden objectives (discovered)

## How it Works for Players

### Finding Quests
1. Look for NPCs with quest indicators
2. Speak with them to view available quests
3. Read quest description and objectives
4. Accept quest to begin

### Quest Progression
1. Complete listed objectives
2. Track progress in quest log
3. Return to quest giver
4. Claim rewards

### Escort Quests
1. Accept escort mission
2. NPC follows you
3. Protect from attackers
4. Deliver to destination
5. Receive reward

### Chain Quests
1. Complete first quest in chain
2. Unlock next quest
3. Progress through story
4. Final quest has best rewards

## Configuration
```csharp
// MondainQuester.cs
public abstract class MondainQuester : BaseVendor
{
    public abstract Type[] Quests { get; }
}

// QuestObjectives.cs
public class SlayObjective : BaseObjective
{
    public Type Creature { get; set; }
    public int Amount { get; set; }
}

public class CollectObjective : BaseObjective
{
    public Type Item { get; set; }
    public int Amount { get; set; }
}
```

## GM Commands
```
[add MondainQuester
[QuestDebug [player]    - Debug player quests
```

## Reward System
| Reward Type | Description |
|-------------|-------------|
| Gold | Currency reward |
| Items | Equipment or consumables |
| Recipes | Crafting recipes |
| Titles | Character titles |
| Access | Area or content access |

### Tier Quest Rewards
Higher tiers = better rewards:
- Tier 1: Basic rewards
- Tier 2: Moderate rewards
- Tier 3: Good rewards
- Tier 4+: Best rewards

## Quest Chains
Popular quest chains:
- Heartwood Quests (elven)
- Sanctuary Quests
- Bedlam Access
- Peerless Keys

## FAQ

**Q: Where do I find quest givers?**
A: Look in cities, dungeons, and camps for NPCs with quest indicators.

**Q: Can I have multiple quests?**
A: Yes, quest log can hold multiple active quests.

**Q: What if I abandon a quest?**
A: You can retake most quests from the quest giver.

**Q: Do quest items stay after abandoning?**
A: Usually removed; check specific quest requirements.

**Q: How do escort quests work?**
A: NPC follows you; deliver safely to earn rewards.

**Q: What are tier quests?**
A: Repeatable quests with increasing difficulty and rewards.

## Quest Log Interface
The quest log shows:
- Active quests
- Objectives and progress
- Rewards preview
- Quest giver location

## Dialog System (Q&A)
NPCs can have conversation trees:
```csharp
public class Q_AEntry
{
    public string Question { get; set; }
    public string[] Responses { get; set; }
}
```

## Related Systems
- Quests (`../Quests/`) - Classic quest system
- Expansions (`../Expansions/`) - ML content
- Peerless (`../Peerless/`) - Key quests
