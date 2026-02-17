using System;
using Server;
using Server.Targeting;
using Server.Mobiles;
using Server.Spells;
using Server.Items;

using Server.Spells.VystiaSpells;
namespace Server.Spells.VystiaSpells.Illusion
{
    /// <summary>
    /// True Invisibility - True Invisibility
    /// Circle: 7 (28 mana)
    /// </summary>
    public class IllusionTrueInvisibilitySpell : VystiaSpell
    {
        private static readonly SpellInfo m_Info = new SpellInfo(
            "True Invisibility", "Trueus Invisibilityum",
            203,
            9041,
            false
        );

        public override SpellCircle Circle => SpellCircle.Seventh;
        public override VystiaMagicSchool MagicSchool => VystiaMagicSchool.Illusion;

        public IllusionTrueInvisibilitySpell(Mobile caster, Item scroll) : base(caster, scroll, m_Info)
        {
        }

        public override bool CheckCast()
        {
            if (!base.CheckCast())
                return false;            // Vystia Reagent Check
            if (!Caster.Backpack.ConsumeTotal(typeof(ChaosPrism), 1) || !Caster.Backpack.ConsumeTotal(typeof(PhantomPetal), 1))
            {
                Caster.SendMessage("You do not have enough reagents to cast this spell.");
                Caster.SendMessage("Required: ChaosPrism (1), PhantomPetal (1)");
                return false;
            }



            return true;
        }

        public override void OnCast()
        {
            Caster.Target = new InternalTarget(this);
        }

        public void Target(Mobile target)
        {
            if (!Caster.CanSee(target))
            {
                Caster.SendLocalizedMessage(500237);
            }
            else if (CheckBSequence(target))
            {
                SpellHelper.Turn(Caster, target);

                // Visual effect
                target.FixedParticles(0x376A, 9, 32, 5007, EffectLayer.Waist);
                target.PlaySound(0x3C4);

                // Calculate duration (30 seconds - 2 minutes)
                double duration = 30.0 + (Caster.Skills.Magery.Value / 2.0);

                // True Invisibility - True Invisibility Circle: 7 (28 mana)
                target.Hidden = true;

                target.SendMessage(0x3B2, "You fade from sight!");
                if (target != Caster)
                    Caster.SendMessage(0x3B2, "You grant invisibility!");

                // Note: Hidden status is broken by actions, no timer needed
            }

            FinishSequence();
        }

        private class InternalTarget : Target
        {
            private readonly IllusionTrueInvisibilitySpell m_Owner;

            public InternalTarget(IllusionTrueInvisibilitySpell owner)
                : base(10, false, TargetFlags.Beneficial)
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
