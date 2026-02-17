# Walkability Check

## Overview

`walkability_check.py` implements the `Map.CanFit()` logic from ServUO, determining whether a creature can occupy a specific position.

## ServUO Reference

Ported from `Server/Map.cs`:
- `Map.CanFit()` - Main walkability check
- `Map.GetAverageZ()` - Terrain height averaging

## How CanFit Works

The check evaluates multiple factors:

### 1. Map Bounds
```python
if x < 0 or y < 0 or x >= map_width or y >= map_height:
    return False
```

### 2. Terrain Height (GetAverageZ)

Samples 4 corners of the terrain tile to handle slopes:
```
(x, y)     - zTop
(x, y+1)   - zLeft
(x+1, y)   - zRight
(x+1, y+1) - zBottom
```

Calculates:
- **low_z**: Minimum of 4 corners
- **top_z**: Maximum of 4 corners
- **avg_z**: Average (using pair with smaller difference)

### 3. Land Tile Check

```python
if (landFlags & Impassable) and avg_z > z and (z + height) > low_z:
    return False  # Impassable terrain blocks us
```

A surface exists if:
- Land is NOT impassable
- Our Z matches the average Z

### 4. Static Tile Check

For each static at (x, y):
```python
static_top = static.z + static_height

if (is_surface or is_impassable):
    if static_top > z and (z + height) > static.z:
        return False  # Static blocks our space
```

Statics can also provide surfaces:
```python
if is_surface and not is_impassable:
    if z == static_top:
        has_surface = True
```

### 5. Surface Requirement

If `require_surface=True` (default), we need a surface to stand on.

## API Reference

### WalkabilityChecker Class

```python
class WalkabilityChecker:
    def __init__(self,
                 map_extractor: MapDataExtractor,
                 tiledata: Optional[TileDataReference] = None,
                 map_width: int = 7168,
                 map_height: int = 4096)
```

### Core Methods

#### can_fit(x, y, z, height=16, check_blocks_fit=False, require_surface=True)
Main walkability check.

```python
checker = WalkabilityChecker(extractor)
can_walk = checker.can_fit(5400, 50, 33)
```

#### can_fit_detailed(x, y, z, ...)
Returns detailed result with blocking reason.

```python
result = checker.can_fit_detailed(5400, 50, 33)
print(f"Can fit: {result.can_fit}")
print(f"Has surface: {result.has_surface}")
print(f"Surface Z: {result.surface_z}")
print(f"Blocking reason: {result.blocking_reason}")
```

#### get_average_z(x, y)
Calculate terrain height averaging.

```python
avg = checker.get_average_z(5400, 50)
print(f"Low: {avg.low_z}, Avg: {avg.avg_z}, Top: {avg.top_z}")
```

#### get_surface_z(x, y, start_z=0)
Find the surface Z level at a position.

```python
surface_z = checker.get_surface_z(5400, 50)
```

#### find_valid_z(x, y, z, height=16, max_z_range=20)
Search for a valid Z near target position.

```python
valid_z = checker.find_valid_z(5400, 50, 33)
if valid_z is not None:
    print(f"Valid Z: {valid_z}")
```

#### is_walkable(x, y)
Simple check if any walkable surface exists.

```python
if checker.is_walkable(5400, 50):
    print("Position has a walkable surface")
```

#### get_blocking_statics(x, y, z, height=16)
Get list of statics that would block movement.

```python
blocking = checker.get_blocking_statics(5400, 50, 33)
for b in blocking:
    print(f"Blocked by tile {b['tile_id']} at Z={b['z']}")
```

### Data Classes

```python
@dataclass
class AverageZ:
    low_z: int    # Minimum Z of 4 corners
    avg_z: int    # Average Z (surface detection)
    top_z: int    # Maximum Z of 4 corners

@dataclass
class CanFitResult:
    can_fit: bool
    has_surface: bool
    surface_z: int
    blocking_reason: Optional[str]
```

## Constants

| Constant | Value | Description |
|----------|-------|-------------|
| `DEFAULT_HEIGHT` | 16 | Human creature height |
| `MAX_Z_STEP` | 8 | Maximum Z step up/down |

## Usage Examples

### Basic Walkability Check

```python
from map_data_extractor import MapDataExtractor
from walkability_check import WalkabilityChecker

extractor = MapDataExtractor(r"C:\UO\Client")
checker = WalkabilityChecker(extractor)

# Check specific position
if checker.can_fit(5400, 50, 33):
    print("Can walk here")
```

### Scan Area for Walkable Tiles

```python
walkable_tiles = []

for y in range(y1, y2 + 1):
    for x in range(x1, x2 + 1):
        avg = checker.get_average_z(x, y)
        if checker.can_fit(x, y, avg.avg_z):
            walkable_tiles.append((x, y, avg.avg_z))

print(f"Found {len(walkable_tiles)} walkable tiles")
```

### Debug Blocked Position

```python
result = checker.can_fit_detailed(5400, 50, 33)

if not result.can_fit:
    print(f"Cannot walk: {result.blocking_reason}")

    # Get detailed blocking info
    blocking = checker.get_blocking_statics(5400, 50, 33)
    for b in blocking:
        print(f"  Blocked by: tile_id={b['tile_id']}, Z={b['z']}-{b['top']}")
```

### Find Standing Position

```python
# Find valid Z for creature at target location
target_z = 33
valid_z = checker.find_valid_z(5400, 50, target_z, max_z_range=30)

if valid_z is not None:
    print(f"Can stand at Z={valid_z}")
else:
    print("No valid standing position found")
```

## Integration with Movement

The walkability checker is used by the movement system:

1. **Movement.CheckMovement()** calls `CanFit()` on destination
2. **Pathfinding** validates each step with `CanFit()`
3. **AI Movement** uses `find_valid_z()` for spawn/teleport

## Performance Tips

1. **Reuse checker instance** - avoids recreating TileDataReference
2. **Use is_walkable() for quick scans** - simpler than can_fit_detailed()
3. **Preload map region** before intensive checks:
   ```python
   extractor.preload_region(x1, y1, x2, y2)
   ```
