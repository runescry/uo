using System;
using Server;
using Server.Targeting;
using Server.Mobiles;
using Server.Spells;
using Server.Items;

using Server.Spells.VystiaSpells;
namespace Server.Spells.VystiaSpells.Necromancy
{
    /// <summary>
    /// Army of the Dead - Army of the Dead
    /// Circle: 7 (28 mana)
    /// </summary>
    public class NecromancyArmyoftheDeadSpell : VystiaSpell
    {
        private static readonly SpellInfo m_Info = new SpellInfo(
            "Army of the Dead", "Armyum Ofum Theus Deadum",
            203,
            9041,
            false
        );

        public override SpellCircle Circle => SpellCircle.Seventh;
        public override VystiaMagicSchool MagicSchool => VystiaMagicSchool.Necromancy;

        public NecromancyArmyoftheDeadSpell(Mobile caster, Item scroll) : base(caster, scroll, m_Info)
        {
        }

        public override bool CheckCast()
        {
            if (!base.CheckCast())
                return false;            // Vystia Reagent Check
            if (!Caster.Backpack.ConsumeTotal(typeof(PhylacteryShard), 1) || !Caster.Backpack.ConsumeTotal(typeof(ReaperEssence), 1))
            {
                Caster.SendMessage("You do not have enough reagents to cast this spell.");
                Caster.SendMessage("Required: PhylacteryShard (1), ReaperEssence (1)");
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

                // Army of the Dead - Army of the Dead Circle: 7 (28 mana)
                // Note: Summon implementation requires creature definitions
                Caster.SendMessage(0x3B2, "You summon a creature to aid you!");
                Caster.SendMessage(0x22, "(Summon creature not yet implemented - needs creature class)");
            }

            FinishSequence();
        }
    }
}
