using System;
using Server;
using Server.Targeting;
using Server.Mobiles;
using Server.Spells;
using Server.Items;

using Server.Spells.VystiaSpells;
namespace Server.Spells.VystiaSpells.Summoning
{
    /// <summary>
    /// Summon Hydra - Summon Hydra
    /// Circle: 5 (20 mana)
    /// </summary>
    public class SummoningSummonHydraSpell : VystiaSpell
    {
        private static readonly SpellInfo m_Info = new SpellInfo(
            "Summon Hydra", "Summonum Hydraum",
            203,
            9041,
            false
        );

        public override SpellCircle Circle => SpellCircle.Fifth;
        public override VystiaMagicSchool MagicSchool => VystiaMagicSchool.Summoning;

        public SummoningSummonHydraSpell(Mobile caster, Item scroll) : base(caster, scroll, m_Info)
        {
        }

        public override bool CheckCast()
        {
            if (!base.CheckCast())
                return false;            // Vystia Reagent Check
            if (!Caster.Backpack.ConsumeTotal(typeof(ChaosShard), 1) || !Caster.Backpack.ConsumeTotal(typeof(BindingRune), 1))
            {
                Caster.SendMessage("You do not have enough reagents to cast this spell.");
                Caster.SendMessage("Required: ChaosShard (1), BindingRune (1)");
                return false;
            }



            return true;
        }

        public override void OnCast()
        {
            if (CheckSequence())
            {
                // Check follower slots
                if (Caster.Followers + 2 > Caster.FollowersMax)
                {
                    Caster.SendLocalizedMessage(1049645); // You have too many followers
                    FinishSequence();
                    return;
                }

                // Create and summon creature
                BaseCreature creature = new SwampDragon();
                TimeSpan duration = TimeSpan.FromMinutes(10);

                SpellHelper.Summon(creature, Caster, 0x215, duration, false, false);
            }

            FinishSequence();
        }
    }
}
