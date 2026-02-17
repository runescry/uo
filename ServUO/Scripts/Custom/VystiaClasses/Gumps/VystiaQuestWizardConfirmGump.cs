using Server;
using Server.Gumps;
using Server.Mobiles;
using Server.Network;
using Server.Custom.VystiaClasses.Quests;
using Server.Services.LLM;

namespace Server.Custom.VystiaClasses.Gumps
{
    public class VystiaQuestWizardConfirmGump : Gump
    {
        public enum ConfirmKind
        {
            DeleteQuest = 0
        }

        private readonly Mobile m_From;
        private readonly ConfirmKind m_Kind;
        private readonly int m_QuestId;

        // return-to wizard state
        private readonly VystiaQuestWizardGump.WizardStep m_ReturnStep;
        private readonly int m_ReturnWaypointId;
        private readonly string m_NpcName;
        private readonly string m_NpcTitle;
        private readonly NPCPersonalities.PersonalityType m_Personality;
        private readonly NPCPersonalities.SpeechPattern m_Speech;
        private readonly VystiaQuestWizardGump.WizardStep m_AfterSelectStep;

        public VystiaQuestWizardConfirmGump(
            Mobile from,
            ConfirmKind kind,
            int questId,
            VystiaQuestWizardGump.WizardStep returnStep,
            int returnWaypointId,
            string npcName,
            string npcTitle,
            NPCPersonalities.PersonalityType personality,
            NPCPersonalities.SpeechPattern speech,
            VystiaQuestWizardGump.WizardStep afterSelectStep)
            : base(120, 120)
        {
            m_From = from;
            m_Kind = kind;
            m_QuestId = questId;
            m_ReturnStep = returnStep;
            m_ReturnWaypointId = returnWaypointId;
            m_NpcName = npcName ?? "";
            m_NpcTitle = npcTitle ?? "";
            m_Personality = personality;
            m_Speech = speech;
            m_AfterSelectStep = afterSelectStep;

            Closable = true;
            Disposable = true;
            Dragable = true;
            Resizable = false;

            Build();
        }

        private void Build()
        {
            AddPage(0);
            AddBackground(0, 0, 520, 220, 9270);
            AddAlphaRegion(10, 10, 500, 200);

            AddHtml(20, 20, 480, 25, Center(Color("Confirm Action", 0xFFFFFF)), false, false);

            string msg = "Are you sure?";
            if (m_Kind == ConfirmKind.DeleteQuest)
            {
                var quest = DynamicQuestManager.GetQuest(m_QuestId);
                string title = quest != null ? quest.Title : $"ID {m_QuestId}";
                msg = $"Delete quest <b>{title}</b>?\nThis removes the quest definition from the server.";
            }

            AddHtml(20, 60, 480, 60, Color(msg, 0xFFAA00), false, false);

            // Yes
            AddButton(120, 150, 4005, 4007, 1, GumpButtonType.Reply, 0);
            AddHtml(155, 150, 120, 20, Color("YES", 0x88FF88), false, false);

            // No
            AddButton(300, 150, 4017, 4019, 2, GumpButtonType.Reply, 0);
            AddHtml(335, 150, 120, 20, Color("NO", 0xAAAAAA), false, false);
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            if (m_From == null || m_From.Deleted)
                return;

            if (info.ButtonID == 1)
            {
                if (m_Kind == ConfirmKind.DeleteQuest)
                {
                    var quest = DynamicQuestManager.GetQuest(m_QuestId);
                    if (quest != null)
                    {
                        DynamicQuestManager.RemoveDynamicQuest(quest);
                        DynamicQuestManager.Save();
                        m_From.SendMessage($"Quest deleted: {quest.Title}");
                    }
                    else
                    {
                        m_From.SendMessage("Quest not found.");
                    }
                }
            }

            // Return to wizard (default main page after deletion)
            m_From.SendGump(new VystiaQuestWizardGump(
                m_From,
                VystiaQuestWizardGump.WizardStep.SelectOrCreate,
                0,
                0,
                false,
                m_NpcName,
                m_NpcTitle,
                m_Personality,
                m_Speech,
                m_AfterSelectStep));
        }

        private string Color(string text, int color) => $"<BASEFONT COLOR=#{color:X6}>{text}</BASEFONT>";
        private string Center(string text) => $"<CENTER>{text}</CENTER>";
    }
}


