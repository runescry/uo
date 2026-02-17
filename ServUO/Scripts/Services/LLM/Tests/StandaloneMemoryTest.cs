using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Server.Services.LLM.Tests
{
    /// <summary>
    /// Standalone memory system test that can run without the game
    /// Simulates complete conversation flows with detailed output
    /// </summary>
    public class StandaloneMemoryTest
    {
        private static int testsPassed = 0;
        private static int testsFailed = 0;

        public static async Task RunAllTests()
        {
            Console.WriteLine("╔════════════════════════════════════════════════════════════╗");
            Console.WriteLine("║       LLM Memory System - Standalone Test Suite           ║");
            Console.WriteLine("╚════════════════════════════════════════════════════════════╝");
            Console.WriteLine();

            // Initialize memory system (simulate what happens on server start)
            Console.WriteLine("[INIT] Initializing memory system...");
            InMemoryFallbackStore.Activate();
            Console.WriteLine("[INIT] In-memory fallback activated");
            Console.WriteLine();

            // Run all test suites
            await TestSuite1_BasicMemoryOperations();
            await TestSuite2_RelationshipSystem();
            await TestSuite3_ImportanceScoring();
            await TestSuite4_ConversationSimulation();
            await TestSuite5_RealWorldScenario();

            // Print summary
            PrintSummary();
        }

        #region Test Suite 1: Basic Memory Operations

        private static async Task TestSuite1_BasicMemoryOperations()
        {
            Console.WriteLine("═══════════════════════════════════════════════════════════");
            Console.WriteLine("TEST SUITE 1: Basic Memory Operations");
            Console.WriteLine("═══════════════════════════════════════════════════════════");
            Console.WriteLine();

            // Test 1.1: Save and retrieve a single memory
            await RunTest("1.1 Save and Retrieve Memory", async () =>
            {
                var memory = new Memory
                {
                    NpcName = "TestNPC",
                    PlayerName = "TestPlayer",
                    Type = MemoryType.Fact,
                    Content = "Player introduced themselves",
                    Importance = 8,
                    CreatedAt = DateTime.UtcNow,
                    LastAccessed = DateTime.UtcNow
                };

                await InMemoryFallbackStore.SaveMemoryAsync(memory);
                var retrieved = await InMemoryFallbackStore.GetMemoriesAsync("TestNPC", "TestPlayer", 10);

                Assert(retrieved.Count > 0, "Memory should be retrievable");
                Assert(retrieved[0].Content == "Player introduced themselves", "Content should match");
                Console.WriteLine($"   ✓ Memory saved and retrieved: '{retrieved[0].Content}'");
                return true;
            });

            // Test 1.2: Multiple memories sorted by importance
            await RunTest("1.2 Multiple Memories Sorted", async () =>
            {
                string npc = "SortTestNPC";
                string player = "SortTestPlayer";

                // Create memories with different importance
                await InMemoryFallbackStore.SaveMemoryAsync(new Memory
                {
                    NpcName = npc, PlayerName = player, Type = MemoryType.Conversation,
                    Content = "Low importance", Importance = 2, CreatedAt = DateTime.UtcNow, LastAccessed = DateTime.UtcNow
                });

                await InMemoryFallbackStore.SaveMemoryAsync(new Memory
                {
                    NpcName = npc, PlayerName = player, Type = MemoryType.Fact,
                    Content = "High importance", Importance = 9, CreatedAt = DateTime.UtcNow, LastAccessed = DateTime.UtcNow
                });

                await InMemoryFallbackStore.SaveMemoryAsync(new Memory
                {
                    NpcName = npc, PlayerName = player, Type = MemoryType.Preference,
                    Content = "Medium importance", Importance = 5, CreatedAt = DateTime.UtcNow, LastAccessed = DateTime.UtcNow
                });

                var retrieved = await InMemoryFallbackStore.GetMemoriesAsync(npc, player, 10);

                Assert(retrieved.Count == 3, "Should have 3 memories");
                Assert(retrieved[0].Importance == 9, "First memory should be most important");
                Assert(retrieved[0].Content == "High importance", "First memory should be highest importance");

                Console.WriteLine($"   ✓ Memories sorted correctly:");
                foreach (var mem in retrieved)
                {
                    Console.WriteLine($"     - [{mem.Type}] Importance {mem.Importance}: {mem.Content}");
                }

                return true;
            });

            // Test 1.3: Memory pruning (max 50)
            await RunTest("1.3 Memory Pruning", async () =>
            {
                string npc = "PruneNPC";
                string player = "PrunePlayer";

                // Add 60 memories
                for (int i = 0; i < 60; i++)
                {
                    await InMemoryFallbackStore.SaveMemoryAsync(new Memory
                    {
                        NpcName = npc, PlayerName = player, Type = MemoryType.Conversation,
                        Content = $"Memory {i}", Importance = i % 10, CreatedAt = DateTime.UtcNow, LastAccessed = DateTime.UtcNow
                    });
                }

                var retrieved = await InMemoryFallbackStore.GetMemoriesAsync(npc, player, 100);

                Assert(retrieved.Count <= 50, $"Should prune to max 50, got {retrieved.Count}");
                Console.WriteLine($"   ✓ Created 60 memories, pruned to {retrieved.Count}");

                return true;
            });

            Console.WriteLine();
        }

        #endregion

        #region Test Suite 2: Relationship System

        private static async Task TestSuite2_RelationshipSystem()
        {
            Console.WriteLine("═══════════════════════════════════════════════════════════");
            Console.WriteLine("TEST SUITE 2: Relationship System");
            Console.WriteLine("═══════════════════════════════════════════════════════════");
            Console.WriteLine();

            // Test 2.1: Create new relationship
            await RunTest("2.1 Create New Relationship", async () =>
            {
                string npc = "RelNPC1";
                string player = "RelPlayer1";

                var relationship = new Relationship
                {
                    NpcName = npc,
                    PlayerName = player,
                    Type = RelationshipType.Stranger,
                    Score = 5,
                    FirstMet = DateTime.UtcNow,
                    LastInteraction = DateTime.UtcNow,
                    InteractionCount = 1
                };

                await InMemoryFallbackStore.SaveRelationshipAsync(relationship);
                var retrieved = await InMemoryFallbackStore.GetRelationshipAsync(npc, player);

                Assert(retrieved != null, "Relationship should exist");
                Assert(retrieved.Score == 5, "Score should match");
                Assert(retrieved.Type == RelationshipType.Stranger, "Should be Stranger");

                Console.WriteLine($"   ✓ Relationship created: {retrieved.Type} (Score: {retrieved.Score})");
                return true;
            });

            // Test 2.2: Relationship progression
            await RunTest("2.2 Relationship Progression", async () =>
            {
                string npc = "RelNPC2";
                string player = "RelPlayer2";

                // Start as Stranger
                var rel = new Relationship
                {
                    NpcName = npc, PlayerName = player,
                    Type = RelationshipType.Stranger, Score = 0,
                    FirstMet = DateTime.UtcNow, LastInteraction = DateTime.UtcNow,
                    InteractionCount = 0
                };

                await InMemoryFallbackStore.SaveRelationshipAsync(rel);
                Console.WriteLine($"   Initial: {rel.Type} (Score: {rel.Score})");

                // Simulate 25 positive interactions
                for (int i = 0; i < 25; i++)
                {
                    var current = await InMemoryFallbackStore.GetRelationshipAsync(npc, player);
                    current.Score += 1;
                    current.InteractionCount++;
                    current.LastInteraction = DateTime.UtcNow;

                    // Update type based on score
                    if (current.Score >= 21) current.Type = RelationshipType.Acquaintance;
                    if (current.Score >= 41) current.Type = RelationshipType.Friend;
                    if (current.Score >= 61) current.Type = RelationshipType.CloseFriend;
                    if (current.Score >= 81) current.Type = RelationshipType.Ally;

                    await InMemoryFallbackStore.SaveRelationshipAsync(current);
                }

                var final = await InMemoryFallbackStore.GetRelationshipAsync(npc, player);

                Assert(final.Score == 25, "Score should be 25");
                Assert(final.Type == RelationshipType.Acquaintance, "Should be Acquaintance at 25");
                Assert(final.InteractionCount == 25, "Should have 25 interactions");

                Console.WriteLine($"   After 25 interactions: {final.Type} (Score: {final.Score})");
                return true;
            });

            // Test 2.3: Relationship types at different scores
            await RunTest("2.3 Relationship Type Thresholds", async () =>
            {
                var tests = new[]
                {
                    new { Score = 10, Expected = RelationshipType.Stranger },
                    new { Score = 25, Expected = RelationshipType.Acquaintance },
                    new { Score = 50, Expected = RelationshipType.Friend },
                    new { Score = 70, Expected = RelationshipType.CloseFriend },
                    new { Score = 90, Expected = RelationshipType.Ally }
                };

                foreach (var test in tests)
                {
                    RelationshipType type = GetRelationshipTypeFromScore(test.Score);
                    Assert(type == test.Expected, $"Score {test.Score} should be {test.Expected}, got {type}");
                    Console.WriteLine($"   ✓ Score {test.Score:D2} = {type}");
                }

                return await Task.FromResult(true);
            });

            Console.WriteLine();
        }

        private static RelationshipType GetRelationshipTypeFromScore(int score)
        {
            if (score >= 81) return RelationshipType.Ally;
            else if (score >= 61) return RelationshipType.CloseFriend;
            else if (score >= 41) return RelationshipType.Friend;
            else if (score >= 21) return RelationshipType.Acquaintance;
            else if (score <= -10) return RelationshipType.Enemy;
            else return RelationshipType.Stranger;
        }

        #endregion

        #region Test Suite 3: Importance Scoring

        private static async Task TestSuite3_ImportanceScoring()
        {
            Console.WriteLine("═══════════════════════════════════════════════════════════");
            Console.WriteLine("TEST SUITE 3: Memory Importance Scoring");
            Console.WriteLine("═══════════════════════════════════════════════════════════");
            Console.WriteLine();

            await RunTest("3.1 Importance Scoring Algorithm", async () =>
            {
                var testCases = new[]
                {
                    new { Message = "Remember this important fact!", MinScore = 9, Description = "Explicit remember" },
                    new { Message = "Don't forget what I'm about to tell you", MinScore = 9, Description = "Don't forget" },
                    new { Message = "My name is Aldric", MinScore = 8, Description = "Name introduction" },
                    new { Message = "I am a warrior", MinScore = 8, Description = "Identity" },
                    new { Message = "I'm looking for a quest", MinScore = 7, Description = "Quest seeking" },
                    new { Message = "I have a mission for you", MinScore = 7, Description = "Mission" },
                    new { Message = "I hate orcs with a passion", MinScore = 6, Description = "Strong hatred" },
                    new { Message = "I really dislike spiders", MinScore = 6, Description = "Dislike" },
                    new { Message = "I love crafting weapons", MinScore = 5, Description = "Preference" },
                    new { Message = "My favorite food is bread", MinScore = 5, Description = "Favorite" },
                    new { Message = "This is a longer conversation about various things", MinScore = 2, Description = "General chat" },
                    new { Message = "Hello", MinScore = 0, Description = "Greeting (not saved)" },
                    new { Message = "Hi", MinScore = 0, Description = "Short greeting" }
                };

                Console.WriteLine("   Testing importance scoring:");
                foreach (var test in testCases)
                {
                    bool shouldSave = MemoryHelpers.ShouldSaveMemory(test.Message, "", out int importance);

                    if (test.MinScore == 0)
                    {
                        Assert(!shouldSave, $"{test.Description} should not be saved");
                        Console.WriteLine($"   ✓ '{test.Message}' → Not saved (correct)");
                    }
                    else
                    {
                        Assert(shouldSave, $"{test.Description} should be saved");
                        Assert(importance >= test.MinScore, $"{test.Description}: Expected >={test.MinScore}, got {importance}");
                        Console.WriteLine($"   ✓ '{test.Message}' → Importance: {importance} (≥{test.MinScore})");
                    }
                }

                return await Task.FromResult(true);
            });

            Console.WriteLine();
        }

        #endregion

        #region Test Suite 4: Conversation Simulation

        private static async Task TestSuite4_ConversationSimulation()
        {
            Console.WriteLine("═══════════════════════════════════════════════════════════");
            Console.WriteLine("TEST SUITE 4: Conversation Flow Simulation");
            Console.WriteLine("═══════════════════════════════════════════════════════════");
            Console.WriteLine();

            await RunTest("4.1 Multi-Turn Conversation", async () =>
            {
                string npc = "Aldric the Blacksmith";
                string player = "Runescry";

                Console.WriteLine($"\n   Simulating conversation between {player} and {npc}:");
                Console.WriteLine("   " + new string('─', 55));

                // Turn 1: Introduction
                await SimulateConversationTurn(npc, player, "Hello! My name is Runescry", 1);

                // Turn 2: Preference
                await SimulateConversationTurn(npc, player, "I love crafting swords and armor", 2);

                // Turn 3: Quest
                await SimulateConversationTurn(npc, player, "Do you have any quests for me?", 3);

                // Turn 4: Small talk
                await SimulateConversationTurn(npc, player, "Nice forge you have here", 4);

                // Turn 5: Remember request
                await SimulateConversationTurn(npc, player, "Remember, I'm allergic to silver", 5);

                Console.WriteLine("   " + new string('─', 55));

                // Check final state
                var memories = await InMemoryFallbackStore.GetMemoriesAsync(npc, player, 10);
                var relationship = await InMemoryFallbackStore.GetRelationshipAsync(npc, player);

                Console.WriteLine($"\n   Final State:");
                Console.WriteLine($"   - Memories saved: {memories.Count}");
                Console.WriteLine($"   - Relationship: {relationship.Type} (Score: {relationship.Score})");
                Console.WriteLine($"   - Interactions: {relationship.InteractionCount}");

                Console.WriteLine($"\n   Memories:");
                foreach (var mem in memories)
                {
                    Console.WriteLine($"   - [{mem.Type}] Importance {mem.Importance}: {mem.Content}");
                }

                Assert(memories.Count >= 3, "Should have at least 3 important memories");
                Assert(relationship.InteractionCount == 5, "Should have 5 interactions");

                return true;
            });

            Console.WriteLine();
        }

        private static async Task SimulateConversationTurn(string npc, string player, string message, int turn)
        {
            Console.WriteLine($"\n   Turn {turn}:");
            Console.WriteLine($"   {player}: \"{message}\"");

            // Check if memory should be saved
            bool shouldSave = MemoryHelpers.ShouldSaveMemory(message, "", out int importance);

            if (shouldSave)
            {
                var memory = new Memory
                {
                    NpcName = npc,
                    PlayerName = player,
                    Type = DetermineMemoryType(message),
                    Content = $"{player} said: \"{message}\"",
                    Importance = importance,
                    CreatedAt = DateTime.UtcNow,
                    LastAccessed = DateTime.UtcNow
                };

                await InMemoryFallbackStore.SaveMemoryAsync(memory);
                Console.WriteLine($"   → Memory saved: [{memory.Type}] Importance: {importance}");
            }
            else
            {
                Console.WriteLine($"   → Memory not saved (low importance)");
            }

            // Update relationship
            var relationship = await InMemoryFallbackStore.GetRelationshipAsync(npc, player);
            if (relationship == null)
            {
                relationship = new Relationship
                {
                    NpcName = npc,
                    PlayerName = player,
                    Type = RelationshipType.Stranger,
                    Score = 1,
                    FirstMet = DateTime.UtcNow,
                    LastInteraction = DateTime.UtcNow,
                    InteractionCount = 1
                };
            }
            else
            {
                relationship.Score += 1;
                relationship.InteractionCount++;
                relationship.LastInteraction = DateTime.UtcNow;
                relationship.Type = GetRelationshipTypeFromScore(relationship.Score);
            }

            await InMemoryFallbackStore.SaveRelationshipAsync(relationship);
            Console.WriteLine($"   → Relationship updated: {relationship.Type} (Score: {relationship.Score})");
        }

        private static MemoryType DetermineMemoryType(string message)
        {
            var lower = message.ToLower();

            if (lower.Contains("name") || lower.Contains("i am") || lower.Contains("i'm"))
                return MemoryType.Fact;

            if (lower.Contains("like") || lower.Contains("prefer") || lower.Contains("favorite") || lower.Contains("hate") || lower.Contains("love"))
                return MemoryType.Preference;

            if (lower.Contains("quest") || lower.Contains("mission") || lower.Contains("task") || lower.Contains("job"))
                return MemoryType.Event;

            if (lower.Contains("friend") || lower.Contains("enemy") || lower.Contains("ally") || lower.Contains("trust"))
                return MemoryType.Relationship;

            return MemoryType.Conversation;
        }

        #endregion

        #region Test Suite 5: Real World Scenario

        private static async Task TestSuite5_RealWorldScenario()
        {
            Console.WriteLine("═══════════════════════════════════════════════════════════");
            Console.WriteLine("TEST SUITE 5: Real-World Scenario");
            Console.WriteLine("═══════════════════════════════════════════════════════════");
            Console.WriteLine();

            await RunTest("5.1 Player Returns After Many Interactions", async () =>
            {
                string npc = "Morgana the Mage";
                string player = "AdventurerBob";

                Console.WriteLine($"\n   Scenario: {player} visits {npc} multiple times over several 'days'");
                Console.WriteLine("   " + new string('─', 55));

                // Day 1: First meeting
                Console.WriteLine($"\n   Day 1 - First Meeting:");
                await SimulateConversationTurn(npc, player, "Hello! My name is AdventurerBob", 1);
                await SimulateConversationTurn(npc, player, "I'm interested in learning magic", 2);

                // Day 2: 3 more conversations
                Console.WriteLine($"\n   Day 2 - Learning Magic:");
                await SimulateConversationTurn(npc, player, "Can you teach me about spells?", 3);
                await SimulateConversationTurn(npc, player, "I love fire magic!", 4);
                await SimulateConversationTurn(npc, player, "How much for a spell scroll?", 5);

                // Day 5: More interactions
                Console.WriteLine($"\n   Day 5 - Becoming Friends:");
                for (int i = 6; i <= 15; i++)
                {
                    await SimulateConversationTurn(npc, player, $"Interaction {i}", i);
                }

                // Day 10: Quest
                Console.WriteLine($"\n   Day 10 - Quest Time:");
                await SimulateConversationTurn(npc, player, "I need a difficult quest", 16);
                await SimulateConversationTurn(npc, player, "Remember, I specialize in fire magic", 17);

                // Check what NPC remembers
                Console.WriteLine("   " + new string('─', 55));
                var memories = await InMemoryFallbackStore.GetMemoriesAsync(npc, player, 5);
                var relationship = await InMemoryFallbackStore.GetRelationshipAsync(npc, player);

                Console.WriteLine($"\n   What {npc} Remembers About {player}:");
                Console.WriteLine($"   - Relationship: {relationship.Type} (Score: {relationship.Score}/100)");
                Console.WriteLine($"   - Total Interactions: {relationship.InteractionCount}");
                Console.WriteLine($"   - First Met: {relationship.FirstMet:yyyy-MM-dd HH:mm}");
                Console.WriteLine($"\n   Top {memories.Count} Memories:");

                foreach (var mem in memories)
                {
                    Console.WriteLine($"   - [{mem.Type}] Importance {mem.Importance}: {mem.Content}");
                }

                // Verify expectations
                Assert(relationship.InteractionCount == 17, "Should have 17 interactions");
                Assert(relationship.Score >= 17, "Score should be at least 17");
                Assert(memories.Count >= 3, "Should remember important facts");

                // Check if NPC would be considered a friend
                bool isFriend = relationship.Score >= 21;
                Console.WriteLine($"\n   Status: {player} and {npc} are now {(isFriend ? "friends" : "acquaintances")}!");

                return true;
            });

            Console.WriteLine();
        }

        #endregion

        #region Helper Methods

        private static async Task<bool> RunTest(string name, Func<Task<bool>> test)
        {
            Console.Write($"[TEST] {name}... ");

            try
            {
                bool result = await test();

                if (result)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("PASS");
                    Console.ResetColor();
                    testsPassed++;
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("FAIL");
                    Console.ResetColor();
                    testsFailed++;
                }

                return result;
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"EXCEPTION: {ex.Message}");
                Console.ResetColor();
                Console.WriteLine($"   Stack: {ex.StackTrace}");
                testsFailed++;
                return false;
            }
        }

        private static void Assert(bool condition, string message)
        {
            if (!condition)
            {
                throw new Exception($"Assertion failed: {message}");
            }
        }

        private static void PrintSummary()
        {
            Console.WriteLine();
            Console.WriteLine("╔════════════════════════════════════════════════════════════╗");
            Console.WriteLine("║                      TEST SUMMARY                          ║");
            Console.WriteLine("╚════════════════════════════════════════════════════════════╝");
            Console.WriteLine();
            Console.WriteLine($"  Total Tests: {testsPassed + testsFailed}");

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"  Passed: {testsPassed}");
            Console.ResetColor();

            if (testsFailed > 0)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"  Failed: {testsFailed}");
                Console.ResetColor();
            }
            else
            {
                Console.WriteLine($"  Failed: {testsFailed}");
            }

            double percentage = (double)testsPassed / (testsPassed + testsFailed) * 100;
            Console.WriteLine($"  Success Rate: {percentage:F1}%");
            Console.WriteLine();

            if (testsFailed == 0)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("  ✓ All tests passed successfully!");
                Console.ResetColor();
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("  ⚠ Some tests failed. Review output above.");
                Console.ResetColor();
            }

            Console.WriteLine();
            Console.WriteLine("  Memory System Status: " + (InMemoryFallbackStore.IsActive ? "ACTIVE" : "INACTIVE"));
            Console.WriteLine("  Storage Backend: In-Memory Fallback");
            Console.WriteLine();

            // Print fallback statistics
            string stats = InMemoryFallbackStore.GetStatistics();
            Console.WriteLine("  " + stats.Replace("\n", "\n  "));

            Console.WriteLine();
        }

        #endregion
    }
}
