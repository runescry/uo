"""
Place a multi from text file using CentrED-style placement
"""

from pathlib import Path
import sys
sys.path.append(str(Path(__file__).parent))
from centred_style_placer import CentrEDStylePlacer

def load_multi_from_file(filepath: str):
    """Load multi from text file format: TileID X Y Z Hue"""

    print(f"Loading multi from: {filepath}")

    components = []
    with open(filepath, 'r') as f:
        for line_num, line in enumerate(f, 1):
            line = line.strip()
            if not line:
                continue

            parts = line.split()
            if len(parts) != 5:
                print(f"Warning: Line {line_num} has {len(parts)} parts (expected 5): {line}")
                continue

            try:
                tile_id = int(parts[0], 16)  # Hex format
                x = int(parts[1])
                y = int(parts[2])
                z = int(parts[3])
                hue = int(parts[4])

                components.append({
                    'tile_id': tile_id,
                    'x': x,
                    'y': y,
                    'z': z,
                    'hue': hue
                })
            except ValueError as e:
                print(f"Warning: Line {line_num} parse error: {e}: {line}")
                continue

    print(f"Loaded {len(components)} components")
    return components

def place_multi_centred_style(multi_file: str, world_x: int, world_y: int, world_z: int = 0):
    """Place multi from file using CentrED approach"""

    print(f"\n{'='*60}")
    print(f"PLACING MULTI (CentrED Style)")
    print(f"{'='*60}")
    print(f"Multi file: {multi_file}")
    print(f"World position: ({world_x}, {world_y}, {world_z})")
    print()

    # Load multi
    components = load_multi_from_file(multi_file)

    if not components:
        print("ERROR: No components loaded!")
        return

    # Create placer
    placer = CentrEDStylePlacer(
        statics_path=Path(r'C:\Program Files (x86)\Electronic Arts\Ultima Online Classic\statics0.mul'),
        staidx_path=Path(r'C:\Program Files (x86)\Electronic Arts\Ultima Online Classic\staidx0.mul')
    )

    # Convert components to world coordinates and group by block
    blocks = {}
    for comp in components:
        # World coordinates
        wx = world_x + comp['x']
        wy = world_y + comp['y']
        wz = world_z + comp['z']

        # Block coordinates
        block_x = wx // 8
        block_y = wy // 8

        # Local coordinates
        local_x = wx & 0x7
        local_y = wy & 0x7

        block_key = (block_x, block_y)
        if block_key not in blocks:
            blocks[block_key] = []

        blocks[block_key].append({
            'tile_id': comp['tile_id'],
            'x': local_x,
            'y': local_y,
            'z': wz,
            'hue': comp['hue']
        })

    print(f"Multi spans {len(blocks)} blocks")
    print()

    # Place each block
    for i, ((block_x, block_y), tiles) in enumerate(blocks.items(), 1):
        print(f"[{i}/{len(blocks)}] ", end='')
        placer.add_tiles_to_block(block_x, block_y, tiles)

    print()
    print(f"{'='*60}")
    print(f"PLACEMENT COMPLETE!")
    print(f"{'='*60}")
    print(f"Multi 0x81 placed at ({world_x}, {world_y}, {world_z})")
    print(f"Total components: {len(components)}")
    print(f"Check CentrED at ({world_x}, {world_y})")

if __name__ == '__main__':
    import shutil

    # Place Multi 0x64 (real multi from multi.mul) at (1750, 800)
    place_multi_centred_style(
        multi_file=r'C:\DevEnv\GIT\UO\Vystia Town Generator\Multi_0x64_extracted.txt',
        world_x=1750,
        world_y=800,
        world_z=0
    )

    # Copy to CentrED
    print(f"\nCopying to CentrED location...")
    source_statics = Path(r'C:\Program Files (x86)\Electronic Arts\Ultima Online Classic\statics0.mul')
    source_staidx = Path(r'C:\Program Files (x86)\Electronic Arts\Ultima Online Classic\staidx0.mul')

    target_statics = Path(r'C:\DevEnv\GIT\UO\ServUO-fresh\Data\Maps\statics0.mul')
    target_staidx = Path(r'C:\DevEnv\GIT\UO\ServUO-fresh\Data\Maps\staidx0.mul')

    shutil.copy(source_statics, target_statics)
    shutil.copy(source_staidx, target_staidx)

    print(f"\nDone! Check CentrED at (1750, 800)")
