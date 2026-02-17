using System;
using System.Collections.Generic;
using Server.Items;
using Server.Mobiles;
using Server.Movement;
using Server.Spells;
using Server.Spells.First;
using Server.Spells.Second;
using Server.Spells.Third;
using Server.Spells.Fourth;
using Server.Spells.Fifth;   // Paralyze, Poison
using Server.Spells.Sixth;
using Server.Spells.Seventh; // Mana Vampire
using Server.Targeting;

namespace Server.Services.AISidekicks
{
    /// <summary>
    /// Advanced Mage Combat AI for Sidekicks
    /// Implements proper UO PvP mage tactics:
    /// - Curse debuff management (resist reduction)
    /// - Spell combos (Curse->Explosion->Poison->Energy Bolt)
    /// - Paralyze for setup/execute
    /// - Mana Vampire for mage vs mage
    /// - Positioning (3-11 tiles)
    /// - Teleport escape
    /// - Defensive priorities
    /// - Interrupt mechanics
    /// </summary>
    public class MageCombatAI
    {
        private AutonomousSidekick m_Sidekick;

        // Combat ranges based on predictive kiting strategy
        // PvE RANGES - Optimized via dual-enemy simulation (Ogre Lord + Lich): 99% victory rate
        // Best config from simulation: min_safe=16, max_cast=24, optimal_cast=24
        // NOTE: UO spell target range is 10-12 tiles for most spells, but we kite at longer ranges
        public const int PVE_MIN_SAFE_RANGE = 16;     // Minimum distance before retreating (simulation optimal)
        public const int PVE_MAX_CAST_RANGE = 24;     // Maximum spell casting range (simulation optimal)
        public const int PVE_OPTIMAL_CAST_RANGE = 24; // Target casting distance (simulation optimal)
        public const int PVE_SPELL_RELEASE_RANGE_MIN = 8;  // Minimum range to release spell (simulation optimal)
        public const int PVE_SPELL_RELEASE_RANGE_MAX = 11; // Maximum range to release spell (simulation optimal)

        // PvP RANGES - Tighter ranges for fighting players/sidekicks (they don't chase like monsters)
        public const int PVP_MIN_SAFE_RANGE = 6;      // Much closer - both mages will be at similar range
        public const int PVP_MAX_CAST_RANGE = 12;     // Standard spell range
        public const int PVP_OPTIMAL_CAST_RANGE = 10; // Optimal PvP casting distance
        public const int PVP_SPELL_RELEASE_RANGE_MIN = 5;  // Closer release range for PvP
        public const int PVP_SPELL_RELEASE_RANGE_MAX = 10; // Max release range for PvP

        // Active ranges (set dynamically based on enemy type)
        private int m_MinSafeRange = PVE_MIN_SAFE_RANGE;
        private int m_MaxCastRange = PVE_MAX_CAST_RANGE;
        private int m_OptimalCastRange = PVE_OPTIMAL_CAST_RANGE;
        private int m_SpellReleaseRangeMin = PVE_SPELL_RELEASE_RANGE_MIN;
        private int m_SpellReleaseRangeMax = PVE_SPELL_RELEASE_RANGE_MAX;

        // PvP detection state
        private bool m_IsPvPCombat = false;
        private Mobile m_LastRangeCheckEnemy = null;

        public const int MIN_CAST_RANGE = 3;      // Minimum casting distance
        public const int MIN_RETREAT_DISTANCE = 4;    // Start retreating when enemy within this range (optimized)
        public const int MEDITATION_FLEE_DISTANCE_MIN = 20; // Minimum distance to flee for meditation (reduced from 29)
        public const int MEDITATION_FLEE_DISTANCE_MAX = 30; // Maximum distance to flee for meditation (reduced from 43)
        public const int HEAL_SAFE_DISTANCE = 10;  // Distance to retreat for healing (reduced from 17)
        
        // Chase mode constants - for finishing low-health targets
        public const int CHASE_RANGE = 5;            // Chase to this distance when enemy is low health (guaranteed spell range)
        public const double ENEMY_LOW_HEALTH = 0.35; // Chase aggressively when enemy below 35% health
        public const int GIVE_UP_CHASE_DISTANCE = 24; // Stop chasing beyond this distance (increased for PvP - players kite further)

        // Health/Mana thresholds (optimized via dual-enemy simulation)
        public const double CRITICAL_HEALTH = 0.29;  // 29% health - critical
        public const double LOW_HEALTH = 0.70;       // 70% health - heal (optimized from 62%)
        public const int LOW_MANA = 27;              // Low mana - go invisible to meditate
        public const int CRITICAL_MANA = 18;         // Critical mana - must go invis
        public const int FULL_COMBO_MANA = 55;       // Mana needed for full dump combo

        // Spell combo tracking
        private DateTime m_LastExplosionCast = DateTime.MinValue;
        private DateTime m_LastEnergyBoltCast = DateTime.MinValue;
        private DateTime m_LastCurseCast = DateTime.MinValue;
        private DateTime m_LastPoisonCast = DateTime.MinValue;
        private DateTime m_LastParalyzeCast = DateTime.MinValue;
        private DateTime m_LastManaVampireCast = DateTime.MinValue;
        private DateTime m_NextAllowedCast = DateTime.MinValue;
        private DateTime m_LastInvisTime = DateTime.MinValue;
        private DateTime m_LastInterruptAttempt = DateTime.MinValue;
        private DateTime m_LastBandageTime = DateTime.MinValue;
        private int m_LastHealth = 0; // Track health to detect damage
        private bool m_TookDamageThisTick = false; // Flag to track if damage was taken this tick

        // Debuff tracking with timestamps
        private DateTime m_CurseAppliedTime = DateTime.MinValue;
        private DateTime m_PoisonAppliedTime = DateTime.MinValue;
        private DateTime m_ParalyzeAppliedTime = DateTime.MinValue;
        private Mobile m_DebuffTarget = null;

        // Debuff durations and cooldowns
        private const double CURSE_DURATION = 30.0;
        private const double POISON_DURATION = 20.0;
        private const double PARALYZE_DURATION = 6.0;
        private const double CURSE_COOLDOWN = 2.5;
        private const double POISON_COOLDOWN = 2.0;
        private const double PARALYZE_COOLDOWN = 3.0;
        private const double MANA_VAMPIRE_COOLDOWN = 4.0;

        // Stuck detection
        private int m_ConsecutiveRetreatFailures = 0;
        private Point3D m_LastPosition = Point3D.Zero;
        private DateTime m_LastPositionCheck = DateTime.MinValue;
        private const int STUCK_THRESHOLD = 2; // After 2 failed retreats, consider stuck (optimized: 99% victory rate)
        private const int STUCK_DISTANCE_THRESHOLD = 2; // If we haven't moved more than 2 tiles, we're stuck

        private CombatCombo m_CurrentCombo = CombatCombo.None;
        private IDamageable m_ComboTarget = null;
        private int m_ComboStep = 0;

        // Spell tracking
        private bool m_WaitingForEnergyBolt = false;
        private bool m_WaitingForPoison = false;

        // Corner detection
        private Point3D m_LastCornerTarget = Point3D.Zero;
        private DateTime m_LastCornerCheck = DateTime.MinValue;
        private const int CORNER_CHECK_INTERVAL = 500; // ms
        private const int CORNER_SEARCH_RADIUS = 8; // tiles
        private const int CORNER_MIN_DISTANCE = 3;
        private const int CORNER_MAX_DISTANCE = 8;

        // Directional juking
        private Direction m_LastRetreatDirection = Direction.North;
        private DateTime m_LastDirectionChange = DateTime.MinValue;
        private int m_ConsecutiveSameDirection = 0;
        private const int JUKING_THRESHOLD = 2; // Reverse after 2 moves same direction
        private const int JUKING_COOLDOWN = 1000; // ms

        // Teleport escape when stuck
        private int m_ConsecutiveStuckTicks = 0;
        private DateTime m_LastTeleportTime = DateTime.MinValue;
        private const int STUCK_TICKS_BEFORE_TELEPORT = 5; // Teleport after 5 consecutive stuck detections
        private const int TELEPORT_COOLDOWN_SECONDS = 10; // Can't teleport more than once per 10 seconds
        
        // Potion usage tracking
        // m_LastHealPotionTime removed - using server's BeginAction cooldown instead
        private DateTime m_LastCurePotionTime = DateTime.MinValue;
        private DateTime m_PoisonedTime = DateTime.MinValue; // Track when we got poisoned for reaction delay
        private bool m_WasPoisoned = false; // Track poison state changes
        private DateTime m_LastAgilityPotionTime = DateTime.MinValue;
        private DateTime m_LastRefreshPotionTime = DateTime.MinValue;
        // Server potion cooldowns (from BasePotion classes)
        // Heal potions: 10 second server cooldown via BeginAction(typeof(BaseHealPotion))
        // Cure potions: NO server cooldown - can drink back-to-back
        // Agility/Strength potions: NO cooldown but can't stack (already under effect)
        // Refresh potions: NO server cooldown
        private const double CURE_POTION_COOLDOWN = 0.5; // Small delay to avoid spam (server has no cooldown)
        private const double AGILITY_POTION_COOLDOWN = 120.0; // 2 minute cooldown (potion effect lasts ~2 min)
        private const double REFRESH_POTION_COOLDOWN = 2.0; // Small delay (server has no cooldown)
        private const int TELEPORT_MANA_COST = 9; // Teleport costs 9 mana

        // Distance hysteresis - prevents rapid oscillation between "too close" and "too far"
        private int m_LastReportedDistance = -1;
        private DateTime m_LastDistanceChange = DateTime.MinValue;
        private const int DISTANCE_HYSTERESIS = 2; // Only react to distance changes > 2 tiles
        private const double DISTANCE_DEBOUNCE_MS = 500; // Minimum time between distance-based action changes

        // Predictive positioning (optimized via simulation - 99% victory rate)
        private const double ENEMY_MOVE_SPEED = 1.05; // tiles per 0.5s tick (simulation optimal)
        
        // Spell cast times (in seconds) - optimized via simulation
        private const double CAST_TIME_EXPLOSION = 1.92;
        private const double CAST_TIME_ENERGY_BOLT = 1.52;
        private const double CAST_TIME_GREATER_HEAL = 1.5;
        private const double CAST_TIME_MAGIC_ARROW = 1.0;
        private const double CAST_TIME_CURSE = 1.5;
        private const double CAST_TIME_POISON = 1.0;
        private const double CAST_TIME_PARALYZE = 1.5;

        public enum CombatCombo
        {
            None,
            CurseOnly,            // Apply Curse debuff
            ExplosionEnergyBolt,  // Standard burst: Explosion -> Energy Bolt
            FullDump,             // Full combo: Explosion -> Poison -> Energy Bolt (Curse pre-applied)
            ParalyzeCombo,        // Paralyze -> Explosion -> Energy Bolt
            FinisherCombo,        // Energy Bolt -> Energy Bolt (low health finish)
            ManaVampire,          // Drain enemy mana (mage vs mage)
            Interrupt             // Magic Arrow spam to interrupt
        }

        public enum CombatPriority
        {
            Critical,     // Emergency (teleport, critical heal)
            Defensive,    // Cure, heal
            Setup,        // Apply Curse before combos
            Offensive,    // Attack combos
            Interrupt,    // Interrupt enemy casting
            Positioning   // Movement only
        }

        public MageCombatAI(AutonomousSidekick sidekick)
        {
            m_Sidekick = sidekick;
            // Initialize last health to current health
            m_LastHealth = sidekick.Hits;
        }

        #region PvP vs PvE Detection

        /// <summary>
        /// Determines if the target is a PvP target (player or sidekick) vs PvE (creature/monster)
        /// </summary>
        private bool IsPvPTarget(Mobile enemy)
        {
            if (enemy == null) return false;

            // Players are always PvP
            if (enemy.Player) return true;

            // Other sidekicks are PvP
            if (enemy is AutonomousSidekick) return true;

            // Everything else (BaseCreature, monsters) is PvE
            return false;
        }

        /// <summary>
        /// Updates combat ranges based on whether we're fighting a player/sidekick or a creature
        /// </summary>
        private void UpdateCombatRanges(Mobile enemy)
        {
            if (enemy == null) return;

            // Only recalculate if enemy changed
            if (m_LastRangeCheckEnemy == enemy && m_IsPvPCombat == IsPvPTarget(enemy))
                return;

            m_LastRangeCheckEnemy = enemy;
            m_IsPvPCombat = IsPvPTarget(enemy);

            if (m_IsPvPCombat)
            {
                // PvP ranges - tighter, more aggressive
                m_MinSafeRange = PVP_MIN_SAFE_RANGE;
                m_MaxCastRange = PVP_MAX_CAST_RANGE;
                m_OptimalCastRange = PVP_OPTIMAL_CAST_RANGE;
                m_SpellReleaseRangeMin = PVP_SPELL_RELEASE_RANGE_MIN;
                m_SpellReleaseRangeMax = PVP_SPELL_RELEASE_RANGE_MAX;

                Utility.PushColor(ConsoleColor.Cyan);
                Console.WriteLine($"[MageCombatAI] {m_Sidekick.Name} - PvP MODE: Ranges set to MinSafe={m_MinSafeRange}, MaxCast={m_MaxCastRange}");
                Utility.PopColor();
            }
            else
            {
                // PvE ranges - kiting distances
                m_MinSafeRange = PVE_MIN_SAFE_RANGE;
                m_MaxCastRange = PVE_MAX_CAST_RANGE;
                m_OptimalCastRange = PVE_OPTIMAL_CAST_RANGE;
                m_SpellReleaseRangeMin = PVE_SPELL_RELEASE_RANGE_MIN;
                m_SpellReleaseRangeMax = PVE_SPELL_RELEASE_RANGE_MAX;

                Utility.PushColor(ConsoleColor.Cyan);
                Console.WriteLine($"[MageCombatAI] {m_Sidekick.Name} - PvE MODE: Ranges set to MinSafe={m_MinSafeRange}, MaxCast={m_MaxCastRange}");
                Utility.PopColor();
            }
        }

        #endregion

        #region Debuff Status Helpers

        private bool IsCurseActive()
        {
            if (m_DebuffTarget == null) return false;
            return (DateTime.UtcNow - m_CurseAppliedTime).TotalSeconds < CURSE_DURATION;
        }

        private bool IsPoisonActiveOnTarget()
        {
            if (m_DebuffTarget == null) return false;
            if (m_DebuffTarget is Mobile target && target.Poisoned)
                return true;
            return (DateTime.UtcNow - m_PoisonAppliedTime).TotalSeconds < POISON_DURATION;
        }

        private bool IsParalyzeActive()
        {
            if (m_DebuffTarget == null) return false;
            if (m_DebuffTarget is Mobile target && target.Paralyzed)
                return true;
            return (DateTime.UtcNow - m_ParalyzeAppliedTime).TotalSeconds < PARALYZE_DURATION;
        }

        private bool IsEnemyCaster(Mobile enemy)
        {
            if (enemy == null) return false;
            return enemy.Skills.Magery.Value >= 50 || 
                   enemy.Skills.Necromancy.Value >= 50 ||
                   enemy.Spell != null;
        }

        private void ResetDebuffTracking()
        {
            m_CurseAppliedTime = DateTime.MinValue;
            m_PoisonAppliedTime = DateTime.MinValue;
            m_ParalyzeAppliedTime = DateTime.MinValue;
        }

        #endregion

        #region Poison Immunity Detection

        /// <summary>
        /// Check if a creature is immune to poison (undead, constructs, elementals, etc.)
        /// </summary>
        private bool IsPoisonImmune(Mobile target)
        {
            if (target == null) return true;

            // Players are never poison immune (for PvP)
            if (target.Player)
                return false;

            // Sidekicks are never poison immune (for PvP)
            if (target is AutonomousSidekick)
                return false;

            // Check poison resistance - 100% resist means immune
            if (target.PoisonResistance >= 100)
                return true;

            // Check if it's a BaseCreature with specific properties
            if (target is BaseCreature bc)
            {
                // Explicitly poison immune
                if (bc.PoisonImmune != null)
                    return true;

                // Check creature type flags
                if (bc.OppositionGroup == OppositionGroup.FeyAndUndead || IsUndead(bc))
                    return true;

                if (IsConstruct(bc))
                    return true;

                if (IsPoisonCreature(bc))
                    return true;

                if (IsDemon(bc) && bc.PoisonResistance >= 50)
                    return true;
            }

            return false;
        }

        private bool IsUndead(BaseCreature bc)
        {
            int body = bc.Body.BodyID;
            
            // Skeletons
            if (body == 50 || body == 56 || body == 57 || body == 168 || body == 170 || body == 247 || body == 302)
                return true;
            
            // Zombies
            if (body == 3 || body == 8)
                return true;
            
            // Liches
            if (body == 24 || body == 79 || body == 78)
                return true;
            
            // Wraiths, Spectres, Shades
            if (body == 26 || body == 30 || body == 262 || body == 165)
                return true;
            
            // Mummies
            if (body == 154)
                return true;
            
            // Bone Knights, Bone Magi
            if (body == 57 || body == 148 || body == 147)
                return true;
            
            // Revenants, Rotting Corpses
            if (body == 303 || body == 155 || body == 308)
                return true;

            // Vampires
            if (body == 125 || body == 124 || body == 317)
                return true;

            // Shadow Knight, Dark creatures
            if (body == 146 || body == 318 || body == 310 || body == 311 || body == 436 || body == 740)
                return true;

            string typeName = bc.GetType().Name.ToLower();
            if (typeName.Contains("skeleton") || typeName.Contains("zombie") || 
                typeName.Contains("lich") || typeName.Contains("wraith") ||
                typeName.Contains("spectre") || typeName.Contains("shade") ||
                typeName.Contains("mummy") || typeName.Contains("revenant") ||
                typeName.Contains("undead") || typeName.Contains("vampire") ||
                typeName.Contains("boneknight") || typeName.Contains("bonemagi") ||
                typeName.Contains("ghostly") || typeName.Contains("phantom") ||
                typeName.Contains("corpse") || typeName.Contains("spirit") ||
                typeName.Contains("wight") || typeName.Contains("ghoul") ||
                typeName.Contains("banshee") || typeName.Contains("darknight"))
                return true;

            return false;
        }

        private bool IsConstruct(BaseCreature bc)
        {
            int body = bc.Body.BodyID;
            
            // Golems
            if (body == 752 || body == 14 || body == 304 || body == 305 || body == 435)
                return true;

            // Animated weapons/armor
            if (body == 778 || body == 779)
                return true;

            // Exodus creatures
            if (body == 748 || body == 1429)
                return true;

            string typeName = bc.GetType().Name.ToLower();
            if (typeName.Contains("golem") || typeName.Contains("construct") ||
                typeName.Contains("animated") || typeName.Contains("clockwork") ||
                typeName.Contains("exodus") || typeName.Contains("mechanical"))
                return true;

            return false;
        }

        private bool IsPoisonCreature(BaseCreature bc)
        {
            int body = bc.Body.BodyID;
            
            // Poison Elementals
            if (body == 162)
                return true;

            // Giant Serpents, Silver Serpents
            if (body == 21 || body == 92)
                return true;

            // Swamp creatures
            if (body == 85 || body == 779 || body == 0xBC || body == 780 || body == 734 || body == 726)
                return true;

            string typeName = bc.GetType().Name.ToLower();
            if (typeName.Contains("poisonelemental") || typeName.Contains("toxicelemental") ||
                typeName.Contains("plague") || typeName.Contains("venomous") ||
                typeName.Contains("serpent") || typeName.Contains("slith") ||
                typeName.Contains("swamp") || typeName.Contains("toxic"))
                return true;

            if (bc.PoisonResistance >= 70)
                return true;

            return false;
        }

        private bool IsDemon(BaseCreature bc)
        {
            int body = bc.Body.BodyID;

            if (body == 9 || body == 10 || body == 40 || body == 102)
                return true;

            string typeName = bc.GetType().Name.ToLower();
            if (typeName.Contains("balron") || typeName.Contains("daemon") ||
                typeName.Contains("archfiend") || typeName.Contains("abyssal"))
                return true;

            return false;
        }

        #endregion

        /// <summary>
        /// Main combat decision method - called from SidekickAI.DoActionCombat()
        /// Returns true if combat action was taken
        /// </summary>
        public bool DoCombat(Mobile enemy)
        {
            // CRITICAL: Check for ControlOrder changes FIRST
            if (m_Sidekick.Controlled && m_Sidekick.ControlOrder != OrderType.None && m_Sidekick.ControlOrder != OrderType.Attack)
            {
                Utility.PushColor(ConsoleColor.Cyan);
                Console.WriteLine($"[MageCombatAI.DoCombat] {m_Sidekick.Name} - ControlOrder changed to {m_Sidekick.ControlOrder}, stopping combat");
                Utility.PopColor();
                
                m_Sidekick.Combatant = null;
                m_Sidekick.Warmode = false;
                m_CurrentCombo = CombatCombo.None;
                m_ComboTarget = null;
                return false;
            }
            
            if (enemy == null || enemy.Deleted || !enemy.Alive)
            {
                m_CurrentCombo = CombatCombo.None;
                m_ComboTarget = null;
                m_WaitingForEnergyBolt = false;
                m_WaitingForPoison = false;
                return false;
            }

            // Update combat ranges based on enemy type (PvP vs PvE)
            UpdateCombatRanges(enemy);

            int distance = (int)m_Sidekick.GetDistanceToSqrt(enemy);

            // RESOURCE DEPLETION CHECK: If we have no mana and critical health, flee/bandage only
            if (IsResourceDepleted())
            {
                Utility.PushColor(ConsoleColor.Red);
                Console.WriteLine($"[MageCombatAI] {m_Sidekick.Name} - RESOURCE DEPLETED! Mana: {m_Sidekick.Mana}, HP: {m_Sidekick.Hits}/{m_Sidekick.HitsMax} - flee/bandage mode");
                Utility.PopColor();

                // Try bandage first
                if (!m_Sidekick.Poisoned && TryUseBandage())
                {
                    Utility.PushColor(ConsoleColor.Green);
                    Console.WriteLine($"[MageCombatAI] {m_Sidekick.Name} - Using bandage in resource-depleted state");
                    Utility.PopColor();
                }

                // Try heal potion
                if (TryUseHealPotion())
                {
                    Utility.PushColor(ConsoleColor.Green);
                    Console.WriteLine($"[MageCombatAI] {m_Sidekick.Name} - Using heal potion in resource-depleted state");
                    Utility.PopColor();
                }

                // Always flee when resource depleted
                if (distance <= m_MinSafeRange + 5)
                {
                    RunFrom(enemy);
                }
                return true;
            }

            // Reset debuff tracking if target changed
            if (m_DebuffTarget != enemy)
            {
                ResetDebuffTracking();
                m_DebuffTarget = enemy;
            }

            CombatPriority priority = EvaluatePriority(enemy, distance);

            Utility.PushColor(ConsoleColor.Cyan);
            bool poisonImmune = IsPoisonImmune(enemy);
            Console.WriteLine($"[MageCombatAI] {m_Sidekick.Name} vs {enemy.Name} - Distance: {distance}, Priority: {priority}, Combo: {m_CurrentCombo}, Curse: {IsCurseActive()}, PoisonImmune: {poisonImmune}");
            Utility.PopColor();

            switch (priority)
            {
                case CombatPriority.Critical:
                    return HandleCritical(enemy, distance);

                case CombatPriority.Defensive:
                    return HandleDefensive(enemy, distance);

                case CombatPriority.Setup:
                    return HandleSetup(enemy, distance);

                case CombatPriority.Interrupt:
                    return HandleInterrupt(enemy, distance);

                case CombatPriority.Offensive:
                    return HandleOffensive(enemy, distance);

                case CombatPriority.Positioning:
                    return HandlePositioning(enemy, distance);

                default:
                    return false;
            }
        }

        /// <summary>
        /// Evaluate combat priority based on situation
        /// </summary>
        private CombatPriority EvaluatePriority(Mobile enemy, int distance)
        {
            double healthPercent = (double)m_Sidekick.Hits / (double)m_Sidekick.HitsMax;

            m_TookDamageThisTick = m_LastHealth > 0 && m_Sidekick.Hits < m_LastHealth;
            m_LastHealth = m_Sidekick.Hits;

            // PRIORITY 0: ACTIVE COMBO - takes precedence over EVERYTHING except critical health
            if (m_CurrentCombo != CombatCombo.None && m_ComboTarget == enemy)
            {
                if (healthPercent >= CRITICAL_HEALTH)
                    return CombatPriority.Offensive;
            }

            // CRITICAL: Within retreat distance or very low health
            if (distance <= MIN_RETREAT_DISTANCE || healthPercent < CRITICAL_HEALTH)
                return CombatPriority.Critical;

            // DEFENSIVE: Poisoned, low health, or took damage
            if (m_Sidekick.Poisoned || healthPercent < LOW_HEALTH || (m_TookDamageThisTick && m_Sidekick.Hits < m_Sidekick.HitsMax))
                return CombatPriority.Defensive;

            // INTERRUPT: Enemy is casting
            if (enemy.Spell != null && enemy.Spell.IsCasting)
                return CombatPriority.Interrupt;

            // SETUP: Apply Curse if not active and in range
            if (!IsCurseActive() && distance <= m_MaxCastRange &&
                m_Sidekick.Mana >= 11 && DateTime.UtcNow >= m_NextAllowedCast &&
                m_CurrentCombo == CombatCombo.None)
                return CombatPriority.Setup;

            // OFFENSIVE: In casting range with mana (cooldown handled in HandleOffensive)
            if (distance >= m_MinSafeRange && distance <= m_MaxCastRange && m_Sidekick.Mana >= 20)
            {
                return CombatPriority.Offensive;
            }

            // POSITIONING: Need to get into casting range
            return CombatPriority.Positioning;
        }

        #region Setup Phase (Curse)

        private bool HandleSetup(Mobile enemy, int distance)
        {
            if (!IsCurseActive() && CanCastCurse())
            {
                Utility.PushColor(ConsoleColor.DarkYellow);
                Console.WriteLine($"[MageCombatAI] {m_Sidekick.Name} - SETUP: Applying Curse to {enemy.Name}");
                Utility.PopColor();

                return CastCurse(enemy);
            }

            return HandleOffensive(enemy, distance);
        }

        private bool CanCastCurse() =>
            m_Sidekick.Mana >= 11 &&
            m_Sidekick.Skills.Magery.Value >= 40 &&
            (DateTime.UtcNow - m_LastCurseCast).TotalSeconds >= CURSE_COOLDOWN &&
            DateTime.UtcNow >= m_NextAllowedCast;

        private bool CastCurse(Mobile enemy)
        {
            if (enemy == null || enemy.Deleted) return false;
            if (m_Sidekick.Map == null || !m_Sidekick.InLOS(enemy)) return false;

            Utility.PushColor(ConsoleColor.DarkYellow);
            Console.WriteLine($"[MageCombatAI] {m_Sidekick.Name} - Casting CURSE on {enemy.Name}");
            Utility.PopColor();

            var spell = new CurseSpell(m_Sidekick, null);
            spell.Cast();
            m_LastCurseCast = DateTime.UtcNow;
            m_CurseAppliedTime = DateTime.UtcNow;
            m_NextAllowedCast = DateTime.UtcNow + TimeSpan.FromSeconds(2.0);

            if (m_Sidekick.Target != null)
                m_Sidekick.Target.Invoke(m_Sidekick, enemy);

            return true;
        }

        #endregion

        #region New Spell Casting (Poison, Paralyze, Mana Vampire)

        private bool CanCastPoison() =>
            m_Sidekick.Mana >= 9 &&
            m_Sidekick.Skills.Magery.Value >= 40 &&
            (DateTime.UtcNow - m_LastPoisonCast).TotalSeconds >= POISON_COOLDOWN;

        private bool CastPoison(Mobile enemy)
        {
            if (enemy == null || enemy.Deleted)
            {
                Console.WriteLine($"[MageCombatAI] {m_Sidekick.Name} - POISON failed: enemy null/deleted");
                return false;
            }
            if (m_Sidekick.Map == null || !m_Sidekick.InLOS(enemy))
            {
                Console.WriteLine($"[MageCombatAI] {m_Sidekick.Name} - POISON failed: no LOS to {enemy.Name}");
                return false;
            }

            // Check if already casting a spell
            if (m_Sidekick.Spell != null && m_Sidekick.Spell.IsCasting)
            {
                Console.WriteLine($"[MageCombatAI] {m_Sidekick.Name} - POISON failed: already casting {m_Sidekick.Spell.GetType().Name}");
                return false;
            }

            // Check spell cooldown
            if (Core.TickCount - m_Sidekick.NextSpellTime < 0)
            {
                Console.WriteLine($"[MageCombatAI] {m_Sidekick.Name} - POISON failed: spell cooldown (wait {m_Sidekick.NextSpellTime - Core.TickCount}ms)");
                return false;
            }

            Utility.PushColor(ConsoleColor.DarkGreen);
            Console.WriteLine($"[MageCombatAI] {m_Sidekick.Name} - Casting POISON on {enemy.Name}");
            Utility.PopColor();

            m_ComboTarget = enemy;

            var spell = new PoisonSpell(m_Sidekick, null);
            spell.Cast();
            m_LastPoisonCast = DateTime.UtcNow;
            m_PoisonAppliedTime = DateTime.UtcNow;

            // Invoke target immediately (NPC spells don't have cast animation delay)
            if (m_Sidekick.Target != null)
                m_Sidekick.Target.Invoke(m_Sidekick, enemy);

            return true;
        }

        private bool CanCastParalyze() =>
            m_Sidekick.Mana >= 11 &&
            m_Sidekick.Skills.Magery.Value >= 50 &&
            (DateTime.UtcNow - m_LastParalyzeCast).TotalSeconds >= PARALYZE_COOLDOWN &&
            DateTime.UtcNow >= m_NextAllowedCast;

        private bool CastParalyze(Mobile enemy)
        {
            if (enemy == null || enemy.Deleted) return false;
            if (m_Sidekick.Map == null || !m_Sidekick.InLOS(enemy)) return false;

            Utility.PushColor(ConsoleColor.Blue);
            Console.WriteLine($"[MageCombatAI] {m_Sidekick.Name} - Casting PARALYZE on {enemy.Name}");
            Utility.PopColor();

            var spell = new ParalyzeSpell(m_Sidekick, null);
            spell.Cast();
            m_LastParalyzeCast = DateTime.UtcNow;
            m_ParalyzeAppliedTime = DateTime.UtcNow;
            m_NextAllowedCast = DateTime.UtcNow + TimeSpan.FromSeconds(2.0);

            if (m_Sidekick.Target != null)
                m_Sidekick.Target.Invoke(m_Sidekick, enemy);

            return true;
        }

        private bool CanCastManaVampire() =>
            m_Sidekick.Mana >= 40 &&
            m_Sidekick.Skills.Magery.Value >= 80 &&
            (DateTime.UtcNow - m_LastManaVampireCast).TotalSeconds >= MANA_VAMPIRE_COOLDOWN &&
            DateTime.UtcNow >= m_NextAllowedCast;

        private bool CastManaVampire(Mobile enemy)
        {
            if (enemy == null || enemy.Deleted) return false;
            if (m_Sidekick.Map == null || !m_Sidekick.InLOS(enemy)) return false;

            Utility.PushColor(ConsoleColor.DarkMagenta);
            Console.WriteLine($"[MageCombatAI] {m_Sidekick.Name} - Casting MANA VAMPIRE on {enemy.Name}");
            Utility.PopColor();

            var spell = new ManaVampireSpell(m_Sidekick, null);
            spell.Cast();
            m_LastManaVampireCast = DateTime.UtcNow;
            m_NextAllowedCast = DateTime.UtcNow + TimeSpan.FromSeconds(2.5);

            if (m_Sidekick.Target != null)
                m_Sidekick.Target.Invoke(m_Sidekick, enemy);

            return true;
        }

        #endregion

        /// <summary>
        /// Handle critical situations (go invis for mana, emergency heal, retreat)
        /// </summary>
        private bool HandleCritical(Mobile enemy, int distance)
        {
            double healthPercent = (double)m_Sidekick.Hits / (double)m_Sidekick.HitsMax;

            // CRITICAL HEALTH EMERGENCY HEAL - bypass spell cooldown when dying
            // This overrides everything else - survival is the #1 priority
            if (healthPercent < CRITICAL_HEALTH && m_Sidekick.Mana >= 11)
            {
                Utility.PushColor(ConsoleColor.Red);
                Console.WriteLine($"[MageCombatAI] {m_Sidekick.Name} - CRITICAL HEALTH ({healthPercent:P0})! Emergency heal - bypassing cooldown");
                Utility.PopColor();

                // Reset spell cooldown to allow immediate heal
                m_NextAllowedCast = DateTime.MinValue;

                // Cancel any current offensive spell to allow heal
                if (m_Sidekick.Spell != null && m_Sidekick.Spell.IsCasting)
                {
                    var currentSpell = m_Sidekick.Spell as Spell;
                    if (currentSpell != null && !(currentSpell is GreaterHealSpell) && !(currentSpell is HealSpell))
                    {
                        Utility.PushColor(ConsoleColor.Yellow);
                        Console.WriteLine($"[MageCombatAI] {m_Sidekick.Name} - Cancelling {currentSpell.GetType().Name} to cast emergency heal");
                        Utility.PopColor();
                        currentSpell.FinishSequence();
                    }
                }

                // Cast Greater Heal immediately
                if (m_Sidekick.Spell == null || !m_Sidekick.Spell.IsCasting)
                {
                    var healSpell = new GreaterHealSpell(m_Sidekick, null);
                    if (healSpell.Cast())
                    {
                        Utility.PushColor(ConsoleColor.Green);
                        Console.WriteLine($"[MageCombatAI] {m_Sidekick.Name} - EMERGENCY Greater Heal cast at {healthPercent:P0}!");
                        Utility.PopColor();
                        m_NextAllowedCast = DateTime.UtcNow + TimeSpan.FromSeconds(2.5);

                        // Also retreat if too close
                        if (distance <= MIN_RETREAT_DISTANCE)
                        {
                            RunFrom(enemy);
                        }
                        return true;
                    }
                }

                // If spell failed, try bandage
                if (!m_Sidekick.Poisoned && TryUseBandage())
                {
                    Utility.PushColor(ConsoleColor.Green);
                    Console.WriteLine($"[MageCombatAI] {m_Sidekick.Name} - EMERGENCY bandage at {healthPercent:P0}!");
                    Utility.PopColor();
                    if (distance <= MIN_RETREAT_DISTANCE)
                    {
                        RunFrom(enemy);
                    }
                    return true;
                }

                // If bandage failed, try heal potion
                if (TryUseHealPotion())
                {
                    Utility.PushColor(ConsoleColor.Green);
                    Console.WriteLine($"[MageCombatAI] {m_Sidekick.Name} - EMERGENCY heal potion at {healthPercent:P0}!");
                    Utility.PopColor();
                    if (distance <= MIN_RETREAT_DISTANCE)
                    {
                        RunFrom(enemy);
                    }
                    return true;
                }
            }

            bool isStuck = CheckIfStuck();

            bool inRetreatRange = distance <= MIN_RETREAT_DISTANCE;
            bool canCast = m_Sidekick.Mana >= 20 && DateTime.UtcNow >= m_NextAllowedCast && m_Sidekick.InLOS(enemy);

            // FIX: If we have mana and can cast, try to cast-while-kiting instead of just retreating
            if (inRetreatRange && canCast && healthPercent > 0.50)
            {
                Utility.PushColor(ConsoleColor.Green);
                Console.WriteLine($"[MageCombatAI] {m_Sidekick.Name} - Close ({distance} tiles) but have mana ({m_Sidekick.Mana}), casting while kiting");
                Utility.PopColor();
                
                CombatCombo quickCombo = SelectCombo(enemy);
                if (quickCombo != CombatCombo.None)
                {
                    m_CurrentCombo = quickCombo;
                    m_ComboTarget = enemy;
                    if (StartCombo(enemy, distance))
                    {
                        RunFrom(enemy);
                        return true;
                    }
                }
            }
            
            if (inRetreatRange && healthPercent > 0.30 && !isStuck)
            {
                Utility.PushColor(ConsoleColor.Yellow);
                Console.WriteLine($"[MageCombatAI] {m_Sidekick.Name} - In melee ({distance}) with {healthPercent:P0} health, retreating FIRST");
                Utility.PopColor();

                Point3D positionBeforeRetreat = m_Sidekick.Location;
                RunFrom(enemy);
                
                if (m_Sidekick.Location != positionBeforeRetreat)
                {
                    m_ConsecutiveRetreatFailures = 0;
                    return true;
                }
                else
                {
                    m_ConsecutiveRetreatFailures++;
                    Utility.PushColor(ConsoleColor.Yellow);
                    Console.WriteLine($"[MageCombatAI] {m_Sidekick.Name} - Retreat failed, will attempt heal");
                    Utility.PopColor();
                }
            }

            // If stuck, prioritize healing and casting over retreating
            if (isStuck)
            {
                m_ConsecutiveStuckTicks++;
                Utility.PushColor(ConsoleColor.Red);
                Console.WriteLine($"[MageCombatAI] {m_Sidekick.Name} - STUCK DETECTED! Tick {m_ConsecutiveStuckTicks}/{STUCK_TICKS_BEFORE_TELEPORT}");
                Utility.PopColor();

                // STUCK POTIONS
                if (m_ConsecutiveStuckTicks >= 2)
                {
                    if (m_Sidekick.Stam < m_Sidekick.StamMax * 0.5)
                    {
                        if (TryUseRefreshPotion())
                        {
                            Utility.PushColor(ConsoleColor.Magenta);
                            Console.WriteLine($"[MageCombatAI] {m_Sidekick.Name} - STUCK: Refreshed stamina to help escape!");
                            Utility.PopColor();
                        }
                    }
                    
                    if (TryUseAgilityPotion())
                    {
                        Utility.PushColor(ConsoleColor.Magenta);
                        Console.WriteLine($"[MageCombatAI] {m_Sidekick.Name} - STUCK: Boosted dex to help push through!");
                        Utility.PopColor();
                    }
                }

                // TELEPORT ESCAPE
                if (m_ConsecutiveStuckTicks >= STUCK_TICKS_BEFORE_TELEPORT &&
                    m_Sidekick.Mana >= TELEPORT_MANA_COST &&
                    DateTime.UtcNow >= m_LastTeleportTime.AddSeconds(TELEPORT_COOLDOWN_SECONDS) &&
                    DateTime.UtcNow >= m_NextAllowedCast)
                {
                    if (TryTeleportEscape(enemy))
                    {
                        m_ConsecutiveStuckTicks = 0;
                        m_ConsecutiveRetreatFailures = 0;
                        return true;
                    }
                }

                // Heal while stuck
                if (healthPercent < LOW_HEALTH && m_Sidekick.Mana >= 11 && DateTime.UtcNow >= m_NextAllowedCast)
                {
                    Utility.PushColor(ConsoleColor.Green);
                    Console.WriteLine($"[MageCombatAI] {m_Sidekick.Name} - STUCK: Healing at {healthPercent:P0} health");
                    Utility.PopColor();

                    var healSpell = new GreaterHealSpell(m_Sidekick, null);
                    if (healSpell != null)
                    {
                        healSpell.Cast();
                        m_NextAllowedCast = DateTime.UtcNow + TimeSpan.FromSeconds(2.5);
                        m_ConsecutiveRetreatFailures = 0;

                        if (distance <= MIN_RETREAT_DISTANCE)
                        {
                            RunFrom(enemy);
                        }

                        return true;
                    }
                }

                // Cast a spell when stuck
                if (m_Sidekick.Mana >= 20 && DateTime.UtcNow >= m_NextAllowedCast && distance <= m_MaxCastRange)
                {
                    if (m_Sidekick.Map != null && m_Sidekick.InLOS(enemy))
                    {
                        Utility.PushColor(ConsoleColor.Yellow);
                        Console.WriteLine($"[MageCombatAI] {m_Sidekick.Name} - STUCK: Casting spell at close range ({distance} tiles)");
                        Utility.PopColor();

                        var spell = new EnergyBoltSpell(m_Sidekick, null);
                        if (spell != null)
                        {
                            spell.Cast();
                            m_NextAllowedCast = DateTime.UtcNow + TimeSpan.FromSeconds(2.0);
                            m_ConsecutiveRetreatFailures = 0;
                            return true;
                        }
                    }
                }

                if (TryUseBandage())
                {
                    m_ConsecutiveRetreatFailures = 0;
                    return true;
                }
            }
            else
            {
                m_ConsecutiveStuckTicks = 0;
            }

            // Low mana - go invisible to meditate
            if (m_Sidekick.Mana < CRITICAL_MANA && CanCastInvisibility())
            {
                Utility.PushColor(ConsoleColor.Magenta);
                Console.WriteLine($"[MageCombatAI] {m_Sidekick.Name} - CRITICAL MANA ({m_Sidekick.Mana}), going invisible to meditate!");
                Utility.PopColor();

                return CastInvisibility();
            }

            // Very low health emergency heal
            if (healthPercent < CRITICAL_HEALTH && m_Sidekick.Mana >= 11 && DateTime.UtcNow >= m_NextAllowedCast)
            {
                Utility.PushColor(ConsoleColor.Red);
                Console.WriteLine($"[MageCombatAI] {m_Sidekick.Name} - CRITICAL HEALTH ({healthPercent:P0}), emergency heal!");
                Utility.PopColor();

                var healSpell = new GreaterHealSpell(m_Sidekick, null);
                if (healSpell != null)
                {
                    healSpell.Cast();
                    m_NextAllowedCast = DateTime.UtcNow + TimeSpan.FromSeconds(2.5);
                    m_ConsecutiveRetreatFailures = 0;
                    
                    if (distance <= MIN_RETREAT_DISTANCE)
                    {
                        RunFrom(enemy);
                    }
                    
                    return true;
                }
            }
            else if (healthPercent < CRITICAL_HEALTH && m_Sidekick.Mana < 11 && !m_Sidekick.Poisoned)
            {
                Utility.PushColor(ConsoleColor.Red);
                Console.WriteLine($"[MageCombatAI] {m_Sidekick.Name} - CRITICAL HEALTH ({healthPercent:P0}) but low mana ({m_Sidekick.Mana}), trying heal potion!");
                Utility.PopColor();

                if (TryUseHealPotion())
                {
                    if (distance <= MIN_RETREAT_DISTANCE)
                    {
                        RunFrom(enemy);
                    }
                    return true;
                }
            }

            // Retreat from melee range
            if (distance <= m_MinSafeRange && !isStuck)
            {
                Utility.PushColor(ConsoleColor.Yellow);
                Console.WriteLine($"[MageCombatAI] {m_Sidekick.Name} - In melee range ({distance}), retreating!");
                Utility.PopColor();

                Point3D positionBeforeRetreat = m_Sidekick.Location;
                RunFrom(enemy);
                
                if (m_Sidekick.Location == positionBeforeRetreat)
                {
                    m_ConsecutiveRetreatFailures++;
                }
                else
                {
                    m_ConsecutiveRetreatFailures = 0;
                    m_LastPosition = m_Sidekick.Location;
                    m_LastPositionCheck = DateTime.UtcNow;
                }
                
                return true;
            }

            return false;
        }

        /// <summary>
        /// Check if the sidekick is stuck
        /// </summary>
        private bool CheckIfStuck()
        {
            if (m_ConsecutiveRetreatFailures >= STUCK_THRESHOLD)
            {
                return true;
            }

            if (m_LastPositionCheck != DateTime.MinValue && (DateTime.UtcNow - m_LastPositionCheck).TotalSeconds >= 2.0)
            {
                if (m_LastPosition != Point3D.Zero)
                {
                    int distanceMoved = (int)m_Sidekick.GetDistanceToSqrt(m_LastPosition);
                    if (distanceMoved <= STUCK_DISTANCE_THRESHOLD)
                    {
                        return true;
                    }
                }

                m_LastPosition = m_Sidekick.Location;
                m_LastPositionCheck = DateTime.UtcNow;
            }
            else if (m_LastPositionCheck == DateTime.MinValue)
            {
                m_LastPosition = m_Sidekick.Location;
                m_LastPositionCheck = DateTime.UtcNow;
            }

            return false;
        }

        /// <summary>
        /// Check if we should react to a distance change (applies hysteresis to prevent oscillation)
        /// </summary>
        private bool ShouldReactToDistanceChange(int currentDistance)
        {
            // Always react if this is the first check
            if (m_LastReportedDistance < 0)
            {
                m_LastReportedDistance = currentDistance;
                m_LastDistanceChange = DateTime.UtcNow;
                return true;
            }

            // Check if distance changed significantly
            int deltaDistance = Math.Abs(currentDistance - m_LastReportedDistance);
            double timeSinceLastChange = (DateTime.UtcNow - m_LastDistanceChange).TotalMilliseconds;

            // React immediately to critical situations (enemy very close or very far)
            if (currentDistance <= MIN_RETREAT_DISTANCE || currentDistance > GIVE_UP_CHASE_DISTANCE)
            {
                m_LastReportedDistance = currentDistance;
                m_LastDistanceChange = DateTime.UtcNow;
                return true;
            }

            // Apply hysteresis - only react if distance changed enough AND enough time passed
            if (deltaDistance > DISTANCE_HYSTERESIS && timeSinceLastChange > DISTANCE_DEBOUNCE_MS)
            {
                m_LastReportedDistance = currentDistance;
                m_LastDistanceChange = DateTime.UtcNow;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Check if the sidekick is resource-depleted (no mana to cast, low HP)
        /// Returns true if should flee/bandage instead of trying to cast
        /// </summary>
        private bool IsResourceDepleted()
        {
            // Can't do anything useful with no mana
            bool noMana = m_Sidekick.Mana < 6; // Can't even cast Cure
            bool criticalHealth = (double)m_Sidekick.Hits / (double)m_Sidekick.HitsMax < CRITICAL_HEALTH;

            return noMana && criticalHealth;
        }

        /// <summary>
        /// Handle defensive actions (cure, heal, meditate, invisibility)
        /// </summary>
        private bool HandleDefensive(Mobile enemy, int distance)
        {
            double healthPercent = (double)m_Sidekick.Hits / (double)m_Sidekick.HitsMax;

            // Low mana and at safe distance - go invisible to meditate
            if (m_Sidekick.Mana < LOW_MANA && distance >= MEDITATION_FLEE_DISTANCE_MIN && CanCastInvisibility())
            {
                Utility.PushColor(ConsoleColor.Magenta);
                Console.WriteLine($"[MageCombatAI] {m_Sidekick.Name} - Low mana ({m_Sidekick.Mana}) at safe distance ({distance}), going invisible to meditate");
                Utility.PopColor();

                return CastInvisibility();
            }

            // Low mana but NOT at safe distance - SPRINT away
            if (m_Sidekick.Mana < LOW_MANA)
            {
                if (distance >= MEDITATION_FLEE_DISTANCE_MIN)
                {
                    if (CanCastInvisibility())
                    {
                        return CastInvisibility();
                    }

                    if (!m_Sidekick.Meditating && m_Sidekick.Skills[SkillName.Meditation].Value > 0)
                    {
                        try
                        {
                            m_Sidekick.UseSkill(SkillName.Meditation);
                        }
                        catch { }
                    }

                    return true;
                }
                else
                {
                    int targetFleeDistance = (MEDITATION_FLEE_DISTANCE_MIN + MEDITATION_FLEE_DISTANCE_MAX) / 2;
                    Utility.PushColor(ConsoleColor.Yellow);
                    Console.WriteLine($"[MageCombatAI] {m_Sidekick.Name} - Low mana ({m_Sidekick.Mana}), SPRINTING to {targetFleeDistance} tiles away");
                    Utility.PopColor();

                    Direction retreatDir = (Direction)(((int)m_Sidekick.GetDirectionTo(enemy) - 4) & (int)Direction.Mask);
                    Point3D currentPos = m_Sidekick.Location;
                    Point3D farRetreatPoint = CalculatePointInDirection(currentPos, retreatDir, targetFleeDistance);

                    if (m_Sidekick.AIObject is SidekickAI sidekickAI)
                    {
                        sidekickAI.MoveTo(farRetreatPoint, true, 0);
                    }
                    else
                    {
                        for (int i = 0; i < 10; i++)
                        {
                            RunFrom(enemy);
                        }
                    }

                    return true;
                }
            }

            // Cure poison
            if (m_Sidekick.Poisoned && m_Sidekick.Mana >= 6 && DateTime.UtcNow >= m_NextAllowedCast)
            {
                Utility.PushColor(ConsoleColor.Yellow);
                Console.WriteLine($"[MageCombatAI] {m_Sidekick.Name} - Poisoned, casting cure");
                Utility.PopColor();

                var cureSpell = new CureSpell(m_Sidekick, null);
                if (cureSpell != null)
                {
                    cureSpell.Cast();
                    m_NextAllowedCast = DateTime.UtcNow + TimeSpan.FromSeconds(2.0);
                    return true;
                }
            }
            else if (m_Sidekick.Poisoned && m_Sidekick.Mana < 6)
            {
                if (TryUseCurePotion())
                {
                    return true;
                }
            }

            // Heal if low health or took damage
            if (healthPercent < LOW_HEALTH || (m_TookDamageThisTick && m_Sidekick.Hits < m_Sidekick.HitsMax))
            {
                bool inRetreatRange = distance <= MIN_RETREAT_DISTANCE;
                bool healedThisTick = false;

                Utility.PushColor(ConsoleColor.Green);
                Console.WriteLine($"[MageCombatAI] {m_Sidekick.Name} - DEFENSIVE: Low health ({healthPercent:P0}) at distance {distance}, attempting heal");
                Utility.PopColor();

                // PRIORITY 1: If we can cast Greater Heal, do it immediately (regardless of range)
                bool canCastHeal = m_Sidekick.Mana >= 11;
                bool cooldownReady = DateTime.UtcNow >= m_NextAllowedCast;
                bool notCasting = m_Sidekick.Spell == null || !m_Sidekick.Spell.IsCasting;

                if (!healedThisTick && canCastHeal && cooldownReady && notCasting)
                {
                    Utility.PushColor(ConsoleColor.Green);
                    Console.WriteLine($"[MageCombatAI] {m_Sidekick.Name} - Casting Greater Heal (HP: {healthPercent:P0})");
                    Utility.PopColor();

                    var healSpell = new GreaterHealSpell(m_Sidekick, null);
                    if (healSpell != null)
                    {
                        healSpell.Cast();
                        m_NextAllowedCast = DateTime.UtcNow + TimeSpan.FromSeconds(2.5);
                        m_ConsecutiveRetreatFailures = 0;
                        healedThisTick = true;
                    }
                }
                else if (!healedThisTick)
                {
                    // Log why we can't cast heal
                    string reason = !canCastHeal ? $"low mana ({m_Sidekick.Mana})" :
                                   !cooldownReady ? "spell cooldown" :
                                   !notCasting ? $"casting {m_Sidekick.Spell?.GetType().Name}" : "unknown";
                    Utility.PushColor(ConsoleColor.Yellow);
                    Console.WriteLine($"[MageCombatAI] {m_Sidekick.Name} - Cannot cast Greater Heal: {reason}");
                    Utility.PopColor();
                }

                // PRIORITY 2: Use bandage if spell healing not available
                if (!healedThisTick && !m_Sidekick.Poisoned)
                {
                    bool bandageUsed = TryUseBandage();
                    if (bandageUsed)
                    {
                        Utility.PushColor(ConsoleColor.Green);
                        Console.WriteLine($"[MageCombatAI] {m_Sidekick.Name} - Using bandage (HP: {healthPercent:P0})");
                        Utility.PopColor();
                        healedThisTick = true;
                    }
                }

                // PRIORITY 3: Use heal potion as last resort
                if (!healedThisTick && TryUseHealPotion())
                {
                    Utility.PushColor(ConsoleColor.Green);
                    Console.WriteLine($"[MageCombatAI] {m_Sidekick.Name} - Using heal potion (HP: {healthPercent:P0})");
                    Utility.PopColor();
                    healedThisTick = true;
                }

                // After healing, retreat if too close
                if (inRetreatRange)
                {
                    Point3D positionBeforeRetreat = m_Sidekick.Location;
                    RunFrom(enemy);
                    if (m_Sidekick.Location != positionBeforeRetreat)
                    {
                        m_ConsecutiveRetreatFailures = 0;
                    }
                    else
                    {
                        m_ConsecutiveRetreatFailures++;
                    }
                }

                // Return true if we healed or retreated
                if (healedThisTick || inRetreatRange)
                {
                    return true;
                }
            }

            // CRITICAL: After healing in melee range, ALWAYS retreat
            if (distance < m_MinSafeRange)
            {
                bool isStuck = CheckIfStuck();
                
                Point3D positionBeforeRetreat = m_Sidekick.Location;
                int retreatAttempts = 0;
                const int MAX_RETREAT_ATTEMPTS = 3;
                
                while (retreatAttempts < MAX_RETREAT_ATTEMPTS && m_Sidekick.Location == positionBeforeRetreat && distance < m_MinSafeRange)
                {
                    RunFrom(enemy);
                    retreatAttempts++;
                    distance = (int)m_Sidekick.GetDistanceToSqrt(enemy);
                    
                    if (m_Sidekick.Location != positionBeforeRetreat)
                    {
                        m_ConsecutiveRetreatFailures = 0;
                        m_LastPosition = m_Sidekick.Location;
                        m_LastPositionCheck = DateTime.UtcNow;
                        return true;
                    }
                }
                
                if (m_Sidekick.Location == positionBeforeRetreat)
                {
                    m_ConsecutiveRetreatFailures++;
                }
                
                return true;
            }

            return false;
        }

        /// <summary>
        /// Handle spell interruption (Magic Arrow spam)
        /// </summary>
        private bool HandleInterrupt(Mobile enemy, int distance)
        {
            if (distance > m_MaxCastRange || m_Sidekick.Mana < 4)
                return false;

            if ((DateTime.UtcNow - m_LastInterruptAttempt).TotalSeconds < 1.0)
                return false;

            if (DateTime.UtcNow >= m_NextAllowedCast)
            {
                if (m_Sidekick.Map == null || !m_Sidekick.InLOS(enemy))
                {
                    return false;
                }

                // Cycle between Magic Arrow and Poison for interrupts
                // Use Poison if: target not poisoned, not immune, we have mana, and cooldown ready
                bool usePoison = !IsPoisonImmune(enemy) &&
                                 !(enemy is Mobile mob && mob.Poisoned) &&
                                 m_Sidekick.Mana >= 9 &&
                                 CanCastPoison();

                if (usePoison)
                {
                    Utility.PushColor(ConsoleColor.DarkGreen);
                    Console.WriteLine($"[MageCombatAI] {m_Sidekick.Name} - Interrupting {enemy.Name} with Poison");
                    Utility.PopColor();

                    m_ComboTarget = enemy;
                    var spell = new PoisonSpell(m_Sidekick, null);
                    spell.Cast();
                    m_LastPoisonCast = DateTime.UtcNow;
                    m_NextAllowedCast = DateTime.UtcNow + TimeSpan.FromSeconds(1.0);

                    // Invoke target immediately
                    if (m_Sidekick.Target != null)
                        m_Sidekick.Target.Invoke(m_Sidekick, enemy);
                }
                else
                {
                    Utility.PushColor(ConsoleColor.Magenta);
                    Console.WriteLine($"[MageCombatAI] {m_Sidekick.Name} - Interrupting {enemy.Name} with Magic Arrow");
                    Utility.PopColor();

                    var spell = new MagicArrowSpell(m_Sidekick, null);
                    spell.Cast();
                    m_NextAllowedCast = DateTime.UtcNow + TimeSpan.FromSeconds(0.75);
                }

                m_LastInterruptAttempt = DateTime.UtcNow;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Predict enemy position after a given cast time
        /// </summary>
        private int PredictEnemyDistanceAfterCastTime(int currentDistance, double castTimeSeconds)
        {
            double enemyMovementTiles = (castTimeSeconds / 0.5) * ENEMY_MOVE_SPEED;
            int predictedDistance = (int)(currentDistance - enemyMovementTiles);
            
            if (predictedDistance < 1)
                predictedDistance = 1;
            
            return predictedDistance;
        }
        
        /// <summary>
        /// Decide if it's safe to start casting based on predicted distance
        /// </summary>
        private bool ShouldStartCasting(int currentDistance, double castTimeSeconds, int spellReleaseRangeMin, int spellReleaseRangeMax)
        {
            int predictedDistance = PredictEnemyDistanceAfterCastTime(currentDistance, castTimeSeconds);
            return predictedDistance >= spellReleaseRangeMin;
        }
        
        /// <summary>
        /// Calculate dynamic retreat distance based on aggressor distance
        /// </summary>
        private int CalculateRetreatDistance(int aggressorDistance)
        {
            if (aggressorDistance < 10)
                return HEAL_SAFE_DISTANCE;
            
            if (aggressorDistance < 15)
                return HEAL_SAFE_DISTANCE - 2;
            
            if (aggressorDistance <= m_OptimalCastRange)
                return aggressorDistance;
            
            return m_OptimalCastRange;
        }

        /// <summary>
        /// Handle offensive spell combos with predictive kiting
        /// </summary>
        private bool HandleOffensive(Mobile enemy, int distance)
        {
            // PRIORITY 1: Continue active combo
            if (m_CurrentCombo != CombatCombo.None && m_ComboTarget == enemy)
            {
                return ContinueCombo(enemy, distance);
            }

            // Spell is currently casting -> stay in place
            if (m_Sidekick.Spell != null && m_Sidekick.Spell.IsCasting)
            {
                Utility.PushColor(ConsoleColor.Cyan);
                Console.WriteLine($"[MageCombatAI] {m_Sidekick.Name} - Spell casting, staying in place at {distance} tiles");
                Utility.PopColor();
                return true;
            }

            // Spell sequencing -> move to release range if needed
            if (m_Sidekick.Spell != null && m_Sidekick.Spell is Server.Spells.Spell spell && spell.State == Server.Spells.SpellState.Sequencing)
            {
                if (distance < m_SpellReleaseRangeMin)
                {
                    RunFrom(enemy);
                    return true;
                }
                else if (distance > m_SpellReleaseRangeMax)
                {
                    if (m_Sidekick.AIObject is SidekickAI sidekickAI)
                    {
                        sidekickAI.MoveTo(enemy, true, m_SpellReleaseRangeMax);
                    }
                    return true;
                }
                else
                {
                    return true;
                }
            }

            // Predictive kiting: If too close, retreat to optimal range first
            if (distance < m_MinSafeRange)
            {
                bool isStuck = CheckIfStuck();
                
                // If stuck, allow casting even if slightly too close
                if (isStuck && distance >= 8 && distance <= m_MaxCastRange &&
                    m_Sidekick.Mana >= 20 && DateTime.UtcNow >= m_NextAllowedCast &&
                    m_Sidekick.Map != null && m_Sidekick.InLOS(enemy))
                {
                    Utility.PushColor(ConsoleColor.Green);
                    Console.WriteLine($"[MageCombatAI] {m_Sidekick.Name} - STUCK at {distance} tiles, casting despite being close");
                    Utility.PopColor();
                    
                    CombatCombo stuckCombo = SelectCombo(enemy);
                    if (stuckCombo != CombatCombo.None)
                    {
                        m_CurrentCombo = stuckCombo;
                        m_ComboTarget = enemy;
                        return StartCombo(enemy, distance);
                    }
                }
                
                if (!isStuck)
                {
                    Utility.PushColor(ConsoleColor.Yellow);
                    Console.WriteLine($"[MageCombatAI] {m_Sidekick.Name} - Too close ({distance} < {m_MinSafeRange}), retreating to {m_OptimalCastRange} tiles");
                    Utility.PopColor();
                    
                    int targetDistance = CalculateRetreatDistance(distance);
                    for (int i = 0; i < 5 && distance < targetDistance; i++)
                    {
                        RunFrom(enemy);
                        distance = (int)m_Sidekick.GetDistanceToSqrt(enemy);
                    }
                    return true;
                }
            }

            // If too far, RUN to chase/optimal range - but only if within give-up distance
            if (distance > m_MaxCastRange)
            {
                // HOLD POSITION if enemy is beyond chase threshold - stay in combat but don't chase
                // This keeps us ready if they come back with an Explosion combo
                if (distance > GIVE_UP_CHASE_DISTANCE)
                {
                    Utility.PushColor(ConsoleColor.Yellow);
                    Console.WriteLine($"[MageCombatAI] {m_Sidekick.Name} - Enemy at {distance} tiles - HOLDING POSITION (beyond chase distance {GIVE_UP_CHASE_DISTANCE})");
                    Utility.PopColor();

                    // Meditate/recover while waiting if possible
                    if (m_Sidekick.Mana < m_Sidekick.ManaMax && !m_Sidekick.Meditating &&
                        m_Sidekick.Skills[SkillName.Meditation].Value > 0)
                    {
                        try { m_Sidekick.UseSkill(SkillName.Meditation); } catch { }
                    }

                    // Use bandage if hurt while waiting
                    if (m_Sidekick.Hits < m_Sidekick.HitsMax && !m_Sidekick.Poisoned)
                    {
                        TryUseBandage();
                    }

                    return true; // Stay in combat, just don't chase
                }

                double enemyHealthPct = (double)enemy.Hits / (double)enemy.HitsMax;
                int targetRange = (enemyHealthPct < ENEMY_LOW_HEALTH) ? CHASE_RANGE : m_OptimalCastRange;

                Utility.PushColor(ConsoleColor.Yellow);
                if (targetRange == CHASE_RANGE)
                    Console.WriteLine($"[MageCombatAI] {m_Sidekick.Name} - CHASE MODE: Enemy at {enemyHealthPct:P0} health, RUNNING to {targetRange} tiles!");
                else
                    Console.WriteLine($"[MageCombatAI] {m_Sidekick.Name} - Too far ({distance} > {m_MaxCastRange}), RUNNING to {targetRange} tiles");
                Utility.PopColor();

                if (m_Sidekick.AIObject is SidekickAI sidekickAI)
                {
                    sidekickAI.MoveTo(enemy, true, targetRange);
                }
                return true;
            }

            // At good distance - select combo
            CombatCombo selectedCombo = SelectCombo(enemy);
            
            if (selectedCombo != CombatCombo.None)
            {
                double castTime = CAST_TIME_EXPLOSION;
                if (selectedCombo == CombatCombo.FinisherCombo)
                    castTime = CAST_TIME_ENERGY_BOLT;
                
                bool safeToCast = ShouldStartCasting(distance, castTime, m_SpellReleaseRangeMin, m_SpellReleaseRangeMax);
                
                if (safeToCast)
                {
                    int predictedDistance = PredictEnemyDistanceAfterCastTime(distance, castTime);
                    Utility.PushColor(ConsoleColor.Green);
                    Console.WriteLine($"[MageCombatAI] {m_Sidekick.Name} - At {distance} tiles, predicted distance {predictedDistance} - starting combo");
                    Utility.PopColor();
                    
                    m_CurrentCombo = selectedCombo;
                    m_ComboTarget = enemy;
                    m_ComboStep = 0;
                    return StartCombo(enemy, distance);
                }
                else
                {
                    int predictedDistance = PredictEnemyDistanceAfterCastTime(distance, castTime);
                    double enemyHealthPct = (double)enemy.Hits / (double)enemy.HitsMax;
                    
                    // If enemy is low health, DON'T retreat - stand and fight!
                    if (enemyHealthPct < ENEMY_LOW_HEALTH && predictedDistance < m_SpellReleaseRangeMin)
                    {
                        Utility.PushColor(ConsoleColor.Green);
                        Console.WriteLine($"[MageCombatAI] {m_Sidekick.Name} - Enemy at {enemyHealthPct:P0} health - STANDING GROUND to finish!");
                        Utility.PopColor();
                        
                        m_CurrentCombo = selectedCombo;
                        m_ComboTarget = enemy;
                        m_ComboStep = 0;
                        return StartCombo(enemy, distance);
                    }
                    
                    // Adjust position
                    if (predictedDistance < m_SpellReleaseRangeMin)
                    {
                        RunFrom(enemy);
                    }
                    else if (predictedDistance > m_SpellReleaseRangeMax)
                    {
                        if (m_Sidekick.AIObject is SidekickAI sidekickAI)
                        {
                            sidekickAI.MoveTo(enemy, true, m_SpellReleaseRangeMax);
                        }
                    }
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Select appropriate combo based on situation
        /// </summary>
        private CombatCombo SelectCombo(Mobile enemy)
        {
            double enemyHealthPercent = (double)enemy.Hits / (double)enemy.HitsMax;
            bool curseActive = IsCurseActive();
            bool enemyPoisoned = IsPoisonActiveOnTarget();

            // Execute mode: Enemy low health - use Paralyze or spam Energy Bolt
            if (enemyHealthPercent < ENEMY_LOW_HEALTH && m_Sidekick.Mana >= 20)
            {
                if (m_Sidekick.Mana >= 31 && !IsParalyzeActive() && CanCastParalyze())
                {
                    return CombatCombo.ParalyzeCombo;
                }
                return CombatCombo.FinisherCombo;
            }

            // Mana Vampire: Enemy is a caster with decent mana
            if (IsEnemyCaster(enemy) && enemy.Mana > 30 && m_Sidekick.Mana >= 40 && CanCastManaVampire())
            {
                if ((DateTime.UtcNow - m_LastManaVampireCast).TotalSeconds > 10)
                {
                    return CombatCombo.ManaVampire;
                }
            }

            // Full dump: Curse active, have mana, target can be poisoned
            if (curseActive && m_Sidekick.Mana >= FULL_COMBO_MANA && !enemyPoisoned && !IsPoisonImmune(enemy))
            {
                return CombatCombo.FullDump;
            }

            // Standard combo: Explosion -> Energy Bolt
            if (m_Sidekick.Mana >= 40)
            {
                return CombatCombo.ExplosionEnergyBolt;
            }

            // Low mana fallback
            if (m_Sidekick.Mana >= 20)
            {
                return CombatCombo.FinisherCombo;
            }
            
            return CombatCombo.None;
        }

        /// <summary>
        /// Start a new combo
        /// </summary>
        private bool StartCombo(Mobile enemy, int distance)
        {
            Utility.PushColor(ConsoleColor.Green);
            Console.WriteLine($"[MageCombatAI] {m_Sidekick.Name} - Starting combo: {m_CurrentCombo} on {enemy.Name}");
            Utility.PopColor();

            switch (m_CurrentCombo)
            {
                case CombatCombo.FullDump:
                    // Full combo: Explosion -> Poison -> Energy Bolt
                    if (CastExplosionOnly(enemy))
                    {
                        m_LastExplosionCast = DateTime.UtcNow;
                        m_WaitingForPoison = true;
                        m_ComboStep = 1;
                        m_NextAllowedCast = DateTime.UtcNow + TimeSpan.FromSeconds(1.5);
                        return true;
                    }
                    break;

                case CombatCombo.ExplosionEnergyBolt:
                    if (CastExplosionOnly(enemy))
                    {
                        m_LastExplosionCast = DateTime.UtcNow;
                        m_WaitingForEnergyBolt = true;
                        m_ComboStep = 1;
                        m_NextAllowedCast = DateTime.UtcNow + TimeSpan.FromSeconds(3.0);
                        return true;
                    }
                    break;

                case CombatCombo.ParalyzeCombo:
                    if (CastParalyze(enemy))
                    {
                        m_ComboStep = 1;
                        m_NextAllowedCast = DateTime.UtcNow + TimeSpan.FromSeconds(2.0);
                        return true;
                    }
                    break;

                case CombatCombo.FinisherCombo:
                    if (CastEnergyBolt(enemy))
                    {
                        m_LastEnergyBoltCast = DateTime.UtcNow;
                        m_NextAllowedCast = DateTime.UtcNow + TimeSpan.FromSeconds(2.0);
                        m_ConsecutiveRetreatFailures = 0;
                        return true;
                    }
                    break;

                case CombatCombo.ManaVampire:
                    if (CastManaVampire(enemy))
                    {
                        m_LastManaVampireCast = DateTime.UtcNow;
                        m_CurrentCombo = CombatCombo.None;
                        m_NextAllowedCast = DateTime.UtcNow + TimeSpan.FromSeconds(2.5);
                        return true;
                    }
                    break;
            }

            // Combo failed to start
            m_CurrentCombo = CombatCombo.None;
            m_ComboTarget = null;
            return false;
        }

        /// <summary>
        /// Continue an active combo
        /// </summary>
        private bool ContinueCombo(Mobile enemy, int distance)
        {
            // KITE WHILE COMBOING: Always retreat if enemy gets too close during any combo!
            if (distance <= MIN_RETREAT_DISTANCE)
            {
                Utility.PushColor(ConsoleColor.Yellow);
                Console.WriteLine($"[MageCombatAI] {m_Sidekick.Name} - Enemy too close ({distance} tiles) during combo, kiting!");
                Utility.PopColor();
                RunFrom(enemy);
            }

            switch (m_CurrentCombo)
            {
                case CombatCombo.FullDump:
                    return ContinueFullDump(enemy, distance);

                case CombatCombo.ExplosionEnergyBolt:
                    if (m_WaitingForEnergyBolt)
                    {
                        if (DateTime.UtcNow >= m_NextAllowedCast)
                        {
                            if (CastEnergyBoltForCombo(enemy))
                            {
                                Utility.PushColor(ConsoleColor.Green);
                                Console.WriteLine($"[MageCombatAI] {m_Sidekick.Name} - COMBO COMPLETE! Explosion + Energy Bolt on {enemy.Name}");
                                Utility.PopColor();

                                m_CurrentCombo = CombatCombo.None;
                                m_ComboTarget = null;
                                m_WaitingForEnergyBolt = false;
                                m_NextAllowedCast = DateTime.UtcNow + TimeSpan.FromSeconds(2.0);
                                return true;
                            }
                            else
                            {
                                return true; // Keep trying
                            }
                        }
                        else
                        {
                            double remaining = (m_NextAllowedCast - DateTime.UtcNow).TotalSeconds;
                            Utility.PushColor(ConsoleColor.Cyan);
                            Console.WriteLine($"[MageCombatAI] {m_Sidekick.Name} - Waiting {remaining:F1}s for Energy Bolt at {distance} tiles");
                            Utility.PopColor();
                            return true;
                        }
                    }
                    break;

                case CombatCombo.ParalyzeCombo:
                    return ContinueParalyzeCombo(enemy, distance);

                case CombatCombo.FinisherCombo:
                    if (DateTime.UtcNow >= m_NextAllowedCast && m_Sidekick.Mana >= 20)
                    {
                        if (CastEnergyBolt(enemy))
                        {
                            m_LastEnergyBoltCast = DateTime.UtcNow;
                            m_NextAllowedCast = DateTime.UtcNow + TimeSpan.FromSeconds(2.0);
                            m_ConsecutiveRetreatFailures = 0;
                            return true;
                        }
                    }
                    else
                    {
                        m_CurrentCombo = CombatCombo.None;
                        m_ComboTarget = null;
                    }
                    break;
            }

            return false;
        }

        private bool ContinueFullDump(Mobile enemy, int distance)
        {
            // Full dump: Explosion (cast) -> Poison -> Energy Bolt

            // KITE WHILE COMBOING: If enemy is too close, retreat!
            if (distance <= MIN_RETREAT_DISTANCE)
            {
                RunFrom(enemy);
            }

            // Step 1: Waiting to cast Poison
            if (m_WaitingForPoison && m_ComboStep == 1)
            {
                if (DateTime.UtcNow >= m_NextAllowedCast)
                {
                    // Double-check poison immunity
                    if (IsPoisonImmune(enemy))
                    {
                        Utility.PushColor(ConsoleColor.Yellow);
                        Console.WriteLine($"[MageCombatAI] {m_Sidekick.Name} - Target is poison immune, skipping Poison step");
                        Utility.PopColor();
                        
                        m_WaitingForPoison = false;
                        m_WaitingForEnergyBolt = true;
                        m_ComboStep = 2;
                        m_NextAllowedCast = DateTime.UtcNow + TimeSpan.FromSeconds(0.5);
                        return true;
                    }
                    
                    if (CastPoison(enemy))
                    {
                        m_WaitingForPoison = false;
                        m_WaitingForEnergyBolt = true;
                        m_ComboStep = 2;
                        m_NextAllowedCast = DateTime.UtcNow + TimeSpan.FromSeconds(1.5);
                        return true;
                    }
                    else
                    {
                        // Poison failed, skip to Energy Bolt
                        Utility.PushColor(ConsoleColor.Red);
                        Console.WriteLine($"[MageCombatAI] {m_Sidekick.Name} - POISON cast failed, skipping to Energy Bolt");
                        Utility.PopColor();
                        m_WaitingForPoison = false;
                        m_WaitingForEnergyBolt = true;
                        m_ComboStep = 2;
                    }
                }
                return true;
            }

            // Step 2: Waiting to cast Energy Bolt
            if (m_WaitingForEnergyBolt && m_ComboStep == 2)
            {
                if (DateTime.UtcNow >= m_NextAllowedCast)
                {
                    if (CastEnergyBoltForCombo(enemy))
                    {
                        Utility.PushColor(ConsoleColor.Yellow);
                        Console.WriteLine($"[MageCombatAI] {m_Sidekick.Name} - *** FULL DUMP COMPLETE ***");
                        Utility.PopColor();

                        m_CurrentCombo = CombatCombo.None;
                        m_ComboTarget = null;
                        m_WaitingForEnergyBolt = false;
                        m_NextAllowedCast = DateTime.UtcNow + TimeSpan.FromSeconds(2.0);
                        return true;
                    }
                }
                return true;
            }

            return false;
        }

        private bool ContinueParalyzeCombo(Mobile enemy, int distance)
        {
            // KITE WHILE COMBOING: If enemy is too close, retreat!
            if (distance <= MIN_RETREAT_DISTANCE)
            {
                RunFrom(enemy);
            }

            // After Paralyze, follow up with Explosion -> Energy Bolt
            if (m_ComboStep == 1 && DateTime.UtcNow >= m_NextAllowedCast)
            {
                if (CastExplosionOnly(enemy))
                {
                    m_LastExplosionCast = DateTime.UtcNow;
                    m_WaitingForEnergyBolt = true;
                    m_ComboStep = 2;
                    m_NextAllowedCast = DateTime.UtcNow + TimeSpan.FromSeconds(2.5);
                    return true;
                }
            }

            if (m_ComboStep == 2 && m_WaitingForEnergyBolt)
            {
                if (DateTime.UtcNow >= m_NextAllowedCast)
                {
                    if (CastEnergyBoltForCombo(enemy))
                    {
                        Utility.PushColor(ConsoleColor.Yellow);
                        Console.WriteLine($"[MageCombatAI] {m_Sidekick.Name} - *** PARALYZE COMBO COMPLETE ***");
                        Utility.PopColor();

                        m_CurrentCombo = CombatCombo.None;
                        m_ComboTarget = null;
                        m_WaitingForEnergyBolt = false;
                        m_NextAllowedCast = DateTime.UtcNow + TimeSpan.FromSeconds(2.0);
                        return true;
                    }
                }
                return true;
            }

            return true;
        }

        /// <summary>
        /// Handle positioning to maintain optimal range
        /// </summary>
        private bool HandlePositioning(Mobile enemy, int distance)
        {
            if (m_CurrentCombo != CombatCombo.None && m_ComboTarget == enemy)
            {
                return ContinueCombo(enemy, distance);
            }

            bool isStuck = CheckIfStuck();

            // Apply distance hysteresis to prevent rapid oscillation
            bool shouldReactToDistance = ShouldReactToDistanceChange(distance);

            // If stuck and at reasonable range, try casting
            if (isStuck && distance >= 8 && distance <= m_MaxCastRange &&
                m_Sidekick.Mana >= 20 && DateTime.UtcNow >= m_NextAllowedCast)
            {
                Utility.PushColor(ConsoleColor.Green);
                Console.WriteLine($"[MageCombatAI] {m_Sidekick.Name} - STUCK at {distance} tiles, switching to offensive mode");
                Utility.PopColor();

                if (m_Sidekick.Map != null && m_Sidekick.InLOS(enemy) && m_CurrentCombo == CombatCombo.None)
                {
                    CombatCombo selectedCombo = SelectCombo(enemy);
                    if (selectedCombo != CombatCombo.None)
                    {
                        m_CurrentCombo = selectedCombo;
                        m_ComboTarget = enemy;
                        return StartCombo(enemy, distance);
                    }
                }
            }

            // Dynamic kiting: If too close, retreat (always react to critical distance)
            if (distance <= m_MinSafeRange && !isStuck)
            {
                int targetDistance = CalculateRetreatDistance(distance);
                Utility.PushColor(ConsoleColor.Yellow);
                Console.WriteLine($"[MageCombatAI] {m_Sidekick.Name} - Too close ({distance} <= {m_MinSafeRange}), retreating to {targetDistance} tiles");
                Utility.PopColor();

                // Try corner waypoint
                Point3D corner = FindNearestCorner(enemy);
                if (corner != Point3D.Zero)
                {
                    int distanceToCorner = (int)m_Sidekick.GetDistanceToSqrt(corner);
                    
                    if (distanceToCorner >= 1 && distanceToCorner <= 4)
                    {
                        Utility.PushColor(ConsoleColor.Cyan);
                        Console.WriteLine($"[MageCombatAI] {m_Sidekick.Name} - Using corner waypoint at {corner}");
                        Utility.PopColor();

                        Point3D positionBeforeRetreat = m_Sidekick.Location;
                        
                        if (m_Sidekick.AIObject is SidekickAI sidekickAI)
                        {
                            sidekickAI.RunFrom(enemy, corner);
                        }
                        else
                        {
                            Direction dirToCorner = m_Sidekick.GetDirectionTo(corner);
                            m_Sidekick.Direction = dirToCorner | Direction.Running;
                            if (m_Sidekick.CanMove && !m_Sidekick.Frozen && !m_Sidekick.Paralyzed)
                            {
                                m_Sidekick.Move(dirToCorner | Direction.Running);
                            }
                        }
                        
                        if (m_Sidekick.Location != positionBeforeRetreat)
                        {
                            m_ConsecutiveRetreatFailures = 0;
                            m_LastPosition = m_Sidekick.Location;
                            m_LastPositionCheck = DateTime.UtcNow;
                        }
                        else
                        {
                            m_ConsecutiveRetreatFailures++;
                        }
                        
                        return true;
                    }
                }

                // Try directional juking
                Point3D positionBeforeJuke = m_Sidekick.Location;
                if (TryDirectionalJuke(enemy))
                {
                    if (m_Sidekick.Location != positionBeforeJuke)
                    {
                        m_ConsecutiveRetreatFailures = 0;
                        m_LastPosition = m_Sidekick.Location;
                        m_LastPositionCheck = DateTime.UtcNow;
                    }
                    return true;
                }

                // Fall back to RunFrom
                Point3D positionBeforeRetreat2 = m_Sidekick.Location;
                RunFrom(enemy);
                
                if (m_Sidekick.Location == positionBeforeRetreat2)
                {
                    m_ConsecutiveRetreatFailures++;
                    
                    // If retreat failed and we're at reasonable range, try casting
                    if (distance >= 8 && distance <= m_MaxCastRange &&
                        m_Sidekick.Mana >= 20 && DateTime.UtcNow >= m_NextAllowedCast &&
                        m_Sidekick.Map != null && m_Sidekick.InLOS(enemy))
                    {
                        CombatCombo selectedCombo = SelectCombo(enemy);
                        if (selectedCombo != CombatCombo.None)
                        {
                            m_CurrentCombo = selectedCombo;
                            m_ComboTarget = enemy;
                            return StartCombo(enemy, distance);
                        }
                    }
                }
                else
                {
                    m_ConsecutiveRetreatFailures = 0;
                    m_LastPosition = m_Sidekick.Location;
                    m_LastPositionCheck = DateTime.UtcNow;
                }
                
                return true;
            }

            // If too far, move closer - but only if within give-up distance and hysteresis allows
            if (distance > m_OptimalCastRange && shouldReactToDistance)
            {
                // HOLD POSITION if enemy is beyond chase threshold - stay in combat but don't chase
                if (distance > GIVE_UP_CHASE_DISTANCE)
                {
                    Utility.PushColor(ConsoleColor.Yellow);
                    Console.WriteLine($"[MageCombatAI] {m_Sidekick.Name} - Enemy at {distance} tiles - HOLDING POSITION (beyond chase distance {GIVE_UP_CHASE_DISTANCE})");
                    Utility.PopColor();

                    // Meditate/recover while waiting if possible
                    if (m_Sidekick.Mana < m_Sidekick.ManaMax && !m_Sidekick.Meditating &&
                        m_Sidekick.Skills[SkillName.Meditation].Value > 0)
                    {
                        try { m_Sidekick.UseSkill(SkillName.Meditation); } catch { }
                    }

                    // Use bandage if hurt while waiting
                    if (m_Sidekick.Hits < m_Sidekick.HitsMax && !m_Sidekick.Poisoned)
                    {
                        TryUseBandage();
                    }

                    return true; // Stay in combat, just don't chase
                }

                double enemyHealthPct = (double)enemy.Hits / (double)enemy.HitsMax;
                int targetRange = (enemyHealthPct < ENEMY_LOW_HEALTH) ? CHASE_RANGE : m_OptimalCastRange;

                Utility.PushColor(ConsoleColor.Cyan);
                if (targetRange == CHASE_RANGE)
                    Console.WriteLine($"[MageCombatAI] {m_Sidekick.Name} - CHASE MODE: Enemy at {enemyHealthPct:P0} health, closing to {targetRange} tiles");
                else
                    Console.WriteLine($"[MageCombatAI] {m_Sidekick.Name} - Too far ({distance}), moving to optimal range");
                Utility.PopColor();

                return MoveTo(enemy, targetRange);
            }

            // Enemy closing in and we're on cooldown - retreat
            if (distance <= m_MinSafeRange + 2 && DateTime.UtcNow < m_NextAllowedCast)
            {
                double enemyHealthPct = (double)enemy.Hits / (double)enemy.HitsMax;
                
                // If enemy is low health, don't retreat
                if (enemyHealthPct < ENEMY_LOW_HEALTH)
                {
                    Utility.PushColor(ConsoleColor.Green);
                    Console.WriteLine($"[MageCombatAI] {m_Sidekick.Name} - Enemy at {enemyHealthPct:P0} health - standing ground to finish!");
                    Utility.PopColor();
                    return true;
                }
                
                Utility.PushColor(ConsoleColor.Yellow);
                Console.WriteLine($"[MageCombatAI] {m_Sidekick.Name} - Enemy closing in ({distance} tiles), on cooldown - retreating");
                Utility.PopColor();

                RunFrom(enemy);
                return true;
            }

            // Low mana fallbacks
            if (m_Sidekick.Mana < 20)
            {
                Utility.PushColor(ConsoleColor.Yellow);
                Console.WriteLine($"[MageCombatAI] {m_Sidekick.Name} - Low mana ({m_Sidekick.Mana}), looking for fallback actions");
                Utility.PopColor();
                
                // Teleport escape if stuck with low mana
                if (isStuck && m_Sidekick.Mana >= 9 && 
                    DateTime.UtcNow >= m_LastTeleportTime.AddSeconds(TELEPORT_COOLDOWN_SECONDS) &&
                    DateTime.UtcNow >= m_NextAllowedCast)
                {
                    if (TryTeleportEscape(enemy))
                    {
                        m_ConsecutiveStuckTicks = 0;
                        m_ConsecutiveRetreatFailures = 0;
                        return true;
                    }
                }
                
                // Bandages to stay alive
                if (m_Sidekick.Hits < m_Sidekick.HitsMax && !m_Sidekick.Poisoned)
                {
                    if (TryUseBandage())
                    {
                        Utility.PushColor(ConsoleColor.Green);
                        Console.WriteLine($"[MageCombatAI] {m_Sidekick.Name} - Low mana, using bandage to stay alive");
                        Utility.PopColor();
                        return true;
                    }
                }
                
                // Retreat if stuck with low mana
                if (isStuck && distance <= m_MinSafeRange + 3)
                {
                    RunFrom(enemy);
                    return true;
                }
            }
            
            return true;
        }

        #region Spell Casting Methods

        private bool CastExplosionOnly(Mobile enemy)
        {
            if (enemy == null || enemy.Deleted || m_Sidekick.Mana < LOW_MANA)
                return false;

            if (m_Sidekick.Map == null || !m_Sidekick.InLOS(enemy))
            {
                Utility.PushColor(ConsoleColor.Yellow);
                Console.WriteLine($"[MageCombatAI] {m_Sidekick.Name} - Cannot cast EXPLOSION on {enemy.Name} - no line of sight");
                Utility.PopColor();
                return false;
            }

            // Check spell cooldown (NextSpellTime)
            if (Core.TickCount - m_Sidekick.NextSpellTime < 0)
            {
                long waitMs = m_Sidekick.NextSpellTime - Core.TickCount;
                Utility.PushColor(ConsoleColor.Yellow);
                Console.WriteLine($"[MageCombatAI] {m_Sidekick.Name} - EXPLOSION blocked: spell cooldown (wait {waitMs}ms)");
                Utility.PopColor();
                return false;
            }

            // Check if already casting another spell
            if (m_Sidekick.Spell != null)
            {
                var existingSpell = m_Sidekick.Spell as Spell;
                if (existingSpell != null)
                {
                    if (existingSpell.State == Server.Spells.SpellState.Casting)
                    {
                        Utility.PushColor(ConsoleColor.Yellow);
                        Console.WriteLine($"[MageCombatAI] {m_Sidekick.Name} - EXPLOSION blocked: already casting {existingSpell.GetType().Name}");
                        Utility.PopColor();
                        return false;
                    }
                    if (existingSpell.State == Server.Spells.SpellState.Sequencing)
                    {
                        existingSpell.FinishSequence();
                    }
                }
            }

            m_ComboTarget = enemy;

            Utility.PushColor(ConsoleColor.Magenta);
            Console.WriteLine($"[MageCombatAI] {m_Sidekick.Name} - Casting EXPLOSION on {enemy.Name} (combo mode)");
            Utility.PopColor();

            var spell = new ExplosionSpell(m_Sidekick, null);
            bool started = spell.Cast();

            if (!started)
            {
                Utility.PushColor(ConsoleColor.Red);
                Console.WriteLine($"[MageCombatAI] {m_Sidekick.Name} - EXPLOSION cast failed to start!");
                Utility.PopColor();
                return false;
            }

            return true;
        }

        private bool CastExplosion(Mobile enemy)
        {
            if (enemy == null || enemy.Deleted || m_Sidekick.Mana < LOW_MANA)
                return false;

            if (m_Sidekick.Map == null || !m_Sidekick.InLOS(enemy))
            {
                return false;
            }

            Utility.PushColor(ConsoleColor.Magenta);
            Console.WriteLine($"[MageCombatAI] {m_Sidekick.Name} - Casting EXPLOSION on {enemy.Name}");
            Utility.PopColor();

            var spell = new ExplosionSpell(m_Sidekick, null);
            spell.Cast();

            if (m_Sidekick.Target != null)
            {
                m_Sidekick.Target.Invoke(m_Sidekick, enemy);
            }

            return true;
        }

        private bool CastEnergyBolt(Mobile enemy)
        {
            if (enemy == null || enemy.Deleted || m_Sidekick.Mana < LOW_MANA)
                return false;

            if (Core.TickCount - m_Sidekick.NextSpellTime < 0)
            {
                return false;
            }

            if (m_Sidekick.Map == null || !m_Sidekick.InLOS(enemy))
            {
                Utility.PushColor(ConsoleColor.Yellow);
                Console.WriteLine($"[MageCombatAI] {m_Sidekick.Name} - Cannot cast ENERGY BOLT on {enemy.Name} - no line of sight");
                Utility.PopColor();
                return false;
            }

            // Clear spells in Sequencing state
            if (m_Sidekick.Spell != null)
            {
                var existingSpell = m_Sidekick.Spell as Spell;
                if (existingSpell != null)
                {
                    if (existingSpell.State == Server.Spells.SpellState.Sequencing)
                    {
                        existingSpell.FinishSequence();
                    }
                    else if (existingSpell.State == Server.Spells.SpellState.Casting)
                    {
                        return false; // Wait for it to complete
                    }
                }
            }

            Utility.PushColor(ConsoleColor.Magenta);
            Console.WriteLine($"[MageCombatAI] {m_Sidekick.Name} - Casting ENERGY BOLT on {enemy.Name}");
            Utility.PopColor();

            var spell = new EnergyBoltSpell(m_Sidekick, null);
            bool castStarted = spell.Cast();

            if (!castStarted || m_Sidekick.Spell != spell)
            {
                return false;
            }

            if (m_Sidekick.Target != null)
            {
                m_Sidekick.Target.Invoke(m_Sidekick, enemy);
            }

            return true;
        }

        private bool CastEnergyBoltForCombo(Mobile enemy)
        {
            if (enemy == null || enemy.Deleted || !enemy.Alive)
                return false;

            if (m_Sidekick.Mana < 11)
                return false;

            if (Core.TickCount - m_Sidekick.NextSpellTime < 0)
            {
                return false;
            }

            if (m_Sidekick.Spell != null)
            {
                var existingSpell = m_Sidekick.Spell as Spell;
                if (existingSpell != null)
                {
                    if (existingSpell.State == Server.Spells.SpellState.Casting)
                        return false;
                    if (existingSpell.State == Server.Spells.SpellState.Sequencing)
                        existingSpell.FinishSequence();
                }
            }

            Utility.PushColor(ConsoleColor.Magenta);
            Console.WriteLine($"[MageCombatAI] {m_Sidekick.Name} - COMBO: Energy Bolt on {enemy.Name}");
            Utility.PopColor();

            var spell = new EnergyBoltSpell(m_Sidekick, null);
            if (!spell.Cast())
                return false;

            if (m_Sidekick.Target != null)
                m_Sidekick.Target.Invoke(m_Sidekick, enemy);

            return true;
        }

        private bool CanCastInvisibility()
        {
            if ((DateTime.UtcNow - m_LastInvisTime).TotalSeconds < 5.0)
                return false;

            if (m_Sidekick.Mana < 9)
                return false;

            if (DateTime.UtcNow < m_NextAllowedCast)
                return false;

            return true;
        }

        private bool CastInvisibility()
        {
            if (!CanCastInvisibility())
                return false;

            Utility.PushColor(ConsoleColor.Magenta);
            Console.WriteLine($"[MageCombatAI] {m_Sidekick.Name} - Going INVISIBLE to meditate");
            Utility.PopColor();

            var spell = new InvisibilitySpell(m_Sidekick, null);
            spell.Cast();

            m_LastInvisTime = DateTime.UtcNow;
            m_NextAllowedCast = DateTime.UtcNow + TimeSpan.FromSeconds(2.0);

            Timer.DelayCall(TimeSpan.FromSeconds(1.0), () =>
            {
                if (!m_Sidekick.Deleted && m_Sidekick.Alive && m_Sidekick.Hidden)
                {
                    if (m_Sidekick.Skills[SkillName.Meditation].Value > 0)
                    {
                        try
                        {
                            m_Sidekick.UseSkill(SkillName.Meditation);
                        }
                        catch { }
                    }
                }
            });

            return true;
        }

        #endregion

        #region Teleport Escape

        private bool TryTeleportEscape(Mobile enemy)
        {
            if (enemy == null || enemy.Deleted || m_Sidekick.Map == null)
                return false;

            Map map = m_Sidekick.Map;
            Point3D currentLoc = m_Sidekick.Location;
            Point3D enemyLoc = enemy.Location;

            int dx = currentLoc.X - enemyLoc.X;
            int dy = currentLoc.Y - enemyLoc.Y;

            double length = Math.Sqrt(dx * dx + dy * dy);
            if (length < 1) length = 1;
            double ndx = dx / length;
            double ndy = dy / length;

            Point3D bestLocation = Point3D.Zero;
            int bestDistance = 0;

            for (int targetDist = 12; targetDist >= 8; targetDist--)
            {
                int targetX = currentLoc.X + (int)(ndx * targetDist);
                int targetY = currentLoc.Y + (int)(ndy * targetDist);

                Point3D testLoc = new Point3D(targetX, targetY, currentLoc.Z);

                if (IsValidTeleportLocation(testLoc, map))
                {
                    bestLocation = testLoc;
                    bestDistance = targetDist;
                    break;
                }

                // Try offset angles
                for (int angle = 30; angle <= 90; angle += 30)
                {
                    double radians = angle * Math.PI / 180.0;

                    double leftX = ndx * Math.Cos(radians) - ndy * Math.Sin(radians);
                    double leftY = ndx * Math.Sin(radians) + ndy * Math.Cos(radians);
                    testLoc = new Point3D(currentLoc.X + (int)(leftX * targetDist), currentLoc.Y + (int)(leftY * targetDist), currentLoc.Z);
                    if (IsValidTeleportLocation(testLoc, map))
                    {
                        bestLocation = testLoc;
                        bestDistance = targetDist;
                        break;
                    }

                    double rightX = ndx * Math.Cos(-radians) - ndy * Math.Sin(-radians);
                    double rightY = ndx * Math.Sin(-radians) + ndy * Math.Cos(-radians);
                    testLoc = new Point3D(currentLoc.X + (int)(rightX * targetDist), currentLoc.Y + (int)(rightY * targetDist), currentLoc.Z);
                    if (IsValidTeleportLocation(testLoc, map))
                    {
                        bestLocation = testLoc;
                        bestDistance = targetDist;
                        break;
                    }
                }

                if (bestLocation != Point3D.Zero)
                    break;
            }

            if (bestLocation == Point3D.Zero)
            {
                return false;
            }

            m_PendingTeleportLocation = bestLocation;
            m_PendingTeleportDistance = bestDistance;

            Utility.PushColor(ConsoleColor.Cyan);
            Console.WriteLine($"[MageCombatAI] {m_Sidekick.Name} - Casting TELEPORT to escape ({bestDistance} tiles away)");
            Utility.PopColor();

            var teleportSpell = new TeleportSpell(m_Sidekick, null);
            teleportSpell.Cast();

            m_LastTeleportTime = DateTime.UtcNow;
            m_NextAllowedCast = DateTime.UtcNow + TimeSpan.FromSeconds(2.0);

            return true;
        }

        private Point3D m_PendingTeleportLocation = Point3D.Zero;
        private int m_PendingTeleportDistance = 0;

        public bool ProcessTeleportTarget()
        {
            if (m_PendingTeleportLocation == Point3D.Zero)
                return false;

            if (m_Sidekick.Target != null && m_Sidekick.Target is TeleportSpell.InternalTarget)
            {
                var target = m_Sidekick.Target;
                var landTarget = new LandTarget(m_PendingTeleportLocation, m_Sidekick.Map);

                target.Invoke(m_Sidekick, landTarget);

                Utility.PushColor(ConsoleColor.Cyan);
                Console.WriteLine($"[MageCombatAI] {m_Sidekick.Name} - TELEPORT ESCAPE SUCCESS!");
                Utility.PopColor();

                m_PendingTeleportLocation = Point3D.Zero;
                m_PendingTeleportDistance = 0;

                return true;
            }

            return false;
        }

        private bool IsValidTeleportLocation(Point3D loc, Map map)
        {
            if (map == null)
                return false;

            int z = loc.Z;
            LandTile landTile = map.Tiles.GetLandTile(loc.X, loc.Y);
            int landZ = landTile.Z;

            if (Math.Abs(z - landZ) > 10)
                z = landZ;

            loc = new Point3D(loc.X, loc.Y, z);

            if (!map.CanSpawnMobile(loc.X, loc.Y, loc.Z))
                return false;

            if (SpellHelper.CheckMulti(loc, map))
                return false;

            if (Region.Find(loc, map).GetRegion(typeof(Server.Regions.HouseRegion)) != null)
                return false;

            return true;
        }

        #endregion

        #region Movement Methods

        private void RunFrom(Mobile enemy)
        {
            if (enemy == null || enemy.Deleted)
                return;

            if (m_Sidekick.AIObject is SidekickAI sidekickAI)
            {
                sidekickAI.RunFrom(enemy);
            }
            else
            {
                Direction awayDir = (Direction)(((int)m_Sidekick.GetDirectionTo(enemy) - 4) & (int)Direction.Mask);
                Direction runDir = awayDir | Direction.Running;

                m_Sidekick.Direction = runDir;

                if (m_Sidekick.CanMove && !m_Sidekick.Frozen && !m_Sidekick.Paralyzed)
                {
                    m_Sidekick.Move(runDir);
                }
            }
        }

        private bool MoveTo(Mobile enemy, int targetRange)
        {
            if (enemy == null || enemy.Deleted)
                return false;

            int currentDistance = (int)m_Sidekick.GetDistanceToSqrt(enemy);
            
            if (currentDistance <= targetRange && currentDistance >= MIN_CAST_RANGE)
                return true;

            double enemyHealthPercent = (double)enemy.Hits / (double)enemy.HitsMax;
            bool aggressivePursuit = enemyHealthPercent < 0.30;

            Direction towardDir = m_Sidekick.GetDirectionTo(enemy);
            
            if (aggressivePursuit || currentDistance > targetRange + 2)
            {
                towardDir |= Direction.Running;
            }
            
            m_Sidekick.Direction = towardDir;

            if (m_Sidekick.CanMove && !m_Sidekick.Frozen && !m_Sidekick.Paralyzed)
            {
                int stepsToMove = 1;
                if (aggressivePursuit)
                {
                    stepsToMove = Math.Min(3, (currentDistance - targetRange) / 2);
                }
                else if (currentDistance > targetRange + 3)
                {
                    stepsToMove = 2;
                }

                for (int i = 0; i < Math.Max(1, stepsToMove); i++)
                {
                    if (m_Sidekick.Move(towardDir))
                    {
                        int newDistance = (int)m_Sidekick.GetDistanceToSqrt(enemy);
                        if (newDistance <= targetRange && newDistance >= MIN_CAST_RANGE)
                            return true;
                    }
                    else
                    {
                        break;
                    }
                }
                
                return true;
            }

            return false;
        }

        #endregion

        #region Corner Detection and Kiting

        private Point3D FindNearestCorner(Mobile enemy)
        {
            if (enemy == null || enemy.Deleted || m_Sidekick.Map == null)
                return Point3D.Zero;

            if ((DateTime.UtcNow - m_LastCornerCheck).TotalMilliseconds < CORNER_CHECK_INTERVAL && m_LastCornerTarget != Point3D.Zero)
            {
                if (IsEscapeWaypoint(m_LastCornerTarget, enemy))
                {
                    return m_LastCornerTarget;
                }
            }

            m_LastCornerCheck = DateTime.UtcNow;
            Point3D bestCorner = Point3D.Zero;
            int bestScore = 0;
            Point3D currentPos = m_Sidekick.Location;
            Direction retreatDir = (Direction)(((int)m_Sidekick.GetDirectionTo(enemy) - 4) & (int)Direction.Mask);

            int[] directionPriority = new int[] { 0, 2, 6, 1, 7, 3, 5, 4 };
            
            int currentDistanceToEnemy = (int)m_Sidekick.GetDistanceToSqrt(enemy);
            int minSearchDistance = Math.Max(10, currentDistanceToEnemy + 5);
            int maxSearchDistance = Math.Min(20, currentDistanceToEnemy + 10);
            
            for (int priority = 0; priority < directionPriority.Length; priority++)
            {
                int dirOffset = directionPriority[priority];
                Direction testDir = (Direction)(((int)retreatDir + dirOffset) & (int)Direction.Mask);

                for (int dist = minSearchDistance; dist <= maxSearchDistance; dist++)
                {
                    Point3D testPoint = CalculatePointInDirection(currentPos, testDir, dist);

                    if (IsEscapeWaypoint(testPoint, enemy))
                    {
                        int distanceFromEnemy = (int)Math.Sqrt(Math.Pow(testPoint.X - enemy.X, 2) + Math.Pow(testPoint.Y - enemy.Y, 2));
                        int distanceFromCurrent = (int)Math.Sqrt(Math.Pow(testPoint.X - currentPos.X, 2) + Math.Pow(testPoint.Y - currentPos.Y, 2));

                        int score = distanceFromEnemy * 5;
                        score -= distanceFromCurrent * 3;
                        
                        if (dirOffset == 0)
                            score += 20;
                        else if (dirOffset == 2 || dirOffset == 6)
                            score += 15;
                        else if (dirOffset == 1 || dirOffset == 7)
                            score += 10;

                        if (score > bestScore)
                        {
                            bestScore = score;
                            bestCorner = testPoint;
                        }
                    }
                }
            }

            m_LastCornerTarget = bestCorner;

            return bestCorner;
        }

        private bool IsEscapeWaypoint(Point3D location, Mobile enemy)
        {
            if (enemy == null || enemy.Deleted || m_Sidekick.Map == null)
                return false;

            if (!m_Sidekick.Map.CanFit(location.X, location.Y, location.Z, 16, false, true, true))
                return false;

            int distanceFromEnemy = (int)Math.Sqrt(Math.Pow(location.X - enemy.X, 2) + Math.Pow(location.Y - enemy.Y, 2));
            int currentDistanceFromEnemy = (int)m_Sidekick.GetDistanceToSqrt(enemy);
            
            if (distanceFromEnemy < currentDistanceFromEnemy - 1)
                return false;

            return true;
        }

        private Point3D CalculatePointInDirection(Point3D start, Direction dir, int distance)
        {
            int x = start.X;
            int y = start.Y;
            int z = start.Z;

            Server.Movement.Movement.Offset(dir, ref x, ref y);

            if (distance > 1)
            {
                for (int i = 1; i < distance; i++)
                {
                    Server.Movement.Movement.Offset(dir, ref x, ref y);
                }
            }

            return new Point3D(x, y, z);
        }

        private bool TryDirectionalJuke(Mobile enemy)
        {
            if (enemy == null || enemy.Deleted || m_Sidekick.Map == null)
                return false;

            if ((DateTime.UtcNow - m_LastDirectionChange).TotalMilliseconds < JUKING_COOLDOWN)
                return false;

            Point3D corner = FindNearestCorner(enemy);
            if (corner != Point3D.Zero)
            {
                return false;
            }

            Direction currentRetreatDir = (Direction)(((int)m_Sidekick.GetDirectionTo(enemy) - 4) & (int)Direction.Mask);
            
            if (currentRetreatDir == m_LastRetreatDirection)
            {
                m_ConsecutiveSameDirection++;
            }
            else
            {
                m_ConsecutiveSameDirection = 0;
                m_LastRetreatDirection = currentRetreatDir;
            }

            if (m_ConsecutiveSameDirection >= JUKING_THRESHOLD)
            {
                Direction reverseDir = (Direction)(((int)currentRetreatDir + 4) & (int)Direction.Mask);
                
                Direction leftPerp = (Direction)(((int)currentRetreatDir - 2) & (int)Direction.Mask);
                Direction rightPerp = (Direction)(((int)currentRetreatDir + 2) & (int)Direction.Mask);

                Point3D leftPoint = CalculatePointInDirection(m_Sidekick.Location, leftPerp, 1);
                Point3D rightPoint = CalculatePointInDirection(m_Sidekick.Location, rightPerp, 1);

                if (m_Sidekick.Map.CanFit(leftPoint.X, leftPoint.Y, leftPoint.Z, 16, false, true, true))
                {
                    m_Sidekick.Direction = leftPerp | Direction.Running;
                    if (m_Sidekick.CanMove && !m_Sidekick.Frozen && !m_Sidekick.Paralyzed)
                    {
                        m_Sidekick.Move(leftPerp | Direction.Running);
                        m_ConsecutiveSameDirection = 0;
                        m_LastDirectionChange = DateTime.UtcNow;
                        m_LastRetreatDirection = leftPerp;
                        return true;
                    }
                }
                else if (m_Sidekick.Map.CanFit(rightPoint.X, rightPoint.Y, rightPoint.Z, 16, false, true, true))
                {
                    m_Sidekick.Direction = rightPerp | Direction.Running;
                    if (m_Sidekick.CanMove && !m_Sidekick.Frozen && !m_Sidekick.Paralyzed)
                    {
                        m_Sidekick.Move(rightPerp | Direction.Running);
                        m_ConsecutiveSameDirection = 0;
                        m_LastDirectionChange = DateTime.UtcNow;
                        m_LastRetreatDirection = rightPerp;
                        return true;
                    }
                }
                else
                {
                    m_Sidekick.Direction = reverseDir | Direction.Running;
                    if (m_Sidekick.CanMove && !m_Sidekick.Frozen && !m_Sidekick.Paralyzed)
                    {
                        m_Sidekick.Move(reverseDir | Direction.Running);
                        m_ConsecutiveSameDirection = 0;
                        m_LastDirectionChange = DateTime.UtcNow;
                        m_LastRetreatDirection = reverseDir;
                        return true;
                    }
                }
            }

            return false;
        }

        #endregion

        #region Potions and Bandages

        public bool TryUseBandagePublic()
        {
            return TryUseBandage();
        }

        private bool TryUseBandage()
        {
            if (m_Sidekick.Hits >= m_Sidekick.HitsMax)
            {
                return false;
            }
            
            double cooldownRemaining = 10.0 - (DateTime.UtcNow - m_LastBandageTime).TotalSeconds;
            if (cooldownRemaining > 0)
            {
                return false;
            }

            Container pack = m_Sidekick.Backpack;
            if (pack == null)
            {
                return false;
            }

            Item bandage = pack.FindItemByType(typeof(Server.Items.Bandage));
            if (bandage == null || bandage.Amount < 1)
            {
                return false;
            }

            try
            {
                var bandageContext = Server.Items.BandageContext.GetContext(m_Sidekick);

                if (bandageContext != null)
                {
                    return false;
                }

                Server.Items.BandageContext.BeginHeal(m_Sidekick, m_Sidekick);
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
            if (m_Sidekick.Hits >= m_Sidekick.HitsMax)
                return false;

            if (m_Sidekick.Poisoned)
                return false;

            // Check server's potion cooldown (10 seconds for heal potions)
            if (!m_Sidekick.CanBeginAction(typeof(BaseHealPotion)))
            {
                Utility.PushColor(ConsoleColor.Yellow);
                Console.WriteLine($"[MageCombatAI] {m_Sidekick.Name} - Heal potion on server cooldown");
                Utility.PopColor();
                return false;
            }

            Container pack = m_Sidekick.Backpack;
            if (pack == null)
                return false;

            GreaterHealPotion potion = pack.FindItemByType(typeof(GreaterHealPotion)) as GreaterHealPotion;
            if (potion == null)
                return false;

            try
            {
                if (!m_Sidekick.BeginAction(typeof(BaseHealPotion)))
                    return false;

                potion.DoHeal(m_Sidekick);
                BasePotion.PlayDrinkEffect(m_Sidekick);
                potion.Consume();

                Timer.DelayCall(TimeSpan.FromSeconds(potion.Delay), () => m_Sidekick.EndAction(typeof(BaseHealPotion)));

                Utility.PushColor(ConsoleColor.Magenta);
                Console.WriteLine($"[MageCombatAI] {m_Sidekick.Name} - Used {potion.GetType().Name}!");
                Utility.PopColor();

                return true;
            }
            catch
            {
                return false;
            }
        }

        private bool TryUseCurePotion()
        {
            // Track when we first got poisoned for reaction delay
            if (m_Sidekick.Poisoned && !m_WasPoisoned)
            {
                m_WasPoisoned = true;
                m_PoisonedTime = DateTime.UtcNow;
            }
            else if (!m_Sidekick.Poisoned)
            {
                m_WasPoisoned = false;
                return false;
            }

            // Reaction delay - wait at least 1 second after getting poisoned before curing
            double timeSincePoisoned = (DateTime.UtcNow - m_PoisonedTime).TotalSeconds;
            if (timeSincePoisoned < 1.0)
                return false;

            // Cooldown between cure potion uses
            double cooldownRemaining = CURE_POTION_COOLDOWN - (DateTime.UtcNow - m_LastCurePotionTime).TotalSeconds;
            if (cooldownRemaining > 0)
                return false;

            Container pack = m_Sidekick.Backpack;
            if (pack == null)
                return false;

            GreaterCurePotion potion = pack.FindItemByType(typeof(GreaterCurePotion)) as GreaterCurePotion;
            if (potion == null)
                return false;

            try
            {
                potion.DoCure(m_Sidekick);
                BasePotion.PlayDrinkEffect(m_Sidekick);
                potion.Consume();
                m_LastCurePotionTime = DateTime.UtcNow;

                Utility.PushColor(ConsoleColor.Magenta);
                Console.WriteLine($"[MageCombatAI] {m_Sidekick.Name} - Used {potion.GetType().Name}!");
                Utility.PopColor();

                return true;
            }
            catch
            {
                return false;
            }
        }

        private bool TryUseAgilityPotion()
        {
            double cooldownRemaining = AGILITY_POTION_COOLDOWN - (DateTime.UtcNow - m_LastAgilityPotionTime).TotalSeconds;
            if (cooldownRemaining > 0)
                return false;

            Container pack = m_Sidekick.Backpack;
            if (pack == null)
                return false;

            GreaterAgilityPotion potion = pack.FindItemByType(typeof(GreaterAgilityPotion)) as GreaterAgilityPotion;
            if (potion == null)
                return false;

            try
            {
                if (potion.DoAgility(m_Sidekick))
                {
                    BasePotion.PlayDrinkEffect(m_Sidekick);
                    potion.Consume();
                    m_LastAgilityPotionTime = DateTime.UtcNow;

                    Utility.PushColor(ConsoleColor.Magenta);
                    Console.WriteLine($"[MageCombatAI] {m_Sidekick.Name} - Used {potion.GetType().Name}!");
                    Utility.PopColor();

                    return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        private bool TryUseRefreshPotion()
        {
            double cooldownRemaining = REFRESH_POTION_COOLDOWN - (DateTime.UtcNow - m_LastRefreshPotionTime).TotalSeconds;
            if (cooldownRemaining > 0)
                return false;

            if (m_Sidekick.Stam >= m_Sidekick.StamMax * 0.5)
                return false;

            Container pack = m_Sidekick.Backpack;
            if (pack == null)
                return false;

            TotalRefreshPotion potion = pack.FindItemByType(typeof(TotalRefreshPotion)) as TotalRefreshPotion;
            if (potion == null)
                return false;

            try
            {
                m_Sidekick.Stam += Scale(m_Sidekick, (int)(potion.Refresh * m_Sidekick.StamMax));
                BasePotion.PlayDrinkEffect(m_Sidekick);
                potion.Consume();
                m_LastRefreshPotionTime = DateTime.UtcNow;

                Utility.PushColor(ConsoleColor.Magenta);
                Console.WriteLine($"[MageCombatAI] {m_Sidekick.Name} - Used {potion.GetType().Name}!");
                Utility.PopColor();

                return true;
            }
            catch
            {
                return false;
            }
        }

        private int Scale(Mobile m, int value)
        {
            return value;
        }

        #endregion
    }
}