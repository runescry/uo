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
    /// Hawk Form (Aerial Shapeshift) - Transform into hawk: Can fly over obstacles, +40 DEX, -50% damage taken from ranged, weak melee, CANNOT CAST SPELLS
    /// Circle: 5 (14 mana)
    /// </summary>
    public class NatureHawkFormAerialShapeshiftSpell : VystiaSpell
    {
        private static readonly SpellInfo m_Info = new SpellInfo(
            "Hawk Form (Aerial Shapeshift)", "Hawkum Formum (aerialum Shapeshift)um",
            203,
            9041,
            false
        );

        public override SpellCircle Circle => SpellCircle.Fifth;
        public override VystiaMagicSchool MagicSchool => VystiaMagicSchool.Nature;

        public NatureHawkFormAerialShapeshiftSpell(Mobile caster, Item scroll) : base(caster, scroll, m_Info)
        {
        }

        public override bool CheckCast()
        {
            if (!base.CheckCast())
                return false;            // Vystia Reagent Check
            if (!Caster.Backpack.ConsumeTotal(typeof(ElderwoodSeed), 1) || !Caster.Backpack.ConsumeTotal(typeof(PrimalVine), 1))
            {
                Caster.SendMessage("You do not have enough reagents to cast this spell.");
                Caster.SendMessage("Required: ElderwoodSeed (1), PrimalVine (1)");
                return false;
            }



            return true;
        }

        public override void OnCast()
        {
            if (CheckSequence())
            {
                // Check if already in hawk form - toggle off
                if (Caster.BodyMod == 5) // Eagle/Hawk body
                {
                    RemoveHawkForm(Caster);
                    Caster.SendMessage(0x3B2, "You return to your normal form.");
                }
                else
                {
                    ApplyHawkForm(Caster);
                }
            }

            FinishSequence();
        }

        private void ApplyHawkForm(Mobile m)
        {
            // Visual effect
            m.FixedParticles(0x3728, 10, 15, 5038, 0x21, 0, EffectLayer.Waist);
            m.PlaySound(0x1E3); // Bird cry

            // Transform into hawk/eagle
            m.BodyMod = 5; // Eagle
            m.HueMod = 0;

            // Calculate duration based on skill (4-7 minutes)
            double duration = 4.0 + (Caster.Skills.Magery.Value / 40.0);

            // +40 DEX buff
            m.AddStatMod(new StatMod(StatType.Dex, "HawkForm_Dex", 40, TimeSpan.FromMinutes(duration)));

            m.SendMessage(0x3B2, "You transform into a swift hawk! (+40 DEX, aerial mobility)");
            m.SendMessage(0x22, "You cannot cast spells while in hawk form!");

            // Auto-revert after duration
            Timer.DelayCall(TimeSpan.FromMinutes(duration), () =>
            {
                if (m != null && !m.Deleted && m.BodyMod == 5)
                {
                    RemoveHawkForm(m);
                    m.SendMessage("Your hawk form fades.");
                }
            });
        }

        private static void RemoveHawkForm(Mobile m)
        {
            m.BodyMod = 0;
            m.HueMod = -1;

            // Remove stat mods
            m.RemoveStatMod("HawkForm_Dex");

            // Visual effect
            m.FixedParticles(0x3728, 10, 15, 5038, 0x21, 0, EffectLayer.Waist);
            m.PlaySound(0x1FE);
        }
    }
}
