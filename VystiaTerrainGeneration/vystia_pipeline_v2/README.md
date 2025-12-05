# Vystia Terrain Generation Pipeline V2

**Directory:** `VystiaTerrainGeneration/vystia_pipeline_v2/` (formerly "VystiaGeneration")
**Purpose:** Procedural terrain generation pipeline for Ultima Online

**Clean-room terrain generation pipeline** - creates base world terrain from scratch

Generates UOMapMake-ready assets at native **8192×4096** resolution with **zero post-processing fixes required**.

## Key Features

- **Native 8192×4096 generation** - No resizing, no padding artifacts
- **Procedural ocean islands** - 78 randomly generated landmasses fill padding zones around core continent
- **Deterministic builds** - Same inputs always produce same outputs
- **UO-compliant BMPs** - Terrain.bmp (8-bit palette) and Altitude.bmp (8-bit grayscale) in BMPv3 format
- **Validated output** - Automated checks ensure UOMapMake compatibility
- **Fast execution** - Complete build in ~40 seconds

## Pipeline Stages

| Stage | Script | Description |
|-------|--------|-------------|
| **0** | `00_normalize_and_expand.py` | Centers 3200×2400 core continent in 8192×4096 canvas and generates procedural islands in ocean padding zones |
| **1** | `10_generate_regions.py` | Voronoi segmentation with 21 lore-aligned regions using anchored seeds |
| **5** | `50_write_uo_bitmaps.py` | Writes UO-compatible Terrain.bmp and Altitude.bmp with validation |

### Planned Stages (Future)

- **Stage 2:** Mountain mask generation with distance-based falloff
- **Stage 3:** Biome-to-terrain color mapping using VystiaTiles.xml palette
- **Stage 4:** Advanced heightmap synthesis with mountain elevation

## Quick Start

### Prerequisites

```bash
pip install pillow opencv-python numpy
```

### Run Build

```bash
cd C:\DevEnv\GIT\UO\VystiaGeneration\vystia_pipeline_v2
python build_all_v2.py
```

### Expected Output

```
[10:03:05] [INFO] Vystia Pipeline V2 - Build Started
[10:03:05] [INFO] Stages to run: 3

[10:03:10] [SUCCESS] OK: Stage 0: Normalize & Expand with Procedural Islands
[10:03:42] [SUCCESS] OK: Stage 1: Generate Voronoi Regions
[10:03:45] [SUCCESS] OK: Stage 5: Write UO Bitmaps

[10:03:45] [SUCCESS] ALL STAGES PASSED
[10:03:45] [INFO] Duration: 40.3 seconds
```

## Directory Structure

```
vystia_pipeline_v2/
├── build_all_v2.py          # Main orchestrator
├── input/                   # Original assets (3200×2400)
│   ├── continents.png
│   ├── mountains.png
│   └── HvMVAcKMEg7dADyZja2SYQ.jpeg
├── input_norm/              # Normalized 8192×4096 inputs with procedural islands
│   ├── continents.png       # Core + 78 procedural islands
│   ├── mountains.png
│   └── basemap.png
├── scripts/                 # Pipeline stages
│   ├── 00_normalize_and_expand.py
│   ├── 10_generate_regions.py
│   └── 50_write_uo_bitmaps.py
├── exports/                 # Final UOMapMake-ready files
│   ├── Terrain.bmp          # 8192×4096, 8-bit palette, BMPv3
│   ├── Altitude.bmp         # 8192×4096, 8-bit grayscale, BMPv3
│   ├── validation_report.md
│   └── regions_final.png
├── preview/                 # QA visualization images
│   ├── 00_expanded_world.png     # Shows core + procedural islands
│   ├── 10_regions_preview.png    # Voronoi regions overlay
│   ├── 50_terrain_final.png
│   └── 50_altitude_final.png
└── builds/logs/             # Build logs with timestamps
    └── build_20251026_100305.log
```

## Output Specifications

### Terrain.bmp
- **Resolution:** 8192×4096
- **Format:** BMPv3 (BITMAPINFOHEADER, 40 bytes)
- **Color Mode:** 8-bit indexed palette (P)
- **Colors:** 21 unique region colors
- **Compression:** None (BI_RGB)

### Altitude.bmp
- **Resolution:** 8192×4096
- **Format:** BMPv3 (BITMAPINFOHEADER, 40 bytes)
- **Color Mode:** 8-bit grayscale (L)
- **Range:** 0 (ocean) to 119 (inland)
- **Compression:** None (BI_RGB)

## Validation

Every build automatically validates:

- ✓ Exact 8192×4096 dimensions
- ✓ BMPv3 header format (40-byte DIB)
- ✓ Terrain uses 8-bit palette mode
- ✓ Altitude uses 8-bit grayscale mode
- ✓ No compression (BI_RGB)

Check `exports/validation_report.md` after each build.

## Procedural Island Generation

Stage 0 generates **78 random islands** in the ocean padding zones using:

- **15 island clusters** distributed across padding zones (top/bottom/left/right)
- **3-8 islands per cluster** with random sizes (50-400 pixel radius)
- **Irregular shapes** using noise-based radial displacement
- **Deterministic seed** (42) - same islands every build

## Region Anchoring

21 lore-aligned regions are anchored to specific locations in the core continent:

| Region | Anchor (core coords) |
|--------|---------------------|
| Frosthold | (250, 150) |
| Winterguard | (400, 250) |
| Verdantpeak | (600, 200) |
| Skyreach Peak | (1050, 130) |
| Ironclad Empire | (800, 400) |
| ... | ... |

Anchors are automatically offset to account for centering in 8192×4096 canvas.

## Memory Efficiency

Voronoi segmentation at 8192×4096 (33.5M pixels) uses **chunked processing** (512-row chunks) to avoid memory overflow when calculating distances to 21 seed points.

## Using with UOMapMake

1. Verify build success: Check `exports/validation_report.md` shows **READY for UOMapMake.exe**
2. Locate files:
   - `exports/Terrain.bmp`
   - `exports/Altitude.bmp`
3. Run UOMapMake.exe with these files
4. Output: `map5.mul`, `statics5.mul`, `staidx5.mul` for ServUO

## Differences from V1 Pipeline

| Aspect | V1 (Full Map) | V2 (Current) |
|--------|--------------|--------------|
| **Resolution Approach** | Generate at variable size, pad/resize at end | Native 8192×4096 from Stage 0 |
| **Ocean Padding** | Solid black or ocean color fill | Procedural islands (78 islands) |
| **Post-Processing** | Multiple fix scripts needed | Zero fixes required |
| **Memory** | Full Voronoi in one pass | Chunked 512-row processing |
| **Validation** | Manual checks | Automated validation_report.md |
| **Build Time** | ~2 minutes + manual fixes | ~40 seconds automated |

## Troubleshooting

### Build Fails at Stage 0
- **Check:** Input files exist in `input/` directory
- **Check:** Files are valid images (continents.png, mountains.png, basemap)

### Build Fails at Stage 1
- **Error:** Memory allocation failed
- **Fix:** Chunk size in `10_generate_regions.py` can be reduced from 512 to 256

### Validation Report Shows Errors
- **Check:** `exports/validation_report.md` for specific issues
- **Common:** BMPv3 header forcing may have failed - check PIL version

### UOMapMake Rejects Files
- **Check:** Validation report status
- **Check:** File dimensions with `identify Terrain.bmp` or Python PIL
- **Check:** No spaces in file path

## Development

### Adding New Stages

1. Create script: `scripts/XX_stage_name.py`
2. Follow conventions:
   - Read from `input_norm/` or `exports/`
   - Write outputs to `exports/`
   - Write previews to `preview/XX_*.png`
   - Print clear stage completion messages
3. Add to `STAGES` list in `build_all_v2.py`

### Testing Individual Stages

```bash
cd C:\DevEnv\GIT\UO\VystiaGeneration\vystia_pipeline_v2
python scripts/00_normalize_and_expand.py
python scripts/10_generate_regions.py
python scripts/50_write_uo_bitmaps.py
```

## License & Credits

**Vystia Custom UO Shard Project**

Based on the clean-room pipeline specification in `vystia_world_generation_summary.md`

---

**Last Updated:** 2025-12-05 (Directory renamed from "VystiaGeneration" to "VystiaTerrainGeneration")
**Build Date:** 26 October 2025
**Pipeline Version:** 2.0
**Target UO Map Size:** 8192×4096 (Map5)
