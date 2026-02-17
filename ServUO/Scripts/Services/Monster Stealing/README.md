# Monster Stealing Service

## Overview
The Monster Stealing system allows players to steal special items directly from monsters using the Stealing skill, providing an alternative method to obtain rare items.

## Era
- **Expansion:** Mondain's Legacy+ (`Core.ML`)

## Files
| File | Description |
|------|-------------|
| `Core/` | Core stealing logic and mechanics |
| `Items/` | Stealable item definitions |

## Functionality
Players with high Stealing skill can attempt to steal unique items from creatures that cannot be obtained through normal combat loot.

### Stealable Items
Special items only obtainable through stealing:
- Artifacts
- Rare resources
- Quest items
- Unique decorations

### Skill Requirements
| Target Difficulty | Stealing Skill |
|-------------------|----------------|
| Easy | 50+ |
| Medium | 75+ |
| Hard | 100+ |
| Very Hard | 100+ (GM) |

## How it Works for Players

### Stealing from Monsters
1. Train Stealing skill to appropriate level
2. Find a monster with stealable item
3. Use Stealing skill
4. Target the monster
5. Success = obtain item
6. Failure = damage/aggro

### Success Factors
- Stealing skill level
- Target creature difficulty
- Item weight
- Random chance

### Risks
- Failed attempts alert monster
- May trigger attack
- Some creatures immune to stealing
- Cooldown between attempts

## Configuration
```csharp
// Core stealing definitions
public class StealableArtifact
{
    public Type CreatureType { get; set; }
    public Type ItemType { get; set; }
    public int Difficulty { get; set; }
    public double Chance { get; set; }
}
```

## GM Commands
```
[add [StealableItem]
[SetSkill Stealing 100
```

## Stealable Item List (Examples)
| Creature | Item | Difficulty |
|----------|------|------------|
| Balron | Balron's Ring | Very Hard |
| Dragon | Dragon Scale | Hard |
| Lich | Lich Staff | Hard |
| (Various) | (Various) | Variable |

## FAQ

**Q: What skill do I need?**
A: Stealing skill, ideally GM (100) for best items.

**Q: Can I steal from any monster?**
A: Only monsters with stealable items defined.

**Q: How do I know if a monster has stealable items?**
A: Experience or wiki/guides - not shown in-game.

**Q: What happens if I fail?**
A: Monster becomes aggressive, possible damage.

**Q: Is there a cooldown?**
A: Yes, between steal attempts on same target.

**Q: Do items respawn on the monster?**
A: Depends on item - some are one-time, others respawn.

## Skill Training
Stealing skill trained by:
- Stealing from monsters (with stealables)
- Practice with friends (containers)
- NPC training

## Stealing Process
```
1. Select Stealing skill
2. Target creature
3. Skill check vs difficulty
4. Success: Item in backpack
   Failure: Alert creature, possible damage
```

## Related Systems
- Thievery skills (Snooping, Stealing)
- Loot Generation (`../LootGeneration/`)
- Monster definitions
