using System;
using Server.Items;

namespace Server.Engines.Craft
{
    /// <summary>
    /// Leathercraft Crafting System - Uses SkillName.Tailoring
    /// For Ranger class - uses leather and hides
    /// </summary>
    public class DefLeathercraft : CraftSystem
    {
        public override SkillName MainSkill => SkillName.Tailoring;

        public override string GumpTitleString => "<CENTER>LEATHERCRAFT MENU</CENTER>";

        private static CraftSystem m_CraftSystem;

        public static CraftSystem CraftSystem
        {
            get
            {
                if (m_CraftSystem == null)
                    m_CraftSystem = new DefLeathercraft();

                return m_CraftSystem;
            }
        }

        public override double GetChanceAtMin(CraftItem item)
        {
            return 0.0; // 0% at minimum skill
        }

        private DefLeathercraft()
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
            from.PlaySound(0x248); // Leather working sound
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
                from.PlaySound(0x248); // Leather working sound
                return 1044154; // You create the item.
            }
        }

        public override void InitCraftList()
        {
            int index = -1;

            // ============================================
            // TOOLS (Skill 20-70)
            // ============================================

            index = AddCraft(typeof(Server.Items.Vystia.LeathercraftKit), "Tools", "Leathercraft Kit", 20.0, 70.0, typeof(Leather), "Leather", 10, "You need leather.");

            // ============================================
            // QUIVERS (Skill 40-90)
            // ============================================

            index = AddCraft(typeof(Server.Items.Vystia.RangerQuiver), "Quivers", "Ranger's Quiver", 40.0, 90.0, typeof(Leather), "Leather", 15, "You need leather.");
            AddRes(index, typeof(Cloth), "Cloth", 5, "You need cloth.");

            index = AddCraft(typeof(Server.Items.Vystia.EliteRangerQuiver), "Quivers", "Elite Ranger's Quiver", 75.0, 125.0, typeof(Leather), "Leather", 25, "You need leather.");
            AddRes(index, typeof(Cloth), "Cloth", 10, "You need cloth.");
            AddRes(index, typeof(SpinedLeather), "Spined Leather", 5, "You need spined leather.");

            // ============================================
            // RANGER ARMOR (Skill 50-100)
            // ============================================

            index = AddCraft(typeof(Server.Items.Vystia.RangerLeatherCap), "Ranger Armor", "Ranger's Leather Cap", 50.0, 100.0, typeof(Leather), "Leather", 8, "You need leather.");

            index = AddCraft(typeof(Server.Items.Vystia.RangerLeatherChest), "Ranger Armor", "Ranger's Leather Tunic", 60.0, 110.0, typeof(Leather), "Leather", 15, "You need leather.");
            AddRes(index, typeof(SpinedLeather), "Spined Leather", 3, "You need spined leather.");

            index = AddCraft(typeof(Server.Items.Vystia.RangerLeatherLegs), "Ranger Armor", "Ranger's Leather Leggings", 55.0, 105.0, typeof(Leather), "Leather", 12, "You need leather.");

            index = AddCraft(typeof(Server.Items.Vystia.RangerLeatherGloves), "Ranger Armor", "Ranger's Leather Gloves", 45.0, 95.0, typeof(Leather), "Leather", 6, "You need leather.");

            Console.WriteLine("[Vystia] Leathercraft system initialized with 7 recipes.");
        }
    }
}
