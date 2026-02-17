// Vystia Tier-Gated Recipe Access System
// Restricts certain crafting recipes to players with required faction/piety tiers

using System;
using System.Collections.Generic;
using Server;
using Server.Items;
using Server.Mobiles;
using Server.Engines.Craft;
using Server.Custom.VystiaClasses.Factions;
using Server.Custom.VystiaClasses.Religion;

namespace Server.Custom.VystiaClasses.Crafting
{
    #region Recipe Requirement Types

    /// <summary>
    /// Types of tier requirements for recipes
    /// </summary>
    public enum RecipeRequirementType
    {
        None,
        FactionReputation,
        ReligionPiety,
        Both
    }

    #endregion

    #region Recipe Requirement Data

    /// <summary>
    /// Defines the tier requirements for a specific recipe
    /// </summary>
    public class RecipeRequirement
    {
        public Type ItemType { get; set; }
        public RecipeRequirementType RequirementType { get; set; }

        // Faction requirements
        public VystiaFaction RequiredFaction { get; set; }
        public ReputationTier MinimumReputationTier { get; set; }

        // Religion requirements
        public VystiaReligion RequiredReligion { get; set; }
        public PietyTier MinimumPietyTier { get; set; }

        // Message to display when requirement not met
        public string FailureMessage { get; set; }

        public RecipeRequirement(Type itemType)
        {
            ItemType = itemType;
            RequirementType = RecipeRequirementType.None;
            RequiredFaction = VystiaFaction.None;
            MinimumReputationTier = ReputationTier.Neutral;
            RequiredReligion = VystiaReligion.None;
            MinimumPietyTier = PietyTier.None;
            FailureMessage = "You do not meet the requirements to craft this item.";
        }
    }

    #endregion

    #region Tier Gated Recipe System

    /// <summary>
    /// System for managing tier-gated recipes
    /// Checks faction reputation and religion piety before allowing crafting
    /// </summary>
    public static class TierGatedRecipeSystem
    {
        private static readonly Dictionary<Type, RecipeRequirement> s_Requirements = new Dictionary<Type, RecipeRequirement>();

        /// <summary>
        /// Initialize the tier-gated recipe system with default recipes
        /// </summary>
        public static void Initialize()
        {
            Console.WriteLine("[Vystia] Initializing Tier-Gated Recipe System...");

            // ============================================
            // FACTION-GATED RECIPES
            // ============================================

            // Frostguard faction recipes
            AddFactionRequirement(typeof(Server.Items.Vystia.FrostguardToken), VystiaFaction.Frostguard, ReputationTier.Friendly, "You must be Friendly with the Frostguard to craft this.");

            // Flame Legion faction recipes
            AddFactionRequirement(typeof(Server.Items.Vystia.FlameLegionToken), VystiaFaction.FlameLegion, ReputationTier.Friendly, "You must be Friendly with the Flame Legion to craft this.");

            // Greenward faction recipes
            AddFactionRequirement(typeof(Server.Items.Vystia.GreenwardToken), VystiaFaction.Greenward, ReputationTier.Friendly, "You must be Friendly with the Greenward to craft this.");

            // Arcane Conclave faction recipes
            AddFactionRequirement(typeof(Server.Items.Vystia.ArcaneConclaveToken), VystiaFaction.ArcaneConclave, ReputationTier.Friendly, "You must be Friendly with the Arcane Conclave to craft this.");

            // Technoguild faction recipes
            AddFactionRequirement(typeof(Server.Items.Vystia.TechnoguildToken), VystiaFaction.Technoguild, ReputationTier.Friendly, "You must be Friendly with the Technoguild to craft this.");

            // Sandwalkers faction recipes
            AddFactionRequirement(typeof(Server.Items.Vystia.SandwalkersToken), VystiaFaction.Sandwalkers, ReputationTier.Friendly, "You must be Friendly with the Sandwalkers to craft this.");

            // Voidborn faction recipes
            AddFactionRequirement(typeof(Server.Items.Vystia.VoidbornToken), VystiaFaction.Voidborn, ReputationTier.Friendly, "You must be Friendly with the Voidborn to craft this.");

            // ============================================
            // RELIGION-GATED RECIPES (Portable Shrines)
            // ============================================

            AddReligionRequirement(typeof(Server.Items.Vystia.FrostTotemShrine), VystiaReligion.FrosthelmFaith, PietyTier.Devoted, "You must be Devoted to the Frosthelm Faith to craft this.");
            AddReligionRequirement(typeof(Server.Items.Vystia.SunDialShrine), VystiaReligion.SuryasSandscript, PietyTier.Devoted, "You must be Devoted to the Surya's Sandscript to craft this.");
            AddReligionRequirement(typeof(Server.Items.Vystia.MoonstoneCircle), VystiaReligion.LunarasCovenant, PietyTier.Devoted, "You must be Devoted to the Lunara's Covenant to craft this.");
            AddReligionRequirement(typeof(Server.Items.Vystia.StarChartShrine), VystiaReligion.CelestisArcanum, PietyTier.Devoted, "You must be Devoted to the Celestis Arcanum to craft this.");
            AddReligionRequirement(typeof(Server.Items.Vystia.TidePoolBasin), VystiaReligion.OceanasCovenant, PietyTier.Devoted, "You must be Devoted to the Oceana's Covenant to craft this.");
            AddReligionRequirement(typeof(Server.Items.Vystia.CogsmithePortableAnvil), VystiaReligion.CogsmithCreed, PietyTier.Devoted, "You must be Devoted to the Cogsmith Creed to craft this.");

            Console.WriteLine("[Vystia] Tier-Gated Recipe System initialized with {0} gated recipes.", s_Requirements.Count);
        }

        /// <summary>
        /// Add a faction reputation requirement to a recipe
        /// </summary>
        public static void AddFactionRequirement(Type itemType, VystiaFaction faction, ReputationTier minTier, string failureMessage = null)
        {
            if (itemType == null || faction == VystiaFaction.None)
                return;

            var req = new RecipeRequirement(itemType)
            {
                RequirementType = RecipeRequirementType.FactionReputation,
                RequiredFaction = faction,
                MinimumReputationTier = minTier,
                FailureMessage = failureMessage ?? $"You must be {minTier} with {FactionData.GetInfo(faction)?.Name} to craft this."
            };

            s_Requirements[itemType] = req;
        }

        /// <summary>
        /// Add a religion piety requirement to a recipe
        /// </summary>
        public static void AddReligionRequirement(Type itemType, VystiaReligion religion, PietyTier minTier, string failureMessage = null)
        {
            if (itemType == null || religion == VystiaReligion.None)
                return;

            var req = new RecipeRequirement(itemType)
            {
                RequirementType = RecipeRequirementType.ReligionPiety,
                RequiredReligion = religion,
                MinimumPietyTier = minTier,
                FailureMessage = failureMessage ?? $"You must be {minTier} in {ReligionData.GetInfo(religion)?.Name} to craft this."
            };

            s_Requirements[itemType] = req;
        }

        /// <summary>
        /// Add both faction and religion requirements to a recipe
        /// </summary>
        public static void AddDualRequirement(Type itemType, VystiaFaction faction, ReputationTier repTier, VystiaReligion religion, PietyTier pietyTier, string failureMessage = null)
        {
            if (itemType == null)
                return;

            var req = new RecipeRequirement(itemType)
            {
                RequirementType = RecipeRequirementType.Both,
                RequiredFaction = faction,
                MinimumReputationTier = repTier,
                RequiredReligion = religion,
                MinimumPietyTier = pietyTier,
                FailureMessage = failureMessage ?? "You do not meet the faction and religion requirements to craft this."
            };

            s_Requirements[itemType] = req;
        }

        /// <summary>
        /// Check if a player can craft a specific item type
        /// </summary>
        public static bool CanCraft(PlayerMobile pm, Type itemType, out string failureMessage)
        {
            failureMessage = null;

            if (pm == null || itemType == null)
                return true; // No check needed

            if (!s_Requirements.TryGetValue(itemType, out var req))
                return true; // No requirements for this item

            switch (req.RequirementType)
            {
                case RecipeRequirementType.FactionReputation:
                    return CheckFactionRequirement(pm, req, out failureMessage);

                case RecipeRequirementType.ReligionPiety:
                    return CheckReligionRequirement(pm, req, out failureMessage);

                case RecipeRequirementType.Both:
                    if (!CheckFactionRequirement(pm, req, out failureMessage))
                        return false;
                    return CheckReligionRequirement(pm, req, out failureMessage);

                default:
                    return true;
            }
        }

        private static bool CheckFactionRequirement(PlayerMobile pm, RecipeRequirement req, out string failureMessage)
        {
            failureMessage = null;

            if (req.RequiredFaction == VystiaFaction.None)
                return true;

            var currentTier = VystiaReputation.GetFactionTier(pm, req.RequiredFaction);
            if (currentTier < req.MinimumReputationTier)
            {
                failureMessage = req.FailureMessage;
                return false;
            }

            return true;
        }

        private static bool CheckReligionRequirement(PlayerMobile pm, RecipeRequirement req, out string failureMessage)
        {
            failureMessage = null;

            if (req.RequiredReligion == VystiaReligion.None)
                return true;

            var pietyData = VystiaPiety.GetPiety(pm);
            if (pietyData == null || pietyData.Religion != req.RequiredReligion)
            {
                failureMessage = $"You must follow {ReligionData.GetInfo(req.RequiredReligion)?.Name} to craft this.";
                return false;
            }

            var currentTier = ReligionData.GetTier(pietyData.Piety);
            if (currentTier < req.MinimumPietyTier)
            {
                failureMessage = req.FailureMessage;
                return false;
            }

            return true;
        }

        /// <summary>
        /// Get the requirement for an item type (for display in gumps)
        /// </summary>
        public static RecipeRequirement GetRequirement(Type itemType)
        {
            if (itemType != null && s_Requirements.TryGetValue(itemType, out var req))
                return req;
            return null;
        }

        /// <summary>
        /// Check if an item type has tier requirements
        /// </summary>
        public static bool HasRequirements(Type itemType)
        {
            return itemType != null && s_Requirements.ContainsKey(itemType);
        }

        /// <summary>
        /// Get a description of the requirements for display
        /// </summary>
        public static string GetRequirementDescription(Type itemType)
        {
            var req = GetRequirement(itemType);
            if (req == null)
                return null;

            switch (req.RequirementType)
            {
                case RecipeRequirementType.FactionReputation:
                    var factionInfo = FactionData.GetInfo(req.RequiredFaction);
                    return $"Requires {req.MinimumReputationTier} with {factionInfo?.Name}";

                case RecipeRequirementType.ReligionPiety:
                    var religionInfo = ReligionData.GetInfo(req.RequiredReligion);
                    return $"Requires {req.MinimumPietyTier} in {religionInfo?.Name}";

                case RecipeRequirementType.Both:
                    var fInfo = FactionData.GetInfo(req.RequiredFaction);
                    var rInfo = ReligionData.GetInfo(req.RequiredReligion);
                    return $"Requires {req.MinimumReputationTier} ({fInfo?.Name}) and {req.MinimumPietyTier} ({rInfo?.Name})";

                default:
                    return null;
            }
        }
    }

    #endregion

    #region Craft System Integration Hook

    /// <summary>
    /// Hook class that integrates with CraftSystem to check tier requirements
    /// Call TierGatedCraftHook.CheckRequirements() before crafting
    /// </summary>
    public static class TierGatedCraftHook
    {
        /// <summary>
        /// Check if player meets tier requirements for crafting an item
        /// Call this from CraftSystem.CanCraft or similar method
        /// </summary>
        public static bool CheckRequirements(Mobile from, Type itemType)
        {
            if (!(from is PlayerMobile pm))
                return true;

            if (!TierGatedRecipeSystem.CanCraft(pm, itemType, out string failureMessage))
            {
                pm.SendMessage(0x22, failureMessage);
                return false;
            }

            return true;
        }

        /// <summary>
        /// Create a TryCraft delegate that checks tier requirements
        /// Can be assigned to CraftItem.TryCraft for gated recipes
        /// </summary>
        public static Action<Mobile, CraftItem, ITool> CreateGatedCraftDelegate(CraftSystem system)
        {
            return (mobile, craftItem, tool) =>
            {
                if (!CheckRequirements(mobile, craftItem.ItemType))
                {
                    // Requirements not met, delegate should prevent crafting
                    return;
                }

                // Requirements met, proceed with normal crafting
                system.CreateItem(mobile, craftItem.ItemType, typeof(IronIngot), tool, craftItem);
            };
        }
    }

    #endregion
}

