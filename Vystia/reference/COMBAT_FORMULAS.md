# Vystia Combat Formulas Reference

*Extracted from ServUO source code - Single Source of Truth*
*Last Updated: 2026-01-01*

---

## Table of Contents

1. [Melee Damage Formula](#1-melee-damage-formula)
2. [Armor & Resistance Reduction](#2-armor--resistance-reduction)
3. [Swing Timer / Attack Speed](#3-swing-timer--attack-speed)
4. [Resource Regeneration](#4-resource-regeneration)
5. [Spell Casting Time](#5-spell-casting-time)
6. [Spell Recovery (Cooldown)](#6-spell-recovery-cooldown)
7. [Mana Cost](#7-mana-cost)
8. [Damage Multipliers](#8-damage-multipliers)
9. [Vystia Secondary Resources](#9-vystia-secondary-resources)
10. [Vystia Ability Costs](#10-vystia-ability-costs)
11. [Quick Reference Tables](#11-quick-reference-tables)

---

## 1. Melee Damage Formula

**Source:** `ServUO/Scripts/Items/Equipment/Weapons/BaseWeapon.cs:3768-3823`

### AOS Era Formula (Current)

```
FinalDamage = BaseDamage + (BaseDamage × TotalBonus)

TotalBonus = STR_Bonus + Anatomy_Bonus + Tactics_Bonus + Lumberjack_Bonus + WeaponDamage%
```

### Bonus Calculations

| Bonus | Formula | Rate | Cap |
|-------|---------|------|-----|
| **STR** | `(STR × 0.30) / 5.0` | ~1% per 5 STR | 100% |
| **Anatomy** | `(Skill × 0.50) / 5.0` | ~1% per 2 skill | 100% |
| **Tactics** | `(Skill × 0.625) / 6.25` | ~1% per 1.6 skill | 100% |
| **Lumberjacking** | `(Skill × 0.20) / 10.0` | ~1% per 5 skill | 100% (axes only) |
| **Weapon Damage** | `AosAttribute.WeaponDamage` | 1% per point | 100% |

### Example Calculation

```
Scenario: 100 STR, 100 Tactics, 100 Anatomy, 20% Weapon Damage bonus
  Base Weapon Damage: 15

  STR Bonus:      (100 × 0.30) / 5.0  = 6.0%
  Anatomy Bonus:  (100 × 0.50) / 5.0  = 10.0%
  Tactics Bonus:  (100 × 0.625) / 6.25 = 10.0%
  Weapon Damage:  20%

  Total Bonus: 6 + 10 + 10 + 20 = 46%
  Final Damage: 15 + (15 × 0.46) = 15 + 6.9 = 21.9 ≈ 22
```

---

## 2. Armor & Resistance Reduction

**Source:** `ServUO/Scripts/Misc/AOS.cs:168-221`

### Formula (Percentage-Based)

```
Damage_After_Resist = Base_Damage × (Damage_Type% / 100) × (100 - Resistance) / 100

Total_Damage = (Physical + Fire + Cold + Poison + Energy) / 10000
```

### Resistance Ranges

| Target Type | Min Resistance | Max Resistance |
|-------------|----------------|----------------|
| Players | -70% | +70% |
| Creatures | -100% | +100% |

**Note:** Negative resistance = damage AMPLIFICATION

### Example Calculation

```
Scenario: 30 base damage, 100% physical, target has 50 Physical Resist
  Physical Damage = 30 × 100 × (100 - 50) / 10000
                  = 30 × 100 × 50 / 10000
                  = 15 damage

Scenario: 30 base damage, target has -20 Physical Resist (vulnerable)
  Physical Damage = 30 × 100 × (100 - (-20)) / 10000
                  = 30 × 100 × 120 / 10000
                  = 36 damage (amplified!)
```

### Elemental Damage Split

Weapons and spells distribute damage across types (must sum to 100%):

```csharp
int phys = 100;  // Physical %
int fire = 0;    // Fire %
int cold = 0;    // Cold %
int pois = 0;    // Poison %
int nrgy = 0;    // Energy %
// phys + fire + cold + pois + nrgy = 100
```

---

## 3. Swing Timer / Attack Speed

**Source:** `ServUO/Scripts/Items/Equipment/Weapons/BaseWeapon.cs:1541-1631`

### SE Era Formula (Current)

```
stamTicks = Stamina / 30
baseTicks = WeaponSpeed × 4
speedBonus = min(AosAttribute.WeaponSpeed, 60)

ticks = floor((baseTicks - stamTicks) × (100 / (100 + speedBonus)))
ticks = max(5, ticks)  // Minimum 5 ticks

SwingTime = ticks × 0.25 seconds
```

### Minimum Swing Speed

- **Absolute Minimum:** 1.25 seconds (5 ticks)
- **Speed Bonus Cap:** 60%

### Example Calculation

```
Scenario: Weapon Speed 30, 100 Stamina, 30% Speed Bonus
  stamTicks = 100 / 30 = 3.33
  baseTicks = 30 × 4 = 120

  ticks = floor((120 - 3.33) × (100 / 130))
        = floor(116.67 × 0.769)
        = floor(89.7) = 89 ticks

  SwingTime = 89 × 0.25 = 22.25 seconds

Scenario: Optimized (120 Stam, 60% Speed Bonus)
  stamTicks = 120 / 30 = 4
  ticks = floor((120 - 4) × (100 / 160))
        = floor(116 × 0.625)
        = floor(72.5) = 72 ticks

  SwingTime = 72 × 0.25 = 18 seconds
```

---

## 4. Resource Regeneration

**Source:** `ServUO/Scripts/Misc/RegenRates.cs`

**Tick Rate:** 250ms (0.25 seconds)

### Hit Points

```
HP_Regen_Per_Tick = (BaseRegen + AosAttribute.RegenHits) / 10.0

COMBAT PENALTY: HP regeneration STOPS completely in combat
```

### Stamina

```
Stam_Regen_Per_Tick = (STR / 50.0) + (AosAttribute.RegenStam / 10.0)

Per Second = Stam_Regen_Per_Tick × 4
```

### Mana

```
Mana_Regen_Per_Tick = (INT / 30.0) + (AosAttribute.RegenMana / 10.0)

Per Second = Mana_Regen_Per_Tick × 4
```

### Regeneration Table

| Stat Value | Base Regen/Tick | +10 Bonus | Per Second |
|------------|-----------------|-----------|------------|
| **Stamina (100 STR)** | 2.0 | 3.0 | 12.0/sec |
| **Mana (100 INT)** | 3.33 | 4.33 | 17.3/sec |
| **HP (no bonus)** | 0.1 | 1.1 | 4.4/sec |

---

## 5. Spell Casting Time

**Source:** `ServUO/Scripts/Spells/Base/Spell.cs:1031-1086`

### Base Formula

```
BaseCastTime = (4 + SpellCircle) × 0.25 seconds
FinalCastTime = BaseCastTime - (FC × 0.25 seconds)
```

### Faster Casting (FC) Caps

| Spell Type | FC Cap | Max Reduction |
|------------|--------|---------------|
| Magery | 2 | 0.5 seconds |
| Necromancy | 2 | 0.5 seconds |
| Mysticism | 2 | 0.5 seconds |
| Chivalry (Magery ≥ 70) | 2 | 0.5 seconds |
| Chivalry (Magery < 70) | 4 | 1.0 seconds |
| Other | 4 | 1.0 seconds |

### Modifiers

| Modifier | Effect |
|----------|--------|
| Protection Spell | FC reduced by 2 |
| Urali Potion | FC reduced by 2 |
| DreadHorn | Cast time doubled |
| **Minimum** | 0.25 seconds |

### Cast Time by Circle

| Circle | Base | FC 0 | FC 2 | FC 4 |
|--------|------|------|------|------|
| 1 | 1.25s | 1.25s | 0.75s | 0.25s* |
| 2 | 1.50s | 1.50s | 1.00s | 0.50s |
| 3 | 1.75s | 1.75s | 1.25s | 0.75s |
| 4 | 2.00s | 2.00s | 1.50s | 1.00s |
| 5 | 2.25s | 2.25s | 1.75s | 1.25s |
| 6 | 2.50s | 2.50s | 2.00s | 1.50s |
| 7 | 2.75s | 2.75s | 2.25s | 1.75s |
| 8 | 3.00s | 3.00s | 2.50s | 2.00s |

*FC 4 only available for Chivalry with low Magery

---

## 6. Spell Recovery (Cooldown)

**Source:** `ServUO/Scripts/Spells/Base/Spell.cs:999-1023`

### Formula

```
RecoveryTime = (6 - FCR) / 4 seconds

FCR Cap: 6
Minimum: 0 seconds
```

### Recovery by FCR

| FCR | Recovery Time |
|-----|---------------|
| 0 | 1.50 seconds |
| 1 | 1.25 seconds |
| 2 | 1.00 seconds |
| 3 | 0.75 seconds |
| 4 | 0.50 seconds |
| 5 | 0.25 seconds |
| 6 | 0.00 seconds |

### Total Time Between Spells

```
Total = CastTime + RecoveryTime

Example: 5th Circle, FC 2, FCR 4
  Cast: 2.25 - 0.50 = 1.75s
  Recovery: (6 - 4) / 4 = 0.50s
  Total: 2.25 seconds
```

---

## 7. Mana Cost

**Source:** `ServUO/Scripts/Spells/Base/Spell.cs:945-980`

### Base Mana by Circle

| Circle | Base Mana |
|--------|-----------|
| 1 | 4 |
| 2 | 6 |
| 3 | 9 |
| 4 | 11 |
| 5 | 14 |
| 6 | 20 |
| 7 | 40 |
| 8 | 50 |

### Lower Mana Cost (LMC) Formula

```
EffectiveLMC = min(AosAttribute.LowerManaCost + ArmorLMC, 40)
FinalMana = BaseMana × (1.0 - EffectiveLMC / 100)

LMC CAP: 40% maximum reduction
```

### Modifiers

| Modifier | Effect |
|----------|--------|
| MindRot Curse | Increases mana cost |
| Purge Magic | +50% mana cost |
| Mana Phasing Orb | FREE cast (0 mana) |

### Mana Cost with LMC

| Circle | Base | 20% LMC | 40% LMC |
|--------|------|---------|---------|
| 1 | 4 | 3 | 2 |
| 2 | 6 | 5 | 4 |
| 3 | 9 | 7 | 5 |
| 4 | 11 | 9 | 7 |
| 5 | 14 | 11 | 8 |
| 6 | 20 | 16 | 12 |
| 7 | 40 | 32 | 24 |
| 8 | 50 | 40 | 30 |

---

## 8. Damage Multipliers

**Source:** `ServUO/Scripts/Items/Equipment/Weapons/BaseWeapon.cs:2385-2576`

### Multiplicative Bonuses

| Source | Bonus Range |
|--------|-------------|
| Weapon Ability | +50% to +200% |
| Special Move | +25% to +50% |
| Slayer | +200% |
| Super Slayer | +100% |
| Opposition Slayer | +100% |
| Pack Instinct | Varies |
| Double Strike | -10% |

### Total Multiplier Cap

```
percentageBonus = min(percentageBonus, 300)

Maximum: 300% bonus (4.0× total multiplier)
```

### Ignore Armor

| Target | Effect |
|--------|--------|
| Creatures | Full damage bypasses resistance |
| Players (melee) | Capped at 35 damage |
| Players (ranged) | Capped at 30 damage |

---

## 9. Vystia Secondary Resources

**Source:** `ServUO/Scripts/Custom/VystiaClasses/Systems/SecondaryResource.cs`

### Resource Types (15 Total)

#### Combat Resources

| Resource | Class | Max | Generation | Decay |
|----------|-------|-----|------------|-------|
| **Fury** | Barbarian | 100 | +5/hit dealt, +10/hit taken, +20/kill | -5/sec OOC |
| **Chi** | Monk | 5 | On ability use | None |
| **ComboPoints** | Rogue | 5 | Per-target, on hit | 30s duration |
| **Focus** | Ranger | 100 | +10/sec stationary | -5/sec moving |
| **Zeal** | Templar | 10 | +1/hit, +2/crit, +3/kill | -1/sec OOC |
| **Fortitude** | Knight | 10 | +1/block, +bonus on big hits | -1/sec OOC |
| **Pursuit** | Bounty Hunter | 10 | Per marked target | No expiration |

#### Magic Resources

| Resource | Class | Max | Generation | Decay |
|----------|-------|-----|------------|-------|
| **SoulShards** | Warlock | 3 | On kill/crit (25% chance) | None |
| **LifeForce** | Necromancer | 100 | +10-25 per nearby death | -2/tick OOC |
| **ChillStacks** | Ice Mage | 5 | Per-target, on ice spell | 5 stacks = Frozen |
| **Crescendo** | Bard | 20 | +1/tick while channeling | Decays OOC |
| **Faith** | Cleric | 100 | +5/heal, +10/crit, +20/rez | None |

#### Crafting/Utility Resources

| Resource | Class | Max | Generation | Decay |
|----------|-------|-----|------------|-------|
| **Steam** | Artificer | 100 | +10/sec near boiler | -5/sec away |
| **Charges** | Artificer | 10 | Crafted only | None |
| **Virtues** | Paladin | 3×4 | 4 virtue types | None |

### Resource Thresholds

| Resource | Threshold | Effect |
|----------|-----------|--------|
| Fury 100 | Rage Mode | +damage, reduced defense |
| Chill 5 | Frozen | Target frozen, +50% cold damage |
| ComboPoints 5 | Finisher Ready | Max damage finishers |
| Chi 5 | Ultimate Ready | Strongest abilities |

---

## 10. Vystia Ability Costs

**Source:** `ServUO/Scripts/Custom/VystiaClasses/Abilities/`

### Ability Distribution

| Category | Schools | Total Abilities |
|----------|---------|-----------------|
| Magic | 12 schools | 384 (32 each) |
| Martial | 14 classes | ~352 (~25 each) |
| **Total** | 26 | **736** |

### Cost Ranges by Circle

| Circle | Mana Cost | Stamina Cost | Damage Range | Typical Cooldown |
|--------|-----------|--------------|--------------|------------------|
| 1 | 4 | 5-10 | 4-10 | 0-3s |
| 2 | 6-7 | 10-15 | 8-14 | 3-6s |
| 3 | 9 | 15-20 | 15-25 | 6-10s |
| 4 | 11 | 20-25 | 20-35 | 10-15s |
| 5 | 14 | 25-30 | 30-50 | 15-25s |
| 6 | 20 | 30-40 | 40-60 | 25-40s |
| 7 | 40 | 40-50 | 50-80 | 40-60s |
| 8 | 60+ | 50-60 | 80-120+ | 60-120s |

### Ability ID Ranges

| School | ID Range | Example |
|--------|----------|---------|
| Ice Magic | 1000-1031 | Frost Touch (1000) |
| Nature | 1032-1063 | Entangle (1032) |
| Hex | 1064-1095 | Curse (1064) |
| Elemental | 1096-1127 | Fireball (1096) |
| Dark | 1128-1159 | Shadow Bolt (1128) |
| Divination | 1160-1191 | Foresight (1160) |
| Necromancy | 1192-1223 | Raise Dead (1192) |
| Summoning | 1224-1255 | Summon (1224) |
| Shamanic | 1256-1287 | Spirit Link (1256) |
| Bardic | 1288-1319 | Inspire (1288) |
| Enchanting | 1320-1351 | Enchant (1320) |
| Illusion | 1352-1383 | Mirror Image (1352) |
| Barbarian | 2016-2047 | Reckless Strike (2016) |
| (other martial) | 2048+ | Varies |

### Effect Types (26+)

```
DAMAGE:      DirectDamage, DamageOverTime, PercentDamage
HEALING:     DirectHeal, HealOverTime, PercentHeal
BUFFS:       ApplyBuff, ApplyDebuff, RemoveBuff, DispelMagic
CC:          ApplyCC, RemoveCC
RESOURCES:   RestoreMana, DrainMana, GenerateResource, ConsumeResource
STACKS:      ApplyStack, ConsumeStack, CheckStack
SUMMON:      Summon, Dismiss, PetCommand
TRANSFORM:   ApplyTransform, RemoveTransform
MOVEMENT:    Teleport, Knockback, Pull
UTILITY:     BreakStealth, EnterStealth, Interrupt, Taunt
SPECIAL:     ConditionalEffect, RepeatEffect, DelayedEffect
```

---

## 11. Quick Reference Tables

### Stat Scaling Summary

| Stat | Melee Damage | Spell Damage | Regen |
|------|--------------|--------------|-------|
| STR | +1% per 5 | - | Stam: +0.02/tick per point |
| DEX | Swing speed | - | - |
| INT | - | +0.5% per point | Mana: +0.033/tick per point |

### Key Attribute Caps

| Attribute | Cap | Effect |
|-----------|-----|--------|
| Faster Casting (FC) | 2-4 | -0.25s cast per point |
| Faster Cast Recovery (FCR) | 6 | -0.25s recovery per point |
| Lower Mana Cost (LMC) | 40% | -1% mana per point |
| Weapon Speed | 60% | Swing speed bonus |
| Weapon Damage | 100% | Damage bonus |
| Damage Multiplier | 300% | Total multiplicative cap |

### Time-to-Kill Estimates

| Scenario | Weapon DPS | Spell DPS | Combined |
|----------|------------|-----------|----------|
| Low Gear | ~15/swing | ~20/cast | ~10 DPS |
| Mid Gear | ~25/swing | ~35/cast | ~20 DPS |
| High Gear | ~40/swing | ~50/cast | ~35 DPS |
| Optimized | ~60/swing | ~80/cast | ~50 DPS |

### Resistance Breakpoints

| Resist | Damage Taken | Notes |
|--------|--------------|-------|
| -70 | 170% | Maximum vulnerability (players) |
| 0 | 100% | No reduction |
| 35 | 65% | "Capped" in some contexts |
| 50 | 50% | Half damage |
| 70 | 30% | Maximum resist (players) |

---

## Source File Reference

| Formula | Source File | Line Numbers |
|---------|-------------|--------------|
| Melee Damage | BaseWeapon.cs | 3768-3823 |
| Armor Reduction | AOS.cs | 168-221 |
| Swing Timer | BaseWeapon.cs | 1541-1631 |
| Resource Regen | RegenRates.cs | (various) |
| Cast Time | Spell.cs | 1031-1086 |
| Cast Recovery | Spell.cs | 999-1023 |
| Mana Cost | Spell.cs | 945-980 |
| Damage Multipliers | BaseWeapon.cs | 2385-2576 |
| Secondary Resources | SecondaryResource.cs | 1-1186 |
| Ability Definitions | AbilityDefinition.cs | 1-804 |
| Ability Execution | AbilityExecutor.cs | 1-1100+ |

---

*This document is the authoritative reference for all combat calculations in Vystia.*
*For ability-specific details, see the individual ability files in `ServUO/Scripts/Custom/VystiaClasses/Abilities/`*
