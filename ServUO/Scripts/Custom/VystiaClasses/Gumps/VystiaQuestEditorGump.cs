using System;
using System.Collections.Generic;
using System.Linq;
using Server;
using Server.Gumps;
using Server.Mobiles;
using Server.Network;
using Server.Commands;
using Server.Custom.VystiaClasses.Quests;

namespace Server.Custom.VystiaClasses.Gumps
{
    public class VystiaQuestEditorGump : Gump
    {
        private Mobile m_From;
        private int m_Page;
        private DynamicQuest m_EditingQuest;
        private int m_EditMode; // 0=view, 1=edit details, 2=objectives, 3=rewards, 4=waypoints, 5=single waypoint
        private int m_SelectedWaypointID;

        // Colors
        private const int LabelColor = 0xFFFFFF;
        private const int HeaderColor = 0x00FFFF;
        private const int ValueColor = 0x90EE90;
        private const int WarningColor = 0xFF6600;

        public VystiaQuestEditorGump(Mobile from, int page = 0, DynamicQuest editQuest = null, int editMode = 0, int selectedWaypointId = 0)
            : base(50, 50)
        {
            m_From = from;
            m_Page = page;
            m_EditingQuest = editQuest;
            m_EditMode = editMode;
            m_SelectedWaypointID = selectedWaypointId;

            Closable = true;
            Disposable = true;
            Dragable = true;
            Resizable = false;

            AddPage(0);

            // Background
            AddBackground(0, 0, 600, 500, 9270);
            AddAlphaRegion(10, 10, 580, 480);

            // Title
            AddHtml(0, 15, 600, 20, Center(Color("Vystia Quest Editor", HeaderColor)), false, false);

            if (m_EditingQuest == null)
                BuildQuestList();
            else if (m_EditMode == 1)
                BuildEditDetails();
            else if (m_EditMode == 2)
                BuildEditObjectives();
            else if (m_EditMode == 3)
                BuildEditRewards();
            else if (m_EditMode == 4)
                BuildEditWaypoints();
            else if (m_EditMode == 5)
                BuildEditSingleWaypoint();
            else
                BuildQuestView();
        }

        private void BuildQuestList()
        {
            AddHtml(20, 45, 200, 20, Color("All Quests", HeaderColor), false, false);

            // New Quest button
            AddButton(450, 45, 4005, 4007, 1, GumpButtonType.Reply, 0);
            AddHtml(485, 45, 100, 20, Color("New Quest", ValueColor), false, false);

            // Column headers
            int y = 75;
            AddHtml(20, y, 40, 20, Color("ID", HeaderColor), false, false);
            AddHtml(60, y, 200, 20, Color("Title", HeaderColor), false, false);
            AddHtml(270, y, 100, 20, Color("Class", HeaderColor), false, false);
            AddHtml(380, y, 80, 20, Color("Tier", HeaderColor), false, false);
            AddHtml(470, y, 100, 20, Color("Actions", HeaderColor), false, false);

            y += 25;
            AddImage(20, y - 5, 0x39);
            y += 5;

            var quests = DynamicQuestManager.GetAllDynamicQuests();
            int startIndex = m_Page * 12;
            int endIndex = Math.Min(startIndex + 12, quests.Count);

            for (int i = startIndex; i < endIndex; i++)
            {
                var quest = quests[i];
                int buttonBase = 100 + (i - startIndex) * 10;

                AddHtml(20, y, 40, 20, Color(quest.QuestID.ToString(), LabelColor), false, false);
                AddHtml(60, y, 200, 20, Color(Truncate(quest.Title, 25), LabelColor), false, false);
                AddHtml(270, y, 100, 20, Color(quest.RequiredClass.ToString(), LabelColor), false, false);
                AddHtml(380, y, 80, 20, Color(quest.Tier.ToString(), LabelColor), false, false);

                // Edit button
                AddButton(470, y, 4005, 4007, buttonBase + 1, GumpButtonType.Reply, 0);
                AddHtml(505, y, 40, 20, Color("Edit", ValueColor), false, false);

                // Delete button
                AddButton(540, y, 4017, 4019, buttonBase + 2, GumpButtonType.Reply, 0);

                y += 25;
            }

            // Pagination
            if (m_Page > 0)
            {
                AddButton(200, 460, 4014, 4016, 2, GumpButtonType.Reply, 0);
                AddHtml(235, 460, 50, 20, Color("Prev", LabelColor), false, false);
            }

            if (endIndex < quests.Count)
            {
                AddButton(350, 460, 4005, 4007, 3, GumpButtonType.Reply, 0);
                AddHtml(385, 460, 50, 20, Color("Next", LabelColor), false, false);
            }

            AddHtml(280, 460, 60, 20, Center(Color($"{m_Page + 1}", LabelColor)), false, false);
        }

        private void BuildQuestView()
        {
            AddHtml(20, 45, 400, 20, Color($"Quest: {m_EditingQuest.Title}", HeaderColor), false, false);

            // Back button
            AddButton(500, 45, 4014, 4016, 10, GumpButtonType.Reply, 0);
            AddHtml(535, 45, 50, 20, Color("Back", LabelColor), false, false);

            int y = 80;

            // Quest details
            AddHtml(20, y, 100, 20, Color("ID:", LabelColor), false, false);
            AddHtml(130, y, 200, 20, Color(m_EditingQuest.QuestID.ToString(), ValueColor), false, false);
            y += 25;

            AddHtml(20, y, 100, 20, Color("Class:", LabelColor), false, false);
            AddHtml(130, y, 200, 20, Color(m_EditingQuest.RequiredClass.ToString(), ValueColor), false, false);
            y += 25;

            AddHtml(20, y, 100, 20, Color("Tier:", LabelColor), false, false);
            AddHtml(130, y, 200, 20, Color(m_EditingQuest.Tier.ToString(), ValueColor), false, false);
            y += 25;

            AddHtml(20, y, 100, 20, Color("Prereq ID:", LabelColor), false, false);
            AddHtml(130, y, 200, 20, Color(m_EditingQuest.PrerequisiteQuestID.ToString(), ValueColor), false, false);
            y += 25;

            AddHtml(20, y, 560, 20, Color("Description:", LabelColor), false, false);
            y += 20;
            AddHtml(20, y, 560, 60, Color(m_EditingQuest.Description, ValueColor), false, true);
            y += 70;

            // Edit buttons row 1
            AddButton(20, y, 4005, 4007, 11, GumpButtonType.Reply, 0);
            AddHtml(55, y, 100, 20, Color("Edit Details", ValueColor), false, false);

            AddButton(160, y, 4005, 4007, 12, GumpButtonType.Reply, 0);
            AddHtml(195, y, 100, 20, Color("Objectives", ValueColor), false, false);

            AddButton(300, y, 4005, 4007, 13, GumpButtonType.Reply, 0);
            AddHtml(335, y, 80, 20, Color("Rewards", ValueColor), false, false);

            AddButton(420, y, 4005, 4007, 15, GumpButtonType.Reply, 0);
            AddHtml(455, y, 100, 20, Color("Waypoints", ValueColor), false, false);

            y += 35;

            // Objectives
            AddHtml(20, y, 200, 20, Color("Objectives:", HeaderColor), false, false);
            y += 25;

            foreach (var obj in m_EditingQuest.ObjectivesList)
            {
                AddHtml(30, y, 500, 20, Color($"- {obj.DisplayName}: {obj.Amount}", LabelColor), false, false);
                y += 20;
            }

            y += 15;

            // Rewards
            AddHtml(20, y, 200, 20, Color("Rewards:", HeaderColor), false, false);
            y += 25;

            foreach (var reward in m_EditingQuest.Rewards)
            {
                AddHtml(30, y, 500, 20, Color($"- {reward.GetDescription()}", LabelColor), false, false);
                y += 20;
            }

            // Test button
            AddButton(20, 450, 4005, 4007, 14, GumpButtonType.Reply, 0);
            AddHtml(55, 450, 200, 20, Color("Give Quest to Target", WarningColor), false, false);
        }

        private void BuildEditDetails()
        {
            AddHtml(20, 45, 400, 20, Color("Edit Quest Details", HeaderColor), false, false);

            // Back button
            AddButton(500, 45, 4014, 4016, 10, GumpButtonType.Reply, 0);
            AddHtml(535, 45, 50, 20, Color("Back", LabelColor), false, false);

            int y = 80;

            // Title
            AddHtml(20, y, 100, 20, Color("Title:", LabelColor), false, false);
            AddBackground(130, y - 2, 350, 24, 9350);
            AddTextEntry(135, y, 340, 20, LabelColor, 0, m_EditingQuest.Title);
            y += 35;

            // Description
            AddHtml(20, y, 100, 20, Color("Description:", LabelColor), false, false);
            y += 22;
            AddBackground(20, y, 560, 80, 9350);
            AddTextEntry(25, y + 5, 550, 70, LabelColor, 1, m_EditingQuest.Description);
            y += 95;

            // Required Class
            AddHtml(20, y, 100, 20, Color("Class:", LabelColor), false, false);
            AddBackground(130, y - 2, 150, 24, 9350);
            AddTextEntry(135, y, 140, 20, LabelColor, 2, ((int)m_EditingQuest.RequiredClass).ToString());
            AddHtml(290, y, 300, 20, Color("(0=None, 1-26=Class ID)", ValueColor), false, false);
            y += 35;

            // Tier
            AddHtml(20, y, 100, 20, Color("Tier:", LabelColor), false, false);
            AddBackground(130, y - 2, 150, 24, 9350);
            AddTextEntry(135, y, 140, 20, LabelColor, 3, ((int)m_EditingQuest.Tier).ToString());
            AddHtml(290, y, 300, 20, Color("(0=Init, 1=Appr, 2=Jour, 3=Master)", ValueColor), false, false);
            y += 35;

            // Prerequisite
            AddHtml(20, y, 100, 20, Color("Prereq ID:", LabelColor), false, false);
            AddBackground(130, y - 2, 150, 24, 9350);
            AddTextEntry(135, y, 140, 20, LabelColor, 4, m_EditingQuest.PrerequisiteQuestID.ToString());
            AddHtml(290, y, 300, 20, Color("(0=None, or Quest ID)", ValueColor), false, false);
            y += 50;

            // Save button
            AddButton(250, y, 4005, 4007, 20, GumpButtonType.Reply, 0);
            AddHtml(285, y, 100, 20, Color("Save Changes", ValueColor), false, false);

            // Class reference
            y += 40;
            AddHtml(20, y, 560, 20, Color("Class IDs: 1=IceMage, 2=Warlock, 3=Necromancer, 4=Druid, 5=Sorcerer, 6=Bard...", LabelColor), false, false);
        }

        private void BuildEditObjectives()
        {
            AddHtml(20, 45, 400, 20, Color("Edit Objectives", HeaderColor), false, false);

            // Back button
            AddButton(500, 45, 4014, 4016, 10, GumpButtonType.Reply, 0);
            AddHtml(535, 45, 50, 20, Color("Back", LabelColor), false, false);

            int y = 80;

            // Current objectives
            AddHtml(20, y, 200, 20, Color("Current Objectives:", HeaderColor), false, false);
            y += 25;

            int objIndex = 0;
            foreach (var obj in m_EditingQuest.ObjectivesList)
            {
                AddHtml(30, y, 400, 20, Color($"{obj.Key}: {obj.DisplayName} x{obj.Amount}", LabelColor), false, false);
                AddButton(500, y, 4017, 4019, 200 + objIndex, GumpButtonType.Reply, 0); // Delete
                y += 25;
                objIndex++;
            }

            y += 20;
            AddImage(20, y - 5, 0x39);
            y += 10;

            // Add new objective
            AddHtml(20, y, 200, 20, Color("Add New Objective:", HeaderColor), false, false);
            y += 30;

            AddHtml(20, y, 80, 20, Color("Key:", LabelColor), false, false);
            AddBackground(100, y - 2, 150, 24, 9350);
            AddTextEntry(105, y, 140, 20, LabelColor, 10, "kill_creature");
            y += 30;

            AddHtml(20, y, 80, 20, Color("Display:", LabelColor), false, false);
            AddBackground(100, y - 2, 300, 24, 9350);
            AddTextEntry(105, y, 290, 20, LabelColor, 11, "Kill Creatures");
            y += 30;

            AddHtml(20, y, 80, 20, Color("Amount:", LabelColor), false, false);
            AddBackground(100, y - 2, 80, 24, 9350);
            AddTextEntry(105, y, 70, 20, LabelColor, 12, "10");
            y += 40;

            AddButton(100, y, 4005, 4007, 30, GumpButtonType.Reply, 0);
            AddHtml(135, y, 150, 20, Color("Add Objective", ValueColor), false, false);

            // Help text
            y += 50;
            AddHtml(20, y, 560, 80, Color(
                "Objective Keys:\n" +
                "kill_<type> - Kill creatures (e.g., kill_frostwolf)\n" +
                "collect_<item> - Collect items (e.g., collect_frostbloom)\n" +
                "talk_to_<npc> - Talk to NPC\n" +
                "cast_<spell> - Cast spells\n" +
                "visit_<location> - Visit location", LabelColor), false, false);
        }

        private void BuildEditRewards()
        {
            AddHtml(20, 45, 400, 20, Color("Edit Rewards", HeaderColor), false, false);

            // Back button
            AddButton(500, 45, 4014, 4016, 10, GumpButtonType.Reply, 0);
            AddHtml(535, 45, 50, 20, Color("Back", LabelColor), false, false);

            int y = 80;

            // Current rewards
            AddHtml(20, y, 200, 20, Color("Current Rewards:", HeaderColor), false, false);
            y += 25;

            int rewardIndex = 0;
            foreach (var reward in m_EditingQuest.Rewards)
            {
                AddHtml(30, y, 400, 20, Color(reward.GetDescription(), LabelColor), false, false);
                AddButton(500, y, 4017, 4019, 300 + rewardIndex, GumpButtonType.Reply, 0); // Delete
                y += 25;
                rewardIndex++;
            }

            y += 20;
            AddImage(20, y - 5, 0x39);
            y += 10;

            // Add rewards section
            AddHtml(20, y, 200, 20, Color("Add Reward:", HeaderColor), false, false);
            y += 30;

            // Gold reward
            AddButton(20, y, 4005, 4007, 40, GumpButtonType.Reply, 0);
            AddHtml(55, y, 60, 20, Color("Gold:", LabelColor), false, false);
            AddBackground(120, y - 2, 100, 24, 9350);
            AddTextEntry(125, y, 90, 20, LabelColor, 20, "500");
            y += 35;

            // Skill reward
            AddButton(20, y, 4005, 4007, 41, GumpButtonType.Reply, 0);
            AddHtml(55, y, 60, 20, Color("Skill:", LabelColor), false, false);
            AddBackground(120, y - 2, 120, 24, 9350);
            AddTextEntry(125, y, 110, 20, LabelColor, 21, "Magery");
            AddHtml(250, y, 30, 20, Color("+", LabelColor), false, false);
            AddBackground(270, y - 2, 60, 24, 9350);
            AddTextEntry(275, y, 50, 20, LabelColor, 22, "5.0");
            y += 35;

            // Item reward
            AddButton(20, y, 4005, 4007, 42, GumpButtonType.Reply, 0);
            AddHtml(55, y, 60, 20, Color("Item:", LabelColor), false, false);
            AddBackground(120, y - 2, 150, 24, 9350);
            AddTextEntry(125, y, 140, 20, LabelColor, 23, "Frostbloom");
            AddHtml(280, y, 20, 20, Color("x", LabelColor), false, false);
            AddBackground(300, y - 2, 50, 24, 9350);
            AddTextEntry(305, y, 40, 20, LabelColor, 24, "10");
            y += 35;

            // Title reward
            AddButton(20, y, 4005, 4007, 43, GumpButtonType.Reply, 0);
            AddHtml(55, y, 60, 20, Color("Title:", LabelColor), false, false);
            AddBackground(120, y - 2, 200, 24, 9350);
            AddTextEntry(125, y, 190, 20, LabelColor, 25, "the Frozen");
        }

        private void BuildEditWaypoints()
        {
            AddHtml(20, 45, 300, 20, Color("Edit Waypoints", HeaderColor), false, false);

            // Back button
            AddButton(500, 45, 4014, 4016, 10, GumpButtonType.Reply, 0);
            AddHtml(535, 45, 50, 20, Color("Back", LabelColor), false, false);

            int y = 75;

            // Quick action buttons
            AddButton(20, y, 4005, 4007, 500, GumpButtonType.Reply, 0);
            AddHtml(55, y, 90, 20, Color("Add Origin", ValueColor), false, false);

            AddButton(160, y, 4005, 4007, 501, GumpButtonType.Reply, 0);
            AddHtml(195, y, 100, 20, Color("Add Waypoint", ValueColor), false, false);

            AddButton(310, y, 4005, 4007, 502, GumpButtonType.Reply, 0);
            AddHtml(345, y, 90, 20, Color("Add Finish", ValueColor), false, false);

            y += 35;

            // Waypoint list header
            AddHtml(20, y, 30, 20, Color("#", HeaderColor), false, false);
            AddHtml(50, y, 80, 20, Color("Type", HeaderColor), false, false);
            AddHtml(140, y, 180, 20, Color("Name", HeaderColor), false, false);
            AddHtml(330, y, 80, 20, Color("NPC", HeaderColor), false, false);
            AddHtml(420, y, 150, 20, Color("Actions", HeaderColor), false, false);

            y += 25;
            AddImage(20, y - 5, 0x39);
            y += 5;

            if (m_EditingQuest != null)
            {
                foreach (var waypoint in m_EditingQuest.Waypoints.OrderBy(w => w.OrderIndex))
                {
                    int buttonBase = 600 + waypoint.WaypointID * 10;

                    // Order index
                    AddHtml(20, y, 30, 20, Color(waypoint.OrderIndex.ToString(), LabelColor), false, false);

                    // Type (color-coded)
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
                    AddHtml(50, y, 80, 20, Color(waypoint.Type.ToString(), typeColor), false, false);

                    // Name
                    AddHtml(140, y, 180, 20, Color(Truncate(waypoint.Name, 22), LabelColor), false, false);

                    // NPC status
                    string npcStatus = waypoint.AssignedNPCSerial.Value != -1 ? "Linked" : "None";
                    int npcColor = npcStatus == "Linked" ? ValueColor : WarningColor;
                    AddHtml(330, y, 80, 20, Color(npcStatus, npcColor), false, false);

                    // Edit button
                    AddButton(420, y, 4005, 4007, buttonBase + 1, GumpButtonType.Reply, 0);

                    // Move Up (if not first)
                    if (waypoint.OrderIndex > 0)
                        AddButton(445, y, 4014, 4016, buttonBase + 2, GumpButtonType.Reply, 0);
                    else
                        AddImage(445, y, 4014, 900); // Grayed out

                    // Move Down (if not last)
                    if (waypoint.OrderIndex < m_EditingQuest.Waypoints.Count - 1)
                        AddButton(470, y, 4005, 4007, buttonBase + 3, GumpButtonType.Reply, 0);
                    else
                        AddImage(470, y, 4005, 900); // Grayed out

                    // Delete
                    AddButton(500, y, 4017, 4019, buttonBase + 4, GumpButtonType.Reply, 0);

                    y += 25;
                }
            }

            // Help text
            y = 420;
            AddHtml(20, y, 560, 60, Color(
                "Waypoint Types:\n" +
                "- Origin: Starting NPC/location where quest begins\n" +
                "- Waypoint: Intermediate steps (kill, collect, visit)\n" +
                "- Finish: Final NPC or boss to complete the quest", LabelColor), false, false);
        }

        private void BuildEditSingleWaypoint()
        {
            var waypoint = m_EditingQuest?.GetWaypoint(m_SelectedWaypointID);
            if (waypoint == null)
            {
                AddHtml(20, 45, 400, 20, Color("Waypoint not found", WarningColor), false, false);
                AddButton(500, 45, 4014, 4016, 10, GumpButtonType.Reply, 0);
                AddHtml(535, 45, 50, 20, Color("Back", LabelColor), false, false);
                return;
            }

            AddHtml(20, 45, 400, 20, Color($"Edit Waypoint: {waypoint.Name}", HeaderColor), false, false);

            // Back button
            AddButton(500, 45, 4014, 4016, 16, GumpButtonType.Reply, 0);
            AddHtml(535, 45, 50, 20, Color("Back", LabelColor), false, false);

            int y = 80;

            // Name
            AddHtml(20, y, 80, 20, Color("Name:", LabelColor), false, false);
            AddBackground(100, y - 2, 300, 24, 9350);
            AddTextEntry(105, y, 290, 20, LabelColor, 50, waypoint.Name);
            y += 35;

            // Description
            AddHtml(20, y, 80, 20, Color("Desc:", LabelColor), false, false);
            AddBackground(100, y - 2, 480, 24, 9350);
            AddTextEntry(105, y, 470, 20, LabelColor, 51, waypoint.Description ?? "");
            y += 35;

            // Type selection (radio buttons style)
            AddHtml(20, y, 80, 20, Color("Type:", LabelColor), false, false);

            AddButton(100, y, waypoint.Type == Server.Custom.VystiaClasses.Quests.WaypointType.Origin ? 4006 : 4005, 4007, 700, GumpButtonType.Reply, 0);
            AddHtml(125, y, 60, 20, Color("Origin", ValueColor), false, false);

            AddButton(195, y, waypoint.Type == Server.Custom.VystiaClasses.Quests.WaypointType.Waypoint ? 4006 : 4005, 4007, 701, GumpButtonType.Reply, 0);
            AddHtml(220, y, 70, 20, Color("Waypoint", ValueColor), false, false);

            AddButton(300, y, waypoint.Type == Server.Custom.VystiaClasses.Quests.WaypointType.BossCompletion ? 4006 : 4005, 4007, 702, GumpButtonType.Reply, 0);
            AddHtml(325, y, 70, 20, Color("Boss End", ValueColor), false, false);

            AddButton(405, y, waypoint.Type == Server.Custom.VystiaClasses.Quests.WaypointType.NPCCompletion ? 4006 : 4005, 4007, 703, GumpButtonType.Reply, 0);
            AddHtml(430, y, 70, 20, Color("NPC End", ValueColor), false, false);
            y += 35;

            // Condition
            AddHtml(20, y, 80, 20, Color("Condition:", LabelColor), false, false);
            AddBackground(100, y - 2, 150, 24, 9350);
            AddTextEntry(105, y, 140, 20, LabelColor, 52, ((int)waypoint.Condition).ToString());
            AddHtml(260, y, 320, 20, Color("(0=TalkToNPC, 1=Location, 2=DefeatBoss, 3=Collect, 4=Cast, 5=Custom, 6=RecruitSidekick)", ValueColor), false, false);
            y += 35;

            // Location
            AddHtml(20, y, 80, 20, Color("Location:", LabelColor), false, false);
            string locText = waypoint.Location != Point3D.Zero
                ? $"{waypoint.Location.X}, {waypoint.Location.Y}, {waypoint.Location.Z}"
                : "Not set";
            AddHtml(100, y, 200, 20, Color(locText, ValueColor), false, false);
            AddButton(320, y, 4005, 4007, 710, GumpButtonType.Reply, 0);
            AddHtml(355, y, 150, 20, Color("Set to My Location", ValueColor), false, false);
            y += 35;

            // NPC Link
            AddHtml(20, y, 80, 20, Color("NPC:", LabelColor), false, false);
            string npcText = waypoint.AssignedNPCSerial.Value != -1
                ? $"Serial: {waypoint.AssignedNPCSerial.Value}"
                : "None";
            AddHtml(100, y, 150, 20, Color(npcText, ValueColor), false, false);
            AddButton(260, y, 4005, 4007, 711, GumpButtonType.Reply, 0);
            AddHtml(295, y, 80, 20, Color("Link NPC", ValueColor), false, false);
            AddButton(380, y, 4017, 4019, 712, GumpButtonType.Reply, 0);
            AddHtml(405, y, 80, 20, Color("Unlink", LabelColor), false, false);
            y += 35;

            // LLM Dialogue Context
            AddHtml(20, y, 150, 20, Color("LLM Context:", LabelColor), false, false);
            y += 22;
            AddBackground(20, y, 560, 60, 9350);
            AddTextEntry(25, y + 5, 550, 50, LabelColor, 53, waypoint.NPCDialogueContext ?? "");
            y += 75;

            // Objective key and amount (for custom conditions)
            AddHtml(20, y, 100, 20, Color("Objective Key:", LabelColor), false, false);
            AddBackground(120, y - 2, 150, 24, 9350);
            AddTextEntry(125, y, 140, 20, LabelColor, 54, waypoint.ObjectiveKey ?? "");

            AddHtml(290, y, 60, 20, Color("Amount:", LabelColor), false, false);
            AddBackground(350, y - 2, 60, 24, 9350);
            AddTextEntry(355, y, 50, 20, LabelColor, 55, waypoint.RequiredAmount.ToString());
            y += 45;

            // Save button
            AddButton(250, y, 4005, 4007, 720, GumpButtonType.Reply, 0);
            AddHtml(285, y, 100, 20, Color("Save Waypoint", ValueColor), false, false);
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            if (m_From == null || m_From.Deleted)
                return;

            int buttonID = info.ButtonID;

            if (buttonID == 0)
                return;

            // List view buttons
            if (m_EditingQuest == null)
            {
                if (buttonID == 1) // New Quest
                {
                    var quest = new DynamicQuest();
                    DynamicQuestManager.RegisterDynamicQuest(quest);
                    m_From.SendMessage("New quest created.");
                    m_From.SendGump(new VystiaQuestEditorGump(m_From, m_Page, quest, 0));
                    return;
                }
                else if (buttonID == 2) // Prev page
                {
                    m_From.SendGump(new VystiaQuestEditorGump(m_From, m_Page - 1));
                    return;
                }
                else if (buttonID == 3) // Next page
                {
                    m_From.SendGump(new VystiaQuestEditorGump(m_From, m_Page + 1));
                    return;
                }
                else if (buttonID >= 100)
                {
                    var quests = DynamicQuestManager.GetAllDynamicQuests();
                    int index = m_Page * 12 + (buttonID - 100) / 10;
                    int action = (buttonID - 100) % 10;

                    if (index < quests.Count)
                    {
                        var quest = quests[index];
                        if (action == 1) // Edit
                        {
                            m_From.SendGump(new VystiaQuestEditorGump(m_From, m_Page, quest, 0));
                            return;
                        }
                        else if (action == 2) // Delete
                        {
                            DynamicQuestManager.RemoveDynamicQuest(quest);
                            DynamicQuestManager.Save();
                            m_From.SendMessage($"Quest deleted: {quest.Title}");
                            m_From.SendGump(new VystiaQuestEditorGump(m_From, m_Page));
                            return;
                        }
                    }
                }
            }
            else // Editing a quest
            {
                if (buttonID == 10) // Back
                {
                    if (m_EditMode == 0)
                        m_From.SendGump(new VystiaQuestEditorGump(m_From, m_Page));
                    else
                        m_From.SendGump(new VystiaQuestEditorGump(m_From, m_Page, m_EditingQuest, 0));
                    return;
                }
                else if (buttonID == 11) // Edit Details
                {
                    m_From.SendGump(new VystiaQuestEditorGump(m_From, m_Page, m_EditingQuest, 1));
                    return;
                }
                else if (buttonID == 12) // Edit Objectives
                {
                    m_From.SendGump(new VystiaQuestEditorGump(m_From, m_Page, m_EditingQuest, 2));
                    return;
                }
                else if (buttonID == 13) // Edit Rewards
                {
                    m_From.SendGump(new VystiaQuestEditorGump(m_From, m_Page, m_EditingQuest, 3));
                    return;
                }
                else if (buttonID == 15) // Edit Waypoints
                {
                    m_From.SendGump(new VystiaQuestEditorGump(m_From, m_Page, m_EditingQuest, 4));
                    return;
                }
                else if (buttonID == 16) // Back from single waypoint to waypoints list
                {
                    m_From.SendGump(new VystiaQuestEditorGump(m_From, m_Page, m_EditingQuest, 4));
                    return;
                }
                else if (buttonID == 14) // Give quest to target
                {
                    m_From.SendMessage("Target a player to give them this quest.");
                    m_From.Target = new GiveQuestTarget(m_EditingQuest);
                    m_From.SendGump(new VystiaQuestEditorGump(m_From, m_Page, m_EditingQuest, 0));
                    return;
                }
                else if (buttonID == 20) // Save details
                {
                    string title = GetString(info, 0) ?? m_EditingQuest.Title;
                    string desc = GetString(info, 1) ?? m_EditingQuest.Description;
                    int classId = GetInt(info, 2, (int)m_EditingQuest.RequiredClass);
                    int tier = GetInt(info, 3, (int)m_EditingQuest.Tier);
                    int prereq = GetInt(info, 4, m_EditingQuest.PrerequisiteQuestID);

                    m_EditingQuest.SetTitle(title);
                    m_EditingQuest.SetDescription(desc);
                    m_EditingQuest.SetRequiredClass((PlayerClassTypeV2)classId);
                    m_EditingQuest.SetTier((QuestTier)tier);
                    m_EditingQuest.SetPrerequisiteQuestID(prereq);

                    m_From.SendMessage("Quest details saved.");
                    m_From.SendGump(new VystiaQuestEditorGump(m_From, m_Page, m_EditingQuest, 0));
                    return;
                }
                else if (buttonID == 30) // Add objective
                {
                    string key = GetString(info, 10) ?? "objective";
                    string display = GetString(info, 11) ?? "Objective";
                    int amount = GetInt(info, 12, 1);

                    m_EditingQuest.AddObjective(key, amount, display);
                    m_From.SendMessage($"Added objective: {display}");
                    m_From.SendGump(new VystiaQuestEditorGump(m_From, m_Page, m_EditingQuest, 2));
                    return;
                }
                else if (buttonID >= 200 && buttonID < 300) // Delete objective
                {
                    int index = buttonID - 200;
                    if (index < m_EditingQuest.ObjectivesList.Count)
                    {
                        var obj = m_EditingQuest.ObjectivesList[index];
                        m_EditingQuest.RemoveObjective(obj.Key);
                        m_From.SendMessage("Objective removed.");
                    }
                    m_From.SendGump(new VystiaQuestEditorGump(m_From, m_Page, m_EditingQuest, 2));
                    return;
                }
                else if (buttonID == 40) // Add gold reward
                {
                    int amount = GetInt(info, 20, 100);
                    m_EditingQuest.AddReward(new GoldReward(amount));
                    m_From.SendMessage($"Added gold reward: {amount}");
                    m_From.SendGump(new VystiaQuestEditorGump(m_From, m_Page, m_EditingQuest, 3));
                    return;
                }
                else if (buttonID == 41) // Add skill reward
                {
                    string skillName = GetString(info, 21) ?? "Magery";
                    double amount = GetDouble(info, 22, 5.0);

                    if (Enum.TryParse(skillName, true, out SkillName skill))
                    {
                        m_EditingQuest.AddReward(new SkillReward(skill, amount));
                        m_From.SendMessage($"Added skill reward: +{amount} {skill}");
                    }
                    else
                    {
                        m_From.SendMessage($"Unknown skill: {skillName}");
                    }
                    m_From.SendGump(new VystiaQuestEditorGump(m_From, m_Page, m_EditingQuest, 3));
                    return;
                }
                else if (buttonID == 42) // Add item reward
                {
                    string typeName = GetString(info, 23) ?? "Gold";
                    int amount = GetInt(info, 24, 1);

                    m_EditingQuest.AddReward(new ItemReward(typeName, amount));
                    m_From.SendMessage($"Added item reward: {amount}x {typeName}");
                    m_From.SendGump(new VystiaQuestEditorGump(m_From, m_Page, m_EditingQuest, 3));
                    return;
                }
                else if (buttonID == 43) // Add title reward
                {
                    string title = GetString(info, 25) ?? "the Hero";
                    m_EditingQuest.AddReward(new TitleReward(title));
                    m_From.SendMessage($"Added title reward: {title}");
                    m_From.SendGump(new VystiaQuestEditorGump(m_From, m_Page, m_EditingQuest, 3));
                    return;
                }
                else if (buttonID >= 300 && buttonID < 400) // Delete reward
                {
                    int index = buttonID - 300;
                    if (index < m_EditingQuest.Rewards.Count)
                    {
                        m_EditingQuest.Rewards.RemoveAt(index);
                        m_From.SendMessage("Reward removed.");
                    }
                    m_From.SendGump(new VystiaQuestEditorGump(m_From, m_Page, m_EditingQuest, 3));
                    return;
                }
                // Waypoint quick add buttons (500-502)
                else if (buttonID == 500) // Add Origin
                {
                    var wp = new QuestWaypoint("Quest Start", Server.Custom.VystiaClasses.Quests.WaypointType.Origin);
                    wp.Description = "Talk to this NPC to begin the quest.";
                    m_EditingQuest.AddWaypoint(wp);
                    m_From.SendMessage("Origin waypoint added.");
                    m_From.SendGump(new VystiaQuestEditorGump(m_From, m_Page, m_EditingQuest, 4));
                    return;
                }
                else if (buttonID == 501) // Add Waypoint
                {
                    var wp = new QuestWaypoint("New Waypoint", Server.Custom.VystiaClasses.Quests.WaypointType.Waypoint);
                    wp.Description = "Complete this objective.";
                    m_EditingQuest.AddWaypoint(wp);
                    m_From.SendMessage("Waypoint added.");
                    m_From.SendGump(new VystiaQuestEditorGump(m_From, m_Page, m_EditingQuest, 4));
                    return;
                }
                else if (buttonID == 502) // Add Finish
                {
                    var wp = new QuestWaypoint("Quest Complete", Server.Custom.VystiaClasses.Quests.WaypointType.NPCCompletion);
                    wp.Description = "Return to the NPC to complete the quest.";
                    m_EditingQuest.AddWaypoint(wp);
                    m_From.SendMessage("Completion waypoint added.");
                    m_From.SendGump(new VystiaQuestEditorGump(m_From, m_Page, m_EditingQuest, 4));
                    return;
                }
                // Waypoint actions (600+)
                else if (buttonID >= 600 && buttonID < 700)
                {
                    int waypointId = (buttonID - 600) / 10;
                    int action = (buttonID - 600) % 10;

                    if (action == 1) // Edit waypoint
                    {
                        m_From.SendGump(new VystiaQuestEditorGump(m_From, m_Page, m_EditingQuest, 5, waypointId));
                        return;
                    }
                    else if (action == 2) // Move up
                    {
                        m_EditingQuest.MoveWaypointUp(waypointId);
                        m_From.SendMessage("Waypoint moved up.");
                    }
                    else if (action == 3) // Move down
                    {
                        m_EditingQuest.MoveWaypointDown(waypointId);
                        m_From.SendMessage("Waypoint moved down.");
                    }
                    else if (action == 4) // Delete
                    {
                        m_EditingQuest.RemoveWaypoint(waypointId);
                        m_From.SendMessage("Waypoint deleted.");
                    }
                    m_From.SendGump(new VystiaQuestEditorGump(m_From, m_Page, m_EditingQuest, 4));
                    return;
                }
                // Single waypoint type selection (700-703)
                else if (buttonID >= 700 && buttonID <= 703)
                {
                    var waypoint = m_EditingQuest.GetWaypoint(m_SelectedWaypointID);
                    if (waypoint != null)
                    {
                        waypoint.Type = (Server.Custom.VystiaClasses.Quests.WaypointType)(buttonID - 700);
                        m_From.SendMessage($"Waypoint type set to {waypoint.Type}.");
                    }
                    m_From.SendGump(new VystiaQuestEditorGump(m_From, m_Page, m_EditingQuest, 5, m_SelectedWaypointID));
                    return;
                }
                // Single waypoint actions (710-720)
                else if (buttonID == 710) // Set location
                {
                    var waypoint = m_EditingQuest.GetWaypoint(m_SelectedWaypointID);
                    if (waypoint != null)
                    {
                        waypoint.Location = m_From.Location;
                        waypoint.Map = m_From.Map;
                        m_From.SendMessage($"Location set to {m_From.Location}.");
                    }
                    m_From.SendGump(new VystiaQuestEditorGump(m_From, m_Page, m_EditingQuest, 5, m_SelectedWaypointID));
                    return;
                }
                else if (buttonID == 711) // Link NPC
                {
                    m_From.SendMessage("Target an NPC to link to this waypoint.");
                    m_From.Target = new LinkNPCTarget(m_EditingQuest, m_SelectedWaypointID, m_From, m_Page);
                    return;
                }
                else if (buttonID == 712) // Unlink NPC
                {
                    var waypoint = m_EditingQuest.GetWaypoint(m_SelectedWaypointID);
                    if (waypoint != null)
                    {
                        waypoint.AssignedNPCSerial = -1;
                        m_From.SendMessage("NPC unlinked.");
                    }
                    m_From.SendGump(new VystiaQuestEditorGump(m_From, m_Page, m_EditingQuest, 5, m_SelectedWaypointID));
                    return;
                }
                else if (buttonID == 720) // Save waypoint
                {
                    var waypoint = m_EditingQuest.GetWaypoint(m_SelectedWaypointID);
                    if (waypoint != null)
                    {
                        waypoint.Name = GetString(info, 50) ?? waypoint.Name;
                        waypoint.Description = GetString(info, 51) ?? waypoint.Description;
                        int conditionId = GetInt(info, 52, (int)waypoint.Condition);
                        waypoint.Condition = (WaypointCondition)conditionId;
                        waypoint.NPCDialogueContext = GetString(info, 53);
                        waypoint.ObjectiveKey = GetString(info, 54);
                        waypoint.RequiredAmount = GetInt(info, 55, waypoint.RequiredAmount);
                        m_From.SendMessage("Waypoint saved.");
                    }
                    m_From.SendGump(new VystiaQuestEditorGump(m_From, m_Page, m_EditingQuest, 4));
                    return;
                }
            }

            m_From.SendGump(new VystiaQuestEditorGump(m_From, m_Page, m_EditingQuest, m_EditMode));
        }

        private string GetString(RelayInfo info, int id)
        {
            TextRelay relay = info.GetTextEntry(id);
            return relay?.Text;
        }

        private int GetInt(RelayInfo info, int id, int defaultValue)
        {
            string text = GetString(info, id);
            if (int.TryParse(text, out int result))
                return result;
            return defaultValue;
        }

        private double GetDouble(RelayInfo info, int id, double defaultValue)
        {
            string text = GetString(info, id);
            if (double.TryParse(text, out double result))
                return result;
            return defaultValue;
        }

        private string Color(string text, int color) => $"<BASEFONT COLOR=#{color:X6}>{text}</BASEFONT>";
        private string Center(string text) => $"<CENTER>{text}</CENTER>";
        private string Truncate(string text, int maxLength) => text.Length > maxLength ? text.Substring(0, maxLength) + "..." : text;

        public static void Initialize()
        {
            // Retired: legacy editor is no longer registered to any command.
            // Quest creation/editing is now done via the step-by-step wizard (VystiaQuestWizardGump).
        }

        [Usage("QuestEditorClassic")]
        [Aliases("QEClassic")]
        [Description("Retired. Use [QE / [QuestWizard instead.")]
        private static void QuestEditor_OnCommand(CommandEventArgs e)
        {
            e.Mobile.SendGump(new VystiaQuestEditorGump(e.Mobile));
        }
    }

    public class GiveQuestTarget : Server.Targeting.Target
    {
        private DynamicQuest m_Quest;

        public GiveQuestTarget(DynamicQuest quest) : base(12, false, Server.Targeting.TargetFlags.None)
        {
            m_Quest = quest;
        }

        protected override void OnTarget(Mobile from, object targeted)
        {
            if (targeted is PlayerMobile pm)
            {
                if (VystiaQuestSystem.StartQuest(pm, m_Quest.QuestID))
                    from.SendMessage($"Quest '{m_Quest.Title}' given to {pm.Name}.");
                else
                    from.SendMessage("Failed to give quest.");
            }
            else
            {
                from.SendMessage("That is not a player.");
            }
        }
    }

    public class LinkNPCTarget : Server.Targeting.Target
    {
        private DynamicQuest m_Quest;
        private int m_WaypointID;
        private Mobile m_From;
        private int m_Page;

        public LinkNPCTarget(DynamicQuest quest, int waypointId, Mobile from, int page)
            : base(12, false, Server.Targeting.TargetFlags.None)
        {
            m_Quest = quest;
            m_WaypointID = waypointId;
            m_From = from;
            m_Page = page;
        }

        protected override void OnTarget(Mobile from, object targeted)
        {
            if (targeted is Mobile npc && !(targeted is PlayerMobile))
            {
                var waypoint = m_Quest.GetWaypoint(m_WaypointID);
                if (waypoint != null)
                {
                    waypoint.AssignedNPCSerial = npc.Serial;
                    waypoint.Location = npc.Location;
                    waypoint.Map = npc.Map;
                    from.SendMessage($"NPC '{npc.Name}' linked to waypoint '{waypoint.Name}'.");
                }
                from.SendGump(new VystiaQuestEditorGump(from, m_Page, m_Quest, 5, m_WaypointID));
            }
            else
            {
                from.SendMessage("That is not a valid NPC.");
                from.SendGump(new VystiaQuestEditorGump(from, m_Page, m_Quest, 5, m_WaypointID));
            }
        }

        protected override void OnTargetCancel(Mobile from, Server.Targeting.TargetCancelType cancelType)
        {
            from.SendGump(new VystiaQuestEditorGump(from, m_Page, m_Quest, 5, m_WaypointID));
        }
    }
}
