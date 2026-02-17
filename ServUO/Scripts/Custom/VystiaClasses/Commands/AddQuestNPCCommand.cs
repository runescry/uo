using System;
using Server;
using Server.Commands;
using Server.Mobiles;
using Server.Custom.VystiaClasses.Quests;
using Server.Gumps;
using Server.Services.LLM;
using Server.Custom.VystiaClasses.Gumps;

namespace Server.Custom.VystiaClasses.Commands
{
    /// <summary>
    /// GM command to spawn a QuestNPC linked to a quest waypoint
    /// Usage: [addquestNPC or [aqn
    /// </summary>
    public class AddQuestNPCCommand
    {
        public static void Initialize()
        {
            CommandSystem.Register("addquestNPC", AccessLevel.GameMaster, new CommandEventHandler(OnAddQuestNPC));
            CommandSystem.Register("aqn", AccessLevel.GameMaster, new CommandEventHandler(OnAddQuestNPC));
        }

        [Usage("addquestNPC")]
        [Aliases("aqn")]
        [Description("Opens a wizard to spawn a QuestNPC linked to a quest waypoint.")]
        private static void OnAddQuestNPC(CommandEventArgs e)
        {
            Mobile from = e.Mobile;

            if (from == null)
                return;

            // Quest Wizard is the only supported flow now (NPC linking is integrated in Step 7).
            from.SendGump(new VystiaQuestWizardGump(
                from,
                VystiaQuestWizardGump.WizardStep.SelectOrCreate,
                0,
                0,
                false,
                "",
                "",
                NPCPersonalities.PersonalityType.Sage,
                NPCPersonalities.SpeechPattern.Formal,
                VystiaQuestWizardGump.WizardStep.SpawnNPCs));
        }
    }
}
