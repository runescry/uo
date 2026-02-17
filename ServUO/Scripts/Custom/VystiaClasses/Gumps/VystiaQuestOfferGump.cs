using System;
using Server;
using Server.Gumps;
using Server.Mobiles;
using Server.Network;
using Server.Custom.VystiaClasses.Quests;

namespace Server.Custom.VystiaClasses.Gumps
{
    /// <summary>
    /// Simple quest offer / started confirmation gump.
    /// - If quest already started, shows current waypoint.
    /// - If not started, shows offer + Accept/Decline.
    /// </summary>
    public class VystiaQuestOfferGump : Gump
    {
        private readonly PlayerMobile m_Player;
        private readonly int m_QuestID;
        private readonly bool m_JustStarted;

        public VystiaQuestOfferGump(PlayerMobile pm, int questID, bool justStarted)
            : base(120, 120)
        {
            m_Player = pm;
            m_QuestID = questID;
            m_JustStarted = justStarted;

            Closable = true;
            Disposable = true;
            Dragable = true;
            Resizable = false;

            Build();
        }

        private void Build()
        {
            AddPage(0);

            AddBackground(0, 0, 520, 360, 9270);
            AddAlphaRegion(10, 10, 500, 340);

            var quest = DynamicQuestManager.GetQuest(m_QuestID);
            string title = quest != null ? quest.Title : "Unknown Quest";
            string desc = quest != null ? quest.Description : "Quest not found.";

            AddHtml(20, 20, 480, 25, Center(Color(title, 0x00FFFF)), false, false);

            bool hasStarted = m_Player != null && VystiaQuestSystem.HasActiveQuest(m_Player, m_QuestID);
            // For ephemeral quests, ignore completion status since they're meant to be unique per generation
            bool hasCompleted = false;
            if (m_Player != null && quest is DynamicQuest dq && !dq.IsEphemeral)
            {
                hasCompleted = VystiaQuestSystem.HasCompletedQuest(m_Player, m_QuestID);
            }

            int y = 55;
            if (hasCompleted)
            {
                AddHtml(20, y, 480, 25, Color("You have already completed this quest.", 0x88FF88), false, false);
                y += 30;
            }
            else if (hasStarted)
            {
                AddHtml(20, y, 480, 25, Color(m_JustStarted ? "Quest started!" : "Quest is already active.", 0x88FF88), false, false);
                y += 30;

                var wp = VystiaQuestSystem.GetCurrentWaypoint(m_Player, m_QuestID);
                if (wp != null)
                {
                    AddHtml(20, y, 480, 20, Color("Current objective:", 0xFFFFFF), false, false);
                    y += 22;
                    AddHtml(20, y, 480, 20, Color(wp.Name ?? "(unnamed)", 0x90EE90), false, false);
                    y += 22;
                    if (!string.IsNullOrEmpty(wp.Description))
                    {
                        AddHtml(20, y, 480, 60, Color(wp.Description, 0xAAAAAA), false, true);
                        y += 65;
                    }
                }
            }
            else
            {
                // Offer
                AddHtml(20, y, 480, 18, Color("Quest Offer:", 0xFFFFFF), false, false);
                y += 22;
                AddHtml(20, y, 480, 140, Color(desc, 0xCCCCCC), false, true);
                y += 150;

                AddHtml(20, y, 480, 18, Color("Do you accept this quest?", 0xFFAA00), false, false);
                y += 28;

                AddButton(120, 300, 4005, 4007, 1, GumpButtonType.Reply, 0);
                AddHtml(155, 300, 120, 20, Color("Accept", 0x88FF88), false, false);

                AddButton(300, 300, 4017, 4019, 2, GumpButtonType.Reply, 0);
                AddHtml(335, 300, 120, 20, Color("Decline", 0xAAAAAA), false, false);
                return;
            }

            // Close button (when started/completed)
            AddButton(210, 300, 4017, 4019, 2, GumpButtonType.Reply, 0);
            AddHtml(245, 300, 200, 20, Color("Close", 0xAAAAAA), false, false);
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            if (m_Player == null || m_Player.Deleted)
                return;

            if (info.ButtonID == 1)
            {
                // Accept
                if (VystiaQuestSystem.StartQuest(m_Player, m_QuestID))
                {
                    m_Player.SendMessage($"Quest started: {DynamicQuestManager.GetQuest(m_QuestID)?.Title ?? m_QuestID.ToString()}");

                    // If the Origin waypoint is TalkToNPC, accepting here should immediately advance past it.
                    var dq = DynamicQuestManager.GetQuest(m_QuestID);
                    var origin = dq?.GetOrigin();
                    if (origin != null && origin.Condition == WaypointCondition.TalkToNPC)
                    {
                        VystiaQuestSystem.CompleteWaypoint(m_Player, m_QuestID, origin.WaypointID);
                    }

                    m_Player.SendGump(new VystiaQuestOfferGump(m_Player, m_QuestID, true));
                }
                else
                {
                    // StartQuest should have sent a specific error message, but if we get here, log it
                    Console.WriteLine($"[VystiaQuestOfferGump] StartQuest returned false for quest {m_QuestID}, player {m_Player?.Name}");
                    m_Player.SendMessage("You cannot start this quest right now. Check your quest log or class requirements.");
                }
            }
        }

        private string Color(string text, int color) => $"<BASEFONT COLOR=#{color:X6}>{text}</BASEFONT>";
        private string Center(string text) => $"<CENTER>{text}</CENTER>";
    }
}


