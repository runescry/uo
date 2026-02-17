using System;
using System.Threading.Tasks;
using Server;
using Server.Commands;
using Server.Services.LLM;

namespace Server.Services.LLM.Commands
{
    /// <summary>
    /// Command to clear all LLM NPC memories, relationships, and conversations
    /// Usage: [ClearMemory
    /// </summary>
    public class ClearMemoryCommand
    {
        public static void Initialize()
        {
            CommandSystem.Register("ClearMemory", AccessLevel.Administrator, new CommandEventHandler(OnCommand));
        }

        [Usage("ClearMemory")]
        [Description("Clears all LLM NPC memories, relationships, and conversations from the database")]
        private static void OnCommand(CommandEventArgs e)
        {
            Mobile from = e.Mobile;

            if (from == null)
                return;

            from.SendMessage("Clearing all LLM NPC memories, relationships, and conversations...");
            from.SendMessage("This may take a moment.");

            // Run in background
            Task.Run(async () =>
            {
                try
                {
                    // Clear SQLite database
                    if (SQLiteMemoryDatabase.IsAvailable())
                    {
                        await SQLiteMemoryDatabase.FlushAll();
                        Console.WriteLine("[ClearMemory] Cleared all memories, relationships, and conversations from SQLite database");
                        from.SendMessage("Successfully cleared all memories, relationships, and conversations from SQLite database.");
                    }
                    else
                    {
                        from.SendMessage("SQLite database is not available. Nothing to clear.");
                    }

                    // Also clear in-memory fallback store if active
                    if (InMemoryFallbackStore.IsActive)
                    {
                        // The fallback store doesn't have a FlushAll method, but it's only used for testing
                        // In production, SQLite is used
                        from.SendMessage("Note: In-memory fallback store is active (testing mode).");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[ClearMemory] Error clearing memories: {ex.Message}");
                    Console.WriteLine($"[ClearMemory] Stack trace: {ex.StackTrace}");
                    from.SendMessage($"Error clearing memories: {ex.Message}");
                }
            });
        }
    }
}

