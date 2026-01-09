# Cleric - Current State vs Future State

**Class:** Cleric
**Ability IDs:** 2192-2207
**Archetype:** Healer/Support
**Theme:** Divine healer with holy damage and protective magic

---

## Current State

### Ability Distribution (16 abilities)
- **Healing:** 4 abilities (25%) - Heal, Prayer of Healing, Greater Heal, Divine Intervention
- **Damage:** 3 abilities (19%) - Smite, Divine Smite, Holy Nova
- **Buffs:** 3 abilities (19%) - Bless, Shield of Faith, Mass Blessing
- **Utility:** 5 abilities (31%) - Light, Turn Undead, Purify, Sanctuary, Resurrection
- **Transform:** 1 ability (6%) - Avatar of Light

### Abilities by Circle

#### Circle 1 (4 abilities)

| ID | Name | Type | Mana | Damage/Heal | Effect | Description |
|----|------|------|------|-------------|--------|-------------|
| 2192 | Heal | Heal | 4 | 18-28 | Effect: 0x376A, Hue: 0x1F2, Sound: 0x481 | Direct healing spell |
| 2193 | Smite | Damage | 4 | 12-18 Holy | Effect: 0x36D4, Hue: 0x1E5, Sound: 0x481 | Holy damage attack |
| 2194 | Bless | Buff | 4 | +10 All Stats, 30s | - | +10 STR/DEX/INT for 30 seconds |
| 2195 | Light | Utility | 4 | - | Effect: 0x36D4, Hue: 0x1E5, Sound: 0x481 | Illuminate area (placeholder: All Stats +10, 30s) |

#### Circle 2 (4 abilities)

| ID | Name | Type | Mana | Damage/Heal | Effect | Description |
|----|------|------|------|-------------|--------|-------------|
| 2196 | Prayer of Healing | Heal | 7 | 12-18 | Effect: 0x376A, Hue: 0x1F2, Sound: 0x481 | Group heal (weaker than single-target) |
| 2197 | Turn Undead | Utility | 7 | - | Effect: 0x36D4, Hue: 0x1E5, Sound: 0x481 | Fear undead (placeholder: All Stats +10, 30s) |
| 2198 | Shield of Faith | Buff | 7 | +10 All Stats, 30s | - | Damage absorb shield (placeholder: stat buff) |
| 2199 | Purify | Utility | 7 | - | Effect: 0x36D4, Hue: 0x1E5, Sound: 0x481 | Remove poison/disease (placeholder: All Stats +10, 30s) |

#### Circle 3 (4 abilities)

| ID | Name | Type | Mana | Damage/Heal | Effect | Description |
|----|------|------|------|-------------|--------|-------------|
| 2200 | Greater Heal | Heal | 10 | 35-55 | Effect: 0x376A, Hue: 0x1F2, Sound: 0x481 | Large single-target heal |
| 2201 | Divine Smite | Damage | 10 | 28-42 Holy | Effect: 0x36D4, Hue: 0x1E5, Sound: 0x481 | Empowered holy damage |
| 2202 | Sanctuary | AoE | 10 | 4 tile range | Effect: 0x36D4, Hue: 0x1E5, Sound: 0x481 | Pacify enemies in area (placeholder: 0 damage) |
| 2203 | Mass Blessing | Buff | 10 | +10 All Stats, 60s | - | Buff all allies in range (doubled duration) |

#### Circle 4 (4 abilities)

| ID | Name | Type | Mana | Damage/Heal | Effect | Description |
|----|------|------|------|-------------|--------|-------------|
| 2204 | Divine Intervention | Heal | 13 | 100-150 | Effect: 0x376A, Hue: 0x1F2, Sound: 0x481 | Emergency massive heal |
| 2205 | Holy Nova | AoE | 13 | 40-60 Holy, 4 tiles | Effect: 0x36D4, Hue: 0x1E5, Sound: 0x481 | Damage enemies, heal allies in AoE |
| 2206 | Resurrection | Utility | 13 | - | Effect: 0x36D4, Hue: 0x1E5, Sound: 0x481 | Revive dead ally (placeholder: All Stats +10, 30s) |
| 2207 | Avatar of Light | Transform | 13 | - | Effect: 0x36D4, Hue: 0x1E5, Sound: 0x481 | Become divine being (placeholder: All Stats +10, 30s) |

### Core Mechanics
- **Heal Scaling:** Circle 1 (18-28) → Circle 2 (12-18 AoE) → Circle 3 (35-55) → Circle 4 (100-150)
- **Damage Scaling:** Circle 1 (12-18) → Circle 3 (28-42) → Circle 4 (40-60 AoE)
- **Buff Duration:** 30s (single) → 60s (mass)
- **Holy Damage Type:** All damage is VystiaDamageType.Holy (effective vs undead)

### Strengths
- ✅ Strong single-target healing (4 heal spells, 18-150 HP range)
- ✅ Multiple buff options (Bless, Shield, Mass Blessing)
- ✅ Holy damage effective against undead
- ✅ Mix of single-target and AoE support
- ✅ Emergency heal (Divine Intervention: 100-150 HP)

### Weaknesses
- ⚠️ Many placeholder implementations (Light, Turn Undead, Purify, Sanctuary, Resurrection, Avatar of Light)
- ⚠️ No mobility abilities
- ⚠️ Limited CC (only Turn Undead, not functional)
- ⚠️ Low damage output (3 damage abilities, lowest 12-18)
- ⚠️ Mana-intensive (Circle 4 abilities cost 13 mana)

---

## Known Issues

### Placeholder Implementations
The following abilities are registered but use generic buff placeholders instead of specialized effects:

1. **Light (2195)** - Should illuminate area, currently gives All Stats +10
2. **Turn Undead (2197)** - Should fear undead, currently gives All Stats +10
3. **Shield of Faith (2198)** - Should absorb damage, currently gives All Stats +10
4. **Purify (2199)** - Should remove poison/disease, currently gives All Stats +10
5. **Sanctuary (2202)** - Should pacify enemies, currently deals 0 damage AoE
6. **Resurrection (2206)** - Should revive dead, currently gives All Stats +10
7. **Avatar of Light (2207)** - Should transform player, currently gives All Stats +10

### Missing Features
- **Actual Fear mechanic** for Turn Undead
- **Damage absorption** for Shield of Faith
- **Condition removal** for Purify (poison/disease/curse)
- **Light source** for Light ability
- **Pacification mechanic** for Sanctuary
- **Resurrection logic** for dead players
- **Transform/polymorph** for Avatar of Light
- **Dual effect logic** for Holy Nova (damage enemies, heal allies)

---

## Future State Proposals

**Date:** 2025-12-28
**Source:** Industry archetype standards (`Vystia/admin/ARCHETYPE_BALANCE_ANALYSIS.md`)

### Target Distribution
Based on **Healer/Support** archetype standards:
- **Healing:** 4-5 abilities (25-31%) - Current ✅
- **Damage:** 2-3 abilities (12-19%) - Current ✅
- **Buffs:** 3-4 abilities (19-25%) - Current ✅
- **Utility:** 4-5 abilities (25-31%) - Current ✅ (need functional implementations)
- **Mobility:** 1-2 abilities (6-12%) - Missing ❌

### Proposed New Abilities

#### Mobility Needs
**Recommended:** Replace one placeholder utility with mobility

**Divine Step**
**Circle:** 2
**Mana:** 7
**Effect:** Short-range teleport (8 tiles)
**Theme:** Divine intervention allows instant repositioning
**Purpose:** Escape danger, reposition for heals

### Ability Replacements/Updates

#### High Priority Fixes
1. **Turn Undead (2197)** → Implement actual fear mechanic against undead
2. **Shield of Faith (2198)** → Implement damage absorption shield
3. **Purify (2199)** → Implement cure poison/disease
4. **Resurrection (2206)** → Implement actual resurrection logic

#### Medium Priority
5. **Light (2195)** → Implement light source effect
6. **Sanctuary (2202)** → Implement pacification/invulnerability zone
7. **Avatar of Light (2207)** → Implement polymorph transformation with stat boost

#### Low Priority (Quality of Life)
8. **Holy Nova (2205)** → Implement dual logic (damage enemies, heal allies in same AoE)

### Implementation Priority
- **Phase 1 (Critical):** Turn Undead, Shield of Faith, Purify, Resurrection
- **Phase 2 (Important):** Add mobility ability (Divine Step), fix Sanctuary
- **Phase 3 (Enhancement):** Avatar of Light transform, Holy Nova dual effect

---

## File Locations

**Implementation:** `D:\UO\ServUO\Scripts\Custom\VystiaClasses\Abilities\Generated\Martial\ClericAbilities.cs`
**Documentation:** `D:\UO\Vystia\design\martial\Cleric.md`
**Class Definition:** `D:\UO\ServUO\Scripts\Custom\VystiaClasses\Classes\Martial\Cleric.cs`

**Generation Script:** `generate_martial_abilities.py` (auto-generated file, do not edit manually)

---

**Last Updated:** 2025-12-28
**Status:** Fully documented, 7 placeholder implementations identified, mobility gap noted
