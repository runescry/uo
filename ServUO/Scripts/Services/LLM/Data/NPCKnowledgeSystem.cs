using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Server;

namespace Server.Services.LLM
{
    /// <summary>
    /// Proactive knowledge system for NPCs
    /// Pre-loads relevant lore based on NPC role and location instead of reactive per-query searches
    /// </summary>
    public static class NPCKnowledgeSystem
    {
        /// <summary>
        /// NPC role types that determine what knowledge they should have
        /// </summary>
        public enum NPCRole
        {
            Blacksmith,
            Weaponsmith,
            Armorer,
            Guard,
            Mage,
            Scholar,
            Vendor,
            Innkeeper,
            Merchant,
            Healer,
            Ranger,
            Sailor,
            Miner,
            Farmer,
            QuestGiver,
            Commoner,
            Jeweler,
            // Vystia faction leaders
            FactionLeader,
            Emperor,
            Chieftain,
            Elder,
            Sultan,
            Archmage
        }

        /// <summary>
        /// Get relevant lore for an NPC based on their role and location
        /// This is called ONCE when NPC spawns, not per conversation
        /// NOW WITH DOMAIN-BASED FILTERING: Only loads knowledge NPC should know!
        /// </summary>
        public static List<LoreEntry> GetNPCKnowledge(NPCRole role, string locationName, Point3D location, Map map)
        {
            var knowledge = new List<LoreEntry>();
            var allLore = SimpleLoreSystem.GetAllLore();

            Console.WriteLine($"[NPCKnowledge] Building knowledge base for {role} at {locationName}");
            Console.WriteLine($"[NPCKnowledge] Total lore entries available: {allLore.Count}");

            if (allLore.Count == 0)
            {
                Console.WriteLine($"[NPCKnowledge] WARNING: SimpleLoreSystem has NO lore entries! Knowledge base will be empty.");
            }

            // 1. Add role-specific knowledge (NOW FILTERED BY EXPERTISE)
            var roleKnowledge = GetRoleKnowledge(role, allLore);
            knowledge.AddRange(roleKnowledge);
            Console.WriteLine($"[NPCKnowledge] Role-specific knowledge: {roleKnowledge.Count} entries");

            // 2. Add location-specific knowledge
            var locationKnowledge = GetLocationKnowledge(locationName, location, map, allLore);
            knowledge.AddRange(locationKnowledge);
            Console.WriteLine($"[NPCKnowledge] Location-specific knowledge: {locationKnowledge.Count} entries");

            // 3. Remove duplicates
            knowledge = knowledge.GroupBy(l => l.ID).Select(g => g.First()).ToList();

            // 4. FILTER BY EXPERTISE LEVEL - Only keep Expert and Proficient knowledge
            var filteredKnowledge = knowledge.Where(l =>
            {
                var expertise = l.GetExpertise(role);
                return expertise == KnowledgeExpertise.Expert || expertise == KnowledgeExpertise.Proficient;
            }).ToList();

            Console.WriteLine($"[NPCKnowledge] Loaded {knowledge.Count} total entries, {filteredKnowledge.Count} after expertise filtering");
            Console.WriteLine($"[NPCKnowledge] Expert/Proficient only for {role}");

            return filteredKnowledge;
        }

        /// <summary>
        /// Get lore entries relevant to an NPC's role/profession
        /// ALL NPCs get historical knowledge - everyone knows the history of Sosaria
        /// </summary>
        public static List<LoreEntry> GetRoleKnowledge(NPCRole role, List<LoreEntry> allLore)
        {
            var relevant = new List<LoreEntry>();

            // ALL NPCs get historical knowledge - everyone knows the history of Sosaria/Ultima Online
            // LIMIT to most important entries to keep prompt size manageable and response times fast
            var historicalKnowledge = allLore.Where(l =>
                (l.Category == "History" && l.Importance >= 8) || // Major historical events only
                (l.Category == "NPC" && l.Importance >= 8) || // Very important characters only
                l.Importance >= 9 // Very important lore entries
            )
            .OrderByDescending(l => l.Importance) // Sort by importance
            .Take(25) // Limit to top 25 most important entries (keeps prompt small, response fast)
            .ToList();
            relevant.AddRange(historicalKnowledge);

            switch (role)
            {
                case NPCRole.Blacksmith:
                case NPCRole.Weaponsmith:
                case NPCRole.Armorer:
                    // Crafting, metals, weapons, armor, AND locations (they know where adventurers go)
                    // Historical knowledge already added above
                    var blacksmithKnowledge = allLore.Where(l =>
                        l.Category == "Crafting" ||
                        l.Category == "Location" || // Towns, dungeons - blacksmiths know where their customers go
                        l.Category == "Dungeon" || // Dungeons like Destard
                        l.Tags.Any(t => t.Contains("weapon") || t.Contains("armor") ||
                                       t.Contains("metal") || t.Contains("forge") ||
                                       t.Contains("craft") || t.Contains("smith") ||
                                       t.Contains("town") || t.Contains("city") ||
                                       t.Contains("dungeon") || t.Contains("destard") ||
                                       t.Contains("britain") || t.Contains("trinsic") ||
                                       t.Contains("yew") || t.Contains("minoc") ||
                                       t.Contains("vesper") || t.Contains("moonglow"))
                    ).ToList();
                    relevant.AddRange(blacksmithKnowledge);
                    break;

                case NPCRole.Jeweler:
                    // Gems, jewelry, enchantments, precious items
                    // Historical knowledge already added above
                    var jewelerKnowledge = allLore.Where(l =>
                        l.Category == "Crafting" ||
                        l.Category == "Location" || // Towns where they sell jewelry
                        l.Tags.Any(t => t.Contains("gem") || t.Contains("jewelry") ||
                                       t.Contains("ruby") || t.Contains("sapphire") ||
                                       t.Contains("emerald") || t.Contains("diamond") ||
                                       t.Contains("amethyst") || t.Contains("citrine") ||
                                       t.Contains("tourmaline") || t.Contains("amber") ||
                                       t.Contains("star sapphire") || t.Contains("precious") ||
                                       t.Contains("treasure") || t.Contains("enchant") ||
                                       t.Contains("town") || t.Contains("city"))
                    ).ToList();
                    relevant.AddRange(jewelerKnowledge);
                    break;

                case NPCRole.Guard:
                    // Threats, monsters, law, defense
                    // Historical knowledge already added above
                    var guardKnowledge = allLore.Where(l =>
                        l.Category == "Monster" || l.Category == "Dungeon" ||
                        l.Tags.Any(t => t.Contains("threat") || t.Contains("danger") ||
                                       t.Contains("law") || t.Contains("guard") ||
                                       t.Contains("defense") || t.Contains("monster"))
                    ).ToList();
                    relevant.AddRange(guardKnowledge);
                    break;

                case NPCRole.Mage:
                    // Magic, spells, reagents
                    // Historical knowledge already added above
                    var mageKnowledge = allLore.Where(l =>
                        l.Category == "Magic" || l.Category == "Reagent" ||
                        l.Tags.Any(t => t.Contains("magic") || t.Contains("spell") ||
                                       t.Contains("mage") || t.Contains("arcane") ||
                                       t.Contains("reagent") || t.Contains("wizard"))
                    ).ToList();
                    relevant.AddRange(mageKnowledge);
                    break;

                case NPCRole.Scholar:
                    // Everything! Scholars know a lot (historical knowledge already added above)
                    var scholarKnowledge = allLore.Where(l =>
                        l.Category == "Location" || l.Importance >= 5 // Scholars know even more
                    ).ToList();
                    relevant.AddRange(scholarKnowledge);
                    break;

                case NPCRole.Healer:
                    // Medicine, healing, reagents
                    // Historical knowledge already added above
                    var healerKnowledge = allLore.Where(l =>
                        l.Tags.Any(t => t.Contains("heal") || t.Contains("medicine") ||
                                       t.Contains("cure") || t.Contains("reagent") ||
                                       t.Contains("herb") || t.Contains("poison"))
                    ).ToList();
                    relevant.AddRange(healerKnowledge);
                    break;

                case NPCRole.Ranger:
                    // Wilderness, monsters, locations
                    // Historical knowledge already added above
                    var rangerKnowledge = allLore.Where(l =>
                        l.Category == "Location" || l.Category == "Monster" ||
                        l.Tags.Any(t => t.Contains("forest") || t.Contains("wilderness") ||
                                       t.Contains("animal") || t.Contains("hunt"))
                    ).ToList();
                    relevant.AddRange(rangerKnowledge);
                    break;

                case NPCRole.Sailor:
                    // Seas, islands, ports
                    // Historical knowledge already added above
                    var sailorKnowledge = allLore.Where(l =>
                        l.Tags.Any(t => t.Contains("sea") || t.Contains("ocean") ||
                                       t.Contains("island") || t.Contains("port") ||
                                       t.Contains("ship") || t.Contains("sail"))
                    ).ToList();
                    relevant.AddRange(sailorKnowledge);
                    break;

                case NPCRole.QuestGiver:
                    // Important lore, history, NPCs (historical knowledge already added above)
                    var questGiverKnowledge = allLore.Where(l =>
                        l.Importance >= 6 || l.Category == "Location" // Quest givers know locations too
                    ).ToList();
                    relevant.AddRange(questGiverKnowledge);
                    break;

                case NPCRole.Vendor:
                case NPCRole.Merchant:
                case NPCRole.Innkeeper:
                    // Historical knowledge already added above
                    // They also know about locations (where they do business)
                    var vendorKnowledge = allLore.Where(l =>
                        l.Category == "Location" && l.Importance >= 7
                    ).ToList();
                    relevant.AddRange(vendorKnowledge);
                    break;

                case NPCRole.Commoner:
                default:
                    // Historical knowledge already added above
                    // Commoners also know about important locations
                    var commonerKnowledge = allLore.Where(l =>
                        l.Category == "Location" && l.Importance >= 7
                    ).ToList();
                    relevant.AddRange(commonerKnowledge);
                    break;
            }

            // Remove duplicates (historical knowledge might overlap with role-specific)
            relevant = relevant.GroupBy(l => l.ID).Select(g => g.First()).ToList();
            
            Console.WriteLine($"[NPCKnowledge] Role {role} knowledge: {relevant.Count} entries (including historical knowledge)");
            
            return relevant;
        }

        /// <summary>
        /// Get lore entries about the NPC's current location
        /// </summary>
        private static List<LoreEntry> GetLocationKnowledge(string locationName, Point3D location, Map map, List<LoreEntry> allLore)
        {
            var relevant = new List<LoreEntry>();

            // Extract city name from location string
            string cityName = ExtractCityName(locationName);

            if (!string.IsNullOrEmpty(cityName))
            {
                relevant = allLore.Where(l =>
                    l.Tags.Any(t => t.IndexOf(cityName, StringComparison.OrdinalIgnoreCase) >= 0) ||
                    l.Title.IndexOf(cityName, StringComparison.OrdinalIgnoreCase) >= 0 ||
                    l.Content.IndexOf(cityName, StringComparison.OrdinalIgnoreCase) >= 0
                ).ToList();

                Console.WriteLine($"[NPCKnowledge] Found {relevant.Count} entries for location '{cityName}'");
            }

            // Also add nearby dungeons and important locations (NPCs should know about major places)
            // Add all dungeons and major locations regardless of current location
            var dungeonAndLocationKnowledge = allLore.Where(l =>
                l.Category == "Dungeon" ||
                (l.Category == "Location" && l.Importance >= 8) // Important locations like major cities
            ).ToList();

            relevant.AddRange(dungeonAndLocationKnowledge);
            Console.WriteLine($"[NPCKnowledge] Added {dungeonAndLocationKnowledge.Count} dungeon/location entries");

            // Add important NPCs and characters (everyone should know about major figures like Blackthorn)
            var importantNPCs = allLore.Where(l =>
                (l.Category == "NPC" && l.Importance >= 8) || // Important characters
                (l.Category == "History" && l.Importance >= 9) || // Major historical events
                l.Importance >= 10 || // Critical lore (Dark Lord = Blackthorn, etc.)
                l.Tags.Any(t => t.Contains("blackthorn") || t.Contains("dark lord") ||
                               t.Contains("lord british") || t.Contains("virtue") ||
                               t.Contains("chaos") || t.Contains("order"))
            ).ToList();

            relevant.AddRange(importantNPCs);
            Console.WriteLine($"[NPCKnowledge] Added {importantNPCs.Count} important NPC/character entries");

            return relevant;
        }

        /// <summary>
        /// Extract city name from location string like "the city of Britain"
        /// </summary>
        private static string ExtractCityName(string locationName)
        {
            if (string.IsNullOrEmpty(locationName))
                return null;

            // Common patterns: "the city of Britain", "Britain", "the wilderness of Britannia"
            string[] cityNames = new[]
            {
                "Britain", "Trinsic", "Vesper", "Moonglow", "Yew", "Minoc",
                "Skara Brae", "Jhelom", "Magincia", "Cove", "Buccaneer's Den",
                "Nujel'm", "Serpent's Hold", "Ocllo"
            };

            foreach (string city in cityNames)
            {
                if (locationName.IndexOf(city, StringComparison.OrdinalIgnoreCase) >= 0)
                    return city;
            }

            return null;
        }

        /// <summary>
        /// Format knowledge base into a prompt string
        /// Optimized for speed - limits entries and uses compact format
        /// </summary>
        public static string FormatKnowledgeForPrompt(List<LoreEntry> knowledge)
        {
            if (knowledge == null || knowledge.Count == 0)
                return "";

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("YOUR BACKGROUND KNOWLEDGE:");

            // SMART PRIORITIZATION: Balance profession-specific knowledge with general lore
            // For profession NPCs (blacksmiths, mages, etc.), their craft knowledge is MORE important than history
            // For general NPCs, history/lore is more important

            // Categorize entries
            var craftingEntries = knowledge.Where(l =>
                l.Category == "Crafting" ||
                l.Category == "Magic" ||
                l.Category == "Monster" ||
                l.Tags.Any(t => t.Contains("metal") || t.Contains("ore") || t.Contains("weapon") ||
                               t.Contains("armor") || t.Contains("reagent") || t.Contains("spell") ||
                               t.Contains("gem") || t.Contains("jewelry"))
            ).OrderByDescending(l => l.Importance).ToList();

            var historicalEntries = knowledge.Where(l =>
                l.Category == "History" ||
                l.Category == "NPC" ||
                l.Category == "Location" ||
                l.Category == "Dungeon"
            ).OrderByDescending(l => l.Importance).ToList();

            // Include top profession entries first (if any), then historical context
            var topKnowledge = new List<LoreEntry>();
            topKnowledge.AddRange(craftingEntries.Take(15)); // Profession knowledge (metals, spells, etc.)
            topKnowledge.AddRange(historicalEntries.Take(10)); // General context (history, locations)

            // Remove duplicates and take top 20 total for full immersion
            topKnowledge = topKnowledge.GroupBy(l => l.ID).Select(g => g.First()).Take(20).ToList();

            foreach (var entry in topKnowledge)
            {
                // NO TRUNCATION - full immersion with complete content
                sb.AppendLine($"- {entry.Title}: {entry.Content}");
            }

            sb.AppendLine();
            sb.AppendLine("(Reference this knowledge when relevant, but don't recite it - speak naturally)");

            return sb.ToString();
        }

        /// <summary>
        /// Get referral information for a question category outside NPC's expertise
        /// </summary>
        public static string GetReferralPhrase(NPCRole role, QuestionCategory category)
        {
            var domain = new RoleKnowledgeDomain(role);
            var expertise = domain.GetExpertise(category);

            if (expertise == KnowledgeExpertise.Ignorant)
            {
                return domain.GetReferralPhrase(category);
            }

            // If they have some expertise, no referral needed
            return null;
        }

        /// <summary>
        /// Check if NPC should answer a question or defer based on expertise
        /// </summary>
        public static bool ShouldAnswerQuestion(NPCRole role, QuestionCategory category)
        {
            var domain = new RoleKnowledgeDomain(role);
            return domain.ShouldAnswer(category);
        }

        /// <summary>
        /// Get knowledge expertise level for an NPC role and category
        /// </summary>
        public static KnowledgeExpertise GetExpertiseLevel(NPCRole role, QuestionCategory category)
        {
            var domain = new RoleKnowledgeDomain(role);
            return domain.GetExpertise(category);
        }

        /// <summary>
        /// Infer NPC role from personality type
        /// </summary>
        public static NPCRole InferRoleFromPersonality(NPCPersonalities.PersonalityType personality)
        {
            switch (personality)
            {
                case NPCPersonalities.PersonalityType.Guard:
                    return NPCRole.Guard;
                case NPCPersonalities.PersonalityType.Mage:
                    return NPCRole.Mage;
                case NPCPersonalities.PersonalityType.Merchant:
                    return NPCRole.Merchant;
                case NPCPersonalities.PersonalityType.Sage:
                    return NPCRole.Scholar;
                case NPCPersonalities.PersonalityType.Healer:
                    return NPCRole.Healer;
                case NPCPersonalities.PersonalityType.Warrior:
                    return NPCRole.Guard; // Warriors know about threats
                case NPCPersonalities.PersonalityType.Blacksmith:
                case NPCPersonalities.PersonalityType.Weaponsmith:
                case NPCPersonalities.PersonalityType.Armorer:
                    return NPCRole.Blacksmith;
                case NPCPersonalities.PersonalityType.Tailor:
                case NPCPersonalities.PersonalityType.LeatherWorker:
                case NPCPersonalities.PersonalityType.Jeweler:
                    return NPCRole.Jeweler; // Jewelers need gem knowledge
                case NPCPersonalities.PersonalityType.Carpenter:
                case NPCPersonalities.PersonalityType.Tinker:
                case NPCPersonalities.PersonalityType.Bowyer:
                    return NPCRole.Vendor; // Other crafters are vendors
                case NPCPersonalities.PersonalityType.InnKeeper:
                    return NPCRole.Innkeeper;
                case NPCPersonalities.PersonalityType.Farmer:
                case NPCPersonalities.PersonalityType.Fisherman:
                case NPCPersonalities.PersonalityType.Miner:
                    return NPCRole.Commoner; // Workers have general knowledge
                case NPCPersonalities.PersonalityType.Ranger:
                    return NPCRole.Ranger;
                // Vystia faction leaders
                case NPCPersonalities.PersonalityType.Emperor:
                    return NPCRole.Emperor;
                case NPCPersonalities.PersonalityType.Chieftain:
                    return NPCRole.Chieftain;
                case NPCPersonalities.PersonalityType.Elder:
                    return NPCRole.Elder;
                case NPCPersonalities.PersonalityType.Sultan:
                    return NPCRole.Sultan;
                case NPCPersonalities.PersonalityType.FactionLeader:
                    return NPCRole.FactionLeader;
                case NPCPersonalities.PersonalityType.Commoner:
                default:
                    return NPCRole.Commoner;
            }
        }
    }
}
