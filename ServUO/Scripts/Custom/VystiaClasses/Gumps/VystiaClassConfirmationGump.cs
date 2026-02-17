using System;
using Server;
using Server.Gumps;
using Server.Mobiles;
using Server.Network;

namespace Server.Custom.VystiaClasses.Gumps
{
    /// <summary>
    /// Confirmation gump showing detailed class information
    /// Allows player to confirm or cancel their class selection
    /// </summary>
    public class VystiaClassConfirmationGump : Gump
    {
        private const int GumpWidth = 600;
        private const int GumpHeight = 550;
        private readonly PlayerClassTypeV2 _selectedClass;

        public VystiaClassConfirmationGump(PlayerMobile pm, PlayerClassTypeV2 classType) : base(100, 100)
        {
            if (pm == null)
                return;

            pm.CloseGump(typeof(VystiaClassConfirmationGump));
            pm.CloseGump(typeof(VystiaClassSelectionGump));

            _selectedClass = classType;

            Closable = true;
            Disposable = true;
            Dragable = true;
            Resizable = false;

            AddPage(0);

            // Background
            AddBackground(0, 0, GumpWidth, GumpHeight, 9200);
            AddImageTiled(10, 10, GumpWidth - 20, GumpHeight - 20, 2624);

            // Get class instance to display stats
            _ = VystiaClassManager.Instance;
            PlayerClassV2 classInstance = PlayerClassV2.GetClass(classType);

            if (classInstance == null)
            {
                // Class not yet implemented
                AddHtml(20, 20, GumpWidth - 40, 60,
                    "<CENTER><BASEFONT COLOR=#FF0000 SIZE=7>Class Not Yet Implemented</BASEFONT></CENTER>",
                    false, false);

                AddHtml(20, 100, GumpWidth - 40, 350,
                    "<CENTER><BASEFONT COLOR=#FFFFFF>This class is planned but not yet available.<BR><BR>" +
                    "Please choose a different class for now.<BR><BR>" +
                    "Check back in future updates!</BASEFONT></CENTER>",
                    true, true);

                // Back button
                AddButton(GumpWidth / 2 - 60, GumpHeight - 60, 4017, 4019, 0, GumpButtonType.Reply, 0);
                AddHtml(GumpWidth / 2 - 20, GumpHeight - 55, 100, 25,
                    "<BASEFONT COLOR=#FFFFFF>Go Back</BASEFONT>", false, false);

                return;
            }

            // Header
            AddHtml(20, 20, GumpWidth - 40, 50,
                $"<CENTER><BASEFONT COLOR=#FFD700 SIZE=7>{classInstance.ClassName}</BASEFONT><BR>" +
                $"<BASEFONT COLOR=#C0C0C0 SIZE=4>{classInstance.ClassDescription}</BASEFONT></CENTER>",
                false, false);

            // Divider
            AddImageTiled(20, 75, GumpWidth - 40, 2, 9277);

            // Stats section
            int y = 90;
            AddHtml(30, y, GumpWidth - 60, 25,
                "<BASEFONT COLOR=#FFD700 SIZE=6>Starting Stats</BASEFONT>", false, false);
            y += 30;

            AddHtml(50, y, GumpWidth - 100, 80,
                $"<BASEFONT COLOR=#FFFFFF>" +
                $"Strength: {classInstance.StartStr} (Cap: {classInstance.StrCap})<BR>" +
                $"Dexterity: {classInstance.StartDex} (Cap: {classInstance.DexCap})<BR>" +
                $"Intelligence: {classInstance.StartInt} (Cap: {classInstance.IntCap})</BASEFONT>",
                false, false);
            y += 90;

            // Skills section
            AddHtml(30, y, GumpWidth - 60, 25,
                "<BASEFONT COLOR=#FFD700 SIZE=6>Primary Skills</BASEFONT>", false, false);
            y += 30;

            string skillsText = "<BASEFONT COLOR=#FFFFFF>";
            if (classInstance.PrimarySkills != null && classInstance.StartingSkillValues != null)
            {
                for (int i = 0; i < classInstance.PrimarySkills.Length && i < classInstance.StartingSkillValues.Length; i++)
                {
                    string skillName = classInstance.PrimarySkills[i].ToString();
                    double skillValue = classInstance.StartingSkillValues[i];
                    skillsText += $"• {skillName}: {skillValue:F1}<BR>";
                }
            }
            skillsText += "</BASEFONT>";

            AddHtml(50, y, GumpWidth - 100, 120, skillsText, true, true);
            y += 130;

            // Warning
            AddHtml(30, y, GumpWidth - 60, 40,
                "<CENTER><BASEFONT COLOR=#FF4444 SIZE=5>⚠ This choice is permanent! ⚠<BR>" +
                "Make sure this is the class you want!</BASEFONT></CENTER>",
                false, false);
            y += 50;

            // Buttons
            int buttonY = GumpHeight - 60;

            // Cancel button (left)
            AddButton(50, buttonY, 4017, 4019, 0, GumpButtonType.Reply, 0);
            AddHtml(90, buttonY + 5, 100, 25, "<BASEFONT COLOR=#FFFFFF>Go Back</BASEFONT>", false, false);

            // Confirm button (right)
            AddButton(GumpWidth - 150, buttonY, 4023, 4025, 1, GumpButtonType.Reply, 0);
            AddHtml(GumpWidth - 105, buttonY + 5, 100, 25, "<BASEFONT COLOR=#00FF00>CONFIRM</BASEFONT>", false, false);
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            PlayerMobile pm = sender?.Mobile as PlayerMobile;
            if (pm == null)
                return;

            if (info.ButtonID == 1) // Confirm
            {
                // Apply the class
                VystiaClassApplicator.ApplyClass(pm, _selectedClass);
            }
            else // Cancel (button 0 or close gump)
            {
                // Return to class selection
                pm.SendGump(new VystiaClassSelectionGump(pm));
            }
        }
    }
}
