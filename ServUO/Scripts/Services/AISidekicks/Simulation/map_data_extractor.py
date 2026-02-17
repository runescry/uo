#!/usr/bin/env python3
r"""
Map Data Extractor for Combat Simulation

Reads UO map files (map0.mul, statics0.mul, staidx0.mul) with on-demand
coordinate extraction and caching for use in combat simulation.

Based on extraction patterns from Vystia Town Generator tools.

Usage:
    from map_data_extractor import MapDataExtractor

    extractor = MapDataExtractor("C:\\Program Files (x86)\\Electronic Arts\\Ultima Online Classic")

    # Get terrain at a specific coordinate
    terrain = extractor.get_terrain(5400, 50)
    print(f"Tile ID: {terrain.tile_id}, Z: {terrain.z}")

    # Get all statics at a coordinate
    statics = extractor.get_statics(5400, 50)
    for s in statics:
        print(f"Static: {s.tile_id} at Z={s.z}")

    # Extract a region for simulation
    region = extractor.extract_region(5379, 4, 5499, 125)
"""

import struct
from dataclasses import dataclass
from pathlib import Path
from typing import Dict, List, Optional, Tuple
import json


# Map dimensions (Malas/ML dimensions: 896x512 blocks)
# Can be overridden for different map files
DEFAULT_MAP_WIDTH_BLOCKS = 896   # 7168 tiles / 8
DEFAULT_MAP_HEIGHT_BLOCKS = 512  # 4096 tiles / 8

# Block structure sizes
BLOCK_SIZE = 196  # bytes per terrain block (4 byte header + 64 cells * 3 bytes)
STATIC_ENTRY_SIZE = 7  # bytes per static item
INDEX_ENTRY_SIZE = 12  # bytes per staidx entry


@dataclass
class TerrainTile:
    """Represents a single terrain tile from map0.mul"""
    x: int
    y: int
    tile_id: int
    z: int

    def to_dict(self) -> dict:
        return {
            'x': self.x,
            'y': self.y,
            'tile_id': self.tile_id,
            'z': self.z
        }


@dataclass
class StaticItem:
    """Represents a static item from statics0.mul"""
    x: int
    y: int
    z: int
    tile_id: int
    hue: int

    def to_dict(self) -> dict:
        return {
            'x': self.x,
            'y': self.y,
            'z': self.z,
            'tile_id': self.tile_id,
            'hue': self.hue
        }


@dataclass
class MapBlock:
    """Represents an 8x8 block of terrain tiles"""
    block_x: int
    block_y: int
    terrain: List[TerrainTile]
    statics: List[StaticItem]

    def get_terrain_at(self, x: int, y: int) -> Optional[TerrainTile]:
        """Get terrain tile at world coordinates within this block"""
        local_x = x % 8
        local_y = y % 8
        index = local_y * 8 + local_x
        if 0 <= index < len(self.terrain):
            return self.terrain[index]
        return None

    def get_statics_at(self, x: int, y: int) -> List[StaticItem]:
        """Get all statics at world coordinates within this block"""
        return [s for s in self.statics if s.x == x and s.y == y]


class MapDataExtractor:
    """
    Extracts and caches map data from UO .mul files.

    Uses on-demand loading with block-level caching for efficient
    access to terrain and static data during simulation.
    """

    def __init__(self,
                 data_path: str,
                 map_index: int = 0,
                 width_blocks: int = DEFAULT_MAP_WIDTH_BLOCKS,
                 height_blocks: int = DEFAULT_MAP_HEIGHT_BLOCKS):
        """
        Initialize the map data extractor.

        Args:
            data_path: Path to directory containing map files
            map_index: Map index (0 for Felucca/Trammel, etc.)
            width_blocks: Map width in blocks (default 896 for ML)
            height_blocks: Map height in blocks (default 512 for ML)
        """
        self.data_path = Path(data_path)
        self.map_index = map_index
        self.width_blocks = width_blocks
        self.height_blocks = height_blocks

        # File paths
        self.map_file = self.data_path / f"map{map_index}.mul"
        self.statics_file = self.data_path / f"statics{map_index}.mul"
        self.staidx_file = self.data_path / f"staidx{map_index}.mul"

        # Cache: block_id -> MapBlock
        self._block_cache: Dict[int, MapBlock] = {}

        # Statistics
        self._cache_hits = 0
        self._cache_misses = 0

        # Validate files exist
        self._validate_files()

    def _validate_files(self):
        """Validate that required map files exist"""
        if not self.map_file.exists():
            raise FileNotFoundError(f"Map file not found: {self.map_file}")
        if not self.statics_file.exists():
            raise FileNotFoundError(f"Statics file not found: {self.statics_file}")
        if not self.staidx_file.exists():
            raise FileNotFoundError(f"StaIdx file not found: {self.staidx_file}")

    def _get_block_id(self, block_x: int, block_y: int) -> int:
        """
        Calculate block ID using CentrED's formula.

        CentrED uses column-major ordering: blockNum = blockX * mapHeight + blockY
        """
        return block_x * self.height_blocks + block_y

    def _coords_to_block(self, x: int, y: int) -> Tuple[int, int]:
        """Convert world coordinates to block coordinates"""
        return x // 8, y // 8

    def _load_block(self, block_x: int, block_y: int) -> MapBlock:
        """
        Load a single 8x8 block from map files.

        Args:
            block_x: Block X coordinate
            block_y: Block Y coordinate

        Returns:
            MapBlock containing terrain and statics
        """
        block_id = self._get_block_id(block_x, block_y)

        # Check cache first
        if block_id in self._block_cache:
            self._cache_hits += 1
            return self._block_cache[block_id]

        self._cache_misses += 1

        # Load terrain
        terrain = self._load_terrain_block(block_x, block_y, block_id)

        # Load statics
        statics = self._load_statics_block(block_x, block_y, block_id)

        # Create and cache block
        block = MapBlock(
            block_x=block_x,
            block_y=block_y,
            terrain=terrain,
            statics=statics
        )

        self._block_cache[block_id] = block
        return block

    def _load_terrain_block(self, block_x: int, block_y: int, block_id: int) -> List[TerrainTile]:
        """Load terrain tiles for a single block"""
        terrain = []

        # Calculate file offset
        block_offset = block_id * BLOCK_SIZE

        try:
            with open(self.map_file, 'rb') as f:
                f.seek(block_offset)

                # Skip 4-byte block header
                f.read(4)

                # Read 64 terrain cells (8x8)
                for local_y in range(8):
                    for local_x in range(8):
                        tile_data = f.read(3)
                        if len(tile_data) < 3:
                            continue

                        tile_id = struct.unpack('<H', tile_data[0:2])[0]
                        z = struct.unpack('b', tile_data[2:3])[0]

                        world_x = block_x * 8 + local_x
                        world_y = block_y * 8 + local_y

                        terrain.append(TerrainTile(
                            x=world_x,
                            y=world_y,
                            tile_id=tile_id,
                            z=z
                        ))
        except (IOError, struct.error) as e:
            print(f"Warning: Error reading terrain block ({block_x}, {block_y}): {e}")

        return terrain

    def _load_statics_block(self, block_x: int, block_y: int, block_id: int) -> List[StaticItem]:
        """Load static items for a single block"""
        statics = []

        try:
            with open(self.staidx_file, 'rb') as idx_f:
                # Read index entry
                idx_f.seek(block_id * INDEX_ENTRY_SIZE)

                offset = struct.unpack('<I', idx_f.read(4))[0]
                length = struct.unpack('<I', idx_f.read(4))[0]
                # Third int is unused

                # Check for empty block
                if offset == 0xFFFFFFFF or length == 0:
                    return statics

                # Read statics
                with open(self.statics_file, 'rb') as sta_f:
                    sta_f.seek(offset)
                    count = length // STATIC_ENTRY_SIZE

                    for _ in range(count):
                        static_data = sta_f.read(STATIC_ENTRY_SIZE)
                        if len(static_data) < STATIC_ENTRY_SIZE:
                            break

                        tile_id = struct.unpack('<H', static_data[0:2])[0]
                        local_x = struct.unpack('B', static_data[2:3])[0]
                        local_y = struct.unpack('B', static_data[3:4])[0]
                        z = struct.unpack('b', static_data[4:5])[0]
                        hue = struct.unpack('<H', static_data[5:7])[0]

                        world_x = block_x * 8 + local_x
                        world_y = block_y * 8 + local_y

                        statics.append(StaticItem(
                            x=world_x,
                            y=world_y,
                            z=z,
                            tile_id=tile_id,
                            hue=hue
                        ))
        except (IOError, struct.error) as e:
            print(f"Warning: Error reading statics block ({block_x}, {block_y}): {e}")

        return statics

    def get_terrain(self, x: int, y: int) -> Optional[TerrainTile]:
        """
        Get terrain tile at world coordinates.

        Args:
            x: World X coordinate
            y: World Y coordinate

        Returns:
            TerrainTile at the specified coordinates, or None if not found
        """
        block_x, block_y = self._coords_to_block(x, y)
        block = self._load_block(block_x, block_y)
        return block.get_terrain_at(x, y)

    def get_terrain_z(self, x: int, y: int) -> int:
        """
        Get terrain Z-level at world coordinates.

        Args:
            x: World X coordinate
            y: World Y coordinate

        Returns:
            Z-level at the specified coordinates, or 0 if not found
        """
        terrain = self.get_terrain(x, y)
        return terrain.z if terrain else 0

    def get_statics(self, x: int, y: int) -> List[StaticItem]:
        """
        Get all static items at world coordinates.

        Args:
            x: World X coordinate
            y: World Y coordinate

        Returns:
            List of StaticItem at the specified coordinates
        """
        block_x, block_y = self._coords_to_block(x, y)
        block = self._load_block(block_x, block_y)
        return block.get_statics_at(x, y)

    def get_highest_z(self, x: int, y: int) -> int:
        """
        Get the highest Z-level at a coordinate (terrain or static).

        Useful for determining the surface Z for movement checks.

        Args:
            x: World X coordinate
            y: World Y coordinate

        Returns:
            Highest Z-level at the coordinate
        """
        terrain_z = self.get_terrain_z(x, y)
        statics = self.get_statics(x, y)

        if not statics:
            return terrain_z

        # Find highest static that could be a surface
        static_heights = [s.z for s in statics]
        max_static_z = max(static_heights) if static_heights else terrain_z

        return max(terrain_z, max_static_z)

    def extract_region(self, x1: int, y1: int, x2: int, y2: int) -> Dict:
        """
        Extract all terrain and statics within a rectangular region.

        Args:
            x1, y1: Top-left corner coordinates
            x2, y2: Bottom-right corner coordinates

        Returns:
            Dictionary containing terrain, statics, and statistics
        """
        # Ensure correct ordering
        min_x, max_x = min(x1, x2), max(x1, x2)
        min_y, max_y = min(y1, y2), max(y1, y2)

        terrain_list = []
        statics_list = []

        # Calculate block range
        min_block_x = min_x // 8
        max_block_x = max_x // 8
        min_block_y = min_y // 8
        max_block_y = max_y // 8

        # Load all blocks in range
        for block_y in range(min_block_y, max_block_y + 1):
            for block_x in range(min_block_x, max_block_x + 1):
                block = self._load_block(block_x, block_y)

                # Filter terrain within bounds
                for tile in block.terrain:
                    if min_x <= tile.x <= max_x and min_y <= tile.y <= max_y:
                        terrain_list.append(tile.to_dict())

                # Filter statics within bounds
                for static in block.statics:
                    if min_x <= static.x <= max_x and min_y <= static.y <= max_y:
                        statics_list.append(static.to_dict())

        # Calculate statistics
        terrain_z_values = [t['z'] for t in terrain_list]
        static_z_values = [s['z'] for s in statics_list]

        return {
            'bounds': {
                'x1': min_x,
                'y1': min_y,
                'x2': max_x,
                'y2': max_y,
                'width': max_x - min_x + 1,
                'height': max_y - min_y + 1
            },
            'terrain': terrain_list,
            'statics': statics_list,
            'statistics': {
                'total_terrain_tiles': len(terrain_list),
                'total_statics': len(statics_list),
                'terrain_z_range': [min(terrain_z_values), max(terrain_z_values)] if terrain_z_values else [0, 0],
                'terrain_z_avg': sum(terrain_z_values) / len(terrain_z_values) if terrain_z_values else 0,
                'static_z_range': [min(static_z_values), max(static_z_values)] if static_z_values else [0, 0],
                'static_z_avg': sum(static_z_values) / len(static_z_values) if static_z_values else 0,
                'blocks_loaded': len(self._block_cache),
                'cache_hits': self._cache_hits,
                'cache_misses': self._cache_misses
            }
        }

    def preload_region(self, x1: int, y1: int, x2: int, y2: int):
        """
        Preload all blocks in a region into cache.

        Useful for simulation where you'll be accessing the same area repeatedly.

        Args:
            x1, y1: Top-left corner coordinates
            x2, y2: Bottom-right corner coordinates
        """
        min_x, max_x = min(x1, x2), max(x1, x2)
        min_y, max_y = min(y1, y2), max(y1, y2)

        min_block_x = min_x // 8
        max_block_x = max_x // 8
        min_block_y = min_y // 8
        max_block_y = max_y // 8

        for block_y in range(min_block_y, max_block_y + 1):
            for block_x in range(min_block_x, max_block_x + 1):
                self._load_block(block_x, block_y)

    def clear_cache(self):
        """Clear the block cache"""
        self._block_cache.clear()
        self._cache_hits = 0
        self._cache_misses = 0

    def get_cache_stats(self) -> Dict:
        """Get cache statistics"""
        total = self._cache_hits + self._cache_misses
        hit_rate = self._cache_hits / total if total > 0 else 0

        return {
            'blocks_cached': len(self._block_cache),
            'cache_hits': self._cache_hits,
            'cache_misses': self._cache_misses,
            'hit_rate': hit_rate
        }

    def save_region_to_json(self, x1: int, y1: int, x2: int, y2: int,
                           output_path: str):
        """
        Extract a region and save to JSON file.

        Args:
            x1, y1: Top-left corner coordinates
            x2, y2: Bottom-right corner coordinates
            output_path: Path to save JSON file
        """
        region = self.extract_region(x1, y1, x2, y2)

        with open(output_path, 'w') as f:
            json.dump(region, f, indent=2)

        print(f"Saved region to {output_path}")
        print(f"  Terrain tiles: {region['statistics']['total_terrain_tiles']}")
        print(f"  Static items: {region['statistics']['total_statics']}")


def detect_map_dimensions(map_path: Path) -> Tuple[int, int]:
    """
    Detect map dimensions from file size.

    Maps use column-major ordering: block_idx = block_x * height_blocks + block_y

    Args:
        map_path: Path to map.mul file

    Returns:
        Tuple of (width_blocks, height_blocks)
    """
    if not map_path.exists():
        raise FileNotFoundError(f"Map file not found: {map_path}")

    file_size = map_path.stat().st_size
    total_blocks = file_size // BLOCK_SIZE

    if total_blocks == 0:
        return (0, 0)

    # Try common height values first
    common_heights = [512, 204, 160, 144, 256, 128, 64]

    for height_blocks in common_heights:
        if total_blocks % height_blocks == 0:
            width_blocks = total_blocks // height_blocks
            return (width_blocks, height_blocks)

    # Fallback: try to find factors with reasonable aspect ratio
    for height_blocks in range(64, 1024, 8):
        if total_blocks % height_blocks == 0:
            width_blocks = total_blocks // height_blocks
            if 0.5 <= (width_blocks / height_blocks) <= 4.0:
                return (width_blocks, height_blocks)

    # Last resort: assume square-ish
    import math
    height_blocks = int(math.sqrt(total_blocks))
    height_blocks = (height_blocks // 8) * 8
    if height_blocks == 0:
        height_blocks = 8
    width_blocks = total_blocks // height_blocks
    return (width_blocks, height_blocks)


# Module-level convenience functions
_default_extractor: Optional[MapDataExtractor] = None


def init_extractor(data_path: str, map_index: int = 0) -> MapDataExtractor:
    """Initialize the default map extractor"""
    global _default_extractor
    _default_extractor = MapDataExtractor(data_path, map_index)
    return _default_extractor


def get_terrain(x: int, y: int) -> Optional[TerrainTile]:
    """Get terrain at coordinates using default extractor"""
    if _default_extractor is None:
        raise RuntimeError("Map extractor not initialized. Call init_extractor() first.")
    return _default_extractor.get_terrain(x, y)


def get_statics(x: int, y: int) -> List[StaticItem]:
    """Get statics at coordinates using default extractor"""
    if _default_extractor is None:
        raise RuntimeError("Map extractor not initialized. Call init_extractor() first.")
    return _default_extractor.get_statics(x, y)


def get_terrain_z(x: int, y: int) -> int:
    """Get terrain Z at coordinates using default extractor"""
    if _default_extractor is None:
        raise RuntimeError("Map extractor not initialized. Call init_extractor() first.")
    return _default_extractor.get_terrain_z(x, y)


# Test function
def main():
    """Test the map data extractor on the dungeon test region"""
    import sys

    # Default UO client path
    default_path = r"C:\Program Files (x86)\Electronic Arts\Ultima Online Classic"

    if len(sys.argv) > 1:
        data_path = sys.argv[1]
    else:
        data_path = default_path

    print("=" * 80)
    print("MAP DATA EXTRACTOR TEST")
    print("=" * 80)
    print(f"Data path: {data_path}")

    try:
        # Initialize extractor
        extractor = MapDataExtractor(data_path)

        # Test region from Combat Sim Plan: (5379, 4) to (5499, 125)
        test_x1, test_y1 = 5379, 4
        test_x2, test_y2 = 5499, 125

        print(f"\nTest Region: ({test_x1}, {test_y1}) to ({test_x2}, {test_y2})")
        print(f"Size: {test_x2 - test_x1 + 1}x{test_y2 - test_y1 + 1} tiles")

        # Test single coordinate access
        test_x, test_y = 5400, 50
        print(f"\nSingle coordinate test ({test_x}, {test_y}):")

        terrain = extractor.get_terrain(test_x, test_y)
        if terrain:
            print(f"  Terrain: tile_id={terrain.tile_id}, z={terrain.z}")
        else:
            print("  Terrain: Not found")

        statics = extractor.get_statics(test_x, test_y)
        print(f"  Statics: {len(statics)} items")
        for s in statics[:5]:  # Show first 5
            print(f"    - tile_id={s.tile_id}, z={s.z}, hue={s.hue}")

        highest_z = extractor.get_highest_z(test_x, test_y)
        print(f"  Highest Z: {highest_z}")

        # Extract full region
        print(f"\nExtracting full region...")
        region = extractor.extract_region(test_x1, test_y1, test_x2, test_y2)

        print(f"\nRegion Statistics:")
        print(f"  Terrain tiles: {region['statistics']['total_terrain_tiles']}")
        print(f"  Static items: {region['statistics']['total_statics']}")
        print(f"  Terrain Z range: {region['statistics']['terrain_z_range']}")
        print(f"  Terrain Z avg: {region['statistics']['terrain_z_avg']:.1f}")
        print(f"  Static Z range: {region['statistics']['static_z_range']}")
        print(f"  Static Z avg: {region['statistics']['static_z_avg']:.1f}")

        # Cache stats
        cache_stats = extractor.get_cache_stats()
        print(f"\nCache Statistics:")
        print(f"  Blocks cached: {cache_stats['blocks_cached']}")
        print(f"  Cache hits: {cache_stats['cache_hits']}")
        print(f"  Cache misses: {cache_stats['cache_misses']}")
        print(f"  Hit rate: {cache_stats['hit_rate']*100:.1f}%")

        # Save to JSON
        output_path = Path(__file__).parent / "test_region_data.json"
        extractor.save_region_to_json(test_x1, test_y1, test_x2, test_y2, str(output_path))

        print("\n" + "=" * 80)
        print("TEST COMPLETE")
        print("=" * 80)

    except FileNotFoundError as e:
        print(f"\nError: {e}")
        print("\nPlease provide the path to your UO client directory as an argument:")
        print(f"  python {sys.argv[0]} <path_to_uo_client>")
        sys.exit(1)


if __name__ == "__main__":
    main()
