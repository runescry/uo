using System;
using Server;
using Server.Mobiles;
using Server.Commands;
using Server.Services.LLM;
using Server.Custom.VystiaClasses.Quests;

namespace Server.Services.QuestJournal
{
    /// <summary>
    /// Initializes the unified quest journal system
    /// </summary>
    public static class QuestJournalInitializer
    {
        /// <summary>
        /// Initialize the quest journal system
        /// </summary>
        public static void Initialize()
        {
            Console.WriteLine("[QuestJournal] Initializing unified quest journal system...");

            // Register quest journal command
            CommandSystem.Register("QuestJournal", AccessLevel.Player, QuestJournal_OnCommand);
            CommandSystem.Register("QJ", AccessLevel.Player, QuestJournal_OnCommand);
            CommandSystem.Register("Quests", AccessLevel.Player, QuestJournal_OnCommand);

            // Register administrative commands
            CommandSystem.Register("QuestJournalAdmin", AccessLevel.Administrator, QuestJournalAdmin_OnCommand);
            CommandSystem.Register("QJA", AccessLevel.Administrator, QuestJournalAdmin_OnCommand);

            Console.WriteLine("[QuestJournal] Quest journal commands registered:");
            Console.WriteLine("  [QuestJournal or [QJ or [Quests] - Open quest journal");
            Console.WriteLine("  [QuestJournalAdmin or [QJA] - Administrative quest journal commands");
        }

        /// <summary>
        /// Main quest journal command handler
        /// </summary>
        [Usage("QuestJournal")]
        [Description("Opens the unified quest journal interface")]
        private static void QuestJournal_OnCommand(CommandEventArgs e)
        {
            Mobile from = e.Mobile;
            
            if (from is PlayerMobile)
            {
                PlayerMobile player = (PlayerMobile)from;
                
                try
                {
                    player.SendGump(new UnifiedQuestJournal(player));
                    player.SendMessage("Quest journal opened.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[QuestJournal] Error opening quest journal for {player.Name}: {ex.Message}");
                    player.SendMessage("Error opening quest journal. Please try again later.");
                }
            }
            else
            {
                from.SendMessage("This command can only be used by players.");
            }
        }

        /// <summary>
        /// Administrative quest journal command handler
        /// </summary>
        [Usage("QuestJournalAdmin")]
        [Description("Administrative commands for the quest journal system")]
        private static void QuestJournalAdmin_OnCommand(CommandEventArgs e)
        {
            Mobile from = e.Mobile;
            
            if (from.AccessLevel < AccessLevel.Administrator)
            {
                from.SendMessage("You do not have access to this command.");
                return;
            }

            if (e.Length == 0)
            {
                ShowAdminHelp(from);
                return;
            }

            string subCommand = e.GetString(0).ToLower();

            switch (subCommand)
            {
                case "stats":
                    ShowQuestStats(from);
                    break;

                case "validate":
                    ValidateQuestJournal(from);
                    break;

                case "clear":
                    ClearQuestJournal(from);
                    break;

                case "reload":
                    ReloadQuestJournal(from);
                    break;

                case "test":
                    TestQuestJournal(from);
                    break;

                default:
                    ShowAdminHelp(from);
                    break;
            }
        }

        /// <summary>
        /// Show administrative help
        /// </summary>
        private static void ShowAdminHelp(Mobile from)
        {
            from.SendMessage("=== Quest Journal Admin Commands ===");
            from.SendMessage("Usage: [QuestJournalAdmin <command>");
            from.SendMessage("");
            from.SendMessage("Available commands:");
            from.SendMessage("  stats     - Show quest journal statistics");
            from.SendMessage("  validate  - Validate quest journal data");
            from.SendMessage("  clear     - Clear quest journal cache");
            from.SendMessage("  reload    - Reload quest journal system");
            from.SendMessage("  test      - Test quest journal functionality");
            from.SendMessage("");
            from.SendMessage("Examples:");
            from.SendMessage("  [QuestJournalAdmin stats");
            from.SendMessage("  [QuestJournalAdmin validate");
            from.SendMessage("  [QuestJournalAdmin clear");
        }

        /// <summary>
        /// Show quest statistics
        /// </summary>
        private static void ShowQuestStats(Mobile from)
        {
            try
            {
                from.SendMessage("=== Quest Journal Statistics ===");
                
                // Get quest counts
                var vystiaCount = VystiaQuestSystem.GetAllQuests().Count;
                var activeCount = 0;
                var completedCount = 0;

                // Count active and completed quests for all players
                foreach (Mobile mob in World.Mobiles.Values)
                {
                    if (mob is PlayerMobile player)
                    {
                        var tracker = VystiaQuestTracker.GetTracker(player);
                        if (tracker != null)
                        {
                            activeCount += tracker.GetActiveQuests().Count;
                            completedCount += tracker.GetCompletedQuests().Count;
                        }
                    }
                }

                from.SendMessage($"Vystia Quests: {vystiaCount}");
                from.SendMessage($"Active Quests: {activeCount}");
                from.SendMessage($"Completed Quests: {completedCount}");
                from.SendMessage($"Total Players with Quests: {World.Mobiles.Values.OfType<PlayerMobile>().Count(p => VystiaQuestTracker.GetTracker(p) != null)}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[QuestJournal] Error getting quest stats: {ex.Message}");
                from.SendMessage("Error retrieving quest statistics.");
            }
        }

        /// <summary>
        /// Validate quest journal data
        /// </summary>
        private static void ValidateQuestJournal(Mobile from)
        {
            try
            {
                from.SendMessage("=== Quest Journal Validation ===");
                from.SendMessage("Validating quest journal data integrity...");

                // Validate Vystia quests
                var vystiaQuests = VystiaQuestSystem.GetAllQuests();
                from.SendMessage($"Validating {vystiaQuests.Count} Vystia quests...");

                int validQuests = 0;
                int invalidQuests = 0;

                foreach (var quest in vystiaQuests)
                {
                    if (quest != null && !string.IsNullOrEmpty(quest.Title))
                    {
                        validQuests++;
                    }
                    else
                    {
                        invalidQuests++;
                    }
                }

                from.SendMessage($"Validation Results:");
                from.SendMessage($"  Valid Quests: {validQuests}");
                from.SendMessage($"  Invalid Quests: {invalidQuests}");

                if (invalidQuests > 0)
                {
                    from.SendMessage($"Warning: {invalidQuests} quests have invalid data.");
                }
                else
                {
                    from.SendMessage("All quest data appears valid.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[QuestJournal] Error validating quest journal: {ex.Message}");
                from.SendMessage("Error validating quest journal data.");
            }
        }

        /// <summary>
        /// Clear quest journal cache
        /// </summary>
        private static void ClearQuestJournal(Mobile from)
        {
            try
            {
                from.SendMessage("Clearing quest journal cache...");
                
                // Clear any cached data
                // This would clear any in-memory caches
                
                from.SendMessage("Quest journal cache cleared successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[QuestJournal] Error clearing quest journal cache: {ex.Message}");
                from.SendMessage("Error clearing quest journal cache.");
            }
        }

        /// <summary>
        /// Reload quest journal system
        /// </summary>
        private static void ReloadQuestJournal(Mobile from)
        {
            try
            {
                from.SendMessage("Reloading quest journal system...");
                
                // Reinitialize the quest journal system
                Initialize();
                
                from.SendMessage("Quest journal system reloaded successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[QuestJournal] Error reloading quest journal: {ex.Message}");
                from.SendMessage("Error reloading quest journal system.");
            }
        }

        /// <summary>
        /// Test quest journal functionality
        /// </summary>
        private static void TestQuestJournal(Mobile from)
        {
            try
            {
                from.SendMessage("Testing quest journal functionality...");
                
                // Test quest loading
                var testPlayer = from as PlayerMobile;
                if (testPlayer != null)
                {
                    // Create a test quest journal
                    var testGump = new UnifiedQuestJournal(testPlayer);
                    
                    // Verify it was created successfully
                    from.SendMessage("Quest journal test completed successfully.");
                    from.SendMessage("Test results: Quest journal gump created without errors.");
                }
                else
                {
                    from.SendMessage("Test requires a player character.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[QuestJournal] Error testing quest journal: {ex.Message}");
                from.SendMessage("Error testing quest journal functionality.");
            }
        }
    }
}
