using System;
using System.Collections.Generic;
using System.Linq;
using Server.Mobiles;
using Server.Custom.VystiaClasses.Quests;
using Server.Engines.PartySystem;
using Server.Services.QuestPersistence;

namespace Server.Services.MultiplayerQuests
{
    /// <summary>
    /// Tracks and synchronizes progress for shared multiplayer quests
    /// </summary>
    public static class SharedProgressTracker
    {
        private static readonly Dictionary<int, SharedQuestProgress> s_QuestProgress = new Dictionary<int, SharedQuestProgress>();
        private static readonly Dictionary<Serial, List<int>> s_PlayerQuestProgress = new Dictionary<Serial, List<int>>();
        private static readonly object s_Lock = new object();

        /// <summary>
        /// Initialize the shared progress tracker
        /// </summary>
        public static void Initialize()
        {
            Console.WriteLine("[SharedProgressTracker] Initialized shared progress tracking system");
        }

        /// <summary>
        /// Create progress tracking for a shared quest
        /// </summary>
        public static void CreateSharedProgress(SharedQuestInfo sharedQuest)
        {
            if (sharedQuest == null)
                return;

            lock (s_Lock)
            {
                var progress = new SharedQuestProgress
                {
                    QuestId = sharedQuest.QuestId,
                    Party = sharedQuest.Party,
                    StartedAt = DateTime.UtcNow,
                    LastUpdated = DateTime.UtcNow,
                    MemberProgress = new Dictionary<Serial, MemberProgressData>(),
                    ObjectiveProgress = new Dictionary<string, ObjectiveProgressData>(),
                    IsCompleted = false,
                    CompletionTime = DateTime.MinValue
                };

                // Initialize member progress
                foreach (var memberProgress in sharedQuest.MemberProgress)
                {
                    if (memberProgress.Member != null && !memberProgress.Member.Deleted)
                    {
                        progress.MemberProgress[memberProgress.Member.Serial] = new MemberProgressData
                        {
                            Member = memberProgress.Member,
                            JoinedAt = memberProgress.JoinedAt,
                            AcceptedAt = memberProgress.AcceptedAt,
                            HasAccepted = memberProgress.HasAccepted,
                            ObjectiveProgress = new Dictionary<string, int>(),
                            TotalContribution = 0,
                            ActionsCount = 0,
                            LastActivity = DateTime.UtcNow
                        };

                        // Initialize objective progress for this member
                        foreach (var objective in sharedQuest.CooperativeObjectives)
                        {
                            progress.MemberProgress[memberProgress.Member.Serial].ObjectiveProgress[objective.ObjectiveId] = 0;
                        }

                        // Track player's quest progress
                        if (!s_PlayerQuestProgress.ContainsKey(memberProgress.Member.Serial))
                        {
                            s_PlayerQuestProgress[memberProgress.Member.Serial] = new List<int>();
                        }
                        s_PlayerQuestProgress[memberProgress.Member.Serial].Add(sharedQuest.QuestId);
                    }
                }

                // Initialize objective progress
                foreach (var objective in sharedQuest.CooperativeObjectives)
                {
                    progress.ObjectiveProgress[objective.ObjectiveId] = new ObjectiveProgressData
                    {
                        ObjectiveId = objective.ObjectiveId,
                        Description = objective.Description,
                        Type = objective.Type,
                        RequiredCount = objective.RequiredCount,
                        CurrentProgress = 0,
                        ProgressByMember = new Dictionary<Serial, int>(),
                        ContributionByRole = new Dictionary<string, int>(),
                        StartedAt = DateTime.UtcNow,
                        CompletedAt = DateTime.MinValue,
                        IsCompleted = false
                    };
                }

                s_QuestProgress[sharedQuest.QuestId] = progress;
            }
        }

        /// <summary>
        /// Update member progress
        /// </summary>
        public static bool UpdateMemberProgress(int questId, PlayerMobile player, string objectiveId, int amount)
        {
            if (player == null)
                return false;

            lock (s_Lock)
            {
                if (!s_QuestProgress.TryGetValue(questId, out var progress))
                    return false;

                if (!progress.MemberProgress.TryGetValue(player.Serial, out var memberData))
                    return false;

                if (!memberData.HasAccepted)
                    return false;

                // Update member's objective progress
                int currentProgress = memberData.ObjectiveProgress.GetValueOrDefault(objectiveId, 0);
                int newProgress = Math.Min(currentProgress + amount, progress.ObjectiveProgress[objectiveId].RequiredCount);
                memberData.ObjectiveProgress[objectiveId] = newProgress;

                // Update member statistics
                memberData.TotalContribution += amount;
                memberData.ActionsCount++;
                memberData.LastActivity = DateTime.UtcNow;

                // Update objective progress
                UpdateObjectiveProgress(progress, objectiveId, player.Serial, amount);

                // Update quest last updated time
                progress.LastUpdated = DateTime.UtcNow;

                // Sync progress to other party members
                SyncProgressToParty(progress, player, objectiveId, amount);

                // Check for completion
                CheckObjectiveCompletion(progress, objectiveId);

                return true;
            }
        }

        /// <summary>
        /// Update objective progress
        /// </summary>
        private static void UpdateObjectiveProgress(SharedQuestProgress progress, string objectiveId, Serial playerSerial, int amount)
        {
            if (!progress.ObjectiveProgress.TryGetValue(objectiveId, out var objectiveData))
                return;

            // Update total progress
            objectiveData.CurrentProgress = Math.Min(objectiveData.CurrentProgress + amount, objectiveData.RequiredCount);

            // Update member contribution
            objectiveData.ProgressByMember[playerSerial] = objectiveData.ProgressByMember.GetValueOrDefault(playerSerial, 0) + amount;

            // Update role contribution if applicable
            if (progress.MemberProgress.TryGetValue(playerSerial, out var memberData))
            {
                string role = DeterminePlayerRole(memberData.Member, objectiveData);
                objectiveData.ContributionByRole[role] = objectiveData.ContributionByRole.GetValueOrDefault(role, 0) + amount;
            }
        }

        /// <summary>
        /// Check if an objective is completed
        /// </summary>
        private static void CheckObjectiveCompletion(SharedQuestProgress progress, string objectiveId)
        {
            if (!progress.ObjectiveProgress.TryGetValue(objectiveId, out var objectiveData))
                return;

            bool isCompleted = false;

            switch (objectiveData.Type)
            {
                case CooperativeObjectiveType.IndividualContribution:
                    // Check if any member has completed their individual requirement
                    isCompleted = progress.MemberProgress.Values.Any(mp =>
                        mp.ObjectiveProgress.GetValueOrDefault(objectiveId, 0) >= objectiveData.RequiredCount);
                    break;

                case CooperativeObjectiveType.GroupContribution:
                    // Check if total party contribution meets requirement
                    int totalProgress = progress.MemberProgress.Values.Sum(mp =>
                        mp.ObjectiveProgress.GetValueOrDefault(objectiveId, 0));
                    isCompleted = totalProgress >= objectiveData.RequiredCount;
                    break;

                case CooperativeObjectiveType.RoleBased:
                    // Check if all required roles have completed their contributions
                    var requiredRoles = GetRequiredRolesForObjective(progress, objectiveId);
                    isCompleted = requiredRoles.All(role =>
                        objectiveData.ContributionByRole.GetValueOrDefault(role, 0) >= objectiveData.RequiredCount);
                    break;

                case CooperativeObjectiveType.AllMembers:
                    // Check if all members have completed
                    isCompleted = progress.MemberProgress.Values.All(mp =>
                        mp.ObjectiveProgress.GetValueOrDefault(objectiveId, 0) >= objectiveData.RequiredCount);
                    break;

                case CooperativeObjectiveType.LeaderOnly:
                    // Check if party leader has completed
                    var party = progress.Party as Party;
                    if (party != null && party.Leader != null)
                    {
                        var leaderProgress = progress.MemberProgress.GetValueOrDefault(party.Leader.Serial);
                        if (leaderProgress != null)
                        {
                            isCompleted = leaderProgress.ObjectiveProgress.GetValueOrDefault(objectiveId, 0) >= objectiveData.RequiredCount;
                        }
                    }
                    break;
            }

            if (isCompleted && !objectiveData.IsCompleted)
            {
                objectiveData.IsCompleted = true;
                objectiveData.CompletedAt = DateTime.UtcNow;

                // Send objective completion notification
                SendObjectiveCompletionNotification(progress, objectiveId);

                // Check if all objectives are completed
                CheckQuestCompletion(progress);
            }
        }

        /// <summary>
        /// Check if the entire quest is completed
        /// </summary>
        private static void CheckQuestCompletion(SharedQuestProgress progress)
        {
            if (progress.ObjectiveProgress.Values.All(obj => obj.IsCompleted))
            {
                progress.IsCompleted = true;
                progress.CompletionTime = DateTime.UtcNow;

                // Mark all participating members as completed
                foreach (var memberData in progress.MemberProgress.Values)
                {
                    if (memberData.HasAccepted)
                    {
                        memberData.HasCompleted = true;
                        memberData.CompletedAt = DateTime.UtcNow;
                    }
                }

                // Send quest completion notification
                SendQuestCompletionNotification(progress);

                Console.WriteLine($"[SharedProgressTracker] Shared quest {progress.QuestId} completed by party");
            }
        }

        /// <summary>
        /// Sync progress to party members
        /// </summary>
        private static void SyncProgressToParty(SharedQuestProgress progress, PlayerMobile player, string objectiveId, int amount)
        {
            var party = progress.Party as Party;
            if (party == null)
                return;

            foreach (var member in party.Members)
            {
                if (member.Mobile != null && member.Mobile != player)
                {
                    var memberData = progress.MemberProgress.GetValueOrDefault(member.Mobile.Serial);
                    if (memberData != null && memberData.HasAccepted)
                    {
                        int currentProgress = memberData.ObjectiveProgress.GetValueOrDefault(objectiveId, 0);
                        int requiredProgress = progress.ObjectiveProgress[objectiveId].RequiredCount;
                        
                        member.Mobile.SendMessage($"{player.Name} contributed {amount} to objective ({currentProgress}/{requiredProgress})");
                    }
                }
            }
        }

        /// <summary>
        /// Get progress for a shared quest
        /// </summary>
        public static SharedQuestProgress GetQuestProgress(int questId)
        {
            lock (s_Lock)
            {
                return s_QuestProgress.GetValueOrDefault(questId);
            }
        }

        /// <summary>
        /// Get player's progress on a shared quest
        /// </summary>
        public static MemberProgressData GetPlayerProgress(int questId, PlayerMobile player)
        {
            if (player == null)
                return null;

            lock (s_Lock)
            {
                var progress = s_QuestProgress.GetValueOrDefault(questId);
                return progress?.MemberProgress.GetValueOrDefault(player.Serial);
            }
        }

        /// <summary>
        /// Get all shared quests for a player
        /// </summary>
        public static List<SharedQuestProgress> GetPlayerSharedQuests(PlayerMobile player)
        {
            if (player == null)
                return new List<SharedQuestProgress>();

            lock (s_Lock)
            {
                var questIds = s_PlayerQuestProgress.GetValueOrDefault(player.Serial, new List<int>());
                return questIds.Select(id => s_QuestProgress.GetValueOrDefault(id))
                    .Where(p => p != null && !p.IsCompleted)
                    .ToList();
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
                var playerQuestIds = s_PlayerQuestProgress.GetValueOrDefault(member.Serial, new List<int>());
                var questsToLeave = new List<int>();

                foreach (var questId in playerQuestIds)
                {
                    if (s_QuestProgress.TryGetValue(questId, out var progress) && progress.Party == party)
                    {
                        // Remove member from progress tracking
                        progress.MemberProgress.Remove(member.Serial);

                        // Update objective progress to remove member's contributions
                        foreach (var objectiveData in progress.ObjectiveProgress.Values)
                        {
                            objectiveData.ProgressByMember.Remove(member.Serial);
                            
                            // Recalculate role contributions
                            RecalculateRoleContributions(objectiveData, progress);
                        }

                        questsToLeave.Add(questId);

                        // Check if minimum party size is still met
                        if (progress.MemberProgress.Count < 2)
                        {
                            // Mark quest as incomplete
                            progress.IsCompleted = false;
                            progress.CompletionTime = DateTime.MinValue;
                        }

                        SendMemberLeftNotification(progress, member);
                    }
                }

                // Remove quests from player's tracking
                foreach (var questId in questsToLeave)
                {
                    s_PlayerQuestProgress[member.Serial].Remove(questId);
                }

                Console.WriteLine($"[SharedProgressTracker] {member.Name} left {questsToLeave.Count} shared quest progress tracking");
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
                var partyQuests = s_QuestProgress.Values.Where(p => p.Party == party && !p.IsCompleted).ToList();

                foreach (var progress in partyQuests)
                {
                    // Add member to progress tracking
                    progress.MemberProgress[member.Serial] = new MemberProgressData
                    {
                        Member = member,
                        JoinedAt = DateTime.UtcNow,
                        AcceptedAt = DateTime.MinValue,
                        HasAccepted = false,
                        ObjectiveProgress = new Dictionary<string, int>(),
                        TotalContribution = 0,
                        ActionsCount = 0,
                        LastActivity = DateTime.UtcNow
                    };

                    // Initialize objective progress for new member
                    foreach (var objectiveData in progress.ObjectiveProgress.Values)
                    {
                        progress.MemberProgress[member.Serial].ObjectiveProgress[objectiveData.ObjectiveId] = 0;
                    }

                    // Track player's quest progress
                    if (!s_PlayerQuestProgress.ContainsKey(member.Serial))
                    {
                        s_PlayerQuestProgress[member.Serial] = new List<int>();
                    }
                    s_PlayerQuestProgress[member.Serial].Add(progress.QuestId);

                    SendMemberJoinedNotification(progress, member);
                }

                Console.WriteLine($"[SharedProgressTracker] {member.Name} joined {partyQuests.Count} shared quest progress tracking");
            }
        }

        /// <summary>
        /// Recalculate role contributions
        /// </summary>
        private static void RecalculateRoleContributions(ObjectiveProgressData objectiveData, SharedQuestProgress progress)
        {
            objectiveData.ContributionByRole.Clear();

            foreach (var memberData in progress.MemberProgress.Values)
            {
                if (memberData.HasAccepted)
                {
                    string role = DeterminePlayerRole(memberData.Member, objectiveData);
                    int memberContribution = objectiveData.ProgressByMember.GetValueOrDefault(memberData.Member.Serial, 0);
                    objectiveData.ContributionByRole[role] = objectiveData.ContributionByRole.GetValueOrDefault(role, 0) + memberContribution;
                }
            }
        }

        /// <summary>
        /// Determine player role for an objective
        /// </summary>
        private static string DeterminePlayerRole(PlayerMobile player, ObjectiveProgressData objectiveData)
        {
            // This would be implemented based on player class, skills, or party position
            // For now, return a default role
            return "member";
        }

        /// <summary>
        /// Get required roles for an objective
        /// </summary>
        private static List<string> GetRequiredRolesForObjective(SharedQuestProgress progress, string objectiveId)
        {
            // This would be implemented based on the objective requirements
            // For now, return default roles
            return new List<string> { "member" };
        }

        /// <summary>
        /// Send objective completion notification
        /// </summary>
        private static void SendObjectiveCompletionNotification(SharedQuestProgress progress, string objectiveId)
        {
            var objectiveData = progress.ObjectiveProgress[objectiveId];
            
            foreach (var memberData in progress.MemberProgress.Values)
            {
                if (memberData.HasAccepted)
                {
                    memberData.Member.SendMessage($"Objective completed: {objectiveData.Description}");
                }
            }
        }

        /// <summary>
        /// Send quest completion notification
        /// </summary>
        private static void SendQuestCompletionNotification(SharedQuestProgress progress)
        {
            foreach (var memberData in progress.MemberProgress.Values)
            {
                if (memberData.HasAccepted)
                {
                    memberData.Member.SendMessage($"Shared quest completed! Total contribution: {memberData.TotalContribution}");
                }
            }
        }

        /// <summary>
        /// Send member left notification
        /// </summary>
        private static void SendMemberLeftNotification(SharedQuestProgress progress, PlayerMobile member)
        {
            foreach (var memberData in progress.MemberProgress.Values)
            {
                if (memberData.HasAccepted && memberData.Member != member)
                {
                    memberData.Member.SendMessage($"{member.Name} has left the shared quest");
                }
            }
        }

        /// <summary>
        /// Send member joined notification
        /// </summary>
        private static void SendMemberJoinedNotification(SharedQuestProgress progress, PlayerMobile member)
        {
            foreach (var memberData in progress.MemberProgress.Values)
            {
                if (memberData.HasAccepted && memberData.Member != member)
                {
                    memberData.Member.SendMessage($"{member.Name} has joined the shared quest");
                }
            }
        }

        /// <summary>
        /// Get progress statistics
        /// </summary>
        public static ProgressStatistics GetStatistics()
        {
            lock (s_Lock)
            {
                var stats = new ProgressStatistics
                {
                    TotalTrackedQuests = s_QuestProgress.Count,
                    ActiveQuests = s_QuestProgress.Values.Count(p => !p.IsCompleted),
                    CompletedQuests = s_QuestProgress.Values.Count(p => p.IsCompleted),
                    TotalParticipants = s_PlayerQuestProgress.Values.Sum(list => list.Count),
                    LastActivity = DateTime.UtcNow
                };

                if (stats.TotalTrackedQuests > 0)
                {
                    stats.CompletionRate = (double)stats.CompletedQuests / stats.TotalTrackedQuests;
                }

                return stats;
            }
        }
    }

    /// <summary>
    /// Shared quest progress data
    /// </summary>
    public class SharedQuestProgress
    {
        public int QuestId { get; set; }
        public Party Party { get; set; }
        public DateTime StartedAt { get; set; }
        public DateTime LastUpdated { get; set; }
        public Dictionary<Serial, MemberProgressData> MemberProgress { get; set; }
        public Dictionary<string, ObjectiveProgressData> ObjectiveProgress { get; set; }
        public bool IsCompleted { get; set; }
        public DateTime CompletionTime { get; set; }
    }

    /// <summary>
    /// Member progress data
    /// </summary>
    public class MemberProgressData
    {
        public PlayerMobile Member { get; set; }
        public DateTime JoinedAt { get; set; }
        public DateTime AcceptedAt { get; set; }
        public bool HasAccepted { get; set; }
        public DateTime CompletedAt { get; set; }
        public bool HasCompleted { get; set; }
        public Dictionary<string, int> ObjectiveProgress { get; set; }
        public int TotalContribution { get; set; }
        public int ActionsCount { get; set; }
        public DateTime LastActivity { get; set; }
    }

    /// <summary>
    /// Objective progress data
    /// </summary>
    public class ObjectiveProgressData
    {
        public string ObjectiveId { get; set; }
        public string Description { get; set; }
        public CooperativeObjectiveType Type { get; set; }
        public int RequiredCount { get; set; }
        public int CurrentProgress { get; set; }
        public Dictionary<Serial, int> ProgressByMember { get; set; }
        public Dictionary<string, int> ContributionByRole { get; set; }
        public DateTime StartedAt { get; set; }
        public DateTime CompletedAt { get; set; }
        public bool IsCompleted { get; set; }
    }

    /// <summary>
    /// Progress statistics
    /// </summary>
    public class ProgressStatistics
    {
        public int TotalTrackedQuests { get; set; }
        public int ActiveQuests { get; set; }
        public int CompletedQuests { get; set; }
        public int TotalParticipants { get; set; }
        public double CompletionRate { get; set; }
        public DateTime LastActivity { get; set; }
    }
}
