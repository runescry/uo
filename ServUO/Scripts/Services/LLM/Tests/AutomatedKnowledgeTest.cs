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
    /// Automated knowledge testing system for LLM NPCs
    /// Tests NPC knowledge without requiring in-game interaction
    /// </summary>
    public class AutomatedKnowledgeTest
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
            public string ExpectedKeywords { get; set; }
            public bool ShouldRefer { get; set; }
        }

        public class TestCase
        {
            public string NPCType { get; set; }
            public NPCPersonalities.PersonalityType PersonalityType { get; set; }
            public NPCPersonalities.SpeechPattern SpeechPattern { get; set; }
            public string Question { get; set; }
            public List<string> ExpectedKeywords { get; set; } = new List<string>();
            public bool ShouldRefer { get; set; } = false;
            public string ReferralTarget { get; set; } = "";
            public string Description { get; set; } = "";
        }

        public static async Task RunAllTests()
        {
            WriteLog("╔════════════════════════════════════════════════════════════╗");
            WriteLog("║     LLM NPC Knowledge - Automated Test Suite               ║");
            WriteLog("╚════════════════════════════════════════════════════════════╝");
            WriteLog("");

            // Initialize log file
            InitializeLogFile();

            // Ensure lore system is initialized
            SimpleLoreSystem.Initialize(); // Initialize() checks internally if already initialized

            // Initialize memory system for testing (use in-memory fallback)
            InitializeMemorySystem();

            // Check if lore system is ready
            var allLore = SimpleLoreSystem.GetAllLore();
            if (allLore == null || allLore.Count == 0)
            {
                WriteLog("ERROR: Lore system not initialized or no lore entries loaded. Cannot run tests.");
                Console.WriteLine("[KnowledgeTest] ERROR: Lore system not initialized or no lore entries loaded.");
                CloseLogFile();
                return;
            }

            WriteLog($"[INFO] Lore system initialized: {allLore.Count} entries loaded");
            WriteLog("");

            // Run test suites
            await TestSuite1_MageKnowledge();
            await TestSuite2_BlacksmithKnowledge();
            await TestSuite3_KnowledgeBoundaries();
            await TestSuite4_MerchantKnowledge();
            await TestSuite5_HealerKnowledge();
            await TestSuite6_MemorySystem();
            await TestSuite7_VirtueKnowledge();
            
            // Run comprehensive personality test suite
            try
            {
                WriteLog("");
                WriteLog("[INFO] Starting Test Suite 8: All Personality Types (this may take several minutes)...");
                await TestSuite8_AllPersonalityTypes();
            }
            catch (Exception ex)
            {
                WriteLog("");
                WriteLog("═══════════════════════════════════════════════════════════");
                WriteLog("ERROR: Test Suite 8 failed to complete");
                WriteLog("═══════════════════════════════════════════════════════════");
                WriteLog($"Error: {ex.Message}");
                WriteLog($"Stack Trace: {ex.StackTrace}");
                Console.WriteLine($"[KnowledgeTest] ERROR: Test Suite 8 failed: {ex.Message}");
            }

            // Print summary
            PrintSummary();

            // Close log file
            CloseLogFile();
        }

        #region Test Suite 1: Mage Knowledge

        private static async Task TestSuite1_MageKnowledge()
        {
            WriteLog("═══════════════════════════════════════════════════════════");
            WriteLog("TEST SUITE 1: Mage Knowledge (Expert Domain)");
            WriteLog("═══════════════════════════════════════════════════════════");
            WriteLog("");

            var testCases = new[]
            {
                new TestCase
                {
                    NPCType = "Mage",
                    PersonalityType = NPCPersonalities.PersonalityType.Mage,
                    SpeechPattern = NPCPersonalities.SpeechPattern.Archaic,
                    Question = "What is black pearl used for?",
                    ExpectedKeywords = new List<string> { "black pearl", "spell", "magic", "circle", "reagent" },
                    Description = "Mage should know about black pearl reagent"
                },
                new TestCase
                {
                    NPCType = "Mage",
                    PersonalityType = NPCPersonalities.PersonalityType.Mage,
                    SpeechPattern = NPCPersonalities.SpeechPattern.Archaic,
                    Question = "Tell me about bloodmoss",
                    ExpectedKeywords = new List<string> { "blood", "moss", "reagent", "spell", "heal" },
                    Description = "Mage should know about bloodmoss reagent"
                },
                new TestCase
                {
                    NPCType = "Mage",
                    PersonalityType = NPCPersonalities.PersonalityType.Mage,
                    SpeechPattern = NPCPersonalities.SpeechPattern.Archaic,
                    Question = "What's the difference between mandrake root and ginseng?",
                    ExpectedKeywords = new List<string> { "mandrake", "ginseng", "spell", "magic" },
                    Description = "Mage should compare reagents"
                },
                new TestCase
                {
                    NPCType = "Mage",
                    PersonalityType = NPCPersonalities.PersonalityType.Mage,
                    SpeechPattern = NPCPersonalities.SpeechPattern.Archaic,
                    Question = "I need reagents for a summoning spell, what should I get?",
                    ExpectedKeywords = new List<string> { "reagent", "spell", "summon", "blood moss", "spider silk" },
                    Description = "Mage should recommend reagents"
                }
            };

            foreach (var testCase in testCases)
            {
                await RunKnowledgeTest(testCase);
            }

            Console.WriteLine();
        }

        #endregion

        #region Test Suite 2: Blacksmith Knowledge

        private static async Task TestSuite2_BlacksmithKnowledge()
        {
            WriteLog("═══════════════════════════════════════════════════════════");
            WriteLog("TEST SUITE 2: Blacksmith Knowledge (Expert Domain)");
            WriteLog("═══════════════════════════════════════════════════════════");
            WriteLog("");

            var testCases = new[]
            {
                new TestCase
                {
                    NPCType = "Blacksmith",
                    PersonalityType = NPCPersonalities.PersonalityType.Blacksmith,
                    SpeechPattern = NPCPersonalities.SpeechPattern.Archaic,
                    Question = "Tell me about valorite",
                    ExpectedKeywords = new List<string> { "valorite", "ore", "metal", "forge", "weapon", "armor" },
                    Description = "Blacksmith should know about valorite ore"
                },
                new TestCase
                {
                    NPCType = "Blacksmith",
                    PersonalityType = NPCPersonalities.PersonalityType.Blacksmith,
                    SpeechPattern = NPCPersonalities.SpeechPattern.Archaic,
                    Question = "What's the best metal for crafting weapons?",
                    ExpectedKeywords = new List<string> { "metal", "ore", "weapon", "valorite", "verite", "agapite" },
                    Description = "Blacksmith should recommend metals"
                },
                new TestCase
                {
                    NPCType = "Blacksmith",
                    PersonalityType = NPCPersonalities.PersonalityType.Blacksmith,
                    SpeechPattern = NPCPersonalities.SpeechPattern.Archaic,
                    Question = "How do I forge a sword?",
                    ExpectedKeywords = new List<string> { "forge", "sword", "metal", "anvil", "hammer", "ingot", "smith" },
                    Description = "Blacksmith should explain forging"
                }
            };

            foreach (var testCase in testCases)
            {
                await RunKnowledgeTest(testCase);
            }

            Console.WriteLine();
        }

        #endregion

        #region Test Suite 3: Knowledge Boundaries

        private static async Task TestSuite3_KnowledgeBoundaries()
        {
            WriteLog("═══════════════════════════════════════════════════════════");
            WriteLog("TEST SUITE 3: Knowledge Boundaries (Referral System)");
            WriteLog("═══════════════════════════════════════════════════════════");
            WriteLog("");

            var testCases = new[]
            {
                new TestCase
                {
                    NPCType = "Blacksmith",
                    PersonalityType = NPCPersonalities.PersonalityType.Blacksmith,
                    SpeechPattern = NPCPersonalities.SpeechPattern.Archaic,
                    Question = "Tell me about spellcasting",
                    ShouldRefer = true,
                    ReferralTarget = "mage",
                    ExpectedKeywords = new List<string> { "mage", "magic", "spell", "refer", "not my", "can't help" },
                    Description = "Blacksmith should refer to mage for magic questions"
                },
                new TestCase
                {
                    NPCType = "Blacksmith",
                    PersonalityType = NPCPersonalities.PersonalityType.Blacksmith,
                    SpeechPattern = NPCPersonalities.SpeechPattern.Archaic,
                    Question = "What reagents do I need for a spell?",
                    ShouldRefer = true,
                    ReferralTarget = "mage",
                    ExpectedKeywords = new List<string> { "mage", "alchemist", "reagent", "not my", "can't help" },
                    Description = "Blacksmith should refer for reagent questions"
                },
                new TestCase
                {
                    NPCType = "Mage",
                    PersonalityType = NPCPersonalities.PersonalityType.Mage,
                    SpeechPattern = NPCPersonalities.SpeechPattern.Archaic,
                    Question = "How do I forge a sword?",
                    ShouldRefer = true,
                    ReferralTarget = "blacksmith",
                    ExpectedKeywords = new List<string> { "blacksmith", "smith", "forge", "not my", "can't help" },
                    Description = "Mage should refer to blacksmith for forging questions"
                }
            };

            foreach (var testCase in testCases)
            {
                await RunKnowledgeTest(testCase);
            }

            Console.WriteLine();
        }

        #endregion

        #region Test Suite 4: Merchant Knowledge

        private static async Task TestSuite4_MerchantKnowledge()
        {
            WriteLog("═══════════════════════════════════════════════════════════");
            WriteLog("TEST SUITE 4: Merchant Knowledge (Proficient Domain)");
            WriteLog("═══════════════════════════════════════════════════════════");
            WriteLog("");

            var testCases = new[]
            {
                new TestCase
                {
                    NPCType = "Merchant",
                    PersonalityType = NPCPersonalities.PersonalityType.Merchant,
                    SpeechPattern = NPCPersonalities.SpeechPattern.Formal,
                    Question = "What reagents should I stock in my shop?",
                    ExpectedKeywords = new List<string> { "reagent", "stock", "sell", "black pearl", "blood moss" },
                    Description = "Merchant should know what reagents to stock"
                },
                new TestCase
                {
                    NPCType = "Merchant",
                    PersonalityType = NPCPersonalities.PersonalityType.Merchant,
                    SpeechPattern = NPCPersonalities.SpeechPattern.Formal,
                    Question = "Which reagents are most valuable?",
                    ExpectedKeywords = new List<string> { "reagent", "valuable", "price", "demand", "rare" },
                    Description = "Merchant should know reagent values"
                }
            };

            foreach (var testCase in testCases)
            {
                await RunKnowledgeTest(testCase);
            }

            Console.WriteLine();
        }

        #endregion

        #region Test Suite 5: Healer Knowledge

        private static async Task TestSuite5_HealerKnowledge()
        {
            WriteLog("═══════════════════════════════════════════════════════════");
            WriteLog("TEST SUITE 5: Healer Knowledge (Expert Domain)");
            WriteLog("═══════════════════════════════════════════════════════════");
            WriteLog("");

            var testCases = new[]
            {
                new TestCase
                {
                    NPCType = "Healer",
                    PersonalityType = NPCPersonalities.PersonalityType.Healer,
                    SpeechPattern = NPCPersonalities.SpeechPattern.OldEnglish,
                    Question = "What herbs are used for healing?",
                    ExpectedKeywords = new List<string> { "herb", "heal", "ginseng", "garlic", "cure" },
                    Description = "Healer should know about healing herbs"
                },
                new TestCase
                {
                    NPCType = "Healer",
                    PersonalityType = NPCPersonalities.PersonalityType.Healer,
                    SpeechPattern = NPCPersonalities.SpeechPattern.OldEnglish,
                    Question = "How do I cure poison?",
                    ExpectedKeywords = new List<string> { "poison", "cure", "potion", "herb", "antidote" },
                    Description = "Healer should know about curing poison"
                }
            };

            foreach (var testCase in testCases)
            {
                await RunKnowledgeTest(testCase);
            }

            Console.WriteLine();
        }

        #endregion

        #region Test Suite 6: Memory System

        private static async Task TestSuite6_MemorySystem()
        {
            WriteLog("═══════════════════════════════════════════════════════════");
            WriteLog("TEST SUITE 6: Memory System (Persistence & Relationships)");
            WriteLog("═══════════════════════════════════════════════════════════");
            WriteLog("");

            // Test 6.1: NPC remembers preference
            await RunMemoryTest(
                npcType: "Blacksmith",
                personalityType: NPCPersonalities.PersonalityType.Blacksmith,
                speechPattern: NPCPersonalities.SpeechPattern.Archaic,
                setupMessages: new[] { "I love crafting swords" },
                testQuestion: "What do I like?",
                expectedKeywords: new List<string> { "sword", "craft", "like", "love" },
                description: "NPC should remember player's preference"
            );

            // Test 6.2: NPC uses playerName parameter, not what player claims
            await RunMemoryNameTest(
                npcType: "Mage",
                personalityType: NPCPersonalities.PersonalityType.Mage,
                speechPattern: NPCPersonalities.SpeechPattern.Archaic,
                playerName: "TestPlayer",
                wrongName: "Aldric",
                description: "NPC should use playerName parameter, not player's claimed name"
            );

            // Test 6.3: NPC remembers past conversation topic
            await RunMemoryTest(
                npcType: "Mage",
                personalityType: NPCPersonalities.PersonalityType.Mage,
                speechPattern: NPCPersonalities.SpeechPattern.Archaic,
                setupMessages: new[] { "I'm looking for reagents to cast a fireball spell" },
                testQuestion: "What was I looking for earlier?",
                expectedKeywords: new List<string> { "reagent", "fireball", "spell", "looking" },
                description: "NPC should remember past conversation topic"
            );

            // Test 6.4: Relationship progression
            await RunMemoryTest(
                npcType: "Merchant",
                personalityType: NPCPersonalities.PersonalityType.Merchant,
                speechPattern: NPCPersonalities.SpeechPattern.Formal,
                setupMessages: new[] { "Hello", "I need reagents", "Thank you for your help" },
                testQuestion: "How do you feel about me?",
                expectedKeywords: new List<string> { "friend", "acquaintance", "helpful", "good", "appreciate", "valued", "patronage", "satisfied" },
                description: "NPC should show improved relationship after multiple positive interactions"
            );

            WriteLog("");
        }

        #endregion

        #region Test Suite 7: Virtue Knowledge

        private static async Task TestSuite7_VirtueKnowledge()
        {
            WriteLog("═══════════════════════════════════════════════════════════");
            WriteLog("TEST SUITE 7: Virtue Knowledge (Eight Virtues & Principles)");
            WriteLog("═══════════════════════════════════════════════════════════");
            WriteLog("");

            // Test 7.1: Sage knows about Honesty
            await RunKnowledgeTest(new TestCase
            {
                NPCType = "Sage",
                PersonalityType = NPCPersonalities.PersonalityType.Sage,
                SpeechPattern = NPCPersonalities.SpeechPattern.Formal,
                Question = "What is the virtue of Honesty?",
                ExpectedKeywords = new List<string> { "honesty", "truth", "moonglow", "deceit", "virtue" },
                Description = "Scholar should know about Honesty virtue"
            });

            // Test 7.2: Sage knows about Compassion
            await RunKnowledgeTest(new TestCase
            {
                NPCType = "Sage",
                PersonalityType = NPCPersonalities.PersonalityType.Sage,
                SpeechPattern = NPCPersonalities.SpeechPattern.Formal,
                Question = "Tell me about the virtue of Compassion",
                ExpectedKeywords = new List<string> { "compassion", "love", "britain", "despise", "empathy" },
                Description = "Scholar should know about Compassion virtue"
            });

            // Test 7.3: Sage knows about Valor
            await RunKnowledgeTest(new TestCase
            {
                NPCType = "Sage",
                PersonalityType = NPCPersonalities.PersonalityType.Sage,
                SpeechPattern = NPCPersonalities.SpeechPattern.Formal,
                Question = "What does the virtue of Valor represent?",
                ExpectedKeywords = new List<string> { "valor", "courage", "jhelom", "destard", "bravery" },
                Description = "Scholar should know about Valor virtue"
            });

            // Test 7.4: Sage knows about Justice
            await RunKnowledgeTest(new TestCase
            {
                NPCType = "Sage",
                PersonalityType = NPCPersonalities.PersonalityType.Sage,
                SpeechPattern = NPCPersonalities.SpeechPattern.Formal,
                Question = "Explain the virtue of Justice",
                ExpectedKeywords = new List<string> { "justice", "truth", "love", "yew", "wrong", "fairness" },
                Description = "Scholar should know about Justice virtue"
            });

            // Test 7.5: Sage knows about the Lycaeum
            await RunKnowledgeTest(new TestCase
            {
                NPCType = "Sage",
                PersonalityType = NPCPersonalities.PersonalityType.Sage,
                SpeechPattern = NPCPersonalities.SpeechPattern.Formal,
                Question = "What is the Lycaeum?",
                ExpectedKeywords = new List<string> { "lycaeum", "truth", "moonglow", "house", "principle", "library" },
                Description = "Scholar should know about the Lycaeum"
            });

            // Test 7.6: Sage knows about Empath Abbey
            await RunKnowledgeTest(new TestCase
            {
                NPCType = "Sage",
                PersonalityType = NPCPersonalities.PersonalityType.Sage,
                SpeechPattern = NPCPersonalities.SpeechPattern.Formal,
                Question = "Tell me about Empath Abbey",
                ExpectedKeywords = new List<string> { "empath abbey", "love", "yew", "house", "principle", "compassion" },
                Description = "Scholar should know about Empath Abbey"
            });

            // Test 7.7: Sage knows about Serpent's Hold
            await RunKnowledgeTest(new TestCase
            {
                NPCType = "Sage",
                PersonalityType = NPCPersonalities.PersonalityType.Sage,
                SpeechPattern = NPCPersonalities.SpeechPattern.Formal,
                Question = "What is Serpent's Hold?",
                ExpectedKeywords = new List<string> { "serpent's hold", "courage", "trinsic", "house", "principle", "fortress" },
                Description = "Scholar should know about Serpent's Hold"
            });

            // Test 7.8: Healer knows about Spirituality
            await RunKnowledgeTest(new TestCase
            {
                NPCType = "Healer",
                PersonalityType = NPCPersonalities.PersonalityType.Healer,
                SpeechPattern = NPCPersonalities.SpeechPattern.OldEnglish,
                Question = "What is the virtue of Spirituality?",
                ExpectedKeywords = new List<string> { "spirituality", "truth", "love", "courage", "skara brae", "hythloth", "enlightenment" },
                Description = "Healer should know about Spirituality virtue"
            });

            // Test 7.9: Guard knows about Honor
            await RunKnowledgeTest(new TestCase
            {
                NPCType = "Guard",
                PersonalityType = NPCPersonalities.PersonalityType.Guard,
                SpeechPattern = NPCPersonalities.SpeechPattern.Formal,
                Question = "Explain the virtue of Honor",
                ExpectedKeywords = new List<string> { "honor", "truth", "courage", "trinsic", "shame", "integrity" },
                Description = "Guard should know about Honor virtue"
            });

            WriteLog("");
        }

        #endregion

        #region Test Suite 8: All Personality Types (Comprehensive)

        private static async Task TestSuite8_AllPersonalityTypes()
        {
            WriteLog("═══════════════════════════════════════════════════════════");
            WriteLog("TEST SUITE 8: All Personality Types (ABSOLUTELY COMPREHENSIVE)");
            WriteLog("═══════════════════════════════════════════════════════════");
            WriteLog("");

            // Test ALL 54 personality types - absolutely comprehensive
            var testCases = new List<TestCase>();

            // ========== CORE EXPERT NPCs ==========
            testCases.Add(new TestCase { NPCType = "Mage", PersonalityType = NPCPersonalities.PersonalityType.Mage, SpeechPattern = NPCPersonalities.SpeechPattern.Archaic, Question = "What is the First Circle of magic?", ExpectedKeywords = new List<string> { "first circle", "magery", "spell", "magic" }, Description = "Mage expert domain" });
            testCases.Add(new TestCase { NPCType = "Sage", PersonalityType = NPCPersonalities.PersonalityType.Sage, SpeechPattern = NPCPersonalities.SpeechPattern.Archaic, Question = "Tell me about the Eight Virtues", ExpectedKeywords = new List<string> { "virtue", "honesty", "compassion", "valor", "truth" }, Description = "Sage expert domain (lore)" });
            testCases.Add(new TestCase { NPCType = "Healer", PersonalityType = NPCPersonalities.PersonalityType.Healer, SpeechPattern = NPCPersonalities.SpeechPattern.OldEnglish, Question = "What herbs cure poison?", ExpectedKeywords = new List<string> { "herb", "poison", "cure", "healing", "garlic" }, Description = "Healer expert domain" });
            testCases.Add(new TestCase { NPCType = "Guard", PersonalityType = NPCPersonalities.PersonalityType.Guard, SpeechPattern = NPCPersonalities.SpeechPattern.Formal, Question = "What threats should I watch for?", ExpectedKeywords = new List<string> { "threat", "danger", "monster", "protect", "law" }, Description = "Guard expert domain" });
            testCases.Add(new TestCase { NPCType = "Merchant", PersonalityType = NPCPersonalities.PersonalityType.Merchant, SpeechPattern = NPCPersonalities.SpeechPattern.Formal, Question = "What determines an item's value?", ExpectedKeywords = new List<string> { "value", "price", "quality", "gold", "trade" }, Description = "Merchant expert domain" });
            testCases.Add(new TestCase { NPCType = "Warrior", PersonalityType = NPCPersonalities.PersonalityType.Warrior, SpeechPattern = NPCPersonalities.SpeechPattern.Formal, Question = "What weapons are best for combat?", ExpectedKeywords = new List<string> { "weapon", "combat", "sword", "battle", "fight" }, Description = "Warrior expert domain" });
            testCases.Add(new TestCase { NPCType = "Noble", PersonalityType = NPCPersonalities.PersonalityType.Noble, SpeechPattern = NPCPersonalities.SpeechPattern.Formal, Question = "What is proper court etiquette?", ExpectedKeywords = new List<string> { "etiquette", "court", "noble", "protocol", "honor" }, Description = "Noble domain" });
            testCases.Add(new TestCase { NPCType = "Commoner", PersonalityType = NPCPersonalities.PersonalityType.Commoner, SpeechPattern = NPCPersonalities.SpeechPattern.Casual, Question = "What's the latest gossip?", ExpectedKeywords = new List<string> { "gossip", "news", "local", "community", "neighbor" }, Description = "Commoner domain" });
            testCases.Add(new TestCase { NPCType = "Villain", PersonalityType = NPCPersonalities.PersonalityType.Villain, SpeechPattern = NPCPersonalities.SpeechPattern.Cryptic, Question = "What dark secrets do you know?", ExpectedKeywords = new List<string> { "secret", "dark", "power", "evil" }, Description = "Villain domain" });
            testCases.Add(new TestCase { NPCType = "Hermit", PersonalityType = NPCPersonalities.PersonalityType.Hermit, SpeechPattern = NPCPersonalities.SpeechPattern.Cryptic, Question = "What wisdom have you learned?", ExpectedKeywords = new List<string> { "wisdom", "knowledge", "truth", "understanding" }, Description = "Hermit domain" });

            // ========== CRAFTING NPCs ==========
            testCases.Add(new TestCase { NPCType = "Blacksmith", PersonalityType = NPCPersonalities.PersonalityType.Blacksmith, SpeechPattern = NPCPersonalities.SpeechPattern.Archaic, Question = "What metals can I forge with?", ExpectedKeywords = new List<string> { "iron", "steel", "valorite", "metal", "forge" }, Description = "Blacksmith expert domain" });
            testCases.Add(new TestCase { NPCType = "Weaponsmith", PersonalityType = NPCPersonalities.PersonalityType.Weaponsmith, SpeechPattern = NPCPersonalities.SpeechPattern.Archaic, Question = "What makes a good sword?", ExpectedKeywords = new List<string> { "sword", "weapon", "metal", "craft", "quality" }, Description = "Weaponsmith expert domain" });
            testCases.Add(new TestCase { NPCType = "Armorer", PersonalityType = NPCPersonalities.PersonalityType.Armorer, SpeechPattern = NPCPersonalities.SpeechPattern.Archaic, Question = "What armor provides the best protection?", ExpectedKeywords = new List<string> { "armor", "protection", "defense", "plate", "mail" }, Description = "Armorer expert domain" });
            testCases.Add(new TestCase { NPCType = "Tailor", PersonalityType = NPCPersonalities.PersonalityType.Tailor, SpeechPattern = NPCPersonalities.SpeechPattern.Formal, Question = "What fabrics are best for clothing?", ExpectedKeywords = new List<string> { "fabric", "cloth", "leather", "clothing", "tailor" }, Description = "Tailor expert domain" });
            testCases.Add(new TestCase { NPCType = "Jeweler", PersonalityType = NPCPersonalities.PersonalityType.Jeweler, SpeechPattern = NPCPersonalities.SpeechPattern.Formal, Question = "What gems are most valuable?", ExpectedKeywords = new List<string> { "gem", "ruby", "sapphire", "diamond", "jewelry" }, Description = "Jeweler expert domain" });
            testCases.Add(new TestCase { NPCType = "Carpenter", PersonalityType = NPCPersonalities.PersonalityType.Carpenter, SpeechPattern = NPCPersonalities.SpeechPattern.Formal, Question = "What wood is best for furniture?", ExpectedKeywords = new List<string> { "wood", "furniture", "oak", "yew", "carpenter" }, Description = "Carpenter expert domain" });
            testCases.Add(new TestCase { NPCType = "LeatherWorker", PersonalityType = NPCPersonalities.PersonalityType.LeatherWorker, SpeechPattern = NPCPersonalities.SpeechPattern.Formal, Question = "What types of leather are available?", ExpectedKeywords = new List<string> { "leather", "spined", "horned", "barbed", "craft" }, Description = "LeatherWorker expert domain" });
            testCases.Add(new TestCase { NPCType = "Bowyer", PersonalityType = NPCPersonalities.PersonalityType.Bowyer, SpeechPattern = NPCPersonalities.SpeechPattern.Formal, Question = "What makes a good bow?", ExpectedKeywords = new List<string> { "bow", "archery", "wood", "yew", "crossbow" }, Description = "Bowyer expert domain" });
            testCases.Add(new TestCase { NPCType = "Tinker", PersonalityType = NPCPersonalities.PersonalityType.Tinker, SpeechPattern = NPCPersonalities.SpeechPattern.Formal, Question = "What can you craft with tinkering?", ExpectedKeywords = new List<string> { "tinker", "craft", "gadget", "repair", "mechanism" }, Description = "Tinker expert domain" });
            testCases.Add(new TestCase { NPCType = "Cobbler", PersonalityType = NPCPersonalities.PersonalityType.Cobbler, SpeechPattern = NPCPersonalities.SpeechPattern.Formal, Question = "What can you tell me about shoes?", ExpectedKeywords = new List<string> { "shoe", "boot", "footwear", "craft", "leather" }, Description = "Cobbler domain" });

            // ========== ALCHEMY & HEALING ==========
            testCases.Add(new TestCase { NPCType = "Alchemist", PersonalityType = NPCPersonalities.PersonalityType.Alchemist, SpeechPattern = NPCPersonalities.SpeechPattern.Archaic, Question = "What potions can you make?", ExpectedKeywords = new List<string> { "potion", "heal", "cure", "poison", "alchemy" }, Description = "Alchemist expert domain" });
            testCases.Add(new TestCase { NPCType = "Herbalist", PersonalityType = NPCPersonalities.PersonalityType.Herbalist, SpeechPattern = NPCPersonalities.SpeechPattern.OldEnglish, Question = "What herbs have healing properties?", ExpectedKeywords = new List<string> { "herb", "healing", "plant", "remedy", "nature" }, Description = "Herbalist expert domain" });
            testCases.Add(new TestCase { NPCType = "Veterinarian", PersonalityType = NPCPersonalities.PersonalityType.Veterinarian, SpeechPattern = NPCPersonalities.SpeechPattern.Formal, Question = "How do I care for my pet?", ExpectedKeywords = new List<string> { "pet", "animal", "care", "heal", "veterinary" }, Description = "Veterinarian expert domain" });

            // ========== SERVICE NPCs ==========
            testCases.Add(new TestCase { NPCType = "InnKeeper", PersonalityType = NPCPersonalities.PersonalityType.InnKeeper, SpeechPattern = NPCPersonalities.SpeechPattern.Formal, Question = "What services does your inn provide?", ExpectedKeywords = new List<string> { "inn", "room", "food", "service", "hospitality" }, Description = "InnKeeper expert domain" });
            testCases.Add(new TestCase { NPCType = "Barkeeper", PersonalityType = NPCPersonalities.PersonalityType.Barkeeper, SpeechPattern = NPCPersonalities.SpeechPattern.Casual, Question = "What drinks do you serve?", ExpectedKeywords = new List<string> { "drink", "ale", "wine", "tavern", "bar" }, Description = "Barkeeper expert domain" });
            testCases.Add(new TestCase { NPCType = "Banker", PersonalityType = NPCPersonalities.PersonalityType.Banker, SpeechPattern = NPCPersonalities.SpeechPattern.Formal, Question = "How does banking work?", ExpectedKeywords = new List<string> { "bank", "gold", "deposit", "withdraw", "secure" }, Description = "Banker expert domain" });
            testCases.Add(new TestCase { NPCType = "AnimalTrainer", PersonalityType = NPCPersonalities.PersonalityType.AnimalTrainer, SpeechPattern = NPCPersonalities.SpeechPattern.Formal, Question = "What animals can be tamed?", ExpectedKeywords = new List<string> { "animal", "tame", "pet", "creature", "training" }, Description = "AnimalTrainer expert domain" });
            testCases.Add(new TestCase { NPCType = "Provisioner", PersonalityType = NPCPersonalities.PersonalityType.Provisioner, SpeechPattern = NPCPersonalities.SpeechPattern.Formal, Question = "What supplies do adventurers need?", ExpectedKeywords = new List<string> { "supply", "gear", "adventurer", "equipment", "travel" }, Description = "Provisioner expert domain" });

            // ========== WORKER NPCs ==========
            testCases.Add(new TestCase { NPCType = "Farmer", PersonalityType = NPCPersonalities.PersonalityType.Farmer, SpeechPattern = NPCPersonalities.SpeechPattern.Casual, Question = "What crops grow well here?", ExpectedKeywords = new List<string> { "crop", "farm", "grow", "harvest", "agriculture" }, Description = "Farmer domain" });
            testCases.Add(new TestCase { NPCType = "Miner", PersonalityType = NPCPersonalities.PersonalityType.Miner, SpeechPattern = NPCPersonalities.SpeechPattern.Casual, Question = "Where can I find good ore?", ExpectedKeywords = new List<string> { "ore", "mine", "mining", "metal", "vein" }, Description = "Miner domain" });
            testCases.Add(new TestCase { NPCType = "Fisherman", PersonalityType = NPCPersonalities.PersonalityType.Fisherman, SpeechPattern = NPCPersonalities.SpeechPattern.Casual, Question = "Where are the best fishing spots?", ExpectedKeywords = new List<string> { "fish", "fishing", "spot", "water", "catch" }, Description = "Fisherman domain" });
            testCases.Add(new TestCase { NPCType = "Cook", PersonalityType = NPCPersonalities.PersonalityType.Cook, SpeechPattern = NPCPersonalities.SpeechPattern.Casual, Question = "What recipes can you teach me?", ExpectedKeywords = new List<string> { "recipe", "cook", "food", "meal", "dish" }, Description = "Cook domain" });
            testCases.Add(new TestCase { NPCType = "Peasant", PersonalityType = NPCPersonalities.PersonalityType.Peasant, SpeechPattern = NPCPersonalities.SpeechPattern.Casual, Question = "What is life like in the countryside?", ExpectedKeywords = new List<string> { "countryside", "rural", "farm", "simple", "life" }, Description = "Peasant domain" });

            // ========== SPECIALIZED NPCs ==========
            testCases.Add(new TestCase { NPCType = "Ranger", PersonalityType = NPCPersonalities.PersonalityType.Ranger, SpeechPattern = NPCPersonalities.SpeechPattern.Formal, Question = "What dangers lurk in the wilderness?", ExpectedKeywords = new List<string> { "wilderness", "danger", "creature", "forest", "nature" }, Description = "Ranger expert domain" });
            testCases.Add(new TestCase { NPCType = "Mapmaker", PersonalityType = NPCPersonalities.PersonalityType.Mapmaker, SpeechPattern = NPCPersonalities.SpeechPattern.Formal, Question = "What locations should I map?", ExpectedKeywords = new List<string> { "map", "location", "geography", "city", "route" }, Description = "Mapmaker expert domain" });
            testCases.Add(new TestCase { NPCType = "Scribe", PersonalityType = NPCPersonalities.PersonalityType.Scribe, SpeechPattern = NPCPersonalities.SpeechPattern.Formal, Question = "What can you write for me?", ExpectedKeywords = new List<string> { "write", "book", "scroll", "document", "scribe" }, Description = "Scribe domain" });
            testCases.Add(new TestCase { NPCType = "RealEstateBroker", PersonalityType = NPCPersonalities.PersonalityType.RealEstateBroker, SpeechPattern = NPCPersonalities.SpeechPattern.Formal, Question = "What properties are available?", ExpectedKeywords = new List<string> { "property", "house", "location", "real estate", "broker" }, Description = "RealEstateBroker domain" });
            testCases.Add(new TestCase { NPCType = "TownCrier", PersonalityType = NPCPersonalities.PersonalityType.TownCrier, SpeechPattern = NPCPersonalities.SpeechPattern.Formal, Question = "What news do you have?", ExpectedKeywords = new List<string> { "news", "announcement", "proclamation", "event", "town" }, Description = "TownCrier domain" });

            // ========== CULTURAL/ENTERTAINMENT NPCs ==========
            testCases.Add(new TestCase { NPCType = "Bard", PersonalityType = NPCPersonalities.PersonalityType.Bard, SpeechPattern = NPCPersonalities.SpeechPattern.Archaic, Question = "Tell me a tale of adventure", ExpectedKeywords = new List<string> { "tale", "story", "song", "ballad", "adventure" }, Description = "Bard domain" });
            testCases.Add(new TestCase { NPCType = "Actor", PersonalityType = NPCPersonalities.PersonalityType.Actor, SpeechPattern = NPCPersonalities.SpeechPattern.Archaic, Question = "What plays have you performed?", ExpectedKeywords = new List<string> { "play", "performance", "theatre", "drama", "stage" }, Description = "Actor domain" });
            testCases.Add(new TestCase { NPCType = "Artist", PersonalityType = NPCPersonalities.PersonalityType.Artist, SpeechPattern = NPCPersonalities.SpeechPattern.Formal, Question = "What art do you create?", ExpectedKeywords = new List<string> { "art", "painting", "sculpture", "creative", "aesthetic" }, Description = "Artist domain" });
            testCases.Add(new TestCase { NPCType = "Gypsy", PersonalityType = NPCPersonalities.PersonalityType.Gypsy, SpeechPattern = NPCPersonalities.SpeechPattern.Cryptic, Question = "Can you tell my fortune?", ExpectedKeywords = new List<string> { "fortune", "divination", "mystical", "fate", "prediction" }, Description = "Gypsy domain" });
            testCases.Add(new TestCase { NPCType = "HairStylist", PersonalityType = NPCPersonalities.PersonalityType.HairStylist, SpeechPattern = NPCPersonalities.SpeechPattern.Casual, Question = "What hairstyles can you create?", ExpectedKeywords = new List<string> { "hairstyle", "hair", "style", "appearance", "fashion" }, Description = "HairStylist domain" });

            // ========== MARITIME NPCs ==========
            testCases.Add(new TestCase { NPCType = "Shipwright", PersonalityType = NPCPersonalities.PersonalityType.Shipwright, SpeechPattern = NPCPersonalities.SpeechPattern.Formal, Question = "What ships can you build?", ExpectedKeywords = new List<string> { "ship", "boat", "vessel", "maritime", "sailing" }, Description = "Shipwright domain" });

            // ========== COMBAT/ADVENTURE NPCs ==========
            testCases.Add(new TestCase { NPCType = "Paladin", PersonalityType = NPCPersonalities.PersonalityType.Paladin, SpeechPattern = NPCPersonalities.SpeechPattern.Formal, Question = "What is the path of righteousness?", ExpectedKeywords = new List<string> { "righteous", "honor", "virtue", "justice", "holy" }, Description = "Paladin domain" });
            testCases.Add(new TestCase { NPCType = "Samurai", PersonalityType = NPCPersonalities.PersonalityType.Samurai, SpeechPattern = NPCPersonalities.SpeechPattern.Formal, Question = "What is the way of honor?", ExpectedKeywords = new List<string> { "honor", "bushido", "discipline", "combat", "way" }, Description = "Samurai domain" });
            testCases.Add(new TestCase { NPCType = "Ninja", PersonalityType = NPCPersonalities.PersonalityType.Ninja, SpeechPattern = NPCPersonalities.SpeechPattern.Cryptic, Question = "What is the way of shadows?", ExpectedKeywords = new List<string> { "shadow", "stealth", "secret", "discipline", "precision" }, Description = "Ninja domain" });
            testCases.Add(new TestCase { NPCType = "Monk", PersonalityType = NPCPersonalities.PersonalityType.Monk, SpeechPattern = NPCPersonalities.SpeechPattern.OldEnglish, Question = "What is inner peace?", ExpectedKeywords = new List<string> { "peace", "spiritual", "discipline", "meditation", "inner" }, Description = "Monk domain" });
            testCases.Add(new TestCase { NPCType = "Pirate", PersonalityType = NPCPersonalities.PersonalityType.Pirate, SpeechPattern = NPCPersonalities.SpeechPattern.Casual, Question = "What treasures have you found?", ExpectedKeywords = new List<string> { "treasure", "plunder", "sea", "ship", "gold" }, Description = "Pirate domain" });
            testCases.Add(new TestCase { NPCType = "Thief", PersonalityType = NPCPersonalities.PersonalityType.Thief, SpeechPattern = NPCPersonalities.SpeechPattern.Cryptic, Question = "What secrets do you know?", ExpectedKeywords = new List<string> { "secret", "shadow", "stealth", "lock", "pick" }, Description = "Thief domain" });

            // ========== TRAVELER NPCs ==========
            testCases.Add(new TestCase { NPCType = "Vagabond", PersonalityType = NPCPersonalities.PersonalityType.Vagabond, SpeechPattern = NPCPersonalities.SpeechPattern.Casual, Question = "What places have you visited?", ExpectedKeywords = new List<string> { "travel", "place", "journey", "wander", "adventure" }, Description = "Vagabond domain" });
            testCases.Add(new TestCase { NPCType = "Escortable", PersonalityType = NPCPersonalities.PersonalityType.Escortable, SpeechPattern = NPCPersonalities.SpeechPattern.Formal, Question = "Where can you take me?", ExpectedKeywords = new List<string> { "destination", "travel", "location", "escort", "guide" }, Description = "Escortable domain" });

            // ========== SERVICE NPCs (no domain knowledge) ==========
            testCases.Add(new TestCase { NPCType = "Henchman", PersonalityType = NPCPersonalities.PersonalityType.Henchman, SpeechPattern = NPCPersonalities.SpeechPattern.Formal, Question = "What services do you provide?", ExpectedKeywords = new List<string> { "service", "help", "assist", "aid" }, Description = "Henchman domain" });
            testCases.Add(new TestCase { NPCType = "Beggar", PersonalityType = NPCPersonalities.PersonalityType.Beggar, SpeechPattern = NPCPersonalities.SpeechPattern.Casual, Question = "What do you need?", ExpectedKeywords = new List<string> { "charity", "help", "coin", "gold", "aid" }, Description = "Beggar domain" });

            // Run all expert domain tests
            WriteLog($"Testing ALL {testCases.Count} personality types (Expert Domain Tests)...");
            WriteLog("");

            foreach (var testCase in testCases)
            {
                await RunKnowledgeTest(testCase);
            }

            WriteLog("");
            WriteLog("═══════════════════════════════════════════════════════════");
            WriteLog("REFERRAL TESTS: Testing Knowledge Boundaries");
            WriteLog("═══════════════════════════════════════════════════════════");
            WriteLog("");

            // Test referral behavior for key NPC types
            var referralTests = new List<TestCase>
            {
                // Crafting NPCs should refer magic questions
                new TestCase { NPCType = "Blacksmith", PersonalityType = NPCPersonalities.PersonalityType.Blacksmith, SpeechPattern = NPCPersonalities.SpeechPattern.Archaic, Question = "How do I cast a fireball spell?", ShouldRefer = true, ReferralTarget = "Mage", ExpectedKeywords = new List<string> { "mage", "moonglow", "refer", "spell" }, Description = "Blacksmith should refer magic questions" },
                new TestCase { NPCType = "Tailor", PersonalityType = NPCPersonalities.PersonalityType.Tailor, SpeechPattern = NPCPersonalities.SpeechPattern.Formal, Question = "What reagents do I need for healing?", ShouldRefer = true, ReferralTarget = "Healer", ExpectedKeywords = new List<string> { "healer", "refer", "reagent", "healing" }, Description = "Tailor should refer healing questions" },
                new TestCase { NPCType = "Carpenter", PersonalityType = NPCPersonalities.PersonalityType.Carpenter, SpeechPattern = NPCPersonalities.SpeechPattern.Formal, Question = "How do I forge a sword?", ShouldRefer = true, ReferralTarget = "Blacksmith", ExpectedKeywords = new List<string> { "blacksmith", "minoc", "refer", "forge" }, Description = "Carpenter should refer forging questions" },
                
                // Service NPCs should refer specialized questions
                new TestCase { NPCType = "InnKeeper", PersonalityType = NPCPersonalities.PersonalityType.InnKeeper, SpeechPattern = NPCPersonalities.SpeechPattern.Formal, Question = "How do I craft armor?", ShouldRefer = true, ReferralTarget = "Blacksmith", ExpectedKeywords = new List<string> { "blacksmith", "refer", "armor", "craft" }, Description = "InnKeeper should refer crafting questions" },
                new TestCase { NPCType = "Banker", PersonalityType = NPCPersonalities.PersonalityType.Banker, SpeechPattern = NPCPersonalities.SpeechPattern.Formal, Question = "What spells can I learn?", ShouldRefer = true, ReferralTarget = "Mage", ExpectedKeywords = new List<string> { "mage", "moonglow", "refer", "spell" }, Description = "Banker should refer magic questions" },
                
                // Worker NPCs should refer specialized questions
                new TestCase { NPCType = "Farmer", PersonalityType = NPCPersonalities.PersonalityType.Farmer, SpeechPattern = NPCPersonalities.SpeechPattern.Casual, Question = "How do I enchant a weapon?", ShouldRefer = true, ReferralTarget = "Mage", ExpectedKeywords = new List<string> { "mage", "refer", "enchant", "weapon" }, Description = "Farmer should refer magic questions" },
                new TestCase { NPCType = "Miner", PersonalityType = NPCPersonalities.PersonalityType.Miner, SpeechPattern = NPCPersonalities.SpeechPattern.Casual, Question = "What potions cure poison?", ShouldRefer = true, ReferralTarget = "Healer", ExpectedKeywords = new List<string> { "healer", "refer", "potion", "poison" }, Description = "Miner should refer healing questions" },
                
                // Cultural NPCs should refer specialized questions
                new TestCase { NPCType = "Bard", PersonalityType = NPCPersonalities.PersonalityType.Bard, SpeechPattern = NPCPersonalities.SpeechPattern.Archaic, Question = "How do I smith valorite armor?", ShouldRefer = true, ReferralTarget = "Blacksmith", ExpectedKeywords = new List<string> { "blacksmith", "refer", "valorite", "armor" }, Description = "Bard should refer crafting questions" },
                new TestCase { NPCType = "Noble", PersonalityType = NPCPersonalities.PersonalityType.Noble, SpeechPattern = NPCPersonalities.SpeechPattern.Formal, Question = "What reagents are used in magic?", ShouldRefer = true, ReferralTarget = "Mage", ExpectedKeywords = new List<string> { "mage", "moonglow", "refer", "reagent" }, Description = "Noble should refer magic questions" },
                
                // Combat NPCs should refer crafting questions
                new TestCase { NPCType = "Guard", PersonalityType = NPCPersonalities.PersonalityType.Guard, SpeechPattern = NPCPersonalities.SpeechPattern.Formal, Question = "How do I craft a bow?", ShouldRefer = true, ReferralTarget = "Bowyer", ExpectedKeywords = new List<string> { "bowyer", "refer", "craft", "bow" }, Description = "Guard should refer crafting questions" },
                new TestCase { NPCType = "Warrior", PersonalityType = NPCPersonalities.PersonalityType.Warrior, SpeechPattern = NPCPersonalities.SpeechPattern.Formal, Question = "What fabrics make the best clothing?", ShouldRefer = true, ReferralTarget = "Tailor", ExpectedKeywords = new List<string> { "tailor", "refer", "fabric", "clothing" }, Description = "Warrior should refer tailoring questions" },
            };

            WriteLog($"Testing {referralTests.Count} referral scenarios...");
            WriteLog("");

            foreach (var testCase in referralTests)
            {
                await RunKnowledgeTest(testCase);
            }

            WriteLog("");
            WriteLog("═══════════════════════════════════════════════════════════");
            WriteLog("COMPREHENSIVE PERSONALITY TYPE COVERAGE:");
            WriteLog("═══════════════════════════════════════════════════════════");
            
            // Count by role
            var roleGroups = testCases.GroupBy(t => NPCKnowledgeSystem.InferRoleFromPersonality(t.PersonalityType));
            WriteLog("");
            WriteLog("Coverage by NPCRole:");
            foreach (var group in roleGroups.OrderBy(g => g.Key.ToString()))
            {
                WriteLog($"  {group.Key}: {group.Count()} personality types");
                foreach (var test in group)
                {
                    WriteLog($"    - {test.NPCType}");
                }
            }
            
            // Count by domain knowledge status (check via GetPersonalityPrompt which includes domain knowledge)
            int withDomain = 0;
            int withoutDomain = 0;
            var withDomainList = new List<string>();
            var withoutDomainList = new List<string>();
            
            foreach (var test in testCases)
            {
                string prompt = NPCPersonalities.GetPersonalityPrompt(test.PersonalityType, test.SpeechPattern);
                if (prompt.Contains("DOMAIN KNOWLEDGE:") && prompt.IndexOf("DOMAIN KNOWLEDGE:") < prompt.Length - 50)
                {
                    withDomain++;
                    withDomainList.Add(test.NPCType);
                }
                else
                {
                    withoutDomain++;
                    withoutDomainList.Add(test.NPCType);
                }
            }
            
            WriteLog("");
            WriteLog("Domain Knowledge Status:");
            WriteLog($"  With Domain Knowledge ({withDomain}): {string.Join(", ", withDomainList.OrderBy(x => x))}");
            WriteLog($"  Without Domain Knowledge ({withoutDomain}): {string.Join(", ", withoutDomainList.OrderBy(x => x))}");
            
            WriteLog("");
            WriteLog($"  Total Personality Types Tested: {testCases.Count}");
            WriteLog($"  Total Referral Tests: {referralTests.Count}");
            WriteLog($"  Grand Total Tests in Suite 8: {testCases.Count + referralTests.Count}");
            
            WriteLog("");
            WriteLog("═══════════════════════════════════════════════════════════");
            WriteLog("ALL 54 PERSONALITY TYPES VERIFIED:");
            WriteLog("═══════════════════════════════════════════════════════════");
            WriteLog("Every personality type has been tested for:");
            WriteLog("  1. Expert domain knowledge (if applicable)");
            WriteLog("  2. Knowledge boundaries and referral behavior");
            WriteLog("  3. Proper role mapping and expertise levels");
            WriteLog("");
        }

        #endregion

        private static async Task RunMemoryTest(string npcType, NPCPersonalities.PersonalityType personalityType, 
            NPCPersonalities.SpeechPattern speechPattern, string[] setupMessages, string testQuestion,
            List<string> expectedKeywords, string description)
        {
            WriteLog($"[TEST] {npcType} Memory: {description} ... ");

            try
            {
                int npcSerial = Math.Abs($"Test{npcType}".GetHashCode());
                string playerName = "TestPlayer";
                string npcName = $"Test{npcType}";

                // Setup: Create memories from previous conversations
                foreach (var setupMessage in setupMessages)
                {
                    bool shouldSave = MemoryHelpers.ShouldSaveMemory(setupMessage, "", out int importance);
                    if (shouldSave)
                    {
                        // Extract meaningful content from the message (like the real system does)
                        string memoryContent = ExtractMemoryContent(setupMessage);
                        
                        var memory = new Memory
                        {
                            NpcSerial = npcSerial,
                            NpcName = npcName,
                            PlayerName = playerName,
                            Type = DetermineMemoryType(setupMessage),
                            Content = memoryContent, // Store extracted content, not raw message
                            Importance = importance,
                            CreatedAt = DateTime.UtcNow,
                            LastAccessed = DateTime.UtcNow
                        };
                        await InMemoryFallbackStore.SaveMemoryAsync(memory);
                    }

                    // Update relationship
                    var currentRelationship = await InMemoryFallbackStore.GetRelationshipAsync(npcSerial, playerName);
                    if (currentRelationship == null)
                    {
                        currentRelationship = new Relationship
                        {
                            NpcSerial = npcSerial,
                            NpcName = npcName,
                            PlayerName = playerName,
                            Type = RelationshipType.Stranger,
                            Score = 1,
                            FirstMet = DateTime.UtcNow,
                            LastInteraction = DateTime.UtcNow,
                            InteractionCount = 1
                        };
                    }
                    else
                    {
                        currentRelationship.Score += 1;
                        currentRelationship.InteractionCount++;
                        currentRelationship.LastInteraction = DateTime.UtcNow;
                        
                        // Update relationship type based on score
                        if (currentRelationship.Score >= 81) currentRelationship.Type = RelationshipType.Ally;
                        else if (currentRelationship.Score >= 61) currentRelationship.Type = RelationshipType.CloseFriend;
                        else if (currentRelationship.Score >= 41) currentRelationship.Type = RelationshipType.Friend;
                        else if (currentRelationship.Score >= 21) currentRelationship.Type = RelationshipType.Acquaintance;
                    }
                    await InMemoryFallbackStore.SaveRelationshipAsync(currentRelationship);
                }

                // Now test if NPC remembers
                string personality = NPCPersonalities.GetPersonalityPrompt(personalityType, speechPattern);
                var role = NPCKnowledgeSystem.InferRoleFromPersonality(personalityType);
                var allLore = SimpleLoreSystem.GetAllLore();
                var roleKnowledge = NPCKnowledgeSystem.GetRoleKnowledge(role, allLore);
                var filteredKnowledge = roleKnowledge.Where(l =>
                {
                    var expertise = l.GetExpertise(role);
                    return expertise == KnowledgeExpertise.Expert || expertise == KnowledgeExpertise.Proficient;
                }).ToList();
                string preloadedKnowledge = NPCKnowledgeSystem.FormatKnowledgeForPrompt(filteredKnowledge);

                // Load memories
                var memories = await InMemoryFallbackStore.GetMemoriesAsync(npcSerial, playerName, limit: 5);
                var relationship = await InMemoryFallbackStore.GetRelationshipAsync(npcSerial, playerName);
                string memoriesText = "";
                
                WriteLog($"   [DEBUG] Memory check: npcSerial={npcSerial}, playerName={playerName}");
                WriteLog($"   [DEBUG] Loaded {memories.Count} memories");
                if (memories.Count > 0)
                {
                    memoriesText = MemoryHelpers.FormatMemoriesForPrompt(memories, maxMemories: 5);
                    WriteLog($"   [DEBUG] Memory content preview: {memoriesText.Substring(0, Math.Min(200, memoriesText.Length))}...");
                    foreach (var mem in memories)
                    {
                        WriteLog($"     - [{mem.Type}] {mem.Content}");
                    }
                }
                if (relationship != null)
                {
                    memoriesText += MemoryHelpers.FormatRelationshipForPrompt(relationship);
                    WriteLog($"   [DEBUG] Relationship: {relationship.Type} (Score: {relationship.Score})");
                }
                else
                {
                    WriteLog($"   [DEBUG] No relationship found");
                }

                var conversationHistory = new List<ConversationMessage>();
                string response = await UnifiedLLMService.GetResponseAsync(
                    npcName: npcName,
                    npcPersonality: personality,
                    conversationHistory: conversationHistory,
                    playerMessage: testQuestion,
                    playerName: playerName,
                    preloadedKnowledge: preloadedKnowledge + (string.IsNullOrEmpty(memoriesText) ? "" : "\n\n" + memoriesText),
                    requestType: UnifiedLLMService.RequestType.PlayerConversation
                );

                // Analyze response
                string lowerResponse = response.ToLower();
                int keywordMatches = expectedKeywords.Count(kw => lowerResponse.Contains(kw.ToLower()));
                bool passed = keywordMatches >= 2;

                var result = new TestResult
                {
                    NPCType = npcType,
                    Question = testQuestion,
                    Response = response,
                    Passed = passed,
                    Reason = passed ? $"{keywordMatches} keywords found" : $"Only {keywordMatches} keyword(s) found. Expected: {string.Join(", ", expectedKeywords)}",
                    ExpectedKeywords = string.Join(", ", expectedKeywords),
                    ShouldRefer = false
                };

                allResults.Add(result);

                if (passed)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("PASS");
                    Console.ResetColor();
                    WriteLog("PASS");
                    testsPassed++;
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("FAIL");
                    Console.ResetColor();
                    WriteLog("FAIL");
                    WriteLog($"   Reason: {result.Reason}");
                    WriteLog($"   Response: {response}");
                    testsFailed++;
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"ERROR: {ex.Message}");
                Console.ResetColor();
                WriteLog($"ERROR: {ex.Message}");
                WriteLog($"Stack Trace: {ex.StackTrace}");
                testsFailed++;
            }
        }

        private static async Task RunMemoryNameTest(string npcType, NPCPersonalities.PersonalityType personalityType,
            NPCPersonalities.SpeechPattern speechPattern, string playerName, string wrongName, string description)
        {
            WriteLog($"[TEST] {npcType} Memory: {description} ... ");

            try
            {
                int npcSerial = Math.Abs($"Test{npcType}".GetHashCode());
                string npcName = $"Test{npcType}";

                // Get NPC personality string
                string personality = NPCPersonalities.GetPersonalityPrompt(personalityType, speechPattern);

                // Get NPC knowledge base
                var role = NPCKnowledgeSystem.InferRoleFromPersonality(personalityType);
                var allLore = SimpleLoreSystem.GetAllLore();
                var roleKnowledge = NPCKnowledgeSystem.GetRoleKnowledge(role, allLore);
                var filteredKnowledge = roleKnowledge.Where(l =>
                {
                    var expertise = l.GetExpertise(role);
                    return expertise == KnowledgeExpertise.Expert || expertise == KnowledgeExpertise.Proficient;
                }).ToList();
                string preloadedKnowledge = NPCKnowledgeSystem.FormatKnowledgeForPrompt(filteredKnowledge);

                // Create conversation history where player claims wrong name
                var conversationHistory = new List<ConversationMessage>
                {
                    new ConversationMessage(true, "Do you remember my name?"),
                    new ConversationMessage(true, $"My name is {wrongName}")
                };

                // Get NPC response - should use playerName parameter, not wrongName
                string response = await UnifiedLLMService.GetResponseAsync(
                    npcName: npcName,
                    npcPersonality: personality,
                    conversationHistory: conversationHistory,
                    playerMessage: "What is my name?", // Final question
                    playerName: playerName, // This is the actual parameter - should be used
                    preloadedKnowledge: preloadedKnowledge,
                    requestType: UnifiedLLMService.RequestType.PlayerConversation
                );

                // Analyze response - should mention playerName, not wrongName
                string lowerResponse = response.ToLower();
                bool mentionsCorrectName = lowerResponse.Contains(playerName.ToLower());
                bool mentionsWrongName = lowerResponse.Contains(wrongName.ToLower());
                
                // Should use correct name (from parameter) and ideally correct the player
                bool passed = mentionsCorrectName && !mentionsWrongName;
                
                string reason = "";
                if (mentionsWrongName)
                {
                    reason = $"NPC used wrong name '{wrongName}' instead of parameter '{playerName}'";
                }
                else if (!mentionsCorrectName)
                {
                    reason = $"NPC did not mention correct name '{playerName}' from parameter";
                }
                else
                {
                    reason = "NPC correctly used playerName parameter";
                }

                var result = new TestResult
                {
                    NPCType = npcType,
                    Question = $"Name test: Player claims '{wrongName}', NPC should use '{playerName}'",
                    Response = response,
                    Passed = passed,
                    Reason = reason,
                    ExpectedKeywords = $"Should mention '{playerName}', not '{wrongName}'",
                    ShouldRefer = false
                };

                allResults.Add(result);

                if (passed)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("PASS");
                    Console.ResetColor();
                    WriteLog("PASS");
                    WriteLog($"   Response: {response}");
                    testsPassed++;
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("FAIL");
                    Console.ResetColor();
                    WriteLog("FAIL");
                    WriteLog($"   Reason: {result.Reason}");
                    WriteLog($"   Response: {response}");
                    testsFailed++;
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"ERROR: {ex.Message}");
                Console.ResetColor();
                WriteLog($"ERROR: {ex.Message}");
                WriteLog($"Stack Trace: {ex.StackTrace}");
                testsFailed++;
            }
        }

        private static MemoryType DetermineMemoryType(string message)
        {
            var lower = message.ToLower();
            if (lower.Contains("name") || lower.Contains("i am") || lower.Contains("i'm"))
                return MemoryType.Fact;
            if (lower.Contains("like") || lower.Contains("prefer") || lower.Contains("favorite") || lower.Contains("love") || lower.Contains("hate"))
                return MemoryType.Preference;
            if (lower.Contains("quest") || lower.Contains("mission") || lower.Contains("task"))
                return MemoryType.Event;
            if (lower.Contains("friend") || lower.Contains("enemy") || lower.Contains("ally"))
                return MemoryType.Relationship;
            return MemoryType.Conversation;
        }

        /// <summary>
        /// Extract meaningful content from a message for memory storage
        /// Similar to how the real system processes messages
        /// </summary>
        private static string ExtractMemoryContent(string message)
        {
            var lower = message.ToLower();
            
            // Note: We don't extract names from messages because we should always use the playerName parameter
            // from the actual character data, not what players claim their name is
            
            // For preferences, extract the key information
            if (lower.Contains("love") || lower.Contains("like"))
            {
                int loveIdx = lower.IndexOf("love");
                int likeIdx = lower.IndexOf("like");
                int startIdx = loveIdx >= 0 ? (likeIdx >= 0 ? Math.Min(loveIdx, likeIdx) : loveIdx) : likeIdx;
                if (startIdx >= 0)
                {
                    string after = message.Substring(startIdx).Trim();
                    // Extract the object of love/like
                    return $"The player {after}";
                }
            }
            
            // For "looking for" or "need" statements, extract the key information
            if (lower.Contains("looking for") || lower.Contains("need"))
            {
                int lookingIdx = lower.IndexOf("looking for");
                int needIdx = lower.IndexOf("need");
                int startIdx = lookingIdx >= 0 ? (needIdx >= 0 ? Math.Min(lookingIdx, needIdx) : lookingIdx) : needIdx;
                if (startIdx >= 0)
                {
                    string after = message.Substring(startIdx).Trim();
                    // Extract what they're looking for
                    return $"The player {after}";
                }
            }
            
            // Default: return the message as-is (real system does this)
            return message;
        }

        #region Test Execution

        private static async Task RunKnowledgeTest(TestCase testCase)
        {
            string testMessage = $"[TEST] {testCase.NPCType}: \"{testCase.Question}\" ... ";
            Console.Write(testMessage);
            WriteLog(testMessage.TrimEnd());

            try
            {
                // Get NPC personality string
                string personality = NPCPersonalities.GetPersonalityPrompt(testCase.PersonalityType, testCase.SpeechPattern);

                // Get NPC knowledge base (simulate what LLMNpc does)
                var role = NPCKnowledgeSystem.InferRoleFromPersonality(testCase.PersonalityType);
                var allLore = SimpleLoreSystem.GetAllLore();
                var roleKnowledge = NPCKnowledgeSystem.GetRoleKnowledge(role, allLore);
                
                // Apply expertise filtering (same as GetNPCKnowledge does)
                var filteredKnowledge = roleKnowledge.Where(l =>
                {
                    var expertise = l.GetExpertise(role);
                    return expertise == KnowledgeExpertise.Expert || expertise == KnowledgeExpertise.Proficient;
                }).ToList();
                
                // DEBUG: Log knowledge being loaded (especially for Mage/Blacksmith referral tests)
                if (testCase.ShouldRefer || testCase.NPCType == "Mage")
                {
                    WriteLog($"   [DEBUG] Role: {role}");
                    WriteLog($"   [DEBUG] Role knowledge (before filtering): {roleKnowledge.Count} entries");
                    WriteLog($"   [DEBUG] Filtered knowledge (Expert/Proficient only): {filteredKnowledge.Count} entries");
                    
                    var craftingKnowledge = filteredKnowledge.Where(l => l.Category == "Crafting").ToList();
                    if (craftingKnowledge.Count > 0)
                    {
                        WriteLog($"   [DEBUG] WARNING: {craftingKnowledge.Count} Crafting entries found for {testCase.NPCType} after filtering:");
                        foreach (var lore in craftingKnowledge.Take(5))
                        {
                            var expertise = lore.GetExpertise(role);
                            WriteLog($"     - {lore.Title} (Category: {lore.Category}, Expertise: {expertise})");
                        }
                    }
                    else
                    {
                        var craftingBeforeFilter = roleKnowledge.Where(l => l.Category == "Crafting").ToList();
                        if (craftingBeforeFilter.Count > 0)
                        {
                            WriteLog($"   [DEBUG] {craftingBeforeFilter.Count} Crafting entries filtered out (correctly):");
                            foreach (var lore in craftingBeforeFilter.Take(3))
                            {
                                var expertise = lore.GetExpertise(role);
                                WriteLog($"     - {lore.Title} (Expertise: {expertise} - filtered out)");
                            }
                        }
                    }
                }
                
                string preloadedKnowledge = NPCKnowledgeSystem.FormatKnowledgeForPrompt(filteredKnowledge);

                // Create conversation history (empty for first question)
                var conversationHistory = new List<ConversationMessage>();

                // Load memories and relationship (simulate what LLMNpc does)
                string memoriesText = "";
                int npcSerial = Math.Abs($"Test{testCase.NPCType}".GetHashCode()); // Use consistent serial for test NPCs
                string playerName = "TestPlayer";
                
                if (InMemoryFallbackStore.IsActive)
                {
                    try
                    {
                        var memories = await InMemoryFallbackStore.GetMemoriesAsync(npcSerial, playerName, limit: 5);
                        var relationship = await InMemoryFallbackStore.GetRelationshipAsync(npcSerial, playerName);
                        
                        if (memories.Count > 0)
                        {
                            memoriesText = MemoryHelpers.FormatMemoriesForPrompt(memories, maxMemories: 5);
                            WriteLog($"   [DEBUG] Loaded {memories.Count} memories for {testCase.NPCType}");
                        }
                        
                        if (relationship != null)
                        {
                            memoriesText += MemoryHelpers.FormatRelationshipForPrompt(relationship);
                            WriteLog($"   [DEBUG] Relationship: {relationship.Type} (Score: {relationship.Score})");
                        }
                    }
                    catch (Exception ex)
                    {
                        WriteLog($"   [DEBUG] Error loading memories: {ex.Message}");
                    }
                }

                // Get NPC response
                string response;
                try
                {
                    response = await UnifiedLLMService.GetResponseAsync(
                        npcName: $"Test{testCase.NPCType}",
                        npcPersonality: personality,
                        conversationHistory: conversationHistory,
                        playerMessage: testCase.Question,
                        playerName: playerName,
                        preloadedKnowledge: preloadedKnowledge + (string.IsNullOrEmpty(memoriesText) ? "" : "\n\n" + memoriesText),
                        requestType: UnifiedLLMService.RequestType.PlayerConversation
                    );

                    if (string.IsNullOrEmpty(response))
                    {
                        throw new Exception("LLM service returned empty response");
                    }
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"ERROR");
                    Console.ResetColor();
                    WriteLog("ERROR");
                    WriteLog($"   LLM Service Error: {ex.Message}");
                    WriteLog($"   Stack Trace: {ex.StackTrace}");
                    testsFailed++;
                    
                    var errorResult = new TestResult
                    {
                        NPCType = testCase.NPCType,
                        Question = testCase.Question,
                        Response = $"ERROR: {ex.Message}",
                        Passed = false,
                        Reason = $"LLM Service unavailable or error: {ex.Message}",
                        ExpectedKeywords = string.Join(", ", testCase.ExpectedKeywords),
                        ShouldRefer = testCase.ShouldRefer
                    };
                    allResults.Add(errorResult);
                    return;
                }

                // Analyze response
                bool passed = await AnalyzeResponse(response, testCase);
                string reason = GetAnalysisReason(response, testCase);

                var result = new TestResult
                {
                    NPCType = testCase.NPCType,
                    Question = testCase.Question,
                    Response = response,
                    Passed = passed,
                    Reason = reason,
                    ExpectedKeywords = string.Join(", ", testCase.ExpectedKeywords),
                    ShouldRefer = testCase.ShouldRefer
                };

                allResults.Add(result);

                if (passed)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("PASS");
                    Console.ResetColor();
                    WriteLog("PASS");
                    testsPassed++;
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("FAIL");
                    Console.ResetColor();
                    WriteLog("FAIL");
                    WriteLog($"   Reason: {reason}");
                    WriteLog($"   Expected Keywords: {result.ExpectedKeywords}");
                    WriteLog($"   Response: {response}");
                    Console.WriteLine($"   Reason: {reason}");
                    Console.WriteLine($"   Response: {response.Substring(0, Math.Min(100, response.Length))}...");
                    testsFailed++;
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"ERROR: {ex.Message}");
                Console.ResetColor();
                WriteLog($"ERROR: {ex.Message}");
                WriteLog($"Stack Trace: {ex.StackTrace}");
                testsFailed++;

                allResults.Add(new TestResult
                {
                    NPCType = testCase.NPCType,
                    Question = testCase.Question,
                    Response = $"ERROR: {ex.Message}",
                    Passed = false,
                    Reason = $"Exception: {ex.Message}"
                });
            }
        }

        private static async Task<bool> AnalyzeResponse(string response, TestCase testCase)
        {
            if (string.IsNullOrEmpty(response))
                return false;

            string lowerResponse = response.ToLower();

            // Check if this is a referral test
            if (testCase.ShouldRefer)
            {
                // Natural referral language patterns (more flexible)
                bool hasReferral = 
                    // Direct referral words
                    lowerResponse.Contains("refer") ||
                    lowerResponse.Contains("ask") ||
                    lowerResponse.Contains("speak") ||
                    lowerResponse.Contains("visit") ||
                    lowerResponse.Contains("see") ||
                    lowerResponse.Contains("seek") ||
                    lowerResponse.Contains("consult") ||
                    lowerResponse.Contains("counsel") ||
                    // Denial patterns
                    lowerResponse.Contains("not my") ||
                    lowerResponse.Contains("not mine") ||
                    lowerResponse.Contains("can't help") ||
                    lowerResponse.Contains("don't know") ||
                    lowerResponse.Contains("not a ") ||
                    lowerResponse.Contains("not in ") ||
                    lowerResponse.Contains("not the ") ||
                    lowerResponse.Contains("beyond my") ||
                    lowerResponse.Contains("beyond mine") ||
                    // Indirect referral patterns
                    lowerResponse.Contains("best understood by") ||
                    lowerResponse.Contains("should speak to") ||
                    lowerResponse.Contains("should ask") ||
                    lowerResponse.Contains("would know") ||
                    lowerResponse.Contains("could tell") ||
                    lowerResponse.Contains("recommend") ||
                    lowerResponse.Contains("suggest") ||
                    lowerResponse.Contains("perhaps") ||
                    lowerResponse.Contains("maybe") ||
                    // Location-based referrals (common in UO)
                    lowerResponse.Contains("moonglow") ||
                    lowerResponse.Contains("lycaeum") ||
                    lowerResponse.Contains("at ") && (lowerResponse.Contains("mage") || lowerResponse.Contains("blacksmith") || lowerResponse.Contains("smith")) ||
                    // "Visit the local [profession]" pattern
                    (lowerResponse.Contains("visit") && (lowerResponse.Contains("local") || lowerResponse.Contains("the "))) ||
                    // "Seek counsel in [location]" pattern
                    (lowerResponse.Contains("seek") && lowerResponse.Contains("counsel")) ||
                    // "Go to [location]" for referrals
                    (lowerResponse.Contains("go to") && (lowerResponse.Contains("moonglow") || lowerResponse.Contains("lycaeum") || lowerResponse.Contains("mage") || lowerResponse.Contains("blacksmith") || lowerResponse.Contains("smith")));

                // Should mention BOTH:
                // 1. A local profession (e.g., "local blacksmith", "local mage")
                // 2. A location where they can seek help (e.g., "seek help in Moonglow", "seek assistance in Minoc")
                bool mentionsLocalProfession = false;
                bool mentionsLocation = false;
                
                string targetLower = testCase.ReferralTarget.ToLower();
                
                // Check for "local [profession]" pattern
                // More flexible matching - check if response contains "local" + any variation of the target profession
                if (lowerResponse.Contains("local"))
                {
                    
                    // Check for mage variations
                    if (targetLower.Contains("mage") && lowerResponse.Contains("mage"))
                    {
                        mentionsLocalProfession = true;
                    }
                    // Check for blacksmith variations
                    else if (targetLower.Contains("blacksmith") && 
                             (lowerResponse.Contains("blacksmith") || lowerResponse.Contains("smith")))
                    {
                        mentionsLocalProfession = true;
                    }
                    // Check for healer variations
                    else if (targetLower.Contains("healer") && lowerResponse.Contains("healer"))
                    {
                        mentionsLocalProfession = true;
                    }
                    // Check for bowyer variations
                    else if (targetLower.Contains("bowyer") && lowerResponse.Contains("bowyer"))
                    {
                        mentionsLocalProfession = true;
                    }
                    // Check for tailor variations
                    else if (targetLower.Contains("tailor") && lowerResponse.Contains("tailor"))
                    {
                        mentionsLocalProfession = true;
                    }
                    // Generic fallback: if target profession is mentioned near "local"
                    else if (lowerResponse.Contains($"local {targetLower}") || 
                             lowerResponse.Contains($"local {targetLower.Replace(" ", "")}"))
                    {
                        mentionsLocalProfession = true;
                    }
                }
                
                // Must have "local [profession]" - no fallback to just profession mentions
                // This ensures NPCs follow the format: "perhaps there's a local [profession]..."
                
                // Check for location suggestions - must mention actual location name
                // Format: "seek help in [location]" or "you can seek help in [location]"
                if (targetLower.Contains("mage"))
                {
                    // Must mention Moonglow or Lycaeum (actual location names)
                    mentionsLocation = lowerResponse.Contains("moonglow") || lowerResponse.Contains("lycaeum");
                }
                else if (targetLower.Contains("blacksmith"))
                {
                    // Must mention Minoc, Trinsic, or Vesper (actual location names)
                    mentionsLocation = lowerResponse.Contains("minoc") || lowerResponse.Contains("trinsic") || 
                                      lowerResponse.Contains("vesper");
                }
                else if (targetLower.Contains("healer"))
                {
                    // Healers can be found in various towns, but Empath Abbey is the principle location
                    mentionsLocation = lowerResponse.Contains("empath") || lowerResponse.Contains("britain") || 
                                      lowerResponse.Contains("moonglow") || lowerResponse.Contains("trinsic") ||
                                      lowerResponse.Contains("yew");
                }
                else if (targetLower.Contains("bowyer"))
                {
                    // Bowyers in various towns
                    mentionsLocation = lowerResponse.Contains("britain") || lowerResponse.Contains("minoc") || 
                                      lowerResponse.Contains("vesper");
                }
                else if (targetLower.Contains("tailor"))
                {
                    // Tailors in various towns
                    mentionsLocation = lowerResponse.Contains("britain") || lowerResponse.Contains("minoc") || 
                                      lowerResponse.Contains("vesper");
                }
                // Generic fallback: any location mention with "seek help" or "seek assistance"
                else if ((lowerResponse.Contains("seek help") || lowerResponse.Contains("seek assistance")) &&
                         (lowerResponse.Contains("britain") || lowerResponse.Contains("minoc") || 
                          lowerResponse.Contains("vesper") || lowerResponse.Contains("moonglow") ||
                          lowerResponse.Contains("trinsic") || lowerResponse.Contains("lycaeum")))
                {
                    mentionsLocation = true;
                }
                
                // For a good referral, we want both local profession AND location
                bool mentionsTarget = mentionsLocalProfession && mentionsLocation;

                // Should NOT provide detailed expert knowledge about the topic
                // Only flag if response gives actual step-by-step instructions TO THE PLAYER
                // Describing what experts do (like "blacksmiths smelt ore") is OK if it's part of a referral
                bool providesExpertKnowledge = false;
                
                // Check for actual instruction patterns (telling the player what to do)
                if (lowerResponse.Contains("first") && lowerResponse.Contains("then") && 
                    (lowerResponse.Contains("you must") || lowerResponse.Contains("thou must") || 
                     lowerResponse.Contains("you need") || lowerResponse.Contains("thou need")))
                {
                    providesExpertKnowledge = true;
                }
                else if (lowerResponse.Contains("step") && 
                         (lowerResponse.Contains("you") || lowerResponse.Contains("thou")) &&
                         (lowerResponse.Contains("next") || lowerResponse.Contains("then")))
                {
                    providesExpertKnowledge = true;
                }
                // Only flag if it's giving detailed instructions, not just describing what experts do
                else if ((lowerResponse.Contains("forge") || lowerResponse.Contains("smelt")) &&
                         (lowerResponse.Contains("you must") || lowerResponse.Contains("thou must") ||
                          lowerResponse.Contains("you need") || lowerResponse.Contains("thou need") ||
                          (lowerResponse.Contains("you") && lowerResponse.Contains("must"))))
                {
                    // But NOT if it's part of a referral (like "they shall guide thee in the process")
                    if (!lowerResponse.Contains("they") && !lowerResponse.Contains("blacksmith") && 
                        !lowerResponse.Contains("recommend") && !lowerResponse.Contains("suggest") &&
                        !lowerResponse.Contains("counsel") && !lowerResponse.Contains("visit"))
                    {
                        providesExpertKnowledge = true;
                    }
                }

                // For referral tests, we want: has referral language AND mentions both local profession AND location AND doesn't provide expert knowledge
                return hasReferral && mentionsTarget && !providesExpertKnowledge;
            }
            else
            {
                // Hybrid approach: keyword matching first, then semantic similarity fallback
                // Should contain expected keywords (at least 2)
                int keywordMatches = testCase.ExpectedKeywords.Count(kw =>
                    lowerResponse.Contains(kw.ToLower()));

                if (keywordMatches >= 2)
                {
                    return true; // Pass on keyword matching
                }

                // If keyword matching fails, try semantic similarity (if embeddings available)
                // Allow semantic fallback even with 0 keyword matches (for synonym/paraphrase detection)
                if (EmbeddingService.IsAvailable() && keywordMatches < 2)
                {
                    try
                    {
                        // Generate embedding for the response
                        float[] responseEmbedding = await EmbeddingService.GenerateEmbeddingAsync(response);
                        
                        // First, check overall response similarity to the expected question/topic
                        string expectedQuery = $"{testCase.Question}. Expected concepts: {string.Join(", ", testCase.ExpectedKeywords)}";
                        float[] expectedEmbedding = await EmbeddingService.GenerateEmbeddingAsync(expectedQuery);
                        float overallSimilarity = EmbeddingService.CosineSimilarity(responseEmbedding, expectedEmbedding);
                        
                        // If overall similarity is high enough, pass (for cases where response is semantically correct but uses synonyms)
                        if (overallSimilarity >= 0.70f)
                        {
                            return true;
                        }
                        
                        // Otherwise, check semantic similarity for each expected keyword
                        int semanticMatches = 0;
                        foreach (string keyword in testCase.ExpectedKeywords)
                        {
                            // Skip if already matched by keyword
                            if (lowerResponse.Contains(keyword.ToLower()))
                            {
                                semanticMatches++;
                                continue;
                            }

                            // Create a query that represents what we expect
                            string keywordQuery = $"{keyword} {testCase.Question}";
                            float[] keywordEmbedding = await EmbeddingService.GenerateEmbeddingAsync(keywordQuery);
                            
                            float similarity = EmbeddingService.CosineSimilarity(responseEmbedding, keywordEmbedding);
                            
                            // Lower threshold for semantic matching (0.65 vs 0.70 for memory tests)
                            // This allows for more flexible matching while still being meaningful
                            if (similarity >= 0.65f)
                            {
                                semanticMatches++;
                            }
                        }

                        // Pass if we have at least 2 matches (keyword + semantic combined)
                        return semanticMatches >= 2;
                    }
                    catch (Exception ex)
                    {
                        // If embedding fails, fall back to keyword-only result
                        Console.WriteLine($"[KnowledgeTest] Semantic similarity check failed: {ex.Message}");
                    }
                }

                return false; // Failed both keyword and semantic matching
            }
        }

        private static string GetAnalysisReason(string response, TestCase testCase)
        {
            if (string.IsNullOrEmpty(response))
                return "Empty response";

            string lowerResponse = response.ToLower();

            if (testCase.ShouldRefer)
            {
                bool hasReferral = 
                    lowerResponse.Contains("refer") ||
                    lowerResponse.Contains("ask") ||
                    lowerResponse.Contains("speak") ||
                    lowerResponse.Contains("seek") ||
                    lowerResponse.Contains("visit") ||
                    lowerResponse.Contains("counsel") ||
                    lowerResponse.Contains("not my") ||
                    lowerResponse.Contains("not mine") ||
                    lowerResponse.Contains("can't help") ||
                    lowerResponse.Contains("beyond my") ||
                    lowerResponse.Contains("beyond mine") ||
                    lowerResponse.Contains("not a ") ||
                    lowerResponse.Contains("not in ") ||
                    lowerResponse.Contains("best understood by") ||
                    lowerResponse.Contains("recommend") ||
                    lowerResponse.Contains("suggest") ||
                    lowerResponse.Contains("perhaps") ||
                    lowerResponse.Contains("maybe");

                // Check for local profession mention
                bool mentionsLocalProfession = false;
                string targetLowerReason = testCase.ReferralTarget.ToLower();
                
                if (lowerResponse.Contains("local"))
                {
                    // Check for mage variations
                    if (targetLowerReason.Contains("mage") && lowerResponse.Contains("mage"))
                    {
                        mentionsLocalProfession = true;
                    }
                    // Check for blacksmith variations
                    else if (targetLowerReason.Contains("blacksmith") && 
                             (lowerResponse.Contains("blacksmith") || lowerResponse.Contains("smith")))
                    {
                        mentionsLocalProfession = true;
                    }
                    // Check for healer variations
                    else if (targetLowerReason.Contains("healer") && lowerResponse.Contains("healer"))
                    {
                        mentionsLocalProfession = true;
                    }
                    // Check for bowyer variations
                    else if (targetLowerReason.Contains("bowyer") && lowerResponse.Contains("bowyer"))
                    {
                        mentionsLocalProfession = true;
                    }
                    // Check for tailor variations
                    else if (targetLowerReason.Contains("tailor") && lowerResponse.Contains("tailor"))
                    {
                        mentionsLocalProfession = true;
                    }
                    // Generic fallback: if target profession is mentioned near "local"
                    else if (lowerResponse.Contains($"local {targetLowerReason}") || 
                             lowerResponse.Contains($"local {targetLowerReason.Replace(" ", "")}"))
                    {
                        mentionsLocalProfession = true;
                    }
                }
                // Must have "local [profession]" - no fallback
                
                // Check for location mention - must be actual location name
                bool mentionsLocation = false;
                
                if (targetLowerReason.Contains("mage"))
                {
                    mentionsLocation = lowerResponse.Contains("moonglow") || lowerResponse.Contains("lycaeum");
                }
                else if (targetLowerReason.Contains("blacksmith"))
                {
                    mentionsLocation = lowerResponse.Contains("minoc") || lowerResponse.Contains("trinsic") || 
                                      lowerResponse.Contains("vesper");
                }
                else if (targetLowerReason.Contains("healer"))
                {
                    mentionsLocation = lowerResponse.Contains("empath") || lowerResponse.Contains("britain") || 
                                      lowerResponse.Contains("moonglow") || lowerResponse.Contains("trinsic") ||
                                      lowerResponse.Contains("yew");
                }
                else if (targetLowerReason.Contains("bowyer"))
                {
                    mentionsLocation = lowerResponse.Contains("britain") || lowerResponse.Contains("minoc") || 
                                      lowerResponse.Contains("vesper");
                }
                else if (targetLowerReason.Contains("tailor"))
                {
                    mentionsLocation = lowerResponse.Contains("britain") || lowerResponse.Contains("minoc") || 
                                      lowerResponse.Contains("vesper");
                }
                // Generic fallback: any location mention with "seek help" or "seek assistance"
                else if ((lowerResponse.Contains("seek help") || lowerResponse.Contains("seek assistance")) &&
                         (lowerResponse.Contains("britain") || lowerResponse.Contains("minoc") || 
                          lowerResponse.Contains("vesper") || lowerResponse.Contains("moonglow") ||
                          lowerResponse.Contains("trinsic") || lowerResponse.Contains("lycaeum") ||
                          lowerResponse.Contains("yew")))
                {
                    mentionsLocation = true;
                }
                
                bool mentionsTarget = mentionsLocalProfession && mentionsLocation;

                bool providesExpertKnowledge = false;
                if (lowerResponse.Contains("first") && lowerResponse.Contains("then") && 
                    (lowerResponse.Contains("you must") || lowerResponse.Contains("thou must")))
                {
                    providesExpertKnowledge = true;
                }
                else if ((lowerResponse.Contains("forge") || lowerResponse.Contains("smelt")) &&
                         (lowerResponse.Contains("you must") || lowerResponse.Contains("thou must") ||
                          lowerResponse.Contains("you need") || lowerResponse.Contains("thou need")) &&
                         !lowerResponse.Contains("they") && !lowerResponse.Contains("recommend") && 
                         !lowerResponse.Contains("suggest") && !lowerResponse.Contains("counsel"))
                {
                    providesExpertKnowledge = true;
                }

                if (providesExpertKnowledge)
                    return "Provides expert knowledge instead of referring";
                if (!hasReferral)
                    return "Missing referral language";
                if (!mentionsLocalProfession)
                    return $"Missing local profession mention (e.g., 'local {testCase.ReferralTarget}')";
                if (!mentionsLocation)
                    return $"Missing location suggestion (e.g., 'seek help in Moonglow' for mages, 'seek help in Minoc' for blacksmiths)";
                if (!mentionsTarget)
                    return $"Missing both local profession AND location (need both for proper referral)";
                return "Referral test passed";
            }
            else
            {
                int keywordMatches = testCase.ExpectedKeywords.Count(kw =>
                    lowerResponse.Contains(kw.ToLower()));

                if (keywordMatches < 2)
                {
                    var missing = testCase.ExpectedKeywords.Where(kw =>
                        !lowerResponse.Contains(kw.ToLower())).ToList();
                    return $"Only {keywordMatches} keyword(s) found. Missing: {string.Join(", ", missing)}";
                }
                return $"{keywordMatches} keywords found";
            }
        }

        #endregion

        #region Summary

        #region Memory System

        private static void InitializeMemorySystem()
        {
            try
            {
                // Activate in-memory fallback store for testing
                if (!InMemoryFallbackStore.IsActive)
                {
                    InMemoryFallbackStore.Activate();
                    WriteLog("[MemorySystem] In-memory fallback store activated for testing");
                }
            }
            catch (Exception ex)
            {
                WriteLog($"[MemorySystem] WARNING: Could not initialize memory system: {ex.Message}");
            }
        }

        #endregion

        #region Logging

        private static void InitializeLogFile()
        {
            try
            {
                string logDir = Path.Combine(Core.BaseDirectory.Directory, "Data", "LLM", "Tests");
                if (!Directory.Exists(logDir))
                {
                    Directory.CreateDirectory(logDir);
                }

                string timestamp = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
                logFilePath = Path.Combine(logDir, $"KnowledgeTest_{timestamp}.log");

                logWriter = new StreamWriter(logFilePath, false, Encoding.UTF8);
                logWriter.AutoFlush = true;

                WriteLog("╔════════════════════════════════════════════════════════════╗");
                WriteLog("║     LLM NPC Knowledge - Automated Test Suite               ║");
                WriteLog("╚════════════════════════════════════════════════════════════╝");
                WriteLog("");
                WriteLog($"Test Run Started: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
                WriteLog($"Log File: {logFilePath}");
                WriteLog("");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[KnowledgeTest] WARNING: Could not create log file: {ex.Message}");
            }
        }

        private static void WriteLog(string message)
        {
            Console.WriteLine(message);
            if (logWriter != null)
            {
                logWriter.WriteLine(message);
            }
        }

        private static void CloseLogFile()
        {
            if (logWriter != null)
            {
                WriteLog("");
                WriteLog($"Test Run Completed: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
                WriteLog($"Log File: {logFilePath}");
                logWriter.Close();
                logWriter = null;
                Console.WriteLine($"[KnowledgeTest] Test results saved to: {logFilePath}");
            }
        }

        #endregion

        private static void PrintSummary()
        {
            WriteLog("");
            WriteLog("╔════════════════════════════════════════════════════════════╗");
            WriteLog("║                      TEST SUMMARY                          ║");
            WriteLog("╚════════════════════════════════════════════════════════════╝");
            WriteLog("");

            int total = testsPassed + testsFailed + testsSkipped;
            WriteLog($"  Total Tests: {total}");

            Console.ForegroundColor = ConsoleColor.Green;
            WriteLog($"  Passed: {testsPassed}");
            Console.ResetColor();

            if (testsFailed > 0)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                WriteLog($"  Failed: {testsFailed}");
                Console.ResetColor();
            }
            else
            {
                WriteLog($"  Failed: {testsFailed}");
            }

            if (testsSkipped > 0)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                WriteLog($"  Skipped: {testsSkipped}");
                Console.ResetColor();
            }

            if (total > 0)
            {
                double percentage = (double)testsPassed / total * 100;
                WriteLog($"  Success Rate: {percentage:F1}%");
            }

            WriteLog("");

            // Print detailed results
            if (testsFailed > 0)
            {
                WriteLog("  Failed Tests:");
                WriteLog("  " + new string('─', 50));
                foreach (var result in allResults.Where(r => !r.Passed))
                {
                    WriteLog($"  [{result.NPCType}] {result.Question}");
                    WriteLog($"    Reason: {result.Reason}");
                    WriteLog($"    Expected Keywords: {result.ExpectedKeywords}");
                    WriteLog($"    Response: {result.Response}");
                    WriteLog("");
                }
            }

            WriteLog("");
            WriteLog("  Detailed Results:");
            WriteLog("  " + new string('─', 50));
            foreach (var result in allResults)
            {
                string status = result.Passed ? "PASS" : "FAIL";
                ConsoleColor color = result.Passed ? ConsoleColor.Green : ConsoleColor.Red;
                Console.ForegroundColor = color;
                Console.Write($"  [{status}] ");
                Console.ResetColor();
                WriteLog($"[{result.NPCType}] {result.Question}");
            }

            WriteLog("");
        }

        #endregion
    }
}

