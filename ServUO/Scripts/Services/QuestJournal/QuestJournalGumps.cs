using System;
using System.Collections.Generic;
using Server.Mobiles;
using Server.Gumps;
using Server.Network;
using Server.Custom.VystiaClasses.Quests;
using Server.Custom.VystiaClasses.Quests.Generation;

namespace Server.Services.QuestJournal
{
    /// <summary>
    /// Gump for selecting quest filters
    /// </summary>
    public class QuestFilterGump : Gump
    {
        private readonly PlayerMobile m_Player;
        private readonly UnifiedQuestJournal m_ParentGump;

        public QuestFilterGump(PlayerMobile player, UnifiedQuestJournal parentGump) : base(200, 200)
        {
            m_Player = player;
            m_ParentGump = parentGump;

            Closable = true;
            Disposable = true;
            Dragable = true;
            Resizable = false;

            Build();
        }

        private void Build()
        {
            AddPage(0);

            // Background
            AddBackground(0, 0, 300, 250, 9270);
            AddAlphaRegion(10, 10, 280, 230);

            // Title
            AddHtml(10, 10, 280, 20, Center(Color("Quest Filter", 0x00FFFF)), false, false);

            // Filter options
            int y = 40;
            AddButton(20, y, 0xFAF, 0xFAE, (int)QuestFilterType.All, GumpButtonType.Reply, 0);
            AddHtml(25, y + 4, 250, 20, Color("All Quests", 0xFFFFFF), false, false);
            y += 25;

            AddButton(20, y, 0xFAF, 0xFAE, (int)QuestFilterType.Active, GumpButtonType.Reply, 0);
            AddHtml(25, y + 4, 250, 20, Color("Active Quests", 0xFFFFFF), false, false);
            y += 25;

            AddButton(20, y, 0xFAF, 0xFAE, (int)QuestFilterType.Completed, GumpButtonType.Reply, 0);
            AddHtml(25, y + 4, 250, 20, Color("Completed Quests", 0xFFFFFF), false, false);
            y += 25;

            AddButton(20, y, 0xFAF, 0xFAE, (int)QuestFilterType.Vystia, GumpButtonType.Reply, 0);
            AddHtml(25, y + 4, 250, 20, Color("Vystia Quests", 0x00FFFF), false, false);
            y += 25;

            AddButton(20, y, 0xFAF, 0xFAE, (int)QuestFilterType.Dynamic, GumpButtonType.Reply, 0);
            AddHtml(25, y + 4, 250, 20, Color("Dynamic Quests", 0xFF00FF), false, false);
            y += 25;

            AddButton(20, y, 0xFAF, 0xFAE, (int)QuestFilterType.Traditional, GumpButtonType.Reply, 0);
            AddHtml(25, y + 4, 250, 20, Color("Traditional Quests", 0xFF8800), false, false);
            y += 25;

            AddButton(20, y, 0xFAF, 0xFAE, (int)QuestFilterType.ByDifficulty, GumpButtonType.Reply, 0);
            AddHtml(25, y + 4, 250, 20, Color("By Difficulty", 0xFFFFFF), false, false);
            y += 25;

            AddButton(20, y, 0xFAF, 0xFAE, (int)QuestFilterType.ByLocation, GumpButtonType.Reply, 0);
            AddHtml(25, y + 4, 250, 20, Color("By Location", 0xFFFFFF), false, false);

            // Close button
            AddButton(110, 210, 0xFAB, 0xFAC, 0, GumpButtonType.Reply, 0);
            AddHtml(115, 214, 70, 20, Center(Color("Close", 0xFFFFFF)), false, false);
        }

        public override void OnResponse(NetState state, RelayInfo info)
        {
            int buttonID = info.ButtonID;

            if (buttonID >= 0 && buttonID <= (int)QuestFilterType.ByLocation)
            {
                var filterType = (QuestFilterType)buttonID;
                m_Player.SendGump(new UnifiedQuestJournal(m_Player, filterType, QuestSortType.ByProgress, string.Empty, 0, false));
            }
        }

        private string Color(string text, int color)
        {
            return $"<BASEFONT COLOR=#{color:X}>{text}</BASEFONT>";
        }

        private string Center(string text)
        {
            return $"<CENTER>{text}</CENTER>";
        }
    }

    /// <summary>
    /// Gump for selecting quest sorting options
    /// </summary>
    public class QuestSortGump : Gump
    {
        private readonly PlayerMobile m_Player;
        private readonly UnifiedQuestJournal m_ParentGump;

        public QuestSortGump(PlayerMobile player, UnifiedQuestJournal parentGump) : base(200, 200)
        {
            m_Player = player;
            m_ParentGump = parentGump;

            Closable = true;
            Disposable = true;
            Dragable = true;
            Resizable = false;

            Build();
        }

        private void Build()
        {
            AddPage(0);

            // Background
            AddBackground(0, 0, 300, 280, 9270);
            AddAlphaRegion(10, 10, 280, 260);

            // Title
            AddHtml(10, 10, 280, 20, Center(Color("Quest Sort", 0x00FFFF)), false, false);

            // Sort options
            int y = 40;
            AddButton(20, y, 0xFAF, 0xFAE, (int)QuestSortType.ByName, GumpButtonType.Reply, 0);
            AddHtml(25, y + 4, 250, 20, Color("By Name", 0xFFFFFF), false, false);
            y += 25;

            AddButton(20, y, 0xFAF, 0xFAE, (int)QuestSortType.ByDifficulty, GumpButtonType.Reply, 0);
            AddHtml(25, y + 4, 250, 20, Color("By Difficulty", 0xFFFFFF), false, false);
            y += 25;

            AddButton(20, y, 0xFAF, 0xFAE, (int)QuestSortType.ByProgress, GumpButtonType.Reply, 0);
            AddHtml(25, y + 4, 250, 20, Color("By Progress", 0xFFFFFF), false, false);
            y += 25;

            AddButton(20, y, 0xFAF, 0xFAE, (int)QuestSortType.ByType, GumpButtonType.Reply, 0);
            AddHtml(25, y + 4, 250, 20, Color("By Type", 0xFFFFFF), false, false);
            y += 25;

            AddButton(20, y, 0xFAF, 0xFAE, (int)QuestSortType.ByLocation, GumpButtonType.Reply, 0);
            AddHtml(25, y + 4, 250, 20, Color("By Location", 0xFFFFFF), false, false);
            y += 25;

            AddButton(20, y, 0xFAF, 0xFAE, (int)QuestSortType.ByTimeRemaining, GumpButtonType.Reply, 0);
            AddHtml(25, y + 4, 250, 20, Color("By Time Remaining", 0xFFFFFF), false, false);
            y += 25;

            AddButton(20, y, 0xFAF, 0xFAE, (int)QuestSortType.ByDateStarted, GumpButtonType.Reply, 0);
            AddHtml(25, y + 4, 250, 20, Color("By Date Started", 0xFFFFFF), false, false);

            // Close button
            AddButton(110, 240, 0xFAB, 0xFAC, 0, GumpButtonType.Reply, 0);
            AddHtml(115, 244, 70, 20, Center(Color("Close", 0xFFFFFF)), false, false);
        }

        public override void OnResponse(NetState state, RelayInfo info)
        {
            int buttonID = info.ButtonID;

            if (buttonID >= 0 && buttonID <= (int)QuestSortType.ByDateStarted)
            {
                var sortType = (QuestSortType)buttonID;
                m_Player.SendGump(new UnifiedQuestJournal(m_Player, QuestFilterType.Active, sortType, string.Empty, 0, false));
            }
        }

        private string Color(string text, int color)
        {
            return $"<BASEFONT COLOR=#{color:X}>{text}</BASEFONT>";
        }

        private string Center(string text)
        {
            return $"<CENTER>{text}</CENTER>";
        }
    }

    /// <summary>
    /// Gump for searching quests
    /// </summary>
    public class QuestSearchGump : Gump
    {
        private readonly PlayerMobile m_Player;
        private readonly UnifiedQuestJournal m_ParentGump;
        private int m_SearchEntryID;

        public QuestSearchGump(PlayerMobile player, UnifiedQuestJournal parentGump) : base(250, 250)
        {
            m_Player = player;
            m_ParentGump = parentGump;

            Closable = true;
            Disposable = true;
            Dragable = true;
            Resizable = false;

            Build();
        }

        private void Build()
        {
            AddPage(0);

            // Background
            AddBackground(0, 0, 350, 200, 9270);
            AddAlphaRegion(10, 10, 330, 180);

            // Title
            AddHtml(10, 10, 330, 20, Center(Color("Search Quests", 0x00FFFF)), false, false);

            // Search field
            AddLabel(20, 50, 310, "Enter search term:");
            m_SearchEntryID = AddTextEntry(20, 75, 310, 20, 0, "", 20);

            // Search button
            AddButton(125, 105, 0xFAB, 0xFAC, 1, GumpButtonType.Reply, 0);
            AddHtml(130, 109, 90, 20, Center(Color("Search", 0xFFFFFF)), false, false);

            // Clear button
            AddButton(20, 105, 0xFAD, 0xFAE, 2, GumpButtonType.Reply, 0);
            AddHtml(25, 109, 70, 20, Center(Color("Clear", 0xFFFFFF)), false, false);

            // Close button
            AddButton(135, 160, 0xFAB, 0xFAC, 0, GumpButtonType.Reply, 0);
            AddHtml(140, 164, 70, 20, Center(Color("Close", 0xFFFFFF)), false, false);
        }

        public override void OnResponse(NetState state, RelayInfo info)
        {
            int buttonID = info.ButtonID;

            switch (buttonID)
            {
                case 0: // Close
                    break;

                case 1: // Search
                    string searchTerm = info.TextEntries[0].Text;
                    m_Player.SendGump(new UnifiedQuestJournal(m_Player, QuestFilterType.Active, QuestSortType.ByProgress, searchTerm, 0, false));
                    break;

                case 2: // Clear
                    m_Player.SendGump(new UnifiedQuestJournal(m_Player, QuestFilterType.Active, QuestSortType.ByProgress, string.Empty, 0, false));
                    break;
            }
        }

        private string Color(string text, int color)
        {
            return $"<BASEFONT COLOR=#{color:X}>{text}</BASEFONT>";
        }

        private string Center(string text)
        {
            return $"<CENTER>{text}</CENTER>";
        }
    }

    /// <summary>
    /// Gump for displaying detailed quest information
    /// </summary>
    public class QuestDetailsGump : Gump
    {
        private readonly PlayerMobile m_Player;
        private readonly UnifiedQuestInfo m_Quest;

        public QuestDetailsGump(PlayerMobile player, UnifiedQuestInfo quest) : base(150, 150)
        {
            m_Player = player;
            m_Quest = quest;

            Closable = true;
            Disposable = true;
            Dragable = true;
            Resizable = false;

            Build();
        }

        private void Build()
        {
            AddPage(0);

            // Background
            AddBackground(0, 0, 500, 400, 9270);
            AddAlphaRegion(10, 10, 480, 380);

            // Title
            AddHtml(10, 10, 480, 30, Center(Color(m_Quest.Title, 0x00FFFF)), false, false);

            // Quest type and difficulty
            AddHtml(10, 45, 480, 20, Color($"{m_Quest.QuestType} Quest - {m_Quest.Difficulty}", 0xFFFFFF), false, false);

            // Description
            AddHtml(10, 75, 480, 60, Color(m_Quest.Description, 0xFFFFFF), false, false);

            // Location
            AddHtml(10, 140, 480, 20, Color($"Location: {m_Quest.Location}", 0xFFFFFF), false, false);

            // Progress section
            AddHtml(10, 170, 480, 20, Color("Progress:", 0x00FF00), false, false);
            QuestProgressRenderer.RenderProgressBar(this, 20, 195, 460, 25, m_Quest.Progress);

            // Objectives
            if (m_Quest.Objectives != null && m_Quest.Objectives.Count > 0)
            {
                QuestProgressRenderer.RenderObjectives(this, 20, 230, 460, m_Quest.Objectives);
            }

            // Rewards
            if (m_Quest.Rewards != null && m_Quest.Rewards.Count > 0)
            {
                int rewardsY = 230 + (m_Quest.Objectives?.Count ?? 0) * 25;
                QuestProgressRenderer.RenderRewards(this, 20, rewardsY, 460, m_Quest.Rewards);
            }

            // Time information
            if (m_Quest.TimeRemaining.HasValue)
            {
                QuestProgressRenderer.RenderTimeRemaining(this, 20, 350, m_Quest);
            }

            // Action buttons
            int buttonY = 380;
            if (m_Quest.CanAbandon && m_Quest.IsActive)
            {
                AddButton(350, buttonY, 0xFA1, 0xFA2, 1, GumpButtonType.Reply, 0);
                AddHtml(355, buttonY + 4, 110, 20, Center(Color("Abandon Quest", 0xFF0000)), false, false);
            }

            AddButton(20, buttonY, 0xFAB, 0xFAC, 0, GumpButtonType.Reply, 0);
            AddHtml(25, buttonY + 4, 110, 20, Center(Color("Close", 0xFFFFFF)), false, false);
        }

        public override void OnResponse(NetState state, RelayInfo info)
        {
            int buttonID = info.ButtonID;

            switch (buttonID)
            {
                case 0: // Close
                    break;

                case 1: // Abandon quest
                    AbandonQuest();
                    break;
            }
        }

        private void AbandonQuest()
        {
            // Implementation would depend on the quest type
            if (m_Quest.QuestType == "Vystia")
            {
                VystiaQuestSystem.RemoveQuestFromAllPlayers(m_Quest.QuestId);
                m_Player.SendMessage($"You have abandoned the quest: {m_Quest.Title}");
            }
            else if (m_Quest.QuestType == "Dynamic")
            {
                var attachment = GeneratedQuestInstanceAttachment.Get(m_Player);
                attachment?.RemoveInstancesForQuest(m_Quest.QuestId);
                m_Player.SendMessage($"You have abandoned the quest: {m_Quest.Title}");
            }
        }

        private string Color(string text, int color)
        {
            return $"<BASEFONT COLOR=#{color:X}>{text}</BASEFONT>";
        }

        private string Center(string text)
        {
            return $"<CENTER>{text}</CENTER>";
        }
    }
}
