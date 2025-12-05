"""
Check Backup Headers - Compare headers in backup vs current file
"""

import struct
from pathlib import Path

MAP_WIDTH_BLOCKS = 896
MAP_HEIGHT_BLOCKS = 512

def get_block_number(block_x: int, block_y: int) -> int:
    return block_x * MAP_HEIGHT_BLOCKS + block_y

backup_map = Path(r'Vystia Town Generator/backups/delete_area_backup_20251119_173651/map0.mul')
current_map = Path(r'C:\Program Files (x86)\Electronic Arts\Ultima Online Classic\map0.mul')

# Check a few blocks in deletion area
del_x1, del_y1 = 3477, 1904
del_x2, del_y2 = 3892, 2324

min_block_x = del_x1 // 8
max_block_x = min(del_x1 // 8 + 5, del_x2 // 8)  # Check first 5 blocks
min_block_y = del_y1 // 8
max_block_y = min(del_y1 // 8 + 5, del_y2 // 8)

print("Checking backup vs current file headers...")
print(f"Backup: {backup_map}")
print(f"Current: {current_map}")

BLOCK_SIZE = 196

with open(backup_map, 'rb') as backup_file, open(current_map, 'rb') as current_file:
    for block_y in range(min_block_y, min(max_block_y + 1, min_block_y + 3)):
        for block_x in range(min_block_x, min(max_block_x + 1, min_block_x + 3)):
            block_id = get_block_number(block_x, block_y)
            offset = block_id * BLOCK_SIZE
            
            # Read from backup
            backup_file.seek(offset)
            backup_header = backup_file.read(4)
            backup_header_val = struct.unpack('<I', backup_header)[0] if len(backup_header) == 4 else None
            
            # Read from current
            current_file.seek(offset)
            current_header = current_file.read(4)
            current_header_val = struct.unpack('<I', current_header)[0] if len(current_header) == 4 else None
            
            print(f"\nBlock ({block_x}, {block_y}) ID {block_id}:")
            print(f"  Backup header:   {backup_header.hex()} = {backup_header_val} ({hex(backup_header_val) if backup_header_val else 'None'})")
            print(f"  Current header:  {current_header.hex()} = {current_header_val} ({hex(current_header_val) if current_header_val else 'None'})")
            
            if backup_header_val != current_header_val:
                print(f"  *** HEADER MISMATCH ***")
            elif backup_header_val == block_id:
                print(f"  *** WARNING: Header matches block ID (suspicious) ***")

