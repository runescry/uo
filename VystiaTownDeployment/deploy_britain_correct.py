"""
Britain Deployment Script - Using CentrED's Correct Block Calculation
Based on analysis of CentrED source code
"""

import struct
from pathlib import Path

# Map dimensions
MAP_WIDTH_BLOCKS = 896   # 7168 tiles / 8
MAP_HEIGHT_BLOCKS = 512  # 4096 tiles / 8

def get_block_number(block_x: int, block_y: int) -> int:
    """
    CentrED's block calculation: blockNum = blockX * mapHeight + blockY
    NOT blockX + blockY * mapWidth (which we were using!)
    """
    return block_x * MAP_HEIGHT_BLOCKS + block_y

def get_staidx_offset(block_x: int, block_y: int) -> int:
    """Calculate offset in staidx.mul for this block"""
    return get_block_number(block_x, block_y) * 12

print("="*60)
print("Britain Deployment - Using CentrED Block Calculation")
print("="*60)

# Source: OSI Britain
osi_statics = Path(r'C:\Ultima Online\statics0.mul')
osi_staidx = Path(r'C:\Ultima Online\staidx0.mul')

# Target: Vystia map
target_statics = Path(r'C:\Program Files (x86)\Electronic Arts\Ultima Online Classic\statics0.mul')
target_staidx = Path(r'C:\Program Files (x86)\Electronic Arts\Ultima Online Classic\staidx0.mul')

# Britain bounds (OSI): (1400, 1500) to (1750, 1800)
# OSI Britain center approximately: (1575, 1650)
# Target center: (1750, 800)
osi_center_x = 1575
osi_center_y = 1650
target_x = 1750
target_y = 800
shift_x = target_x - osi_center_x  # 1750 - 1575 = 175
shift_y = target_y - osi_center_y  # 800 - 1650 = -850

# New target bounds will be approximately:
# (1400+shift, 1500+shift) to (1750+shift, 1800+shift)
target_x1 = 1400 + shift_x
target_y1 = 1500 + shift_y
target_x2 = 1750 + shift_x
target_y2 = 1800 + shift_y

print(f"Reading OSI Britain statics...")
print(f"Source bounds: (1400, 1500) to (1750, 1800)")
print(f"Target bounds: ({target_x1}, {target_y1}) to ({target_x2}, {target_y2})")
print(f"Target center: ({target_x}, {target_y})")
print(f"Shift: ({shift_x}, {shift_y}) tiles\n")

# Step 1: Read all OSI Britain statics with WORLD coordinates
britain_statics = []

with open(osi_staidx, 'rb') as idx, open(osi_statics, 'rb') as sta:
    for block_y in range(187, 226):  # Britain Y blocks
        for block_x in range(175, 219):  # Britain X blocks
            # EXPERIMENT: Use CentrED formula for reading OSI too
            block_id = get_block_number(block_x, block_y)  # block_x * 512 + block_y

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

# Step 2: Shift to new coordinates AND adjust Z-level
# Keep original OSI Z-levels - no adjustment
# Vystia terrain is now flattened to Z=0, OSI statics avg Z=0.7
Z_ADJUST = 0

for static in britain_statics:
    static['x'] += shift_x
    static['y'] += shift_y
    static['z'] += Z_ADJUST
    # Clamp to signed byte range to prevent overflow
    if static['z'] < -128:
        static['z'] = -128
    elif static['z'] > 127:
        static['z'] = 127

print(f"Shifted statics to target location and adjusted Z by {Z_ADJUST}")

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

# Step 5: Clear Britain area blocks first, then add Britain blocks
print(f"Clearing Britain area blocks...")
# Get all block IDs in Britain area
britain_block_ids = set()
for y in range(1500, 1801):
    for x in range(1400, 1751):
        block_x = x // 8
        block_y = y // 8
        block_id = get_block_number(block_x, block_y)
        britain_block_ids.add(block_id)

# Clear these blocks
for block_id in britain_block_ids:
    if block_id in vystia_blocks:
        del vystia_blocks[block_id]

print(f"Cleared {len(britain_block_ids)} blocks in Britain area")

# Step 6: Add Britain blocks
for block_id, statics in blocks.items():
    vystia_blocks[block_id] = statics

print(f"Merged - total {len(vystia_blocks)} blocks")

# Step 6: Write using CentrED's method (append to end)
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
print(f"Britain deployed to: ({target_x1}, {target_y1}) to ({target_x2}, {target_y2})")
print(f"Center approximately: ({target_x}, {target_y})")
print(f"\nNow copy to CentrED location and restart both CentrED and ServUO")
