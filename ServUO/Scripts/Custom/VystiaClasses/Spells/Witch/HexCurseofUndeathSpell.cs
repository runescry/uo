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
    /// Curse of Undeath - Target becomes cursed undead: heals become damage, holy spells deal double damage, cannot naturally regenerate, +50% poison/curse damage taken
    /// Circle: 8 (50 mana)
    /// </summary>
    public class HexCurseofUndeathSpell : VystiaSpell
    {
        private static readonly SpellInfo m_Info = new SpellInfo(
            "Curse of Undeath", "Curseus Ofum Undeathum",
            203,
            9041,
            false
        );

        public override SpellCircle Circle => SpellCircle.Eighth;
        public override VystiaMagicSchool MagicSchool => VystiaMagicSchool.Hex;

        public HexCurseofUndeathSpell(Mobile caster, Item scroll) : base(caster, scroll, m_Info)
        {
        }

        public override bool CheckCast()
        {
            if (!base.CheckCast())
                return false;            // Vystia Reagent Check
            if (!Caster.Backpack.ConsumeTotal(typeof(CursedPearl), 1) || !Caster.Backpack.ConsumeTotal(typeof(CursedSalt), 1) || !Caster.Backpack.ConsumeTotal(typeof(CursedSalt), 1))
            {
                Caster.SendMessage("You do not have enough reagents to cast this spell.");
                Caster.SendMessage("Required: CursedPearl (1), CursedSalt (1), CursedSalt (1)");
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
            private readonly HexCurseofUndeathSpell m_Owner;

            public InternalTarget(HexCurseofUndeathSpell owner)
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
