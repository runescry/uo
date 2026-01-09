# Vystia Character Classes - Known Issues

## Overview
This document tracks known bugs, compilation errors, and technical debt in the Vystia character class system.

**Last Updated:** 2025-12-11
**Status:** 0 compilation errors, 0 code warnings, 0 critical bugs

**Major Completions (2025-12-11):**
- All 384 spells implemented and functional across 12 magic schools
- All 25 character classes fully implemented
- All 26 custom skills integrated (server + client)
- Spell skill gains working via CheckFizzle pattern

---

## 🔴 Critical Issues (Build Blockers)

**NONE** - All critical issues resolved! ✅

---

## 🟢 Recently Resolved Issues

### ✓ Build Warning Cleanup (2025-12-09)
**Status:** ✅ FULLY RESOLVED - ALL CODE WARNINGS ELIMINATED
**Date Fixed:** 2025-12-09
**Severity:** Low (code quality improvement)
**Category:** Build Quality

**Problem:** 45 compiler warnings (35 CS0108 + 10 CS0162) were cluttering the build output and potentially masking real issues.

**CS0108 Warnings (24 occurrences):**
- **Files:** 4 faction leader NPCs (ArchmagePyrusAshborn, SultanAziralRashid, ChieftainBjornFrostbeard, ElderSeraphinaLeafwhisper)
- **Issue:** Properties/methods hiding inherited members from BaseVendor without `new` keyword
- **Affected Members:** LLMConversationEnabled, PersonalityType, SpeechPattern, HearingRange, ShouldHandleConversation(), HandleConversation()

**CS0162 Warnings (10 occurrences):**
- **Files:** 3 vendor NPCs (IronhavenBanker, IronhavenGuardCaptain, FrostholmHealer)
- **Issue:** Hardcoded switch statements (e.g., `switch("Banker")`) causing all other case blocks to be unreachable code
- **Impact:** Dead code cluttering the source files

**Solutions Applied:**

**1. Added `new` Keyword to Hiding Members**
- Added `new` modifier to all 6 properties/methods in each faction leader file
- Explicitly indicates intentional member hiding
- Example:
```csharp
// Before:
public bool LLMConversationEnabled { get; set; } = true;

// After:
public new bool LLMConversationEnabled { get; set; } = true;
```

**2. Removed Unnecessary Switch Statements**
- Replaced hardcoded switch with direct code for each vendor type
- Eliminated unreachable case blocks
- Simplified vendor appearance setup
- Example:
```csharp
// Before (IronhavenBanker.cs):
switch ("Banker")
{
    case "Banker":
        AddItem(new FancyShirt(Hue));
        break;
    case "Healer": // unreachable
        AddItem(new Robe(0x47E));
        break;
    // ... more unreachable cases
}

// After:
// Banker appearance
AddItem(new FancyShirt(Hue));
AddItem(new LongPants(Hue));
AddItem(new Boots());
```

**Files Modified:**
- `Scripts/Mobiles/Vystia/NPCs/FactionLeaders/ArchmagePyrusAshborn.cs`
- `Scripts/Mobiles/Vystia/NPCs/FactionLeaders/SultanAziralRashid.cs`
- `Scripts/Mobiles/Vystia/NPCs/FactionLeaders/ChieftainBjornFrostbeard.cs`
- `Scripts/Mobiles/Vystia/NPCs/FactionLeaders/ElderSeraphinaLeafwhisper.cs`
- `Scripts/Mobiles/Vystia/NPCs/Vendors/IronhavenBanker.cs`
- `Scripts/Mobiles/Vystia/NPCs/Vendors/IronhavenGuardCaptain.cs`
- `Scripts/Mobiles/Vystia/NPCs/Vendors/FrostholmHealer.cs`

**Test Results:**
✅ **Clean Build Achieved:**
- 0 CS0108 warnings (was 35)
- 0 CS0162 warnings (was 10)
- 0 compilation errors
- Only file locking warnings remain (expected when server is running)

**Result:** Build output is now clean and professional, making it easier to spot real issues!

---

### ✓ Ice Magic Spellbook Client/Server Integration (2025-12-06)
**Status:** ✅ FULLY RESOLVED - ALL CASTING METHODS WORKING
**Date Fixed:** 2025-12-06
**Test Results:** ✅ Double-click icon works, ✅ Index page click works, ✅ Drag spell works
**Severity:** Critical (build blocker - prevented spell casting)
**Category:** Client/Server Integration

**Problem:** Ice Magic spellbook was not functional in ClassicUO client. Spells showed "You do not have that spell" error when casting.

**Root Causes Identified:**
1. **SpellRegistry Array Size Too Small** - Registry array was 745 slots, but Ice Magic uses IDs 999-1030
2. **Double Spell Registration** - Initialize() was being called twice on server startup
3. **GetTypeForSpell() Missing Ice Magic** - Spell ID lookup couldn't identify Ice Magic spellbook type
4. **Client UI Issue** - Only double-clicking spell icons worked, not index page or dragging

**Investigation Process:**
Through systematic debugging with extensive logging, we traced the complete spell casting flow:
- Client → Server packet transmission (packet 0x27)
- Server spell validation (HasSpell checks)
- Spell registry lookup (NewSpell instantiation)
- Spellbook type detection (GetTypeForSpell)

**Solutions Applied:**

**1. SpellRegistry Array Expansion**
- **File:** `Scripts/Spells/Base/SpellRegistry.cs`
- **Change:** Increased `m_Types` array from 745 → 1500 slots
- **Reason:** Accommodate all Vystia spells (IDs 999-1383 for 12 magic schools × 32 spells)
- **Code:**
```csharp
// OLD: private static readonly Type[] m_Types = new Type[745];
// NEW: private static readonly Type[] m_Types = new Type[1500];
```

**2. Duplicate Registration Guard**
- **File:** `Scripts/Custom/VystiaClasses/Spells/VystiaSpellInitializer.cs`
- **Change:** Added static flag to prevent double-initialization
- **Reason:** Initializer.Initialize() was being called twice during server startup
- **Code:**
```csharp
private static bool _initialized = false;

public static void Initialize()
{
    if (_initialized)
    {
        Console.WriteLine("[VYSTIA] WARNING: Initialize() called again, skipping duplicate registration!");
        return;
    }
    _initialized = true;
    // ... registration code
}
```

**3. Spellbook Type Detection**
- **File:** `Scripts/Items/Equipment/Spellbooks/Spellbook.cs`
- **Change:** Added Ice Magic spell ID range to `GetTypeForSpell()`
- **Reason:** Server couldn't identify which spellbook type to search for when client doesn't send book serial
- **Code:**
```csharp
// Added to GetTypeForSpell() method:
else if (spellID >= 999 && spellID < 1032)
{
    return SpellbookType.VystiaIceMage;  // Ice Magic (IDs 999-1031)
}
```
- **Impact:** Enables casting from index pages and spell dragging

**4. Comprehensive Logging Added**
Added debug logging throughout the spell casting pipeline:
- Client: SpellbookGump, GameActions (packet sending)
- Server: PacketHandlers (packet 0x27 parsing), Spellbook (validation), SpellRegistry (instantiation)
- Logs show complete spell casting flow for debugging

**Test Results:**
✅ **All Three Casting Methods Now Work:**
1. Double-clicking spell icon on spell pages ✓
2. Clicking spell name on index/dictionary pages ✓
3. Dragging spell icon to create spell button ✓

**Files Modified:**
- `ClassicUO/src/ClassicUO.Client/Game/UI/Gumps/SpellbookGump.cs` - Added logging
- `ClassicUO/src/ClassicUO.Client/Game/GameActions.cs` - Added logging
- `ServUO/Scripts/Spells/Base/SpellRegistry.cs` - Array size + logging
- `ServUO/Scripts/Custom/VystiaClasses/Spells/VystiaSpellInitializer.cs` - Duplicate guard + logging
- `ServUO/Scripts/Items/Equipment/Spellbooks/Spellbook.cs` - Ice Magic type detection + logging
- `ServUO/Server/Network/PacketHandlers.cs` - Packet 0x27 parsing logging

**Diagnostic Tools Created:**
```
[CLIENT] CastSpellFromBook logs - Track spell ID and book serial sent
[SPELLBOOK] GetSpellDefinition logs - Verify spell lookup
[PACKET 0x27] logs - Show packet parsing
[SERVER] CastSpellRequest logs - Show server-side validation
[SERVER] HasSpell logs - Show bitmask checking
[REGISTRY] NewSpell logs - Show spell instantiation
[FIND] logs - Show spellbook type detection
```

**Result:** Ice Magic spellbook is now 100% functional in client and server!

---

### ✓ Ice Magic 100% Complete (2025-12-05 to 2025-12-06)
**Status:** ✅ Fully Implemented
**Date Started:** 2025-12-05 (API fixes)
**Date Completed:** 2025-12-06 (all 32 spells)
**Severity:** High (was build blocker, now fully functional)
**Category:** Spell Implementation

**Phase 1: API Fix (2025-12-05)**
Fixed 3 existing Ice Mage spells that had API incompatibility issues:
- `Spells/IceMage/IceBoltSpell.cs`
- `Spells/IceMage/FrostArmorSpell.cs`
- `Spells/IceMage/BlizzardSpell.cs`

**Problem:** The custom spells were trying to override properties/methods that don't exist in ServUO's `Spell` base class. They were written using RunUO/ModernUO API instead of ServUO's API.

**Solution Applied:**
1. Changed inheritance from `Spell` to `MagerySpell`
2. Changed namespace to `Server.Spells.VystiaSpells.IceMage`
3. Removed incorrect override properties (CastDelay, RequiredSkill, RequiredMana)
4. Fixed damage calculations to use `GetNewAosDamage()`
5. Fixed ResistanceMod usage (2-arg constructor, object-based removal)
6. Added spell reflection support
7. Updated target types to `IDamageable`

**Result:** All 18 compilation errors resolved - clean build achieved.

**Phase 2: Complete Implementation (2025-12-06)**
Implemented all remaining 29 Ice Magic spells (Circles 1-8):

**New Files Created (29 spells):**
- Circle 1: FrostTouchSpell.cs, IceShardSpell.cs, FrostWardSpell.cs, ChillAuraSpell.cs
- Circle 2: FreezingGraspSpell.cs, IceShieldSpell.cs, FrostSlickSpell.cs, GlacialMendSpell.cs
- Circle 3: IcyBurstSpell.cs, FrozenGroundSpell.cs, FrostNovaSpell.cs
- Circle 4: IceWallSpell.cs, IcicleBarrageSpell.cs, PermafrostSpell.cs
- Circle 5: GlacialStrikeSpell.cs, FrozenTombSpell.cs, ShatterSpell.cs, HypothermiaSpell.cs
- Circle 6: GlacialFortressSpell.cs, AvalancheSpell.cs, DeepFreezeSpell.cs
- Circle 7: AbsoluteZeroSpell.cs, GlacierSummonSpell.cs, EternalWinterSpell.cs, FimbulwintersWrathSpell.cs
- Circle 8: FrostMeteorSpell.cs, IceAgeSpell.cs, RimeReaperSpell.cs, CocytusPrisonSpell.cs

**Key Features Implemented:**
- Slow/freeze effects (multiple tiers)
- Ice walls and terrain control (IceWallItem, FrozenGroundEffect, etc.)
- AoE damage (Blizzard, Absolute Zero, Ice Age)
- Ice barriers and fortresses (GlacialFortress)
- Summons (Ice Elemental via GlacierSummon)
- Execute mechanics (Rime Reaper instant kill below 20% HP)
- Transformations (Fimbulwinter's Wrath)
- Screen-wide devastation (Ice Age 30-tile radius)
- Complex mechanics (knockback, line projection, cone targeting)

**All spells use:**
- MagerySpell inheritance (correct ServUO API)
- Vystia ice reagents (Frostbloom, Winterleaf, Glacier Crystal, Permafrost Essence, Arctic Pearl, Frozen Soul, Eternal Ice, Heart of Winter)
- Spell reflection support
- Proper damage/buff/debuff mechanics
- Visual effects (0x481 ice blue hue)
- Sound effects
- StatMod/ResistanceMod for temporary effects
- Timer.DelayCall for durations and DoT
- IPooledEnumerable for AoE targeting
- Ground targeting with IPoint3D

**Documentation Updated:**
- `Vystia/Magic/IceMagic.md` - Complete rewrite with "What You See" and "How It Works" sections
- `Vystia/Magic/README.md` - Updated implementation status
- `ServUO/Scripts/Custom/VystiaClasses/IMPLEMENTATION_SUMMARY.md` - Updated spell counts
- `ServUO/Scripts/Custom/VystiaClasses/README.md` - Updated class status
- `ServUO/Scripts/Custom/VystiaClasses/KNOWN_ISSUES.md` - This file

**Final Result:** Ice Magic is 100% complete (32/32 spells) and fully functional!

---

### ✓ Custom Spellbooks Already Extracted (2025-12-06)
**Status:** ✅ Resolved
**Date Fixed:** 2025-12-06
**Severity:** N/A (was not actually an issue)
**Category:** Code Organization

**Investigation:** Upon attempting to extract spellbooks from class files, discovered that comprehensive spellbook implementations already exist in `Scripts/Items/Equipment/Spellbooks/VystiaSpellbooks.cs`.

**Existing Implementation:**
- All 12 Vystia magic school spellbooks fully implemented in `Server.Items` namespace
- Each spellbook has custom SpellbookType enum values
- Custom BookOffset values for spell ID ranges (1000-1383)
- Special bonuses when equipped (+damage, +duration, etc.)
- Better naming (e.g., "Tome of Frozen Arts" vs "Ice Mage Spellbook")
- Proper hues matching regional themes

**Spellbooks in VystiaSpellbooks.cs:**
1. IceMageSpellbook - Tome of Frozen Arts (Frosthold)
2. DruidSpellbook - Codex of the Wild (Verdantpeak)
3. WitchSpellbook - Grimoire of Shadowfen Hexes (Shadowfen)
4. SorcererSpellbook - Tome of Elemental Fury (Emberlands)
5. WarlockSpellbook - Codex of Shadows (ShadowVoid)
6. OracleSpellbook - Crystal Codex (Crystal Barrens)
7. VystiaNecromancerSpellbook - Necronomicon (ShadowVoid)
8. SummonerSpellbook - Codex of Binding (Underwater)
9. ShamanSpellbook - Tome of Spirits (Skyreach/Wilderlands)
10. BardSpellbook - Songbook of Legends (Multi-regional)
11. EnchanterSpellbook - Codex of Enhancement (Multi-regional)
12. IllusionistSpellbook - Tome of Deception (Multi-regional)

**Action Taken:**
- Confirmed embedded spellbook definitions in class files are simple placeholders
- Class files already reference the comprehensive spellbooks from `Server.Items`
- No extraction needed - system already uses the better implementations

**Result:** No changes required. The existing spellbook system is superior to what was embedded in class files.

---

## 🟢 Low Priority Issues

### 3. Testing Commands Partially Implemented
**Status:** 🟡 In Progress
**Severity:** Low
**Category:** Developer Tools

**Problem:** Some GM commands exist for testing but not all.

**Implemented Commands:**
```
[rvs / [resetvystiaskills  - Reset all 26 Vystia skills to 0.0
[svs <value>               - Set all Vystia skills to specified value
[skillcap [value]          - Get/set Mobile.SkillsCap
[skillinfo                 - Show skill total breakdown
[spellbook <type>          - Give specific spellbook
```

**Still Needed:**
```csharp
[SetClass <classname>     // Assign class to targeted player
[ResetClass               // Remove class from targeted player
[ShowClassStats           // Display class info
```

**Impact:** Most testing can be done, but class assignment still manual.

**Priority:** Low

**Estimated Effort:** Low (1-2 hours)

---

## 🟢 Resolved Issues

### ✓ Duplicate Class Definitions (Fixed 2025-12-05)
**Status:** ✅ Resolved
**Date Fixed:** 2025-12-05

**Problem:** RageTotem, ConstructControlDevice, ShapeshiftTotem, HolySymbol, ArtificerBlueprints, and ClockworkScout were defined inline in class files, causing potential duplicate definitions.

**Solution:**
1. Extracted all 6 items to standalone files in `Items/AbilityItems/` and `Items/Creatures/`
2. Added namespace `Server.Items.VystiaClassItems`
3. Updated class files with `using Server.Items.VystiaClassItems;`
4. Removed embedded class definitions
5. Build verified - 0 duplicate definition errors

**Files Created:**
- `Items/AbilityItems/RageTotem.cs`
- `Items/AbilityItems/ConstructControlDevice.cs`
- `Items/AbilityItems/ShapeshiftTotem.cs`
- `Items/AbilityItems/HolySymbol.cs`
- `Items/AbilityItems/ArtificerBlueprints.cs`
- `Items/Creatures/ClockworkScout.cs`

---

### ✓ Incorrect Resource Names (Fixed 2025-12-05)
**Status:** ✅ Resolved
**Date Fixed:** 2025-12-05

**Problem:** Class implementations used non-existent resource names that didn't match existing Vystia resources.

**Errors Fixed:**
- Ice Mage: `FrostOre` → `FrozenOre`, `GlacialIngot` → `FrostforgedIngot`
- Artificer: `ClockworkIngot` → `SteamworkOre`, `GearSpring` → `ClockworkGear + ClockworkSpring`
- Witch: `SwampMoss` → `SwampLotus`, `RotlungSpore` → `BogIronOre`

**Files Updated:**
- `IceMageClass.cs`
- `ArtificerClass.cs`
- `AllClasses.cs`

**Result:** All resource names now match existing Vystia resource system.

---

## Issue Summary

| Category | Count | Status |
|----------|-------|--------|
| Build Blockers | 0 | ✅ All Clear |
| Medium Priority | 0 | ✅ All Clear |
| Low Priority | 1 | 🟡 Testing commands |
| Resolved | 7 | ✅ Fixed |
| **Total** | **8** | **Mostly Complete** |

---

## Build Error Count

| Type | Count | Notes |
|------|-------|-------|
| Compilation Errors | 0 ✅ | All resolved |
| Code Warnings | 0 ✅ | Fixed 2025-12-09 (was 45) |
| Resource Errors | 0 ✅ | Fixed 2025-12-05 |
| Duplicate Definitions | 0 ✅ | Fixed 2025-12-05 |

**Build Status:** ✅ **CLEAN BUILD** - All errors and warnings resolved!

---

## Completion Status

### ✅ COMPLETE (2025-12-11)
1. ✅ All 384 spells implemented across 12 magic schools
2. ✅ All 25 character classes implemented
3. ✅ All 12 spellbooks functional (client + server)
4. ✅ All 26 custom skills integrated
5. ✅ Spell skill gains via CheckFizzle pattern
6. ✅ Combat skill gains via VystiaResourceManager
7. ✅ Skill management GM commands (`[rvs`, `[svs`, `[skillcap`, `[skillinfo`)
8. ✅ All equipment (172 items) implemented
9. ✅ All reagents (96) and scrolls (384) implemented

### 🔄 Future Development
1. Class selection system integration
2. Class-specific quest lines
3. PvP balance testing
4. Talent trees/specializations
5. Class halls and trainers

---

## Documentation Status

| Document | Status | Last Updated |
|----------|--------|--------------|
| README.md | ✅ Current | 2025-12-06 |
| IMPLEMENTATION_SUMMARY.md | ⚠️ Needs Update | 2025-12-07 |
| RESOURCE_CORRECTIONS.md | ✅ Current | 2025-12-05 |
| KNOWN_ISSUES.md | ✅ Current | 2025-12-11 |
| Vystia/Magic/README.md | ✅ Current | 2025-12-06 |
| Vystia/CUSTOM_SKILLS_COMPLETE.md | ✅ NEW | 2025-12-11 |
| CLAUDE.md (project root) | ⚠️ Needs Update | 2025-12-09 |

**Archived Documents (moved to Vystia/archive/):**
- CUSTOM_SKILLS_PLAN.md (skills now fully implemented)
- MISSING_ITEMS_TODO.md (items all implemented)

---

## Reporting New Issues

When reporting issues:
1. Add new issue to appropriate severity section
2. Use format: **Status:** ❌/⚠️/🟡 + **Severity:** High/Medium/Low
3. Include affected files, error messages, and steps to reproduce
4. Update issue summary table
5. Update "Last Updated" date

---

*This document is maintained as issues are discovered and resolved.*
*Report issues to: Documentation tracking only - no external bug tracker*
