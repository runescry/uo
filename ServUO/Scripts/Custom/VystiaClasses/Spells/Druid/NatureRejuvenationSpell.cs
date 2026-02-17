using System;
using Server;
using Server.Targeting;
using Server.Mobiles;
using Server.Spells;
using Server.Items;

using Server.Spells.VystiaSpells;
namespace Server.Spells.VystiaSpells.Nature
{
    /// <summary>
    /// Rejuvenation - Heals 3-6 HP/tick (10 ticks = 30-60 total)
    /// Circle: 2 (6 mana)
    /// </summary>
    public class NatureRejuvenationSpell : VystiaSpell
    {
        private static readonly SpellInfo m_Info = new SpellInfo(
            "Rejuvenation", "Rejuvenationum",
            203,
            9041,
            false
        );

        public override SpellCircle Circle => SpellCircle.Second;
        public override VystiaMagicSchool MagicSchool => VystiaMagicSchool.Nature;

        public NatureRejuvenationSpell(Mobile caster, Item scroll) : base(caster, scroll, m_Info)
        {
        }

        public override bool CheckCast()
        {
            if (!base.CheckCast())
                return false;            // Vystia Reagent Check
            if (!Caster.Backpack.ConsumeTotal(typeof(Moonpetal), 1) || !Caster.Backpack.ConsumeTotal(typeof(DruidBark), 1))
            {
                Caster.SendMessage("You do not have enough reagents to cast this spell.");
                Caster.SendMessage("Required: Moonpetal (1), DruidBark (1)");
                return false;
            }



            return true;
        }

        public override void OnCast()
        {
// Check fizzle and trigger skill gain

            Caster.Target = new InternalTarget(this);
        }

        public void Target(Mobile target)
        {
            if (!Caster.CanSee(target))
            {
                Caster.SendLocalizedMessage(500237); // Target can not be seen.
            }
            else if (CheckBSequence(target)) // Beneficial sequence for healing
            {
                SpellHelper.Turn(Caster, target);

                // Initial visual effect
                target.FixedParticles(0x376A, 10, 15, 5038, 0x3B2, 0, EffectLayer.Waist);
                Caster.PlaySound(0x1F2);

                // Start HoT effect - 10 ticks, 3-6 HP per tick
                target.SendMessage(0x3B2, "Rejuvenation flows through you! (10 ticks of 3-6 HP)");
                new RejuvenationHoT(target, Caster, 10).Start();
            }

            FinishSequence();
        }

        /// <summary>
        /// Rejuvenation HoT - Heals 3-6 HP per tick for 10 ticks
        /// </summary>
        private class RejuvenationHoT
        {
            private Mobile m_Target;
            private Mobile m_Caster;
            private Timer m_Timer;
            private int m_TicksRemaining;

            public RejuvenationHoT(Mobile target, Mobile caster, int ticks)
            {
                m_Target = target;
                m_Caster = caster;
                m_TicksRemaining = ticks;
            }

            public void Start()
            {
                m_Timer = Timer.DelayCall(TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1), OnTick);
            }

            private void OnTick()
            {
                if (m_TicksRemaining <= 0 || m_Target == null || m_Target.Deleted || !m_Target.Alive)
                {
                    Stop();
                    return;
                }

                // Heal 3-6 HP
                int healAmount = Utility.RandomMinMax(3, 6);
                m_Target.Heal(healAmount, m_Caster, false);

                // Visual effect every tick
                m_Target.FixedParticles(0x376A, 5, 10, 5038, 0x3B2, 0, EffectLayer.Waist);

                m_Target.SendMessage(0x3B2, $"[Rejuvenation] +{healAmount} HP ({m_TicksRemaining - 1} ticks remaining)");

                m_TicksRemaining--;

                if (m_TicksRemaining <= 0)
                {
                    m_Target.SendMessage(0x3B2, "Rejuvenation fades.");
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
            private readonly NatureRejuvenationSpell m_Owner;

            public InternalTarget(NatureRejuvenationSpell owner)
                : base(8, false, TargetFlags.Beneficial)
            {
                m_Owner = owner;
            }

            protected override void OnTarget(Mobile from, object o)
            {
                if (o is Mobile)
                    m_Owner.Target((Mobile)o);
            }

            protected override void OnTargetFinish(Mobile from)
            {
                m_Owner.FinishSequence();
            }
        }
    }
}
