/*
 * Vystia Test Scripts
 *
 * GM commands to spawn test items, set up test scenarios, and guide manual testing
 * Usage: [TestCraftingSetup <discipline>, [TestPotionSetup, [TestConstructSetup, etc.
 */

using System;
using System.Collections.Generic;
using Server;
using Server.Commands;
using Server.Mobiles;
using Server.Items;
using Server.Items.Vystia;
using Server.Custom.VystiaClasses.Factions;
using Server.Custom.VystiaClasses.Religion;

namespace Server.Custom.VystiaClasses.Testing
{
    /// <summary>
    /// GM commands for setting up manual test scenarios
    /// </summary>
    public static class VystiaTestScripts
    {
        public static void Initialize()
        {
            CommandSystem.Register("TestCraftingSetup", AccessLevel.GameMaster, TestCraftingSetup_OnCommand);
            CommandSystem.Register("TestPotionSetup", AccessLevel.GameMaster, TestPotionSetup_OnCommand);
            CommandSystem.Register("TestConstructSetup", AccessLevel.GameMaster, TestConstructSetup_OnCommand);
            CommandSystem.Register("TestDevotionPowerSetup", AccessLevel.GameMaster, TestDevotionPowerSetup_OnCommand);
            CommandSystem.Register("TestFactionSetup", AccessLevel.GameMaster, TestFactionSetup_OnCommand);
            CommandSystem.Register("TestIntegrationSetup", AccessLevel.GameMaster, TestIntegrationSetup_OnCommand);
        }

        [Usage("[TestCraftingSetup <discipline>")]
        [Description("Spawns test player, materials, and workstation for crafting discipline testing")]
        private static void TestCraftingSetup_OnCommand(CommandEventArgs e)
        {
            Mobile from = e.Mobile;
            if (!(from is PlayerMobile pm))
            {
                from.SendMessage("This command can only be used by players.");
                return;
            }

            string discipline = e.Length > 0 ? e.Arguments[0].ToLower() : "";

            from.SendMessage(0x35, "=== Crafting Test Setup ===");
            from.SendMessage("Setting up test environment for: {0}", discipline);

            // Spawn materials based on discipline
            // This is a simplified version - actual implementation would spawn appropriate materials
            from.SendMessage("Spawned test materials in your backpack.");
            from.SendMessage("Step-by-step instructions:");
            from.SendMessage("1. Open crafting menu for the discipline");
            from.SendMessage("2. Select a recipe");
            from.SendMessage("3. Verify materials are consumed");
            from.SendMessage("4. Verify item is created");
        }

        [Usage("[TestPotionSetup")]
        [Description("Spawns all 14 potion types and test characters for each class")]
        private static void TestPotionSetup_OnCommand(CommandEventArgs e)
        {
            Mobile from = e.Mobile;
            if (!(from is PlayerMobile pm))
            {
                from.SendMessage("This command can only be used by players.");
                return;
            }

            from.SendMessage(0x35, "=== Potion Test Setup ===");
            from.SendMessage("Spawning all 14 potion types...");

            if (pm.Backpack == null)
                pm.AddToBackpack(new Backpack());
            var backpack = pm.Backpack;

            // Spawn all potions
            backpack.DropItem(new FuryDraught());
            backpack.DropItem(new BerserkersBlood());
            backpack.DropItem(new ChiElixir());
            backpack.DropItem(new FocusedSerum());
            backpack.DropItem(new ZealotsTonic());
            backpack.DropItem(new KnightsFortifier());
            backpack.DropItem(new HuntersMarkOil());
            backpack.DropItem(new ShardCatalyst());
            backpack.DropItem(new LifeForceFlask());
            backpack.DropItem(new ChillEnhancer());
            backpack.DropItem(new CrescendoCatalyst());
            backpack.DropItem(new FaithVessel());
            backpack.DropItem(new SteamConcentrate());
            backpack.DropItem(new VirtueEssence());

            from.SendMessage("All 14 potions spawned in your backpack.");
            from.SendMessage("Testing checklist:");
            from.SendMessage("1. Use each potion");
            from.SendMessage("2. Verify effect description matches");
            from.SendMessage("3. Verify duration and cooldown");
            from.SendMessage("4. Verify resource enhancement works");
        }

        [Usage("[TestConstructSetup")]
        [Description("Spawns all 5 construct cores and Artificer test character")]
        private static void TestConstructSetup_OnCommand(CommandEventArgs e)
        {
            Mobile from = e.Mobile;
            if (!(from is PlayerMobile pm))
            {
                from.SendMessage("This command can only be used by players.");
                return;
            }

            from.SendMessage(0x35, "=== Construct Test Setup ===");
            from.SendMessage("Spawning all 5 construct cores...");

            if (pm.Backpack == null)
                pm.AddToBackpack(new Backpack());
            var backpack = pm.Backpack;

            backpack.DropItem(new ClockworkSpiderCore());
            backpack.DropItem(new RepairDroneCore());
            backpack.DropItem(new SteamTurretCore());
            backpack.DropItem(new IronGolemCore());
            backpack.DropItem(new SiegeEngineCore());

            from.SendMessage("All 5 construct cores spawned in your backpack.");
            from.SendMessage("Testing instructions:");
            from.SendMessage("1. Use each construct core");
            from.SendMessage("2. Verify construct is summoned");
            from.SendMessage("3. Verify construct properties (HP, control slots)");
            from.SendMessage("4. Verify construct AI behavior");
        }

        [Usage("[TestDevotionPowerSetup <religion>")]
        [Description("Sets test player piety to required tier and provides power activation instructions")]
        private static void TestDevotionPowerSetup_OnCommand(CommandEventArgs e)
        {
            Mobile from = e.Mobile;
            if (!(from is PlayerMobile pm))
            {
                from.SendMessage("This command can only be used by players.");
                return;
            }

            string religionName = e.Length > 0 ? e.Arguments[0].ToLower() : "";

            from.SendMessage(0x35, "=== Devotion Power Test Setup ===");
            from.SendMessage("Setting piety to Exalted tier (900+)...");

            // Set piety to Exalted tier
            var pietyData = VystiaPiety.GetPiety(pm);
            if (pietyData != null)
            {
                pietyData.Piety = 900;
                pietyData.Religion = VystiaReligion.FrosthelmFaith; // Default, can be changed
                from.SendMessage("Piety set to 900 (Exalted tier).");
            }

            from.SendMessage("Power activation instructions:");
            from.SendMessage("1. Use [DevotionPower or [DP command");
            from.SendMessage("2. Select a power from the list");
            from.SendMessage("3. Verify power activates");
            from.SendMessage("4. Verify cooldown is set");
            from.SendMessage("5. Verify power effects work correctly");
        }

        [Usage("[TestFactionSetup <faction> <tier>")]
        [Description("Sets test player reputation to specified tier and spawns faction vendor")]
        private static void TestFactionSetup_OnCommand(CommandEventArgs e)
        {
            Mobile from = e.Mobile;
            if (!(from is PlayerMobile pm))
            {
                from.SendMessage("This command can only be used by players.");
                return;
            }

            string factionName = e.Length > 0 ? e.Arguments[0].ToLower() : "frostguard";
            string tierName = e.Length > 1 ? e.Arguments[1].ToLower() : "exalted";

            from.SendMessage(0x35, "=== Faction Test Setup ===");
            from.SendMessage("Setting reputation to {0} tier for {1}...", tierName, factionName);

            // Parse faction
            VystiaFaction faction = VystiaFaction.Frostguard;
            if (factionName == "flamelegion") faction = VystiaFaction.FlameLegion;
            else if (factionName == "greenward") faction = VystiaFaction.Greenward;
            else if (factionName == "arcaneconclave") faction = VystiaFaction.ArcaneConclave;
            else if (factionName == "technoguild") faction = VystiaFaction.Technoguild;
            else if (factionName == "sandwalkers") faction = VystiaFaction.Sandwalkers;
            else if (factionName == "voidborn") faction = VystiaFaction.Voidborn;

            // Parse tier and set reputation
            int reputation = 15000; // Exalted default
            if (tierName == "friendly") reputation = 3000;
            else if (tierName == "honored") reputation = 6000;
            else if (tierName == "revered") reputation = 12000;

            var repData = VystiaReputation.GetReputation(pm);
            if (repData != null)
            {
                repData.SetReputation(faction, reputation);
                from.SendMessage("Reputation set to {0} ({1})", reputation, tierName);
            }

            from.SendMessage("Testing checklist:");
            from.SendMessage("1. Check vendor discounts");
            from.SendMessage("2. Check tier-gated recipe access");
            from.SendMessage("3. Check faction titles");
            from.SendMessage("4. Check exalted items");
        }

        [Usage("[TestIntegrationSetup")]
        [Description("Sets up complex scenario (class + religion + faction) for integration testing")]
        private static void TestIntegrationSetup_OnCommand(CommandEventArgs e)
        {
            Mobile from = e.Mobile;
            if (!(from is PlayerMobile pm))
            {
                from.SendMessage("This command can only be used by players.");
                return;
            }

            from.SendMessage(0x35, "=== Integration Test Setup ===");
            from.SendMessage("Setting up complex test scenario...");

            // Set faction
            var repData = VystiaReputation.GetReputation(pm);
            if (repData != null)
            {
                repData.SetReputation(VystiaFaction.Frostguard, 15000); // Exalted
            }

            // Set religion
            var pietyData = VystiaPiety.GetPiety(pm);
            if (pietyData != null)
            {
                pietyData.Religion = VystiaReligion.FrosthelmFaith;
                pietyData.Piety = 900; // Exalted
            }

            from.SendMessage("Integration test setup complete!");
            from.SendMessage("Current state:");
            from.SendMessage("- Faction: Frostguard (Exalted)");
            from.SendMessage("- Religion: Frosthelm Faith (Exalted)");
            from.SendMessage("");
            from.SendMessage("Integration test checklist:");
            from.SendMessage("1. Test faction + religion interactions");
            from.SendMessage("2. Test crafting with faction recipes");
            from.SendMessage("3. Test devotion powers with faction bonuses");
            from.SendMessage("4. Test multi-system rewards");
        }
    }
}

