using System;
using System.Collections.Generic;
using System.Linq;
using Server;
using Server.Items;
using Server.Mobiles;
using Server.Spells;
using Server.Spells.Fourth;
using Server.Spells.Second;
using Server.Targeting;

namespace Server.Services.AISidekicks
{
    /// <summary>
    /// Combat AI for Tamer archetype - pet commander with veterinary healing
    /// Behavior: Command pets to fight, heal pets with bandages/spells, stay at safe distance
    /// </summary>
    public class TamerCombatAI
    {
        private AutonomousSidekick m_Sidekick;
        
        // Tamer state
        private enum TamerState
        {
            Idle,       // No threats, guard stance
            Combat,     // Pets fighting, stay back
            Defense,    // Self being attacked
            Recovery    // After combat, healing pets
        }
        
        private TamerState m_CurrentState = TamerState.Idle;
        
        // Health thresholds
        public const double SELF_CRITICAL_HEALTH = 0.25;   // 25% - emergency heal self
        public const double SELF_LOW_HEALTH = 0.40;        // 40% - use potion
        public const double SELF_BANDAGE_THRESHOLD = 0.60; // 60% - bandage self
        
        public const double PET_CRITICAL_HEALTH = 0.25;    // 25% - emergency
        public const double PET_LOW_HEALTH = 0.50;         // 50% - also cast Greater Heal while bandaging
        public const double PET_BANDAGE_THRESHOLD = 1.00;  // 100% - bandage as soon as any damage taken
        
        // Distance constants
        public const int BANDAGE_RANGE = 1;                // Stay 1 tile from pet for bandaging
        public const int SAFE_DISTANCE = 10;               // Safe distance from enemy when retreating
        public const int SPELL_RANGE = 10;                 // Spell cast range
        public const int PET_RECALL_RANGE = 20;            // Max range before recalling pets
        
        // Cooldowns
        private DateTime m_LastBandageSelfTime = DateTime.MinValue;
        private DateTime m_LastBandagePetTime = DateTime.MinValue;
        private DateTime m_LastHealSpellTime = DateTime.MinValue;
        private DateTime m_LastCureSpellTime = DateTime.MinValue;
        private DateTime m_LastCommandTime = DateTime.MinValue;
        private DateTime m_LastHealPotionTime = DateTime.MinValue;
        private DateTime m_LastCurePotionTime = DateTime.MinValue;
        
        private const double BANDAGE_COOLDOWN = 10.0;
        private const double PET_BANDAGE_COOLDOWN = 10.0;
        private const double SPELL_COOLDOWN = 2.0;
        private const double COMMAND_COOLDOWN = 1.0;

        // Pending heal target - used by SidekickAI.ProcessSpellTarget to target pets instead of self
        private Mobile m_PendingHealTarget = null;
        public Mobile PendingHealTarget
        {
            get { return m_PendingHealTarget; }
            set { m_PendingHealTarget = value; }
        }
        private const double HEAL_POTION_COOLDOWN = 10.0;
        private const double CURE_POTION_COOLDOWN = 1.0;
        
        // Combat tracking
        private Mobile m_LastKillTarget = null;
        private Mobile m_SelfAttacker = null;
        private DateTime m_LastSelfAttackTime = DateTime.MinValue;
        
        // Stuck detection (general movement)
        private Point3D m_LastPosition = Point3D.Zero;
        private int m_StuckCounter = 0;
        private const int STUCK_THRESHOLD = 5;
        private DateTime m_LastTeleportTime = DateTime.MinValue;
        private const double TELEPORT_COOLDOWN = 10.0;

        // Hysteresis: Once we reach the pet, STAY - don't keep moving
        private bool m_ReachedPet = false;


        #region Helper Methods

        /// <summary>
        /// Get the direction opposite to the given direction
        /// </summary>
        private Direction GetOppositeDirection(Direction dir)
        {
            switch (dir & Direction.Mask)
            {
                case Direction.North: return Direction.South;
                case Direction.South: return Direction.North;
                case Direction.East: return Direction.West;
                case Direction.West: return Direction.East;
                case Direction.Up: return Direction.Down;      // NE -> SW
                case Direction.Down: return Direction.Up;      // SW -> NE
                case Direction.Right: return Direction.Left;   // SE -> NW
                case Direction.Left: return Direction.Right;   // NW -> SE
                default: return Direction.South;
            }
        }

        /// <summary>
        /// Get offset for a direction
        /// </summary>
        private void GetDirectionOffset(Direction dir, out int offsetX, out int offsetY)
        {
            offsetX = 0;
            offsetY = 0;
            switch (dir & Direction.Mask)
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
        }

        /// <summary>
        /// Get the exact tile where the tamer should stand - opposite side of pet from enemy
        /// This gives us a FIXED tile coordinate that doesn't fluctuate like distance does
        /// </summary>
        private Point3D GetSafeTileNearPet(Mobile pet, Mobile enemy)
        {
            // Get direction from pet to enemy
            Direction enemyDir = pet.GetDirectionTo(enemy);

            // We want to be on the OPPOSITE side
            Direction safeDir = GetOppositeDirection(enemyDir);

            // Get the offset for that direction
            GetDirectionOffset(safeDir, out int offsetX, out int offsetY);

            // Calculate the exact tile
            Point3D safeTile = new Point3D(pet.X + offsetX, pet.Y + offsetY, pet.Z);

            // Check if that tile is walkable
            if (m_Sidekick.Map.CanFit(safeTile.X, safeTile.Y, safeTile.Z, 16, false, false))
            {
                return safeTile;
            }

            // If not walkable, try adjacent tiles (rotate around)
            Direction[] fallbacks = {
                (Direction)(((int)safeDir + 1) & 0x7),  // Rotate clockwise
                (Direction)(((int)safeDir + 7) & 0x7),  // Rotate counter-clockwise
                (Direction)(((int)safeDir + 2) & 0x7),
                (Direction)(((int)safeDir + 6) & 0x7)
            };

            foreach (var fallbackDir in fallbacks)
            {
                GetDirectionOffset(fallbackDir, out offsetX, out offsetY);
                safeTile = new Point3D(pet.X + offsetX, pet.Y + offsetY, pet.Z);

                if (m_Sidekick.Map.CanFit(safeTile.X, safeTile.Y, safeTile.Z, 16, false, false))
                {
                    return safeTile;
                }
            }

            // Last resort - just return a tile south of pet
            return new Point3D(pet.X, pet.Y + 1, pet.Z);
        }

        /// <summary>
        /// Check if sidekick is on the target tile (exact coordinate match)
        /// </summary>
        private bool IsOnTile(Point3D tile)
        {
            return m_Sidekick.X == tile.X && m_Sidekick.Y == tile.Y;
        }

        /// <summary>
        /// Move to an exact tile
        /// </summary>
        private void MoveToTile(Point3D tile)
        {
            if (m_Sidekick.AIObject is SidekickAI sidekickAI)
            {
                sidekickAI.MoveTo(tile, true, 0);
            }
        }

        #endregion

        public TamerCombatAI(AutonomousSidekick sidekick)
        {
            m_Sidekick = sidekick;
        }
        
        /// <summary>
        /// Main combat loop for tamers
        /// </summary>
        public bool DoCombat(Mobile enemy)
        {
            if (m_Sidekick == null || m_Sidekick.Deleted || !m_Sidekick.Alive)
                return false;

            // CRITICAL: If we're actively bandaging, STOP EVERYTHING and stay still
            // The bandage system will handle success/failure - we just need to not move
            var activeBandage = BandageContext.GetContext(m_Sidekick);
            if (activeBandage != null)
            {
                Utility.PushColor(ConsoleColor.Cyan);
                Console.WriteLine($"[TamerCombatAI] {m_Sidekick.Name} - BANDAGING IN PROGRESS - staying in place!");
                Utility.PopColor();
                return true; // Block all other actions while bandaging
            }

            // Get our pets
            var pets = GetMyPets();
            
            Utility.PushColor(ConsoleColor.Cyan);
            Console.WriteLine($"[TamerCombatAI] {m_Sidekick.Name} - State: {m_CurrentState}, Pets: {pets.Count}, Enemy: {enemy?.Name ?? "none"}");
            Utility.PopColor();
            
            double selfHealthPct = (double)m_Sidekick.Hits / (double)m_Sidekick.HitsMax;
            
            // Check for stuck condition
            CheckAndHandleStuck(enemy);
            
            // Check if we're being directly attacked
            bool selfUnderAttack = IsSelfUnderAttack();
            
            // Get pet status to help make decisions
            var mostInjuredPet = GetMostInjuredPet(pets);
            bool petNeedsHealing = mostInjuredPet != null && (double)mostInjuredPet.Hits / mostInjuredPet.HitsMax < PET_BANDAGE_THRESHOLD;
            
            // PRIORITY 1: Self-preservation - ONLY flee if directly attacked by an aggressor
            if (selfUnderAttack)
            {
                m_CurrentState = TamerState.Defense;
                if (HandleSelfDefense(enemy))
                    return true;
            }
            
            // PRIORITY 2: Cure self poison
            if (m_Sidekick.Poisoned)
            {
                if (TryCureSelf())
                    return true;
            }
            
            // PRIORITY 2.5: Proactive self-healing (even when not attacked)
            if (selfHealthPct < SELF_BANDAGE_THRESHOLD)
            {
                if (TryBandageSelf())
                {
                    Utility.PushColor(ConsoleColor.Green);
                    Console.WriteLine($"[TamerCombatAI] {m_Sidekick.Name} - Proactive self-bandage ({selfHealthPct:P0} health)");
                    Utility.PopColor();
                }
            }
            
            // PRIORITY 3: Pet emergency healing (critical health or poisoned)
            var criticalPet = GetCriticalPet(pets);
            if (criticalPet != null)
            {
                if (HandlePetEmergency(criticalPet))
                    return true;
            }
            
            var poisonedPet = GetPoisonedPet(pets);
            if (poisonedPet != null)
            {
                if (TryCurePet(poisonedPet))
                    return true;
            }
            
            // PRIORITY 4: Command pets to fight and support them
            if (enemy != null && enemy.Alive)
            {
                m_CurrentState = TamerState.Combat;
                
                // Send pets to attack if not already attacking this target
                if (!ArePetsAttacking(pets, enemy))
                {
                    CommandAllKill(pets, enemy);
                }
                
                // Pet is tanking - Tamer should get adjacent to pet and STAY THERE
                // HYSTERESIS: Once we reach the pet, we NEVER move again (until combat ends)

                var pet = pets[0];  // Use first pet for positioning
                var injuredPet = GetMostInjuredPet(pets);

                // Check if we're adjacent to the pet (within 2 tiles - bandage range)
                bool adjacentToPet = m_Sidekick.InRange(pet, 2);

                // Check bandage cooldown
                double bandageCooldownRemaining = PET_BANDAGE_COOLDOWN - (DateTime.UtcNow - m_LastBandagePetTime).TotalSeconds;
                bool bandageReady = bandageCooldownRemaining <= 0;

                // HYSTERESIS with smart reset:
                // - Set m_ReachedPet when we become adjacent
                // - Reset m_ReachedPet if NOT adjacent AND bandage is ready (pet moved, we need to follow)
                if (adjacentToPet)
                {
                    m_ReachedPet = true;
                }
                else if (m_ReachedPet && bandageReady)
                {
                    // Pet moved away and we need to bandage - reset flag so we can follow
                    Utility.PushColor(ConsoleColor.Magenta);
                    Console.WriteLine($"[TamerCombatAI] {m_Sidekick.Name} - Pet moved away, resetting ReachedPet to follow");
                    Utility.PopColor();
                    m_ReachedPet = false;
                }

                Utility.PushColor(ConsoleColor.Yellow);
                Console.WriteLine($"[TamerCombatAI] {m_Sidekick.Name} - Adjacent: {adjacentToPet}, ReachedPet: {m_ReachedPet}, BandageReady: {bandageReady}, MyPos: ({m_Sidekick.X},{m_Sidekick.Y}), PetPos: ({pet.X},{pet.Y})");
                Utility.PopColor();

                // STEP 1: Get adjacent to pet if NOT adjacent
                if (!adjacentToPet)
                {
                    // Calculate safe tile and move there
                    Point3D safeTile = GetSafeTileNearPet(pet, enemy);

                    Utility.PushColor(ConsoleColor.Yellow);
                    Console.WriteLine($"[TamerCombatAI] {m_Sidekick.Name} - Moving to pet (target tile: {safeTile.X},{safeTile.Y})");
                    Utility.PopColor();

                    MoveToTile(safeTile);
                    // Don't return - still try to bandage/heal while moving
                }
                // else: We're adjacent - stay and bandage!

                // STEP 2: Try to bandage injured pet
                if (injuredPet != null)
                {
                    double petHealthPct = (double)injuredPet.Hits / injuredPet.HitsMax;

                    if (petHealthPct < PET_BANDAGE_THRESHOLD)
                    {
                        // Try to bandage - let the bandage system handle range checks
                        if (TryBandagePet(injuredPet))
                        {
                            Utility.PushColor(ConsoleColor.Cyan);
                            Console.WriteLine($"[TamerCombatAI] {m_Sidekick.Name} - BANDAGING {injuredPet.Name}!");
                            Utility.PopColor();
                            return true;
                        }

                        // SUPPLEMENT: Cast Greater Heal if pet < 50% health
                        if (petHealthPct < PET_LOW_HEALTH && m_Sidekick.Mana >= 11 && m_Sidekick.InRange(injuredPet, SPELL_RANGE))
                        {
                            Utility.PushColor(ConsoleColor.Magenta);
                            Console.WriteLine($"[TamerCombatAI] {m_Sidekick.Name} - Casting Greater Heal on {injuredPet.Name} ({petHealthPct:P0})");
                            Utility.PopColor();
                            TryCastHealOnPet(injuredPet);
                        }
                    }
                }

                return true;
            }
            
            // PRIORITY 5: No enemy - recovery or idle
            // Reset hysteresis since combat ended
            m_ReachedPet = false;

            if (pets.Count > 0)
            {
                // Check if any pet needs healing (any damage at all)
                var injuredPet = GetMostInjuredPet(pets);
                if (injuredPet != null)
                {
                    m_CurrentState = TamerState.Recovery;
                    double petHealthPct = (double)injuredPet.Hits / injuredPet.HitsMax;

                    Utility.PushColor(ConsoleColor.Yellow);
                    Console.WriteLine($"[TamerCombatAI] {m_Sidekick.Name} - RECOVERY MODE: Healing {injuredPet.Name} ({petHealthPct:P0})");
                    Utility.PopColor();

                    // Move to pet if not adjacent
                    if (!m_Sidekick.InRange(injuredPet, 2))
                    {
                        if (m_Sidekick.AIObject is SidekickAI sidekickAI)
                        {
                            sidekickAI.MoveTo(injuredPet, true, 1);
                        }
                    }

                    // Try bandaging first
                    if (TryBandagePet(injuredPet))
                    {
                        Utility.PushColor(ConsoleColor.Green);
                        Console.WriteLine($"[TamerCombatAI] {m_Sidekick.Name} - RECOVERY: Bandaging {injuredPet.Name}");
                        Utility.PopColor();
                    }

                    // Also cast Greater Heal if we have mana and pet is below 80%
                    if (petHealthPct < 0.80 && m_Sidekick.Mana >= 11 && m_Sidekick.InRange(injuredPet, SPELL_RANGE))
                    {
                        if (TryCastHealOnPet(injuredPet))
                        {
                            Utility.PushColor(ConsoleColor.Magenta);
                            Console.WriteLine($"[TamerCombatAI] {m_Sidekick.Name} - RECOVERY: Casting Greater Heal on {injuredPet.Name}");
                            Utility.PopColor();
                        }
                    }

                    return true;
                }

                // All pets healthy - recall and guard
                m_CurrentState = TamerState.Idle;
                CommandAllGuard(pets);
            }

            return false;
        }
        
        #region Pet Management
        
        /// <summary>
        /// Get all pets controlled by this sidekick
        /// </summary>
        private List<BaseCreature> GetMyPets()
        {
            var pets = new List<BaseCreature>();
            
            // Scan nearby for creatures we control
            var eable = m_Sidekick.GetMobilesInRange(PET_RECALL_RANGE);
            
            foreach (Mobile m in eable)
            {
                if (m is BaseCreature bc && bc != m_Sidekick && bc.ControlMaster == m_Sidekick && bc.Alive && !bc.Deleted)
                {
                    pets.Add(bc);
                }
            }
            
            eable.Free();
            
            return pets;
        }
        
        /// <summary>
        /// Get the pet with lowest health percentage
        /// </summary>
        private BaseCreature GetMostInjuredPet(List<BaseCreature> pets)
        {
            BaseCreature mostInjured = null;
            double lowestHealthPct = 1.0;
            
            foreach (var pet in pets)
            {
                if (pet.Hits < pet.HitsMax)
                {
                    double pct = (double)pet.Hits / (double)pet.HitsMax;
                    if (pct < lowestHealthPct)
                    {
                        lowestHealthPct = pct;
                        mostInjured = pet;
                    }
                }
            }
            
            return mostInjured;
        }
        
        /// <summary>
        /// Get a pet that's critically injured (< 25%)
        /// </summary>
        private BaseCreature GetCriticalPet(List<BaseCreature> pets)
        {
            foreach (var pet in pets)
            {
                if (pet.Hits < pet.HitsMax * PET_CRITICAL_HEALTH)
                    return pet;
            }
            return null;
        }
        
        /// <summary>
        /// Get a poisoned pet
        /// </summary>
        private BaseCreature GetPoisonedPet(List<BaseCreature> pets)
        {
            foreach (var pet in pets)
            {
                if (pet.Poisoned)
                    return pet;
            }
            return null;
        }
        
        /// <summary>
        /// Check if pets are already attacking the target
        /// </summary>
        private bool ArePetsAttacking(List<BaseCreature> pets, Mobile target)
        {
            foreach (var pet in pets)
            {
                if (pet.Combatant == target || pet.ControlTarget == target && pet.ControlOrder == OrderType.Attack)
                    return true;
            }
            return false;
        }
        
        #endregion
        
        #region Pet Commands
        
        /// <summary>
        /// Command all pets to attack a target
        /// </summary>
        private void CommandAllKill(List<BaseCreature> pets, Mobile target)
        {
            if ((DateTime.UtcNow - m_LastCommandTime).TotalSeconds < COMMAND_COOLDOWN)
                return;
            
            if (target == null || !target.Alive)
                return;
            
            Utility.PushColor(ConsoleColor.Yellow);
            Console.WriteLine($"[TamerCombatAI] {m_Sidekick.Name} - ALL KILL: {target.Name}");
            Utility.PopColor();
            
            foreach (var pet in pets)
            {
                pet.ControlTarget = target;
                pet.ControlOrder = OrderType.Attack;
                pet.Combatant = target;
            }
            
            m_Sidekick.Say("All kill!");
            m_LastKillTarget = target;
            m_LastCommandTime = DateTime.UtcNow;
        }
        
        /// <summary>
        /// Command all pets to follow the tamer
        /// </summary>
        private void CommandAllFollow(List<BaseCreature> pets)
        {
            if ((DateTime.UtcNow - m_LastCommandTime).TotalSeconds < COMMAND_COOLDOWN)
                return;
            
            Utility.PushColor(ConsoleColor.Cyan);
            Console.WriteLine($"[TamerCombatAI] {m_Sidekick.Name} - ALL FOLLOW ME");
            Utility.PopColor();
            
            foreach (var pet in pets)
            {
                pet.ControlTarget = m_Sidekick;
                pet.ControlOrder = OrderType.Follow;
            }
            
            m_Sidekick.Say("All follow me!");
            m_LastCommandTime = DateTime.UtcNow;
        }
        
        /// <summary>
        /// Command all pets to guard the tamer
        /// </summary>
        private void CommandAllGuard(List<BaseCreature> pets)
        {
            if ((DateTime.UtcNow - m_LastCommandTime).TotalSeconds < COMMAND_COOLDOWN)
                return;
            
            // Don't spam guard command
            bool allGuarding = pets.All(p => p.ControlOrder == OrderType.Guard);
            if (allGuarding)
                return;
            
            Utility.PushColor(ConsoleColor.Green);
            Console.WriteLine($"[TamerCombatAI] {m_Sidekick.Name} - ALL GUARD ME");
            Utility.PopColor();
            
            foreach (var pet in pets)
            {
                pet.ControlTarget = m_Sidekick;
                pet.ControlOrder = OrderType.Guard;
            }
            
            m_Sidekick.Say("All guard me!");
            m_LastCommandTime = DateTime.UtcNow;
        }
        
        #endregion
        
        #region Pet Healing
        
        /// <summary>
        /// Handle pet emergency (critical health)
        /// </summary>
        private bool HandlePetEmergency(BaseCreature pet)
        {
            Utility.PushColor(ConsoleColor.Red);
            Console.WriteLine($"[TamerCombatAI] {m_Sidekick.Name} - PET EMERGENCY: {pet.Name} at {pet.Hits}/{pet.HitsMax}");
            Utility.PopColor();
            
            // Try Greater Heal spell first (faster than bandage for emergency)
            if (TryCastHealOnPet(pet))
                return true;
            
            // Fallback to bandage
            if (TryBandagePet(pet))
                return true;
            
            return false;
        }
        
        /// <summary>
        /// Try to cast Greater Heal on a pet
        /// </summary>
        private bool TryCastHealOnPet(BaseCreature pet)
        {
            if ((DateTime.UtcNow - m_LastHealSpellTime).TotalSeconds < SPELL_COOLDOWN)
                return false;
            
            // Check mana (Greater Heal = 11 mana)
            if (m_Sidekick.Mana < 11)
                return false;
            
            // Check if already casting
            if (m_Sidekick.Spell != null && ((Spell)m_Sidekick.Spell).IsCasting)
                return false;
            
            int distance = (int)m_Sidekick.GetDistanceToSqrt(pet);
            if (distance > SPELL_RANGE || !m_Sidekick.InLOS(pet))
            {
                // Move closer to pet
                if (m_Sidekick.AIObject is SidekickAI sidekickAI)
                {
                    sidekickAI.MoveTo(pet, true, SPELL_RANGE - 2);
                }
                return false;
            }
            
            Utility.PushColor(ConsoleColor.Magenta);
            Console.WriteLine($"[TamerCombatAI] {m_Sidekick.Name} - Casting Greater Heal on {pet.Name}");
            Utility.PopColor();

            try
            {
                // Set pending heal target BEFORE casting - SidekickAI.ProcessSpellTarget will use this
                m_PendingHealTarget = pet;

                var spell = new GreaterHealSpell(m_Sidekick, null);
                spell.Cast();

                m_LastHealSpellTime = DateTime.UtcNow;
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[TamerCombatAI] Greater Heal cast failed: {ex.Message}");
                m_PendingHealTarget = null;
                return false;
            }
        }
        
        /// <summary>
        /// Try to cure a poisoned pet
        /// </summary>
        private bool TryCurePet(BaseCreature pet)
        {
            if ((DateTime.UtcNow - m_LastCureSpellTime).TotalSeconds < SPELL_COOLDOWN)
                return false;
            
            // Check mana (Cure = 6 mana)
            if (m_Sidekick.Mana < 6)
                return false;
            
            if (m_Sidekick.Spell != null && ((Spell)m_Sidekick.Spell).IsCasting)
                return false;
            
            int distance = (int)m_Sidekick.GetDistanceToSqrt(pet);
            if (distance > SPELL_RANGE || !m_Sidekick.InLOS(pet))
            {
                if (m_Sidekick.AIObject is SidekickAI sidekickAI)
                {
                    sidekickAI.MoveTo(pet, true, SPELL_RANGE - 2);
                }
                return false;
            }
            
            Utility.PushColor(ConsoleColor.Green);
            Console.WriteLine($"[TamerCombatAI] {m_Sidekick.Name} - Casting Cure on {pet.Name}");
            Utility.PopColor();
            
            try
            {
                var spell = new CureSpell(m_Sidekick, null);
                spell.Cast();
                
                Timer.DelayCall(TimeSpan.FromMilliseconds(500), () =>
                {
                    if (m_Sidekick.Target != null && pet != null && !pet.Deleted && pet.Alive)
                    {
                        m_Sidekick.Target.Invoke(m_Sidekick, pet);
                    }
                });
                
                m_LastCureSpellTime = DateTime.UtcNow;
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[TamerCombatAI] Cure cast failed: {ex.Message}");
                return false;
            }
        }
        
        /// <summary>
        /// Try to bandage a pet (uses Veterinary skill)
        /// Simple approach: Just try to start the bandage. If it fails due to distance,
        /// the caller will move us closer and we'll try again next tick.
        /// </summary>
        private bool TryBandagePet(BaseCreature pet)
        {
            double cooldownRemaining = PET_BANDAGE_COOLDOWN - (DateTime.UtcNow - m_LastBandagePetTime).TotalSeconds;
            if (cooldownRemaining > 0)
            {
                Utility.PushColor(ConsoleColor.Gray);
                Console.WriteLine($"[TamerCombatAI] {m_Sidekick.Name} - Bandage on cooldown ({cooldownRemaining:F1}s remaining)");
                Utility.PopColor();
                return false;
            }

            // Don't bandage at full health
            if (pet.Hits >= pet.HitsMax)
                return false;
            
            // Check for bandages
            Container pack = m_Sidekick.Backpack;
            if (pack == null)
            {
                Console.WriteLine($"[TamerCombatAI] {m_Sidekick.Name} - No backpack!");
                return false;
            }
            
            Item bandage = pack.FindItemByType(typeof(Bandage));
            if (bandage == null || bandage.Amount < 1)
            {
                Utility.PushColor(ConsoleColor.Red);
                Console.WriteLine($"[TamerCombatAI] {m_Sidekick.Name} - NO BANDAGES in backpack!");
                Utility.PopColor();
                return false;
            }
            
            // Check if already bandaging
            var bandageContext = BandageContext.GetContext(m_Sidekick);
            if (bandageContext != null)
            {
                Utility.PushColor(ConsoleColor.Gray);
                Console.WriteLine($"[TamerCombatAI] {m_Sidekick.Name} - Already bandaging someone");
                Utility.PopColor();
                return false;
            }
            
            Utility.PushColor(ConsoleColor.Cyan);
            Console.WriteLine($"[TamerCombatAI] {m_Sidekick.Name} - BANDAGING {pet.Name} ({pet.Hits}/{pet.HitsMax})");
            Utility.PopColor();
            
            try
            {
                // BandageContext.BeginHeal handles both player and creature healing
                // For creatures, it uses Veterinary + Animal Lore skills
                BandageContext.BeginHeal(m_Sidekick, pet, false);
                m_LastBandagePetTime = DateTime.UtcNow;
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[TamerCombatAI] Bandage pet failed: {ex.Message}");
                return false;
            }
        }
        
        #endregion
        
        #region Self Defense
        
        /// <summary>
        /// Check if the tamer is being directly attacked
        /// </summary>
        private bool IsSelfUnderAttack()
        {
            // Check recent aggressors
            foreach (AggressorInfo info in m_Sidekick.Aggressors)
            {
                if (info.Attacker != null && info.Attacker.Alive && !info.Attacker.Deleted)
                {
                    if (m_Sidekick.InRange(info.Attacker, 3))
                    {
                        m_SelfAttacker = info.Attacker;
                        m_LastSelfAttackTime = DateTime.UtcNow;
                        return true;
                    }
                }
            }
            
            // Also check if someone is targeting us
            if (m_Sidekick.Combatant != null && m_Sidekick.Combatant != m_Sidekick)
            {
                var attacker = m_Sidekick.Combatant as Mobile;
                if (attacker != null && attacker.Combatant == m_Sidekick && m_Sidekick.InRange(attacker, 5))
                {
                    m_SelfAttacker = attacker;
                    m_LastSelfAttackTime = DateTime.UtcNow;
                    return true;
                }
            }
            
            return false;
        }
        
        /// <summary>
        /// Handle self-defense when being attacked
        /// </summary>
        private bool HandleSelfDefense(Mobile enemy)
        {
            Utility.PushColor(ConsoleColor.Red);
            Console.WriteLine($"[TamerCombatAI] {m_Sidekick.Name} - SELF DEFENSE MODE! Attacker: {m_SelfAttacker?.Name ?? "unknown"}");
            Utility.PopColor();

            double healthPct = (double)m_Sidekick.Hits / (double)m_Sidekick.HitsMax;
            var pets = GetMyPets();

            // Command pets to attack whoever is attacking us
            if (m_SelfAttacker != null && m_SelfAttacker.Alive)
            {
                if (!ArePetsAttacking(pets, m_SelfAttacker))
                {
                    CommandAllKill(pets, m_SelfAttacker);
                }
            }

            // Check if our pet is actively engaged with the attacker
            // If so, DON'T RUN - the pet is protecting us. Stay and heal the pet instead.
            bool petIsProtecting = false;
            if (m_SelfAttacker != null && pets.Count > 0)
            {
                foreach (var pet in pets)
                {
                    if (pet.Combatant == m_SelfAttacker)
                    {
                        petIsProtecting = true;
                        break;
                    }
                }
            }

            if (petIsProtecting)
            {
                Utility.PushColor(ConsoleColor.Green);
                Console.WriteLine($"[TamerCombatAI] {m_Sidekick.Name} - Pet is protecting me, staying to heal!");
                Utility.PopColor();

                // Don't flee - return false to continue to pet healing logic
                // But still try to heal self first if needed
                if (healthPct < SELF_LOW_HEALTH && TryUseHealPotion())
                    return true;
                if (healthPct < SELF_BANDAGE_THRESHOLD && TryBandageSelf())
                    return true;

                return false;  // Continue to pet healing/positioning
            }

            // No pet protecting us - try to heal self
            if (healthPct < SELF_CRITICAL_HEALTH)
            {
                // Emergency: use potion
                if (TryUseHealPotion())
                    return true;
            }

            if (healthPct < SELF_LOW_HEALTH)
            {
                // Use potion
                if (TryUseHealPotion())
                    return true;
            }

            // Try to bandage self if not too damaged
            if (healthPct < SELF_BANDAGE_THRESHOLD)
            {
                if (TryBandageSelf())
                    return true;
            }

            // Retreat from attacker (only if pet is NOT protecting us)
            if (m_SelfAttacker != null)
            {
                int distance = (int)m_Sidekick.GetDistanceToSqrt(m_SelfAttacker);
                if (distance < SAFE_DISTANCE)
                {
                    if (m_Sidekick.AIObject is SidekickAI sidekickAI)
                    {
                        sidekickAI.RunFrom(m_SelfAttacker);
                        return true;
                    }
                }
            }
            
            return false;
        }
        
        /// <summary>
        /// Try to cure self poison
        /// </summary>
        private bool TryCureSelf()
        {
            // Try cure potion first (instant)
            if (TryUseCurePotion())
                return true;
            
            // Try cure spell
            if (m_Sidekick.Mana >= 6 && m_Sidekick.Spell == null)
            {
                try
                {
                    var spell = new CureSpell(m_Sidekick, null);
                    spell.Cast();
                    
                    Timer.DelayCall(TimeSpan.FromMilliseconds(500), () =>
                    {
                        if (m_Sidekick.Target != null)
                        {
                            m_Sidekick.Target.Invoke(m_Sidekick, m_Sidekick);
                        }
                    });
                    
                    return true;
                }
                catch
                {
                    return false;
                }
            }
            
            return false;
        }
        
        /// <summary>
        /// Try to bandage self
        /// </summary>
        public bool TryBandageSelf()
        {
            if ((DateTime.UtcNow - m_LastBandageSelfTime).TotalSeconds < BANDAGE_COOLDOWN)
                return false;
            
            if (m_Sidekick.Hits >= m_Sidekick.HitsMax)
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
                BandageContext.BeginHeal(m_Sidekick, m_Sidekick, false);
                m_LastBandageSelfTime = DateTime.UtcNow;
                return true;
            }
            catch
            {
                return false;
            }
        }
        
        /// <summary>
        /// Public wrapper for bandage self
        /// </summary>
        public bool TryUseBandagePublic()
        {
            return TryBandageSelf();
        }
        
        #endregion
        
        #region Potions
        
        private bool TryUseHealPotion()
        {
            if ((DateTime.UtcNow - m_LastHealPotionTime).TotalSeconds < HEAL_POTION_COOLDOWN)
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
            
            Utility.PushColor(ConsoleColor.Magenta);
            Console.WriteLine($"[TamerCombatAI] {m_Sidekick.Name} - Using Greater Heal Potion");
            Utility.PopColor();
            
            potion.Drink(m_Sidekick);
            m_LastHealPotionTime = DateTime.UtcNow;
            return true;
        }
        
        private bool TryUseCurePotion()
        {
            if ((DateTime.UtcNow - m_LastCurePotionTime).TotalSeconds < CURE_POTION_COOLDOWN)
                return false;
            
            if (!m_Sidekick.Poisoned)
                return false;
            
            Container pack = m_Sidekick.Backpack;
            if (pack == null)
                return false;
            
            GreaterCurePotion potion = pack.FindItemByType(typeof(GreaterCurePotion)) as GreaterCurePotion;
            if (potion == null)
                return false;
            
            Utility.PushColor(ConsoleColor.Green);
            Console.WriteLine($"[TamerCombatAI] {m_Sidekick.Name} - Using Greater Cure Potion");
            Utility.PopColor();
            
            potion.Drink(m_Sidekick);
            m_LastCurePotionTime = DateTime.UtcNow;
            return true;
        }
        
        #endregion
        
        #region Positioning
        
        /// <summary>
        /// Maintain position near pet - no retreat unless directly attacked
        /// </summary>
        private void MaintainSafeDistance(Mobile enemy)
        {
            // Tamer no longer maintains safe distance from enemy
            // Only retreats if directly attacked (handled in HandleSelfDefense)
            return;
        }
        
        /// <summary>
        /// Check if tamer is stuck and try to teleport out
        /// </summary>
        private void CheckAndHandleStuck(Mobile enemy)
        {
            // Check if we've moved
            if (m_Sidekick.Location == m_LastPosition)
            {
                m_StuckCounter++;
                
                if (m_StuckCounter >= STUCK_THRESHOLD)
                {
                    Utility.PushColor(ConsoleColor.Yellow);
                    Console.WriteLine($"[TamerCombatAI] {m_Sidekick.Name} - STUCK detected! Counter: {m_StuckCounter}");
                    Utility.PopColor();
                    
                    // Try to teleport if we have mana and cooldown is up
                    if (TryTeleportEscape(enemy))
                    {
                        m_StuckCounter = 0;
                        return;
                    }
                    
                    // Try bandaging while stuck
                    if (m_Sidekick.Hits < m_Sidekick.HitsMax)
                    {
                        TryBandageSelf();
                    }
                }
            }
            else
            {
                m_StuckCounter = 0;
            }
            
            m_LastPosition = m_Sidekick.Location;
        }
        
        /// <summary>
        /// Try to teleport away from danger
        /// </summary>
        private bool TryTeleportEscape(Mobile enemy)
        {
            // Check cooldown
            if ((DateTime.UtcNow - m_LastTeleportTime).TotalSeconds < TELEPORT_COOLDOWN)
                return false;
            
            // Check mana (Teleport = 9 mana)
            if (m_Sidekick.Mana < 9)
                return false;
            
            // Check if we have Magery skill
            if (m_Sidekick.Skills.Magery.Value < 30)
                return false;
            
            // Find a safe location to teleport to
            Mobile owner = m_Sidekick.ControlMaster ?? m_Sidekick.Owner;
            Point3D targetLocation = m_Sidekick.Location;
            
            if (owner != null && !owner.Deleted && owner.Alive)
            {
                // Teleport near owner
                targetLocation = owner.Location;
            }
            else if (enemy != null && enemy.Alive)
            {
                // Teleport away from enemy
                int dx = m_Sidekick.X - enemy.X;
                int dy = m_Sidekick.Y - enemy.Y;
                
                // Normalize and extend
                double dist = Math.Sqrt(dx * dx + dy * dy);
                if (dist > 0)
                {
                    dx = (int)(dx / dist * 10);
                    dy = (int)(dy / dist * 10);
                }
                
                targetLocation = new Point3D(m_Sidekick.X + dx, m_Sidekick.Y + dy, m_Sidekick.Z);
            }
            
            Utility.PushColor(ConsoleColor.Magenta);
            Console.WriteLine($"[TamerCombatAI] {m_Sidekick.Name} - Attempting teleport escape!");
            Utility.PopColor();
            
            try
            {
                var spell = new Server.Spells.Third.TeleportSpell(m_Sidekick, null);
                spell.Cast();
                m_LastTeleportTime = DateTime.UtcNow;
                return true;
            }
            catch
            {
                return false;
            }
        }
        
        #endregion
    }
}

