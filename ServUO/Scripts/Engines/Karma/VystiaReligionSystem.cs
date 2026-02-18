using System;
using System.Collections.Generic;
using System.Linq;
using Server.Mobiles;
using Server.Items;
using Server.Gumps;
using Server.Network;
using Server.Commands;
using Server.Custom.VystiaClasses.Religion;

namespace Server.Engines.Karma
{
    /// <summary>
    /// Vystia Religion System - Replaces Karma System with Vystia religions
    /// Maintains karma infrastructure while using Vystia religion names and content
    /// </summary>
    public class VystiaReligionSystem
    {
        private static readonly Dictionary<VystiaReligion, VystiaReligionDefinition> s_ReligionDefinitions;
        private static readonly Dictionary<Mobile, VystiaReligionPlayerData> s_PlayerData;
        private static readonly object s_Lock = new object();
        private static bool s_Initialized = false;

        static VystiaReligionSystem()
        {
            s_ReligionDefinitions = new Dictionary<VystiaReligion, VystiaReligionDefinition>();
            s_PlayerData = new Dictionary<Mobile, VystiaReligionPlayerData>();
            InitializeReligions();
        }

        /// <summary>
        /// Initialize the Vystia Religion System
        /// </summary>
        public static void Initialize()
        {
            if (s_Initialized)
                return;

            s_Initialized = true;

            // Register commands
            CommandSystem.Register("VystiaReligion", AccessLevel.Player, VystiaReligion_OnCommand);
            CommandSystem.Register("VR", AccessLevel.Player, VystiaReligion_OnCommand);

            Console.WriteLine("[VystiaReligionSystem] Initialized Vystia Religion System");
            Console.WriteLine($"[VystiaReligionSystem] Registered {s_ReligionDefinitions.Count} Vystia religions");
        }

        /// <summary>
        /// Get religion definition by religion type
        /// </summary>
        public static VystiaReligionDefinition GetReligionDefinition(VystiaReligion religion)
        {
            return s_ReligionDefinitions.GetValueOrDefault(religion);
        }

        /// <summary>
        /// Get player religion data
        /// </summary>
        public static VystiaReligionPlayerData GetPlayerData(Mobile mobile)
        {
            if (mobile == null)
                return null;

            lock (s_Lock)
            {
                if (!s_PlayerData.ContainsKey(mobile))
                {
                    s_PlayerData[mobile] = new VystiaReligionPlayerData(mobile);
                }

                return s_PlayerData[mobile];
            }
        }

        /// <summary>
        /// Get player's current religion
        /// </summary>
        public static VystiaReligion GetPlayerReligion(Mobile mobile)
        {
            var playerData = GetPlayerData(mobile);
            return playerData?.Religion ?? VystiaReligion.None;
        }

        /// <summary>
        /// Set player's religion
        /// </summary>
        public static bool SetPlayerReligion(Mobile mobile, VystiaReligion religion)
        {
            if (mobile == null || religion == VystiaReligion.None)
                return false;

            lock (s_Lock)
            {
                var playerData = GetPlayerData(mobile);
                var oldReligion = playerData.Religion;

                if (oldReligion != VystiaReligion.None)
                {
                    // Leaving current religion
                    mobile.SendMessage($"You have left the {oldReligion} religion!");
                }

                playerData.Religion = religion;
                playerData.Piety = 0;
                playerData.LastPrayer = DateTime.MinValue;

                mobile.SendMessage($"You have joined the {religion} religion!");
                mobile.SendMessage($"Your piety starts at 0. Pray daily to increase your piety!");

                return true;
            }
        }

        /// <summary>
        /// Add piety to player's religion account
        /// </summary>
        public static bool AddPiety(Mobile from, int piety)
        {
            if (from == null || piety <= 0)
                return false;

            lock (s_Lock)
            {
                var playerData = GetPlayerData(from);
                if (playerData.Religion == VystiaReligion.None)
                {
                    from.SendMessage("You must join a religion first!");
                    return false;
                }

                playerData.Piety += piety;
                from.SendMessage($"You have gained {piety} piety with {playerData.Religion}!");

                // Check for piety milestones
                CheckPietyMilestones(from, playerData);

                return true;
            }
        }

        /// <summary>
        /// Get religion piety tier
        /// </summary>
        public static VystiaPietyTier GetPietyTier(Mobile mobile)
        {
            var playerData = GetPlayerData(mobile);
            if (playerData.Religion == VystiaReligion.None)
                return VystiaPietyTier.None;

            var piety = playerData.Piety;
            var tier = VystiaPietyTier.None;

            if (piety >= 900)
                tier = VystiaPietyTier.Devout;
            else if (piety >= 500)
                tier = VystiaPietyTier.Faithful;
            else if (piety >= 200)
                tier = VystiaPietyTier.Devoted;
            else if (piety >= 50)
                tier = VystiaPietyTier.Believer;
            else if (piety >= 10)
                tier = VystiaPietyTier.Novice;
            else
                tier = VystiaPietyTier.None;

            return tier;
        }

        /// <summary>
        /// Perform daily prayer
        /// </summary>
        public static bool PerformDailyPrayer(Mobile mobile)
        {
            if (mobile == null)
                return false;

            lock (s_Lock)
            {
                var playerData = GetPlayerData(mobile);
                if (playerData.Religion == VystiaReligion.None)
                {
                    mobile.SendMessage("You must join a religion first!");
                    return false;
                }

                var now = DateTime.UtcNow;
                var lastPrayer = playerData.LastPrayer;
                
                // Check if 24 hours have passed since last prayer
                if (lastPrayer != DateTime.MinValue && (now - lastPrayer).TotalHours < 24)
                {
                    var hoursRemaining = 24 - (int)(now - lastPrayer).TotalHours;
                    mobile.SendMessage($"You must wait {hoursRemaining} hours before praying again.");
                    return false;
                }

                // Perform prayer
                playerData.LastPrayer = now;
                var pietyGain = 10 + Utility.RandomMinMax(0, 5); // 10-15 piety
                
                AddPiety(mobile, pietyGain);
                mobile.SendMessage($"You have prayed to {playerData.Religion} and gained {pietyGain} piety!");

                // Apply passive bonuses based on piety tier
                ApplyPassiveBonuses(mobile, playerData);

                return true;
            }
        }

        /// <summary>
        /// Perform tithe (donation)
        /// </summary>
        public static bool PerformTithe(Mobile mobile, int goldAmount)
        {
            if (mobile == null || goldAmount <= 0)
                return false;

            lock (s_Lock)
            {
                var playerData = GetPlayerData(mobile);
                if (playerData.Religion == VystiaReligion.None)
                {
                    mobile.SendMessage("You must join a religion first!");
                    return false;
                }

                // Check if player has enough gold
                if (mobile.Backpack == null || mobile.Backpack.GetAmount(typeof(Gold)) < goldAmount)
                {
                    mobile.SendMessage("You don't have enough gold for the tithe!");
                    return false;
                }

                // Take gold from backpack
                if (mobile.Backpack.ConsumeTotal(typeof(Gold), goldAmount))
                {
                    var pietyGain = goldAmount / 100; // 1 piety per 100 gold
                    AddPiety(mobile, pietyGain);
                    mobile.SendMessage($"You have tithed {goldAmount} gold to {playerData.Religion} and gained {pietyGain} piety!");
                    return true;
                }

                return false;
            }
        }

        /// <summary>
        /// Perform pilgrimage
        /// </summary>
        public static bool PerformPilgrimage(Mobile mobile)
        {
            if (mobile == null)
                return false;

            lock (s_Lock)
            {
                var playerData = GetPlayerData(mobile);
                if (playerData.Religion == VystiaReligion.None)
                {
                    mobile.SendMessage("You must join a religion first!");
                    return false;
                }

                var religionDef = GetReligionDefinition(playerData.Religion);
                if (religionDef == null)
                    return false;

                // Check if player is at the correct pilgrimage location
                if (!IsAtPilgrimageLocation(mobile, playerData.Religion))
                {
                    mobile.SendMessage($"You must visit the {religionDef.HolySite} to perform a pilgrimage!");
                    return false;
                }

                // Check if enough time has passed since last pilgrimage
                var now = DateTime.UtcNow;
                var lastPilgrimage = playerData.LastPilgrimage;
                
                if (lastPilgrimage != DateTime.MinValue && (now - lastPilgrimage).TotalDays < 7)
                {
                    var daysRemaining = 7 - (int)(now - lastPilgrimage).TotalDays;
                    mobile.SendMessage($"You must wait {daysRemaining} days before performing another pilgrimage.");
                    return false;
                }

                // Perform pilgrimage
                playerData.LastPilgrimage = now;
                AddPiety(mobile, 75);
                mobile.SendMessage($"You have completed a pilgrimage to {religionDef.HolySite} and gained 75 piety!");

                return true;
            }
        }

        /// <summary>
        /// Check if player is at pilgrimage location
        /// </summary>
        private static bool IsAtPilgrimageLocation(Mobile mobile, VystiaReligion religion)
        {
            var religionDef = GetReligionDefinition(religion);
            if (religionDef == null)
                return false;

            // This would check if the player is at the correct location
            // For now, we'll assume they are if they're close enough to a shrine
            return mobile.InRange(religionDef.HolySiteLocation, 10);
        }

        /// <summary>
        /// Check for piety milestones
        /// </summary>
        private static void CheckPietyMilestones(Mobile mobile, VystiaReligionPlayerData playerData)
        {
            var oldTier = playerData.LastMilestoneTier;
            var newTier = GetPietyTier(mobile);

            if (newTier > oldTier)
            {
                playerData.LastMilestoneTier = newTier;
                mobile.SendMessage($"Congratulations! You have reached the {newTier} tier in {playerData.Religion}!");
                
                // Grant milestone rewards
                GrantMilestoneRewards(mobile, playerData, newTier);
            }
        }

        /// <summary>
        /// Grant milestone rewards
        /// </summary>
        private static void GrantMilestoneRewards(Mobile mobile, VystiaReligionPlayerData playerData, VystiaPietyTier tier)
        {
            switch (tier)
            {
                case VystiaPietyTier.Believer:
                    mobile.SendMessage("You have gained access to basic religious blessings!");
                    break;
                case VystiaPietyTier.Devoted:
                    mobile.SendMessage("You have gained access to devotion powers!");
                    break;
                case VystiaPietyTier.Faithful:
                    mobile.SendMessage("You have gained access to faith powers!");
                    break;
                case VystiaPietyTier.Devout:
                    mobile.SendMessage("You have gained access to devout powers!");
                    break;
            }
        }

        /// <summary>
        /// Apply passive bonuses based on piety tier
        /// </summary>
        private static void ApplyPassiveBonuses(Mobile mobile, VystiaReligionPlayerData playerData)
        {
            var tier = GetPietyTier(mobile);
            
            switch (tier)
            {
                case VystiaPietyTier.Believer:
                    // Basic passive bonus
                    break;
                case VystiaPietyTier.Devoted:
                    // Enhanced passive bonus
                    break;
                case VystiaPietyTier.Faithful:
                    // Strong passive bonus
                    break;
                case VystiaPietyTier.Devout:
                    // Maximum passive bonus
                    break;
            }
        }

        /// <summary>
        /// Handle religion command
        /// </summary>
        private static void VystiaReligion_OnCommand(CommandEventArgs e)
        {
            var from = e.Mobile as PlayerMobile;
            if (from == null)
                return;

            if (e.Length == 0)
            {
                ShowReligionInfo(from);
                return;
            }

            switch (e.GetString(0).ToLower())
            {
                case "join":
                    JoinReligion(from, e);
                    break;
                case "leave":
                    LeaveReligion(from);
                    break;
                case "pray":
                    PerformDailyPrayer(from);
                    break;
                case "tithe":
                    PerformTithe(from, e);
                    break;
                case "pilgrimage":
                    PerformPilgrimage(from);
                    break;
                case "status":
                    ShowReligionStatus(from);
                    break;
                case "info":
                    ShowReligionInfo(from);
                    break;
                default:
                    ShowReligionInfo(from);
                    break;
            }
        }

        /// <summary>
        /// Show religion information
        /// </summary>
        private static void ShowReligionInfo(Mobile from)
        {
            from.SendMessage("=== VYSTIA RELIGION SYSTEM ===");
            from.SendMessage("Commands:");
            from.SendMessage("  [VystiaReligion join <religion>] - Join a religion");
            from.SendMessage("  [VystiaReligion leave] - Leave current religion");
            from.SendMessage("  [VystiaReligion pray] - Perform daily prayer");
            from.SendMessage("  [VystiaReligion tithe <amount>] - Tithe gold");
            from.SendMessage("  [VystiaReligion pilgrimage] - Perform pilgrimage");
            from.SendMessage("  [VystiaReligion status] - Show your religion status");
            from.SendMessage("  [VystiaReligion info] - Show this help");
            from.SendMessage("");
            from.SendMessage("Available Religions:");
            
            foreach (var religion in Enum.GetValues(typeof(VystiaReligion)))
            {
                if (religion != VystiaReligion.None)
                {
                    var def = GetReligionDefinition(religion);
                    from.SendMessage($"  {religion} - {def?.Description}");
                }
            }
        }

        /// <summary>
        /// Join a religion
        /// </summary>
        private static void JoinReligion(Mobile from, CommandEventArgs e)
        {
            if (e.Length < 2)
            {
                from.SendMessage("Usage: [VystiaReligion join <religion>");
                return;
            }

            if (!Enum.TryParse<VystiaReligion>(e.GetString(1), true, out var religion) || religion == VystiaReligion.None)
            {
                from.SendMessage("Invalid religion. Available religions:");
                foreach (var r in Enum.GetValues(typeof(VystiaReligion)))
                {
                    if (r != VystiaReligion.None)
                        from.SendMessage($"  {r}");
                }
                return;
            }

            SetPlayerReligion(from, religion);
        }

        /// <summary>
        /// Leave current religion
        /// </summary>
        private static void LeaveReligion(Mobile from)
        {
            SetPlayerReligion(from, VystiaReligion.None);
        }

        /// <summary>
        /// Perform tithe
        /// </summary>
        private static void PerformTithe(Mobile from, CommandEventArgs e)
        {
            if (e.Length < 2)
            {
                from.SendMessage("Usage: [VystiaReligion tithe <amount>");
                return;
            }

            if (!int.TryParse(e.GetString(1), out int amount) || amount <= 0)
            {
                from.SendMessage("Invalid amount. Please specify a positive number.");
                return;
            }

            PerformTithe(from, amount);
        }

        /// <summary>
        /// Show religion status
        /// </summary>
        private static void ShowReligionStatus(Mobile from)
        {
            var religion = GetPlayerReligion(from);
            var tier = GetPietyTier(from);
            var playerData = GetPlayerData(from);

            from.SendMessage("=== RELIGION STATUS ===");
            from.SendMessage($"Religion: {religion}");
            from.SendMessage($"Piety Tier: {tier}");
            from.SendMessage($"Piety: {playerData.Piety}");
            from.SendMessage($"Last Prayer: {playerData.LastPrayer:yyyy-MM-dd HH:mm:ss}");
        }

        /// <summary>
        /// Initialize religion definitions
        /// </summary>
        private static void InitializeReligions()
        {
            s_ReligionDefinitions[VystiaReligion.FrosthelmFaith] = new VystiaReligionDefinition
            {
                Religion = VystiaReligion.FrosthelmFaith,
                Name = "Frosthelm Faith",
                Description = "The faith of ice and snow spirits",
                Color = 1153, // Ice blue
                HolySite = "Frosthelm Cathedral",
                HolySiteLocation = new Point3D(1496, 1628, 10), // Example location
                Deity = "Frost Lords",
                Benefits = "Cold resistance, ice magic bonuses, winter protection"
            };

            s_ReligionDefinitions[VystiaReligion.SuryasSandscript] = new VystiaReligionDefinition
            {
                Religion = VystiaReligion.SuryasSandscript,
                Name = "Surya's Sandscript",
                Description = "The faith of sun and desert spirits",
                Color = 1358, // Desert orange
                HolySite = "Sun Temple",
                HolySiteLocation = new Point3D(527, 2233, 0), // Example location
                Deity = "Surya",
                Benefits = "Heat resistance, fire magic bonuses, desert protection"
            };

            s_ReligionDefinitions[VystiaReligion.LunarasCovenant] = new VystiaReligionDefinition
            {
                Religion = VystiaReligion.LunarasCovenant,
                Name = "Lunara's Covenant",
                Description = "The faith of nature's balance",
                Color = 1273, // Forest green
                HolySite = "Nature's Altar",
                HolySiteLocation = new Point3D(1728, 352, 0), // Example location
                Deity = "Lunara",
                Benefits = "Poison resistance, nature magic bonuses, animal friendship"
            };

            s_ReligionDefinitions[VystiaReligion.CelestisArcanum] = new VystiaReligionDefinition
            {
                Religion = VystiaReligion.CelestisArcanum,
                Name = "Celestis Arcanum",
                Description = "The faith of cosmic knowledge",
                Color = 1104, // Cosmic blue
                HolySite = "Observatory",
                HolySiteLocation = new Point3D(2500, 400, 0), // Example location
                Deity = "Celestial Beings",
                Benefits = "Magic resistance, arcane bonuses, time manipulation"
            };

            s_ReligionDefinitions[VystiaReligion.OceanasCovenant] = new VystiaReligionDefinition
            {
                Religion = VystiaReligion.OceanasCovenant,
                Name = "Oceana's Covenant",
                Description = "The faith of ocean and tides",
                Color = 1103, // Ocean blue
                HolySite = "Ocean Shrine",
                HolySiteLocation = new Point3D(2731, 3462, 0), // Example location
                Deity = "Oceana",
                Benefits = "Water resistance, water magic bonuses, sailing protection"
            };

            s_ReligionDefinitions[VystiaReligion.VoidwalkersPath] = new VystiaReligionDefinition
            {
                Religion = VystiaReligion.VoidwalkersPath,
                Name = "Voidwalkers Path",
                Description = "The faith of void and shadows",
                Color = 1102, // Void black
                HolySite = "Void Altar",
                HolySiteLocation = new Point3D(1000, 1000, 0), // Example location
                Deity = "Void Beings",
                Benefits = "Shadow resistance, void magic bonuses, dimensional protection"
            };
        }

        /// <summary>
        /// Get all religion definitions
        /// </summary>
        public static Dictionary<VystiaReligion, VystiaReligionDefinition> GetAllReligions()
        {
            return new Dictionary<VystiaReligion, VystiaReligionDefinition>(s_ReligionDefinitions);
        }

        /// <summary>
        /// Get system statistics
        /// </summary>
        public static VystiaReligionStatistics GetStatistics()
        {
            lock (s_Lock)
            {
                var stats = new VystiaReligionStatistics
                {
                    TotalPlayers = s_PlayerData.Count,
                    ReligionDistribution = new Dictionary<VystiaReligion, int>(),
                    PietyDistribution = new Dictionary<VystiaPietyTier, int>(),
                    LastUpdate = DateTime.UtcNow
                };

                // Count religion distribution
                foreach (var playerData in s_PlayerData.Values)
                {
                    if (stats.ReligionDistribution.ContainsKey(playerData.Religion))
                        stats.ReligionDistribution[playerData.Religion]++;
                    else
                        stats.ReligionDistribution[playerData.Religion] = 1;

                    var tier = GetPietyTier(playerData.Player);
                    if (stats.PietyDistribution.ContainsKey(tier))
                        stats.PietyDistribution[tier]++;
                    else
                        stats.PietyDistribution[tier] = 1;
                }

                return stats;
            }
        }
    }

    /// <summary>
    /// Vystia religion definition
    /// </summary>
    public class VystiaReligionDefinition
    {
        public VystiaReligion Religion { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Color { get; set; }
        public string HolySite { get; set; }
        public Point3D HolySiteLocation { get; set; }
        public string Deity { get; set; }
        public string Benefits { get; set; }
    }

    /// <summary>
    /// Vystia religion player data
    /// </summary>
    public class VystiaReligionPlayerData
    {
        public Mobile Player { get; set; }
        public VystiaReligion Religion { get; set; }
        public int Piety { get; set; }
        public DateTime LastPrayer { get; set; }
        public DateTime LastPilgrimage { get; set; }
        public VystiaPietyTier LastMilestoneTier { get; set; }

        public VystiaReligionPlayerData(Mobile player)
        {
            Player = player;
            Religion = VystiaReligion.None;
            Piety = 0;
            LastPrayer = DateTime.MinValue;
            LastPilgrimage = DateTime.MinValue;
            LastMilestoneTier = VystiaPietyTier.None;
        }
    }

    /// <summary>
    /// Vystia piety tiers
    /// </summary>
    public enum VystiaPietyTier
    {
        None = 0,
        Novice = 1,
        Believer = 2,
        Devoted = 3,
        Faithful = 4,
        Devout = 5
    }

    /// <summary>
    /// Vystia religion statistics
    /// </summary>
    public class VystiaReligionStatistics
    {
        public int TotalPlayers { get; set; }
        public Dictionary<VystiaReligion, int> ReligionDistribution { get; set; }
        public Dictionary<VystiaPietyTier, int> PietyDistribution { get; set; }
        public DateTime LastUpdate { get; set; }

        public VystiaReligionStatistics()
        {
            ReligionDistribution = new Dictionary<VystiaReligion, int>();
            PietyDistribution = new Dictionary<VystiaPietyTier, int>();
        }
    }
}
