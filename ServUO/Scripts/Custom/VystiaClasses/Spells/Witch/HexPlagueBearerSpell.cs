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
    /// Plague Bearer - Target takes 6-10 poison damage/tick and spreads poison to enemies within 2 tiles
    /// Circle: 4 (11 mana)
    /// </summary>
    public class HexPlagueBearerSpell : VystiaSpell
    {
        private static readonly SpellInfo m_Info = new SpellInfo(
            "Plague Bearer", "Plagueus Bearerum",
            203,
            9041,
            false
        );

        public override SpellCircle Circle => SpellCircle.Fourth;
        public override VystiaMagicSchool MagicSchool => VystiaMagicSchool.Hex;

        public HexPlagueBearerSpell(Mobile caster, Item scroll) : base(caster, scroll, m_Info)
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
                Caster.PlaySound(0x205);

                // Spell effect - Damage over time
                int ticks = 6;
                int damagePerTick = 5 + (int)(Caster.Skills.Conjuration.Value * 0.1);

                Timer.DelayCall(TimeSpan.Zero, TimeSpan.FromSeconds(5), ticks, () =>
                {
                    if (target is Mobile mobile && !mobile.Deleted && mobile.Alive)
                    {
                        mobile.Damage(damagePerTick, Caster);
                        mobile.FixedParticles(0x374A, 10, 15, 5021, EffectLayer.Waist);
                    }
                });

                if (target is Mobile m)
                    m.SendMessage("You have been afflicted with a curse!");
            }

            FinishSequence();
        }

        private class InternalTarget : Target
        {
            private readonly HexPlagueBearerSpell m_Owner;

            public InternalTarget(HexPlagueBearerSpell owner)
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
