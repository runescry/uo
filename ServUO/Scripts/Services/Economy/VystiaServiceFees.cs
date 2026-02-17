using System;
using Server;
using Server.Items;
using Server.Mobiles;

namespace Server.Services.Economy
{
    /// <summary>
    /// Vystia Service Fees - Gold Sinks for NPC Services
    ///
    /// Resurrection Fees:
    /// - Healer resurrection: 50g (design doc value)
    /// - Young players: Free (tutorial protection)
    ///
    /// Travel Fees (Moongates):
    /// - Same region: 100g
    /// - Cross-region: 150g
    /// - Cross-facet: 250g
    ///
    /// Other Services:
    /// - Pet stabling: 30g/day (100g/week)
    /// - Recall rune charging: 25g per use added
    /// </summary>
    public static class VystiaServiceFees
    {
        #region Configuration

        /// <summary>
        /// Enable Vystia service fees (can be disabled for testing)
        /// </summary>
        public static bool Enabled = true;

        #endregion

        #region Resurrection Fees

        /// <summary>
        /// Base resurrection fee for healer NPC
        /// </summary>
        public const int HealerResurrectionFee = 50;

        /// <summary>
        /// Get the resurrection fee for a player
        /// </summary>
        public static int GetResurrectionFee(Mobile m)
        {
            if (!Enabled)
                return 0;

            // Young players resurrect for free (tutorial protection)
            if (m is PlayerMobile pm && pm.Young)
                return 0;

            return HealerResurrectionFee;
        }

        /// <summary>
        /// Check if player can afford resurrection
        /// </summary>
        public static bool CanAffordResurrection(Mobile m)
        {
            int fee = GetResurrectionFee(m);
            if (fee == 0)
                return true;

            return Banker.GetBalance(m) >= fee;
        }

        #endregion

        #region Travel Fees

        /// <summary>
        /// Public moongate travel fee (same region)
        /// </summary>
        public const int MoongateLocalFee = 100;

        /// <summary>
        /// Cross-region moongate fee
        /// </summary>
        public const int MoongateCrossRegionFee = 150;

        /// <summary>
        /// Cross-facet moongate fee
        /// </summary>
        public const int MoongateCrossFacetFee = 250;

        /// <summary>
        /// Calculate moongate travel fee based on destination
        /// </summary>
        public static int GetMoongateFee(Mobile from, Point3D destination, Map destMap)
        {
            if (!Enabled)
                return 0;

            // Young players travel free
            if (from is PlayerMobile pm && pm.Young)
                return 0;

            // Cross-facet travel
            if (from.Map != destMap)
                return MoongateCrossFacetFee;

            // Calculate distance for cross-region determination
            // Regions over 1000 tiles apart are considered cross-region
            double distance = from.GetDistanceToSqrt(destination);
            if (distance > 1000)
                return MoongateCrossRegionFee;

            return MoongateLocalFee;
        }

        #endregion

        #region Pet Stabling Fees

        /// <summary>
        /// Pet stabling fee per day
        /// </summary>
        public const int PetStablingPerDay = 30;

        /// <summary>
        /// Pet stabling fee per week
        /// </summary>
        public const int PetStablingPerWeek = 100;

        /// <summary>
        /// Calculate stabling fee for a pet
        /// </summary>
        public static int GetStablingFee(int days)
        {
            if (!Enabled)
                return 0;

            if (days >= 7)
                return (days / 7) * PetStablingPerWeek + (days % 7) * PetStablingPerDay;

            return days * PetStablingPerDay;
        }

        #endregion

        #region Recall/Mark Fees

        /// <summary>
        /// Cost to add one charge to a rune via NPC service
        /// </summary>
        public const int RuneChargeFee = 25;

        #endregion

        #region Utility Methods

        /// <summary>
        /// Attempt to charge a service fee
        /// Returns true if successful, false if insufficient funds
        /// </summary>
        public static bool ChargeServiceFee(Mobile m, int amount, string serviceName)
        {
            if (amount <= 0)
                return true;

            // Try backpack gold first
            int backpackGold = GetBackpackGold(m);

            if (backpackGold >= amount)
            {
                m.Backpack.ConsumeTotal(typeof(Gold), amount);
                m.SendMessage(0x35, "You paid {0:N0} gold for {1}.", amount, serviceName);
                return true;
            }

            // Try bank
            if (Banker.Withdraw(m, amount))
            {
                m.SendMessage(0x35, "{0:N0} gold was withdrawn from your bank for {1}.", amount, serviceName);
                return true;
            }

            m.SendMessage(0x22, "You don't have enough gold. {0} costs {1:N0} gold.", serviceName, amount);
            return false;
        }

        private static int GetBackpackGold(Mobile m)
        {
            if (m?.Backpack == null)
                return 0;

            Item[] gold = m.Backpack.FindItemsByType(typeof(Gold));
            int total = 0;
            foreach (Item g in gold)
                total += g.Amount;
            return total;
        }

        #endregion
    }
}
