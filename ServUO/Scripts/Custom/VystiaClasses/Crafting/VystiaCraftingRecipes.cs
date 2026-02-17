/*
 * Vystia Crafting Recipes
 * Extends DefBlacksmithy with regional weapon and armor recipes
 * Uses regional Vystia ingots for crafting
 */

using System;
using Server;
using Server.Items;
using Server.Engines.Craft;

// Alias for Vystia items to avoid ambiguity with base UO items
using Vystia = Server.Items.Vystia;

namespace Server.Custom.VystiaClasses.Crafting
{
    /// <summary>
    /// Adds Vystia regional crafting recipes to the blacksmithing system
    /// </summary>
    public static class VystiaCraftingRecipes
    {
        // Group names for crafting menu
        private const string VystiaWeapons = "Vystia Weapons";
        private const string VystiaArmor = "Vystia Armor";
        private const string VystiaShields = "Vystia Shields";

        public static void Initialize()
        {
            // Hook into the blacksmithing system
            CraftSystem blacksmithy = DefBlacksmithy.CraftSystem;

            // Add regional weapon recipes
            AddFrostholdWeapons(blacksmithy);
            AddEmberlandsWeapons(blacksmithy);
            AddCrystalBarrensWeapons(blacksmithy);
            AddIroncladWeapons(blacksmithy);
            AddShadowVoidWeapons(blacksmithy);

            // Add regional armor recipes
            AddFrostholdArmor(blacksmithy);
            AddEmberlandsArmor(blacksmithy);
            AddIroncladArmor(blacksmithy);
            AddVoidforgedArmor(blacksmithy);

            // Add regional shields
            AddVystiaShields(blacksmithy);

            // ============================================
            // LEGENDARY WEAPONS (Phase 4.10)
            // ============================================

            // TODO: Add legendary weapon recipes when legendary weapon classes and boss materials are created
            // These recipes require GM skill (100), boss materials, and faction reputation
            // Recipes:
            // - The Eternal Winter (Halberd): 20 Frostforged, 10 Eternal Ice, 1 Heart of Winter
            // - Phoenix Ascension (Katana): 20 Emberforged, 10 Everburning Coal, 1 Lava Pearl
            // - The Cogmaster (War Hammer): 20 Clockwork, 10 Steam Core, 1 Forge Heart
            // - Voidcaller (Staff): 20 Voidforged, 10 Lich Dust, 1 Phylactery Fragment

            Console.WriteLine("[Vystia] Crafting recipes registered: Regional weapons, armor, and shields");
            Console.WriteLine("[Vystia] NOTE: Legendary weapon recipes pending weapon and material class creation.");
        }

        #region Frosthold Weapons (Cold Damage)

        private static void AddFrostholdWeapons(CraftSystem cs)
        {
            // Swords
            cs.AddCraft(typeof(Vystia.IcicleBlade), VystiaWeapons, "Icicle Blade", 70.0, 100.0, typeof(FrostforgedIngot), "Frostforged Ingot", 12, "You need more frostforged ingots.");
            cs.AddCraft(typeof(Vystia.WintersEdge), VystiaWeapons, "Winter's Edge", 75.0, 125.0, typeof(FrostforgedIngot), "Frostforged Ingot", 14, "You need more frostforged ingots.");
            cs.AddCraft(typeof(Vystia.Frostbite), VystiaWeapons, "Frostbite", 65.0, 115.0, typeof(FrostforgedIngot), "Frostforged Ingot", 8, "You need more frostforged ingots.");
            cs.AddCraft(typeof(Vystia.GlacierShard), VystiaWeapons, "Glacier Shard", 68.0, 118.0, typeof(FrostforgedIngot), "Frostforged Ingot", 8, "You need more frostforged ingots.");

            // Axes
            cs.AddCraft(typeof(Vystia.FrozenCleaver), VystiaWeapons, "Frozen Cleaver", 72.0, 122.0, typeof(FrostforgedIngot), "Frostforged Ingot", 14, "You need more frostforged ingots.");
            cs.AddCraft(typeof(Vystia.IceShardAxe), VystiaWeapons, "Ice Shard Axe", 70.0, 100.0, typeof(FrostforgedIngot), "Frostforged Ingot", 12, "You need more frostforged ingots.");
            cs.AddCraft(typeof(Vystia.GlacialHatchet), VystiaWeapons, "Glacial Hatchet", 65.0, 115.0, typeof(FrostforgedIngot), "Frostforged Ingot", 8, "You need more frostforged ingots.");

            // Maces
            cs.AddCraft(typeof(Vystia.GlacialHammer), VystiaWeapons, "Glacial Hammer", 74.0, 124.0, typeof(FrostforgedIngot), "Frostforged Ingot", 16, "You need more frostforged ingots.");
            cs.AddCraft(typeof(Vystia.FrostMaul), VystiaWeapons, "Frost Maul", 68.0, 118.0, typeof(FrostforgedIngot), "Frostforged Ingot", 10, "You need more frostforged ingots.");
            cs.AddCraft(typeof(Vystia.IceClub), VystiaWeapons, "Ice Club", 55.0, 105.0, typeof(FrostforgedIngot), "Frostforged Ingot", 6, "You need more frostforged ingots.");

            // Polearms
            cs.AddCraft(typeof(Vystia.IceLance), VystiaWeapons, "Ice Lance", 76.0, 126.0, typeof(FrostforgedIngot), "Frostforged Ingot", 12, "You need more frostforged ingots.");
            cs.AddCraft(typeof(Vystia.FrozenHalberd), VystiaWeapons, "Frozen Halberd", 78.0, 128.0, typeof(FrostforgedIngot), "Frostforged Ingot", 20, "You need more frostforged ingots.");
        }

        #endregion

        #region Emberlands Weapons (Fire Damage)

        private static void AddEmberlandsWeapons(CraftSystem cs)
        {
            // Swords
            cs.AddCraft(typeof(Vystia.FlameTongue), VystiaWeapons, "Flame Tongue", 70.0, 100.0, typeof(EmberforgedIngot), "Emberforged Ingot", 8, "You need more emberforged ingots.");
            cs.AddCraft(typeof(Vystia.MagmaBlade), VystiaWeapons, "Magma Blade", 72.0, 122.0, typeof(EmberforgedIngot), "Emberforged Ingot", 10, "You need more emberforged ingots.");
            cs.AddCraft(typeof(Vystia.PhoenixWing), VystiaWeapons, "Phoenix Wing", 75.0, 125.0, typeof(EmberforgedIngot), "Emberforged Ingot", 10, "You need more emberforged ingots.");
            cs.AddCraft(typeof(Vystia.LavaEdge), VystiaWeapons, "Lava Edge", 74.0, 124.0, typeof(EmberforgedIngot), "Emberforged Ingot", 12, "You need more emberforged ingots.");

            // Axes
            cs.AddCraft(typeof(Vystia.MoltenAxe), VystiaWeapons, "Molten Axe", 76.0, 126.0, typeof(EmberforgedIngot), "Emberforged Ingot", 16, "You need more emberforged ingots.");
            cs.AddCraft(typeof(Vystia.FlameCleaver), VystiaWeapons, "Flame Cleaver", 72.0, 122.0, typeof(EmberforgedIngot), "Emberforged Ingot", 16, "You need more emberforged ingots.");
            cs.AddCraft(typeof(Vystia.LavaPick), VystiaWeapons, "Lava Pick", 60.0, 110.0, typeof(EmberforgedIngot), "Emberforged Ingot", 6, "You need more emberforged ingots.");

            // Maces
            cs.AddCraft(typeof(Vystia.MoltenMace), VystiaWeapons, "Molten Mace", 68.0, 118.0, typeof(EmberforgedIngot), "Emberforged Ingot", 14, "You need more emberforged ingots.");
            cs.AddCraft(typeof(Vystia.MagmaHammer), VystiaWeapons, "Magma Hammer", 74.0, 124.0, typeof(EmberforgedIngot), "Emberforged Ingot", 16, "You need more emberforged ingots.");

            // Polearms
            cs.AddCraft(typeof(Vystia.LavaSpear), VystiaWeapons, "Lava Spear", 72.0, 122.0, typeof(EmberforgedIngot), "Emberforged Ingot", 12, "You need more emberforged ingots.");
            cs.AddCraft(typeof(Vystia.VolcanicPike), VystiaWeapons, "Volcanic Pike", 76.0, 126.0, typeof(EmberforgedIngot), "Emberforged Ingot", 12, "You need more emberforged ingots.");
        }

        #endregion

        #region Crystal Barrens Weapons (Energy Damage)

        private static void AddCrystalBarrensWeapons(CraftSystem cs)
        {
            // Swords
            cs.AddCraft(typeof(Vystia.CrystalShard), VystiaWeapons, "Crystal Shard", 72.0, 122.0, typeof(CrystallineIngot), "Crystalline Ingot", 8, "You need more crystalline ingots.");
            cs.AddCraft(typeof(Vystia.PrismBlade), VystiaWeapons, "Prism Blade", 78.0, 128.0, typeof(CrystallineIngot), "Crystalline Ingot", 12, "You need more crystalline ingots.");
            cs.AddCraft(typeof(Vystia.RefractionEdge), VystiaWeapons, "Refraction Edge", 76.0, 126.0, typeof(CrystallineIngot), "Crystalline Ingot", 8, "You need more crystalline ingots.");
        }

        #endregion

        #region Ironclad Weapons (Physical + Durability)

        private static void AddIroncladWeapons(CraftSystem cs)
        {
            // Swords
            cs.AddCraft(typeof(Vystia.ClockworkSword), VystiaWeapons, "Clockwork Sword", 80.0, 130.0, typeof(ClockworkIngot), "Clockwork Ingot", 10, "You need more clockwork ingots.");
            cs.AddCraft(typeof(Vystia.GearBlade), VystiaWeapons, "Gear Blade", 78.0, 128.0, typeof(ClockworkIngot), "Clockwork Ingot", 14, "You need more clockwork ingots.");
            cs.AddCraft(typeof(Vystia.SteamSaber), VystiaWeapons, "Steam Saber", 75.0, 125.0, typeof(ClockworkIngot), "Clockwork Ingot", 8, "You need more clockwork ingots.");

            // Axes
            cs.AddCraft(typeof(Vystia.GearAxe), VystiaWeapons, "Gear Axe", 82.0, 132.0, typeof(ClockworkIngot), "Clockwork Ingot", 14, "You need more clockwork ingots.");
            cs.AddCraft(typeof(Vystia.SteamCleaver), VystiaWeapons, "Steam Cleaver", 78.0, 128.0, typeof(ClockworkIngot), "Clockwork Ingot", 14, "You need more clockwork ingots.");

            // Maces
            cs.AddCraft(typeof(Vystia.PistonMace), VystiaWeapons, "Piston Mace", 76.0, 126.0, typeof(ClockworkIngot), "Clockwork Ingot", 14, "You need more clockwork ingots.");
            cs.AddCraft(typeof(Vystia.SteamHammer), VystiaWeapons, "Steam Hammer", 80.0, 130.0, typeof(ClockworkIngot), "Clockwork Ingot", 16, "You need more clockwork ingots.");
        }

        #endregion

        #region ShadowVoid Weapons (Void/Dark Damage)

        private static void AddShadowVoidWeapons(CraftSystem cs)
        {
            // Swords
            cs.AddCraft(typeof(Vystia.ShadowFang), VystiaWeapons, "Shadow Fang", 74.0, 124.0, typeof(VoidforgedIngot), "Voidforged Ingot", 8, "You need more voidforged ingots.");
            cs.AddCraft(typeof(Vystia.VoidEdge), VystiaWeapons, "Void Edge", 78.0, 128.0, typeof(VoidforgedIngot), "Voidforged Ingot", 8, "You need more voidforged ingots.");
            cs.AddCraft(typeof(Vystia.DarkBlade), VystiaWeapons, "Dark Blade", 76.0, 126.0, typeof(VoidforgedIngot), "Voidforged Ingot", 10, "You need more voidforged ingots.");
        }

        #endregion

        #region Frosthold Armor

        private static void AddFrostholdArmor(CraftSystem cs)
        {
            cs.AddCraft(typeof(Vystia.FrostforgedPlateChest), VystiaArmor, "Frostforged Plate Chest", 80.0, 130.0, typeof(FrostforgedIngot), "Frostforged Ingot", 25, "You need more frostforged ingots.");
            cs.AddCraft(typeof(Vystia.FrostforgedPlateLegs), VystiaArmor, "Frostforged Plate Legs", 78.0, 128.0, typeof(FrostforgedIngot), "Frostforged Ingot", 20, "You need more frostforged ingots.");
            cs.AddCraft(typeof(Vystia.FrostforgedPlateArms), VystiaArmor, "Frostforged Plate Arms", 76.0, 126.0, typeof(FrostforgedIngot), "Frostforged Ingot", 18, "You need more frostforged ingots.");
            cs.AddCraft(typeof(Vystia.FrostforgedPlateGloves), VystiaArmor, "Frostforged Plate Gloves", 74.0, 124.0, typeof(FrostforgedIngot), "Frostforged Ingot", 12, "You need more frostforged ingots.");
            cs.AddCraft(typeof(Vystia.FrostforgedPlateGorget), VystiaArmor, "Frostforged Plate Gorget", 72.0, 122.0, typeof(FrostforgedIngot), "Frostforged Ingot", 10, "You need more frostforged ingots.");
            cs.AddCraft(typeof(Vystia.FrostforgedPlateHelm), VystiaArmor, "Frostforged Plate Helm", 76.0, 126.0, typeof(FrostforgedIngot), "Frostforged Ingot", 15, "You need more frostforged ingots.");
        }

        #endregion

        #region Emberlands Armor

        private static void AddEmberlandsArmor(CraftSystem cs)
        {
            cs.AddCraft(typeof(Vystia.EmberforgedPlateChest), VystiaArmor, "Emberforged Plate Chest", 80.0, 130.0, typeof(EmberforgedIngot), "Emberforged Ingot", 25, "You need more emberforged ingots.");
            cs.AddCraft(typeof(Vystia.EmberforgedPlateLegs), VystiaArmor, "Emberforged Plate Legs", 78.0, 128.0, typeof(EmberforgedIngot), "Emberforged Ingot", 20, "You need more emberforged ingots.");
            cs.AddCraft(typeof(Vystia.EmberforgedPlateArms), VystiaArmor, "Emberforged Plate Arms", 76.0, 126.0, typeof(EmberforgedIngot), "Emberforged Ingot", 18, "You need more emberforged ingots.");
            cs.AddCraft(typeof(Vystia.EmberforgedPlateGloves), VystiaArmor, "Emberforged Plate Gloves", 74.0, 124.0, typeof(EmberforgedIngot), "Emberforged Ingot", 12, "You need more emberforged ingots.");
            cs.AddCraft(typeof(Vystia.EmberforgedPlateGorget), VystiaArmor, "Emberforged Plate Gorget", 72.0, 122.0, typeof(EmberforgedIngot), "Emberforged Ingot", 10, "You need more emberforged ingots.");
            cs.AddCraft(typeof(Vystia.EmberforgedPlateHelm), VystiaArmor, "Emberforged Plate Helm", 76.0, 126.0, typeof(EmberforgedIngot), "Emberforged Ingot", 15, "You need more emberforged ingots.");
        }

        #endregion

        #region Ironclad Armor

        private static void AddIroncladArmor(CraftSystem cs)
        {
            cs.AddCraft(typeof(Vystia.ClockworkPlateChest), VystiaArmor, "Clockwork Plate Chest", 85.0, 135.0, typeof(ClockworkIngot), "Clockwork Ingot", 25, "You need more clockwork ingots.");
            cs.AddCraft(typeof(Vystia.ClockworkPlateLegs), VystiaArmor, "Clockwork Plate Legs", 82.0, 132.0, typeof(ClockworkIngot), "Clockwork Ingot", 20, "You need more clockwork ingots.");
            cs.AddCraft(typeof(Vystia.ClockworkPlateArms), VystiaArmor, "Clockwork Plate Arms", 80.0, 130.0, typeof(ClockworkIngot), "Clockwork Ingot", 18, "You need more clockwork ingots.");
            cs.AddCraft(typeof(Vystia.ClockworkPlateGloves), VystiaArmor, "Clockwork Plate Gloves", 78.0, 128.0, typeof(ClockworkIngot), "Clockwork Ingot", 12, "You need more clockwork ingots.");
            cs.AddCraft(typeof(Vystia.ClockworkPlateGorget), VystiaArmor, "Clockwork Plate Gorget", 76.0, 126.0, typeof(ClockworkIngot), "Clockwork Ingot", 10, "You need more clockwork ingots.");
            cs.AddCraft(typeof(Vystia.ClockworkPlateHelm), VystiaArmor, "Clockwork Plate Helm", 80.0, 130.0, typeof(ClockworkIngot), "Clockwork Ingot", 15, "You need more clockwork ingots.");
        }

        #endregion

        #region Voidforged Armor

        private static void AddVoidforgedArmor(CraftSystem cs)
        {
            cs.AddCraft(typeof(Vystia.VoidforgedPlateChest), VystiaArmor, "Voidforged Plate Chest", 82.0, 132.0, typeof(VoidforgedIngot), "Voidforged Ingot", 25, "You need more voidforged ingots.");
            cs.AddCraft(typeof(Vystia.VoidforgedPlateLegs), VystiaArmor, "Voidforged Plate Legs", 80.0, 130.0, typeof(VoidforgedIngot), "Voidforged Ingot", 20, "You need more voidforged ingots.");
            cs.AddCraft(typeof(Vystia.VoidforgedPlateArms), VystiaArmor, "Voidforged Plate Arms", 78.0, 128.0, typeof(VoidforgedIngot), "Voidforged Ingot", 18, "You need more voidforged ingots.");
            cs.AddCraft(typeof(Vystia.VoidforgedPlateGloves), VystiaArmor, "Voidforged Plate Gloves", 76.0, 126.0, typeof(VoidforgedIngot), "Voidforged Ingot", 12, "You need more voidforged ingots.");
            cs.AddCraft(typeof(Vystia.VoidforgedPlateGorget), VystiaArmor, "Voidforged Plate Gorget", 74.0, 124.0, typeof(VoidforgedIngot), "Voidforged Ingot", 10, "You need more voidforged ingots.");
            cs.AddCraft(typeof(Vystia.VoidforgedPlateHelm), VystiaArmor, "Voidforged Plate Helm", 78.0, 128.0, typeof(VoidforgedIngot), "Voidforged Ingot", 15, "You need more voidforged ingots.");
        }

        #endregion

        #region Vystia Shields

        private static void AddVystiaShields(CraftSystem cs)
        {
            cs.AddCraft(typeof(Vystia.IceWall), VystiaShields, "Ice Wall Shield", 72.0, 122.0, typeof(FrostforgedIngot), "Frostforged Ingot", 18, "You need more frostforged ingots.");
            cs.AddCraft(typeof(Vystia.FlameGuard), VystiaShields, "Flame Guard Shield", 72.0, 122.0, typeof(EmberforgedIngot), "Emberforged Ingot", 16, "You need more emberforged ingots.");
            cs.AddCraft(typeof(Vystia.PrismShield), VystiaShields, "Prism Shield", 74.0, 124.0, typeof(CrystallineIngot), "Crystalline Ingot", 14, "You need more crystalline ingots.");
            cs.AddCraft(typeof(Vystia.ClockworkShield), VystiaShields, "Clockwork Shield", 78.0, 128.0, typeof(ClockworkIngot), "Clockwork Ingot", 25, "You need more clockwork ingots.");
            cs.AddCraft(typeof(Vystia.BogShield), VystiaShields, "Bog Shield", 68.0, 118.0, typeof(ShadowforgedIngot), "Shadowforged Ingot", 8, "You need more shadowforged ingots.");
            cs.AddCraft(typeof(Vystia.SandShield), VystiaShields, "Sand Shield", 65.0, 115.0, typeof(SunforgedIngot), "Sunforged Ingot", 10, "You need more sunforged ingots.");
            cs.AddCraft(typeof(Vystia.VoidShield), VystiaShields, "Void Shield", 80.0, 130.0, typeof(VoidforgedIngot), "Voidforged Ingot", 25, "You need more voidforged ingots.");
            cs.AddCraft(typeof(Vystia.LivingShield), VystiaShields, "Living Shield", 70.0, 100.0, typeof(NatureforgedIngot), "Natureforged Ingot", 8, "You need more natureforged ingots.");
        }

        #endregion
    }
}
