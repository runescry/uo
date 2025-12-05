"""
Extract Magincia from T2A map as a CentrED blueprint
Similar to the Britain extraction we did
"""

import struct
import json
from pathlib import Path
from collections import defaultdict

# T2A Map dimensions
T2A_MAP_WIDTH_BLOCKS = 768   # 6144 tiles / 8
T2A_MAP_HEIGHT_BLOCKS = 512  # 4096 tiles / 8

# Magincia bounds (from T2A coordinates)
MAGINCIA_X1, MAGINCIA_Y1 = 3600, 2000
MAGINCIA_X2, MAGINCIA_Y2 = 3900, 2400

def get_block_number(block_x: int, block_y: int) -> int:
    """T2A block calculation"""
    return block_x * T2A_MAP_HEIGHT_BLOCKS + block_y

print("="*80)
print("MAGINCIA BLUEPRINT EXTRACTION FROM T2A")
print("="*80)

# Source: T2A map files
t2a_statics = Path(r'C:\DevEnv\GIT\UO\centredsharp\output-t2a\editable_maps\statics0.mul')
t2a_staidx = Path(r'C:\DevEnv\GIT\UO\centredsharp\output-t2a\editable_maps\staidx0.mul')

if not t2a_statics.exists() or not t2a_staidx.exists():
    print("ERROR: T2A map files not found!")
    exit(1)

print(f"Source: {t2a_statics.parent}")
print(f"Magincia bounds: ({MAGINCIA_X1}, {MAGINCIA_Y1}) to ({MAGINCIA_X2}, {MAGINCIA_Y2})")

# Calculate block range
block_x1 = MAGINCIA_X1 // 8
block_y1 = MAGINCIA_Y1 // 8
block_x2 = MAGINCIA_X2 // 8
block_y2 = MAGINCIA_Y2 // 8

print(f"Block range: ({block_x1}, {block_y1}) to ({block_x2}, {block_y2})")

# Extract all statics from Magincia
magincia_statics = []

with open(t2a_staidx, 'rb') as idx, open(t2a_statics, 'rb') as sta:
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
                if MAGINCIA_X1 <= world_x <= MAGINCIA_X2 and MAGINCIA_Y1 <= world_y <= MAGINCIA_Y2:
                    magincia_statics.append({
                        'tile_id': tile_id,
                        'x': world_x,
                        'y': world_y,
                        'z': z,
                        'hue': hue
                    })

print(f"\nExtracted {len(magincia_statics)} statics from Magincia")

# Calculate center point for the blueprint
center_x = (MAGINCIA_X1 + MAGINCIA_X2) // 2
center_y = (MAGINCIA_Y1 + MAGINCIA_Y2) // 2

print(f"Center point: ({center_x}, {center_y})")

# Convert to relative coordinates (offset from center)
blueprint_items = []
for static in magincia_statics:
    rel_x = static['x'] - center_x
    rel_y = static['y'] - center_y

    blueprint_items.append({
        'tile_id': static['tile_id'],
        'x': rel_x,
        'y': rel_y,
        'z': static['z'],
        'hue': static['hue']
    })

# Export as JSON blueprint
output_dir = Path("C:/DevEnv/GIT/UO/Vystia Town Generator/magincia_blueprint")
output_dir.mkdir(parents=True, exist_ok=True)

blueprint_data = {
    'name': 'Old Magincia (T2A)',
    'description': 'Complete Magincia island from T2A before destruction',
    'center': {'x': center_x, 'y': center_y},
    'bounds': {
        'x1': MAGINCIA_X1,
        'y1': MAGINCIA_Y1,
        'x2': MAGINCIA_X2,
        'y2': MAGINCIA_Y2
    },
    'total_items': len(blueprint_items),
    'items': blueprint_items
}

blueprint_file = output_dir / "magincia_complete.json"
with open(blueprint_file, 'w') as f:
    json.dump(blueprint_data, f, indent=2)

print(f"\n[OK] Blueprint saved to: {blueprint_file}")
print(f"[OK] Total items: {len(blueprint_items)}")
print("\n" + "="*80)
print("NEXT STEPS")
print("="*80)
print("1. Import this blueprint into CentrED+")
print("2. Position it at the Magincia location in your map")
print("3. Save from CentrED+ to update the map files")
