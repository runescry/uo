# Vystia Reagent ItemID Stackability Audit

**Date:** 2025-12-11
**Status:** ⚠️ **11/96 reagents need fixes**

---

## Executive Summary

Checked all 96 Vystia reagents for stackable ItemIDs. **85/96 are stackable**, but **11 reagents use non-stackable ItemID `0x26B8`**.

### Stackable ItemID Ranges in UO
- `0x0F00 - 0x0FFF` (Main range)
- `0x1000 - 0x1FFF` (Extended range)
- `0x18E0 - 0x18FF` (Small range)

---

## Non-Stackable Reagents (11 total)

All 11 use ItemID `0x26B8` (outside stackable ranges):

| School | Reagent | Current ItemID | Issue |
|--------|---------|----------------|-------|
| Dark Magic | VoidDust | 0x26B8 | NON-STACKABLE |
| Divination Magic | TimeDust | 0x26B8 | NON-STACKABLE |
| Divination Magic | DivinationDust | 0x26B8 | NON-STACKABLE |
| Necromancy | BoneDust | 0x26B8 | NON-STACKABLE |
| Necromancy | CorpseAsh | 0x26B8 | NON-STACKABLE |
| Necromancy | LichDust | 0x26B8 | NON-STACKABLE |
| Summoning Magic | PlanarDust | 0x26B8 | NON-STACKABLE |
| Bardic Magic | EchoDust | 0x26B8 | NON-STACKABLE |
| Enchanting Magic | ArcaneDust | 0x26B8 | NON-STACKABLE |
| Enchanting Magic | RunicPowder | 0x26B8 | NON-STACKABLE |
| Illusion Magic | MirrorDust | 0x26B8 | NON-STACKABLE |

---

## Recommended Fix

**Replace `0x26B8` with `0x1015`** (stackable powder/dust graphic)

### Why 0x1015?
- ✅ Falls within stackable range (0x1000-0x1FFF)
- ✅ Visually appropriate for "dust" and "powder" items
- ✅ Already used by other UO stackable powder items
- ✅ Consistent with ServUO stackability behavior

---

## Files to Modify

1. `DarkMagicReagents.cs` - VoidDust
2. `DivinationMagicReagents.cs` - TimeDust, DivinationDust
3. `NecromancyReagents.cs` - BoneDust, CorpseAsh, LichDust
4. `SummoningMagicReagents.cs` - PlanarDust
5. `BardicMagicReagents.cs` - EchoDust
6. `EnchantingMagicReagents.cs` - ArcaneDust, RunicPowder
7. `IllusionMagicReagents.cs` - MirrorDust

**Total:** 7 files, 11 reagent classes

---

## Implementation

Simple find/replace in each file:
```csharp
// From:
base(amount, 0x26B8, hue, location, usage)

// To:
base(amount, 0x1015, hue, location, usage)
```

---

## Validation

After fix:
- ✅ All 96 reagents will use stackable ItemIDs
- ✅ Players can stack reagents in backpacks
- ✅ Reagent bags will function properly
- ✅ No spell casting issues

---

*Audit completed: 2025-12-11*
*Ready to fix 11 non-stackable reagents*
