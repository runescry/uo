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
    /// Force of Nature - Shapeshifts rapidly between all forms (changes every 5s), gains all benefits simultaneously, can still cast spells
    /// Circle: 7 (40 mana)
    /// </summary>
    public class NatureForceofNatureSpell : VystiaSpell
    {
        private static readonly SpellInfo m_Info = new SpellInfo(
            "Force of Nature", "Forceus Ofum Natureus",
            203,
            9041,
            false
        );

        public override SpellCircle Circle => SpellCircle.Seventh;
        public override VystiaMagicSchool MagicSchool => VystiaMagicSchool.Nature;

        public NatureForceofNatureSpell(Mobile caster, Item scroll) : base(caster, scroll, m_Info)
        {
        }

        public override bool CheckCast()
        {
            if (!base.CheckCast())
                return false;            // Vystia Reagent Check
            if (!Caster.Backpack.ConsumeTotal(typeof(LivingBark), 1) || !Caster.Backpack.ConsumeTotal(typeof(AncientRoot), 1))
            {
                Caster.SendMessage("You do not have enough reagents to cast this spell.");
                Caster.SendMessage("Required: LivingBark (1), AncientRoot (1)");
                return false;
            }



            return true;
        }

        // Body IDs for different forms
        private static readonly int[] FormBodies = { 225, 213, 5, 301 }; // Wolf, Bear, Hawk, Treant
        private static readonly string[] FormNames = { "Wolf", "Bear", "Hawk", "Treant" };

        public override void OnCast()
        {
            if (CheckSequence())
            {
                // Check if already in force of nature - toggle off
                if (Caster.BodyMod != 0 && IsForceOfNatureActive(Caster))
                {
                    RemoveForceOfNature(Caster);
                    Caster.SendMessage(0x3B2, "The force of nature subsides.");
                }
                else
                {
                    ApplyForceOfNature(Caster);
                }
            }

            FinishSequence();
        }

        private static bool IsForceOfNatureActive(Mobile m)
        {
            // Check if the stat mod exists
            foreach (StatMod mod in m.StatMods)
            {
                if (mod.Name.StartsWith("ForceOfNature_"))
                    return true;
            }
            return false;
        }

        private void ApplyForceOfNature(Mobile m)
        {
            // Visual effect
            m.FixedParticles(0x3728, 10, 30, 5038, 0x21, 0, EffectLayer.Waist);
            m.PlaySound(0x1F2);

            // Calculate duration based on skill (2-4 minutes)
            double duration = 2.0 + (Caster.Skills.Magery.Value / 60.0);

            // Apply combined stat bonuses (all forms at once)
            // Wolf: +25 DEX, Bear: +30 STR, Hawk: +40 DEX, Treant: +50 STR
            // Combined: +80 STR, +65 DEX
            m.AddStatMod(new StatMod(StatType.Str, "ForceOfNature_Str", 80, TimeSpan.FromMinutes(duration)));
            m.AddStatMod(new StatMod(StatType.Dex, "ForceOfNature_Dex", 65, TimeSpan.FromMinutes(duration)));

            // Combined resistances: +15 Physical (bear), +30 Physical (treant) = +45 Physical
            ResistanceMod physMod = new ResistanceMod(ResistanceType.Physical, 45);
            m.AddResistanceMod(physMod);

            m.SendMessage(0x3B2, "You become one with the force of nature! (+80 STR, +65 DEX, +45 Physical Resist)");
            m.SendMessage(0x3B2, "You can still cast spells while channeling nature's power!");

            // Start form-shifting timer (changes appearance every 5 seconds)
            new FormShiftTimer(m, duration, physMod).Start();
        }

        private class FormShiftTimer : Timer
        {
            private Mobile m_Mobile;
            private DateTime m_EndTime;
            private int m_CurrentFormIndex;
            private ResistanceMod m_PhysMod;

            public FormShiftTimer(Mobile m, double durationMinutes, ResistanceMod physMod)
                : base(TimeSpan.FromSeconds(0), TimeSpan.FromSeconds(5))
            {
                m_Mobile = m;
                m_EndTime = DateTime.UtcNow + TimeSpan.FromMinutes(durationMinutes);
                m_CurrentFormIndex = 0;
                m_PhysMod = physMod;
            }

            protected override void OnTick()
            {
                if (m_Mobile == null || m_Mobile.Deleted || !m_Mobile.Alive || DateTime.UtcNow >= m_EndTime)
                {
                    if (m_Mobile != null && !m_Mobile.Deleted)
                    {
                        m_Mobile.RemoveResistanceMod(m_PhysMod);
                        RemoveForceOfNature(m_Mobile);
                        m_Mobile.SendMessage("The force of nature fades.");
                    }
                    Stop();
                    return;
                }

                // Shift to next form
                m_Mobile.BodyMod = FormBodies[m_CurrentFormIndex];
                m_Mobile.FixedParticles(0x3728, 5, 10, 5038, 0x21, 0, EffectLayer.Waist);
                m_Mobile.SendMessage(0x3B2, $"You shift into {FormNames[m_CurrentFormIndex]} aspect!");

                m_CurrentFormIndex = (m_CurrentFormIndex + 1) % FormBodies.Length;
            }
        }

        private static void RemoveForceOfNature(Mobile m)
        {
            m.BodyMod = 0;
            m.HueMod = -1;

            // Remove stat mods
            m.RemoveStatMod("ForceOfNature_Str");
            m.RemoveStatMod("ForceOfNature_Dex");

            // Visual effect
            m.FixedParticles(0x3728, 10, 15, 5038, 0x21, 0, EffectLayer.Waist);
            m.PlaySound(0x1FE);
        }
    }
}
