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
    /// Toxic Bloom - Explodes for 15-25 poison damage, applies poison DoT (5 damage/tick for 10s)
    /// Circle: 4 (11 mana)
    /// </summary>
    public class NatureToxicBloomSpell : VystiaSpell
    {
        private static readonly SpellInfo m_Info = new SpellInfo(
            "Toxic Bloom", "Toxicum Bloomum",
            203,
            9041,
            false
        );

        public override SpellCircle Circle => SpellCircle.Fourth;
        public override VystiaMagicSchool MagicSchool => VystiaMagicSchool.Nature;

        public NatureToxicBloomSpell(Mobile caster, Item scroll) : base(caster, scroll, m_Info)
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
// Check fizzle and trigger skill gain

            Caster.Target = new InternalTarget(this);
        }

        public void Target(IDamageable target)
        {
            if (!Caster.CanSee(target))
            {
                Caster.SendLocalizedMessage(500237);
            }
            else if (CheckHSequence(target))
            {
                SpellHelper.Turn(Caster, target);

                if (target is Mobile mobile)
                {
                    Caster.DoHarmful(mobile);

                    // Visual effect - poison explosion
                    mobile.FixedParticles(0x374A, 10, 15, 5021, 0x48E, 0, EffectLayer.Waist); // Green poison
                    mobile.PlaySound(0x205);

                    // 15-25 poison damage
                    double damage = Utility.RandomMinMax(15, 25);
                    damage += Caster.Skills.Magery.Value / 15.0;
                    SpellHelper.Damage(this, mobile, damage, 0, 0, 0, 100, 0); // 100% poison

                    mobile.SendMessage(0x22, "A toxic bloom explodes on you!");
                    Caster.SendMessage(0x3B2, "Your toxic bloom explodes on the target!");

                    // Apply poison DoT (5 damage/tick for 10 ticks = 10 seconds)
                    new ToxicBloomDoT(Caster, mobile).Start();
                }
            }

            FinishSequence();
        }

        private class ToxicBloomDoT
        {
            private Mobile m_Caster;
            private Mobile m_Target;
            private Timer m_Timer;
            private int m_Ticks;
            private const int MAX_TICKS = 10;

            public ToxicBloomDoT(Mobile caster, Mobile target)
            {
                m_Caster = caster;
                m_Target = target;
                m_Ticks = 0;
            }

            public void Start()
            {
                m_Timer = Timer.DelayCall(TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1), OnTick);
            }

            private void OnTick()
            {
                if (m_Ticks >= MAX_TICKS || m_Target == null || m_Target.Deleted || !m_Target.Alive)
                {
                    Stop();
                    return;
                }

                if (m_Caster != null && !m_Caster.Deleted)
                {
                    // 5 poison damage per tick
                    AOS.Damage(m_Target, m_Caster, 5, 0, 0, 0, 100, 0);
                    m_Target.FixedParticles(0x374A, 1, 10, 5021, 0x48E, 0, EffectLayer.Waist);

                    if (m_Ticks % 3 == 0)
                        m_Target.SendMessage(0x22, "The poison burns!");
                }

                m_Ticks++;
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
            private readonly NatureToxicBloomSpell m_Owner;

            public InternalTarget(NatureToxicBloomSpell owner)
                : base(12, false, TargetFlags.Harmful)
            {
                m_Owner = owner;
            }

            protected override void OnTarget(Mobile from, object o)
            {
                if (o is IDamageable)
                    m_Owner.Target((IDamageable)o);
            }

            protected override void OnTargetFinish(Mobile from)
            {
                m_Owner.FinishSequence();
            }
        }
    }
}
