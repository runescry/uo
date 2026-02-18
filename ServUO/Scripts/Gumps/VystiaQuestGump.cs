using System;
using System.Collections.Generic;
using Server.Mobiles;
using Server.Items;
using Server.Gumps;
using Server.Network;
using Server.Commands;
using Server.Services.UnifiedQuestSystem;
using Server.Custom.VystiaClasses.Quests;

namespace Server.Gumps
{
    /// <summary>
    /// Vystia Quest Gump - Enhanced quest gump with Vystia styling and content
    /// </summary>
    public class VystiaQuestGump : BaseGump
    {
        private readonly Mobile m_From;
        private readonly UnifiedQuestData m_Quest;
        private readonly VystiaQuestDefinition m_QuestDef;

        public VystiaQuestGump(Mobile from, UnifiedQuestData quest) : base(50, 50)
        {
            m_From = from;
            m_Quest = quest;
            m_QuestDef = VystiaQuestSystem.GetQuestDefinition(
                quest.Metadata?.GetValueOrDefault("vystia_quest_type", "unknown") as string
            );

            Closable = true;
            Disposable = false;

            AddPage(0);

            AddBackground(0, 0, 400, 450, 9380);
            AddAlphaRegion(10, 10, 380, 430);

            // Quest header
            AddHtml(10, 10, 380, 30, $"<CENTER><BASEFONT COLOR=#FFFFFF><BIG>{m_Quest.Title}</BIG></BASEFONT></CENTER>", false, false);
            
            AddHtml(10, 45, 380, 60, $"<BASEFONT COLOR=#FFFFFF>{m_Quest.Description}</BASEFONT>", false, false);

            // Quest objectives
            AddHtml(10, 110, 380, 150, $"<BASEFONT COLOR=#FFFFFF><B>Objectives:</BASEFONT></BASEFONT></BASEFONT>", false, false);
            
            if (m_Quest.Objectives != null)
            {
                int i = 0;
                foreach (var objective in m_Quest.Objectives)
                {
                    var status = objective.IsCompleted ? "✓" : "○";
                    var progress = $"{objective.CurrentProgress}/{objective.RequiredAmount}";
                    AddHtml(10, 140 + (i * 25), 380, 20, $"<BASEFONT COLOR=#FFFFFF>{status} {objective.Description}: {progress}</BASEFONT>", false, false);
                    i++;
                }
            }

            // Quest rewards
            AddHtml(10, 300, 380, 30, $"<BASEFONT COLOR=#FFFFFF><B>Rewards:</BASEFONT></BASEFONT></BASEFONT>", false, false);
            
            if (m_Quest.Rewards != null)
            {
                int i = 0;
                foreach (var reward in m_Quest.Rewards)
                {
                    AddHtml(10, 330 + (i * 25), 380, 20, $"<BASEFONT COLOR=#FFFFFF>• {reward.Description}</BASEFONT>", false, false);
                    i++;
                }
            }

            // Quest metadata
            AddHtml(10, 400, 380, 30, $"<BASEFONT COLOR=#FFFFFF><B>Details:</BASEFONT></BASEFONT></BASEFONT>", false, false);
            
            if (m_Quest.Metadata != null)
            {
                AddHtml(10, 430, 380, 30, $"<BASEFONT COLOR=#FFFFFF>Type: {m_Quest.Metadata.GetValueOrDefault("vystia_quest_type", "unknown")}</BASEFONT>", false, false);
                AddHtml(10, 450, 380, 30, $"<BASEFONT COLOR=#FFFFFF>Tier: {m_Quest.Metadata.GetValueOrDefault("vystia_quest_tier", "unknown")}</BASEFONT>", false, false);
                AddHtml(10, 470, 380, 30, $<BASEFONT COLOR=#FFFFFF>Difficulty: {m_Quest.Metadata.GetValueOrDefault("vystia_quest_difficulty", "unknown")}</BASEFONT>", false, false);
            }

            // Action buttons
            AddButton(20, 380, 4005, 4013, 1, GumpButtonType.Reply, 0);
            AddHtml(55, 380, 300, 20, "<BASEFONT COLOR=#FFFFFF>Accept Quest</BASEFONT>", false, false);

            AddButton(20, 410, 4005, 4013, 2, GumpButtonType.Reply, 0);
            AddHtml(55, 410, 300, 20, "<BASEFONT COLOR=#FFFFFF>Abandon Quest</BASEFONT>", false, false);

            AddButton(20, 440, 4005, 4013, 3, GumpButtonType.Reply, 0);
            AddHtml(55, 440, 300, 20, "<BASEFONT COLOR=#FFFFFF>View Progress</BASEFONT>", false, false);
        }

        public override void OnResponse(NetworkNetState sender, RelayInfo info)
        {
            switch (info.ButtonID)
            {
                case 1: // Accept Quest
                    AcceptQuest();
                    break;
                case 2: // Abandon Quest
                    AbandonQuest();
                    break;
                case 3: // View Progress
                    ViewProgress();
                    break;
            }
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            // Handle response
        }

        private void AcceptQuest()
        {
            if (m_From is PlayerMobile player)
            {
                VystiaQuestSystem.CompleteVystiaQuest(player, m_Quest);
            }
        }

        private void AbandonQuest()
        {
            if (m_From is PlayerMobile player)
            {
                VystiaQuestSystem.HandleQuestProgress(player, m_Quest.QuestId, "abandon", 0, "Quest abandoned");
            }
        }

        private void ViewProgress()
        {
            if (m_From is PlayerMobile player)
            {
                VystiaQuestSystem.HandleQuestProgress(player, m_Quest.QuestId, "progress", 0, "Quest progress check");
            }
        }
    }

    /// <summary>
    /// Vystia Quest Journal Gump - Enhanced quest journal with Vystia styling
    /// </summary>
    public class VystiaQuestJournalGump : BaseGump
    {
        private readonly Mobile m_From;
        private readonly VystiaQuestPlayerData m_PlayerData;

        public VystiaQuestJournalGump(Mobile from) : base(50, 50)
        {
            m_From = from;
            m_PlayerData = VystiaQuestSystem.GetPlayerData(from);

            Closable = true;
            Disposable = false;

            AddPage(0);

            AddBackground(0, 0, 420, 400, 9380);
            AddAlphaRegion(10, 10, 400, 380);

            AddHtml(10, 10, 400, 30, $"<CENTER><BASEFONT COLOR=#FFFFFF><BIG>VYSTIA QUEST JOURNAL</BIG></BASEFONT></CENTER>", false, false);
            
            AddHtml(10, 50, 400, 20, $"<BASEFONT COLOR=#FFFFFF>Quest Progress Overview:</BASEFONT></BASEFONT>", false, false);

            // Active quests
            AddHtml(10, 80, 400, 20, $"<BASEFONT COLOR=#FFFFFF><B>Active Quests: {m_PlayerData.ActiveQuests.Count}</BASEFONT></BASEFONT>", false, false);
            
            if (m_PlayerData.ActiveQuests.Count > 0)
            {
                AddHtml(10, 110, 380, 150, $"<BASEFONT COLOR=#FFFFFF>Active Quests:</BASEFONT></BASEFONT>", false, false);
                
                int i = 0;
                foreach (var questId in m_PlayerData.ActiveQuests)
                {
                    var quest = UnifiedQuestSystem.GetQuest(questId);
                    if (quest != null)
                    {
                        var status = quest.IsCompleted ? "✓" : "○";
                        AddHtml(10, 140 + (i * 25), 380, 20, $"<BASEFONT COLOR=#FFFFFF>{status} {quest.Title}</BASEFONT>", false, false);
                        i++;
                    }
                }
            }

            // Completed quests
            AddHtml(10, 300, 400, 20, $"<BASEFONT COLOR=#FFFFFF>Completed Quests: {m_PlayerData.CompletedQuests.Count}</BASEFONT></BASEFONT>", false, false);
            
            if (m_PlayerData.CompletedQuests.Count > 0)
            {
                AddHtml(10, 330, 380, 150, $"<BASEFONT COLOR=#FFFFFF>Completed Quests:</BASEFONT></BASEFONT>", false, false);
                
                int i = 0;
                foreach (var questId in m_PlayerData.CompletedQuests)
                {
                    var quest = UnifiedQuestSystem.GetQuest(questId);
                    if (quest != null)
                    {
                        AddHtml(10, 360 + (i * 25), 380, 20, $"<BASEFONT COLOR=#FFFFFF>✓ {quest.Title}</BASEFONT></BASEFONT>", false, false);
                        i++;
                    }
                }
            }

            // Statistics
            AddHtml(10, 410, 400, 20, $"<BASEFONT COLOR=#FFFFFF>Total Quests: {m_PlayerData.ActiveQuests.Count + m_PlayerData.CompletedQuests.Count}</BASEFONT>", false, false);
        }
    }

    /// <summary>
    /// Vystia Quest Offer Gump - Enhanced quest offer gump with Vystia styling
    /// </summary>
    public class VystiaQuestOfferGump : BaseGump
    {
        private readonly Mobile m_From;
        private readonly UnifiedQuestData m_Quest;
        private readonly VystiaQuestDefinition m_QuestDef;

        public VystiaQuestOfferGump(Mobile from, UnifiedQuestData quest) : base(50, 50)
        {
            m_From = from;
            m_Quest = quest;
            m_QuestDef = VystiaQuestSystem.GetQuestDefinition(
                quest.Metadata?.GetValueOrDefault("vystia_quest_type", "unknown") as string
            );

            Closable = true;
            Disposable = false;

            AddPage(0);

            AddBackground(0, 0, 400, 300, 9380);
            AddAlphaRegion(10, 10, 380, 280);

            // Quest offer header
            AddHtml(10, 10, 380, 30, $"<CENTER><BASEFONT COLOR=#FFFFFF><BIG>QUEST OFFER</BIG></BASEFONT></CENTER>", false, false);
            
            AddHtml(10, 45, 380, 60, $"<BASEFONT COLOR=#FFFFFF>{m_Quest.Title}</BASEFONT>", false, false);
            AddHtml(10, 105, 380, 60, $"<BASEFONT COLOR=#FFFFFF>{m_Quest.Description}</BASEFONT>", false, false);

            // Quest objectives preview
            AddHtml(10, 170, 380, 20, $"<BASEFONT COLOR=#FFFFFF><B>Objectives:</BASEFONT></BASEFONT>", false, false);
            
            if (m_Quest.Objectives != null)
            {
                int i = 0;
                foreach (var objective in m_Quest.Objectives)
                {
                    var status = objective.IsCompleted ? "✓" : "○";
                    var progress = $"{objective.CurrentProgress}/{objective.RequiredAmount}";
                    AddHtml(10, 200 + (i * 25), 380, 20, $"<BASEFONT COLOR=#FFFFFF>{status} {objective.Description}: {progress}</BASEFONT>", false, false);
                    i++;
                }
            }

            // Quest rewards preview
            AddHtml(10, 270, 380, 20, $"<BASEFONT COLOR=#FFFFFF><B>Rewards:</BASEFONT></BASEFONT></BASEFONT>", false, false);
            
            if (m_Quest.Rewards != null)
            {
                int i = 0;
                foreach (var reward in m_Quest.Rewards)
                {
                    AddHtml(10, 300 + (i * 25), 380, 20, $"<BASEFONT COLOR=#FFFFFF>• {reward.Description}</BASEFONT>", false, false);
                    i++;
                }
            }

            // Accept/Decline buttons
            AddButton(20, 330, 4005, 4013, 1, GumpButtonType.Reply, 0);
            AddHtml(55, 330, 300, 20, "<BASEFONT COLOR=#FFFFFF>Accept Quest</BASEFONT>", false, false);

            AddButton(20, 360, 4005, 4013, 2, GumpButtonType.Reply, 0);
            AddHtml(55, 360, 300, 20, "<BASEFONT COLOR=#FFFFFF>Decline Quest</BASEFONT>", false, false);

            AddHtml(10, 390, 380, 20, $"<BASEFONT COLOR=#FFFFFF>Difficulty: {m_Quest.Metadata?.GetValueOrDefault("vystia_quest_difficulty", "unknown")}</BASEFONT>", false, false);
        }
    }

        public override void OnResponse(NetworkNetState sender, RelayInfo info)
        {
            switch (info.ButtonID)
            {
                case 1: // Accept Quest
                    AcceptQuest();
                    break;
                case 2: // Decline Quest
                    DeclineQuest();
                    break;
            }
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            // Handle response
        }

        private void AcceptQuest()
        {
            if (m_From is PlayerMobile player)
            {
                VystiaQuestSystem.CompleteVystiaQuest(player, m_Quest);
                m_From.CloseGump(this);
            }
        }

        private void DeclineQuest()
        {
            m_From.SendMessage($"You have declined the {m_Quest.Title} quest.");
            m_From.CloseGump(this);
        }
    }
}
