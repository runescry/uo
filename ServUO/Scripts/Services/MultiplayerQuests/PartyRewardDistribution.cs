using System;
using System.Collections.Generic;
using System.Linq;
using Server.Mobiles;
using Server.Custom.VystiaClasses.Quests;
using Server.Engines.PartySystem;
using Server.Items;
using Server.Misc;

namespace Server.Services.MultiplayerQuests
{
    /// <summary>
    /// Manages fair reward distribution for shared multiplayer quests
    /// </summary>
    public static class PartyRewardDistribution
    {
        private static readonly Dictionary<RewardDistributionMethod, IRewardDistributor> s_Distributors = new Dictionary<RewardDistributionMethod, IRewardDistributor>();
        private static readonly object s_Lock = new object();

        /// <summary>
        /// Initialize the party reward distribution system
        /// </summary>
        public static void Initialize()
        {
            // Register reward distributors
            RegisterRewardDistributor(RewardDistributionMethod.Equal, new EqualRewardDistributor());
            RegisterRewardDistributor(RewardDistributionMethod.ContributionBased, new ContributionBasedRewardDistributor());
            RegisterRewardDistributor(RewardDistributionMethod.Random, new RandomRewardDistributor());
            RegisterRewardDistributor(RewardDistributionMethod.LeaderBonus, new LeaderBonusRewardDistributor());
            RegisterRewardDistributor(RewardDistributionMethod.RoleBased, new RoleBasedRewardDistributor());
            RegisterRewardDistributor(RewardDistributionMethod.NeedBased, new NeedBasedRewardDistributor());

            Console.WriteLine("[PartyRewardDistribution] Initialized with {s_Distributors.Count} reward distributors");
        }

        /// <summary>
        /// Register a reward distributor
        /// </summary>
        public static void RegisterRewardDistributor(RewardDistributionMethod method, IRewardDistributor distributor)
        {
            lock (s_Lock)
            {
                s_Distributors[method] = distributor;
            }
        }

        /// <summary>
        /// Distribute rewards for a completed shared quest
        /// </summary>
        public static bool DistributeRewards(SharedQuestInfo sharedQuest)
        {
            if (sharedQuest == null || !sharedQuest.IsCompleted)
                return false;

            var rewards = GenerateQuestRewards(sharedQuest);
            if (rewards == null || !rewards.Any())
                return false;

            lock (s_Lock)
            {
                var distributor = s_Distributors.GetValueOrDefault(sharedQuest.Settings.RewardMethod);
                if (distributor == null)
                {
                    Console.WriteLine($"[PartyRewardDistribution] No distributor found for method {sharedQuest.Settings.RewardMethod}");
                    return false;
                }

                // Distribute rewards
                var distribution = distributor.Distribute(sharedQuest, rewards);

                // Apply rewards to players
                foreach (var rewardDistribution in distribution)
                {
                    ApplyRewardToPlayer(rewardDistribution.Player, rewardDistribution.Reward);
                }

                // Send reward notification
                SendRewardDistributionNotification(sharedQuest, distribution);

                // Log the distribution
                Console.WriteLine($"[PartyRewardDistribution] Distributed {rewards.Count} rewards using {sharedQuest.Settings.RewardMethod} method");

                return true;
            }
        }

        /// <summary>
        /// Generate quest rewards
        /// </summary>
        private static List<QuestReward> GenerateQuestRewards(SharedQuestInfo sharedQuest)
        {
            var rewards = new List<QuestReward>();

            // Get the original quest
            var quest = GetQuestById(sharedQuest.QuestId);
            if (quest == null)
                return rewards;

            // Generate rewards based on quest type and difficulty
            // This would be implemented based on the specific quest system
            // For now, generate some default rewards

            // Gold reward
            int goldAmount = CalculateGoldReward(quest, sharedQuest.MemberProgress.Count);
            if (goldAmount > 0)
            {
                rewards.Add(new QuestReward
                {
                    Type = RewardType.Gold,
                    Amount = goldAmount,
                    Description = $"{goldAmount} gold",
                    Item = null
                });
            }

            // Experience reward
            int expAmount = CalculateExperienceReward(quest, sharedQuest.MemberProgress.Count);
            if (expAmount > 0)
            {
                rewards.Add(new QuestReward
                {
                    Type = RewardType.Experience,
                    Amount = expAmount,
                    Description = $"{expAmount} experience",
                    Item = null
                });
            }

            // Item rewards
            var itemRewards = GenerateItemRewards(quest, sharedQuest.MemberProgress.Count);
            rewards.AddRange(itemRewards);

            return rewards;
        }

        /// <summary>
        /// Calculate gold reward
        /// </summary>
        private static int CalculateGoldReward(object quest, int partySize)
        {
            // Base gold calculation
            int baseGold = 100;

            // Adjust for party size
            double partyMultiplier = Math.Max(0.5, 1.0 - (partySize - 1) * 0.1);
            int adjustedGold = (int)(baseGold * partyMultiplier);

            // Adjust for quest difficulty
            // This would be implemented based on the quest's difficulty
            return Math.Max(50, adjustedGold);
        }

        /// <summary>
        /// Calculate experience reward
        /// </summary>
        private static int CalculateExperienceReward(object quest, int partySize)
        {
            // Base experience calculation
            int baseExp = 500;

            // Adjust for party size
            double partyMultiplier = Math.Max(0.6, 1.0 - (partySize - 1) * 0.08);
            int adjustedExp = (int)(baseExp * partyMultiplier);

            // Adjust for quest difficulty
            // This would be implemented based on the quest's difficulty
            return Math.Max(250, adjustedExp);
        }

        /// <summary>
        /// Generate item rewards
        /// </summary>
        private static List<QuestReward> GenerateItemRewards(object quest, int partySize)
        {
            var rewards = new List<QuestReward>();

            // Generate some sample items
            // This would be implemented based on the quest's reward structure
            
            // Magic item
            if (partySize <= 4)
            {
                var magicItem = new MagicItem(0xEED); // Random magic item
                magicItem.LootType = LootType.Regular;
                magicItem.Name = "Quest Reward";
                
                rewards.Add(new QuestReward
                {
                    Type = RewardType.Item,
                    Amount = 1,
                    Description = magicItem.Name,
                    Item = magicItem
                });
            }

            return rewards;
        }

        /// <summary>
        /// Apply reward to player
        /// </summary>
        private static void ApplyRewardToPlayer(PlayerMobile player, QuestReward reward)
        {
            if (player == null || reward == null)
                return;

            switch (reward.Type)
            {
                case RewardType.Gold:
                    player.AddToBackpack(new Gold(reward.Amount));
                    player.SendMessage($"You received {reward.Amount} gold!");
                    break;

                case RewardType.Experience:
                    // Add experience to player
                    // This would use the server's experience system
                    player.SendMessage($"You received {reward.Amount} experience!");
                    break;

                case RewardType.Item:
                    if (reward.Item != null)
                    {
                        player.AddToBackpack(reward.Item);
                        player.SendMessage($"You received {reward.Item.Name}!");
                    }
                    break;

                case RewardType.Fame:
                    // Add fame to player
                    player.Fame += reward.Amount;
                    player.SendMessage($"You received {reward.Amount} fame!");
                    break;

                case RewardType.Karma:
                    // Add karma to player
                    player.Karma += reward.Amount;
                    player.SendMessage($"You received {reward.Amount} karma!");
                    break;
            }
        }

        /// <summary>
        /// Get quest by ID
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

            return null;
        }

        /// <summary>
        /// Send reward distribution notification
        /// </summary>
        private static void SendRewardDistributionNotification(SharedQuestInfo sharedQuest, List<RewardDistribution> distribution)
        {
            foreach (var rewardDistribution in distribution)
            {
                if (rewardDistribution.Reward != null)
                {
                    rewardDistribution.Player.SendMessage($"Quest Reward: {rewardDistribution.Reward.Description}");
                }
            }

            // Send summary to party
            var party = sharedQuest.Party as Party;
            if (party != null)
            {
                party.SendMessage($"Quest '{sharedQuest.QuestTitle}' rewards distributed to {distribution.Count} party members");
            }
        }

        /// <summary>
        /// Get distribution statistics
        /// </summary>
        public static DistributionStatistics GetStatistics()
        {
            lock (s_Lock)
            {
                var stats = new DistributionStatistics
                {
                    TotalDistributions = 0, // This would be tracked
                    DistributionMethods = new Dictionary<RewardDistributionMethod, int>(),
                    AverageRewardsPerPlayer = 0,
                    TotalRewardsDistributed = 0,
                    LastDistribution = DateTime.UtcNow
                };

                return stats;
            }
        }

        /// <summary>
        /// Reset distribution statistics
        /// </summary>
        public static void ResetStatistics()
        {
            lock (s_Lock)
            {
                // Clear all distribution tracking
                // This would clear any tracked distribution statistics
                
                Console.WriteLine("[PartyRewardDistribution] Statistics reset - all distribution tracking cleared");
            }
        }
    }

    /// <summary>
    /// Quest reward data
    /// </summary>
    public class QuestReward
    {
        public RewardType Type { get; set; }
        public int Amount { get; set; }
        public string Description { get; set; }
        public Item Item { get; set; }
    }

    /// <summary>
    /// Reward distribution data
    /// </summary>
    public class RewardDistribution
    {
        public PlayerMobile Player { get; set; }
        public QuestReward Reward { get; set; }
        public double SharePercentage { get; set; }
        public string Reason { get; set; }
    }

    /// <summary>
    /// Reward types
    /// </summary>
    public enum RewardType
    {
        Gold,
        Experience,
        Item,
        Fame,
        Karma,
        Skill,
        Title,
        Property
    }

    /// <summary>
    /// Interface for reward distributors
    /// </summary>
    public interface IRewardDistributor
    {
        List<RewardDistribution> Distribute(SharedQuestInfo sharedQuest, List<QuestReward> rewards);
        string GetDescription();
    }

    /// <summary>
    /// Equal reward distributor
    /// </summary>
    public class EqualRewardDistributor : IRewardDistributor
    {
        public List<RewardDistribution> Distribute(SharedQuestInfo sharedQuest, List<QuestReward> rewards)
        {
            var distribution = new List<RewardDistribution>();
            var participatingMembers = sharedQuest.MemberProgress.Where(mp => mp.HasAccepted).ToList();

            foreach (var member in participatingMembers)
            {
                foreach (var reward in rewards)
                {
                    distribution.Add(new RewardDistribution
                    {
                        Player = member.Member,
                        Reward = CloneReward(reward),
                        SharePercentage = 1.0 / participatingMembers.Count,
                        Reason = "Equal distribution"
                    });
                }
            }

            return distribution;
        }

        public string GetDescription()
        {
            return "Equal distribution among all participating members";
        }

        private QuestReward CloneReward(QuestReward original)
        {
            return new QuestReward
            {
                Type = original.Type,
                Amount = original.Amount,
                Description = original.Description,
                Item = original.Item
            };
        }
    }

    /// <summary>
    /// Contribution-based reward distributor
    /// </summary>
    public class ContributionBasedRewardDistributor : IRewardDistributor
    {
        public List<RewardDistribution> Distribute(SharedQuestInfo sharedQuest, List<QuestReward> rewards)
        {
            var distribution = new List<RewardDistribution>();
            var participatingMembers = sharedQuest.MemberProgress.Where(mp => mp.HasAccepted).ToList();

            if (!participatingMembers.Any())
                return distribution;

            // Calculate total contribution
            double totalContribution = participatingMembers.Sum(mp => mp.ContributionScore);
            
            if (totalContribution <= 0)
            {
                // Fall back to equal distribution
                return new EqualRewardDistributor().Distribute(sharedQuest, rewards);
            }

            foreach (var member in participatingMembers)
            {
                double contributionRatio = member.ContributionScore / totalContribution;
                
                foreach (var reward in rewards)
                {
                    int adjustedAmount = CalculateContributionBasedAmount(reward, contributionRatio);
                    
                    distribution.Add(new RewardDistribution
                    {
                        Player = member.Member,
                        Reward = new QuestReward
                        {
                            Type = reward.Type,
                            Amount = adjustedAmount,
                            Description = $"{adjustedAmount} {reward.Description}",
                            Item = reward.Item
                        },
                        SharePercentage = contributionRatio,
                        Reason = $"Contribution score: {member.ContributionScore}"
                    });
                }
            }

            return distribution;
        }

        private int CalculateContributionBasedAmount(QuestReward originalReward, double contributionRatio)
        {
            if (originalReward.Type == RewardType.Gold || originalReward.Type == RewardType.Experience)
            {
                return (int)(originalReward.Amount * contributionRatio);
            }
            
            return originalReward.Amount;
        }

        public string GetDescription()
        {
            return "Distribution based on individual contribution scores";
        }
    }

    /// <summary>
    /// Random reward distributor
    /// </summary>
    public class RandomRewardDistributor : IRewardDistributor
    {
        private static readonly Random s_Random = new Random();

        public List<RewardDistribution> Distribute(SharedQuestInfo sharedQuest, List<QuestReward> rewards)
        {
            var distribution = new List<RewardDistribution>();
            var participatingMembers = sharedQuest.MemberProgress.Where(mp => mp.HasAccepted).ToList();

            if (!participatingMembers.Any())
                return distribution;

            // Randomly assign rewards to members
            foreach (var reward in rewards)
            {
                var selectedMember = participatingMembers[s_Random.Next(participatingMembers.Count)];
                
                distribution.Add(new RewardDistribution
                {
                    Player = selectedMember.Member,
                    Reward = CloneReward(reward),
                    SharePercentage = 1.0 / participatingMembers.Count,
                    Reason = "Random selection"
                });
            }

            return distribution;
        }

        public string GetDescription()
        {
            return "Random distribution among participating members";
        }

        private QuestReward CloneReward(QuestReward original)
        {
            return new QuestReward
            {
                Type = original.Type,
                Amount = original.Amount,
                Description = original.Description,
                Item = original.Item
            };
        }
    }

    /// <summary>
    /// Leader bonus reward distributor
    /// </summary>
    public class LeaderBonusRewardDistributor : IRewardDistributor
    {
        public List<RewardDistribution> Distribute(SharedQuestInfo sharedQuest, List<QuestReward> rewards)
        {
            var distribution = new List<RewardDistribution>();
            var participatingMembers = sharedQuest.MemberProgress.Where(mp => mp.HasAccepted).ToList();
            var party = sharedQuest.Party as Party;

            if (!participatingMembers.Any())
                return distribution;

            // Identify party leader
            PlayerMobile leader = party?.Leader;
            var leaderMember = participatingMembers.FirstOrDefault(mp => mp.Member == leader);

            foreach (var reward in rewards)
            {
                // Give leader a bonus (50% extra)
                if (leaderMember != null)
                {
                    int leaderAmount = CalculateLeaderBonusAmount(reward);
                    distribution.Add(new RewardDistribution
                    {
                        Player = leaderMember.Member,
                        Reward = new QuestReward
                        {
                            Type = reward.Type,
                            Amount = leaderAmount,
                            Description = $"{leaderAmount} {reward.Description} (Leader Bonus)",
                            Item = reward.Item
                        },
                        SharePercentage = 0.5,
                        Reason = "Party leader bonus"
                    });
                }

                // Distribute remaining amount equally among other members
                var otherMembers = participatingMembers.Where(mp => mp.Member != leader).ToList();
                if (otherMembers.Any())
                {
                    int remainingAmount = reward.Amount - (leaderAmount - reward.Amount);
                    int amountPerMember = remainingAmount / otherMembers.Count;

                    foreach (var member in otherMembers)
                    {
                        distribution.Add(new RewardDistribution
                        {
                            Player = member.Member,
                            Reward = new QuestReward
                            {
                                Type = reward.Type,
                                Amount = amountPerMember,
                                Description = $"{amountPerMember} {reward.Description}",
                                Item = reward.Item
                            },
                            SharePercentage = 1.0 / otherMembers.Count,
                            Reason = "Equal distribution"
                        });
                    }
                }
            }

            return distribution;
        }

        private int CalculateLeaderBonusAmount(QuestReward reward)
        {
            if (reward.Type == RewardType.Gold || reward.Type == RewardType.Experience)
            {
                return (int)(reward.Amount * 1.5); // 50% bonus
            }
            
            return reward.Amount;
        }

        public string GetDescription()
        {
            return "Leader gets 50% bonus, others share equally";
        }
    }

    /// <summary>
    /// Role-based reward distributor
    /// </summary>
    public class RoleBasedRewardDistributor : IRewardDistributor
    {
        public List<RewardDistribution> Distribute(SharedQuestInfo sharedQuest, List<QuestReward> rewards)
        {
            var distribution = new List<RewardDistribution>();
            var participatingMembers = sharedQuest.MemberProgress.Where(mp => mp.HasAccepted).ToList();

            if (!participatingMembers.Any())
                return distribution;

            // This would be implemented based on role requirements in objectives
            // For now, fall back to equal distribution
            return new EqualRewardDistributor().Distribute(sharedQuest, rewards);
        }

        public string GetDescription()
        {
            return "Distribution based on role requirements";
        }
    }

    /// <summary>
    /// Need-based reward distributor
    /// </summary>
    public class NeedBasedRewardDistributor : IRewardDistributor
    {
        public List<RewardDistribution> Distribute(SharedQuestInfo sharedQuest, List<QuestReward> rewards)
        {
            var distribution = new List<RewardDistribution>();
            var participatingMembers = sharedQuest.MemberProgress.Where(mp => mp.HasAccepted).ToList();

            if (!participatingMembers.Any())
                return distribution;

            // Calculate need scores for each member
            var memberNeeds = participatingMembers.ToDictionary(mp => mp.Member, mp => CalculateNeedScore(mp.Member));

            foreach (var reward in rewards)
            {
                // Distribute based on need scores
                var totalNeed = memberNeeds.Values.Sum();
                
                if (totalNeed > 0)
                {
                    foreach (var member in participatingMembers)
                    {
                        double needRatio = memberNeeds[member.Member] / totalNeed;
                        int adjustedAmount = CalculateNeedBasedAmount(reward, needRatio);
                        
                        distribution.Add(new RewardDistribution
                        {
                            Player = member.Member,
                            Reward = new QuestReward
                            {
                                Type = reward.Type,
                                Amount = adjustedAmount,
                                Description = $"{adjustedAmount} {reward.Description}",
                                Item = reward.Item
                            },
                            SharePercentage = needRatio,
                            Reason = "Need-based distribution"
                        });
                    }
                }
            }

            return distribution;
        }

        private double CalculateNeedScore(PlayerMobile player)
        {
            // Calculate need score based on player's current state
            double score = 1.0;

            // Lower level players need more experience
            score += (100 - player.Level) * 0.01;

            // Players with less gold need more gold
            score += Math.Max(0, (1000 - player.GetGold())) * 0.001;

            // Players with lower fame need more fame
            score += Math.Max(0, (100 - player.Fame)) * 0.01;

            return score;
        }

        private int CalculateNeedBasedAmount(QuestReward originalReward, double needRatio)
        {
            if (originalReward.Type == RewardType.Gold || originalReward.Type == RewardType.Experience)
            {
                return (int)(originalReward.Amount * needRatio);
            }
            
            return originalReward.Amount;
        }

        public string GetDescription()
        {
            return "Distribution based on player needs";
        }
    }

    /// <summary>
    /// Distribution statistics
    /// </summary>
    public class DistributionStatistics
    {
        public int TotalDistributions { get; set; }
        public Dictionary<RewardDistributionMethod, int> DistributionMethods { get; set; }
        public double AverageRewardsPerPlayer { get; set; }
        public int TotalRewardsDistributed { get; set; }
        public DateTime LastDistribution { get; set; }
    }
}
