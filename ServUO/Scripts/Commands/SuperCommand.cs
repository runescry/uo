using System;
using Server;
using Server.Commands;
using Server.Mobiles;

namespace Server.Scripts.Commands
{
    public class SuperCommand
    {
        public static void Initialize()
        {
            CommandSystem.Register("Super", AccessLevel.GameMaster, new CommandEventHandler(Super_OnCommand));
            CommandSystem.Register("God", AccessLevel.GameMaster, new CommandEventHandler(Super_OnCommand));
            CommandSystem.Register("MaxAll", AccessLevel.GameMaster, new CommandEventHandler(Super_OnCommand));
        }

        [Usage("Super")]
        [Aliases("God", "MaxAll")]
        [Description("Sets all your stats to 1000 and all skills to 100.0. Usage: [super")]
        public static void Super_OnCommand(CommandEventArgs e)
        {
            Mobile from = e.Mobile;

            if (from == null)
                return;

            // Set stats to 1000
            from.RawStr = 1000;
            from.RawDex = 1000;
            from.RawInt = 1000;

            // Set stat caps to 1000
            from.StrCap = 1000;
            from.DexCap = 1000;
            from.IntCap = 1000;

            // Set all skills to 100.0
            foreach (Skill skill in from.Skills)
            {
                skill.Base = 100.0; // 100.0 display value
                skill.Cap = 100.0;
            }

            // Update the player
            from.UpdateTotals();

            from.SendMessage(0x35, "GOD MODE ACTIVATED!");
            from.SendMessage("All stats set to 1000.");
            from.SendMessage("All skills set to 100.0 (GM level).");
        }
    }
}
