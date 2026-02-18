using System;
using System.Collections.Generic;
using System.Linq;
using Server.Mobiles;
using Server.Engines.PartySystem;
using Server.Network;
using Server.Gumps;

namespace Server.Services.MultiplayerQuests
{
    /// <summary>
    /// Manages communication for multiplayer quests
    /// </summary>
    public static class QuestCommunicationSystem
    {
        private static readonly Dictionary<int, List<QuestCommunication>> s_QuestCommunications = new Dictionary<int, List<QuestCommunication>>();
        private static readonly Dictionary<Serial, List<int>> s_PlayerSubscriptions = new Dictionary<Serial, List<int>>();
        private static readonly object s_Lock = new object();

        /// <summary>
        /// Initialize the quest communication system
        /// </summary>
        public static void Initialize()
        {
            Console.WriteLine("[QuestCommunicationSystem] Initialized quest communication system");
        }

        /// <summary>
        /// Send a quest communication message
        /// </summary>
        public static bool SendMessage(QuestCommunication message)
        {
            if (message == null)
                return false;

            lock (s_Lock)
            {
                // Store the message
                if (!s_QuestCommunications.ContainsKey(message.SharedQuestId))
                {
                    s_QuestCommunications[message.SharedQuestId] = new List<QuestCommunication>();
                }
                s_QuestCommunications[message.SharedQuestId].Add(message);

                // Send to recipients
                foreach (var recipient in message.Recipients)
                {
                    if (recipient != null && !recipient.Deleted)
                    {
                        SendQuestMessageToPlayer(recipient, message);
                    }
                }

                // Log the communication
                Console.WriteLine($"[QuestCommunication] {message.Sender.Name} sent {message.Type} message for quest {message.SharedQuestId}");

                return true;
            }
        }

        /// <summary>
        /// Send quest sharing notification
        /// </summary>
        public static bool SendQuestSharedNotification(SharedQuestInfo sharedQuest)
        {
            if (sharedQuest == null)
                return false;

            var message = new QuestCommunication
            {
                SharedQuestId = sharedQuest.QuestId,
                Sender = sharedQuest.SharedBy,
                Message = $"I've shared the quest '{sharedQuest.QuestTitle}' with our party!",
                Type = QuestMessageType.QuestShared,
                SentAt = DateTime.UtcNow,
                Recipients = sharedQuest.MemberProgress.Select(mp => mp.Member).Where(m => m != sharedQuest.SharedBy).ToList(),
                Metadata = new Dictionary<string, object>
                {
                    ["QuestId"] = sharedQuest.QuestId,
                    ["QuestTitle"] = sharedQuest.QuestTitle,
                    ["SharedBy"] = sharedQuest.SharedBy.Name,
                    ["PartySize"] = sharedQuest.MemberProgress.Count
                }
            };

            return SendMessage(message);
        }

        /// <summary>
        /// Send quest acceptance notification
        /// </summary>
        public static bool SendQuestAcceptedNotification(SharedQuestInfo sharedQuest, PlayerMobile accepter)
        {
            if (sharedQuest == null || accepter == null)
                return false;

            var message = new QuestCommunication
            {
                SharedQuestId = sharedQuest.QuestId,
                Sender = accepter,
                Message = $"I've accepted the shared quest '{sharedQuest.QuestTitle}'!",
                Type = QuestMessageType.QuestAccepted,
                SentAt = DateTime.UtcNow,
                Recipients = sharedQuest.MemberProgress.Select(mp => mp.Member).Where(m => m != accepter).ToList(),
                Metadata = new Dictionary<string, object>
                {
                    ["QuestId"] = sharedQuest.QuestId,
                    ["QuestTitle"] = sharedQuest.QuestTitle,
                    ["AcceptedBy"] = accepter.Name,
                    ["AcceptedAt"] = DateTime.UtcNow
                }
            };

            return SendMessage(message);
        }

        /// <summary>
        /// Send quest decline notification
        /// </summary>
        public static bool SendQuestDeclinedNotification(SharedQuestInfo sharedQuest, PlayerMobile decliner)
        {
            if (sharedQuest == null || decliner == null)
                return false;

            var message = new QuestCommunication
            {
                SharedQuestId = sharedQuest.QuestId,
                Sender = decliner,
                Message = $"I've declined the shared quest '{sharedQuest.QuestTitle}'.",
                Type = QuestMessageType.QuestDeclined,
                SentAt = DateTime.UtcNow,
                Recipients = sharedQuest.MemberProgress.Select(mp => mp.Member).Where(m => m != decliner).ToList(),
                Metadata = new Dictionary<string, object>
                {
                    ["QuestId"] = sharedQuest.QuestId,
                    ["QuestTitle"] = sharedQuest.QuestTitle,
                    ["DeclinedBy"] = decliner.Name,
                    ["DeclinedAt"] = DateTime.UtcNow
                }
            };

            return SendMessage(message);
        }

        /// <summary>
        /// Send objective progress notification
        /// </summary>
        public static bool SendObjectiveProgressNotification(SharedQuestInfo sharedQuest, string objectiveId, PlayerMobile player, int progress)
        {
            if (sharedQuest == null || player == null)
                return false;

            var objective = sharedQuest.CooperativeObjectives.FirstOrDefault(o => o.ObjectiveId == objectiveId);
            if (objective == null)
                return false;

            var message = new QuestCommunication
            {
                SharedQuestId = sharedQuest.QuestId,
                Sender = player,
                Message = $"I've made progress on '{objective.Description}': {progress}/{objective.RequiredCount}",
                Type = QuestMessageType.ObjectiveProgress,
                SentAt = DateTime.UtcNow,
                Recipients = sharedQuest.MemberProgress.Select(mp => mp.Member).Where(m => m != player).ToList(),
                Metadata = new Dictionary<string, object>
                {
                    ["QuestId"] = sharedQuest.QuestId,
                    ["ObjectiveId"] = objectiveId,
                    ["ObjectiveDescription"] = objective.Description,
                    ["Progress"] = progress,
                    ["Required"] = objective.RequiredCount,
                    ["Player"] = player.Name
                }
            };

            return SendMessage(message);
        }

        /// <summary>
        /// Send objective completion notification
        /// </summary>
        public static bool SendObjectiveCompletedNotification(SharedQuestInfo sharedQuest, string objectiveId)
        {
            if (sharedQuest == null)
                return false;

            var objective = sharedQuest.CooperativeObjectives.FirstOrDefault(o => o.ObjectiveId == objectiveId);
            if (objective == null)
                return false;

            var message = new QuestCommunication
            {
                SharedQuestId = sharedQuest.QuestId,
                Sender = null, // System message
                Message = $"Objective completed: {objective.Description}",
                Type = QuestMessageType.ObjectiveCompleted,
                SentAt = DateTime.UtcNow,
                Recipients = sharedQuest.MemberProgress.Select(mp => mp.Member).ToList(),
                Metadata = new Dictionary<string, object>
                {
                    ["QuestId"] = sharedQuest.QuestId,
                    ["ObjectiveId"] = objectiveId,
                    ["ObjectiveDescription"] = objective.Description,
                    ["CompletedAt"] = objective.CompletedAt
                }
            };

            return SendMessage(message);
        }

        /// <summary>
        /// Send quest completion notification
        /// </summary>
        public static bool SendQuestCompletedNotification(SharedQuestInfo sharedQuest)
        {
            if (sharedQuest == null)
                return false;

            var message = new QuestCommunication
            {
                SharedQuestId = sharedQuest.QuestId,
                Sender = null, // System message
                Message = $"Quest completed: {sharedQuest.QuestTitle}",
                Type = QuestMessageType.QuestCompleted,
                SentAt = DateTime.UtcNow,
                Recipients = sharedQuest.MemberProgress.Select(mp => mp.Member).ToList(),
                Metadata = new Dictionary<string, object>
                {
                    ["QuestId"] = sharedQuest.QuestId,
                    ["QuestTitle"] = sharedQuest.QuestTitle,
                    ["CompletedAt"] = sharedQuest.CompletedAt,
                    ["Duration"] = (sharedQuest.CompletedAt - sharedQuest.SharedAt).TotalMinutes
                }
            };

            return SendMessage(message);
        }

        /// <summary>
        /// Send member joined notification
        /// </summary>
        public static bool SendMemberJoinedNotification(SharedQuestInfo sharedQuest, PlayerMobile newMember)
        {
            if (sharedQuest == null || newMember == null)
                return false;

            var message = new QuestCommunication
            {
                SharedQuestId = sharedQuest.QuestId,
                Sender = newMember,
                Message = $"I've joined the shared quest '{sharedQuest.QuestTitle}'!",
                Type = QuestMessageType.MemberJoined,
                SentAt = DateTime.UtcNow,
                Recipients = sharedQuest.MemberProgress.Select(mp => mp.Member).Where(m => m != newMember).ToList(),
                Metadata = new Dictionary<string, object>
                {
                    ["QuestId"] = sharedQuest.QuestId,
                    ["QuestTitle"] = sharedQuest.QuestTitle,
                    ["JoinedBy"] = newMember.Name,
                    ["JoinedAt"] = DateTime.UtcNow
                }
            };

            return SendMessage(message);
        }

        /// <summary>
        /// Send member left notification
        /// </summary>
        public static bool SendMemberLeftNotification(SharedQuestInfo sharedQuest, PlayerMobile leavingMember)
        {
            if (sharedQuest == null || leavingMember == null)
                return false;

            var message = new QuestCommunication
            {
                SharedQuestId = sharedQuest.QuestId,
                Sender = leavingMember,
                Message = $"I've left the shared quest '{sharedQuest.QuestTitle}'.",
                Type = QuestMessageType.MemberLeft,
                SentAt = DateTime.UtcNow,
                Recipients = sharedQuest.MemberProgress.Select(mp => mp.Member).Where(m => m != leavingMember).ToList(),
                Metadata = new Dictionary<string, object>
                {
                    ["QuestId"] = sharedQuest.QuestId,
                    ["QuestTitle"] = sharedQuest.QuestTitle,
                    ["LeftBy"] = leavingMember.Name,
                    ["LeftAt"] = DateTime.UtcNow
                }
            };

            return SendMessage(message);
        }

        /// <summary>
        /// Send coordination request
        /// </summary>
        public static bool SendCoordinationRequest(SharedQuestInfo sharedQuest, PlayerMobile requester, string request)
        {
            if (sharedQuest == null || requester == null)
                return false;

            var message = new QuestCommunication
            {
                SharedQuestId = sharedQuest.QuestId,
                Sender = requester,
                Message = request,
                Type = QuestMessageType.CoordinationRequest,
                SentAt = DateTime.UtcNow,
                Recipients = sharedQuest.MemberProgress.Select(mp => mp.Member).Where(m => m != requester).ToList(),
                Metadata = new Dictionary<string, object>
                {
                    ["QuestId"] = sharedQuest.QuestId,
                    ["QuestTitle"] = sharedQuest.QuestTitle,
                    ["Requester"] = requester.Name,
                    ["Request"] = request,
                    ["RequestedAt"] = DateTime.UtcNow
                }
            };

            return SendMessage(message);
        }

        /// <summary>
        /// Send status update
        /// </summary>
        public static bool SendStatusUpdate(SharedQuestInfo sharedQuest, PlayerMobile player, string status)
        {
            if (sharedQuest == null || player == null)
                return false;

            var message = new QuestCommunication
            {
                SharedQuestId = sharedQuest.QuestId,
                Sender = player,
                Message = status,
                Type = QuestMessageType.StatusUpdate,
                SentAt = DateTime.UtcNow,
                Recipients = sharedQuest.MemberProgress.Select(mp => mp.Member).Where(m => m != player).ToList(),
                Metadata = new Dictionary<string, object>
                {
                    ["QuestId"] = sharedQuest.QuestId,
                    ["QuestTitle"] = sharedQuest.QuestTitle,
                    ["Player"] = player.Name,
                    ["Status"] = status,
                    ["UpdatedAt"] = DateTime.UtcNow
                }
            };

            return SendMessage(message);
        }

        /// <summary>
        /// Get communications for a quest
        /// </summary>
        public static List<QuestCommunication> GetQuestCommunications(int questId, int maxCount = 50)
        {
            lock (s_Lock)
            {
                var communications = s_QuestCommunications.GetValueOrDefault(questId, new List<QuestCommunication>());
                return communications.TakeLast(maxCount).ToList();
            }
        }

        /// <summary>
        /// Get player's quest communications
        /// </summary>
        public static List<QuestCommunication> GetPlayerCommunications(PlayerMobile player, int maxCount = 50)
        {
            if (player == null)
                return new List<QuestCommunication>();

            lock (s_Lock)
            {
                var questIds = s_PlayerSubscriptions.GetValueOrDefault(player.Serial, new List<int>());
                var communications = new List<QuestCommunication>();

                foreach (var questId in questIds)
                {
                    var questCommunications = s_QuestCommunications.GetValueOrDefault(questId, new List<QuestCommunication>());
                    communications.AddRange(questCommunications.Where(c => c.Recipients.Contains(player)));
                }

                return communications.OrderByDescending(c => c.SentAt).Take(maxCount).ToList();
            }
        }

        /// <summary>
        /// Subscribe player to quest communications
        /// </summary>
        public static void SubscribeToQuest(PlayerMobile player, int questId)
        {
            if (player == null)
                return;

            lock (s_Lock)
            {
                if (!s_PlayerSubscriptions.ContainsKey(player.Serial))
                {
                    s_PlayerSubscriptions[player.Serial] = new List<int>();
                }

                if (!s_PlayerSubscriptions[player.Serial].Contains(questId))
                {
                    s_PlayerSubscriptions[player.Serial].Add(questId);
                }
            }
        }

        /// <summary>
        /// Unsubscribe player from quest communications
        /// </summary>
        public static void UnsubscribeFromQuest(PlayerMobile player, int questId)
        {
            if (player == null)
                return;

            lock (s_Lock)
            {
                if (s_PlayerSubscriptions.ContainsKey(player.Serial))
                {
                    s_PlayerSubscriptions[player.Serial].Remove(questId);
                }
            }
        }

        /// <summary>
        /// Send quest message to player
        /// </summary>
        private static void SendQuestMessageToPlayer(PlayerMobile player, QuestCommunication message)
        {
            if (player == null || message == null)
                return;

            // Format the message based on type
            string formattedMessage = FormatQuestMessage(message);

            // Send as system message or gump
            if (message.Type == QuestMessageType.QuestShared || message.Type == QuestMessageType.QuestCompleted)
            {
                // Use gump for important messages
                player.SendGump(new QuestCommunicationGump(message));
            }
            else
            {
                // Use regular chat for other messages
                player.SendMessage(0x3B2, formattedMessage);
            }
        }

        /// <summary>
        /// Format quest message for display
        /// </summary>
        private static string FormatQuestMessage(QuestCommunication message)
        {
            var prefix = GetMessageTypePrefix(message.Type);
            var sender = message.Sender?.Name ?? "System";
            var timestamp = message.SentAt.ToString("HH:mm");

            return $"[{timestamp}] {prefix} {sender}: {message.Message}";
        }

        /// <summary>
        /// Get message type prefix
        /// </summary>
        private static string GetMessageTypePrefix(QuestMessageType type)
        {
            switch (type)
            {
                case QuestMessageType.QuestShared:
                    return "[QUEST]";
                case QuestMessageType.QuestAccepted:
                    return "[ACCEPT]";
                case QuestMessageType.QuestDeclined:
                    return "[DECLINE]";
                case QuestMessageType.ObjectiveProgress:
                    return "[PROGRESS]";
                case QuestMessageType.ObjectiveCompleted:
                    return "[COMPLETE]";
                case QuestMessageType.QuestCompleted:
                    return "[COMPLETED]";
                case QuestMessageType.MemberJoined:
                    return "[JOINED]";
                case QuestMessageType.MemberLeft:
                    return "[LEFT]";
                case QuestMessageType.CoordinationRequest:
                    return "[COORD]";
                case QuestMessageType.StatusUpdate:
                    return "[STATUS]";
                default:
                    return "[QUEST]";
            }
        }

        /// <summary>
        /// Get communication statistics
        /// </summary>
        public static CommunicationStatistics GetStatistics()
        {
            lock (s_Lock)
            {
                var stats = new CommunicationStatistics
                {
                    TotalMessages = s_QuestCommunications.Values.Sum(list => list.Count),
                    TotalSubscriptions = s_PlayerSubscriptions.Values.Sum(list => list.Count),
                    MessageTypes = new Dictionary<QuestMessageType, int>(),
                    AverageMessagesPerQuest = 0,
                    LastMessage = DateTime.UtcNow
                };

                // Count message types
                foreach (var communications in s_QuestCommunications.Values)
                {
                    foreach (var message in communications)
                    {
                        stats.MessageTypes[message.Type] = stats.MessageTypes.GetValueOrDefault(message.Type, 0) + 1;
                    }
                }

                if (stats.TotalMessages > 0 && s_QuestCommunications.Count > 0)
                {
                    stats.AverageMessagesPerQuest = (double)stats.TotalMessages / s_QuestCommunications.Count;
                }

                return stats;
            }
        }
    }

    /// <summary>
    /// Quest communication gump for displaying messages
    /// </summary>
    public class QuestCommunicationGump : Gump
    {
        private readonly QuestCommunication m_Message;

        public QuestCommunicationGump(QuestCommunication message) : base(100, 100)
        {
            m_Message = message;

            Closable = true;
            Disposable = true;
            Dragable = true;
            Resizable = false;

            BuildGump();
        }

        private void BuildGump()
        {
            // Background
            AddBackground(0, 0, 400, 200, 9270);
            AddAlphaRegion(10, 10, 380, 180);

            // Title
            AddHtml(10, 10, 380, 20, Center(Color("Quest Communication", 0x00FFFF)), false, false);

            // Message
            var formattedMessage = FormatQuestMessage(m_Message);
            AddHtml(10, 40, 380, 100, Color(formattedMessage, 0xFFFFFF), false, false);

            // Metadata (if any)
            if (m_Message.Metadata != null && m_Message.Metadata.Any())
            {
                int y = 150;
                AddHtml(10, y, 380, 20, Color("Details:", 0x00FF00), false, false);
                y += 25;

                foreach (var kvp in m_Message.Metadata)
                {
                    AddHtml(10, y, 380, 20, Color($"{kvp.Key}: {kvp.Value}", 0xFFFFFF), false, false);
                    y += 20;
                }
            }

            // Close button
            AddButton(170, 170, 0xFAB, 0xFAC, 0, GumpButtonType.Reply, 0);
            AddHtml(175, 174, 60, 20, Center(Color("Close", 0xFFFFFF)), false, false);
        }

        public override void OnResponse(NetState state, RelayInfo info)
        {
            // Just close the gump
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
    /// Communication statistics
    /// </summary>
    public class CommunicationStatistics
    {
        public int TotalMessages { get; set; }
        public int TotalSubscriptions { get; set; }
        public Dictionary<QuestMessageType, int> MessageTypes { get; set; }
        public double AverageMessagesPerQuest { get; set; }
        public DateTime LastMessage { get; set; }
    }
}
