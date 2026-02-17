/*
 * Vystia Zone Control System
 *
 * Defines zone types with different PvP rules, death penalties, and loot rules.
 *
 * Zone Types:
 * - Sanctuary (Green): No PvP, full protection, guards respond
 * - Contested (Yellow): Consensual PvP, reduced penalties
 * - Lawless (Red): Open PvP, full loot on death
 * - Extreme (Black): Hardcore PvP, permadeath risk, best rewards
 *
 * Based on design document specifications.
 */

using System;
using System.Collections.Generic;
using Server;
using Server.Mobiles;
using Server.Items;
using Server.Commands;
using Server.Regions;
using Server.Targeting;
using Server.Network;

namespace Server.Custom.VystiaClasses.Zones
{
    #region Enums

    /// <summary>
    /// Zone danger levels for Vystia
    /// </summary>
    public enum VystiaZoneType
    {
        Sanctuary,      // Green - Safe zone, no PvP
        Contested,      // Yellow - Consensual PvP
        Lawless,        // Red - Open PvP, full loot
        Extreme         // Black - Hardcore, permadeath risk
    }

    /// <summary>
    /// PvP flag status
    /// </summary>
    public enum PvPStatus
    {
        Protected,      // Cannot be attacked
        Flagged,        // Can be attacked, attacker gets consequences
        Criminal,       // Open target for all
        Murderer        // Kill-on-sight
    }

    #endregion

    #region Zone Data

    /// <summary>
    /// Configuration for a zone type
    /// </summary>
    public class ZoneConfig
    {
        public VystiaZoneType ZoneType { get; set; }
        public string Name { get; set; }
        public int Hue { get; set; }                    // Display color
        public bool AllowPvP { get; set; }              // Can players attack each other?
        public bool RequireConsent { get; set; }        // Need to be flagged for PvP?
        public bool GuardsRespond { get; set; }         // Do guards attack criminals?
        public double DeathPenalty { get; set; }        // Skill loss multiplier (0.0 = none)
        public double LootDropRate { get; set; }        // % of items dropped on death (0.0-1.0)
        public bool CanResurrectSelf { get; set; }      // Ankh/self-res allowed?
        public double XPMultiplier { get; set; }        // Bonus XP in dangerous zones
        public double GoldMultiplier { get; set; }      // Bonus gold drops
        public bool PermadeathRisk { get; set; }        // Character can be permanently lost?

        public ZoneConfig(VystiaZoneType type)
        {
            ZoneType = type;
            ApplyDefaults();
        }

        private void ApplyDefaults()
        {
            switch (ZoneType)
            {
                case VystiaZoneType.Sanctuary:
                    Name = "Sanctuary";
                    Hue = 0x47; // Green
                    AllowPvP = false;
                    RequireConsent = true;
                    GuardsRespond = true;
                    DeathPenalty = 0.0;
                    LootDropRate = 0.0;
                    CanResurrectSelf = true;
                    XPMultiplier = 1.0;
                    GoldMultiplier = 1.0;
                    PermadeathRisk = false;
                    break;

                case VystiaZoneType.Contested:
                    Name = "Contested";
                    Hue = 0x35; // Yellow
                    AllowPvP = true;
                    RequireConsent = true;
                    GuardsRespond = true;
                    DeathPenalty = 0.25;
                    LootDropRate = 0.1;  // 10% item loss
                    CanResurrectSelf = true;
                    XPMultiplier = 1.25;
                    GoldMultiplier = 1.25;
                    PermadeathRisk = false;
                    break;

                case VystiaZoneType.Lawless:
                    Name = "Lawless";
                    Hue = 0x22; // Red
                    AllowPvP = true;
                    RequireConsent = false;
                    GuardsRespond = false;
                    DeathPenalty = 0.5;
                    LootDropRate = 0.5;  // 50% item loss
                    CanResurrectSelf = true;
                    XPMultiplier = 1.5;
                    GoldMultiplier = 1.5;
                    PermadeathRisk = false;
                    break;

                case VystiaZoneType.Extreme:
                    Name = "Extreme";
                    Hue = 0x455; // Black
                    AllowPvP = true;
                    RequireConsent = false;
                    GuardsRespond = false;
                    DeathPenalty = 1.0;
                    LootDropRate = 1.0;  // Full loot
                    CanResurrectSelf = false;
                    XPMultiplier = 2.0;
                    GoldMultiplier = 2.0;
                    PermadeathRisk = true;
                    break;
            }
        }
    }

    #endregion

    #region Zone System

    /// <summary>
    /// Main zone management system
    /// </summary>
    public static class VystiaZoneSystem
    {
        private static Dictionary<VystiaZoneType, ZoneConfig> m_ZoneConfigs;
        private static Dictionary<string, VystiaZoneType> m_RegionZones;
        private static bool m_Enabled = true;

        public static bool Enabled
        {
            get { return m_Enabled; }
            set { m_Enabled = value; }
        }

        #region Initialization

        public static void Initialize()
        {
            // Initialize zone configurations
            m_ZoneConfigs = new Dictionary<VystiaZoneType, ZoneConfig>();
            foreach (VystiaZoneType type in Enum.GetValues(typeof(VystiaZoneType)))
            {
                m_ZoneConfigs[type] = new ZoneConfig(type);
            }

            // Initialize region to zone mappings
            m_RegionZones = new Dictionary<string, VystiaZoneType>(StringComparer.OrdinalIgnoreCase);
            SetupDefaultRegionZones();

            // Register commands
            CommandSystem.Register("ZoneInfo", AccessLevel.Player, new CommandEventHandler(ZoneInfo_OnCommand));
            CommandSystem.Register("ZI", AccessLevel.Player, new CommandEventHandler(ZoneInfo_OnCommand));
            CommandSystem.Register("SetZone", AccessLevel.GameMaster, new CommandEventHandler(SetZone_OnCommand));
            CommandSystem.Register("SZ", AccessLevel.GameMaster, new CommandEventHandler(SetZone_OnCommand));
            CommandSystem.Register("ZoneList", AccessLevel.GameMaster, new CommandEventHandler(ZoneList_OnCommand));
            CommandSystem.Register("ZL", AccessLevel.GameMaster, new CommandEventHandler(ZoneList_OnCommand));
            CommandSystem.Register("TogglePvP", AccessLevel.Player, new CommandEventHandler(TogglePvP_OnCommand));

            // Hook into combat system
            EventSink.PlayerDeath += OnPlayerDeath;

            Console.WriteLine("[Vystia] Zone Control System initialized with {0} zone types", m_ZoneConfigs.Count);
        }

        /// <summary>
        /// Set up default region to zone type mappings
        /// </summary>
        private static void SetupDefaultRegionZones()
        {
            // Towns and cities are Sanctuary zones
            SetRegionZone("Britain", VystiaZoneType.Sanctuary);
            SetRegionZone("Trinsic", VystiaZoneType.Sanctuary);
            SetRegionZone("Moonglow", VystiaZoneType.Sanctuary);
            SetRegionZone("Yew", VystiaZoneType.Sanctuary);
            SetRegionZone("Minoc", VystiaZoneType.Sanctuary);
            SetRegionZone("Vesper", VystiaZoneType.Sanctuary);
            SetRegionZone("Skara Brae", VystiaZoneType.Sanctuary);
            SetRegionZone("Jhelom", VystiaZoneType.Sanctuary);
            SetRegionZone("Magincia", VystiaZoneType.Sanctuary);
            SetRegionZone("Nujel'm", VystiaZoneType.Sanctuary);
            SetRegionZone("Serpent's Hold", VystiaZoneType.Sanctuary);
            SetRegionZone("Delucia", VystiaZoneType.Sanctuary);
            SetRegionZone("Papua", VystiaZoneType.Sanctuary);

            // Vystia main settlements
            SetRegionZone("Ironhaven", VystiaZoneType.Sanctuary);
            SetRegionZone("Frostholm", VystiaZoneType.Sanctuary);
            SetRegionZone("Verdantia", VystiaZoneType.Sanctuary);
            SetRegionZone("Crystalspire", VystiaZoneType.Sanctuary);
            SetRegionZone("Sunport", VystiaZoneType.Sanctuary);

            // Frontier/wilderness areas are Contested
            SetRegionZone("Frosthold Wilds", VystiaZoneType.Contested);
            SetRegionZone("Emberlands Border", VystiaZoneType.Contested);
            SetRegionZone("Verdantpeak Forest", VystiaZoneType.Contested);
            SetRegionZone("Desert Outskirts", VystiaZoneType.Contested);
            SetRegionZone("Shadowfen Marshes", VystiaZoneType.Contested);

            // Dangerous areas are Lawless
            SetRegionZone("Shadowfen Deep", VystiaZoneType.Lawless);
            SetRegionZone("Emberlands Core", VystiaZoneType.Lawless);
            SetRegionZone("Frosthold Peaks", VystiaZoneType.Lawless);
            SetRegionZone("Crystal Barrens", VystiaZoneType.Lawless);
            SetRegionZone("Obsidian Wastes", VystiaZoneType.Lawless);

            // Boss/endgame areas are Extreme
            SetRegionZone("ShadowVoid", VystiaZoneType.Extreme);
            SetRegionZone("Void Rift", VystiaZoneType.Extreme);
            SetRegionZone("Eternal Frost", VystiaZoneType.Extreme);
            SetRegionZone("Molten Core", VystiaZoneType.Extreme);
        }

        #endregion

        #region Zone Configuration

        /// <summary>
        /// Get configuration for a zone type
        /// </summary>
        public static ZoneConfig GetConfig(VystiaZoneType type)
        {
            return m_ZoneConfigs.TryGetValue(type, out ZoneConfig config) ? config : m_ZoneConfigs[VystiaZoneType.Sanctuary];
        }

        /// <summary>
        /// Set zone type for a region
        /// </summary>
        public static void SetRegionZone(string regionName, VystiaZoneType zoneType)
        {
            m_RegionZones[regionName] = zoneType;
        }

        /// <summary>
        /// Get zone type for a region
        /// </summary>
        public static VystiaZoneType GetRegionZone(string regionName)
        {
            if (string.IsNullOrEmpty(regionName))
                return VystiaZoneType.Contested;

            return m_RegionZones.TryGetValue(regionName, out VystiaZoneType type) ? type : VystiaZoneType.Contested;
        }

        /// <summary>
        /// Get zone type for a mobile's current location
        /// </summary>
        public static VystiaZoneType GetZoneType(Mobile m)
        {
            if (m == null || m.Map == null || m.Map == Map.Internal)
                return VystiaZoneType.Sanctuary;

            Region region = m.Region;
            while (region != null)
            {
                if (m_RegionZones.TryGetValue(region.Name ?? "", out VystiaZoneType type))
                    return type;
                region = region.Parent;
            }

            // Default based on map
            if (m.Map == Map.Felucca)
                return VystiaZoneType.Lawless;

            return VystiaZoneType.Contested;
        }

        /// <summary>
        /// Get zone configuration for a mobile's current location
        /// </summary>
        public static ZoneConfig GetZoneConfig(Mobile m)
        {
            return GetConfig(GetZoneType(m));
        }

        #endregion

        #region PvP Rules

        /// <summary>
        /// Check if PvP is allowed between two players
        /// </summary>
        public static bool CanAttack(Mobile attacker, Mobile defender)
        {
            if (!m_Enabled)
                return true;

            if (attacker == null || defender == null)
                return false;

            // Same checks for both locations
            ZoneConfig attackerZone = GetZoneConfig(attacker);
            ZoneConfig defenderZone = GetZoneConfig(defender);

            // Use most restrictive zone
            if (!attackerZone.AllowPvP || !defenderZone.AllowPvP)
            {
                attacker.SendMessage(0x22, "PvP is not allowed in this area.");
                return false;
            }

            // Check consent requirements
            if (attackerZone.RequireConsent || defenderZone.RequireConsent)
            {
                if (!IsPvPFlagged(attacker) || !IsPvPFlagged(defender))
                {
                    attacker.SendMessage(0x22, "Both players must be flagged for PvP in this zone.");
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Check if a player is flagged for PvP
        /// </summary>
        public static bool IsPvPFlagged(Mobile m)
        {
            if (m == null)
                return false;

            // Criminals and murderers are always flagged
            if (m.Criminal || m.Murderer)
                return true;

            // Check for PvP flag buff/status
            if (m is PlayerMobile pm)
            {
                // Check for voluntary PvP flag (stored in a simple way)
                return pm.Criminal; // Simplified - could use custom property
            }

            return false;
        }

        /// <summary>
        /// Toggle PvP flag for a player
        /// </summary>
        public static void TogglePvPFlag(PlayerMobile pm)
        {
            if (pm == null)
                return;

            ZoneConfig zone = GetZoneConfig(pm);

            if (!zone.AllowPvP)
            {
                pm.SendMessage("PvP is not available in this zone.");
                return;
            }

            if (!zone.RequireConsent)
            {
                pm.SendMessage("PvP consent is not required in this zone - combat is always enabled.");
                return;
            }

            // Toggle criminal flag as PvP indicator (simplified)
            if (pm.Criminal)
            {
                pm.Criminal = false;
                pm.SendMessage(0x35, "You are no longer flagged for PvP.");
            }
            else
            {
                pm.Criminal = true;
                pm.SendMessage(0x22, "You are now flagged for PvP. Other flagged players can attack you!");
            }
        }

        #endregion

        #region Death Handling

        /// <summary>
        /// Handle player death based on zone rules
        /// </summary>
        private static void OnPlayerDeath(PlayerDeathEventArgs e)
        {
            if (!m_Enabled)
                return;

            PlayerMobile pm = e.Mobile as PlayerMobile;
            if (pm == null)
                return;

            ZoneConfig zone = GetZoneConfig(pm);

            // Apply skill loss
            if (zone.DeathPenalty > 0)
            {
                ApplySkillLoss(pm, zone.DeathPenalty);
            }

            // Apply loot drop
            if (zone.LootDropRate > 0)
            {
                ApplyLootDrop(pm, zone.LootDropRate);
            }

            // Notify about resurrection restrictions
            if (!zone.CanResurrectSelf)
            {
                pm.SendMessage(0x22, "Warning: Self-resurrection is not available in this zone. " +
                    "You must be resurrected by another player or return to a healer.");
            }

            // Permadeath warning (not actually implemented - would need special handling)
            if (zone.PermadeathRisk)
            {
                pm.SendMessage(0x22, "DANGER: This is an Extreme zone. Repeated deaths may result in permanent consequences!");
            }
        }

        /// <summary>
        /// Apply skill loss on death
        /// </summary>
        private static void ApplySkillLoss(PlayerMobile pm, double multiplier)
        {
            if (pm == null || multiplier <= 0)
                return;

            double totalLoss = 0;

            foreach (Skill skill in pm.Skills)
            {
                if (skill.Base > 0)
                {
                    double loss = skill.Base * 0.005 * multiplier; // 0.5% per point of multiplier
                    if (loss > 0.1) // Minimum loss threshold
                    {
                        skill.Base -= loss;
                        totalLoss += loss;
                    }
                }
            }

            if (totalLoss > 0)
            {
                pm.SendMessage(0x22, "You have lost {0:F1} total skill points due to death in a dangerous zone.",
                    totalLoss);
            }
        }

        /// <summary>
        /// Apply loot drop on death
        /// </summary>
        private static void ApplyLootDrop(PlayerMobile pm, double dropRate)
        {
            if (pm == null || dropRate <= 0)
                return;

            Container backpack = pm.Backpack;
            if (backpack == null)
                return;

            List<Item> toDrop = new List<Item>();

            foreach (Item item in backpack.Items)
            {
                if (item is BankCheck || item is Gold)
                    continue; // Don't drop money

                if (item.LootType == LootType.Blessed || item.LootType == LootType.Newbied)
                    continue; // Don't drop blessed/newbied items

                if (Utility.RandomDouble() < dropRate)
                {
                    toDrop.Add(item);
                }
            }

            if (toDrop.Count > 0)
            {
                Corpse corpse = pm.Corpse as Corpse;

                foreach (Item item in toDrop)
                {
                    if (corpse != null)
                    {
                        item.MoveToWorld(corpse.Location, corpse.Map);
                    }
                    else
                    {
                        item.MoveToWorld(pm.Location, pm.Map);
                    }
                }

                pm.SendMessage(0x22, "{0} item(s) dropped from your pack due to death in a dangerous zone.",
                    toDrop.Count);
            }
        }

        #endregion

        #region Commands

        [Usage("ZoneInfo")]
        [Description("Display information about the current zone.")]
        private static void ZoneInfo_OnCommand(CommandEventArgs e)
        {
            Mobile from = e.Mobile;

            VystiaZoneType zoneType = GetZoneType(from);
            ZoneConfig config = GetConfig(zoneType);

            from.SendMessage("=== Zone Information ===");
            from.SendMessage("Zone Type: {0}", config.Name);
            from.SendMessage("Region: {0}", from.Region?.Name ?? "Wilderness");
            from.SendMessage("");
            from.SendMessage("PvP Allowed: {0}", config.AllowPvP ? "Yes" : "No");

            if (config.AllowPvP)
            {
                from.SendMessage("Consent Required: {0}", config.RequireConsent ? "Yes" : "No");
            }

            from.SendMessage("Guards Respond: {0}", config.GuardsRespond ? "Yes" : "No");
            from.SendMessage("");
            from.SendMessage("Death Penalty: {0:P0} skill loss", config.DeathPenalty);
            from.SendMessage("Loot Drop: {0:P0} of items", config.LootDropRate);
            from.SendMessage("Self-Res Allowed: {0}", config.CanResurrectSelf ? "Yes" : "No");
            from.SendMessage("");
            from.SendMessage("XP Bonus: {0:P0}", config.XPMultiplier - 1);
            from.SendMessage("Gold Bonus: {0:P0}", config.GoldMultiplier - 1);

            if (config.PermadeathRisk)
            {
                from.SendMessage(0x22, "WARNING: Permadeath risk in this zone!");
            }
        }

        [Usage("SetZone <regionName> <Sanctuary|Contested|Lawless|Extreme>")]
        [Description("Set the zone type for a region.")]
        private static void SetZone_OnCommand(CommandEventArgs e)
        {
            Mobile from = e.Mobile;

            if (e.Arguments.Length < 2)
            {
                from.SendMessage("Usage: [SetZone <regionName> <Sanctuary|Contested|Lawless|Extreme>");
                return;
            }

            string regionName = e.Arguments[0];
            string zoneTypeStr = e.Arguments[1];

            if (!Enum.TryParse(zoneTypeStr, true, out VystiaZoneType zoneType))
            {
                from.SendMessage("Invalid zone type. Use: Sanctuary, Contested, Lawless, or Extreme");
                return;
            }

            SetRegionZone(regionName, zoneType);
            from.SendMessage("Region '{0}' set to zone type: {1}", regionName, zoneType);
        }

        [Usage("ZoneList")]
        [Description("List all configured zones.")]
        private static void ZoneList_OnCommand(CommandEventArgs e)
        {
            Mobile from = e.Mobile;

            from.SendMessage("=== Configured Zone Types ===");

            var grouped = new Dictionary<VystiaZoneType, List<string>>();
            foreach (VystiaZoneType type in Enum.GetValues(typeof(VystiaZoneType)))
            {
                grouped[type] = new List<string>();
            }

            foreach (var kvp in m_RegionZones)
            {
                grouped[kvp.Value].Add(kvp.Key);
            }

            foreach (VystiaZoneType type in Enum.GetValues(typeof(VystiaZoneType)))
            {
                ZoneConfig config = GetConfig(type);
                from.SendMessage("");
                from.SendMessage("--- {0} (Hue: 0x{1:X}) ---", config.Name, config.Hue);

                if (grouped[type].Count == 0)
                {
                    from.SendMessage("  (No regions assigned)");
                }
                else
                {
                    foreach (string region in grouped[type])
                    {
                        from.SendMessage("  - {0}", region);
                    }
                }
            }
        }

        [Usage("TogglePvP")]
        [Description("Toggle your PvP flag in contested zones.")]
        private static void TogglePvP_OnCommand(CommandEventArgs e)
        {
            if (e.Mobile is PlayerMobile pm)
            {
                TogglePvPFlag(pm);
            }
        }

        #endregion
    }

    #endregion

    #region Zone Region Override

    /// <summary>
    /// Custom region class for Vystia zones
    /// Override this to create actual regions with zone rules
    /// </summary>
    public class VystiaZoneRegion : BaseRegion
    {
        private VystiaZoneType m_ZoneType;

        public VystiaZoneType ZoneType
        {
            get { return m_ZoneType; }
            set { m_ZoneType = value; }
        }

        public ZoneConfig Config
        {
            get { return VystiaZoneSystem.GetConfig(m_ZoneType); }
        }

        public VystiaZoneRegion(string name, Map map, int priority, params Rectangle3D[] area)
            : base(name, map, priority, area)
        {
            m_ZoneType = VystiaZoneType.Contested;
        }

        public VystiaZoneRegion(string name, Map map, int priority, VystiaZoneType zoneType, params Rectangle3D[] area)
            : base(name, map, priority, area)
        {
            m_ZoneType = zoneType;
            VystiaZoneSystem.SetRegionZone(name, zoneType);
        }

        public override void OnEnter(Mobile m)
        {
            base.OnEnter(m);

            if (m is PlayerMobile pm)
            {
                ZoneConfig config = Config;

                pm.SendMessage(config.Hue, "You have entered {0} - a {1} zone.",
                    Name, config.Name);

                if (config.PermadeathRisk)
                {
                    pm.SendMessage(0x22, "WARNING: This is an Extreme zone with permadeath risk!");
                }
                else if (config.ZoneType == VystiaZoneType.Lawless)
                {
                    pm.SendMessage(0x22, "Caution: Open PvP is enabled. Guard your belongings!");
                }
            }
        }

        public override void OnExit(Mobile m)
        {
            base.OnExit(m);

            if (m is PlayerMobile pm)
            {
                pm.SendMessage("You have left {0}.", Name);
            }
        }

        public override bool AllowHousing(Mobile from, Point3D p)
        {
            // No housing in Extreme zones
            if (m_ZoneType == VystiaZoneType.Extreme)
                return false;

            return base.AllowHousing(from, p);
        }

        public override void AlterLightLevel(Mobile m, ref int global, ref int personal)
        {
            base.AlterLightLevel(m, ref global, ref personal);

            // Extreme zones are darker
            if (m_ZoneType == VystiaZoneType.Extreme)
            {
                global = Math.Max(global, 12); // Darker ambient
            }
        }
    }

    #endregion
}
