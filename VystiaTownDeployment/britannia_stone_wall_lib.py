"""
City Wall Generator - Automatically surround a city with walls
Uses CentrED's correct block calculation formula
"""

import struct
import xml.etree.ElementTree as ET
import shutil
from datetime import datetime
from pathlib import Path
from typing import List, Tuple, Dict, Optional

# Map dimensions
MAP_WIDTH_BLOCKS = 896   # 7168 tiles / 8
MAP_HEIGHT_BLOCKS = 512  # 4096 tiles / 8

# Advanced wall structure with battlements (from Britannia city walls)
WALL_STRUCTURE_STONE = {
    'name': 'stone_battlements',
    'foundation': 0x00C5,      # Stone foundation base
    'wall_body': 0x071E,       # Main wall body (outer walls at Z=20)
    'floor': 0x04AB,           # Walkway floor
    'hue': 2500,               # Grey stone hue
    'battlements_ew': {        # East-West battlements
        'merlons': 0x0522,     # Grey stone merlon (consistent)
        'caps': 0x0519,        # Stone cap
    },
    'battlements_ns': {        # North-South battlements (rotated)
        'merlons': 0x0522,     # Grey stone merlon (consistent, same as EW)
        'caps': 0x0519,        # Stone cap (same as EW)
    }
}

WALL_STRUCTURE_SANDSTONE = {
    'name': 'sandstone_battlements',
    'foundation': 0x00C5,      # Stone foundation base (same)
    'wall_body': 0x071E,       # Main wall body (same tile, different hue)
    'floor': 0x04AB,           # Walkway floor (same)
    'hue': 0x03E9,             # Sandstone hue (1001 decimal)
    'battlements_ew': {        # East-West battlements
        'merlons': 0x0522,     # Sandstone merlon
        'caps': 0x0519,        # Sandstone cap
    },
    'battlements_ns': {        # North-South battlements (rotated)
        'merlons': 0x0522,     # Sandstone merlon (same as EW)
        'caps': 0x0519,        # Sandstone cap (same as EW)
    }
}

# Default to stone
WALL_STRUCTURE = WALL_STRUCTURE_STONE

# Simple wall tile IDs (fallback)
WALL_TILES = {
    'stone': {
        'north_south': 0x001B,  # Vertical wall (north-south)
        'east_west': 0x001C,    # Horizontal wall (east-west)
        'corner_nw': 0x001A,    # Northwest corner
        'corner_ne': 0x001A,    # Northeast corner
        'corner_sw': 0x001A,    # Southwest corner
        'corner_se': 0x001A,    # Southeast corner
    },
    'advanced': WALL_STRUCTURE,  # Advanced layered wall
    'stone_old': 0x0051,      # Old non-directional stone wall
    'stone_thick': 0x0052,    # Thick stone wall
    'stone_thin': 0x0053,     # Thin stone wall
    'brick': 0x0054,          # Brick wall
    'wood': 0x0055,           # Wooden wall
    'marble': 0x0056,         # Marble wall
    'stone_ruined': 0x0057,   # Ruined stone wall
}

def get_block_number(block_x: int, block_y: int) -> int:
    """
    CentrED's block calculation: blockNum = blockX * mapHeight + blockY
    """
    return block_x * MAP_HEIGHT_BLOCKS + block_y


def generate_advanced_wall_stack(x: int, y: int, z_base: int, is_east_west: bool, position_index: int, hue: int = 2500) -> List[Dict]:
    """
    Generate a single position's advanced wall stack with raised battlements on alternating positions.

    Args:
        x: X coordinate
        y: Y coordinate
        z_base: Base Z-level
        is_east_west: True for horizontal walls, False for vertical
        position_index: Position along the wall (for alternating battlements)
        hue: Hue for colored stones (default 2500 for Britannia grey)

    Returns:
        List of static items for this position
    """
    items = []

    # Select battlement set based on orientation
    battlements = WALL_STRUCTURE['battlements_ew'] if is_east_west else WALL_STRUCTURE['battlements_ns']

    # Layer 1: Foundation (Z=0)
    items.append({
        'tile_id': WALL_STRUCTURE['foundation'],
        'x': x,
        'y': y,
        'z': z_base,
        'hue': hue
    })

    # Layer 2: Wall body (Z=20)
    items.append({
        'tile_id': WALL_STRUCTURE['wall_body'],
        'x': x,
        'y': y,
        'z': z_base + 20,
        'hue': hue
    })

    # Layer 3: Raised battlements on alternating positions
    if position_index % 2 == 0:
        # Additional wall support at Z=25
        items.append({
            'tile_id': WALL_STRUCTURE['wall_body'],
            'x': x,
            'y': y,
            'z': z_base + 25,
            'hue': hue
        })
        # Merlon block at Z=25
        items.append({
            'tile_id': battlements['merlons'],  # 0x0522 blocks
            'x': x,
            'y': y,
            'z': z_base + 25,
            'hue': 0
        })
        # Cap block at Z=30
        items.append({
            'tile_id': battlements['caps'],  # 0x0519 cap
            'x': x,
            'y': y,
            'z': z_base + 30,
            'hue': 0
        })

    return items


def generate_wall_perimeter(
    center_x: int,
    center_y: int,
    width: int,
    height: int,
    wall_type: str = 'stone',
    gate_positions: List[Tuple[int, int]] = None,
    z_level: int = 0,
    wall_height: int = 1
) -> List[Dict]:
    """
    Generate wall statics around a rectangular perimeter.

    Args:
        center_x: X coordinate of city center
        center_y: Y coordinate of city center
        width: Width of city in tiles
        height: Height of city in tiles
        wall_type: Type of wall ('stone', 'brick', etc.)
        gate_positions: List of (x, y) positions where gates should be (walls skipped)
        z_level: Z-level for walls
        wall_height: Number of wall tiles to stack vertically (default: 1)

    Returns:
        List of wall static dictionaries
    """
    if gate_positions is None:
        gate_positions = []

    gate_set = set(gate_positions)

    # Get wall tiles - support both directional and non-directional
    wall_tiles = WALL_TILES.get(wall_type, WALL_TILES['stone'])
    if isinstance(wall_tiles, dict):
        # Directional walls
        tile_east_west = wall_tiles['east_west']
        tile_north_south = wall_tiles['north_south']
        tile_corner_nw = wall_tiles['corner_nw']
        tile_corner_ne = wall_tiles['corner_ne']
        tile_corner_sw = wall_tiles['corner_sw']
        tile_corner_se = wall_tiles['corner_se']
    else:
        # Non-directional walls (old style)
        tile_east_west = wall_tiles
        tile_north_south = wall_tiles
        tile_corner_nw = wall_tiles
        tile_corner_ne = wall_tiles
        tile_corner_sw = wall_tiles
        tile_corner_se = wall_tiles

    # Calculate bounds
    half_width = width // 2
    half_height = height // 2

    min_x = center_x - half_width
    max_x = center_x + half_width
    min_y = center_y - half_height
    max_y = center_y + half_height

    walls = []

    # Top wall (north) - horizontal, use east_west tiles
    for x in range(min_x, max_x + 1):
        if (x, min_y) not in gate_set:
            # Determine tile (corners vs straight)
            if x == min_x:
                tile_id = tile_corner_nw
            elif x == max_x:
                tile_id = tile_corner_ne
            else:
                tile_id = tile_east_west

            # Stack wall tiles vertically
            for h in range(wall_height):
                walls.append({
                    'tile_id': tile_id,
                    'x': x,
                    'y': min_y,
                    'z': z_level + (h * 20),  # Each wall tile is ~20 z-units high
                    'hue': 0
                })

    # Bottom wall (south) - horizontal, use east_west tiles
    for x in range(min_x, max_x + 1):
        if (x, max_y) not in gate_set:
            # Determine tile (corners vs straight)
            if x == min_x:
                tile_id = tile_corner_sw
            elif x == max_x:
                tile_id = tile_corner_se
            else:
                tile_id = tile_east_west

            # Stack wall tiles vertically
            for h in range(wall_height):
                walls.append({
                    'tile_id': tile_id,
                    'x': x,
                    'y': max_y,
                    'z': z_level + (h * 20),
                    'hue': 0
                })

    # Left wall (west) - vertical, use north_south tiles
    for y in range(min_y + 1, max_y):  # +1 and -1 to avoid corners twice
        if (min_x, y) not in gate_set:
            # Stack wall tiles vertically
            for h in range(wall_height):
                walls.append({
                    'tile_id': tile_north_south,
                    'x': min_x,
                    'y': y,
                    'z': z_level + (h * 20),
                    'hue': 0
                })

    # Right wall (east) - vertical, use north_south tiles
    for y in range(min_y + 1, max_y):
        if (max_x, y) not in gate_set:
            # Stack wall tiles vertically
            for h in range(wall_height):
                walls.append({
                    'tile_id': tile_north_south,
                    'x': max_x,
                    'y': y,
                    'z': z_level + (h * 20),
                    'hue': 0
                })

    return walls


def generate_advanced_wall_perimeter(
    center_x: int,
    center_y: int,
    width: int,
    height: int,
    gate_positions: List[Tuple[int, int]] = None,
    z_level: int = 0,
    walkway_width: int = 2,
    wall_structure: dict = None
) -> List[Dict]:
    """
    Generate advanced wall perimeter with battlements and walkway (Britannia-style).

    Args:
        center_x: X coordinate of city center
        center_y: Y coordinate of city center
        width: Width of city in tiles
        height: Height of city in tiles
        gate_positions: List of (x, y) positions where gates should be (walls skipped)
        z_level: Z-level for walls
        walkway_width: Width of walkway in tiles (default: 2)
        wall_structure: Wall structure dict (default: WALL_STRUCTURE_STONE)

    Returns:
        List of wall static dictionaries
    """
    if gate_positions is None:
        gate_positions = []

    if wall_structure is None:
        wall_structure = WALL_STRUCTURE_STONE

    gate_set = set(gate_positions)

    # Use the wall structure for this generation
    WALL_STRUCT = wall_structure
    inner_wall_hue = WALL_STRUCT['hue']

    # Calculate bounds
    half_width = width // 2
    half_height = height // 2

    min_x = center_x - half_width
    max_x = center_x + half_width
    min_y = center_y - half_height
    max_y = center_y + half_height

    walls = []

    # Top wall (north) - horizontal
    position_index = 0
    for x in range(min_x, max_x + 1):
        if (x, min_y) not in gate_set:
            walls.extend(generate_advanced_wall_stack(x, min_y, z_level, True, position_index))
            position_index += 1

    # Bottom wall (south) - horizontal
    position_index = 0
    for x in range(min_x, max_x + 1):
        if (x, max_y) not in gate_set:
            walls.extend(generate_advanced_wall_stack(x, max_y, z_level, True, position_index))
            position_index += 1

    # Left wall (west) - vertical
    position_index = 0
    for y in range(min_y + 1, max_y):
        if (min_x, y) not in gate_set:
            walls.extend(generate_advanced_wall_stack(min_x, y, z_level, False, position_index))
            position_index += 1

    # Right wall (east) - vertical
    position_index = 0
    for y in range(min_y + 1, max_y):
        if (max_x, y) not in gate_set:
            walls.extend(generate_advanced_wall_stack(max_x, y, z_level, False, position_index))
            position_index += 1

    # Generate walkway (inner courtyard area)
    # Walkway runs parallel to walls at Z=20 - FLAT, no raised blocks
    for w in range(1, walkway_width + 1):
        # Top walkway (south of north wall)
        for x in range(min_x, max_x + 1):
            walls.append({
                'tile_id': WALL_STRUCTURE['floor'],
                'x': x,
                'y': min_y + w,
                'z': z_level + 20,
                'hue': 0
            })

        # Bottom walkway (north of south wall)
        for x in range(min_x, max_x + 1):
            walls.append({
                'tile_id': WALL_STRUCTURE['floor'],
                'x': x,
                'y': max_y - w,
                'z': z_level + 20,
                'hue': 0
            })

        # Left walkway (east of west wall)
        for y in range(min_y + walkway_width + 1, max_y - walkway_width):
            walls.append({
                'tile_id': WALL_STRUCTURE['floor'],
                'x': min_x + w,
                'y': y,
                'z': z_level + 20,
                'hue': 0
            })

        # Right walkway (west of east wall)
        for y in range(min_y + walkway_width + 1, max_y - walkway_width):
            walls.append({
                'tile_id': WALL_STRUCTURE['floor'],
                'x': max_x - w,
                'y': y,
                'z': z_level + 20,
                'hue': 0
            })

    # Generate inner stone walls (on the inside edge of the walkway)
    # Inner walls also have raised battlements (alternating pattern)
    inner_wall_hue = 2500  # Grey stone

    # Top inner wall (south edge of top walkway) - horizontal
    position_index = 0
    for x in range(min_x, max_x + 1):
        # Foundation at Z=0
        walls.append({
            'tile_id': WALL_STRUCTURE['foundation'],
            'x': x,
            'y': min_y + walkway_width + 1,
            'z': z_level,
            'hue': inner_wall_hue
        })
        # Wall body at Z=20
        walls.append({
            'tile_id': WALL_STRUCTURE['wall_body'],
            'x': x,
            'y': min_y + walkway_width + 1,
            'z': z_level + 20,
            'hue': inner_wall_hue
        })
        # Add FULL raised battlements on alternating positions (same as outer walls)
        if position_index % 2 == 0:
            # Wall body support at Z=25
            walls.append({
                'tile_id': WALL_STRUCTURE['wall_body'],
                'x': x,
                'y': min_y + walkway_width + 1,
                'z': z_level + 25,
                'hue': inner_wall_hue
            })
            # Merlon at Z=25
            walls.append({
                'tile_id': WALL_STRUCTURE['battlements_ew']['merlons'],
                'x': x,
                'y': min_y + walkway_width + 1,
                'z': z_level + 25,
                'hue': 0
            })
            # Cap at Z=30
            walls.append({
                'tile_id': WALL_STRUCTURE['battlements_ew']['caps'],
                'x': x,
                'y': min_y + walkway_width + 1,
                'z': z_level + 30,
                'hue': 0
            })
        position_index += 1

    # Bottom inner wall (north edge of bottom walkway) - horizontal
    position_index = 0
    for x in range(min_x, max_x + 1):
        # Foundation at Z=0
        walls.append({
            'tile_id': WALL_STRUCTURE['foundation'],
            'x': x,
            'y': max_y - walkway_width - 1,
            'z': z_level,
            'hue': inner_wall_hue
        })
        # Wall body at Z=20
        walls.append({
            'tile_id': WALL_STRUCTURE['wall_body'],
            'x': x,
            'y': max_y - walkway_width - 1,
            'z': z_level + 20,
            'hue': inner_wall_hue
        })
        # Add FULL raised battlements on alternating positions
        if position_index % 2 == 0:
            # Wall body support at Z=25
            walls.append({
                'tile_id': WALL_STRUCTURE['wall_body'],
                'x': x,
                'y': max_y - walkway_width - 1,
                'z': z_level + 25,
                'hue': inner_wall_hue
            })
            # Merlon at Z=25
            walls.append({
                'tile_id': WALL_STRUCTURE['battlements_ew']['merlons'],
                'x': x,
                'y': max_y - walkway_width - 1,
                'z': z_level + 25,
                'hue': 0
            })
            # Cap at Z=30
            walls.append({
                'tile_id': WALL_STRUCTURE['battlements_ew']['caps'],
                'x': x,
                'y': max_y - walkway_width - 1,
                'z': z_level + 30,
                'hue': 0
            })
        position_index += 1

    # Left inner wall (east edge of left walkway) - vertical
    position_index = 0
    for y in range(min_y + walkway_width + 1, max_y - walkway_width):
        # Foundation at Z=0
        walls.append({
            'tile_id': WALL_STRUCTURE['foundation'],
            'x': min_x + walkway_width + 1,
            'y': y,
            'z': z_level,
            'hue': inner_wall_hue
        })
        # Wall body at Z=20
        walls.append({
            'tile_id': WALL_STRUCTURE['wall_body'],
            'x': min_x + walkway_width + 1,
            'y': y,
            'z': z_level + 20,
            'hue': inner_wall_hue
        })
        # Add FULL raised battlements on alternating positions
        if position_index % 2 == 0:
            # Wall body support at Z=25
            walls.append({
                'tile_id': WALL_STRUCTURE['wall_body'],
                'x': min_x + walkway_width + 1,
                'y': y,
                'z': z_level + 25,
                'hue': inner_wall_hue
            })
            # Merlon at Z=25
            walls.append({
                'tile_id': WALL_STRUCTURE['battlements_ns']['merlons'],
                'x': min_x + walkway_width + 1,
                'y': y,
                'z': z_level + 25,
                'hue': 0
            })
            # Cap at Z=30
            walls.append({
                'tile_id': WALL_STRUCTURE['battlements_ns']['caps'],
                'x': min_x + walkway_width + 1,
                'y': y,
                'z': z_level + 30,
                'hue': 0
            })
        position_index += 1

    # Right inner wall (west edge of right walkway) - vertical
    position_index = 0
    for y in range(min_y + walkway_width + 1, max_y - walkway_width):
        # Foundation at Z=0
        walls.append({
            'tile_id': WALL_STRUCTURE['foundation'],
            'x': max_x - walkway_width - 1,
            'y': y,
            'z': z_level,
            'hue': inner_wall_hue
        })
        # Wall body at Z=20
        walls.append({
            'tile_id': WALL_STRUCTURE['wall_body'],
            'x': max_x - walkway_width - 1,
            'y': y,
            'z': z_level + 20,
            'hue': inner_wall_hue
        })
        # Add FULL raised battlements on alternating positions
        if position_index % 2 == 0:
            # Wall body support at Z=25
            walls.append({
                'tile_id': WALL_STRUCTURE['wall_body'],
                'x': max_x - walkway_width - 1,
                'y': y,
                'z': z_level + 25,
                'hue': inner_wall_hue
            })
            # Merlon at Z=25
            walls.append({
                'tile_id': WALL_STRUCTURE['battlements_ns']['merlons'],
                'x': max_x - walkway_width - 1,
                'y': y,
                'z': z_level + 25,
                'hue': 0
            })
            # Cap at Z=30
            walls.append({
                'tile_id': WALL_STRUCTURE['battlements_ns']['caps'],
                'x': max_x - walkway_width - 1,
                'y': y,
                'z': z_level + 30,
                'hue': 0
            })
        position_index += 1

    return walls


def generate_circular_wall(
    center_x: int,
    center_y: int,
    radius: int,
    wall_type: str = 'stone',
    gate_positions: List[Tuple[int, int]] = None,
    z_level: int = 0
) -> List[Dict]:
    """
    Generate a circular wall around a city center.
    
    Args:
        center_x: X coordinate of city center
        center_y: Y coordinate of city center
        radius: Radius of city in tiles
        wall_type: Type of wall ('stone', 'brick', etc.)
        gate_positions: List of (x, y) positions where gates should be (walls skipped)
        z_level: Z-level for walls
    
    Returns:
        List of wall static dictionaries
    """
    if gate_positions is None:
        gate_positions = []
    
    gate_set = set(gate_positions)
    wall_tile_id = WALL_TILES.get(wall_type, WALL_TILES['stone'])
    
    walls = []
    
    # Use Bresenham's circle algorithm to draw perimeter
    x = 0
    y = radius
    d = 3 - 2 * radius
    
    def add_wall_point(px: int, py: int):
        """Add wall point if not a gate"""
        if (px, py) not in gate_set:
            walls.append({
                'tile_id': wall_tile_id,
                'x': center_x + px,
                'y': center_y + py,
                'z': z_level,
                'hue': 0
            })
    
    # Draw circle using 8-way symmetry
    while x <= y:
        # Add 8 symmetric points
        add_wall_point(x, y)
        add_wall_point(-x, y)
        add_wall_point(x, -y)
        add_wall_point(-x, -y)
        add_wall_point(y, x)
        add_wall_point(-y, x)
        add_wall_point(y, -x)
        add_wall_point(-y, -x)
        
        if d < 0:
            d = d + 4 * x + 6
        else:
            d = d + 4 * (x - y) + 10
            y -= 1
        x += 1
    
    return walls


def group_statics_into_blocks(statics: List[Dict]) -> Dict[int, List[Dict]]:
    """
    Group statics into blocks using CentrED's block calculation.
    
    Args:
        statics: List of static dictionaries with world coordinates
    
    Returns:
        Dictionary mapping block_id to list of statics (with local coordinates)
    """
    blocks = {}
    
    for static in statics:
        # Calculate block coordinates
        block_x = static['x'] // 8
        block_y = static['y'] // 8
        
        # Calculate local coordinates within block
        local_x = static['x'] & 0x7  # Same as % 8
        local_y = static['y'] & 0x7
        
        # Use CentrED's block calculation
        block_id = get_block_number(block_x, block_y)
        
        if block_id not in blocks:
            blocks[block_id] = []
        
        blocks[block_id].append({
            'tile_id': static['tile_id'],
            'x': local_x,
            'y': local_y,
            'z': static['z'],
            'hue': static['hue']
        })
    
    return blocks


def load_existing_map(staidx_path: Path, statics_path: Path) -> Dict[int, List[Dict]]:
    """
    Load existing statics from map files.
    
    Returns:
        Dictionary mapping block_id to list of statics
    """
    blocks = {}
    
    with open(staidx_path, 'rb') as idx, open(statics_path, 'rb') as sta:
        for block_id in range(MAP_WIDTH_BLOCKS * MAP_HEIGHT_BLOCKS):
            idx.seek(block_id * 12)
            offset = struct.unpack('<I', idx.read(4))[0]
            length = struct.unpack('<I', idx.read(4))[0]
            
            if offset == 0xFFFFFFFF or length == 0:
                continue
            
            sta.seek(offset)
            count = length // 7
            
            blocks[block_id] = []
            for _ in range(count):
                tile_id = struct.unpack('<H', sta.read(2))[0]
                x = struct.unpack('B', sta.read(1))[0]
                y = struct.unpack('B', sta.read(1))[0]
                z = struct.unpack('b', sta.read(1))[0]
                hue = struct.unpack('<H', sta.read(2))[0]
                blocks[block_id].append({
                    'tile_id': tile_id,
                    'x': x,
                    'y': y,
                    'z': z,
                    'hue': hue
                })
    
    return blocks


def get_centred_map_paths(config_path: Optional[Path] = None) -> Tuple[Optional[Path], Optional[Path], Optional[Path]]:
    """
    Read CentrED configuration and return map file paths.
    
    Args:
        config_path: Path to Cedserver.xml (default: auto-detect)
    
    Returns:
        Tuple of (statics_path, staidx_path, map_path) or (None, None, None) if not found
    """
    if config_path is None:
        # Try default CentrED config location
        default_config = Path(r'C:\DevEnv\GIT\UO\centredsharp\output\Cedserver.xml')
        if default_config.exists():
            config_path = default_config
        else:
            return None, None, None
    
    if not config_path.exists():
        return None, None, None
    
    try:
        tree = ET.parse(config_path)
        root = tree.getroot()
        
        # Find Map element
        map_elem = root.find('Map')
        if map_elem is None:
            return None, None, None
        
        statics_path = map_elem.find('Statics')
        staidx_path = map_elem.find('StaIdx')
        map_path = map_elem.find('Map')
        
        if statics_path is not None and staidx_path is not None:
            return (
                Path(statics_path.text) if statics_path.text else None,
                Path(staidx_path.text) if staidx_path.text else None,
                Path(map_path.text) if map_path is not None and map_path.text else None
            )
    except Exception as e:
        print(f"Error reading CentrED config: {e}")
        return None, None, None
    
    return None, None, None


def backup_map_files(
    statics_path: Path,
    staidx_path: Path,
    backup_dir: Path,
    backup_name: Optional[str] = None
) -> Tuple[Path, Path]:
    """
    Backup map files to a subdirectory.
    
    Args:
        statics_path: Path to statics0.mul
        staidx_path: Path to staidx0.mul
        backup_dir: Directory to store backups (will create subdirectory)
        backup_name: Optional name for backup (default: timestamp)
    
    Returns:
        Tuple of (backup_statics_path, backup_staidx_path)
    """
    if backup_name is None:
        timestamp = datetime.now().strftime("%Y%m%d_%H%M%S")
        backup_name = f"backup_{timestamp}"
    
    backup_subdir = backup_dir / backup_name
    backup_subdir.mkdir(parents=True, exist_ok=True)
    
    backup_statics = backup_subdir / statics_path.name
    backup_staidx = backup_subdir / staidx_path.name
    
    print(f"Backing up map files to: {backup_subdir}")
    shutil.copy2(statics_path, backup_statics)
    shutil.copy2(staidx_path, backup_staidx)
    
    # Also backup map0.mul if it exists in the same directory
    map_path = statics_path.parent / 'map0.mul'
    if map_path.exists():
        backup_map = backup_subdir / 'map0.mul'
        shutil.copy2(map_path, backup_map)
        print(f"  Backed up: map0.mul")
    
    print(f"  Backed up: {statics_path.name}")
    print(f"  Backed up: {staidx_path.name}")
    
    return backup_statics, backup_staidx


def write_map_files(
    blocks: Dict[int, List[Dict]],
    staidx_path: Path,
    statics_path: Path
):
    """
    Write blocks to map files.
    """
    with open(statics_path, 'wb') as sta, open(staidx_path, 'wb') as idx:
        current_offset = 0
        
        for block_id in range(MAP_WIDTH_BLOCKS * MAP_HEIGHT_BLOCKS):
            if block_id in blocks and len(blocks[block_id]) > 0:
                statics = blocks[block_id]
                
                # Write statics
                for s in statics:
                    sta.write(struct.pack('<H', s['tile_id']))
                    sta.write(struct.pack('B', s['x']))
                    sta.write(struct.pack('B', s['y']))
                    sta.write(struct.pack('b', s['z']))
                    sta.write(struct.pack('<H', s['hue']))
                
                # Write index
                length = len(statics) * 7
                idx.write(struct.pack('<I', current_offset))
                idx.write(struct.pack('<I', length))
                idx.write(struct.pack('<I', 0))
                
                current_offset += length
            else:
                # Empty block
                idx.write(struct.pack('<I', 0xFFFFFFFF))
                idx.write(struct.pack('<I', 0xFFFFFFFF))
                idx.write(struct.pack('<I', 0))
            
            if block_id % 50000 == 0 and block_id > 0:
                print(f"  Progress: {block_id}/{MAP_WIDTH_BLOCKS * MAP_HEIGHT_BLOCKS}...")
    
    print(f"Wrote {len([b for b in blocks.values() if len(b) > 0])} blocks")


def get_terrain_z_at(map_path: Path, x: int, y: int) -> int:
    """
    Read terrain Z-level at given coordinates from map0.mul.
    This helps determine the correct Z-level for walls.
    
    Args:
        map_path: Path to map0.mul
        x: World X coordinate
        y: World Y coordinate
    
    Returns:
        Z-level of terrain at (x, y)
    """
    if not map_path.exists():
        return 0
    
    try:
        block_x = x // 8
        block_y = y // 8
        local_x = x & 0x7
        local_y = y & 0x7
        
        # CentrED block calculation
        block_id = block_x * MAP_HEIGHT_BLOCKS + block_y
        
        # Each block is 196 bytes: 4 byte header + 64 cells * 3 bytes
        block_offset = block_id * 196
        cell_offset = block_offset + 4 + (local_y * 8 + local_x) * 3
        
        with open(map_path, 'rb') as f:
            f.seek(cell_offset)
            tile_id = struct.unpack('<H', f.read(2))[0]
            z = struct.unpack('b', f.read(1))[0]
            return z
    except Exception as e:
        print(f"Warning: Could not read terrain Z at ({x}, {y}): {e}")
        return 0


def main():
    """
    Example usage: Generate walls around Ironheart Capital
    """
    print("="*60)
    print("City Wall Generator")
    print("="*60)
    
    # Configuration
    # Ironheart Capital coordinates from config
    CITY_CENTER_X = 800
    CITY_CENTER_Y = 600
    CITY_WIDTH = 200  # tiles
    CITY_HEIGHT = 200  # tiles
    WALL_TYPE = 'stone'
    
    # Gate positions (where walls should be skipped)
    # Example: main gate on south side
    gate_positions = [
        (CITY_CENTER_X, CITY_CENTER_Y + CITY_HEIGHT // 2),  # South gate
        (CITY_CENTER_X, CITY_CENTER_Y - CITY_HEIGHT // 2),  # North gate
    ]
    
    # Map file paths
    target_statics = Path(r'C:\Program Files (x86)\Electronic Arts\Ultima Online Classic\statics0.mul')
    target_staidx = Path(r'C:\Program Files (x86)\Electronic Arts\Ultima Online Classic\staidx0.mul')
    
    # Alternative CentrED path (uncomment if needed)
    # target_statics = Path(r'C:\DevEnv\GIT\UO\centredsharp\output\editable_maps\statics0.mul')
    # target_staidx = Path(r'C:\DevEnv\GIT\UO\centredsharp\output\editable_maps\staidx0.mul')
    
    print(f"\nCity: Ironheart Capital")
    print(f"Center: ({CITY_CENTER_X}, {CITY_CENTER_Y})")
    print(f"Size: {CITY_WIDTH}x{CITY_HEIGHT} tiles")
    print(f"Wall type: {WALL_TYPE}")
    print(f"Gates: {len(gate_positions)} positions\n")
    
    # Step 1: Generate wall statics
    print("Generating wall perimeter...")
    walls = generate_wall_perimeter(
        CITY_CENTER_X,
        CITY_CENTER_Y,
        CITY_WIDTH,
        CITY_HEIGHT,
        WALL_TYPE,
        gate_positions,
        z_level=0
    )
    print(f"Generated {len(walls)} wall pieces")
    
    # Step 2: Group into blocks
    print("Grouping into blocks...")
    wall_blocks = group_statics_into_blocks(walls)
    print(f"Walls span {len(wall_blocks)} blocks")
    
    # Step 3: Load existing map
    print("Loading existing map...")
    existing_blocks = load_existing_map(target_staidx, target_statics)
    print(f"Loaded {len(existing_blocks)} existing blocks")
    
    # Step 4: Merge walls with existing blocks
    print("Merging walls with existing map...")
    for block_id, statics in wall_blocks.items():
        if block_id in existing_blocks:
            # Add walls to existing statics
            existing_blocks[block_id].extend(statics)
        else:
            # New block
            existing_blocks[block_id] = statics
    
    # Step 5: Write to files
    print("Writing to map files...")
    write_map_files(existing_blocks, target_staidx, target_statics)
    
    print("\n" + "="*60)
    print("WALL GENERATION COMPLETE!")
    print("="*60)
    print(f"Walls added around ({CITY_CENTER_X}, {CITY_CENTER_Y})")
    print(f"Total wall pieces: {len(walls)}")
    print("\nNote: Restart CentrED and ServUO to see changes")


if __name__ == '__main__':
    main()

