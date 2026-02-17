using System;
using Server;
using Server.Targeting;
using Server.Mobiles;
using Server.Spells;
using Server.Items;

using Server.Spells.VystiaSpells;
namespace Server.Spells.VystiaSpells.Necromancy
{
    /// <summary>
    /// Bone Armor - Bone Armor
    /// Circle: 3 (12 mana)
    /// </summary>
    public class NecromancyBoneArmorSpell : VystiaSpell
    {
        private static readonly SpellInfo m_Info = new SpellInfo(
            "Bone Armor", "Boneus Armatura",
            203,
            9041,
            false
        );

        public override SpellCircle Circle => SpellCircle.Third;
        public override VystiaMagicSchool MagicSchool => VystiaMagicSchool.Necromancy;

        public NecromancyBoneArmorSpell(Mobile caster, Item scroll) : base(caster, scroll, m_Info)
        {
        }

        public override bool CheckCast()
        {
            if (!base.CheckCast())
                return false;            // Vystia Reagent Check
            if (!Caster.Backpack.ConsumeTotal(typeof(CorpseAsh), 1) || !Caster.Backpack.ConsumeTotal(typeof(SoulFragment), 1))
            {
                Caster.SendMessage("You do not have enough reagents to cast this spell.");
                Caster.SendMessage("Required: CorpseAsh (1), SoulFragment (1)");
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

                // Bone Armor - Bone Armor Circle: 3 (12 mana)
                target.AddStatMod(new StatMod(StatType.Int, "NecromancyBoneArmor_Int", -3, TimeSpan.FromSeconds(duration)));

                target.SendMessage(0x22, "You feel weakened by NecromancyBoneArmor!");
                Caster.SendMessage(0x3B2, "You curse your enemy!");
            }

            FinishSequence();
        }

        private class InternalTarget : Target
        {
            private readonly NecromancyBoneArmorSpell m_Owner;

            public InternalTarget(NecromancyBoneArmorSpell owner)
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
