using System;
using Server.Items;

namespace Server.Engines.Craft
{
    /// <summary>
    /// Woodshaping Crafting System - Uses SkillName.Carpentry
    /// For Druid class - uses wood and natural materials
    /// </summary>
    public class DefWoodshaping : CraftSystem
    {
        public override SkillName MainSkill => SkillName.Carpentry;

        public override string GumpTitleString => "<CENTER>WOODSHAPING MENU</CENTER>";

        private static CraftSystem m_CraftSystem;

        public static CraftSystem CraftSystem
        {
            get
            {
                if (m_CraftSystem == null)
                    m_CraftSystem = new DefWoodshaping();

                return m_CraftSystem;
            }
        }

        public override double GetChanceAtMin(CraftItem item)
        {
            return 0.0; // 0% at minimum skill
        }

        private DefWoodshaping()
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
            from.PlaySound(0x23D); // Wood working sound
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
                from.PlaySound(0x23D); // Wood working sound
                return 1044154; // You create the item.
            }
        }

        public override void InitCraftList()
        {
            int index = -1;

            // ============================================
            // TOOLS (Skill 20-70)
            // ============================================

            index = AddCraft(typeof(Server.Items.Vystia.WoodshapingKit), "Tools", "Woodshaping Kit", 20.0, 70.0, typeof(Board), "Board", 10, "You need boards.");

            // ============================================
            // STAVES (Skill 40-90)
            // ============================================

            index = AddCraft(typeof(Server.Items.Vystia.DruidStaff), "Staves", "Druid's Staff", 50.0, 100.0, typeof(Board), "Board", 15, "You need boards.");
            AddRes(index, typeof(Cloth), "Cloth", 3, "You need cloth.");

            // ============================================
            // TOTEMS (Skill 50-100)
            // ============================================

            index = AddCraft(typeof(Server.Items.Vystia.NatureTotem), "Totems", "Nature Totem", 55.0, 105.0, typeof(Board), "Board", 20, "You need boards.");
            AddRes(index, typeof(Cloth), "Cloth", 5, "You need cloth.");

            index = AddCraft(typeof(Server.Items.Vystia.GreaterNatureTotem), "Totems", "Greater Nature Totem", 80.0, 130.0, typeof(Board), "Board", 30, "You need boards.");
            AddRes(index, typeof(Cloth), "Cloth", 10, "You need cloth.");
            AddRes(index, typeof(OakBoard), "Oak Board", 10, "You need oak boards.");

            // ============================================
            // BOWS (Skill 45-95)
            // ============================================

            index = AddCraft(typeof(Bow), "Bows", "Wooden Bow", 30.0, 80.0, typeof(Board), "Board", 8, "You need boards.");

            index = AddCraft(typeof(CompositeBow), "Bows", "Composite Bow", 60.0, 110.0, typeof(Board), "Board", 12, "You need boards.");
            AddRes(index, typeof(Leather), "Leather", 5, "You need leather.");

            index = AddCraft(typeof(Crossbow), "Bows", "Crossbow", 55.0, 105.0, typeof(Board), "Board", 15, "You need boards.");
            AddRes(index, typeof(IronIngot), "Iron Ingot", 5, "You need ingots.");

            Console.WriteLine("[Vystia] Woodshaping system initialized with 8 recipes.");
        }
    }
}
