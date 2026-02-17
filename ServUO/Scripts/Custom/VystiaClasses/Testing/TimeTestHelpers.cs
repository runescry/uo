/*
 * Vystia Time Test Helpers
 *
 * Utilities for testing time-based systems (cooldowns, durations, decay)
 * Note: Cannot manipulate DateTime.UtcNow directly, so we use stored timestamps
 */

using System;
using Server.Mobiles;
using Server.Custom.VystiaClasses.Religion;

namespace Server.Custom.VystiaClasses.Testing
{
    /// <summary>
    /// Helper methods for testing time-based systems
    /// </summary>
    public static class TimeTestHelpers
    {
        /// <summary>
        /// Check if a player can pray at a specific time
        /// </summary>
        public static bool CanPrayAtTime(PlayerMobile pm, DateTime testTime)
        {
            if (pm == null)
                return false;

            var pietyData = VystiaPiety.GetPiety(pm);
            if (pietyData == null)
                return false;

            // Check if last prayer was more than 24 hours ago
            if (pietyData.LastPrayer == DateTime.MinValue)
                return true;

            TimeSpan timeSincePrayer = testTime - pietyData.LastPrayer;
            return timeSincePrayer >= TimeSpan.FromHours(24);
        }

        /// <summary>
        /// Check if a player can tithe at a specific time
        /// </summary>
        public static bool CanTitheAtTime(PlayerMobile pm, DateTime testTime)
        {
            if (pm == null)
                return false;

            var pietyData = VystiaPiety.GetPiety(pm);
            if (pietyData == null)
                return false;

            // Check if last tithe was more than 24 hours ago
            if (pietyData.LastTithe == DateTime.MinValue)
                return true;

            TimeSpan timeSinceTithe = testTime - pietyData.LastTithe;
            return timeSinceTithe >= TimeSpan.FromHours(24);
        }

        /// <summary>
        /// Check if a player can perform pilgrimage at a specific time
        /// </summary>
        public static bool CanPilgrimageAtTime(PlayerMobile pm, DateTime testTime)
        {
            if (pm == null)
                return false;

            var pietyData = VystiaPiety.GetPiety(pm);
            if (pietyData == null)
                return false;

            // Check if last pilgrimage was more than 7 days ago
            if (pietyData.LastPilgrimage == DateTime.MinValue)
                return true;

            TimeSpan timeSincePilgrimage = testTime - pietyData.LastPilgrimage;
            return timeSincePilgrimage >= TimeSpan.FromDays(7);
        }

        /// <summary>
        /// Set a player's last prayer time to a specific time
        /// </summary>
        public static void SetLastPrayerTime(PlayerMobile pm, DateTime time)
        {
            if (pm == null)
                return;

            var pietyData = VystiaPiety.GetPiety(pm);
            if (pietyData != null)
            {
                pietyData.LastPrayer = time;
            }
        }

        /// <summary>
        /// Set a player's last tithe time to a specific time
        /// </summary>
        public static void SetLastTitheTime(PlayerMobile pm, DateTime time)
        {
            if (pm == null)
                return;

            var pietyData = VystiaPiety.GetPiety(pm);
            if (pietyData != null)
            {
                pietyData.LastTithe = time;
            }
        }

        /// <summary>
        /// Set a player's last pilgrimage time to a specific time
        /// </summary>
        public static void SetLastPilgrimageTime(PlayerMobile pm, DateTime time)
        {
            if (pm == null)
                return;

            var pietyData = VystiaPiety.GetPiety(pm);
            if (pietyData != null)
            {
                pietyData.LastPilgrimage = time;
            }
        }

        /// <summary>
        /// Check if enough time has passed for a cooldown
        /// </summary>
        public static bool IsCooldownExpired(DateTime lastUse, TimeSpan cooldown, DateTime testTime)
        {
            if (lastUse == DateTime.MinValue)
                return true;

            TimeSpan timeSinceUse = testTime - lastUse;
            return timeSinceUse >= cooldown;
        }

        /// <summary>
        /// Get the time remaining on a cooldown
        /// </summary>
        public static TimeSpan GetCooldownRemaining(DateTime lastUse, TimeSpan cooldown, DateTime testTime)
        {
            if (lastUse == DateTime.MinValue)
                return TimeSpan.Zero;

            TimeSpan timeSinceUse = testTime - lastUse;
            TimeSpan remaining = cooldown - timeSinceUse;
            
            return remaining > TimeSpan.Zero ? remaining : TimeSpan.Zero;
        }
    }
}
