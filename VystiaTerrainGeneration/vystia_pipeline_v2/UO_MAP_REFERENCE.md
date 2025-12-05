# Ultima Online Map Reference
**Complete technical reference for UO map generation and configuration**

---

## Table of Contents
- [Map Dimensions](#map-dimensions)
- [Terrain Types & Palette](#terrain-types--palette)
- [Region to Terrain Mapping](#region-to-terrain-mapping)
- [UOMapMake Requirements](#uomapmake-requirements)
- [ServUO MapDefinitions](#servuo-mapdefinitions)

---

## Map Dimensions

### Standard UO Maps

| Map Index | Map Name | Width | Height | Season | Description |
|-----------|----------|-------|--------|--------|-------------|
| 0 | Felucca | 7168 | 4096 | 4 (Desolation) | Original world, PvP enabled |
| 1 | Trammel | 7168 | 4096 | 0 (Spring) | Mirror of Felucca, PvE safe zone |
| 2 | Ilshenar | 2304 | 1600 | 1 (Summer) | Lost Land expansion |
| 3 | Malas | 2560 | 2048 | 1 (Summer) | Samurai Empire expansion |
| 4 | Tokuno | 1448 | 1448 | 1 (Summer) | Tokuno Islands |
| 5 | TerMur | 1280 | 4096 | 1 (Summer) | Stygian Abyss expansion |

### Vystia Custom Map

| Map Index | Map Name | Width | Height | Season | Description |
|-----------|----------|-------|--------|--------|-------------|
| 0 | Vystia | 7168 | 4096 | 0 (Spring) | Custom hand-drawn world (replaces Felucca) |

**Season Values:**
- 0 = Spring
- 1 = Summer
- 2 = Fall
- 3 = Winter
- 4 = Desolation

---

## Terrain Types & Palette

### UO Terrain Palette (from Terrain.xml)
Used for 8-bit indexed BMP files compatible with UOMapMake.exe

| ID | Terrain Type | RGB Hex | R | G | B | Description |
|----|--------------|---------|---|---|---|-------------|
| 1 | Grass | #00FF00 | 0 | 255 | 0 | Standard grass plains |
| 2 | Stone/Rock | #A0A0A0 | 160 | 160 | 160 | Gray stone/rock |
| 3 | Wood | #8B4513 | 139 | 69 | 19 | Brown wood/planks |
| 4 | Sand | #FFFF00 | 255 | 255 | 0 | Yellow sand/desert |
| 5 | Deep Water | #0000FF | 0 | 0 | 255 | Deep blue ocean |
| 6 | Shallow Water | #00BFFF | 0 | 191 | 255 | Light blue shallow water |
| 7 | Dirt | #964B00 | 150 | 75 | 0 | Brown dirt |
| 8 | Swamp Grass | #556B2F | 85 | 107 | 47 | Dark olive swamp grass |
| 9 | Lava | #FF4500 | 255 | 69 | 0 | Orange-red lava |
| 10 | Jungle | #228B22 | 34 | 139 | 34 | Forest green jungle |
| 11 | Forest | #006400 | 0 | 100 | 0 | Dark green forest |
| 12 | Snow | #FFFFFF | 255 | 255 | 255 | White snow/ice |
| 13 | Cave Floor | #4A4A4A | 74 | 74 | 74 | Dark gray cave floor |
| 14 | Ice | #B0E0E6 | 176 | 224 | 230 | Powder blue ice |
| 15 | Ash | #696969 | 105 | 105 | 105 | Dim gray ash |
| 16 | Marble | #F5F5F5 | 245 | 245 | 245 | White smoke marble |
| 17 | Metal | #708090 | 112 | 128 | 144 | Slate gray metal |
| 18 | Rock | #808080 | 128 | 128 | 128 | Gray rock/mountain |
| 19 | Cave Wall | #2F4F4F | 47 | 79 | 79 | Dark slate gray cave wall |
| 20 | Mountain Cave | #3C3C3C | 60 | 60 | 60 | Very dark gray mountain cave |
| 21 | Dungeon Cave | #1C1C1C | 28 | 28 | 28 | Almost black dungeon cave |
| 22 | Brick | #B22222 | 178 | 34 | 34 | Firebrick red brick |
| 23 | Cobblestone | #A9A9A9 | 169 | 169 | 169 | Dark gray cobblestone |
| 24 | Flagstone | #BEBEBE | 190 | 190 | 190 | Gray flagstone |
| 25 | Mossy Stone | #556B2F | 85 | 107 | 47 | Dark olive mossy stone |
| 26 | Cracked Earth | #8B4513 | 139 | 69 | 19 | Saddle brown cracked earth |
| 27 | Volcanic Rock | #8B0000 | 139 | 0 | 0 | Dark red volcanic rock |
| 28 | Crystal | #E0FFFF | 224 | 255 | 255 | Light cyan crystal |
| 29 | Obsidian | #000000 | 0 | 0 | 0 | Black obsidian |
| 30 | Void | #0F0F0F | 15 | 15 | 15 | Near-black void |
| 31 | Charcoal | #36454F | 54 | 69 | 79 | Charcoal gray |
| 32 | Slate | #708090 | 112 | 128 | 144 | Slate gray |
| 33 | Granite | #C0C0C0 | 192 | 192 | 192 | Silver granite |
| 34 | Sandstone | #F4A460 | 244 | 164 | 96 | Sandy brown sandstone |
| 35 | Limestone | #FFFACD | 255 | 250 | 205 | Lemon chiffon limestone |
| 36 | Shale | #778899 | 119 | 136 | 153 | Light slate gray shale |
| 37 | Clay | #D2691E | 210 | 105 | 30 | Chocolate clay |
| 38 | Mud | #654321 | 101 | 67 | 33 | Dark brown mud |
| 39 | Peat | #3D2817 | 61 | 40 | 23 | Very dark brown peat |
| 40 | Moss | #8FBC8F | 143 | 188 | 143 | Dark sea green moss |
| 41 | Swamp | #2F4F2F | 47 | 79 | 47 | Dark green swamp |
| 42 | Bog | #6B8E23 | 107 | 142 | 35 | Olive drab bog |
| 43 | Lichen | #9ACD32 | 154 | 205 | 50 | Yellow green lichen |
| 44 | Fungus | #8B008B | 139 | 0 | 139 | Dark magenta fungus |
| 45 | Coral | #FF7F50 | 255 | 127 | 80 | Coral orange |

---

## Region to Terrain Mapping

### Vystia World Regions
Mapping of lore regions to UO terrain types

| Region Name | Terrain Type | UO ID | RGB Color | Description |
|-------------|--------------|-------|-----------|-------------|
| Frosthold | Snow | 12 | #FFFFFF | Northern arctic tundra |
| Winterguard | Snow | 12 | #FFFFFF | Frozen northern reaches |
| Verdantpeak | Forest | 11 | #006400 | Mountain forests |
| Skyreach Peak | Rock | 18 | #808080 | High mountain peaks |
| Ironclad Empire | Rock | 18 | #808080 | Rocky highlands |
| Radiant Plains | Grass | 1 | #00FF00 | Golden grasslands |
| Hollow Forests | Forest | 11 | #006400 | Dense dark forests |
| Mystic Canyons | Rock | 18 | #808080 | Canyon lands |
| Deepforge | Mountain Cave | 20 | #3C3C3C | Underground mountain caves |
| Emberlands | Sand | 4 | #FFFF00 | Volcanic ash desert |
| Whispering Sands | Sand | 4 | #FFFF00 | Great desert |
| Blazing Frontier | Sand | 4 | #FFFF00 | Southern desert |
| Golden Steppe | Grass | 1 | #00FF00 | Rolling grasslands |
| Shadowfen | Swamp | 41 | #2F4F2F | Dark wetlands |
| Wilderlands | Forest | 11 | #006400 | Wild untamed forests |
| Forgotten Depths | Dungeon Cave | 21 | #1C1C1C | Deep underground dungeons |
| Verdant Isles | Forest | 11 | #006400 | Forested archipelago |
| Glimmering Archipelago | Shallow Water | 6 | #00BFFF | Island chain |
| Eternal Twilight | Forest | 11 | #006400 | Mystical twilight forest |
| Crystal Barrens | Rock | 18 | #808080 | Crystalline wasteland |
| Sunken Isles | Shallow Water | 6 | #00BFFF | Partially submerged islands |
| Ocean (default) | Deep Water | 5 | #0000FF | Open ocean |

### Region Statistics (Vystia 7168×4096)
- **Total pixels:** 29,360,128
- **Land pixels:** 4,137,452 (14%)
- **Ocean pixels:** 25,222,676 (86%)
- **Unique terrain types:** 10
- **Procedural islands:** 84 generated

---

## UOMapMake Requirements

### Required Files
UOMapMake.exe needs an "Import Files" directory containing:

```
Import Files/
├── art.mul       (59,793,412 bytes) - Tile artwork definitions
├── artidx.mul    (786,432 bytes)    - Art index
└── tiledata.mul  (1,036,288 bytes)  - Tile data/properties
```

These files must be copied from a UO client installation.

### Input BMP Format
**Terrain.bmp:**
- Format: 8-bit indexed color BMP (BMPv3)
- Palette: 256-color palette using UO terrain RGB values
- Dimensions: Must match target map size exactly
- Bit depth: 8 bits per pixel
- Compression: None (BI_RGB)

**Altitude.bmp:**
- Format: 8-bit indexed color BMP (BMPv3)
- Palette: 256-level grayscale (0-255)
- Dimensions: Must match Terrain.bmp exactly
- Bit depth: 8 bits per pixel
- Compression: None (BI_RGB)
- Values: 0=sea level, 255=maximum height

### Output Files
UOMapMake generates (for Map0 example):
- `Map0.mul` - Terrain and altitude data
- `Statics0.mul` - Static item/decoration data
- `StaIdx0.mul` - Static item index

### File Sizes (7168×4096)
- Map0.mul: ~117 MB (7168 × 4096 × 196 bytes per block)
- Statics0.mul: Variable (depends on placed items)
- StaIdx0.mul: ~2.9 MB (index entries)

---

## ServUO MapDefinitions

### Configuration Example

```csharp
// C:\DevEnv\GIT\UO\ServUO\Scripts\Misc\MapDefinitions.cs

public static void Configure()
{
    // Vystia - Custom world replacing Felucca
    RegisterMap(
        0,                      // mapIndex - Map 0 (Felucca slot)
        0,                      // mapID - Map ID 0
        0,                      // fileIndex - uses map0.mul files
        7168, 4096,             // map dimensions (width, height)
        0,                      // season: 0 = Spring
        "Vystia",               // map name
        MapRules.FeluccaRules   // rule set (PvP enabled)
    );

    // Other standard maps
    RegisterMap(1, 1, 1, 7168, 4096, 0, "Trammel", MapRules.TrammelRules);
    RegisterMap(2, 2, 2, 2304, 1600, 1, "Ilshenar", MapRules.TrammelRules);
    RegisterMap(3, 3, 3, 2560, 2048, 1, "Malas", MapRules.TrammelRules);
    RegisterMap(4, 4, 4, 1448, 1448, 1, "Tokuno", MapRules.TrammelRules);
    RegisterMap(5, 5, 5, 1280, 4096, 1, "TerMur", MapRules.TrammelRules);
}
```

### RegisterMap Parameters

```csharp
RegisterMap(
    int mapIndex,      // Unique index (0-31 for standard maps)
    int mapID,         // Client communication ID (0-5 for visible maps)
    int fileIndex,     // File ID for .mul files (0-5 for visible maps)
    int width,         // Map width in tiles
    int height,        // Map height in tiles
    int season,        // 0=Spring, 1=Summer, 2=Fall, 3=Winter, 4=Desolation
    string name,       // Display name
    MapRules rules     // Rule set (FeluccaRules, TrammelRules, Internal)
);
```

### Map Rules

**MapRules.FeluccaRules:**
- PvP enabled
- Full looting on death
- Criminal system active
- Housing allowed

**MapRules.TrammelRules:**
- PvP disabled (safe zone)
- Limited looting on death
- No criminal system
- Housing allowed

---

## File Locations

### Pipeline Output
```
C:\DevEnv\GIT\UO\VystiaGeneration\vystia_pipeline_v2\
├── Terrain.bmp       - Generated terrain (7168×4096)
├── Altitude.bmp      - Generated altitude (7168×4096)
├── exports/
│   ├── Terrain.bmp   - Backup copy
│   ├── Altitude.bmp  - Backup copy
│   ├── regions_final.png
│   ├── terrain_uo.png
│   └── validation_report.md
└── preview/
    ├── 00_expanded_world.png
    ├── 10_regions_preview.png
    ├── 45_uo_terrain.png
    ├── 50_terrain_final.png
    └── 50_altitude_final.png
```

### UO Client Files
```
C:\UORenaissance\
├── map0.mul      - Generated by UOMapMake
├── statics0.mul  - Generated by UOMapMake
├── staidx0.mul   - Generated by UOMapMake
├── art.mul       - UO client files
├── artidx.mul
└── tiledata.mul
```

### ServUO Server
```
C:\DevEnv\GIT\UO\ServUO\
├── Scripts.dll              - Compiled scripts
├── Scripts\
│   ├── Misc\
│   │   └── MapDefinitions.cs
│   └── Commands\
│       └── VystiaCityGenerator.cs
└── Data\
    └── Regions.xml          - Region definitions
```

---

## Workflow Summary

### 1. Generate Map Files
```bash
cd C:\DevEnv\GIT\UO\VystiaGeneration\vystia_pipeline_v2
python build_all_v2.py
```
Output: `Terrain.bmp` and `Altitude.bmp` (7168×4096)

### 2. Run UOMapMake
```
1. Copy "Import Files" folder to pipeline root
2. Run UOMapMake.exe from pipeline root
3. Select Terrain.bmp and Altitude.bmp as inputs
4. Output generates Map0.mul, Statics0.mul, StaIdx0.mul
```

### 3. Deploy to Client
```bash
copy Map0.mul C:\UORenaissance\
copy Statics0.mul C:\UORenaissance\
copy StaIdx0.mul C:\UORenaissance\
```

### 4. Configure ServUO
```csharp
// Update MapDefinitions.cs
RegisterMap(0, 0, 0, 7168, 4096, 0, "Vystia", MapRules.FeluccaRules);
```

### 5. Build & Deploy
```bash
dotnet build Scripts\Scripts.csproj --configuration Release
copy Scripts\bin\Release\Scripts.dll Scripts.dll
```

### 6. Test In-Game
```
[GoVystia 3584 2048 0  // Center of 7168×4096 map
[WhereAmI              // Verify location
```

---

## Technical Notes

### Color Palette Indexing
- BMP files use 8-bit indexed color (256 colors max)
- Terrain IDs map directly to palette indices
- Color values must match exactly for UOMapMake recognition
- Ocean (ID 5, #0000FF) should be the dominant color

### Altitude Mapping
- Altitude values are quantized to 7-10 levels
- Ocean = 0 (sea level)
- Plains = 10-20
- Hills = 30-50
- Mountains = 80-100
- Peaks = 100+ (max 255)

### Performance Considerations
- 7168×4096 = 29,360,128 pixels
- File size: ~84 MB for both BMPs
- UOMapMake processing: ~30-60 seconds
- Client rendering: Handled by UO client engine

---

## In-Game Commands

### Navigation Commands

**`[GoVystia <x> <y> [z]`**
- Teleports to coordinates on Vystia map (Map 0 or 2)
- Example: `[GoVystia 3584 2048 0` (center of 7168×4096)
- Example: `[GoVystia 1152 800 0` (center of 2304×1600)
- Requires: Administrator access level

**`[Go <map>`**
- Teleports to default location on specified map
- Example: `[Go Felucca`
- Example: `[Go Ilshenar`
- Example: `[Go Trammel`

**`[Tele <x> <y> [z] [mapid]`**
- Standard teleport command
- Example: `[Tele 1000 1000 0 0` (to Map 0)
- Example: `[Tele 1152 800 0 2` (to Map 2)

### Location & Debug Commands

**`[WhereAmI`**
- Shows current location details:
  - Position (X, Y, Z)
  - Map name
  - Map ID
  - Map Index
- Example output:
  ```
  Position: (3584, 2048, 0)
  Map: Vystia
  Map ID: 0
  Map Index: 0
  ```

**`[Where`**
- Shows basic coordinates
- Format: `X: 1234 Y: 5678 Z: 0 Map: Felucca`

**`[GetZ`**
- Returns Z coordinate (altitude) at current location
- Useful for terrain height debugging

### Item & Tile Commands

**`[Tile <itemID>`**
- Spawns a static tile at your feet
- Example: `[Tile 0x9` (stone floor)
- Example: `[Tile 0x31F4` (dirt)
- Requires: Static item ID in hex format

**`[TileXYZ <itemID> <x> <y> <z>`**
- Spawns tile at specific coordinates
- Example: `[TileXYZ 0x9 1000 1000 0`

**`[Add <item/mobile name>`**
- Spawns item or creature at your location
- Example: `[Add Mongbat`
- Example: `[Add Dirt`
- Example: `[Add StaticTarget`

**`[Props`**
- Opens properties gump for targeted object
- Click on any item/mobile after using command
- Shows: Body ID, Hue, Location, ItemID, etc.

**`[Get ItemID`** or **`[Get StaticID`**
- Target a static tile to see its ID
- Returns hex ID value (e.g., 0x31F4)

**`[Static <show|hide>`**
- Toggles static item visibility
- Useful for debugging map decoration

### Spawning & Generation Commands

**`[TestSpawn`**
- Spawns a test mongbat at your location
- Verifies creature rendering on custom maps
- Requires: Administrator access level

**`[CleanMap2`**
- Removes all spawners, mobiles, and items from Map 2 (Ilshenar)
- Shows deletion count summary
- Useful for cleaning tutorial/test maps
- Requires: Administrator access level

**`[GenIronheart`**
- Generates Ironheart Capital city (if implemented)
- Creates buildings, NPCs, spawners
- Requires: Administrator access level

**`[ClearIronheart`**
- Clears Ironheart Capital from map
- Removes all city-related items and mobiles
- Requires: Administrator access level

**`[FillGrass <size>`**
- Fills large area with grass tiles for testing
- Sizes: small (500), medium (1000), large (2000), huge (4000)
- Example: `[FillGrass medium`
- Requires: Administrator access level

### Map & Region Commands

**`[SetMap <mapid>`**
- Changes your current map
- Example: `[SetMap 0` (Felucca/Vystia)
- Example: `[SetMap 2` (Ilshenar)

**`[MapInfo`**
- Shows information about all loaded maps
- Displays: Map name, dimensions, tile count

**`[RegionInfo`**
- Shows current region information
- Displays: Region name, coordinates, rules

### Access Level Commands

**`[Admin`**
- Sets your access level to Administrator
- Required for most map editing commands

**`[GM`**
- Sets access level to GameMaster
- Limited editing capabilities

**`[Player`**
- Sets access level back to Player
- Removes command access

### Item Search Commands

**`[FindItem <name>`**
- Searches for items by name
- Example: `[FindItem Door`
- Returns list of matching items with IDs

**`[FindMobile <name>`**
- Searches for mobile types by name
- Example: `[FindMobile Dragon`
- Returns list of creature types

**`[FindTile <keyword>`**
- Searches for static tiles by keyword
- Example: `[FindTile grass`
- Example: `[FindTile stone`

### Tile ID Reference Commands

**`[TileInfo <id>`**
- Shows information about a tile ID
- Example: `[TileInfo 0x31F4`
- Shows: Name, flags, height, properties

**`[ItemInfo <id>`**
- Shows information about an item ID
- Example: `[ItemInfo 0x9`
- Shows: Name, weight, stackable, etc.

### Measurement Commands

**`[Measure`**
- Measures distance between two points
- Click start point, then end point
- Returns: Distance in tiles, X/Y differences

**`[Area`**
- Calculates area of rectangular region
- Click two corners
- Returns: Width, height, total tiles

### Common Tile IDs

#### Terrain Tiles (for [Tile command)
```
0x31F4 - Dirt
0x9    - Stone floor
0x15   - Grass
0x16   - More grass
0x17   - More grass
0x18   - Sand
0x3D   - Water
0x51   - Cave floor
0x5A   - Snow
0x71   - Swamp
0x82   - Lava
```

#### Decoration Items
```
0xC2C  - Tree (oak)
0xCAD  - Palm tree
0xD94  - Rock (small)
0xD95  - Rock (medium)
0xD96  - Rock (large)
0x11EA - Grass tuft
0x1B7D - Mountain rock
```

### Command Usage Tips

1. **Use [Props frequently** - Essential for debugging invisible items
2. **[WhereAmI before teleporting** - Always know where you are
3. **[Add for quick testing** - Faster than manual spawning
4. **[CleanMap2 before fresh tests** - Start with clean slate
5. **[TestSpawn to verify rendering** - Ensure creatures display correctly

### Coordinate Reference Points

#### Vystia (7168×4096)
- Center: `3584, 2048`
- NW Corner: `0, 0`
- NE Corner: `7167, 0`
- SW Corner: `0, 4095`
- SE Corner: `7167, 4095`
- North (Frosthold region): `~2200, 1000`
- South (Blazing Frontier): `~2900, 1800`

#### Ilshenar/Tutorial (2304×1600)
- Center: `1152, 800`
- NW Corner: `0, 0`
- NE Corner: `2303, 0`
- SW Corner: `0, 1599`
- SE Corner: `2303, 1599`

### Example Command Sequences

#### Testing New Map
```
[Admin
[WhereAmI
[GoVystia 3584 2048 0
[WhereAmI
[TestSpawn
[Props (click on mongbat)
```

#### Finding Specific Tile Type
```
[Admin
[FindTile grass
[Tile 0x15
[Props (click on tile)
[Get ItemID (target tile)
```

#### Cleaning and Regenerating Area
```
[Admin
[GoVystia 1000 1000 0
[CleanMap2
[FillGrass medium
[Add Mongbat
```

#### Measuring Map Distance
```
[Admin
[GoVystia 0 0 0
[WhereAmI
[GoVystia 7167 4095 0
[WhereAmI
(Manual calculation: 7167 - 0 = 7167 tiles wide)
```

---

**Last Updated:** 2025-10-27
**Vystia Pipeline Version:** 2.0
**Target UO Version:** ServUO (Modern)
