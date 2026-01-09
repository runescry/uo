# BountyHunter - Ability Documentation

**Class:** BountyHunter
**Ability IDs:** 2128-2143 (16 abilities)
**Archetype:** Hunter/Debuff
**Theme:** Relentless tracker who marks targets and deals massive damage to hunted prey
**Implementation Status:** ✅ All 16 abilities implemented

**File Location:** `D:\UO\ServUO\Scripts\Custom\VystiaClasses\Abilities\Generated\Martial\BountyHunterAbilities.cs`

---

## Ability Overview

### Distribution by Type
- **Direct Damage:** 5 abilities (31%)
- **Buffs/Utility:** 7 abilities (44%)
- **AoE/Traps:** 3 abilities (19%)
- **Crowd Control:** 1 ability (6%)

### Distribution by Circle
- **Circle 1:** 4 abilities (marks, tracking, damage, traps)
- **Circle 2:** 4 abilities (CC, anti-undead, mobility, traps)
- **Circle 3:** 4 abilities (tracking, damage, debuff, traps)
- **Circle 4:** 4 abilities (execution damage, massive buffs, finisher)

---

## Circle 1 Abilities (Foundation)

| ID | Name | Type | Mana | Range | Target | Damage/Effect | Duration | Description |
|----|------|------|------|-------|--------|---------------|----------|-------------|
| 2128 | Mark Target | Buff | 4 | 12 | Single | Buff +10 | 30s | Mark target for bonus damage tracking |
| 2129 | Quick Draw | Damage | 4 | - | Single | 14-20 Physical | - | Fast ranged attack for quick bursts |
| 2130 | Tracking | Utility | 4 | 12 | Single | Buff +10 | 30s | Track marked target anywhere |
| 2131 | Caltrops | AoE | 4 | - | AoE (r4) | 0-0 Physical | - | Drop caltrops to slow enemies in area |

**Circle 1 Purpose:** Mark targets, quick damage, basic tracking, area denial

---

## Circle 2 Abilities (Control & Mobility)

| ID | Name | Type | Mana | Range | Target | Damage/Effect | Duration | Description |
|----|------|------|------|-------|--------|---------------|----------|-------------|
| 2132 | Bola | CC | 7 | 12 | Single | Buff +10 | 30s | Root target in place for 4 seconds |
| 2133 | Silver Strike | Damage | 7 | - | Single | 20-30 Physical | - | Damage that ignores undead resistances |
| 2134 | Net Trap | Utility | 7 | 12 | Single | Buff +10 | 30s | Snare first enemy that triggers trap |
| 2135 | Pursuit | Buff | 7 | - | Self | Buff +10 | 30s | Gain +25% movement speed vs marked targets |

**Circle 2 Purpose:** Crowd control, anti-undead damage, chase mechanics, trap setting

---

## Circle 3 Abilities (Advanced Tactics)

| ID | Name | Type | Mana | Range | Target | Damage/Effect | Duration | Description |
|----|------|------|------|-------|--------|---------------|----------|-------------|
| 2136 | Hunter's Quarry | Buff | 10 | - | Self | Buff +10 | 60s | Track marked target anywhere on map |
| 2137 | Explosive Trap | AoE | 10 | - | AoE (r4) | 22-35 Physical | - | Fire trap that explodes on trigger |
| 2138 | Venomous Blade | Damage | 10 | - | Single | 18-28 Physical | - | Weapon attack adds poison DoT |
| 2139 | Interrogation | Debuff | 10 | 12 | Single | Buff +10 | 30s | Mark reveals enemy weaknesses (debuff) |

**Circle 3 Purpose:** Extended tracking, high trap damage, poison DoT, enemy debuffs

---

## Circle 4 Abilities (Execution Phase)

| ID | Name | Type | Mana | Range | Target | Damage/Effect | Duration | Description |
|----|------|------|------|-------|--------|---------------|----------|-------------|
| 2140 | Execution Contract | Damage | 13 | - | Single | 70-110 Physical | - | Massive damage to marked targets |
| 2141 | Dead or Alive | Buff | 13 | - | Self | Buff +10 | 60s | Gain +100% damage against marked targets |
| 2142 | Monster Slayer | Buff | 13 | - | Self | Buff +10 | 60s | Gain +50% damage against all creatures |
| 2143 | Final Tally | Damage | 13 | - | Single | 55-85 Physical | - | Consume pursuit stacks for burst damage |

**Circle 4 Purpose:** Ultimate execution power, massive damage multipliers, stack consumption finisher

---

## Ability Details

### Mark Target (2128) - Circle 1
**Type:** Buff | **Mana:** 4 | **Range:** 12 tiles
**Effect:** Mark target with buff (+10 for 30s)
**Visual:** Effect 0x36D4, Sound 0x1E5, Hue 0x481
**Purpose:** Foundation ability - marks enemy for all other tracking/damage bonuses

### Quick Draw (2129) - Circle 1
**Type:** Damage | **Mana:** 4
**Damage:** 14-20 Physical
**Visual:** Effect 0x36D4, Sound 0x1E5, Hue 0x481
**Purpose:** Fast, low-cost ranged attack for sustained DPS

### Tracking (2130) - Circle 1
**Type:** Utility | **Mana:** 4 | **Range:** 12 tiles
**Effect:** Buff (+10 for 30s) - enables tracking marked target
**Visual:** Effect 0x36D4, Sound 0x1E5, Hue 0x481
**Purpose:** Basic tracking - reveals marked target's location

### Caltrops (2131) - Circle 1
**Type:** AoE | **Mana:** 4 | **AoE:** 4 tile radius
**Damage:** 0-0 Physical (placeholder - needs slow effect)
**Visual:** Effect 0x36D4, Sound 0x1E5, Hue 0x481
**Purpose:** Area denial - slows enemies in area

### Bola (2132) - Circle 2
**Type:** Crowd Control | **Mana:** 7 | **Range:** 12 tiles
**Effect:** Buff (+10 for 30s) - intended as 4s root
**Visual:** Effect 0x36D4, Sound 0x1E5, Hue 0x481
**Purpose:** Root enemy in place - prevents escape

### Silver Strike (2133) - Circle 2
**Type:** Damage | **Mana:** 7
**Damage:** 20-30 Physical (bypasses undead resistances)
**Visual:** Effect 0x36D4, Sound 0x1E5, Hue 0x481
**Purpose:** Anti-undead specialist ability

### Net Trap (2134) - Circle 2
**Type:** Utility | **Mana:** 7 | **Range:** 12 tiles
**Effect:** Buff (+10 for 30s) - snare first enemy trigger
**Visual:** Effect 0x36D4, Sound 0x1E5, Hue 0x481
**Purpose:** Trap that snares first enemy to trigger it

### Pursuit (2135) - Circle 2
**Type:** Buff | **Mana:** 7
**Effect:** AllStatsBuff (+10 for 30s) - intended as +25% speed vs marked
**Purpose:** Chase mechanic - catch fleeing marked targets

### Hunter's Quarry (2136) - Circle 3
**Type:** Buff | **Mana:** 10
**Effect:** AllStatsBuff (+10 for 60s) - track anywhere on map
**Purpose:** Ultimate tracking - no escape for marked prey

### Explosive Trap (2137) - Circle 3
**Type:** AoE | **Mana:** 10 | **AoE:** 4 tile radius
**Damage:** 22-35 Physical
**Visual:** Effect 0x36D4, Sound 0x1E5, Hue 0x481
**Purpose:** High-damage fire trap for area control

### Venomous Blade (2138) - Circle 3
**Type:** Damage | **Mana:** 10
**Damage:** 18-28 Physical + poison DoT (intended)
**Visual:** Effect 0x36D4, Sound 0x1E5, Hue 0x481
**Purpose:** Weapon attack with poison damage over time

### Interrogation (2139) - Circle 3
**Type:** Debuff | **Mana:** 10 | **Range:** 12 tiles
**Effect:** Buff (+10 for 30s) - reveals weaknesses (intended as debuff)
**Visual:** Effect 0x36D4, Sound 0x1E5, Hue 0x481
**Purpose:** Mark reveals enemy vulnerabilities - increases damage taken

### Execution Contract (2140) - Circle 4
**Type:** Damage | **Mana:** 13
**Damage:** 70-110 Physical (massive damage to marked)
**Visual:** Effect 0x36D4, Sound 0x1E5, Hue 0x481
**Purpose:** Ultimate single-target execute for marked prey

### Dead or Alive (2141) - Circle 4
**Type:** Buff | **Mana:** 13
**Effect:** AllStatsBuff (+10 for 60s) - intended as +100% damage vs marked
**Purpose:** Double damage against marked targets

### Monster Slayer (2142) - Circle 4
**Type:** Buff | **Mana:** 13
**Effect:** AllStatsBuff (+10 for 60s) - intended as +50% vs creatures
**Purpose:** Anti-PvE specialization - bonus damage to monsters

### Final Tally (2143) - Circle 4
**Type:** Damage | **Mana:** 13
**Damage:** 55-85 Physical (consumes pursuit stacks)
**Visual:** Effect 0x36D4, Sound 0x1E5, Hue 0x481
**Purpose:** Burst finisher that consumes accumulated pursuit stacks

---

## Playstyle & Rotation

### Opening (Mark & Track)
1. **Mark Target** (2128) - Mark primary target
2. **Tracking** (2130) - Enable location tracking
3. **Quick Draw** (2129) - Start damage

### Mid-Combat (Control & Pressure)
1. **Bola** (2132) - Root to prevent escape
2. **Pursuit** (2135) - Speed buff for chase
3. **Caltrops** (2131) - Area denial if grouped
4. **Silver Strike** (2133) - If target is undead

### Advanced (Traps & DoT)
1. **Net Trap** (2134) - Pre-place for ambush
2. **Explosive Trap** (2137) - High AoE damage
3. **Venomous Blade** (2138) - Add poison DoT
4. **Interrogation** (2139) - Debuff for team

### Execute Phase (Circle 4)
1. **Dead or Alive** (2141) - +100% damage buff
2. **Monster Slayer** (2142) - If PvE
3. **Execution Contract** (2140) - Massive marked damage
4. **Final Tally** (2143) - Consume stacks for burst

---

## Known Issues & Placeholder Implementations

### Buff System Placeholders
Most buffs use generic `VystiaBuffType.AllStatsBuff` with placeholder values:
- **Issue:** Buffs apply +10 to all stats instead of specific effects
- **Affected:** Mark Target, Tracking, Bola, Net Trap, Pursuit, Hunter's Quarry, Interrogation, Dead or Alive, Monster Slayer
- **Fix Needed:** Implement custom buff types for:
  - Mark tracking (reveal location)
  - Root/snare CC (Bola, Net Trap)
  - Movement speed (Pursuit +25%)
  - Damage multipliers (Dead or Alive +100%, Monster Slayer +50%)
  - Debuff (Interrogation weakness reveal)

### Missing Mechanics
1. **Caltrops (2131):** AoE does 0 damage - needs slow/snare implementation
2. **Venomous Blade (2138):** No poison DoT applied - needs DoT system
3. **Final Tally (2143):** Doesn't consume pursuit stacks - needs stack tracking

### Visual/Sound Effects
All abilities use same placeholder effects:
- Effect: 0x36D4 (generic)
- Sound: 0x1E5 (generic)
- Hue: 0x481 (generic)

**Improvement Needed:** Unique effects for:
- Tracking/marking abilities (different visual)
- Traps (explosion, net, caltrops visuals)
- Execution abilities (more dramatic effects)

---

## Strengths (As-Is)

✅ **High Single-Target Damage:** Execution Contract (70-110), Final Tally (55-85)
✅ **Strong Marking System:** Multiple mark/track abilities for coordination
✅ **Trap Variety:** Caltrops, Net Trap, Explosive Trap for different scenarios
✅ **Anti-Undead Specialist:** Silver Strike bypasses undead resistances
✅ **Stacking Damage:** Pursuit → Final Tally combo potential
✅ **Long Buff Durations:** Circle 4 buffs last 60 seconds

---

## Weaknesses (Current Implementation)

⚠️ **No True Tracking:** Tracking/Hunter's Quarry don't reveal target location
⚠️ **No Movement Speed:** Pursuit doesn't actually increase speed
⚠️ **No Crowd Control:** Bola doesn't root - just applies generic buff
⚠️ **Placeholder Buffs:** Most buffs don't match intended effects
⚠️ **No Poison DoT:** Venomous Blade missing poison application
⚠️ **No Stack System:** Final Tally can't consume pursuit stacks (not tracked)
⚠️ **Generic Visuals:** All abilities share same effects/sounds

---

## Integration with Class System

### Resource Requirements
- **Primary Resource:** Mana (all abilities)
- **Circle 1:** 4 mana (low cost)
- **Circle 2:** 7 mana (moderate)
- **Circle 3-4:** 10-13 mana (high cost)

### Class Synergies
- **BountyHunter.cs:** Should provide starting `BountyLedger` item
- **Mark System:** Requires target tracking attachment/debuff system
- **Pursuit Stacks:** Needs secondary resource tracking (like combo points)

### Commands for Testing
```
[ability 2128          # Test Mark Target
[ability 2140          # Test Execution Contract
[buff                  # View active buffs (check mark status)
```

---

## Future Enhancement Recommendations

### Phase 1: Core Mechanics (Critical)
1. Implement true mark/tracking system (reveal target location)
2. Add root/snare CC effects (Bola, Net Trap, Caltrops)
3. Implement movement speed buff (Pursuit)
4. Add damage multiplier buffs (Dead or Alive, Monster Slayer)

### Phase 2: Advanced Features (Important)
1. Add pursuit stack resource system
2. Implement poison DoT (Venomous Blade)
3. Add debuff system (Interrogation)
4. Create trap placement mechanics

### Phase 3: Polish (Enhancement)
1. Unique visual effects per ability type
2. Distinct sound effects for mark/track/execute
3. Trap visual indicators on ground
4. Mark visual on target (overhead icon)

---

## References

**Implementation File:**
`D:\UO\ServUO\Scripts\Custom\VystiaClasses\Abilities\Generated\Martial\BountyHunterAbilities.cs`

**Class Definition:**
`D:\UO\ServUO\Scripts\Custom\VystiaClasses\Classes\BountyHunter.cs`

**Ability Framework:**
`D:\UO\ServUO\Scripts\Custom\VystiaClasses\Core\AbilityDefinition.cs`

**Quick Reference:**
`D:\UO\Vystia\reference\CLASSES.md` (BountyHunter stats/skills)

---

**Last Updated:** 2025-12-28
**Status:** Documentation complete - matches implementation
**Implementation:** 16/16 abilities registered (100%)
**Functionality:** Placeholder buffs/effects - needs system integration
