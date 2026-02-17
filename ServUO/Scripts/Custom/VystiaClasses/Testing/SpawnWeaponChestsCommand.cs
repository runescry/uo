/*
 * Spawn Weapon Chests Command
 *
 * Creates multiple chests containing all equippable weapons organized by category.
 * All weapons are made unrestricted (no race/elf/gargoyle requirements).
 *
 * Usage:
 *   [SpawnWeaponChests or [SWC - Spawn weapon chests
 *   [ClearWeaponChests or [CWC - Clear spawned weapon chests
 */

using System;
using System.Collections.Generic;
using System.Reflection;
using Server;
using Server.Commands;
using Server.Items;
using Server.Mobiles;
using Server.Custom.VystiaClasses.Testing;

namespace Server.Custom.VystiaClasses.Testing
{
    public static class SpawnWeaponChestsCommand
    {
        // Track spawned chests for cleanup
        private static List<Item> s_SpawnedChests = new List<Item>();

        public static void Initialize()
        {
            CommandSystem.Register("SpawnWeaponChests", AccessLevel.GameMaster, SpawnWeaponChests_OnCommand);
            CommandSystem.Register("SWC", AccessLevel.GameMaster, SpawnWeaponChests_OnCommand);
            CommandSystem.Register("ClearWeaponChests", AccessLevel.GameMaster, ClearWeaponChests_OnCommand);
            CommandSystem.Register("CleanupWeaponChests", AccessLevel.GameMaster, ClearWeaponChests_OnCommand);
        }

        #region Spawn Command

        [Usage("[SpawnWeaponChests")]
        [Description("Spawns multiple chests containing all equippable weapons (unrestricted) organized by category.")]
        private static void SpawnWeaponChests_OnCommand(CommandEventArgs e)
        {
            Mobile from = e.Mobile;
            Point3D baseLoc = from.Location;
            Map map = from.Map;

            from.SendMessage(0x35, "Spawning weapon chests (all weapons unrestricted)...");

            int chestsCreated = 0;
            int weaponsCreated = 0;
            int xOffset = 0;
            int yOffset = 0;
            int chestSpacing = 2;

            // Create chests in a grid pattern
            Action<string, int, List<Type>> createChest = (name, hue, weaponTypes) =>
            {
                if (weaponTypes.Count == 0) return;

                Point3D chestLoc = new Point3D(baseLoc.X + xOffset, baseLoc.Y + yOffset, baseLoc.Z);
                MetalChest chest = new MetalChest();
                chest.Name = $"[TEST] {name}";
                chest.Hue = hue;
                chest.MoveToWorld(chestLoc, map);

                // Track for cleanup
                s_SpawnedChests.Add(chest);

                foreach (Type weaponType in weaponTypes)
                {
                    try
                    {
                        Item weapon = (Item)Activator.CreateInstance(weaponType);
                        if (weapon != null)
                        {
                            // Make weapon unrestricted
                            MakeUnrestricted(weapon);
                            chest.DropItem(weapon);
                            weaponsCreated++;
                        }
                    }
                    catch { }
                }

                chestsCreated++;
                xOffset += chestSpacing;
                if (xOffset > 8)
                {
                    xOffset = 0;
                    yOffset += chestSpacing;
                }
            };

            // 1. Swords
            createChest("Swords", 1150, GetSwordTypes());

            // 2. Axes
            createChest("Axes", 1358, GetAxeTypes());

            // 3. Maces & Bashing
            createChest("Maces & Bashing", 2401, GetMaceTypes());

            // 4. Knives & Daggers
            createChest("Knives & Daggers", 1109, GetKnifeTypes());

            // 5. Polearms
            createChest("Polearms", 2305, GetPolearmTypes());

            // 6. Spears
            createChest("Spears", 2010, GetSpearTypes());

            // 7. Staves
            createChest("Staves", 1154, GetStaffTypes());

            // 8. Ranged - Bows
            createChest("Bows & Ranged", 1175, GetRangedTypes());

            // 9. Wands
            createChest("Wands", 1153, GetWandTypes());

            // 10. Fencing Weapons
            createChest("Fencing Weapons", 2213, GetFencingTypes());

            // 11. Ninja Weapons
            createChest("Ninja & Exotic", 1, GetNinjaTypes());

            // 12. Vystia Regional Weapons
            createChest("Vystia Regional", 1152, GetVystiaRegionalTypes());

            // 13. Vystia Legendary Weapons
            createChest("Vystia Legendary", 1161, GetVystiaLegendaryTypes());

            from.SendMessage(0x55, "Created {0} chests with {1} unrestricted weapons!", chestsCreated, weaponsCreated);
            from.SendMessage(0x35, "Use [ClearWeaponChests to remove them.");
        }

        #endregion

        #region Cleanup Command

        [Usage("[ClearWeaponChests")]
        [Description("Removes all weapon test chests spawned by [SpawnWeaponChests.")]
        private static void ClearWeaponChests_OnCommand(CommandEventArgs e)
        {
            Mobile from = e.Mobile;

            int deleted = 0;

            // Delete tracked chests
            foreach (Item chest in s_SpawnedChests)
            {
                if (chest != null && !chest.Deleted)
                {
                    chest.Delete();
                    deleted++;
                }
            }
            s_SpawnedChests.Clear();

            // Also find any [TEST] chests nearby (in case of server restart)
            List<Item> nearbyTestChests = new List<Item>();
            foreach (Item item in from.GetItemsInRange(50))
            {
                if (item is MetalChest && item.Name != null && item.Name.StartsWith("[TEST]"))
                {
                    nearbyTestChests.Add(item);
                }
            }

            foreach (Item chest in nearbyTestChests)
            {
                chest.Delete();
                deleted++;
            }

            if (deleted > 0)
                from.SendMessage(0x55, "Deleted {0} weapon test chest(s).", deleted);
            else
                from.SendMessage(0x22, "No weapon test chests found to delete.");
        }

        #endregion

        #region Make Weapons Unrestricted

        /// <summary>
        /// Removes race restrictions and other equip limitations from a weapon
        /// </summary>
        private static void MakeUnrestricted(Item item)
        {
            if (item == null) return;

            // Add "[U]" to name to indicate unrestricted
            if (item.Name == null)
                item.Name = $"[U] {item.GetType().Name}";
            else if (!item.Name.StartsWith("[U]"))
                item.Name = $"[U] {item.Name}";

            // For BaseWeapon, we need to handle restrictions
            if (item is BaseWeapon weapon)
            {
                // Set LootType to Regular so it can be freely moved
                weapon.LootType = LootType.Regular;

                // Remove any cursed/blessed status
                // weapon.LootType handles this

                // Try to set StrRequirement/DexRequirement/IntRequirement to 0 via reflection
                TrySetProperty(weapon, "StrRequirement", 0);
                TrySetProperty(weapon, "DexRequirement", 0);
                TrySetProperty(weapon, "IntRequirement", 0);

                // For race restrictions, we can't easily override virtual properties,
                // but we can use a workaround - make it blessed and set Identified
                weapon.Identified = true;
            }

            // For BaseArmor
            if (item is BaseArmor armor)
            {
                armor.LootType = LootType.Regular;
                armor.Identified = true;
                TrySetProperty(armor, "StrRequirement", 0);
            }
        }

        /// <summary>
        /// Try to set a property value using reflection
        /// </summary>
        private static void TrySetProperty(object obj, string propertyName, object value)
        {
            try
            {
                var prop = obj.GetType().GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);
                if (prop != null && prop.CanWrite)
                {
                    prop.SetValue(obj, value);
                }
            }
            catch { }
        }

        #endregion

        #region Weapon Type Lists

        private static List<Type> GetSwordTypes()
        {
            return new List<Type>
            {
                // Standard Swords
                typeof(Broadsword),
                typeof(Cutlass),
                typeof(Katana),
                typeof(Longsword),
                typeof(Scimitar),
                typeof(VikingSword),
                typeof(BoneHarvester),
                typeof(CrescentBlade),
                typeof(ThinLongsword),
                // Two-Handed Swords
                typeof(NoDachi),
                typeof(Daisho),
                // Elven Swords (Unrestricted)
                typeof(UnrestrictedElvenSpellblade),
                typeof(UnrestrictedRadiantScimitar),
                // Gargish Swords (Unrestricted)
                typeof(UnrestrictedGargishKatana),
                typeof(UnrestrictedGargishTalwar),
                typeof(UnrestrictedGargishDaisho),
                typeof(UnrestrictedGlassSword),
                typeof(UnrestrictedBloodBlade),
                typeof(UnrestrictedDreadSword),
            };
        }

        private static List<Type> GetAxeTypes()
        {
            return new List<Type>
            {
                // One-Handed Axes
                typeof(Axe),
                typeof(DoubleAxe),
                typeof(OrnateAxe),
                typeof(DualShortAxes),
                // Two-Handed Axes
                typeof(BattleAxe),
                typeof(LargeBattleAxe),
                typeof(TwoHandedAxe),
                typeof(ExecutionersAxe),
                typeof(HeavyOrnateAxe),
                // Bardiche (Axe/Polearm hybrid)
                typeof(Bardiche),
                // Gargish Axes (Unrestricted)
                typeof(UnrestrictedGargishAxe),
                typeof(UnrestrictedGargishBattleAxe),
                typeof(UnrestrictedGargishBardiche),
                // Cleaver variants
                typeof(Cleaver),
                typeof(WarCleaver),
            };
        }

        private static List<Type> GetMaceTypes()
        {
            return new List<Type>
            {
                // One-Handed Maces
                typeof(Club),
                typeof(Mace),
                typeof(WarMace),
                typeof(Scepter),
                typeof(DiamondMace),
                // Two-Handed Maces
                typeof(Maul),
                typeof(WarHammer),
                typeof(HammerPick),
                typeof(Tetsubo),
                // Gargish Maces (Unrestricted)
                typeof(UnrestrictedGargishMaul),
                typeof(UnrestrictedGargishWarHammer),
                typeof(UnrestrictedDiscMace),
            };
        }

        private static List<Type> GetKnifeTypes()
        {
            return new List<Type>
            {
                typeof(Dagger),
                typeof(AssassinSpike),
                typeof(ButcherKnife),
                typeof(SkinningKnife),
                typeof(Kryss),
                // Gargish (Unrestricted)
                typeof(UnrestrictedGargishDagger),
                typeof(UnrestrictedGargishKryss),
                // Elven (Unrestricted)
                typeof(UnrestrictedLeafblade),
                typeof(UnrestrictedElvenMachete),
                typeof(WarCleaver),
            };
        }

        private static List<Type> GetPolearmTypes()
        {
            return new List<Type>
            {
                typeof(Halberd),
                typeof(Bardiche),
                typeof(Scythe),
                typeof(BladedStaff),
                typeof(DoubleBladedStaff),
                typeof(Pike),
                typeof(Lance),
                // Gargish (Unrestricted)
                typeof(UnrestrictedGargishScythe),
            };
        }

        private static List<Type> GetSpearTypes()
        {
            return new List<Type>
            {
                typeof(Spear),
                typeof(ShortSpear),
                typeof(Pitchfork),
                typeof(WarFork),
                typeof(DualPointedSpear),
                // Gargish (Unrestricted)
                typeof(UnrestrictedGargishPike),
                typeof(UnrestrictedGargishWarFork),
            };
        }

        private static List<Type> GetStaffTypes()
        {
            return new List<Type>
            {
                typeof(QuarterStaff),
                typeof(GnarledStaff),
                typeof(BlackStaff),
                typeof(ShepherdsCrook),
                typeof(GlassStaff),
                typeof(SerpentStoneStaff),
                // Elven (Unrestricted)
                typeof(UnrestrictedWildStaff),
                // Gargish (Unrestricted)
                typeof(UnrestrictedGargishGnarledStaff),
            };
        }

        private static List<Type> GetRangedTypes()
        {
            return new List<Type>
            {
                // Bows
                typeof(Bow),
                typeof(CompositeBow),
                typeof(Yumi),
                // Elven Bows (Unrestricted)
                typeof(UnrestrictedElvenCompositeLongbow),
                typeof(UnrestrictedMagicalShortbow),
                // Crossbows
                typeof(Crossbow),
                typeof(HeavyCrossbow),
                typeof(RepeatingCrossbow),
                // Thrown (Unrestricted)
                typeof(UnrestrictedBoomerang),
                typeof(UnrestrictedCyclone),
                typeof(UnrestrictedSoulGlaive),
            };
        }

        private static List<Type> GetWandTypes()
        {
            return new List<Type>
            {
                typeof(ClumsyWand),
                typeof(FeebleWand),
                typeof(FireballWand),
                typeof(GreaterHealWand),
                typeof(HarmWand),
                typeof(HealWand),
                typeof(IDWand),
                typeof(LightningWand),
                typeof(MagicArrowWand),
                typeof(ManaDrainWand),
                typeof(WeaknessWand),
            };
        }

        private static List<Type> GetFencingTypes()
        {
            return new List<Type>
            {
                typeof(Kryss),
                typeof(WarFork),
                typeof(Spear),
                typeof(ShortSpear),
                typeof(Pike),
                typeof(Lance),
                typeof(Pitchfork),
                // Gargish (Unrestricted)
                typeof(UnrestrictedGargishKryss),
                typeof(UnrestrictedGargishPike),
                typeof(UnrestrictedGargishWarFork),
                typeof(UnrestrictedGargishLance),
            };
        }

        private static List<Type> GetNinjaTypes()
        {
            return new List<Type>
            {
                typeof(Kama),
                typeof(Lajatang),
                typeof(Nunchaku),
                typeof(Sai),
                typeof(Tessen),
                typeof(Tekagi),
                typeof(Wakizashi),
                typeof(Bokuto),
                typeof(Fukiya),
            };
        }

        private static List<Type> GetVystiaRegionalTypes()
        {
            var types = new List<Type>();

            // Try to load Vystia regional weapons
            // Frosthold
            TryAddType(types, "Server.Items.IcicleBlade");
            TryAddType(types, "Server.Items.WintersEdge");
            TryAddType(types, "Server.Items.FrozenCleaver");
            TryAddType(types, "Server.Items.GlacialHammer");
            TryAddType(types, "Server.Items.IceShard");
            TryAddType(types, "Server.Items.FrostSpear");

            // Emberlands
            TryAddType(types, "Server.Items.FlameTongue");
            TryAddType(types, "Server.Items.MagmaBlade");
            TryAddType(types, "Server.Items.MoltenAxe");
            TryAddType(types, "Server.Items.LavaSpear");
            TryAddType(types, "Server.Items.VolcanicHammer");
            TryAddType(types, "Server.Items.InfernoBlade");

            // Crystal Barrens
            TryAddType(types, "Server.Items.CrystalShard");
            TryAddType(types, "Server.Items.PrismBlade");
            TryAddType(types, "Server.Items.RefractionEdge");

            // Ironclad
            TryAddType(types, "Server.Items.ClockworkSword");
            TryAddType(types, "Server.Items.GearBlade");
            TryAddType(types, "Server.Items.SteamSaber");
            TryAddType(types, "Server.Items.PistonMace");
            TryAddType(types, "Server.Items.CogAxe");
            TryAddType(types, "Server.Items.GearStaff");
            TryAddType(types, "Server.Items.BrassKnuckles");

            // ShadowVoid
            TryAddType(types, "Server.Items.ShadowFang");
            TryAddType(types, "Server.Items.VoidEdge");
            TryAddType(types, "Server.Items.DarkBlade");

            // Desert
            TryAddType(types, "Server.Items.SandstormBlade");
            TryAddType(types, "Server.Items.DesertScimitar");
            TryAddType(types, "Server.Items.DuneWalkerSpear");

            // Verdantpeak
            TryAddType(types, "Server.Items.ThornBlade");
            TryAddType(types, "Server.Items.VineLash");
            TryAddType(types, "Server.Items.NatureStaff");

            // Shadowfen
            TryAddType(types, "Server.Items.SwampFang");
            TryAddType(types, "Server.Items.BogBlade");
            TryAddType(types, "Server.Items.MarshMace");

            return types;
        }

        private static List<Type> GetVystiaLegendaryTypes()
        {
            var types = new List<Type>();

            // Vystia Legendary Weapons
            TryAddType(types, "Server.Items.TheEternalWinter");
            TryAddType(types, "Server.Items.PhoenixAscension");
            TryAddType(types, "Server.Items.TheCogmaster");
            TryAddType(types, "Server.Items.PrismaticEdge");
            TryAddType(types, "Server.Items.Voidcaller");

            return types;
        }

        private static void TryAddType(List<Type> list, string typeName)
        {
            Type type = ScriptCompiler.FindTypeByFullName(typeName);
            if (type != null)
                list.Add(type);
        }

        #endregion
    }
}
