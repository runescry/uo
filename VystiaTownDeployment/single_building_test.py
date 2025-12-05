"""
Single Building Test - Place one complete OSI building
"""

import struct
import json
from pathlib import Path
from typing import List, Dict

# Map dimensions
MAP_WIDTH_BLOCKS = 896
MAP_HEIGHT_BLOCKS = 512

def get_block_number(block_x: int, block_y: int) -> int:
    """CentrED's block calculation"""
    return block_x * MAP_HEIGHT_BLOCKS + block_y

def place_single_building(center_x: int, center_y: int, template_id: int = 0):
    """Place a single OSI building at specified location"""

    # Load FILTERED templates (buildings only, no decorations)
    template_file = Path('building_templates_filtered.json')
    with open(template_file, 'r') as f:
        data = json.load(f)
        templates = data['templates']

    if template_id >= len(templates):
        print(f"ERROR: Template {template_id} not found")
        return []

    template = templates[template_id]

    print(f"\n{'='*60}")
    print(f"PLACING SINGLE BUILDING")
    print(f"{'='*60}")
    print(f"Template ID: {template_id}")
    print(f"Source: {template['source_town']}")
    print(f"Size: {template['size']} statics")
    print(f"Location: ({center_x}, {center_y})")
    print()

    # Calculate building bounds to center it
    statics_data = template['statics']
    x_coords = [s['x'] for s in statics_data]
    y_coords = [s['y'] for s in statics_data]

    width = max(x_coords) - min(x_coords)
    height = max(y_coords) - min(y_coords)

    print(f"Building dimensions: {width}x{height} tiles")

    # Offset to center the building
    offset_x = center_x - width // 2
    offset_y = center_y - height // 2

    statics = []
    for static in statics_data:
        # Adjust Z-level: OSI Britain was at Z=-5, Vystia is at Z=0
        adjusted_z = static['z'] + 5

        # Clamp to valid range
        if adjusted_z < -128:
            adjusted_z = -128
        elif adjusted_z > 127:
            adjusted_z = 127

        statics.append({
            'tile_id': static['tile_id'],
            'x': offset_x + static['x'],
            'y': offset_y + static['y'],
            'z': adjusted_z,
            'hue': static.get('hue', 0)
        })

    print(f"Building positioned at ({offset_x}, {offset_y})")
    print(f"Total statics: {len(statics)}")

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

    # Place ONE building at (1750, 800)
    # Use template 1 which is 100% roof tiles (pure building)
    statics = place_single_building(
        center_x=1750,
        center_y=800,
        template_id=1  # Pure building template (was template 1 before filtering)
    )

    # Save JSON
    with open('single_building.json', 'w') as f:
        json.dump({'statics': statics}, f, indent=2)

    # Deploy
    target_statics = Path(r'C:\Program Files (x86)\Electronic Arts\Ultima Online Classic\statics0.mul')
    target_staidx = Path(r'C:\Program Files (x86)\Electronic Arts\Ultima Online Classic\staidx0.mul')

    deploy_to_mul(statics, target_statics, target_staidx)

    # Copy to CentrED
    print(f"\nCopying to CentrED...")
    shutil.copy(target_statics, r'C:\DevEnv\GIT\UO\ServUO-fresh\Data\Maps\statics0.mul')
    shutil.copy(target_staidx, r'C:\DevEnv\GIT\UO\ServUO-fresh\Data\Maps\staidx0.mul')

    print(f"\nDone! Single building at (1750, 800)")
