"""
Extract Multi 0x64 properly by reading multi.mul directly
Based on ServUO's MultiData.cs format
"""

import struct
from pathlib import Path

MULTI_MUL = Path(r'C:\Program Files (x86)\Electronic Arts\Ultima Online Classic\multi.mul')
MULTI_IDX = Path(r'C:\Program Files (x86)\Electronic Arts\Ultima Online Classic\multi.idx')

def extract_multi_servuo_style(multi_id: int, output_file: str):
    """Extract multi using ServUO's format"""

    print(f"Extracting Multi 0x{multi_id:04X} (ServUO style)...")

    # Read index
    with open(MULTI_IDX, 'rb') as idx:
        idx.seek(multi_id * 12)
        offset = struct.unpack('<I', idx.read(4))[0]
        length = struct.unpack('<I', idx.read(4))[0]

    if offset == 0xFFFFFFFF or length == 0:
        print(f"Multi {multi_id} not found")
        return []

    print(f"Offset: {offset}, Length: {length}, Components: {length // 12}")

    # Read components (ServUO format: ItemID, OffsetX, OffsetY, OffsetZ, Flags)
    components = []
    with open(MULTI_MUL, 'rb') as mul:
        mul.seek(offset)

        for i in range(length // 12):
            item_id = struct.unpack('<H', mul.read(2))[0]
            offset_x = struct.unpack('<h', mul.read(2))[0]
            offset_y = struct.unpack('<h', mul.read(2))[0]
            offset_z = struct.unpack('<h', mul.read(2))[0]
            flags = struct.unpack('<I', mul.read(4))[0]

            # ServUO filters:
            # - Skip if ItemID is out of bounds
            # - Skip if it's a "background" flag component (these are visual markers only)

            # For now, let's keep everything that's not obviously invalid
            if item_id == 0 or item_id >= 0xFFFD:
                continue  # Skip terminators

            # Skip insane coordinates
            if abs(offset_x) > 100 or abs(offset_y) > 100 or abs(offset_z) > 127:
                continue

            components.append({
                'item_id': item_id,
                'x': offset_x,
                'y': offset_y,
                'z': offset_z,
                'hue': 0,
                'flags': flags
            })

    print(f"Valid components: {len(components)}")

    # Save to file
    with open(output_file, 'w') as f:
        for comp in components:
            f.write(f"0x{comp['item_id']:X} {comp['x']} {comp['y']} {comp['z']} {comp['hue']}\n")

    print(f"Saved to: {output_file}")

    # Show some samples
    print(f"\nFirst 20 components:")
    for i, comp in enumerate(components[:20]):
        print(f"  {i}: 0x{comp['item_id']:04X} at ({comp['x']:+3d}, {comp['y']:+3d}, {comp['z']:+3d}) flags=0x{comp['flags']:08X}")

    # Show tile ID distribution
    from collections import Counter
    tile_counts = Counter([c['item_id'] for c in components])
    print(f"\nMost common tile IDs:")
    for tile_id, count in tile_counts.most_common(10):
        print(f"  0x{tile_id:04X}: {count} times")

    return components

if __name__ == '__main__':
    extract_multi_servuo_style(
        multi_id=0x64,
        output_file=r'C:\DevEnv\GIT\UO\Vystia Town Generator\Multi_0x64_servuo.txt'
    )
