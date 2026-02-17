using System;
using Server;
using Server.Targeting;
using Server.Mobiles;
using Server.Spells;
using Server.Items;

using Server.Spells.VystiaSpells;
namespace Server.Spells.VystiaSpells.Enchanting
{
    /// <summary>
    /// Enchant Army - Enchant Army
    /// Circle: 7 (28 mana)
    /// </summary>
    public class EnchantingEnchantArmySpell : VystiaSpell
    {
        private static readonly SpellInfo m_Info = new SpellInfo(
            "Enchant Army", "Enchantum Armyum",
            203,
            9041,
            false
        );

        public override SpellCircle Circle => SpellCircle.Seventh;
        public override VystiaMagicSchool MagicSchool => VystiaMagicSchool.Enchanting;

        public EnchantingEnchantArmySpell(Mobile caster, Item scroll) : base(caster, scroll, m_Info)
        {
        }

        public override bool CheckCast()
        {
            if (!base.CheckCast())
                return false;            // Vystia Reagent Check
            if (!Caster.Backpack.ConsumeTotal(typeof(RunicPowder), 1) || !Caster.Backpack.ConsumeTotal(typeof(TitanRune), 1))
            {
                Caster.SendMessage("You do not have enough reagents to cast this spell.");
                Caster.SendMessage("Required: RunicPowder (1), TitanRune (1)");
                return false;
            }



            return true;
        }

        public override void OnCast()
        {
            if (CheckSequence())
            {
                SpellHelper.Turn(Caster, Caster);

                // Visual effect
                Caster.FixedParticles(0x3728, 10, 15, 5038, 0x21, 0, EffectLayer.Waist);
                Caster.PlaySound(0x212);

                // Enchant Army - Enchant Army Circle: 7 (28 mana)
                // Note: Summon implementation requires creature definitions
                Caster.SendMessage(0x3B2, "You summon a creature to aid you!");
                Caster.SendMessage(0x22, "(Summon creature not yet implemented - needs creature class)");
            }

            FinishSequence();
        }
    }
}
