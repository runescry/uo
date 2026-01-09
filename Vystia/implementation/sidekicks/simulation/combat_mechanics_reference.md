# UO Combat Mechanics Reference

This document contains the extracted mechanics from ServUO needed for accurate combat simulation.

## Spell Mechanics

### Mana Costs by Circle
| Circle | Mana Cost |
|--------|-----------|
| 1st    | 4         |
| 2nd    | 6         |
| 3rd    | 9         |
| 4th    | 11        |
| 5th    | 14        |
| 6th    | 20        |
| 7th    | 40        |
| 8th    | 50        |

### Cast Times (AOS)
Formula: `CastDelayBase = (4 + CircleNumber) * 0.25 seconds`

| Circle | Base Cast Time |
|--------|----------------|
| 1st    | 1.25s          |
| 2nd    | 1.50s          |
| 3rd    | 1.75s          |
| 4th    | 2.00s          |
| 5th    | 2.25s          |
| 6th    | 2.50s          |
| 7th    | 2.75s          |
| 8th    | 3.00s          |

**Faster Casting**: Each point of FC reduces cast time by 0.25s (cap: 2 for magery, 4 for chivalry)

### Cast Times (Pre-AOS)
Formula: `0.5 + (0.25 * CircleNumber) seconds`

### Key Combat Spells

#### Magic Arrow (1st Circle)
- **Mana**: 4
- **Range**: 10 (ML) or 12 tiles
- **Damage (AOS)**: `GetNewAosDamage(10, 1, 4, target)` = 10 + 1d4 base
- **Damage (Pre-AOS)**: 4-7 random
- **Element**: 100% Fire
- **Delayed Damage**: Yes (1.0s AOS, 0.5s Pre-AOS)
- **Use**: Interrupt enemy casting

#### Fireball (3rd Circle)
- **Mana**: 9
- **Range**: 10-12 tiles
- **Damage (AOS)**: `GetNewAosDamage(19, 1, 5, target)` = 19 + 1d5 base
- **Damage (Pre-AOS)**: 10-15 random
- **Element**: 100% Fire

#### Lightning (4th Circle)
- **Mana**: 11
- **Range**: 10-12 tiles
- **Damage (AOS)**: `GetNewAosDamage(23, 1, 4, target)` = 23 + 1d4 base
- **Damage (Pre-AOS)**: 12-20 random
- **Element**: 100% Energy
- **Delayed Damage**: No (instant)

#### Energy Bolt (6th Circle)
- **Mana**: 20
- **Range**: 10-12 tiles
- **Damage (AOS)**: `GetNewAosDamage(40, 1, 5, target)` = 40 + 1d5 base
- **Damage (Pre-AOS)**: 24-41 random
- **Element**: 100% Energy
- **Delayed Damage**: Yes (1.0s)

#### Explosion (6th Circle)
- **Mana**: 20
- **Range**: 10-12 tiles
- **Damage (AOS)**: `GetNewAosDamage(40, 1, 5, target)` = 40 + 1d5 base
- **Damage (Pre-AOS)**: 23-44 random
- **Element**: 100% Fire
- **Delayed Damage**: 3.0s (AOS) or 2.5s (Pre-AOS) timer before damage hits
- **Notes**: Can't stack on same target in AOS

### AOS Damage Formula
```
damage = Utility.Dice(dice, sides, bonus) * 100  // e.g., 40 + 1d5

// Inscribe bonus (max 10% at GM)
scribeBonus = inscribeSkill >= 1000 ? 10 : inscribeSkill / 200

// Damage bonus
damageBonus = scribeBonus + (Int / 10) + SDI_from_items

// Eval Int scaling (30% base + 9% per 10 skill)
evalScale = 30 + ((9 * evalSkill) / 100)

damage = damage * evalScale / 100
damage = damage * (100 + damageBonus) / 100
damage = damage * scalar  // target-specific modifiers
```

### Spell Damage Increase (SDI)
- **PvP Cap**: 15% (or 30% if spell-focused, 20% in TOL)
- **PvM**: No cap

### Spell Interruption
Casting is interrupted when:
1. Taking damage (unless Protection spell or Casting Focus)
2. Moving (Post-ML)
3. Equipping items
4. Death

**Protection Spell**: Gives chance to avoid interruption based on protection value
**Casting Focus**: Up to 12% chance to ignore interruption

---

## Healing Mechanics

### Bandage Healing

#### Heal Amount (AOS)
```
min = (anatomy / 8.0) + (healing / 5.0) + 4.0
max = (anatomy / 6.0) + (healing / 2.5) + 4.0
heal = min + random(0-1) * (max - min)
```

At GM (100/100): min=32.5, max=60.7 → avg ~46.6 HP

#### Bandage Timing (AOS)
**Self-healing**: 4-8 seconds based on dex
```
seconds = Math.Max(4, Math.Ceiling(11.0 - dex / 20))
```
- 100 dex = 6 seconds
- 120 dex = 5 seconds
- 140+ dex = 4 seconds

**Healing others**: 2-4 seconds
```
seconds = Math.Max(2, Math.Ceiling(4 - dex / 60))
```

#### Bandage Interrupts
- Target moves >2 tiles away
- Healer or target dies
- Target poisoned at start (must cure first)
- Target under Mortal Strike

### Healing Potions

| Potion Type    | Min HP | Max HP | Cooldown |
|----------------|--------|--------|----------|
| Lesser Heal    | 6      | 8      | 3.0s     |
| Heal           | 13     | 16     | 8.0s     |
| Greater Heal   | 20     | 25     | 10.0s    |

**Enhancement**: `scalar = 1.0 + (EnhancePotions + Alchemy/33) / 100` (cap 50% in ML)

### Healing Spells

#### Heal (1st Circle)
```
toHeal = (Magery / 120) + random(1, 4)
if (healing others): toHeal *= 1.5
```
- At 100 Magery: ~5-9 HP (self), ~7-13 HP (others)
- Mana: 9

#### Greater Heal (4th Circle)
```
toHeal = (Magery * 0.4) + random(1, 10)
```
- At 100 Magery: 41-50 HP
- Mana: 11

---

## Poison Mechanics

### Poison Levels (AOS)

| Level   | Min DMG | Max DMG | % Max HP | Interval | Ticks |
|---------|---------|---------|----------|----------|-------|
| Lesser  | 4       | 16      | 7.5%     | 2.25s    | 10    |
| Regular | 8       | 18      | 10%      | 3.25s    | 10    |
| Greater | 12      | 20      | 15%      | 4.25s    | 10    |
| Deadly  | 16      | 30      | 30%      | 5.25s    | 15    |
| Lethal  | 20      | 50      | 35%      | 5.25s    | 20    |

**Damage per tick**: `1 + (target.Hits * PoisonScalar)`

### Cure Chances

#### Cure Spell (2nd Circle)
```
chance = (10000 + (Magery * 75) - ((PoisonLevel + 1) * 3300)) / 100
```
- Mana: 6

#### Cure Potions
| Poison   | Standard | Greater |
|----------|----------|---------|
| Lesser   | 100%     | 100%    |
| Regular  | 95%      | 100%    |
| Greater  | 45%      | 75%     |
| Deadly   | 25%      | 45%     |
| Lethal   | 15%      | 25%     |

---

## SidekickAI Combat Parameters

### Health Thresholds
- **CRITICAL_HEALTH**: 29% - Emergency heal trigger
- **LOW_HEALTH**: 62% - Standard healing threshold

### Mana Thresholds
- **LOW_MANA**: 27 - Consider meditation
- **CRITICAL_MANA**: 18 - Force invisibility + meditation

### Distance Parameters
- **SPELL_RELEASE_RANGE_MIN**: 8 tiles
- **SPELL_RELEASE_RANGE_MAX**: 11 tiles
- **MIN_CAST_RANGE**: 3 tiles
- **MAX_CAST_RANGE**: 24 tiles
- **MEDITATION_FLEE_DISTANCE**: 29-43 tiles

### Combat Priorities (in order)
1. **Critical**: Distance ≤2 OR health <29%
2. **Defensive**: Poisoned OR health <62% OR took damage
3. **Interrupt**: Enemy casting
4. **Offensive**: Safe distance (16-24) AND mana ≥20
5. **Positioning**: Maintain optimal range

### Spell Combos
- **Explosion + Energy Bolt**: Requires 40+ mana, burst damage
- **Energy Bolt spam**: Requires 20+ mana per cast
- **Magic Arrow spam**: For interrupts, 4 mana, 0.75s cooldown

### Bandage Cooldown
- 10 seconds between bandage attempts

### Stuck Detection
- 2+ consecutive failed retreats
- Moved ≤2 tiles in 2 seconds
- When stuck: Allow casting at 8+ tiles instead of normal range

---

## Movement Mechanics

### Movement Speed
- **Walking**: ~1 tile per second
- **Running**: ~2 tiles per second

### Direction Costs (A* Pathfinding)
- Cardinal (N,S,E,W): Weight 1
- Diagonal (NE,NW,SE,SW): Weight 1.41

### Z-Level Rules
- **Max step up**: 2 Z units
- **Max step down**: Varies by tile
- Person height: 16 Z units

---

## Simulation Parameters to Optimize

These are the key parameters that can be tuned:

1. **min_retreat_distance**: When to start retreating (default: 4)
2. **casting_range**: Optimal spell casting distance (default: 12)
3. **health_heal_threshold**: When to prioritize healing (default: 62%)
4. **critical_health_threshold**: Emergency actions (default: 29%)
5. **mana_meditation_threshold**: When to meditate (default: 27)
6. **interrupt_priority**: When to interrupt vs damage (based on enemy cast progress)
7. **corner_awareness_radius**: How far to detect corners (default: 10-20 tiles)
8. **stuck_threshold**: Tiles moved before considered stuck (default: 2)
