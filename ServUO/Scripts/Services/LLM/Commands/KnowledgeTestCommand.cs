using System;
using System.Threading.Tasks;
using Server;
using Server.Commands;
using Server.Services.LLM.Tests;

namespace Server.Services.LLM.Commands
{
    /// <summary>
    /// Command to run automated knowledge testing
    /// Usage: [KnowledgeTest
    /// </summary>
    public class KnowledgeTestCommand
    {
        public static void Initialize()
        {
            CommandSystem.Register("KnowledgeTest", AccessLevel.Administrator, new CommandEventHandler(OnCommand));
        }

        [Usage("KnowledgeTest")]
        [Description("Runs automated knowledge testing for LLM NPCs")]
        private static void OnCommand(CommandEventArgs e)
        {
            Mobile from = e.Mobile;

            if (from == null)
                return;

            from.SendMessage("Starting automated knowledge testing...");
            from.SendMessage("This may take a few minutes. Check console for results.");

            // Run tests in background
            Task.Run(async () =>
            {
                try
                {
                    await AutomatedKnowledgeTest.RunAllTests();
                    from.SendMessage("Knowledge testing complete!");
                    from.SendMessage("Check console and Data/LLM/Tests/ folder for detailed results.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[KnowledgeTest] Error running tests: {ex.Message}");
                    Console.WriteLine($"[KnowledgeTest] Stack trace: {ex.StackTrace}");
                    from.SendMessage($"Error running tests: {ex.Message}");
                }
            });
        }
    }
}

