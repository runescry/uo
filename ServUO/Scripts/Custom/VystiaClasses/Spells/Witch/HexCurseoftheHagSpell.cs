using System;
using Server;
using Server.Targeting;
using Server.Mobiles;
using Server.Spells;
using Server.Items;

using Server.Spells.VystiaSpells;
namespace Server.Spells.VystiaSpells.Hex
{
    /// <summary>
    /// Curse of the Hag - Target polymorphs into weak form (chicken), -90% damage, -50% movement, cannot cast spells
    /// Circle: 7 (40 mana)
    /// </summary>
    public class HexCurseoftheHagSpell : VystiaSpell
    {
        private static readonly SpellInfo m_Info = new SpellInfo(
            "Curse of the Hag", "Curseus Ofum Theus Hagum",
            203,
            9041,
            false
        );

        public override SpellCircle Circle => SpellCircle.Seventh;
        public override VystiaMagicSchool MagicSchool => VystiaMagicSchool.Hex;

        public HexCurseoftheHagSpell(Mobile caster, Item scroll) : base(caster, scroll, m_Info)
        {
        }

        public override bool CheckCast()
        {
            if (!base.CheckCast())
                return false;            // Vystia Reagent Check
            if (!Caster.Backpack.ConsumeTotal(typeof(CursedPearl), 1) || !Caster.Backpack.ConsumeTotal(typeof(CursedSalt), 1))
            {
                Caster.SendMessage("You do not have enough reagents to cast this spell.");
                Caster.SendMessage("Required: CursedPearl (1), CursedSalt (1)");
                return false;
            }



            return true;
        }

        public override void OnCast()
        {
            if (CheckSequence())
            {
                // Toggle off if already in this form
                if (Caster.BodyMod == 225)
                {
                    RemoveHexCurseoftheHag(Caster);
                    Caster.SendMessage(0x3B2, "You return to your normal form.");
                }
                else
                {
                    ApplyHexCurseoftheHag(Caster);
                }
            }

            FinishSequence();
        }

        private void ApplyHexCurseoftheHag(Mobile m)
        {
            // Visual effect
            m.FixedParticles(0x3728, 10, 15, 5038, 0x21, 0, EffectLayer.Waist);
            m.PlaySound(0x1FE);

            // Transform
            m.BodyMod = 225;
            m.HueMod = 0;

            // Calculate duration (3-6 minutes)
            double duration = 3.0 + (Caster.Skills.Magery.Value / 40.0);

            // Curse of the Hag - Target polymorphs into weak form (chicken), -90% damage, -50% movement, cannot cast spells Circle: 7 (40 mana)
            m.AddStatMod(new StatMod(StatType.Str, "HexCurseoftheHag_Str", 30, TimeSpan.FromMinutes(duration)));

            m.SendMessage(0x3B2, "You transform! (+30 STR)");
            m.SendMessage(0x22, "You cannot cast spells in this form!");

            // Auto-revert after duration
            Timer.DelayCall(TimeSpan.FromMinutes(duration), () =>
            {
                if (m != null && !m.Deleted && m.BodyMod == 225)
                {
                    RemoveHexCurseoftheHag(m);
                    m.SendMessage("Your transformation fades.");
                }
            });
        }

        private static void RemoveHexCurseoftheHag(Mobile m)
        {
            m.BodyMod = 0;
            m.HueMod = -1;
            m.RemoveStatMod("HexCurseoftheHag_Str");

            m.FixedParticles(0x3728, 10, 15, 5038, 0x21, 0, EffectLayer.Waist);
            m.PlaySound(0x1FE);
        }
    }
}
