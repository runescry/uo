using System;
using System.Collections.Generic;
using System.Linq;
using Server;
using Server.Commands;
using Server.Mobiles;
using Server.Custom.VystiaClasses.Quests;
using Server.Custom.VystiaClasses.Quests.Generation;
using Server.Engines.XmlSpawner2;

namespace Server.Custom.VystiaClasses.Quests.Commands
{
    /// <summary>
    /// GM command to purge ALL quests from the system
    /// Usage: [PurgeAllQuests [confirm]
    /// </summary>
    public class PurgeAllQuestsCommand
    {
        public static void Initialize()
        {
            CommandSystem.Register("PurgeAllQuests", AccessLevel.Administrator, OnCommand);
        }

        [Usage("PurgeAllQuests [confirm]")]
        [Description("PURGES ALL QUESTS from the system. Requires 'confirm' parameter. This will delete all quest definitions, player tracking, spawned NPCs/items, and generated instances.")]
        private static void OnCommand(CommandEventArgs e)
        {
            Mobile from = e.Mobile;
            
            if (e.Length < 1 || !e.GetString(0).Equals("confirm", StringComparison.OrdinalIgnoreCase))
            {
                from.SendMessage(38, "WARNING: This command will DELETE ALL QUESTS from the system!");
                from.SendMessage(38, "This includes:");
                from.SendMessage(38, "  - All quest definitions");
                from.SendMessage(38, "  - All player quest tracking (active and completed)");
                from.SendMessage(38, "  - All spawned NPCs, bosses, and items");
                from.SendMessage(38, "  - All generated quest instances");
                from.SendMessage(38, "");
                from.SendMessage(68, "To confirm, type: [PurgeAllQuests confirm");
                
                // Show current quest count
                var currentQuests = DynamicQuestManager.GetAllDynamicQuests();
                from.SendMessage($"Current quest count: {currentQuests.Count}");
                return;
            }

            from.SendMessage(38, "Purging all quests... This may take a moment.");
            
            int questCount = 0;
            int playerTrackingCleared = 0;
            int spawnedEntitiesDeleted = 0;
            int instanceCount = 0;

            // Get all quests before deletion (we need the IDs)
            var allQuests = DynamicQuestManager.GetAllDynamicQuests()?.ToList() ?? new List<DynamicQuest>();
            questCount = allQuests.Count;

            if (questCount == 0)
            {
                from.SendMessage(68, "No quests found in the system.");
                Console.WriteLine("[PurgeAllQuests] No quests found to purge.");
            }

            // Delete all quests (this will clean up player tracking and spawned entities)
            foreach (var quest in allQuests)
            {
                if (quest == null)
                    continue;
                    
                try
                {
                    DynamicQuestManager.RemoveDynamicQuest(quest);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[PurgeAllQuests] Error deleting quest [{quest?.QuestID ?? -1}]: {ex.Message}");
                    Console.WriteLine($"[PurgeAllQuests] Stack trace: {ex.StackTrace}");
                }
            }

            // Clear all player quest tracking
            // IMPORTANT: Create a snapshot list first to avoid collection modification during iteration
            var allPlayers = new List<PlayerMobile>();
            foreach (Mobile m in World.Mobiles.Values)
            {
                if (m is PlayerMobile pm && !pm.Deleted)
                {
                    allPlayers.Add(pm);
                }
            }
            
            foreach (var pm in allPlayers)
            {
                if (pm == null || pm.Deleted)
                    continue;
                    
                try
                {
                    // Clear VystiaQuestTracker
                    var tracker = VystiaQuestTracker.GetTracker(pm);
                    if (tracker != null)
                    {
                        tracker.ClearAllQuests();
                        playerTrackingCleared++;
                    }

                    // Delete GeneratedQuestInstanceAttachment entirely
                    var genAttachment = GeneratedQuestInstanceAttachment.Get(pm);
                    if (genAttachment != null)
                    {
                        instanceCount += genAttachment.Instances?.Count ?? 0;
                        
                        // Collect all serials first to avoid collection modification issues
                        var serialsToDelete = new List<int>();
                        if (genAttachment.Instances != null)
                        {
                            foreach (var inst in genAttachment.Instances)
                            {
                                if (inst?.SpawnedSerials != null)
                                {
                                    serialsToDelete.AddRange(inst.SpawnedSerials);
                                }
                            }
                        }
                        
                        // Delete all spawned entities from instances
                        foreach (int serialValue in serialsToDelete)
                        {
                            try
                            {
                                Serial serial = (Serial)serialValue;
                                var mob = World.FindMobile(serial);
                                if (mob != null && !mob.Deleted)
                                {
                                    mob.Delete();
                                    spawnedEntitiesDeleted++;
                                }
                                else
                                {
                                    var item = World.FindItem(serial);
                                    if (item != null && !item.Deleted)
                                    {
                                        item.Delete();
                                        spawnedEntitiesDeleted++;
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"[PurgeAllQuests] Error deleting entity {serialValue}: {ex.Message}");
                                // best-effort cleanup - continue
                            }
                        }
                        
                        genAttachment.Delete();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[PurgeAllQuests] Error clearing quests for player {pm?.Name ?? "Unknown"}: {ex.Message}");
                    Console.WriteLine($"[PurgeAllQuests] Stack trace: {ex.StackTrace}");
                }
            }

            // Save the quest registry (should be empty now)
            try
            {
                DynamicQuestManager.Save();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[PurgeAllQuests] Error saving quest registry: {ex.Message}");
            }

            // Summary
            from.SendMessage(68, $"Quest purge complete!");
            from.SendMessage($"  - Deleted {questCount} quest definition(s)");
            from.SendMessage($"  - Cleared quest tracking for {playerTrackingCleared} player(s)");
            from.SendMessage($"  - Deleted {spawnedEntitiesDeleted} spawned entity/entities");
            from.SendMessage($"  - Removed {instanceCount} generated quest instance(s)");
            
            Console.WriteLine($"[PurgeAllQuests] Purge complete: {questCount} quests, {playerTrackingCleared} players, {spawnedEntitiesDeleted} entities, {instanceCount} instances");
        }
    }
}

