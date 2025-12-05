"""
Extract Map Area - Extract land tiles and static tiles from source map
Extracts terrain and statics from specified coordinates and saves to JSON.
"""

import struct
import json
from pathlib import Path
import datetime

# Map dimensions (ML: 896x512 blocks)
MAP_WIDTH_BLOCKS = 896   # 7168 tiles / 8
MAP_HEIGHT_BLOCKS = 512  # 4096 tiles / 8

def get_block_number(block_x: int, block_y: int) -> int:
    """CentrED's block calculation: blockNum = blockX * mapHeight + blockY"""
    return block_x * MAP_HEIGHT_BLOCKS + block_y

print("="*80)
print("MAP AREA EXTRACTION")
print("="*80)

# Source map files
source_map = Path(r'D:\client\map0.mul')
source_statics = Path(r'D:\client\statics0.mul')
source_staidx = Path(r'D:\client\staidx0.mul')

# Extraction coordinates (source map)
x1, y1 = 3477, 1910
x2, y2 = 3890, 2324

print(f"Source map: {source_map}")
print(f"Source statics: {source_statics}")
print(f"Source staidx: {source_staidx}")
print(f"\nExtraction bounds: ({x1}, {y1}) to ({x2}, {y2})")

# Verify files exist
if not source_map.exists():
    print(f"\nERROR: Source map file not found: {source_map}")
    exit(1)

if not source_statics.exists() or not source_staidx.exists():
    print(f"\nERROR: Source static files not found!")
    print(f"  Statics: {source_statics}")
    print(f"  StaIdx: {source_staidx}")
    exit(1)

# Initialize extraction data
extracted_data = {
    'source': 'T2A Editable Maps',
    'extraction_date': datetime.datetime.now().isoformat(),
    'bounds': {
        'x1': x1,
        'y1': y1,
        'x2': x2,
        'y2': y2
    },
    'terrain': [],
    'statics': []
}

# Step 1: Extract terrain tiles
print(f"\n[1/2] Extracting terrain tiles...")
terrain_count = 0

with open(source_map, 'rb') as map_file:
    for y in range(y1, y2 + 1):
        for x in range(x1, x2 + 1):
            block_x = x // 8
            block_y = y // 8
            cell_x = x % 8
            cell_y = y % 8

            # CentrED block calculation
            block_id = get_block_number(block_x, block_y)
            block_offset = block_id * 196  # Each block is 196 bytes
            cell_offset = block_offset + 4 + (cell_y * 8 + cell_x) * 3

            try:
                map_file.seek(cell_offset)
                tile_id = struct.unpack('<H', map_file.read(2))[0]
                z = struct.unpack('b', map_file.read(1))[0]

                extracted_data['terrain'].append({
                    'x': x,
                    'y': y,
                    'tile_id': tile_id,
                    'z': z
                })
                terrain_count += 1
            except (struct.error, IOError) as e:
                print(f"Warning: Could not read terrain at ({x}, {y}): {e}")

print(f"  Extracted {terrain_count} terrain tiles")

# Step 2: Extract static tiles
print(f"\n[2/2] Extracting static tiles...")
static_count = 0

# Calculate block range
min_block_x = x1 // 8
max_block_x = x2 // 8
min_block_y = y1 // 8
max_block_y = y2 // 8

with open(source_staidx, 'rb') as idx, open(source_statics, 'rb') as sta:
    for block_y in range(min_block_y, max_block_y + 1):
        for block_x in range(min_block_x, max_block_x + 1):
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

                # Only include statics within the exact bounds
                if x1 <= world_x <= x2 and y1 <= world_y <= y2:
                    extracted_data['statics'].append({
                        'tile_id': tile_id,
                        'x': world_x,
                        'y': world_y,
                        'z': z,
                        'hue': hue
                    })
                    static_count += 1

print(f"  Extracted {static_count} static tiles")

# Add statistics
terrain_z = [t['z'] for t in extracted_data['terrain']]
static_z = [s['z'] for s in extracted_data['statics']]

extracted_data['statistics'] = {
    'total_terrain_tiles': len(extracted_data['terrain']),
    'total_statics': len(extracted_data['statics']),
    'terrain_z_range': [min(terrain_z), max(terrain_z)] if terrain_z else [0, 0],
    'terrain_z_avg': sum(terrain_z) / len(terrain_z) if terrain_z else 0,
    'static_z_range': [min(static_z), max(static_z)] if static_z else [0, 0],
    'static_z_avg': sum(static_z) / len(static_z) if static_z else 0
}

# Save to JSON
script_dir = Path(__file__).parent
output_dir = script_dir / "extracted_areas"
output_dir.mkdir(parents=True, exist_ok=True)

timestamp = datetime.datetime.now().strftime("%Y%m%d_%H%M%S")
export_file = output_dir / f"extracted_area_{timestamp}.json"

with open(export_file, 'w') as f:
    json.dump(extracted_data, f, indent=2)

print("\n" + "="*80)
print("EXTRACTION COMPLETE")
print("="*80)
print(f"Saved to: {export_file}")
print(f"\nStatistics:")
print(f"  Terrain tiles: {extracted_data['statistics']['total_terrain_tiles']}")
print(f"  Static tiles: {extracted_data['statistics']['total_statics']}")
print(f"  Terrain Z range: {extracted_data['statistics']['terrain_z_range']}")
print(f"  Terrain Z avg: {extracted_data['statistics']['terrain_z_avg']:.1f}")
print(f"  Static Z range: {extracted_data['statistics']['static_z_range']}")
print(f"  Static Z avg: {extracted_data['statistics']['static_z_avg']:.1f}")
print("\nNext steps:")
print("1. Review the exported JSON file to verify the extraction")
print("2. Run delete_map_area.py to clear the destination area")
print("3. Run place_map_area.py to place the extracted tiles")

