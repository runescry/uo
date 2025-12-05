# City Wall Generation Guide

This guide explains how to automatically surround cities with walls on your UO map using Python scripts.

## Overview

The wall generation tools allow you to:
- Generate rectangular or circular walls around cities
- Specify gate positions where walls are skipped
- Choose different wall types (stone, brick, wood, etc.)
- Automatically write walls to map files using CentrED's correct block calculation

## Quick Start

### Basic Rectangular Wall

Generate a rectangular wall around Ironheart Capital:

```bash
python generate_city_walls_cli.py --center-x 800 --center-y 600 --width 200 --height 200
```

### Circular Wall

Generate a circular wall:

```bash
python generate_city_walls_cli.py --center-x 1200 --center-y 200 --radius 100 --circular
```

### With Gates

Add gates at specific positions (walls will be skipped):

```bash
python generate_city_walls_cli.py --center-x 800 --center-y 600 --width 200 --height 200 --gates "800,500 800,700"
```

This creates gates at:
- (800, 500) - North gate
- (800, 700) - South gate

### Custom Wall Type

Use brick walls instead of stone:

```bash
python generate_city_walls_cli.py --center-x 1800 --center-y 400 --width 150 --height 150 --wall-type brick
```

### Dry Run (Preview)

See what would be generated without writing files:

```bash
python generate_city_walls_cli.py --center-x 800 --center-y 600 --width 200 --height 200 --dry-run
```

## Available Wall Types

- `stone` - Standard stone wall (most common)
- `stone_thick` - Thick stone wall
- `stone_thin` - Thin stone wall
- `brick` - Brick wall
- `wood` - Wooden wall
- `marble` - Marble wall
- `stone_ruined` - Ruined stone wall

## Using the Python Module

You can also use the wall generator as a Python module:

```python
from generate_city_walls import generate_wall_perimeter, WALL_TILES

# Generate walls
walls = generate_wall_perimeter(
    center_x=800,
    center_y=600,
    width=200,
    height=200,
    wall_type='stone',
    gate_positions=[(800, 500), (800, 700)],
    z_level=0
)

print(f"Generated {len(walls)} wall pieces")
```

## City Coordinates Reference

Based on `VYSTIA_WORLD_CONFIG.json`:

| City | Center X | Center Y | Suggested Size |
|------|----------|----------|----------------|
| Ironheart Capital | 800 | 600 | 200x200 |
| Frostholm | 1200 | 200 | 150x150 |
| Verdant Grove | 1800 | 400 | 180x180 |
| Deepforge City | 2400 | 1400 | 200x200 |
| Sandara | 4200 | 2600 | 180x180 |
| Emberforge | 5400 | 1800 | 150x150 |
| Pearl Harbor | 3600 | 3400 | 160x160 |
| Crystal Spire | 4800 | 2200 | 140x140 |

## Output Locations

By default, walls are written to:
- `C:\Program Files (x86)\Electronic Arts\Ultima Online Classic\statics0.mul`
- `C:\Program Files (x86)\Electronic Arts\Ultima Online Classic\staidx0.mul`

To write to CentrED's location instead:

```bash
python generate_city_walls_cli.py --center-x 800 --center-y 600 --width 200 --height 200 --output-dir "C:\DevEnv\GIT\UO\centredsharp\output\editable_maps"
```

## Important Notes

1. **Backup First**: Always backup your map files before generating walls
2. **Restart Required**: You must restart both CentrED and ServUO after generating walls
3. **Block Calculation**: The script uses CentrED's correct formula: `blockID = blockX * 512 + blockY`
4. **Z-Level**: Default is 0. Adjust if your terrain is at a different height
5. **Gate Positions**: Gates are specified in world coordinates, not local coordinates

## Troubleshooting

### "File not found" Error

Make sure UO Classic is installed or specify `--output-dir`:

```bash
python generate_city_walls_cli.py --output-dir "C:\path\to\map\files" ...
```

### Walls Not Appearing

1. Restart CentrED server
2. Restart ServUO server
3. Check that coordinates are within map bounds (0-7167 X, 0-4095 Y)
4. Verify Z-level matches terrain height

### Wrong Wall Positions

- Check that you're using world coordinates, not block coordinates
- Verify city center coordinates are correct
- Use `--dry-run` to preview before writing

## Advanced Usage

### Multiple Cities

Generate walls for multiple cities in a batch:

```bash
# Ironheart Capital
python generate_city_walls_cli.py --center-x 800 --center-y 600 --width 200 --height 200

# Frostholm
python generate_city_walls_cli.py --center-x 1200 --center-y 200 --width 150 --height 150 --wall-type stone_thick

# Verdant Grove
python generate_city_walls_cli.py --center-x 1800 --center-y 400 --width 180 --height 180 --wall-type wood
```

### Custom Script

Create a custom Python script for your specific needs:

```python
from generate_city_walls import generate_wall_perimeter, group_statics_into_blocks, load_existing_map, write_map_files
from pathlib import Path

# Define cities
cities = [
    {'name': 'Ironheart', 'x': 800, 'y': 600, 'w': 200, 'h': 200},
    {'name': 'Frostholm', 'x': 1200, 'y': 200, 'w': 150, 'h': 150},
]

# Generate walls for all cities
all_walls = []
for city in cities:
    walls = generate_wall_perimeter(city['x'], city['y'], city['w'], city['h'])
    all_walls.extend(walls)
    print(f"Generated {len(walls)} walls for {city['name']}")

# Write to map
wall_blocks = group_statics_into_blocks(all_walls)
target_staidx = Path(r'C:\Program Files (x86)\Electronic Arts\Ultima Online Classic\staidx0.mul')
target_statics = Path(r'C:\Program Files (x86)\Electronic Arts\Ultima Online Classic\statics0.mul')

existing_blocks = load_existing_map(target_staidx, target_statics)
for block_id, statics in wall_blocks.items():
    if block_id in existing_blocks:
        existing_blocks[block_id].extend(statics)
    else:
        existing_blocks[block_id] = statics

write_map_files(existing_blocks, target_staidx, target_statics)
print("All walls generated!")
```

## See Also

- `deploy_britain_correct.py` - Example of writing statics to map files
- `VYSTIA_WORLD_CONFIG.json` - City locations and configurations
- CentrED# documentation - Map editing tool

