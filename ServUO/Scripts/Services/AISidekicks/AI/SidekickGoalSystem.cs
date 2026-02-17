using System;
using System.Collections.Generic;
using System.Linq;
using Server;
using Server.Mobiles;
using Server.Services.AISidekicks;

namespace Server.Services.AISidekicks
{
    /// <summary>
    /// Situation assessment for decision-making
    /// </summary>
    public class SituationAssessment
    {
        public bool OwnerInCombat { get; set; }
        public bool SidekickInCombat { get; set; }
        public double OwnerHealthPercent { get; set; }
        public double SidekickHealthPercent { get; set; }
        public double OwnerManaPercent { get; set; }
        public double SidekickManaPercent { get; set; }
        public int NearbyEnemies { get; set; }
        public int NearbyLoot { get; set; }
        public int DistanceToOwner { get; set; }
        public bool OwnerNeedsHealing { get; set; }
        public bool SidekickNeedsHealing { get; set; }
    }

    /// <summary>
    /// Goal system for autonomous sidekick behavior
    /// </summary>
    public static class SidekickGoalSystem
    {
        /// <summary>
        /// Assess the current situation
        /// </summary>
        public static SituationAssessment AssessSituation(AutonomousSidekick sidekick)
        {
            SituationAssessment assessment = new SituationAssessment();

            if (sidekick.Owner == null || sidekick.Owner.Deleted)
            {
                return assessment;
            }

            // Owner status
            assessment.OwnerInCombat = sidekick.Owner.Combatant != null && sidekick.Owner.Combatant.Alive;
            assessment.OwnerHealthPercent = sidekick.Owner.HitsMax > 0 
                ? (double)sidekick.Owner.Hits / sidekick.Owner.HitsMax 
                : 1.0;
            assessment.OwnerManaPercent = sidekick.Owner.ManaMax > 0 
                ? (double)sidekick.Owner.Mana / sidekick.Owner.ManaMax 
                : 1.0;
            assessment.OwnerNeedsHealing = assessment.OwnerHealthPercent < 0.5;

            // Sidekick status
            assessment.SidekickInCombat = sidekick.Combatant != null && sidekick.Combatant.Alive;
            assessment.SidekickHealthPercent = sidekick.HitsMax > 0 
                ? (double)sidekick.Hits / sidekick.HitsMax 
                : 1.0;
            assessment.SidekickManaPercent = sidekick.ManaMax > 0 
                ? (double)sidekick.Mana / sidekick.ManaMax 
                : 1.0;
            assessment.SidekickNeedsHealing = assessment.SidekickHealthPercent < 0.3;

            // Distance to owner
            assessment.DistanceToOwner = (int)sidekick.GetDistanceToSqrt(sidekick.Owner);

            // Nearby enemies
            assessment.NearbyEnemies = CountNearbyEnemies(sidekick, 12);

            // Nearby loot (items on ground from defeated enemies)
            assessment.NearbyLoot = CountNearbyLoot(sidekick, 8);

            return assessment;
        }

        /// <summary>
        /// Score all goals based on current situation
        /// </summary>
        public static Dictionary<SidekickGoal, float> ScoreGoals(AutonomousSidekick sidekick, SituationAssessment situation)
        {
            Dictionary<SidekickGoal, float> scores = new Dictionary<SidekickGoal, float>();

            // Initialize all goals with 0 score
            foreach (SidekickGoal goal in Enum.GetValues(typeof(SidekickGoal)))
            {
                scores[goal] = 0.0f;
            }

            // Combat goal - highest priority if owner or sidekick in combat
            if (situation.OwnerInCombat || situation.SidekickInCombat)
            {
                scores[SidekickGoal.Combat] = 100.0f;
            }

            // Assist owner goal - high priority if owner needs healing
            if (situation.OwnerNeedsHealing && !situation.SidekickInCombat)
            {
                scores[SidekickGoal.AssistOwner] = 80.0f + (float)((1.0 - situation.OwnerHealthPercent) * 20.0);
            }

            // Rest goal - if sidekick needs healing or mana
            if (situation.SidekickNeedsHealing && !situation.SidekickInCombat)
            {
                scores[SidekickGoal.Rest] = 70.0f;
            }
            else if (situation.SidekickManaPercent < 0.2 && sidekick.CombatStyle == CombatStyle.Mage)
            {
                scores[SidekickGoal.Rest] = 50.0f;
            }

            // Loot goal - low priority, only if not in combat
            if (situation.NearbyLoot > 0 && !situation.SidekickInCombat && !situation.OwnerInCombat)
            {
                scores[SidekickGoal.Loot] = 30.0f;
            }

            // Guard goal - if owner is in combat but sidekick isn't
            if (situation.OwnerInCombat && !situation.SidekickInCombat)
            {
                scores[SidekickGoal.Guard] = 40.0f;
            }

            // Follow owner - default goal, low priority
            if (!situation.OwnerInCombat && !situation.SidekickInCombat)
            {
                if (situation.DistanceToOwner > sidekick.FollowDistance)
                {
                    scores[SidekickGoal.FollowOwner] = 20.0f;
                }
                else
                {
                    scores[SidekickGoal.FollowOwner] = 10.0f;
                }
            }

            // Explore goal - very low priority, only when idle
            if (!situation.OwnerInCombat && !situation.SidekickInCombat && 
                situation.DistanceToOwner <= sidekick.FollowDistance)
            {
                scores[SidekickGoal.Explore] = 5.0f;
            }

            return scores;
        }

        /// <summary>
        /// Select the best goal based on scores
        /// </summary>
        public static SidekickGoal SelectBestGoal(Dictionary<SidekickGoal, float> scores)
        {
            if (scores == null || scores.Count == 0)
            {
                return SidekickGoal.FollowOwner; // Default
            }

            return scores.OrderByDescending(kvp => kvp.Value).First().Key;
        }

        /// <summary>
        /// Execute a goal
        /// </summary>
        public static void ExecuteGoal(AutonomousSidekick sidekick, SidekickGoal goal, SituationAssessment situation)
        {
            switch (goal)
            {
                case SidekickGoal.FollowOwner:
                    ExecuteFollowOwner(sidekick, situation);
                    break;
                case SidekickGoal.Combat:
                    ExecuteCombat(sidekick, situation);
                    break;
                case SidekickGoal.AssistOwner:
                    ExecuteAssistOwner(sidekick, situation);
                    break;
                case SidekickGoal.Explore:
                    ExecuteExplore(sidekick, situation);
                    break;
                case SidekickGoal.Loot:
                    ExecuteLoot(sidekick, situation);
                    break;
                case SidekickGoal.Guard:
                    ExecuteGuard(sidekick, situation);
                    break;
                case SidekickGoal.Rest:
                    ExecuteRest(sidekick, situation);
                    break;
                case SidekickGoal.Socialize:
                    ExecuteSocialize(sidekick, situation);
                    break;
                case SidekickGoal.Investigate:
                    ExecuteInvestigate(sidekick, situation);
                    break;
            }
        }

        #region Goal Execution Methods

        private static void ExecuteFollowOwner(AutonomousSidekick sidekick, SituationAssessment situation)
        {
            if (sidekick.Owner == null || sidekick.Owner.Deleted)
                return;

            sidekick.ControlOrder = OrderType.Follow;
            sidekick.ControlTarget = sidekick.Owner;
            sidekick.IsFollowing = true;
        }

        private static void ExecuteCombat(AutonomousSidekick sidekick, SituationAssessment situation)
        {
            // Combat is handled by the AI system
            // This just sets the goal
            if (sidekick.Owner != null && sidekick.Owner.Combatant != null)
            {
                sidekick.Combatant = sidekick.Owner.Combatant;
            }
        }

        private static void ExecuteAssistOwner(AutonomousSidekick sidekick, SituationAssessment situation)
        {
            // Assist owner - heal if possible
            if (sidekick.Owner != null && situation.OwnerNeedsHealing)
            {
                // Try to heal owner (implementation depends on skills)
                // For now, just move closer
                if (situation.DistanceToOwner > 5)
                {
                    sidekick.ControlOrder = OrderType.Follow;
                    sidekick.ControlTarget = sidekick.Owner;
                }
            }
        }

        private static void ExecuteExplore(AutonomousSidekick sidekick, SituationAssessment situation)
        {
            // Explore nearby area - move randomly within follow distance
            if (sidekick.Owner != null && situation.DistanceToOwner <= sidekick.FollowDistance)
            {
                // Small random movement
                Direction randomDir = (Direction)Utility.Random(8);
                if (sidekick.CanMove)
                {
                    sidekick.Move(randomDir);
                }
            }
        }

        private static void ExecuteLoot(AutonomousSidekick sidekick, SituationAssessment situation)
        {
            // Loot nearby items - find and pick up items
            // Implementation would search for items and pick them up
            // For now, just a placeholder
        }

        private static void ExecuteGuard(AutonomousSidekick sidekick, SituationAssessment situation)
        {
            // Guard owner - stay close and protect
            if (sidekick.Owner != null)
            {
                sidekick.ControlOrder = OrderType.Guard;
                sidekick.ControlTarget = sidekick.Owner;
            }
        }

        private static void ExecuteRest(AutonomousSidekick sidekick, SituationAssessment situation)
        {
            // Rest - stop moving, maybe meditate if mage
            sidekick.ControlOrder = OrderType.Stay;
            
            // If mage and low mana, could trigger meditation
            if (sidekick.CombatStyle == CombatStyle.Mage && situation.SidekickManaPercent < 0.3)
            {
                // Meditation would be handled by skill system
            }
        }

        private static void ExecuteSocialize(AutonomousSidekick sidekick, SituationAssessment situation)
        {
            // Socialize - talk to nearby NPCs
            // Implementation would find nearby NPCs and interact
        }

        private static void ExecuteInvestigate(AutonomousSidekick sidekick, SituationAssessment situation)
        {
            // Investigate - move toward interesting objects/events
            // Implementation would identify interesting targets and move toward them
        }

        #endregion

        #region Helper Methods

        private static int CountNearbyEnemies(AutonomousSidekick sidekick, int range)
        {
            int count = 0;
            if (sidekick.Map == null)
                return 0;

            foreach (Mobile m in sidekick.Map.GetMobilesInRange(sidekick.Location, range))
            {
                if (m != sidekick && m != sidekick.Owner && m.Alive && !m.Deleted)
                {
                    if (m is BaseCreature bc && (bc.Controlled || bc.Summoned))
                        continue; // Skip pets/summons

                    if (sidekick.CanBeHarmful(m, false))
                    {
                        count++;
                    }
                }
            }

            return count;
        }

        private static int CountNearbyLoot(AutonomousSidekick sidekick, int range)
        {
            int count = 0;
            if (sidekick.Map == null)
                return 0;

            foreach (Item item in sidekick.Map.GetItemsInRange(sidekick.Location, range))
            {
                if (item != null && !item.Deleted && item.Movable && item.Visible)
                {
                    // Check if item is loot (on ground, not in container)
                    if (item.Parent == null && item.RootParent == null)
                    {
                        count++;
                    }
                }
            }

            return count;
        }

        #endregion
    }
}

