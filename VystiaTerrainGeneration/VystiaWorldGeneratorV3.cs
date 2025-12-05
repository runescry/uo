using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;

namespace VystiaGenerator
{
    /// <summary>
    /// Vystia World Generator V3 - Organic coastlines matching hand-drawn map
    /// Uses noise-based polygon generation for realistic continental shapes
    /// </summary>
    public class VystiaWorldGeneratorV3
    {
        private const int WORLD_WIDTH = 7000;
        private const int WORLD_HEIGHT = 4096;
        
        private readonly Random rand;
        private byte[,] heightMap;
        private BiomeType[,] biomeMap;
        private float[,] temperatureMap;
        private float[,] moistureMap;
        private int[] permutation;

        public VystiaWorldGeneratorV3(int? seed = null)
        {
            rand = seed.HasValue ? new Random(seed.Value) : new Random();
            heightMap = new byte[WORLD_HEIGHT, WORLD_WIDTH];
            biomeMap = new BiomeType[WORLD_HEIGHT, WORLD_WIDTH];
            temperatureMap = new float[WORLD_HEIGHT, WORLD_WIDTH];
            moistureMap = new float[WORLD_HEIGHT, WORLD_WIDTH];
            InitializePermutation();
        }

        private void InitializePermutation()
        {
            permutation = new int[512];
            int[] p = new int[256];
            for (int i = 0; i < 256; i++) p[i] = i;
            
            for (int i = 0; i < 256; i++)
            {
                int j = rand.Next(256);
                int temp = p[i];
                p[i] = p[j];
                p[j] = temp;
            }
            
            for (int i = 0; i < 512; i++)
                permutation[i] = p[i % 256];
        }

        public void GenerateWorld(string outputPath)
        {
            Console.WriteLine("=== VYSTIA WORLD GENERATOR V3 ===");
            Console.WriteLine("Organic coastline generation matching hand-drawn map");
            Console.WriteLine($"Generating {WORLD_WIDTH}x{WORLD_HEIGHT} world...\n");

            // Initialize as ocean
            Console.WriteLine("1. Initializing ocean base...");
            for (int y = 0; y < WORLD_HEIGHT; y++)
                for (int x = 0; x < WORLD_WIDTH; x++)
                    heightMap[y, x] = 15; // Deep ocean

            // Generate each continent with organic shapes
            Console.WriteLine("2. Generating continental landmasses...");
            GenerateFrosthold();
            GenerateVerdantpeak();
            GenerateCrystalBarrens();
            GenerateIroncladEmpire();
            GenerateRadiantPlains();
            GenerateEmberlands();
            GenerateShadowfen();
            GenerateWhisperingSands();
            GenerateBlazingFrontier();
            GenerateSunkenIsles();

            // Add the Great Bay
            Console.WriteLine("3. Carving the Great Bay...");
            CarveGreatBay();

            // Connect with land bridges
            Console.WriteLine("4. Creating land bridges and connections...");
            CreateLandBridges();

            // Add coastal features
            Console.WriteLine("5. Adding coastal detail (bays, peninsulas, fjords)...");
            AddCoastalFeatures();

            // Generate climate
            Console.WriteLine("6. Generating temperature map...");
            GenerateTemperatureMap();
            
            Console.WriteLine("7. Generating moisture map...");
            GenerateMoistureMap();

            // Assign biomes
            Console.WriteLine("8. Assigning biomes based on climate...");
            AssignBiomes();

            // Place cities
            Console.WriteLine("9. Placing cities...");
            PlaceCities();

            // Save outputs
            Console.WriteLine("10. Saving world files...");
            Directory.CreateDirectory(outputPath);
            SaveHeightMap(Path.Combine(outputPath, "heightmap.bmp"));
            SaveBiomeMap(Path.Combine(outputPath, "biomemap.bmp"));
            SaveTemperatureMap(Path.Combine(outputPath, "temperature.bmp"));
            SaveMoistureMap(Path.Combine(outputPath, "moisture.bmp"));

            Console.WriteLine($"\n=== GENERATION COMPLETE ===");
            Console.WriteLine($"Output saved to: {outputPath}");
            Console.WriteLine("\nGenerated files:");
            Console.WriteLine("  - heightmap.bmp (altitude data)");
            Console.WriteLine("  - biomemap.bmp (terrain types)");
            Console.WriteLine("  - temperature.bmp (climate data)");
            Console.WriteLine("  - moisture.bmp (rainfall data)");
        }

        // ORGANIC LANDMASS GENERATION
        private void GenerateFrosthold()
        {
            Console.WriteLine("  → Frosthold (frozen north)");
            var polygon = new List<Point>
            {
                new Point(500, 300),
                new Point(900, 200),
                new Point(1400, 250),
                new Point(1700, 400),
                new Point(1650, 700),
                new Point(1500, 1000),
                new Point(1200, 1100),
                new Point(800, 1050),
                new Point(550, 800),
                new Point(450, 500)
            };
            
            CreateOrganicLandmass(polygon, 140, 220, 0.15f, true);
            
            // Add fjords to southern coast
            for (int i = 0; i < 8; i++)
            {
                int fjordX = 700 + i * 100;
                CarveFjord(fjordX, 1000, 150, 40);
            }
        }

        private void GenerateVerdantpeak()
        {
            Console.WriteLine("  → Verdantpeak (western forests)");
            var polygon = new List<Point>
            {
                new Point(800, 1300),
                new Point(1200, 1200),
                new Point(1600, 1400),
                new Point(1500, 1800),
                new Point(1100, 2000),
                new Point(850, 1900),
                new Point(750, 1600)
            };
            
            CreateOrganicLandmass(polygon, 120, 200, 0.18f, true);
        }

        private void GenerateCrystalBarrens()
        {
            Console.WriteLine("  → Crystal Barrens (crystal highlands)");
            var polygon = new List<Point>
            {
                new Point(1400, 1500),
                new Point(1900, 1400),
                new Point(2200, 1600),
                new Point(2100, 1900),
                new Point(1600, 1900),
                new Point(1350, 1750)
            };
            
            CreateOrganicLandmass(polygon, 100, 160, 0.20f, false);
        }

        private void GenerateIroncladEmpire()
        {
            Console.WriteLine("  → Ironclad Empire (central supercontinent)");
            var polygon = new List<Point>
            {
                new Point(2600, 1300),
                new Point(3200, 1200),
                new Point(3800, 1100),
                new Point(4400, 1200),
                new Point(4300, 1800),
                new Point(4100, 2300),
                new Point(3600, 2600),
                new Point(2900, 2550),
                new Point(2600, 2200),
                new Point(2500, 1700)
            };
            
            CreateOrganicLandmass(polygon, 80, 140, 0.25f, false);
            
            // Add multiple peninsulas
            CreatePeninsula(3000, 1400, 200, 300, 45);
            CreatePeninsula(3500, 2500, 180, 250, -30);
            CreatePeninsula(4200, 1600, 150, 200, 90);
        }

        private void GenerateRadiantPlains()
        {
            Console.WriteLine("  → Radiant Plains (eastern grasslands)");
            var polygon = new List<Point>
            {
                new Point(3900, 1300),
                new Point(4600, 1200),
                new Point(4800, 1500),
                new Point(4700, 1900),
                new Point(4200, 2000),
                new Point(3900, 1700)
            };
            
            CreateOrganicLandmass(polygon, 90, 130, 0.15f, false);
        }

        private void GenerateEmberlands()
        {
            Console.WriteLine("  → Emberlands (volcanic east)");
            var polygon = new List<Point>
            {
                new Point(5200, 1500),
                new Point(5800, 1400),
                new Point(6200, 1600),
                new Point(6100, 2000),
                new Point(5700, 2200),
                new Point(5300, 2100),
                new Point(5150, 1800)
            };
            
            CreateOrganicLandmass(polygon, 100, 170, 0.22f, false);
            
            // Add volcanic peaks
            AddVolcanicPeaks(5600, 1700, 3);
        }

        private void GenerateShadowfen()
        {
            Console.WriteLine("  → Shadowfen (southeastern swamps)");
            var polygon = new List<Point>
            {
                new Point(5500, 2100),
                new Point(6100, 2000),
                new Point(6400, 2300),
                new Point(6300, 2600),
                new Point(5800, 2600),
                new Point(5450, 2400)
            };
            
            CreateOrganicLandmass(polygon, 60, 90, 0.18f, false);
        }

        private void GenerateWhisperingSands()
        {
            Console.WriteLine("  → Whispering Sands (southern desert)");
            var polygon = new List<Point>
            {
                new Point(2400, 2700),
                new Point(3200, 2600),
                new Point(3800, 2800),
                new Point(3700, 3300),
                new Point(3200, 3400),
                new Point(2500, 3350),
                new Point(2350, 3000)
            };
            
            CreateOrganicLandmass(polygon, 70, 120, 0.20f, false);
        }

        private void GenerateBlazingFrontier()
        {
            Console.WriteLine("  → Blazing Frontier (southeast desert)");
            var polygon = new List<Point>
            {
                new Point(4000, 2900),
                new Point(4800, 2800),
                new Point(5200, 3100),
                new Point(5000, 3500),
                new Point(4400, 3600),
                new Point(3950, 3300)
            };
            
            CreateOrganicLandmass(polygon, 80, 130, 0.18f, false);
        }

        private void GenerateSunkenIsles()
        {
            Console.WriteLine("  → The Sunken Isles (southwestern archipelago)");
            
            // Generate 40-50 islands of varying sizes
            int islandCount = 45;
            for (int i = 0; i < islandCount; i++)
            {
                int centerX = 500 + rand.Next(1100);
                int centerY = 2800 + rand.Next(1000);
                int size = 30 + rand.Next(150);
                
                CreateIsland(centerX, centerY, size);
            }
        }

        private void CreateOrganicLandmass(List<Point> polygon, byte minHeight, byte maxHeight, float noiseScale, bool mountains)
        {
            // Find bounding box
            int minX = polygon.Min(p => p.X) - 200;
            int maxX = polygon.Max(p => p.X) + 200;
            int minY = polygon.Min(p => p.Y) - 200;
            int maxY = polygon.Max(p => p.Y) + 200;
            
            minX = Math.Max(0, minX);
            maxX = Math.Min(WORLD_WIDTH - 1, maxX);
            minY = Math.Max(0, minY);
            maxY = Math.Min(WORLD_HEIGHT - 1, maxY);
            
            for (int y = minY; y <= maxY; y++)
            {
                for (int x = minX; x <= maxX; x++)
                {
                    // Check if point is inside polygon with noise perturbation
                    float noise = PerlinNoise(x * noiseScale, y * noiseScale) * 80;
                    if (IsPointInPolygon(x + (int)noise, y + (int)noise, polygon))
                    {
                        // Calculate distance to nearest edge
                        float distToEdge = DistanceToPolygonEdge(x, y, polygon);
                        float normalizedDist = Math.Min(1.0f, distToEdge / 200.0f);
                        
                        // Height increases toward center
                        byte height = (byte)(minHeight + (maxHeight - minHeight) * normalizedDist);
                        
                        // Add terrain variation
                        float variation = PerlinNoise(x * 0.02f, y * 0.02f) * 30;
                        height = (byte)Math.Max(minHeight, Math.Min(maxHeight, height + variation));
                        
                        // Mountains get extra height in center
                        if (mountains && normalizedDist > 0.5f)
                        {
                            height = (byte)Math.Min(255, height + 60);
                        }
                        
                        heightMap[y, x] = Math.Max(heightMap[y, x], height);
                    }
                }
            }
        }

        private void CreateIsland(int centerX, int centerY, int size)
        {
            for (int dy = -size; dy <= size; dy++)
            {
                for (int dx = -size; dx <= size; dx++)
                {
                    int x = centerX + dx;
                    int y = centerY + dy;
                    
                    if (x < 0 || x >= WORLD_WIDTH || y < 0 || y >= WORLD_HEIGHT)
                        continue;
                    
                    float dist = (float)Math.Sqrt(dx * dx + dy * dy);
                    float noise = PerlinNoise(x * 0.05f, y * 0.05f) * size * 0.3f;
                    
                    if (dist + noise < size)
                    {
                        float falloff = 1.0f - (dist / size);
                        byte height = (byte)(60 + falloff * 60);
                        heightMap[y, x] = Math.Max(heightMap[y, x], height);
                    }
                }
            }
        }

        private void CarveGreatBay()
        {
            // The Great Bay - major inland sea cutting into Ironclad from the north
            for (int y = 800; y <= 1400; y++)
            {
                for (int x = 2800; x <= 4000; x++)
                {
                    // Bay shape with organic edges
                    float centerX = 3400;
                    float centerY = 1100;
                    float dx = (x - centerX) / 600.0f;
                    float dy = (y - centerY) / 300.0f;
                    float dist = (float)Math.Sqrt(dx * dx + dy * dy);
                    
                    float noise = PerlinNoise(x * 0.01f, y * 0.01f) * 0.3f;
                    
                    if (dist + noise < 1.0f)
                    {
                        heightMap[y, x] = 10; // Shallow bay water
                    }
                }
            }
        }

        private void CreateLandBridges()
        {
            // Frosthold to Verdantpeak (island chain)
            CreateIslandChain(1000, 1100, 1400, 1300, 8, 60);
            
            // Verdantpeak to Ironclad (narrow land bridge)
            CreateNarrowBridge(2000, 1600, 2600, 1500, 100);
            
            // Whispering Sands to Blazing Frontier (connection)
            CreateNarrowBridge(3700, 3000, 4000, 3000, 150);
        }

        private void CreateIslandChain(int x1, int y1, int x2, int y2, int islandCount, int islandSize)
        {
            for (int i = 0; i <= islandCount; i++)
            {
                float t = i / (float)islandCount;
                int x = (int)(x1 + (x2 - x1) * t);
                int y = (int)(y1 + (y2 - y1) * t);
                
                // Add some randomness
                x += rand.Next(-50, 50);
                y += rand.Next(-50, 50);
                
                CreateIsland(x, y, islandSize);
            }
        }

        private void CreateNarrowBridge(int x1, int y1, int x2, int y2, int width)
        {
            int steps = Math.Max(Math.Abs(x2 - x1), Math.Abs(y2 - y1));
            
            for (int i = 0; i <= steps; i++)
            {
                float t = i / (float)steps;
                int x = (int)(x1 + (x2 - x1) * t);
                int y = (int)(y1 + (y2 - y1) * t);
                
                for (int dy = -width/2; dy <= width/2; dy++)
                {
                    for (int dx = -width/2; dx <= width/2; dx++)
                    {
                        int nx = x + dx;
                        int ny = y + dy;
                        
                        if (nx >= 0 && nx < WORLD_WIDTH && ny >= 0 && ny < WORLD_HEIGHT)
                        {
                            float dist = (float)Math.Sqrt(dx * dx + dy * dy);
                            if (dist < width / 2)
                            {
                                byte height = (byte)(70 + 30 * (1 - dist / (width / 2)));
                                heightMap[ny, nx] = Math.Max(heightMap[ny, nx], height);
                            }
                        }
                    }
                }
            }
        }

        private void AddCoastalFeatures()
        {
            // Add bays, peninsulas, and coastal roughness
            // This pass adds detail to existing coastlines
            
            for (int y = 1; y < WORLD_HEIGHT - 1; y++)
            {
                for (int x = 1; x < WORLD_WIDTH - 1; x++)
                {
                    // Skip if not near coast
                    if (!IsNearCoast(x, y, 20))
                        continue;
                    
                    // Add coastal noise
                    float coastalNoise = PerlinNoise(x * 0.03f, y * 0.03f);
                    if (coastalNoise > 0.4f && heightMap[y, x] > 30)
                    {
                        heightMap[y, x] = (byte)Math.Min(255, heightMap[y, x] + 15);
                    }
                    else if (coastalNoise < -0.4f && heightMap[y, x] > 30)
                    {
                        heightMap[y, x] = (byte)Math.Max(30, heightMap[y, x] - 20);
                    }
                }
                
                if (y % 200 == 0)
                    Console.WriteLine($"  Processing coastal detail: {y}/{WORLD_HEIGHT}");
            }
        }

        private void CarveFjord(int x, int y, int depth, int width)
        {
            for (int d = 0; d < depth; d++)
            {
                for (int w = -width/2; w <= width/2; w++)
                {
                    int nx = x + w;
                    int ny = y - d;
                    
                    if (nx >= 0 && nx < WORLD_WIDTH && ny >= 0 && ny < WORLD_HEIGHT)
                    {
                        float falloff = 1.0f - (Math.Abs(w) / (float)(width / 2));
                        byte targetHeight = (byte)(20 + 10 * falloff);
                        heightMap[ny, nx] = Math.Min(heightMap[ny, nx], targetHeight);
                    }
                }
            }
        }

        private void CreatePeninsula(int x, int y, int length, int width, float angle)
        {
            float radians = angle * (float)Math.PI / 180.0f;
            float dx = (float)Math.Cos(radians);
            float dy = (float)Math.Sin(radians);
            
            for (int l = 0; l < length; l++)
            {
                int centerX = x + (int)(dx * l);
                int centerY = y + (int)(dy * l);
                
                int currentWidth = (int)(width * (1.0f - l / (float)length));
                
                for (int w = -currentWidth/2; w <= currentWidth/2; w++)
                {
                    int nx = centerX + (int)(-dy * w);
                    int ny = centerY + (int)(dx * w);
                    
                    if (nx >= 0 && nx < WORLD_WIDTH && ny >= 0 && ny < WORLD_HEIGHT)
                    {
                        float falloff = 1.0f - (l / (float)length);
                        byte height = (byte)(80 + falloff * 40);
                        heightMap[ny, nx] = Math.Max(heightMap[ny, nx], height);
                    }
                }
            }
        }

        private void AddVolcanicPeaks(int centerX, int centerY, int count)
        {
            for (int i = 0; i < count; i++)
            {
                int x = centerX + rand.Next(-200, 200);
                int y = centerY + rand.Next(-200, 200);
                
                for (int dy = -80; dy <= 80; dy++)
                {
                    for (int dx = -80; dx <= 80; dx++)
                    {
                        int nx = x + dx;
                        int ny = y + dy;
                        
                        if (nx >= 0 && nx < WORLD_WIDTH && ny >= 0 && ny < WORLD_HEIGHT)
                        {
                            float dist = (float)Math.Sqrt(dx * dx + dy * dy);
                            if (dist < 80)
                            {
                                float height = (1.0f - dist / 80.0f) * 100;
                                heightMap[ny, nx] = (byte)Math.Min(255, heightMap[ny, nx] + height);
                            }
                        }
                    }
                }
            }
        }

        private bool IsNearCoast(int x, int y, int range)
        {
            bool hasWater = false;
            bool hasLand = false;
            
            for (int dy = -range; dy <= range; dy += 5)
            {
                for (int dx = -range; dx <= range; dx += 5)
                {
                    int nx = x + dx;
                    int ny = y + dy;
                    
                    if (nx >= 0 && nx < WORLD_WIDTH && ny >= 0 && ny < WORLD_HEIGHT)
                    {
                        if (heightMap[ny, nx] < 30)
                            hasWater = true;
                        else
                            hasLand = true;
                    }
                }
            }
            
            return hasWater && hasLand;
        }

        // UTILITY FUNCTIONS
        private bool IsPointInPolygon(int x, int y, List<Point> polygon)
        {
            bool inside = false;
            int j = polygon.Count - 1;
            
            for (int i = 0; i < polygon.Count; j = i++)
            {
                if (((polygon[i].Y > y) != (polygon[j].Y > y)) &&
                    (x < (polygon[j].X - polygon[i].X) * (y - polygon[i].Y) / (polygon[j].Y - polygon[i].Y) + polygon[i].X))
                {
                    inside = !inside;
                }
            }
            
            return inside;
        }

        private float DistanceToPolygonEdge(int x, int y, List<Point> polygon)
        {
            float minDist = float.MaxValue;
            
            for (int i = 0; i < polygon.Count; i++)
            {
                int j = (i + 1) % polygon.Count;
                float dist = DistanceToLineSegment(x, y, polygon[i].X, polygon[i].Y, polygon[j].X, polygon[j].Y);
                minDist = Math.Min(minDist, dist);
            }
            
            return minDist;
        }

        private float DistanceToLineSegment(int px, int py, int x1, int y1, int x2, int y2)
        {
            float dx = x2 - x1;
            float dy = y2 - y1;
            float lengthSquared = dx * dx + dy * dy;
            
            if (lengthSquared == 0)
                return (float)Math.Sqrt((px - x1) * (px - x1) + (py - y1) * (py - y1));
            
            float t = Math.Max(0, Math.Min(1, ((px - x1) * dx + (py - y1) * dy) / lengthSquared));
            float projX = x1 + t * dx;
            float projY = y1 + t * dy;
            
            return (float)Math.Sqrt((px - projX) * (px - projX) + (py - projY) * (py - projY));
        }

        private float PerlinNoise(float x, float y)
        {
            int xi = (int)Math.Floor(x) & 255;
            int yi = (int)Math.Floor(y) & 255;
            
            float xf = x - (float)Math.Floor(x);
            float yf = y - (float)Math.Floor(y);
            
            float u = Fade(xf);
            float v = Fade(yf);
            
            int aa = permutation[permutation[xi] + yi];
            int ab = permutation[permutation[xi] + yi + 1];
            int ba = permutation[permutation[xi + 1] + yi];
            int bb = permutation[permutation[xi + 1] + yi + 1];
            
            float x1 = Lerp(Grad(aa, xf, yf), Grad(ba, xf - 1, yf), u);
            float x2 = Lerp(Grad(ab, xf, yf - 1), Grad(bb, xf - 1, yf - 1), u);
            
            return Lerp(x1, x2, v);
        }

        private float Fade(float t) => t * t * t * (t * (t * 6 - 15) + 10);
        private float Lerp(float a, float b, float t) => a + t * (b - a);
        
        private float Grad(int hash, float x, float y)
        {
            int h = hash & 15;
            float u = h < 8 ? x : y;
            float v = h < 4 ? y : x;
            return ((h & 1) == 0 ? u : -u) + ((h & 2) == 0 ? v : -v);
        }

        // CLIMATE GENERATION
        private void GenerateTemperatureMap()
        {
            for (int y = 0; y < WORLD_HEIGHT; y++)
            {
                for (int x = 0; x < WORLD_WIDTH; x++)
                {
                    float latitude = y / (float)WORLD_HEIGHT;
                    float baseTemp = 1.0f - latitude;
                    
                    float altitude = heightMap[y, x] / 255.0f;
                    float altitudeMod = Math.Max(0, 1.0f - altitude * 0.7f);
                    
                    float noise = PerlinNoise(x * 0.004f, y * 0.004f) * 0.15f;
                    
                    temperatureMap[y, x] = Math.Max(0, Math.Min(1, baseTemp * altitudeMod + noise));
                }
                
                if (y % 400 == 0)
                    Console.WriteLine($"  Temperature: {y}/{WORLD_HEIGHT}");
            }
        }

        private void GenerateMoistureMap()
        {
            for (int y = 0; y < WORLD_HEIGHT; y++)
            {
                for (int x = 0; x < WORLD_WIDTH; x++)
                {
                    float distToWater = DistanceToWater(x, y, 80);
                    float moistureBase = 1.0f - (distToWater / 80.0f);
                    
                    float noise = PerlinNoise(x * 0.003f, y * 0.003f);
                    
                    moistureMap[y, x] = Math.Max(0, Math.Min(1, moistureBase + noise * 0.25f));
                }
                
                if (y % 400 == 0)
                    Console.WriteLine($"  Moisture: {y}/{WORLD_HEIGHT}");
            }
        }

        private float DistanceToWater(int x, int y, int searchRadius)
        {
            float minDist = searchRadius;
            
            for (int dy = -searchRadius; dy <= searchRadius; dy += 8)
            {
                for (int dx = -searchRadius; dx <= searchRadius; dx += 8)
                {
                    int nx = x + dx;
                    int ny = y + dy;
                    
                    if (nx >= 0 && nx < WORLD_WIDTH && ny >= 0 && ny < WORLD_HEIGHT)
                    {
                        if (heightMap[ny, nx] < 30)
                        {
                            float dist = (float)Math.Sqrt(dx * dx + dy * dy);
                            minDist = Math.Min(minDist, dist);
                        }
                    }
                }
            }
            
            return minDist;
        }

        private void AssignBiomes()
        {
            for (int y = 0; y < WORLD_HEIGHT; y++)
            {
                for (int x = 0; x < WORLD_WIDTH; x++)
                {
                    byte height = heightMap[y, x];
                    float temp = temperatureMap[y, x];
                    float moisture = moistureMap[y, x];
                    
                    if (height < 30)
                    {
                        biomeMap[y, x] = BiomeType.Ocean;
                    }
                    else if (temp < 0.25f)
                    {
                        biomeMap[y, x] = BiomeType.Frozen;
                    }
                    else if (temp > 0.8f && moisture < 0.35f)
                    {
                        biomeMap[y, x] = BiomeType.Volcanic;
                    }
                    else if (temp > 0.65f && moisture < 0.45f)
                    {
                        biomeMap[y, x] = BiomeType.Desert;
                    }
                    else if (height > 180)
                    {
                        biomeMap[y, x] = BiomeType.Mountain;
                    }
                    else if (moisture > 0.7f && temp > 0.5f && temp < 0.75f)
                    {
                        biomeMap[y, x] = BiomeType.Swamp;
                    }
                    else if (moisture > 0.55f && temp > 0.35f && temp < 0.75f)
                    {
                        biomeMap[y, x] = BiomeType.Forest;
                    }
                    else if (rand.NextDouble() < 0.015f && height > 110 && temp > 0.4f && temp < 0.7f)
                    {
                        biomeMap[y, x] = BiomeType.Crystal;
                    }
                    else
                    {
                        biomeMap[y, x] = BiomeType.Plains;
                    }
                }
                
                if (y % 400 == 0)
                    Console.WriteLine($"  Biomes: {y}/{WORLD_HEIGHT}");
            }
        }

        private void PlaceCities()
        {
            var cities = new[]
            {
                ("Frostspire", 900, 600, BiomeType.Frozen, 35),
                ("Verdantpeak", 1200, 1600, BiomeType.Forest, 30),
                ("Crystal Barrens", 1700, 1700, BiomeType.Crystal, 25),
                ("Ironclad Capital", 3400, 1800, BiomeType.Industrial, 40),
                ("Radiant Plains", 4400, 1600, BiomeType.Plains, 30),
                ("Emberforge", 5700, 1800, BiomeType.Volcanic, 30),
                ("Shadowfen", 5900, 2400, BiomeType.Swamp, 25),
                ("Oasis", 3000, 3000, BiomeType.Desert, 30),
                ("Blazing Frontier", 4600, 3200, BiomeType.Desert, 28)
            };

            foreach (var (name, x, y, biome, radius) in cities)
            {
                for (int dy = -radius; dy <= radius; dy++)
                {
                    for (int dx = -radius; dx <= radius; dx++)
                    {
                        int nx = x + dx;
                        int ny = y + dy;
                        
                        if (nx >= 0 && nx < WORLD_WIDTH && ny >= 0 && ny < WORLD_HEIGHT)
                        {
                            float dist = (float)Math.Sqrt(dx * dx + dy * dy);
                            if (dist < radius)
                            {
                                biomeMap[ny, nx] = BiomeType.Industrial;
                                
                                if (dist < radius * 0.8f)
                                    heightMap[ny, nx] = 100;
                            }
                        }
                    }
                }
                
                Console.WriteLine($"  → Placed: {name}");
            }
        }

        // SAVE FUNCTIONS
        private void SaveHeightMap(string path)
        {
            using (Bitmap bmp = new Bitmap(WORLD_WIDTH, WORLD_HEIGHT, PixelFormat.Format8bppIndexed))
            {
                ColorPalette palette = bmp.Palette;
                for (int i = 0; i < 256; i++)
                    palette.Entries[i] = Color.FromArgb(i, i, i);
                bmp.Palette = palette;
                
                BitmapData data = bmp.LockBits(new Rectangle(0, 0, WORLD_WIDTH, WORLD_HEIGHT),
                    ImageLockMode.WriteOnly, PixelFormat.Format8bppIndexed);
                
                unsafe
                {
                    for (int y = 0; y < WORLD_HEIGHT; y++)
                    {
                        byte* row = (byte*)data.Scan0 + (y * data.Stride);
                        for (int x = 0; x < WORLD_WIDTH; x++)
                        {
                            row[x] = heightMap[y, x];
                        }
                    }
                }
                
                bmp.UnlockBits(data);
                bmp.Save(path);
            }
        }

        private void SaveBiomeMap(string path)
        {
            using (Bitmap bmp = new Bitmap(WORLD_WIDTH, WORLD_HEIGHT))
            {
                for (int y = 0; y < WORLD_HEIGHT; y++)
                {
                    for (int x = 0; x < WORLD_WIDTH; x++)
                    {
                        Color color = GetBiomeColor(biomeMap[y, x]);
                        bmp.SetPixel(x, y, color);
                    }
                    
                    if (y % 400 == 0)
                        Console.WriteLine($"  Saving biome: {y}/{WORLD_HEIGHT}");
                }
                
                bmp.Save(path);
            }
        }

        private void SaveTemperatureMap(string path)
        {
            using (Bitmap bmp = new Bitmap(WORLD_WIDTH, WORLD_HEIGHT))
            {
                for (int y = 0; y < WORLD_HEIGHT; y++)
                {
                    for (int x = 0; x < WORLD_WIDTH; x++)
                    {
                        int intensity = (int)(temperatureMap[y, x] * 255);
                        bmp.SetPixel(x, y, Color.FromArgb(intensity, 0, 255 - intensity));
                    }
                }
                
                bmp.Save(path);
            }
        }

        private void SaveMoistureMap(string path)
        {
            using (Bitmap bmp = new Bitmap(WORLD_WIDTH, WORLD_HEIGHT))
            {
                for (int y = 0; y < WORLD_HEIGHT; y++)
                {
                    for (int x = 0; x < WORLD_WIDTH; x++)
                    {
                        int intensity = (int)(moistureMap[y, x] * 255);
                        bmp.SetPixel(x, y, Color.FromArgb(0, intensity, 255 - intensity));
                    }
                }
                
                bmp.Save(path);
            }
        }

        private Color GetBiomeColor(BiomeType biome)
        {
            return biome switch
            {
                BiomeType.Ocean => Color.FromArgb(0, 0, 139),
                BiomeType.Plains => Color.FromArgb(154, 205, 50),
                BiomeType.Forest => Color.FromArgb(0, 100, 0),
                BiomeType.Desert => Color.FromArgb(244, 164, 96),
                BiomeType.Frozen => Color.FromArgb(255, 255, 255),
                BiomeType.Mountain => Color.FromArgb(128, 128, 128),
                BiomeType.Swamp => Color.FromArgb(85, 107, 47),
                BiomeType.Volcanic => Color.FromArgb(139, 0, 0),
                BiomeType.Crystal => Color.FromArgb(0, 255, 255),
                BiomeType.Shadow => Color.FromArgb(128, 0, 128),
                BiomeType.Industrial => Color.FromArgb(169, 169, 169),
                _ => Color.Black
            };
        }
    }

    public enum BiomeType
    {
        Ocean, Plains, Forest, Desert, Frozen, Mountain,
        Swamp, Volcanic, Crystal, Shadow, Industrial
    }

    public class Program
    {
        public static void Main(string[] args)
        {
            var generator = new VystiaWorldGeneratorV3();
            generator.GenerateWorld("VystiaWorld");
            
            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }
    }
}