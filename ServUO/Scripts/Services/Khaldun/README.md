# Khaldun Dungeon Service

## Overview
The Khaldun dungeon service manages the unique puzzle and trap mechanics within Khaldun, one of Britannia's most challenging dungeons featuring puzzle chests and environmental hazards.

## Era
- **Expansion:** Third Dawn+ (`Core.T2A` or `Core.AOS`)
- **Location:** Felucca (primarily)

## Files
| File | Description |
|------|-------------|
| `KhaldunGen.cs` | Dungeon generation and setup |
| `KhaldunPitTeleporter.cs` | Pit trap teleportation system |
| `PuzzleChest.cs` | Puzzle chest mechanics |
| `RaisableItem.cs` | Items that raise/lower |
| `RaiseSwitch.cs` | Switches to control raisable items |

## Functionality
Khaldun features unique dungeon mechanics not found elsewhere, focusing on puzzles and environmental navigation.

### Puzzle Chests
Special chests that require solving a combination puzzle:
- Multiple-dial lock system
- Trial and error solving
- Traps for wrong guesses
- Valuable loot on success

### Pit Teleporters
Hidden pit traps that teleport players:
- Appear as normal floor
- Teleport to lower levels
- Can be avoided with detect hidden
- Navigation puzzle element

### Raisable Items
Platform elements that raise and lower:
- Controlled by switches
- Block or allow passage
- Timed mechanics
- Puzzle solving requirement

## How it Works for Players

### Entering Khaldun
1. Travel to Khaldun entrance (Felucca)
2. Navigate initial areas
3. Prepare for puzzles and traps

### Solving Puzzle Chests
1. Find a puzzle chest
2. Attempt to pick the combination
3. Wrong attempts trigger traps
4. Correct combination opens chest
5. Claim valuable loot

### Navigating Pits
1. Move carefully through dungeon
2. Watch for trap indicators
3. Use Detect Hidden skill
4. Avoid or strategically use teleporters

### Using Raise Switches
1. Find raise switches in dungeon
2. Activate to raise/lower platforms
3. Creates pathways to new areas
4. May require multiple switch activation

## Configuration
```csharp
// PuzzleChest.cs
public class PuzzleChest : LockableContainer
{
    public int[] Solution { get; set; }
    public int Attempts { get; set; }
    public int MaxAttempts { get; set; }
}

// KhaldunPitTeleporter.cs
public class KhaldunPitTeleporter : Item
{
    public Point3D DestinationPoint { get; set; }
    public Map DestinationMap { get; set; }
}
```

## GM Commands
```
[KhaldunGen            - Generate Khaldun content
[add PuzzleChest       - Place puzzle chest
[add KhaldunPitTeleporter - Place pit teleporter
[add RaiseSwitch       - Place raise switch
```

## Puzzle Chest Mechanics
| Component | Function |
|-----------|----------|
| Dials | Set combination |
| Submit | Test combination |
| Trap | Wrong guess penalty |
| Reward | Correct solution loot |

### Trap Effects
Wrong guesses may trigger:
- Poison
- Explosion damage
- Paralysis
- Monster spawns

### Loot Quality
Puzzle chests contain:
- Gold
- Gems
- Magic items
- Rare artifacts

## Pit Teleporter System
```
Surface Level → Detection chance
             ↓ (miss detection)
Lower Level  → Different dungeon area
```

### Detection
- Detect Hidden skill check
- Higher skill = better avoidance
- Can reveal pit locations
- Some pits unavoidable

## FAQ

**Q: How do I solve puzzle chests?**
A: Trial and error. Set dials and submit. Wrong = trap, right = loot.

**Q: Can I avoid all pit traps?**
A: Most can be detected with high Detect Hidden skill.

**Q: What's in puzzle chests?**
A: Gold, gems, magic items, and occasionally rare items.

**Q: Are raise switches timed?**
A: Some are permanent, others reset after a time.

**Q: Is Khaldun group-friendly?**
A: Yes, but puzzle elements are individual challenges.

**Q: What level should I be?**
A: Mid to high level recommended due to creatures and traps.

## Dungeon Layout
| Area | Features |
|------|----------|
| Entry | Basic monsters |
| Mid-Level | Puzzle chests, traps |
| Deep Level | Boss creatures, best loot |
| Secret Areas | Rare spawns, hidden chests |

## Related Systems
- Treasure Maps (`../TreasureMaps/`) - Similar chest mechanics
- Seasonal Events (`../Seasonal Events/TreasuresOfKhaldun/`)
- Doom (`../Doom/`) - Another puzzle dungeon
