using System;
using System.Collections.Generic;
using System.Linq;
using Server;
using Server.Mobiles;

namespace Server.Services.LLM
{
    /// <summary>
    /// Static location database for known NPC locations by town
    /// Provides fast lookups for NPC referrals without scanning regions
    /// </summary>
    public static class NPCLocationDatabase
    {
        /// <summary>
        /// Information about a known NPC location
        /// </summary>
        public struct LocationInfo
        {
            public Point3D Location;
            public string NPCName;
            public DateTime LastVerified;
            public int Serial; // NPC serial for tracking

            public LocationInfo(Point3D location, string npcName, int serial)
            {
                Location = location;
                NPCName = npcName;
                Serial = serial;
                LastVerified = DateTime.UtcNow;
            }
        }

        /// <summary>
        /// Static database: Town Name -> Role -> List of Locations
        /// </summary>
        private static Dictionary<string, Dictionary<NPCKnowledgeSystem.NPCRole, List<LocationInfo>>> s_LocationDatabase =
            new Dictionary<string, Dictionary<NPCKnowledgeSystem.NPCRole, List<LocationInfo>>>();

        private static readonly object s_DatabaseLock = new object();
        private static bool s_Initialized = false;

        /// <summary>
        /// Initialize the location database by scanning all NPCs
        /// </summary>
        public static void Initialize()
        {
            if (s_Initialized)
                return;

            lock (s_DatabaseLock)
            {
                if (s_Initialized)
                    return;

                // Console.WriteLine("[NPCLocationDatabase] Initializing location database...");
                AutoPopulateDatabase();
                s_Initialized = true;
                // Console.WriteLine($"[NPCLocationDatabase] Database initialized with {s_LocationDatabase.Count} towns");
            }
        }

        /// <summary>
        /// Auto-populate database by scanning all NPCs in the world
        /// </summary>
        private static void AutoPopulateDatabase()
        {
            int npcCount = 0;
            int locationCount = 0;

            // Scan all maps
            foreach (Map map in Map.AllMaps)
            {
                if (map == null || map == Map.Internal)
                    continue;

                // Get all mobiles on this map
                var mobiles = World.Mobiles.Values.Where(m => 
                    m != null && 
                    m.Map == map && 
                    m.Alive && 
                    !m.Player);

                foreach (Mobile m in mobiles)
                {
                    // Check if it's a conversational NPC
                    if (m is ILLMConversational conversational && conversational.LLMConversationEnabled)
                    {
                        npcCount++;
                        NPCKnowledgeSystem.NPCRole role = NPCKnowledgeSystem.InferRoleFromPersonality(conversational.PersonalityType);
                        string townName = NPCPersonalities.GetLocationName(m);
                        
                        if (!string.IsNullOrEmpty(townName))
                        {
                            AddLocation(townName, role, m.Location, m.Name, m.Serial);
                            locationCount++;
                        }
                    }
                    // Also check BaseVendor types
                    else if (m is BaseVendor vendor)
                    {
                        npcCount++;
                        NPCKnowledgeSystem.NPCRole role = InferRoleFromVendor(vendor);
                        if (role != NPCKnowledgeSystem.NPCRole.Commoner) // Only track specific roles
                        {
                            string townName = NPCPersonalities.GetLocationName(m);
                            if (!string.IsNullOrEmpty(townName))
                            {
                                AddLocation(townName, role, m.Location, m.Name, m.Serial);
                                locationCount++;
                            }
                        }
                    }
                }
            }

            // Console.WriteLine($"[NPCLocationDatabase] Scanned {npcCount} NPCs, added {locationCount} locations");
        }

        /// <summary>
        /// Infer role from BaseVendor type or name
        /// </summary>
        private static NPCKnowledgeSystem.NPCRole InferRoleFromVendor(BaseVendor vendor)
        {
            string vendorTypeName = vendor.GetType().Name.ToLower();
            string vendorName = vendor.Name?.ToLower() ?? "";
            string vendorTitle = vendor.Title?.ToLower() ?? "";

            // DEBUG: Log what we're checking
            Console.WriteLine($"[NPCLocationDatabase] InferRoleFromVendor called:");
            Console.WriteLine($"  TypeName: '{vendorTypeName}'");
            Console.WriteLine($"  Name: '{vendorName}'");
            Console.WriteLine($"  Title: '{vendorTitle}'");

            // Vystia faction leaders - check first for specific roles
            if (vendorTypeName.Contains("emperor") || vendorName.Contains("emperor") || vendorTitle.Contains("emperor"))
            {
                Console.WriteLine($"  -> Detected as Emperor");
                return NPCKnowledgeSystem.NPCRole.Emperor;
            }

            if (vendorTypeName.Contains("chieftain") || vendorName.Contains("chieftain") || vendorTitle.Contains("chieftain"))
                return NPCKnowledgeSystem.NPCRole.Chieftain;

            if (vendorTypeName.Contains("elder") || (vendorName.Contains("elder") && (vendorTitle.Contains("leader") || vendorTitle.Contains("council"))))
                return NPCKnowledgeSystem.NPCRole.Elder;

            if (vendorTypeName.Contains("sultan") || vendorName.Contains("sultan") || vendorTitle.Contains("sultan"))
                return NPCKnowledgeSystem.NPCRole.Sultan;

            // Archmage separate from regular mages - must have "arch" prefix
            if (vendorTypeName.Contains("archmage") || vendorName.Contains("archmage") || vendorTitle.Contains("archmage"))
                return NPCKnowledgeSystem.NPCRole.Archmage;

            // General faction leader check (for any other leaders)
            if (vendorTitle.Contains("leader") || vendorTitle.Contains("lord") || vendorTitle.Contains("lady"))
                return NPCKnowledgeSystem.NPCRole.FactionLeader;

            // Standard roles
            if (vendorTypeName.Contains("healer") || vendorTypeName.Contains("herbalist") ||
                vendorName.Contains("healer") || vendorName.Contains("herbalist"))
                return NPCKnowledgeSystem.NPCRole.Healer;

            if (vendorTypeName.Contains("mage") || vendorTypeName.Contains("wizard") ||
                vendorName.Contains("mage") || vendorName.Contains("wizard"))
                return NPCKnowledgeSystem.NPCRole.Mage;

            if (vendorTypeName.Contains("blacksmith") || vendorTypeName.Contains("smith") ||
                vendorName.Contains("blacksmith") || vendorName.Contains("smith"))
                return NPCKnowledgeSystem.NPCRole.Blacksmith;

            if (vendorTypeName.Contains("scholar") || vendorTypeName.Contains("sage") ||
                vendorName.Contains("scholar") || vendorName.Contains("sage") || vendorName.Contains("librarian"))
                return NPCKnowledgeSystem.NPCRole.Scholar;

            if (vendorTypeName.Contains("guard") || vendorName.Contains("guard"))
                return NPCKnowledgeSystem.NPCRole.Guard;

            Console.WriteLine($"  -> Defaulting to Merchant (no matches)");
            return NPCKnowledgeSystem.NPCRole.Merchant;
        }

        /// <summary>
        /// Add a location to the database
        /// </summary>
        public static void AddLocation(string townName, NPCKnowledgeSystem.NPCRole role, Point3D location, string npcName, int serial)
        {
            lock (s_DatabaseLock)
            {
                if (!s_LocationDatabase.ContainsKey(townName))
                {
                    s_LocationDatabase[townName] = new Dictionary<NPCKnowledgeSystem.NPCRole, List<LocationInfo>>();
                }

                if (!s_LocationDatabase[townName].ContainsKey(role))
                {
                    s_LocationDatabase[townName][role] = new List<LocationInfo>();
                }

                // Check if this NPC already exists (by serial) and update it
                var existing = s_LocationDatabase[townName][role].FirstOrDefault(l => l.Serial == serial);
                if (existing.Serial != 0 || existing.Serial == serial)
                {
                    s_LocationDatabase[townName][role].Remove(existing);
                }

                s_LocationDatabase[townName][role].Add(new LocationInfo(location, npcName, serial));
            }
        }

        /// <summary>
        /// Get known locations for a role in a town
        /// </summary>
        public static List<LocationInfo> GetLocations(string townName, NPCKnowledgeSystem.NPCRole role)
        {
            lock (s_DatabaseLock)
            {
                if (!s_LocationDatabase.ContainsKey(townName))
                    return new List<LocationInfo>();

                if (!s_LocationDatabase[townName].ContainsKey(role))
                    return new List<LocationInfo>();

                return new List<LocationInfo>(s_LocationDatabase[townName][role]);
            }
        }

        /// <summary>
        /// Find the nearest location for a role in a town from a given point
        /// </summary>
        public static LocationInfo? FindNearestLocation(string townName, NPCKnowledgeSystem.NPCRole role, Point3D fromLocation)
        {
            var locations = GetLocations(townName, role);
            if (locations.Count == 0)
                return null;

            LocationInfo nearest = locations[0];
            int nearestDistance = int.MaxValue;

            foreach (var loc in locations)
            {
                int dx = fromLocation.X - loc.Location.X;
                int dy = fromLocation.Y - loc.Location.Y;
                int distance = (dx * dx) + (dy * dy);

                if (distance < nearestDistance)
                {
                    nearestDistance = distance;
                    nearest = loc;
                }
            }

            return nearest;
        }

        /// <summary>
        /// Update location for an NPC (called when NPC moves)
        /// </summary>
        public static void UpdateLocation(int serial, Point3D newLocation)
        {
            lock (s_DatabaseLock)
            {
                foreach (var townDict in s_LocationDatabase.Values)
                {
                    foreach (var roleList in townDict.Values)
                    {
                        for (int i = 0; i < roleList.Count; i++)
                        {
                            if (roleList[i].Serial == serial)
                            {
                                var updated = roleList[i];
                                updated.Location = newLocation;
                                updated.LastVerified = DateTime.UtcNow;
                                roleList[i] = updated;
                                return;
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Remove location for an NPC (called when NPC is deleted)
        /// </summary>
        public static void RemoveLocation(int serial)
        {
            lock (s_DatabaseLock)
            {
                foreach (var townDict in s_LocationDatabase.Values)
                {
                    foreach (var roleList in townDict.Values)
                    {
                        roleList.RemoveAll(loc => loc.Serial == serial);
                    }
                }
            }
        }

        /// <summary>
        /// Get statistics about the database
        /// </summary>
        public static string GetStatistics()
        {
            lock (s_DatabaseLock)
            {
                int totalTowns = s_LocationDatabase.Count;
                int totalLocations = 0;
                foreach (var townDict in s_LocationDatabase.Values)
                {
                    foreach (var roleList in townDict.Values)
                    {
                        totalLocations += roleList.Count;
                    }
                }
                return $"Towns: {totalTowns}, Total Locations: {totalLocations}";
            }
        }
    }
}

