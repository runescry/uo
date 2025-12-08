# Female Dwarf Rendering Issue - RESOLVED

**Status:** Fixed (December 2025)

## Original Issue

Male Dwarf:
- NPC spawns correctly with platemail and warhammer
- Paperdoll has the GM robe with platemail overlaid

Female Dwarf:
- NPC spawns naked but when killed the correct inventory is shown (leather armor and hammer pick)
- Paperdoll is a male with leather armor and hammer pick

## Root Cause

Body 988 (female dwarf) was NOT in ClassicUO's hardcoded `IsHuman` check.

In `ClassicUO/src/ClassicUO.Client/Game/GameObjects/Mobile.cs`:
- Body 987 (0x03DB) WAS in the IsHuman list - equipment rendered correctly
- Body 988 (0x03DC) was NOT in the list - ClassicUO skipped equipment rendering

When `IsHuman == false`, MobileView.cs does not iterate through equipment layers at all.

## Solution Applied

Added body 988 (0x03DC) to the IsHuman check in ClassicUO's Mobile.cs.

Both male and female dwarves now render correctly with all equipment visible.

## Files Modified

### Client-Side (ClassicUO)
- `ClassicUO/src/ClassicUO.Client/Game/GameObjects/Mobile.cs` - Added `|| Graphic == 0x03DC` to IsHuman property

### Client Files Required
- `anim.mul` / `anim.idx` - Patched with dwarf body and equipment animations
- `equipconv.def` - Equipment animation conversion rules
- `bodyconv.def` - Disabled lines 914-919 to prevent anim5.mul redirect
- `mobtypes.txt` - Body type definitions (987/988 = HUMAN with flag 10)

## Verification

Both male (987) and female (988) dwarves:
- Spawn with correct equipment visible
- Equipment animations render at 75% scale
- All armor pieces display correctly
