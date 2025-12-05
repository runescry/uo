# Quick Start: Generate City Walls

## What You Need

1. **City center coordinates** (X, Y) - from CentrED#
2. **City size** - width/height for rectangular, or radius for circular
3. **Optional:** Gate positions, wall type, Z-level

## Quick Example

Generate walls around Ironheart Capital with automatic backup:

```bash
python generate_city_walls_cli.py --center-x 800 --center-y 600 --width 200 --height 200 --auto-z
```

## What Happens Automatically

✅ **Auto-detects CentrED map location** from `Cedserver.xml`  
✅ **Backs up map files** to `backups/` directory before writing  
✅ **Auto-detects Z-level** from terrain (if using `--auto-z`)  
✅ **Merges with existing map** data  

## Minimum Required Inputs

**Rectangular:**
```bash
--center-x X --center-y Y --width W --height H
```

**Circular:**
```bash
--center-x X --center-y Y --radius R --circular
```

## Finding Your Coordinates

1. Open CentrED# client
2. Navigate to city center
3. Read X, Y from status bar
4. Use those exact coordinates

## Full Example with Gates

```bash
python generate_city_walls_cli.py \
  --center-x 800 \
  --center-y 600 \
  --width 200 \
  --height 200 \
  --auto-z \
  --gates "800,500 800,700" \
  --wall-type stone
```

Creates:
- Walls around (800, 600)
- 200x200 rectangle
- Auto Z-level from terrain
- Gates at north (800, 500) and south (800, 700)
- Stone wall type

## Test First (Dry Run)

Preview without writing files:

```bash
python generate_city_walls_cli.py --center-x 800 --center-y 600 --width 200 --height 200 --dry-run
```

## After Generating

1. **Restart CentrED# server**
2. **Restart ServUO server**
3. **Open CentrED# client** and check walls

## Backup Location

Backups are automatically saved to:
```
Vystia Town Generator/backups/backup_YYYYMMDD_HHMMSS/
```

## Need More Help?

- `docs/WALL_PLACEMENT_INPUTS.md` - Detailed input guide
- `docs/CITY_WALL_GENERATION.md` - Full documentation
- `python generate_city_walls_cli.py --help` - Command help

