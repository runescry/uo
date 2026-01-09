# Barbarian - Implementation Documentation

**Class:** Barbarian
**Ability IDs:** 2016-2031 (16 abilities)
**Archetype:** Burst Melee
**Theme:** Rage-fueled berserker from Frosthold
**Primary Resource:** Fury
**File Location:** `ServUO/Scripts/Custom/VystiaClasses/Abilities/Generated/Martial/BarbarianAbilities.cs`

---

## Overview

The Barbarian is a frost-themed melee berserker specializing in high-damage burst attacks, area-of-effect crowd control, and self-buffing rage mechanics. Abilities scale across 4 circles with increasing mana costs and power.

### Ability Distribution (16 abilities)
- **Direct Damage:** 4 abilities (25%) - Single-target and chain attacks
- **AoE Damage:** 4 abilities (25%) - Whirlwind, slams, and environmental effects
- **Buffs:** 7 abilities (44%) - Rage states, resistance, attack speed
- **Crowd Control:** 1 ability (6%) - Fear effect

### Circle Distribution
- **Circle 1:** 4 abilities (4 mana cost)
- **Circle 2:** 4 abilities (7 mana cost)
- **Circle 3:** 4 abilities (10 mana cost)
- **Circle 4:** 4 abilities (13 mana cost)

---

## Abilities by Circle

### Circle 1 Abilities (4 mana)

#### Reckless Strike

| Property | Value |
|----------|-------|
| **File** | BarbarianAbilities.cs (line 16-19) |
| **Ability ID** | 2016 |
| **Circle** | 1 |
| **Type** | Direct Damage |
| **Mana Cost** | 4 |
| **Damage** | 18-28 Physical |
| **Target** | Single Target |
| **Range** | 12 tiles |
| **Description** | High damage, costs HP |
| **Effect ID** | 0x36D4 |
| **Sound ID** | 0x1E5 |
| **Hit Sound** | 0x481 |

High-risk, high-reward opener. Trades health for increased damage output.

---

#### Frost Fury

| Property | Value |
|----------|-------|
| **File** | BarbarianAbilities.cs (line 21-24) |
| **Ability ID** | 2017 |
| **Circle** | 1 |
| **Type** | Direct Damage |
| **Mana Cost** | 4 |
| **Damage** | 15-22 Physical |
| **Target** | Single Target |
| **Range** | 12 tiles |
| **Description** | Cold damage melee attack |
| **Effect ID** | 0x36D4 |
| **Sound ID** | 0x1E5 |
| **Hit Sound** | 0x481 |

Frost-infused melee strike. Lower damage than Reckless Strike but no HP cost.

---

#### War Cry

| Property | Value |
|----------|-------|
| **File** | BarbarianAbilities.cs (line 26-35) |
| **Ability ID** | 2018 |
| **Circle** | 1 |
| **Type** | Crowd Control / Buff |
| **Mana Cost** | 4 |
| **Target** | Single Target |
| **Range** | 12 tiles |
| **Buff Type** | AllStatsBuff |
| **Buff Magnitude** | 10 |
| **Buff Duration** | 30 seconds |
| **Description** | Fear nearby enemies |
| **Effect ID** | 0x36D4 |
| **Sound ID** | 0x1E5 |
| **Hit Sound** | 0x481 |

Intimidating shout that fears enemies and buffs the barbarian's stats.

---

#### Thick Skin

| Property | Value |
|----------|-------|
| **File** | BarbarianAbilities.cs (line 37-39) |
| **Ability ID** | 2019 |
| **Circle** | 1 |
| **Type** | Buff |
| **Mana Cost** | 4 |
| **Target** | Self |
| **Buff Type** | AllStatsBuff |
| **Buff Magnitude** | 10 |
| **Buff Duration** | 30 seconds |
| **Description** | +15 cold resist |
| **Effect** | None specified |

Passive damage reduction, particularly effective against cold damage.

---

### Circle 2 Abilities (7 mana)

#### Whirlwind

| Property | Value |
|----------|-------|
| **File** | BarbarianAbilities.cs (line 41-44) |
| **Ability ID** | 2020 |
| **Circle** | 2 |
| **Type** | AoE Damage |
| **Mana Cost** | 7 |
| **Damage** | 14-22 Physical |
| **AoE Range** | 4 tiles |
| **Target** | Area of Effect |
| **Range** | 7 tiles |
| **Description** | Spin attack all nearby |
| **Effect ID** | 0x36D4 |
| **Sound ID** | 0x1E5 |
| **Hit Sound** | 0x481 |

Spinning melee attack hitting all enemies in 4-tile radius.

---

#### Berserker Rage

| Property | Value |
|----------|-------|
| **File** | BarbarianAbilities.cs (line 46-48) |
| **Ability ID** | 2021 |
| **Circle** | 2 |
| **Type** | Buff |
| **Mana Cost** | 7 |
| **Target** | Self |
| **Buff Type** | AllStatsBuff |
| **Buff Magnitude** | 10 |
| **Buff Duration** | 30 seconds |
| **Description** | +50% damage, -20% defense |
| **Effect** | None specified |

Classic berserker trade-off: increased damage at the cost of survivability.

---

#### Ground Slam

| Property | Value |
|----------|-------|
| **File** | BarbarianAbilities.cs (line 50-53) |
| **Ability ID** | 2022 |
| **Circle** | 2 |
| **Type** | AoE Damage / CC |
| **Mana Cost** | 7 |
| **Damage** | 10-18 Physical |
| **AoE Range** | 4 tiles |
| **Target** | Area of Effect |
| **Range** | 7 tiles |
| **Description** | Knockdown nearby enemies |
| **Effect ID** | 0x36D4 |
| **Sound ID** | 0x1E5 |
| **Hit Sound** | 0x481 |

Slams the ground, dealing moderate AoE damage and knocking down enemies.

---

#### Winter's Endurance

| Property | Value |
|----------|-------|
| **File** | BarbarianAbilities.cs (line 55-57) |
| **Ability ID** | 2023 |
| **Circle** | 2 |
| **Type** | Buff |
| **Mana Cost** | 7 |
| **Target** | Self |
| **Buff Type** | AllStatsBuff |
| **Buff Magnitude** | 10 |
| **Buff Duration** | 30 seconds |
| **Description** | HP regen in cold areas |
| **Effect** | None specified |

Passive regeneration buff, thematically tied to Frosthold environment.

---

### Circle 3 Abilities (10 mana)

#### Avalanche Strike

| Property | Value |
|----------|-------|
| **File** | BarbarianAbilities.cs (line 59-62) |
| **Ability ID** | 2024 |
| **Circle** | 3 |
| **Type** | AoE Damage |
| **Mana Cost** | 10 |
| **Damage** | 20-35 Physical |
| **AoE Range** | 4 tiles |
| **Target** | Area of Effect |
| **Range** | 10 tiles |
| **Description** | Ice shards from ground slam |
| **Effect ID** | 0x36D4 |
| **Sound ID** | 0x1E5 |
| **Hit Sound** | 0x481 |

Enhanced ground slam with frost theme. High AoE damage.

---

#### Blood Rage

| Property | Value |
|----------|-------|
| **File** | BarbarianAbilities.cs (line 64-66) |
| **Ability ID** | 2025 |
| **Circle** | 3 |
| **Type** | Buff |
| **Mana Cost** | 10 |
| **Target** | Self |
| **Buff Type** | AllStatsBuff |
| **Buff Magnitude** | 10 |
| **Buff Duration** | 60 seconds |
| **Description** | Lifesteal on attacks |
| **Effect** | None specified |

Extended duration buff providing life steal. Sustain in extended fights.

---

#### Intimidating Roar

| Property | Value |
|----------|-------|
| **File** | BarbarianAbilities.cs (line 68-77) |
| **Ability ID** | 2026 |
| **Circle** | 3 |
| **Type** | Debuff |
| **Mana Cost** | 10 |
| **Target** | Single Target |
| **Range** | 12 tiles |
| **Buff Type** | AllStatsBuff |
| **Buff Magnitude** | 10 |
| **Buff Duration** | 30 seconds |
| **Description** | Reduce enemy damage 25% |
| **Effect ID** | 0x36D4 |
| **Sound ID** | 0x1E5 |
| **Hit Sound** | 0x481 |

Debuffs enemy damage output. Defensive utility for tough encounters.

---

#### Frenzy

| Property | Value |
|----------|-------|
| **File** | BarbarianAbilities.cs (line 79-81) |
| **Ability ID** | 2027 |
| **Circle** | 3 |
| **Type** | Buff |
| **Mana Cost** | 10 |
| **Target** | Self |
| **Buff Type** | AllStatsBuff |
| **Buff Magnitude** | 10 |
| **Buff Duration** | 60 seconds |
| **Description** | +50% attack speed |
| **Effect** | None specified |

Dramatically increases attack speed for burst damage windows.

---

### Circle 4 Abilities (13 mana)

#### Wrath of the North

| Property | Value |
|----------|-------|
| **File** | BarbarianAbilities.cs (line 83-86) |
| **Ability ID** | 2028 |
| **Circle** | 4 |
| **Type** | AoE Damage |
| **Mana Cost** | 13 |
| **Damage** | 30-50 Physical |
| **AoE Range** | 4 tiles |
| **Target** | Area of Effect |
| **Range** | 13 tiles |
| **Description** | Massive cold AoE |
| **Effect ID** | 0x36D4 |
| **Sound ID** | 0x1E5 |
| **Hit Sound** | 0x481 |

Ultimate AoE nuke with Frosthold theme. Highest damage potential.

---

#### Deathwish

| Property | Value |
|----------|-------|
| **File** | BarbarianAbilities.cs (line 88-90) |
| **Ability ID** | 2029 |
| **Circle** | 4 |
| **Type** | Buff |
| **Mana Cost** | 13 |
| **Target** | Self |
| **Buff Type** | AllStatsBuff |
| **Buff Magnitude** | 10 |
| **Buff Duration** | 60 seconds |
| **Description** | More damage when low HP |
| **Effect** | None specified |

High-risk buff scaling damage with missing health. Execute mechanic.

---

#### Rampage

| Property | Value |
|----------|-------|
| **File** | BarbarianAbilities.cs (line 92-95) |
| **Ability ID** | 2030 |
| **Circle** | 4 |
| **Type** | Multi-hit Damage |
| **Mana Cost** | 13 |
| **Damage** | 40-60 Physical (per hit) |
| **Target** | Single Target |
| **Range** | 12 tiles |
| **Description** | Chain of 5 attacks |
| **Effect ID** | 0x36D4 |
| **Sound ID** | 0x1E5 |
| **Hit Sound** | 0x481 |

Five consecutive attacks in rapid succession. Massive single-target burst.

---

#### Primal Avatar

| Property | Value |
|----------|-------|
| **File** | BarbarianAbilities.cs (line 97-106) |
| **Ability ID** | 2031 |
| **Circle** | 4 |
| **Type** | Transform Buff |
| **Mana Cost** | 13 |
| **Target** | Self |
| **Range** | 12 tiles |
| **Buff Type** | AllStatsBuff |
| **Buff Magnitude** | 10 |
| **Buff Duration** | 30 seconds |
| **Description** | Transform into frost giant |
| **Effect ID** | 0x36D4 |
| **Sound ID** | 0x1E5 |
| **Hit Sound** | 0x481 |

Ultimate transformation buff. Likely increases size, stats, and damage.

---

## Animation & Effects

### Effect IDs Used
- **0x36D4** - Primary impact visual (used by most abilities)
- **0x1E5** - Cast/activation sound
- **0x481** - Hit sound effect

All abilities currently share the same effect IDs, which should be differentiated in future updates.

---

## Known Issues

### Placeholder Implementations
All 16 abilities are currently **data structures only** with no actual functionality implemented:

1. **Damage Effects:** Damage values defined but no actual damage application
2. **Buff Mechanics:** Buff types/durations specified but no stat modifications
3. **AoE Detection:** AoE ranges defined but no area targeting logic
4. **HP Costs:** "Costs HP" noted but no health deduction
5. **Lifesteal:** "Lifesteal" described but no healing on hit
6. **Fear/CC:** "Fear" and "Knockdown" described but no crowd control
7. **Attack Speed:** "+50% attack speed" noted but no swing speed modification
8. **Damage Scaling:** "More damage when low HP" noted but no conditional scaling
9. **Chain Attacks:** "Chain of 5 attacks" noted but no multi-hit logic
10. **Transformation:** "Transform into frost giant" noted but no body change

### Effect Variety
All abilities use identical effect IDs (0x36D4 / 0x1E5 / 0x481). Each ability should have unique visuals:
- Frost effects for cold-themed abilities
- Blood effects for Blood Rage/Deathwish
- Ground shake effects for slams
- Whirlwind visual for Whirlwind
- Giant transformation for Primal Avatar

### Balance Concerns
- **Heavy Buff Focus:** 7/16 abilities are buffs (44%) - may feel passive
- **Limited Mobility:** No gap closers or charges
- **No Self-Healing:** Relies entirely on lifesteal from Blood Rage
- **High Mana Costs:** Circle 4 abilities at 13 mana may be prohibitive

---

## Integration Points

### Class Definition
**File:** `ServUO/Scripts/Custom/VystiaClasses/Classes/BarbarianClass.cs`

### Primary Resource
**Fury** - Likely builds through dealing/taking damage

### Related Skills
- Swordsmanship / Macing (melee weapons)
- Tactics (combat effectiveness)
- Anatomy (critical hits)
- Parrying (survivability)
- Wrestling (unarmed combat)
- VystiaMartialArts (custom skill)

### Starting Equipment
**Hue:** 1150-1153 (Frosthold theme)
**Resources:** FrozenOre, FrostforgedIngot, EternalIce

---

## Future Development Priorities

### Critical (Phase 1)
1. Implement actual damage application for all damage abilities
2. Implement buff stat modifications
3. Add AoE targeting and detection
4. Implement HP cost for Reckless Strike
5. Add unique visual effects per ability

### Important (Phase 2)
1. Implement lifesteal mechanic for Blood Rage
2. Implement fear/knockdown crowd control
3. Add attack speed modification for Frenzy
4. Implement low-HP damage scaling for Deathwish
5. Add chain attack logic for Rampage

### Enhancement (Phase 3)
1. Add transformation visual for Primal Avatar
2. Balance mana costs and cooldowns
3. Add mobility ability (charge/leap)
4. Differentiate AoE patterns (cone vs. circle)
5. Add rage resource generation mechanics

---

**Last Updated:** 2025-12-28
**Status:** Data structures complete, functionality pending implementation
**Build Status:** ✅ Compiles successfully (0 errors)
