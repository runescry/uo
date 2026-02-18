using System;
using System.Collections.Generic;
using System.Linq;
using Server;
using Server.Mobiles;
using Server.Items;
using Server.Engines.XmlSpawner2;

namespace Server.Custom.VystiaClasses.Quests
{
    /// <summary>
    /// Core Vystia Quest System
    /// Manages class-specific quests for all 26 classes
    /// </summary>
    public static class VystiaQuestSystem
    {
        // Quest registry
        private static Dictionary<int, VystiaQuest> s_Quests = new Dictionary<int, VystiaQuest>();
        private static int s_NextQuestID = 1;

        /// <summary>
        /// Register a quest in the system
        /// </summary>
        public static int RegisterQuest(VystiaQuest quest)
        {
            int id = s_NextQuestID++;
            quest.QuestID = id;
            s_Quests[id] = quest;
            return id;
        }

        /// <summary>
        /// Get a quest by ID
        /// </summary>
        public static VystiaQuest GetQuest(int questID)
        {
            if (s_Quests.TryGetValue(questID, out VystiaQuest quest))
                return quest;
            return null;
        }

        /// <summary>
        /// Unregister (remove) a quest from the registry.
        /// Note: This does not clean up player trackers; it simply removes the quest definition.
        /// </summary>
        public static bool UnregisterQuest(int questID)
        {
            return s_Quests.Remove(questID);
        }

        /// <summary>
        /// Remove a quest from all players' active and completed quest tracking.
        /// This should be called when a quest is deleted to prevent "Vystia Quest #X" from appearing in quest logs.
        /// </summary>
        public static void RemoveQuestFromAllPlayers(int questID)
        {
            int removedCount = 0;
            
            foreach (Mobile m in World.Mobiles.Values)
            {
                if (m is PlayerMobile pm)
                {
                    var tracker = VystiaQuestTracker.GetTracker(pm);
                    if (tracker != null)
                    {
                        bool hadActive = tracker.IsActive(questID);
                        bool hadCompleted = tracker.HasCompleted(questID);
                        
                        if (hadActive || hadCompleted)
                        {
                            tracker.RemoveQuest(questID);
                            removedCount++;
                        }
                    }
                }
            }
            
            if (removedCount > 0)
            {
                Console.WriteLine($"[VystiaQuestSystem] Removed quest [{questID}] from {removedCount} player(s).");
            }
        }

        /// <summary>
        /// Get all quests for a specific class
        /// </summary>
        public static List<VystiaQuest> GetClassQuests(PlayerClassTypeV2 classType)
        {
            List<VystiaQuest> result = new List<VystiaQuest>();
            foreach (var quest in s_Quests.Values)
            {
                if (quest.RequiredClass == classType || quest.RequiredClass == PlayerClassTypeV2.None)
                    result.Add(quest);
            }
            return result;
        }

        /// <summary>
        /// Check if player has completed a quest
        /// </summary>
        public static bool HasCompletedQuest(PlayerMobile pm, int questID)
        {
            var tracker = VystiaQuestTracker.GetTracker(pm);
            return tracker?.HasCompleted(questID) ?? false;
        }

        /// <summary>
        /// Check if player has an active quest
        /// </summary>
        public static bool HasActiveQuest(PlayerMobile pm, int questID)
        {
            var tracker = VystiaQuestTracker.GetTracker(pm);
            return tracker?.IsActive(questID) ?? false;
        }

        /// <summary>
        /// Start a quest for a player
        /// </summary>
        public static bool StartQuest(PlayerMobile pm, int questID)
        {
            VystiaQuest quest = GetQuest(questID);
            if (quest == null)
            {
                pm.SendMessage("Quest not found.");
                Console.WriteLine($"[VystiaQuestSystem] StartQuest failed: Quest {questID} not found in registry.");
                return false;
            }

            // For ephemeral quests, skip completion check (they're meant to be unique per generation)
            bool isEphemeral = quest is DynamicQuest dq && dq.IsEphemeral;

            // Check class requirement
            var playerClass = VystiaClassManager.Instance?.GetClassType(pm) ?? pm.VystiaClassV2;
            if (quest.RequiredClass != PlayerClassTypeV2.None && playerClass != quest.RequiredClass)
            {
                pm.SendMessage($"This quest requires the {quest.RequiredClass} class.");
                Console.WriteLine($"[VystiaQuestSystem] StartQuest failed: Class requirement. Player: {playerClass}, Required: {quest.RequiredClass}");
                return false;
            }

            // Check religion requirement
            if (quest.Religion != Server.Custom.VystiaClasses.Religion.VystiaReligion.None)
            {
                var playerReligion = Server.Custom.VystiaClasses.Religion.VystiaPiety.GetReligion(pm);
                if (playerReligion != quest.Religion)
                {
                    pm.SendMessage($"This quest is only available to followers of {quest.Religion}.");
                    Console.WriteLine($"[VystiaQuestSystem] StartQuest failed: Religion requirement. Player: {playerReligion}, Required: {quest.Religion}");
                    return false;
                }
            }

            // Check faction requirement (optional - can be used to filter quests by faction reputation)
            // Note: This is a soft filter - players can still see the quest but may not get full rewards
            // For hard blocking, add reputation tier checks here if needed

            // Check if already completed (skip for ephemeral quests)
            if (!isEphemeral && HasCompletedQuest(pm, questID))
            {
                pm.SendMessage("You have already completed this quest.");
                Console.WriteLine($"[VystiaQuestSystem] StartQuest failed: Quest {questID} already completed.");
                return false;
            }

            // Check if already active
            if (HasActiveQuest(pm, questID))
            {
                pm.SendMessage("You are already on this quest.");
                Console.WriteLine($"[VystiaQuestSystem] StartQuest failed: Quest {questID} already active.");
                return false;
            }

            // Check prerequisites
            if (quest.PrerequisiteQuestID > 0 && !HasCompletedQuest(pm, quest.PrerequisiteQuestID))
            {
                pm.SendMessage("You must complete the previous quest first.");
                Console.WriteLine($"[VystiaQuestSystem] StartQuest failed: Prerequisite quest {quest.PrerequisiteQuestID} not completed.");
                return false;
            }

            // Start the quest
            var tracker = VystiaQuestTracker.GetOrCreateTracker(pm);
            tracker.StartQuest(questID);

            pm.SendMessage($"Quest started: {quest.Title}");
            pm.SendMessage(quest.Description);

            // Visual effect
            Effects.SendLocationParticles(pm, 0x376A, 9, 32, 5008);
            pm.PlaySound(0x1F7);

            return true;
        }

        /// <summary>
        /// Complete a quest for a player
        /// </summary>
        public static bool CompleteQuest(PlayerMobile pm, int questID)
        {
            VystiaQuest quest = GetQuest(questID);
            if (quest == null)
                return false;

            var tracker = VystiaQuestTracker.GetTracker(pm);
            if (tracker == null || !tracker.IsActive(questID))
                return false;

            // Check if objectives are met
            var progress = tracker.GetProgress(questID);
            if (!quest.AreObjectivesComplete(progress))
            {
                pm.SendMessage("Quest objectives not yet complete.");
                return false;
            }

            // Mark as complete
            tracker.CompleteQuest(questID);

            // Give rewards
            quest.GiveRewards(pm);

            // Award piety if quest has piety reward
            if (quest.PietyReward > 0)
            {
                Server.Custom.VystiaClasses.Religion.VystiaPiety.AddPiety(pm, quest.PietyReward, $"quest: {quest.Title}");
            }

            // Award reputation if quest has faction and reputation reward
            if (quest.Faction != Server.Custom.VystiaClasses.Factions.VystiaFaction.None && quest.ReputationTier > 0)
            {
                Server.Custom.VystiaClasses.Factions.ReputationRewards.AwardQuestReputation(pm, quest.Faction, quest.ReputationTier);
            }
            else if (quest.Faction != Server.Custom.VystiaClasses.Factions.VystiaFaction.None && quest.ReputationReward > 0)
            {
                // Direct reputation reward (if specified)
                Server.Custom.VystiaClasses.Factions.VystiaReputation.AddReputation(pm, quest.Faction, quest.ReputationReward, $"quest: {quest.Title}");
            }

            pm.SendMessage($"Quest completed: {quest.Title}");

            // Visual effects
            Effects.SendLocationParticles(pm, 0x373A, 10, 15, 5018);
            pm.PlaySound(0x1F2);

            return true;
        }

        /// <summary>
        /// Update quest progress for a player
        /// </summary>
        public static void UpdateProgress(PlayerMobile pm, string objectiveKey, int amount = 1)
        {
            var tracker = VystiaQuestTracker.GetTracker(pm);
            if (tracker == null)
                return;

            foreach (var activeQuestID in tracker.GetActiveQuests())
            {
                var quest = GetQuest(activeQuestID);
                if (quest == null)
                    continue;

                bool isWaypointKey = objectiveKey.StartsWith("waypoint_", StringComparison.OrdinalIgnoreCase);

                bool hasObjective = quest.HasObjective(objectiveKey);
                if (!hasObjective && isWaypointKey && quest is DynamicQuest dynQuest)
                {
                    // Only treat it as valid if THIS dynamic quest actually owns the waypoint key.
                    hasObjective = dynQuest.Waypoints.Any(w =>
                        w != null &&
                        (objectiveKey.Equals(w.GetObjectiveKeyForProgress(), StringComparison.OrdinalIgnoreCase)
                         || objectiveKey.Equals(w.GetLegacyObjectiveKeyForProgress(), StringComparison.OrdinalIgnoreCase)));
                }

                if (!hasObjective)
                    continue;

                tracker.UpdateProgress(activeQuestID, objectiveKey, amount);

                var progress = tracker.GetProgress(activeQuestID);
                int current = progress.GetProgress(objectiveKey);
                int required = quest.GetObjectiveRequirement(objectiveKey);

                // For waypoint keys, required is always 1
                if (isWaypointKey)
                    required = 1;

                pm.SendMessage($"Quest Progress: {objectiveKey} ({current}/{required})");

                // Check if quest is now completeable
                if (AreAllObjectivesComplete(quest, progress))
                {
                    pm.SendMessage($"Quest '{quest.Title}' objectives complete! Return to the quest giver.");
                }
            }
        }

        public static void UpdateProgressForQuest(PlayerMobile pm, int questID, string objectiveKey, int amount = 1)
        {
            var tracker = VystiaQuestTracker.GetTracker(pm);
            if (tracker == null || !tracker.IsActive(questID))
                return;

            var quest = GetQuest(questID);
            if (quest == null)
                return;

            bool isWaypointKey = objectiveKey.StartsWith("waypoint_", StringComparison.OrdinalIgnoreCase);

            bool hasObjective = quest.HasObjective(objectiveKey);
            if (!hasObjective && isWaypointKey && quest is DynamicQuest dynQuest)
            {
                hasObjective = dynQuest.Waypoints.Any(w =>
                    w != null &&
                    (objectiveKey.Equals(w.GetObjectiveKeyForProgress(), StringComparison.OrdinalIgnoreCase)
                     || objectiveKey.Equals(w.GetLegacyObjectiveKeyForProgress(), StringComparison.OrdinalIgnoreCase)));
            }

            if (!hasObjective)
                return;

            tracker.UpdateProgress(questID, objectiveKey, amount);
        }

        /// <summary>
        /// Check if all objectives (including waypoints for DynamicQuests) are complete
        /// </summary>
        public static bool AreAllObjectivesComplete(VystiaQuest quest, QuestProgress progress)
        {
            // Check standard objectives
            if (!quest.AreObjectivesComplete(progress))
                return false;

            // For DynamicQuests, also check waypoints
            if (quest is DynamicQuest dynQuest)
            {
                foreach (var waypoint in dynQuest.Waypoints)
                {
                    if (!dynQuest.IsWaypointComplete(waypoint, progress))
                        return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Get the current waypoint for a player's active quest
        /// </summary>
        public static QuestWaypoint GetCurrentWaypoint(PlayerMobile pm, int questID)
        {
            var quest = DynamicQuestManager.GetQuest(questID);
            if (quest == null)
                return null;

            var tracker = VystiaQuestTracker.GetTracker(pm);
            if (tracker == null || !tracker.IsActive(questID))
                return null;

            var progress = tracker.GetProgress(questID);
            return quest.GetCurrentWaypoint(progress);
        }

        /// <summary>
        /// Check if a specific waypoint is complete for a player
        /// </summary>
        public static bool IsWaypointComplete(PlayerMobile pm, int questID, int waypointID)
        {
            var quest = DynamicQuestManager.GetQuest(questID);
            if (quest == null)
                return false;

            var waypoint = quest.GetWaypoint(waypointID);
            if (waypoint == null)
                return false;

            var tracker = VystiaQuestTracker.GetTracker(pm);
            if (tracker == null)
                return false;

            var progress = tracker.GetProgress(questID);
            return quest.IsWaypointComplete(waypoint, progress);
        }

        /// <summary>
        /// Complete a specific waypoint for a player
        /// </summary>
        public static void CompleteWaypoint(PlayerMobile pm, int questID, int waypointID)
        {
            var quest = DynamicQuestManager.GetQuest(questID);
            if (quest == null)
                return;

            var waypoint = quest.GetWaypoint(waypointID);
            if (waypoint == null)
                return;

            string key = waypoint.GetObjectiveKeyForProgress();
            UpdateProgressForQuest(pm, questID, key, 1);

            // Back-compat: also satisfy legacy key if it differs, so older in-progress trackers don't get stuck.
            string legacyKey = waypoint.GetLegacyObjectiveKeyForProgress();
            if (!key.Equals(legacyKey, StringComparison.OrdinalIgnoreCase))
            {
                UpdateProgressForQuest(pm, questID, legacyKey, 1);
            }
        }

        /// <summary>
        /// Hook point for the AISidekicks system: mark a RecruitSidekick waypoint complete when the player recruits a sidekick.
        /// If the waypoint's TargetTypeName is set, it must match the recruited archetype name (case-insensitive).
        /// </summary>
        public static void OnSidekickRecruited(PlayerMobile pm, string archetypeName)
        {
            if (pm == null)
                return;

            var tracker = VystiaQuestTracker.GetTracker(pm);
            if (tracker == null)
                return;

            string recruited = (archetypeName ?? "").Trim();

            foreach (var questID in tracker.GetActiveQuests())
            {
                var quest = DynamicQuestManager.GetQuest(questID);
                if (quest == null)
                    continue;

                var progress = tracker.GetProgress(questID);
                var current = quest.GetCurrentWaypoint(progress);
                if (current == null)
                    continue;

                if (current.Condition != WaypointCondition.RecruitSidekick)
                    continue;

                string required = (current.TargetTypeName ?? "").Trim();

                // If no required archetype specified, any recruited sidekick completes the step.
                bool matches = string.IsNullOrEmpty(required)
                    || required.Equals(recruited, StringComparison.OrdinalIgnoreCase)
                    || (!string.IsNullOrEmpty(recruited) && required.IndexOf(recruited, StringComparison.OrdinalIgnoreCase) >= 0);

                if (!matches)
                    continue;

                CompleteWaypoint(pm, questID, current.WaypointID);
                pm.SendMessage($"Sidekick recruited: {recruited}. Waypoint complete: {current.Name}");
            }
        }
    }

    /// <summary>
    /// Base class for Vystia quests
    /// </summary>
    public abstract class VystiaQuest
    {
        public int QuestID { get; set; }
        public abstract string Title { get; }
        public abstract string Description { get; }
        public abstract PlayerClassTypeV2 RequiredClass { get; }
        public virtual int PrerequisiteQuestID { get { return 0; } }
        public abstract QuestTier Tier { get; }
        
        // Religion integration
        public virtual Server.Custom.VystiaClasses.Religion.VystiaReligion Religion { get { return Server.Custom.VystiaClasses.Religion.VystiaReligion.None; } }
        public virtual int PietyReward { get { return 0; } }
        
        // Faction integration
        public virtual Server.Custom.VystiaClasses.Factions.VystiaFaction Faction { get { return Server.Custom.VystiaClasses.Factions.VystiaFaction.None; } }
        public virtual int ReputationReward { get { return 0; } }
        public virtual int ReputationTier { get { return 0; } } // Quest tier (1-4) for reputation calculation

        // Objectives dictionary: key -> required amount
        protected Dictionary<string, int> Objectives = new Dictionary<string, int>();

        /// <summary>
        /// Get the objectives dictionary for this quest
        /// </summary>
        public Dictionary<string, int> GetObjectives()
        {
            return Objectives;
        }

        /// <summary>
        /// Get the objectives dictionary as a read-only dictionary
        /// </summary>
        public IReadOnlyDictionary<string, int> ObjectivesReadOnly => Objectives;

        public bool HasObjective(string key)
        {
            return Objectives.ContainsKey(key);
        }

        public int GetObjectiveRequirement(string key)
        {
            if (Objectives.TryGetValue(key, out int amount))
                return amount;
            return 0;
        }

        public bool AreObjectivesComplete(QuestProgress progress)
        {
            foreach (var obj in Objectives)
            {
                if (progress.GetProgress(obj.Key) < obj.Value)
                    return false;
            }
            return true;
        }

        public abstract void GiveRewards(PlayerMobile pm);
    }

    /// <summary>
    /// Quest tier enumeration
    /// </summary>
    public enum QuestTier
    {
        Initiation,   // Tier 1: Level 1-30
        Apprentice,   // Tier 2: Level 30-60
        Journeyman,   // Tier 3: Level 60-90
        Master        // Tier 4: Level 90+
    }

    /// <summary>
    /// Tracks progress on a single quest
    /// </summary>
    public class QuestProgress
    {
        private Dictionary<string, int> m_Progress = new Dictionary<string, int>();

        public int GetProgress(string key)
        {
            if (m_Progress.TryGetValue(key, out int value))
                return value;
            return 0;
        }

        public void SetProgress(string key, int value)
        {
            m_Progress[key] = value;
        }

        public void AddProgress(string key, int amount)
        {
            if (!m_Progress.ContainsKey(key))
                m_Progress[key] = 0;
            m_Progress[key] += amount;
        }

        /// <summary>
        /// Convert progress to dictionary for serialization
        /// </summary>
        public Dictionary<string, int> ToDictionary()
        {
            return new Dictionary<string, int>(m_Progress);
        }

        public void Serialize(GenericWriter writer)
        {
            writer.Write(m_Progress.Count);
            foreach (var kvp in m_Progress)
            {
                writer.Write(kvp.Key);
                writer.Write(kvp.Value);
            }
        }

        public void Deserialize(GenericReader reader)
        {
            int count = reader.ReadInt();
            for (int i = 0; i < count; i++)
            {
                string key = reader.ReadString();
                int value = reader.ReadInt();
                m_Progress[key] = value;
            }
        }
    }

    /// <summary>
    /// Player attachment for tracking quest progress
    /// </summary>
    public class VystiaQuestTracker : XmlAttachment
    {
        public const string AttachmentName = "VystiaQuestTracker";

        private HashSet<int> m_CompletedQuests = new HashSet<int>();
        private Dictionary<int, QuestProgress> m_ActiveQuests = new Dictionary<int, QuestProgress>();
        private Dictionary<int, DateTime> m_DailyQuestCompletions = new Dictionary<int, DateTime>(); // questID -> last completion time

        public static VystiaQuestTracker GetTracker(PlayerMobile pm)
        {
            if (pm == null)
                return null;

            return XmlAttach.FindAttachment(pm, typeof(VystiaQuestTracker), AttachmentName) as VystiaQuestTracker;
        }

        public static VystiaQuestTracker GetOrCreateTracker(PlayerMobile pm)
        {
            if (pm == null)
                return null;

            var tracker = GetTracker(pm);
            if (tracker != null)
                return tracker;

            tracker = new VystiaQuestTracker();
            tracker.Name = AttachmentName;
            XmlAttach.AttachTo(pm, tracker);
            return tracker;
        }

        public VystiaQuestTracker()
        {
            Name = AttachmentName;
        }

        // needed for deserialization
        public VystiaQuestTracker(ASerial serial)
            : base(serial)
        {
        }

        public bool HasCompleted(int questID)
        {
            return m_CompletedQuests.Contains(questID);
        }

        public bool IsActive(int questID)
        {
            return m_ActiveQuests.ContainsKey(questID);
        }

        public List<int> GetActiveQuests()
        {
            return new List<int>(m_ActiveQuests.Keys);
        }

        public QuestProgress GetProgress(int questID)
        {
            if (m_ActiveQuests.TryGetValue(questID, out QuestProgress progress))
                return progress;
            return new QuestProgress();
        }

        public void StartQuest(int questID)
        {
            if (!m_ActiveQuests.ContainsKey(questID))
                m_ActiveQuests[questID] = new QuestProgress();
        }

        public void CompleteQuest(int questID)
        {
            m_ActiveQuests.Remove(questID);
            m_CompletedQuests.Add(questID);
        }

        public void UpdateProgress(int questID, string objectiveKey, int amount)
        {
            if (m_ActiveQuests.TryGetValue(questID, out QuestProgress progress))
            {
                progress.AddProgress(objectiveKey, amount);
            }
        }

        /// <summary>
        /// Remove a quest from active quests (abandon/delete)
        /// </summary>
        public void RemoveActiveQuest(int questID)
        {
            m_ActiveQuests.Remove(questID);
        }

        /// <summary>
        /// Remove a quest from completed quests
        /// </summary>
        public void RemoveCompletedQuest(int questID)
        {
            m_CompletedQuests.Remove(questID);
        }

        /// <summary>
        /// Remove a quest from both active and completed tracking
        /// </summary>
        public void RemoveQuest(int questID)
        {
            m_ActiveQuests.Remove(questID);
            m_CompletedQuests.Remove(questID);
        }

        /// <summary>
        /// Clear all active and completed quests for this player
        /// </summary>
        public void ClearAllQuests()
        {
            m_ActiveQuests.Clear();
            m_CompletedQuests.Clear();
        }

        /// <summary>
        /// Get the last completion time for a daily quest
        /// </summary>
        public DateTime GetDailyQuestLastCompletion(int questID)
        {
            if (m_DailyQuestCompletions.TryGetValue(questID, out DateTime lastCompletion))
                return lastCompletion;
            return DateTime.MinValue;
        }

        /// <summary>
        /// Set the last completion time for a daily quest
        /// </summary>
        public void SetDailyQuestLastCompletion(int questID, DateTime completionTime)
        {
            m_DailyQuestCompletions[questID] = completionTime;
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(1); // version (bumped to 1 for daily quest tracking)

            writer.Write(m_CompletedQuests.Count);
            foreach (int id in m_CompletedQuests)
                writer.Write(id);

            writer.Write(m_ActiveQuests.Count);
            foreach (var kvp in m_ActiveQuests)
            {
                writer.Write(kvp.Key);
                (kvp.Value ?? new QuestProgress()).Serialize(writer);
            }

            // Version 1: Daily quest completions
            writer.Write(m_DailyQuestCompletions.Count);
            foreach (var kvp in m_DailyQuestCompletions)
            {
                writer.Write(kvp.Key);
                writer.Write(kvp.Value);
            }
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch (version)
            {
                case 1:
                    {
                        m_CompletedQuests = new HashSet<int>();
                        m_ActiveQuests = new Dictionary<int, QuestProgress>();
                        m_DailyQuestCompletions = new Dictionary<int, DateTime>();

                        int completed = reader.ReadInt();
                        for (int i = 0; i < completed; i++)
                            m_CompletedQuests.Add(reader.ReadInt());

                        int active = reader.ReadInt();
                        for (int i = 0; i < active; i++)
                        {
                            int questID = reader.ReadInt();
                            var progress = new QuestProgress();
                            progress.Deserialize(reader);
                            m_ActiveQuests[questID] = progress;
                        }

                        // Version 1: Daily quest completions
                        int dailyCount = reader.ReadInt();
                        for (int i = 0; i < dailyCount; i++)
                        {
                            int questID = reader.ReadInt();
                            DateTime completionTime = reader.ReadDateTime();
                            m_DailyQuestCompletions[questID] = completionTime;
                        }

                        Name = AttachmentName;
                        break;
                    }
                case 0:
                    {
                        m_CompletedQuests = new HashSet<int>();
                        m_ActiveQuests = new Dictionary<int, QuestProgress>();
                        m_DailyQuestCompletions = new Dictionary<int, DateTime>();

                        int completed = reader.ReadInt();
                        for (int i = 0; i < completed; i++)
                            m_CompletedQuests.Add(reader.ReadInt());

                        int active = reader.ReadInt();
                        for (int i = 0; i < active; i++)
                        {
                            int questID = reader.ReadInt();
                            var progress = new QuestProgress();
                            progress.Deserialize(reader);
                            m_ActiveQuests[questID] = progress;
                        }

                        // Version 0 didn't have daily quest tracking
                        Name = AttachmentName;
                        break;
                    }
            }
        }
    }
}
