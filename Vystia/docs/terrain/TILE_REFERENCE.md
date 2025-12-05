# Vystia Tile Reference Guide

## Land Tiles vs Static Items

**Land Tiles:**
- Base terrain (grass, snow, sand, etc.)
- Defined in map*.mul files
- Modified server-side with `SetLandBlock()`
- Client reads from disk files (not visible until .mul export)

**Static Items:**
- Objects placed on terrain (trees, rocks, decorations)
- Defined in statics*.mul files
- Created with `new Static(tileId)`
- Immediately visible to client

---

## Vystia Biome Tile Ranges

### Land Tiles (Terrain Base)

| Biome | Land Tile Range | Decimal | Notes |
|-------|-----------------|---------|-------|
| **Grass** | 0x0002-0x0005 | 2-5 | Green grass terrain |
| **Desert** | 0x0016-0x001D | 22-29 | Sand/desert terrain |
| **Snow** | 0x010C-0x0113 | 268-275 | White snow terrain |
| **Lava** | 0x01F4-0x01F7 | 500-503 | Red/orange lava |
| **Swamp** | 0x0283-0x0286 | 643-646 | Dark swamp terrain |
| **Water** | 0x00A8-0x00AB | 168-171 | Blue water |

### Static Items (Decorations)

| Type | Static Range | Decimal | Usage |
|------|--------------|---------|-------|
| **Trees** | 0x0CCA-0x0CD8 | 3274-3288 | Forest vegetation |
| **Cacti** | 0x0D94-0x0D98 | 3476-3480 | Desert plants |
| **Rocks** | 0x1363-0x1369 | 4963-4969 | Mountain rocks |
| **Crystals** | 0x2223-0x2227 | 8739-8743 | Volcanic crystals |

---

## Current Implementation Status

### Working Static Items (Visible Now)

| Biome | Current Tile | Type | Status |
|-------|--------------|------|--------|
| **Grassland** | 0x0003 | Land tile as static | ✅ Working great |
| **Desert** | 0x0029 | Land tile as static | ✅ Working great |
| **Mountain** | 0x053B | Stone static | ✅ Working great |
| **Volcanic** | 0x1775 + hue 0x0026 | Lava static | ⏳ Needs testing |

### Problematic Biomes

| Biome | Land Tile | Static Attempt | Issue |
|-------|-----------|----------------|-------|
| **Snow** | 0x010C-0x0113 | Various tested | ❌ No good static equivalent found |

**Snow attempts:**
- 0x11A8 - Looked like weeds
- 0x11A4 - Flashing red lights
- 0x10C4 - Spider webs
- 0x0385 - Wrong appearance
- 0x011C - Wooden planks (land tile ID, not static)
- 0x0983 - Currently testing

---

## Solution Paths

### Path 1: Using Land Tiles as Statics (Current)

**Works for:**
- ✅ Grass (0x0003)
- ✅ Desert (0x0029)

**Doesn't work for:**
- ❌ Snow (0x010C) - Not a valid static item

**Why it works:**
Some land tile IDs can also be used as static items. When you create `new Static(0x0003)`, it renders the grass tile as a static object.

### Path 2: Finding Static Equivalents

Search for static items that visually match land tiles:

**Snow candidates to test:**
- 0x0983-0x098F (snow patch range)
- 0x0DEE-0x0DF0 (snow ground)
- 0x3124 (ice/snow tile)

**Command:** `[TestSnowTiles` tests multiple options

### Path 3: Export to .mul Files (Permanent Solution)

Use real land tiles (0x010C for snow) and export:

1. Generate terrain with `SetLandBlock()` using proper land tile IDs
2. Export server map with CentrED+
3. Replace client .mul files
4. All terrain visible permanently

**See:** [REAL_TERRAIN_EXPORT.md](REAL_TERRAIN_EXPORT.md)

---

## Recommended Tile IDs for Commands

### For Testing with Static Items

```csharp
// Working biomes
CreateBiomeArea(map, x, y, size, 0x0003, "Grass", 5);    // Grass
CreateBiomeArea(map, x, y, size, 0x0029, "Desert", 5);   // Desert
CreateBiomeArea(map, x, y, size, 0x053B, "Mountain", 25); // Stone

// Testing needed
CreateBiomeArea(map, x, y, size, 0x0983, "Snow", 5);     // Snow (testing)
CreateBiomeArea(map, x, y, size, 0x1775, "Volcanic", 10); // Lava
```

### For Production with SetLandBlock

```csharp
// Use proper land tile IDs
SetLandBlock(tileMatrix, x, y, 0x0003);  // Grass
SetLandBlock(tileMatrix, x, y, 0x0029);  // Desert
SetLandBlock(tileMatrix, x, y, 0x010C);  // Snow (requires .mul export)
SetLandBlock(tileMatrix, x, y, 0x01F4);  // Lava
SetLandBlock(tileMatrix, x, y, 0x0283);  // Swamp
SetLandBlock(tileMatrix, x, y, 0x00A8);  // Water
```

---

## Adding Decorations

Once base terrain is working, add biome-appropriate decorations:

### Grassland
```csharp
// Add trees
for (int i = 0; i < treeCount; i++)
{
    Static tree = new Static(Utility.Random(0x0CCA, 0x0CD8 - 0x0CCA));
    tree.MoveToWorld(location, map);
}
```

### Desert
```csharp
// Add cacti
Static cactus = new Static(Utility.Random(0x0D94, 0x0D98 - 0x0D94));
cactus.MoveToWorld(location, map);
```

### Mountains
```csharp
// Add rocks
Static rock = new Static(Utility.Random(0x1363, 0x1369 - 0x1363));
rock.MoveToWorld(location, map);
```

### Volcanic
```csharp
// Add crystals
Static crystal = new Static(Utility.Random(0x2223, 0x2227 - 0x2223));
crystal.Hue = 0x0026; // Red hue for volcanic theme
crystal.MoveToWorld(location, map);
```

---

## Testing Checklist

- [x] Grass terrain (0x0003) - Working
- [x] Desert terrain (0x0029) - Working
- [x] Mountain terrain (0x053B) - Working
- [ ] Snow terrain - In progress (testing 0x0983)
- [ ] Volcanic terrain (0x1775) - Needs testing
- [ ] Swamp terrain - Not yet implemented
- [ ] Water terrain - Not yet implemented

---

## References

**Land Tile IDs:**
- Grass: 0x0002-0x0005
- Desert: 0x0016-0x001D
- Snow: 0x010C-0x0113
- Lava: 0x01F4-0x01F7
- Swamp: 0x0283-0x0286
- Water: 0x00A8-0x00AB

**Static Decorations:**
- Trees: 0x0CCA-0x0CD8
- Cacti: 0x0D94-0x0D98
- Rocks: 0x1363-0x1369
- Crystals: 0x2223-0x2227

**Mountain Stone (working):** 0x053B

---

*Last Updated: 2025-10-21*
*Status: Grass, desert, mountains working. Snow static testing in progress.*
