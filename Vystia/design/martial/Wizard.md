# Wizard - Ability Documentation

**Class:** Wizard
**Ability IDs:** 2208-2223 (16 abilities)
**Archetype:** Flex Caster
**Theme:** Arcane generalist - master of magical fundamentals
**Primary Resource:** Mana

**Implementation Status:** ✅ All 16 abilities implemented
**File Location:** `ServUO/Scripts/Custom/VystiaClasses/Abilities/Generated/Martial/WizardAbilities.cs`

---

## Ability Summary

### Ability Distribution
- **Damage:** 5 abilities (31%) - Direct, missiles, AoE, barrage, meteor
- **Buff/Support:** 5 abilities (31%) - Shield, metamagic, power, time warp, ascendancy
- **Utility:** 4 abilities (25%) - Detect magic, counterspell, polymorph, ritual casting
- **Mobility:** 2 abilities (13%) - Blink, spellsteal

### Core Mechanics
- **Arcane Mastery:** Pure arcane damage scaling across all circles
- **Metamagic:** Spell enhancement mechanics (duration, damage)
- **Versatility:** Mix of single-target, AoE, buffs, and utility
- **Mana Economy:** Efficient low-circle spells, expensive high-circle ultimates

---

## Circle 1 Abilities (4 abilities)

| ID | Name | Type | Mana | Range | Effect | Description |
|----|------|------|------|-------|--------|-------------|
| 2208 | Arcane Bolt | Damage | 4 | 12 tiles | 10-16 arcane | Basic arcane damage |
| 2209 | Magic Shield | Buff | 4 | Self | 10 power, 30s | Mana absorbs damage |
| 2210 | Detect Magic | Utility | 4 | 12 tiles | 10 power, 30s | See magical auras |
| 2211 | Counterspell | Utility | 4 | 12 tiles | 10 power, 30s | Interrupt spellcast |

**Circle 1 Theme:** Foundation spells - basic damage, protection, detection, interruption

**Effect Details:**
- **Arcane Bolt:** Single-target arcane damage (10-16), standard impact effect (0x36D4, sound 0x481)
- **Magic Shield:** AllStatsBuff for 30 seconds (placeholder - intended to absorb damage with mana)
- **Detect Magic:** AllStatsBuff for 30 seconds (placeholder - intended to reveal magical auras)
- **Counterspell:** AllStatsBuff for 30 seconds (placeholder - intended to interrupt casting)

---

## Circle 2 Abilities (4 abilities)

| ID | Name | Type | Mana | Range | Effect | Description |
|----|------|------|------|-------|--------|-------------|
| 2212 | Arcane Missiles | Damage | 7 | 12 tiles | 18-28 arcane | Multi-hit arcane |
| 2213 | Blink | Mobility | 7 | 12 tiles | 10 power, 30s | Short teleport |
| 2214 | Polymorph | Utility | 7 | 12 tiles | 10 power, 30s | Turn into sheep |
| 2215 | Spellsteal | Utility | 7 | 12 tiles | 10 power, 30s | Steal enemy buff |

**Circle 2 Theme:** Intermediate spells - improved damage, mobility, transformation, dispel

**Effect Details:**
- **Arcane Missiles:** Increased damage (18-28), same impact effect as Bolt
- **Blink:** AllStatsBuff for 30 seconds (placeholder - intended to teleport short distance)
- **Polymorph:** AllStatsBuff for 30 seconds (placeholder - intended to transform target)
- **Spellsteal:** AllStatsBuff for 30 seconds (placeholder - intended to remove buff from enemy and apply to caster)

---

## Circle 3 Abilities (4 abilities)

| ID | Name | Type | Mana | Range | Effect | Description |
|----|------|------|------|-------|--------|-------------|
| 2216 | Arcane Explosion | AoE | 10 | 4 tiles | 22-35 arcane | Arcane AoE burst |
| 2217 | Metamagic: Extend | Buff | 10 | Self | 10 power, 60s | +50% spell duration |
| 2218 | Ritual Casting | Utility | 10 | 12 tiles | 10 power, 30s | Cast without combat |
| 2219 | Arcane Power | Buff | 10 | Self | 10 power, 60s | +30% spell damage |

**Circle 3 Theme:** Advanced spells - AoE damage, spell enhancement, ritual magic

**Effect Details:**
- **Arcane Explosion:** AoE damage (22-35) in 4-tile radius, same impact effect
- **Metamagic: Extend:** AllStatsBuff for 60 seconds (placeholder - intended to extend spell durations by 50%)
- **Ritual Casting:** AllStatsBuff for 30 seconds (placeholder - intended to allow spell casting outside combat)
- **Arcane Power:** AllStatsBuff for 60 seconds (placeholder - intended to increase spell damage by 30%)

---

## Circle 4 Abilities (4 abilities)

| ID | Name | Type | Mana | Range | Effect | Description |
|----|------|------|------|-------|--------|-------------|
| 2220 | Arcane Barrage | Damage | 13 | 12 tiles | 45-70 arcane | Consume charges damage |
| 2221 | Time Warp | Buff | 13 | Party | 10 power, 60s | +30% haste party |
| 2222 | Meteor | AoE | 13 | 4 tiles | 55-85 arcane | Massive fire AoE |
| 2223 | Arcane Ascendancy | Transform | 13 | Self | 10 power, 30s | Become arcane being |

**Circle 4 Theme:** Ultimate spells - massive damage, party buffs, transformation

**Effect Details:**
- **Arcane Barrage:** Highest single-target damage (45-70), same impact effect
- **Time Warp:** AllStatsBuff for 60 seconds (placeholder - intended to grant party-wide haste)
- **Meteor:** Highest AoE damage (55-85) in 4-tile radius, same impact effect
- **Arcane Ascendancy:** AllStatsBuff for 30 seconds (placeholder - intended to transform caster into powerful arcane form)

---

## Ability Categories

### Damage Abilities (5 total)
| Circle | Name | Mana | Damage | Type | Range |
|--------|------|------|--------|------|-------|
| 1 | Arcane Bolt | 4 | 10-16 | Single | 12 tiles |
| 2 | Arcane Missiles | 7 | 18-28 | Single | 12 tiles |
| 3 | Arcane Explosion | 10 | 22-35 | AoE (4 tiles) | Self-centered |
| 4 | Arcane Barrage | 13 | 45-70 | Single | 12 tiles |
| 4 | Meteor | 13 | 55-85 | AoE (4 tiles) | Targeted |

**Damage Progression:** Scales from 10-16 (Circle 1) to 55-85 (Circle 4 AoE)

### Buff/Support Abilities (5 total)
| Circle | Name | Mana | Duration | Effect |
|--------|------|------|----------|--------|
| 1 | Magic Shield | 4 | 30s | Mana absorption |
| 3 | Metamagic: Extend | 10 | 60s | +50% duration |
| 3 | Arcane Power | 10 | 60s | +30% spell damage |
| 4 | Time Warp | 13 | 60s | +30% haste (party) |
| 4 | Arcane Ascendancy | 13 | 30s | Arcane transformation |

**Buff Progression:** Defensive → Enhancement → Party Support → Transformation

### Utility Abilities (4 total)
| Circle | Name | Mana | Effect |
|--------|------|------|--------|
| 1 | Detect Magic | 4 | Reveal auras |
| 1 | Counterspell | 4 | Interrupt cast |
| 2 | Polymorph | 7 | Transform target |
| 3 | Ritual Casting | 10 | Non-combat casting |

**Utility Progression:** Detection/Interruption → Transformation → Ritual Magic

### Mobility Abilities (2 total)
| Circle | Name | Mana | Effect |
|--------|------|------|--------|
| 2 | Blink | 7 | Short teleport |
| 2 | Spellsteal | 7 | Steal buff (mobility via combat advantage) |

---

## Visual/Audio Effects

**Standard Effect Set (used by all damage abilities):**
- **Impact Effect:** 0x36D4 (arcane sparkle)
- **Sound:** 0x481 (magic impact sound)
- **Hue:** 0x1E5 (arcane blue)

**Note:** All 16 abilities currently use the same visual/audio effects. Future implementation should customize effects per ability type.

---

## Known Issues

### Placeholder Implementations
The following abilities are registered but have placeholder buff effects instead of full mechanics:

**Circle 1:**
- **Magic Shield:** Currently applies AllStatsBuff. Should absorb damage using mana pool.
- **Detect Magic:** Currently applies AllStatsBuff. Should reveal hidden magical items/effects/creatures.
- **Counterspell:** Currently applies AllStatsBuff. Should interrupt enemy spellcasting.

**Circle 2:**
- **Blink:** Currently applies AllStatsBuff. Should teleport caster to targeted location (short range).
- **Polymorph:** Currently applies AllStatsBuff. Should transform target into sheep (crowd control).
- **Spellsteal:** Currently applies AllStatsBuff. Should remove one buff from enemy and apply to caster.

**Circle 3:**
- **Metamagic: Extend:** Currently applies AllStatsBuff. Should increase duration of next spell by 50%.
- **Ritual Casting:** Currently applies AllStatsBuff. Should allow casting high-level spells outside combat with preparation time.
- **Arcane Power:** Currently applies AllStatsBuff. Should increase spell damage by 30% for duration.

**Circle 4:**
- **Arcane Barrage:** Damage is functional but should consume stacks/charges for scaling damage.
- **Time Warp:** Currently applies AllStatsBuff. Should grant party-wide haste (+30% cast/swing speed).
- **Arcane Ascendancy:** Currently applies AllStatsBuff. Should transform caster into arcane being with enhanced stats/abilities.

### Missing Features
- **Metamagic System:** No actual metamagic buff application logic
- **Arcane Charges:** No charge/stack system for Arcane Barrage scaling
- **Polymorph Transform:** No body transformation mechanic
- **Spell Interruption:** No interrupt system for Counterspell
- **Teleport Logic:** No teleportation implementation for Blink
- **Party Buff System:** Time Warp doesn't apply to party members
- **Transformation System:** Arcane Ascendancy doesn't change caster form

---

## Design Notes

### Archetype: Flex Caster
The Wizard embodies the "Flex Caster" archetype with:
- **Balanced toolkit:** 31% damage, 31% buffs, 25% utility, 13% mobility
- **Scaling options:** Damage scales from 10-16 (Circle 1) to 55-85 (Circle 4)
- **Versatile gameplay:** Can adapt to different encounter types (single-target, AoE, support)
- **Mana efficiency:** Low-circle spells cost 4-7 mana, high-circle cost 10-13 mana

### Thematic Coherence
- **Pure Arcane:** All damage abilities use VystiaDamageType.Arcane
- **Metamagic Focus:** Multiple spell enhancement abilities (Extend, Power)
- **Generalist Design:** No elemental specialization, focused on raw arcane manipulation
- **Academic Theme:** Ritual Casting, Detect Magic, Counterspell suggest scholarly approach

### Balance Considerations
- **Strong AoE:** Two AoE abilities (Arcane Explosion, Meteor) with good damage
- **Party Support:** Time Warp provides rare party-wide buff
- **Low Mobility:** Only 2 mobility abilities, both in Circle 2
- **Utility Gaps:** No healing, no permanent summons, limited crowd control

---

## Comparison to Other Caster Classes

| Class | Damage % | Utility % | Defense % | Mobility % | Theme |
|-------|----------|-----------|-----------|-----------|-------|
| **Wizard** | 31% | 25% | 25% (buffs) | 13% | Arcane generalist |
| Oracle | ~25% | ~40% | ~20% | ~15% | Divination/fate |
| Ice Mage | ~40% | ~15% | ~25% | ~20% | Ice specialization |
| Sorcerer | ~45% | ~10% | ~20% | ~25% | Fire/elemental |

**Wizard Niche:** Balanced arcane caster with metamagic focus and party support options.

---

## Implementation Details

**File Location:** `ServUO/Scripts/Custom/VystiaClasses/Abilities/Generated/Martial/WizardAbilities.cs`

**Registration Method:** `WizardAbilities.RegisterAll()`

**Dependencies:**
- `AbilityDefinition` class for ability data structure
- `AbilityRegistry` for ability registration
- `VystiaDamageType.Arcane` for damage type
- `VystiaBuffType.AllStatsBuff` for placeholder buffs
- `AbilitySchool.Wizard` for school categorization

**Generation Source:** Auto-generated by `generate_martial_abilities.py` script

---

**Last Updated:** 2025-12-28
**Status:** All abilities implemented with placeholders for complex mechanics
