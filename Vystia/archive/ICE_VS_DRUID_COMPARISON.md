# Ice Magic (Working) vs Druid (Not Working) - Complete Comparison

## Reference: Ice Magic is the Source of Truth

This document compares EVERY configuration point between Ice Magic (which works) and Druid (which doesn't).

---

## SERVER-SIDE COMPARISON

### File: ServUO/Scripts/Items/Equipment/Spellbooks/VystiaSpellbooks.cs

#### Ice Magic Spellbook
```csharp
public class IceMageSpellbook : Spellbook
{
    [Constructable]
    public IceMageSpellbook() : this(0xFFFFFFFF) // Fill with all 32 spells
    {
    }

    [Constructable]
    public IceMageSpellbook(ulong content) : base(content, 0x2252) // Chivalry book graphic
    {
        Name = "Tome of Frozen Arts";
        Hue = 0x481; // Ice Blue
        Weight = 3.0;
        Layer = Layer.OneHanded;
    }

    public IceMageSpellbook(Serial serial) : base(serial) { }

    public override SpellbookType SpellbookType => SpellbookType.VystiaIceMage;
    public override int BookOffset => 999; // Spell IDs 999-1030
    public override int BookCount => 32;

    public override void Serialize(GenericWriter writer)
    {
        base.Serialize(writer);
        writer.Write((int)0);
    }

    public override void Deserialize(GenericReader reader)
    {
        base.Deserialize(reader);
        int version = reader.ReadInt();
    }
}
```

#### Druid Spellbook
```csharp
public class DruidSpellbook : Spellbook
{
    [Constructable]
    public DruidSpellbook() : this(0xFFFFFFFF) // Fill with all 32 spells
    {
    }

    [Constructable]
    public DruidSpellbook(ulong content) : base(content, 0xEFA) // ← DIFFERENT GRAPHIC
    {
        Name = "Codex of the Wild";
        Hue = 0x7D6; // Forest Green ← DIFFERENT HUE
        Weight = 3.0;
        Layer = Layer.OneHanded;
    }

    public DruidSpellbook(Serial serial) : base(serial) { }

    public override SpellbookType SpellbookType => SpellbookType.VystiaDruid; // ← DIFFERENT ENUM
    public override int BookOffset => 1031; // Spell IDs 1031-1062 ← DIFFERENT OFFSET
    public override int BookCount => 32; // ← SAME

    public override void Serialize(GenericWriter writer)
    {
        base.Serialize(writer);
        writer.Write((int)0);
    }

    public override void Deserialize(GenericReader reader)
    {
        base.Deserialize(reader);
        int version = reader.ReadInt();
    }
}
```

**Differences:**
| Property | Ice Magic | Druid |
|----------|-----------|-------|
| Graphic | 0x2252 | 0xEFA |
| Hue | 0x481 | 0x7D6 |
| SpellbookType | VystiaIceMage | VystiaDruid |
| BookOffset | 999 | 1031 |

**Structure:** ✅ IDENTICAL (except expected value differences)

---

## CLIENT-SIDE COMPARISON

### File: ClassicUO/Game/Data/SpellbookTypes.cs

#### Enum Definitions
```csharp
VystiaIceMagic = 10,     // ← Ice Magic
VystiaDruid,             // ← Druid (value = 11)
```

**Status:** ✅ BOTH DEFINED

---

### File: ClassicUO/Game/Data/SpellsVystiaIceMagic.cs vs SpellsVystiaNature.cs

#### Ice Magic Spell Definitions
```csharp
internal static class SpellsVystiaIceMagic
{
    public static int MaxSpellCount => 32;

    private static Dictionary<int, SpellDefinition> _spellsDict;

    public static Dictionary<int, SpellDefinition> GetAllSpells => _spellsDict;

    static SpellsVystiaIceMagic()
    {
        _spellsDict = new Dictionary<int, SpellDefinition>
        {
            {1, new SpellDefinition("Frost Touch", 1000, 0x1B5B, "Frio Tactus", TargetType.Harmful, Reagents.None)},
            {2, new SpellDefinition("Ice Shard", 1001, 0x1B5C, "Glacies Sagitta", TargetType.Harmful, Reagents.None)},
            // ... 32 total spells (IDs 1000-1031)
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
```

#### Druid Spell Definitions
```csharp
internal static class SpellsVystiaNature // ← Note: "Nature" not "Druid"
{
    public static int MaxSpellCount => 32;

    private static Dictionary<int, SpellDefinition> _spellsDict;

    public static Dictionary<int, SpellDefinition> GetAllSpells => _spellsDict;

    static SpellsVystiaNature()
    {
        _spellsDict = new Dictionary<int, SpellDefinition>
        {
            {1, new SpellDefinition("Thorn Strike", 1032, 0x1B5B, "Spina Ictus", TargetType.Harmful, Reagents.None)},
            {2, new SpellDefinition("Wild Growth", 1033, 0x1B5C, "Crescere Silvae", TargetType.Beneficial, Reagents.None)},
            // ... 32 total spells (IDs 1032-1063) ← DIFFERENT RANGE
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
```

**Differences:**
| Property | Ice Magic | Druid |
|----------|-----------|-------|
| Class Name | SpellsVystiaIceMagic | SpellsVystiaNature |
| Spell IDs | 1000-1031 | 1032-1063 |

**Structure:** ✅ IDENTICAL

---

### File: ClassicUO/Game/UI/Gumps/SpellbookGump.cs

#### Method 1: AssignGraphic() - Line ~1646

**Ice Magic:**
```csharp
case 0x2252:
    // Check hue to distinguish Vystia Ice Magic (0x481) from Chivalry
    if (item.Hue == 0x481)
        _spellBookType = SpellBookType.VystiaIceMagic;
    else
        _spellBookType = SpellBookType.Chivalry;
    break;
```

**Druid:**
```csharp
case 0x0EFA: // ← Note: 0x0EFA (4 digits) vs Ice's 0x2252
    // Check hue to distinguish Vystia spellbooks from Magery
    if (item.Hue == 0x7D6) // Forest Green
        _spellBookType = SpellBookType.VystiaDruid;
    else if (item.Hue == 0x54E) // Fiery Orange
        _spellBookType = SpellBookType.VystiaSorcerer;
    // ... more hue checks for other books using 0xEFA
    else
        _spellBookType = SpellBookType.Magery;
    break;
```

**Differences:**
- Ice Magic has its OWN graphic (0x2252) shared only with Chivalry
- Druid SHARES graphic (0xEFA) with 8 other Vystia books + Magery
- Both use hue to distinguish ← SAME PATTERN

**Structure:** ✅ SAME PATTERN, different graphics

---

#### Method 2: GetBookInfo() - Line ~1090

**Ice Magic:**
```csharp
case SpellBookType.VystiaIceMagic:
    maxSpellsCount = SpellsVystiaIceMagic.MaxSpellCount;
    bookGraphic = 0x08AC; // Reuse Magery book graphic for display
    minimizedGraphic = 0x08BA;
    iconStartGraphic = 0x08C0; // Reuse Magery icons for now
    break;
```

**Druid:**
```csharp
case SpellBookType.VystiaDruid:
    maxSpellsCount = SpellsVystiaNature.MaxSpellCount;
    bookGraphic = 0x08AC;
    minimizedGraphic = 0x08BA;
    iconStartGraphic = 0x08C0;
    break;
```

**Differences:** NONE - ✅ IDENTICAL (except class name)

---

#### Method 3: GetSpellIdOffset() - Line ~1597

**Ice Magic:**
```csharp
case SpellBookType.VystiaIceMagic:
    return 1000; // Spells 1000-1031
```

**Druid:**
```csharp
case SpellBookType.VystiaDruid:
    return 1032; // Spells 1032-1063
```

**Differences:**
| Property | Ice Magic | Druid |
|----------|-----------|-------|
| Base ID | 1000 | 1032 |

**Structure:** ✅ IDENTICAL

---

#### Method 4: GetSpellDefinition() - Line ~948

**Ice Magic:**
```csharp
case SpellBookType.VystiaIceMagic:
    def = SpellsVystiaIceMagic.GetSpell(idx);
    break;
```

**Druid:**
```csharp
case SpellBookType.VystiaDruid:
    def = SpellsVystiaNature.GetSpell(idx);
    break;
```

**Structure:** ✅ IDENTICAL (except class name)

---

#### Method 5: GetSpellNames() - Line ~1326

**Ice Magic:**
```csharp
case SpellBookType.VystiaIceMagic:
    def = SpellsVystiaIceMagic.GetSpell(offset + 1);
    name = def.Name;
    abbreviature = def.PowerWords;
    reagents = def.CreateReagentListString("\n");
    break;
```

**Druid:**
```csharp
case SpellBookType.VystiaDruid:
    def = SpellsVystiaNature.GetSpell(offset + 1);
    name = def.Name;
    abbreviature = def.PowerWords;
    reagents = def.CreateReagentListString("\n");
    break;
```

**Structure:** ✅ IDENTICAL (except class name)

---

#### Method 6: GetSpellToolTip() - Line ~1240

**Ice Magic:**
```csharp
case SpellBookType.VystiaIceMagic:
    offset = 0; // No cliloc tooltips for Vystia spells yet
    break;
```

**Druid:**
```csharp
case SpellBookType.VystiaDruid:
    offset = 0; // No cliloc tooltips for Vystia spells yet
    break;
```

**Structure:** ✅ IDENTICAL

---

### File: ClassicUO/Game/Data/SpellDefinition.cs - FullIndexGetSpell()

**Ice Magic:**
```csharp
// Line 237
if (fullidx < 1032) return SpellsVystiaIceMagic.GetSpell(fullidx - 999);  // 1000-1031 → 1-32
```

**Druid:**
```csharp
// Line 238
if (fullidx < 1064) return SpellsVystiaNature.GetSpell(fullidx - 1031);   // 1032-1063 → 1-32
```

**Math Check:**
- Ice spell 1000: `GetSpell(1000 - 999)` = `GetSpell(1)` ✓
- Ice spell 1031: `GetSpell(1031 - 999)` = `GetSpell(32)` ✓
- Druid spell 1032: `GetSpell(1032 - 1031)` = `GetSpell(1)` ✓
- Druid spell 1063: `GetSpell(1063 - 1031)` = `GetSpell(32)` ✓

**Structure:** ✅ IDENTICAL PATTERN, math is correct

---

## CRITICAL FINDING

### The ONE Difference: Item Graphic 0xEFA

**Ice Magic uses graphic 0x2252:**
- This graphic is ONLY shared with standard Chivalry spellbooks
- Chivalry uses a different hue, so hue check works perfectly
- `case 0x2252` matches immediately

**Druid uses graphic 0xEFA:**
- This graphic is shared with:
  - Magery (default)
  - 7 other Vystia spellbooks (Druid, Sorcerer, Oracle, Summoner, Shaman, Bard, Enchanter, Illusionist)
- `case 0x0EFA` has a long if-else chain checking 8 different hues
- Falls to Magery if no hue matches

### Hypothesis: Hue Not Being Sent Correctly

If the server sends a Druid spellbook with:
- Graphic: 0xEFA ✓
- Hue: 0 (no hue) ✗

Then AssignGraphic would:
```csharp
case 0x0EFA:
    if (item.Hue == 0x7D6) // FALSE (hue is 0)
        _spellBookType = SpellBookType.VystiaDruid;
    // ... all other checks fail
    else
        _spellBookType = SpellBookType.Magery; // ← FALLS HERE
```

Result: Druid book treated as Magery book → wrong spells shown

---

## DIAGNOSTIC TEST

Spawn a Druid spellbook in-game:
```
[add DruidSpellbook
```

Single-click to view properties. Check:
1. **Graphic ID** - Should show 3834 (decimal for 0xEFA)
2. **Hue** - Should show 2006 (decimal for 0x7D6)

If hue shows 0 or a different value, that's the bug!

---

## ALTERNATE THEORY: Graphic ID Mismatch

If the server sends graphic 0xEFA but the client expects 0x0EFA (with leading zero), they might not match.

**Test:** Try changing client code from:
```csharp
case 0x0EFA:
```

To:
```csharp
case 0xEFA:
```

(Though in C#, these are the same value, so this is unlikely)

---

## NEXT STEPS

1. **In-game test:** Spawn Druid book, check hue value
2. **If hue is wrong:** Fix server-side hue application
3. **If hue is correct:** Add debug logging to AssignGraphic to see which case matches

---

*Analysis created: 2025-12-08*
*Ice Magic is confirmed working - use as reference for all other schools*
