"""
Extract Britain from OSI map0 as a CentrED blueprint
Creates a blueprint with relative coordinates so you can place it anywhere in CentrEDSharp
Includes both terrain (land tiles) and statics
"""

import struct
import json
from pathlib import Path

# OSI Map0 dimensions
OSI_MAP_WIDTH_BLOCKS = 896   # 7168 tiles / 8
OSI_MAP_HEIGHT_BLOCKS = 512  # 4096 tiles / 8

# Britain bounds (OSI coordinates)
BRITAIN_X1, BRITAIN_Y1 = 1400, 1500
BRITAIN_X2, BRITAIN_Y2 = 1750, 1800

def get_block_number(block_x: int, block_y: int) -> int:
    """OSI map0 block calculation: blockNum = blockX * mapHeight + blockY"""
    return block_x * OSI_MAP_HEIGHT_BLOCKS + block_y

print("="*80)
print("BRITAIN BLUEPRINT EXTRACTION FROM OSI MAP0")
print("="*80)

# Source: OSI map files
osi_base = Path(r'C:\Program Files (x86)\Electronic Arts\Ultima Online Classic')
osi_map = osi_base / 'map0.mul'
osi_statics = osi_base / 'statics0.mul'
osi_staidx = osi_base / 'staidx0.mul'
osi_multi_mul = osi_base / 'multi.mul'
osi_multi_idx = osi_base / 'multi.idx'

if not osi_map.exists():
    print("ERROR: OSI map file not found!")
    print(f"  Map: {osi_map}")
    exit(1)

if not osi_statics.exists() or not osi_staidx.exists():
    print("ERROR: OSI static files not found!")
    print(f"  Statics: {osi_statics}")
    print(f"  StaIdx: {osi_staidx}")
    exit(1)

if not osi_multi_mul.exists() or not osi_multi_idx.exists():
    print("WARNING: Multi files not found! Buildings stored as multis won't be extracted.")
    print(f"  Multi.mul: {osi_multi_mul}")
    print(f"  Multi.idx: {osi_multi_idx}")
    print("  Continuing without multi extraction...")
    has_multis = False
else:
    has_multis = True

def read_multi(multi_id: int):
    """Read a multi structure from multi.mul and return its components"""
    if not has_multis:
        return []
    
    try:
        # Read index entry
        with open(osi_multi_idx, 'rb') as idx:
            idx.seek(multi_id * 12)
            offset = struct.unpack('<I', idx.read(4))[0]
            length = struct.unpack('<I', idx.read(4))[0]
            extra = struct.unpack('<I', idx.read(4))[0]

        if offset == 0xFFFFFFFF or length == 0:
            return []

        # Read multi data (12 bytes per component: tile_id, x, y, z, flags)
        components = []
        with open(osi_multi_mul, 'rb') as mul:
            mul.seek(offset)
            component_count = length // 12

            for i in range(component_count):
                tile_id = struct.unpack('<H', mul.read(2))[0]
                x = struct.unpack('<h', mul.read(2))[0]  # signed
                y = struct.unpack('<h', mul.read(2))[0]  # signed
                z = struct.unpack('<h', mul.read(2))[0]  # signed
                flags = struct.unpack('<I', mul.read(4))[0]

                # Filter out invalid tile IDs
                if tile_id == 0 or tile_id >= 0xFFFD:
                    continue

                components.append({
                    'tile_id': tile_id,
                    'x': x,
                    'y': y,
                    'z': z,
                    'flags': flags
                })

        return components
    except Exception as e:
        print(f"Warning: Could not read multi {multi_id}: {e}")
        return []

print(f"Source: {osi_base}")
print(f"Britain bounds: ({BRITAIN_X1}, {BRITAIN_Y1}) to ({BRITAIN_X2}, {BRITAIN_Y2})")

# Calculate center point for the blueprint
center_x = (BRITAIN_X1 + BRITAIN_X2) // 2
center_y = (BRITAIN_Y1 + BRITAIN_Y2) // 2

print(f"Center point: ({center_x}, {center_y})")

# Step 1: Extract terrain tiles
print("\n[1/2] Extracting terrain tiles...")
terrain_items = []
terrain_count = 0

with open(osi_map, 'rb') as map_file:
    for y in range(BRITAIN_Y1, BRITAIN_Y2 + 1):
        for x in range(BRITAIN_X1, BRITAIN_X2 + 1):
            block_x = x // 8
            block_y = y // 8
            cell_x = x % 8
            cell_y = y % 8

            # OSI block calculation
            block_id = get_block_number(block_x, block_y)
            block_offset = block_id * 196  # Each block is 196 bytes
            cell_offset = block_offset + 4 + (cell_y * 8 + cell_x) * 3

            try:
                map_file.seek(cell_offset)
                tile_id = struct.unpack('<H', map_file.read(2))[0]
                z = struct.unpack('b', map_file.read(1))[0]

                # Convert to relative coordinates
                rel_x = x - center_x
                rel_y = y - center_y

                terrain_items.append({
                    'tile_id': tile_id,
                    'x': rel_x,
                    'y': rel_y,
                    'z': z
                })
                terrain_count += 1
            except (struct.error, IOError) as e:
                print(f"Warning: Could not read terrain at ({x}, {y}): {e}")

print(f"  Extracted {terrain_count} terrain tiles")

# Step 2: Extract static tiles and multis
print("\n[2/2] Extracting static tiles and multis...")
static_items = []
static_count = 0
multi_items = []  # Components from multis
multi_count = 0
multis_found = {}  # Track which multis we've already extracted

# Calculate block range
block_x1 = BRITAIN_X1 // 8
block_y1 = BRITAIN_Y1 // 8
block_x2 = BRITAIN_X2 // 8
block_y2 = BRITAIN_Y2 // 8

with open(osi_staidx, 'rb') as idx, open(osi_statics, 'rb') as sta:
    for block_y in range(block_y1, block_y2 + 1):
        for block_x in range(block_x1, block_x2 + 1):
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

                # Only include statics within exact bounds
                if BRITAIN_X1 <= world_x <= BRITAIN_X2 and BRITAIN_Y1 <= world_y <= BRITAIN_Y2:
                    # Check if this is a multi (tile_id >= 0x4000)
                    if tile_id >= 0x4000:
                        # This is a multi - extract its components
                        multi_id = tile_id - 0x4000
                        
                        # Only extract each multi once (cache results)
                        if multi_id not in multis_found:
                            components = read_multi(multi_id)
                            multis_found[multi_id] = components
                        else:
                            components = multis_found[multi_id]
                        
                        # Convert multi components to world coordinates relative to center
                        for comp in components:
                            comp_world_x = world_x + comp['x']
                            comp_world_y = world_y + comp['y']
                            
                            # Only include if within bounds
                            if BRITAIN_X1 <= comp_world_x <= BRITAIN_X2 and BRITAIN_Y1 <= comp_world_y <= BRITAIN_Y2:
                                rel_x = comp_world_x - center_x
                                rel_y = comp_world_y - center_y
                                
                                multi_items.append({
                                    'tile_id': comp['tile_id'],
                                    'x': rel_x,
                                    'y': rel_y,
                                    'z': z + comp['z'],  # Add multi's Z offset
                                    'hue': 0
                                })
                                multi_count += 1
                    else:
                        # Regular static item
                        rel_x = world_x - center_x
                        rel_y = world_y - center_y

                        static_items.append({
                            'tile_id': tile_id,
                            'x': rel_x,
                            'y': rel_y,
                            'z': z,
                            'hue': hue
                        })
                        static_count += 1

print(f"  Extracted {static_count} static tiles")
print(f"  Extracted {multi_count} components from {len(multis_found)} multis")

# Combine all items (terrain + statics + multi components)
blueprint_items = terrain_items + static_items + multi_items

# Export as JSON blueprint
script_dir = Path(__file__).parent
output_dir = script_dir / "britain_blueprint"
output_dir.mkdir(parents=True, exist_ok=True)

blueprint_data = {
    'name': 'Britain (OSI)',
    'description': 'Complete Britain city from OSI map0 - includes terrain and statics. Use relative coordinates to place anywhere.',
    'center': {'x': center_x, 'y': center_y},  # Original OSI center
    'bounds': {
        'x1': BRITAIN_X1,
        'y1': BRITAIN_Y1,
        'x2': BRITAIN_X2,
        'y2': BRITAIN_Y2
    },
    'total_items': len(blueprint_items),
    'terrain_count': len(terrain_items),
    'statics_count': len(static_items),
    'multi_count': len(multi_items),
    'multis_found': len(multis_found),
    'items': blueprint_items
}

blueprint_file = output_dir / "britain_complete.json"
with open(blueprint_file, 'w') as f:
    json.dump(blueprint_data, f, indent=2)

print(f"\n[OK] Blueprint saved to: {blueprint_file}")
print(f"[OK] Total items: {len(blueprint_items)}")
print(f"     - Terrain: {len(terrain_items)}")
print(f"     - Statics: {len(static_items)}")
print(f"     - Multi components: {len(multi_items)} (from {len(multis_found)} multis)")
print("\n" + "="*80)
print("BLUEPRINT READY FOR CENTREDSHARP")
print("="*80)
print("The blueprint uses relative coordinates (offset from center).")
print("To place Britain at a new location:")
print("  1. Import the blueprint into CentrEDSharp")
print("  2. Specify your desired center coordinates (X, Y)")
print("  3. Each item's world position = new_center + relative_coordinates")
print(f"\nOriginal OSI center: ({center_x}, {center_y})")
print(f"Blueprint size: {BRITAIN_X2 - BRITAIN_X1 + 1} x {BRITAIN_Y2 - BRITAIN_Y1 + 1} tiles")
