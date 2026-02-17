using System;
using Server;
using Server.Mobiles;

namespace Server.Custom.VystiaClasses
{
    /// <summary>
    /// Utility class to apply Vystia class selection to players
    /// </summary>
    public static class VystiaClassApplicator
    {
        /// <summary>
        /// Apply a Vystia class to a player
        /// </summary>
        public static void ApplyClass(PlayerMobile pm, PlayerClassTypeV2 classType)
        {
            if (pm == null || pm.Deleted)
                return;

            // Get the class instance
            PlayerClassV2 classInstance = PlayerClassV2.GetClass(classType);
            if (classInstance == null)
            {
                pm.SendMessage(0x22, "Error: This class is not yet implemented.");
                return;
            }

            // Check if player already has a class assigned
            if (pm.VystiaClassV2 != PlayerClassTypeV2.None)
            {
                pm.SendMessage(0x22, "You have already chosen a class! This cannot be changed.");
                return;
            }

            if (!VystiaClassManager.Instance.AssignClass(pm, classType))
                return;

            // Confirmation messages
            pm.SendMessage(0x35, $"You are now a {classInstance.ClassName}!");
            pm.SendMessage(0x3B2, classInstance.ClassDescription);
            pm.SendMessage(0x3B2, "Your stats, skills, and equipment have been adjusted.");

            // Visual effect
            Effects.SendLocationParticles(pm, 0x376A, 9, 32, 5005);
            pm.PlaySound(0x1F2);

            // Log the class selection
            Console.WriteLine($"VystiaClass: {pm.Name} ({pm.Account?.Username ?? "unknown"}) selected class: {classInstance.ClassName}");
        }

        /// <summary>
        /// Check if a player has selected a Vystia class
        /// </summary>
        public static bool HasSelectedClass(PlayerMobile pm)
        {
            return pm != null && pm.VystiaClassV2 != PlayerClassTypeV2.None;
        }

        /// <summary>
        /// Get the player's current class name
        /// </summary>
        public static string GetClassName(PlayerMobile pm)
        {
            if (pm == null || pm.VystiaClassV2 == PlayerClassTypeV2.None)
                return "None";

            PlayerClassV2 classInstance = PlayerClassV2.GetClass(pm.VystiaClassV2);
            return classInstance?.ClassName ?? "Unknown";
        }
    }
}
