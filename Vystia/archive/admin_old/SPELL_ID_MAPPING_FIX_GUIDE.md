# Spell ID Mapping Fix Guide

**Last Updated:** 2025-01-13  
**Purpose:** Reference guide for fixing spell ID mapping issues between client and server for Vystia custom spells (IDs 1000-1383)

---

## Overview

Vystia custom spells use server spell IDs 1000-1383, which are different from standard UO spells (0-799). Standard UO spells use 1-based indexing from the client (client sends 1-64, server converts to 0-63), but Vystia spells send the actual server spell ID directly (1000, 1001, 1002, etc.).

This guide documents the fixes required to ensure proper spell ID mapping between client and server.

---

## Problem Symptoms

If spell ID mappings are incorrect, you may see:
- Spells appearing in wrong circles in the spellbook
- Clicking a spell icon casts a different spell
- "This spell has been temporarily disabled" errors
- Server logs showing invalid spell IDs (e.g., 999 instead of 1000)
- `GetTypeForSpell` returning `Invalid` for valid spell IDs

---

## Files That Need to Be Updated

### 1. Server: Packet Handlers (`ServUO/Server/Network/PacketHandlers.cs`)

**Two packet handlers need fixes:**

#### Packet 0x1C (Direct Cast Spell)
**Location:** `CastSpell` method (around line 1913)

**Original Code:**
```csharp
int spellID = pvSrc.ReadInt16() - 1;
```

**Fixed Code:**
```csharp
int rawSpellID = pvSrc.ReadInt16();
// For standard UO spells (0-799), client sends 1-based IDs, so subtract 1
// For Vystia custom spells (1000+), client sends actual server spell ID, so don't subtract
int spellID = (rawSpellID >= 1000) ? rawSpellID : (rawSpellID - 1);
```

#### Packet 0x27 (Cast Spell From Book)
**Location:** `TextCommand` method, case 0x27 (around line 888)

**Original Code:**
```csharp
int spellID = Utility.ToInt32(split[0]) - 1;
```

**Fixed Code:**
```csharp
int rawSpellID = Utility.ToInt32(split[0]);
// For standard UO spells (0-799), client sends 1-based IDs, so subtract 1
// For Vystia custom spells (1000+), client sends actual server spell ID, so don't subtract
int spellID = (rawSpellID >= 1000) ? rawSpellID : (rawSpellID - 1);
```

---

### 2. Server: Spellbook Display (`ServUO/Scripts/Items/Equipment/Spellbooks/Spellbook.cs`)

**Location:** `DisplayTo` method (around line 878)

**Issue:** Server sends `BookOffset + 1` to client, but for Vystia spellbooks, the client expects `BookOffset` directly.

**Original Code:**
```csharp
to.Send(new NewSpellbookContent(this, ItemID, BookOffset + 1, m_Content));
```

**Fixed Code:**
```csharp
// For Vystia spellbooks (1000+), send BookOffset directly (not +1) because client creates items as offset + index
// For standard spellbooks (0-799), send BookOffset + 1 to maintain compatibility
int offsetToSend = (BookOffset >= 1000) ? BookOffset : (BookOffset + 1);

to.Send(new NewSpellbookContent(this, ItemID, offsetToSend, m_Content));
```

**Apply to all spellbook content packets:**
- `NewSpellbookContent`
- `SpellbookContent6017`
- `SpellbookContent`

---

### 3. Server: Spellbook Type Detection (`ServUO/Scripts/Items/Equipment/Spellbooks/Spellbook.cs`)

**Location:** `GetTypeForSpell` method (around line 422)

**Ensure spell ID ranges are correct for all 12 Vystia schools:**

```csharp
// Vystia Custom Spellbooks (12 schools, 384 spells total, IDs 1000-1383)
else if (spellID >= 1000 && spellID <= 1031)
{
    return SpellbookType.VystiaIceMage;  // Ice Magic (1000-1031)
}
else if (spellID >= 1032 && spellID <= 1063)
{
    return SpellbookType.VystiaDruid;  // Nature Magic (1032-1063)
}
// ... (continue for all 12 schools)
```

---

### 4. Server: Spellbook BookOffset (`ServUO/Scripts/Items/Equipment/Spellbooks/VystiaSpellbooks.cs`)

**Location:** Each spellbook class's `BookOffset` property

**Ensure `BookOffset` matches the starting spell ID in `VystiaSpellInitializer.cs`:**

```csharp
public override int BookOffset => 1000; // Spell IDs 1000-1031 (Ice Magic)
public override int BookOffset => 1032; // Spell IDs 1032-1063 (Nature Magic)
// ... (continue for all 12 schools)
```

---

### 5. Client: Spell Routing (`ClassicUO/src/ClassicUO.Client/Game/Data/SpellDefinition.cs`)

**Location:** `FullIndexGetSpell` method (around line 237)

**Ensure routing correctly maps server IDs to client dictionary keys:**

```csharp
if (fullidx < 1032) return SpellsVystiaIceMagic.GetSpell(fullidx - 999);       // 1000-1031 → 1-32
if (fullidx < 1064) return SpellsVystiaNature.GetSpell(fullidx - 1031);       // 1032-1063 → 1-32
// ... (continue for all 12 schools)
```

**Note:** Client dictionary keys are 1-based, so subtract (startingID - 1) to convert server ID to dictionary key.

---

### 6. Client: Spellbook Icon Serial (`ClassicUO/src/ClassicUO.Client/Game/UI/Gumps/SpellbookGump.cs`)

**Location:** Icon creation loop (around line 602)

**Icon serial calculation should be:**
```csharp
int spellOffset = GetSpellIdOffset(_spellBookType); // Returns 1000 for Ice Magic
uint iconSerial = (uint)(spellOffset + i); // i is normalized index (0, 1, 2, 3...)
```

**Lookup logic (around line 898):**
```csharp
if (serial >= 1000)
{
    int baseOffset = GetSpellIdOffset(_spellBookType);
    // serial is the server spell ID (e.g., 1000, 1001, 1002...)
    // Dictionary keys are 1-based (1, 2, 3...)
    // So: idx = (serverID - baseOffset) + 1
    idx = (int)(serial - baseOffset) + 1;
}
```

---

## Verification Checklist

After making changes, verify:

- [ ] Server builds without errors
- [ ] Client builds without errors
- [ ] Spellbook displays correct spells in correct circles
- [ ] Clicking spell icons casts the correct spell
- [ ] Server logs show correct spell IDs (1000, 1001, etc., not 999, 1000)
- [ ] `GetTypeForSpell` returns correct spellbook type for all Vystia spells
- [ ] Spell index selection (using spell list) works correctly
- [ ] Spell icon selection (clicking icons) works correctly

---

## Debugging Tips

### Server-Side Debugging

Add console logging to track spell ID flow:

```csharp
// In PacketHandlers.cs
Console.WriteLine($"[CASTSPELL] Raw SpellID: {rawSpellID}, Final SpellID: {spellID}");

// In Spellbook.cs
Console.WriteLine($"[SERVER] HasSpell({originalSpellID}) - BookOffset: {BookOffset}, NormalizedID: {spellID}");
```

### Client-Side Debugging

Add console logging to track icon serial and spell lookup:

```csharp
// In SpellbookGump.cs
Console.WriteLine($"[SPELLBOOK] Creating icon - i: {i}, spellOffset: {spellOffset}, iconSerial: {iconSerial}");
Console.WriteLine($"[SPELLBOOK] OnIconDoubleClick - IconSerial: {iconSerial}");
Console.WriteLine($"[SPELLBOOK] GetSpellDefinition(serial: {serial}) -> idx: {idx}");
Console.WriteLine($"[SPELLBOOK] Spell found - ID: {def.ID}, Name: {def.Name}");
```

---

## Common Issues and Solutions

### Issue: Client sends 999 instead of 1000

**Cause:** Server packet handler is subtracting 1 from all spell IDs.

**Solution:** Update packet handlers (0x1C and 0x27) to not subtract 1 for Vystia spells (1000+).

---

### Issue: Spellbook shows wrong spells or missing spells

**Cause:** Server is sending wrong `BookOffset` value to client.

**Solution:** Update `Spellbook.cs` `DisplayTo` method to send `BookOffset` (not `BookOffset + 1`) for Vystia spellbooks.

---

### Issue: Clicking spell icon casts wrong spell

**Cause:** Icon serial calculation or lookup is incorrect.

**Solution:** Verify icon serial = `spellOffset + i` and lookup uses `(serial - baseOffset) + 1` to get dictionary key.

---

### Issue: "This spell has been temporarily disabled"

**Cause:** Server receives invalid spell ID (e.g., 999) and can't find spell in registry.

**Solution:** Check packet handlers are not subtracting 1 from Vystia spell IDs.

---

### Issue: `GetTypeForSpell` returns `Invalid`

**Cause:** Spell ID ranges in `GetTypeForSpell` don't match actual spell ID ranges.

**Solution:** Update spell ID ranges to match `VystiaSpellInitializer.cs` registrations.

---

## Testing Commands

Use these GM commands to test spell functionality:

```
[AllSpells          - Fill spellbook with all spells
[PracticeTarget     - Spawn infinite HP test dummy
[GMMode             - Set stats/skills for testing
```

---

## Related Files

- `ServUO/Scripts/Custom/VystiaClasses/Spells/VystiaSpellInitializer.cs` - Server spell registrations
- `ServUO/Scripts/Items/Equipment/Spellbooks/VystiaSpellbooks.cs` - Spellbook BookOffset definitions
- `ClassicUO/src/ClassicUO.Client/Game/Data/SpellsVystia*.cs` - Client spell definitions (12 files)
- `ClassicUO/src/ClassicUO.Client/Game/Data/SpellDefinition.cs` - Client spell routing
- `Vystia/admin/analyses/MASTER_ALIGNMENT_ANALYSIS.md` - Spell ID range reference

---

## Summary

**Key Principle:** Vystia spells (1000+) send actual server spell IDs, while standard UO spells (0-799) send 1-based IDs that need to be converted.

**Always check:**
1. Packet handlers don't subtract 1 from Vystia spell IDs
2. Spellbook sends correct offset to client
3. Client icon serials match server spell IDs
4. Spell ID ranges are correct in all lookup methods

---

**Last Fixed:** 2025-01-13 - Fixed packet 0x27 handler and spellbook offset for Vystia spellbooks

