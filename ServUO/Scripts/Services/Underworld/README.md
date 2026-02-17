# Underworld Service

## Overview
The Underworld service manages the Stygian Abyss dungeon content including multiple specialized areas like the Maze of Death, Navrey's Lair, and puzzle rooms.

## Era
- **Expansion:** Stygian Abyss+ (`Core.SA`)
- **Location:** Stygian Abyss dungeon

## Files
| File | Description |
|------|-------------|
| `Generate.cs` | Dungeon generation |
| `ExperimentalRoom/` | Experimental area content |
| `Maze of Death/` | Maze dungeon section |
| `Navrey's Lair/` | Navrey Night-Eyes boss |
| `PuzzleRoom/` | Puzzle encounter area |
| `Underworld Dragons/` | Dragon encounter area |

## Dungeon Sections

### Maze of Death
Deadly labyrinth area:
- Complex navigation
- Traps throughout
- Mini-bosses
- Valuable loot

### Navrey's Lair
Boss encounter area:
- Navrey Night-Eyes spider boss
- Key requirements
- Group content
- Artifact drops

### Puzzle Room
Puzzle-based challenges:
- Logic puzzles
- Timed challenges
- Rewards for completion
- Optional content

### Experimental Room
Unique encounter:
- Special mechanics
- Experimental creatures
- Unusual challenges

### Underworld Dragons
Dragon encounters:
- Powerful dragon spawns
- Dragonscale resources
- High-value loot

## How it Works for Players

### Entering the Underworld
1. Travel to Stygian Abyss entrance
2. Navigate through dungeon
3. Access different sections
4. Complete challenges

### Maze of Death
1. Enter maze entrance
2. Navigate complex passages
3. Avoid/disarm traps
4. Find exit or treasure

### Navrey Encounter
1. Collect entry keys
2. Access Navrey's Lair
3. Fight boss with group
4. Claim rewards

### Puzzle Rooms
1. Locate puzzle room
2. Solve presented puzzle
3. Correct solution rewards
4. Wrong solution penalties

## Configuration
```csharp
// Generate.cs
public static void Generate()
{
    // Create Underworld content
    GenerateMazeOfDeath();
    GenerateNavreysLair();
    GeneratePuzzleRoom();
}
```

## GM Commands
```
[UnderworldGen         - Generate content
[add NavreyNightEyes   - Spawn boss
```

## Navrey Night-Eyes
| Attribute | Value |
|-----------|-------|
| Type | Giant Spider |
| Difficulty | Peerless |
| Location | Navrey's Lair |
| Keys | Required |
| Group | Recommended |

### Navrey Loot
- Navrey's Clutch (artifact)
- Web drops
- SA artifacts
- Gold and gems

## FAQ

**Q: How do I get to the Underworld?**
A: Through Stygian Abyss dungeon entrance.

**Q: Do I need a group?**
A: For Navrey and some content, yes.

**Q: What's the maze reward?**
A: Treasure at the end, avoiding traps.

**Q: How do puzzle rooms work?**
A: Solve logic puzzle for rewards.

**Q: What level should I be?**
A: End-game content, strong characters needed.

## Section Difficulty
| Section | Difficulty | Group? |
|---------|------------|--------|
| Entrance | Moderate | Optional |
| Maze | Hard | Optional |
| Navrey | Very Hard | Recommended |
| Dragons | Hard | Optional |
| Puzzles | Variable | Solo |

## Related Systems
- Peerless (`../Peerless/`) - Boss mechanics
- Instanced Peerless (`../InstancedPeerless/`)
- Tomb of Kings (`../Tomb of Kings/`)
