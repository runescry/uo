using System;
using System.Collections.Generic;
using Server;
using Server.Commands;
using Server.Gumps;
using Server.Mobiles;
using Server.Network;
using Server.Items;
using Server.Custom.VystiaClasses;
using Server.Items.VystiaClassItems;
using Server.Targeting;

namespace Server.Scripts.Commands
{
    public class VystiaGMTestingGump : Gump
    {
        private readonly Mobile m_From;
        private readonly int m_Page;

        // All 26 character classes
        private static readonly PlayerClassTypeV2[] AllClasses = new PlayerClassTypeV2[]
        {
            PlayerClassTypeV2.Barbarian, PlayerClassTypeV2.IceMage, PlayerClassTypeV2.Beastmaster,
            PlayerClassTypeV2.Sorcerer, PlayerClassTypeV2.Ranger, PlayerClassTypeV2.Illusionist,
            PlayerClassTypeV2.Witch, PlayerClassTypeV2.Warlock, PlayerClassTypeV2.Druid,
            PlayerClassTypeV2.Alchemist, PlayerClassTypeV2.Wizard, PlayerClassTypeV2.Oracle,
            PlayerClassTypeV2.Artificer, PlayerClassTypeV2.Fighter, PlayerClassTypeV2.Monk,
            PlayerClassTypeV2.Templar, PlayerClassTypeV2.Necromancer, PlayerClassTypeV2.Summoner,
            PlayerClassTypeV2.BountyHunter, PlayerClassTypeV2.Knight, PlayerClassTypeV2.Shaman,
            PlayerClassTypeV2.Cleric, PlayerClassTypeV2.Paladin, PlayerClassTypeV2.Rogue, PlayerClassTypeV2.Bard,
            PlayerClassTypeV2.Enchanter
        };

        // All 12 magic school spellbooks
        private static readonly Dictionary<string, Type> Spellbooks = new Dictionary<string, Type>
        {
            ["Ice Magic"] = typeof(IceMageSpellbook),
            ["Nature Magic"] = typeof(DruidSpellbook),
            ["Hex Magic"] = typeof(WitchSpellbook),
            ["Elemental Magic"] = typeof(SorcererSpellbook),
            ["Dark Magic"] = typeof(WarlockSpellbook),
            ["Divination"] = typeof(OracleSpellbook),
            ["Necromancy"] = typeof(VystiaNecromancerSpellbook),
            ["Summoning"] = typeof(SummonerSpellbook),
            ["Shamanic"] = typeof(ShamanSpellbook),
#if VYSTIA_SONGWEAVING
            ["Songweaving"] = typeof(SongweavingSpellbook),
#endif
            ["Enchanting"] = typeof(EnchanterSpellbook),
            ["Illusion"] = typeof(IllusionistSpellbook)
        };

        // Special class items
        private static readonly Dictionary<string, Type> SpecialItems = new Dictionary<string, Type>
        {
            ["Rage Totem"] = typeof(RageTotem),
            ["Beast Whistle"] = typeof(BeastWhistle),
            ["Alchemist Kit"] = typeof(AlchemistKit),
            ["Crystal Orb"] = typeof(CrystalOrb),
            ["Monk Beads"] = typeof(MonkBeads),
            ["Templar Cross"] = typeof(TemplarCross),
            ["Summoning Circle"] = typeof(SummoningCircle),
            ["Bounty Ledger"] = typeof(BountyLedger),
            ["Knight Banner"] = typeof(KnightBanner),
            ["Spirit Totem"] = typeof(SpiritTotem),
            ["Magic Lute"] = typeof(MagicLute),
            ["Enchanting Crystal"] = typeof(EnchantingCrystal),
            ["Construct Control Device"] = typeof(ConstructControlDevice),
            ["Clockwork Scout"] = typeof(ClockworkScout),
            ["Shapeshift Totem"] = typeof(ShapeshiftTotem),
            ["Holy Symbol"] = typeof(HolySymbol),
            ["Artificer Blueprints"] = typeof(ArtificerBlueprints)
        };

        public VystiaGMTestingGump(Mobile from, int page = 0) : base(50, 50)
        {
            m_From = from;
            m_Page = page;

            AddPage(0);

            // Background
            AddBackground(0, 0, 600, 550, 5054);
            AddAlphaRegion(10, 10, 580, 530);

            // Title
            AddHtml(20, 20, 560, 30, "<center><basefont color=#00FF00 size=7>Vystia GM Testing Tools</basefont></center>", false, false);

            // Page tabs
            AddButton(20, 60, 4005, 4007, 1000, GumpButtonType.Reply, 0); // Classes tab
            AddHtml(55, 62, 100, 20, "<basefont color=#FFFFFF>Classes</basefont>", false, false);

            AddButton(160, 60, 4005, 4007, 1001, GumpButtonType.Reply, 0); // Spellbooks tab
            AddHtml(195, 62, 100, 20, "<basefont color=#FFFFFF>Spellbooks</basefont>", false, false);

            AddButton(300, 60, 4005, 4007, 1002, GumpButtonType.Reply, 0); // Items tab
            AddHtml(335, 62, 100, 20, "<basefont color=#FFFFFF>Items</basefont>", false, false);

            AddButton(440, 60, 4005, 4007, 1003, GumpButtonType.Reply, 0); // Utilities tab
            AddHtml(475, 62, 100, 20, "<basefont color=#FFFFFF>Utilities</basefont>", false, false);

            // Content area
            switch (m_Page)
            {
                case 0: DrawClassesPage(); break;
                case 1: DrawSpellbooksPage(); break;
                case 2: DrawItemsPage(); break;
                case 3: DrawUtilitiesPage(); break;
            }

            // Close button
            AddButton(20, 510, 4017, 4019, 0, GumpButtonType.Reply, 0);
            AddHtml(55, 512, 100, 20, "<basefont color=#FF0000>Close</basefont>", false, false);
        }

        private void DrawClassesPage()
        {
            AddHtml(20, 100, 560, 30, "<basefont color=#FFFF00 size=5>Assign Character Classes</basefont>", false, false);
            AddHtml(20, 130, 560, 20, "<basefont color=#CCCCCC>Click a class to assign it to your currently targeted player</basefont>", false, false);

            int x = 30;
            int y = 160;
            int cols = 3;
            int col = 0;
            int buttonID = 2000;

            foreach (PlayerClassTypeV2 classType in AllClasses)
            {
                if (classType == PlayerClassTypeV2.None) continue;

                AddButton(x, y, 4005, 4007, buttonID++, GumpButtonType.Reply, 0);
                AddHtml(x + 35, y + 2, 150, 20, $"<basefont color=#FFFFFF>{classType}</basefont>", false, false);

                col++;
                if (col >= cols)
                {
                    col = 0;
                    y += 30;
                    x = 30;
                }
                else
                {
                    x += 190;
                }
            }
        }

        private void DrawSpellbooksPage()
        {
            AddHtml(20, 100, 560, 30, "<basefont color=#FFFF00 size=5>Give Spellbooks & Reagents</basefont>", false, false);
            AddHtml(20, 130, 560, 20, "<basefont color=#CCCCCC>Each button gives: Spellbook + All Reagents + All Scrolls</basefont>", false, false);

            int x = 30;
            int y = 160;
            int buttonID = 3000;

            foreach (var kvp in Spellbooks)
            {
                AddButton(x, y, 4005, 4007, buttonID++, GumpButtonType.Reply, 0);
                AddHtml(x + 35, y + 2, 250, 20, $"<basefont color=#00FFFF>{kvp.Key}</basefont>", false, false);

                y += 30;
            }
        }

        private void DrawItemsPage()
        {
            AddHtml(20, 100, 560, 30, "<basefont color=#FFFF00 size=5>Give Class Special Items</basefont>", false, false);
            AddHtml(20, 130, 560, 20, "<basefont color=#CCCCCC>Click to give yourself the special ability item</basefont>", false, false);

            int x = 30;
            int y = 160;
            int cols = 2;
            int col = 0;
            int buttonID = 4000;

            foreach (var kvp in SpecialItems)
            {
                AddButton(x, y, 4005, 4007, buttonID++, GumpButtonType.Reply, 0);
                AddHtml(x + 35, y + 2, 220, 20, $"<basefont color=#FFAAFF>{kvp.Key}</basefont>", false, false);

                col++;
                if (col >= cols)
                {
                    col = 0;
                    y += 30;
                    x = 30;
                }
                else
                {
                    x += 280;
                }
            }
        }

        private void DrawUtilitiesPage()
        {
            AddHtml(20, 100, 560, 30, "<basefont color=#FFFF00 size=5>Utilities & Quick Actions</basefont>", false, false);

            int y = 150;

            // Reset class
            AddButton(30, y, 4005, 4007, 5000, GumpButtonType.Reply, 0);
            AddHtml(65, y + 2, 300, 20, "<basefont color=#FF6666>Remove Class from Targeted Player</basefont>", false, false);
            y += 40;

            // Give all spellbooks
            AddButton(30, y, 4005, 4007, 5001, GumpButtonType.Reply, 0);
            AddHtml(65, y + 2, 300, 20, "<basefont color=#66FF66>Give ALL 12 Spellbooks + Reagents</basefont>", false, false);
            y += 40;

            // Max stats
            AddButton(30, y, 4005, 4007, 5002, GumpButtonType.Reply, 0);
            AddHtml(65, y + 2, 300, 20, "<basefont color=#6666FF>Max Out Targeted Player Stats</basefont>", false, false);
            y += 40;

            // Max skills
            AddButton(30, y, 4005, 4007, 5003, GumpButtonType.Reply, 0);
            AddHtml(65, y + 2, 300, 20, "<basefont color=#FFFF66>Max Out Targeted Player Skills</basefont>", false, false);
            y += 40;

            // Show class info
            AddButton(30, y, 4005, 4007, 5004, GumpButtonType.Reply, 0);
            AddHtml(65, y + 2, 300, 20, "<basefont color=#66FFFF>Show Targeted Player Class Info</basefont>", false, false);
            y += 40;

            // Give test gear
            AddButton(30, y, 4005, 4007, 5005, GumpButtonType.Reply, 0);
            AddHtml(65, y + 2, 300, 20, "<basefont color=#FF66FF>Give Test Equipment Set</basefont>", false, false);
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            Mobile from = sender.Mobile;
            int buttonID = info.ButtonID;

            if (buttonID == 0) return; // Close

            // Page navigation
            if (buttonID >= 1000 && buttonID < 1004)
            {
                from.SendGump(new VystiaGMTestingGump(from, buttonID - 1000));
                return;
            }

            // Assign class (2000+)
            if (buttonID >= 2000 && buttonID < 2000 + AllClasses.Length)
            {
                int classIndex = buttonID - 2000;
                PlayerClassTypeV2 classType = AllClasses[classIndex];

                from.SendMessage(0x35, $"Target a player to assign them the {classType} class...");
                from.Target = new ClassAssignTarget(classType);
                from.SendGump(new VystiaGMTestingGump(from, m_Page));
                return;
            }

            // Give spellbook (3000+)
            if (buttonID >= 3000 && buttonID < 3000 + Spellbooks.Count)
            {
                int index = buttonID - 3000;
                int i = 0;
                foreach (var kvp in Spellbooks)
                {
                    if (i == index)
                    {
                        GiveSpellbookPackage(from, kvp.Value, kvp.Key);
                        break;
                    }
                    i++;
                }
                from.SendGump(new VystiaGMTestingGump(from, m_Page));
                return;
            }

            // Give special item (4000+)
            if (buttonID >= 4000 && buttonID < 4000 + SpecialItems.Count)
            {
                int index = buttonID - 4000;
                int i = 0;
                foreach (var kvp in SpecialItems)
                {
                    if (i == index)
                    {
                        try
                        {
                            Item item = (Item)Activator.CreateInstance(kvp.Value);
                            from.AddToBackpack(item);
                            from.SendMessage(0x35, $"Added {kvp.Key} to your backpack!");
                        }
                        catch (Exception ex)
                        {
                            from.SendMessage(0x22, $"Error creating {kvp.Key}: {ex.Message}");
                        }
                        break;
                    }
                    i++;
                }
                from.SendGump(new VystiaGMTestingGump(from, m_Page));
                return;
            }

            // Utilities (5000+)
            switch (buttonID)
            {
                case 5000: // Remove class
                    from.SendMessage(0x35, "Target a player to remove their class...");
                    from.Target = new ClassResetTarget();
                    break;

                case 5001: // Give all spellbooks
                    GiveAllSpellbooks(from);
                    break;

                case 5002: // Max stats
                    from.SendMessage(0x35, "Target a player to max their stats...");
                    from.Target = new MaxStatsTarget();
                    break;

                case 5003: // Max skills
                    from.SendMessage(0x35, "Target a player to max their skills...");
                    from.Target = new MaxSkillsTarget();
                    break;

                case 5004: // Show class info
                    from.SendMessage(0x35, "Target a player to see their class info...");
                    from.Target = new ClassInfoTarget();
                    break;

                case 5005: // Give test gear
                    GiveTestGear(from);
                    break;
            }

            from.SendGump(new VystiaGMTestingGump(from, m_Page));
        }

        private void GiveSpellbookPackage(Mobile to, Type spellbookType, string schoolName)
        {
            try
            {
                // Give spellbook
                Item spellbook = (Item)Activator.CreateInstance(spellbookType);
                to.AddToBackpack(spellbook);

                // Give reagent bag (find matching bag)
                string bagName = schoolName.Replace(" ", "") + "ReagentBag";
                Type bagType = ScriptCompiler.FindTypeByName(bagName);
                if (bagType != null)
                {
                    Item bag = (Item)Activator.CreateInstance(bagType);
                    to.AddToBackpack(bag);
                }

                to.SendMessage(0x35, $"Added {schoolName} spellbook and reagents to your backpack!");
            }
            catch (Exception ex)
            {
                to.SendMessage(0x22, $"Error creating {schoolName} package: {ex.Message}");
            }
        }

        private void GiveAllSpellbooks(Mobile to)
        {
            int count = 0;
            foreach (var kvp in Spellbooks)
            {
                GiveSpellbookPackage(to, kvp.Value, kvp.Key);
                count++;
            }
            to.SendMessage(0x35, $"Added all {count} spellbooks with reagents to your backpack!");
        }

        private void GiveTestGear(Mobile to)
        {
            // Give basic test equipment
            to.AddToBackpack(new PlateChest());
            to.AddToBackpack(new PlateLegs());
            to.AddToBackpack(new PlateArms());
            to.AddToBackpack(new PlateGloves());
            to.AddToBackpack(new PlateHelm());
            to.AddToBackpack(new HeaterShield());
            to.AddToBackpack(new Longsword());
            to.AddToBackpack(new Bow());
            to.AddToBackpack(new Arrow(100));
            to.AddToBackpack(new Bandage(100));
            to.AddToBackpack(new GreaterHealPotion(10));
            to.AddToBackpack(new Gold(10000));

            to.SendMessage(0x35, "Added test equipment set to your backpack!");
        }

        // Target classes for various utilities
        private class ClassAssignTarget : Target
        {
            private readonly PlayerClassTypeV2 m_ClassType;

            public ClassAssignTarget(PlayerClassTypeV2 classType) : base(12, false, TargetFlags.None)
            {
                m_ClassType = classType;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                if (targeted is PlayerMobile pm)
                {
                    VystiaClassApplicator.ApplyClass(pm, m_ClassType);
                    from.SendMessage(0x35, $"Assigned {m_ClassType} class to {pm.Name}!");
                }
                else
                {
                    from.SendMessage(0x22, "That is not a player!");
                }
            }
        }

        private class ClassResetTarget : Target
        {
            public ClassResetTarget() : base(12, false, TargetFlags.None) { }

            protected override void OnTarget(Mobile from, object targeted)
            {
                if (targeted is PlayerMobile pm)
                {
                    VystiaClassManager.Instance.RemoveClass(pm);
                    from.SendMessage(0x35, $"Removed class from {pm.Name}!");
                }
                else
                {
                    from.SendMessage(0x22, "That is not a player!");
                }
            }
        }

        private class MaxStatsTarget : Target
        {
            public MaxStatsTarget() : base(12, false, TargetFlags.None) { }

            protected override void OnTarget(Mobile from, object targeted)
            {
                if (targeted is PlayerMobile pm)
                {
                    pm.Str = 150;
                    pm.Dex = 150;
                    pm.Int = 150;
                    pm.Hits = pm.HitsMax;
                    pm.Stam = pm.StamMax;
                    pm.Mana = pm.ManaMax;
                    from.SendMessage(0x35, $"Maxed stats for {pm.Name}!");
                }
                else
                {
                    from.SendMessage(0x22, "That is not a player!");
                }
            }
        }

        private class MaxSkillsTarget : Target
        {
            public MaxSkillsTarget() : base(12, false, TargetFlags.None) { }

            protected override void OnTarget(Mobile from, object targeted)
            {
                if (targeted is PlayerMobile pm)
                {
                    foreach (Skill skill in pm.Skills)
                    {
                        skill.Base = 100.0;
                    }
                    from.SendMessage(0x35, $"Maxed all skills for {pm.Name}!");
                }
                else
                {
                    from.SendMessage(0x22, "That is not a player!");
                }
            }
        }

        private class ClassInfoTarget : Target
        {
            public ClassInfoTarget() : base(12, false, TargetFlags.None) { }

            protected override void OnTarget(Mobile from, object targeted)
            {
                if (targeted is PlayerMobile pm)
                {
                    string className = VystiaClassApplicator.GetClassName(pm);
                    from.SendMessage(0x35, $"{pm.Name}'s Class: {className}");
                    from.SendMessage(0x3B2, $"STR: {pm.Str}, DEX: {pm.Dex}, INT: {pm.Int}");
                    from.SendMessage(0x3B2, $"HP: {pm.Hits}/{pm.HitsMax}, Mana: {pm.Mana}/{pm.ManaMax}");
                }
                else
                {
                    from.SendMessage(0x22, "That is not a player!");
                }
            }
        }
    }

    public class VystiaTestCommand
    {
        public static void Initialize()
        {
            CommandSystem.Register("vystiatest", AccessLevel.GameMaster, new CommandEventHandler(VystiaTest_OnCommand));
            CommandSystem.Register("vtest", AccessLevel.GameMaster, new CommandEventHandler(VystiaTest_OnCommand));
        }

        [Usage("vystiatest")]
        [Aliases("vtest")]
        [Description("Opens the Vystia GM Testing Tools gump")]
        private static void VystiaTest_OnCommand(CommandEventArgs e)
        {
            Mobile from = e.Mobile;
            from.SendGump(new VystiaGMTestingGump(from, 0));
            from.SendMessage(0x35, "Vystia GM Testing Tools opened!");
        }
    }
}
