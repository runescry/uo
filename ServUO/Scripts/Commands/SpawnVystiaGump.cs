using System;
using System.Collections.Generic;
using Server;
using Server.Commands;
using Server.Gumps;
using Server.Mobiles;
using Server.Network;
using Server.Items;

namespace Server.Scripts.Commands
{
    public class SpawnVystiaGump : Gump
    {
        private readonly Mobile m_From;
        private readonly int m_Radius;
        private readonly int m_Page;

        // Region definitions with their creatures
        private static readonly Dictionary<string, List<Type>> RegionCreatures = new Dictionary<string, List<Type>>
        {
            ["Bosses"] = new List<Type>
            {
                typeof(AncientKraken), typeof(AncientTreant), typeof(CovenMatriarch),
                typeof(CrystalDrakeAlpha), typeof(ForgeMaster), typeof(FrostFather),
                typeof(GriffinLord), typeof(SphinxOfSurya), typeof(TimewornLich), typeof(VolcanoWyrm)
            },
            ["Frosthold"] = new List<Type>
            {
                typeof(FrostElemental), typeof(FrostGiant), typeof(FrostWraith), typeof(FrozenHorror),
                typeof(GlacialBear), typeof(IceGolem), typeof(IceSprite), typeof(IceTroll),
                typeof(Snowdrifter), typeof(VystiaArcticOgre), typeof(VystiaFrostDragon), typeof(WinterWolf)
            },
            ["Emberlands"] = new List<Type>
            {
                typeof(AshGolem), typeof(EmberBat), typeof(FlameSprite), typeof(LavaHound),
                typeof(MagmaTroll), typeof(VystiaEmberPhoenix), typeof(VystiaFireAnt), typeof(VystiaLavaElemental)
            },
            ["Desert"] = new List<Type>
            {
                typeof(Ankheg), typeof(DesertHarpy), typeof(DesertMummy), typeof(DuneReaper),
                typeof(OasisGuardian), typeof(SandElemental), typeof(ScarabBeetle), typeof(SunlitSerpent),
                typeof(VystiaDesertScorpion), typeof(VystiaDesertWyrm), typeof(VystiaSandVortex)
            },
            ["Shadowfen"] = new List<Type>
            {
                typeof(BogBeast), typeof(BogWitch), typeof(FenStalker), typeof(MireLeech),
                typeof(Mistwalker), typeof(ShadowWolf), typeof(SwampHorror), typeof(SwampTroll),
                typeof(SwampWisp), typeof(ToxicToad), typeof(VystiaMireElemental),
                typeof(VystiaShadowBogling), typeof(VystiaSwampDrake)
            },
            ["Verdantpeak"] = new List<Type>
            {
                typeof(ForestBear), typeof(ForestGuardian), typeof(ForestSerpent), typeof(ForestSprite),
                typeof(GiantStag), typeof(MossGolem), typeof(NatureElemental), typeof(ThornVine),
                typeof(TreantSapling), typeof(VineCreeper), typeof(VystiaWoodlandDrake),
                typeof(WildBoar), typeof(WildcatAlpha)
            },
            ["Crystal Barrens"] = new List<Type>
            {
                typeof(CrystalGolem), typeof(CrystalSerpent), typeof(PrismaticWisp), typeof(VystiaCrystalElemental)
            },
            ["Ironclad"] = new List<Type>
            {
                typeof(BrassAutomaton), typeof(ClockworkSpider), typeof(CopperSentinel), typeof(GearWolf),
                typeof(IronElemental), typeof(OilSlime), typeof(SteamElemental), typeof(SteamGolem),
                typeof(VystiaMechanicalDrake)
            },
            ["Skyreach"] = new List<Type>
            {
                typeof(AirSprite), typeof(GaleRider), typeof(NimbusWraith), typeof(SkyEagle),
                typeof(SkyGolem), typeof(StormGiant), typeof(StormHarpy), typeof(StormRoc),
                typeof(StormWisp), typeof(ThunderBird), typeof(Zephyr), typeof(VystiaCloudDrake),
                typeof(VystiaCloudSerpent), typeof(VystiaGaleElemental), typeof(VystiaLightningDrake)
            },
            ["Underwater"] = new List<Type>
            {
                typeof(AbyssalCrab), typeof(AbyssalJellyfish), typeof(AbyssalSquid), typeof(CoralGolem),
                typeof(DeepwaterShark), typeof(KelpHorror), typeof(Merfolk), typeof(SeaHag),
                typeof(SirenWraith), typeof(TidalElemental), typeof(VystiaDeepSeaSerpent), typeof(VystiaSeaWyrm)
            },
            ["ShadowVoid"] = new List<Type>
            {
                typeof(AbyssalHorror), typeof(NightmareHound), typeof(ShadowElemental), typeof(VoidStalker),
                typeof(VoidTentacle), typeof(VoidWraith), typeof(VystiaDarkWisp), typeof(VystiaShadowDemon),
                typeof(VystiaVoidDrake)
            },
            ["Misc"] = new List<Type>
            {
                typeof(VystiaBandit), typeof(VystiaEttin), typeof(VystiaGhoul), typeof(VystiaGiantSpider),
                typeof(VystiaGoblin), typeof(VystiaOgre), typeof(VystiaOrc), typeof(VystiaOrcCaptain),
                typeof(VystiaSkeleton), typeof(VystiaSpectre), typeof(VystiaSpider), typeof(VystiaTroll),
                typeof(VystiaWolf), typeof(VystiaZombie), typeof(WildHorse), typeof(PracticeDummy)
            }
        };

        // Magic items (spellbooks and reagent bags) - organized by school
        private static readonly Dictionary<string, (Type Spellbook, Type ReagentBag)> MagicItems = new Dictionary<string, (Type, Type)>
        {
            ["Ice Magic"] = (typeof(IceMageSpellbook), typeof(IceMagicReagentBag)),
            ["Nature Magic"] = (typeof(DruidSpellbook), typeof(NatureMagicReagentBag)),
            ["Hex Magic"] = (typeof(WitchSpellbook), typeof(HexMagicReagentBag)),
            ["Elemental Magic"] = (typeof(SorcererSpellbook), typeof(ElementalMagicReagentBag)),
            ["Dark Magic"] = (typeof(WarlockSpellbook), typeof(DarkMagicReagentBag)),
            ["Divination Magic"] = (typeof(OracleSpellbook), typeof(DivinationMagicReagentBag)),
            ["Necromancy"] = (typeof(VystiaNecromancerSpellbook), typeof(NecromancyReagentBag)),
            ["Summoning Magic"] = (typeof(SummonerSpellbook), typeof(SummoningMagicReagentBag)),
            ["Shamanic Magic"] = (typeof(ShamanSpellbook), typeof(ShamanicMagicReagentBag)),
#if VYSTIA_SONGWEAVING
            ["Songweaving"] = (typeof(SongweavingSpellbook), typeof(SongweavingReagentBag)),
#endif
            ["Enchanting Magic"] = (typeof(EnchanterSpellbook), typeof(EnchantingMagicReagentBag)),
            ["Illusion Magic"] = (typeof(IllusionistSpellbook), typeof(IllusionMagicReagentBag))
        };

        // Magic school display order
        private static readonly string[] MagicSchoolOrder = new string[]
        {
            "Ice Magic", "Nature Magic", "Hex Magic", "Elemental Magic", "Dark Magic", "Divination Magic",
            "Necromancy", "Summoning Magic", "Shamanic Magic",
#if VYSTIA_SONGWEAVING
            "Songweaving",
#endif
            "Enchanting Magic", "Illusion Magic"
        };

        // Vendors (12 magic school vendors + 2 general vendors)
        private static readonly Dictionary<string, Type> VendorTypes = new Dictionary<string, Type>
        {
            ["Ice Magic Vendor"] = typeof(IceMageVendor),
            ["Nature Magic Vendor"] = typeof(DruidVendor),
            ["Hex Magic Vendor"] = typeof(WitchVendor),
            ["Elemental Magic Vendor"] = typeof(SorcererVendor),
            ["Dark Magic Vendor"] = typeof(WarlockVendor),
            ["Divination Magic Vendor"] = typeof(OracleVendor),
            ["Necromancy Vendor"] = typeof(NecromancerVendor),
            ["Summoning Magic Vendor"] = typeof(SummonerVendor),
            ["Shamanic Magic Vendor"] = typeof(ShamanVendor),
#if VYSTIA_SONGWEAVING
            ["Songweaving Vendor"] = typeof(BardVendor),
#endif
            ["Enchanting Magic Vendor"] = typeof(EnchanterVendor),
            ["Illusion Magic Vendor"] = typeof(IllusionistVendor),
            ["All Reagents Vendor"] = typeof(VystiaReagentVendor),
            ["All Resources Vendor"] = typeof(VystiaResourceVendor)
        };

        // Vendor display order
        private static readonly string[] VendorOrder = new string[]
        {
            "Ice Magic Vendor", "Nature Magic Vendor", "Hex Magic Vendor", "Elemental Magic Vendor",
            "Dark Magic Vendor", "Divination Magic Vendor", "Necromancy Vendor", "Summoning Magic Vendor",
            "Shamanic Magic Vendor",
#if VYSTIA_SONGWEAVING
            "Songweaving Vendor",
#endif
            "Enchanting Magic Vendor", "Illusion Magic Vendor",
            "All Reagents Vendor", "All Resources Vendor"
        };

        // Magic school hues (matching spellbook colors)
        private static readonly int[] MagicSchoolHues = new int[]
        {
            0x481,  // Ice Magic - Ice Blue
            0x7D6,  // Nature Magic - Forest Green
            0x81D,  // Hex Magic - Murky Green/Purple
            0x54E,  // Elemental Magic - Fiery Orange
            0x455,  // Dark Magic - Void Black
            0x482,  // Divination Magic - Crystal Blue
            0x455,  // Necromancy - Void Black
            0x555,  // Summoning Magic - Deep Blue
            0x501,  // Shamanic Magic - Storm Blue
#if VYSTIA_SONGWEAVING
            0x8A5,  // Songweaving - Golden
#endif
            0x8FD,  // Enchanting Magic - Arcane Purple
            0x47E   // Illusion Magic - Silvery
        };

        // Region display order and colors
        private static readonly string[] RegionOrder = new string[]
        {
            "Bosses", "Frosthold", "Emberlands", "Desert", "Shadowfen", "Verdantpeak",
            "Crystal Barrens", "Ironclad", "Skyreach", "Underwater", "ShadowVoid", "Misc"
        };

        private static readonly int[] RegionHues = new int[]
        {
            1161,  // Bosses - Red
            1152,  // Frosthold - Ice Blue
            1358,  // Emberlands - Fire Orange
            2305,  // Desert - Sand
            2073,  // Shadowfen - Swamp Green
            2010,  // Verdantpeak - Forest Green
            1154,  // Crystal Barrens - Crystal Blue
            2401,  // Ironclad - Bronze
            1281,  // Skyreach - Storm Blue
            1365,  // Underwater - Deep Blue
            1109,  // ShadowVoid - Black
            0      // Misc - Default
        };

        public static void Initialize()
        {
            CommandSystem.Register("spawnvystia", AccessLevel.GameMaster, SpawnVystia_OnCommand);
            CommandSystem.Register("clearvystia", AccessLevel.GameMaster, ClearVystia_OnCommand);
        }

        [Usage("spawnvystia [radius]")]
        [Description("Opens a gump to spawn Vystia creatures by region. Default radius is 10 tiles.")]
        private static void SpawnVystia_OnCommand(CommandEventArgs e)
        {
            Mobile from = e.Mobile;
            int radius = 10;

            if (e.Arguments.Length > 0)
            {
                if (!int.TryParse(e.Arguments[0], out radius) || radius < 5 || radius > 50)
                {
                    from.SendMessage("Usage: [spawnvystia [radius]. Radius must be between 5 and 50.");
                    return;
                }
            }

            from.CloseGump(typeof(SpawnVystiaGump));
            from.SendGump(new SpawnVystiaGump(from, radius, 0));
        }

        [Usage("clearvystia [radius]")]
        [Description("Removes all Vystia creatures in a radius around you. Default radius is 10 tiles.")]
        private static void ClearVystia_OnCommand(CommandEventArgs e)
        {
            Mobile from = e.Mobile;
            int radius = 10;

            if (e.Arguments.Length > 0)
            {
                if (!int.TryParse(e.Arguments[0], out radius) || radius < 5 || radius > 100)
                {
                    from.SendMessage("Usage: [clearvystia [radius]. Radius must be between 5 and 100.");
                    return;
                }
            }

            int deleted = 0;
            List<Mobile> toDelete = new List<Mobile>();

            foreach (Mobile m in from.GetMobilesInRange(radius))
            {
                if (m is BaseCreature bc)
                {
                    foreach (var region in RegionCreatures.Values)
                    {
                        if (region.Contains(bc.GetType()))
                        {
                            toDelete.Add(m);
                            break;
                        }
                    }
                }
            }

            foreach (Mobile m in toDelete)
            {
                m.Delete();
                deleted++;
            }

            from.SendMessage($"Deleted {deleted} Vystia creatures within {radius} tiles.");
        }

        public SpawnVystiaGump(Mobile from, int radius, int page = 0) : base(50, 50)
        {
            m_From = from;
            m_Radius = radius;
            m_Page = page;

            Closable = true;
            Disposable = true;
            Dragable = true;
            Resizable = false;

            AddPage(0);

            // Background
            AddBackground(0, 0, 450, 550, 9270);

            if (page == 0)
                BuildCreaturesPage();
            else if (page == 1)
                BuildMagicPage();
            else
                BuildVendorsPage();
        }

        private void BuildCreaturesPage()
        {
            // Title
            AddHtml(0, 15, 450, 20, Center(Color("Vystia Creature Spawner", 0x00FF00)), false, false);
            AddHtml(0, 35, 450, 20, Center($"Radius: {m_Radius} tiles"), false, false);

            // Spawn All button
            AddButton(150, 60, 4023, 4024, 1, GumpButtonType.Reply, 0);
            AddHtml(190, 62, 150, 20, Color("Spawn ALL (131)", 0xFFFF00), false, false);

            // Clear All button
            AddButton(150, 85, 4020, 4021, 2, GumpButtonType.Reply, 0);
            AddHtml(190, 87, 150, 20, Color("Clear ALL", 0xFF6666), false, false);

            // Page navigation
            AddButton(20, 85, 4014, 4015, 999, GumpButtonType.Reply, 0);
            AddHtml(60, 87, 150, 20, Color("Magic Items >>", 0x00FFFF), false, false);

            AddButton(250, 85, 4014, 4015, 997, GumpButtonType.Reply, 0);
            AddHtml(290, 87, 130, 20, Color("Vendors >>", 0xFFCC00), false, false);

            // Separator
            AddImageTiled(20, 115, 410, 2, 5410);

            // Region buttons
            int y = 130;
            for (int i = 0; i < RegionOrder.Length; i++)
            {
                string region = RegionOrder[i];
                int count = RegionCreatures[region].Count;
                int hue = RegionHues[i];

                // Spawn button
                AddButton(30, y, 4005, 4006, 100 + i, GumpButtonType.Reply, 0);

                // Region name with count
                string label = $"{region} ({count})";
                AddHtml(70, y + 2, 150, 20, Color(label, HueToHtmlColor(hue)), false, false);

                // Clear region button
                AddButton(230, y, 4017, 4018, 200 + i, GumpButtonType.Reply, 0);
                AddHtml(270, y + 2, 80, 20, Color("Clear", 0xFF6666), false, false);

                y += 28;
            }

            // Footer
            AddImageTiled(20, y + 10, 410, 2, 5410);
            AddHtml(0, y + 20, 450, 20, Center("Click region to spawn, 'Clear' to remove"), false, false);

            // Close button
            AddButton(200, y + 45, 4023, 4024, 0, GumpButtonType.Reply, 0);
            AddHtml(240, y + 47, 60, 20, "Close", false, false);
        }

        private void BuildMagicPage()
        {
            // Title
            AddHtml(0, 15, 450, 20, Center(Color("Vystia Magic Items", 0x00FFFF)), false, false);
            AddHtml(0, 35, 450, 20, Center("Spellbooks & Reagent Bags"), false, false);

            // Page navigation
            AddButton(20, 60, 4014, 4015, 998, GumpButtonType.Reply, 0);
            AddHtml(60, 62, 150, 20, Color("<< Creatures", 0x00FF00), false, false);

            AddButton(200, 60, 4014, 4015, 997, GumpButtonType.Reply, 0);
            AddHtml(240, 62, 130, 20, Color("Vendors >>", 0xFFCC00), false, false);

            // Spawn All Magic button
            AddButton(20, 85, 4023, 4024, 900, GumpButtonType.Reply, 0);
            AddHtml(60, 87, 180, 20, Color("Spawn ALL Magic (24)", 0xFFFF00), false, false);

            // Separator
            AddImageTiled(20, 90, 410, 2, 5410);

            // Magic school buttons
            int y = 105;
            for (int i = 0; i < MagicSchoolOrder.Length; i++)
            {
                string school = MagicSchoolOrder[i];
                int hue = MagicSchoolHues[i];

                // School name header
                AddHtml(30, y, 200, 20, Color(school, HueToHtmlColor(hue)), false, false);

                // Spellbook button
                AddButton(240, y, 4005, 4006, 500 + i, GumpButtonType.Reply, 0);
                AddHtml(280, y + 2, 80, 20, Color("Book", 0xFFFFFF), false, false);

                // Reagent bag button
                AddButton(350, y, 4005, 4006, 600 + i, GumpButtonType.Reply, 0);
                AddHtml(390, y + 2, 50, 20, Color("Bag", 0xFFFFFF), false, false);

                y += 25;
            }

            // Footer
            AddImageTiled(20, y + 10, 410, 2, 5410);
            AddHtml(0, y + 20, 450, 20, Center("Click 'Book' for spellbook, 'Bag' for reagents"), false, false);

            // Close button
            AddButton(200, y + 45, 4023, 4024, 0, GumpButtonType.Reply, 0);
            AddHtml(240, y + 47, 60, 20, "Close", false, false);
        }

        private void BuildVendorsPage()
        {
            // Title
            AddHtml(0, 15, 450, 20, Center(Color("Vystia Vendors", 0xFFCC00)), false, false);
            AddHtml(0, 35, 450, 20, Center("Magic School & Resource Vendors"), false, false);

            // Page navigation
            AddButton(20, 60, 4014, 4015, 998, GumpButtonType.Reply, 0);
            AddHtml(60, 62, 150, 20, Color("<< Creatures", 0x00FF00), false, false);

            AddButton(200, 60, 4014, 4015, 999, GumpButtonType.Reply, 0);
            AddHtml(240, 62, 150, 20, Color("<< Magic Items", 0x00FFFF), false, false);

            // Spawn All Vendors button
            AddButton(20, 85, 4023, 4024, 800, GumpButtonType.Reply, 0);
            AddHtml(60, 87, 180, 20, Color("Spawn ALL Vendors (14)", 0xFFFF00), false, false);

            // Separator
            AddImageTiled(20, 115, 410, 2, 5410);

            // Vendor buttons
            int y = 130;
            for (int i = 0; i < VendorOrder.Length; i++)
            {
                string vendorName = VendorOrder[i];

                // Determine color based on vendor type
                int hue = 0xFFFFFF;
                if (i < 12) // Magic school vendors
                {
                    hue = MagicSchoolHues[i];
                }
                else // General vendors
                {
                    hue = 0xFFCC00; // Golden for general vendors
                }

                // Spawn button
                AddButton(30, y, 4005, 4006, 700 + i, GumpButtonType.Reply, 0);
                AddHtml(70, y + 2, 350, 20, Color(vendorName, HueToHtmlColor(hue)), false, false);

                y += 25;
            }

            // Footer
            AddImageTiled(20, y + 10, 410, 2, 5410);
            AddHtml(0, y + 20, 450, 20, Center("Click vendor to spawn at cursor location"), false, false);

            // Close button
            AddButton(200, y + 45, 4023, 4024, 0, GumpButtonType.Reply, 0);
            AddHtml(240, y + 47, 60, 20, "Close", false, false);
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            Mobile from = sender.Mobile;

            if (from == null)
                return;

            int buttonId = info.ButtonID;

            if (buttonId == 0) // Close
                return;

            // Page navigation
            if (buttonId == 999) // Go to Magic page
            {
                from.SendGump(new SpawnVystiaGump(from, m_Radius, 1));
                return;
            }
            if (buttonId == 998) // Go to Creatures page
            {
                from.SendGump(new SpawnVystiaGump(from, m_Radius, 0));
                return;
            }
            if (buttonId == 997) // Go to Vendors page
            {
                from.SendGump(new SpawnVystiaGump(from, m_Radius, 2));
                return;
            }

            // Magic items
            if (buttonId == 900) // Spawn ALL magic items
            {
                int total = 0;
                foreach (var schoolName in MagicSchoolOrder)
                {
                    var (spellbookType, bagType) = MagicItems[schoolName];
                    total += GiveMagicItem(from, spellbookType) ? 1 : 0;
                    total += GiveMagicItem(from, bagType) ? 1 : 0;
                }
                from.SendMessage($"Spawned {total} magic items (spellbooks + reagent bags where applicable).");
                from.SendGump(new SpawnVystiaGump(from, m_Radius, 1));
                return;
            }
            if (buttonId >= 500 && buttonId < 600) // Spawn spellbook
            {
                int index = buttonId - 500;
                if (index < MagicSchoolOrder.Length)
                {
                    string schoolName = MagicSchoolOrder[index];
                    var (spellbookType, _) = MagicItems[schoolName];
                    GiveMagicItem(from, spellbookType);
                    from.SendMessage($"Spawned {schoolName} spellbook.");
                }
                from.SendGump(new SpawnVystiaGump(from, m_Radius, 1));
                return;
            }
            if (buttonId >= 600 && buttonId < 700) // Spawn reagent bag
            {
                int index = buttonId - 600;
                if (index < MagicSchoolOrder.Length)
                {
                    string schoolName = MagicSchoolOrder[index];
                    var (_, bagType) = MagicItems[schoolName];
                    if (GiveMagicItem(from, bagType))
                        from.SendMessage($"Spawned {schoolName} reagent bag.");
                    else
                        from.SendMessage($"{schoolName} has no reagent bag.");
                }
                from.SendGump(new SpawnVystiaGump(from, m_Radius, 1));
                return;
            }

            // Vendors
            if (buttonId == 800) // Spawn ALL vendors
            {
                int total = 0;
                foreach (var vendorName in VendorOrder)
                {
                    Type vendorType = VendorTypes[vendorName];
                    if (SpawnVendor(from, vendorType))
                    {
                        total++;
                    }
                }
                from.SendMessage($"Spawned {total} vendors at your location.");
                from.SendGump(new SpawnVystiaGump(from, m_Radius, 2));
                return;
            }
            if (buttonId >= 700 && buttonId < 800) // Spawn individual vendor
            {
                int index = buttonId - 700;
                if (index < VendorOrder.Length)
                {
                    string vendorName = VendorOrder[index];
                    Type vendorType = VendorTypes[vendorName];
                    if (SpawnVendor(from, vendorType))
                    {
                        from.SendMessage($"Spawned {vendorName} at your location.");
                    }
                    else
                    {
                        from.SendMessage($"Failed to spawn {vendorName}.");
                    }
                }
                from.SendGump(new SpawnVystiaGump(from, m_Radius, 2));
                return;
            }

            if (buttonId == 1) // Spawn ALL creatures
            {
                int total = 0;
                foreach (var region in RegionCreatures)
                {
                    total += SpawnRegion(from, region.Key, region.Value);
                }
                from.SendMessage($"Spawned {total} creatures from all regions.");
                from.SendGump(new SpawnVystiaGump(from, m_Radius, 0));
            }
            else if (buttonId == 2) // Clear ALL
            {
                int deleted = ClearAllVystia(from, m_Radius);
                from.SendMessage($"Deleted {deleted} Vystia creatures.");
                from.SendGump(new SpawnVystiaGump(from, m_Radius, 0));
            }
            else if (buttonId >= 100 && buttonId < 200) // Spawn region
            {
                int index = buttonId - 100;
                if (index < RegionOrder.Length)
                {
                    string region = RegionOrder[index];
                    int spawned = SpawnRegion(from, region, RegionCreatures[region]);
                    from.SendMessage($"Spawned {spawned} creatures from {region}.");
                }
                from.SendGump(new SpawnVystiaGump(from, m_Radius, 0));
            }
            else if (buttonId >= 200 && buttonId < 300) // Clear region
            {
                int index = buttonId - 200;
                if (index < RegionOrder.Length)
                {
                    string region = RegionOrder[index];
                    int deleted = ClearRegion(from, RegionCreatures[region], m_Radius);
                    from.SendMessage($"Deleted {deleted} creatures from {region}.");
                }
                from.SendGump(new SpawnVystiaGump(from, m_Radius, 0));
            }
        }

        private bool GiveMagicItem(Mobile to, Type itemType)
        {
            if (itemType == null)
                return false;

            try
            {
                Item item = (Item)Activator.CreateInstance(itemType);
                if (item != null)
                {
                    // If this is a spellbook, fill it with all spells
                    if (item is Spellbook spellbook)
                    {
                        // Set all 32 spell bits (0xFFFFFFFF = first 32 bits set to 1)
                        spellbook.Content = 0xFFFFFFFF;
                    }
                    // If this is a reagent bag, increase reagent amounts to 50 each
                    else if (item is Bag bag && item.GetType().Name.Contains("ReagentBag"))
                    {
                        foreach (Item reagent in new List<Item>(bag.Items))
                        {
                            if (reagent != null)
                            {
                                reagent.Amount = 50;
                            }
                        }
                    }

                    if (to.Backpack != null)
                        to.Backpack.DropItem(item);
                    else
                        item.MoveToWorld(to.Location, to.Map);

                    return true;
                }
            }
            catch
            {
                // Silent fail for individual items
            }

            return false;
        }

        private bool SpawnVendor(Mobile from, Type vendorType)
        {
            try
            {
                Mobile vendor = (Mobile)Activator.CreateInstance(vendorType);
                if (vendor != null)
                {
                    vendor.MoveToWorld(from.Location, from.Map);
                    return true;
                }
            }
            catch
            {
                // Silent fail
            }
            return false;
        }

        private int SpawnRegion(Mobile from, string regionName, List<Type> creatures)
        {
            int spawned = 0;
            Point3D center = from.Location;
            Map map = from.Map;

            foreach (Type creatureType in creatures)
            {
                try
                {
                    BaseCreature creature = (BaseCreature)Activator.CreateInstance(creatureType);

                    if (creature != null)
                    {
                        Point3D spawnLoc = GetRandomSpawnLocation(center, m_Radius, map);
                        creature.MoveToWorld(spawnLoc, map);
                        creature.Home = spawnLoc;
                        creature.RangeHome = 10;
                        spawned++;
                    }
                }
                catch
                {
                    // Silent fail for individual creatures
                }
            }

            return spawned;
        }

        private int ClearRegion(Mobile from, List<Type> creatures, int radius)
        {
            int deleted = 0;
            List<Mobile> toDelete = new List<Mobile>();

            foreach (Mobile m in from.GetMobilesInRange(radius))
            {
                if (m is BaseCreature bc && creatures.Contains(bc.GetType()))
                {
                    toDelete.Add(m);
                }
            }

            foreach (Mobile m in toDelete)
            {
                m.Delete();
                deleted++;
            }

            return deleted;
        }

        private int ClearAllVystia(Mobile from, int radius)
        {
            int deleted = 0;
            List<Mobile> toDelete = new List<Mobile>();

            foreach (Mobile m in from.GetMobilesInRange(radius))
            {
                if (m is BaseCreature bc)
                {
                    foreach (var region in RegionCreatures.Values)
                    {
                        if (region.Contains(bc.GetType()))
                        {
                            toDelete.Add(m);
                            break;
                        }
                    }
                }
            }

            foreach (Mobile m in toDelete)
            {
                m.Delete();
                deleted++;
            }

            return deleted;
        }

        private static Point3D GetRandomSpawnLocation(Point3D center, int radius, Map map)
        {
            for (int i = 0; i < 20; i++)
            {
                int x = center.X + Utility.RandomMinMax(-radius, radius);
                int y = center.Y + Utility.RandomMinMax(-radius, radius);
                int z = map.GetAverageZ(x, y);

                Point3D loc = new Point3D(x, y, z);

                if (map.CanSpawnMobile(loc))
                    return loc;
            }

            return center;
        }

        private static string Center(string text)
        {
            return $"<CENTER>{text}</CENTER>";
        }

        private static string Color(string text, int color)
        {
            return $"<BASEFONT COLOR=#{color:X6}>{text}</BASEFONT>";
        }

        private static int HueToHtmlColor(int hue)
        {
            // Approximate UO hue to HTML color
            switch (hue)
            {
                // Creature region hues
                case 1152: return 0x88CCFF; // Ice Blue
                case 1358: return 0xFF6600; // Fire Orange
                case 2305: return 0xDDCC88; // Sand
                case 2073: return 0x668866; // Swamp Green
                case 2010: return 0x228822; // Forest Green
                case 1154: return 0x66CCFF; // Crystal Blue
                case 2401: return 0xCC9966; // Bronze
                case 1281: return 0x6688CC; // Storm Blue
                case 1365: return 0x3366AA; // Deep Blue
                case 1109: return 0x666666; // Dark Gray
                case 1161: return 0xFF4444; // Red (Bosses)

                // Magic school hues (duplicates removed - use creature region colors)
                case 0x481: return 0x88CCFF; // Ice Magic - Ice Blue
                case 0x7D6: return 0x228822; // Nature Magic - Forest Green
                case 0x81D: return 0x9966CC; // Hex Magic - Murky Purple
                // 0x54E = 1358 (Emberlands) - already defined
                // 0x455 = 1109 (ShadowVoid) - already defined
                // 0x482 = 1154 (Crystal Barrens) - already defined
                // 0x555 = 1365 (Underwater) - already defined
                // 0x501 = 1281 (Skyreach) - already defined
#if VYSTIA_SONGWEAVING
                case 0x8A5: return 0xFFCC00; // Songweaving - Golden
#endif
                case 0x8FD: return 0xAA66FF; // Enchanting Magic - Arcane Purple
                case 0x47E: return 0xCCCCCC; // Illusion Magic - Silvery

                default: return 0xFFFFFF;   // White
            }
        }
    }
}
