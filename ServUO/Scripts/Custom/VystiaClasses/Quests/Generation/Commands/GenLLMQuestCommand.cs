using System;
using System.Threading.Tasks;
using Server;
using Server.Commands;
using Server.Mobiles;

namespace Server.Custom.VystiaClasses.Quests.Generation.Commands
{
    public static class GenLLMQuestCommand
    {
        public static void Initialize()
        {
            CommandSystem.Register("GenLLMQuest", AccessLevel.GameMaster, OnCommand);
        }

        [Usage("GenLLMQuest [poiId|theme]")]
        [Description("Generates an ephemeral DynamicQuest using LLM. Optionally specify POI or theme.")]
        private static void OnCommand(CommandEventArgs e)
        {
            if (!(e.Mobile is PlayerMobile pm))
            {
                e.Mobile.SendMessage("This command requires a PlayerMobile.");
                return;
            }

            string arg = e.ArgString?.Trim();
            bool useLLM = true; // Use LLM by default

            // If arg looks like a POI ID (uppercase with underscores), use demo plan for testing
            if (!string.IsNullOrWhiteSpace(arg) && arg.Contains("_") && arg == arg.ToUpper())
            {
                useLLM = false; // Use demo plan for known POI IDs
            }

            pm.SendMessage("[LLMQuest] Generating quest plan...");

            if (useLLM)
            {
                // Use LLM to generate plan
                Task.Run(async () =>
                {
                    try
                    {
                        string json = await LLMQuestGenerationService.GeneratePlanJsonAsync(pm, arg);
                        if (string.IsNullOrWhiteSpace(json))
                        {
                            pm.SendMessage(38, "[LLMQuest] LLM generation failed. Falling back to demo plan.");
                            // Fallback to demo
                            json = LLMQuestGenerationService.BuildDemoPlanJson("IRONCLAD_BRITAIN_CENTER");
                        }

                        if (LLMQuestGenerationService.CreateFromPlanJson(pm, json, out int questId, out string error))
                        {
                            pm.SendMessage(68, $"[LLMQuest] Generated quest id {questId} (ephemeral, LLM-generated).");
                        }
                        else
                        {
                            pm.SendMessage(38, "[LLMQuest] Failed: " + error);
                        }
                    }
                    catch (Exception ex)
                    {
                        pm.SendMessage(38, $"[LLMQuest] Error: {ex.Message}");
                        Console.WriteLine($"[GenLLMQuest] Error: {ex.Message}\n{ex.StackTrace}");
                    }
                });
            }
            else
            {
                // Use demo plan (for testing)
                string poiId = arg ?? "IRONCLAD_BRITAIN_CENTER";
                string json = LLMQuestGenerationService.BuildDemoPlanJson(poiId);
                if (LLMQuestGenerationService.CreateFromPlanJson(pm, json, out int questId, out string error))
                {
                    pm.SendMessage($"[LLMQuest] Generated quest id {questId} (ephemeral, demo plan).");
                }
                else
                {
                    pm.SendMessage("[LLMQuest] Failed: " + error);
                }
            }
        }
    }
}


