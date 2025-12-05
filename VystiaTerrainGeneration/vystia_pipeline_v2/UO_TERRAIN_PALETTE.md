# Ultima Online Terrain Palette Reference

This document defines the complete UO terrain color palette extracted from `UOL 1.5\Data\System\Terrain.xml`.

These RGB values **MUST** be used exactly in Terrain.bmp files for UOMapMake.exe to process them correctly.

---

## Complete Terrain Palette

| Palette ID | Terrain Name | RGB Color | Hex Code | Base Altitude | Tile ID | Notes |
|------------|--------------|-----------|----------|---------------|---------|-------|
| 0 | No Draw | `RGB(50, 65, 75)` | `#32414B` | 216 | 2 | Dark gray-blue |
| 1 | Grass | `RGB(0, 100, 0)` | `#006400` | 88 | 3 | Dark green |
| 2 | Furrows N | `RGB(130, 100, 50)` | `#826432` | 88 | 9 | Brown farmland |
| 3 | Furrows W | `RGB(130, 100, 85)` | `#826455` | 88 | 336 | Brown farmland |
| 4 | Sand | `RGB(227, 191, 51)` | `#E3BF33` | 88 | 22 | Yellow desert |
| 5 | Deep Water | `RGB(0, 0, 130)` | `#000082` | 24 | 168 | Dark blue ocean |
| 6 | Shallow Water | `RGB(0, 0, 170)` | `#0000AA` | 24 | 102 | Medium blue water |
| 7 | Large Dirt | `RGB(120, 110, 90)` | `#786E5A` | 88 | 113 | Brown earth |
| 8 | Small Dirt | `RGB(135, 125, 90)` | `#877D5A` | 88 | 117 | Light brown earth |
| 9 | Water | `RGB(0, 110, 255)` | `#006EFF` | 24 | 168 | Bright blue water |
| 10 | Jungle | `RGB(0, 75, 75)` | `#004B4B` | 88 | 172 | Dark teal |
| 11 | Forest | `RGB(0, 110, 90)` | `#006E5A` | 88 | 196 | Dark green-teal |
| 12 | Snow | `RGB(255, 255, 255)` | `#FFFFFF` | 88 | 282 | White |
| 13 | Lava | `RGB(255, 0, 0)` | `#FF0000` | 88 | 500 | Bright red |
| 14 | Stars | `RGB(75, 75, 100)` | `#4B4B64` | 88 | 506 | Dark blue-gray |
| 18 | Rock | `RGB(75, 75, 75)` | `#4B4B4B` | 152 | 556 | Gray stone |
| 19 | Blank | `RGB(0, 0, 0)` | `#000000` | 88 | 580 | Black |
| 20 | Mountain Cave | `RGB(100, 95, 0)` | `#645F00` | 216 | 581 | Dark olive |
| 21 | Dungeon Cave | `RGB(100, 95, 80)` | `#645F50` | 216 | 581 | Brown-gray |
| 41 | Swamp | `RGB(50, 210, 240)` | `#32D2F0` | 88 | 15849 | Cyan |
| 42 | Grass Embankment | `RGB(0, 120, 0)` | `#007800` | 216 | 3 | Medium green |
| 50 | Moss | `RGB(60, 190, 240)` | `#3CBEF0` | 88 | 15853 | Light cyan |
| 62 | Grass Without Static | `RGB(0, 200, 0)` | `#00C800` | 88 | 3 | Bright green |
| 80 | Beach | `RGB(255, 255, 192)` | `#FFFFC0` | 216 | 22 | Pale yellow |

---

## How BMP Palette Indexing Works

### 8-bit Palette Mode
Terrain.bmp uses **8-bit indexed color mode** where:
- Each pixel stores a single byte (0-255)
- That byte is an **index** into the color palette table
- The palette table contains 256 RGB color entries
- Palette position = Terrain ID

### Example
```
Pixel Value    →    Palette Index    →    RGB Color         →    Terrain Type
     1         →         ID 1        →    RGB(0,100,0)      →    Grass
     5         →         ID 5        →    RGB(0,0,130)      →    Deep Water
    12         →         ID 12       →    RGB(255,255,255)  →    Snow
```

### Critical Rules
1. **Never use palette index 0** - Can cause rendering issues
2. **RGB values must match exactly** - UOMapMake validates colors
3. **File format must be BMPv3** - 40-byte BITMAPINFOHEADER
4. **Dimensions must be correct** - 8192×4096 for Map5

---

## Vystia Region Mapping

Current mapping of Vystia custom regions to UO terrain types:

| Vystia Region | UO Terrain ID | Terrain Type | RGB Color |
|---------------|---------------|--------------|-----------|
| Ocean | 5 | Deep Water | `RGB(0,0,130)` |
| Frosthold | 12 | Snow | `RGB(255,255,255)` |
| Winterguard | 12 | Snow | `RGB(255,255,255)` |
| Verdantpeak | 11 | Forest | `RGB(0,110,90)` |
| Verdant Isles | 11 | Forest | `RGB(0,110,90)` |
| Eternal Twilight | 11 | Forest | `RGB(0,110,90)` |
| Hollow Forests | 11 | Forest | `RGB(0,110,90)` |
| Wilderlands | 11 | Forest | `RGB(0,110,90)` |
| Skyreach Peak | 18 | Rock | `RGB(75,75,75)` |
| Crystal Barrens | 18 | Rock | `RGB(75,75,75)` |
| Mystic Canyons | 18 | Rock | `RGB(75,75,75)` |
| Ironclad Empire | 18 | Rock | `RGB(75,75,75)` |
| Deepforge | 20 | Mountain Cave | `RGB(100,95,0)` |
| Forgotten Depths | 21 | Dungeon Cave | `RGB(100,95,80)` |
| Emberlands | 4 | Sand | `RGB(227,191,51)` |
| Whispering Sands | 4 | Sand | `RGB(227,191,51)` |
| Blazing Frontier | 4 | Sand | `RGB(227,191,51)` |
| Radiant Plains | 1 | Grass | `RGB(0,100,0)` |
| Golden Steppe | 1 | Grass | `RGB(0,100,0)` |
| Shadowfen | 41 | Swamp | `RGB(50,210,240)` |
| Glimmering Archipelago | 6 | Shallow Water | `RGB(0,0,170)` |
| Sunken Isles | 6 | Shallow Water | `RGB(0,0,170)` |

---

## Color Classification Heuristics

The pipeline's `05_map_colors_to_terrain.py` uses these rules to automatically classify colors:

### Water Detection
- **High blue channel** (B > 100)
- **Blue dominant** (B > R and B > G)
- **Deep Water**: B > 150
- **Shallow Water**: B = 100-150

### Snow Detection
- **High brightness** (R, G, B all > 200)
- **Low saturation** (very close R/G/B values)

### Rock/Mountains Detection
- **Gray tones** (low saturation < 0.3)
- **Medium brightness** (60 < value < 180)

### Sand/Desert Detection
- **Yellow/tan colors** (R > 150, G > 120, B < 120)
- **Warm tones** (high R+G, low B)

### Forest Detection
- **Texture-based**: High local variance (bumpy/tree texture)
- **Green tones**: G > R and G > B
- **Medium-high green**: G > 80

### Grass Detection
- **Light green**: High brightness with green dominant
- **Yellowish-green**: Similar R and G values, both high

### Swamp Detection
- **Brownish-green**: Green slightly dominant
- **Low saturation**: < 0.35
- **Medium brightness**: 60 < value < 160

---

## Altitude Mapping

Altitude.bmp uses the same 8-bit palette format but represents elevation:

| Palette Index | Z Height | Visual Color (for preview) | Usage |
|---------------|----------|----------------------------|-------|
| 1 | ~0-5 | Dark blue | Ocean floor |
| 10 | ~10 | Cyan | Coastal/beach |
| 20 | ~20 | Dark green | Low elevation |
| 30 | ~30 | Green | Medium-low |
| 50 | ~50 | Yellow-green | Medium |
| 80 | ~80 | Tan | Medium-high |
| 100 | ~100 | Gray | Mountains/peaks |

**Note**: Altitude indices represent actual Z-coordinate heights in UO. The palette colors are only for visualization - UOMapMake reads the index value as the height.

---

## Pipeline Files Using This Palette

1. **`scripts/uo_terrain_palette.py`**
   - Python dictionary with full palette data
   - Helper functions for RGB lookups

2. **`scripts/05_map_colors_to_terrain.py`**
   - Analyzes source images
   - Maps colors to terrain IDs automatically

3. **`scripts/45_map_to_uo_terrain.py`**
   - Maps custom regions to UO terrain types
   - Uses REGION_TO_TERRAIN mapping

4. **`scripts/50_write_uo_bitmaps.py`**
   - Writes final Terrain.bmp with exact palette
   - Generates Altitude.bmp with elevation indices

---

## References

- **Source**: `C:\DevEnv\GIT\UO\UOL 1.5\Data\System\Terrain.xml`
- **Tool**: UOMapMake.exe (UO Landscaper 1.5)
- **Format Spec**: Windows BMP v3 (BITMAPINFOHEADER)
- **Color Mode**: 8-bit indexed palette (256 colors max)

---

*Last Updated: 2025-10-26*
*Wystia Pipeline v2 - Procedural World Generation*
