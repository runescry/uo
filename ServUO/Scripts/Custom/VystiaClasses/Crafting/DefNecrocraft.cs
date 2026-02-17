using System;
using Server.Items;

namespace Server.Engines.Craft
{
    /// <summary>
    /// Necrocraft Crafting System - Uses SkillName.Necromancy (ID 64)
    /// For Necromancer class - uses bones and soul essences
    /// </summary>
    public class DefNecrocraft : CraftSystem
    {
        public override SkillName MainSkill => SkillName.Necromancy;

        public override string GumpTitleString => "<CENTER>NECROCRAFT MENU</CENTER>";

        private static CraftSystem m_CraftSystem;

        public static CraftSystem CraftSystem
        {
            get
            {
                if (m_CraftSystem == null)
                    m_CraftSystem = new DefNecrocraft();

                return m_CraftSystem;
            }
        }

        public override double GetChanceAtMin(CraftItem item)
        {
            return 0.0; // 0% at minimum skill
        }

        private DefNecrocraft()
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
            from.PlaySound(0x1F6); // Dark magic sound
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
                from.PlaySound(0x1F6); // Dark magic sound
                return 1044154; // You create the item.
            }
        }

        public override void InitCraftList()
        {
            int index = -1;

            // ============================================
            // TOOLS (Skill 20-70)
            // ============================================

            index = AddCraft(typeof(Server.Items.Vystia.NecrocraftKit), "Tools", "Necrocraft Kit", 20.0, 70.0, typeof(Bone), "Bone", 10, "You need bones.");

            // ============================================
            // BONE WEAPONS (Skill 40-90)
            // ============================================

            index = AddCraft(typeof(Server.Items.Vystia.BoneWand), "Bone Weapons", "Bone Wand", 40.0, 90.0, typeof(Bone), "Bone", 8, "You need bones.");

            index = AddCraft(typeof(Server.Items.Vystia.SkullStaff), "Bone Weapons", "Skull Staff", 70.0, 120.0, typeof(Bone), "Bone", 25, "You need bones.");
            AddRes(index, typeof(NightSightPotion), "Night Sight Potion", 1, "You need a potion for the dark ritual.");

            // ============================================
            // SOUL ITEMS (Skill 50-100)
            // ============================================

            index = AddCraft(typeof(Server.Items.Vystia.SoulVessel), "Soul Items", "Soul Vessel", 55.0, 105.0, typeof(Bone), "Bone", 10, "You need bones.");
            AddRes(index, typeof(Bottle), "Bottle", 1, "You need a bottle.");

            // ============================================
            // BONE ARMOR (Skill 60-110)
            // ============================================

            index = AddCraft(typeof(Server.Items.Vystia.BoneArmor), "Bone Armor", "Necromancer's Bone Armor", 65.0, 115.0, typeof(Bone), "Bone", 25, "You need bones.");
            AddRes(index, typeof(Leather), "Leather", 10, "You need leather.");

            index = AddCraft(typeof(BoneHelm), "Bone Armor", "Bone Helm", 50.0, 100.0, typeof(Bone), "Bone", 12, "You need bones.");

            index = AddCraft(typeof(BoneLegs), "Bone Armor", "Bone Leggings", 55.0, 105.0, typeof(Bone), "Bone", 18, "You need bones.");

            index = AddCraft(typeof(BoneArms), "Bone Armor", "Bone Arms", 45.0, 95.0, typeof(Bone), "Bone", 10, "You need bones.");

            index = AddCraft(typeof(BoneGloves), "Bone Armor", "Bone Gloves", 40.0, 90.0, typeof(Bone), "Bone", 8, "You need bones.");

            Console.WriteLine("[Vystia] Necrocraft system initialized with 10 recipes.");
        }
    }
}
