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
    /// Nature's Wrath - Deals 35-55 damage (mix of poison/physical), applies poison DoT, roots all targets
    /// Circle: 6 (20 mana)
    /// </summary>
    public class NatureNaturesWrathSpell : VystiaSpell
    {
        private static readonly SpellInfo m_Info = new SpellInfo(
            "Nature's Wrath", "Nature'sum Wrathum",
            203,
            9041,
            false
        );

        public override SpellCircle Circle => SpellCircle.Sixth;
        public override VystiaMagicSchool MagicSchool => VystiaMagicSchool.Nature;

        public NatureNaturesWrathSpell(Mobile caster, Item scroll) : base(caster, scroll, m_Info)
        {
        }

        public override bool CheckCast()
        {
            if (!base.CheckCast())
                return false;            // Vystia Reagent Check
            if (!Caster.Backpack.ConsumeTotal(typeof(PrimalVine), 1) || !Caster.Backpack.ConsumeTotal(typeof(LivingBark), 1))
            {
                Caster.SendMessage("You do not have enough reagents to cast this spell.");
                Caster.SendMessage("Required: PrimalVine (1), LivingBark (1)");
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
                Caster.PlaySound(0x5D0);

                // Spell effect
                // Deal damage
                double damage = Utility.RandomMinMax(35, 55);
                SpellHelper.Damage(this, target, damage, 0, 0, 0, 100, 0);
            }

            FinishSequence();
        }

        private class InternalTarget : Target
        {
            private readonly NatureNaturesWrathSpell m_Owner;

            public InternalTarget(NatureNaturesWrathSpell owner)
                : base(14, false, TargetFlags.Harmful)
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
