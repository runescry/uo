// Vystia Hiking System
// Allows players to travel to previously discovered locations

using System;
using System.Collections.Generic;
using System.IO;
using Server;
using Server.Items;
using Server.Mobiles;
using Server.Gumps;
using Server.Commands;
using Server.Network;

namespace Server.Custom.VystiaClasses.Systems
{
    #region Waypoint Data

    /// <summary>
    /// Represents a discovered location/waypoint
    /// </summary>
    public class HikingWaypoint
    {
        public string Name { get; set; }
        public Point3D Location { get; set; }
        public Map Map { get; set; }
        public string RegionName { get; set; }
        public DateTime DiscoveredDate { get; set; }
        public int TimesVisited { get; set; }

        public HikingWaypoint(string name, Point3D location, Map map, string regionName)
        {
            Name = name;
            Location = location;
            Map = map;
            RegionName = regionName;
            DiscoveredDate = DateTime.UtcNow;
            TimesVisited = 0;
        }

        public void Serialize(GenericWriter writer)
        {
            writer.Write(0); // version
            writer.Write(Name);
            writer.Write(Location);
            writer.Write(Map);
            writer.Write(RegionName ?? "");
            writer.Write(DiscoveredDate);
            writer.Write(TimesVisited);
        }

        public static HikingWaypoint Deserialize(GenericReader reader)
        {
            int version = reader.ReadInt();
            string name = reader.ReadString();
            Point3D location = reader.ReadPoint3D();
            Map map = reader.ReadMap();
            string region = reader.ReadString();
            DateTime discovered = reader.ReadDateTime();
            int visits = reader.ReadInt();

            var wp = new HikingWaypoint(name, location, map, region);
            wp.DiscoveredDate = discovered;
            wp.TimesVisited = visits;
            return wp;
        }
    }

    #endregion

    #region Hiking System

    /// <summary>
    /// Main hiking system - manages waypoints and travel
    /// </summary>
    public static class VystiaHikingSystem
    {
        private static readonly Dictionary<PlayerMobile, List<HikingWaypoint>> s_PlayerWaypoints = new Dictionary<PlayerMobile, List<HikingWaypoint>>();

        // Travel costs
        private const int BaseTravelCost = 100; // Gold
        private const int DistanceCostPer100Tiles = 10; // Additional gold per 100 tiles

        // Cooldown between travels
        private static readonly TimeSpan TravelCooldown = TimeSpan.FromMinutes(5);
        private static readonly Dictionary<PlayerMobile, DateTime> s_TravelCooldowns = new Dictionary<PlayerMobile, DateTime>();

        // Save file path
        private static readonly string SavePath = Path.Combine("Saves/Vystia", "HikingWaypoints.bin");

        /// <summary>
        /// Initialize the hiking system
        /// </summary>
        public static void Initialize()
        {
            EventSink.WorldSave += OnWorldSave;
            EventSink.WorldLoad += OnWorldLoad;

            Console.WriteLine("[Vystia] Hiking System initialized.");
        }

        /// <summary>
        /// Get waypoints for a player
        /// </summary>
        public static List<HikingWaypoint> GetWaypoints(PlayerMobile pm)
        {
            if (pm == null)
                return new List<HikingWaypoint>();

            if (!s_PlayerWaypoints.TryGetValue(pm, out var waypoints))
            {
                waypoints = new List<HikingWaypoint>();
                s_PlayerWaypoints[pm] = waypoints;
            }

            return waypoints;
        }

        /// <summary>
        /// Discover a new waypoint at current location
        /// </summary>
        public static bool DiscoverWaypoint(PlayerMobile pm, string name)
        {
            if (pm == null || string.IsNullOrEmpty(name))
                return false;

            var waypoints = GetWaypoints(pm);

            // Check for duplicate name
            foreach (var wp in waypoints)
            {
                if (wp.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
                {
                    pm.SendMessage(0x22, "You already have a waypoint with that name.");
                    return false;
                }
            }

            // Check for nearby existing waypoint
            foreach (var wp in waypoints)
            {
                if (wp.Map == pm.Map && wp.Location.X == pm.Location.X && wp.Location.Y == pm.Location.Y)
                {
                    pm.SendMessage(0x22, "You have already marked this location.");
                    return false;
                }
            }

            // Create new waypoint
            string regionName = pm.Region?.Name ?? "Unknown";
            var newWaypoint = new HikingWaypoint(name, pm.Location, pm.Map, regionName);
            waypoints.Add(newWaypoint);

            pm.SendMessage(0x35, "You mark this location as '{0}'.", name);
            pm.PlaySound(0x1FA);
            pm.FixedParticles(0x376A, 9, 32, 5005, EffectLayer.Waist);

            return true;
        }

        /// <summary>
        /// Remove a waypoint
        /// </summary>
        public static bool RemoveWaypoint(PlayerMobile pm, string name)
        {
            if (pm == null)
                return false;

            var waypoints = GetWaypoints(pm);

            for (int i = 0; i < waypoints.Count; i++)
            {
                if (waypoints[i].Name.Equals(name, StringComparison.OrdinalIgnoreCase))
                {
                    waypoints.RemoveAt(i);
                    pm.SendMessage(0x35, "Waypoint '{0}' removed.", name);
                    return true;
                }
            }

            pm.SendMessage(0x22, "No waypoint found with that name.");
            return false;
        }

        /// <summary>
        /// Travel to a waypoint
        /// </summary>
        public static bool TravelToWaypoint(PlayerMobile pm, HikingWaypoint waypoint)
        {
            if (pm == null || waypoint == null)
                return false;

            // Check cooldown
            if (s_TravelCooldowns.TryGetValue(pm, out var lastTravel))
            {
                var remaining = TravelCooldown - (DateTime.UtcNow - lastTravel);
                if (remaining > TimeSpan.Zero)
                {
                    pm.SendMessage(0x22, "You must rest before hiking again. Wait {0:F0} more seconds.", remaining.TotalSeconds);
                    return false;
                }
            }

            // Calculate travel cost
            int distance = (int)pm.GetDistanceToSqrt(waypoint.Location);
            int cost = BaseTravelCost + (distance / 100) * DistanceCostPer100Tiles;

            // Apply camping skill discount (up to 50% reduction)
            double campingSkill = pm.Skills[SkillName.Camping].Value;
            double discount = Math.Min(0.5, campingSkill / 200.0);
            cost = (int)(cost * (1.0 - discount));

            // Check gold
            Container pack = pm.Backpack;
            if (pack == null || pack.ConsumeTotal(typeof(Gold), cost) == false)
            {
                pm.SendMessage(0x22, "You need {0} gold to hike to that location.", cost);
                return false;
            }

            // Check if location is valid
            if (!waypoint.Map.CanFit(waypoint.Location, 16, true, true))
            {
                pm.SendMessage(0x22, "The destination is blocked. You cannot travel there.");
                // Refund gold
                pack.DropItem(new Gold(cost));
                return false;
            }

            // Perform travel
            pm.PlaySound(0x1FC);
            pm.FixedParticles(0x376A, 9, 32, 5030, EffectLayer.Waist);

            pm.MoveToWorld(waypoint.Location, waypoint.Map);

            pm.PlaySound(0x1FC);
            pm.FixedParticles(0x376A, 9, 32, 5030, EffectLayer.Waist);

            waypoint.TimesVisited++;
            s_TravelCooldowns[pm] = DateTime.UtcNow;

            pm.SendMessage(0x35, "You hike to {0}. (Cost: {1} gold)", waypoint.Name, cost);

            // Gain camping skill
            pm.CheckSkill(SkillName.Camping, 0, 100);

            return true;
        }

        /// <summary>
        /// Get travel cost to a waypoint
        /// </summary>
        public static int GetTravelCost(PlayerMobile pm, HikingWaypoint waypoint)
        {
            if (pm == null || waypoint == null)
                return 0;

            int distance = (int)pm.GetDistanceToSqrt(waypoint.Location);
            int cost = BaseTravelCost + (distance / 100) * DistanceCostPer100Tiles;

            double campingSkill = pm.Skills[SkillName.Camping].Value;
            double discount = Math.Min(0.5, campingSkill / 200.0);
            return (int)(cost * (1.0 - discount));
        }

        /// <summary>
        /// Save waypoint data
        /// </summary>
        private static void OnWorldSave(WorldSaveEventArgs e)
        {
            try
            {
                string dir = Path.GetDirectoryName(SavePath);
                if (!Directory.Exists(dir))
                    Directory.CreateDirectory(dir);

                BinaryFileWriter writer = new BinaryFileWriter(SavePath, true);

                writer.Write(s_PlayerWaypoints.Count);

                foreach (var kvp in s_PlayerWaypoints)
                {
                    writer.Write(kvp.Key);
                    writer.Write(kvp.Value.Count);

                    foreach (var wp in kvp.Value)
                    {
                        wp.Serialize(writer);
                    }
                }

                writer.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("[Vystia Hiking] Error saving waypoints: {0}", ex.Message);
            }
        }

        /// <summary>
        /// Load waypoint data
        /// </summary>
        private static void OnWorldLoad()
        {
            if (!File.Exists(SavePath))
                return;

            try
            {
                using (FileStream fs = new FileStream(SavePath, FileMode.Open))
                using (BinaryReader br = new BinaryReader(fs))
                {
                    GenericReader reader = new BinaryFileReader(br);

                    int playerCount = reader.ReadInt();

                    for (int i = 0; i < playerCount; i++)
                    {
                        Mobile m = reader.ReadMobile();
                        int waypointCount = reader.ReadInt();

                        var waypoints = new List<HikingWaypoint>();

                        for (int j = 0; j < waypointCount; j++)
                        {
                            waypoints.Add(HikingWaypoint.Deserialize(reader));
                        }

                        if (m is PlayerMobile pm)
                        {
                            s_PlayerWaypoints[pm] = waypoints;
                        }
                    }
                }

                Console.WriteLine("[Vystia Hiking] Loaded waypoint data for {0} players.", s_PlayerWaypoints.Count);
            }
            catch (Exception ex)
            {
                Console.WriteLine("[Vystia Hiking] Error loading waypoints: {0}", ex.Message);
            }
        }
    }

    #endregion

    #region Hiking Gump

    /// <summary>
    /// Gump for viewing and traveling to waypoints
    /// </summary>
    public class HikingGump : Gump
    {
        private PlayerMobile m_Player;
        private List<HikingWaypoint> m_Waypoints;
        private int m_Page;
        private const int ItemsPerPage = 8;

        public HikingGump(PlayerMobile pm, int page = 0) : base(50, 50)
        {
            m_Player = pm;
            m_Waypoints = VystiaHikingSystem.GetWaypoints(pm);
            m_Page = page;

            Closable = true;
            Disposable = true;
            Dragable = true;
            Resizable = false;

            AddPage(0);
            AddBackground(0, 0, 400, 450, 9200);
            AddLabel(150, 15, 0x35, "Hiking Waypoints");
            AddLabel(20, 40, 0, string.Format("Waypoints: {0}", m_Waypoints.Count));

            int startIndex = m_Page * ItemsPerPage;
            int endIndex = Math.Min(startIndex + ItemsPerPage, m_Waypoints.Count);

            int y = 70;
            for (int i = startIndex; i < endIndex; i++)
            {
                var wp = m_Waypoints[i];
                int cost = VystiaHikingSystem.GetTravelCost(pm, wp);

                AddButton(20, y, 4005, 4007, i + 1, GumpButtonType.Reply, 0); // Travel button
                AddLabel(60, y, 0x35, wp.Name);
                AddLabel(200, y, 0, wp.RegionName);
                AddLabel(300, y, 0, string.Format("{0}g", cost));
                AddButton(360, y, 4017, 4019, 1000 + i, GumpButtonType.Reply, 0); // Delete button

                y += 35;
            }

            // Navigation
            if (m_Page > 0)
            {
                AddButton(20, 400, 4014, 4016, 500, GumpButtonType.Reply, 0); // Previous
                AddLabel(55, 400, 0, "Previous");
            }

            if (endIndex < m_Waypoints.Count)
            {
                AddButton(320, 400, 4005, 4007, 501, GumpButtonType.Reply, 0); // Next
                AddLabel(280, 400, 0, "Next");
            }

            // Help text
            AddHtml(20, 360, 360, 30, "<basefont color=#888888>Click a waypoint to travel. Cost varies by distance.</basefont>", false, false);
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            if (m_Player == null || m_Player.Deleted)
                return;

            int buttonID = info.ButtonID;

            if (buttonID == 0)
                return;

            // Navigation
            if (buttonID == 500)
            {
                m_Player.SendGump(new HikingGump(m_Player, m_Page - 1));
                return;
            }

            if (buttonID == 501)
            {
                m_Player.SendGump(new HikingGump(m_Player, m_Page + 1));
                return;
            }

            // Delete waypoint
            if (buttonID >= 1000)
            {
                int index = buttonID - 1000;
                if (index >= 0 && index < m_Waypoints.Count)
                {
                    var wp = m_Waypoints[index];
                    VystiaHikingSystem.RemoveWaypoint(m_Player, wp.Name);
                    m_Player.SendGump(new HikingGump(m_Player, m_Page));
                }
                return;
            }

            // Travel to waypoint
            int waypointIndex = buttonID - 1;
            if (waypointIndex >= 0 && waypointIndex < m_Waypoints.Count)
            {
                VystiaHikingSystem.TravelToWaypoint(m_Player, m_Waypoints[waypointIndex]);
            }
        }
    }

    #endregion

    #region Hiking Commands

    /// <summary>
    /// Commands for the hiking system
    /// </summary>
    public static class HikingCommands
    {
        public static void Initialize()
        {
            CommandSystem.Register("MarkWaypoint", AccessLevel.Player, new CommandEventHandler(MarkWaypoint_OnCommand));
            CommandSystem.Register("MW", AccessLevel.Player, new CommandEventHandler(MarkWaypoint_OnCommand));
            CommandSystem.Register("Hike", AccessLevel.Player, new CommandEventHandler(Hike_OnCommand));
            CommandSystem.Register("Waypoints", AccessLevel.Player, new CommandEventHandler(Waypoints_OnCommand));
            CommandSystem.Register("WP", AccessLevel.Player, new CommandEventHandler(Waypoints_OnCommand));

            Console.WriteLine("[Vystia] Hiking commands registered.");
        }

        /// <summary>
        /// [MarkWaypoint <name> - Mark current location as a waypoint
        /// </summary>
        [Usage("MarkWaypoint <name>")]
        [Description("Mark your current location as a waypoint for hiking.")]
        private static void MarkWaypoint_OnCommand(CommandEventArgs e)
        {
            if (!(e.Mobile is PlayerMobile pm))
                return;

            if (e.Arguments.Length < 1)
            {
                pm.SendMessage("Usage: [MarkWaypoint <name>");
                return;
            }

            string name = string.Join(" ", e.Arguments);
            VystiaHikingSystem.DiscoverWaypoint(pm, name);
        }

        /// <summary>
        /// [Hike - Open the hiking waypoint gump
        /// </summary>
        [Usage("Hike")]
        [Description("Open the hiking waypoint menu to travel to discovered locations.")]
        private static void Hike_OnCommand(CommandEventArgs e)
        {
            if (e.Mobile is PlayerMobile pm)
            {
                pm.SendGump(new HikingGump(pm));
            }
        }

        /// <summary>
        /// [Waypoints - List all waypoints
        /// </summary>
        [Usage("Waypoints")]
        [Description("List all your discovered waypoints.")]
        private static void Waypoints_OnCommand(CommandEventArgs e)
        {
            if (!(e.Mobile is PlayerMobile pm))
                return;

            var waypoints = VystiaHikingSystem.GetWaypoints(pm);

            if (waypoints.Count == 0)
            {
                pm.SendMessage("You have no waypoints. Use [MarkWaypoint <name> to mark locations.");
                return;
            }

            pm.SendMessage(0x35, "=== Your Waypoints ({0}) ===", waypoints.Count);
            foreach (var wp in waypoints)
            {
                int cost = VystiaHikingSystem.GetTravelCost(pm, wp);
                pm.SendMessage("- {0} ({1}) - {2}g", wp.Name, wp.RegionName, cost);
            }
            pm.SendMessage("Use [Hike to travel to a waypoint.");
        }
    }

    #endregion

    #region Trail Map Item

    /// <summary>
    /// Trail map - item that reveals nearby waypoints
    /// </summary>
    public class VystiaTrailMap : Item
    {
        [Constructable]
        public VystiaTrailMap() : base(0x14EC) // Map graphic
        {
            Name = "trail map";
            Weight = 1.0;
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (!(from is PlayerMobile pm))
                return;

            if (!IsChildOf(pm.Backpack))
            {
                pm.SendLocalizedMessage(1042001);
                return;
            }

            // Auto-discover nearby interesting locations
            var region = pm.Region;
            if (region != null && !string.IsNullOrEmpty(region.Name))
            {
                string waypointName = region.Name + " Entrance";
                if (VystiaHikingSystem.DiscoverWaypoint(pm, waypointName))
                {
                    pm.SendMessage(0x35, "You study the map and discover a new location!");
                    Delete();
                }
                else
                {
                    pm.SendMessage("You already know this area well.");
                }
            }
            else
            {
                pm.SendMessage("The map doesn't reveal anything useful here.");
            }
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add("Reveals nearby waypoints");
        }

        public VystiaTrailMap(Serial serial) : base(serial) { }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt();
        }
    }

    #endregion
}
