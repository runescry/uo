using System;
using System.Collections.Generic;

namespace Server.Services.LLM
{
    /// <summary>
    /// Defines expertise levels for NPC knowledge domains
    /// Enables authentic NPC interactions by limiting knowledge to realistic boundaries
    /// </summary>
    public enum KnowledgeExpertise
    {
        /// <summary>
        /// Deep knowledge, can explain at length with details
        /// Example: Blacksmith explaining forging techniques
        /// </summary>
        Expert,

        /// <summary>
        /// Working knowledge, practical understanding
        /// Example: Blacksmith knowing dungeon locations from customer stories
        /// </summary>
        Proficient,

        /// <summary>
        /// Basic facts only, knows it exists
        /// Example: Blacksmith knowing the Eight Virtues exist
        /// </summary>
        Aware,

        /// <summary>
        /// Doesn't know, should refer elsewhere
        /// Example: Blacksmith asked about magic spells
        /// </summary>
        Ignorant
    }

    /// <summary>
    /// Question categories for domain classification
    /// </summary>
    public enum QuestionCategory
    {
        Magic,           // Spells, reagents, magical theory
        History,         // Historical events, timelines, ancient lore
        Healing,         // Medicine, ailments, potions, herbs
        Law,             // Legal matters, justice, courts
        Combat,          // Battle tactics, weapons usage, fighting
        Crafting,        // Smithing, tailoring, crafting techniques
        Geography,       // Locations, cities, travel routes
        Monsters,        // Creatures, threats, weaknesses
        Dungeons,        // Dungeon layouts, dangers, treasures
        Trade,           // Commerce, pricing, supply/demand
        Religion,        // Virtues, spirituality, philosophy
        Nature,          // Plants, animals, weather, seasons
        Technology,      // Mechanisms, constructs, inventions
        General          // Miscellaneous or unknown
    }

    /// <summary>
    /// Defines an NPC role's knowledge domain with expertise levels
    /// </summary>
    public class RoleKnowledgeDomain
    {
        public NPCKnowledgeSystem.NPCRole Role { get; set; }
        public Dictionary<QuestionCategory, KnowledgeExpertise> DomainExpertise { get; set; }
        public Dictionary<QuestionCategory, string> ReferralPhrases { get; set; }

        public RoleKnowledgeDomain(NPCKnowledgeSystem.NPCRole role)
        {
            Role = role;
            DomainExpertise = new Dictionary<QuestionCategory, KnowledgeExpertise>();
            ReferralPhrases = new Dictionary<QuestionCategory, string>();
            InitializeDomains();
        }

        /// <summary>
        /// Initialize expertise levels and referral phrases for this role
        /// </summary>
        private void InitializeDomains()
        {
            switch (Role)
            {
                case NPCKnowledgeSystem.NPCRole.Blacksmith:
                case NPCKnowledgeSystem.NPCRole.Weaponsmith:
                case NPCKnowledgeSystem.NPCRole.Armorer:
                    InitializeBlacksmithDomain();
                    break;

                case NPCKnowledgeSystem.NPCRole.Guard:
                    InitializeGuardDomain();
                    break;

                case NPCKnowledgeSystem.NPCRole.Mage:
                    InitializeMageDomain();
                    break;

                case NPCKnowledgeSystem.NPCRole.Scholar:
                    InitializeScholarDomain();
                    break;

                case NPCKnowledgeSystem.NPCRole.Healer:
                    InitializeHealerDomain();
                    break;

                case NPCKnowledgeSystem.NPCRole.Merchant:
                case NPCKnowledgeSystem.NPCRole.Vendor:
                    InitializeMerchantDomain();
                    break;

                case NPCKnowledgeSystem.NPCRole.Innkeeper:
                    InitializeInnkeeperDomain();
                    break;

                case NPCKnowledgeSystem.NPCRole.Ranger:
                    InitializeRangerDomain();
                    break;

                // Vystia faction leaders - all use same domain (political/historical experts)
                case NPCKnowledgeSystem.NPCRole.FactionLeader:
                case NPCKnowledgeSystem.NPCRole.Emperor:
                case NPCKnowledgeSystem.NPCRole.Chieftain:
                case NPCKnowledgeSystem.NPCRole.Elder:
                case NPCKnowledgeSystem.NPCRole.Sultan:
                case NPCKnowledgeSystem.NPCRole.Archmage:
                    InitializeFactionLeaderDomain();
                    break;

                case NPCKnowledgeSystem.NPCRole.Commoner:
                case NPCKnowledgeSystem.NPCRole.Farmer:
                case NPCKnowledgeSystem.NPCRole.Miner:
                default:
                    InitializeCommonerDomain();
                    break;
            }
        }

        #region Domain Initializers

        private void InitializeBlacksmithDomain()
        {
            // EXPERT: Core profession
            DomainExpertise[QuestionCategory.Crafting] = KnowledgeExpertise.Expert;
            DomainExpertise[QuestionCategory.Combat] = KnowledgeExpertise.Proficient; // From customers

            // PROFICIENT: Related knowledge
            DomainExpertise[QuestionCategory.Monsters] = KnowledgeExpertise.Proficient; // From customers
            DomainExpertise[QuestionCategory.Dungeons] = KnowledgeExpertise.Proficient; // From customers
            DomainExpertise[QuestionCategory.Geography] = KnowledgeExpertise.Proficient; // Local area

            // AWARE: General knowledge
            DomainExpertise[QuestionCategory.History] = KnowledgeExpertise.Aware;
            DomainExpertise[QuestionCategory.Trade] = KnowledgeExpertise.Aware;

            // IGNORANT: Out of domain
            DomainExpertise[QuestionCategory.Magic] = KnowledgeExpertise.Ignorant;
            DomainExpertise[QuestionCategory.Healing] = KnowledgeExpertise.Ignorant;
            DomainExpertise[QuestionCategory.Law] = KnowledgeExpertise.Ignorant;
            DomainExpertise[QuestionCategory.Religion] = KnowledgeExpertise.Ignorant;

            // Referral phrases - format: "perhaps there's a local [profession] to be of assistance or you can seek help in [location]"
            ReferralPhrases[QuestionCategory.Magic] = "Perhaps there's a local mage to be of assistance, or you can seek help in Moonglow.";
            ReferralPhrases[QuestionCategory.Healing] = "Perhaps there's a local healer to be of assistance, or you can seek help in Yew.";
            ReferralPhrases[QuestionCategory.Law] = "Perhaps there's a local guard to be of assistance, or you can seek help in Yew.";
            ReferralPhrases[QuestionCategory.Religion] = "Perhaps there's a local scholar to be of assistance, or you can seek help in Skara Brae.";
            ReferralPhrases[QuestionCategory.History] = "Perhaps there's a local scholar to be of assistance, or you can seek help in Moonglow.";
        }

        private void InitializeGuardDomain()
        {
            // EXPERT: Security and threats
            DomainExpertise[QuestionCategory.Combat] = KnowledgeExpertise.Expert;
            DomainExpertise[QuestionCategory.Monsters] = KnowledgeExpertise.Expert;
            DomainExpertise[QuestionCategory.Dungeons] = KnowledgeExpertise.Expert;
            DomainExpertise[QuestionCategory.Law] = KnowledgeExpertise.Expert;

            // PROFICIENT: Local knowledge
            DomainExpertise[QuestionCategory.Geography] = KnowledgeExpertise.Proficient;
            DomainExpertise[QuestionCategory.History] = KnowledgeExpertise.Proficient; // Recent events

            // AWARE: General knowledge
            DomainExpertise[QuestionCategory.Crafting] = KnowledgeExpertise.Aware;
            DomainExpertise[QuestionCategory.Trade] = KnowledgeExpertise.Aware;

            // IGNORANT: Specialized knowledge
            DomainExpertise[QuestionCategory.Magic] = KnowledgeExpertise.Ignorant;
            DomainExpertise[QuestionCategory.Healing] = KnowledgeExpertise.Ignorant;
            DomainExpertise[QuestionCategory.Religion] = KnowledgeExpertise.Ignorant;

            ReferralPhrases[QuestionCategory.Magic] = "Perhaps there's a local mage to be of assistance, or you can seek help in Moonglow.";
            ReferralPhrases[QuestionCategory.Healing] = "Perhaps there's a local healer to be of assistance, or you can seek help in Yew.";
            ReferralPhrases[QuestionCategory.Crafting] = "Perhaps there's a local blacksmith to be of assistance, or you can seek help in Minoc.";
            ReferralPhrases[QuestionCategory.History] = "Perhaps there's a local scholar to be of assistance, or you can seek help in Moonglow.";
        }

        private void InitializeMageDomain()
        {
            // EXPERT: Magical knowledge
            DomainExpertise[QuestionCategory.Magic] = KnowledgeExpertise.Expert;
            DomainExpertise[QuestionCategory.Monsters] = KnowledgeExpertise.Proficient; // Magical creatures

            // PROFICIENT: Academic knowledge
            DomainExpertise[QuestionCategory.History] = KnowledgeExpertise.Proficient;
            DomainExpertise[QuestionCategory.Dungeons] = KnowledgeExpertise.Proficient; // Magic-related dungeons
            DomainExpertise[QuestionCategory.Religion] = KnowledgeExpertise.Proficient; // Philosophical

            // AWARE: Basic knowledge
            DomainExpertise[QuestionCategory.Geography] = KnowledgeExpertise.Aware;
            DomainExpertise[QuestionCategory.Combat] = KnowledgeExpertise.Aware; // Academic only

            // IGNORANT: Physical crafts
            DomainExpertise[QuestionCategory.Crafting] = KnowledgeExpertise.Ignorant;
            DomainExpertise[QuestionCategory.Healing] = KnowledgeExpertise.Ignorant; // Different specialty
            DomainExpertise[QuestionCategory.Law] = KnowledgeExpertise.Ignorant;
            DomainExpertise[QuestionCategory.Trade] = KnowledgeExpertise.Ignorant;

            // Referral phrases - format: "perhaps there's a local [profession] to be of assistance or you can seek help in [location]"
            ReferralPhrases[QuestionCategory.Crafting] = "Perhaps there's a local blacksmith to be of assistance, or you can seek help in Minoc.";
            ReferralPhrases[QuestionCategory.Healing] = "Perhaps there's a local healer to be of assistance, or you can seek help in Yew.";
            ReferralPhrases[QuestionCategory.Law] = "Perhaps there's a local guard to be of assistance, or you can seek help in Yew.";
            ReferralPhrases[QuestionCategory.Combat] = "Perhaps there's a local warrior to be of assistance, or you can seek help in Jhelom.";
        }

        private void InitializeScholarDomain()
        {
            // EXPERT: Academic knowledge
            DomainExpertise[QuestionCategory.History] = KnowledgeExpertise.Expert;
            DomainExpertise[QuestionCategory.Religion] = KnowledgeExpertise.Expert;
            DomainExpertise[QuestionCategory.Geography] = KnowledgeExpertise.Expert;

            // PROFICIENT: Broad knowledge
            DomainExpertise[QuestionCategory.Magic] = KnowledgeExpertise.Proficient; // Academic
            DomainExpertise[QuestionCategory.Crafting] = KnowledgeExpertise.Proficient; // Theoretical
            DomainExpertise[QuestionCategory.Monsters] = KnowledgeExpertise.Proficient; // Taxonomy
            DomainExpertise[QuestionCategory.Dungeons] = KnowledgeExpertise.Proficient; // Documentation
            DomainExpertise[QuestionCategory.Trade] = KnowledgeExpertise.Proficient; // Economic history
            DomainExpertise[QuestionCategory.Law] = KnowledgeExpertise.Proficient; // Legal history

            // AWARE: Practical matters
            DomainExpertise[QuestionCategory.Combat] = KnowledgeExpertise.Aware; // Observed
            DomainExpertise[QuestionCategory.Healing] = KnowledgeExpertise.Aware; // Academic

            // Even scholars defer to practitioners
            ReferralPhrases[QuestionCategory.Crafting] = "Perhaps there's a local blacksmith to be of assistance, or you can seek help in Minoc.";
            ReferralPhrases[QuestionCategory.Combat] = "Perhaps there's a local warrior to be of assistance, or you can seek help in Jhelom.";
        }

        private void InitializeHealerDomain()
        {
            // EXPERT: Medical knowledge
            DomainExpertise[QuestionCategory.Healing] = KnowledgeExpertise.Expert;
            DomainExpertise[QuestionCategory.Nature] = KnowledgeExpertise.Expert; // Herbs

            // PROFICIENT: Related knowledge
            DomainExpertise[QuestionCategory.Monsters] = KnowledgeExpertise.Proficient; // Venoms/poisons
            DomainExpertise[QuestionCategory.Magic] = KnowledgeExpertise.Proficient; // Healing magic

            // AWARE: General knowledge
            DomainExpertise[QuestionCategory.Religion] = KnowledgeExpertise.Aware; // Compassion
            DomainExpertise[QuestionCategory.Geography] = KnowledgeExpertise.Aware;
            DomainExpertise[QuestionCategory.History] = KnowledgeExpertise.Aware;

            // IGNORANT: Out of domain
            DomainExpertise[QuestionCategory.Combat] = KnowledgeExpertise.Ignorant;
            DomainExpertise[QuestionCategory.Crafting] = KnowledgeExpertise.Ignorant;
            DomainExpertise[QuestionCategory.Law] = KnowledgeExpertise.Ignorant;
            DomainExpertise[QuestionCategory.Trade] = KnowledgeExpertise.Ignorant;

            ReferralPhrases[QuestionCategory.Combat] = "Perhaps there's a local warrior to be of assistance, or you can seek help in Jhelom.";
            ReferralPhrases[QuestionCategory.Crafting] = "Perhaps there's a local blacksmith to be of assistance, or you can seek help in Minoc.";
            ReferralPhrases[QuestionCategory.History] = "Perhaps there's a local scholar to be of assistance, or you can seek help in Moonglow.";
        }

        private void InitializeMerchantDomain()
        {
            // EXPERT: Commerce
            DomainExpertise[QuestionCategory.Trade] = KnowledgeExpertise.Expert;

            // PROFICIENT: Business-related
            DomainExpertise[QuestionCategory.Geography] = KnowledgeExpertise.Proficient; // Trade routes
            DomainExpertise[QuestionCategory.Crafting] = KnowledgeExpertise.Proficient; // What they sell

            // AWARE: General knowledge
            DomainExpertise[QuestionCategory.History] = KnowledgeExpertise.Aware;
            DomainExpertise[QuestionCategory.Monsters] = KnowledgeExpertise.Aware; // Travel dangers

            // IGNORANT: Specialized knowledge
            DomainExpertise[QuestionCategory.Magic] = KnowledgeExpertise.Ignorant;
            DomainExpertise[QuestionCategory.Combat] = KnowledgeExpertise.Ignorant;
            DomainExpertise[QuestionCategory.Healing] = KnowledgeExpertise.Ignorant;
            DomainExpertise[QuestionCategory.Law] = KnowledgeExpertise.Ignorant;

            ReferralPhrases[QuestionCategory.Magic] = "Perhaps there's a local mage to be of assistance, or you can seek help in Moonglow.";
            ReferralPhrases[QuestionCategory.Combat] = "Perhaps there's a local warrior to be of assistance, or you can seek help in Jhelom.";
            ReferralPhrases[QuestionCategory.Crafting] = "Perhaps there's a local blacksmith to be of assistance, or you can seek help in Minoc.";
        }

        private void InitializeInnkeeperDomain()
        {
            // EXPERT: Local gossip
            DomainExpertise[QuestionCategory.Geography] = KnowledgeExpertise.Expert; // Local area
            DomainExpertise[QuestionCategory.Trade] = KnowledgeExpertise.Proficient; // Travelers

            // PROFICIENT: Stories and tales
            DomainExpertise[QuestionCategory.History] = KnowledgeExpertise.Proficient; // Stories
            DomainExpertise[QuestionCategory.Monsters] = KnowledgeExpertise.Proficient; // From tales
            DomainExpertise[QuestionCategory.Dungeons] = KnowledgeExpertise.Proficient; // From adventurers

            // AWARE: General knowledge
            DomainExpertise[QuestionCategory.Combat] = KnowledgeExpertise.Aware;
            DomainExpertise[QuestionCategory.Religion] = KnowledgeExpertise.Aware;

            // IGNORANT: Technical knowledge
            DomainExpertise[QuestionCategory.Magic] = KnowledgeExpertise.Ignorant;
            DomainExpertise[QuestionCategory.Crafting] = KnowledgeExpertise.Ignorant;
            DomainExpertise[QuestionCategory.Healing] = KnowledgeExpertise.Ignorant;
            DomainExpertise[QuestionCategory.Law] = KnowledgeExpertise.Ignorant;

            ReferralPhrases[QuestionCategory.Magic] = "Perhaps there's a local mage to be of assistance, or you can seek help in Moonglow.";
            ReferralPhrases[QuestionCategory.Crafting] = "Perhaps there's a local blacksmith to be of assistance, or you can seek help in Minoc.";
            ReferralPhrases[QuestionCategory.History] = "Perhaps there's a local scholar to be of assistance, or you can seek help in Moonglow.";
        }

        private void InitializeRangerDomain()
        {
            // EXPERT: Wilderness knowledge
            DomainExpertise[QuestionCategory.Nature] = KnowledgeExpertise.Expert;
            DomainExpertise[QuestionCategory.Monsters] = KnowledgeExpertise.Expert; // Wilderness creatures
            DomainExpertise[QuestionCategory.Geography] = KnowledgeExpertise.Expert; // Tracking/navigation

            // PROFICIENT: Related skills
            DomainExpertise[QuestionCategory.Combat] = KnowledgeExpertise.Proficient;
            DomainExpertise[QuestionCategory.Dungeons] = KnowledgeExpertise.Proficient; // Exploration

            // AWARE: Basic knowledge
            DomainExpertise[QuestionCategory.History] = KnowledgeExpertise.Aware;
            DomainExpertise[QuestionCategory.Crafting] = KnowledgeExpertise.Aware; // Basic gear

            // IGNORANT: Civilization knowledge
            DomainExpertise[QuestionCategory.Magic] = KnowledgeExpertise.Ignorant;
            DomainExpertise[QuestionCategory.Law] = KnowledgeExpertise.Ignorant;
            DomainExpertise[QuestionCategory.Trade] = KnowledgeExpertise.Ignorant;
            DomainExpertise[QuestionCategory.Religion] = KnowledgeExpertise.Ignorant;

            ReferralPhrases[QuestionCategory.Magic] = "Perhaps there's a local mage to be of assistance, or you can seek help in Moonglow.";
            ReferralPhrases[QuestionCategory.Law] = "Perhaps there's a local guard to be of assistance, or you can seek help in Yew.";
        }

        private void InitializeFactionLeaderDomain()
        {
            // EXPERT: Political, historical, and geographical knowledge
            DomainExpertise[QuestionCategory.History] = KnowledgeExpertise.Expert; // They make history
            DomainExpertise[QuestionCategory.Geography] = KnowledgeExpertise.Expert; // Their territories
            DomainExpertise[QuestionCategory.Law] = KnowledgeExpertise.Expert; // They enforce/create it
            DomainExpertise[QuestionCategory.Trade] = KnowledgeExpertise.Expert; // Economic policy

            // PROFICIENT: Military and magical knowledge
            DomainExpertise[QuestionCategory.Combat] = KnowledgeExpertise.Proficient; // Military strategy
            DomainExpertise[QuestionCategory.Magic] = KnowledgeExpertise.Proficient; // Educated in magic
            DomainExpertise[QuestionCategory.Monsters] = KnowledgeExpertise.Proficient; // Threats to realm
            DomainExpertise[QuestionCategory.Dungeons] = KnowledgeExpertise.Proficient; // Realm knowledge

            // AWARE: General educated knowledge
            DomainExpertise[QuestionCategory.Religion] = KnowledgeExpertise.Aware; // Cultural knowledge
            DomainExpertise[QuestionCategory.Healing] = KnowledgeExpertise.Aware; // General knowledge
            DomainExpertise[QuestionCategory.Crafting] = KnowledgeExpertise.Aware; // Economic oversight
            DomainExpertise[QuestionCategory.Nature] = KnowledgeExpertise.Aware; // Environmental awareness

            // Leaders don't refer people - they ARE the authority
            // Leave referral phrases empty (they know everything or can find out)
        }

        private void InitializeCommonerDomain()
        {
            // PROFICIENT: Their specific trade only
            DomainExpertise[QuestionCategory.Nature] = KnowledgeExpertise.Proficient; // If farmer
            DomainExpertise[QuestionCategory.Crafting] = KnowledgeExpertise.Proficient; // If miner

            // AWARE: Basic general knowledge
            DomainExpertise[QuestionCategory.Geography] = KnowledgeExpertise.Aware; // Local area
            DomainExpertise[QuestionCategory.History] = KnowledgeExpertise.Aware; // Basic
            DomainExpertise[QuestionCategory.Religion] = KnowledgeExpertise.Aware; // Basic virtues

            // IGNORANT: Almost everything else
            DomainExpertise[QuestionCategory.Magic] = KnowledgeExpertise.Ignorant;
            DomainExpertise[QuestionCategory.Combat] = KnowledgeExpertise.Ignorant;
            DomainExpertise[QuestionCategory.Healing] = KnowledgeExpertise.Ignorant;
            DomainExpertise[QuestionCategory.Law] = KnowledgeExpertise.Ignorant;
            DomainExpertise[QuestionCategory.Trade] = KnowledgeExpertise.Ignorant;
            DomainExpertise[QuestionCategory.Monsters] = KnowledgeExpertise.Ignorant;
            DomainExpertise[QuestionCategory.Dungeons] = KnowledgeExpertise.Ignorant;

            ReferralPhrases[QuestionCategory.Magic] = "Perhaps there's a local mage to be of assistance, or you can seek help in Moonglow.";
            ReferralPhrases[QuestionCategory.History] = "Perhaps there's a local scholar to be of assistance, or you can seek help in Moonglow.";
            ReferralPhrases[QuestionCategory.Combat] = "Perhaps there's a local warrior to be of assistance, or you can seek help in Jhelom.";
            ReferralPhrases[QuestionCategory.Law] = "Perhaps there's a local guard to be of assistance, or you can seek help in Yew.";
        }

        #endregion

        /// <summary>
        /// Get expertise level for a specific question category
        /// </summary>
        public KnowledgeExpertise GetExpertise(QuestionCategory category)
        {
            if (DomainExpertise.ContainsKey(category))
                return DomainExpertise[category];

            // Default to Aware if not specified
            return KnowledgeExpertise.Aware;
        }

        /// <summary>
        /// Get referral phrase for a category this role is ignorant about
        /// </summary>
        public string GetReferralPhrase(QuestionCategory category)
        {
            if (ReferralPhrases.ContainsKey(category))
                return ReferralPhrases[category];

            // Generic fallback
            return "I'm not sure who to direct ye to - perhaps ask around town?";
        }

        /// <summary>
        /// Check if this role should answer a question or defer
        /// </summary>
        public bool ShouldAnswer(QuestionCategory category)
        {
            var expertise = GetExpertise(category);
            return expertise != KnowledgeExpertise.Ignorant;
        }
    }
}
