using System;
using Server;
using Server.Mobiles;
using Server.Items;
using Server.Spells;
using Server.Spells.First;
using Server.Spells.Second;
using Server.Spells.Fourth;

namespace Server.Services.AISidekicks
{
    /// <summary>
    /// Combat AI for Healer/Cleric archetype - support/healing focused
    /// Behavior: Prioritize healing owner/self, maintain safe distance, offensive spells secondary
    /// </summary>
    public class HealerCombatAI
    {
        private AutonomousSidekick m_Sidekick;
        
        // Combat ranges
        public const int MIN_SAFE_RANGE = 6;      // Stay back from combat
        public const int OPTIMAL_RANGE = 8;       // Ideal casting distance
        public const int MAX_RANGE = 12;          // Maximum engagement range
        public const int BANDAGE_RANGE = 2;       // Must be within 2 tiles to bandage (matches Bandage.Range)
        
        // Health thresholds
        public const double CRITICAL_HEALTH = 0.25;
        public const double LOW_HEALTH = 0.50;
        public const double HEAL_THRESHOLD = 0.70;
        public const double OWNER_HEAL_THRESHOLD = 0.80; // More aggressive about healing owner
        
        // Cooldowns
        private DateTime m_LastBandageTime = DateTime.MinValue;
        private DateTime m_LastHealPotionTime = DateTime.MinValue;
        private DateTime m_LastCurePotionTime = DateTime.MinValue;
        private DateTime m_LastHealSpellTime = DateTime.MinValue;
        private DateTime m_LastOwnerHealTime = DateTime.MinValue;
        
        private const double BANDAGE_COOLDOWN = 10.0;
        private const double HEAL_POTION_COOLDOWN = 10.0;
        private const double CURE_POTION_COOLDOWN = 1.0;
        private const double HEAL_SPELL_COOLDOWN = 2.0;
        private const double OWNER_HEAL_COOLDOWN = 3.0;
        
        // Damage tracking
        private int m_LastHealth = -1;
        private bool m_TookDamageThisTick = false;

        // Stuck detection for pet healing
        private int m_StuckCounter = 0;
        private int m_LastDistanceToPet = -1;
        private Mobile m_LastPetTarget = null;
        private const int STUCK_THRESHOLD = 3; // If stuck for 3+ ticks, use spell instead

        public HealerCombatAI(AutonomousSidekick sidekick)
        {
            m_Sidekick = sidekick;
            m_LastHealth = sidekick.Hits;
        }
        
        /// <summary>
        /// Main loop for healers - SUPPORT ONLY, NO COMBAT
        /// Healer focuses entirely on keeping owner and pets alive
        /// </summary>
        public bool DoCombat(Mobile enemy)
        {
            if (m_Sidekick == null || m_Sidekick.Deleted || !m_Sidekick.Alive)
                return false;

            // CRITICAL: If we're actively bandaging, STAY IN PLACE - don't move!
            var activeBandage = BandageContext.GetContext(m_Sidekick);
            if (activeBandage != null)
            {
                Utility.PushColor(ConsoleColor.Cyan);
                Console.WriteLine($"[HealerCombatAI] {m_Sidekick.Name} - BANDAGING IN PROGRESS - staying in place!");
                Utility.PopColor();
                return true; // Block all other actions while bandaging
            }

            // Healer works even without an enemy - it's a support class
            // Track damage taken
            m_TookDamageThisTick = m_LastHealth > 0 && m_Sidekick.Hits < m_LastHealth;
            m_LastHealth = m_Sidekick.Hits;
            
            double healthPercent = (double)m_Sidekick.Hits / (double)m_Sidekick.HitsMax;
            
            Utility.PushColor(ConsoleColor.White);
            Console.WriteLine($"[HealerCombatAI] {m_Sidekick.Name} - Health: {healthPercent:P0}, Mode: SUPPORT ONLY");
            Utility.PopColor();
            
            // PRIORITY 1: SELF-PRESERVATION - A dead healer can't heal anyone!
            // Critical self-health check MUST come first
            if (healthPercent < CRITICAL_HEALTH)
            {
                Utility.PushColor(ConsoleColor.Red);
                Console.WriteLine($"[HealerCombatAI] {m_Sidekick.Name} - CRITICAL SELF-HEALTH! Must heal self first!");
                Utility.PopColor();
                
                if (HandleCritical(enemy))
                    return true;
            }
            
            // PRIORITY 2: Cure self poison (can kill quickly)
            if (m_Sidekick.Poisoned)
            {
                if (TryUseCurePotion())
                    return true;

                // Try cure spell on self
                if (m_Sidekick.Mana >= 6)
                {
                    var cureSpell = new CureSpell(m_Sidekick, null);
                    cureSpell.Cast();
                    return true;
                }
            }
            
            // PRIORITY 3: Self-heal if moderately low (prevent getting to critical)
            if (healthPercent < LOW_HEALTH)
            {
                Utility.PushColor(ConsoleColor.Yellow);
                Console.WriteLine($"[HealerCombatAI] {m_Sidekick.Name} - Low health ({healthPercent:P0}), healing self before others");
                Utility.PopColor();
                
                HandleSelfHealing();
                // Don't return - continue to help others if we started a heal
            }
            
            // PRIORITY 4: Check and heal owner first (spells + bandages)
            if (CheckAndHealOwner())
            {
                return true;
            }
            
            // PRIORITY 5: Check and heal other sidekicks (party members)
            if (CheckAndHealOtherSidekicks())
            {
                return true;
            }
            
            // PRIORITY 6: Check and heal owner's pets
            if (CheckAndHealOwnerPets())
            {
                return true;
            }
            
            // PRIORITY 7: Cure poison (owner, sidekicks, pets)
            if (HandlePoison())
            {
                return true;
            }
            
            // PRIORITY 8: Proactive self-heal if took damage
            if (m_TookDamageThisTick && healthPercent < HEAL_THRESHOLD)
            {
                HandleSelfHealing();
            }
            
            // PRIORITY 9: Stay near owner (for bandaging range)
            StayNearOwner();
            
            // NO OFFENSIVE SPELLS - Healer is pure support
            
            return true;
        }
        
        /// <summary>
        /// Check owner health and heal if needed (spells + bandages)
        /// </summary>
        private bool CheckAndHealOwner()
        {
            Mobile owner = m_Sidekick.ControlMaster ?? m_Sidekick.Owner;
            if (owner == null || owner.Deleted || !owner.Alive)
                return false;
            
            double ownerHealthPercent = (double)owner.Hits / (double)owner.HitsMax;
            
            // Heal owner if below threshold
            if (ownerHealthPercent < OWNER_HEAL_THRESHOLD)
            {
                int distanceToOwner = (int)m_Sidekick.GetDistanceToSqrt(owner);
                
                // Cure owner poison first (if in spell range)
                if (owner.Poisoned && m_Sidekick.Mana >= 6 && distanceToOwner <= 10)
                {
                    var cureSpell = new CureSpell(m_Sidekick, null);
                    cureSpell.Cast();
                    m_LastOwnerHealTime = DateTime.UtcNow;
                    
                    Utility.PushColor(ConsoleColor.Green);
                    Console.WriteLine($"[HealerCombatAI] {m_Sidekick.Name} - Curing owner {owner.Name}!");
                    Utility.PopColor();
                    
                    Timer.DelayCall(TimeSpan.FromMilliseconds(500), () => 
                    {
                        if (m_Sidekick.Target != null)
                            m_Sidekick.Target.Invoke(m_Sidekick, owner);
                    });
                    return true;
                }
                
                // Try spell healing if mana available and in range
                if (m_Sidekick.Mana >= 11 && distanceToOwner <= 10)
                {
                    double cooldownRemaining = OWNER_HEAL_COOLDOWN - (DateTime.UtcNow - m_LastOwnerHealTime).TotalSeconds;
                    if (cooldownRemaining <= 0)
                    {
                        if (ownerHealthPercent < 0.50)
                        {
                            var healSpell = new GreaterHealSpell(m_Sidekick, null);
                            healSpell.Cast();
                            
                            Utility.PushColor(ConsoleColor.Green);
                            Console.WriteLine($"[HealerCombatAI] {m_Sidekick.Name} - Casting Greater Heal on owner {owner.Name} ({ownerHealthPercent:P0})");
                            Utility.PopColor();
                        }
                        else
                        {
                            var healSpell = new HealSpell(m_Sidekick, null);
                            healSpell.Cast();
                            
                            Utility.PushColor(ConsoleColor.Green);
                            Console.WriteLine($"[HealerCombatAI] {m_Sidekick.Name} - Casting Heal on owner {owner.Name} ({ownerHealthPercent:P0})");
                            Utility.PopColor();
                        }
                        
                        m_LastOwnerHealTime = DateTime.UtcNow;
                        
                        Timer.DelayCall(TimeSpan.FromMilliseconds(500), () => 
                        {
                            if (m_Sidekick.Target != null)
                                m_Sidekick.Target.Invoke(m_Sidekick, owner);
                        });
                        return true;
                    }
                }
                
                // Low mana or spell on cooldown - try bandaging owner
                if (distanceToOwner <= 2)
                {
                    // Close enough to bandage
                    if (TryBandageTarget(owner))
                    {
                        Utility.PushColor(ConsoleColor.Cyan);
                        Console.WriteLine($"[HealerCombatAI] {m_Sidekick.Name} - Bandaging owner {owner.Name}");
                        Utility.PopColor();
                        return true;
                    }
                }
                else
                {
                    // Move closer to bandage owner
                    Utility.PushColor(ConsoleColor.Yellow);
                    Console.WriteLine($"[HealerCombatAI] {m_Sidekick.Name} - Moving to owner to bandage (distance: {distanceToOwner})");
                    Utility.PopColor();
                    
                    if (m_Sidekick.AIObject is SidekickAI sidekickAI)
                    {
                        sidekickAI.MoveTo(owner, true, 1);
                    }
                    return false;
                }
            }
            
            return false;
        }
        
        /// <summary>
        /// Check owner's other sidekicks and heal if needed (PRIORITY)
        /// </summary>
        private bool CheckAndHealOtherSidekicks()
        {
            Mobile owner = m_Sidekick.ControlMaster ?? m_Sidekick.Owner;
            if (owner == null || !(owner is PlayerMobile pm))
                return false;
            
            // Find injured sidekicks (prioritize over pets)
            AutonomousSidekick injuredSidekick = null;
            double lowestHealthPct = 1.0;
            
            foreach (Mobile m in pm.AllFollowers)
            {
                if (m == m_Sidekick || m.Deleted || !m.Alive)
                    continue;
                
                // Only look for other sidekicks here
                if (m is AutonomousSidekick sk)
                {
                    double pct = (double)sk.Hits / sk.HitsMax;
                    
                    Utility.PushColor(ConsoleColor.Gray);
                    Console.WriteLine($"[HealerCombatAI] {m_Sidekick.Name} - Checking sidekick {sk.Name}: {pct:P0} health");
                    Utility.PopColor();
                    
                    if (pct < ALLY_HEAL_THRESHOLD && pct < lowestHealthPct)
                    {
                        lowestHealthPct = pct;
                        injuredSidekick = sk;
                    }
                }
            }
            
            if (injuredSidekick == null)
                return false;
            
            int distanceToAlly = (int)m_Sidekick.GetDistanceToSqrt(injuredSidekick);
            
            Utility.PushColor(ConsoleColor.Yellow);
            Console.WriteLine($"[HealerCombatAI] {m_Sidekick.Name} - Found injured sidekick {injuredSidekick.Name} ({lowestHealthPct:P0}), distance: {distanceToAlly}");
            Utility.PopColor();
            
            // Cure sidekick poison first
            if (injuredSidekick.Poisoned && m_Sidekick.Mana >= 6 && distanceToAlly <= 10)
            {
                var cureSpell = new CureSpell(m_Sidekick, null);
                cureSpell.Cast();
                
                Utility.PushColor(ConsoleColor.Green);
                Console.WriteLine($"[HealerCombatAI] {m_Sidekick.Name} - Curing sidekick {injuredSidekick.Name}!");
                Utility.PopColor();
                
                Timer.DelayCall(TimeSpan.FromMilliseconds(500), () => 
                {
                    if (m_Sidekick.Target != null)
                        m_Sidekick.Target.Invoke(m_Sidekick, injuredSidekick);
                });
                return true;
            }
            
            // Critical sidekick - use Greater Heal spell
            if (lowestHealthPct < 0.50 && m_Sidekick.Mana >= 11 && distanceToAlly <= 10)
            {
                var healSpell = new GreaterHealSpell(m_Sidekick, null);
                healSpell.Cast();
                
                Utility.PushColor(ConsoleColor.Green);
                Console.WriteLine($"[HealerCombatAI] {m_Sidekick.Name} - Casting Greater Heal on sidekick {injuredSidekick.Name} ({lowestHealthPct:P0})");
                Utility.PopColor();
                
                Timer.DelayCall(TimeSpan.FromMilliseconds(500), () => 
                {
                    if (m_Sidekick.Target != null)
                        m_Sidekick.Target.Invoke(m_Sidekick, injuredSidekick);
                });
                return true;
            }
            
            // Bandage sidekick (uses Healing + Anatomy for humanoids)
            if (distanceToAlly <= 2)
            {
                if (TryBandageTarget(injuredSidekick))
                {
                    Utility.PushColor(ConsoleColor.Cyan);
                    Console.WriteLine($"[HealerCombatAI] {m_Sidekick.Name} - Bandaging sidekick {injuredSidekick.Name}");
                    Utility.PopColor();
                    return true;
                }
            }
            else
            {
                // Move closer to sidekick
                Utility.PushColor(ConsoleColor.Yellow);
                Console.WriteLine($"[HealerCombatAI] {m_Sidekick.Name} - Moving to sidekick {injuredSidekick.Name} to bandage");
                Utility.PopColor();
                
                if (m_Sidekick.AIObject is SidekickAI sidekickAI)
                {
                    sidekickAI.MoveTo(injuredSidekick, true, 1);
                }
            }
            
            return false;
        }
        
        // Ally (sidekick) heal threshold - more aggressive than pets
        private const double ALLY_HEAL_THRESHOLD = 0.80;
        
        /// <summary>
        /// Check owner's pets AND pets owned by other sidekicks and heal if needed
        /// </summary>
        private bool CheckAndHealOwnerPets()
        {
            Mobile owner = m_Sidekick.ControlMaster ?? m_Sidekick.Owner;
            if (owner == null || !(owner is PlayerMobile pm))
                return false;
            
            // Find injured pets (exclude sidekicks - handled separately)
            BaseCreature injuredPet = null;
            double lowestHealthPct = 1.0;
            
            // First, check player's direct pets
            foreach (Mobile m in pm.AllFollowers)
            {
                if (m == m_Sidekick || m.Deleted || !m.Alive)
                    continue;
                
                // Skip sidekicks - they're handled by CheckAndHealOtherSidekicks
                if (m is AutonomousSidekick)
                    continue;
                
                if (m is BaseCreature bc)
                {
                    double pct = (double)bc.Hits / bc.HitsMax;
                    if (pct < PET_HEAL_THRESHOLD && pct < lowestHealthPct)
                    {
                        lowestHealthPct = pct;
                        injuredPet = bc;
                    }
                }
            }
            
            // Second, check pets owned by OTHER SIDEKICKS (like Tamer's dragon)
            foreach (Mobile m in pm.AllFollowers)
            {
                if (m == m_Sidekick || m.Deleted || !m.Alive)
                    continue;
                
                // Look at other sidekicks
                if (m is AutonomousSidekick otherSidekick)
                {
                    // Check this sidekick's pets (like a Tamer's dragon)
                    var nearbyMobiles = otherSidekick.GetMobilesInRange(20);
                    foreach (Mobile pet in nearbyMobiles)
                    {
                        if (pet == m_Sidekick || pet.Deleted || !pet.Alive)
                            continue;
                        
                        if (pet is BaseCreature bc && bc.ControlMaster == otherSidekick)
                        {
                            double pct = (double)bc.Hits / bc.HitsMax;
                            
                            Utility.PushColor(ConsoleColor.Gray);
                            Console.WriteLine($"[HealerCombatAI] {m_Sidekick.Name} - Found {otherSidekick.Name}'s pet {bc.Name}: {pct:P0} health");
                            Utility.PopColor();
                            
                            if (pct < PET_HEAL_THRESHOLD && pct < lowestHealthPct)
                            {
                                lowestHealthPct = pct;
                                injuredPet = bc;
                            }
                        }
                    }
                    nearbyMobiles.Free();
                }
            }
            
            if (injuredPet == null)
                return false;
            
            int distanceToPet = (int)m_Sidekick.GetDistanceToSqrt(injuredPet);
            
            // Cure pet poison first
            if (injuredPet.Poisoned && m_Sidekick.Mana >= 6 && distanceToPet <= 10)
            {
                var cureSpell = new CureSpell(m_Sidekick, null);
                cureSpell.Cast();
                
                Utility.PushColor(ConsoleColor.Green);
                Console.WriteLine($"[HealerCombatAI] {m_Sidekick.Name} - Curing pet {injuredPet.Name}!");
                Utility.PopColor();
                
                Timer.DelayCall(TimeSpan.FromMilliseconds(500), () => 
                {
                    if (m_Sidekick.Target != null)
                        m_Sidekick.Target.Invoke(m_Sidekick, injuredPet);
                });
                return true;
            }
            
            Utility.PushColor(ConsoleColor.Yellow);
            Console.WriteLine($"[HealerCombatAI] {m_Sidekick.Name} - Pet {injuredPet.Name} needs healing ({lowestHealthPct:P0}), distance: {distanceToPet}");
            Utility.PopColor();

            // Track if we're stuck trying to reach the pet
            if (m_LastPetTarget == injuredPet && m_LastDistanceToPet >= 0)
            {
                // Same pet, check if distance has decreased
                if (distanceToPet >= m_LastDistanceToPet)
                {
                    m_StuckCounter++;
                    Utility.PushColor(ConsoleColor.DarkYellow);
                    Console.WriteLine($"[HealerCombatAI] {m_Sidekick.Name} - Stuck counter: {m_StuckCounter} (distance not decreasing: {m_LastDistanceToPet} -> {distanceToPet})");
                    Utility.PopColor();
                }
                else
                {
                    m_StuckCounter = 0; // Making progress, reset counter
                }
            }
            else
            {
                // New target, reset tracking
                m_StuckCounter = 0;
            }
            m_LastPetTarget = injuredPet;
            m_LastDistanceToPet = distanceToPet;

            // PRIMARY: Bandaging - within BANDAGE_RANGE tiles (adjacent for large creatures like dragons)
            if (distanceToPet <= BANDAGE_RANGE)
            {
                // Adjacent - bandage!
                m_StuckCounter = 0; // Reset since we reached the target
                if (TryBandageTarget(injuredPet))
                {
                    Utility.PushColor(ConsoleColor.Cyan);
                    Console.WriteLine($"[HealerCombatAI] {m_Sidekick.Name} - BANDAGING pet {injuredPet.Name}");
                    Utility.PopColor();
                    // Stay here and don't move - we're bandaging
                    return true;
                }
            }
            else
            {
                // Not adjacent - check if stuck and should use spell instead
                bool useSpellInstead = m_StuckCounter >= STUCK_THRESHOLD && distanceToPet <= 10 && m_Sidekick.Mana >= 11;

                if (useSpellInstead)
                {
                    // STUCK! Use Greater Heal spell instead of trying to move
                    Utility.PushColor(ConsoleColor.Magenta);
                    Console.WriteLine($"[HealerCombatAI] {m_Sidekick.Name} - STUCK! Using Greater Heal on pet {injuredPet.Name} instead of bandaging (stuck {m_StuckCounter} ticks)");
                    Utility.PopColor();

                    var healSpell = new GreaterHealSpell(m_Sidekick, null);
                    healSpell.Cast();

                    Timer.DelayCall(TimeSpan.FromMilliseconds(500), () =>
                    {
                        if (m_Sidekick.Target != null)
                            m_Sidekick.Target.Invoke(m_Sidekick, injuredPet);
                    });

                    // Reset stuck counter after using spell
                    m_StuckCounter = 0;
                    return true;
                }
                else
                {
                    // Not stuck yet - try to move to pet using alternate tile selection
                    Utility.PushColor(ConsoleColor.Yellow);
                    Console.WriteLine($"[HealerCombatAI] {m_Sidekick.Name} - Moving to pet {injuredPet.Name} to bandage (distance: {distanceToPet})");
                    Utility.PopColor();

                    // Use MoveToFreeTile to avoid blocking other sidekicks
                    MoveToFreeTile(injuredPet, BANDAGE_RANGE);
                }
            }

            // SUPPLEMENT: Cast Greater Heal if pet < 50% health (bandages take time to apply)
            // This provides immediate healing while bandage is being applied or while moving
            if (lowestHealthPct < 0.50 && m_Sidekick.Mana >= 11 && distanceToPet <= 10)
            {
                var healSpell = new GreaterHealSpell(m_Sidekick, null);
                healSpell.Cast();

                Utility.PushColor(ConsoleColor.Magenta);
                Console.WriteLine($"[HealerCombatAI] {m_Sidekick.Name} - Casting Greater Heal on pet {injuredPet.Name} ({lowestHealthPct:P0}) while bandaging");
                Utility.PopColor();

                Timer.DelayCall(TimeSpan.FromMilliseconds(500), () =>
                {
                    if (m_Sidekick.Target != null)
                        m_Sidekick.Target.Invoke(m_Sidekick, injuredPet);
                });
            }

            return true; // Block further actions - we're moving to heal
        }
        
        // Pet heal threshold - bandage as soon as any damage taken
        private const double PET_HEAL_THRESHOLD = 1.00;
        
        /// <summary>
        /// Handle critical health - bandage loop + potion + escape + spell
        /// </summary>
        private bool HandleCritical(Mobile enemy)
        {
            double healthPercent = (double)m_Sidekick.Hits / (double)m_Sidekick.HitsMax;

            Utility.PushColor(ConsoleColor.Red);
            Console.WriteLine($"[HealerCombatAI] {m_Sidekick.Name} - CRITICAL HEALTH ({healthPercent:P0})!");
            Utility.PopColor();

            // PRIORITY 1: ALWAYS keep bandage loop going (if not poisoned)
            if (!m_Sidekick.Poisoned && TryUseBandage())
            {
                Utility.PushColor(ConsoleColor.Cyan);
                Console.WriteLine($"[HealerCombatAI] {m_Sidekick.Name} - Emergency bandage loop!");
                Utility.PopColor();
            }

            // PRIORITY 2: Heal potion (instant) - always try when critical
            if (TryUseHealPotion())
            {
                Utility.PushColor(ConsoleColor.Magenta);
                Console.WriteLine($"[HealerCombatAI] {m_Sidekick.Name} - Used heal potion in emergency!");
                Utility.PopColor();
                // Don't return - continue to escape
            }

            // PRIORITY 3: ESCAPE! Run/Teleport away from enemy
            if (enemy != null && enemy.Alive && !enemy.Deleted)
            {
                int distanceToEnemy = (int)m_Sidekick.GetDistanceToSqrt(enemy);

                // If enemy is close, try to escape
                if (distanceToEnemy <= 4)
                {
                    // Try Teleport spell if have mana
                    if (m_Sidekick.Mana >= 9 && m_Sidekick.Skills.Magery.Value >= 30)
                    {
                        Utility.PushColor(ConsoleColor.Yellow);
                        Console.WriteLine($"[HealerCombatAI] {m_Sidekick.Name} - EMERGENCY TELEPORT to escape!");
                        Utility.PopColor();

                        // Find a safe tile away from enemy
                        Direction awayDir = enemy.GetDirectionTo(m_Sidekick);
                        int offsetX = 0, offsetY = 0;
                        switch (awayDir & Direction.Mask)
                        {
                            case Direction.North: offsetY = -8; break;
                            case Direction.South: offsetY = 8; break;
                            case Direction.East: offsetX = 8; break;
                            case Direction.West: offsetX = -8; break;
                            case Direction.Up: offsetX = 6; offsetY = -6; break;
                            case Direction.Right: offsetX = 6; offsetY = 6; break;
                            case Direction.Down: offsetX = -6; offsetY = 6; break;
                            case Direction.Left: offsetX = -6; offsetY = -6; break;
                        }

                        Point3D targetLoc = new Point3D(m_Sidekick.X + offsetX, m_Sidekick.Y + offsetY, m_Sidekick.Z);
                        int z = m_Sidekick.Map.GetAverageZ(targetLoc.X, targetLoc.Y);
                        targetLoc = new Point3D(targetLoc.X, targetLoc.Y, z);

                        if (m_Sidekick.Map.CanFit(targetLoc.X, targetLoc.Y, targetLoc.Z, 16, false, false))
                        {
                            var teleSpell = new Server.Spells.Third.TeleportSpell(m_Sidekick, null);
                            teleSpell.Cast();

                            // Target the escape location
                            Timer.DelayCall(TimeSpan.FromMilliseconds(500), () =>
                            {
                                if (m_Sidekick.Target != null)
                                    m_Sidekick.Target.Invoke(m_Sidekick, targetLoc);
                            });
                            // Don't return - also cast Greater Heal while escaping
                        }
                    }
                    else
                    {
                        // No teleport available - RUN!
                        if (m_Sidekick.AIObject is SidekickAI sidekickAI)
                        {
                            Utility.PushColor(ConsoleColor.Yellow);
                            Console.WriteLine($"[HealerCombatAI] {m_Sidekick.Name} - RUNNING from enemy!");
                            Utility.PopColor();
                            sidekickAI.RunFrom(enemy);
                        }
                    }
                }
            }

            // PRIORITY 4: Greater Heal spell on self while escaping
            double healCooldownRemaining = HEAL_SPELL_COOLDOWN - (DateTime.UtcNow - m_LastHealSpellTime).TotalSeconds;
            if (m_Sidekick.Mana >= 11 && healCooldownRemaining <= 0)
            {
                var healSpell = new GreaterHealSpell(m_Sidekick, null);
                healSpell.Cast();
                m_LastHealSpellTime = DateTime.UtcNow;

                Utility.PushColor(ConsoleColor.Green);
                Console.WriteLine($"[HealerCombatAI] {m_Sidekick.Name} - Casting Greater Heal while escaping!");
                Utility.PopColor();
            }

            return true;
        }
        
        /// <summary>
        /// Handle poison curing (self or owner)
        /// </summary>
        private bool HandlePoison()
        {
            // Check owner first
            Mobile owner = m_Sidekick.ControlMaster ?? m_Sidekick.Owner;
            if (owner != null && owner.Poisoned && !owner.Deleted && owner.Alive)
            {
                if (m_Sidekick.Mana >= 6)
                {
                    var cureSpell = new CureSpell(m_Sidekick, null);
                    cureSpell.Cast();
                    
                    // Target owner
                    Timer.DelayCall(TimeSpan.FromMilliseconds(500), () => 
                    {
                        if (m_Sidekick.Target != null)
                            m_Sidekick.Target.Invoke(m_Sidekick, owner);
                    });
                    
                    Utility.PushColor(ConsoleColor.Green);
                    Console.WriteLine($"[HealerCombatAI] {m_Sidekick.Name} - Curing owner poison!");
                    Utility.PopColor();
                    return true;
                }
            }
            
            // Then cure self
            if (m_Sidekick.Poisoned)
            {
                // Try cure potion first (faster)
                if (TryUseCurePotion())
                    return true;
                
                // Try cure spell
                if (m_Sidekick.Mana >= 6)
                {
                    var cureSpell = new CureSpell(m_Sidekick, null);
                    cureSpell.Cast();
                    return true;
                }
            }
            
            return false;
        }
        
        /// <summary>
        /// Handle self-healing - bandages always loop, potion at 75%, escape + spell when critical
        /// </summary>
        private void HandleSelfHealing()
        {
            double healthPercent = (double)m_Sidekick.Hits / (double)m_Sidekick.HitsMax;

            // PRIORITY 1: ALWAYS keep bandage loop going if below 100% and not poisoned
            if (healthPercent < 1.0 && !m_Sidekick.Poisoned)
            {
                if (TryUseBandage())
                {
                    Utility.PushColor(ConsoleColor.Cyan);
                    Console.WriteLine($"[HealerCombatAI] {m_Sidekick.Name} - Bandage loop ({healthPercent:P0})");
                    Utility.PopColor();
                }
            }

            // PRIORITY 2: Heal potion at 75% or below (instant)
            if (healthPercent <= 0.75 && TryUseHealPotion())
            {
                Utility.PushColor(ConsoleColor.Magenta);
                Console.WriteLine($"[HealerCombatAI] {m_Sidekick.Name} - Used heal potion ({healthPercent:P0})");
                Utility.PopColor();
            }

            // PRIORITY 3: Greater Heal spell when below 50%
            if (healthPercent < LOW_HEALTH && m_Sidekick.Mana >= 11)
            {
                double cooldownRemaining = HEAL_SPELL_COOLDOWN - (DateTime.UtcNow - m_LastHealSpellTime).TotalSeconds;
                if (cooldownRemaining <= 0)
                {
                    var healSpell = new GreaterHealSpell(m_Sidekick, null);
                    healSpell.Cast();
                    m_LastHealSpellTime = DateTime.UtcNow;

                    Utility.PushColor(ConsoleColor.Green);
                    Console.WriteLine($"[HealerCombatAI] {m_Sidekick.Name} - Self Greater Heal ({healthPercent:P0})");
                    Utility.PopColor();
                }
            }
        }
        
        /// <summary>
        /// Stay near owner for bandage range
        /// </summary>
        private void StayNearOwner()
        {
            Mobile owner = m_Sidekick.ControlMaster ?? m_Sidekick.Owner;
            if (owner == null || owner.Deleted || !owner.Alive)
                return;
            
            int distanceToOwner = (int)m_Sidekick.GetDistanceToSqrt(owner);
            
            // Stay within 5 tiles of owner (close enough to quickly reach for bandaging)
            if (distanceToOwner > 5)
            {
                if (m_Sidekick.AIObject is SidekickAI sidekickAI)
                {
                    sidekickAI.MoveTo(owner, false, 3);
                }
            }
        }
        
        /// <summary>
        /// Try to bandage another target (owner or pet)
        /// </summary>
        private bool TryBandageTarget(Mobile target)
        {
            if (target == null || target.Deleted || !target.Alive)
                return false;

            if (target.Hits >= target.HitsMax)
                return false;

            double cooldownRemaining = BANDAGE_COOLDOWN - (DateTime.UtcNow - m_LastBandageTime).TotalSeconds;
            if (cooldownRemaining > 0)
            {
                Utility.PushColor(ConsoleColor.Gray);
                Console.WriteLine($"[HealerCombatAI] {m_Sidekick.Name} - Bandage on cooldown ({cooldownRemaining:F1}s remaining)");
                Utility.PopColor();
                return false;
            }
            
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
                BandageContext.BeginHeal(m_Sidekick, target);
                m_LastBandageTime = DateTime.UtcNow;
                return true;
            }
            catch
            {
                return false;
            }
        }
        
        #region Helper Methods

        /// <summary>
        /// Find a free tile adjacent to the target that isn't occupied by another sidekick
        /// Returns the best direction to approach from
        /// </summary>
        private Direction FindFreeTileDirection(Mobile target)
        {
            // All 8 directions to check
            Direction[] directions = {
                Direction.North, Direction.East, Direction.South, Direction.West,
                Direction.Up, Direction.Right, Direction.Down, Direction.Left // Diagonals
            };

            Mobile owner = m_Sidekick.ControlMaster ?? m_Sidekick.Owner;
            var occupiedTiles = new System.Collections.Generic.List<Point3D>();

            // Find tiles occupied by other sidekicks
            if (owner is PlayerMobile pm)
            {
                foreach (Mobile m in pm.AllFollowers)
                {
                    if (m == m_Sidekick || m.Deleted || !m.Alive)
                        continue;

                    if (m is AutonomousSidekick)
                    {
                        occupiedTiles.Add(m.Location);
                    }
                }
            }

            // Check each direction for a free tile
            foreach (Direction dir in directions)
            {
                int offsetX = 0, offsetY = 0;
                switch (dir)
                {
                    case Direction.North: offsetY = -1; break;
                    case Direction.South: offsetY = 1; break;
                    case Direction.East: offsetX = 1; break;
                    case Direction.West: offsetX = -1; break;
                    case Direction.Up: offsetX = 1; offsetY = -1; break;      // NE
                    case Direction.Right: offsetX = 1; offsetY = 1; break;    // SE
                    case Direction.Down: offsetX = -1; offsetY = 1; break;    // SW
                    case Direction.Left: offsetX = -1; offsetY = -1; break;   // NW
                }

                Point3D checkTile = new Point3D(target.X + offsetX, target.Y + offsetY, target.Z);

                // Check if this tile is occupied by another sidekick
                bool tileOccupied = false;
                foreach (var occ in occupiedTiles)
                {
                    if (occ.X == checkTile.X && occ.Y == checkTile.Y)
                    {
                        tileOccupied = true;
                        break;
                    }
                }

                if (!tileOccupied)
                {
                    // Also check if the sidekick can walk to this tile (not blocked by terrain)
                    if (m_Sidekick.Map.CanFit(checkTile.X, checkTile.Y, checkTile.Z, 16, false, false))
                    {
                        return dir;
                    }
                }
            }

            // No free tile found, return the opposite of our current direction
            return Direction.North;
        }

        /// <summary>
        /// Move to a specific side of the target to avoid blocking other sidekicks
        /// </summary>
        private void MoveToFreeTile(Mobile target, int desiredRange)
        {
            Direction freeDir = FindFreeTileDirection(target);

            // Calculate offset based on direction
            int offsetX = 0, offsetY = 0;
            switch (freeDir)
            {
                case Direction.North: offsetY = -desiredRange; break;
                case Direction.South: offsetY = desiredRange; break;
                case Direction.East: offsetX = desiredRange; break;
                case Direction.West: offsetX = -desiredRange; break;
                case Direction.Up: offsetX = desiredRange; offsetY = -desiredRange; break;
                case Direction.Right: offsetX = desiredRange; offsetY = desiredRange; break;
                case Direction.Down: offsetX = -desiredRange; offsetY = desiredRange; break;
                case Direction.Left: offsetX = -desiredRange; offsetY = -desiredRange; break;
            }

            Utility.PushColor(ConsoleColor.DarkYellow);
            Console.WriteLine($"[HealerCombatAI] {m_Sidekick.Name} - Approaching {target.Name} from {freeDir}");
            Utility.PopColor();

            if (m_Sidekick.AIObject is SidekickAI sidekickAI)
            {
                // Set direction first to encourage pathing from that side
                Point3D targetTile = new Point3D(target.X + offsetX, target.Y + offsetY, target.Z);
                m_Sidekick.Direction = m_Sidekick.GetDirectionTo(targetTile);
                sidekickAI.MoveTo(target, true, desiredRange);
            }
        }

        #endregion

        #region Healing Methods

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

