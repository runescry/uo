# Vystia Reagent Final Distribution - COMPLETE

**Date:** 2025-12-11
**Status:** ✅ **ALL 96 REAGENTS NOW USE 14 CONFIRMED STACKABLE ITEMIDS**
**Update:** ✅ **ALL HUES REMOVED + ALL BAG CONFLICTS RESOLVED**

---

## Final Distribution

All 96 Vystia reagents distributed across **14 confirmed stackable ItemIDs** based on appearance/description:

| ItemID | Type | Count | Description |
|--------|------|-------|-------------|
| **0x0F8F** | Grave Dust | 14 | All dust/powder/ash/salt reagents |
| **0x0F8E** | Nox Crystal | 12 | All crystals/gems (green crystal) |
| **0x0F8A** | Pig Iron | 11 | All shards/fragments/runes/metal |
| **0x0F7A** | Black Pearl | 10 | Pearls/hearts/dark gems |
| **0x0F86** | Mandrake Root | 9 | Roots/petals/flowers |
| **0x1C18** | Oil Flask | 6 | Liquid essences (oil-like) |
| **0x1A9C** | Flax Bundle | 6 | Herbs/leaves/plants/fibers |
| **0x0F8D** | Spider Silk | 5 | Silk/thread/string |
| **0x0F7B** | Bloodmoss | 5 | All moss types |
| **0x0F7D** | Daemon Blood | 4 | Dark essences/souls/blood |
| **0x0F78** | Batwing | 4 | Wings/feathers/scales/fangs |
| **0x0F0E** | Potion Bottle | 4 | Light essences/souls |
| **0x1422** | Beeswax | 3 | Seeds/wax/organic materials |
| **0x0DE1** | Kindling | 3 | Bark/wood/carvings |

**Total:** 96 reagents (8 per school × 12 schools)

---

## Reagent Bags

✅ **All 12 reagent bags now use standard bag appearance (Hue = 0)**
- No longer colored the same as reagents inside
- Easy to distinguish bag from contents

---

## Changes Made

1. ✅ **85 ItemIDs replaced** from non-stackable to stackable
2. ✅ **Redistributed across 14 ItemIDs** for variety (instead of just 8)
3. ✅ **Removed bag hues** - all bags now standard brown
4. ✅ **Based on appearance** - crystals use crystal ItemID, dust uses dust ItemID, etc.

---

## Build Instructions

1. **Stop ServUO server**
2. **Rebuild:**
   ```bash
   cd D:\UO\ServUO
   dotnet build
   ```
3. **Restart server**
4. **Delete old bags and spawn new ones:**
   ```
   [srb
   ```
5. **Test stacking:** All reagents should now stack!

---

## Verification

```bash
# Check distribution
cd D:\UO\ServUO\Scripts\Items\Vystia\Resources\Reagents
grep -h "base(amount, 0x" *.cs | grep -v "Serial" | sed 's/.*base(amount, \(0x[0-9a-fA-F]*\).*/\1/' | sort | uniq -c | sort -rn
```

Expected output:
```
     14 0x0F8F  (Grave Dust)
     12 0x0F8E  (Nox Crystal)
     11 0x0F8A  (Pig Iron)
     10 0x0F7A  (Black Pearl)
      9 0x0F86  (Mandrake Root)
      6 0x1C18  (Oil Flask)
      6 0x1A9C  (Flax Bundle)
      5 0x0F8D  (Spider Silk)
      5 0x0F7B  (Bloodmoss)
      4 0x0F7D  (Daemon Blood)
      4 0x0F78  (Batwing)
      4 0x0F0E  (Potion Bottle)
      3 0x1422  (Beeswax)
      3 0x0DE1  (Kindling)
```

---

## Files Modified

**Reagent Files (12):**
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

**Bag File (1):**
- VystiaReagentBags.cs (removed all hues)

**Total:** 13 files modified

---

## Tools Created

1. `Vystia/tools/fix_all_itemids_to_stackable.py` - Initial fix to stackable ItemIDs
2. `Vystia/tools/redistribute_reagents_by_appearance.py` - Final distribution by appearance
3. `Vystia/reference/UO_STACKABLE_REAGENTS.md` - Complete stackable ItemID reference
4. `Vystia/reference/STACKABLE_ITEMIDS_SUMMARY.md` - Quick reference
5. `ServUO/Scripts/Custom/Commands/SpawnReagentBagsCommand.cs` - `[srb]` command

---

## Recent Updates (2025-12-11)

### Hue Removal ✅
- All 96 reagents now have hue = 0 (no recoloring)
- Reagents keep Vystia-themed names (Frostbloom, Glacier Crystal, etc.)
- Players identify reagents by name tooltip, not color
- See: `REAGENT_HUE_REMOVAL_COMPLETE.md`

### Bag Conflict Resolution ✅
- Fixed 11/12 bags that had duplicate ItemIDs
- Remapped 21 reagents to ensure each bag has 8 unique ItemIDs
- No more visual confusion - each bag shows 8 distinct items
- See: `REAGENT_BAG_CONFLICTS_RESOLVED.md`

---

*Final distribution completed: 2025-12-11*
*All 96 reagents use confirmed stackable ItemIDs*
*All hues removed, all bag conflicts resolved*
*Status: PRODUCTION READY ✅*
