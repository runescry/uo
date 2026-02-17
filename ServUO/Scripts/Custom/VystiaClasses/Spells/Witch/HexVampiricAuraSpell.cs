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
    /// Vampiric Aura - All damage dealt by caster heals caster for 40% of damage, +15% spell damage
    /// Circle: 6 (20 mana)
    /// </summary>
    public class HexVampiricAuraSpell : VystiaSpell
    {
        private static readonly SpellInfo m_Info = new SpellInfo(
            "Vampiric Aura", "Vampiricum Auraum",
            203,
            9041,
            false
        );

        public override SpellCircle Circle => SpellCircle.Sixth;
        public override VystiaMagicSchool MagicSchool => VystiaMagicSchool.Hex;

        public HexVampiricAuraSpell(Mobile caster, Item scroll) : base(caster, scroll, m_Info)
        {
        }

        public override bool CheckCast()
        {
            if (!base.CheckCast())
                return false;            // Vystia Reagent Check
            if (!Caster.Backpack.ConsumeTotal(typeof(HagsHair), 1) || !Caster.Backpack.ConsumeTotal(typeof(CursedPearl), 1))
            {
                Caster.SendMessage("You do not have enough reagents to cast this spell.");
                Caster.SendMessage("Required: HagsHair (1), CursedPearl (1)");
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
                Caster.PlaySound(0x1FB);

                // Spell effect
                // Heal target
                if (target is Mobile mobile)
                {
                    double healAmount = Utility.RandomMinMax(0, 0);
                    mobile.Heal((int)healAmount);
                    mobile.SendMessage("You have been healed!");
                }
            }

            FinishSequence();
        }

        private class InternalTarget : Target
        {
            private readonly HexVampiricAuraSpell m_Owner;

            public InternalTarget(HexVampiricAuraSpell owner)
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
