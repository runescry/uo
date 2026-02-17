using System;
using Server;
using Server.Commands;
using Server.Items;

namespace Server.Scripts.Commands
{
    public class SpawnReagentBagsCommand
    {
        public static void Initialize()
        {
            CommandSystem.Register("spawnreagentbags", AccessLevel.GameMaster, new CommandEventHandler(SpawnReagentBags_OnCommand));
            CommandSystem.Register("srb", AccessLevel.GameMaster, new CommandEventHandler(SpawnReagentBags_OnCommand));
        }

        [Usage("SpawnReagentBags")]
        [Aliases("srb")]
        [Description("Spawns all 12 Vystia reagent bags in a line to the east, 1 tile apart.")]
        public static void SpawnReagentBags_OnCommand(CommandEventArgs e)
        {
            Mobile from = e.Mobile;

            if (from == null || from.Map == null)
                return;

            // Get starting position (1 tile east of player)
            int startX = from.X + 1;
            int startY = from.Y;
            int z = from.Z;
            Map map = from.Map;

            // All 12 reagent bag types in order
            Type[] bagTypes = new Type[]
            {
                typeof(IceMagicReagentBag),
                typeof(NatureMagicReagentBag),
                typeof(HexMagicReagentBag),
                typeof(ElementalMagicReagentBag),
                typeof(DarkMagicReagentBag),
                typeof(DivinationMagicReagentBag),
                typeof(NecromancyReagentBag),
                typeof(SummoningMagicReagentBag),
                typeof(ShamanicMagicReagentBag),
                typeof(SongweavingReagentBag),
                typeof(EnchantingMagicReagentBag),
                typeof(IllusionMagicReagentBag)
            };

            int spawned = 0;

            for (int i = 0; i < bagTypes.Length; i++)
            {
                try
                {
                    // Create bag
                    Item bag = (Item)Activator.CreateInstance(bagTypes[i]);

                    if (bag != null)
                    {
                        // Place 1 tile east per bag (X increases by 1 each time)
                        Point3D loc = new Point3D(startX + i, startY, z);

                        // Check if location is valid
                        if (map.CanFit(loc, 16, false, false))
                        {
                            bag.MoveToWorld(loc, map);
                            spawned++;
                        }
                        else
                        {
                            // Try to find a valid Z
                            loc.Z = map.GetAverageZ(loc.X, loc.Y);
                            bag.MoveToWorld(loc, map);
                            spawned++;
                        }
                    }
                }
                catch (Exception ex)
                {
                    from.SendMessage(0x22, $"Error spawning bag {bagTypes[i].Name}: {ex.Message}");
                }
            }

            from.SendMessage(0x35, $"Spawned {spawned}/12 reagent bags in a line to the east.");
        }
    }
}
