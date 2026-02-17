using System;
using System.Collections.Generic;
using Server;
using Server.Items;
using Server.Mobiles;
using Server.Items.VystiaClassItems;
using Server.Items.Vystia;
using Server.Gumps;
using Server.Network;
using Server.Commands;
using Server.Custom.VystiaClasses.Religion;
using Server.Custom.VystiaClasses.Factions;
using Server.Custom.VystiaClasses.Abilities;

namespace Server.Custom.VystiaClasses.Testing
{
    public class VystiaTestKitGump : Gump
    {
        private PlayerMobile m_Player;
        private bool m_SpawnAtPlayer; // true = spawn at player, false = spawn at Green Acres

        // Green Acres coordinates
        private const int GreenAcresX = 5445;
        private const int GreenAcresY = 1153;
        private const int GreenAcresZ = 0;

        public static void Initialize()
        {
            CommandSystem.Register("VTKGump", AccessLevel.GameMaster, VTKGump_OnCommand);
            CommandSystem.Register("VTKG", AccessLevel.GameMaster, VTKGump_OnCommand);
        }

        [Usage("VTKGump")]
        [Description("Opens the Vystia Test Kit gump for selective item spawning")]
        private static void VTKGump_OnCommand(CommandEventArgs e)
        {
            PlayerMobile pm = e.Mobile as PlayerMobile;
            if (pm == null)
                return;

            pm.SendGump(new VystiaTestKitGump(pm, false));
        }

        public VystiaTestKitGump(PlayerMobile player, bool spawnAtPlayer) : base(50, 50)
        {
            m_Player = player;
            m_SpawnAtPlayer = spawnAtPlayer;

            Closable = true;
            Disposable = true;
            Dragable = true;
            Resizable = false;

            AddPage(0);

            // Main background (30% larger: 450->585, 550->715, but increased height for new sections)
            const int GumpWidth = 585;   // 450 * 1.3 = 585
            const int GumpHeight = 900;  // Increased to fit all skill/school buttons
            AddBackground(0, 0, GumpWidth, GumpHeight, 9200);
            AddAlphaRegion(10, 10, GumpWidth - 20, GumpHeight - 20);

            // Title
            AddLabel(150, 15, 0x480, "Vystia Test Kit");
            AddLabel(140, 35, 0x3B2, "GM Testing Interface");

            // Location toggle
            AddLabel(20, 60, 0, "Spawn Location:");
            AddButton(140, 60, m_SpawnAtPlayer ? 9027 : 9026, m_SpawnAtPlayer ? 9027 : 9026, 1, GumpButtonType.Reply, 0);
            AddLabel(160, 60, m_SpawnAtPlayer ? 0x35 : 0x3B2, m_SpawnAtPlayer ? "At Player" : "Green Acres");

            // Divider (scaled to new width)
            AddImageTiled(20, 85, GumpWidth - 40, 2, 9151);

            int y = 95;

            // === VYSTIA CRAFTING SKILLS ===
            AddLabel(20, y, 0x480, "Vystia Crafting Skills (Tool + Components + Station):");
            y += 25;

            // Crafting Row 1
            AddButton(20, y, 4005, 4007, (int)VystiaSkillChestSpawner.CraftingSkill.Engineering + VystiaSkillChestSpawner.CRAFTING_START, GumpButtonType.Reply, 0);
            AddLabel(55, y, 0x3B2, "Engineering");
            AddButton(170, y, 4005, 4007, (int)VystiaSkillChestSpawner.CraftingSkill.Transmutation + VystiaSkillChestSpawner.CRAFTING_START, GumpButtonType.Reply, 0);
            AddLabel(205, y, 0x3B2, "Transmutation");
            AddButton(320, y, 4005, 4007, (int)VystiaSkillChestSpawner.CraftingSkill.Clothcraft + VystiaSkillChestSpawner.CRAFTING_START, GumpButtonType.Reply, 0);
            AddLabel(355, y, 0x3B2, "Clothcraft");
            y += 25;

            // Crafting Row 2
            AddButton(20, y, 4005, 4007, (int)VystiaSkillChestSpawner.CraftingSkill.Leathercraft + VystiaSkillChestSpawner.CRAFTING_START, GumpButtonType.Reply, 0);
            AddLabel(55, y, 0x3B2, "Leathercraft");
            AddButton(170, y, 4005, 4007, (int)VystiaSkillChestSpawner.CraftingSkill.Woodshaping + VystiaSkillChestSpawner.CRAFTING_START, GumpButtonType.Reply, 0);
            AddLabel(205, y, 0x3B2, "Woodshaping");
            AddButton(320, y, 4005, 4007, (int)VystiaSkillChestSpawner.CraftingSkill.VystiaInscription + VystiaSkillChestSpawner.CRAFTING_START, GumpButtonType.Reply, 0);
            AddLabel(355, y, 0x3B2, "Vystia Inscription");
            y += 25;

            // Crafting Row 3
            AddButton(20, y, 4005, 4007, (int)VystiaSkillChestSpawner.CraftingSkill.Jewelcraft + VystiaSkillChestSpawner.CRAFTING_START, GumpButtonType.Reply, 0);
            AddLabel(55, y, 0x3B2, "Jewelcraft");
            AddButton(170, y, 4005, 4007, (int)VystiaSkillChestSpawner.CraftingSkill.Necrocraft + VystiaSkillChestSpawner.CRAFTING_START, GumpButtonType.Reply, 0);
            AddLabel(205, y, 0x3B2, "Necrocraft");
            AddButton(320, y, 4005, 4007, (int)VystiaSkillChestSpawner.CraftingSkill.Runecrafting + VystiaSkillChestSpawner.CRAFTING_START, GumpButtonType.Reply, 0);
            AddLabel(355, y, 0x3B2, "Runecrafting");
            y += 35;

            // Divider
            AddImageTiled(20, y, GumpWidth - 40, 2, 9151);
            y += 10;

            // === MAGIC SCHOOLS ===
            AddLabel(20, y, 0x480, "Magic Schools (Spellbook + 500 Reagents Each):");
            y += 25;

            // Magic Row 1
            AddButton(20, y, 4005, 4007, (int)AbilitySchool.Ice + VystiaSkillChestSpawner.MAGIC_START, GumpButtonType.Reply, 0);
            AddLabel(55, y, 0x481, "Ice Magic");
            AddButton(170, y, 4005, 4007, (int)AbilitySchool.Nature + VystiaSkillChestSpawner.MAGIC_START, GumpButtonType.Reply, 0);
            AddLabel(205, y, 0x7D6, "Nature Magic");
            AddButton(320, y, 4005, 4007, (int)AbilitySchool.Hex + VystiaSkillChestSpawner.MAGIC_START, GumpButtonType.Reply, 0);
            AddLabel(355, y, 0x81D, "Hex Magic");
            y += 25;

            // Magic Row 2
            AddButton(20, y, 4005, 4007, (int)AbilitySchool.Elemental + VystiaSkillChestSpawner.MAGIC_START, GumpButtonType.Reply, 0);
            AddLabel(55, y, 0x54E, "Elemental");
            AddButton(170, y, 4005, 4007, (int)AbilitySchool.Dark + VystiaSkillChestSpawner.MAGIC_START, GumpButtonType.Reply, 0);
            AddLabel(205, y, 0x455, "Dark Magic");
            AddButton(320, y, 4005, 4007, (int)AbilitySchool.Divination + VystiaSkillChestSpawner.MAGIC_START, GumpButtonType.Reply, 0);
            AddLabel(355, y, 0x482, "Divination");
            y += 25;

            // Magic Row 3
            AddButton(20, y, 4005, 4007, (int)AbilitySchool.Necromancy + VystiaSkillChestSpawner.MAGIC_START, GumpButtonType.Reply, 0);
            AddLabel(55, y, 0x455, "Necromancy");
            AddButton(170, y, 4005, 4007, (int)AbilitySchool.Summoning + VystiaSkillChestSpawner.MAGIC_START, GumpButtonType.Reply, 0);
            AddLabel(205, y, 0x3B2, "Summoning");
            AddButton(320, y, 4005, 4007, (int)AbilitySchool.Shamanic + VystiaSkillChestSpawner.MAGIC_START, GumpButtonType.Reply, 0);
            AddLabel(355, y, 0x7D6, "Shamanic");
            y += 25;

            // Magic Row 4
            AddButton(20, y, 4005, 4007, (int)AbilitySchool.Bardic + VystiaSkillChestSpawner.MAGIC_START, GumpButtonType.Reply, 0);
#if VYSTIA_SONGWEAVING
            AddLabel(55, y, 0x3B2, "Songweaving");
#endif
            AddButton(170, y, 4005, 4007, (int)AbilitySchool.Enchanting + VystiaSkillChestSpawner.MAGIC_START, GumpButtonType.Reply, 0);
            AddLabel(205, y, 0x3B2, "Enchanting");
            AddButton(320, y, 4005, 4007, (int)AbilitySchool.Illusion + VystiaSkillChestSpawner.MAGIC_START, GumpButtonType.Reply, 0);
            AddLabel(355, y, 0x3B2, "Illusion");
            y += 35;

            // Divider
            AddImageTiled(20, y, GumpWidth - 40, 2, 9151);
            y += 10;

            // === MARTIAL SCHOOLS ===
            AddLabel(20, y, 0x480, "Martial Schools (Weapon + Ability Book):");
            y += 25;

            // Martial Row 1
            AddButton(20, y, 4005, 4007, (int)AbilitySchool.Fighter + VystiaSkillChestSpawner.MARTIAL_START, GumpButtonType.Reply, 0);
            AddLabel(55, y, 0x3B2, "Fighter");
            AddButton(170, y, 4005, 4007, (int)AbilitySchool.Barbarian + VystiaSkillChestSpawner.MARTIAL_START, GumpButtonType.Reply, 0);
            AddLabel(205, y, 0x26, "Barbarian");
            AddButton(320, y, 4005, 4007, (int)AbilitySchool.Monk + VystiaSkillChestSpawner.MARTIAL_START, GumpButtonType.Reply, 0);
            AddLabel(355, y, 0x3B2, "Monk");
            y += 25;

            // Martial Row 2
            AddButton(20, y, 4005, 4007, (int)AbilitySchool.Rogue + VystiaSkillChestSpawner.MARTIAL_START, GumpButtonType.Reply, 0);
            AddLabel(55, y, 0x3B2, "Rogue");
            AddButton(170, y, 4005, 4007, (int)AbilitySchool.Ranger + VystiaSkillChestSpawner.MARTIAL_START, GumpButtonType.Reply, 0);
            AddLabel(205, y, 0x3B2, "Ranger");
            AddButton(320, y, 4005, 4007, (int)AbilitySchool.Knight + VystiaSkillChestSpawner.MARTIAL_START, GumpButtonType.Reply, 0);
            AddLabel(355, y, 0x3B2, "Knight");
            y += 25;

            // Martial Row 3
            AddButton(20, y, 4005, 4007, (int)AbilitySchool.Paladin + VystiaSkillChestSpawner.MARTIAL_START, GumpButtonType.Reply, 0);
            AddLabel(55, y, 0x3B2, "Paladin");
            AddButton(170, y, 4005, 4007, (int)AbilitySchool.Templar + VystiaSkillChestSpawner.MARTIAL_START, GumpButtonType.Reply, 0);
            AddLabel(205, y, 0x3B2, "Templar");
            AddButton(320, y, 4005, 4007, (int)AbilitySchool.BountyHunter + VystiaSkillChestSpawner.MARTIAL_START, GumpButtonType.Reply, 0);
            AddLabel(355, y, 0x3B2, "Bounty Hunter");
            y += 25;

            // Martial Row 4
            AddButton(20, y, 4005, 4007, (int)AbilitySchool.Beastmaster + VystiaSkillChestSpawner.MARTIAL_START, GumpButtonType.Reply, 0);
            AddLabel(55, y, 0x7D6, "Beastmaster");
            AddButton(170, y, 4005, 4007, (int)AbilitySchool.Artificer + VystiaSkillChestSpawner.MARTIAL_START, GumpButtonType.Reply, 0);
            AddLabel(205, y, 0x3B2, "Artificer");
            AddButton(320, y, 4005, 4007, (int)AbilitySchool.Alchemist + VystiaSkillChestSpawner.MARTIAL_START, GumpButtonType.Reply, 0);
            AddLabel(355, y, 0x3B2, "Alchemist");
            y += 25;

            // Martial Row 5
            AddButton(20, y, 4005, 4007, (int)AbilitySchool.Cleric + VystiaSkillChestSpawner.MARTIAL_START, GumpButtonType.Reply, 0);
            AddLabel(55, y, 0x3B2, "Cleric");
            AddButton(170, y, 4005, 4007, (int)AbilitySchool.Wizard + VystiaSkillChestSpawner.MARTIAL_START, GumpButtonType.Reply, 0);
            AddLabel(205, y, 0x3B2, "Wizard");
            y += 35;

            // Divider
            AddImageTiled(20, y, GumpWidth - 40, 2, 9151);
            y += 10;

            // === BULK SPAWN OPTIONS ===
            AddLabel(20, y, 0x480, "Bulk Spawn:");
            y += 25;

            AddButton(20, y, 4005, 4007, 200, GumpButtonType.Reply, 0);
            AddLabel(55, y, 0x35, "Spawn ALL Tables (12 tables)");
            y += 25;

            AddButton(20, y, 4005, 4007, 201, GumpButtonType.Reply, 0);
            AddLabel(55, y, 0, "Spawn Test Creatures Only");
            y += 25;

            AddButton(20, y, 4005, 4007, 202, GumpButtonType.Reply, 0);
            AddLabel(55, y, 0, "Spawn Materials Only");
            y += 35;

            // Divider
            AddImageTiled(20, y, GumpWidth - 40, 2, 9151);
            y += 10;

            // === TO BACKPACK OPTIONS ===
            AddLabel(20, y, 0x480, "Add to Backpack:");
            y += 25;

            AddButton(20, y, 4005, 4007, 300, GumpButtonType.Reply, 0);
            AddLabel(55, y, 0, "Full Test Kit (VTK backpack version)");
            y += 25;

            AddButton(20, y, 4005, 4007, 301, GumpButtonType.Reply, 0);
            AddLabel(55, y, 0, "All Spawn Stones");
            y += 25;

            AddButton(20, y, 4005, 4007, 302, GumpButtonType.Reply, 0);
            AddLabel(55, y, 0, "Combat Dummy Stones");
            y += 35;

            // Divider
            AddImageTiled(20, y, GumpWidth - 40, 2, 9151);
            y += 10;

            // === CLEANUP OPTIONS ===
            AddLabel(20, y, 0x480, "Cleanup:");
            y += 25;

            AddButton(20, y, 4005, 4007, 400, GumpButtonType.Reply, 0);
            AddLabel(55, y, 0x22, "Clean Green Acres (remove VTK items)");
            y += 25;

            AddButton(20, y, 4005, 4007, 401, GumpButtonType.Reply, 0);
            AddLabel(55, y, 0x22, "Clean Area Around Me (20 tiles)");
            y += 35;

            // Divider
            AddImageTiled(20, y, GumpWidth - 40, 2, 9151);
            y += 10;

            // === TELEPORT OPTIONS ===
            AddLabel(20, y, 0x480, "Teleport:");
            y += 25;

            AddButton(20, y, 4005, 4007, 500, GumpButtonType.Reply, 0);
            AddLabel(55, y, 0, "Go to Green Acres");
            y += 25;

            // Info
            AddLabel(20, y + 10, 0x3B2, $"Green Acres: {GreenAcresX}, {GreenAcresY}, Felucca");
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            if (m_Player == null || m_Player.Deleted)
                return;

            int buttonId = info.ButtonID;

            // Toggle location
            if (buttonId == 1)
            {
                m_Player.SendGump(new VystiaTestKitGump(m_Player, !m_SpawnAtPlayer));
                return;
            }

            // Get spawn location
            Point3D spawnLoc = m_SpawnAtPlayer ? m_Player.Location : new Point3D(GreenAcresX, GreenAcresY, GreenAcresZ);
            Map spawnMap = m_SpawnAtPlayer ? m_Player.Map : Map.Felucca;

            // Special case: Combat Dummies button (111) opens config gump
            if (buttonId == 111)
            {
                m_Player.SendGump(new CombatDummyConfigGump(m_Player, false, CombatDummyMode.Hybrid, PlayerClassTypeV2.None, VystiaFaction.None, VystiaReligion.None, CombatDummyMode.Hybrid, PlayerClassTypeV2.None, PlayerClassTypeV2.None));
                return;
            }

            // Bulk spawn options (200-202)
            if (buttonId == 200) // Spawn ALL Tables
            {
                SpawnAllTables(spawnLoc, spawnMap);
                m_Player.SendMessage(0x35, "Spawned all 12 category tables!");
                m_Player.SendGump(new VystiaTestKitGump(m_Player, m_SpawnAtPlayer));
                return;
            }
            else if (buttonId == 201) // Spawn Test Creatures Only
            {
                // Spawn categories 2 and 3 (Ancients and Bosses)
                SpawnCategory(2, spawnLoc, spawnMap);
                SpawnCategory(3, new Point3D(spawnLoc.X + 5, spawnLoc.Y, spawnLoc.Z), spawnMap);
                m_Player.SendMessage(0x35, "Spawned test creature stones!");
                m_Player.SendGump(new VystiaTestKitGump(m_Player, m_SpawnAtPlayer));
                return;
            }
            else if (buttonId == 202) // Spawn Materials Only
            {
                // Spawn categories 6-9 (Materials)
                SpawnCategory(6, spawnLoc, spawnMap);
                SpawnCategory(7, new Point3D(spawnLoc.X + 5, spawnLoc.Y, spawnLoc.Z), spawnMap);
                SpawnCategory(8, new Point3D(spawnLoc.X + 10, spawnLoc.Y, spawnLoc.Z), spawnMap);
                SpawnCategory(9, new Point3D(spawnLoc.X + 15, spawnLoc.Y, spawnLoc.Z), spawnMap);
                m_Player.SendMessage(0x35, "Spawned material tables!");
                m_Player.SendGump(new VystiaTestKitGump(m_Player, m_SpawnAtPlayer));
                return;
            }

            // Crafting Skills (600-608)
            if (buttonId >= VystiaSkillChestSpawner.CRAFTING_START && buttonId < VystiaSkillChestSpawner.MAGIC_START)
            {
                VystiaSkillChestSpawner.CraftingSkill skill = (VystiaSkillChestSpawner.CraftingSkill)(buttonId - VystiaSkillChestSpawner.CRAFTING_START);
                Point3D chestLoc = m_SpawnAtPlayer ? GetSpawnLocationInFront(m_Player, 2) : spawnLoc;
                Map chestMap = m_SpawnAtPlayer ? m_Player.Map : spawnMap;
                
                VystiaSkillChestSpawner.SpawnCraftingSkillChest(skill, chestLoc, chestMap);
                m_Player.SendMessage(0x35, "Spawned {0} chest!", VystiaSkillChestSpawner.GetCraftingSkillName(skill));
                m_Player.SendGump(new VystiaTestKitGump(m_Player, m_SpawnAtPlayer));
                return;
            }

            // Magic Schools (300-311)
            if (buttonId >= VystiaSkillChestSpawner.MAGIC_START && buttonId < VystiaSkillChestSpawner.MARTIAL_START)
            {
                AbilitySchool school = (AbilitySchool)(buttonId - VystiaSkillChestSpawner.MAGIC_START);
                Point3D chestLoc = m_SpawnAtPlayer ? GetSpawnLocationInFront(m_Player, 2) : spawnLoc;
                Map chestMap = m_SpawnAtPlayer ? m_Player.Map : spawnMap;
                
                VystiaSkillChestSpawner.SpawnMagicSchoolChest(school, chestLoc, chestMap);
                m_Player.SendMessage(0x35, "Spawned {0} chest!", VystiaSkillChestSpawner.GetMagicSchoolName(school));
                m_Player.SendGump(new VystiaTestKitGump(m_Player, m_SpawnAtPlayer));
                return;
            }

            // Martial Schools (800-813)
            if (buttonId >= VystiaSkillChestSpawner.MARTIAL_START && buttonId < 900)
            {
                AbilitySchool school = (AbilitySchool)(buttonId - VystiaSkillChestSpawner.MARTIAL_START);
                Point3D chestLoc = m_SpawnAtPlayer ? GetSpawnLocationInFront(m_Player, 2) : spawnLoc;
                Map chestMap = m_SpawnAtPlayer ? m_Player.Map : spawnMap;
                
                VystiaSkillChestSpawner.SpawnMartialSchoolChest(school, chestLoc, chestMap);
                m_Player.SendMessage(0x35, "Spawned {0} chest!", VystiaSkillChestSpawner.GetMartialSchoolName(school));
                m_Player.SendGump(new VystiaTestKitGump(m_Player, m_SpawnAtPlayer));
                return;
            }

            // Legacy categories (100-110) - spawn in front of player with offset based on category
            if (buttonId >= 100 && buttonId <= 110)
            {
                int category = buttonId - 100;

                // Calculate spawn position in front of player
                Point3D individualLoc = GetSpawnLocationInFront(m_Player, 2);

                SpawnCategory(category, individualLoc, m_Player.Map);
                m_Player.SendGump(new VystiaTestKitGump(m_Player, m_SpawnAtPlayer));
                return;
            }

            // Backpack options (300-302)
            switch (buttonId)
            {
                case 300: // Full test kit
                    GiveFullTestKit();
                    break;
                case 301: // All spawn stones
                    GiveAllSpawnStones();
                    break;
                case 302: // Combat dummy stones
                    GiveCombatDummyStones();
                    break;
            }

            // Cleanup options (400-401)
            switch (buttonId)
            {
                case 400: // Clean Green Acres
                    CleanupArea(new Point3D(GreenAcresX, GreenAcresY, GreenAcresZ), Map.Felucca, 50);
                    break;
                case 401: // Clean around player
                    CleanupArea(m_Player.Location, m_Player.Map, 20);
                    break;
            }

            // Teleport options (500)
            if (buttonId == 500)
            {
                m_Player.MoveToWorld(new Point3D(GreenAcresX, GreenAcresY, GreenAcresZ), Map.Felucca);
                m_Player.SendMessage(0x35, "Teleported to Green Acres.");
            }

            // Reopen gump for most actions
            if (buttonId >= 200)
            {
                m_Player.SendGump(new VystiaTestKitGump(m_Player, m_SpawnAtPlayer));
            }
        }

        private void SpawnCategory(int category, Point3D loc, Map map)
        {
            Container chest = CreateTableWithChest(map, loc.X, loc.Y, loc.Z, GetCategoryName(category), GetCategoryHue(category));

            switch (category)
            {
                case 0: // Religion Shrines
                    for (int i = 1; i <= 6; i++)
                        chest.DropItem(new VystiaShrineStone((VystiaReligion)i));
                    break;

                case 1: // Faction Stones
                    for (int i = 1; i <= 7; i++)
                        chest.DropItem(new VystiaFactionStone((VystiaFaction)i));
                    break;

                case 2: // Ancient Beings
                    foreach (VystiaAncientStone.AncientType t in Enum.GetValues(typeof(VystiaAncientStone.AncientType)))
                        chest.DropItem(new VystiaAncientStone(t));
                    break;

                case 3: // Bosses
                    foreach (VystiaBossStone.BossType t in Enum.GetValues(typeof(VystiaBossStone.BossType)))
                        chest.DropItem(new VystiaBossStone(t));
                    break;

                case 4: // Crafting Stations
                    chest.DropItem(new VystiaCraftingStone("Forge", 0xFB1, 0x3B2));
                    chest.DropItem(new VystiaCraftingStone("Anvil", 0xFAF, 0x3B2));
                    chest.DropItem(new VystiaCraftingStone("Carpentry Bench", 0x1E7F, 0x3B2));
                    chest.DropItem(new VystiaCraftingStone("Alchemy Table", 0x1E9D, 0x3B2));
                    chest.DropItem(new VystiaCraftingStone("Tailoring Table", 0xF9C, 0x3B2));
                    chest.DropItem(new VystiaCraftingStone("Tinkering Table", 0x1EB9, 0x3B2));
                    break;

                case 5: // Faction Vendors
                    chest.DropItem(new VystiaVendorStone("Frostguard Vendor", VystiaFaction.Frostguard));
                    chest.DropItem(new VystiaVendorStone("Flame Legion Vendor", VystiaFaction.FlameLegion));
                    chest.DropItem(new VystiaVendorStone("Greenward Vendor", VystiaFaction.Greenward));
                    chest.DropItem(new VystiaVendorStone("Arcane Conclave Vendor", VystiaFaction.ArcaneConclave));
                    chest.DropItem(new VystiaVendorStone("Technoguild Vendor", VystiaFaction.Technoguild));
                    chest.DropItem(new VystiaVendorStone("Sandwalkers Vendor", VystiaFaction.Sandwalkers));
                    chest.DropItem(new VystiaVendorStone("Voidborn Vendor", VystiaFaction.Voidborn));
                    break;

                case 6: // Ingots
                    chest.DropItem(new FrostforgedIngot(100));
                    chest.DropItem(new EmberforgedIngot(100));
                    chest.DropItem(new CrystallineIngot(100));
                    chest.DropItem(new ClockworkIngot(100));
                    chest.DropItem(new VoidforgedIngot(100));
                    chest.DropItem(new ShadowforgedIngot(100));
                    chest.DropItem(new SunforgedIngot(100));
                    chest.DropItem(new NatureforgedIngot(100));
                    break;

                case 7: // Ores
                    chest.DropItem(new FrozenOre(100));
                    chest.DropItem(new MoltenOre(100));
                    chest.DropItem(new CrystalOre(100));
                    chest.DropItem(new SteamworkOre(100));
                    chest.DropItem(new ObsidianOre(100));
                    chest.DropItem(new BogIronOre(100));
                    chest.DropItem(new SandstoneOre(100));
                    chest.DropItem(new LivingOre(100));
                    break;

                case 8: // Reagents
                    chest.DropItem(new BlackPearl(100));
                    chest.DropItem(new Bloodmoss(100));
                    chest.DropItem(new Garlic(100));
                    chest.DropItem(new Ginseng(100));
                    chest.DropItem(new MandrakeRoot(100));
                    chest.DropItem(new Nightshade(100));
                    chest.DropItem(new SpidersSilk(100));
                    chest.DropItem(new SulfurousAsh(100));
                    break;

                case 9: // Basic Materials
                    chest.DropItem(new IronIngot(100));
                    chest.DropItem(new Leather(100));
                    chest.DropItem(new Board(100));
                    chest.DropItem(new Cloth(100));
                    break;

                case 10: // Crafting Tools (Vystia Regional Variants)
                    // Smith Hammers - Regional variants
                    chest.DropItem(new FrostforgedSmithHammer());
                    chest.DropItem(new EmberforgedSmithHammer());
                    chest.DropItem(new SteamforgedSmithHammer());
                    // Tinker Tools - Regional variants
                    chest.DropItem(new FrostforgedTinkerTools());
                    chest.DropItem(new ClockworkTinkerTools());
                    // Sewing Kits - Regional variants
                    chest.DropItem(new LivingVineSewingKit());
                    chest.DropItem(new SandsilkSewingKit());
                    // Carpenter Tools - Regional variants
                    chest.DropItem(new LivingwoodSaw());
                    chest.DropItem(new FrostwillowSaw());
                    // Alchemy Tools - Regional variants
                    chest.DropItem(new CrystalMortarPestle());
                    chest.DropItem(new HexedMortarPestle());
                    // Inscription Tools - Regional variants
                    chest.DropItem(new OracleScribePen());
                    chest.DropItem(new EnchantedScribePen());
                    // Cooking Tools - Regional variants
                    chest.DropItem(new EmberforgedSkillet());
                    chest.DropItem(new FrostforgedSkillet());
                    // Tailoring Scissors - Regional variants
                    chest.DropItem(new CrystalScissors());
                    chest.DropItem(new VoidtouchedScissors());
                    break;

                case 11: // Combat Dummies
                    chest.DropItem(new VystiaCombatDummyStone(VystiaFaction.None, VystiaReligion.None, CombatDummyMode.Passive));
                    chest.DropItem(new VystiaCombatDummyStone(VystiaFaction.None, VystiaReligion.None, CombatDummyMode.Melee));
                    chest.DropItem(new VystiaCombatDummyStone(VystiaFaction.None, VystiaReligion.None, CombatDummyMode.Caster));
                    chest.DropItem(new VystiaCombatDummyStone(VystiaFaction.None, VystiaReligion.None, CombatDummyMode.Hybrid));
                    chest.DropItem(new VystiaCombatDummyStone(VystiaFaction.Frostguard, VystiaReligion.None, CombatDummyMode.Hybrid));
                    chest.DropItem(new VystiaCombatDummyStone(VystiaFaction.FlameLegion, VystiaReligion.None, CombatDummyMode.Hybrid));
                    chest.DropItem(new VystiaCombatDummyStone(VystiaFaction.Voidborn, VystiaReligion.None, CombatDummyMode.Caster));
                    chest.DropItem(new VystiaCombatDummyStone(VystiaFaction.None, VystiaReligion.FrosthelmFaith, CombatDummyMode.Hybrid));
                    chest.DropItem(new VystiaCombatDummyStone(VystiaFaction.None, VystiaReligion.SuryasSandscript, CombatDummyMode.Melee));
                    chest.DropItem(new VystiaCombatDummyStone(VystiaFaction.None, VystiaReligion.OceanasCovenant, CombatDummyMode.Caster));
                    break;
            }

            m_Player.SendMessage(0x35, "Spawned: {0}", GetCategoryName(category));
        }

        private string GetCategoryName(int category)
        {
            switch (category)
            {
                case 0: return "Religion Shrine Stones";
                case 1: return "Faction Stones";
                case 2: return "Ancient Being Spawn Stones";
                case 3: return "Boss Spawn Stones";
                case 4: return "Crafting Station Stones";
                case 5: return "Vendor Spawn Stones";
                case 6: return "Vystia Ingots (100 each)";
                case 7: return "Vystia Ores (100 each)";
                case 8: return "Magic Reagents (100 each)";
                case 9: return "Basic Materials (100 each)";
                case 10: return "Crafting Tools";
                case 11: return "Combat Dummy Stones";
                default: return "Unknown";
            }
        }

        private int GetCategoryHue(int category)
        {
            switch (category)
            {
                case 2: return 0x35;  // Ancients - gold
                case 3: return 0x22;  // Bosses - red
                case 11: return 0x455; // Combat dummies
                default: return 0x3B2; // Default gold
            }
        }

        private void SpawnAllTables(Point3D baseLoc, Map map)
        {
            int spacing = 5;

            // If spawning at player, offset to spawn in front
            Point3D startLoc = baseLoc;
            if (m_SpawnAtPlayer)
            {
                startLoc = GetSpawnLocationInFront(m_Player, 2);
            }

            int x = startLoc.X;
            int y = startLoc.Y;
            int z = startLoc.Z;

            // Row 1
            SpawnCategory(0, new Point3D(x, y, z), map);
            SpawnCategory(1, new Point3D(x + spacing, y, z), map);
            SpawnCategory(2, new Point3D(x + spacing * 2, y, z), map);
            SpawnCategory(3, new Point3D(x + spacing * 3, y, z), map);

            // Row 2
            SpawnCategory(4, new Point3D(x, y + spacing, z), map);
            SpawnCategory(5, new Point3D(x + spacing, y + spacing, z), map);
            SpawnCategory(6, new Point3D(x + spacing * 2, y + spacing, z), map);
            SpawnCategory(7, new Point3D(x + spacing * 3, y + spacing, z), map);

            // Row 3
            SpawnCategory(8, new Point3D(x, y + spacing * 2, z), map);
            SpawnCategory(9, new Point3D(x + spacing, y + spacing * 2, z), map);
            SpawnCategory(10, new Point3D(x + spacing * 2, y + spacing * 2, z), map);
            SpawnCategory(11, new Point3D(x + spacing * 3, y + spacing * 2, z), map);
        }

        private void GiveFullTestKit()
        {
            if (m_Player.Backpack == null)
                return;

            Container testKit = new Backpack();
            testKit.Name = "Vystia Test Kit";
            testKit.Hue = 0x3B2;

            // Religion stones
            for (int i = 1; i <= 6; i++)
                testKit.DropItem(new VystiaShrineStone((VystiaReligion)i));

            // Faction stones
            for (int i = 1; i <= 7; i++)
                testKit.DropItem(new VystiaFactionStone((VystiaFaction)i));

            // Materials
            testKit.DropItem(new FrostforgedIngot(100));
            testKit.DropItem(new EmberforgedIngot(100));
            testKit.DropItem(new IronIngot(100));
            testKit.DropItem(new Leather(100));
            testKit.DropItem(new Board(100));

            // Reagents
            testKit.DropItem(new BlackPearl(100));
            testKit.DropItem(new Bloodmoss(100));
            testKit.DropItem(new SulfurousAsh(100));

            // Tools
            testKit.DropItem(new SmithHammer());
            testKit.DropItem(new TinkerTools());

            // Bard testing kit
#if VYSTIA_SONGWEAVING
            testKit.DropItem(new SongweavingSpellbook());
            testKit.DropItem(new MagicLute());
#endif
#if VYSTIA_CRESCENDO
            testKit.DropItem(new CrescendoCrystal(5));
            testKit.DropItem(new CrescendoCatalyst(2));
#endif

            m_Player.Backpack.DropItem(testKit);
            m_Player.SendMessage(0x35, "Full test kit added to your backpack!");
        }

        private void GiveAllSpawnStones()
        {
            if (m_Player.Backpack == null)
                return;

            // Ancient stones
            foreach (VystiaAncientStone.AncientType t in Enum.GetValues(typeof(VystiaAncientStone.AncientType)))
                m_Player.Backpack.DropItem(new VystiaAncientStone(t));

            // Boss stones
            foreach (VystiaBossStone.BossType t in Enum.GetValues(typeof(VystiaBossStone.BossType)))
                m_Player.Backpack.DropItem(new VystiaBossStone(t));

            m_Player.SendMessage(0x35, "All creature spawn stones added to your backpack!");
        }

        private void GiveCombatDummyStones()
        {
            if (m_Player.Backpack == null)
                return;

            foreach (CombatDummyMode mode in Enum.GetValues(typeof(CombatDummyMode)))
                m_Player.Backpack.DropItem(new VystiaCombatDummyStone(VystiaFaction.None, VystiaReligion.None, mode));

            m_Player.Backpack.DropItem(new VystiaCombatDummyStone(VystiaFaction.Frostguard, VystiaReligion.None, CombatDummyMode.Hybrid));
            m_Player.Backpack.DropItem(new VystiaCombatDummyStone(VystiaFaction.FlameLegion, VystiaReligion.None, CombatDummyMode.Hybrid));
            m_Player.Backpack.DropItem(new VystiaCombatDummyStone(VystiaFaction.None, VystiaReligion.FrosthelmFaith, CombatDummyMode.Hybrid));

            m_Player.SendMessage(0x35, "Combat dummy stones added to your backpack!");
        }

        private void CleanupArea(Point3D center, Map map, int radius)
        {
            List<Item> toDelete = new List<Item>();

            foreach (Item item in World.Items.Values)
            {
                try
                {
                    if (item == null || item.Deleted || item.Map != map)
                        continue;

                    Point3D loc = item.GetWorldLocation();
                    if (Math.Abs(loc.X - center.X) > radius || Math.Abs(loc.Y - center.Y) > radius)
                        continue;

                    bool shouldDelete = false;

                    // VTK tables
                    if (item.Name != null && item.Name.StartsWith("VTK Table:"))
                        shouldDelete = true;
                    // Mini houses (legacy)
                    else if (item is MiniHouseAddon)
                        shouldDelete = true;
                    // Named chests
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
                    // Vystia test items
                    else if (item is VystiaShrineStone || item is VystiaFactionStone ||
                             item is VystiaAncientStone || item is VystiaBossStone ||
                             item is VystiaCraftingStone || item is VystiaVendorStone ||
                             item is VystiaCombatDummyStone)
                    {
                        shouldDelete = true;
                    }

                    if (shouldDelete)
                        toDelete.Add(item);
                }
                catch { }
            }

            // Also cleanup combat dummies (mobiles)
            List<Mobile> mobilesToDelete = new List<Mobile>();
            foreach (Mobile m in World.Mobiles.Values)
            {
                try
                {
                    if (m == null || m.Deleted || m.Map != map)
                        continue;

                    if (Math.Abs(m.X - center.X) > radius || Math.Abs(m.Y - center.Y) > radius)
                        continue;

                    if (m is VystiaCombatDummy)
                        mobilesToDelete.Add(m);
                }
                catch { }
            }

            int itemCount = 0;
            int mobileCount = 0;

            foreach (Item item in toDelete)
            {
                try
                {
                    item.Delete();
                    itemCount++;
                }
                catch { }
            }

            foreach (Mobile m in mobilesToDelete)
            {
                try
                {
                    m.Delete();
                    mobileCount++;
                }
                catch { }
            }

            m_Player.SendMessage(0x35, "Cleanup complete! Deleted {0} items and {1} mobiles.", itemCount, mobileCount);
        }

        private static Container CreateTableWithChest(Map map, int x, int y, int z, string chestName, int chestHue)
        {
            Item table = new Item(0x0B8F);
            table.Name = "VTK Table: " + chestName;
            table.Movable = false;
            table.MoveToWorld(new Point3D(x, y, z), map);

            Container chest = new WoodenChest();
            chest.Name = chestName;
            chest.Hue = chestHue;
            chest.Movable = false;
            chest.MoveToWorld(new Point3D(x, y, z + 6), map);

            return chest;
        }

        private static Point3D GetSpawnLocationInFront(Mobile m, int distance)
        {
            if (m == null || m.Map == null)
                return Point3D.Zero;

            int offsetX = 0, offsetY = 0;
            switch (m.Direction & Direction.Mask)
            {
                case Direction.North: offsetY = -distance; break;
                case Direction.South: offsetY = distance; break;
                case Direction.East: offsetX = distance; break;
                case Direction.West: offsetX = -distance; break;
                case Direction.Up: offsetX = -distance; offsetY = -distance; break;
                case Direction.Down: offsetX = distance; offsetY = distance; break;
                case Direction.Left: offsetX = -distance; offsetY = distance; break;
                case Direction.Right: offsetX = distance; offsetY = -distance; break;
            }

            Point3D loc = new Point3D(m.X + offsetX, m.Y + offsetY, m.Z);

            // Try to find valid Z at location
            int z = m.Map.GetAverageZ(loc.X, loc.Y);
            if (m.Map.CanFit(loc.X, loc.Y, z, 16, false, false))
                loc.Z = z;

            return loc;
        }
    }
}

