# Mini Champion System Service

## Overview
The Mini Champion System provides smaller-scale champion spawn encounters designed for solo or small group play, with reduced difficulty and proportionally scaled rewards.

## Era
- **Expansion:** All (typically SA+)

## Files
| File | Description |
|------|-------------|
| `MiniChampionSpawn.cs` | Mini champion spawn controller |
| `MiniChampSpawnInfo.cs` | Spawn configuration and creature lists |
| `MiniChampType.cs` | Mini champion type enumeration |

## Functionality
Mini champions are smaller versions of full champion spawns, featuring fewer waves, weaker creatures, and scaled-down rewards.

### Differences from Full Champions
| Aspect | Full Champion | Mini Champion |
|--------|---------------|---------------|
| Waves | 4 levels | 2-3 levels |
| Creatures | Many per wave | Fewer per wave |
| Boss Difficulty | Very Hard | Moderate |
| Rewards | Power Scrolls | Lesser rewards |
| Group Size | Large group | Solo/small group |

## Mini Champion Types
Various themed mini spawns:
- Undead themed
- Demon themed
- Elemental themed
- Creature-specific themes

## How it Works for Players

### Finding Mini Champions
1. Located in specific dungeon areas
2. Smaller altars than full champions
3. May be in instanced areas

### Fighting the Spawn
1. Approach active mini spawn
2. Kill creatures to advance
3. Fewer creatures per wave
4. Mini boss appears sooner

### Claiming Rewards
- Reduced gold rewards
- Lesser scrolls possible
- Artifact chances (lower)
- Quick completion bonus

## Configuration
```csharp
// MiniChampionSpawn.cs
public class MiniChampionSpawn : Item
{
    public MiniChampType Type { get; set; }
    public int Level { get; set; }
    public int MaxKills { get; set; }
}

// MiniChampSpawnInfo.cs
public class MiniChampSpawnInfo
{
    public Type[] SpawnTypes { get; set; }
    public Type ChampionType { get; set; }
    public int[] LevelKills { get; set; }
}
```

## GM Commands
```
[add MiniChampionSpawn
[MiniChampInfo           - View spawn status
```

## Spawn Progression
```
Level 1: 20-30 weak creatures
Level 2: 15-20 medium creatures
(Optional Level 3: 10-15 strong creatures)
Boss: Mini champion appears
```

## Rewards
| Reward Type | Drop Rate |
|-------------|-----------|
| Gold | 100% |
| Scrolls of Power (+5) | Rare |
| Artifacts | Very Rare |
| Special Items | Themed drops |

## FAQ

**Q: Can I solo mini champions?**
A: Yes, they're designed for solo or small group play.

**Q: Where are mini champions located?**
A: Various dungeon locations, check spawn info.

**Q: Do mini champions drop Power Scrolls?**
A: Lower chance for lesser scrolls (+5) compared to full champions.

**Q: How long does a mini champion take?**
A: 15-30 minutes typically.

**Q: Is there a rotation like full champions?**
A: May be, depends on server configuration.

**Q: Are mini champion rewards worth it?**
A: Good for smaller time investment, decent loot.

## Comparison Table
| Feature | Full | Mini |
|---------|------|------|
| Time to complete | 1-2 hours | 15-30 min |
| Players needed | 5-10+ | 1-3 |
| Power Scroll chance | High | Low |
| Gold per hour | High | Moderate |

## Related Systems
- Champion System (`../ChampionSystem/`) - Full champions
- Points Systems (`../PointsSystems/`)
- Dungeon content
