# Vystia Terrain Generation Guide

## Overview

This guide explains two approaches to generating the Vystia world terrain in ServUO. The core issue is that previous attempts only created **decorative items** (rocks, grass objects) but didn't modify the **actual map terrain tiles** that the client renders.

## Understanding UO Map Architecture

### Map File Structure

Ultima Online maps consist of several components:

1. **Land Tiles (`map*.mul`)** - Base terrain layer (grass, dirt, stone, water)
   - Defines tile type (ID)
   - Defines elevation (Z-level)
   - Organized in 8x8 blocks (64 tiles per block)

2. **Static Tiles (`statics*.mul`, `staidx*.mul`)** - Permanent objects (trees, buildings, rocks)
   - Placed on top of land tiles
   - Have X, Y, Z coordinates and item IDs

3. **Dynamic Items** - Runtime objects (created by scripts)
   - What your current generators create
   - Stored in server saves, not map files

### Why Your Current Approach Doesn't Work

Your existing generators (`VystiaWorldGenerator.cs`, `VystiaCityGenerator.cs`) create **Item** and **Static** objects:

```csharp
Item grass = new Item(0x31F4); // Creates a decorative item
grass.MoveToWorld(new Point3D(x, y, 0), map);
```

**Problem:** This places objects on the map, but the **underlying terrain** is still reading from the original Felucca `map0.mul` file, which has its own terrain (Britain, dungeons, etc.). Your items are floating on top of existing terrain or appearing in "void" areas.

### The Solution

You need to modify the **actual land tiles** using ServUO's `TileMatrix` API:

```csharp
// Get the map's tile matrix
TileMatrix tiles = map.Tiles;

// Create a land tile with specific type and elevation
LandTile tile = new LandTile(tileID, elevation);

// Set it in the map
tiles.SetLandBlock(blockX, blockY, landTileArray);
```

---

## Map Selection for Vystia

### Available Maps in ServUO

From `MapDefinitions.cs`:

| Map # | Name | Size | Notes |
|-------|------|------|-------|
| 0 | Felucca | 7168 x 4096 | ✅ **Best for Vystia** - Correct size |
| 1 | Trammel | 7168 x 4096 | Mirror of Felucca |
| 2 | Ilshenar | 2304 x 1600 | Too small |
| 3 | Malas | 2560 x 2048 | Too small |
| 4 | Tokuno | 1448 x 1448 | Too small |
| 5 | TerMur | 1280 x 4096 | ❌ Too narrow (wrong dimensions) |

**Decision:** Use **Map 0 (Felucca)** - It's already 7168x4096, matching your Vystia config perfectly.

### Current Inconsistency Issue

Your scripts currently conflict:

- `VystiaCityGenerator.cs` line 89: `Map map = Map.TerMur; // Map 5` ❌
- `VystiaWorldGenerator.cs` line 68: `Map map = Map.Felucca; // Map 0` ✅
- `Vystia_World_Config.json` line 6: `"map": "Felucca"` ✅

**Fix:** All scripts must use `Map.Felucca` (Map 0) consistently.

---

## Approach B: Quick Fix (Simple Terrain Test)

### Goal
Get basic terrain working quickly on Map 0 to verify the system works before building the full procedural generator.

### What This Does
- Uses Map 0 (Felucca) consistently across all scripts
- Generates **actual land tiles** (not just decorative items)
- Creates simple test regions with different tile types and elevations
- Proves the terrain generation pipeline works

### Implementation Steps

#### 1. Fix Map Inconsistencies

**File:** `C:\DevEnv\GIT\UO\ServUO\Scripts\Commands\VystiaCityGenerator.cs`

**Change line 89** from:
```csharp
Map map = Map.TerMur; // Map 5
```

To:
```csharp
Map map = Map.Felucca; // Map 0 - Vystia uses Felucca
```

**Change line 140** from:
```csharp
e.Mobile.Map = Map.TerMur; // Map 5
```

To:
```csharp
e.Mobile.Map = Map.Felucca; // Map 0 - Vystia
```

#### 2. Create Simple Terrain Test Command

Create a new file: `C:\DevEnv\GIT\UO\ServUO\Scripts\Commands\VystiaTerrainTest.cs`

```csharp
using System;
using Server;
using Server.Commands;

namespace Server.Commands
{
    public class VystiaTerrainTest
    {
        public static void Initialize()
        {
            CommandSystem.Register("VystiaTerrainTest", AccessLevel.Administrator,
                new CommandEventHandler(TerrainTest_OnCommand));
        }

        [Usage("VystiaTerrainTest")]
        [Description("Creates test terrain patches on Map 0 to verify terrain generation works")]
        public static void TerrainTest_OnCommand(CommandEventArgs e)
        {
            e.Mobile.SendMessage("Starting Vystia terrain test on Map 0...");

            Map map = Map.Felucca;
            TileMatrix tiles = map.Tiles;

            // Test Area 1: Grassland (1000, 1000) - 64x64 tiles
            e.Mobile.SendMessage("Creating grassland test area at (1000, 1000)...");
            CreateTerrainPatch(tiles, 1000, 1000, 64, 64, 0x0003, 0); // Grass, flat

            // Test Area 2: Mountain (1100, 1000) - 64x64 tiles with elevation
            e.Mobile.SendMessage("Creating mountain test area at (1100, 1000)...");
            CreateTerrainPatch(tiles, 1100, 1000, 64, 64, 0x00DC, 25); // Stone, elevated

            // Test Area 3: Snow (1200, 1000) - 64x64 tiles
            e.Mobile.SendMessage("Creating snow test area at (1200, 1000)...");
            CreateTerrainPatch(tiles, 1200, 1000, 64, 64, 0x0480, 0); // Snow, flat

            // Test Area 4: Desert (1300, 1000) - 64x64 tiles
            e.Mobile.SendMessage("Creating desert test area at (1300, 1000)...");
            CreateTerrainPatch(tiles, 1300, 1000, 64, 64, 0x0029, 0); // Sand, flat

            // Test Area 5: Volcanic (1400, 1000) - 64x64 tiles
            e.Mobile.SendMessage("Creating volcanic test area at (1400, 1000)...");
            CreateTerrainPatch(tiles, 1400, 1000, 64, 64, 0x01AE, 10); // Lava rock, slightly elevated

            e.Mobile.SendMessage("Terrain test complete!");
            e.Mobile.SendMessage("Teleport to test areas:");
            e.Mobile.SendMessage("  Grassland: [Go 1032 1032 0");
            e.Mobile.SendMessage("  Mountain:  [Go 1132 1032 25");
            e.Mobile.SendMessage("  Snow:      [Go 1232 1032 0");
            e.Mobile.SendMessage("  Desert:    [Go 1332 1032 0");
            e.Mobile.SendMessage("  Volcanic:  [Go 1432 1032 10");
        }

        private static void CreateTerrainPatch(TileMatrix tiles, int startX, int startY,
            int width, int height, short tileID, sbyte baseZ)
        {
            // Calculate block coordinates (blocks are 8x8 tiles)
            int startBlockX = startX >> 3; // Divide by 8
            int startBlockY = startY >> 3;
            int endBlockX = (startX + width) >> 3;
            int endBlockY = (startY + height) >> 3;

            // Iterate through each 8x8 block
            for (int blockX = startBlockX; blockX <= endBlockX; blockX++)
            {
                for (int blockY = startBlockY; blockY <= endBlockY; blockY++)
                {
                    // Create a new land tile block (8x8 = 64 tiles)
                    LandTile[] block = new LandTile[64];

                    // Fill the block with tiles
                    for (int i = 0; i < 64; i++)
                    {
                        // Calculate world coordinates for this tile
                        int tileX = (blockX << 3) + (i & 0x7); // i % 8
                        int tileY = (blockY << 3) + (i >> 3);  // i / 8

                        // Check if this tile is within our patch area
                        if (tileX >= startX && tileX < startX + width &&
                            tileY >= startY && tileY < startY + height)
                        {
                            // Create varied elevation for more interesting terrain
                            sbyte z = baseZ;
                            if (tileID == 0x00DC) // Mountain - add variation
                            {
                                z = (sbyte)(baseZ + ((tileX + tileY) % 15));
                            }

                            block[i] = new LandTile(tileID, z);
                        }
                        else
                        {
                            // Outside our patch - read existing tile
                            LandTile[] existingBlock = tiles.GetLandBlock(blockX, blockY);
                            block[i] = existingBlock[i];
                        }
                    }

                    // Set the modified block back into the map
                    tiles.SetLandBlock(blockX, blockY, block);
                }
            }
        }
    }
}
```

#### 3. Test the System

1. **Rebuild ServUO:**
   ```bash
   cd C:\DevEnv\GIT\UO\ServUO
   dotnet build
   ```

2. **Start the server**

3. **Log in as admin**

4. **Run the test command:**
   ```
   [VystiaTerrainTest
   ```

5. **Teleport to test areas:**
   ```
   [Go 1032 1032 0
   [Go 1132 1032 25
   [Go 1232 1032 0
   [Go 1332 1032 0
   [Go 1432 1032 10
   ```

6. **Verify you see:**
   - Grass tiles at first location
   - Stone mountain at second location (elevated)
   - Snow at third location
   - Desert sand at fourth location
   - Volcanic rock at fifth location

### Common Tile IDs for Testing

| Terrain Type | Tile ID (hex) | Tile ID (dec) | Notes |
|--------------|---------------|---------------|-------|
| Grass | 0x0003 | 3 | Standard green grass |
| Dirt | 0x0009 | 9 | Brown dirt |
| Stone | 0x00DC | 220 | Gray stone (mountains) |
| Sand | 0x0029 | 41 | Desert sand |
| Snow | 0x0480 | 1152 | White snow |
| Swamp | 0x015F | 351 | Murky swamp |
| Water | 0x00A8 | 168 | Deep water |
| Lava | 0x01AE | 430 | Volcanic lava |
| Forest Floor | 0x000B | 11 | Dark forest ground |
| Cave Floor | 0x0259 | 601 | Underground cave |

### Troubleshooting Quick Fix

**Problem:** Tiles don't appear or client crashes

**Solution:** Make sure you're:
- Using valid tile IDs (0x0001 to 0x3FFF)
- Setting Z values between -127 and 127
- Not creating tiles outside map bounds

**Problem:** Changes don't persist after restart

**Solution:** The `SetLandBlock` approach modifies runtime data only. For persistence, you'd need to:
- Export modified tiles to .mul files, OR
- Re-run terrain generation on each server start, OR
- Implement a terrain save/load system

**Problem:** Client shows "green acres" or void

**Solution:**
- Verify you're using Map 0 (Felucca)
- Check that tile IDs are valid
- Ensure client has proper map files

---

## Approach A: Full Procedural Terrain Generator

### Goal
Generate the complete Vystia world with realistic terrain matching your concept art, using procedural generation techniques.

### Features
- **Perlin noise terrain generation** for realistic elevation
- **Biome-based tile assignment** using your `Vystia_World_Config.json`
- **Multi-pass smoothing** for natural transitions
- **Coastal detection** for proper water/land boundaries
- **Island generation** for archipelagos
- **Mountain ranges** with realistic slopes
- **River and lake generation** (optional)

### Architecture Overview

```
VystiaProceduralGenerator.cs
├── TerrainGenerator
│   ├── GenerateHeightMap() - Perlin noise elevation
│   ├── GenerateMoistureMap() - Rainfall/humidity
│   ├── AssignBiomes() - Classify tiles by elevation + moisture
│   └── ApplySmoothing() - Blend transitions
├── BiomeMapper
│   ├── GetBiomeForConditions() - Match config rules
│   └── GetTileIDForBiome() - Tile ID from biome type
├── TileWriter
│   ├── WriteLandTiles() - Set actual terrain tiles
│   └── OptimizeBlocks() - Efficient block writing
└── Validator
    ├── ValidateMapBounds() - Check dimensions
    └── ValidateGeneration() - Verify output quality
```

### Implementation Components

#### 1. Perlin Noise Generator

```csharp
public class PerlinNoise
{
    private readonly int[] permutation;
    private readonly Random random;

    public PerlinNoise(int seed)
    {
        random = new Random(seed);
        permutation = new int[512];

        // Initialize permutation table
        for (int i = 0; i < 256; i++)
            permutation[i] = i;

        // Shuffle using seed
        for (int i = 0; i < 256; i++)
        {
            int j = random.Next(256);
            int temp = permutation[i];
            permutation[i] = permutation[j];
            permutation[j] = temp;
        }

        // Duplicate for easier indexing
        for (int i = 0; i < 256; i++)
            permutation[256 + i] = permutation[i];
    }

    public double Noise(double x, double y)
    {
        // Classic Perlin noise implementation
        int xi = (int)Math.Floor(x) & 255;
        int yi = (int)Math.Floor(y) & 255;

        double xf = x - Math.Floor(x);
        double yf = y - Math.Floor(y);

        double u = Fade(xf);
        double v = Fade(yf);

        int aa = permutation[permutation[xi] + yi];
        int ab = permutation[permutation[xi] + yi + 1];
        int ba = permutation[permutation[xi + 1] + yi];
        int bb = permutation[permutation[xi + 1] + yi + 1];

        double x1 = Lerp(Grad(aa, xf, yf), Grad(ba, xf - 1, yf), u);
        double x2 = Lerp(Grad(ab, xf, yf - 1), Grad(bb, xf - 1, yf - 1), u);

        return Lerp(x1, x2, v);
    }

    public double OctaveNoise(double x, double y, int octaves, double persistence, double lacunarity)
    {
        double total = 0;
        double frequency = 1;
        double amplitude = 1;
        double maxValue = 0;

        for (int i = 0; i < octaves; i++)
        {
            total += Noise(x * frequency, y * frequency) * amplitude;
            maxValue += amplitude;
            amplitude *= persistence;
            frequency *= lacunarity;
        }

        return total / maxValue;
    }

    private double Fade(double t)
    {
        return t * t * t * (t * (t * 6 - 15) + 10);
    }

    private double Lerp(double a, double b, double t)
    {
        return a + t * (b - a);
    }

    private double Grad(int hash, double x, double y)
    {
        int h = hash & 3;
        double u = h < 2 ? x : y;
        double v = h < 2 ? y : x;
        return ((h & 1) == 0 ? u : -u) + ((h & 2) == 0 ? v : -v);
    }
}
```

#### 2. Height Map Generator

```csharp
public class HeightMapGenerator
{
    private readonly PerlinNoise noise;
    private readonly int width;
    private readonly int height;
    private readonly double scale;
    private readonly int octaves;
    private readonly double persistence;
    private readonly double lacunarity;

    public HeightMapGenerator(int seed, int width, int height,
        double scale = 140.0, int octaves = 7,
        double persistence = 0.55, double lacunarity = 2.1)
    {
        noise = new PerlinNoise(seed);
        this.width = width;
        this.height = height;
        this.scale = scale;
        this.octaves = octaves;
        this.persistence = persistence;
        this.lacunarity = lacunarity;
    }

    public double[,] Generate()
    {
        double[,] heightMap = new double[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                double nx = x / scale;
                double ny = y / scale;

                // Generate base elevation using octave noise
                double elevation = noise.OctaveNoise(nx, ny, octaves, persistence, lacunarity);

                // Normalize to 0-1 range
                elevation = (elevation + 1.0) / 2.0;

                // Apply distance-based falloff for island creation (optional)
                // elevation *= CalculateIslandMask(x, y, width, height);

                heightMap[x, y] = elevation;
            }
        }

        return heightMap;
    }

    private double CalculateIslandMask(int x, int y, int width, int height)
    {
        // Create island falloff from center
        double centerX = width / 2.0;
        double centerY = height / 2.0;
        double maxDistance = Math.Sqrt(centerX * centerX + centerY * centerY);
        double distance = Math.Sqrt(Math.Pow(x - centerX, 2) + Math.Pow(y - centerY, 2));
        double distanceRatio = distance / maxDistance;

        // Falloff curve
        return Math.Max(0, 1.0 - Math.Pow(distanceRatio, 2));
    }
}
```

#### 3. Moisture Map Generator

```csharp
public class MoistureMapGenerator
{
    private readonly PerlinNoise noise;
    private readonly int width;
    private readonly int height;

    public MoistureMapGenerator(int seed, int width, int height)
    {
        noise = new PerlinNoise(seed + 1000); // Offset seed for different pattern
        this.width = width;
        this.height = height;
    }

    public double[,] Generate()
    {
        double[,] moistureMap = new double[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                double nx = x / 200.0; // Different scale than elevation
                double ny = y / 200.0;

                double moisture = noise.OctaveNoise(nx, ny, 4, 0.5, 2.0);
                moisture = (moisture + 1.0) / 2.0; // Normalize to 0-1

                moistureMap[x, y] = moisture;
            }
        }

        return moistureMap;
    }
}
```

#### 4. Biome Classifier

```csharp
public class BiomeClassifier
{
    private readonly List<BiomeDefinition> biomes;

    public class BiomeDefinition
    {
        public string Name { get; set; }
        public short TileID { get; set; }
        public double MinElevation { get; set; }
        public double MaxElevation { get; set; }
        public double MinMoisture { get; set; }
        public double MaxMoisture { get; set; }
        public double Frequency { get; set; }
    }

    public BiomeClassifier(List<BiomeDefinition> biomes)
    {
        this.biomes = biomes;
    }

    public BiomeDefinition ClassifyBiome(double elevation, double moisture)
    {
        // Normalize elevation to 0-100 range for comparison with config
        double normalizedElevation = elevation * 100.0;

        // Find matching biomes
        foreach (var biome in biomes)
        {
            if (normalizedElevation >= biome.MinElevation &&
                normalizedElevation <= biome.MaxElevation &&
                moisture >= biome.MinMoisture &&
                moisture <= biome.MaxMoisture)
            {
                return biome;
            }
        }

        // Default to grassland if no match
        return biomes.Find(b => b.Name == "grassland") ?? biomes[0];
    }
}
```

#### 5. Main Terrain Generator

```csharp
using System;
using System.Collections.Generic;
using Server;
using Server.Commands;
using Newtonsoft.Json;
using System.IO;

namespace Server.Commands
{
    public class VystiaProceduralGenerator
    {
        public static void Initialize()
        {
            CommandSystem.Register("GenVystiaFull", AccessLevel.Administrator,
                new CommandEventHandler(GenerateFull_OnCommand));
        }

        [Usage("GenVystiaFull")]
        [Description("Generates the complete Vystia world using procedural terrain generation")]
        public static void GenerateFull_OnCommand(CommandEventArgs e)
        {
            e.Mobile.SendMessage("Starting full Vystia procedural generation...");
            e.Mobile.SendMessage("WARNING: This will take 20-40 minutes!");
            e.Mobile.SendMessage("Loading configuration...");

            // Load config
            string configPath = Path.Combine(Core.BaseDirectory, "Data", "WorldGen", "Vystia_World_Config.json");
            if (!File.Exists(configPath))
            {
                e.Mobile.SendMessage("ERROR: Config file not found at: " + configPath);
                return;
            }

            var config = JsonConvert.DeserializeObject<VystiaConfig>(File.ReadAllText(configPath));

            e.Mobile.SendMessage($"Loaded config: {config.world_config.name}");
            e.Mobile.SendMessage($"Seed: {config.world_config.seed}");
            e.Mobile.SendMessage($"Dimensions: {config.world_config.dimensions.width} x {config.world_config.dimensions.height}");

            // Generate terrain
            GenerateTerrain(e.Mobile, config);
        }

        private static void GenerateTerrain(Mobile from, VystiaConfig config)
        {
            Map map = Map.Felucca;
            int width = config.world_config.dimensions.width;
            int height = config.world_config.dimensions.height;
            int seed = config.world_config.seed;

            from.SendMessage("[1/5] Generating height map...");
            HeightMapGenerator heightGen = new HeightMapGenerator(
                seed, width, height,
                config.terrain_generation.parameters.scale,
                config.terrain_generation.parameters.octaves,
                config.terrain_generation.parameters.persistence,
                config.terrain_generation.parameters.lacunarity
            );
            double[,] heightMap = heightGen.Generate();
            from.SendMessage("Height map complete!");

            from.SendMessage("[2/5] Generating moisture map...");
            MoistureMapGenerator moistureGen = new MoistureMapGenerator(seed, width, height);
            double[,] moistureMap = moistureGen.Generate();
            from.SendMessage("Moisture map complete!");

            from.SendMessage("[3/5] Classifying biomes...");
            List<BiomeClassifier.BiomeDefinition> biomeDefs = new List<BiomeClassifier.BiomeDefinition>();
            foreach (var biome in config.terrain_generation.biomes)
            {
                biomeDefs.Add(new BiomeClassifier.BiomeDefinition
                {
                    Name = biome.name,
                    TileID = (short)biome.tile_id,
                    MinElevation = biome.elevation_range[0],
                    MaxElevation = biome.elevation_range[1],
                    MinMoisture = biome.moisture_range[0],
                    MaxMoisture = biome.moisture_range[1],
                    Frequency = biome.frequency
                });
            }
            BiomeClassifier classifier = new BiomeClassifier(biomeDefs);
            from.SendMessage("Biome classification ready!");

            from.SendMessage("[4/5] Writing terrain tiles...");
            from.SendMessage("This will take 15-30 minutes...");
            WriteTerrain(map, width, height, heightMap, moistureMap, classifier, from);
            from.SendMessage("Terrain writing complete!");

            from.SendMessage("[5/5] Validating terrain...");
            ValidateTerrain(map, width, height, from);

            from.SendMessage("==============================================");
            from.SendMessage("Vystia world generation COMPLETE!");
            from.SendMessage("==============================================");
            from.SendMessage("Visit key locations:");
            from.SendMessage("  Frosthold:     [Go 1200 200 0");
            from.SendMessage("  Ironheart:     [Go 800 600 0");
            from.SendMessage("  Verdant Grove: [Go 1800 400 0");
            from.SendMessage("  Emberlands:    [Go 5400 1800 0");
        }

        private static void WriteTerrain(Map map, int width, int height,
            double[,] heightMap, double[,] moistureMap,
            BiomeClassifier classifier, Mobile from)
        {
            TileMatrix tiles = map.Tiles;
            int blockWidth = width >> 3;
            int blockHeight = height >> 3;
            int processedBlocks = 0;
            int totalBlocks = blockWidth * blockHeight;

            for (int blockX = 0; blockX < blockWidth; blockX++)
            {
                for (int blockY = 0; blockY < blockHeight; blockY++)
                {
                    LandTile[] block = new LandTile[64];

                    for (int i = 0; i < 64; i++)
                    {
                        int x = (blockX << 3) + (i & 0x7);
                        int y = (blockY << 3) + (i >> 3);

                        if (x < width && y < height)
                        {
                            double elevation = heightMap[x, y];
                            double moisture = moistureMap[x, y];

                            var biome = classifier.ClassifyBiome(elevation, moisture);

                            // Calculate Z elevation (0-127 range)
                            sbyte z = (sbyte)Math.Min(127, elevation * 100);

                            // Water tiles should be at Z=0 or below
                            if (elevation < 0.1) // Water level threshold
                            {
                                block[i] = new LandTile(0x00A8, 0); // Water tile
                            }
                            else
                            {
                                block[i] = new LandTile(biome.TileID, z);
                            }
                        }
                        else
                        {
                            block[i] = new LandTile(0x0003, 0); // Default grass
                        }
                    }

                    tiles.SetLandBlock(blockX, blockY, block);
                    processedBlocks++;

                    // Progress update every 1000 blocks
                    if (processedBlocks % 1000 == 0)
                    {
                        double progress = (processedBlocks / (double)totalBlocks) * 100.0;
                        from.SendMessage($"Progress: {progress:F1}% ({processedBlocks}/{totalBlocks} blocks)");
                    }
                }
            }
        }

        private static void ValidateTerrain(Map map, int width, int height, Mobile from)
        {
            // Count biome distribution
            Dictionary<short, int> tileCounts = new Dictionary<short, int>();
            TileMatrix tiles = map.Tiles;

            int sampleSize = 10000;
            Random rand = new Random();

            for (int i = 0; i < sampleSize; i++)
            {
                int x = rand.Next(width);
                int y = rand.Next(height);

                LandTile tile = tiles.GetLandTile(x, y);
                if (!tileCounts.ContainsKey(tile.ID))
                    tileCounts[tile.ID] = 0;
                tileCounts[tile.ID]++;
            }

            from.SendMessage("Terrain validation (sampled " + sampleSize + " tiles):");
            foreach (var kvp in tileCounts)
            {
                double percentage = (kvp.Value / (double)sampleSize) * 100.0;
                from.SendMessage($"  Tile {kvp.Key:X4}: {percentage:F1}%");
            }
        }
    }

    // Config classes for JSON deserialization
    public class VystiaConfig
    {
        public WorldConfig world_config { get; set; }
        public TerrainGeneration terrain_generation { get; set; }
    }

    public class WorldConfig
    {
        public string name { get; set; }
        public int seed { get; set; }
        public Dimensions dimensions { get; set; }
    }

    public class Dimensions
    {
        public int width { get; set; }
        public int height { get; set; }
    }

    public class TerrainGeneration
    {
        public GenerationParameters parameters { get; set; }
        public List<Biome> biomes { get; set; }
    }

    public class GenerationParameters
    {
        public int octaves { get; set; }
        public double persistence { get; set; }
        public double lacunarity { get; set; }
        public double scale { get; set; }
    }

    public class Biome
    {
        public string name { get; set; }
        public int tile_id { get; set; }
        public double[] elevation_range { get; set; }
        public double[] moisture_range { get; set; }
        public double frequency { get; set; }
    }
}
```

### File Structure for Full Implementation

```
C:\DevEnv\GIT\UO\ServUO\Scripts\Commands\Vystia\
├── VystiaProceduralGenerator.cs (main command)
├── Generation\
│   ├── PerlinNoise.cs
│   ├── HeightMapGenerator.cs
│   ├── MoistureMapGenerator.cs
│   └── BiomeClassifier.cs
├── Writers\
│   └── TerrainWriter.cs
└── Validation\
    └── TerrainValidator.cs
```

### Performance Optimization Tips

1. **Batch tile writing** - Write entire blocks at once
2. **Progress reporting** - Update player every 1000 blocks
3. **Memory management** - Process in chunks if needed
4. **Cache biome lookups** - Store biome assignments
5. **Parallel processing** - Use multiple threads for height/moisture maps

### Expected Generation Time

| Hardware | Time Estimate |
|----------|---------------|
| High-end (16GB+ RAM, SSD) | 15-25 minutes |
| Mid-range (8GB RAM, HDD) | 30-45 minutes |
| Low-end | 60+ minutes |

### Troubleshooting Full Generation

**Problem:** Out of memory errors

**Solution:**
- Reduce map size temporarily
- Process in smaller chunks
- Increase server RAM allocation

**Problem:** Generation hangs/freezes

**Solution:**
- Check progress messages
- Verify no infinite loops in noise generation
- Ensure config file is valid JSON

**Problem:** Biomes don't match expectations

**Solution:**
- Adjust elevation_range and moisture_range in config
- Tweak Perlin noise parameters (octaves, scale)
- Verify tile IDs are correct for each biome

---

## Configuration Reference

### Key Config Parameters

From `Vystia_World_Config.json`:

```json
{
  "terrain_generation": {
    "parameters": {
      "octaves": 7,           // More octaves = more detail (slower)
      "persistence": 0.55,    // How much each octave contributes
      "lacunarity": 2.1,      // Frequency multiplier per octave
      "scale": 140.0          // Overall terrain scale (larger = smoother)
    }
  }
}
```

**Effect of changing parameters:**

- **Octaves (1-10):** More = more detailed terrain, but slower generation
- **Persistence (0.0-1.0):** Higher = rougher, more varied terrain
- **Lacunarity (1.5-3.0):** Higher = more "spiky" features
- **Scale (50-300):** Lower = more chaotic, higher = smoother/gentler

### Tile ID Reference

Your config uses these tile IDs for biomes:

| Biome | Tile ID | Hex | Description |
|-------|---------|-----|-------------|
| Tundra | 1152 | 0x0480 | Frozen wasteland |
| Snow Mountain | 1153 | 0x0481 | Snow peaks |
| Forest | 11 | 0x000B | Ancient woodlands |
| Grassland | 3 | 0x0003 | Plains |
| Desert | 41 | 0x0029 | Sand dunes |
| Volcanic | 430 | 0x01AE | Lava and ash |
| Swamp | 351 | 0x015F | Murky wetlands |
| Mountain | 220 | 0x00DC | Rock mountains |
| Crystal Plains | 1775 | 0x06EF | Glowing formations |
| Twilight Forest | 3280 | 0x0CD0 | Perpetual dusk |
| Jungle | 3277 | 0x0CCD | Tropical |

---

## Next Steps

### For Quick Test (Approach B):

1. ✅ Create `VystiaTerrainTest.cs`
2. ✅ Fix map inconsistencies in existing files
3. ✅ Compile and test
4. ✅ Verify terrain appears correctly
5. ✅ Document results

### For Full Generation (Approach A):

1. ✅ Create all procedural generation classes
2. ✅ Add JSON.NET reference (if not already present)
3. ✅ Test on small area first (1000x1000)
4. ✅ Profile performance
5. ✅ Run full 7168x4096 generation
6. ✅ Add city placement integration
7. ✅ Add dungeon placement
8. ✅ Add spawner placement

---

## Persistence Strategy

### Current Limitation

The `SetLandBlock` method modifies **runtime terrain data** only. Changes are lost on server restart.

### Solutions:

#### Option 1: Regenerate on Startup
- Add terrain generation to server startup sequence
- Fast if using same seed
- No additional storage needed

#### Option 2: Export to .mul Files
- Use external tools (UOFiddler, CentrED+) to export
- Permanent changes
- Requires client file replacement

#### Option 3: Hybrid Approach
- Generate once, save to custom format
- Load from saved data on startup
- Faster than full regeneration

---

## Resources

### External Tools for Map Editing

- **CentrED+** - Full map editor with UO support
- **UOFiddler** - View/edit .mul files
- **Pandora's Box** - Legacy map editor
- **UO Landscaper** - Terrain sculpting

### Documentation

- ServUO Forums: https://www.servuo.com
- UO Map File Format: https://uo.stratics.com/heptazane/fileformats.shtml
- Perlin Noise Tutorial: https://adrianb.io/2014/08/09/perlinnoise.html

---

## Summary

### Approach B (Quick Test)
- ✅ Fast to implement (30 minutes)
- ✅ Proves terrain generation works
- ✅ Simple test areas
- ❌ Not full Vystia world
- ❌ Manual tile placement

### Approach A (Full Procedural)
- ✅ Complete Vystia world
- ✅ Matches your concept art
- ✅ Automated, reproducible
- ✅ Configurable via JSON
- ❌ Complex implementation (2-3 hours)
- ❌ Long generation time (20-40 min)

**Recommended Path:**
1. Start with Approach B to verify the system works
2. Once confirmed, implement Approach A for the full world
3. Consider persistence strategy for production use

---

*Last Updated: 2025-10-21*
*Author: Claude Code*
*Version: 1.0*
