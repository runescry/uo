"""
Deploy an exact building from Britain with NO modifications
Just copy it exactly as-is to see if our deployment system works
"""

import struct
import json
from pathlib import Path
from typing import List, Dict

MAP_WIDTH_BLOCKS = 896
MAP_HEIGHT_BLOCKS = 512

def get_block_number(block_x: int, block_y: int) -> int:
    return block_x * MAP_HEIGHT_BLOCKS + block_y

def deploy_exact_building(source_file: str, target_x: int, target_y: int):
    """Deploy building from JSON file to target coordinates"""

    with open(source_file, 'r') as f:
        data = json.load(f)

    source_statics = data['statics']

    print(f"\n{'='*60}")
    print(f"DEPLOYING EXACT BUILDING")
    print(f"{'='*60}")
    print(f"Source: {source_file}")
    print(f"Statics: {len(source_statics)}")

    # Calculate source center
    x_coords = [s['x'] for s in source_statics]
    y_coords = [s['y'] for s in source_statics]

    source_center_x = (min(x_coords) + max(x_coords)) // 2
    source_center_y = (min(y_coords) + max(y_coords)) // 2

    shift_x = target_x - source_center_x
    shift_y = target_y - source_center_y

    print(f"Source center: ({source_center_x}, {source_center_y})")
    print(f"Target: ({target_x}, {target_y})")
    print(f"Shift: ({shift_x}, {shift_y})")

    # Shift statics to new location
    # KEEP ORIGINAL Z-LEVELS - don't adjust!
    # But Vystia terrain is at Z=0, Britain was at Z=-5 for terrain
    # So we need to add +5 to all Z to account for terrain difference
    Z_ADJUST = 5

    statics = []
    for s in source_statics:
        new_z = s['z'] + Z_ADJUST
        if new_z < -128:
            new_z = -128
        elif new_z > 127:
            new_z = 127

        statics.append({
            'tile_id': s['tile_id'],
            'x': s['x'] + shift_x,
            'y': s['y'] + shift_y,
            'z': new_z,
            'hue': s.get('hue', 0)
        })

    print(f"Z-adjustment: +{Z_ADJUST} (Britain terrain Z=-5, Vystia Z=0)")

    return statics

def deploy_to_mul(statics: List[Dict], target_statics: Path, target_staidx: Path):
    """Deploy to MUL files"""
    print(f"\n{'='*60}")
    print(f"DEPLOYING TO MUL FILES")
    print(f"{'='*60}")

    # Load existing
    vystia_blocks = {}
    with open(target_staidx, 'rb') as idx, open(target_statics, 'rb') as sta:
        for i in range(MAP_WIDTH_BLOCKS * MAP_HEIGHT_BLOCKS):
            idx.seek(i * 12)
            offset = struct.unpack('<I', idx.read(4))[0]
            length = struct.unpack('<I', idx.read(4))[0]

            if offset == 0xFFFFFFFF or length == 0:
                continue

            sta.seek(offset)
            count = length // 7

            vystia_blocks[i] = []
            for _ in range(count):
                tile_id = struct.unpack('<H', sta.read(2))[0]
                x = struct.unpack('B', sta.read(1))[0]
                y = struct.unpack('B', sta.read(1))[0]
                z = struct.unpack('b', sta.read(1))[0]
                hue = struct.unpack('<H', sta.read(2))[0]
                vystia_blocks[i].append({'tile_id': tile_id, 'x': x, 'y': y, 'z': z, 'hue': hue})

    print(f"Loaded {len(vystia_blocks)} existing blocks")

    # Group new statics
    new_blocks = {}
    for static in statics:
        block_x = static['x'] // 8
        block_y = static['y'] // 8
        local_x = static['x'] & 0x7
        local_y = static['y'] & 0x7
        block_id = get_block_number(block_x, block_y)

        if block_id not in new_blocks:
            new_blocks[block_id] = []

        new_blocks[block_id].append({
            'tile_id': static['tile_id'],
            'x': local_x,
            'y': local_y,
            'z': static['z'],
            'hue': static['hue']
        })

    print(f"Grouped into {len(new_blocks)} blocks")

    # Merge
    for block_id, statics_list in new_blocks.items():
        if block_id in vystia_blocks:
            vystia_blocks[block_id].extend(statics_list)
        else:
            vystia_blocks[block_id] = statics_list

    # Write
    print(f"Writing to files...")
    with open(target_statics, 'wb') as sta, open(target_staidx, 'wb') as idx:
        current_offset = 0

        for block_id in range(MAP_WIDTH_BLOCKS * MAP_HEIGHT_BLOCKS):
            if block_id in vystia_blocks and len(vystia_blocks[block_id]) > 0:
                statics_list = vystia_blocks[block_id]

                for s in statics_list:
                    sta.write(struct.pack('<H', s['tile_id']))
                    sta.write(struct.pack('B', s['x']))
                    sta.write(struct.pack('B', s['y']))
                    sta.write(struct.pack('b', s['z']))
                    sta.write(struct.pack('<H', s['hue']))

                length = len(statics_list) * 7
                idx.write(struct.pack('<I', current_offset))
                idx.write(struct.pack('<I', length))
                idx.write(struct.pack('<I', 0))
                current_offset += length
            else:
                idx.write(struct.pack('<I', 0xFFFFFFFF))
                idx.write(struct.pack('<I', 0xFFFFFFFF))
                idx.write(struct.pack('<I', 0))

            if block_id % 100000 == 0 and block_id > 0:
                print(f"  Progress: {block_id}/{MAP_WIDTH_BLOCKS * MAP_HEIGHT_BLOCKS}...")

    print(f"\n{'='*60}")
    print(f"DEPLOYMENT COMPLETE!")
    print(f"{'='*60}")

if __name__ == '__main__':
    import shutil

    # Deploy Britain bank area to (1750, 800)
    statics = deploy_exact_building(
        source_file='britain_bank_area.json',
        target_x=1750,
        target_y=800
    )

    # Deploy
    target_statics = Path(r'C:\Program Files (x86)\Electronic Arts\Ultima Online Classic\statics0.mul')
    target_staidx = Path(r'C:\Program Files (x86)\Electronic Arts\Ultima Online Classic\staidx0.mul')

    deploy_to_mul(statics, target_statics, target_staidx)

    # Copy to CentrED
    print(f"\nCopying to CentrED...")
    shutil.copy(target_statics, r'C:\DevEnv\GIT\UO\ServUO-fresh\Data\Maps\statics0.mul')
    shutil.copy(target_staidx, r'C:\DevEnv\GIT\UO\ServUO-fresh\Data\Maps\staidx0.mul')

    print(f"\nDone! Britain bank area deployed at (1750, 800)")
    print(f"This includes {len(statics)} statics - walls, roofs, decorations, all intact")
