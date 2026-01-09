# Fast A* Pathfinding

## Overview

`fast_astar.py` implements ServUO's FastAStarAlgorithm for efficient pathfinding within a fixed 38x38 tile search area.

## ServUO Reference

Ported from `Scripts/Services/Pathing/FastAStarAlgorithm.cs`

## Algorithm Characteristics

### Search Area
- **Fixed size**: 38x38 tiles (AREA_SIZE = 38)
- **Centered**: Between start and goal positions
- **Z planes**: 13 height planes for multi-level paths

### Node Indexing
```python
index = local_x + (local_y * AREA_SIZE) + (z_plane * AREA_SIZE * AREA_SIZE)
```

Where:
- `local_x = world_x - x_offset`
- `local_y = world_y - y_offset`
- `z_plane = (z + 128) // 20`

### Heuristic Function
ServUO uses weighted squared distance:
```python
dx = (x - goal_x) * 11
dy = (y - goal_y) * 11
dz = z - goal_z
h = dx*dx + dy*dy + dz*dz
```

The 11x weight on X/Y makes the pathfinder prefer shorter paths over smoother Z transitions.

## Constants

| Constant | Value | Description |
|----------|-------|-------------|
| `MAX_DEPTH` | 300 | Maximum search iterations |
| `AREA_SIZE` | 38 | Search area size |
| `PLANE_OFFSET` | 128 | Z offset for plane calculation |
| `PLANE_COUNT` | 13 | Number of Z planes |
| `PLANE_HEIGHT` | 20 | Z range per plane |
| `NODE_COUNT` | 18,772 | Total nodes (38×38×13) |

## API Reference

### FastAStar Class

```python
class FastAStar:
    def __init__(self, movement_checker: MovementChecker)
```

### Core Methods

#### find_path(start_x, start_y, start_z, goal_x, goal_y, goal_z, ...)
Find path from start to goal.

```python
path = pathfinder.find_path(5400, 50, 33, 5420, 70, 38)
if path:
    for direction in path:
        print(direction.name)
```

**Returns:** List of Direction, or None if no path

#### find_path_with_positions(start_x, start_y, start_z, goal_x, goal_y, goal_z, ...)
Find path and return positions.

```python
positions = pathfinder.find_path_with_positions(5400, 50, 33, 5420, 70, 38)
if positions:
    for x, y, z in positions:
        print(f"({x}, {y}, {z})")
```

**Returns:** List of (x, y, z) tuples, or None

#### path_length(start_x, start_y, start_z, goal_x, goal_y, goal_z)
Get path length without full path.

```python
length = pathfinder.path_length(5400, 50, 33, 5420, 70, 38)
# Returns: 21 (or -1 if no path)
```

#### in_range(start_x, start_y, goal_x, goal_y)
Check if goal is within pathfinding range.

```python
if pathfinder.in_range(5400, 50, 5420, 70):
    # Goal is within 38 tiles
```

### PathFollower Class

Helper for following computed paths.

```python
follower = PathFollower(pathfinder)

# Set goal
if follower.set_goal(start_x, start_y, start_z, goal_x, goal_y, goal_z):
    # Follow path step by step
    while not follower.is_complete():
        direction = follower.get_next_direction()
        if direction:
            # Move in direction
            pass
```

#### Methods

| Method | Returns | Description |
|--------|---------|-------------|
| `set_goal(...)` | bool | Calculate path to goal |
| `get_next_direction()` | Direction | Get next direction |
| `has_path()` | bool | Check if valid path exists |
| `is_complete()` | bool | Check if path finished |
| `remaining_steps()` | int | Steps remaining |

## Usage Examples

### Basic Pathfinding

```python
from map_data_extractor import MapDataExtractor
from walkability_check import WalkabilityChecker
from movement_check import MovementChecker
from fast_astar import FastAStar

# Initialize
extractor = MapDataExtractor(r"C:\UO\Client")
walk_checker = WalkabilityChecker(extractor)
move_checker = MovementChecker(extractor, walk_checker)
pathfinder = FastAStar(move_checker)

# Find path
path = pathfinder.find_path(5400, 50, 33, 5420, 70, 38)

if path:
    print(f"Found path with {len(path)} steps")
else:
    print("No path found")
```

### Following a Path

```python
# Get positions along path
positions = pathfinder.find_path_with_positions(
    start_x, start_y, start_z,
    goal_x, goal_y, goal_z
)

# Simulate movement
for x, y, z in positions:
    print(f"Move to ({x}, {y}, {z})")
```

### AI Movement Integration

```python
class AIMovement:
    def __init__(self, pathfinder):
        self.follower = PathFollower(pathfinder)
        self.x = 0
        self.y = 0
        self.z = 0

    def move_to(self, goal_x, goal_y, goal_z):
        """Set movement goal"""
        return self.follower.set_goal(
            self.x, self.y, self.z,
            goal_x, goal_y, goal_z
        )

    def tick(self, move_checker):
        """Process one movement step"""
        if self.follower.is_complete():
            return False

        direction = self.follower.get_next_direction()
        if direction:
            can_move, new_z = move_checker.check_movement(
                self.x, self.y, self.z, direction
            )
            if can_move:
                dx, dy = move_checker.get_direction_offset(direction)
                self.x += dx
                self.y += dy
                self.z = new_z
                return True

        return False
```

### Checking Reachability

```python
def is_reachable(pathfinder, from_pos, to_pos):
    """Check if destination is reachable"""
    # First check range
    if not pathfinder.in_range(from_pos[0], from_pos[1], to_pos[0], to_pos[1]):
        return False

    # Then try to find path
    path = pathfinder.find_path(
        from_pos[0], from_pos[1], from_pos[2],
        to_pos[0], to_pos[1], to_pos[2]
    )

    return path is not None
```

## Performance Notes

1. **Range check first**: Always call `in_range()` before pathfinding
2. **Path caching**: Reuse paths when goal hasn't changed
3. **Preload map region**: Ensure map data is cached:
   ```python
   extractor.preload_region(x1, y1, x2, y2)
   ```
4. **MAX_DEPTH limit**: Paths longer than 300 steps will fail
5. **Memory efficient**: Uses heapq instead of ServUO's linked list

## Limitations

- **Fixed range**: Cannot path beyond 38 tiles
- **Single map**: No multi-map pathfinding
- **No dynamic obstacles**: Doesn't account for moving mobiles
- **Z precision**: Uses 20-unit Z planes (may miss some valid paths)

## Integration with Combat Simulator

The pathfinder is used by `sidekick_ai_movement.py` for:

1. **MoveTo(target, range)**: Path within casting range of target
2. **RunFrom(enemy)**: Retreat path away from enemy
3. **CanReach(target)**: Check if target is reachable

See `sidekick_ai_movement.md` for AI-specific usage.
