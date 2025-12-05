"""
Extract Magincia from T2A Map and Deploy to Current Server
Extracts the original non-destroyed Magincia and deploys it to the current map.
"""

import struct
from pathlib import Path
import datetime

# T2A Map dimensions (Pre-ML: 768x512 blocks)
T2A_MAP_WIDTH_BLOCKS = 768   # 6144 tiles / 8
T2A_MAP_HEIGHT_BLOCKS = 512  # 4096 tiles / 8

# Current Server Map dimensions (ML: 896x512 blocks)
SERVER_MAP_WIDTH_BLOCKS = 896  # 7168 tiles / 8
SERVER_MAP_HEIGHT_BLOCKS = 512  # 4096 tiles / 8

def get_block_number_t2a(block_x: int, block_y: int) -> int:
    """T2A block calculation"""
    return block_x * T2A_MAP_HEIGHT_BLOCKS + block_y

def get_block_number_server(block_x: int, block_y: int) -> int:
    """Server map block calculation"""
    return block_x * SERVER_MAP_HEIGHT_BLOCKS + block_y

print("="*80)
print("MAGINCIA EXTRACTION AND DEPLOYMENT")
print("="*80)

# Source: T2A Magincia (from D:\client)
t2a_statics = Path(r'D:\client\statics0.mul')
t2a_staidx = Path(r'D:\client\staidx0.mul')

# Target: Current server map (find the actual server map location)
# You'll need to specify where your ServUO reads the map from
target_statics = Path(r'C:\Program Files (x86)\Electronic Arts\Ultima Online Classic\statics0.mul')
target_staidx = Path(r'C:\Program Files (x86)\Electronic Arts\Ultima Online Classic\staidx0.mul')

# Magincia bounds (approximate - from OSI coords)
# Old Magincia is an island at roughly (3650, 2050) to (3850, 2350)
# These are approximate - you may need to adjust
magincia_x1 = 3600
magincia_y1 = 2000
magincia_x2 = 3900
magincia_y2 = 2400

print(f"Source: T2A map")
print(f"  Statics: {t2a_statics}")
print(f"  StaIdx: {t2a_staidx}")
print(f"\nTarget: Server map")
print(f"  Statics: {target_statics}")
print(f"  StaIdx: {target_staidx}")
print(f"\nMagincia bounds: ({magincia_x1}, {magincia_y1}) to ({magincia_x2}, {magincia_y2})")

if not t2a_statics.exists() or not t2a_staidx.exists():
    print(f"\nERROR: T2A map files not found!")
    exit(1)

if not target_statics.exists() or not target_staidx.exists():
    print(f"\nERROR: Target map files not found!")
    print(f"Please specify the correct path to your ServUO map files.")
    exit(1)

# Calculate block range
block_x1 = magincia_x1 // 8
block_y1 = magincia_y1 // 8
block_x2 = magincia_x2 // 8
block_y2 = magincia_y2 // 8

print(f"\nBlock range: ({block_x1}, {block_y1}) to ({block_x2}, {block_y2})")

# Step 1: Read all Magincia statics from T2A
print(f"\nReading Magincia statics from T2A map...")
magincia_statics = []

with open(t2a_staidx, 'rb') as idx, open(t2a_statics, 'rb') as sta:
    for block_y in range(block_y1, block_y2 + 1):
        for block_x in range(block_x1, block_x2 + 1):
            block_id = get_block_number_t2a(block_x, block_y)

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

                # Only include statics within the exact bounds
                if magincia_x1 <= world_x <= magincia_x2 and magincia_y1 <= world_y <= magincia_y2:
                    magincia_statics.append({
                        'tile_id': tile_id,
                        'x': world_x,
                        'y': world_y,
                        'z': z,
                        'hue': hue
                    })

print(f"Extracted {len(magincia_statics)} statics from T2A Magincia")

# Step 2: Export to JSON for inspection
import json
output_dir = Path("C:/DevEnv/GIT/UO/Vystia Town Generator/extracted_magincia")
output_dir.mkdir(parents=True, exist_ok=True)

export_data = {
    'source': 'T2A Original Map',
    'bounds': {
        'x1': magincia_x1,
        'y1': magincia_y1,
        'x2': magincia_x2,
        'y2': magincia_y2
    },
    'total_statics': len(magincia_statics),
    'statics': magincia_statics
}

timestamp = datetime.datetime.now().strftime("%Y%m%d_%H%M%S")
export_file = output_dir / f"magincia_t2a_{timestamp}.json"

with open(export_file, 'w') as f:
    json.dump(export_data, f, indent=2)

print(f"\nExported Magincia data to: {export_file}")
print("="*80)
print("EXTRACTION COMPLETE")
print("="*80)
print("\nNext steps:")
print("1. Review the exported JSON file to verify the extraction")
print("2. Use a deployment script to write these statics to your server map")
print("3. Make sure to backup your server map files first!")
