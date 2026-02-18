using System;
using Server;
using Server.Mobiles;
using Server.Commands;
using Server.Items;

namespace Server.Mobiles
{
    /// <summary>
    /// Manages automatic spawning of Chronicler NPCs in major cities
    /// Ensures players have access to LLM quest generation
    /// </summary>
    public static class ChroniclerSpawner
    {
        private static bool s_Initialized = false;

        /// <summary>
        /// Initialize Chronicler spawns on server startup
        /// </summary>
        public static void Initialize()
        {
            if (s_Initialized)
                return;

            s_Initialized = true;

            // Register command for manual respawning
            CommandSystem.Register("RespawnChroniclers", AccessLevel.GameMaster, RespawnChroniclers_OnCommand);
            
            // Register command to validate Chronicler spawns
            CommandSystem.Register("ValidateChroniclers", AccessLevel.GameMaster, ValidateChroniclers_OnCommand);

            // Hook into world load event to spawn Chroniclers automatically
            EventSink.WorldLoad += OnWorldLoad;

            Console.WriteLine("[ChroniclerSpawner] Initialized - will spawn Chroniclers on world load");
        }

        /// <summary>
        /// Called when world loads - spawns Chroniclers
        /// </summary>
        private static void OnWorldLoad()
        {
            Console.WriteLine("[ChroniclerSpawner] World loaded - spawning Chronicler NPCs...");
            
            // Wait a moment for world to fully load before spawning
            Timer.DelayCall(TimeSpan.FromSeconds(2.0), SpawnChroniclers);
        }

        /// <summary>
        /// Spawn Chronicler NPCs in major cities
        /// </summary>
        public static void SpawnChroniclers()
        {
            Console.WriteLine("[ChroniclerSpawner] Initializing Chronicler spawns in major cities...");

            int spawned = 0;

            // Britain - Near West Britain Bank (high traffic area)
            if (SpawnChronicler("Britain", new Point3D(1432, 1681, 20), Map.Trammel))
                spawned++;

            // Britain - Felucca side (duplicate for both facets)
            if (SpawnChronicler("Britain", new Point3D(1432, 1681, 20), Map.Felucca))
                spawned++;

            // Moonglow - Near the Moonglow Library (scholarly theme fits Chronicler)
            if (SpawnChronicler("Moonglow", new Point3D(4477, 1172, 0), Map.Trammel))
                spawned++;

            // Moonglow - Felucca side
            if (SpawnChronicler("Moonglow", new Point3D(4477, 1172, 0), Map.Felucca))
                spawned++;

            // Vystia - Central location (if Vystia map is available)
            // Note: This is a placeholder - adjust to actual Vystia coordinates when available
            Map vystiaMap = Map.Parse("Vystia");
            if (vystiaMap != null && SpawnChronicler("Vystia", new Point3D(1000, 1000, 0), vystiaMap))
                spawned++;

            Console.WriteLine($"[ChroniclerSpawner] Spawned {spawned} Chronicler NPCs in major cities");
        }

        /// <summary>
        /// Spawn a single Chronicler at specified location
        /// </summary>
        private static bool SpawnChronicler(string cityName, Point3D location, Map map)
        {
            try
            {
                // Check if map is valid
                if (map == null || map == Map.Internal)
                {
                    Console.WriteLine($"[ChroniclerSpawner] Skipped {cityName} - invalid map");
                    return false;
                }

                // Check if a Chronicler already exists at this location
                foreach (Mobile m in map.GetMobilesInRange(location, 10))
                {
                    if (m is Chronicler)
                    {
                        Console.WriteLine($"[ChroniclerSpawner] Chronicler already exists near {cityName}");
                        return false;
                    }
                }

                // Create and spawn the Chronicler
                Chronicler chronicler = new Chronicler();
                chronicler.MoveToWorld(location, map);

                // Face towards center of spawn area for better player interaction
                chronicler.Direction = Direction.South;

                Console.WriteLine($"[ChroniclerSpawner] Spawned Chronicler in {cityName} at {location}");

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ChroniclerSpawner] Error spawning Chronicler in {cityName}: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Command to validate Chronicler spawns
        /// </summary>
        [Usage("ValidateChroniclers")]
        [Description("Validates Chronicler NPC spawns and reports status")]
        private static void ValidateChroniclers_OnCommand(CommandEventArgs e)
        {
            Mobile from = e.Mobile;

            from.SendMessage(68, "[ChroniclerSpawner] Validating Chronicler spawns...");

            if (ValidateChroniclerSpawns())
            {
                from.SendMessage(68, "[ChroniclerSpawner] Chronicler spawns validated successfully!");
            }
            else
            {
                from.SendMessage(38, "[ChroniclerSpawner] WARNING: Chronicler spawn validation failed!");
                from.SendMessage(38, "[ChroniclerSpawner] Use [RespawnChroniclers to fix spawn issues.");
            }
        }

        /// <summary>
        /// Command to manually respawn all Chroniclers
        /// </summary>
        [Usage("RespawnChroniclers")]
        [Description("Respawns all Chronicler NPCs in major cities")]
        private static void RespawnChroniclers_OnCommand(CommandEventArgs e)
        {
            Mobile from = e.Mobile;

            from.SendMessage(68, "[ChroniclerSpawner] Respawning Chronicler NPCs...");

            // Remove existing Chroniclers
            int removed = 0;
            foreach (Mobile m in World.Mobiles.Values)
            {
                if (m is Chronicler)
                {
                    m.Delete();
                    removed++;
                }
            }

            // Spawn new ones
            SpawnChroniclers();

            from.SendMessage(68, $"[ChroniclerSpawner] Removed {removed} old Chroniclers and spawned new ones");
        }

        /// <summary>
        /// Get list of all Chronicler NPCs in the world
        /// </summary>
        public static Chronicler[] GetChroniclers()
        {
            var chroniclers = new System.Collections.Generic.List<Chronicler>();
            
            foreach (Mobile m in World.Mobiles.Values)
            {
                if (m is Chronicler chronicler && !chronicler.Deleted)
                {
                    chroniclers.Add(chronicler);
                }
            }

            return chroniclers.ToArray();
        }

        /// <summary>
        /// Check if Chroniclers are properly spawned
        /// </summary>
        public static bool ValidateChroniclerSpawns()
        {
            var chroniclers = GetChroniclers();
            
            if (chroniclers.Length == 0)
            {
                Console.WriteLine("[ChroniclerSpawner] WARNING: No Chronicler NPCs found in world!");
                return false;
            }

            Console.WriteLine($"[ChroniclerSpawner] Found {chroniclers.Length} Chronicler NPCs:");
            
            foreach (var chronicler in chroniclers)
            {
                string location = $"{chronicler.Location} on {chronicler.Map?.Name ?? "Unknown"}";
                Console.WriteLine($"  - {chronicler.Name} at {location}");
            }

            return chroniclers.Length >= 2; // At least Britain and Moonglow
        }
    }
}
