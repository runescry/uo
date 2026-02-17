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
    /// Summon Undead Dragon - Summon Undead Dragon
    /// Circle: 8 (32 mana)
    /// </summary>
    public class NecromancySummonUndeadDragonSpell : VystiaSpell
    {
        private static readonly SpellInfo m_Info = new SpellInfo(
            "Summon Undead Dragon", "Summonum Undeadum Dragonum",
            203,
            9041,
            false
        );

        public override SpellCircle Circle => SpellCircle.Eighth;
        public override VystiaMagicSchool MagicSchool => VystiaMagicSchool.Necromancy;

        public NecromancySummonUndeadDragonSpell(Mobile caster, Item scroll) : base(caster, scroll, m_Info)
        {
        }

        public override bool CheckCast()
        {
            if (!base.CheckCast())
                return false;            // Vystia Reagent Check
            if (!Caster.Backpack.ConsumeTotal(typeof(PhylacteryShard), 1) || !Caster.Backpack.ConsumeTotal(typeof(ReaperEssence), 1) || !Caster.Backpack.ConsumeTotal(typeof(ReaperEssence), 1))
            {
                Caster.SendMessage("You do not have enough reagents to cast this spell.");
                Caster.SendMessage("Required: PhylacteryShard (1), ReaperEssence (1), ReaperEssence (1)");
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
