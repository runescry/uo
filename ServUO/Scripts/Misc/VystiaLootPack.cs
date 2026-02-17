#region References
using System;
using Server.Items;
#endregion

namespace Server
{
    /// <summary>
    /// Vystia-specific loot packs for regional creatures and bosses.
    /// Each region has its own resources, reagents, and equipment drops.
    /// </summary>
    public class VystiaLootPack
    {
        #region Regional Resource Items

        // Frosthold Resources
        public static readonly LootPackItem[] FrostholdResources = new[]
        {
            new LootPackItem(typeof(FrozenArtifact), 10),
            new LootPackItem(typeof(FrostSeal), 2),
            new LootPackItem(typeof(FrozenOre), 15),           // Phase 1 - Regional ore
            new LootPackItem(typeof(FrostforgedIngot), 8),     // Phase 1 - Regional ingot
            new LootPackItem(typeof(FrostwillowLog), 10),      // Phase 1 - Regional wood
            new LootPackItem(typeof(FrostHide), 12),           // Phase 1 - Regional hide
            new LootPackItem(typeof(FrostEssence), 8),         // Phase 2 - Elemental reagent
            new LootPackItem(typeof(IceCrystal), 6),           // Phase 2 - Special resource
            new LootPackItem(typeof(EternalIce), 3),           // Phase 2 - Special resource (rare)
            new LootPackItem(typeof(Sapphire), 5),             // Ice-themed gem
        };

        // Emberlands Resources
        public static readonly LootPackItem[] EmberlandsResources = new[]
        {
            new LootPackItem(typeof(MoltenOre), 15),           // Phase 1 - Regional ore
            new LootPackItem(typeof(EmberforgedIngot), 8),     // Phase 1 - Regional ingot
            new LootPackItem(typeof(FlamewoodLog), 10),        // Phase 1 - Regional wood
            new LootPackItem(typeof(FireHide), 12),            // Phase 1 - Regional hide
            // REMOVED OLD REAGENT: new LootPackItem(typeof(EmberBloom), 8),           // Phase 2 - Elemental reagent
            new LootPackItem(typeof(LavaPearl), 4),            // Phase 2 - Special resource
            new LootPackItem(typeof(EverburningCoal), 3),      // Phase 2 - Special resource (rare)
            new LootPackItem(typeof(PhoenixFeather), 1),       // Phase 2 - Exotic reagent (very rare)
            new LootPackItem(typeof(Ruby), 5),                 // Fire-themed gem
            new LootPackItem(typeof(SulfurousAsh), 8),
        };

        // Crystal Barrens Resources
        public static readonly LootPackItem[] CrystalResources = new[]
        {
            new LootPackItem(typeof(CrystalOre), 15),          // Phase 1 - Regional ore
            new LootPackItem(typeof(CrystallineIngot), 8),     // Phase 1 - Regional ingot
            new LootPackItem(typeof(CrystalwoodLog), 10),      // Phase 1 - Regional wood
            // REMOVED OLD REAGENT: new LootPackItem(typeof(CrystalPollen), 8),        // Phase 2 - Nature reagent
            new LootPackItem(typeof(PrismaticShard), 4),       // Phase 2 - Special resource
            new LootPackItem(typeof(LeyCrystal), 3),           // Phase 2 - Special resource (rare)
            new LootPackItem(typeof(LeyLineEssence), 2),       // Phase 2 - Exotic reagent
            new LootPackItem(typeof(TimeDust), 1),             // Phase 2 - Exotic reagent (very rare)
            new LootPackItem(typeof(Diamond), 5),
            new LootPackItem(typeof(StarSapphire), 5),
        };

        // Shadowfen Resources
        public static readonly LootPackItem[] ShadowfenResources = new[]
        {
            new LootPackItem(typeof(BogIronOre), 15),          // Phase 1 - Regional ore
            new LootPackItem(typeof(ShadowforgedIngot), 8),    // Phase 1 - Regional ingot
            new LootPackItem(typeof(ShadowwoodLog), 10),       // Phase 1 - Regional wood
            new LootPackItem(typeof(ShadowHide), 12),          // Phase 1 - Regional hide
            new LootPackItem(typeof(VoidDust), 8),             // Phase 2 - Elemental reagent
            new LootPackItem(typeof(SwampLotus), 6),           // Phase 2 - Nature reagent
            new LootPackItem(typeof(ShadowSilk), 4),           // Phase 2 - Special resource
            // REMOVED OLD REAGENT: new LootPackItem(typeof(KrakenInk), 1),            // Phase 2 - Exotic reagent (very rare)
            new LootPackItem(typeof(Emerald), 5),
            new LootPackItem(typeof(Nightshade), 8),
            new LootPackItem(typeof(SpidersSilk), 6),
        };

        // Verdantpeak Resources
        public static readonly LootPackItem[] VerdantpeakResources = new[]
        {
            new LootPackItem(typeof(LivingOre), 15),           // Phase 1 - Regional ore
            new LootPackItem(typeof(NatureforgedIngot), 8),    // Phase 1 - Regional ingot
            new LootPackItem(typeof(LivingwoodLog), 10),       // Phase 1 - Regional wood
            new LootPackItem(typeof(LivingBark), 8),           // Phase 2 - Nature reagent
            new LootPackItem(typeof(TreantHeart), 2),          // Phase 2 - Special resource (rare)
            new LootPackItem(typeof(Emerald), 5),
            new LootPackItem(typeof(Ginseng), 8),
            new LootPackItem(typeof(Bloodmoss), 6),
        };

        // Desert Resources
        public static readonly LootPackItem[] DesertResources = new[]
        {
            new LootPackItem(typeof(SandstoneOre), 15),        // Phase 1 - Regional ore
            new LootPackItem(typeof(SunforgedIngot), 8),       // Phase 1 - Regional ingot
            new LootPackItem(typeof(PetrifiedWoodLog), 10),    // Phase 1 - Regional wood
            // REMOVED OLD REAGENT: new LootPackItem(typeof(DesertRose), 6),           // Phase 2 - Nature reagent
            new LootPackItem(typeof(Citrine), 5),
            new LootPackItem(typeof(Amber), 5),
        };

        // Ironclad Empire Resources
        public static readonly LootPackItem[] IroncladResources = new[]
        {
            new LootPackItem(typeof(SteamworkOre), 15),        // Phase 1 - Regional ore
            new LootPackItem(typeof(ClockworkIngot), 8),       // Phase 1 - Regional ingot
            new LootPackItem(typeof(IronwoodLog), 10),         // Phase 1 - Regional wood
            new LootPackItem(typeof(ClockworkGear), 10),       // Phase 2 - Mechanical component
            new LootPackItem(typeof(ClockworkSpring), 6),      // Phase 2 - Mechanical component
            new LootPackItem(typeof(SteamCore), 2),            // Phase 2 - Mechanical component (rare)
        };

        // Obsidian Wastes Resources
        public static readonly LootPackItem[] ObsidianResources = new[]
        {
            new LootPackItem(typeof(ObsidianOre), 15),         // Phase 1 - Regional ore
            new LootPackItem(typeof(VoidforgedIngot), 8),      // Phase 1 - Regional ingot
            new LootPackItem(typeof(VoidDust), 8),             // Phase 2 - Elemental reagent
            new LootPackItem(typeof(StormCrystal), 4),         // Phase 2 - Elemental reagent
            // REMOVED OLD REAGENT: new LootPackItem(typeof(DragonScalePowder), 1),    // Phase 2 - Exotic reagent (very rare)
            new LootPackItem(typeof(Amethyst), 5),
        };

        #endregion

        #region Boss Loot Items

        // Frost Father specific drops
        public static readonly LootPackItem[] FrostFatherBossItems = new[]
        {
            new LootPackItem(typeof(FrozenArtifact), 100),
            new LootPackItem(typeof(HeartwoodCoreFragment), 1),  // 1% base chance
            new LootPackItem(typeof(TheEternalWinter), 2),       // 2% base chance
        };

        #endregion

        #region Regional Creature Loot Packs

        /// <summary>
        /// Frosthold - Poor creatures (Frost Wolves, Ice Snakes)
        /// </summary>
        public static readonly LootPack FrostholdPoor = new LootPack(
            new[]
            {
                new LootPackEntry(true, LootPack.Gold, 100.00, "2d10+20"),
                new LootPackEntry(false, FrostholdResources, 5.00, 1),
                new LootPackEntry(false, LootPack.GemItems, 1.00, 1),
            });

        /// <summary>
        /// Frosthold - Average creatures (Ice Elementals, Frost Drakes)
        /// </summary>
        public static readonly LootPack FrostholdAverage = new LootPack(
            new[]
            {
                new LootPackEntry(true, LootPack.Gold, 100.00, "8d10+100"),
                new LootPackEntry(false, FrostholdResources, 15.00, 1),
                new LootPackEntry(false, LootPack.AosMagicItemsAverageType1, 32.80, 1, 3, 0, 50),
                new LootPackEntry(false, LootPack.GemItems, 5.00, "1d3"),
            });

        /// <summary>
        /// Frosthold - Rich creatures (Ancient White Dragons, Frost Giants)
        /// </summary>
        public static readonly LootPack FrostholdRich = new LootPack(
            new[]
            {
                new LootPackEntry(true, LootPack.Gold, 100.00, "15d10+225"),
                new LootPackEntry(false, FrostholdResources, 30.00, "1d2"),
                new LootPackEntry(false, LootPack.AosMagicItemsRichType1, 76.30, 1, 4, 0, 75),
                new LootPackEntry(false, LootPack.AosMagicItemsRichType1, 76.30, 1, 4, 0, 75),
                new LootPackEntry(false, LootPack.GemItems, 10.00, "1d5"),
            });

        /// <summary>
        /// Frosthold - Boss (Frost Father)
        /// </summary>
        public static readonly LootPack FrostholdBoss = new LootPack(
            new[]
            {
                new LootPackEntry(true, LootPack.Gold, 100.00, "10d100+800"),
                new LootPackEntry(false, FrostFatherBossItems, 100.00, "1d3"),
                new LootPackEntry(false, LootPack.AosMagicItemsUltraRich, 100.00, 1, 5, 25, 100),
                new LootPackEntry(false, LootPack.AosMagicItemsUltraRich, 100.00, 1, 5, 25, 100),
                new LootPackEntry(false, LootPack.AosMagicItemsUltraRich, 100.00, 1, 5, 33, 100),
                new LootPackEntry(false, LootPack.GemItems, 100.00, "3d4"),
                new LootPackEntry(false, LootPack.HighScrollItems, 100.00, "1d3"),
            });

        /// <summary>
        /// Emberlands - Poor creatures (Fire Newts, Ember Imps)
        /// </summary>
        public static readonly LootPack EmberlandsPoor = new LootPack(
            new[]
            {
                new LootPackEntry(true, LootPack.Gold, 100.00, "2d10+20"),
                new LootPackEntry(false, EmberlandsResources, 5.00, 1),
                new LootPackEntry(false, LootPack.GemItems, 1.00, 1),
            });

        /// <summary>
        /// Emberlands - Average creatures (Fire Elementals, Lava Serpents)
        /// </summary>
        public static readonly LootPack EmberlandsAverage = new LootPack(
            new[]
            {
                new LootPackEntry(true, LootPack.Gold, 100.00, "8d10+100"),
                new LootPackEntry(false, EmberlandsResources, 15.00, 1),
                new LootPackEntry(false, LootPack.AosMagicItemsAverageType1, 32.80, 1, 3, 0, 50),
                new LootPackEntry(false, LootPack.GemItems, 5.00, "1d3"),
            });

        /// <summary>
        /// Emberlands - Rich creatures (Ancient Red Dragons, Fire Lords)
        /// </summary>
        public static readonly LootPack EmberlandsRich = new LootPack(
            new[]
            {
                new LootPackEntry(true, LootPack.Gold, 100.00, "15d10+225"),
                new LootPackEntry(false, EmberlandsResources, 30.00, "1d2"),
                new LootPackEntry(false, LootPack.AosMagicItemsRichType1, 76.30, 1, 4, 0, 75),
                new LootPackEntry(false, LootPack.AosMagicItemsRichType1, 76.30, 1, 4, 0, 75),
                new LootPackEntry(false, LootPack.GemItems, 10.00, "1d5"),
            });

        /// <summary>
        /// Crystal Barrens - Poor creatures (Crystal Beetles, Shard Spiders)
        /// </summary>
        public static readonly LootPack CrystalPoor = new LootPack(
            new[]
            {
                new LootPackEntry(true, LootPack.Gold, 100.00, "2d10+20"),
                new LootPackEntry(false, CrystalResources, 5.00, 1),
                new LootPackEntry(false, LootPack.GemItems, 3.00, 1),  // Higher gem chance in crystal region
            });

        /// <summary>
        /// Crystal Barrens - Average creatures (Crystal Elementals, Prismatic Drakes)
        /// </summary>
        public static readonly LootPack CrystalAverage = new LootPack(
            new[]
            {
                new LootPackEntry(true, LootPack.Gold, 100.00, "8d10+100"),
                new LootPackEntry(false, CrystalResources, 15.00, 1),
                new LootPackEntry(false, LootPack.AosMagicItemsAverageType1, 32.80, 1, 3, 0, 50),
                new LootPackEntry(false, LootPack.GemItems, 10.00, "1d4"),  // Higher gem chance
            });

        /// <summary>
        /// Crystal Barrens - Rich creatures (Ancient Crystal Dragons, Ley Guardians)
        /// </summary>
        public static readonly LootPack CrystalRich = new LootPack(
            new[]
            {
                new LootPackEntry(true, LootPack.Gold, 100.00, "15d10+225"),
                new LootPackEntry(false, CrystalResources, 30.00, "1d2"),
                new LootPackEntry(false, LootPack.AosMagicItemsRichType1, 76.30, 1, 4, 0, 75),
                new LootPackEntry(false, LootPack.AosMagicItemsRichType1, 76.30, 1, 4, 0, 75),
                new LootPackEntry(false, LootPack.GemItems, 20.00, "1d6"),  // Higher gem chance
            });

        /// <summary>
        /// Shadowfen - Poor creatures (Swamp Leeches, Bog Toads)
        /// </summary>
        public static readonly LootPack ShadowfenPoor = new LootPack(
            new[]
            {
                new LootPackEntry(true, LootPack.Gold, 100.00, "2d10+20"),
                new LootPackEntry(false, ShadowfenResources, 5.00, 1),
                new LootPackEntry(false, LootPack.GemItems, 1.00, 1),
            });

        /// <summary>
        /// Shadowfen - Average creatures (Bog Things, Plague Beasts)
        /// </summary>
        public static readonly LootPack ShadowfenAverage = new LootPack(
            new[]
            {
                new LootPackEntry(true, LootPack.Gold, 100.00, "8d10+100"),
                new LootPackEntry(false, ShadowfenResources, 15.00, 1),
                new LootPackEntry(false, LootPack.AosMagicItemsAverageType1, 32.80, 1, 3, 0, 50),
                new LootPackEntry(false, LootPack.PotionItems, 10.00, "1d2"),  // Poison potions
            });

        /// <summary>
        /// Shadowfen - Rich creatures (Ancient Swamp Dragons, Bog Lords)
        /// </summary>
        public static readonly LootPack ShadowfenRich = new LootPack(
            new[]
            {
                new LootPackEntry(true, LootPack.Gold, 100.00, "15d10+225"),
                new LootPackEntry(false, ShadowfenResources, 30.00, "1d2"),
                new LootPackEntry(false, LootPack.AosMagicItemsRichType1, 76.30, 1, 4, 0, 75),
                new LootPackEntry(false, LootPack.AosMagicItemsRichType1, 76.30, 1, 4, 0, 75),
                new LootPackEntry(false, LootPack.PotionItems, 20.00, "1d3"),
            });

        /// <summary>
        /// Verdantpeak - Poor creatures (Forest Wolves, Woodland Sprites)
        /// </summary>
        public static readonly LootPack VerdantpeakPoor = new LootPack(
            new[]
            {
                new LootPackEntry(true, LootPack.Gold, 100.00, "2d10+20"),
                new LootPackEntry(false, VerdantpeakResources, 5.00, 1),
                new LootPackEntry(false, LootPack.GemItems, 1.00, 1),
            });

        /// <summary>
        /// Verdantpeak - Average creatures (Treants, Forest Drakes)
        /// </summary>
        public static readonly LootPack VerdantpeakAverage = new LootPack(
            new[]
            {
                new LootPackEntry(true, LootPack.Gold, 100.00, "8d10+100"),
                new LootPackEntry(false, VerdantpeakResources, 15.00, 1),
                new LootPackEntry(false, LootPack.AosMagicItemsAverageType1, 32.80, 1, 3, 0, 50),
                new LootPackEntry(false, LootPack.GemItems, 5.00, "1d3"),
            });

        /// <summary>
        /// Verdantpeak - Rich creatures (Ancient Treants, Forest Guardians)
        /// </summary>
        public static readonly LootPack VerdantpeakRich = new LootPack(
            new[]
            {
                new LootPackEntry(true, LootPack.Gold, 100.00, "15d10+225"),
                new LootPackEntry(false, VerdantpeakResources, 30.00, "1d2"),
                new LootPackEntry(false, LootPack.AosMagicItemsRichType1, 76.30, 1, 4, 0, 75),
                new LootPackEntry(false, LootPack.AosMagicItemsRichType1, 76.30, 1, 4, 0, 75),
                new LootPackEntry(false, LootPack.GemItems, 10.00, "1d5"),
            });

        /// <summary>
        /// Desert - Poor creatures (Scorpions, Sand Serpents)
        /// </summary>
        public static readonly LootPack DesertPoor = new LootPack(
            new[]
            {
                new LootPackEntry(true, LootPack.Gold, 100.00, "2d10+20"),
                new LootPackEntry(false, DesertResources, 5.00, 1),
                new LootPackEntry(false, LootPack.GemItems, 1.00, 1),
            });

        /// <summary>
        /// Desert - Average creatures (Sand Elementals, Desert Drakes)
        /// </summary>
        public static readonly LootPack DesertAverage = new LootPack(
            new[]
            {
                new LootPackEntry(true, LootPack.Gold, 100.00, "8d10+100"),
                new LootPackEntry(false, DesertResources, 15.00, 1),
                new LootPackEntry(false, LootPack.AosMagicItemsAverageType1, 32.80, 1, 3, 0, 50),
                new LootPackEntry(false, LootPack.GemItems, 5.00, "1d3"),
            });

        /// <summary>
        /// Desert - Rich creatures (Sand Wurms, Djinni)
        /// </summary>
        public static readonly LootPack DesertRich = new LootPack(
            new[]
            {
                new LootPackEntry(true, LootPack.Gold, 100.00, "15d10+225"),
                new LootPackEntry(false, DesertResources, 30.00, "1d2"),
                new LootPackEntry(false, LootPack.AosMagicItemsRichType1, 76.30, 1, 4, 0, 75),
                new LootPackEntry(false, LootPack.AosMagicItemsRichType1, 76.30, 1, 4, 0, 75),
                new LootPackEntry(false, LootPack.GemItems, 10.00, "1d5"),
            });

        /// <summary>
        /// Ironclad Empire - Poor creatures (Clockwork Beetles, Steam Mephits)
        /// </summary>
        public static readonly LootPack IroncladPoor = new LootPack(
            new[]
            {
                new LootPackEntry(true, LootPack.Gold, 100.00, "2d10+20"),
                new LootPackEntry(false, IroncladResources, 5.00, 1),
                new LootPackEntry(false, LootPack.GemItems, 1.00, 1),
            });

        /// <summary>
        /// Ironclad Empire - Average creatures (Clockwork Golems, Steam Elementals)
        /// </summary>
        public static readonly LootPack IroncladAverage = new LootPack(
            new[]
            {
                new LootPackEntry(true, LootPack.Gold, 100.00, "8d10+100"),
                new LootPackEntry(false, IroncladResources, 15.00, 1),
                new LootPackEntry(false, LootPack.AosMagicItemsAverageType1, 32.80, 1, 3, 0, 50),
                new LootPackEntry(false, LootPack.GemItems, 5.00, "1d3"),
            });

        /// <summary>
        /// Ironclad Empire - Rich creatures (Clockwork Titans, Forge Masters)
        /// </summary>
        public static readonly LootPack IroncladRich = new LootPack(
            new[]
            {
                new LootPackEntry(true, LootPack.Gold, 100.00, "15d10+225"),
                new LootPackEntry(false, IroncladResources, 30.00, "1d2"),
                new LootPackEntry(false, LootPack.AosMagicItemsRichType1, 76.30, 1, 4, 0, 75),
                new LootPackEntry(false, LootPack.AosMagicItemsRichType1, 76.30, 1, 4, 0, 75),
                new LootPackEntry(false, LootPack.GemItems, 10.00, "1d5"),
            });

        /// <summary>
        /// Obsidian Wastes - Poor creatures (Void Creepers)
        /// </summary>
        public static readonly LootPack ObsidianPoor = new LootPack(
            new[]
            {
                new LootPackEntry(true, LootPack.Gold, 100.00, "2d10+20"),
                new LootPackEntry(false, ObsidianResources, 5.00, 1),
                new LootPackEntry(false, LootPack.GemItems, 1.00, 1),
            });

        /// <summary>
        /// Obsidian Wastes - Average creatures (Shadow Wraiths, Void Elementals)
        /// </summary>
        public static readonly LootPack ObsidianAverage = new LootPack(
            new[]
            {
                new LootPackEntry(true, LootPack.Gold, 100.00, "8d10+100"),
                new LootPackEntry(false, ObsidianResources, 15.00, 1),
                new LootPackEntry(false, LootPack.AosMagicItemsAverageType1, 32.80, 1, 3, 0, 50),
                new LootPackEntry(false, LootPack.GemItems, 5.00, "1d3"),
            });

        /// <summary>
        /// Obsidian Wastes - Rich creatures (Void Lords, Void Avatars)
        /// </summary>
        public static readonly LootPack ObsidianRich = new LootPack(
            new[]
            {
                new LootPackEntry(true, LootPack.Gold, 100.00, "15d10+225"),
                new LootPackEntry(false, ObsidianResources, 30.00, "1d2"),
                new LootPackEntry(false, LootPack.AosMagicItemsRichType1, 76.30, 1, 4, 0, 75),
                new LootPackEntry(false, LootPack.AosMagicItemsRichType1, 76.30, 1, 4, 0, 75),
                new LootPackEntry(false, LootPack.GemItems, 10.00, "1d5"),
            });

        #endregion

        #region Generic Boss Packs

        /// <summary>
        /// Generic Vystia boss loot (for bosses without specific item drops)
        /// </summary>
        public static readonly LootPack VystiaBoss = new LootPack(
            new[]
            {
                new LootPackEntry(true, LootPack.Gold, 100.00, "10d100+800"),
                new LootPackEntry(false, LootPack.AosMagicItemsUltraRich, 100.00, 1, 5, 25, 100),
                new LootPackEntry(false, LootPack.AosMagicItemsUltraRich, 100.00, 1, 5, 25, 100),
                new LootPackEntry(false, LootPack.AosMagicItemsUltraRich, 100.00, 1, 5, 33, 100),
                new LootPackEntry(false, LootPack.AosMagicItemsUltraRich, 100.00, 1, 5, 33, 100),
                new LootPackEntry(false, LootPack.GemItems, 100.00, "3d4"),
                new LootPackEntry(false, LootPack.HighScrollItems, 100.00, "1d3"),
            });

        /// <summary>
        /// Mini-boss loot (dungeon mini-bosses, rare spawns)
        /// </summary>
        public static readonly LootPack VystiaMiniBoss = new LootPack(
            new[]
            {
                new LootPackEntry(true, LootPack.Gold, 100.00, "5d100+500"),
                new LootPackEntry(false, LootPack.AosMagicItemsFilthyRichType1, 79.50, 1, 5, 0, 100),
                new LootPackEntry(false, LootPack.AosMagicItemsFilthyRichType1, 79.50, 1, 5, 0, 100),
                new LootPackEntry(false, LootPack.AosMagicItemsFilthyRichType2, 77.60, 1, 5, 25, 100),
                new LootPackEntry(false, LootPack.GemItems, 50.00, "2d3"),
                new LootPackEntry(false, LootPack.MedScrollItems, 50.00, "1d2"),
            });

        #endregion

        #region Helper Methods

        /// <summary>
        /// Gets the appropriate loot pack for a Frosthold creature based on strength.
        /// </summary>
        public static LootPack GetFrostholdPack(int strength)
        {
            if (strength >= 500)
                return FrostholdRich;
            if (strength >= 200)
                return FrostholdAverage;
            return FrostholdPoor;
        }

        /// <summary>
        /// Gets the appropriate loot pack for an Emberlands creature based on strength.
        /// </summary>
        public static LootPack GetEmberlandsPack(int strength)
        {
            if (strength >= 500)
                return EmberlandsRich;
            if (strength >= 200)
                return EmberlandsAverage;
            return EmberlandsPoor;
        }

        /// <summary>
        /// Gets the appropriate loot pack for a Crystal Barrens creature based on strength.
        /// </summary>
        public static LootPack GetCrystalPack(int strength)
        {
            if (strength >= 500)
                return CrystalRich;
            if (strength >= 200)
                return CrystalAverage;
            return CrystalPoor;
        }

        /// <summary>
        /// Gets the appropriate loot pack for a Shadowfen creature based on strength.
        /// </summary>
        public static LootPack GetShadowfenPack(int strength)
        {
            if (strength >= 500)
                return ShadowfenRich;
            if (strength >= 200)
                return ShadowfenAverage;
            return ShadowfenPoor;
        }

        /// <summary>
        /// Gets the appropriate loot pack for a Verdantpeak creature based on strength.
        /// </summary>
        public static LootPack GetVerdantpeakPack(int strength)
        {
            if (strength >= 500)
                return VerdantpeakRich;
            if (strength >= 200)
                return VerdantpeakAverage;
            return VerdantpeakPoor;
        }

        /// <summary>
        /// Gets the appropriate loot pack for a Desert creature based on strength.
        /// </summary>
        public static LootPack GetDesertPack(int strength)
        {
            if (strength >= 500)
                return DesertRich;
            if (strength >= 200)
                return DesertAverage;
            return DesertPoor;
        }

        /// <summary>
        /// Gets the appropriate loot pack for an Ironclad Empire creature based on strength.
        /// </summary>
        public static LootPack GetIroncladPack(int strength)
        {
            if (strength >= 500)
                return IroncladRich;
            if (strength >= 200)
                return IroncladAverage;
            return IroncladPoor;
        }

        /// <summary>
        /// Gets the appropriate loot pack for an Obsidian Wastes creature based on strength.
        /// </summary>
        public static LootPack GetObsidianPack(int strength)
        {
            if (strength >= 500)
                return ObsidianRich;
            if (strength >= 200)
                return ObsidianAverage;
            return ObsidianPoor;
        }

        #endregion
    }
}
