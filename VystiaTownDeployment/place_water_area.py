"""
Place Water Area - Fill a specific area with water tiles
Fills the specified coordinates with water tiles in the destination map.
"""

import struct
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
print("PLACE WATER AREA")
print("="*80)

# Destination map files
dest_map = Path(r'C:\Program Files (x86)\Electronic Arts\Ultima Online Classic\map0.mul')

# Water fill coordinates
water_x1, water_y1 = 3405, 1900
water_x2, water_y2 = 3916, 2348

print(f"Destination map: {dest_map}")
print(f"\nWater fill bounds: ({water_x1}, {water_y1}) to ({water_x2}, {water_y2})")

# Verify file exists
if not dest_map.exists():
    print(f"\nERROR: Destination map file not found: {dest_map}")
    exit(1)

# Create backup
print(f"\n[1/4] Creating backup...")
script_dir = Path(__file__).parent
backup_dir = script_dir / "backups"
backup_dir.mkdir(parents=True, exist_ok=True)
timestamp = datetime.datetime.now().strftime("%Y%m%d_%H%M%S")
backup_subdir = backup_dir / f"water_place_backup_{timestamp}"
backup_subdir.mkdir(parents=True, exist_ok=True)

shutil.copy2(dest_map, backup_subdir / 'map0.mul')
print(f"  Backup created: {backup_subdir}")

# Load existing terrain blocks
print(f"\n[2/4] Loading existing terrain blocks...")
terrain_blocks = {}

with open(dest_map, 'rb') as map_file:
    for block_id in range(MAP_WIDTH_BLOCKS * MAP_HEIGHT_BLOCKS):
        block_offset = block_id * 196
        
        try:
            map_file.seek(block_offset)
            
            # Read header (4 bytes)
            header = map_file.read(4)
            if len(header) < 4:
                header = b'\x00\x00\x00\x00'
            
            # Read all 64 tiles (8x8)
            tiles = []
            for _ in range(64):
                tile_data = map_file.read(3)
                if len(tile_data) < 3:
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
            terrain_blocks[block_id] = {
                'header': b'\x00\x00\x00\x00',
                'tiles': [(0x0001, 0)] * 64
            }

print(f"  Loaded {len(terrain_blocks)} terrain blocks")

# Pre-initialize all blocks touched by water fill area
print(f"\n[3/4] Pre-initializing blocks in water fill area...")
min_block_x = water_x1 // 8
max_block_x = water_x2 // 8
min_block_y = water_y1 // 8
max_block_y = water_y2 // 8

blocks_initialized = 0
for block_y in range(min_block_y, max_block_y + 1):
    for block_x in range(min_block_x, max_block_x + 1):
        block_id = get_block_number(block_x, block_y)
        
        # Ensure block exists and is fully initialized
        if block_id not in terrain_blocks:
            terrain_blocks[block_id] = {
                'header': b'\x00\x00\x00\x00',
                'tiles': [(0x0001, 0)] * 64
            }
            blocks_initialized += 1
        else:
            # Ensure block has exactly 64 tiles
            if len(terrain_blocks[block_id]['tiles']) < 64:
                terrain_blocks[block_id]['tiles'].extend([(0x0001, 0)] * (64 - len(terrain_blocks[block_id]['tiles'])))
            elif len(terrain_blocks[block_id]['tiles']) > 64:
                terrain_blocks[block_id]['tiles'] = terrain_blocks[block_id]['tiles'][:64]
            
            # Ensure header is valid
            if len(terrain_blocks[block_id]['header']) < 4:
                terrain_blocks[block_id]['header'] = b'\x00\x00\x00\x00'

print(f"  Initialized {blocks_initialized} new blocks")

# Fill water tiles
WATER_TILE_ID = 0x0000
WATER_Z = 0
water_tiles_placed = 0

for y in range(water_y1, water_y2 + 1):
    for x in range(water_x1, water_x2 + 1):
        block_x = x // 8
        block_y = y // 8
        local_x = x & 0x7
        local_y = y & 0x7
        
        block_id = get_block_number(block_x, block_y)
        tile_index = local_y * 8 + local_x
        
        # Block should already be initialized
        if block_id not in terrain_blocks:
            terrain_blocks[block_id] = {
                'header': b'\x00\x00\x00\x00',
                'tiles': [(0x0001, 0)] * 64
            }
        
        terrain_blocks[block_id]['tiles'][tile_index] = (WATER_TILE_ID, WATER_Z)
        water_tiles_placed += 1

print(f"  Placed {water_tiles_placed} water tiles")

# Write updated map file
print(f"\n[4/4] Writing updated map file...")
EXPECTED_BLOCKS = MAP_WIDTH_BLOCKS * MAP_HEIGHT_BLOCKS
BLOCK_SIZE = 196
EXPECTED_FILE_SIZE = EXPECTED_BLOCKS * BLOCK_SIZE

with open(dest_map, 'wb') as map_file:
    map_file.truncate(EXPECTED_FILE_SIZE)
    # Ensure we start writing from position 0
    map_file.seek(0)
    
    for block_id in range(EXPECTED_BLOCKS):
        # Ensure block exists
        if block_id not in terrain_blocks:
            terrain_blocks[block_id] = {
                'header': b'\x00\x00\x00\x00',
                'tiles': [(0x0001, 0)] * 64
            }
        
        block_data = terrain_blocks[block_id]
        
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
            print(f"  Progress: {block_id}/{EXPECTED_BLOCKS}...")
    
    # Ensure file is exactly the right size
    current_pos = map_file.tell()
    if current_pos != EXPECTED_FILE_SIZE:
        map_file.truncate(EXPECTED_FILE_SIZE)
        print(f"  File truncated from {current_pos} to {EXPECTED_FILE_SIZE} bytes")

print(f"  Map written ({EXPECTED_BLOCKS} blocks, {EXPECTED_FILE_SIZE} bytes)")

print("\n" + "="*80)
print("WATER PLACEMENT COMPLETE")
print("="*80)
print(f"Water tiles placed in area: ({water_x1}, {water_y1}) to ({water_x2}, {water_y2})")
print(f"  Water tiles placed: {water_tiles_placed}")
print(f"\nBackup saved to: {backup_subdir}")

