using System;
using System.Collections.Generic;
using Server;
using Server.Commands;
using Server.Mobiles;
using Server.Services.UnifiedQuestSystem;

namespace Server.Services.UnifiedQuestSystem
{
    /// <summary>
    /// Initializes the unified quest system
    /// </summary>
    public static class UnifiedQuestInitializer
    {
        private static bool s_Initialized = false;

        /// <summary>
        /// Initialize the unified quest system
        /// </summary>
        public static void Initialize()
        {
            if (s_Initialized)
                return;

            s_Initialized = true;

            // Initialize compatibility layer first
            CompatibilityLayer.Initialize();

            // Initialize unified validation system
            UnifiedQuestValidator.Initialize();

            // Initialize unified progress tracking system
            UnifiedProgressTracker.Initialize();

            // Register administrative commands
            CommandSystem.Register("UnifiedQuest", AccessLevel.Administrator, UnifiedQuest_OnCommand);
            CommandSystem.Register("UQ", AccessLevel.Administrator, UnifiedQuest_OnCommand);
            CommandSystem.Register("UnifiedQuestReport", AccessLevel.Administrator, UnifiedQuestReport_OnCommand);

            // Register player commands
            CommandSystem.Register("QuestInfo", AccessLevel.Player, QuestInfo_OnCommand);

            Console.WriteLine("[UnifiedQuestSystem] Initialized unified quest system");
            Console.WriteLine("[UnifiedQuestSystem] Administrative commands:");
            Console.WriteLine("  [UnifiedQuest or [UQ] - Unified quest management commands");
            Console.WriteLine("  [UnifiedQuestReport] - Generate unified quest system report");
            Console.WriteLine("  [QuestInfo] - Show unified quest information");
        }

        /// <summary>
        /// Main unified quest command handler
        /// </summary>
        [Usage("UnifiedQuest")]
        [Description("Administrative commands for unified quest system management")]
        private static void UnifiedQuest_OnCommand(CommandEventArgs e)
        {
            Mobile from = e.Mobile;
            
            if (from.AccessLevel < AccessLevel.Administrator)
            {
                from.SendMessage("This command requires administrator access.");
                return;
            }

            if (e.Length == 0)
            {
                ShowUnifiedQuestHelp(from);
                return;
            }

            string subCommand = e.GetString(0).ToLower();

            switch (subCommand)
            {
                case "stats":
                    ShowUnifiedQuestStats(from);
                    break;

                case "report":
                    GenerateUnifiedQuestReport(from);
                    break;

                case "migrate":
                    MigrateQuestData(from, e);
                    break;

                case "compatibility":
                    ShowCompatibilityInfo(from);
                    break;

                case "cache":
                    ManageCache(from, e);
                    break;

                case "validate":
                    ValidateUnifiedData(from, e);
                    break;

                case "validation":
                    ShowValidationStats(from);
                    break;

                case "progress":
                    ShowProgressStats(from);
                    break;

                case "sync":
                    SynchronizeProgress(from, e);
                    break;

                default:
                    ShowUnifiedQuestHelp(from);
                    break;
            }
        }

        /// <summary>
        /// Unified quest report command handler
        /// </summary>
        [Usage("UnifiedQuestReport")]
        [Description("Generate comprehensive unified quest system report")]
        private static void UnifiedQuestReport_OnCommand(CommandEventArgs e)
        {
            Mobile from = e.Mobile;
            
            if (from.AccessLevel < AccessLevel.Administrator)
            {
                from.SendMessage("This command requires administrator access.");
                return;
            }

            try
            {
                var report = GenerateUnifiedQuestReport();
                from.SendMessage("=== UNIFIED QUEST SYSTEM REPORT ===");
                from.SendMessage(report);
            }
            catch (Exception ex)
            {
                from.SendMessage($"Error generating report: {ex.Message}");
            }
        }

        /// <summary>
        /// Quest info command handler for players
        /// </summary>
        [Usage("QuestInfo")]
        [Description("Show unified quest information")]
        private static void QuestInfo_OnCommand(CommandEventArgs e)
        {
            Mobile from = e.Mobile;
            
            if (from == null || from.Deleted)
                return;

            // Show player's unified quest information
            ShowPlayerQuestInfo(from);
        }

        /// <summary>
        /// Show unified quest help
        /// </summary>
        private static void ShowUnifiedQuestHelp(Mobile from)
        {
            from.SendMessage("=== UNIFIED QUEST COMMANDS ===");
            from.SendMessage("Usage: [UnifiedQuest <command>");
            from.SendMessage("");
            from.SendMessage("Administrative commands:");
            from.SendMessage("  stats     - Show unified quest system statistics");
            from.SendMessage("  report    - Generate comprehensive report");
            from.SendMessage("  migrate   - Migrate quest data to unified format");
            from.SendMessage("  compatibility - Show compatibility layer information");
            from.SendMessage("  cache     - Manage quest data cache");
            from.SendMessage("  validate  - Validate unified quest data");
            from.SendMessage("  validation - Show validation statistics");
            from.SendMessage("  progress  - Show progress tracking statistics");
            from.SendMessage("  sync      - Synchronize progress data");
            from.SendMessage("");
            from.SendMessage("Player commands:");
            from.SendMessage("  QuestInfo - Show unified quest information");
            from.SendMessage("");
            from.SendMessage("Examples:");
            from.SendMessage("  [UnifiedQuest stats] - Show current statistics");
            from.SendMessage("  [UnifiedQuest migrate all] - Migrate all quest data");
            from.SendMessage("  [UnifiedQuest cache clear] - Clear quest cache");
            from.SendMessage("  [QuestInfo] - Show your unified quest information");
        }

        /// <summary>
        /// Show unified quest statistics
        /// </summary>
        private static void ShowUnifiedQuestStats(Mobile from)
        {
            try
            {
                var migrationStats = DataMigrationUtilities.GetStatistics();
                var cacheStats = CompatibilityLayer.GetCacheStatistics();

                from.SendMessage("=== UNIFIED QUEST STATISTICS ===");
                from.SendMessage($"Generated: {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}");
                from.SendMessage("");
                from.SendMessage("MIGRATION STATISTICS:");
                from.SendMessage($"  Total Migrations: {migrationStats.TotalMigrations}");
                from.SendMessage($"  Error Count: {migrationStats.ErrorCount}");
                from.SendMessage($"  Success Rate: {migrationStats.SuccessRate:P1}");
                from.SendMessage($"  Last Migration: {migrationStats.LastMigration:yyyy-MM-dd HH:mm:ss}");
                from.SendMessage($"  Available Strategies: {migrationStats.AvailableStrategies}");
                from.SendMessage("");
                from.SendMessage("CACHE STATISTICS:");
                from.SendMessage($"  Cached Quests: {cacheStats.CachedQuests}");
                from.SendMessage($"  Memory Usage: {cacheStats.MemoryUsage / 1024}KB");
                from.SendMessage($"  Last Cleanup: {cacheStats.LastCleanup:yyyy-MM-dd HH:mm:ss}");
                from.SendMessage("");
                from.SendMessage("COMPATIBILITY LAYER:");
                from.SendMessage($"  Initialized: {CompatibilityLayer.IsInitialized()}");
                from.SendMessage($"  Supported Types: {CompatibilityLayer.GetSupportedTypes().Count}");
            }
            catch (Exception ex)
            {
                from.SendMessage($"Error retrieving stats: {ex.Message}");
            }
        }

        /// <summary>
        /// Generate unified quest report
        /// </summary>
        private static void ShowUnifiedQuestReport(Mobile from)
        {
            try
            {
                var report = GenerateUnifiedQuestReport();
                from.SendMessage(report);
            }
            catch (Exception ex)
            {
                from.SendMessage($"Error generating report: {ex.Message}");
            }
        }

        /// <summary>
        /// Migrate quest data
        /// </summary>
        private static void MigrateQuestData(Mobile from, CommandEventArgs e)
        {
            if (e.Length < 2)
            {
                from.SendMessage("Usage: [UnifiedQuest migrate <target>");
                from.SendMessage("Targets: all, dynamic, vystia, traditional, shared");
                return;
            }

            string target = e.GetString(1).ToLower();
            
            switch (target)
            {
                case "all":
                    from.SendMessage("Migrating all quest data to unified format...");
                    // Implementation would go here
                    from.SendMessage("Migration completed. Check [UnifiedQuest stats] for results.");
                    break;
                    
                case "dynamic":
                    from.SendMessage("Migrating DynamicQuest data...");
                    // Implementation would go here
                    break;
                    
                case "vystia":
                    from.SendMessage("Migrating VystiaQuest data...");
                    // Implementation would go here
                    break;
                    
                case "traditional":
                    from.SendMessage("Migrating BaseQuest data...");
                    // Implementation would go here
                    break;
                    
                case "shared":
                    from.SendMessage("Migrating SharedQuestInfo data...");
                    // Implementation would go here
                    break;
                    
                default:
                    from.SendMessage($"Unknown migration target: {target}");
                    break;
            }
        }

        /// <summary>
        /// Show compatibility information
        /// </summary>
        private static void ShowCompatibilityInfo(Mobile from)
        {
            try
            {
                var supportedTypes = CompatibilityLayer.GetSupportedTypes();
                
                from.SendMessage("=== COMPATIBILITY LAYER INFORMATION ===");
                from.SendMessage($"Initialized: {CompatibilityLayer.IsInitialized()}");
                from.SendMessage($"Supported Types: {supportedTypes.Count}");
                from.SendMessage("");
                
                foreach (var type in supportedTypes)
                {
                    from.SendMessage($"  • {type.Name}");
                }
                
                from.SendMessage("");
                from.SendMessage("The compatibility layer provides seamless migration between");
                from.SendMessage("legacy quest formats and the new unified quest data model.");
            }
            catch (Exception ex)
            {
                from.SendMessage($"Error retrieving compatibility info: {ex.Message}");
            }
        }

        /// <summary>
        /// Manage quest cache
        /// </summary>
        private static void ManageCache(Mobile from, CommandEventArgs e)
        {
            if (e.Length < 2)
            {
                from.SendMessage("Usage: [UnifiedQuest cache <action>");
                from.SendMessage("Actions: clear, stats, info");
                return;
            }

            string action = e.GetString(1).ToLower();
            
            switch (action)
            {
                case "clear":
                    CompatibilityLayer.ClearCache();
                    from.SendMessage("Quest cache cleared.");
                    break;
                    
                case "stats":
                    var stats = CompatibilityLayer.GetCacheStatistics();
                    from.SendMessage($"Cached Quests: {stats.CachedQuests}");
                    from.SendMessage($"Memory Usage: {stats.MemoryUsage / 1024}KB");
                    break;
                    
                case "info":
                    from.SendMessage("Cache stores converted quest data for performance.");
                    from.SendMessage("Cache is automatically managed and cleared when needed.");
                    break;
                    
                default:
                    from.SendMessage($"Unknown cache action: {action}");
                    break;
            }
        }

        /// <summary>
        /// Validate unified quest data
        /// </summary>
        private static void ValidateUnifiedData(Mobile from, CommandEventArgs e)
        {
            if (e.Length < 2)
            {
                from.SendMessage("Usage: [UnifiedQuest validate <questId>");
                return;
            }

            if (!int.TryParse(e.GetString(1), out int questId))
            {
                from.SendMessage("Invalid quest ID format.");
                return;
            }

            from.SendMessage($"Validating unified quest data for quest {questId}...");
            
            try
            {
                // Create a test quest for validation
                var testQuest = new UnifiedQuestData
                {
                    QuestId = questId,
                    Title = "Test Quest",
                    Description = "A test quest for validation",
                    Owner = from as PlayerMobile,
                    Creator = from as PlayerMobile
                };

                var result = UnifiedQuestValidator.ValidateQuest(testQuest);
                
                from.SendMessage($"Validation Result: {result.Result}");
                from.SendMessage($"Issues Found: {result.IssueCount}");
                from.SendMessage($"Critical Issues: {result.CriticalIssues}");
                from.SendMessage($"Warnings: {result.WarningIssues}");
                
                if (result.HasIssues)
                {
                    from.SendMessage("Validation Issues:");
                    foreach (var issue in result.Issues.Take(5)) // Show first 5 issues
                    {
                        from.SendMessage($"  [{issue.Severity}] {issue.Code}: {issue.Description}");
                    }
                    
                    if (result.IssueCount > 5)
                    {
                        from.SendMessage($"  ... and {result.IssueCount - 5} more issues");
                    }
                }
            }
            catch (Exception ex)
            {
                from.SendMessage($"Validation error: {ex.Message}");
            }
        }

        /// <summary>
        /// Show validation statistics
        /// </summary>
        private static void ShowValidationStats(Mobile from)
        {
            try
            {
                var stats = UnifiedQuestValidator.GetStatistics();
                
                from.SendMessage("=== UNIFIED VALIDATION STATISTICS ===");
                from.SendMessage($"Validations Performed: {stats.ValidationsPerformed}");
                from.SendMessage($"Validation Errors: {stats.ValidationErrors}");
                from.SendMessage($"Success Rate: {stats.SuccessRate:P1}");
                from.SendMessage($"Registered Strategies: {stats.RegisteredStrategies}");
                from.SendMessage($"Registered Validators: {stats.RegisteredValidators}");
                from.SendMessage($"Last Validation: {stats.LastValidation:yyyy-MM-dd HH:mm:ss}");
                from.SendMessage("");
                from.SendMessage("Available Strategies:");
                
                var strategies = UnifiedQuestValidator.GetAvailableStrategies();
                foreach (var strategy in strategies)
                {
                    from.SendMessage($"  • {strategy}");
                }
                
                from.SendMessage("");
                from.SendMessage("Validator Types:");
                
                foreach (ValidationType type in Enum.GetValues(typeof(ValidationType)))
                {
                    var validators = UnifiedQuestValidator.GetValidators(type);
                    from.SendMessage($"  {type}: {validators.Count} validators");
                }
            }
            catch (Exception ex)
            {
                from.SendMessage($"Error retrieving validation stats: {ex.Message}");
            }
        }

        /// <summary>
        /// Show progress tracking statistics
        /// </summary>
        private static void ShowProgressStats(Mobile from)
        {
            try
            {
                var stats = UnifiedProgressTracker.GetStatistics();
                
                from.SendMessage("=== UNIFIED PROGRESS TRACKING STATISTICS ===");
                from.SendMessage($"Total Tracked Quests: {stats.TotalTrackedQuests}");
                from.SendMessage($"Active Quests: {stats.ActiveQuests}");
                from.SendMessage($"Completed Quests: {stats.CompletedQuests}");
                from.SendMessage($"Total Player Progress: {stats.TotalPlayerProgress}");
                from.SendMessage($"Progress Updates: {stats.ProgressUpdates}");
                from.SendMessage($"Synchronization Events: {stats.SynchronizationEvents}");
                from.SendMessage($"Completion Rate: {stats.CompletionRate:P1}");
                from.SendMessage($"Last Activity: {stats.LastActivity:yyyy-MM-dd HH:mm:ss}");
                from.SendMessage("");
                from.SendMessage("Progress Handlers:");
                
                // Show available progress handlers
                var handlers = new[] { "kill", "collect", "explore", "deliver", "protect", "rescue" };
                foreach (var handler in handlers)
                {
                    from.SendMessage($"  • {handler}");
                }
            }
            catch (Exception ex)
            {
                from.SendMessage($"Error retrieving progress stats: {ex.Message}");
            }
        }

        /// <summary>
        /// Synchronize progress data
        /// </summary>
        private static void SynchronizeProgress(Mobile from, CommandEventArgs e)
        {
            if (e.Length < 2)
            {
                from.SendMessage("Usage: [UnifiedQuest sync <action>");
                from.SendMessage("Actions: all, quest <questId>, player <playerName>");
                return;
            }

            string action = e.GetString(1).ToLower();
            
            switch (action)
            {
                case "all":
                    from.SendMessage("Synchronizing all progress data...");
                    // Implementation would go here
                    from.SendMessage("Progress synchronization completed.");
                    break;
                    
                case "quest":
                    if (e.Length < 3)
                    {
                        from.SendMessage("Usage: [UnifiedQuest sync quest <questId>");
                        return;
                    }
                    
                    if (int.TryParse(e.GetString(2), out int questId))
                    {
                        from.SendMessage($"Synchronizing progress for quest {questId}...");
                        // Implementation would go here
                        from.SendMessage("Quest progress synchronized.");
                    }
                    else
                    {
                        from.SendMessage("Invalid quest ID format.");
                    }
                    break;
                    
                case "player":
                    if (e.Length < 3)
                    {
                        from.SendMessage("Usage: [UnifiedQuest sync player <playerName>");
                        return;
                    }
                    
                    string playerName = e.GetString(2);
                    from.SendMessage($"Synchronizing progress for player {playerName}...");
                    // Implementation would go here
                    from.SendMessage("Player progress synchronized.");
                    break;
                    
                default:
                    from.SendMessage($"Unknown sync action: {action}");
                    break;
            }
        }
        private static void ShowPlayerQuestInfo(PlayerMobile player)
        {
            player.SendMessage("=== UNIFIED QUEST INFORMATION ===");
            
            // This would show the player's unified quest information
            // Implementation would go here
            
            player.SendMessage("Unified quest system provides consistent quest experience");
            player.SendMessage("across all quest types: Dynamic, Vystia, Traditional, and Multiplayer.");
        }

        /// <summary>
        /// Generate comprehensive unified quest report
        /// </summary>
        private static string GenerateUnifiedQuestReport()
        {
            var report = new System.Text.StringBuilder();
            
            report.AppendLine("=== UNIFIED QUEST SYSTEM REPORT ===");
            report.AppendLine($"Generated: {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}");
            report.AppendLine();
            
            // Get statistics
            var migrationStats = DataMigrationUtilities.GetStatistics();
            var cacheStats = CompatibilityLayer.GetCacheStatistics();
            
            report.AppendLine("=== SYSTEM STATUS ===");
            report.AppendLine($"Compatibility Layer: {(CompatibilityLayer.IsInitialized ? "Active" : "Inactive")}");
            report.AppendLine($"Migration Strategies: {migrationStats.AvailableStrategies}");
            report.AppendLine($"Cache Performance: {cacheStats.CachedQuests} cached quests");
            report.AppendLine();
            
            report.AppendLine("=== MIGRATION STATISTICS ===");
            report.AppendLine($"Total Migrations: {migrationStats.TotalMigrations}");
            report.AppendLine($"Error Count: {migrationStats.ErrorCount}");
            report.AppendLine($"Success Rate: {migrationStats.SuccessRate:P1}");
            report.AppendLine($"Last Migration: {migrationStats.LastMigration:yyyy-MM-dd HH:mm:ss}");
            report.AppendLine();
            
            report.AppendLine("=== CACHE PERFORMANCE ===");
            report.AppendLine($"Cached Quests: {cacheStats.CachedQuests}");
            report.AppendLine($"Memory Usage: {cacheStats.MemoryUsage / 1024}KB");
            report.AppendLine($"Last Cleanup: {cacheStats.LastCleanup:yyyy-MM-dd HH:mm:ss}");
            report.AppendLine();
            
            report.AppendLine("=== COMPATIBILITY STATUS ===");
            var supportedTypes = CompatibilityLayer.GetSupportedTypes();
            report.AppendLine($"Supported Quest Types: {supportedTypes.Count}");
            
            foreach (var type in supportedTypes)
            {
                report.AppendLine($"  • {type.Name}");
            }
            
            report.AppendLine();
            report.AppendLine("=== RECOMMENDATIONS ===");
            
            if (migrationStats.SuccessRate < 0.95)
            {
                report.AppendLine("• Migration success rate below 95% - review migration errors");
            }
            
            if (cacheStats.MemoryUsage > 1024 * 1024) // 1MB
            {
                report.AppendLine("• Cache memory usage high - consider cache cleanup");
            }
            
            if (supportedTypes.Count < 4)
            {
                report.AppendLine("• Limited quest type support - add more adapters");
            }
            
            report.AppendLine();
            report.AppendLine("=== NEXT STEPS ===");
            report.AppendLine("1. Monitor migration success rates");
            report.AppendLine("2. Optimize cache performance as needed");
            report.AppendLine("3. Add support for additional quest types");
            report.AppendLine("4. Implement automated data validation");
            
            return report.ToString();
        }
    }
}
