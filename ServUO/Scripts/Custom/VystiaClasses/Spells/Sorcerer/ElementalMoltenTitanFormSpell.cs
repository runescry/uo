using System;
using Server;
using Server.Targeting;
using Server.Mobiles;
using Server.Spells;
using Server.Items;

using Server.Spells.VystiaSpells;
namespace Server.Spells.VystiaSpells.Elemental
{
    /// <summary>
    /// Molten Titan Form - Molten Titan Form
    /// Circle: 6 (24 mana)
    /// </summary>
    public class ElementalMoltenTitanFormSpell : VystiaSpell
    {
        private static readonly SpellInfo m_Info = new SpellInfo(
            "Molten Titan Form", "Moltenum Titanum Formum",
            203,
            9041,
            false
        );

        public override SpellCircle Circle => SpellCircle.Sixth;
        public override VystiaMagicSchool MagicSchool => VystiaMagicSchool.Elemental;

        public ElementalMoltenTitanFormSpell(Mobile caster, Item scroll) : base(caster, scroll, m_Info)
        {
        }

        public override bool CheckCast()
        {
            if (!base.CheckCast())
                return false;            // Vystia Reagent Check
            if (!Caster.Backpack.ConsumeTotal(typeof(DragonHeart), 1) || !Caster.Backpack.ConsumeTotal(typeof(PrimordialEmber), 1))
            {
                Caster.SendMessage("You do not have enough reagents to cast this spell.");
                Caster.SendMessage("Required: DragonHeart (1), PrimordialEmber (1)");
                return false;
            }



            return true;
        }

        public override void OnCast()
        {
            if (CheckSequence())
            {
                // Toggle off if already in this form
                if (Caster.BodyMod == 189)
                {
                    RemoveElementalMoltenTitanForm(Caster);
                    Caster.SendMessage(0x3B2, "You return to your normal form.");
                }
                else
                {
                    ApplyElementalMoltenTitanForm(Caster);
                }
            }

            FinishSequence();
        }

        private void ApplyElementalMoltenTitanForm(Mobile m)
        {
            // Visual effect
            m.FixedParticles(0x3728, 10, 15, 5038, 0x21, 0, EffectLayer.Waist);
            m.PlaySound(0x1FE);

            // Transform
            m.BodyMod = 189;
            m.HueMod = 0;

            // Calculate duration (3-6 minutes)
            double duration = 3.0 + (Caster.Skills.Magery.Value / 40.0);

            // Molten Titan Form - Molten Titan Form Circle: 6 (24 mana)
            m.AddStatMod(new StatMod(StatType.Str, "ElementalMoltenTitanForm_Str", 100, TimeSpan.FromMinutes(duration)));

            m.SendMessage(0x3B2, "You transform! (+100 STR)");
            m.SendMessage(0x22, "You cannot cast spells in this form!");

            // Auto-revert after duration
            Timer.DelayCall(TimeSpan.FromMinutes(duration), () =>
            {
                if (m != null && !m.Deleted && m.BodyMod == 189)
                {
                    RemoveElementalMoltenTitanForm(m);
                    m.SendMessage("Your transformation fades.");
                }
            });
        }

        private static void RemoveElementalMoltenTitanForm(Mobile m)
        {
            m.BodyMod = 0;
            m.HueMod = -1;
            m.RemoveStatMod("ElementalMoltenTitanForm_Str");

            m.FixedParticles(0x3728, 10, 15, 5038, 0x21, 0, EffectLayer.Waist);
            m.PlaySound(0x1FE);
        }
    }
}
