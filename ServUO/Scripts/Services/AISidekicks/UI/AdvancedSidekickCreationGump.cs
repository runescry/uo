using System;
using System.Collections.Generic;
using Server;
using Server.Gumps;
using Server.Mobiles;
using Server.Network;
using Server.Items;
using Server.Services.AISidekicks;
using Server.Custom.VystiaClasses.Quests;

namespace Server.Services.AISidekicks.UI
{
    public class AdvancedSidekickCreationGump : Gump
    {
        private PlayerMobile m_Player;

        // State
        private string m_SidekickName;
        private SidekickArchetypeType m_SelectedArchetype;
        private bool m_Female;
        private Race m_Race;
        private int m_HairIndex;
        private int m_FacialHairIndex;
        private int m_SkinHue;
        private int m_HairHue;
        private int m_FacialHairHue;

        // Hair data - Human
        private static readonly string[] m_HumanMaleHairNames = { "Bald", "Short", "Long", "Ponytail", "Mohawk", "Pageboy", "Afro", "Receeding", "PigTails", "Krisna" };
        private static readonly int[] m_HumanMaleHairItems = { 0, 0x203B, 0x203C, 0x203D, 0x2044, 0x2045, 0x2047, 0x2048, 0x2049, 0x204A };

        private static readonly string[] m_HumanFemaleHairNames = { "Bald", "Short", "Long", "Ponytail", "Mohawk", "Pageboy", "Buns", "Afro", "PigTails", "Krisna" };
        private static readonly int[] m_HumanFemaleHairItems = { 0, 0x203B, 0x203C, 0x203D, 0x2044, 0x2045, 0x2046, 0x2047, 0x2049, 0x204A };

        // Beards - Human male only
        private static readonly string[] m_HumanBeardNames = { "None", "Long Beard", "Goatee", "Short Beard", "Mustache", "Vandyke", "Full Beard", "Medium" };
        private static readonly int[] m_HumanBeardItems = { 0, 0x2040, 0x203E, 0x203F, 0x2041, 0x204B, 0x204C, 0x204D };

        // Hair data - Elf
        private static readonly string[] m_ElfMaleHairNames = { "Bald", "Long Feather", "Short", "Mullet", "Knob", "Braided", "Spiked", "Mid Long" };
        private static readonly int[] m_ElfMaleHairItems = { 0, 0x2FC0, 0x2FC1, 0x2FC2, 0x2FCE, 0x2FCF, 0x2FD1, 0x2FBF };

        private static readonly string[] m_ElfFemaleHairNames = { "Bald", "Long Feather", "Short", "Mullet", "Knob", "Braided", "Spiked", "Flower", "Bun" };
        private static readonly int[] m_ElfFemaleHairItems = { 0, 0x2FC0, 0x2FC1, 0x2FC2, 0x2FCE, 0x2FCF, 0x2FD1, 0x2FCC, 0x2FD0 };

        // Skin tones
        private static readonly int[] m_SkinTones = { 1002, 1003, 1004, 1005, 1006, 1007, 1008, 1009, 1010, 1011, 1012, 1013, 1014, 1015,
                                                       1016, 1017, 1018, 1019, 1020, 1021, 1022, 1023, 1024, 1025, 1026, 1027, 1028, 1029,
                                                       1030, 1031, 1032, 1033, 1034, 1035, 1036, 1037, 1038, 1039, 1040, 1041, 1042, 1043,
                                                       1044, 1045, 1046, 1047, 1048, 1049, 1050, 1051, 1052, 1053, 1054, 1055, 1056, 1057, 1058 };
        private int m_SkinIndex = 0;

        public AdvancedSidekickCreationGump(PlayerMobile player) : this(player, SidekickArchetypeType.Warrior,
            SidekickNameLibrary.GetRandomName(SidekickArchetypeType.Warrior, true),
            false, Race.Human, 1, 0, 0, 1102, 1102)
        {
        }

        public AdvancedSidekickCreationGump(PlayerMobile player, SidekickArchetypeType archetype, string name, bool female, Race race,
            int hairIndex, int facialHairIndex, int skinIndex, int hairHue, int facialHairHue)
            : base(50, 50)
        {
            m_Player = player;
            m_SelectedArchetype = archetype;
            m_SidekickName = name;
            m_Female = female;
            m_Race = race;
            m_HairIndex = hairIndex;
            m_FacialHairIndex = facialHairIndex;
            m_SkinIndex = skinIndex;
            m_SkinHue = m_SkinTones[Math.Min(skinIndex, m_SkinTones.Length - 1)];
            m_HairHue = hairHue;
            m_FacialHairHue = facialHairHue;

            Closable = true;
            Disposable = true;
            Dragable = true;
            Resizable = false;

            BuildGump();
        }

        private void BuildGump()
        {
            AddPage(0);

            int width = 750;
            int height = 620;

            // Main background
            AddBackground(0, 0, width, height, 9270);

            // Title
            AddHtml(0, 25, width, 25, "<center><basefont color=#FFFFFF><b>Create Your Companion</b></basefont></center>", false, false);

            int y = 65;

            // === NAME SECTION ===
            AddHtml(40, y, 80, 20, "<basefont color=#FFFFFF>Name:</basefont>", false, false);
            AddBackground(120, y - 2, 300, 22, 9350);
            AddTextEntry(125, y, 290, 18, 0, 1, m_SidekickName, 30);
            AddButton(440, y - 2, 4005, 4007, 400, GumpButtonType.Reply, 0);
            AddHtml(475, y, 80, 20, "<basefont color=#AAAAFF>Random</basefont>", false, false);
            y += 50;

            // === CLASS SELECTION ===
            AddHtml(40, y, 150, 20, "<basefont color=#FFFFFF><b>Select Class:</b></basefont>", false, false);
            y += 35;

            int classIndex = 0;
            int colWidth = 180;
            int startX = 60;
            int startY = y;
            int rowHeight = 42;
            int classesPerCol = 4;

            foreach (SidekickArchetypeType type in Enum.GetValues(typeof(SidekickArchetypeType)))
            {
                bool isSelected = (type == m_SelectedArchetype);
                int col = classIndex / classesPerCol;
                int row = classIndex % classesPerCol;
                int xPos = startX + (col * colWidth);
                int yPos = startY + (row * rowHeight);

                AddButton(xPos, yPos, isSelected ? 9723 : 9720, 9723, 100 + classIndex, GumpButtonType.Reply, 0);
                string color = isSelected ? "#FFD700" : "#FFFFFF";
                AddHtml(xPos + 28, yPos + 2, 130, 20, $"<basefont color={color}>{type}</basefont>", false, false);

                classIndex++;
            }
            y = startY + (classesPerCol * rowHeight) + 20;

            // === CLASS INFO ===
            SidekickArchetype archetype = SidekickArchetype.GetArchetype(m_SelectedArchetype);
            if (archetype != null)
            {
                AddHtml(40, y, 620, 20, $"<basefont color=#00FF00>Stats:  STR {archetype.StartingStr}  |  DEX {archetype.StartingDex}  |  INT {archetype.StartingInt}</basefont>", false, false);
                y += 22;

                string skills = "Skills: ";
                for (int i = 0; i < Math.Min(5, archetype.StartingSkills.Count); i++)
                {
                    if (i > 0) skills += ",  ";
                    skills += archetype.StartingSkills[i].SkillName.ToString();
                }
                AddHtml(40, y, 620, 20, $"<basefont color=#AAAAAA>{skills}</basefont>", false, false);
            }
            y += 45;

            // === APPEARANCE SECTION ===
            AddHtml(40, y, 200, 20, "<basefont color=#FFFFFF><b>Appearance:</b></basefont>", false, false);
            y += 42;

            int leftCol = 60;
            int rightCol = 410;
            int labelWidth = 100;
            int controlX = 170;
            int controlXRight = 520;

            // Left column
            // Gender
            AddHtml(leftCol, y, labelWidth, 20, "<basefont color=#FFFFFF>Gender:</basefont>", false, false);
            AddButton(controlX, y, !m_Female ? 9723 : 9720, 9723, 502, GumpButtonType.Reply, 0);
            AddHtml(controlX + 28, y, 60, 20, $"<basefont color={(!m_Female ? "#FFD700" : "#FFFFFF")}>Male</basefont>", false, false);
            AddButton(controlX + 90, y, m_Female ? 9723 : 9720, 9720, 503, GumpButtonType.Reply, 0);
            AddHtml(controlX + 118, y, 60, 20, $"<basefont color={(m_Female ? "#FFD700" : "#FFFFFF")}>Female</basefont>", false, false);

            // Right column - Race
            AddHtml(rightCol, y, labelWidth, 20, "<basefont color=#FFFFFF>Race:</basefont>", false, false);
            AddButton(controlXRight, y, m_Race == Race.Human ? 9723 : 9720, 9723, 500, GumpButtonType.Reply, 0);
            AddHtml(controlXRight + 28, y, 60, 20, $"<basefont color={(m_Race == Race.Human ? "#FFD700" : "#FFFFFF")}>Human</basefont>", false, false);
            AddButton(controlXRight + 100, y, m_Race == Race.Elf ? 9723 : 9720, 9720, 501, GumpButtonType.Reply, 0);
            AddHtml(controlXRight + 128, y, 50, 20, $"<basefont color={(m_Race == Race.Elf ? "#FFD700" : "#FFFFFF")}>Elf</basefont>", false, false);
            y += 46;

            // Skin Tone
            AddHtml(leftCol, y, labelWidth, 20, "<basefont color=#FFFFFF>Skin Tone:</basefont>", false, false);
            AddButton(controlX, y, 5603, 5607, 306, GumpButtonType.Reply, 0);
            AddHtml(controlX + 35, y, 120, 20, $"<basefont color=#FFFFFF>Tone {m_SkinIndex + 1} / {m_SkinTones.Length}</basefont>", false, false);
            AddButton(controlX + 155, y, 5601, 5605, 307, GumpButtonType.Reply, 0);

            // Hair Style
            AddHtml(rightCol, y, labelWidth, 20, "<basefont color=#FFFFFF>Hair Style:</basefont>", false, false);
            AddButton(controlXRight, y, 5603, 5607, 300, GumpButtonType.Reply, 0);
            AddHtml(controlXRight + 35, y, 120, 20, $"<basefont color=#FFFFFF>{GetHairName()}</basefont>", false, false);
            AddButton(controlXRight + 155, y, 5601, 5605, 301, GumpButtonType.Reply, 0);
            y += 46;

            // Hair Color
            AddHtml(leftCol, y, labelWidth, 20, "<basefont color=#FFFFFF>Hair Color:</basefont>", false, false);
            AddButton(controlX, y, 5603, 5607, 302, GumpButtonType.Reply, 0);
            AddHtml(controlX + 35, y, 120, 20, $"<basefont color=#FFFFFF>Hue {m_HairHue}</basefont>", false, false);
            AddButton(controlX + 155, y, 5601, 5605, 303, GumpButtonType.Reply, 0);

            // Facial Hair (male human only) - or empty space
            if (!m_Female && m_Race == Race.Human)
            {
                AddHtml(rightCol, y, labelWidth, 20, "<basefont color=#FFFFFF>Beard Style:</basefont>", false, false);
                AddButton(controlXRight, y, 5603, 5607, 304, GumpButtonType.Reply, 0);
                AddHtml(controlXRight + 35, y, 120, 20, $"<basefont color=#FFFFFF>{GetBeardName()}</basefont>", false, false);
                AddButton(controlXRight + 155, y, 5601, 5605, 305, GumpButtonType.Reply, 0);
            }
            y += 46;

            // Beard Color (male human only)
            if (!m_Female && m_Race == Race.Human)
            {
                AddHtml(rightCol, y, labelWidth, 20, "<basefont color=#FFFFFF>Beard Color:</basefont>", false, false);
                AddButton(controlXRight, y, 5603, 5607, 308, GumpButtonType.Reply, 0);
                AddHtml(controlXRight + 35, y, 120, 20, $"<basefont color=#FFFFFF>Hue {m_FacialHairHue}</basefont>", false, false);
                AddButton(controlXRight + 155, y, 5601, 5605, 309, GumpButtonType.Reply, 0);
            }

            // === BUTTONS ===
            int buttonY = height - 55;
            int centerX = width / 2;

            AddButton(centerX - 120, buttonY, 4005, 4007, 1, GumpButtonType.Reply, 0);
            AddHtml(centerX - 85, buttonY + 2, 100, 20, "<basefont color=#00FF00><b>CREATE</b></basefont>", false, false);

            AddButton(centerX + 30, buttonY, 4017, 4019, 0, GumpButtonType.Reply, 0);
            AddHtml(centerX + 65, buttonY + 2, 100, 20, "<basefont color=#FF6666>Cancel</basefont>", false, false);
        }

        private int GetHairItemID()
        {
            int[] items;
            if (m_Race == Race.Elf)
                items = m_Female ? m_ElfFemaleHairItems : m_ElfMaleHairItems;
            else
                items = m_Female ? m_HumanFemaleHairItems : m_HumanMaleHairItems;

            int index = m_HairIndex % items.Length;
            return items[index];
        }

        private string GetHairName()
        {
            string[] names;
            if (m_Race == Race.Elf)
                names = m_Female ? m_ElfFemaleHairNames : m_ElfMaleHairNames;
            else
                names = m_Female ? m_HumanFemaleHairNames : m_HumanMaleHairNames;

            int index = m_HairIndex % names.Length;
            return names[index];
        }

        private int GetHairCount()
        {
            if (m_Race == Race.Elf)
                return m_Female ? m_ElfFemaleHairItems.Length : m_ElfMaleHairItems.Length;
            return m_Female ? m_HumanFemaleHairItems.Length : m_HumanMaleHairItems.Length;
        }

        private int GetBeardItemID()
        {
            if (m_Female || m_Race != Race.Human)
                return 0;
            int index = m_FacialHairIndex % m_HumanBeardItems.Length;
            return m_HumanBeardItems[index];
        }

        private string GetBeardName()
        {
            if (m_Female || m_Race != Race.Human)
                return "None";
            int index = m_FacialHairIndex % m_HumanBeardNames.Length;
            return m_HumanBeardNames[index];
        }

        private int GetBeardCount()
        {
            if (m_Female || m_Race != Race.Human)
                return 0;
            return m_HumanBeardItems.Length;
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            Mobile from = sender.Mobile;
            if (from == null) return;

            if (info.ButtonID == 0) // Cancel
            {
                from.SendMessage("Companion creation cancelled.");
                return;
            }

            // Get name from text entry
            TextRelay nameEntry = info.GetTextEntry(1);
            string currentName = nameEntry != null ? nameEntry.Text.Trim() : m_SidekickName;
            m_SidekickName = currentName;

            // Create button
            if (info.ButtonID == 1)
            {
                CreateSidekick(from);
                return;
            }

            // Handle archetype selection (100-199)
            if (info.ButtonID >= 100 && info.ButtonID < 200)
            {
                var newArchetype = (SidekickArchetypeType)(info.ButtonID - 100);
                if (newArchetype != m_SelectedArchetype)
                {
                    m_SelectedArchetype = newArchetype;
                    m_SidekickName = SidekickNameLibrary.GetRandomName(m_SelectedArchetype, !m_Female);
                }
            }

            int id = info.ButtonID;

            // Hair Style
            if (id == 300) m_HairIndex = AdjustIndex(m_HairIndex, -1, GetHairCount());
            else if (id == 301) m_HairIndex = AdjustIndex(m_HairIndex, 1, GetHairCount());
            // Hair Color
            else if (id == 302) m_HairHue = AdjustHue(m_HairHue, -1);
            else if (id == 303) m_HairHue = AdjustHue(m_HairHue, 1);
            // Facial Hair Style
            else if (id == 304) m_FacialHairIndex = AdjustIndex(m_FacialHairIndex, -1, GetBeardCount());
            else if (id == 305) m_FacialHairIndex = AdjustIndex(m_FacialHairIndex, 1, GetBeardCount());
            // Skin Tone
            else if (id == 306) m_SkinIndex = AdjustIndex(m_SkinIndex, -1, m_SkinTones.Length);
            else if (id == 307) m_SkinIndex = AdjustIndex(m_SkinIndex, 1, m_SkinTones.Length);
            // Beard Color
            else if (id == 308) m_FacialHairHue = AdjustHue(m_FacialHairHue, -1);
            else if (id == 309) m_FacialHairHue = AdjustHue(m_FacialHairHue, 1);
            // Randomize Name
            else if (id == 400)
            {
                m_SidekickName = SidekickNameLibrary.GetRandomName(m_SelectedArchetype, !m_Female);
            }
            // Race: Human
            else if (id == 500)
            {
                if (m_Race != Race.Human)
                {
                    m_Race = Race.Human;
                    m_HairIndex = Math.Min(m_HairIndex, GetHairCount() - 1);
                }
            }
            // Race: Elf
            else if (id == 501)
            {
                if (m_Race != Race.Elf)
                {
                    m_Race = Race.Elf;
                    m_HairIndex = Math.Min(m_HairIndex, GetHairCount() - 1);
                    m_FacialHairIndex = 0;
                }
            }
            // Gender: Male
            else if (id == 502)
            {
                if (m_Female)
                {
                    m_Female = false;
                    m_SidekickName = SidekickNameLibrary.GetRandomName(m_SelectedArchetype, true);
                    m_HairIndex = Math.Min(m_HairIndex, GetHairCount() - 1);
                }
            }
            // Gender: Female
            else if (id == 503)
            {
                if (!m_Female)
                {
                    m_Female = true;
                    m_SidekickName = SidekickNameLibrary.GetRandomName(m_SelectedArchetype, false);
                    m_HairIndex = Math.Min(m_HairIndex, GetHairCount() - 1);
                    m_FacialHairIndex = 0;
                }
            }

            // Refresh gump
            from.SendGump(new AdvancedSidekickCreationGump((PlayerMobile)from, m_SelectedArchetype, m_SidekickName, m_Female, m_Race,
                m_HairIndex, m_FacialHairIndex, m_SkinIndex, m_HairHue, m_FacialHairHue));
        }

        private int AdjustIndex(int current, int change, int max)
        {
            if (max <= 0) return 0;
            int next = current + change;
            if (next < 0) return max - 1;
            if (next >= max) return 0;
            return next;
        }

        private int AdjustHue(int current, int change)
        {
            int next = current + (change * 5);
            if (next < 2) next = 1000;
            if (next > 1000) next = 2;
            return next;
        }

        private void CreateSidekick(Mobile from)
        {
            if (string.IsNullOrWhiteSpace(m_SidekickName))
            {
                from.SendMessage("Please enter a name.");
                from.SendGump(new AdvancedSidekickCreationGump((PlayerMobile)from, m_SelectedArchetype, m_SidekickName, m_Female, m_Race,
                    m_HairIndex, m_FacialHairIndex, m_SkinIndex, m_HairHue, m_FacialHairHue));
                return;
            }

            try
            {
                int hairID = GetHairItemID();
                int beardID = GetBeardItemID();

                Console.WriteLine($"[SidekickCreation] Creating {m_SelectedArchetype} sidekick named {m_SidekickName}");

                AutonomousSidekick sidekick = new AutonomousSidekick(m_SelectedArchetype, (PlayerMobile)from,
                    !m_Female, m_Race, m_SkinHue, hairID, m_HairHue, beardID, m_FacialHairHue);

                sidekick.Name = m_SidekickName;
                sidekick.MoveToWorld(from.Location, from.Map);

                from.SendMessage($"Your {m_SelectedArchetype} companion {m_SidekickName} has been created!");

                // Quest integration: complete RecruitSidekick waypoint if this matches the player's current step.
                if (from is PlayerMobile pm)
                {
                    VystiaQuestSystem.OnSidekickRecruited(pm, m_SelectedArchetype.ToString());
                }
            }
            catch (Exception ex)
            {
                from.SendMessage($"Error creating companion: {ex.Message}");
                Console.WriteLine($"[SidekickCreation] Error: {ex}");
            }
        }
    }
}
