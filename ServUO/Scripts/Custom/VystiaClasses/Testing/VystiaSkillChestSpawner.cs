using System;
using System.Collections.Generic;
using Server;
using Server.Items;
using Server.Items.VystiaClassItems;
using Server.Mobiles;
using Server.Custom.VystiaClasses.Abilities;
using Server.Items.Vystia;
using Server.Engines.Craft;

namespace Server.Custom.VystiaClasses.Testing
{
    /// <summary>
    /// Helper class to spawn individual skill/school chests for VTK
    /// </summary>
    public static class VystiaSkillChestSpawner
    {
        // Button ID ranges (avoiding conflicts with existing 200-202 bulk spawn)
        public const int CRAFTING_START = 600;
        public const int MAGIC_START = 700;
        public const int MARTIAL_START = 800;

        // Vystia Crafting skill IDs
        public enum CraftingSkill
        {
            Engineering = 0,      // Artificer - SkillName.Engineering (ID 80)
            Transmutation = 1,    // Alchemist - SkillName.Transmutation (ID 81)
            Clothcraft = 2,       // Bard - SkillName.Tailoring
            Leathercraft = 3,     // Ranger - SkillName.Tailoring
            Woodshaping = 4,      // Druid - SkillName.Carpentry
            VystiaInscription = 5, // Oracle - SkillName.Inscribe
            Jewelcraft = 6,       // Sorcerer - SkillName.Tinkering
            Necrocraft = 7,       // Necromancer - SkillName.Necromancy (ID 64)
            Runecrafting = 8      // Enchanter - SkillName.Runeweaving (ID 68)
        }

        /// <summary>
        /// Spawns a chest for a crafting skill with tool, components, and station
        /// </summary>
        public static Container SpawnCraftingSkillChest(CraftingSkill skill, Point3D loc, Map map)
        {
            Container chest = CreateChest(GetCraftingSkillName(skill), GetCraftingSkillHue(skill), loc, map);

            switch (skill)
            {
                case CraftingSkill.Engineering: // Artificer - SkillName.Engineering (ID 80)
                    chest.DropItem(new EngineeringToolKit());
                    chest.DropItem(new Bottle(500)); // Bottles for crafting explosives
                    chest.DropItem(new ClockworkIngot(1000)); // Extra ingots for crafting SteamCores
                    chest.DropItem(new ClockworkGear(500));
                    chest.DropItem(new ClockworkSpring(500));
                    chest.DropItem(new Server.Items.SteamCore(500)); // Now uses stackable item ID (0x0F8E)
                    chest.DropItem(new VystiaCraftingStone("Tinkering Table", 0x1EB9, 0x3B2));
                    break;

                case CraftingSkill.Transmutation: // Alchemist - SkillName.Transmutation (ID 81)
                    chest.DropItem(new MortarPestle());
                    chest.DropItem(new Bottle(500));
                    // Nature Magic reagents (Druid)
                    chest.DropItem(new WildMoss(500));
                    chest.DropItem(new Moonpetal(500));
                    chest.DropItem(new DruidBark(500));
                    chest.DropItem(new TreantSap(500));
                    // Hex Magic reagents (Witch)
                    chest.DropItem(new BogMoss(500));
                    chest.DropItem(new ViperFang(500));
                    chest.DropItem(new Witchweed(500));
                    chest.DropItem(new ToadsEye(500));
                    chest.DropItem(new VystiaCraftingStone("Alchemy Table", 0x1E9D, 0x3B2));
                    break;

                case CraftingSkill.Clothcraft: // Bard - SkillName.Tailoring
                    chest.DropItem(new SewingKit());
                    chest.DropItem(new Cloth(500));
                    chest.DropItem(new VystiaCraftingStone("Tailoring Table", 0xF9C, 0x3B2));
                    break;

                case CraftingSkill.Leathercraft: // Ranger - SkillName.Tailoring
                    chest.DropItem(new SewingKit());
                    chest.DropItem(new Leather(500));
                    chest.DropItem(new VystiaCraftingStone("Tailoring Table", 0xF9C, 0x3B2));
                    break;

                case CraftingSkill.Woodshaping: // Druid - SkillName.Carpentry
                    chest.DropItem(new DovetailSaw());
                    chest.DropItem(new Board(500));
                    chest.DropItem(new VystiaCraftingStone("Carpentry Bench", 0x1E7F, 0x3B2));
                    break;

                case CraftingSkill.VystiaInscription: // Oracle - SkillName.Inscribe
                    chest.DropItem(new ScribesPen());
                    chest.DropItem(new BlankScroll(500));
                    // Divination Magic reagents
                    chest.DropItem(new TimeSand(500));
                    chest.DropItem(new TimeDust(500));
                    chest.DropItem(new DivinationDust(500));
                    chest.DropItem(new FateCrystal(500));
                    break;

                case CraftingSkill.Jewelcraft: // Sorcerer - SkillName.Tinkering
                    chest.DropItem(new TinkerTools());
                    chest.DropItem(new IronIngot(500));
                    chest.DropItem(new GoldIngot(500));
                    chest.DropItem(new VystiaCraftingStone("Tinkering Table", 0x1EB9, 0x3B2));
                    break;

                case CraftingSkill.Necrocraft: // Necromancer - SkillName.Necromancy (ID 64)
                    chest.DropItem(new MortarPestle());
                    // Bones for crafting bone items
                    chest.DropItem(new Bone(500));
                    // Blank scrolls for soul vessels
                    chest.DropItem(new BlankScroll(500));
                    // Necromancy reagents
                    chest.DropItem(new GraveMoss(500));
                    chest.DropItem(new BoneDust(500));
                    chest.DropItem(new CorpseAsh(500));
                    chest.DropItem(new SoulFragment(500));
                    // Additional necromancy reagents for higher-tier crafting
                    chest.DropItem(new NecroticShroud(500));
                    chest.DropItem(new LichDust(500));
                    chest.DropItem(new PhylacteryShard(500));
                    chest.DropItem(new ReaperEssence(500));
                    chest.DropItem(new VystiaCraftingStone("Necromantic Altar", 0x1E9D, 0x455)); // Dark purple hue for necromancy
                    break;

                case CraftingSkill.Runecrafting: // Enchanter - SkillName.Runeweaving (ID 68)
                    chest.DropItem(new ScribesPen());
                    // Enchanting Magic reagents
                    chest.DropItem(new ArcaneDust(500));
                    chest.DropItem(new EssenceOfMagic(500));
                    chest.DropItem(new ManaCrystal(500));
                    chest.DropItem(new LeyLineEssence(500));
                    chest.DropItem(new BlankScroll(500));
                    break;
            }

            return chest;
        }

        /// <summary>
        /// Spawns a chest for a magic school with full spellbook and 500 of each reagent
        /// </summary>
        public static Container SpawnMagicSchoolChest(AbilitySchool school, Point3D loc, Map map)
        {
            Container chest = CreateChest(GetMagicSchoolName(school), GetMagicSchoolHue(school), loc, map);

            // Add full spellbook
            Item spellbook = GetSpellbookForSchool(school);
            if (spellbook != null)
                chest.DropItem(spellbook);

            // Add 500 of each reagent for this school
            AddReagentsToChest(chest, school, 500);

            return chest;
        }

        /// <summary>
        /// Spawns a chest for a martial school with starting weapon and ability book
        /// </summary>
        public static Container SpawnMartialSchoolChest(AbilitySchool school, Point3D loc, Map map)
        {
            Container chest = CreateChest(GetMartialSchoolName(school), GetMartialSchoolHue(school), loc, map);

            // Add starting weapon(s)
            Item[] weapons = GetStartingWeaponsForSchool(school);
            foreach (Item weapon in weapons)
            {
                if (weapon != null)
                    chest.DropItem(weapon);
            }

            // Add ability book if applicable (some martial classes have spellbooks too)
            Spellbook abilityBook = GetAbilityBookForSchool(school);
            if (abilityBook != null)
                chest.DropItem(abilityBook);

            return chest;
        }

        #region Helper Methods

        private static Container CreateChest(string name, int hue, Point3D loc, Map map)
        {
            Item table = new Item(0x0B8F);
            table.Name = "VTK Table: " + name;
            table.Movable = false;
            table.MoveToWorld(new Point3D(loc.X, loc.Y, loc.Z), map);

            Container chest = new WoodenChest();
            chest.Name = name;
            chest.Hue = hue;
            chest.Movable = false;
            chest.MoveToWorld(new Point3D(loc.X, loc.Y, loc.Z + 6), map);

            return chest;
        }

        public static string GetCraftingSkillName(CraftingSkill skill)
        {
            switch (skill)
            {
                case CraftingSkill.Engineering: return "Engineering Kit (Artificer)";
                case CraftingSkill.Transmutation: return "Transmutation Kit (Alchemist)";
                case CraftingSkill.Clothcraft: return "Clothcraft Kit (Bard)";
                case CraftingSkill.Leathercraft: return "Leathercraft Kit (Ranger)";
                case CraftingSkill.Woodshaping: return "Woodshaping Kit (Druid)";
                case CraftingSkill.VystiaInscription: return "Vystia Inscription Kit (Oracle)";
                case CraftingSkill.Jewelcraft: return "Jewelcraft Kit (Sorcerer)";
                case CraftingSkill.Necrocraft: return "Necrocraft Kit (Necromancer)";
                case CraftingSkill.Runecrafting: return "Runecrafting Kit (Enchanter)";
                default: return "Unknown Vystia Crafting Skill";
            }
        }

        private static int GetCraftingSkillHue(CraftingSkill skill)
        {
            return 0x3B2; // Default gold
        }

        public static string GetMagicSchoolName(AbilitySchool school)
        {
            switch (school)
            {
                case AbilitySchool.Ice: return "Ice Magic Kit";
                case AbilitySchool.Nature: return "Nature Magic Kit";
                case AbilitySchool.Hex: return "Hex Magic Kit";
                case AbilitySchool.Elemental: return "Elemental Magic Kit";
                case AbilitySchool.Dark: return "Dark Magic Kit";
                case AbilitySchool.Divination: return "Divination Magic Kit";
                case AbilitySchool.Necromancy: return "Necromancy Kit";
                case AbilitySchool.Summoning: return "Summoning Magic Kit";
                case AbilitySchool.Shamanic: return "Shamanic Magic Kit";
#if VYSTIA_SONGWEAVING
                case AbilitySchool.Bardic: return "Songweaving Kit";
#endif
                case AbilitySchool.Enchanting: return "Enchanting Magic Kit";
                case AbilitySchool.Illusion: return "Illusion Magic Kit";
                default: return "Unknown Magic School";
            }
        }

        private static int GetMagicSchoolHue(AbilitySchool school)
        {
            switch (school)
            {
                case AbilitySchool.Ice: return 0x481; // Ice blue
                case AbilitySchool.Nature: return 0x7D6; // Forest green
                case AbilitySchool.Hex: return 0x81D; // Murky green/purple
                case AbilitySchool.Elemental: return 0x54E; // Fiery orange
                case AbilitySchool.Dark: return 0x455; // Void black/purple
                case AbilitySchool.Divination: return 0x482; // Crystal blue
                case AbilitySchool.Necromancy: return 0x455; // Void black
                case AbilitySchool.Summoning: return 0x3B2; // Gold
                case AbilitySchool.Shamanic: return 0x7D6; // Green
#if VYSTIA_SONGWEAVING
                case AbilitySchool.Bardic: return 0x3B2; // Gold
#endif
                case AbilitySchool.Enchanting: return 0x3B2; // Gold
                case AbilitySchool.Illusion: return 0x3B2; // Gold
                default: return 0x3B2;
            }
        }

        public static string GetMartialSchoolName(AbilitySchool school)
        {
            switch (school)
            {
                case AbilitySchool.Fighter: return "Fighter Kit";
                case AbilitySchool.Barbarian: return "Barbarian Kit";
                case AbilitySchool.Monk: return "Monk Kit";
                case AbilitySchool.Rogue: return "Rogue Kit";
                case AbilitySchool.Ranger: return "Ranger Kit";
                case AbilitySchool.Knight: return "Knight Kit";
                case AbilitySchool.Paladin: return "Paladin Kit";
                case AbilitySchool.Templar: return "Templar Kit";
                case AbilitySchool.BountyHunter: return "Bounty Hunter Kit";
                case AbilitySchool.Beastmaster: return "Beastmaster Kit";
                case AbilitySchool.Artificer: return "Artificer Kit";
                case AbilitySchool.Alchemist: return "Alchemist Kit";
                case AbilitySchool.Cleric: return "Cleric Kit";
                case AbilitySchool.Wizard: return "Wizard Kit";
                default: return "Unknown Martial School";
            }
        }

        private static int GetMartialSchoolHue(AbilitySchool school)
        {
            switch (school)
            {
                case AbilitySchool.Fighter: return 2305;
                case AbilitySchool.Barbarian: return 0x26; // Blood red
                case AbilitySchool.Monk: return 2305;
                case AbilitySchool.Rogue: return 1109;
                case AbilitySchool.Ranger: return 1719;
                case AbilitySchool.Knight: return 2305;
                case AbilitySchool.Paladin: return 1150; // Light grey
                case AbilitySchool.Templar: return 1150; // Light grey
                case AbilitySchool.BountyHunter: return 1719;
                case AbilitySchool.Beastmaster: return 0x7D6; // Dark green
                case AbilitySchool.Artificer: return 2305;
                case AbilitySchool.Alchemist: return 2010;
                case AbilitySchool.Cleric: return 1153;
                case AbilitySchool.Wizard: return 1154;
                default: return 0x3B2;
            }
        }

        private static Item GetSpellbookForSchool(AbilitySchool school)
        {
            switch (school)
            {
                case AbilitySchool.Ice: return new IceMageSpellbook();
                case AbilitySchool.Nature: return new DruidSpellbook();
                case AbilitySchool.Hex: return new WitchSpellbook();
                case AbilitySchool.Elemental: return new SorcererSpellbook();
                case AbilitySchool.Dark: return new WarlockSpellbook();
                case AbilitySchool.Divination: return new OracleSpellbook();
                case AbilitySchool.Necromancy: return new VystiaNecromancerSpellbook();
                case AbilitySchool.Summoning: return new SummonerSpellbook();
                case AbilitySchool.Shamanic: return new ShamanSpellbook();
#if VYSTIA_SONGWEAVING
                case AbilitySchool.Bardic: return new SongweavingSpellbook();
#endif
                case AbilitySchool.Enchanting: return new EnchanterSpellbook();
                case AbilitySchool.Illusion: return new IllusionistSpellbook();
                default: return null;
            }
        }

        private static void AddReagentsToChest(Container chest, AbilitySchool school, int amount)
        {
            switch (school)
            {
                case AbilitySchool.Ice:
                    chest.DropItem(new Frostbloom(amount));
                    chest.DropItem(new GlacierCrystal(amount));
                    chest.DropItem(new Winterleaf(amount));
                    chest.DropItem(new PermafrostEssence(amount));
                    chest.DropItem(new ArcticPearl(amount));
                    chest.DropItem(new FrozenSoul(amount));
                    chest.DropItem(new FrostEssence(amount));
                    chest.DropItem(new Server.Items.HeartOfWinter(amount));
                    break;

                case AbilitySchool.Nature:
                    chest.DropItem(new WildMoss(amount));
                    chest.DropItem(new Moonpetal(amount));
                    chest.DropItem(new DruidBark(amount));
                    chest.DropItem(new TreantSap(amount));
                    chest.DropItem(new ElderwoodSeed(amount));
                    chest.DropItem(new PrimalVine(amount));
                    chest.DropItem(new LivingBark(amount));
                    chest.DropItem(new AncientRoot(amount));
                    break;

                case AbilitySchool.Hex:
                    chest.DropItem(new BogMoss(amount));
                    chest.DropItem(new ViperFang(amount));
                    chest.DropItem(new Witchweed(amount));
                    chest.DropItem(new ToadsEye(amount));
                    chest.DropItem(new SwampLotus(amount));
                    chest.DropItem(new HagsHair(amount));
                    chest.DropItem(new CursedPearl(amount));
                    chest.DropItem(new CursedSalt(amount));
                    break;

                case AbilitySchool.Elemental:
                    chest.DropItem(new AshPetal(amount));
                    chest.DropItem(new LavaGlass(amount));
                    chest.DropItem(new Flameweed(amount));
                    chest.DropItem(new MagmaEssence(amount));
                    chest.DropItem(new PhoenixFeather(amount));
                    chest.DropItem(new DragonHeart(amount));
                    chest.DropItem(new PrimordialEmber(amount));
                    chest.DropItem(new ElementalCore(amount));
                    break;

                case AbilitySchool.Dark:
                    chest.DropItem(new ShadowMoss(amount));
                    chest.DropItem(new VoidCrystal(amount));
                    chest.DropItem(new VoidWeed(amount));
                    chest.DropItem(new ShadowPetal(amount));
                    chest.DropItem(new VoidDust(amount));
                    chest.DropItem(new VoidSilk(amount));
                    chest.DropItem(new DemonHeart(amount));
                    chest.DropItem(new ShadowEssence(amount));
                    break;

                case AbilitySchool.Divination:
                    chest.DropItem(new TimeSand(amount));
                    chest.DropItem(new TimeDust(amount));
                    chest.DropItem(new DivinationDust(amount));
                    chest.DropItem(new FateCrystal(amount));
                    chest.DropItem(new StarlightCrystal(amount));
                    chest.DropItem(new PropheticLeaf(amount));
                    chest.DropItem(new SeeingStone(amount));
                    chest.DropItem(new FateThread(amount));
                    break;

                case AbilitySchool.Necromancy:
                    chest.DropItem(new GraveMoss(amount));
                    chest.DropItem(new BoneDust(amount));
                    chest.DropItem(new CorpseAsh(amount));
                    chest.DropItem(new SoulFragment(amount));
                    chest.DropItem(new NecroticShroud(amount));
                    chest.DropItem(new LichDust(amount));
                    chest.DropItem(new PhylacteryShard(amount));
                    chest.DropItem(new ReaperEssence(amount));
                    break;

                case AbilitySchool.Summoning:
                    chest.DropItem(new PlanarDust(amount));
                    chest.DropItem(new EtherShard(amount));
                    chest.DropItem(new AetherShard(amount));
                    chest.DropItem(new SummoningCrystal(amount));
                    chest.DropItem(new ChaosShard(amount));
                    chest.DropItem(new BindingRune(amount));
                    chest.DropItem(new DimensionalKey(amount));
                    chest.DropItem(new SummoningSalt(amount));
                    break;

                case AbilitySchool.Shamanic:
                    chest.DropItem(new LightningRoot(amount));
                    chest.DropItem(new ThunderMoss(amount));
                    chest.DropItem(new StormCrystal(amount));
                    chest.DropItem(new StormEssence(amount));
                    chest.DropItem(new Server.Items.SpiritFeather(amount));
                    chest.DropItem(new PrimalThunder(amount));
                    chest.DropItem(new TotemCarving(amount));
                    chest.DropItem(new WindEssence(amount));
                    break;

#if VYSTIA_SONGWEAVING
                case AbilitySchool.Bardic:
                    chest.DropItem(new SongPetal(amount));
                    chest.DropItem(new EchoDust(amount));
                    chest.DropItem(new VoiceCrystal(amount));
                    chest.DropItem(new MuseEssence(amount));
                    chest.DropItem(new HarmonyGem(amount));
                    chest.DropItem(new EternalNote(amount));
                    chest.DropItem(new GoldenString(amount));
                    chest.DropItem(new DragonScale(amount));
                    chest.DropItem(new SongweavingSpellbook());
                    chest.DropItem(new MagicLute());
#if VYSTIA_CRESCENDO
                    chest.DropItem(new CrescendoCrystal(5));
                    chest.DropItem(new CrescendoCatalyst(2));
#endif
                    break;
#endif

                case AbilitySchool.Enchanting:
                    chest.DropItem(new ArcaneDust(amount));
                    chest.DropItem(new EssenceOfMagic(amount));
                    chest.DropItem(new ManaCrystal(amount));
                    chest.DropItem(new LeyLineEssence(amount));
                    chest.DropItem(new LeyLineShard(amount));
                    chest.DropItem(new RuneFragment(amount));
                    chest.DropItem(new RunicPowder(amount));
                    chest.DropItem(new TitanRune(amount));
                    break;

                case AbilitySchool.Illusion:
                    chest.DropItem(new MirrorDust(amount));
                    chest.DropItem(new PhantomSilk(amount));
                    chest.DropItem(new MirageEssence(amount));
                    chest.DropItem(new DreamCrystal(amount));
                    chest.DropItem(new RealitySplinter(amount));
                    chest.DropItem(new VoidMirror(amount));
                    chest.DropItem(new ChaosPrism(amount));
                    chest.DropItem(new PhantomPetal(amount));
                    break;
            }
        }

        private static Item[] GetStartingWeaponsForSchool(AbilitySchool school)
        {
            switch (school)
            {
                case AbilitySchool.Fighter:
                    return new Item[] { new VikingSword() { Name = "Fighter's Blade", Hue = 2305 } };

                case AbilitySchool.Barbarian:
                    return new Item[] { new DoubleAxe() { Name = "Frostborn Axe", Hue = 0x26 } };

                case AbilitySchool.Monk:
                    return new Item[] { }; // No weapon - unarmed

                case AbilitySchool.Rogue:
                    return new Item[] { 
                        new AssassinSpike() { Name = "Shadow Blade", Hue = 1109 },
                        new AssassinSpike() { Name = "Shadow Blade", Hue = 1109 }
                    };

                case AbilitySchool.Ranger:
                    return new Item[] { new Server.Items.Vystia.VystiaYumi() { Name = "Desert Longbow" } };

                case AbilitySchool.Knight:
                    return new Item[] { new VikingSword() { Name = "Knight's Blade", Hue = 2305 } };

                case AbilitySchool.Paladin:
                    return new Item[] { new Server.Items.Vystia.VystiaLance() { Name = "Holy Lance" } };

                case AbilitySchool.Templar:
                    return new Item[] { new Longsword() { Name = "Zealot's Blade", Hue = 1150 } };

                case AbilitySchool.BountyHunter:
                    return new Item[] { 
                        new Kryss() { Name = "Hunter's Blade", Hue = 1719 },
                        new Crossbow() { Name = "Tracker's Crossbow", Hue = 1719 }
                    };

                case AbilitySchool.Beastmaster:
                    return new Item[] { new ShepherdsCrook() { Name = "Beastmaster's Crook", Hue = 0x7D6 } };

                case AbilitySchool.Artificer:
                    return new Item[] { new Mace() { Name = "Engineer's Wrench", Hue = 2305 } };

                case AbilitySchool.Alchemist:
                    return new Item[] { new GnarledStaff() { Name = "Mixing Rod", Hue = 2010 } };

                case AbilitySchool.Cleric:
                    return new Item[] { new Scepter() { Name = "Staff of Healing", Hue = 1153 } };

                case AbilitySchool.Wizard:
                    return new Item[] { new QuarterStaff() { Name = "Arcane Staff", Hue = 1154 } };

                default:
                    return new Item[] { };
            }
        }

        private static Spellbook GetAbilityBookForSchool(AbilitySchool school)
        {
            // Some martial classes have spellbooks (Cleric, Wizard, Paladin)
            switch (school)
            {
                case AbilitySchool.Paladin:
                    return new BookOfChivalry();
                case AbilitySchool.Cleric:
                    return new Spellbook((ulong)0xFFFFFFFFFFFFFFFF); // Standard magery
                case AbilitySchool.Wizard:
                    return new Spellbook((ulong)0xFFFFFFFFFFFFFFFF); // Standard magery
                default:
                    return null; // Most martial classes don't have spellbooks
            }
        }

        #endregion
    }
}
