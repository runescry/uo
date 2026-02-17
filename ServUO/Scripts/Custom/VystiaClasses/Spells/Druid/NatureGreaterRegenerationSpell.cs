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
    /// Greater Regeneration - Heals 8-12 HP/tick (15 ticks = 120-180 total)
    /// Circle: 5 (14 mana)
    /// </summary>
    public class NatureGreaterRegenerationSpell : VystiaSpell
    {
        private static readonly SpellInfo m_Info = new SpellInfo(
            "Greater Regeneration", "Greaterum Regenerationum",
            203,
            9041,
            false
        );

        public override SpellCircle Circle => SpellCircle.Fifth;
        public override VystiaMagicSchool MagicSchool => VystiaMagicSchool.Nature;

        public NatureGreaterRegenerationSpell(Mobile caster, Item scroll) : base(caster, scroll, m_Info)
        {
        }

        public override bool CheckCast()
        {
            if (!base.CheckCast())
                return false;            // Vystia Reagent Check
            if (!Caster.Backpack.ConsumeTotal(typeof(ElderwoodSeed), 1) || !Caster.Backpack.ConsumeTotal(typeof(PrimalVine), 1))
            {
                Caster.SendMessage("You do not have enough reagents to cast this spell.");
                Caster.SendMessage("Required: ElderwoodSeed (1), PrimalVine (1)");
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

                // Start HoT effect - 15 ticks, 8-12 HP per tick (120-180 total)
                target.SendMessage(0x3B2, "Greater Regeneration empowers you! (15 ticks of 8-12 HP)");
                new GreaterRegenerationHoT(target, Caster, 15).Start();
            }

            FinishSequence();
        }

        /// <summary>
        /// Greater Regeneration HoT - Heals 8-12 HP per tick for 15 ticks
        /// </summary>
        private class GreaterRegenerationHoT
        {
            private Mobile m_Target;
            private Mobile m_Caster;
            private Timer m_Timer;
            private int m_TicksRemaining;

            public GreaterRegenerationHoT(Mobile target, Mobile caster, int ticks)
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

                // Heal 8-12 HP
                int healAmount = Utility.RandomMinMax(8, 12);
                m_Target.Heal(healAmount, m_Caster, false);

                // Visual effect every tick
                m_Target.FixedParticles(0x376A, 5, 10, 5038, 0x3B2, 0, EffectLayer.Waist);

                m_Target.SendMessage(0x3B2, $"[Greater Regen] +{healAmount} HP ({m_TicksRemaining - 1} ticks remaining)");

                m_TicksRemaining--;

                if (m_TicksRemaining <= 0)
                {
                    m_Target.SendMessage(0x3B2, "Greater Regeneration fades.");
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
            private readonly NatureGreaterRegenerationSpell m_Owner;

            public InternalTarget(NatureGreaterRegenerationSpell owner)
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
