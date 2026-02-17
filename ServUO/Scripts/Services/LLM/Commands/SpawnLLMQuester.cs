using System;
using Server;
using Server.Commands;
using Server.Mobiles;
using Server.Services.LLM.Examples;
using Server.Targeting;

namespace Server.Services.LLM
{
    /// <summary>
    /// Command to spawn LLM quest NPCs (Mondain/BaseQuest system, not Vystia DynamicQuests)
    /// </summary>
    public class SpawnLLMQuesterCommand
    {
        public static void Initialize()
        {
            CommandSystem.Register("SpawnLLMQuester", AccessLevel.GameMaster, SpawnLLMQuester_OnCommand);
            CommandSystem.Register("SpawnWoodcutter", AccessLevel.GameMaster, SpawnWoodcutter_OnCommand);
        }

        [Usage("SpawnLLMQuester")]
        [Description("Spawns an LLM quest NPC (Mondain/BaseQuest) at the targeted location")]
        private static void SpawnLLMQuester_OnCommand(CommandEventArgs e)
        {
            Mobile from = e.Mobile;
            from.SendMessage("Target a location to spawn an LLM quest NPC.");
            from.SendMessage("This uses the Mondain/BaseQuest system (not Vystia DynamicQuests).");
            from.SendMessage("This will spawn a Woodcutter offering the Simple Gather Quest.");

            from.Target = new InternalTarget();
        }

        [Usage("SpawnWoodcutter")]
        [Description("Spawns a Woodcutter quest NPC (Mondain/BaseQuest) offering the Simple Gather Quest")]
        private static void SpawnWoodcutter_OnCommand(CommandEventArgs e)
        {
            Mobile from = e.Mobile;
            from.SendMessage("Target a location to spawn the Woodcutter quest NPC.");
            from.SendMessage("This uses the Mondain/BaseQuest system (not Vystia DynamicQuests).");

            from.Target = new InternalTarget();
        }

        private class InternalTarget : Target
        {
            public InternalTarget() : base(12, true, TargetFlags.None)
            {
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                IPoint3D p = targeted as IPoint3D;
                if (p == null)
                {
                    from.SendMessage("Invalid target.");
                    return;
                }

                Point3D loc = new Point3D(p);
                Map map = from.Map;

                if (map == null || map == Map.Internal)
                {
                    from.SendMessage("Invalid location.");
                    return;
                }

                // Create the LLM quester
                LLMQuester quester = new LLMQuester(
                    "Theron the Woodcutter",
                    typeof(SimpleGatherQuest),
                    NPCPersonalities.PersonalityType.Commoner,
                    NPCPersonalities.SpeechPattern.Casual
                );

                quester.MoveToWorld(loc, map);

                from.SendMessage("Spawned Theron the Woodcutter offering the Simple Gather Quest!");
                from.SendMessage("Try talking to him to get the quest.");
            }
        }
    }
}
