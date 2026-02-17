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
    /// Detect Life - Reveals hidden creatures within 12 tiles, shows HP bars
    /// Circle: 1 (4 mana)
    /// </summary>
    public class NatureDetectLifeSpell : VystiaSpell
    {
        private static readonly SpellInfo m_Info = new SpellInfo(
            "Detect Life", "Detectum Vita",
            203,
            9041,
            false
        );

        public override SpellCircle Circle => SpellCircle.First;
        public override VystiaMagicSchool MagicSchool => VystiaMagicSchool.Nature;

        public NatureDetectLifeSpell(Mobile caster, Item scroll) : base(caster, scroll, m_Info)
        {
        }

        public override bool CheckCast()
        {
            if (!base.CheckCast())
                return false;            // Vystia Reagent Check
            if (!Caster.Backpack.ConsumeTotal(typeof(WildMoss), 1) || !Caster.Backpack.ConsumeTotal(typeof(Moonpetal), 1))
            {
                Caster.SendMessage("You do not have enough reagents to cast this spell.");
                Caster.SendMessage("Required: WildMoss (1), Moonpetal (1)");
                return false;
            }



            return true;
        }

        public override void OnCast()
        {
            if (CheckSequence())
            {
                // Visual effect on caster
                Caster.FixedParticles(0x375A, 10, 30, 5013, 0x21, 0, EffectLayer.Waist);
                Caster.PlaySound(0x1EA);

                Caster.SendMessage(0x3B2, "You sense the life force of nearby creatures...");

                int revealed = 0;
                int detected = 0;

                // Get all mobiles within 12 tiles
                IPooledEnumerable eable = Caster.Map.GetMobilesInRange(Caster.Location, 12);

                foreach (Mobile m in eable)
                {
                    if (m == Caster)
                        continue;

                    detected++;

                    // Reveal hidden creatures
                    if (m.Hidden)
                    {
                        m.RevealingAction();
                        m.FixedParticles(0x375A, 10, 15, 5013, 0x21, 0, EffectLayer.Waist);
                        m.SendMessage(0x22, "You have been detected!");
                        revealed++;
                    }

                    // Show a brief visual on each detected creature
                    m.FixedParticles(0x373A, 1, 10, 5007, 0x21, 0, EffectLayer.Head);
                }

                eable.Free();

                if (revealed > 0)
                    Caster.SendMessage(0x3B2, $"You reveal {revealed} hidden creatures!");

                Caster.SendMessage(0x3B2, $"You detect {detected} living creatures within range.");
            }

            FinishSequence();
        }
    }
}
