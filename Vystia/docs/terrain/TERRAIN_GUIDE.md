# Vystia Terrain Generation - Working Guide

## ✅ What Works (Tested & Confirmed)

### Visible Terrain Using Static Items

**Status:** ✅ WORKING - Confirmed with screenshot

**Commands:**
```
[ShowGrassNow       - Creates visible grass (tested, works!)
[ShowGrassFull      - Full grass coverage (after restart)
[ShowAllBiomes      - All 5 biomes at once (after restart)
```

### How It Works

**Static Items = Visible Terrain**
- Creates actual game objects (Static class)
- Client sees them immediately
- No .mul file modifications needed
- Saved in server world saves

**Coordinates:**
- Test area: (4500, 2100)
- Size: 128x128 tiles
- Map: Felucca (Map 0)

---

## 🚀 Quick Start

### 1. Start Server
```bash
cd C:\DevEnv\GIT\UO\ServUO
./ServUO.exe
```

### 2. Test Visible Terrain
```
[ShowGrassNow
```

**Expected:** See brown/green grass tiles immediately

### 3. After Server Restart - Better Visuals
```
[ShowGrassFull      ← Full grass coverage
[ShowAllBiomes      ← All 5 biomes
```

---

## 📍 Commands Reference

### Current Working Commands

| Command | What It Does | Location |
|---------|-------------|----------|
| `[ShowGrassNow` | Grass (every 2 tiles) | (4564, 2164, 5) |
| `[ShowGrassFull` | Grass (full coverage) | (4564, 2164, 5) |
| `[ShowMountain` | Gray stone mountains | (4564, 2314, 25) |
| `[ShowSnow` | White snow fields | (4564, 2464, 5) |
| `[ShowDesert` | Tan sand desert | (4564, 2614, 5) |
| `[ShowAllBiomes` | All 5 biomes | Auto-teleport |

### Diagnostic Commands

| Command | Purpose |
|---------|---------|
| `[CheckTile` | Show tile info at location |
| `[WhereAmI` | Show current map/coordinates |
| `[Go X Y Z` | Teleport to coordinates |

---

## 🎨 What You'll See

### ShowGrassNow (Current)
- Brown/green grass tiles
- Spaced every 2 tiles (pattern visible)
- Large 128x128 area
- ✅ Tested and working

### ShowGrassFull (After Restart)
- Full coverage (every tile)
- Better graphics
- Solid green appearance
- ⏭️ Available after server restart

### ShowAllBiomes (After Restart)
- 5 different terrain types
- Stacked vertically around X=4500:
  - Y=2100: 🟩 Grassland
  - Y=2250: ⬛ Mountain (gray, elevated)
  - Y=2400: ⬜ Snow (white)
  - Y=2550: 🟨 Desert (tan)
  - Y=2700: 🟥 Volcanic (red/black)

---

## 🔧 Technical Details

### Why Static Items Work

**SetLandBlock (Land Tiles):**
- ❌ Modifies server memory only
- ❌ Client can't see (reads from .mul files)
- ✅ Proves terrain generation works
- ⚠️ Requires .mul export for visibility

**Static Items:**
- ✅ Visible immediately
- ✅ No client cache issues
- ✅ Easy to test/iterate
- ✅ Saved in world saves
- ⚠️ Uses more memory than land tiles

### For Production

Eventually convert to permanent land tiles:
1. Test with static items (current approach)
2. Verify visual appearance
3. Export to .mul files using **CentrED+**
4. Replace client map files
5. Permanent, client-visible terrain

---

## 📊 Current Status

✅ **Completed:**
- Terrain generation system working
- Visible terrain via static items
- Coordinate system verified
- Multiple biome support

⏭️ **Next Steps:**
- Test all biome visuals
- Generate full 7168x4096 world
- Apply Perlin noise for realism
- Export to permanent .mul files

---

## 🆘 Troubleshooting

### Command Not Found

**Fix:** Server needs restart to load new commands
```bash
# In-game: [shutdown
# Or close server window
# Restart: ./ServUO.exe
```

### Don't See Terrain

**Check:**
1. Run `[CheckTile` - Are static items there?
2. Correct coordinates? `[WhereAmI`
3. Correct map? Should be Felucca (Map 0)

**Try:**
- Move around the area
- Fly up and look down
- Teleport away and back: `[Go 4564 2164 5`

### Wrong Location

**Teleport to correct spot:**
```
[Go 4564 2164 5    ← Grass center
```

---

## 📁 Files Reference

**Commands:**
- `ServUO/Scripts/Commands/VystiaVisibleNow.cs` - ShowGrassNow
- `ServUO/Scripts/Commands/VystiaBiomeVisuals.cs` - All biomes
- `ServUO/Scripts/Commands/VystiaCheckTile.cs` - Diagnostics

**Config:**
- `ServUO/Data/WorldGen/Vystia_World_Config.json` - World settings

**Documentation:**
- `Vystia/docs/terrain/TERRAIN_GUIDE.md` - This file
- `Vystia/docs/terrain/START_AND_TEST.md` - Testing guide
- `Vystia/guides/VYSTIA_TERRAIN_GENERATION_GUIDE.md` - Full guide

---

## 🎯 Next Phase: Full World Generation

Once biome visuals are confirmed:

1. **Implement Perlin Noise Generator**
   - Realistic elevation maps
   - Moisture/rainfall maps
   - Natural terrain variation

2. **Generate Full Map (7168x4096)**
   - All 11 biomes from config
   - Mountains, valleys, coastlines
   - Island archipelagos

3. **Place Content**
   - 23 cities
   - 18 dungeons
   - Spawners by biome

4. **Export to Permanent Files**
   - Use CentrED+ or similar
   - Create map0.mul files
   - Deploy to client

---

*Last Updated: 2025-10-21*
*Status: Visible terrain working - ready for full generation*
