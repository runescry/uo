# Fighter - Weapon Master

**Class:** Fighter
**Ability IDs:** 2000-2015
**Archetype:** Melee DPS / Tank
**Theme:** Weapon mastery, stance dancing, battlefield control
**File Location:** `ServUO/Scripts/Custom/VystiaClasses/Abilities/Generated/Martial/FighterAbilities.cs`

---

## Overview

The Fighter is a versatile melee combatant specializing in weapon mastery and adaptive stances. With a balanced mix of damage, defense, and utility abilities, Fighters can switch between aggressive offense and ironclad defense to dominate any battlefield situation.

**Core Mechanics:**
- **Stance System:** Defensive Stance, Ironclad Stance, Avatar of War
- **Weapon Specialization:** Weapon Mastery enhances damage output
- **Battlefield Control:** AoE abilities and crowd control (stuns, disarm)
- **Execute Mechanic:** Bonus damage vs low HP targets

---

## Ability Breakdown (16 Total)

### By Type
- **Single-Target Damage:** 5 abilities (31%)
- **AoE Damage:** 2 abilities (13%)
- **Buffs/Stances:** 6 abilities (38%)
- **Utility/CC:** 3 abilities (19%)

### By Circle
- **Circle 1:** 4 abilities (basic attacks, stance, buff)
- **Circle 2:** 4 abilities (AoE, mobility, CC, defense)
- **Circle 3:** 4 abilities (mastery buffs, execute, rally)
- **Circle 4:** 4 abilities (ultimate abilities, transforms)

---

## Circle 1 Abilities (Mana Cost: 4)

### Single-Target Damage

| ID | Name | Type | Cost | Damage | Range | Description |
|----|------|------|------|--------|-------|-------------|
| 2000 | **Power Strike** | Physical Damage | 4 | 15-25 | 12 | High-damage single target attack |

**Effects:**
- Damage Type: Physical
- Impact: Effect 0x36D4, Sound 0x1E5, Anim 0x481

---

| ID | Name | Type | Cost | Damage | Range | Description |
|----|------|------|------|--------|-------|-------------|
| 2001 | **Shield Bash** | Physical Damage + CC | 4 | 8-12 | 12 | Stun target for 2s |

**Effects:**
- Damage Type: Physical
- CC: 2s stun (placeholder - stun not implemented)
- Impact: Effect 0x36D4, Sound 0x1E5, Anim 0x481

**Known Issues:**
- Stun effect not implemented in current version

---

### Buff / Stance

| ID | Name | Type | Cost | Effect | Duration | Range | Description |
|----|------|------|------|--------|----------|-------|-------------|
| 2002 | **Battle Shout** | Buff (Ally) | 4 | +10 All Stats | 30s | 12 | Buff allies with +10 STR |

**Effects:**
- Buff Type: AllStatsBuff
- Magnitude: 10
- Target: Single ally
- Impact: Effect 0x36D4, Sound 0x1E5, Anim 0x481

**Known Issues:**
- Description says "+10 STR" but applies AllStatsBuff (STR/DEX/INT)

---

| ID | Name | Type | Cost | Effect | Duration | Description |
|----|------|------|------|--------|----------|-------------|
| 2003 | **Defensive Stance** | Buff (Self) | 4 | +20 All Resists | 30s | +20 all resists for 30s |

**Effects:**
- Buff Type: AllStatsBuff (placeholder - should be resist buff)
- Magnitude: 10
- Duration: 30s

**Known Issues:**
- Uses AllStatsBuff instead of resist buff type
- Description says "+20 resists" but magnitude is 10

---

## Circle 2 Abilities (Mana Cost: 7)

### AoE Damage

| ID | Name | Type | Cost | Damage | AoE | Range | Description |
|----|------|------|------|--------|-----|-------|-------------|
| 2004 | **Cleave** | Physical AoE | 7 | 12-20 | 4 | 12 | Hit all enemies in front |

**Effects:**
- Damage Type: Physical
- AoE: 4-tile cone/arc
- Impact: Effect 0x36D4, Sound 0x1E5, Anim 0x481

---

### Buff / Defense

| ID | Name | Type | Cost | Effect | Duration | Description |
|----|------|------|------|--------|----------|-------------|
| 2005 | **Shield Wall** | Buff (Self) | 7 | Block 50% dmg | 10s | Block 50% damage for 10s |

**Effects:**
- Buff Type: AllStatsBuff (placeholder)
- Magnitude: 10
- Duration: 30s

**Known Issues:**
- Uses AllStatsBuff instead of damage reduction buff
- Description says "10s" but duration is 30s
- Description says "50% block" but magnitude is 10

---

### Utility / CC

| ID | Name | Type | Cost | Effect | Duration | Range | Description |
|----|------|------|------|--------|----------|-------|-------------|
| 2006 | **Disarm** | Debuff (CC) | 7 | Disarm | 5s | 12 | Disarm target for 5s |

**Effects:**
- Buff Type: AllStatsBuff (placeholder - should be disarm debuff)
- Magnitude: 10
- Duration: 30s
- Impact: Effect 0x36D4, Sound 0x1E5, Anim 0x481

**Known Issues:**
- Uses AllStatsBuff instead of disarm debuff
- Description says "5s" but duration is 30s
- Disarm mechanic not implemented

---

### Mobility + Damage

| ID | Name | Type | Cost | Damage | Range | Description |
|----|------|------|------|--------|-------|-------------|
| 2007 | **Charge** | Physical Damage + Mobility | 7 | 20-30 | 12 | Rush to target and stun |

**Effects:**
- Damage Type: Physical
- Impact: Effect 0x36D4, Sound 0x1E5, Anim 0x481

**Known Issues:**
- Mobility/teleport mechanic not implemented
- Stun effect not implemented

---

## Circle 3 Abilities (Mana Cost: 10)

### Buff / Damage

| ID | Name | Type | Cost | Effect | Duration | Description |
|----|------|------|------|--------|----------|-------------|
| 2008 | **Weapon Mastery** | Buff (Self) | 10 | +25% damage | 20s | +25% damage for 20s |

**Effects:**
- Buff Type: AllStatsBuff (placeholder)
- Magnitude: 10
- Duration: 60s

**Known Issues:**
- Uses AllStatsBuff instead of damage% buff
- Description says "20s" but duration is 60s
- Description says "+25%" but magnitude is 10

---

### Buff / Defense

| ID | Name | Type | Cost | Effect | Duration | Description |
|----|------|------|------|--------|----------|-------------|
| 2009 | **Ironclad Stance** | Buff (Self) | 10 | -30% dmg taken | 60s | Reduce damage taken 30% |

**Effects:**
- Buff Type: AllStatsBuff (placeholder)
- Magnitude: 10
- Duration: 60s

**Known Issues:**
- Uses AllStatsBuff instead of damage reduction buff
- Description says "30%" but magnitude is 10

---

### Execute Damage

| ID | Name | Type | Cost | Damage | Range | Description |
|----|------|------|------|--------|-------|-------------|
| 2010 | **Execute** | Physical Damage (Execute) | 10 | 40-60 | 12 | Bonus damage if target <30% HP |

**Effects:**
- Damage Type: Physical
- Execute Threshold: <30% HP
- Impact: Effect 0x36D4, Sound 0x1E5, Anim 0x481

**Known Issues:**
- Execute bonus damage mechanic not implemented
- Currently just flat 40-60 damage

---

### Buff / Healing (Group)

| ID | Name | Type | Cost | Effect | Duration | Description |
|----|------|------|------|--------|----------|-------------|
| 2011 | **Rallying Cry** | Buff (AoE) + Heal | 10 | Heal + buff | 60s | Heal and buff nearby allies |

**Effects:**
- Buff Type: AllStatsBuff
- Magnitude: 10
- Duration: 60s

**Known Issues:**
- Healing mechanic not implemented
- No AoE range specified
- Buff values unspecified

---

## Circle 4 Abilities (Mana Cost: 13)

### AoE Damage

| ID | Name | Type | Cost | Damage | AoE | Range | Description |
|----|------|------|------|--------|-----|-------|-------------|
| 2012 | **Bladestorm** | Physical AoE | 13 | 25-40 | 4 | 12 | Spin dealing damage to all nearby |

**Effects:**
- Damage Type: Physical
- AoE: 4-tile radius
- Impact: Effect 0x36D4, Sound 0x1E5, Anim 0x481

---

### Transform / Buff

| ID | Name | Type | Cost | Effect | Duration | Range | Description |
|----|------|------|------|--------|----------|-------|-------------|
| 2013 | **Avatar of War** | Transform (Self) | 13 | War Avatar | 30s | 12 | Transform into war avatar |

**Effects:**
- Buff Type: AllStatsBuff (placeholder)
- Magnitude: 10
- Duration: 30s
- Impact: Effect 0x36D4, Sound 0x1E5, Anim 0x481

**Known Issues:**
- Transform mechanic not implemented
- No stat bonuses specified
- Uses generic buff instead of transform

---

### Damage + Debuff

| ID | Name | Type | Cost | Damage | Range | Description |
|----|------|------|------|--------|-------|-------------|
| 2014 | **Mortal Strike** | Physical Damage + Debuff | 13 | 35-50 | 12 | Reduce healing received 50% |

**Effects:**
- Damage Type: Physical
- Debuff: -50% healing received
- Impact: Effect 0x36D4, Sound 0x1E5, Anim 0x481

**Known Issues:**
- Healing reduction debuff not implemented

---

### Buff / Defense (Ultimate)

| ID | Name | Type | Cost | Effect | Duration | Description |
|----|------|------|------|--------|----------|-------------|
| 2015 | **Last Stand** | Buff (Self) | 13 | Invulnerable | 8s | Cannot die for 8s |

**Effects:**
- Buff Type: AllStatsBuff (placeholder)
- Magnitude: 10
- Duration: 60s

**Known Issues:**
- Invulnerability mechanic not implemented
- Description says "8s" but duration is 60s
- Uses generic buff instead of invulnerability

---

## Known Issues & Placeholders

### Critical (Mechanics Not Implemented)
1. **Crowd Control:** Shield Bash stun, Charge stun, Disarm effect
2. **Execute Mechanic:** Execute bonus damage vs <30% HP targets
3. **Mobility:** Charge teleport/rush mechanic
4. **Transform:** Avatar of War transform system
5. **Invulnerability:** Last Stand cannot-die mechanic
6. **Healing Reduction:** Mortal Strike healing debuff
7. **Healing:** Rallying Cry heal component

### Medium (Buff Types Wrong)
8. **Resist Buffs:** Defensive Stance uses AllStatsBuff instead of resist buff
9. **Damage Reduction:** Shield Wall, Ironclad Stance use AllStatsBuff
10. **Damage Increase:** Weapon Mastery uses AllStatsBuff instead of damage% buff

### Low (Duration/Value Mismatches)
11. **Duration Mismatches:** Shield Wall (10s vs 30s), Weapon Mastery (20s vs 60s), Last Stand (8s vs 60s)
12. **Value Mismatches:** Defensive Stance (+20 vs 10), Shield Wall (50% vs 10), Weapon Mastery (25% vs 10), Ironclad Stance (30% vs 10)
13. **Description Errors:** Battle Shout says "+10 STR" but buffs all stats

### Visual/Effect Issues
14. **Generic Effects:** All abilities use same effect (0x36D4), sound (0x1E5), animation (0x481)
15. **No Unique Visuals:** Each ability should have distinct visual/sound effects

---

## Strengths

✅ **Versatile Toolkit:** Mix of damage, defense, utility
✅ **Stance System:** Adaptable to different combat situations
✅ **Strong AoE:** Cleave and Bladestorm for multi-target damage
✅ **Execute Mechanic:** Designed to finish low-HP targets
✅ **Group Utility:** Battle Shout, Rallying Cry support allies

---

## Weaknesses

⚠️ **Placeholder Implementations:** Most buffs use generic AllStatsBuff
⚠️ **Missing Mechanics:** CC, execute bonus, mobility, transforms not implemented
⚠️ **Generic Effects:** All abilities share same visual/sound effects
⚠️ **Duration/Value Bugs:** Many abilities have mismatched values vs descriptions
⚠️ **Limited True Defense:** Defensive abilities don't actually reduce damage yet

---

## Recommended Fixes (Priority Order)

### Phase 1 - Critical Mechanics
1. Implement CC system (stuns, disarm)
2. Implement damage reduction buffs (Shield Wall, Ironclad Stance)
3. Fix Charge mobility mechanic
4. Implement Execute bonus damage calculation

### Phase 2 - Buff System
5. Create damage% buff type for Weapon Mastery
6. Create resist buff type for Defensive Stance
7. Implement healing reduction debuff for Mortal Strike
8. Fix all duration/magnitude mismatches

### Phase 3 - Advanced Mechanics
9. Implement Avatar of War transform system
10. Implement Last Stand invulnerability
11. Implement Rallying Cry healing component
12. Create unique visual/sound effects for each ability

---

## File Locations

**Ability Definitions:**
`ServUO/Scripts/Custom/VystiaClasses/Abilities/Generated/Martial/FighterAbilities.cs`

**Class Definition:**
`ServUO/Scripts/Custom/VystiaClasses/Classes/Fighter.cs`

**Ability Framework:**
`ServUO/Scripts/Custom/VystiaClasses/Systems/AbilityDefinition.cs`
`ServUO/Scripts/Custom/VystiaClasses/Systems/AbilityExecutor.cs`

**Buff System:**
`ServUO/Scripts/Custom/VystiaClasses/Systems/VystiaBuffSystem.cs`

---

**Last Updated:** 2025-12-28
**Status:** Generated implementation (placeholders present, core systems functional)
