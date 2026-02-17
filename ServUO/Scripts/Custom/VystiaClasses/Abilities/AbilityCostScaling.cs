/*
 * Vystia Class System v2.0
 * Ability Cost Scaling by Circle
 *
 * This file defines mana costs, cooldowns, and cast times
 * that scale with ability circle (1-8).
 */

using System;

namespace Server.Custom.VystiaClasses.Abilities
{
    /// <summary>
    /// Provides scaling values for ability costs based on circle
    /// </summary>
    public static class AbilityCostScaling
    {
        #region Mana Costs

        // Mana cost ranges by circle (min, max)
        private static readonly (int Min, int Max)[] ManaCosts = new[]
        {
            (4, 6),     // Circle 1
            (8, 10),    // Circle 2
            (12, 15),   // Circle 3
            (18, 22),   // Circle 4
            (25, 30),   // Circle 5
            (35, 42),   // Circle 6
            (48, 55),   // Circle 7
            (60, 75)    // Circle 8
        };

        /// <summary>
        /// Get the base mana cost for a circle (uses midpoint)
        /// </summary>
        public static int GetManaCost(int circle)
        {
            if (circle < 1) circle = 1;
            if (circle > 8) circle = 8;

            var range = ManaCosts[circle - 1];
            return (range.Min + range.Max) / 2;
        }

        /// <summary>
        /// Get the minimum mana cost for a circle
        /// </summary>
        public static int GetMinManaCost(int circle)
        {
            if (circle < 1) circle = 1;
            if (circle > 8) circle = 8;
            return ManaCosts[circle - 1].Min;
        }

        /// <summary>
        /// Get the maximum mana cost for a circle
        /// </summary>
        public static int GetMaxManaCost(int circle)
        {
            if (circle < 1) circle = 1;
            if (circle > 8) circle = 8;
            return ManaCosts[circle - 1].Max;
        }

        #endregion

        #region Stamina Costs (for martial abilities)

        // Stamina cost ranges by tier (for martial abilities, 1-4 tiers)
        private static readonly (int Min, int Max)[] StaminaCosts = new[]
        {
            (8, 12),    // Tier 1 - Basic strikes
            (15, 20),   // Tier 2 - Advanced techniques
            (25, 35),   // Tier 3 - Powerful attacks
            (40, 55)    // Tier 4 - Ultimate abilities
        };

        /// <summary>
        /// Get the base stamina cost for a martial tier
        /// </summary>
        public static int GetStaminaCost(int tier)
        {
            if (tier < 1) tier = 1;
            if (tier > 4) tier = 4;

            var range = StaminaCosts[tier - 1];
            return (range.Min + range.Max) / 2;
        }

        #endregion

        #region Cooldowns

        // Cooldown ranges by circle (min, max) in seconds
        private static readonly (double Min, double Max)[] Cooldowns = new[]
        {
            (0.0, 1.0),     // Circle 1 - Spammable
            (1.0, 2.0),     // Circle 2
            (2.0, 4.0),     // Circle 3
            (4.0, 6.0),     // Circle 4
            (6.0, 10.0),    // Circle 5
            (10.0, 15.0),   // Circle 6
            (15.0, 25.0),   // Circle 7
            (30.0, 60.0)    // Circle 8 - Ultimate
        };

        /// <summary>
        /// Get the base cooldown for a circle (uses midpoint)
        /// </summary>
        public static TimeSpan GetCooldown(int circle)
        {
            if (circle < 1) circle = 1;
            if (circle > 8) circle = 8;

            var range = Cooldowns[circle - 1];
            return TimeSpan.FromSeconds((range.Min + range.Max) / 2);
        }

        /// <summary>
        /// Get the minimum cooldown for a circle
        /// </summary>
        public static TimeSpan GetMinCooldown(int circle)
        {
            if (circle < 1) circle = 1;
            if (circle > 8) circle = 8;
            return TimeSpan.FromSeconds(Cooldowns[circle - 1].Min);
        }

        /// <summary>
        /// Get the maximum cooldown for a circle
        /// </summary>
        public static TimeSpan GetMaxCooldown(int circle)
        {
            if (circle < 1) circle = 1;
            if (circle > 8) circle = 8;
            return TimeSpan.FromSeconds(Cooldowns[circle - 1].Max);
        }

        #endregion

        #region Cast Times

        // Cast times by circle in seconds
        private static readonly double[] CastTimes = new[]
        {
            0.0,    // Circle 1 - Instant
            0.5,    // Circle 2
            1.0,    // Circle 3
            1.5,    // Circle 4
            2.0,    // Circle 5
            2.25,   // Circle 6
            2.5,    // Circle 7
            3.0     // Circle 8
        };

        /// <summary>
        /// Get the cast time for a circle
        /// </summary>
        public static TimeSpan GetCastTime(int circle)
        {
            if (circle < 1) circle = 1;
            if (circle > 8) circle = 8;
            return TimeSpan.FromSeconds(CastTimes[circle - 1]);
        }

        /// <summary>
        /// Check if an ability at this circle is instant cast
        /// </summary>
        public static bool IsInstant(int circle)
        {
            return circle <= 1;
        }

        #endregion

        #region Damage/Heal Scaling

        // Base damage ranges by circle (min, max)
        // These are the base values before skill scaling
        private static readonly (int Min, int Max)[] BaseDamage = new[]
        {
            (4, 10),      // Circle 1
            (8, 16),      // Circle 2
            (15, 30),     // Circle 3
            (25, 45),     // Circle 4
            (40, 65),     // Circle 5
            (55, 90),     // Circle 6
            (75, 120),    // Circle 7
            (100, 160)    // Circle 8
        };

        /// <summary>
        /// Get the base damage range for a circle
        /// </summary>
        public static (int Min, int Max) GetBaseDamage(int circle)
        {
            if (circle < 1) circle = 1;
            if (circle > 8) circle = 8;
            return BaseDamage[circle - 1];
        }

        /// <summary>
        /// Get the base heal range for a circle (slightly higher than damage)
        /// </summary>
        public static (int Min, int Max) GetBaseHeal(int circle)
        {
            var dmg = GetBaseDamage(circle);
            // Heals are 20% higher than damage
            return ((int)(dmg.Min * 1.2), (int)(dmg.Max * 1.2));
        }

        #endregion

        #region Utility Methods

        /// <summary>
        /// Apply all standard scaling to an ability definition
        /// </summary>
        public static AbilityDefinition ApplyCircleScaling(this AbilityDefinition ability)
        {
            int circle = ability.Circle;

            // Only apply if not already set
            if (ability.ManaCost == 0)
                ability.ManaCost = GetManaCost(circle);

            if (ability.Cooldown == TimeSpan.Zero)
                ability.Cooldown = GetCooldown(circle);

            if (ability.CastTime == TimeSpan.Zero && !ability.IsInstant)
            {
                var castTime = GetCastTime(circle);
                if (castTime > TimeSpan.Zero)
                    ability.CastTime = castTime;
                else
                    ability.IsInstant = true;
            }

            return ability;
        }

        /// <summary>
        /// Get a summary string of the costs for a circle
        /// </summary>
        public static string GetCostSummary(int circle)
        {
            return $"Circle {circle}: Mana {GetManaCost(circle)}, Cooldown {GetCooldown(circle).TotalSeconds:F1}s, Cast {GetCastTime(circle).TotalSeconds:F1}s";
        }

        #endregion
    }
}
