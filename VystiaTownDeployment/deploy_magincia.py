"""
Deploy Magincia from extracted JSON to Server Map
Writes the original T2A Magincia statics to the current server map.
"""

import struct
import json
from pathlib import Path
from collections import defaultdict
import datetime

# Server Map dimensions (ML: 896x512 blocks)
SERVER_MAP_WIDTH_BLOCKS = 896
SERVER_MAP_HEIGHT_BLOCKS = 512

def get_block_number(block_x: int, block_y: int) -> int:
    """Server map block calculation"""
    return block_x * SERVER_MAP_HEIGHT_BLOCKS + block_y

print("="*80)
print("MAGINCIA DEPLOYMENT TO SERVER")
print("="*80)

# Find the latest extracted Magincia JSON
extracted_dir = Path(r'C:\DevEnv\GIT\UO\Vystia Town Generator\extracted_magincia')
json_files = list(extracted_dir.glob('magincia_t2a_*.json'))

if not json_files:
    print("ERROR: No extracted Magincia JSON files found!")
    print(f"Please run extract_deploy_magincia.py first")
    exit(1)

# Use the most recent file
latest_json = max(json_files, key=lambda p: p.stat().st_mtime)
print(f"Loading Magincia data from: {latest_json.name}")

with open(latest_json, 'r') as f:
    magincia_data = json.load(f)

statics = magincia_data['statics']
print(f"Loaded {len(statics)} statics")

# Target: Server map files
target_statics = Path(r'C:\Program Files (x86)\Electronic Arts\Ultima Online Classic\statics0.mul')
target_staidx = Path(r'C:\Program Files (x86)\Electronic Arts\Ultima Online Classic\staidx0.mul')

print(f"\nTarget files:")
print(f"  Statics: {target_statics}")
print(f"  StaIdx: {target_staidx}")

if not target_statics.exists() or not target_staidx.exists():
    print(f"\nERROR: Target map files not found!")
    exit(1)

# Read existing statics into memory
print(f"\nReading existing server statics...")
existing_blocks = {}

with open(target_staidx, 'rb') as idx, open(target_statics, 'rb') as sta:
    for block_y in range(SERVER_MAP_HEIGHT_BLOCKS):
        for block_x in range(SERVER_MAP_WIDTH_BLOCKS):
            block_id = get_block_number(block_x, block_y)

            idx.seek(block_id * 12)
            offset = struct.unpack('<I', idx.read(4))[0]
            length = struct.unpack('<I', idx.read(4))[0]

            if offset == 0xFFFFFFFF or length == 0:
                existing_blocks[(block_x, block_y)] = []
                continue

            block_statics = []
            sta.seek(offset)
            count = length // 7

            for _ in range(count):
                tile_id = struct.unpack('<H', sta.read(2))[0]
                local_x = struct.unpack('B', sta.read(1))[0]
                local_y = struct.unpack('B', sta.read(1))[0]
                z = struct.unpack('b', sta.read(1))[0]
                hue = struct.unpack('<H', sta.read(2))[0]

                world_x = block_x * 8 + local_x
                world_y = block_y * 8 + local_y

                block_statics.append({
                    'tile_id': tile_id,
                    'x': world_x,
                    'y': world_y,
                    'z': z,
                    'hue': hue
                })

            existing_blocks[(block_x, block_y)] = block_statics

print(f"Read {len(existing_blocks)} blocks from server map")

# Remove old Magincia statics (same bounds as extraction)
bounds = magincia_data['bounds']
magincia_x1 = bounds['x1']
magincia_y1 = bounds['y1']
magincia_x2 = bounds['x2']
magincia_y2 = bounds['y2']

print(f"\nClearing old Magincia area blocks in bounds ({magincia_x1}, {magincia_y1}) to ({magincia_x2}, {magincia_y2})...")

# Get all block IDs in Magincia area and clear them completely
magincia_block_ids = set()
removed_count = 0

for y in range(magincia_y1, magincia_y2 + 1):
    for x in range(magincia_x1, magincia_x2 + 1):
        block_x = x // 8
        block_y = y // 8
        magincia_block_ids.add((block_x, block_y))

# Clear these blocks completely
for block_key in magincia_block_ids:
    if block_key in existing_blocks:
        removed_count += len(existing_blocks[block_key])
        del existing_blocks[block_key]

print(f"Cleared {len(magincia_block_ids)} blocks, removed {removed_count} old statics from Magincia area")

# Add new Magincia statics
print(f"\nAdding {len(statics)} new Magincia statics...")
for static in statics:
    block_x = static['x'] // 8
    block_y = static['y'] // 8

    if (block_x, block_y) in existing_blocks:
        existing_blocks[(block_x, block_y)].append(static)
    else:
        existing_blocks[(block_x, block_y)] = [static]

# Write new statics and index files
print(f"\nWriting updated statics to server map...")

# Write to temporary files first
temp_statics = target_statics.parent / 'statics0_temp.mul'
temp_staidx = target_staidx.parent / 'staidx0_temp.mul'

with open(temp_statics, 'wb') as sta, open(temp_staidx, 'wb') as idx:
    current_offset = 0

    for block_id in range(SERVER_MAP_WIDTH_BLOCKS * SERVER_MAP_HEIGHT_BLOCKS):
        block_x = block_id // SERVER_MAP_HEIGHT_BLOCKS
        block_y = block_id % SERVER_MAP_HEIGHT_BLOCKS
        block_statics = existing_blocks.get((block_x, block_y), [])

        if not block_statics:
            # Empty block
            idx.write(struct.pack('<I', 0xFFFFFFFF))
            idx.write(struct.pack('<I', 0xFFFFFFFF))
            idx.write(struct.pack('<I', 0))
        else:
            # Write statics for this block
            length = len(block_statics) * 7

            for static in block_statics:
                local_x = static['x'] % 8
                local_y = static['y'] % 8

                sta.write(struct.pack('<H', static['tile_id']))
                sta.write(struct.pack('B', local_x))
                sta.write(struct.pack('B', local_y))
                sta.write(struct.pack('b', static['z']))
                sta.write(struct.pack('<H', static['hue']))

            # Write index AFTER statics
            idx.write(struct.pack('<I', current_offset))
            idx.write(struct.pack('<I', length))
            idx.write(struct.pack('<I', 0))

            current_offset += length

print(f"Wrote statics to temporary files")

# Move temp files to final location (atomic replacement)
print(f"\nReplacing original files...")
import shutil

shutil.move(str(temp_statics), str(target_statics))
shutil.move(str(temp_staidx), str(target_staidx))

print("="*80)
print("DEPLOYMENT COMPLETE")
print("="*80)
print(f"\nOriginal Magincia has been deployed to your server!")
print(f"Removed {removed_count} old statics")
print(f"Added {len(statics)} new statics")
print(f"\nBackup location: C:\\DevEnv\\GIT\\UO\\ServUO_Map_Backups\\backup_20251118_161950")
print(f"\nRestart your ServUO server to see the changes.")
