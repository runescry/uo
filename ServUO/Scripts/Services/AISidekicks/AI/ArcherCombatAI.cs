using System;
using Server;
using Server.Mobiles;
using Server.Items;
using Server.Spells;
using Server.Spells.Fourth;
using Server.Spells.Second;

namespace Server.Services.AISidekicks
{
    /// <summary>
    /// Combat AI for Archer/Ranger archetype - ranged combat with kiting
    /// Behavior: Maintain optimal range, kite if too close, use bandages/potions
    /// </summary>
    public class ArcherCombatAI
    {
        private AutonomousSidekick m_Sidekick;
        
        // Combat ranges
        public const int MIN_SAFE_RANGE = 4;      // Minimum safe distance
        public const int OPTIMAL_RANGE = 6;       // Ideal shooting distance
        public const int MAX_RANGE = 10;          // Maximum engagement range
        
        // Health thresholds
        public const double CRITICAL_HEALTH = 0.25;
        public const double LOW_HEALTH = 0.50;
        public const double HEAL_THRESHOLD = 0.70;
        
        // Cooldowns
        private DateTime m_LastBandageTime = DateTime.MinValue;
        private DateTime m_LastHealPotionTime = DateTime.MinValue;
        private DateTime m_LastCurePotionTime = DateTime.MinValue;
        
        private const double BANDAGE_COOLDOWN = 10.0;
        private const double HEAL_POTION_COOLDOWN = 10.0;
        private const double CURE_POTION_COOLDOWN = 1.0;
        
        // Damage tracking
        private int m_LastHealth = -1;
        private bool m_TookDamageThisTick = false;
        
        // Kiting
        private int m_ConsecutiveRetreatFailures = 0;
        
        public ArcherCombatAI(AutonomousSidekick sidekick)
        {
            m_Sidekick = sidekick;
            m_LastHealth = sidekick.Hits;
        }
        
        /// <summary>
        /// Main combat loop for archers
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
            
            Utility.PushColor(ConsoleColor.Cyan);
            Console.WriteLine($"[ArcherCombatAI] {m_Sidekick.Name} - Distance: {distance}, Health: {healthPercent:P0}, Enemy: {enemy.Name}");
            Utility.PopColor();
            
            // PRIORITY 1: Critical health - emergency actions
            if (healthPercent < CRITICAL_HEALTH)
            {
                HandleCritical(enemy, distance);
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
            
            // PRIORITY 4: Positioning - maintain optimal range
            HandlePositioning(enemy, distance);
            
            // PRIORITY 5: Attack
            HandleRangedCombat(enemy, distance);
            
            return true;
        }
        
        /// <summary>
        /// Handle critical health situations
        /// </summary>
        private void HandleCritical(Mobile enemy, int distance)
        {
            Utility.PushColor(ConsoleColor.Red);
            Console.WriteLine($"[ArcherCombatAI] {m_Sidekick.Name} - CRITICAL HEALTH! Attempting emergency heal");
            Utility.PopColor();
            
            // Use heal potion
            if (TryUseHealPotion())
                return;
            
            // Use bandage
            TryUseBandage();
            
            // Try to retreat if too close
            if (distance < MIN_SAFE_RANGE)
            {
                Retreat(enemy);
            }
        }
        
        /// <summary>
        /// Handle healing
        /// </summary>
        private void HandleHealing(Mobile enemy, int distance)
        {
            double healthPercent = (double)m_Sidekick.Hits / (double)m_Sidekick.HitsMax;
            
            // If took damage, bandage while fighting
            if (m_TookDamageThisTick && !m_Sidekick.Poisoned)
            {
                if (TryUseBandage())
                {
                    Utility.PushColor(ConsoleColor.Green);
                    Console.WriteLine($"[ArcherCombatAI] {m_Sidekick.Name} - Took damage, using bandage");
                    Utility.PopColor();
                }
            }
            
            // Use potion if significantly low
            if (healthPercent < LOW_HEALTH)
            {
                TryUseHealPotion();
            }
        }
        
        /// <summary>
        /// Handle positioning - maintain optimal range from enemy
        /// </summary>
        private void HandlePositioning(Mobile enemy, int distance)
        {
            // Too close - need to retreat
            if (distance < MIN_SAFE_RANGE)
            {
                Utility.PushColor(ConsoleColor.Yellow);
                Console.WriteLine($"[ArcherCombatAI] {m_Sidekick.Name} - Too close ({distance} tiles), retreating!");
                Utility.PopColor();
                
                if (!Retreat(enemy))
                {
                    m_ConsecutiveRetreatFailures++;
                    
                    // If stuck, try to move around
                    if (m_ConsecutiveRetreatFailures >= 3)
                    {
                        Utility.PushColor(ConsoleColor.Red);
                        Console.WriteLine($"[ArcherCombatAI] {m_Sidekick.Name} - Stuck! Attempting to strafe");
                        Utility.PopColor();
                        
                        Strafe(enemy);
                    }
                }
                else
                {
                    m_ConsecutiveRetreatFailures = 0;
                }
            }
            // Too far - move closer
            else if (distance > MAX_RANGE)
            {
                Utility.PushColor(ConsoleColor.Yellow);
                Console.WriteLine($"[ArcherCombatAI] {m_Sidekick.Name} - Too far ({distance} tiles), moving closer");
                Utility.PopColor();
                
                if (m_Sidekick.AIObject is SidekickAI sidekickAI)
                {
                    sidekickAI.MoveTo(enemy, true, OPTIMAL_RANGE);
                }
            }
            // At optimal range - stay put
            else if (distance >= MIN_SAFE_RANGE && distance <= MAX_RANGE)
            {
                m_ConsecutiveRetreatFailures = 0;
            }
        }
        
        /// <summary>
        /// Handle ranged combat - face and shoot
        /// </summary>
        private void HandleRangedCombat(Mobile enemy, int distance)
        {
            // Face the enemy
            m_Sidekick.Direction = m_Sidekick.GetDirectionTo(enemy);
            
            // Combat is handled by base Mobile combat system with equipped bow
        }
        
        /// <summary>
        /// Retreat from enemy
        /// </summary>
        private bool Retreat(Mobile enemy)
        {
            if (m_Sidekick.AIObject is SidekickAI sidekickAI)
            {
                sidekickAI.RunFrom(enemy);
                return true;
            }
            return false;
        }
        
        /// <summary>
        /// Strafe around enemy when stuck
        /// </summary>
        private void Strafe(Mobile enemy)
        {
            // Try to move perpendicular to enemy
            Direction toEnemy = m_Sidekick.GetDirectionTo(enemy);
            Direction strafe = (Direction)(((int)toEnemy + 2) % 8); // 90 degrees clockwise
            
            if (!m_Sidekick.Move(strafe))
            {
                strafe = (Direction)(((int)toEnemy + 6) % 8); // 90 degrees counter-clockwise
                m_Sidekick.Move(strafe);
            }
        }
        
        #region Healing Methods
        
        private bool TryCurePoison()
        {
            if (TryUseCurePotion())
            {
                Utility.PushColor(ConsoleColor.Magenta);
                Console.WriteLine($"[ArcherCombatAI] {m_Sidekick.Name} - Used cure potion!");
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

