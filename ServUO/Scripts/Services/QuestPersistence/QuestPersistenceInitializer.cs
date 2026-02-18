using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Server;
using Server.Commands;
using Server.Mobiles;
using Server.Services.LLM;

namespace Server.Services.QuestPersistence
{
    /// <summary>
    /// Initializes the unified quest persistence system
    /// Integrates with existing LLM and quest systems
    /// </summary>
    public static class QuestPersistenceInitializer
    {
        private static bool s_Initialized = false;

        /// <summary>
        /// Initialize the unified quest persistence system
        /// </summary>
        public static void Initialize()
        {
            if (s_Initialized)
                return;

            s_Initialized = true;

            try
            {
                // Initialize the unified persistence system
                QuestPersistenceManager.Initialize();

                // Register administrative commands
                RegisterCommands();

                // Hook into world events for data integrity
                HookWorldEvents();

                Console.WriteLine("[QuestPersistenceInitializer] Unified quest persistence system initialized");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[QuestPersistenceInitializer] Error initializing: {ex.Message}");
            }
        }

        /// <summary>
        /// Register administrative commands for quest persistence management
        /// </summary>
        private static void RegisterCommands()
        {
            CommandSystem.Register("ValidateQuests", AccessLevel.GameMaster, ValidateQuests_OnCommand);
            CommandSystem.Register("RepairQuests", AccessLevel.GameMaster, RepairQuests_OnCommand);
            CommandSystem.Register("MigrateQuests", AccessLevel.GameMaster, MigrateQuests_OnCommand);
            CommandSystem.Register("BackupQuests", AccessLevel.GameMaster, BackupQuests_OnCommand);
            CommandSystem.Register("RestoreQuests", AccessLevel.GameMaster, RestoreQuests_OnCommand);
            CommandSystem.Register("QuestStats", AccessLevel.GameMaster, QuestStats_OnCommand);
            CommandSystem.Register("ValidateAllQuests", AccessLevel.Administrator, ValidateAllQuests_OnCommand);
        }

        /// <summary>
        /// Hook into world events for data integrity
        /// </summary>
        private static void HookWorldEvents()
        {
            // Hook into player login to validate quest data
            EventSink.Login += OnPlayerLogin;

            // Hook into world save for backup
            EventSink.WorldSave += OnWorldSave;

            Console.WriteLine("[QuestPersistenceInitializer] World events hooked for quest persistence");
        }

        /// <summary>
        /// Called when player logs in - validate quest data
        /// </summary>
        private static async void OnPlayerLogin(LoginEventArgs e)
        {
            if (e.Mobile is PlayerMobile player)
            {
                try
                {
                    // Validate quest data in background
                    _ = Task.Run(async () =>
                    {
                        var result = await QuestStateValidator.ValidatePlayerQuestStateAsync(player);
                        
                        if (!result.IsValid)
                        {
                            Console.WriteLine($"[QuestPersistenceInitializer] Quest validation issues for {player.Name}: {result.Errors.Count} errors, {result.Warnings.Count} warnings");
                            
                            // Send message to GMs about quest issues
                            foreach (var gm in World.Mobiles.Values.OfType<PlayerMobile>().Where(m => m.AccessLevel >= AccessLevel.GameMaster))
                            {
                                gm.SendMessage(38, $"[QuestPersistence] {player.Name} has {result.Errors.Count} quest validation errors. Use [ValidateQuests {player.Name} for details.");
                            }
                        }
                    });
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[QuestPersistenceInitializer] Error validating quests for {player.Name}: {ex.Message}");
                }
            }
        }

        /// <summary>
        /// Called on world save - create backup
        /// </summary>
        private static async void OnWorldSave(WorldSaveEventArgs e)
        {
            try
            {
                // Create backup for all players periodically
                if (DateTime.UtcNow.Minute % 30 == 0) // Every 30 minutes
                {
                    Console.WriteLine("[QuestPersistenceInitializer] Creating periodic quest backups...");
                    
                    foreach (var player in World.Mobiles.Values.OfType<PlayerMobile>().ToList())
                    {
                        await QuestPersistenceManager.BackupQuestDataAsync(player);
                    }
                    
                    Console.WriteLine("[QuestPersistenceInitializer] Periodic quest backups completed");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[QuestPersistenceInitializer] Error creating periodic backups: {ex.Message}");
            }
        }

        #region Command Handlers

        [Usage("ValidateQuests [playerName]")]
        [Description("Validate quest data integrity for a player")]
        private static async void ValidateQuests_OnCommand(CommandEventArgs e)
        {
            Mobile from = e.Mobile;
            PlayerMobile target = null;

            if (e.Length > 0)
            {
                string playerName = e.GetString(0);
                target = World.Mobiles.Values.OfType<PlayerMobile>().FirstOrDefault(m => m.Name.Equals(playerName, StringComparison.OrdinalIgnoreCase));
                
                if (target == null)
                {
                    from.SendMessage(38, $"Player '{playerName}' not found.");
                    return;
                }
            }
            else if (from is PlayerMobile)
            {
                target = from as PlayerMobile;
            }
            else
            {
                from.SendMessage(38, "Usage: [ValidateQuests [playerName]");
                return;
            }

            from.SendMessage(68, $"[QuestPersistence] Validating quest data for {target.Name}...");

            var result = await QuestStateValidator.ValidatePlayerQuestStateAsync(target);

            if (result.IsValid)
            {
                from.SendMessage(68, $"[QuestPersistence] Quest data validation completed successfully for {target.Name}");
                if (result.Warnings.Count > 0)
                {
                    from.SendMessage(38, $"[QuestPersistence] {result.Warnings.Count} warnings found:");
                    foreach (var warning in result.Warnings)
                    {
                        from.SendMessage(38, $"  Warning: {warning}");
                    }
                }
            }
            else
            {
                from.SendMessage(38, $"[QuestPersistence] Quest data validation failed for {target.Name}:");
                foreach (var error in result.Errors)
                {
                    from.SendMessage(38, $"  Error: {error}");
                }
                if (result.Warnings.Count > 0)
                {
                    from.SendMessage(38, $"Warnings:");
                    foreach (var warning in result.Warnings)
                    {
                        from.SendMessage(38, $"  Warning: {warning}");
                    }
                }
                from.SendMessage(68, $"[QuestPersistence] Use [RepairQuests {target.Name} to attempt repair");
            }
        }

        [Usage("RepairQuests [playerName]")]
        [Description("Repair quest data issues for a player")]
        private static async void RepairQuests_OnCommand(CommandEventArgs e)
        {
            Mobile from = e.Mobile;
            PlayerMobile target = null;

            if (e.Length > 0)
            {
                string playerName = e.GetString(0);
                target = World.Mobiles.Values.OfType<PlayerMobile>().FirstOrDefault(m => m.Name.Equals(playerName, StringComparison.OrdinalIgnoreCase));
                
                if (target == null)
                {
                    from.SendMessage(38, $"Player '{playerName}' not found.");
                    return;
                }
            }
            else if (from is PlayerMobile)
            {
                target = from as PlayerMobile;
            }
            else
            {
                from.SendMessage(38, "Usage: [RepairQuests [playerName]");
                return;
            }

            from.SendMessage(68, $"[QuestPersistence] Repairing quest data for {target.Name}...");

            var result = await QuestStateValidator.RepairPlayerQuestStateAsync(target);

            if (result.Errors.Count == 0)
            {
                from.SendMessage(68, $"[QuestPersistence] Quest data repair completed for {target.Name}: {result.RepairedIssues} issues fixed");
            }
            else
            {
                from.SendMessage(38, $"[QuestPersistence] Quest data repair partially completed for {target.Name}:");
                from.SendMessage(38, $"  Fixed: {result.RepairedIssues} issues");
                from.SendMessage(38, $"  Unfixable: {result.UnfixableIssues} issues");
                foreach (var error in result.Errors)
                {
                    from.SendMessage(38, $"  Error: {error}");
                }
            }
        }

        [Usage("MigrateQuests [playerName]")]
        [Description("Migrate quest data from legacy systems to unified persistence")]
        private static async void MigrateQuests_OnCommand(CommandEventArgs e)
        {
            Mobile from = e.Mobile;
            PlayerMobile target = null;

            if (e.Length > 0)
            {
                string playerName = e.GetString(0);
                target = World.Mobiles.Values.OfType<PlayerMobile>().FirstOrDefault(m => m.Name.Equals(playerName, StringComparison.OrdinalIgnoreCase));
                
                if (target == null)
                {
                    from.SendMessage(38, $"Player '{playerName}' not found.");
                    return;
                }
            }
            else if (from is PlayerMobile)
            {
                target = from as PlayerMobile;
            }
            else
            {
                from.SendMessage(38, "Usage: [MigrateQuests [playerName]");
                return;
            }

            from.SendMessage(68, $"[QuestPersistence] Migrating quest data for {target.Name}...");

            var result = await QuestPersistenceManager.MigrateLegacyQuestDataAsync(target);

            if (result.Errors.Count == 0)
            {
                from.SendMessage(68, $"[QuestPersistence] Migration completed for {target.Name}:");
                from.SendMessage(68, $"  Vystia quests: {result.VystiaQuestsMigrated}");
                from.SendMessage(68, $"  Dynamic quests: {result.DynamicQuestsMigrated}");
                from.SendMessage(68, $"  Traditional quests: {result.TraditionalQuestsMigrated}");
                from.SendMessage(68, $"  Total: {result.TotalMigrated} quests migrated");
            }
            else
            {
                from.SendMessage(38, $"[QuestPersistence] Migration partially completed for {target.Name}:");
                from.SendMessage(68, $"  Total migrated: {result.TotalMigrated} quests");
                from.SendMessage(38, $"  Errors: {result.Errors.Count}");
                foreach (var error in result.Errors)
                {
                    from.SendMessage(38, $"  Error: {error}");
                }
            }
        }

        [Usage("BackupQuests [playerName]")]
        [Description("Create backup of quest data for a player")]
        private static async void BackupQuests_OnCommand(CommandEventArgs e)
        {
            Mobile from = e.Mobile;
            PlayerMobile target = null;

            if (e.Length > 0)
            {
                string playerName = e.GetString(0);
                target = World.Mobiles.Values.OfType<PlayerMobile>().FirstOrDefault(m => m.Name.Equals(playerName, StringComparison.OrdinalIgnoreCase));
                
                if (target == null)
                {
                    from.SendMessage(38, $"Player '{playerName}' not found.");
                    return;
                }
            }
            else if (from is PlayerMobile)
            {
                target = from as PlayerMobile;
            }
            else
            {
                from.SendMessage(38, "Usage: [BackupQuests [playerName]");
                return;
            }

            from.SendMessage(68, $"[QuestPersistence] Creating backup of quest data for {target.Name}...");

            await QuestPersistenceManager.BackupQuestDataAsync(target);

            from.SendMessage(68, $"[QuestPersistence] Backup completed for {target.Name}");
        }

        [Usage("RestoreQuests [playerName] [backupDate]")]
        [Description("Restore quest data from backup for a player")]
        private static async void RestoreQuests_OnCommand(CommandEventArgs e)
        {
            Mobile from = e.Mobile;
            PlayerMobile target = null;
            DateTime backupDate = DateTime.MinValue;

            if (e.Length >= 1)
            {
                string playerName = e.GetString(0);
                target = World.Mobiles.Values.OfType<PlayerMobile>().FirstOrDefault(m => m.Name.Equals(playerName, StringComparison.OrdinalIgnoreCase));
                
                if (target == null)
                {
                    from.SendMessage(38, $"Player '{playerName}' not found.");
                    return;
                }

                if (e.Length >= 2)
                {
                    if (DateTime.TryParse(e.GetString(1), out backupDate))
                    {
                        // Valid date format
                    }
                    else
                    {
                        from.SendMessage(38, "Invalid date format. Use YYYY-MM-DD format.");
                        return;
                    }
                }
                else
                {
                    // Show available backup dates
                    var backupDates = await QuestPersistenceManager.GetBackupDatesAsync(target);
                    if (backupDates.Count > 0)
                    {
                        from.SendMessage(68, $"[QuestPersistence] Available backup dates for {target.Name}:");
                        foreach (var date in backupDates)
                        {
                            from.SendMessage(68, $"  {date:yyyy-MM-dd HH:mm:ss}");
                        }
                        from.SendMessage(38, "Usage: [RestoreQuests [playerName] [backupDate]");
                    }
                    else
                    {
                        from.SendMessage(38, $"No backups found for {target.Name}");
                    }
                    return;
                }
            }
            else
            {
                from.SendMessage(38, "Usage: [RestoreQuests [playerName] [backupDate]");
                return;
            }

            from.SendMessage(68, $"[QuestPersistence] Restoring quest data for {target.Name} from {backupDate:yyyy-MM-dd HH:mm:ss}...");

            await QuestPersistenceManager.RestoreQuestDataAsync(target, backupDate);

            from.SendMessage(68, $"[QuestPersistence] Restore completed for {target.Name}");
        }

        [Usage("QuestStats")]
        [Description("Show statistics about quest persistence")]
        private static async void QuestStats_OnCommand(CommandEventArgs e)
        {
            Mobile from = e.Mobile;

            from.SendMessage(68, "[QuestPersistence] Gathering quest statistics...");

            var stats = await QuestPersistenceManager.GetStatsAsync();

            from.SendMessage(68, "=== Quest Persistence Statistics ===");
            from.SendMessage(68, $"Total Players: {stats.TotalPlayers}");
            from.SendMessage(68, $"Players with Quests: {stats.PlayersWithQuests}");
            from.SendMessage(68, $"Total Quests: {stats.TotalQuests}");
            from.SendMessage(68, $"Active Quests: {stats.ActiveQuests}");
            from.SendMessage(68, $"Completed Quests: {stats.CompletedQuests}");
            from.SendMessage(68, $"Available Backups: {stats.BackupCount}");
            from.SendMessage(68, "=== End Statistics ===");
        }

        [Usage("ValidateAllQuests")]
        [Description("Validate quest data for all players")]
        private static async void ValidateAllQuests_OnCommand(CommandEventArgs e)
        {
            Mobile from = e.Mobile;

            from.SendMessage(68, "[QuestPersistence] Validating quest data for all players...");

            var summary = await QuestStateValidator.ValidateAllPlayersAsync();

            from.SendMessage(68, "=== All Players Validation Summary ===");
            from.SendMessage(68, $"Total Players: {summary.TotalPlayers}");
            from.SendMessage(summary.ValidPlayers == summary.TotalPlayers ? 68 : 38, $"Valid Players: {summary.ValidPlayers}");
            from.SendMessage(summary.InvalidPlayers == 0 ? 68 : 38, $"Invalid Players: {summary.InvalidPlayers}");
            from.SendMessage(summary.TotalErrors == 0 ? 68 : 38, $"Total Errors: {summary.TotalErrors}");
            from.SendMessage(summary.TotalWarnings == 0 ? 68 : 38, $"Total Warnings: {summary.TotalWarnings}");

            if (summary.Errors.Count > 0)
            {
                from.SendMessage(38, "Errors:");
                foreach (var error in summary.Errors.Take(10)) // Limit to first 10 errors
                {
                    from.SendMessage(38, $"  {error}");
                }
                if (summary.Errors.Count > 10)
                {
                    from.SendMessage(38, $"  ... and {summary.Errors.Count - 10} more errors");
                }
            }

            from.SendMessage(68, "=== End Summary ===");
        }

        #endregion
    }
}
