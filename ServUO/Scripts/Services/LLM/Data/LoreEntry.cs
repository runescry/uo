using System;
using System.Collections.Generic;

namespace Server.Services.LLM
{
    /// <summary>
    /// Represents a single piece of world lore
    /// </summary>
    public class LoreEntry
    {
        public string ID { get; set; }
        public string Category { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public List<string> Tags { get; set; }
        public int Importance { get; set; }
        public string Source { get; set; }

        /// <summary>
        /// Short summary for tier-1 compressed knowledge (30 chars max)
        /// </summary>
        public string Summary { get; set; }

        /// <summary>
        /// Domain expertise mapping: which NPC roles have what level of knowledge
        /// Key: NPCRole as string, Value: KnowledgeExpertise level
        /// </summary>
        public Dictionary<string, KnowledgeExpertise> DomainExpertise { get; set; }

        /// <summary>
        /// Question category for domain classification
        /// </summary>
        public QuestionCategory QuestionCategory { get; set; }

        /// <summary>
        /// Primary referral target (which role should answer this)
        /// </summary>
        public string PrimaryReferralTarget { get; set; }

        /// <summary>
        /// Secondary referral target (alternative expert)
        /// </summary>
        public string SecondaryReferralTarget { get; set; }

        public LoreEntry()
        {
            Tags = new List<string>();
            DomainExpertise = new Dictionary<string, KnowledgeExpertise>();
            QuestionCategory = QuestionCategory.General;
        }

        /// <summary>
        /// Get expertise level for a specific NPC role
        /// </summary>
        public KnowledgeExpertise GetExpertise(NPCKnowledgeSystem.NPCRole role)
        {
            string roleKey = role.ToString();
            if (DomainExpertise.ContainsKey(roleKey))
                return DomainExpertise[roleKey];

            // Default based on importance and category
            return GetDefaultExpertise(role);
        }

        /// <summary>
        /// Determine default expertise level based on role and lore category
        /// </summary>
        private KnowledgeExpertise GetDefaultExpertise(NPCKnowledgeSystem.NPCRole role)
        {
            // High importance lore = everyone Aware
            if (Importance >= 9)
                return KnowledgeExpertise.Aware;

            // Scholars know everything at Proficient minimum
            if (role == NPCKnowledgeSystem.NPCRole.Scholar)
                return KnowledgeExpertise.Proficient;

            // Category-based defaults
            switch (Category)
            {
                case "History":
                    if (role == NPCKnowledgeSystem.NPCRole.Scholar)
                        return KnowledgeExpertise.Expert;
                    return KnowledgeExpertise.Aware; // Everyone knows basic history

                case "Magic":
                    if (role == NPCKnowledgeSystem.NPCRole.Mage)
                        return KnowledgeExpertise.Expert;
                    return KnowledgeExpertise.Ignorant; // Most don't know magic

                case "Crafting":
                    if (role == NPCKnowledgeSystem.NPCRole.Blacksmith ||
                        role == NPCKnowledgeSystem.NPCRole.Weaponsmith ||
                        role == NPCKnowledgeSystem.NPCRole.Armorer)
                        return KnowledgeExpertise.Expert;
                    return KnowledgeExpertise.Ignorant;

                case "Monster":
                case "Dungeon":
                    if (role == NPCKnowledgeSystem.NPCRole.Guard ||
                        role == NPCKnowledgeSystem.NPCRole.Ranger)
                        return KnowledgeExpertise.Expert;
                    return KnowledgeExpertise.Aware; // General knowledge

                case "Location":
                    // Everyone knows about locations to some degree
                    return KnowledgeExpertise.Proficient;

                default:
                    return KnowledgeExpertise.Aware;
            }
        }
    }

    /// <summary>
    /// Categories for lore entries
    /// </summary>
    public enum LoreCategory
    {
        Location,      // Cities, dungeons, landmarks
        History,       // Historical events, wars, ages
        Faction,       // Organizations, guilds
        NPC,           // Notable characters
        Item,          // Legendary items, artifacts
        Monster,       // Creature lore
        Magic,         // Spells, magical theory
        Virtue,        // The eight virtues
        Quest,         // Quest backgrounds
        Custom         // Server-specific lore
    }
}
