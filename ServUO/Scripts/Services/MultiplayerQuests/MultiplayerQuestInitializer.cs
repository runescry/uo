using System;
using System.Collections.Generic;
using System.Linq;
using Server;
using Server.Commands;
using Server.Mobiles;
using Server.Services.LLM;
using Server.Services.QuestJournal;
using Server.Services.QuestVariety;
using Server.Services.MultiplayerQuests;
using Server.Custom.VystiaClasses.Quests;
using Server.Engines.Quests;
using Server.Engines.PartySystem;

namespace Server.Services.MultiplayerQuests
{
    /// <summary>
    /// Initializes the multiplayer quest system
    /// </summary>
    public static class MultiplayerQuestInitializer
    {
        private static bool s_Initialized = false;

        /// <summary>
        /// Initialize the multiplayer quest system
        /// </summary>
        public static void Initialize()
        {
            if (s_Initialized)
                return;

            s_Initialized = true;

            // Initialize all multiplayer quest subsystems
            PartyQuestSharingSystem.Initialize();
            CooperativeObjectiveManager.Initialize();
            SharedProgressTracker.Initialize();
            PartyRewardDistribution.Initialize();
            QuestCommunicationSystem.Initialize();
            MultiplayerQuestValidator.Initialize();

            // Register administrative commands
            CommandSystem.Register("MultiplayerQuest", AccessLevel.Administrator, MultiplayerQuest_OnCommand);
            CommandSystem.Register("MPQ", AccessLevel.Administrator, MultiplayerQuest_OnCommand);
            CommandSystem.Register("MultiplayerQuestReport", AccessLevel.Administrator, MultiplayerQuestReport_OnCommand);

            // Register player commands
            CommandSystem.Register("ShareQuest", AccessLevel.Player, ShareQuest_OnCommand);
            CommandSystem.Register("AcceptQuest", AccessLevel.Player, AcceptQuest_OnCommand);
            CommandSystem.Register("DeclineQuest", AccessLevel.Player, DeclineQuest_OnCommand);
            CommandSystem.Register("QuestStatus", AccessLevel.Player, QuestStatus_OnCommand);
            CommandSystem.Register("PartyChat", AccessLevel.Player, PartyChat_OnCommand);

            Console.WriteLine("[MultiplayerQuest] Multiplayer quest system initialized");
            Console.WriteLine("[MultiplayerQuest] Administrative commands:");
            Console.WriteLine("  [MultiplayerQuest or [MPQ] - Multiplayer quest management commands");
            Console.WriteLine("  [MultiplayerQuestReport] - Generate multiplayer quest report");
            Console.WriteLine("  [ShareQuest] - Share current quest with party");
            Console.WriteLine("  [AcceptQuest] - Accept a shared quest");
            Console.WriteLine("  [DeclineQuest] - Decline a shared quest");
            Console.WriteLine("  [QuestStatus] - Show shared quest status");
            Console.WriteLine("  [PartyChat] - Send message to party members");
        }

        /// <summary>
        /// Main multiplayer quest command handler
        /// </summary>
        [Usage("MultiplayerQuest")]
        [Description("Administrative commands for multiplayer quest management")]
        private static void MultiplayerQuest_OnCommand(CommandEventArgs e)
        {
            Mobile from = e.Mobile;
            
            if (from.AccessLevel < AccessLevel.Administrator)
            {
                from.SendMessage("This command requires administrator access.");
                return;
            }

            if (e.Length == 0)
            {
                ShowMultiplayerQuestHelp(from);
                return;
            }

            string subCommand = e.GetString(0).ToLower();

            switch (subCommand)
            {
                case "stats":
                    ShowMultiplayerQuestStats(from);
                    break;

                case "report":
                    GenerateMultiplayerQuestReport(from);
                    break;

                case "reset":
                    ResetMultiplayerQuestStats(from);
                    break;

                case "validate":
                    ValidateQuestCommand(from, e);
                    break;

                case "sharing":
                    ShowSharingStats(from);
                    break;

                case "progress":
                    ShowProgressStats(from);
                    break;

                case "rewards":
                    ShowRewardStats(from);
                    break;

                case "communication":
                    ShowCommunicationStats(from);
                    break;

                default:
                    ShowMultiplayerQuestHelp(from);
                    break;
            }
        }

        /// <summary>
        /// Multiplayer quest report command handler
        /// </summary>
        [Usage("MultiplayerQuestReport")]
        [Description("Generate comprehensive multiplayer quest report")]
        private static void MultiplayerQuestReport_OnCommand(CommandEventArgs e)
        {
            Mobile from = e.Mobile;
            
            if (from.AccessLevel < AccessLevel.Administrator)
            {
                from.SendMessage("This command requires administrator access.");
                return;
            }

            try
            {
                var report = GenerateMultiplayerQuestReport();
                from.SendMessage("=== MULTIPLAYER QUEST REPORT ===");
                from.SendMessage(report);
            }
            catch (Exception ex)
            {
                from.SendMessage($"Error generating report: {ex.Message}");
            }
        }

        /// <summary>
        /// Share quest command handler
        /// </summary>
        [Usage("ShareQuest")]
        [Description("Share current quest with party")]
        private static void ShareQuest_OnCommand(CommandEventArgs e)
        {
            Mobile from = e.Mobile;
            
            if (from == null || from.Deleted)
                return;

            var party = from.Party as Party;
            if (party == null)
            {
                from.SendMessage("You must be in a party to share quests.");
                return;
            }

            // Get player's current active quest
            var activeQuests = GetPlayerActiveQuests(from);
            
            if (!activeQuests.Any())
            {
                from.SendMessage("You have no active quests to share.");
                return;
            }

            // If multiple quests, prompt for selection
            int questId = 0;
            if (activeQuests.Count > 1)
            {
                from.SendMessage("You have multiple active quests. Which one would you like to share?");
                for (int i = 0; i < activeQuests.Count; i++)
                {
                    var quest = activeQuests[i];
                    from.SendMessage($"{i + 1}. {quest.Title}");
                }
                from.SendMessage("Usage: [ShareQuest <number>]");
                return;
            }
            else
            {
                questId = activeQuests[0].QuestId;
            }

            // Share the quest
            bool success = PartyQuestSharingSystem.ShareQuestWithParty(from, questId);
            
            if (success)
            {
                from.SendMessage("Quest shared with party successfully!");
            }
        }

        /// <summary>
        /// Accept quest command handler
        /// </summary>
        [Usage("AcceptQuest")]
        [Description("Accept a shared quest")]
        private static void AcceptQuest_OnCommand(CommandEventArgs e)
        {
            Mobile from = e.Mobile;
            
            if (from == null || from.Deleted)
                return;

            if (e.Length == 0)
            {
                from.SendMessage("Usage: [AcceptQuest <questId>]");
                return;
            }

            if (!int.TryParse(e.GetString(0), out int questId))
            {
                from.SendMessage("Invalid quest ID format.");
                return;
            }

            // Accept the shared quest
            bool success = PartyQuestSharingSystem.AcceptSharedQuest(from, questId);
            
            if (success)
            {
                from.SendMessage("Quest accepted successfully!");
            }
        }

        /// <summary>
        /// Decline quest command handler
        /// </summary>
        [Usage("DeclineQuest")]
        [Description("Decline a shared quest")]
        private static void DeclineQuest_OnCommand(CommandEventArgs e)
        {
            Mobile from = e.Mobile;
            
            if (from == null || from.Deleted)
                return;

            if (e.Length == 0)
            {
                from.SendMessage("Usage: [DeclineQuest <questId>]");
                return;
            }

            if (!int.TryParse(e.GetString(0), out int questId))
            {
                from.SendMessage("Invalid quest ID format.");
                return;
            }

            // Decline the shared quest
            bool success = PartyQuestSharingSystem.DeclineSharedQuest(from, questId);
            
            if (success)
            {
                from.SendMessage("Quest declined successfully.");
            }
        }

        /// <summary>
        /// Quest status command handler
        /// </summary>
        [Usage("QuestStatus")]
        [Description("Show shared quest status")]
        private static void QuestStatus_OnCommand(CommandEventArgs e)
        {
            Mobile from = e.Mobile;
            
            if (from == null || from.Deleted)
                return;

            var sharedQuests = PartyQuestSharingSystem.GetPlayerSharedQuests(from);
            
            if (!sharedQuests.Any())
            {
                from.SendMessage("You have no shared quests.");
                return;
            }

            from.SendMessage("=== SHARED QUEST STATUS ===");
            
            foreach (var quest in sharedQuests)
            {
                var progress = SharedProgressTracker.GetPlayerProgress(quest.QuestId, from);
                
                from.SendMessage($"Quest: {quest.QuestTitle}");
                from.SendMessage($"Status: {(quest.IsCompleted ? "Completed" : "Active")}");
                
                if (progress != null)
                {
                    from.SendMessage($"Accepted: {progress.HasAccepted}");
                    from.SendMessage($"Progress: {progress.TotalContribution} contributions");
                    
                    if (progress.ObjectiveProgress.Any())
                    {
                        from.SendMessage("Objectives:");
                        foreach (var kvp in progress.ObjectiveProgress)
                        {
                            var objective = quest.CooperativeObjectives.FirstOrDefault(o => o.ObjectiveId == kvp.Key);
                            var required = objective?.RequiredCount ?? 0;
                            from.SendMessage($"  {kvp.Key}: {kvp.Value}/{requiredProgress}");
                        }
                    }
                }
                
                from.SendMessage("---");
            }
        }

        /// <summary>
        /// Party chat command handler
        /// </summary>
        [Usage("PartyChat")]
        [Description("Send message to party members")]
        private static void PartyChat_OnCommand(CommandEventArgs e)
        {
            Mobile from = e.Mobile;
            
            if (from == null || from.Deleted)
                return;

            var party = from.Party as Party;
            if (party == null)
            {
                from.SendMessage("You must be in a party to use party chat.");
                return;
            }

            if (e.Length == 0)
            {
                from.SendMessage("Usage: [PartyChat <message>]");
                return;
            }

            string message = e.GetString(0);
            
            // Send message to all party members
            foreach (var member in party.Members)
            {
                if (member.Mobile != null && !member.Mobile.Deleted)
                {
                    member.Mobile.SendMessage($"[Party] {from.Name}: {message}");
                }
            }
        }

        /// <summary>
        /// Show multiplayer quest help
        /// </summary>
        private static void ShowMultiplayerQuestHelp(Mobile from)
        {
            from.SendMessage("=== MULTIPLAYER QUEST COMMANDS ===");
            from.SendMessage("Usage: [MultiplayerQuest <command>");
            from.SendMessage("");
            from.SendMessage("Administrative commands:");
            from.SendMessage("  stats     - Show multiplayer quest statistics");
            from.SendMessage("  report    - Generate comprehensive report");
            from.SendMessage("  reset     - Reset all statistics");
            from.SendMessage("  validate  - Validate quest for multiplayer compatibility");
            from.SendMessage("  sharing   - Show quest sharing statistics");
            from.SendMessage("  progress  - Show progress tracking statistics");
            from.SendMessage("  rewards   - Show reward distribution statistics");
            from.SendMessage("  communication - Show communication statistics");
            from.SendMessage("");
            from.SendMessage("Player commands:");
            from.SendMessage("  ShareQuest - Share current quest with party");
            from.SendMessage("  AcceptQuest - Accept a shared quest");
            from.SendMessage("  DeclineQuest - Decline a shared quest");
            from.SendMessage("  QuestStatus - Show shared quest status");
            from.SendMessage("  PartyChat - Send message to party members");
            from.SendMessage("");
            from.SendMessage("Examples:");
            from.SendMessage("  [MultiplayerQuest stats] - Show current statistics");
            from.WriteLine("  [ShareQuest] - Share your current quest with party");
            from.SendMessage("  [AcceptQuest 123] - Accept shared quest with ID 123");
            from.SendMessage("  [PartyChat Let's meet at the dungeon entrance] - Send message to party");
        }

        /// <summary>
        /// Show multiplayer quest statistics
        /// </summary>
        private static void ShowMultiplayerStats(Mobile from)
        {
            try
            {
                var sharingStats = PartyQuestSharingSystem.GetStatistics();
                var progressStats = SharedProgressTracker.GetStatistics();
                var rewardStats = PartyRewardDistribution.GetStatistics();
                var communicationStats = QuestCommunicationSystem.GetStatistics();

                from.SendMessage("=== MULTIPLAYER QUEST STATISTICS ===");
                from.SendMessage($"Generated: {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}");
                from.SendMessage("");
                from.SendMessage("SHARING STATISTICS:");
                from.SendMessage($"  Total Shared Quests: {sharingStats.TotalSharedQuests}");
                from.SendMessage($"  Completed Quests: {sharingStats.CompletedQuests}");
                from.SendMessage($"  Active Quests: {sharingStats.ActiveQuests}");
                from.SendMessage($"  Completion Rate: {sharingStats.CompletionRate:P1}");
                from.SendMessage("");
                from.SendMessage("PROGRESS TRACKING:");
                from.SendMessage($"  Total Tracked Quests: {progressStats.TotalTrackedQuests}");
                from.SendMessage($"  Active Quests: {progressStats.ActiveQuests}");
                from.SendMessage($"  Completed Quests: {progressStats.CompletedQuests}");
                from.SendMessage($"  Completion Rate: {progressStats.CompletionRate:P1}");
                from.SendMessage($"  Total Participants: {progressStats.TotalParticipants}");
                from.SendMessage("");
                from.SendMessage("REWARD DISTRIBUTION:");
                from.SendMessage($"  Total Distributions: {rewardStats.TotalDistributions}");
                from.SendMessage($"  Average Rewards/Player: {rewardStats.AverageRewardsPerPlayer:F1}");
                from.SendMessage($"  Last Distribution: {rewardStats.LastDistribution:yyyy-MM-dd HH:mm:ss}");
                from.SendMessage("");
                from.SendMessage("COMMUNICATION:");
                from.SendMessage($"  Total Messages: {communicationStats.TotalMessages}");
                from.SendMessage($"  Total Subscriptions: {communicationStats.TotalSubscriptions}");
                from.SendMessage($"  Average Messages/Quest: {communicationStats.AverageMessagesPerQuest:F1}");
                from.SendMessage($"  Last Message: {communicationStats.LastMessage:yyyy-MM-dd HH:mm:ss}");
            }
            catch (Exception ex)
            {
                from.SendMessage($"Error retrieving stats: {ex.Message}");
            }
        }

        /// <summary>
        /// Show sharing statistics
        /// </summary>
        private static void ShowSharingStats(Mobile from)
        {
            try
            {
                var stats = PartyQuestSharingSystem.GetStatistics();
                
                from.SendMessage("=== QUEST SHARING STATISTICS ===");
                from.SendMessage($"Generated: {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}");
                from.SendMessage("");
                from.SendMessage("OVERALL STATISTICS:");
                from.SendMessage($"  Total Shared Quests: {stats.TotalSharedQuests}");
                from.SendMessage($"  Completed Quests: {stats.CompletedQuests}");
                from.SendMessage($"  Active Quests: {stats.ActiveQuests}");
                from.SendMessage($"  Acceptance Rate: {stats.CompletionRate:P1}");
                from.SendMessage("");
                from.SendMessage("RECENT SHARING ACTIVITY:");
                from.SendMessage($"  Last Activity: {stats.LastActivity:yyyy-MM-dd HH:mm:ss}");
            }
            catch (Exception ex)
            {
                from.SendMessage($"Error retrieving sharing stats: {ex.Message}");
            }
        }

        /// <summary>
        /// Show progress statistics
        /// </summary>
        private static void ShowProgressStats(Mobile from)
        {
            try
            {
                var stats = SharedProgressTracker.GetStatistics();
                
                from.SendMessage("=== PROGRESS TRACKING STATISTICS ===");
                from.SendMessage($"Generated: {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}");
                from.SendMessage("");
                from.SendMessage("OVERALL STATISTICS:");
                from.SendMessage($"  Total Tracked Quests: {stats.TotalTrackedQuests}");
                from.SendMessage($"  Active Quests: {stats.ActiveQuests}");
                from.SendMessage($"  Completed Quests: {stats.CompletedQuests}");
                from.SendMessage($"  Completion Rate: {stats.CompletionRate:P1}");
                from.SendMessage($"  Total Participants: {stats.TotalParticipants}");
                from.SendMessage("");
                from.SendMessage("RECENT ACTIVITY:");
                from.SendMessage($"  Last Activity: {stats.LastActivity:yyyy-MM-dd HH:mm:ss}");
            }
            catch (Exception ex)
            {
                from.SendMessage($"Error retrieving progress stats: {ex.Message}");
            }
        }

        /// <summary>
        /// Show reward statistics
        /// </summary>
        private static void ShowRewardStats(Mobile from)
        {
            try
            {
                var stats = PartyRewardDistribution.GetStatistics();
                
                from.SendMessage("=== REWARD DISTRIBUTION STATISTICS ===");
                from.SendMessage($"Generated: {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}");
                from.SendMessage("");
                from.SendMessage("OVERALL STATISTICS:");
                from.SendMessage($"  Total Distributions: {stats.TotalDistributions}");
                from.SendMessage($"  Average Rewards/Player: {stats.AverageRewardsPerPlayer:F1}");
                from.SendMessage($"  Last Distribution: {stats.LastDistribution:yyyy-MM-dd HH:mm:ss}");
                from.SendMessage("");
                from.SendMessage("DISTRIBUTION METHODS:");
                foreach (var kvp in stats.DistributionMethods)
                {
                    from.SendMessage($"  {kvp.Key}: {kvp.Value}");
                }
            }
            catch (Exception ex)
            {
                from.SendMessage($"Error retrieving reward stats: {ex.Message}");
            }
        }

        /// <summary>
        /// Show communication statistics
        /// </summary>
        private static void ShowCommunicationStats(Mobile from)
        {
            try
            {
                var stats = QuestCommunicationSystem.GetStatistics();
                
                from.SendMessage("=== COMMUNICATION STATISTICS ===");
                from.SendMessage($"Generated: {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}");
                from.SendMessage("");
                from.SendMessage("OVERALL STATISTICS:");
                from.SendMessage($"  Total Messages: {stats.TotalMessages}");
                from.SendMessage($"  Total Subscriptions: {stats.TotalSubscriptions}");
                from.SendMessage($"  Average Messages/Quest: {stats.AverageMessagesPerQuest:F1}");
                from.SendMessage($"  Last Message: {stats.LastMessage:yyyy-MM-dd HH:mm:ss}");
                from.SendMessage("");
                from.SendMessage("MESSAGE TYPES:");
                foreach (var kvp in stats.MessageTypes)
                {
                    from.SendMessage($"  {kvp.Key}: {kvp.Value}");
                }
            }
            catch (Exception ex)
            {
                from.SendMessage($"Error retrieving communication stats: {ex.Message}");
            }
        }

        /// <summary>
        /// Reset all multiplayer quest statistics
        /// </summary>
        private static void ResetMultiplayerQuestStats(Mobile from)
        {
            // This would reset all statistics tracking
            PartyQuestSharingSystem.ResetStatistics();
            SharedProgressTracker.ResetStatistics();
            PartyRewardDistribution.ResetStatistics();
            QuestCommunicationSystem.ResetStatistics();
            
            from.SendMessage("[MultiplayerQuest] All multiplayer quest statistics have been reset.");
        }

        /// <summary>
        /// Validate quest command handler
        /// </summary>
        private static void ValidateQuestCommand(Mobile from, CommandEventArgs e)
        {
            if (e.Length < 2)
            {
                from.SendMessage("Usage: [MultiplayerQuest validate <questId>");
                return;
            }

            if (!int.TryParse(e.GetString(1), out int questId))
            {
                from.SendMessage("Invalid quest ID format.");
                return;
            }

            // Get the quest
            var quest = GetQuestById(questId);
            if (quest == null)
            {
                from.SendMessage($"Quest {questId} not found.");
                return;
            }

            // Get the player's party
            var party = from.Party as Party;
            if (party == null)
            {
                from.SendMessage("You must be in a party to validate quests.");
                return;
            }

            // Validate the quest
            var settings = new SharedQuestSettings(); // Use default settings
            var validation = MultiplayerQuestValidator.ValidateQuestForMultiplayer(quest, party, settings);
            
            from.SendMessage($"=== QUEST VALIDATION RESULTS ===");
            from.SendMessage($"Quest ID: {questId}");
            from.SendMessage($"Quest Type: {GetQuestType(quest)}");
            from.SendMessage($"Party Size: {party.Members.Count}");
            from.SendMessage($"Validation Result: {validation.Severity}");
            from.SendMessage("");
            
            if (validation.IsValid)
            {
                from.SendMessage("✓ Quest is valid for multiplayer");
            }
            else
            {
                from.SendMessage("✗ Quest has validation issues:");
            }
            
            if (validation.Errors.Any())
            {
                from.SendMessage("ERRORS:");
                foreach (var error in validation.Errors)
                {
                    from.SendMessage($"  • {error}");
                }
            }
            
            if (validation.Warnings.Any())
            {
                from.SendMessage("WARNINGS:");
                foreach (var warning in validation.Warnings)
                {
                    from.SendMessage($"  • {warning}");
                }
            }
            
            if (validation.Issues.Any())
            {
                from.SendMessage("ISSUES:");
                foreach (var issue in validation.Issues)
                {
                    from.SendMessage($"  • [{issue.Severity}] {issue.Description} ({issue.Recommendation})");
                }
            }
        }

        /// <summary>
        /// Generate comprehensive multiplayer quest report
        /// </summary>
        private static string GenerateMultiplayerQuestReport()
        {
            var report = new System.Text.StringBuilder();
            
            report.AppendLine("=== MULTIPLAYER QUEST SYSTEM REPORT ===");
            report.AppendLine($"Generated: {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}");
            report.AppendLine();
            
            // Get statistics from all systems
            var sharingStats = PartyQuestSharingSystem.GetStatistics();
            
            report.AppendLine("=== SHARING STATISTICS ===");
            report.AppendLine($"Total Shared Quests: {sharingStats.TotalSharedQuests}");
            report.AppendLine($"Completed Quests: {sharingStats.CompletedQuests}");
            report.AppendLine($"Active Quests: {sharingStats.ActiveQuests}");
            report.AppendLine($"Acceptance Rate: {sharingStats.CompletionRate:P1}");
            report.AppendLine($"Last Activity: {sharingStats.LastActivity:yyyy-MM-dd HH:mm:ss}");
            report.AppendLine();
            
            report.AppendLine("=== PROGRESS TRACKING ===");
            report.AppendLine($"Total Tracked Quests: {progressStats.TotalTrackedQuests}");
            report.AppendLine($"Active Quests: {progressStats.ActiveQuests}");
            report.AppendLine($"Completed Quests: {progressStats.CompletedQuests}");
            report.AppendLine($"Completion Rate: {progressStats.CompletionRate:P1}");
            report.AppendLine($"Total Participants: {progressStats.TotalParticipants}");
            report.AppendLine($"Last Activity: {progressStats.LastActivity:yyyy-MM-dd HH:mm:ss}");
            report.AppendLine();
            
            report.AppendLine("=== REWARD DISTRIBUTION ===");
            report.AppendLine($"Total Distributions: {rewardStats.TotalDistributions}");
            report.AppendLine($"Average Rewards/Player: {rewardStats.AverageRewardsPerPlayer:F2}");
            report.AppendLine($"Last Distribution: {rewardStats.LastDistribution:yyyy-MM-dd HH:mm:ss}");
            report.AppendLine();
            
            report.AppendLine("=== COMMUNICATION ===");
            report.AppendLine($"Total Messages: {communicationStats.TotalMessages}");
            report.AppendLine($"Total Subscriptions: {communicationStats.TotalSubscriptions}");
            report.AppendLine($"Average Messages/Quest: {communicationStats.AverageMessagesPerQuest:F2}");
            report.AppendLine($"Last Message: {communicationStats.LastMessage:yyyy-MM-dd HH:mm:ss}");
            report.AppendLine();
            
            report.AppendLine("=== RECOMMENDATIONS ===");
            
            // Add recommendations based on statistics
            if (sharingStats.CompletionRate < 0.5)
            {
                report.AppendLine("• Consider adjusting quest difficulty for party size");
            }
            
            if (progressStats.CompletionRate < 0.6)
            {
                report.AppendLine("• Review cooperative objective design");
            }
            
            if (communicationStats.AverageMessagesPerQuest < 2)
            {
                report.AppendLine("• Encourage more party communication");
            }
            
            if (rewardStats.AverageRewardsPerPlayer < 50)
            {
                report.AppendLine("• Consider increasing reward values for multiplayer quests");
            }
            
            report.AppendLine();
            report.AppendLine("=== ACTIVE SHARED QUESTS ===");
            var activeQuests = PartyQuestSharingSystem.GetPlayerSharedQuests(null); // Get all active quests
            
            foreach (var quest in activeQuests.Take(10))
            {
                var progress = SharedProgressTracker.GetPlayerProgress(quest.QuestId, null);
                var party = quest.Party as Party;
                
                report.AppendLine($"• {quest.QuestTitle}");
                report.AppendLine($"  Party: {party?.Leader?.Name ?? "Unknown"} ({party?.Members.Count ?? 0} members)");
                report.AppendLine($"  Progress: {(progress?.HasCompleted ? "Completed" : "Active")}");
                report.AppendLine($"  Shared By: {quest.SharedBy?.Name ?? "Unknown"}");
                report.AppendLine($"  Shared At: {quest.SharedAt:yyyy-MM-dd HH:mm:ss}");
                report.AppendLine();
            }
            
            return report.ToString();
        }

        /// <summary>
        /// Get player's active quests
        /// </summary>
        private static List<object> GetPlayerActiveQuests(PlayerMobile player)
        {
            var quests = new List<object>();
            
            // Get DynamicQuests
            var dynamicQuests = GetPlayerDynamicQuests(player);
            if (dynamicQuests.Any())
            {
                quests.AddRange(dynamicQuests);
            }
            
            // Get VystiaQuests
            var vystiaQuests = GetPlayerVystiaQuests(player);
            if (vystiaQuests.Any())
            {
                quests.AddRange(vystiaQuests);
            }
            
            // Get traditional quests
            var traditionalQuests = GetPlayerTraditionalQuests(player);
            if (traditionalQuests.Any())
            {
                quests.AddRange(traditionalQuests);
            }
            
            return quests;
        }

        /// <summary>
        /// Get player's DynamicQuests
        /// </summary>
        private static List<DynamicQuest> GetPlayerDynamicQuests(PlayerMobile player)
        {
            // This would be implemented based on the DynamicQuest system
            return new List<DynamicQuest>();
        }

        /// <summary>
        /// Get player's VystiaQuests
        /// </summary>
        private static List<VystiaQuest> GetPlayerVystiaQuests(PlayerMobile player)
        {
            // This would be implemented based on the VystiaQuest system
            return new List<VystiaQuest>();
        }

        /// <summary>
        /// Get player's traditional quests
        /// </summary>
        private static List<BaseQuest> GetPlayerTraditionalQuests(PlayerMobile player)
        {
            // This would be implemented based on the traditional quest system
            return new List<BaseQuest>();
        }

        /// <summary>
        /// Get quest by ID
        /// </summary>
        private static object GetQuestById(int questId)
        {
            // Try DynamicQuest first
            var dynamicQuest = DynamicQuestManager.GetQuest(questId);
            if (dynamicQuest != null)
                return dynamicQuest;

            // Try VystiaQuest
            var vystiaQuest = VystiaQuestSystem.GetQuest(questId);
            if (vystiaQuest != null)
                return vystiaQuest;

            // Try traditional quest
            // This would be implemented based on the traditional quest system

            return null;
        }

        /// <summary>
        /// Get quest type
        /// </summary>
        private static string GetQuestType(object quest)
        {
            if (quest is DynamicQuest)
                return "dynamic";
            
            if (quest is VystiaQuest)
                return "vystia";
            
            if (quest is BaseQuest)
                return "traditional";
            
            return "unknown";
        }
    }
}
