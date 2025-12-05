"""
City Wall Generator CLI - Command-line interface for generating city walls
Usage: python generate_city_walls_cli.py --center-x 800 --center-y 600 --width 200 --height 200
"""

import argparse
from pathlib import Path
from generate_city_walls import (
    generate_wall_perimeter,
    generate_advanced_wall_perimeter,
    generate_circular_wall,
    group_statics_into_blocks,
    load_existing_map,
    write_map_files,
    get_centred_map_paths,
    backup_map_files,
    get_terrain_z_at,
    WALL_TILES
)


def main():
    parser = argparse.ArgumentParser(
        description='Generate walls around a city on UO map',
        formatter_class=argparse.RawDescriptionHelpFormatter,
        epilog="""
Examples:
  # Rectangular wall around Ironheart Capital
  python generate_city_walls_cli.py --center-x 800 --center-y 600 --width 200 --height 200
  
  # Circular wall with gates
  python generate_city_walls_cli.py --center-x 1200 --center-y 200 --radius 100 --circular --gates "1200,200 1200,300"
  
  # Custom wall type and output location
  python generate_city_walls_cli.py --center-x 1800 --center-y 400 --width 150 --height 150 --wall-type brick --output-dir "C:\\DevEnv\\GIT\\UO\\centredsharp\\output\\editable_maps"
        """
    )
    
    # City location
    parser.add_argument('--center-x', type=int, required=True,
                       help='X coordinate of city center')
    parser.add_argument('--center-y', type=int, required=True,
                       help='Y coordinate of city center')
    
    # City shape
    shape_group = parser.add_mutually_exclusive_group(required=True)
    shape_group.add_argument('--width', type=int,
                            help='Width of rectangular city (tiles)')
    shape_group.add_argument('--radius', type=int,
                            help='Radius of circular city (tiles)')
    
    parser.add_argument('--height', type=int,
                       help='Height of rectangular city (tiles)')
    parser.add_argument('--circular', action='store_true',
                       help='Generate circular wall instead of rectangular')
    
    # Wall configuration
    parser.add_argument('--wall-type', type=str, default='stone',
                       choices=['stone', 'advanced', 'stone_old', 'stone_thick', 'stone_thin', 'brick', 'wood', 'marble', 'stone_ruined'],
                       help='Type of wall to use (default: stone - uses directional tiles, advanced = Britannia-style battlements)')
    parser.add_argument('--material', type=str, default='stone',
                       choices=['stone', 'sandstone'],
                       help='Material for advanced walls (default: stone, sandstone uses hue 0x03E9)')
    parser.add_argument('--wall-height', type=int, default=1,
                       help='Number of wall tiles to stack vertically (default: 1, ignored for advanced walls)')
    parser.add_argument('--walkway-width', type=int, default=2,
                       help='Width of walkway for advanced walls (default: 2)')
    parser.add_argument('--z-level', type=int, default=None,
                       help='Z-level for walls (default: auto-detect from terrain or 0)')
    parser.add_argument('--auto-z', action='store_true',
                       help='Auto-detect Z-level from terrain at city center')
    
    # Gates
    parser.add_argument('--gates', type=str, default='',
                       help='Gate positions as "x1,y1 x2,y2 ..." (walls skipped at these positions)')
    
    # Output
    parser.add_argument('--output-dir', type=str, default=None,
                       help='Directory containing statics0.mul and staidx0.mul (default: auto-detect from CentrED config)')
    parser.add_argument('--no-backup', action='store_true',
                       help='Skip automatic backup before writing')
    parser.add_argument('--backup-dir', type=str, default=None,
                       help='Directory for backups (default: backups/ in script directory)')
    parser.add_argument('--dry-run', action='store_true',
                       help='Show what would be generated without writing files')
    
    args = parser.parse_args()
    
    # Validate arguments
    if not args.circular and (args.width is None or args.height is None):
        parser.error('--width and --height required for rectangular walls')
    if args.circular and args.radius is None:
        parser.error('--radius required for circular walls')
    
    # Parse gate positions
    gate_positions = []
    if args.gates:
        for gate_str in args.gates.split():
            try:
                x, y = map(int, gate_str.split(','))
                gate_positions.append((x, y))
            except ValueError:
                parser.error(f'Invalid gate position: {gate_str}. Use format "x,y"')
    
    # Determine output paths - try CentrED config first
    target_statics = None
    target_staidx = None
    map_path = None
    
    if args.output_dir:
        output_dir = Path(args.output_dir)
        target_statics = output_dir / 'statics0.mul'
        target_staidx = output_dir / 'staidx0.mul'
        map_path = output_dir / 'map0.mul'
    else:
        # Auto-detect from CentrED config
        statics_path, staidx_path, map_path = get_centred_map_paths()
        if statics_path and staidx_path:
            target_statics = statics_path
            target_staidx = staidx_path
            print(f"Auto-detected CentrED map location from config")
        else:
            # Fallback to UO Classic install
            target_statics = Path(r'C:\Program Files (x86)\Electronic Arts\Ultima Online Classic\statics0.mul')
            target_staidx = Path(r'C:\Program Files (x86)\Electronic Arts\Ultima Online Classic\staidx0.mul')
            print(f"Using default UO Classic location")
    
    # Check files exist
    if not args.dry_run:
        if not target_statics.exists():
            print(f"ERROR: {target_statics} not found!")
            print("Specify --output-dir or ensure CentrED config is accessible")
            return 1
        if not target_staidx.exists():
            print(f"ERROR: {target_staidx} not found!")
            return 1
    
    # Determine Z-level
    z_level = args.z_level
    if z_level is None:
        if args.auto_z and map_path and map_path.exists():
            z_level = get_terrain_z_at(map_path, args.center_x, args.center_y)
            print(f"Auto-detected terrain Z-level: {z_level}")
        else:
            z_level = 0
            if args.auto_z:
                print(f"Warning: Could not auto-detect Z-level, using 0")
    
    # Print configuration
    print("="*60)
    print("City Wall Generator")
    print("="*60)
    print(f"City center: ({args.center_x}, {args.center_y})")
    if args.circular:
        print(f"Shape: Circular (radius: {args.radius})")
    else:
        print(f"Shape: Rectangular ({args.width}x{args.height})")
    print(f"Wall type: {args.wall_type}")
    print(f"Z-level: {z_level}")
    print(f"Gates: {len(gate_positions)} positions")
    if gate_positions:
        for gate in gate_positions:
            print(f"  - Gate at ({gate[0]}, {gate[1]})")
    print(f"Output: {target_statics}")
    if args.dry_run:
        print("Mode: DRY RUN (no files will be written)")
    elif not args.no_backup:
        print("Backup: ENABLED (will backup before writing)")
    print()
    
    # Generate walls
    print("Generating wall perimeter...")
    if args.circular:
        walls = generate_circular_wall(
            args.center_x,
            args.center_y,
            args.radius,
            args.wall_type,
            gate_positions,
            z_level
        )
    elif args.wall_type == 'advanced':
        # Use advanced Britannia-style walls with battlements
        # Select wall structure based on material
        from generate_city_walls import WALL_STRUCTURE_STONE, WALL_STRUCTURE_SANDSTONE
        if args.material == 'sandstone':
            wall_struct = WALL_STRUCTURE_SANDSTONE
        else:
            wall_struct = WALL_STRUCTURE_STONE

        walls = generate_advanced_wall_perimeter(
            args.center_x,
            args.center_y,
            args.width,
            args.height,
            gate_positions,
            z_level,
            args.walkway_width,
            wall_struct
        )
    else:
        walls = generate_wall_perimeter(
            args.center_x,
            args.center_y,
            args.width,
            args.height,
            args.wall_type,
            gate_positions,
            z_level,
            args.wall_height
        )
    
    print(f"Generated {len(walls)} wall pieces")
    
    if args.dry_run:
        print("\nDRY RUN - No files written")
        print(f"Would write {len(walls)} wall pieces")
        return 0
    
    # Backup map files
    if not args.no_backup:
        script_dir = Path(__file__).parent
        if args.backup_dir:
            backup_dir = Path(args.backup_dir)
        else:
            backup_dir = script_dir / 'backups'
        
        backup_map_files(target_statics, target_staidx, backup_dir)
        print()
    
    # Group into blocks
    print("Grouping into blocks...")
    wall_blocks = group_statics_into_blocks(walls)
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
    print(f"Walls added around ({args.center_x}, {args.center_y})")
    print(f"Total wall pieces: {len(walls)}")
    print("\nNote: Restart CentrED and ServUO to see changes")
    
    return 0


if __name__ == '__main__':
    exit(main())

