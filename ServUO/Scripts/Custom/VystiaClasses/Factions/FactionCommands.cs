/*
 * Vystia Faction System Commands
 * GM and player commands for faction reputation
 */

using System;
using Server;
using Server.Mobiles;
using Server.Commands;
using Server.Custom.VystiaClasses.Factions;

namespace Server.Custom.VystiaClasses.Commands
{
    public static class FactionCommands
    {
        public static void Initialize()
        {
            CommandSystem.Register("Factions", AccessLevel.Player, new CommandEventHandler(Factions_OnCommand));
            CommandSystem.Register("Faction", AccessLevel.Player, new CommandEventHandler(Faction_OnCommand));
            CommandSystem.Register("SetReputation", AccessLevel.GameMaster, new CommandEventHandler(SetReputation_OnCommand));
            CommandSystem.Register("AddReputation", AccessLevel.GameMaster, new CommandEventHandler(AddReputation_OnCommand));
            CommandSystem.Register("DonateFaction", AccessLevel.Player, new CommandEventHandler(DonateFaction_OnCommand));
        }

        /// <summary>
        /// [Factions - Shows all faction standings
        /// </summary>
        [Usage("Factions")]
        [Description("Shows your standing with all factions.")]
        private static void Factions_OnCommand(CommandEventArgs e)
        {
            if (e.Mobile is PlayerMobile pm)
            {
                pm.SendMessage("=== Faction Standings ===");

                foreach (VystiaFaction faction in Enum.GetValues(typeof(VystiaFaction)))
                {
                    if (faction == VystiaFaction.None)
                        continue;

                    int rep = VystiaReputation.GetFactionReputation(pm, faction);
                    var tier = VystiaReputation.GetFactionTier(pm, faction);
                    var info = FactionData.GetInfo(faction);
                    int hue = FactionData.GetTierHue(tier);
                    int discount = FactionData.GetVendorDiscount(tier);

                    string discountStr = discount > 0 ? $" ({discount}% discount)" : "";

                    pm.SendMessage(hue, "{0}: {1} ({2}){3}",
                        info?.Name ?? faction.ToString(),
                        rep,
                        tier,
                        discountStr);
                }

                pm.SendMessage("Use [Faction <1-7> for detailed information.");
            }
        }

        /// <summary>
        /// [Faction <id> - Shows detailed faction info
        /// </summary>
        [Usage("Faction <faction id 1-7>")]
        [Description("Shows detailed information about a specific faction.")]
        private static void Faction_OnCommand(CommandEventArgs e)
        {
            if (e.Mobile is PlayerMobile pm)
            {
                if (e.Arguments.Length < 1)
                {
                    pm.SendMessage("Usage: [Faction <1-7>");
                    pm.SendMessage("  1. Frostguard (Frosthold)");
                    pm.SendMessage("  2. Flame Legion (Emberlands)");
                    pm.SendMessage("  3. Greenward (Verdantpeak)");
                    pm.SendMessage("  4. Arcane Conclave (Crystal Barrens)");
                    pm.SendMessage("  5. Technoguild (Ironclad)");
                    pm.SendMessage("  6. Sandwalkers (Desert)");
                    pm.SendMessage("  7. Voidborn (ShadowVoid)");
                    return;
                }

                if (int.TryParse(e.Arguments[0], out int factionId))
                {
                    if (factionId < 1 || factionId > 7)
                    {
                        pm.SendMessage("Invalid faction ID. Use 1-7.");
                        return;
                    }

                    VystiaFaction faction = (VystiaFaction)factionId;
                    var info = FactionData.GetInfo(faction);
                    int rep = VystiaReputation.GetFactionReputation(pm, faction);
                    var tier = VystiaReputation.GetFactionTier(pm, faction);
                    int discount = FactionData.GetVendorDiscount(tier);

                    pm.SendMessage("=== {0} ===", info?.Name ?? faction.ToString());
                    pm.SendMessage("Description: {0}", info?.Description);
                    pm.SendMessage("Current Standing: {0} ({1})", tier, rep);

                    if (info?.EnemyFaction != VystiaFaction.None)
                    {
                        var enemyInfo = FactionData.GetInfo(info.EnemyFaction);
                        pm.SendMessage("Enemy Faction: {0}", enemyInfo?.Name);
                    }

                    if (discount > 0)
                        pm.SendMessage("Vendor Discount: {0}%", discount);

                    // Show tier thresholds
                    pm.SendMessage("--- Reputation Tiers ---");
                    pm.SendMessage("  Hostile: Below -1000");
                    pm.SendMessage("  Unfriendly: -1000 to 0");
                    pm.SendMessage("  Neutral: 0 to 3000");
                    pm.SendMessage("  Friendly: 3000 to 6000 (5% discount)");
                    pm.SendMessage("  Honored: 6000 to 12000 (8% discount)");
                    pm.SendMessage("  Revered: 12000 to 15000 (12% discount)");
                    pm.SendMessage("  Exalted: 15000+ (15% discount)");

                    // Progress to next tier
                    int nextThreshold = 0;
                    ReputationTier nextTier = ReputationTier.Neutral;

                    if (tier == ReputationTier.Hostile) { nextThreshold = -1000; nextTier = ReputationTier.Unfriendly; }
                    else if (tier == ReputationTier.Unfriendly) { nextThreshold = 0; nextTier = ReputationTier.Neutral; }
                    else if (tier == ReputationTier.Neutral) { nextThreshold = 1; nextTier = ReputationTier.Friendly; }
                    else if (tier == ReputationTier.Friendly) { nextThreshold = 1501; nextTier = ReputationTier.Allied; }
                    else if (tier == ReputationTier.Allied) { nextThreshold = 4501; nextTier = ReputationTier.Honored; }
                    else if (tier == ReputationTier.Honored) { nextThreshold = 9001; nextTier = ReputationTier.Exalted; }

                    if (tier != ReputationTier.Exalted)
                    {
                        int remaining = nextThreshold - rep;
                        pm.SendMessage("Progress to {0}: {1} reputation needed", nextTier, remaining);
                    }
                    else
                    {
                        pm.SendMessage("You have achieved maximum standing!");
                    }
                }
            }
        }

        /// <summary>
        /// [SetReputation <faction> <amount> - Sets exact reputation (GM)
        /// </summary>
        [Usage("SetReputation <faction 1-7> <amount>")]
        [Description("Sets exact reputation with a faction.")]
        private static void SetReputation_OnCommand(CommandEventArgs e)
        {
            if (e.Mobile is PlayerMobile pm)
            {
                if (e.Arguments.Length < 2)
                {
                    pm.SendMessage("Usage: [SetReputation <faction 1-7> <amount>");
                    return;
                }

                if (int.TryParse(e.Arguments[0], out int factionId) && int.TryParse(e.Arguments[1], out int amount))
                {
                    if (factionId < 1 || factionId > 7)
                    {
                        pm.SendMessage("Invalid faction ID. Use 1-7.");
                        return;
                    }

                    VystiaFaction faction = (VystiaFaction)factionId;
                    var data = VystiaReputation.GetReputation(pm);
                    if (data != null)
                    {
                        data.SetReputation(faction, amount);
                        var info = FactionData.GetInfo(faction);
                        pm.SendMessage("Reputation with {0} set to {1}.", info?.Name, amount);
                    }
                }
            }
        }

        /// <summary>
        /// [AddReputation <faction> <amount> - Adds reputation (GM)
        /// </summary>
        [Usage("AddReputation <faction 1-7> <amount>")]
        [Description("Adds (or removes if negative) reputation with a faction.")]
        private static void AddReputation_OnCommand(CommandEventArgs e)
        {
            if (e.Mobile is PlayerMobile pm)
            {
                if (e.Arguments.Length < 2)
                {
                    pm.SendMessage("Usage: [AddReputation <faction 1-7> <amount>");
                    return;
                }

                if (int.TryParse(e.Arguments[0], out int factionId) && int.TryParse(e.Arguments[1], out int amount))
                {
                    if (factionId < 1 || factionId > 7)
                    {
                        pm.SendMessage("Invalid faction ID. Use 1-7.");
                        return;
                    }

                    VystiaFaction faction = (VystiaFaction)factionId;
                    VystiaReputation.AddReputation(pm, faction, amount, "GM command");
                }
            }
        }

        /// <summary>
        /// [DonateFaction <faction> <gold> - Donate gold for reputation
        /// </summary>
        [Usage("DonateFaction <faction 1-7> <gold amount>")]
        [Description("Donate gold to gain reputation with a faction (+50 per 1000g).")]
        private static void DonateFaction_OnCommand(CommandEventArgs e)
        {
            if (e.Mobile is PlayerMobile pm)
            {
                if (e.Arguments.Length < 2)
                {
                    pm.SendMessage("Usage: [DonateFaction <faction 1-7> <gold amount>");
                    pm.SendMessage("You receive +50 reputation per 1,000 gold donated.");
                    return;
                }

                if (int.TryParse(e.Arguments[0], out int factionId) && int.TryParse(e.Arguments[1], out int goldAmount))
                {
                    if (factionId < 1 || factionId > 7)
                    {
                        pm.SendMessage("Invalid faction ID. Use 1-7.");
                        return;
                    }

                    VystiaFaction faction = (VystiaFaction)factionId;
                    ReputationRewards.AwardDonationReputation(pm, faction, goldAmount);
                }
            }
        }
    }
}
