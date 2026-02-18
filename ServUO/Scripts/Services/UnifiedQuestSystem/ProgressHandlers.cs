using System;
using System.Collections.Generic;
using System.Linq;
using Server.Mobiles;
using Server.Items;
using Server.Custom.VystiaClasses.Quests;

namespace Server.Services.UnifiedQuestSystem
{
    /// <summary>
    /// Kill progress handler
    /// </summary>
    public class KillProgressHandler : IProgressHandler
    {
        public string HandlerType => "kill";

        public ValidationResult ValidateUpdate(UnifiedQuestData quest, UnifiedQuestProgress questProgress, PlayerQuestProgress playerProgress, ProgressUpdate update)
        {
            var result = new ValidationResult { IsValid = true };

            // Validate objective exists
            if (!string.IsNullOrEmpty(update.ObjectiveId) && !questProgress.ObjectiveProgress.ContainsKey(update.ObjectiveId))
            {
                result.IsValid = false;
                result.ErrorMessage = $"Objective {update.ObjectiveId} not found in quest";
                return result;
            }

            // Validate amount
            if (update.Amount.HasValue && update.Amount.Value <= 0)
            {
                result.IsValid = false;
                result.ErrorMessage = "Kill amount must be positive";
                return result;
            }

            // Check if already completed
            if (!string.IsNullOrEmpty(update.ObjectiveId))
            {
                var objective = questProgress.ObjectiveProgress[update.ObjectiveId];
                if (objective.IsCompleted)
                {
                    result.Warnings.Add("Objective already completed");
                }
            }

            return result;
        }

        public void ApplyUpdate(UnifiedQuestData quest, UnifiedQuestProgress questProgress, PlayerQuestProgress playerProgress, ProgressUpdate update)
        {
            if (string.IsNullOrEmpty(update.ObjectiveId))
                return;

            // Update quest progress
            var questObjective = questProgress.ObjectiveProgress[update.ObjectiveId];
            questObjective.UpdateProgress(update.Amount ?? 1);

            // Update player progress
            var currentProgress = playerProgress.ObjectiveProgress.GetValueOrDefault(update.ObjectiveId, 0);
            playerProgress.ObjectiveProgress[update.ObjectiveId] = currentProgress + (update.Amount ?? 1);

            // Add contribution score
            playerProgress.ContributionScore += update.ContributionValue ?? 1;
        }
    }

    /// <summary>
    /// Collect progress handler
    /// </summary>
    public class CollectProgressHandler : IProgressHandler
    {
        public string HandlerType => "collect";

        public ValidationResult ValidateUpdate(UnifiedQuestData quest, UnifiedQuestProgress questProgress, PlayerQuestProgress playerProgress, ProgressUpdate update)
        {
            var result = new ValidationResult { IsValid = true };

            // Validate objective exists
            if (!string.IsNullOrEmpty(update.ObjectiveId) && !questProgress.ObjectiveProgress.ContainsKey(update.ObjectiveId))
            {
                result.IsValid = false;
                result.ErrorMessage = $"Objective {update.ObjectiveId} not found in quest";
                return result;
            }

            // Validate amount
            if (update.Amount.HasValue && update.Amount.Value <= 0)
            {
                result.IsValid = false;
                result.ErrorMessage = "Collect amount must be positive";
                return result;
            }

            // Check if player has required items
            if (update.Metadata?.ContainsKey("item_type") == true)
            {
                var itemType = update.Metadata["item_type"] as Type;
                var requiredAmount = update.Amount ?? 1;
                
                if (update.Source is PlayerMobile player && itemType != null)
                {
                    var itemCount = player.Backpack.GetItems(itemType).Count;
                    if (itemCount < requiredAmount)
                    {
                        result.IsValid = false;
                        result.ErrorMessage = $"Insufficient items: need {requiredAmount}, have {itemCount}";
                        return result;
                    }
                }
            }

            return result;
        }

        public void ApplyUpdate(UnifiedQuestData quest, UnifiedQuestProgress questProgress, PlayerQuestProgress playerProgress, ProgressUpdate update)
        {
            if (string.IsNullOrEmpty(update.ObjectiveId))
                return;

            // Update quest progress
            var questObjective = questProgress.ObjectiveProgress[update.ObjectiveId];
            questObjective.UpdateProgress(update.Amount ?? 1);

            // Update player progress
            var currentProgress = playerProgress.ObjectiveProgress.GetValueOrDefault(update.ObjectiveId, 0);
            playerProgress.ObjectiveProgress[update.ObjectiveId] = currentProgress + (update.Amount ?? 1);

            // Remove items from player's backpack if specified
            if (update.Metadata?.ContainsKey("item_type") == true && update.Source is PlayerMobile player)
            {
                var itemType = update.Metadata["item_type"] as Type;
                var amountToRemove = update.Amount ?? 1;
                
                if (itemType != null)
                {
                    var itemsToRemove = player.Backpack.GetItems(itemType).Take(amountToRemove).ToList();
                    foreach (var item in itemsToRemove)
                    {
                        item.Delete();
                    }
                }
            }

            // Add contribution score
            playerProgress.ContributionScore += update.ContributionValue ?? 1;
        }
    }

    /// <summary>
    /// Explore progress handler
    /// </summary>
    public class ExploreProgressHandler : IProgressHandler
    {
        public string HandlerType => "explore";

        public ValidationResult ValidateUpdate(UnifiedQuestData quest, UnifiedQuestProgress questProgress, PlayerQuestProgress playerProgress, ProgressUpdate update)
        {
            var result = new ValidationResult { IsValid = true };

            // Validate objective exists
            if (!string.IsNullOrEmpty(update.ObjectiveId) && !questProgress.ObjectiveProgress.ContainsKey(update.ObjectiveId))
            {
                result.IsValid = false;
                result.ErrorMessage = $"Objective {update.ObjectiveId} not found in quest";
                return result;
            }

            // Validate location
            if (!update.Metadata?.ContainsKey("location") == true)
            {
                result.IsValid = false;
                result.ErrorMessage = "Explore progress requires location metadata";
                return result;
            }

            // Check if player is at the required location
            if (update.Source is PlayerMobile player && update.Metadata["location"] is Point3D requiredLocation)
            {
                var distance = player.GetDistanceTo(requiredLocation);
                if (distance > 10) // Within 10 tiles
                {
                    result.IsValid = false;
                    result.ErrorMessage = $"Too far from exploration location: {distance} tiles away";
                    return result;
                }
            }

            return result;
        }

        public void ApplyUpdate(UnifiedQuestData quest, UnifiedQuestProgress questProgress, PlayerQuestProgress playerProgress, ProgressUpdate update)
        {
            if (string.IsNullOrEmpty(update.ObjectiveId))
                return;

            // Update quest progress (explore objectives are typically one-time)
            var questObjective = questProgress.ObjectiveProgress[update.ObjectiveId];
            if (!questObjective.IsCompleted)
            {
                questObjective.UpdateProgress(1);
            }

            // Update player progress
            playerProgress.ObjectiveProgress[update.ObjectiveId] = 1;

            // Add contribution score
            playerProgress.ContributionScore += update.ContributionValue ?? 2; // Exploration often worth more
        }
    }

    /// <summary>
    /// Deliver progress handler
    /// </summary>
    public class DeliverProgressHandler : IProgressHandler
    {
        public string HandlerType => "deliver";

        public ValidationResult ValidateUpdate(UnifiedQuestData quest, UnifiedQuestProgress questProgress, PlayerQuestProgress playerProgress, ProgressUpdate update)
        {
            var result = new ValidationResult { IsValid = true };

            // Validate objective exists
            if (!string.IsNullOrEmpty(update.ObjectiveId) && !questProgress.ObjectiveProgress.ContainsKey(update.ObjectiveId))
            {
                result.IsValid = false;
                result.ErrorMessage = $"Objective {update.ObjectiveId} not found in quest";
                return result;
            }

            // Validate delivery target
            if (!update.Metadata?.ContainsKey("target_mobile") == true)
            {
                result.IsValid = false;
                result.ErrorMessage = "Deliver progress requires target mobile metadata";
                return result;
            }

            // Check if player is near the target
            if (update.Source is PlayerMobile player && update.Metadata["target_mobile"] is Mobile targetMobile)
            {
                var distance = player.GetDistanceTo(targetMobile);
                if (distance > 5) // Within 5 tiles for delivery
                {
                    result.IsValid = false;
                    result.ErrorMessage = $"Too far from delivery target: {distance} tiles away";
                    return result;
                }
            }

            // Check if player has the required items
            if (update.Metadata?.ContainsKey("item_type") == true)
            {
                var itemType = update.Metadata["item_type"] as Type;
                var requiredAmount = update.Amount ?? 1;
                
                if (update.Source is PlayerMobile player && itemType != null)
                {
                    var itemCount = player.Backpack.GetItems(itemType).Count;
                    if (itemCount < requiredAmount)
                    {
                        result.IsValid = false;
                        result.ErrorMessage = $"Insufficient items for delivery: need {requiredAmount}, have {itemCount}";
                        return result;
                    }
                }
            }

            return result;
        }

        public void ApplyUpdate(UnifiedQuestData quest, UnifiedQuestProgress questProgress, PlayerQuestProgress playerProgress, ProgressUpdate update)
        {
            if (string.IsNullOrEmpty(update.ObjectiveId))
                return;

            // Update quest progress
            var questObjective = questProgress.ObjectiveProgress[update.ObjectiveId];
            questObjective.UpdateProgress(update.Amount ?? 1);

            // Update player progress
            var currentProgress = playerProgress.ObjectiveProgress.GetValueOrDefault(update.ObjectiveId, 0);
            playerProgress.ObjectiveProgress[update.ObjectiveId] = currentProgress + (update.Amount ?? 1);

            // Remove items from player's backpack if specified
            if (update.Metadata?.ContainsKey("item_type") == true && update.Source is PlayerMobile player)
            {
                var itemType = update.Metadata["item_type"] as Type;
                var amountToRemove = update.Amount ?? 1;
                
                if (itemType != null)
                {
                    var itemsToRemove = player.Backpack.GetItems(itemType).Take(amountToRemove).ToList();
                    foreach (var item in itemsToRemove)
                    {
                        item.Delete();
                    }
                }
            }

            // Add contribution score
            playerProgress.ContributionScore += update.ContributionValue ?? 3; // Delivery often worth more
        }
    }

    /// <summary>
    /// Protect progress handler
    /// </summary>
    public class ProtectProgressHandler : IProgressHandler
    {
        public string HandlerType => "protect";

        public ValidationResult ValidateUpdate(UnifiedQuestData quest, UnifiedQuestProgress questProgress, PlayerQuestProgress playerProgress, ProgressUpdate update)
        {
            var result = new ValidationResult { IsValid = true };

            // Validate objective exists
            if (!string.IsNullOrEmpty(update.ObjectiveId) && !questProgress.ObjectiveProgress.ContainsKey(update.ObjectiveId))
            {
                result.IsValid = false;
                result.ErrorMessage = $"Objective {update.ObjectiveId} not found in quest";
                return result;
            }

            // Validate protection target
            if (!update.Metadata?.ContainsKey("target_mobile") == true)
            {
                result.IsValid = false;
                result.ErrorMessage = "Protect progress requires target mobile metadata";
                return result;
            }

            // Check if target is still alive
            if (update.Metadata["target_mobile"] is Mobile targetMobile && targetMobile.Deleted)
            {
                result.IsValid = false;
                result.ErrorMessage = "Protection target has been killed or removed";
                return result;
            }

            // Check if player is near the target
            if (update.Source is PlayerMobile player)
            {
                var distance = player.GetDistanceTo(targetMobile);
                if (distance > 15) // Within 15 tiles for protection
                {
                    result.Warnings.Add("Player is far from protection target");
                }
            }

            return result;
        }

        public void ApplyUpdate(UnifiedQuestData quest, UnifiedQuestProgress questProgress, PlayerQuestProgress playerProgress, ProgressUpdate update)
        {
            if (string.IsNullOrEmpty(update.ObjectiveId))
                return;

            // Protection objectives are typically time-based
            // Update quest progress based on protection duration
            var questObjective = questProgress.ObjectiveProgress[update.ObjectiveId];
            
            // For protection, we track time rather than count
            if (update.Metadata?.ContainsKey("protection_time") == true)
            {
                var protectionTime = TimeSpan.FromSeconds(Convert.ToDouble(update.Metadata["protection_time"]));
                questObjective.CurrentCount = (int)protectionTime.TotalSeconds;
                questObjective.IsCompleted = questObjective.CurrentCount >= questObjective.RequiredCount;
            }
            else
            {
                // Default protection progress
                questObjective.UpdateProgress(1);
            }

            // Update player progress
            var currentProgress = playerProgress.ObjectiveProgress.GetValueOrDefault(update.ObjectiveId, 0);
            playerProgress.ObjectiveProgress[update.ObjectiveId] = currentProgress + 1;

            // Add contribution score
            playerProgress.ContributionScore += update.ContributionValue ?? 2; // Protection worth more than basic tasks
        }
    }

    /// <summary>
    /// Rescue progress handler
    /// </summary>
    public class RescueProgressHandler : IProgressHandler
    {
        public string HandlerType => "rescue";

        public ValidationResult ValidateUpdate(UnifiedQuestData quest, UnifiedQuestProgress questProgress, PlayerQuestProgress playerProgress, ProgressUpdate update)
        {
            var result = new ValidationResult { IsValid = true };

            // Validate objective exists
            if (!string.IsNullOrEmpty(update.ObjectiveId) && !questProgress.ObjectiveProgress.ContainsKey(update.ObjectiveId))
            {
                result.IsValid = false;
                result.ErrorMessage = $"Objective {update.ObjectiveId} not found in quest";
                return result;
            }

            // Validate rescue target
            if (!update.Metadata?.ContainsKey("target_mobile") == true)
            {
                result.IsValid = false;
                result.ErrorMessage = "Rescue progress requires target mobile metadata";
                return result;
            }

            // Check if target is still alive
            if (update.Metadata["target_mobile"] is Mobile targetMobile && targetMobile.Deleted)
            {
                result.IsValid = false;
                result.ErrorMessage = "Rescue target has been killed or removed";
                return result;
            }

            // Check if player is near the target
            if (update.Source is PlayerMobile player)
            {
                var distance = player.GetDistanceTo(targetMobile);
                if (distance > 5) // Within 5 tiles for rescue
                {
                    result.IsValid = false;
                    result.ErrorMessage = $"Too far from rescue target: {distance} tiles away";
                    return result;
                }
            }

            return result;
        }

        public void ApplyUpdate(UnifiedQuestData quest, UnifiedQuestProgress questProgress, PlayerQuestProgress playerProgress, ProgressUpdate update)
        {
            if (string.IsNullOrEmpty(update.ObjectiveId))
                return;

            // Update quest progress (rescue is typically one-time)
            var questObjective = questProgress.ObjectiveProgress[update.ObjectiveId];
            if (!questObjective.IsCompleted)
            {
                questObjective.UpdateProgress(1);
            }

            // Update player progress
            playerProgress.ObjectiveProgress[update.ObjectiveId] = 1;

            // Add contribution score
            playerProgress.ContributionScore += update.ContributionValue ?? 5; // Rescue worth significantly more

            // Add rescue event to metadata
            if (update.Metadata?.ContainsKey("target_mobile") == true)
            {
                update.Metadata["rescued_at"] = DateTime.UtcNow;
                update.Metadata["rescued_by"] = playerProgress.PlayerName;
            }
        }
    }

    /// <summary>
    /// Custom progress handler for special quest types
    /// </summary>
    public class CustomProgressHandler : IProgressHandler
    {
        private readonly string _handlerType;
        private readonly Func<UnifiedQuestData, UnifiedQuestProgress, PlayerQuestProgress, ProgressUpdate, ValidationResult> _validator;
        private readonly Action<UnifiedQuestData, UnifiedQuestProgress, PlayerQuestProgress, ProgressUpdate> _applier;

        public CustomProgressHandler(string handlerType, 
            Func<UnifiedQuestData, UnifiedQuestProgress, PlayerQuestProgress, ProgressUpdate, ValidationResult> validator,
            Action<UnifiedQuestData, UnifiedQuestProgress, PlayerQuestProgress, ProgressUpdate> applier)
        {
            _handlerType = handlerType;
            _validator = validator;
            _applier = applier;
        }

        public string HandlerType => _handlerType;

        public ValidationResult ValidateUpdate(UnifiedQuestData quest, UnifiedQuestProgress questProgress, PlayerQuestProgress playerProgress, ProgressUpdate update)
        {
            return _validator?.Invoke(quest, questProgress, playerProgress, update) ?? new ValidationResult { IsValid = true };
        }

        public void ApplyUpdate(UnifiedQuestData quest, UnifiedQuestProgress questProgress, PlayerQuestProgress playerProgress, ProgressUpdate update)
        {
            _applier?.Invoke(quest, questProgress, playerProgress, update);
        }
    }
}
