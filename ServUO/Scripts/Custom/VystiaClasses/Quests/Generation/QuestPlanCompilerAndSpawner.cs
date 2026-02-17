using System;
using System.Collections.Generic;
using Server;
using Server.Mobiles;
using Server.Services.LLM;
using Server.Custom.VystiaClasses.Quests;
using Server.Items;

namespace Server.Custom.VystiaClasses.Quests.Generation
{
    public sealed class CompiledQuestInstance
    {
        public int QuestId { get; set; }
        public List<int> SpawnedSerials { get; } = new List<int>(); // Mobile serial values
        public DateTime ExpiresAtUtc { get; set; }
    }

    public interface IQuestPlanCompiler
    {
        CompiledQuestInstance CompileAndSpawn(PlayerMobile owner, DynamicQuestPlan plan);
    }

    /// <summary>
    /// Converts a validated plan into a temporary (ephemeral) DynamicQuest + spawns required NPCs/bosses.
    /// For now: spawns QuestNPCs and allowlisted boss creature types.
    /// </summary>
    public sealed class QuestPlanCompilerAndSpawner : IQuestPlanCompiler
    {
        public CompiledQuestInstance CompileAndSpawn(PlayerMobile owner, DynamicQuestPlan plan)
        {
            if (owner == null || owner.Deleted || plan == null)
                return null;

            var quest = new DynamicQuest();
            quest.IsEphemeral = true;
            quest.SetTitle(plan.Title ?? "Untitled Quest");
            quest.SetDescription(plan.Description ?? "");

            // Tier mapping by name
            if (!string.IsNullOrWhiteSpace(plan.Tier) &&
                Enum.TryParse(plan.Tier, true, out QuestTier parsedTier))
            {
                quest.SetTier(parsedTier);
            }

            // Build waypoints in order
            foreach (var wpPlan in plan.Waypoints)
            {
                if (wpPlan == null)
                    continue;

                var wp = new QuestWaypoint();

                if (Enum.TryParse(wpPlan.Type, true, out WaypointType t))
                    wp.Type = t;

                if (Enum.TryParse(wpPlan.Condition, true, out WaypointCondition c))
                    wp.Condition = c;

                wp.Name = wpPlan.Name ?? wp.Type.ToString();
                wp.Description = wpPlan.Description ?? "";
                wp.RequiredAmount = Math.Max(1, wpPlan.RequiredAmount);
                wp.Radius = Math.Max(1, wpPlan.Radius);
                wp.NPCDialogueContext = wpPlan.NpcDialogueContext ?? "";
                wp.PlayerLocationHint = wpPlan.PlayerLocationHint ?? "";

                // Location grounding
                if (VystiaPoiRegistry.TryResolveLocation(wpPlan.PoiId, out var loc, out var map, out var radius))
                {
                    wp.Location = loc;
                    wp.Map = map;
                    if (wp.Condition == WaypointCondition.ReachLocation)
                        wp.Radius = Math.Max(1, radius);
                    Console.WriteLine($"[LLMQuest] Waypoint '{wp.Name}' resolved POI '{wpPlan.PoiId}' to location {loc} on {map?.Name ?? "Unknown"}");
                }
                else
                {
                    Console.WriteLine($"[LLMQuest] Warning: Could not resolve POI '{wpPlan.PoiId}' for waypoint '{wp.Name}'. Waypoint will have no location.");
                    owner.SendMessage(38, $"[LLMQuest] Warning: POI '{wpPlan.PoiId}' not found for waypoint '{wp.Name}'.");
                }

                // Boss target binding
                if (wp.Condition == WaypointCondition.DefeatBoss)
                {
                    // Normalize boss type name: remove spaces and hyphens to match C# class names
                    // e.g., "Frost Father" -> "FrostFather"
                    string normalized = wpPlan.BossTypeName?.Trim().Replace(" ", "").Replace("-", "");
                    wp.TargetTypeName = normalized;
                }

                quest.AddWaypoint(wp);
            }

            // Register the quest (assigns QuestID)
            DynamicQuestManager.RegisterDynamicQuest(quest);

            // Spawn NPCs/bosses for each waypoint as needed
            var instance = new CompiledQuestInstance
            {
                QuestId = quest.QuestID,
                ExpiresAtUtc = DateTime.UtcNow.AddMinutes(Math.Max(10, plan.ExpiresMinutes)),
            };

            // Track spawned NPC locations to ensure minimum spacing (location + map)
            List<Tuple<Point3D, Map>> spawnedNPCLocations = new List<Tuple<Point3D, Map>>();
            const int minNPCDistance = 30; // Minimum distance between NPCs

            for (int i = 0; i < plan.Waypoints.Count; i++)
            {
                var wpPlan = plan.Waypoints[i];
                if (wpPlan == null)
                    continue;

                var wp = quest.GetWaypointByOrder(i);
                if (wp == null)
                    continue;

                // Spawn NPC for talk objectives or for narrative anchors.
                bool shouldSpawnNPC = wp.Condition == WaypointCondition.TalkToNPC || wp.Type == WaypointType.Origin || wp.Type == WaypointType.NPCCompletion;
                
                Console.WriteLine($"[LLMQuest] Processing waypoint[{i}] '{wp.Name}': Condition={wp.Condition}, Type={wp.Type}, ShouldSpawnNPC={shouldSpawnNPC}");
                
                if (shouldSpawnNPC)
                {
                    if (wp.Map == null)
                    {
                        owner.SendMessage(38, $"[LLMQuest] Warning: Waypoint '{wp.Name}' has no map set. POI: {wpPlan.PoiId}");
                        Console.WriteLine($"[LLMQuest] Waypoint '{wp.Name}' (POI: {wpPlan.PoiId}) has no map - skipping NPC spawn.");
                        continue;
                    }

                    if (wp.Location == Point3D.Zero)
                    {
                        owner.SendMessage(38, $"[LLMQuest] Warning: Waypoint '{wp.Name}' has invalid location. POI: {wpPlan.PoiId}");
                        Console.WriteLine($"[LLMQuest] Waypoint '{wp.Name}' (POI: {wpPlan.PoiId}) has invalid location - skipping NPC spawn.");
                        continue;
                    }
                    
                    Console.WriteLine($"[LLMQuest] Attempting to spawn NPC for waypoint '{wp.Name}' at {wp.Location} on {wp.Map?.Name ?? "Unknown"}");

                    QuestNPC npc = null;

                    // Try to spawn from NPC template if specified
                    if (!string.IsNullOrWhiteSpace(wpPlan.NpcTemplateId))
                    {
                        VystiaNpcTemplateRegistry.EnsureLoaded();
                        if (VystiaNpcTemplateRegistry.TryGet(wpPlan.NpcTemplateId, out var template))
                        {
                            // Create QuestNPC with template data (avatar of the template NPC)
                            string npcName = !string.IsNullOrWhiteSpace(template.Name) ? template.Name : wpPlan.Name ?? "Quest Contact";
                            string npcTitle = !string.IsNullOrWhiteSpace(template.Title) ? template.Title : "";
                            npc = new QuestNPC(npcName, npcTitle);

                            // Parse personality and speech from template
                            if (Enum.TryParse(template.DefaultPersonality, true, out NPCPersonalities.PersonalityType pType))
                                npc.PersonalityType = pType;
                            if (Enum.TryParse(template.DefaultSpeechPattern, true, out NPCPersonalities.SpeechPattern sPattern))
                                npc.SpeechPattern = sPattern;
                            
                            // Store template ID so NPC knows if it's a faction leader
                            npc.NpcTemplateId = wpPlan.NpcTemplateId;
                        }
                        else
                        {
                            Console.WriteLine($"[LLMQuest] Warning: NPC template '{wpPlan.NpcTemplateId}' not found in registry. Falling back to generic QuestNPC.");
                        }
                    }

                    // Fallback to generic QuestNPC
                    if (npc == null)
                    {
                        string npcName = !string.IsNullOrWhiteSpace(wpPlan.Name) ? wpPlan.Name : "Quest Contact";
                        npc = new QuestNPC(npcName, "");
                        npc.PersonalityType = NPCPersonalities.PersonalityType.Commoner;
                        npc.SpeechPattern = NPCPersonalities.SpeechPattern.Casual;
                    }

                    try
                    {
                        // Spawn NPC within 40 tile radius of waypoint location, but ensure minimum distance from other NPCs
                        Point3D spawnLoc = GetRandomSpawnLocationWithSpacing(wp.Map, wp.Location, 40, spawnedNPCLocations, minNPCDistance);
                        
                        if (spawnLoc == Point3D.Zero)
                        {
                            owner.SendMessage(38, $"[LLMQuest] Failed to find valid spawn location for '{npc.Name}' near {wp.Location}. Trying fallback location...");
                            Console.WriteLine($"[LLMQuest] WARNING: GetRandomSpawnLocationWithSpacing failed for waypoint '{wp.Name}' at {wp.Location}. Attempting fallback.");
                            
                            // Fallback: try spawning directly at waypoint location (or nearby if that fails)
                            Point3D fallbackLoc = GetGroundPoint(wp.Map, wp.Location.X, wp.Location.Y);
                            if (fallbackLoc == Point3D.Zero || !wp.Map.CanSpawnMobile(fallbackLoc))
                            {
                                // Last resort: try a simple random location nearby
                                fallbackLoc = GetRandomSpawnLocation(wp.Map, wp.Location, 20);
                                if (fallbackLoc == Point3D.Zero)
                                {
                                    owner.SendMessage(38, $"[LLMQuest] CRITICAL: Could not find ANY valid spawn location for '{npc.Name}'. Skipping spawn.");
                                    Console.WriteLine($"[LLMQuest] CRITICAL: All spawn location attempts failed for waypoint '{wp.Name}' at {wp.Location} on {wp.Map?.Name ?? "Unknown"}.");
                                    npc.Delete();
                                    continue;
                                }
                            }
                            spawnLoc = fallbackLoc;
                        }
                        
                        npc.MoveToWorld(spawnLoc, wp.Map);
                        npc.LinkToWaypoint(quest.QuestID, wp.WaypointID);
                        instance.SpawnedSerials.Add(npc.Serial.Value);
                        spawnedNPCLocations.Add(new Tuple<Point3D, Map>(spawnLoc, wp.Map)); // Track this NPC's location and map

                        owner.SendMessage(68, $"[LLMQuest] Spawned '{npc.Name}' at {spawnLoc} ({wp.Map?.Name ?? "Unknown"}).");
                        Console.WriteLine($"[LLMQuest] Successfully spawned NPC '{npc.Name}' (Serial: {npc.Serial.Value}) at {spawnLoc} on {wp.Map?.Name ?? "Unknown"} for waypoint '{wp.Name}'.");
                    }
                    catch (Exception ex)
                    {
                        owner.SendMessage(38, $"[LLMQuest] Failed to spawn NPC '{npc.Name}': {ex.Message}");
                        Console.WriteLine($"[LLMQuest] Exception spawning NPC '{npc.Name}' at {wp.Location} ({wp.Map?.Name ?? "Unknown"}): {ex.Message}\n{ex.StackTrace}");
                        npc.Delete(); // Clean up failed spawn
                    }
                }

                if (wp.Condition == WaypointCondition.DefeatBoss)
                {
                    SpawnBoss(owner, wp.TargetTypeName, wp.Location, wp.Map, instance);
                }

                // Spawn items for CollectItems waypoints
                if (wp.Condition == WaypointCondition.CollectItems)
                {
                    SpawnCollectItems(owner, wp, quest.QuestID, instance);
                }
            }

            return instance;
        }

        private static void SpawnCollectItems(PlayerMobile owner, QuestWaypoint wp, int questId, CompiledQuestInstance instance)
        {
            if (owner == null || wp == null || wp.Map == null || wp.Location == Point3D.Zero)
                return;

            // For v1, spawn generic quest items (can be enhanced later with specific item types from plan)
            int itemCount = Math.Max(1, wp.RequiredAmount);
            
            for (int i = 0; i < itemCount; i++)
            {
                // Spawn within 40 tile radius
                Point3D spawnLoc = GetRandomSpawnLocation(wp.Map, wp.Location, 40);
                
                // Create a generic quest item (use a gem as base item - itemID 0x0F15)
                Item questItem = new Item(0x0F15); // Gem item
                questItem.Name = wp.Name ?? "Quest Item";
                questItem.Hue = 0x48E; // Gold color to make it stand out
                questItem.QuestItem = true; // Mark as quest item
                
                questItem.MoveToWorld(spawnLoc, wp.Map);
                
                // Track spawned items for cleanup
                if (instance != null)
                {
                    instance.SpawnedSerials.Add(questItem.Serial.Value);
                }
                
                owner.SendMessage(68, $"[LLMQuest] Spawned quest item '{questItem.Name}' at {spawnLoc}.");
                Console.WriteLine($"[LLMQuest] Spawned quest item '{questItem.Name}' (Serial: {questItem.Serial.Value}) at {spawnLoc} for waypoint '{wp.Name}'.");
            }
        }

        private static void SpawnBoss(Mobile owner, string typeName, Point3D loc, Map map, CompiledQuestInstance instance)
        {
            if (owner == null || map == null || string.IsNullOrWhiteSpace(typeName))
                return;

            try
            {
                Type t = ScriptCompiler.FindTypeByName(typeName);
                if (t == null)
                {
                    owner.SendMessage(38, $"[LLMQuest] Could not find creature type: {typeName}");
                    return;
                }

                object inst = Activator.CreateInstance(t);
                if (!(inst is BaseCreature bc))
                {
                    owner.SendMessage(38, $"[LLMQuest] Type {typeName} is not a creature.");
                    return;
                }

                // Spawn boss within 40 tile radius of waypoint location
                Point3D spawnLoc = GetRandomSpawnLocation(map, loc, 40);
                
                bc.MoveToWorld(spawnLoc, map);
                bc.Home = spawnLoc;
                bc.RangeHome = 10;
                instance?.SpawnedSerials.Add(bc.Serial.Value);

                owner.SendMessage(68, $"[LLMQuest] Spawned boss '{bc.Name}' at {spawnLoc} ({map}).");
                Console.WriteLine($"[LLMQuest] Successfully spawned boss '{bc.Name}' (Serial: {bc.Serial.Value}) at {spawnLoc} on {map?.Name ?? "Unknown"}.");
            }
            catch (Exception ex)
            {
                owner.SendMessage(38, $"[LLMQuest] Failed to spawn {typeName}: {ex.Message}");
                Console.WriteLine($"[LLMQuest] Exception spawning boss '{typeName}' at {loc} ({map?.Name ?? "Unknown"}): {ex.Message}\n{ex.StackTrace}");
            }
        }

        /// <summary>
        /// Gets a random spawn location within radius tiles of the center point, ensuring minimum distance from existing spawns.
        /// </summary>
        private static Point3D GetRandomSpawnLocationWithSpacing(Map map, Point3D center, int radius, List<Tuple<Point3D, Map>> existingLocations, int minDistance)
        {
            if (map == null)
                return Point3D.Zero;

            // Try up to 50 times to find a valid spawn location with proper spacing
            for (int attempts = 0; attempts < 50; attempts++)
            {
                int angle = Utility.Random(360);
                int distance = Utility.RandomMinMax(5, radius); // At least 5 tiles away, max radius
                
                double radians = angle * Math.PI / 180.0;
                int x = center.X + (int)(distance * Math.Cos(radians));
                int y = center.Y + (int)(distance * Math.Sin(radians));
                
                Point3D groundLoc = GetGroundPoint(map, x, y);
                
                // Check if location is valid for spawning
                if (!map.CanSpawnMobile(groundLoc))
                    continue;
                
                // Check minimum distance from existing NPCs on the same map
                bool tooClose = false;
                foreach (var existingLoc in existingLocations)
                {
                    if (existingLoc.Item2 == map) // Same map
                    {
                        int dist = (int)Math.Sqrt(Math.Pow(groundLoc.X - existingLoc.Item1.X, 2) + Math.Pow(groundLoc.Y - existingLoc.Item1.Y, 2));
                        if (dist < minDistance)
                        {
                            tooClose = true;
                            break;
                        }
                    }
                }
                
                if (!tooClose)
                {
                    return groundLoc;
                }
            }
            
            // Fallback: try without spacing requirement if we can't find a good spot
            return GetRandomSpawnLocation(map, center, radius);
        }

        /// <summary>
        /// Gets a random spawn location within 40 tiles of the center point.
        /// </summary>
        private static Point3D GetRandomSpawnLocation(Map map, Point3D center, int radius = 40)
        {
            if (map == null)
                return Point3D.Zero;

            // Try up to 20 times to find a valid spawn location
            for (int attempts = 0; attempts < 20; attempts++)
            {
                int angle = Utility.Random(360);
                int distance = Utility.RandomMinMax(5, radius); // At least 5 tiles away, max radius
                
                double radians = angle * Math.PI / 180.0;
                int x = center.X + (int)(distance * Math.Cos(radians));
                int y = center.Y + (int)(distance * Math.Sin(radians));
                
                Point3D groundLoc = GetGroundPoint(map, x, y);
                
                // Check if location is valid for spawning
                if (map.CanSpawnMobile(groundLoc))
                {
                    return groundLoc;
                }
            }
            
            // Fallback to center if no valid location found
            return GetGroundPoint(map, center.X, center.Y);
        }

        /// <summary>
        /// Gets a valid ground-level Z coordinate for the given X/Y on the map.
        /// </summary>
        private static Point3D GetGroundPoint(Map map, int x, int y)
        {
            if (map == null)
                return Point3D.Zero;

            int z = map.GetAverageZ(x, y);
            return new Point3D(x, y, z);
        }
    }
}


