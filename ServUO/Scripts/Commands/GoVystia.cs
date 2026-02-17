using System;
using Server;
using Server.Commands;

namespace Server.Scripts.Commands
{
    public class GoVystia
    {
        public static void Initialize()
        {
            CommandSystem.Register("GoVystia", AccessLevel.Counselor, new CommandEventHandler(GoVystia_OnCommand));
            CommandSystem.Register("gv", AccessLevel.Counselor, new CommandEventHandler(GoVystia_OnCommand));
        }

        [Usage("GoVystia [x y [z]]")]
        [Description("Teleports to Vystia map. Without coordinates, goes to 1000,1000. With x y, goes to those coordinates.")]
        private static void GoVystia_OnCommand(CommandEventArgs e)
        {
            Mobile from = e.Mobile;

            // Find Vystia map (index 9)
            Map vystia = Map.Maps[9];

            if (vystia == null)
            {
                from.SendMessage("Error: Vystia map (index 9) is not registered.");
                from.SendMessage("Available maps:");
                foreach (Map m in Map.AllMaps)
                {
                    if (m != null && m.MapIndex != 0x7F && m.MapIndex != 0xFF)
                        from.SendMessage("  - {0} (index {1})", m.Name, m.MapIndex);
                }
                return;
            }

            int x, y, z;

            if (e.Length == 0)
            {
                // Default to 1000, 1000
                x = 1000;
                y = 1000;
            }
            else if (e.Length >= 2)
            {
                x = e.GetInt32(0);
                y = e.GetInt32(1);
            }
            else
            {
                from.SendMessage("Usage: [GoVystia [x y [z]]");
                return;
            }

            if (e.Length >= 3)
            {
                z = e.GetInt32(2);
            }
            else
            {
                z = vystia.GetAverageZ(x, y);
            }

            from.SendMessage("Teleporting to Vystia at {0}, {1}, {2}...", x, y, z);
            from.MoveToWorld(new Point3D(x, y, z), vystia);
            from.SendMessage("Arrived at Vystia ({0}).", vystia.Name);
        }
    }
}
