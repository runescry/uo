"""
Convert JSON blueprint to CentrED MultiText format (.txt)
CentrED supports MultiText format for blueprints
"""

import json
from pathlib import Path

def convert_json_to_multitext(json_path: Path, output_path: Path = None):
    """
    Convert JSON blueprint to CentrED MultiText format
    
    MultiText format:
    Line 1: "6 version"
    Line 2: template_id (can be 0)
    Line 3: item_version (can be 0)
    Line 4: "num_components"
    Then: "id x y z flags" for each item
    """
    
    # Load JSON blueprint
    with open(json_path, 'r') as f:
        blueprint = json.load(f)
    
    # Determine output path
    if output_path is None:
        output_path = json_path.parent / f"{json_path.stem}.txt"
    
    # Get items (statics only for blueprints, or all items)
    items = blueprint.get('items', [])
    
    # Use statics_count from metadata if available, otherwise filter
    terrain_count = blueprint.get('terrain_count', 0)
    statics_count = blueprint.get('statics_count', 0)
    
    # Building tile ID ranges (these are the actual building pieces)
    building_walls = set(range(0x0080, 0x0090)) | set(range(0x003D, 0x0045))
    building_floors = set(range(0x0495, 0x04B0))
    building_roofs = set(range(0x06A5, 0x06E0))
    building_doors = set(range(0x0675, 0x0685))
    building_windows = set(range(0x0063, 0x0067)) | set(range(0x0068, 0x006C))
    all_building_ids = building_walls | building_floors | building_roofs | building_doors | building_windows
    
    # Get terrain and statics sections
    terrain_items = items[:terrain_count] if terrain_count > 0 else []
    static_items = items[terrain_count:terrain_count + statics_count] if statics_count > 0 else []
    
    # Extract building pieces from BOTH terrain and statics sections
    # Buildings can be stored as terrain tiles OR as statics
    building_items_from_terrain = [item for item in terrain_items if item.get('tile_id', 0) in all_building_ids]
    building_items_from_statics = [item for item in static_items if item.get('tile_id', 0) in all_building_ids]
    
    # Combine all building pieces
    all_building_items = building_items_from_terrain + building_items_from_statics
    
    print(f"  Found {len(building_items_from_terrain)} building pieces in terrain section")
    print(f"  Found {len(building_items_from_statics)} building pieces in statics section")
    print(f"  Total building pieces: {len(all_building_items)}")
    
    # Filter out trees from statics (keep buildings)
    tree_ranges = [
        (0x0C99, 0x0CE8),  # Main tree range
        (0x0CCA, 0x0CD8),  # Tree stumps
        (0x0D41, 0x0D70),  # Various trees
        (0x0D7A, 0x0D7E),  # Swamp trees
        (0x0D85, 0x0D8C),  # Yucca trees
        (0x0D90, 0x0DA1),  # Palm trees
        (0x12B6, 0x12BC),  # Tree trunks
        (0x5659, 0x5666),  # Additional tree range (22105-22126 in decimal)
    ]
    
    def is_tree(tile_id):
        return any(min_id <= tile_id <= max_id for min_id, max_id in tree_ranges)
    
    # Filter statics to remove trees, then add building pieces
    non_tree_statics = [item for item in static_items if not is_tree(item.get('tile_id', 0))]
    trees_removed = len(static_items) - len(non_tree_statics)
    
    # Combine: building pieces from terrain + building pieces from statics + other non-tree statics
    # But avoid duplicates (buildings might appear in both)
    final_items = all_building_items + [item for item in non_tree_statics if item.get('tile_id', 0) not in all_building_ids]
    
    if trees_removed > 0:
        print(f"  Filtered out {trees_removed} trees/vegetation from statics")
    
    static_items = final_items
    
    print(f"Converting {len(static_items)} items from {json_path.name}")
    print(f"  Total items in JSON: {len(items)}")
    print(f"  Static items: {len(static_items)}")
    
    # Write MultiText format
    with open(output_path, 'w') as f:
        # Header
        f.write("6 version\n")
        f.write("0\n")  # template_id
        f.write("0\n")  # item_version
        f.write(f"{len(static_items)}\n")  # number of components
        
        # Items (relative coordinates from blueprint)
        for item in static_items:
            tile_id = item.get('tile_id', 0)
            x = item.get('x', 0)  # Already relative in blueprint
            y = item.get('y', 0)  # Already relative in blueprint
            z = item.get('z', 0)
            flags = 0  # Default flags
            
            f.write(f"{tile_id} {x} {y} {z} {flags}\n")
    
    print(f"Created CentrED blueprint: {output_path}")
    
    # Copy to CentrED Blueprints folder (relative to CentrED executable location)
    # CentrED looks for "Blueprints" folder relative to where it runs
    import shutil
    
    # Try root Blueprints folder first (most likely location)
    centred_blueprints_dir = Path(r'D:\UO\centredsharp\Blueprints')
    if not centred_blueprints_dir.exists():
        # Try CentrED subfolder
        centred_blueprints_dir = Path(r'D:\UO\centredsharp\CentrED\Blueprints')
    
    if centred_blueprints_dir.exists():
        target_file = centred_blueprints_dir / output_path.name
        shutil.copy2(output_path, target_file)
        print(f"Copied to CentrED Blueprints folder: {target_file}")
        print(f"\nBlueprint is ready to use in CentrED!")
        print(f"  - Restart CentrED or reload blueprints")
        print(f"  - The blueprint will appear in the Blueprints tree")
    else:
        # Create Blueprints folder in root
        centred_blueprints_dir = Path(r'D:\UO\centredsharp\Blueprints')
        centred_blueprints_dir.mkdir(parents=True, exist_ok=True)
        target_file = centred_blueprints_dir / output_path.name
        shutil.copy2(output_path, target_file)
        print(f"Created and copied to Blueprints folder: {target_file}")
        print(f"\nBlueprint is ready to use in CentrED!")
        print(f"  - Restart CentrED or reload blueprints")
        print(f"  - The blueprint will appear in the Blueprints tree")
    
    return output_path

def main():
    import argparse
    
    parser = argparse.ArgumentParser(description='Convert JSON blueprint to CentrED MultiText format')
    parser.add_argument('json_file', type=str, help='Path to JSON blueprint file')
    parser.add_argument('-o', '--output', type=str, help='Output .txt file path (default: same name as input)')
    parser.add_argument('--include-terrain', action='store_true', 
                       help='Include terrain tiles (default: statics only)')
    
    args = parser.parse_args()
    
    json_path = Path(args.json_file)
    if not json_path.exists():
        print(f"ERROR: JSON file not found: {json_path}")
        return 1
    
    output_path = Path(args.output) if args.output else None
    
    convert_json_to_multitext(json_path, output_path)
    
    return 0

if __name__ == '__main__':
    exit(main())
