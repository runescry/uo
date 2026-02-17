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
    /// Contagious Hex - Cursed target spreads hex to allies within 3 tiles (-10% attack speed, 3-5 damage/tick)
    /// Circle: 3 (9 mana)
    /// </summary>
    public class HexContagiousHexSpell : VystiaSpell
    {
        private static readonly SpellInfo m_Info = new SpellInfo(
            "Contagious Hex", "Contagiousum Hexum",
            203,
            9041,
            false
        );

        public override SpellCircle Circle => SpellCircle.Third;
        public override VystiaMagicSchool MagicSchool => VystiaMagicSchool.Hex;

        public HexContagiousHexSpell(Mobile caster, Item scroll) : base(caster, scroll, m_Info)
        {
        }

        public override bool CheckCast()
        {
            if (!base.CheckCast())
                return false;            // Vystia Reagent Check
            if (!Caster.Backpack.ConsumeTotal(typeof(Witchweed), 1) || !Caster.Backpack.ConsumeTotal(typeof(ToadsEye), 1))
            {
                Caster.SendMessage("You do not have enough reagents to cast this spell.");
                Caster.SendMessage("Required: Witchweed (1), ToadsEye (1)");
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
                Caster.PlaySound(0x1FC);

                // Spell effect
                // Deal damage
                double damage = Utility.RandomMinMax(3, 5);
                SpellHelper.Damage(this, target, damage, 0, 0, 0, 100, 0);
            }

            FinishSequence();
        }

        private class InternalTarget : Target
        {
            private readonly HexContagiousHexSpell m_Owner;

            public InternalTarget(HexContagiousHexSpell owner)
                : base(12, false, TargetFlags.Harmful)
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
