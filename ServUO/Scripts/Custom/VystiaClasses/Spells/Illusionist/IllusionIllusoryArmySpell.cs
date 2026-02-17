using System;
using Server;
using Server.Targeting;
using Server.Mobiles;
using Server.Spells;
using Server.Items;

using Server.Spells.VystiaSpells;
namespace Server.Spells.VystiaSpells.Illusion
{
    /// <summary>
    /// Illusory Army - Illusory Army
    /// Circle: 5 (20 mana)
    /// </summary>
    public class IllusionIllusoryArmySpell : VystiaSpell
    {
        private static readonly SpellInfo m_Info = new SpellInfo(
            "Illusory Army", "Illusoryum Armyum",
            203,
            9041,
            false
        );

        public override SpellCircle Circle => SpellCircle.Fifth;
        public override VystiaMagicSchool MagicSchool => VystiaMagicSchool.Illusion;

        public IllusionIllusoryArmySpell(Mobile caster, Item scroll) : base(caster, scroll, m_Info)
        {
        }

        public override bool CheckCast()
        {
            if (!base.CheckCast())
                return false;            // Vystia Reagent Check
            if (!Caster.Backpack.ConsumeTotal(typeof(RealitySplinter), 1) || !Caster.Backpack.ConsumeTotal(typeof(VoidMirror), 1))
            {
                Caster.SendMessage("You do not have enough reagents to cast this spell.");
                Caster.SendMessage("Required: RealitySplinter (1), VoidMirror (1)");
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

                // Illusory Army - Illusory Army Circle: 5 (20 mana)
                // Note: Summon implementation requires creature definitions
                Caster.SendMessage(0x3B2, "You summon a creature to aid you!");
                Caster.SendMessage(0x22, "(Summon creature not yet implemented - needs creature class)");
            }

            FinishSequence();
        }
    }
}
