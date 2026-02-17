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
    /// Barkskin - +5 Physical Resistance, +5 Poison Resistance
    /// Circle: 1 (4 mana)
    /// </summary>
    public class NatureBarkskinSpell : VystiaSpell
    {
        private static readonly SpellInfo m_Info = new SpellInfo(
            "Barkskin", "Barkskinum",
            203,
            9041,
            false
        );

        public override SpellCircle Circle => SpellCircle.First;
        public override VystiaMagicSchool MagicSchool => VystiaMagicSchool.Nature;

        public NatureBarkskinSpell(Mobile caster, Item scroll) : base(caster, scroll, m_Info)
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

        public void Target(Mobile target)
        {
            if (!Caster.CanSee(target))
            {
                Caster.SendLocalizedMessage(500237);
            }
            else if (CheckBSequence(target))
            {
                SpellHelper.Turn(Caster, target);

                // Visual effect - bark/wood texture
                target.FixedParticles(0x375A, 10, 15, 5013, 0x21, 0, EffectLayer.Waist);
                target.PlaySound(0x22F);

                // Calculate duration based on skill (2-5 minutes)
                double duration = 2.0 + (Caster.Skills.Magery.Value / 50.0);

                // +5 Physical Resistance, +5 Poison Resistance
                ResistanceMod physMod = new ResistanceMod(ResistanceType.Physical, 5);
                ResistanceMod poisMod = new ResistanceMod(ResistanceType.Poison, 5);

                target.AddResistanceMod(physMod);
                target.AddResistanceMod(poisMod);

                target.SendMessage(0x3B2, "Your skin hardens like bark!");
                if (target != Caster)
                    Caster.SendMessage(0x3B2, "You grant barkskin protection!");

                // Remove after duration
                Timer.DelayCall(TimeSpan.FromMinutes(duration), () =>
                {
                    if (target != null && !target.Deleted)
                    {
                        target.RemoveResistanceMod(physMod);
                        target.RemoveResistanceMod(poisMod);
                        target.SendMessage("The barkskin fades.");
                    }
                });
            }

            FinishSequence();
        }

        private class InternalTarget : Target
        {
            private readonly NatureBarkskinSpell m_Owner;

            public InternalTarget(NatureBarkskinSpell owner)
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
