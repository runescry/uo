using System;
using Server;
using Server.Targeting;
using Server.Mobiles;
using Server.Spells;
using Server.Items;

using Server.Spells.VystiaSpells;
namespace Server.Spells.VystiaSpells.Nature
{
    /// <summary>
    /// Bear Form (Lesser Shapeshift) - Transform into bear: +30 STR, +15 Physical Resist, melee attacks deal bonus damage, CANNOT CAST SPELLS
    /// Circle: 3 (9 mana)
    /// </summary>
    public class NatureBearFormLesserShapeshiftSpell : VystiaSpell
    {
        private static readonly SpellInfo m_Info = new SpellInfo(
            "Bear Form (Lesser Shapeshift)", "Bearum Formum (lesserum Shapeshift)um",
            203,
            9041,
            false
        );

        public override SpellCircle Circle => SpellCircle.Third;
        public override VystiaMagicSchool MagicSchool => VystiaMagicSchool.Nature;

        public NatureBearFormLesserShapeshiftSpell(Mobile caster, Item scroll) : base(caster, scroll, m_Info)
        {
        }

        public override bool CheckCast()
        {
            if (!base.CheckCast())
                return false;            // Vystia Reagent Check
            if (!Caster.Backpack.ConsumeTotal(typeof(DruidBark), 1) || !Caster.Backpack.ConsumeTotal(typeof(TreantSap), 1))
            {
                Caster.SendMessage("You do not have enough reagents to cast this spell.");
                Caster.SendMessage("Required: DruidBark (1), TreantSap (1)");
                return false;
            }



            return true;
        }

        private static int m_OriginalBody;
        private static int m_OriginalHue;

        public override void OnCast()
        {
            if (CheckSequence())
            {
                // Check if already in bear form - toggle off
                if (Caster.BodyMod == 213) // Bear body
                {
                    RemoveBearForm(Caster);
                    Caster.SendMessage(0x3B2, "You return to your normal form.");
                }
                else
                {
                    ApplyBearForm(Caster);
                }
            }

            FinishSequence();
        }

        private void ApplyBearForm(Mobile m)
        {
            // Store original body
            m_OriginalBody = m.BodyMod;
            m_OriginalHue = m.HueMod;

            // Visual effect
            m.FixedParticles(0x3728, 10, 15, 5038, 0x21, 0, EffectLayer.Waist);
            m.PlaySound(0xA3); // Bear growl

            // Transform into bear
            m.BodyMod = 213; // Brown bear
            m.HueMod = 0;

            // Calculate duration based on skill (3-6 minutes)
            double duration = 3.0 + (Caster.Skills.Magery.Value / 40.0);

            // +30 STR buff
            m.AddStatMod(new StatMod(StatType.Str, "BearForm_Str", 30, TimeSpan.FromMinutes(duration)));

            // +15 Physical Resistance
            ResistanceMod physMod = new ResistanceMod(ResistanceType.Physical, 15);
            m.AddResistanceMod(physMod);

            m.SendMessage(0x3B2, "You transform into a mighty bear! (+30 STR, +15 Physical Resist)");
            m.SendMessage(0x22, "You cannot cast spells while in bear form!");

            // Auto-revert after duration
            Timer.DelayCall(TimeSpan.FromMinutes(duration), () =>
            {
                if (m != null && !m.Deleted && m.BodyMod == 213)
                {
                    RemoveBearForm(m);
                    m.SendMessage("Your bear form fades.");
                }
            });
        }

        private static void RemoveBearForm(Mobile m)
        {
            m.BodyMod = 0;
            m.HueMod = -1;

            // Remove stat mods
            m.RemoveStatMod("BearForm_Str");

            // Visual effect
            m.FixedParticles(0x3728, 10, 15, 5038, 0x21, 0, EffectLayer.Waist);
            m.PlaySound(0x1FE);
        }
    }
}
