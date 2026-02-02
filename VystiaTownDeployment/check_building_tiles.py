"""Check if buildings are in terrain or statics section"""
import json

data = json.load(open('britain_blueprint/britain_complete.json'))
terrain = data['items'][:data['terrain_count']]
statics = data['items'][data['terrain_count']:]

# Building tile ID ranges
building_walls = set(range(0x0080, 0x0090)) | set(range(0x003D, 0x0045))
building_floors = set(range(0x0495, 0x04B0))
building_roofs = set(range(0x06A5, 0x06E0))
building_doors = set(range(0x0675, 0x0685))
all_building_ids = building_walls | building_floors | building_roofs | building_doors

terrain_buildings = [t for t in terrain if t['tile_id'] in all_building_ids]
static_buildings = [s for s in statics if s['tile_id'] in all_building_ids]

print(f'Building pieces in TERRAIN section: {len(terrain_buildings)}')
print(f'Building pieces in STATICS section: {len(static_buildings)}')
print(f'\nTerrain section tile ID range: {min([t["tile_id"] for t in terrain])} to {max([t["tile_id"] for t in terrain])}')
print(f'Statics section tile ID range: {min([s["tile_id"] for s in statics])} to {max([s["tile_id"] for s in statics])}')

if terrain_buildings:
    print(f'\nSample building tile IDs in TERRAIN: {sorted(set([t["tile_id"] for t in terrain_buildings[:100]]))[:30]}')

if static_buildings:
    print(f'\nSample building tile IDs in STATICS: {sorted(set([s["tile_id"] for s in static_buildings[:100]]))[:30]}')
