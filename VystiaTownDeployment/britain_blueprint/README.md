# Britain Blueprint - Import Guide

This blueprint contains the complete Britain city from OSI map0, including:
- **105,651 terrain tiles** (land/ground)
- **56,608 static tiles** (buildings, items, decorations)
- **Total: 162,259 items**

All coordinates are **relative** (offset from center), allowing you to place Britain anywhere on your map.

## Original OSI Location
- **Center:** (1575, 1650)
- **Bounds:** (1400, 1500) to (1750, 1800)
- **Size:** 351 x 301 tiles

## Import Methods

### Method 1: Using the Placement Script (Recommended)

Use the `place_britain_blueprint.py` script to automatically place Britain at your desired location:

```bash
cd VystiaTownDeployment
python place_britain_blueprint.py --center-x 3000 --center-y 2000
```

**Options:**
- `--center-x X` - X coordinate where to place Britain center (required)
- `--center-y Y` - Y coordinate where to place Britain center (required)
- `--z-adjust N` - Adjust all Z-levels by N (default: 0)
- `--dry-run` - Preview placement without writing files
- `--map PATH` - Path to map0.mul (auto-detects from CentrED config if not specified)
- `--statics PATH` - Path to statics0.mul (auto-detects if not specified)
- `--staidx PATH` - Path to staidx0.mul (auto-detects if not specified)

**Example:**
```bash
# Place Britain at (3000, 2000) with +5 Z adjustment
python place_britain_blueprint.py --center-x 3000 --center-y 2000 --z-adjust 5

# Preview first (dry run)
python place_britain_blueprint.py --center-x 3000 --center-y 2000 --dry-run
```

The script will:
1. Read the blueprint JSON
2. Convert relative coordinates to world coordinates at your specified center
3. Write terrain to `map0.mul`
4. Write statics to `statics0.mul` and `staidx0.mul`
5. Auto-detect map file paths from your CentrEDSharp config

**After running:**
- Restart CentrEDSharp to see the changes
- Restart your UO server if needed

### Method 2: Manual Import in CentrEDSharp

If CentrEDSharp has a blueprint import feature:

1. **Open CentrEDSharp**
2. **Load your map** (the one you want to place Britain on)
3. **Navigate to your desired location** (where you want Britain's center)
4. **Look for Import/Blueprint menu** (usually in File or Tools menu)
5. **Select the blueprint file:** `britain_complete.json`
6. **Set placement coordinates:**
   - Center X: Your desired X coordinate
   - Center Y: Your desired Y coordinate
7. **Import/Place**

**Note:** If CentrEDSharp doesn't have a direct JSON import, you may need to:
- Convert the JSON to a format CentrEDSharp supports
- Or use Method 1 (the placement script) instead

### Method 3: Manual Coordinate Calculation

If you need to manually place items, the blueprint uses this format:

```json
{
  "center": {"x": 1575, "y": 1650},  // Original OSI center
  "items": [
    {
      "tile_id": 197,
      "x": -175,  // Relative to center
      "y": -150,  // Relative to center
      "z": 10,
      "hue": 0
    }
  ]
}
```

**To place at new center (X, Y):**
- World X = `new_center_x + item.x`
- World Y = `new_center_y + item.y`
- World Z = `item.z + z_adjustment`

## Blueprint Format

The blueprint JSON structure:
```json
{
  "name": "Britain (OSI)",
  "description": "...",
  "center": {"x": 1575, "y": 1650},
  "bounds": {"x1": 1400, "y1": 1500, "x2": 1750, "y2": 1800},
  "total_items": 162259,
  "terrain_count": 105651,
  "statics_count": 56608,
  "items": [
    {
      "tile_id": 197,
      "x": -175,  // Relative X
      "y": -150,  // Relative Y
      "z": 10,
      "hue": 0
    }
  ]
}
```

## Tips

1. **Z-Level Adjustment:** OSI Britain terrain was at different Z-levels than your map. You may need to adjust:
   ```bash
   python place_britain_blueprint.py --center-x 3000 --center-y 2000 --z-adjust 5
   ```

2. **Check Terrain First:** Use `--dry-run` to preview placement before writing files

3. **Backup First:** Always backup your map files before placing large blueprints

4. **CentrEDSharp Config:** The script auto-detects map paths from:
   - `D:\UO\centredsharp\Output-Custom-0\Cedserver.xml`

5. **File Locations:** After placement, files are written to:
   - Map: `D:\UO\UO Adventures\Client\Data Files\map0.mul`
   - Statics: `D:\UO\UO Adventures\Client\Data Files\statics0.mul`
   - StaIdx: `D:\UO\UO Adventures\Client\Data Files\staidx0.mul`

## Troubleshooting

**Script can't find map files:**
- Specify paths manually: `--map PATH --statics PATH --staidx PATH`

**Items appear in wrong location:**
- Check that you're using the correct center coordinates
- Verify your map uses the same coordinate system (map0)

**Z-levels look wrong:**
- Adjust with `--z-adjust` parameter
- Check terrain height at your placement location first

**CentrEDSharp doesn't show changes:**
- Restart CentrEDSharp
- Reload the map
- Check that files were written to the correct location
