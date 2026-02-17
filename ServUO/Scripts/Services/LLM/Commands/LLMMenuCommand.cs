using System;
using Server;
using Server.Commands;

namespace Server.Services.LLM
{
    /// <summary>
    /// Simple command to show available LLM NPC commands
    /// </summary>
    public class LLMMenuCommand
    {
        public static void Initialize()
        {
            CommandSystem.Register("LLMMenu", AccessLevel.GameMaster, LLMMenu_OnCommand);
            CommandSystem.Register("SpawnLLM", AccessLevel.GameMaster, LLMMenu_OnCommand);
        }

        [Usage("LLMMenu")]
        [Description("Shows available LLM NPC commands")]
        private static void LLMMenu_OnCommand(CommandEventArgs e)
        {
            Mobile from = e.Mobile;

            from.SendMessage("=== LLM NPC Commands ===");
            from.SendMessage("");
            from.SendMessage("=== LLM QUEST SYSTEMS (SEPARATE) ===");
            from.SendMessage("Vystia Dynamic Quests (LLM-generated via Chronicler/QuestNPCs):");
            from.SendMessage("  [SpawnVystiaQuestGiver Chronicler - LLM quest generator");
            from.SendMessage("  [GenLLMQuest - Generate a demo Vystia dynamic quest");
            from.SendMessage("  [aqn - Quest wizard (spawn/link QuestNPCs)");
            from.SendMessage("");
            from.SendMessage("Mondain/BaseQuest (LLMQuester system):");
            from.SendMessage("  [SpawnLLMQuester - Spawns a Mondain quest NPC (SimpleGatherQuest)");
            from.SendMessage("");
            from.SendMessage("[SpawnPersonalityNPC <personality> <speech> - Spawn NPC with personality");
            from.SendMessage("  Personalities: Merchant, Guard, Noble, Sage, Commoner, Villain, Hermit, Healer, Warrior, Mage");
            from.SendMessage("  Speech: Modern, Formal, OldEnglish, Cryptic, Casual, Archaic");
            from.SendMessage("");
            from.SendMessage("Example: [SpawnPersonalityNPC Sage OldEnglish");
            from.SendMessage("Example: [SpawnPersonalityNPC Guard Formal");
            from.SendMessage("");
            from.SendMessage("[SpawnLLMPersonality <type> - Spawn pre-configured NPC");
            from.SendMessage("  Use: [SpawnLLMPersonality list - to see all types");
            from.SendMessage("");
            from.SendMessage("[LLMConfig - Configure LLM provider settings");
            from.SendMessage("[GenerateEmbeddings - Generate lore embeddings (one-time)");
            from.SendMessage("[VectorLoreStats - View lore system statistics");
        }
    }
}
