# Proposed Class Equipment Diversification

## Problem
Currently, **12 out of 25 classes** use the same base weapon (Gnarled Staff `0x13F8`), making classes visually identical. Armor/clothing also needs more variety.

## Proposed Weapon Changes

### Magic Classes - Weapon Diversification

| Class | Current | Proposed | Item ID | Flipable | Rationale |
|------|---------|----------|---------|----------|-----------|
| **Ice Mage** | Gnarled Staff | **Glass Staff** | `0x905` | `0x4070` | Crystal/ice theme, visually distinct |
| **Warlock** | Gnarled Staff | **Black Staff** | `0xDF0` | `0xDF1` | Dark magic theme, shadowy appearance |
| **Necromancer** | Scythe (Vystia) | **Scythe (Vystia)** | `0x26BA` | `0x26C4` | ✓ Keep - already unique |
| **Druid** | Wild Staff (Vystia) | **Wild Staff (Vystia)** | `0x2D25` | `0x2D31` | ✓ Keep - already unique |
| **Sorcerer** | Gnarled Staff | **Quarter Staff** | `0xE89` | `0xE8A` | Elemental focus, simpler design |
| **Bard** | Lute | **Lute** | `0xEB3` | N/A | ✓ Keep - instrument, unique |

### Extended Magic Classes

| Class | Current | Proposed | Item ID | Flipable | Rationale |
|------|---------|----------|---------|----------|-----------|
| **Witch** | Gnarled Staff | **Shepherd's Crook** | `0xE81` | `0xE82` | Hex/curse theme, twisted appearance |
| **Oracle** | Gnarled Staff | **Glass Staff** | `0x905` | `0x4070` | Crystal/divination theme, clear/reflective |
| **Summoner** | Gnarled Staff | **Serpent Stone Staff** | `0x906` | `0x406F` | Summoning/binding theme, serpent motif |
| **Shaman** | Gnarled Staff | **Shepherd's Crook** | `0xE81` | `0xE82` | Spirit/totem theme, natural appearance |
| **Enchanter** | Gnarled Staff | **Scepter** | `0x26BC` | `0x26C6` | Enchanting/runic theme, regal appearance |
| **Illusionist** | Gnarled Staff | **Black Staff** | `0xDF0` | `0xDF1` | Illusion/deception theme, mysterious |
| **Alchemist** | Gnarled Staff | **Mace** | `0xF5C` | `0xF5D` | Alchemy/potion theme, tool-like |
| **Cleric** | Gnarled Staff | **Scepter** | `0x26BC` | `0x26C6` | Healing/divine theme, ceremonial |
| **Wizard** | Gnarled Staff | **Quarter Staff** | `0xE89` | `0xE8A` | Traditional magery, classic staff |

## Proposed Armor/Clothing Changes

### Current Issues
- Many classes use generic `Robe()` with only name/hue changes
- Leather armor classes are similar
- Plate armor classes are identical

### Proposed Armor Diversification

**Note**: UO uses `Robe()` class for all robes - visual distinction comes from hue and name. For more variety, we can use different hat types and add accessories.

#### Magic Classes

| Class | Current Armor | Proposed Armor | Rationale |
|------|---------------|----------------|-----------|
| **Ice Mage** | Robe + WizardsHat | Robe + **Floppy Hat** (`0x1713`) | More distinct, less generic |
| **Warlock** | Robe + Cloak | Robe + Cloak + **Wide Brim Hat** (`0x1714`) | Darker, more mysterious |
| **Necromancer** | Robe + BoneHelm | Robe + BoneHelm | ✓ Keep - already unique |
| **Druid** | Leather Set | Leather Set | ✓ Keep - appropriate |
| **Sorcerer** | Robe + WizardsHat | Robe + **Floppy Hat** (`0x1713`) | More elemental-themed |
| **Bard** | FancyShirt + LongPants | FancyShirt + LongPants | ✓ Keep - appropriate |

#### Extended Magic Classes

| Class | Current Armor | Proposed Armor | Rationale |
|------|---------------|----------------|-----------|
| **Witch** | Robe + Cloak + WizardsHat | Robe + Cloak + **Floppy Hat** (`0x1713`) | More witch-like appearance |
| **Oracle** | Robe + Cloak | Robe + Cloak + **Wide Brim Hat** (`0x1714`) | More seer-like, mysterious |
| **Summoner** | Robe + Cloak | Robe + Cloak + **Floppy Hat** (`0x1713`) | More conjurer-themed |
| **Shaman** | Leather Set | Leather Set + **Body Sash** (`0x1541`) | More spirit-themed, tribal |
| **Enchanter** | Robe + Cloak | Robe + Cloak + **WizardsHat** | More runic/enchanting theme |
| **Illusionist** | Robe + Cloak | Robe + Cloak + **Floppy Hat** (`0x1713`) | More mysterious appearance |
| **Alchemist** | Robe + Cloak | Robe + Cloak + **Half Apron** (`0x153B`) | More lab coat-like, practical |
| **Cleric** | Robe + Cloak | Robe + Cloak + **Wide Brim Hat** (`0x1714`) | More vestment-like, holy |
| **Wizard** | Robe + WizardsHat | Robe + WizardsHat | ✓ Keep - traditional mage |

#### Martial Classes

| Class | Current Armor | Proposed Armor | Rationale |
|------|---------------|----------------|-----------|
| **Barbarian** | Bone Set | Bone Set | ✓ Keep - appropriate |
| **Rogue** | Leather Set | Leather Set | ✓ Keep - appropriate |
| **Monk** | MonkRobe | MonkRobe | ✓ Keep - unique |
| **Knight** | Plate Set | Plate Set | ✓ Keep - appropriate |
| **Paladin** | Plate Set | Plate Set | ✓ Keep - appropriate |
| **Ranger** | Leather Set | Leather Set | ✓ Keep - appropriate |
| **Fighter** | Chain Set | Chain Set | ✓ Keep - appropriate |
| **Templar** | Plate Set | Plate Set | ✓ Keep - appropriate |
| **Bounty Hunter** | Leather Set | Leather Set | ✓ Keep - appropriate |
| **Beastmaster** | Leather Set | Leather Set | ✓ Keep - appropriate |
| **Artificer** | Leather Set | Leather Set | ✓ Keep - appropriate |

## Summary of Changes

### Weapons
- **Reduce Gnarled Staff usage**: From 12 classes → 0 classes
- **New weapon distribution**:
  - Glass Staff: 2 classes (Ice Mage, Oracle)
  - Black Staff: 2 classes (Warlock, Illusionist)
  - Quarter Staff: 2 classes (Sorcerer, Wizard)
  - Shepherd's Crook: 2 classes (Witch, Shaman)
  - Scepter: 2 classes (Enchanter, Cleric)
  - Serpent Stone Staff: 1 class (Summoner)
  - Mace: 1 class (Alchemist)
  - Keep existing unique: Necromancer (Scythe), Druid (Wild Staff), Bard (Lute)

### Armor
- **Hats**: Diversify hat types - use `FloppyHat()` (`0x1713`) for Ice Mage, Sorcerer, Witch, Summoner, Illusionist
- **Hats**: Use `WideBrimHat()` (`0x1714`) for Warlock, Oracle, Cleric
- **Accessories**: Add `BodySash` for Shaman, `HalfApron` for Alchemist
- **Martial classes**: Mostly keep as-is (already diverse)
- **Note**: UO doesn't have separate "FancyRobe" class - all robes use `Robe()` with different hues/names

## Implementation Notes

1. **Item IDs Verified**:
   - Glass Staff: `0x905` (flipable: `0x4070`) ✓
   - Black Staff: `0xDF0` (flipable: `0xDF1`) ✓
   - Quarter Staff: `0xE89` (flipable: `0xE8A`) ✓
   - Shepherd's Crook: `0xE81` (flipable: `0xE82`) ✓
   - Scepter: `0x26BC` (flipable: `0x26C6`) ✓
   - Serpent Stone Staff: `0x906` (flipable: `0x406F`) ✓
   - Mace: `0xF5C` (flipable: `0xF5D`) ✓
   - Floppy Hat: `0x1713` ✓
   - Wide Brim Hat: `0x1714` ✓
   - Body Sash: `0x1541` ✓
   - Half Apron: `0x153B` ✓

2. **Classes needing updates**: 12 magic classes (weapons) + 9 magic classes (armor)

3. **Visual diversity**: Each class will now have a visually distinct weapon and armor combination

## Final Weapon Distribution Summary

After changes, weapon distribution will be:

| Weapon Type | Count | Classes Using It |
|-------------|-------|------------------|
| **Glass Staff** | 2 | Ice Mage, Oracle |
| **Black Staff** | 2 | Warlock, Illusionist |
| **Quarter Staff** | 2 | Sorcerer, Wizard |
| **Shepherd's Crook** | 2 | Witch, Shaman |
| **Scepter** | 2 | Enchanter, Cleric |
| **Serpent Stone Staff** | 1 | Summoner |
| **Mace** | 1 | Alchemist |
| **Scythe (Vystia)** | 1 | Necromancer |
| **Wild Staff (Vystia)** | 1 | Druid |
| **Lute** | 1 | Bard |
| **Two Handed Axe** | 1 | Barbarian |
| **Dagger** (x2) | 1 | Rogue |
| **Viking Sword** | 2 | Knight, Fighter |
| **Longsword** | 1 | Templar |
| **Kryss + Crossbow** | 1 | Bounty Hunter |
| **Bow** | 1 | Beastmaster |
| **Mace** | 1 | Artificer |
| **Lance (Vystia)** | 1 | Paladin |
| **Yumi (Vystia)** | 1 | Ranger |
| **None (Fists)** | 1 | Monk |

**Result**: No weapon type used by more than 2 classes (except Gnarled Staff which will be eliminated)
