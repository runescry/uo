# TileData Reference

## Overview

`tiledata_reference.py` provides tile flag lookups for walkability and movement checks. It supports both a fast hardcoded mode and full tiledata.mul parsing.

## Tile Flags

Tile flags determine how tiles interact with movement, line of sight, and other game mechanics.

### Key Flags for Movement

| Flag | Value | Description |
|------|-------|-------------|
| `Impassable` | 0x00000040 | Cannot walk through |
| `Surface` | 0x00000200 | Can stand on top |
| `Bridge` | 0x00000400 | Bridge tile (special Z handling) |
| `Wet` | 0x00000080 | Water tile |
| `Wall` | 0x00000010 | Wall (blocks LoS) |

### All Flags

```python
class TileFlags(IntFlag):
    None_ = 0x00000000
    Background = 0x00000001
    Weapon = 0x00000002
    Transparent = 0x00000004
    Translucent = 0x00000008
    Wall = 0x00000010
    Damaging = 0x00000020
    Impassable = 0x00000040
    Wet = 0x00000080
    Surface = 0x00000200
    Bridge = 0x00000400
    Window = 0x00001000
    NoShoot = 0x00002000
    Foliage = 0x00020000
    Roof = 0x08000000
    Door = 0x10000000
    StairBack = 0x20000000
    StairRight = 0x40000000
```

## tiledata.mul Format

### Land Tiles (0x0000 - 0x3FFF)

Organized in 512 groups of 32 tiles each.

**Group Structure:**
```
[Header: 4 bytes]
[32 entries × 26 bytes = 832 bytes]
```

**Land Entry (26 bytes):**
```
[Flags: 4 bytes]
[Texture ID: 2 bytes]
[Name: 20 bytes, null-terminated]
```

### Static Tiles (0x0000+)

Follow after land tiles, also in groups of 32.

**Static Entry (37 bytes):**
```
[Flags: 4 bytes]
[Weight: 1 byte]
[Quality/Layer: 1 byte]
[Misc data: 6 bytes]
[Unknown: 1 byte]
[Quantity: 1 byte]
[Animation ID: 2 bytes]
[Unknown: 1 byte]
[Hue: 1 byte]
[Stacking offset: 2 bytes]
[Height: 1 byte]
[Name: 20 bytes]
```

## API Reference

### TileDataReference Class

```python
class TileDataReference:
    def __init__(self, tiledata_path: Optional[str] = None)
```

**Modes:**
- **Hardcoded mode** (no path): Fast lookups using built-in tile lists
- **File mode** (with path): Complete data from tiledata.mul

### Flag Query Methods

```python
# Land tile checks
flags = ref.get_land_flags(tile_id)
is_impass = ref.is_impassable_land(tile_id)
is_wet = ref.is_water(tile_id)
can_walk = ref.can_walk_on_land(tile_id)

# Static tile checks
flags = ref.get_static_flags(tile_id)
is_impass = ref.is_impassable_static(tile_id)
is_surf = ref.is_surface(tile_id)
is_bridge = ref.is_bridge(tile_id)
is_wall = ref.is_wall(tile_id)
height = ref.get_static_height(tile_id)
can_walk = ref.can_walk_through_static(tile_id)
```

### Convenience Functions

```python
from tiledata_reference import is_blocking_tile, is_walkable_surface

# Quick checks without TileDataReference instance
if is_blocking_tile(0x0C99, is_static=True):
    print("This is a blocking tree")

if is_walkable_surface(0x04AB):
    print("This is a floor tile")
```

## Hardcoded Tile Lists

For fast simulation without tiledata.mul:

### Common Impassable Land Tiles
- Mountains: 220-285
- Void: 0x0244
- Lava: 1758-1765
- Cave walls: 1339-1345

### Common Water Land Tiles
- Water: 168-175, 310-311
- Deep water: 0xA8-0xAB

### Common Impassable Statics
- Stone walls: 0x0001-0x0005
- Brick walls: 0x001B-0x001F
- Dungeon walls: 0x0041-0x004F
- Rocks: 0x0DE0-0x0DE5
- Trees: 0x0C99-0x0CAD

### Common Surface Statics
- Stone floors: 0x04AB-0x04AE
- Wood floors: 0x0519-0x051C
- Bridges: 0x0763-0x0766
- Stairs: 0x071E-0x0721

## Usage Examples

### Basic Usage (Hardcoded Mode)

```python
from tiledata_reference import TileDataReference, TileFlags

ref = TileDataReference()

# Check if a mountain tile is impassable
if ref.is_impassable_land(220):
    print("Mountain is impassable")

# Check static flags
flags = ref.get_static_flags(0x0C99)
if flags & TileFlags.Impassable:
    print("Tree blocks movement")
```

### With tiledata.mul (Complete Data)

```python
ref = TileDataReference(r"C:\UO\tiledata.mul")

# Get tile name
name = ref.get_static_name(0x0C99)
print(f"Tile name: {name}")  # "pine tree" or similar

# Get exact height
height = ref.get_static_height(0x0C99)
print(f"Tree height: {height}")
```

### Integration with Map Extractor

```python
from map_data_extractor import MapDataExtractor
from tiledata_reference import TileDataReference

extractor = MapDataExtractor(r"C:\UO\Client")
ref = TileDataReference()

# Check walkability at a coordinate
terrain = extractor.get_terrain(5400, 50)
statics = extractor.get_statics(5400, 50)

walkable = ref.can_walk_on_land(terrain.tile_id)
for s in statics:
    if ref.is_impassable_static(s.tile_id):
        walkable = False
        break

print(f"Can walk at (5400, 50): {walkable}")
```

## Performance Notes

1. **Hardcoded mode** is ~10x faster for common tiles
2. **File mode** caches parsed entries automatically
3. For simulation, hardcoded mode covers 95%+ of cases
4. Use file mode only when you need exact heights or tile names
