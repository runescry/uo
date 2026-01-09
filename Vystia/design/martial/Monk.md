# Monk - Current State vs Future State

**Class:** Monk
**Ability IDs:** 2032-2047
**Archetype:** Combo DPS (Martial Artist)
**Theme:** Disciplined martial artist with chi manipulation
**Implementation:** `ServUO/Scripts/Custom/VystiaClasses/Abilities/Generated/Martial/MonkAbilities.cs`

---

## Current State

### Ability Distribution (16 abilities)
- **Damage:** 8 abilities (50%)
- **Buff/Utility:** 7 abilities (43.75%)
- **Healing:** 1 ability (6.25%)
- **Mobility:** 0 abilities (0%) ⚠️

### Implemented Abilities

#### Circle 1 Abilities (Mana Cost: 4)

| ID | Name | Type | Cost | Damage/Effect | Description |
|----|------|------|------|---------------|-------------|
| 2032 | Jab | Damage | 4 | 8-12 Physical | Quick strike, generates Chi |
| 2033 | Tiger Palm | Damage | 4 | 12-18 Physical | Palm strike with knockback |
| 2034 | Roll | Buff | 4 | +10 stats, 30s | Dodge roll, remove roots |
| 2035 | Inner Peace | Buff | 4 | +10 stats, 30s | Mana regen boost |

**Effects:**
- Jab: 0x36D4 (visual), 0x1E5 (effect), 0x481 (sound)
- Tiger Palm: 0x36D4 (visual), 0x1E5 (effect), 0x481 (sound)
- Roll: 0x36D4 (visual), 0x1E5 (effect), 0x481 (sound)

---

#### Circle 2 Abilities (Mana Cost: 7)

| ID | Name | Type | Cost | Damage/Effect | Description |
|----|------|------|------|---------------|-------------|
| 2036 | Flurry of Blows | Damage | 7 | 20-30 Physical | Rapid 5-hit combo |
| 2037 | Clockwork Reflexes | Buff | 7 | +10 stats, 30s | +30% dodge chance |
| 2038 | Stunning Fist | Damage | 7 | 10-15 Physical | Stun for 3s |
| 2039 | Healing Mist | Healing | 7 | 15-25 HP | Heal nearby allies |

**Effects:**
- Flurry of Blows: 0x36D4 (visual), 0x1E5 (effect), 0x481 (sound)
- Stunning Fist: 0x36D4 (visual), 0x1E5 (effect), 0x481 (sound)
- Healing Mist: 0x376A (visual), 0x1F2 (effect), 0x481 (sound)

---

#### Circle 3 Abilities (Mana Cost: 10)

| ID | Name | Type | Cost | Damage/Effect | Description |
|----|------|------|------|---------------|-------------|
| 2040 | Rising Sun Kick | Damage | 10 | 25-40 Physical | Powerful kick with knockup |
| 2041 | Iron Fist | Buff | 10 | +10 stats, 60s | Unarmed = magic damage |
| 2042 | Perfect Balance | Buff | 10 | +10 stats, 60s | Immune to knockdown |
| 2043 | Chi Wave | Damage | 10 | 18-28 Physical | Ranged chi projectile |

**Effects:**
- Rising Sun Kick: 0x36D4 (visual), 0x1E5 (effect), 0x481 (sound)
- Chi Wave: 0x36D4 (visual), 0x1E5 (effect), 0x481 (sound)

---

#### Circle 4 Abilities (Mana Cost: 13)

| ID | Name | Type | Cost | Damage/Effect | Description |
|----|------|------|------|---------------|-------------|
| 2044 | Quivering Palm | Damage | 13 | 50-80 Physical | Delayed massive damage |
| 2045 | Serenity | Buff | 13 | +10 stats, 60s | Immune to CC for 10s |
| 2046 | Touch of Death | Damage | 13 | 100-150 Physical | Execute below 10% HP |
| 2047 | Transcendence | Buff | 13 | +10 stats, 30s | Ethereal form |

**Effects:**
- Quivering Palm: 0x36D4 (visual), 0x1E5 (effect), 0x481 (sound)
- Touch of Death: 0x36D4 (visual), 0x1E5 (effect), 0x481 (sound)
- Transcendence: 0x36D4 (visual), 0x1E5 (effect), 0x481 (sound)

---

### Core Mechanics
- **Chi Generation:** Jab generates Chi resource (placeholder - not fully implemented)
- **Combo System:** Flurry of Blows suggests 5-hit combo mechanics (not implemented)
- **Unarmed Combat:** Iron Fist converts unarmed damage to magic type
- **Knockback/Knockup:** Tiger Palm, Rising Sun Kick (not implemented)
- **Execute Mechanics:** Touch of Death at <10% HP (not implemented)
- **Delayed Damage:** Quivering Palm (not implemented)

### Strengths
- ✅ Strong damage scaling (8-150 damage range)
- ✅ Good buff coverage (7 defensive/utility buffs)
- ✅ Self-healing via Healing Mist
- ✅ Unique execute mechanic (Touch of Death)
- ✅ CC immunity options (Serenity, Perfect Balance)

### Weaknesses
- ⚠️ **Zero mobility abilities** (0/16 abilities)
- ⚠️ No gap closers or movement speed
- ⚠️ Placeholder buff values (+10 stats generic)
- ⚠️ Missing combo mechanics (no Chi resource tracking)
- ⚠️ Knockback/knockup not implemented
- ⚠️ All damage abilities use same visual effects
- ⚠️ No AoE damage abilities
- ⚠️ Execute condition not enforced

---

## Known Issues

### Placeholder Implementations
1. **Generic Buff Effects:** All buffs use `VystiaBuffType.AllStatsBuff` with +10 value
   - Should have specific buff types: DodgeBuff, DamageTypeBuff, CCImmunityBuff, etc.
   - Clockwork Reflexes should add actual dodge mechanics
   - Iron Fist should modify damage type conversion
   - Perfect Balance should add knockdown immunity
   - Serenity should add CC immunity duration

2. **Missing Combo System:**
   - No Chi resource tracking
   - Jab doesn't actually generate Chi
   - No combo point system
   - Flurry of Blows doesn't execute 5 hits

3. **Unimplemented Mechanics:**
   - Tiger Palm knockback not functional
   - Rising Sun Kick knockup not functional
   - Quivering Palm delayed damage not implemented
   - Touch of Death execute condition (<10% HP) not checked
   - Roll doesn't remove roots/snares
   - Transcendence ethereal form not implemented

4. **Missing Target Types:**
   - Roll and Transcendence use `SingleTarget` but should be self-target
   - Healing Mist should have AoE targeting for "nearby allies"

5. **Visual Effects:**
   - Most abilities use same effect (0x36D4)
   - Should have unique chi/martial effects per ability

---

## Future State Proposals

**Date:** 2025-12-13
**Source:** Industry archetype standards (`Vystia/admin/ARCHETYPE_BALANCE_ANALYSIS.md`)

### Target Distribution
Based on **Combo DPS** archetype standards:
- **Damage:** 8-10 abilities (50-62%) - Current: 8 (50%) ✅
- **Utility:** 4-6 abilities (25-37%) - Current: 7 (43%) ✅
- **Mobility:** 2-3 abilities (12-18%) - Current: 0 (0%) ⚠️ **CRITICAL GAP**
- **Healing:** 1-2 abilities (6-12%) - Current: 1 (6%) ✅

### Critical Needs

#### 1. Mobility Abilities (HIGH PRIORITY)
**Missing:** Gap closers, movement speed, teleports

**Recommended Additions:**
- **Flying Kick (Circle 2):** Leap to target, deal damage, generate Chi
- **Zen Flight (Circle 3):** +50% movement speed, cannot be slowed
- **Chi Torpedo (Circle 3):** Roll forward 8 tiles, damaging enemies in path

#### 2. Chi Resource System (HIGH PRIORITY)
**Current:** No Chi tracking, Jab "generates Chi" but doesn't work

**Recommended Implementation:**
- Add `SecondaryResource.Chi` tracking
- Jab generates 1 Chi per hit
- Tiger Palm generates 2 Chi
- Flurry of Blows costs 3 Chi
- Touch of Death costs 5 Chi
- Maximum 5 Chi

#### 3. Combo Mechanics (MEDIUM PRIORITY)
**Current:** No combo point system

**Recommended:**
- Track combo points (0-5)
- Basic attacks (Jab, Tiger Palm) build combo
- Finishers (Flurry, Quivering Palm, Touch of Death) consume combo
- Damage scales with combo points consumed

#### 4. Specific Buff Types (MEDIUM PRIORITY)
Replace generic `AllStatsBuff` with:
- Clockwork Reflexes → `DodgeChanceBuff` (+30% dodge)
- Iron Fist → `DamageConversionBuff` (physical → magic)
- Perfect Balance → `KnockdownImmunityBuff`
- Serenity → `CCImmunityBuff` (10s duration)

### Ability Replacements/Updates

#### Circle 1 Changes
- **Roll:** Change target type to `Self`, implement root/snare removal
- **Inner Peace:** Add mana regen mechanic (current: generic buff)

#### Circle 2 Changes
- **NEW: Flying Kick:** Replace one buff with mobility gap closer
- **Clockwork Reflexes:** Implement actual dodge chance buff
- **Stunning Fist:** Add stun CC effect (3s duration)
- **Healing Mist:** Change to AoE targeting

#### Circle 3 Changes
- **NEW: Zen Flight:** Add movement speed buff ability
- **Iron Fist:** Implement damage type conversion
- **Perfect Balance:** Add knockdown immunity
- **Rising Sun Kick:** Implement knockup CC effect

#### Circle 4 Changes
- **Quivering Palm:** Implement 3s delayed damage bomb
- **Serenity:** Implement CC immunity duration
- **Touch of Death:** Add HP% execution check (<10% HP)
- **Transcendence:** Implement ethereal form (phase through enemies)

### Implementation Priority

**Phase 1 (Critical - Mobility Gap):**
1. Add Flying Kick (Circle 2 gap closer)
2. Add Zen Flight (Circle 3 movement speed)
3. Implement Roll as self-target with root removal

**Phase 2 (Important - Resource System):**
4. Implement Chi resource tracking
5. Update Jab/Tiger Palm to generate Chi
6. Update finishers to consume Chi
7. Add combo point display

**Phase 3 (Enhancement - Mechanics):**
8. Replace generic buffs with specific buff types
9. Implement CC effects (stun, knockback, knockup)
10. Implement execute condition for Touch of Death
11. Implement delayed damage for Quivering Palm
12. Implement ethereal form for Transcendence

**Phase 4 (Polish - Effects):**
13. Create unique visual effects for each ability
14. Add chi particle effects
15. Improve sound effects variety

---

## File Locations

**Implementation File:**
- `ServUO/Scripts/Custom/VystiaClasses/Abilities/Generated/Martial/MonkAbilities.cs`

**Class Definition:**
- `ServUO/Scripts/Custom/VystiaClasses/Classes/MonkClass.cs`

**Special Items:**
- `ServUO/Scripts/Custom/VystiaClasses/Items/ClassSpecialItems.cs` (MonkBeads)

**Related Systems:**
- `ServUO/Scripts/Custom/VystiaClasses/Systems/SecondaryResource.cs` (Chi resource)
- `ServUO/Scripts/Custom/VystiaClasses/Systems/VystiaBuffSystem.cs` (Buff types)
- `ServUO/Scripts/Custom/VystiaClasses/Systems/CrowdControlSystem.cs` (CC effects)

---

## Testing Commands

```
[setclass monk               - Set player class to Monk
[giveability 2032-2047       - Give individual abilities
[vstats                      - View secondary resources (Chi)
[vbuffs                      - View active buffs
```

---

**Last Updated:** 2025-12-28
**Status:** Fully documented from implementation
**Implementation:** 16/16 abilities generated (placeholder mechanics)
**Critical Gap:** Zero mobility abilities (0/16)
