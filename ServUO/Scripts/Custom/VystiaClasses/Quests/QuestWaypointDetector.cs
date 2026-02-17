using System;
using System.Collections.Generic;
using System.Linq;
using Server;
using Server.Mobiles;

namespace Server.Custom.VystiaClasses.Quests
{
    /// <summary>
    /// Detects when players enter waypoint locations and completes location-based objectives
    /// </summary>
    public static class QuestWaypointDetector
    {
        private static bool s_Initialized = false;
        private static TimeSpan CheckInterval = TimeSpan.FromSeconds(2.0);
        private static DateTime s_LastCheck = DateTime.MinValue;

        // Cache of players with active location waypoints
        private static Dictionary<PlayerMobile, List<LocationWaypointInfo>> s_ActiveLocationWaypoints = new Dictionary<PlayerMobile, List<LocationWaypointInfo>>();

        public static void Initialize()
        {
            if (s_Initialized)
                return;

            s_Initialized = true;

            // Hook into movement events
            EventSink.Movement += OnMovement;

            Console.WriteLine("QuestWaypointDetector: Location waypoint detection initialized.");
        }

        private static void OnMovement(MovementEventArgs e)
        {
            if (!(e.Mobile is PlayerMobile pm))
                return;

            // Throttle checks to reduce overhead
            if (DateTime.UtcNow - s_LastCheck < CheckInterval)
                return;

            s_LastCheck = DateTime.UtcNow;

            CheckPlayerWaypoints(pm);
        }

        /// <summary>
        /// Check if player has entered any location-based waypoints
        /// </summary>
        private static void CheckPlayerWaypoints(PlayerMobile pm)
        {
            var tracker = VystiaQuestTracker.GetTracker(pm);
            if (tracker == null)
                return;

            var activeQuests = tracker.GetActiveQuests();
            if (activeQuests.Count == 0)
                return;

            foreach (var questId in activeQuests)
            {
                var quest = DynamicQuestManager.GetQuest(questId);
                if (quest == null)
                    continue;

                var progress = tracker.GetProgress(questId);
                var currentWaypoint = quest.GetCurrentWaypoint(progress);

                if (currentWaypoint == null)
                    continue;

                // Only check location-based waypoints
                if (currentWaypoint.Condition != WaypointCondition.ReachLocation)
                    continue;

                // Check if player is in range
                if (IsPlayerAtWaypoint(pm, currentWaypoint))
                {
                    CompleteLocationWaypoint(pm, quest, currentWaypoint);
                }
            }
        }

        /// <summary>
        /// Check if player is within the waypoint's radius
        /// </summary>
        private static bool IsPlayerAtWaypoint(PlayerMobile pm, QuestWaypoint waypoint)
        {
            // Must be on same map
            if (pm.Map != waypoint.Map)
                return false;

            // Check by region name if specified
            if (!string.IsNullOrEmpty(waypoint.RegionName))
            {
                var region = pm.Region;
                while (region != null)
                {
                    if (region.Name != null && region.Name.IndexOf(waypoint.RegionName, StringComparison.OrdinalIgnoreCase) >= 0)
                        return true;
                    region = region.Parent;
                }
                return false;
            }

            // Check by distance to location
            if (waypoint.Location != Point3D.Zero)
            {
                double distance = pm.GetDistanceToSqrt(waypoint.Location);
                return distance <= waypoint.Radius;
            }

            return false;
        }

        /// <summary>
        /// Complete a location-based waypoint
        /// </summary>
        private static void CompleteLocationWaypoint(PlayerMobile pm, DynamicQuest quest, QuestWaypoint waypoint)
        {
            string key = waypoint.GetObjectiveKeyForProgress();

            // Update quest progress
            VystiaQuestSystem.UpdateProgress(pm, key, 1);

            // Notify player
            pm.SendMessage(0x44, $"Location reached: {waypoint.Name}");

            // Visual/sound effects
            pm.FixedParticles(0x376A, 9, 32, 5008, EffectLayer.Waist);
            pm.PlaySound(0x1F8);

            // Check if quest is now completeable
            var tracker = VystiaQuestTracker.GetTracker(pm);
            if (tracker != null)
            {
                var progress = tracker.GetProgress(quest.QuestID);
                if (quest.AreObjectivesComplete(progress))
                {
                    pm.SendMessage(0x35, $"Quest '{quest.Title}' objectives complete! Return to the quest giver.");
                }
                else
                {
                    // Show next waypoint
                    var nextWaypoint = quest.GetCurrentWaypoint(progress);
                    if (nextWaypoint != null)
                    {
                        pm.SendMessage($"Next objective: {nextWaypoint.Name}");
                    }
                }
            }
        }

        /// <summary>
        /// Force check a specific player's waypoints (for GM testing)
        /// </summary>
        public static void ForceCheck(PlayerMobile pm)
        {
            if (pm == null)
                return;

            CheckPlayerWaypoints(pm);
        }

        /// <summary>
        /// Get debug info about a player's active location waypoints
        /// </summary>
        public static string GetDebugInfo(PlayerMobile pm)
        {
            var tracker = VystiaQuestTracker.GetTracker(pm);
            if (tracker == null)
                return "No quest tracker found.";

            var activeQuests = tracker.GetActiveQuests();
            if (activeQuests.Count == 0)
                return "No active quests.";

            var result = new System.Text.StringBuilder();
            result.AppendLine($"Active Quests: {activeQuests.Count}");

            foreach (var questId in activeQuests)
            {
                var quest = DynamicQuestManager.GetQuest(questId);
                if (quest == null)
                    continue;

                result.AppendLine($"  Quest: {quest.Title} (ID: {questId})");

                var progress = tracker.GetProgress(questId);
                var currentWp = quest.GetCurrentWaypoint(progress);

                if (currentWp != null)
                {
                    result.AppendLine($"    Current Waypoint: {currentWp.Name} ({currentWp.Type})");
                    result.AppendLine($"    Condition: {currentWp.Condition}");

                    if (currentWp.Condition == WaypointCondition.ReachLocation)
                    {
                        result.AppendLine($"    Location: {currentWp.Location} (Radius: {currentWp.Radius})");
                        result.AppendLine($"    Map: {currentWp.Map?.Name ?? "Not set"}");

                        if (!string.IsNullOrEmpty(currentWp.RegionName))
                            result.AppendLine($"    Region: {currentWp.RegionName}");

                        bool atLocation = IsPlayerAtWaypoint(pm, currentWp);
                        result.AppendLine($"    Player At Location: {atLocation}");
                    }
                }
                else
                {
                    result.AppendLine("    All waypoints complete");
                }
            }

            return result.ToString();
        }
    }

    /// <summary>
    /// Helper class for caching location waypoint info
    /// </summary>
    internal class LocationWaypointInfo
    {
        public int QuestID { get; set; }
        public int WaypointID { get; set; }
        public Point3D Location { get; set; }
        public Map Map { get; set; }
        public int Radius { get; set; }
        public string RegionName { get; set; }
    }
}
