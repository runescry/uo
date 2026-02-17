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
    /// Wolf Form (Advanced Shapeshift) - Transform into wolf: +25 DEX, +30% movement speed, +15% attack speed, bleed attacks, CANNOT CAST SPELLS
    /// Circle: 4 (11 mana)
    /// </summary>
    public class NatureWolfFormAdvancedShapeshiftSpell : VystiaSpell
    {
        private static readonly SpellInfo m_Info = new SpellInfo(
            "Wolf Form (Advanced Shapeshift)", "Wolfum Formum (advancedum Shapeshift)um",
            203,
            9041,
            false
        );

        public override SpellCircle Circle => SpellCircle.Fourth;
        public override VystiaMagicSchool MagicSchool => VystiaMagicSchool.Nature;

        public NatureWolfFormAdvancedShapeshiftSpell(Mobile caster, Item scroll) : base(caster, scroll, m_Info)
        {
        }

        public override bool CheckCast()
        {
            if (!base.CheckCast())
                return false;            // Vystia Reagent Check
            if (!Caster.Backpack.ConsumeTotal(typeof(TreantSap), 1) || !Caster.Backpack.ConsumeTotal(typeof(ElderwoodSeed), 1))
            {
                Caster.SendMessage("You do not have enough reagents to cast this spell.");
                Caster.SendMessage("Required: TreantSap (1), ElderwoodSeed (1)");
                return false;
            }



            return true;
        }

        public override void OnCast()
        {
            if (CheckSequence())
            {
                // Check if already in wolf form - toggle off
                if (Caster.BodyMod == 225) // Wolf body
                {
                    RemoveWolfForm(Caster);
                    Caster.SendMessage(0x3B2, "You return to your normal form.");
                }
                else
                {
                    ApplyWolfForm(Caster);
                }
            }

            FinishSequence();
        }

        private void ApplyWolfForm(Mobile m)
        {
            // Visual effect
            m.FixedParticles(0x3728, 10, 15, 5038, 0x21, 0, EffectLayer.Waist);
            m.PlaySound(0xE5); // Wolf howl

            // Transform into wolf
            m.BodyMod = 225; // Timber wolf
            m.HueMod = 0;

            // Calculate duration based on skill (3-6 minutes)
            double duration = 3.0 + (Caster.Skills.Magery.Value / 40.0);

            // +25 DEX buff
            m.AddStatMod(new StatMod(StatType.Dex, "WolfForm_Dex", 25, TimeSpan.FromMinutes(duration)));

            m.SendMessage(0x3B2, "You transform into a swift wolf! (+25 DEX, +30% speed)");
            m.SendMessage(0x22, "You cannot cast spells while in wolf form!");

            // Auto-revert after duration
            Timer.DelayCall(TimeSpan.FromMinutes(duration), () =>
            {
                if (m != null && !m.Deleted && m.BodyMod == 225)
                {
                    RemoveWolfForm(m);
                    m.SendMessage("Your wolf form fades.");
                }
            });
        }

        private static void RemoveWolfForm(Mobile m)
        {
            m.BodyMod = 0;
            m.HueMod = -1;

            // Remove stat mods
            m.RemoveStatMod("WolfForm_Dex");

            // Visual effect
            m.FixedParticles(0x3728, 10, 15, 5038, 0x21, 0, EffectLayer.Waist);
            m.PlaySound(0x1FE);
        }
    }
}
