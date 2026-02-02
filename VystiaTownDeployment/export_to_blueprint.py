"""
Export a region from OSI map0.mul to CentrED# Blueprint (.txt) format

Usage:
    python export_to_blueprint.py britain
    python export_to_blueprint.py deepforge
    python export_to_blueprint.py custom --x1 1270 --y1 1400 --x2 1750 --y2 1800 --name MyArea

Output will be placed in CentrED#'s Blueprints folder for immediate use.
"""

import struct
import argparse
from pathlib import Path
from collections import defaultdict

# Map dimensions
MAP_WIDTH_BLOCKS = 896   # 7168 tiles / 8
MAP_HEIGHT_BLOCKS = 512  # 4096 tiles / 8

def get_block_number(block_x: int, block_y: int) -> int:
    return block_x * MAP_HEIGHT_BLOCKS + block_y

# Source: OSI map files
OSI_MAP = Path(r'C:\Program Files (x86)\Electronic Arts\Ultima Online Classic\map0.mul')
OSI_STATICS = Path(r'C:\Program Files (x86)\Electronic Arts\Ultima Online Classic\statics0.mul')
OSI_STAIDX = Path(r'C:\Program Files (x86)\Electronic Arts\Ultima Online Classic\staidx0.mul')

# Alternative source for Deepforge (UO Adventures has different dungeon)
ALT_MAP = Path(r'D:\UO\UO Adventures\Client\Data Files\map0.mul')
ALT_STATICS = Path(r'D:\UO\UO Adventures\Client\Data Files\statics0.mul')
ALT_STAIDX = Path(r'D:\UO\UO Adventures\Client\Data Files\staidx0.mul')

# Output folder
BLUEPRINT_FOLDER = Path(r'D:\UO\centredsharp\Blueprints\Vystia')

# Predefined regions
REGIONS = {
    'britain': {
        'name': 'Britain',
        'x1': 1270, 'y1': 1400, 'x2': 1750, 'y2': 1800,
        'source': 'osi'
    },
    'deepforge': {
        'name': 'Deepforge_DwarfEntry',
        'x1': 1305, 'y1': 318, 'x2': 1413, 'y2': 429,
        'source': 'alt'  # UO Adventures has this dungeon
    },
    'minoc': {
        'name': 'Minoc',
        'x1': 2411, 'y1': 366, 'x2': 2560, 'y2': 540,
        'source': 'osi'
    },
    'vesper': {
        'name': 'Vesper',
        'x1': 2715, 'y1': 590, 'x2': 3000, 'y2': 1100,
        'source': 'osi'
    },
    'trinsic': {
        'name': 'Trinsic',
        'x1': 1820, 'y1': 2640, 'x2': 2140, 'y2': 2910,
        'source': 'osi'
    },
    'yew': {
        'name': 'Yew',
        'x1': 450, 'y1': 850, 'x2': 700, 'y2': 1200,
        'source': 'osi'
    },
    'moonglow': {
        'name': 'Moonglow',
        'x1': 4390, 'y1': 1030, 'x2': 4620, 'y2': 1340,
        'source': 'osi'
    }
}

def extract_statics(x1, y1, x2, y2, source='osi'):
    """Extract statics from a region"""

    if source == 'alt':
        statics_path = ALT_STATICS
        staidx_path = ALT_STAIDX
    else:
        statics_path = OSI_STATICS
        staidx_path = OSI_STAIDX

    static_items = []

    # Calculate center for relative coordinates
    center_x = (x1 + x2) // 2
    center_y = (y1 + y2) // 2

    # Calculate source block range
    source_block_x1 = x1 // 8
    source_block_y1 = y1 // 8
    source_block_x2 = x2 // 8
    source_block_y2 = y2 // 8

    with open(staidx_path, 'rb') as idx, open(statics_path, 'rb') as sta:
        for block_y in range(source_block_y1, source_block_y2 + 1):
            for block_x in range(source_block_x1, source_block_x2 + 1):
                block_id = get_block_number(block_x, block_y)

                idx.seek(block_id * 12)
                offset = struct.unpack('<I', idx.read(4))[0]
                length = struct.unpack('<I', idx.read(4))[0]

                if offset == 0xFFFFFFFF or length == 0:
                    continue

                sta.seek(offset)
                count = length // 7

                for _ in range(count):
                    tile_id = struct.unpack('<H', sta.read(2))[0]
                    local_x = struct.unpack('B', sta.read(1))[0]
                    local_y = struct.unpack('B', sta.read(1))[0]
                    z = struct.unpack('b', sta.read(1))[0]
                    hue = struct.unpack('<H', sta.read(2))[0]

                    # Calculate world coordinates
                    world_x = block_x * 8 + local_x
                    world_y = block_y * 8 + local_y

                    # Only include statics within bounds
                    if x1 <= world_x <= x2 and y1 <= world_y <= y2:
                        # Convert to relative coordinates (centered at 0,0)
                        rel_x = world_x - center_x
                        rel_y = world_y - center_y

                        static_items.append({
                            'tile_id': tile_id,
                            'x': rel_x,
                            'y': rel_y,
                            'z': z,
                            'hue': hue
                        })

    return static_items, center_x, center_y

def export_blueprint(name, statics):
    """Export statics to UO Architect .txt format"""

    BLUEPRINT_FOLDER.mkdir(parents=True, exist_ok=True)
    output_path = BLUEPRINT_FOLDER / f'{name}.txt'

    with open(output_path, 'w') as f:
        # Header (UO Architect format)
        f.write('6 version\n')
        f.write('0 template id\n')
        f.write('0 item version\n')
        f.write(f'{len(statics)} num components\n')

        # Sort by Z, then Y, then X for better organization
        sorted_statics = sorted(statics, key=lambda s: (s['z'], s['y'], s['x']))

        for item in sorted_statics:
            # Format: tile_id x y z flags
            f.write(f"{item['tile_id']} {item['x']} {item['y']} {item['z']} 1\n")

    return output_path

def main():
    parser = argparse.ArgumentParser(description='Export OSI map region to CentrED# Blueprint')
    parser.add_argument('region', nargs='?', help='Predefined region name or "custom"')
    parser.add_argument('--x1', type=int, help='Source X1')
    parser.add_argument('--y1', type=int, help='Source Y1')
    parser.add_argument('--x2', type=int, help='Source X2')
    parser.add_argument('--y2', type=int, help='Source Y2')
    parser.add_argument('--name', type=str, help='Blueprint name')
    parser.add_argument('--source', type=str, default='osi', help='Source: osi or alt')
    parser.add_argument('--list', action='store_true', help='List available regions')

    args = parser.parse_args()

    if args.list or not args.region:
        print("Available predefined regions:")
        for key, info in REGIONS.items():
            print(f"  {key}: {info['name']} ({info['x1']},{info['y1']}) to ({info['x2']},{info['y2']})")
        print("\nUsage:")
        print("  python export_to_blueprint.py britain")
        print("  python export_to_blueprint.py deepforge")
        print("  python export_to_blueprint.py custom --x1 1270 --y1 1400 --x2 1750 --y2 1800 --name MyArea")
        return

    if args.region == 'custom':
        if not all([args.x1, args.y1, args.x2, args.y2, args.name]):
            print("Custom region requires --x1, --y1, --x2, --y2, and --name")
            return
        x1, y1, x2, y2 = args.x1, args.y1, args.x2, args.y2
        name = args.name
        source = args.source
    elif args.region in REGIONS:
        region = REGIONS[args.region]
        x1, y1, x2, y2 = region['x1'], region['y1'], region['x2'], region['y2']
        name = region['name']
        source = region['source']
    else:
        print(f"Unknown region: {args.region}")
        print("Use --list to see available regions")
        return

    print("="*80)
    print(f"EXPORTING {name} TO CENTRED# BLUEPRINT")
    print("="*80)
    print(f"Source: ({x1}, {y1}) to ({x2}, {y2})")

    print("\n[1/2] Extracting statics...")
    statics, center_x, center_y = extract_statics(x1, y1, x2, y2, source)
    print(f"  Extracted {len(statics)} static tiles")
    print(f"  Center point: ({center_x}, {center_y})")

    print("\n[2/2] Writing blueprint...")
    output_path = export_blueprint(name, statics)
    print(f"  Wrote: {output_path}")

    print("\n" + "="*80)
    print("EXPORT COMPLETE!")
    print("="*80)
    print(f"\nBlueprint saved to: {output_path}")
    print("\nTo use in CentrED#:")
    print("  1. Open CentrED#")
    print("  2. Go to Tools -> Blueprints")
    print(f"  3. Find 'Vystia/{name}' in the tree")
    print("  4. Click to load, then click on map to place")

if __name__ == '__main__':
    main()
