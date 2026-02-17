using System;
using Server;
using Server.Items;
using Server.Mobiles;
using Server.Commands;
using Server.Custom.VystiaClasses.Religion;
using Server.Custom.VystiaClasses.Factions;
// Ingots and Ores are in Server.Items namespace, not Server.Items.Vystia.Resources

namespace Server.Custom.VystiaClasses.Testing
{
    /// <summary>
    /// Comprehensive test items for Vystia systems
    /// Provides all materials, tools, and items needed for testing
    /// </summary>
    public class VystiaTestItems
    {
        public static void Initialize()
        {
            CommandSystem.Register("VystiaTestKit", AccessLevel.GameMaster, new CommandEventHandler(TestKit_OnCommand));
            CommandSystem.Register("VTK", AccessLevel.GameMaster, new CommandEventHandler(TestKit_OnCommand)); // Short alias - puts items in backpack
            CommandSystem.Register("vtk", AccessLevel.GameMaster, new CommandEventHandler(SpawnInGreenAcres_OnCommand)); // Spawns in Green Acres
            CommandSystem.Register("vtkclean", AccessLevel.GameMaster, new CommandEventHandler(CleanupGreenAcres_OnCommand)); // Cleanup vtk spawns
        }

        [Usage("VystiaTestKit")]
        [Description("Spawns a complete testing kit with all Vystia materials, tools, and test items")]
        private static void TestKit_OnCommand(CommandEventArgs e)
        {
            Mobile from = e.Mobile;
            if (from == null || from.Backpack == null)
                return;

            // Create container for test items
            Container testKit = new Backpack();
            testKit.Name = "Vystia Test Kit";
            testKit.Hue = 0x3B2; // Gold color

            // === RELIGION TEST ITEMS ===
            AddSectionLabel(testKit, "=== RELIGION TEST ITEMS ===");
            for (int i = 1; i <= 6; i++)
            {
                VystiaReligion religion = (VystiaReligion)i;
                testKit.DropItem(new VystiaShrineStone(religion));
            }

            // === FACTION TEST ITEMS ===
            AddSectionLabel(testKit, "=== FACTION TEST ITEMS ===");
            for (int i = 1; i <= 7; i++)
            {
                VystiaFaction faction = (VystiaFaction)i;
                testKit.DropItem(new VystiaFactionStone(faction));
            }

            // === CRAFTING MATERIALS - INGOTS ===
            AddSectionLabel(testKit, "=== CRAFTING INGOTS (100 each) ===");
            testKit.DropItem(new FrostforgedIngot(100));
            testKit.DropItem(new EmberforgedIngot(100));
            testKit.DropItem(new CrystallineIngot(100));
            testKit.DropItem(new ClockworkIngot(100));
            testKit.DropItem(new VoidforgedIngot(100));
            testKit.DropItem(new ShadowforgedIngot(100));
            testKit.DropItem(new SunforgedIngot(100));
            testKit.DropItem(new NatureforgedIngot(100));

            // === CRAFTING MATERIALS - ORES ===
            AddSectionLabel(testKit, "=== CRAFTING ORES (100 each) ===");
            testKit.DropItem(new FrozenOre(100));
            testKit.DropItem(new MoltenOre(100));
            testKit.DropItem(new CrystalOre(100)); // Note: Class name is CrystalOre, not CrystallineOre
            testKit.DropItem(new SteamworkOre(100));
            testKit.DropItem(new ObsidianOre(100)); // Note: Class name is ObsidianOre, not VoidOre
            testKit.DropItem(new BogIronOre(100)); // Note: Class name is BogIronOre, not ShadowOre
            testKit.DropItem(new SandstoneOre(100)); // Note: Class name is SandstoneOre, not SunOre
            testKit.DropItem(new LivingOre(100));

            // === CRAFTING TOOLS ===
            AddSectionLabel(testKit, "=== CRAFTING TOOLS ===");
            testKit.DropItem(new SmithHammer());
            testKit.DropItem(new TinkerTools());
            testKit.DropItem(new SewingKit());
            testKit.DropItem(new Scissors());
            testKit.DropItem(new DovetailSaw());
            testKit.DropItem(new MortarPestle());
            testKit.DropItem(new ScribesPen());
            testKit.DropItem(new Skillet());

            // === CRAFTING TABLES/STATIONS ===
            AddSectionLabel(testKit, "=== CRAFTING STATIONS (Place on ground) ===");
            testKit.DropItem(new VystiaCraftingStone("Forge", 0xFB1, 0x3B2)); // Forge
            testKit.DropItem(new VystiaCraftingStone("Anvil", 0xFAF, 0x3B2)); // Anvil
            testKit.DropItem(new VystiaCraftingStone("Carpentry Bench", 0x1E7F, 0x3B2)); // Carpentry bench
            testKit.DropItem(new VystiaCraftingStone("Alchemy Table", 0x1E9D, 0x3B2)); // Alchemy table
            testKit.DropItem(new VystiaCraftingStone("Tailoring Table", 0xF9C, 0x3B2)); // Tailoring table
            testKit.DropItem(new VystiaCraftingStone("Tinkering Table", 0x1EB9, 0x3B2)); // Tinkering table

            // === VENDOR TEST ITEMS ===
            AddSectionLabel(testKit, "=== VENDOR TEST ITEMS ===");
            testKit.DropItem(new VystiaVendorStone("Frostguard Vendor", VystiaFaction.Frostguard));
            testKit.DropItem(new VystiaVendorStone("Flame Legion Vendor", VystiaFaction.FlameLegion));
            testKit.DropItem(new VystiaVendorStone("Greenward Vendor", VystiaFaction.Greenward));
            testKit.DropItem(new VystiaVendorStone("Arcane Conclave Vendor", VystiaFaction.ArcaneConclave));
            testKit.DropItem(new VystiaVendorStone("Technoguild Vendor", VystiaFaction.Technoguild));
            testKit.DropItem(new VystiaVendorStone("Sandwalkers Vendor", VystiaFaction.Sandwalkers));
            testKit.DropItem(new VystiaVendorStone("Voidborn Vendor", VystiaFaction.Voidborn));

            // === REAGENTS ===
            AddSectionLabel(testKit, "=== MAGIC REAGENTS (100 each) ===");
            testKit.DropItem(new BlackPearl(100));
            testKit.DropItem(new Bloodmoss(100));
            testKit.DropItem(new Garlic(100));
            testKit.DropItem(new Ginseng(100));
            testKit.DropItem(new MandrakeRoot(100));
            testKit.DropItem(new Nightshade(100));
            testKit.DropItem(new SpidersSilk(100));
            testKit.DropItem(new SulfurousAsh(100));

            // === BASIC MATERIALS ===
            AddSectionLabel(testKit, "=== BASIC MATERIALS ===");
            testKit.DropItem(new IronIngot(100));
            testKit.DropItem(new Leather(100));
            testKit.DropItem(new Board(100));
            testKit.DropItem(new Cloth(100));

            from.Backpack.DropItem(testKit);
            from.SendMessage(0x35, "You have received a complete Vystia Test Kit!");
        }

        [Usage("vtk")]
        [Description("Spawns all Vystia test items in Green Acres area on tables with chests, 5 tiles apart")]
        private static void SpawnInGreenAcres_OnCommand(CommandEventArgs e)
        {
            Mobile from = e.Mobile;
            if (from == null)
                return;

            // Green Acres center coordinates
            int baseX = 5445;
            int baseY = 1153;
            int baseZ = 0;
            Map map = Map.Felucca; // Green Acres is on Felucca
            int tableSpacing = 5; // 5 tiles apart (tables are much smaller than houses)

            from.SendMessage(0x35, "Spawning Vystia test items in Green Acres on tables...");

            int currentX = baseX;
            int currentY = baseY;

            // === RELIGION SHRINES TABLE ===
            Container religionChest = CreateTableWithChest(map, currentX, currentY, baseZ, "Religion Shrine Stones", 0x3B2);
            for (int i = 1; i <= 6; i++)
            {
                VystiaReligion religion = (VystiaReligion)i;
                religionChest.DropItem(new VystiaShrineStone(religion));
            }
            currentX += tableSpacing;

            // === FACTION STONES TABLE ===
            Container factionChest = CreateTableWithChest(map, currentX, currentY, baseZ, "Faction Stones", 0x3B2);
            for (int i = 1; i <= 7; i++)
            {
                VystiaFaction faction = (VystiaFaction)i;
                factionChest.DropItem(new VystiaFactionStone(faction));
            }
            currentX += tableSpacing;

            // === ANCIENT BEING STONES TABLE ===
            Container ancientChest = CreateTableWithChest(map, currentX, currentY, baseZ, "Ancient Being Spawn Stones", 0x35);
            foreach (VystiaAncientStone.AncientType ancientType in Enum.GetValues(typeof(VystiaAncientStone.AncientType)))
            {
                ancientChest.DropItem(new VystiaAncientStone(ancientType));
            }
            currentX += tableSpacing;

            // === BOSS STONES TABLE ===
            Container bossChest = CreateTableWithChest(map, currentX, currentY, baseZ, "Boss Spawn Stones", 0x22);
            foreach (VystiaBossStone.BossType bossType in Enum.GetValues(typeof(VystiaBossStone.BossType)))
            {
                bossChest.DropItem(new VystiaBossStone(bossType));
            }
            currentX = baseX;
            currentY += tableSpacing;

            // === CRAFTING STATIONS TABLE ===
            Container craftingChest = CreateTableWithChest(map, currentX, currentY, baseZ, "Crafting Station Stones", 0x3B2);
            craftingChest.DropItem(new VystiaCraftingStone("Forge", 0xFB1, 0x3B2));
            craftingChest.DropItem(new VystiaCraftingStone("Anvil", 0xFAF, 0x3B2));
            craftingChest.DropItem(new VystiaCraftingStone("Carpentry Bench", 0x1E7F, 0x3B2));
            craftingChest.DropItem(new VystiaCraftingStone("Alchemy Table", 0x1E9D, 0x3B2));
            craftingChest.DropItem(new VystiaCraftingStone("Tailoring Table", 0xF9C, 0x3B2));
            craftingChest.DropItem(new VystiaCraftingStone("Tinkering Table", 0x1EB9, 0x3B2));
            currentX += tableSpacing;

            // === VENDOR STONES TABLE ===
            Container vendorChest = CreateTableWithChest(map, currentX, currentY, baseZ, "Vendor Spawn Stones", 0x3B2);
            vendorChest.DropItem(new VystiaVendorStone("Frostguard Vendor", VystiaFaction.Frostguard));
            vendorChest.DropItem(new VystiaVendorStone("Flame Legion Vendor", VystiaFaction.FlameLegion));
            vendorChest.DropItem(new VystiaVendorStone("Greenward Vendor", VystiaFaction.Greenward));
            vendorChest.DropItem(new VystiaVendorStone("Arcane Conclave Vendor", VystiaFaction.ArcaneConclave));
            vendorChest.DropItem(new VystiaVendorStone("Technoguild Vendor", VystiaFaction.Technoguild));
            vendorChest.DropItem(new VystiaVendorStone("Sandwalkers Vendor", VystiaFaction.Sandwalkers));
            vendorChest.DropItem(new VystiaVendorStone("Voidborn Vendor", VystiaFaction.Voidborn));
            currentX += tableSpacing;

            // === INGOTS TABLE ===
            Container ingotsChest = CreateTableWithChest(map, currentX, currentY, baseZ, "Vystia Ingots (100 each)", 0x3B2);
            ingotsChest.DropItem(new FrostforgedIngot(100));
            ingotsChest.DropItem(new EmberforgedIngot(100));
            ingotsChest.DropItem(new CrystallineIngot(100));
            ingotsChest.DropItem(new ClockworkIngot(100));
            ingotsChest.DropItem(new VoidforgedIngot(100));
            ingotsChest.DropItem(new ShadowforgedIngot(100));
            ingotsChest.DropItem(new SunforgedIngot(100));
            ingotsChest.DropItem(new NatureforgedIngot(100));
            currentX += tableSpacing;

            // === ORES TABLE ===
            Container oresChest = CreateTableWithChest(map, currentX, currentY, baseZ, "Vystia Ores (100 each)", 0x3B2);
            oresChest.DropItem(new FrozenOre(100));
            oresChest.DropItem(new MoltenOre(100));
            oresChest.DropItem(new CrystalOre(100));
            oresChest.DropItem(new SteamworkOre(100));
            oresChest.DropItem(new ObsidianOre(100));
            oresChest.DropItem(new BogIronOre(100));
            oresChest.DropItem(new SandstoneOre(100));
            oresChest.DropItem(new LivingOre(100));
            currentX = baseX;
            currentY += tableSpacing;

            // === REAGENTS TABLE ===
            Container reagentsChest = CreateTableWithChest(map, currentX, currentY, baseZ, "Magic Reagents (100 each)", 0x3B2);
            reagentsChest.DropItem(new BlackPearl(100));
            reagentsChest.DropItem(new Bloodmoss(100));
            reagentsChest.DropItem(new Garlic(100));
            reagentsChest.DropItem(new Ginseng(100));
            reagentsChest.DropItem(new MandrakeRoot(100));
            reagentsChest.DropItem(new Nightshade(100));
            reagentsChest.DropItem(new SpidersSilk(100));
            reagentsChest.DropItem(new SulfurousAsh(100));
            currentX += tableSpacing;

            // === BASIC MATERIALS TABLE ===
            Container basicChest = CreateTableWithChest(map, currentX, currentY, baseZ, "Basic Materials (100 each)", 0x3B2);
            basicChest.DropItem(new IronIngot(100));
            basicChest.DropItem(new Leather(100));
            basicChest.DropItem(new Board(100));
            basicChest.DropItem(new Cloth(100));
            currentX += tableSpacing;

            // === CRAFTING TOOLS TABLE ===
            Container toolsChest = CreateTableWithChest(map, currentX, currentY, baseZ, "Crafting Tools", 0x3B2);
            toolsChest.DropItem(new SmithHammer());
            toolsChest.DropItem(new TinkerTools());
            toolsChest.DropItem(new SewingKit());
            toolsChest.DropItem(new Scissors());
            toolsChest.DropItem(new DovetailSaw());
            toolsChest.DropItem(new MortarPestle());
            toolsChest.DropItem(new ScribesPen());
            toolsChest.DropItem(new Skillet());
            currentX += tableSpacing;

            // === COMBAT DUMMIES TABLE ===
            Container combatChest = CreateTableWithChest(map, currentX, currentY, baseZ, "Combat Dummy Stones", 0x455);
            // Mode variants
            combatChest.DropItem(new VystiaCombatDummyStone(VystiaFaction.None, VystiaReligion.None, CombatDummyMode.Passive));
            combatChest.DropItem(new VystiaCombatDummyStone(VystiaFaction.None, VystiaReligion.None, CombatDummyMode.Melee));
            combatChest.DropItem(new VystiaCombatDummyStone(VystiaFaction.None, VystiaReligion.None, CombatDummyMode.Caster));
            combatChest.DropItem(new VystiaCombatDummyStone(VystiaFaction.None, VystiaReligion.None, CombatDummyMode.Hybrid));
            // Faction variants
            combatChest.DropItem(new VystiaCombatDummyStone(VystiaFaction.Frostguard, VystiaReligion.None, CombatDummyMode.Hybrid));
            combatChest.DropItem(new VystiaCombatDummyStone(VystiaFaction.FlameLegion, VystiaReligion.None, CombatDummyMode.Hybrid));
            combatChest.DropItem(new VystiaCombatDummyStone(VystiaFaction.Voidborn, VystiaReligion.None, CombatDummyMode.Caster));
            // Religion variants
            combatChest.DropItem(new VystiaCombatDummyStone(VystiaFaction.None, VystiaReligion.FrosthelmFaith, CombatDummyMode.Hybrid));
            combatChest.DropItem(new VystiaCombatDummyStone(VystiaFaction.None, VystiaReligion.SuryasSandscript, CombatDummyMode.Melee));
            combatChest.DropItem(new VystiaCombatDummyStone(VystiaFaction.None, VystiaReligion.OceanasCovenant, CombatDummyMode.Caster));

            from.SendMessage(0x35, $"All Vystia test items have been spawned in Green Acres on tables!");
            from.SendMessage(0x3B2, "Tables are spaced 5 tiles apart starting at ({0}, {1})", baseX, baseY);
            from.SendMessage(0x3B2, "Use [Go {0} {1} {2} to teleport there.", baseX, baseY, baseZ);
            from.SendMessage(0x22, "Use [vtkclean to remove all spawned items and tables.");
        }

        [Usage("vtkclean")]
        [Description("Removes all items and tables spawned by [vtk command in Green Acres area")]
        private static void CleanupGreenAcres_OnCommand(CommandEventArgs e)
        {
            Mobile from = e.Mobile;
            if (from == null)
                return;

            // Green Acres area bounds (expanded to cover all tables)
            int baseX = 5445;
            int baseY = 1153;
            Map map = Map.Felucca;

            // Calculate bounds - tables span about 3 rows x 5 columns, 5 tiles apart
            int minX = baseX - 5;
            int maxX = baseX + (5 * 5) + 10;
            int minY = baseY - 5;
            int maxY = baseY + (3 * 5) + 10;

            from.SendMessage(0x35, "Cleaning up Green Acres test area...");

            // Collect items to delete first (don't modify collection while iterating)
            System.Collections.Generic.List<Item> itemsToDelete = new System.Collections.Generic.List<Item>();

            // Find all items in the area
            foreach (Item item in World.Items.Values)
            {
                try
                {
                    if (item == null || item.Deleted || item.Map != map)
                        continue;

                    Point3D loc = item.GetWorldLocation();

                    // Check if item is in our cleanup area
                    if (loc.X >= minX && loc.X <= maxX && loc.Y >= minY && loc.Y <= maxY)
                    {
                        bool shouldDelete = false;

                        // Check if it's a VTK table (identified by name)
                        if (item.Name != null && item.Name.StartsWith("VTK Table:"))
                        {
                            shouldDelete = true;
                        }
                        // Check if it's a mini house addon (legacy cleanup)
                        else if (item is MiniHouseAddon)
                        {
                            shouldDelete = true;
                        }
                        // Check if it's a chest with our naming pattern
                        else if (item is Container container)
                        {
                            string name = container.Name ?? "";
                            if (name.Contains("Shrine Stones") || name.Contains("Faction Stones") ||
                                name.Contains("Ancient Being") || name.Contains("Boss Spawn") ||
                                name.Contains("Crafting Station") || name.Contains("Vendor Spawn") ||
                                name.Contains("Vystia Ingots") || name.Contains("Vystia Ores") ||
                                name.Contains("Magic Reagents") || name.Contains("Basic Materials") ||
                                name.Contains("Crafting Tools") || name.Contains("Combat Dummy"))
                            {
                                shouldDelete = true;
                            }
                        }
                        // Check if it's a sign (sign post item ID 0x0BD2)
                        else if (item.ItemID == 0x0BD2)
                        {
                            string name = item.Name ?? "";
                            if (name.Contains("Religion") || name.Contains("Faction") ||
                                name.Contains("Ancient") || name.Contains("Boss") ||
                                name.Contains("Crafting") || name.Contains("Vendor") ||
                                name.Contains("Ingots") || name.Contains("Ores") ||
                                name.Contains("Reagents") || name.Contains("Materials") ||
                                name.Contains("Tools"))
                            {
                                shouldDelete = true;
                            }
                        }
                        // Check if it's a Vystia test item (stones, etc.)
                        else if (item is VystiaShrineStone || item is VystiaFactionStone ||
                                 item is VystiaAncientStone || item is VystiaBossStone ||
                                 item is VystiaCraftingStone || item is VystiaVendorStone ||
                                 item is VystiaCombatDummyStone)
                        {
                            shouldDelete = true;
                        }

                        if (shouldDelete)
                        {
                            itemsToDelete.Add(item);
                        }
                    }
                }
                catch
                {
                    // Skip items that cause errors (deleted, etc.)
                    continue;
                }
            }

            // Now delete all collected items
            int tableCount = 0;
            int chestCount = 0;
            int signCount = 0;
            int itemCount = 0;

            foreach (Item item in itemsToDelete)
            {
                try
                {
                    if (item == null || item.Deleted)
                        continue;

                    if (item.Name != null && item.Name.StartsWith("VTK Table:"))
                        tableCount++;
                    else if (item is MiniHouseAddon)
                        tableCount++; // Legacy houses count as tables
                    else if (item is Container)
                        chestCount++;
                    else if (item.ItemID == 0x0BD2)
                        signCount++;
                    else
                        itemCount++;

                    item.Delete();
                }
                catch
                {
                    // Skip items that can't be deleted
                    continue;
                }
            }

            int totalDeleted = tableCount + chestCount + signCount + itemCount;
            from.SendMessage(0x35, $"Cleanup complete! Deleted {totalDeleted} items:");
            from.SendMessage(0x3B2, $"  - {tableCount} tables");
            from.SendMessage(0x3B2, $"  - {chestCount} chests");
            from.SendMessage(0x3B2, $"  - {signCount} signs");
            from.SendMessage(0x3B2, $"  - {itemCount} test items");
        }

        private static Container CreateTableWithChest(Map map, int x, int y, int z, string chestName, int chestHue)
        {
            // Create table (0x0B8F is a writing table, height ~6)
            Item table = new Item(0x0B8F); // Writing table
            table.Name = "VTK Table: " + chestName;
            table.Movable = false;
            table.MoveToWorld(new Point3D(x, y, z), map);

            // Create chest on top of the table (table height is about 6)
            Container chest = new WoodenChest();
            chest.Name = chestName;
            chest.Hue = chestHue;
            chest.Movable = false;
            chest.MoveToWorld(new Point3D(x, y, z + 6), map);

            return chest;
        }

        private static void SpawnLabel(Map map, int x, int y, int z, string text, int hue)
        {
            Item label = new Item(0x0FEF); // Blank scroll
            label.Name = text;
            label.Hue = hue;
            label.Weight = 0;
            label.Movable = false;
            label.MoveToWorld(new Point3D(x, y, z), map);
        }

        private static void AddSectionLabel(Container container, string label)
        {
            Item labelItem = new Item(0x0FEF); // Blank scroll
            labelItem.Name = label;
            labelItem.Hue = 0x3B2;
            labelItem.Weight = 0;
            container.DropItem(labelItem);
        }
    }

    /// <summary>
    /// Crafting station stone - place on ground to create a crafting station
    /// </summary>
    public class VystiaCraftingStone : Item
    {
        private string m_StationName;
        private int m_StationItemID;

        [CommandProperty(AccessLevel.GameMaster)]
        public string StationName
        {
            get { return m_StationName; }
            set { m_StationName = value; InvalidateProperties(); }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int StationItemID
        {
            get { return m_StationItemID; }
            set { m_StationItemID = value; ItemID = value; }
        }

        [Constructable]
        public VystiaCraftingStone(string stationName, int itemID, int hue = 0)
            : base(itemID)
        {
            m_StationName = stationName;
            m_StationItemID = itemID;
            Name = stationName + " Stone";
            Hue = hue;
            Weight = 1.0;
            Movable = true;
        }

        public VystiaCraftingStone(Serial serial)
            : base(serial)
        {
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (from == null || !from.InRange(GetWorldLocation(), 2))
            {
                from.SendMessage(0x22, "You must be closer to use the crafting stone.");
                return;
            }

            // Create the crafting station
            Item station = new Item(m_StationItemID);
            station.Name = m_StationName;
            station.Movable = false;
            station.MoveToWorld(GetWorldLocation(), Map);

            from.SendMessage(0x35, "You have created a {0}!", m_StationName);
            from.PlaySound(0x1F2);

            // Delete the stone
            Delete();
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add(1060659, "Usage\tDouble-click to create {0}", m_StationName);
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // version
            writer.Write(m_StationName);
            writer.Write(m_StationItemID);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
            m_StationName = reader.ReadString();
            m_StationItemID = reader.ReadInt();
        }
    }

    /// <summary>
    /// Vendor stone - place on ground to spawn a faction vendor
    /// </summary>
    public class VystiaVendorStone : Item
    {
        private string m_VendorName;
        private VystiaFaction m_Faction;

        [CommandProperty(AccessLevel.GameMaster)]
        public string VendorName
        {
            get { return m_VendorName; }
            set { m_VendorName = value; InvalidateProperties(); }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public VystiaFaction Faction
        {
            get { return m_Faction; }
            set { m_Faction = value; InvalidateProperties(); }
        }

        [Constructable]
        public VystiaVendorStone(string vendorName, VystiaFaction faction)
            : base(0x1F13) // Gem graphic
        {
            m_VendorName = vendorName;
            m_Faction = faction;
            Name = vendorName + " Stone";
            Hue = FactionData.GetInfo(faction)?.Hue ?? 0;
            Weight = 1.0;
            Movable = true;
        }

        public VystiaVendorStone(Serial serial)
            : base(serial)
        {
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (from == null || !(from is PlayerMobile))
                return;

            if (!from.InRange(GetWorldLocation(), 2))
            {
                from.SendMessage(0x22, "You must be closer to use the vendor stone.");
                return;
            }

            // Create the appropriate vendor based on faction
            BaseVendor vendor = CreateVendor(m_Faction);
            if (vendor != null)
            {
                vendor.MoveToWorld(GetWorldLocation(), Map);
                from.SendMessage(0x35, "You have created a {0}!", m_VendorName);
                from.PlaySound(0x1F2);
                Delete();
            }
        }

        private BaseVendor CreateVendor(VystiaFaction faction)
        {
            switch (faction)
            {
                case VystiaFaction.Frostguard:
                    return new FrostguardVendor();
                case VystiaFaction.FlameLegion:
                    return new FlameLegionVendor();
                case VystiaFaction.Greenward:
                    return new GreenwardVendor();
                case VystiaFaction.ArcaneConclave:
                    return new ArcaneConclaveVendor();
                case VystiaFaction.Technoguild:
                    return new TechnoguildVendor();
                case VystiaFaction.Sandwalkers:
                    return new SandwalkersVendor();
                case VystiaFaction.Voidborn:
                    return new VoidbornVendor();
                default:
                    return null;
            }
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add(1060658, "Faction\t{0}", FactionData.GetInfo(m_Faction)?.Name ?? "Unknown");
            list.Add(1060659, "Usage\tDouble-click to spawn vendor");
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // version
            writer.Write(m_VendorName);
            writer.Write((int)m_Faction);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
            m_VendorName = reader.ReadString();
            m_Faction = (VystiaFaction)reader.ReadInt();
        }
    }
}

