/*
 * Vystia Test Results Tracker
 *
 * Tracks manual test results and generates reports
 * Usage: [TestResults <testname> <pass/fail>
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Server;
using Server.Commands;
using Server.Mobiles;

namespace Server.Custom.VystiaClasses.Testing
{
    /// <summary>
    /// Tracks manual test results and generates reports
    /// </summary>
    public static class TestResultsTracker
    {
        private static readonly string TestResultsFile = Path.Combine(Core.BaseDirectory, "Saves", "Vystia", "TestResults.txt");
        private static readonly Dictionary<string, TestResultEntry> s_TestResults = new Dictionary<string, TestResultEntry>();

        /// <summary>
        /// Test result entry
        /// </summary>
        public class TestResultEntry
        {
            public string TestName { get; set; }
            public bool Passed { get; set; }
            public DateTime TestDate { get; set; }
            public string TesterName { get; set; }
            public string Notes { get; set; }
        }

        public static void Initialize()
        {
            CommandSystem.Register("TestResults", AccessLevel.GameMaster, TestResults_OnCommand);
            CommandSystem.Register("TR", AccessLevel.GameMaster, TestResults_OnCommand);
            CommandSystem.Register("TestReport", AccessLevel.GameMaster, TestReport_OnCommand);

            // Ensure directory exists
            string directory = Path.GetDirectoryName(TestResultsFile);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            // Load existing results
            LoadResults();
        }

        [Usage("[TestResults <testname> <pass/fail> [notes]")]
        [Description("Record a manual test result")]
        private static void TestResults_OnCommand(CommandEventArgs e)
        {
            Mobile from = e.Mobile;
            if (!(from is PlayerMobile pm))
            {
                from.SendMessage("This command can only be used by players.");
                return;
            }

            if (e.Length < 2)
            {
                from.SendMessage("Usage: [TestResults <testname> <pass/fail> [notes]");
                from.SendMessage("Example: [TestResults PotionCreation pass All potions work correctly");
                return;
            }

            string testName = e.Arguments[0];
            string result = e.Arguments[1].ToLower();
            string notes = e.Length > 2 ? string.Join(" ", e.Arguments.Skip(2)) : "";

            bool passed = result == "pass" || result == "p" || result == "true" || result == "1";

            RecordResult(testName, passed, pm.Name, notes);

            from.SendMessage(passed ? 0x55 : 0x22,
                "Test result recorded: {0} - {1}", testName, passed ? "PASS" : "FAIL");
        }

        [Usage("[TestReport [category]")]
        [Description("Generate a test results report")]
        private static void TestReport_OnCommand(CommandEventArgs e)
        {
            Mobile from = e.Mobile;
            string category = e.Length > 0 ? e.Arguments[0].ToLower() : "all";

            from.SendMessage(0x35, "=== Test Results Report ===");

            var results = s_TestResults.Values.ToList();

            if (category != "all")
            {
                results = results.Where(r => r.TestName.ToLower().Contains(category)).ToList();
            }

            if (results.Count == 0)
            {
                from.SendMessage("No test results found.");
                return;
            }

            int passed = results.Count(r => r.Passed);
            int failed = results.Count(r => !r.Passed);
            int total = results.Count;

            from.SendMessage("");
            from.SendMessage(passed == total ? 0x55 : 0x22,
                "Passed: {0} | Failed: {1} | Total: {2}", passed, failed, total);

            from.SendMessage("");
            from.SendMessage("Test Results:");
            foreach (var result in results.OrderBy(r => r.TestName))
            {
                int hue = result.Passed ? 0x55 : 0x22;
                from.SendMessage(hue, "[{0}] {1} - {2} ({3})",
                    result.Passed ? "PASS" : "FAIL",
                    result.TestName,
                    result.TesterName,
                    result.TestDate.ToString("yyyy-MM-dd HH:mm"));

                if (!string.IsNullOrEmpty(result.Notes))
                {
                    from.SendMessage("  Notes: {0}", result.Notes);
                }
            }

            // Save report to file
            SaveReport(category);
            from.SendMessage("");
            from.SendMessage("Report saved to: {0}", TestResultsFile);
        }

        /// <summary>
        /// Record a test result
        /// </summary>
        private static void RecordResult(string testName, bool passed, string testerName, string notes)
        {
            var entry = new TestResultEntry
            {
                TestName = testName,
                Passed = passed,
                TestDate = DateTime.UtcNow,
                TesterName = testerName,
                Notes = notes
            };

            s_TestResults[testName] = entry;
            SaveResults();
        }

        /// <summary>
        /// Load test results from file
        /// </summary>
        private static void LoadResults()
        {
            if (!File.Exists(TestResultsFile))
                return;

            try
            {
                using (var reader = new StreamReader(TestResultsFile))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        if (string.IsNullOrWhiteSpace(line) || line.StartsWith("#"))
                            continue;

                        var parts = line.Split('|');
                        if (parts.Length >= 4)
                        {
                            var entry = new TestResultEntry
                            {
                                TestName = parts[0],
                                Passed = bool.Parse(parts[1]),
                                TestDate = DateTime.Parse(parts[2]),
                                TesterName = parts[3],
                                Notes = parts.Length > 4 ? parts[4] : ""
                            };

                            s_TestResults[entry.TestName] = entry;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error loading test results: {0}", ex.Message);
            }
        }

        /// <summary>
        /// Save test results to file
        /// </summary>
        private static void SaveResults()
        {
            try
            {
                using (var writer = new StreamWriter(TestResultsFile, false))
                {
                    writer.WriteLine("# Vystia Test Results");
                    writer.WriteLine("# Format: TestName|Passed|Date|Tester|Notes");
                    writer.WriteLine();

                    foreach (var entry in s_TestResults.Values.OrderBy(e => e.TestName))
                    {
                        writer.WriteLine("{0}|{1}|{2}|{3}|{4}",
                            entry.TestName,
                            entry.Passed,
                            entry.TestDate.ToString("yyyy-MM-dd HH:mm:ss"),
                            entry.TesterName,
                            entry.Notes ?? "");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error saving test results: {0}", ex.Message);
            }
        }

        /// <summary>
        /// Save a formatted report to file
        /// </summary>
        private static void SaveReport(string category)
        {
            string reportFile = TestResultsFile.Replace(".txt", "_Report.txt");
            try
            {
                using (var writer = new StreamWriter(reportFile, false))
                {
                    writer.WriteLine("Vystia Test Results Report");
                    writer.WriteLine("Generated: {0}", DateTime.UtcNow);
                    writer.WriteLine("Category: {0}", category);
                    writer.WriteLine();

                    var results = s_TestResults.Values.ToList();
                    if (category != "all")
                    {
                        results = results.Where(r => r.TestName.ToLower().Contains(category)).ToList();
                    }

                    int passed = results.Count(r => r.Passed);
                    int failed = results.Count(r => !r.Passed);
                    int total = results.Count;

                    writer.WriteLine("Summary: {0} Passed, {1} Failed, {2} Total", passed, failed, total);
                    writer.WriteLine();

                    foreach (var result in results.OrderBy(r => r.TestName))
                    {
                        writer.WriteLine("[{0}] {1}", result.Passed ? "PASS" : "FAIL", result.TestName);
                        writer.WriteLine("  Tester: {0}", result.TesterName);
                        writer.WriteLine("  Date: {0}", result.TestDate);
                        if (!string.IsNullOrEmpty(result.Notes))
                        {
                            writer.WriteLine("  Notes: {0}", result.Notes);
                        }
                        writer.WriteLine();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error saving report: {0}", ex.Message);
            }
        }
    }
}
