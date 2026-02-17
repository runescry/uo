using System;
using Server;
using Server.Mobiles;
using Server.Items;
using Server.Spells;
using Server.Spells.Necromancy; // BloodOathSpell, MindRotSpell, etc.
using Server.Spells.Second;  // Cure
using Server.Spells.Third;   // Curse
using Server.Spells.Fourth;  // Greater Heal, Energy Bolt
using Server.Spells.Fifth;   // Poison (optional)
using Server.Spells.Sixth;   // Explosion

namespace Server.Services.AISidekicks
{
    public enum CombatPhase
    {
        Setup,      // Apply debuffs (Corpse Skin, Curse)
        Pressure,   // Sustained damage (Strangle, Pain Spike spam)
        Spike,      // Burst combo (Evil Omen → Explosion + Flamestrike sync)
        Execute,    // Target low, finish with Pain Spike
        Defensive   // Healing/retreating
    }

    public class NecromancerCombatAI
    {
        private AutonomousSidekick m_Sidekick;

        // Combat ranges
        public const int MIN_SAFE_RANGE = 5;
        public const int MAX_CAST_RANGE = 10;
        public const int OPTIMAL_CAST_RANGE = 8;
        public const int MIN_RETREAT_DISTANCE = 4;

        // Health/Mana thresholds
        public const double CRITICAL_HEALTH = 0.35;  // Raised from 0.25 - start healing earlier
        public const double LOW_HEALTH = 0.60;       // Raised from 0.50 - stay in defensive longer
        public const double HEAL_THRESHOLD = 0.70;
        public const double SPIKE_THRESHOLD = 0.90;   // Start spike combo when enemy below this (trigger early for burst damage)
        public const double EXECUTE_THRESHOLD = 0.35; // Switch to execute mode
        public const int LOW_MANA = 30;
        public const int SPIKE_MANA_REQUIRED = 45;    // Mana needed for full dump combo (Evil Omen + Explosion + EBolt)

        // Cooldown timestamps (no bandage - Necromancer lacks Healing skill)
        private DateTime m_LastHealPotionTime = DateTime.MinValue;
        private DateTime m_LastCurePotionTime = DateTime.MinValue;
        private DateTime m_PoisonedTime = DateTime.MinValue; // Track when we got poisoned for reaction delay
        private bool m_WasPoisoned = false; // Track poison state changes
        private DateTime m_LastHealSpellTime = DateTime.MinValue;
        private DateTime m_LastPainSpikeTime = DateTime.MinValue;
        private DateTime m_LastPoisonStrikeTime = DateTime.MinValue;
        private DateTime m_LastStrangleTime = DateTime.MinValue;
        private DateTime m_LastEvilOmenTime = DateTime.MinValue;
        private DateTime m_LastCorpseSkinTime = DateTime.MinValue;
        private DateTime m_LastWitherTime = DateTime.MinValue;
        private DateTime m_LastCurseTime = DateTime.MinValue;
        private DateTime m_LastExplosionTime = DateTime.MinValue;
        private DateTime m_LastEnergyBoltTime = DateTime.MinValue;
        private DateTime m_LastBloodOathTime = DateTime.MinValue;
        private DateTime m_LastMindRotTime = DateTime.MinValue;

        // Cooldown durations (seconds)
        private const double HEAL_POTION_COOLDOWN = 10.0;
        private const double CURE_POTION_COOLDOWN = 1.0;
        private const double HEAL_SPELL_COOLDOWN = 2.0;
        private const double PAIN_SPIKE_COOLDOWN = 1.5;
        private const double POISON_STRIKE_COOLDOWN = 2.5;
        private const double STRANGLE_COOLDOWN = 5.0;
        private const double EVIL_OMEN_COOLDOWN = 3.0;
        private const double CORPSE_SKIN_COOLDOWN = 3.0;
        private const double WITHER_COOLDOWN = 4.0;
        private const double CURSE_COOLDOWN = 3.0;
        private const double EXPLOSION_COOLDOWN = 3.5;
        private const double EXPLOSION_DELAY = 3.0;     // Time for explosion to detonate after cast
        private const double ENERGY_BOLT_COOLDOWN = 1.5;
        private const double BLOOD_OATH_COOLDOWN = 8.0;   // Blood Oath lasts ~8+ seconds
        private const double MIND_ROT_COOLDOWN = 20.0;    // Mind Rot lasts ~20+ seconds

        // Debuff durations (for tracking expiration)
        private const double CORPSE_SKIN_DURATION = 12.0;
        private const double CURSE_DURATION = 30.0;
        private const double EVIL_OMEN_DURATION = 10.0; // Or until consumed
        private const double BLOOD_OATH_DURATION = 8.0;  // Minimum duration
        private const double MIND_ROT_DURATION = 20.0;   // Base duration

        // Damage tracking
        private int m_LastHealth = -1;
        private bool m_TookDamageThisTick = false;

        // Debuff tracking with timestamps (replaces booleans)
        private DateTime m_CorpseSkinAppliedTime = DateTime.MinValue;
        private DateTime m_CurseAppliedTime = DateTime.MinValue;
        private DateTime m_EvilOmenAppliedTime = DateTime.MinValue;
        private DateTime m_BloodOathAppliedTime = DateTime.MinValue;
        private DateTime m_MindRotAppliedTime = DateTime.MinValue;
        private Mobile m_DebuffTarget = null;

        // Combo state machine
        private CombatPhase m_CurrentPhase = CombatPhase.Setup;
        private bool m_ExplosionQueued = false;
        private DateTime m_ExplosionQueuedTime = DateTime.MinValue;

        // Pressure phase combo tracking (Evil Omen + Explosion + Energy Bolt)
        private bool m_WaitingForExplosion = false;
        private DateTime m_ExplosionStartTime = DateTime.MinValue;
        private bool m_WaitingForEnergyBolt = false;
        private DateTime m_EnergyBoltStartTime = DateTime.MinValue;

        public NecromancerCombatAI(AutonomousSidekick sidekick)
        {
            m_Sidekick = sidekick;
            m_LastHealth = sidekick.Hits;
        }

        #region Debuff Status Helpers

        private bool IsCorpseSkinActive()
        {
            return (DateTime.UtcNow - m_CorpseSkinAppliedTime).TotalSeconds < CORPSE_SKIN_DURATION;
        }

        private bool IsCurseActive()
        {
            return (DateTime.UtcNow - m_CurseAppliedTime).TotalSeconds < CURSE_DURATION;
        }

        private bool IsEvilOmenActive()
        {
            // Evil Omen is consumed on next harmful effect, so this is approximate
            return (DateTime.UtcNow - m_EvilOmenAppliedTime).TotalSeconds < EVIL_OMEN_DURATION;
        }

        private bool IsBloodOathActive()
        {
            return (DateTime.UtcNow - m_BloodOathAppliedTime).TotalSeconds < BLOOD_OATH_DURATION;
        }

        private bool IsMindRotActive()
        {
            return (DateTime.UtcNow - m_MindRotAppliedTime).TotalSeconds < MIND_ROT_DURATION;
        }

        /// <summary>
        /// Check if enemy is a caster (mage, necro, or currently casting)
        /// </summary>
        private bool IsEnemyCaster(Mobile enemy)
        {
            if (enemy == null) return false;
            return enemy.Skills.Magery.Value >= 50 ||
                   enemy.Skills.Necromancy.Value >= 50 ||
                   enemy.Spell != null;
        }

        /// <summary>
        /// Check if enemy is primarily a melee fighter (not a caster)
        /// </summary>
        private bool IsEnemyMelee(Mobile enemy)
        {
            if (enemy == null) return false;
            // If they have high combat skills and low magic, they're melee
            double meleeSkill = Math.Max(enemy.Skills.Swords.Value,
                                Math.Max(enemy.Skills.Macing.Value,
                                Math.Max(enemy.Skills.Fencing.Value, enemy.Skills.Wrestling.Value)));
            double magicSkill = Math.Max(enemy.Skills.Magery.Value, enemy.Skills.Necromancy.Value);

            // Melee if: combat skill > 50 AND magic skill < 50, OR it's a creature (most are melee)
            return (meleeSkill > 50 && magicSkill < 50) || (enemy is BaseCreature && magicSkill < 50);
        }

        private bool HasSetupDebuffs()
        {
            return IsCorpseSkinActive() && IsCurseActive();
        }

        #endregion

        #region Phase Determination

        private CombatPhase DeterminePhase(Mobile enemy, double selfHealthPct, double enemyHealthPct)
        {
            // Defensive takes priority - enter at CRITICAL or below, stay until above LOW_HEALTH
            // Also enter defensive if poisoned
            if (selfHealthPct <= CRITICAL_HEALTH || m_Sidekick.Poisoned)
                return CombatPhase.Defensive;

            // Stay in defensive mode until healed above LOW_HEALTH threshold
            if (m_CurrentPhase == CombatPhase.Defensive && selfHealthPct <= LOW_HEALTH)
                return CombatPhase.Defensive;

            // Execute mode - enemy is low, finish them
            if (enemyHealthPct < EXECUTE_THRESHOLD && HasSetupDebuffs())
                return CombatPhase.Execute;

            // Spike mode - enemy below threshold, we have mana, debuffs are up
            if (enemyHealthPct < SPIKE_THRESHOLD &&
                m_Sidekick.Mana >= SPIKE_MANA_REQUIRED &&
                HasSetupDebuffs())
                return CombatPhase.Spike;

            // Setup mode - need to apply debuffs
            if (!HasSetupDebuffs())
                return CombatPhase.Setup;

            // Default to pressure
            return CombatPhase.Pressure;
        }

        #endregion

        public bool DoCombat(Mobile enemy)
        {
            if (m_Sidekick == null || m_Sidekick.Deleted || !m_Sidekick.Alive)
                return false;

            if (enemy == null || enemy.Deleted || !enemy.Alive)
                return false;

            // Track damage taken
            m_TookDamageThisTick = m_LastHealth > 0 && m_Sidekick.Hits < m_LastHealth;
            m_LastHealth = m_Sidekick.Hits;

            double selfHealthPct = (double)m_Sidekick.Hits / m_Sidekick.HitsMax;
            double enemyHealthPct = (double)enemy.Hits / enemy.HitsMax;
            int distance = (int)m_Sidekick.GetDistanceToSqrt(enemy);

            // Reset debuff tracking if target changed to a different mobile
            // IMPORTANT: Don't reset if the "new" target is the sidekick's owner (accidental targeting)
            // or if the target is a player (brief PvP interaction)
            bool shouldResetDebuffs = false;
            if (m_DebuffTarget == null || m_DebuffTarget.Deleted)
            {
                shouldResetDebuffs = true;
            }
            else if (m_DebuffTarget.Serial != enemy.Serial)
            {
                // Only reset if the new target is a monster/NPC, not a player
                // This prevents debuff loss when owner accidentally attacks sidekick
                bool newTargetIsNPC = !(enemy is PlayerMobile);
                bool oldTargetIsNPC = !(m_DebuffTarget is PlayerMobile);

                // Reset if switching between NPCs, but not if switching to/from players
                shouldResetDebuffs = newTargetIsNPC && oldTargetIsNPC;

                if (!shouldResetDebuffs)
                {
                    Log($"Ignoring brief target switch to {enemy.Name} (keeping debuffs on {m_DebuffTarget.Name})", ConsoleColor.Gray);
                }
            }

            if (shouldResetDebuffs)
            {
                Log($"Target changed: {m_DebuffTarget?.Name ?? "null"} -> {enemy.Name}, resetting debuffs", ConsoleColor.Yellow);
                ResetDebuffTracking();
                m_DebuffTarget = enemy;
            }

            // Determine combat phase
            CombatPhase newPhase = DeterminePhase(enemy, selfHealthPct, enemyHealthPct);
            if (newPhase != m_CurrentPhase)
            {
                LogPhaseChange(newPhase);
                m_CurrentPhase = newPhase;
            }

            LogStatus(selfHealthPct, enemyHealthPct, distance);

            // Proactive healing when taking damage (regardless of phase)
            if (m_TookDamageThisTick && selfHealthPct < HEAL_THRESHOLD)
            {
                // Try heal potion first (instant)
                if (selfHealthPct < LOW_HEALTH && TryUseHealPotion())
                {
                    Log("PROACTIVE: Used heal potion after taking damage!", ConsoleColor.Green);
                    return true;
                }

                // Try Greater Heal spell
                if (m_Sidekick.Skills.Magery.Value >= 50 && m_Sidekick.Mana >= 11 &&
                    CooldownReady(m_LastHealSpellTime, HEAL_SPELL_COOLDOWN))
                {
                    Log("PROACTIVE: Casting Greater Heal after taking damage!", ConsoleColor.Green);
                    CastGreaterHeal();
                    return true;
                }
            }

            // Execute phase-specific logic
            switch (m_CurrentPhase)
            {
                case CombatPhase.Defensive:
                    return HandleDefensivePhase(enemy, distance);

                case CombatPhase.Setup:
                    return HandleSetupPhase(enemy, distance);

                case CombatPhase.Pressure:
                    return HandlePressurePhase(enemy, distance);

                case CombatPhase.Spike:
                    return HandleSpikePhase(enemy, distance);

                case CombatPhase.Execute:
                    return HandleExecutePhase(enemy, distance);
            }

            return true;
        }

        #region Phase Handlers

        private bool HandleDefensivePhase(Mobile enemy, int distance)
        {
            double healthPct = (double)m_Sidekick.Hits / m_Sidekick.HitsMax;
            Log($"DEFENSIVE: HP={healthPct:P0}, Mana={m_Sidekick.Mana}, Dist={distance}", ConsoleColor.Red);

            // Cure poison first
            if (m_Sidekick.Poisoned && TryCurePoison())
                return true;

            // Emergency healing - potion first for instant heal
            if (TryUseHealPotion())
            {
                Log("Used heal potion!", ConsoleColor.Green);
                return true;
            }

            // Note: Necromancer has no Healing skill, so bandages won't work
            // Rely on Greater Heal spell and potions instead

            // Cast Greater Heal if we have the skill and mana (with cooldown check)
            if (m_Sidekick.Skills.Magery.Value >= 50 && m_Sidekick.Mana >= 11 &&
                CooldownReady(m_LastHealSpellTime, HEAL_SPELL_COOLDOWN))
            {
                CastGreaterHeal();
                return true;
            }
            else if (!CooldownReady(m_LastHealSpellTime, HEAL_SPELL_COOLDOWN))
            {
                Log($"Greater Heal on cooldown ({(DateTime.UtcNow - m_LastHealSpellTime).TotalSeconds:F1}s)", ConsoleColor.Gray);
                // Try to retreat while waiting for heal cooldown
                if (distance <= MIN_RETREAT_DISTANCE)
                {
                    RunFrom(enemy);
                    return true;
                }
            }
            else
            {
                Log($"Cannot cast Greater Heal: Magery={m_Sidekick.Skills.Magery.Value}, Mana={m_Sidekick.Mana}", ConsoleColor.Yellow);
            }

            // Retreat if enemy close
            if (distance <= MIN_RETREAT_DISTANCE)
            {
                RunFrom(enemy);
                return true;
            }

            return false;
        }

        private bool HandleSetupPhase(Mobile enemy, int distance)
        {
            // Move into range if needed (priority over debuffs)
            if (distance > MAX_CAST_RANGE)
            {
                MoveTo(enemy);
                return true;
            }

            // CAST DEBUFFS FIRST - even if enemy is close, we need these up!
            // Cast while kiting is a core mage skill

            // Apply Corpse Skin first (resist debuff)
            if (!IsCorpseSkinActive() && CanCastCorpseSkin())
            {
                CastCorpseSkin(enemy);
                // Also retreat if too close
                if (distance < MIN_RETREAT_DISTANCE)
                    RunFrom(enemy);
                return true;
            }

            // Then Curse (stat debuff + more resist reduction)
            if (!IsCurseActive() && CanCastCurse())
            {
                CastCurse(enemy);
                if (distance < MIN_RETREAT_DISTANCE)
                    RunFrom(enemy);
                return true;
            }

            // Blood Oath vs melee enemies - reflects damage back
            if (IsEnemyMelee(enemy) && !IsBloodOathActive() && CanCastBloodOath())
            {
                CastBloodOath(enemy);
                if (distance < MIN_RETREAT_DISTANCE)
                    RunFrom(enemy);
                return true;
            }

            // Mind Rot vs caster enemies - doubles their mana costs
            if (IsEnemyCaster(enemy) && !IsMindRotActive() && CanCastMindRot())
            {
                CastMindRot(enemy);
                if (distance < MIN_RETREAT_DISTANCE)
                    RunFrom(enemy);
                return true;
            }

            // All debuffs applied, retreat if needed before transitioning
            if (distance < MIN_RETREAT_DISTANCE)
            {
                RunFrom(enemy);
                return true;
            }

            // Setup complete, phase will change on next tick
            return HandlePressurePhase(enemy, distance);
        }

        private bool HandlePressurePhase(Mobile enemy, int distance)
        {
            if (distance > MAX_CAST_RANGE)
            {
                MoveTo(enemy);
                return true;
            }

            // CAST WHILE KITING - damage is more important than perfect positioning
            bool needsRetreat = distance < MIN_RETREAT_DISTANCE;

            // Refresh debuffs if expiring soon
            if (!IsCorpseSkinActive() && CanCastCorpseSkin())
            {
                CastCorpseSkin(enemy);
                if (needsRetreat) RunFrom(enemy);
                return true;
            }

            // USE MAGE/NECRO HYBRID COMBOS - Evil Omen amplifies next hit!
            // If we have mana and Evil Omen is ready, start a dump combo
            if (m_Sidekick.Mana >= SPIKE_MANA_REQUIRED && CanCastEvilOmen() && CanCastExplosion())
            {
                Log("PRESSURE: Starting Evil Omen + Explosion combo!", ConsoleColor.Yellow);
                CastEvilOmen(enemy);
                m_WaitingForExplosion = true;
                m_ExplosionStartTime = DateTime.UtcNow;
                if (needsRetreat) RunFrom(enemy);
                return true;
            }

            // Continue dump combo if waiting for explosion timing
            if (m_WaitingForExplosion && CanCastExplosion())
            {
                double timeSinceOmen = (DateTime.UtcNow - m_ExplosionStartTime).TotalSeconds;
                if (timeSinceOmen >= 0.3) // Small delay after Evil Omen
                {
                    Log("PRESSURE: Queuing Explosion!", ConsoleColor.Yellow);
                    CastExplosion(enemy);
                    m_WaitingForExplosion = false;
                    m_WaitingForEnergyBolt = true;
                    m_EnergyBoltStartTime = DateTime.UtcNow;
                    if (needsRetreat) RunFrom(enemy);
                    return true;
                }
            }

            // Finish with Energy Bolt
            if (m_WaitingForEnergyBolt && CanCastEnergyBolt())
            {
                double timeSinceExplosion = (DateTime.UtcNow - m_EnergyBoltStartTime).TotalSeconds;
                if (timeSinceExplosion >= EXPLOSION_DELAY - 0.3) // Time it with explosion hit
                {
                    Log("PRESSURE: ENERGY BOLT for combo finish!", ConsoleColor.Yellow);
                    CastEnergyBolt(enemy);
                    m_WaitingForEnergyBolt = false;
                    Log("*** PRESSURE COMBO COMPLETE ***", ConsoleColor.Green);
                    if (needsRetreat) RunFrom(enemy);
                    return true;
                }
                // Wait for timing
                if (needsRetreat) RunFrom(enemy);
                return true;
            }

            // Strangle for sustained DoT + heal reduction
            if (CanCastStrangle())
            {
                CastStrangle(enemy);
                if (needsRetreat) RunFrom(enemy);
                return true;
            }

            // Poison Strike for damage
            if (CanCastPoisonStrike())
            {
                CastPoisonStrike(enemy);
                if (needsRetreat) RunFrom(enemy);
                return true;
            }

            // Pain Spike spam as filler
            if (CanCastPainSpike())
            {
                CastPainSpike(enemy);
                if (needsRetreat) RunFrom(enemy);
                return true;
            }

            // Wither if multiple enemies
            if (CanCastWither() && HasMultipleEnemiesNearby(enemy))
            {
                CastWither();
                if (needsRetreat) RunFrom(enemy);
                return true;
            }

            // Nothing to cast, just retreat if needed
            if (needsRetreat)
            {
                RunFrom(enemy);
            }

            return true;
        }

        private bool HandleSpikePhase(Mobile enemy, int distance)
        {
            // THE DUMP COMBO:
            // 1. Evil Omen (amplifies next harmful effect)
            // 2. Explosion (queued, ~3s delay) - benefits from Corpse Skin fire debuff
            // 3. Energy Bolt (timed to land WITH Explosion)
            //
            // Corpse Skin should already be active from Setup phase
            // Note: EBolt is energy damage (no Corpse Skin bonus) but Evil Omen amplifies it

            if (distance > MAX_CAST_RANGE)
            {
                MoveTo(enemy);
                return true;
            }

            // Step 1: Cast Evil Omen if not active
            if (!IsEvilOmenActive() && CanCastEvilOmen())
            {
                CastEvilOmen(enemy);
                return true;
            }

            // Step 2: Queue Explosion
            if (!m_ExplosionQueued && CanCastExplosion())
            {
                CastExplosion(enemy);
                m_ExplosionQueued = true;
                m_ExplosionQueuedTime = DateTime.UtcNow;
                return true;
            }

            // Step 3: Time Energy Bolt to sync with Explosion
            // Explosion delay is ~2.5-3s, Energy Bolt is instant
            // Cast Energy Bolt ~2s after Explosion was queued
            if (m_ExplosionQueued)
            {
                double timeSinceExplosion = (DateTime.UtcNow - m_ExplosionQueuedTime).TotalSeconds;
                
                if (timeSinceExplosion >= 2.0 && CanCastEnergyBolt())
                {
                    CastEnergyBolt(enemy);
                    m_ExplosionQueued = false; // Combo complete
                    
                    Log("*** DUMP COMBO COMPLETE ***", ConsoleColor.Yellow);
                    return true;
                }

                // Still waiting for sync timing - spam Pain Spike
                if (CanCastPainSpike())
                {
                    CastPainSpike(enemy);
                    return true;
                }
            }

            return true;
        }

        private bool HandleExecutePhase(Mobile enemy, int distance)
        {
            // Target is low - spam Pain Spike to finish
            // Pain Spike deals more damage to lower health targets

            if (distance > MAX_CAST_RANGE)
            {
                MoveTo(enemy);
                return true;
            }

            // Evil Omen → Pain Spike for amplified finisher
            if (!IsEvilOmenActive() && CanCastEvilOmen())
            {
                CastEvilOmen(enemy);
                return true;
            }

            // Spam Pain Spike
            if (CanCastPainSpike())
            {
                CastPainSpike(enemy);
                return true;
            }

            // Poison Strike as backup
            if (CanCastPoisonStrike())
            {
                CastPoisonStrike(enemy);
                return true;
            }

            return true;
        }

        #endregion

        #region Movement Helpers

        private void RunFrom(Mobile enemy)
        {
            if (m_Sidekick.AIObject is SidekickAI ai)
            {
                Log($"Retreating from {enemy.Name}", ConsoleColor.Yellow);
                ai.RunFrom(enemy);
            }
        }

        private void MoveTo(Mobile enemy)
        {
            if (m_Sidekick.AIObject is SidekickAI ai)
            {
                Log($"Moving to casting range", ConsoleColor.Cyan);
                ai.MoveTo(enemy, true, OPTIMAL_CAST_RANGE);
            }
        }

        #endregion

        #region Spell Casting - Necromancy

        private bool CanCastPainSpike() =>
            m_Sidekick.Mana >= 5 &&
            m_Sidekick.Skills.Necromancy.Value >= 20 &&
            CooldownReady(m_LastPainSpikeTime, PAIN_SPIKE_COOLDOWN);

        private void CastPainSpike(Mobile enemy)
        {
            var spell = new PainSpikeSpell(m_Sidekick, null);
            spell.Cast();
            m_LastPainSpikeTime = DateTime.UtcNow;
            InvokeSpellTarget(enemy);
            Log($"Pain Spike → {enemy.Name}", ConsoleColor.DarkMagenta);
        }

        private bool CanCastPoisonStrike() =>
            m_Sidekick.Mana >= 17 &&
            m_Sidekick.Skills.Necromancy.Value >= 50 &&
            CooldownReady(m_LastPoisonStrikeTime, POISON_STRIKE_COOLDOWN);

        private void CastPoisonStrike(Mobile enemy)
        {
            var spell = new PoisonStrikeSpell(m_Sidekick, null);
            spell.Cast();
            m_LastPoisonStrikeTime = DateTime.UtcNow;
            InvokeSpellTarget(enemy);
            Log($"Poison Strike → {enemy.Name}", ConsoleColor.DarkGreen);
        }

        private bool CanCastStrangle() =>
            m_Sidekick.Mana >= 29 &&
            m_Sidekick.Skills.Necromancy.Value >= 65 &&
            CooldownReady(m_LastStrangleTime, STRANGLE_COOLDOWN);

        private void CastStrangle(Mobile enemy)
        {
            var spell = new StrangleSpell(m_Sidekick, null);
            spell.Cast();
            m_LastStrangleTime = DateTime.UtcNow;
            InvokeSpellTarget(enemy);
            Log($"Strangle → {enemy.Name}", ConsoleColor.DarkRed);
        }

        private bool CanCastEvilOmen() =>
            m_Sidekick.Mana >= 11 &&
            m_Sidekick.Skills.Necromancy.Value >= 20 &&
            CooldownReady(m_LastEvilOmenTime, EVIL_OMEN_COOLDOWN);

        private void CastEvilOmen(Mobile enemy)
        {
            var spell = new EvilOmenSpell(m_Sidekick, null);
            spell.Cast();
            m_LastEvilOmenTime = DateTime.UtcNow;
            m_EvilOmenAppliedTime = DateTime.UtcNow;
            InvokeSpellTarget(enemy);
            Log($"Evil Omen → {enemy.Name} (NEXT HIT AMPLIFIED)", ConsoleColor.DarkCyan);
        }

        private bool CanCastCorpseSkin() =>
            m_Sidekick.Mana >= 11 &&
            m_Sidekick.Skills.Necromancy.Value >= 20 &&
            CooldownReady(m_LastCorpseSkinTime, CORPSE_SKIN_COOLDOWN);

        private void CastCorpseSkin(Mobile enemy)
        {
            var spell = new CorpseSkinSpell(m_Sidekick, null);
            spell.Cast();
            m_LastCorpseSkinTime = DateTime.UtcNow;
            m_CorpseSkinAppliedTime = DateTime.UtcNow;
            InvokeSpellTarget(enemy);
            Log($"Corpse Skin → {enemy.Name} (-15% fire/poison resist)", ConsoleColor.Gray);
        }

        private bool CanCastWither() =>
            m_Sidekick.Mana >= 23 &&
            m_Sidekick.Skills.Necromancy.Value >= 60 &&
            CooldownReady(m_LastWitherTime, WITHER_COOLDOWN);

        private void CastWither()
        {
            var spell = new WitherSpell(m_Sidekick, null);
            spell.Cast();
            m_LastWitherTime = DateTime.UtcNow;
            Log("Wither (AoE)", ConsoleColor.DarkGray);
        }

        // Blood Oath - reflects damage back to attacker (great vs melee enemies)
        private bool CanCastBloodOath() =>
            m_Sidekick.Mana >= 13 &&
            m_Sidekick.Skills.Necromancy.Value >= 20 &&
            !IsBloodOathActive() &&
            CooldownReady(m_LastBloodOathTime, BLOOD_OATH_COOLDOWN);

        private void CastBloodOath(Mobile enemy)
        {
            var spell = new BloodOathSpell(m_Sidekick, null);
            spell.Cast();
            m_LastBloodOathTime = DateTime.UtcNow;
            m_BloodOathAppliedTime = DateTime.UtcNow;
            InvokeSpellTarget(enemy);
            Log($"Blood Oath → {enemy.Name} (DAMAGE REFLECTION)", ConsoleColor.DarkRed);
        }

        // Mind Rot - increases enemy mana cost (devastating vs casters)
        private bool CanCastMindRot() =>
            m_Sidekick.Mana >= 17 &&
            m_Sidekick.Skills.Necromancy.Value >= 30 &&
            !IsMindRotActive() &&
            CooldownReady(m_LastMindRotTime, MIND_ROT_COOLDOWN);

        private void CastMindRot(Mobile enemy)
        {
            var spell = new MindRotSpell(m_Sidekick, null);
            spell.Cast();
            m_LastMindRotTime = DateTime.UtcNow;
            m_MindRotAppliedTime = DateTime.UtcNow;
            InvokeSpellTarget(enemy);
            Log($"Mind Rot → {enemy.Name} (MANA COST +100%)", ConsoleColor.Magenta);
        }

        #endregion

        #region Spell Casting - Magery

        private bool CanCastCurse() =>
            m_Sidekick.Mana >= 11 &&
            m_Sidekick.Skills.Magery.Value >= 40 &&
            CooldownReady(m_LastCurseTime, CURSE_COOLDOWN);

        private void CastCurse(Mobile enemy)
        {
            var spell = new CurseSpell(m_Sidekick, null);
            spell.Cast();
            m_LastCurseTime = DateTime.UtcNow;
            m_CurseAppliedTime = DateTime.UtcNow;
            InvokeSpellTarget(enemy);
            Log($"Curse → {enemy.Name} (stats & resists down)", ConsoleColor.DarkYellow);
        }

        private bool CanCastExplosion() =>
            m_Sidekick.Mana >= 20 &&
            m_Sidekick.Skills.Magery.Value >= 65 &&
            CooldownReady(m_LastExplosionTime, EXPLOSION_COOLDOWN);

        private void CastExplosion(Mobile enemy)
        {
            var spell = new ExplosionSpell(m_Sidekick, null);
            spell.Cast();
            m_LastExplosionTime = DateTime.UtcNow;
            InvokeSpellTarget(enemy);
            Log($"Explosion QUEUED → {enemy.Name}", ConsoleColor.Red);
        }

        private bool CanCastEnergyBolt() =>
            m_Sidekick.Mana >= 11 &&
            m_Sidekick.Skills.Magery.Value >= 40 &&
            CooldownReady(m_LastEnergyBoltTime, ENERGY_BOLT_COOLDOWN);

        private void CastEnergyBolt(Mobile enemy)
        {
            var spell = new EnergyBoltSpell(m_Sidekick, null);
            spell.Cast();
            m_LastEnergyBoltTime = DateTime.UtcNow;
            InvokeSpellTarget(enemy);
            Log($"ENERGY BOLT → {enemy.Name}", ConsoleColor.Magenta);
        }

        private void CastGreaterHeal()
        {
            var spell = new GreaterHealSpell(m_Sidekick, null);
            spell.Cast();

            // Must invoke target on self for beneficial spell
            if (m_Sidekick.Target != null)
            {
                m_Sidekick.Target.Invoke(m_Sidekick, m_Sidekick);
            }

            m_LastHealSpellTime = DateTime.UtcNow;
            Log("Greater Heal (self)", ConsoleColor.Green);
        }

        #endregion

        #region Utility

        private bool CooldownReady(DateTime lastCast, double cooldown) =>
            (DateTime.UtcNow - lastCast).TotalSeconds >= cooldown;

        /// <summary>
        /// Invoke spell target immediately after casting.
        /// Uses same approach as MageCombatAI for reliable targeting.
        /// </summary>
        private void InvokeSpellTarget(Mobile enemy)
        {
            if (m_Sidekick?.Target != null && enemy != null && !enemy.Deleted && enemy.Alive)
            {
                m_Sidekick.Target.Invoke(m_Sidekick, enemy);
            }
        }

        private void ResetDebuffTracking()
        {
            m_CorpseSkinAppliedTime = DateTime.MinValue;
            m_CurseAppliedTime = DateTime.MinValue;
            m_EvilOmenAppliedTime = DateTime.MinValue;
            m_BloodOathAppliedTime = DateTime.MinValue;
            m_MindRotAppliedTime = DateTime.MinValue;
            m_ExplosionQueued = false;
        }

        private bool HasMultipleEnemiesNearby(Mobile primaryEnemy)
        {
            int count = 0;
            bool ownerNearby = false;

            foreach (Mobile m in m_Sidekick.GetMobilesInRange(5))
            {
                if (m == m_Sidekick) continue;

                // Check if owner is in AoE range - don't cast if so
                if (m == m_Sidekick.ControlMaster)
                {
                    ownerNearby = true;
                    break;
                }

                // Count hostile mobiles
                if (m != primaryEnemy && m.Alive && !m.Deleted)
                {
                    if (m.Combatant == m_Sidekick || m.Combatant == m_Sidekick.ControlMaster)
                        count++;
                }
            }

            // Don't use AoE if owner would be hit
            if (ownerNearby)
            {
                Log("Owner nearby - skipping AoE to avoid friendly fire", ConsoleColor.Yellow);
                return false;
            }

            return count >= 1;
        }

        private void Log(string message, ConsoleColor color)
        {
            Utility.PushColor(color);
            Console.WriteLine($"[NecroAI] {m_Sidekick.Name} - {message}");
            Utility.PopColor();
        }

        private void LogStatus(double selfHp, double enemyHp, int dist)
        {
            Utility.PushColor(ConsoleColor.DarkMagenta);
            Console.WriteLine($"[NecroAI] {m_Sidekick.Name} | Phase: {m_CurrentPhase} | HP: {selfHp:P0} | Enemy: {enemyHp:P0} | Dist: {dist} | Mana: {m_Sidekick.Mana}");
            Utility.PopColor();
        }

        private void LogPhaseChange(CombatPhase newPhase)
        {
            Utility.PushColor(ConsoleColor.White);
            Console.WriteLine($"[NecroAI] {m_Sidekick.Name} === PHASE: {m_CurrentPhase} → {newPhase} ===");
            Utility.PopColor();
        }

        #endregion

        #region Potions/Bandages (unchanged from original)

        private bool TryCurePoison()
        {
            if (TryUseCurePotion()) return true;

            if (m_Sidekick.Skills.Magery.Value >= 40 && m_Sidekick.Mana >= 6)
            {
                var spell = new CureSpell(m_Sidekick, null);
                spell.Cast();
                Log("Casting Cure", ConsoleColor.Green);
                return true;
            }
            return false;
        }

        // Note: No TryUseBandage - Necromancer lacks Healing skill

        private bool TryUseHealPotion()
        {
            if (!CooldownReady(m_LastHealPotionTime, HEAL_POTION_COOLDOWN))
            {
                Log("Heal potion on cooldown", ConsoleColor.Gray);
                return false;
            }
            if (m_Sidekick.Hits >= m_Sidekick.HitsMax)
            {
                Log("Full health, skipping potion", ConsoleColor.Gray);
                return false;
            }
            if (m_Sidekick.Poisoned)
            {
                Log("Poisoned, skipping heal potion", ConsoleColor.Gray);
                return false;
            }

            var pack = m_Sidekick.Backpack;
            var potion = pack?.FindItemByType(typeof(GreaterHealPotion)) as GreaterHealPotion;
            if (potion == null)
            {
                Log("No Greater Heal Potion in backpack!", ConsoleColor.Red);
                return false;
            }

            potion.Drink(m_Sidekick);
            m_LastHealPotionTime = DateTime.UtcNow;
            Log($"Drank Greater Heal Potion (HP: {m_Sidekick.Hits}/{m_Sidekick.HitsMax})", ConsoleColor.Green);
            return true;
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

            if (!CooldownReady(m_LastCurePotionTime, CURE_POTION_COOLDOWN)) return false;

            var pack = m_Sidekick.Backpack;
            var potion = pack?.FindItemByType(typeof(GreaterCurePotion)) as GreaterCurePotion;
            if (potion == null)
                return false;

            potion.Drink(m_Sidekick);
            m_LastCurePotionTime = DateTime.UtcNow;
            Log("Used Greater Cure Potion!", ConsoleColor.Magenta);
            return true;
        }

        // Necromancer has no Healing skill - bandages won't work
        public bool TryUseBandagePublic() => false;

        #endregion
    }
}