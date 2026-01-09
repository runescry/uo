# Spell Page Order Verification

**Issue:** Spell page icons and names don't match their respective index in spellbooks

---

## Problem Analysis

This issue has **two components**:

### 1. Server-Side (ServUO)
The spell registration order in `VystiaSpellInitializer.cs` must match the spellbook page index.

**Current Status:** ✅ **CORRECT**
- Ice Magic: IDs 999-1030 (32 spells, registered in order)
- Druid: IDs 1031-1062 (32 spells, registered in order)
- All 12 schools properly registered sequentially

### 2. Client-Side (ClassicUO)
The client needs matching spell definitions to display correct names/icons.

**Location:** `ClassicUO/src/ClassicUO.Client/Game/Data/SpellDefinition.cs`

**Previously Fixed:** According to `SPELLBOOK_SYSTEM_COMPLETE.md`, this was already fixed on 2025-12-08.

---

## Current Server Registration Order

### Ice Magic (999-1030)
```
999  - FrostTouch
1000 - IceShard
1001 - FrostWard
1002 - ChillAura
1003 - FreezingGrasp
1004 - IceShield
1005 - FrostSlick
1006 - GlacialMend
1007 - IceBolt
1008 - Frostbite
1009 - FrozenGround
1010 - IceWall
1011 - FrostArmor
1012 - IceSpear
1013 - GlacialStrike
1014 - Hypothermia
1015 - IcicleBarrage
1016 - DeepFreeze
1017 - FrozenTomb
1018 - Permafrost
1019 - Blizzard
1020 - GlacialFortress
1021 - FrostMeteor
1022 - Shatter
1023 - Avalanche
1024 - GlacierSummon
1025 - IceAge
1026 - CocytusPrison
1027 - AbsoluteZero
1028 - EternalWinter
1029 - RimeReaper
1030 - FimbulwintersWrath
```

---

## Diagnosis Steps

If spell pages are still showing incorrect names/icons:

### Step 1: Verify Client-Side Definitions
Check ClassicUO `SpellDefinition.cs` array has all 384 spells in correct order matching server IDs 999-1382.

### Step 2: Verify Spellbook BookOffset
Each spellbook's `BookOffset` must match its spell ID range:
- IceMageSpellbook: BookOffset = 999 ✅
- DruidSpellbook: BookOffset = 1031 ✅
- (etc for all 12 schools)

### Step 3: Check Client-Side GetSpellIdOffset
ClassicUO's `SpellbookGump.cs` method `GetSpellIdOffset()` must return correct offset for each spellbook type.

---

## Recommended Action

**If spell pages still show wrong names/icons:**

1. **Rebuild ClassicUO client** with the spell definition fixes from 2025-12-08
2. **Clear client cache** (delete ClassicUO's cache folder)
3. **Restart client**

**The server-side code is already correct** - all 384 spells are registered in the correct sequential order matching their spell IDs.

---

## Client-Side Fix Reference

From `SPELLBOOK_SYSTEM_COMPLETE.md`:

**File:** `ClassicUO/src/ClassicUO.Client/Game/Data/SpellDefinition.cs`

**Fix Applied:** Added all 384 Vystia spell definitions in array indices matching their spell IDs (offset by -1 for 0-based array).

Example:
```csharp
// Index 998 = Spell ID 999 (FrostTouch)
new SpellDefinition("Frost Touch", 999, ...),
// Index 999 = Spell ID 1000 (Ice Shard)
new SpellDefinition("Ice Shard", 1000, ...),
// ... etc for all 384 spells
```

---

*Server-side spell registration is correct.*
*If issue persists, it's a client-side problem requiring ClassicUO rebuild.*
