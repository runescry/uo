using System;
using Server;
using Server.Commands;
using Server.Mobiles;
using Server.Targeting;

namespace Server.Services.LLM
{
    /// <summary>
    /// Commands to spawn groups of NPCs
    /// </summary>
    public class SpawnNPCGroups
    {
        public static void Initialize()
        {
            CommandSystem.Register("SpawnTownNPCs", AccessLevel.GameMaster, SpawnTownNPCs_OnCommand);
            CommandSystem.Register("SpawnMagicNPCs", AccessLevel.GameMaster, SpawnMagicNPCs_OnCommand);
            CommandSystem.Register("SpawnAdventurerNPCs", AccessLevel.GameMaster, SpawnAdventurerNPCs_OnCommand);
        }

        [Usage("SpawnTownNPCs")]
        [Description("Spawns a set of town NPCs at targeted locations")]
        private static void SpawnTownNPCs_OnCommand(CommandEventArgs e)
        {
            Mobile from = e.Mobile;
            from.SendMessage("Spawning town NPCs...");

            from.Target = new InternalTarget((mobile, point, map) =>
            {
                SpawnNPC("Blacksmith", NPCPersonalities.PersonalityType.Merchant, point, map, mobile);
                SpawnNPC("Town Guard", NPCPersonalities.PersonalityType.Guard, new Point3D(point.X + 2, point.Y, point.Z), map, mobile);
                SpawnNPC("Herbalist", NPCPersonalities.PersonalityType.Healer, new Point3D(point.X + 4, point.Y, point.Z), map, mobile);
                SpawnNPC("Tavernkeeper", NPCPersonalities.PersonalityType.Commoner, new Point3D(point.X, point.Y + 2, point.Z), map, mobile);
                SpawnNPC("Librarian", NPCPersonalities.PersonalityType.Sage, new Point3D(point.X + 2, point.Y + 2, point.Z), map, mobile);
                mobile.SendMessage("Town NPCs spawned!");
            });
            from.SendMessage("Target the location to spawn town NPCs.");
        }

        [Usage("SpawnMagicNPCs")]
        [Description("Spawns a set of magic NPCs at targeted locations")]
        private static void SpawnMagicNPCs_OnCommand(CommandEventArgs e)
        {
            Mobile from = e.Mobile;
            from.SendMessage("Spawning magic NPCs...");

            from.Target = new InternalTarget((mobile, point, map) =>
            {
                SpawnNPC("Archmage", NPCPersonalities.PersonalityType.Mage, point, map, mobile);
                SpawnNPC("Mystic", NPCPersonalities.PersonalityType.Sage, new Point3D(point.X + 2, point.Y, point.Z), map, mobile);
                SpawnNPC("Necromancer", NPCPersonalities.PersonalityType.Villain, new Point3D(point.X + 4, point.Y, point.Z), map, mobile);
                SpawnNPC("Druid", NPCPersonalities.PersonalityType.Hermit, new Point3D(point.X, point.Y + 2, point.Z), map, mobile);
                mobile.SendMessage("Magic NPCs spawned!");
            });
            from.SendMessage("Target the location to spawn magic NPCs.");
        }

        [Usage("SpawnAdventurerNPCs")]
        [Description("Spawns a set of adventurer NPCs at targeted locations")]
        private static void SpawnAdventurerNPCs_OnCommand(CommandEventArgs e)
        {
            Mobile from = e.Mobile;
            from.SendMessage("Spawning adventurer NPCs...");

            from.Target = new InternalTarget((mobile, point, map) =>
            {
                SpawnNPC("Veteran Warrior", NPCPersonalities.PersonalityType.Warrior, point, map, mobile);
                SpawnNPC("Treasure Hunter", NPCPersonalities.PersonalityType.Commoner, new Point3D(point.X + 2, point.Y, point.Z), map, mobile);
                SpawnNPC("Bard", NPCPersonalities.PersonalityType.Commoner, new Point3D(point.X + 4, point.Y, point.Z), map, mobile);
                SpawnNPC("Ranger", NPCPersonalities.PersonalityType.Warrior, new Point3D(point.X, point.Y + 2, point.Z), map, mobile);
                mobile.SendMessage("Adventurer NPCs spawned!");
            });
            from.SendMessage("Target the location to spawn adventurer NPCs.");
        }

        private static void SpawnNPC(string title, NPCPersonalities.PersonalityType personality, Point3D location, Map map, Mobile from)
        {
            string[] firstNames = { "Aldric", "Beatrix", "Cedric", "Daphne", "Edmund", "Fiona", "Gareth", "Helena", "Ivor", "Jocelyn" };
            string firstName = firstNames[Utility.Random(firstNames.Length)];
            string fullName = $"{firstName} the {title}";

            // Get proper ground Z level at spawn location
            int z = map.GetAverageZ(location.X, location.Y);
            Point3D spawnLoc = new Point3D(location.X, location.Y, z);

            LLMNpc npc = new LLMNpc(fullName, personality, NPCPersonalities.SpeechPattern.Archaic);
            npc.MoveToWorld(spawnLoc, map);

            from.SendMessage($"Spawned: {fullName} at {spawnLoc}");
        }

        private class InternalTarget : Target
        {
            private Action<Mobile, Point3D, Map> m_Callback;

            public InternalTarget(Action<Mobile, Point3D, Map> callback) : base(12, true, TargetFlags.None)
            {
                m_Callback = callback;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                IPoint3D p = targeted as IPoint3D;
                if (p == null)
                {
                    from.SendMessage("Invalid target.");
                    return;
                }

                Point3D loc = new Point3D(p);
                Map map = from.Map;

                if (map == null || map == Map.Internal)
                {
                    from.SendMessage("Invalid location.");
                    return;
                }

                m_Callback(from, loc, map);
            }
        }
    }
}
