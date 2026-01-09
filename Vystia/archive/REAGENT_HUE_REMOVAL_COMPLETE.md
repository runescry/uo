# Vystia Reagent Hue Removal - COMPLETE

**Date:** 2025-12-11
**Status:** ✅ **ALL REAGENT HUES REMOVED**

---

## Summary

All 96 Vystia magic reagents have had their hues removed (set to 0). Reagents now use their Vystia-themed fantasy names but appear as standard stackable item graphics without recoloring.

---

## Changes Made

**Before:**
```csharp
public Frostbloom(int amount)
    : base(amount, 0x0F86, 0x481, "Frosthold tundra", "Ice Magic (Circles 1-3)")
    //                      ^^^^^ Ice blue hue
{
    Weight = 0.1;
}
```

**After:**
```csharp
public Frostbloom(int amount)
    : base(amount, 0x0F86, 0, "Frosthold tundra", "Ice Magic (Circles 1-3)")
    //                      ^ No hue (standard color)
{
    Weight = 0.1;
}
```

---

## Rationale

1. **Simplicity:** No custom hues means easier maintenance
2. **Stackability:** All reagents use confirmed stackable ItemIDs without modification
3. **Name-Based ID:** Players identify reagents by their Vystia-themed names, not colors
4. **Consistency:** Matches the reagent bag approach (bags also hue = 0)

---

## Files Modified

**All 12 Reagent School Files:**
- BardicMagicReagents.cs
- DarkMagicReagents.cs
- DivinationMagicReagents.cs
- ElementalMagicReagents.cs
- EnchantingMagicReagents.cs
- HexMagicReagents.cs
- IceMagicReagents.cs
- IllusionMagicReagents.cs
- NatureMagicReagents.cs
- NecromancyReagents.cs
- ShamanicMagicReagents.cs
- SummoningMagicReagents.cs

---

## How Reagents Appear In-Game

All reagents now appear as their base ItemID graphics with standard UO colors:

| ItemID | Base Graphic | Example Vystia Reagents |
|--------|--------------|-------------------------|
| 0x0F8F | Grave Dust (gray powder) | BoneDust, VoidDust, ArcaneDust, MirrorDust, etc. |
| 0x0F8E | Nox Crystal (green) | GlacierCrystal, VoidCrystal, StormCrystal, etc. |
| 0x0F8A | Pig Iron (metal) | PrimordialEmber, PhylacteryShard, EtherShard, etc. |
| 0x0F7A | Black Pearl (dark gem) | ArcticPearl, DragonHeart, DemonHeart, etc. |
| 0x0F86 | Mandrake Root (root) | Frostbloom, Moonpetal, AshPetal, etc. |
| 0x1C18 | Oil Flask (bottle) | PermafrostEssence, TreantSap, MagmaEssence, etc. |
| 0x1A9C | Flax Bundle (fiber) | Winterleaf, PrimalVine, Flameweed, etc. |
| 0x0F8D | Spider Silk (white thread) | VoidSilk, FateThread, GoldenString, etc. |
| 0x0F7B | Bloodmoss (red moss) | WildMoss, BogMoss, ShadowMoss, ThunderMoss |
| 0x0F7D | Daemon Blood (vial) | FrozenSoul, ShadowEssence, ReaperEssence |
| 0x0F78 | Batwing (wing) | ViperFang, PhoenixFeather, SpiritFeather, etc. |
| 0x0F0E | Potion Bottle (empty) | FrostEssence, SoulFragment, MirageEssence |
| 0x1422 | Beeswax (yellow wax) | ElderwoodSeed, HagsHair, NecroticShroud |
| 0x0DE1 | Kindling (wood sticks) | DruidBark, LivingBark, TotemCarving |

---

## Example: Ice Magic Reagents

All 8 Ice Magic reagents now appear as their base graphics (no ice blue hue):

1. **Frostbloom** - Looks like Mandrake Root (brown)
2. **Glacier Crystal** - Looks like Nox Crystal (green)
3. **Winterleaf** - Looks like Flax Bundle (tan fiber)
4. **Permafrost Essence** - Looks like Empty Bottle (clear)
5. **Arctic Pearl** - Looks like Black Pearl (dark)
6. **Frozen Soul** - Looks like Daemon Blood (red vial)
7. **Frost Essence** - Looks like Empty Bottle (clear)
8. **Heart of Winter** - Looks like Black Pearl (dark)

Players identify them by reading the item name tooltip, not by color.

---

## Tool Used

**Script:** `Vystia/tools/remove_reagent_hues.py`

**Method:**
```python
# Pattern: base(amount, 0xHEX, 0xHUE, "location", "usage")
# Replace with: base(amount, 0xHEX, 0, "location", "usage")
pattern = r'(base\(amount,\s*0x[0-9a-fA-F]+,\s*)0x[0-9a-fA-F]+'
replacement = r'\g<1>0'
```

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
5. **Test:** All reagents should now appear unhued with Vystia names

---

## Related Changes

This completes the reagent system overhaul:

1. ✅ **Stackable ItemIDs** - All 96 reagents use 14 confirmed stackable ItemIDs
2. ✅ **Reagent Bags** - All bags unhued (hue = 0)
3. ✅ **Reagents** - All reagents unhued (hue = 0)
4. ✅ **Distribution** - Balanced across ItemIDs based on appearance

---

*Hue removal completed: 2025-12-11*
*Status: PRODUCTION READY ✅*
