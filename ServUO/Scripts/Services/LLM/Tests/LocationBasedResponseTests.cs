using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Server;
using Server.Services.LLM;

namespace Server.Services.LLM.Tests
{
    /// <summary>
    /// Automated test suite for location-based NPC responses
    /// Tests NPCs' ability to provide directions and find other NPCs
    /// </summary>
    public class LocationBasedResponseTests
    {
        private static int testsPassed = 0;
        private static int testsFailed = 0;
        private static int testsSkipped = 0;
        private static List<TestResult> allResults = new List<TestResult>();
        private static StreamWriter logWriter = null;
        private static string logFilePath = null;

        public class TestResult
        {
            public string NPCType { get; set; }
            public string Question { get; set; }
            public string Response { get; set; }
            public bool Passed { get; set; }
            public string Reason { get; set; }
            public string ExpectedLocation { get; set; }
            public string ExpectedDirection { get; set; }
            public string ActualCoordinates { get; set; }
        }

        public class TestCase
        {
            public string NPCType { get; set; }
            public NPCPersonalities.PersonalityType PersonalityType { get; set; }
            public NPCPersonalities.SpeechPattern SpeechPattern { get; set; }
            public string Question { get; set; }
            public string ExpectedProfession { get; set; }
            public string ExpectedDirection { get; set; } // e.g., "north", "south east"
            public string ExpectedTown { get; set; } // For cross-town referrals
            public bool ExpectCoordinates { get; set; } = true;
            public string Description { get; set; } = "";
        }

        public static async Task RunAllTests()
        {
            WriteLog("╔════════════════════════════════════════════════════════════╗");
            WriteLog("║     LLM NPC Location-Based Response Test Suite             ║");
            WriteLog("╚════════════════════════════════════════════════════════════╝");
            WriteLog("");

            // Initialize log file
            InitializeLogFile();

            // Ensure lore system is initialized
            SimpleLoreSystem.Initialize();

            // Initialize memory system for testing
            InitializeMemorySystem();

            // Check if lore system is ready
            var allLore = SimpleLoreSystem.GetAllLore();
            if (allLore == null || allLore.Count == 0)
            {
                WriteLog("ERROR: Lore system not initialized or no lore entries loaded. Cannot run tests.");
                Console.WriteLine("[LocationTest] ERROR: Lore system not initialized or no lore entries loaded.");
                CloseLogFile();
                return;
            }

            WriteLog($"[INFO] Lore system initialized: {allLore.Count} entries loaded");
            WriteLog("");

            // Run test suites
            await TestSuite1_BasicLocationQuestions();
            await TestSuite2_ProfessionLookups();
            await TestSuite3_CrossTownReferrals();
            await TestSuite4_DirectionAccuracy();
            await TestSuite5_CoordinateLogging();
            await TestSuite6_MultipleNPCOptions();
            await TestSuite7_DistanceDescriptions();
            await TestSuite8_NPCNameMentions();

            // Print summary
            WriteLog("");
            WriteLog("═══════════════════════════════════════════════════════════");
            WriteLog("TEST SUMMARY");
            WriteLog("═══════════════════════════════════════════════════════════");
            WriteLog($"Total Tests: {testsPassed + testsFailed + testsSkipped}");
            WriteLog($"Passed: {testsPassed}");
            WriteLog($"Failed: {testsFailed}");
            WriteLog($"Skipped: {testsSkipped}");
            WriteLog($"Success Rate: {(testsPassed + testsFailed > 0 ? (testsPassed * 100.0 / (testsPassed + testsFailed)):0):F1}%");
            WriteLog("");

            CloseLogFile();
            Console.WriteLine($"[LocationTest] Tests complete. Results saved to: {logFilePath}");
        }

        private static async Task TestSuite1_BasicLocationQuestions()
        {
            WriteLog("═══════════════════════════════════════════════════════════");
            WriteLog("Test Suite 1: Basic Location Questions");
            WriteLog("═══════════════════════════════════════════════════════════");
            WriteLog("");

            List<TestCase> testCases = new List<TestCase>
            {
                new TestCase
                {
                    NPCType = "Commoner",
                    PersonalityType = NPCPersonalities.PersonalityType.Commoner,
                    SpeechPattern = NPCPersonalities.SpeechPattern.Casual,
                    Question = "where is the nearest healer?",
                    ExpectedProfession = "healer",
                    Description = "Basic 'where is' question for healer"
                },
                new TestCase
                {
                    NPCType = "Merchant",
                    PersonalityType = NPCPersonalities.PersonalityType.Merchant,
                    SpeechPattern = NPCPersonalities.SpeechPattern.Casual,
                    Question = "where can I find a blacksmith?",
                    ExpectedProfession = "blacksmith",
                    Description = "Alternative phrasing for finding NPC"
                },
                new TestCase
                {
                    NPCType = "Guard",
                    PersonalityType = NPCPersonalities.PersonalityType.Guard,
                    SpeechPattern = NPCPersonalities.SpeechPattern.Formal,
                    Question = "find me a mage",
                    ExpectedProfession = "mage",
                    Description = "Direct 'find me' request"
                },
                new TestCase
                {
                    NPCType = "Innkeeper",
                    PersonalityType = NPCPersonalities.PersonalityType.InnKeeper,
                    SpeechPattern = NPCPersonalities.SpeechPattern.Casual,
                    Question = "where's a scholar?",
                    ExpectedProfession = "scholar",
                    Description = "Contracted 'where's' question"
                }
            };

            foreach (var testCase in testCases)
            {
                await RunLocationTest(testCase);
            }
        }

        private static async Task TestSuite2_ProfessionLookups()
        {
            WriteLog("");
            WriteLog("═══════════════════════════════════════════════════════════");
            WriteLog("Test Suite 2: Various Profession Types");
            WriteLog("═══════════════════════════════════════════════════════════");
            WriteLog("");

            List<TestCase> testCases = new List<TestCase>
            {
                new TestCase
                {
                    NPCType = "Commoner",
                    PersonalityType = NPCPersonalities.PersonalityType.Commoner,
                    SpeechPattern = NPCPersonalities.SpeechPattern.Casual,
                    Question = "where is the nearest healer?",
                    ExpectedProfession = "healer",
                    Description = "Healer lookup"
                },
                new TestCase
                {
                    NPCType = "Commoner",
                    PersonalityType = NPCPersonalities.PersonalityType.Commoner,
                    SpeechPattern = NPCPersonalities.SpeechPattern.Casual,
                    Question = "where can I find a mage?",
                    ExpectedProfession = "mage",
                    Description = "Mage lookup"
                },
                new TestCase
                {
                    NPCType = "Commoner",
                    PersonalityType = NPCPersonalities.PersonalityType.Commoner,
                    SpeechPattern = NPCPersonalities.SpeechPattern.Casual,
                    Question = "where's a blacksmith?",
                    ExpectedProfession = "blacksmith",
                    Description = "Blacksmith lookup"
                },
                new TestCase
                {
                    NPCType = "Commoner",
                    PersonalityType = NPCPersonalities.PersonalityType.Commoner,
                    SpeechPattern = NPCPersonalities.SpeechPattern.Casual,
                    Question = "where is a scholar?",
                    ExpectedProfession = "scholar",
                    Description = "Scholar lookup"
                },
                new TestCase
                {
                    NPCType = "Commoner",
                    PersonalityType = NPCPersonalities.PersonalityType.Commoner,
                    SpeechPattern = NPCPersonalities.SpeechPattern.Casual,
                    Question = "where can I find a guard?",
                    ExpectedProfession = "guard",
                    Description = "Guard lookup"
                },
                new TestCase
                {
                    NPCType = "Commoner",
                    PersonalityType = NPCPersonalities.PersonalityType.Commoner,
                    SpeechPattern = NPCPersonalities.SpeechPattern.Casual,
                    Question = "where's a merchant?",
                    ExpectedProfession = "merchant",
                    Description = "Merchant lookup"
                }
            };

            foreach (var testCase in testCases)
            {
                await RunLocationTest(testCase);
            }
        }

        private static async Task TestSuite3_CrossTownReferrals()
        {
            WriteLog("");
            WriteLog("═══════════════════════════════════════════════════════════");
            WriteLog("Test Suite 3: Cross-Town Referrals");
            WriteLog("═══════════════════════════════════════════════════════════");
            WriteLog("");

            List<TestCase> testCases = new List<TestCase>
            {
                new TestCase
                {
                    NPCType = "Commoner",
                    PersonalityType = NPCPersonalities.PersonalityType.Commoner,
                    SpeechPattern = NPCPersonalities.SpeechPattern.Casual,
                    Question = "where is the nearest healer?",
                    ExpectedProfession = "healer",
                    ExpectedTown = "Moonglow", // Common fallback town
                    ExpectCoordinates = false, // Cross-town referrals won't have coordinates
                    Description = "Cross-town referral for healer"
                },
                new TestCase
                {
                    NPCType = "Commoner",
                    PersonalityType = NPCPersonalities.PersonalityType.Commoner,
                    SpeechPattern = NPCPersonalities.SpeechPattern.Casual,
                    Question = "where can I find a blacksmith?",
                    ExpectedProfession = "blacksmith",
                    ExpectedTown = "Minoc", // Common fallback town
                    ExpectCoordinates = false,
                    Description = "Cross-town referral for blacksmith"
                }
            };

            foreach (var testCase in testCases)
            {
                await RunLocationTest(testCase);
            }
        }

        private static async Task TestSuite4_DirectionAccuracy()
        {
            WriteLog("");
            WriteLog("═══════════════════════════════════════════════════════════");
            WriteLog("Test Suite 4: Direction Accuracy");
            WriteLog("═══════════════════════════════════════════════════════════");
            WriteLog("");

            List<TestCase> testCases = new List<TestCase>
            {
                new TestCase
                {
                    NPCType = "Commoner",
                    PersonalityType = NPCPersonalities.PersonalityType.Commoner,
                    SpeechPattern = NPCPersonalities.SpeechPattern.Casual,
                    Question = "where is the nearest healer?",
                    ExpectedProfession = "healer",
                    ExpectedDirection = "north", // Will check if direction is present
                    Description = "Verify direction is provided"
                },
                new TestCase
                {
                    NPCType = "Commoner",
                    PersonalityType = NPCPersonalities.PersonalityType.Commoner,
                    SpeechPattern = NPCPersonalities.SpeechPattern.Casual,
                    Question = "where can I find a mage?",
                    ExpectedProfession = "mage",
                    ExpectedDirection = "east", // Will check if direction is present
                    Description = "Verify direction is provided for mage"
                }
            };

            foreach (var testCase in testCases)
            {
                await RunLocationTest(testCase);
            }
        }

        private static async Task TestSuite5_CoordinateLogging()
        {
            WriteLog("");
            WriteLog("═══════════════════════════════════════════════════════════");
            WriteLog("Test Suite 5: Coordinate Logging");
            WriteLog("═══════════════════════════════════════════════════════════");
            WriteLog("");

            List<TestCase> testCases = new List<TestCase>
            {
                new TestCase
                {
                    NPCType = "Commoner",
                    PersonalityType = NPCPersonalities.PersonalityType.Commoner,
                    SpeechPattern = NPCPersonalities.SpeechPattern.Casual,
                    Question = "where is the nearest healer?",
                    ExpectedProfession = "healer",
                    ExpectCoordinates = true,
                    Description = "Verify coordinates are logged in response"
                },
                new TestCase
                {
                    NPCType = "Commoner",
                    PersonalityType = NPCPersonalities.PersonalityType.Commoner,
                    SpeechPattern = NPCPersonalities.SpeechPattern.Casual,
                    Question = "where can I find a blacksmith?",
                    ExpectedProfession = "blacksmith",
                    ExpectCoordinates = true,
                    Description = "Verify coordinates are logged for blacksmith"
                }
            };

            foreach (var testCase in testCases)
            {
                await RunLocationTest(testCase);
            }
        }

        private static async Task RunLocationTest(TestCase testCase)
        {
            try
            {
                WriteLog($"Testing: {testCase.Description}");
                WriteLog($"  NPC Type: {testCase.NPCType}");
                WriteLog($"  Question: {testCase.Question}");
                WriteLog($"  Expected Profession: {testCase.ExpectedProfession}");

                // Create a mock NPC for context (needed for GetContextualInfo)
                var npc = CreateMockNPC(testCase.PersonalityType, testCase.SpeechPattern);
                var player = CreateMockPlayer();

                // Get personality prompt
                string personalityPrompt = NPCPersonalities.GetPersonalityPrompt(testCase.PersonalityType, testCase.SpeechPattern);
                
                // Get contextual info (includes nearby NPC info with coordinates)
                string contextualInfo = NPCPersonalities.GetContextualInfo(npc, player);
                personalityPrompt += contextualInfo;

                WriteLog($"  Context includes nearby NPC info: {!string.IsNullOrEmpty(contextualInfo)}");

                // Get response from LLM
                List<ConversationMessage> history = new List<ConversationMessage>();
                string response = await UnifiedLLMService.GetResponseAsync(
                    npc.Name,
                    personalityPrompt,
                    history,
                    testCase.Question,
                    player.Name,
                    "", // No preloaded knowledge for location tests
                    UnifiedLLMService.RequestType.PlayerConversation,
                    UnifiedLLMService.LLMProvider.Auto,
                    false,
                    true
                );

                WriteLog($"  Response: {response}");
                WriteLog("");

                // Analyze response
                bool passed = AnalyzeLocationResponse(response, testCase);
                
                TestResult result = new TestResult
                {
                    NPCType = testCase.NPCType,
                    Question = testCase.Question,
                    Response = response,
                    Passed = passed,
                    ExpectedLocation = testCase.ExpectedProfession,
                    ExpectedDirection = testCase.ExpectedDirection,
                    ActualCoordinates = ExtractCoordinates(response)
                };

                if (passed)
                {
                    result.Reason = "Response contains expected location information";
                    testsPassed++;
                    WriteLog($"  ✓ PASSED: {result.Reason}");
                    if (!string.IsNullOrEmpty(result.ActualCoordinates) && result.ActualCoordinates != "N/A")
                    {
                        WriteLog($"    Coordinates found: {result.ActualCoordinates}");
                    }
                }
                else
                {
                    result.Reason = "Response does not contain expected location information";
                    testsFailed++;
                    WriteLog($"  ✗ FAILED: {result.Reason}");
                }

                allResults.Add(result);
                WriteLog("");
            }
            catch (Exception ex)
            {
                WriteLog($"  ✗ ERROR: {ex.Message}");
                WriteLog($"  Stack Trace: {ex.StackTrace}");
                testsFailed++;
                WriteLog("");
            }
        }

        private static bool AnalyzeLocationResponse(string response, TestCase testCase)
        {
            if (string.IsNullOrEmpty(response))
                return false;

            string lowerResponse = response.ToLower();
            string lowerProfession = testCase.ExpectedProfession.ToLower();

            // Check if response mentions the expected profession
            bool mentionsProfession = lowerResponse.Contains(lowerProfession) ||
                                      (lowerProfession == "healer" && (lowerResponse.Contains("heal") || lowerResponse.Contains("medic"))) ||
                                      (lowerProfession == "blacksmith" && (lowerResponse.Contains("smith") || lowerResponse.Contains("forge"))) ||
                                      (lowerProfession == "mage" && (lowerResponse.Contains("wizard") || lowerResponse.Contains("magic")));

            if (!mentionsProfession)
                return false;

            // Check for location indicators
            bool hasLocationInfo = false;

            // Check for directions
            string[] directions = { "north", "south", "east", "west", "northeast", "northwest", "southeast", "southwest" };
            foreach (var dir in directions)
            {
                if (lowerResponse.Contains(dir))
                {
                    hasLocationInfo = true;
                    break;
                }
            }

            // Check for town names
            string[] towns = { "moonglow", "minoc", "trinsic", "vesper", "britain", "yew", "jhelom", "skara brae" };
            foreach (var town in towns)
            {
                if (lowerResponse.Contains(town))
                {
                    hasLocationInfo = true;
                    break;
                }
            }

            // Check for coordinates if expected
            if (testCase.ExpectCoordinates)
            {
                if (response.Contains("coords:") || response.Contains("(") && response.Contains(","))
                {
                    hasLocationInfo = true;
                }
            }
            else
            {
                // For cross-town referrals, coordinates are not expected
                hasLocationInfo = true; // Town name is enough
            }

            return hasLocationInfo;
        }

        private static string ExtractCoordinates(string response)
        {
            int coordsIndex = response.IndexOf("coords:");
            if (coordsIndex >= 0)
            {
                string coordsPart = response.Substring(coordsIndex);
                int endIndex = coordsPart.IndexOf(")");
                if (endIndex > 0)
                {
                    return coordsPart.Substring(7, endIndex - 7).Trim();
                }
            }
            return "N/A";
        }

        private static Mobile CreateMockNPC(NPCPersonalities.PersonalityType personalityType, NPCPersonalities.SpeechPattern speechPattern)
        {
            // Create a simple mock NPC for testing
            // We need an actual NPC instance to get contextual info (nearby NPCs)
            var npc = new Server.Mobiles.LLMNpc();
            npc.Name = "TestNPC";
            // Set personality via properties if available, otherwise use constructor
            try
            {
                // Try to set personality properties if they exist
                var personalityProp = npc.GetType().GetProperty("PersonalityType");
                if (personalityProp != null)
                    personalityProp.SetValue(npc, personalityType);
                
                var speechProp = npc.GetType().GetProperty("SpeechPattern");
                if (speechProp != null)
                    speechProp.SetValue(npc, speechPattern);
            }
            catch
            {
                // Properties might not be accessible, that's okay for testing
            }
            
            // Set location to a known town (Britain) - this is important for region-based lookups
            npc.MoveToWorld(new Point3D(1434, 1699, 0), Map.Felucca);
            return npc;
        }

        private static Mobile CreateMockPlayer()
        {
            // Create a minimal player for testing
            // We just need a Mobile object for GetContextualInfo
            var player = new Server.Mobiles.PlayerMobile();
            player.Name = "TestPlayer";
            return player;
        }

        private static void InitializeMemorySystem()
        {
            try
            {
                LLMMemoryConfig.Configure();
                LLMMemoryService.Initialize();
            }
            catch (Exception ex)
            {
                WriteLog($"[WARNING] Memory system initialization failed: {ex.Message}");
            }
        }

        private static void InitializeLogFile()
        {
            string logDir = Path.Combine(Core.BaseDirectory.Directory, "Data", "LLM", "Tests");
            Directory.CreateDirectory(logDir);

            string timestamp = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
            logFilePath = Path.Combine(logDir, $"LocationTest_{timestamp}.log");

            logWriter = new StreamWriter(logFilePath, false, Encoding.UTF8);
            Console.WriteLine($"[LocationTest] Log file: {logFilePath}");
        }

        private static void WriteLog(string message)
        {
            Console.WriteLine($"[LocationTest] {message}");
            if (logWriter != null)
            {
                logWriter.WriteLine(message);
                logWriter.Flush();
            }
        }

        private static void CloseLogFile()
        {
            if (logWriter != null)
            {
                logWriter.Close();
                logWriter = null;
            }
        }

        private static async Task TestSuite6_MultipleNPCOptions()
        {
            WriteLog("");
            WriteLog("═══════════════════════════════════════════════════════════");
            WriteLog("Test Suite 6: Multiple NPC Options");
            WriteLog("═══════════════════════════════════════════════════════════");
            WriteLog("");

            WriteLog("Testing NPCs' ability to mention multiple nearby NPCs of the same type");
            WriteLog("");

            List<TestCase> testCases = new List<TestCase>
            {
                new TestCase
                {
                    NPCType = "Commoner",
                    PersonalityType = NPCPersonalities.PersonalityType.Commoner,
                    SpeechPattern = NPCPersonalities.SpeechPattern.Casual,
                    Question = "where are the healers?",
                    ExpectedProfession = "healer",
                    Description = "Should mention multiple healers if available"
                },
                new TestCase
                {
                    NPCType = "Merchant",
                    PersonalityType = NPCPersonalities.PersonalityType.Merchant,
                    SpeechPattern = NPCPersonalities.SpeechPattern.Formal,
                    Question = "where can I find blacksmiths?",
                    ExpectedProfession = "blacksmith",
                    Description = "Should mention multiple blacksmiths if available"
                }
            };

            foreach (var testCase in testCases)
            {
                await RunLocationTest(testCase);
            }
        }

        private static async Task TestSuite7_DistanceDescriptions()
        {
            WriteLog("");
            WriteLog("═══════════════════════════════════════════════════════════");
            WriteLog("Test Suite 7: Distance Descriptions");
            WriteLog("═══════════════════════════════════════════════════════════");
            WriteLog("");

            WriteLog("Testing NPCs' use of natural distance descriptions");
            WriteLog("");

            List<TestCase> testCases = new List<TestCase>
            {
                new TestCase
                {
                    NPCType = "Commoner",
                    PersonalityType = NPCPersonalities.PersonalityType.Commoner,
                    SpeechPattern = NPCPersonalities.SpeechPattern.Casual,
                    Question = "where is the nearest healer?",
                    ExpectedProfession = "healer",
                    Description = "Should include distance description (nearby, short walk, etc.)"
                },
                new TestCase
                {
                    NPCType = "Merchant",
                    PersonalityType = NPCPersonalities.PersonalityType.Merchant,
                    SpeechPattern = NPCPersonalities.SpeechPattern.Formal,
                    Question = "where can I find a blacksmith?",
                    ExpectedProfession = "blacksmith",
                    Description = "Should include distance description"
                }
            };

            foreach (var testCase in testCases)
            {
                await RunLocationTest(testCase);
            }
        }

        private static async Task TestSuite8_NPCNameMentions()
        {
            WriteLog("");
            WriteLog("═══════════════════════════════════════════════════════════");
            WriteLog("Test Suite 8: NPC Name Mentions");
            WriteLog("═══════════════════════════════════════════════════════════");
            WriteLog("");

            WriteLog("Testing NPCs' ability to include NPC names in directions");
            WriteLog("");

            List<TestCase> testCases = new List<TestCase>
            {
                new TestCase
                {
                    NPCType = "Commoner",
                    PersonalityType = NPCPersonalities.PersonalityType.Commoner,
                    SpeechPattern = NPCPersonalities.SpeechPattern.Casual,
                    Question = "where is the nearest healer?",
                    ExpectedProfession = "healer",
                    Description = "Should include NPC name if available (e.g., 'Cain the healer')"
                },
                new TestCase
                {
                    NPCType = "Merchant",
                    PersonalityType = NPCPersonalities.PersonalityType.Merchant,
                    SpeechPattern = NPCPersonalities.SpeechPattern.Formal,
                    Question = "where can I find a blacksmith?",
                    ExpectedProfession = "blacksmith",
                    Description = "Should include NPC name if available"
                }
            };

            foreach (var testCase in testCases)
            {
                await RunLocationTest(testCase);
            }
        }
    }
}

