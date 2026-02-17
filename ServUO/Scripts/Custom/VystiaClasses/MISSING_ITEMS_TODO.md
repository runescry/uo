# Vystia Character Classes - Missing Custom Items

## Overview
This document lists all custom items that were designed for the class system but need to be implemented. The class code references these items but they don't exist yet.

---

## ❌ Custom Ability Items (5 items)

### 1. RageTotem
**Used By:** Barbarian
**File:** Referenced in `BarbarianClass.cs`
**Current Status:** ⚠️ Implemented in class file but needs to be moved to separate item file

**Specifications:**
- **Item ID:** 0x1F1C (totem)
- **Hue:** 1150 (ice blue)
- **Weight:** 1.0
- **Loot Type:** Blessed
- **Charges:** 10

**Functionality:**
- Double-click to activate rage mode
- Grants +20 STR for 30 seconds
- Consumes 1 charge per use
- Visual: Ice particles (0x3709)
- Sound: 0x208

**Implementation Details:**
```csharp
public class RageTotem : Item
{
    private int m_Charges = 10;

    public override void OnDoubleClick(Mobile from)
    {
        // Activate rage buff
        // Apply +20 STR StatMod for 30 seconds
        // Consume 1 charge
        // Visual and sound effects
    }
}

public class RageBuff
{
    // Timer-based buff system
    // +20 STR modifier
    // 30 second duration
}
```

**Dependencies:**
- None (uses standard ServUO)

---

### 2. ConstructControlDevice
**Used By:** Artificer
**File:** Referenced in `ArtificerClass.cs`
**Current Status:** ⚠️ Implemented in class file but needs to be moved to separate item file

**Specifications:**
- **Item ID:** 0x1EB8 (clock)
- **Hue:** 2305 (metallic)
- **Weight:** 2.0
- **Loot Type:** Blessed
- **Charges:** 5

**Functionality:**
- Double-click to summon clockwork construct
- Summons ClockworkScout creature
- Scout lasts 10 minutes then auto-despawns
- Consumes 1 charge per summon
- Visual: Mechanical particles (0x3728)
- Sound: 0x22F

**Implementation Details:**
```csharp
public class ConstructControlDevice : Item
{
    private int m_Charges = 5;

    public override void OnDoubleClick(Mobile from)
    {
        // Summon ClockworkScout
        // Find valid spawn location (2 tile radius)
        // Set construct as controlled by player
        // Auto-despawn after 10 minutes
        // Consume 1 charge
    }
}
```

**Dependencies:**
- ClockworkScout creature (implemented in ArtificerClass.cs)

**Related Creature:**
```csharp
public class ClockworkScout : BaseCreature
{
    // Body: 752 (Golem), Hue: 2305
    // Stats: 80-100 STR, 60-80 DEX, 30-50 INT
    // 10 minute lifespan, no corpse on death
}
```

---

### 3. ShapeshiftTotem
**Used By:** Druid
**File:** Referenced in `DruidClass.cs`
**Current Status:** ⚠️ Implemented in class file but needs to be moved to separate item file

**Specifications:**
- **Item ID:** 0x1F1C (totem)
- **Hue:** 2010 (forest green)
- **Weight:** 1.0
- **Loot Type:** Blessed
- **Charges:** Unlimited

**Functionality:**
- Double-click to shapeshift
- Options: Bear, Wolf, Hawk, Human
- Changes body type temporarily
- Grants form-specific bonuses

**Implementation Details:**
```csharp
public class ShapeshiftTotem : Item
{
    public override void OnDoubleClick(Mobile from)
    {
        // Display form selection gump
        // Transform player body
        // Apply stat/skill modifiers based on form
        // Duration: until dismissed or death
    }
}

// Form bonuses:
// Bear: +20 STR, +10 DEX, +15 Wrestling
// Wolf: +15 DEX, +10 STR, +15 Tracking, +10 Stealth
// Hawk: +20 DEX, -10 STR, Flight, +15 Tracking
// Human: Restore normal form
```

**Dependencies:**
- Gump system for form selection
- Body transformation system

**Suggested Bodies:**
- Bear: Body 211 (Grizzly Bear)
- Wolf: Body 225 (Timber Wolf)
- Hawk: Body 5 (Bird)

---

### 4. HolySymbol
**Used By:** Cleric
**File:** Referenced in `AllClasses.cs`
**Current Status:** ⚠️ Implemented in class file but needs to be moved to separate item file

**Specifications:**
- **Item ID:** 0x1F14 (holy symbol)
- **Hue:** 1153 (holy white/gold)
- **Weight:** 0.5
- **Loot Type:** Blessed
- **Cooldown:** Time-based (not charge-based)

**Functionality:**
- Double-click to channel divine healing
- AoE heal (5 tile radius)
- Heals 10-20 HP to all allies in range
- Visual: Holy light (0x373A, 0x376A)
- Sound: 0x1F2
- Cooldown: 60 seconds

**Implementation Details:**
```csharp
public class HolySymbol : Item
{
    private DateTime m_NextUse;

    public override void OnDoubleClick(Mobile from)
    {
        if (DateTime.UtcNow < m_NextUse)
        {
            from.SendMessage("The holy symbol needs time to recharge.");
            return;
        }

        // Heal all allies in 5 tile radius
        // 10-20 HP per target
        // Visual and sound effects
        // Set 60 second cooldown
    }
}
```

**Dependencies:**
- None (uses standard ServUO)

---

### 5. ArtificerBlueprints
**Used By:** Artificer
**File:** Referenced in `ArtificerClass.cs`
**Current Status:** ⚠️ Implemented in class file but needs to be moved to separate item file

**Specifications:**
- **Item ID:** 0x14F0 (book)
- **Hue:** 2305 (metallic)
- **Weight:** 3.0
- **Loot Type:** Blessed

**Functionality:**
- Double-click to open blueprint gump
- Display available construct designs
- Reference material for crafting
- Future: Unlock new construct types

**Implementation Details:**
```csharp
public class ArtificerBlueprints : Item
{
    public override void OnDoubleClick(Mobile from)
    {
        from.SendGump(new ArtificerBlueprintsGump());
    }
}

public class ArtificerBlueprintsGump : Gump
{
    // Display construct types:
    // - Clockwork Scout (basic)
    // - Mechanical Turret (defense)
    // - Steam Golem (tank)
    // - Repair Spider (utility)
}
```

**Dependencies:**
- Gump system

---

## ❌ Custom Spellbooks (3 items)

### 1. IceMageSpellbook
**Used By:** Ice Mage
**File:** Referenced in `IceMageClass.cs`
**Current Status:** ⚠️ Implemented in class file but needs to be moved to separate item file

**Specifications:**
- **Base:** Spellbook (0xEFA)
- **Hue:** 1150 (ice blue)
- **Loot Type:** Blessed
- **Spell Count:** 3 (expandable to 64)

**Contains:**
1. Ice Bolt (slot 0) - Single target damage + slow
2. Frost Armor (slot 1) - Defensive buff
3. Freeze (slot 2) - Crowd control

**Implementation:**
```csharp
public class IceMageSpellbook : Spellbook
{
    public override SpellbookType SpellbookType => SpellbookType.Regular;
    public override int BookOffset => 0;
    public override int BookCount => 64;

    [Constructable]
    public IceMageSpellbook() : base((ulong)0, 0xEFA)
    {
        Name = "Ice Mage Spellbook";
        Hue = 1150;
        LootType = LootType.Blessed;

        // Add starting spells
        Content |= (1ul << 0);  // Ice Bolt
        Content |= (1ul << 1);  // Frost Armor
        Content |= (1ul << 2);  // Freeze
    }
}
```

**Dependencies:**
- Ice Mage spells (see below)

---

### 2. DruidSpellbook
**Used By:** Druid
**File:** Referenced in `DruidClass.cs`
**Current Status:** ⚠️ Implemented in class file but needs to be moved to separate item file

**Specifications:**
- **Base:** Spellbook (0xEFA)
- **Hue:** 2010 (forest green)
- **Loot Type:** Blessed
- **Spell Count:** TBD (nature magic)

**Future Spells:**
- Entangling Roots
- Nature's Wrath
- Healing Touch
- Summon Spirit Wolf
- Regeneration Aura

**Implementation:**
```csharp
public class DruidSpellbook : Spellbook
{
    [Constructable]
    public DruidSpellbook() : base((ulong)0, 0xEFA)
    {
        Name = "Druid Spellbook";
        Hue = 2010;
        LootType = LootType.Blessed;
    }
}
```

---

### 3. WitchSpellbook (Grimoire of Hexes)
**Used By:** Witch
**File:** Referenced in `AllClasses.cs`
**Current Status:** ⚠️ Implemented in class file but needs to be moved to separate item file

**Specifications:**
- **Base:** Spellbook (0xEFA)
- **Hue:** 2073 (murky green)
- **Loot Type:** Blessed
- **Spell Count:** TBD (hex/curse magic)

**Future Spells:**
- Hex of Weakness
- Poison Bolt
- Bog Curse
- Summon Swamp Familiar
- Death Bloom

**Implementation:**
```csharp
public class WitchSpellbook : Spellbook
{
    [Constructable]
    public WitchSpellbook() : base((ulong)0, 0xEFA)
    {
        Name = "Grimoire of Hexes";
        Hue = 2073;
        LootType = LootType.Blessed;
    }
}
```

---

## ✅ Custom Spells (3 implemented, many more needed)

### Implemented Ice Mage Spells

#### 1. Ice Bolt ✓
**File:** `Spells/IceMage/IceBoltSpell.cs`
**Status:** Fully implemented

**Specifications:**
- **Circle:** 3rd
- **Mana Cost:** 9
- **Reagents:** MandrakeRoot, SulfurousAsh
- **Cast Time:** 1.5s
- **Skill Required:** 30.0 Magery
- **Range:** 12 tiles
- **Damage:** 10-18 base (scales with Magery/EvalInt/INT)
- **Special:** 25% chance to slow (-15 DEX for 5s)
- **Visual:** Ice projectile (0x36E4), blue hue
- **Sound:** 0x1FB

#### 2. Frost Armor ✓
**File:** `Spells/IceMage/FrostArmorSpell.cs`
**Status:** Fully implemented

**Specifications:**
- **Circle:** 4th
- **Mana Cost:** 12
- **Reagents:** Garlic, Ginseng, MandrakeRoot
- **Cast Time:** 2.0s
- **Skill Required:** 40.0 Magery
- **Duration:** 120-240s (scales with Magery)
- **Effect:** +10 Physical Resist, +20 Cold Resist
- **Visual:** Frost aura (0x375A)
- **Sound:** 0x1E9

#### 3. Blizzard ✓
**File:** `Spells/IceMage/BlizzardSpell.cs`
**Status:** Fully implemented

**Specifications:**
- **Circle:** 6th
- **Mana Cost:** 25
- **Reagents:** MandrakeRoot, SulfurousAsh, Bloodmoss, SpidersSilk
- **Cast Time:** 3.0s
- **Skill Required:** 70.0 Magery
- **Range:** 12 tiles (ground target)
- **Area:** 5 tile radius
- **Duration:** 10 seconds (10 ticks)
- **Damage:** 3-8 per tick (scales with Magery)
- **Special:** Slows enemies in area
- **Visual:** Ice storm (0x3709)
- **Sound:** 0x64F

---

### Needed Future Spells

#### Ice Mage (additional spells)
- **Ice Lance** - Piercing projectile
- **Frozen Fortress** - Ice wall creation
- **Permafrost** - Ground freeze AoE
- **Glacial Spike** - High damage single target
- **Ice Prison** - Freeze enemy in place

#### Druid Spells
- **Entangling Roots** - Root enemies
- **Nature's Wrath** - Lightning storm
- **Healing Touch** - Direct heal
- **Summon Spirit Wolf** - Pet summon
- **Wild Shape** - Shapeshift spell (alternative to totem)
- **Regeneration** - HoT (Heal over Time)
- **Thorn Barrier** - Damage reflect

#### Witch Spells
- **Hex of Weakness** - Stat debuff
- **Poison Bolt** - Poison damage projectile
- **Bog Curse** - DoT + slow
- **Summon Swamp Familiar** - Toad/bat pet
- **Death Bloom** - Poison plant AoE
- **Wither** - Necrotic damage
- **Nightmare Vision** - Fear effect

#### Sorcerer Spells (Elemental)
- **Magma Bolt** - Fire damage
- **Volcanic Shield** - Fire reflect
- **Pyroclastic Surge** - Fire AoE
- **Elemental Embodiment** - Fire form transformation
- **Meteor Storm** - Ultimate fire spell

#### Warlock Spells (Dark Magic)
- **Eldritch Blast** - Force beam
- **Life Drain** - HP steal
- **Summon Patron** - Demon summon
- **Dark Pact** - HP for power trade
- **Shadow Bolt** - Shadow damage
- **Curse of Agony** - DoT

#### Necromancer Spells
- **Raise Undead** - Summon skeleton/zombie
- **Life Drain** - HP steal
- **Death Nova** - Necrotic AoE
- **Phylactery** - Soul transfer
- **Corpse Explosion** - AoE from corpse
- **Death Coil** - Heal undead or damage living

---

## Implementation Priority

### Phase 1 - Core Ability Items (Highest Priority)
1. **RageTotem** - Required for Barbarian gameplay
2. **ConstructControlDevice** - Required for Artificer gameplay
3. **ShapeshiftTotem** - Required for Druid gameplay
4. **HolySymbol** - Required for Cleric gameplay

### Phase 2 - Spellbooks
1. **IceMageSpellbook** - Move to separate file
2. **DruidSpellbook** - Move to separate file
3. **WitchSpellbook** - Move to separate file

### Phase 3 - Additional Spells
1. Complete Ice Mage spell set (5 more spells)
2. Implement Druid spell set (7 spells)
3. Implement Witch spell set (7 spells)
4. Implement other caster spell sets

### Phase 4 - Utility Items
1. **ArtificerBlueprints** - Reference material

---

## File Organization

### Recommended Structure
```
Scripts/Custom/VystiaClasses/
├── Items/
│   ├── AbilityItems/
│   │   ├── RageTotem.cs
│   │   ├── ConstructControlDevice.cs
│   │   ├── ShapeshiftTotem.cs
│   │   ├── HolySymbol.cs
│   │   └── ArtificerBlueprints.cs
│   ├── Spellbooks/
│   │   ├── IceMageSpellbook.cs
│   │   ├── DruidSpellbook.cs
│   │   └── WitchSpellbook.cs
│   └── Creatures/
│       └── ClockworkScout.cs
├── Spells/
│   ├── IceMage/
│   │   ├── IceBoltSpell.cs ✓
│   │   ├── FrostArmorSpell.cs ✓
│   │   ├── BlizzardSpell.cs ✓
│   │   ├── IceLanceSpell.cs
│   │   └── [more...]
│   ├── Druid/
│   │   └── [7 spells]
│   ├── Witch/
│   │   └── [7 spells]
│   └── [other classes]
```

---

## Testing Checklist

### Per Ability Item
- [ ] Item creates successfully
- [ ] Double-click activates ability
- [ ] Visual effects display correctly
- [ ] Sound effects play
- [ ] Charges/cooldown system works
- [ ] Stat modifiers apply and expire correctly
- [ ] Serialization/deserialization works

### Per Spell
- [ ] Spell casts successfully
- [ ] Mana cost deducts correctly
- [ ] Reagents consumed
- [ ] Targeting works (single/AoE/ground)
- [ ] Damage/healing scales with skills
- [ ] Visual effects display
- [ ] Sound effects play
- [ ] Buffs/debuffs apply and expire
- [ ] PvP and PvE both work

---

## Resource Requirements

All ability items and spells should use **existing Vystia resources**:

**Ice Mage:**
- FrozenOre (common)
- FrostforgedIngot (powerful spells)
- EternalIce (ultimate spells)

**Artificer:**
- SteamworkOre
- ClockworkGear
- ClockworkSpring
- SteamCore (advanced constructs)

**Druid:**
- LivingBark
- LivingOre
- TreantHeart (special item, exists)

**Witch:**
- SwampLotus
- BogIronOre
- Standard poison potions

**Cleric:**
- Standard reagents
- Holy water (if implemented)

---

*Last Updated: 2025-12-05*
*Status: 8 items need extraction/reorganization, 3 spells complete, ~50+ spells needed*
