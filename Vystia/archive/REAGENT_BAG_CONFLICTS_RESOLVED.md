# Reagent Bag ItemID Conflicts - RESOLVED

**Date:** 2025-12-11
**Status:** ✅ **ALL CONFLICTS RESOLVED - 12/12 BAGS CONFLICT-FREE**

---

## Problem

Each reagent bag contained 8 reagents for a magic school. However, multiple reagents in the same bag shared the same ItemID, making them visually indistinguishable to players. This created confusion when trying to identify which reagent was which.

**Example (Ice Magic - BEFORE FIX):**
- PermafrostEssence: 0x0F0E (Potion Bottle)
- FrostEssence: 0x0F0E (Potion Bottle) ❌ **DUPLICATE!**
- ArcticPearl: 0x0F7A (Black Pearl)
- HeartOfWinter: 0x0F7A (Black Pearl) ❌ **DUPLICATE!**

Players would see two identical-looking bottles and two identical-looking pearls in their bag, with no way to distinguish them visually except by reading the name tooltip.

---

## Initial Audit

**Bags with conflicts:** 11/12
**Total ItemID conflicts:** 21

| Bag | Conflicts | Details |
|-----|-----------|---------|
| Ice Magic | 2 | 2 pairs of duplicates |
| Nature Magic | 2 | 2 pairs of duplicates |
| Hex Magic | 1 | 1 pair of duplicates |
| Elemental Magic | 1 | 1 pair of duplicates |
| **Dark Magic** | 0 | ✅ No conflicts |
| Divination Magic | 2 | 3 dusts + 2 crystals |
| Necromancy | 1 | 3 dusts |
| Summoning Magic | 2 | 4 shards + 2 dusts |
| Shamanic Magic | 1 | 2 essences |
| Bardic Magic | 1 | 2 silks |
| Enchanting Magic | 2 | 3 runes + 2 dusts |
| Illusion Magic | 1 | 2 crystals |

---

## Solution

Remapped reagent ItemIDs to ensure **each bag has 8 reagents with 8 unique ItemIDs**.

### Strategy:
1. Preserve thematic appropriateness (dusts→grave dust, crystals→nox crystal, etc.)
2. Ensure no bag has duplicate ItemIDs
3. Maintain balanced distribution across all 14 confirmed stackable ItemIDs

### Changes Made:

**21 reagents remapped** across 11 reagent files:

#### Ice Magic (2 changes)
- FrostEssence: 0x0F0E → **0x1C18** (Oil Flask)
- HeartOfWinter: 0x0F7A → **0x0F7B** (Bloodmoss)

#### Nature Magic (2 changes)
- LivingBark: 0x0DE1 → **0x0F78** (Batwing)
- AncientRoot: 0x0F86 → **0x0F7A** (Black Pearl)

#### Hex Magic (1 change)
- CursedPearl: 0x0F7A → **0x0F8E** (Nox Crystal)

#### Elemental Magic (1 change)
- ElementalCore: 0x0F8E → **0x0F7D** (Daemon Blood)

#### Divination Magic (3 changes)
- TimeDust: 0x0F8F → **0x0F7D** (Daemon Blood)
- DivinationDust: 0x0F8F → **0x0DE1** (Kindling)
- StarlightCrystal: 0x0F8E → **0x0F0E** (Potion Bottle)

#### Necromancy (2 changes)
- CorpseAsh: 0x0F8F → **0x0DE1** (Kindling)
- LichDust: 0x0F8F → **0x0F86** (Mandrake Root)

#### Summoning Magic (4 changes)
- AetherShard: 0x0F8A → **0x0F7D** (Daemon Blood)
- ChaosShard: 0x0F8A → **0x0DE1** (Kindling)
- BindingRune: 0x0F8A → **0x0F86** (Mandrake Root)
- SummoningSalt: 0x0F8F → **0x1422** (Beeswax)

#### Shamanic Magic (1 change)
- WindEssence: 0x1C18 → **0x0F0E** (Potion Bottle)

#### Bardic Magic (1 change)
- GoldenString: 0x0F8D → **0x1422** (Beeswax)

#### Enchanting Magic (3 changes)
- RuneFragment: 0x0F8A → **0x0DE1** (Kindling)
- RunicPowder: 0x0F8F → **0x0F86** (Mandrake Root)
- TitanRune: 0x0F8A → **0x0F7A** (Black Pearl)

#### Illusion Magic (1 change)
- ChaosPrism: 0x0F8E → **0x1C18** (Oil Flask)

---

## Verification Results

**All 12 bags now have 8 unique ItemIDs:**

### Ice Magic ✅
```
0x0F0E - PermafrostEssence
0x0F7A - ArcticPearl
0x0F7B - HeartOfWinter (CHANGED)
0x0F7D - FrozenSoul
0x0F86 - Frostbloom
0x0F8E - GlacierCrystal
0x1A9C - Winterleaf
0x1C18 - FrostEssence (CHANGED)
```

### Nature Magic ✅
```
0x0DE1 - DruidBark
0x0F78 - LivingBark (CHANGED)
0x0F7A - AncientRoot (CHANGED)
0x0F7B - WildMoss
0x0F86 - Moonpetal
0x1422 - ElderwoodSeed
0x1A9C - PrimalVine
0x1C18 - TreantSap
```

### Hex Magic ✅
```
0x0F78 - ViperFang
0x0F7A - ToadsEye
0x0F7B - BogMoss
0x0F86 - SwampLotus
0x0F8E - CursedPearl (CHANGED)
0x0F8F - CursedSalt
0x1422 - HagsHair
0x1A9C - Witchweed
```

### Elemental Magic ✅
```
0x0F78 - PhoenixFeather
0x0F7A - DragonHeart
0x0F7D - ElementalCore (CHANGED)
0x0F86 - AshPetal
0x0F8A - PrimordialEmber
0x0F8E - LavaGlass
0x1A9C - Flameweed
0x1C18 - MagmaEssence
```

### Dark Magic ✅
```
0x0F7A - DemonHeart
0x0F7B - ShadowMoss
0x0F7D - ShadowEssence
0x0F86 - ShadowPetal
0x0F8D - VoidSilk
0x0F8E - VoidCrystal
0x0F8F - VoidDust
0x1A9C - VoidWeed
```

### Divination Magic ✅
```
0x0DE1 - DivinationDust (CHANGED)
0x0F0E - StarlightCrystal (CHANGED)
0x0F7A - SeeingStone
0x0F7D - TimeDust (CHANGED)
0x0F8D - FateThread
0x0F8E - FateCrystal
0x0F8F - TimeSand
0x1A9C - PropheticLeaf
```

### Necromancy ✅
```
0x0DE1 - CorpseAsh (CHANGED)
0x0F0E - SoulFragment
0x0F7B - GraveMoss
0x0F7D - ReaperEssence
0x0F86 - LichDust (CHANGED)
0x0F8A - PhylacteryShard
0x0F8F - BoneDust
0x1422 - NecroticShroud
```

### Summoning Magic ✅
```
0x0DE1 - ChaosShard (CHANGED)
0x0F7A - DimensionalKey
0x0F7D - AetherShard (CHANGED)
0x0F86 - BindingRune (CHANGED)
0x0F8A - EtherShard
0x0F8E - SummoningCrystal
0x0F8F - PlanarDust
0x1422 - SummoningSalt (CHANGED)
```

### Shamanic Magic ✅
```
0x0DE1 - TotemCarving
0x0F0E - WindEssence (CHANGED)
0x0F78 - SpiritFeather
0x0F7B - ThunderMoss
0x0F86 - LightningRoot
0x0F8A - PrimalThunder
0x0F8E - StormCrystal
0x1C18 - StormEssence
```

### Bardic Magic ✅
```
0x0F78 - DragonScale
0x0F7A - HarmonyGem
0x0F86 - SongPetal
0x0F8D - EternalNote
0x0F8E - VoiceCrystal
0x0F8F - EchoDust
0x1422 - GoldenString (CHANGED)
0x1C18 - MuseEssence
```

### Enchanting Magic ✅
```
0x0DE1 - RuneFragment (CHANGED)
0x0F7A - TitanRune (CHANGED)
0x0F7D - LeyLineEssence
0x0F86 - RunicPowder (CHANGED)
0x0F8A - LeyLineShard
0x0F8E - ManaCrystal
0x0F8F - ArcaneDust
0x1C18 - EssenceOfMagic
```

### Illusion Magic ✅
```
0x0F0E - MirageEssence
0x0F7A - VoidMirror
0x0F86 - PhantomPetal
0x0F8A - RealitySplinter
0x0F8D - PhantomSilk
0x0F8E - DreamCrystal
0x0F8F - MirrorDust
0x1C18 - ChaosPrism (CHANGED)
```

---

## Tools Used

1. **check_bag_itemid_conflicts.py** - Initial audit showing 11/12 bags had conflicts
2. **fix_bag_itemid_conflicts.py** - Applied new ItemID mapping to resolve all conflicts
3. **verify_bag_fixes.py** - Verified all 12 bags now have 8 unique ItemIDs

---

## Build Instructions

1. **Stop ServUO server**
2. **Rebuild:**
   ```bash
   cd D:\UO\ServUO
   dotnet build
   ```
3. **Restart server**
4. **Delete old reagent bags and spawn new ones:**
   ```
   [srb
   ```
5. **Verify:** Each bag should now have 8 visually distinct reagents

---

## Player Experience (After Fix)

**Example: Ice Magic Bag**

Players will now see 8 visually distinct items:
1. Frostbloom (looks like Mandrake Root - brown plant)
2. Glacier Crystal (looks like Nox Crystal - green crystal)
3. Winterleaf (looks like Flax Bundle - tan fiber)
4. Permafrost Essence (looks like Potion Bottle - clear bottle)
5. Arctic Pearl (looks like Black Pearl - dark gem)
6. Frozen Soul (looks like Daemon Blood - red vial)
7. Frost Essence (looks like Oil Flask - oil bottle) **✨ Now distinct!**
8. Heart of Winter (looks like Bloodmoss - red moss) **✨ Now distinct!**

No more confusion! Every reagent in the bag is visually unique.

---

## Related Changes

This completes the reagent system overhaul:

1. ✅ **Stackable ItemIDs** - All 96 reagents use 14 confirmed stackable ItemIDs
2. ✅ **Reagent Bags** - All bags unhued (hue = 0)
3. ✅ **Reagents** - All reagents unhued (hue = 0)
4. ✅ **Distribution** - Balanced across ItemIDs based on appearance
5. ✅ **Conflict Resolution** - Each bag has 8 unique ItemIDs for visual clarity

---

*Conflicts resolved: 2025-12-11*
*Status: PRODUCTION READY ✅*
