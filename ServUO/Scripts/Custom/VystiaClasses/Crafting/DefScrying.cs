using System;
using Server.Items;
using Server.Items.Vystia;

namespace Server.Engines.Craft
{
    /// <summary>
    /// Scrying Crafting System - Uses SkillName.Divination (ID 63)
    /// For Oracle class - creates prophecy scrolls and scrying tools
    /// </summary>
    public class DefScrying : CraftSystem
    {
        public override SkillName MainSkill => SkillName.Divination;

        public override string GumpTitleString => "<CENTER>SCRYING MENU</CENTER>";

        private static CraftSystem m_CraftSystem;

        public static CraftSystem CraftSystem
        {
            get
            {
                if (m_CraftSystem == null)
                    m_CraftSystem = new DefScrying();

                return m_CraftSystem;
            }
        }

        public override double GetChanceAtMin(CraftItem item)
        {
            return 0.0; // 0% at minimum skill
        }

        private DefScrying()
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
            from.PlaySound(0x1F4); // Mystical sound
        }

        public override int PlayEndingEffect(Mobile from, bool failed, bool lostMaterial, bool toolBroken, int quality, bool makersMark, CraftItem item)
        {
            if (toolBroken)
                from.SendLocalizedMessage(1044038); // You have worn out your tool

            if (failed)
            {
                from.PlaySound(0x1F2); // Fizzle
                return 1044043; // You failed to create the item, and some of your materials are lost.
            }
            else
            {
                from.PlaySound(0x1F4); // Mystical sound
                return 1044154; // You create the item.
            }
        }

        public override void InitCraftList()
        {
            int index = -1;

            // ============================================
            // TOOLS (Skill 20-70)
            // ============================================

            // Scrying Kit - Skill 20-70
            index = AddCraft(typeof(ScryingKit), "Tools", "Scrying Kit", 20.0, 70.0, typeof(TimeSand), "Time Sand", 5, "You need time sand.");
            AddRes(index, typeof(TimeDust), "Time Dust", 3, "You need time dust.");

            // ============================================
            // INSIGHT SCROLLS (Skill 0-60) - Reveal Info
            // ============================================

            // Scroll of Insight - Skill 0-50
            index = AddCraft(typeof(ScrollOfInsight), "Insight Scrolls", "Scroll of Insight", 0.0, 50.0, typeof(TimeSand), "Time Sand", 2, "You need time sand.");
            AddRes(index, typeof(BlankScroll), "Blank Scroll", 1, "You need a blank scroll.");

            // Scroll of Weakness - Skill 25-75
            index = AddCraft(typeof(ScrollOfWeakness), "Insight Scrolls", "Scroll of Weakness", 25.0, 75.0, typeof(TimeDust), "Time Dust", 3, "You need time dust.");
            AddRes(index, typeof(DivinationDust), "Divination Dust", 2, "You need divination dust.");
            AddRes(index, typeof(BlankScroll), "Blank Scroll", 1, "You need a blank scroll.");

            // Scroll of Fate - Skill 50-100
            index = AddCraft(typeof(ScrollOfFate), "Insight Scrolls", "Scroll of Fate", 50.0, 100.0, typeof(DivinationDust), "Divination Dust", 4, "You need divination dust.");
            AddRes(index, typeof(FateCrystal), "Fate Crystal", 2, "You need fate crystals.");
            AddRes(index, typeof(BlankScroll), "Blank Scroll", 1, "You need a blank scroll.");

            // ============================================
            // BLESSING SCROLLS (Skill 40-90) - Apply Buffs
            // ============================================

            // Scroll of Foresight - Skill 40-90
            index = AddCraft(typeof(ScrollOfForesight), "Blessing Scrolls", "Scroll of Foresight", 40.0, 90.0, typeof(DivinationDust), "Divination Dust", 3, "You need divination dust.");
            AddRes(index, typeof(FateCrystal), "Fate Crystal", 2, "You need fate crystals.");
            AddRes(index, typeof(BlankScroll), "Blank Scroll", 1, "You need a blank scroll.");

            // Scroll of Clarity - Skill 55-105
            index = AddCraft(typeof(ScrollOfClarity), "Blessing Scrolls", "Scroll of Clarity", 55.0, 105.0, typeof(FateCrystal), "Fate Crystal", 3, "You need fate crystals.");
            AddRes(index, typeof(StarlightCrystal), "Starlight Crystal", 2, "You need starlight crystals.");
            AddRes(index, typeof(BlankScroll), "Blank Scroll", 1, "You need a blank scroll.");

            // Scroll of Destiny - Skill 90-140
            index = AddCraft(typeof(ScrollOfDestiny), "Blessing Scrolls", "Scroll of Destiny", 90.0, 140.0, typeof(SeeingStone), "Seeing Stone", 3, "You need seeing stones.");
            AddRes(index, typeof(FateThread), "Fate Thread", 2, "You need fate threads.");
            AddRes(index, typeof(PropheticLeaf), "Prophetic Leaf", 3, "You need prophetic leaves.");
            AddRes(index, typeof(BlankScroll), "Blank Scroll", 1, "You need a blank scroll.");

            // ============================================
            // SCRYING TOOLS (Skill 50-100)
            // ============================================

            // Scrying Crystal Ball - Skill 60-110
            index = AddCraft(typeof(ScryingCrystalBall), "Scrying Tools", "Scrying Crystal Ball", 60.0, 110.0, typeof(FateCrystal), "Fate Crystal", 5, "You need fate crystals.");
            AddRes(index, typeof(StarlightCrystal), "Starlight Crystal", 3, "You need starlight crystals.");

            // Oracle's Compass - Skill 70-120
            index = AddCraft(typeof(OraclesCompass), "Scrying Tools", "Oracle's Compass", 70.0, 120.0, typeof(StarlightCrystal), "Starlight Crystal", 4, "You need starlight crystals.");
            AddRes(index, typeof(PropheticLeaf), "Prophetic Leaf", 3, "You need prophetic leaves.");
            AddRes(index, typeof(SeeingStone), "Seeing Stone", 2, "You need seeing stones.");

            // ============================================
            // TALISMANS (Skill 65-115)
            // ============================================

            // Seer's Talisman - Skill 65-115
            index = AddCraft(typeof(SeersTalisman), "Talismans", "Seer's Talisman", 65.0, 115.0, typeof(StarlightCrystal), "Starlight Crystal", 5, "You need starlight crystals.");
            AddRes(index, typeof(PropheticLeaf), "Prophetic Leaf", 3, "You need prophetic leaves.");

            // Oracle's Eye Talisman - Skill 80-130
            index = AddCraft(typeof(OraclesEyeTalisman), "Talismans", "Oracle's Eye Talisman", 80.0, 130.0, typeof(SeeingStone), "Seeing Stone", 4, "You need seeing stones.");
            AddRes(index, typeof(FateThread), "Fate Thread", 3, "You need fate threads.");
            AddRes(index, typeof(PropheticLeaf), "Prophetic Leaf", 2, "You need prophetic leaves.");

            Console.WriteLine("[Vystia] Scrying system initialized with 12 recipes.");
        }
    }
}
