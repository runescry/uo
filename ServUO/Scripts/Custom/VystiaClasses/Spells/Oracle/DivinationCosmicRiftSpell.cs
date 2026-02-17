using System;
using Server;
using Server.Targeting;
using Server.Mobiles;
using Server.Spells;
using Server.Items;

using Server.Spells.VystiaSpells;
namespace Server.Spells.VystiaSpells.Divination
{
    /// <summary>
    /// Cosmic Rift - Cosmic Rift
    /// Circle: 7 (28 mana)
    /// </summary>
    public class DivinationCosmicRiftSpell : VystiaSpell
    {
        private static readonly SpellInfo m_Info = new SpellInfo(
            "Cosmic Rift", "Cosmicum Riftum",
            203,
            9041,
            false
        );

        public override SpellCircle Circle => SpellCircle.Seventh;
        public override VystiaMagicSchool MagicSchool => VystiaMagicSchool.Divination;

        public DivinationCosmicRiftSpell(Mobile caster, Item scroll) : base(caster, scroll, m_Info)
        {
        }

        public override bool CheckCast()
        {
            if (!base.CheckCast())
                return false;            // Vystia Reagent Check
            if (!Caster.Backpack.ConsumeTotal(typeof(SeeingStone), 1) || !Caster.Backpack.ConsumeTotal(typeof(FateThread), 1))
            {
                Caster.SendMessage("You do not have enough reagents to cast this spell.");
                Caster.SendMessage("Required: SeeingStone (1), FateThread (1)");
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
                // Deal damage
                double damage = Utility.RandomMinMax(55, 90);
                SpellHelper.Damage(this, target, damage, 0, 0, 0, 0, 100);
            }

            FinishSequence();
        }

        private class InternalTarget : Target
        {
            private readonly DivinationCosmicRiftSpell m_Owner;

            public InternalTarget(DivinationCosmicRiftSpell owner)
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
