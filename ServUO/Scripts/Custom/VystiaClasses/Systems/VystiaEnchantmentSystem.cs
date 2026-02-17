/*
 * Vystia Class System v2.0
 * Enchantment System
 *
 * Handles item enchantments based on Runeweaving skill.
 * Allows Enchanters to apply temporary or permanent bonuses to items.
 */

using System;
using System.Collections.Generic;
using Server;
using Server.Items;
using Server.Mobiles;

namespace Server.Custom.VystiaClasses.Systems
{
    /// <summary>
    /// Manages item enchantments for Runeweaving skill
    /// </summary>
    public static class VystiaEnchantmentSystem
    {
        /// <summary>
        /// Get maximum number of enchantments per item based on Runeweaving skill
        /// Formula: 1 at 50 skill, +1 per 25 skill (max 3 at GM)
        /// </summary>
        public static int GetMaxEnchantments(Mobile enchanter)
        {
            if (enchanter == null || enchanter.Skills == null)
                return 0;

            double runeweaving = enchanter.Skills[SkillName.Runeweaving].Value;
            if (runeweaving < 50.0)
                return 0;

            // 1 at 50, 2 at 75, 3 at 100
            return 1 + (int)((runeweaving - 50.0) / 25.0);
        }

        /// <summary>
        /// Get enchantment power multiplier based on Runeweaving skill
        /// Formula: +1% per 10 skill (100% at GM)
        /// </summary>
        public static double GetEnchantmentPower(Mobile enchanter)
        {
            if (enchanter == null || enchanter.Skills == null)
                return 1.0;

            double runeweaving = enchanter.Skills[SkillName.Runeweaving].Value;
            if (runeweaving <= 0)
                return 1.0;

            // +1% per 10 skill = 0.1% per point = 100% at GM
            return 1.0 + (runeweaving * 0.01);
        }

        /// <summary>
        /// Get enchantment success chance based on Runeweaving skill
        /// Formula: Base 50% + 0.5% per skill point (100% at GM)
        /// </summary>
        public static double GetEnchantmentSuccessChance(Mobile enchanter)
        {
            if (enchanter == null || enchanter.Skills == null)
                return 0.0;

            double runeweaving = enchanter.Skills[SkillName.Runeweaving].Value;
            if (runeweaving <= 0)
                return 0.0;

            // Base 50% + 0.5% per point = 100% at GM
            double chance = Math.Min(1.0, 0.50 + (runeweaving * 0.005));

            // Vystia: Enchanter + Celestis Arcanum: Enchant success +5%
            if (enchanter is PlayerMobile pm)
            {
                double synergyBonus = Server.Custom.VystiaClasses.Systems.VystiaSkillIntegration.GetClassReligionSynergyEnchantBonus(pm);
                if (synergyBonus > 0)
                {
                    chance = Math.Min(1.0, chance + synergyBonus); // +5% success chance
                }
            }

            return chance;
        }

        /// <summary>
        /// Get enchantment duration in seconds based on Runeweaving skill
        /// Formula: Base 300 seconds (5 min) + 10 seconds per skill point (15 min at GM)
        /// </summary>
        public static TimeSpan GetEnchantmentDuration(Mobile enchanter)
        {
            if (enchanter == null || enchanter.Skills == null)
                return TimeSpan.FromSeconds(300);

            double runeweaving = enchanter.Skills[SkillName.Runeweaving].Value;
            if (runeweaving <= 0)
                return TimeSpan.FromSeconds(300);

            // Base 300 seconds + 10 per point = 1300 seconds (21.7 min) at GM
            int seconds = 300 + (int)(runeweaving * 10);
            return TimeSpan.FromSeconds(seconds);
        }

        /// <summary>
        /// Check if an item can be enchanted
        /// </summary>
        public static bool CanEnchant(Item item)
        {
            if (item == null || item.Deleted)
                return false;

            // Can enchant weapons, armor, and jewelry
            return item is BaseWeapon || item is BaseArmor || item is BaseJewel;
        }

        /// <summary>
        /// Apply an enchantment to an item
        /// This is a placeholder - full implementation would track enchantments per item
        /// </summary>
        public static bool TryEnchant(Mobile enchanter, Item item, EnchantmentType type, int value)
        {
            if (enchanter == null || item == null || !CanEnchant(item))
                return false;

            double successChance = GetEnchantmentSuccessChance(enchanter);
            if (Utility.RandomDouble() > successChance)
                return false;

            // Apply enchantment via AosAttributes or custom properties
            // This is a simplified version - full implementation would track multiple enchantments
            if (item is BaseWeapon weapon)
            {
                // Apply weapon damage bonus as example
                weapon.Attributes.WeaponDamage += (int)(value * GetEnchantmentPower(enchanter));
            }
            else if (item is BaseArmor armor)
            {
                // Apply armor bonus as example
                armor.Attributes.DefendChance += (int)(value * GetEnchantmentPower(enchanter));
            }

            return true;
        }
    }

    /// <summary>
    /// Types of enchantments that can be applied
    /// </summary>
    public enum EnchantmentType
    {
        WeaponDamage,
        WeaponSpeed,
        WeaponHitChance,
        ArmorDefense,
        ArmorResistance,
        StatBonus,
        SkillBonus,
        ManaRegen,
        StaminaRegen,
        HitPointRegen
    }
}

