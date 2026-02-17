using System;
using Server;
using Server.Mobiles;
using Server.Items;
using Server.SkillHandlers;

namespace Server.Services.AISidekicks
{
    /// <summary>
    /// Combat AI for Thief archetype - stealth approach + backstab
    /// Behavior: Hide when possible, approach hidden, backstab, escape if low health
    /// </summary>
    public class ThiefCombatAI
    {
        private AutonomousSidekick m_Sidekick;
        
        // Health thresholds
        public const double CRITICAL_HEALTH = 0.25;
        public const double LOW_HEALTH = 0.40;       // Thieves should escape earlier
        public const double HEAL_THRESHOLD = 0.60;
        public const double ESCAPE_HEALTH = 0.30;    // Run and hide below this
        
        // Cooldowns
        private DateTime m_LastBandageTime = DateTime.MinValue;
        private DateTime m_LastHealPotionTime = DateTime.MinValue;
        private DateTime m_LastCurePotionTime = DateTime.MinValue;
        private DateTime m_LastHideAttempt = DateTime.MinValue;
        
        private const double BANDAGE_COOLDOWN = 10.0;
        private const double HEAL_POTION_COOLDOWN = 10.0;
        private const double CURE_POTION_COOLDOWN = 1.0;
        private const double HIDE_COOLDOWN = 5.0;
        
        // Damage tracking
        private int m_LastHealth = -1;
        private bool m_TookDamageThisTick = false;

        // Combat state - tracks if thief is approaching while hidden (for stealth attack)
        #pragma warning disable CS0414 // Field is assigned but never read - used for state tracking
        private bool m_ApproachingHidden = false;
        #pragma warning restore CS0414

        public ThiefCombatAI(AutonomousSidekick sidekick)
        {
            m_Sidekick = sidekick;
            m_LastHealth = sidekick.Hits;
        }
        
        /// <summary>
        /// Main combat loop for thieves
        /// </summary>
        public bool DoCombat(Mobile enemy)
        {
            if (m_Sidekick == null || m_Sidekick.Deleted || !m_Sidekick.Alive)
                return false;
                
            if (enemy == null || enemy.Deleted || !enemy.Alive)
                return false;
            
            // Track damage taken
            m_TookDamageThisTick = m_LastHealth > 0 && m_Sidekick.Hits < m_LastHealth;
            m_LastHealth = m_Sidekick.Hits;
            
            double healthPercent = (double)m_Sidekick.Hits / (double)m_Sidekick.HitsMax;
            int distance = (int)m_Sidekick.GetDistanceToSqrt(enemy);
            
            Utility.PushColor(ConsoleColor.DarkGray);
            Console.WriteLine($"[ThiefCombatAI] {m_Sidekick.Name} - Distance: {distance}, Health: {healthPercent:P0}, Hidden: {m_Sidekick.Hidden}");
            Utility.PopColor();
            
            // PRIORITY 1: Escape if health is very low
            if (healthPercent < ESCAPE_HEALTH && !m_Sidekick.Hidden)
            {
                if (HandleEscape(enemy, distance))
                    return true;
            }
            
            // PRIORITY 2: Cure poison
            if (m_Sidekick.Poisoned)
            {
                TryCurePoison();
            }
            
            // PRIORITY 3: Heal if needed
            if (healthPercent < LOW_HEALTH || (m_TookDamageThisTick && healthPercent < HEAL_THRESHOLD))
            {
                HandleHealing(enemy, distance);
            }
            
            // PRIORITY 4: Stealth approach if not in combat
            if (!enemy.Combatant?.Equals(m_Sidekick) == true && distance > 3)
            {
                if (HandleStealthApproach(enemy, distance))
                    return true;
            }
            
            // PRIORITY 5: Melee combat
            return HandleMeleeCombat(enemy, distance);
        }
        
        /// <summary>
        /// Try to escape and hide when health is low
        /// </summary>
        private bool HandleEscape(Mobile enemy, int distance)
        {
            Utility.PushColor(ConsoleColor.Red);
            Console.WriteLine($"[ThiefCombatAI] {m_Sidekick.Name} - Low health, attempting escape!");
            Utility.PopColor();
            
            // Use heal potion while escaping
            TryUseHealPotion();
            
            // Try to run away
            if (m_Sidekick.AIObject is SidekickAI sidekickAI)
            {
                sidekickAI.RunFrom(enemy);
            }
            
            // Try to hide
            if (TryHide())
            {
                Utility.PushColor(ConsoleColor.DarkGray);
                Console.WriteLine($"[ThiefCombatAI] {m_Sidekick.Name} - Successfully hidden, escaping!");
                Utility.PopColor();
                return true;
            }
            
            return false;
        }
        
        /// <summary>
        /// Handle healing
        /// </summary>
        private void HandleHealing(Mobile enemy, int distance)
        {
            // If took damage, bandage
            if (m_TookDamageThisTick && !m_Sidekick.Poisoned)
            {
                TryUseBandage();
            }
            
            // Use potion if low
            double healthPercent = (double)m_Sidekick.Hits / (double)m_Sidekick.HitsMax;
            if (healthPercent < LOW_HEALTH)
            {
                TryUseHealPotion();
            }
        }
        
        /// <summary>
        /// Approach enemy while hidden for backstab
        /// </summary>
        private bool HandleStealthApproach(Mobile enemy, int distance)
        {
            // Try to hide if not hidden
            if (!m_Sidekick.Hidden)
            {
                if (TryHide())
                {
                    Utility.PushColor(ConsoleColor.DarkGray);
                    Console.WriteLine($"[ThiefCombatAI] {m_Sidekick.Name} - Hidden! Approaching target...");
                    Utility.PopColor();
                    m_ApproachingHidden = true;
                }
                else
                {
                    // Can't hide, just fight normally
                    return false;
                }
            }
            
            // Move toward enemy while hidden (using Stealth skill)
            if (m_Sidekick.Hidden && distance > 1)
            {
                // Use stealth movement
                if (m_Sidekick.Skills.Stealth.Value > 0)
                {
                    // Check if we can stealth
                    if (!m_Sidekick.AllowedStealthSteps.Equals(0) || m_Sidekick.CheckSkill(SkillName.Stealth, 0, 100))
                    {
                        m_Sidekick.AllowedStealthSteps = (int)(m_Sidekick.Skills.Stealth.Value / 5.0);
                    }
                }
                
                // Move toward target
                if (m_Sidekick.AIObject is SidekickAI sidekickAI)
                {
                    sidekickAI.MoveTo(enemy, false, 1); // Walk, don't run (to stay hidden)
                }
                
                return true;
            }
            
            return false;
        }
        
        /// <summary>
        /// Handle melee combat - fast strikes
        /// </summary>
        private bool HandleMeleeCombat(Mobile enemy, int distance)
        {
            m_ApproachingHidden = false;
            
            // Ensure we're in melee range
            if (distance > 1)
            {
                if (m_Sidekick.AIObject is SidekickAI sidekickAI)
                {
                    sidekickAI.MoveTo(enemy, true, 1);
                }
            }
            
            // Face the enemy
            m_Sidekick.Direction = m_Sidekick.GetDirectionTo(enemy);
            
            // If we were hidden, this is a backstab!
            if (m_Sidekick.Hidden && distance <= 1)
            {
                Utility.PushColor(ConsoleColor.DarkGray);
                Console.WriteLine($"[ThiefCombatAI] {m_Sidekick.Name} - BACKSTAB!");
                Utility.PopColor();
                m_Sidekick.Hidden = false; // Reveal
            }
            
            return true;
        }
        
        /// <summary>
        /// Try to hide
        /// </summary>
        private bool TryHide()
        {
            double cooldownRemaining = HIDE_COOLDOWN - (DateTime.UtcNow - m_LastHideAttempt).TotalSeconds;
            if (cooldownRemaining > 0)
                return false;
            
            m_LastHideAttempt = DateTime.UtcNow;
            
            // Use Hiding skill
            if (m_Sidekick.Skills.Hiding.Value > 0)
            {
                // Check skill
                if (m_Sidekick.CheckSkill(SkillName.Hiding, 0, 100))
                {
                    m_Sidekick.Hidden = true;
                    return true;
                }
            }
            
            return false;
        }
        
        #region Healing Methods
        
        private bool TryCurePoison()
        {
            if (TryUseCurePotion())
            {
                Utility.PushColor(ConsoleColor.Magenta);
                Console.WriteLine($"[ThiefCombatAI] {m_Sidekick.Name} - Used cure potion!");
                Utility.PopColor();
                return true;
            }
            return false;
        }
        
        public bool TryUseBandage()
        {
            if (m_Sidekick.Hits >= m_Sidekick.HitsMax)
                return false;
            
            double cooldownRemaining = BANDAGE_COOLDOWN - (DateTime.UtcNow - m_LastBandageTime).TotalSeconds;
            if (cooldownRemaining > 0)
                return false;
            
            Container pack = m_Sidekick.Backpack;
            if (pack == null)
                return false;
            
            Item bandage = pack.FindItemByType(typeof(Bandage));
            if (bandage == null || bandage.Amount < 1)
                return false;
            
            var bandageContext = BandageContext.GetContext(m_Sidekick);
            if (bandageContext != null)
                return false;
            
            try
            {
                BandageContext.BeginHeal(m_Sidekick, m_Sidekick);
                m_LastBandageTime = DateTime.UtcNow;
                return true;
            }
            catch
            {
                return false;
            }
        }
        
        private bool TryUseHealPotion()
        {
            double cooldownRemaining = HEAL_POTION_COOLDOWN - (DateTime.UtcNow - m_LastHealPotionTime).TotalSeconds;
            if (cooldownRemaining > 0)
                return false;
            
            if (m_Sidekick.Hits >= m_Sidekick.HitsMax)
                return false;
            
            if (m_Sidekick.Poisoned)
                return false;
            
            Container pack = m_Sidekick.Backpack;
            if (pack == null)
                return false;
            
            GreaterHealPotion potion = pack.FindItemByType(typeof(GreaterHealPotion)) as GreaterHealPotion;
            if (potion == null)
                return false;
            
            potion.Drink(m_Sidekick);
            m_LastHealPotionTime = DateTime.UtcNow;
            return true;
        }
        
        private bool TryUseCurePotion()
        {
            double cooldownRemaining = CURE_POTION_COOLDOWN - (DateTime.UtcNow - m_LastCurePotionTime).TotalSeconds;
            if (cooldownRemaining > 0)
                return false;
            
            if (!m_Sidekick.Poisoned)
                return false;
            
            Container pack = m_Sidekick.Backpack;
            if (pack == null)
                return false;
            
            GreaterCurePotion potion = pack.FindItemByType(typeof(GreaterCurePotion)) as GreaterCurePotion;
            if (potion == null)
                return false;
            
            potion.Drink(m_Sidekick);
            m_LastCurePotionTime = DateTime.UtcNow;
            return true;
        }
        
        #endregion
        
        public bool TryUseBandagePublic()
        {
            return TryUseBandage();
        }
    }
}

