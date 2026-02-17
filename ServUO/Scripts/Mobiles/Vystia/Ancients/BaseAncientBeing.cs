using System;
using Server;
using Server.Items;
using Server.Mobiles;

namespace Server.Mobiles
{
    /// <summary>
    /// Base class for all Vystia Ancient Beings
    /// Provides quest giver, recipe teaching, and divine blessing functionality
    /// </summary>
    public abstract class BaseAncientBeing : BaseCreature
    {
        #region Ancient Being Types

        public enum AncientBeingRole
        {
            QuestGiver,
            RecipeTeacher,
            DivineBlessing,
            RiddleMaster
        }

        /// <summary>
        /// The role(s) this ancient being fulfills
        /// </summary>
        public abstract AncientBeingRole[] Roles { get; }

        /// <summary>
        /// Whether this being is a quest giver
        /// </summary>
        public bool IsQuestGiver => Array.IndexOf(Roles, AncientBeingRole.QuestGiver) >= 0;

        /// <summary>
        /// Whether this being teaches recipes
        /// </summary>
        public bool IsRecipeTeacher => Array.IndexOf(Roles, AncientBeingRole.RecipeTeacher) >= 0;

        /// <summary>
        /// Whether this being grants divine blessings
        /// </summary>
        public bool IsDivineBlessingGiver => Array.IndexOf(Roles, AncientBeingRole.DivineBlessing) >= 0;

        /// <summary>
        /// Whether this being is a riddle master
        /// </summary>
        public bool IsRiddleMaster => Array.IndexOf(Roles, AncientBeingRole.RiddleMaster) >= 0;

        #endregion

        #region Religion

        /// <summary>
        /// The religion associated with this ancient being
        /// </summary>
        public abstract Server.Custom.VystiaClasses.Religion.VystiaReligion AssociatedReligion { get; }

        #endregion

        #region Constructor

        public BaseAncientBeing(AIType ai, FightMode fightMode, int rangePerception, int rangeFight, double activeSpeed, double passiveSpeed)
            : base(ai, fightMode, rangePerception, rangeFight, activeSpeed, passiveSpeed)
        {
        }

        public BaseAncientBeing(Serial serial)
            : base(serial)
        {
            // Ancient beings are typically non-aggressive
            FightMode = FightMode.None;
            Tamable = false;
        }

        #endregion

        #region Quest Giver

        /// <summary>
        /// Offer quests to a player
        /// Override to provide quest-specific logic
        /// </summary>
        public virtual void OfferQuests(PlayerMobile pm)
        {
            if (!IsQuestGiver)
                return;

            // TODO: Integrate with quest system to offer religion-specific quests
            Say("I have quests for those who follow my path.");
        }

        #endregion

        #region Recipe Teaching

        /// <summary>
        /// Teach recipes to a player
        /// Override to provide recipe-specific logic
        /// </summary>
        public virtual void TeachRecipes(PlayerMobile pm)
        {
            if (!IsRecipeTeacher)
                return;

            // TODO: Integrate with recipe teaching system
            Say("I can teach you ancient crafting techniques, if you prove worthy.");
        }

        #endregion

        #region Divine Blessings

        /// <summary>
        /// Grant a divine blessing to a player (Champion tier only)
        /// </summary>
        public virtual void GrantDivineBlessing(PlayerMobile pm)
        {
            if (!IsDivineBlessingGiver)
                return;

            var pietyTier = Server.Custom.VystiaClasses.Religion.VystiaPiety.GetPietyTier(pm);
            if (pietyTier < Server.Custom.VystiaClasses.Religion.PietyTier.Exalted) // Exalted = 900+ piety
            {
                Say("Only the most devoted Champions may receive my blessing.");
                return;
            }

            var playerReligion = Server.Custom.VystiaClasses.Religion.VystiaPiety.GetReligion(pm);
            if (playerReligion != AssociatedReligion)
            {
                Say("You do not follow my path. I cannot bless you.");
                return;
            }

            // Grant blessing using the divine blessing system
            VystiaDivineBlessingSystem.GrantBlessing(pm, this);
        }

        #endregion

        #region Serialization

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }

        #endregion

        #region Interaction

        public override void OnDoubleClick(Mobile from)
        {
            if (from is PlayerMobile pm)
            {
                if (IsQuestGiver)
                {
                    OfferQuests(pm);
                }
                else if (IsRecipeTeacher)
                {
                    TeachRecipes(pm);
                }
                else if (IsDivineBlessingGiver)
                {
                    GrantDivineBlessing(pm);
                }
                else
                {
                    base.OnDoubleClick(from);
                }
            }
            else
            {
                base.OnDoubleClick(from);
            }
        }

        #endregion
    }
}
