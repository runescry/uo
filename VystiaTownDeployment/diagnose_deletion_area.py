"""
Diagnose Deletion Area - Check what's actually in the deletion area blocks
Helps identify why only the deletion area becomes void.
"""

import struct
from pathlib import Path

# Map dimensions (ML: 896x512 blocks)
MAP_WIDTH_BLOCKS = 896
MAP_HEIGHT_BLOCKS = 512

def get_block_number(block_x: int, block_y: int) -> int:
    """CentrED's block calculation: blockNum = blockX * mapHeight + blockY"""
    return block_x * MAP_HEIGHT_BLOCKS + block_y

print("="*80)
print("DIAGNOSE DELETION AREA")
print("="*80)

dest_map = Path(r'C:\Program Files (x86)\Electronic Arts\Ultima Online Classic\map0.mul')

# Deletion coordinates
x1, y1 = 3477, 1904
x2, y2 = 3892, 2324

print(f"Map file: {dest_map}")
print(f"Deletion bounds: ({x1}, {y1}) to ({x2}, {y2})")

# Get file size
file_size = dest_map.stat().st_size
EXPECTED_BLOCKS = MAP_WIDTH_BLOCKS * MAP_HEIGHT_BLOCKS
BLOCK_SIZE = 196
EXPECTED_FILE_SIZE = EXPECTED_BLOCKS * BLOCK_SIZE

print(f"\nFile size: {file_size:,} bytes")
print(f"Expected size: {EXPECTED_FILE_SIZE:,} bytes")
print(f"Expected blocks: {EXPECTED_BLOCKS:,}")

# Calculate block range
min_block_x = x1 // 8
max_block_x = x2 // 8
min_block_y = y1 // 8
max_block_y = y2 // 8

print(f"\nBlock range: ({min_block_x}, {min_block_y}) to ({max_block_x}, {max_block_y})")

# Check blocks in deletion area
print(f"\nChecking blocks in deletion area...")
incomplete_blocks = []
missing_blocks = []
valid_blocks = []
corrupted_headers = []

with open(dest_map, 'rb') as map_file:
    for block_y in range(min_block_y, max_block_y + 1):
        for block_x in range(min_block_x, max_block_x + 1):
            block_id = get_block_number(block_x, block_y)
            block_offset = block_id * BLOCK_SIZE
            
            # Check if block offset is beyond file size
            if block_offset >= file_size:
                missing_blocks.append((block_x, block_y, block_id))
                continue
            
            try:
                map_file.seek(block_offset)
                
                # Read header
                header = map_file.read(4)
                if len(header) < 4:
                    incomplete_blocks.append((block_x, block_y, block_id, "header"))
                    continue
                
                # Check for unusual header values
                header_val = struct.unpack('<I', header)[0]
                if header_val != 0:
                    corrupted_headers.append((block_x, block_y, block_id, hex(header_val)))
                
                # Try to read all tiles
                tiles_read = 0
                for _ in range(64):
                    tile_data = map_file.read(3)
                    if len(tile_data) < 3:
                        incomplete_blocks.append((block_x, block_y, block_id, f"tile_{tiles_read}"))
                        break
                    tiles_read += 1
                
                if tiles_read == 64:
                    valid_blocks.append((block_x, block_y, block_id))
                    
            except (IOError, struct.error) as e:
                missing_blocks.append((block_x, block_y, block_id))

print(f"\nResults:")
print(f"  Valid blocks: {len(valid_blocks)}")
print(f"  Incomplete blocks: {len(incomplete_blocks)}")
print(f"  Missing blocks: {len(missing_blocks)}")
print(f"  Blocks with non-zero headers: {len(corrupted_headers)}")

if incomplete_blocks:
    print(f"\nIncomplete blocks (first 10):")
    for block_x, block_y, block_id, issue in incomplete_blocks[:10]:
        print(f"  Block ({block_x}, {block_y}) ID {block_id}: {issue}")

if missing_blocks:
    print(f"\nMissing blocks (first 10):")
    for block_x, block_y, block_id in missing_blocks[:10]:
        print(f"  Block ({block_x}, {block_y}) ID {block_id}: offset {block_id * BLOCK_SIZE} beyond file size")

if corrupted_headers:
    print(f"\nBlocks with non-zero headers (first 10):")
    for block_x, block_y, block_id, header_val in corrupted_headers[:10]:
        print(f"  Block ({block_x}, {block_y}) ID {block_id}: header = {header_val}")

# Check if blocks outside deletion area are different
print(f"\nComparing with blocks outside deletion area...")
outside_valid = 0
outside_incomplete = 0

# Check a sample of blocks outside deletion area
sample_blocks = [
    (min_block_x - 1, min_block_y - 1),  # Before deletion area
    (max_block_x + 1, max_block_y + 1),  # After deletion area
    (min_block_x, min_block_y - 1),  # Above deletion area
    (min_block_x - 1, min_block_y),  # Left of deletion area
]

for block_x, block_y in sample_blocks:
    if block_x < 0 or block_x >= MAP_WIDTH_BLOCKS or block_y < 0 or block_y >= MAP_HEIGHT_BLOCKS:
        continue
    
    block_id = get_block_number(block_x, block_y)
    block_offset = block_id * BLOCK_SIZE
    
    if block_offset >= file_size:
        outside_incomplete += 1
        continue
    
    try:
        map_file.seek(block_offset)
        header = map_file.read(4)
        if len(header) == 4:
            tiles_read = 0
            for _ in range(64):
                tile_data = map_file.read(3)
                if len(tile_data) == 3:
                    tiles_read += 1
            if tiles_read == 64:
                outside_valid += 1
            else:
                outside_incomplete += 1
        else:
            outside_incomplete += 1
    except:
        outside_incomplete += 1

print(f"  Sample blocks outside deletion area: {outside_valid} valid, {outside_incomplete} incomplete")

print("\n" + "="*80)
print("DIAGNOSIS COMPLETE")
print("="*80)

