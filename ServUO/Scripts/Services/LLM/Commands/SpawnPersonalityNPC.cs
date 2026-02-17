using System;
using Server;
using Server.Commands;
using Server.Mobiles;

namespace Server.Services.LLM
{
    /// <summary>
    /// Command to spawn NPCs with new personality system
    /// </summary>
    public class SpawnPersonalityNPCCommand
    {
        public static void Initialize()
        {
            CommandSystem.Register("SpawnPersonalityNPC", AccessLevel.GameMaster, SpawnNPC_OnCommand);
        }

        [Usage("SpawnPersonalityNPC [personality] [speech]")]
        [Description("Spawns an LLM NPC with specified personality and speech pattern. Omit arguments to see options.")]
        private static void SpawnNPC_OnCommand(CommandEventArgs e)
        {
            Mobile from = e.Mobile;

            if (e.Length < 2)
            {
                from.SendMessage("=== Spawn Personality NPC ===");
                from.SendMessage("Usage: [SpawnPersonalityNPC <personality> <speech>");
                from.SendMessage("");
                from.SendMessage("Personalities: Merchant, Guard, Noble, Sage, Commoner, Villain, Hermit, Healer, Warrior, Mage");
                from.SendMessage("Speech Patterns: Modern, Formal, OldEnglish, Cryptic, Casual, Archaic");
                from.SendMessage("");
                from.SendMessage("Example: [SpawnPersonalityNPC Sage OldEnglish");
                from.SendMessage("Example: [SpawnPersonalityNPC Guard Formal");
                return;
            }

            string personalityStr = e.GetString(0);
            string speechStr = e.GetString(1);

            // Parse personality type
            NPCPersonalities.PersonalityType personality;
            if (!Enum.TryParse(personalityStr, true, out personality))
            {
                from.SendMessage($"Invalid personality: {personalityStr}");
                from.SendMessage("Valid: Merchant, Guard, Noble, Sage, Commoner, Villain, Hermit, Healer, Warrior, Mage");
                return;
            }

            // Parse speech pattern
            NPCPersonalities.SpeechPattern speech;
            if (!Enum.TryParse(speechStr, true, out speech))
            {
                from.SendMessage($"Invalid speech pattern: {speechStr}");
                from.SendMessage("Valid: Modern, Formal, OldEnglish, Cryptic, Casual, Archaic");
                return;
            }

            // Generate name based on personality
            string name = GenerateName(personality);

            // Spawn the NPC
            LLMNpc npc = new LLMNpc(name, personality, speech);

            // Get proper ground Z level at spawn location
            Point3D loc = from.Location;
            int z = from.Map.GetAverageZ(loc.X, loc.Y);
            Point3D spawnLoc = new Point3D(loc.X, loc.Y, z);

            npc.MoveToWorld(spawnLoc, from.Map);

            from.SendMessage($"Spawned {name} ({personality} / {speech}) at {spawnLoc}");
        }

        private static string GenerateName(NPCPersonalities.PersonalityType personality)
        {
            string[] firstNames = { "Aldric", "Beatrix", "Cedric", "Daphne", "Edmund", "Fiona", "Gareth", "Helena", "Ivor", "Jocelyn" };
            string firstName = firstNames[Utility.Random(firstNames.Length)];

            switch (personality)
            {
                case NPCPersonalities.PersonalityType.Merchant:
                    return $"{firstName} the Merchant";
                case NPCPersonalities.PersonalityType.Guard:
                    return $"Captain {firstName}";
                case NPCPersonalities.PersonalityType.Noble:
                    return $"Lord {firstName}";
                case NPCPersonalities.PersonalityType.Sage:
                    return $"{firstName} the Sage";
                case NPCPersonalities.PersonalityType.Commoner:
                    return firstName;
                case NPCPersonalities.PersonalityType.Villain:
                    return $"{firstName} the Dark";
                case NPCPersonalities.PersonalityType.Hermit:
                    return $"{firstName} the Hermit";
                case NPCPersonalities.PersonalityType.Healer:
                    return $"{firstName} the Healer";
                case NPCPersonalities.PersonalityType.Warrior:
                    return $"{firstName} the Warrior";
                case NPCPersonalities.PersonalityType.Mage:
                    return $"{firstName} the Mage";
                default:
                    return firstName;
            }
        }
    }
}
