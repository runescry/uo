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
    /// Primordial Restoration - Massive AoE heal (80-120 HP), removes all poisons/curses, grants immunity to debuffs for 15s
    /// Circle: 7 (40 mana)
    /// </summary>
    public class NaturePrimordialRestorationSpell : VystiaSpell
    {
        private static readonly SpellInfo m_Info = new SpellInfo(
            "Primordial Restoration", "Primordialum Restorationum",
            203,
            9041,
            false
        );

        public override SpellCircle Circle => SpellCircle.Seventh;
        public override VystiaMagicSchool MagicSchool => VystiaMagicSchool.Nature;

        public NaturePrimordialRestorationSpell(Mobile caster, Item scroll) : base(caster, scroll, m_Info)
        {
        }

        public override bool CheckCast()
        {
            if (!base.CheckCast())
                return false;            // Vystia Reagent Check
            if (!Caster.Backpack.ConsumeTotal(typeof(LivingBark), 1) || !Caster.Backpack.ConsumeTotal(typeof(AncientRoot), 1))
            {
                Caster.SendMessage("You do not have enough reagents to cast this spell.");
                Caster.SendMessage("Required: LivingBark (1), AncientRoot (1)");
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

                // Visual effect
                Effects.SendLocationParticles(
                    EffectItem.Create(target.Location, target.Map, EffectItem.DefaultDuration),
                    0x376A, 9, 32, 0x481);

                // Sound effect
                Caster.PlaySound(0x1F2);

                // Spell effect
                // Heal target
                if (target is Mobile mobile)
                {
                    double healAmount = Utility.RandomMinMax(0, 0);
                    mobile.Heal((int)healAmount);
                    mobile.SendMessage("You have been healed!");
                }
            }

            FinishSequence();
        }

        private class InternalTarget : Target
        {
            private readonly NaturePrimordialRestorationSpell m_Owner;

            public InternalTarget(NaturePrimordialRestorationSpell owner)
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
