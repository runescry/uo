using System;
using System.Collections.Generic;
using Server;
using Server.Items;
using Server.Mobiles;
using Server.Targeting;

namespace Server.Commands
{
    public class VystiaLootCrate
    {
        public static void Initialize()
        {
            CommandSystem.Register("droploot", AccessLevel.GameMaster, new CommandEventHandler(DropLoot_OnCommand));
            CommandSystem.Register("clearloot", AccessLevel.GameMaster, new CommandEventHandler(ClearLoot_OnCommand));
        }

        private static List<Item> m_SpawnedItems = new List<Item>();

        // Layout configuration
        private const int TABLE_SPACING_X = 3;  // Horizontal spacing between tables
        private const int ROW_SPACING_Y = 3;    // Vertical spacing between rows
        private const int START_OFFSET_Y = -2;  // Start 2 tiles north of GM

        // Column positions (X offset from GM)
        private const int COL_RESOURCES = 0;
        private const int COL_WEAPONS = 4;
        private const int COL_ARMOR = 8;
        private const int COL_SHIELDS = 12;

        [Usage("droploot")]
        [Description("Spawns organized Vystia loot display north of the GM - tables with chests by region and item type.")]
        private static void DropLoot_OnCommand(CommandEventArgs e)
        {
            Mobile from = e.Mobile;

            // Clear any existing items first
            ClearItems();

            int spawned = 0;
            int baseY = from.Y + START_OFFSET_Y;

            // Row 0: Headers/Legendary/Global Resources
            int row0Y = baseY;
            spawned += SpawnLabeledTable(from, COL_RESOURCES, row0Y, "Resources", 0, new VystiaGlobalResourceCrate());
            spawned += SpawnLabeledTable(from, COL_WEAPONS, row0Y, "Legendary", 1153, new VystiaLegendaryWeaponCrate());
            spawned += SpawnLabeledTable(from, COL_ARMOR, row0Y, "Boss Loot", 1157, new VystiaBossCrate());
            spawned += SpawnLabeledTable(from, COL_SHIELDS, row0Y, "All Shields", 1153, new VystiaShieldCrate());

            // Row 1: Frosthold
            int row1Y = baseY - ROW_SPACING_Y;
            spawned += SpawnLabeledTable(from, COL_RESOURCES, row1Y, "Frosthold", 1152, new VystiaFrostholdResourceCrate());
            spawned += SpawnLabeledTable(from, COL_WEAPONS, row1Y, "Frosthold", 1152, new VystiaFrostholdWeaponCrate());
            spawned += SpawnLabeledTable(from, COL_ARMOR, row1Y, "Frosthold", 1152, new VystiaFrostholdArmorCrate());

            // Row 2: Emberlands
            int row2Y = baseY - ROW_SPACING_Y * 2;
            spawned += SpawnLabeledTable(from, COL_RESOURCES, row2Y, "Emberlands", 1358, new VystiaEmberlandsResourceCrate());
            spawned += SpawnLabeledTable(from, COL_WEAPONS, row2Y, "Emberlands", 1358, new VystiaEmberlandsWeaponCrate());
            spawned += SpawnLabeledTable(from, COL_ARMOR, row2Y, "Emberlands", 1358, new VystiaEmberlandsArmorCrate());

            // Row 3: Crystal Barrens
            int row3Y = baseY - ROW_SPACING_Y * 3;
            spawned += SpawnLabeledTable(from, COL_RESOURCES, row3Y, "Crystal", 1154, new VystiaCrystalResourceCrate());
            spawned += SpawnLabeledTable(from, COL_WEAPONS, row3Y, "Crystal", 1154, new VystiaCrystalWeaponCrate());
            spawned += SpawnLabeledTable(from, COL_ARMOR, row3Y, "Crystal", 1154, new VystiaCrystalArmorCrate());

            // Row 4: Shadowfen
            int row4Y = baseY - ROW_SPACING_Y * 4;
            spawned += SpawnLabeledTable(from, COL_RESOURCES, row4Y, "Shadowfen", 1109, new VystiaShadowfenResourceCrate());
            spawned += SpawnLabeledTable(from, COL_WEAPONS, row4Y, "Shadowfen", 1109, new VystiaShadowfenWeaponCrate());
            spawned += SpawnLabeledTable(from, COL_ARMOR, row4Y, "Shadowfen", 1109, new VystiaShadowfenArmorCrate());

            // Row 5: Verdantpeak
            int row5Y = baseY - ROW_SPACING_Y * 5;
            spawned += SpawnLabeledTable(from, COL_RESOURCES, row5Y, "Verdantpeak", 2010, new VystiaVerdantpeakResourceCrate());
            spawned += SpawnLabeledTable(from, COL_WEAPONS, row5Y, "Verdantpeak", 2010, new VystiaVerdantpeakWeaponCrate());
            spawned += SpawnLabeledTable(from, COL_ARMOR, row5Y, "Verdantpeak", 2010, new VystiaVerdantpeakArmorCrate());

            // Row 6: Desert
            int row6Y = baseY - ROW_SPACING_Y * 6;
            spawned += SpawnLabeledTable(from, COL_RESOURCES, row6Y, "Desert", 2305, new VystiaDesertResourceCrate());
            spawned += SpawnLabeledTable(from, COL_WEAPONS, row6Y, "Desert", 2305, new VystiaDesertWeaponCrate());
            spawned += SpawnLabeledTable(from, COL_ARMOR, row6Y, "Desert", 2305, new VystiaDesertArmorCrate());

            // Row 7: Ironclad
            int row7Y = baseY - ROW_SPACING_Y * 7;
            spawned += SpawnLabeledTable(from, COL_RESOURCES, row7Y, "Ironclad", 2401, new VystiaIroncladResourceCrate());
            spawned += SpawnLabeledTable(from, COL_WEAPONS, row7Y, "Ironclad", 2401, new VystiaIroncladWeaponCrate());
            spawned += SpawnLabeledTable(from, COL_ARMOR, row7Y, "Ironclad", 2401, new VystiaIroncladArmorCrate());

            // Row 8: Obsidian
            int row8Y = baseY - ROW_SPACING_Y * 8;
            spawned += SpawnLabeledTable(from, COL_RESOURCES, row8Y, "Obsidian", 1109, new VystiaObsidianResourceCrate());
            spawned += SpawnLabeledTable(from, COL_WEAPONS, row8Y, "Obsidian", 1109, new VystiaObsidianWeaponCrate());
            spawned += SpawnLabeledTable(from, COL_ARMOR, row8Y, "Obsidian", 1109, new VystiaObsidianArmorCrate());

            from.SendMessage(0x35, "Spawned {0} Vystia loot tables north of your position. Use [clearloot to remove them.", spawned);
        }

        [Usage("clearloot")]
        [Description("Removes all spawned Vystia loot displays.")]
        private static void ClearLoot_OnCommand(CommandEventArgs e)
        {
            int count = ClearItems();
            e.Mobile.SendMessage(0x35, "Removed {0} Vystia loot items.", count);
        }

        private static int ClearItems()
        {
            int count = 0;
            foreach (Item item in m_SpawnedItems)
            {
                if (item != null && !item.Deleted)
                {
                    item.Delete();
                    count++;
                }
            }
            m_SpawnedItems.Clear();
            return count;
        }

        private static int SpawnLabeledTable(Mobile from, int offsetX, int y, string label, int hue, Item chest)
        {
            int x = from.X + offsetX;
            int z = from.Z;
            Map map = from.Map;

            // Create the table
            Item table = new Static(0xB90); // LargeTable graphic
            table.Name = label;
            table.Hue = hue;
            table.Movable = false;
            table.MoveToWorld(new Point3D(x, y, z), map);
            m_SpawnedItems.Add(table);

            // Place the chest on the table (6 Z units higher)
            chest.MoveToWorld(new Point3D(x, y, z + 6), map);
            m_SpawnedItems.Add(chest);

            return 1;
        }
    }

    #region Global Resource Crate

    public class VystiaGlobalResourceCrate : WoodenChest
    {
        [Constructable]
        public VystiaGlobalResourceCrate()
        {
            Name = "Vystia Global Resources";
            Hue = 0;

            // Phase 1 resources - samples of each
            DropItem(new FrozenOre(5));
            DropItem(new MoltenOre(5));
            DropItem(new CrystalOre(5));
            DropItem(new FrostforgedIngot(5));
            DropItem(new EmberforgedIngot(5));
            DropItem(new CrystallineIngot(5));
            DropItem(new FrostwillowLog(5));
            DropItem(new FlamewoodLog(5));
            DropItem(new FrostHide(5));
            DropItem(new FireHide(5));

            // Phase 2 reagents
            DropItem(new FrostEssence(5));
            // REMOVED OLD REAGENT: DropItem(new EmberBloom(5));
            DropItem(new StormCrystal(5));
            DropItem(new VoidDust(5));
            DropItem(new LivingBark(5));
            DropItem(new SwampLotus(5));
            DropItem(new ClockworkGear(5));
            DropItem(new ClockworkSpring(5));
            DropItem(new SteamCore(3));

            // Special resources
            DropItem(new EternalIce(3));
            DropItem(new EverburningCoal(3));
            DropItem(new PrismaticShard(3));
        }

        public VystiaGlobalResourceCrate(Serial serial) : base(serial) { }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    #endregion

    #region Regional Resource Crates

    public class VystiaFrostholdResourceCrate : WoodenChest
    {
        [Constructable]
        public VystiaFrostholdResourceCrate()
        {
            Name = "Frosthold Resources";
            Hue = 1152;

            DropItem(new FrozenOre(10));
            DropItem(new FrostforgedIngot(10));
            DropItem(new FrostwillowLog(10));
            DropItem(new FrostwillowBoard(10));
            DropItem(new FrostHide(10));
            DropItem(new FrostLeather(10));
            DropItem(new FrostEssence(10));
            DropItem(new EternalIce(5));
            DropItem(new IceCrystal(5));
            DropItem(new FrozenArtifact(5));
        }

        public VystiaFrostholdResourceCrate(Serial serial) : base(serial) { }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class VystiaEmberlandsResourceCrate : WoodenChest
    {
        [Constructable]
        public VystiaEmberlandsResourceCrate()
        {
            Name = "Emberlands Resources";
            Hue = 1358;

            DropItem(new MoltenOre(10));
            DropItem(new EmberforgedIngot(10));
            DropItem(new FlamewoodLog(10));
            DropItem(new FlamewoodBoard(10));
            DropItem(new FireHide(10));
            DropItem(new FireLeather(10));
            // REMOVED OLD REAGENT: DropItem(new EmberBloom(10));
            DropItem(new EverburningCoal(5));
            DropItem(new Server.Items.LavaPearl(5));
        }

        public VystiaEmberlandsResourceCrate(Serial serial) : base(serial) { }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class VystiaCrystalResourceCrate : WoodenChest
    {
        [Constructable]
        public VystiaCrystalResourceCrate()
        {
            Name = "Crystal Barrens Resources";
            Hue = 1154;

            DropItem(new CrystalOre(10));
            DropItem(new CrystallineIngot(10));
            DropItem(new CrystalwoodLog(10));
            DropItem(new CrystalwoodBoard(10));
            DropItem(new StormCrystal(10));
            // REMOVED OLD REAGENT: DropItem(new CrystalPollen(10));
            DropItem(new PrismaticShard(5));
            DropItem(new LeyCrystal(5));
            DropItem(new LeyLineEssence(5));
        }

        public VystiaCrystalResourceCrate(Serial serial) : base(serial) { }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class VystiaShadowfenResourceCrate : WoodenChest
    {
        [Constructable]
        public VystiaShadowfenResourceCrate()
        {
            Name = "Shadowfen Resources";
            Hue = 1109;

            DropItem(new BogIronOre(10));
            DropItem(new ShadowforgedIngot(10));
            DropItem(new ShadowwoodLog(10));
            DropItem(new ShadowwoodBoard(10));
            DropItem(new ShadowHide(10));
            DropItem(new ShadowLeather(10));
            DropItem(new SwampLotus(10));
            DropItem(new VoidDust(10));
            DropItem(new ShadowSilk(5));
        }

        public VystiaShadowfenResourceCrate(Serial serial) : base(serial) { }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class VystiaVerdantpeakResourceCrate : WoodenChest
    {
        [Constructable]
        public VystiaVerdantpeakResourceCrate()
        {
            Name = "Verdantpeak Resources";
            Hue = 2010;

            DropItem(new LivingOre(10));
            DropItem(new NatureforgedIngot(10));
            DropItem(new LivingwoodLog(10));
            DropItem(new LivingwoodBoard(10));
            DropItem(new LivingBark(10));
            DropItem(new TreantHeart(5));

            for (int i = 0; i < 5; i++)
                DropItem(new HeartwoodCoreFragment());
        }

        public VystiaVerdantpeakResourceCrate(Serial serial) : base(serial) { }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class VystiaDesertResourceCrate : WoodenChest
    {
        [Constructable]
        public VystiaDesertResourceCrate()
        {
            Name = "Desert Resources";
            Hue = 2305;

            DropItem(new SandstoneOre(10));
            DropItem(new SunforgedIngot(10));
            DropItem(new PetrifiedWoodLog(10));
            DropItem(new PetrifiedWoodBoard(10));
            // REMOVED OLD REAGENT: DropItem(new DesertRose(10));
            DropItem(new TimeDust(5));
        }

        public VystiaDesertResourceCrate(Serial serial) : base(serial) { }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class VystiaIroncladResourceCrate : WoodenChest
    {
        [Constructable]
        public VystiaIroncladResourceCrate()
        {
            Name = "Ironclad Resources";
            Hue = 2401;

            DropItem(new SteamworkOre(10));
            DropItem(new ClockworkIngot(10));
            DropItem(new IronwoodLog(10));
            DropItem(new IronwoodBoard(10));
            DropItem(new ClockworkGear(10));
            DropItem(new ClockworkSpring(10));
            DropItem(new SteamCore(5));
        }

        public VystiaIroncladResourceCrate(Serial serial) : base(serial) { }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class VystiaObsidianResourceCrate : WoodenChest
    {
        [Constructable]
        public VystiaObsidianResourceCrate()
        {
            Name = "Obsidian Resources";
            Hue = 1109;

            DropItem(new ObsidianOre(10));
            DropItem(new VoidforgedIngot(10));
            DropItem(new VoidDust(10));
            DropItem(new ShadowSilk(5));
            // REMOVED OLD REAGENT: DropItem(new DragonScalePowder(5));
            DropItem(new PhoenixFeather(5));
            // REMOVED OLD REAGENT: DropItem(new KrakenInk(5));
        }

        public VystiaObsidianResourceCrate(Serial serial) : base(serial) { }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    #endregion

    #region Legendary Weapon Crate

    public class VystiaLegendaryWeaponCrate : MetalGoldenChest
    {
        [Constructable]
        public VystiaLegendaryWeaponCrate()
        {
            Name = "Vystia Legendary Weapons";
            Hue = 1153;

            DropItem(new TheEternalWinter());
            DropItem(new PhoenixAscension());
            DropItem(new TheCogmaster());
            DropItem(new PrismaticEdge());
            DropItem(new Voidcaller());
        }

        public VystiaLegendaryWeaponCrate(Serial serial) : base(serial) { }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    #endregion

    #region Regional Weapon Crates

    public class VystiaFrostholdWeaponCrate : MetalChest
    {
        [Constructable]
        public VystiaFrostholdWeaponCrate()
        {
            Name = "Frosthold Weapons";
            Hue = 1152;

            DropItem(new IcicleBlade());
            DropItem(new WintersEdge());
            DropItem(new Frostbite());
            DropItem(new GlacierShard());
        }

        public VystiaFrostholdWeaponCrate(Serial serial) : base(serial) { }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class VystiaEmberlandsWeaponCrate : MetalChest
    {
        [Constructable]
        public VystiaEmberlandsWeaponCrate()
        {
            Name = "Emberlands Weapons";
            Hue = 1358;

            DropItem(new FlameTongue());
            DropItem(new MagmaBlade());
            DropItem(new PhoenixWing());
            DropItem(new LavaEdge());
        }

        public VystiaEmberlandsWeaponCrate(Serial serial) : base(serial) { }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class VystiaCrystalWeaponCrate : MetalChest
    {
        [Constructable]
        public VystiaCrystalWeaponCrate()
        {
            Name = "Crystal Barrens Weapons";
            Hue = 1154;

            DropItem(new CrystalBlade());
            DropItem(new PrismShard());
            DropItem(new LeyCutter());
            DropItem(new ResonanceMaul());
        }

        public VystiaCrystalWeaponCrate(Serial serial) : base(serial) { }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class VystiaShadowfenWeaponCrate : MetalChest
    {
        [Constructable]
        public VystiaShadowfenWeaponCrate()
        {
            Name = "Shadowfen Weapons";
            Hue = 1109;

            DropItem(new ShadowFang());
            DropItem(new BogCleaver());
            DropItem(new VenomSting());
            DropItem(new MireCrusher());
        }

        public VystiaShadowfenWeaponCrate(Serial serial) : base(serial) { }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class VystiaVerdantpeakWeaponCrate : MetalChest
    {
        [Constructable]
        public VystiaVerdantpeakWeaponCrate()
        {
            Name = "Verdantpeak Weapons";
            Hue = 2010;

            DropItem(new NaturesBlade());
            DropItem(new ThornEdge());
            DropItem(new SeedlingKnife());
            DropItem(new IronbarkMaul());
        }

        public VystiaVerdantpeakWeaponCrate(Serial serial) : base(serial) { }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class VystiaDesertWeaponCrate : MetalChest
    {
        [Constructable]
        public VystiaDesertWeaponCrate()
        {
            Name = "Desert Weapons";
            Hue = 2305;

            DropItem(new SunBlade());
            DropItem(new DuneScimitar());
            DropItem(new MirageStiletto());
            DropItem(new SandstormAxe());
        }

        public VystiaDesertWeaponCrate(Serial serial) : base(serial) { }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class VystiaIroncladWeaponCrate : MetalChest
    {
        [Constructable]
        public VystiaIroncladWeaponCrate()
        {
            Name = "Ironclad Weapons";
            Hue = 2401;

            DropItem(new ClockworkBlade());
            DropItem(new GearEdge());
            DropItem(new PistonPunch());
            DropItem(new SteamHammer());
        }

        public VystiaIroncladWeaponCrate(Serial serial) : base(serial) { }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class VystiaObsidianWeaponCrate : MetalChest
    {
        [Constructable]
        public VystiaObsidianWeaponCrate()
        {
            Name = "Obsidian Weapons";
            Hue = 1109;

            DropItem(new VoidBlade());
            DropItem(new ObsidianEdge());
            DropItem(new ShadowFangDagger());
            DropItem(new VoidCrusher());
        }

        public VystiaObsidianWeaponCrate(Serial serial) : base(serial) { }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    #endregion

    #region Legacy Weapon Crate (backwards compatibility)

    public class VystiaWeaponCrate : VystiaLegendaryWeaponCrate
    {
        [Constructable]
        public VystiaWeaponCrate() : base() { }

        public VystiaWeaponCrate(Serial serial) : base(serial) { }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    #endregion

    #region Regional Armor Crates

    public class VystiaFrostholdArmorCrate : MetalChest
    {
        [Constructable]
        public VystiaFrostholdArmorCrate()
        {
            Name = "Frosthold Armor";
            Hue = 1152;

            DropItem(new FrostforgedHelm());
            DropItem(new FrostforgedGorget());
            DropItem(new FrostforgedChest());
            DropItem(new FrostforgedArms());
            DropItem(new FrostforgedGloves());
            DropItem(new FrostforgedLegs());
            DropItem(new FrostforgedShield());
        }

        public VystiaFrostholdArmorCrate(Serial serial) : base(serial) { }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class VystiaEmberlandsArmorCrate : MetalChest
    {
        [Constructable]
        public VystiaEmberlandsArmorCrate()
        {
            Name = "Emberlands Armor";
            Hue = 1358;

            DropItem(new EmberforgedHelm());
            DropItem(new EmberforgedGorget());
            DropItem(new EmberforgedChest());
            DropItem(new EmberforgedArms());
            DropItem(new EmberforgedGloves());
            DropItem(new EmberforgedLegs());
            DropItem(new EmberforgedShield());
        }

        public VystiaEmberlandsArmorCrate(Serial serial) : base(serial) { }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class VystiaCrystalArmorCrate : MetalChest
    {
        [Constructable]
        public VystiaCrystalArmorCrate()
        {
            Name = "Crystal Barrens Armor";
            Hue = 1154;

            DropItem(new CrystallineHelm());
            DropItem(new CrystallineGorget());
            DropItem(new CrystallineChest());
            DropItem(new CrystallineArms());
            DropItem(new CrystallineGloves());
            DropItem(new CrystallineLegs());
            DropItem(new CrystallineShield());
        }

        public VystiaCrystalArmorCrate(Serial serial) : base(serial) { }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class VystiaShadowfenArmorCrate : MetalChest
    {
        [Constructable]
        public VystiaShadowfenArmorCrate()
        {
            Name = "Shadowfen Armor";
            Hue = 1109;

            DropItem(new ShadowforgedHelm());
            DropItem(new ShadowforgedGorget());
            DropItem(new ShadowforgedChest());
            DropItem(new ShadowforgedArms());
            DropItem(new ShadowforgedGloves());
            DropItem(new ShadowforgedLegs());
            DropItem(new ShadowforgedShield());
        }

        public VystiaShadowfenArmorCrate(Serial serial) : base(serial) { }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class VystiaVerdantpeakArmorCrate : MetalChest
    {
        [Constructable]
        public VystiaVerdantpeakArmorCrate()
        {
            Name = "Verdantpeak Armor";
            Hue = 2010;

            DropItem(new NatureforgedHelm());
            DropItem(new NatureforgedGorget());
            DropItem(new NatureforgedChest());
            DropItem(new NatureforgedArms());
            DropItem(new NatureforgedGloves());
            DropItem(new NatureforgedLegs());
            DropItem(new NatureforgedShield());
        }

        public VystiaVerdantpeakArmorCrate(Serial serial) : base(serial) { }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class VystiaDesertArmorCrate : MetalChest
    {
        [Constructable]
        public VystiaDesertArmorCrate()
        {
            Name = "Desert Armor";
            Hue = 2305;

            DropItem(new SunforgedHelm());
            DropItem(new SunforgedGorget());
            DropItem(new SunforgedChest());
            DropItem(new SunforgedArms());
            DropItem(new SunforgedGloves());
            DropItem(new SunforgedLegs());
            DropItem(new SunforgedShield());
        }

        public VystiaDesertArmorCrate(Serial serial) : base(serial) { }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class VystiaIroncladArmorCrate : MetalChest
    {
        [Constructable]
        public VystiaIroncladArmorCrate()
        {
            Name = "Ironclad Armor";
            Hue = 2401;

            DropItem(new ClockworkHelm());
            DropItem(new ClockworkGorget());
            DropItem(new ClockworkChest());
            DropItem(new ClockworkArms());
            DropItem(new ClockworkGloves());
            DropItem(new ClockworkLegs());
            DropItem(new ClockworkShield());
        }

        public VystiaIroncladArmorCrate(Serial serial) : base(serial) { }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class VystiaObsidianArmorCrate : MetalChest
    {
        [Constructable]
        public VystiaObsidianArmorCrate()
        {
            Name = "Obsidian Armor";
            Hue = 1109;

            DropItem(new VoidforgedHelm());
            DropItem(new VoidforgedGorget());
            DropItem(new VoidforgedChest());
            DropItem(new VoidforgedArms());
            DropItem(new VoidforgedGloves());
            DropItem(new VoidforgedLegs());
            DropItem(new VoidforgedShield());
        }

        public VystiaObsidianArmorCrate(Serial serial) : base(serial) { }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    #endregion

    #region Shield Crate

    public class VystiaShieldCrate : MetalChest
    {
        [Constructable]
        public VystiaShieldCrate()
        {
            Name = "Vystia Regional Shields";
            Hue = 1153;

            DropItem(new FrostforgedShield());
            DropItem(new EmberforgedShield());
            DropItem(new CrystallineShield());
            DropItem(new ShadowforgedShield());
            DropItem(new NatureforgedShield());
            DropItem(new SunforgedShield());
            DropItem(new ClockworkShield());
            DropItem(new VoidforgedShield());
        }

        public VystiaShieldCrate(Serial serial) : base(serial) { }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    #endregion

    #region Boss Crate

    public class VystiaBossCrate : MetalGoldenChest
    {
        [Constructable]
        public VystiaBossCrate()
        {
            Name = "Vystia Boss Loot";
            Hue = 1157;

            DropItem(new FrozenArtifact(20));

            DropItem(new TheEternalWinter());
            DropItem(new PhoenixAscension());
            DropItem(new TheCogmaster());
            DropItem(new PrismaticEdge());
            DropItem(new Voidcaller());

            for (int i = 0; i < 5; i++)
                DropItem(new HeartwoodCoreFragment());

            DropItem(new FrozenOre(25));
            DropItem(new FrostforgedIngot(15));
            DropItem(new FrostwillowLog(15));
            DropItem(new FrostHide(10));
        }

        public VystiaBossCrate(Serial serial) : base(serial) { }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    #endregion
}
