/*
 * Vystia Server-Side Test Runner
 *
 * Executes test commands directly on the server without requiring client connection.
 * All output is logged to files and console.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using Server;
using Server.Commands;
using Server.Items;
using Server.Mobiles;

namespace Server.Custom.VystiaClasses.Testing
{
    /// <summary>
    /// Server-side test runner that executes commands programmatically
    /// </summary>
    public static class ServerTestRunner
    {
        private static ConsoleMobile s_SystemMobile;

        /// <summary>
        /// Initialize the test runner
        /// </summary>
        public static void Initialize()
        {
            // Create a system mobile for command execution
            s_SystemMobile = new ConsoleMobile();
            s_SystemMobile.AccessLevel = AccessLevel.Administrator;
            s_SystemMobile.Name = "System Test Runner";
            
            TestLogging.WriteLine("Server Test Runner initialized");
        }

        /// <summary>
        /// Execute a command programmatically
        /// </summary>
        public static void ExecuteCommand(string command, string[] args = null)
        {
            if (s_SystemMobile == null)
                Initialize();

            if (args == null)
                args = new string[0];

            string argString = string.Join(" ", args);
            CommandEventArgs e = new CommandEventArgs(s_SystemMobile, command, argString, args);

            TestLogging.WriteLine("EXECUTING COMMAND: {0} {1}", command, argString);

            try
            {
                // Find and execute the command
                CommandEntry entry = null;
                CommandSystem.Entries.TryGetValue(command, out entry);

                if (entry != null)
                {
                    if (s_SystemMobile.AccessLevel >= entry.AccessLevel)
                    {
                        if (entry.Handler != null)
                        {
                            entry.Handler(e);
                            EventSink.InvokeCommand(e);
                            TestLogging.WriteLine("COMMAND EXECUTED: {0}", command);
                        }
                        else
                        {
                            TestLogging.WriteLine("ERROR: Command {0} has no handler", command);
                        }
                    }
                    else
                    {
                        TestLogging.WriteLine("ERROR: Insufficient access level for command {0}", command);
                    }
                }
                else
                {
                    TestLogging.WriteLine("ERROR: Command {0} not found", command);
                }
            }
            catch (Exception ex)
            {
                TestLogging.LogException($"ExecuteCommand({command})", ex);
            }
        }

        /// <summary>
        /// Run the full automated test suite
        /// </summary>
        public static void RunAutomatedTests(string scope = "all")
        {
            TestLogging.WriteLine("");
            TestLogging.WriteLine("=== STARTING AUTOMATED TEST SUITE ===");
            TestLogging.WriteLine("Scope: {0}", scope);
            TestLogging.WriteLine("Time: {0}", DateTime.UtcNow);
            TestLogging.WriteLine("");

            ExecuteCommand("TestVystiaSystems", new[] { scope });

            TestLogging.WriteLine("");
            TestLogging.WriteLine("=== AUTOMATED TEST SUITE COMPLETED ===");
        }

        /// <summary>
        /// Run a sequence of test setup commands
        /// </summary>
        public static void RunTestSetup(string setupType)
        {
            TestLogging.WriteLine("");
            TestLogging.WriteLine("=== RUNNING TEST SETUP: {0} ===", setupType);
            TestLogging.WriteLine("Time: {0}", DateTime.UtcNow);

            switch (setupType.ToLower())
            {
                case "crafting":
                    ExecuteCommand("TestCraftingSetup", new[] { "runecrafting" });
                    break;
                case "potions":
                    ExecuteCommand("TestPotionSetup");
                    break;
                case "constructs":
                    ExecuteCommand("TestConstructSetup");
                    break;
                case "devotion":
                    ExecuteCommand("TestDevotionPowerSetup", new[] { "frostfather" });
                    break;
                case "faction":
                    ExecuteCommand("TestFactionSetup", new[] { "frostguard", "exalted" });
                    break;
                case "integration":
                    ExecuteCommand("TestIntegrationSetup");
                    break;
                case "all":
                    ExecuteCommand("TestCraftingSetup", new[] { "runecrafting" });
                    ExecuteCommand("TestPotionSetup");
                    ExecuteCommand("TestConstructSetup");
                    ExecuteCommand("TestDevotionPowerSetup", new[] { "frostfather" });
                    ExecuteCommand("TestFactionSetup", new[] { "frostguard", "exalted" });
                    ExecuteCommand("TestIntegrationSetup");
                    break;
                default:
                    TestLogging.WriteLine("ERROR: Unknown setup type: {0}", setupType);
                    break;
            }

            TestLogging.WriteLine("=== TEST SETUP COMPLETED ===");
        }

        /// <summary>
        /// Run all tests with full logging
        /// </summary>
        public static void RunFullTestSuite()
        {
            TestLogging.Initialize();
            
            TestLogging.WriteLine("========================================");
            TestLogging.WriteLine("VYSTIA FULL TEST SUITE");
            TestLogging.WriteLine("Started: {0}", DateTime.UtcNow);
            TestLogging.WriteLine("========================================");
            TestLogging.WriteLine("");

            // Run automated tests
            RunAutomatedTests("all");

            TestLogging.WriteLine("");
            TestLogging.WriteLine("========================================");
            TestLogging.WriteLine("TEST SUITE COMPLETED");
            TestLogging.WriteLine("Ended: {0}", DateTime.UtcNow);
            TestLogging.WriteLine("========================================");
        }

        /// <summary>
        /// A console/system mobile for executing commands
        /// </summary>
        private class ConsoleMobile : Mobile
        {
            public ConsoleMobile()
            {
                // Initialize as a system mobile
                Name = "System";
            }

            public ConsoleMobile(Serial serial) : base(serial)
            {
                // Serialization constructor - required by ServUO
                // This mobile is never actually serialized, but the constructor is required
            }

            public new void SendMessage(string message)
            {
                // Redirect messages to logging instead of client
                TestLogging.WriteLine("SERVER: {0}", message);
            }

            public new void SendMessage(int hue, string message)
            {
                // Redirect colored messages to logging
                TestLogging.WriteLine("SERVER: {0}", message);
            }

            public new void SendMessage(int hue, string format, params object[] args)
            {
                // Redirect formatted messages to logging
                TestLogging.WriteLine("SERVER: {0}", string.Format(format, args));
            }

            public override void OnSpeech(SpeechEventArgs e)
            {
                // System mobile doesn't need speech handling
            }

            public override void OnDamage(int amount, Mobile from, bool willKill)
            {
                // System mobile can't be damaged
            }

            public override void OnDeath(Container c)
            {
                // System mobile can't die
            }

            public override bool CanBeHarmful(IDamageable target, bool message, bool ignoreOurBlessedness)
            {
                return false;
            }

            public override bool CanBeBeneficial(Mobile target, bool message, bool allowDead)
            {
                return false;
            }

            public override void Serialize(GenericWriter writer)
            {
                // System mobile doesn't need serialization
            }

            public override void Deserialize(GenericReader reader)
            {
                // System mobile doesn't need deserialization
            }
        }
    }
}
