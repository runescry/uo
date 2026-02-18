using System;
using System.Collections.Generic;
using System.Linq;
using Server.Mobiles;
using Server.Custom.VystiaClasses.Quests;
using Server.Custom.VystiaClasses.Quests.Generation;
using Server.Services.QuestPersistence;

namespace Server.Services.QuestVariety
{
    /// <summary>
    /// Learns and tracks player preferences for personalized quest generation
    /// </summary>
    public static class PlayerPreferenceLearning
    {
        private static readonly Dictionary<Serial, PlayerPreferences> s_PlayerPreferences = new Dictionary<Serial, PlayerPreferences>();
        private static readonly object s_Lock = new object();

        /// <summary>
        /// Initialize the player preference system
        /// </summary>
        public static void Initialize()
        {
            Console.WriteLine("[PlayerPreferenceLearning] Initialized");
        }

        /// <summary>
        /// Get player preferences
        /// </summary>
        public static PlayerPreferences GetPlayerPreferences(PlayerMobile player)
        {
            if (player == null)
                return null;

            lock (s_Lock)
            {
                if (!s_PlayerPreferences.ContainsKey(player.Serial))
                {
                    s_PlayerPreferences[player.Serial] = CreateDefaultPreferences(player);
                }

                return s_PlayerPreferences[player.Serial];
            }
        }

        /// <summary>
        /// Update preferences based on quest completion
        /// </summary>
        public static void UpdatePreferencesFromQuest(PlayerMobile player, DynamicQuest quest, int rating = 0)
        {
            if (player == null || quest == null)
                return;

            var preferences = GetPlayerPreferences(player);
            if (preferences == null)
                return;

            lock (s_Lock)
            {
                // Update quest completion statistics
                preferences.TotalQuestsCompleted++;
                preferences.LastUpdated = DateTime.UtcNow;

                // Update average rating
                if (rating > 0)
                {
                    preferences.AverageQuestRating = (preferences.AverageQuestRating * (preferences.TotalQuestsCompleted - 1) + rating) / preferences.TotalQuestsCompleted;
                }

                // Extract quest features
                var questFeatures = ExtractQuestFeatures(quest);

                // Update theme preferences
                UpdateThemePreferences(preferences, questFeatures, rating);

                // Update location preferences
                UpdateLocationPreferences(preferences, questFeatures, rating);

                // Update difficulty preferences
                UpdateDifficultyPreferences(preferences, questFeatures, rating);

                // Update quest type preferences
                UpdateQuestTypePreferences(preferences, questFeatures, rating);

                // Update liked/disliked quests
                UpdateQuestPreferences(preferences, quest, rating);

                // Save preferences
                SavePreferences(player);
            }
        }

        /// <summary>
        /// Get personalized quest recommendations
        /// </summary>
        public static List<string> GetPersonalizedRecommendations(PlayerMobile player)
        {
            var preferences = GetPlayerPreferences(player);
            if (preferences == null)
                return new List<string>();

            var recommendations = new List<string>();

            // Theme recommendations
            var topThemes = preferences.ThemePreferences
                .Select(kvp => new { Key = kvp.Key, Value = kvp.Value })
                .OrderByDescending(kvp => kvp.Value)
                .Take(3)
                .ToList();

            if (topThemes.Any())
            {
                recommendations.Add($"Preferred themes: {string.Join(", ", topThemes.Select(t => t.Key))}");
            }

            // Location recommendations
            var topLocations = preferences.LocationPreferences
                .Select(kvp => new { Key = kvp.Key, Value = kvp.Value })
                .OrderByDescending(kvp => kvp.Value)
                .Take(3)
                .ToList();

            if (topLocations.Any())
            {
                recommendations.Add($"Preferred locations: {string.Join(", ", topLocations.Select(l => l.Key))}");
            }

            // Avoid disliked quests
            if (preferences.DislikedQuests.Any())
            {
                recommendations.Add($"Avoid themes: {string.Join(", ", preferences.DislikedQuests.Take(3))}");
            }

            return recommendations;
        }

        /// <summary>
        /// Adjust quest generation parameters based on preferences
        /// </summary>
        public static Dictionary<string, object> GetGenerationParameters(PlayerMobile player)
        {
            var preferences = GetPlayerPreferences(player);
            if (preferences == null)
                return new Dictionary<string, object>();

            var parameters = new Dictionary<string, object>();

            // Theme bias
            if (preferences.ThemePreferences.Any())
            {
                var topTheme = preferences.ThemePreferences.OrderByDescending(kvp => kvp.Value).First();
                parameters["theme_bias"] = topTheme.Key;
            }

            // Difficulty adjustment
            if (preferences.DifficultyPreferences.Any())
            {
                var avgDifficulty = preferences.DifficultyPreferences.Values.Average();
                parameters["difficulty_adjustment"] = avgDifficulty > 0 ? "harder" : "easier";
            }

            // Location preference
            if (preferences.LocationPreferences.Any())
            {
                var topLocation = preferences.LocationPreferences.OrderByDescending(kvp => kvp.Value).First();
                parameters["location_preference"] = topLocation.Key;
            }

            // Avoid disliked themes
            if (preferences.DislikedQuests.Any())
            {
                parameters["avoid_themes"] = preferences.DislikedQuests.ToList();
            }

            return parameters;
        }

        /// <summary>
        /// Create default preferences for a new player
        /// </summary>
        private static PlayerPreferences CreateDefaultPreferences(PlayerMobile player)
        {
            return new PlayerPreferences
            {
                Player = player,
                ThemePreferences = new Dictionary<string, double>(),
                DifficultyPreferences = new Dictionary<string, double>(),
                LocationPreferences = new Dictionary<string, double>(),
                QuestTypePreferences = new Dictionary<string, double>(),
                DislikedQuests = new List<string>(),
                PreferredQuests = new List<string>(),
                AverageQuestRating = 0.0,
                TotalQuestsCompleted = 0,
                LastUpdated = DateTime.UtcNow
            };
        }

        /// <summary>
        /// Extract features from a quest
        /// </summary>
        private static Dictionary<string, List<string>> ExtractQuestFeatures(DynamicQuest quest)
        {
            var features = new Dictionary<string, List<string>>();
            var text = $"{quest.Title} {quest.Description}".ToLower();

            // Extract theme keywords
            var themeKeywords = new List<string>();
            var themePatterns = new Dictionary<string, List<string>>
            {
                ["combat"] = new List<string> { "kill", "defeat", "battle", "fight", "enemy" },
                ["exploration"] = new List<string> { "explore", "discover", "find", "search", "investigate" },
                ["collection"] = new List<string> { "collect", "gather", "acquire", "obtain" },
                ["delivery"] = new List<string> { "deliver", "bring", "carry", "transport" },
                ["protection"] = new List<string> { "protect", "guard", "defend", "shield" },
                ["rescue"] = new List<string> { "rescue", "save", "help", "assist" }
            };

            foreach (var pattern in themePatterns)
            {
                if (pattern.Value.Any(keyword => text.Contains(keyword)))
                {
                    themeKeywords.Add(pattern.Key);
                }
            }

            features["themes"] = themeKeywords;

            // Extract location keywords
            var locationKeywords = new List<string>();
            var locationPatterns = new Dictionary<string, List<string>>
            {
                ["forest"] = new List<string> { "forest", "woods", "trees", "jungle" },
                ["dungeon"] = new List<string> { "dungeon", "cave", "crypt", "underground" },
                ["city"] = new List<string> { "city", "town", "village", "settlement" },
                ["mountain"] = new List<string> { "mountain", "peak", "hill", "cliff" },
                ["water"] = new List<string> { "lake", "river", "ocean", "sea", "beach" }
            };

            foreach (var pattern in locationPatterns)
            {
                if (pattern.Value.Any(keyword => text.Contains(keyword)))
                {
                    locationKeywords.Add(pattern.Key);
                }
            }

            features["locations"] = locationKeywords;

            // Extract difficulty indicators
            var difficultyKeywords = new List<string>();
            if (text.Contains("easy") || text.Contains("simple"))
                difficultyKeywords.Add("easy");
            if (text.Contains("hard") || text.Contains("difficult") || text.Contains("challenging"))
                difficultyKeywords.Add("hard");
            if (text.Contains("medium") || text.Contains("moderate"))
                difficultyKeywords.Add("medium");

            features["difficulty"] = difficultyKeywords;

            // Extract quest type keywords
            var questTypeKeywords = new List<string>();
            if (text.Contains("kill") || text.Contains("defeat"))
                questTypeKeywords.Add("combat");
            if (text.Contains("collect") || text.Contains("gather"))
                questTypeKeywords.Add("collection");
            if (text.Contains("deliver") || text.Contains("bring"))
                questTypeKeywords.Add("delivery");
            if (text.Contains("explore") || text.Contains("discover"))
                questTypeKeywords.Add("exploration");

            features["quest_types"] = questTypeKeywords;

            return features;
        }

        /// <summary>
        /// Update theme preferences based on quest rating
        /// </summary>
        private static void UpdateThemePreferences(PlayerPreferences preferences, Dictionary<string, List<string>> features, int rating)
        {
            var themes = features.ContainsKey("themes") ? features["themes"] : new List<string>();
            {
                if (!preferences.ThemePreferences.ContainsKey(theme))
                {
                    preferences.ThemePreferences[theme] = 0.0;
                }

                // Update preference based on rating
                double adjustment;
                if (rating >= 4)
                    adjustment = 0.2;  // Strong positive
                else if (rating >= 3)
                    adjustment = 0.1;  // Positive
                else if (rating >= 2)
                    adjustment = 0.0;  // Neutral
                else if (rating >= 1)
                    adjustment = -0.1; // Negative
                else
                    adjustment = -0.2;  // Strong negative

                preferences.ThemePreferences[theme] += adjustment;
                preferences.ThemePreferences[theme] = Math.Max(-1.0, Math.Min(1.0, preferences.ThemePreferences[theme]));
            }
        }

        /// <summary>
        /// Update location preferences based on quest rating
        /// </summary>
        private static void UpdateLocationPreferences(PlayerPreferences preferences, Dictionary<string, List<string>> features, int rating)
        {
            var locations = features.ContainsKey("locations") ? features["locations"] : new List<string>();
            foreach (var location in locations)
            {
                if (!preferences.LocationPreferences.ContainsKey(location))
                {
                    preferences.LocationPreferences[location] = 0.0;
                }

                double adjustment;
                if (rating >= 4)
                    adjustment = 0.15;
                else if (rating >= 3)
                    adjustment = 0.1;
                else if (rating >= 2)
                    adjustment = 0.0;
                else if (rating >= 1)
                    adjustment = -0.1;
                else
                    adjustment = -0.15;

                preferences.LocationPreferences[location] += adjustment;
                preferences.LocationPreferences[location] = Math.Max(-1.0, Math.Min(1.0, preferences.LocationPreferences[location]));
            }
        }

        /// <summary>
        /// Update difficulty preferences based on quest rating
        /// </summary>
        private static void UpdateDifficultyPreferences(PlayerPreferences preferences, Dictionary<string, List<string>> features, int rating)
        {
            var difficulties = features.ContainsKey("difficulty") ? features["difficulty"] : new List<string>();
            foreach (var difficulty in difficulties)
            {
                if (!preferences.DifficultyPreferences.ContainsKey(difficulty))
                {
                    preferences.DifficultyPreferences[difficulty] = 0.0;
                }

                double adjustment;
                if (rating >= 4)
                    adjustment = 0.1;
                else if (rating >= 3)
                    adjustment = 0.05;
                else if (rating >= 2)
                    adjustment = 0.0;
                else if (rating >= 1)
                    adjustment = -0.05;
                else
                    adjustment = -0.1;

                preferences.DifficultyPreferences[difficulty] += adjustment;
                preferences.DifficultyPreferences[difficulty] = Math.Max(-1.0, Math.Min(1.0, preferences.DifficultyPreferences[difficulty]));
            }
        }

        /// <summary>
        /// Update quest type preferences based on quest rating
        /// </summary>
        private static void UpdateQuestTypePreferences(PlayerPreferences preferences, Dictionary<string, List<string>> features, int rating)
        {
            var questTypes = features.ContainsKey("quest_types") ? features["quest_types"] : new List<string>();
            foreach (var questType in questTypes)
            {
                if (!preferences.QuestTypePreferences.ContainsKey(questType))
                {
                    preferences.QuestTypePreferences[questType] = 0.0;
                }

                double adjustment;
                if (rating >= 4)
                    adjustment = 0.15;
                else if (rating >= 3)
                    adjustment = 0.1;
                else if (rating >= 2)
                    adjustment = 0.0;
                else if (rating >= 1)
                    adjustment = -0.1;
                else
                    adjustment = -0.15;

                preferences.QuestTypePreferences[questType] += adjustment;
                preferences.QuestTypePreferences[questType] = Math.Max(-1.0, Math.Min(1.0, preferences.QuestTypePreferences[questType]));
            }
        }

        /// <summary>
        /// Update liked/disliked quest lists
        /// </summary>
        private static void UpdateQuestPreferences(PlayerPreferences preferences, DynamicQuest quest, int rating)
        {
            var questText = $"{quest.Title} {quest.Description}".ToLower();

            if (rating >= 4)
            {
                // Add to preferred quests
                if (!preferences.PreferredQuests.Contains(questText))
                {
                    preferences.PreferredQuests.Add(questText);
                    // Remove from disliked if it was there
                    preferences.DislikedQuests.Remove(questText);
                }
            }
            else if (rating <= 2)
            {
                // Add to disliked quests
                if (!preferences.DislikedQuests.Contains(questText))
                {
                    preferences.DislikedQuests.Add(questText);
                    // Remove from preferred if it was there
                    preferences.PreferredQuests.Remove(questText);
                }
            }

            // Keep lists from growing too large
            if (preferences.PreferredQuests.Count > 50)
            {
                preferences.PreferredQuests.RemoveAt(0);
            }

            if (preferences.DislikedQuests.Count > 50)
            {
                preferences.DislikedQuests.RemoveAt(0);
            }
        }

        /// <summary>
        /// Save preferences to persistent storage
        /// </summary>
        private static void SavePreferences(PlayerMobile player)
        {
            // This would integrate with the quest persistence system
            // For now, preferences are kept in memory
            // In a full implementation, this would serialize to database
        }

        /// <summary>
        /// Load preferences from persistent storage
        /// </summary>
        public static void LoadPreferences(PlayerMobile player)
        {
            // This would load from the quest persistence system
            // For now, preferences are created on-demand
        }

        /// <summary>
        /// Get preference statistics for analysis
        /// </summary>
        public static PreferenceStatistics GetPreferenceStatistics()
        {
            lock (s_Lock)
            {
                var stats = new PreferenceStatistics
                {
                    TotalPlayers = s_PlayerPreferences.Count,
                    AverageQuestsCompleted = s_PlayerPreferences.Values.Count > 0 ? 
                        s_PlayerPreferences.Values.Average(p => p.TotalQuestsCompleted) : 0,
                    AverageRating = s_PlayerPreferences.Values.Count > 0 ? 
                        s_PlayerPreferences.Values.Average(p => p.AverageQuestRating) : 0,
                    MostCommonThemes = GetMostCommonThemes(),
                    MostCommonLocations = GetMostCommonLocations(),
                    TotalDislikedQuests = s_PlayerPreferences.Values.Sum(p => p.DislikedQuests.Count),
                    TotalPreferredQuests = s_PlayerPreferences.Values.Sum(p => p.PreferredQuests.Count)
                };

                return stats;
            }
        }

        /// <summary>
        /// Get most common themes across all players
        /// </summary>
        private static Dictionary<string, int> GetMostCommonThemes()
        {
            var themeCounts = new Dictionary<string, int>();

            foreach (var preferences in s_PlayerPreferences.Values)
            {
                foreach (var theme in preferences.ThemePreferences)
                {
                    if (theme.Value > 0) // Only count positive preferences
                    {
                        themeCounts[theme.Key] = themeCounts.ContainsKey(theme.Key) ? themeCounts[theme.Key] + 1 : 1;
                    }
                }
            }

            return themeCounts.OrderByDescending(kvp => kvp.Value).Take(10).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
        }

        /// <summary>
        /// Get most common locations across all players
        /// </summary>
        private static Dictionary<string, int> GetMostCommonLocations()
        {
            var locationCounts = new Dictionary<string, int>();

            foreach (var preferences in s_PlayerPreferences.Values)
            {
                foreach (var location in preferences.LocationPreferences)
                {
                    if (location.Value > 0) // Only count positive preferences
                    {
                        locationCounts[location.Key] = locationCounts.ContainsKey(location.Key) ? locationCounts[location.Key] + 1 : 1;
                    }
                }
            }

            return locationCounts.OrderByDescending(kvp => kvp.Value).Take(10).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
        }
    }

    /// <summary>
    /// Preference statistics for analysis
    /// </summary>
    public class PreferenceStatistics
    {
        public int TotalPlayers { get; set; }
        public double AverageQuestsCompleted { get; set; }
        public double AverageRating { get; set; }
        public Dictionary<string, int> MostCommonThemes { get; set; }
        public Dictionary<string, int> MostCommonLocations { get; set; }
        public int TotalDislikedQuests { get; set; }
        public int TotalPreferredQuests { get; set; }
    }
}
