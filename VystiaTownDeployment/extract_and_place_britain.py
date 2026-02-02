"""
Extract Britain from OSI map0.mul and place it directly at new location
"""

import struct
import json
import random
from pathlib import Path
from collections import defaultdict

# Map dimensions
MAP_WIDTH_BLOCKS = 896   # 7168 tiles / 8
MAP_HEIGHT_BLOCKS = 512  # 4096 tiles / 8

# Water tile IDs (to replace with grass)
WATER_TILES = set(range(168, 172)) | {100}  # 168-171 ocean + 100 shallow water

# Grass tile IDs (to replace water with)
GRASS_TILES = [3, 4, 5, 6]

# Vystia base grass Z level
VYSTIA_GRASS_Z = 0

def get_block_number(block_x: int, block_y: int) -> int:
    """CentrED's block calculation: blockNum = blockX * mapHeight + blockY"""
    return block_x * MAP_HEIGHT_BLOCKS + block_y

# Source: OSI map files
OSI_MAP = Path(r'C:\Program Files (x86)\Electronic Arts\Ultima Online Classic\map0.mul')
OSI_STATICS = Path(r'C:\Program Files (x86)\Electronic Arts\Ultima Online Classic\statics0.mul')
OSI_STAIDX = Path(r'C:\Program Files (x86)\Electronic Arts\Ultima Online Classic\staidx0.mul')

# Target: TerrainTest map files
TARGET_MAP = Path(r'D:\UO\centredsharp\Output-TerrainTest\editable_maps\map6.mul')
TARGET_STATICS = Path(r'D:\UO\centredsharp\Output-TerrainTest\editable_maps\statics6.mul')
TARGET_STAIDX = Path(r'D:\UO\centredsharp\Output-TerrainTest\editable_maps\staidx6.mul')

# Britain bounds on OSI map0
SOURCE_X1, SOURCE_Y1 = 1270, 1400
SOURCE_X2, SOURCE_Y2 = 1750, 1800

# Target location
TARGET_CENTER_X, TARGET_CENTER_Y = 4000, 1600

# Parse command line arguments
import argparse
parser = argparse.ArgumentParser(description='Extract and place Britain from OSI map0')
parser.add_argument('--statics-only', action='store_true',
                    help='Only place statics (buildings), preserve existing terrain')
parser.add_argument('--center-x', type=int, default=TARGET_CENTER_X,
                    help=f'Target center X coordinate (default: {TARGET_CENTER_X})')
parser.add_argument('--center-y', type=int, default=TARGET_CENTER_Y,
                    help=f'Target center Y coordinate (default: {TARGET_CENTER_Y})')
args = parser.parse_args()

TARGET_CENTER_X = args.center_x
TARGET_CENTER_Y = args.center_y
STATICS_ONLY = args.statics_only

print("="*80)
print("EXTRACT AND PLACE BRITAIN FROM OSI MAP0")
if STATICS_ONLY:
    print("MODE: STATICS ONLY (preserving existing terrain)")
print("="*80)
print(f"Source (OSI map0): ({SOURCE_X1}, {SOURCE_Y1}) to ({SOURCE_X2}, {SOURCE_Y2})")
print(f"Target center: ({TARGET_CENTER_X}, {TARGET_CENTER_Y})")

# Calculate source center for relative coordinates
source_center_x = (SOURCE_X1 + SOURCE_X2) // 2
source_center_y = (SOURCE_Y1 + SOURCE_Y2) // 2

print(f"Source center: ({source_center_x}, {source_center_y})")

# Calculate coordinate shift
shift_x = TARGET_CENTER_X - source_center_x
shift_y = TARGET_CENTER_Y - source_center_y

print(f"Coordinate shift: ({shift_x}, {shift_y})")

# Step 1: Extract and place terrain (skip if statics-only)
if not STATICS_ONLY:
    print("\n[1/2] Extracting terrain from OSI map0...")
    terrain_blocks = defaultdict(dict)

    with open(OSI_MAP, 'rb') as osi_map:
        for y in range(SOURCE_Y1, SOURCE_Y2 + 1):
            for x in range(SOURCE_X1, SOURCE_X2 + 1):
                block_x = x // 8
                block_y = y // 8
                cell_x = x % 8
                cell_y = y % 8

                block_id = get_block_number(block_x, block_y)
                block_offset = block_id * 196
                cell_offset = block_offset + 4 + (cell_y * 8 + cell_x) * 3

                try:
                    osi_map.seek(cell_offset)
                    tile_id = struct.unpack('<H', osi_map.read(2))[0]
                    z = struct.unpack('b', osi_map.read(1))[0]

                    # Replace water tiles with grass (keep original Z level)
                    if tile_id in WATER_TILES:
                        tile_id = random.choice(GRASS_TILES)
                        # Keep original Z level (no adjustment)
                    # Keep original Z level for all terrain (no adjustments)

                    # Calculate target coordinates
                    target_x = x + shift_x
                    target_y = y + shift_y

                    # Group by target block
                    target_block_x = target_x // 8
                    target_block_y = target_y // 8
                    target_block_id = get_block_number(target_block_x, target_block_y)
                    target_cell_x = target_x % 8
                    target_cell_y = target_y % 8
                    target_cell_index = target_cell_y * 8 + target_cell_x

                    terrain_blocks[target_block_id][target_cell_index] = (tile_id, z)
                except (struct.error, IOError) as e:
                    print(f"Warning: Could not read terrain at ({x}, {y}): {e}")

    total_terrain = sum(len(cells) for cells in terrain_blocks.values())
    water_replaced = 0
    for cells in terrain_blocks.values():
        for tile_id, z in cells.values():
            if tile_id in GRASS_TILES:
                # Count how many were originally water (now grass)
                water_replaced += 1

    print(f"  Extracted {total_terrain} terrain tiles")
    print(f"  Replaced water tiles with grass: {water_replaced} tiles")

    # Write terrain to target map
    print("  Writing terrain to target map...")
    with open(TARGET_MAP, 'r+b') as target_map:
        for block_id, cells in terrain_blocks.items():
            block_offset = block_id * 196
            target_map.seek(block_offset)
            
            # Read existing block
            header = target_map.read(4)
            if len(header) < 4:
                header = b'\x00\x00\x00\x00'
            
            existing_tiles = []
            for _ in range(64):
                tile_data = target_map.read(3)
                if len(tile_data) < 3:
                    existing_tiles.append((0x0001, 0))
                else:
                    tile_id = struct.unpack('<H', tile_data[0:2])[0]
                    z = struct.unpack('b', tile_data[2:3])[0]
                    existing_tiles.append((tile_id, z))
            
            # Update tiles
            for cell_index, (tile_id, z) in cells.items():
                existing_tiles[cell_index] = (tile_id, z)
            
            # Write back
            target_map.seek(block_offset)
            target_map.write(header)
            for tile_id, z in existing_tiles:
                target_map.write(struct.pack('<H', tile_id))
                target_map.write(struct.pack('b', z))

    print(f"  Wrote {len(terrain_blocks)} terrain blocks")
else:
    print("\n[SKIP] Terrain extraction (statics-only mode)")

# Step 2: Extract and place statics
if STATICS_ONLY:
    print("\n[1/1] Extracting statics from OSI map0...")
else:
    print("\n[2/2] Extracting statics from OSI map0...")
static_items = []

# Calculate source block range
source_block_x1 = SOURCE_X1 // 8
source_block_y1 = SOURCE_Y1 // 8
source_block_x2 = SOURCE_X2 // 8
source_block_y2 = SOURCE_Y2 // 8

with open(OSI_STAIDX, 'rb') as osi_idx, open(OSI_STATICS, 'rb') as osi_sta:
    for block_y in range(source_block_y1, source_block_y2 + 1):
        for block_x in range(source_block_x1, source_block_x2 + 1):
            block_id = get_block_number(block_x, block_y)

            osi_idx.seek(block_id * 12)
            offset = struct.unpack('<I', osi_idx.read(4))[0]
            length = struct.unpack('<I', osi_idx.read(4))[0]

            if offset == 0xFFFFFFFF or length == 0:
                continue

            osi_sta.seek(offset)
            count = length // 7

            for _ in range(count):
                tile_id = struct.unpack('<H', osi_sta.read(2))[0]
                local_x = struct.unpack('B', osi_sta.read(1))[0]
                local_y = struct.unpack('B', osi_sta.read(1))[0]
                z = struct.unpack('b', osi_sta.read(1))[0]
                hue = struct.unpack('<H', osi_sta.read(2))[0]

                # Calculate source world coordinates
                source_world_x = block_x * 8 + local_x
                source_world_y = block_y * 8 + local_y

                # Only include statics within exact bounds
                if SOURCE_X1 <= source_world_x <= SOURCE_X2 and SOURCE_Y1 <= source_world_y <= SOURCE_Y2:
                    # Calculate target coordinates
                    target_world_x = source_world_x + shift_x
                    target_world_y = source_world_y + shift_y

                    # Keep original Z level (no adjustments)
                    # Clamp to valid range only
                    adjusted_z = z
                    if adjusted_z < -128:
                        adjusted_z = -128
                    elif adjusted_z > 127:
                        adjusted_z = 127

                    static_items.append({
                        'tile_id': tile_id,
                        'x': target_world_x,
                        'y': target_world_y,
                        'z': adjusted_z,
                        'hue': hue
                    })

print(f"  Extracted {len(static_items)} static tiles")

# Group statics by target block
target_blocks = defaultdict(list)
for item in static_items:
    block_x = item['x'] // 8
    block_y = item['y'] // 8
    block_id = get_block_number(block_x, block_y)
    
    local_x = item['x'] % 8
    local_y = item['y'] % 8
    
    target_blocks[block_id].append({
        'tile_id': item['tile_id'],
        'x': local_x,
        'y': local_y,
        'z': item['z'],
        'hue': item['hue']
    })

# Calculate target bounds to clear existing statics
target_x1 = TARGET_CENTER_X - (source_center_x - SOURCE_X1)
target_y1 = TARGET_CENTER_Y - (source_center_y - SOURCE_Y1)
target_x2 = TARGET_CENTER_X + (SOURCE_X2 - source_center_x)
target_y2 = TARGET_CENTER_Y + (SOURCE_Y2 - source_center_y)

target_block_x1 = target_x1 // 8
target_block_y1 = target_y1 // 8
target_block_x2 = target_x2 // 8
target_block_y2 = target_y2 // 8

print(f"  Clearing existing statics in target area: ({target_x1}, {target_y1}) to ({target_x2}, {target_y2})")

# Read existing statics from target and filter out those in Britain area
print("  Reading existing statics...")
existing_statics = {}

# First, read all statics data
with open(TARGET_STAIDX, 'rb') as target_idx, open(TARGET_STATICS, 'rb') as target_sta:
    for block_id in range(MAP_WIDTH_BLOCKS * MAP_HEIGHT_BLOCKS):
        target_idx.seek(block_id * 12)
        offset = struct.unpack('<I', target_idx.read(4))[0]
        length = struct.unpack('<I', target_idx.read(4))[0]
        
        if offset != 0xFFFFFFFF and length > 0:
            target_sta.seek(offset)
            block_data = target_sta.read(length)
            
            # Check if this block is in the Britain placement area
            block_x = block_id // MAP_HEIGHT_BLOCKS
            block_y = block_id % MAP_HEIGHT_BLOCKS
            
            if target_block_x1 <= block_x <= target_block_x2 and target_block_y1 <= block_y <= target_block_y2:
                # This block is in the Britain area - filter out statics within bounds
                filtered_data = b''
                count = length // 7
                data_pos = 0
                for _ in range(count):
                    if data_pos + 7 > len(block_data):
                        break
                    
                    static_data = block_data[data_pos:data_pos+7]
                    data_pos += 7
                    
                    tile_id = struct.unpack('<H', static_data[0:2])[0]
                    local_x = struct.unpack('B', static_data[2:3])[0]
                    local_y = struct.unpack('B', static_data[3:4])[0]
                    z = struct.unpack('b', static_data[4:5])[0]
                    hue = struct.unpack('<H', static_data[5:7])[0]
                    
                    # Calculate world coordinates
                    world_x = block_x * 8 + local_x
                    world_y = block_y * 8 + local_y
                    
                    # Keep static only if outside Britain bounds
                    if not (target_x1 <= world_x <= target_x2 and target_y1 <= world_y <= target_y2):
                        filtered_data += static_data
                
                if len(filtered_data) > 0:
                    existing_statics[block_id] = filtered_data
                else:
                    existing_statics[block_id] = b''
            else:
                # Block is outside Britain area - keep all statics
                existing_statics[block_id] = block_data
        else:
            existing_statics[block_id] = b''

# Add new Britain statics
print("  Adding Britain statics...")
for block_id, new_items in target_blocks.items():
    new_data = b''
    for item in new_items:
        new_data += struct.pack('<H', item['tile_id'])
        new_data += struct.pack('B', item['x'])
        new_data += struct.pack('B', item['y'])
        new_data += struct.pack('b', item['z'])
        new_data += struct.pack('<H', item['hue'])
    
    # Append to existing (which has been filtered to exclude Britain area)
    if block_id in existing_statics:
        existing_statics[block_id] += new_data
    else:
        existing_statics[block_id] = new_data

# Write updated statics
current_offset = 0
with open(TARGET_STATICS, 'wb') as target_sta, open(TARGET_STAIDX, 'wb') as target_idx:
    for block_id in range(MAP_WIDTH_BLOCKS * MAP_HEIGHT_BLOCKS):
        if block_id in existing_statics and len(existing_statics[block_id]) > 0:
            data = existing_statics[block_id]
            target_sta.write(data)
            
            target_idx.write(struct.pack('<I', current_offset))
            target_idx.write(struct.pack('<I', len(data)))
            target_idx.write(struct.pack('<I', 0))
            
            current_offset += len(data)
        else:
            target_idx.write(struct.pack('<I', 0xFFFFFFFF))
            target_idx.write(struct.pack('<I', 0xFFFFFFFF))
            target_idx.write(struct.pack('<I', 0))

print(f"  Wrote {len(static_items)} static tiles")

print("\n" + "="*80)
print("BRITAIN PLACED SUCCESSFULLY")
print("="*80)
print(f"Source: OSI map0 ({SOURCE_X1}, {SOURCE_Y1}) to ({SOURCE_X2}, {SOURCE_Y2})")
print(f"Target: Center ({TARGET_CENTER_X}, {TARGET_CENTER_Y})")
print(f"Approximate bounds: ({TARGET_CENTER_X - (source_center_x - SOURCE_X1)}, {TARGET_CENTER_Y - (source_center_y - SOURCE_Y1)}) to ({TARGET_CENTER_X + (SOURCE_X2 - source_center_x)}, {TARGET_CENTER_Y + (SOURCE_Y2 - source_center_y)})")
print("\nNote: Restart CentrEDSharp to see changes")
