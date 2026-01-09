# Sidekick AI Movement

## Overview

`sidekick_ai_movement.py` implements the Sidekick AI movement logic for mage combat simulation. It handles movement toward targets, retreating from enemies, and maintaining optimal casting range.

## ServUO Reference

Ported from `Scripts/Services/AISidekicks/AI/SidekickAI.cs`:
- `MoveTo()` - Move toward target
- `RunFrom()` - Retreat from enemy
- `WalkMobileRange()` - Maintain range
- `DoMove()` - Execute single step

## Spell Casting Ranges

| Range Type | Tiles | Usage |
|------------|-------|-------|
| MELEE | 1 | Touch spells |
| CLOSE | 8 | Close range spells |
| MEDIUM | 12 | Standard spells |
| FAR | 20 | Long range spells |

## API Reference

### SidekickMovement Class

```python
class SidekickMovement:
    def __init__(self, pathfinder: FastAStar, move_checker: MovementChecker)
```

### Position Methods

```python
# Set/get position
ai.set_position(5400, 50, 33)
pos = ai.get_position()  # Returns (x, y, z) tuple

# Distance calculations
dist = ai.get_distance_to(target_x, target_y)

# Direction to target
direction = ai.get_direction_to(target_x, target_y)
```

### Movement Methods

#### move_to(target_x, target_y, target_z, run=True, range=1)
Move toward a target position.

```python
result = ai.move_to(5420, 70, 38, run=True, range=1)
if result == MoveResult.AtDestination:
    print("Arrived!")
```

#### move_to_target(target_x, target_y, target_z, casting_range=12)
Move toward target, stopping at casting range.

```python
result = ai.move_to_target(enemy_x, enemy_y, enemy_z, casting_range=12)
```

#### run_from(target_x, target_y, run=True, preferred_corner=None)
Retreat from a target.

```python
result = ai.run_from(enemy_x, enemy_y)

# With preferred corner
result = ai.run_from(enemy_x, enemy_y, preferred_corner=(5380, 10, 33))
```

#### maintain_casting_range(target_x, target_y, target_z, casting_range=12, min_distance=4)
Stay within optimal casting range.

```python
result = ai.maintain_casting_range(
    enemy_x, enemy_y, enemy_z,
    casting_range=12,
    min_distance=4
)
```

#### walk_mobile_range(target_x, target_y, target_z, steps, run, min_range, max_range)
General range maintenance.

```python
result = ai.walk_mobile_range(
    enemy_x, enemy_y, enemy_z,
    steps=3,
    run=True,
    min_range=8,
    max_range=20
)
```

#### do_move(direction, run=False)
Execute a single movement step.

```python
from movement_check import Direction

result = ai.do_move(Direction.North, run=True)
```

### Utility Methods

#### can_reach(target_x, target_y, target_z)
Check if target is reachable.

```python
if ai.can_reach(target_x, target_y, target_z):
    print("Path exists")
```

#### get_valid_directions()
Get available movement directions.

```python
valid = ai.get_valid_directions()
print(f"Can move: {[d.name for d in valid]}")
```

#### is_cornered(enemy_x, enemy_y)
Check if cornered (limited retreat options).

```python
if ai.is_cornered(enemy_x, enemy_y):
    print("CORNERED - Fight or die!")
```

#### get_stats()
Get movement statistics.

```python
stats = ai.get_stats()
print(f"Moves made: {stats['moves_made']}")
print(f"Paths calculated: {stats['paths_calculated']}")
```

### MoveResult Enum

| Value | Description |
|-------|-------------|
| `Success` | Movement executed |
| `Blocked` | Movement blocked |
| `AtDestination` | Already at target/range |
| `PathNotFound` | No path exists |
| `BadState` | Cannot move (casting, etc.) |

## Usage Examples

### Basic Movement

```python
from sidekick_ai_movement import SidekickMovement, MoveResult
from fast_astar import FastAStar
from movement_check import MovementChecker

# Initialize
ai = SidekickMovement(pathfinder, move_checker)
ai.set_position(5400, 50, 33)

# Move toward target
while True:
    result = ai.move_to_target(5420, 70, 38, casting_range=12)

    if result == MoveResult.AtDestination:
        print("In casting range!")
        break
    elif result == MoveResult.PathNotFound:
        print("Cannot reach target!")
        break
```

### Combat Kiting

```python
def kite_enemy(ai, enemy_x, enemy_y, enemy_z):
    """Mage kiting behavior"""
    distance = ai.get_distance_to(enemy_x, enemy_y)

    if distance < 4:
        # Too close - retreat!
        ai.run_from(enemy_x, enemy_y)
        return "retreating"
    elif distance > 12:
        # Too far - approach
        ai.move_to_target(enemy_x, enemy_y, enemy_z, casting_range=12)
        return "approaching"
    else:
        # In range - can cast
        return "casting"
```

### Retreat with Corner Preference

```python
def retreat_to_corner(ai, enemy_x, enemy_y, corner_positions):
    """Retreat toward nearest safe corner"""
    # Find closest corner
    best_corner = None
    best_dist = float('inf')

    for corner in corner_positions:
        dist = ai.get_distance_to(corner[0], corner[1])
        if dist < best_dist:
            best_dist = dist
            best_corner = corner

    # Retreat toward corner
    if best_corner and best_dist <= 8:
        return ai.run_from(enemy_x, enemy_y, preferred_corner=best_corner)
    else:
        return ai.run_from(enemy_x, enemy_y)
```

### Combat Simulation Loop

```python
def simulate_combat_movement(ai, enemy_x, enemy_y, enemy_z, max_turns=100):
    """Simulate mage vs enemy movement"""
    turns = 0
    states = []

    while turns < max_turns:
        pos = ai.get_position()
        dist = ai.get_distance_to(enemy_x, enemy_y)

        # Record state
        states.append({
            'turn': turns,
            'position': pos,
            'distance': dist
        })

        # Mage behavior
        if dist < 4:
            result = ai.run_from(enemy_x, enemy_y)
        elif dist > 12:
            result = ai.move_to_target(enemy_x, enemy_y, enemy_z)
        else:
            # In casting range - simulate cast
            break

        if result == MoveResult.Blocked and ai.is_cornered(enemy_x, enemy_y):
            print("CORNERED!")
            break

        turns += 1

    return states
```

## Integration with Combat Simulator

The `SidekickMovement` class provides the movement layer for `mage_combat_simulator.py`:

```python
from sidekick_ai_movement import SidekickMovement

# In combat simulator
class MageCombatSimulator:
    def __init__(self, ...):
        self.mage_movement = SidekickMovement(pathfinder, move_checker)

    def mage_move_phase(self):
        if self.should_retreat():
            return self.mage_movement.run_from(self.enemy_x, self.enemy_y)
        else:
            return self.mage_movement.maintain_casting_range(
                self.enemy_x, self.enemy_y, self.enemy_z
            )
```

## Performance Notes

1. **Direct movement first**: Uses direct movement before calculating paths
2. **Path caching**: Reuses paths to same target
3. **Statistics tracking**: Monitor with `get_stats()` for optimization
4. **Reset stats**: Use `reset_stats()` between simulations
