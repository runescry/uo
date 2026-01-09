# Alchemist - Current Implementation

**Class:** Alchemist
**Ability IDs:** 2176-2191
**Archetype:** Burst/Utility
**Theme:** Battle chemist with explosive flasks, mutagens, and alchemical transformations
**Resource:** Verdantpeak (Hue 2010)

---

## Ability Breakdown (16 abilities)

### Circle 1 (4 abilities)

| ID | Name | Type | Cost | Damage/Heal | AoE | Effect | Description |
|----|------|------|------|-------------|-----|--------|-------------|
| 2176 | Fire Bomb | AoE Damage | 4 | 12-18 Physical | 4 tiles | Impact: 0x36D4, Sound: 0x1E5, Particles: 0x481 | Fire flask AoE |
| 2177 | Ice Bomb | AoE Damage | 4 | 10-15 Physical | 4 tiles | Impact: 0x36D4, Sound: 0x1E5, Particles: 0x481 | Cold flask + slow |
| 2178 | Healing Draught | Heal | 4 | 20-30 HP | Self | Impact: 0x376A, Sound: 0x1F2, Particles: 0x481 | Quick healing potion |
| 2179 | Reagent Gather | Utility | 4 | - | 12 tiles | AllStatsBuff (10s, 30 strength) | Collect nearby reagents |

### Circle 2 (4 abilities)

| ID | Name | Type | Cost | Damage/Heal | AoE | Effect | Description |
|----|------|------|------|-------------|-----|--------|-------------|
| 2180 | Acid Flask | Damage | 7 | 15-22 Physical | Single | Impact: 0x36D4, Sound: 0x1E5, Particles: 0x481 | +acid DoT |
| 2181 | Smoke Bomb | AoE CC | 7 | 0 damage | 4 tiles | Impact: 0x36D4, Sound: 0x1E5, Particles: 0x481 | Blind in area |
| 2182 | Mutagen: Strength | Buff | 7 | - | Self | AllStatsBuff (10s, 30 strength) | +20 STR |
| 2183 | Mutagen: Speed | Buff | 7 | - | Self | AllStatsBuff (10s, 30 strength) | +30% attack speed |

### Circle 3 (4 abilities)

| ID | Name | Type | Cost | Damage/Heal | AoE | Effect | Description |
|----|------|------|------|-------------|-----|--------|-------------|
| 2184 | Alchemical Bomb | AoE Damage | 10 | 28-42 Physical | 4 tiles | Impact: 0x36D4, Sound: 0x1E5, Particles: 0x481 | Choose element type |
| 2185 | Transmutation | Utility | 10 | - | 12 tiles | AllStatsBuff (10s, 30 strength) | Alter object properties |
| 2186 | Mutagen: Resistance | Buff | 10 | - | Self | AllStatsBuff (10s, 60 strength) | +20 all resists |
| 2187 | Instant Brew | Utility | 10 | - | 12 tiles | AllStatsBuff (10s, 30 strength) | Quick craft potion |

### Circle 4 (4 abilities)

| ID | Name | Type | Cost | Damage/Heal | AoE | Effect | Description |
|----|------|------|------|-------------|-----|--------|-------------|
| 2188 | Philosopher's Bomb | AoE Damage | 13 | 45-70 Physical | 4 tiles | Impact: 0x36D4, Sound: 0x1E5, Particles: 0x481 | Massive alchemy explosion |
| 2189 | Elixir of Life | AoE Heal | 13 | 80-120 HP | Party | Impact: 0x376A, Sound: 0x1F2, Particles: 0x481 | Full party heal |
| 2190 | Mutagen: Transcendence | Transform | 13 | - | 12 tiles | AllStatsBuff (10s, 30 strength) | Mutate into hybrid form |
| 2191 | Volatile Mixture | AoE Damage | 13 | 55-85 Physical | 4 tiles | Impact: 0x36D4, Sound: 0x1E5, Particles: 0x481 | Chain reaction explosions |

---

## Ability Categorization

### Damage Abilities (7/16 = 44%)
- **AoE Damage:** Fire Bomb, Ice Bomb, Alchemical Bomb, Philosopher's Bomb, Volatile Mixture (5)
- **Single Target:** Acid Flask (1)
- **Healing:** Healing Draught, Elixir of Life (2 - counted separately)

### Utility Abilities (4/16 = 25%)
- **Buff/Transform:** Mutagen: Strength, Mutagen: Speed, Mutagen: Resistance, Mutagen: Transcendence (4)
- **Crafting/Gathering:** Reagent Gather, Transmutation, Instant Brew (3)

### Support Abilities (3/16 = 19%)
- **Healing:** Healing Draught (single), Elixir of Life (party) (2)
- **Crowd Control:** Smoke Bomb (1)

### Defense Abilities (0/16 = 0%)
- None

### Mobility Abilities (0/16 = 0%)
- None

---

## Core Mechanics

### Explosive Flasks (5 abilities)
Primary damage source via AoE bombs with 4-tile radius:
- **Fire Bomb** (Circle 1): 12-18 damage, 4 mana
- **Ice Bomb** (Circle 1): 10-15 damage + slow, 4 mana
- **Alchemical Bomb** (Circle 3): 28-42 damage, 10 mana
- **Philosopher's Bomb** (Circle 4): 45-70 damage, 13 mana
- **Volatile Mixture** (Circle 4): 55-85 damage, 13 mana

### Mutagen System (4 abilities)
Self-buffs with increasing duration:
- **Mutagen: Strength** (Circle 2): +20 STR, 10s, 7 mana
- **Mutagen: Speed** (Circle 2): +30% attack speed, 10s, 7 mana
- **Mutagen: Resistance** (Circle 3): +20 all resists, 10s, 10 mana
- **Mutagen: Transcendence** (Circle 4): Hybrid form, 10s, 13 mana

### Alchemical Crafting (3 abilities)
Utility abilities for resource manipulation:
- **Reagent Gather** (Circle 1): Collect nearby reagents
- **Transmutation** (Circle 3): Alter object properties
- **Instant Brew** (Circle 3): Quick craft potion

---

## Strengths
- ✅ Strong AoE burst damage (5 bomb abilities)
- ✅ Excellent multi-target potential
- ✅ Party healing via Elixir of Life (80-120 HP)
- ✅ Versatile buff suite via mutagens
- ✅ Low-cost starter abilities (4 mana Circle 1)
- ✅ Progression: Damage scales well from Circle 1 to 4

## Weaknesses
- ⚠️ **Zero mobility** - No dash, teleport, or movement abilities
- ⚠️ **Zero defense** - No shields, barriers, or damage reduction
- ⚠️ **All damage is Physical** - No elemental variety despite theme
- ⚠️ **Short buff durations** - All buffs only 10 seconds
- ⚠️ **Generic buff system** - All use "AllStatsBuff" placeholder
- ⚠️ **High AoE dependency** - 5/7 damage abilities are AoE
- ⚠️ **Limited single-target** - Only 1 single-target damage ability

---

## Implementation Issues

### Critical Issues
1. **All damage is Physical type** - Should have Fire (2176), Cold (2177), Poison (2180), Energy (2184), etc.
2. **AllStatsBuff placeholder** - All buffs use generic buff type instead of specific effects
3. **Missing DoT implementation** - Acid Flask (2180) has "+acid DoT" but not implemented
4. **Missing Slow implementation** - Ice Bomb (2177) has "+slow" but not implemented
5. **Missing Blind implementation** - Smoke Bomb (2181) has "Blind in area" but deals 0 damage with no CC
6. **Generic effect IDs** - Many abilities share same impact/sound effects
7. **Transmutation/Instant Brew/Reagent Gather** - All are placeholder SingleTarget buffs

### Balance Issues
1. **No mobility** - Every other class archetype has at least 1 mobility ability
2. **No defense** - Burst classes usually have 1-2 defensive cooldowns
3. **Short buff durations** - 10 seconds is very short for combat buffs
4. **Mutagen: Resistance** - 10s duration with 60 strength value (unclear what this means)
5. **Elixir of Life** - 80-120 healing is strong, but 13 mana cost may be too low

### Design Inconsistencies
1. **Element selection** - Alchemical Bomb (2184) says "Choose element type" but type is hardcoded Physical
2. **Chain reactions** - Volatile Mixture (2191) says "Chain reaction explosions" but no chain mechanic
3. **Hybrid form** - Mutagen: Transcendence (2190) says "Mutate into hybrid form" but just applies buff
4. **Party heal** - Elixir of Life (2189) targets party but uses SingleTarget with AoE heal

---

## File Locations

### Class Definition
- `ServUO/Scripts/Custom/VystiaClasses/Classes/Martial/Alchemist.cs`

### Abilities
- `ServUO/Scripts/Custom/VystiaClasses/Abilities/Generated/Martial/AlchemistAbilities.cs`

### Special Items
- AlchemistKit: `ServUO/Scripts/Items/Vystia/Equipment/ClassSpecialItems.cs`

### Resources
- LivingOre, NatureforgedIngot, LivingBark (Verdantpeak region)

### Reference Documentation
- Class stats: `Vystia/reference/CLASSES.md`
- Ability commands: `Vystia/reference/COMMANDS.md`

---

## Testing Commands

```
[setclass alchemist           - Assign Alchemist class
[giveability 2176             - Give Fire Bomb
[giveability 2176 2191        - Give all abilities (2176-2191)
[ability 2176                 - Cast Fire Bomb
[clearabilities               - Remove all abilities
```

---

**Last Updated:** 2025-12-28
**Status:** Implementation documented, critical issues identified
**Next Steps:** Fix damage types, implement DoT/CC effects, add mobility/defense abilities
