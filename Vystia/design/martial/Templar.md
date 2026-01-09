# Templar - Implementation Documentation

**Class:** Templar
**Ability IDs:** 2112-2127 (16 abilities)
**Archetype:** Burst DPS
**Theme:** Zealous inquisitor with holy judgment powers
**Resource:** Zeal (builds through combat)
**School:** AbilitySchool.Templar

---

## Ability Overview

### Distribution by Type
- **Damage Abilities:** 5 (31.3%) - Judgment Strike, Smiting Blow, Condemn, Execution, Final Judgment
- **AoE Damage:** 1 (6.3%) - Wrath of the Justicar
- **Buff Abilities:** 6 (37.5%) - Zealot's Fervor, Aura of Justice, Divine Shield, Zealous Inspiration, Seal of Justice, Divine Retribution
- **Utility:** 4 (25.0%) - Interrogate, Absolution, Inquisitor's Eye, Iron Justicar

### Distribution by Circle
- **Circle 1:** 4 abilities (IDs 2112-2115)
- **Circle 2:** 4 abilities (IDs 2116-2119)
- **Circle 3:** 4 abilities (IDs 2120-2123)
- **Circle 4:** 4 abilities (IDs 2124-2127)

---

## Circle 1 Abilities (Mana Cost: 4)

| ID | Name | Type | Cost | Damage/Effect | Target | Description |
|---|---|---|---|---|---|---|
| 2112 | Judgment Strike | Damage | 4 | 16-24 Holy | Single (12 tiles) | Basic holy damage + mark |
| 2113 | Zealot's Fervor | Buff | 4 | +10 All Stats, 30s | Self | Build zeal on hit |
| 2114 | Aura of Justice | Buff | 4 | +10 All Stats, 30s | Self | +10% damage to allies |
| 2115 | Interrogate | Utility | 4 | +10 All Stats, 30s | Single (12 tiles) | Weaken marked target |

### Judgment Strike (2112)
- **Damage Type:** Holy
- **Range:** 12 tiles
- **Damage:** 16-24
- **Effect:** 0x36D4 (visual), 0x1E5 (sound), 0x481 (impact)
- **Notes:** Entry-level damage with marking mechanic

### Zealot's Fervor (2113)
- **Buff Type:** AllStatsBuff
- **Duration:** 30 seconds
- **Effect:** +10 to all stats, builds zeal on hit
- **Notes:** Self-buff for resource generation

### Aura of Justice (2114)
- **Buff Type:** AllStatsBuff
- **Duration:** 30 seconds
- **Effect:** +10 to all stats, +10% damage to nearby allies
- **Notes:** Party support buff

### Interrogate (2115)
- **Buff Type:** AllStatsBuff (debuff)
- **Duration:** 30 seconds
- **Effect:** Weaken marked target
- **Range:** 12 tiles
- **Effect:** 0x36D4 (visual), 0x1E5 (sound), 0x481 (impact)
- **Notes:** Utility debuff for marked enemies

---

## Circle 2 Abilities (Mana Cost: 7)

| ID | Name | Type | Cost | Damage/Effect | Target | Description |
|---|---|---|---|---|---|---|
| 2116 | Divine Shield | Buff | 7 | +10 All Stats, 30s | Self | Protective barrier |
| 2117 | Smiting Blow | Damage | 7 | 22-34 Holy | Single (12 tiles) | Zeal-empowered strike |
| 2118 | Condemn | Damage | 7 | 18-28 Holy | Single (12 tiles) | Mark + damage taken increase |
| 2119 | Absolution | Utility | 7 | +10 All Stats, 30s | Single (12 tiles) | Cleanse debuffs |

### Divine Shield (2116)
- **Buff Type:** AllStatsBuff
- **Duration:** 30 seconds
- **Effect:** +10 to all stats, protective barrier
- **Notes:** Defensive self-buff

### Smiting Blow (2117)
- **Damage Type:** Holy
- **Range:** 12 tiles
- **Damage:** 22-34
- **Effect:** 0x36D4 (visual), 0x1E5 (sound), 0x481 (impact)
- **Notes:** Higher damage, consumes zeal stacks

### Condemn (2118)
- **Damage Type:** Holy
- **Range:** 12 tiles
- **Damage:** 18-28
- **Effect:** 0x36D4 (visual), 0x1E5 (sound), 0x481 (impact)
- **Notes:** Applies mark and increases damage taken

### Absolution (2119)
- **Buff Type:** AllStatsBuff (cleanse)
- **Duration:** 30 seconds
- **Range:** 12 tiles
- **Effect:** 0x36D4 (visual), 0x1E5 (sound), 0x481 (impact)
- **Notes:** Remove debuffs from ally

---

## Circle 3 Abilities (Mana Cost: 10)

| ID | Name | Type | Cost | Damage/Effect | Target | Description |
|---|---|---|---|---|---|---|
| 2120 | Execution | Damage | 10 | 60-100 Holy | Single (12 tiles) | Execute marked target below 20% HP |
| 2121 | Zealous Inspiration | Buff | 10 | +10 All Stats, 60s | Party | Buff party attack speed |
| 2122 | Seal of Justice | Buff | 10 | +10 All Stats, 60s | Self | Attacks slow enemies |
| 2123 | Inquisitor's Eye | Utility | 10 | +10 All Stats, 30s | Single (12 tiles) | Reveal stealthed enemies |

### Execution (2120)
- **Damage Type:** Holy
- **Range:** 12 tiles
- **Damage:** 60-100
- **Effect:** 0x36D4 (visual), 0x1E5 (sound), 0x481 (impact)
- **Notes:** High damage finisher for low-HP marked targets (<20%)

### Zealous Inspiration (2121)
- **Buff Type:** AllStatsBuff
- **Duration:** 60 seconds
- **Effect:** +10 to all stats, increase party attack speed
- **Notes:** Strong party support buff

### Seal of Justice (2122)
- **Buff Type:** AllStatsBuff
- **Duration:** 60 seconds
- **Effect:** +10 to all stats, melee attacks apply slow
- **Notes:** Self-buff with CC on-hit effect

### Inquisitor's Eye (2123)
- **Buff Type:** AllStatsBuff (vision)
- **Duration:** 30 seconds
- **Range:** 12 tiles
- **Effect:** 0x36D4 (visual), 0x1E5 (sound), 0x481 (impact)
- **Notes:** True sight, reveal stealthed/invisible enemies

---

## Circle 4 Abilities (Mana Cost: 13)

| ID | Name | Type | Cost | Damage/Effect | Target | Description |
|---|---|---|---|---|---|---|
| 2124 | Final Judgment | Damage | 13 | 50-80 Holy | Single (12 tiles) | Consume all zeal for burst damage |
| 2125 | Wrath of the Justicar | AoE Damage | 13 | 40-65 Holy | AoE (4 tiles) | Holy AoE judgment |
| 2126 | Divine Retribution | Buff | 13 | +10 All Stats, 60s | Self | Return damage when hit |
| 2127 | Iron Justicar | Utility | 13 | +10 All Stats, 30s | Single (12 tiles) | Become invulnerable judge |

### Final Judgment (2124)
- **Damage Type:** Holy
- **Range:** 12 tiles
- **Damage:** 50-80
- **Effect:** 0x36D4 (visual), 0x1E5 (sound), 0x481 (impact)
- **Notes:** Consumes all zeal stacks for massive burst damage

### Wrath of the Justicar (2125)
- **Damage Type:** Holy
- **AoE Radius:** 4 tiles
- **Damage:** 40-65
- **Effect:** 0x36D4 (visual), 0x1E5 (sound), 0x481 (impact)
- **Notes:** Multi-target holy damage finisher

### Divine Retribution (2126)
- **Buff Type:** AllStatsBuff
- **Duration:** 60 seconds
- **Effect:** +10 to all stats, reflect damage when hit
- **Notes:** Thorns-like defensive buff

### Iron Justicar (2127)
- **Buff Type:** AllStatsBuff (invulnerability)
- **Duration:** 30 seconds
- **Range:** 12 tiles
- **Effect:** 0x36D4 (visual), 0x1E5 (sound), 0x481 (impact)
- **Notes:** Ultimate defensive ability, likely temporary invulnerability

---

## Core Mechanics

### Zeal Resource System
- **Generation:** Built through combat hits (Zealot's Fervor, normal attacks)
- **Consumption:** Smiting Blow, Final Judgment
- **Purpose:** Burst damage resource for timing big attacks

### Mark System
- **Application:** Judgment Strike, Condemn
- **Synergies:** Interrogate (weaken marked), Execution (execute marked <20% HP)
- **Purpose:** Target prioritization and burst damage setup

### Party Support
- **Aura of Justice:** +10% damage to nearby allies
- **Zealous Inspiration:** Increase party attack speed
- **Purpose:** Hybrid DPS/support role

### Defensive Tools
- **Divine Shield:** Protective barrier
- **Divine Retribution:** Damage reflection
- **Iron Justicar:** Invulnerability (ultimate defense)

---

## Known Issues & Placeholder Implementations

### Placeholder Buff System
- **Issue:** All buffs currently use `VystiaBuffType.AllStatsBuff`
- **Expected:** Unique buff types for each ability (DivineShield, ZealotsFervor, etc.)
- **Impact:** Limited distinction between different buffs

### Generic Visual Effects
- **Issue:** All abilities use same effect IDs (0x36D4, 0x1E5, 0x481)
- **Expected:** Unique holy-themed effects per ability
- **Impact:** Reduced visual clarity and thematic variety

### Missing Conditional Logic
- **Execution (2120):** Execute mechanic (<20% HP bonus) not implemented
- **Condemn (2118):** Damage taken increase not implemented
- **Seal of Justice (2122):** Slow-on-hit effect not implemented
- **Inquisitor's Eye (2123):** True sight reveal not implemented
- **Iron Justicar (2127):** Invulnerability not implemented

### No Zeal Resource Implementation
- **Issue:** Zeal generation/consumption logic missing
- **Expected:** Resource tracking and conditional ability costs
- **Impact:** Missing core class identity

---

## Strengths

✅ **Good Damage Scaling:** 16-24 → 22-34 → 60-100 → 50-80 progressive scaling
✅ **Clear Burst Windows:** Execute and Final Judgment provide defined kill opportunities
✅ **Party Support:** 3 party-oriented buffs (Aura, Inspiration, Retribution)
✅ **AoE Option:** Wrath of the Justicar for multi-target fights
✅ **Defensive Tools:** Divine Shield, Retribution, Iron Justicar
✅ **Utility Coverage:** Cleanse, true sight, mark system

---

## Weaknesses

⚠️ **No Mobility:** Zero gap closers, charges, or movement abilities
⚠️ **Heavy Mana Costs:** Circle 4 abilities cost 13 mana (high resource drain)
⚠️ **Placeholder Implementations:** Many abilities lack unique mechanics
⚠️ **Limited CC:** Only Seal of Justice provides crowd control (slow)
⚠️ **Zeal Not Implemented:** Core resource system missing
⚠️ **Same Visual Effects:** Lack of visual variety across abilities

---

## File Locations

**Implementation:**
- `ServUO/Scripts/Custom/VystiaClasses/Abilities/Generated/Martial/TemplarAbilities.cs`
- Auto-generated by `generate_martial_abilities.py`

**Class Definition:**
- `ServUO/Scripts/Custom/VystiaClasses/Classes/Martial/Templar.cs`

**Documentation:**
- `Vystia/design/martial/Templar.md` (this file)

---

## Future Enhancement Recommendations

### Phase 1: Core Identity (High Priority)
1. **Implement Zeal Resource System**
   - Track zeal stacks (0-5)
   - Generate on hits (Zealot's Fervor passive)
   - Consume for burst damage (Smiting Blow, Final Judgment)

2. **Implement Conditional Mechanics**
   - Execution: Bonus damage below 20% HP
   - Condemn: Apply damage taken debuff
   - Seal of Justice: Apply slow on melee hits

3. **Add Mobility Options**
   - Divine Leap (gap closer)
   - Zealous Charge (engage tool)
   - Judgment Dash (repositioning)

### Phase 2: Visual/Feel Polish (Medium Priority)
4. **Unique Visual Effects**
   - Golden/holy themed effects for judgment abilities
   - White/blue auras for defensive abilities
   - Red/orange effects for zeal-consuming attacks

5. **Unique Buff Types**
   - VystiaBuffType.DivineShield
   - VystiaBuffType.ZealotsFervor
   - VystiaBuffType.SealOfJustice
   - etc.

### Phase 3: Advanced Mechanics (Low Priority)
6. **Combo System**
   - Mark → Interrogate → Execution combo
   - Zeal generation → Final Judgment burst pattern

7. **Talent Tree Options**
   - Offensive: More damage on marked targets
   - Defensive: Increased barrier strength
   - Support: Larger aura radius

---

**Last Updated:** 2025-12-28
**Status:** ✅ Implementation complete, mechanics pending
