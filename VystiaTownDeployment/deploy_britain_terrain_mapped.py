"""
Britain Deployment Script - Using Terrain Height Mapping
Adjusts each static based on local terrain Z difference between OSI and Vystia
"""

import struct
import json
from pathlib import Path

# Map dimensions
MAP_WIDTH_BLOCKS = 896   # 7168 tiles / 8
MAP_HEIGHT_BLOCKS = 512  # 4096 tiles / 8

def get_block_number(block_x: int, block_y: int) -> int:
    """CentrED's block calculation: blockNum = blockX * mapHeight + blockY"""
    return block_x * MAP_HEIGHT_BLOCKS + block_y

print("="*60)
print("Britain Deployment - Using Terrain Height Mapping")
print("="*60)

# Load terrain height map
terrain_map_path = Path('britain_terrain_height_map.json')
print(f"Loading terrain height map...")
with open(terrain_map_path, 'r') as f:
    terrain_data = json.load(f)

# Create lookup dict for quick Z adjustment lookup
# Key: (osi_x, osi_y) -> z_adjustment
z_adjustment_map = {}
for tile in terrain_data['terrain_map']:
    z_adjustment_map[(tile['osi_x'], tile['osi_y'])] = tile['z_adjustment']

print(f"Loaded {len(z_adjustment_map)} terrain mappings")
print(f"Average Z adjustment: {terrain_data['statistics']['adjustment_avg']:+.1f}")

# Source: OSI Britain
osi_statics = Path(r'C:\Ultima Online\statics0.mul')
osi_staidx = Path(r'C:\Ultima Online\staidx0.mul')

# Target: Vystia map
target_statics = Path(r'C:\Program Files (x86)\Electronic Arts\Ultima Online Classic\statics0.mul')
target_staidx = Path(r'C:\Program Files (x86)\Electronic Arts\Ultima Online Classic\staidx0.mul')

# Britain bounds (OSI)
shift_x = terrain_data['shift']['x']
shift_y = terrain_data['shift']['y']

print(f"\nReading OSI Britain statics...")
print(f"Shift: ({shift_x}, {shift_y}) tiles\n")

# Step 1: Read all OSI Britain statics with WORLD coordinates
britain_statics = []

with open(osi_staidx, 'rb') as idx, open(osi_statics, 'rb') as sta:
    for block_y in range(187, 226):  # Britain Y blocks
        for block_x in range(175, 219):  # Britain X blocks
            # Use OSI's block calculation
            block_id = block_x + (block_y * 896)

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

                britain_statics.append({
                    'tile_id': tile_id,
                    'x': world_x,
                    'y': world_y,
                    'z': z,
                    'hue': hue
                })

print(f"Read {len(britain_statics)} statics from OSI Britain")

# Step 2: Shift to new coordinates AND adjust Z based on terrain mapping
print(f"Shifting statics and adjusting Z based on terrain mapping...")

adjusted_count = 0
for static in britain_statics:
    osi_x = static['x']
    osi_y = static['y']

    # Shift coordinates
    static['x'] += shift_x
    static['y'] += shift_y

    # Adjust Z based on local terrain difference
    if (osi_x, osi_y) in z_adjustment_map:
        z_adjustment = z_adjustment_map[(osi_x, osi_y)]
        static['z'] += z_adjustment
        adjusted_count += 1

        # Clamp to signed byte range
        if static['z'] < -128:
            static['z'] = -128
        elif static['z'] > 127:
            static['z'] = 127

print(f"Adjusted {adjusted_count}/{len(britain_statics)} statics based on terrain mapping")

# Step 3: Group into blocks with CORRECT block calculation
blocks = {}
for static in britain_statics:
    # Calculate LOCAL coordinates using bitwise AND
    local_x = static['x'] & 0x7  # Same as % 8
    local_y = static['y'] & 0x7

    # Calculate block coordinates
    block_x = static['x'] // 8
    block_y = static['y'] // 8

    # Use CentrED's block calculation!
    block_id = get_block_number(block_x, block_y)

    if block_id not in blocks:
        blocks[block_id] = []

    blocks[block_id].append({
        'tile_id': static['tile_id'],
        'x': local_x,
        'y': local_y,
        'z': static['z'],
        'hue': static['hue']
    })

print(f"Grouped into {len(blocks)} blocks")

# Step 4: Load existing Vystia blocks
print(f"Loading existing Vystia map...")
vystia_blocks = {}

with open(target_staidx, 'rb') as idx, open(target_statics, 'rb') as sta:
    for i in range(MAP_WIDTH_BLOCKS * MAP_HEIGHT_BLOCKS):
        idx.seek(i * 12)
        offset = struct.unpack('<I', idx.read(4))[0]
        length = struct.unpack('<I', idx.read(4))[0]

        if offset == 0xFFFFFFFF or length == 0:
            continue

        sta.seek(offset)
        count = length // 7

        vystia_blocks[i] = []
        for _ in range(count):
            tile_id = struct.unpack('<H', sta.read(2))[0]
            x = struct.unpack('B', sta.read(1))[0]
            y = struct.unpack('B', sta.read(1))[0]
            z = struct.unpack('b', sta.read(1))[0]
            hue = struct.unpack('<H', sta.read(2))[0]
            vystia_blocks[i].append({'tile_id': tile_id, 'x': x, 'y': y, 'z': z, 'hue': hue})

print(f"Loaded {len(vystia_blocks)} existing Vystia blocks")

# Step 5: Merge - REPLACE Vystia blocks with Britain blocks (overwrite)
for block_id, statics in blocks.items():
    vystia_blocks[block_id] = statics

print(f"Merged - total {len(vystia_blocks)} blocks")

# Step 6: Write using CentrED's method
print(f"Writing to files...")

with open(target_statics, 'wb') as sta, open(target_staidx, 'wb') as idx:
    current_offset = 0

    for block_id in range(MAP_WIDTH_BLOCKS * MAP_HEIGHT_BLOCKS):
        if block_id in vystia_blocks and len(vystia_blocks[block_id]) > 0:
            statics = vystia_blocks[block_id]

            # Write statics
            for s in statics:
                sta.write(struct.pack('<H', s['tile_id']))
                sta.write(struct.pack('B', s['x']))
                sta.write(struct.pack('B', s['y']))
                sta.write(struct.pack('b', s['z']))
                sta.write(struct.pack('<H', s['hue']))

            # Write index
            length = len(statics) * 7
            idx.write(struct.pack('<I', current_offset))
            idx.write(struct.pack('<I', length))
            idx.write(struct.pack('<I', 0))

            current_offset += length
        else:
            # Empty block
            idx.write(struct.pack('<I', 0xFFFFFFFF))
            idx.write(struct.pack('<I', 0xFFFFFFFF))
            idx.write(struct.pack('<I', 0))

        if block_id % 50000 == 0 and block_id > 0:
            print(f"  Progress: {block_id}/{MAP_WIDTH_BLOCKS * MAP_HEIGHT_BLOCKS}...")

print("\n" + "="*60)
print("DEPLOYMENT COMPLETE!")
print("="*60)
print(f"Britain deployed with terrain-mapped Z-levels")
print(f"Center approximately: (1871, 700)")
print(f"\nNow copy to CentrED location and restart both CentrED and ServUO")
