// Vystia Faction Title System
// Grants special titles to players who reach Exalted tier with factions

using System;
using System.Collections.Generic;
using Server;
using Server.Items;
using Server.Mobiles;
using Server.Commands;

namespace Server.Custom.VystiaClasses.Factions
{
    #region Faction Title Data

    /// <summary>
    /// Defines the titles available for each faction at different tiers
    /// </summary>
    public static class FactionTitles
    {
        // Title format: [Tier][Faction] => Title
        private static readonly Dictionary<string, string> s_Titles = new Dictionary<string, string>
        {
            // Frostguard Titles
            { "Friendly_Frostguard", "Friend of the Frostguard" },
            { "Honored_Frostguard", "Guardian of Frosthold" },
            { "Revered_Frostguard", "Champion of Winter" },
            { "Exalted_Frostguard", "Iceborn of the Frostguard" },

            // Flame Legion Titles
            { "Friendly_FlameLegion", "Friend of the Flame" },
            { "Honored_FlameLegion", "Ember Warrior" },
            { "Revered_FlameLegion", "Phoenix Guard" },
            { "Exalted_FlameLegion", "Flamewarden of the Legion" },

            // Greenward Titles
            { "Friendly_Greenward", "Friend of the Forest" },
            { "Honored_Greenward", "Protector of Verdantpeak" },
            { "Revered_Greenward", "Elder of the Grove" },
            { "Exalted_Greenward", "Archdruid of the Greenward" },

            // Arcane Conclave Titles
            { "Friendly_ArcaneConclave", "Initiate of the Conclave" },
            { "Honored_ArcaneConclave", "Scholar of Crystals" },
            { "Revered_ArcaneConclave", "Arcane Magister" },
            { "Exalted_ArcaneConclave", "Archmage of the Conclave" },

            // Technoguild Titles
            { "Friendly_Technoguild", "Apprentice Engineer" },
            { "Honored_Technoguild", "Master Craftsman" },
            { "Revered_Technoguild", "Grand Engineer" },
            { "Exalted_Technoguild", "Technolord of the Guild" },

            // Sandwalkers Titles
            { "Friendly_Sandwalkers", "Friend of the Sands" },
            { "Honored_Sandwalkers", "Dune Rider" },
            { "Revered_Sandwalkers", "Sandstorm Walker" },
            { "Exalted_Sandwalkers", "Sultan's Chosen" },

            // Voidborn Titles
            { "Friendly_Voidborn", "Shadow Initiate" },
            { "Honored_Voidborn", "Void Touched" },
            { "Revered_Voidborn", "Shadow Master" },
            { "Exalted_Voidborn", "Herald of the Void" }
        };

        /// <summary>
        /// Get the title for a specific faction and tier
        /// </summary>
        public static string GetTitle(VystiaFaction faction, ReputationTier tier)
        {
            if (faction == VystiaFaction.None || tier < ReputationTier.Friendly)
                return null;

            string key = $"{tier}_{faction}";
            if (s_Titles.TryGetValue(key, out string title))
                return title;

            return null;
        }

        /// <summary>
        /// Get the highest available title for a player with a faction
        /// </summary>
        public static string GetHighestTitle(PlayerMobile pm, VystiaFaction faction)
        {
            var tier = VystiaReputation.GetFactionTier(pm, faction);
            return GetTitle(faction, tier);
        }

        /// <summary>
        /// Get all titles a player has earned with a faction
        /// </summary>
        public static List<string> GetAllTitles(PlayerMobile pm, VystiaFaction faction)
        {
            var titles = new List<string>();
            var currentTier = VystiaReputation.GetFactionTier(pm, faction);

            for (ReputationTier tier = ReputationTier.Friendly; tier <= currentTier; tier++)
            {
                var title = GetTitle(faction, tier);
                if (!string.IsNullOrEmpty(title))
                    titles.Add(title);
            }

            return titles;
        }
    }

    #endregion

    #region Title Management System

    /// <summary>
    /// Manages faction titles for players
    /// </summary>
    public static class FactionTitleManager
    {
        // Stores the currently displayed faction title for each player
        private static readonly Dictionary<PlayerMobile, FactionTitleData> s_ActiveTitles = new Dictionary<PlayerMobile, FactionTitleData>();

        /// <summary>
        /// Set a player's active faction title
        /// </summary>
        public static bool SetTitle(PlayerMobile pm, VystiaFaction faction)
        {
            if (pm == null)
                return false;

            var tier = VystiaReputation.GetFactionTier(pm, faction);
            var title = FactionTitles.GetTitle(faction, tier);

            if (string.IsNullOrEmpty(title))
            {
                pm.SendMessage(0x22, "You don't have a title with that faction yet.");
                return false;
            }

            s_ActiveTitles[pm] = new FactionTitleData { Faction = faction, Title = title };
            pm.Title = title;

            var info = FactionData.GetInfo(faction);
            pm.SendMessage(info?.Hue ?? 0, "Your title is now: {0}", title);

            return true;
        }

        /// <summary>
        /// Clear a player's faction title
        /// </summary>
        public static void ClearTitle(PlayerMobile pm)
        {
            if (pm == null)
                return;

            if (s_ActiveTitles.ContainsKey(pm))
            {
                s_ActiveTitles.Remove(pm);
                pm.Title = null;
                pm.SendMessage("Your faction title has been cleared.");
            }
        }

        /// <summary>
        /// Get a player's current active faction title info
        /// </summary>
        public static FactionTitleData GetActiveTitle(PlayerMobile pm)
        {
            if (pm != null && s_ActiveTitles.TryGetValue(pm, out var data))
                return data;
            return null;
        }

        /// <summary>
        /// Check if player has reached Exalted with any faction
        /// </summary>
        public static bool HasExaltedTitle(PlayerMobile pm)
        {
            if (pm == null)
                return false;

            foreach (VystiaFaction faction in Enum.GetValues(typeof(VystiaFaction)))
            {
                if (faction != VystiaFaction.None)
                {
                    var tier = VystiaReputation.GetFactionTier(pm, faction);
                    if (tier >= ReputationTier.Exalted)
                        return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Get all factions where player is Exalted
        /// </summary>
        public static List<VystiaFaction> GetExaltedFactions(PlayerMobile pm)
        {
            var factions = new List<VystiaFaction>();

            if (pm == null)
                return factions;

            foreach (VystiaFaction faction in Enum.GetValues(typeof(VystiaFaction)))
            {
                if (faction != VystiaFaction.None)
                {
                    var tier = VystiaReputation.GetFactionTier(pm, faction);
                    if (tier >= ReputationTier.Exalted)
                        factions.Add(faction);
                }
            }

            return factions;
        }
    }

    /// <summary>
    /// Data class for storing active title information
    /// </summary>
    public class FactionTitleData
    {
        public VystiaFaction Faction { get; set; }
        public string Title { get; set; }
    }

    #endregion

    #region Title Commands

    /// <summary>
    /// Commands for managing faction titles
    /// </summary>
    public static class FactionTitleCommands
    {
        public static void Initialize()
        {
            CommandSystem.Register("FactionTitle", AccessLevel.Player, new CommandEventHandler(FactionTitle_OnCommand));
            CommandSystem.Register("FT", AccessLevel.Player, new CommandEventHandler(FactionTitle_OnCommand));
            CommandSystem.Register("ClearTitle", AccessLevel.Player, new CommandEventHandler(ClearTitle_OnCommand));
            CommandSystem.Register("CT", AccessLevel.Player, new CommandEventHandler(ClearTitle_OnCommand));
            CommandSystem.Register("MyTitles", AccessLevel.Player, new CommandEventHandler(MyTitles_OnCommand));

            Console.WriteLine("[Vystia] Faction Title commands registered.");
        }

        /// <summary>
        /// [FactionTitle <faction> - Set your active faction title
        /// </summary>
        [Usage("FactionTitle <faction>")]
        [Description("Set your displayed faction title. Use faction name (Frostguard, FlameLegion, etc.)")]
        private static void FactionTitle_OnCommand(CommandEventArgs e)
        {
            if (!(e.Mobile is PlayerMobile pm))
                return;

            if (e.Arguments.Length < 1)
            {
                pm.SendMessage("Usage: [FactionTitle <faction>");
                pm.SendMessage("Factions: Frostguard, FlameLegion, Greenward, ArcaneConclave, Technoguild, Sandwalkers, Voidborn");
                return;
            }

            string factionName = e.Arguments[0];
            VystiaFaction faction = VystiaFaction.None;

            // Parse faction name
            if (Enum.TryParse(factionName, true, out faction) && faction != VystiaFaction.None)
            {
                FactionTitleManager.SetTitle(pm, faction);
            }
            else
            {
                pm.SendMessage(0x22, "Unknown faction. Valid factions: Frostguard, FlameLegion, Greenward, ArcaneConclave, Technoguild, Sandwalkers, Voidborn");
            }
        }

        /// <summary>
        /// [ClearTitle - Clear your faction title
        /// </summary>
        [Usage("ClearTitle")]
        [Description("Clear your displayed faction title.")]
        private static void ClearTitle_OnCommand(CommandEventArgs e)
        {
            if (e.Mobile is PlayerMobile pm)
            {
                FactionTitleManager.ClearTitle(pm);
            }
        }

        /// <summary>
        /// [MyTitles - List all your earned faction titles
        /// </summary>
        [Usage("MyTitles")]
        [Description("List all faction titles you have earned.")]
        private static void MyTitles_OnCommand(CommandEventArgs e)
        {
            if (!(e.Mobile is PlayerMobile pm))
                return;

            pm.SendMessage(0x35, "=== Your Faction Titles ===");

            bool hasTitles = false;

            foreach (VystiaFaction faction in Enum.GetValues(typeof(VystiaFaction)))
            {
                if (faction == VystiaFaction.None)
                    continue;

                var titles = FactionTitles.GetAllTitles(pm, faction);
                if (titles.Count > 0)
                {
                    hasTitles = true;
                    var info = FactionData.GetInfo(faction);
                    pm.SendMessage(info?.Hue ?? 0, "{0}:", info?.Name ?? faction.ToString());

                    foreach (var title in titles)
                    {
                        pm.SendMessage("  - {0}", title);
                    }
                }
            }

            if (!hasTitles)
            {
                pm.SendMessage(0x22, "You have not earned any faction titles yet. Reach Friendly reputation to unlock your first title.");
            }
        }
    }

    #endregion

    #region Exalted Rewards System

    /// <summary>
    /// Special rewards and bonuses for Exalted-tier players
    /// </summary>
    public static class ExaltedRewards
    {
        /// <summary>
        /// Check if player qualifies for Exalted rewards with a faction
        /// </summary>
        public static bool IsExalted(PlayerMobile pm, VystiaFaction faction)
        {
            if (pm == null || faction == VystiaFaction.None)
                return false;

            return VystiaReputation.GetFactionTier(pm, faction) >= ReputationTier.Exalted;
        }

        /// <summary>
        /// Get the Exalted bonus modifier for a player (for damage, crafting, etc.)
        /// Returns 0.0 to 1.0 (0% to 100% bonus)
        /// </summary>
        public static double GetExaltedBonus(PlayerMobile pm, VystiaFaction faction)
        {
            if (!IsExalted(pm, faction))
                return 0.0;

            // 10% bonus for Exalted status
            return 0.10;
        }

        /// <summary>
        /// Get additional vendor discount for Exalted players (beyond normal tier discount)
        /// </summary>
        public static int GetExaltedVendorBonus(PlayerMobile pm, VystiaFaction faction)
        {
            if (!IsExalted(pm, faction))
                return 0;

            // Additional 5% discount on top of the 15% Exalted tier discount
            return 5;
        }

        /// <summary>
        /// Grant Exalted achievement rewards when player first reaches Exalted
        /// Called when reputation changes
        /// </summary>
        public static void OnReachExalted(PlayerMobile pm, VystiaFaction faction)
        {
            if (pm == null || faction == VystiaFaction.None)
                return;

            var info = FactionData.GetInfo(faction);
            var title = FactionTitles.GetTitle(faction, ReputationTier.Exalted);

            // Send congratulations
            pm.SendMessage(0x35, "*** ACHIEVEMENT UNLOCKED ***");
            pm.SendMessage(info?.Hue ?? 0, "You have reached Exalted status with {0}!", info?.Name ?? faction.ToString());
            pm.SendMessage(0x35, "You have earned the title: {0}", title);
            pm.SendMessage(0x35, "You now have access to exclusive Exalted rewards!");

            // Visual effects
            pm.PlaySound(0x5C9); // Fanfare sound
            pm.FixedParticles(0x376A, 9, 32, 5030, info?.Hue ?? 0, 0, EffectLayer.Waist);

            // Grant bonus tokens
            Server.Items.Vystia.FactionTokenSystem.GiveTokens(pm, faction, 50);
            pm.SendMessage(0x35, "You receive 50 bonus {0} tokens!", info?.Name);
        }
    }

    #endregion
}
