using System;
using System.Collections.Generic;
using System.Linq;
using Server.Mobiles;
using Server.Gumps;
using Server.Network;

namespace Server.Services.QuestJournal
{
    /// <summary>
    /// Renders quest progress in a standardized format
    /// </summary>
    public static class QuestProgressRenderer
    {
        /// <summary>
        /// Render progress bar for a quest
        /// </summary>
        public static void RenderProgressBar(Gump gump, int x, int y, int width, int height, QuestProgressInfo progress, string title = null)
        {
            if (!string.IsNullOrEmpty(title))
            {
                gump.AddHtml(x, y - 20, width, 20, Color(title, 0xFFFFFF), false, false);
            }

            // Background
            gump.AddImageTiled(x, y, width, height, 0x242F);
            
            // Progress fill
            int fillWidth = (int)(width * progress.ProgressPercentage);
            if (fillWidth > 0)
            {
                gump.AddImageTiled(x, y, fillWidth, height, 0x23D4); // Green fill
            }
            
            // Border
            gump.AddImageTiled(x - 2, y - 2, width + 4, height + 4, 0x242E);
            
            // Progress text
            string progressText = $"{progress.ProgressText} ({progress.ProgressPercentage:P0})";
            gump.AddHtml(x, y + 2, width, height - 4, Center(Color(progressText, "#FFFFFF")), false, false);
        }

        /// <summary>
        /// Render objective list
        /// </summary>
        public static void RenderObjectives(Gump gump, int x, int y, int width, List<QuestObjectiveInfo> objectives, string title = "Objectives:")
        {
            gump.AddHtml(x, y, width, 20, Color(title, "#00FFFF"), false, false);
            y += 25;

            foreach (var objective in objectives)
            {
                string statusColor = objective.IsCompleted ? "#00FF00" : "#FFFFFF";
                string statusIcon = objective.IsCompleted ? "✓" : "○";
                
                string objectiveText = $"{statusIcon} {objective.Description} - {objective.ProgressText}";
                gump.AddHtml(x + 10, y, width - 20, 20, Color(objectiveText, statusColor), false, false);
                y += 22;
            }
        }

        /// <summary>
        /// Render rewards list
        /// </summary>
        public static void RenderRewards(Gump gump, int x, int y, int width, List<QuestRewardInfo> rewards, string title = "Rewards:")
        {
            gump.AddHtml(x, y, width, 20, Color(title, 0xFFFF00), false, false);
            y += 25;

            foreach (var reward in rewards)
            {
                string rewardText = $"• {reward.Description}";
                if (reward.Amount > 0)
                    rewardText += $" ({reward.Amount})";
                
                gump.AddHtml(x + 10, y, width - 20, 20, Color(rewardText, 0xFFFFFF), false, false);
                y += 22;
            }
        }

        /// <summary>
        /// Render quest status indicators
        /// </summary>
        public static void RenderStatus(Gump gump, int x, int y, UnifiedQuestInfo quest)
        {
            string statusText;
            int statusColor;
            
            if (quest.IsCompleted)
            {
                statusText = "COMPLETED";
                statusColor = 0x00FF00;
            }
            else if (quest.IsActive)
            {
                statusText = "IN PROGRESS";
                statusColor = 0xFFFF00;
            }
            else
            {
                statusText = "AVAILABLE";
                statusColor = 0xFFFFFF;
            }

            gump.AddHtml(x, y, 100, 20, Color(statusText, statusColor), false, false);
        }

        /// <summary>
        /// Render time remaining (if applicable)
        /// </summary>
        public static void RenderTimeRemaining(Gump gump, int x, int y, UnifiedQuestInfo quest)
        {
            if (quest.TimeRemaining.HasValue)
            {
                var timeRemaining = quest.TimeRemaining.Value;
                string timeText;
                
                if (timeRemaining.TotalHours > 1)
                    timeText = $"Time: {timeRemaining.Hours}h {timeRemaining.Minutes}m";
                else if (timeRemaining.TotalMinutes > 1)
                    timeText = $"Time: {timeRemaining.Minutes}m {timeRemaining.Seconds}s";
                else
                    timeText = $"Time: {timeRemaining.Seconds}s";
                
                int color = timeRemaining.TotalHours < 1 ? 0x00FF00 : timeRemaining.TotalHours < 6 ? 0xFFFF00 : 0xFF0000;
                gump.AddHtml(x, y, 150, 20, Color(timeText, color), false, false);
            }
        }

        /// <summary>
        /// Render quest difficulty indicator
        /// </summary>
        public static void RenderDifficulty(Gump gump, int x, int y, UnifiedQuestInfo quest)
        {
            string difficultyText = $"Difficulty: {quest.Difficulty}";
            int difficultyColor = GetDifficultyColor(quest.Difficulty);
            
            gump.AddHtml(x, y, 120, 20, Color(difficultyText, difficultyColor), false, false);
        }

        /// <summary>
        /// Get color based on difficulty
        /// </summary>
        private static int GetDifficultyColor(string difficulty)
        {
            switch (difficulty?.ToLower())
            {
                case "initiation":
                    return 0x00FF00; // Green
                case "apprentice":
                    return 0xFFFF00; // Yellow
                case "journeyman":
                    return 0xFF8800; // Orange
                case "master":
                    return 0xFF0000; // Red
                default:
                    return 0xFFFFFF; // White
            }
        }

        /// <summary>
        /// Center text with color
        /// </summary>
        private static string Color(string text, int color)
        {
            return $"<BASEFONT COLOR=#{color:X}>{text}</BASEFONT>";
        }

        /// <summary>
        /// Center text
        /// </summary>
        private static string Center(string text)
        {
            return $"<CENTER>{text}</CENTER>";
        }
    }

    /// <summary>
    /// Quest filtering system
    /// </summary>
    public static class QuestFilterSystem
    {
        /// <summary>
        /// Filter quests based on criteria
        /// </summary>
        public static List<UnifiedQuestInfo> FilterQuests(List<UnifiedQuestInfo> quests, QuestFilterType filterType, string filterValue = null, PlayerMobile player = null)
        {
            if (quests == null)
                return new List<UnifiedQuestInfo>();

            switch (filterType)
            {
                case QuestFilterType.All:
                    return quests;

                case QuestFilterType.Active:
                    return quests.Where(q => q.IsActive).ToList();

                case QuestFilterType.Completed:
                    return quests.Where(q => q.IsCompleted).ToList();

                case QuestFilterType.Vystia:
                    return quests.Where(q => q.QuestType == "Vystia").ToList();

                case QuestFilterType.Dynamic:
                    return quests.Where(q => q.QuestType == "Dynamic").ToList();

                case QuestFilterType.Traditional:
                    return quests.Where(q => q.QuestType == "Traditional").ToList();

                case QuestFilterType.ByDifficulty:
                    if (!string.IsNullOrEmpty(filterValue))
                        return quests.Where(q => q.Difficulty.Equals(filterValue, StringComparison.OrdinalIgnoreCase)).ToList();
                    break;

                case QuestFilterType.ByLocation:
                    if (!string.IsNullOrEmpty(filterValue))
                        return quests.Where(q => q.Location != null && q.Location.Contains(filterValue, StringComparison.OrdinalIgnoreCase)).ToList();
                    break;

                case QuestFilterType.ByType:
                    if (!string.IsNullOrEmpty(filterValue))
                        return quests.Where(q => q.QuestType.Equals(filterValue, StringComparison.OrdinalIgnoreCase)).ToList();
                    break;
            }

            return quests;
        }

        /// <summary>
        /// Search quests by text
        /// </summary>
        public static List<UnifiedQuestInfo> SearchQuests(List<UnifiedQuestInfo> quests, string searchTerm)
        {
            if (string.IsNullOrEmpty(searchTerm) || quests == null)
                return quests;

            searchTerm = searchTerm.ToLower();
            return quests.Where(q => 
                q.Title.ToLower().Contains(searchTerm) ||
                q.Description.ToLower().Contains(searchTerm) ||
                q.Location?.ToLower().Contains(searchTerm) == true ||
                q.QuestType.ToLower().Contains(searchTerm)
            ).ToList();
        }
    }

    /// <summary>
    /// Quest sorting system
    /// </summary>
    public static class QuestSortManager
    {
        /// <summary>
        /// Sort quests based on criteria
        /// </summary>
        public static List<UnifiedQuestInfo> SortQuests(List<UnifiedQuestInfo> quests, QuestSortType sortType, bool ascending = true)
        {
            if (quests == null)
                return new List<UnifiedQuestInfo>();

            switch (sortType)
            {
                case QuestSortType.ByName:
                    return ascending ? quests.OrderBy(q => q.Title).ToList() : quests.OrderByDescending(q => q.Title).ToList();

                case QuestSortType.ByDifficulty:
                    return ascending ? quests.OrderBy(q => GetDifficultyOrder(q.Difficulty)).ToList() : quests.OrderByDescending(q => GetDifficultyOrder(q.Difficulty)).ToList();

                case QuestSortType.ByProgress:
                    return ascending ? quests.OrderBy(q => q.Progress.ProgressPercentage).ToList() : quests.OrderByDescending(q => q.Progress.ProgressPercentage).ToList();

                case QuestSortType.ByType:
                    return ascending ? quests.OrderBy(q => q.QuestType).ToList() : quests.OrderByDescending(q => q.QuestType).ToList();

                case QuestSortType.ByLocation:
                    return ascending ? quests.OrderBy(q => q.Location).ToList() : quests.OrderByDescending(q => q.Location).ToList();

                case QuestSortType.ByTimeRemaining:
                    return ascending ? quests.OrderBy(q => q.TimeRemaining ?? TimeSpan.MaxValue).ToList() : quests.OrderByDescending(q => q.TimeRemaining ?? TimeSpan.MinValue).ToList();

                case QuestSortType.ByDateStarted:
                    return ascending ? quests.OrderBy(q => q.StartedAt ?? DateTime.MaxValue).ToList() : quests.OrderByDescending(q => q.StartedAt ?? DateTime.MinValue).ToList();

                default:
                    return quests;
            }
        }

        /// <summary>
        /// Get difficulty order for sorting
        /// </summary>
        private static int GetDifficultyOrder(string difficulty)
        {
            switch (difficulty?.ToLower())
            {
                case "initiation": return 1;
                case "apprentice": return 2;
                case "journeyman": return 3;
                case "master": return 4;
                default: return 5;
            }
        }
    }
}
