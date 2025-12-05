"""
Place tiles using CentrED's exact approach:
- Modify blocks IN-PLACE (don't rewrite entire file)
- Read existing index, append if needed
- Use exact same formulas as CentrED
"""

import struct
from pathlib import Path
from typing import List, Dict

MAP_WIDTH_BLOCKS = 896
MAP_HEIGHT_BLOCKS = 512

def get_block_number(block_x: int, block_y: int) -> int:
    """CentrED's exact formula: x * Height + y"""
    return block_x * MAP_HEIGHT_BLOCKS + block_y

def get_staidx_offset(block_x: int, block_y: int) -> int:
    """Offset in staidx.mul for this block"""
    return get_block_number(block_x, block_y) * 12

class CentrEDStylePlacer:
    """Places tiles exactly like CentrED does"""

    def __init__(self, statics_path: Path, staidx_path: Path):
        self.statics_path = statics_path
        self.staidx_path = staidx_path

    def add_tiles_to_block(self, block_x: int, block_y: int, tiles: List[Dict]):
        """
        Add tiles to a specific block using CentrED's method
        tiles: list of {'tile_id', 'x', 'y', 'z', 'hue'} where x,y are LOCAL (0-7)
        """

        print(f"Adding {len(tiles)} tiles to block ({block_x}, {block_y})")

        # Open files for read/write
        with open(self.staidx_path, 'r+b') as staidx_file, \
             open(self.statics_path, 'r+b') as statics_file:

            # Read current index for this block
            idx_offset = get_staidx_offset(block_x, block_y)
            staidx_file.seek(idx_offset)

            current_offset = struct.unpack('<I', staidx_file.read(4))[0]
            current_length = struct.unpack('<I', staidx_file.read(4))[0]
            extra = struct.unpack('<I', staidx_file.read(4))[0]

            print(f"  Current: offset={current_offset}, length={current_length}")

            # Read existing tiles from this block
            existing_tiles = []
            if current_offset != 0xFFFFFFFF and current_length > 0:
                statics_file.seek(current_offset)
                count = current_length // 7
                for _ in range(count):
                    tile_id = struct.unpack('<H', statics_file.read(2))[0]
                    x = struct.unpack('B', statics_file.read(1))[0]
                    y = struct.unpack('B', statics_file.read(1))[0]
                    z = struct.unpack('b', statics_file.read(1))[0]
                    hue = struct.unpack('<H', statics_file.read(2))[0]
                    existing_tiles.append({'tile_id': tile_id, 'x': x, 'y': y, 'z': z, 'hue': hue})

            print(f"  Existing tiles: {len(existing_tiles)}")

            # Combine existing + new tiles
            all_tiles = existing_tiles + tiles
            new_length = len(all_tiles) * 7

            print(f"  New total: {len(all_tiles)} tiles, {new_length} bytes")

            # CentrED logic: if new data is bigger OR no existing space, append to end
            if new_length > current_length or current_offset <= 0 or current_offset == 0xFFFFFFFF:
                # Append to end of statics file
                statics_file.seek(0, 2)  # Seek to end
                new_offset = statics_file.tell()
                print(f"  Appending at end: offset={new_offset}")
            else:
                # Reuse existing space
                new_offset = current_offset
                print(f"  Reusing existing space: offset={new_offset}")

            # Write statics
            statics_file.seek(new_offset)
            for tile in all_tiles:
                statics_file.write(struct.pack('<H', tile['tile_id']))
                statics_file.write(struct.pack('B', tile['x']))
                statics_file.write(struct.pack('B', tile['y']))
                statics_file.write(struct.pack('b', tile['z']))
                statics_file.write(struct.pack('<H', tile['hue']))

            # Update index
            staidx_file.seek(idx_offset)
            if new_length == 0:
                # Empty block
                staidx_file.write(struct.pack('<I', 0xFFFFFFFF))
                staidx_file.write(struct.pack('<I', 0))
            else:
                staidx_file.write(struct.pack('<I', new_offset))
                staidx_file.write(struct.pack('<I', new_length))
            staidx_file.write(struct.pack('<I', 0))

            print(f"  Block saved OK")

def place_simple_test_structure(center_x: int, center_y: int):
    """Place a simple 3x3 test structure"""

    print(f"\n{'='*60}")
    print(f"PLACING TEST STRUCTURE (CentrED Style)")
    print(f"{'='*60}")
    print(f"Center: ({center_x}, {center_y})")
    print()

    placer = CentrEDStylePlacer(
        statics_path=Path(r'C:\Program Files (x86)\Electronic Arts\Ultima Online Classic\statics0.mul'),
        staidx_path=Path(r'C:\Program Files (x86)\Electronic Arts\Ultima Online Classic\staidx0.mul')
    )

    # Create a simple 3x3 stone floor
    # Tile 0x0495 = wooden planks
    # Tile 0x0509 = stone floor

    tiles_to_place = []
    for dx in range(-1, 2):  # -1, 0, 1
        for dy in range(-1, 2):
            world_x = center_x + dx
            world_y = center_y + dy

            # Calculate block and local coords
            block_x = world_x // 8
            block_y = world_y // 8
            local_x = world_x & 0x7
            local_y = world_y & 0x7

            tiles_to_place.append({
                'block_x': block_x,
                'block_y': block_y,
                'tile': {
                    'tile_id': 0x0509,  # Stone floor
                    'x': local_x,
                    'y': local_y,
                    'z': 0,
                    'hue': 0
                }
            })

    # Group by block
    blocks = {}
    for item in tiles_to_place:
        block_key = (item['block_x'], item['block_y'])
        if block_key not in blocks:
            blocks[block_key] = []
        blocks[block_key].append(item['tile'])

    print(f"Placing tiles across {len(blocks)} blocks")
    print()

    # Place each block
    for (block_x, block_y), tiles in blocks.items():
        placer.add_tiles_to_block(block_x, block_y, tiles)

    print()
    print(f"{'='*60}")
    print(f"PLACEMENT COMPLETE!")
    print(f"{'='*60}")
    print(f"Placed {len(tiles_to_place)} tiles in 3x3 pattern")
    print(f"Check CentrED at ({center_x}, {center_y})")

if __name__ == '__main__':
    import shutil

    # Copy files to CentrED location after placement
    source_statics = Path(r'C:\Program Files (x86)\Electronic Arts\Ultima Online Classic\statics0.mul')
    source_staidx = Path(r'C:\Program Files (x86)\Electronic Arts\Ultima Online Classic\staidx0.mul')

    target_statics = Path(r'C:\DevEnv\GIT\UO\ServUO-fresh\Data\Maps\statics0.mul')
    target_staidx = Path(r'C:\DevEnv\GIT\UO\ServUO-fresh\Data\Maps\staidx0.mul')

    # Place test structure
    place_simple_test_structure(1750, 800)

    # Copy to CentrED
    print(f"\nCopying to CentrED location...")
    shutil.copy(source_statics, target_statics)
    shutil.copy(source_staidx, target_staidx)

    print(f"\nDone! Check CentrED at (1750, 800)")
