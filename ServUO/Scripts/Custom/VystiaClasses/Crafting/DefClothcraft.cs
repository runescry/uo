using System;
using Server.Items;

namespace Server.Engines.Craft
{
    /// <summary>
    /// Clothcraft Crafting System - Uses SkillName.Tailoring
    /// For Bard class - uses cloth and fabric
    /// </summary>
    public class DefClothcraft : CraftSystem
    {
        public override SkillName MainSkill => SkillName.Tailoring;

        public override string GumpTitleString => "<CENTER>CLOTHCRAFT MENU</CENTER>";

        private static CraftSystem m_CraftSystem;

        public static CraftSystem CraftSystem
        {
            get
            {
                if (m_CraftSystem == null)
                    m_CraftSystem = new DefClothcraft();

                return m_CraftSystem;
            }
        }

        public override double GetChanceAtMin(CraftItem item)
        {
            return 0.0; // 0% at minimum skill
        }

        private DefClothcraft()
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
            from.PlaySound(0x248); // Cloth working sound
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
                from.PlaySound(0x248); // Cloth working sound
                return 1044154; // You create the item.
            }
        }

        public override void InitCraftList()
        {
            int index = -1;

            // ============================================
            // TOOLS (Skill 20-70)
            // ============================================

            index = AddCraft(typeof(Server.Items.Vystia.ClothcraftKit), "Tools", "Clothcraft Kit", 20.0, 70.0, typeof(Cloth), "Cloth", 10, "You need cloth.");

            // ============================================
            // BARDIC ROBES (Skill 40-100)
            // ============================================

            index = AddCraft(typeof(Server.Items.Vystia.BardicRobe), "Bardic Wear", "Bardic Robe", 50.0, 100.0, typeof(Cloth), "Cloth", 20, "You need cloth.");

            index = AddCraft(typeof(Server.Items.Vystia.MaestroRobe), "Bardic Wear", "Maestro's Robe", 85.0, 135.0, typeof(Cloth), "Cloth", 30, "You need cloth.");
            AddRes(index, typeof(SpoolOfThread), "Thread", 10, "You need thread.");

            // ============================================
            // CLOAKS (Skill 35-85)
            // ============================================

            index = AddCraft(typeof(Server.Items.Vystia.BardicCloak), "Cloaks", "Bardic Cloak", 40.0, 90.0, typeof(Cloth), "Cloth", 12, "You need cloth.");

            index = AddCraft(typeof(Cloak), "Cloaks", "Plain Cloak", 30.0, 80.0, typeof(Cloth), "Cloth", 8, "You need cloth.");

            // ============================================
            // HATS (Skill 30-80)
            // ============================================

            index = AddCraft(typeof(Server.Items.Vystia.BardicCap), "Hats", "Bardic Cap", 35.0, 85.0, typeof(Cloth), "Cloth", 8, "You need cloth.");

            index = AddCraft(typeof(FeatheredHat), "Hats", "Feathered Hat", 45.0, 95.0, typeof(Cloth), "Cloth", 10, "You need cloth.");

            // ============================================
            // BASIC CLOTHING (Skill 0-50)
            // ============================================

            index = AddCraft(typeof(FancyShirt), "Basic Clothing", "Fancy Shirt", 25.0, 75.0, typeof(Cloth), "Cloth", 6, "You need cloth.");

            index = AddCraft(typeof(Doublet), "Basic Clothing", "Doublet", 30.0, 80.0, typeof(Cloth), "Cloth", 8, "You need cloth.");

            Console.WriteLine("[Vystia] Clothcraft system initialized with 9 recipes.");
        }
    }
}
