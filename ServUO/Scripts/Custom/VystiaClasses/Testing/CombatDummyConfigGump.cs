using System;
using Server;
using Server.Gumps;
using Server.Mobiles;
using Server.Network;
using Server.Custom.VystiaClasses;
using Server.Custom.VystiaClasses.Religion;
using Server.Custom.VystiaClasses.Factions;

namespace Server.Custom.VystiaClasses.Testing
{
    /// <summary>
    /// Configuration gump for spawning custom combat dummies with class, faction, and religion selection
    /// </summary>
    public class CombatDummyConfigGump : Gump
    {
        private readonly PlayerMobile m_Player;
        private CombatDummyMode m_SelectedMode = CombatDummyMode.Hybrid;
        private PlayerClassTypeV2 m_SelectedClass = PlayerClassTypeV2.None;
        private VystiaFaction m_SelectedFaction = VystiaFaction.None;
        private VystiaReligion m_SelectedReligion = VystiaReligion.None;

        // Opposing pairs page - separate class selection for each dummy
        private PlayerClassTypeV2 m_PairClass1 = PlayerClassTypeV2.None;
        private PlayerClassTypeV2 m_PairClass2 = PlayerClassTypeV2.None;
        private CombatDummyMode m_PairMode = CombatDummyMode.Hybrid;

        // Button ID ranges
        private const int MODE_START = 100;        // 100-103
        private const int CLASS_START = 200;       // 200-225
        private const int FACTION_START = 300;     // 300-307
        private const int RELIGION_START = 400;    // 400-406
        private const int SPAWN_BUTTON = 500;
        private const int SPAWN_ALL_MODES = 501;
        private const int BACK_BUTTON = 510;

        // Tab buttons
        private const int TAB_SINGLE = 600;
        private const int TAB_PAIRS = 601;

        // Opposing pairs page buttons
        private const int PAIR_MODE_START = 700;        // 700-703
        private const int PAIR_CLASS1_START = 800;      // 800-825
        private const int PAIR_CLASS2_START = 900;       // 900-925
        private const int SPAWN_FACTION_PAIR = 1000;     // Frostguard vs FlameLegion
        private const int SPAWN_RELIGION_PAIR = 1001;    // Frostfather vs Emberheart
        private const int SPAWN_VOID_PAIR = 1002;        // Voidborn vs Greenward
        private const int SPAWN_MAGIC_PAIR = 1003;       // Arcane vs Technoguild

        private bool m_ShowPairsPage = false;

        public CombatDummyConfigGump(PlayerMobile player) : this(player, false, CombatDummyMode.Hybrid, PlayerClassTypeV2.None, VystiaFaction.None, VystiaReligion.None, CombatDummyMode.Hybrid, PlayerClassTypeV2.None, PlayerClassTypeV2.None)
        {
        }

        public CombatDummyConfigGump(PlayerMobile player, bool showPairsPage, CombatDummyMode mode, PlayerClassTypeV2 classType, VystiaFaction faction, VystiaReligion religion, CombatDummyMode pairMode, PlayerClassTypeV2 pairClass1, PlayerClassTypeV2 pairClass2)
            : base(50, 50)
        {
            m_Player = player;
            m_ShowPairsPage = showPairsPage;
            m_SelectedMode = mode;
            m_SelectedClass = classType;
            m_SelectedFaction = faction;
            m_SelectedReligion = religion;
            m_PairMode = pairMode;
            m_PairClass1 = pairClass1;
            m_PairClass2 = pairClass2;

            Closable = true;
            Disposable = true;
            Dragable = true;
            Resizable = false;

            BuildGump();
        }

        private void BuildGump()
        {
            AddPage(0);

            // Main background - larger to fit all options
            AddBackground(0, 0, 650, 720, 9200);
            AddAlphaRegion(10, 10, 630, 700);

            // Title
            AddLabel(250, 15, 0x455, "Combat Dummy Configuration");
            
            // Tab buttons
            AddButton(20, 40, m_ShowPairsPage ? 4006 : 4005, m_ShowPairsPage ? 4007 : 4006, TAB_SINGLE, GumpButtonType.Reply, 0);
            AddLabel(55, 40, m_ShowPairsPage ? 0 : 0x35, "Single Dummy");
            AddButton(200, 40, !m_ShowPairsPage ? 4006 : 4005, !m_ShowPairsPage ? 4007 : 4006, TAB_PAIRS, GumpButtonType.Reply, 0);
            AddLabel(235, 40, !m_ShowPairsPage ? 0 : 0x35, "Opposing Pairs");

            if (m_ShowPairsPage)
            {
                BuildOpposingPairsPage();
                return;
            }

            // Single dummy page
            AddLabel(220, 65, 0x3B2, "Configure class, faction, and religion");

            // Divider
            AddImageTiled(20, 55, 610, 2, 9151);

            int y = 65;

            // === COMBAT MODE SELECTION ===
            AddLabel(20, y, 0x480, "Combat Mode:");
            y += 20;

            int modeX = 30;
            AddModeButton(modeX, y, CombatDummyMode.Passive, "Passive", 0x3B2);
            AddModeButton(modeX + 120, y, CombatDummyMode.Melee, "Melee", 0x35);
            AddModeButton(modeX + 240, y, CombatDummyMode.Caster, "Caster", 0x5);
            AddModeButton(modeX + 360, y, CombatDummyMode.Hybrid, "Hybrid", 0x22);
            y += 30;

            // Divider
            AddImageTiled(20, y, 610, 2, 9151);
            y += 10;

            // === CLASS SELECTION ===
            AddLabel(20, y, 0x480, "Class (determines skills & appearance):");
            y += 20;

            // Row 1 - None + Magic Classes
            int classX = 30;
            AddClassButton(classX, y, PlayerClassTypeV2.None, "None");
            AddClassButton(classX + 105, y, PlayerClassTypeV2.IceMage, "Ice Mage");
            AddClassButton(classX + 210, y, PlayerClassTypeV2.Warlock, "Warlock");
            AddClassButton(classX + 315, y, PlayerClassTypeV2.Necromancer, "Necro");
            AddClassButton(classX + 420, y, PlayerClassTypeV2.Druid, "Druid");
            AddClassButton(classX + 525, y, PlayerClassTypeV2.Sorcerer, "Sorcerer");
            y += 22;

            // Row 2 - More Magic
            AddClassButton(classX, y, PlayerClassTypeV2.Bard, "Bard");
            AddClassButton(classX + 105, y, PlayerClassTypeV2.Witch, "Witch");
            AddClassButton(classX + 210, y, PlayerClassTypeV2.Oracle, "Oracle");
            AddClassButton(classX + 315, y, PlayerClassTypeV2.Summoner, "Summoner");
            AddClassButton(classX + 420, y, PlayerClassTypeV2.Shaman, "Shaman");
            AddClassButton(classX + 525, y, PlayerClassTypeV2.Wizard, "Wizard");
            y += 22;

            // Row 3 - Martial Classes
            AddClassButton(classX, y, PlayerClassTypeV2.Barbarian, "Barbarian");
            AddClassButton(classX + 105, y, PlayerClassTypeV2.Rogue, "Rogue");
            AddClassButton(classX + 210, y, PlayerClassTypeV2.Monk, "Monk");
            AddClassButton(classX + 315, y, PlayerClassTypeV2.Knight, "Knight");
            AddClassButton(classX + 420, y, PlayerClassTypeV2.Paladin, "Paladin");
            AddClassButton(classX + 525, y, PlayerClassTypeV2.Ranger, "Ranger");
            y += 22;

            // Row 4 - More Martial
            AddClassButton(classX, y, PlayerClassTypeV2.Fighter, "Fighter");
            AddClassButton(classX + 105, y, PlayerClassTypeV2.Templar, "Templar");
            AddClassButton(classX + 210, y, PlayerClassTypeV2.BountyHunter, "B.Hunter");
            AddClassButton(classX + 315, y, PlayerClassTypeV2.Beastmaster, "Beastmstr");
            AddClassButton(classX + 420, y, PlayerClassTypeV2.Artificer, "Artificer");
            AddClassButton(classX + 525, y, PlayerClassTypeV2.Alchemist, "Alchemist");
            y += 22;

            // Row 5 - Remaining
            AddClassButton(classX, y, PlayerClassTypeV2.Cleric, "Cleric");
            AddClassButton(classX + 105, y, PlayerClassTypeV2.Enchanter, "Enchanter");
            AddClassButton(classX + 210, y, PlayerClassTypeV2.Illusionist, "Illusionist");
            y += 30;

            // Divider
            AddImageTiled(20, y, 610, 2, 9151);
            y += 10;

            // === FACTION SELECTION ===
            AddLabel(20, y, 0x480, "Faction (determines magic school & enemies):");
            y += 20;

            int factionX = 30;
            AddFactionButton(factionX, y, VystiaFaction.None, "None", 0x3B2);
            AddFactionButton(factionX + 105, y, VystiaFaction.Frostguard, "Frostguard", 0x480);
            AddFactionButton(factionX + 210, y, VystiaFaction.FlameLegion, "FlameLegion", 0x489);
            AddFactionButton(factionX + 315, y, VystiaFaction.Greenward, "Greenward", 0x59B);
            y += 22;

            AddFactionButton(factionX, y, VystiaFaction.ArcaneConclave, "Arcane", 0x47E);
            AddFactionButton(factionX + 105, y, VystiaFaction.Technoguild, "Technoguild", 0x47F);
            AddFactionButton(factionX + 210, y, VystiaFaction.Sandwalkers, "Sandwalkers", 0x6B7);
            AddFactionButton(factionX + 315, y, VystiaFaction.Voidborn, "Voidborn", 0x455);
            y += 30;

            // Divider
            AddImageTiled(20, y, 610, 2, 9151);
            y += 10;

            // === RELIGION SELECTION ===
            AddLabel(20, y, 0x480, "Religion (determines magic school & enemies):");
            y += 20;

            int religionX = 30;
            AddReligionButton(religionX, y, VystiaReligion.None, "None", 0x3B2);
            AddReligionButton(religionX + 120, y, VystiaReligion.FrosthelmFaith, "Frostfather", 0x480);
            AddReligionButton(religionX + 240, y, VystiaReligion.SuryasSandscript, "Emberheart", 0x489);
            AddReligionButton(religionX + 360, y, VystiaReligion.LunarasCovenant, "Greenward", 0x59B);
            y += 22;

            AddReligionButton(religionX, y, VystiaReligion.CelestisArcanum, "Crystalline", 0x47E);
            AddReligionButton(religionX + 120, y, VystiaReligion.OceanasCovenant, "VoidWalker", 0x455);
            AddReligionButton(religionX + 240, y, VystiaReligion.CogsmithCreed, "CogsmithCreed", 0x47F);
            y += 30;

            // Divider
            AddImageTiled(20, y, 610, 2, 9151);
            y += 10;

            // === CURRENT SELECTION SUMMARY ===
            AddLabel(20, y, 0x480, "Current Selection:");
            y += 20;

            int summaryHue = 0x35;
            AddLabel(30, y, summaryHue, $"Mode: {m_SelectedMode}");
            AddLabel(180, y, summaryHue, $"Class: {(m_SelectedClass == PlayerClassTypeV2.None ? "Generic" : m_SelectedClass.ToString())}");
            y += 18;
            AddLabel(30, y, summaryHue, $"Faction: {(m_SelectedFaction == VystiaFaction.None ? "Unaffiliated" : m_SelectedFaction.ToString())}");
            AddLabel(250, y, summaryHue, $"Religion: {(m_SelectedReligion == VystiaReligion.None ? "Secular" : m_SelectedReligion.ToString())}");
            y += 30;

            // Divider
            AddImageTiled(20, y, 610, 2, 9151);
            y += 10;

            // === SPAWN BUTTONS ===
            AddLabel(20, y, 0x480, "Actions:");
            y += 25;

            // Spawn single dummy
            AddButton(30, y, 4005, 4007, SPAWN_BUTTON, GumpButtonType.Reply, 0);
            AddLabel(65, y, 0x35, "Spawn Combat Dummy (with maxed skills)");
            y += 25;

            // Spawn all 4 modes
            AddButton(30, y, 4005, 4007, SPAWN_ALL_MODES, GumpButtonType.Reply, 0);
            AddLabel(65, y, 0, "Spawn All 4 Modes (Passive, Melee, Caster, Hybrid)");
            y += 30;

            // Back button
            AddButton(30, y, 4005, 4007, BACK_BUTTON, GumpButtonType.Reply, 0);
            AddLabel(65, y, 0x3B2, "Back to VTK Main Menu");
            y += 35;

            // Info text
            AddLabel(20, y, 0x3B2, "Skills are automatically maxed based on class:");
            y += 18;
            AddLabel(30, y, 0, "- Martial classes: Swords, Tactics, Anatomy, Wrestling, Parrying, Healing");
            y += 16;
            AddLabel(30, y, 0, "- Magic classes: Magery, EvalInt, MagicResist, Meditation + regional skill");
            y += 16;
            AddLabel(30, y, 0, "- Generic (None): All combat skills at 100");
        }

        private void AddModeButton(int x, int y, CombatDummyMode mode, string label, int hue)
        {
            int buttonId = MODE_START + (int)mode;
            bool selected = m_SelectedMode == mode;

            AddButton(x, y, selected ? 9027 : 9026, selected ? 9027 : 9026, buttonId, GumpButtonType.Reply, 0);
            AddLabel(x + 20, y, selected ? hue : 0, label);
        }

        private void AddClassButton(int x, int y, PlayerClassTypeV2 classType, string label)
        {
            int buttonId = CLASS_START + (int)classType;
            bool selected = m_SelectedClass == classType;

            AddButton(x, y, selected ? 9027 : 9026, selected ? 9027 : 9026, buttonId, GumpButtonType.Reply, 0);
            AddLabel(x + 18, y, selected ? 0x35 : 0, label);
        }

        private void AddFactionButton(int x, int y, VystiaFaction faction, string label, int hue)
        {
            int buttonId = FACTION_START + (int)faction;
            bool selected = m_SelectedFaction == faction;

            AddButton(x, y, selected ? 9027 : 9026, selected ? 9027 : 9026, buttonId, GumpButtonType.Reply, 0);
            AddLabel(x + 18, y, selected ? hue : 0, label);
        }

        private void AddReligionButton(int x, int y, VystiaReligion religion, string label, int hue)
        {
            int buttonId = RELIGION_START + (int)religion;
            bool selected = m_SelectedReligion == religion;

            AddButton(x, y, selected ? 9027 : 9026, selected ? 9027 : 9026, buttonId, GumpButtonType.Reply, 0);
            AddLabel(x + 18, y, selected ? hue : 0, label);
        }

        private void BuildOpposingPairsPage()
        {
            int y = 75;

            AddLabel(220, 65, 0x3B2, "Configure opposing pairs with separate classes");

            // Divider
            AddImageTiled(20, y, 610, 2, 9151);
            y += 10;

            // === COMBAT MODE SELECTION ===
            AddLabel(20, y, 0x480, "Combat Mode (for both dummies):");
            y += 20;

            int modeX = 30;
            AddButton(modeX, y, m_PairMode == CombatDummyMode.Passive ? 9027 : 9026, m_PairMode == CombatDummyMode.Passive ? 9027 : 9026, PAIR_MODE_START + (int)CombatDummyMode.Passive, GumpButtonType.Reply, 0);
            AddLabel(modeX + 20, y, m_PairMode == CombatDummyMode.Passive ? 0x3B2 : 0, "Passive");
            AddButton(modeX + 120, y, m_PairMode == CombatDummyMode.Melee ? 9027 : 9026, m_PairMode == CombatDummyMode.Melee ? 9027 : 9026, PAIR_MODE_START + (int)CombatDummyMode.Melee, GumpButtonType.Reply, 0);
            AddLabel(modeX + 140, y, m_PairMode == CombatDummyMode.Melee ? 0x35 : 0, "Melee");
            AddButton(modeX + 240, y, m_PairMode == CombatDummyMode.Caster ? 9027 : 9026, m_PairMode == CombatDummyMode.Caster ? 9027 : 9026, PAIR_MODE_START + (int)CombatDummyMode.Caster, GumpButtonType.Reply, 0);
            AddLabel(modeX + 260, y, m_PairMode == CombatDummyMode.Caster ? 0x5 : 0, "Caster");
            AddButton(modeX + 360, y, m_PairMode == CombatDummyMode.Hybrid ? 9027 : 9026, m_PairMode == CombatDummyMode.Hybrid ? 9027 : 9026, PAIR_MODE_START + (int)CombatDummyMode.Hybrid, GumpButtonType.Reply, 0);
            AddLabel(modeX + 380, y, m_PairMode == CombatDummyMode.Hybrid ? 0x22 : 0, "Hybrid");
            y += 30;

            // Divider
            AddImageTiled(20, y, 610, 2, 9151);
            y += 10;

            // === DUMMY 1 CLASS SELECTION ===
            AddLabel(20, y, 0x480, "Dummy 1 Class:");
            y += 20;

            int classX = 30;
            AddClassButton(classX, y, PlayerClassTypeV2.None, "None", PAIR_CLASS1_START, m_PairClass1);
            AddClassButton(classX + 105, y, PlayerClassTypeV2.Barbarian, "Barbarian", PAIR_CLASS1_START, m_PairClass1);
            AddClassButton(classX + 210, y, PlayerClassTypeV2.Fighter, "Fighter", PAIR_CLASS1_START, m_PairClass1);
            AddClassButton(classX + 315, y, PlayerClassTypeV2.Knight, "Knight", PAIR_CLASS1_START, m_PairClass1);
            AddClassButton(classX + 420, y, PlayerClassTypeV2.IceMage, "Ice Mage", PAIR_CLASS1_START, m_PairClass1);
            AddClassButton(classX + 525, y, PlayerClassTypeV2.Sorcerer, "Sorcerer", PAIR_CLASS1_START, m_PairClass1);
            y += 22;

            AddClassButton(classX, y, PlayerClassTypeV2.Warlock, "Warlock", PAIR_CLASS1_START, m_PairClass1);
            AddClassButton(classX + 105, y, PlayerClassTypeV2.Druid, "Druid", PAIR_CLASS1_START, m_PairClass1);
            AddClassButton(classX + 210, y, PlayerClassTypeV2.Ranger, "Ranger", PAIR_CLASS1_START, m_PairClass1);
            AddClassButton(classX + 315, y, PlayerClassTypeV2.Rogue, "Rogue", PAIR_CLASS1_START, m_PairClass1);
            AddClassButton(classX + 420, y, PlayerClassTypeV2.Monk, "Monk", PAIR_CLASS1_START, m_PairClass1);
            AddClassButton(classX + 525, y, PlayerClassTypeV2.Paladin, "Paladin", PAIR_CLASS1_START, m_PairClass1);
            y += 30;

            // Divider
            AddImageTiled(20, y, 610, 2, 9151);
            y += 10;

            // === DUMMY 2 CLASS SELECTION ===
            AddLabel(20, y, 0x480, "Dummy 2 Class:");
            y += 20;

            AddClassButton(classX, y, PlayerClassTypeV2.None, "None", PAIR_CLASS2_START, m_PairClass2);
            AddClassButton(classX + 105, y, PlayerClassTypeV2.Barbarian, "Barbarian", PAIR_CLASS2_START, m_PairClass2);
            AddClassButton(classX + 210, y, PlayerClassTypeV2.Fighter, "Fighter", PAIR_CLASS2_START, m_PairClass2);
            AddClassButton(classX + 315, y, PlayerClassTypeV2.Knight, "Knight", PAIR_CLASS2_START, m_PairClass2);
            AddClassButton(classX + 420, y, PlayerClassTypeV2.IceMage, "Ice Mage", PAIR_CLASS2_START, m_PairClass2);
            AddClassButton(classX + 525, y, PlayerClassTypeV2.Sorcerer, "Sorcerer", PAIR_CLASS2_START, m_PairClass2);
            y += 22;

            AddClassButton(classX, y, PlayerClassTypeV2.Warlock, "Warlock", PAIR_CLASS2_START, m_PairClass2);
            AddClassButton(classX + 105, y, PlayerClassTypeV2.Druid, "Druid", PAIR_CLASS2_START, m_PairClass2);
            AddClassButton(classX + 210, y, PlayerClassTypeV2.Ranger, "Ranger", PAIR_CLASS2_START, m_PairClass2);
            AddClassButton(classX + 315, y, PlayerClassTypeV2.Rogue, "Rogue", PAIR_CLASS2_START, m_PairClass2);
            AddClassButton(classX + 420, y, PlayerClassTypeV2.Monk, "Monk", PAIR_CLASS2_START, m_PairClass2);
            AddClassButton(classX + 525, y, PlayerClassTypeV2.Paladin, "Paladin", PAIR_CLASS2_START, m_PairClass2);
            y += 30;

            // Divider
            AddImageTiled(20, y, 610, 2, 9151);
            y += 10;

            // === CURRENT SELECTION SUMMARY ===
            AddLabel(20, y, 0x480, "Current Selection:");
            y += 20;

            int summaryHue = 0x35;
            AddLabel(30, y, summaryHue, $"Mode: {m_PairMode}");
            AddLabel(180, y, summaryHue, $"Dummy 1: {(m_PairClass1 == PlayerClassTypeV2.None ? "Generic" : m_PairClass1.ToString())}");
            AddLabel(350, y, summaryHue, $"Dummy 2: {(m_PairClass2 == PlayerClassTypeV2.None ? "Generic" : m_PairClass2.ToString())}");
            y += 30;

            // Divider
            AddImageTiled(20, y, 610, 2, 9151);
            y += 10;

            // === OPPOSING PAIRS ===
            AddLabel(20, y, 0x480, "Opposing Pairs (faction/religion are fixed):");
            y += 25;

            // Faction pair: Frostguard vs FlameLegion
            AddButton(30, y, 4005, 4007, SPAWN_FACTION_PAIR, GumpButtonType.Reply, 0);
            AddLabel(65, y, 0x480, "Frostguard vs FlameLegion (Ice vs Fire)");
            y += 25;

            // Religion pair: Frostfather vs Emberheart
            AddButton(30, y, 4005, 4007, SPAWN_RELIGION_PAIR, GumpButtonType.Reply, 0);
            AddLabel(65, y, 0x489, "Frostfather vs Emberheart (Cold vs Fire Religion)");
            y += 25;

            // Void pair: Voidborn vs Greenward
            AddButton(30, y, 4005, 4007, SPAWN_VOID_PAIR, GumpButtonType.Reply, 0);
            AddLabel(65, y, 0x455, "Voidborn vs Greenward (Void vs Nature)");
            y += 25;

            // Magic pair: Arcane vs Technoguild
            AddButton(30, y, 4005, 4007, SPAWN_MAGIC_PAIR, GumpButtonType.Reply, 0);
            AddLabel(65, y, 0x47E, "Arcane vs Technoguild (Magic vs Technology)");
            y += 30;

            // Back button
            AddButton(30, y, 4005, 4007, BACK_BUTTON, GumpButtonType.Reply, 0);
            AddLabel(65, y, 0x3B2, "Back to VTK Main Menu");
        }

        private void AddClassButton(int x, int y, PlayerClassTypeV2 classType, string label, int buttonStart, PlayerClassTypeV2 selectedClass)
        {
            int buttonId = buttonStart + (int)classType;
            bool selected = selectedClass == classType;

            AddButton(x, y, selected ? 9027 : 9026, selected ? 9027 : 9026, buttonId, GumpButtonType.Reply, 0);
            AddLabel(x + 18, y, selected ? 0x35 : 0, label);
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            if (m_Player == null || m_Player.Deleted)
                return;

            int buttonId = info.ButtonID;

            // Mode selection (100-103) - single dummy page only
            if (buttonId >= MODE_START && buttonId <= MODE_START + 3)
            {
                m_SelectedMode = (CombatDummyMode)(buttonId - MODE_START);
                m_Player.SendGump(new CombatDummyConfigGump(m_Player, false, m_SelectedMode, m_SelectedClass, m_SelectedFaction, m_SelectedReligion, m_PairMode, m_PairClass1, m_PairClass2));
                return;
            }

            // Tab switching
            if (buttonId == TAB_SINGLE)
            {
                m_Player.SendGump(new CombatDummyConfigGump(m_Player, false, m_SelectedMode, m_SelectedClass, m_SelectedFaction, m_SelectedReligion, m_PairMode, m_PairClass1, m_PairClass2));
                return;
            }
            if (buttonId == TAB_PAIRS)
            {
                m_Player.SendGump(new CombatDummyConfigGump(m_Player, true, m_SelectedMode, m_SelectedClass, m_SelectedFaction, m_SelectedReligion, m_PairMode, m_PairClass1, m_PairClass2));
                return;
            }

            // Opposing pairs page - Mode selection (700-703)
            if (buttonId >= PAIR_MODE_START && buttonId <= PAIR_MODE_START + 3)
            {
                m_PairMode = (CombatDummyMode)(buttonId - PAIR_MODE_START);
                m_Player.SendGump(new CombatDummyConfigGump(m_Player, true, m_SelectedMode, m_SelectedClass, m_SelectedFaction, m_SelectedReligion, m_PairMode, m_PairClass1, m_PairClass2));
                return;
            }

            // Opposing pairs page - Dummy 1 class selection (800-825)
            if (buttonId >= PAIR_CLASS1_START && buttonId <= PAIR_CLASS1_START + 25)
            {
                m_PairClass1 = (PlayerClassTypeV2)(buttonId - PAIR_CLASS1_START);
                m_Player.SendGump(new CombatDummyConfigGump(m_Player, true, m_SelectedMode, m_SelectedClass, m_SelectedFaction, m_SelectedReligion, m_PairMode, m_PairClass1, m_PairClass2));
                return;
            }

            // Opposing pairs page - Dummy 2 class selection (900-925)
            if (buttonId >= PAIR_CLASS2_START && buttonId <= PAIR_CLASS2_START + 25)
            {
                m_PairClass2 = (PlayerClassTypeV2)(buttonId - PAIR_CLASS2_START);
                m_Player.SendGump(new CombatDummyConfigGump(m_Player, true, m_SelectedMode, m_SelectedClass, m_SelectedFaction, m_SelectedReligion, m_PairMode, m_PairClass1, m_PairClass2));
                return;
            }

            // Class selection (200-225) - single dummy page only
            if (buttonId >= CLASS_START && buttonId <= CLASS_START + 25)
            {
                m_SelectedClass = (PlayerClassTypeV2)(buttonId - CLASS_START);
                m_Player.SendGump(new CombatDummyConfigGump(m_Player, false, m_SelectedMode, m_SelectedClass, m_SelectedFaction, m_SelectedReligion, m_PairMode, m_PairClass1, m_PairClass2));
                return;
            }

            // Faction selection (300-307) - single dummy page only
            if (buttonId >= FACTION_START && buttonId <= FACTION_START + 7)
            {
                m_SelectedFaction = (VystiaFaction)(buttonId - FACTION_START);
                m_Player.SendGump(new CombatDummyConfigGump(m_Player, false, m_SelectedMode, m_SelectedClass, m_SelectedFaction, m_SelectedReligion, m_PairMode, m_PairClass1, m_PairClass2));
                return;
            }

            // Religion selection (400-406) - single dummy page only
            if (buttonId >= RELIGION_START && buttonId <= RELIGION_START + 6)
            {
                m_SelectedReligion = (VystiaReligion)(buttonId - RELIGION_START);
                m_Player.SendGump(new CombatDummyConfigGump(m_Player, false, m_SelectedMode, m_SelectedClass, m_SelectedFaction, m_SelectedReligion, m_PairMode, m_PairClass1, m_PairClass2));
                return;
            }

            // Spawn single dummy
            if (buttonId == SPAWN_BUTTON)
            {
                SpawnDummy(m_SelectedMode);
                m_Player.SendGump(new CombatDummyConfigGump(m_Player, false, m_SelectedMode, m_SelectedClass, m_SelectedFaction, m_SelectedReligion, m_PairMode, m_PairClass1, m_PairClass2));
                return;
            }

            // Spawn all 4 modes
            if (buttonId == SPAWN_ALL_MODES)
            {
                int offset = 0;
                foreach (CombatDummyMode mode in Enum.GetValues(typeof(CombatDummyMode)))
                {
                    SpawnDummyAtOffset(mode, offset);
                    offset += 2;
                }
                m_Player.SendMessage(0x35, "Spawned 4 combat dummies (all modes) with current class/faction/religion settings.");
                m_Player.SendGump(new CombatDummyConfigGump(m_Player, false, m_SelectedMode, m_SelectedClass, m_SelectedFaction, m_SelectedReligion, m_PairMode, m_PairClass1, m_PairClass2));
                return;
            }

            // Spawn opposing faction pair: Frostguard vs FlameLegion
            if (buttonId == SPAWN_FACTION_PAIR)
            {
                SpawnOpposingPair(
                    VystiaFaction.Frostguard, VystiaReligion.None,
                    VystiaFaction.FlameLegion, VystiaReligion.None,
                    "Frostguard", "FlameLegion"
                );
                m_Player.SendGump(new CombatDummyConfigGump(m_Player, true, m_SelectedMode, m_SelectedClass, m_SelectedFaction, m_SelectedReligion, m_PairMode, m_PairClass1, m_PairClass2));
                return;
            }

            // Spawn opposing religion pair: Frostfather vs Emberheart
            if (buttonId == SPAWN_RELIGION_PAIR)
            {
                SpawnOpposingPair(
                    VystiaFaction.None, VystiaReligion.FrosthelmFaith,
                    VystiaFaction.None, VystiaReligion.SuryasSandscript,
                    "Frosthelm Faith", "Surya's Sandscript"
                );
                m_Player.SendGump(new CombatDummyConfigGump(m_Player, true, m_SelectedMode, m_SelectedClass, m_SelectedFaction, m_SelectedReligion, m_PairMode, m_PairClass1, m_PairClass2));
                return;
            }

            // Spawn void pair: Voidborn vs Greenward
            if (buttonId == SPAWN_VOID_PAIR)
            {
                SpawnOpposingPair(
                    VystiaFaction.Voidborn, VystiaReligion.None,
                    VystiaFaction.Greenward, VystiaReligion.None,
                    "Voidborn", "Greenward"
                );
                m_Player.SendGump(new CombatDummyConfigGump(m_Player, true, m_SelectedMode, m_SelectedClass, m_SelectedFaction, m_SelectedReligion, m_PairMode, m_PairClass1, m_PairClass2));
                return;
            }

            // Spawn magic pair: Arcane vs Technoguild
            if (buttonId == SPAWN_MAGIC_PAIR)
            {
                SpawnOpposingPair(
                    VystiaFaction.ArcaneConclave, VystiaReligion.None,
                    VystiaFaction.Technoguild, VystiaReligion.None,
                    "Arcane Conclave", "Technoguild"
                );
                m_Player.SendGump(new CombatDummyConfigGump(m_Player, true, m_SelectedMode, m_SelectedClass, m_SelectedFaction, m_SelectedReligion, m_PairMode, m_PairClass1, m_PairClass2));
                return;
            }

            // Back button
            if (buttonId == BACK_BUTTON)
            {
                m_Player.SendGump(new VystiaTestKitGump(m_Player, false));
                return;
            }
        }

        private void SpawnDummy(CombatDummyMode mode)
        {
            VystiaCombatDummy dummy = new VystiaCombatDummy(m_SelectedFaction, m_SelectedReligion, mode, m_SelectedClass);
            dummy.MoveToWorld(m_Player.Location, m_Player.Map);

            string className = m_SelectedClass == PlayerClassTypeV2.None ? "Generic" : m_SelectedClass.ToString();
            string factionName = m_SelectedFaction == VystiaFaction.None ? "None" : m_SelectedFaction.ToString();
            string religionName = m_SelectedReligion == VystiaReligion.None ? "None" : m_SelectedReligion.ToString();

            m_Player.SendMessage(0x35, $"Spawned Combat Dummy [{mode}] - Class: {className}, Faction: {factionName}, Religion: {religionName}");
        }

        private void SpawnDummyAtOffset(CombatDummyMode mode, int offset)
        {
            VystiaCombatDummy dummy = new VystiaCombatDummy(m_SelectedFaction, m_SelectedReligion, mode, m_SelectedClass);

            // Spawn in front of player with offset
            int x = m_Player.X;
            int y = m_Player.Y;

            switch (m_Player.Direction & Direction.Mask)
            {
                case Direction.North: y -= 2; x += offset; break;
                case Direction.South: y += 2; x += offset; break;
                case Direction.East: x += 2; y += offset; break;
                case Direction.West: x -= 2; y += offset; break;
                case Direction.Up: x -= 2; y -= 2; x += offset; break;
                case Direction.Down: x += 2; y += 2; x += offset; break;
                case Direction.Left: x -= 2; y += 2; x += offset; break;
                case Direction.Right: x += 2; y -= 2; x += offset; break;
                default: y += offset; break;
            }

            dummy.MoveToWorld(new Point3D(x, y, m_Player.Z), m_Player.Map);
        }

        /// <summary>
        /// Spawn two opposing dummies that will fight each other
        /// </summary>
        private void SpawnOpposingPair(
            VystiaFaction faction1, VystiaReligion religion1,
            VystiaFaction faction2, VystiaReligion religion2,
            string name1, string name2)
        {
            // Use Hybrid mode if Passive is selected (so they can fight)
            CombatDummyMode spawnMode = m_PairMode == CombatDummyMode.Passive ? CombatDummyMode.Hybrid : m_PairMode;

            // Spawn first dummy with its selected class
            VystiaCombatDummy dummy1 = new VystiaCombatDummy(faction1, religion1, spawnMode, m_PairClass1);
            Point3D loc1 = GetSpawnLocationInFront(m_Player, 2);
            dummy1.MoveToWorld(loc1, m_Player.Map);

            // Spawn second dummy with its selected class
            VystiaCombatDummy dummy2 = new VystiaCombatDummy(faction2, religion2, spawnMode, m_PairClass2);
            Point3D loc2 = GetSpawnLocationInFront(m_Player, 4);
            dummy2.MoveToWorld(loc2, m_Player.Map);

            // Make them attack each other
            dummy1.Combatant = dummy2;
            dummy2.Combatant = dummy1;

            string class1Name = m_PairClass1 == PlayerClassTypeV2.None ? "Generic" : m_PairClass1.ToString();
            string class2Name = m_PairClass2 == PlayerClassTypeV2.None ? "Generic" : m_PairClass2.ToString();
            string modeText = m_PairMode == CombatDummyMode.Passive ? "Hybrid (auto-upgraded from Passive)" : spawnMode.ToString();
            m_Player.SendMessage(0x35, $"Spawned opposing pair: {name1} vs {name2}");
            m_Player.SendMessage(0x22, $"Dummy 1: {class1Name}, Dummy 2: {class2Name}, Mode: {modeText}");
            m_Player.SendMessage(0x22, "Dummies are now fighting each other!");
        }

        /// <summary>
        /// Get spawn location in front of player at specified distance
        /// </summary>
        private Point3D GetSpawnLocationInFront(PlayerMobile player, int distance)
        {
            int x = player.X;
            int y = player.Y;

            switch (player.Direction & Direction.Mask)
            {
                case Direction.North: y -= distance; break;
                case Direction.South: y += distance; break;
                case Direction.East: x += distance; break;
                case Direction.West: x -= distance; break;
                case Direction.Up: x -= distance; y -= distance; break;
                case Direction.Down: x += distance; y += distance; break;
                case Direction.Left: x -= distance; y += distance; break;
                case Direction.Right: x += distance; y -= distance; break;
            }

            return new Point3D(x, y, player.Z);
        }
    }
}

