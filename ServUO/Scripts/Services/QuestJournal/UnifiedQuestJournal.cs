using System;
using System.Collections.Generic;
using System.Linq;
using Server.Mobiles;
using Server.Gumps;
using Server.Network;
using Server.Custom.VystiaClasses.Quests;
using Server.Custom.VystiaClasses.Quests.Generation;
using Server.Services.QuestPersistence;

namespace Server.Services.QuestJournal
{
    /// <summary>
    /// Unified quest journal interface that consolidates all quest types
    /// Provides consistent user experience across Vystia, Dynamic, and Traditional quests
    /// </summary>
    public class UnifiedQuestJournal : Gump
    {
        private readonly PlayerMobile m_Player;
        private readonly List<UnifiedQuestInfo> m_Quests;
        private readonly QuestFilterType m_CurrentFilter;
        private readonly QuestSortType m_CurrentSort;
        private readonly string m_SearchTerm;
        private readonly int m_CurrentPage;
        private readonly int m_ItemsPerPage;
        private readonly bool m_ShowCompleted;

        // UI Constants
        private const int WIDTH = 600;
        private const int HEIGHT = 500;
        private const int HEADER_HEIGHT = 80;
        private const int FOOTER_HEIGHT = 60;
        private const int QUEST_HEIGHT = 100;
        private const int PADDING = 10;

        public UnifiedQuestJournal(PlayerMobile player) : base(100, 100)
        {
            m_Player = player;
            m_CurrentFilter = QuestFilterType.Active;
            m_CurrentSort = QuestSortType.ByProgress;
            m_SearchTerm = string.Empty;
            m_CurrentPage = 0;
            m_ItemsPerPage = 4;
            m_ShowCompleted = false;

            Closable = true;
            Disposable = true;
            Dragable = true;
            Resizable = false;

            m_Quests = LoadQuests();
            Build();
        }

        public UnifiedQuestJournal(PlayerMobile player, QuestFilterType filter, QuestSortType sort, string searchTerm, int page, bool showCompleted) : base(100, 100)
        {
            m_Player = player;
            m_CurrentFilter = filter;
            m_CurrentSort = sort;
            m_SearchTerm = searchTerm ?? string.Empty;
            m_CurrentPage = page;
            m_ItemsPerPage = 4;
            m_ShowCompleted = showCompleted;

            Closable = true;
            Disposable = true;
            Dragable = true;
            Resizable = false;

            m_Quests = LoadQuests();
            Build();
        }

        private void Build()
        {
            AddPage(0);

            // Background
            AddBackground(0, 0, WIDTH, HEIGHT, 9270);
            AddAlphaRegion(PADDING, PADDING, WIDTH - PADDING * 2, HEIGHT - PADDING * 2);

            // Header
            BuildHeader();

            // Filter and Sort Controls
            BuildControls();

            // Quest List
            BuildQuestList();

            // Footer
            BuildFooter();
        }

        private void BuildHeader()
        {
            // Title
            AddHtml(PADDING, PADDING, WIDTH - PADDING * 2, 30, Center(Color("Unified Quest Journal", 0x00FFFF)), false, false);
            
            // Player info
            AddHtml(PADDING, 40, WIDTH - PADDING * 2, 20, Center(Color($"Player: {m_Player.Name}", 0xFFFFFF)), false, false);
            
            // Stats
            var activeCount = m_Quests.Count(q => q.IsActive);
            var completedCount = m_Quests.Count(q => q.IsCompleted);
            AddHtml(PADDING, 60, WIDTH - PADDING * 2, 20, Center(Color($"Active: {activeCount} | Completed: {completedCount} | Total: {m_Quests.Count}", 0xFFFFFF)), false, false);
        }

        private void BuildControls()
        {
            int y = HEADER_HEIGHT + PADDING;

            // Filter buttons
            AddButton(PADDING, y, 0xFAB, 0xFAD, 1, GumpButtonType.Reply, 0);
            AddHtml(PADDING + 5, y + 4, 110, 20, Center(Color("Filter", 0xFFFFFF)), false, false);

            AddButton(PADDING + 130, y, 0xFAF, 0xFAE, 2, GumpButtonType.Reply, 0);
            AddHtml(PADDING + 135, y + 4, 110, 20, Center(Color("Sort", 0xFFFFFF)), false, false);

            AddButton(PADDING + 260, y, 0xFAD, 0xFAD, 3, GumpButtonType.Reply, 0);
            AddHtml(PADDING + 265, y + 4, 110, 20, Center(Color("Search", 0xFFFFFF)), false, false);

            // Current filter/sort display
            y += 25;
            string filterText = $"Filter: {m_CurrentFilter} | Sort: {m_CurrentSort}";
            if (!string.IsNullOrEmpty(m_SearchTerm))
                filterText += $" | Search: '{m_SearchTerm}'";
            
            AddHtml(PADDING, y, WIDTH - PADDING * 2, 20, Color(filterText, 0xFFFFFF), false, false);
        }

        private void BuildQuestList()
        {
            int y = HEADER_HEIGHT + 70;
            int startIndex = m_CurrentPage * m_ItemsPerPage;
            int endIndex = Math.Min(startIndex + m_ItemsPerPage, m_Quests.Count);

            for (int i = startIndex; i < endIndex; i++)
            {
                var quest = m_Quests[i];
                BuildQuestEntry(quest, y, i);
                y += QUEST_HEIGHT + PADDING;
            }

            // Page navigation
            if (m_Quests.Count > m_ItemsPerPage)
            {
                BuildPagination(y);
            }
        }

        private void BuildQuestEntry(UnifiedQuestInfo quest, int y, int questIndex)
        {
            // Quest background
            AddImageTiled(PADDING + 10, y, WIDTH - PADDING * 2 - 20, QUEST_HEIGHT, 0x242F);
            
            // Quest type indicator
            string typeColor = GetQuestTypeColor(quest.QuestType);
            AddHtml(PADDING + 15, y + 5, 80, 20, Color(quest.QuestType, typeColor), false, false);

            // Quest title
            AddHtml(PADDING + 100, y + 5, WIDTH - 200, 20, Color(quest.Title, 0xFFFFFF), false, false);

            // Status
            QuestProgressRenderer.RenderStatus(this, PADDING + 15, y + 30, quest);

            // Progress bar
            QuestProgressRenderer.RenderProgressBar(this, PADDING + 15, y + 50, WIDTH - PADDING * 2 - 30, 15, quest.Progress);

            // Location and difficulty
            string infoText = $"Location: {quest.Location} | {quest.Difficulty}";
            AddHtml(PADDING + 15, y + 70, WIDTH - PADDING * 2 - 30, 20, Color(infoText, 0xFFFFFF), false, false);

            // Time remaining (if applicable)
            if (quest.TimeRemaining.HasValue)
            {
                QuestProgressRenderer.RenderTimeRemaining(this, PADDING + 15, y + 90, quest);
            }

            // Action buttons
            int buttonY = y + 30;
            AddButton(WIDTH - PADDING - 80, buttonY, 0xFAE, 0xFAF, 100 + questIndex, GumpButtonType.Reply, 0);
            AddHtml(WIDTH - PADDING - 75, buttonY + 4, 60, 20, Center(Color("Details", 0xFFFFFF)), false, false);

            if (quest.CanAbandon && quest.IsActive)
            {
                AddButton(WIDTH - PADDING - 80, buttonY + 25, 0xFA1, 0xFA2, 200 + questIndex, GumpButtonType.Reply, 0);
                AddHtml(WIDTH - PADDING - 75, buttonY + 29, 60, 20, Center(Color("Abandon", 0xFF0000)), false, false);
            }
        }

        private void BuildPagination(int y)
        {
            int totalPages = (int)Math.Ceiling((double)m_Quests.Count / m_ItemsPerPage);
            
            AddHtml(PADDING, y, WIDTH - PADDING * 2, 20, Center(Color($"Page {m_CurrentPage + 1} of {totalPages}", 0xFFFFFF)), false, false);

            // Previous button
            if (m_CurrentPage > 0)
            {
                AddButton(WIDTH - 150, y, 0xFAB, 0xFAC, 1000, GumpButtonType.Reply, 0);
                AddHtml(WIDTH - 145, y + 4, 50, 20, Center(Color("Previous", 0xFFFFFF)), false, false);
            }

            // Next button
            if (m_CurrentPage < totalPages - 1)
            {
                AddButton(WIDTH - 80, y, 0xFAF, 0xFAE, 1001, GumpButtonType.Reply, 0);
                AddHtml(WIDTH - 75, y + 4, 50, 20, Center(Color("Next", 0xFFFFFF)), false, false);
            }
        }

        private void BuildFooter()
        {
            int y = HEIGHT - FOOTER_HEIGHT - PADDING;

            // Close button
            AddButton(WIDTH - 80, y, 0xFAB, 0xFAC, 0, GumpButtonType.Reply, 0);
            AddHtml(WIDTH - 75, y + 4, 60, 20, Center(Color("Close", 0xFFFFFF)), false, false);

            // Refresh button
            AddButton(PADDING, y, 0xFAD, 0xFAE, 999, GumpButtonType.Reply, 0);
            AddHtml(PADDING + 5, y + 4, 60, 20, Center(Color("Refresh", 0xFFFFFF)), false, false);
        }

        private List<UnifiedQuestInfo> LoadQuests()
        {
            var quests = new List<UnifiedQuestInfo>();

            // Load Vystia quests
            var vystiaAdapter = new VystiaQuestAdapter();
            // TODO: Replace with actual method to get all Vystia quests
            // var vystiaQuests = VystiaQuestSystem.GetAllQuests();
            var vystiaQuests = new List<VystiaQuest>(); // Placeholder
            foreach (var quest in vystiaQuests)
            {
                var unifiedQuest = new UnifiedQuestInfo
                {
                    QuestId = vystiaAdapter.GetQuestId(quest),
                    Title = vystiaAdapter.GetTitle(quest),
                    Description = vystiaAdapter.GetDescription(quest),
                    QuestType = vystiaAdapter.GetQuestType(quest),
                    Difficulty = vystiaAdapter.GetDifficulty(quest),
                    Location = vystiaAdapter.GetLocation(quest),
                    IsActive = vystiaAdapter.IsActive(quest, m_Player),
                    IsCompleted = vystiaAdapter.IsCompleted(quest, m_Player),
                    Progress = vystiaAdapter.GetProgress(quest, m_Player),
                    Objectives = vystiaAdapter.GetObjectives(quest, m_Player),
                    Rewards = vystiaAdapter.GetRewards(quest),
                    CanAbandon = vystiaAdapter.CanAbandon(quest, m_Player),
                    OriginalQuest = quest,
                    Adapter = vystiaAdapter
                };
                quests.Add(unifiedQuest);
            }

            // Load Dynamic quests
            var dynamicAdapter = new DynamicQuestAdapter();
            var attachment = GeneratedQuestInstanceAttachment.Get(m_Player);
            if (attachment != null)
            {
                foreach (var instance in attachment.Instances)
                {
                    var quest = DynamicQuestManager.GetQuest(instance.QuestId);
                    if (quest != null)
                    {
                        var unifiedQuest = new UnifiedQuestInfo
                        {
                            QuestId = dynamicAdapter.GetQuestId(quest),
                            Title = dynamicAdapter.GetTitle(quest),
                            Description = dynamicAdapter.GetDescription(quest),
                            QuestType = dynamicAdapter.GetQuestType(quest),
                            Difficulty = dynamicAdapter.GetDifficulty(quest),
                            Location = dynamicAdapter.GetLocation(quest),
                            IsActive = dynamicAdapter.IsActive(quest, m_Player),
                            IsCompleted = dynamicAdapter.IsCompleted(quest, m_Player),
                            Progress = dynamicAdapter.GetProgress(quest, m_Player),
                            Objectives = dynamicAdapter.GetObjectives(quest, m_Player),
                            Rewards = dynamicAdapter.GetRewards(quest),
                            CanAbandon = dynamicAdapter.CanAbandon(quest, m_Player),
                            OriginalQuest = quest,
                            Adapter = dynamicAdapter
                        };
                        quests.Add(unifiedQuest);
                    }
                }
            }

            // Load Traditional quests (placeholder - would need actual implementation)
            var traditionalAdapter = new TraditionalQuestAdapter();
            // Traditional quests would need to be loaded from ServUO's quest system
            // This is a placeholder for demonstration

            // Apply filters
            quests = QuestFilterSystem.FilterQuests(quests, m_CurrentFilter, null, m_Player);
            
            // Apply search
            if (!string.IsNullOrEmpty(m_SearchTerm))
            {
                quests = QuestFilterSystem.SearchQuests(quests, m_SearchTerm);
            }
            
            // Apply sorting
            quests = QuestSortManager.SortQuests(quests, m_CurrentSort, false);

            return quests;
        }

        private int GetQuestTypeColor(string questType)
        {
            switch (questType)
            {
                case "Vystia": return 0x00FFFF;
                case "Dynamic": return 0xFF00FF;
                case "Traditional": return 0xFF8800;
                default: return 0xFFFFFF;
            }
        }

        public override void OnResponse(NetState state, RelayInfo info)
        {
            int buttonID = info.ButtonID;

            switch (buttonID)
            {
                case 0: // Close
                    break;

                case 1: // Filter
                    m_Player.SendGump(new QuestFilterGump(m_Player, this));
                    break;

                case 2: // Sort
                    m_Player.SendGump(new QuestSortGump(m_Player, this));
                    break;

                case 3: // Search
                    m_Player.SendGump(new QuestSearchGump(m_Player, this));
                    break;

                case 999: // Refresh
                    m_Player.SendGump(new UnifiedQuestJournal(m_Player));
                    break;

                default:
                    if (buttonID >= 100 && buttonID < 200) // Quest details
                    {
                        int questIndex = buttonID - 100;
                        if (questIndex >= 0 && questIndex < m_Quests.Count)
                        {
                            var quest = m_Quests[questIndex];
                            m_Player.SendGump(new QuestDetailsGump(m_Player, quest));
                        }
                    }
                    else if (buttonID >= 200 && buttonID < 300) // Abandon quest
                    {
                        int questIndex = buttonID - 200;
                        if (questIndex >= 0 && questIndex < m_Quests.Count)
                        {
                            var quest = m_Quests[questIndex];
                            AbandonQuest(quest);
                            m_Player.SendGump(new UnifiedQuestJournal(m_Player));
                        }
                    }
                    else if (buttonID == 1000) // Previous page
                    {
                        if (m_CurrentPage > 0)
                        {
                            m_Player.SendGump(new UnifiedQuestJournal(m_Player, m_CurrentFilter, m_CurrentSort, m_SearchTerm, m_CurrentPage - 1, m_ShowCompleted));
                        }
                    }
                    else if (buttonID == 1001) // Next page
                    {
                        int totalPages = (int)Math.Ceiling((double)m_Quests.Count / m_ItemsPerPage);
                        if (m_CurrentPage < totalPages - 1)
                        {
                            m_Player.SendGump(new UnifiedQuestJournal(m_Player, m_CurrentFilter, m_CurrentSort, m_SearchTerm, m_CurrentPage + 1, m_ShowCompleted));
                        }
                    }
                    break;
            }
        }

        private void AbandonQuest(UnifiedQuestInfo quest)
        {
            // Implementation would depend on the quest type
            if (quest.QuestType == "Vystia")
            {
                VystiaQuestSystem.RemoveQuestFromAllPlayers(quest.QuestId);
                m_Player.SendMessage($"You have abandoned the quest: {quest.Title}");
            }
            else if (quest.QuestType == "Dynamic")
            {
                // Remove from GeneratedQuestInstanceAttachment
                var attachment = GeneratedQuestInstanceAttachment.Get(m_Player);
                attachment?.RemoveInstancesForQuest(quest.QuestId);
                m_Player.SendMessage($"You have abandoned the quest: {quest.Title}");
            }
            // Traditional quests would need their own abandon logic
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
