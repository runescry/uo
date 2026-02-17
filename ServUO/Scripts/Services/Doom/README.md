# Doom Dungeon Service

## Overview
The Doom dungeon service manages the gauntlet-style encounters, puzzle rooms, and boss mechanics within Doom - one of the most challenging dungeons in Ultima Online.

## Era
- **Expansion:** Age of Shadows+ (`Core.AOS`)
- **Location:** Malas facet

## Files
| File | Description |
|------|-------------|
| `GauntletSpawner.cs` | Gauntlet room spawn controller |
| `GaryRoom.cs` | Gary the Dungeon Master puzzle room |
| `GuardianRoom.cs` | Guardian encounter room |
| `LeverPuzzleController.cs` | Lever puzzle logic |
| `LeverPuzzleItems.cs` | Puzzle item definitions |
| `LeverPuzzleRegions.cs` | Puzzle area regions |

## Functionality
Doom features unique encounter types including gauntlet battles, puzzle mechanics, and the infamous Dark Father boss.

### Dungeon Structure
1. **Entry Area** - Initial dungeon zones
2. **Gauntlet Rooms** - Wave-based combat
3. **Puzzle Rooms** - Gary/Guardian challenges
4. **Boss Area** - Dark Father encounter

### Gauntlet System
The gauntlet spawns waves of creatures that must be defeated to progress:
- Waves increase in difficulty
- Timed spawning mechanics
- Must clear room to advance
- Unique rewards per gauntlet

## How it Works for Players

### Entering Doom
1. Travel to Malas
2. Find Doom dungeon entrance
3. Navigate through initial areas
4. Prepare for gauntlet encounters

### Gauntlet Battles
1. Enter a gauntlet room
2. Waves of creatures spawn
3. Defeat all creatures in wave
4. New wave spawns until complete
5. Room clears, proceed to next area

### Gary's Room
A puzzle room featuring Gary the Dungeon Master:
1. Enter Gary's room
2. Solve the presented puzzle
3. Failure results in consequences
4. Success grants passage/rewards

### Guardian Room
Combat challenge room:
1. Face guardian creatures
2. Defeat them to proceed
3. Special mechanics may apply

### Lever Puzzles
1. Find lever puzzle rooms
2. Activate levers in correct sequence
3. Incorrect sequence triggers traps
4. Correct sequence opens passage

## Configuration
```csharp
// GauntletSpawner.cs
public class GauntletSpawner : Item
{
    public TimeSpan SpawnDelay { get; set; }
    public int MaxSpawnCount { get; set; }
    public Type[] SpawnTypes { get; set; }
}

// LeverPuzzleController.cs
public class LeverPuzzleController : Item
{
    public int[] CorrectSequence { get; set; }
    public int CurrentPosition { get; set; }
}
```

## GM Commands
```
[add GauntletSpawner
[add LeverPuzzleController
[DoomGen                 - Generate Doom dungeon
```

## Dungeon Loot
Doom offers unique drops:
- **Doom Artifacts** - Rare equipment
- **Dark Father Loot** - Boss drops
- **Gauntlet Rewards** - Room completion items

### Notable Artifacts
| Artifact | Type |
|----------|------|
| Staff of the Magi | Weapon |
| Rune Beetle Carapace | Armor |
| Spirit of the Totem | Talisman |
| (Many more) | Various |

## Boss: Dark Father
The ultimate Doom encounter:
- Spawned by completing gauntlet
- Extremely powerful demon
- Requires group coordination
- Drops rare artifacts

## FAQ

**Q: How do I get to Doom?**
A: Use moongates to Malas, then navigate to Doom entrance.

**Q: Can I solo the gauntlet?**
A: Possible with strong characters, but designed for groups.

**Q: What happens if I die in a puzzle room?**
A: You respawn outside the room and can retry.

**Q: How do lever puzzles work?**
A: Pull levers in the correct order. Wrong order resets or triggers traps.

**Q: Is Doom instanced?**
A: No, Doom is a shared dungeon space.

**Q: How often can I fight Dark Father?**
A: After defeating him, respawn timer determines availability.

## Gauntlet Room Types
| Room | Challenge |
|------|-----------|
| Undead Gauntlet | Skeleton/zombie waves |
| Demon Gauntlet | Daemon variants |
| Elemental Gauntlet | Elemental creatures |
| Final Gauntlet | Mixed high-level creatures |

## Related Systems
- Champion System (`../ChampionSystem/`) - Similar wave mechanics
- Peerless (`../Peerless/`) - Boss encounter comparison
- Malas (`../Malas/`) - Map region
