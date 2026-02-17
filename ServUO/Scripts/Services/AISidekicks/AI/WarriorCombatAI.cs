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
    /// Combat AI for Warrior archetype - melee tank with healing
    /// Behavior: Stay in melee range, use bandages/potions, tank damage
    /// </summary>
    public class WarriorCombatAI
    {
        private AutonomousSidekick m_Sidekick;
        
        // Health thresholds
        public const double CRITICAL_HEALTH = 0.25;  // 25% - emergency heal
        public const double LOW_HEALTH = 0.50;       // 50% - regular heal
        public const double HEAL_THRESHOLD = 0.70;   // 70% - bandage if took damage
        
        // Cooldowns
        private DateTime m_LastBandageTime = DateTime.MinValue;
        private DateTime m_LastHealPotionTime = DateTime.MinValue;
        private DateTime m_LastCurePotionTime = DateTime.MinValue;
        private DateTime m_LastAgilityPotionTime = DateTime.MinValue;
        
        private const double BANDAGE_COOLDOWN = 10.0;
        private const double HEAL_POTION_COOLDOWN = 10.0;
        private const double CURE_POTION_COOLDOWN = 1.0;
        private const double AGILITY_POTION_COOLDOWN = 120.0;
        
        // Damage tracking
        private int m_LastHealth = -1;
        private bool m_TookDamageThisTick = false;
        
        // Weapon re-equip tracking
        private DateTime m_LastSpellCastTime = DateTime.MinValue;
        private const double WEAPON_REEQUIP_DELAY = 0.5; // Wait for spell to finish before re-equipping
        
        public WarriorCombatAI(AutonomousSidekick sidekick)
        {
            m_Sidekick = sidekick;
            m_LastHealth = sidekick.Hits;
        }
        
        /// <summary>
        /// Check if weapon is missing and re-equip it from backpack
        /// This is needed because spell casting clears hands
        /// </summary>
        private bool TryReequipWeapon()
        {
            // Don't try to re-equip too soon after casting (spell might still be in progress)
            if ((DateTime.UtcNow - m_LastSpellCastTime).TotalSeconds < WEAPON_REEQUIP_DELAY)
                return false;
            
            // Check if weapon is already equipped
            Item rightHand = m_Sidekick.FindItemOnLayer(Layer.TwoHanded);
            if (rightHand == null)
                rightHand = m_Sidekick.FindItemOnLayer(Layer.OneHanded);
            
            if (rightHand is BaseWeapon)
                return false; // Already have a weapon equipped
            
            // Find weapon in backpack
            Container pack = m_Sidekick.Backpack;
            if (pack == null)
                return false;
            
            // Look for any weapon (prioritize axes for lumberjack builds)
            BaseWeapon weapon = null;
            
            // First try to find an axe (for Lumberjacking bonus)
            weapon = pack.FindItemByType(typeof(DoubleAxe)) as BaseWeapon;
            if (weapon == null)
                weapon = pack.FindItemByType(typeof(LargeBattleAxe)) as BaseWeapon;
            if (weapon == null)
                weapon = pack.FindItemByType(typeof(ExecutionersAxe)) as BaseWeapon;
            if (weapon == null)
                weapon = pack.FindItemByType(typeof(TwoHandedAxe)) as BaseWeapon;
            
            // Fallback to any weapon
            if (weapon == null)
                weapon = pack.FindItemByType(typeof(BaseWeapon)) as BaseWeapon;
            
            if (weapon == null)
                return false;
            
            // Equip the weapon
            if (m_Sidekick.EquipItem(weapon))
            {
                Utility.PushColor(ConsoleColor.Cyan);
                Console.WriteLine($"[WarriorCombatAI] {m_Sidekick.Name} - Re-equipped weapon: {weapon.GetType().Name}");
                Utility.PopColor();
                return true;
            }
            
            return false;
        }
        
        /// <summary>
        /// Main combat loop for warriors
        /// </summary>
        public bool DoCombat(Mobile enemy)
        {
            if (m_Sidekick == null || m_Sidekick.Deleted || !m_Sidekick.Alive)
                return false;
                
            if (enemy == null || enemy.Deleted || !enemy.Alive)
                return false;
            
            // Check if weapon needs to be re-equipped (spell casting clears hands)
            TryReequipWeapon();
            
            // Track damage taken
            m_TookDamageThisTick = m_LastHealth > 0 && m_Sidekick.Hits < m_LastHealth;
            m_LastHealth = m_Sidekick.Hits;
            
            double healthPercent = (double)m_Sidekick.Hits / (double)m_Sidekick.HitsMax;
            int distance = (int)m_Sidekick.GetDistanceToSqrt(enemy);
            
            // PRIORITY 1: Critical health - emergency actions
            if (healthPercent < CRITICAL_HEALTH)
            {
                if (HandleCritical(enemy, distance))
                    return true;
            }
            
            // PRIORITY 2: Cure poison
            if (m_Sidekick.Poisoned)
            {
                if (TryCurePoison())
                    return true;
            }
            
            // PRIORITY 3: Heal if low health or took damage
            if (healthPercent < LOW_HEALTH || (m_TookDamageThisTick && healthPercent < HEAL_THRESHOLD))
            {
                if (HandleHealing(enemy, distance))
                    return true;
            }
            
            // PRIORITY 4: Engage in melee combat
            return HandleMeleeCombat(enemy, distance);
        }
        
        /// <summary>
        /// Handle critical health situations
        /// </summary>
        private bool HandleCritical(Mobile enemy, int distance)
        {
            Utility.PushColor(ConsoleColor.Red);
            Console.WriteLine($"[WarriorCombatAI] {m_Sidekick.Name} - CRITICAL HEALTH ({m_Sidekick.Hits}/{m_Sidekick.HitsMax})!");
            Utility.PopColor();
            
            // Try heal potion first (instant)
            if (TryUseHealPotion())
            {
                Utility.PushColor(ConsoleColor.Magenta);
                Console.WriteLine($"[WarriorCombatAI] {m_Sidekick.Name} - Used heal potion in emergency!");
                Utility.PopColor();
                return true;
            }
            
            // Try bandage
            if (TryUseBandage())
            {
                Utility.PushColor(ConsoleColor.Green);
                Console.WriteLine($"[WarriorCombatAI] {m_Sidekick.Name} - Using bandage in emergency!");
                Utility.PopColor();
                return true;
            }
            
            // If we have magery skill, try casting Greater Heal
            if (m_Sidekick.Skills.Magery.Value >= 50 && m_Sidekick.Mana >= 11)
            {
                var healSpell = new GreaterHealSpell(m_Sidekick, null);
                healSpell.Cast();
                m_LastSpellCastTime = DateTime.UtcNow; // Track for weapon re-equip
                
                Utility.PushColor(ConsoleColor.Green);
                Console.WriteLine($"[WarriorCombatAI] {m_Sidekick.Name} - Casting Greater Heal in emergency!");
                Utility.PopColor();
                return true;
            }
            
            // Retreat to heal when critical - warriors need to survive to keep fighting
            if (distance <= 2)
            {
                Utility.PushColor(ConsoleColor.Yellow);
                Console.WriteLine($"[WarriorCombatAI] {m_Sidekick.Name} - RETREATING to heal (critical health)!");
                Utility.PopColor();
                
                if (m_Sidekick.AIObject is SidekickAI sidekickAI)
                {
                    sidekickAI.RunFrom(enemy);
                    return true;
                }
            }
            
            return false;
        }
        
        /// <summary>
        /// Handle healing when needed
        /// </summary>
        private bool HandleHealing(Mobile enemy, int distance)
        {
            double healthPercent = (double)m_Sidekick.Hits / (double)m_Sidekick.HitsMax;
            
            // If took damage this tick, immediately try to bandage
            if (m_TookDamageThisTick && !m_Sidekick.Poisoned)
            {
                if (TryUseBandage())
                {
                    Utility.PushColor(ConsoleColor.Green);
                    Console.WriteLine($"[WarriorCombatAI] {m_Sidekick.Name} - Took damage ({m_Sidekick.Hits}/{m_Sidekick.HitsMax}), using bandage");
                    Utility.PopColor();
                    // Don't return - keep fighting while bandaging
                }
            }
            
            // If significantly low, use potion
            if (healthPercent < LOW_HEALTH)
            {
                if (TryUseHealPotion())
                {
                    Utility.PushColor(ConsoleColor.Magenta);
                    Console.WriteLine($"[WarriorCombatAI] {m_Sidekick.Name} - Low health, used heal potion!");
                    Utility.PopColor();
                    // Don't return - keep fighting
                }
            }
            
            // Try bandage if not already bandaging and below threshold
            if (healthPercent < HEAL_THRESHOLD && !m_Sidekick.Poisoned)
            {
                TryUseBandage();
            }
            
            return false; // Continue to melee combat
        }
        
        /// <summary>
        /// Handle melee combat - stay close and swing
        /// </summary>
        private bool HandleMeleeCombat(Mobile enemy, int distance)
        {
            // Ensure we're in melee range
            if (distance > 1)
            {
                // Move to enemy
                if (m_Sidekick.AIObject is SidekickAI sidekickAI)
                {
                    sidekickAI.MoveTo(enemy, true, 1);
                }
            }
            
            // Face the enemy
            m_Sidekick.Direction = m_Sidekick.GetDirectionTo(enemy);
            
            // Combat is handled by base Mobile combat system
            return true;
        }
        
        #region Healing Methods
        
        /// <summary>
        /// Try to cure poison with potion or spell
        /// </summary>
        private bool TryCurePoison()
        {
            // Try cure potion first
            if (TryUseCurePotion())
            {
                Utility.PushColor(ConsoleColor.Magenta);
                Console.WriteLine($"[WarriorCombatAI] {m_Sidekick.Name} - Used cure potion!");
                Utility.PopColor();
                return true;
            }
            
            // Try cure spell if we have magery
            if (m_Sidekick.Skills.Magery.Value >= 40 && m_Sidekick.Mana >= 6)
            {
                var cureSpell = new CureSpell(m_Sidekick, null);
                cureSpell.Cast();
                m_LastSpellCastTime = DateTime.UtcNow; // Track for weapon re-equip
                
                Utility.PushColor(ConsoleColor.Green);
                Console.WriteLine($"[WarriorCombatAI] {m_Sidekick.Name} - Casting Cure spell!");
                Utility.PopColor();
                return true;
            }
            
            return false;
        }
        
        /// <summary>
        /// Try to use a bandage
        /// </summary>
        public bool TryUseBandage()
        {
            // Don't waste bandages at full health
            if (m_Sidekick.Hits >= m_Sidekick.HitsMax)
                return false;
            
            // Check cooldown
            double cooldownRemaining = BANDAGE_COOLDOWN - (DateTime.UtcNow - m_LastBandageTime).TotalSeconds;
            if (cooldownRemaining > 0)
                return false;
            
            // Find bandages
            Container pack = m_Sidekick.Backpack;
            if (pack == null)
                return false;
            
            Item bandage = pack.FindItemByType(typeof(Bandage));
            if (bandage == null || bandage.Amount < 1)
                return false;
            
            // Check if already bandaging
            var bandageContext = BandageContext.GetContext(m_Sidekick);
            if (bandageContext != null)
                return false;
            
            // Start bandage heal
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
        
        /// <summary>
        /// Try to use a Greater Heal Potion
        /// </summary>
        private bool TryUseHealPotion()
        {
            // Check cooldown
            double cooldownRemaining = HEAL_POTION_COOLDOWN - (DateTime.UtcNow - m_LastHealPotionTime).TotalSeconds;
            if (cooldownRemaining > 0)
                return false;
            
            // Don't use at full health
            if (m_Sidekick.Hits >= m_Sidekick.HitsMax)
                return false;
            
            // Don't use while poisoned
            if (m_Sidekick.Poisoned)
                return false;
            
            Container pack = m_Sidekick.Backpack;
            if (pack == null)
                return false;
            
            // Look for Greater Heal Potion
            GreaterHealPotion potion = pack.FindItemByType(typeof(GreaterHealPotion)) as GreaterHealPotion;
            if (potion == null)
                return false;
            
            // Use it
            potion.Drink(m_Sidekick);
            m_LastHealPotionTime = DateTime.UtcNow;
            return true;
        }
        
        /// <summary>
        /// Try to use a Greater Cure Potion
        /// </summary>
        private bool TryUseCurePotion()
        {
            // Check cooldown
            double cooldownRemaining = CURE_POTION_COOLDOWN - (DateTime.UtcNow - m_LastCurePotionTime).TotalSeconds;
            if (cooldownRemaining > 0)
                return false;
            
            // Only use when poisoned
            if (!m_Sidekick.Poisoned)
                return false;
            
            Container pack = m_Sidekick.Backpack;
            if (pack == null)
                return false;
            
            // Look for Greater Cure Potion
            GreaterCurePotion potion = pack.FindItemByType(typeof(GreaterCurePotion)) as GreaterCurePotion;
            if (potion == null)
                return false;
            
            // Use it
            potion.Drink(m_Sidekick);
            m_LastCurePotionTime = DateTime.UtcNow;
            return true;
        }
        
        /// <summary>
        /// Try to use an Agility Potion for speed boost
        /// </summary>
        public bool TryUseAgilityPotion()
        {
            // Check cooldown
            double cooldownRemaining = AGILITY_POTION_COOLDOWN - (DateTime.UtcNow - m_LastAgilityPotionTime).TotalSeconds;
            if (cooldownRemaining > 0)
                return false;
            
            Container pack = m_Sidekick.Backpack;
            if (pack == null)
                return false;
            
            // Look for Greater Agility Potion
            GreaterAgilityPotion potion = pack.FindItemByType(typeof(GreaterAgilityPotion)) as GreaterAgilityPotion;
            if (potion == null)
                return false;
            
            // Use it
            potion.Drink(m_Sidekick);
            m_LastAgilityPotionTime = DateTime.UtcNow;
            return true;
        }
        
        #endregion
        
        /// <summary>
        /// Public wrapper for bandage usage
        /// </summary>
        public bool TryUseBandagePublic()
        {
            return TryUseBandage();
        }
    }
}

