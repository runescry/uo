#!/usr/bin/env python3
r"""
TileData Reference for Combat Simulation

Provides tile flag lookups (Impassable, Surface, etc.) for walkability checks.
Supports both hardcoded common tiles and full tiledata.mul parsing.

Based on UOFiddler_tools patterns and ServUO TileData structures.

Usage:
    from tiledata_reference import TileDataReference, TileFlags

    # Using hardcoded reference (fast, no file needed)
    ref = TileDataReference()
    if ref.is_impassable(0x0001):
        print("Tile is impassable")

    # Using tiledata.mul file (complete data)
    ref = TileDataReference("C:\\UO\\tiledata.mul")
    flags = ref.get_land_flags(0x0001)
    if flags & TileFlags.Impassable:
        print("Tile is impassable")
"""

import struct
from dataclasses import dataclass
from enum import IntFlag
from pathlib import Path
from typing import Dict, List, Optional, Set, Tuple


class TileFlags(IntFlag):
    """
    Tile flags from UO TileData.

    These flags determine how tiles interact with movement, LoS, etc.
    Based on ServUO's TileFlag enum.
    """
    None_ = 0x00000000
    Background = 0x00000001
    Weapon = 0x00000002
    Transparent = 0x00000004
    Translucent = 0x00000008
    Wall = 0x00000010
    Damaging = 0x00000020
    Impassable = 0x00000040
    Wet = 0x00000080
    Unknown1 = 0x00000100
    Surface = 0x00000200
    Bridge = 0x00000400
    Generic = 0x00000800  # Stackable for items
    Window = 0x00001000
    NoShoot = 0x00002000
    ArticleA = 0x00004000
    ArticleAn = 0x00008000
    Internal = 0x00010000  # Monogen for items
    Foliage = 0x00020000
    PartialHue = 0x00040000
    Unknown2 = 0x00080000  # NoHouse for items
    Map = 0x00100000
    Container = 0x00200000
    Wearable = 0x00400000
    LightSource = 0x00800000
    Animation = 0x01000000
    HoverOver = 0x02000000  # NoDiagonal for items
    Unknown3 = 0x04000000  # Armor for items
    Roof = 0x08000000
    Door = 0x10000000
    StairBack = 0x20000000
    StairRight = 0x40000000
    # High flag (for HS client extended flags)
    AlphaBlend = 0x80000000


@dataclass
class LandTileData:
    """Data for a land (terrain) tile"""
    tile_id: int
    flags: TileFlags
    texture_id: int
    name: str


@dataclass
class StaticTileData:
    """Data for a static (item) tile"""
    tile_id: int
    flags: TileFlags
    weight: int
    quality: int  # Layer for wearables
    quantity: int
    anim_id: int
    unknown1: int
    hue: int
    unknown2: int
    height: int
    name: str


# Common terrain tiles that are impassable (walls, rocks, mountains, etc.)
# This hardcoded list covers the most common cases for fast lookups
COMMON_IMPASSABLE_LAND: Set[int] = {
    # Mountain/rock tiles
    220, 221, 222, 223, 224, 225, 226, 227, 228, 229,
    230, 231, 232, 233, 234, 235, 236, 237, 238, 239,
    240, 241, 242, 243, 244, 245, 246, 247, 248, 249,
    250, 251, 252, 253, 254, 255,
    # More mountain ranges
    256, 257, 258, 259, 260, 261, 262, 263, 264, 265,
    266, 267, 268, 269, 270, 271, 272, 273, 274, 275,
    276, 277, 278, 279, 280, 281, 282, 283, 284, 285,
    # Void/impassable
    0x0244,  # Void tile
    # Lava
    1758, 1759, 1760, 1761, 1762, 1763, 1764, 1765,
    # Cave walls
    1339, 1340, 1341, 1342, 1343, 1344, 1345,
}

# Common terrain tiles that are water (wet flag)
COMMON_WATER_LAND: Set[int] = {
    # Water tiles
    168, 169, 170, 171, 172, 173, 174, 175,
    310, 311,
    # Deep water
    0xA8, 0xA9, 0xAA, 0xAB,
}

# Common static tiles that are impassable
COMMON_IMPASSABLE_STATICS: Set[int] = {
    # Stone walls
    0x0001, 0x0002, 0x0003, 0x0004, 0x0005,
    # Brick walls
    0x001B, 0x001C, 0x001D, 0x001E, 0x001F,
    # Wooden walls
    0x002A, 0x002B, 0x002C, 0x002D,
    # Dungeon walls
    0x0041, 0x0042, 0x0043, 0x0044, 0x0045, 0x0046, 0x0047, 0x0048,
    0x0049, 0x004A, 0x004B, 0x004C, 0x004D, 0x004E, 0x004F,
    # Rocks/boulders
    0x0DE0, 0x0DE1, 0x0DE2, 0x0DE3, 0x0DE4, 0x0DE5,
    # Trees (blocking)
    0x0C99, 0x0C9A, 0x0C9B, 0x0C9C, 0x0C9D, 0x0C9E, 0x0C9F,
    0x0CA0, 0x0CA1, 0x0CA2, 0x0CA3, 0x0CA4, 0x0CA5, 0x0CA6,
    0x0CA7, 0x0CA8, 0x0CA9, 0x0CAA, 0x0CAB, 0x0CAC, 0x0CAD,
    # Cave/dungeon rocks
    0x177A, 0x177B, 0x177C, 0x177D,
}

# Common static tiles that are surfaces (can stand on)
COMMON_SURFACE_STATICS: Set[int] = {
    # Floors
    0x04AB, 0x04AC, 0x04AD, 0x04AE,  # Stone floors
    0x0519, 0x051A, 0x051B, 0x051C,  # Wood floors
    # Bridges
    0x0763, 0x0764, 0x0765, 0x0766,
    # Stairs
    0x071E, 0x071F, 0x0720, 0x0721,
}


class TileDataReference:
    """
    Provides tile data lookups for walkability and movement checks.

    Can operate in two modes:
    1. Hardcoded mode (default): Uses built-in lists of common tiles
    2. File mode: Parses tiledata.mul for complete data
    """

    # TileData file structure constants
    LAND_GROUP_SIZE = 4 + (32 * 26)  # Header (4) + 32 entries * 26 bytes
    STATIC_GROUP_SIZE = 4 + (32 * 37)  # Header (4) + 32 entries * 37 bytes
    LAND_GROUPS = 512  # Total land groups
    LAND_ENTRY_SIZE = 26  # Old format: flags(4) + texID(2) + name(20)
    STATIC_ENTRY_SIZE = 37  # Old format: flags(4) + weight(1) + quality(1) + misc(8) + unk(1) + quantity(1) + animID(2) + unk(1) + hue(1) + stacking(2) + height(1) + name(20)

    def __init__(self, tiledata_path: Optional[str] = None):
        """
        Initialize the tile data reference.

        Args:
            tiledata_path: Optional path to tiledata.mul for complete data.
                          If not provided, uses hardcoded common tiles only.
        """
        self.tiledata_path = Path(tiledata_path) if tiledata_path else None
        self.use_file = tiledata_path is not None and Path(tiledata_path).exists()

        # Caches for file-based lookups
        self._land_cache: Dict[int, LandTileData] = {}
        self._static_cache: Dict[int, StaticTileData] = {}

        # Pre-parse common ranges if using file
        if self.use_file:
            self._validate_file()

    def _validate_file(self):
        """Validate tiledata.mul file exists and has reasonable size"""
        if not self.tiledata_path.exists():
            raise FileNotFoundError(f"TileData file not found: {self.tiledata_path}")

        file_size = self.tiledata_path.stat().st_size
        min_expected = self.LAND_GROUPS * self.LAND_GROUP_SIZE
        if file_size < min_expected:
            print(f"Warning: TileData file smaller than expected ({file_size} < {min_expected})")

    def _read_land_tile(self, tile_id: int) -> Optional[LandTileData]:
        """Read a single land tile entry from tiledata.mul"""
        if tile_id < 0 or tile_id >= 0x4000:  # Land tiles: 0x0000 - 0x3FFF
            return None

        if tile_id in self._land_cache:
            return self._land_cache[tile_id]

        group = tile_id // 32
        entry_in_group = tile_id % 32

        try:
            with open(self.tiledata_path, 'rb') as f:
                # Seek to group
                group_offset = group * self.LAND_GROUP_SIZE
                f.seek(group_offset)

                # Skip header
                f.read(4)

                # Seek to entry
                f.seek(group_offset + 4 + entry_in_group * self.LAND_ENTRY_SIZE)

                # Read entry
                flags = TileFlags(struct.unpack('<I', f.read(4))[0])
                texture_id = struct.unpack('<H', f.read(2))[0]
                name_bytes = f.read(20)
                name = name_bytes.split(b'\0', 1)[0].decode('utf-8', 'ignore').strip()

                tile_data = LandTileData(
                    tile_id=tile_id,
                    flags=flags,
                    texture_id=texture_id,
                    name=name
                )

                self._land_cache[tile_id] = tile_data
                return tile_data

        except (IOError, struct.error) as e:
            print(f"Warning: Error reading land tile {tile_id}: {e}")
            return None

    def _read_static_tile(self, tile_id: int) -> Optional[StaticTileData]:
        """Read a single static tile entry from tiledata.mul"""
        if tile_id < 0:
            return None

        if tile_id in self._static_cache:
            return self._static_cache[tile_id]

        group = tile_id // 32
        entry_in_group = tile_id % 32

        # Calculate offset (after all land groups)
        land_section_size = self.LAND_GROUPS * self.LAND_GROUP_SIZE
        static_offset = land_section_size + group * self.STATIC_GROUP_SIZE

        try:
            with open(self.tiledata_path, 'rb') as f:
                # Seek to group
                f.seek(static_offset)

                # Skip header
                f.read(4)

                # Seek to entry
                f.seek(static_offset + 4 + entry_in_group * self.STATIC_ENTRY_SIZE)

                # Read entry (old format, 37 bytes)
                flags = TileFlags(struct.unpack('<I', f.read(4))[0])
                weight = struct.unpack('B', f.read(1))[0]
                quality = struct.unpack('B', f.read(1))[0]
                # Skip misc data (6 bytes)
                f.read(6)
                unknown1 = struct.unpack('B', f.read(1))[0]
                quantity = struct.unpack('B', f.read(1))[0]
                anim_id = struct.unpack('<H', f.read(2))[0]
                unknown2 = struct.unpack('B', f.read(1))[0]
                hue = struct.unpack('B', f.read(1))[0]
                # Skip stacking offset (2 bytes)
                f.read(2)
                height = struct.unpack('B', f.read(1))[0]
                name_bytes = f.read(20)
                name = name_bytes.split(b'\0', 1)[0].decode('utf-8', 'ignore').strip()

                tile_data = StaticTileData(
                    tile_id=tile_id,
                    flags=flags,
                    weight=weight,
                    quality=quality,
                    quantity=quantity,
                    anim_id=anim_id,
                    unknown1=unknown1,
                    hue=hue,
                    unknown2=unknown2,
                    height=height,
                    name=name
                )

                self._static_cache[tile_id] = tile_data
                return tile_data

        except (IOError, struct.error) as e:
            print(f"Warning: Error reading static tile {tile_id}: {e}")
            return None

    def get_land_flags(self, tile_id: int) -> TileFlags:
        """
        Get flags for a land (terrain) tile.

        Args:
            tile_id: Land tile ID

        Returns:
            TileFlags for the tile
        """
        if self.use_file:
            tile_data = self._read_land_tile(tile_id)
            return tile_data.flags if tile_data else TileFlags.None_
        else:
            # Use hardcoded reference
            flags = TileFlags.None_
            if tile_id in COMMON_IMPASSABLE_LAND:
                flags |= TileFlags.Impassable
            if tile_id in COMMON_WATER_LAND:
                flags |= TileFlags.Wet
            return flags

    def get_static_flags(self, tile_id: int) -> TileFlags:
        """
        Get flags for a static (item) tile.

        Args:
            tile_id: Static tile ID

        Returns:
            TileFlags for the tile
        """
        if self.use_file:
            tile_data = self._read_static_tile(tile_id)
            return tile_data.flags if tile_data else TileFlags.None_
        else:
            # Use hardcoded reference
            flags = TileFlags.None_
            if tile_id in COMMON_IMPASSABLE_STATICS:
                flags |= TileFlags.Impassable
            if tile_id in COMMON_SURFACE_STATICS:
                flags |= TileFlags.Surface
            return flags

    def get_static_height(self, tile_id: int) -> int:
        """
        Get height of a static tile.

        Args:
            tile_id: Static tile ID

        Returns:
            Height in Z-units (0 if not found or using hardcoded mode)
        """
        if self.use_file:
            tile_data = self._read_static_tile(tile_id)
            return tile_data.height if tile_data else 0
        else:
            # Hardcoded heights for common items
            # Most walls are ~20 high, floors are 0
            if tile_id in COMMON_IMPASSABLE_STATICS:
                return 20
            if tile_id in COMMON_SURFACE_STATICS:
                return 0
            return 0

    def is_impassable_land(self, tile_id: int) -> bool:
        """Check if a land tile is impassable"""
        flags = self.get_land_flags(tile_id)
        return bool(flags & TileFlags.Impassable)

    def is_impassable_static(self, tile_id: int) -> bool:
        """Check if a static tile is impassable"""
        flags = self.get_static_flags(tile_id)
        return bool(flags & TileFlags.Impassable)

    def is_surface(self, tile_id: int) -> bool:
        """Check if a static tile is a surface (can stand on)"""
        flags = self.get_static_flags(tile_id)
        return bool(flags & TileFlags.Surface)

    def is_bridge(self, tile_id: int) -> bool:
        """Check if a static tile is a bridge"""
        flags = self.get_static_flags(tile_id)
        return bool(flags & TileFlags.Bridge)

    def is_water(self, tile_id: int) -> bool:
        """Check if a land tile is water"""
        flags = self.get_land_flags(tile_id)
        return bool(flags & TileFlags.Wet)

    def is_wall(self, tile_id: int) -> bool:
        """Check if a static tile is a wall"""
        flags = self.get_static_flags(tile_id)
        return bool(flags & TileFlags.Wall)

    def can_walk_on_land(self, tile_id: int) -> bool:
        """
        Check if a land tile can be walked on.

        Returns True if not impassable and not water.
        """
        flags = self.get_land_flags(tile_id)
        return not (flags & (TileFlags.Impassable | TileFlags.Wet))

    def can_walk_through_static(self, tile_id: int) -> bool:
        """
        Check if a static tile can be walked through.

        Returns True if not impassable and not a wall.
        """
        flags = self.get_static_flags(tile_id)
        return not (flags & (TileFlags.Impassable | TileFlags.Wall))

    def get_land_name(self, tile_id: int) -> str:
        """Get the name of a land tile"""
        if self.use_file:
            tile_data = self._read_land_tile(tile_id)
            return tile_data.name if tile_data else f"Unknown Land {tile_id}"
        return f"Land {tile_id}"

    def get_static_name(self, tile_id: int) -> str:
        """Get the name of a static tile"""
        if self.use_file:
            tile_data = self._read_static_tile(tile_id)
            return tile_data.name if tile_data else f"Unknown Static {tile_id}"
        return f"Static {tile_id}"

    def clear_cache(self):
        """Clear the tile data cache"""
        self._land_cache.clear()
        self._static_cache.clear()

    def get_cache_stats(self) -> Dict:
        """Get cache statistics"""
        return {
            'land_cached': len(self._land_cache),
            'static_cached': len(self._static_cache),
            'using_file': self.use_file
        }


# Convenience functions for common checks
def is_blocking_tile(tile_id: int, is_static: bool = True) -> bool:
    """
    Quick check if a tile blocks movement.

    Uses hardcoded reference for fast lookups without needing tiledata.mul.

    Args:
        tile_id: Tile ID to check
        is_static: True if static tile, False if land tile

    Returns:
        True if the tile blocks movement
    """
    if is_static:
        return tile_id in COMMON_IMPASSABLE_STATICS
    else:
        return tile_id in COMMON_IMPASSABLE_LAND


def is_walkable_surface(tile_id: int) -> bool:
    """
    Quick check if a static tile is a walkable surface.

    Args:
        tile_id: Static tile ID

    Returns:
        True if the tile is a surface that can be walked on
    """
    return tile_id in COMMON_SURFACE_STATICS


def get_tile_height_estimate(tile_id: int) -> int:
    """
    Get estimated height of a static tile without tiledata.mul.

    Args:
        tile_id: Static tile ID

    Returns:
        Estimated height in Z-units
    """
    if tile_id in COMMON_IMPASSABLE_STATICS:
        return 20  # Most walls/blocking items
    if tile_id in COMMON_SURFACE_STATICS:
        return 0  # Floors are flat
    return 5  # Default small item height


# Test function
def main():
    """Test the tile data reference"""
    import sys

    print("=" * 80)
    print("TILEDATA REFERENCE TEST")
    print("=" * 80)

    # Test hardcoded mode
    print("\n--- Hardcoded Mode ---")
    ref = TileDataReference()

    # Test some land tiles
    test_lands = [0, 1, 220, 168, 500]
    print("\nLand tile tests:")
    for tile_id in test_lands:
        flags = ref.get_land_flags(tile_id)
        impass = ref.is_impassable_land(tile_id)
        water = ref.is_water(tile_id)
        print(f"  Land {tile_id}: flags={flags}, impassable={impass}, water={water}")

    # Test some static tiles
    test_statics = [0x0001, 0x04AB, 0x0C99, 0x0763, 0x1000]
    print("\nStatic tile tests:")
    for tile_id in test_statics:
        flags = ref.get_static_flags(tile_id)
        impass = ref.is_impassable_static(tile_id)
        surface = ref.is_surface(tile_id)
        height = ref.get_static_height(tile_id)
        print(f"  Static 0x{tile_id:04X}: flags={flags}, impassable={impass}, surface={surface}, height={height}")

    # Test with file if provided
    if len(sys.argv) > 1:
        tiledata_path = sys.argv[1]
        print(f"\n--- File Mode ({tiledata_path}) ---")
        try:
            ref_file = TileDataReference(tiledata_path)

            print("\nLand tile tests (from file):")
            for tile_id in test_lands:
                flags = ref_file.get_land_flags(tile_id)
                name = ref_file.get_land_name(tile_id)
                impass = ref_file.is_impassable_land(tile_id)
                print(f"  Land {tile_id} '{name}': flags={flags}, impassable={impass}")

            print("\nStatic tile tests (from file):")
            for tile_id in test_statics:
                flags = ref_file.get_static_flags(tile_id)
                name = ref_file.get_static_name(tile_id)
                height = ref_file.get_static_height(tile_id)
                print(f"  Static 0x{tile_id:04X} '{name}': flags={flags}, height={height}")

            cache_stats = ref_file.get_cache_stats()
            print(f"\nCache stats: {cache_stats}")

        except FileNotFoundError as e:
            print(f"Error: {e}")

    print("\n" + "=" * 80)
    print("TEST COMPLETE")
    print("=" * 80)


if __name__ == "__main__":
    main()
