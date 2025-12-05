"""
Delete Map Area - Delete land tiles and static tiles from destination map
Clears terrain and statics in specified coordinates and writes updated map files.
"""

import struct
import shutil
from pathlib import Path
import datetime
import random

# Map dimensions (ML: 896x512 blocks)
MAP_WIDTH_BLOCKS = 896   # 7168 tiles / 8
MAP_HEIGHT_BLOCKS = 512  # 4096 tiles / 8

def get_block_number(block_x: int, block_y: int) -> int:
    """CentrED's block calculation: blockNum = blockX * mapHeight + blockY"""
    return block_x * MAP_HEIGHT_BLOCKS + block_y

print("="*80)
print("MAP AREA DELETION")
print("="*80)

# Destination map files
dest_map = Path(r'C:\Program Files (x86)\Electronic Arts\Ultima Online Classic\map0.mul')
dest_statics = Path(r'C:\Program Files (x86)\Electronic Arts\Ultima Online Classic\statics0.mul')
dest_staidx = Path(r'C:\Program Files (x86)\Electronic Arts\Ultima Online Classic\staidx0.mul')

# Deletion coordinates (expanded by 22 tiles in each direction: 12 + 8 + 2)
x1, y1 = 3455, 1882  # 3457 - 2, 1884 - 2
x2, y2 = 3914, 2346  # 3912 + 2, 2344 + 2

print(f"Destination map: {dest_map}")
print(f"Destination statics: {dest_statics}")
print(f"Destination staidx: {dest_staidx}")
print(f"\nDeletion bounds: ({x1}, {y1}) to ({x2}, {y2})")

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
print(f"\n[1/4] Creating backup...")
script_dir = Path(__file__).parent
backup_dir = script_dir / "backups"
backup_dir.mkdir(parents=True, exist_ok=True)
timestamp = datetime.datetime.now().strftime("%Y%m%d_%H%M%S")
backup_subdir = backup_dir / f"delete_area_backup_{timestamp}"
backup_subdir.mkdir(parents=True, exist_ok=True)

shutil.copy2(dest_map, backup_subdir / 'map0.mul')
shutil.copy2(dest_statics, backup_subdir / 'statics0.mul')
shutil.copy2(dest_staidx, backup_subdir / 'staidx0.mul')
print(f"  Backup created: {backup_subdir}")

# Step 1: Load existing terrain blocks
print(f"\n[2/4] Loading existing terrain blocks...")
terrain_blocks = {}

# Default terrain: randomized tile IDs from 0x00a8-0x00ab at Z=-5
DEFAULT_TERRAIN_TILE_IDS = [0x00a8, 0x00a9, 0x00aa, 0x00ab]
DEFAULT_TERRAIN_Z = -5

def get_random_tile_id():
    """Return a random tile ID from the available set."""
    return random.choice(DEFAULT_TERRAIN_TILE_IDS)

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
                    # Incomplete read, use random default tile
                    tiles.append((get_random_tile_id(), DEFAULT_TERRAIN_Z))
                else:
                    tile_id = struct.unpack('<H', tile_data[0:2])[0]
                    z = struct.unpack('b', tile_data[2:3])[0]
                    tiles.append((tile_id, z))
            
            terrain_blocks[block_id] = {
                'header': header,
                'tiles': tiles
            }
        except (IOError, struct.error):
            # Block doesn't exist or is corrupted, initialize with random default tiles
            terrain_blocks[block_id] = {
                'header': b'\x00\x00\x00\x00',
                'tiles': [(get_random_tile_id(), DEFAULT_TERRAIN_Z) for _ in range(64)]
            }

print(f"  Loaded {len(terrain_blocks)} terrain blocks")

# Step 2: Replace terrain in deletion area with randomized tiles (0x00a8-0x00ab)
print(f"\n[3/4] Replacing terrain in deletion area with randomized tiles (0x00a8-0x00ab)...")
cleared_terrain_blocks = 0

min_block_x = x1 // 8
max_block_x = x2 // 8
min_block_y = y1 // 8
max_block_y = y2 // 8

for block_y in range(min_block_y, max_block_y + 1):
    for block_x in range(min_block_x, max_block_x + 1):
        block_id = get_block_number(block_x, block_y)
        
        # Ensure block exists and is fully initialized
        if block_id not in terrain_blocks:
            terrain_blocks[block_id] = {
                'header': b'\x00\x00\x00\x00',
                'tiles': [(get_random_tile_id(), DEFAULT_TERRAIN_Z) for _ in range(64)]
            }
        
        # Ensure block has exactly 64 tiles before clearing
        if len(terrain_blocks[block_id]['tiles']) < 64:
            terrain_blocks[block_id]['tiles'].extend([(get_random_tile_id(), DEFAULT_TERRAIN_Z) for _ in range(64 - len(terrain_blocks[block_id]['tiles']))])
        elif len(terrain_blocks[block_id]['tiles']) > 64:
            terrain_blocks[block_id]['tiles'] = terrain_blocks[block_id]['tiles'][:64]
        
        # Ensure header is valid
        if len(terrain_blocks[block_id]['header']) < 4:
            terrain_blocks[block_id]['header'] = b'\x00\x00\x00\x00'
        
        # Replace tiles within bounds with tile (0x00a9)
        for local_y in range(8):
            for local_x in range(8):
                world_x = block_x * 8 + local_x
                world_y = block_y * 8 + local_y
                
                # Only replace tiles within bounds with random tile
                if x1 <= world_x <= x2 and y1 <= world_y <= y2:
                    tile_index = local_y * 8 + local_x
                    terrain_blocks[block_id]['tiles'][tile_index] = (get_random_tile_id(), DEFAULT_TERRAIN_Z)
        
        cleared_terrain_blocks += 1

print(f"  Replaced {cleared_terrain_blocks} terrain blocks with randomized tiles (0x00a8-0x00ab)")

# Step 3: Load and clear statics
print(f"\n[4/4] Loading and clearing statics...")
static_blocks = {}

# Load existing statics
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

            # Calculate world coordinates
            world_x = block_x * 8 + local_x
            world_y = block_y * 8 + local_y

            # Keep statics outside deletion area
            if not (x1 <= world_x <= x2 and y1 <= world_y <= y2):
                block_statics.append({
                    'tile_id': tile_id,
                    'x': local_x,
                    'y': local_y,
                    'z': z,
                    'hue': hue
                })

        static_blocks[block_id] = block_statics

# Count cleared statics
total_original = sum(len(blocks) for blocks in static_blocks.values())
total_remaining = sum(len(blocks) for blocks in static_blocks.values())
cleared_statics = total_original - total_remaining

print(f"  Cleared {cleared_statics} static tiles")
print(f"  Remaining statics: {total_remaining}")

# Step 4: Write updated map files
print(f"\nWriting updated map files...")

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
                'tiles': [(get_random_tile_id(), DEFAULT_TERRAIN_Z) for _ in range(64)]
            }
        
        block_data = terrain_blocks[block_id]
        
        # Ensure we have exactly 64 tiles - update the block data
        if len(block_data['tiles']) < 64:
            block_data['tiles'].extend([(get_random_tile_id(), DEFAULT_TERRAIN_Z) for _ in range(64 - len(block_data['tiles']))])
        elif len(block_data['tiles']) > 64:
            block_data['tiles'] = block_data['tiles'][:64]
        
        tiles = block_data['tiles']
        
        # Write header as block ID (headers must match block index for client to recognize blocks)
        map_file.write(struct.pack('<I', block_id))
        
        # Write tiles (exactly 64 tiles * 3 bytes = 192 bytes)
        for tile_id, z in tiles[:64]:
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
print("DELETION COMPLETE")
print("="*80)
print(f"Replaced area: ({x1}, {y1}) to ({x2}, {y2})")
print(f"  Replaced {cleared_terrain_blocks} terrain blocks with randomized tiles (0x00a8-0x00ab)")
print(f"  Cleared {cleared_statics} static tiles")
print(f"\nBackup saved to: {backup_subdir}")
print("\nNext step: Run place_map_area.py to place extracted tiles")

