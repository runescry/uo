using System;
using System.Collections.Generic;
using Server;
using Server.Mobiles;
using Server.Custom.VystiaClasses.Factions;

namespace Server.Mobiles
{
    /// <summary>
    /// Recipe Teaching System
    /// Allows ancient beings to teach faction/piety-gated recipes to players
    /// </summary>
    public static class VystiaRecipeTeachingSystem
    {
        /// <summary>
        /// Teach a recipe to a player if they meet requirements
        /// </summary>
        public static bool TeachRecipe(PlayerMobile pm, string recipeName, VystiaFaction requiredFaction, ReputationTier requiredTier, 
            Server.Custom.VystiaClasses.Religion.VystiaReligion requiredReligion, Server.Custom.VystiaClasses.Religion.PietyTier requiredPiety)
        {
            if (pm == null)
                return false;

            // Check faction requirement
            if (requiredFaction != VystiaFaction.None)
            {
                var playerTier = VystiaReputation.GetFactionTier(pm, requiredFaction);
                if (playerTier < requiredTier)
                {
                    pm.SendMessage($"You must be {requiredTier} with {requiredFaction} to learn this recipe.");
                    return false;
                }
            }

            // Check religion/piety requirement
            if (requiredReligion != Server.Custom.VystiaClasses.Religion.VystiaReligion.None)
            {
                var playerReligion = Server.Custom.VystiaClasses.Religion.VystiaPiety.GetReligion(pm);
                if (playerReligion != requiredReligion)
                {
                    pm.SendMessage($"You must follow {requiredReligion} to learn this recipe.");
                    return false;
                }

                var playerPiety = Server.Custom.VystiaClasses.Religion.VystiaPiety.GetPietyTier(pm);
                if (playerPiety < requiredPiety)
                {
                    pm.SendMessage($"You must be {requiredPiety} tier to learn this recipe.");
                    return false;
                }
            }

            // TODO: Actually teach the recipe (integrate with crafting system)
            pm.SendMessage(0x35, "You have learned the recipe: {0}!", recipeName);
            return true;
        }

        /// <summary>
        /// Get available recipes for an ancient being
        /// </summary>
        public static List<RecipeInfo> GetAvailableRecipes(BaseAncientBeing ancientBeing)
        {
            // TODO: Return recipes based on ancient being type
            return new List<RecipeInfo>();
        }
    }

    /// <summary>
    /// Recipe information
    /// </summary>
    public class RecipeInfo
    {
        public string Name { get; set; }
        public VystiaFaction RequiredFaction { get; set; }
        public ReputationTier RequiredTier { get; set; }
        public Server.Custom.VystiaClasses.Religion.VystiaReligion RequiredReligion { get; set; }
        public Server.Custom.VystiaClasses.Religion.PietyTier RequiredPiety { get; set; }
    }
}
