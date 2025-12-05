import struct
import json
from pathlib import Path

# Extract Vystia data at Britain deployment location
map_path = Path(r'C:\DevEnv\GIT\UO\UOL 1.5\map0.mul')
statics_path = Path(r'C:\Program Files (x86)\Electronic Arts\Ultima Online Classic\statics0.mul')
staidx_path = Path(r'C:\Program Files (x86)\Electronic Arts\Ultima Online Classic\staidx0.mul')

print('Extracting Vystia Z-level data at Britain deployment location...')
print('='*60)

# Britain deployment bounds (from deploy script)
# Target center: (1871, 700)
# Target bounds: (1696, 550) to (2046, 850)
min_x, min_y = 1696, 550
max_x, max_y = 2046, 850

vystia_data = {
    'name': 'Vystia Britain Deployment Area',
    'bounds': {'x1': min_x, 'y1': min_y, 'x2': max_x, 'y2': max_y},
    'center': {'x': 1871, 'y': 700},
    'terrain': [],
    'statics': []
}

# Step 1: Extract Vystia terrain Z-levels
print(f'Extracting Vystia terrain from ({min_x}, {min_y}) to ({max_x}, {max_y})...')

with open(map_path, 'rb') as map_file:
    for y in range(min_y, max_y + 1):
        for x in range(min_x, max_x + 1):
            block_x = x // 8
            block_y = y // 8
            cell_x = x % 8
            cell_y = y % 8

            # Vystia uses CentrED block calculation
            block_id = block_x * 512 + block_y
            block_offset = block_id * 196
            cell_offset = block_offset + 4 + (cell_y * 8 + cell_x) * 3

            map_file.seek(cell_offset)
            tile_id = struct.unpack('<H', map_file.read(2))[0]
            z = struct.unpack('b', map_file.read(1))[0]

            vystia_data['terrain'].append({
                'x': x,
                'y': y,
                'tile_id': tile_id,
                'z': z
            })

print(f'  Extracted {len(vystia_data["terrain"])} terrain tiles')

# Step 2: Extract deployed Britain statics
print(f'Extracting deployed statics...')

statics_by_coord = {}

with open(staidx_path, 'rb') as idx, open(statics_path, 'rb') as sta:
    min_block_x = min_x // 8
    max_block_x = max_x // 8
    min_block_y = min_y // 8
    max_block_y = max_y // 8

    for block_y in range(min_block_y, max_block_y + 1):
        for block_x in range(min_block_x, max_block_x + 1):
            # Vystia uses CentrED block calculation
            block_id = block_x * 512 + block_y

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

                world_x = block_x * 8 + local_x
                world_y = block_y * 8 + local_y

                if min_x <= world_x <= max_x and min_y <= world_y <= max_y:
                    coord_key = (world_x, world_y)
                    if coord_key not in statics_by_coord:
                        statics_by_coord[coord_key] = []

                    statics_by_coord[coord_key].append({
                        'tile_id': tile_id,
                        'z': z,
                        'hue': hue
                    })

for (x, y), statics_list in statics_by_coord.items():
    vystia_data['statics'].append({
        'x': x,
        'y': y,
        'statics': statics_list
    })

print(f'  Extracted statics for {len(vystia_data["statics"])} coordinates')

# Step 3: Statistics
terrain_z = [t['z'] for t in vystia_data['terrain']]
all_static_z = []
for s in vystia_data['statics']:
    for static in s['statics']:
        all_static_z.append(static['z'])

vystia_data['statistics'] = {
    'total_terrain_tiles': len(vystia_data['terrain']),
    'total_statics_coordinates': len(vystia_data['statics']),
    'total_statics': len(all_static_z),
    'terrain_z_range': [min(terrain_z), max(terrain_z)],
    'terrain_z_avg': sum(terrain_z) / len(terrain_z),
    'statics_z_range': [min(all_static_z), max(all_static_z)] if all_static_z else [0, 0],
    'statics_z_avg': sum(all_static_z) / len(all_static_z) if all_static_z else 0
}

# Step 4: Load OSI Britain data for comparison
with open('britain_complete_zlevel_data.json', 'r') as f:
    osi_britain = json.load(f)

# Add comparison
vystia_data['comparison'] = {
    'osi_britain_terrain_z_avg': osi_britain['statistics']['terrain_z_avg'],
    'osi_britain_statics_z_avg': osi_britain['statistics']['statics_z_avg'],
    'osi_statics_relative_to_terrain': osi_britain['statistics']['statics_z_avg'] - osi_britain['statistics']['terrain_z_avg'],
    'vystia_terrain_z_avg': vystia_data['statistics']['terrain_z_avg'],
    'vystia_statics_z_avg': vystia_data['statistics']['statics_z_avg'],
    'vystia_statics_relative_to_terrain': vystia_data['statistics']['statics_z_avg'] - vystia_data['statistics']['terrain_z_avg'],
    'terrain_z_difference': vystia_data['statistics']['terrain_z_avg'] - osi_britain['statistics']['terrain_z_avg'],
    'statics_z_difference': vystia_data['statistics']['statics_z_avg'] - osi_britain['statistics']['statics_z_avg']
}

# Save to JSON
output_path = Path('vystia_britain_deployment_zlevel_data.json')
with open(output_path, 'w') as f:
    json.dump(vystia_data, f, indent=2)

print(f'\n' + '='*60)
print(f'COMPLETE!')
print(f'='*60)
print(f'Saved to: {output_path}')
print(f'\nVYSTIA DEPLOYMENT AREA:')
print(f'  Terrain tiles: {vystia_data["statistics"]["total_terrain_tiles"]}')
print(f'  Statics coordinates: {vystia_data["statistics"]["total_statics_coordinates"]}')
print(f'  Total statics: {vystia_data["statistics"]["total_statics"]}')
print(f'  Terrain Z range: {vystia_data["statistics"]["terrain_z_range"]}')
print(f'  Terrain Z avg: {vystia_data["statistics"]["terrain_z_avg"]:.1f}')
print(f'  Statics Z range: {vystia_data["statistics"]["statics_z_range"]}')
print(f'  Statics Z avg: {vystia_data["statistics"]["statics_z_avg"]:.1f}')

print(f'\nCOMPARISON WITH OSI BRITAIN:')
print(f'  OSI terrain avg Z: {vystia_data["comparison"]["osi_britain_terrain_z_avg"]:.1f}')
print(f'  OSI statics avg Z: {vystia_data["comparison"]["osi_britain_statics_z_avg"]:.1f}')
print(f'  OSI statics relative to terrain: {vystia_data["comparison"]["osi_statics_relative_to_terrain"]:+.1f}')
print(f'  ---')
print(f'  Vystia terrain avg Z: {vystia_data["comparison"]["vystia_terrain_z_avg"]:.1f}')
print(f'  Vystia deployed statics avg Z: {vystia_data["comparison"]["vystia_statics_z_avg"]:.1f}')
print(f'  Vystia statics relative to terrain: {vystia_data["comparison"]["vystia_statics_relative_to_terrain"]:+.1f}')
print(f'  ---')
print(f'  Terrain Z difference (Vystia - OSI): {vystia_data["comparison"]["terrain_z_difference"]:+.1f}')
print(f'  Statics Z difference (Vystia - OSI): {vystia_data["comparison"]["statics_z_difference"]:+.1f}')
