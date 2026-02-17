using System;
using Server;
using Server.Targeting;
using Server.Mobiles;
using Server.Spells;
using Server.Items;

using Server.Spells.VystiaSpells;
namespace Server.Spells.VystiaSpells.Hex
{
    /// <summary>
    /// Crippling Curse - -20 DEX, -30% movement speed, -20% attack speed
    /// Circle: 4 (11 mana)
    /// </summary>
    public class HexCripplingCurseSpell : VystiaSpell
    {
        private static readonly SpellInfo m_Info = new SpellInfo(
            "Crippling Curse", "Cripplingum Curseus",
            203,
            9041,
            false
        );

        public override SpellCircle Circle => SpellCircle.Fourth;
        public override VystiaMagicSchool MagicSchool => VystiaMagicSchool.Hex;

        public HexCripplingCurseSpell(Mobile caster, Item scroll) : base(caster, scroll, m_Info)
        {
        }

        public override bool CheckCast()
        {
            if (!base.CheckCast())
                return false;            // Vystia Reagent Check
            if (!Caster.Backpack.ConsumeTotal(typeof(ToadsEye), 1) || !Caster.Backpack.ConsumeTotal(typeof(SwampLotus), 1))
            {
                Caster.SendMessage("You do not have enough reagents to cast this spell.");
                Caster.SendMessage("Required: ToadsEye (1), SwampLotus (1)");
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
                target.FixedParticles(0x36BD, 20, 10, 5044, 0x21, 0, EffectLayer.Head);
                target.PlaySound(0x307);

                // Crippling Curse - -20 DEX, -30% movement speed, -20% attack speed Circle: 4 (11 mana)
                double damage = Utility.RandomMinMax(25, 62);
                damage += Caster.Skills.Magery.Value / 10.0;

                SpellHelper.Damage(this, target, damage, 100, 0, 0, 0, 0);

                Caster.SendMessage(0x3B2, "Your spell strikes true!");
            }

            FinishSequence();
        }

        private class InternalTarget : Target
        {
            private readonly HexCripplingCurseSpell m_Owner;

            public InternalTarget(HexCripplingCurseSpell owner)
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
