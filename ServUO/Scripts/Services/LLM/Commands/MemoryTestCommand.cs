using System;
using System.Threading.Tasks;
using Server;
using Server.Commands;
using Server.Services.LLM.Tests;

namespace Server.Services.LLM.Commands
{
    public class MemoryTestCommand
    {
        public static void Initialize()
        {
            CommandSystem.Register("MemoryTest", AccessLevel.Administrator, new CommandEventHandler(OnCommand));
        }

        [Usage("MemoryTest")]
        [Description("Runs comprehensive long-term memory testing for LLM NPCs")]
        private static void OnCommand(CommandEventArgs e)
        {
            Mobile from = e.Mobile;

            if (from == null)
                return;

            from.SendMessage("Starting comprehensive memory testing...");
            from.SendMessage("This will test long-term memory with complex scenarios.");
            from.SendMessage("Check console and log file for results.");

            // Run tests in background
            Task.Run(async () =>
            {
                try
                {
                    await LongTermMemoryTests.RunAllTests();
                    from.SendMessage($"Memory testing complete! Check console and log file: {LongTermMemoryTests.LogFilePath}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[MemoryTest] Error running tests: {ex.Message}");
                    Console.WriteLine($"[MemoryTest] Stack trace: {ex.StackTrace}");
                    from.SendMessage($"Error running tests: {ex.Message}");
                }
            });
        }
    }
}

