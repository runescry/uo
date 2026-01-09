# Beastmaster - Implementation Documentation

**Class:** Beastmaster
**Ability IDs:** 2144-2159 (16 abilities)
**Ability School:** `AbilitySchool.Beastmaster`
**Archetype:** Pet DPS
**Region:** Frosthold
**Theme:** Pack alpha who commands beasts and excels at archery
**Role:** Pet/Ranged DPS

---

## File Locations

- **Class Definition:** `ServUO/Scripts/Custom/VystiaClasses/Classes/BeastmasterClass.cs`
- **Abilities:** `ServUO/Scripts/Custom/VystiaClasses/Abilities/Generated/Martial/BeastmasterAbilities.cs`
- **Special Item:** `ServUO/Scripts/Custom/VystiaClasses/Items/AbilityItems/ClassSpecialItems.cs` (BeastWhistle)

---

## Class Stats & Skills

### Base Stats (Total: 300)
| Stat | Start | Cap |
|------|-------|-----|
| STR | 100 | 110 |
| DEX | 100 | 130 |
| INT | 100 | 100 |

### Primary Skills (6 skills, Total: 240.0)
| Skill | Starting Value |
|-------|---------------|
| Animal Taming | 50.0 |
| Animal Lore | 50.0 |
| Veterinary | 40.0 |
| Archery | 35.0 |
| Tactics | 35.0 |
| Tracking | 30.0 |

### Starting Equipment
- **Armor:** Full leather set (Frosthold hue 1150)
  - Beastmaster's Jerkin (chest)
  - Leather legs, arms, gloves, gorget, cap
- **Weapon:** Beastmaster's Bow (hue 1150)
- **Resources:** 20 Frozen Ore, 100 Arrows
- **Special Item:** Beast Whistle (10 charges)

---

## Special Class Item: Beast Whistle

**Item ID:** 0x1F1C (Flute graphic)
**Hue:** 1150 (Frosthold)
**Charges:** 10
**Effect:** Summons temporary animal companion (scales with BeastBonding skill)
**Duration:** 10 minutes
**Follower Cost:** 2 slots
**Visual:** Particle effect 0x376A (blue sparkles)
**Sound:** 0x4CF (whistle sound)

---

## Ability Distribution (16 abilities)

| Category | Count | Percentage | Ability IDs |
|----------|-------|------------|-------------|
| **Damage** | 3 | 18.75% | 2148, 2155, 2157 |
| **Pet Buffs** | 6 | 37.5% | 2146, 2149, 2150, 2153, 2154, 2156 |
| **Pet Utility** | 4 | 25% | 2144, 2145, 2151, 2152 |
| **Healing** | 1 | 6.25% | 2147 |
| **Transform** | 2 | 12.5% | 2158, 2159 |

---

## Circle 1 Abilities (Mana: 4, IDs: 2144-2147)

| ID | Name | Type | Cost | Effect | Description |
|----|------|------|------|--------|-------------|
| 2144 | Call Pet | Utility | 4 mana | +10 All Stats buff, 30s | Summon bonded beast |
| 2145 | Growl | Utility | 4 mana | +10 All Stats buff, 30s | Pet taunts enemy |
| 2146 | Pack Tactics | Buff | 4 mana | +10 All Stats buff, 30s | Bonus with pet nearby |
| 2147 | Mend Pet | Heal | 4 mana | 25-40 healing | Heal pet |

**Shared Properties (Circle 1):**
- **Range:** 12 tiles (single target)
- **Effect:** 0x36D4 (visual), 0x1E5 (hue), 0x481 (sound) - except Mend Pet
- **Mend Pet Effect:** 0x376A (visual), 0x1F2 (hue), 0x481 (sound)

---

## Circle 2 Abilities (Mana: 7, IDs: 2148-2151)

| ID | Name | Type | Cost | Damage/Effect | Description |
|----|------|------|------|---------------|-------------|
| 2148 | Kill Command | Damage | 7 mana | 18-28 Physical | Pet attacks target |
| 2149 | Feral Bond | Buff | 7 mana | +10 All Stats buff, 30s | Share senses with pet |
| 2150 | Alpha's Command | Buff | 7 mana | +10 All Stats buff, 30s | Empower pet |
| 2151 | Bestial Call | Utility | 7 mana | +10 All Stats buff, 30s | Summon temporary beast |

**Shared Properties (Circle 2):**
- **Range:** 12 tiles (single target)
- **Effect:** 0x36D4 (visual), 0x1E5 (hue), 0x481 (sound)
- **Damage Type:** Physical (100%)

---

## Circle 3 Abilities (Mana: 10, IDs: 2152-2155)

| ID | Name | Type | Cost | Damage/Effect | Description |
|----|------|------|------|---------------|-------------|
| 2152 | Call of the Wild | Utility | 10 mana | +10 All Stats buff, 30s | Summon wolf pack |
| 2153 | Primal Rage | Buff | 10 mana | +10 All Stats buff, 60s | Pet enrages |
| 2154 | Spirit Bond | Buff | 10 mana | +10 All Stats buff, 60s | Share HP with pet |
| 2155 | Savage Rend | Damage | 10 mana | 28-42 Physical | Pet combo attack |

**Shared Properties (Circle 3):**
- **Range:** 12 tiles (single target)
- **Effect:** 0x36D4 (visual), 0x1E5 (hue), 0x481 (sound)
- **Buff Duration:** 60s (longer than Circle 1-2)
- **Damage Type:** Physical (100%)

---

## Circle 4 Abilities (Mana: 13, IDs: 2156-2159)

| ID | Name | Type | Cost | Damage/Effect | Description |
|----|------|------|------|---------------|-------------|
| 2156 | Beast Mastery | Buff | 13 mana | +10 All Stats buff, 60s | Pet +100% damage |
| 2157 | Stampede | AoE Damage | 13 mana | 35-55 Physical, Range 4 | All pets charge |
| 2158 | Alpha Predator | Transform | 13 mana | +10 All Stats buff, 30s | Merge with beast |
| 2159 | Winter Pack | Utility | 13 mana | +10 All Stats buff, 30s | Summon 3 arctic wolves |

**Shared Properties (Circle 4):**
- **Range:** 12 tiles (single target), except Stampede (AoE radius 4)
- **Effect:** 0x36D4 (visual), 0x1E5 (hue), 0x481 (sound)
- **Stampede Type:** AoE (Area of Effect)

---

## Ability Categories Breakdown

### Damage Abilities (3 total)
| Circle | ID | Name | Damage | Type | Notes |
|--------|----|----|--------|------|-------|
| 2 | 2148 | Kill Command | 18-28 | Physical | Pet attack |
| 3 | 2155 | Savage Rend | 28-42 | Physical | Pet combo |
| 4 | 2157 | Stampede | 35-55 | Physical | AoE radius 4 |

**Damage Scaling:**
- Circle 2: 18-28 avg 23
- Circle 3: 28-42 avg 35
- Circle 4: 35-55 avg 45

### Pet Buff Abilities (6 total)
| Circle | ID | Name | Buff Amount | Duration | Special |
|--------|----|----|-------------|----------|---------|
| 1 | 2146 | Pack Tactics | +10 All Stats | 30s | Nearby bonus |
| 2 | 2149 | Feral Bond | +10 All Stats | 30s | Share senses |
| 2 | 2150 | Alpha's Command | +10 All Stats | 30s | Empower |
| 3 | 2153 | Primal Rage | +10 All Stats | 60s | Enrage |
| 3 | 2154 | Spirit Bond | +10 All Stats | 60s | HP share |
| 4 | 2156 | Beast Mastery | +10 All Stats | 60s | +100% dmg |

**Buff Progression:**
- Circle 1-2: 30s duration
- Circle 3-4: 60s duration (doubled)
- All buffs: +10 to all stats

### Pet Utility Abilities (4 total)
| Circle | ID | Name | Effect | Description |
|--------|----|----|--------|-------------|
| 1 | 2144 | Call Pet | Summon | Bonded beast |
| 1 | 2145 | Growl | Taunt | Pet aggro |
| 2 | 2151 | Bestial Call | Summon | Temporary beast |
| 3 | 2152 | Call of the Wild | Summon | Wolf pack |

### Healing Abilities (1 total)
| Circle | ID | Name | Healing | Range | Effect |
|--------|----|----|---------|-------|--------|
| 1 | 2147 | Mend Pet | 25-40 | 12 tiles | 0x376A visual |

### Transform Abilities (2 total)
| Circle | ID | Name | Effect | Duration |
|--------|----|----|--------|----------|
| 4 | 2158 | Alpha Predator | Merge with beast | 30s |
| 4 | 2159 | Winter Pack | Summon 3 arctic wolves | 30s |

---

## Visual & Sound Effects

### Standard Effect Set (13 abilities)
- **Visual:** 0x36D4 (sparkle/particle effect)
- **Hue:** 0x1E5 (485 decimal - blue/ice)
- **Sound:** 0x481 (1153 decimal)
- **Used By:** All abilities except Mend Pet

### Mend Pet Effect Set (1 ability)
- **Visual:** 0x376A (healing sparkles)
- **Hue:** 0x1F2 (498 decimal - green/healing)
- **Sound:** 0x481 (1153 decimal)
- **Used By:** ID 2147 (Mend Pet)

---

## Mana Cost Progression

| Circle | Mana Cost | Ability Count | Total Mana |
|--------|-----------|---------------|------------|
| 1 | 4 | 4 | 16 |
| 2 | 7 | 4 | 28 |
| 3 | 10 | 4 | 40 |
| 4 | 13 | 4 | 52 |

**Full Rotation Cost:** 136 mana (all 16 abilities once)

---

## Core Mechanics

### Pet Synergy System
- **Pack Tactics:** Bonus when pet is nearby
- **Feral Bond:** Shared senses mechanic
- **Spirit Bond:** HP sharing mechanic
- **Beast Mastery:** +100% pet damage boost

### Summoning Progression
- **Circle 1:** Call bonded pet (permanent)
- **Circle 2:** Summon temporary beast (single)
- **Circle 3:** Summon wolf pack (multiple)
- **Circle 4:** Summon 3 arctic wolves (elite pack)

### Transform System
- **Alpha Predator (C4):** Merge with beast form
- **Winter Pack (C4):** Become pack leader

---

## Strengths

- ✅ **Strong Pet Buffs:** 6 dedicated pet enhancement abilities
- ✅ **Multiple Summons:** 4 summoning abilities (circles 1-4)
- ✅ **Healing Support:** Mend Pet for sustained fights
- ✅ **AoE Damage:** Stampede for multi-target scenarios
- ✅ **Long Buffs:** 60s duration on Circle 3-4 buffs
- ✅ **Transform Options:** 2 high-level transform abilities
- ✅ **Archery Integration:** DEX-focused stats (cap 130)

---

## Weaknesses

- ⚠️ **Low Direct Damage:** Only 3 damage abilities (18.75%)
- ⚠️ **Pet Dependency:** 10/16 abilities require active pet
- ⚠️ **No Mobility:** Zero movement/escape abilities
- ⚠️ **No Defense:** Zero defensive/mitigation abilities
- ⚠️ **Generic Buffs:** All buffs use same effect (+10 All Stats)
- ⚠️ **Limited Healing:** Only 1 heal ability (pet-only)
- ⚠️ **No Self-Buffs:** All buffs appear pet-focused
- ⚠️ **Single Damage Type:** 100% Physical damage only

---

## Known Issues & Placeholder Implementations

### Critical Issues
1. **Generic Buff Effects:** All 6 buff abilities use identical `+10 All Stats, 30-60s` effect
   - No differentiation between Pack Tactics, Feral Bond, Alpha's Command, etc.
   - Beast Mastery description says "+100% damage" but code shows same `+10 All Stats` buff
   - **Impact:** Abilities don't match their thematic descriptions

2. **Summon Abilities Non-Functional:** 4 summon abilities (2144, 2145, 2151, 2152, 2159) have placeholder buffs
   - Call Pet (2144) should summon but applies +10 All Stats buff
   - Growl (2145) should taunt but applies +10 All Stats buff
   - Bestial Call (2151) should summon temporary beast but applies buff
   - Call of the Wild (2152) should summon wolf pack but applies buff
   - Winter Pack (2159) should summon 3 arctic wolves but applies buff
   - **Impact:** Core class fantasy is broken

3. **Transform Abilities Non-Functional:** 1 transform ability (2158) has placeholder buff
   - Alpha Predator (2158) should transform player but applies +10 All Stats buff
   - **Impact:** High-level class fantasy is missing

### Medium Priority Issues
4. **Taunt System Missing:** Growl (2145) should apply threat/aggro mechanic
   - No `WithDebuff` or custom taunt effect defined
   - **Impact:** Tank pet gameplay not functional

5. **HP Sharing Missing:** Spirit Bond (2154) should link HP pools
   - Currently just applies generic +10 All Stats buff
   - No custom damage sharing logic
   - **Impact:** Unique defensive mechanic not implemented

6. **Sense Sharing Missing:** Feral Bond (2149) should share vision/detection
   - Currently just applies generic +10 All Stats buff
   - No custom vision logic
   - **Impact:** Unique utility mechanic not implemented

### Low Priority Issues
7. **Effect Variety:** 13/16 abilities use identical visual/sound (0x36D4, 0x481)
   - Only Mend Pet has unique effect (0x376A)
   - **Impact:** Abilities feel samey visually

8. **Range Uniformity:** 15/16 abilities use 12-tile range (except Stampede AoE)
   - No variety in engagement distances
   - **Impact:** Lacks tactical positioning depth

---

## Implementation Recommendations

### Phase 1: Critical (Core Class Fantasy)
1. **Implement Summoning System**
   - Create temporary creature spawning logic for Bestial Call, Call of the Wild, Winter Pack
   - Add follower slot management and duration tracking
   - Integrate with BeastWhistle item mechanics (ClassSpecialItems.cs reference)

2. **Differentiate Buff Effects**
   - Beast Mastery (2156): Implement +100% pet damage multiplier
   - Primal Rage (2153): Add attack speed or damage bonus
   - Pack Tactics (2146): Range-based proximity bonus
   - Alpha's Command (2150): STR/DEX boost for pet

3. **Implement Transform System**
   - Alpha Predator (2158): Merge player with pet (combine stats, new form)
   - Add transformation body type, stat bonuses, special abilities

### Phase 2: Important (Unique Mechanics)
4. **Add Taunt System**
   - Growl (2145): Implement threat/aggro mechanics
   - Add AI targeting override for pet

5. **Add HP Sharing**
   - Spirit Bond (2154): Link player and pet HP pools
   - Damage splitting logic, synchronized healing

6. **Add Sense Sharing**
   - Feral Bond (2149): Share pet's vision/detection range
   - Reveal hidden enemies near pet

### Phase 3: Enhancement (Polish)
7. **Add Defensive Abilities**
   - Consider pet intercept/shield abilities
   - Pet sacrifice to save player

8. **Add Mobility Options**
   - Pet charge/leap abilities
   - Player teleport to pet

9. **Diversify Effects**
   - Unique visual/sound per ability circle
   - Thematic effects matching ability names

---

## Comparison to Industry Standards (Pet DPS Archetype)

### Expected Distribution
- **Damage:** 25-30% (4-5 abilities) - **Current: 18.75% (3 abilities) ❌ LOW**
- **Pet Buffs:** 30-35% (5-6 abilities) - **Current: 37.5% (6 abilities) ✅ GOOD**
- **Utility:** 20-25% (3-4 abilities) - **Current: 25% (4 abilities) ✅ GOOD**
- **Defense:** 10-15% (1-2 abilities) - **Current: 0% (0 abilities) ❌ MISSING**
- **Mobility:** 5-10% (1 ability) - **Current: 0% (0 abilities) ❌ MISSING**

### Critical Gaps
1. **No Defensive Abilities:** Pet DPS classes typically have 1-2 defensive cooldowns
   - Suggestion: Pet intercept, damage reduction, or shield abilities
2. **No Mobility Abilities:** Most archetypes have at least 1 movement ability
   - Suggestion: Pet charge, player leap to pet, or speed boost
3. **Below-Average Damage:** 18.75% vs industry 25-30%
   - Suggestion: Add 1-2 more direct damage abilities

---

## Related Documentation

- **Class System:** `/ServUO/Scripts/Custom/VystiaClasses/CLAUDE.md`
- **Ability Framework:** `/ServUO/Scripts/Custom/VystiaClasses/Systems/AbilityDefinition.cs`
- **Balance Analysis:** `/Vystia/admin/ARCHETYPE_BALANCE_ANALYSIS.md`

---

**Last Updated:** 2025-12-13
**Status:** Implementation documented, critical issues identified, recommendations provided
**Build Status:** ✅ Compiles successfully (0 errors)
**Functional Status:** ⚠️ Placeholder implementations need replacement
