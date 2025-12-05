# VYSTIA IMPLEMENTATION PLAN

## Executive Summary

This plan outlines the systematic implementation of Vystia's resource, crafting, creature, and equipment systems for ServUO. The implementation is organized into 6 phases with clear dependencies and deliverables.

**Estimated Scope:** ~150+ files across resources, creatures, equipment, and crafting modifications

---

## PHASE 1: CORE RESOURCES
**Priority:** CRITICAL - All other phases depend on this
**Dependencies:** None

### 1.1 Regional Ores (8 files)

Create base ore classes that can be mined from regional deposits.

| File | Class | Region | Hue | Mining Skill |
|------|-------|--------|-----|--------------|
| `Scripts/Items/Vystia/Resources/Ores/FrozenOre.cs` | FrozenOre | Frosthold | 1152 | 85.0 |
| `Scripts/Items/Vystia/Resources/Ores/MoltenOre.cs` | MoltenOre | Emberlands | 1358 | 90.0 |
| `Scripts/Items/Vystia/Resources/Ores/CrystalOre.cs` | CrystalOre | Crystal Barrens | 1154 | 95.0 |
| `Scripts/Items/Vystia/Resources/Ores/SteamworkOre.cs` | SteamworkOre | Ironclad | 2401 | 80.0 |
| `Scripts/Items/Vystia/Resources/Ores/BogIronOre.cs` | BogIronOre | Shadowfen | 2212 | 75.0 |
| `Scripts/Items/Vystia/Resources/Ores/SandstoneOre.cs` | SandstoneOre | Desert | 2305 | 70.0 |
| `Scripts/Items/Vystia/Resources/Ores/ObsidianOre.cs` | ObsidianOre | Obsidian | 1109 | 100.0 |
| `Scripts/Items/Vystia/Resources/Ores/LivingOre.cs` | LivingOre | Verdantpeak | 2010 | 65.0 |

**Template Structure:**
```csharp
public class FrozenOre : BaseOre
{
    [Constructable]
    public FrozenOre() : this(1) { }

    [Constructable]
    public FrozenOre(int amount) : base(CraftResource.FrozenOre, amount)
    {
        Hue = 1152;
    }
}
```

### 1.2 Regional Ingots (8 files)

Create ingot classes smelted from regional ores.

| File | Class | Smelted From | Hue | Properties |
|------|-------|--------------|-----|------------|
| `Scripts/Items/Vystia/Resources/Ingots/FrostforgedIngot.cs` | FrostforgedIngot | FrozenOre | 1152 | Cold +5 |
| `Scripts/Items/Vystia/Resources/Ingots/EmberforgedIngot.cs` | EmberforgedIngot | MoltenOre | 1358 | Fire +5 |
| `Scripts/Items/Vystia/Resources/Ingots/CrystallineIngot.cs` | CrystallineIngot | CrystalOre | 1154 | Energy +5 |
| `Scripts/Items/Vystia/Resources/Ingots/ClockworkIngot.cs` | ClockworkIngot | SteamworkOre | 2401 | Durability +25% |
| `Scripts/Items/Vystia/Resources/Ingots/ShadowforgedIngot.cs` | ShadowforgedIngot | BogIronOre | 2212 | Self-Repair 1 |
| `Scripts/Items/Vystia/Resources/Ingots/SunforgedIngot.cs` | SunforgedIngot | SandstoneOre | 2305 | Weight -30% |
| `Scripts/Items/Wystia/Resources/Ingots/VoidforgedIngot.cs` | VoidforgedIngot | ObsidianOre | 1109 | Mage Armor |
| `Scripts/Items/Vystia/Resources/Ingots/NatureforgedIngot.cs` | NatureforgedIngot | LivingOre | 2010 | HP Regen +1 |

### 1.3 Regional Woods (7 files)

| File | Class | Region | Hue | Properties |
|------|-------|--------|-----|------------|
| `Scripts/Items/Vystia/Resources/Woods/Frostwood.cs` | Frostwood | Frosthold | 1152 | Cold Resist +5 |
| `Scripts/Items/Vystia/Resources/Woods/Flamewood.cs` | Flamewood | Emberlands | 1358 | Fire Resist +5 |
| `Scripts/Items/Vystia/Resources/Woods/CrystalWood.cs` | CrystalWood | Crystal | 1154 | Spell Channeling |
| `Scripts/Items/Vystia/Resources/Woods/Shadowwood.cs` | Shadowwood | Shadowfen | 2212 | Stealth +10 |
| `Scripts/Items/Vystia/Resources/Woods/LivingWood.cs` | LivingWood | Verdantpeak | 2010 | Self-Repair |
| `Scripts/Items/Vystia/Resources/Woods/Ironwood.cs` | Ironwood | Ironclad | 2401 | Physical +10 |
| `Scripts/Items/Vystia/Resources/Woods/PetrifiedWood.cs` | PetrifiedWood | Desert | 2305 | Durability +50% |

### 1.4 Regional Boards (7 files)

Matching board classes for each wood type (crafted from logs).

### 1.5 Regional Leathers (3 files)

| File | Class | Source Creature | Hue | Properties |
|------|-------|-----------------|-----|------------|
| `Scripts/Items/Vystia/Resources/Leathers/FrostLeather.cs` | FrostLeather | Winter Wolf | 1152 | Cold Resist |
| `Scripts/Items/Vystia/Resources/Leathers/FireLeather.cs` | FireLeather | Lava Hound | 1358 | Fire Resist |
| `Scripts/Items/Vystia/Resources/Leathers/ShadowLeather.cs` | ShadowLeather | Shadow Wolf | 1109 | Stealth |

### 1.6 CraftResource Enum Extension

**Modify:** `Scripts/Services/Craft/Core/CraftResource.cs`

Add new resource types to the CraftResource enum:
```csharp
// Vystia Metals
FrozenOre,
MoltenOre,
CrystalOre,
SteamworkOre,
BogIronOre,
SandstoneOre,
ObsidianOre,
LivingOre,

// Vystia Woods
Frostwood,
Flamewood,
CrystalWood,
Shadowwood,
LivingWood,
Ironwood,
PetrifiedWood,

// Vystia Leathers
FrostLeather,
FireLeather,
ShadowLeather,
```

**Phase 1 Deliverables:** 33 resource item files + CraftResource modifications

---

## PHASE 2: REAGENTS & SPECIAL MATERIALS
**Priority:** HIGH
**Dependencies:** Phase 1 (for loot pack integration)

### 2.1 Elemental Reagents (4 files)

| File | Class | Source | Hue | Uses |
|------|-------|--------|-----|------|
| `Scripts/Items/Vystia/Resources/Reagents/FrostEssence.cs` | FrostEssence | Ice Elementals | 1152 | Ice spells, Frostforged crafting |
| `Scripts/Items/Vystia/Resources/Reagents/EmberBloom.cs` | EmberBloom | Volcanic vents | 1358 | Fire spells, Emberforged crafting |
| `Scripts/Items/Vystia/Resources/Reagents/StormCrystal.cs` | StormCrystal | Lightning strikes | 1001 | Lightning spells |
| `Scripts/Items/Vystia/Resources/Reagents/VoidDust.cs` | VoidDust | Shadow creatures | 1109 | Shadow magic |

### 2.2 Nature Reagents (4 files)

| File | Class | Source | Hue | Uses |
|------|-------|--------|-----|------|
| `Scripts/Items/Vystia/Resources/Reagents/LivingBark.cs` | LivingBark | Treants | 2010 | Nature spells, healing |
| `Scripts/Items/Vystia/Resources/Reagents/SwampLotus.cs` | SwampLotus | Bog waters | 2212 | Poison spells |
| `Scripts/Items/Vystia/Resources/Reagents/DesertRose.cs` | DesertRose | Oasis gardens | 2305 | Illusion magic |
| `Scripts/Items/Vystia/Resources/Reagents/CrystalPollen.cs` | CrystalPollen | Crystal flowers | 1154 | Mana restoration |

### 2.3 Mechanical Components (3 files)

| File | Class | Source | Hue | Uses |
|------|-------|--------|-----|------|
| `Scripts/Items/Vystia/Resources/Components/ClockworkGear.cs` | ClockworkGear | Clockwork creatures | 2401 | Clockwork crafting |
| `Scripts/Items/Vystia/Resources/Components/ClockworkSpring.cs` | ClockworkSpring | Clockwork creatures | 2401 | Clockwork crafting |
| `Scripts/Items/Vystia/Resources/Components/SteamCore.cs` | SteamCore | Steam vents | 2401 | Power source |

### 2.4 Exotic Reagents (5 files)

| File | Class | Source | Hue | Uses |
|------|-------|--------|-----|------|
| `Scripts/Items/Vystia/Resources/Reagents/DragonScalePowder.cs` | DragonScalePowder | Dragons | N/A | High-level magic |
| `Scripts/Items/Vystia/Resources/Reagents/PhoenixFeather.cs` | PhoenixFeather | Phoenix | 1358 | Resurrection |
| `Scripts/Items/Vystia/Resources/Reagents/KrakenInk.cs` | KrakenInk | Kraken | 1109 | Water magic |
| `Scripts/Items/Vystia/Resources/Reagents/TimeDust.cs` | TimeDust | Time anomalies | 1154 | Haste/slow |
| `Scripts/Items/Vystia/Resources/Reagents/LeyLineEssence.cs` | LeyLineEssence | Ley nodes | 1156 | Teleportation |

### 2.5 Special Regional Resources (8 files)

| File | Class | Region | Use |
|------|-------|--------|-----|
| `Scripts/Items/Vystia/Resources/Special/EternalIce.cs` | EternalIce | Frosthold | Never melts |
| `Scripts/Items/Vystia/Resources/Special/IceCrystal.cs` | IceCrystal | Frosthold | Magic focus |
| `Scripts/Items/Vystia/Resources/Special/EverburningCoal.cs` | EverburningCoal | Emberlands | Perpetual fuel |
| `Scripts/Items/Vystia/Resources/Special/LavaPearl.cs` | LavaPearl | Emberlands | Fire jewelry |
| `Scripts/Items/Vystia/Resources/Special/PrismaticShard.cs` | PrismaticShard | Crystal | All-element magic |
| `Scripts/Items/Vystia/Resources/Special/LeyCrystal.cs` | LeyCrystal | Crystal | Teleportation |
| `Scripts/Items/Vystia/Resources/Special/ShadowSilk.cs` | ShadowSilk | Shadowfen | Stealth armor |
| `Scripts/Items/Vystia/Resources/Special/TreantHeart.cs` | TreantHeart | Verdantpeak | Nature power |

**Phase 2 Deliverables:** 24 reagent/component files

---

## PHASE 3: LOOT PACK UPDATE
**Priority:** HIGH
**Dependencies:** Phase 1, Phase 2

### 3.1 Update VystiaLootPack.cs

Add new resource items to regional loot pack arrays:

```csharp
// Updated Frosthold Resources
public static readonly LootPackItem[] FrostholdResources = new[]
{
    new LootPackItem(typeof(FrozenArtifact), 10),
    new LootPackItem(typeof(FrostSeal), 2),
    new LootPackItem(typeof(FrozenOre), 15),        // NEW
    new LootPackItem(typeof(IceCrystal), 8),        // NEW
    new LootPackItem(typeof(FrostEssence), 5),      // NEW
    new LootPackItem(typeof(Sapphire), 5),
};

// Updated Emberlands Resources
public static readonly LootPackItem[] EmberlandsResources = new[]
{
    new LootPackItem(typeof(MoltenOre), 15),        // NEW
    new LootPackItem(typeof(EmberBloom), 8),        // NEW
    new LootPackItem(typeof(LavaPearl), 3),         // NEW
    new LootPackItem(typeof(Ruby), 5),
    new LootPackItem(typeof(SulfurousAsh), 8),
};

// Add missing regional packs
public static readonly LootPackItem[] IroncladResources = new[]
{
    new LootPackItem(typeof(SteamworkOre), 15),
    new LootPackItem(typeof(ClockworkGear), 8),
    new LootPackItem(typeof(ClockworkSpring), 5),
    new LootPackItem(typeof(SteamCore), 2),
};

public static readonly LootPackItem[] ObsidianResources = new[]
{
    new LootPackItem(typeof(ObsidianOre), 15),
    new LootPackItem(typeof(VoidDust), 8),
    new LootPackItem(typeof(Amethyst), 5),
};
```

### 3.2 Add Missing Regional Loot Packs

Create loot packs for:
- IroncladPoor / IroncladAverage / IroncladRich
- ObsidianPoor / ObsidianAverage / ObsidianRich

### 3.3 Add Boss-Specific Loot Packs

| Boss | Region | Pack Name | Special Drops |
|------|--------|-----------|---------------|
| Frost Father | Frosthold | FrostFatherBossItems | ✅ Already exists |
| Volcano Wyrm | Emberlands | VolcanoWyrmBossItems | Phoenix Ascension |
| Forge Master | Ironclad | ForgeMasterBossItems | The Cogmaster |
| Crystal Dragon | Crystal | CrystalDragonBossItems | Prismatic Edge |
| Shadow Lich | Shadowfen | ShadowLichBossItems | Voidcaller |
| Ancient Treant | Verdantpeak | AncientTreantBossItems | Heartwood Core |

**Phase 3 Deliverables:** Updated VystiaLootPack.cs with all regional resources

---

## PHASE 4: CREATURES
**Priority:** MEDIUM
**Dependencies:** Phase 1, Phase 2, Phase 3

### 4.1 Frosthold Creatures (6 files)

| File | Class | Tier | Drops |
|------|-------|------|-------|
| `Scripts/Mobiles/Vystia/Frosthold/FrostWolf.cs` | FrostWolf | Poor | Frost Leather |
| `Scripts/Mobiles/Vystia/Frosthold/IceSnake.cs` | IceSnake | Poor | - |
| `Scripts/Mobiles/Vystia/Frosthold/VystiaIceElemental.cs` | VystiaIceElemental | Average | Ice Crystal, Frost Essence |
| `Scripts/Mobiles/Vystia/Frosthold/FrostDrake.cs` | FrostDrake | Average | Frozen Ore |
| `Scripts/Mobiles/Vystia/Frosthold/FrostGiant.cs` | FrostGiant | Rich | Frozen Ore, Ice Crystals |
| `Scripts/Mobiles/Vystia/Frosthold/AncientWhiteDragon.cs` | AncientWhiteDragon | Rich | Frozen Artifact |

### 4.2 Emberlands Creatures (6 files)

| File | Class | Tier | Drops |
|------|-------|------|-------|
| `Scripts/Mobiles/Vystia/Emberlands/FireNewt.cs` | FireNewt | Poor | - |
| `Scripts/Mobiles/Vystia/Emberlands/EmberImp.cs` | EmberImp | Poor | Ember Bloom |
| `Scripts/Mobiles/Vystia/Emberlands/LavaHound.cs` | LavaHound | Average | Fire Leather |
| `Scripts/Mobiles/Vystia/Emberlands/LavaSerpent.cs` | LavaSerpent | Average | Molten Ore |
| `Scripts/Mobiles/Vystia/Emberlands/MagmaElemental.cs` | MagmaElemental | Rich | Molten Ore, Lava Pearl |
| `Scripts/Mobiles/Vystia/Emberlands/AncientRedDragon.cs` | AncientRedDragon | Rich | Molten Ore, Ember Bloom |

### 4.3 Crystal Barrens Creatures (5 files)

| File | Class | Tier | Drops |
|------|-------|------|-------|
| `Scripts/Mobiles/Vystia/Crystal/CrystalBeetle.cs` | CrystalBeetle | Poor | Crystal Pollen |
| `Scripts/Mobiles/Vystia/Crystal/ShardSpider.cs` | ShardSpider | Poor | - |
| `Scripts/Mobiles/Vystia/Crystal/CrystalElemental.cs` | CrystalElemental | Average | Crystal Ore |
| `Scripts/Mobiles/Vystia/Crystal/PrismaticDrake.cs` | PrismaticDrake | Average | Crystal Ore, Prismatic Shard |
| `Scripts/Mobiles/Vystia/Crystal/LeyGuardian.cs` | LeyGuardian | Rich | Ley Crystal, Crystal Ore |

### 4.4 Shadowfen Creatures (5 files)

| File | Class | Tier | Drops |
|------|-------|------|-------|
| `Scripts/Mobiles/Vystia/Shadowfen/SwampLeech.cs` | SwampLeech | Poor | - |
| `Scripts/Mobiles/Vystia/Shadowfen/BogToad.cs` | BogToad | Poor | Swamp Lotus |
| `Scripts/Mobiles/Vystia/Shadowfen/ShadowWolf.cs` | ShadowWolf | Average | Shadow Leather |
| `Scripts/Mobiles/Vystia/Shadowfen/BogThing.cs` | BogThing | Average | Bog Iron Ore |
| `Scripts/Mobiles/Vystia/Shadowfen/BogLord.cs` | BogLord | Rich | Bog Iron Ore, Shadow Silk |

### 4.5 Verdantpeak Creatures (5 files)

| File | Class | Tier | Drops |
|------|-------|------|-------|
| `Scripts/Mobiles/Vystia/Verdantpeak/ForestWolf.cs` | ForestWolf | Poor | - |
| `Scripts/Mobiles/Vystia/Verdantpeak/WoodlandSprite.cs` | WoodlandSprite | Poor | Living Bark |
| `Scripts/Mobiles/Vystia/Verdantpeak/VystiaTreant.cs` | VystiaTreant | Average | Living Wood, Living Bark |
| `Scripts/Mobiles/Vystia/Verdantpeak/ForestDrake.cs` | ForestDrake | Average | Living Ore |
| `Scripts/Mobiles/Vystia/Verdantpeak/ForestGuardian.cs` | ForestGuardian | Rich | Living Ore, Treant Heart |

### 4.6 Desert Creatures (5 files)

| File | Class | Tier | Drops |
|------|-------|------|-------|
| `Scripts/Mobiles/Vystia/Desert/DesertScorpion.cs` | DesertScorpion | Poor | - |
| `Scripts/Mobiles/Vystia/Desert/SandSerpent.cs` | SandSerpent | Poor | - |
| `Scripts/Mobiles/Vystia/Desert/SandElemental.cs` | SandElemental | Average | Sandstone Ore, Desert Rose |
| `Scripts/Mobiles/Vystia/Desert/DesertDrake.cs` | DesertDrake | Average | Sandstone Ore |
| `Scripts/Mobiles/Vystia/Desert/SandWurm.cs` | SandWurm | Rich | Sandstone Ore, Glass Sand |

### 4.7 Ironclad Creatures (5 files)

| File | Class | Tier | Drops |
|------|-------|------|-------|
| `Scripts/Mobiles/Vystia/Ironclad/ClockworkBeetle.cs` | ClockworkBeetle | Poor | Clockwork Gear |
| `Scripts/Mobiles/Vystia/Ironclad/SteamMephit.cs` | SteamMephit | Poor | Steam Vapor |
| `Scripts/Mobiles/Vystia/Ironclad/ClockworkGolem.cs` | ClockworkGolem | Average | Clockwork Gear, Spring |
| `Scripts/Mobiles/Vystia/Ironclad/SteamElemental.cs` | SteamElemental | Average | Steamwork Ore, Steam Core |
| `Scripts/Mobiles/Vystia/Ironclad/ClockworkTitan.cs` | ClockworkTitan | Rich | Steamwork Ore, All components |

### 4.8 Obsidian Creatures (4 files)

| File | Class | Tier | Drops |
|------|-------|------|-------|
| `Scripts/Mobiles/Vystia/Obsidian/VoidCreeper.cs` | VoidCreeper | Poor | Void Dust |
| `Scripts/Mobiles/Vystia/Obsidian/ShadowWraith.cs` | ShadowWraith | Average | Void Dust, Obsidian Ore |
| `Scripts/Mobiles/Vystia/Obsidian/VoidElemental.cs` | VoidElemental | Average | Obsidian Ore |
| `Scripts/Mobiles/Vystia/Obsidian/VoidLord.cs` | VoidLord | Rich | Obsidian Ore, Void Crystal |

**Phase 4 Deliverables:** 41 creature files

---

## PHASE 5: BOSSES & LEGENDARY ITEMS
**Priority:** MEDIUM
**Dependencies:** Phase 4

### 5.1 Regional Bosses (9 files - Frost Father already exists)

| File | Class | Region | Legendary Drop |
|------|-------|--------|----------------|
| ✅ `FrostFather.cs` | FrostFather | Frosthold | The Eternal Winter |
| `Scripts/Mobiles/Vystia/Bosses/VolcanoWyrm.cs` | VolcanoWyrm | Emberlands | Phoenix Ascension |
| `Scripts/Mobiles/Vystia/Bosses/ForgeMaster.cs` | ForgeMaster | Ironclad | The Cogmaster |
| `Scripts/Mobiles/Vystia/Bosses/CrystalDragon.cs` | CrystalDragon | Crystal | Prismatic Edge |
| `Scripts/Mobiles/Vystia/Bosses/ShadowLich.cs` | ShadowLich | Shadowfen | Voidcaller |
| `Scripts/Mobiles/Vystia/Bosses/AncientTreant.cs` | AncientTreant | Verdantpeak | Nature's Crown |
| `Scripts/Mobiles/Vystia/Bosses/SandKing.cs` | SandKing | Desert | Desert's Fury |
| `Scripts/Mobiles/Vystia/Bosses/ClockworkOverlord.cs` | ClockworkOverlord | Ironclad | Steamwork Exosuit |
| `Scripts/Mobiles/Vystia/Bosses/VoidAvatar.cs` | VoidAvatar | Obsidian | Void Shroud |

### 5.2 Legendary Weapons (4 files - The Eternal Winter already exists)

| File | Class | Type | Drop Source |
|------|-------|------|-------------|
| ✅ `TheEternalWinter.cs` | TheEternalWinter | Halberd | Frost Father |
| `Scripts/Items/Vystia/Weapons/Artifacts/PhoenixAscension.cs` | PhoenixAscension | Katana | Volcano Wyrm |
| `Scripts/Items/Vystia/Weapons/Artifacts/TheCogmaster.cs` | TheCogmaster | War Hammer | Forge Master |
| `Scripts/Items/Vystia/Weapons/Artifacts/PrismaticEdge.cs` | PrismaticEdge | Longsword | Crystal Dragon |
| `Scripts/Items/Vystia/Weapons/Artifacts/Voidcaller.cs` | Voidcaller | Quarter Staff | Shadow Lich |

### 5.3 Legendary Artifacts (4 files)

| File | Class | Type | Source |
|------|-------|------|--------|
| `Scripts/Items/Vystia/Artifacts/HeartwoodCore.cs` | HeartwoodCore | Quest Item | 5x Fragments |
| `Scripts/Items/Vystia/Artifacts/MagmaHeart.cs` | MagmaHeart | Crafting Station | Magma Elemental |
| `Scripts/Items/Vystia/Artifacts/LuminousScepter.cs` | LuminousScepter | Weapon | Light Elemental |
| `Scripts/Items/Vystia/Artifacts/MirrorOfTruth.cs` | MirrorOfTruth | Utility | Shadow Wraith |

**Phase 5 Deliverables:** 17 boss/legendary files

---

## PHASE 6: CRAFTING SYSTEM INTEGRATION
**Priority:** MEDIUM-LOW
**Dependencies:** All previous phases

### 6.1 CraftResourceInfo Updates

**Modify:** `Scripts/Services/Craft/Core/CraftResourceInfo.cs`

Add resource info entries for all Vystia materials:
```csharp
new CraftResourceInfo(1152, 0, "Frostforged", CraftAttributeInfo.Frostforged, CraftResource.FrozenOre, typeof(FrostforgedIngot), typeof(FrozenOre)),
new CraftResourceInfo(1358, 0, "Emberforged", CraftAttributeInfo.Emberforged, CraftResource.MoltenOre, typeof(EmberforgedIngot), typeof(MoltenOre)),
// ... etc
```

### 6.2 CraftAttributeInfo Updates

**Modify:** `Scripts/Services/Craft/Core/CraftAttributeInfo.cs`

Define attribute bonuses for each material:
```csharp
public static readonly CraftAttributeInfo Frostforged = new CraftAttributeInfo
{
    ArmorColdResist = 5,
    WeaponColdDamage = 5,
    // Additional properties
};
```

### 6.3 DefBlacksmithy Updates

**Modify:** `Scripts/Services/Craft/DefBlacksmithy.cs`

1. Add smelting recipes (ore → ingot)
2. Add regional weapon recipes
3. Add regional armor recipes
4. Add special crafting requirements:

```csharp
// Example: Frostforged crafting requirement
public override bool ConsumeOnFailure(Mobile from, CraftResource resource, CraftItem craftItem)
{
    if (resource == CraftResource.FrozenOre)
    {
        // 10% shatter chance on failure
        if (Utility.RandomDouble() < 0.10)
            return true;
    }
    return base.ConsumeOnFailure(from, resource, craftItem);
}

// Location requirement check
public override bool CheckCraft(Mobile from, CraftSystem system, Type typeItem, object tool)
{
    if (RequiresFrostLocation(typeItem))
    {
        // Check for snow/ice within 5 tiles
        if (!NearSnowOrIce(from, 5))
        {
            from.SendMessage("You must be near snow or ice to craft Frostforged items.");
            return false;
        }
    }
    return base.CheckCraft(from, system, typeItem, tool);
}
```

### 6.4 Regional Weapon Recipes

Add crafting entries for all regional weapon variants:

**Frosthold Weapons:**
```csharp
AddCraft(typeof(IcicleBlade), "Frosthold Weapons", "Icicle Blade", 85.0, 135.0, typeof(FrostforgedIngot), "Frostforged Ingots", 12, "You don't have enough ingots.");
AddCraft(typeof(WintersEdge), "Frosthold Weapons", "Winter's Edge", 85.0, 135.0, typeof(FrostforgedIngot), "Frostforged Ingots", 14, "You don't have enough ingots.");
// ... etc
```

### 6.5 Regional Armor Recipes

Add crafting entries for all regional armor sets.

### 6.6 Special Crafting Requirements

Implement special requirements from VYSTIA_EQUIPMENT_GUIDE.md:

| Material | Requirement | Implementation |
|----------|-------------|----------------|
| Frostforged | Near snow/ice | Location check |
| Frostforged | Frozen Essence (1 per 10 ingots) | Extra resource consumption |
| Emberforged | Near forge/lava | Location check |
| Emberforged | Sulfurous Ash (1 per 10 ingots) | Extra resource consumption |
| Clockwork | GM Tinkering + GM Blacksmith | Dual skill check |
| Clockwork | Gears/springs components | Component consumption |

**Phase 6 Deliverables:** Modifications to 4+ core crafting files

---

## PHASE 7: REGIONAL EQUIPMENT (OPTIONAL EXPANSION)
**Priority:** LOW
**Dependencies:** Phase 6

### 7.1 Regional Weapon Variants

Create all regional weapon variants from VYSTIA_EQUIPMENT_GUIDE.md:

**Frosthold Swords (4 files):**
- IcicleBlade.cs, WintersEdge.cs, Frostbite.cs, GlacierShard.cs

**Emberlands Swords (4 files):**
- FlameTongue.cs, MagmaBlade.cs, PhoenixWing.cs, LavaEdge.cs

**... (Continue for all regions and weapon types)**

### 7.2 Regional Armor Sets

Create full armor sets for each region (6 pieces each):

**Frostforged Plate Set:**
- FrostforgedPlateHelm.cs
- FrostforgedPlateGorget.cs
- FrostforgedPlateChest.cs
- FrostforgedPlateArms.cs
- FrostforgedPlateGloves.cs
- FrostforgedPlateLegs.cs

### 7.3 Regional Shields

Create all regional shield variants.

**Phase 7 Deliverables:** 100+ regional equipment files

---

## IMPLEMENTATION TIMELINE SUMMARY

| Phase | Description | Files | Dependencies |
|-------|-------------|-------|--------------|
| 1 | Core Resources | ~33 | None |
| 2 | Reagents & Materials | ~24 | Phase 1 |
| 3 | Loot Pack Update | 1 (major update) | Phase 1, 2 |
| 4 | Creatures | ~41 | Phase 1, 2, 3 |
| 5 | Bosses & Legendaries | ~17 | Phase 4 |
| 6 | Crafting Integration | ~4 (modifications) | All |
| 7 | Regional Equipment | ~100+ | Phase 6 |

**Total Estimated Files:** ~220+

---

## TESTING CHECKLIST

### Phase 1 Testing
- [ ] All ore types spawn correctly
- [ ] All ores smelt into correct ingots
- [ ] All ingots stack properly
- [ ] Wood types harvest from correct regions
- [ ] Leather drops from correct creatures

### Phase 2 Testing
- [ ] All reagents are stackable
- [ ] Reagents have correct hues
- [ ] Special materials have correct properties

### Phase 3 Testing
- [ ] Loot packs drop correct regional items
- [ ] Drop rates are balanced
- [ ] Boss packs include legendary chances

### Phase 4 Testing
- [ ] Creatures spawn with correct stats
- [ ] Creatures use correct AI types
- [ ] Creatures drop correct loot
- [ ] Creature hues match region themes

### Phase 5 Testing
- [ ] Boss mechanics work correctly
- [ ] Phase transitions function properly
- [ ] Legendary items have correct stats
- [ ] Drop rates are as specified

### Phase 6 Testing
- [ ] Smelting recipes work
- [ ] Crafting recipes produce correct items
- [ ] Special requirements are enforced
- [ ] Skill requirements are correct
- [ ] Material bonuses apply to crafted items

---

## NOTES

1. **Backwards Compatibility:** All new items use existing UO graphics with hue variations
2. **Performance:** New creatures should use existing AI types where possible
3. **Balance:** All stats should be reviewed against existing UO:R content
4. **Testing:** Each phase should be fully tested before proceeding to the next

This plan provides a structured approach to implementing the complete Vystia system while maintaining clear dependencies and deliverables.
