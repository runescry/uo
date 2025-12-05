"""
Fill Water Area - Fill a specific area with water tiles
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
print("FILL WATER AREA")
print("="*80)

# Destination map files
dest_map = Path(r'C:\Program Files (x86)\Electronic Arts\Ultima Online Classic\map0.mul')

# Water fill coordinates
x1, y1 = 3506, 1904
x2, y2 = 3892, 1910

print(f"Destination map: {dest_map}")
print(f"\nWater fill bounds: ({x1}, {y1}) to ({x2}, {y2})")

# Verify file exists
if not dest_map.exists():
    print(f"\nERROR: Destination map file not found: {dest_map}")
    exit(1)

# Create backup
print(f"\n[1/3] Creating backup...")
script_dir = Path(__file__).parent
backup_dir = script_dir / "backups"
backup_dir.mkdir(parents=True, exist_ok=True)
timestamp = datetime.datetime.now().strftime("%Y%m%d_%H%M%S")
backup_subdir = backup_dir / f"water_fill_backup_{timestamp}"
backup_subdir.mkdir(parents=True, exist_ok=True)

shutil.copy2(dest_map, backup_subdir / 'map0.mul')
print(f"  Backup created: {backup_subdir}")

# Load existing terrain blocks
print(f"\n[2/3] Loading existing terrain blocks...")
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
                tiles = [(0x0001, 0)] * 64
            else:
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

# Fill area with water tiles
print(f"\n[3/3] Filling area with water tiles...")
WATER_TILE_ID = 0x0000  # Water tile ID
WATER_Z = 0  # Water Z-level
water_tiles_placed = 0

for y in range(y1, y2 + 1):
    for x in range(x1, x2 + 1):
        block_x = x // 8
        block_y = y // 8
        local_x = x & 0x7
        local_y = y & 0x7
        
        block_id = get_block_number(block_x, block_y)
        tile_index = local_y * 8 + local_x
        
        if block_id in terrain_blocks:
            terrain_blocks[block_id]['tiles'][tile_index] = (WATER_TILE_ID, WATER_Z)
            water_tiles_placed += 1

print(f"  Placed {water_tiles_placed} water tiles")

# Write updated map file
print(f"\nWriting updated map file...")

with open(dest_map, 'wb') as map_file:
    for block_id in range(MAP_WIDTH_BLOCKS * MAP_HEIGHT_BLOCKS):
        block_data = terrain_blocks[block_id]
        
        # Write header
        map_file.write(block_data['header'])
        
        # Write tiles
        for tile_id, z in block_data['tiles']:
            map_file.write(struct.pack('<H', tile_id))
            map_file.write(struct.pack('b', z))
        
        if block_id % 50000 == 0 and block_id > 0:
            print(f"  Progress: {block_id}/{MAP_WIDTH_BLOCKS * MAP_HEIGHT_BLOCKS}...")

print(f"  Terrain written")

print("\n" + "="*80)
print("WATER FILL COMPLETE")
print("="*80)
print(f"Filled area: ({x1}, {y1}) to ({x2}, {y2})")
print(f"  Water tiles placed: {water_tiles_placed}")
print(f"\nBackup saved to: {backup_subdir}")

