using System;
using System.Collections.Generic;
using Server;
using Server.Items;
using Server.Mobiles;
using Server.Targeting;
using Server.Gumps;

namespace Server.Custom.VystiaClasses.Religion
{
    /// <summary>
    /// Blessed Items System
    /// Allows players to bless items at shrines for religion-specific bonuses
    /// </summary>
    public static class VystiaBlessedItemSystem
    {
        /// <summary>
        /// Minimum piety required to bless items (Zealot tier = 500)
        /// </summary>
        public const int MinPietyForBlessing = 500;

        /// <summary>
        /// Gold cost as percentage of item base value
        /// </summary>
        public const double GoldCostPercent = 0.05; // 5%

        /// <summary>
        /// Success rates by piety tier (from design document)
        /// </summary>
        public const double SuccessRate500 = 0.50; // 50% at 500-599
        public const double SuccessRate600 = 0.60; // 60% at 600-699
        public const double SuccessRate700 = 0.70; // 70% at 700-799
        public const double SuccessRate800 = 0.80; // 80% at 800-899
        public const double SuccessRate900 = 0.90; // 90% at 900-1000

        /// <summary>
        /// Critical blessing chance by piety tier (from design document)
        /// </summary>
        public const double CriticalChance500 = 0.03; // 3% at 500-599
        public const double CriticalChance600 = 0.05; // 5% at 600-699
        public const double CriticalChance700 = 0.08; // 8% at 700-799
        public const double CriticalChance800 = 0.12; // 12% at 800-899
        public const double CriticalChance900 = 0.18; // 18% at 900-1000

        /// <summary>
        /// Check if an item can be blessed
        /// </summary>
        public static bool CanBlessItem(Item item)
        {
            if (item == null || item.Deleted)
                return false;

            // Only weapons, armor, and jewelry can be blessed
            return item is BaseWeapon || item is BaseArmor || item is BaseJewel;
        }

        /// <summary>
        /// Calculate blessing success rate based on piety (tiered ranges from design document)
        /// </summary>
        public static double CalculateSuccessRate(int piety)
        {
            if (piety < MinPietyForBlessing)
                return 0.0;

            // Tiered success rates from design document
            if (piety >= 900)
                return SuccessRate900; // 90% at 900-1000
            else if (piety >= 800)
                return SuccessRate800; // 80% at 800-899
            else if (piety >= 700)
                return SuccessRate700; // 70% at 700-799
            else if (piety >= 600)
                return SuccessRate600; // 60% at 600-699
            else
                return SuccessRate500; // 50% at 500-599
        }

        /// <summary>
        /// Calculate critical blessing chance based on piety (tiered ranges from design document)
        /// </summary>
        public static double CalculateCriticalChance(int piety)
        {
            if (piety < MinPietyForBlessing)
                return 0.0;

            // Tiered critical rates from design document
            if (piety >= 900)
                return CriticalChance900; // 18% at 900-1000
            else if (piety >= 800)
                return CriticalChance800; // 12% at 800-899
            else if (piety >= 700)
                return CriticalChance700; // 8% at 700-799
            else if (piety >= 600)
                return CriticalChance600; // 5% at 600-699
            else
                return CriticalChance500; // 3% at 500-599
        }

        /// <summary>
        /// Calculate gold cost for blessing an item
        /// </summary>
        public static int CalculateBlessingCost(Item item)
        {
            if (item == null)
                return 0;

            // Use item's base value (or estimate if not available)
            int baseValue = 0;
            if (item is BaseWeapon)
                baseValue = 1000; // Default weapon value
            else if (item is BaseArmor)
                baseValue = 500; // Default armor value
            else if (item is BaseJewel)
                baseValue = 300; // Default jewel value

            // Minimum cost of 100 gold
            return Math.Max(100, (int)(baseValue * GoldCostPercent));
        }

        /// <summary>
        /// Attempt to bless an item
        /// </summary>
        public static bool BlessItem(PlayerMobile pm, Item item, VystiaReligion religion)
        {
            if (pm == null || item == null || item.Deleted)
                return false;

            if (!CanBlessItem(item))
            {
                pm.SendMessage(0x22, "This item cannot be blessed.");
                return false;
            }

            var pietyData = VystiaPiety.GetPiety(pm);
            if (pietyData == null || pietyData.Religion != religion)
            {
                pm.SendMessage(0x22, "You must follow this religion to bless items.");
                return false;
            }

            if (pietyData.Piety < MinPietyForBlessing)
            {
                pm.SendMessage(0x22, "You need at least {0} piety (Zealot tier) to bless items.", MinPietyForBlessing);
                return false;
            }

            int cost = CalculateBlessingCost(item);
            if (Banker.GetBalance(pm) + GetBackpackGold(pm) < cost)
            {
                pm.SendMessage(0x22, "You need {0:N0} gold to bless this item.", cost);
                return false;
            }

            // Deduct gold
            if (!DeductGold(pm, cost))
            {
                pm.SendMessage(0x22, "Failed to deduct gold.");
                return false;
            }

            // Calculate success
            double successRate = CalculateSuccessRate(pietyData.Piety);
            bool success = Utility.RandomDouble() < successRate;

            if (!success)
            {
                pm.SendMessage(0x22, "The blessing fails! Your item remains unchanged.");
                pm.PlaySound(0x1F8); // Fail sound
                return false;
            }

            // Determine if critical blessing
            double criticalChance = CalculateCriticalChance(pietyData.Piety);
            bool isCritical = Utility.RandomDouble() < criticalChance;

            // Apply blessing
            ApplyBlessing(item, religion, isCritical ? BlessingType.Critical : BlessingType.Standard);

            string blessingType = isCritical ? "critical" : "standard";
            pm.SendMessage(0x35, "Your item has been blessed with a {0} blessing!", blessingType);
            pm.PlaySound(0x1F2); // Success sound
            pm.FixedParticles(0x376A, 9, 32, 5030, ReligionData.GetInfo(religion)?.Hue ?? 0, 0, EffectLayer.Waist);

            return true;
        }

        /// <summary>
        /// Apply blessing to an item
        /// </summary>
        private static void ApplyBlessing(Item item, VystiaReligion religion, BlessingType type)
        {
            // Use property system to store blessing data
            if (item is IBlessedItem blessedItem)
            {
                blessedItem.BlessingReligion = religion;
                blessedItem.BlessingType = type;
                blessedItem.BlessedDate = DateTime.UtcNow;
            }
            else
            {
                // Add blessed item property if item doesn't implement interface
                // For now, we'll use a property attachment system
                // This would need to be implemented based on ServUO's property system
                // For simplicity, we'll assume items can implement IBlessedItem
            }

            item.InvalidateProperties();
        }

        /// <summary>
        /// Get blessing effectiveness multiplier based on user's religion
        /// </summary>
        public static double GetBlessingEffectiveness(PlayerMobile user, VystiaReligion blessingReligion)
        {
            if (user == null)
                return 0.0;

            var userReligion = VystiaPiety.GetReligion(user);

            // Same religion: 100%
            if (userReligion == blessingReligion)
                return 1.0;

            // Faithless: 50%
            if (userReligion == VystiaReligion.None)
                return 0.5;

            // Opposed religion: Cannot equip (0%)
            if (VystiaReligionSystem.AreReligionsOpposed(userReligion, blessingReligion))
                return 0.0;

            // Different (non-opposed): 25%
            return 0.25;
        }

        /// <summary>
        /// Check if player can equip a blessed item
        /// </summary>
        public static bool CanEquipBlessedItem(PlayerMobile pm, Item item)
        {
            if (pm == null || item == null)
                return true; // Not a blessed item, allow

            if (!(item is IBlessedItem blessedItem) || blessedItem.BlessingReligion == VystiaReligion.None)
                return true; // Not blessed, allow

            var userReligion = VystiaPiety.GetReligion(pm);

            // Opposed religion: Cannot equip
            if (VystiaReligionSystem.AreReligionsOpposed(userReligion, blessedItem.BlessingReligion))
                return false;

            return true;
        }

        private static int GetBackpackGold(Mobile m)
        {
            if (m.Backpack == null)
                return 0;

            Item[] gold = m.Backpack.FindItemsByType(typeof(Gold));
            int total = 0;
            foreach (Item g in gold)
                total += g.Amount;
            return total;
        }

        private static bool DeductGold(Mobile m, int amount)
        {
            // Try backpack first
            if (m.Backpack != null)
            {
                Item[] gold = m.Backpack.FindItemsByType(typeof(Gold));
                int backpackGold = 0;
                foreach (Item g in gold)
                    backpackGold += g.Amount;

                if (backpackGold >= amount)
                {
                    m.Backpack.ConsumeTotal(typeof(Gold), amount);
                    return true;
                }
                else if (backpackGold > 0)
                {
                    m.Backpack.ConsumeTotal(typeof(Gold), backpackGold);
                    amount -= backpackGold;
                }
            }

            // Remainder from bank
            return Banker.Withdraw(m, amount);
        }

        #region Blessing Refresh

        private static Dictionary<PlayerMobile, DateTime> s_BlessingRefreshCooldowns = new Dictionary<PlayerMobile, DateTime>();

        /// <summary>
        /// Check if player can refresh blessings (8 hour cooldown, Adherent tier required)
        /// </summary>
        public static bool CanRefreshBlessings(PlayerMobile pm)
        {
            if (pm == null)
                return false;

            var pietyData = VystiaPiety.GetPiety(pm);
            if (pietyData == null || pietyData.Piety < 50) // Adherent tier
                return false;

            if (!s_BlessingRefreshCooldowns.ContainsKey(pm))
                return true;

            return s_BlessingRefreshCooldowns[pm] <= DateTime.UtcNow;
        }

        /// <summary>
        /// Get remaining cooldown for blessing refresh
        /// </summary>
        public static TimeSpan GetBlessingRefreshCooldown(PlayerMobile pm)
        {
            if (pm == null || !s_BlessingRefreshCooldowns.ContainsKey(pm))
                return TimeSpan.Zero;

            DateTime cooldownEnd = s_BlessingRefreshCooldowns[pm];
            if (cooldownEnd <= DateTime.UtcNow)
                return TimeSpan.Zero;

            return cooldownEnd - DateTime.UtcNow;
        }

        /// <summary>
        /// Refresh all blessed items (restore effectiveness, 8 hour cooldown)
        /// </summary>
        public static bool RefreshBlessings(PlayerMobile pm)
        {
            if (pm == null)
                return false;

            if (!CanRefreshBlessings(pm))
            {
                TimeSpan remaining = GetBlessingRefreshCooldown(pm);
                pm.SendMessage(0x22, "You must wait {0:F1} more hours before refreshing blessings again.", remaining.TotalHours);
                return false;
            }

            // Refresh all blessed items in inventory
            int refreshedCount = 0;
            foreach (Item item in pm.Items)
            {
                if (item is IBlessedItem blessedItem && blessedItem.BlessingReligion != VystiaReligion.None)
                {
                    // Refresh by updating the blessed date (effectiveness is calculated from this)
                    blessedItem.BlessedDate = DateTime.UtcNow;
                    refreshedCount++;
                }
            }

            if (pm.Backpack != null)
            {
                foreach (Item item in pm.Backpack.Items)
                {
                    if (item is IBlessedItem blessedItem && blessedItem.BlessingReligion != VystiaReligion.None)
                    {
                        blessedItem.BlessedDate = DateTime.UtcNow;
                        refreshedCount++;
                    }
                }
            }

            // Set 8 hour cooldown
            s_BlessingRefreshCooldowns[pm] = DateTime.UtcNow + TimeSpan.FromHours(8);

            pm.SendMessage(0x35, "You have refreshed {0} blessed item{1}!", refreshedCount, refreshedCount == 1 ? "" : "s");
            pm.PlaySound(0x1F2);
            pm.FixedParticles(0x376A, 9, 32, 5030, ReligionData.GetInfo(VystiaPiety.GetReligion(pm))?.Hue ?? 0, 0, EffectLayer.Waist);

            return true;
        }

        #endregion
    }
}
