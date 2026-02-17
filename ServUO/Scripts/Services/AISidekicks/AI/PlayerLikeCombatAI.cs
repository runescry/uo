using System;
using Server;
using Server.Mobiles;
using Server.Services.AISidekicks;

namespace Server.Services.AISidekicks
{
    /// <summary>
    /// Player-like combat AI for sidekicks
    /// Handles positioning, skill usage, item management, and tactical decisions
    /// </summary>
    public class PlayerLikeCombatAI
    {
        private AutonomousSidekick m_Sidekick;
        private DateTime m_LastPositionCheck = DateTime.UtcNow;
        private DateTime m_LastSkillUse = DateTime.UtcNow;
        private DateTime m_LastItemUse = DateTime.UtcNow;
        private TimeSpan m_PositionCheckInterval = TimeSpan.FromSeconds(1);
        private TimeSpan m_SkillUseInterval = TimeSpan.FromSeconds(2);
        private TimeSpan m_ItemUseInterval = TimeSpan.FromSeconds(3);

        public PlayerLikeCombatAI(AutonomousSidekick sidekick)
        {
            m_Sidekick = sidekick;
        }

        /// <summary>
        /// Main combat think method - called during combat
        /// </summary>
        public void OnThink()
        {
            if (m_Sidekick == null || m_Sidekick.Deleted || m_Sidekick.Combatant == null)
                return;

            Mobile target = m_Sidekick.Combatant as Mobile;
            if (target == null || target.Deleted || !target.Alive)
                return;

            // Positioning
            if (DateTime.UtcNow - m_LastPositionCheck >= m_PositionCheckInterval)
            {
                MaintainOptimalPosition(target);
                m_LastPositionCheck = DateTime.UtcNow;
            }

            // Skill usage
            if (DateTime.UtcNow - m_LastSkillUse >= m_SkillUseInterval)
            {
                if (ShouldUseSkill())
                {
                    UseAppropriateSkill(target);
                    m_LastSkillUse = DateTime.UtcNow;
                }
            }

            // Item usage (potions, bandages)
            if (DateTime.UtcNow - m_LastItemUse >= m_ItemUseInterval)
            {
                if (ShouldUsePotion())
                {
                    UseHealthPotion();
                    m_LastItemUse = DateTime.UtcNow;
                }
            }

            // Tactical decisions
            if (ShouldRetreat())
            {
                Retreat(target);
            }
        }

        /// <summary>
        /// Maintain optimal position for combat style
        /// </summary>
        private void MaintainOptimalPosition(Mobile target)
        {
            int optimalRange = GetOptimalRange();
            int currentRange = (int)m_Sidekick.GetDistanceToSqrt(target);

            if (currentRange < optimalRange - 1)
            {
                // Too close, move away
                MoveAwayFrom(target);
            }
            else if (currentRange > optimalRange + 1)
            {
                // Too far, move closer
                MoveToward(target);
            }
            else
            {
                // Good range, face target
                m_Sidekick.Direction = m_Sidekick.GetDirectionTo(target);
            }
        }

        /// <summary>
        /// Get optimal combat range based on combat style
        /// </summary>
        private int GetOptimalRange()
        {
            switch (m_Sidekick.CombatStyle)
            {
                case CombatStyle.Melee:
                    return 1;
                case CombatStyle.Archer:
                    return 5;
                case CombatStyle.Mage:
                    return 10;
                case CombatStyle.Hybrid:
                    return 5;
                default:
                    return 1;
            }
        }

        /// <summary>
        /// Move away from target
        /// </summary>
        private void MoveAwayFrom(Mobile target)
        {
            if (!m_Sidekick.CanMove)
                return;

            Direction awayDir = GetDirectionAwayFrom(target);
            m_Sidekick.Direction = awayDir;
            m_Sidekick.Move(awayDir);
        }

        /// <summary>
        /// Move toward target
        /// </summary>
        private void MoveToward(Mobile target)
        {
            if (!m_Sidekick.CanMove)
                return;

            Direction towardDir = m_Sidekick.GetDirectionTo(target);
            m_Sidekick.Direction = towardDir;
            m_Sidekick.Move(towardDir);
        }

        /// <summary>
        /// Get direction away from target
        /// </summary>
        private Direction GetDirectionAwayFrom(Mobile target)
        {
            Direction toward = m_Sidekick.GetDirectionTo(target);
            // Reverse direction
            return (Direction)(((int)toward + 4) % 8);
        }

        /// <summary>
        /// Check if should use a skill
        /// </summary>
        private bool ShouldUseSkill()
        {
            // Use skills periodically during combat
            return Utility.Random(100) < 30; // 30% chance
        }

        /// <summary>
        /// Use appropriate skill based on combat style
        /// </summary>
        private void UseAppropriateSkill(Mobile target)
        {
            // Skill usage would be handled by the skill system
            // This is a placeholder for future enhancement
            // Could trigger weapon abilities, spells, etc.
        }

        /// <summary>
        /// Check if should use a potion
        /// </summary>
        private bool ShouldUsePotion()
        {
            if (m_Sidekick.HitsMax == 0)
                return false;

            double healthPercent = (double)m_Sidekick.Hits / m_Sidekick.HitsMax;
            return healthPercent < 0.5; // Use potion if below 50% health
        }

        /// <summary>
        /// Use health potion
        /// </summary>
        private void UseHealthPotion()
        {
            // Search for health potion in pack
            if (m_Sidekick.Backpack != null)
            {
                // Look for any potion item (simplified - would need proper potion type checking)
                // This is a placeholder - potion usage would need proper implementation
            }
        }

        /// <summary>
        /// Check if should retreat
        /// </summary>
        private bool ShouldRetreat()
        {
            if (m_Sidekick.HitsMax == 0)
                return false;

            double healthPercent = (double)m_Sidekick.Hits / m_Sidekick.HitsMax;
            
            // Retreat if very low health and owner is not nearby
            if (healthPercent < 0.2)
            {
                if (m_Sidekick.Owner != null)
                {
                    int distanceToOwner = (int)m_Sidekick.GetDistanceToSqrt(m_Sidekick.Owner);
                    return distanceToOwner > 10; // Retreat if owner is far
                }
                return true;
            }

            return false;
        }

        /// <summary>
        /// Retreat from combat
        /// </summary>
        private void Retreat(Mobile target)
        {
            // Move away from target
            MoveAwayFrom(target);

            // If owner is nearby, move toward owner
            if (m_Sidekick.Owner != null && !m_Sidekick.Owner.Deleted)
            {
                int distanceToOwner = (int)m_Sidekick.GetDistanceToSqrt(m_Sidekick.Owner);
                if (distanceToOwner < 15)
                {
                    Direction towardOwner = m_Sidekick.GetDirectionTo(m_Sidekick.Owner);
                    m_Sidekick.Direction = towardOwner;
                    if (m_Sidekick.CanMove)
                    {
                        m_Sidekick.Move(towardOwner);
                    }
                }
            }
        }
    }
}

