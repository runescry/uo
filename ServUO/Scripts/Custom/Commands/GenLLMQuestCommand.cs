using System;
using Server;
using Server.Commands;
using Server.Mobiles;
using Server.Custom.VystiaClasses.Quests.Generation;
using Newtonsoft.Json;

namespace Server.Custom.Commands
{
    public class GenLLMQuestCommand
    {
        public static void Initialize()
        {
            CommandSystem.Register("GenLLMQuest", AccessLevel.GameMaster, new CommandEventHandler(GenLLMQuest_OnCommand));
        }

        [Usage("GenLLMQuest [poiId]")]
        [Description("Generates a demo LLM quest. Example: [GenLLMQuest IRONCLAD_BRITAIN_CENTER")]
        private static void GenLLMQuest_OnCommand(CommandEventArgs e)
        {
            Mobile from = e.Mobile;
            if (!(from is PlayerMobile pm))
            {
                from.SendMessage("This command only works on PlayerMobiles.");
                return;
            }

            string poiId = e.Length > 0 ? e.GetString(0) : "IRONCLAD_BRITAIN_CENTER";

            // Generate a demo quest plan
            string demoJson = GenerateDemoQuestPlan(poiId, pm.Name);

            // Try to create the quest
            if (LLMQuestGenerationService.CreateFromPlanJson(pm, demoJson, out int questId, out string error))
            {
                from.SendMessage(68, $"Success! Generated quest ID {questId} at POI: {poiId}");
                from.SendMessage(68, "Accept the quest from the offer gump to start!");
            }
            else
            {
                from.SendMessage(38, $"Failed to generate quest: {error}");
            }
        }

        /// <summary>
        /// Generates a demo quest plan JSON for testing the pipeline.
        /// In production, this would come from an actual LLM planner.
        /// </summary>
        private static string GenerateDemoQuestPlan(string poiId, string playerName)
        {
            var plan = new
            {
                title = "The Ironclad Supplies Crisis",
                description = $"Quartermaster Grimwald needs urgent assistance gathering supplies for the Ironclad garrison. Help him before morale collapses!",
                tier = 1,
                expiresMinutes = 120,
                waypoints = new object[]
                {
                    new
                    {
                        type = "Origin",
                        condition = "TalkToNPC",
                        poiId = poiId,
                        npcTemplateId = "QUEST_QUARTERMASTER",
                        npcDialogueContext = $"Greetings, {playerName}. The garrison is running dangerously low on supplies. Can you help?",
                        playerLocationHint = "Seek Quartermaster Grimwald near the Ironclad forge district."
                    },
                    new
                    {
                        type = "Waypoint",
                        condition = "DefeatBoss",
                        poiId = "IRONCLAD_BRITAIN_CENTER",
                        bossTypeName = "ForgeMaster",
                        npcDialogueContext = "The Forge Master has gone rogue and seized our supply depot. Defeat him to reclaim our resources!",
                        playerLocationHint = "Journey to the Great Forge and confront the Forge Master."
                    },
                    new
                    {
                        type = "NPCCompletion",
                        condition = "TalkToNPC",
                        poiId = poiId,
                        npcTemplateId = "QUEST_QUARTERMASTER",
                        npcDialogueContext = "Excellent work! The supplies are secure and morale is restored. The garrison thanks you!",
                        playerLocationHint = "Return to Quartermaster Grimwald with news of your victory."
                    }
                }
            };

            return JsonConvert.SerializeObject(plan, Formatting.Indented);
        }
    }
}
