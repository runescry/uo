using System;
using Server;
using Server.Commands;
using Server.Mobiles;
using Server.Custom.VystiaClasses.Economy;

namespace Server.Custom.Commands
{
    public static class VystiaRepairCommand
    {
        public static void Initialize()
        {
            CommandSystem.Register("RepairCost", AccessLevel.Player, new CommandEventHandler(RepairCost_OnCommand));
            CommandSystem.Register("SpawnBlacksmith", AccessLevel.GameMaster, new CommandEventHandler(SpawnBlacksmith_OnCommand));
        }

        /// <summary>
        /// Check repair cost for equipped/backpack items
        /// Usage: [RepairCost
        /// </summary>
        [Usage("RepairCost")]
        [Description("Shows repair costs for all damaged equipment.")]
        private static void RepairCost_OnCommand(CommandEventArgs e)
        {
            PlayerMobile pm = e.Mobile as PlayerMobile;
            if (pm == null)
                return;

            int totalCost = 0;
            int damagedCount = 0;

            pm.SendMessage(0x35, "=== Repair Cost Estimate ===");

            // Check backpack
            if (pm.Backpack != null)
            {
                foreach (Item item in pm.Backpack.Items)
                {
                    int cost = VystiaRepairService.CalculateRepairCost(item);
                    if (cost > 0)
                    {
                        var tier = VystiaRepairService.GetRepairTier(item);
                        pm.SendMessage("  {0}: {1:N0}g ({2})", item.Name ?? item.GetType().Name, cost, VystiaRepairService.GetTierName(tier));
                        totalCost += cost;
                        damagedCount++;
                    }
                }
            }

            // Check equipped items
            foreach (Item item in pm.Items)
            {
                int cost = VystiaRepairService.CalculateRepairCost(item);
                if (cost > 0)
                {
                    var tier = VystiaRepairService.GetRepairTier(item);
                    pm.SendMessage("  {0}: {1:N0}g ({2})", item.Name ?? item.GetType().Name, cost, VystiaRepairService.GetTierName(tier));
                    totalCost += cost;
                    damagedCount++;
                }
            }

            if (damagedCount == 0)
            {
                pm.SendMessage(0x35, "All your equipment is in perfect condition!");
            }
            else
            {
                pm.SendMessage(0x35, "Total: {0} items, {1:N0}g to repair all.", damagedCount, totalCost);
            }
        }

        /// <summary>
        /// Spawn a Vystia blacksmith with repair service
        /// Usage: [SpawnBlacksmith
        /// </summary>
        [Usage("SpawnBlacksmith")]
        [Description("Spawns a Vystia blacksmith with repair service at your location.")]
        private static void SpawnBlacksmith_OnCommand(CommandEventArgs e)
        {
            Mobile from = e.Mobile;

            VystiaBlacksmith smith = new VystiaBlacksmith();
            smith.MoveToWorld(from.Location, from.Map);

            from.SendMessage("Spawned Vystia Blacksmith at your location. Say 'repair' to test.");
        }
    }
}
