"""
Analyze wall structure in a specific area to understand how walls are built
"""

import struct
from pathlib import Path
from collections import defaultdict

# Map dimensions
MAP_WIDTH_BLOCKS = 896
MAP_HEIGHT_BLOCKS = 512

def get_block_number(block_x: int, block_y: int) -> int:
    """CentrED's block calculation"""
    return block_x * MAP_HEIGHT_BLOCKS + block_y


def load_statics_in_area(staidx_path: Path, statics_path: Path, min_x: int, min_y: int, max_x: int, max_y: int):
    """
    Load all statics in a specific area.

    Returns:
        Dictionary mapping (x, y) to list of statics at that position
    """
    # Calculate which blocks we need to read
    min_block_x = min_x // 8
    max_block_x = max_x // 8
    min_block_y = min_y // 8
    max_block_y = max_y // 8

    statics_by_pos = defaultdict(list)

    with open(staidx_path, 'rb') as idx, open(statics_path, 'rb') as sta:
        for block_x in range(min_block_x, max_block_x + 1):
            for block_y in range(min_block_y, max_block_y + 1):
                block_id = get_block_number(block_x, block_y)

                idx.seek(block_id * 12)
                offset = struct.unpack('<I', idx.read(4))[0]
                length = struct.unpack('<I', idx.read(4))[0]

                if offset == 0xFFFFFFFF or length == 0:
                    continue

                sta.seek(offset)
                count = length // 7

                for _ in range(count):
                    tile_id = struct.unpack('<H', sta.read(2))[0]
                    local_x = struct.unpack('B', sta.read(1))[0]
                    local_y = struct.unpack('B', sta.read(1))[0]
                    z = struct.unpack('b', sta.read(1))[0]
                    hue = struct.unpack('<H', sta.read(2))[0]

                    # Convert to world coordinates
                    world_x = block_x * 8 + local_x
                    world_y = block_y * 8 + local_y

                    # Only include statics in our target area
                    if min_x <= world_x <= max_x and min_y <= world_y <= max_y:
                        statics_by_pos[(world_x, world_y)].append({
                            'tile_id': tile_id,
                            'z': z,
                            'hue': hue
                        })

    return statics_by_pos


def analyze_wall_structure(min_x: int, min_y: int, max_x: int, max_y: int):
    """Analyze the wall structure in the given area"""

    statics_path = Path(r'C:\DevEnv\GIT\UO\centredsharp\output\editable_maps\statics0.mul')
    staidx_path = Path(r'C:\DevEnv\GIT\UO\centredsharp\output\editable_maps\staidx0.mul')

    print(f"Analyzing area: ({min_x}, {min_y}) to ({max_x}, {max_y})")
    print("="*80)

    statics = load_statics_in_area(staidx_path, statics_path, min_x, min_y, max_x, max_y)

    print(f"Found {len(statics)} positions with statics\n")

    # Analyze each position
    for y in range(min_y, max_y + 1):
        for x in range(min_x, max_x + 1):
            if (x, y) in statics:
                items = statics[(x, y)]
                print(f"\nPosition ({x}, {y}):")
                print(f"  Total items: {len(items)}")

                # Sort by Z-level for easier reading
                items_sorted = sorted(items, key=lambda i: i['z'])

                for item in items_sorted:
                    tile_hex = f"0x{item['tile_id']:04X}"
                    print(f"    Tile: {tile_hex} (dec: {item['tile_id']:5d}), Z: {item['z']:3d}, Hue: {item['hue']}")

    # Summary of tile types used
    print("\n" + "="*80)
    print("TILE SUMMARY:")
    print("="*80)

    tile_usage = defaultdict(lambda: {'count': 0, 'z_levels': set()})

    for pos, items in statics.items():
        for item in items:
            tile_id = item['tile_id']
            tile_usage[tile_id]['count'] += 1
            tile_usage[tile_id]['z_levels'].add(item['z'])

    for tile_id in sorted(tile_usage.keys()):
        info = tile_usage[tile_id]
        tile_hex = f"0x{tile_id:04X}"
        z_levels = sorted(info['z_levels'])
        print(f"Tile {tile_hex}: Used {info['count']} times at Z-levels: {z_levels}")

    # Analyze stacking patterns
    print("\n" + "="*80)
    print("STACKING PATTERNS:")
    print("="*80)

    for pos, items in sorted(statics.items()):
        if len(items) > 1:
            x, y = pos
            items_sorted = sorted(items, key=lambda i: i['z'])
            z_diffs = []
            for i in range(1, len(items_sorted)):
                diff = items_sorted[i]['z'] - items_sorted[i-1]['z']
                z_diffs.append(diff)

            print(f"Position ({x}, {y}): {len(items)} items stacked")
            print(f"  Z-levels: {[i['z'] for i in items_sorted]}")
            print(f"  Z-differences: {z_diffs}")
            tiles_hex = [f"0x{i['tile_id']:04X}" for i in items_sorted]
            print(f"  Tiles: {tiles_hex}")


if __name__ == '__main__':
    # Analyze the specified area
    analyze_wall_structure(1890, 2117, 1909, 2123)
