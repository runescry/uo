using System;
using Server.Commands;

namespace Server.Misc
{
    public class VystiaMapDiagnostics
    {
        public static void Initialize()
        {
            CommandSystem.Register("VystiaDiagnostics", AccessLevel.Administrator, new CommandEventHandler(VystiaDiagnostics_OnCommand));
        }

        [Usage("VystiaDiagnostics")]
        [Description("Diagnostics for Vystia Map 9 configuration")]
        private static void VystiaDiagnostics_OnCommand(CommandEventArgs e)
        {
            Mobile from = e.Mobile;
            
            from.SendMessage("=== Vystia Map 9 Diagnostics ===");
            
            // Check if map is registered
            Map vystia = Map.Vystia;
            if (vystia != null)
            {
                from.SendMessage("✓ Map.Vystia is accessible");
                from.SendMessage("  Name: {0}", vystia.Name);
                from.SendMessage("  Index: {0}", vystia.MapIndex);
                from.SendMessage("  Width: {0}, Height: {1}", vystia.Width, vystia.Height);
            }
            else
            {
                from.SendMessage("✗ Map.Vystia is NULL!");
            }
            
            // Check Map.Maps array
            if (Map.Maps[9] != null)
            {
                from.SendMessage("✓ Map.Maps[9] is registered");
            }
            else
            {
                from.SendMessage("✗ Map.Maps[9] is NULL!");
            }
            
            // Check Map.AllMaps
            bool foundInAllMaps = false;
            foreach (Map map in Map.AllMaps)
            {
                if (map != null && map.Name == "Vystia")
                {
                    foundInAllMaps = true;
                    from.SendMessage("✓ Found in Map.AllMaps");
                    break;
                }
            }
            if (!foundInAllMaps)
            {
                from.SendMessage("✗ NOT found in Map.AllMaps");
            }
            
            // Check Map.Parse
            Map parsed = Map.Parse("Vystia");
            if (parsed != null)
            {
                from.SendMessage("✓ Map.Parse(\"Vystia\") works");
            }
            else
            {
                from.SendMessage("✗ Map.Parse(\"Vystia\") returns NULL");
            }
            
            // Check file paths
            from.SendMessage("");
            from.SendMessage("=== File Paths ===");
            string map9Path = Core.FindDataFile("map9.mul");
            string staidx9Path = Core.FindDataFile("staidx9.mul");
            string statics9Path = Core.FindDataFile("statics9.mul");
            
            from.SendMessage("map9.mul: {0}", map9Path ?? "NOT FOUND");
            from.SendMessage("staidx9.mul: {0}", staidx9Path ?? "NOT FOUND");
            from.SendMessage("statics9.mul: {0}", statics9Path ?? "NOT FOUND");
            
            // Check if tiles are loaded
            if (vystia != null)
            {
                from.SendMessage("");
                from.SendMessage("=== TileMatrix Status ===");
                from.SendMessage("Tiles object: {0}", vystia.Tiles != null ? "Exists" : "NULL");
                if (vystia.Tiles != null)
                {
                    from.SendMessage("Tiles.Exists: {0}", vystia.Tiles.Exists ? "Yes" : "No");
                }
            }
            
            // Check Ultima.Files paths
            from.SendMessage("");
            from.SendMessage("=== Ultima.Files Paths ===");
            string filesMap9 = Ultima.Files.GetFilePath("map9.mul");
            string filesStaidx9 = Ultima.Files.GetFilePath("staidx9.mul");
            string filesStatics9 = Ultima.Files.GetFilePath("statics9.mul");
            
            from.SendMessage("Files.GetFilePath(\"map9.mul\"): {0}", filesMap9 ?? "NOT FOUND");
            from.SendMessage("Files.GetFilePath(\"staidx9.mul\"): {0}", filesStaidx9 ?? "NOT FOUND");
            from.SendMessage("Files.GetFilePath(\"statics9.mul\"): {0}", filesStatics9 ?? "NOT FOUND");
        }
    }
}

