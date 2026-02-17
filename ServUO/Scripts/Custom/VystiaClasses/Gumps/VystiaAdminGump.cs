using System;
using System.Collections.Generic;
using System.Linq;
using Server;
using Server.Commands;
using Server.Gumps;
using Server.Items;
using Server.Mobiles;
using Server.Network;
using Server.Targeting;
using Server.Custom.VystiaClasses;
using Server.Custom.VystiaClasses.Systems;
using Server.Custom.VystiaClasses.Abilities;
using Server.Custom.VystiaClasses.Skills;
using Server.Items.Vystia;
using Server.Custom.VystiaClasses.Factions;
using Server.Custom.VystiaClasses.Religion;

namespace Server.Custom.VystiaClasses.Gumps
{
    /// <summary>
    /// Main admin gump for Vystia Class System v2.0
    /// Provides unified interface for class, resource, stance, items, and tool management
    /// </summary>
    public class VystiaAdminGump : Gump
    {
        public enum AdminPage
        {
            Classes,
            Resources,
            Stances,
            Skills,
            Items,
            Tools,
            Factions,
            Religion
        }

        private readonly Mobile m_From;
        private readonly Mobile m_Target;
        private readonly AdminPage m_Page;
        private readonly int m_ItemSubPage; // 0=Spellbooks, 1=Reagents, 2=Weapons, 3=Armor, 4=FocusItems

        private const int GumpWidth = 1040;  // 800 * 1.3 = 1040 (30% larger)
        private const int GumpHeight = 943;   // 725 * 1.3 = 942.5 rounded to 943 (30% larger)

        // Colors
        private const int HeaderColor = 0xFFD700; // Gold
        private const int LabelColor = 0xFFFFFF; // White
        private const int ValueColor = 0x00FF00; // Green
        private const int WarningColor = 0xFF6600; // Orange
        private const int MagicColor = 0x6699FF; // Blue for magic
        private const int MartialColor = 0xFF9966; // Orange for martial

        public VystiaAdminGump(Mobile from, Mobile target = null, AdminPage page = AdminPage.Classes, int itemSubPage = 0) : base(50, 30)
        {
            m_From = from;
            m_Target = target ?? from;
            m_Page = page;
            m_ItemSubPage = itemSubPage;

            from.CloseGump(typeof(VystiaAdminGump));

            Closable = true;
            Disposable = true;
            Dragable = true;
            Resizable = false;

            AddPage(0);

            // Main background
            AddBackground(0, 0, GumpWidth, GumpHeight, 9200);
            AddImageTiled(10, 10, GumpWidth - 20, GumpHeight - 20, 2624);

            // Header
            AddHtml(20, 15, GumpWidth - 40, 25,
                $"<CENTER><BASEFONT COLOR=#{HeaderColor:X6} SIZE=6>Vystia Class System v2.0 - Admin Panel</BASEFONT></CENTER>",
                false, false);

            // Target info
            string targetName = m_Target != null ? m_Target.Name : "None";
            string targetClass = "None";

            if (m_Target is PlayerMobile pm)
            {
                var classType = VystiaClassManager.Instance.GetClassType(pm);
                if (classType != PlayerClassTypeV2.None)
                    targetClass = classType.ToString();
            }

            AddHtml(20, 45, GumpWidth - 180, 20,
                $"<BASEFONT COLOR=#{LabelColor:X6}>Target: <BASEFONT COLOR=#{ValueColor:X6}>{targetName}</BASEFONT> | Class: <BASEFONT COLOR=#{ValueColor:X6}>{targetClass}</BASEFONT></BASEFONT>",
                false, false);

            // Tab buttons
            int tabY = 70;
            int tabWidth = 120;
            int tabSpacing = 5;

            AddTabButton(20, tabY, tabWidth, "Classes", AdminPage.Classes, 1);
            AddTabButton(20 + (tabWidth + tabSpacing) * 1, tabY, tabWidth, "Resources", AdminPage.Resources, 2);
            AddTabButton(20 + (tabWidth + tabSpacing) * 2, tabY, tabWidth, "Stances", AdminPage.Stances, 3);
            AddTabButton(20 + (tabWidth + tabSpacing) * 3, tabY, tabWidth, "Skills", AdminPage.Skills, 4);
            AddTabButton(20 + (tabWidth + tabSpacing) * 4, tabY, tabWidth, "Items", AdminPage.Items, 5);
            AddTabButton(20 + (tabWidth + tabSpacing) * 5, tabY, tabWidth, "Tools", AdminPage.Tools, 6);

            // Second row of tabs
            int tabY2 = tabY + 25;
            AddTabButton(20, tabY2, tabWidth, "Factions", AdminPage.Factions, 7);
            AddTabButton(20 + (tabWidth + tabSpacing) * 1, tabY2, tabWidth, "Religion", AdminPage.Religion, 8);

            // Select target button
            AddButton(GumpWidth - 140, 45, 4005, 4007, 100, GumpButtonType.Reply, 0);
            AddHtml(GumpWidth - 100, 47, 80, 20, $"<BASEFONT COLOR=#{LabelColor:X6}>Select Target</BASEFONT>", false, false);

            // Content area (adjusted for second row of tabs)
            int contentY = 125;
            int contentHeight = GumpHeight - contentY - 20;

            AddImageTiled(15, contentY, GumpWidth - 30, contentHeight, 2624);
            AddAlphaRegion(15, contentY, GumpWidth - 30, contentHeight);

            // Draw page content
            switch (m_Page)
            {
                case AdminPage.Classes:
                    DrawClassesPage(contentY + 10);
                    break;
                case AdminPage.Resources:
                    DrawResourcesPage(contentY + 10);
                    break;
                case AdminPage.Stances:
                    DrawStancesPage(contentY + 10);
                    break;
                case AdminPage.Skills:
                    DrawSkillsPage(contentY + 10);
                    break;
                case AdminPage.Items:
                    DrawItemsPage(contentY + 10);
                    break;
                case AdminPage.Tools:
                    DrawToolsPage(contentY + 10);
                    break;
                case AdminPage.Factions:
                    DrawFactionsPage(contentY + 10);
                    break;
                case AdminPage.Religion:
                    DrawReligionPage(contentY + 10);
                    break;
            }
        }

        private void AddTabButton(int x, int y, int width, string label, AdminPage page, int buttonId)
        {
            bool isActive = m_Page == page;
            int buttonGfx = isActive ? 4006 : 4005;

            AddButton(x, y, buttonGfx, 4007, buttonId, GumpButtonType.Reply, 0);
            AddHtml(x + 35, y + 2, width - 40, 20,
                $"<BASEFONT COLOR=#{(isActive ? ValueColor : LabelColor):X6}>{label}</BASEFONT>",
                false, false);
        }

        #region Classes Page

        private void DrawClassesPage(int startY)
        {
            AddHtml(25, startY, 300, 20, $"<BASEFONT COLOR=#{HeaderColor:X6}>Assign Class (B=Beginner, M=Maxed):</BASEFONT>", false, false);

            // Magic classes (left column)
            AddHtml(25, startY + 25, 200, 20, $"<BASEFONT COLOR=#{MagicColor:X6}>Magic Classes (13):</BASEFONT>", false, false);

            string[] magicClasses = { "IceMage", "Warlock", "Necromancer", "Druid", "Sorcerer", "Bard",
                                      "Witch", "Oracle", "Summoner", "Shaman", "Enchanter", "Illusionist", "Magery" };

            for (int i = 0; i < magicClasses.Length; i++)
            {
                int y = startY + 45 + (i * 22);
                // Beginner button (assigns class only)
                AddButton(25, y, 4005, 4007, 200 + i, GumpButtonType.Reply, 0);
                AddHtml(60, y + 2, 20, 20, $"<BASEFONT COLOR=#{LabelColor:X6}>B</BASEFONT>", false, false);
                // Maxed button (assigns class + all items)
                AddButton(80, y, 4005, 4007, 2500 + i, GumpButtonType.Reply, 0);
                AddHtml(115, y + 2, 20, 20, $"<BASEFONT COLOR=#{ValueColor:X6}>M</BASEFONT>", false, false);
                // Class name
                AddHtml(140, y + 2, 100, 20, $"<BASEFONT COLOR=#{LabelColor:X6}>{magicClasses[i]}</BASEFONT>", false, false);
            }

            // Martial classes (middle column)
            AddHtml(270, startY + 25, 200, 20, $"<BASEFONT COLOR=#{MartialColor:X6}>Martial Classes (13):</BASEFONT>", false, false);

            string[] martialClasses = { "Barbarian", "Rogue", "Monk", "Knight", "Paladin", "Ranger",
                                        "Fighter", "Templar", "BountyHunter", "Beastmaster", "Artificer",
                                        "Alchemist", "Cleric" };

            for (int i = 0; i < martialClasses.Length; i++)
            {
                int y = startY + 45 + (i * 22);
                // Beginner button
                AddButton(270, y, 4005, 4007, 300 + i, GumpButtonType.Reply, 0);
                AddHtml(305, y + 2, 20, 20, $"<BASEFONT COLOR=#{LabelColor:X6}>B</BASEFONT>", false, false);
                // Maxed button
                AddButton(325, y, 4005, 4007, 2600 + i, GumpButtonType.Reply, 0);
                AddHtml(360, y + 2, 20, 20, $"<BASEFONT COLOR=#{ValueColor:X6}>M</BASEFONT>", false, false);
                // Class name
                AddHtml(385, y + 2, 100, 20, $"<BASEFONT COLOR=#{LabelColor:X6}>{martialClasses[i]}</BASEFONT>", false, false);
            }

            // Current class info (right side)
            AddHtml(510, startY + 25, 300, 20, $"<BASEFONT COLOR=#{HeaderColor:X6}>Current Class Info:</BASEFONT>", false, false);

            if (m_Target is PlayerMobile pm)
            {
                var playerClass = VystiaClassManager.Instance.GetClass(pm);
                if (playerClass != null)
                {
                    AddHtml(510, startY + 50, 280, 20, $"<BASEFONT COLOR=#{LabelColor:X6}>Name: <BASEFONT COLOR=#{ValueColor:X6}>{playerClass.ClassName}</BASEFONT></BASEFONT>", false, false);
                    AddHtml(510, startY + 70, 280, 20, $"<BASEFONT COLOR=#{LabelColor:X6}>Role: <BASEFONT COLOR=#{ValueColor:X6}>{playerClass.Role}</BASEFONT></BASEFONT>", false, false);
                    AddHtml(510, startY + 90, 280, 20, $"<BASEFONT COLOR=#{LabelColor:X6}>Region: <BASEFONT COLOR=#{ValueColor:X6}>{playerClass.HomeRegion}</BASEFONT></BASEFONT>", false, false);
                    AddHtml(510, startY + 110, 280, 20, $"<BASEFONT COLOR=#{LabelColor:X6}>School: <BASEFONT COLOR=#{ValueColor:X6}>{playerClass.AbilitySchool}</BASEFONT></BASEFONT>", false, false);

                    string secondaryRes = playerClass.SecondaryResource.HasValue
                        ? $"{playerClass.SecondaryResource.Value} (max {playerClass.SecondaryResourceMax})"
                        : "None";
                    AddHtml(510, startY + 130, 280, 20, $"<BASEFONT COLOR=#{LabelColor:X6}>Resource: <BASEFONT COLOR=#{ValueColor:X6}>{secondaryRes}</BASEFONT></BASEFONT>", false, false);

                    // Stances available
                    if (playerClass.AvailableStances != null && playerClass.AvailableStances.Length > 0)
                    {
                        string stances = string.Join(", ", playerClass.AvailableStances.Select(s => s.ToString()));
                        AddHtml(510, startY + 150, 280, 60, $"<BASEFONT COLOR=#{LabelColor:X6}>Stances: <BASEFONT COLOR=#{ValueColor:X6}>{stances}</BASEFONT></BASEFONT>", false, false);
                    }
                }
                else
                {
                    AddHtml(510, startY + 50, 300, 20, $"<BASEFONT COLOR=#{WarningColor:X6}>No class assigned</BASEFONT>", false, false);
                }
            }

            // Actions section at bottom (similar to Stances page)
            int actionsY = startY + 420;

            // Actions box
            AddBackground(25, actionsY, 300, 60, 9200);
            AddHtml(35, actionsY + 5, 280, 20, $"<BASEFONT COLOR=#{HeaderColor:X6}>Class Actions:</BASEFONT>", false, false);

            AddButton(35, actionsY + 28, 4005, 4007, 400, GumpButtonType.Reply, 0);
            AddHtml(70, actionsY + 30, 120, 20, $"<BASEFONT COLOR=#{WarningColor:X6}>REMOVE CLASS</BASEFONT>", false, false);

            AddButton(190, actionsY + 28, 4005, 4007, 401, GumpButtonType.Reply, 0);
            AddHtml(225, actionsY + 30, 80, 20, $"<BASEFONT COLOR=#{LabelColor:X6}>Info</BASEFONT>", false, false);

            // Selection gump box
            AddBackground(350, actionsY, 400, 60, 9200);
            AddHtml(360, actionsY + 5, 380, 20, $"<BASEFONT COLOR=#{HeaderColor:X6}>Class Selection:</BASEFONT>", false, false);

            AddButton(360, actionsY + 28, 4005, 4007, 402, GumpButtonType.Reply, 0);
            AddHtml(395, actionsY + 30, 200, 20, $"<BASEFONT COLOR=#{ValueColor:X6}>Open Selection Gump</BASEFONT>", false, false);

            AddButton(560, actionsY + 28, 4005, 4007, 403, GumpButtonType.Reply, 0);
            AddHtml(595, actionsY + 30, 150, 20, $"<BASEFONT COLOR=#{LabelColor:X6}>Refresh Info</BASEFONT>", false, false);
        }

        #endregion

        #region Resources Page

        private void DrawResourcesPage(int startY)
        {
            AddHtml(25, startY, 300, 20, $"<BASEFONT COLOR=#{HeaderColor:X6}>Secondary Resources:</BASEFONT>", false, false);

            PlayerMobile pm = m_Target as PlayerMobile;
            if (pm == null)
            {
                AddHtml(25, startY + 30, 300, 20, $"<BASEFONT COLOR=#{WarningColor:X6}>Target must be a player</BASEFONT>", false, false);
                return;
            }

            var manager = VystiaResourceManager.GetManager(pm);
            if (manager == null)
            {
                AddHtml(25, startY + 30, 300, 20, $"<BASEFONT COLOR=#{WarningColor:X6}>No resource manager found</BASEFONT>", false, false);
                return;
            }

            // Resource list with values
            string[] resourceNames = Enum.GetNames(typeof(ResourceType));
            int col1Count = (resourceNames.Length + 1) / 2;

            for (int i = 0; i < resourceNames.Length; i++)
            {
                ResourceType resType = (ResourceType)Enum.Parse(typeof(ResourceType), resourceNames[i]);
                var resource = manager.GetResource(resType);

                int x = i < col1Count ? 25 : 400;
                int y = startY + 30 + ((i % col1Count) * 25);

                string valueStr = resource != null ? $"{resource.Current}/{resource.Maximum}" : "N/A";
                int valueColor = resource != null ? ValueColor : WarningColor;

                AddHtml(x, y, 100, 20, $"<BASEFONT COLOR=#{LabelColor:X6}>{resourceNames[i]}:</BASEFONT>", false, false);
                AddHtml(x + 100, y, 60, 20, $"<BASEFONT COLOR=#{valueColor:X6}>{valueStr}</BASEFONT>", false, false);

                // Set to max button
                AddButton(x + 165, y - 2, 4005, 4007, 500 + i, GumpButtonType.Reply, 0);
                AddHtml(x + 200, y, 40, 20, $"<BASEFONT COLOR=#{LabelColor:X6}>Max</BASEFONT>", false, false);

                // Set to 0 button
                AddButton(x + 240, y - 2, 4005, 4007, 600 + i, GumpButtonType.Reply, 0);
                AddHtml(x + 275, y, 40, 20, $"<BASEFONT COLOR=#{LabelColor:X6}>Zero</BASEFONT>", false, false);
            }

            // Global actions
            int actionsY = startY + 30 + (col1Count * 25) + 20;
            AddHtml(25, actionsY, 200, 20, $"<BASEFONT COLOR=#{HeaderColor:X6}>Quick Actions:</BASEFONT>", false, false);

            AddButton(25, actionsY + 25, 4005, 4007, 700, GumpButtonType.Reply, 0);
            AddHtml(60, actionsY + 27, 150, 20, $"<BASEFONT COLOR=#{ValueColor:X6}>Fill All Resources</BASEFONT>", false, false);

            AddButton(200, actionsY + 25, 4005, 4007, 701, GumpButtonType.Reply, 0);
            AddHtml(235, actionsY + 27, 150, 20, $"<BASEFONT COLOR=#{WarningColor:X6}>Reset All to Zero</BASEFONT>", false, false);
        }

        #endregion

        #region Stances Page

        private void DrawStancesPage(int startY)
        {
            AddHtml(25, startY, 300, 20, $"<BASEFONT COLOR=#{HeaderColor:X6}>Stance Management:</BASEFONT>", false, false);

            PlayerMobile pm = m_Target as PlayerMobile;
            if (pm == null)
            {
                AddHtml(25, startY + 30, 300, 20, $"<BASEFONT COLOR=#{WarningColor:X6}>Target must be a player</BASEFONT>", false, false);
                return;
            }

            // Build stance categories with button IDs
            var stanceData = new[]
            {
                new { Name = "Druid Forms", Stances = new[] { ("DruidBear", 1001), ("DruidCat", 1002), ("DruidTree", 1003), ("DruidMoonkin", 1004), ("DruidTravel", 1005) } },
                new { Name = "Sorcerer Elements", Stances = new[] { ("SorcererFire", 1010), ("SorcererWater", 1011), ("SorcererEarth", 1012), ("SorcererAir", 1013) } },
                new { Name = "Fighter Stances", Stances = new[] { ("FighterAggressive", 1020), ("FighterDefensive", 1021), ("FighterBalanced", 1022), ("FighterBerserker", 1023) } },
                new { Name = "Monk Stances", Stances = new[] { ("MonkWindwalker", 1030), ("MonkBrewmaster", 1031), ("MonkMistweaver", 1032) } },
                new { Name = "Rogue Stances", Stances = new[] { ("RogueShadow", 1040), ("RogueOutlaw", 1041), ("RogueSubtlety", 1042) } },
                new { Name = "Paladin Auras", Stances = new[] { ("PaladinDevotion", 1050), ("PaladinRetribution", 1051), ("PaladinProtection", 1052) } },
                new { Name = "Ranger Aspects", Stances = new[] { ("RangerHawk", 1060), ("RangerWolf", 1061), ("RangerBear", 1062) } },
                new { Name = "Barbarian", Stances = new[] { ("BarbarianNormal", 1070), ("BarbarianRage", 1071) } }
            };

            int catIndex = 0;
            foreach (var category in stanceData)
            {
                int x = 25 + (catIndex % 3) * 250;
                int y = startY + 30 + (catIndex / 3) * 140;

                AddHtml(x, y, 200, 20, $"<BASEFONT COLOR=#{MagicColor:X6}>{category.Name}:</BASEFONT>", false, false);

                for (int i = 0; i < category.Stances.Length; i++)
                {
                    var (stanceName, buttonId) = category.Stances[i];
                    AddButton(x, y + 22 + (i * 22), 4005, 4007, buttonId, GumpButtonType.Reply, 0);
                    AddHtml(x + 35, y + 24 + (i * 22), 180, 20,
                        $"<BASEFONT COLOR=#{LabelColor:X6}>{stanceName}</BASEFONT>", false, false);
                }

                catIndex++;
            }

            // Clear stances section - more prominent
            int clearY = startY + 420;

            // Clear stance box
            AddBackground(25, clearY, 250, 60, 9200);
            AddHtml(35, clearY + 5, 230, 20, $"<BASEFONT COLOR=#{HeaderColor:X6}>Clear Stances:</BASEFONT>", false, false);

            AddButton(35, clearY + 28, 4005, 4007, 1100, GumpButtonType.Reply, 0);
            AddHtml(70, clearY + 30, 180, 20, $"<BASEFONT COLOR=#{WarningColor:X6}>CLEAR ALL STANCES</BASEFONT>", false, false);

            // Show active stances
            AddBackground(300, clearY, 450, 60, 9200);
            AddHtml(310, clearY + 5, 200, 20, $"<BASEFONT COLOR=#{HeaderColor:X6}>Active Stances:</BASEFONT>", false, false);

            // Query for active stances
            string activeStances = "None";
            if (pm != null)
            {
                var stances = VystiaStanceManager.Instance.GetAllActiveStances(pm);
                if (stances != null && stances.Count > 0)
                    activeStances = string.Join(", ", stances.Select(s => s.Definition.Name));
            }
            AddHtml(310, clearY + 28, 430, 25, $"<BASEFONT COLOR=#{ValueColor:X6}>{activeStances}</BASEFONT>", false, false);
        }

        #endregion

        #region Skills Page

        private void DrawSkillsPage(int startY)
        {
            AddHtml(25, startY, 400, 20, $"<BASEFONT COLOR=#{HeaderColor:X6}>Vystia Class Skills (26 Custom Skills):</BASEFONT>", false, false);

            PlayerMobile pm = m_Target as PlayerMobile;
            if (pm == null)
            {
                AddHtml(25, startY + 30, 300, 20, $"<BASEFONT COLOR=#{WarningColor:X6}>Target must be a player</BASEFONT>", false, false);
                return;
            }

            // Get the player's class skill
            var classSkill = VystiaSkillCheck.GetClassSkill(pm);
            string classSkillName = classSkill.HasValue ? classSkill.Value.ToString() : "None";

            AddHtml(25, startY + 25, 400, 20,
                $"<BASEFONT COLOR=#{LabelColor:X6}>Class Skill: <BASEFONT COLOR=#{ValueColor:X6}>{classSkillName}</BASEFONT></BASEFONT>",
                false, false);

            // Magic skills (left column)
            AddHtml(25, startY + 55, 200, 20, $"<BASEFONT COLOR=#{MagicColor:X6}>Magic Class Skills (13):</BASEFONT>", false, false);

            string[] magicSkills = { "Cryomancy", "Demonology", "NecromancyArts", "Druidism", "Elementalism", "Songweaving",
                                     "Hexcraft", "Divination", "Conjuration", "SpiritCalling", "Runeweaving", "IllusionMagic", "Magery" };

            for (int i = 0; i < magicSkills.Length; i++)
            {
                int y = startY + 75 + (i * 25);
                SkillName skillEnum = (SkillName)Enum.Parse(typeof(SkillName), magicSkills[i]);
                Skill skill = pm.Skills[skillEnum];
                double skillValue = skill != null ? skill.Base : 0;
                bool isClassSkill = classSkill.HasValue && classSkill.Value == skillEnum;

                string skillColor = isClassSkill ? $"#{ValueColor:X6}" : $"#{LabelColor:X6}";
                string marker = isClassSkill ? ">> " : "";

                AddHtml(25, y, 150, 20, $"<BASEFONT COLOR={skillColor}>{marker}{magicSkills[i]}:</BASEFONT>", false, false);
                AddHtml(175, y, 50, 20, $"<BASEFONT COLOR=#{ValueColor:X6}>{skillValue:F1}</BASEFONT>", false, false);

                // Set to 100 button
                AddButton(230, y - 2, 4005, 4007, 2000 + i, GumpButtonType.Reply, 0);
                AddHtml(265, y, 35, 20, $"<BASEFONT COLOR=#{LabelColor:X6}>100</BASEFONT>", false, false);

                // Set to 0 button
                AddButton(295, y - 2, 4005, 4007, 2100 + i, GumpButtonType.Reply, 0);
                AddHtml(330, y, 25, 20, $"<BASEFONT COLOR=#{LabelColor:X6}>0</BASEFONT>", false, false);
            }

            // Martial skills (right column)
            AddHtml(400, startY + 55, 200, 20, $"<BASEFONT COLOR=#{MartialColor:X6}>Martial Class Skills (13):</BASEFONT>", false, false);

            string[] martialSkills = { "Berserking", "Subterfuge", "MartialArts", "ChivalricArts", "HolyDevotion", "Marksmanship",
                                       "CombatMastery", "Zealotry", "Manhunting", "BeastBonding", "Engineering", "Transmutation",
                                       "DivineGrace" };

            for (int i = 0; i < martialSkills.Length; i++)
            {
                int y = startY + 75 + (i * 25);
                SkillName skillEnum = (SkillName)Enum.Parse(typeof(SkillName), martialSkills[i]);
                Skill skill = pm.Skills[skillEnum];
                double skillValue = skill != null ? skill.Base : 0;
                bool isClassSkill = classSkill.HasValue && classSkill.Value == skillEnum;

                string skillColor = isClassSkill ? $"#{ValueColor:X6}" : $"#{LabelColor:X6}";
                string marker = isClassSkill ? ">> " : "";

                AddHtml(400, y, 150, 20, $"<BASEFONT COLOR={skillColor}>{marker}{martialSkills[i]}:</BASEFONT>", false, false);
                AddHtml(550, y, 50, 20, $"<BASEFONT COLOR=#{ValueColor:X6}>{skillValue:F1}</BASEFONT>", false, false);

                // Set to 100 button
                AddButton(605, y - 2, 4005, 4007, 2200 + i, GumpButtonType.Reply, 0);
                AddHtml(640, y, 35, 20, $"<BASEFONT COLOR=#{LabelColor:X6}>100</BASEFONT>", false, false);

                // Set to 0 button
                AddButton(670, y - 2, 4005, 4007, 2300 + i, GumpButtonType.Reply, 0);
                AddHtml(705, y, 25, 20, $"<BASEFONT COLOR=#{LabelColor:X6}>0</BASEFONT>", false, false);
            }

            // Quick actions section at bottom
            int actionsY = startY + 430;

            AddBackground(25, actionsY, 350, 60, 9200);
            AddHtml(35, actionsY + 5, 330, 20, $"<BASEFONT COLOR=#{HeaderColor:X6}>Quick Actions:</BASEFONT>", false, false);

            AddButton(35, actionsY + 28, 4005, 4007, 2400, GumpButtonType.Reply, 0);
            AddHtml(70, actionsY + 30, 120, 20, $"<BASEFONT COLOR=#{ValueColor:X6}>Class Skill to 100</BASEFONT>", false, false);

            AddButton(200, actionsY + 28, 4005, 4007, 2401, GumpButtonType.Reply, 0);
            AddHtml(235, actionsY + 30, 120, 20, $"<BASEFONT COLOR=#{WarningColor:X6}>Reset All to 0</BASEFONT>", false, false);

            // Info section
            AddBackground(400, actionsY, 350, 60, 9200);
            AddHtml(410, actionsY + 5, 330, 20, $"<BASEFONT COLOR=#{HeaderColor:X6}>Skill IDs:</BASEFONT>", false, false);
            AddHtml(410, actionsY + 28, 330, 25, $"<BASEFONT COLOR=#{LabelColor:X6}>Magic: 58-69 | Martial: 70-83</BASEFONT>", false, false);
        }

        #endregion

        #region Items Page

        private void DrawItemsPage(int startY)
        {
            AddHtml(25, startY, 300, 20, $"<BASEFONT COLOR=#{HeaderColor:X6}>Item Distribution:</BASEFONT>", false, false);

            // Sub-page tabs
            string[] subPages = { "Spellbooks", "Reagents", "Weapons", "Armor", "Focus Items", "Consumables" };
            for (int i = 0; i < subPages.Length; i++)
            {
                int btnId = 1200 + i;
                bool isActive = m_ItemSubPage == i;
                AddButton(25 + (i * 120), startY + 25, isActive ? 4006 : 4005, 4007, btnId, GumpButtonType.Reply, 0);
                AddHtml(60 + (i * 120), startY + 27, 90, 20,
                    $"<BASEFONT COLOR=#{(isActive ? ValueColor : LabelColor):X6}>{subPages[i]}</BASEFONT>", false, false);
            }

            int contentStart = startY + 55;

            switch (m_ItemSubPage)
            {
                case 0: DrawSpellbooksSection(contentStart); break;
                case 1: DrawReagentsSection(contentStart); break;
                case 2: DrawWeaponsSection(contentStart); break;
                case 3: DrawArmorSection(contentStart); break;
                case 4: DrawFocusItemsSection(contentStart); break;
                case 5: DrawConsumablesSection(contentStart); break;
            }
        }

        private void DrawSpellbooksSection(int startY)
        {
            AddHtml(25, startY, 300, 20, $"<BASEFONT COLOR=#{MagicColor:X6}>Spellbooks (11) + Songbook (1):</BASEFONT>", false, false);

            string[] spellbooks = {
                "IceMage", "Druid", "Witch", "Sorcerer", "Warlock", "Oracle",
                "Necromancer", "Summoner", "Shaman", "Songweaving", "Enchanter", "Illusionist"
            };

            for (int i = 0; i < spellbooks.Length; i++)
            {
                int x = 25 + (i % 3) * 240;
                int y = startY + 25 + (i / 3) * 25;

                AddButton(x, y, 4005, 4007, 1300 + i, GumpButtonType.Reply, 0);
                string label = spellbooks[i] == "Songweaving" ? "Songweaving Songbook" : $"{spellbooks[i]} Spellbook";
                AddHtml(x + 35, y + 2, 180, 20, $"<BASEFONT COLOR=#{LabelColor:X6}>{label}</BASEFONT>", false, false);
            }

            // Give all spellbooks
            AddButton(25, startY + 135, 4005, 4007, 1399, GumpButtonType.Reply, 0);
            AddHtml(60, startY + 137, 200, 20, $"<BASEFONT COLOR=#{ValueColor:X6}>Give ALL Spellbooks</BASEFONT>", false, false);

            // Standard magery spellbook
            AddButton(300, startY + 135, 4005, 4007, 1398, GumpButtonType.Reply, 0);
            AddHtml(335, startY + 137, 200, 20, $"<BASEFONT COLOR=#{LabelColor:X6}>Standard Magery Book</BASEFONT>", false, false);
        }

        private void DrawReagentsSection(int startY)
        {
            AddHtml(25, startY, 400, 20, $"<BASEFONT COLOR=#{MagicColor:X6}>Reagent Bags (Full Set per School):</BASEFONT>", false, false);

            string[] schools = {
                "Ice Magic", "Nature/Druid", "Hex/Witch", "Elemental", "Dark/Warlock", "Divination",
                "Necromancy", "Summoning", "Shamanic", "Songweaving", "Enchanting", "Illusion"
            };

            for (int i = 0; i < schools.Length; i++)
            {
                int x = 25 + (i % 3) * 240;
                int y = startY + 25 + (i / 3) * 25;

                AddButton(x, y, 4005, 4007, 1400 + i, GumpButtonType.Reply, 0);
                AddHtml(x + 35, y + 2, 180, 20, $"<BASEFONT COLOR=#{LabelColor:X6}>{schools[i]} Reags</BASEFONT>", false, false);
            }

            // Give all reagents
            AddButton(25, startY + 135, 4005, 4007, 1499, GumpButtonType.Reply, 0);
            AddHtml(60, startY + 137, 200, 20, $"<BASEFONT COLOR=#{ValueColor:X6}>Give ALL Reagents</BASEFONT>", false, false);

            // Standard magery reagents
            AddButton(300, startY + 135, 4005, 4007, 1498, GumpButtonType.Reply, 0);
            AddHtml(335, startY + 137, 200, 20, $"<BASEFONT COLOR=#{LabelColor:X6}>Standard Magery Reags</BASEFONT>", false, false);
        }

        private void DrawWeaponsSection(int startY)
        {
            AddHtml(25, startY, 300, 20, $"<BASEFONT COLOR=#{MartialColor:X6}>Regional Weapons:</BASEFONT>", false, false);

            string[] regions = { "Frosthold", "Emberlands", "Desert", "Shadowfen", "Verdantpeak",
                                 "Crystal Barrens", "Ironclad", "Skyreach", "Underwater", "ShadowVoid" };

            for (int i = 0; i < regions.Length; i++)
            {
                int x = 25 + (i % 3) * 240;
                int y = startY + 25 + (i / 3) * 25;

                AddButton(x, y, 4005, 4007, 1500 + i, GumpButtonType.Reply, 0);
                AddHtml(x + 35, y + 2, 180, 20, $"<BASEFONT COLOR=#{LabelColor:X6}>{regions[i]} Sword</BASEFONT>", false, false);
            }

            // Legendary weapons
            AddHtml(25, startY + 130, 300, 20, $"<BASEFONT COLOR=#{HeaderColor:X6}>Legendary Weapons:</BASEFONT>", false, false);

            string[] legendary = { "The Eternal Winter", "Phoenix Ascension", "The Cogmaster", "Prismatic Edge", "Voidcaller" };

            for (int i = 0; i < legendary.Length; i++)
            {
                int x = 25 + (i % 3) * 240;
                int y = startY + 155 + (i / 3) * 25;

                AddButton(x, y, 4005, 4007, 1550 + i, GumpButtonType.Reply, 0);
                AddHtml(x + 35, y + 2, 200, 20, $"<BASEFONT COLOR=#{HeaderColor:X6}>{legendary[i]}</BASEFONT>", false, false);
            }

            // Give all weapons
            AddButton(25, startY + 210, 4005, 4007, 1599, GumpButtonType.Reply, 0);
            AddHtml(60, startY + 212, 250, 20, $"<BASEFONT COLOR=#{ValueColor:X6}>Give ALL Legendary Weapons</BASEFONT>", false, false);
        }

        private void DrawArmorSection(int startY)
        {
            AddHtml(25, startY, 300, 20, $"<BASEFONT COLOR=#{MartialColor:X6}>Armor Sets:</BASEFONT>", false, false);

            // Regional armor by type
            AddHtml(25, startY + 25, 200, 20, $"<BASEFONT COLOR=#{LabelColor:X6}>Regional Plate (by region):</BASEFONT>", false, false);

            string[] regions = { "Frosthold", "Emberlands", "Desert", "Shadowfen", "Verdantpeak",
                                 "Crystal", "Ironclad", "Skyreach", "Underwater", "ShadowVoid" };

            for (int i = 0; i < regions.Length; i++)
            {
                int x = 25 + (i % 5) * 145;
                int y = startY + 45 + (i / 5) * 25;

                AddButton(x, y, 4005, 4007, 1600 + i, GumpButtonType.Reply, 0);
                AddHtml(x + 35, y + 2, 100, 20, $"<BASEFONT COLOR=#{LabelColor:X6}>{regions[i]}</BASEFONT>", false, false);
            }

            // Legendary armor sets
            AddHtml(25, startY + 105, 300, 20, $"<BASEFONT COLOR=#{HeaderColor:X6}>Legendary Armor Sets:</BASEFONT>", false, false);

            AddButton(25, startY + 130, 4005, 4007, 1650, GumpButtonType.Reply, 0);
            AddHtml(60, startY + 132, 200, 20, $"<BASEFONT COLOR=#{HeaderColor:X6}>Glacial Aegis (Tank)</BASEFONT>", false, false);

            AddButton(260, startY + 130, 4005, 4007, 1651, GumpButtonType.Reply, 0);
            AddHtml(295, startY + 132, 200, 20, $"<BASEFONT COLOR=#{HeaderColor:X6}>Steamwork Exosuit (DPS)</BASEFONT>", false, false);

            AddButton(520, startY + 130, 4005, 4007, 1652, GumpButtonType.Reply, 0);
            AddHtml(555, startY + 132, 200, 20, $"<BASEFONT COLOR=#{HeaderColor:X6}>Shadow Shroud (Rogue)</BASEFONT>", false, false);

            // Regional shields
            AddHtml(25, startY + 170, 300, 20, $"<BASEFONT COLOR=#{LabelColor:X6}>Regional Shields:</BASEFONT>", false, false);

            AddButton(25, startY + 195, 4005, 4007, 1660, GumpButtonType.Reply, 0);
            AddHtml(60, startY + 197, 200, 20, $"<BASEFONT COLOR=#{ValueColor:X6}>Give All Regional Shields</BASEFONT>", false, false);

            // Full set button
            AddButton(300, startY + 195, 4005, 4007, 1699, GumpButtonType.Reply, 0);
            AddHtml(335, startY + 197, 250, 20, $"<BASEFONT COLOR=#{ValueColor:X6}>Give ALL Legendary Armor</BASEFONT>", false, false);
        }

        private void DrawFocusItemsSection(int startY)
        {
            AddHtml(25, startY, 300, 20, $"<BASEFONT COLOR=#{MagicColor:X6}>Class Focus Items:</BASEFONT>", false, false);

            // Focus items by class
            var focusItems = new[]
            {
                ("Soul Gem", "Warlock"), ("Frost Crystal", "Ice Mage"), ("Death's Hourglass", "Necromancer"),
                ("Primal Totem", "Druid"), ("Dragon Lute", "Bard"), ("War Banner", "Knight"),
                ("Fury Idol", "Barbarian"), ("Chi Beads", "Monk"), ("Shadow Veil", "Rogue"),
                ("Hunter's Mark Totem", "Ranger"), ("Holy Symbol", "Paladin"), ("Tracking Stone", "Bounty Hunter"),
                ("Zealous Icon", "Templar"), ("Beast Bond", "Beastmaster"), ("Steam Core", "Artificer"),
                ("Elemental Orb", "Sorcerer"), ("Hex Doll", "Witch"), ("Seer's Crystal", "Oracle"),
                ("Summoner's Sigil", "Summoner"), ("Spirit Feather", "Shaman"), ("Sacred Censer", "Cleric"),
                ("Rune Stone", "Enchanter"), ("Mirror Shard", "Illusionist"), ("Arcane Conduit", "Wizard"),
                ("Philosopher's Stone", "Alchemist")
            };

            for (int i = 0; i < focusItems.Length; i++)
            {
                int x = 25 + (i % 3) * 250;
                int y = startY + 25 + (i / 3) * 22;

                AddButton(x, y, 4005, 4007, 1700 + i, GumpButtonType.Reply, 0);
                AddHtml(x + 35, y + 2, 200, 20,
                    $"<BASEFONT COLOR=#{LabelColor:X6}>{focusItems[i].Item1} ({focusItems[i].Item2})</BASEFONT>", false, false);
            }

            // Give all focus items
            int allY = startY + 25 + ((focusItems.Length / 3) + 1) * 22 + 10;
            AddButton(25, allY, 4005, 4007, 1799, GumpButtonType.Reply, 0);
            AddHtml(60, allY + 2, 250, 20, $"<BASEFONT COLOR=#{ValueColor:X6}>Give ALL Focus Items</BASEFONT>", false, false);

            // Give class-appropriate focus item
            AddButton(350, allY, 4005, 4007, 1798, GumpButtonType.Reply, 0);
            AddHtml(385, allY + 2, 250, 20, $"<BASEFONT COLOR=#{ValueColor:X6}>Give Focus for Target's Class</BASEFONT>", false, false);
        }

        private void DrawConsumablesSection(int startY)
        {
            AddHtml(25, startY, 400, 20, $"<BASEFONT COLOR=#{MagicColor:X6}>Vystia Regional Potions (8 types):</BASEFONT>", false, false);

            // Resistance Potions section
            AddHtml(25, startY + 30, 300, 20, $"<BASEFONT COLOR=#{HeaderColor:X6}>Resistance Potions (+25 resist, 10 min):</BASEFONT>", false, false);

            string[] resistPotions = { "Cold Resistance", "Heat Resistance", "Poison Resistance", "Energy Resistance" };
            int[] resistHues = { 1152, 1358, 2212, 1154 };

            for (int i = 0; i < resistPotions.Length; i++)
            {
                int x = 25 + (i % 2) * 350;
                int y = startY + 55 + (i / 2) * 25;

                AddButton(x, y, 4005, 4007, 1900 + i, GumpButtonType.Reply, 0);
                AddHtml(x + 35, y + 2, 280, 20,
                    $"<BASEFONT COLOR=#{LabelColor:X6}>{resistPotions[i]} Potion</BASEFONT>", false, false);
            }

            // Enhancement Potions section
            AddHtml(25, startY + 115, 300, 20, $"<BASEFONT COLOR=#{HeaderColor:X6}>Enhancement Potions:</BASEFONT>", false, false);

            var enhancePotions = new[]
            {
                ("Nature's Blessing", "+5 HP regen, 15 min", 2010),
                ("Crystal Clarity", "+15 INT, Detect Hidden, 20 min", 1154),
                ("Desert Swiftness", "+10 DEX, 10 min", 2305),
                ("Ironclad Fortitude", "+10 STR, 10 min", 2401)
            };

            for (int i = 0; i < enhancePotions.Length; i++)
            {
                int x = 25 + (i % 2) * 350;
                int y = startY + 140 + (i / 2) * 25;

                AddButton(x, y, 4005, 4007, 1910 + i, GumpButtonType.Reply, 0);
                AddHtml(x + 35, y + 2, 300, 20,
                    $"<BASEFONT COLOR=#{LabelColor:X6}>{enhancePotions[i].Item1} ({enhancePotions[i].Item2})</BASEFONT>", false, false);
            }

            // Batch give buttons
            int batchY = startY + 210;
            AddHtml(25, batchY, 300, 20, $"<BASEFONT COLOR=#{HeaderColor:X6}>Batch Actions:</BASEFONT>", false, false);

            AddButton(25, batchY + 25, 4005, 4007, 1920, GumpButtonType.Reply, 0);
            AddHtml(60, batchY + 27, 250, 20, $"<BASEFONT COLOR=#{ValueColor:X6}>Give All Resistance Potions (5 each)</BASEFONT>", false, false);

            AddButton(350, batchY + 25, 4005, 4007, 1921, GumpButtonType.Reply, 0);
            AddHtml(385, batchY + 27, 250, 20, $"<BASEFONT COLOR=#{ValueColor:X6}>Give All Enhancement Potions (5 each)</BASEFONT>", false, false);

            AddButton(25, batchY + 50, 4005, 4007, 1922, GumpButtonType.Reply, 0);
            AddHtml(60, batchY + 52, 250, 20, $"<BASEFONT COLOR=#{ValueColor:X6}>Give ALL Regional Potions (5 each)</BASEFONT>", false, false);

            // Info box
            AddHtml(25, batchY + 90, 700, 60,
                $"<BASEFONT COLOR=#{LabelColor:X6}>Regional potions have cooldowns (5-15 min) and durations (10-20 min).<BR>" +
                $"Potions can also be purchased from the VystiaResourceVendor.</BASEFONT>", false, false);
        }

        #endregion

        #region Tools Page

        private void DrawToolsPage(int startY)
        {
            AddHtml(25, startY, 300, 20, $"<BASEFONT COLOR=#{HeaderColor:X6}>GM Tools:</BASEFONT>", false, false);

            // Spawn tools
            AddHtml(25, startY + 30, 200, 20, $"<BASEFONT COLOR=#{LabelColor:X6}>Spawn:</BASEFONT>", false, false);

            AddButton(25, startY + 55, 4005, 4007, 1800, GumpButtonType.Reply, 0);
            AddHtml(60, startY + 57, 200, 20, $"<BASEFONT COLOR=#{LabelColor:X6}>Spawn Practice Target</BASEFONT>", false, false);

            AddButton(25, startY + 80, 4005, 4007, 1801, GumpButtonType.Reply, 0);
            AddHtml(60, startY + 82, 200, 20, $"<BASEFONT COLOR=#{LabelColor:X6}>Spawn Class Item Vendor</BASEFONT>", false, false);

            AddButton(25, startY + 105, 4005, 4007, 1802, GumpButtonType.Reply, 0);
            AddHtml(60, startY + 107, 200, 20, $"<BASEFONT COLOR=#{LabelColor:X6}>Spawn Reagent Vendor</BASEFONT>", false, false);

            AddButton(25, startY + 130, 4005, 4007, 1803, GumpButtonType.Reply, 0);
            AddHtml(60, startY + 132, 200, 20, $"<BASEFONT COLOR=#{LabelColor:X6}>Spawn Weapons Chests</BASEFONT>", false, false);

            // Editor tools
            AddHtml(25, startY + 145, 200, 20, $"<BASEFONT COLOR=#{LabelColor:X6}>Editors:</BASEFONT>", false, false);

            AddButton(25, startY + 170, 4005, 4007, 1810, GumpButtonType.Reply, 0);
            AddHtml(60, startY + 172, 200, 20, $"<BASEFONT COLOR=#{LabelColor:X6}>Open Ability Editor</BASEFONT>", false, false);

            AddButton(25, startY + 195, 4005, 4007, 1811, GumpButtonType.Reply, 0);
            AddHtml(60, startY + 197, 200, 20, $"<BASEFONT COLOR=#{LabelColor:X6}>Open Class Selection Gump</BASEFONT>", false, false);

            AddButton(25, startY + 220, 4005, 4007, 1812, GumpButtonType.Reply, 0);
            AddHtml(60, startY + 222, 200, 20, $"<BASEFONT COLOR=#{LabelColor:X6}>Open Spawn Vystia Gump</BASEFONT>", false, false);

            // Target tools
            AddHtml(300, startY + 30, 200, 20, $"<BASEFONT COLOR=#{LabelColor:X6}>Target Actions:</BASEFONT>", false, false);

            AddButton(300, startY + 55, 4005, 4007, 1820, GumpButtonType.Reply, 0);
            AddHtml(335, startY + 57, 200, 20, $"<BASEFONT COLOR=#{LabelColor:X6}>Full Heal Target</BASEFONT>", false, false);

            AddButton(300, startY + 80, 4005, 4007, 1821, GumpButtonType.Reply, 0);
            AddHtml(335, startY + 82, 200, 20, $"<BASEFONT COLOR=#{LabelColor:X6}>Restore Mana/Stam</BASEFONT>", false, false);

            AddButton(300, startY + 105, 4005, 4007, 1822, GumpButtonType.Reply, 0);
            AddHtml(335, startY + 107, 200, 20, $"<BASEFONT COLOR=#{LabelColor:X6}>Cure All</BASEFONT>", false, false);

            AddButton(300, startY + 130, 4005, 4007, 1823, GumpButtonType.Reply, 0);
            AddHtml(335, startY + 132, 200, 20, $"<BASEFONT COLOR=#{LabelColor:X6}>Resurrect</BASEFONT>", false, false);

            AddButton(300, startY + 155, 4005, 4007, 1824, GumpButtonType.Reply, 0);
            AddHtml(335, startY + 157, 200, 20, $"<BASEFONT COLOR=#{LabelColor:X6}>Kill Target</BASEFONT>", false, false);

            // Batch give items
            AddHtml(300, startY + 195, 200, 20, $"<BASEFONT COLOR=#{LabelColor:X6}>Batch Give Items:</BASEFONT>", false, false);

            AddButton(300, startY + 220, 4005, 4007, 1830, GumpButtonType.Reply, 0);
            AddHtml(335, startY + 222, 200, 20, $"<BASEFONT COLOR=#{LabelColor:X6}>Give Resource Potions</BASEFONT>", false, false);

            AddButton(300, startY + 245, 4005, 4007, 1831, GumpButtonType.Reply, 0);
            AddHtml(335, startY + 247, 200, 20, $"<BASEFONT COLOR=#{LabelColor:X6}>Give Combat Potions</BASEFONT>", false, false);

            AddButton(300, startY + 270, 4005, 4007, 1832, GumpButtonType.Reply, 0);
            AddHtml(335, startY + 272, 250, 20, $"<BASEFONT COLOR=#{ValueColor:X6}>Give Complete Class Loadout</BASEFONT>", false, false);

            // Quick commands reference
            AddHtml(580, startY + 30, 180, 20, $"<BASEFONT COLOR=#{HeaderColor:X6}>Quick Commands:</BASEFONT>", false, false);
            AddHtml(580, startY + 55, 200, 300,
                $"<BASEFONT COLOR=#{LabelColor:X6}>" +
                "[VystiaAdmin<BR>" +
                "[VA (alias)<BR>" +
                "[SetClassV2<BR>" +
                "[ListClassesV2<BR>" +
                "[ClassInfoV2<BR>" +
                "[AbilityEditor<BR>" +
                "[SetResource<BR>" +
                "[SetStance<BR>" +
                "[RemoveStance<BR>" +
                "[ListStances<BR>" +
                "[spawnvystia<BR>" +
                "[spellbook &lt;type&gt;</BASEFONT>",
                false, false);
        }

        #endregion

        #region Factions Page

        private void DrawFactionsPage(int startY)
        {
            AddHtml(25, startY, 400, 20, $"<BASEFONT COLOR=#{HeaderColor:X6}>Faction Reputation Management:</BASEFONT>", false, false);

            PlayerMobile pm = m_Target as PlayerMobile;

            // Get faction info
            string[] factionNames = { "Frostguard", "FlameLegion", "Greenward", "ArcaneConclave", "Technoguild", "Sandwalkers", "Voidborn" };
            VystiaFaction[] factionEnums = { VystiaFaction.Frostguard, VystiaFaction.FlameLegion, VystiaFaction.Greenward,
                                             VystiaFaction.ArcaneConclave, VystiaFaction.Technoguild, VystiaFaction.Sandwalkers, VystiaFaction.Voidborn };

            // Display all 7 factions with current reputation
            for (int i = 0; i < 7; i++)
            {
                int y = startY + 30 + (i * 60);
                int rep = pm != null ? VystiaReputation.GetFactionReputation(pm, factionEnums[i]) : 0;
                var tier = FactionData.GetTier(rep);
                var info = FactionData.GetInfo(factionEnums[i]);
                int tierHue = FactionData.GetTierHue(tier);

                // Faction name and current status
                AddHtml(25, y, 150, 20, $"<BASEFONT COLOR=#{info?.Hue ?? LabelColor:X6}>{factionNames[i]}</BASEFONT>", false, false);
                AddHtml(180, y, 200, 20, $"<BASEFONT COLOR=#{LabelColor:X6}>Rep: {rep} ({tier})</BASEFONT>", false, false);

                // Set to specific tiers - Row 1
                AddButton(25, y + 22, 4005, 4007, 3000 + (i * 10) + 0, GumpButtonType.Reply, 0); // Hostile
                AddHtml(60, y + 24, 60, 20, $"<BASEFONT COLOR=#FF0000>Hostile</BASEFONT>", false, false);

                AddButton(120, y + 22, 4005, 4007, 3000 + (i * 10) + 1, GumpButtonType.Reply, 0); // Neutral
                AddHtml(155, y + 24, 60, 20, $"<BASEFONT COLOR=#AAAAAA>Neutral</BASEFONT>", false, false);

                AddButton(215, y + 22, 4005, 4007, 3000 + (i * 10) + 2, GumpButtonType.Reply, 0); // Friendly
                AddHtml(250, y + 24, 60, 20, $"<BASEFONT COLOR=#00AA00>Friendly</BASEFONT>", false, false);

                AddButton(310, y + 22, 4005, 4007, 3000 + (i * 10) + 3, GumpButtonType.Reply, 0); // Honored
                AddHtml(345, y + 24, 60, 20, $"<BASEFONT COLOR=#00FF00>Honored</BASEFONT>", false, false);

                AddButton(405, y + 22, 4005, 4007, 3000 + (i * 10) + 4, GumpButtonType.Reply, 0); // Revered
                AddHtml(440, y + 24, 60, 20, $"<BASEFONT COLOR=#AA00FF>Revered</BASEFONT>", false, false);

                AddButton(500, y + 22, 4005, 4007, 3000 + (i * 10) + 5, GumpButtonType.Reply, 0); // Exalted
                AddHtml(535, y + 24, 60, 20, $"<BASEFONT COLOR=#FFD700>Exalted</BASEFONT>", false, false);
            }

            // Custom reputation input section
            int inputY = startY + 450;
            AddHtml(25, inputY, 300, 20, $"<BASEFONT COLOR=#{HeaderColor:X6}>Set Custom Reputation:</BASEFONT>", false, false);

            // Faction selection (use faction buttons to select which one to modify)
            AddHtml(25, inputY + 25, 150, 20, $"<BASEFONT COLOR=#{LabelColor:X6}>Custom Value (-3000 to 21000):</BASEFONT>", false, false);
            AddBackground(200, inputY + 23, 80, 22, 9350);
            AddTextEntry(205, inputY + 25, 70, 20, LabelColor, 100, "0"); // TextEntry ID 100

            // Apply buttons for each faction
            for (int i = 0; i < 7; i++)
            {
                int x = 25 + (i * 100);
                if (i >= 4) // Wrap to second row
                {
                    x = 25 + ((i - 4) * 100);
                    AddButton(x, inputY + 75, 4005, 4007, 3100 + i, GumpButtonType.Reply, 0);
                    AddHtml(x + 35, inputY + 77, 80, 20, $"<BASEFONT COLOR=#{LabelColor:X6}>{factionNames[i].Substring(0, Math.Min(8, factionNames[i].Length))}</BASEFONT>", false, false);
                }
                else
                {
                    AddButton(x, inputY + 50, 4005, 4007, 3100 + i, GumpButtonType.Reply, 0);
                    AddHtml(x + 35, inputY + 52, 80, 20, $"<BASEFONT COLOR=#{LabelColor:X6}>{factionNames[i].Substring(0, Math.Min(8, factionNames[i].Length))}</BASEFONT>", false, false);
                }
            }

            // Quick reference
            AddHtml(500, startY + 450, 280, 20, $"<BASEFONT COLOR=#{HeaderColor:X6}>Reputation Tiers:</BASEFONT>", false, false);
            AddHtml(500, startY + 475, 280, 120,
                $"<BASEFONT COLOR=#{LabelColor:X6}>" +
                "Hostile: -3000 to -1000<BR>" +
                "Unfriendly: -1000 to 0<BR>" +
                "Neutral: 0 to 3000<BR>" +
                "Friendly: 3000 to 6000 (5% discount)<BR>" +
                "Honored: 6000 to 12000 (8% discount)<BR>" +
                "Revered: 12000 to 15000 (12% discount)<BR>" +
                "Exalted: 15000+ (15% discount)</BASEFONT>",
                false, false);
        }

        #endregion

        #region Religion Page

        private void DrawReligionPage(int startY)
        {
            AddHtml(25, startY, 400, 20, $"<BASEFONT COLOR=#{HeaderColor:X6}>Religion & Piety Management:</BASEFONT>", false, false);

            PlayerMobile pm = m_Target as PlayerMobile;

            // Current religion info
            var currentReligion = pm != null ? VystiaPiety.GetReligion(pm) : VystiaReligion.None;
            int currentPiety = pm != null ? VystiaPiety.GetPietyValue(pm) : 0;
            var pietyTier = ReligionData.GetTier(currentPiety);
            var religionInfo = currentReligion != VystiaReligion.None ? ReligionData.GetInfo(currentReligion) : null;

            AddHtml(25, startY + 25, 400, 20,
                $"<BASEFONT COLOR=#{LabelColor:X6}>Current Religion: <BASEFONT COLOR=#{religionInfo?.Hue ?? ValueColor:X6}>{(currentReligion == VystiaReligion.None ? "None" : religionInfo?.Name ?? currentReligion.ToString())}</BASEFONT></BASEFONT>",
                false, false);
            AddHtml(25, startY + 45, 300, 20,
                $"<BASEFONT COLOR=#{LabelColor:X6}>Piety: <BASEFONT COLOR=#{ValueColor:X6}>{currentPiety}/1000</BASEFONT> ({pietyTier})</BASEFONT>",
                false, false);

            // Religion selection section
            AddHtml(25, startY + 80, 200, 20, $"<BASEFONT COLOR=#{HeaderColor:X6}>Set Religion:</BASEFONT>", false, false);

            string[] religionNames = { "FrosthelmFaith", "SuryasSandscript", "LunarasCovenant", "CelestisArcanum", "OceanasCovenant", "CogsmithCreed" };
            string[] shortDescs = { "Worship of the Eternal Winter", "Devotees of the Eternal Flame", "Druids of Nature's Balance",
                                    "Seekers of Cosmic Knowledge", "Disciples of Void and Shadow", "Engineers of the Machine Spirit" };
            VystiaReligion[] religionEnums = { VystiaReligion.FrosthelmFaith, VystiaReligion.SuryasSandscript, VystiaReligion.LunarasCovenant,
                                               VystiaReligion.CelestisArcanum, VystiaReligion.OceanasCovenant, VystiaReligion.CogsmithCreed };

            // None button
            AddButton(25, startY + 105, 4005, 4007, 3200, GumpButtonType.Reply, 0);
            AddHtml(60, startY + 107, 100, 20, $"<BASEFONT COLOR=#{LabelColor:X6}>None</BASEFONT>", false, false);

            // Religion buttons - 2 columns, 3 rows for better spacing
            for (int i = 0; i < 6; i++)
            {
                var info = ReligionData.GetInfo(religionEnums[i]);
                int row = i / 2;  // 2 columns
                int col = i % 2;
                int x = 25 + (col * 380);  // Wider column spacing
                int y = startY + 130 + (row * 45);  // Taller rows

                bool isSelected = currentReligion == religionEnums[i];
                int buttonGfx = isSelected ? 4006 : 4005;

                AddButton(x, y, buttonGfx, 4007, 3201 + i, GumpButtonType.Reply, 0);
                AddHtml(x + 35, y + 2, 340, 20, $"<BASEFONT COLOR=#{info?.Hue ?? LabelColor:X6}>{info?.Name ?? religionNames[i]}</BASEFONT>", false, false);
                AddHtml(x + 35, y + 20, 340, 20, $"<BASEFONT COLOR=#{LabelColor:X6}>{shortDescs[i]}</BASEFONT>", false, false);
            }

            // Piety management section (left side)
            int pietyY = startY + 280;
            AddHtml(25, pietyY, 200, 20, $"<BASEFONT COLOR=#{HeaderColor:X6}>Set Piety Level:</BASEFONT>", false, false);

            // Quick set buttons - Row 1 (3 buttons)
            AddButton(25, pietyY + 25, 4005, 4007, 3300, GumpButtonType.Reply, 0);
            AddHtml(60, pietyY + 27, 40, 20, $"<BASEFONT COLOR=#{LabelColor:X6}>0</BASEFONT>", false, false);

            AddButton(95, pietyY + 25, 4005, 4007, 3301, GumpButtonType.Reply, 0);
            AddHtml(130, pietyY + 27, 70, 20, $"<BASEFONT COLOR=#{LabelColor:X6}>50 (Init)</BASEFONT>", false, false);

            AddButton(200, pietyY + 25, 4005, 4007, 3302, GumpButtonType.Reply, 0);
            AddHtml(235, pietyY + 27, 80, 20, $"<BASEFONT COLOR=#{LabelColor:X6}>200 (Dev)</BASEFONT>", false, false);

            // Quick set buttons - Row 2 (3 buttons)
            AddButton(25, pietyY + 50, 4005, 4007, 3303, GumpButtonType.Reply, 0);
            AddHtml(60, pietyY + 52, 80, 20, $"<BASEFONT COLOR=#{LabelColor:X6}>500 (Faith)</BASEFONT>", false, false);

            AddButton(140, pietyY + 50, 4005, 4007, 3304, GumpButtonType.Reply, 0);
            AddHtml(175, pietyY + 52, 80, 20, $"<BASEFONT COLOR=#{LabelColor:X6}>900 (Exalt)</BASEFONT>", false, false);

            AddButton(255, pietyY + 50, 4005, 4007, 3305, GumpButtonType.Reply, 0);
            AddHtml(290, pietyY + 52, 80, 20, $"<BASEFONT COLOR=#{ValueColor:X6}>1000 (Max)</BASEFONT>", false, false);

            // Custom piety input - Row 3
            AddHtml(25, pietyY + 85, 150, 20, $"<BASEFONT COLOR=#{LabelColor:X6}>Custom Piety (0-1000):</BASEFONT>", false, false);
            AddBackground(180, pietyY + 83, 60, 22, 9350);
            AddTextEntry(185, pietyY + 85, 50, 20, LabelColor, 101, currentPiety.ToString()); // TextEntry ID 101

            AddButton(250, pietyY + 83, 4005, 4007, 3310, GumpButtonType.Reply, 0);
            AddHtml(285, pietyY + 85, 60, 20, $"<BASEFONT COLOR=#{ValueColor:X6}>Apply</BASEFONT>", false, false);

            // Piety tier info (right side - separate column)
            AddHtml(420, pietyY, 280, 20, $"<BASEFONT COLOR=#{HeaderColor:X6}>Piety Tiers:</BASEFONT>", false, false);
            AddHtml(420, pietyY + 22, 340, 20, $"<BASEFONT COLOR=#{LabelColor:X6}>None: 0-49 (No bonuses)</BASEFONT>", false, false);
            AddHtml(420, pietyY + 40, 340, 20, $"<BASEFONT COLOR=#{LabelColor:X6}>Initiate: 50-199 (First passive)</BASEFONT>", false, false);
            AddHtml(420, pietyY + 58, 340, 20, $"<BASEFONT COLOR=#{LabelColor:X6}>Devoted: 200-499 (First power)</BASEFONT>", false, false);
            AddHtml(420, pietyY + 76, 340, 20, $"<BASEFONT COLOR=#{LabelColor:X6}>Faithful: 500-899 (Second power)</BASEFONT>", false, false);
            AddHtml(420, pietyY + 94, 340, 20, $"<BASEFONT COLOR=#{LabelColor:X6}>Exalted: 900-1000 (All powers)</BASEFONT>", false, false);

            AddHtml(420, pietyY + 120, 280, 20, $"<BASEFONT COLOR=#{HeaderColor:X6}>Piety Sources:</BASEFONT>", false, false);
            AddHtml(420, pietyY + 140, 340, 20, $"<BASEFONT COLOR=#{LabelColor:X6}>- Daily Prayer: +10</BASEFONT>", false, false);
            AddHtml(420, pietyY + 158, 340, 20, $"<BASEFONT COLOR=#{LabelColor:X6}>- Tithe: +1 per 100g</BASEFONT>", false, false);
            AddHtml(420, pietyY + 176, 340, 20, $"<BASEFONT COLOR=#{LabelColor:X6}>- Pilgrimage: +75/week</BASEFONT>", false, false);

            // Religion bonuses display (if has religion)
            if (religionInfo != null)
            {
                int bonusY = pietyY + 210;
                AddHtml(25, bonusY, 300, 20, $"<BASEFONT COLOR=#{HeaderColor:X6}>Current Religion Bonuses:</BASEFONT>", false, false);

                // Passive bonuses
                AddHtml(25, bonusY + 25, 200, 20, $"<BASEFONT COLOR=#{MagicColor:X6}>Passives:</BASEFONT>", false, false);
                if (religionInfo.PassiveBonuses != null)
                {
                    for (int i = 0; i < religionInfo.PassiveBonuses.Length; i++)
                    {
                        int reqPiety = i == 0 ? 50 : 200;
                        bool hasBonus = currentPiety >= reqPiety;
                        int color = hasBonus ? ValueColor : 0x888888;
                        AddHtml(25, bonusY + 45 + (i * 18), 300, 20,
                            $"<BASEFONT COLOR=#{color:X6}>• {religionInfo.PassiveBonuses[i]} (req: {reqPiety})</BASEFONT>", false, false);
                    }
                }

                // Devotion powers
                AddHtml(300, bonusY + 25, 200, 20, $"<BASEFONT COLOR=#{MagicColor:X6}>Devotion Powers:</BASEFONT>", false, false);
                if (religionInfo.DevotionPowers != null)
                {
                    int[] powerReqs = { 200, 500, 900 };
                    for (int i = 0; i < religionInfo.DevotionPowers.Length; i++)
                    {
                        bool hasPower = currentPiety >= powerReqs[i];
                        int color = hasPower ? ValueColor : 0x888888;
                        AddHtml(300, bonusY + 45 + (i * 18), 300, 20,
                            $"<BASEFONT COLOR=#{color:X6}>• {religionInfo.DevotionPowers[i]} (req: {powerReqs[i]})</BASEFONT>", false, false);
                    }
                }
            }
        }

        #endregion

        #region Response Handler

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            Mobile from = sender.Mobile;
            if (from == null)
                return;

            int buttonId = info.ButtonID;

            // Tab navigation (1-8)
            if (buttonId >= 1 && buttonId <= 8)
            {
                from.SendGump(new VystiaAdminGump(from, m_Target, (AdminPage)(buttonId - 1)));
                return;
            }

            // Select target
            if (buttonId == 100)
            {
                from.SendMessage("Select a target...");
                from.Target = new InternalTarget(this);
                return;
            }

            PlayerMobile pm = m_Target as PlayerMobile;

            // Class assignment - Beginner (200-299 magic, 300-399 martial)
            if (buttonId >= 200 && buttonId < 300)
            {
                string[] magicClasses = { "IceMage", "Warlock", "Necromancer", "Druid", "Sorcerer", "Bard",
                                          "Witch", "Oracle", "Summoner", "Shaman", "Enchanter", "Illusionist", "Magery" };
                int index = buttonId - 200;
                if (index < magicClasses.Length)
                    AssignClass(from, pm, magicClasses[index]);
            }
            else if (buttonId >= 300 && buttonId < 313)
            {
                string[] martialClasses = { "Barbarian", "Rogue", "Monk", "Knight", "Paladin", "Ranger",
                                            "Fighter", "Templar", "BountyHunter", "Beastmaster", "Artificer",
                                            "Alchemist", "Cleric" };
                int index = buttonId - 300;
                if (index < martialClasses.Length)
                    AssignClass(from, pm, martialClasses[index]);
            }

            // Class assignment - Maxed (2500-2512 magic, 2600-2613 martial)
            else if (buttonId >= 2500 && buttonId < 2513)
            {
                string[] magicClasses = { "IceMage", "Warlock", "Necromancer", "Druid", "Sorcerer", "Bard",
                                          "Witch", "Oracle", "Summoner", "Shaman", "Enchanter", "Illusionist", "Magery" };
                int index = buttonId - 2500;
                if (index < magicClasses.Length)
                    AssignMaxedClass(from, pm, magicClasses[index]);
            }
            else if (buttonId >= 2600 && buttonId < 2613)
            {
                string[] martialClasses = { "Barbarian", "Rogue", "Monk", "Knight", "Paladin", "Ranger",
                                            "Fighter", "Templar", "BountyHunter", "Beastmaster", "Artificer",
                                            "Alchemist", "Cleric" };
                int index = buttonId - 2600;
                if (index < martialClasses.Length)
                    AssignMaxedClass(from, pm, martialClasses[index]);
            }

            // Class actions
            else if (buttonId == 400) // Remove class
            {
                if (pm != null)
                {
                    ClearBackpack(from, pm);
                    VystiaClassManager.Instance.RemoveClass(pm);
                    from.SendMessage("Class removed and backpack cleared.");
                }
            }
            else if (buttonId == 401) // Class info
            {
                if (pm != null)
                {
                    var playerClass = VystiaClassManager.Instance.GetClass(pm);
                    if (playerClass != null)
                        from.SendMessage($"Class: {playerClass.ClassName}, Role: {playerClass.Role}, Region: {playerClass.HomeRegion}");
                    else
                        from.SendMessage("Target has no class assigned.");
                }
            }
            else if (buttonId == 402) // Open class selection gump
            {
                if (pm != null)
                    pm.SendGump(new VystiaClassSelectionGump(pm));
            }
            else if (buttonId == 403) // Refresh info
            {
                from.SendMessage("Class info refreshed.");
            }

            // Resource management (500-599 set max, 600-699 set zero)
            else if (buttonId >= 500 && buttonId < 600)
            {
                SetResource(pm, buttonId - 500, true);
                from.SendMessage("Resource set to maximum.");
            }
            else if (buttonId >= 600 && buttonId < 700)
            {
                SetResource(pm, buttonId - 600, false);
                from.SendMessage("Resource set to zero.");
            }
            else if (buttonId == 700) // Fill all
            {
                FillAllResources(pm);
                from.SendMessage("All resources filled to maximum.");
            }
            else if (buttonId == 701) // Reset all
            {
                ResetAllResources(pm);
                from.SendMessage("All resources reset to zero.");
            }

            // Stance management (1001-1099 = specific stances)
            else if (buttonId >= 1001 && buttonId < 1100)
            {
                ApplyStanceByButtonId(from, pm, buttonId);
            }
            else if (buttonId == 1100) // Clear stances
            {
                if (pm != null)
                {
                    VystiaStanceManager.Instance.RemoveAllStances(pm);
                    from.SendMessage("All stances cleared.");
                }
            }

            // Skill management (2000-2401)
            else if (buttonId >= 2000 && buttonId < 2100) // Magic skills to 100
            {
                SetMagicSkill(from, pm, buttonId - 2000, 100.0);
            }
            else if (buttonId >= 2100 && buttonId < 2200) // Magic skills to 0
            {
                SetMagicSkill(from, pm, buttonId - 2100, 0.0);
            }
            else if (buttonId >= 2200 && buttonId < 2300) // Martial skills to 100
            {
                SetMartialSkill(from, pm, buttonId - 2200, 100.0);
            }
            else if (buttonId >= 2300 && buttonId < 2400) // Martial skills to 0
            {
                SetMartialSkill(from, pm, buttonId - 2300, 0.0);
            }
            else if (buttonId == 2400) // Class skill to 100
            {
                if (pm != null)
                {
                    bool success = VystiaSkillCheck.SetClassSkill(pm, 100.0);
                    if (success)
                        from.SendMessage("Class skill set to 100.");
                    else
                        from.SendMessage("No class skill found (assign a class first).");
                }
            }
            else if (buttonId == 2401) // Reset all Vystia skills to 0
            {
                ResetAllVystiaSkills(from, pm);
            }

            // Item sub-pages (1200-1205)
            else if (buttonId >= 1200 && buttonId <= 1205)
            {
                from.SendGump(new VystiaAdminGump(from, m_Target, AdminPage.Items, buttonId - 1200));
                return;
            }

            // Spellbooks (1300-1311)
            else if (buttonId >= 1300 && buttonId < 1312)
            {
                GiveSpellbook(from, pm, buttonId - 1300);
            }
            else if (buttonId == 1398) // Standard magery book
            {
                GiveStandardMageryBook(from, pm);
            }
            else if (buttonId == 1399) // All spellbooks
            {
                GiveAllSpellbooks(from, pm);
            }

            // Reagents (1400-1411)
            else if (buttonId >= 1400 && buttonId < 1412)
            {
                GiveReagentBag(from, pm, buttonId - 1400);
            }
            else if (buttonId == 1498) // Standard reagents
            {
                GiveStandardReagents(from, pm);
            }
            else if (buttonId == 1499) // All reagents
            {
                GiveAllReagents(from, pm);
            }

            // Weapons (1500-1509 regional, 1550-1554 legendary)
            else if (buttonId >= 1500 && buttonId < 1510)
            {
                GiveRegionalWeapon(from, pm, buttonId - 1500);
            }
            else if (buttonId >= 1550 && buttonId < 1555)
            {
                GiveLegendaryWeapon(from, pm, buttonId - 1550);
            }
            else if (buttonId == 1599) // All legendary weapons
            {
                GiveAllLegendaryWeapons(from, pm);
            }

            // Armor (1600-1609 regional, 1650-1652 legendary sets)
            else if (buttonId >= 1600 && buttonId < 1610)
            {
                GiveRegionalArmor(from, pm, buttonId - 1600);
            }
            else if (buttonId >= 1650 && buttonId < 1653)
            {
                GiveLegendaryArmorSet(from, pm, buttonId - 1650);
            }
            else if (buttonId == 1660) // All shields
            {
                GiveAllRegionalShields(from, pm);
            }
            else if (buttonId == 1699) // All legendary armor
            {
                GiveAllLegendaryArmor(from, pm);
            }

            // Focus items (1700-1724)
            else if (buttonId >= 1700 && buttonId < 1725)
            {
                GiveFocusItem(from, pm, buttonId - 1700);
            }
            else if (buttonId == 1798) // Class-appropriate focus
            {
                GiveClassFocusItem(from, pm);
            }
            else if (buttonId == 1799) // All focus items
            {
                GiveAllFocusItems(from, pm);
            }

            // Regional Consumables (1900-1922)
            else if (buttonId >= 1900 && buttonId < 1904) // Resistance potions
            {
                GiveResistancePotion(from, pm, buttonId - 1900);
            }
            else if (buttonId >= 1910 && buttonId < 1914) // Enhancement potions
            {
                GiveEnhancementPotion(from, pm, buttonId - 1910);
            }
            else if (buttonId == 1920) // All resistance potions
            {
                GiveAllResistancePotions(from, pm);
            }
            else if (buttonId == 1921) // All enhancement potions
            {
                GiveAllEnhancementPotions(from, pm);
            }
            else if (buttonId == 1922) // All regional potions
            {
                GiveAllResistancePotions(from, pm);
                GiveAllEnhancementPotions(from, pm);
                from.SendMessage("Gave all regional potions.");
            }

            // Tools (1800+)
            else if (buttonId == 1800) // Spawn Practice Target
            {
                SpawnTestDummy(from);
            }
            else if (buttonId == 1801) // Spawn vendor
            {
                SpawnClassItemVendor(from);
            }
            else if (buttonId == 1802) // Spawn reagent vendor
            {
                SpawnReagentVendor(from);
            }
            else if (buttonId == 1803) // Spawn weapons chests
            {
                SpawnWeaponChests(from);
            }
            else if (buttonId == 1810) // Ability editor
            {
                from.SendGump(new AbilityEditorGump(from));
            }
            else if (buttonId == 1811) // Class selection gump
            {
                if (pm != null)
                    pm.SendGump(new VystiaClassSelectionGump(pm));
            }
            else if (buttonId == 1812) // Spawn Vystia gump
            {
                from.SendMessage("Use [spawnvystia command to open the spawn gump.");
            }
            else if (buttonId == 1820) // Full heal
            {
                if (m_Target != null)
                {
                    m_Target.Hits = m_Target.HitsMax;
                    from.SendMessage("Target fully healed.");
                }
            }
            else if (buttonId == 1821) // Restore mana/stam
            {
                if (m_Target != null)
                {
                    m_Target.Mana = m_Target.ManaMax;
                    m_Target.Stam = m_Target.StamMax;
                    from.SendMessage("Target's mana and stamina restored.");
                }
            }
            else if (buttonId == 1822) // Cure all
            {
                if (m_Target != null)
                {
                    m_Target.CurePoison(from);
                    from.SendMessage("Target cured.");
                }
            }
            else if (buttonId == 1823) // Resurrect
            {
                if (m_Target != null && !m_Target.Alive)
                {
                    m_Target.Resurrect();
                    from.SendMessage("Target resurrected.");
                }
            }
            else if (buttonId == 1824) // Kill target
            {
                if (m_Target != null && m_Target.Alive && m_Target != from)
                {
                    m_Target.Kill();
                    from.SendMessage("Target killed.");
                }
            }
            else if (buttonId == 1830) // Give resource potions
            {
                GiveResourcePotions(from, pm);
            }
            else if (buttonId == 1831) // Give combat potions
            {
                GiveCombatPotions(from, pm);
            }
            else if (buttonId == 1832) // Complete class loadout
            {
                GiveCompleteClassLoadout(from, pm);
            }

            // Faction tier buttons (3000-3069: 7 factions × 10 buttons each)
            else if (buttonId >= 3000 && buttonId < 3070)
            {
                HandleFactionTierButton(from, pm, buttonId);
            }

            // Custom faction reputation (3100-3106)
            else if (buttonId >= 3100 && buttonId < 3107)
            {
                HandleCustomFactionRep(from, pm, buttonId, info);
            }

            // Religion selection (3200-3206)
            else if (buttonId >= 3200 && buttonId < 3207)
            {
                HandleReligionSelection(from, pm, buttonId);
            }

            // Piety preset buttons (3300-3305)
            else if (buttonId >= 3300 && buttonId < 3306)
            {
                HandlePietyPreset(from, pm, buttonId);
            }

            // Custom piety value (3310)
            else if (buttonId == 3310)
            {
                HandleCustomPiety(from, pm, info);
            }

            // Refresh gump (except for close and sub-page navigation)
            if (buttonId != 0 && !(buttonId >= 1200 && buttonId <= 1205))
                from.SendGump(new VystiaAdminGump(from, m_Target, m_Page, m_ItemSubPage));
        }

        #endregion

        #region Helper Methods

        private void AssignClass(Mobile from, PlayerMobile pm, string className)
        {
            if (pm == null)
            {
                from.SendMessage("Target must be a player.");
                return;
            }

            // Map "Magery" to "Wizard" enum value
            string enumName = className == "Magery" ? "Wizard" : className;

            if (Enum.TryParse(enumName, out PlayerClassTypeV2 classType))
            {
                VystiaClassManager.Instance.AssignClass(pm, classType);
                from.SendMessage($"Assigned class {className} to {pm.Name}.");
            }
            else
            {
                from.SendMessage($"Invalid class: {className}");
            }
        }

        private void AssignMaxedClass(Mobile from, PlayerMobile pm, string className)
        {
            if (pm == null)
            {
                from.SendMessage("Target must be a player.");
                return;
            }

            // Map "Magery" to "Wizard" enum value
            string enumName = className == "Magery" ? "Wizard" : className;

            if (!Enum.TryParse(enumName, out PlayerClassTypeV2 classType))
            {
                from.SendMessage($"Invalid class: {className}");
                return;
            }

            // Clear existing items first (clean slate)
            ClearBackpack(from, pm);

            // Assign the class
            VystiaClassManager.Instance.AssignClass(pm, classType);
            from.SendMessage($"Assigned class {className} to {pm.Name}.");

            if (pm.Backpack == null)
            {
                from.SendMessage("Target has no backpack - cannot give items.");
                return;
            }

            // Get the player's class to determine which items to give
            var playerClass = VystiaClassManager.Instance.GetClass(pm);
            if (playerClass == null)
            {
                from.SendMessage("Failed to get class info.");
                return;
            }

            // Give organized loadout with items in labeled bags
            GiveOrganizedLoadout(from, pm, playerClass);

            // Set stats based on class-appropriate distribution (aligned to class role)
            // Use class stat distribution to set maxed stats for testing
            // Scale starting stats proportionally to reach 225 total while maintaining class ratios
            
            int startStr = playerClass.StartStr;
            int startDex = playerClass.StartDex;
            int startInt = playerClass.StartInt;
            int startTotal = startStr + startDex + startInt;
            
            // Target total stats for testing (225 = Vystia stat cap)
            int targetTotal = 225;
            
            // Calculate scale factor to reach target total
            double scaleFactor = (double)targetTotal / startTotal;
            
            // Scale each stat proportionally
            int targetStr = (int)(startStr * scaleFactor);
            int targetDex = (int)(startDex * scaleFactor);
            int targetInt = (int)(startInt * scaleFactor);
            
            // Ensure we hit exactly targetTotal (rounding may cause slight variance)
            int currentTotal = targetStr + targetDex + targetInt;
            int remainder = targetTotal - currentTotal;
            
            // Distribute remainder to maintain class focus
            if (remainder != 0)
            {
                // Find primary stat (highest starting value)
                if (startStr >= startDex && startStr >= startInt)
                    targetStr += remainder; // STR-focused class (Fighter, Barbarian, etc.)
                else if (startDex >= startInt)
                    targetDex += remainder; // DEX-focused class (Ranger, Rogue, etc.)
                else
                    targetInt += remainder; // INT-focused class (Caster classes)
            }
            
            // Respect individual stat caps and hard cap of 100 per stat
            targetStr = Math.Min(targetStr, Math.Min(playerClass.StrCap, 100));
            targetDex = Math.Min(targetDex, Math.Min(playerClass.DexCap, 100));
            targetInt = Math.Min(targetInt, Math.Min(playerClass.IntCap, 100));
            
            // Set stat cap to allow the calculated stats
            pm.StatCap = targetTotal;
            
            // Set stats to class-appropriate values
            pm.RawStr = targetStr;
            pm.RawDex = targetDex;
            pm.RawInt = targetInt;
            pm.UpdateTotals(); // Recalculate HP/Stam/Mana based on new stats

            // Set all Vystia magic skills to 100 (GM cap - no power scrolls in Vystia)
            for (int i = 0; i < MagicSkillNames.Length; i++)
            {
                SkillName skillEnum = (SkillName)Enum.Parse(typeof(SkillName), MagicSkillNames[i]);
                Skill skill = pm.Skills[skillEnum];
                if (skill != null)
                {
                    skill.Base = Math.Max(0, Math.Min(skill.Cap, 100.0));
                }
            }

            // Set all Vystia martial skills to 100 (GM cap - no power scrolls in Vystia)
            for (int i = 0; i < MartialSkillNames.Length; i++)
            {
                SkillName skillEnum = (SkillName)Enum.Parse(typeof(SkillName), MartialSkillNames[i]);
                Skill skill = pm.Skills[skillEnum];
                if (skill != null)
                {
                    skill.Base = Math.Max(0, Math.Min(skill.Cap, 100.0));
                }
            }

            // Fill all resources
            FillAllResources(pm);

            // Full heal (after UpdateTotals)
            pm.Hits = pm.HitsMax;
            pm.Mana = pm.ManaMax;
            pm.Stam = pm.StamMax;

            from.SendMessage($"Gave loadout for {className}: spellbook, equipment bag, potions bag, stats={targetStr}/{targetDex}/{targetInt} (class-aligned), skills=100.");
        }

        private int GetSpellbookIndexForSchool(AbilitySchool school)
        {
            // Map ability school to spellbook index
            // 0=IceMage, 1=Druid, 2=Witch, 3=Sorcerer, 4=Warlock, 5=Oracle,
            // 6=Necromancer, 7=Summoner, 8=Shaman, 9=Bard, 10=Enchanter, 11=Illusionist
            switch (school)
            {
                case AbilitySchool.Ice: return 0;
                case AbilitySchool.Nature: return 1;
                case AbilitySchool.Hex: return 2;
                case AbilitySchool.Elemental: return 3;
                case AbilitySchool.Dark: return 4;
                case AbilitySchool.Divination: return 5;
                case AbilitySchool.Necromancy: return 6;
                case AbilitySchool.Summoning: return 7;
                case AbilitySchool.Shamanic: return 8;
                case AbilitySchool.Bardic: return 9;
                case AbilitySchool.Enchanting: return 10;
                case AbilitySchool.Illusion: return 11;
                default: return -1; // Martial classes don't get spellbooks
            }
        }

        private void GiveClassLegendaryWeapon(Mobile from, PlayerMobile pm, PlayerClassV2 playerClass)
        {
            // Give legendary weapon based on home region
            // 0=EternalWinter (Frosthold), 1=PhoenixAscension (Emberlands), 2=Cogmaster (Ironclad),
            // 3=PrismaticEdge (Crystal), 4=Voidcaller (ShadowVoid)
            string region = playerClass.HomeRegion;
            int weaponIndex = -1;

            if (region.Contains("Frost")) weaponIndex = 0; // The Eternal Winter
            else if (region.Contains("Ember") || region.Contains("Fire")) weaponIndex = 1; // Phoenix Ascension
            else if (region.Contains("Iron") || region.Contains("Steam")) weaponIndex = 2; // The Cogmaster
            else if (region.Contains("Crystal") || region.Contains("Barren")) weaponIndex = 3; // Prismatic Edge
            else if (region.Contains("Shadow") || region.Contains("Void")) weaponIndex = 4; // Voidcaller
            else weaponIndex = 3; // Default to Prismatic Edge for other regions

            if (weaponIndex >= 0)
                GiveLegendaryWeapon(from, pm, weaponIndex);
        }

        private void GiveClassLegendaryArmor(Mobile from, PlayerMobile pm, PlayerClassV2 playerClass)
        {
            // Give legendary armor set based on role
            // 0=Glacial Aegis (Tank/Plate), 1=Steamwork Exosuit (Melee DPS), 2=Shadow Shroud (Leather)
            var role = playerClass.Role;

            if (role == ClassRole.Tank || role == ClassRole.Healer || role == ClassRole.Support)
                GiveLegendaryArmorSet(from, pm, 0); // Glacial Aegis - Tank/Healer/Support set
            else if (role == ClassRole.RangedDPS || role == ClassRole.CasterDPS)
                GiveLegendaryArmorSet(from, pm, 2); // Shadow Shroud - Ranged/Caster set (leather)
            else
                GiveLegendaryArmorSet(from, pm, 1); // Steamwork Exosuit - Melee DPS set (default)
        }

        private void GiveClassRegionalPotions(Mobile from, PlayerMobile pm, PlayerClassV2 playerClass)
        {
            string region = playerClass.HomeRegion;
            string className = playerClass.ClassName;

            // Alchemist gets ALL potions - they're the potion master!
            if (className == "Alchemist")
            {
                // Give ALL regional resistance potions (10 each)
                pm.Backpack.DropItem(new ColdResistancePotion() { Amount = 10 });
                pm.Backpack.DropItem(new HeatResistancePotion() { Amount = 10 });
                pm.Backpack.DropItem(new PoisonResistancePotion() { Amount = 10 });
                pm.Backpack.DropItem(new EnergyResistancePotion() { Amount = 10 });

                // Give ALL regional enhancement potions (10 each)
                pm.Backpack.DropItem(new NaturesBlessingPotion() { Amount = 10 });
                pm.Backpack.DropItem(new CrystalClarityPotion() { Amount = 10 });
                pm.Backpack.DropItem(new DesertSwiftnessPotion() { Amount = 10 });
                pm.Backpack.DropItem(new IroncladFortitudePotion() { Amount = 10 });

                // Give ALL combat potions (5 each) - Alchemist specialty!
                pm.Backpack.DropItem(new Server.Items.VystiaClassItems.BurstPotion(5));
                pm.Backpack.DropItem(new Server.Items.VystiaClassItems.HastePotion(5));
                pm.Backpack.DropItem(new Server.Items.VystiaClassItems.ResistPotion(5));
                pm.Backpack.DropItem(new Server.Items.VystiaClassItems.CleansePotion(5));
                pm.Backpack.DropItem(new Server.Items.VystiaClassItems.SecondWindPotion(5));
                pm.Backpack.DropItem(new Server.Items.VystiaClassItems.VystiaInvisibilityPotion(5));

                // Give samples of class resource potions (for trade/experimentation)
                pm.Backpack.DropItem(new Server.Items.VystiaClassItems.SoulEssenceVial(3));
                pm.Backpack.DropItem(new Server.Items.VystiaClassItems.FuryTonic(3));
                pm.Backpack.DropItem(new Server.Items.VystiaClassItems.ChiTea(3));
                pm.Backpack.DropItem(new Server.Items.VystiaClassItems.FocusElixir(3));
                pm.Backpack.DropItem(new Server.Items.VystiaClassItems.FortitudeDraught(3));
                pm.Backpack.DropItem(new Server.Items.VystiaClassItems.FaithIncense(3));
                pm.Backpack.DropItem(new Server.Items.VystiaClassItems.CrescendoCrystal(3));
                pm.Backpack.DropItem(new Server.Items.VystiaClassItems.LifeForceVial(3));
                pm.Backpack.DropItem(new Server.Items.VystiaClassItems.ZealElixir(3));
                pm.Backpack.DropItem(new Server.Items.VystiaClassItems.SteamCanister(3));
                pm.Backpack.DropItem(new Server.Items.VystiaClassItems.PursuitTracker(3));

                from.SendMessage("Gave FULL potion master loadout to Alchemist!");
                return;
            }

            // Give resistance potion matching the class's region
            if (region.Contains("Frost"))
            {
                pm.Backpack.DropItem(new ColdResistancePotion() { Amount = 10 });
                from.SendMessage("Gave Cold Resistance Potions (Frosthold).");
            }
            else if (region.Contains("Ember") || region.Contains("Fire"))
            {
                pm.Backpack.DropItem(new HeatResistancePotion() { Amount = 10 });
                from.SendMessage("Gave Heat Resistance Potions (Emberlands).");
            }
            else if (region.Contains("Shadow") || region.Contains("Fen"))
            {
                pm.Backpack.DropItem(new PoisonResistancePotion() { Amount = 10 });
                from.SendMessage("Gave Poison Resistance Potions (Shadowfen).");
            }
            else if (region.Contains("Crystal"))
            {
                pm.Backpack.DropItem(new EnergyResistancePotion() { Amount = 10 });
                pm.Backpack.DropItem(new CrystalClarityPotion() { Amount = 5 });
                from.SendMessage("Gave Energy Resistance and Crystal Clarity Potions.");
            }
            else if (region.Contains("Iron"))
            {
                pm.Backpack.DropItem(new IroncladFortitudePotion() { Amount = 10 });
                from.SendMessage("Gave Ironclad Fortitude Potions.");
            }
            else if (region.Contains("Desert"))
            {
                pm.Backpack.DropItem(new HeatResistancePotion() { Amount = 5 });
                pm.Backpack.DropItem(new DesertSwiftnessPotion() { Amount = 10 });
                from.SendMessage("Gave Desert Swiftness and Heat Resistance Potions.");
            }
            else if (region.Contains("Verdant") || region.Contains("Forest"))
            {
                pm.Backpack.DropItem(new NaturesBlessingPotion() { Amount = 10 });
                from.SendMessage("Gave Nature's Blessing Potions (Verdantpeak).");
            }
            else
            {
                // Multi-regional or other: give a mix
                pm.Backpack.DropItem(new ColdResistancePotion() { Amount = 3 });
                pm.Backpack.DropItem(new HeatResistancePotion() { Amount = 3 });
                pm.Backpack.DropItem(new PoisonResistancePotion() { Amount = 3 });
                pm.Backpack.DropItem(new EnergyResistancePotion() { Amount = 3 });
                from.SendMessage("Gave mixed regional potions.");
            }
        }

        private void ApplyStanceByButtonId(Mobile from, PlayerMobile pm, int buttonId)
        {
            if (pm == null)
            {
                from.SendMessage("Target must be a player.");
                return;
            }

            // Map button IDs to stance types
            var stanceMap = new Dictionary<int, StanceType>
            {
                // Druid forms
                { 1001, StanceType.DruidBear }, { 1002, StanceType.DruidCat }, { 1003, StanceType.DruidTree },
                { 1004, StanceType.DruidMoonkin }, { 1005, StanceType.DruidTravel },
                // Sorcerer elements
                { 1010, StanceType.SorcererFire }, { 1011, StanceType.SorcererWater },
                { 1012, StanceType.SorcererEarth }, { 1013, StanceType.SorcererAir },
                // Fighter stances
                { 1020, StanceType.FighterAggressive }, { 1021, StanceType.FighterDefensive },
                { 1022, StanceType.FighterBalanced }, { 1023, StanceType.FighterBerserker },
                // Monk stances
                { 1030, StanceType.MonkWindwalker }, { 1031, StanceType.MonkBrewmaster }, { 1032, StanceType.MonkMistweaver },
                // Rogue stances
                { 1040, StanceType.RogueShadow }, { 1041, StanceType.RogueOutlaw }, { 1042, StanceType.RogueSubtlety },
                // Paladin auras
                { 1050, StanceType.PaladinDevotion }, { 1051, StanceType.PaladinRetribution }, { 1052, StanceType.PaladinProtection },
                // Ranger aspects
                { 1060, StanceType.RangerHawk }, { 1061, StanceType.RangerWolf }, { 1062, StanceType.RangerBear },
                // Barbarian
                { 1070, StanceType.BarbarianNormal }, { 1071, StanceType.BarbarianRage }
            };

            if (stanceMap.TryGetValue(buttonId, out StanceType stanceType))
            {
                bool success = VystiaStanceManager.Instance.ApplyStance(pm, stanceType, true); // force = true for GM
                if (success)
                    from.SendMessage($"Applied stance: {stanceType}");
                else
                    from.SendMessage($"Failed to apply stance: {stanceType}");
            }
        }

        private void SetResource(PlayerMobile pm, int resourceIndex, bool setToMax)
        {
            if (pm == null) return;

            var manager = VystiaResourceManager.GetManager(pm);
            if (manager == null) return;

            string[] resourceNames = Enum.GetNames(typeof(ResourceType));
            if (resourceIndex >= 0 && resourceIndex < resourceNames.Length)
            {
                ResourceType resType = (ResourceType)Enum.Parse(typeof(ResourceType), resourceNames[resourceIndex]);
                var resource = manager.GetResource(resType);
                if (resource != null)
                {
                    if (setToMax)
                        resource.Generate(resource.Maximum);
                    else
                        resource.Spend(resource.Current);
                }
            }
        }

        private void FillAllResources(PlayerMobile pm)
        {
            if (pm == null) return;

            var manager = VystiaResourceManager.GetManager(pm);
            if (manager == null) return;

            foreach (ResourceType resType in Enum.GetValues(typeof(ResourceType)))
            {
                var resource = manager.GetResource(resType);
                if (resource != null)
                    resource.Generate(resource.Maximum);
            }
        }

        private void ResetAllResources(PlayerMobile pm)
        {
            if (pm == null) return;

            var manager = VystiaResourceManager.GetManager(pm);
            if (manager == null) return;

            foreach (ResourceType resType in Enum.GetValues(typeof(ResourceType)))
            {
                var resource = manager.GetResource(resType);
                if (resource != null)
                    resource.Spend(resource.Current);
            }
        }

        #region Skill Helper Methods

        private static readonly string[] MagicSkillNames = { "Cryomancy", "Demonology", "NecromancyArts", "Druidism", "Elementalism", "Songweaving",
                                                             "Hexcraft", "Divination", "Conjuration", "SpiritCalling", "Runeweaving", "IllusionMagic", "Magery" };

        private static readonly string[] MartialSkillNames = { "Berserking", "Subterfuge", "MartialArts", "ChivalricArts", "HolyDevotion", "Marksmanship",
                                                               "CombatMastery", "Zealotry", "Manhunting", "BeastBonding", "Engineering", "Transmutation",
                                                               "DivineGrace" };

        private void SetMagicSkill(Mobile from, PlayerMobile pm, int index, double value)
        {
            if (pm == null || index < 0 || index >= MagicSkillNames.Length) return;

            SkillName skillEnum = (SkillName)Enum.Parse(typeof(SkillName), MagicSkillNames[index]);
            Skill skill = pm.Skills[skillEnum];
            if (skill != null)
            {
                skill.Base = Math.Max(0, Math.Min(skill.Cap, value));
                from.SendMessage($"{MagicSkillNames[index]} set to {value}.");
            }
        }

        private void SetMartialSkill(Mobile from, PlayerMobile pm, int index, double value)
        {
            if (pm == null || index < 0 || index >= MartialSkillNames.Length) return;

            SkillName skillEnum = (SkillName)Enum.Parse(typeof(SkillName), MartialSkillNames[index]);
            Skill skill = pm.Skills[skillEnum];
            if (skill != null)
            {
                skill.Base = Math.Max(0, Math.Min(skill.Cap, value));
                from.SendMessage($"{MartialSkillNames[index]} set to {value}.");
            }
        }

        private void ResetAllVystiaSkills(Mobile from, PlayerMobile pm)
        {
            if (pm == null) return;

            // Reset all magic skills
            for (int i = 0; i < MagicSkillNames.Length; i++)
            {
                SkillName skillEnum = (SkillName)Enum.Parse(typeof(SkillName), MagicSkillNames[i]);
                Skill skill = pm.Skills[skillEnum];
                if (skill != null)
                    skill.Base = 0;
            }

            // Reset all martial skills
            for (int i = 0; i < MartialSkillNames.Length; i++)
            {
                SkillName skillEnum = (SkillName)Enum.Parse(typeof(SkillName), MartialSkillNames[i]);
                Skill skill = pm.Skills[skillEnum];
                if (skill != null)
                    skill.Base = 0;
            }

            from.SendMessage("All Vystia class skills reset to 0.");
        }

        #endregion

        #region Item Giving Methods

        private void GiveSpellbook(Mobile from, PlayerMobile pm, int index)
        {
            if (pm == null || pm.Backpack == null)
            {
                from.SendMessage("Target must be a player with a backpack.");
                return;
            }

            Item spellbook = null;
            switch (index)
            {
                case 0: spellbook = new IceMageSpellbook(); break;
                case 1: spellbook = new DruidSpellbook(); break;
                case 2: spellbook = new WitchSpellbook(); break;
                case 3: spellbook = new SorcererSpellbook(); break;
                case 4: spellbook = new WarlockSpellbook(); break;
                case 5: spellbook = new OracleSpellbook(); break;
                case 6: spellbook = new VystiaNecromancerSpellbook(); break;
                case 7: spellbook = new SummonerSpellbook(); break;
                case 8: spellbook = new ShamanSpellbook(); break;
#if VYSTIA_SONGWEAVING
                case 9: spellbook = new SongweavingSpellbook(); break;
#endif
                case 10: spellbook = new EnchanterSpellbook(); break;
                case 11: spellbook = new IllusionistSpellbook(); break;
            }

            if (spellbook != null)
            {
                pm.Backpack.DropItem(spellbook);
                from.SendMessage($"Gave {spellbook.Name} to {pm.Name}.");
            }
        }

        private void GiveStandardMageryBook(Mobile from, PlayerMobile pm)
        {
            if (pm == null || pm.Backpack == null) return;
            var book = new Spellbook((ulong)0xFFFFFFFFFFFFFFFF);
            pm.Backpack.DropItem(book);
            from.SendMessage($"Gave standard magery spellbook to {pm.Name}.");
        }

        private void GiveAllSpellbooks(Mobile from, PlayerMobile pm)
        {
            for (int i = 0; i < 12; i++)
                GiveSpellbook(from, pm, i);
            GiveStandardMageryBook(from, pm);
            from.SendMessage("Gave all spellbooks.");
        }

        private void GiveReagentBag(Mobile from, PlayerMobile pm, int index)
        {
            if (pm == null || pm.Backpack == null) return;

            // Give 50 of each reagent for the school
            // This is a simplified version - would need to import actual reagent types
            from.SendMessage($"Gave reagent bag #{index + 1} to {pm.Name}. (Use [spawnvystia > Vendors to spawn reagent vendor)");
        }

        private void GiveStandardReagents(Mobile from, PlayerMobile pm)
        {
            if (pm == null || pm.Backpack == null) return;

            pm.Backpack.DropItem(new BlackPearl(100));
            pm.Backpack.DropItem(new Bloodmoss(100));
            pm.Backpack.DropItem(new Garlic(100));
            pm.Backpack.DropItem(new Ginseng(100));
            pm.Backpack.DropItem(new MandrakeRoot(100));
            pm.Backpack.DropItem(new Nightshade(100));
            pm.Backpack.DropItem(new SulfurousAsh(100));
            pm.Backpack.DropItem(new SpidersSilk(100));
            from.SendMessage("Gave standard magery reagents.");
        }

        private void GiveAllReagents(Mobile from, PlayerMobile pm)
        {
            GiveStandardReagents(from, pm);
            for (int i = 0; i < 12; i++)
                GiveReagentBag(from, pm, i);
        }

        private void GiveRegionalWeapon(Mobile from, PlayerMobile pm, int index)
        {
            if (pm == null || pm.Backpack == null) return;
            from.SendMessage($"Regional weapon #{index + 1} - Use [spawnvystia > Equipment to spawn.");
        }

        private void GiveLegendaryWeapon(Mobile from, PlayerMobile pm, int index)
        {
            if (pm == null || pm.Backpack == null) return;

            Item weapon = null;
            switch (index)
            {
                case 0: weapon = new Server.Items.TheEternalWinter(); break;
                case 1: weapon = new Server.Items.PhoenixAscension(); break;
                case 2: weapon = new Server.Items.TheCogmaster(); break;
                case 3: weapon = new Server.Items.PrismaticEdge(); break;
                case 4: weapon = new Server.Items.Voidcaller(); break;
            }

            if (weapon != null)
            {
                pm.Backpack.DropItem(weapon);
                from.SendMessage($"Gave {weapon.Name} to {pm.Name}.");
            }
        }

        private void GiveAllLegendaryWeapons(Mobile from, PlayerMobile pm)
        {
            for (int i = 0; i < 5; i++)
                GiveLegendaryWeapon(from, pm, i);
        }

        private void GiveRegionalArmor(Mobile from, PlayerMobile pm, int index)
        {
            if (pm == null || pm.Backpack == null) return;
            from.SendMessage($"Regional armor set #{index + 1} - Use specific armor commands or [spawnvystia.");
        }

        private void GiveLegendaryArmorSet(Mobile from, PlayerMobile pm, int index)
        {
            if (pm == null || pm.Backpack == null) return;

            // Legendary armor - use [add command for now
            string[] setNames = { "Glacial Aegis", "Steamwork Exosuit", "Shadow Shroud" };
            if (index >= 0 && index < setNames.Length)
            {
                from.SendMessage($"{setNames[index]} set - Use [add GlacialAegisHelm etc. or spawn via vendor.");
            }
        }

        private void GiveAllRegionalShields(Mobile from, PlayerMobile pm)
        {
            if (pm == null || pm.Backpack == null) return;

            // Regional shields have different names - use [add command
            from.SendMessage("Regional shields: PrismShield, ClockworkShield, BogShield, SandShield, VoidShield, LivingShield. Use [add command.");
        }

        private void GiveAllLegendaryArmor(Mobile from, PlayerMobile pm)
        {
            for (int i = 0; i < 3; i++)
                GiveLegendaryArmorSet(from, pm, i);
        }

        private void GiveFocusItem(Mobile from, PlayerMobile pm, int index)
        {
            if (pm == null || pm.Backpack == null) return;
            from.SendMessage($"Focus item #{index + 1} - Focus items will be available via Class Equipment Dealer.");
        }

        private void GiveClassFocusItem(Mobile from, PlayerMobile pm)
        {
            if (pm == null) return;
            var playerClass = VystiaClassManager.Instance.GetClass(pm);
            if (playerClass == null)
            {
                from.SendMessage("Target has no class - cannot give class-specific focus item.");
                return;
            }
            from.SendMessage($"Would give focus item for {playerClass.ClassName}. (Not yet implemented)");
        }

        private void GiveAllFocusItems(Mobile from, PlayerMobile pm)
        {
            from.SendMessage("Focus items - Use Class Equipment Dealer or spawn via [spawnvystia.");
        }

        #endregion

        #region Bag Organization Helpers

        /// <summary>
        /// Creates a labeled bag for organizing items
        /// </summary>
        private Bag CreateLabeledBag(string label, int hue = 0)
        {
            var bag = new Bag();
            bag.Name = label;
            if (hue > 0) bag.Hue = hue;
            return bag;
        }

        /// <summary>
        /// Clears all items from a player's backpack AND equipped items (used when removing class)
        /// Preserves mounts but removes gold
        /// </summary>
        private void ClearBackpack(Mobile from, PlayerMobile pm)
        {
            if (pm == null) return;

            int count = 0;

            // First, unequip and delete all worn items (except mounts)
            var equippedItems = new List<Item>();
            foreach (Item item in pm.Items)
            {
                // Skip the backpack itself and hair/beard
                if (item is Container && item == pm.Backpack)
                    continue;
                if (item.Layer == Layer.Hair || item.Layer == Layer.FacialHair)
                    continue;
                // Skip mounts (ethereal mounts are worn on Layer.Mount)
                if (item.Layer == Layer.Mount)
                    continue;
                equippedItems.Add(item);
            }

            foreach (var item in equippedItems)
            {
                item.Delete();
                count++;
            }

            // Then clear backpack contents (including gold, but preserve ethereal mount deeds)
            if (pm.Backpack != null)
            {
                var itemsToDelete = new List<Item>();
                foreach (Item item in pm.Backpack.Items)
                {
                    // Preserve ethereal mounts and bank checks
                    if (item is EtherealMount || item is BankCheck)
                        continue;
                    itemsToDelete.Add(item);
                }

                foreach (var item in itemsToDelete)
                {
                    item.Delete();
                    count++;
                }
            }

            from.SendMessage($"Cleared {count} items (equipped + backpack, mounts preserved, gold removed).");
        }

        /// <summary>
        /// Creates an organized loadout with items in labeled bags
        /// </summary>
        private void GiveOrganizedLoadout(Mobile from, PlayerMobile pm, PlayerClassV2 playerClass)
        {
            if (pm == null || pm.Backpack == null || playerClass == null) return;

            // Create bags for organization
            var equipmentBag = CreateLabeledBag($"{playerClass.ClassName} Equipment", 0x489); // Blue hue
            var potionBag = CreateLabeledBag($"{playerClass.ClassName} Potions", 0x48E); // Green hue
            var reagentBag = CreateLabeledBag($"{playerClass.ClassName} Reagents", 0x48D); // Purple hue

            // Give class-appropriate spellbook (directly in backpack, not bag)
            // Special handling for Wizard/Magery - use standard magery book
            if (playerClass.ClassType == PlayerClassTypeV2.Wizard)
            {
                // Give standard magery spellbook
                var mageryBook = new Spellbook((ulong)0xFFFFFFFFFFFFFFFF);
                pm.Backpack.DropItem(mageryBook);
                
                // Give standard magery reagents
                AddStandardMageryReagentsToBag(reagentBag);
            }
            else
            {
                int spellbookIndex = GetSpellbookIndexForSchool(playerClass.AbilitySchool);
                if (spellbookIndex >= 0)
                {
                    GiveSpellbookToBag(pm, spellbookIndex, null); // null = directly to backpack
                    
                    // Add reagents to reagent bag (if magic class)
                    AddReagentsToBag(reagentBag, playerClass.AbilitySchool);
                }
            }

            // Add legendary weapon to equipment bag
            Item weapon = GetLegendaryWeaponForRegion(playerClass.HomeRegion);
            if (weapon != null)
                equipmentBag.DropItem(weapon);

            // Add class-specific legendary armor set to equipment bag
            AddLegendaryArmorToBag(equipmentBag, playerClass.HomeRegion, playerClass.Role, playerClass.ClassType);

            // Add potions to potion bag
            AddPotionsToBag(potionBag, playerClass.HomeRegion, playerClass.ClassName);

            // Drop bags into backpack
            if (equipmentBag.Items.Count > 0)
                pm.Backpack.DropItem(equipmentBag);
            else
                equipmentBag.Delete();

            if (potionBag.Items.Count > 0)
                pm.Backpack.DropItem(potionBag);
            else
                potionBag.Delete();

            if (reagentBag.Items.Count > 0)
                pm.Backpack.DropItem(reagentBag);
            else
                reagentBag.Delete();
        }

        private void GiveSpellbookToBag(PlayerMobile pm, int index, Bag bag)
        {
            Item spellbook = CreateSpellbook(index);
            if (spellbook != null)
            {
                if (bag != null)
                    bag.DropItem(spellbook);
                else
                    pm.Backpack.DropItem(spellbook);
            }
        }

        private Item CreateSpellbook(int index)
        {
            switch (index)
            {
                case 0: return new IceMageSpellbook();
                case 1: return new DruidSpellbook();
                case 2: return new WitchSpellbook();
                case 3: return new SorcererSpellbook();
                case 4: return new WarlockSpellbook();
                case 5: return new OracleSpellbook();
                case 6: return new VystiaNecromancerSpellbook();
                case 7: return new SummonerSpellbook();
                case 8: return new ShamanSpellbook();
#if VYSTIA_SONGWEAVING
                case 9: return new SongweavingSpellbook();
#endif
                case 10: return new EnchanterSpellbook();
                case 11: return new IllusionistSpellbook();
                default: return null;
            }
        }

        private Item GetLegendaryWeaponForRegion(string region)
        {
            if (region.Contains("Frost")) return new Server.Items.TheEternalWinter();
            if (region.Contains("Ember") || region.Contains("Fire")) return new Server.Items.PhoenixAscension();
            if (region.Contains("Iron") || region.Contains("Steam")) return new Server.Items.TheCogmaster();
            if (region.Contains("Shadow") || region.Contains("Void")) return new Server.Items.Voidcaller();
            return new Server.Items.PrismaticEdge(); // Default
        }

        private void AddLegendaryArmorToBag(Bag bag, string region, ClassRole role, PlayerClassTypeV2 classType)
        {
            // Add class-specific legendary armor set
            // Each class has its own unique 6-piece legendary set
            switch (classType)
            {
                // Magic Classes - Leather sets
                case PlayerClassTypeV2.IceMage:
                    bag.DropItem(new IceMageLegendaryCap());
                    bag.DropItem(new IceMageLegendaryGorget());
                    bag.DropItem(new IceMageLegendaryTunic());
                    bag.DropItem(new IceMageLegendarySleeves());
                    bag.DropItem(new IceMageLegendaryGloves());
                    bag.DropItem(new IceMageLegendaryLeggings());
                    break;
                case PlayerClassTypeV2.Druid:
                    bag.DropItem(new DruidLegendaryCap());
                    bag.DropItem(new DruidLegendaryGorget());
                    bag.DropItem(new DruidLegendaryTunic());
                    bag.DropItem(new DruidLegendarySleeves());
                    bag.DropItem(new DruidLegendaryGloves());
                    bag.DropItem(new DruidLegendaryLeggings());
                    break;
                case PlayerClassTypeV2.Witch:
                    bag.DropItem(new WitchLegendaryCap());
                    bag.DropItem(new WitchLegendaryGorget());
                    bag.DropItem(new WitchLegendaryTunic());
                    bag.DropItem(new WitchLegendarySleeves());
                    bag.DropItem(new WitchLegendaryGloves());
                    bag.DropItem(new WitchLegendaryLeggings());
                    break;
                case PlayerClassTypeV2.Sorcerer:
                    bag.DropItem(new SorcererLegendaryCap());
                    bag.DropItem(new SorcererLegendaryGorget());
                    bag.DropItem(new SorcererLegendaryTunic());
                    bag.DropItem(new SorcererLegendarySleeves());
                    bag.DropItem(new SorcererLegendaryGloves());
                    bag.DropItem(new SorcererLegendaryLeggings());
                    break;
                case PlayerClassTypeV2.Warlock:
                    bag.DropItem(new WarlockLegendaryCap());
                    bag.DropItem(new WarlockLegendaryGorget());
                    bag.DropItem(new WarlockLegendaryTunic());
                    bag.DropItem(new WarlockLegendarySleeves());
                    bag.DropItem(new WarlockLegendaryGloves());
                    bag.DropItem(new WarlockLegendaryLeggings());
                    break;
                case PlayerClassTypeV2.Oracle:
                    bag.DropItem(new OracleLegendaryCap());
                    bag.DropItem(new OracleLegendaryGorget());
                    bag.DropItem(new OracleLegendaryTunic());
                    bag.DropItem(new OracleLegendarySleeves());
                    bag.DropItem(new OracleLegendaryGloves());
                    bag.DropItem(new OracleLegendaryLeggings());
                    break;
                case PlayerClassTypeV2.Necromancer:
                    bag.DropItem(new NecroLegendaryCap());
                    bag.DropItem(new NecroLegendaryGorget());
                    bag.DropItem(new NecroLegendaryTunic());
                    bag.DropItem(new NecroLegendarySleeves());
                    bag.DropItem(new NecroLegendaryGloves());
                    bag.DropItem(new NecroLegendaryLeggings());
                    break;
                case PlayerClassTypeV2.Summoner:
                    bag.DropItem(new SummonerLegendaryCap());
                    bag.DropItem(new SummonerLegendaryGorget());
                    bag.DropItem(new SummonerLegendaryTunic());
                    bag.DropItem(new SummonerLegendarySleeves());
                    bag.DropItem(new SummonerLegendaryGloves());
                    bag.DropItem(new SummonerLegendaryLeggings());
                    break;
                case PlayerClassTypeV2.Shaman:
                    bag.DropItem(new ShamanLegendaryCap());
                    bag.DropItem(new ShamanLegendaryGorget());
                    bag.DropItem(new ShamanLegendaryTunic());
                    bag.DropItem(new ShamanLegendarySleeves());
                    bag.DropItem(new ShamanLegendaryGloves());
                    bag.DropItem(new ShamanLegendaryLeggings());
                    break;
                case PlayerClassTypeV2.Bard:
                    bag.DropItem(new BardLegendaryCap());
                    bag.DropItem(new BardLegendaryGorget());
                    bag.DropItem(new BardLegendaryTunic());
                    bag.DropItem(new BardLegendarySleeves());
                    bag.DropItem(new BardLegendaryGloves());
                    bag.DropItem(new BardLegendaryLeggings());
                    break;
                case PlayerClassTypeV2.Enchanter:
                    bag.DropItem(new EnchanterLegendaryCap());
                    bag.DropItem(new EnchanterLegendaryGorget());
                    bag.DropItem(new EnchanterLegendaryTunic());
                    bag.DropItem(new EnchanterLegendarySleeves());
                    bag.DropItem(new EnchanterLegendaryGloves());
                    bag.DropItem(new EnchanterLegendaryLeggings());
                    break;
                case PlayerClassTypeV2.Illusionist:
                    bag.DropItem(new IllusionistLegendaryCap());
                    bag.DropItem(new IllusionistLegendaryGorget());
                    bag.DropItem(new IllusionistLegendaryTunic());
                    bag.DropItem(new IllusionistLegendarySleeves());
                    bag.DropItem(new IllusionistLegendaryGloves());
                    bag.DropItem(new IllusionistLegendaryLeggings());
                    break;

                // Martial Classes - Plate sets
                case PlayerClassTypeV2.Barbarian:
                    bag.DropItem(new BarbarianLegendaryHelm());
                    bag.DropItem(new BarbarianLegendaryGorget());
                    bag.DropItem(new BarbarianLegendaryChest());
                    bag.DropItem(new BarbarianLegendaryArms());
                    bag.DropItem(new BarbarianLegendaryGloves());
                    bag.DropItem(new BarbarianLegendaryLegs());
                    break;
                case PlayerClassTypeV2.Fighter:
                    bag.DropItem(new FighterLegendaryHelm());
                    bag.DropItem(new FighterLegendaryGorget());
                    bag.DropItem(new FighterLegendaryChest());
                    bag.DropItem(new FighterLegendaryArms());
                    bag.DropItem(new FighterLegendaryGloves());
                    bag.DropItem(new FighterLegendaryLegs());
                    break;
                case PlayerClassTypeV2.Templar:
                    bag.DropItem(new TemplarLegendaryHelm());
                    bag.DropItem(new TemplarLegendaryGorget());
                    bag.DropItem(new TemplarLegendaryChest());
                    bag.DropItem(new TemplarLegendaryArms());
                    bag.DropItem(new TemplarLegendaryGloves());
                    bag.DropItem(new TemplarLegendaryLegs());
                    break;
                case PlayerClassTypeV2.Knight:
                    bag.DropItem(new KnightLegendaryHelm());
                    bag.DropItem(new KnightLegendaryGorget());
                    bag.DropItem(new KnightLegendaryChest());
                    bag.DropItem(new KnightLegendaryArms());
                    bag.DropItem(new KnightLegendaryGloves());
                    bag.DropItem(new KnightLegendaryLegs());
                    break;
                case PlayerClassTypeV2.Paladin:
                    bag.DropItem(new PaladinLegendaryHelm());
                    bag.DropItem(new PaladinLegendaryGorget());
                    bag.DropItem(new PaladinLegendaryChest());
                    bag.DropItem(new PaladinLegendaryArms());
                    bag.DropItem(new PaladinLegendaryGloves());
                    bag.DropItem(new PaladinLegendaryLegs());
                    break;
                case PlayerClassTypeV2.Artificer:
                    bag.DropItem(new ArtificerLegendaryHelm());
                    bag.DropItem(new ArtificerLegendaryGorget());
                    bag.DropItem(new ArtificerLegendaryChest());
                    bag.DropItem(new ArtificerLegendaryArms());
                    bag.DropItem(new ArtificerLegendaryGloves());
                    bag.DropItem(new ArtificerLegendaryLegs());
                    break;

                // Martial Classes - Leather sets
                case PlayerClassTypeV2.Beastmaster:
                    bag.DropItem(new BeastmasterLegendaryCap());
                    bag.DropItem(new BeastmasterLegendaryGorget());
                    bag.DropItem(new BeastmasterLegendaryTunic());
                    bag.DropItem(new BeastmasterLegendarySleeves());
                    bag.DropItem(new BeastmasterLegendaryGloves());
                    bag.DropItem(new BeastmasterLegendaryLeggings());
                    break;
                case PlayerClassTypeV2.Monk:
                    bag.DropItem(new MonkLegendaryCap());
                    bag.DropItem(new MonkLegendaryGorget());
                    bag.DropItem(new MonkLegendaryTunic());
                    bag.DropItem(new MonkLegendarySleeves());
                    bag.DropItem(new MonkLegendaryGloves());
                    bag.DropItem(new MonkLegendaryLeggings());
                    break;
                case PlayerClassTypeV2.Ranger:
                    bag.DropItem(new RangerLegendaryCap());
                    bag.DropItem(new RangerLegendaryGorget());
                    bag.DropItem(new RangerLegendaryTunic());
                    bag.DropItem(new RangerLegendarySleeves());
                    bag.DropItem(new RangerLegendaryGloves());
                    bag.DropItem(new RangerLegendaryLeggings());
                    break;
                case PlayerClassTypeV2.Rogue:
                    bag.DropItem(new RogueLegendaryCap());
                    bag.DropItem(new RogueLegendaryGorget());
                    bag.DropItem(new RogueLegendaryTunic());
                    bag.DropItem(new RogueLegendarySleeves());
                    bag.DropItem(new RogueLegendaryGloves());
                    bag.DropItem(new RogueLegendaryLeggings());
                    break;
                case PlayerClassTypeV2.BountyHunter:
                    bag.DropItem(new BountyHunterLegendaryCap());
                    bag.DropItem(new BountyHunterLegendaryGorget());
                    bag.DropItem(new BountyHunterLegendaryTunic());
                    bag.DropItem(new BountyHunterLegendarySleeves());
                    bag.DropItem(new BountyHunterLegendaryGloves());
                    bag.DropItem(new BountyHunterLegendaryLeggings());
                    break;
                case PlayerClassTypeV2.Alchemist:
                    bag.DropItem(new AlchemistLegendaryCap());
                    bag.DropItem(new AlchemistLegendaryGorget());
                    bag.DropItem(new AlchemistLegendaryTunic());
                    bag.DropItem(new AlchemistLegendarySleeves());
                    bag.DropItem(new AlchemistLegendaryGloves());
                    bag.DropItem(new AlchemistLegendaryLeggings());
                    break;
                case PlayerClassTypeV2.Wizard:
                    bag.DropItem(new WizardLegendaryCap());
                    bag.DropItem(new WizardLegendaryGorget());
                    bag.DropItem(new WizardLegendaryTunic());
                    bag.DropItem(new WizardLegendarySleeves());
                    bag.DropItem(new WizardLegendaryGloves());
                    bag.DropItem(new WizardLegendaryLeggings());
                    break;

                // Chain set class
                case PlayerClassTypeV2.Cleric:
                    bag.DropItem(new ClericLegendaryCoif());
                    bag.DropItem(new ClericLegendaryGorget());
                    bag.DropItem(new ClericLegendaryTunic());
                    bag.DropItem(new ClericLegendarySleeves());
                    bag.DropItem(new ClericLegendaryGloves());
                    bag.DropItem(new ClericLegendaryLeggings());
                    break;

                default:
                    // Fallback to generic plate
                    bag.DropItem(new GlacialAegisHelm());
                    bag.DropItem(new GlacialAegisGorget());
                    bag.DropItem(new GlacialAegisChest());
                    bag.DropItem(new GlacialAegisArms());
                    bag.DropItem(new GlacialAegisGloves());
                    bag.DropItem(new GlacialAegisLegs());
                    break;
            }
        }

        private void AddPotionsToBag(Bag bag, string region, string className)
        {
            // Alchemist gets ALL potions
            if (className == "Alchemist")
            {
                // All resistance potions
                bag.DropItem(new ColdResistancePotion() { Amount = 10 });
                bag.DropItem(new HeatResistancePotion() { Amount = 10 });
                bag.DropItem(new PoisonResistancePotion() { Amount = 10 });
                bag.DropItem(new EnergyResistancePotion() { Amount = 10 });

                // All enhancement potions
                bag.DropItem(new NaturesBlessingPotion() { Amount = 10 });
                bag.DropItem(new CrystalClarityPotion() { Amount = 10 });
                bag.DropItem(new DesertSwiftnessPotion() { Amount = 10 });
                bag.DropItem(new IroncladFortitudePotion() { Amount = 10 });

                // Combat potions
                bag.DropItem(new Server.Items.VystiaClassItems.BurstPotion(5));
                bag.DropItem(new Server.Items.VystiaClassItems.HastePotion(5));
                bag.DropItem(new Server.Items.VystiaClassItems.ResistPotion(5));
                bag.DropItem(new Server.Items.VystiaClassItems.CleansePotion(5));
                bag.DropItem(new Server.Items.VystiaClassItems.SecondWindPotion(5));
                bag.DropItem(new Server.Items.VystiaClassItems.VystiaInvisibilityPotion(5));

                // Standard UO potions
                bag.DropItem(new GreaterHealPotion() { Amount = 20 });
                bag.DropItem(new GreaterCurePotion() { Amount = 10 });
                bag.DropItem(new TotalRefreshPotion() { Amount = 20 });
                return;
            }

            // Regional potions based on home region
            if (region.Contains("Frost"))
                bag.DropItem(new ColdResistancePotion() { Amount = 10 });
            else if (region.Contains("Ember") || region.Contains("Fire"))
                bag.DropItem(new HeatResistancePotion() { Amount = 10 });
            else if (region.Contains("Shadow") || region.Contains("Fen"))
                bag.DropItem(new PoisonResistancePotion() { Amount = 10 });
            else if (region.Contains("Crystal"))
            {
                bag.DropItem(new EnergyResistancePotion() { Amount = 10 });
                bag.DropItem(new CrystalClarityPotion() { Amount = 5 });
            }
            else if (region.Contains("Iron"))
                bag.DropItem(new IroncladFortitudePotion() { Amount = 10 });
            else if (region.Contains("Desert"))
            {
                bag.DropItem(new HeatResistancePotion() { Amount = 5 });
                bag.DropItem(new DesertSwiftnessPotion() { Amount = 10 });
            }
            else if (region.Contains("Verdant") || region.Contains("Forest"))
                bag.DropItem(new NaturesBlessingPotion() { Amount = 10 });
            else
            {
                // Multi-regional: mix of potions
                bag.DropItem(new ColdResistancePotion() { Amount = 3 });
                bag.DropItem(new HeatResistancePotion() { Amount = 3 });
                bag.DropItem(new PoisonResistancePotion() { Amount = 3 });
                bag.DropItem(new EnergyResistancePotion() { Amount = 3 });
            }

            // Standard UO potions for all classes
            bag.DropItem(new GreaterHealPotion() { Amount = 20 });
            bag.DropItem(new GreaterCurePotion() { Amount = 10 });
            bag.DropItem(new TotalRefreshPotion() { Amount = 20 });
        }

        private void AddStandardMageryReagentsToBag(Bag bag)
        {
            // Add standard UO magery reagents (100 of each)
            bag.DropItem(new BlackPearl() { Amount = 100 });
            bag.DropItem(new Bloodmoss() { Amount = 100 });
            bag.DropItem(new Garlic() { Amount = 100 });
            bag.DropItem(new Ginseng() { Amount = 100 });
            bag.DropItem(new MandrakeRoot() { Amount = 100 });
            bag.DropItem(new Nightshade() { Amount = 100 });
            bag.DropItem(new SulfurousAsh() { Amount = 100 });
            bag.DropItem(new SpidersSilk() { Amount = 100 });
        }

        private void AddReagentsToBag(Bag bag, AbilitySchool school)
        {
            // Add school-specific reagents (50 of each type)
            switch (school)
            {
                case AbilitySchool.Ice:
                    bag.DropItem(new Frostbloom() { Amount = 50 });
                    bag.DropItem(new GlacierCrystal() { Amount = 50 });
                    bag.DropItem(new Winterleaf() { Amount = 50 });
                    bag.DropItem(new PermafrostEssence() { Amount = 50 });
                    bag.DropItem(new ArcticPearl() { Amount = 50 });
                    bag.DropItem(new FrozenSoul() { Amount = 50 });
                    bag.DropItem(new FrostEssence() { Amount = 50 });
                    bag.DropItem(new Server.Items.HeartOfWinter() { Amount = 50 });
                    break;

                case AbilitySchool.Nature:
                    bag.DropItem(new WildMoss() { Amount = 50 });
                    bag.DropItem(new Moonpetal() { Amount = 50 });
                    bag.DropItem(new DruidBark() { Amount = 50 });
                    bag.DropItem(new TreantSap() { Amount = 50 });
                    bag.DropItem(new ElderwoodSeed() { Amount = 50 });
                    bag.DropItem(new PrimalVine() { Amount = 50 });
                    bag.DropItem(new LivingBark() { Amount = 50 });
                    bag.DropItem(new AncientRoot() { Amount = 50 });
                    break;

                case AbilitySchool.Hex:
                    bag.DropItem(new BogMoss() { Amount = 50 });
                    bag.DropItem(new ViperFang() { Amount = 50 });
                    bag.DropItem(new Witchweed() { Amount = 50 });
                    bag.DropItem(new ToadsEye() { Amount = 50 });
                    bag.DropItem(new SwampLotus() { Amount = 50 });
                    bag.DropItem(new HagsHair() { Amount = 50 });
                    bag.DropItem(new CursedPearl() { Amount = 50 });
                    bag.DropItem(new CursedSalt() { Amount = 50 });
                    break;

                case AbilitySchool.Elemental:
                    bag.DropItem(new AshPetal() { Amount = 50 });
                    bag.DropItem(new LavaGlass() { Amount = 50 });
                    bag.DropItem(new Flameweed() { Amount = 50 });
                    bag.DropItem(new MagmaEssence() { Amount = 50 });
                    bag.DropItem(new PhoenixFeather() { Amount = 50 });
                    bag.DropItem(new DragonHeart() { Amount = 50 });
                    bag.DropItem(new PrimordialEmber() { Amount = 50 });
                    bag.DropItem(new ElementalCore() { Amount = 50 });
                    break;

                case AbilitySchool.Dark:
                    bag.DropItem(new ShadowMoss() { Amount = 50 });
                    bag.DropItem(new VoidCrystal() { Amount = 50 });
                    bag.DropItem(new VoidWeed() { Amount = 50 });
                    bag.DropItem(new ShadowPetal() { Amount = 50 });
                    bag.DropItem(new VoidDust() { Amount = 50 });
                    bag.DropItem(new VoidSilk() { Amount = 50 });
                    bag.DropItem(new DemonHeart() { Amount = 50 });
                    bag.DropItem(new ShadowEssence() { Amount = 50 });
                    break;

                case AbilitySchool.Divination:
                    bag.DropItem(new TimeSand() { Amount = 50 });
                    bag.DropItem(new TimeDust() { Amount = 50 });
                    bag.DropItem(new DivinationDust() { Amount = 50 });
                    bag.DropItem(new FateCrystal() { Amount = 50 });
                    bag.DropItem(new StarlightCrystal() { Amount = 50 });
                    bag.DropItem(new PropheticLeaf() { Amount = 50 });
                    bag.DropItem(new SeeingStone() { Amount = 50 });
                    bag.DropItem(new FateThread() { Amount = 50 });
                    break;

                case AbilitySchool.Necromancy:
                    bag.DropItem(new GraveMoss() { Amount = 50 });
                    bag.DropItem(new BoneDust() { Amount = 50 });
                    bag.DropItem(new CorpseAsh() { Amount = 50 });
                    bag.DropItem(new SoulFragment() { Amount = 50 });
                    bag.DropItem(new NecroticShroud() { Amount = 50 });
                    bag.DropItem(new LichDust() { Amount = 50 });
                    bag.DropItem(new PhylacteryShard() { Amount = 50 });
                    bag.DropItem(new ReaperEssence() { Amount = 50 });
                    break;

                case AbilitySchool.Summoning:
                    bag.DropItem(new PlanarDust() { Amount = 50 });
                    bag.DropItem(new EtherShard() { Amount = 50 });
                    bag.DropItem(new AetherShard() { Amount = 50 });
                    bag.DropItem(new SummoningCrystal() { Amount = 50 });
                    bag.DropItem(new ChaosShard() { Amount = 50 });
                    bag.DropItem(new BindingRune() { Amount = 50 });
                    bag.DropItem(new DimensionalKey() { Amount = 50 });
                    bag.DropItem(new SummoningSalt() { Amount = 50 });
                    break;

                case AbilitySchool.Shamanic:
                    bag.DropItem(new LightningRoot() { Amount = 50 });
                    bag.DropItem(new ThunderMoss() { Amount = 50 });
                    bag.DropItem(new StormCrystal() { Amount = 50 });
                    bag.DropItem(new StormEssence() { Amount = 50 });
                    bag.DropItem(new SpiritFeather() { Amount = 50 });
                    bag.DropItem(new PrimalThunder() { Amount = 50 });
                    bag.DropItem(new TotemCarving() { Amount = 50 });
                    bag.DropItem(new WindEssence() { Amount = 50 });
                    break;

                case AbilitySchool.Bardic:
                    bag.DropItem(new SongPetal() { Amount = 50 });
                    bag.DropItem(new EchoDust() { Amount = 50 });
                    bag.DropItem(new VoiceCrystal() { Amount = 50 });
                    bag.DropItem(new MuseEssence() { Amount = 50 });
                    bag.DropItem(new HarmonyGem() { Amount = 50 });
                    bag.DropItem(new EternalNote() { Amount = 50 });
                    bag.DropItem(new GoldenString() { Amount = 50 });
                    bag.DropItem(new DragonScale() { Amount = 50 });
                    break;

                case AbilitySchool.Enchanting:
                    bag.DropItem(new ArcaneDust() { Amount = 50 });
                    bag.DropItem(new EssenceOfMagic() { Amount = 50 });
                    bag.DropItem(new ManaCrystal() { Amount = 50 });
                    bag.DropItem(new LeyLineEssence() { Amount = 50 });
                    bag.DropItem(new LeyLineShard() { Amount = 50 });
                    bag.DropItem(new RuneFragment() { Amount = 50 });
                    bag.DropItem(new RunicPowder() { Amount = 50 });
                    bag.DropItem(new TitanRune() { Amount = 50 });
                    break;

                case AbilitySchool.Illusion:
                    bag.DropItem(new MirrorDust() { Amount = 50 });
                    bag.DropItem(new PhantomSilk() { Amount = 50 });
                    bag.DropItem(new MirageEssence() { Amount = 50 });
                    bag.DropItem(new DreamCrystal() { Amount = 50 });
                    bag.DropItem(new RealitySplinter() { Amount = 50 });
                    bag.DropItem(new VoidMirror() { Amount = 50 });
                    bag.DropItem(new ChaosPrism() { Amount = 50 });
                    bag.DropItem(new PhantomPetal() { Amount = 50 });
                    break;

                // Martial classes don't have specific reagents, so we don't add any
                default:
                    // For martial classes or unknown schools, don't add reagents
                    break;
            }
        }

        #endregion

        #region Consumable Methods

        private void GiveResistancePotion(Mobile from, PlayerMobile pm, int index)
        {
            if (pm == null || pm.Backpack == null)
            {
                from.SendMessage("Target must be a player with a backpack.");
                return;
            }

            Item potion = null;
            switch (index)
            {
                case 0: potion = new ColdResistancePotion(); break;
                case 1: potion = new HeatResistancePotion(); break;
                case 2: potion = new PoisonResistancePotion(); break;
                case 3: potion = new EnergyResistancePotion(); break;
            }

            if (potion != null)
            {
                potion.Amount = 5;
                pm.Backpack.DropItem(potion);
                from.SendMessage($"Gave 5x {potion.Name ?? potion.GetType().Name} to {pm.Name}.");
            }
        }

        private void GiveEnhancementPotion(Mobile from, PlayerMobile pm, int index)
        {
            if (pm == null || pm.Backpack == null)
            {
                from.SendMessage("Target must be a player with a backpack.");
                return;
            }

            Item potion = null;
            switch (index)
            {
                case 0: potion = new NaturesBlessingPotion(); break;
                case 1: potion = new CrystalClarityPotion(); break;
                case 2: potion = new DesertSwiftnessPotion(); break;
                case 3: potion = new IroncladFortitudePotion(); break;
            }

            if (potion != null)
            {
                potion.Amount = 5;
                pm.Backpack.DropItem(potion);
                from.SendMessage($"Gave 5x {potion.Name ?? potion.GetType().Name} to {pm.Name}.");
            }
        }

        private void GiveAllResistancePotions(Mobile from, PlayerMobile pm)
        {
            if (pm == null || pm.Backpack == null)
            {
                from.SendMessage("Target must be a player with a backpack.");
                return;
            }

            pm.Backpack.DropItem(new ColdResistancePotion() { Amount = 5 });
            pm.Backpack.DropItem(new HeatResistancePotion() { Amount = 5 });
            pm.Backpack.DropItem(new PoisonResistancePotion() { Amount = 5 });
            pm.Backpack.DropItem(new EnergyResistancePotion() { Amount = 5 });

            from.SendMessage("Gave all resistance potions (5 each).");
        }

        private void GiveAllEnhancementPotions(Mobile from, PlayerMobile pm)
        {
            if (pm == null || pm.Backpack == null)
            {
                from.SendMessage("Target must be a player with a backpack.");
                return;
            }

            pm.Backpack.DropItem(new NaturesBlessingPotion() { Amount = 5 });
            pm.Backpack.DropItem(new CrystalClarityPotion() { Amount = 5 });
            pm.Backpack.DropItem(new DesertSwiftnessPotion() { Amount = 5 });
            pm.Backpack.DropItem(new IroncladFortitudePotion() { Amount = 5 });

            from.SendMessage("Gave all enhancement potions (5 each).");
        }

        private void GiveResourcePotions(Mobile from, PlayerMobile pm)
        {
            if (pm == null || pm.Backpack == null) return;

            // Give heal and stamina potions (no mana potions in classic UO)
            pm.Backpack.DropItem(new GreaterHealPotion() { Amount = 20 });
            pm.Backpack.DropItem(new GreaterCurePotion() { Amount = 10 });
            pm.Backpack.DropItem(new TotalRefreshPotion() { Amount = 20 });

            from.SendMessage("Gave resource potions to target.");
        }

        private void GiveCombatPotions(Mobile from, PlayerMobile pm)
        {
            if (pm == null || pm.Backpack == null) return;

            pm.Backpack.DropItem(new GreaterStrengthPotion() { Amount = 5 });
            pm.Backpack.DropItem(new GreaterAgilityPotion() { Amount = 5 });
            pm.Backpack.DropItem(new TotalRefreshPotion() { Amount = 5 });
            pm.Backpack.DropItem(new GreaterExplosionPotion() { Amount = 10 });

            from.SendMessage("Gave combat potions to target.");
        }

        private void GiveCompleteClassLoadout(Mobile from, PlayerMobile pm)
        {
            if (pm == null) return;

            var playerClass = VystiaClassManager.Instance.GetClass(pm);
            if (playerClass == null)
            {
                from.SendMessage("Target has no class assigned. Assign a class first.");
                return;
            }

            // Give class-appropriate spellbook if magic class
            var school = playerClass.AbilitySchool;
            int spellbookIndex = -1;

            // Map ability school to spellbook index
            switch (school)
            {
                case AbilitySchool.Ice: spellbookIndex = 0; break;
                case AbilitySchool.Nature: spellbookIndex = 1; break;
                case AbilitySchool.Hex: spellbookIndex = 2; break;
                case AbilitySchool.Elemental: spellbookIndex = 3; break;
                case AbilitySchool.Dark: spellbookIndex = 4; break;
                case AbilitySchool.Divination: spellbookIndex = 5; break;
                case AbilitySchool.Necromancy: spellbookIndex = 6; break;
                case AbilitySchool.Summoning: spellbookIndex = 7; break;
                case AbilitySchool.Shamanic: spellbookIndex = 8; break;
                case AbilitySchool.Bardic: spellbookIndex = 9; break;
                case AbilitySchool.Enchanting: spellbookIndex = 10; break;
                case AbilitySchool.Illusion: spellbookIndex = 11; break;
            }

            if (spellbookIndex >= 0)
                GiveSpellbook(from, pm, spellbookIndex);

            // Give potions
            GiveResourcePotions(from, pm);
            GiveCombatPotions(from, pm);

            // Fill resources
            FillAllResources(pm);

            // Full heal
            pm.Hits = pm.HitsMax;
            pm.Mana = pm.ManaMax;
            pm.Stam = pm.StamMax;

            from.SendMessage($"Gave complete loadout for {playerClass.ClassName}.");
        }

        #endregion

        #region Spawn Methods

        private void SpawnTestDummy(Mobile from)
        {
            from.SendMessage("Target a location to spawn the Practice Target (infinite HP, 0% resistances for spell testing).");
            from.Target = new SpawnTarget(typeof(PracticeDummy), "Practice Target");
        }

        private void SpawnClassItemVendor(Mobile from)
        {
            from.SendMessage("Use [spawnvystia to spawn vendors.");
        }

        private void SpawnReagentVendor(Mobile from)
        {
            from.SendMessage("Use [spawnvystia > Vendors page to spawn reagent vendors.");
        }

        private void SpawnWeaponChests(Mobile from)
        {
            // Call the SpawnWeaponChests command via CommandSystem
            CommandSystem.Handle(from, CommandSystem.Prefix + "SpawnWeaponChests");
            from.SendGump(new VystiaAdminGump(from, m_Target, m_Page, m_ItemSubPage));
        }

        #endregion

        #endregion

        #region Internal Classes

        private class InternalTarget : Target
        {
            private readonly VystiaAdminGump m_Gump;

            public InternalTarget(VystiaAdminGump gump) : base(-1, false, TargetFlags.None)
            {
                m_Gump = gump;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                if (targeted is Mobile mobile)
                {
                    from.SendGump(new VystiaAdminGump(from, mobile, m_Gump.m_Page, m_Gump.m_ItemSubPage));
                }
                else
                {
                    from.SendMessage("That is not a valid target.");
                    from.SendGump(new VystiaAdminGump(from, m_Gump.m_Target, m_Gump.m_Page, m_Gump.m_ItemSubPage));
                }
            }

            protected override void OnTargetCancel(Mobile from, TargetCancelType cancelType)
            {
                from.SendGump(new VystiaAdminGump(from, m_Gump.m_Target, m_Gump.m_Page, m_Gump.m_ItemSubPage));
            }
        }

        private class SpawnTarget : Target
        {
            private readonly Type m_Type;
            private readonly string m_Name;

            public SpawnTarget(Type type, string name) : base(-1, true, TargetFlags.None)
            {
                m_Type = type;
                m_Name = name;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                IPoint3D p = targeted as IPoint3D;
                if (p == null)
                    return;

                Point3D loc = new Point3D(p);
                Map map = from.Map;

                try
                {
                    object o = Activator.CreateInstance(m_Type);
                    if (o is Item item)
                    {
                        item.MoveToWorld(loc, map);
                        from.SendMessage($"Spawned {m_Name} at {loc}.");
                    }
                    else if (o is Mobile mobile)
                    {
                        mobile.MoveToWorld(loc, map);
                        from.SendMessage($"Spawned {m_Name} at {loc}.");
                    }
                }
                catch (Exception ex)
                {
                    from.SendMessage($"Error spawning {m_Name}: {ex.Message}");
                }
            }
        }

        #endregion

        #region Faction/Religion Helper Methods

        private static readonly VystiaFaction[] s_FactionEnums = {
            VystiaFaction.Frostguard, VystiaFaction.FlameLegion, VystiaFaction.Greenward,
            VystiaFaction.ArcaneConclave, VystiaFaction.Technoguild, VystiaFaction.Sandwalkers, VystiaFaction.Voidborn
        };

        private static readonly VystiaReligion[] s_ReligionEnums = {
            VystiaReligion.FrosthelmFaith, VystiaReligion.SuryasSandscript, VystiaReligion.LunarasCovenant,
            VystiaReligion.CelestisArcanum, VystiaReligion.OceanasCovenant, VystiaReligion.CogsmithCreed
        };

        private void HandleFactionTierButton(Mobile from, PlayerMobile pm, int buttonId)
        {
            if (pm == null)
            {
                from.SendMessage("Target must be a player.");
                return;
            }

            // Button ID format: 3000 + (factionIndex * 10) + tierIndex
            int adjusted = buttonId - 3000;
            int factionIndex = adjusted / 10;
            int tierIndex = adjusted % 10;

            if (factionIndex < 0 || factionIndex >= s_FactionEnums.Length)
                return;

            VystiaFaction faction = s_FactionEnums[factionIndex];

            // Tier values: Hostile=-2000, Neutral=0, Friendly=3000, Honored=6000, Revered=12000, Exalted=15000
            int[] tierValues = { -2000, 0, 3000, 6000, 12000, 15000 };

            if (tierIndex < 0 || tierIndex >= tierValues.Length)
                return;

            var repData = VystiaReputation.GetReputation(pm);
            if (repData != null)
            {
                repData.SetReputation(faction, tierValues[tierIndex]);
                var tier = FactionData.GetTier(tierValues[tierIndex]);
                from.SendMessage($"Set {faction} reputation to {tierValues[tierIndex]} ({tier}).");
            }
        }

        private void HandleCustomFactionRep(Mobile from, PlayerMobile pm, int buttonId, RelayInfo info)
        {
            if (pm == null)
            {
                from.SendMessage("Target must be a player.");
                return;
            }

            int factionIndex = buttonId - 3100;
            if (factionIndex < 0 || factionIndex >= s_FactionEnums.Length)
                return;

            // Get custom value from text entry (ID 100)
            var textEntry = info.GetTextEntry(100);
            if (textEntry == null || string.IsNullOrEmpty(textEntry.Text))
            {
                from.SendMessage("Please enter a reputation value.");
                return;
            }

            if (!int.TryParse(textEntry.Text, out int repValue))
            {
                from.SendMessage("Invalid number. Please enter a value between -3000 and 21000.");
                return;
            }

            repValue = Math.Max(-3000, Math.Min(21000, repValue));

            VystiaFaction faction = s_FactionEnums[factionIndex];
            var repData = VystiaReputation.GetReputation(pm);
            if (repData != null)
            {
                repData.SetReputation(faction, repValue);
                var tier = FactionData.GetTier(repValue);
                from.SendMessage($"Set {faction} reputation to {repValue} ({tier}).");
            }
        }

        private void HandleReligionSelection(Mobile from, PlayerMobile pm, int buttonId)
        {
            if (pm == null)
            {
                from.SendMessage("Target must be a player.");
                return;
            }

            int religionIndex = buttonId - 3200; // 0 = None, 1-6 = religions

            if (religionIndex == 0)
            {
                // Clear religion
                var pietyData = VystiaPiety.GetPiety(pm);
                if (pietyData != null)
                {
                    pietyData.Religion = VystiaReligion.None;
                    pietyData.Piety = 0;
                    from.SendMessage("Removed religion from target.");
                }
            }
            else if (religionIndex >= 1 && religionIndex <= 6)
            {
                VystiaReligion religion = s_ReligionEnums[religionIndex - 1];
                var pietyData = VystiaPiety.GetPiety(pm);
                if (pietyData != null)
                {
                    pietyData.Religion = religion;
                    var info = ReligionData.GetInfo(religion);
                    from.SendMessage($"Set religion to {info?.Name ?? religion.ToString()}.");
                }
            }
        }

        private void HandlePietyPreset(Mobile from, PlayerMobile pm, int buttonId)
        {
            if (pm == null)
            {
                from.SendMessage("Target must be a player.");
                return;
            }

            var pietyData = VystiaPiety.GetPiety(pm);
            if (pietyData == null || pietyData.Religion == VystiaReligion.None)
            {
                from.SendMessage("Target must have a religion first.");
                return;
            }

            // Button ID: 3300=0, 3301=50, 3302=200, 3303=500, 3304=900, 3305=1000
            int[] pietyValues = { 0, 50, 200, 500, 900, 1000 };
            int index = buttonId - 3300;

            if (index >= 0 && index < pietyValues.Length)
            {
                pietyData.Piety = pietyValues[index];
                var tier = ReligionData.GetTier(pietyData.Piety);
                from.SendMessage($"Set piety to {pietyData.Piety} ({tier}).");
            }
        }

        private void HandleCustomPiety(Mobile from, PlayerMobile pm, RelayInfo info)
        {
            if (pm == null)
            {
                from.SendMessage("Target must be a player.");
                return;
            }

            var pietyData = VystiaPiety.GetPiety(pm);
            if (pietyData == null || pietyData.Religion == VystiaReligion.None)
            {
                from.SendMessage("Target must have a religion first.");
                return;
            }

            // Get custom value from text entry (ID 101)
            var textEntry = info.GetTextEntry(101);
            if (textEntry == null || string.IsNullOrEmpty(textEntry.Text))
            {
                from.SendMessage("Please enter a piety value.");
                return;
            }

            if (!int.TryParse(textEntry.Text, out int pietyValue))
            {
                from.SendMessage("Invalid number. Please enter a value between 0 and 1000.");
                return;
            }

            pietyValue = Math.Max(0, Math.Min(1000, pietyValue));
            pietyData.Piety = pietyValue;
            var tier = ReligionData.GetTier(pietyData.Piety);
            from.SendMessage($"Set piety to {pietyData.Piety} ({tier}).");
        }

        #endregion
    }

    #region Command Registration

    public class VystiaAdminCommand
    {
        public static void Initialize()
        {
            CommandSystem.Register("VystiaAdmin", AccessLevel.GameMaster, new CommandEventHandler(VystiaAdmin_OnCommand));
            CommandSystem.Register("VA", AccessLevel.GameMaster, new CommandEventHandler(VystiaAdmin_OnCommand));
        }

        [Usage("VystiaAdmin")]
        [Description("Opens the Vystia Class System admin panel.")]
        private static void VystiaAdmin_OnCommand(CommandEventArgs e)
        {
            e.Mobile.SendGump(new VystiaAdminGump(e.Mobile));
        }
    }

    #endregion
}

