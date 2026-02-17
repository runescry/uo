using System;
using System.Collections.Generic;
using Server;
using Server.Targeting;
using Server.Mobiles;
using Server.Spells;
using Server.Items;

using Server.Spells.VystiaSpells;
namespace Server.Spells.VystiaSpells.Nature
{
    /// <summary>
    /// Healing Grove - Creates a healing area that heals allies for 5-8 HP/tick (10 ticks)
    /// Circle: 4 (11 mana)
    /// </summary>
    public class NatureHealingGroveSpell : VystiaSpell
    {
        private static readonly SpellInfo m_Info = new SpellInfo(
            "Healing Grove", "Healingum Groveus",
            203,
            9041,
            false
        );

        public override SpellCircle Circle => SpellCircle.Fourth;
        public override VystiaMagicSchool MagicSchool => VystiaMagicSchool.Nature;

        public NatureHealingGroveSpell(Mobile caster, Item scroll) : base(caster, scroll, m_Info)
        {
        }

        public override bool CheckCast()
        {
            if (!base.CheckCast())
                return false;            // Vystia Reagent Check
            if (!Caster.Backpack.ConsumeTotal(typeof(TreantSap), 1) || !Caster.Backpack.ConsumeTotal(typeof(ElderwoodSeed), 1))
            {
                Caster.SendMessage("You do not have enough reagents to cast this spell.");
                Caster.SendMessage("Required: TreantSap (1), ElderwoodSeed (1)");
                return false;
            }

            return true;
        }

        public override void OnCast()
        {
            Caster.Target = new InternalTarget(this);
        }

        public void Target(IPoint3D p)
        {
            if (!Caster.CanSee(p))
            {
                Caster.SendLocalizedMessage(500237); // Target can not be seen.
            }
            else if (CheckSequence())
            {
                SpellHelper.Turn(Caster, p);

                Point3D loc = new Point3D(p);

                // Initial visual effect - nature growth
                Effects.SendLocationParticles(
                    EffectItem.Create(loc, Caster.Map, EffectItem.DefaultDuration),
                    0x376A, 9, 32, 0x3B2, 0, 5029, 0);
                Caster.PlaySound(0x1F2);

                Caster.SendMessage(0x3B2, "You create a Healing Grove! Allies within 5 tiles will be healed.");

                // Start AoE HoT effect - 10 ticks, 5-8 HP per tick
                new HealingGroveAoE(Caster, loc, Caster.Map, 10).Start();
            }

            FinishSequence();
        }

        /// <summary>
        /// Healing Grove AoE - Heals all allies in area 5-8 HP per tick for 10 ticks
        /// </summary>
        private class HealingGroveAoE
        {
            private Mobile m_Caster;
            private Point3D m_Location;
            private Map m_Map;
            private Timer m_Timer;
            private int m_TicksRemaining;
            private const int RADIUS = 5;

            public HealingGroveAoE(Mobile caster, Point3D location, Map map, int ticks)
            {
                m_Caster = caster;
                m_Location = location;
                m_Map = map;
                m_TicksRemaining = ticks;
            }

            public void Start()
            {
                m_Timer = Timer.DelayCall(TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1), OnTick);
            }

            private void OnTick()
            {
                if (m_TicksRemaining <= 0 || m_Caster == null || m_Caster.Deleted || m_Map == null)
                {
                    Stop();
                    return;
                }

                // Visual effect at location every 2 ticks
                if (m_TicksRemaining % 2 == 0)
                {
                    Effects.SendLocationParticles(
                        EffectItem.Create(m_Location, m_Map, EffectItem.DefaultDuration),
                        0x376A, 9, 15, 0x3B2, 0, 5029, 0);
                }

                // Get all allies in radius
                IPooledEnumerable eable = m_Map.GetMobilesInRange(m_Location, RADIUS);
                List<Mobile> allies = new List<Mobile>();

                foreach (Mobile m in eable)
                {
                    if (m != null && m.Alive && !m.Deleted)
                    {
                        // Heal caster, party members, and pets
                        if (m == m_Caster ||
                            (m is BaseCreature && ((BaseCreature)m).ControlMaster == m_Caster) ||
                            (m_Caster.Party != null && m_Caster.Party == m.Party) ||
                            m_Caster.Guild != null && m_Caster.Guild == m.Guild)
                        {
                            allies.Add(m);
                        }
                    }
                }
                eable.Free();

                // Heal all allies
                foreach (Mobile ally in allies)
                {
                    int healAmount = Utility.RandomMinMax(5, 8);
                    ally.Heal(healAmount, m_Caster, false);
                    ally.FixedParticles(0x376A, 5, 10, 5038, 0x3B2, 0, EffectLayer.Waist);
                    ally.SendMessage(0x3B2, $"[Healing Grove] +{healAmount} HP");
                }

                m_TicksRemaining--;

                if (m_TicksRemaining <= 0)
                {
                    if (m_Caster != null && !m_Caster.Deleted)
                        m_Caster.SendMessage(0x3B2, "The Healing Grove fades.");
                    Stop();
                }
            }

            private void Stop()
            {
                if (m_Timer != null)
                {
                    m_Timer.Stop();
                    m_Timer = null;
                }
            }
        }

        private class InternalTarget : Target
        {
            private readonly NatureHealingGroveSpell m_Owner;

            public InternalTarget(NatureHealingGroveSpell owner)
                : base(12, true, TargetFlags.None)
            {
                m_Owner = owner;
            }

            protected override void OnTarget(Mobile from, object o)
            {
                if (o is IPoint3D)
                    m_Owner.Target((IPoint3D)o);
            }

            protected override void OnTargetFinish(Mobile from)
            {
                m_Owner.FinishSequence();
            }
        }
    }
}
