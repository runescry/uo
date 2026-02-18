using System;
using System.Collections.Generic;
using System.Linq;
using Server.Mobiles;
using Server.Items;
using Server.Engines.BulkOrders;
using Server.Services.UnifiedQuestSystem;

namespace Server.Services.UnifiedQuestSystem.Integrations
{
    /// <summary>
    /// Integration bridge between Unified Quest System and Bulk Order System
    /// Connects quest objectives with crafting bulk orders
    /// </summary>
    public static class BulkOrderQuestIntegration
    {
        private static readonly Dictionary<PlayerMobile, List<BulkOrderQuestLink>> s_ActiveLinks;
        private static readonly object s_Lock = new object();
        private static bool s_Initialized = false;
        private static int s_TotalLinksCreated = 0;
        private static int s_QuestsCompleted = 0;

        static BulkOrderQuestIntegration()
        {
            s_ActiveLinks = new Dictionary<PlayerMobile, List<BulkOrderQuestLink>>();
        }

        /// <summary>
        /// Initialize the Bulk Order-Quest integration
        /// </summary>
        public static void Initialize()
        {
            if (s_Initialized)
                return;

            s_Initialized = true;

            // Register event handlers
            EventSink.OnBODComplete += OnBulkOrderComplete;
            EventSink.OnBODClaim += OnBulkOrderClaim;

            Console.WriteLine("[BulkOrderQuestIntegration] Initialized Bulk Order-Quest integration");
        }

        /// <summary>
        /// Create a quest link for a bulk order
        /// </summary>
        public static BulkOrderQuestLink CreateQuestLink(PlayerMobile player, IBOD bod, UnifiedQuestData quest)
        {
            if (player == null || bod == null || quest == null)
                return null;

            lock (s_Lock)
            {
                var link = new BulkOrderQuestLink
                {
                    Player = player,
                    BulkOrder = bod,
                    Quest = quest,
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true
                };

                // Add to active links
                if (!s_ActiveLinks.ContainsKey(player))
                    s_ActiveLinks[player] = new List<BulkOrderQuestLink>();

                s_ActiveLinks[player].Add(link);
                s_TotalLinksCreated++;

                // Update quest with bulk order objective
                UpdateQuestWithBulkOrder(quest, bod);

                Console.WriteLine($"[BulkOrderQuestIntegration] Created quest link for {player.Name}: {quest.Title}");

                return link;
            }
        }

        /// <summary>
        /// Get active quest links for a player
        /// </summary>
        public static List<BulkOrderQuestLink> GetActiveLinks(PlayerMobile player)
        {
            lock (s_Lock)
            {
                return s_ActiveLinks.GetValueOrDefault(player, new List<BulkOrderQuestLink>());
            }
        }

        /// <summary>
        /// Get integration statistics
        /// </summary>
        public static BulkOrderQuestStatistics GetStatistics()
        {
            lock (s_Lock)
            {
                return new BulkOrderQuestStatistics
                {
                    TotalLinksCreated = s_TotalLinksCreated,
                    QuestsCompleted = s_QuestsCompleted,
                    ActiveLinks = s_ActiveLinks.Values.Sum(list => list.Count),
                    ActivePlayers = s_ActiveLinks.Count,
                    LastActivity = DateTime.UtcNow
                };
            }
        }

        /// <summary>
        /// Generate crafting quests based on bulk orders
        /// </summary>
        public static List<UnifiedQuestData> GenerateCraftingQuests(PlayerMobile player, int count = 3)
        {
            var quests = new List<UnifiedQuestData>();

            // Get player's bulk orders
            var availableBODs = GetAvailableBulkOrders(player);
            
            for (int i = 0; i < Math.Min(count, availableBODs.Count); i++)
            {
                var bod = availableBODs[i];
                var quest = CreateCraftingQuest(player, bod);
                
                if (quest != null)
                {
                    quests.Add(quest);
                    CreateQuestLink(player, bod, quest);
                }
            }

            return quests;
        }

        /// <summary>
        /// Handle bulk order completion
        /// </summary>
        private static void OnBulkOrderComplete(BODCompleteEventArgs e)
        {
            if (e.Owner is PlayerMobile player)
            {
                var links = GetActiveLinks(player);
                var relevantLinks = links.Where(l => l.BulkOrder == e.BOD && l.IsActive).ToList();

                foreach (var link in relevantLinks)
                {
                    // Update quest progress
                    UpdateQuestProgress(link, e.BOD);
                    
                    // Check if quest is complete
                    if (IsQuestComplete(link))
                    {
                        CompleteQuest(link);
                        s_QuestsCompleted++;
                    }
                }
            }
        }

        /// <summary>
        /// Handle bulk order claim
        /// </summary>
        private static void OnBulkOrderClaim(BODClaimEventArgs e)
        {
            if (e.From is PlayerMobile player)
            {
                // Remove completed links
                var links = GetActiveLinks(player);
                var completedLinks = links.Where(l => l.BulkOrder == e.BOD).ToList();

                foreach (var link in completedLinks)
                {
                    link.IsActive = false;
                    RemoveLink(link);
                }
            }
        }

        /// <summary>
        /// Update quest with bulk order objective
        /// </summary>
        private static void UpdateQuestWithBulkOrder(UnifiedQuestData quest, IBOD bod)
        {
            var objective = new QuestObjective
            {
                ObjectiveId = $"bulk_order_{bod.GetHashCode()}",
                Description = GenerateBulkOrderDescription(bod),
                Type = ObjectiveType.Craft,
                RequiredAmount = bod.AmountMax,
                CurrentProgress = 0,
                IsCompleted = false,
                Metadata = new Dictionary<string, object>
                {
                    { "bod_type", bod.BODType.ToString() },
                    { "material", bod.Material.ToString() },
                    { "exceptional", bod.RequireExceptional },
                    { "bod_hash", bod.GetHashCode() }
                }
            };

            if (quest.Objectives == null)
                quest.Objectives = new List<QuestObjective>();

            quest.Objectives.Add(objective);
        }

        /// <summary>
        /// Generate description for bulk order
        /// </summary>
        private static string GenerateBulkOrderDescription(IBOD bod)
        {
            var material = bod.Material.ToString();
            var type = bod.BODType.ToString();
            var exceptional = bod.RequireExceptional ? "exceptional " : "";
            var amount = bod.AmountMax;

            return $"Craft {amount} {exceptional}{material} {type} items";
        }

        /// <summary>
        /// Get available bulk orders for player
        /// </summary>
        private static List<IBOD> GetAvailableBulkOrders(PlayerMobile player)
        {
            var bods = new List<IBOD>();

            // Get player's bulk order deeds
            var pack = player.Backpack;
            if (pack != null)
            {
                var deeds = pack.FindItemsByType<BulkOrderDeed>();
                foreach (var deed in deeds)
                {
                    if (deed.BOD != null)
                        bods.Add(deed.BOD);
                }
            }

            return bods;
        }

        /// <summary>
        /// Create crafting quest from bulk order
        /// </summary>
        private static UnifiedQuestData CreateCraftingQuest(PlayerMobile player, IBOD bod)
        {
            var quest = new UnifiedQuestData
            {
                QuestId = DateTime.UtcNow.GetHashCode(),
                Title = $"Crafting Order: {bod.Material} {bod.BODType}",
                Description = $"Complete a bulk order for {bod.AmountMax} {bod.Material} {bod.BODType} items.",
                Type = QuestType.Crafting,
                Owner = player,
                Creator = player,
                StartedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddDays(7),
                Objectives = new List<QuestObjective>(),
                Rewards = GenerateCraftingRewards(bod)
            };

            UpdateQuestWithBulkOrder(quest, bod);

            return quest;
        }

        /// <summary>
        /// Generate rewards for crafting quest
        /// </summary>
        private static List<QuestReward> GenerateCraftingRewards(IBOD bod)
        {
            var rewards = new List<QuestReward>();

            // Base gold reward
            var goldAmount = CalculateGoldReward(bod);
            rewards.Add(new QuestReward
            {
                Type = RewardType.Gold,
                Amount = goldAmount,
                Description = $"{goldAmount} gold"
            });

            // Skill gain bonus
            rewards.Add(new QuestReward
            {
                Type = RewardType.SkillGain,
                Amount = 0.1,
                Description = "Crafting skill gain bonus"
            });

            // Bulk order points
            rewards.Add(new QuestReward
            {
                Type = RewardType.Points,
                Amount = CalculatePointsReward(bod),
                Description = "Bulk order points"
            });

            return rewards;
        }

        /// <summary>
        /// Calculate gold reward based on bulk order
        /// </summary>
        private static int CalculateGoldReward(IBOD bod)
        {
            var baseAmount = 1000;
            var materialBonus = GetMaterialBonus(bod.Material);
            var typeBonus = GetTypeBonus(bod.BODType);
            var amountMultiplier = Math.Min(bod.AmountMax / 10, 5);

            return baseAmount + materialBonus + typeBonus + (amountMultiplier * 500);
        }

        /// <summary>
        /// Calculate points reward based on bulk order
        /// </summary>
        private static int CalculatePointsReward(IBOD bod)
        {
            var basePoints = 10;
            var difficultyBonus = bod.RequireExceptional ? 5 : 0;
            var amountBonus = Math.Min(bod.AmountMax / 20, 10);

            return basePoints + difficultyBonus + amountBonus;
        }

        /// <summary>
        /// Get material bonus for reward calculation
        /// </summary>
        private static int GetMaterialBonus(BulkMaterialType material)
        {
            switch (material)
            {
                case BulkMaterialType.DullCopper: return 100;
                case BulkMaterialType.ShadowIron: return 200;
                case BulkMaterialType.Copper: return 150;
                case BulkMaterialType.Bronze: return 250;
                case BulkMaterialType.Gold: return 300;
                case BulkMaterialType.Agapite: return 350;
                case BulkMaterialType.Verite: return 400;
                case BulkMaterialType.Valorite: return 500;
                case BulkMaterialType.SpinedLeather: return 200;
                case BulkMaterialType.HornedLeather: return 300;
                case BulkMaterialType.BarbedLeather: return 400;
                default: return 0;
            }
        }

        /// <summary>
        /// Get type bonus for reward calculation
        /// </summary>
        private static int GetTypeBonus(BODType type)
        {
            switch (type)
            {
                case BODType.Smith: return 100;
                case BODType.Tailor: return 150;
                case BODType.Carpenter: return 120;
                case BODType.Fletching: return 130;
                case BODType.Tinkering: return 110;
                case BODType.Inscription: return 140;
                default: return 0;
            }
        }

        /// <summary>
        /// Update quest progress based on bulk order
        /// </summary>
        private static void UpdateQuestProgress(BulkOrderQuestLink link, IBOD bod)
        {
            var objective = link.Quest.Objectives?.FirstOrDefault(o => 
                o.Metadata?.ContainsKey("bod_hash") == true && 
                o.Metadata["bod_hash"].Equals(bod.GetHashCode()));

            if (objective != null)
            {
                objective.CurrentProgress = bod.AmountCur;
                objective.IsCompleted = bod.Complete;

                // Update quest progress
                UnifiedProgressTracker.TrackProgress(link.Quest, new ProgressUpdate
                {
                    ProgressType = "craft",
                    Amount = bod.AmountCur,
                    Description = $"Crafted {bod.AmountCur}/{bod.AmountMax} items",
                    Source = link.Player,
                    ObjectiveId = objective.ObjectiveId
                });
            }
        }

        /// <summary>
        /// Check if quest is complete
        /// </summary>
        private static bool IsQuestComplete(BulkOrderQuestLink link)
        {
            return link.Quest.Objectives?.All(o => o.IsCompleted) == true;
        }

        /// <summary>
        /// Complete the quest
        /// </summary>
        private static void CompleteQuest(BulkOrderQuestLink link)
        {
            // Mark quest as complete
            link.Quest.IsCompleted = true;
            link.Quest.CompletedAt = DateTime.UtcNow;

            // Give rewards
            foreach (var reward in link.Quest.Rewards)
            {
                GiveReward(link.Player, reward);
            }

            // Notify player
            link.Player.SendMessage($"Quest completed: {link.Quest.Title}");
            link.Player.SendMessage($"Rewards: {string.Join(", ", link.Quest.Rewards.Select(r => r.Description))}");

            // Remove link
            RemoveLink(link);
        }

        /// <summary>
        /// Give reward to player
        /// </summary>
        private static void GiveReward(PlayerMobile player, QuestReward reward)
        {
            switch (reward.Type)
            {
                case RewardType.Gold:
                    player.AddToBackpack(new Gold(reward.Amount));
                    break;
                case RewardType.SkillGain:
                    // Apply skill gain bonus
                    player.SendMessage($"Skill gain bonus: +{reward.Amount:P1}");
                    break;
                case RewardType.Points:
                    // Add bulk order points
                    player.SendMessage($"Bulk order points: +{reward.Amount}");
                    break;
            }
        }

        /// <summary>
        /// Remove quest link
        /// </summary>
        private static void RemoveLink(BulkOrderQuestLink link)
        {
            lock (s_Lock)
            {
                if (s_ActiveLinks.ContainsKey(link.Player))
                {
                    s_ActiveLinks[link.Player].Remove(link);
                    
                    if (s_ActiveLinks[link.Player].Count == 0)
                        s_ActiveLinks.Remove(link.Player);
                }
            }
        }

        /// <summary>
        /// Clear completed links
        /// </summary>
        public static void ClearCompletedLinks()
        {
            lock (s_Lock)
            {
                var completedLinks = s_ActiveLinks.Values.SelectMany(list => list)
                    .Where(link => !link.IsActive || link.Quest.IsCompleted)
                    .ToList();

                foreach (var link in completedLinks)
                {
                    RemoveLink(link);
                }

                Console.WriteLine($"[BulkOrderQuestIntegration] Cleared {completedLinks.Count} completed links");
            }
        }

        /// <summary>
        /// Reset statistics
        /// </summary>
        public static void ResetStatistics()
        {
            lock (s_Lock)
            {
                s_TotalLinksCreated = 0;
                s_QuestsCompleted = 0;
                Console.WriteLine("[BulkOrderQuestIntegration] Statistics reset");
            }
        }
    }

    /// <summary>
    /// Links a bulk order with a quest
    /// </summary>
    public class BulkOrderQuestLink
    {
        public PlayerMobile Player { get; set; }
        public IBOD BulkOrder { get; set; }
        public UnifiedQuestData Quest { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsActive { get; set; }
    }

    /// <summary>
    /// Bulk Order-Quest integration statistics
    /// </summary>
    public class BulkOrderQuestStatistics
    {
        public int TotalLinksCreated { get; set; }
        public int QuestsCompleted { get; set; }
        public int ActiveLinks { get; set; }
        public int ActivePlayers { get; set; }
        public DateTime LastActivity { get; set; }
    }

    /// <summary>
    /// Event arguments for bulk order completion
    /// </summary>
    public class BODCompleteEventArgs : EventArgs
    {
        public Mobile Owner { get; set; }
        public IBOD BOD { get; set; }
    }

    /// <summary>
    /// Event arguments for bulk order claim
    /// </summary>
    public class BODClaimEventArgs : EventArgs
    {
        public Mobile From { get; set; }
        public IBOD BOD { get; set; }
    }
}
