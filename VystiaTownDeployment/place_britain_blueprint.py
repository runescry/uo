"""
Place Britain Blueprint at a specific location
Reads the blueprint JSON and places it at your specified coordinates
"""

import struct
import json
import shutil
from pathlib import Path
from typing import List, Dict
from collections import defaultdict

# Map dimensions (ML: 896x512 blocks)
MAP_WIDTH_BLOCKS = 896   # 7168 tiles / 8
MAP_HEIGHT_BLOCKS = 512  # 4096 tiles / 8

def get_block_number(block_x: int, block_y: int) -> int:
    """CentrED's block calculation: blockNum = blockX * mapHeight + blockY"""
    return block_x * MAP_HEIGHT_BLOCKS + block_y

def load_blueprint(blueprint_path: Path) -> Dict:
    """Load blueprint JSON file"""
    with open(blueprint_path, 'r') as f:
        return json.load(f)

def place_blueprint(blueprint_data: Dict, center_x: int, center_y: int, z_adjust: int = 0) -> List[Dict]:
    """
    Convert blueprint items to world coordinates at new center location
    
    Args:
        blueprint_data: Blueprint JSON data
        center_x: New center X coordinate
        center_y: New center Y coordinate
        z_adjust: Optional Z-level adjustment (default: 0)
    
    Returns:
        List of items with world coordinates
    """
    items = []
    
    for item in blueprint_data['items']:
        # Convert relative coordinates to world coordinates
        world_x = center_x + item['x']
        world_y = center_y + item['y']
        world_z = item['z'] + z_adjust
        
        # Clamp Z to valid range
        if world_z < -128:
            world_z = -128
        elif world_z > 127:
            world_z = 127
        
        items.append({
            'tile_id': item['tile_id'],
            'x': world_x,
            'y': world_y,
            'z': world_z,
            'hue': item.get('hue', 0)
        })
    
    return items

def group_items_by_block(items: List[Dict]) -> Dict[int, List[Dict]]:
    """Group items by block ID for efficient writing"""
    blocks = defaultdict(list)
    
    for item in items:
        block_x = item['x'] // 8
        block_y = item['y'] // 8
        block_id = get_block_number(block_x, block_y)
        
        # Convert to local block coordinates
        local_x = item['x'] % 8
        local_y = item['y'] % 8
        
        blocks[block_id].append({
            'tile_id': item['tile_id'],
            'x': local_x,
            'y': local_y,
            'z': item['z'],
            'hue': item.get('hue', 0)
        })
    
    return blocks

def write_terrain_to_map(items: List[Dict], map_path: Path, terrain_count: int = None):
    """Write terrain items to map0.mul"""
    # Terrain items come first in the blueprint (if terrain_count is specified)
    # Otherwise filter by tile_id < 0x4000
    if terrain_count is not None:
        terrain_items = items[:terrain_count]
    else:
        terrain_items = [item for item in items if item['tile_id'] < 0x4000]
    
    if not terrain_items:
        print("  No terrain items to write")
        return
    
    print(f"  Writing {len(terrain_items)} terrain tiles...")
    
    # Group by block
    terrain_blocks = defaultdict(dict)
    for item in terrain_items:
        block_x = item['x'] // 8
        block_y = item['y'] // 8
        block_id = get_block_number(block_x, block_y)
        cell_x = item['x'] % 8
        cell_y = item['y'] % 8
        cell_index = cell_y * 8 + cell_x
        
        terrain_blocks[block_id][cell_index] = (item['tile_id'], item['z'])
    
    # Read existing map, update blocks, write back
    with open(map_path, 'r+b') as map_file:
        for block_id, cells in terrain_blocks.items():
            block_offset = block_id * 196
            map_file.seek(block_offset)
            
            # Read existing block header
            header = map_file.read(4)
            if len(header) < 4:
                header = b'\x00\x00\x00\x00'
            
            # Read existing tiles
            existing_tiles = []
            for _ in range(64):
                tile_data = map_file.read(3)
                if len(tile_data) < 3:
                    existing_tiles.append((0x0001, 0))
                else:
                    tile_id = struct.unpack('<H', tile_data[0:2])[0]
                    z = struct.unpack('b', tile_data[2:3])[0]
                    existing_tiles.append((tile_id, z))
            
            # Update tiles
            for cell_index, (tile_id, z) in cells.items():
                existing_tiles[cell_index] = (tile_id, z)
            
            # Write back
            map_file.seek(block_offset)
            map_file.write(header)
            for tile_id, z in existing_tiles:
                map_file.write(struct.pack('<H', tile_id))
                map_file.write(struct.pack('b', z))

def write_statics_to_map(items: List[Dict], statics_path: Path, staidx_path: Path, terrain_count: int = None, statics_count: int = None):
    """Write static items to statics0.mul and staidx0.mul"""
    # Static items come after terrain items in the blueprint (if counts are specified)
    # Otherwise filter by tile_id >= 0x4000
    if terrain_count is not None and statics_count is not None:
        static_items = items[terrain_count:terrain_count + statics_count]
    else:
        static_items = [item for item in items if item['tile_id'] >= 0x4000]
    
    if not static_items:
        print("  No static items to write")
        return
    
    print(f"  Writing {len(static_items)} static tiles...")
    
    # Group by block
    blocks = group_items_by_block(static_items)
    
    # Read existing index and statics
    index_entries = {}
    with open(staidx_path, 'rb') as idx:
        for block_id in range(MAP_WIDTH_BLOCKS * MAP_HEIGHT_BLOCKS):
            idx.seek(block_id * 12)
            offset = struct.unpack('<I', idx.read(4))[0]
            length = struct.unpack('<I', idx.read(4))[0]
            index_entries[block_id] = (offset, length)
    
    # Read existing statics
    existing_statics = {}
    with open(statics_path, 'rb') as sta:
        for block_id, (offset, length) in index_entries.items():
            if offset != 0xFFFFFFFF and length > 0:
                sta.seek(offset)
                existing_statics[block_id] = sta.read(length)
            else:
                existing_statics[block_id] = b''
    
    # Merge new statics with existing
    for block_id, new_items in blocks.items():
        # Convert new items to binary
        new_data = b''
        for item in new_items:
            new_data += struct.pack('<H', item['tile_id'])
            new_data += struct.pack('B', item['x'])
            new_data += struct.pack('B', item['y'])
            new_data += struct.pack('b', item['z'])
            new_data += struct.pack('<H', item['hue'])
        
        # Append to existing
        if block_id in existing_statics:
            existing_statics[block_id] += new_data
        else:
            existing_statics[block_id] = new_data
    
    # Write updated statics and index
    current_offset = 0
    with open(statics_path, 'wb') as sta, open(staidx_path, 'wb') as idx:
        for block_id in range(MAP_WIDTH_BLOCKS * MAP_HEIGHT_BLOCKS):
            if block_id in existing_statics and len(existing_statics[block_id]) > 0:
                data = existing_statics[block_id]
                sta.write(data)
                
                idx.write(struct.pack('<I', current_offset))
                idx.write(struct.pack('<I', len(data)))
                idx.write(struct.pack('<I', 0))  # Extra field
                
                current_offset += len(data)
            else:
                idx.write(struct.pack('<I', 0xFFFFFFFF))
                idx.write(struct.pack('<I', 0))
                idx.write(struct.pack('<I', 0))

def main():
    import argparse
    
    parser = argparse.ArgumentParser(description='Place Britain blueprint at specified location')
    parser.add_argument('--blueprint', type=str, 
                       default='britain_blueprint/britain_complete.json',
                       help='Path to blueprint JSON file')
    parser.add_argument('--center-x', type=int, required=True,
                       help='Center X coordinate where to place Britain')
    parser.add_argument('--center-y', type=int, required=True,
                       help='Center Y coordinate where to place Britain')
    parser.add_argument('--z-adjust', type=int, default=0,
                       help='Z-level adjustment (default: 0)')
    parser.add_argument('--map', type=str,
                       help='Path to map0.mul (auto-detect from CentrED config if not specified)')
    parser.add_argument('--statics', type=str,
                       help='Path to statics0.mul (auto-detect from CentrED config if not specified)')
    parser.add_argument('--staidx', type=str,
                       help='Path to staidx0.mul (auto-detect from CentrED config if not specified)')
    parser.add_argument('--dry-run', action='store_true',
                       help='Preview what would be placed without writing files')
    parser.add_argument('--statics-only', action='store_true',
                       help='Only place statics (buildings), preserve existing terrain')
    
    args = parser.parse_args()
    
    # Load blueprint
    script_dir = Path(__file__).parent
    blueprint_path = script_dir / args.blueprint
    
    if not blueprint_path.exists():
        print(f"ERROR: Blueprint file not found: {blueprint_path}")
        return 1
    
    print("="*80)
    print("PLACING BRITAIN BLUEPRINT")
    print("="*80)
    print(f"Blueprint: {blueprint_path}")
    print(f"Target center: ({args.center_x}, {args.center_y})")
    if args.z_adjust != 0:
        print(f"Z adjustment: {args.z_adjust:+d}")
    
    blueprint_data = load_blueprint(blueprint_path)
    print(f"Blueprint items: {blueprint_data['total_items']}")
    print(f"  - Terrain: {blueprint_data.get('terrain_count', 'unknown')}")
    print(f"  - Statics: {blueprint_data.get('statics_count', 'unknown')}")
    
    # Convert to world coordinates
    items = place_blueprint(blueprint_data, args.center_x, args.center_y, args.z_adjust)
    
    if args.dry_run:
        print("\n[DRY RUN] Would place items:")
        print(f"  Total: {len(items)}")
        x_coords = [item['x'] for item in items]
        y_coords = [item['y'] for item in items]
        print(f"  Bounds: ({min(x_coords)}, {min(y_coords)}) to ({max(x_coords)}, {max(y_coords)})")
        return 0
    
    # Determine output paths
    if args.map and args.statics and args.staidx:
        map_path = Path(args.map)
        statics_path = Path(args.statics)
        staidx_path = Path(args.staidx)
    else:
        # Try to auto-detect from CentrED config
        config_path = Path(r'D:\UO\centredsharp\Output-Custom-0\Cedserver.xml')
        if config_path.exists():
            import xml.etree.ElementTree as ET
            tree = ET.parse(config_path)
            root = tree.getroot()
            map_path = Path(root.find('.//Map').text)
            statics_path = Path(root.find('.//Statics').text)
            staidx_path = Path(root.find('.//StaIdx').text)
            print(f"\nAuto-detected map paths from CentrED config")
        else:
            print("ERROR: Must specify --map, --statics, --staidx or have CentrED config accessible")
            return 1
    
    print(f"\nTarget files:")
    print(f"  Map: {map_path}")
    print(f"  Statics: {statics_path}")
    print(f"  StaIdx: {staidx_path}")
    
    # Write to map files
    print("\nWriting to map files...")
    terrain_count = blueprint_data.get('terrain_count')
    statics_count = blueprint_data.get('statics_count')
    
    if args.statics_only:
        print("  Mode: STATICS ONLY (preserving existing terrain)")
        # Skip terrain, only write statics
    else:
        write_terrain_to_map(items, map_path, terrain_count)
    
    write_statics_to_map(items, statics_path, staidx_path, terrain_count, statics_count)
    
    print("\n" + "="*80)
    print("BLUEPRINT PLACED SUCCESSFULLY")
    print("="*80)
    print("Note: Restart CentrEDSharp and your server to see changes")
    
    return 0

if __name__ == '__main__':
    exit(main())
