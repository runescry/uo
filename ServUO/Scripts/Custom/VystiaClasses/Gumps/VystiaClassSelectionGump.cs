using System;
using System.Collections.Generic;
using Server;
using Server.Gumps;
using Server.Mobiles;
using Server.Network;

namespace Server.Custom.VystiaClasses.Gumps
{
    /// <summary>
    /// Main class selection gump shown to new players
    /// Displays all 26 Vystia classes in a grid layout (v2)
    /// </summary>
    public class VystiaClassSelectionGump : Gump
    {
        private const int GumpWidth = 900;
        private const int GumpHeight = 700;

        // All 26 Vystia classes organized by region
        private static readonly ClassInfo[] AllClasses = new ClassInfo[]
        {
            // Frosthold (Ice/Cold)
            new ClassInfo(PlayerClassTypeV2.Barbarian, "Barbarian", "Frosthold Berserker", "Savage warrior with rage mechanics", 1150),
            new ClassInfo(PlayerClassTypeV2.IceMage, "Ice Mage", "Master of Ice", "Control frost and freeze enemies", 1150),
            new ClassInfo(PlayerClassTypeV2.Beastmaster, "Beastmaster", "Beast Tamer", "Command wild creatures", 1150),

            // Emberlands (Fire/Lava)
            new ClassInfo(PlayerClassTypeV2.Sorcerer, "Sorcerer", "Elemental Fury", "Master of fire and elemental magic", 1358),

            // Desert
            new ClassInfo(PlayerClassTypeV2.Ranger, "Ranger", "Dune Strider", "Master archer and tracker", 1719),
            new ClassInfo(PlayerClassTypeV2.Illusionist, "Illusionist", "Desert Mirage", "Master of illusions and deception", 1719),

            // Shadowfen (Swamp/Poison)
            new ClassInfo(PlayerClassTypeV2.Witch, "Witch", "Hex Caster", "Curses, hexes, and poison magic", 2073),
            new ClassInfo(PlayerClassTypeV2.Warlock, "Warlock", "Shadow Dealer", "Dark magic and soul manipulation", 1109),

            // Verdantpeak (Forest/Nature)
            new ClassInfo(PlayerClassTypeV2.Druid, "Druid", "Nature's Guardian", "Shapeshifting and nature magic", 2010),
            new ClassInfo(PlayerClassTypeV2.Alchemist, "Alchemist", "Potion Master", "Brew powerful elixirs and poisons", 2010),

            // Crystal Barrens (Crystal/Energy)
            new ClassInfo(PlayerClassTypeV2.Oracle, "Oracle", "Seer of Fates", "Divination and time magic", 1154),

            // Ironclad (Mechanical/Steam)
            new ClassInfo(PlayerClassTypeV2.Artificer, "Artificer", "Construct Master", "Build clockwork companions", 2305),
            new ClassInfo(PlayerClassTypeV2.Fighter, "Fighter", "Legionnaire", "Elite soldier and weapon master", 2305),
            new ClassInfo(PlayerClassTypeV2.Monk, "Monk", "Iron Fist", "Unarmed combat specialist", 2305),
            new ClassInfo(PlayerClassTypeV2.Templar, "Templar", "Forge Guardian", "Holy warrior of the forge", 2305),

            // ShadowVoid (Void/Darkness)
            new ClassInfo(PlayerClassTypeV2.Necromancer, "Necromancer", "Death Master", "Command the undead", 1109),

            // Underwater (Aquatic)
            new ClassInfo(PlayerClassTypeV2.Summoner, "Summoner", "Creature Caller", "Summon powerful beings", 1365),

            // Sunken Isles
            new ClassInfo(PlayerClassTypeV2.BountyHunter, "Bounty Hunter", "Relentless Tracker", "Hunt down targets for gold", 1719),

            // Glimmering Archipelago
            new ClassInfo(PlayerClassTypeV2.Knight, "Knight", "Noble Warrior", "Mounted combat specialist", 1153),

            // Wilderlands (Shamanic/Spirits)
            new ClassInfo(PlayerClassTypeV2.Shaman, "Shaman", "Spirit Walker", "Command elemental totems", 1281),

            // Multi-Regional
            new ClassInfo(PlayerClassTypeV2.Rogue, "Rogue", "Shadow Operative", "Stealth, poisons, and precision strikes", 1109),
            new ClassInfo(PlayerClassTypeV2.Wizard, "Wizard", "Arcane Scholar", "Master of all magic schools", 1154),
            new ClassInfo(PlayerClassTypeV2.Cleric, "Cleric", "Holy Healer", "Divine magic and AoE healing", 1153),
            new ClassInfo(PlayerClassTypeV2.Paladin, "Paladin", "Holy Knight", "Tank and healer hybrid", 1153),
            new ClassInfo(PlayerClassTypeV2.Bard, "Bard", "Song Weaver", "Buff allies with music", 2011),
            new ClassInfo(PlayerClassTypeV2.Enchanter, "Enchanter", "Arcane Enhancer", "Magical item enchantments", 1154)
        };

        private int _currentPage = 0;
        private const int ClassesPerPage = 12; // 4 rows × 3 columns

        public VystiaClassSelectionGump(PlayerMobile pm, int page = 0) : base(50, 50)
        {
            if (pm == null)
                return;

            pm.CloseGump(typeof(VystiaClassSelectionGump));
            _currentPage = page;

            Closable = false; // Force class selection
            Disposable = false;
            Dragable = true;
            Resizable = false;

            AddPage(0);

            // Background
            AddBackground(0, 0, GumpWidth, GumpHeight, 9200);
            AddImageTiled(10, 10, GumpWidth - 20, GumpHeight - 20, 2624);

            // Header
            AddHtml(20, 20, GumpWidth - 40, 60,
                "<CENTER><BASEFONT COLOR=#FFD700 SIZE=9>Choose Your Vystia Class</BASEFONT><BR>" +
                "<BASEFONT COLOR=#FFFFFF SIZE=5>This choice is permanent - choose wisely!</BASEFONT></CENTER>",
                false, false);

            // Class grid
            int startY = 100;
            int startX = 30;
            int columnWidth = 280;
            int rowHeight = 120;

            int startIndex = _currentPage * ClassesPerPage;
            int endIndex = Math.Min(startIndex + ClassesPerPage, AllClasses.Length);

            for (int i = startIndex; i < endIndex; i++)
            {
                int localIndex = i - startIndex;
                int col = localIndex % 3;
                int row = localIndex / 3;

                int x = startX + (col * columnWidth);
                int y = startY + (row * rowHeight);

                ClassInfo cls = AllClasses[i];

                // Class button background
                AddBackground(x, y, 260, 100, 9350);

                // Class button (using i+1 as button ID, 0 reserved for navigation)
                AddButton(x + 10, y + 10, 4005, 4007, i + 1, GumpButtonType.Reply, 0);

                // Class name (colored by region)
                AddHtml(x + 50, y + 8, 200, 25,
                    $"<BASEFONT COLOR=#{cls.Hue:X6}>{cls.Name}</BASEFONT>",
                    false, false);

                // Class title
                AddHtml(x + 50, y + 30, 200, 20,
                    $"<BASEFONT COLOR=#C0C0C0 SIZE=1>{cls.Title}</BASEFONT>",
                    false, false);

                // Class description
                AddHtml(x + 50, y + 50, 200, 40,
                    $"<BASEFONT COLOR=#A0A0A0 SIZE=1>{cls.Description}</BASEFONT>",
                    false, false);
            }

            // Page navigation
            int totalPages = (AllClasses.Length + ClassesPerPage - 1) / ClassesPerPage;

            if (totalPages > 1)
            {
                // Previous page button
                if (_currentPage > 0)
                {
                    AddButton(50, GumpHeight - 60, 4014, 4016, 997, GumpButtonType.Reply, 0);
                    AddHtml(80, GumpHeight - 55, 100, 25, "<BASEFONT COLOR=#FFFFFF>Previous</BASEFONT>", false, false);
                }

                // Page indicator
                AddHtml(GumpWidth / 2 - 50, GumpHeight - 55, 100, 25,
                    $"<CENTER><BASEFONT COLOR=#FFD700>Page {_currentPage + 1} of {totalPages}</BASEFONT></CENTER>",
                    false, false);

                // Next page button
                if (_currentPage < totalPages - 1)
                {
                    AddHtml(GumpWidth - 150, GumpHeight - 55, 70, 25, "<BASEFONT COLOR=#FFFFFF>Next</BASEFONT>", false, false);
                    AddButton(GumpWidth - 80, GumpHeight - 60, 4005, 4007, 998, GumpButtonType.Reply, 0);
                }
            }

            // Help text at bottom
            AddHtml(20, GumpHeight - 90, GumpWidth - 40, 25,
                "<CENTER><BASEFONT COLOR=#FFFF00>Click a class to view details and confirm your selection</BASEFONT></CENTER>",
                false, false);
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            PlayerMobile pm = sender?.Mobile as PlayerMobile;
            if (pm == null)
                return;

            int buttonID = info.ButtonID;

            // Page navigation
            if (buttonID == 997) // Previous page
            {
                pm.SendGump(new VystiaClassSelectionGump(pm, _currentPage - 1));
                return;
            }
            else if (buttonID == 998) // Next page
            {
                pm.SendGump(new VystiaClassSelectionGump(pm, _currentPage + 1));
                return;
            }

            // Class selection (button IDs 1-26)
            if (buttonID >= 1 && buttonID <= AllClasses.Length)
            {
                ClassInfo selectedClass = AllClasses[buttonID - 1];
                pm.SendGump(new VystiaClassConfirmationGump(pm, selectedClass.ClassType));
            }
            else
            {
                // No button pressed or invalid - reopen gump
                pm.SendGump(new VystiaClassSelectionGump(pm, _currentPage));
            }
        }

        /// <summary>
        /// Internal class to store class display information
        /// </summary>
        private class ClassInfo
        {
            public PlayerClassTypeV2 ClassType { get; }
            public string Name { get; }
            public string Title { get; }
            public string Description { get; }
            public int Hue { get; }

            public ClassInfo(PlayerClassTypeV2 classType, string name, string title, string description, int hue)
            {
                ClassType = classType;
                Name = name;
                Title = title;
                Description = description;
                Hue = hue;
            }
        }
    }
}
