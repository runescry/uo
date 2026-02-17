using System;
using System.Collections.Generic;
using Server;
using Server.Commands;
using Server.Mobiles;

namespace Server.Scripts.Commands
{
    /// <summary>
    /// GM command to spawn Vystia class trainers
    /// </summary>
    public class SpawnClassTrainersCommand
    {
        // All 25 class trainers
        private static readonly Dictionary<string, Type> ClassTrainers = new Dictionary<string, Type>
        {
            // Frosthold (3)
            ["Barbarian"] = typeof(BarbarianTrainer),
            ["Beastmaster"] = typeof(BeastmasterTrainer),
            ["IceMage"] = typeof(IceMageTrainer),

            // Emberlands (1)
            ["Sorcerer"] = typeof(SorcererTrainer),

            // Desert (2)
            ["Ranger"] = typeof(RangerTrainer),
            ["Illusionist"] = typeof(IllusionistTrainer),

            // Shadowfen (1)
            ["Witch"] = typeof(WitchTrainer),

            // ShadowVoid (2)
            ["Warlock"] = typeof(WarlockTrainer),
            ["Necromancer"] = typeof(NecromancerTrainer),

            // Verdantpeak (2)
            ["Druid"] = typeof(DruidTrainer),
            ["Alchemist"] = typeof(AlchemistTrainer),

            // Crystal Barrens (2)
            ["Oracle"] = typeof(OracleTrainer),
            ["Wizard"] = typeof(WizardTrainer),

            // Ironclad (4)
            ["Artificer"] = typeof(ArtificerTrainer),
            ["Fighter"] = typeof(FighterTrainer),
            ["Monk"] = typeof(MonkTrainer),
            ["Templar"] = typeof(TemplarTrainer),

            // Underwater (1)
            ["Summoner"] = typeof(SummonerTrainer),

            // Multi-Regional (7)
            ["BountyHunter"] = typeof(BountyHunterTrainer),
            ["Knight"] = typeof(KnightTrainer),
            ["Shaman"] = typeof(ShamanTrainer),
            ["Cleric"] = typeof(ClericTrainer),
            ["Paladin"] = typeof(PaladinTrainer),
            ["Bard"] = typeof(BardTrainer),
            ["Enchanter"] = typeof(EnchanterTrainer)
        };

        public static void Initialize()
        {
            CommandSystem.Register("spawntrainer", AccessLevel.GameMaster, SpawnTrainer_OnCommand);
            CommandSystem.Register("spawnalltrainers", AccessLevel.GameMaster, SpawnAllTrainers_OnCommand);
            CommandSystem.Register("str", AccessLevel.GameMaster, SpawnTrainer_OnCommand); // Shortcut
            CommandSystem.Register("sat", AccessLevel.GameMaster, SpawnAllTrainers_OnCommand); // Shortcut
        }

        [Usage("spawntrainer <classname>")]
        [Description("Spawns a specific class trainer at your location. Example: [spawntrainer IceMage")]
        private static void SpawnTrainer_OnCommand(CommandEventArgs e)
        {
            Mobile from = e.Mobile;

            if (e.Arguments.Length == 0)
            {
                from.SendMessage("Usage: [spawntrainer <classname>");
                from.SendMessage("Available classes: " + string.Join(", ", ClassTrainers.Keys));
                return;
            }

            string className = e.Arguments[0];

            // Find matching class (case-insensitive)
            Type trainerType = null;
            string matchedClass = null;

            foreach (var kvp in ClassTrainers)
            {
                if (kvp.Key.Equals(className, StringComparison.OrdinalIgnoreCase))
                {
                    trainerType = kvp.Value;
                    matchedClass = kvp.Key;
                    break;
                }
            }

            if (trainerType == null)
            {
                from.SendMessage($"Unknown class: {className}");
                from.SendMessage("Available classes: " + string.Join(", ", ClassTrainers.Keys));
                return;
            }

            try
            {
                Mobile trainer = (Mobile)Activator.CreateInstance(trainerType);
                if (trainer != null)
                {
                    trainer.MoveToWorld(from.Location, from.Map);
                    from.SendMessage($"Spawned {matchedClass} Trainer at your location.");
                }
            }
            catch (Exception ex)
            {
                from.SendMessage($"Failed to spawn trainer: {ex.Message}");
            }
        }

        [Usage("spawnalltrainers")]
        [Description("Spawns all 25 class trainers in a circle around you")]
        private static void SpawnAllTrainers_OnCommand(CommandEventArgs e)
        {
            Mobile from = e.Mobile;
            Point3D center = from.Location;
            Map map = from.Map;

            int spawned = 0;
            int radius = 8;
            int index = 0;
            int total = ClassTrainers.Count;

            foreach (var kvp in ClassTrainers)
            {
                try
                {
                    Mobile trainer = (Mobile)Activator.CreateInstance(kvp.Value);
                    if (trainer != null)
                    {
                        // Calculate circular position
                        double angle = (2 * Math.PI * index) / total;
                        int x = center.X + (int)(radius * Math.Cos(angle));
                        int y = center.Y + (int)(radius * Math.Sin(angle));
                        int z = map.GetAverageZ(x, y);

                        Point3D loc = new Point3D(x, y, z);

                        // Try to find a valid spawn location
                        if (!map.CanSpawnMobile(loc))
                        {
                            loc = center; // Fall back to center
                        }

                        trainer.MoveToWorld(loc, map);
                        spawned++;
                    }
                }
                catch
                {
                    // Silent fail for individual trainers
                }

                index++;
            }

            from.SendMessage($"Spawned {spawned}/{total} class trainers in a circle around you.");
        }
    }
}
