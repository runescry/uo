using System;
using Server;
using Server.Commands;
using Server.Mobiles;

namespace Server.Commands
{
    public class SpawnAncientLineCommand
    {
        public static void Initialize()
        {
            CommandSystem.Register("SpawnAncientLine", AccessLevel.GameMaster, new CommandEventHandler(SpawnAncientLine_OnCommand));
            CommandSystem.Register("sal", AccessLevel.GameMaster, new CommandEventHandler(SpawnAncientLine_OnCommand));
        }

        [Usage("SpawnAncientLine [spacing]")]
        [Aliases("sal")]
        [Description("Spawns all 12 ancient creatures in a line, frozen for display. Optional spacing parameter (default: 2 tiles).")]
        public static void SpawnAncientLine_OnCommand(CommandEventArgs e)
        {
            Mobile from = e.Mobile;
            int spacing = 2; // Default spacing between creatures

            if (e.Arguments.Length > 0)
            {
                if (!int.TryParse(e.Arguments[0], out spacing) || spacing < 1 || spacing > 10)
                {
                    from.SendMessage("Invalid spacing. Using default of 2 tiles.");
                    spacing = 2;
                }
            }

            Point3D start = from.Location;
            Map map = from.Map;

            // Array of all 12 ancient creatures
            Type[] ancientCreatures = new Type[]
            {
                typeof(FrosthelmEternalWinter),
                typeof(ElderOakbark),
                typeof(EmberflameAshenTyrant),
                typeof(VerdantheartForestGuardian),
                typeof(CrystalwingPrismaticOracle),
                typeof(AbyssusDepthKing),
                typeof(IronbarkWarAncient),
                typeof(TheCrystalSphinx),
                typeof(FrostFatherAvatar),
                typeof(GreatMachinistConstruct),
                typeof(LunaraDryadHerald),
                typeof(SphynxOfEmberlands)
            };

            from.SendMessage($"Spawning {ancientCreatures.Length} ancient creatures in a line with {spacing} tile spacing...");

            int successCount = 0;
            int x = start.X;
            int y = start.Y;
            int z = start.Z;

            for (int i = 0; i < ancientCreatures.Length; i++)
            {
                try
                {
                    // Create the creature
                    Mobile creature = (Mobile)Activator.CreateInstance(ancientCreatures[i]);

                    if (creature != null)
                    {
                        // Calculate position (line extends east)
                        Point3D spawnLoc = new Point3D(x + (i * spacing), y, z);

                        // Spawn the creature
                        creature.MoveToWorld(spawnLoc, map);

                        // Freeze the creature (unable to move)
                        creature.Frozen = true;
                        creature.CantWalk = true;

                        // Make them non-aggressive for display
                        if (creature is BaseCreature bc)
                        {
                            bc.Tamable = false;
                            bc.AI = AIType.AI_Melee; // Passive AI
                            bc.FightMode = FightMode.None; // Won't attack
                        }

                        successCount++;
                    }
                }
                catch (Exception ex)
                {
                    from.SendMessage($"Error spawning {ancientCreatures[i].Name}: {ex.Message}");
                }
            }

            from.SendMessage($"Spawned {successCount}/{ancientCreatures.Length} ancient creatures.");
            from.SendMessage("All creatures are frozen and set to non-aggressive display mode.");
            from.SendMessage("Use [remove to delete them or [props to unfreeze individual creatures.");
        }
    }
}
