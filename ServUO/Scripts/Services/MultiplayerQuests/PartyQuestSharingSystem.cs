using System;
using System.Collections.Generic;
using System.Linq;
using Server.Mobiles;
using Server.Custom.VystiaClasses.Quests;
using Server.Engines.PartySystem;
using Server.Services.QuestJournal;
using Server.Services.QuestPersistence;

namespace Server.Services.MultiplayerQuests
{
    /// <summary>
    /// Manages quest sharing between party members
    /// </summary>
    public static class PartyQuestSharingSystem
    {
        private static readonly Dictionary<int, SharedQuestInfo> s_SharedQuests = new Dictionary<int, SharedQuestInfo>();
        private static readonly Dictionary<Serial, List<int>> s_PlayerSharedQuests = new Dictionary<Serial, List<int>>();
        private static readonly object s_Lock = new object();

        /// <summary>
        /// Initialize the party quest sharing system
        /// </summary>
        public static void Initialize()
        {
            Console.WriteLine("[PartyQuestSharing] Initialized multiplayer quest sharing system");
        }

        /// <summary>
        /// Share a quest with party members
        /// </summary>
        public static bool ShareQuestWithParty(PlayerMobile sharer, int questId, SharedQuestSettings settings = null)
        {
            if (sharer == null)
                return false;

            var party = sharer.Party as Party;
            if (party == null)
            {
                sharer.SendMessage("You must be in a party to share quests.");
                return false;
            }

            var quest = GetQuestById(questId);
            if (quest == null)
            {
                sharer.SendMessage("Quest not found.");
                return false;
            }

            // Validate party size
            if (party.Members.Count < (settings?.MinimumPartySize ?? 2))
            {
                sharer.SendMessage($"Party must have at least {settings?.MinimumPartySize ?? 2} members to share quests.");
                return false;
            }

            if (party.Members.Count > (settings?.MaximumPartySize ?? 8))
            {
                sharer.SendMessage($"Party cannot have more than {settings?.MaximumPartySize ?? 8} members for shared quests.");
                return false;
            }

            // Check if quest is already shared with this party
            if (IsQuestSharedWithParty(questId, party))
            {
                sharer.SendMessage("This quest is already shared with your party.");
                return false;
            }

            lock (s_Lock)
            {
                var sharedQuest = new SharedQuestInfo
                {
                    QuestId = questId,
                    QuestTitle = GetQuestTitle(quest),
                    QuestDescription = GetQuestDescription(quest),
                    SharedAt = DateTime.UtcNow,
                    SharedBy = sharer,
                    Party = party,
                    IsActive = true,
                    IsCompleted = false,
                    MemberProgress = new List<PartyMemberProgress>(),
                    CooperativeObjectives = GenerateCooperativeObjectives(quest),
                    Settings = settings ?? new SharedQuestSettings()
                };

                // Initialize progress for all party members
                foreach (var member in party.Members)
                {
                    if (member.Mobile != null && !member.Mobile.Deleted)
                    {
                        var memberProgress = new PartyMemberProgress
                        {
                            Member = member.Mobile,
                            JoinedAt = DateTime.UtcNow,
                            HasAccepted = member.Mobile == sharer, // Sharer auto-accepts
                            AcceptedAt = member.Mobile == sharer ? DateTime.UtcNow : DateTime.MinValue,
                            ObjectiveProgress = new Dictionary<string, int>(),
                            HasCompleted = false,
                            ContributionScore = 0,
                            ActionsPerformed = new List<string>()
                        };

                        // Initialize objective progress
                        foreach (var objective in sharedQuest.CooperativeObjectives)
                        {
                            memberProgress.ObjectiveProgress[objective.ObjectiveId] = 0;
                        }

                        sharedQuest.MemberProgress.Add(memberProgress);

                        // Track player's shared quests
                        if (!s_PlayerSharedQuests.ContainsKey(member.Mobile.Serial))
                        {
                            s_PlayerSharedQuests[member.Mobile.Serial] = new List<int>();
                        }
                        s_PlayerSharedQuests[member.Mobile.Serial].Add(questId);
                    }
                }

                s_SharedQuests[questId] = sharedQuest;

                // Send quest sharing notification
                SendQuestSharingNotification(sharedQuest);

                // Log the sharing
                Console.WriteLine($"[PartyQuestSharing] {sharer.Name} shared quest '{sharedQuest.QuestTitle}' with party of {party.Members.Count} members");

                return true;
            }
        }

        /// <summary>
        /// Accept a shared quest
        /// </summary>
        public static bool AcceptSharedQuest(PlayerMobile player, int questId)
        {
            if (player == null)
                return false;

            lock (s_Lock)
            {
                if (!s_SharedQuests.TryGetValue(questId, out var sharedQuest))
                {
                    player.SendMessage("Shared quest not found.");
                    return false;
                }

                var memberProgress = sharedQuest.MemberProgress.FirstOrDefault(mp => mp.Member == player);
                if (memberProgress == null)
                {
                    player.SendMessage("You are not a member of this shared quest.");
                    return false;
                }

                if (memberProgress.HasAccepted)
                {
                    player.SendMessage("You have already accepted this quest.");
                    return false;
                }

                memberProgress.HasAccepted = true;
                memberProgress.AcceptedAt = DateTime.UtcNow;

                // Send acceptance notification
                SendQuestAcceptanceNotification(sharedQuest, player);

                // Check if all required members have accepted
                if (sharedQuest.Settings.RequireAllAcceptance)
                {
                    var allAccepted = sharedQuest.MemberProgress.All(mp => mp.HasAccepted);
                    if (allAccepted)
                    {
                        SendAllMembersAcceptedNotification(sharedQuest);
                    }
                }

                Console.WriteLine($"[PartyQuestSharing] {player.Name} accepted shared quest '{sharedQuest.QuestTitle}'");
                return true;
            }
        }

        /// <summary>
        /// Decline a shared quest
        /// </summary>
        public static bool DeclineSharedQuest(PlayerMobile player, int questId)
        {
            if (player == null)
                return false;

            lock (s_Lock)
            {
                if (!s_SharedQuests.TryGetValue(questId, out var sharedQuest))
                {
                    player.SendMessage("Shared quest not found.");
                    return false;
                }

                var memberProgress = sharedQuest.MemberProgress.FirstOrDefault(mp => mp.Member == player);
                if (memberProgress == null)
                {
                    player.SendMessage("You are not a member of this shared quest.");
                    return false;
                }

                // Remove member from the shared quest
                sharedQuest.MemberProgress.Remove(memberProgress);
                s_PlayerSharedQuests[player.Serial].Remove(questId);

                // Send decline notification
                SendQuestDeclineNotification(sharedQuest, player);

                // Check if minimum party size is still met
                if (sharedQuest.MemberProgress.Count < sharedQuest.Settings.MinimumPartySize)
                {
                    // Cancel the shared quest
                    CancelSharedQuest(sharedQuest, "Insufficient party members after decline");
                }

                Console.WriteLine($"[PartyQuestSharing] {player.Name} declined shared quest '{sharedQuest.QuestTitle}'");
                return true;
            }
        }

        /// <summary>
        /// Get all shared quests for a player
        /// </summary>
        public static List<SharedQuestInfo> GetPlayerSharedQuests(PlayerMobile player)
        {
            if (player == null)
                return new List<SharedQuestInfo>();

            lock (s_Lock)
            {
                var sharedQuestIds = s_PlayerSharedQuests.GetValueOrDefault(player.Serial, new List<int>());
                return sharedQuestIds.Select(id => s_SharedQuests.GetValueOrDefault(id))
                    .Where(q => q != null && q.IsActive)
                    .ToList();
            }
        }

        /// <summary>
        /// Get shared quest by ID
        /// </summary>
        public static SharedQuestInfo GetSharedQuest(int questId)
        {
            lock (s_Lock)
            {
                return s_SharedQuests.GetValueOrDefault(questId);
            }
        }

        /// <summary>
        /// Check if a quest is shared with a party
        /// </summary>
        public static bool IsQuestSharedWithParty(int questId, Party party)
        {
            if (party == null)
                return false;

            lock (s_Lock)
            {
                return s_SharedQuests.Values.Any(sq => sq.QuestId == questId && sq.Party == party);
            }
        }

        /// <summary>
        /// Handle party member leaving
        /// </summary>
        public static void HandlePartyMemberLeave(PlayerMobile member, Party party)
        {
            if (member == null || party == null)
                return;

            lock (s_Lock)
            {
                var memberSharedQuests = s_PlayerSharedQuests.GetValueOrDefault(member.Serial, new List<int>());
                var questsToLeave = new List<int>();

                foreach (var questId in memberSharedQuests)
                {
                    if (s_SharedQuests.TryGetValue(questId, out var sharedQuest) && sharedQuest.Party == party)
                    {
                        // Remove member from the shared quest
                        var memberProgress = sharedQuest.MemberProgress.FirstOrDefault(mp => mp.Member == member);
                        if (memberProgress != null)
                        {
                            sharedQuest.MemberProgress.Remove(memberProgress);
                        }

                        questsToLeave.Add(questId);

                        // Check if minimum party size is still met
                        if (sharedQuest.MemberProgress.Count < sharedQuest.Settings.MinimumPartySize)
                        {
                            // Cancel the shared quest
                            CancelSharedQuest(sharedQuest, "Insufficient party members after member left");
                        }
                        else if (!sharedQuest.Settings.AllowLateJoiners)
                        {
                            // Mark as incomplete for the leaving member
                            memberProgress.HasCompleted = false;
                        }

                        SendMemberLeftNotification(sharedQuest, member);
                    }
                }

                // Remove quests from player's list
                foreach (var questId in questsToLeave)
                {
                    s_PlayerSharedQuests[member.Serial].Remove(questId);
                }

                Console.WriteLine($"[PartyQuestSharing] {member.Name} left {questsToLeave.Count} shared quests");
            }
        }

        /// <summary>
        /// Handle new party member joining
        /// </summary>
        public static void HandlePartyMemberJoin(PlayerMobile member, Party party)
        {
            if (member == null || party == null)
                return;

            lock (s_Lock)
            {
                var partySharedQuests = s_SharedQuests.Values.Where(sq => sq.Party == party && sq.IsActive).ToList();

                foreach (var sharedQuest in partySharedQuests)
                {
                    if (sharedQuest.Settings.AllowLateJoiners)
                    {
                        // Add member to existing shared quest
                        var memberProgress = new PartyMemberProgress
                        {
                            Member = member,
                            JoinedAt = DateTime.UtcNow,
                            HasAccepted = false,
                            ObjectiveProgress = new Dictionary<string, int>(),
                            HasCompleted = false,
                            ContributionScore = 0,
                            ActionsPerformed = new List<string>()
                        };

                        // Initialize objective progress
                        foreach (var objective in sharedQuest.CooperativeObjectives)
                        {
                            memberProgress.ObjectiveProgress[objective.ObjectiveId] = 0;
                        }

                        sharedQuest.MemberProgress.Add(memberProgress);

                        // Track player's shared quests
                        if (!s_PlayerSharedQuests.ContainsKey(member.Serial))
                        {
                            s_PlayerSharedQuests[member.Serial] = new List<int>();
                        }
                        s_PlayerSharedQuests[member.Serial].Add(sharedQuest.QuestId);

                        SendMemberJoinedNotification(sharedQuest, member);
                    }
                }

                Console.WriteLine($"[PartyQuestSharing] {member.Name} joined {partySharedQuests.Count} shared quests");
            }
        }

        /// <summary>
        /// Cancel a shared quest
        /// </summary>
        private static void CancelSharedQuest(SharedQuestInfo sharedQuest, string reason)
        {
            sharedQuest.IsActive = false;

            // Remove from all players' shared quest lists
            foreach (var memberProgress in sharedQuest.MemberProgress)
            {
                s_PlayerSharedQuests[memberProgress.Member.Serial].Remove(sharedQuest.QuestId);
            }

            // Remove from shared quests
            s_SharedQuests.Remove(sharedQuest.QuestId);

            // Send cancellation notification
            SendQuestCancellationNotification(sharedQuest, reason);

            Console.WriteLine($"[PartyQuestSharing] Shared quest '{sharedQuest.QuestTitle}' cancelled: {reason}");
        }

        /// <summary>
        /// Generate cooperative objectives for a quest
        /// </summary>
        private static List<CooperativeObjective> GenerateCooperativeObjectives(object quest)
        {
            var objectives = new List<CooperativeObjective>();

            // This would be implemented based on the quest type and content
            // For now, return some default cooperative objectives

            objectives.Add(new CooperativeObjective
            {
                ObjectiveId = "cooperative_1",
                Description = "Complete the quest together as a party",
                Type = CooperativeObjectiveType.GroupContribution,
                RequiredCount = 1,
                CurrentProgress = 0,
                RequiredRoles = new List<string> { "leader", "member" },
                RoleContributions = new Dictionary<string, int>(),
                Settings = new CooperativeObjectiveSettings()
            });

            return objectives;
        }

        /// <summary>
        /// Get quest by ID (supports multiple quest types)
        /// </summary>
        private static object GetQuestById(int questId)
        {
            // Try DynamicQuest first
            var dynamicQuest = DynamicQuestManager.GetQuest(questId);
            if (dynamicQuest != null)
                return dynamicQuest;

            // Try VystiaQuest
            var vystiaQuest = VystiaQuestSystem.GetQuest(questId);
            if (vystiaQuest != null)
                return vystiaQuest;

            // Try traditional quest
            // This would need to be implemented based on the traditional quest system

            return null;
        }

        /// <summary>
        /// Get quest title
        /// </summary>
        private static string GetQuestTitle(object quest)
        {
            if (quest is DynamicQuest dynamicQuest)
                return dynamicQuest.Title;
            
            if (quest is VystiaQuest vystiaQuest)
                return vystiaQuest.Title;

            return "Unknown Quest";
        }

        /// <summary>
        /// Get quest description
        /// </summary>
        private static string GetQuestDescription(object quest)
        {
            if (quest is DynamicQuest dynamicQuest)
                return dynamicQuest.Description;
            
            if (quest is VystiaQuest vystiaQuest)
                return vystiaQuest.Description;

            return "Unknown quest description";
        }

        /// <summary>
        /// Send quest sharing notification
        /// </summary>
        private static void SendQuestSharingNotification(SharedQuestInfo sharedQuest)
        {
            foreach (var memberProgress in sharedQuest.MemberProgress)
            {
                if (memberProgress.Member != sharedQuest.SharedBy)
                {
                    memberProgress.Member.SendMessage($"{sharedQuest.SharedBy.Name} has shared quest '{sharedQuest.QuestTitle}' with the party!");
                    memberProgress.Member.SendMessage($"Type [AcceptQuest {sharedQuest.QuestId}] to accept or [DeclineQuest {sharedQuest.QuestId}] to decline.");
                }
            }
        }

        /// <summary>
        /// Send quest acceptance notification
        /// </summary>
        private static void SendQuestAcceptanceNotification(SharedQuestInfo sharedQuest, PlayerMobile accepter)
        {
            foreach (var memberProgress in sharedQuest.MemberProgress)
            {
                if (memberProgress.Member != accepter)
                {
                    memberProgress.Member.SendMessage($"{accepter.Name} has accepted the shared quest '{sharedQuest.QuestTitle}'!");
                }
            }
        }

        /// <summary>
        /// Send quest decline notification
        /// </summary>
        private static void SendQuestDeclineNotification(SharedQuestInfo sharedQuest, PlayerMobile decliner)
        {
            foreach (var memberProgress in sharedQuest.MemberProgress)
            {
                if (memberProgress.Member != decliner)
                {
                    memberProgress.Member.SendMessage($"{decliner.Name} has declined the shared quest '{sharedQuest.QuestTitle}'.");
                }
            }
        }

        /// <summary>
        /// Send all members accepted notification
        /// </summary>
        private static void SendAllMembersAcceptedNotification(SharedQuestInfo sharedQuest)
        {
            foreach (var memberProgress in sharedQuest.MemberProgress)
            {
                memberProgress.Member.SendMessage($"All party members have accepted '{sharedQuest.QuestTitle}'! The quest is now active.");
            }
        }

        /// <summary>
        /// Send member left notification
        /// </summary>
        private static void SendMemberLeftNotification(SharedQuestInfo sharedQuest, PlayerMobile leaver)
        {
            foreach (var memberProgress in sharedQuest.MemberProgress)
            {
                if (memberProgress.Member != leaver)
                {
                    memberProgress.Member.SendMessage($"{leaver.Name} has left the shared quest '{sharedQuest.QuestTitle}'.");
                }
            }
        }

        /// <summary>
        /// Send member joined notification
        /// </summary>
        private static void SendMemberJoinedNotification(SharedQuestInfo sharedQuest, PlayerMobile joiner)
        {
            foreach (var memberProgress in sharedQuest.MemberProgress)
            {
                if (memberProgress.Member != joiner)
                {
                    memberProgress.Member.SendMessage($"{joiner.Name} has joined the shared quest '{sharedQuest.QuestTitle}'!");
                }
            }
        }

        /// <summary>
        /// Send quest cancellation notification
        /// </summary>
        private static void SendQuestCancellationNotification(SharedQuestInfo sharedQuest, string reason)
        {
            foreach (var memberProgress in sharedQuest.MemberProgress)
            {
                memberProgress.Member.SendMessage($"The shared quest '{sharedQuest.QuestTitle}' has been cancelled: {reason}");
            }
        }

        /// <summary>
        /// Get sharing statistics
        /// </summary>
        public static PartyQuestStatistics GetStatistics()
        {
            lock (s_Lock)
            {
                var stats = new PartyQuestStatistics
                {
                    TotalSharedQuests = s_SharedQuests.Count,
                    CompletedQuests = s_SharedQuests.Values.Count(q => q.IsCompleted),
                    ActiveQuests = s_SharedQuests.Values.Count(q => q.IsActive),
                    LastActivity = DateTime.UtcNow
                };

                if (stats.TotalSharedQuests > 0)
                {
                    stats.CompletionRate = (double)stats.CompletedQuests / stats.TotalSharedQuests;
                }

                return stats;
            }
        }
    }
}
