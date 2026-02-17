using System;
using System.Collections.Generic;
using Server;
using Server.Items;
using Server.Mobiles;

namespace Server.Mobiles
{
    /// <summary>
    /// Base class for all Vystia regional bosses
    /// Provides phase tracking, enrage timers, and standardized loot/reward systems
    /// </summary>
    public abstract class BaseVystiaBoss : BaseCreature
    {
        #region Phase System

        protected int m_CurrentPhase = 1;
        protected int m_MaxPhases = 3;
        protected DateTime m_NextPhaseCheck = DateTime.UtcNow;

        /// <summary>
        /// Current phase (1-based)
        /// </summary>
        public int CurrentPhase => m_CurrentPhase;

        /// <summary>
        /// Maximum number of phases for this boss
        /// </summary>
        public int MaxPhases => m_MaxPhases;

        /// <summary>
        /// Check and transition phases based on health percentage
        /// </summary>
        protected virtual void CheckPhaseTransition()
        {
            if (m_CurrentPhase >= m_MaxPhases)
                return;

            double hpPercent = (double)Hits / HitsMax;
            int newPhase = GetPhaseForHealth(hpPercent);

            if (newPhase > m_CurrentPhase)
            {
                int oldPhase = m_CurrentPhase;
                m_CurrentPhase = newPhase;
                OnPhaseTransition(oldPhase, newPhase);
            }
        }

        /// <summary>
        /// Get the phase number for a given health percentage
        /// Override in derived classes for custom phase thresholds
        /// </summary>
        protected virtual int GetPhaseForHealth(double hpPercent)
        {
            // Default: 3 phases at 66% and 33%
            if (m_MaxPhases == 3)
            {
                if (hpPercent <= 0.33) return 3;
                if (hpPercent <= 0.66) return 2;
                return 1;
            }
            else if (m_MaxPhases == 4)
            {
                if (hpPercent <= 0.25) return 4;
                if (hpPercent <= 0.50) return 3;
                if (hpPercent <= 0.75) return 2;
                return 1;
            }
            else if (m_MaxPhases == 5)
            {
                if (hpPercent <= 0.20) return 5;
                if (hpPercent <= 0.40) return 4;
                if (hpPercent <= 0.60) return 3;
                if (hpPercent <= 0.80) return 2;
                return 1;
            }

            return 1;
        }

        /// <summary>
        /// Called when boss transitions to a new phase
        /// Override to add phase-specific effects
        /// </summary>
        protected virtual void OnPhaseTransition(int oldPhase, int newPhase)
        {
            Say($"Phase {newPhase} begins!");
            Effects.SendLocationParticles(EffectItem.Create(Location, Map, EffectItem.DefaultDuration), 0x3728, 10, 10, 2023);
            PlaySound(0x1F2);
        }

        #endregion

        #region Enrage System

        protected DateTime m_EnrageStartTime;
        protected TimeSpan m_EnrageTimer = TimeSpan.FromMinutes(15);
        protected bool m_IsEnraged = false;

        /// <summary>
        /// Enrage timer for this boss (default 15 minutes)
        /// </summary>
        public TimeSpan EnrageTimer => m_EnrageTimer;

        /// <summary>
        /// Whether the boss is currently enraged
        /// </summary>
        public bool IsEnraged => m_IsEnraged;

        /// <summary>
        /// Time remaining until enrage
        /// </summary>
        public TimeSpan TimeUntilEnrage
        {
            get
            {
                if (m_IsEnraged)
                    return TimeSpan.Zero;
                return m_EnrageTimer - (DateTime.UtcNow - m_EnrageStartTime);
            }
        }

        /// <summary>
        /// Check if boss should enrage
        /// </summary>
        protected virtual void CheckEnrage()
        {
            if (m_IsEnraged)
                return;

            if (DateTime.UtcNow - m_EnrageStartTime >= m_EnrageTimer)
            {
                m_IsEnraged = true;
                OnEnrage();
            }
        }

        /// <summary>
        /// Called when boss enrages
        /// Override to add enrage-specific effects
        /// </summary>
        protected virtual void OnEnrage()
        {
            Say("ENRAGE! You have taken too long!");
            Effects.SendLocationParticles(EffectItem.Create(Location, Map, EffectItem.DefaultDuration), 0x3709, 20, 10, 2023);
            PlaySound(0x1F2);

            // Enrage: +50% damage, +25% attack speed
            DamageMin = (int)(DamageMin * 1.5);
            DamageMax = (int)(DamageMax * 1.5);
        }

        #endregion

        #region Constructor

        public BaseVystiaBoss(AIType ai, FightMode fightMode, int rangePerception, int rangeFight, double activeSpeed, double passiveSpeed)
            : base(ai, fightMode, rangePerception, rangeFight, activeSpeed, passiveSpeed)
        {
            m_EnrageStartTime = DateTime.UtcNow;
            m_NextPhaseCheck = DateTime.UtcNow;
        }

        public BaseVystiaBoss(Serial serial) : base(serial)
        {
        }

        #endregion

        #region Overrides

        public override void OnThink()
        {
            base.OnThink();

            // Check phase transitions every 2 seconds
            if (DateTime.UtcNow >= m_NextPhaseCheck)
            {
                m_NextPhaseCheck = DateTime.UtcNow + TimeSpan.FromSeconds(2);
                CheckPhaseTransition();
            }

            // Check enrage
            CheckEnrage();
        }

        public override bool OnBeforeDeath()
        {
            // Award reputation and piety before death
            if (LastKiller is PlayerMobile killer)
            {
                AwardBossRewards(killer);
            }

            return base.OnBeforeDeath();
        }

        public override void OnDeath(Container c)
        {
            base.OnDeath(c);

            // Generate boss loot using centralized system
            if (c != null)
            {
                VystiaBossLootSystem.GenerateBossLoot(this, c);
            }
        }

        #endregion

        #region Boss Rewards

        /// <summary>
        /// Award reputation and piety rewards for killing this boss
        /// </summary>
        protected virtual void AwardBossRewards(PlayerMobile killer)
        {
            // Award reputation to all players in combat
            var combatants = new System.Collections.Generic.List<Mobile>();
            foreach (var aggr in Aggressors)
            {
                if (aggr.Attacker != null && !combatants.Contains(aggr.Attacker))
                    combatants.Add(aggr.Attacker);
            }
            foreach (var aggr in Aggressed)
            {
                if (aggr.Defender != null && !combatants.Contains(aggr.Defender))
                    combatants.Add(aggr.Defender);
            }
            foreach (var combatant in combatants)
            {
                if (combatant is PlayerMobile pm && pm.Alive)
                {
                    // Award faction reputation
                    var faction = GetAlignedFaction();
                    if (faction != Server.Custom.VystiaClasses.Factions.VystiaFaction.None)
                    {
                        Server.Custom.VystiaClasses.Factions.ReputationRewards.AwardBossReputation(pm, faction, false);
                    }

                    // Award piety for killing religion's enemy boss
                    var playerReligion = Server.Custom.VystiaClasses.Religion.VystiaPiety.GetReligion(pm);
                    if (playerReligion != Server.Custom.VystiaClasses.Religion.VystiaReligion.None)
                    {
                        var enemyReligion = GetEnemyReligion();
                        if (enemyReligion != Server.Custom.VystiaClasses.Religion.VystiaReligion.None && 
                            Server.Custom.VystiaClasses.Religion.VystiaReligionSystem.AreReligionsOpposed(playerReligion, enemyReligion))
                        {
                            Server.Custom.VystiaClasses.Religion.VystiaPiety.AddPiety(pm, 35, $"boss kill: {Name}");
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Get the faction aligned with this boss's region
        /// Override in derived classes
        /// </summary>
        protected virtual Server.Custom.VystiaClasses.Factions.VystiaFaction GetAlignedFaction()
        {
            return Server.Custom.VystiaClasses.Factions.VystiaFaction.None;
        }

        /// <summary>
        /// Get the religion that opposes this boss
        /// Override in derived classes
        /// </summary>
        protected virtual Server.Custom.VystiaClasses.Religion.VystiaReligion GetEnemyReligion()
        {
            return Server.Custom.VystiaClasses.Religion.VystiaReligion.None;
        }

        #endregion

        #region Serialization

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // version

            writer.Write(m_CurrentPhase);
            writer.Write(m_MaxPhases);
            writer.Write(m_EnrageStartTime);
            writer.Write(m_EnrageTimer);
            writer.Write(m_IsEnraged);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            m_CurrentPhase = reader.ReadInt();
            m_MaxPhases = reader.ReadInt();
            m_EnrageStartTime = reader.ReadDateTime();
            m_EnrageTimer = reader.ReadTimeSpan();
            m_IsEnraged = reader.ReadBool();

            if (m_EnrageStartTime == DateTime.MinValue)
                m_EnrageStartTime = DateTime.UtcNow;
        }

        #endregion
    }
}
