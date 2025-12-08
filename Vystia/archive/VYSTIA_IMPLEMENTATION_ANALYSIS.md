# VYSTIA IMPLEMENTATION CROSS-REFERENCE ANALYSIS

## Overview
This document provides a comprehensive cross-reference analysis between Vystia's design documents (Resources, Equipment, Items) and what has been implemented in ServUO, identifying gaps, dependencies, and implementation priorities.

**Analysis Date:** December 2024
**Status:** Initial Implementation Phase

---

## IMPLEMENTATION STATUS SUMMARY

### Currently Implemented in ServUO

| Category | Item | File Location | Status |
|----------|------|---------------|--------|
| **Creatures** | Frost Father (Boss) | `Scripts/Mobiles/Vystia/Bosses/FrostFather.cs` | ✅ Complete |
| **Resources** | Frozen Artifact | `Scripts/Items/Vystia/Resources/FrozenArtifact.cs` | ✅ Complete |
| **Resources** | Frost Seal | `Scripts/Items/Vystia/Resources/FrostSeal.cs` | ✅ Complete |
| **Resources** | Heartwood Core Fragment | `Scripts/Items/Vystia/Resources/HeartwoodCoreFragment.cs` | ✅ Complete |
| **Weapons** | The Eternal Winter (Halberd) | `Scripts/Items/Vystia/Weapons/TheEternalWinter.cs` | ✅ Complete |
| **Loot System** | VystiaLootPack | `Scripts/Misc/VystiaLootPack.cs` | ✅ Complete |

### NOT YET Implemented

Everything else from the design documents remains unimplemented.

---

## CROSS-REFERENCE: RESOURCES → EQUIPMENT → CRAFTING

### Mining Resources (VYSTIA_RESOURCES_GUIDE.md)

| Ore Type | Region | Ingot Name Used In Equipment Guide | Can Craft? | Missing Implementation |
|----------|--------|-------------------------------------|------------|------------------------|
| Frozen Ore | Frosthold | **Frostforged Ingots** | ❌ NO | Need: FrozenOre.cs, FrostforgedIngot.cs |
| Molten Ore | Emberlands | **Emberforged Ingots** | ❌ NO | Need: MoltenOre.cs, EmberforgedIngot.cs |
| Crystal Ore | Crystal Barrens | **Crystalline Ingots** | ❌ NO | Need: CrystalOre.cs, CrystallineIngot.cs |
| Steamwork Ore | Ironclad | **Clockwork Ingots** | ❌ NO | Need: SteamworkOre.cs, ClockworkIngot.cs |
| Bog Iron Ore | Shadowfen | **Shadowforged Ingots** | ❌ NO | Need: BogIronOre.cs, ShadowforgedIngot.cs |
| Sandstone Ore | Desert | **Sunforged Ingots** | ❌ NO | Need: SandstoneOre.cs, SunforgedIngot.cs |
| Obsidian Ore | Obsidian | **Voidforged Ingots** | ❌ NO | Need: ObsidianOre.cs, VoidforgedIngot.cs |
| Living Ore | Verdantpeak | **Natureforged Ingots** | ❌ NO | Need: LivingOre.cs, NatureforgedIngot.cs |

**CRITICAL GAP:** Equipment Guide references ingot types (Frostforged, Emberforged, etc.) but Resources Guide only defines ore types. Missing the ore→ingot conversion and ingot item definitions.

### Wood Types (VYSTIA_RESOURCES_GUIDE.md → Equipment)

| Wood Type | Region | Board Name | Used For | Missing Implementation |
|-----------|--------|------------|----------|------------------------|
| Frostwood | Frosthold | Frostwood Boards | Frost Bow, Ice Crossbow | Need: Frostwood.cs, FrostwoodBoard.cs |
| Flamewood | Emberlands | Flamewood Boards | (Not in equipment guide) | Need: Flamewood.cs, FlamewoodBoard.cs |
| Living Wood | Verdantpeak | Living Wood Boards | Living Bow, Nature's Crossbow | Need: LivingWood.cs, LivingWoodBoard.cs |
| Shadowwood | Shadowfen | Shadowwood Boards | Bog Shield | Need: Shadowwood.cs, ShadowwoodBoard.cs |
| Crystal Wood | Crystal | Crystal Wood Boards | (Not in equipment guide) | Need: CrystalWood.cs, CrystalWoodBoard.cs |
| Ironwood | Ironclad | Ironwood Boards | (Not in equipment guide) | Need: Ironwood.cs, IronwoodBoard.cs |
| Petrified Wood | Desert | Petrified Boards | (Not in equipment guide) | Need: PetrifiedWood.cs, PetrifiedBoard.cs |

### Leather Types (VYSTIA_RESOURCES_GUIDE.md)

| Leather Type | Source Creature | Creature Exists? | Missing Implementation |
|--------------|-----------------|------------------|------------------------|
| Frost Leather | Winter Wolves | ❌ NO | Need: WinterWolf.cs, FrostLeather.cs |
| Fire Leather | Lava Hounds | ❌ NO | Need: LavaHound.cs, FireLeather.cs |
| Shadow Leather | Shadow Wolves | ❌ NO | Need: ShadowWolf.cs, ShadowLeather.cs |

---

## CROSS-REFERENCE: EQUIPMENT → CRAFTING MATERIALS

### Frosthold Equipment (VYSTIA_EQUIPMENT_GUIDE.md)

| Equipment | Materials Required | Materials Exist? | Gap Analysis |
|-----------|-------------------|------------------|--------------|
| Icicle Blade | 12 Frostforged ingots | ❌ NO | Need FrostforgedIngot |
| Winter's Edge | 14 Frostforged ingots | ❌ NO | Need FrostforgedIngot |
| Frostbite | 8 Frostforged ingots | ❌ NO | Need FrostforgedIngot |
| Glacier Shard | 8 Frostforged ingots | ❌ NO | Need FrostforgedIngot |
| Frost Bow | 7 Frostwood boards | ❌ NO | Need FrostwoodBoard |
| Ice Crossbow | 10 Frostwood + 1 Frostforged | ❌ NO | Need both materials |
| Frostforged Plate Set | 100 Frostforged ingots | ❌ NO | Need FrostforgedIngot |
| **The Eternal Winter** | **BOSS DROP** | ✅ YES | Implemented |

### Emberlands Equipment (VYSTIA_EQUIPMENT_GUIDE.md)

| Equipment | Materials Required | Materials Exist? | Gap Analysis |
|-----------|-------------------|------------------|--------------|
| Flame Tongue | 8 Emberforged ingots | ❌ NO | Need EmberforgedIngot |
| Magma Blade | 10 Emberforged ingots | ❌ NO | Need EmberforgedIngot |
| Phoenix Wing | 10 Emberforged ingots | ❌ NO | Need EmberforgedIngot |
| Lava Edge | 12 Emberforged ingots | ❌ NO | Need EmberforgedIngot |
| Emberforged Plate Set | 100 Emberforged ingots | ❌ NO | Need EmberforgedIngot |

### Ironclad Equipment (Special - Requires Components)

| Equipment | Materials Required | Materials Exist? | Gap Analysis |
|-----------|-------------------|------------------|--------------|
| Clockwork Sword | 10 Clockwork ingots + 5 gears | ❌ NO | Need ClockworkIngot + ClockworkGear |
| Gear Blade | 14 Clockwork ingots + 3 gears | ❌ NO | Need ClockworkIngot + ClockworkGear |
| Steam Saber | 8 Clockwork ingots + 2 springs | ❌ NO | Need ClockworkIngot + ClockworkSpring |
| Clockwork Plate Set | 100 Clockwork + 20 gears | ❌ NO | Need all components |

---

## CROSS-REFERENCE: CREATURES → LOOT DROPS → RESOURCES

### Implemented Creature Drops

| Creature | Drops | Implemented? | Notes |
|----------|-------|--------------|-------|
| Frost Father | Frozen Artifact (1-3 guaranteed) | ✅ YES | Working |
| Frost Father | Frozen Artifact (2-4 @ 10%) | ✅ YES | Working |
| Frost Father | Heartwood Core Fragment (1%) | ✅ YES | Working |
| Frost Father | The Eternal Winter (2%) | ✅ YES | Working |
| Frost Father | Frost Seal (via loot pack) | ✅ YES | In VystiaLootPack |

### Missing Creature → Resource Drops

| Creature (from docs) | Expected Drop | Creature Exists? | Drop Exists? |
|---------------------|---------------|------------------|--------------|
| Ice Elementals | Ice Crystals, Frost Essence | ❌ NO | ❌ NO |
| Ancient Ice Dragons | Frozen Artifacts | ❌ NO | ✅ YES |
| Polar Elementals | Frozen Artifacts | ❌ NO | ✅ YES |
| Winter Wolves | Frost Leather | ❌ NO | ❌ NO |
| Ancient Treants | Heartwood Core Fragment (0.1%) | ❌ NO | ✅ YES |
| Treants | Living Bark | ❌ NO | ❌ NO |
| Magma Elementals | Magma Heart (0.05%) | ❌ NO | ❌ NO |
| Fire Elementals | Ember Bloom | ❌ NO | ❌ NO |
| Clockwork Creatures | Clockwork Gear | ❌ NO | ❌ NO |
| Shadow Creatures | Void Dust | ❌ NO | ❌ NO |

---

## LOOT PACK ANALYSIS

### VystiaLootPack.cs Current Contents

| Region | Loot Pack | Resources Included | Gap Analysis |
|--------|-----------|-------------------|--------------|
| Frosthold | FrostholdPoor/Average/Rich/Boss | FrozenArtifact, FrostSeal, Sapphire | Missing: Ice Crystals, Frost Essence, Frozen Ore |
| Emberlands | EmberlandsPoor/Average/Rich | Ruby, SulfurousAsh | Missing: Molten Ore, Ember Bloom, Lava Pearl |
| Crystal | CrystalPoor/Average/Rich | Diamond, StarSapphire | Missing: Crystal Ore, Crystal Pollen, Prismatic Shard |
| Shadowfen | ShadowfenPoor/Average/Rich | Emerald, Nightshade, SpidersSilk | Missing: Bog Iron Ore, Void Dust, Swamp Lotus |
| Verdantpeak | VerdantpeakPoor/Average/Rich | Emerald, Ginseng, BloodMoss | Missing: Living Ore, Living Bark, Living Wood |
| Desert | DesertPoor/Average/Rich | Citrine, Amber | Missing: Sandstone Ore, Desert Rose, Glass Sand |

### Missing Regional Loot Packs

| Region | Status | Needs |
|--------|--------|-------|
| Ironclad | ❌ NOT IMPLEMENTED | IroncladPoor/Average/Rich, Clockwork parts |
| Obsidian | ❌ NOT IMPLEMENTED | ObsidianPoor/Average/Rich, Voidforged materials |

---

## REAGENT SYSTEM ANALYSIS

### Vystia Reagents (15 New) - Implementation Status

| Reagent | Source | Hue | Implemented? |
|---------|--------|-----|--------------|
| Frost Essence | Ice elementals, frozen waterfalls | 1152 | ❌ NO |
| Ember Bloom | Volcanic vents | 1358 | ❌ NO |
| Storm Crystal | Lightning strikes | 1001 | ❌ NO |
| Void Dust | Shadow creatures | 1109 | ❌ NO |
| Living Bark | Treants, ancient trees | 2010 | ❌ NO |
| Swamp Lotus | Bog waters | 2212 | ❌ NO |
| Desert Rose | Oasis gardens | 2305 | ❌ NO |
| Crystal Pollen | Crystal flowers | 1154 | ❌ NO |
| Clockwork Gear | Clockwork creatures | 2401 | ❌ NO |
| Steam Vapor | Steam vents | 1001 | ❌ NO |
| Dragon Scale Powder | Ground dragon scales | N/A | ❌ NO |
| Phoenix Feather | Phoenix nests | 1358 | ❌ NO |
| Kraken Ink | Kraken | 1109 | ❌ NO |
| Time Dust | Time anomalies | 1154 | ❌ NO |
| Ley Line Essence | Ley line nodes | 1156 | ❌ NO |

---

## CRAFTING SYSTEM GAPS

### Blacksmithing - Missing DefBlacksmithy Modifications

To craft regional weapons/armor, need to modify:
- `Scripts/Services/Craft/DefBlacksmithy.cs` - Add new material types
- Add ore/ingot conversion recipes
- Add regional weapon/armor recipes with material requirements

### Special Crafting Requirements (From Equipment Guide)

| Material | Special Requirement | Implementation Needed |
|----------|--------------------|-----------------------|
| Frostforged | Must be within 5 tiles of snow/ice | Location check in crafting |
| Frostforged | Requires Frozen Essence (1 per 10 ingots) | Reagent consumption |
| Emberforged | Must be near forge or lava | Location check in crafting |
| Emberforged | Requires Sulfurous Ash (1 per 10 ingots) | Reagent consumption |
| Clockwork | Requires GM Tinkering + GM Blacksmith | Dual skill check |
| Clockwork | Needs gears/springs components | Component consumption |

### Skill Requirements (Equipment Guide)

| Material | Min Skill | Success at |
|----------|-----------|------------|
| Iron | 0.0 | 50.0 |
| Sunforged | 65.0 | 115.0 |
| Natureforged | 70.0 | 120.0 |
| Shadowforged | 75.0 | 125.0 |
| Steamwork | 80.0 | 130.0 |
| Frostforged | 85.0 | 135.0 |
| Emberforged | 90.0 | 140.0 |
| Crystalline | 95.0 | 145.0 |
| Voidforged | 100.0 | 150.0 |

---

## ARTIFACT WEAPONS - IMPLEMENTATION STATUS

### Legendary Weapons (Equipment Guide)

| Weapon | Type | Drop Source | Implemented? |
|--------|------|-------------|--------------|
| The Eternal Winter | Halberd | Frost Father | ✅ YES |
| Phoenix Ascension | Katana | Volcano Wyrm | ❌ NO |
| The Cogmaster | War Hammer | Forge Master | ❌ NO |
| Prismatic Edge | Longsword | Crystal Dragon | ❌ NO |
| Voidcaller | Quarter Staff | Shadow Lich | ❌ NO |

### Legendary Artifacts (Items Guide)

| Artifact | Type | Drop Source | Implemented? |
|----------|------|-------------|--------------|
| Heartwood Core | Quest Item | Ancient Treants (0.1%) | ❌ NO (only fragment exists) |
| Magma Heart | Crafting Station | Magma Elementals (0.05%) | ❌ NO |
| Luminous Scepter | Weapon | Light Elementals (0.02%) | ❌ NO |
| Mirror of Truth | Utility | Shadow Wraiths (0.1%) | ❌ NO |

---

## PRIORITY IMPLEMENTATION ROADMAP

### Phase 1: Core Resources (HIGH PRIORITY)
1. **Ore Types** - 8 new ore classes
2. **Ingot Types** - 8 new ingot classes (smelted from ores)
3. **Wood Types** - 7 new wood/board classes
4. **Basic Reagents** - Frost Essence, Ember Bloom, Void Dust, Living Bark

### Phase 2: Crafting Integration (MEDIUM PRIORITY)
1. Modify DefBlacksmithy to add new ore→ingot recipes
2. Modify DefBlacksmithy to add regional weapon recipes
3. Add special crafting location checks
4. Add component requirements for Clockwork items

### Phase 3: Creature Implementation (MEDIUM PRIORITY)
1. Regional basic creatures (Ice Elementals, Fire Elementals, etc.)
2. Regional mid-tier creatures (Frost Drakes, Lava Serpents)
3. Regional leather source creatures (Winter Wolf, Lava Hound, Shadow Wolf)

### Phase 4: Additional Bosses (LOWER PRIORITY)
1. Volcano Wyrm (Emberlands boss)
2. Forge Master (Ironclad boss)
3. Crystal Dragon (Crystal Barrens boss)
4. Shadow Lich (Shadowfen boss)
5. Ancient Treant (Verdantpeak boss)

### Phase 5: Equipment Expansion (LOWER PRIORITY)
1. Regional weapon variants (40+ weapons)
2. Regional armor sets (6 complete sets)
3. Regional shield variants (8+ shields)
4. Legendary artifact armor sets

---

## NAMING INCONSISTENCIES FOUND

| Document | Name Used | Other Document | Name Used | Resolution |
|----------|-----------|----------------|-----------|------------|
| Resources | "Frozen Ore" | Equipment | "Frostforged Ingots" | Frozen Ore → smelt → Frostforged Ingots |
| Resources | "Steamwork Ore" | Equipment | "Clockwork Ingots" | Steamwork Ore → smelt → Clockwork Ingots |
| Resources | "Living Ore" | Equipment | "Natureforged Ingots" | Living Ore → smelt → Natureforged Ingots |
| Items | "Ice Crystals" | Loot Pack | Not present | Add to Frosthold loot packs |
| Items | "Frozen Artifacts" | Implemented | "FrozenArtifact" | ✅ Consistent |

---

## SUMMARY

### Implementation Completeness

| Category | Documented | Implemented | % Complete |
|----------|------------|-------------|------------|
| Ore Types | 9 | 0 | 0% |
| Ingot Types | 9 | 0 | 0% |
| Wood Types | 8 | 0 | 0% |
| Leather Types | 7 | 0 | 0% |
| New Reagents | 15 | 0 | 0% |
| Legendary Artifacts | 4 | 1 partial | 10% |
| Legendary Weapons | 5 | 1 | 20% |
| Regional Weapons | 40+ | 0 | 0% |
| Regional Armor Sets | 6 | 0 | 0% |
| Bosses | 10 | 1 | 10% |
| Loot Packs | 8 regions | 6 regions | 75% |
| Crafting Recipes | 200+ | 0 | 0% |

### Critical Dependencies Chain

```
1. Ore Items → 2. Ingot Items → 3. Crafting Recipes → 4. Regional Weapons/Armor
                                        ↑
                     5. Reagent Items (for special crafting)
                                        ↑
                     6. Creatures (to drop resources)
```

### Recommended Next Steps

1. **Create base resource items** (ores, ingots, woods, boards, leathers)
2. **Implement reagent items** (15 new Vystia reagents)
3. **Update VystiaLootPack** to include new resources
4. **Create regional creatures** that drop the resources
5. **Modify crafting system** to use new materials
6. **Implement regional equipment** using crafting recipes

---

## FILES TO CREATE (Priority Order)

### Batch 1: Core Ores & Ingots
```
Scripts/Items/Vystia/Resources/Ores/
  - FrozenOre.cs
  - MoltenOre.cs
  - CrystalOre.cs
  - SteamworkOre.cs
  - BogIronOre.cs
  - SandstoneOre.cs
  - ObsidianOre.cs
  - LivingOre.cs

Scripts/Items/Vystia/Resources/Ingots/
  - FrostforgedIngot.cs
  - EmberforgedIngot.cs
  - CrystallineIngot.cs
  - ClockworkIngot.cs
  - ShadowforgedIngot.cs
  - SunforgedIngot.cs
  - VoidforgedIngot.cs
  - NatureforgedIngot.cs
```

### Batch 2: Core Reagents
```
Scripts/Items/Vystia/Resources/Reagents/
  - FrostEssence.cs
  - EmberBloom.cs
  - VoidDust.cs
  - LivingBark.cs
  - StormCrystal.cs
  - SwampLotus.cs
  - DesertRose.cs
  - CrystalPollen.cs
  - ClockworkGear.cs (also crafting component)
  - SteamVapor.cs
```

### Batch 3: Basic Regional Creatures
```
Scripts/Mobiles/Vystia/Frosthold/
  - IceElemental.cs (if not using base UO)
  - FrostWolf.cs
  - FrostDrake.cs

Scripts/Mobiles/Vystia/Emberlands/
  - LavaSerpent.cs
  - EmberImp.cs
  - LavaHound.cs
```

This analysis document should be updated as implementation progresses.
