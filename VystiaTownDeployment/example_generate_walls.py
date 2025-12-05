"""
Example: Generate walls for multiple Vystia cities
"""

from generate_city_walls import (
    generate_wall_perimeter,
    generate_circular_wall,
    group_statics_into_blocks,
    load_existing_map,
    write_map_files
)
from pathlib import Path

# City configurations from VYSTIA_WORLD_CONFIG.json
CITIES = [
    {
        'name': 'Ironheart Capital',
        'x': 800,
        'y': 600,
        'width': 200,
        'height': 200,
        'wall_type': 'stone',
        'gates': [
            (800, 500),  # North gate
            (800, 700),  # South gate
            (700, 600),  # West gate
            (900, 600),  # East gate
        ]
    },
    {
        'name': 'Frostholm',
        'x': 1200,
        'y': 200,
        'width': 150,
        'height': 150,
        'wall_type': 'stone_thick',
        'gates': [
            (1200, 125),  # North gate
            (1200, 275),  # South gate
        ]
    },
    {
        'name': 'Verdant Grove',
        'x': 1800,
        'y': 400,
        'width': 180,
        'height': 180,
        'wall_type': 'wood',
        'gates': [
            (1800, 310),  # North gate
            (1800, 490),  # South gate
        ]
    },
    {
        'name': 'Deepforge City',
        'x': 2400,
        'y': 1400,
        'width': 200,
        'height': 200,
        'wall_type': 'stone',
        'gates': [
            (2400, 1300),  # North gate
            (2400, 1500),  # South gate
        ]
    },
]

def main():
    print("="*60)
    print("Vystia City Wall Generator - Batch Mode")
    print("="*60)
    
    # Map file paths
    target_statics = Path(r'C:\Program Files (x86)\Electronic Arts\Ultima Online Classic\statics0.mul')
    target_staidx = Path(r'C:\Program Files (x86)\Electronic Arts\Ultima Online Classic\staidx0.mul')
    
    # Alternative CentrED path (uncomment if needed)
    # target_statics = Path(r'C:\DevEnv\GIT\UO\centredsharp\output\editable_maps\statics0.mul')
    # target_staidx = Path(r'C:\DevEnv\GIT\UO\centredsharp\output\editable_maps\staidx0.mul')
    
    if not target_statics.exists():
        print(f"ERROR: {target_statics} not found!")
        print("Please update the path in this script or install UO Classic")
        return
    
    # Generate walls for all cities
    all_walls = []
    
    for city in CITIES:
        print(f"\nGenerating walls for {city['name']}...")
        print(f"  Location: ({city['x']}, {city['y']})")
        print(f"  Size: {city['width']}x{city['height']}")
        print(f"  Wall type: {city['wall_type']}")
        print(f"  Gates: {len(city['gates'])}")
        
        walls = generate_wall_perimeter(
            city['x'],
            city['y'],
            city['width'],
            city['height'],
            city['wall_type'],
            city['gates'],
            z_level=0
        )
        
        all_walls.extend(walls)
        print(f"  Generated {len(walls)} wall pieces")
    
    print(f"\nTotal: {len(all_walls)} wall pieces across {len(CITIES)} cities")
    
    # Group into blocks
    print("\nGrouping into blocks...")
    wall_blocks = group_statics_into_blocks(all_walls)
    print(f"Walls span {len(wall_blocks)} blocks")
    
    # Load existing map
    print("Loading existing map...")
    existing_blocks = load_existing_map(target_staidx, target_statics)
    print(f"Loaded {len(existing_blocks)} existing blocks")
    
    # Merge walls with existing blocks
    print("Merging walls with existing map...")
    for block_id, statics in wall_blocks.items():
        if block_id in existing_blocks:
            existing_blocks[block_id].extend(statics)
        else:
            existing_blocks[block_id] = statics
    
    # Write to files
    print("Writing to map files...")
    write_map_files(existing_blocks, target_staidx, target_statics)
    
    print("\n" + "="*60)
    print("WALL GENERATION COMPLETE!")
    print("="*60)
    print(f"Generated walls for {len(CITIES)} cities")
    print(f"Total wall pieces: {len(all_walls)}")
    print("\nNote: Restart CentrED and ServUO to see changes")

if __name__ == '__main__':
    main()

