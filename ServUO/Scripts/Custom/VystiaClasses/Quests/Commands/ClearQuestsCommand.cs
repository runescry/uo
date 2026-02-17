using System;
using Server;
using Server.Commands;
using Server.Mobiles;
using Server.Engines.XmlSpawner2;
using Server.Custom.VystiaClasses.Quests;
using Server.Custom.VystiaClasses.Quests.Generation;

namespace Server.Custom.VystiaClasses.Quests.Commands
{
    /// <summary>
    /// GM command to clear all quests for a player
    /// Usage: [ClearQuests [playerName]
    /// </summary>
    public class ClearQuestsCommand
    {
        public static void Initialize()
        {
            CommandSystem.Register("ClearQuests", AccessLevel.GameMaster, OnCommand);
        }

        [Usage("ClearQuests [playerName]")]
        [Description("Clears all active and completed Vystia quests for a player. If no name is specified, clears your own quests.")]
        private static void OnCommand(CommandEventArgs e)
        {
            Mobile from = e.Mobile;
            PlayerMobile target = null;

            if (e.Length >= 1)
            {
                // Find player by name
                string playerName = e.GetString(0);
                target = FindPlayerByName(playerName);
                
                if (target == null)
                {
                    from.SendMessage($"Player '{playerName}' not found.");
                    return;
                }
            }
            else if (from is PlayerMobile pm)
            {
                target = pm;
            }
            else
            {
                from.SendMessage("You must be a player or specify a player name.");
                return;
            }

            var tracker = VystiaQuestTracker.GetTracker(target);
            int activeCount = 0;
            
            if (tracker != null)
            {
                activeCount = tracker.GetActiveQuests()?.Count ?? 0;
                tracker.ClearAllQuests();
            }

            // Also clear generated quest instances (LLM-generated ephemeral quests)
            var genTracker = GeneratedQuestInstanceAttachment.Get(target);
            int genCount = 0;
            if (genTracker != null)
            {
                genCount = genTracker.Instances?.Count ?? 0;
                // Delete the attachment to clear all instances
                genTracker.Delete();
            }

            int totalCleared = activeCount + genCount;
            
            if (target == from)
            {
                from.SendMessage($"Cleared all your quests ({totalCleared} quests removed: {activeCount} active, {genCount} generated).");
            }
            else
            {
                from.SendMessage($"Cleared all quests for {target.Name} ({totalCleared} quests removed: {activeCount} active, {genCount} generated).");
                target.SendMessage("All your quests have been cleared by a Game Master.");
            }
        }

        /// <summary>
        /// Find a player by name
        /// </summary>
        private static PlayerMobile FindPlayerByName(string name)
        {
            foreach (Mobile m in World.Mobiles.Values)
            {
                if (m is PlayerMobile pm && pm.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
                {
                    return pm;
                }
            }
            return null;
        }
    }
}

