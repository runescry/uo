using System;
using System.Collections.Generic;
using Server.Mobiles;

namespace Server.Services.LLM
{
    /// <summary>
    /// Maps NPC class types to their specific personality types
    /// </summary>
    public static class NPCPersonalityMapper
    {
        /// <summary>
        /// Gets the personality type for an NPC based on its class type
        /// </summary>
        public static NPCPersonalities.PersonalityType GetPersonalityForNPC(Mobile npc)
        {
            if (npc == null)
                return NPCPersonalities.PersonalityType.Commoner;

            Type npcType = npc.GetType();

            // Check direct class mapping first
            if (NPCPersonalityDatabase.ClassToPersonalityMap.TryGetValue(npcType, out NPCPersonalities.PersonalityType personality))
            {
                return personality;
            }

            // Check base class mappings
            Type currentType = npcType.BaseType;
            while (currentType != null && currentType != typeof(object))
            {
                if (NPCPersonalityDatabase.ClassToPersonalityMap.TryGetValue(currentType, out personality))
                {
                    return personality;
                }
                currentType = currentType.BaseType;
            }

            // Fallback: Use name/title-based suggestion
            string nameOrTitle = npc.Name ?? npc.GetType().Name;
            if (!string.IsNullOrEmpty(npc.Title))
            {
                nameOrTitle = $"{nameOrTitle} {npc.Title}";
            }

            return NPCPersonalities.SuggestPersonality(nameOrTitle);
        }

        /// <summary>
        /// Gets the suggested speech pattern for an NPC based on its personality type
        /// </summary>
        public static NPCPersonalities.SpeechPattern GetSpeechPatternForPersonality(NPCPersonalities.PersonalityType personality)
        {
            switch (personality)
            {
                case NPCPersonalities.PersonalityType.Noble:
                case NPCPersonalities.PersonalityType.Guard:
                case NPCPersonalities.PersonalityType.Banker:
                case NPCPersonalities.PersonalityType.Scribe:
                    return NPCPersonalities.SpeechPattern.Formal;

                case NPCPersonalities.PersonalityType.Sage:
                case NPCPersonalities.PersonalityType.Gypsy:
                case NPCPersonalities.PersonalityType.Hermit:
                case NPCPersonalities.PersonalityType.Villain:
                    return NPCPersonalities.SpeechPattern.Cryptic;

                case NPCPersonalities.PersonalityType.Mage:
                case NPCPersonalities.PersonalityType.Paladin:
                case NPCPersonalities.PersonalityType.Samurai:
                case NPCPersonalities.PersonalityType.Ninja:
                case NPCPersonalities.PersonalityType.Monk:
                    return NPCPersonalities.SpeechPattern.Archaic;

                case NPCPersonalities.PersonalityType.Actor:
                case NPCPersonalities.PersonalityType.Bard:
                case NPCPersonalities.PersonalityType.Barkeeper:
                case NPCPersonalities.PersonalityType.TownCrier:
                    return NPCPersonalities.SpeechPattern.Casual;

                default:
                    return NPCPersonalities.SpeechPattern.Modern;
            }
        }
    }
}

