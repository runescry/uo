using System;
using Server;
using Server.Targeting;
using Server.Mobiles;
using Server.Spells;
using Server.Items;

using Server.Spells.VystiaSpells;
namespace Server.Spells.VystiaSpells.Enchanting
{
    /// <summary>
    /// Rune of Apocalypse - Rune of Apocalypse
    /// Circle: 8 (32 mana)
    /// </summary>
    public class EnchantingRuneofApocalypseSpell : VystiaSpell
    {
        private static readonly SpellInfo m_Info = new SpellInfo(
            "Rune of Apocalypse", "Runeus Ofum Apocalypseus",
            203,
            9041,
            false
        );

        public override SpellCircle Circle => SpellCircle.Eighth;
        public override VystiaMagicSchool MagicSchool => VystiaMagicSchool.Enchanting;

        public EnchantingRuneofApocalypseSpell(Mobile caster, Item scroll) : base(caster, scroll, m_Info)
        {
        }

        public override bool CheckCast()
        {
            if (!base.CheckCast())
                return false;            // Vystia Reagent Check
            if (!Caster.Backpack.ConsumeTotal(typeof(RunicPowder), 1) || !Caster.Backpack.ConsumeTotal(typeof(TitanRune), 1) || !Caster.Backpack.ConsumeTotal(typeof(TitanRune), 1))
            {
                Caster.SendMessage("You do not have enough reagents to cast this spell.");
                Caster.SendMessage("Required: RunicPowder (1), TitanRune (1), TitanRune (1)");
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

                // Spell effect
                // Deal damage
                double damage = Utility.RandomMinMax(150, 250);
                SpellHelper.Damage(this, target, damage, 0, 0, 0, 0, 100);
            }

            FinishSequence();
        }

        private class InternalTarget : Target
        {
            private readonly EnchantingRuneofApocalypseSpell m_Owner;

            public InternalTarget(EnchantingRuneofApocalypseSpell owner)
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
