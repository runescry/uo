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
    /// Avatar of Summoning - Avatar of Summoning
    /// Circle: 8 (32 mana)
    /// </summary>
    public class SummoningAvatarofSummoningSpell : VystiaSpell
    {
        private static readonly SpellInfo m_Info = new SpellInfo(
            "Avatar of Summoning", "Avatarum Ofum Summoningum",
            203,
            9041,
            false
        );

        public override SpellCircle Circle => SpellCircle.Eighth;
        public override VystiaMagicSchool MagicSchool => VystiaMagicSchool.Summoning;

        public SummoningAvatarofSummoningSpell(Mobile caster, Item scroll) : base(caster, scroll, m_Info)
        {
        }

        public override bool CheckCast()
        {
            if (!base.CheckCast())
                return false;            // Vystia Reagent Check
            if (!Caster.Backpack.ConsumeTotal(typeof(DimensionalKey), 1) || !Caster.Backpack.ConsumeTotal(typeof(SummoningSalt), 1) || !Caster.Backpack.ConsumeTotal(typeof(SummoningSalt), 1))
            {
                Caster.SendMessage("You do not have enough reagents to cast this spell.");
                Caster.SendMessage("Required: DimensionalKey (1), SummoningSalt (1), SummoningSalt (1)");
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
