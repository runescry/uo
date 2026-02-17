/*
 * Vystia Systems Test Command
 *
 * GM command to run automated tests on Faction, Religion, and Crafting systems.
 * Usage: [TestVystiaSystems [faction|religion|crafting|all]
 */

using System;
using System.Collections.Generic;
using System.Linq;
using Server;
using Server.Commands;
using Server.Mobiles;

namespace Server.Custom.VystiaClasses.Testing
{
    public static class TestVystiaSystemsCommand
    {
        public static void Initialize()
        {
            CommandSystem.Register("TestVystiaSystems", AccessLevel.GameMaster, TestVystiaSystems_OnCommand);
            CommandSystem.Register("TVS", AccessLevel.GameMaster, TestVystiaSystems_OnCommand);
            
            // Register server-side test runner command
            CommandSystem.Register("RunServerTests", AccessLevel.Administrator, RunServerTests_OnCommand);
            CommandSystem.Register("RST", AccessLevel.Administrator, RunServerTests_OnCommand);
            
            // Initialize test logging
            TestLogging.Initialize();
            
            // Initialize test scripts
            VystiaTestScripts.Initialize();
            
            // Initialize test results tracker
            TestResultsTracker.Initialize();
            
            // Initialize server test runner
            ServerTestRunner.Initialize();
            
            Console.WriteLine("[Vystia Tests] Test suite initialized. Use [TestVystiaSystems to run tests.");
            Console.WriteLine("[Vystia Tests] Use [RunServerTests to run tests server-side without client.");
        }

        [Usage("[TestVystiaSystems [faction|religion|crafting|all]")]
        [Description("Runs automated tests on Vystia systems. Use 'all' or no argument for all tests.")]
        private static void TestVystiaSystems_OnCommand(CommandEventArgs e)
        {
            Mobile from = e.Mobile;
            string scope = e.Length > 0 ? e.Arguments[0].ToLower() : "all";

            List<TestResult> results = new List<TestResult>();
            DateTime startTime = DateTime.UtcNow;

            // Initialize logging
            TestLogging.Initialize();

            from.SendMessage(0x35, "=== Vystia Systems Test Suite ===");
            from.SendMessage("Running {0} tests...", scope);

            // Estimate total tests for logging
            int estimatedTotal = scope == "all" ? 54 : 
                                scope == "faction" ? 11 :
                                scope == "religion" ? 11 :
                                scope == "crafting" ? 13 :
                                scope == "reward" ? 9 :
                                scope == "time" ? 6 : 54;

            TestLogging.LogTestSuiteStart(scope, estimatedTotal);

            try
            {
                switch (scope)
                {
                    case "faction":
                        results.AddRange(VystiaSystemsTestSuite.RunFactionTests());
                        break;
                    case "religion":
                        results.AddRange(VystiaSystemsTestSuite.RunReligionTests());
                        break;
                    case "crafting":
                        results.AddRange(VystiaSystemsTestSuite.RunCraftingTests());
                        break;
                    case "reward":
                        results.AddRange(VystiaSystemsTestSuite.RunRewardTests());
                        break;
                    case "time":
                        results.AddRange(VystiaSystemsTestSuite.RunTimeTests());
                        break;
                    case "all":
                    default:
                        results.AddRange(VystiaSystemsTestSuite.RunAllTests());
                        break;
                }
            }
            catch (Exception ex)
            {
                string errorMsg = $"Test suite crashed: {ex.Message}";
                from.SendMessage(0x22, errorMsg);
                TestLogging.LogException("TestVystiaSystems_OnCommand", ex);
                TestLogging.LogTestSuiteEnd(scope, 0, 0, 0, DateTime.UtcNow - startTime);
                return;
            }

            // Calculate results
            int passed = results.Count(r => r.Passed);
            int failed = results.Count(r => !r.Passed);
            int total = results.Count;
            TimeSpan duration = DateTime.UtcNow - startTime;

            // Log results
            TestLogging.LogTestSuiteEnd(scope, passed, failed, total, duration);

            // Output summary
            from.SendMessage("");
            from.SendMessage(0x35, "=== Test Results ===");
            from.SendMessage(passed == total ? 0x55 : 0x22,
                "Passed: {0} | Failed: {1} | Total: {2}",
                passed, failed, total);

            // Output by category
            var categories = results.GroupBy(r => r.Category);
            foreach (var category in categories)
            {
                int catPassed = category.Count(r => r.Passed);
                int catTotal = category.Count();
                int hue = catPassed == catTotal ? 0x55 : 0x22;

                from.SendMessage("");
                from.SendMessage(hue, "--- {0} Tests ({1}/{2}) ---",
                    category.Key, catPassed, catTotal);

                // Log category summary
                TestLogging.LogCategorySummary(category.Key, catPassed, catTotal);

                foreach (var result in category)
                {
                    if (result.Passed)
                    {
                        from.SendMessage(0x55, "[PASS] {0}", result.Name);
                        TestLogging.LogTestResult(result.Name, result.Category, true);
                    }
                    else
                    {
                        from.SendMessage(0x22, "[FAIL] {0}: {1}", result.Name, result.ErrorMessage);
                        TestLogging.LogTestResult(result.Name, result.Category, false, result.ErrorMessage);
                    }
                }
            }

            // Final summary
            from.SendMessage("");
            if (failed == 0)
            {
                from.SendMessage(0x55, "All {0} tests passed!", total);
            }
            else
            {
                from.SendMessage(0x22, "{0} test(s) failed. Review errors above.", failed);
                from.SendMessage(0x22, "Check Logs/VystiaTests/ for detailed logs.");
            }
        }

        [Usage("[RunServerTests [scope] [setup]")]
        [Description("Runs tests server-side without requiring client connection. Scope: all|faction|religion|crafting|reward|time. Setup: all|crafting|potions|constructs|devotion|faction|integration")]
        private static void RunServerTests_OnCommand(CommandEventArgs e)
        {
            string scope = e.Length > 0 ? e.Arguments[0].ToLower() : "all";
            string setup = e.Length > 1 ? e.Arguments[1].ToLower() : "";

            Console.WriteLine("");
            Console.WriteLine("========================================");
            Console.WriteLine("SERVER-SIDE TEST EXECUTION");
            Console.WriteLine("Scope: {0}", scope);
            if (!string.IsNullOrEmpty(setup))
                Console.WriteLine("Setup: {0}", setup);
            Console.WriteLine("Time: {0}", DateTime.UtcNow);
            Console.WriteLine("========================================");
            Console.WriteLine("");

            // Initialize logging
            TestLogging.Initialize();

            // Run test setup if specified
            if (!string.IsNullOrEmpty(setup))
            {
                ServerTestRunner.RunTestSetup(setup);
            }

            // Run automated tests
            ServerTestRunner.RunAutomatedTests(scope);

            Console.WriteLine("");
            Console.WriteLine("========================================");
            Console.WriteLine("SERVER-SIDE TEST EXECUTION COMPLETED");
            Console.WriteLine("Check Logs/VystiaTests/ for detailed results");
            Console.WriteLine("========================================");
            Console.WriteLine("");

            // Also send message to mobile if available
            if (e.Mobile != null)
            {
                e.Mobile.SendMessage(0x35, "Server-side tests completed. Check console and Logs/VystiaTests/ for results.");
            }
        }
    }
}
