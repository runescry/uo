# Tomb of Kings Service

## Overview
The Tomb of Kings dungeon service manages this multi-chamber Stygian Abyss dungeon featuring the Arisen invasion, secret doors, and the Serpent's Breath area.

## Era
- **Expansion:** Stygian Abyss+ (`Core.SA`)
- **Location:** Stygian Abyss

## Files
| File | Description |
|------|-------------|
| `Generation.cs` | Dungeon generation |
| `TombOfKingsRegion.cs` | Main dungeon region |
| `ToKBridgeRegion.cs` | Bridge area region |
| `ToKTeleporter.cs` | Dungeon teleporters |
| `TombOfKingsSecretDoor.cs` | Hidden door mechanics |
| `SacredQuestBlocker.cs` | Quest access control |
| `Arisen Invasion/` | Arisen invasion event |
| `Chambers/` | Individual chamber content |
| `Serpent's Breath/` | Serpent's Breath area |

## Functionality
A complex dungeon with multiple sections, secret passages, and special events.

### Dungeon Areas
| Area | Description |
|------|-------------|
| Main Chambers | Primary dungeon rooms |
| Bridge | Connecting passage |
| Secret Areas | Hidden rooms |
| Serpent's Breath | Toxic area |

### Arisen Invasion
Special event where undead Arisen attack:
- Wave-based encounters
- Special loot
- Timed event

## How it Works for Players

### Entering the Tomb
1. Travel to Stygian Abyss
2. Find Tomb of Kings entrance
3. Navigate chambers
4. Discover secrets

### Secret Doors
1. Search for hidden doors
2. May require Detect Hidden
3. Opens passage to secret areas
4. Contains better loot

### Serpent's Breath
1. Toxic area of dungeon
2. Requires protection
3. Contains valuable resources
4. Challenging navigation

### Arisen Invasion
1. Event activates periodically
2. Undead waves spawn
3. Defeat for rewards
4. Timed completion

## Configuration
```csharp
// TombOfKingsRegion.cs
public class TombOfKingsRegion : BaseRegion
{
    // Region-specific rules
}

// Generation.cs
public static void Generate()
{
    // Create dungeon content
}
```

## GM Commands
```
[ToKGen                - Generate dungeon
[add ToKTeleporter
[add TombOfKingsSecretDoor
```

## Chamber Content
| Chamber | Features |
|---------|----------|
| Entry | Basic monsters |
| Mid | Moderate difficulty |
| Deep | Advanced encounters |
| Boss | Champion creatures |

## Secret Doors
Hidden passages throughout:
- Detected with skills
- Lead to treasure rooms
- May have traps
- Better loot behind them

## FAQ

**Q: How do I find secret doors?**
A: Use Detect Hidden skill, look for visual clues.

**Q: What protection do I need for Serpent's Breath?**
A: Poison resistance and cure potions recommended.

**Q: When does Arisen Invasion happen?**
A: Periodically, check event schedules.

**Q: What loot is available?**
A: SA-era items, artifacts, resources.

**Q: Can I solo this dungeon?**
A: Parts yes, some areas need groups.

## Teleporter System
Dungeon uses teleporters for:
- Level transitions
- Secret area access
- Quick travel
- Event areas

## Related Systems
- Underworld (`../Underworld/`) - SA dungeon content
- Seasonal Events (`../Seasonal Events/`)
- Revamped Dungeons (`../Revamped Dungeons/`)
