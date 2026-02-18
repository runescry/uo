using System;
using System.Collections.Generic;
using System.Linq;
using Server.Mobiles;
using Server.Custom.VystiaClasses.Quests;
using Server.Custom.VystiaClasses.Quests.Generation;

namespace Server.Services.QuestVariety
{
    /// <summary>
    /// Manages quest theme rotation to maintain variety
    /// </summary>
    public static class QuestThemeRotation
    {
        private static readonly Dictionary<string, QuestTheme> s_Themes = new Dictionary<string, QuestTheme>();
        private static readonly Dictionary<string, DateTime> s_ThemeLastUsed = new Dictionary<string, DateTime>();
        private static readonly Dictionary<string, int> s_ThemeUsageCount = new Dictionary<string, int>();
        private static readonly Random s_Random = new Random();

        /// <summary>
        /// Initialize the quest theme system
        /// </summary>
        public static void Initialize()
        {
            // Initialize default themes
            InitializeDefaultThemes();
            
            Console.WriteLine("[QuestThemeRotation] Initialized with {s_Themes.Count} themes");
        }

        /// <summary>
        /// Select a theme for quest generation
        /// </summary>
        public static QuestTheme SelectTheme(PlayerMobile player = null, List<string> excludedThemes = null)
        {
            var availableThemes = s_Themes.Values
                .Where(t => t.IsActive)
                .ToList();

            // Apply exclusions
            if (excludedThemes != null)
            {
                availableThemes = availableThemes.Where(t => !excludedThemes.Contains(t.ThemeId)).ToList();
            }

            // Apply cooldowns
            var now = DateTime.UtcNow;
            availableThemes = availableThemes.Where(t => 
                !s_ThemeLastUsed.ContainsKey(t.ThemeId) || 
                (now - s_ThemeLastUsed[t.ThemeId]) >= TimeSpan.FromHours(t.CooldownPeriod)
            ).ToList();

            // Apply player preferences if available
            if (player != null)
            {
                var preferences = PlayerPreferenceLearning.GetPlayerPreferences(player);
                if (preferences != null)
                {
                    availableThemes = ApplyPlayerPreferences(availableThemes, preferences);
                }
            }

            if (!availableThemes.Any())
            {
                // If no themes available (all on cooldown), reset cooldowns
                ResetCooldowns();
                availableThemes = s_Themes.Values.Where(t => t.IsActive).ToList();
            }

            // Weighted selection
            var selectedTheme = WeightedThemeSelection(availableThemes);
            
            // Update usage tracking
            UpdateThemeUsage(selectedTheme);
            
            return selectedTheme;
        }

        /// <summary>
        /// Get theme cooldown information
        /// </summary>
        public static TimeSpan GetThemeCooldown(string themeId)
        {
            if (!s_ThemeLastUsed.ContainsKey(themeId))
                return TimeSpan.Zero;

            var lastUsed = s_ThemeLastUsed[themeId];
            var cooldownPeriod = TimeSpan.FromHours(s_Themes[themeId].CooldownPeriod);
            var timeSinceUse = DateTime.UtcNow - lastUsed;
            
            return cooldownPeriod - timeSinceUse;
        }

        /// <summary>
        /// Check if a theme is available for use
        /// </summary>
        public static bool IsThemeAvailable(string themeId)
        {
            if (!s_Themes.ContainsKey(themeId) || !s_Themes[themeId].IsActive)
                return false;

            var cooldown = GetThemeCooldown(themeId);
            return cooldown <= TimeSpan.Zero;
        }

        /// <summary>
        /// Get theme usage statistics
        /// </summary>
        public static Dictionary<string, ThemeUsageStats> GetThemeStats()
        {
            var stats = new Dictionary<string, ThemeUsageStats>();
            
            foreach (var theme in s_Themes.Values)
            {
                stats[theme.ThemeId] = new ThemeUsageStats
                {
                    ThemeId = theme.ThemeId,
                    Name = theme.Name,
                    UsageCount = s_ThemeUsageCount.GetValueOrDefault(theme.ThemeId, 0),
                    LastUsed = s_ThemeLastUsed.GetValueOrDefault(theme.ThemeId, DateTime.MinValue),
                    CooldownRemaining = GetThemeCooldown(theme.ThemeId),
                    Weight = theme.Weight,
                    IsActive = theme.IsActive
                };
            }

            return stats;
        }

        /// <summary>
        /// Add or update a theme
        /// </summary>
        public static void AddOrUpdateTheme(QuestTheme theme)
        {
            s_Themes[theme.ThemeId] = theme;
            
            if (!s_ThemeUsageCount.ContainsKey(theme.ThemeId))
            {
                s_ThemeUsageCount[theme.ThemeId] = 0;
            }
            
            Console.WriteLine($"[QuestThemeRotation] Updated theme: {theme.Name}");
        }

        /// <summary>
        /// Deactivate a theme
        /// </summary>
        public static void DeactivateTheme(string themeId)
        {
            if (s_Themes.ContainsKey(themeId))
            {
                s_Themes[themeId].IsActive = false;
                Console.WriteLine($"[QuestThemeRotation] Deactivated theme: {s_Themes[themeId].Name}");
            }
        }

        /// <summary>
        /// Get theme suggestions based on recent usage
        /// </summary>
        public static List<QuestTheme> GetThemeSuggestions(int maxSuggestions = 5)
        {
            var now = DateTime.UtcNow;
            var recentUsage = s_ThemeLastUsed
                .OrderBy(kvp => kvp.Value)
                .Take(10)
                .ToList();

            var suggestions = new List<QuestTheme>();
            
            // Suggest themes that haven't been used recently
            foreach (var theme in s_Themes.Values.Where(t => t.IsActive))
            {
                if (!recentUsage.Any(kvp => kvp.Key == theme.ThemeId))
                {
                    suggestions.Add(theme);
                }
            }

            return suggestions.Take(maxSuggestions).ToList();
        }

        /// <summary>
        /// Analyze theme distribution and provide recommendations
        /// </summary>
        public static ThemeDistributionAnalysis AnalyzeThemeDistribution()
        {
            var totalUsage = s_ThemeUsageCount.Values.Sum();
            var analysis = new ThemeDistributionAnalysis
            {
                TotalThemes = s_Themes.Count,
                ActiveThemes = s_Themes.Values.Count(t => t.IsActive),
                TotalUsage = totalUsage,
                ThemeDistribution = new Dictionary<string, double>(),
                Recommendations = new List<string>(),
                AnalyzedAt = DateTime.UtcNow
            };

            if (totalUsage > 0)
            {
                // Calculate distribution percentages
                foreach (var theme in s_Themes.Values)
                {
                    double percentage = (double)s_ThemeUsageCount.GetValueOrDefault(theme.ThemeId, 0) / totalUsage * 100;
                    analysis.ThemeDistribution[theme.ThemeId] = percentage;
                }

                // Generate recommendations
                var overusedThemes = analysis.ThemeDistribution.Where(kvp => kvp.Value > 25).ToList();
                var underusedThemes = analysis.ThemeDistribution.Where(kvp => kvp.Value < 5).ToList();

                if (overusedThemes.Any())
                {
                    analysis.Recommendations.Add($"Consider reducing usage of overused themes: {string.Join(", ", overusedThemes.Select(kvp => kvp.Key))}");
                }

                if (underusedThemes.Any())
                {
                    analysis.Recommendations.Add($"Consider promoting underused themes: {string.Join(", ", underusedThemes.Select(kvp => kvp.Key))}");
                }

                // Check for theme balance
                var maxUsage = analysis.ThemeDistribution.Values.Max();
                var minUsage = analysis.ThemeDistribution.Values.Min();
                
                if (maxUsage > minUsage * 5)
                {
                    analysis.Recommendations.Add("Theme usage is unbalanced - consider adjusting weights or cooldowns");
                }
            }

            return analysis;
        }

        /// <summary>
        /// Initialize default quest themes
        /// </summary>
        private static void InitializeDefaultThemes()
        {
            var themes = new[]
            {
                new QuestTheme
                {
                    ThemeId = "combat_basic",
                    Name = "Basic Combat",
                    Description = "Simple combat quests with enemies and battles",
                    Keywords = new List<string> { "kill", "defeat", "battle", "fight", "enemy", "monster" },
                    Objectives = new List<string> { "defeat enemies", "clear area", "win battle" },
                    Locations = new List<string> { "forest", "dungeon", "cave", "plains" },
                    Rewards = new List<string> { "experience", "gold", "equipment" },
                    Weight = 1.0,
                    CooldownPeriod = 2,
                    IsActive = true
                },
                new QuestTheme
                {
                    ThemeId = "exploration_mystery",
                    Name = "Exploration & Mystery",
                    Description = "Quests involving discovery and investigation",
                    Keywords = new List<string> { "explore", "discover", "find", "search", "investigate", "mystery" },
                    Objectives = new List<string> { "explore area", "find location", "solve mystery", "discover secret" },
                    Locations = new List<string> { "ruins", "cave", "forest", "mountain", "island" },
                    Rewards = new List<string> { "knowledge", "artifacts", "reputation" },
                    Weight = 1.2,
                    CooldownPeriod = 4,
                    IsActive = true
                },
                new QuestTheme
                {
                    ThemeId = "collection_gathering",
                    Name = "Collection & Gathering",
                    Description = "Quests focused on collecting items and resources",
                    Keywords = new List<string> { "collect", "gather", "find", "acquire", "obtain", "retrieve" },
                    Objectives = new List<string> { "collect items", "gather resources", "find artifacts" },
                    Locations = new List<string> { "forest", "fields", "mountains", "rivers" },
                    Rewards = new List<string> { "items", "resources", "crafting materials" },
                    Weight = 0.8,
                    CooldownPeriod = 1,
                    IsActive = true
                },
                new QuestTheme
                {
                    ThemeId = "delivery_transport",
                    Name = "Delivery & Transport",
                    Description = "Quests involving delivery and transportation",
                    Keywords = new List<string> { "deliver", "bring", "carry", "transport", "return", "take" },
                    Objectives = new List<string> { "deliver package", "transport item", "return message" },
                    Locations = new List<string> { "city", "town", "village", "castle", "outpost" },
                    Rewards = new List<string> { "gold", "reputation", "favors" },
                    Weight = 0.9,
                    CooldownPeriod = 3,
                    IsActive = true
                },
                new QuestTheme
                {
                    ThemeId = "protection_guard",
                    Name = "Protection & Guard",
                    Description = "Quests involving protection and guarding duties",
                    Keywords = new List<string> { "protect", "guard", "defend", "shield", "watch", "safeguard" },
                    Objectives = new List<string> { "protect npc", "guard location", "defend area" },
                    Locations = new List<string> { "village", "castle", "caravan", "merchant" },
                    Rewards = new List<string> { "reputation", "honor", "protection payment" },
                    Weight = 1.1,
                    CooldownPeriod = 3,
                    IsActive = true
                },
                new QuestTheme
                {
                    ThemeId = "rescue_aid",
                    Name = "Rescue & Aid",
                    Description = "Quests involving rescue operations and helping others",
                    Keywords = new List<string> { "rescue", "save", "help", "assist", "free", "liberate" },
                    Objectives = new List<string> { "rescue person", "save village", "help npc" },
                    Locations = new List<string> { "dungeon", "cave", "prison", "dangerous area" },
                    Rewards = new List<string> { "gratitude", "rewards", "allies" },
                    Weight = 1.3,
                    CooldownPeriod = 5,
                    IsActive = true
                },
                new QuestTheme
                {
                    ThemeId = "political_intrigue",
                    Name = "Political Intrigue",
                    Description = "Complex quests involving politics and intrigue",
                    Keywords = new List<string> { "politics", "intrigue", "conspiracy", "spy", "diplomacy", "negotiation" },
                    Objectives = new List<string> { "investigate conspiracy", "deliver secret message", "negotiate treaty" },
                    Locations = new List<string> { "court", "castle", "embassy", "secret meeting" },
                    Rewards = new List<string> { "political favor", "information", "influence" },
                    Weight = 1.5,
                    CooldownPeriod = 8,
                    IsActive = true
                },
                new QuestTheme
                {
                    ThemeId = "magical_mystical",
                    Name = "Magical & Mystical",
                    Description = "Quests involving magic and supernatural elements",
                    Keywords = new List<string> { "magic", "spell", "potion", "artifact", "supernatural", "mystical" },
                    Objectives = new List<string> { "find magical item", "cast spell", "investigate magical phenomenon" },
                    Locations = new List<string> { "tower", "ruins", "magical forest", "otherworldly portal" },
                    Rewards = new List<string> { "magical items", "spells", "enchanted equipment" },
                    Weight = 1.4,
                    CooldownPeriod = 6,
                    IsActive = true
                }
            };

            foreach (var theme in themes)
            {
                s_Themes[theme.ThemeId] = theme;
                s_ThemeUsageCount[theme.ThemeId] = 0;
            }
        }

        /// <summary>
        /// Apply player preferences to theme selection
        /// </summary>
        private static List<QuestTheme> ApplyPlayerPreferences(List<QuestTheme> themes, PlayerPreferences preferences)
        {
            var weightedThemes = new List<QuestTheme>();

            foreach (var theme in themes)
            {
                double adjustedWeight = theme.Weight;

                // Apply theme preferences
                if (preferences.ThemePreferences.TryGetValue(theme.ThemeId, out double prefWeight))
                {
                    adjustedWeight *= (1.0 + prefWeight);
                }

                // Apply location preferences
                foreach (var location in theme.Locations)
                {
                    if (preferences.LocationPreferences.TryGetValue(location, out double locPref))
                    {
                        adjustedWeight *= (1.0 + locPref * 0.5);
                    }
                }

                // Apply quest type preferences
                foreach (var questType in theme.Objectives)
                {
                    if (preferences.QuestTypePreferences.TryGetValue(questType, out double typePref))
                    {
                        adjustedWeight *= (1.0 + typePref * 0.3);
                    }
                }

                // Apply disliked quests
                if (preferences.DislikedQuests.Any(disliked => theme.Keywords.Any(keyword => disliked.Contains(keyword))))
                {
                    adjustedWeight *= 0.1; // Heavily penalize disliked themes
                }

                // Apply preferred quests
                if (preferences.PreferredQuests.Any(preferred => theme.Keywords.Any(keyword => preferred.Contains(keyword))))
                {
                    adjustedWeight *= 1.5; // Boost preferred themes
                }

                weightedThemes.Add(new QuestTheme
                {
                    ThemeId = theme.ThemeId,
                    Name = theme.Name,
                    Description = theme.Description,
                    Keywords = theme.Keywords,
                    Objectives = theme.Objectives,
                    Locations = theme.Locations,
                    Rewards = theme.Rewards,
                    Weight = adjustedWeight,
                    CooldownPeriod = theme.CooldownPeriod,
                    LastUsed = theme.LastUsed,
                    UsageCount = theme.UsageCount,
                    IsActive = theme.IsActive
                });
            }

            return weightedThemes;
        }

        /// <summary>
        /// Weighted theme selection
        /// </summary>
        private static QuestTheme WeightedThemeSelection(List<QuestTheme> themes)
        {
            if (!themes.Any())
                return null;

            var totalWeight = themes.Sum(t => t.Weight);
            var randomValue = s_Random.NextDouble() * totalWeight;

            double currentWeight = 0;
            foreach (var theme in themes)
            {
                currentWeight += theme.Weight;
                if (randomValue <= currentWeight)
                {
                    return theme;
                }
            }

            return themes.Last(); // Fallback
        }

        /// <summary>
        /// Update theme usage tracking
        /// </summary>
        private static void UpdateThemeUsage(QuestTheme theme)
        {
            s_ThemeLastUsed[theme.ThemeId] = DateTime.UtcNow;
            s_ThemeUsageCount[theme.ThemeId] = s_ThemeUsageCount.GetValueOrDefault(theme.ThemeId, 0) + 1;
            
            // Update theme object
            theme.LastUsed = DateTime.UtcNow;
            theme.UsageCount = s_ThemeUsageCount[theme.ThemeId];
            s_Themes[theme.ThemeId] = theme;
        }

        /// <summary>
        /// Reset all theme cooldowns
        /// </summary>
        private static void ResetCooldowns()
        {
            s_ThemeLastUsed.Clear();
            Console.WriteLine("[QuestThemeRotation] Reset all theme cooldowns due to no available themes");
        }
    }

    /// <summary>
    /// Theme distribution analysis
    /// </summary>
    public class ThemeDistributionAnalysis
    {
        public int TotalThemes { get; set; }
        public int ActiveThemes { get; set; }
        public int TotalUsage { get; set; }
        public Dictionary<string, double> ThemeDistribution { get; set; }
        public List<string> Recommendations { get; set; }
        public DateTime AnalyzedAt { get; set; }
    }
}
