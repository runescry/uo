using System;
using Server;
using Server.Mobiles;
using Server.Items;
using Server.Spells;
using Server.Spells.Chivalry;
using Server.Spells.Fourth;
using Server.Spells.Second;

namespace Server.Services.AISidekicks
{
    /// <summary>
    /// Combat AI for Paladin archetype - melee tank with Chivalry abilities
    /// Behavior: Stay in melee, use Close Wounds, Consecrate Weapon, Divine Fury
    /// </summary>
    public class PaladinCombatAI
    {
        private AutonomousSidekick m_Sidekick;
        
        // Health thresholds
        public const double CRITICAL_HEALTH = 0.25;
        public const double LOW_HEALTH = 0.50;
        public const double HEAL_THRESHOLD = 0.70;
        
        // Cooldowns
        private DateTime m_LastBandageTime = DateTime.MinValue;
        private DateTime m_LastHealPotionTime = DateTime.MinValue;
        private DateTime m_LastCurePotionTime = DateTime.MinValue;
        private DateTime m_LastCloseWoundsTime = DateTime.MinValue;
        private DateTime m_LastConsecrateTime = DateTime.MinValue;
        private DateTime m_LastDivineFuryTime = DateTime.MinValue;
        
        private const double BANDAGE_COOLDOWN = 10.0;
        private const double HEAL_POTION_COOLDOWN = 10.0;
        private const double CURE_POTION_COOLDOWN = 1.0;
        private const double CLOSE_WOUNDS_COOLDOWN = 4.0;
        private const double CONSECRATE_COOLDOWN = 15.0;
        private const double DIVINE_FURY_COOLDOWN = 20.0;
        
        // Damage tracking
        private int m_LastHealth = -1;
        private bool m_TookDamageThisTick = false;
        
        // Chivalry buff tracking
        private bool m_ConsecrateActive = false;
        private bool m_DivineFuryActive = false;
        
        public PaladinCombatAI(AutonomousSidekick sidekick)
        {
            m_Sidekick = sidekick;
            m_LastHealth = sidekick.Hits;
        }
        
        /// <summary>
        /// Main combat loop for paladins
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
            
            Utility.PushColor(ConsoleColor.White);
            Console.WriteLine($"[PaladinCombatAI] {m_Sidekick.Name} - Distance: {distance}, Health: {healthPercent:P0}, Enemy: {enemy.Name}");
            Utility.PopColor();
            
            // PRIORITY 1: Critical health - emergency heal
            if (healthPercent < CRITICAL_HEALTH)
            {
                HandleCritical(enemy, distance);
            }
            
            // PRIORITY 2: Cure poison (Cleanse by Fire or potion)
            if (m_Sidekick.Poisoned)
            {
                TryCurePoison();
            }
            
            // PRIORITY 3: Heal if needed (Close Wounds or bandage)
            if (healthPercent < LOW_HEALTH || (m_TookDamageThisTick && healthPercent < HEAL_THRESHOLD))
            {
                HandleHealing(enemy, distance);
            }
            
            // PRIORITY 4: Apply offensive buffs before engaging
            HandleBuffs(enemy, distance);
            
            // PRIORITY 5: Melee combat
            return HandleMeleeCombat(enemy, distance);
        }
        
        /// <summary>
        /// Handle critical health situations
        /// </summary>
        private void HandleCritical(Mobile enemy, int distance)
        {
            Utility.PushColor(ConsoleColor.Red);
            Console.WriteLine($"[PaladinCombatAI] {m_Sidekick.Name} - CRITICAL HEALTH!");
            Utility.PopColor();
            
            // Try Close Wounds first (instant)
            if (TryCastCloseWounds())
            {
                return;
            }
            
            // Try heal potion
            if (TryUseHealPotion())
            {
                return;
            }
            
            // Try bandage
            TryUseBandage();
        }
        
        /// <summary>
        /// Handle healing
        /// </summary>
        private void HandleHealing(Mobile enemy, int distance)
        {
            double healthPercent = (double)m_Sidekick.Hits / (double)m_Sidekick.HitsMax;
            
            // If took damage, try Close Wounds (faster than bandage)
            if (m_TookDamageThisTick)
            {
                if (TryCastCloseWounds())
                {
                    Utility.PushColor(ConsoleColor.Yellow);
                    Console.WriteLine($"[PaladinCombatAI] {m_Sidekick.Name} - Took damage, casting Close Wounds");
                    Utility.PopColor();
                    return;
                }
                
                // Fallback to bandage
                if (!m_Sidekick.Poisoned)
                {
                    TryUseBandage();
                }
            }
            
            // Use potion if low
            if (healthPercent < LOW_HEALTH)
            {
                TryUseHealPotion();
            }
        }
        
        /// <summary>
        /// Handle offensive buffs
        /// </summary>
        private void HandleBuffs(Mobile enemy, int distance)
        {
            // If about to engage in melee, apply buffs
            if (distance <= 3)
            {
                // Consecrate Weapon - extra damage
                if (!m_ConsecrateActive && TryCastConsecrateWeapon())
                {
                    Utility.PushColor(ConsoleColor.Yellow);
                    Console.WriteLine($"[PaladinCombatAI] {m_Sidekick.Name} - Consecrate Weapon!");
                    Utility.PopColor();
                    m_ConsecrateActive = true;
                    Timer.DelayCall(TimeSpan.FromSeconds(10), () => m_ConsecrateActive = false);
                }
                
                // Divine Fury - attack speed boost
                if (!m_DivineFuryActive && TryCastDivineFury())
                {
                    Utility.PushColor(ConsoleColor.Yellow);
                    Console.WriteLine($"[PaladinCombatAI] {m_Sidekick.Name} - Divine Fury!");
                    Utility.PopColor();
                    m_DivineFuryActive = true;
                    Timer.DelayCall(TimeSpan.FromSeconds(8), () => m_DivineFuryActive = false);
                }
            }
        }
        
        /// <summary>
        /// Handle melee combat - stay close and swing
        /// </summary>
        private bool HandleMeleeCombat(Mobile enemy, int distance)
        {
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
            
            return true;
        }
        
        #region Chivalry Spells
        
        /// <summary>
        /// Cast Close Wounds (self-heal)
        /// </summary>
        private bool TryCastCloseWounds()
        {
            double cooldownRemaining = CLOSE_WOUNDS_COOLDOWN - (DateTime.UtcNow - m_LastCloseWoundsTime).TotalSeconds;
            if (cooldownRemaining > 0)
                return false;
            
            if (m_Sidekick.Skills.Chivalry.Value < 40)
                return false;
            
            if (m_Sidekick.Mana < 10)
                return false;
            
            try
            {
                var spell = new CloseWoundsSpell(m_Sidekick, null);
                spell.Cast();
                m_LastCloseWoundsTime = DateTime.UtcNow;
                return true;
            }
            catch
            {
                return false;
            }
        }
        
        /// <summary>
        /// Cast Consecrate Weapon (bonus damage)
        /// </summary>
        private bool TryCastConsecrateWeapon()
        {
            double cooldownRemaining = CONSECRATE_COOLDOWN - (DateTime.UtcNow - m_LastConsecrateTime).TotalSeconds;
            if (cooldownRemaining > 0)
                return false;
            
            if (m_Sidekick.Skills.Chivalry.Value < 15)
                return false;
            
            if (m_Sidekick.Mana < 10)
                return false;
            
            try
            {
                var spell = new ConsecrateWeaponSpell(m_Sidekick, null);
                spell.Cast();
                m_LastConsecrateTime = DateTime.UtcNow;
                return true;
            }
            catch
            {
                return false;
            }
        }
        
        /// <summary>
        /// Cast Divine Fury (attack speed)
        /// </summary>
        private bool TryCastDivineFury()
        {
            double cooldownRemaining = DIVINE_FURY_COOLDOWN - (DateTime.UtcNow - m_LastDivineFuryTime).TotalSeconds;
            if (cooldownRemaining > 0)
                return false;
            
            if (m_Sidekick.Skills.Chivalry.Value < 25)
                return false;
            
            if (m_Sidekick.Mana < 15)
                return false;
            
            try
            {
                var spell = new DivineFurySpell(m_Sidekick, null);
                spell.Cast();
                m_LastDivineFuryTime = DateTime.UtcNow;
                return true;
            }
            catch
            {
                return false;
            }
        }
        
        #endregion
        
        #region Healing Methods
        
        private bool TryCurePoison()
        {
            // Try Cleanse by Fire first
            if (m_Sidekick.Skills.Chivalry.Value >= 5 && m_Sidekick.Mana >= 10)
            {
                try
                {
                    var spell = new CleanseByFireSpell(m_Sidekick, null);
                    spell.Cast();
                    
                    Utility.PushColor(ConsoleColor.Yellow);
                    Console.WriteLine($"[PaladinCombatAI] {m_Sidekick.Name} - Cleanse by Fire!");
                    Utility.PopColor();
                    return true;
                }
                catch { }
            }
            
            // Fallback to cure potion
            if (TryUseCurePotion())
            {
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

