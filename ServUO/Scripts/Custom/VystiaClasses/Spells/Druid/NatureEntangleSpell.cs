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
    /// Entangle - Roots target in place (cannot move, can still cast/attack)
    /// Circle: 2 (6 mana)
    /// </summary>
    public class NatureEntangleSpell : VystiaSpell
    {
        private static readonly SpellInfo m_Info = new SpellInfo(
            "Entangle", "Entangleus",
            203,
            9041,
            false
        );

        public override SpellCircle Circle => SpellCircle.Second;
        public override VystiaMagicSchool MagicSchool => VystiaMagicSchool.Nature;

        public NatureEntangleSpell(Mobile caster, Item scroll) : base(caster, scroll, m_Info)
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

        public void Target(IDamageable target)
        {
            if (!Caster.CanSee(target))
            {
                Caster.SendLocalizedMessage(500237); // Target can not be seen.
            }
            else if (CheckHSequence(target))
            {
                SpellHelper.Turn(Caster, target);

                if (target is Mobile mobile)
                {
                    // Calculate root duration based on skill (4-8 seconds)
                    double duration = 4.0 + (Caster.Skills.Magery.Value / 50.0);

                    // Visual effect - vines/roots
                    mobile.FixedParticles(0x375A, 10, 30, 5013, 0x21, 0, EffectLayer.Waist); // Green vines effect
                    mobile.PlaySound(0x1FB); // Entangle sound

                    // Root the target (frozen = can't move)
                    mobile.Frozen = true;
                    mobile.SendMessage(0x22, "Vines entangle your legs! You cannot move!");
                    Caster.SendMessage(0x3B2, "You entangle your target in vines!");

                    // Timer to release the root
                    Timer.DelayCall(TimeSpan.FromSeconds(duration), () =>
                    {
                        if (mobile != null && !mobile.Deleted && mobile.Frozen)
                        {
                            mobile.Frozen = false;
                            mobile.SendMessage("The vines release their grip.");
                            mobile.FixedParticles(0x375A, 10, 15, 5013, 0x21, 0, EffectLayer.Waist);
                        }
                    });
                }
            }

            FinishSequence();
        }

        private class InternalTarget : Target
        {
            private readonly NatureEntangleSpell m_Owner;

            public InternalTarget(NatureEntangleSpell owner)
                : base(10, false, TargetFlags.Harmful)
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
