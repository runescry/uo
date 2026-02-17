# Pathing Service

## Overview
The Pathing system provides pathfinding algorithms for NPC movement, enabling creatures and NPCs to navigate around obstacles intelligently.

## Era
- **Expansion:** All
- **Availability:** Core system

## Files
| File | Description |
|------|-------------|
| `PathAlgorithm.cs` | Base pathfinding algorithm |
| `FastAStarAlgorithm.cs` | Optimized A* implementation |
| `SlowAStarAlgorithm.cs` | Standard A* implementation |
| `Movement.cs` | Movement validation |
| `FastMovement.cs` | Optimized movement checking |
| `MovementPath.cs` | Path storage and execution |
| `MoveResult.cs` | Movement result enumeration |
| `PathFollower.cs` | Path execution controller |

## Functionality
Enables NPCs to find routes to destinations while avoiding obstacles, walls, and impassable terrain.

### Pathfinding Algorithms
- **A* (A-Star)** - Optimal path finding
- **Fast A*** - Performance-optimized version
- **Slow A*** - More thorough but slower

### Movement Validation
Before moving, checks:
- Terrain passability
- Height differences
- Blocked tiles
- Mobile collisions

## How it Works

### Path Calculation
1. Start position defined
2. Goal position defined
3. Algorithm calculates route
4. Path stored for execution

### Path Execution
1. PathFollower receives path
2. Mobile moves step by step
3. Validates each move
4. Handles obstacles/blockages

### Algorithm Selection
```csharp
// Fast for nearby targets
if (distance < 20)
    return FastAStarAlgorithm.Instance;

// Slow for complex paths
return SlowAStarAlgorithm.Instance;
```

## Configuration
```csharp
// PathAlgorithm.cs
public abstract class PathAlgorithm
{
    public abstract bool CheckCondition(Mobile m, Map map, Point3D start, Point3D goal);
    public abstract Direction[] Find(Mobile m, Map map, Point3D start, Point3D goal);
}

// Movement.cs
public static class Movement
{
    public static MoveResult CheckMovement(Mobile m, Direction d)
}
```

## Developer Usage
```csharp
// Finding a path
Direction[] path = PathAlgorithm.FindPath(mobile, start, goal);

// Using PathFollower
PathFollower follower = new PathFollower(mobile, goal);
follower.MoveNext();
```

## MoveResult Values
| Result | Meaning |
|--------|---------|
| Success | Move allowed |
| Blocked | Obstacle present |
| Invalid | Invalid destination |
| BadState | Cannot move currently |

## Performance Considerations
- A* is computationally expensive
- FastAstar for short distances
- SlowAStar for long distances
- Caching for repeated paths

## FAQ

**Q: Why don't NPCs always path correctly?**
A: Complex terrain may exceed algorithm limits.

**Q: Can I change the pathing algorithm?**
A: Yes, configure in mobile AI settings.

**Q: What causes "stuck" NPCs?**
A: Blocked paths, terrain changes, or algorithm limits.

**Q: How far can NPCs path?**
A: Limited by performance - typically 50-100 tiles max.

**Q: Do players use this system?**
A: No, player movement is direct. This is for NPCs.

## A* Algorithm Overview
```
1. Start at origin
2. Calculate cost to each neighbor
3. Move to lowest-cost unexplored
4. Repeat until goal reached
5. Return path
```

## Optimization Tips
- Use appropriate algorithm for distance
- Limit maximum path length
- Cache frequently-used paths
- Avoid recalculating every frame

## Related Systems
- Mobile AI
- Spawner (`../Spawner/`) - NPC spawn positions
- Region system - Movement restrictions
