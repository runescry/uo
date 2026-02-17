using System;
using Server;
using Server.Targeting;
using Server.Mobiles;
using Server.Spells;
using Server.Items;

using Server.Spells.VystiaSpells;
namespace Server.Spells.VystiaSpells.Illusion
{
    /// <summary>
    /// Illusory Terrain - Illusory Terrain
    /// Circle: 4 (16 mana)
    /// </summary>
    public class IllusionIllusoryTerrainSpell : VystiaSpell
    {
        private static readonly SpellInfo m_Info = new SpellInfo(
            "Illusory Terrain", "Illusoryum Terrainum",
            203,
            9041,
            false
        );

        public override SpellCircle Circle => SpellCircle.Fourth;
        public override VystiaMagicSchool MagicSchool => VystiaMagicSchool.Illusion;

        public IllusionIllusoryTerrainSpell(Mobile caster, Item scroll) : base(caster, scroll, m_Info)
        {
        }

        public override bool CheckCast()
        {
            if (!base.CheckCast())
                return false;            // Vystia Reagent Check
            if (!Caster.Backpack.ConsumeTotal(typeof(DreamCrystal), 1) || !Caster.Backpack.ConsumeTotal(typeof(RealitySplinter), 1))
            {
                Caster.SendMessage("You do not have enough reagents to cast this spell.");
                Caster.SendMessage("Required: DreamCrystal (1), RealitySplinter (1)");
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

                // Spell effect - Direct damage
                double damage = Utility.RandomMinMax(20, 25);
                damage += Caster.Skills.Conjuration.Value * 0.5;

                SpellHelper.Damage(this, target, damage, 0, 0, 0, 0, 100);
                if (target is Mobile mobile)
                    mobile.SendMessage("You are struck by magical energy!");
            }

            FinishSequence();
        }

        private class InternalTarget : Target
        {
            private readonly IllusionIllusoryTerrainSpell m_Owner;

            public InternalTarget(IllusionIllusoryTerrainSpell owner)
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
