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
    /// Demi-Lich Transformation - Demi-Lich Transformation
    /// Circle: 7 (28 mana)
    /// </summary>
    public class NecromancyDemiLichTransformationSpell : VystiaSpell
    {
        private static readonly SpellInfo m_Info = new SpellInfo(
            "Demi-Lich Transformation", "Demi-lichum Transformationum",
            203,
            9041,
            false
        );

        public override SpellCircle Circle => SpellCircle.Seventh;
        public override VystiaMagicSchool MagicSchool => VystiaMagicSchool.Necromancy;

        public NecromancyDemiLichTransformationSpell(Mobile caster, Item scroll) : base(caster, scroll, m_Info)
        {
        }

        public override bool CheckCast()
        {
            if (!base.CheckCast())
                return false;            // Vystia Reagent Check
            if (!Caster.Backpack.ConsumeTotal(typeof(PhylacteryShard), 1) || !Caster.Backpack.ConsumeTotal(typeof(ReaperEssence), 1))
            {
                Caster.SendMessage("You do not have enough reagents to cast this spell.");
                Caster.SendMessage("Required: PhylacteryShard (1), ReaperEssence (1)");
                return false;
            }



            return true;
        }

        public override void OnCast()
        {
            if (CheckSequence())
            {
                // Toggle off if already in this form
                if (Caster.BodyMod == 24)
                {
                    RemoveNecromancyDemiLichTransformation(Caster);
                    Caster.SendMessage(0x3B2, "You return to your normal form.");
                }
                else
                {
                    ApplyNecromancyDemiLichTransformation(Caster);
                }
            }

            FinishSequence();
        }

        private void ApplyNecromancyDemiLichTransformation(Mobile m)
        {
            // Visual effect
            m.FixedParticles(0x3728, 10, 15, 5038, 0x21, 0, EffectLayer.Waist);
            m.PlaySound(0x1FE);

            // Transform
            m.BodyMod = 24;
            m.HueMod = 0;

            // Calculate duration (3-6 minutes)
            double duration = 3.0 + (Caster.Skills.Magery.Value / 40.0);

            // Demi-Lich Transformation - Demi-Lich Transformation Circle: 7 (28 mana)
            m.AddStatMod(new StatMod(StatType.Int, "NecromancyDemiLichTransformation_Int", 50, TimeSpan.FromMinutes(duration)));

            m.SendMessage(0x3B2, "You transform! (+50 INT)");
            m.SendMessage(0x22, "You cannot cast spells in this form!");

            // Auto-revert after duration
            Timer.DelayCall(TimeSpan.FromMinutes(duration), () =>
            {
                if (m != null && !m.Deleted && m.BodyMod == 24)
                {
                    RemoveNecromancyDemiLichTransformation(m);
                    m.SendMessage("Your transformation fades.");
                }
            });
        }

        private static void RemoveNecromancyDemiLichTransformation(Mobile m)
        {
            m.BodyMod = 0;
            m.HueMod = -1;
            m.RemoveStatMod("NecromancyDemiLichTransformation_Int");

            m.FixedParticles(0x3728, 10, 15, 5038, 0x21, 0, EffectLayer.Waist);
            m.PlaySound(0x1FE);
        }
    }
}
