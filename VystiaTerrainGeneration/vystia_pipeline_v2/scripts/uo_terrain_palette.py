"""
UO Terrain Palette Mapping
===========================
Defines the exact RGB colors and palette indices from UOL's Terrain.xml
These MUST be used in the Terrain.bmp for UOMapMake to work correctly.
"""

# Format: terrain_id: (name, R, G, B, base_altitude, tile_id)
UO_TERRAIN_PALETTE = {
    0: ("No Draw", 50, 65, 75, 216, 2),
    1: ("Grass", 0, 100, 0, 88, 3),
    2: ("Furrows N", 130, 100, 50, 88, 9),
    3: ("Furrows W", 130, 100, 85, 88, 336),
    4: ("Sand", 227, 191, 51, 88, 22),
    5: ("Deep Water", 0, 0, 130, 24, 168),
    6: ("Shallow Water", 0, 0, 170, 24, 102),
    7: ("Large Dirt", 120, 110, 90, 88, 113),
    8: ("Small Dirt", 135, 125, 90, 88, 117),
    9: ("Water", 0, 110, 255, 24, 168),
    10: ("Jungle", 0, 75, 75, 88, 172),
    11: ("Forest", 0, 110, 90, 88, 196),
    12: ("Snow", 255, 255, 255, 88, 282),
    13: ("Lava", 255, 0, 0, 88, 500),
    14: ("Stars", 75, 75, 100, 88, 506),
    18: ("Rock", 75, 75, 75, 152, 556),
    19: ("Blank", 0, 0, 0, 88, 580),
    20: ("Mountain Cave", 100, 95, 0, 216, 581),
    21: ("Dungeon Cave", 100, 95, 80, 216, 581),
    # Add more as needed
    41: ("Swamp", 50, 210, 240, 88, 15849),
    42: ("Grass Embankment", 0, 120, 0, 216, 3),
    50: ("Moss", 60, 190, 240, 88, 15853),
    62: ("Grass Without Static", 0, 200, 0, 88, 3),
    80: ("Beach", 255, 255, 192, 216, 22),
}

# Suggested mapping from Vystia regions to UO terrain types
REGION_TO_TERRAIN = {
    "Ocean": 5,  # Deep Water
    "Frosthold": 12,  # Snow
    "Winterguard": 12,  # Snow
    "Verdantpeak": 11,  # Forest
    "Skyreach Peak": 18,  # Rock (mountains)
    "Ironclad Empire": 18,  # Rock
    "Radiant Plains": 1,  # Grass
    "Hollow Forests": 11,  # Forest
    "Mystic Canyons": 18,  # Rock
    "Deepforge": 20,  # Mountain Cave
    "Emberlands": 4,  # Sand (desert)
    "Whispering Sands": 4,  # Sand
    "Blazing Frontier": 4,  # Sand
    "Golden Steppe": 1,  # Grass
    "Shadowfen": 41,  # Swamp
    "Wilderlands": 11,  # Forest
    "Forgotten Depths": 21,  # Dungeon Cave
    "Verdant Isles": 11,  # Forest
    "Glimmering Archipelago": 6,  # Shallow Water
    "Eternal Twilight": 11,  # Forest
    "Crystal Barrens": 18,  # Rock
    "Sunken Isles": 6,  # Shallow Water
}

def get_terrain_rgb(terrain_id):
    """Get RGB tuple for a terrain ID"""
    if terrain_id in UO_TERRAIN_PALETTE:
        _, r, g, b, _, _ = UO_TERRAIN_PALETTE[terrain_id]
        return (r, g, b)
    return (0, 0, 0)

def get_terrain_base_altitude(terrain_id):
    """Get base altitude for a terrain ID"""
    if terrain_id in UO_TERRAIN_PALETTE:
        _, _, _, _, base, _ = UO_TERRAIN_PALETTE[terrain_id]
        return base
    return 88
