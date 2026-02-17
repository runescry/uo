using System;
using Server.Items;
using Server.Items.Vystia;

namespace Server.Engines.Craft
{
    /// <summary>
    /// Engineering Crafting System - Uses SkillName.Engineering (ID 80)
    /// For Artificer class - uses Ironclad mechanical components
    /// </summary>
    public class DefEngineering : CraftSystem
    {
        public override SkillName MainSkill => SkillName.Engineering;

        public override string GumpTitleString => "<CENTER>ENGINEERING MENU</CENTER>";

        private static CraftSystem m_CraftSystem;

        public static CraftSystem CraftSystem
        {
            get
            {
                if (m_CraftSystem == null)
                    m_CraftSystem = new DefEngineering();

                return m_CraftSystem;
            }
        }

        public override double GetChanceAtMin(CraftItem item)
        {
            return 0.0; // 0% at minimum skill
        }

        private DefEngineering()
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
            from.PlaySound(0x22F); // Mechanical whirring
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
                from.PlaySound(0x2A); // Click sound
                return 1044154; // You create the item.
            }
        }

        public override void InitCraftList()
        {
            int index = -1;

            // ============================================
            // BASIC COMPONENTS (Low skill)
            // ============================================

            // Clockwork Gear - Skill 0-50
            index = AddCraft(typeof(ClockworkGear), "Basic Components", "Clockwork Gear", -25.0, 25.0, typeof(ClockworkIngot), "Clockwork Ingot", 1, "You need clockwork ingot to make this.");

            // Clockwork Spring - Skill 15-65
            index = AddCraft(typeof(ClockworkSpring), "Basic Components", "Clockwork Spring", 15.0, 65.0, typeof(ClockworkIngot), "Clockwork Ingot", 2, "You need clockwork ingots.");

            // Steam Core - Skill 55-105
            index = AddCraft(typeof(SteamCore), "Basic Components", "Steam Core", 55.0, 105.0, typeof(ClockworkIngot), "Clockwork Ingot", 5, "You need clockwork ingots.");
            AddRes(index, typeof(ClockworkGear), "Clockwork Gear", 3, "You need clockwork gears.");

            // ============================================
            // GADGETS (Medium skill)
            // ============================================

            // Smoke Bomb (Engineering version) - Skill 45-95
            index = AddCraft(typeof(SmokeBomb), "Gadgets", "Smoke Grenade", 45.0, 95.0, typeof(ClockworkGear), "Clockwork Gear", 2, "You need clockwork gears.");
            AddRes(index, typeof(ClockworkSpring), "Clockwork Spring", 1, "You need a clockwork spring.");

            // Lesser Explosion Potion (Mechanical) - Skill 25-75
            index = AddCraft(typeof(VystiaLesserEngineeringExplosive), "Gadgets", "Small Explosive", 25.0, 75.0, typeof(ClockworkGear), "Clockwork Gear", 1, "You need clockwork gears.");
            AddRes(index, typeof(Bottle), "Bottle", 1, "You need a bottle.");

            // Explosion Potion (Mechanical) - Skill 50-100
            index = AddCraft(typeof(VystiaEngineeringExplosive), "Gadgets", "Medium Explosive", 50.0, 100.0, typeof(ClockworkGear), "Clockwork Gear", 2, "You need clockwork gears.");
            AddRes(index, typeof(ClockworkSpring), "Clockwork Spring", 1, "You need a spring.");
            AddRes(index, typeof(Bottle), "Bottle", 1, "You need a bottle.");

            // Greater Explosion Potion (Mechanical) - Skill 80-130
            index = AddCraft(typeof(VystiaGreaterEngineeringExplosive), "Gadgets", "Large Explosive", 80.0, 130.0, typeof(SteamCore), "Steam Core", 1, "You need a steam core.");
            AddRes(index, typeof(ClockworkGear), "Clockwork Gear", 3, "You need clockwork gears.");
            AddRes(index, typeof(Bottle), "Bottle", 1, "You need a bottle.");

            // ============================================
            // TOOLS (Medium skill)
            // ============================================

            // Engineering Tool Kit - Skill 35-85
            index = AddCraft(typeof(EngineeringToolKit), "Tools", "Engineering Tool Kit", 35.0, 85.0, typeof(ClockworkIngot), "Clockwork Ingot", 3, "You need clockwork ingots.");
            AddRes(index, typeof(ClockworkGear), "Clockwork Gear", 2, "You need clockwork gears.");

            // ============================================
            // CLOCKWORK ITEMS (High skill)
            // ============================================

            // Construct Control Device - Skill 70-100 (Vystia: 100 is GM)
            index = AddCraft(typeof(Server.Items.VystiaClassItems.ConstructControlDevice), "Clockwork Items", "Construct Control Device", 70.0, 100.0, typeof(SteamCore), "Steam Core", 2, "You need steam cores.");
            AddRes(index, typeof(ClockworkGear), "Clockwork Gear", 5, "You need clockwork gears.");
            AddRes(index, typeof(ClockworkSpring), "Clockwork Spring", 3, "You need clockwork springs.");

            // ============================================
            // CLOCKWORK EQUIPMENT (High skill)
            // ============================================

            // Clockwork Sword - Skill 65-115
            index = AddCraft(typeof(ClockworkSword), "Clockwork Equipment", "Clockwork Sword", 65.0, 115.0, typeof(ClockworkIngot), "Clockwork Ingot", 10, "You need clockwork ingots.");
            AddRes(index, typeof(ClockworkGear), "Clockwork Gear", 3, "You need clockwork gears.");

            // Clockwork Shield - Skill 60-110
            index = AddCraft(typeof(Server.Items.Vystia.ClockworkShield), "Clockwork Equipment", "Clockwork Shield", 60.0, 110.0, typeof(ClockworkIngot), "Clockwork Ingot", 8, "You need clockwork ingots.");
            AddRes(index, typeof(ClockworkGear), "Clockwork Gear", 2, "You need clockwork gears.");

            // Clockwork Plate Helm - Skill 55-105
            index = AddCraft(typeof(ClockworkPlateHelm), "Clockwork Equipment", "Clockwork Plate Helm", 55.0, 105.0, typeof(ClockworkIngot), "Clockwork Ingot", 5, "You need clockwork ingots.");
            AddRes(index, typeof(ClockworkGear), "Clockwork Gear", 1, "You need clockwork gear.");

            // Clockwork Plate Chest - Skill 75-125
            index = AddCraft(typeof(ClockworkPlateChest), "Clockwork Equipment", "Clockwork Plate Chest", 75.0, 125.0, typeof(ClockworkIngot), "Clockwork Ingot", 15, "You need clockwork ingots.");
            AddRes(index, typeof(ClockworkGear), "Clockwork Gear", 3, "You need clockwork gears.");

            // Clockwork Plate Arms - Skill 60-110
            index = AddCraft(typeof(ClockworkPlateArms), "Clockwork Equipment", "Clockwork Plate Arms", 60.0, 110.0, typeof(ClockworkIngot), "Clockwork Ingot", 6, "You need clockwork ingots.");
            AddRes(index, typeof(ClockworkGear), "Clockwork Gear", 2, "You need clockwork gears.");

            // Clockwork Plate Gloves - Skill 55-105
            index = AddCraft(typeof(ClockworkPlateGloves), "Clockwork Equipment", "Clockwork Plate Gloves", 55.0, 105.0, typeof(ClockworkIngot), "Clockwork Ingot", 4, "You need clockwork ingots.");
            AddRes(index, typeof(ClockworkGear), "Clockwork Gear", 2, "You need clockwork gears.");

            // Clockwork Plate Legs - Skill 65-115
            index = AddCraft(typeof(ClockworkPlateLegs), "Clockwork Equipment", "Clockwork Plate Legs", 65.0, 115.0, typeof(ClockworkIngot), "Clockwork Ingot", 10, "You need clockwork ingots.");
            AddRes(index, typeof(ClockworkGear), "Clockwork Gear", 2, "You need clockwork gears.");

            // Clockwork Plate Gorget - Skill 50-100
            index = AddCraft(typeof(ClockworkPlateGorget), "Clockwork Equipment", "Clockwork Plate Gorget", 50.0, 100.0, typeof(ClockworkIngot), "Clockwork Ingot", 3, "You need clockwork ingots.");
            AddRes(index, typeof(ClockworkGear), "Clockwork Gear", 1, "You need clockwork gear.");

            // ============================================
            // CONSTRUCT CORES (Very high skill)
            // ============================================

            // Clockwork Spider Core - Skill 40-90 (Scout, 1 slot)
            index = AddCraft(typeof(Server.Items.Vystia.ClockworkSpiderCore), "Construct Cores", "Clockwork Spider Core", 40.0, 90.0, typeof(ClockworkIngot), "Clockwork Ingot", 5, "You need clockwork ingots.");
            AddRes(index, typeof(ClockworkGear), "Clockwork Gear", 4, "You need clockwork gears.");
            AddRes(index, typeof(ClockworkSpring), "Clockwork Spring", 2, "You need clockwork springs.");

            // Repair Drone Core - Skill 55-105 (Healer, 1 slot)
            index = AddCraft(typeof(Server.Items.Vystia.RepairDroneCore), "Construct Cores", "Repair Drone Core", 55.0, 105.0, typeof(ClockworkIngot), "Clockwork Ingot", 8, "You need clockwork ingots.");
            AddRes(index, typeof(ClockworkGear), "Clockwork Gear", 5, "You need clockwork gears.");
            AddRes(index, typeof(SteamCore), "Steam Core", 1, "You need a steam core.");

            // Steam Turret Core - Skill 65-115 (Ranged, 2 slots)
            index = AddCraft(typeof(Server.Items.Vystia.SteamTurretCore), "Construct Cores", "Steam Turret Core", 65.0, 115.0, typeof(ClockworkIngot), "Clockwork Ingot", 12, "You need clockwork ingots.");
            AddRes(index, typeof(ClockworkGear), "Clockwork Gear", 6, "You need clockwork gears.");
            AddRes(index, typeof(SteamCore), "Steam Core", 2, "You need steam cores.");

            // Iron Golem Core - Skill 80-130 (Tank, 3 slots)
            index = AddCraft(typeof(Server.Items.Vystia.IronGolemCore), "Construct Cores", "Iron Golem Core", 80.0, 130.0, typeof(ClockworkIngot), "Clockwork Ingot", 25, "You need clockwork ingots.");
            AddRes(index, typeof(ClockworkGear), "Clockwork Gear", 10, "You need clockwork gears.");
            AddRes(index, typeof(SteamCore), "Steam Core", 4, "You need steam cores.");
            AddRes(index, typeof(ClockworkSpring), "Clockwork Spring", 5, "You need clockwork springs.");

            // Siege Engine Core - Skill 95-145 (Siege, 5 slots)
            index = AddCraft(typeof(Server.Items.Vystia.SiegeEngineCore), "Construct Cores", "Siege Engine Core", 95.0, 145.0, typeof(ClockworkIngot), "Clockwork Ingot", 50, "You need clockwork ingots.");
            AddRes(index, typeof(ClockworkGear), "Clockwork Gear", 20, "You need clockwork gears.");
            AddRes(index, typeof(SteamCore), "Steam Core", 8, "You need steam cores.");
            AddRes(index, typeof(ClockworkSpring), "Clockwork Spring", 10, "You need clockwork springs.");

            Console.WriteLine("[Vystia] Engineering system initialized with 21 recipes.");
        }
    }

    /// <summary>
    /// Engineering Tool Kit - Portable engineering station
    /// Uses SkillName.Engineering for crafting
    /// </summary>
    public class EngineeringToolKit : Item, ITool
    {
        private int m_UsesRemaining;

        [CommandProperty(AccessLevel.GameMaster)]
        public int UsesRemaining
        {
            get { return m_UsesRemaining; }
            set { m_UsesRemaining = value; InvalidateProperties(); }
        }

        public bool ShowUsesRemaining
        {
            get { return true; }
            set { }
        }

        public CraftSystem CraftSystem => DefEngineering.CraftSystem;

        public bool BreakOnDepletion => true;

        [Constructable]
        public EngineeringToolKit() : this(50)
        {
        }

        [Constructable]
        public EngineeringToolKit(int uses) : base(0x1EB8) // Clockwork item
        {
            Name = "Engineering Tool Kit";
            Hue = 2305; // Ironclad metallic hue
            Weight = 5.0;
            m_UsesRemaining = uses;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add(1060584, m_UsesRemaining.ToString()); // uses remaining: ~1_val~
            list.Add(1070722, "Portable engineering station");
            list.Add("Uses Engineering skill");
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (!IsChildOf(from.Backpack))
            {
                from.SendLocalizedMessage(1042001); // That must be in your pack
                return;
            }

            if (m_UsesRemaining <= 0)
            {
                from.SendMessage(0x22, "This tool kit has been worn out.");
                return;
            }

            from.SendMessage(0x3B2, "You open your engineering tool kit...");
            from.SendGump(new CraftGump(from, CraftSystem, this, null));
        }

        public bool CheckAccessible(Mobile from, ref int num)
        {
            if (RootParent != from)
            {
                num = 1044263; // The tool must be on your person to use.
                return false;
            }

            return true;
        }

        public EngineeringToolKit(Serial serial) : base(serial) { }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version
            writer.Write(m_UsesRemaining);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
            m_UsesRemaining = reader.ReadInt();
        }
    }
}
