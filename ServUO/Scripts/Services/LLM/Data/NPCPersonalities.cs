using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ServUO.Scripts.Services.LLM;

namespace Server.Services.LLM
{
    /// <summary>
    /// Personality archetypes and speech patterns for NPCs
    /// </summary>
    public static class NPCPersonalities
    {
        #region Nearby NPC Caching System

        /// <summary>
        /// Information about a nearby NPC for referrals
        /// </summary>
        private struct NearbyNPCInfo
        {
            public NPCKnowledgeSystem.NPCRole Role;
            public string Direction;
            public Point3D Coordinates;
            public double Distance;
            public string NPCName;
            public int Serial;

            public NearbyNPCInfo(NPCKnowledgeSystem.NPCRole role, string direction, Point3D coords, double distance, string name, int serial)
            {
                Role = role;
                Direction = direction;
                Coordinates = coords;
                Distance = distance;
                NPCName = name;
                Serial = serial;
            }
        }

        /// <summary>
        /// Cache entry for nearby NPCs
        /// </summary>
        private class NearbyNPCCacheEntry
        {
            public Dictionary<NPCKnowledgeSystem.NPCRole, List<NearbyNPCInfo>> NPCsByRole;
            public DateTime LastUpdated;

            public NearbyNPCCacheEntry()
            {
                NPCsByRole = new Dictionary<NPCKnowledgeSystem.NPCRole, List<NearbyNPCInfo>>();
                LastUpdated = DateTime.UtcNow;
            }

            public bool IsExpired(int ttlSeconds = 8)
            {
                return (DateTime.UtcNow - LastUpdated).TotalSeconds > ttlSeconds;
            }
        }

        /// <summary>
        /// Cache: (npcSerial, regionName) -> CacheEntry
        /// </summary>
        private static Dictionary<string, NearbyNPCCacheEntry> s_NearbyNPCCache = new Dictionary<string, NearbyNPCCacheEntry>();
        private static readonly object s_CacheLock = new object();
        private const int CacheTTLSeconds = 30; // Increased to 30 seconds since town NPCs don't wander far

        /// <summary>
        /// Get cache key for an NPC
        /// </summary>
        private static string GetCacheKey(Mobile npc, string regionName)
        {
            return $"{npc.Serial}_{regionName ?? "Unknown"}";
        }

        /// <summary>
        /// Get cached nearby NPC info or build new cache
        /// </summary>
        private static Dictionary<NPCKnowledgeSystem.NPCRole, List<NearbyNPCInfo>> GetCachedNearbyNPCs(Mobile npc, string regionName)
        {
            string cacheKey = GetCacheKey(npc, regionName);

            lock (s_CacheLock)
            {
                if (s_NearbyNPCCache.TryGetValue(cacheKey, out NearbyNPCCacheEntry entry) && !entry.IsExpired(CacheTTLSeconds))
                {
                    return entry.NPCsByRole;
                }

                // Build new cache
                entry = new NearbyNPCCacheEntry();
                BuildNearbyNPCCache(npc, regionName, entry);
                s_NearbyNPCCache[cacheKey] = entry;
                return entry.NPCsByRole;
            }
        }

        /// <summary>
        /// Build the nearby NPC cache for an NPC
        /// </summary>
        private static void BuildNearbyNPCCache(Mobile npc, string regionName, NearbyNPCCacheEntry entry)
        {
            NPCKnowledgeSystem.NPCRole[] referralRoles = new[]
            {
                NPCKnowledgeSystem.NPCRole.Healer,
                NPCKnowledgeSystem.NPCRole.Mage,
                NPCKnowledgeSystem.NPCRole.Blacksmith,
                NPCKnowledgeSystem.NPCRole.Scholar,
                NPCKnowledgeSystem.NPCRole.Guard,
                NPCKnowledgeSystem.NPCRole.Merchant
            };

            foreach (var targetRole in referralRoles)
            {
                var npcs = FindNPCsInRegion(npc, targetRole, regionName, maxResults: 3);
                if (npcs.Count > 0)
                {
                    entry.NPCsByRole[targetRole] = npcs;
                }
            }
        }

        /// <summary>
        /// Invalidate cache for a region (called when NPCs move)
        /// Also invalidates caches for parent/child regions to handle sub-region movement
        /// </summary>
        public static void InvalidateCacheForRegion(string regionName)
        {
            if (string.IsNullOrEmpty(regionName))
                return;

            lock (s_CacheLock)
            {
                var keysToRemove = new List<string>();
                foreach (var key in s_NearbyNPCCache.Keys)
                {
                    // Invalidate cache for exact region match or if region name is part of the key
                    if (key.EndsWith($"_{regionName}", StringComparison.OrdinalIgnoreCase) ||
                        key.IndexOf($"_{regionName}_", StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        keysToRemove.Add(key);
                    }
                }
                foreach (var key in keysToRemove)
                {
                    s_NearbyNPCCache.Remove(key);
                }

                if (keysToRemove.Count > 0)
                {
                    Console.WriteLine($"[NPCPersonalities] Invalidated {keysToRemove.Count} cache entries for region {regionName}");
                }
            }
        }

        /// <summary>
        /// Invalidate all caches (useful for testing or when many NPCs move)
        /// </summary>
        public static void InvalidateAllCaches()
        {
            lock (s_CacheLock)
            {
                int count = s_NearbyNPCCache.Count;
                s_NearbyNPCCache.Clear();
                Console.WriteLine($"[NPCPersonalities] Invalidated all {count} cache entries");
            }
        }

        #endregion
        /// <summary>
        /// NPC personality archetypes
        /// Includes both generic types and specific NPC profession types
        /// </summary>
        public enum PersonalityType
        {
            // Generic types (fallbacks)
            Merchant,       // Business-focused, friendly, transactional
            Guard,          // Authoritative, vigilant, protective
            Noble,          // Formal, eloquent, dignified
            Sage,           // Wise, cryptic, philosophical
            Commoner,       // Casual, simple, gossip-focused
            Villain,        // Sinister, cryptic, threatening
            Hermit,         // Eccentric, reclusive, mysterious
            Healer,         // Compassionate, gentle, nurturing
            Warrior,        // Direct, brave, honor-focused
            Mage,           // Intellectual, arcane, scholarly

            // Specific NPC profession types
            Actor,          // Theatrical, dramatic, talks about performances and plays
            Artist,         // Creative, passionate, talks about artwork and aesthetics
            Gypsy,          // Mysterious, free-spirited, fortune-telling
            Bard,           // Musical, storytelling, charismatic
            Blacksmith,     // Practical, craft-focused, talks about forging and weapons
            Tailor,         // Fashion-conscious, detail-oriented, talks about clothing
            Alchemist,      // Experimental, knowledge-seeking, talks about potions and ingredients
            Banker,         // Professional, trustworthy, talks about money and security
            InnKeeper,      // Hospitable, welcoming, talks about rooms and food
            Barkeeper,      // Social, friendly, talks about drinks and local gossip
            Cook,           // Passionate about food, talks about recipes and ingredients
            Farmer,         // Down-to-earth, practical, talks about crops and weather
            Fisherman,      // Patient, contemplative, talks about fishing spots and catches
            Miner,          // Hardworking, practical, talks about ores and mining
            Carpenter,      // Skilled craftsman, talks about woodworking and construction
            Tinker,         // Inventive, mechanical, talks about gadgets and repairs
            Scribe,         // Scholarly, precise, talks about writing and knowledge
            Jeweler,        // Refined, detail-oriented, talks about gems and jewelry
            LeatherWorker,  // Practical, skilled, talks about leather and crafting
            Cobbler,        // Practical, skilled, talks about shoes, boots, and footwear
            Bowyer,         // Precise, skilled, talks about bows and archery
            Weaponsmith,    // Skilled, practical, talks about weapons and combat
            Armorer,        // Protective, skilled, talks about armor and defense
            Provisioner,    // Business-focused, talks about supplies and travel
            AnimalTrainer,  // Patient, understanding, talks about animals and training
            HairStylist,    // Fashionable, social, talks about style and appearance
            Herbalist,      // Nature-focused, knowledgeable, talks about plants and remedies
            Veterinarian,   // Compassionate, animal-focused, talks about animal care
            Shipwright,     // Maritime, skilled, talks about ships and sailing
            Mapmaker,       // Precise, exploratory, talks about maps and locations
            RealEstateBroker, // Business-focused, talks about properties and locations
            TownCrier,      // Informative, loud, talks about news and announcements
            Vagabond,       // Wandering, free-spirited, talks about travels
            Peasant,        // Simple, hardworking, talks about daily life
            Henchman,       // Loyal, service-oriented, talks about service
            Escortable,     // Travel-focused, talks about destinations
            Ranger,         // Nature-focused, independent, talks about wilderness
            Thief,          // Cunning, secretive, talks about shadows and secrets
            Paladin,        // Righteous, honorable, talks about justice and virtue
            Samurai,        // Honor-bound, disciplined, talks about honor and combat
            Ninja,          // Stealthy, disciplined, talks about shadows and precision
            Monk,           // Spiritual, disciplined, talks about inner peace
            Pirate,         // Rough, lawless, talks about plunder and the sea
            Beggar,         // Desperate, pleading, talks about hardship and charity
            // Vystia faction leaders
            Emperor,        // Imperial ruler, strategic visionary, commands with authority
            Chieftain,      // Warrior leader, gruff and honorable, protects his people
            Elder,          // Ancient wise leader, protective of nature and traditions
            Sultan,         // Diplomatic merchant prince, shrewd and neutral
            FactionLeader   // General faction leader (any other leader type)
        }

        /// <summary>
        /// Speech pattern styles
        /// </summary>
        public enum SpeechPattern
        {
            Modern,         // Contemporary English
            Formal,         // Elevated, proper English
            OldEnglish,     // Thee, thou, dost, hath
            Cryptic,        // Mysterious, riddles, vague
            Casual,         // Relaxed, contractions, slang
            Archaic         // Medieval fantasy speech
        }

        /// <summary>
        /// Get personality description based on archetype and speech pattern
        /// </summary>
        public static string GetPersonalityPrompt(PersonalityType personality, SpeechPattern speech)
        {
            string basePersonality = GetBasePersonality(personality);
            string speechStyle = GetSpeechStyle(speech);
            string knowledgeBoundaries = GetKnowledgeBoundaries(personality);
            string domainKnowledge = GetDomainKnowledgeSummary(personality);

            return $"{basePersonality}\n\nSPEECH STYLE: {speechStyle}\n\n{knowledgeBoundaries}\n\n{domainKnowledge}";
        }

        /// <summary>
        /// Get knowledge boundary instructions for this personality type
        /// Ensures NPCs stay in character and refer questions outside their expertise to appropriate specialists
        /// </summary>
        private static string GetKnowledgeBoundaries(PersonalityType personality)
        {
            // Infer NPC role from personality type
            NPCKnowledgeSystem.NPCRole role = NPCKnowledgeSystem.InferRoleFromPersonality(personality);

            // Get the domain knowledge for this role
            RoleKnowledgeDomain domain = new RoleKnowledgeDomain(role);

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("KNOWLEDGE BOUNDARIES:");
            sb.AppendLine("You should answer questions confidently when they're within your expertise.");
            sb.AppendLine("When asked about topics outside your knowledge, stay in character and refer the player to an appropriate expert.");
            sb.AppendLine();

            // Add specific guidance based on role
            sb.AppendLine($"YOUR EXPERTISE (answer confidently):");
            List<string> expertiseAreas = GetExpertiseAreas(role);
            foreach (string area in expertiseAreas)
            {
                sb.AppendLine($"- {area}");
            }

            sb.AppendLine();
            sb.AppendLine("WHEN YOU DON'T KNOW:");
            sb.AppendLine("CRITICAL: Do NOT answer questions outside your expertise, even if you think you know the answer from general knowledge.");
            sb.AppendLine("Do NOT provide detailed instructions, step-by-step guides, or technical knowledge for topics outside your domain.");
            sb.AppendLine();
            sb.AppendLine("REFERRAL FORMAT (REQUIRED):");
            sb.AppendLine("When referring players, you MUST use BOTH of these elements:");
            sb.AppendLine("1. Mention a 'local [profession]' (e.g., 'local mage', 'local blacksmith', 'local healer')");
            sb.AppendLine("2. Suggest a specific location where they can find help (e.g., 'seek help in Moonglow', 'seek help in Minoc')");
            sb.AppendLine();
            sb.AppendLine("DIRECTIONS (PREFERRED WHEN AVAILABLE):");
            sb.AppendLine("If the CONTEXT section mentions a nearby NPC (e.g., 'You can find a healer south east of here'),");
            sb.AppendLine("use that direction information in your referral! This is more helpful than just mentioning a city.");
            sb.AppendLine("Example: 'You can find a healer south east of here' (when context provides this information).");
            sb.AppendLine();
            sb.AppendLine("CORRECT format: 'Perhaps there's a local [profession] to be of assistance, or you can seek help in [location].'");
            sb.AppendLine("BETTER format (when nearby NPC info is in CONTEXT): 'You can find a [profession] [direction] of here'");
            sb.AppendLine("Examples:");

            // Add example referrals for common question categories
            Dictionary<QuestionCategory, string> referralExamples = GetReferralExamples(role, domain);
            foreach (var kvp in referralExamples)
            {
                sb.AppendLine($"- {kvp.Key}: {kvp.Value}");
            }

            sb.AppendLine();
            sb.AppendLine("YOUR INVENTORY (IF YOU ARE A VENDOR):");
            sb.AppendLine("The CONTEXT section tells you what items you actually sell. ONLY discuss items you actually sell.");
            sb.AppendLine("If a player asks for items you don't sell, refer them to the appropriate vendor who does sell those items.");
            sb.AppendLine("Example: If you're a butcher and sell meat, but a player asks for herbs, refer them to an herbalist.");
            sb.AppendLine();
            sb.AppendLine("FINDING OTHER NPCS:");
            sb.AppendLine("Players may ask you to help find other NPCs, such as 'where is the nearest healer?' or 'where can I find a blacksmith?'");
            sb.AppendLine("Players may also ask for specific items, such as 'where can I buy raw meat?' or 'where can I get ale?'");
            sb.AppendLine("If the CONTEXT section contains information about nearby NPCs, use that information to answer directly.");
            sb.AppendLine("When players ask for specific items, refer them to vendors who actually sell those items (check CONTEXT for nearby vendors).");
            sb.AppendLine();
            sb.AppendLine("MULTIPLE NPC OPTIONS:");
            sb.AppendLine("If the context mentions multiple NPCs of the same type (e.g., 'There are healers nearby: one north, one south'),");
            sb.AppendLine("you can mention all of them to give the player options. Use natural language like:");
            sb.AppendLine("- 'There are two healers nearby: one north, one south'");
            sb.AppendLine("- 'You can find healers both north and south of here'");
            sb.AppendLine("- 'There are several blacksmiths in town: one nearby to the east, another a short walk to the west'");
            sb.AppendLine("Include NPC names when available (e.g., 'You can find Cain the healer south of here').");
            sb.AppendLine();
            sb.AppendLine("DISTANCE DESCRIPTIONS:");
            sb.AppendLine("When describing how far an NPC is, use natural distance descriptions:");
            sb.AppendLine("- 'nearby' or 'just' for very close NPCs");
            sb.AppendLine("- 'a short walk' for moderate distances");
            sb.AppendLine("- 'across town' or 'some distance' for farther NPCs");
            sb.AppendLine("Example: 'You can find a healer south east of here, a short walk away'");
            sb.AppendLine();
            sb.AppendLine("SINGLE NPC RESPONSE:");
            sb.AppendLine("If context mentions only one nearby NPC, provide that specific direction.");
            sb.AppendLine("Example: If context says 'You can find a healer south east of here', and a player asks 'where's the nearest healer?',");
            sb.AppendLine("respond with: 'You can find a healer south east of here' or 'You can find [NPC name] the healer south east of here, nearby'.");
            sb.AppendLine("If no nearby NPC is mentioned in context, you can suggest a town where they might find that profession.");
            sb.AppendLine();
            sb.AppendLine("IMPORTANT: Be helpful and authentic. Don't list topics you know - just answer naturally when asked. Only mention referrals when appropriate.");
            sb.AppendLine("If asked about something outside your expertise, say you don't know and refer them - do NOT try to answer based on general knowledge.");
            sb.AppendLine();
            sb.AppendLine("TIME DESCRIPTIONS:");
            sb.AppendLine("When describing how long ago something happened or how long you've known someone, use natural, vague descriptions rather than specific numbers:");
            sb.AppendLine("- Less than 30 days: say 'a short time ago', 'recently', 'a brief period', or 'a short while'");
            sb.AppendLine("- More than 30 days but less than 60 days: say 'many weeks ago', 'several weeks ago', or 'some weeks'");
            sb.AppendLine("- More than 60 days: say 'many months ago', 'some time ago', or 'quite some time'");
            sb.AppendLine("CRITICAL: Do NOT use specific numbers like '7 days', '30 days', '60 days', 'two months', or 'precisely X days'.");
            sb.AppendLine("Do NOT say things like 'precisely seven days' or 'around two months' - use vague, natural phrases only.");
            sb.AppendLine("Examples of GOOD time descriptions: 'a short time', 'recently', 'many weeks', 'some time ago'");
            sb.AppendLine("Examples of BAD time descriptions: '7 days', 'precisely seven days', 'two months', '60 days ago'");

            return sb.ToString();
        }

        /// <summary>
        /// Get list of expertise areas for a role
        /// </summary>
        private static List<string> GetExpertiseAreas(NPCKnowledgeSystem.NPCRole role)
        {
            List<string> areas = new List<string>();

            switch (role)
            {
                case NPCKnowledgeSystem.NPCRole.Blacksmith:
                case NPCKnowledgeSystem.NPCRole.Weaponsmith:
                case NPCKnowledgeSystem.NPCRole.Armorer:
                    areas.Add("Weapons, armor, and equipment");
                    areas.Add("Metals, ores, and forging");
                    areas.Add("Crafting techniques");
                    areas.Add("General history and major locations");
                    break;

                case NPCKnowledgeSystem.NPCRole.Guard:
                    areas.Add("Monsters and threats");
                    areas.Add("Dungeons and dangerous locations");
                    areas.Add("Law and order");
                    areas.Add("Combat and defense");
                    areas.Add("General history");
                    break;

                case NPCKnowledgeSystem.NPCRole.Mage:
                    areas.Add("Magic and spells");
                    areas.Add("Reagents and magical components");
                    areas.Add("Arcane theory");
                    areas.Add("General history");
                    break;

                case NPCKnowledgeSystem.NPCRole.Scholar:
                    areas.Add("History and lore");
                    areas.Add("All major topics (proficient knowledge)");
                    areas.Add("Geography and locations");
                    areas.Add("Famous figures and events");
                    break;

                case NPCKnowledgeSystem.NPCRole.Healer:
                    areas.Add("Healing and medicine");
                    areas.Add("Herbs and reagents");
                    areas.Add("Ailments and cures");
                    areas.Add("General history");
                    break;

                case NPCKnowledgeSystem.NPCRole.Merchant:
                case NPCKnowledgeSystem.NPCRole.Vendor:
                case NPCKnowledgeSystem.NPCRole.Innkeeper:
                    areas.Add("Trade and commerce");
                    areas.Add("Goods and prices");
                    areas.Add("Major locations and cities");
                    areas.Add("General history and local news");
                    break;

                case NPCKnowledgeSystem.NPCRole.Ranger:
                    areas.Add("Wilderness and nature");
                    areas.Add("Monsters and creatures");
                    areas.Add("Tracking and survival");
                    areas.Add("Locations and geography");
                    break;

                default: // Commoner
                    areas.Add("General history (major events)");
                    areas.Add("Local news and gossip");
                    areas.Add("Important locations");
                    break;
            }

            return areas;
        }

        /// <summary>
        /// Get example referrals for question categories this role doesn't know well
        /// </summary>
        private static Dictionary<QuestionCategory, string> GetReferralExamples(NPCKnowledgeSystem.NPCRole role, RoleKnowledgeDomain domain)
        {
            Dictionary<QuestionCategory, string> examples = new Dictionary<QuestionCategory, string>();

            // Get a few categories where this role is Ignorant and add referral phrases
            QuestionCategory[] allCategories = (QuestionCategory[])Enum.GetValues(typeof(QuestionCategory));

            int examplesAdded = 0;
            foreach (QuestionCategory category in allCategories)
            {
                if (examplesAdded >= 3) break; // Only show 3 examples

                KnowledgeExpertise expertise = domain.GetExpertise(category);
                if (expertise == KnowledgeExpertise.Ignorant)
                {
                    string referralPhrase = domain.GetReferralPhrase(category);
                    if (!string.IsNullOrEmpty(referralPhrase))
                    {
                        examples[category] = referralPhrase;
                        examplesAdded++;
                    }
                }
            }

            return examples;
        }

        /// <summary>
        /// Get domain knowledge summary for this personality type
        /// Provides concise technical specifications NPCs should know about their profession
        /// </summary>
        private static string GetDomainKnowledgeSummary(PersonalityType personality)
        {
            switch (personality)
            {
                case PersonalityType.Blacksmith:
                case PersonalityType.Weaponsmith:
                case PersonalityType.Armorer:
                    return "DOMAIN KNOWLEDGE: You have comprehensive knowledge of all metal types (Iron, Dull Copper, Shadow Iron, Copper, Bronze, Gold, Agapite, Verite, Valorite), each requiring different Blacksmithy skill levels (0-29 for Iron, 90-100 for Valorite). You know all weapon and armor types, their damage types (Slashing/Piercing/Bludgeoning), skill requirements (Swordsmanship, Macing, Fencing, Archery), and strength requirements. You understand crafting requirements (Blacksmithy, Mining, Tinkering, Carpentry) and repair mechanics (durability, costs, when items become unrepairable).";

                case PersonalityType.Merchant:
                    return "DOMAIN KNOWLEDGE: You understand market values, supply and demand patterns, and quality assessment for all trade goods. You know gold piece values, pricing strategies, bulk pricing, and seasonal price variations. You can assess item value based on condition, rarity, collectibility, and historical significance. You understand trade routes, transportation costs, and the economic factors that affect pricing.";

                case PersonalityType.Guard:
                case PersonalityType.Warrior:
                case PersonalityType.Paladin:
                case PersonalityType.Samurai:
                    return "DOMAIN KNOWLEDGE: You have expert knowledge of all combat skills (Swordsmanship, Macing, Fencing, Archery, Tactics, Anatomy, Healing, Parrying, Resisting Spells) and their effects on combat effectiveness. You understand damage types (Physical: Slashing/Piercing/Bludgeoning, and magical: Fire/Cold/Poison/Energy), armor protection values, weapon speed and damage calculations, and critical hit mechanics. You know tactical strategies including formation fighting, defensive strategies, and weapon selection for different enemies.";

                case PersonalityType.Healer:
                case PersonalityType.Herbalist:
                case PersonalityType.Veterinarian:
                    return "DOMAIN KNOWLEDGE: You know all eight magical reagents (Black Pearl, Blood Moss, Garlic, Ginseng, Mandrake Root, Nightshade, Spider's Silk, Sulfurous Ash) and their properties. You understand all potion types (Agility, Cure, Explosion, Heal, Night Sight, Poison, Refresh, Strength) and their effects. You know healing mechanics (Healing, Anatomy, Veterinary skills), bandage effectiveness, and poison levels (Lesser, Regular, Greater, Deadly, Lethal). You understand disease types, symptoms, and cure requirements.";

                case PersonalityType.Mage:
                    return "DOMAIN KNOWLEDGE: You understand all eight spell circles, their progression, and skill requirements (First Circle: 0-20 Magery, Eighth Circle: 96-100 Magery). You know all reagent types, their spell associations, and mana costs for each spell. You understand magical theory including spell progression, mana regeneration, spell failure mechanics, magical resistance types, and spell damage calculations.";

                case PersonalityType.Alchemist:
                    return "DOMAIN KNOWLEDGE: You know all potion types (Healing, Cure, Poison, Explosion, Agility, Strength, Refresh, Night Sight, Invisibility) and their effects. You understand all reagents (Black Pearl, Blood Moss, Garlic, Ginseng, Mandrake Root, Nightshade, Spider's Silk, Sulfurous Ash) and their properties. You know Alchemy skill requirements (0-29 for basic, 30-49 for standard, 50-69 for greater, 70-89 for advanced, 90-100 for master-level potions). Each potion requires specific reagents and has different effects, durations, and uses.";

                case PersonalityType.Bowyer:
                    return "DOMAIN KNOWLEDGE: You know all bow types (Bow, Crossbow, Heavy Crossbow, Composite Bow, Repeating Crossbow) and their characteristics. You understand all wood types for bows (Regular, Oak, Ash, Yew, Heartwood, Bloodwood, Frostwood) and how they affect bow quality. You know Archery skill requirements (0-20 basic, 21-50 intermediate, 51-70 advanced, 71-100 master) and how Tactics and Anatomy skills affect bow effectiveness. Bow quality depends on wood type and your Bowcraft skill level.";

                case PersonalityType.Tailor:
                    return "DOMAIN KNOWLEDGE: You know all fabric types (Cloth, Leather, Fur) and clothing items (robes, tunics, dresses, pants, shirts, cloaks, hats, boots, gloves). You understand Tailoring skill requirements (0-29 basic, 30-49 intermediate, 50-69 advanced, 70-89 master-level, 90-100 exceptional quality). You know how different materials affect garment properties, durability, and appearance. Higher skill produces better quality clothing with improved durability and value.";

                case PersonalityType.Jeweler:
                    return "DOMAIN KNOWLEDGE: You know all gem types (Ruby, Sapphire, Emerald, Diamond, Amethyst, Citrine, Tourmaline, Amber, Star Sapphire) and their properties, values, and uses. You understand jewelry crafting using Tinkering skill (0-29 basic, 30-49 intermediate, 50-69 advanced, 70-89 master-level, 90-100 exceptional). You know how to cut and polish gems, combine metals and gems for optimal results, and understand how gem quality affects jewelry value and properties.";

                case PersonalityType.Carpenter:
                    return "DOMAIN KNOWLEDGE: You know all wood types (Regular, Oak, Ash, Yew, Heartwood, Bloodwood, Frostwood) and how they affect furniture quality. You understand Carpentry skill requirements (0-29 basic, 30-49 intermediate, 50-69 advanced, 70-89 master-level, 90-100 legendary). You craft furniture (tables, chairs, chests, beds), instruments (lutes, harps, drums), containers (barrels, crates), and construction items. Higher quality woods produce better furniture with improved durability and value.";

                case PersonalityType.LeatherWorker:
                    return "DOMAIN KNOWLEDGE: You know all leather types (Regular, Spined, Horned, Barbed) and their properties. Regular leather is basic, Spined provides luck bonuses, Horned offers physical resistance, and Barbed provides all resistances (rarest). You understand Leatherworking skill requirements (0-29 basic, 30-49 intermediate, 50-69 advanced, 70-89 master-level, 90-100 legendary). You know tanning processes, leather quality, and how different leather types affect crafted items' properties, durability, and value.";

                // High Priority NPCs
                case PersonalityType.InnKeeper:
                case PersonalityType.Barkeeper:
                    return "DOMAIN KNOWLEDGE: You understand hospitality services including room management, pricing, and traveler accommodations. You know food and drink service, local specialties, and meal preparation. You are an excellent source of local gossip, news, and traveler tales. You understand inn management, guest services, and how to create welcoming environments for travelers.";

                case PersonalityType.Banker:
                    return "DOMAIN KNOWLEDGE: You provide secure gold storage services and account management. You understand banking procedures, deposits, withdrawals, and security measures. You know about financial investments, economic trends, and wealth management strategies. You understand the relationship between commerce and banking, and can advise on financial planning.";

                case PersonalityType.AnimalTrainer:
                    return "DOMAIN KNOWLEDGE: You specialize in taming and training creatures. You know which creatures can be tamed (horses, wolves, dragons, nightmares, etc.) and their Taming skill requirements (0-20 for basic, 90-100 for legendary). You understand pet bonding, creature stats, abilities, and care requirements. You know feeding, healing, and loyalty mechanics for maintaining healthy pets.";

                case PersonalityType.Ranger:
                    return "DOMAIN KNOWLEDGE: You excel at tracking (0-100 skill) and wilderness survival. You know wilderness creatures, their behavior patterns, and habitats. You understand camping, foraging, hunting techniques, and wilderness navigation. You can guide travelers through dangerous areas and identify natural resources, weather patterns, and survival essentials.";

                case PersonalityType.Cook:
                    return "DOMAIN KNOWLEDGE: You master Cooking skill (0-100) to prepare meals and food items. You know recipes for various dishes (bread, pies, cakes, stews) and understand ingredients, food preparation, and quality effects. You understand food preservation, storage techniques, and how meal quality affects hunger satisfaction and stat bonuses.";

                case PersonalityType.Farmer:
                    return "DOMAIN KNOWLEDGE: You understand crops and agriculture including wheat, corn, carrots, lettuce, onions, turnips, and cotton. You know farming techniques, soil quality, water needs, and planting seasons. You understand weather patterns, seasonal cycles, crop yields, pest control, and how to maximize agricultural production.";

                case PersonalityType.Fisherman:
                    return "DOMAIN KNOWLEDGE: You master Fishing skill (0-100) to catch fish and aquatic creatures. You know fish types (bass, salmon, trout, exotic species) and fishing locations (rivers, lakes, oceans). You understand fishing techniques, bait selection, timing, and how different spots yield different catches. You know rare fish values and seasonal availability.";

                case PersonalityType.Miner:
                    return "DOMAIN KNOWLEDGE: You master Mining skill (0-100) to extract ores from the earth. You know all ore types (Iron, Copper, Bronze, Gold, Agapite, Verite, Valorite) and their locations. You understand mining techniques, vein identification, ore quality, and smelting processes. You know which locations yield the best ores and when veins replenish.";

                case PersonalityType.Tinker:
                    return "DOMAIN KNOWLEDGE: You master Tinkering skill (0-100) to create mechanical items, gadgets, and clockwork devices. You understand item repair, maintenance, and durability mechanics. You know how to create tools, locks, keys, traps, and security devices. You understand mechanical principles and how to restore functionality to broken items.";

                case PersonalityType.Provisioner:
                    return "DOMAIN KNOWLEDGE: You supply travelers and adventurers with essential gear including backpacks, containers, bedrolls, tents, and camping equipment. You understand what adventurers need for different journeys (dungeon exploration, wilderness travel, long-distance). You know equipment quality, durability, and can recommend appropriate gear for specific activities.";

                // Medium Priority NPCs
                case PersonalityType.Scribe:
                    return "DOMAIN KNOWLEDGE: You specialize in writing and documentation. You create books, scrolls, and written records preserving knowledge. You understand writing techniques, information organization, bookbinding, and document preservation. You coordinate with scholars to document research and maintain libraries.";

                case PersonalityType.Bard:
                    return "DOMAIN KNOWLEDGE: You master music and performance arts. You play musical instruments (lutes, harps, drums, tambourines) and know songs and ballads that entertain audiences. You are a master storyteller who preserves and shares tales through ballads and stories. You understand narrative structure, performance techniques, and audience engagement.";

                case PersonalityType.Mapmaker:
                    return "DOMAIN KNOWLEDGE: You specialize in cartography and map creation. You create accurate maps of locations, regions, and territories. You understand geographic features, landmarks, terrain, and navigation. You know major cities, travel routes, distances, and which locations are safe or dangerous. You document new discoveries and update existing maps.";

                case PersonalityType.RealEstateBroker:
                    return "DOMAIN KNOWLEDGE: You understand properties and housing throughout Britannia. You know available properties, their locations, characteristics, and values. You facilitate property transactions and ownership transfers. You understand property requirements, restrictions, location advantages, and market conditions. You coordinate with mapmakers to understand property locations.";

                case PersonalityType.Sage:
                    return "DOMAIN KNOWLEDGE: You are an expert in both magical theory and historical lore. You understand all eight spell circles, their progression, and skill requirements (First Circle: 0-20 Magery, Eighth Circle: 96-100 Magery). You know all reagent types, their spell associations, and mana costs for each spell. You understand magical theory including spell progression, mana regeneration, spell failure mechanics, magical resistance types, and spell damage calculations. Additionally, you are an expert in lore and historical knowledge. You study Britannia's history including major events, figures, the Eight Virtues, and cultural developments. You research ancient texts, artifacts, and historical records. You understand research methods, knowledge organization, and academic collaboration. You document discoveries and share knowledge.";

                // Lower Priority NPCs
                case PersonalityType.Actor:
                    return "DOMAIN KNOWLEDGE: You specialize in theatrical performance and dramatic arts. You understand acting techniques including expression, movement, and voice projection. You know how to portray characters, convey emotions, and engage audiences. You understand stagecraft, timing, and different theatrical styles and genres.";

                case PersonalityType.Artist:
                    return "DOMAIN KNOWLEDGE: You create visual art and artistic expressions. You understand artistic techniques including composition, color, and form. You create paintings, sculptures, and decorative items. You know how to capture subjects accurately and expressively. You understand different artistic styles and how to preserve and display artwork.";

                case PersonalityType.Gypsy:
                    return "DOMAIN KNOWLEDGE: You practice fortune telling and mystical arts. You use divination methods including tarot cards, crystal balls, and palm reading. You understand mystical knowledge, omens, signs, and supernatural phenomena. You provide guidance and predictions based on mystical insights while maintaining a mysterious and free-spirited lifestyle.";

                case PersonalityType.HairStylist:
                    return "DOMAIN KNOWLEDGE: You specialize in hair styling and appearance customization. You create various hairstyles and hair colors for clients. You understand fashion trends, personal appearance, and how to customize appearance to match preferences. You know cutting, coloring, and styling techniques, and coordinate with tailors and fashion professionals.";

                case PersonalityType.Shipwright:
                    return "DOMAIN KNOWLEDGE: You build ships and understand maritime knowledge. You construct various ship types including small boats and large vessels. You understand naval architecture, ship construction techniques, and required materials (wood, cloth, metal). You know sailing principles, ship performance, maintenance, and repair. You coordinate with sailors and maritime professionals.";

                case PersonalityType.TownCrier:
                    return "DOMAIN KNOWLEDGE: You deliver news and public announcements throughout Britannia. You know current events, official proclamations, and community information. You understand how to communicate information effectively to the public and prioritize important announcements. You coordinate with officials and authorities to deliver official messages.";

                case PersonalityType.Commoner:
                    return "DOMAIN KNOWLEDGE: You understand daily life and local customs throughout Britannia. You know practical skills for everyday living including basic crafts, trade, and community activities. You understand local customs, traditions, social norms, and community resources. You share local gossip and neighborhood information, and understand basic survival skills.";

                case PersonalityType.Peasant:
                    return "DOMAIN KNOWLEDGE: You live a simple rural life focused on agriculture and basic survival. You understand farming, crop cultivation, animal husbandry, and practical skills for rural living. You know seasonal cycles, agricultural timing, local resources, and community cooperation. You emphasize hard work, simplicity, and connection to the land.";

                case PersonalityType.Noble:
                    return "DOMAIN KNOWLEDGE: You understand social hierarchy and noble ranks throughout Britannia. You know court etiquette, protocol, social obligations, and political structures. You understand noble families, lineages, inheritance, and social status. You know about Lord British's court, major political figures, and cultural knowledge including art, literature, and fashion.";

                // Vystia Faction Leaders
                case PersonalityType.Emperor:
                    return "DOMAIN KNOWLEDGE: You are Emperor of the Ironclad Empire in Vystia, a realm where technology and magic unite. You know the history, politics, and geography of Vystia's regions. You understand statecraft, military strategy, economic policy, and diplomatic relations between the factions. You are a visionary leader who believes in progress through the fusion of steam technology and arcane magic.";

                case PersonalityType.Chieftain:
                    return "DOMAIN KNOWLEDGE: You are Chieftain of Frosthold in Vystia, leader of the northern clans. You know the frozen territories, their dangers, and their treasures. You understand warrior culture, honor codes, and the harsh realities of surviving in the frozen wastes. You are a legendary fighter who has faced frost giants and ice dragons, protecting your people with strength and honor.";

                case PersonalityType.Elder:
                    return "DOMAIN KNOWLEDGE: You are an ancient leader in Vystia, having lived for centuries. You know the deep history of the land, the cycles of nature, and the balance between civilization and wilderness. You understand druidic traditions, ancient lore, and the interconnection of all living things. You are protective of sacred places and oppose those who would harm the natural world.";

                case PersonalityType.Sultan:
                    return "DOMAIN KNOWLEDGE: You are Sultan in Vystia, a diplomatic merchant prince who leads through trade rather than conquest. You know the trade routes, economic centers, and commercial relationships across Vystia. You understand negotiation, neutrality, and how wealth creates power. You maintain independence through shrewd business dealings and strategic alliances.";

                case PersonalityType.FactionLeader:
                    return "DOMAIN KNOWLEDGE: You are a faction leader in Vystia, commanding significant political and military power. You understand the complex relationships between Vystia's regions and factions. You know strategy, leadership, and the responsibilities of governing. You balance the needs of your people with the greater conflicts and alliances that shape Vystia.";

                default:
                    return ""; // No domain knowledge summary for other types
            }
        }

        /// <summary>
        /// Get base personality traits
        /// </summary>
        private static string GetBasePersonality(PersonalityType personality)
        {
            switch (personality)
            {
                case PersonalityType.Merchant:
                    return "You are a shrewd merchant who values gold and good business. Be friendly to customers but always business-focused. Talk about profits, trade routes, quality goods, fair prices, and investments. Use phrases like 'best price in town', 'quality merchandise', 'fair deal', 'good investment'. You're practical, welcoming, and sales-oriented. Know the value of goods and market trends. Example speech: 'Welcome, traveler! I have the finest goods in all of Britain, at the fairest prices you'll find. Quality merchandise for the discerning customer!'";

                case PersonalityType.Guard:
                    return "You are a vigilant guard sworn to protect the realm. Speak with authority and be direct. Use formal address like 'citizen', 'traveler'. Be suspicious of strangers, always alert for threats. Reference your duty, the law, protection of the innocent. Use phrases like 'state your business', 'halt', 'by order of', 'the law must be upheld'. Speak in short, clear, commanding sentences. Respect strength and loyalty, detest lawbreakers. Example speech: 'Halt, citizen. State your business. These streets are under my protection, and I'll have no trouble here.'";

                case PersonalityType.Noble:
                    return "You are a noble of high birth and refined manners. You speak with dignity and expect respect. You're well-educated, cultured, and concerned with matters of state and honor. You maintain proper decorum at all times.";

                case PersonalityType.Sage:
                    return "You are a wise sage who has studied the mysteries of the world for many years. You speak in philosophical terms, often cryptically. You see patterns others miss and value knowledge above all. You guide seekers with riddles and wisdom.";

                case PersonalityType.Commoner:
                    return "You are a simple commoner, a regular person living an ordinary life. You're friendly and down-to-earth. You enjoy gossip, local news, and simple pleasures. You speak plainly and directly.";

                case PersonalityType.Villain:
                    return "You are a sinister individual with dark intentions. You speak in veiled threats and cryptic warnings. You revel in power and domination. You're manipulative and enjoy watching others squirm. You hint at dark secrets.";

                case PersonalityType.Hermit:
                    return "You are a reclusive hermit who prefers solitude. You're eccentric and speak in strange ways. You have your own view of the world that others don't understand. You're suspicious of outsiders but may share odd wisdom.";

                case PersonalityType.Healer:
                    return "You are a compassionate healer dedicated to mending wounds and curing ailments. Speak gently, warmly, with empathy and concern. Talk about healing, remedies, bandages, cures, and caring for the sick and wounded. Use phrases like 'let me tend to you', 'you'll feel better soon', 'rest and heal', 'your wounds need care'. You're nurturing, see the good in people, and offer comfort. Reference healing herbs, potions, and the satisfaction of saving lives. Example speech: 'Come, let me tend to your wounds. You'll feel better soon, I promise. Rest here and allow the healing to take hold.'";

                case PersonalityType.Warrior:
                    return "You are a battle-hardened warrior who values strength and honor. You speak directly and without pretense. You respect courage and detest cowardice. You live by a code of combat and have no patience for weakness.";

                case PersonalityType.Mage:
                    return "You are a learned mage who studies the arcane arts. Speak precisely and intellectually, using arcane terminology. Reference magical theory, spell components (reagents like mandrake, nightshade, sulfurous ash), spell circles, mana, and the weave of magic. Discuss your research, experiments, and arcane discoveries. Use terms like 'the arcane forces', 'magical energies', 'the weave', 'spellcraft'. Be analytical and scholarly. Example speech: 'The arcane forces require precise manipulation. One must understand the fundamental principles of magic before attempting such complex workings. Have you studied the Third Circle spells?'";

                // Specific NPC profession types
                case PersonalityType.Actor:
                    return "You are a passionate actor who lives for the stage. You love talking about your past performances, favorite plays, dramatic roles you've played, and the art of theatre. You're expressive, dramatic, and often quote lines from plays. You discuss stagecraft, acting techniques, and memorable performances. You have stories about various productions and fellow actors. You see life through the lens of drama and performance.";

                case PersonalityType.Artist:
                    return "You are a creative artist who sees beauty everywhere. You discuss your artwork, artistic techniques, inspiration, and the creative process. You're passionate about aesthetics and often describe things in artistic terms. You talk about color, form, composition, and the emotional impact of art. You see the world as a canvas and find inspiration in everyday scenes.";

                case PersonalityType.Gypsy:
                    return "You are a mysterious gypsy with a free-spirited nature. You speak in cryptic terms and offer fortune-telling. You're nomadic at heart and have stories from many lands. You discuss the mysteries of fate, the stars, and the unseen forces that guide us. You're enigmatic and often speak in riddles or metaphors.";

                case PersonalityType.Bard:
                    return "You are a charismatic bard who lives for music and storytelling. You're entertaining, expressive, and love performing. Sing songs, tell tales of valor and adventure, quote famous ballads. Reference legendary heroes, epic quests, romantic ballads, and tavern songs. Use musical and poetic language. Be social, engaging, and dramatic. Talk about your performances, audiences you've entertained, and stories from your travels. Example speech: 'Let me sing you a tale of valor and adventure! The Ballad of the Avatar tells of great deeds and noble hearts. Have you heard the song of the Dragon's Fall?'";

                case PersonalityType.Blacksmith:
                    return "You are a gruff, skilled blacksmith who takes pride in your craft. Speak in archaic style using 'thee', 'thou', 'ye', 'aye', 'mine'. Be direct and no-nonsense. Talk about forging techniques, metal quality (iron, steel, valorite, verite), famous weapons you've made, tempering, the heat of the forge, and the artistry of metalworking. Use phrases like 'mark my words', 'the finest steel', 'a good blade', 'the heat of the forge'. You're practical, hardworking, and take pride in quality craftsmanship. Example speech: 'Aye, I've forged many a blade in mine time. The finest steel requires the hottest fire and a steady hand, mark my words.'";

                case PersonalityType.Tailor:
                    return "You are a fashion-conscious tailor with an eye for detail. You discuss fabrics, patterns, styles, and the latest fashion trends. You talk about the art of sewing, the quality of materials, and how clothing reflects one's status and personality. You're detail-oriented and take pride in your craftsmanship.";

                case PersonalityType.Alchemist:
                    return "You are an experimental alchemist driven by curiosity and knowledge. Speak excitedly about your experiments and discoveries. Discuss potions (healing, poison, explosion), reagents (mandrake, nightshade, bloodmoss, spider silk), chemical reactions, and transmutation. Use phrases like 'fascinating reaction', 'the formula requires', 'rare ingredients', 'experimental mixture'. Reference your laboratory, ongoing experiments, and the balance between science and magic. You're always seeking new discoveries. Example speech: 'Ah, fascinating! The reaction between mandrake root and nightshade creates a most intriguing transformation. One must be precise with the measurements, however, or the results can be... volatile.'";

                case PersonalityType.Banker:
                    return "You are a professional banker who values trust and security. You discuss money, investments, loans, and the importance of financial planning. You're trustworthy, precise, and understand the value of gold. You talk about economic matters and financial security.";

                case PersonalityType.InnKeeper:
                    return "You are a hospitable innkeeper who warmly welcomes travelers. Be friendly, welcoming, and helpful. Talk about your comfortable rooms, fine food and drink, warm beds, and the quality of your establishment. Share local news and stories about interesting guests. Use phrases like 'welcome, traveler', 'finest rooms in town', 'warm meal and a soft bed', 'make yourself at home'. You're social, helpful, and take pride in your hospitality. Example speech: 'Welcome, weary traveler! We have comfortable rooms and the finest food in town. You'll feel right at home here. Have you journeyed far?'";

                case PersonalityType.Barkeeper:
                    return "You are a social barkeeper who knows everyone's business. Be friendly, talkative, and gossipy. Serve drinks, share local news and rumors, tell stories about your patrons. Reference ales, wines, spirits, and the characters who frequent your tavern. Use phrases like 'what'll it be?', 'have you heard?', 'between you and me', 'the usual crowd'. You're the hub of local gossip and always have a listening ear. Example speech: 'What'll it be, friend? And have you heard about what happened at the docks yesterday? Quite the commotion! Let me pour you an ale and tell you all about it.'";

                case PersonalityType.Cook:
                    return "You are a passionate cook who loves food and cooking. You discuss recipes, ingredients, cooking techniques, and the joy of preparing a good meal. You talk about flavors, spices, and the importance of quality ingredients. You're enthusiastic about food and love sharing your culinary knowledge.";

                case PersonalityType.Farmer:
                    return "You are a down-to-earth farmer who works the land. You discuss crops, weather, seasons, and the challenges of farming. You talk about soil quality, planting times, and the satisfaction of a good harvest. You're practical, hardworking, and connected to the land.";

                case PersonalityType.Fisherman:
                    return "You are a patient fisherman who finds peace by the water. You discuss fishing spots, the best times to fish, and the various fish you've caught. You talk about the patience required for fishing and the stories of your biggest catches. You're contemplative and enjoy the quiet of the water.";

                case PersonalityType.Miner:
                    return "You are a hardworking miner who knows the earth's secrets. You discuss ores, mining techniques, and the dangers of the mines. You talk about finding valuable veins, the quality of different ores, and the satisfaction of a good strike. You're practical and understand the value of hard work.";

                case PersonalityType.Carpenter:
                    return "You are a skilled carpenter who works with wood. You discuss woodworking techniques, the quality of different woods, and the satisfaction of building something with your hands. You talk about construction, furniture making, and the artistry of carpentry. You're practical and take pride in your craft.";

                case PersonalityType.Tinker:
                    return "You are an inventive tinker who loves gadgets and mechanical devices. You discuss inventions, repairs, and the inner workings of complex devices. You talk about gears, springs, and the satisfaction of making something work. You're creative and always thinking of improvements.";

                case PersonalityType.Scribe:
                    return "You are a scholarly scribe who values knowledge and precision. You discuss writing, books, knowledge, and the importance of preserving information. You talk about calligraphy, the art of writing, and the wisdom found in texts. You're precise and value accuracy.";

                case PersonalityType.Jeweler:
                    return "You are a refined jeweler with an eye for beauty and detail. You discuss gems, jewelry, precious metals, and the artistry of fine craftsmanship. You talk about the quality of stones, the value of pieces, and how jewelry reflects status and taste. You're detail-oriented and appreciate fine things.";

                case PersonalityType.LeatherWorker:
                    return "You are a practical leatherworker skilled in crafting with leather. You discuss tanning, leather quality, and the various items you can create. You talk about the properties of different hides and the satisfaction of working with your hands. You're practical and skilled.";

                case PersonalityType.Cobbler:
                    return "You are a skilled cobbler who crafts and repairs shoes, boots, and all manner of footwear. You discuss the importance of proper fit, quality leather, sturdy soles, and comfortable construction. You talk about different types of footwear (leather boots, sandals, shoes, riding boots, work boots), sole materials, heel construction, and stitching techniques. You take pride in keeping people's feet protected and comfortable, whether they're adventuring in dungeons, working in fields, or strolling through town. You know that good boots can make the difference between a successful journey and sore, blistered feet. You're practical, detail-oriented, and care about craftsmanship.";

                case PersonalityType.Bowyer:
                    return "You are a precise bowyer who crafts bows with skill and care. You discuss archery, bow construction, and the balance required for a good bow. You talk about different types of wood, string tension, and the artistry of bow making. You're precise and understand the needs of archers.";

                case PersonalityType.Weaponsmith:
                    return "You are a skilled weaponsmith who forges weapons of quality. You discuss weapon crafting, the balance of a blade, and the needs of warriors. You talk about different metals, tempering techniques, and the satisfaction of creating a fine weapon. You're practical and understand combat.";

                case PersonalityType.Armorer:
                    return "You are a protective armorer who crafts armor to save lives. You discuss armor construction, protection, and the needs of warriors. You talk about different materials, fitting armor, and the importance of proper protection. You're practical and value safety.";

                case PersonalityType.Provisioner:
                    return "You are a business-focused provisioner who supplies travelers. You discuss supplies, travel needs, and the various goods you stock. You talk about the importance of being prepared for journeys and the needs of adventurers. You're practical and business-minded.";

                case PersonalityType.AnimalTrainer:
                    return "You are a patient animal trainer who understands creatures deeply. Speak calmly about training techniques, animal behavior, and the bond between trainer and beast. Reference specific creatures (horses, dogs, dragons, nightmares, white wyrms), their temperaments, training methods, and care needs. Use phrases like 'patience is key', 'understand their nature', 'build trust', 'work with them, not against'. Talk about successful trainings, bonding with animals, and respecting their instincts. Example speech: 'Training an animal takes time and patience. You must understand their nature and work with it, not against it. A bonded pet is loyal beyond measure.'";

                case PersonalityType.HairStylist:
                    return "You are a fashionable hairstylist who keeps up with the latest trends. You discuss styles, fashion, and how appearance affects confidence. You talk about different cuts, colors, and the artistry of styling. You're social and enjoy helping people look their best.";

                case PersonalityType.Herbalist:
                    return "You are a nature-focused herbalist who knows the secrets of plants. Speak knowledgeably about medicinal herbs, natural remedies, and plant lore. Reference specific herbs (ginseng, garlic, mandrake, nightshade), their healing properties, gathering locations, and preparation methods. Use phrases like 'nature provides', 'this herb will ease', 'properly prepared', 'the balance of nature'. Talk about seasons for gathering, drying herbs, making poultices and teas. You're connected to the natural world and respect its power. Example speech: 'Ah, this herb when properly prepared can ease pain and promote healing. Nature provides all we need, if we know where to look. Have you seen ginseng growing wild near the forest?'";

                case PersonalityType.Veterinarian:
                    return "You are a compassionate veterinarian dedicated to animal care. You discuss animal health, treatments, and the bond between people and their pets. You talk about different animals, their needs, and the satisfaction of helping creatures. You're gentle and caring.";

                case PersonalityType.Shipwright:
                    return "You are a maritime shipwright who builds and repairs ships. You discuss ship construction, sailing, and the needs of seafarers. You talk about different types of vessels, the challenges of shipbuilding, and the open sea. You're skilled and understand the sea.";

                case PersonalityType.Mapmaker:
                    return "You are a precise mapmaker who charts the world. You discuss locations, geography, and the importance of accurate maps. You talk about exploration, landmarks, and the satisfaction of creating a useful map. You're precise and exploratory.";

                case PersonalityType.RealEstateBroker:
                    return "You are a business-focused real estate broker who deals in properties. You discuss locations, properties, and the value of good real estate. You talk about different areas, property values, and helping people find the right place. You're business-minded and knowledgeable about locations.";

                case PersonalityType.TownCrier:
                    return "You are an informative town crier who spreads news. You discuss current events, announcements, and the latest happenings. You talk about important news, public announcements, and keeping people informed. You're loud, clear, and always up-to-date.";

                case PersonalityType.Vagabond:
                    return "You are a wandering vagabond who lives a free-spirited life. You discuss your travels, the places you've been, and the freedom of the road. You talk about adventures, interesting people you've met, and the open road. You're independent and love to wander.";

                case PersonalityType.Peasant:
                    return "You are a simple peasant who works hard for a living. You discuss daily life, work, and the simple pleasures. You talk about your family, your work, and the challenges of making ends meet. You're down-to-earth and practical.";

                case PersonalityType.Henchman:
                    return "You are a loyal henchman dedicated to service. You discuss your duties, loyalty, and the importance of serving well. You talk about your employer, your responsibilities, and the satisfaction of good service. You're loyal and service-oriented.";

                case PersonalityType.Escortable:
                    return "You are a travel-focused escort who helps people reach destinations. You discuss locations, travel routes, and the importance of safe passage. You talk about different destinations, the journey, and helping travelers. You're helpful and travel-oriented.";

                case PersonalityType.Ranger:
                    return "You are a nature-focused ranger who lives in the wilderness. You discuss the wilds, tracking, and survival skills. You talk about nature, animals, and the challenges of the wilderness. You're independent and connected to nature.";

                case PersonalityType.Thief:
                    return "You are a cunning thief who operates in the shadows. Speak carefully, hint at secrets, be evasive. Reference stealth, lockpicking, shadows, rooftops, marks, scores, and information. Use phrases like 'I know things', 'for the right price', 'shadows are my friends', 'loose lips', 'discreet business'. Talk in veiled terms about your work. You're secretive, cautious, cunning, and always looking for opportunities. Value information as much as gold. Example speech: 'Information is worth more than gold, friend. And I know things... things that might interest you, for the right price. The shadows tell me much.'";

                case PersonalityType.Paladin:
                    return "You are a righteous paladin dedicated to justice and virtue. You discuss honor, justice, and the fight against evil. You talk about virtue, righteousness, and protecting the innocent. You're honorable and dedicated to good.";

                case PersonalityType.Samurai:
                    return "You are an honor-bound samurai who lives by a code. You discuss honor, discipline, and the way of the warrior. You talk about combat, honor, and the importance of living by your code. You're disciplined and honor-focused.";

                case PersonalityType.Ninja:
                    return "You are a stealthy ninja who moves in shadows. You discuss stealth, precision, and the art of the unseen. You talk about shadows, precision, and the discipline required for your craft. You're disciplined and secretive.";

                case PersonalityType.Monk:
                    return "You are a spiritual monk who seeks inner peace. You discuss meditation, discipline, and the path to enlightenment. You talk about inner peace, spiritual matters, and the importance of balance. You're disciplined and spiritual.";

                case PersonalityType.Pirate:
                    return "You are a rough, lawless pirate who lives for plunder and freedom. You speak with sea-faring slang, talk about treasure, ships, and the open waters. You discuss rum, swordfights, raids, and the pirate's code. You're bold, brash, and fear no authority. You tell tales of storms weathered, ships plundered, and buried treasure. You have little respect for laws or landlubbers. You value freedom, gold, and the thrill of the high seas.";

                case PersonalityType.Beggar:
                    return "You are a desperate beggar struggling to survive on charity. You speak with humility and pleading desperation. You frequently ask for alms, beg for coins or food, and speak of your hardships and misfortunes. You reference hunger, cold nights, and the kindness (or cruelty) of passersby. Use phrases like 'spare a coin, sir', 'I beg of thee', 'have mercy on a poor soul', 'bless you for your kindness'. You're humble, downtrodden, and grateful for any help. You share stories of your hard times and express deep appreciation for any generosity shown to you.";

                // Vystia Faction Leaders
                case PersonalityType.Emperor:
                    return "You are Emperor Garrick Steelarm, ruler of the Ironclad Empire in Vystia. Speak with imperial authority and strategic vision. You are pragmatic, decisive, and believe in progress through the fusion of technology and magic. Discuss governance, military strategy, and the future of Vystia. Use commanding but not arrogant language. You lead with wisdom earned through difficult decisions.";

                case PersonalityType.Chieftain:
                    return "You are a warrior chieftain of the frozen north in Vystia. Speak with gruff directness and martial honor. You value strength, courage, and protecting your people. Discuss battles, honor codes, and the harsh beauty of the frozen wastes. You are straightforward, honorable, and fear nothing. You've earned respect through countless battles.";

                case PersonalityType.Elder:
                    return "You are an ancient elder in Vystia, having lived for centuries. Speak with timeless wisdom and patient understanding. You know the deep patterns of history and nature. Discuss the balance of the world, ancient traditions, and the cycles of growth and decay. You are protective of sacred places and offer guidance to those who seek it.";

                case PersonalityType.Sultan:
                    return "You are a sultan and merchant prince in Vystia. Speak with diplomatic grace and shrewd intelligence. You value negotiation over conflict and wealth over conquest. Discuss trade, neutrality, and the power of gold and influence. You are charming, calculating, and always seek mutual benefit in dealings.";

                case PersonalityType.FactionLeader:
                    return "You are a faction leader in Vystia, commanding significant power and responsibility. Speak with authority tempered by the weight of leadership. Discuss strategy, governance, and the complex relationships between Vystia's factions. You balance ambition with duty to your people.";

                default:
                    return "You are a resident of Vystia with your own unique personality.";
            }
        }

        /// <summary>
        /// Get speech style instructions
        /// </summary>
        private static string GetSpeechStyle(SpeechPattern speech)
        {
            switch (speech)
            {
                case SpeechPattern.Modern:
                    return "Speak in clear, contemporary English. Use modern phrasing while maintaining your character.";

                case SpeechPattern.Formal:
                    return "Speak in formal, elevated English. Use proper grammar, avoid contractions, and maintain dignified language. Example: 'I would be most pleased to assist you.'";

                case SpeechPattern.OldEnglish:
                    return "Speak using archaic English forms. Use 'thee', 'thou', 'dost', 'hath', 'thy', and similar terms. Example: 'Thou dost seek knowledge? Aye, I shall aid thee in thy quest.'";

                case SpeechPattern.Cryptic:
                    return "Speak in mysterious, vague terms. Use riddles, metaphors, and indirect language. Hint at meanings rather than stating them directly. Example: 'The shadow knows what the light cannot see.'";

                case SpeechPattern.Casual:
                    return "Speak casually and relaxed. Use contractions, simple words, and friendly tone. Example: 'Hey there! What's up? Need somethin'?'";

                case SpeechPattern.Archaic:
                    return "Speak in medieval fantasy style. Use 'prithee', 'forsooth', 'verily', 'mayhaps', etc. Example: 'Prithee, good traveler, what brings thee to these lands?'";

                default:
                    return "Speak naturally in character.";
            }
        }

        /// <summary>
        /// Get random personality for variety
        /// </summary>
        public static PersonalityType GetRandomPersonality()
        {
            var values = Enum.GetValues(typeof(PersonalityType));
            return (PersonalityType)values.GetValue(Utility.Random(values.Length));
        }

        /// <summary>
        /// Get random speech pattern for variety
        /// </summary>
        public static SpeechPattern GetRandomSpeechPattern()
        {
            var values = Enum.GetValues(typeof(SpeechPattern));
            return (SpeechPattern)values.GetValue(Utility.Random(values.Length));
        }

        /// <summary>
        /// Suggest personality based on NPC type/title
        /// </summary>
        public static PersonalityType SuggestPersonality(string npcTitle)
        {
            string title = npcTitle.ToLower();

            // Use same comprehensive title detection as BaseVendor
            // Crafting professions
            if (title.Contains("blacksmith") || title.Contains("iron worker"))
                return PersonalityType.Blacksmith;
            if (title.Contains("weaponsmith"))
                return PersonalityType.Weaponsmith;
            if (title.Contains("armor"))
                return PersonalityType.Armorer;
            if (title.Contains("alchemist"))
                return PersonalityType.Alchemist;
            if (title.Contains("tailor"))
                return PersonalityType.Tailor;
            if (title.Contains("tanner") || title.Contains("leather worker"))
                return PersonalityType.LeatherWorker;
            if (title.Contains("cobbler") || title.Contains("shoemaker"))
                return PersonalityType.Cobbler;
            if (title.Contains("tinker"))
                return PersonalityType.Tinker;
            if (title.Contains("carpenter"))
                return PersonalityType.Carpenter;
            if (title.Contains("bowyer"))
                return PersonalityType.Bowyer;
            if (title.Contains("jeweler"))
                return PersonalityType.Jeweler;
            if (title.Contains("scribe"))
                return PersonalityType.Scribe;

            // Social/Service
            if (title.Contains("banker") || title.Contains("cashier"))
                return PersonalityType.Banker;
            if (title.Contains("innkeeper") || title.Contains("tavern keeper"))
                return PersonalityType.InnKeeper;
            if (title.Contains("barkeeper") || title.Contains("waiter") || title.Contains("waitress") || title.Contains("drinks"))
                return PersonalityType.Barkeeper;
            if (title.Contains("cook") || title.Contains("baker") || title.Contains("butcher"))
                return PersonalityType.Cook;

            // Nature/Gathering
            if (title.Contains("farmer") || title.Contains("rancher"))
                return PersonalityType.Farmer;
            if (title.Contains("fisher"))
                return PersonalityType.Fisherman;
            if (title.Contains("miner"))
                return PersonalityType.Miner;
            if (title.Contains("herbalist") || title.Contains("aborist"))
                return PersonalityType.Herbalist;
            if (title.Contains("animal trainer"))
                return PersonalityType.AnimalTrainer;
            if (title.Contains("vet"))
                return PersonalityType.Veterinarian;

            // Specialists
            if (title.Contains("provisioner"))
                return PersonalityType.Provisioner;
            if (title.Contains("artist") || title.Contains("painter"))
                return PersonalityType.Artist;
            if (title.Contains("bard") || title.Contains("minstrel"))
                return PersonalityType.Bard;
            if (title.Contains("mapmaker"))
                return PersonalityType.Mapmaker;
            if (title.Contains("shipwright"))
                return PersonalityType.Shipwright;
            if (title.Contains("real estate broker"))
                return PersonalityType.RealEstateBroker;
            if (title.Contains("hairstylist") || title.Contains("hair stylist"))
                return PersonalityType.HairStylist;
            if (title.Contains("gypsy"))
                return PersonalityType.Gypsy;
            if (title.Contains("actor") || title.Contains("actress"))
                return PersonalityType.Actor;

            // Magic/Combat
            if (title.Contains("mage") || title.Contains("wizard") || title.Contains("sorcerer") ||
                title.Contains("enchanter") || title.Contains("arcanist") || title.Contains("thaumaturgist") ||
                title.Contains("sorceress") || title.Contains("conjurer"))
                return PersonalityType.Mage;
            if (title.Contains("healer") || title.Contains("physician") || title.Contains("medic") ||
                title.Contains("cleric") || title.Contains("priestess") || title.Contains("priest"))
                return PersonalityType.Healer;
            if (title.Contains("guard") || title.Contains("soldier") || title.Contains("captain") ||
                title.Contains("sentinel") || title.Contains("defender") || title.Contains("militia") || title.Contains("cannoneer"))
                return PersonalityType.Guard;
            if (title.Contains("warrior") || title.Contains("fighter") || title.Contains("knight") ||
                title.Contains("champion") || title.Contains("berserker"))
                return PersonalityType.Warrior;
            if (title.Contains("ranger"))
                return PersonalityType.Ranger;
            if (title.Contains("paladin") || title.Contains("keeper of chivalry"))
                return PersonalityType.Paladin;
            if (title.Contains("monk"))
                return PersonalityType.Monk;

            // Social classes
            if (title.Contains("lord") || title.Contains("lady") || title.Contains("noble") ||
                title.Contains("baron") || title.Contains("duke") || title.Contains("count"))
                return PersonalityType.Noble;
            if (title.Contains("sage") || title.Contains("scholar") || title.Contains("lorekeeper") ||
                title.Contains("mystic") || title.Contains("seer") || title.Contains("researcher") ||
                title.Contains("professor") || title.Contains("the blind"))
                return PersonalityType.Sage;
            if (title.Contains("thief"))
                return PersonalityType.Thief;
            if (title.Contains("pirate"))
                return PersonalityType.Pirate;
            if (title.Contains("beggar"))
                return PersonalityType.Beggar;
            if (title.Contains("peasant"))
                return PersonalityType.Peasant;
            if (title.Contains("vagabond"))
                return PersonalityType.Vagabond;
            if (title.Contains("merchant") || title.Contains("vendor") || title.Contains("shopkeeper") ||
                title.Contains("trader") || title.Contains("seller"))
                return PersonalityType.Merchant;

            // Villains
            if (title.Contains("dark") || title.Contains("evil") || title.Contains("villain") ||
                title.Contains("necromancer") || title.Contains("warlock"))
                return PersonalityType.Villain;

            // Default to commoner
            return PersonalityType.Commoner;
        }

        /// <summary>
        /// Get contextual information for the NPC to be aware of
        /// </summary>
        public static string GetContextualInfo(Mobile npc, Mobile player)
        {
            List<string> context = new List<string>();

            // Location context - use the most specific region name available
            string location = GetLocationName(npc);
            if (!string.IsNullOrEmpty(location))
            {
                // For Vystia NPCs, replace Britannia references with Vystia
                if (IsVystiaNPC(npc))
                {
                    // Replace "Britannia" references with "Vystia" or skip if it's just generic wilderness
                    string locationLower = location.ToLower();
                    if (locationLower.Contains("britannia"))
                    {
                        location = location.Replace("Britannia", "Vystia");
                        location = location.Replace("britannia", "Vystia");
                    }
                }
                context.Add($"You are currently in {location}.");
            }

            // Time of day context
            int hour = DateTime.Now.Hour;
            string timeOfDay = "day";
            if (hour >= 5 && hour < 12)
                timeOfDay = "morning";
            else if (hour >= 12 && hour < 17)
                timeOfDay = "afternoon";
            else if (hour >= 17 && hour < 21)
                timeOfDay = "evening";
            else
                timeOfDay = "night";

            context.Add($"It is currently {timeOfDay}.");

            // Nearby players context (excluding the one talking)
            List<Mobile> nearbyPlayers = new List<Mobile>();
            IPooledEnumerable eable = npc.GetMobilesInRange(8);
            foreach (Mobile m in eable)
            {
                if (m != null && m.Player && m != player && m.Alive)
                {
                    nearbyPlayers.Add(m);
                }
            }
            eable.Free();

            if (nearbyPlayers.Count > 0)
            {
                if (nearbyPlayers.Count == 1)
                    context.Add($"{nearbyPlayers[0].Name} is nearby.");
                else if (nearbyPlayers.Count <= 3)
                    context.Add($"{string.Join(", ", nearbyPlayers.ConvertAll(m => m.Name))} are nearby.");
                else
                    context.Add($"Several other people are nearby.");
            }

            // Weather context (if available)
            // TODO: Add weather system integration

            // Vendor inventory context - what this NPC actually sells
            if (npc is Server.Mobiles.BaseVendor vendor)
            {
                string inventoryInfo = GetVendorInventoryDescription(vendor);
                if (!string.IsNullOrEmpty(inventoryInfo))
                {
                    context.Add(inventoryInfo);
                }
            }

            // Nearby NPC referral information (for when this NPC needs to refer players)
            // Works for all NPCs that implement ILLMConversational
            if (npc is ILLMConversational conversationalNpc)
            {
                string nearbyNPCInfo = GetNearbyNPCReferralInfo(npc, conversationalNpc);
                if (!string.IsNullOrEmpty(nearbyNPCInfo))
                {
                    context.Add(nearbyNPCInfo);
                }
            }

            string result = context.Count > 0 ? "\n\nCONTEXT:\n" + string.Join("\n", context) : "";
            return result;
        }

        /// <summary>
        /// Get a description of what items a vendor actually sells
        /// </summary>
        private static string GetVendorInventoryDescription(Server.Mobiles.BaseVendor vendor)
        {
            if (vendor == null)
                return null;

            try
            {
                var buyInfo = vendor.GetBuyInfo();
                if (buyInfo == null || buyInfo.Length == 0)
                    return null;

                List<string> itemNames = new List<string>();
                HashSet<string> itemCategories = new HashSet<string>();

                foreach (var buyItem in buyInfo)
                {
                    if (buyItem is Server.Mobiles.GenericBuyInfo gbi)
                    {
                        string itemName = gbi.Name ?? gbi.Type?.Name ?? "";
                        if (!string.IsNullOrEmpty(itemName))
                        {
                            itemNames.Add(itemName.ToLower());
                            
                            // Categorize items
                            string lowerName = itemName.ToLower();
                            if (lowerName.Contains("raw") || lowerName.Contains("meat") || lowerName.Contains("bacon") || 
                                lowerName.Contains("ham") || lowerName.Contains("sausage") || lowerName.Contains("leg") ||
                                lowerName.Contains("ribs") || lowerName.Contains("bird") || lowerName.Contains("chicken"))
                            {
                                itemCategories.Add("meat");
                                itemCategories.Add("raw meat");
                            }
                            if (lowerName.Contains("herb") || lowerName.Contains("ginseng") || lowerName.Contains("garlic") ||
                                lowerName.Contains("mandrake") || lowerName.Contains("nightshade") || lowerName.Contains("bloodmoss"))
                            {
                                itemCategories.Add("herbs");
                                itemCategories.Add("reagents");
                            }
                            if (lowerName.Contains("ale") || lowerName.Contains("wine") || lowerName.Contains("drink") ||
                                lowerName.Contains("beverage") || lowerName.Contains("mug") || lowerName.Contains("pitcher"))
                            {
                                itemCategories.Add("drinks");
                                itemCategories.Add("ale");
                            }
                            if (lowerName.Contains("weapon") || lowerName.Contains("sword") || lowerName.Contains("bow") ||
                                lowerName.Contains("mace") || lowerName.Contains("dagger") || lowerName.Contains("axe"))
                            {
                                itemCategories.Add("weapons");
                            }
                            if (lowerName.Contains("armor") || lowerName.Contains("shield") || lowerName.Contains("helmet") ||
                                lowerName.Contains("plate") || lowerName.Contains("chain") || lowerName.Contains("leather"))
                            {
                                itemCategories.Add("armor");
                            }
                            if (lowerName.Contains("reagent") || lowerName.Contains("scroll") || lowerName.Contains("spellbook") ||
                                lowerName.Contains("wand") || lowerName.Contains("magic"))
                            {
                                itemCategories.Add("magic");
                                itemCategories.Add("reagents");
                            }
                            if (lowerName.Contains("cloth") || lowerName.Contains("fabric") || lowerName.Contains("thread") ||
                                lowerName.Contains("yarn") || lowerName.Contains("dye") || lowerName.Contains("sewing"))
                            {
                                itemCategories.Add("cloth");
                                itemCategories.Add("tailoring");
                            }
                            if (lowerName.Contains("ore") || lowerName.Contains("ingot") || lowerName.Contains("metal") ||
                                lowerName.Contains("iron") || lowerName.Contains("steel"))
                            {
                                itemCategories.Add("metal");
                                itemCategories.Add("smithing");
                            }
                        }
                    }
                }

                if (itemCategories.Count > 0)
                {
                    List<string> categories = new List<string>(itemCategories);
                    return $"YOUR INVENTORY: You sell {string.Join(", ", categories)}. When discussing your wares, only mention items you actually sell. When players ask for items you don't sell, refer them to the appropriate vendor.";
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[NPCPersonalities] Error getting vendor inventory: {ex.Message}");
            }

            return null;
        }

        /// <summary>
        /// Find NPCs that sell a specific item type based on their actual inventory
        /// </summary>
        public static List<Mobile> FindNPCsByItemType(Mobile npc, string itemRequest, string regionName)
        {
            List<Mobile> matchingNPCs = new List<Mobile>();
            if (npc == null || npc.Map == null || string.IsNullOrEmpty(itemRequest))
                return matchingNPCs;

            string lowerRequest = itemRequest.ToLower();
            
            // Determine what type of item is being requested
            HashSet<string> itemKeywords = new HashSet<string>();
            if (lowerRequest.Contains("raw") || lowerRequest.Contains("meat") || lowerRequest.Contains("leg") ||
                lowerRequest.Contains("ribs") || lowerRequest.Contains("bacon") || lowerRequest.Contains("ham") ||
                lowerRequest.Contains("sausage") || lowerRequest.Contains("bird") || lowerRequest.Contains("chicken"))
            {
                itemKeywords.Add("meat");
                itemKeywords.Add("raw");
            }
            if (lowerRequest.Contains("herb") || lowerRequest.Contains("ginseng") || lowerRequest.Contains("garlic") ||
                lowerRequest.Contains("mandrake") || lowerRequest.Contains("nightshade") || lowerRequest.Contains("bloodmoss") ||
                lowerRequest.Contains("reagent"))
            {
                itemKeywords.Add("herb");
                itemKeywords.Add("reagent");
            }
            if (lowerRequest.Contains("ale") || lowerRequest.Contains("wine") || lowerRequest.Contains("drink") ||
                lowerRequest.Contains("beverage") || lowerRequest.Contains("mug") || lowerRequest.Contains("pitcher"))
            {
                itemKeywords.Add("ale");
                itemKeywords.Add("drink");
            }
            if (lowerRequest.Contains("weapon") || lowerRequest.Contains("sword") || lowerRequest.Contains("bow"))
            {
                itemKeywords.Add("weapon");
            }
            if (lowerRequest.Contains("armor") || lowerRequest.Contains("shield"))
            {
                itemKeywords.Add("armor");
            }

            if (itemKeywords.Count == 0)
                return matchingNPCs;

            // Search region for vendors
            Region currentRegion = npc.Region;
            if (currentRegion == null)
                return matchingNPCs;

            var mobiles = currentRegion.GetMobiles(m => 
                m != npc && 
                m.Alive && 
                m.Map == npc.Map &&
                !m.Player &&
                m is Server.Mobiles.BaseVendor);

            // Parent region fallback
            if (mobiles.Count < 50 && currentRegion.Parent != null)
            {
                mobiles = currentRegion.Parent.GetMobiles(m => 
                    m != npc && 
                    m.Alive && 
                    m.Map == npc.Map &&
                    !m.Player &&
                    m is Server.Mobiles.BaseVendor);
            }

            foreach (Mobile m in mobiles)
            {
                if (m is Server.Mobiles.BaseVendor vendor)
                {
                    var buyInfo = vendor.GetBuyInfo();
                    if (buyInfo == null)
                        continue;

                    bool matches = false;
                    foreach (var buyItem in buyInfo)
                    {
                        if (buyItem is Server.Mobiles.GenericBuyInfo gbi)
                        {
                            string itemName = (gbi.Name ?? gbi.Type?.Name ?? "").ToLower();
                            
                            // Check if this vendor sells items matching the request
                            foreach (string keyword in itemKeywords)
                            {
                                if (itemName.Contains(keyword))
                                {
                                    matches = true;
                                    break;
                                }
                            }
                            if (matches)
                                break;
                        }
                    }

                    if (matches)
                    {
                        matchingNPCs.Add(vendor);
                    }
                }
            }

            return matchingNPCs;
        }

        /// <summary>
        /// Get the most accurate location name for an NPC
        /// Traverses region hierarchy to find the best city/area name
        /// </summary>
        public static string GetLocationName(Mobile npc)
        {
            if (npc == null)
                return null;

            string bestName = null;

            // Try region-based location first
            if (npc.Region != null)
            {
                Region region = npc.Region;

                // Traverse up the region hierarchy to find the most specific named region
                while (region != null)
                {
                    // Check if this region has a valid name
                    if (!string.IsNullOrEmpty(region.Name) && region.Name != "the void")
                    {
                        string lowerName = region.Name.ToLower();

                        // Skip generic dungeon names unless we have nothing else
                        if (!lowerName.Contains("dungeon") || bestName == null)
                        {
                            bestName = region.Name;

                            // If we found a town/city name, prefer it and stop
                            if (IsCityOrTown(region.Name))
                                break;
                        }
                    }

                    region = region.Parent;
                }
            }

            // Fallback: Use coordinate-based location detection for wilderness areas
            if (string.IsNullOrEmpty(bestName) && npc.Map != null)
            {
                bestName = GetLocationByCoordinates(npc.X, npc.Y, npc.Map);
                // Only log if we still couldn't find a location (unusual case)
                if (string.IsNullOrEmpty(bestName))
                {
                    Console.WriteLine($"[NPCPersonalities] WARNING: Could not determine location for NPC at X={npc.X}, Y={npc.Y}, Map={npc.Map.Name}");
                }
            }

            return bestName;
        }

        /// <summary>
        /// Identify location based on coordinates (for wilderness areas without regions)
        /// </summary>
        private static string GetLocationByCoordinates(int x, int y, Map map)
        {
            if (map == null)
                return null;

            // Felucca and Trammel share the same coordinate system
            if (map == Map.Felucca || map == Map.Trammel)
            {
                // Dagger Isle (Ice Island) - northeast
                if (x >= 3650 && x <= 4095 && y >= 1280 && y <= 1792)
                    return "Dagger Isle";

                // Generic wilderness
                return "the wilderness of Britannia";
            }
            else if (map == Map.Ilshenar)
            {
                return "the lost land of Ilshenar";
            }
            else if (map == Map.Malas)
            {
                return "the realm of Malas";
            }
            else if (map == Map.Tokuno)
            {
                return "the Tokuno Islands";
            }
            else if (map == Map.TerMur)
            {
                return "the land of Ter Mur";
            }

            return null;
        }

        /// <summary>
        /// Check if an NPC is a Vystia NPC (by namespace or type name)
        /// </summary>
        private static bool IsVystiaNPC(Mobile npc)
        {
            if (npc == null)
                return false;

            Type npcType = npc.GetType();
            string typeName = npcType.FullName ?? "";
            string typeNamespace = npcType.Namespace ?? "";

            // Check if it's in a Vystia namespace or has Vystia in the name
            string typeNameLower = typeName.ToLower();
            string typeNamespaceLower = typeNamespace.ToLower();
            return typeNamespaceLower.Contains("vystia") ||
                   typeNameLower.Contains("vystia");
        }

        /// <summary>
        /// Check if a region name is a known city or town
        /// </summary>
        private static bool IsCityOrTown(string name)
        {
            if (string.IsNullOrEmpty(name))
                return false;

            string lower = name.ToLower();

            // Major cities
            if (lower.Contains("britain") || lower.Contains("trinsic") ||
                lower.Contains("vesper") || lower.Contains("yew") ||
                lower.Contains("minoc") || lower.Contains("moonglow") ||
                lower.Contains("skara brae") || lower.Contains("jhelom") ||
                lower.Contains("magincia") || lower.Contains("cove") ||
                lower.Contains("serpent") || lower.Contains("nujel'm") ||
                lower.Contains("ocllo") || lower.Contains("buccaneer"))
                return true;

            return false;
        }

        /// <summary>
        /// Format multiple NPC options into a natural string
        /// </summary>
        private static string FormatMultipleNPCs(NPCKnowledgeSystem.NPCRole role, List<NearbyNPCInfo> npcs)
        {
            if (npcs.Count == 0)
                return null;

            string professionName = GetProfessionName(role);

            if (npcs.Count == 1)
            {
                var npc = npcs[0];
                string npcNamePart = !string.IsNullOrEmpty(npc.NPCName) ? $"{npc.NPCName} the " : "";
                string distanceDesc = GetDistanceDescription(npc.Distance);
                return $"You can find {npcNamePart}{professionName} {npc.Direction} of here, {distanceDesc} (coords: {npc.Coordinates.X}, {npc.Coordinates.Y})";
            }
            else
            {
                // Multiple NPCs
                var parts = new List<string>();
                foreach (var npc in npcs)
                {
                    string npcNamePart = !string.IsNullOrEmpty(npc.NPCName) ? $"{npc.NPCName} the " : "";
                    string distanceDesc = GetDistanceDescription(npc.Distance);
                    parts.Add($"{npcNamePart}{professionName} {npc.Direction} ({distanceDesc}, coords: {npc.Coordinates.X}, {npc.Coordinates.Y})");
                }
                return $"There are {professionName}s nearby: {string.Join(", ", parts)}";
            }
        }

        /// <summary>
        /// Get nearby NPC referral information for common referral roles
        /// Uses caching system for performance, with static database and region scanning fallbacks
        /// Also includes item-based vendor information for inventory-aware referrals
        /// Returns a string describing nearby NPCs that can help with referrals
        /// Works for all NPCs implementing ILLMConversational, not just LLMNpc
        /// </summary>
        private static string GetNearbyNPCReferralInfo(Mobile npc, ILLMConversational conversationalNpc)
        {
            if (npc == null || npc.Map == null || npc.Map == Map.Internal)
                return null;

            // Get current region name
            string currentRegionName = GetLocationName(npc);
            if (string.IsNullOrEmpty(currentRegionName))
                currentRegionName = npc.Region?.Name;

            // Get cached nearby NPCs (or build cache if expired)
            var cachedNPCs = GetCachedNearbyNPCs(npc, currentRegionName);

            List<string> nearbyNPCs = new List<string>();

            // Common referral roles to check for
            NPCKnowledgeSystem.NPCRole[] referralRoles = new[]
            {
                NPCKnowledgeSystem.NPCRole.Healer,
                NPCKnowledgeSystem.NPCRole.Mage,
                NPCKnowledgeSystem.NPCRole.Blacksmith,
                NPCKnowledgeSystem.NPCRole.Scholar,
                NPCKnowledgeSystem.NPCRole.Guard,
                NPCKnowledgeSystem.NPCRole.Merchant
            };

            foreach (var targetRole in referralRoles)
            {
                string nearbyInfo = null;

                // Check cache first
                if (cachedNPCs.ContainsKey(targetRole) && cachedNPCs[targetRole].Count > 0)
                {
                    nearbyInfo = FormatMultipleNPCs(targetRole, cachedNPCs[targetRole]);
                }
                else
                {
                    // Cache miss or empty - try region-based lookup
                    nearbyInfo = FindNPCInRegion(npc, targetRole, currentRegionName);
                    
                    // If not found in current region, try scanning the region
                    if (string.IsNullOrEmpty(nearbyInfo))
                    {
                        nearbyInfo = FindNPCByRegionScan(npc, targetRole);
                    }

                    // If still not found, suggest another town
                    if (string.IsNullOrEmpty(nearbyInfo))
                    {
                        nearbyInfo = GetCrossTownReferral(targetRole, currentRegionName);
                    }
                }

                if (!string.IsNullOrEmpty(nearbyInfo))
                {
                    nearbyNPCs.Add(nearbyInfo);
                }
            }

            // Add item-based vendor information
            // Scan for vendors that sell specific item types (meat, herbs, ale, etc.)
            List<string> vendorInfo = new List<string>();
            Region currentRegion = npc.Region;
            if (currentRegion != null)
            {
                var mobiles = currentRegion.GetMobiles(m => 
                    m != npc && 
                    m.Alive && 
                    m.Map == npc.Map &&
                    !m.Player &&
                    m is Server.Mobiles.BaseVendor);

                // Parent region fallback
                if (mobiles.Count < 50 && currentRegion.Parent != null)
                {
                    mobiles = currentRegion.Parent.GetMobiles(m => 
                        m != npc && 
                        m.Alive && 
                        m.Map == npc.Map &&
                        !m.Player &&
                        m is Server.Mobiles.BaseVendor);
                }

                HashSet<string> vendorTypes = new HashSet<string>();
                foreach (Mobile m in mobiles)
                {
                    if (m is Server.Mobiles.BaseVendor vendor)
                    {
                        var buyInfo = vendor.GetBuyInfo();
                        if (buyInfo == null || buyInfo.Length == 0)
                            continue;

                        // Check what this vendor sells
                        bool sellsMeat = false;
                        bool sellsHerbs = false;
                        bool sellsAle = false;

                        foreach (var buyItem in buyInfo)
                        {
                            if (buyItem is Server.Mobiles.GenericBuyInfo gbi)
                            {
                                string itemName = (gbi.Name ?? gbi.Type?.Name ?? "").ToLower();
                                if (itemName.Contains("raw") || itemName.Contains("meat") || itemName.Contains("leg") ||
                                    itemName.Contains("ribs") || itemName.Contains("bacon") || itemName.Contains("ham"))
                                    sellsMeat = true;
                                if (itemName.Contains("herb") || itemName.Contains("ginseng") || itemName.Contains("garlic") ||
                                    itemName.Contains("mandrake") || itemName.Contains("nightshade") || itemName.Contains("bloodmoss"))
                                    sellsHerbs = true;
                                if (itemName.Contains("ale") || itemName.Contains("wine") || itemName.Contains("drink") ||
                                    itemName.Contains("mug") || itemName.Contains("pitcher"))
                                    sellsAle = true;
                            }
                        }

                        if (sellsMeat && !vendorTypes.Contains("butcher"))
                        {
                            vendorTypes.Add("butcher");
                            int dx = npc.X - m.X;
                            int dy = npc.Y - m.Y;
                            double distance = Math.Sqrt((dx * dx) + (dy * dy));
                            string direction = GetDirection(npc.Location, m.Location);
                            string distanceDesc = GetDistanceDescription(distance);
                            string npcName = !string.IsNullOrEmpty(m.Name) ? $"{m.Name} the " : "";
                            vendorInfo.Add($"You can find {npcName}butcher {direction} of here, {distanceDesc} (sells meat, raw meat) (coords: {m.X}, {m.Y})");
                        }
                        if (sellsHerbs && !vendorTypes.Contains("herbalist"))
                        {
                            vendorTypes.Add("herbalist");
                            int dx = npc.X - m.X;
                            int dy = npc.Y - m.Y;
                            double distance = Math.Sqrt((dx * dx) + (dy * dy));
                            string direction = GetDirection(npc.Location, m.Location);
                            string distanceDesc = GetDistanceDescription(distance);
                            string npcName = !string.IsNullOrEmpty(m.Name) ? $"{m.Name} the " : "";
                            vendorInfo.Add($"You can find {npcName}herbalist {direction} of here, {distanceDesc} (sells herbs, reagents) (coords: {m.X}, {m.Y})");
                        }
                        if (sellsAle && !vendorTypes.Contains("tavernkeeper"))
                        {
                            vendorTypes.Add("tavernkeeper");
                            int dx = npc.X - m.X;
                            int dy = npc.Y - m.Y;
                            double distance = Math.Sqrt((dx * dx) + (dy * dy));
                            string direction = GetDirection(npc.Location, m.Location);
                            string distanceDesc = GetDistanceDescription(distance);
                            string npcName = !string.IsNullOrEmpty(m.Name) ? $"{m.Name} the " : "";
                            vendorInfo.Add($"You can find {npcName}tavernkeeper {direction} of here, {distanceDesc} (sells ale, drinks) (coords: {m.X}, {m.Y})");
                        }
                    }
                }
            }

            if (vendorInfo.Count > 0)
            {
                nearbyNPCs.AddRange(vendorInfo);
            }

            if (nearbyNPCs.Count > 0)
            {
                return "Nearby NPCs you can refer players to (or help players find when asked): " + string.Join("; ", nearbyNPCs) + ".";
            }

            return null;
        }

        /// <summary>
        /// Find multiple NPCs in the same region (returns list for caching and multiple options)
        /// Prioritizes static database for town NPCs (they don't wander far from spawn)
        /// Falls back to region scanning only if static database is insufficient
        /// </summary>
        private static List<NearbyNPCInfo> FindNPCsInRegion(Mobile npc, NPCKnowledgeSystem.NPCRole targetRole, string regionName, int maxResults = 3)
        {
            List<NearbyNPCInfo> results = new List<NearbyNPCInfo>();

            if (npc == null || npc.Map == null)
                return results;

            // Prioritize static database lookup (fastest, accurate for town NPCs)
            // Town NPCs stay within their RangeHome (typically 5-10 tiles) of spawn point
            if (!string.IsNullOrEmpty(regionName))
            {
                var staticLocations = NPCLocationDatabase.GetLocations(regionName, targetRole);
                if (staticLocations.Count > 0)
                {
                    // Get all static locations, calculate distances, and sort
                    foreach (var loc in staticLocations)
                    {
                        double distance = Math.Sqrt(
                            Math.Pow(npc.X - loc.Location.X, 2) + 
                            Math.Pow(npc.Y - loc.Location.Y, 2));
                        string direction = GetDirection(npc.Location, loc.Location);
                        results.Add(new NearbyNPCInfo(targetRole, direction, loc.Location, distance, loc.NPCName, loc.Serial));
                    }
                    
                    // Sort by distance and take top results
                    results = results.OrderBy(r => r.Distance).Take(maxResults).ToList();
                    
                    // If we found enough from static database, return early (don't scan region)
                    // Static database is reliable for town NPCs since they don't wander far
                    if (results.Count >= maxResults || results.Count == staticLocations.Count)
                    {
                        return results;
                    }
                }
            }

            // Only scan region if static database didn't find enough
            // This handles cases where NPCs aren't in the static database yet
            if (results.Count < maxResults)
            {
                Region currentRegion = npc.Region;
                if (currentRegion != null)
                {
                    Region searchRegion = currentRegion;
                    var mobiles = currentRegion.GetMobiles(m => 
                        m != npc && 
                        m.Alive && 
                        m.Map == npc.Map &&
                        !m.Player);

                    // Parent region fallback for small sub-regions
                    if (mobiles.Count < 50 && currentRegion.Parent != null)
                    {
                        searchRegion = currentRegion.Parent;
                        mobiles = searchRegion.GetMobiles(m => 
                            m != npc && 
                            m.Alive && 
                            m.Map == npc.Map &&
                            !m.Player);
                    }

                    foreach (Mobile m in mobiles)
                    {
                        if (results.Count >= maxResults)
                            break;

                        NPCKnowledgeSystem.NPCRole? foundRole = null;
                        string npcName = m.Name ?? "";

                        // Check conversational NPCs
                        if (m is ILLMConversational otherConversational && otherConversational.LLMConversationEnabled)
                        {
                            NPCKnowledgeSystem.NPCRole otherRole = NPCKnowledgeSystem.InferRoleFromPersonality(otherConversational.PersonalityType);
                            if (otherRole == targetRole)
                            {
                                foundRole = otherRole;
                            }
                        }
                        // Check BaseVendor types
                        else if (m is Server.Mobiles.BaseVendor vendor)
                        {
                            NPCKnowledgeSystem.NPCRole vendorRole = InferRoleFromVendor(vendor);
                            if (vendorRole == targetRole)
                            {
                                foundRole = vendorRole;
                            }
                        }

                        if (foundRole.HasValue)
                        {
                            int dx = npc.X - m.X;
                            int dy = npc.Y - m.Y;
                            double distance = Math.Sqrt((dx * dx) + (dy * dy));
                            string direction = GetDirection(npc.Location, m.Location);

                            // Avoid duplicates (check if we already have this NPC from static database)
                            if (!results.Any(r => r.Serial == m.Serial))
                            {
                                results.Add(new NearbyNPCInfo(targetRole, direction, m.Location, distance, npcName, m.Serial));
                            }
                        }
                    }
                }
            }

            // Sort by distance and return top results
            return results.OrderBy(r => r.Distance).Take(maxResults).ToList();
        }

        /// <summary>
        /// Find NPC in the same region using region-based lookup (most efficient)
        /// Uses static lookup table for known locations, falls back to region scanning
        /// Works with all ILLMConversational NPCs, not just LLMNpc
        /// Returns formatted string for single NPC (backward compatibility)
        /// </summary>
        private static string FindNPCInRegion(Mobile npc, NPCKnowledgeSystem.NPCRole targetRole, string regionName)
        {
            if (npc == null || npc.Map == null)
                return null;

            // First, try to find in the same region using region scanning
            // This is more efficient than distance-based search
            Region currentRegion = npc.Region;
            if (currentRegion == null)
            {
                Console.WriteLine($"[NPCPersonalities] FindNPCInRegion: No region for NPC {npc.Name}");
                return null;
            }

            // Use region name for logging, but don't require it for the search
            string regionNameForLog = string.IsNullOrEmpty(regionName) ? (currentRegion.Name ?? "Unknown") : regionName;

            Mobile nearestNPC = null;
            int nearestDistance = int.MaxValue;
            int checkedCount = 0;
            int conversationalCount = 0;
            int roleMatchCount = 0;

            // Use Region.GetMobiles() to get all mobiles in the region
            // Check for ILLMConversational interface instead of specific LLMNpc type
            var mobiles = currentRegion.GetMobiles(m => 
                m != npc && 
                m.Alive && 
                m.Map == npc.Map &&
                !m.Player);

            LLMLoggingConfig.LogDebug($"FindNPCInRegion: Searching for {targetRole} in region {regionNameForLog}, found {mobiles.Count} total mobiles");

            // If we're in a small sub-region (like a building), also search the parent region
            // This handles cases where NPCs are in building interiors but need to find NPCs in the town
            Region searchRegion = currentRegion;
            if (mobiles.Count < 50 && currentRegion.Parent != null)
            {
                LLMLoggingConfig.LogDebug($"FindNPCInRegion: Small region detected ({mobiles.Count} mobiles), also searching parent region {currentRegion.Parent.Name}");
                // Search parent region instead (or in addition)
                searchRegion = currentRegion.Parent;
                mobiles = searchRegion.GetMobiles(m => 
                    m != npc && 
                    m.Alive && 
                    m.Map == npc.Map &&
                    !m.Player);
                LLMLoggingConfig.LogDebug($"FindNPCInRegion: Parent region {searchRegion.Name} has {mobiles.Count} total mobiles");
            }

            foreach (Mobile m in mobiles)
            {
                checkedCount++;
                
                // First check if it's a conversational NPC
                if (m is ILLMConversational otherConversational && otherConversational.LLMConversationEnabled)
                {
                    conversationalCount++;
                    // Check if this NPC has the target role
                    NPCKnowledgeSystem.NPCRole otherRole = NPCKnowledgeSystem.InferRoleFromPersonality(otherConversational.PersonalityType);
                    if (otherRole == targetRole)
                    {
                        roleMatchCount++;
                        // Calculate distance
                        int dx = npc.X - m.X;
                        int dy = npc.Y - m.Y;
                        int distance = (dx * dx) + (dy * dy);

                        if (distance < nearestDistance)
                        {
                            nearestDistance = distance;
                            nearestNPC = m;
                            Console.WriteLine($"[NPCPersonalities] FindNPCInRegion: Found {targetRole} '{m.Name}' at distance {Math.Sqrt(distance):F0} tiles");
                        }
                    }
                }
                // Also check for BaseVendor types that might match the role
                else if (m is Server.Mobiles.BaseVendor vendor)
                {
                    // Try to infer role from vendor type or name
                    NPCKnowledgeSystem.NPCRole vendorRole = InferRoleFromVendor(vendor);
                    if (vendorRole == targetRole)
                    {
                        roleMatchCount++;
                        int dx = npc.X - m.X;
                        int dy = npc.Y - m.Y;
                        int distance = (dx * dx) + (dy * dy);

                        if (distance < nearestDistance)
                        {
                            nearestDistance = distance;
                            nearestNPC = m;
                            Console.WriteLine($"[NPCPersonalities] FindNPCInRegion: Found {targetRole} vendor '{m.Name}' at distance {Math.Sqrt(distance):F0} tiles");
                        }
                    }
                }
            }

            LLMLoggingConfig.LogDebug($"FindNPCInRegion: Checked {checkedCount} mobiles, {conversationalCount} conversational, {roleMatchCount} matched role {targetRole}");

            if (nearestNPC == null)
            {
                LLMLoggingConfig.LogDebug($"FindNPCInRegion: No {targetRole} found in region {regionNameForLog}");
                return null;
            }

            // Use the new method that supports multiple NPCs, but return single result for backward compatibility
            var npcs = FindNPCsInRegion(npc, targetRole, regionName, maxResults: 1);
            if (npcs.Count == 0)
            {
                LLMLoggingConfig.LogDebug($"FindNPCInRegion: No {targetRole} found in region {regionNameForLog}");
                return null;
            }

            var nearest = npcs[0];
            string professionName = GetProfessionName(targetRole);
            string distanceDesc = GetDistanceDescription(nearest.Distance);
            string npcNamePart = !string.IsNullOrEmpty(nearest.NPCName) ? $"{nearest.NPCName} the " : "";

            Console.WriteLine($"[NPCPersonalities] FindNPCInRegion: Returning direction to {professionName} {nearest.Direction} at ({nearest.Coordinates.X}, {nearest.Coordinates.Y})");

            // Include coordinates for testing/debugging
            return $"You can find {npcNamePart}{professionName} {nearest.Direction} of here, {distanceDesc} (coords: {nearest.Coordinates.X}, {nearest.Coordinates.Y})";
        }

        /// <summary>
        /// Infer NPC role from BaseVendor type or name
        /// </summary>
        private static NPCKnowledgeSystem.NPCRole InferRoleFromVendor(Server.Mobiles.BaseVendor vendor)
        {
            // Check vendor type
            string vendorTypeName = vendor.GetType().Name.ToLower();
            string vendorName = vendor.Name?.ToLower() ?? "";

            // Healer types
            if (vendorTypeName.Contains("healer") || vendorTypeName.Contains("herbalist") ||
                vendorName.Contains("healer") || vendorName.Contains("herbalist"))
            {
                return NPCKnowledgeSystem.NPCRole.Healer;
            }

            // Mage types
            if (vendorTypeName.Contains("mage") || vendorTypeName.Contains("wizard") ||
                vendorName.Contains("mage") || vendorName.Contains("wizard"))
            {
                return NPCKnowledgeSystem.NPCRole.Mage;
            }

            // Blacksmith types
            if (vendorTypeName.Contains("blacksmith") || vendorTypeName.Contains("smith") ||
                vendorName.Contains("blacksmith") || vendorName.Contains("smith"))
            {
                return NPCKnowledgeSystem.NPCRole.Blacksmith;
            }

            // Scholar types
            if (vendorTypeName.Contains("scholar") || vendorTypeName.Contains("sage") ||
                vendorName.Contains("scholar") || vendorName.Contains("sage") || vendorName.Contains("librarian"))
            {
                return NPCKnowledgeSystem.NPCRole.Scholar;
            }

            // Guard types
            if (vendorTypeName.Contains("guard") || vendorName.Contains("guard"))
            {
                return NPCKnowledgeSystem.NPCRole.Guard;
            }

            // Merchant types (default for most vendors)
            return NPCKnowledgeSystem.NPCRole.Merchant;
        }

        /// <summary>
        /// Fallback: Scan the entire region for NPCs (slower but comprehensive)
        /// Works with all ILLMConversational NPCs, not just LLMNpc
        /// </summary>
        private static string FindNPCByRegionScan(Mobile npc, NPCKnowledgeSystem.NPCRole targetRole)
        {
            if (npc == null || npc.Region == null)
                return null;

            Mobile nearestNPC = null;
            int nearestDistance = int.MaxValue;

            // Scan all mobiles in the region (including non-conversational NPCs)
            var mobiles = npc.Region.GetMobiles(m => 
                m != npc && 
                m.Alive && 
                m.Map == npc.Map &&
                !m.Player);

            foreach (Mobile m in mobiles)
            {
                NPCKnowledgeSystem.NPCRole otherRole = NPCKnowledgeSystem.NPCRole.Commoner;

                // Check if it's a conversational NPC
                if (m is ILLMConversational otherConversational && otherConversational.LLMConversationEnabled)
                {
                    otherRole = NPCKnowledgeSystem.InferRoleFromPersonality(otherConversational.PersonalityType);
                }
                // Check if it's a BaseVendor
                else if (m is Server.Mobiles.BaseVendor vendor)
                {
                    otherRole = InferRoleFromVendor(vendor);
                }

                if (otherRole == targetRole)
                {
                    int dx = npc.X - m.X;
                    int dy = npc.Y - m.Y;
                    int distance = (dx * dx) + (dy * dy);

                    if (distance < nearestDistance)
                    {
                        nearestDistance = distance;
                        nearestNPC = m;
                    }
                }
            }

            if (nearestNPC == null)
                return null;

            string direction = GetDirection(npc.Location, nearestNPC.Location);
            string professionName = GetProfessionName(targetRole);

            // Include coordinates for testing/debugging
            return $"You can find a {professionName} {direction} of here (coords: {nearestNPC.X}, {nearestNPC.Y})";
        }

        /// <summary>
        /// Get cross-town referral when no NPC found in current region
        /// </summary>
        private static string GetCrossTownReferral(NPCKnowledgeSystem.NPCRole targetRole, string currentRegionName)
        {
            // Map of roles to towns that typically have those NPCs
            Dictionary<NPCKnowledgeSystem.NPCRole, string[]> roleToTowns = new Dictionary<NPCKnowledgeSystem.NPCRole, string[]>
            {
                { NPCKnowledgeSystem.NPCRole.Healer, new[] { "Moonglow", "Yew", "Trinsic" } },
                { NPCKnowledgeSystem.NPCRole.Mage, new[] { "Moonglow", "Britain", "Minoc" } },
                { NPCKnowledgeSystem.NPCRole.Blacksmith, new[] { "Minoc", "Britain", "Vesper" } },
                { NPCKnowledgeSystem.NPCRole.Scholar, new[] { "Moonglow", "Britain", "Vesper" } },
                { NPCKnowledgeSystem.NPCRole.Guard, new[] { "Britain", "Trinsic", "Vesper" } },
                { NPCKnowledgeSystem.NPCRole.Merchant, new[] { "Britain", "Vesper", "Trinsic" } }
            };

            if (roleToTowns.TryGetValue(targetRole, out string[] towns))
            {
                // Find a town that's different from current region
                string targetTown = null;
                foreach (var town in towns)
                {
                    if (!string.IsNullOrEmpty(currentRegionName) && 
                        currentRegionName.IndexOf(town, StringComparison.OrdinalIgnoreCase) < 0)
                    {
                        targetTown = town;
                        break;
                    }
                }

                // If all towns match current region, just use the first one
                if (targetTown == null && towns.Length > 0)
                    targetTown = towns[0];

                if (!string.IsNullOrEmpty(targetTown))
                {
                    string professionName = GetProfessionName(targetRole);
                    return $"You can seek a {professionName} in {targetTown}";
                }
            }

            return null;
        }

        /// <summary>
        /// Calculate cardinal/intercardinal direction from source to target
        /// </summary>
        private static string GetDirection(Point3D source, Point3D target)
        {
            int dx = target.X - source.X;
            int dy = target.Y - source.Y;

            // Normalize to get primary directions
            // In UO, Y increases going north, X increases going east
            // So: negative Y = north, positive Y = south
            //     positive X = east, negative X = west

            // Calculate angle (0 = north, 90 = east, 180 = south, 270 = west)
            double angle = Math.Atan2(dx, -dy) * 180.0 / Math.PI;
            if (angle < 0) angle += 360;

            // Convert to cardinal/intercardinal directions
            if (angle >= 337.5 || angle < 22.5)
                return "north";
            else if (angle >= 22.5 && angle < 67.5)
                return "north east";
            else if (angle >= 67.5 && angle < 112.5)
                return "east";
            else if (angle >= 112.5 && angle < 157.5)
                return "south east";
            else if (angle >= 157.5 && angle < 202.5)
                return "south";
            else if (angle >= 202.5 && angle < 247.5)
                return "south west";
            else if (angle >= 247.5 && angle < 292.5)
                return "west";
            else // 292.5 to 337.5
                return "north west";
        }

        /// <summary>
        /// Get distance description based on tile distance
        /// </summary>
        private static string GetDistanceDescription(double distanceInTiles)
        {
            if (distanceInTiles < 30)
                return "nearby";
            else if (distanceInTiles < 90)
                return "a short walk";
            else if (distanceInTiles < 180)
                return "across town";
            else
                return "far from here";
        }

        /// <summary>
        /// Get a friendly profession name for a role
        /// </summary>
        private static string GetProfessionName(NPCKnowledgeSystem.NPCRole role)
        {
            switch (role)
            {
                case NPCKnowledgeSystem.NPCRole.Healer:
                    return "healer";
                case NPCKnowledgeSystem.NPCRole.Mage:
                    return "mage";
                case NPCKnowledgeSystem.NPCRole.Blacksmith:
                    return "blacksmith";
                case NPCKnowledgeSystem.NPCRole.Scholar:
                    return "scholar";
                case NPCKnowledgeSystem.NPCRole.Guard:
                    return "guard";
                case NPCKnowledgeSystem.NPCRole.Merchant:
                    return "merchant";
                case NPCKnowledgeSystem.NPCRole.Innkeeper:
                    return "innkeeper";
                case NPCKnowledgeSystem.NPCRole.Ranger:
                    return "ranger";
                case NPCKnowledgeSystem.NPCRole.Vendor:
                    return "vendor";
                case NPCKnowledgeSystem.NPCRole.Jeweler:
                    return "jeweler";
                case NPCKnowledgeSystem.NPCRole.Weaponsmith:
                    return "weaponsmith";
                case NPCKnowledgeSystem.NPCRole.Armorer:
                    return "armorer";
                default:
                    return "expert";
            }
        }

        /// <summary>
        /// Find NPC by profession name (for direct "where is" questions)
        /// Can be called to find any profession, not just common referral roles
        /// </summary>
        public static string FindNPCByProfession(Mobile npc, string professionName)
        {
            if (npc == null || npc.Map == null || npc.Map == Map.Internal || string.IsNullOrEmpty(professionName))
                return null;

            // Map profession names to roles
            Dictionary<string, NPCKnowledgeSystem.NPCRole> professionToRole = new Dictionary<string, NPCKnowledgeSystem.NPCRole>(StringComparer.OrdinalIgnoreCase)
            {
                { "healer", NPCKnowledgeSystem.NPCRole.Healer },
                { "mage", NPCKnowledgeSystem.NPCRole.Mage },
                { "wizard", NPCKnowledgeSystem.NPCRole.Mage },
                { "blacksmith", NPCKnowledgeSystem.NPCRole.Blacksmith },
                { "smith", NPCKnowledgeSystem.NPCRole.Blacksmith },
                { "scholar", NPCKnowledgeSystem.NPCRole.Scholar },
                { "guard", NPCKnowledgeSystem.NPCRole.Guard },
                { "merchant", NPCKnowledgeSystem.NPCRole.Merchant },
                { "vendor", NPCKnowledgeSystem.NPCRole.Vendor },
                { "innkeeper", NPCKnowledgeSystem.NPCRole.Innkeeper },
                { "ranger", NPCKnowledgeSystem.NPCRole.Ranger },
                { "jeweler", NPCKnowledgeSystem.NPCRole.Jeweler },
                { "weaponsmith", NPCKnowledgeSystem.NPCRole.Weaponsmith },
                { "armorer", NPCKnowledgeSystem.NPCRole.Armorer }
            };

            if (!professionToRole.TryGetValue(professionName.Trim(), out NPCKnowledgeSystem.NPCRole targetRole))
                return null;

            string currentRegionName = GetLocationName(npc);
            if (string.IsNullOrEmpty(currentRegionName))
                currentRegionName = npc.Region?.Name;

            // Try region-based lookup first
            var nearbyInfo = FindNPCInRegion(npc, targetRole, currentRegionName);
            
            // If not found, try scanning the region
            if (string.IsNullOrEmpty(nearbyInfo))
            {
                nearbyInfo = FindNPCByRegionScan(npc, targetRole);
            }

            // If still not found, suggest another town
            if (string.IsNullOrEmpty(nearbyInfo))
            {
                nearbyInfo = GetCrossTownReferral(targetRole, currentRegionName);
            }

            return nearbyInfo;
        }

        /// <summary>
        /// Add personality-appropriate clothing to a mobile
        /// </summary>
        public static void AddPersonalityClothing(Mobile mobile, PersonalityType personality)
        {
            if (mobile == null)
                return;

            switch (personality)
            {
                case PersonalityType.Merchant:
                    mobile.AddItem(new Server.Items.FancyShirt(Utility.RandomNeutralHue()));
                    mobile.AddItem(new Server.Items.LongPants(Utility.RandomNeutralHue()));
                    mobile.AddItem(new Server.Items.Boots());
                    mobile.AddItem(new Server.Items.HalfApron(Utility.RandomBrightHue()));
                    break;

                case PersonalityType.Guard:
                    mobile.AddItem(new Server.Items.PlateChest());
                    mobile.AddItem(new Server.Items.PlateArms());
                    mobile.AddItem(new Server.Items.PlateLegs());
                    mobile.AddItem(new Server.Items.PlateGorget());
                    mobile.AddItem(new Server.Items.Halberd());
                    break;

                case PersonalityType.Noble:
                    mobile.AddItem(new Server.Items.FancyShirt(Utility.RandomBlueHue()));
                    mobile.AddItem(new Server.Items.LongPants(Utility.RandomBlueHue()));
                    mobile.AddItem(new Server.Items.Boots(Utility.RandomNeutralHue()));
                    mobile.AddItem(new Server.Items.FeatheredHat(Utility.RandomRedHue()));
                    break;

                case PersonalityType.Sage:
                case PersonalityType.Mage:
                    mobile.AddItem(new Server.Items.Robe(Utility.RandomBlueHue()));
                    mobile.AddItem(new Server.Items.WizardsHat(Utility.RandomBlueHue()));
                    mobile.AddItem(new Server.Items.Sandals());
                    break;

                case PersonalityType.Healer:
                    mobile.AddItem(new Server.Items.Robe(Utility.RandomPinkHue()));
                    mobile.AddItem(new Server.Items.Sandals());
                    break;

                case PersonalityType.Warrior:
                    mobile.AddItem(new Server.Items.StuddedChest());
                    mobile.AddItem(new Server.Items.StuddedArms());
                    mobile.AddItem(new Server.Items.StuddedLegs());
                    mobile.AddItem(new Server.Items.Longsword());
                    break;

                case PersonalityType.Villain:
                    mobile.AddItem(new Server.Items.Robe(Utility.RandomMinMax(0x0001, 0x0003))); // Dark colors
                    mobile.AddItem(new Server.Items.Sandals());
                    break;

                case PersonalityType.Hermit:
                    mobile.AddItem(new Server.Items.Robe(Utility.RandomNeutralHue()));
                    mobile.AddItem(new Server.Items.Sandals());
                    mobile.AddItem(new Server.Items.GnarledStaff());
                    break;

                default: // Commoner
                    mobile.AddItem(new Server.Items.Shirt(Utility.RandomNeutralHue()));
                    mobile.AddItem(new Server.Items.ShortPants(Utility.RandomNeutralHue()));
                    mobile.AddItem(new Server.Items.Shoes());
                    break;
            }
        }

        #region Relationship-Based Greetings

        /// <summary>
        /// Gets a relationship-based greeting for an NPC based on their personality and relationship level
        /// </summary>
        public static string GetRelationshipBasedGreeting(PersonalityType personality, RelationshipType relationshipType, int relationshipScore)
        {
            // Determine relationship level from score
            string relationshipLevel = GetRelationshipLevel(relationshipScore);
            
            if (personality == PersonalityType.Healer)
                return GetHealerGreeting(relationshipLevel, relationshipType);
            else if (personality == PersonalityType.InnKeeper)
                return GetInnkeeperGreeting(relationshipLevel, relationshipType);
            else if (personality == PersonalityType.Merchant)
                return GetMerchantGreeting(relationshipLevel, relationshipType);
            else if (personality == PersonalityType.Guard)
                return GetGuardGreeting(relationshipLevel, relationshipType);
            else if (personality == PersonalityType.Sage)
                return GetScholarGreeting(relationshipLevel, relationshipType);
            else if (personality == PersonalityType.Blacksmith)
                return GetBlacksmithGreeting(relationshipLevel, relationshipType);
            else if (personality == PersonalityType.Alchemist)
                return GetAlchemistGreeting(relationshipLevel, relationshipType);
            else if (personality == PersonalityType.Bard)
                return GetBardGreeting(relationshipLevel, relationshipType);
            else if (personality == PersonalityType.Villain)
                return GetVillainGreeting(relationshipLevel, relationshipType);
            else if (personality == PersonalityType.Noble)
                return GetNobleGreeting(relationshipLevel, relationshipType);
            else if (personality == PersonalityType.Commoner)
                return GetPeasantGreeting(relationshipLevel, relationshipType);
            else if (personality == PersonalityType.Warrior)
                return GetWarriorGreeting(relationshipLevel, relationshipType);
            else if (personality == PersonalityType.Hermit)
                return GetHermitGreeting(relationshipLevel, relationshipType);
            else
                return GetCommonerGreeting(relationshipLevel, relationshipType);
        }

        private static string GetRelationshipLevel(int score)
        {
            if (score >= 80) return "Best Friend";
            if (score >= 60) return "Good Friend";
            if (score >= 40) return "Friend";
            if (score >= 20) return "Acquaintance";
            if (score >= 0) return "Stranger";
            return "Disliked";
        }

        private static string GetHealerGreeting(string level, RelationshipType type)
        {
            switch (level)
            {
                case "Best Friend":
                    return "My dear friend! It's wonderful to see you again. How are you feeling today?";
                case "Good Friend":
                    return "Hello there! Always good to see a friendly face. Are you keeping well?";
                case "Friend":
                    return "Greetings! Come, let me check if you need any healing.";
                case "Acquaintance":
                    return "Hello. If you need healing or remedies, I'm here to help.";
                case "Stranger":
                    return "Welcome. If you're injured or unwell, I can tend to your wounds.";
                case "Disliked":
                    return "What do you want? If you're hurt, make it quick.";
                default:
                    return "Greetings.";
            }
        }

        private static string GetInnkeeperGreeting(string level, RelationshipType type)
        {
            switch (level)
            {
                case "Best Friend":
                    return "Hey! Your usual spot's waiting for you! Good to see you back!";
                case "Good Friend":
                    return "Welcome back! The usual today, or something special?";
                case "Friend":
                    return "Hello! Come on in, have a seat. What can I get for you?";
                case "Acquaintance":
                    return "Welcome to my inn. What can I get you today?";
                case "Stranger":
                    return "Greetings, traveler. Welcome to my establishment.";
                case "Disliked":
                    return "What do you want? If you're not buying, don't block the entrance.";
                default:
                    return "Welcome.";
            }
        }

        private static string GetMerchantGreeting(string level, RelationshipType type)
        {
            switch (level)
            {
                case "Best Friend":
                    return "Ah, my favorite customer! I have some new items just for you!";
                case "Good Friend":
                    return "Welcome back! I've got some fine wares for you today.";
                case "Friend":
                    return "Hello! Come browse my wares, I think you'll find something good.";
                case "Acquaintance":
                    return "Greetings! Feel free to browse my selection.";
                case "Stranger":
                    return "Welcome, customer. Let me know if you need anything.";
                case "Disliked":
                    return "If you're not buying, don't touch the merchandise.";
                default:
                    return "Greetings.";
            }
        }

        private static string GetGuardGreeting(string level, RelationshipType type)
        {
            switch (level)
            {
                case "Best Friend":
                    return "Good to see you! Everything's quiet on my watch today.";
                case "Good Friend":
                    return "Hello! Keeping the peace, as always. All well with you?";
                case "Friend":
                    return "Greetings! Stay safe out there.";
                case "Acquaintance":
                    return "Hello. Move along, nothing to see here.";
                case "Stranger":
                    return "State your business. No trouble, I hope.";
                case "Disliked":
                    return "What do you want? Don't cause any trouble.";
                default:
                    return "Halt, citizen.";
            }
        }

        private static string GetScholarGreeting(string level, RelationshipType type)
        {
            switch (level)
            {
                case "Best Friend":
                    return "Ah, my friend! Come to discuss some fascinating topic, I hope?";
                case "Good Friend":
                    return "Greetings! I was just reading something interesting you might enjoy.";
                case "Friend":
                    return "Hello! Always good to see an eager mind.";
                case "Acquaintance":
                    return "Greetings. Seek knowledge, do you?";
                case "Stranger":
                    return "Welcome. The pursuit of knowledge is noble.";
                case "Disliked":
                    return "If you're here to disturb my studies, think again.";
                default:
                    return "Greetings.";
            }
        }

        private static string GetBlacksmithGreeting(string level, RelationshipType type)
        {
            switch (level)
            {
                case "Best Friend":
                    return "Hey! Your gear's looking good. Need any repairs or upgrades?";
                case "Good Friend":
                    return "Welcome back! Got your equipment ready for you.";
                case "Friend":
                    return "Hello! Need some metalwork done?";
                case "Acquaintance":
                    return "Greetings. Need weapons or armor?";
                case "Stranger":
                    return "Welcome to my forge. What do you need?";
                case "Disliked":
                    return "If you're not here to buy, don't waste my time.";
                default:
                    return "Greetings.";
            }
        }

        private static string GetAlchemistGreeting(string level, RelationshipType type)
        {
            switch (level)
            {
                case "Best Friend":
                    return "Ah, my friend! Come to try some new concoctions?";
                case "Good Friend":
                    return "Welcome! I have some fresh potions ready for you.";
                case "Friend":
                    return "Greetings! Need some magical assistance?";
                case "Acquaintance":
                    return "Hello. Potions and reagents available.";
                case "Stranger":
                    return "Welcome. The arcane arts require precision.";
                case "Disliked":
                    return "If you break anything, you buy it.";
                default:
                    return "Greetings.";
            }
        }

        private static string GetBardGreeting(string level, RelationshipType type)
        {
            switch (level)
            {
                case "Best Friend":
                    return "My friend! Come to hear a new song I've composed?";
                case "Good Friend":
                    return "Welcome! I have a tale just for you today.";
                case "Friend":
                    return "Greetings! Care to hear some music?";
                case "Acquaintance":
                    return "Hello. Music soothes the soul, does it not?";
                case "Stranger":
                    return "Welcome! A song for a coin?";
                case "Disliked":
                    return "If you don't appreciate art, move along.";
                default:
                    return "Greetings.";
            }
        }

        private static string GetCriminalGreeting(string level, RelationshipType type)
        {
            switch (level)
            {
                case "Best Friend":
                    return "Hey! Got something good for you today, if you're interested.";
                case "Good Friend":
                    return "Psst... over here. Got something special.";
                case "Friend":
                    return "Hey there. Looking for something... unusual?";
                case "Acquaintance":
                    return "What do you want? Keep your voice down.";
                case "Stranger":
                    return "Who are you? What do you want?";
                case "Disliked":
                    return "Get lost before I get angry.";
                default:
                    return "What do you want?";
            }
        }

        private static string GetNobleGreeting(string level, RelationshipType type)
        {
            switch (level)
            {
                case "Best Friend":
                    return "My dear friend! So good to see you again. Do join me.";
                case "Good Friend":
                    return "Ah, welcome! It's always a pleasure to see you.";
                case "Friend":
                    return "Greetings! Fine weather we're having, isn't it?";
                case "Acquaintance":
                    return "Welcome. I hope you find what you seek.";
                case "Stranger":
                    return "Greetings. State your purpose, if you please.";
                case "Disliked":
                    return "I don't believe we have anything to discuss.";
                default:
                    return "Greetings.";
            }
        }

        private static string GetPeasantGreeting(string level, RelationshipType type)
        {
            switch (level)
            {
                case "Best Friend":
                    return "Hey there! Good to see ya! Need any help with the crops?";
                case "Good Friend":
                    return "Hello! Nice day for working, eh?";
                case "Friend":
                    return "Greetings! How's the harvest treating you?";
                case "Acquaintance":
                    return "Hello there. Working hard?";
                case "Stranger":
                    return "Greetings. Just trying to make a living.";
                case "Disliked":
                    return "What do you want? I'm busy.";
                default:
                    return "Greetings.";
            }
        }

        private static string GetChildGreeting(string level, RelationshipType type)
        {
            switch (level)
            {
                case "Best Friend":
                    return "Yay! You're back! Wanna play?";
                case "Good Friend":
                    return "Hello! Want to hear a secret?";
                case "Friend":
                    return "Hi! What are you doing?";
                case "Acquaintance":
                    return "Hello! Are you nice?";
                case "Stranger":
                    return "Hi! Who are you?";
                case "Disliked":
                    return "I don't like you. Go away.";
                default:
                    return "Hello!";
            }
        }

        private static string GetWarriorGreeting(string level, RelationshipType type)
        {
            switch (level)
            {
                case "Best Friend":
                    return "My friend! Good to see you in one piece. Been in any good fights?";
                case "Good Friend":
                    return "Welcome! Keeping your skills sharp, I hope.";
                case "Friend":
                    return "Greetings! Stay strong out there.";
                case "Acquaintance":
                    return "Hello. Training today?";
                case "Stranger":
                    return "State your business. Are you a warrior?";
                case "Disliked":
                    return "What do you want? Don't waste my time.";
                default:
                    return "Greetings.";
            }
        }

        private static string GetVillainGreeting(string level, RelationshipType type)
        {
            switch (level)
            {
                case "Best Friend":
                    return "Ah, my ally! Come, we have much to discuss...";
                case "Good Friend":
                    return "Welcome. Your... talents... are always appreciated.";
                case "Friend":
                    return "Greetings. You understand the way things truly work.";
                case "Acquaintance":
                    return "What do you want? Make it quick.";
                case "Stranger":
                    return "Who dares approach me? State your purpose.";
                case "Disliked":
                    return "Fool! You waste my valuable time.";
                default:
                    return "What do you want?";
            }
        }

        private static string GetHermitGreeting(string level, RelationshipType type)
        {
            switch (level)
            {
                case "Best Friend":
                    return "Ah... you're back. The woods have been quiet without you.";
                case "Good Friend":
                    return "Hello. Not many come to visit. What brings you?";
                case "Friend":
                    return "Greetings. The wilderness suits you, I think.";
                case "Acquaintance":
                    return "Hello. Don't touch anything.";
                case "Stranger":
                    return "Who are you? Why are you here?";
                case "Disliked":
                    return "Leave me be. I want no company.";
                default:
                    return "What do you want?";
            }
        }

        private static string GetCommonerGreeting(string level, RelationshipType type)
        {
            switch (level)
            {
                case "Best Friend":
                    return "Hey! Good to see you again! How have you been?";
                case "Good Friend":
                    return "Hello! Nice to see a friendly face.";
                case "Friend":
                    return "Greetings! How are you today?";
                case "Acquaintance":
                    return "Hello. Can I help you with something?";
                case "Stranger":
                    return "Greetings. Welcome to our town.";
                case "Disliked":
                    return "What do you want? I'm busy.";
                default:
                    return "Greetings.";
            }
        }

        #endregion
    }
}

