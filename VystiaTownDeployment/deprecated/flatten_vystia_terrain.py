"""
Flatten all Vystia map terrain to a single Z-level
This will set all terrain tiles to Z=0 across the entire map
"""

import struct
from pathlib import Path

# Map dimensions
MAP_WIDTH_BLOCKS = 896   # 7168 tiles / 8
MAP_HEIGHT_BLOCKS = 512  # 4096 tiles / 8

# Target Z-level for flattened terrain
FLATTEN_Z = 0

print("="*60)
print("FLATTEN VYSTIA TERRAIN")
print("="*60)
print(f"Setting all terrain to Z={FLATTEN_Z}")
print(f"Map size: {MAP_WIDTH_BLOCKS}x{MAP_HEIGHT_BLOCKS} blocks ({MAP_WIDTH_BLOCKS*8}x{MAP_HEIGHT_BLOCKS*8} tiles)")
print()

# Source map (note: files are capitalized in UOL 1.5)
source_map = Path(r'C:\DevEnv\GIT\UO\UOL 1.5\Map0.mul')
# Target maps (write to both locations)
target_maps = [
    Path(r'C:\DevEnv\GIT\UO\ServUO-fresh\Data\Maps\map0.mul'),
    Path(r'C:\Program Files (x86)\Electronic Arts\Ultima Online Classic\map0.mul'),
]

print(f"Reading source map: {source_map}")

# Read entire map
with open(source_map, 'rb') as f:
    map_data = bytearray(f.read())

total_blocks = MAP_WIDTH_BLOCKS * MAP_HEIGHT_BLOCKS
total_cells = total_blocks * 64  # 64 cells per block (8x8)

print(f"Total blocks: {total_blocks}")
print(f"Total cells: {total_cells}")
print()
print("Flattening terrain...")

cells_modified = 0

# Each block is 196 bytes: 4 byte header + (64 cells * 3 bytes each)
# Each cell: 2 bytes tile_id + 1 byte z
for block_id in range(total_blocks):
    block_offset = block_id * 196

    # Skip 4 byte header
    cell_start = block_offset + 4

    # Process 64 cells (8x8)
    for cell in range(64):
        cell_offset = cell_start + (cell * 3)

        # Read tile_id (2 bytes)
        # Read z (1 byte at offset+2)
        current_z = struct.unpack('b', bytes([map_data[cell_offset + 2]]))[0]

        # Set z to FLATTEN_Z
        if current_z != FLATTEN_Z:
            map_data[cell_offset + 2] = FLATTEN_Z & 0xFF
            cells_modified += 1

    if block_id % 50000 == 0 and block_id > 0:
        print(f"  Progress: {block_id}/{total_blocks} blocks ({cells_modified} cells modified)...")

print(f"\nFlattened {cells_modified} cells (changed Z-level to {FLATTEN_Z})")
print()

# Write to target locations
for target_map in target_maps:
    print(f"Writing to: {target_map}")

    # Create directory if needed
    target_map.parent.mkdir(parents=True, exist_ok=True)

    with open(target_map, 'wb') as f:
        f.write(map_data)

    print(f"  Written: {len(map_data)} bytes")

print()
print("="*60)
print("TERRAIN FLATTENING COMPLETE!")
print("="*60)
print(f"All terrain set to Z={FLATTEN_Z}")
print(f"Modified {cells_modified}/{total_cells} cells ({100*cells_modified/total_cells:.1f}%)")
print()
print("IMPORTANT: Restart CentrED and ServUO to see changes")
print("IMPORTANT: You may want to backup original map0.mul files before this operation")
