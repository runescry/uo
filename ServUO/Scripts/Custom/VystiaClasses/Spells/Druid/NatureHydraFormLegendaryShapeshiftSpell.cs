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
    /// Hydra Form (Legendary Shapeshift) - Transform into multi-headed hydra: +70 STR, +30 DEX, triple attack, regenerates 15 HP/tick, immune to poison, CANNOT CAST SPELLS
    /// Circle: 8 (50 mana)
    /// </summary>
    public class NatureHydraFormLegendaryShapeshiftSpell : VystiaSpell
    {
        private static readonly SpellInfo m_Info = new SpellInfo(
            "Hydra Form (Legendary Shapeshift)", "Hydraum Formum (legendaryum Shapeshift)um",
            203,
            9041,
            false
        );

        public override SpellCircle Circle => SpellCircle.Eighth;
        public override VystiaMagicSchool MagicSchool => VystiaMagicSchool.Nature;

        public NatureHydraFormLegendaryShapeshiftSpell(Mobile caster, Item scroll) : base(caster, scroll, m_Info)
        {
        }

        public override bool CheckCast()
        {
            if (!base.CheckCast())
                return false;            // Vystia Reagent Check
            if (!Caster.Backpack.ConsumeTotal(typeof(LivingBark), 1) || !Caster.Backpack.ConsumeTotal(typeof(AncientRoot), 1) || !Caster.Backpack.ConsumeTotal(typeof(AncientRoot), 1))
            {
                Caster.SendMessage("You do not have enough reagents to cast this spell.");
                Caster.SendMessage("Required: LivingBark (1), AncientRoot (1), AncientRoot (1)");
                return false;
            }



            return true;
        }

        public override void OnCast()
        {
            if (CheckSequence())
            {
                // Check if already in hydra form - toggle off
                if (Caster.BodyMod == 33) // Serpent/Hydra-like body
                {
                    RemoveHydraForm(Caster);
                    Caster.SendMessage(0x3B2, "You return to your normal form.");
                }
                else
                {
                    ApplyHydraForm(Caster);
                }
            }

            FinishSequence();
        }

        private void ApplyHydraForm(Mobile m)
        {
            // Visual effect
            m.FixedParticles(0x3728, 10, 15, 5038, 0x21, 0, EffectLayer.Waist);
            m.PlaySound(0x16A); // Monster roar

            // Transform into hydra
            m.BodyMod = 33; // Serpentine dragon (closest to hydra)
            m.HueMod = 0x48E; // Poisonous green hue

            // Calculate duration based on skill (3-5 minutes)
            double duration = 3.0 + (Caster.Skills.Magery.Value / 60.0);

            // +70 STR buff
            m.AddStatMod(new StatMod(StatType.Str, "HydraForm_Str", 70, TimeSpan.FromMinutes(duration)));

            // +30 DEX buff
            m.AddStatMod(new StatMod(StatType.Dex, "HydraForm_Dex", 30, TimeSpan.FromMinutes(duration)));

            // Poison immunity (100% poison resist)
            ResistanceMod poisMod = new ResistanceMod(ResistanceType.Poison, 70);
            m.AddResistanceMod(poisMod);

            m.SendMessage(0x3B2, "You transform into a legendary hydra! (+70 STR, +30 DEX, poison immune, regenerating)");
            m.SendMessage(0x22, "You cannot cast spells while in hydra form!");

            // Start HP regeneration timer (15 HP every 2 seconds)
            new HydraRegenTimer(m, duration).Start();

            // Auto-revert after duration
            Timer.DelayCall(TimeSpan.FromMinutes(duration), () =>
            {
                if (m != null && !m.Deleted && m.BodyMod == 33)
                {
                    m.RemoveResistanceMod(poisMod);
                    RemoveHydraForm(m);
                    m.SendMessage("Your hydra form fades.");
                }
            });
        }

        private class HydraRegenTimer : Timer
        {
            private Mobile m_Mobile;
            private DateTime m_EndTime;

            public HydraRegenTimer(Mobile m, double durationMinutes)
                : base(TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(2))
            {
                m_Mobile = m;
                m_EndTime = DateTime.UtcNow + TimeSpan.FromMinutes(durationMinutes);
            }

            protected override void OnTick()
            {
                if (m_Mobile == null || m_Mobile.Deleted || !m_Mobile.Alive ||
                    m_Mobile.BodyMod != 33 || DateTime.UtcNow >= m_EndTime)
                {
                    Stop();
                    return;
                }

                // Regenerate 15 HP per tick
                m_Mobile.Heal(15);
                m_Mobile.FixedParticles(0x376A, 1, 8, 5016, 0x21, 0, EffectLayer.Waist);
            }
        }

        private static void RemoveHydraForm(Mobile m)
        {
            m.BodyMod = 0;
            m.HueMod = -1;

            // Remove stat mods
            m.RemoveStatMod("HydraForm_Str");
            m.RemoveStatMod("HydraForm_Dex");

            // Visual effect
            m.FixedParticles(0x3728, 10, 15, 5038, 0x21, 0, EffectLayer.Waist);
            m.PlaySound(0x1FE);
        }
    }
}
