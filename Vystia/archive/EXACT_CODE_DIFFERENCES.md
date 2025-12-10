# EXACT Code Differences: Ice Magic (Working) vs Hex Magic (Not Working)

## Date: 2025-12-08

## Summary

Ice Magic has **FOUR additional case statements** in SpellbookGump.cs that all other Vystia spellbooks are MISSING.

---

## Difference 1: SetBookIcon() - Line 1090

**Location:** `C:\DevEnv\GIT\UO\ClassicUO\src\ClassicUO.Client\Game\UI\Gumps\SpellbookGump.cs:1090`

### Ice Magic (Lines 1090-1096) - EXISTS
```csharp
case SpellBookType.VystiaIceMagic:
    maxSpellsCount = SpellsVystiaIceMagic.MaxSpellCount;
    bookGraphic = 0x08AC; // Reuse Magery book graphic for display
    minimizedGraphic = 0x08BA;
    iconStartGraphic = 0x08C0; // Reuse Magery icons for now

    break;
```

### Hex Magic - **MISSING**
**NO CASE STATEMENT**

Falls through to default, meaning:
- `maxSpellsCount` is NOT set for VystiaWitch
- `bookGraphic` is NOT set for VystiaWitch
- `minimizedGraphic` is NOT set for VystiaWitch
- `iconStartGraphic` is NOT set for VystiaWitch

**IMPACT:** Spellbook doesn't know how many spells it has or what graphics to use.

---

## Difference 2: GetSpellToolTip() - Line 1152

**Location:** `C:\DevEnv\GIT\UO\ClassicUO\src\ClassicUO.Client\Game\UI\Gumps\SpellbookGump.cs:1152`

### Ice Magic (Lines 1152-1155) - EXISTS
```csharp
case SpellBookType.VystiaIceMagic:
    offset = 0; // No cliloc tooltips for Vystia spells yet

    break;
```

### Hex Magic - **MISSING**
**NO CASE STATEMENT**

Falls through to default (line 1157):
```csharp
default:
    offset = 0;

    break;
```

**IMPACT:** Minor - both end up with offset = 0, so this is NOT the bug.

---

## Difference 3: GetSpellID() - Line 1529

**Location:** `C:\DevEnv\GIT\UO\ClassicUO\src\ClassicUO.Client\Game\UI\Gumps\SpellbookGump.cs:1529`

### Ice Magic (Lines 1529-1530) - EXISTS
```csharp
case SpellBookType.VystiaIceMagic:
    return 1000; // Spells 1000-1031
```

### Hex Magic - **MISSING**
**NO CASE STATEMENT**

Falls through to default (line 1532):
```csharp
// TODO: Add remaining Vystia spell schools
default:
    return 0;
```

**IMPACT:** Returns 0 instead of 1064, breaking spell ID calculation.

---

## Difference 4: Spell Definition Lookup - Lines 948 & 1238

**Location 1:** Line 948 (GetSpellDefinition for spell icons)
**Location 2:** Line 1238 (GetSpellNames for spell pages)

### Ice Magic - EXISTS
```csharp
// Line 948
case SpellBookType.VystiaIceMagic:
    def = SpellsVystiaIceMagic.GetSpell(idx);
    break;

// Line 1238
case SpellBookType.VystiaIceMagic:
    def = SpellsVystiaIceMagic.GetSpell(offset + 1);
    name = def.Name;
    abbreviature = def.PowerWords;
    reagents = def.CreateReagentListString("\n");
    break;
```

### Hex Magic - EXISTS (SAME CODE)
```csharp
// Line 958
case SpellBookType.VystiaWitch:
    def = SpellsVystiaHex.GetSpell(idx);
    break;

// Line 1254
case SpellBookType.VystiaWitch:
    def = SpellsVystiaHex.GetSpell(offset + 1);
    name = def.Name;
    abbreviature = def.PowerWords;
    reagents = def.CreateReagentListString("\n");
    break;
```

**IMPACT:** None - these cases exist for both and are identical.

---

## Complete Comparison Table

| Switch Statement | Line | Ice Magic | Hex Magic | Impact if Missing |
|-----------------|------|-----------|-----------|-------------------|
| SetBookIcon() | 1090 | ✓ EXISTS | ❌ MISSING | **CRITICAL** - No spell count, no graphics |
| GetSpellToolTip() | 1152 | ✓ EXISTS | ❌ MISSING | Minor - falls to default (offset=0) |
| GetSpellID() | 1529 | ✓ EXISTS | ❌ MISSING | **CRITICAL** - Returns 0 instead of base ID |
| GetSpellDefinition() | 948 | ✓ EXISTS | ✓ EXISTS | None - both have it |
| GetSpellNames() | 1238 | ✓ EXISTS | ✓ EXISTS | None - both have it |

---

## THE ROOT CAUSE

### Missing Case #1 (Line 1090) - SetBookIcon()

Without this case, the spellbook doesn't initialize:
- `maxSpellsCount` - How many spell slots (should be 32)
- `bookGraphic` - What graphic to display
- `minimizedGraphic` - Minimized icon
- `iconStartGraphic` - Starting spell icon graphic

**Result:** Spellbook renders with default values (probably 0), showing no spells.

### Missing Case #2 (Line 1529) - GetSpellID()

This method returns the BASE spell ID for the spellbook type.

**For Ice Magic:** Returns 1000
**For Hex Magic:** Returns 0 (default)

This breaks all spell ID calculations that depend on knowing the base ID.

---

## THE FIX

Add the missing cases for all 11 remaining Vystia spellbooks:

### Fix Location 1: Line 1090 (after VystiaIceMagic case)

```csharp
case SpellBookType.VystiaDruid:
    maxSpellsCount = SpellsVystiaNature.MaxSpellCount;
    bookGraphic = 0x08AC;
    minimizedGraphic = 0x08BA;
    iconStartGraphic = 0x08C0;
    break;

case SpellBookType.VystiaWitch:
    maxSpellsCount = SpellsVystiaHex.MaxSpellCount;
    bookGraphic = 0x08AC;
    minimizedGraphic = 0x08BA;
    iconStartGraphic = 0x08C0;
    break;

case SpellBookType.VystiaSorcerer:
    maxSpellsCount = SpellsVystiaElemental.MaxSpellCount;
    bookGraphic = 0x08AC;
    minimizedGraphic = 0x08BA;
    iconStartGraphic = 0x08C0;
    break;

// ... continue for all 12 Vystia schools
```

### Fix Location 2: Line 1529 (after VystiaIceMagic case)

```csharp
case SpellBookType.VystiaDruid:
    return 1032; // Spells 1032-1063

case SpellBookType.VystiaWitch:
    return 1064; // Spells 1064-1095

case SpellBookType.VystiaSorcerer:
    return 1096; // Spells 1096-1127

case SpellBookType.VystiaWarlock:
    return 1128; // Spells 1128-1159

case SpellBookType.VystiaOracle:
    return 1160; // Spells 1160-1191

case SpellBookType.VystiaNecromancer:
    return 1192; // Spells 1192-1223

case SpellBookType.VystiaSummoner:
    return 1224; // Spells 1224-1255

case SpellBookType.VystiaShaman:
    return 1256; // Spells 1256-1287

case SpellBookType.VystiaBard:
    return 1288; // Spells 1288-1319

case SpellBookType.VystiaEnchanter:
    return 1320; // Spells 1320-1351

case SpellBookType.VystiaIllusionist:
    return 1352; // Spells 1352-1383
```

---

## VERIFICATION

After applying the fix, all 12 Vystia spellbooks should:
1. Display the correct number of spell slots (32)
2. Show spell icons in the index
3. Show spell pages when clicked
4. Return correct base spell IDs

The TODO comment at line 1531 can be removed once all cases are added.
