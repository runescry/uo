using System;
using Server.Items;

namespace Server.Engines.Craft
{
    /// <summary>
    /// Jewelcraft Crafting System - Uses SkillName.Tinkering
    /// For Sorcerer class - uses gems and precious metals
    /// </summary>
    public class DefJewelcraft : CraftSystem
    {
        public override SkillName MainSkill => SkillName.Tinkering;

        public override string GumpTitleString => "<CENTER>JEWELCRAFT MENU</CENTER>";

        private static CraftSystem m_CraftSystem;

        public static CraftSystem CraftSystem
        {
            get
            {
                if (m_CraftSystem == null)
                    m_CraftSystem = new DefJewelcraft();

                return m_CraftSystem;
            }
        }

        public override double GetChanceAtMin(CraftItem item)
        {
            return 0.0; // 0% at minimum skill
        }

        private DefJewelcraft()
            : base(1, 1, 1.25) // min/max craft effect, delay multiplier
        {
        }

        public override int CanCraft(Mobile from, ITool tool, Type itemType)
        {
            if (tool == null || tool.Deleted || tool.UsesRemaining <= 0)
                return 1044038; // You have worn out your tool!

            int num = 0;
            if (!tool.CheckAccessible(from, ref num))
                return num; // The tool must be on your person to use.

            return 0;
        }

        public override void PlayCraftEffect(Mobile from)
        {
            from.PlaySound(0x241); // Gem working sound
        }

        public override int PlayEndingEffect(Mobile from, bool failed, bool lostMaterial, bool toolBroken, int quality, bool makersMark, CraftItem item)
        {
            if (toolBroken)
                from.SendLocalizedMessage(1044038); // You have worn out your tool

            if (failed)
            {
                return 1044043; // You failed to create the item, and some of your materials are lost.
            }
            else
            {
                from.PlaySound(0x241); // Gem working sound
                return 1044154; // You create the item.
            }
        }

        public override void InitCraftList()
        {
            int index = -1;

            // ============================================
            // TOOLS (Skill 20-70)
            // ============================================

            index = AddCraft(typeof(Server.Items.Vystia.JewelcraftKit), "Tools", "Jewelcraft Kit", 20.0, 70.0, typeof(IronIngot), "Iron Ingot", 5, "You need ingots.");
            AddRes(index, typeof(Ruby), "Ruby", 1, "You need a gem.");

            // ============================================
            // BASIC JEWELRY (Skill 40-90)
            // ============================================

            index = AddCraft(typeof(Server.Items.Vystia.ElementalRing), "Elemental Jewelry", "Elemental Ring", 50.0, 100.0, typeof(GoldIngot), "Gold Ingot", 3, "You need gold.");
            AddRes(index, typeof(Ruby), "Ruby", 1, "You need a ruby.");

            index = AddCraft(typeof(Server.Items.Vystia.SorcerersBracelet), "Elemental Jewelry", "Sorcerer's Bracelet", 55.0, 105.0, typeof(GoldIngot), "Gold Ingot", 4, "You need gold.");
            AddRes(index, typeof(StarSapphire), "Star Sapphire", 1, "You need a star sapphire.");

            // ============================================
            // GREATER JEWELRY (Skill 70-120)
            // ============================================

            index = AddCraft(typeof(Server.Items.Vystia.GreaterElementalRing), "Greater Jewelry", "Greater Elemental Ring", 80.0, 130.0, typeof(GoldIngot), "Gold Ingot", 5, "You need gold.");
            AddRes(index, typeof(Ruby), "Ruby", 2, "You need rubies.");
            AddRes(index, typeof(StarSapphire), "Star Sapphire", 1, "You need a star sapphire.");

            index = AddCraft(typeof(Server.Items.Vystia.MasterSorcerersBracelet), "Greater Jewelry", "Master Sorcerer's Bracelet", 90.0, 140.0, typeof(GoldIngot), "Gold Ingot", 8, "You need gold.");
            AddRes(index, typeof(Diamond), "Diamond", 2, "You need diamonds.");
            AddRes(index, typeof(StarSapphire), "Star Sapphire", 2, "You need star sapphires.");

            // ============================================
            // SPELL FOCUS (Skill 60-110)
            // ============================================

            index = AddCraft(typeof(Server.Items.Vystia.SpellFocus), "Spell Focus", "Spell Focus", 65.0, 115.0, typeof(GoldIngot), "Gold Ingot", 5, "You need gold.");
            AddRes(index, typeof(StarSapphire), "Star Sapphire", 2, "You need star sapphires.");
            AddRes(index, typeof(Emerald), "Emerald", 1, "You need an emerald.");

            // ============================================
            // BASIC RINGS (Skill 30-80)
            // ============================================

            index = AddCraft(typeof(GoldRing), "Basic Jewelry", "Gold Ring", 30.0, 80.0, typeof(GoldIngot), "Gold Ingot", 2, "You need gold.");

            index = AddCraft(typeof(GoldBracelet), "Basic Jewelry", "Gold Bracelet", 35.0, 85.0, typeof(GoldIngot), "Gold Ingot", 3, "You need gold.");

            Console.WriteLine("[Vystia] Jewelcraft system initialized with 9 recipes.");
        }
    }
}
