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
    /// Nature's Touch - Heals 8-15 HP
    /// Circle: 1 (4 mana)
    /// </summary>
    public class NatureNaturesTouchSpell : VystiaSpell
    {
        private static readonly SpellInfo m_Info = new SpellInfo(
            "Nature's Touch", "Nature'sum Tactus",
            203,
            9041,
            false
        );

        public override SpellCircle Circle => SpellCircle.First;
        public override VystiaMagicSchool MagicSchool => VystiaMagicSchool.Nature;

        public NatureNaturesTouchSpell(Mobile caster, Item scroll) : base(caster, scroll, m_Info)
        {
        }

        public override bool CheckCast()
        {
            if (!base.CheckCast())
                return false;            // Vystia Reagent Check
            if (!Caster.Backpack.ConsumeTotal(typeof(WildMoss), 1) || !Caster.Backpack.ConsumeTotal(typeof(Moonpetal), 1))
            {
                Caster.SendMessage("You do not have enough reagents to cast this spell.");
                Caster.SendMessage("Required: WildMoss (1), Moonpetal (1)");
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
                target.FixedParticles(0x376A, 10, 15, 5038, 0x481, 0, EffectLayer.Waist);

                // Sound effect
                Caster.PlaySound(0x1F2);

                // Spell effect
                // Heal target
                if (target is Mobile mobile)
                {
                    double healAmount = Utility.RandomMinMax(8, 15);
                    mobile.Heal((int)healAmount);
                    mobile.SendMessage("You have been healed!");
                }
            }

            FinishSequence();
        }

        private class InternalTarget : Target
        {
            private readonly NatureNaturesTouchSpell m_Owner;

            public InternalTarget(NatureNaturesTouchSpell owner)
                : base(8, false, TargetFlags.Harmful)
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
