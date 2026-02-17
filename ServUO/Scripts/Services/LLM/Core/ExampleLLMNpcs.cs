using System;
using Server;
using Server.Commands;
using Server.Mobiles;

namespace Server.Services.LLM
{
    /// <summary>
    /// Example LLM NPCs with different personalities
    /// Use [SpawnLLMNpcs to create example NPCs at your location
    /// </summary>
    public class ExampleLLMNpcs
    {
        public static void Initialize()
        {
            CommandSystem.Register("SpawnLLMNpcs", AccessLevel.GameMaster, SpawnLLMNpcs_OnCommand);
            CommandSystem.Register("SpawnLLMNpc", AccessLevel.GameMaster, SpawnLLMNpc_OnCommand);
        }

        [Usage("SpawnLLMNpcs")]
        [Description("Spawns several example LLM NPCs with different personalities near your location.")]
        private static void SpawnLLMNpcs_OnCommand(CommandEventArgs e)
        {
            Mobile from = e.Mobile;
            Map map = from.Map;

            if (map == null || map == Map.Internal)
            {
                from.SendMessage("You must be in a valid location to spawn NPCs.");
                return;
            }

            Point3D baseLocation = from.Location;

            // Spawn different personality types
            SpawnNpc(from, map, baseLocation, 2, 2,
                "Eldrin the Sage",
                "You are a wise old sage who has studied magic and history for centuries. You speak formally and enjoy sharing knowledge about the arcane arts and ancient lore.");

            SpawnNpc(from, map, baseLocation, -2, 2,
                "Mira the Merchant",
                "You are a cheerful traveling merchant who loves to gossip about current events and share tales from your travels. You're always looking for a good deal and speak in a friendly, casual manner.");

            SpawnNpc(from, map, baseLocation, 2, -2,
                "Grimbold the Guard",
                "You are a gruff city guard who takes your duty seriously. You're suspicious of strangers but warm up to those who prove themselves trustworthy. You speak plainly and directly.");

            SpawnNpc(from, map, baseLocation, -2, -2,
                "Luna the Mystic",
                "You are a mysterious fortune teller who speaks in riddles and cryptic prophecies. You claim to see the threads of fate and often reference the stars, dreams, and omens.");

            SpawnNpc(from, map, baseLocation, 0, -4,
                "Pip the Jester",
                "You are a playful jester who loves jokes, puns, and wordplay. You're lighthearted and try to make everyone laugh, but you're also clever and sometimes hide wisdom in your humor.");

            from.SendMessage("Spawned 5 example LLM NPCs around you!");
            from.SendMessage("Try talking to them by typing in their presence!");
            from.SendMessage("Right-click them and select 'Reset' to clear conversation history.");
        }

        [Usage("SpawnLLMNpc <name> <personality>")]
        [Description("Spawns a custom LLM NPC with the specified name and personality.")]
        private static void SpawnLLMNpc_OnCommand(CommandEventArgs e)
        {
            Mobile from = e.Mobile;

            if (e.Length < 2)
            {
                from.SendMessage("Usage: [SpawnLLMNpc <name> <personality>");
                from.SendMessage("Example: [SpawnLLMNpc \"Bob the Blacksmith\" \"You are a skilled blacksmith who loves talking about weapons and armor.\"");
                return;
            }

            string name = e.GetString(0);
            string personality = e.Arguments.Length > 1 ? string.Join(" ", e.Arguments, 1, e.Arguments.Length - 1) : "You are a helpful NPC.";

            Map map = from.Map;
            if (map == null || map == Map.Internal)
            {
                from.SendMessage("You must be in a valid location to spawn an NPC.");
                return;
            }

            SpawnNpc(from, map, from.Location, 0, 0, name, personality);
            from.SendMessage($"Spawned {name} at your location!");
        }

        private static void SpawnNpc(Mobile from, Map map, Point3D baseLocation, int offsetX, int offsetY, string name, string personality)
        {
            // Get proper ground Z level at spawn location
            int z = map.GetAverageZ(baseLocation.X + offsetX, baseLocation.Y + offsetY);
            Point3D location = new Point3D(baseLocation.X + offsetX, baseLocation.Y + offsetY, z);

            LLMNpc npc = new LLMNpc(name, personality);
            npc.MoveToWorld(location, map);

            // Randomize appearance
            if (Utility.RandomBool())
            {
                npc.Body = 0x191; // Female body
            }

            from.SendMessage($"Spawned: {name} at {location}");
        }
    }
}
