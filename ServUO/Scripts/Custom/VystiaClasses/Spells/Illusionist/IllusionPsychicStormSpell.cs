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
    /// Psychic Storm - Psychic Storm
    /// Circle: 5 (20 mana)
    /// </summary>
    public class IllusionPsychicStormSpell : VystiaSpell
    {
        private static readonly SpellInfo m_Info = new SpellInfo(
            "Psychic Storm", "Psychicum Stormum",
            203,
            9041,
            false
        );

        public override SpellCircle Circle => SpellCircle.Fifth;
        public override VystiaMagicSchool MagicSchool => VystiaMagicSchool.Illusion;

        public IllusionPsychicStormSpell(Mobile caster, Item scroll) : base(caster, scroll, m_Info)
        {
        }

        public override bool CheckCast()
        {
            if (!base.CheckCast())
                return false;            // Vystia Reagent Check
            if (!Caster.Backpack.ConsumeTotal(typeof(RealitySplinter), 1) || !Caster.Backpack.ConsumeTotal(typeof(VoidMirror), 1))
            {
                Caster.SendMessage("You do not have enough reagents to cast this spell.");
                Caster.SendMessage("Required: RealitySplinter (1), VoidMirror (1)");
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

                // Spell effect - Direct damage
                double damage = Utility.RandomMinMax(25, 30);
                damage += Caster.Skills.Conjuration.Value * 0.5;

                SpellHelper.Damage(this, target, damage, 0, 0, 0, 0, 100);
                if (target is Mobile mobile)
                    mobile.SendMessage("You are struck by magical energy!");
            }

            FinishSequence();
        }

        private class InternalTarget : Target
        {
            private readonly IllusionPsychicStormSpell m_Owner;

            public InternalTarget(IllusionPsychicStormSpell owner)
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
