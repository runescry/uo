using System;
using Server;
using Server.Mobiles;

namespace Server.Custom.VystiaClasses.Quests
{
    /// <summary>
    /// Detects when players kill creatures and advances DefeatBoss waypoints for DynamicQuests.
    /// Uses ServUO's global EventSink.OnKilledBy event.
    /// </summary>
    public static class QuestKillWaypointDetector
    {
        private static bool s_Initialized;

        public static void Initialize()
        {
            if (s_Initialized)
                return;

            s_Initialized = true;
            EventSink.OnKilledBy += OnKilledBy;
            Console.WriteLine("QuestKillWaypointDetector: Kill waypoint detection initialized.");
        }

        private static void OnKilledBy(OnKilledByEventArgs e)
        {
            if (e == null)
                return;

            if (!(e.KilledBy is PlayerMobile pm))
                return;

            Mobile killed = e.Killed;
            if (killed == null || killed.Deleted)
                return;

            var tracker = VystiaQuestTracker.GetTracker(pm);
            if (tracker == null)
                return;

            string killedTypeName = killed.GetType().Name;

            foreach (var questId in tracker.GetActiveQuests())
            {
                var quest = DynamicQuestManager.GetQuest(questId);
                if (quest == null)
                    continue;

                var progress = tracker.GetProgress(questId);
                var current = quest.GetCurrentWaypoint(progress);
                if (current == null)
                    continue;

                if (current.Condition != WaypointCondition.DefeatBoss)
                    continue;

                // If TargetTypeName is empty, accept any kill (useful for generic objectives)
                string required = (current.TargetTypeName ?? "").Trim();
                bool matches = string.IsNullOrEmpty(required) ||
                               required.Equals(killedTypeName, StringComparison.OrdinalIgnoreCase);

                if (!matches)
                    continue;

                VystiaQuestSystem.CompleteWaypoint(pm, questId, current.WaypointID);

                pm.SendMessage($"Creature defeated: {killedTypeName}. Waypoint complete: {current.Name}");

                // If this was the FINAL boss completion step, auto-complete the quest.
                // This prevents "stuck in quest mode" when the quest ends with a kill instead of a final NPC turn-in.
                var completion = quest.GetCompletion();
                if (completion != null &&
                    completion.Type == WaypointType.BossCompletion &&
                    completion.WaypointID == current.WaypointID)
                {
                    var progressAfter = tracker.GetProgress(questId);
                    var next = quest.GetCurrentWaypoint(progressAfter);

                    if (next == null)
                    {
                        pm.SendMessage($"All objectives complete: {quest.Title}");
                        VystiaQuestSystem.CompleteQuest(pm, questId);
                    }
                    else
                    {
                        pm.SendMessage($"Next objective: {next.Name}");
                    }
                }
            }
        }
    }
}


