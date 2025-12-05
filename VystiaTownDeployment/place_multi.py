"""
Place a UO Multi as statics
Multis are pre-built structures (houses, boats, etc) stored in multi.mul
We'll extract the multi and place it as individual statics
"""

import struct
from pathlib import Path

# Multi file locations
MULTI_MUL = Path(r'C:\Program Files (x86)\Electronic Arts\Ultima Online Classic\multi.mul')
MULTI_IDX = Path(r'C:\Program Files (x86)\Electronic Arts\Ultima Online Classic\multi.idx')

def read_multi(multi_id: int):
    """Read a multi structure from multi.mul"""

    print(f"Reading multi ID {multi_id} (0x{multi_id:04X})...")

    if not MULTI_MUL.exists() or not MULTI_IDX.exists():
        print(f"ERROR: Multi files not found!")
        print(f"  {MULTI_MUL}")
        print(f"  {MULTI_IDX}")
        return []

    # Read index entry
    with open(MULTI_IDX, 'rb') as idx:
        idx.seek(multi_id * 12)
        offset = struct.unpack('<I', idx.read(4))[0]
        length = struct.unpack('<I', idx.read(4))[0]
        extra = struct.unpack('<I', idx.read(4))[0]

    if offset == 0xFFFFFFFF or length == 0:
        print(f"Multi {multi_id} not found or empty")
        return []

    print(f"Multi offset: {offset}, length: {length}")

    # Read multi data
    components = []
    with open(MULTI_MUL, 'rb') as mul:
        mul.seek(offset)

        # Each component is 12 bytes:
        # - tile_id (2 bytes)
        # - x offset (2 bytes, signed)
        # - y offset (2 bytes, signed)
        # - z offset (2 bytes, signed)
        # - flags (4 bytes)

        component_count = length // 12
        print(f"Components: {component_count}")

        for i in range(component_count):
            tile_id = struct.unpack('<H', mul.read(2))[0]
            x = struct.unpack('<h', mul.read(2))[0]  # signed
            y = struct.unpack('<h', mul.read(2))[0]  # signed
            z = struct.unpack('<h', mul.read(2))[0]  # signed
            flags = struct.unpack('<I', mul.read(4))[0]

            # Filter out invalid tile IDs (terminators/placeholders)
            # Valid tile IDs are typically 0x0001 to 0x4000 (or so)
            if tile_id == 0 or tile_id >= 0xFFFD:
                continue  # Skip invalid tiles

            # Also skip tiles with impossible coordinates
            if abs(x) > 100 or abs(y) > 100 or abs(z) > 100:
                continue

            components.append({
                'tile_id': tile_id,
                'x': x,
                'y': y,
                'z': z,
                'flags': flags
            })

    return components

def place_multi_as_statics(multi_id: int, world_x: int, world_y: int, world_z: int = 0):
    """Place a multi at world coordinates as individual statics"""

    print(f"\n{'='*60}")
    print(f"PLACING MULTI AS STATICS")
    print(f"{'='*60}")
    print(f"Multi ID: {multi_id} (0x{multi_id:04X})")
    print(f"Location: ({world_x}, {world_y}, {world_z})")
    print()

    # Read multi components
    components = read_multi(multi_id)

    if not components:
        print("No components found!")
        return []

    # Convert to world statics
    statics = []
    for comp in components:
        statics.append({
            'tile_id': comp['tile_id'],
            'x': world_x + comp['x'],
            'y': world_y + comp['y'],
            'z': world_z + comp['z'],
            'hue': 0
        })

    print(f"Created {len(statics)} statics from multi")

    # Show some sample components
    print(f"\nFirst 10 components:")
    for i, comp in enumerate(components[:10]):
        print(f"  {i}: Tile 0x{comp['tile_id']:04X} at ({comp['x']:+3d}, {comp['y']:+3d}, {comp['z']:+3d})")

    return statics

if __name__ == '__main__':
    import json
    from deploy_exact_building import deploy_to_mul
    import shutil

    # Place multi 0x64 (small house) at (1750, 800)
    statics = place_multi_as_statics(
        multi_id=0x64,
        world_x=1750,
        world_y=800,
        world_z=0
    )

    if statics:
        # Save JSON
        with open('multi_0x64.json', 'w') as f:
            json.dump({'multi_id': 0x64, 'statics': statics}, f, indent=2)

        # Deploy
        target_statics = Path(r'C:\Program Files (x86)\Electronic Arts\Ultima Online Classic\statics0.mul')
        target_staidx = Path(r'C:\Program Files (x86)\Electronic Arts\Ultima Online Classic\staidx0.mul')

        deploy_to_mul(statics, target_statics, target_staidx)

        # Copy to CentrED
        print(f"\nCopying to CentrED...")
        shutil.copy(target_statics, r'C:\DevEnv\GIT\UO\ServUO-fresh\Data\Maps\statics0.mul')
        shutil.copy(target_staidx, r'C:\DevEnv\GIT\UO\ServUO-fresh\Data\Maps\staidx0.mul')

        print(f"\nDone! Multi 0x64 placed at (1750, 800)")
