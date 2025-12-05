"""
Check Tile Data - Compare tile data in backup vs current file
"""

import struct
from pathlib import Path

MAP_WIDTH_BLOCKS = 896
MAP_HEIGHT_BLOCKS = 512

def get_block_number(block_x: int, block_y: int) -> int:
    return block_x * MAP_HEIGHT_BLOCKS + block_y

backup_map = Path(r'Vystia Town Generator/backups/delete_area_backup_20251119_173651/map0.mul')
current_map = Path(r'C:\Program Files (x86)\Electronic Arts\Ultima Online Classic\map0.mul')

# Check blocks in deletion area
del_x1, del_y1 = 3477, 1904
del_x2, del_y2 = 3892, 2324

min_block_x = del_x1 // 8
min_block_y = del_y1 // 8

BLOCK_SIZE = 196

print("Checking tile data in backup vs current file...")
print(f"Checking block ({min_block_x}, {min_block_y})...")

block_id = get_block_number(min_block_x, min_block_y)
offset = block_id * BLOCK_SIZE

with open(backup_map, 'rb') as backup_file, open(current_map, 'rb') as current_file:
    # Read backup block
    backup_file.seek(offset)
    backup_header = backup_file.read(4)
    backup_tiles = []
    for i in range(64):
        tile_data = backup_file.read(3)
        if len(tile_data) == 3:
            tile_id = struct.unpack('<H', tile_data[0:2])[0]
            z = struct.unpack('b', tile_data[2:3])[0]
            backup_tiles.append((tile_id, z))
    
    # Read current block
    current_file.seek(offset)
    current_header = current_file.read(4)
    current_tiles = []
    for i in range(64):
        tile_data = current_file.read(3)
        if len(tile_data) == 3:
            tile_id = struct.unpack('<H', tile_data[0:2])[0]
            z = struct.unpack('b', tile_data[2:3])[0]
            current_tiles.append((tile_id, z))
    
    print(f"\nBlock ID: {block_id}")
    print(f"Offset: {offset}")
    
    print(f"\nBackup header: {backup_header.hex()}")
    print(f"Current header: {current_header.hex()}")
    print(f"Headers match: {backup_header == current_header}")
    
    print(f"\nTile comparison (first 10 tiles):")
    mismatches = 0
    for i in range(min(10, len(backup_tiles), len(current_tiles))):
        backup_tile = backup_tiles[i]
        current_tile = current_tiles[i]
        match = backup_tile == current_tile
        if not match:
            mismatches += 1
        print(f"  Tile {i}: Backup={backup_tile}, Current={current_tile}, Match={match}")
    
    # Check if all tiles in deletion area are grass (tile_id=1, z=0)
    grass_count = sum(1 for t in current_tiles if t == (1, 0))
    print(f"\nCurrent block tile analysis:")
    print(f"  Total tiles: {len(current_tiles)}")
    print(f"  Grass tiles (1, 0): {grass_count}")
    print(f"  Non-grass tiles: {len(current_tiles) - grass_count}")
    
    if grass_count == 64:
        print(f"  *** ALL TILES ARE GRASS - This is expected after deletion ***")
    
    # Check world coordinates within deletion bounds
    print(f"\nChecking tiles within deletion bounds ({del_x1}, {del_y1}) to ({del_x2}, {del_y2}):")
    tiles_in_bounds = 0
    tiles_out_bounds = 0
    for local_y in range(8):
        for local_x in range(8):
            world_x = min_block_x * 8 + local_x
            world_y = min_block_y * 8 + local_y
            tile_index = local_y * 8 + local_x
            if del_x1 <= world_x <= del_x2 and del_y1 <= world_y <= del_y2:
                tiles_in_bounds += 1
                if current_tiles[tile_index] != (1, 0):
                    print(f"  Tile ({world_x}, {world_y}) at index {tile_index} is NOT grass: {current_tiles[tile_index]}")
            else:
                tiles_out_bounds += 1
    
    print(f"  Tiles in deletion bounds: {tiles_in_bounds}")
    print(f"  Tiles outside deletion bounds: {tiles_out_bounds}")

