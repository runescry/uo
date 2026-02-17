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
    /// Summon Frenzy - Summon Frenzy
    /// Circle: 4 (16 mana)
    /// </summary>
    public class SummoningSummonFrenzySpell : VystiaSpell
    {
        private static readonly SpellInfo m_Info = new SpellInfo(
            "Summon Frenzy", "Summonum Frenzyum",
            203,
            9041,
            false
        );

        public override SpellCircle Circle => SpellCircle.Fourth;
        public override VystiaMagicSchool MagicSchool => VystiaMagicSchool.Summoning;

        public SummoningSummonFrenzySpell(Mobile caster, Item scroll) : base(caster, scroll, m_Info)
        {
        }

        public override bool CheckCast()
        {
            if (!base.CheckCast())
                return false;            // Vystia Reagent Check
            if (!Caster.Backpack.ConsumeTotal(typeof(SummoningCrystal), 1) || !Caster.Backpack.ConsumeTotal(typeof(ChaosShard), 1))
            {
                Caster.SendMessage("You do not have enough reagents to cast this spell.");
                Caster.SendMessage("Required: SummoningCrystal (1), ChaosShard (1)");
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
                BaseCreature creature = new GreyWolf();
                TimeSpan duration = TimeSpan.FromMinutes(10);

                SpellHelper.Summon(creature, Caster, 0x215, duration, false, false);
            }

            FinishSequence();
        }
    }
}
