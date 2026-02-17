using System;
using System.Linq;
using Server;
using Server.Commands;
using Server.Mobiles;
using Server.Custom.VystiaClasses.Quests;

namespace Server.Custom.VystiaClasses.Quests.Commands
{
    /// <summary>
    /// GM command to delete a quest by ID or title
    /// Usage: [DeleteQuest <id|title>
    /// </summary>
    public class DeleteQuestCommand
    {
        public static void Initialize()
        {
            CommandSystem.Register("DeleteQuest", AccessLevel.GameMaster, OnCommand);
        }

        [Usage("DeleteQuest <id|title>")]
        [Description("Deletes a quest by ID or title. Use quotes for titles with spaces.")]
        private static void OnCommand(CommandEventArgs e)
        {
            Mobile from = e.Mobile;
            if (e.Length < 1)
            {
                from.SendMessage("Usage: [DeleteQuest <id|title>");
                from.SendMessage("Example: [DeleteQuest 1");
                from.SendMessage("Example: [DeleteQuest \"Sample Quest: Trials of Vystia\"");
                
                // List all quests
                var quests = DynamicQuestManager.GetAllDynamicQuests();
                if (quests.Count > 0)
                {
                    from.SendMessage("\nAvailable quests:");
                    foreach (var q in quests.OrderBy(q => q.QuestID).Take(20))
                    {
                        from.SendMessage($"  [{q.QuestID}] {q.Title}");
                    }
                }
                return;
            }

            string arg = e.GetString(0);
            
            // Special case: delete all Sample Quest instances
            if (arg.Equals("sample", StringComparison.OrdinalIgnoreCase) || 
                arg.Equals("sample*", StringComparison.OrdinalIgnoreCase))
            {
                var allQuests = DynamicQuestManager.GetAllDynamicQuests();
                var sampleQuests = allQuests.Where(q => q.Title.IndexOf("Sample", StringComparison.OrdinalIgnoreCase) >= 0).ToList();
                
                if (sampleQuests.Count == 0)
                {
                    from.SendMessage("No Sample Quest found.");
                    return;
                }

                int deleted = 0;
                foreach (var sampleQuest in sampleQuests)
                {
                    if (!sampleQuest.IsEphemeral)
                    {
                        DynamicQuestManager.RemoveDynamicQuest(sampleQuest);
                        deleted++;
                    }
                }
                
                if (deleted > 0)
                {
                    DynamicQuestManager.Save();
                    from.SendMessage($"Deleted {deleted} Sample Quest(s).");
                }
                else
                {
                    from.SendMessage("Sample Quest(s) found but are ephemeral (will auto-cleanup).");
                }
                return;
            }

            DynamicQuest quest = null;

            // Try as ID first
            if (int.TryParse(arg, out int questId))
            {
                quest = DynamicQuestManager.GetQuest(questId);
            }
            else
            {
                // Try as title
                quest = DynamicQuestManager.GetQuestByTitle(arg);
            }

            if (quest == null)
            {
                from.SendMessage($"Quest not found: {arg}");
                return;
            }

            // Check if quest is ephemeral (generated quests)
            if (quest.IsEphemeral)
            {
                from.SendMessage($"Quest [{quest.QuestID}] '{quest.Title}' is ephemeral and will be cleaned up automatically.");
                from.SendMessage("Use [ClearQuests] to clear player quest progress.");
                return;
            }

            // Delete the quest
            DynamicQuestManager.RemoveDynamicQuest(quest);
            DynamicQuestManager.Save();

            from.SendMessage($"Deleted quest [{quest.QuestID}] '{quest.Title}'.");
        }
    }
}

