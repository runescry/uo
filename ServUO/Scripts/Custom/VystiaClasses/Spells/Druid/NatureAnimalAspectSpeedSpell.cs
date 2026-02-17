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
    /// Animal Aspect: Speed - +20 DEX, +15% movement speed
    /// Circle: 2 (6 mana)
    /// </summary>
    public class NatureAnimalAspectSpeedSpell : VystiaSpell
    {
        private static readonly SpellInfo m_Info = new SpellInfo(
            "Animal Aspect: Speed", "Animalum Aspect:um Speedum",
            203,
            9041,
            false
        );

        public override SpellCircle Circle => SpellCircle.Second;
        public override VystiaMagicSchool MagicSchool => VystiaMagicSchool.Nature;

        public NatureAnimalAspectSpeedSpell(Mobile caster, Item scroll) : base(caster, scroll, m_Info)
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

        public void Target(Mobile target)
        {
            if (!Caster.CanSee(target))
            {
                Caster.SendLocalizedMessage(500237);
            }
            else if (CheckBSequence(target))
            {
                SpellHelper.Turn(Caster, target);

                // Visual effect - swift animal spirit
                target.FixedParticles(0x373A, 10, 15, 5018, 0x21, 0, EffectLayer.Waist);
                target.PlaySound(0x1EA);

                // Calculate duration based on skill (2-4 minutes)
                double duration = 2.0 + (Caster.Skills.Magery.Value / 50.0);

                // +20 DEX buff
                target.AddStatMod(new StatMod(StatType.Dex, "AnimalAspect_Speed_Dex", 20, TimeSpan.FromMinutes(duration)));

                target.SendMessage(0x3B2, "You gain the speed of a wild animal! (+20 DEX)");
                if (target != Caster)
                    Caster.SendMessage(0x3B2, "You grant the aspect of speed!");
            }

            FinishSequence();
        }

        private class InternalTarget : Target
        {
            private readonly NatureAnimalAspectSpeedSpell m_Owner;

            public InternalTarget(NatureAnimalAspectSpeedSpell owner)
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
