"""
Fix Map File Size - Truncate map0.mul to exactly 458752 blocks
Fixes the "one block too large" error by ensuring the file is exactly the correct size.
"""

from pathlib import Path

print("="*80)
print("FIX MAP FILE SIZE")
print("="*80)

# Destination map file
dest_map = Path(r'C:\Program Files (x86)\Electronic Arts\Ultima Online Classic\map0.mul')

print(f"Map file: {dest_map}")

# Verify file exists
if not dest_map.exists():
    print(f"\nERROR: Map file not found: {dest_map}")
    exit(1)

# Calculate exact file size
MAP_WIDTH_BLOCKS = 896   # 7168 tiles / 8
MAP_HEIGHT_BLOCKS = 512  # 4096 tiles / 8
EXPECTED_BLOCKS = MAP_WIDTH_BLOCKS * MAP_HEIGHT_BLOCKS  # 458752
BLOCK_SIZE = 196  # 4 bytes header + 64 tiles * 3 bytes each
EXPECTED_FILE_SIZE = EXPECTED_BLOCKS * BLOCK_SIZE  # 89,915,392 bytes

# Get current file size
current_size = dest_map.stat().st_size
print(f"\nCurrent file size: {current_size:,} bytes")
print(f"Expected file size: {EXPECTED_FILE_SIZE:,} bytes")
print(f"Expected blocks: {EXPECTED_BLOCKS:,}")

if current_size == EXPECTED_FILE_SIZE:
    print("\nFile size appears correct, but truncating anyway to ensure exact size...")

# Calculate how many blocks are in the current file
current_blocks = current_size // BLOCK_SIZE
extra_blocks = current_blocks - EXPECTED_BLOCKS
extra_bytes = current_size - EXPECTED_FILE_SIZE

print(f"\nCurrent blocks: {current_blocks:,}")
print(f"Extra blocks: {extra_blocks}")
print(f"Extra bytes: {extra_bytes}")

if current_size < EXPECTED_FILE_SIZE:
    print(f"\nERROR: File is too small! Expected {EXPECTED_FILE_SIZE:,} bytes but got {current_size:,} bytes")
    print("This script can only truncate files, not expand them.")
    exit(1)

# Truncate file to correct size
print(f"\nTruncating file to {EXPECTED_FILE_SIZE:,} bytes...")

with open(dest_map, 'r+b') as map_file:
    # Seek to the correct position and truncate
    map_file.seek(EXPECTED_FILE_SIZE)
    map_file.truncate(EXPECTED_FILE_SIZE)

# Verify new size
new_size = dest_map.stat().st_size
print(f"New file size: {new_size:,} bytes")

# Verify block structure
print(f"\nVerifying block structure...")
import struct

with open(dest_map, 'rb') as map_file:
    # Check that we can read exactly EXPECTED_BLOCKS blocks
    blocks_read = 0
    for block_id in range(EXPECTED_BLOCKS):
        block_offset = block_id * BLOCK_SIZE
        map_file.seek(block_offset)
        
        # Try to read header (4 bytes)
        header = map_file.read(4)
        if len(header) < 4:
            print(f"ERROR: Block {block_id} is incomplete (header)")
            break
        
        # Try to read all 64 tiles
        tiles_read = 0
        for _ in range(64):
            tile_data = map_file.read(3)
            if len(tile_data) < 3:
                print(f"ERROR: Block {block_id} is incomplete (tiles)")
                break
            tiles_read += 1
        
        if tiles_read == 64:
            blocks_read += 1
    
    print(f"Successfully read {blocks_read:,} complete blocks")

if new_size == EXPECTED_FILE_SIZE and blocks_read == EXPECTED_BLOCKS:
    print("\n" + "="*80)
    print("SUCCESS: File size fixed and verified!")
    print("="*80)
    print(f"File now has exactly {EXPECTED_BLOCKS:,} blocks ({EXPECTED_FILE_SIZE:,} bytes)")
    print(f"All {blocks_read:,} blocks are complete and valid")
else:
    if new_size != EXPECTED_FILE_SIZE:
        print(f"\nERROR: File size is still incorrect. Expected {EXPECTED_FILE_SIZE:,} but got {new_size:,}")
    if blocks_read != EXPECTED_BLOCKS:
        print(f"\nERROR: Block structure is invalid. Expected {EXPECTED_BLOCKS:,} blocks but only {blocks_read:,} are complete")
    exit(1)

