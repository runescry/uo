# Map Data Extractor

## Overview

`map_data_extractor.py` reads Ultima Online map files (.mul format) and provides on-demand access to terrain and static data with intelligent caching.

## UO Map File Format

### map0.mul - Terrain Data

The map file contains terrain tiles organized in 8x8 blocks.

**Block Structure (196 bytes):**
```
[Header: 4 bytes]
[64 cells: 64 × 3 bytes = 192 bytes]
```

**Cell Structure (3 bytes):**
```
[Tile ID: 2 bytes, unsigned short]
[Z: 1 byte, signed byte]
```

**Block Ordering (Column-Major):**
```
block_id = block_x * map_height_blocks + block_y
```

This is CentrED's formula, different from row-major ordering.

### staidx0.mul - Static Index

Index file for locating statics in each block.

**Entry Structure (12 bytes):**
```
[Offset: 4 bytes, unsigned int]  - Offset in statics0.mul (0xFFFFFFFF = empty)
[Length: 4 bytes, unsigned int]  - Length in bytes
[Unused: 4 bytes]
```

### statics0.mul - Static Items

Contains static items referenced by the index file.

**Static Entry (7 bytes):**
```
[Tile ID: 2 bytes, unsigned short]
[Local X: 1 byte]  - 0-7 within block
[Local Y: 1 byte]  - 0-7 within block
[Z: 1 byte, signed]
[Hue: 2 bytes, unsigned short]
```

## Map Dimensions

| Map | Width (blocks) | Height (blocks) | Width (tiles) | Height (tiles) |
|-----|----------------|-----------------|---------------|----------------|
| Felucca/Trammel | 896 | 512 | 7168 | 4096 |
| Ilshenar | 288 | 200 | 2304 | 1600 |
| Malas | 320 | 256 | 2560 | 2048 |
| Tokuno | 181 | 181 | 1448 | 1448 |

## API Reference

### MapDataExtractor Class

```python
class MapDataExtractor:
    def __init__(self, data_path: str, map_index: int = 0,
                 width_blocks: int = 896, height_blocks: int = 512)
```

**Parameters:**
- `data_path`: Directory containing map files
- `map_index`: Map number (0 = Felucca, etc.)
- `width_blocks`: Map width in 8x8 blocks
- `height_blocks`: Map height in 8x8 blocks

### Core Methods

#### get_terrain(x, y) -> TerrainTile
Get terrain tile at world coordinates.

```python
terrain = extractor.get_terrain(5400, 50)
print(f"Tile ID: {terrain.tile_id}, Z: {terrain.z}")
```

#### get_statics(x, y) -> List[StaticItem]
Get all static items at world coordinates.

```python
statics = extractor.get_statics(5400, 50)
for s in statics:
    print(f"Static {s.tile_id} at Z={s.z}")
```

#### get_terrain_z(x, y) -> int
Get terrain Z-level (convenience method).

#### get_highest_z(x, y) -> int
Get highest Z-level (terrain or static surface).

#### extract_region(x1, y1, x2, y2) -> dict
Extract all data in a rectangular region.

```python
region = extractor.extract_region(5379, 4, 5499, 125)
print(f"Terrain: {region['statistics']['total_terrain_tiles']}")
print(f"Statics: {region['statistics']['total_statics']}")
```

#### preload_region(x1, y1, x2, y2)
Preload blocks into cache for performance.

### Data Classes

```python
@dataclass
class TerrainTile:
    x: int          # World X coordinate
    y: int          # World Y coordinate
    tile_id: int    # Land tile ID
    z: int          # Z-level

@dataclass
class StaticItem:
    x: int          # World X coordinate
    y: int          # World Y coordinate
    z: int          # Z-level
    tile_id: int    # Static tile ID
    hue: int        # Color hue
```

## Caching Strategy

The extractor uses block-level caching:

1. **On first access**: Load entire 8x8 block (terrain + statics)
2. **Subsequent accesses**: Return from cache
3. **Cache statistics**: Track hits/misses for optimization

```python
stats = extractor.get_cache_stats()
print(f"Hit rate: {stats['hit_rate']*100:.1f}%")
```

## Performance Tips

1. **Preload regions** before simulation:
   ```python
   extractor.preload_region(5379, 4, 5499, 125)
   ```

2. **Reuse extractor instance** - don't create new ones per query

3. **Clear cache** if switching regions:
   ```python
   extractor.clear_cache()
   ```

## Example: Extract Dungeon Region

```python
from map_data_extractor import MapDataExtractor

# Initialize
extractor = MapDataExtractor(
    r"C:\Program Files (x86)\Electronic Arts\Ultima Online Classic"
)

# Extract test region
region = extractor.extract_region(5379, 4, 5499, 125)

# Analyze terrain
terrain_z_values = [t['z'] for t in region['terrain']]
print(f"Z range: {min(terrain_z_values)} to {max(terrain_z_values)}")

# Find impassable statics
from tiledata_reference import is_blocking_tile
blocking = [s for s in region['statics'] if is_blocking_tile(s['tile_id'])]
print(f"Blocking statics: {len(blocking)}")
```
