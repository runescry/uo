"""
Create a terrain height mapping between OSI Britain and Vystia deployment location.
For each tile in Britain, map its OSI terrain Z to the Vystia terrain Z at the deployment location.
This allows us to adjust statics relative to their local terrain.
"""

import struct
import json
from pathlib import Path

# Source: OSI Britain
osi_map = Path(r'C:\Ultima Online\map0.mul')

# Target: Vystia map
vystia_map = Path(r'C:\DevEnv\GIT\UO\UOL 1.5\map0.mul')

# Britain bounds (OSI)
osi_min_x, osi_min_y = 1400, 1500
osi_max_x, osi_max_y = 1750, 1800

# Deployment shift (from deploy script)
osi_center_x = 1575
osi_center_y = 1650
target_x = 1871
target_y = 700
shift_x = target_x - osi_center_x  # 296
shift_y = target_y - osi_center_y  # -950

print('='*60)
print('CREATING TERRAIN HEIGHT MAP')
print('='*60)
print(f'OSI Britain: ({osi_min_x}, {osi_min_y}) to ({osi_max_x}, {osi_max_y})')
print(f'Shift: ({shift_x}, {shift_y})')
print(f'Vystia target: ({osi_min_x + shift_x}, {osi_min_y + shift_y}) to ({osi_max_x + shift_x}, {osi_max_y + shift_y})')
print()

# Store mapping data
terrain_map = []

def read_terrain_z(map_file, x, y, use_centred_formula=False):
    """Read terrain Z at given coordinates"""
    block_x = x // 8
    block_y = y // 8
    cell_x = x % 8
    cell_y = y % 8

    if use_centred_formula:
        # CentrED formula for Vystia
        block_id = block_x * 512 + block_y
    else:
        # OSI formula
        block_id = block_x + (block_y * 896)

    block_offset = block_id * 196
    cell_offset = block_offset + 4 + (cell_y * 8 + cell_x) * 3

    map_file.seek(cell_offset)
    tile_id = struct.unpack('<H', map_file.read(2))[0]
    z = struct.unpack('b', map_file.read(1))[0]
    return tile_id, z

print('Mapping terrain heights...')

with open(osi_map, 'rb') as osi_file, open(vystia_map, 'rb') as vystia_file:
    tile_count = 0

    for osi_y in range(osi_min_y, osi_max_y + 1):
        for osi_x in range(osi_min_x, osi_max_x + 1):
            # Read OSI terrain
            osi_tile_id, osi_z = read_terrain_z(osi_file, osi_x, osi_y, use_centred_formula=False)

            # Calculate Vystia coordinates
            vystia_x = osi_x + shift_x
            vystia_y = osi_y + shift_y

            # Read Vystia terrain
            vystia_tile_id, vystia_z = read_terrain_z(vystia_file, vystia_x, vystia_y, use_centred_formula=True)

            # Calculate adjustment needed
            z_adjustment = vystia_z - osi_z

            terrain_map.append({
                'osi_x': osi_x,
                'osi_y': osi_y,
                'osi_tile_id': osi_tile_id,
                'osi_z': osi_z,
                'vystia_x': vystia_x,
                'vystia_y': vystia_y,
                'vystia_tile_id': vystia_tile_id,
                'vystia_z': vystia_z,
                'z_adjustment': z_adjustment
            })

            tile_count += 1
            if tile_count % 10000 == 0:
                print(f'  Mapped {tile_count} tiles...')

print(f'Mapped {len(terrain_map)} tiles')

# Calculate statistics
osi_z_values = [t['osi_z'] for t in terrain_map]
vystia_z_values = [t['vystia_z'] for t in terrain_map]
adjustments = [t['z_adjustment'] for t in terrain_map]

stats = {
    'total_tiles': len(terrain_map),
    'osi_z_range': [min(osi_z_values), max(osi_z_values)],
    'osi_z_avg': sum(osi_z_values) / len(osi_z_values),
    'vystia_z_range': [min(vystia_z_values), max(vystia_z_values)],
    'vystia_z_avg': sum(vystia_z_values) / len(vystia_z_values),
    'adjustment_range': [min(adjustments), max(adjustments)],
    'adjustment_avg': sum(adjustments) / len(adjustments)
}

# Save to JSON
output = {
    'shift': {'x': shift_x, 'y': shift_y},
    'statistics': stats,
    'terrain_map': terrain_map
}

output_path = Path('britain_terrain_height_map.json')
with open(output_path, 'w') as f:
    json.dump(output, f, indent=2)

print(f'\n' + '='*60)
print('COMPLETE!')
print('='*60)
print(f'Saved to: {output_path}')
print(f'\nSTATISTICS:')
print(f'  Total tiles mapped: {stats["total_tiles"]}')
print(f'  OSI terrain Z: {stats["osi_z_range"][0]} to {stats["osi_z_range"][1]} (avg: {stats["osi_z_avg"]:.1f})')
print(f'  Vystia terrain Z: {stats["vystia_z_range"][0]} to {stats["vystia_z_range"][1]} (avg: {stats["vystia_z_avg"]:.1f})')
print(f'  Z adjustment needed: {stats["adjustment_range"][0]:+d} to {stats["adjustment_range"][1]:+d} (avg: {stats["adjustment_avg"]:+.1f})')
print(f'\nThis map can be used to adjust each static based on its local terrain difference.')
