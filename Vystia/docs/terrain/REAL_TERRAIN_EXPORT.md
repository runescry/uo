# Creating Real Visible Terrain for Vystia

## Current Status

✅ **What Works:**
- Static items create visible terrain (grass, desert, mountains work great)
- Server-side land tiles via SetLandBlock (not visible to client)

❌ **Current Issue:**
- Snow needs to use real land tiles (no good static item equivalent)
- Land tiles modified via SetLandBlock only exist in server memory
- Client reads terrain from .mul files on disk

---

## Solution: Export Server Terrain to Client Files

### The Process

1. **Generate terrain on server** (SetLandBlock)
2. **Export to .mul files** (using CentrED+)
3. **Replace client files** (make terrain permanent)
4. **Client sees real terrain** (land tiles visible)

---

## Method 1: CentrED+ (Recommended)

### What is CentrED+?

- Professional UO map editor
- Can connect to running ServUO server
- Exports server terrain to client .mul files
- Creates: map0.mul, statics0.mul, staidx0.mul

### Download & Setup

**Download:** https://www.uogateway.com/centred/

**Requirements:**
- Windows (or Wine on Linux/Mac)
- Access to your ServUO server
- UO client files location

### Steps

1. **Install CentrED+**
   ```
   Download from https://www.uogateway.com/centred/
   Extract to C:\CentrED+ (or similar)
   ```

2. **Configure Connection**
   - Open CentrED+
   - File → Connect to Server
   - Host: localhost (if running locally)
   - Port: 2597 (default ServUO port)
   - Username/Password: Your admin credentials

3. **Export Map**
   - Tools → Export Map
   - Select Map 0 (Felucca)
   - Choose output directory
   - Wait for export to complete

4. **Backup Original Client Files**
   ```bash
   cd "C:\Program Files (x86)\Electronic Arts\Ultima Online Classic"
   mkdir backup
   copy map0.mul backup\
   copy statics0.mul backup\
   copy staidx0.mul backup\
   ```

5. **Replace Client Files**
   ```bash
   # Copy exported files to client
   copy C:\CentrED+\export\map0.mul "C:\Program Files (x86)\Electronic Arts\Ultima Online Classic\"
   copy C:\CentrED+\export\statics0.mul "C:\Program Files (x86)\Electronic Arts\Ultima Online Classic\"
   copy C:\CentrED+\export\staidx0.mul "C:\Program Files (x86)\Electronic Arts\Ultima Online Classic\"
   ```

6. **Restart Client**
   - Close UO client completely
   - Restart and connect
   - Terrain should now be visible!

---

## Method 2: UOFiddler (Alternative)

### What is UOFiddler?

- UO file editing tool
- Can export/import map data
- More manual than CentrED+

### Download

**Download:** https://github.com/polserver/UOFiddler

### Steps

1. Open UOFiddler
2. File → Load Client Files
3. Map → Select Map 0
4. Export map region
5. Save as .mul files
6. Replace client files (same as CentrED+ steps 4-6)

---

## Method 3: Manual .mul Writing (Advanced)

### For Developers

If you want full control, you can write .mul files directly from C#:

```csharp
using System.IO;
using Server;

public static class MulFileExporter
{
    public static void ExportMapToMul(Map map, string outputPath)
    {
        TileMatrix tiles = map.Tiles;

        // Write map0.mul (land tiles)
        using (FileStream fs = new FileStream(
            Path.Combine(outputPath, "map0.mul"), FileMode.Create))
        using (BinaryWriter writer = new BinaryWriter(fs))
        {
            // Each block is 196 bytes (header + 64 tiles * 3 bytes)
            for (int blockY = 0; blockY < map.MapHeight >> 3; blockY++)
            {
                for (int blockX = 0; blockX < map.MapWidth >> 3; blockX++)
                {
                    // Block header (4 bytes - unused)
                    writer.Write((uint)0);

                    // 64 land tiles (8x8 grid)
                    for (int y = 0; y < 8; y++)
                    {
                        for (int x = 0; x < 8; x++)
                        {
                            int worldX = (blockX << 3) + x;
                            int worldY = (blockY << 3) + y;

                            LandTile tile = tiles.GetLandTile(worldX, worldY);

                            // Write tile ID (2 bytes) and Z (1 byte)
                            writer.Write((ushort)tile.ID);
                            writer.Write((sbyte)tile.Z);
                        }
                    }
                }
            }
        }

        // TODO: Also write statics0.mul and staidx0.mul for static items
    }
}
```

**Warning:** This is complex and error-prone. Use CentrED+ instead unless you need custom export logic.

---

## Temporary Workaround: Find Good Snow Statics

While setting up .mul export, you can test with static items that look similar to snow:

### Test Command

Run `[TestSnowTiles` to create test patches of different snow static items:
- 0x0983-0x098C - Snow patches
- 0x0DEE-0x0DF0 - Snow ground
- 0x3124 - Ice/snow tile

Pick the one that looks closest to real snow and use it temporarily.

---

## Recommended Workflow for Vystia

### Phase 1: Testing (Current)
- ✅ Use static items for grass, desert, mountains, volcanic
- ⚠️ Find acceptable snow static item (or skip snow for now)
- ✅ Verify biome visuals and tile choices

### Phase 2: Full Generation
- Generate complete 7168x4096 Vystia world
- Use SetLandBlock for ALL biomes (including proper snow)
- Server has complete terrain in memory

### Phase 3: Export to Client
- Use CentrED+ to export server terrain
- Replace client .mul files
- All biomes now visible with real land tiles
- Snow appears correctly as white terrain

### Phase 4: Add Content
- Cities, dungeons, spawners
- All placed on permanent terrain
- Ready for players!

---

## Next Steps

**Immediate:**
1. Run `[TestSnowTiles` to find acceptable snow static (temporary)
2. Continue testing other biomes

**Soon:**
1. Download and configure CentrED+
2. Test exporting a small area
3. Verify client sees exported terrain

**Later:**
1. Generate full 7168x4096 Vystia world
2. Export entire map to .mul files
3. Deploy to production

---

## References

- **CentrED+ Download:** https://www.uogateway.com/centred/
- **UOFiddler GitHub:** https://github.com/polserver/UOFiddler
- **ServUO Forums:** https://www.servuo.com/forums/
- **UO Map Format Docs:** http://uo.stratics.com/heptazane/fileformats.shtml

---

*Created: 2025-10-21*
*Status: Snow static testing in progress, CentrED+ export planned*
