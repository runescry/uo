"""
Create a simple house using actual static ITEM IDs
Not from multi.mul - built from scratch with proper items
"""

def create_simple_brick_house():
    """
    Create a simple 7x7 brick house with door
    Uses actual item IDs that will render correctly
    """

    house = []

    # House dimensions
    size = 7
    center_x = 0
    center_y = 0
    floor_z = 0
    wall_z = 0  # Walls at same level as floor - graphic height makes them tall

    # Item IDs (these are REAL static items that will render)
    STONE_FLOOR = 0x0495  # Stone floor tile
    BRICK_WALL_NS = 0x003D  # Brick wall (north-south)
    BRICK_WALL_EW = 0x003C  # Brick wall (east-west)
    WOOD_DOOR = 0x0675  # Wooden door facing south
    ROOF_TILE = 0x0547  # Roof tile

    # Floor (7x7)
    for x in range(size):
        for y in range(size):
            house.append({
                'tile_id': STONE_FLOOR,
                'x': center_x + x - size//2,
                'y': center_y + y - size//2,
                'z': floor_z,
                'hue': 0
            })

    # Walls - North side
    for x in range(size):
        house.append({
            'tile_id': BRICK_WALL_EW,
            'x': center_x + x - size//2,
            'y': center_y - size//2,
            'z': wall_z,
            'hue': 0
        })

    # Walls - South side (with door in middle)
    for x in range(size):
        if x == size // 2:  # Door position
            house.append({
                'tile_id': WOOD_DOOR,
                'x': center_x + x - size//2,
                'y': center_y + size//2,
                'z': wall_z,
                'hue': 0
            })
        else:
            house.append({
                'tile_id': BRICK_WALL_EW,
                'x': center_x + x - size//2,
                'y': center_y + size//2,
                'z': wall_z,
                'hue': 0
            })

    # Walls - West side
    for y in range(1, size-1):  # Skip corners
        house.append({
            'tile_id': BRICK_WALL_NS,
            'x': center_x - size//2,
            'y': center_y + y - size//2,
            'z': wall_z,
            'hue': 0
        })

    # Walls - East side
    for y in range(1, size-1):  # Skip corners
        house.append({
            'tile_id': BRICK_WALL_NS,
            'x': center_x + size//2,
            'y': center_y + y - size//2,
            'z': wall_z,
            'hue': 0
        })

    return house

def save_house_template(filename: str):
    """Save house template to file"""
    house = create_simple_brick_house()

    with open(filename, 'w') as f:
        f.write("# Simple 7x7 Brick House\n")
        f.write("# Format: TileID X Y Z Hue\n")
        for tile in house:
            f.write(f"0x{tile['tile_id']:X} {tile['x']} {tile['y']} {tile['z']} {tile['hue']}\n")

    print(f"Created house template: {filename}")
    print(f"Total tiles: {len(house)}")
    print(f"  Floor: {sum(1 for t in house if t['z'] == 0)}")
    print(f"  Walls: {sum(1 for t in house if t['z'] == 7)}")

if __name__ == '__main__':
    save_house_template(r'C:\DevEnv\GIT\UO\Vystia Town Generator\brick_house_7x7.txt')
