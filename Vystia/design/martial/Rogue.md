# Rogue - Current State vs Future State

**Class:** Rogue
**Ability IDs:** 2048-2063
**Archetype:** Burst/Utility
**Theme:** Shadow operative with combo point system
**File Location:** `ServUO/Scripts/Custom/VystiaClasses/Abilities/Generated/Martial/RogueAbilities.cs`

---

## Current State

### Ability Distribution (16 abilities)
- **Damage:** 5 abilities (31%) - Sinister Strike, Backstab, Ambush, Death from Above, Eviscerate
- **Combo Builders:** 2 abilities (13%) - Sinister Strike, Backstab
- **Combo Finishers:** 4 abilities (25%) - Eviscerate, Kidney Shot, Rupture, Death from Above
- **Stealth:** 4 abilities (25%) - Stealth, Cheap Shot, Shadow Dance, Vanish
- **Utility:** 2 abilities (13%) - Pick Pocket, Gouge
- **Buffs:** 3 abilities (19%) - Sprint, Blade Flurry, Vendetta

### Core Mechanics
- **Combo Point System:** Build combo points with Sinister Strike/Backstab, spend on finishers
- **Stealth Gameplay:** Multiple stealth abilities (Stealth, Shadow Dance, Vanish) with stealth-only openers (Cheap Shot, Ambush)
- **Positional Bonuses:** Backstab does bonus damage from behind
- **Bleed DoT:** Rupture applies physical bleed over time, scales with combo points

### Strengths
- ✅ Strong burst damage with combo finishers
- ✅ Versatile stealth mechanics (3 ways to stealth)
- ✅ Good single-target control (Kidney Shot, Gouge, Cheap Shot)
- ✅ Positional gameplay rewards skilled play (Backstab)
- ✅ DoT pressure with Rupture

### Weaknesses
- ⚠️ **No AoE damage** - Pure single-target focus
- ⚠️ **No defensive abilities** - Relies on stealth/avoidance only
- ⚠️ **Limited mobility** - Only Sprint for movement
- ⚠️ **Minimal utility** - Pick Pocket is niche/placeholder
- ⚠️ **All buffs are placeholders** - Generic AllStatsBuff instead of specific effects

---

## Ability Tables by Circle

### Circle 1 (4 abilities)

| ID | Name | Type | Mana | Damage/Effect | Description |
|----|------|------|------|---------------|-------------|
| 2048 | Sinister Strike | Damage (Builder) | 4 | 12-18 Physical | Build combo point |
| 2049 | Backstab | Damage (Builder) | 4 | 20-30 Physical | Bonus from behind, build combo point |
| 2050 | Stealth | Buff | 4 | AllStatsBuff +10 (30s) | Enter stealth mode |
| 2051 | Pick Pocket | Utility | 4 | AllStatsBuff +10 (30s) | Steal gold from target |

**Effects:** 0x36D4, Sound: 0x1E5, Hit Sound: 0x481

### Circle 2 (4 abilities)

| ID | Name | Type | Mana | Damage/Effect | Description |
|----|------|------|------|---------------|-------------|
| 2052 | Eviscerate | Finisher | 8 | 12 damage/combo × 7 combo | Per combo point damage |
| 2053 | Kidney Shot | Finisher (CC) | 7 | AllStatsBuff +10 (30s) | Stun per combo point |
| 2054 | Gouge | Control | 7 | AllStatsBuff +10 (30s) | Incapacitate for 4s |
| 2055 | Sprint | Buff | 7 | AllStatsBuff +10 (30s) | +50% move speed |

**Effects:** 0x36D4, Sound: 0x1E5, Hit Sound: 0x481

### Circle 3 (4 abilities)

| ID | Name | Type | Mana | Damage/Effect | Description |
|----|------|------|------|---------------|-------------|
| 2056 | Ambush | Damage | 10 | 35-50 Physical | From stealth bonus |
| 2057 | Blade Flurry | Buff | 10 | AllStatsBuff +10 (60s) | Attacks hit 2 targets |
| 2058 | Cheap Shot | Control | 10 | AllStatsBuff +10 (30s) | Stun from stealth |
| 2059 | Rupture | DoT Finisher | 10 | 15 dmg/tick × 6 ticks | Bleed per combo point |

**DoT Type:** Physical
**Effects:** 0x36D4, Sound: 0x1E5, Hit Sound: 0x481

### Circle 4 (4 abilities)

| ID | Name | Type | Mana | Damage/Effect | Description |
|----|------|------|------|---------------|-------------|
| 2060 | Shadow Dance | Buff | 13 | AllStatsBuff +10 (60s) | Use stealth abilities |
| 2061 | Vendetta | Debuff | 13 | AllStatsBuff +10 (30s) | +20% damage to target |
| 2062 | Death from Above | Damage Finisher | 13 | 45-70 Physical | Leap strike finisher |
| 2063 | Vanish | Buff | 13 | AllStatsBuff +10 (60s) | Instant stealth, drop threat |

**Effects:** 0x36D4, Sound: 0x1E5, Hit Sound: 0x481

---

## Abilities by Category

### Combo Builders (2)
| Name | ID | Circle | Damage | Notes |
|------|-----|--------|--------|-------|
| Sinister Strike | 2048 | 1 | 12-18 | Basic builder |
| Backstab | 2049 | 1 | 20-30 | Positional, bonus from behind |

### Combo Finishers (4)
| Name | ID | Circle | Effect | Notes |
|------|-----|--------|--------|-------|
| Eviscerate | 2052 | 2 | 12 dmg/combo × 7 max | Pure damage finisher |
| Kidney Shot | 2053 | 2 | Stun per combo | Control finisher |
| Rupture | 2059 | 3 | 15 dmg/tick × 6 ticks | Bleed DoT finisher |
| Death from Above | 2062 | 4 | 45-70 damage | Leap strike finisher |

### Stealth Abilities (4)
| Name | ID | Circle | Effect | Notes |
|------|-----|--------|--------|-------|
| Stealth | 2050 | 1 | Enter stealth | Basic stealth |
| Cheap Shot | 2058 | 3 | Stun from stealth | Stealth opener |
| Shadow Dance | 2060 | 4 | Use stealth abilities | Combat stealth buff |
| Vanish | 2063 | 4 | Instant stealth | Emergency escape |

### Buffs/Debuffs (3)
| Name | ID | Circle | Effect | Duration | Notes |
|------|-----|--------|--------|----------|-------|
| Sprint | 2055 | 2 | +50% move speed | 30s | Mobility |
| Blade Flurry | 2057 | 3 | Hit 2 targets | 60s | Cleave effect |
| Vendetta | 2061 | 4 | +20% damage to target | 30s | Damage amp |

### Control Abilities (2)
| Name | ID | Circle | Effect | Notes |
|------|-----|--------|--------|-------|
| Gouge | 2054 | 2 | Incapacitate 4s | Short CC |
| Cheap Shot | 2058 | 3 | Stun from stealth | Stealth opener |

### Utility (2)
| Name | ID | Circle | Effect | Notes |
|------|-----|--------|--------|-------|
| Pick Pocket | 2051 | 1 | Steal gold | Niche/placeholder |
| Ambush | 2056 | 3 | High stealth damage | Stealth burst |

---

## Known Issues

### Placeholder Implementations
1. **All Buffs Use Generic AllStatsBuff:**
   - Stealth (2050) - Should provide actual invisibility
   - Sprint (2055) - Should increase movement speed
   - Blade Flurry (2057) - Should enable cleave attacks
   - Shadow Dance (2060) - Should allow stealth abilities in combat
   - Vanish (2063) - Should provide instant stealth + threat drop
   - Vendetta (2061) - Should debuff target, not buff caster

2. **Missing Combo Point Tracking:**
   - Combo builders don't actually grant combo points
   - Finishers don't consume combo points
   - No combo point UI/display

3. **Stealth Not Implemented:**
   - Stealth (2050) doesn't hide player
   - Cheap Shot (2058) doesn't check for stealth
   - Ambush (2056) doesn't check for stealth
   - Shadow Dance (2060) doesn't enable stealth in combat
   - Vanish (2063) doesn't apply stealth

4. **Positional Damage Missing:**
   - Backstab (2049) doesn't check if behind target

5. **Utility Needs Work:**
   - Pick Pocket (2051) - Currently just applies AllStatsBuff
   - Gouge (2054) - Currently just applies AllStatsBuff (should incapacitate)
   - Kidney Shot (2053) - Currently just applies AllStatsBuff (should stun)

6. **Movement Abilities Missing:**
   - Death from Above (2062) - Doesn't actually leap to target
   - Sprint (2055) - Doesn't actually increase movement speed

### Effect Variety
- **All abilities use same effects:** 0x36D4 (visual), 0x1E5 (cast sound), 0x481 (hit sound)
- Need unique effects per ability type (stealth smoke, bleed, stun, etc.)

---

## Future State Proposals

**Date:** 2025-12-28
**Source:** Industry archetype standards (`Vystia/admin/ARCHETYPE_BALANCE_ANALYSIS.md`)

### Target Distribution
Based on **Burst/Utility** archetype standards:
- **Damage:** 5-6 abilities (31-38%) - ✅ Currently at 31%
- **Combo System:** Maintain builder/finisher model - ✅ Good
- **Stealth:** 3-4 abilities (19-25%) - ✅ Currently at 25%
- **Defense:** ADD 1-2 defensive cooldowns - ❌ Currently 0%
- **Mobility:** ADD 1-2 mobility abilities - ⚠️ Only Sprint
- **AoE:** ADD 1 AoE ability - ❌ Currently 0%

### Proposed New Abilities

#### Circle 2: Evasion (Defensive)
**Resource:** Medium (7 mana)
**Effect:** +50% dodge chance for 6 seconds
**Theme:** Agile rogue dodging attacks
**Purpose:** First defensive cooldown, skill-based mitigation
**Replace:** Pick Pocket (2051) - Move to Circle 1

#### Circle 3: Fan of Knives (AoE Damage)
**Resource:** Medium (10 mana)
**Effect:** 25-40 Physical damage to all enemies within 5 tiles
**Theme:** Throwing knives in all directions
**Purpose:** First AoE ability for multi-target scenarios
**Replace:** Gouge (2054) - Rework as single-target finisher

#### Circle 4: Cloak of Shadows (Defensive)
**Resource:** High (13 mana)
**Effect:** Immune to magic for 5 seconds
**Theme:** Shadow magic protection
**Purpose:** Counter for casters, major defensive cooldown
**Replace:** Shadow Dance (2060) - Consolidate with Vanish

### Ability Replacements/Updates

1. **Pick Pocket (2051)** → Replace with **Evasion**
   - Pick Pocket is too niche for combat
   - Need defensive ability in Circle 1-2

2. **Gouge (2054)** → Replace with **Fan of Knives**
   - Already have Kidney Shot for stuns
   - Need AoE damage in Circle 3

3. **Shadow Dance (2060)** → Replace with **Cloak of Shadows**
   - Shadow Dance overlaps with Vanish
   - Need magic immunity in Circle 4

4. **All Buffs** → Implement actual effects
   - Stealth: Apply actual invisibility
   - Sprint: Increase movement speed by 50%
   - Blade Flurry: Enable cleave on melee attacks
   - Vendetta: Debuff target for +20% damage taken

5. **Combo System** → Implement tracking
   - Add combo point display (1-5 points)
   - Builders grant 1 combo point
   - Finishers consume all points, scale with count

6. **Positional** → Implement facing checks
   - Backstab: Check if behind target, +50% damage
   - Ambush: Check if from stealth, +100% damage

### Implementation Priority

- **Phase 1 (Critical):** Implement combo point tracking and stealth mechanics
- **Phase 2 (Important):** Add defensive abilities (Evasion, Cloak of Shadows)
- **Phase 3 (Enhancement):** Add AoE (Fan of Knives), positional mechanics

---

## Testing Commands

```
[addability 2048    - Sinister Strike
[addability 2049    - Backstab
[addability 2050    - Stealth
[addability 2051    - Pick Pocket
[addability 2052    - Eviscerate
[addability 2053    - Kidney Shot
[addability 2054    - Gouge
[addability 2055    - Sprint
[addability 2056    - Ambush
[addability 2057    - Blade Flurry
[addability 2058    - Cheap Shot
[addability 2059    - Rupture
[addability 2060    - Shadow Dance
[addability 2061    - Vendetta
[addability 2062    - Death from Above
[addability 2063    - Vanish
```

---

**Last Updated:** 2025-12-28
**Status:** Documentation complete, implementation has placeholders that need proper mechanics
