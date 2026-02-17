using System;
using System.Linq;
using Server;
using Server.Commands;
using Server.Mobiles;
using Server.Custom.VystiaClasses.Quests;
using Server.Services.LLM;

namespace Server.Custom.VystiaClasses.Quests.Generation.Commands
{
    /// <summary>
    /// GM command to find or respawn quest NPCs
    /// Usage: [FindQuestNPC [respawn]
    /// </summary>
    public class FindQuestNPCCommand
    {
        public static void Initialize()
        {
            CommandSystem.Register("FindQuestNPC", AccessLevel.GameMaster, OnCommand);
            CommandSystem.Register("FQNPC", AccessLevel.GameMaster, OnCommand);
        }

        [Usage("FindQuestNPC [respawn]")]
        [Aliases("FQNPC")]
        [Description("Finds quest NPCs for your active quests. Use 'respawn' to respawn missing NPCs.")]
        private static void OnCommand(CommandEventArgs e)
        {
            Mobile from = e.Mobile;
            if (!(from is PlayerMobile pm))
                return;

            bool respawn = e.Arguments.Length > 0 && e.Arguments[0].ToLower() == "respawn";

            var tracker = VystiaQuestTracker.GetTracker(pm);
            if (tracker == null)
            {
                pm.SendMessage("You have no active quests.");
                return;
            }

            var activeQuests = tracker.GetActiveQuests();
            if (activeQuests == null || activeQuests.Count == 0)
            {
                pm.SendMessage("You have no active quests.");
                return;
            }

            pm.SendMessage($"=== Quest NPCs for {activeQuests.Count} active quest(s) ===");

            foreach (int questID in activeQuests)
            {
                var quest = DynamicQuestManager.GetQuest(questID);
                if (quest == null)
                {
                    pm.SendMessage($"Quest {questID}: Not found in registry.");
                    continue;
                }

                pm.SendMessage($"\nQuest: {quest.Title} (ID: {questID})");

                var progress = tracker.GetProgress(questID);
                var currentWaypoint = quest.GetCurrentWaypoint(progress);

                if (currentWaypoint != null)
                {
                    pm.SendMessage($"Current waypoint: {currentWaypoint.Name}");

                    // Check if waypoint has an assigned NPC
                    if (currentWaypoint.AssignedNPCSerial != Serial.MinusOne && currentWaypoint.AssignedNPCSerial.Value > 0)
                    {
                        var npc = World.FindMobile(currentWaypoint.AssignedNPCSerial);
                        if (npc != null && !npc.Deleted)
                        {
                            int dist = (int)from.GetDistanceToSqrt(npc);
                            pm.SendMessage($"  NPC '{npc.Name}' found at {npc.Location} ({npc.Map?.Name ?? "Unknown"}), distance: {dist} tiles");
                            
                            if (dist > 50)
                            {
                                pm.SendMessage($"  Use [Go to teleport to NPC");
                            }
                        }
                        else
                        {
                            pm.SendMessage($"  NPC missing (serial: {currentWaypoint.AssignedNPCSerial.Value})");
                            
                            if (respawn && currentWaypoint.Map != null)
                            {
                                // Respawn the NPC
                                var newNpc = new QuestNPC(
                                    currentWaypoint.Name ?? "Quest Contact",
                                    "");
                                
                                // Try to get personality from waypoint
                                if (!string.IsNullOrEmpty(currentWaypoint.LLMPersonality))
                                {
                                    if (Enum.TryParse(currentWaypoint.LLMPersonality, true, out NPCPersonalities.PersonalityType pType))
                                        newNpc.PersonalityType = pType;
                                }
                                
                                if (!string.IsNullOrEmpty(currentWaypoint.LLMSpeechPattern))
                                {
                                    if (Enum.TryParse(currentWaypoint.LLMSpeechPattern, true, out NPCPersonalities.SpeechPattern sPattern))
                                        newNpc.SpeechPattern = sPattern;
                                }

                                newNpc.MoveToWorld(currentWaypoint.Location, currentWaypoint.Map);
                                newNpc.LinkToWaypoint(questID, currentWaypoint.WaypointID);
                                
                                pm.SendMessage($"  Respawned '{newNpc.Name}' at {currentWaypoint.Location} ({currentWaypoint.Map?.Name ?? "Unknown"})");
                            }
                        }
                    }
                    else
                    {
                        pm.SendMessage($"  No NPC assigned to this waypoint");
                        
                        if (respawn && currentWaypoint.Condition == WaypointCondition.TalkToNPC && currentWaypoint.Map != null)
                        {
                            // Spawn NPC for this waypoint
                            var newNpc = new QuestNPC(
                                currentWaypoint.Name ?? "Quest Contact",
                                "");
                            
                            newNpc.MoveToWorld(currentWaypoint.Location, currentWaypoint.Map);
                            newNpc.LinkToWaypoint(questID, currentWaypoint.WaypointID);
                            
                            pm.SendMessage($"  Spawned '{newNpc.Name}' at {currentWaypoint.Location} ({currentWaypoint.Map?.Name ?? "Unknown"})");
                        }
                    }
                }

                // Check all waypoints for this quest
                foreach (var wp in quest.Waypoints)
                {
                    if (wp == null || wp.AssignedNPCSerial == Serial.MinusOne || wp.AssignedNPCSerial.Value <= 0)
                        continue;

                    var npc = World.FindMobile(wp.AssignedNPCSerial);
                    if (npc == null || npc.Deleted)
                    {
                        pm.SendMessage($"  Waypoint '{wp.Name}': NPC missing");
                    }
                }
            }
        }
    }
}

