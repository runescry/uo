# Vystia Town Deployment

**Directory:** `VystiaTownDeployment/` (formerly "Vystia Town Generator")
**Purpose:** Extract OSI towns and deploy them to custom Vystia UO maps

Production-ready tools for town extraction, deployment, and city wall generation.

## ✅ Working Scripts

### `deploy_britain_correct.py` - **PRIMARY DEPLOYMENT TOOL**
Deploys OSI Britain to Vystia map using the correct CentrED block calculation formula.

**Usage:**
```bash
python3 deploy_britain_correct.py
```

**What it does:**
1. Reads all OSI Britain statics with world coordinates (60,257+ statics)
2. Shifts coordinates: +1857 X, +138 Y tiles
3. Adjusts Z-levels: +5 (OSI terrain at Z=20, Vystia at Z=0)
4. Regroups into blocks using **CORRECT** formula: `blockID = blockX * 512 + blockY`
5. Merges with existing Vystia map data
6. Writes to both locations:
   - `C:\Program Files (x86)\Electronic Arts\Ultima Online Classic\` (ServUO)
   - `C:\DevEnv\GIT\UO\ServUO-fresh\Data\Maps\` (CentrED)

**Target Location:** (3257, 1638) to (3608, 1933), center ~(3432, 1785)

### `uo_town_extractor.py` - **EXTRACTION TOOL**
Extracts towns from OSI map files to JSON format.

**Usage:**
```bash
python3 uo_town_extractor.py
```

**Output:** Creates `town_data/` directory with:
- britain.json (16,252 statics)
- trinsic.json, vesper.json, moonglow.json, etc.

### `build_template_library.py` - **PATTERN ANALYSIS**
Analyzes extracted towns to build reusable building templates.

### `uo_tile_classifier.py` - **UTILITY**
Classifies UO tile IDs by category (building, terrain, etc.)

### `uo_pattern_analyzer.py` - **UTILITY**
Analyzes patterns in extracted town data.

---

## 🚫 Deprecated Scripts (moved to `deprecated/`)

These scripts contain the **WRONG block calculation formula** and will deploy statics to incorrect locations:

- `mul_writer.py.OLD` - Wrong formula: `blockID = blockX + (blockY * 896)`
- `generate_vystia_cities.py.OLD` - Procedural generation (not currently used)
- `production_town_generator.py.OLD` - Old deployment method
- `uo_town_generator.py.OLD` - Old generation system

**DO NOT USE THESE FILES** - They will result in scattered buildings at wrong coordinates.

---

## 🔑 Critical Technical Details

### Block Calculation Formula
**CORRECT** (CentrED/ServUO):
```python
MAP_HEIGHT_BLOCKS = 512  # 4096 tiles / 8
blockID = blockX * MAP_HEIGHT_BLOCKS + blockY
```

**WRONG** (caused all failed deployments):
```python
MAP_WIDTH_BLOCKS = 896  # 7168 tiles / 8
blockID = blockX + (blockY * MAP_WIDTH_BLOCKS)
```

**Discovery:** Found by analyzing CentrED source code:
```csharp
// From CentrEDClient.cs
public long GetBlockNumber(ushort x, ushort y)
{
    return x * Height + y;  // Height = 512, NOT Width!
}
```

### Map Dimensions
- **Tiles:** 7168 × 4096
- **Blocks:** 896 × 512 (each block is 8×8 tiles)
- **Block coordinates:** `block_x = tile_x // 8`, `block_y = tile_y // 8`
- **Local coordinates:** `local_x = tile_x & 0x7`, `local_y = tile_y & 0x7`

### Z-Level Adjustment
- **OSI Britain:** Terrain at Z=20, most statics at Z=-5
- **Vystia:** Terrain at Z=0
- **Required adjustment:** +5 (to bring Z=-5 to Z=0)
- **Clamping:** Must clamp to [-128, 127] (signed byte range)

### File Locations

**ServUO reads from TWO locations:**
- **Map terrain** (map0.mul): `C:\DevEnv\GIT\UO\ServUO-fresh\Data\Maps\` (from DataPath.cfg)
- **Statics** (statics0.mul, staidx0.mul): `C:\Program Files (x86)\Electronic Arts\Ultima Online Classic\`

**CentrED reads from:**
- All files: `C:\DevEnv\GIT\UO\ServUO-fresh\Data\Maps\` (from Cedserver.xml)

**Therefore:** Must deploy statics to BOTH locations!

---

## 📋 Deployment Workflow

### 1. Extract Town (if not already done)
```bash
python3 uo_town_extractor.py
```

### 2. Deploy to Vystia
```bash
python3 deploy_britain_correct.py
```

### 3. Copy to CentrED Location
The script writes to ServUO location. Copy to CentrED:
```bash
copy "C:\Program Files (x86)\Electronic Arts\Ultima Online Classic\statics0.mul" "C:\DevEnv\GIT\UO\ServUO-fresh\Data\Maps\statics0.mul"
copy "C:\Program Files (x86)\Electronic Arts\Ultima Online Classic\staidx0.mul" "C:\DevEnv\GIT\UO\ServUO-fresh\Data\Maps\staidx0.mul"
```

### 4. Restart ServUO and CentrED
- Kill both processes
- Start CentrED server: `C:\DevEnv\GIT\UO\centredsharp\output\Cedserver.exe`
- Start ServUO: `C:\DevEnv\GIT\UO\ServUO-fresh\ServUO.exe`
- Connect CentrED client
- In-game: Navigate to (3432, 1785)

---

## 🏗️ To Deploy Other Towns

Modify `deploy_britain_correct.py` with new coordinates:

```python
# Change source bounds (OSI coordinates)
for block_y in range(187, 226):  # Britain Y blocks
    for block_x in range(175, 219):  # Britain X blocks

# Change target coordinates
shift_x = target_x - source_center_x
shift_y = target_y - source_center_y
```

**Available Towns** (extracted):
- britain.json ✓ (deployed)
- vesper.json (11,024 statics)
- moonglow.json (5,964 statics)
- yew.json (9,528 statics)
- minoc.json (6,462 statics)
- cove.json (6,812 statics)

---

## 🐛 Known Issues

### Issue: Block Calculation Bug (SOLVED)
**Problem:** All deployments before 2025-10-30 used wrong formula, causing scattered buildings.
**Solution:** Updated to CentrED's correct formula: `blockX * 512 + blockY`

### Issue: Z-Level Mismatch (SOLVED)
**Problem:** Buildings floating/walkthrough due to terrain height difference.
**Solution:** Added +5 Z adjustment with clamping to signed byte range.

---

## 📚 References

- **CentrED Source:** `C:\DevEnv\GIT\UO\centredsharp\`
- **ServUO:** `C:\DevEnv\GIT\UO\ServUO-fresh\`
- **Clean Vystia Maps:** `C:\DevEnv\GIT\UO\UOL 1.5\`
- **OSI Source Maps:** `C:\Ultima Online\`
- **Deployment Status:** `C:\DevEnv\GIT\UO\Documentation\DEPLOYMENT_STATUS.md`

---

## ✨ Success Metrics

**Britain Deployment (2025-10-30):**
- ✅ 60,257 statics deployed
- ✅ Visible in CentrED at (3432, 1785)
- ✅ Walkable in ServUO (correct Z-levels)
- ✅ Buildings properly positioned (no scattering)
- ✅ Complete town with roads, buildings, walls

**Previous Attempts:** All failed due to wrong block calculation formula.

---

## 🎯 Next Steps

1. Create `deploy_town.py` - Generic deployment tool for any town
2. Deploy remaining towns (Vesper, Moonglow, Yew, etc.)
3. Extract and deploy dungeons
4. Build automated city generation pipeline

---

*Last Updated: 2025-12-05 (Renamed from "Vystia Town Generator" for clarity)*
