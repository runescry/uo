"""
Compare Map Blocks - Compare blocks in deletion area vs other areas
Helps identify what's different about blocks that become void.
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
print("COMPARE MAP BLOCKS")
print("="*80)

dest_map = Path(r'C:\Program Files (x86)\Electronic Arts\Ultima Online Classic\map0.mul')

# Deletion coordinates
del_x1, del_y1 = 3477, 1904
del_x2, del_y2 = 3892, 2324

# Comparison coordinates (known good area)
comp_x1, comp_y1 = 3500, 2000
comp_x2, comp_y2 = 3520, 2020

print(f"Map file: {dest_map}")
print(f"Deletion bounds: ({del_x1}, {del_y1}) to ({del_x2}, {del_y2})")
print(f"Comparison bounds: ({comp_x1}, {comp_y1}) to ({comp_x2}, {comp_y2})")

# Get file size
file_size = dest_map.stat().st_size
BLOCK_SIZE = 196
EXPECTED_BLOCKS = MAP_WIDTH_BLOCKS * MAP_HEIGHT_BLOCKS
EXPECTED_FILE_SIZE = EXPECTED_BLOCKS * BLOCK_SIZE

print(f"\nFile size: {file_size:,} bytes")
print(f"Expected size: {EXPECTED_FILE_SIZE:,} bytes")
print(f"Expected blocks: {EXPECTED_BLOCKS:,}")

# Calculate block ranges
del_min_block_x = del_x1 // 8
del_max_block_x = del_x2 // 8
del_min_block_y = del_y1 // 8
del_max_block_y = del_y2 // 8

comp_min_block_x = comp_x1 // 8
comp_max_block_x = comp_x2 // 8
comp_min_block_y = comp_y1 // 8
comp_max_block_y = comp_y2 // 8

print(f"\nDeletion block range: ({del_min_block_x}, {del_min_block_y}) to ({del_max_block_x}, {del_max_block_y})")
print(f"Comparison block range: ({comp_min_block_x}, {comp_min_block_y}) to ({comp_max_block_x}, {comp_max_block_y})")

# Analyze blocks
print(f"\nAnalyzing blocks...")

deletion_blocks = []
comparison_blocks = []

with open(dest_map, 'rb') as map_file:
    # Analyze deletion area blocks
    print(f"\n--- DELETION AREA BLOCKS ---")
    for block_y in range(del_min_block_y, del_max_block_y + 1):
        for block_x in range(del_min_block_x, del_max_block_x + 1):
            block_id = get_block_number(block_x, block_y)
            block_offset = block_id * BLOCK_SIZE
            
            if block_offset >= file_size:
                deletion_blocks.append({
                    'block_x': block_x,
                    'block_y': block_y,
                    'block_id': block_id,
                    'offset': block_offset,
                    'status': 'BEYOND_FILE_SIZE',
                    'header': None,
                    'tiles': None
                })
                continue
            
            try:
                map_file.seek(block_offset)
                
                # Read header
                header = map_file.read(4)
                header_val = struct.unpack('<I', header)[0] if len(header) == 4 else None
                
                # Read tiles
                tiles = []
                tile_data_valid = True
                for i in range(64):
                    tile_data = map_file.read(3)
                    if len(tile_data) < 3:
                        tile_data_valid = False
                        break
                    tile_id = struct.unpack('<H', tile_data[0:2])[0]
                    z = struct.unpack('b', tile_data[2:3])[0]
                    tiles.append((tile_id, z))
                
                deletion_blocks.append({
                    'block_x': block_x,
                    'block_y': block_y,
                    'block_id': block_id,
                    'offset': block_offset,
                    'status': 'VALID' if tile_data_valid else 'INCOMPLETE',
                    'header': header_val,
                    'header_bytes': header.hex() if header else None,
                    'tiles': tiles if tile_data_valid else None,
                    'tile_count': len(tiles) if tiles else 0
                })
                
            except Exception as e:
                deletion_blocks.append({
                    'block_x': block_x,
                    'block_y': block_y,
                    'block_id': block_id,
                    'offset': block_offset,
                    'status': f'ERROR: {str(e)}',
                    'header': None,
                    'tiles': None
                })
    
    # Analyze comparison area blocks
    print(f"\n--- COMPARISON AREA BLOCKS ---")
    for block_y in range(comp_min_block_y, comp_max_block_y + 1):
        for block_x in range(comp_min_block_x, comp_max_block_x + 1):
            block_id = get_block_number(block_x, block_y)
            block_offset = block_id * BLOCK_SIZE
            
            if block_offset >= file_size:
                comparison_blocks.append({
                    'block_x': block_x,
                    'block_y': block_y,
                    'block_id': block_id,
                    'offset': block_offset,
                    'status': 'BEYOND_FILE_SIZE',
                    'header': None,
                    'tiles': None
                })
                continue
            
            try:
                map_file.seek(block_offset)
                
                # Read header
                header = map_file.read(4)
                header_val = struct.unpack('<I', header)[0] if len(header) == 4 else None
                
                # Read tiles
                tiles = []
                tile_data_valid = True
                for i in range(64):
                    tile_data = map_file.read(3)
                    if len(tile_data) < 3:
                        tile_data_valid = False
                        break
                    tile_id = struct.unpack('<H', tile_data[0:2])[0]
                    z = struct.unpack('b', tile_data[2:3])[0]
                    tiles.append((tile_id, z))
                
                comparison_blocks.append({
                    'block_x': block_x,
                    'block_y': block_y,
                    'block_id': block_id,
                    'offset': block_offset,
                    'status': 'VALID' if tile_data_valid else 'INCOMPLETE',
                    'header': header_val,
                    'header_bytes': header.hex() if header else None,
                    'tiles': tiles if tile_data_valid else None,
                    'tile_count': len(tiles) if tiles else 0
                })
                
            except Exception as e:
                comparison_blocks.append({
                    'block_x': block_x,
                    'block_y': block_y,
                    'block_id': block_id,
                    'offset': block_offset,
                    'status': f'ERROR: {str(e)}',
                    'header': None,
                    'tiles': None
                })

# Print summary
print(f"\n{'='*80}")
print("SUMMARY")
print(f"{'='*80}")

print(f"\nDeletion Area:")
print(f"  Total blocks analyzed: {len(deletion_blocks)}")
valid_del = sum(1 for b in deletion_blocks if b['status'] == 'VALID')
incomplete_del = sum(1 for b in deletion_blocks if b['status'] == 'INCOMPLETE')
beyond_del = sum(1 for b in deletion_blocks if b['status'] == 'BEYOND_FILE_SIZE')
print(f"  Valid blocks: {valid_del}")
print(f"  Incomplete blocks: {incomplete_del}")
print(f"  Beyond file size: {beyond_del}")

if deletion_blocks:
    sample_del = deletion_blocks[0]
    print(f"\n  Sample deletion block ({sample_del['block_x']}, {sample_del['block_y']}):")
    print(f"    Block ID: {sample_del['block_id']}")
    print(f"    Offset: {sample_del['offset']}")
    print(f"    Status: {sample_del['status']}")
    if sample_del['header'] is not None:
        print(f"    Header value: {hex(sample_del['header'])} ({sample_del['header']})")
        print(f"    Header bytes: {sample_del['header_bytes']}")
    if sample_del['tiles']:
        print(f"    Tile count: {len(sample_del['tiles'])}")
        print(f"    First 3 tiles: {sample_del['tiles'][:3]}")

print(f"\nComparison Area:")
print(f"  Total blocks analyzed: {len(comparison_blocks)}")
valid_comp = sum(1 for b in comparison_blocks if b['status'] == 'VALID')
incomplete_comp = sum(1 for b in comparison_blocks if b['status'] == 'INCOMPLETE')
beyond_comp = sum(1 for b in comparison_blocks if b['status'] == 'BEYOND_FILE_SIZE')
print(f"  Valid blocks: {valid_comp}")
print(f"  Incomplete blocks: {incomplete_comp}")
print(f"  Beyond file size: {beyond_comp}")

if comparison_blocks:
    sample_comp = comparison_blocks[0]
    print(f"\n  Sample comparison block ({sample_comp['block_x']}, {sample_comp['block_y']}):")
    print(f"    Block ID: {sample_comp['block_id']}")
    print(f"    Offset: {sample_comp['offset']}")
    print(f"    Status: {sample_comp['status']}")
    if sample_comp['header'] is not None:
        print(f"    Header value: {hex(sample_comp['header'])} ({sample_comp['header']})")
        print(f"    Header bytes: {sample_comp['header_bytes']}")
    if sample_comp['tiles']:
        print(f"    Tile count: {len(sample_comp['tiles'])}")
        print(f"    First 3 tiles: {sample_comp['tiles'][:3]}")

# Compare headers
del_headers = [b['header'] for b in deletion_blocks if b['header'] is not None]
comp_headers = [b['header'] for b in comparison_blocks if b['header'] is not None]

print(f"\nHeader Analysis:")
print(f"  Deletion area headers:")
if del_headers:
    print(f"    Non-zero headers: {sum(1 for h in del_headers if h != 0)}")
    print(f"    Zero headers: {sum(1 for h in del_headers if h == 0)}")
    if del_headers:
        print(f"    Sample header values: {[hex(h) for h in del_headers[:5]]}")
else:
    print(f"    No valid headers found")

print(f"  Comparison area headers:")
if comp_headers:
    print(f"    Non-zero headers: {sum(1 for h in comp_headers if h != 0)}")
    print(f"    Zero headers: {sum(1 for h in comp_headers if h == 0)}")
    if comp_headers:
        print(f"    Sample header values: {[hex(h) for h in comp_headers[:5]]}")
else:
    print(f"    No valid headers found")

print(f"\n{'='*80}")

