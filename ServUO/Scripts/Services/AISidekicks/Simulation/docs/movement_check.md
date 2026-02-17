# Movement Check

## Overview

`movement_check.py` implements ServUO's `Movement.CheckMovement()` for validating directional movement. It handles:

- 8-directional movement validation
- Diagonal movement rules (adjacent tiles check)
- Z-level changes and step height limits
- Swimming and water-only creatures

## ServUO Reference

Ported from `Scripts/Services/Pathing/Movement.cs`:
- `MovementImpl.CheckMovement()` - Main movement validation
- `MovementImpl.Check()` - Position validation
- `MovementImpl.GetStartZ()` - Starting Z calculation
- `MovementImpl.IsOk()` - Z range blocking check

## Direction System

### 8 Directions

| Direction | Value | Offset (dx, dy) | Description |
|-----------|-------|-----------------|-------------|
| North | 0x0 | (0, -1) | Up |
| Right | 0x1 | (1, -1) | Northeast (diagonal) |
| East | 0x2 | (1, 0) | Right |
| Down | 0x3 | (1, 1) | Southeast (diagonal) |
| South | 0x4 | (0, 1) | Down |
| Left | 0x5 | (-1, 1) | Southwest (diagonal) |
| West | 0x6 | (-1, 0) | Left |
| Up | 0x7 | (-1, -1) | Northwest (diagonal) |

### Diagonal Movement Rules

For diagonal movement (Right, Down, Left, Up), both adjacent tiles must be checked:

```
Moving Northeast (Right):
  Check: East AND North

     N
     |
  W--+--E  <- Current position
     |
     S

Northeast requires BOTH North and East to be passable
(for players - NPCs only need ONE)
```

## How CheckMovement Works

### 1. Get Destination
```python
dx, dy = DIRECTION_OFFSETS[direction]
dest_x = x + dx
dest_y = y + dy
```

### 2. Check Bounds
```python
if dest_x < 0 or dest_y < 0 or dest_x >= map_width or dest_y >= map_height:
    return False
```

### 3. Get Start Z Info
Calculate current surface Z for step height comparison.

### 4. Check Forward Position
Validate the destination tile:
- Check static surfaces
- Check land tile
- Verify Z range is clear
- Calculate new Z level

### 5. Diagonal Check
For diagonal movement, verify adjacent tiles:
```python
if is_diagonal(direction):
    left_ok = check(left_adjacent)
    right_ok = check(right_adjacent)

    # NPC rule: at least one must be passable
    if not left_ok and not right_ok:
        return False
```

## Constants

| Constant | Value | Description |
|----------|-------|-------------|
| `PERSON_HEIGHT` | 16 | Standard creature height |
| `STEP_HEIGHT` | 2 | Maximum Z step up/down |

## API Reference

### MovementChecker Class

```python
class MovementChecker:
    def __init__(self,
                 map_extractor: MapDataExtractor,
                 walkability_checker: WalkabilityChecker,
                 tiledata: Optional[TileDataReference] = None,
                 map_width: int = 7168,
                 map_height: int = 4096)
```

### Core Methods

#### check_movement(x, y, z, direction, can_swim=False, cant_walk=False)
Main movement validation.

```python
can_move, new_z = checker.check_movement(5400, 50, 33, Direction.North)
```

#### check_movement_detailed(x, y, z, direction, ...)
Returns detailed result with blocking reason.

```python
result = checker.check_movement_detailed(5400, 50, 33, Direction.North)
print(f"Can move: {result.can_move}")
print(f"New Z: {result.new_z}")
print(f"Blocking reason: {result.blocking_reason}")
```

#### get_valid_directions(x, y, z, can_swim=False, cant_walk=False)
Get all valid movement directions from a position.

```python
valid_dirs = checker.get_valid_directions(5400, 50, 33)
print(f"Can move: {[d.name for d in valid_dirs]}")
```

#### get_best_retreat_direction(x, y, z, enemy_x, enemy_y)
Find best direction to move away from enemy.

```python
retreat = checker.get_best_retreat_direction(5400, 50, 33, 5410, 60)
if retreat:
    print(f"Retreat: {retreat.name}")
else:
    print("CORNERED!")
```

#### get_direction_toward(x, y, target_x, target_y)
Get direction from current position toward target.

```python
direction = checker.get_direction_toward(5400, 50, 5410, 60)
# Returns Direction.Down (Southeast)
```

### Direction Enum

```python
from movement_check import Direction

# Cardinal directions
Direction.North  # 0x0
Direction.East   # 0x2
Direction.South  # 0x4
Direction.West   # 0x6

# Diagonal directions
Direction.Right  # 0x1 (Northeast)
Direction.Down   # 0x3 (Southeast)
Direction.Left   # 0x5 (Southwest)
Direction.Up     # 0x7 (Northwest)

# Aliases
Direction.Northeast == Direction.Right
Direction.Southeast == Direction.Down
```

### Data Classes

```python
@dataclass
class MovementResult:
    can_move: bool
    new_z: int
    blocking_reason: Optional[str]
```

## Usage Examples

### Basic Movement Check

```python
from map_data_extractor import MapDataExtractor
from walkability_check import WalkabilityChecker
from movement_check import MovementChecker, Direction

extractor = MapDataExtractor(r"C:\UO\Client")
walk_checker = WalkabilityChecker(extractor)
move_checker = MovementChecker(extractor, walk_checker)

# Check if can move north
can_move, new_z = move_checker.check_movement(5400, 50, 33, Direction.North)
if can_move:
    print(f"Can move north to Z={new_z}")
```

### Pathfinding Support

```python
def simple_path(start, goal, checker):
    """Simple A* support function"""
    path = [start]
    current = start

    while current != goal:
        direction = checker.get_direction_toward(
            current[0], current[1], goal[0], goal[1]
        )

        can_move, new_z = checker.check_movement(
            current[0], current[1], current[2], direction
        )

        if can_move:
            dx, dy = checker.get_direction_offset(direction)
            current = (current[0] + dx, current[1] + dy, new_z)
            path.append(current)
        else:
            # Try alternate directions
            for alt_dir in checker.get_valid_directions(*current):
                dx, dy = checker.get_direction_offset(alt_dir)
                can_move, new_z = checker.check_movement(*current, alt_dir)
                if can_move:
                    current = (current[0] + dx, current[1] + dy, new_z)
                    path.append(current)
                    break
            else:
                return None  # Stuck

    return path
```

### AI Retreat Logic

```python
def retreat_from_enemy(pos, enemy_pos, checker, steps=5):
    """Retreat multiple steps from enemy"""
    x, y, z = pos
    enemy_x, enemy_y = enemy_pos

    for _ in range(steps):
        retreat_dir = checker.get_best_retreat_direction(x, y, z, enemy_x, enemy_y)

        if retreat_dir is None:
            print("CORNERED - Cannot retreat!")
            break

        can_move, new_z = checker.check_movement(x, y, z, retreat_dir)

        if can_move:
            dx, dy = checker.get_direction_offset(retreat_dir)
            x, y, z = x + dx, y + dy, new_z
            print(f"Retreated {retreat_dir.name} to ({x}, {y}, {z})")
        else:
            print(f"Cannot move {retreat_dir.name}")
            break

    return (x, y, z)
```

## Integration with Pathfinding

The movement checker provides the foundation for A* pathfinding:

1. **Neighbor generation**: Use `get_valid_directions()` to find passable neighbors
2. **Movement cost**: Count each step as 1, diagonals as ~1.414
3. **New Z tracking**: Use returned `new_z` for multi-level paths
4. **Heuristic**: Euclidean distance to goal

See `fast_astar.py` for full pathfinding implementation.
