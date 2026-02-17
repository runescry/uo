using System;
using Server;
using Server.Commands;
using Server.Mobiles;
using Server.Custom.VystiaClasses.Economy;

namespace Server.Custom.Commands
{
    public static class VystiaServiceNPCCommands
    {
        public static void Initialize()
        {
            CommandSystem.Register("SpawnHealer", AccessLevel.GameMaster, new CommandEventHandler(SpawnHealer_OnCommand));
            CommandSystem.Register("SpawnMoongateAttendant", AccessLevel.GameMaster, new CommandEventHandler(SpawnMoongateAttendant_OnCommand));
            CommandSystem.Register("ServiceFees", AccessLevel.Player, new CommandEventHandler(ServiceFees_OnCommand));
        }

        /// <summary>
        /// Spawn a Vystia healer with resurrection service
        /// Usage: [SpawnHealer
        /// </summary>
        [Usage("SpawnHealer")]
        [Description("Spawns a Vystia healer with resurrection service at your location.")]
        private static void SpawnHealer_OnCommand(CommandEventArgs e)
        {
            Mobile from = e.Mobile;

            VystiaHealer healer = new VystiaHealer();
            healer.MoveToWorld(from.Location, from.Map);

            from.SendMessage("Spawned Vystia Healer at your location. Say 'resurrect' when dead to test.");
        }

        /// <summary>
        /// Spawn a Vystia moongate attendant
        /// Usage: [SpawnMoongateAttendant
        /// </summary>
        [Usage("SpawnMoongateAttendant")]
        [Description("Spawns a Vystia moongate attendant at your location.")]
        private static void SpawnMoongateAttendant_OnCommand(CommandEventArgs e)
        {
            Mobile from = e.Mobile;

            VystiaMoongateAttendant attendant = new VystiaMoongateAttendant();
            attendant.MoveToWorld(from.Location, from.Map);

            from.SendMessage("Spawned Vystia Moongate Attendant at your location.");
        }

        /// <summary>
        /// Show current service fee rates
        /// Usage: [ServiceFees
        /// </summary>
        [Usage("ServiceFees")]
        [Description("Shows current Vystia service fee rates.")]
        private static void ServiceFees_OnCommand(CommandEventArgs e)
        {
            Mobile from = e.Mobile;

            from.SendMessage(0x35, "=== Vystia Service Fees ===");
            from.SendMessage("Resurrection (base): {0:N0}g + {1:N0}g per fame level",
                VystiaServiceFees.BaseResurrectionCost, VystiaServiceFees.CostPerFameLevel);

            int myResCost = VystiaServiceFees.CalculateResurrectionCost(from);
            from.SendMessage("  Your resurrection cost: {0:N0}g", myResCost);

            from.SendMessage("Travel Fees:");
            from.SendMessage("  Short distance: {0:N0}g", VystiaServiceFees.ShortDistanceTravelCost);
            from.SendMessage("  Medium distance: {0:N0}g", VystiaServiceFees.MediumDistanceTravelCost);
            from.SendMessage("  Long distance: {0:N0}g", VystiaServiceFees.LongDistanceTravelCost);

            from.SendMessage("Stabling:");
            from.SendMessage("  Daily: {0:N0}g per pet", VystiaServiceFees.DailyStablingCost);
            from.SendMessage("  Weekly: {0:N0}g per pet", VystiaServiceFees.WeeklyStablingCost);
        }
    }
}
