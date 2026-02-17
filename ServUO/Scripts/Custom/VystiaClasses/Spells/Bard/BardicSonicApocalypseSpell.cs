using System;
using Server;
using Server.Targeting;
using Server.Mobiles;
using Server.Spells;
using Server.Items;

using Server.Spells.VystiaSpells;
namespace Server.Spells.VystiaSpells.Bardic
{
    /// <summary>
    /// Sonic Apocalypse - Sonic Apocalypse
    /// Circle: 7 (28 mana)
    /// </summary>
    public class BardicSonicApocalypseSpell : VystiaSpell
    {
        private static readonly SpellInfo m_Info = new SpellInfo(
            "Sonic Apocalypse", "Sonicum Apocalypseus",
            203,
            9041,
            false
        );

        public override SpellCircle Circle => SpellCircle.Seventh;
        public override VystiaMagicSchool MagicSchool => VystiaMagicSchool.Bardic;

        public BardicSonicApocalypseSpell(Mobile caster, Item scroll) : base(caster, scroll, m_Info)
        {
        }

        public override bool CheckCast()
        {
            if (!base.CheckCast())
                return false;            // Vystia Reagent Check
            if (!Caster.Backpack.ConsumeTotal(typeof(GoldenString), 1) || !Caster.Backpack.ConsumeTotal(typeof(DragonScale), 1))
            {
                Caster.SendMessage("You do not have enough reagents to cast this spell.");
                Caster.SendMessage("Required: GoldenString (1), DragonScale (1)");
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
            else if (CheckHSequence(target))
            {
                SpellHelper.Turn(Caster, target);

                // Visual effect
                target.FixedParticles(0x374A, 10, 30, 5013, 0x21, 0, EffectLayer.Waist);
                target.PlaySound(0x1FB);

                // Calculate duration (8-20 seconds)
                double duration = 8.0 + (Caster.Skills.Magery.Value / 10.0);

                // Sonic Apocalypse - Sonic Apocalypse Circle: 7 (28 mana)
                target.AddStatMod(new StatMod(StatType.Int, "BardicSonicApocalypse_Int", -7, TimeSpan.FromSeconds(duration)));

                target.SendMessage(0x22, "You feel weakened by BardicSonicApocalypse!");
                Caster.SendMessage(0x3B2, "You curse your enemy!");
            }

            FinishSequence();
        }

        private class InternalTarget : Target
        {
            private readonly BardicSonicApocalypseSpell m_Owner;

            public InternalTarget(BardicSonicApocalypseSpell owner)
                : base(12, false, TargetFlags.Harmful)
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
