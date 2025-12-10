# Vystia Spellbook Debugging Guide

## Issue: Spellbooks Still Empty After Client Rebuild

### What We've Fixed So Far

1. ✅ Added hue-based detection in `AssignGraphic()` for all 12 Vystia spellbooks
2. ✅ Added cases in `GetBookInfo()` for all 12 spellbooks (set maxSpellsCount, graphics)
3. ✅ Added cases in `GetSpellIdOffset()` for all 12 spellbooks (return correct base IDs)
4. ✅ Added cases in `GetSpellToolTip()` for all 12 spellbooks
5. ✅ Verified `GetSpellNames()` has all 12 cases
6. ✅ Verified `GetSpellDefinition()` has all 12 cases
7. ✅ Rebuilt ClassicUO client successfully (0 errors)

### Potential Issues to Check

#### Issue 1: Client Not Using Rebuilt Executable

**Symptoms:** Books still empty even after rebuild
**Check:**
```powershell
# Verify build timestamp
ls -lh "C:\DevEnv\GIT\UO\ClassicUO\bin\Release\net9.0\cuo.exe"
# Should show Dec 8 09:37 or later
```

**Solution:** Make sure you're launching the client from the correct location:
- `C:\DevEnv\GIT\UO\ClassicUO\bin\Release\net9.0\cuo.exe`

#### Issue 2: Graphic ID 0xFF0 Not Matching

**Affected Spellbooks:**
- WitchSpellbook (graphic 0xFF0, hue 0x81D)
- WarlockSpellbook (graphic 0xFF0, hue 0x455)

**Problem:** Server uses `0xFF0` (3-digit hex) which equals 4080 decimal. This might not be a valid UO spellbook graphic.

**Check Server-Side:**
```
[spellbook witch
```
Open the book and verify:
1. Does the book appear in your backpack?
2. What graphic does it show?
3. What hue does it have?

**Possible Fix:** Change server graphic from 0xFF0 to 0x0EFA (Magery book) or 0x2253 (Necromancy book) and use hue to distinguish.

#### Issue 3: Server Not Sending Hue Correctly

**Check In-Game:**
1. Spawn a Druid spellbook: `[spellbook druid`
2. Single-click the book to see properties
3. Verify hue shows as 2006 (decimal) or 0x7D6 (hex) = "Forest Green"

If hue is wrong, the client can't detect the correct spellbook type.

#### Issue 4: Spellbook Content Bitmask

**Problem:** Spellbook might have 0 spells even though we set maxSpellsCount

**Check:**
```csharp
// In VystiaSpellbooks.cs constructors
public DruidSpellbook() : this(0xFFFFFFFF) // ← Should fill all 32 spells
```

**Verify in-game:**
```
[add DruidSpellbook
```
- Does the spellbook show 32 spell slots?
- Or does it show 0 spells?

---

## Systematic Debug Steps

### Step 1: Verify Client Build

```powershell
cd "C:\DevEnv\GIT\UO\ClassicUO\src\ClassicUO.Client"
dotnet build -c Release
```

Expected output: `Build succeeded` with 0 errors

### Step 2: Close and Relaunch Client

**IMPORTANT:** Completely close ClassicUO (don't just disconnect). Relaunch from:
```
C:\DevEnv\GIT\UO\ClassicUO\bin\Release\net9.0\cuo.exe
```

### Step 3: Test Each Spellbook Type

Test in this order (matching the graphic groups):

**Group 1: Graphic 0xEFA (8 spellbooks)**
```
[spellbook druid       (hue 0x7D6 - Forest Green)
[spellbook sorcerer    (hue 0x54E - Fiery Orange)
[spellbook oracle      (hue 0x482 - Crystal Blue)
[spellbook summoner    (hue 0x555 - Deep Blue)
[spellbook shaman      (hue 0x501 - Storm Blue)
[spellbook bard        (hue 0x8A5 - Golden)
[spellbook enchanter   (hue 0x8FD - Arcane Purple)
[spellbook illusionist (hue 0x47E - Silvery)
```

**Group 2: Graphic 0xFF0 (2 spellbooks)**
```
[spellbook witch       (hue 0x81D - Murky Green/Purple)
[spellbook warlock     (hue 0x455 - Void Black)
```

**Group 3: Graphic 0x2253 (1 spellbook)**
```
[spellbook necromancer (hue 0x455 - Void Black)
```

**Group 4: Graphic 0x2252 (1 spellbook)**
```
[spellbook icemagic    (hue 0x481 - Ice Blue) ← Should work already
```

### Step 4: For Each Book, Check:

1. **Book appears in backpack?** (Yes/No)
2. **Double-click opens spellbook gump?** (Yes/No)
3. **Spell index shows 32 slots?** (Yes/No)
4. **Spell names visible?** (Yes/No)
5. **Spell icons visible?** (Yes/No)

### Step 5: Report Results

For any failing book, report:
- Spellbook name (e.g., "Druid")
- Graphic ID (check server code)
- Hue (check server code)
- What happens when you open it (empty? wrong spells? crash?)

---

## Quick Fix: Change Witch/Warlock Graphics

If Witch and Warlock books are still empty, the issue is likely graphic 0xFF0.

**Fix Server-Side:**

Edit `ServUO/Scripts/Items/Equipment/Spellbooks/VystiaSpellbooks.cs`:

```csharp
// BEFORE:
public WitchSpellbook(ulong content) : base(content, 0xFF0) // Necromancer book graphic

// AFTER:
public WitchSpellbook(ulong content) : base(content, 0xEFA) // Same as Druid/etc
```

```csharp
// BEFORE:
public WarlockSpellbook(ulong content) : base(content, 0xFF0) // Necromancer book graphic

// AFTER:
public WarlockSpellbook(ulong content) : base(content, 0xEFA) // Same as Druid/etc
```

Hue 0x81D (Witch) and 0x455 (Warlock) will still distinguish them from other books using 0xEFA.

Then rebuild ServUO and test again.

---

## Expected Behavior After Fix

All 12 Vystia spellbooks should:
- ✅ Open when double-clicked
- ✅ Show 32 spell slots in index
- ✅ Show spell names (not "Empty" or blank)
- ✅ Show spell icons
- ✅ Show power words and reagents
- ✅ Allow spell casting

---

## Current Client Code Status

### AssignGraphic() - Lines 1646-1701

**Handles:**
- 8 books using graphic 0xEFA (differentiated by 8 different hues)
- 2 books using graphic 0xFF0 (differentiated by 2 different hues) ← POTENTIAL ISSUE
- 1 book using graphic 0x2253 with hue 0x455 (Vystia Necromancer)
- 1 book using graphic 0x2252 with hue 0x481 (Ice Magic) ← WORKS

### GetBookInfo() - Lines 1090-1184

**Status:** ✅ All 12 Vystia cases present, all set:
- maxSpellsCount = SpellsVystia<School>.MaxSpellCount (should be 32)
- bookGraphic = 0x08AC
- minimizedGraphic = 0x08BA
- iconStartGraphic = 0x08C0

### GetSpellIdOffset() - Lines 1597-1640

**Status:** ✅ All 12 cases present, return correct base IDs (1000, 1032, 1064, etc.)

---

*Debug guide created: 2025-12-08*
*Client build: Release/net9.0 @ 09:37*
