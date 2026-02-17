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
    /// Swarm - Swarm of insects deals 6-10 damage/tick, -40% accuracy, interrupts spellcasting
    /// Circle: 6 (20 mana)
    /// </summary>
    public class NatureSwarmSpell : VystiaSpell
    {
        private static readonly SpellInfo m_Info = new SpellInfo(
            "Swarm", "Swarmum",
            203,
            9041,
            false
        );

        public override SpellCircle Circle => SpellCircle.Sixth;
        public override VystiaMagicSchool MagicSchool => VystiaMagicSchool.Nature;

        public NatureSwarmSpell(Mobile caster, Item scroll) : base(caster, scroll, m_Info)
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
                Effects.SendLocationParticles(
                    EffectItem.Create(target.Location, target.Map, EffectItem.DefaultDuration),
                    0x376A, 9, 32, 0x481);

                // Sound effect
                Caster.PlaySound(0x5D4);

                // Spell effect
                // Deal damage
                double damage = Utility.RandomMinMax(6, 10);
                SpellHelper.Damage(this, target, damage, 0, 0, 0, 100, 0);
            }

            FinishSequence();
        }

        private class InternalTarget : Target
        {
            private readonly NatureSwarmSpell m_Owner;

            public InternalTarget(NatureSwarmSpell owner)
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
