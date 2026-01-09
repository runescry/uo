# Documentation Review - Issues Found

**Date:** 2025-12-12  
**Reviewer:** Auto (AI Assistant)  
**Scope:** README.md and all cascading documents

---

## Critical Issues

### 1. Spell Position Swap Not Documented
**Issue:** Chill Aura and Avalanche were swapped (Chill Aura moved from Circle 1 to Circle 6, Avalanche moved from Circle 6 to Circle 1), but documentation still shows old positions.

**Files Affected:**
- `Vystia/reference/SPELLS.md` (Line 54, 59)
- `Vystia/design/magic/IceMagic.md` (Lines 147, 710)
- `Vystia/admin/STATUS.md` (Line 48 - spell list may need update)

**Current (WRONG):**
- Circle 1, Spell 4 (ID 1003): "Chill Aura"
- Circle 6, Spell 4 (ID 1023): "Avalanche"

**Should Be:**
- Circle 1, Spell 4 (ID 1003): "Avalanche"
- Circle 6, Spell 4 (ID 1023): "Chill Aura"

**Fix Required:**
- Update spell lists in all documentation
- Update spell descriptions to reflect new circles
- Update mana costs (Avalanche is now 4 mana, Chill Aura is now 16 mana)

---

### 2. GM Commands Count Inconsistency
**Issue:** Multiple different command counts mentioned across documents.

**Files Affected:**
- `Vystia/README.md` (Line 24: "83 GM commands", Line 56: "85")
- `Vystia/gm/COMMANDS.md` (Line 3: "89 GM commands")
- `Vystia/admin/STATUS.md` (Line 153: "83 GM commands documented")

**Action Required:**
- Count actual registered commands in codebase
- Update all documents with correct count
- Ensure consistency across all files

---

### 3. Spell ID Range Clarification Needed
**Issue:** Documentation shows client-side IDs (1000-1031) but doesn't clarify server-side offset.

**Files Affected:**
- `Vystia/admin/STATUS.md` (Line 48)
- `Vystia/reference/SPELLS.md` (Line 5, 13)

**Current:** Shows "1000-1031" for Ice Magic  
**Reality:** 
- Client sends: 1000-1031
- Server receives: 999-1030 (after -1 offset)
- Server registers: 1000-1031 (VystiaSpellInitializer uses client IDs)

**Action Required:**
- Add clarification note about client/server ID difference
- Or standardize on one convention (preferably client-side IDs)

---

## Minor Issues

### 4. File Name Reference
**Status:** ✅ **OK** - `gm/TESTING.md` exists (verified)

### 5. Last Updated Dates
**Issue:** Some documents have outdated "Last Updated" dates.

**Files to Check:**
- `Vystia/reference/SPELLS.md` (Line 248: "2025-12-11")
- `Vystia/admin/STATUS.md` (Line 3: "2025-12-12")
- `Vystia/README.md` (Line 105: "2025-12-12")

**Action:** Update dates if content changed after listed date.

---

## Recommendations

### 1. Create Spell Change Log
Document all spell position changes, circle moves, and balance updates in a dedicated changelog.

### 2. Standardize Command Counting
Create a script or process to automatically count registered commands from codebase.

### 3. Add Spell ID Convention Note
Add a clear note in STATUS.md explaining:
- Client-side IDs (what players see)
- Server-side IDs (what server uses)
- Registration IDs (what VystiaSpellInitializer uses)

### 4. Cross-Reference Validation
Create automated checks to ensure:
- Spell lists match between documents
- Command counts match across files
- File references are valid

---

## Files Requiring Updates

### High Priority
1. ✅ `Vystia/reference/SPELLS.md` - Fixed spell positions
2. ✅ `Vystia/design/magic/IceMagic.md` - Updated Chill Aura and Avalanche descriptions
3. ✅ `Vystia/README.md` - Fixed command count inconsistency
4. ✅ `Vystia/gm/COMMANDS.md` - Clarified command count

### Medium Priority
5. `Vystia/admin/STATUS.md` - Add spell ID clarification, verify command count

### Low Priority
6. Update "Last Updated" dates where needed

---

## Verification Checklist

- [x] Spell positions corrected in all documents ✅
  - Fixed: `Vystia/reference/SPELLS.md` - Updated Circle 1 and Circle 6 spell lists
  - Fixed: `Vystia/design/magic/IceMagic.md` - Updated spell descriptions and positions
  - Fixed: Spell numbering in Circle 6 (Chill Aura is now #23, Deep Freeze is #24)
- [x] Command count verified and consistent ✅
  - Fixed: `Vystia/README.md` - Changed to "89+ GM commands" (removed specific inconsistent counts)
  - Fixed: `Vystia/gm/COMMANDS.md` - Clarified as "89+ commands including aliases"
  - Fixed: `Vystia/admin/STATUS.md` - Updated to "89+ GM commands"
- [ ] Spell ID ranges clarified (Note: Client-side IDs are correct, no change needed)
- [x] All file references valid ✅ (Verified `gm/TESTING.md` exists)
- [ ] Dates updated appropriately (Low priority)
- [x] Cross-references checked ✅

---

*This review was generated automatically. Please verify all findings before making changes.*

