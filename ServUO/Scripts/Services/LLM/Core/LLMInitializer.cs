using System;
using Server;
using Server.Commands;
using Server.Services.LLM.Tests;
using Server.Services.LLM.Commands;

namespace Server.Services.LLM
{
    /// <summary>
    /// Initializes the LLM service on server startup
    /// </summary>
    public class LLMInitializer
    {
        private static bool s_Initialized = false;

        public static void Initialize()
        {
            if (s_Initialized)
                return;

            s_Initialized = true;

            // Phase 1: LLM Providers
            LLMService.Initialize();
            LocalLLMService.Initialize();

            // Phase 2: Knowledge Base (RAG)
            VectorLoreSystem.Initialize();

            // Phase 2.5: NPC Location Database
            NPCLocationDatabase.Initialize();

            // Phase 3: Memory System
            LLMMemoryConfig.Configure();
            // Note: SQLite initialization happens inside LLMMemoryService.Initialize()
            LLMMemoryService.Initialize();

            // Phase 4: Background Services
            ConversationCleanupTimer.Initialize();
            CacheCleanupTimer.Initialize();

            // Phase 5: Commands
            ExampleLLMNpcs.Initialize();
            SpawnPersonalityNPCCommand.Initialize();
            SpawnNPCGroups.Initialize();
            LLMNpcGumpCommand.Initialize();
            LLMConfigCommand.Initialize();
            SpawnLLMQuesterCommand.Initialize();
            LoreSystemCommands.Initialize();
            VectorLoreCommands.Initialize();
            CacheCommands.Initialize();
            KnowledgeTestCommand.Initialize();
            MemoryTestCommand.Initialize();
            LocationTestCommand.Initialize();
            ClearMemoryCommand.Initialize();

            // Log personality re-inference summary (if any occurred during world load)
            // Note: This count is accumulated during NPC deserialization (OnAfterLoad)
            int reInferredCount = Server.Mobiles.LLMNpc.GetReInferredCount();
            if (reInferredCount > 0)
            {
                Server.Mobiles.LLMNpc.ResetReInferredCount();
            }
            
            // Simple ready message
            Console.WriteLine("[LLM Service] Ready");
        }
    }
}
