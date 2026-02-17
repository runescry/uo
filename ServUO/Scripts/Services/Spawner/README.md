# Spawner Service

## Overview
The Spawner system handles creature and item spawning throughout the world, including basic spawners and proximity-based activation.

## Era
- **Expansion:** All
- **Availability:** Core system

## Files
| File | Description |
|------|-------------|
| `Spawner.cs` | Basic spawner implementation |
| `SpawnerGump.cs` | Spawner configuration UI |
| `SpawnerType.cs` | Spawner type definitions |
| `SpawnObject.cs` | Individual spawn definitions |
| `ProximitySpawner.cs` | Player-activated spawner |

## Functionality
Manages creature and item spawning with configurable parameters.

### Spawner Types
| Type | Description |
|------|-------------|
| Basic | Always active spawning |
| Proximity | Activates when player nearby |
| Timed | Spawns at specific times |
| Conditional | Spawns under conditions |

### Spawner Properties
- Spawn list (what to spawn)
- Count (how many)
- Home range (spawn area)
- Delay (time between spawns)
- Running state (active/inactive)

## How it Works

### Basic Spawner
1. Spawner placed in world
2. Timer runs continuously
3. Creatures spawn at intervals
4. Respawn when killed

### Proximity Spawner
1. Spawner waits inactive
2. Player enters range
3. Spawning activates
4. May deactivate when player leaves

## Configuration
```csharp
// Spawner.cs
public class Spawner : Item
{
    public int Count { get; set; }
    public int HomeRange { get; set; }
    public TimeSpan MinDelay { get; set; }
    public TimeSpan MaxDelay { get; set; }
    public List<SpawnObject> SpawnObjects { get; }
}

// ProximitySpawner.cs
public class ProximitySpawner : Spawner
{
    public int ProximityRange { get; set; }
    public bool InstantFlag { get; set; }
}
```

## GM Commands
```
[add Spawner
[add ProximitySpawner
[props [spawner]         - Configure spawner
[SpawnerGump            - Open spawner UI
```

## SpawnObject Properties
| Property | Description |
|----------|-------------|
| SpawnName | Creature/item type |
| MaxCount | Maximum spawn count |
| CurrentCount | Current active count |
| Probability | Spawn chance |

## Spawner Gump
The spawner gump allows:
- Adding spawn entries
- Setting counts
- Configuring delays
- Setting home range
- Starting/stopping spawner

## FAQ

**Q: How do I add creatures to a spawner?**
A: Use `[props` on spawner, or SpawnerGump.

**Q: What's the difference from XmlSpawner?**
A: Basic spawner is simpler; XmlSpawner has more features.

**Q: Can spawners spawn items?**
A: Yes, any type that can be instantiated.

**Q: How do I stop a spawner?**
A: Set Running = false or delete it.

**Q: What's home range?**
A: How far from spawner creatures can wander.

**Q: Do spawners save with world?**
A: Yes, spawner state persists.

## Spawn Timing
```
MinDelay = Minimum time between spawns
MaxDelay = Maximum time between spawns
Actual delay = Random between min and max
```

## Proximity Spawner
Special behavior:
- Inactive until player approaches
- ProximityRange triggers activation
- Can spawn instantly or delayed
- May message player on trigger

## Related Systems
- XmlSpawner (`../XmlSpawner/`) - Advanced spawning
- Pathing (`../Pathing/`) - Creature movement
- Region system - Spawn restrictions
