"""
Extract Britain from current map location and create a blueprint
Extracts from specified bounds and converts to relative coordinates
"""

import struct
import json
from pathlib import Path
from collections import defaultdict

# Map dimensions
MAP_WIDTH_BLOCKS = 896   # 7168 tiles / 8
MAP_HEIGHT_BLOCKS = 512  # 4096 tiles / 8

def get_block_number(block_x: int, block_y: int) -> int:
    """CentrED's block calculation: blockNum = blockX * mapHeight + blockY"""
    return block_x * MAP_HEIGHT_BLOCKS + block_y

# Britain bounds (current location on map)
BRITAIN_X1, BRITAIN_Y1 = 1084, 1485
BRITAIN_X2, BRITAIN_Y2 = 1528, 1963

print("="*80)
print("EXTRACTING BRITAIN FROM CURRENT MAP")
print("="*80)
print(f"Source bounds: ({BRITAIN_X1}, {BRITAIN_Y1}) to ({BRITAIN_X2}, {BRITAIN_Y2})")

# Map files
map_path = Path(r'D:\UO\centredsharp\Output-TerrainTest\editable_maps\map6.mul')
statics_path = Path(r'D:\UO\centredsharp\Output-TerrainTest\editable_maps\statics6.mul')
staidx_path = Path(r'D:\UO\centredsharp\Output-TerrainTest\editable_maps\staidx6.mul')

if not map_path.exists():
    print(f"ERROR: Map file not found: {map_path}")
    exit(1)

# Calculate center point for the blueprint
center_x = (BRITAIN_X1 + BRITAIN_X2) // 2
center_y = (BRITAIN_Y1 + BRITAIN_Y2) // 2

print(f"Center point: ({center_x}, {center_y})")

# Step 1: Extract terrain tiles
print("\n[1/2] Extracting terrain tiles...")
terrain_items = []
terrain_count = 0

with open(map_path, 'rb') as map_file:
    for y in range(BRITAIN_Y1, BRITAIN_Y2 + 1):
        for x in range(BRITAIN_X1, BRITAIN_X2 + 1):
            block_x = x // 8
            block_y = y // 8
            cell_x = x % 8
            cell_y = y % 8

            block_id = get_block_number(block_x, block_y)
            block_offset = block_id * 196
            cell_offset = block_offset + 4 + (cell_y * 8 + cell_x) * 3

            try:
                map_file.seek(cell_offset)
                tile_id = struct.unpack('<H', map_file.read(2))[0]
                z = struct.unpack('b', map_file.read(1))[0]

                # Convert to relative coordinates
                rel_x = x - center_x
                rel_y = y - center_y

                terrain_items.append({
                    'tile_id': tile_id,
                    'x': rel_x,
                    'y': rel_y,
                    'z': z
                })
                terrain_count += 1
            except (struct.error, IOError) as e:
                print(f"Warning: Could not read terrain at ({x}, {y}): {e}")

print(f"  Extracted {terrain_count} terrain tiles")

# Step 2: Extract static tiles
print("\n[2/2] Extracting static tiles...")
static_items = []
static_count = 0

# Calculate block range
block_x1 = BRITAIN_X1 // 8
block_y1 = BRITAIN_Y1 // 8
block_x2 = BRITAIN_X2 // 8
block_y2 = BRITAIN_Y2 // 8

with open(staidx_path, 'rb') as idx, open(statics_path, 'rb') as sta:
    for block_y in range(block_y1, block_y2 + 1):
        for block_x in range(block_x1, block_x2 + 1):
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

                # Calculate world coordinates
                world_x = block_x * 8 + local_x
                world_y = block_y * 8 + local_y

                # Only include statics within exact bounds
                if BRITAIN_X1 <= world_x <= BRITAIN_X2 and BRITAIN_Y1 <= world_y <= BRITAIN_Y2:
                    # Convert to relative coordinates
                    rel_x = world_x - center_x
                    rel_y = world_y - center_y

                    static_items.append({
                        'tile_id': tile_id,
                        'x': rel_x,
                        'y': rel_y,
                        'z': z,
                        'hue': hue
                    })
                    static_count += 1

print(f"  Extracted {static_count} static tiles")

# Combine all items (terrain + statics)
blueprint_items = terrain_items + static_items

# Export as JSON blueprint
script_dir = Path(__file__).parent
output_dir = script_dir / "britain_blueprint"
output_dir.mkdir(parents=True, exist_ok=True)

blueprint_data = {
    'name': 'Britain (Extracted from Map)',
    'description': f'Britain extracted from map at bounds ({BRITAIN_X1}, {BRITAIN_Y1}) to ({BRITAIN_X2}, {BRITAIN_Y2})',
    'center': {'x': center_x, 'y': center_y},
    'bounds': {
        'x1': BRITAIN_X1,
        'y1': BRITAIN_Y1,
        'x2': BRITAIN_X2,
        'y2': BRITAIN_Y2
    },
    'total_items': len(blueprint_items),
    'terrain_count': len(terrain_items),
    'statics_count': len(static_items),
    'items': blueprint_items
}

blueprint_file = output_dir / "britain_extracted.json"
with open(blueprint_file, 'w') as f:
    json.dump(blueprint_data, f, indent=2)

print(f"\n[OK] Blueprint saved to: {blueprint_file}")
print(f"[OK] Total items: {len(blueprint_items)}")
print(f"     - Terrain: {len(terrain_items)}")
print(f"     - Statics: {len(static_items)}")
print(f"\nCenter: ({center_x}, {center_y})")
print(f"Size: {BRITAIN_X2 - BRITAIN_X1 + 1} x {BRITAIN_Y2 - BRITAIN_Y1 + 1} tiles")
