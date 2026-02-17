using System;
using Server;
using Server.Targeting;
using Server.Mobiles;
using Server.Spells;
using Server.Items;

using Server.Spells.VystiaSpells;
namespace Server.Spells.VystiaSpells.Dark
{
    /// <summary>
    /// Summon Succubus - Summon Succubus
    /// Circle: 6 (24 mana)
    /// </summary>
    public class DarkSummonSuccubusSpell : VystiaSpell
    {
        private static readonly SpellInfo m_Info = new SpellInfo(
            "Summon Succubus", "Summonum Succubusum",
            203,
            9041,
            false
        );

        public override SpellCircle Circle => SpellCircle.Sixth;
        public override VystiaMagicSchool MagicSchool => VystiaMagicSchool.Dark;

        public DarkSummonSuccubusSpell(Mobile caster, Item scroll) : base(caster, scroll, m_Info)
        {
        }

        public override bool CheckCast()
        {
            if (!base.CheckCast())
                return false;            // Vystia Reagent Check
            if (!Caster.Backpack.ConsumeTotal(typeof(VoidSilk), 1) || !Caster.Backpack.ConsumeTotal(typeof(DemonHeart), 1))
            {
                Caster.SendMessage("You do not have enough reagents to cast this spell.");
                Caster.SendMessage("Required: VoidSilk (1), DemonHeart (1)");
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

                SpellHelper.Summon(creature, Caster, 0x1FB, duration, false, false);
            }

            FinishSequence();
        }
    }
}
