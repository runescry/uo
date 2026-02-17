using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Server;

namespace Server.Custom.VystiaClasses.Quests.Generation
{
    public sealed class VystiaPoi
    {
        [JsonProperty("poiId")]
        public string PoiId { get; set; }

        [JsonProperty("poiName")]
        public string PoiName { get; set; }

        [JsonProperty("map")]
        public string MapName { get; set; }

        [JsonProperty("x")]
        public int X { get; set; }

        [JsonProperty("y")]
        public int Y { get; set; }

        [JsonProperty("z")]
        public int? Z { get; set; }

        [JsonProperty("radius")]
        public int Radius { get; set; } = 20;
    }

    /// <summary>
    /// POI grounding registry: maps Vystia POI ids/names to real UO map coordinates.
    /// This is the hard "anti-hallucination" layer.
    /// </summary>
    public static class VystiaPoiRegistry
    {
        private static readonly object _sync = new object();
        private static bool _loaded;
        private static Dictionary<string, VystiaPoi> _byId = new Dictionary<string, VystiaPoi>(StringComparer.OrdinalIgnoreCase);

        public static string RegistryPath =>
            Path.Combine(Core.BaseDirectory, "Data", "Vystia", "poi_registry.json");

        public static void EnsureLoaded()
        {
            if (_loaded)
                return;

            lock (_sync)
            {
                if (_loaded)
                    return;

                _byId = new Dictionary<string, VystiaPoi>(StringComparer.OrdinalIgnoreCase);

                try
                {
                    if (File.Exists(RegistryPath))
                    {
                        string json = File.ReadAllText(RegistryPath);
                        var pois = JsonConvert.DeserializeObject<List<VystiaPoi>>(json) ?? new List<VystiaPoi>();

                        foreach (var poi in pois)
                        {
                            if (poi == null || string.IsNullOrWhiteSpace(poi.PoiId))
                                continue;

                            _byId[poi.PoiId.Trim()] = poi;
                        }
                    }
                    else
                    {
                        Console.WriteLine($"[VystiaPoiRegistry] Missing POI registry at {RegistryPath} (generation will be limited).");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[VystiaPoiRegistry] Failed to load POIs: {ex.Message}");
                }

                _loaded = true;
            }
        }

        public static bool TryGet(string poiId, out VystiaPoi poi)
        {
            EnsureLoaded();

            poi = null;
            if (string.IsNullOrWhiteSpace(poiId))
                return false;

            return _byId.TryGetValue(poiId.Trim(), out poi) && poi != null;
        }

        public static bool TryResolveLocation(string poiId, out Point3D loc, out Map map, out int radius)
        {
            loc = Point3D.Zero;
            map = null;
            radius = 0;

            if (!TryGet(poiId, out var poi))
                return false;

            map = ResolveMap(poi.MapName);
            if (map == null)
                return false;

            int z = poi.Z ?? map.GetAverageZ(poi.X, poi.Y);
            loc = new Point3D(poi.X, poi.Y, z);
            radius = Math.Max(1, poi.Radius);
            return true;
        }

        private static Map ResolveMap(string mapName)
        {
            if (string.IsNullOrWhiteSpace(mapName))
                return Map.Felucca;

            // Common ServUO map names
            if (mapName.Equals("Felucca", StringComparison.OrdinalIgnoreCase)) return Map.Felucca;
            if (mapName.Equals("Trammel", StringComparison.OrdinalIgnoreCase)) return Map.Trammel;
            if (mapName.Equals("Ilshenar", StringComparison.OrdinalIgnoreCase)) return Map.Ilshenar;
            if (mapName.Equals("Malas", StringComparison.OrdinalIgnoreCase)) return Map.Malas;
            if (mapName.Equals("Tokuno", StringComparison.OrdinalIgnoreCase)) return Map.Tokuno;
            if (mapName.Equals("TerMur", StringComparison.OrdinalIgnoreCase)) return Map.TerMur;

            return Map.Felucca;
        }

        public static IEnumerable<VystiaPoi> GetAll()
        {
            EnsureLoaded();
            return _byId.Values;
        }
    }
}


