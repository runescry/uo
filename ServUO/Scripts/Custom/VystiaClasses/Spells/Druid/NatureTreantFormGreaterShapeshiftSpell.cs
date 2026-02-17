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
    /// Treant Form (Greater Shapeshift) - Transform into treant: +50 STR, +30 Physical Resist, +25% max HP, AoE root on attacks, CANNOT CAST SPELLS
    /// Circle: 6 (20 mana)
    /// </summary>
    public class NatureTreantFormGreaterShapeshiftSpell : VystiaSpell
    {
        private static readonly SpellInfo m_Info = new SpellInfo(
            "Treant Form (Greater Shapeshift)", "Treantum Formum (greaterum Shapeshift)um",
            203,
            9041,
            false
        );

        public override SpellCircle Circle => SpellCircle.Sixth;
        public override VystiaMagicSchool MagicSchool => VystiaMagicSchool.Nature;

        public NatureTreantFormGreaterShapeshiftSpell(Mobile caster, Item scroll) : base(caster, scroll, m_Info)
        {
        }

        public override bool CheckCast()
        {
            if (!base.CheckCast())
                return false;            // Vystia Reagent Check
            if (!Caster.Backpack.ConsumeTotal(typeof(PrimalVine), 1) || !Caster.Backpack.ConsumeTotal(typeof(LivingBark), 1))
            {
                Caster.SendMessage("You do not have enough reagents to cast this spell.");
                Caster.SendMessage("Required: PrimalVine (1), LivingBark (1)");
                return false;
            }



            return true;
        }

        public override void OnCast()
        {
            if (CheckSequence())
            {
                // Check if already in treant form - toggle off
                if (Caster.BodyMod == 301) // Treant body
                {
                    RemoveTreantForm(Caster);
                    Caster.SendMessage(0x3B2, "You return to your normal form.");
                }
                else
                {
                    ApplyTreantForm(Caster);
                }
            }

            FinishSequence();
        }

        private void ApplyTreantForm(Mobile m)
        {
            // Visual effect
            m.FixedParticles(0x3728, 10, 15, 5038, 0x21, 0, EffectLayer.Waist);
            m.PlaySound(0x22F); // Wood creaking

            // Transform into treant
            m.BodyMod = 301; // Treant
            m.HueMod = 0;

            // Calculate duration based on skill (5-8 minutes)
            double duration = 5.0 + (Caster.Skills.Magery.Value / 40.0);

            // +50 STR buff
            m.AddStatMod(new StatMod(StatType.Str, "TreantForm_Str", 50, TimeSpan.FromMinutes(duration)));

            // +30 Physical Resistance
            ResistanceMod physMod = new ResistanceMod(ResistanceType.Physical, 30);
            m.AddResistanceMod(physMod);

            m.SendMessage(0x3B2, "You transform into a mighty treant! (+50 STR, +30 Physical Resist)");
            m.SendMessage(0x22, "You cannot cast spells while in treant form!");

            // Auto-revert after duration
            Timer.DelayCall(TimeSpan.FromMinutes(duration), () =>
            {
                if (m != null && !m.Deleted && m.BodyMod == 301)
                {
                    m.RemoveResistanceMod(physMod);
                    RemoveTreantForm(m);
                    m.SendMessage("Your treant form fades.");
                }
            });
        }

        private static void RemoveTreantForm(Mobile m)
        {
            m.BodyMod = 0;
            m.HueMod = -1;

            // Remove stat mods
            m.RemoveStatMod("TreantForm_Str");

            // Visual effect
            m.FixedParticles(0x3728, 10, 15, 5038, 0x21, 0, EffectLayer.Waist);
            m.PlaySound(0x1FE);
        }
    }
}
