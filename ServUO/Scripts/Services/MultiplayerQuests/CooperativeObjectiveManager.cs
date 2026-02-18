using System;
using System.Collections.Generic;
using System.Linq;
using Server.Mobiles;
using Server.Custom.VystiaClasses.Quests;
using Server.Engines.PartySystem;

namespace Server.Services.MultiplayerQuests
{
    /// <summary>
    /// Manages cooperative objectives for multiplayer quests
    /// </summary>
    public static class CooperativeObjectiveManager
    {
        private static readonly Dictionary<string, IObjectiveHandler> s_ObjectiveHandlers = new Dictionary<string, IObjectiveHandler>();
        private static readonly object s_Lock = new object();

        /// <summary>
        /// Initialize the cooperative objective manager
        /// </summary>
        public static void Initialize()
        {
            // Register objective handlers
            RegisterObjectiveHandler("kill", new KillObjectiveHandler());
            RegisterObjectiveHandler("collect", new CollectObjectiveHandler());
            RegisterObjectiveHandler("explore", new ExploreObjectiveHandler());
            RegisterObjectiveHandler("deliver", new DeliverObjectiveHandler());
            RegisterObjectiveHandler("protect", new ProtectObjectiveHandler());
            RegisterObjectiveHandler("rescue", new RescueObjectiveHandler());

            Console.WriteLine("[CooperativeObjectiveManager] Initialized with {s_ObjectiveHandlers.Count} objective handlers");
        }

        /// <summary>
        /// Register an objective handler
        /// </summary>
        public static void RegisterObjectiveHandler(string type, IObjectiveHandler handler)
        {
            lock (s_Lock)
            {
                s_ObjectiveHandlers[type] = handler;
            }
        }

        /// <summary>
        /// Update cooperative objective progress
        /// </summary>
        public static bool UpdateObjectiveProgress(SharedQuestInfo sharedQuest, string objectiveId, PlayerMobile player, int amount)
        {
            if (sharedQuest == null || player == null)
                return false;

            var objective = sharedQuest.CooperativeObjectives.FirstOrDefault(o => o.ObjectiveId == objectiveId);
            if (objective == null)
                return false;

            var memberProgress = sharedQuest.MemberProgress.FirstOrDefault(mp => mp.Member == player);
            if (memberProgress == null || !memberProgress.HasAccepted)
                return false;

            lock (s_Lock)
            {
                // Get the objective handler
                var handler = GetObjectiveHandler(objective);
                if (handler == null)
                    return false;

                // Validate the contribution
                if (!handler.ValidateContribution(objective, player, amount))
                    return false;

                // Update progress based on objective type
                bool progressUpdated = UpdateProgressByType(objective, memberProgress, amount);

                if (progressUpdated)
                {
                    // Record the action
                    memberProgress.ActionsPerformed.Add($"{DateTime.UtcNow:HH:mm:ss} - Contributed {amount} to {objective.Description}");
                    memberProgress.ContributionScore += CalculateContributionScore(objective, amount);

                    // Check if objective is completed
                    CheckObjectiveCompletion(sharedQuest, objective);

                    // Sync progress if enabled
                    if (sharedQuest.Settings.SyncProgress)
                    {
                        SyncProgressToPartyMembers(sharedQuest, objective);
                    }

                    // Send progress notification
                    SendProgressNotification(sharedQuest, objective, player, amount);

                    return true;
                }

                return false;
            }
        }

        /// <summary>
        /// Update progress based on objective type
        /// </summary>
        private static bool UpdateProgressByType(CooperativeObjective objective, PartyMemberProgress memberProgress, int amount)
        {
            switch (objective.Type)
            {
                case CooperativeObjectiveType.IndividualContribution:
                    // Each member must contribute individually
                    int currentProgress = memberProgress.ObjectiveProgress.ContainsKey(objective.ObjectiveId) ? memberProgress.ObjectiveProgress[objective.ObjectiveId] : 0;
                    int newProgress = Math.Min(currentProgress + amount, objective.RequiredCount);
                    memberProgress.ObjectiveProgress[objective.ObjectiveId] = newProgress;
                    return newProgress > currentProgress;

                case CooperativeObjectiveType.GroupContribution:
                    // Party as a whole must contribute
                    // This would require tracking total party progress
                    return true; // Placeholder

                case CooperativeObjectiveType.RoleBased:
                    // Specific roles required
                    string playerRole = DeterminePlayerRole(memberProgress.Member, objective);
                    if (objective.RequiredRoles.Contains(playerRole))
                    {
                        int roleProgress = objective.RoleContributions.ContainsKey(playerRole) ? objective.RoleContributions[playerRole] : 0;
                        int newRoleProgress = Math.Min(roleProgress + amount, objective.RequiredCount);
                        objective.RoleContributions[playerRole] = newRoleProgress;
                        return newRoleProgress > roleProgress;
                    }
                    return false;

                case CooperativeObjectiveType.AllMembers:
                    // All members must complete
                    int memberCurrent = memberProgress.ObjectiveProgress.ContainsKey(objective.ObjectiveId) ? memberProgress.ObjectiveProgress[objective.ObjectiveId] : 0;
                    int memberNew = Math.Min(memberCurrent + amount, objective.RequiredCount);
                    memberProgress.ObjectiveProgress[objective.ObjectiveId] = memberNew;
                    return memberNew > memberCurrent;

                case CooperativeObjectiveType.LeaderOnly:
                    // Only party leader can complete
                    var party = memberProgress.Member.Party as Party;
                    if (party != null && party.Leader == memberProgress.Member)
                    {
                        int leaderCurrent = memberProgress.ObjectiveProgress.ContainsKey(objective.ObjectiveId) ? memberProgress.ObjectiveProgress[objective.ObjectiveId] : 0;
                        int leaderNew = Math.Min(leaderCurrent + amount, objective.RequiredCount);
                        memberProgress.ObjectiveProgress[objective.ObjectiveId] = leaderNew;
                        return leaderNew > leaderCurrent;
                    }
                    return false;

                default:
                    return false;
            }
        }

        /// <summary>
        /// Check if an objective is completed
        /// </summary>
        private static void CheckObjectiveCompletion(SharedQuestInfo sharedQuest, CooperativeObjective objective)
        {
            bool isCompleted = false;

            switch (objective.Type)
            {
                case CooperativeObjectiveType.IndividualContribution:
                    // Check if any member has completed their individual requirement
                    isCompleted = sharedQuest.MemberProgress.Any(mp => 
                        mp.ObjectiveProgress.ContainsKey(objective.ObjectiveId) && mp.ObjectiveProgress[objective.ObjectiveId] >= objective.RequiredCount);
                    break;

                case CooperativeObjectiveType.GroupContribution:
                    // Check if total party contribution meets requirement
                    int totalProgress = sharedQuest.MemberProgress.Sum(mp => 
                        mp.ObjectiveProgress.ContainsKey(objective.ObjectiveId) ? mp.ObjectiveProgress[objective.ObjectiveId] : 0);
                    isCompleted = totalProgress >= objective.RequiredCount;
                    break;

                case CooperativeObjectiveType.RoleBased:
                    // Check if all required roles have completed their contributions
                    isCompleted = objective.RequiredRoles.All(role =>
                        objective.RoleContributions.ContainsKey(role) && objective.RoleContributions[role] >= objective.RequiredCount);
                    break;

                case CooperativeObjectiveType.AllMembers:
                    // Check if all members have completed
                    isCompleted = sharedQuest.MemberProgress.All(mp =>
                        mp.ObjectiveProgress.ContainsKey(objective.ObjectiveId) && mp.ObjectiveProgress[objective.ObjectiveId] >= objective.RequiredCount);
                    break;

                case CooperativeObjectiveType.LeaderOnly:
                    // Check if party leader has completed
                    var party = sharedQuest.Party as Party;
                    if (party != null)
                    {
                        var leaderProgress = sharedQuest.MemberProgress.FirstOrDefault(mp => mp.Member == party.Leader);
                        if (leaderProgress != null)
                        {
                            isCompleted = leaderProgress.ObjectiveProgress.ContainsKey(objective.ObjectiveId) && leaderProgress.ObjectiveProgress[objective.ObjectiveId] >= objective.RequiredCount;
                        }
                    }
                    break;
            }

            if (isCompleted && !objective.IsCompleted)
            {
                objective.IsCompleted = true;
                objective.CompletedAt = DateTime.UtcNow;

                // Send completion notification
                SendObjectiveCompletionNotification(sharedQuest, objective);

                // Check if all objectives are completed
                CheckQuestCompletion(sharedQuest);
            }
        }

        /// <summary>
        /// Check if the entire quest is completed
        /// </summary>
        private static void CheckQuestCompletion(SharedQuestInfo sharedQuest)
        {
            if (sharedQuest.CooperativeObjectives.All(o => o.IsCompleted))
            {
                sharedQuest.IsCompleted = true;
                sharedQuest.CompletedAt = DateTime.UtcNow;
                sharedQuest.IsActive = false;

                // Mark all participating members as completed
                foreach (var memberProgress in sharedQuest.MemberProgress)
                {
                    if (memberProgress.HasAccepted)
                    {
                        memberProgress.HasCompleted = true;
                        memberProgress.CompletedAt = DateTime.UtcNow;
                    }
                }

                // Send quest completion notification
                SendQuestCompletionNotification(sharedQuest);

                // Distribute rewards
                PartyRewardDistribution.DistributeRewards(sharedQuest);

                Console.WriteLine($"[CooperativeObjectiveManager] Shared quest '{sharedQuest.QuestTitle}' completed by party");
            }
        }

        /// <summary>
        /// Sync progress to all party members
        /// </summary>
        private static void SyncProgressToPartyMembers(SharedQuestInfo sharedQuest, CooperativeObjective objective)
        {
            if (!objective.Settings.ShowProgressToAll)
                return;

            foreach (var memberProgress in sharedQuest.MemberProgress)
            {
                if (memberProgress.HasAccepted)
                {
                    // Send progress update
                    int currentProgress = memberProgress.ObjectiveProgress.ContainsKey(objective.ObjectiveId) ? memberProgress.ObjectiveProgress[objective.ObjectiveId] : 0;
                    memberProgress.Member.SendMessage($"Objective Progress: {objective.Description} - {currentProgress}/{objective.RequiredCount}");
                }
            }
        }

        /// <summary>
        /// Get objective handler
        /// </summary>
        private static IObjectiveHandler GetObjectiveHandler(CooperativeObjective objective)
        {
            // Extract objective type from objective ID or description
            string type = ExtractObjectiveType(objective);
            
            lock (s_Lock)
            {
                return s_ObjectiveHandlers.ContainsKey(type) ? s_ObjectiveHandlers[type] : null;
            }
        }

        /// <summary>
        /// Extract objective type from objective
        /// </summary>
        private static string ExtractObjectiveType(CooperativeObjective objective)
        {
            var description = objective.Description.ToLower();
            
            if (description.Contains("kill") || description.Contains("defeat") || description.Contains("slay"))
                return "kill";
            
            if (description.Contains("collect") || description.Contains("gather") || description.Contains("find"))
                return "collect";
            
            if (description.Contains("explore") || description.Contains("discover") || description.Contains("search"))
                return "explore";
            
            if (description.Contains("deliver") || description.Contains("bring") || description.Contains("carry"))
                return "deliver";
            
            if (description.Contains("protect") || description.Contains("guard") || description.Contains("defend"))
                return "protect";
            
            if (description.Contains("rescue") || description.Contains("save") || description.Contains("free"))
                return "rescue";

            return "default";
        }

        /// <summary>
        /// Determine player role for role-based objectives
        /// </summary>
        private static string DeterminePlayerRole(PlayerMobile player, CooperativeObjective objective)
        {
            // This would be implemented based on player class, skills, or party position
            // For now, return a default role
            return "member";
        }

        /// <summary>
        /// Calculate contribution score
        /// </summary>
        private static int CalculateContributionScore(CooperativeObjective objective, int amount)
        {
            // Base score calculation
            int baseScore = amount;

            // Apply objective type modifiers
            switch (objective.Type)
            {
                case CooperativeObjectiveType.RoleBased:
                    baseScore *= 2; // Role-based contributions are worth more
                    break;
                case CooperativeObjectiveType.LeaderOnly:
                    baseScore *= 3; // Leader-only contributions are worth most
                    break;
            }

            return baseScore;
        }

        /// <summary>
        /// Send progress notification
        /// </summary>
        private static void SendProgressNotification(SharedQuestInfo sharedQuest, CooperativeObjective objective, PlayerMobile player, int amount)
        {
            foreach (var memberProgress in sharedQuest.MemberProgress)
            {
                if (memberProgress.HasAccepted && memberProgress.Member != player)
                {
                    int currentProgress = memberProgress.ObjectiveProgress.ContainsKey(objective.ObjectiveId) ? memberProgress.ObjectiveProgress[objective.ObjectiveId] : 0;
                    memberProgress.Member.SendMessage($"{player.Name} contributed {amount} to '{objective.Description}' ({currentProgress}/{objective.RequiredCount})");
                }
            }
        }

        /// <summary>
        /// Send objective completion notification
        /// </summary>
        private static void SendObjectiveCompletionNotification(SharedQuestInfo sharedQuest, CooperativeObjective objective)
        {
            foreach (var memberProgress in sharedQuest.MemberProgress)
            {
                if (memberProgress.HasAccepted)
                {
                    memberProgress.Member.SendMessage($"Objective completed: {objective.Description}");
                }
            }
        }

        /// <summary>
        /// Send quest completion notification
        /// </summary>
        private static void SendQuestCompletionNotification(SharedQuestInfo sharedQuest)
        {
            foreach (var memberProgress in sharedQuest.MemberProgress)
            {
                if (memberProgress.HasAccepted)
                {
                    memberProgress.Member.SendMessage($"Quest completed: {sharedQuest.QuestTitle}");
                    memberProgress.Member.SendMessage($"Your contribution score: {memberProgress.ContributionScore}");
                }
            }
        }
    }

    /// <summary>
    /// Interface for objective handlers
    /// </summary>
    public interface IObjectiveHandler
    {
        bool ValidateContribution(CooperativeObjective objective, PlayerMobile player, int amount);
        bool CanContribute(CooperativeObjective objective, PlayerMobile player);
        int CalculateContribution(CooperativeObjective objective, PlayerMobile player, int amount);
    }

    /// <summary>
    /// Base objective handler
    /// </summary>
    public abstract class BaseObjectiveHandler : IObjectiveHandler
    {
        public virtual bool ValidateContribution(CooperativeObjective objective, PlayerMobile player, int amount)
        {
            if (amount <= 0)
                return false;

            return CanContribute(objective, player);
        }

        public abstract bool CanContribute(CooperativeObjective objective, PlayerMobile player);
        public abstract int CalculateContribution(CooperativeObjective objective, PlayerMobile player, int amount);
    }

    /// <summary>
    /// Kill objective handler
    /// </summary>
    public class KillObjectiveHandler : BaseObjectiveHandler
    {
        public override bool CanContribute(CooperativeObjective objective, PlayerMobile player)
        {
            // Check if player is in combat and can kill
            return player.Alive && !player.Blessed;
        }

        public override int CalculateContribution(CooperativeObjective objective, PlayerMobile player, int amount)
        {
            // Base contribution score for kills
            return amount * 10;
        }
    }

    /// <summary>
    /// Collect objective handler
    /// </summary>
    public class CollectObjectiveHandler : BaseObjectiveHandler
    {
        public override bool CanContribute(CooperativeObjective objective, PlayerMobile player)
        {
            // Check if player can collect items
            return player.Alive;
        }

        public override int CalculateContribution(CooperativeObjective objective, PlayerMobile player, int amount)
        {
            // Base contribution score for collection
            return amount * 5;
        }
    }

    /// <summary>
    /// Explore objective handler
    /// </summary>
    public class ExploreObjectiveHandler : BaseObjectiveHandler
    {
        public override bool CanContribute(CooperativeObjective objective, PlayerMobile player)
        {
            // Check if player can explore
            return player.Alive;
        }

        public override int CalculateContribution(CooperativeObjective objective, PlayerMobile player, int amount)
        {
            // Base contribution score for exploration
            return amount * 15;
        }
    }

    /// <summary>
    /// Deliver objective handler
    /// </summary>
    public class DeliverObjectiveHandler : BaseObjectiveHandler
    {
        public override bool CanContribute(CooperativeObjective objective, PlayerMobile player)
        {
            // Check if player can deliver
            return player.Alive;
        }

        public override int CalculateContribution(CooperativeObjective objective, PlayerMobile player, int amount)
        {
            // Base contribution score for delivery
            return amount * 8;
        }
    }

    /// <summary>
    /// Protect objective handler
    /// </summary>
    public class ProtectObjectiveHandler : BaseObjectiveHandler
    {
        public override bool CanContribute(CooperativeObjective objective, PlayerMobile player)
        {
            // Check if player can protect
            return player.Alive && !player.Blessed;
        }

        public override int CalculateContribution(CooperativeObjective objective, PlayerMobile player, int amount)
        {
            // Base contribution score for protection
            return amount * 12;
        }
    }

    /// <summary>
    /// Rescue objective handler
    /// </summary>
    public class RescueObjectiveHandler : BaseObjectiveHandler
    {
        public override bool CanContribute(CooperativeObjective objective, PlayerMobile player)
        {
            // Check if player can rescue
            return player.Alive;
        }

        public override int CalculateContribution(CooperativeObjective objective, PlayerMobile player, int amount)
        {
            // Base contribution score for rescue
            return amount * 20;
        }
    }
}
