using System;
using Server.Commands;

namespace Server.Commands
{
    public class TestMap
    {
        public static void Initialize()
        {
            CommandSystem.Register("TestMap", AccessLevel.Administrator, new CommandEventHandler(TestMap_OnCommand));
        }

        [Usage("TestMap [mapname]")]
        [Description("Tests if a map is loaded and accessible. Shows map information.")]
        private static void TestMap_OnCommand(CommandEventArgs e)
        {
            if (e.Length > 0)
            {
                string mapName = e.GetString(0);
                Map map = Map.Parse(mapName);
                
                if (map != null)
                {
                    e.Mobile.SendMessage("Map '{0}' found!", mapName);
                    e.Mobile.SendMessage("  Index: {0}", map.MapIndex);
                    e.Mobile.SendMessage("  Name: {0}", map.Name);
                    e.Mobile.SendMessage("  Width: {0}, Height: {1}", map.Width, map.Height);
                    e.Mobile.SendMessage("  Tiles loaded: {0}", map.Tiles != null ? "Yes" : "No");
                    if (map.Tiles != null)
                    {
                        e.Mobile.SendMessage("  Tiles exist: {0}", map.Tiles.Exists ? "Yes" : "No");
                    }
                }
                else
                {
                    e.Mobile.SendMessage("Map '{0}' not found!", mapName);
                }
            }
            else
            {
                e.Mobile.SendMessage("Available maps:");
                foreach (Map map in Map.AllMaps)
                {
                    if (map != null && map != Map.Internal)
                    {
                        e.Mobile.SendMessage("  {0} (Index: {1})", map.Name, map.MapIndex);
                    }
                }
            }
        }
    }
}

