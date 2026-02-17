using System;
using System.Collections.Generic;
using System.Linq;
using Server;
using Server.Gumps;
using Server.Network;
using Server.Mobiles;
using Server.Custom.VystiaClasses.Quests;
using Server.Services.LLM;

namespace Server.Gumps
{
    /// <summary>
    /// Multi-step wizard for spawning QuestNPCs linked to quest waypoints
    /// Step 0: Select Quest
    /// Step 1: Select Waypoint
    /// Step 2: Configure NPC
    /// Step 3: Confirm & Spawn
    /// </summary>
    public class AddQuestNPCGump : Gump
    {
        private Mobile m_From;
        private int m_Step;
        private int m_SelectedQuestID;
        private int m_SelectedWaypointID;
        private string m_NPCName;
        private string m_NPCTitle;
        private NPCPersonalities.PersonalityType m_PersonalityType;
        private NPCPersonalities.SpeechPattern m_SpeechPattern;

        private bool m_ReturnToQuestWizard;

        // Colors
        private const int TitleColor = 0xFFFFFF;
        private const int LabelColor = 0xAAAAAA;
        private const int ValueColor = 0x88FF88;
        private const int ErrorColor = 0xFF4444;
        private const int SelectedColor = 0x44FF44;

        public AddQuestNPCGump(
            Mobile from,
            int step,
            int selectedQuestID,
            int selectedWaypointID,
            string npcName,
            string npcTitle,
            NPCPersonalities.PersonalityType personality,
            NPCPersonalities.SpeechPattern speech)
            : base(50, 50)
        {
            m_From = from;
            m_Step = step;
            m_SelectedQuestID = selectedQuestID;
            m_SelectedWaypointID = selectedWaypointID;
            m_NPCName = npcName ?? "";
            m_NPCTitle = npcTitle ?? "";
            m_PersonalityType = personality;
            m_SpeechPattern = speech;
            m_ReturnToQuestWizard = false;

            Closable = true;
            Disposable = true;
            Dragable = true;
            Resizable = false;

            BuildGump();
        }

        /// <summary>
        /// Deep-link constructor used by the Quest Wizard: skips selection steps and returns back to the wizard after spawn.
        /// </summary>
        public AddQuestNPCGump(Mobile from, int questId, int waypointId)
            : this(from, 2, questId, waypointId, "", "", NPCPersonalities.PersonalityType.Sage, NPCPersonalities.SpeechPattern.Formal)
        {
            m_ReturnToQuestWizard = true;
        }

        private void BuildGump()
        {
            AddPage(0);

            // Background
            AddBackground(0, 0, 500, 450, 9270);

            // Title
            AddHtml(20, 20, 460, 25, Center(Color($"Add Quest NPC - Step {m_Step + 1} of 4", TitleColor)), false, false);

            // Progress bar
            DrawProgressBar();

            int y = 70;

            switch (m_Step)
            {
                case 0:
                    BuildStep0_SelectQuest(ref y);
                    break;
                case 1:
                    BuildStep1_SelectWaypoint(ref y);
                    break;
                case 2:
                    BuildStep2_ConfigureNPC(ref y);
                    break;
                case 3:
                    BuildStep3_Confirm(ref y);
                    break;
            }
        }

        private void DrawProgressBar()
        {
            int barY = 50;
            int barWidth = 460;
            int stepWidth = barWidth / 4;

            for (int i = 0; i < 4; i++)
            {
                int x = 20 + (i * stepWidth);
                int color = i <= m_Step ? 0x44FF44 : 0x444444;
                AddHtml(x, barY, stepWidth - 5, 15, Color(GetStepLabel(i), color), false, false);
            }
        }

        private string GetStepLabel(int step)
        {
            switch (step)
            {
                case 0: return "Quest";
                case 1: return "Waypoint";
                case 2: return "Configure";
                case 3: return "Spawn";
                default: return "";
            }
        }

        #region Step 0: Select Quest

        private void BuildStep0_SelectQuest(ref int y)
        {
            AddHtml(20, y, 460, 20, Color("Select a quest with waypoints:", LabelColor), false, false);
            y += 25;

            var quests = DynamicQuestManager.GetAllDynamicQuests();

            if (quests.Count == 0)
            {
                AddHtml(20, y, 460, 40, Color("No dynamic quests found. Create quests using [QuestEditor first.", ErrorColor), false, false);
                return;
            }

            // Filter to quests that have waypoints
            var questsWithWaypoints = quests.Where(q => q.Waypoints.Count > 0).ToList();

            if (questsWithWaypoints.Count == 0)
            {
                AddHtml(20, y, 460, 40, Color("No quests have waypoints. Add waypoints using [QuestEditor first.", ErrorColor), false, false);

                // Show option to view all quests anyway
                y += 50;
                AddHtml(20, y, 460, 20, Color("All available quests (no waypoints):", LabelColor), false, false);
                y += 25;

                foreach (var quest in quests.Take(10))
                {
                    AddHtml(40, y, 380, 20, $"{quest.Title} (ID: {quest.QuestID}) - No waypoints", false, false);
                    y += 22;
                }
                return;
            }

            // Show quests with waypoints
            int buttonID = 100;
            foreach (var quest in questsWithWaypoints)
            {
                bool isSelected = quest.QuestID == m_SelectedQuestID;
                int textColor = isSelected ? SelectedColor : LabelColor;

                AddButton(20, y, isSelected ? 4006 : 4005, 4007, buttonID, GumpButtonType.Reply, 0);
                AddHtml(55, y, 420, 20, Color($"{quest.Title} ({quest.Waypoints.Count} waypoints)", textColor), false, false);
                y += 25;

                buttonID++;

                if (y > 350)
                    break; // Prevent overflow
            }

            // Next button
            if (m_SelectedQuestID > 0)
            {
                AddButton(380, 400, 4005, 4007, 1, GumpButtonType.Reply, 0);
                AddHtml(415, 400, 60, 20, Color("Next", ValueColor), false, false);
            }
        }

        #endregion

        #region Step 1: Select Waypoint

        private void BuildStep1_SelectWaypoint(ref int y)
        {
            var quest = DynamicQuestManager.GetQuest(m_SelectedQuestID);
            if (quest == null)
            {
                AddHtml(20, y, 460, 20, Color("Quest not found. Please go back.", ErrorColor), false, false);
                AddBackButton();
                return;
            }

            AddHtml(20, y, 460, 20, Color($"Quest: {quest.Title}", ValueColor), false, false);
            y += 25;

            AddHtml(20, y, 460, 20, Color("Select a waypoint to link:", LabelColor), false, false);
            y += 25;

            if (quest.Waypoints.Count == 0)
            {
                AddHtml(20, y, 460, 20, Color("This quest has no waypoints.", ErrorColor), false, false);
                AddBackButton();
                return;
            }

            int buttonID = 200;
            foreach (var waypoint in quest.Waypoints.OrderBy(w => w.OrderIndex))
            {
                bool isSelected = waypoint.WaypointID == m_SelectedWaypointID;
                int textColor = isSelected ? SelectedColor : LabelColor;

                // Color by type
                string typeTag = waypoint.Type.ToString();
                int typeColor;
                switch (waypoint.Type)
                {
                    case Server.Custom.VystiaClasses.Quests.WaypointType.Origin:
                        typeColor = 0x00FF00; // Green
                        break;
                    case Server.Custom.VystiaClasses.Quests.WaypointType.NPCCompletion:
                        typeColor = 0xFFD700; // Gold
                        break;
                    case Server.Custom.VystiaClasses.Quests.WaypointType.BossCompletion:
                        typeColor = 0xFF4500; // Red-orange
                        break;
                    default:
                        typeColor = LabelColor;
                        break;
                }

                AddButton(20, y, isSelected ? 4006 : 4005, 4007, buttonID, GumpButtonType.Reply, 0);
                AddHtml(55, y, 60, 20, Color($"[{typeTag}]", typeColor), false, false);
                AddHtml(130, y, 300, 20, Color($"{waypoint.Name}", textColor), false, false);

                // Show if NPC already linked
                if (waypoint.AssignedNPCSerial.Value != -1)
                {
                    AddHtml(400, y, 80, 20, Color("(Linked)", 0x888888), false, false);
                }

                y += 25;
                buttonID++;

                if (y > 350)
                    break;
            }

            // Navigation buttons
            AddBackButton();

            if (m_SelectedWaypointID > 0)
            {
                AddButton(380, 400, 4005, 4007, 1, GumpButtonType.Reply, 0);
                AddHtml(415, 400, 60, 20, Color("Next", ValueColor), false, false);
            }
        }

        #endregion

        #region Step 2: Configure NPC

        private void BuildStep2_ConfigureNPC(ref int y)
        {
            var quest = DynamicQuestManager.GetQuest(m_SelectedQuestID);
            var waypoint = quest?.GetWaypoint(m_SelectedWaypointID);

            if (waypoint == null)
            {
                AddHtml(20, y, 460, 20, Color("Waypoint not found. Please go back.", ErrorColor), false, false);
                AddBackButton();
                return;
            }

            AddHtml(20, y, 460, 20, Color($"Quest: {quest.Title}", ValueColor), false, false);
            y += 22;
            AddHtml(20, y, 460, 20, Color($"Waypoint: {waypoint.Name} ({waypoint.Type})", ValueColor), false, false);
            y += 30;

            // NPC Name
            AddHtml(20, y, 100, 20, Color("NPC Name:", LabelColor), false, false);
            AddBackground(125, y - 2, 200, 24, 9350);
            AddTextEntry(130, y, 190, 20, LabelColor, 0, string.IsNullOrEmpty(m_NPCName) ? "Quest Giver" : m_NPCName);
            y += 30;

            // NPC Title
            AddHtml(20, y, 100, 20, Color("NPC Title:", LabelColor), false, false);
            AddBackground(125, y - 2, 200, 24, 9350);
            AddTextEntry(130, y, 190, 20, LabelColor, 1, string.IsNullOrEmpty(m_NPCTitle) ? "the Quest NPC" : m_NPCTitle);
            y += 35;

            // Personality Type
            AddHtml(20, y, 460, 20, Color("Personality:", LabelColor), false, false);
            y += 22;

            var personalities = new[] {
                NPCPersonalities.PersonalityType.Sage,
                NPCPersonalities.PersonalityType.Warrior,
                NPCPersonalities.PersonalityType.Mage,
                NPCPersonalities.PersonalityType.Merchant,
                NPCPersonalities.PersonalityType.Noble,
                NPCPersonalities.PersonalityType.Commoner,
                NPCPersonalities.PersonalityType.Healer,
                NPCPersonalities.PersonalityType.Guard
            };

            int personalityButtonID = 300;
            int px = 20;
            foreach (var p in personalities)
            {
                bool isSelected = m_PersonalityType == p;
                AddButton(px, y, isSelected ? 4006 : 4005, 4007, personalityButtonID, GumpButtonType.Reply, 0);
                AddHtml(px + 20, y, 80, 20, Color(p.ToString(), isSelected ? SelectedColor : LabelColor), false, false);

                px += 105;
                if (px > 400)
                {
                    px = 20;
                    y += 25;
                }
                personalityButtonID++;
            }
            y += 35;

            // Speech Pattern
            AddHtml(20, y, 460, 20, Color("Speech Pattern:", LabelColor), false, false);
            y += 22;

            var patterns = Enum.GetValues(typeof(NPCPersonalities.SpeechPattern)).Cast<NPCPersonalities.SpeechPattern>().ToArray();
            int patternButtonID = 400;
            px = 20;
            foreach (var p in patterns)
            {
                bool isSelected = m_SpeechPattern == p;
                AddButton(px, y, isSelected ? 4006 : 4005, 4007, patternButtonID, GumpButtonType.Reply, 0);
                AddHtml(px + 20, y, 80, 20, Color(p.ToString(), isSelected ? SelectedColor : LabelColor), false, false);

                px += 95;
                patternButtonID++;
            }

            // Navigation
            AddBackButton();
            AddButton(380, 400, 4005, 4007, 1, GumpButtonType.Reply, 0);
            AddHtml(415, 400, 60, 20, Color("Next", ValueColor), false, false);
        }

        #endregion

        #region Step 3: Confirm & Spawn

        private void BuildStep3_Confirm(ref int y)
        {
            var quest = DynamicQuestManager.GetQuest(m_SelectedQuestID);
            var waypoint = quest?.GetWaypoint(m_SelectedWaypointID);

            if (waypoint == null)
            {
                AddHtml(20, y, 460, 20, Color("Configuration error. Please go back.", ErrorColor), false, false);
                AddBackButton();
                return;
            }

            AddHtml(20, y, 460, 20, Center(Color("Confirm NPC Spawn", TitleColor)), false, false);
            y += 35;

            // Summary
            AddHtml(20, y, 150, 20, Color("Quest:", LabelColor), false, false);
            AddHtml(175, y, 300, 20, Color(quest.Title, ValueColor), false, false);
            y += 22;

            AddHtml(20, y, 150, 20, Color("Waypoint:", LabelColor), false, false);
            AddHtml(175, y, 300, 20, Color($"{waypoint.Name} ({waypoint.Type})", ValueColor), false, false);
            y += 22;

            AddHtml(20, y, 150, 20, Color("NPC Name:", LabelColor), false, false);
            AddHtml(175, y, 300, 20, Color(m_NPCName, ValueColor), false, false);
            y += 22;

            AddHtml(20, y, 150, 20, Color("NPC Title:", LabelColor), false, false);
            AddHtml(175, y, 300, 20, Color(m_NPCTitle, ValueColor), false, false);
            y += 22;

            AddHtml(20, y, 150, 20, Color("Personality:", LabelColor), false, false);
            AddHtml(175, y, 300, 20, Color(m_PersonalityType.ToString(), ValueColor), false, false);
            y += 22;

            AddHtml(20, y, 150, 20, Color("Speech Pattern:", LabelColor), false, false);
            AddHtml(175, y, 300, 20, Color(m_SpeechPattern.ToString(), ValueColor), false, false);
            y += 22;

            AddHtml(20, y, 150, 20, Color("Spawn Location:", LabelColor), false, false);
            AddHtml(175, y, 300, 20, Color($"{m_From.Location} ({m_From.Map})", ValueColor), false, false);
            y += 40;

            // Warning if waypoint already linked
            if (waypoint.AssignedNPCSerial.Value != -1)
            {
                AddHtml(20, y, 460, 40, Color("Warning: This waypoint already has an NPC linked. The new NPC will replace the link.", 0xFFAA00), false, false);
                y += 45;
            }

            // Navigation
            AddBackButton();

            // Spawn button
            AddButton(320, 400, 4005, 4007, 500, GumpButtonType.Reply, 0);
            AddHtml(355, 400, 120, 20, Color("SPAWN NPC", SelectedColor), false, false);
        }

        #endregion

        #region Navigation Helpers

        private void AddBackButton()
        {
            AddButton(20, 400, 4014, 4016, 2, GumpButtonType.Reply, 0);
            AddHtml(55, 400, 60, 20, Color("Back", LabelColor), false, false);
        }

        #endregion

        #region OnResponse

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            Mobile from = sender.Mobile;
            if (from == null)
                return;

            int buttonID = info.ButtonID;

            // Cancel
            if (buttonID == 0)
                return;

            // Get text entries for step 2
            string enteredName = GetString(info, 0) ?? m_NPCName;
            string enteredTitle = GetString(info, 1) ?? m_NPCTitle;

            // Next button
            if (buttonID == 1)
            {
                if (m_Step < 3)
                {
                    from.SendGump(new AddQuestNPCGump(from, m_Step + 1, m_SelectedQuestID, m_SelectedWaypointID,
                        enteredName, enteredTitle, m_PersonalityType, m_SpeechPattern));
                }
                return;
            }

            // Back button
            if (buttonID == 2)
            {
                if (m_Step > 0)
                {
                    from.SendGump(new AddQuestNPCGump(from, m_Step - 1, m_SelectedQuestID, m_SelectedWaypointID,
                        enteredName, enteredTitle, m_PersonalityType, m_SpeechPattern));
                }
                return;
            }

            // Quest selection (100-199)
            if (buttonID >= 100 && buttonID < 200)
            {
                var quests = DynamicQuestManager.GetAllDynamicQuests().Where(q => q.Waypoints.Count > 0).ToList();
                int index = buttonID - 100;
                if (index >= 0 && index < quests.Count)
                {
                    m_SelectedQuestID = quests[index].QuestID;
                    m_SelectedWaypointID = 0; // Reset waypoint selection
                }
                from.SendGump(new AddQuestNPCGump(from, 0, m_SelectedQuestID, m_SelectedWaypointID,
                    enteredName, enteredTitle, m_PersonalityType, m_SpeechPattern));
                return;
            }

            // Waypoint selection (200-299)
            if (buttonID >= 200 && buttonID < 300)
            {
                var quest = DynamicQuestManager.GetQuest(m_SelectedQuestID);
                if (quest != null)
                {
                    var waypoints = quest.Waypoints.OrderBy(w => w.OrderIndex).ToList();
                    int index = buttonID - 200;
                    if (index >= 0 && index < waypoints.Count)
                    {
                        m_SelectedWaypointID = waypoints[index].WaypointID;
                    }
                }
                from.SendGump(new AddQuestNPCGump(from, 1, m_SelectedQuestID, m_SelectedWaypointID,
                    enteredName, enteredTitle, m_PersonalityType, m_SpeechPattern));
                return;
            }

            // Personality selection (300-399)
            if (buttonID >= 300 && buttonID < 400)
            {
                var personalities = new[] {
                    NPCPersonalities.PersonalityType.Sage,
                    NPCPersonalities.PersonalityType.Warrior,
                    NPCPersonalities.PersonalityType.Mage,
                    NPCPersonalities.PersonalityType.Merchant,
                    NPCPersonalities.PersonalityType.Noble,
                    NPCPersonalities.PersonalityType.Commoner,
                    NPCPersonalities.PersonalityType.Healer,
                    NPCPersonalities.PersonalityType.Guard
                };
                int index = buttonID - 300;
                if (index >= 0 && index < personalities.Length)
                {
                    m_PersonalityType = personalities[index];
                }
                from.SendGump(new AddQuestNPCGump(from, 2, m_SelectedQuestID, m_SelectedWaypointID,
                    enteredName, enteredTitle, m_PersonalityType, m_SpeechPattern));
                return;
            }

            // Speech pattern selection (400-499)
            if (buttonID >= 400 && buttonID < 500)
            {
                var patterns = Enum.GetValues(typeof(NPCPersonalities.SpeechPattern)).Cast<NPCPersonalities.SpeechPattern>().ToArray();
                int index = buttonID - 400;
                if (index >= 0 && index < patterns.Length)
                {
                    m_SpeechPattern = patterns[index];
                }
                from.SendGump(new AddQuestNPCGump(from, 2, m_SelectedQuestID, m_SelectedWaypointID,
                    enteredName, enteredTitle, m_PersonalityType, m_SpeechPattern));
                return;
            }

            // Spawn NPC (500)
            if (buttonID == 500)
            {
                SpawnNPC(from, enteredName, enteredTitle);
                return;
            }
        }

        private void SpawnNPC(Mobile from, string name, string title)
        {
            var quest = DynamicQuestManager.GetQuest(m_SelectedQuestID);
            var waypoint = quest?.GetWaypoint(m_SelectedWaypointID);

            if (quest == null || waypoint == null)
            {
                from.SendMessage("Error: Quest or waypoint not found.");
                return;
            }

            // Create the NPC
            var npc = new QuestNPC(
                string.IsNullOrEmpty(name) ? "Quest Giver" : name,
                string.IsNullOrEmpty(title) ? "the Quest NPC" : title
            );

            npc.PersonalityType = m_PersonalityType;
            npc.SpeechPattern = m_SpeechPattern;

            // Link to waypoint
            npc.LinkToWaypoint(m_SelectedQuestID, m_SelectedWaypointID);

            // Spawn at GM's location
            npc.MoveToWorld(from.Location, from.Map);

            // Visual effect
            Effects.SendLocationParticles(npc, 0x376A, 9, 32, 5008);
            npc.PlaySound(0x1FE);

            from.SendMessage($"QuestNPC '{npc.Name}' spawned and linked to waypoint '{waypoint.Name}'.");

            // Save quests to persist the link
            DynamicQuestManager.Save();

            from.SendMessage("Quest data saved.");

            if (m_ReturnToQuestWizard)
            {
                Server.Custom.VystiaClasses.Gumps.VystiaQuestWizardGump.OpenSpawnNPCs(from, m_SelectedQuestID, m_SelectedWaypointID);
            }
        }

        private string GetString(RelayInfo info, int id)
        {
            TextRelay relay = info.GetTextEntry(id);
            return relay?.Text?.Trim();
        }

        #endregion

        #region HTML Helpers

        private string Color(string text, int color)
        {
            return $"<BASEFONT COLOR=#{color:X6}>{text}</BASEFONT>";
        }

        private string Center(string text)
        {
            return $"<CENTER>{text}</CENTER>";
        }

        #endregion
    }
}
