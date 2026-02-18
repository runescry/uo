using System;
using System.Collections.Generic;
using System.Linq;
using Server.Mobiles;
using Server.Items;
using Server.Engines.PartySystem;
using Server.Gumps;
using Server.Network;
using Server.Commands;
using Server.Custom.VystiaClasses.Factions;

namespace Server.Engines.VvV
{
    /// <summary>
    /// Vystia Faction System - Replaces Vice vs Virtue with Vystia factions
    /// Maintains VvV infrastructure while using Vystia faction names and content
    /// </summary>
    public class VystiaFactionSystem : PointsSystem
    {
        public static VystiaFactionSystem Instance { get; set; }
        public static bool Enabled { get; set; } = true;
        public static int StartSilver { get; set; } = 2000;
        public static bool EnhancedRules { get; set; } = false;

        public override TextDefinition Name { get; } = new TextDefinition("Vystia Factions");
        public override PointsType Loyalty { get; } = PointsType.VystiaFactions;
        public override bool AutoAdd { get; } = true;
        public override double MaxPoints { get; } = double.MaxValue;
        public override bool ShowOnLoyaltyGump { get; } = true;

        private static readonly Dictionary<VystiaFaction, VystiaFactionDefinition> s_FactionDefinitions;
        private static readonly Dictionary<Mobile, VystiaFactionPlayerData> s_PlayerData;

        static VystiaFactionSystem()
        {
            s_FactionDefinitions = new Dictionary<VystiaFaction, VystiaFactionDefinition>();
            s_PlayerData = new Dictionary<Mobile, VystiaFactionPlayerData>();
            InitializeFactions();
        }

        /// <summary>
        /// Initialize the Vystia Faction System
        /// </summary>
        public static void Initialize()
        {
            if (Instance != null)
                return;

            Instance = new VystiaFactionSystem();
            PointsSystem.Register(Instance);

            // Register commands
            CommandSystem.Register("VystiaFaction", AccessLevel.Player, VystiaFaction_OnCommand);
            CommandSystem.Register("VF", AccessLevel.Player, VystiaFaction_OnCommand);

            Console.WriteLine("[VystiaFactionSystem] Initialized Vystia Faction System");
            Console.WriteLine($"[VystiaFactionSystem] Registered {s_FactionDefinitions.Count} Vystia factions");
        }

        /// <summary>
        /// Get faction definition by faction type
        /// </summary>
        public static VystiaFactionDefinition GetFactionDefinition(VystiaFaction faction)
        {
            return s_FactionDefinitions.GetValueOrDefault(faction);
        }

        /// <summary>
        /// Get player faction data
        /// </summary>
        public static VystiaFactionPlayerData GetPlayerData(Mobile mobile)
        {
            if (mobile == null)
                return null;

            if (!s_PlayerData.ContainsKey(mobile))
            {
                s_PlayerData[mobile] = new VystiaFactionPlayerData(mobile);
            }

            return s_PlayerData[mobile];
        }

        /// <summary>
        /// Add silver to player's faction account
        /// </summary>
        public static bool AddSilver(Mobile from, int silver)
        {
            if (from == null || silver <= 0)
                return false;

            var playerData = GetPlayerData(from);
            if (playerData.Faction == VystiaFaction.None)
            {
                from.SendMessage("You must join a faction first!");
                return false;
            }

            playerData.Silver += silver;
            from.SendMessage($"You have earned {silver} silver for {playerData.Faction}!");

            return true;
        }

        /// <summary>
        /// Get player's current faction
        /// </summary>
        public static VystiaFaction GetPlayerFaction(Mobile mobile)
        {
            var playerData = GetPlayerData(mobile);
            return playerData?.Faction ?? VystiaFaction.None;
        }

        /// <summary>
        /// Set player's faction
        /// </summary>
        public static bool SetPlayerFaction(Mobile mobile, VystiaFaction faction)
        {
            if (mobile == null || faction == VystiaFaction.None)
                return false;

            var playerData = GetPlayerData(mobile);
            var oldFaction = playerData.Faction;

            if (oldFaction != VystiaFaction.None)
            {
                // Leaving current faction
                mobile.SendMessage($"You have left the {oldFaction} faction!");
            }

            playerData.Faction = faction;
            playerData.Silver = StartSilver;

            mobile.SendMessage($"You have joined the {faction} faction!");
            mobile.SendMessage($"You have received {StartSilver} silver to start!");

            return true;
        }

        /// <summary>
        /// Get faction ranking
        /// </summary>
        public static VystiaFactionRank GetFactionRank(Mobile mobile)
        {
            var playerData = GetPlayerData(mobile);
            if (playerData.Faction == VystiaFaction.None)
                return VystiaFactionRank.None;

            var silver = playerData.Silver;
            var rank = VystiaFactionRank.None;

            if (silver >= 10000)
                rank = VystiaFactionRank.Exalted;
            else if (silver >= 5000)
                rank = VystiaFactionRank.Revered;
            else if (silver >= 2500)
                rank = VystiaFactionRank.Honored;
            else if (silver >= 1000)
                rank = VystiaFactionRank.Admired;
            else if (silver >= 500)
                rank = VystiaFactionRank.Respected;
            else if (silver >= 100)
                rank = VystiaFactionRank.Friendly;
            else if (silver >= -1000)
                rank = VystiaFactionRank.Neutral;
            else if (silver >= -2500)
                rank = VystiaFactionRank.Disliked;
            else if (silver >= -5000)
                rank = VystiaFactionRank.Hated;
            else
                rank = VystiaFactionRank.Villain;

            return rank;
        }

        /// <summary>
        /// Get faction vendor discount
        /// </summary>
        public static double GetFactionVendorDiscount(Mobile mobile)
        {
            var rank = GetFactionRank(mobile);
            
            switch (rank)
            {
                case VystiaFactionRank.Exalted: return 0.15;
                case VystiaFactionRank.Revered: return 0.12;
                case VystiaFactionRank.Honored: return 0.10;
                case VystiaFactionRank.Admired: return 0.08;
                case VystiaFactionRank.Respected: return 0.05;
                default: return 0.0;
            }
        }

        /// <summary>
        /// Handle faction command
        /// </summary>
        private static void VystiaFaction_OnCommand(CommandEventArgs e)
        {
            var from = e.Mobile as PlayerMobile;
            if (from == null)
                return;

            if (e.Length == 0)
            {
                ShowFactionInfo(from);
                return;
            }

            switch (e.GetString(0).ToLower())
            {
                case "join":
                    JoinFaction(from, e);
                    break;
                case "leave":
                    LeaveFaction(from);
                    break;
                case "rank":
                    ShowFactionRank(from);
                    break;
                case "vendors":
                    ShowFactionVendors(from);
                    break;
                case "info":
                    ShowFactionInfo(from);
                    break;
                default:
                    ShowFactionInfo(from);
                    break;
            }
        }

        /// <summary>
        /// Show faction information
        /// </summary>
        private static void ShowFactionInfo(Mobile from)
        {
            from.SendMessage("=== VYSTIA FACTION SYSTEM ===");
            from.SendMessage("Commands:");
            from.SendMessage("  [VystiaFaction join <faction>] - Join a faction");
            from.SendMessage("  [VystiaFaction leave] - Leave current faction");
            from.SendMessage("  [VystiaFaction rank] - Show your faction rank");
            from.SendMessage("  [VystiaFaction vendors] - Show faction vendors");
            from.SendMessage("  [VystiaFaction info] - Show this help");
            from.SendMessage("");
            from.SendMessage("Available Factions:");
            
            foreach (var faction in Enum.GetValues(typeof(VystiaFaction)))
            {
                if (faction != VystiaFaction.None)
                {
                    var def = GetFactionDefinition(faction);
                    from.SendMessage($"  {faction} - {def?.Description}");
                }
            }
        }

        /// <summary>
        /// Join a faction
        /// </summary>
        private static void JoinFaction(Mobile from, CommandEventArgs e)
        {
            if (e.Length < 2)
            {
                from.SendMessage("Usage: [VystiaFaction join <faction>");
                return;
            }

            if (!Enum.TryParse<VystiaFaction>(e.GetString(1), true, out var faction) || faction == VystiaFaction.None)
            {
                from.SendMessage("Invalid faction. Available factions:");
                foreach (var f in Enum.GetValues(typeof(VystiaFaction)))
                {
                    if (f != VystiaFaction.None)
                        from.SendMessage($"  {f}");
                }
                return;
            }

            SetPlayerFaction(from, faction);
        }

        /// <summary>
        /// Leave current faction
        /// </summary>
        private static void LeaveFaction(Mobile from)
        {
            SetPlayerFaction(from, VystiaFaction.None);
        }

        /// <summary>
        /// Show faction rank
        /// </summary>
        private static void ShowFactionRank(Mobile from)
        {
            var faction = GetPlayerFaction(from);
            var rank = GetFactionRank(from);
            var playerData = GetPlayerData(from);

            from.SendMessage("=== FACTION STATUS ===");
            from.SendMessage($"Faction: {faction}");
            from.SendMessage($"Rank: {rank}");
            from.SendMessage($"Silver: {playerData.Silver}");
            from.SendMessage($"Vendor Discount: {GetFactionVendorDiscount(from):P1}");
        }

        /// <summary>
        /// Show faction vendors
        /// </summary>
        private static void ShowFactionVendors(Mobile from)
        {
            var faction = GetPlayerFaction(from);
            if (faction == VystiaFaction.None)
            {
                from.SendMessage("You must join a faction first!");
                return;
            }

            from.SendMessage($"=== {faction} VENDORS ===");
            from.SendMessage("Visit faction vendors for special items and discounts!");
            from.SendMessage($"Your current discount: {GetFactionVendorDiscount(from):P1}");
        }

        /// <summary>
        /// Initialize faction definitions
        /// </summary>
        private static void InitializeFactions()
        {
            s_FactionDefinitions[VystiaFaction.Frostguard] = new VystiaFactionDefinition
            {
                Faction = VystiaFaction.Frostguard,
                Name = "Frostguard",
                Description = "The Frostguard of Frosthold",
                Color = 1153, // Ice blue
                Stronghold = "Frosthold",
                Leader = "Frost Commander",
                Benefits = "Ice magic bonuses, cold resistance, winter supplies"
            };

            s_FactionDefinitions[VystiaFaction.FlameLegion] = new VystiaFactionDefinition
            {
                Faction = VystiaFaction.FlameLegion,
                Name = "Flame Legion",
                Description = "The Flame Legion of Emberlands",
                Color = 1358, // Fire orange
                Stronghold = "Emberlands",
                Leader = "Flame Commander",
                Benefits = "Fire magic bonuses, heat resistance, forge supplies"
            };

            s_FactionDefinitions[VystiaFaction.Greenward] = new VystiaFactionDefinition
            {
                Faction = VystiaFaction.Greenward,
                Name = "Greenward",
                Description = "The Greenward of Verdantpeak",
                Color = 1273, // Forest green
                Stronghold = "Verdantpeak",
                Leader = "Nature Warden",
                Benefits = "Nature magic bonuses, poison resistance, herbal supplies"
            };

            s_FactionDefinitions[VystiaFaction.Stormcallers] = new VystiaFactionDefinition
            {
                Faction = VystiaFaction.Stormcallers,
                Name = "Stormcallers",
                Description = "The Stormcallers of Thunderpeak",
                Color = 1104, // Storm blue
                Stronghold = "Thunderpeak",
                Leader = "Storm Lord",
                Benefits = "Lightning magic bonuses, storm resistance, weather supplies"
            };

            s_FactionDefinitions[VystiaFaction.Shadowhand] = new VystiaFactionDefinition
            {
                Faction = VystiaFaction.Shadowhand,
                Name = "Shadowhand",
                Description = "The Shadowhand of Darkwood",
                Color = 1175, // Shadow purple
                Stronghold = "Darkwood",
                Leader = "Shadow Master",
                Benefits = "Shadow magic bonuses, stealth, infiltration supplies"
            };

            s_FactionDefinitions[VystiaFaction.HolyOrder] = new VystiaFactionDefinition
            {
                Faction = VystiaFaction.HolyOrder,
                Name = "Holy Order",
                Description = "The Holy Order of Sanctum",
                Color = 1152, // Holy gold
                Stronghold = "Sanctum",
                Leader = "High Priest",
                Benefits = "Holy magic bonuses, undead resistance, blessing supplies"
            };

            s_FactionDefinitions[VystiaFaction.VoidWalkers] = new VystiaFactionDefinition
            {
                Faction = VystiaFaction.VoidWalkers,
                Name = "Void Walkers",
                Description = "The Void Walkers of Abyss",
                Color = 1102, // Void black
                Stronghold = "Abyss",
                Leader = "Void Lord",
                Benefits = "Void magic bonuses, dimensional resistance, arcane supplies"
            };
        }

        /// <summary>
        /// Get all faction definitions
        /// </summary>
        public static Dictionary<VystiaFaction, VystiaFactionDefinition> GetAllFactions()
        {
            return new Dictionary<VystiaFaction, VystiaFactionDefinition>(s_FactionDefinitions);
        }
    }

    /// <summary>
    /// Vystia faction definition
    /// </summary>
    public class VystiaFactionDefinition
    {
        public VystiaFaction Faction { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Color { get; set; }
        public string Stronghold { get; set; }
        public string Leader { get; set; }
        public string Benefits { get; set; }
    }

    /// <summary>
    /// Vystia faction player data
    /// </summary>
    public class VystiaFactionPlayerData
    {
        public Mobile Player { get; set; }
        public VystiaFaction Faction { get; set; }
        public int Silver { get; set; }
        public DateTime JoinedAt { get; set; }
        public int Kills { get; set; }
        public int Deaths { get; set; }

        public VystiaFactionPlayerData(Mobile player)
        {
            Player = player;
            Faction = VystiaFaction.None;
            Silver = 0;
            JoinedAt = DateTime.UtcNow;
            Kills = 0;
            Deaths = 0;
        }
    }

    /// <summary>
    /// Vystia faction ranks
    /// </summary>
    public enum VystiaFactionRank
    {
        None = 0,
        Villain = -4,
        Hated = -3,
        Disliked = -2,
        Neutral = -1,
        Friendly = 0,
        Respected = 1,
        Admired = 2,
        Honored = 3,
        Revered = 4,
        Exalted = 5
    }
}
