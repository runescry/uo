using System;
using System.Threading.Tasks;
using Server;
using Server.Commands;
using Server.Services.LLM.Tests;

namespace Server.Services.LLM.Commands
{
    /// <summary>
    /// Command to run location-based response testing
    /// Usage: [LocationTest
    /// </summary>
    public class LocationTestCommand
    {
        public static void Initialize()
        {
            CommandSystem.Register("LocationTest", AccessLevel.Administrator, new CommandEventHandler(OnCommand));
        }

        [Usage("LocationTest")]
        [Description("Runs automated location-based response testing for LLM NPCs")]
        private static void OnCommand(CommandEventArgs e)
        {
            Mobile from = e.Mobile;

            if (from == null)
                return;

            from.SendMessage("Starting location-based response testing...");
            from.SendMessage("This may take a few minutes. Check console for results.");

            // Run tests in background
            Task.Run(async () =>
            {
                try
                {
                    await LocationBasedResponseTests.RunAllTests();
                    from.SendMessage("Location testing complete!");
                    from.SendMessage("Check console and Data/LLM/Tests/ folder for detailed results.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[LocationTest] Error running tests: {ex.Message}");
                    Console.WriteLine($"[LocationTest] Stack trace: {ex.StackTrace}");
                    from.SendMessage($"Error running tests: {ex.Message}");
                }
            });
        }
    }
}

