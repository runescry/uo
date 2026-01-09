# Knight - Current State vs Future State

**Class:** Knight
**Ability IDs:** 2080-2095
**Archetype:** Tank
**Theme:** Stalwart defender with holy powers

---

## Current State

### Ability Distribution (16 abilities)
- **Damage:** 5 abilities (31.25%) - Shield Slam, Charge, Lance Strike, Judgment, Crusader Strike, Azure Tide
- **Utility:** 2 abilities (12.5%) - Challenge, Consecration (AoE)
- **Defense:** 8 abilities (50%) - Noble Shield, Rally, Divine Protection, Aura of Devotion, Righteous Fury, Shield of Faith, Guardian of Light, Last Defender
- **Mobility:** 1 ability (6.25%) - Charge (dual purpose)

### Implementation Status
**File Location:** `ServUO/Scripts/Custom/VystiaClasses/Abilities/Generated/Martial/KnightAbilities.cs`

All 16 abilities are implemented using the Vystia Class System v2.0 ability framework.

---

## Abilities by Circle

### Circle 1 (Mana Cost: 4)

| ID | Name | Type | Cost | Damage/Effect | Description |
|----|------|------|------|---------------|-------------|
| 2080 | Shield Slam | Damage | 4 | 10-16 Physical | Shield attack |
| 2081 | Charge | Damage | 4 | 15-22 Physical | Rush and knockdown |
| 2082 | Noble Shield | Buff | 4 | +10 Stats, 30s | Take damage for ally |
| 2083 | Rally | Buff | 4 | +10 Stats, 30s | Morale boost allies |

**Effect:** 0x36D4, Sound: 0x1E5, Impact: 0x481

### Circle 2 (Mana Cost: 7)

| ID | Name | Type | Cost | Damage/Effect | Description |
|----|------|------|------|---------------|-------------|
| 2084 | Challenge | Utility | 7 | +10 Stats, 30s | Force enemy to attack you (12 tiles) |
| 2085 | Consecration | AoE | 7 | 6-10 Physical, 4 tiles | Holy ground damage |
| 2086 | Lance Strike | Damage | 7 | 20-32 Physical | Mounted charge damage |
| 2087 | Divine Protection | Buff | 7 | +10 Stats, 30s | Reduce damage 50% |

**Effect:** 0x36D4, Sound: 0x1E5, Impact: 0x481

### Circle 3 (Mana Cost: 10)

| ID | Name | Type | Cost | Damage/Effect | Description |
|----|------|------|------|---------------|-------------|
| 2088 | Judgment | Damage | 10 | 25-38 Physical | Holy damage + slow |
| 2089 | Aura of Devotion | Buff | 10 | +10 Stats, 60s | +10 all resists allies |
| 2090 | Righteous Fury | Buff | 10 | +10 Stats, 60s | Generate threat on heals |
| 2091 | Shield of Faith | Buff | 10 | +10 Stats, 60s | Damage absorb shield |

**Effect:** 0x36D4, Sound: 0x1E5, Impact: 0x481

### Circle 4 (Mana Cost: 13)

| ID | Name | Type | Cost | Damage/Effect | Description |
|----|------|------|------|---------------|-------------|
| 2092 | Guardian of Light | Buff | 13 | +10 Stats, 60s | Reflect damage taken |
| 2093 | Crusader Strike | Damage | 13 | 40-60 Physical | Holy damage combo |
| 2094 | Last Defender | Buff | 13 | +10 Stats, 60s | Immune while allies alive |
| 2095 | Azure Tide | AoE | 13 | 45-70 Physical, 4 tiles | Massive water + holy AoE |

**Effect:** 0x36D4, Sound: 0x1E5, Impact: 0x481

---

## Abilities by Type

### Damage Abilities (6)
1. **Shield Slam** (2080) - Circle 1, 10-16 damage
2. **Charge** (2081) - Circle 1, 15-22 damage
3. **Lance Strike** (2086) - Circle 2, 20-32 damage
4. **Judgment** (2088) - Circle 3, 25-38 damage
5. **Crusader Strike** (2093) - Circle 4, 40-60 damage
6. **Azure Tide** (2095) - Circle 4, 45-70 AoE damage (4 tile radius)

### AoE Abilities (2)
1. **Consecration** (2085) - Circle 2, 6-10 damage, 4 tile radius
2. **Azure Tide** (2095) - Circle 4, 45-70 damage, 4 tile radius

### Buff/Defense Abilities (8)
1. **Noble Shield** (2082) - Circle 1, protect ally mechanic
2. **Rally** (2083) - Circle 1, morale boost
3. **Divine Protection** (2087) - Circle 2, 50% damage reduction
4. **Aura of Devotion** (2089) - Circle 3, +10 all resists
5. **Righteous Fury** (2090) - Circle 3, threat generation
6. **Shield of Faith** (2091) - Circle 3, damage absorb
7. **Guardian of Light** (2092) - Circle 4, damage reflection
8. **Last Defender** (2094) - Circle 4, immunity mechanic

### Utility/Control (1)
1. **Challenge** (2084) - Circle 2, taunt/threat (12 tile range)

---

## Core Mechanics

### Tank Role
- **High Defense:** 8 defensive/buff abilities (50% of kit)
- **Threat Generation:** Challenge, Righteous Fury for aggro control
- **Ally Protection:** Noble Shield, Rally, Aura of Devotion
- **Damage Absorption:** Divine Protection (50% reduction), Shield of Faith, Guardian of Light
- **Emergency Survival:** Last Defender (immunity mechanic)

### Holy Warrior Theme
- **Consecration:** Holy ground AoE
- **Judgment:** Holy damage with slow
- **Crusader Strike:** Holy damage combo
- **Azure Tide:** Water + holy themed ultimate

### Mounted Combat
- **Lance Strike:** Bonus damage on mounts
- **Charge:** Mobility + knockdown

---

## Strengths
- ✅ Strong defensive toolkit with 8 buff/protection abilities
- ✅ Good threat generation for tanking (Challenge, Righteous Fury)
- ✅ Scales well from Circle 1 to Circle 4
- ✅ Mix of single-target and AoE damage options
- ✅ Holy/water theme consistent throughout
- ✅ Ally support abilities (Rally, Aura of Devotion, Noble Shield)

## Weaknesses
- ⚠️ Limited mobility (only Charge provides movement)
- ⚠️ Placeholder buff values (+10 stats) need tuning
- ⚠️ All damage is Physical type (no elemental variety)
- ⚠️ Buff descriptions are vague on actual mechanics
- ⚠️ No resource system integration (no Honor/Devotion resource)
- ⚠️ Many abilities share identical visual/sound effects
- ⚠️ "Last Defender" immunity mechanic may be overpowered
- ⚠️ No self-healing abilities (reliant on others)

---

## Known Issues

### Placeholder Implementations
1. **All Buffs:** Use generic `VystiaBuffType.AllStatsBuff` with +10 value
   - Should have specific buff types (e.g., DamageReduction, ResistBonus, DamageReflection)
   - Actual values need balancing (50% reduction, +10 resists, etc.)

2. **Visual Effects:** All abilities use same effect (0x36D4, 0x1E5, 0x481)
   - Should have unique holy/water themed effects
   - Azure Tide should have distinctive water animation

3. **Targeting:** Challenge uses generic targeting, should have taunt mechanic
   - Need integration with threat/aggro system
   - Noble Shield needs ally targeting validation

4. **Conditional Logic:**
   - Lance Strike: No mounted check implemented
   - Last Defender: No ally proximity check
   - Righteous Fury: No healing trigger hook

5. **Resource System:** No Honor/Devotion resource tracking
   - Could add resource generation on taking damage
   - Resource spending for enhanced abilities

---

## Future State Proposals

**Date:** 2025-12-28
**Source:** Industry archetype standards, Tank role analysis

### Target Distribution
Based on **Tank** archetype standards:
- **Damage:** 4-5 abilities (25-30%) - Current: 31% ✅
- **Utility:** 2-3 abilities (12-18%) - Current: 12.5% ✅
- **Defense:** 7-9 abilities (45-55%) - Current: 50% ✅
- **Mobility:** 2-3 abilities (12-18%) - Current: 6.25% ⚠️

### Recommended Improvements

#### Phase 1 (Critical) - Buff System Implementation
1. **Replace AllStatsBuff** with specific buff types:
   - Divine Protection → DamageReduction (50%)
   - Aura of Devotion → ResistBonus (+10 all resists)
   - Shield of Faith → DamageAbsorb (fixed amount shield)
   - Guardian of Light → DamageReflection (% of damage taken)

2. **Add Buff Stacking Rules:**
   - Defensive buffs don't stack with each other
   - Offensive buffs (Rally, Righteous Fury) stack

#### Phase 2 (Important) - Mobility & Mechanics
1. **Add Second Mobility Ability** (replace one buff):
   - **Intercept** (Circle 2): Dash to ally, take next hit for them
   - OR **Divine Leap** (Circle 3): Jump to location, stun nearby enemies

2. **Implement Conditional Logic:**
   - Lance Strike: Check if mounted, bonus damage if true
   - Challenge: Actual taunt/threat mechanic
   - Last Defender: Check ally count, grant immunity if > 0

3. **Add Resource System:**
   - **Honor Resource:** 0-100, generates on taking damage
   - **Spend Honor:** Enhanced ability effects at 50+ Honor
   - **100 Honor Dump:** Massive shield or damage reflection

#### Phase 3 (Enhancement) - Visual & Balance
1. **Unique Visual Effects:**
   - Holy abilities: Golden/white particles
   - Water abilities (Azure Tide): Blue water effects
   - Shield abilities: Glowing shield visuals

2. **Damage Type Variety:**
   - Judgment, Crusader Strike → 50% Physical / 50% Energy (holy)
   - Azure Tide → 50% Physical / 50% Cold (water)

3. **Add Self-Healing:**
   - Replace one buff with healing ability
   - OR add healing to Righteous Fury (heal on crit)

---

## Testing Checklist

### Basic Functionality
- [ ] All 16 abilities cast without errors
- [ ] Mana costs deduct correctly (4/7/10/13)
- [ ] Damage ranges match specification
- [ ] Buffs apply and expire correctly

### Combat Scenarios
- [ ] Shield Slam: Interrupts casting
- [ ] Charge: Knocks down target, provides gap close
- [ ] Challenge: Forces enemy to attack caster
- [ ] Lance Strike: Bonus damage when mounted
- [ ] Consecration: Damages all enemies in area
- [ ] Divine Protection: Reduces incoming damage 50%
- [ ] Judgment: Applies slow debuff
- [ ] Last Defender: Grants immunity when allies present
- [ ] Azure Tide: Large AoE damage

### Group Content
- [ ] Noble Shield: Redirects damage to Knight
- [ ] Rally: Boosts ally morale/stats
- [ ] Aura of Devotion: Party-wide resist buff
- [ ] Righteous Fury: Generates threat when healing occurs

---

## File Locations

**Implementation:** `ServUO/Scripts/Custom/VystiaClasses/Abilities/Generated/Martial/KnightAbilities.cs`
**Class Definition:** `ServUO/Scripts/Custom/VystiaClasses/Classes/KnightClass.cs`
**Documentation:** `Vystia/design/martial/Knight.md`

---

**Last Updated:** 2025-12-28
**Status:** Auto-generated abilities implemented, needs Phase 1-3 enhancements for production readiness
