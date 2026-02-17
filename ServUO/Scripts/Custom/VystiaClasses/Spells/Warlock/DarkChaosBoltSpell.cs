using System;
using Server;
using Server.Targeting;
using Server.Mobiles;
using Server.Spells;
using Server.Items;

using Server.Spells.VystiaSpells;
namespace Server.Spells.VystiaSpells.Dark
{
    /// <summary>
    /// Chaos Bolt - Chaos Bolt
    /// Circle: 2 (8 mana)
    /// </summary>
    public class DarkChaosBoltSpell : VystiaSpell
    {
        private static readonly SpellInfo m_Info = new SpellInfo(
            "Chaos Bolt", "Chaosum Sagitta",
            203,
            9041,
            false
        );

        public override SpellCircle Circle => SpellCircle.Second;
        public override VystiaMagicSchool MagicSchool => VystiaMagicSchool.Dark;

        public DarkChaosBoltSpell(Mobile caster, Item scroll) : base(caster, scroll, m_Info)
        {
        }

        public override bool CheckCast()
        {
            if (!base.CheckCast())
                return false;            // Vystia Reagent Check
            if (!Caster.Backpack.ConsumeTotal(typeof(VoidCrystal), 1) || !Caster.Backpack.ConsumeTotal(typeof(VoidWeed), 1))
            {
                Caster.SendMessage("You do not have enough reagents to cast this spell.");
                Caster.SendMessage("Required: VoidCrystal (1), VoidWeed (1)");
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

                // Spell effect - Chaos damage (random element each cast)
                double damage = Utility.RandomMinMax(12, 18);
                damage += Caster.Skills[GetSchoolSkill()].Value * 0.1;

                // Chaos damage - random element each time
                int roll = Utility.Random(5);
                int phys = 0, fire = 0, cold = 0, poison = 0, energy = 0;
                switch (roll)
                {
                    case 0: phys = 100; break;
                    case 1: fire = 100; break;
                    case 2: cold = 100; break;
                    case 3: poison = 100; break;
                    default: energy = 100; break;
                }

                SpellHelper.Damage(this, target, damage, phys, fire, cold, poison, energy);

                if (target is Mobile m)
                    m.SendMessage(0x22, "You are struck by chaotic energy!");
            }

            FinishSequence();
        }

        private class InternalTarget : Target
        {
            private readonly DarkChaosBoltSpell m_Owner;

            public InternalTarget(DarkChaosBoltSpell owner)
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
