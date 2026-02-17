using System;
using Server;
using Server.Targeting;
using Server.Mobiles;
using Server.Spells;
using Server.Items;

using Server.Spells.VystiaSpells;
namespace Server.Spells.VystiaSpells.Shamanic
{
    /// <summary>
    /// Ghost Wolf Form - Ghost Wolf Form
    /// Circle: 1 (4 mana)
    /// </summary>
    public class ShamanicGhostWolfFormSpell : VystiaSpell
    {
        private static readonly SpellInfo m_Info = new SpellInfo(
            "Ghost Wolf Form", "Ghostum Wolfum Formum",
            203,
            9041,
            false
        );

        public override SpellCircle Circle => SpellCircle.First;
        public override VystiaMagicSchool MagicSchool => VystiaMagicSchool.Shamanic;

        public ShamanicGhostWolfFormSpell(Mobile caster, Item scroll) : base(caster, scroll, m_Info)
        {
        }

        public override bool CheckCast()
        {
            if (!base.CheckCast())
                return false;            // Vystia Reagent Check
            if (!Caster.Backpack.ConsumeTotal(typeof(LightningRoot), 1) || !Caster.Backpack.ConsumeTotal(typeof(ThunderMoss), 1))
            {
                Caster.SendMessage("You do not have enough reagents to cast this spell.");
                Caster.SendMessage("Required: LightningRoot (1), ThunderMoss (1)");
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
                    RemoveShamanicGhostWolfForm(Caster);
                    Caster.SendMessage(0x3B2, "You return to your normal form.");
                }
                else
                {
                    ApplyShamanicGhostWolfForm(Caster);
                }
            }

            FinishSequence();
        }

        private void ApplyShamanicGhostWolfForm(Mobile m)
        {
            // Visual effect
            m.FixedParticles(0x3728, 10, 15, 5038, 0x21, 0, EffectLayer.Waist);
            m.PlaySound(0x1FE);

            // Transform
            m.BodyMod = 225;
            m.HueMod = 0;

            // Calculate duration (3-6 minutes)
            double duration = 3.0 + (Caster.Skills.Magery.Value / 40.0);

            // Ghost Wolf Form - Ghost Wolf Form Circle: 1 (4 mana)
            m.AddStatMod(new StatMod(StatType.Dex, "ShamanicGhostWolfForm_Dex", 25, TimeSpan.FromMinutes(duration)));

            m.SendMessage(0x3B2, "You transform! (+25 DEX)");
            m.SendMessage(0x22, "You cannot cast spells in this form!");

            // Auto-revert after duration
            Timer.DelayCall(TimeSpan.FromMinutes(duration), () =>
            {
                if (m != null && !m.Deleted && m.BodyMod == 225)
                {
                    RemoveShamanicGhostWolfForm(m);
                    m.SendMessage("Your transformation fades.");
                }
            });
        }

        private static void RemoveShamanicGhostWolfForm(Mobile m)
        {
            m.BodyMod = 0;
            m.HueMod = -1;
            m.RemoveStatMod("ShamanicGhostWolfForm_Dex");

            m.FixedParticles(0x3728, 10, 15, 5038, 0x21, 0, EffectLayer.Waist);
            m.PlaySound(0x1FE);
        }
    }
}
