# Vystia Magic Reagent Audit Report

**Date:** 2025-12-11
**Issue:** Mismatch between spell requirements and available reagents

---

## Executive Summary

**CRITICAL ISSUE FOUND:** Ice Mage spells require **EternalIce** (14 spells use it), but it's **NOT included in the IceMagicReagentBag** spawnable via [spawnvystia gump.

### Ice Magic Reagent Analysis

#### Defined Reagents (8 total)
✅ Frostbloom
✅ GlacierCrystal
✅ Winterleaf
✅ PermafrostEssence
✅ ArcticPearl
✅ FrozenSoul
✅ FrostEssence
✅ HeartOfWinter

#### Reagents in Spawn Gump Bag (8 total)
✅ Frostbloom (10)
✅ GlacierCrystal (10)
✅ Winterleaf (10)
✅ PermafrostEssence (10)
✅ ArcticPearl (10)
✅ FrozenSoul (10)
✅ FrostEssence (10)
✅ HeartOfWinter (10)

#### Reagents REQUIRED by Spells
- Winterleaf: 22 spells
- PermafrostEssence: 22 spells
- FrozenSoul: 22 spells
- ArcticPearl: 20 spells
- GlacierCrystal: 16 spells
- Frostbloom: 16 spells
- **❌ EternalIce: 14 spells** ← **NOT IN BAG!**
- **❌ HeartOfWinter: 10 spells** ← IN BAG
- FrozenOre: 2 spells (resource, not reagent)

### THE PROBLEM

**EternalIce** is used by 14 Ice Mage spells but:
1. ❌ Not defined in `IceMagicReagents.cs`
2. ❌ Not included in `IceMagicReagentBag`
3. ❌ Cannot be spawned via [spawnvystia gump

**Result:** Players cannot cast 14 Ice Mage spells!

---

## Affected Spells (14 total)

The following Ice Mage spells require **EternalIce** and will fail:

1. Absolute Zero (Circle 8)
2. Blizzard (Circle 7)
3. Chill Aura (Circle 2)
4. Cocytus Prison (Circle 8)
5. Deep Freeze (Circle 5)
6. Flash Freeze (Circle 4)
7. Frost Armor (Circle 3)
8. Frost Nova (Circle 3)
9. Frozen Tomb (Circle 6)
10. Glacier Spike (Circle 1)
11. Hypothermia (Circle 6)
12. Ice Bolt (Circle 1)
13. Shatter (Circle 5)
14. Tundra (Circle 7)

---

## Root Cause Analysis

### Where EternalIce Was Supposed To Be

Looking at Vystia resources, **EternalIce** exists as a **special resource**, not a reagent:
- File: `Scripts/Items/Vystia/Resources/VystiaSpecialResources.cs`
- Type: Special craftable resource from Frosthold
- Purpose: Crafting material for weapons/armor

### What Happened

Ice Mage spells were likely designed to use **EternalIce** as a reagent, but:
1. It was never added to the Ice Magic reagent set
2. The 8-reagent system was implemented without it
3. Spells still reference it in CheckCast()

---

## Solution Options

### Option 1: Add EternalIce as 9th Reagent (RECOMMENDED)
**Pros:** Matches spell requirements exactly
**Cons:** Breaks 8-reagent-per-school pattern

**Implementation:**
1. Add EternalIce class to `IceMagicReagents.cs`
2. Add to `IceMagicReagentBag`
3. Update IceMageVendor to sell it

### Option 2: Replace EternalIce with Existing Reagent
**Pros:** Maintains 8-reagent pattern
**Cons:** Need to update 14 spell files

**Implementation:**
Replace `typeof(EternalIce)` with `typeof(FrostEssence)` in affected spells

### Option 3: Make EternalIce Obtainable As Resource
**Pros:** Adds gameplay depth (must mine/craft it)
**Cons:** More complex, harder for players to get

---

## Audit Results for All 12 Schools

Let me check if other schools have similar issues...

### Schools to Audit:
1. ✅ Ice Magic - **ISSUE FOUND** (EternalIce missing)
2. ⏳ Nature Magic (Druid)
3. ⏳ Hex Magic (Witch)
4. ⏳ Elemental Magic (Sorcerer)
5. ⏳ Dark Magic (Warlock)
6. ⏳ Divination Magic (Oracle)
7. ⏳ Necromancy
8. ⏳ Summoning Magic
9. ⏳ Shamanic Magic
10. ⏳ Bardic Magic
11. ⏳ Enchanting Magic
12. ⏳ Illusion Magic

---

## Immediate Action Required

**CRITICAL FIX NEEDED:**
Either add EternalIce to the Ice Magic reagent system OR replace it in all 14 affected spell files.

**Recommended:** Option 1 (add as 9th reagent) - fastest and preserves original spell design.

---

*Audit in progress - checking remaining 11 schools...*
