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
    /// Ballad of Health - Ballad of Health
    /// Circle: 4 (16 mana)
    /// </summary>
    public class BardicBalladofHealthSpell : VystiaSpell
    {
        private static readonly SpellInfo m_Info = new SpellInfo(
            "Ballad of Health", "Balladum Ofum Healthum",
            203,
            9041,
            false
        );

        public override SpellCircle Circle => SpellCircle.Fourth;
        public override VystiaMagicSchool MagicSchool => VystiaMagicSchool.Bardic;

        public BardicBalladofHealthSpell(Mobile caster, Item scroll) : base(caster, scroll, m_Info)
        {
        }

        public override bool CheckCast()
        {
            if (!base.CheckCast())
                return false;            // Vystia Reagent Check
            if (!Caster.Backpack.ConsumeTotal(typeof(MuseEssence), 1) || !Caster.Backpack.ConsumeTotal(typeof(HarmonyGem), 1))
            {
                Caster.SendMessage("You do not have enough reagents to cast this spell.");
                Caster.SendMessage("Required: MuseEssence (1), HarmonyGem (1)");
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

                // Spell effect
                // Heal target
                if (target is Mobile mobile)
                {
                    double healAmount = Utility.RandomMinMax(6, 10);
                    mobile.Heal((int)healAmount);
                    mobile.SendMessage("You have been healed!");
                }
            }

            FinishSequence();
        }

        private class InternalTarget : Target
        {
            private readonly BardicBalladofHealthSpell m_Owner;

            public InternalTarget(BardicBalladofHealthSpell owner)
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
