# Wall Placement Inputs Guide

This guide explains what inputs you need to provide to ensure walls are placed correctly on your map.

## Required Inputs

### 1. City Center Coordinates (`--center-x`, `--center-y`)

**What it is:** The X and Y coordinates of the city center in **world tile coordinates**.

**How to find:**
- Open CentrED# and navigate to your city
- Look at the bottom status bar or coordinates display
- Use the exact X, Y coordinates shown

**Example:**
```
--center-x 800 --center-y 600
```

**Important Notes:**
- Coordinates are in **tiles**, not blocks
- Valid range: X: 0-7167, Y: 0-4095 (for 896x512 block map)
- These are the **exact coordinates** where you want the city center

### 2. City Size

#### For Rectangular Walls:
- `--width`: Width of city in tiles (e.g., `200`)
- `--height`: Height of city in tiles (e.g., `200`)

#### For Circular Walls:
- `--radius`: Radius of city in tiles (e.g., `100`)
- `--circular`: Flag to indicate circular shape

**How to determine size:**
1. In CentrED#, measure the distance from city center to edge
2. Multiply by 2 for width/height (or use radius directly for circular)
3. Add some buffer (e.g., 20-50 tiles) for wall thickness

**Example:**
```bash
# Rectangular: 200 tiles wide, 200 tiles tall
--width 200 --height 200

# Circular: 100 tile radius
--radius 100 --circular
```

### 3. Z-Level (`--z-level` or `--auto-z`)

**What it is:** The vertical height (elevation) where walls should be placed.

**Options:**

**Option A: Auto-detect (Recommended)**
```bash
--auto-z
```
- Automatically reads terrain Z-level from map0.mul at city center
- Best for most cases - walls will match terrain height

**Option B: Manual**
```bash
--z-level 0
```
- Specify exact Z-level
- Use if terrain is flat or you know the exact height
- Common values: 0 (sea level), 20 (hills), etc.

**How to find manually:**
1. In CentrED#, check terrain height at city center
2. Walls should match terrain Z-level
3. If terrain varies, use average or highest point

### 4. Gate Positions (`--gates`) - Optional

**What it is:** Coordinates where walls should be skipped (for gates/entrances).

**Format:** Space-separated list of "x,y" coordinates

**Example:**
```bash
--gates "800,500 800,700 700,600"
```

This creates gates at:
- (800, 500) - North gate
- (800, 700) - South gate  
- (700, 600) - West gate

**How to determine:**
- Decide where roads/entrances should be
- Use coordinates along the wall perimeter
- For rectangular walls: typically at center of each side
- For circular walls: use coordinates on the circle perimeter

**Tip:** Use `--dry-run` first to preview wall placement, then add gates where needed.

### 5. Wall Type (`--wall-type`) - Optional

**What it is:** The visual style of the wall.

**Available types:**
- `stone` (default) - Standard stone wall
- `stone_thick` - Thick stone wall
- `stone_thin` - Thin stone wall
- `brick` - Brick wall
- `wood` - Wooden wall
- `marble` - Marble wall
- `stone_ruined` - Ruined stone wall

**Example:**
```bash
--wall-type brick
```

## Complete Example

Here's a complete example with all inputs:

```bash
python generate_city_walls_cli.py \
  --center-x 800 \
  --center-y 600 \
  --width 200 \
  --height 200 \
  --auto-z \
  --gates "800,500 800,700 700,600 900,600" \
  --wall-type stone
```

This will:
- Place walls around city at (800, 600)
- Create 200x200 rectangular walls
- Auto-detect Z-level from terrain
- Create 4 gates (N, S, E, W)
- Use stone wall type

## Quick Reference: Minimum Required Inputs

**Rectangular wall:**
```bash
--center-x X --center-y Y --width W --height H
```

**Circular wall:**
```bash
--center-x X --center-y Y --radius R --circular
```

Everything else is optional and will use sensible defaults.

## Finding Coordinates in CentrED#

### Method 1: Status Bar
1. Open CentrED# client
2. Navigate to your city location
3. Look at bottom status bar - shows current X, Y coordinates

### Method 2: Go Command
1. In CentrED#, use `[Go X Y` command
2. Navigate to city center
3. Note the coordinates

### Method 3: Measure Distance
1. Place a marker at city center
2. Measure to edge (count tiles or use ruler tool)
3. Calculate center: `center = edge - distance`

## Common Mistakes to Avoid

### ❌ Wrong: Using Block Coordinates
```bash
--center-x 100 --center-y 75  # Blocks, not tiles!
```
**Correct:** Use tile coordinates (multiply blocks by 8)
```bash
--center-x 800 --center-y 600  # Tiles
```

### ❌ Wrong: Z-level Mismatch
```bash
--z-level 0  # But terrain is at Z=20!
```
**Correct:** Use `--auto-z` or match terrain height
```bash
--auto-z  # Auto-detect
# OR
--z-level 20  # Match terrain
```

### ❌ Wrong: Gates Outside Wall Perimeter
```bash
--center-x 800 --center-y 600 --width 200 --height 200
--gates "1000,800"  # This is outside the wall!
```
**Correct:** Gates must be on the wall perimeter
```bash
--gates "800,500 800,700"  # On north/south walls
```

## Testing Before Writing

Always use `--dry-run` first to preview:

```bash
python generate_city_walls_cli.py \
  --center-x 800 \
  --center-y 600 \
  --width 200 \
  --height 200 \
  --dry-run
```

This shows:
- How many wall pieces will be generated
- Which blocks will be affected
- No files are written

## Backup

The script automatically backs up map files before writing. Backups are stored in:
```
Vystia Town Generator/backups/backup_YYYYMMDD_HHMMSS/
```

To disable backup (not recommended):
```bash
--no-backup
```

To use custom backup location:
```bash
--backup-dir "C:\path\to\backups"
```

## Troubleshooting

### Walls Not Appearing
1. **Check coordinates:** Verify city center is correct
2. **Check Z-level:** Walls might be underground or floating
   - Use `--auto-z` to match terrain
   - Or check terrain height manually
3. **Restart servers:** Must restart CentrED and ServUO after writing

### Wrong Wall Positions
1. **Verify coordinates:** Use exact tile coordinates from CentrED#
2. **Check size:** Make sure width/height/radius are correct
3. **Use dry-run:** Preview before writing

### Gates Not Working
1. **Check gate coordinates:** Must be exactly on wall perimeter
2. **Format:** Use "x1,y1 x2,y2" format with spaces
3. **Test:** Use `--dry-run` to verify gate positions

## Next Steps

After generating walls:
1. Restart CentrED# server
2. Restart ServUO server
3. Open CentrED# client and navigate to city
4. Verify walls appear correctly
5. If needed, restore from backup and adjust inputs

## See Also

- `CITY_WALL_GENERATION.md` - Full usage guide
- `generate_city_walls_cli.py --help` - Command-line help
- CentrED# documentation - Map editing tool

