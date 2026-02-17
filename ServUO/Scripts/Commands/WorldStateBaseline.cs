using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Server;
using Server.Items;
using Server.Mobiles;

namespace Server.Commands
{
    /// <summary>
    /// Comprehensive world state baseline that includes spawners AND all decorated items/statics.
    /// This captures the complete current state of the world for create/recreate/delete scripts.
    /// </summary>
    public static class WorldStateBaseline
    {
        /// <summary>
        /// Scans the entire world state including spawners and all decorated items.
        /// </summary>
        public static WorldStateScanResult ScanWorldState()
        {
            var result = new WorldStateScanResult();
            
            // Scan spawners
            result.SpawnerBaseline = SpawnerBaseline.CalculateFromWorldState(SpawnerBaseline.CalculationMode.Mode);
            
            // Scan decorated items from WeakEntityCollection
            ScanDecoratedItems(result);
            
            // Scan all statics and items that might be decorations
            ScanWorldItems(result);
            
            return result;
        }

        private static void ScanDecoratedItems(WorldStateScanResult result)
        {
            // Known decoration collection keys
            string[] decorationKeys = { "deco", "ml", "sa", "tol", "highseas", "doom", "khaldun", "malas" };
            
            foreach (string key in decorationKeys)
            {
                var collection = WeakEntityCollection.GetCollection(key);
                if (collection != null && collection.Count > 0)
                {
                    var itemData = new DecorationCollectionData
                    {
                        CollectionKey = key,
                        ItemCount = 0
                    };

                    foreach (var entity in collection)
                    {
                        if (entity is Item item && !item.Deleted)
                        {
                            itemData.ItemCount++;
                            var itemInfo = CreateItemInfo(item);
                            if (itemInfo != null)
                            {
                                itemData.Items.Add(itemInfo);
                            }
                        }
                    }

                    if (itemData.ItemCount > 0)
                    {
                        result.DecoratedItems[key] = itemData;
                    }
                }
            }
        }

        private static void ScanWorldItems(WorldStateScanResult result)
        {
            // Scan for statics and other items that might be decorations
            // This includes items that are not movable (typical of decorations)
            int staticCount = 0;
            int spawnerCount = 0;
            int otherDecorationCount = 0;

            foreach (Item item in World.Items.Values)
            {
                if (item == null || item.Deleted)
                    continue;

                // Count spawners separately
                if (item is Spawner)
                {
                    spawnerCount++;
                    continue;
                }

                // Count statics (non-movable items that are likely decorations)
                if (!item.Movable && item.Parent == null)
                {
                    staticCount++;
                    
                    // Check if it's a known decoration type
                    if (IsDecorationType(item))
                    {
                        otherDecorationCount++;
                    }
                }
            }

            result.Statistics.TotalStatics = staticCount;
            result.Statistics.TotalSpawners = spawnerCount;
            result.Statistics.OtherDecorations = otherDecorationCount;
        }

        private static bool IsDecorationType(Item item)
        {
            return item is Static ||
                   item is LocalizedStatic ||
                   item is BaseDoor ||
                   item is Sign ||
                   item is LocalizedSign ||
                   item is Teleporter ||
                   item is Moongate ||
                   item is BaseLight ||
                   item is BaseAddon ||
                   item is MarkContainer ||
                   item is RecallRune;
        }

        private static ItemInfo CreateItemInfo(Item item)
        {
            if (item == null || item.Deleted)
                return null;

            var info = new ItemInfo
            {
                TypeName = item.GetType().Name,
                ItemID = item.ItemID,
                Location = item.Location,
                Map = item.Map != null ? item.Map.Name : "Internal",
                Hue = item.Hue,
                Name = item.Name
            };

            // Add type-specific properties
            if (item is BaseDoor door)
            {
                // Note: BaseDoor doesn't have a Facing property in this version
                // info.Properties["Facing"] = door.Facing.ToString();
                info.Properties["Open"] = door.Open.ToString();
            }
            else if (item is Teleporter tp)
            {
                info.Properties["PointDest"] = tp.PointDest.ToString();
                info.Properties["MapDest"] = tp.MapDest != null ? tp.MapDest.Name : "null";
            }
            else if (item is Moongate gate)
            {
                info.Properties["Target"] = gate.Target.ToString();
                info.Properties["TargetMap"] = gate.TargetMap != null ? gate.TargetMap.Name : "null";
            }
            else if (item is BaseLight light)
            {
                info.Properties["Light"] = light.Light.ToString();
                info.Properties["Protected"] = light.Protected.ToString();
            }
            else if (item is Spawner spawner)
            {
                info.Properties["MinDelay"] = spawner.MinDelay.ToString();
                info.Properties["MaxDelay"] = spawner.MaxDelay.ToString();
                info.Properties["MaxCount"] = spawner.MaxCount.ToString();
                info.Properties["Team"] = spawner.Team.ToString();
                info.Properties["HomeRange"] = spawner.HomeRange.ToString();
                info.Properties["Running"] = spawner.Running.ToString();
                info.Properties["Group"] = spawner.Group.ToString();
            }

            return info;
        }

        /// <summary>
        /// Generates a comprehensive report of the world state.
        /// </summary>
        public static string GenerateReport(WorldStateScanResult result)
        {
            var sb = new StringBuilder();
            
            sb.AppendLine("=== WORLD STATE BASELINE REPORT ===");
            sb.AppendLine($"Generated: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
            sb.AppendLine();

            // Spawner baseline
            sb.AppendLine("--- SPAWNER BASELINE ---");
            if (result.SpawnerBaseline.IsValid)
            {
                sb.AppendLine(result.SpawnerBaseline.ToString());
            }
            else
            {
                sb.AppendLine("No spawners found in world.");
            }
            sb.AppendLine();

            // Statistics
            sb.AppendLine("--- WORLD STATISTICS ---");
            sb.AppendLine($"Total Spawners: {result.Statistics.TotalSpawners}");
            sb.AppendLine($"Total Statics: {result.Statistics.TotalStatics}");
            sb.AppendLine($"Other Decorations: {result.Statistics.OtherDecorations}");
            sb.AppendLine();

            // Decorated items by collection
            sb.AppendLine("--- DECORATED ITEMS BY COLLECTION ---");
            if (result.DecoratedItems.Count > 0)
            {
                foreach (var kvp in result.DecoratedItems.OrderBy(k => k.Key))
                {
                    sb.AppendLine($"Collection '{kvp.Key}': {kvp.Value.ItemCount} items");
                }
            }
            else
            {
                sb.AppendLine("No decorated items found in WeakEntityCollection.");
            }

            return sb.ToString();
        }

        /// <summary>
        /// Generates code for updating SpawnerBaseline defaults.
        /// </summary>
        public static string GenerateSpawnerBaselineCode(WorldStateScanResult result)
        {
            if (!result.SpawnerBaseline.IsValid)
                return "// No spawners found - cannot generate baseline code";

            return SpawnerBaseline.GenerateCodeFromResult(result.SpawnerBaseline);
        }

        /// <summary>
        /// Saves the world state scan to a file.
        /// </summary>
        public static void SaveToFile(WorldStateScanResult result, string filePath = "WorldStateBaseline.txt")
        {
            string report = GenerateReport(result);
            File.WriteAllText(filePath, report);
        }
    }

    /// <summary>
    /// Complete world state scan result including spawners and decorated items.
    /// </summary>
    public class WorldStateScanResult
    {
        public SpawnerBaseline.ScanResult SpawnerBaseline { get; set; }
        public Dictionary<string, DecorationCollectionData> DecoratedItems { get; set; }
        public WorldStatistics Statistics { get; set; }

        public WorldStateScanResult()
        {
            SpawnerBaseline = new SpawnerBaseline.ScanResult();
            DecoratedItems = new Dictionary<string, DecorationCollectionData>();
            Statistics = new WorldStatistics();
        }
    }

    /// <summary>
    /// Data about a decoration collection.
    /// </summary>
    public class DecorationCollectionData
    {
        public string CollectionKey { get; set; }
        public int ItemCount { get; set; }
        public List<ItemInfo> Items { get; set; }

        public DecorationCollectionData()
        {
            Items = new List<ItemInfo>();
        }
    }

    /// <summary>
    /// Information about a single item in the world.
    /// </summary>
    public class ItemInfo
    {
        public string TypeName { get; set; }
        public int ItemID { get; set; }
        public Point3D Location { get; set; }
        public string Map { get; set; }
        public int Hue { get; set; }
        public string Name { get; set; }
        public Dictionary<string, string> Properties { get; set; }

        public ItemInfo()
        {
            Properties = new Dictionary<string, string>();
        }
    }

    /// <summary>
    /// World statistics summary.
    /// </summary>
    public class WorldStatistics
    {
        public int TotalSpawners { get; set; }
        public int TotalStatics { get; set; }
        public int OtherDecorations { get; set; }
    }
}

