"""
Place Map Area - Place extracted tiles at destination coordinates
Loads extracted JSON data, applies coordinate shift, and places tiles on destination map.
NOTE: Water fill should be done separately using place_water_area.py
"""

import struct
import json
import shutil
from pathlib import Path
import datetime

# Map dimensions (ML: 896x512 blocks)
MAP_WIDTH_BLOCKS = 896   # 7168 tiles / 8
MAP_HEIGHT_BLOCKS = 512  # 4096 tiles / 8

def get_block_number(block_x: int, block_y: int) -> int:
    """CentrED's block calculation: blockNum = blockX * mapHeight + blockY"""
    return block_x * MAP_HEIGHT_BLOCKS + block_y

print("="*80)
print("MAP AREA PLACEMENT")
print("="*80)

# Destination map files
dest_map = Path(r'C:\Program Files (x86)\Electronic Arts\Ultima Online Classic\map0.mul')
dest_statics = Path(r'C:\Program Files (x86)\Electronic Arts\Ultima Online Classic\statics0.mul')
dest_staidx = Path(r'C:\Program Files (x86)\Electronic Arts\Ultima Online Classic\staidx0.mul')

# Destination coordinates (where to place extracted tiles)
dest_x1, dest_y1 = 3501, 1911
# Calculate destination end based on source size
source_x1, source_y1 = 3477, 1910
source_x2, source_y2 = 3890, 2324
source_width = source_x2 - source_x1 + 1
source_height = source_y2 - source_y1 + 1
dest_x2 = dest_x1 + source_width - 1
dest_y2 = dest_y1 + source_height - 1

# Calculate coordinate shift
shift_x = dest_x1 - source_x1  # 3501 - 3477 = 24
shift_y = dest_y1 - source_y1  # 1911 - 1910 = 1

print(f"Destination map: {dest_map}")
print(f"Destination statics: {dest_statics}")
print(f"Destination staidx: {dest_staidx}")
print(f"\nSource bounds: ({source_x1}, {source_y1}) to ({source_x2}, {source_y2})")
print(f"Destination bounds: ({dest_x1}, {dest_y1}) to ({dest_x2}, {dest_y2})")
print(f"Coordinate shift: ({shift_x}, {shift_y})")

# Find latest extracted JSON file
script_dir = Path(__file__).parent
extracted_dir = script_dir / "extracted_areas"
if not extracted_dir.exists():
    print(f"\nERROR: Extracted areas directory not found: {extracted_dir}")
    print("Please run extract_map_area.py first")
    exit(1)

json_files = list(extracted_dir.glob("extracted_area_*.json"))
if not json_files:
    print(f"\nERROR: No extracted JSON files found in {extracted_dir}")
    print("Please run extract_map_area.py first")
    exit(1)

# Use the latest file
json_file = max(json_files, key=lambda p: p.stat().st_mtime)
print(f"\nLoading extracted data from: {json_file}")

# Load extracted data
with open(json_file, 'r') as f:
    extracted_data = json.load(f)

print(f"  Source: {extracted_data.get('source', 'Unknown')}")
print(f"  Terrain tiles: {extracted_data['statistics']['total_terrain_tiles']}")
print(f"  Static tiles: {extracted_data['statistics']['total_statics']}")

# Verify files exist
if not dest_map.exists():
    print(f"\nERROR: Destination map file not found: {dest_map}")
    exit(1)

if not dest_statics.exists() or not dest_staidx.exists():
    print(f"\nERROR: Destination static files not found!")
    print(f"  Statics: {dest_statics}")
    print(f"  StaIdx: {dest_staidx}")
    exit(1)

# Create backup
print(f"\n[1/5] Creating backup...")
backup_dir = script_dir / "backups"
backup_dir.mkdir(parents=True, exist_ok=True)
timestamp = datetime.datetime.now().strftime("%Y%m%d_%H%M%S")
backup_subdir = backup_dir / f"place_area_backup_{timestamp}"
backup_subdir.mkdir(parents=True, exist_ok=True)

shutil.copy2(dest_map, backup_subdir / 'map0.mul')
shutil.copy2(dest_statics, backup_subdir / 'statics0.mul')
shutil.copy2(dest_staidx, backup_subdir / 'staidx0.mul')
print(f"  Backup created: {backup_subdir}")

# Step 1: Load existing terrain blocks
print(f"\n[2/5] Loading existing terrain blocks...")
terrain_blocks = {}

with open(dest_map, 'rb') as map_file:
    for block_id in range(MAP_WIDTH_BLOCKS * MAP_HEIGHT_BLOCKS):
        block_offset = block_id * 196
        
        try:
            map_file.seek(block_offset)
            
            # Read header (4 bytes)
            header = map_file.read(4)
            if len(header) < 4:
                # Incomplete block, use defaults
                header = b'\x00\x00\x00\x00'
                tiles = [(0x0001, 0)] * 64  # Default grass tile
            else:
                # Read all 64 tiles (8x8)
                tiles = []
                for _ in range(64):
                    tile_data = map_file.read(3)
                    if len(tile_data) < 3:
                        # Incomplete tile, use default
                        tiles.append((0x0001, 0))
                    else:
                        tile_id = struct.unpack('<H', tile_data[0:2])[0]
                        z = struct.unpack('b', tile_data[2:3])[0]
                        tiles.append((tile_id, z))
            
            terrain_blocks[block_id] = {
                'header': header,
                'tiles': tiles
            }
        except (IOError, struct.error):
            # Block doesn't exist or is corrupted, use defaults
            terrain_blocks[block_id] = {
                'header': b'\x00\x00\x00\x00',
                'tiles': [(0x0001, 0)] * 64
            }

print(f"  Loaded {len(terrain_blocks)} terrain blocks")

# Step 2: Shift and filter terrain tiles
print(f"\n[2/5] Shifting terrain tiles...")
shifted_terrain = []
placed_terrain = 0
skipped_terrain = 0

for tile in extracted_data['terrain']:
    new_x = tile['x'] + shift_x
    new_y = tile['y'] + shift_y
    
    # Only place tiles within destination bounds
    if dest_x1 <= new_x <= dest_x2 and dest_y1 <= new_y <= dest_y2:
        shifted_terrain.append({
            'x': new_x,
            'y': new_y,
            'tile_id': tile['tile_id'],
            'z': tile['z']
        })
        placed_terrain += 1
    else:
        skipped_terrain += 1

print(f"  Placed {placed_terrain} terrain tiles")
print(f"  Skipped {skipped_terrain} terrain tiles (outside bounds)")

# Step 3: Shift and filter static tiles
print(f"\n[3/5] Shifting static tiles...")
shifted_statics = []
placed_statics = 0
skipped_statics = 0

for static in extracted_data['statics']:
    new_x = static['x'] + shift_x
    new_y = static['y'] + shift_y
    
    # Only place statics within destination bounds
    if dest_x1 <= new_x <= dest_x2 and dest_y1 <= new_y <= dest_y2:
        shifted_statics.append({
            'tile_id': static['tile_id'],
            'x': new_x,
            'y': new_y,
            'z': static['z'],
            'hue': static['hue']
        })
        placed_statics += 1
    else:
        skipped_statics += 1

print(f"  Placed {placed_statics} static tiles")
print(f"  Skipped {skipped_statics} static tiles (outside bounds)")

# Place terrain tiles
print(f"  Placing terrain tiles...")
for tile in shifted_terrain:
    block_x = tile['x'] // 8
    block_y = tile['y'] // 8
    local_x = tile['x'] & 0x7
    local_y = tile['y'] & 0x7
    
    block_id = get_block_number(block_x, block_y)
    tile_index = local_y * 8 + local_x
    
    # Ensure block exists and is fully initialized
    if block_id not in terrain_blocks:
        terrain_blocks[block_id] = {
            'header': b'\x00\x00\x00\x00',
            'tiles': [(0x0001, 0)] * 64  # Default grass tiles
        }
    else:
        # Ensure block has exactly 64 tiles
        if len(terrain_blocks[block_id]['tiles']) < 64:
            terrain_blocks[block_id]['tiles'].extend([(0x0001, 0)] * (64 - len(terrain_blocks[block_id]['tiles'])))
        elif len(terrain_blocks[block_id]['tiles']) > 64:
            terrain_blocks[block_id]['tiles'] = terrain_blocks[block_id]['tiles'][:64]
    
    # Place the tile
    terrain_blocks[block_id]['tiles'][tile_index] = (tile['tile_id'], tile['z'])

# Step 4: Load existing statics
print(f"\n[4/5] Loading existing statics...")
static_blocks = {}

with open(dest_staidx, 'rb') as idx, open(dest_statics, 'rb') as sta:
    for block_id in range(MAP_WIDTH_BLOCKS * MAP_HEIGHT_BLOCKS):
        idx.seek(block_id * 12)
        offset = struct.unpack('<I', idx.read(4))[0]
        length = struct.unpack('<I', idx.read(4))[0]

        if offset == 0xFFFFFFFF or length == 0:
            static_blocks[block_id] = []
            continue

        sta.seek(offset)
        count = length // 7
        block_statics = []

        # Calculate block coordinates from block_id
        block_x = block_id // MAP_HEIGHT_BLOCKS
        block_y = block_id % MAP_HEIGHT_BLOCKS

        for _ in range(count):
            tile_id = struct.unpack('<H', sta.read(2))[0]
            local_x = struct.unpack('B', sta.read(1))[0]
            local_y = struct.unpack('B', sta.read(1))[0]
            z = struct.unpack('b', sta.read(1))[0]
            hue = struct.unpack('<H', sta.read(2))[0]

            block_statics.append({
                'tile_id': tile_id,
                'x': local_x,
                'y': local_y,
                'z': z,
                'hue': hue
            })

        static_blocks[block_id] = block_statics

print(f"  Loaded {len(static_blocks)} static blocks")

# Clear destination area statics first
min_block_x = dest_x1 // 8
max_block_x = dest_x2 // 8
min_block_y = dest_y1 // 8
max_block_y = dest_y2 // 8

cleared_statics = 0
for block_y in range(min_block_y, max_block_y + 1):
    for block_x in range(min_block_x, max_block_x + 1):
        block_id = get_block_number(block_x, block_y)
        
        if block_id in static_blocks:
            # Remove statics within destination bounds
            original_count = len(static_blocks[block_id])
            static_blocks[block_id] = [
                s for s in static_blocks[block_id]
                if not (dest_x1 <= block_x * 8 + s['x'] <= dest_x2 and
                       dest_y1 <= block_y * 8 + s['y'] <= dest_y2)
            ]
            cleared_statics += original_count - len(static_blocks[block_id])

print(f"  Cleared {cleared_statics} existing statics from destination area")

# Group shifted statics into blocks
for static in shifted_statics:
    block_x = static['x'] // 8
    block_y = static['y'] // 8
    local_x = static['x'] & 0x7
    local_y = static['y'] & 0x7
    
    block_id = get_block_number(block_x, block_y)
    
    if block_id not in static_blocks:
        static_blocks[block_id] = []
    
    static_blocks[block_id].append({
        'tile_id': static['tile_id'],
        'x': local_x,
        'y': local_y,
        'z': static['z'],
        'hue': static['hue']
    })

print(f"  Added {placed_statics} new statics")

# Step 5: Write updated map files
print(f"\n[5/5] Writing updated map files...")

# Write terrain (map0.mul)
# Calculate exact file size to ensure we write exactly the right number of blocks
EXPECTED_BLOCKS = MAP_WIDTH_BLOCKS * MAP_HEIGHT_BLOCKS  # 458752
BLOCK_SIZE = 196  # 4 bytes header + 64 tiles * 3 bytes each
EXPECTED_FILE_SIZE = EXPECTED_BLOCKS * BLOCK_SIZE

with open(dest_map, 'wb') as map_file:
    # Truncate file to ensure correct size
    map_file.truncate(EXPECTED_FILE_SIZE)
    # Ensure we start writing from position 0
    map_file.seek(0)
    
    for block_id in range(EXPECTED_BLOCKS):
        # Ensure block exists, initialize if missing
        if block_id not in terrain_blocks:
            terrain_blocks[block_id] = {
                'header': b'\x00\x00\x00\x00',
                'tiles': [(0x0001, 0)] * 64  # Default grass tiles
            }
        
        block_data = terrain_blocks[block_id]
        
        # Ensure header is 4 bytes
        header = block_data['header']
        if len(header) < 4:
            header = b'\x00\x00\x00\x00'[:4]
        
        # Ensure we have exactly 64 tiles
        tiles = block_data['tiles']
        if len(tiles) < 64:
            tiles.extend([(0x0001, 0)] * (64 - len(tiles)))
        tiles = tiles[:64]
        
        # Write header as block ID (headers must match block index for client to recognize blocks)
        map_file.write(struct.pack('<I', block_id))
        
        # Write tiles
        for tile_id, z in tiles:
            map_file.write(struct.pack('<H', tile_id))
            map_file.write(struct.pack('b', z))
        
        if block_id % 50000 == 0 and block_id > 0:
            print(f"  Terrain progress: {block_id}/{EXPECTED_BLOCKS}...")
    
    # Ensure file is exactly the right size
    current_pos = map_file.tell()
    if current_pos != EXPECTED_FILE_SIZE:
        map_file.truncate(EXPECTED_FILE_SIZE)
        print(f"  File truncated from {current_pos} to {EXPECTED_FILE_SIZE} bytes")

print(f"  Terrain written ({EXPECTED_BLOCKS} blocks, {EXPECTED_FILE_SIZE} bytes)")

# Write statics
with open(dest_statics, 'wb') as sta, open(dest_staidx, 'wb') as idx:
    current_offset = 0

    for block_id in range(MAP_WIDTH_BLOCKS * MAP_HEIGHT_BLOCKS):
        statics = static_blocks[block_id]

        if len(statics) > 0:
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
            print(f"  Statics progress: {block_id}/{MAP_WIDTH_BLOCKS * MAP_HEIGHT_BLOCKS}...")

print(f"  Statics written")

print("\n" + "="*80)
print("PLACEMENT COMPLETE")
print("="*80)
print(f"Extracted tiles placed at: ({dest_x1}, {dest_y1}) to ({dest_x2}, {dest_y2})")
print(f"  Terrain tiles placed: {placed_terrain}")
print(f"  Static tiles placed: {placed_statics}")
print(f"\nBackup saved to: {backup_subdir}")
print("\nAll operations complete!")

