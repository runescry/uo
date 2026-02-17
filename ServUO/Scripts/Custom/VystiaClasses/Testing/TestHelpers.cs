/*
 * Vystia Test Helpers
 *
 * Utility methods for creating test players, items, and simulating events
 * for automated testing of Vystia systems.
 */

using System;
using System.Collections.Generic;
using Server;
using Server.Mobiles;
using Server.Items;
using Server.Custom.VystiaClasses.Factions;
using Server.Custom.VystiaClasses.Religion;
using Server.Custom.VystiaClasses.Systems;
using Server.Custom.VystiaClasses.Zones;

namespace Server.Custom.VystiaClasses.Testing
{
    /// <summary>
    /// Helper methods for creating test objects and simulating events
    /// </summary>
    public static class TestHelpers
    {
        #region Test Player Creation

        /// <summary>
        /// Create a test player with default properties
        /// </summary>
        public static PlayerMobile CreateTestPlayer(string name = "TestPlayer")
        {
            var player = new PlayerMobile();
            player.Name = name;
            player.Female = false;
            player.Race = Race.Human;
            
            // Set to Internal map (not world) to avoid network state
            player.Map = Map.Internal;
            player.Location = new Point3D(0, 0, 0);
            
            // Initialize stats
            player.RawStr = 100;
            player.RawDex = 100;
            player.RawInt = 100;
            player.Hits = player.HitsMax;
            player.Mana = player.ManaMax;
            player.Stam = player.StamMax;
            
            // Initialize gold
            player.AddToBackpack(new Gold(1000));
            
            return player;
        }

        /// <summary>
        /// Create a test player with specific faction and reputation
        /// </summary>
        public static PlayerMobile CreateTestPlayerWithFaction(VystiaFaction faction, int reputation, string name = "TestPlayer")
        {
            var player = CreateTestPlayer(name);
            
            // Set faction reputation
            var repData = VystiaReputation.GetReputation(player);
            if (repData != null)
            {
                repData.SetReputation(faction, reputation);
            }
            
            return player;
        }

        /// <summary>
        /// Create a test player with specific religion and piety
        /// </summary>
        public static PlayerMobile CreateTestPlayerWithReligion(VystiaReligion religion, int piety, string name = "TestPlayer")
        {
            var player = CreateTestPlayer(name);
            
            // Set religion and piety
            var pietyData = VystiaPiety.GetPiety(player);
            if (pietyData != null)
            {
                pietyData.Religion = religion;
                pietyData.Piety = piety;
            }
            
            return player;
        }

        /// <summary>
        /// Create a test player with both faction and religion
        /// </summary>
        public static PlayerMobile CreateTestPlayerWithFactionAndReligion(
            VystiaFaction faction, int reputation,
            VystiaReligion religion, int piety,
            string name = "TestPlayer")
        {
            var player = CreateTestPlayer(name);
            
            // Set faction
            var repData = VystiaReputation.GetReputation(player);
            if (repData != null)
            {
                repData.SetReputation(faction, reputation);
            }
            
            // Set religion
            var pietyData = VystiaPiety.GetPiety(player);
            if (pietyData != null)
            {
                pietyData.Religion = religion;
                pietyData.Piety = piety;
            }
            
            return player;
        }

        #endregion

        #region Event Simulation

        /// <summary>
        /// Simulate a boss death event
        /// </summary>
        public static void SimulateBossDeath(PlayerMobile killer, BaseCreature boss)
        {
            if (killer == null || boss == null)
            {
                TestLogging.WriteLine("WARNING: SimulateBossDeath called with null parameters");
                return;
            }

            try
            {
                // Trigger boss kill rewards via event system
                // Note: This requires the boss to be registered in VystiaBossRewards
                Container corpse = boss.Corpse;
                if (corpse == null)
                    corpse = new Container(0x0E75); // Use backpack itemID as default
                
                TestLogging.WriteLine("Simulating boss death: {0} killed by {1}", boss.Name, killer.Name);
                EventSink.InvokeCreatureDeath(new CreatureDeathEventArgs(boss, killer, corpse));
            }
            catch (Exception ex)
            {
                TestLogging.LogException("SimulateBossDeath", ex);
                throw;
            }
        }

        /// <summary>
        /// Simulate a player death event (PvP)
        /// </summary>
        public static void SimulatePlayerDeath(PlayerMobile killer, PlayerMobile victim)
        {
            if (killer == null || victim == null)
            {
                TestLogging.WriteLine("WARNING: SimulatePlayerDeath called with null parameters");
                return;
            }

            try
            {
                // Set victim's LastKiller
                victim.LastKiller = killer;
                
                TestLogging.WriteLine("Simulating PvP death: {0} killed by {1}", victim.Name, killer.Name);
                // Trigger PvP kill rewards via event system
                EventSink.InvokePlayerDeath(new PlayerDeathEventArgs(victim));
            }
            catch (Exception ex)
            {
                TestLogging.LogException("SimulatePlayerDeath", ex);
                throw;
            }
        }

        /// <summary>
        /// Create a test boss creature for testing
        /// </summary>
        public static BaseCreature CreateTestBoss(string name = "TestBoss")
        {
            // Create a simple test creature - use a basic type that exists
            var boss = new Server.Mobiles.Dragon();
            boss.Name = name;
            boss.Body = 0x190; // Human body
            boss.Hue = 0;
            
            // Set to Internal map
            boss.Map = Map.Internal;
            boss.Location = new Point3D(0, 0, 0);
            
            // Set stats
            boss.SetStr(100);
            boss.SetDex(100);
            boss.SetInt(100);
            boss.SetHits(1000, 1000);
            
            return boss;
        }

        #endregion

        #region Cleanup

        /// <summary>
        /// Cleanup test objects (items and mobiles)
        /// </summary>
        public static void CleanupTestObjects(List<Item> items, List<Mobile> mobiles)
        {
            if (items != null)
            {
                foreach (var item in items)
                {
                    if (item != null && !item.Deleted)
                    {
                        item.Delete();
                    }
                }
            }

            if (mobiles != null)
            {
                foreach (var mobile in mobiles)
                {
                    if (mobile != null && !mobile.Deleted)
                    {
                        mobile.Delete();
                    }
                }
            }
        }

        /// <summary>
        /// Cleanup a single test player
        /// </summary>
        public static void CleanupTestPlayer(PlayerMobile player)
        {
            if (player != null && !player.Deleted)
            {
                // Delete all items
                var items = new List<Item>(player.Items);
                foreach (var item in items)
                {
                    if (item != null && !item.Deleted)
                        item.Delete();
                }

                // Delete backpack contents
                if (player.Backpack != null)
                {
                    var backpackItems = new List<Item>(player.Backpack.Items);
                    foreach (var item in backpackItems)
                    {
                        if (item != null && !item.Deleted)
                            item.Delete();
                    }
                    player.Backpack.Delete();
                }

                // Delete the player
                player.Delete();
            }
        }

        #endregion
    }
}
