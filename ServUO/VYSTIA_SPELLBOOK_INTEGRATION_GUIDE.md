# Vystia Spellbook Integration Guide

## Overview
This document provides a complete reference for integrating custom Vystia spellbooks with both the ServUO server and ClassicUO client. Based on the successful Ice Magic spellbook implementation (2025-12-06).

**Status:** Ice Magic fully functional (1/12 schools complete)
**Remaining:** 11 magic schools to integrate (352 spells)

---

## Architecture Overview

### Spell ID Allocation
Vystia custom spells use IDs **999-1383** (384 total spells):

| School | Spell IDs | Status |
|--------|-----------|--------|
| Ice Magic | 999-1030 | ✅ Complete |
| Druid (Nature) | 1031-1062 | ⏳ Pending |
| Witch (Hex) | 1063-1094 | ⏳ Pending |
| Sorcerer (Elemental) | 1095-1126 | ⏳ Pending |
| Warlock (Dark) | 1127-1158 | ⏳ Pending |
| Oracle (Divination) | 1159-1190 | ⏳ Pending |
| Necromancer | 1191-1222 | ⏳ Pending |
| Summoner | 1223-1254 | ⏳ Pending |
| Shaman | 1255-1286 | ⏳ Pending |
| Bard | 1287-1318 | ⏳ Pending |
| Enchanter | 1319-1350 | ⏳ Pending |
| Illusionist | 1351-1382 | ⏳ Pending |

**Total:** 12 schools × 32 spells = 384 spells

---

## Critical Server-Side Requirements

### 1. SpellRegistry Array Size

**File:** `ServUO/Scripts/Spells/Base/SpellRegistry.cs`

**CRITICAL:** The spell registry array MUST be large enough to accommodate the highest spell ID.

```csharp
public class SpellRegistry
{
    // BEFORE (BROKEN):
    // private static readonly Type[] m_Types = new Type[745];

    // AFTER (FIXED):
    private static readonly Type[] m_Types = new Type[1500];  // Accommodates IDs up to 1499
}
```

**Why This Matters:**
- Standard UO spells use IDs 0-744
- Vystia spells start at ID 999
- If array is too small, `SpellRegistry.NewSpell()` returns NULL → "You do not have that spell" error

**Rule:** Array size must be **greater than** the highest spell ID you'll use.

---

### 2. Spellbook Type Detection

**File:** `ServUO/Scripts/Items/Equipment/Spellbooks/Spellbook.cs`

**Method:** `GetTypeForSpell(int spellID)`

This method maps spell IDs to SpellbookType enum values. **MUST** include ranges for all custom spellbooks.

```csharp
public static SpellbookType GetTypeForSpell(int spellID)
{
    // ... standard UO spellbooks (0-745)

    // Vystia Custom Spellbooks
    if (spellID >= 999 && spellID < 1031)
    {
        return SpellbookType.VystiaIceMage;  // Ice Magic
    }
    else if (spellID >= 1031 && spellID < 1063)
    {
        return SpellbookType.VystiaDruid;  // Nature Magic
    }
    else if (spellID >= 1063 && spellID < 1095)
    {
        return SpellbookType.VystiaWitch;  // Hex Magic
    }
    // ... continue for all 12 schools

    return SpellbookType.Invalid;
}
```

**Why This Matters:**
- When client doesn't send spellbook serial (clicking index page, dragging spell), server calls `Find(Mobile, spellID)`
- `Find()` calls `GetTypeForSpell()` to determine which spellbook type to search for
- If type is Invalid, `Find()` returns NULL → spell fails with "You do not have that spell"

**Impact:**
- ✅ WITH range: All 3 casting methods work (icon, index, drag)
- ❌ WITHOUT range: Only double-clicking icon works (when client sends book serial)

---

### 3. Spell Registration

**File:** `ServUO/Scripts/Custom/VystiaClasses/Spells/VystiaSpellInitializer.cs`

**Key Points:**
1. Register spells at IDs **999-1030** (server-side IDs)
2. Client sends IDs **1000-1031** (client-side IDs)
3. UO protocol subtracts 1: client 1000 → server 999

```csharp
public class VystiaSpellInitializer
{
    private static bool _initialized = false;  // Prevent double-registration

    public static void Initialize()
    {
        if (_initialized)
        {
            Console.WriteLine("[VYSTIA] WARNING: Duplicate initialization prevented!");
            return;
        }
        _initialized = true;

        // Ice Magic Spells (Registry IDs 999-1030, Client sends 1000-1031)
        // Server subtracts 1 from client spell ID, so register at -1 offset
        Register(999, typeof(FrostTouchSpell));   // Client sends 1000
        Register(1000, typeof(IceShardSpell));    // Client sends 1001
        // ... all 32 spells
        Register(1030, typeof(FrostArmorSpell));  // Client sends 1031
    }

    private static void Register(int spellID, Type type)
    {
        SpellRegistry.Register(spellID, type);
    }
}
```

**Duplicate Registration Guard:**
- `Initializer.Initialize()` may be called multiple times during server startup
- Without guard, spells register twice → potential registry corruption
- Guard ensures registration happens exactly once

---

### 4. Spellbook Item Implementation

**File:** `ServUO/Scripts/Items/Equipment/Spellbooks/VystiaSpellbooks.cs`

Each spellbook must implement:

```csharp
public class IceMageSpellbook : Spellbook
{
    [Constructable]
    public IceMageSpellbook() : this(0xFFFFFFFF)  // Fill with all 32 spells
    {
    }

    public IceMageSpellbook(ulong content) : base(content, 0xEFA)
    {
        Name = "Tome of Frozen Arts";
        Hue = 1150;  // Ice blue
        // ... bonuses
    }

    public override SpellbookType SpellbookType => SpellbookType.VystiaIceMage;
    public override int BookOffset => 999;  // MUST match first spell ID in registry
    public override int BookCount => 32;

    // Serialization required
    public IceMageSpellbook(Serial serial) : base(serial) { }
    public override void Serialize(GenericWriter writer) { base.Serialize(writer); }
    public override void Deserialize(GenericReader reader) { base.Deserialize(reader); }
}
```

**Critical Properties:**
- `BookOffset`: First spell ID (999 for Ice Magic)
- `BookCount`: Number of spells (32 for all Vystia schools)
- `SpellbookType`: Unique enum value for this spellbook
- `Content`: 64-bit bitmask (0xFFFFFFFF = all 32 spells filled)

**HasSpell Calculation:**
```csharp
public bool HasSpell(int spellID)
{
    int normalizedID = spellID - BookOffset;  // 1005 - 999 = 6
    return (normalizedID >= 0 && normalizedID < BookCount && (Content & (1UL << normalizedID)) != 0);
}
```

---

## Client-Side Requirements

### 1. Spell Definitions File

**File:** `ClassicUO/src/ClassicUO.Client/Game/Data/SpellsVystiaIceMagic.cs`

Each magic school needs its own spell definitions file:

```csharp
namespace ClassicUO.Client.Game.Data
{
    internal static class SpellsVystiaIceMagic
    {
        public static int MaxSpellCount => 32;

        private static Dictionary<int, SpellDefinition> _spellsDict;

        public static Dictionary<int, SpellDefinition> GetAllSpells => _spellsDict;

        static SpellsVystiaIceMagic()
        {
            _spellsDict = new Dictionary<int, SpellDefinition>
            {
                // IMPORTANT: Keys are 1-32, NOT absolute spell IDs!
                {
                    1,  // Index 1
                    new SpellDefinition
                    (
                        "Frost Touch",        // Name
                        1000,                 // Actual spell ID (client-side)
                        0x1B5B,              // Icon graphic ID
                        "Frio Tactus",       // Power words
                        TargetType.Harmful,  // Target type
                        Reagents.None        // Client-side reagents (leave None for custom)
                    )
                },
                {
                    2,  // Index 2
                    new SpellDefinition("Ice Shard", 1001, 0x1B5C, "Glacies Sagitta", TargetType.Harmful, Reagents.None)
                },
                // ... all 32 spells (keys 1-32, IDs 1000-1031)
                {
                    32,  // Index 32
                    new SpellDefinition("Frost Armor", 1031, 0x1B77, "Glacies Armatura", TargetType.Beneficial, Reagents.None)
                }
            };
        }

        public static SpellDefinition GetSpell(int spellIndex)
        {
            if (_spellsDict.TryGetValue(spellIndex, out SpellDefinition spell))
            {
                return spell;
            }
            return SpellDefinition.EmptySpell;
        }
    }
}
```

**Key Points:**
- Dictionary keys are **1-32** (normalized indices)
- Spell IDs are **1000-1031** (absolute client IDs)
- Server receives **999-1030** (after subtracting 1)

---

### 2. SpellbookType Enum

**File:** `ClassicUO/src/ClassicUO.Client/Game/Data/SpellbookTypes.cs`

Add enum value for each custom spellbook:

```csharp
internal enum SpellBookType
{
    Regular = 1,
    Necromancy = 2,
    Chivalry = 3,
    Bushido = 4,
    Ninjitsu = 5,
    Spellweaving = 6,
    Mysticism = 7,
    Mastery = 8,
    Bardic = 9,

    // Vystia Custom Spellbooks
    VystiaIceMagic = 10,
    VystiaDruid = 11,
    VystiaWitch = 12,
    VystiaSorcerer = 13,
    VystiaWarlock = 14,
    VystiaOracle = 15,
    VystiaNecromancer = 16,
    VystiaSummoner = 17,
    VystiaShaman = 18,
    VystiaBard = 19,
    VystiaEnchanter = 20,
    VystiaIllusionist = 21
}
```

---

### 3. SpellDefinition Routing

**File:** `ClassicUO/src/ClassicUO.Client/Game/Data/SpellDefinition.cs`

Route spell IDs to the correct spell definition class:

```csharp
public static SpellDefinition FullIndexGetSpell(int fullidx)
{
    // Standard UO spells (0-744)
    // ...

    // Vystia Custom Spells (999-1382)
    if (fullidx >= 999 && fullidx < 1383)
    {
        if (fullidx < 1031) return SpellsVystiaIceMagic.GetSpell(fullidx);
        else if (fullidx < 1063) return SpellsVystiaDruid.GetSpell(fullidx);
        else if (fullidx < 1095) return SpellsVystiaWitch.GetSpell(fullidx);
        // ... continue for all 12 schools
    }

    return EmptySpell;
}
```

**Why This Matters:**
- Client needs to look up spell name, icon, power words for display
- Without routing, client shows empty spell names in spellbook

---

### 4. Spellbook Gump Integration

**File:** `ClassicUO/src/ClassicUO.Client/Game/UI/Gumps/SpellbookGump.cs`

Add case for each spellbook type in multiple methods:

**GetSpellDefinition (line ~935):**
```csharp
private SpellDefinition GetSpellDefinition(int idx)
{
    switch (_spellBookType)
    {
        case SpellBookType.Magery:
            return SpellsMagery.GetSpell(idx);
        case SpellBookType.VystiaIceMagic:
            return SpellsVystiaIceMagic.GetSpell(idx);
        // ... all other spellbook types
    }
    return SpellDefinition.EmptySpell;
}
```

**GetSpellNames (line ~1170):**
```csharp
switch (_spellBookType)
{
    case SpellBookType.VystiaIceMagic:
        def = SpellsVystiaIceMagic.GetSpell(offset + 1);
        name = def.Name;
        abbreviature = def.PowerWords;
        reagents = def.CreateReagentListString("\n");
        break;
}
```

**GetBookInfo (line ~945):**
```csharp
switch (type)
{
    case SpellBookType.VystiaIceMagic:
        maxSpellsCount = SpellsVystiaIceMagic.MaxSpellCount;  // 32
        bookGraphic = 0xEFA;       // Spellbook item graphic
        minimizedGraphic = 0x2253; // Minimized icon
        iconStartGraphic = 0x1B5B; // First spell icon
        break;
}
```

---

## Packet Flow

### Successful Spell Cast (All 3 Methods)

**1. User Action:**
- Double-click spell icon
- Click spell name on index page
- Drag spell to create button

**2. Client → Server (Packet 0x12, Subcommand 0x27):**
```
[TEXTCOMMAND] Packet 0x12 received - Type: 0x27, Command: '1005 1073868573'
[PACKET 0x27] Raw command string: '1005 1073868573'
[PACKET 0x27] Split parts: 2
[PACKET 0x27] Parsed - SpellID: 1004, Serial: 1073868573 (0x4001EF1D)
[PACKET 0x27] World.FindItem(1073868573) returned: IceMageSpellbook
```

**3. Server Validation:**
```
[SERVER] CastSpellRequest - Player: Runescry, SpellID: 1004, InitialBook: IceMageSpellbook
[SERVER] HasSpell(1004) - BookOffset: 999, NormalizedID: 5, InRange[0-32]: True, HasBit: True
[SERVER] Book HasSpell(1004): True
```

**4. Spell Creation:**
```
[REGISTRY] NewSpell(1004) - SUCCESS: Created IceShieldSpell
[SERVER] Spell created successfully: IceShieldSpell
```

**5. Spell Cast:**
- Spell.Cast() executes
- Mana consumed
- Effects applied
- Visual/sound effects play

---

## Common Issues & Solutions

### Issue 1: "You do not have that spell"

**Symptoms:**
- Message appears when clicking spell
- Spell doesn't cast

**Root Causes:**
1. **SpellRegistry array too small** → Spell ID out of bounds
2. **GetTypeForSpell missing range** → Can't find spellbook
3. **Spell not registered** → Registry lookup fails
4. **BookOffset mismatch** → HasSpell fails

**Diagnostic Logs:**
```
[REGISTRY] NewSpell(1005) - OUT OF RANGE (max: 744)  ← Array too small
[FIND] GetTypeForSpell returned: Invalid  ← Missing range
[REGISTRY] NewSpell(1005) - Type is NULL (not registered)  ← Not registered
[SERVER] HasSpell(1005): False  ← BookOffset mismatch
```

**Solutions:**
1. Increase SpellRegistry array size to 1500+
2. Add spell ID range to GetTypeForSpell()
3. Verify VystiaSpellInitializer.Initialize() is called
4. Check BookOffset matches first spell ID (999 for Ice Magic)

---

### Issue 2: Index/Drag Doesn't Work

**Symptoms:**
- Double-clicking spell icon works
- Clicking spell name fails
- Dragging spell fails

**Root Cause:**
- GetTypeForSpell() doesn't include spell ID range
- Server can't identify spellbook type when client doesn't send book serial

**Solution:**
Add spell ID range to GetTypeForSpell() in Spellbook.cs

---

### Issue 3: Spells Register Twice

**Symptoms:**
```
[VYSTIA] Registered spell ID 999: FrostTouchSpell
[VYSTIA] Registered spell ID 1000: IceShardSpell
...
[VYSTIA] Registered spell ID 999: FrostTouchSpell  ← Duplicate!
[VYSTIA] Registered spell ID 1000: IceShardSpell   ← Duplicate!
```

**Root Cause:**
- Initializer.Initialize() called multiple times during server startup

**Solution:**
Add duplicate registration guard:
```csharp
private static bool _initialized = false;
if (_initialized) return;
_initialized = true;
```

---

## Testing Checklist

### Server-Side Testing
- [ ] Server builds with 0 errors
- [ ] Spell registration logs show all 32 spells registered ONCE
- [ ] `[spellbook icemagic]` command creates spellbook
- [ ] Spellbook appears in backpack with all 32 spells
- [ ] Double-clicking spellbook opens client UI

### Client-Side Testing
- [ ] Client builds with 0 errors
- [ ] Spellbook opens and shows all 32 spell names
- [ ] Spell icons display correctly
- [ ] Spell power words show correctly
- [ ] All 3 casting methods work:
  - [ ] Double-click spell icon
  - [ ] Click spell name on index page
  - [ ] Drag spell icon to create button

### Casting Testing
- [ ] Spell consumes mana
- [ ] Reagents consumed (if required)
- [ ] Spell effects apply (damage, buff, debuff)
- [ ] Visual effects play
- [ ] Sound effects play
- [ ] Spell message displays
- [ ] Spell reflection works (if applicable)

---

## Quick Reference: Spell ID Math

**Example: Ice Shield (3rd spell in Circle 2)**

| Perspective | Spell ID | Calculation |
|-------------|----------|-------------|
| Client Dictionary Key | 6 | Circle 2 × 4 + 2 = 6 |
| Client Spell ID | 1005 | 999 + 6 = 1005 |
| Server Registry ID | 1004 | 1005 - 1 = 1004 |
| Server Normalized ID | 5 | 1004 - 999 = 5 |
| Content Bitmask Bit | 5 | 1 << 5 = 0x20 |

**Formula:**
```
Client Dictionary Key = (Circle - 1) × 4 + (Spell - 1) + 1
Client Spell ID = BookOffset + Dictionary Key
Server Registry ID = Client Spell ID - 1
Server Normalized ID = Server Registry ID - BookOffset
```

---

## Files Modified Per Spellbook

### Server Files (ServUO)
1. `Scripts/Spells/Base/SpellRegistry.cs` - Array size (once for all schools)
2. `Scripts/Items/Equipment/Spellbooks/Spellbook.cs` - GetTypeForSpell range
3. `Scripts/Items/Equipment/Spellbooks/VystiaSpellbooks.cs` - Spellbook class
4. `Scripts/Custom/VystiaClasses/Spells/VystiaSpellInitializer.cs` - Registration
5. `Scripts/Custom/VystiaClasses/Spells/<School>/` - 32 spell files

### Client Files (ClassicUO)
1. `Game/Data/SpellbookTypes.cs` - Enum value
2. `Game/Data/SpellsVystia<School>.cs` - Spell definitions (NEW FILE)
3. `Game/Data/SpellDefinition.cs` - Spell ID routing
4. `Game/UI/Gumps/SpellbookGump.cs` - UI integration (3 methods)

---

## Implementation Order (Recommended)

For each new Vystia magic school:

1. **Server Spell Implementation** (highest effort)
   - Create 32 spell files in `Spells/<School>/`
   - Register spells in VystiaSpellInitializer
   - Test with `[<spellname>` commands

2. **Server Spellbook** (medium effort)
   - Add spellbook class to VystiaSpellbooks.cs
   - Add spell ID range to GetTypeForSpell()
   - Test with `[spellbook <type>]` command

3. **Client Spell Definitions** (medium effort)
   - Create SpellsVystia<School>.cs file
   - Add 32 spell definitions
   - Update SpellDefinition routing

4. **Client UI Integration** (low effort)
   - Add enum to SpellbookTypes
   - Add cases to SpellbookGump methods
   - Test all 3 casting methods

5. **End-to-End Testing** (critical)
   - Test all 32 spells cast successfully
   - Verify mana, reagents, effects
   - Test all 3 UI interaction methods

---

## Next Schools To Implement

**Priority Order:**
1. **Nature Magic** (Druid) - IDs 1031-1062
2. **Hex Magic** (Witch) - IDs 1063-1094
3. **Necromancy** - IDs 1191-1222
4. **Summoning** - IDs 1223-1254
5. **Shamanic** - IDs 1255-1286
6. **Elemental** (Sorcerer) - IDs 1095-1126
7. **Dark Magic** (Warlock) - IDs 1127-1158
8. **Divination** (Oracle) - IDs 1159-1190
9. **Bardic** - IDs 1287-1318
10. **Enchanting** - IDs 1319-1350
11. **Illusion** - IDs 1351-1382

---

*Document created: 2025-12-06*
*Based on: Ice Magic spellbook implementation and debugging*
*Status: Complete reference guide for all 12 Vystia magic schools*
