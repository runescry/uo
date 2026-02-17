using System;
using System.Collections.Generic;
using System.Linq;
using Server;
using Server.Commands;
using Server.Gumps;
using Server.Mobiles;
using Server.Network;
using Server.Custom.VystiaClasses.Quests;
using Server.Services.LLM;

// ServUO has built-in types under the Server.* namespace that can conflict with ours.
// Use distinct alias names to avoid binding to Server.WaypointType while inside Server.* namespaces.
using VystiaWaypointType = Server.Custom.VystiaClasses.Quests.WaypointType;
using VystiaWaypointCondition = Server.Custom.VystiaClasses.Quests.WaypointCondition;

namespace Server.Custom.VystiaClasses.Gumps
{
    /// <summary>
    /// Step-by-step quest creation wizard (GM-friendly).
    /// Flow:
    ///  Step 0: Select or Create Quest
    ///  Step 1: Quest Details
    ///  Step 2: Origin Waypoint
    ///  Step 3: Add Intermediate Waypoints (repeat)
    ///  Step 4: Completion Waypoint
    ///  Step 5: Objectives (optional)
    ///  Step 6: Rewards (optional)
    ///  Step 7: Spawn NPCs (optional)
    /// </summary>
    public class VystiaQuestWizardGump : Gump
    {
        public enum WizardStep
        {
            SelectOrCreate = 0,
            Details = 1,
            Origin = 2,
            Intermediate = 3,
            Completion = 4,
            Objectives = 5,
            Rewards = 6,
            SpawnNPCs = 7
        }

        private readonly Mobile m_From;
        private readonly WizardStep m_Step;
        private readonly int m_SelectedQuestID;
        private readonly int m_CurrentWaypointID;
        private readonly bool m_IntermediateContinue; // when true, stay in intermediate loop after adding
        private readonly WizardStep m_AfterSelectStep; // where step 0's Next should go

        // NPC spawn (Step 7) state (kept inside wizard so we never detour to another gump)
        private readonly string m_NPCName;
        private readonly string m_NPCTitle;
        private readonly NPCPersonalities.PersonalityType m_NPCPersonality;
        private readonly NPCPersonalities.SpeechPattern m_NPCSpeech;

        // Colors
        private const int TitleColor = 0xFFFFFF;
        private const int LabelColor = 0xAAAAAA;
        private const int ValueColor = 0x88FF88;
        private const int ErrorColor = 0xFF4444;
        private const int SelectedColor = 0x44FF44;
        private const int WarningColor = 0xFFAA00;

        // Layout (1.5x larger for readability)
        private const int GumpWidth = 930;
        private const int GumpHeight = 780;
        private const int Margin = 20;
        private const int InnerWidth = GumpWidth - (Margin * 2); // 890
        private const int FooterY = GumpHeight - 45; // matches prior 520-45
        private const int RightButtonX = GumpWidth - 110;
        private const int RightButtonLabelX = GumpWidth - 75;

        public VystiaQuestWizardGump(
            Mobile from,
            WizardStep step = WizardStep.SelectOrCreate,
            int selectedQuestID = 0,
            int currentWaypointID = 0,
            bool intermediateContinue = false,
            string npcName = null,
            string npcTitle = null,
            NPCPersonalities.PersonalityType npcPersonality = NPCPersonalities.PersonalityType.Sage,
            NPCPersonalities.SpeechPattern npcSpeech = NPCPersonalities.SpeechPattern.Formal,
            WizardStep afterSelectStep = WizardStep.Details)
            : base(50, 50)
        {
            m_From = from;
            m_Step = step;
            m_SelectedQuestID = selectedQuestID;
            m_CurrentWaypointID = currentWaypointID;
            m_IntermediateContinue = intermediateContinue;
            m_NPCName = npcName ?? "";
            m_NPCTitle = npcTitle ?? "";
            m_NPCPersonality = npcPersonality;
            m_NPCSpeech = npcSpeech;
            m_AfterSelectStep = afterSelectStep;

            Closable = true;
            Disposable = true;
            Dragable = true;
            Resizable = false;

            Build();
        }

        private VystiaQuestWizardGump Rebuild(WizardStep step, int questId, int waypointId = 0, bool intermediateContinue = false)
        {
            return new VystiaQuestWizardGump(
                m_From,
                step,
                questId,
                waypointId,
                intermediateContinue,
                m_NPCName,
                m_NPCTitle,
                m_NPCPersonality,
                m_NPCSpeech,
                m_AfterSelectStep);
        }

        private void OpenTypePicker(DynamicQuest quest, QuestWaypoint waypoint)
        {
            if (quest == null || waypoint == null)
                return;

            m_From.SendGump(new VystiaQuestWizardPickerGump(
                m_From,
                VystiaQuestWizardPickerGump.PickerKind.WaypointType,
                quest.QuestID,
                waypoint.WaypointID,
                m_Step,
                m_NPCName,
                m_NPCTitle,
                m_NPCPersonality,
                m_NPCSpeech,
                m_AfterSelectStep));
        }

        private void OpenConditionPicker(DynamicQuest quest, QuestWaypoint waypoint)
        {
            if (quest == null || waypoint == null)
                return;

            // Constrain options by context to keep it intuitive:
            // - Origin: Talk / Reach Location
            // - Completion: Talk / Defeat
            // - Intermediate: All
            int filter = 0;
            if (m_Step == WizardStep.Origin)
                filter = 1;
            else if (m_Step == WizardStep.Completion)
                filter = 2;

            m_From.SendGump(new VystiaQuestWizardPickerGump(
                m_From,
                VystiaQuestWizardPickerGump.PickerKind.WaypointCondition,
                quest.QuestID,
                waypoint.WaypointID,
                m_Step,
                m_NPCName,
                m_NPCTitle,
                m_NPCPersonality,
                m_NPCSpeech,
                m_AfterSelectStep,
                filter));
        }

        private void OpenSidekickArchetypePicker(DynamicQuest quest, QuestWaypoint waypoint)
        {
            if (quest == null || waypoint == null)
                return;

            m_From.SendGump(new VystiaQuestWizardPickerGump(
                m_From,
                VystiaQuestWizardPickerGump.PickerKind.SidekickArchetype,
                quest.QuestID,
                waypoint.WaypointID,
                m_Step,
                m_NPCName,
                m_NPCTitle,
                m_NPCPersonality,
                m_NPCSpeech,
                m_AfterSelectStep));
        }

        private void Build()
        {
            AddPage(0);

            AddBackground(0, 0, GumpWidth, GumpHeight, 9270);
            AddAlphaRegion(10, 10, GumpWidth - 20, GumpHeight - 20);

            AddHtml(Margin, 18, InnerWidth, 25, Center(Color($"Vystia Quest Wizard - Step {(int)m_Step + 1} of 8", TitleColor)), false, false);
            DrawProgressBar();

            int y = 70;

            switch (m_Step)
            {
                case WizardStep.SelectOrCreate:
                    BuildStep0_SelectOrCreate(ref y);
                    break;
                case WizardStep.Details:
                    BuildStep1_Details(ref y);
                    break;
                case WizardStep.Origin:
                    BuildStep2_Origin(ref y);
                    break;
                case WizardStep.Intermediate:
                    BuildStep3_Intermediate(ref y);
                    break;
                case WizardStep.Completion:
                    BuildStep4_Completion(ref y);
                    break;
                case WizardStep.Objectives:
                    BuildStep5_Objectives(ref y);
                    break;
                case WizardStep.Rewards:
                    BuildStep6_Rewards(ref y);
                    break;
                case WizardStep.SpawnNPCs:
                    BuildStep7_SpawnNPCs(ref y);
                    break;
            }
        }

        private void DrawProgressBar()
        {
            int barY = 45;
            int barX = Margin;
            int barWidth = InnerWidth;
            int steps = 8;
            int stepWidth = barWidth / steps;

            for (int i = 0; i < steps; i++)
            {
                int x = barX + (i * stepWidth);
                int color = i <= (int)m_Step ? SelectedColor : 0x444444;
                AddHtml(x, barY, stepWidth - 2, 18, Color(GetStepLabel(i), color), false, false);
            }
        }

        private string GetStepLabel(int i)
        {
            switch ((WizardStep)i)
            {
                case WizardStep.SelectOrCreate: return "Quest";
                case WizardStep.Details: return "Details";
                case WizardStep.Origin: return "Origin";
                case WizardStep.Intermediate: return "Waypoints";
                case WizardStep.Completion: return "Finish";
                case WizardStep.Objectives: return "Objectives";
                case WizardStep.Rewards: return "Rewards";
                case WizardStep.SpawnNPCs: return "NPCs";
                default: return "";
            }
        }

        private DynamicQuest GetSelectedQuest()
        {
            if (m_SelectedQuestID <= 0)
                return null;

            return DynamicQuestManager.GetQuest(m_SelectedQuestID);
        }

        #region Step 0: Select/Create

        private void BuildStep0_SelectOrCreate(ref int y)
        {
            AddHtml(Margin, y, InnerWidth, 20, Color("Select an existing quest, or create a new one:", LabelColor), false, false);
            y += 28;

            AddButton(Margin, y, 4005, 4007, 50, GumpButtonType.Reply, 0);
            AddHtml(Margin + 35, y, 300, 20, Color("Create New Quest", ValueColor), false, false);

            y += 35;
            AddImage(Margin, y - 5, 0x39);
            y += 10;

            var quests = DynamicQuestManager.GetAllDynamicQuests();
            if (quests.Count == 0)
            {
                AddHtml(Margin, y, InnerWidth, 40, Color("No dynamic quests exist yet. Click 'Create New Quest' to start.", WarningColor), false, false);
                return;
            }

            AddHtml(Margin, y, InnerWidth, 18, Color("Existing quests:", LabelColor), false, false);
            y += 22;

            int buttonID = 100;
            foreach (var quest in quests.OrderBy(q => q.QuestID).Take(22))
            {
                bool selected = quest.QuestID == m_SelectedQuestID;
                AddButton(Margin, y, selected ? 4006 : 4005, 4007, buttonID, GumpButtonType.Reply, 0);
                AddHtml(Margin + 35, y, InnerWidth - 50, 20, Color($"[{quest.QuestID}] {quest.Title}", selected ? SelectedColor : LabelColor), false, false);
                y += 24;
                buttonID++;
            }

            if (m_SelectedQuestID > 0)
            {
                // Delete selected quest (with confirm)
                AddButton(RightButtonX - 170, FooterY, 4017, 4019, 60, GumpButtonType.Reply, 0);
                AddHtml(RightButtonX - 135, FooterY, 140, 20, Color("Delete Quest", WarningColor), false, false);

                AddButton(RightButtonX, FooterY, 4005, 4007, 1, GumpButtonType.Reply, 0);
                AddHtml(RightButtonLabelX, FooterY, 80, 20, Color("Next", ValueColor), false, false);
            }
        }

        #endregion

        #region Step 1: Details

        private void BuildStep1_Details(ref int y)
        {
            var quest = GetSelectedQuest();
            if (quest == null)
            {
                AddHtml(Margin, y, InnerWidth, 20, Color("Quest not found. Please go back.", ErrorColor), false, false);
                AddBackButton();
                return;
            }

            AddHtml(Margin, y, InnerWidth, 20, Color($"Editing quest: [{quest.QuestID}] {quest.Title}", ValueColor), false, false);
            y += 30;

            AddHtml(Margin, y, 120, 20, Color("Title:", LabelColor), false, false);
            AddBackground(140, y - 2, InnerWidth - 140, 24, 9350);
            AddTextEntry(145, y, InnerWidth - 150, 20, TitleColor, 0, quest.Title ?? "");
            y += 32;

            AddHtml(Margin, y, 120, 20, Color("Description:", LabelColor), false, false);
            y += 22;
            AddBackground(Margin, y, InnerWidth, 90, 9350);
            AddTextEntry(Margin + 5, y + 5, InnerWidth - 10, 80, TitleColor, 1, quest.Description ?? "");
            y += 105;

            AddHtml(Margin, y, 120, 20, Color("Class ID:", LabelColor), false, false);
            AddBackground(140, y - 2, 80, 24, 9350);
            AddTextEntry(145, y, 70, 20, TitleColor, 2, ((int)quest.RequiredClass).ToString());
            AddHtml(230, y, InnerWidth - 230, 20, Color("(0=None, 1-26=Class ID)", LabelColor), false, false);
            y += 32;

            AddHtml(Margin, y, 120, 20, Color("Tier:", LabelColor), false, false);
            AddBackground(140, y - 2, 80, 24, 9350);
            AddTextEntry(145, y, 70, 20, TitleColor, 3, ((int)quest.Tier).ToString());
            AddHtml(230, y, InnerWidth - 230, 20, Color("(0=Init, 1=Appr, 2=Jour, 3=Master)", LabelColor), false, false);
            y += 32;

            AddHtml(Margin, y, 120, 20, Color("Prereq ID:", LabelColor), false, false);
            AddBackground(140, y - 2, 80, 24, 9350);
            AddTextEntry(145, y, 70, 20, TitleColor, 4, quest.PrerequisiteQuestID.ToString());
            AddHtml(230, y, InnerWidth - 230, 20, Color("(0=None, or Quest ID)", LabelColor), false, false);

            AddBackButton();
            AddNextButton();
        }

        #endregion

        #region Waypoint Steps (Origin / Intermediate / Completion)

        private void BuildWaypointEditor(ref int y, DynamicQuest quest, QuestWaypoint waypoint, bool showSpawnNpc)
        {
            AddHtml(Margin, y, InnerWidth, 20, Color($"Quest: [{quest.QuestID}] {quest.Title}", ValueColor), false, false);
            y += 22;

            AddHtml(Margin, y, InnerWidth, 20, Color($"Waypoint: #{waypoint.OrderIndex} (ID {waypoint.WaypointID})", LabelColor), false, false);
            y += 28;

            AddHtml(Margin, y, 120, 20, Color("Name:", LabelColor), false, false);
            AddBackground(140, y - 2, InnerWidth - 140, 24, 9350);
            AddTextEntry(145, y, InnerWidth - 150, 20, TitleColor, 50, waypoint.Name ?? "");
            y += 32;

            AddHtml(Margin, y, 120, 20, Color("Description:", LabelColor), false, false);
            AddBackground(140, y - 2, InnerWidth - 140, 24, 9350);
            AddTextEntry(145, y, InnerWidth - 150, 20, TitleColor, 51, waypoint.Description ?? "");
            y += 35;

            // TYPE (automatic based on wizard step; displayed only)
            AddHtml(Margin, y, 120, 20, Color("Type:", LabelColor), false, false);
            AddHtml(140, y, InnerWidth - 140, 20, Color(waypoint.Type.ToString(), ValueColor), false, false);
            y += 26;

            // CONDITION (picker, but constrained by wizard context)
            AddHtml(Margin, y, 120, 20, Color("How to complete:", LabelColor), false, false);
            AddHtml(140, y, 250, 20, Color(waypoint.Condition.ToString(), ValueColor), false, false);
            AddButton(GumpWidth - 250, y, 4005, 4007, 7060, GumpButtonType.Reply, 0);
            AddHtml(GumpWidth - 215, y, 180, 20, Color("Pick completion...", ValueColor), false, false);
            y += 30;

            // Condition-specific fields (only show what matters)
            if (waypoint.Condition == VystiaWaypointCondition.ReachLocation)
            {
                AddHtml(Margin, y, 120, 20, Color("Location:", LabelColor), false, false);
                string loc = waypoint.Location != Point3D.Zero ? $"{waypoint.Location.X},{waypoint.Location.Y},{waypoint.Location.Z} ({waypoint.Map})" : "Not set";
                AddHtml(140, y, InnerWidth - 330, 20, Color(loc, ValueColor), false, false);
                AddButton(GumpWidth - 250, y, 4005, 4007, 710, GumpButtonType.Reply, 0);
                AddHtml(GumpWidth - 215, y, 140, 20, Color("Use my loc", ValueColor), false, false);
                y += 30;

                AddHtml(Margin, y, 120, 20, Color("Radius:", LabelColor), false, false);
                AddBackground(140, y - 2, 60, 24, 9350);
                AddTextEntry(145, y, 50, 20, TitleColor, 56, waypoint.Radius.ToString());
                AddHtml(210, y, InnerWidth - 210, 20, Color("How close the player must get (tiles).", LabelColor), false, false);
                y += 32;
            }
            else if (waypoint.Condition == VystiaWaypointCondition.DefeatBoss)
            {
                AddHtml(Margin, y, 120, 20, Color("Creature Type:", LabelColor), false, false);
                AddBackground(140, y - 2, 260, 24, 9350);
                AddTextEntry(145, y, 250, 20, TitleColor, 57, waypoint.TargetTypeName ?? "");
                AddHtml(410, y, InnerWidth - 410, 20, Color("Script class name, e.g. FrostGiant (not 'creature').", LabelColor), false, false);
                y += 30;

                AddHtml(Margin, y, 120, 20, Color("Amount:", LabelColor), false, false);
                AddBackground(140, y - 2, 60, 24, 9350);
                AddTextEntry(145, y, 50, 20, TitleColor, 55, waypoint.RequiredAmount.ToString());
                AddHtml(210, y, InnerWidth - 210, 20, Color("How many kills are required.", LabelColor), false, false);
                y += 32;
            }
            else if (waypoint.Condition == VystiaWaypointCondition.CollectItems)
            {
                AddHtml(Margin, y, 120, 20, Color("Item Type:", LabelColor), false, false);
                AddBackground(140, y - 2, 260, 24, 9350);
                AddTextEntry(145, y, 250, 20, TitleColor, 57, waypoint.TargetTypeName ?? "");
                AddHtml(410, y, InnerWidth - 410, 20, Color("Script class name, e.g. Frostbloom.", LabelColor), false, false);
                y += 30;

                AddHtml(Margin, y, 120, 20, Color("Amount:", LabelColor), false, false);
                AddBackground(140, y - 2, 60, 24, 9350);
                AddTextEntry(145, y, 50, 20, TitleColor, 55, waypoint.RequiredAmount.ToString());
                AddHtml(210, y, InnerWidth - 210, 20, Color("How many items are required.", LabelColor), false, false);
                y += 32;
            }
            else if (waypoint.Condition == VystiaWaypointCondition.CastSpell)
            {
                AddHtml(Margin, y, 120, 20, Color("Spell:", LabelColor), false, false);
                AddBackground(140, y - 2, 260, 24, 9350);
                AddTextEntry(145, y, 250, 20, TitleColor, 57, waypoint.TargetTypeName ?? "");
                AddHtml(410, y, InnerWidth - 410, 20, Color("Spell name or identifier (implementation-dependent).", LabelColor), false, false);
                y += 30;

                AddHtml(Margin, y, 120, 20, Color("Amount:", LabelColor), false, false);
                AddBackground(140, y - 2, 60, 24, 9350);
                AddTextEntry(145, y, 50, 20, TitleColor, 55, waypoint.RequiredAmount.ToString());
                AddHtml(210, y, InnerWidth - 210, 20, Color("How many casts are required.", LabelColor), false, false);
                y += 32;
            }
            else if (waypoint.Condition == VystiaWaypointCondition.Custom)
            {
                AddHtml(Margin, y, 120, 20, Color("Objective Key:", LabelColor), false, false);
                AddBackground(140, y - 2, 260, 24, 9350);
                AddTextEntry(145, y, 250, 20, TitleColor, 54, waypoint.ObjectiveKey ?? "");
                AddHtml(410, y, InnerWidth - 410, 20, Color("A unique key you will update via scripts, e.g. custom_ice_ritual.", LabelColor), false, false);
                y += 30;

                AddHtml(Margin, y, 120, 20, Color("Amount:", LabelColor), false, false);
                AddBackground(140, y - 2, 60, 24, 9350);
                AddTextEntry(145, y, 50, 20, TitleColor, 55, waypoint.RequiredAmount.ToString());
                AddHtml(210, y, InnerWidth - 210, 20, Color("How much progress is required for this key.", LabelColor), false, false);
                y += 32;
            }
            else if (waypoint.Condition == VystiaWaypointCondition.RecruitSidekick)
            {
                AddHtml(Margin, y, 120, 20, Color("Archetype:", LabelColor), false, false);
                AddBackground(140, y - 2, 200, 24, 9350);
                AddTextEntry(145, y, 190, 20, TitleColor, 57, waypoint.TargetTypeName ?? "");
                AddButton(GumpWidth - 250, y, 4005, 4007, 7070, GumpButtonType.Reply, 0);
                AddHtml(GumpWidth - 215, y, 180, 20, Color("Pick Archetype", ValueColor), false, false);
                y += 30;

                AddHtml(140, y, InnerWidth - 140, 20, Color("Leave blank to accept ANY recruited sidekick.", LabelColor), false, false);
                y += 26;
            }
            else
            {
                // TalkToNPC: no extra fields needed (NPC link handled separately)
                AddHtml(Margin, y, InnerWidth, 20, Color("This step completes when the player talks to the linked NPC.", LabelColor), false, false);
                y += 26;
            }

            AddHtml(Margin, y, 120, 20, Color("LLM Context:", LabelColor), false, false);
            y += 22;
            AddBackground(Margin, y, InnerWidth, 70, 9350);
            AddTextEntry(Margin + 5, y + 5, InnerWidth - 10, 60, TitleColor, 53, waypoint.NPCDialogueContext ?? "");
            y += 85;

            AddHtml(Margin, y, 140, 20, Color("Player Hint:", LabelColor), false, false);
            AddHtml(Margin + 140, y, InnerWidth - 140, 20, Color("(GM note: e.g. \"near the entrance of Destard\", \"on the icy island\")", LabelColor), false, false);
            y += 22;
            AddBackground(Margin, y, InnerWidth, 50, 9350);
            AddTextEntry(Margin + 5, y + 5, InnerWidth - 10, 40, TitleColor, 58, waypoint.PlayerLocationHint ?? "");
            y += 65;

            // NPC link status + spawn option
            string npcStatus = waypoint.AssignedNPCSerial.Value != -1 ? $"Linked ({waypoint.AssignedNPCSerial.Value})" : "Not linked";
            AddHtml(Margin, y, InnerWidth, 20, Color($"NPC: {npcStatus}", waypoint.AssignedNPCSerial.Value != -1 ? ValueColor : WarningColor), false, false);

            if (showSpawnNpc && waypoint.Condition == VystiaWaypointCondition.TalkToNPC)
            {
                AddButton(410, y, 4005, 4007, 900, GumpButtonType.Reply, 0);
                AddHtml(445, y, 160, 20, Color("Spawn NPC for this", ValueColor), false, false);
            }
        }

        // Type/Condition are selected via VystiaQuestWizardPickerGump.

        private void BuildStep2_Origin(ref int y)
        {
            var quest = GetSelectedQuest();
            if (quest == null)
            {
                AddHtml(20, y, 580, 20, Color("Quest not found. Please go back.", ErrorColor), false, false);
                AddBackButton();
                return;
            }

            // Ensure origin exists
            var origin = quest.GetOrigin();
            if (origin == null)
            {
                origin = new QuestWaypoint("Quest Start", VystiaWaypointType.Origin);
                origin.Description = "Talk to this NPC to begin the quest.";
                origin.Location = m_From.Location;
                origin.Map = m_From.Map;
                quest.AddWaypoint(origin);
                DynamicQuestManager.Save();
            }
            else
            {
                // Origin step always stays Origin
                origin.Type = VystiaWaypointType.Origin;
            }

            AddHtml(20, y, 580, 20, Color("Define the Origin (where players begin):", LabelColor), false, false);
            y += 26;

            BuildWaypointEditor(ref y, quest, origin, true);

            AddBackButton();
            AddNextButton();
        }

        private void BuildStep3_Intermediate(ref int y)
        {
            var quest = GetSelectedQuest();
            if (quest == null)
            {
                AddHtml(20, y, 580, 20, Color("Quest not found. Please go back.", ErrorColor), false, false);
                AddBackButton();
                return;
            }

            AddHtml(20, y, 580, 20, Color("Add intermediate waypoints (optional).", LabelColor), false, false);
            y += 26;

            // Show current list
            AddHtml(20, y, 580, 18, Color("Current chain:", LabelColor), false, false);
            y += 20;
            foreach (var wp in quest.Waypoints.OrderBy(w => w.OrderIndex))
            {
                AddHtml(30, y, 580, 18, Color($"- #{wp.OrderIndex}: {wp.Type} - {wp.Name}", 0xCCCCCC), false, false);
                y += 18;
                if (y > 155) break;
            }
            y += 10;
            AddImage(20, y - 5, 0x39);
            y += 10;

            QuestWaypoint editing = null;
            if (m_CurrentWaypointID > 0)
                editing = quest.GetWaypoint(m_CurrentWaypointID);

            if (editing == null || editing.Type != VystiaWaypointType.Waypoint)
            {
                editing = new QuestWaypoint("New Waypoint", VystiaWaypointType.Waypoint);
                editing.Description = "Complete this step.";
                editing.Location = m_From.Location;
                editing.Map = m_From.Map;
                quest.AddWaypoint(editing);
                DynamicQuestManager.Save();
            }

            AddHtml(20, y, 580, 20, Color("Configure this waypoint:", LabelColor), false, false);
            y += 26;

            BuildWaypointEditor(ref y, quest, editing, true);

            // Replace Next/Back with: Back + "Add Another" + "Continue"
            AddButton(20, 475, 4014, 4016, 2, GumpButtonType.Reply, 0);
            AddHtml(55, 475, 60, 20, Color("Back", LabelColor), false, false);

            AddButton(330, 475, 4005, 4007, 910, GumpButtonType.Reply, 0);
            AddHtml(365, 475, 120, 20, Color("Add Another", ValueColor), false, false);

            AddButton(520, 475, 4005, 4007, 1, GumpButtonType.Reply, 0);
            AddHtml(555, 475, 60, 20, Color("Next", ValueColor), false, false);
        }

        private void BuildStep4_Completion(ref int y)
        {
            var quest = GetSelectedQuest();
            if (quest == null)
            {
                AddHtml(20, y, 580, 20, Color("Quest not found. Please go back.", ErrorColor), false, false);
                AddBackButton();
                return;
            }

            // Ensure completion exists
            var completion = quest.GetCompletion();
            if (completion == null)
            {
                completion = new QuestWaypoint("Quest Complete", VystiaWaypointType.NPCCompletion);
                completion.Description = "Return to the final NPC to complete the quest.";
                completion.Location = m_From.Location;
                completion.Map = m_From.Map;
                quest.AddWaypoint(completion);
                DynamicQuestManager.Save();
            }
            else
            {
                // Completion type should align with condition for clarity.
                completion.Type = completion.Condition == VystiaWaypointCondition.DefeatBoss
                    ? VystiaWaypointType.BossCompletion
                    : VystiaWaypointType.NPCCompletion;
            }

            AddHtml(20, y, 580, 20, Color("Define the Completion (final step):", LabelColor), false, false);
            y += 26;

            BuildWaypointEditor(ref y, quest, completion, true);

            AddBackButton();
            AddNextButton();
        }

        #endregion

        #region Step 5: Objectives

        private void BuildStep5_Objectives(ref int y)
        {
            var quest = GetSelectedQuest();
            if (quest == null)
            {
                AddHtml(20, y, 580, 20, Color("Quest not found. Please go back.", ErrorColor), false, false);
                AddBackButton();
                return;
            }

            AddHtml(20, y, 580, 20, Color("Add objectives (optional). Waypoints are tracked automatically.", LabelColor), false, false);
            y += 28;

            AddHtml(20, y, 580, 18, Color("Current objectives:", LabelColor), false, false);
            y += 22;

            int idx = 0;
            foreach (var obj in quest.ObjectivesList)
            {
                AddHtml(30, y, 470, 18, Color($"{obj.Key}: {obj.DisplayName} x{obj.Amount}", 0xCCCCCC), false, false);
                AddButton(520, y, 4017, 4019, 200 + idx, GumpButtonType.Reply, 0);
                y += 22;
                idx++;
                if (y > 235) break;
            }

            y += 10;
            AddImage(20, y - 5, 0x39);
            y += 10;

            AddHtml(20, y, 580, 18, Color("Add new objective:", LabelColor), false, false);
            y += 25;

            AddHtml(20, y, 80, 20, Color("Key:", LabelColor), false, false);
            AddBackground(100, y - 2, 180, 24, 9350);
            AddTextEntry(105, y, 170, 20, TitleColor, 10, "kill_creature");
            y += 28;

            AddHtml(20, y, 80, 20, Color("Display:", LabelColor), false, false);
            AddBackground(100, y - 2, 360, 24, 9350);
            AddTextEntry(105, y, 350, 20, TitleColor, 11, "Kill Creatures");
            y += 28;

            AddHtml(20, y, 80, 20, Color("Amount:", LabelColor), false, false);
            AddBackground(100, y - 2, 80, 24, 9350);
            AddTextEntry(105, y, 70, 20, TitleColor, 12, "1");
            y += 35;

            AddButton(100, y, 4005, 4007, 30, GumpButtonType.Reply, 0);
            AddHtml(135, y, 180, 20, Color("Add Objective", ValueColor), false, false);

            AddBackButton();
            AddNextButton();
        }

        #endregion

        #region Step 6: Rewards

        private void BuildStep6_Rewards(ref int y)
        {
            var quest = GetSelectedQuest();
            if (quest == null)
            {
                AddHtml(20, y, 580, 20, Color("Quest not found. Please go back.", ErrorColor), false, false);
                AddBackButton();
                return;
            }

            AddHtml(20, y, 580, 20, Color("Add rewards (optional).", LabelColor), false, false);
            y += 28;

            AddHtml(20, y, 580, 18, Color("Current rewards:", LabelColor), false, false);
            y += 22;

            int idx = 0;
            foreach (var reward in quest.Rewards)
            {
                AddHtml(30, y, 470, 18, Color(reward.GetDescription(), 0xCCCCCC), false, false);
                AddButton(520, y, 4017, 4019, 300 + idx, GumpButtonType.Reply, 0);
                y += 22;
                idx++;
                if (y > 215) break;
            }

            y += 10;
            AddImage(20, y - 5, 0x39);
            y += 10;

            // Add reward controls (same as editor, but compact)
            AddHtml(20, y, 580, 18, Color("Add reward:", LabelColor), false, false);
            y += 25;

            AddButton(20, y, 4005, 4007, 40, GumpButtonType.Reply, 0);
            AddHtml(55, y, 70, 20, Color("Gold:", LabelColor), false, false);
            AddBackground(120, y - 2, 100, 24, 9350);
            AddTextEntry(125, y, 90, 20, TitleColor, 20, "500");
            y += 28;

            AddButton(20, y, 4005, 4007, 41, GumpButtonType.Reply, 0);
            AddHtml(55, y, 70, 20, Color("Skill:", LabelColor), false, false);
            AddBackground(120, y - 2, 140, 24, 9350);
            AddTextEntry(125, y, 130, 20, TitleColor, 21, "Magery");
            AddHtml(265, y, 20, 20, Color("+", LabelColor), false, false);
            AddBackground(285, y - 2, 70, 24, 9350);
            AddTextEntry(290, y, 60, 20, TitleColor, 22, "5.0");
            y += 28;

            AddButton(20, y, 4005, 4007, 42, GumpButtonType.Reply, 0);
            AddHtml(55, y, 70, 20, Color("Item:", LabelColor), false, false);
            AddBackground(120, y - 2, 180, 24, 9350);
            AddTextEntry(125, y, 170, 20, TitleColor, 23, "Frostbloom");
            AddHtml(305, y, 20, 20, Color("x", LabelColor), false, false);
            AddBackground(325, y - 2, 60, 24, 9350);
            AddTextEntry(330, y, 50, 20, TitleColor, 24, "1");
            y += 28;

            AddButton(20, y, 4005, 4007, 43, GumpButtonType.Reply, 0);
            AddHtml(55, y, 70, 20, Color("Title:", LabelColor), false, false);
            AddBackground(120, y - 2, 260, 24, 9350);
            AddTextEntry(125, y, 250, 20, TitleColor, 25, "the Frozen");

            AddBackButton();
            AddNextButton();
        }

        #endregion

        #region Step 7: Spawn NPCs

        private void BuildStep7_SpawnNPCs(ref int y)
        {
            var quest = GetSelectedQuest();
            if (quest == null)
            {
                AddHtml(Margin, y, InnerWidth, 40, Color("No quest selected. Click 'Select Quest' to choose a quest, then link/spawn NPCs.", WarningColor), false, false);
                y += 45;

                AddButton(Margin, y, 4005, 4007, 9800, GumpButtonType.Reply, 0);
                AddHtml(Margin + 35, y, 250, 20, Color("Select Quest", ValueColor), false, false);

                // Also allow Done to just close.
                AddButton(RightButtonX, FooterY, 4005, 4007, 999, GumpButtonType.Reply, 0);
                AddHtml(RightButtonLabelX, FooterY, 80, 20, Color("Done", ValueColor), false, false);
                return;
            }

            AddHtml(Margin, y, InnerWidth, 20, Color("Optional: spawn & link QuestNPCs for TalkToNPC waypoints.", LabelColor), false, false);
            y += 28;

            var talkWaypoints = quest.Waypoints
                .Where(w => w.Condition == VystiaWaypointCondition.TalkToNPC)
                .OrderBy(w => w.OrderIndex)
                .ToList();

            if (talkWaypoints.Count == 0)
            {
                AddHtml(Margin, y, InnerWidth, 40, Color("No TalkToNPC waypoints found. You're done!", ValueColor), false, false);
                AddBackButton();
                AddButton(RightButtonX, FooterY, 4005, 4007, 999, GumpButtonType.Reply, 0);
                AddHtml(RightButtonLabelX, FooterY, 80, 20, Color("Done", ValueColor), false, false);
                return;
            }

            AddHtml(Margin, y, InnerWidth, 18, Color("Select a TalkToNPC waypoint:", LabelColor), false, false);
            y += 22;

            int buttonId = 800;
            foreach (var wp in talkWaypoints.Take(14))
            {
                bool selected = wp.WaypointID == m_CurrentWaypointID;
                string status = wp.AssignedNPCSerial.Value != -1 ? "Linked" : "Missing";
                int statusColor = wp.AssignedNPCSerial.Value != -1 ? ValueColor : WarningColor;

                AddButton(Margin, y, selected ? 4006 : 4005, 4007, buttonId, GumpButtonType.Reply, 0);
                AddHtml(Margin + 35, y, InnerWidth - 170, 18, Color($"#{wp.OrderIndex}: {wp.Name}", selected ? SelectedColor : LabelColor), false, false);
                AddHtml(GumpWidth - 170, y, 150, 18, Color(status, statusColor), false, false);
                y += 22;
                buttonId++;
            }

            // If a waypoint is selected, show inline spawn config (no detour gump)
            if (m_CurrentWaypointID > 0)
            {
                y += 10;
                AddImage(Margin, y - 5, 0x39);
                y += 10;

                var selectedWp = quest.GetWaypoint(m_CurrentWaypointID);
                if (selectedWp != null)
                {
                    AddHtml(Margin, y, InnerWidth, 20, Color($"Spawn NPC for: #{selectedWp.OrderIndex} {selectedWp.Name}", ValueColor), false, false);
                    y += 28;

                    // NPC Name / Title
                    AddHtml(Margin, y, 120, 20, Color("NPC Name:", LabelColor), false, false);
                    AddBackground(140, y - 2, 320, 24, 9350);
                    AddTextEntry(145, y, 310, 20, TitleColor, 2000, string.IsNullOrEmpty(m_NPCName) ? "Quest Giver" : m_NPCName);
                    y += 30;

                    AddHtml(Margin, y, 120, 20, Color("NPC Title:", LabelColor), false, false);
                    AddBackground(140, y - 2, 320, 24, 9350);
                    AddTextEntry(145, y, 310, 20, TitleColor, 2001, string.IsNullOrEmpty(m_NPCTitle) ? "the Quest NPC" : m_NPCTitle);
                    y += 35;

                    // Personality (bigger radio buttons)
                    AddHtml(Margin, y, InnerWidth, 20, Color("Personality:", LabelColor), false, false);
                    y += 22;

                    var personalities = new[]
                    {
                        NPCPersonalities.PersonalityType.Sage,
                        NPCPersonalities.PersonalityType.Warrior,
                        NPCPersonalities.PersonalityType.Mage,
                        NPCPersonalities.PersonalityType.Merchant,
                        NPCPersonalities.PersonalityType.Noble,
                        NPCPersonalities.PersonalityType.Commoner,
                        NPCPersonalities.PersonalityType.Healer,
                        NPCPersonalities.PersonalityType.Guard
                    };

                    int px = Margin;
                    for (int i = 0; i < personalities.Length; i++)
                    {
                        bool sel = (m_NPCPersonality == personalities[i]);
                        AddButton(px, y, sel ? 9723 : 9720, 9723, 9300 + i, GumpButtonType.Reply, 0);
                        AddHtml(px + 30, y + 2, 110, 20, Color(personalities[i].ToString(), sel ? SelectedColor : LabelColor), false, false);
                        px += 220;
                        if (px > (GumpWidth - 250))
                        {
                            px = Margin;
                            y += 28;
                        }
                    }
                    y += 38;

                    // Speech pattern
                    AddHtml(Margin, y, InnerWidth, 20, Color("Speech Pattern:", LabelColor), false, false);
                    y += 22;

                    var patterns = Enum.GetValues(typeof(NPCPersonalities.SpeechPattern)).Cast<NPCPersonalities.SpeechPattern>().ToArray();
                    px = Margin;
                    for (int i = 0; i < patterns.Length; i++)
                    {
                        bool sel = (m_NPCSpeech == patterns[i]);
                        AddButton(px, y, sel ? 9723 : 9720, 9723, 9400 + i, GumpButtonType.Reply, 0);
                        AddHtml(px + 30, y + 2, 110, 20, Color(patterns[i].ToString(), sel ? SelectedColor : LabelColor), false, false);
                        px += 220;
                        if (px > (GumpWidth - 250))
                        {
                            px = Margin;
                            y += 28;
                        }
                    }

                    // Status + spawn
                    y += 45;
                    string linkStatus = selectedWp.AssignedNPCSerial.Value != -1 ? $"Currently linked: {selectedWp.AssignedNPCSerial.Value}" : "Currently not linked";
                    AddHtml(Margin, y, InnerWidth, 20, Color(linkStatus, selectedWp.AssignedNPCSerial.Value != -1 ? ValueColor : WarningColor), false, false);
                    y += 25;

                    AddButton((GumpWidth / 2) - 120, FooterY, 4005, 4007, 9500, GumpButtonType.Reply, 0);
                    AddHtml((GumpWidth / 2) - 85, FooterY, 220, 20, Color("SPAWN & LINK NPC", SelectedColor), false, false);
                }
            }

            // Controls
            AddBackButton();

            // World content: spawn Vystia NPCs (bosses/leaders/key NPCs)
            AddButton(Margin + 170, FooterY, 4005, 4007, 9600, GumpButtonType.Reply, 0);
            AddHtml(Margin + 205, FooterY, 220, 20, Color("Spawn Vystia NPC...", ValueColor), false, false);

            AddButton(RightButtonX, FooterY, 4005, 4007, 999, GumpButtonType.Reply, 0);
            AddHtml(RightButtonLabelX, FooterY, 80, 20, Color("Done", ValueColor), false, false);
        }

        #endregion

        #region Navigation Helpers

        private void AddBackButton()
        {
            AddButton(Margin, FooterY, 4014, 4016, 2, GumpButtonType.Reply, 0);
            AddHtml(Margin + 35, FooterY, 80, 20, Color("Back", LabelColor), false, false);
        }

        private void AddNextButton()
        {
            AddButton(RightButtonX, FooterY, 4005, 4007, 1, GumpButtonType.Reply, 0);
            AddHtml(RightButtonLabelX, FooterY, 80, 20, Color("Next", ValueColor), false, false);
        }

        #endregion

        #region Response

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            if (m_From == null || m_From.Deleted)
                return;

            int buttonID = info.ButtonID;
            if (buttonID == 0)
                return;

            // Step-local helpers
            DynamicQuest quest = GetSelectedQuest();
            QuestWaypoint wp = null;
            if (quest != null && m_CurrentWaypointID > 0)
                wp = quest.GetWaypoint(m_CurrentWaypointID);

            // NPC config (step 7) text entries
            string enteredNpcName = GetString(info, 2000) ?? m_NPCName;
            string enteredNpcTitle = GetString(info, 2001) ?? m_NPCTitle;

            // NOTE: Old radio-button handlers removed. Type/Condition are now chosen via picker gumps (7050/7060).

            // Set to my location for active waypoint editor
            if (buttonID == 710 && quest != null)
            {
                var editWp = ResolveEditableWaypoint(quest);
                if (editWp != null)
                {
                    editWp.Location = m_From.Location;
                    editWp.Map = m_From.Map;
                    DynamicQuestManager.Save();
                    m_From.SendMessage("Waypoint location set to your location.");
                }

                m_From.SendGump(new VystiaQuestWizardGump(m_From, m_Step, m_SelectedQuestID, editWp?.WaypointID ?? m_CurrentWaypointID, m_IntermediateContinue));
                return;
            }

            // Spawn NPC for the current waypoint (stay inside wizard - no detour gump)
            if (buttonID == 900 && quest != null)
            {
                var editWp = ResolveEditableWaypoint(quest);
                if (editWp != null)
                {
                    m_From.SendGump(new VystiaQuestWizardGump(
                        m_From,
                        WizardStep.SpawnNPCs,
                        m_SelectedQuestID,
                        editWp.WaypointID,
                        false,
                        enteredNpcName,
                        enteredNpcTitle,
                        m_NPCPersonality,
                        m_NPCSpeech,
                        m_AfterSelectStep));
                }
                return;
            }

            // Waypoint editor pickers (dropdown-like)
            if ((buttonID == 7050 || buttonID == 7060 || buttonID == 7070) && quest != null)
            {
                var editWp = ResolveEditableWaypoint(quest);
                if (editWp != null)
                {
                    // Type is automatic now; only condition and sidekick archetype are pickable.
                    if (buttonID == 7060)
                        OpenConditionPicker(quest, editWp);
                    else if (buttonID == 7070)
                        OpenSidekickArchetypePicker(quest, editWp);
                    return;
                }
            }

            // Step 0: Create new
            if (m_Step == WizardStep.SelectOrCreate)
            {
                if (buttonID == 60 && m_SelectedQuestID > 0)
                {
                    m_From.SendGump(new VystiaQuestWizardConfirmGump(
                        m_From,
                        VystiaQuestWizardConfirmGump.ConfirmKind.DeleteQuest,
                        m_SelectedQuestID,
                        m_Step,
                        m_CurrentWaypointID,
                        enteredNpcName,
                        enteredNpcTitle,
                        m_NPCPersonality,
                        m_NPCSpeech,
                        m_AfterSelectStep));
                    return;
                }

                if (buttonID == 50)
                {
                    var newQuest = new DynamicQuest();
                    DynamicQuestManager.RegisterDynamicQuest(newQuest);
                    DynamicQuestManager.Save();
                    m_From.SendMessage($"Created quest [{newQuest.QuestID}].");
                    m_From.SendGump(Rebuild(WizardStep.Details, newQuest.QuestID));
                    return;
                }

                // Select quest (100+)
                if (buttonID >= 100 && buttonID < 200)
                {
                    var quests = DynamicQuestManager.GetAllDynamicQuests().OrderBy(q => q.QuestID).ToList();
                    int index = buttonID - 100;
                    if (index >= 0 && index < quests.Count)
                    {
                        m_From.SendGump(Rebuild(WizardStep.SelectOrCreate, quests[index].QuestID));
                        return;
                    }
                }
            }

            // Navigation: Back
            if (buttonID == 2)
            {
                m_From.SendGump(Rebuild(PrevStep(m_Step), m_SelectedQuestID, m_CurrentWaypointID));
                return;
            }

            // Step 7 entry: no quest selected -> go to select/create, then jump back to NPCs
            if (buttonID == 9800)
            {
                m_From.SendGump(new VystiaQuestWizardGump(
                    m_From,
                    WizardStep.SelectOrCreate,
                    0,
                    0,
                    false,
                    enteredNpcName,
                    enteredNpcTitle,
                    m_NPCPersonality,
                    m_NPCSpeech,
                    WizardStep.SpawnNPCs));
                return;
            }

            // Step 7: personality selection (9300+)
            if (m_Step == WizardStep.SpawnNPCs && buttonID >= 9300 && buttonID < 9400)
            {
                var personalities = new[]
                {
                    NPCPersonalities.PersonalityType.Sage,
                    NPCPersonalities.PersonalityType.Warrior,
                    NPCPersonalities.PersonalityType.Mage,
                    NPCPersonalities.PersonalityType.Merchant,
                    NPCPersonalities.PersonalityType.Noble,
                    NPCPersonalities.PersonalityType.Commoner,
                    NPCPersonalities.PersonalityType.Healer,
                    NPCPersonalities.PersonalityType.Guard
                };

                int idx = buttonID - 9300;
                if (idx >= 0 && idx < personalities.Length)
                {
                    m_From.SendGump(new VystiaQuestWizardGump(
                        m_From,
                        WizardStep.SpawnNPCs,
                        m_SelectedQuestID,
                        m_CurrentWaypointID,
                        false,
                        enteredNpcName,
                        enteredNpcTitle,
                        personalities[idx],
                        m_NPCSpeech,
                        m_AfterSelectStep));
                    return;
                }
            }

            // Step 7: speech selection (9400+)
            if (m_Step == WizardStep.SpawnNPCs && buttonID >= 9400 && buttonID < 9500)
            {
                var patterns = Enum.GetValues(typeof(NPCPersonalities.SpeechPattern)).Cast<NPCPersonalities.SpeechPattern>().ToArray();
                int idx = buttonID - 9400;
                if (idx >= 0 && idx < patterns.Length)
                {
                    m_From.SendGump(new VystiaQuestWizardGump(
                        m_From,
                        WizardStep.SpawnNPCs,
                        m_SelectedQuestID,
                        m_CurrentWaypointID,
                        false,
                        enteredNpcName,
                        enteredNpcTitle,
                        m_NPCPersonality,
                        patterns[idx],
                        m_AfterSelectStep));
                    return;
                }
            }

            // Objectives: delete objective buttons
            if (m_Step == WizardStep.Objectives && quest != null && buttonID >= 200 && buttonID < 300)
            {
                int index = buttonID - 200;
                if (index >= 0 && index < quest.ObjectivesList.Count)
                {
                    var obj = quest.ObjectivesList[index];
                    quest.RemoveObjective(obj.Key);
                    DynamicQuestManager.Save();
                }
                m_From.SendGump(Rebuild(WizardStep.Objectives, m_SelectedQuestID, m_CurrentWaypointID));
                return;
            }

            // Rewards: delete reward buttons
            if (m_Step == WizardStep.Rewards && quest != null && buttonID >= 300 && buttonID < 400)
            {
                int index = buttonID - 300;
                if (index >= 0 && index < quest.Rewards.Count)
                {
                    quest.Rewards.RemoveAt(index);
                    DynamicQuestManager.Save();
                }
                m_From.SendGump(Rebuild(WizardStep.Rewards, m_SelectedQuestID, m_CurrentWaypointID));
                return;
            }

            // Step 7: select waypoint for NPC spawning
            if (m_Step == WizardStep.SpawnNPCs && quest != null && buttonID >= 800 && buttonID < 900)
            {
                var talkWaypoints = quest.Waypoints
                    .Where(w => w.Condition == VystiaWaypointCondition.TalkToNPC)
                    .OrderBy(w => w.OrderIndex)
                    .ToList();

                int index = buttonID - 800;
                if (index >= 0 && index < talkWaypoints.Count)
                {
                    m_From.SendGump(new VystiaQuestWizardGump(
                        m_From,
                        WizardStep.SpawnNPCs,
                        m_SelectedQuestID,
                        talkWaypoints[index].WaypointID,
                        false,
                        enteredNpcName,
                        enteredNpcTitle,
                        m_NPCPersonality,
                        m_NPCSpeech,
                        m_AfterSelectStep));
                    return;
                }
            }

            // Step 7: Spawn & Link NPC (inline)
            if (m_Step == WizardStep.SpawnNPCs && quest != null && buttonID == 9500)
            {
                if (m_CurrentWaypointID <= 0)
                {
                    m_From.SendMessage("Select a waypoint first.");
                    m_From.SendGump(Rebuild(WizardStep.SpawnNPCs, m_SelectedQuestID, m_CurrentWaypointID));
                    return;
                }

                var targetWp = quest.GetWaypoint(m_CurrentWaypointID);
                if (targetWp == null)
                {
                    m_From.SendMessage("Waypoint not found.");
                    m_From.SendGump(Rebuild(WizardStep.SpawnNPCs, m_SelectedQuestID, m_CurrentWaypointID));
                    return;
                }

                var npc = new Server.Custom.VystiaClasses.Quests.QuestNPC(
                    string.IsNullOrEmpty(enteredNpcName) ? "Quest Giver" : enteredNpcName,
                    string.IsNullOrEmpty(enteredNpcTitle) ? "the Quest NPC" : enteredNpcTitle);

                npc.PersonalityType = m_NPCPersonality;
                npc.SpeechPattern = m_NPCSpeech;

                // Persist desired defaults on the waypoint too (helps future spawns)
                targetWp.LLMPersonality = m_NPCPersonality.ToString();
                targetWp.LLMSpeechPattern = m_NPCSpeech.ToString();
                targetWp.NPCTypeName = npc.GetType().Name;

                npc.MoveToWorld(m_From.Location, m_From.Map);
                npc.LinkToWaypoint(quest.QuestID, targetWp.WaypointID);

                Effects.SendLocationParticles(npc, 0x376A, 9, 32, 5008);
                npc.PlaySound(0x1FE);

                DynamicQuestManager.Save();
                m_From.SendMessage($"QuestNPC '{npc.Name}' spawned and linked to '{targetWp.Name}'.");

                m_From.SendGump(new VystiaQuestWizardGump(
                    m_From,
                    WizardStep.SpawnNPCs,
                    m_SelectedQuestID,
                    m_CurrentWaypointID,
                    false,
                    enteredNpcName,
                    enteredNpcTitle,
                    m_NPCPersonality,
                    m_NPCSpeech,
                    m_AfterSelectStep));
                return;
            }

            // Step 7: Done
            if (m_Step == WizardStep.SpawnNPCs && buttonID == 999)
            {
                m_From.SendMessage($"Quest wizard complete for [{m_SelectedQuestID}].");
                return;
            }

            // Step 7: Spawn Vystia NPC picker (world content)
            if (m_Step == WizardStep.SpawnNPCs && buttonID == 9600)
            {
                m_From.SendGump(new VystiaNPCSpawnPickerGump(
                    m_From,
                    0,
                    WizardStep.SpawnNPCs,
                    m_SelectedQuestID,
                    m_CurrentWaypointID,
                    enteredNpcName,
                    enteredNpcTitle,
                    m_NPCPersonality,
                    m_NPCSpeech,
                    m_AfterSelectStep));
                return;
            }

            // Step 3: Add another intermediate waypoint
            if (m_Step == WizardStep.Intermediate && quest != null && buttonID == 910)
            {
                // Save current waypoint edits, then create a new blank intermediate and keep step
                SaveWaypointFromEntries(info, quest, ResolveEditableWaypoint(quest));
                DynamicQuestManager.Save();

                var newWp = new QuestWaypoint("New Waypoint", VystiaWaypointType.Waypoint);
                newWp.Description = "Complete this step.";
                newWp.Location = m_From.Location;
                newWp.Map = m_From.Map;
                quest.AddWaypoint(newWp);
                DynamicQuestManager.Save();

                m_From.SendGump(new VystiaQuestWizardGump(m_From, WizardStep.Intermediate, m_SelectedQuestID, newWp.WaypointID, true));
                return;
            }

            // Step-specific actions that happen before Next
            if (buttonID == 30 && m_Step == WizardStep.Objectives && quest != null)
            {
                string key = GetString(info, 10) ?? "objective";
                string display = GetString(info, 11) ?? "Objective";
                int amount = GetInt(info, 12, 1);
                quest.AddObjective(key, amount, display);
                DynamicQuestManager.Save();
                m_From.SendMessage("Objective added.");
                m_From.SendGump(new VystiaQuestWizardGump(m_From, WizardStep.Objectives, m_SelectedQuestID));
                return;
            }

            if (m_Step == WizardStep.Rewards && quest != null)
            {
                if (buttonID == 40)
                {
                    int amt = GetInt(info, 20, 100);
                    quest.AddReward(new GoldReward(amt));
                    DynamicQuestManager.Save();
                    m_From.SendGump(new VystiaQuestWizardGump(m_From, WizardStep.Rewards, m_SelectedQuestID));
                    return;
                }
                if (buttonID == 41)
                {
                    string skillName = GetString(info, 21) ?? "Magery";
                    double amt = GetDouble(info, 22, 5.0);
                    if (Enum.TryParse(skillName, true, out SkillName skill))
                    {
                        quest.AddReward(new SkillReward(skill, amt));
                        DynamicQuestManager.Save();
                        m_From.SendGump(new VystiaQuestWizardGump(m_From, WizardStep.Rewards, m_SelectedQuestID));
                        return;
                    }
                    m_From.SendMessage($"Unknown skill: {skillName}");
                    m_From.SendGump(new VystiaQuestWizardGump(m_From, WizardStep.Rewards, m_SelectedQuestID));
                    return;
                }
                if (buttonID == 42)
                {
                    string typeName = GetString(info, 23) ?? "Gold";
                    int amt = GetInt(info, 24, 1);
                    quest.AddReward(new ItemReward(typeName, amt));
                    DynamicQuestManager.Save();
                    m_From.SendGump(new VystiaQuestWizardGump(m_From, WizardStep.Rewards, m_SelectedQuestID));
                    return;
                }
                if (buttonID == 43)
                {
                    string title = GetString(info, 25) ?? "the Hero";
                    quest.AddReward(new TitleReward(title));
                    DynamicQuestManager.Save();
                    m_From.SendGump(new VystiaQuestWizardGump(m_From, WizardStep.Rewards, m_SelectedQuestID));
                    return;
                }
            }

            // Next button
            if (buttonID == 1)
            {
                if (m_Step == WizardStep.SelectOrCreate)
                {
                    // If this wizard instance was opened for NPC linking ([aqn]), jump straight to NPC step after selecting quest.
                    m_From.SendGump(Rebuild(m_AfterSelectStep, m_SelectedQuestID));
                    return;
                }

                if (quest == null)
                {
                    m_From.SendMessage("Quest not found.");
                    m_From.SendGump(Rebuild(WizardStep.SelectOrCreate, 0));
                    return;
                }

                if (m_Step == WizardStep.Details)
                {
                    quest.SetTitle(GetString(info, 0) ?? quest.Title);
                    quest.SetDescription(GetString(info, 1) ?? quest.Description);
                    quest.SetRequiredClass((PlayerClassTypeV2)GetInt(info, 2, (int)quest.RequiredClass));
                    quest.SetTier((QuestTier)GetInt(info, 3, (int)quest.Tier));
                    quest.SetPrerequisiteQuestID(GetInt(info, 4, quest.PrerequisiteQuestID));
                    DynamicQuestManager.Save();
                    m_From.SendGump(Rebuild(WizardStep.Origin, m_SelectedQuestID));
                    return;
                }

                if (m_Step == WizardStep.Origin)
                {
                    var origin = quest.GetOrigin();
                    SaveWaypointFromEntries(info, quest, origin);
                    DynamicQuestManager.Save();
                    m_From.SendGump(Rebuild(WizardStep.Intermediate, m_SelectedQuestID));
                    return;
                }

                if (m_Step == WizardStep.Intermediate)
                {
                    var editWp = ResolveEditableWaypoint(quest);
                    if (editWp != null && editWp.Type == VystiaWaypointType.Waypoint)
                    {
                        SaveWaypointFromEntries(info, quest, editWp);
                        DynamicQuestManager.Save();
                    }
                    m_From.SendGump(Rebuild(WizardStep.Completion, m_SelectedQuestID));
                    return;
                }

                if (m_Step == WizardStep.Completion)
                {
                    var completion = quest.GetCompletion();
                    SaveWaypointFromEntries(info, quest, completion);
                    DynamicQuestManager.Save();
                    m_From.SendGump(Rebuild(WizardStep.Objectives, m_SelectedQuestID));
                    return;
                }

                if (m_Step == WizardStep.Objectives)
                {
                    m_From.SendGump(Rebuild(WizardStep.Rewards, m_SelectedQuestID));
                    return;
                }

                if (m_Step == WizardStep.Rewards)
                {
                    m_From.SendGump(Rebuild(WizardStep.SpawnNPCs, m_SelectedQuestID));
                    return;
                }
            }

            // Default refresh
            m_From.SendGump(new VystiaQuestWizardGump(
                m_From,
                m_Step,
                m_SelectedQuestID,
                m_CurrentWaypointID,
                m_IntermediateContinue,
                enteredNpcName,
                enteredNpcTitle,
                m_NPCPersonality,
                m_NPCSpeech,
                m_AfterSelectStep));
        }

        private DynamicQuest CreateSampleQuest(Mobile from)
        {
            if (from == null)
                return null;

            var quest = new DynamicQuest();
            quest.SetTitle("Sample Quest: Trials of Vystia");
            quest.SetDescription("A demo quest fully wired via the Quest Wizard. Speak to the guides, then slay Frost Father to finish.");
            quest.SetRequiredClass(PlayerClassTypeV2.None);
            quest.SetTier(QuestTier.Initiation);
            quest.SetPrerequisiteQuestID(0);

            quest.ClearWaypoints();
            quest.ClearObjectives();
            quest.ClearRewards();

            // 3 talk steps (origin + 2 intermediates)
            var wp0 = new QuestWaypoint("Speak to the Mentor", VystiaWaypointType.Origin);
            wp0.Description = "Talk to Mentor Arlen to begin.";
            wp0.Condition = VystiaWaypointCondition.TalkToNPC;
            wp0.NPCDialogueContext =
                "You are the quest giver. Offer the quest and instruct the player to find Scout Lyra next. " +
                "Be clear: their next step is to talk to Scout Lyra.";
            wp0.PlayerLocationHint = "The Scout can usually be found in Yew.";

            var wp1 = new QuestWaypoint("Consult the Scout", VystiaWaypointType.Waypoint);
            wp1.Description = "Find Scout Lyra and ask about the threat.";
            wp1.Condition = VystiaWaypointCondition.TalkToNPC;
            wp1.NPCDialogueContext =
                "You are the scout. Brief the player on the situation and direct them to report to Captain Doran next. " +
                "Be clear: their next step is to talk to Captain Doran.";
            wp1.PlayerLocationHint = "the ranger can be typically found in yew";

            var wp2 = new QuestWaypoint("Report to the Captain", VystiaWaypointType.Waypoint);
            wp2.Description = "Return to Captain Doran for final instructions.";
            wp2.Condition = VystiaWaypointCondition.TalkToNPC;
            wp2.NPCDialogueContext =
                "You are the captain. Give final instructions: the player must hunt and slay Frost Father to complete the trial. " +
                "Be clear: their next step is to find and kill Frost Father.";
            wp2.PlayerLocationHint = "Frost Father is said to haunt the Frozen Halls — deep within the icy dungeon.";
            wp2.PlayerLocationHint = "the captain is on duty in the lords castle";

            // Kill-creature finish
            var wp3 = new QuestWaypoint("Slay Frost Father", VystiaWaypointType.BossCompletion);
            wp3.Description = "Defeat Frost Father to prove your strength.";
            wp3.Condition = VystiaWaypointCondition.DefeatBoss;
            wp3.TargetTypeName = "FrostFather";
            wp3.RequiredAmount = 1;
            wp3.PlayerLocationHint = "the evil frostfather is on the island of ice, beware, it will not be easy";

            quest.AddWaypoint(wp0);
            quest.AddWaypoint(wp1);
            quest.AddWaypoint(wp2);
            quest.AddWaypoint(wp3);

            DynamicQuestManager.RegisterDynamicQuest(quest);

            // Spawn locations (demo defaults)
            // - Giver: GM location
            // - Scout: 533 999 0
            // - Captain: 1332 1621 50
            // - FrostFather: 4075 459 1
            //
            // NOTE: All spawns use the GM's current map/facet.
            // If these coords are intended for a specific facet, set your GM to that facet before running the demo.
            var map = from.Map;
            var baseLoc = from.Location;

            Point3D loc0 = baseLoc;
            Point3D loc1 = new Point3D(533, 999, 0);
            Point3D loc2 = new Point3D(1332, 1621, 50);
            Point3D bossLoc = new Point3D(4075, 459, 1);

            SpawnAndLinkQuestNpc(from, quest, wp0, "Mentor Arlen", "the Mentor", loc0, map, NPCPersonalities.PersonalityType.Sage, NPCPersonalities.SpeechPattern.Formal);
            SpawnAndLinkQuestNpc(from, quest, wp1, "Scout Lyra", "the Scout", loc1, map, NPCPersonalities.PersonalityType.Guard, NPCPersonalities.SpeechPattern.Casual);
            SpawnAndLinkQuestNpc(from, quest, wp2, "Captain Doran", "the Captain", loc2, map, NPCPersonalities.PersonalityType.Warrior, NPCPersonalities.SpeechPattern.Formal);

            SpawnDemoBoss(from, "FrostFather", bossLoc, map);
            wp3.Location = bossLoc;
            wp3.Map = map;

            DynamicQuestManager.Save();
            return quest;
        }

        private Point3D GetGroundPoint(Map map, int x, int y)
        {
            if (map == null)
                return Point3D.Zero;

            int z = map.GetAverageZ(x, y);
            return new Point3D(x, y, z);
        }

        private void SpawnAndLinkQuestNpc(
            Mobile from,
            DynamicQuest quest,
            QuestWaypoint waypoint,
            string name,
            string title,
            Point3D loc,
            Map map,
            NPCPersonalities.PersonalityType personality,
            NPCPersonalities.SpeechPattern speech)
        {
            if (from == null || quest == null || waypoint == null || map == null)
                return;

            var npc = new Server.Custom.VystiaClasses.Quests.QuestNPC(name, title);
            npc.PersonalityType = personality;
            npc.SpeechPattern = speech;

            // Persist defaults on waypoint too (optional)
            waypoint.LLMPersonality = personality.ToString();
            waypoint.LLMSpeechPattern = speech.ToString();
            waypoint.NPCTypeName = npc.GetType().Name;

            npc.MoveToWorld(loc, map);
            npc.LinkToWaypoint(quest.QuestID, waypoint.WaypointID);

            Effects.SendLocationParticles(npc, 0x376A, 9, 32, 5008);
            npc.PlaySound(0x1FE);
        }

        private void SpawnDemoBoss(Mobile from, string typeName, Point3D loc, Map map)
        {
            if (from == null || map == null || string.IsNullOrWhiteSpace(typeName))
                return;

            try
            {
                Type t = ScriptCompiler.FindTypeByName(typeName);
                if (t == null)
                {
                    from.SendMessage($"[SampleQuest] Could not find creature type: {typeName}");
                    return;
                }

                object inst = Activator.CreateInstance(t);
                if (!(inst is BaseCreature bc))
                {
                    from.SendMessage($"[SampleQuest] Type {typeName} is not a creature.");
                    return;
                }

                bc.MoveToWorld(loc, map);
                bc.Home = loc;
                bc.RangeHome = 10;

                Effects.SendLocationParticles(bc, 0x376A, 9, 32, 5008);
                bc.PlaySound(0x1FE);
            }
            catch (Exception ex)
            {
                from.SendMessage($"[SampleQuest] Failed to spawn {typeName}: {ex.Message}");
            }
        }

        private QuestWaypoint ResolveEditableWaypoint(DynamicQuest quest)
        {
            if (quest == null)
                return null;

            if (m_Step == WizardStep.Origin)
                return quest.GetOrigin();
            if (m_Step == WizardStep.Completion)
                return quest.GetCompletion();

            if (m_Step == WizardStep.Intermediate)
            {
                if (m_CurrentWaypointID > 0)
                    return quest.GetWaypoint(m_CurrentWaypointID);

                // If no explicit selection, try the last intermediate waypoint
                return quest.GetIntermediateWaypoints().LastOrDefault();
            }

            return null;
        }

        private void SaveWaypointFromEntries(RelayInfo info, DynamicQuest quest, QuestWaypoint waypoint)
        {
            if (quest == null || waypoint == null)
                return;

            waypoint.Name = GetString(info, 50) ?? waypoint.Name;
            waypoint.Description = GetString(info, 51) ?? waypoint.Description;
            waypoint.NPCDialogueContext = GetString(info, 53);
            waypoint.PlayerLocationHint = GetString(info, 58);
            waypoint.ObjectiveKey = GetString(info, 54);
            waypoint.RequiredAmount = GetInt(info, 55, waypoint.RequiredAmount);
            waypoint.Radius = GetInt(info, 56, waypoint.Radius);
            waypoint.TargetTypeName = GetString(info, 57) ?? waypoint.TargetTypeName;
        }

        private WizardStep PrevStep(WizardStep step)
        {
            if (step == WizardStep.SelectOrCreate)
                return WizardStep.SelectOrCreate;

            return (WizardStep)((int)step - 1);
        }

        private string GetString(RelayInfo info, int id)
        {
            TextRelay relay = info.GetTextEntry(id);
            return relay?.Text?.Trim();
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

        #endregion

        public static void Initialize()
        {
            CommandSystem.Register("QuestWizard", AccessLevel.GameMaster, QuestWizard_OnCommand);
            CommandSystem.Register("QW", AccessLevel.GameMaster, QuestWizard_OnCommand);

            // Make these the default entry points (replaces old multi-mode editor UX)
            CommandSystem.Register("QuestEditor", AccessLevel.GameMaster, QuestWizard_OnCommand);
            CommandSystem.Register("QE", AccessLevel.GameMaster, QuestWizard_OnCommand);
        }

        /// <summary>
        /// Convenience entry point used by other gumps (e.g. NPC spawn wizard) to return to this flow.
        /// </summary>
        public static void OpenSpawnNPCs(Mobile from, int questId, int waypointId = 0)
        {
            if (from == null)
                return;

            from.SendGump(new VystiaQuestWizardGump(from, WizardStep.SpawnNPCs, questId, waypointId));
        }

        [Usage("QuestWizard")]
        [Aliases("QW", "QuestEditor", "QE")]
        [Description("Opens the Vystia Quest Wizard (step-by-step).")]
        private static void QuestWizard_OnCommand(CommandEventArgs e)
        {
            e.Mobile.SendGump(new VystiaQuestWizardGump(e.Mobile));
        }
    }
}


