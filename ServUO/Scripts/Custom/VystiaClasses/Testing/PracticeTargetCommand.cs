using System;
using Server;
using Server.Commands;
using Server.Mobiles;

namespace Server.Custom.VystiaClasses.Testing
{
    /// <summary>
    /// GM command to spawn a Practice Target (infinite HP NPC for spell testing)
    /// Usage: [PracticeTarget or [PT
    /// </summary>
    public class PracticeTargetCommand
    {
        public static void Initialize()
        {
            CommandSystem.Register("PracticeTarget", AccessLevel.GameMaster, OnCommand);
            CommandSystem.Register("PT", AccessLevel.GameMaster, OnCommand);
        }

        [Usage("PracticeTarget")]
        [Aliases("PT")]
        [Description("Spawns a Practice Target at your location. This NPC has 100 million HP and 0% resistances to show spell effects clearly.")]
        private static void OnCommand(CommandEventArgs e)
        {
            Mobile from = e.Mobile;
            
            if (from == null)
                return;

            PracticeDummy dummy = new PracticeDummy();
            dummy.MoveToWorld(from.Location, from.Map);
            
            from.SendMessage(0x3B2, "Practice Target spawned! This NPC has 100 million HP and 0% resistances.");
            from.SendMessage(0x3B2, "Cast spells on it to see full visual effects. It cannot die.");
        }
    }
}

