# Divination Magic - Crystal Barrens School

| Property | Value |
|----------|-------|
| **Class** | Oracle |
| **Region** | Crystal Barrens |
| **Theme** | Foresight, crystal magic, energy, time manipulation, fate |
| **Spellbook** | Oracle Spellbook |
| **Hue** | 0x482 (Crystal Blue) |
| **Spell IDs** | 1192-1223 |
| **Status** | 100% Complete (32/32 spells) |

---

## Reagents

All Divination Magic spells use Vystia reagents found in the Crystal Barrens.

| Reagent | Item ID | Circles | Description |
|---------|---------|---------|-------------|
| Time Sand | 0x0F8F | 1-3 | Sand that flows through time |
| Time Dust | 0x0F7D | 2-4 | Dust of past and future |
| Divination Dust | 0x0DE1 | 3-5 | Dust for seeing truth |
| Fate Crystal | 0x0F8E | 4-6 | Crystal showing fate |
| Starlight Crystal | 0x0F0E | 5-7 | Crystal of starlight |
| Prophetic Leaf | 0x1A9C | 6-8 | Leaf showing prophecies |
| Seeing Stone | 0x0F7A | 7-8 | Stone for scrying |
| Fate Thread | 0x0F8D | 8 | Thread of destiny |

---

## Circle 1 - Basic Divination (4 Mana)

### 1. Crystal Dart

| Property | Value |
|----------|-------|
| **File** | DivinationCrystalDartSpell.cs |
| **Words** | "Crystalum Dartum" |
| **Reagents** | Time Sand, Time Dust |
| **Range** | 10 tiles |
| **Target** | Single enemy |

**Effect:**
- Placeholder: +5 STR buff for 1 minute (designed as 5-11 energy damage)

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Target | 0x376A | 0x481 | Crystal particles |
| Sound | 0x1F2 | - | Magic sound |

**Known Issues:**
- Not implemented as damage spell - applies STR buff instead

---

### 2. Glimpse Future

| Property | Value |
|----------|-------|
| **File** | DivinationGlimpseFutureSpell.cs |
| **Words** | "Glimpseum Futureum" |
| **Reagents** | Time Sand, Time Dust |
| **Range** | 10 tiles |
| **Target** | Single enemy |

**Effect:**
- Placeholder: +5 STR buff for 1 minute (designed as "see enemy's next action")

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Target | 0x376A | 0x481 | Crystal particles |
| Sound | 0x1F2 | - | Magic sound |

**Known Issues:**
- Not implemented as foresight mechanic - applies STR buff instead

---

### 3. Minor Ward

| Property | Value |
|----------|-------|
| **File** | DivinationMinorWardSpell.cs |
| **Words** | "Minorum Wardum" |
| **Reagents** | Time Sand, Time Dust |
| **Range** | 10 tiles |
| **Target** | Single enemy |

**Effect:**
- Placeholder: +5 STR buff for 1 minute (designed as +10 damage shield)

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Target | 0x376A | 0x481 | Crystal particles |
| Sound | 0x1F2 | - | Magic sound |

**Known Issues:**
- Not implemented as damage shield - applies STR buff instead

---

### 4. Clarity

| Property | Value |
|----------|-------|
| **File** | DivinationClaritySpell.cs |
| **Words** | "Clarityum" |
| **Reagents** | Time Sand, Time Dust |
| **Range** | 10 tiles |
| **Target** | Single enemy |

**Effect:**
- Placeholder: +5 STR buff for 1 minute (designed as +5 INT for 30s)

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Target | 0x376A | 0x481 | Crystal particles |
| Sound | 0x1F2 | - | Magic sound |

**Known Issues:**
- Applies STR buff instead of INT buff
- Uses harmful target flags (should be beneficial)

---

## Circle 2 - Energy Arts (6 Mana)

### 5. Prismatic Bolt

| Property | Value |
|----------|-------|
| **File** | DivinationPrismaticBoltSpell.cs |
| **Words** | "Prismaticum Sagitta" |
| **Reagents** | Time Dust, Divination Dust |
| **Range** | 10 tiles |
| **Target** | Single enemy |
| **Damage** | 10-15 + (skill * 0.5) energy |

**Effect:**
- Energy damage (100% energy type)
- Rainbow effect (design spec)

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Target | 0x376A | 0x481 | Crystal particles |
| Sound | 0x1F2 | - | Magic sound |

---

### 6. Foresight

| Property | Value |
|----------|-------|
| **File** | DivinationForesightSpell.cs |
| **Words** | "Foresightum" |
| **Reagents** | Time Dust, Divination Dust |
| **Range** | 10 tiles |
| **Target** | Single enemy |

**Effect:**
- Placeholder: +8 STR buff for 2 minutes (designed as "next attack misses")

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Target | 0x376A | 0x481 | Crystal particles |
| Sound | 0x1F2 | - | Magic sound |

**Known Issues:**
- Not implemented as dodge mechanic - applies STR buff instead

---

### 7. Crystal Shield

| Property | Value |
|----------|-------|
| **File** | DivinationCrystalShieldSpell.cs |
| **Words** | "Crystalum Shieldum" |
| **Reagents** | Time Dust, Divination Dust |
| **Range** | 10 tiles |
| **Target** | Single enemy |

**Effect:**
- Placeholder: +8 STR buff for 2 minutes (designed as 30 damage absorb shield)

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Target | 0x376A | 0x481 | Crystal particles |
| Sound | 0x1F2 | - | Magic sound |

**Known Issues:**
- Not implemented as absorb shield - applies STR buff instead

---

### 8. Haste Self

| Property | Value |
|----------|-------|
| **File** | DivinationHasteSelfSpell.cs |
| **Words** | "Hasteum Selfum" |
| **Reagents** | Time Dust, Divination Dust |
| **Range** | 10 tiles |
| **Target** | Single enemy |

**Effect:**
- Placeholder: +8 STR buff for 2 minutes (designed as +20% speed for 20s)

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Target | 0x376A | 0x481 | Crystal particles |
| Sound | 0x1F2 | - | Magic sound |

**Known Issues:**
- Not implemented as haste - applies STR buff instead

---

## Circle 3 - Area Energy (9 Mana)

### 9. Energy Burst

| Property | Value |
|----------|-------|
| **File** | DivinationEnergyBurstSpell.cs |
| **Words** | "Energyum Burstum" |
| **Reagents** | Divination Dust, Fate Crystal |
| **Range** | 10 tiles |
| **Target** | Single enemy |

**Effect:**
- Placeholder: +10 STR buff for 3 minutes (designed as 3-tile AoE, 15-26 energy damage)

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Area | 0x376A | 0x481 | Location particles |
| Sound | 0x1F2 | - | Magic sound |

**Known Issues:**
- Not implemented as AoE - applies single-target STR buff instead

---

### 10. Precognition

| Property | Value |
|----------|-------|
| **File** | DivinationPrecognitionSpell.cs |
| **Words** | "Precognitionum" |
| **Reagents** | Divination Dust, Fate Crystal |
| **Range** | 10 tiles |
| **Target** | Single enemy |

**Effect:**
- Placeholder: +10 STR buff for 3 minutes (designed as detect hidden/traps)

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Target | 0x376A | 0x481 | Crystal particles |
| Sound | 0x1F2 | - | Magic sound |

**Known Issues:**
- Not implemented as detection - applies STR buff instead

---

### 11. Temporal Slow

| Property | Value |
|----------|-------|
| **File** | DivinationTemporalSlowSpell.cs |
| **Words** | "Temporalum Slowum" |
| **Reagents** | Divination Dust, Fate Crystal |
| **Range** | 10 tiles |
| **Target** | Single enemy |

**Effect:**
- Placeholder: +10 STR buff for 3 minutes (designed as -40% speed for 12s)

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Target | 0x376A | 0x481 | Crystal particles |
| Sound | 0x1F2 | - | Magic sound |

**Known Issues:**
- Not implemented as slow - applies STR buff instead

---

### 12. Mana Crystal

| Property | Value |
|----------|-------|
| **File** | DivinationManaCrystalSpell.cs |
| **Words** | "Manuum Crystalum" |
| **Reagents** | Divination Dust, Fate Crystal |
| **Range** | 10 tiles |
| **Target** | Single enemy |

**Effect:**
- Placeholder: +10 STR buff for 3 minutes (designed as create mana crystal item, restore 25 mana)

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Target | 0x376A | 0x481 | Crystal particles |
| Sound | 0x1F2 | - | Magic sound |

**Known Issues:**
- Not implemented as item creation - applies STR buff instead

---

## Circle 4 - Advanced Energy (11 Mana)

### 13. Prismatic Spray

| Property | Value |
|----------|-------|
| **File** | DivinationPrismaticSpraySpell.cs |
| **Words** | "Prismaticum Sprayum" |
| **Reagents** | Fate Crystal, Starlight Crystal |
| **Range** | 10 tiles |
| **Target** | Single enemy |

**Effect:**
- Placeholder: +12 STR buff for 4 minutes (designed as cone, 18-32 damage, random status)

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Target | 0x376A | 0x481 | Crystal particles |
| Sound | 0x1F2 | - | Magic sound |

**Known Issues:**
- Not implemented as cone AoE - applies STR buff instead

---

### 14. Time Warp

| Property | Value |
|----------|-------|
| **File** | DivinationTimeWarpSpell.cs |
| **Words** | "Timeus Warpum" |
| **Reagents** | Fate Crystal, Starlight Crystal |
| **Range** | 10 tiles |
| **Target** | Single enemy |

**Effect:**
- Placeholder: +12 STR buff for 4 minutes (designed as rewind to position 3s ago)

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Target | 0x376A | 0x481 | Crystal particles |
| Sound | 0x1F2 | - | Magic sound |

**Known Issues:**
- Not implemented as time rewind - applies STR buff instead

---

### 15. Barrier of Light

| Property | Value |
|----------|-------|
| **File** | DivinationBarrierofLightSpell.cs |
| **Words** | "Barrierum Lightum" |
| **Reagents** | Fate Crystal, Starlight Crystal |
| **Range** | 10 tiles |
| **Target** | Single enemy |

**Effect:**
- Placeholder: +12 STR buff for 4 minutes (designed as party shield, absorb 50 damage each)

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Target | 0x376A | 0x481 | Crystal particles |
| Sound | 0x1F2 | - | Magic sound |

**Known Issues:**
- Not implemented as party shield - applies single-target STR buff instead

---

### 16. Oracle's Sight

| Property | Value |
|----------|-------|
| **File** | DivinationOraclesSightSpell.cs |
| **Words** | "Oracleum Sightum" |
| **Reagents** | Fate Crystal, Starlight Crystal |
| **Range** | 10 tiles |
| **Target** | Single enemy |

**Effect:**
- Placeholder: +12 STR buff for 4 minutes (designed as see all enemies 25 tiles, show HP/mana)

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Target | 0x376A | 0x481 | Crystal particles |
| Sound | 0x1F2 | - | Magic sound |

**Known Issues:**
- Not implemented as detection - applies STR buff instead

---

## Circle 5 - Mass Energy (14 Mana)

### 17. Crystal Barrage

| Property | Value |
|----------|-------|
| **File** | DivinationCrystalBarrageSpell.cs |
| **Words** | "Crystalum Barragem" |
| **Reagents** | Starlight Crystal, Prophetic Leaf |
| **Range** | 10 tiles |
| **Target** | Single enemy |

**Effect:**
- Placeholder: +15 STR buff for 5 minutes (designed as 5-tile AoE, 25-42 energy damage)

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Target | 0x376A | 0x481 | Crystal particles |
| Sound | 0x1F2 | - | Magic sound |

**Known Issues:**
- Not implemented as AoE - applies STR buff instead

---

### 18. Fate's Thread

| Property | Value |
|----------|-------|
| **File** | DivinationFatesThreadSpell.cs |
| **Words** | "Fateum Threadum" |
| **Reagents** | Starlight Crystal, Prophetic Leaf |
| **Range** | 10 tiles |
| **Target** | Single enemy |

**Effect:**
- Placeholder: +15 STR buff for 5 minutes (designed as link two allies, share buffs/damage)

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Target | 0x376A | 0x481 | Crystal particles |
| Sound | 0x1F2 | - | Magic sound |

**Known Issues:**
- Not implemented as ally link - applies STR buff instead

---

### 19. Mass Haste

| Property | Value |
|----------|-------|
| **File** | DivinationMassHasteSpell.cs |
| **Words** | "Masseum Hasteum" |
| **Reagents** | Starlight Crystal, Prophetic Leaf |
| **Range** | 10 tiles |
| **Target** | Single enemy |

**Effect:**
- Placeholder: +15 STR buff for 5 minutes (designed as party +25% speed for 30s)

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Target | 0x376A | 0x481 | Crystal particles |
| Sound | 0x1F2 | - | Magic sound |

**Known Issues:**
- Not implemented as party haste - applies single-target STR buff instead

---

### 20. Temporal Shield

| Property | Value |
|----------|-------|
| **File** | DivinationTemporalShieldSpell.cs |
| **Words** | "Temporalum Shieldum" |
| **Reagents** | Starlight Crystal, Prophetic Leaf |
| **Range** | 10 tiles |
| **Target** | Single enemy |

**Effect:**
- Placeholder: +15 STR buff for 5 minutes (designed as rewind damage, immune 1s after hit)

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Target | 0x376A | 0x481 | Crystal particles |
| Sound | 0x1F2 | - | Magic sound |

**Known Issues:**
- Not implemented as temporal shield - applies STR buff instead

---

## Circle 6 - Greater Energy (20 Mana)

### 21. Prismatic Storm

| Property | Value |
|----------|-------|
| **File** | DivinationPrismaticStormSpell.cs |
| **Words** | "Prismaticum Stormum" |
| **Reagents** | Prophetic Leaf, Seeing Stone |
| **Range** | 10 tiles |
| **Target** | Single enemy |
| **Damage** | 30-40 + (skill * 0.5) energy |

**Effect:**
- Energy damage (100% energy type)
- Design: 8-tile radius AoE, rainbow explosions

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Area | 0x376A | 0x481 | Location particles |
| Sound | 0x1F2 | - | Magic sound |

**Known Issues:**
- Not implemented as AoE - single-target damage only

---

### 22. Prophecy of Doom

| Property | Value |
|----------|-------|
| **File** | DivinationProphecyofDoomSpell.cs |
| **Words** | "Prophecyum Doomum" |
| **Reagents** | Prophetic Leaf, Seeing Stone |
| **Range** | 10 tiles |
| **Target** | Single enemy |

**Effect:**
- Placeholder: +20 STR buff for 6 minutes (designed as mark, allies +30% damage)

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Target | 0x376A | 0x481 | Crystal particles |
| Sound | 0x1F2 | - | Magic sound |

**Known Issues:**
- Not implemented as damage amplification - applies STR buff instead

---

### 23. Crystal Fortress

| Property | Value |
|----------|-------|
| **File** | DivinationCrystalFortressSpell.cs |
| **Words** | "Crystalum Fortressum" |
| **Reagents** | Prophetic Leaf, Seeing Stone |
| **Range** | 10 tiles |
| **Target** | Single enemy |

**Effect:**
- Placeholder: +20 STR buff for 6 minutes (designed as AoE shield, 200 HP absorb, reflect damage)

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Target | 0x376A | 0x481 | Crystal particles |
| Sound | 0x1F2 | - | Magic sound |

**Known Issues:**
- Not implemented as AoE shield - applies STR buff instead

---

### 24. Time Stop

| Property | Value |
|----------|-------|
| **File** | DivinationTimeStopSpell.cs |
| **Words** | "Timeus Stopum" |
| **Reagents** | Prophetic Leaf, Seeing Stone |
| **Range** | 10 tiles |
| **Target** | Single enemy |

**Effect:**
- Placeholder: +20 STR buff for 6 minutes (designed as freeze enemy 5s, complete disable)

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Target | 0x376A | 0x481 | Crystal particles |
| Sound | 0x1F2 | - | Magic sound |

**Known Issues:**
- Not implemented as paralyze - applies STR buff instead

---

## Circle 7 - Master Divination (40 Mana)

### 25. Cosmic Rift

| Property | Value |
|----------|-------|
| **File** | DivinationCosmicRiftSpell.cs |
| **Words** | "Cosmicum Riftum" |
| **Reagents** | Seeing Stone, Fate Thread |
| **Range** | 10 tiles |
| **Target** | Single enemy |

**Effect:**
- Placeholder: +22 STR buff for 7 minutes (designed as rift, pulls enemies, 55-90 damage)

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Target | 0x376A | 0x481 | Crystal particles |
| Sound | 0x1F2 | - | Magic sound |

**Known Issues:**
- Not implemented as rift/pull - applies STR buff instead

---

### 26. Fate Shift

| Property | Value |
|----------|-------|
| **File** | DivinationFateShiftSpell.cs |
| **Words** | "Fateum Shiftum" |
| **Reagents** | Seeing Stone, Fate Thread |
| **Range** | 10 tiles |
| **Target** | Single enemy |

**Effect:**
- Placeholder: +22 STR buff for 7 minutes (designed as swap HP/mana with target)

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Target | 0x376A | 0x481 | Crystal particles |
| Sound | 0x1F2 | - | Magic sound |

**Known Issues:**
- Not implemented as stat swap - applies STR buff instead

---

### 27. Mass Foresight

| Property | Value |
|----------|-------|
| **File** | DivinationMassForesightSpell.cs |
| **Words** | "Masseum Foresightum" |
| **Reagents** | Seeing Stone, Fate Thread |
| **Range** | 10 tiles |
| **Target** | Single enemy |

**Effect:**
- Placeholder: +22 STR buff for 7 minutes (designed as party "avoid next attack" buff)

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Target | 0x376A | 0x481 | Crystal particles |
| Sound | 0x1F2 | - | Magic sound |

**Known Issues:**
- Not implemented as party dodge - applies single-target STR buff instead

---

### 28. Chrono Lord

| Property | Value |
|----------|-------|
| **File** | DivinationChronoLordSpell.cs |
| **Words** | "Chronoum Lordum" |
| **Reagents** | Seeing Stone, Fate Thread |
| **Range** | 10 tiles |
| **Target** | Single enemy |

**Effect:**
- Placeholder: +22 STR buff for 7 minutes (designed as control time, rewind cooldowns, double spell speed)

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Target | 0x376A | 0x481 | Crystal particles |
| Sound | 0x1F2 | - | Magic sound |

**Known Issues:**
- Not implemented as time control - applies STR buff instead

---

## Circle 8 - Legendary Divination (50 Mana)

### 29. Prismatic Apocalypse

| Property | Value |
|----------|-------|
| **File** | DivinationPrismaticApocalypseSpell.cs |
| **Words** | "Prismaticum Apocalypseum" |
| **Reagents** | Seeing Stone, Fate Thread (x2) |
| **Range** | 10 tiles |
| **Target** | Single enemy |

**Effect:**
- Placeholder: +25 STR buff for 8 minutes (designed as screen-wide, 80-145 energy damage, random status)

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Target | 0x376A | 0x481 | Crystal particles |
| Sound | 0x1F2 | - | Magic sound |

**Known Issues:**
- Not implemented as screen-wide AoE - applies STR buff instead

---

### 30. Crystal Maze

| Property | Value |
|----------|-------|
| **File** | DivinationCrystalMazeSpell.cs |
| **Words** | "Crystalum Mazeum" |
| **Reagents** | Seeing Stone, Fate Thread (x2) |
| **Range** | 10 tiles |
| **Target** | Single enemy |

**Effect:**
- Placeholder: +25 STR buff for 8 minutes (designed as trap in crystal labyrinth, 20 tiles, 45s)

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Target | 0x376A | 0x481 | Crystal particles |
| Sound | 0x1F2 | - | Magic sound |

**Known Issues:**
- Not implemented as maze - applies STR buff instead

---

### 31. Timeless State

| Property | Value |
|----------|-------|
| **File** | DivinationTimelessStateSpell.cs |
| **Words** | "Timelessum Stateum" |
| **Reagents** | Seeing Stone, Fate Thread (x2) |
| **Range** | 10 tiles |
| **Target** | Single enemy |

**Effect:**
- Placeholder: +25 STR buff for 8 minutes (designed as party immune to damage 10s)

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Target | 0x376A | 0x481 | Crystal particles |
| Sound | 0x1F2 | - | Magic sound |

**Known Issues:**
- Not implemented as immunity - applies STR buff instead

---

### 32. Oracle Ascendant

| Property | Value |
|----------|-------|
| **File** | DivinationOracleAscendantSpell.cs |
| **Words** | "Oracleus Ascendantum" |
| **Reagents** | Seeing Stone, Fate Thread (x2) |
| **Range** | 10 tiles |
| **Target** | Single enemy |

**Effect:**
- Placeholder: +25 STR buff for 15 minutes (designed as auto-dodge, instant cast, perfect accuracy)

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Target | 0x376A | 0x481 | Crystal particles |
| Sound | 0x1F2 | - | Magic sound |

**Known Issues:**
- Not implemented as ascendant state - applies STR buff instead

---

## Known Issues Summary

**Critical - All Spells:**
- All 32 spells currently apply placeholder STR buffs instead of designed effects
- All spells use harmful target flags (TargetFlags.Harmful) even for buffs/healing
- No actual divination mechanics implemented (foresight, time manipulation, shields)

**Spell-Specific Issues:**
1. **Circle 1-2:** Basic spells should be beneficial (Clarity, Foresight, Crystal Shield, Haste Self)
2. **Energy Damage:** Only Prismatic Bolt (#5) and Prismatic Storm (#21) deal actual damage
3. **AoE Spells:** Energy Burst, Prismatic Spray, Crystal Barrage - not implemented as AoE
4. **Time Mechanics:** Time Warp, Time Stop, Temporal Shield - not functional
5. **Detection:** Oracle's Sight, Precognition - not implemented
6. **Party Buffs:** Mass Haste, Mass Foresight, Barrier of Light, Timeless State - single-target only
7. **Advanced Mechanics:** Fate's Thread (ally link), Fate Shift (stat swap), Crystal Maze - not functional

---

## File Locations

**Implementation:**
- Spell Files: `D:\UO\ServUO\Scripts\Custom\VystiaClasses\Spells\Oracle\`
- Reagents: `D:\UO\ServUO\Scripts\Items\Vystia\Resources\Reagents\DivinationMagicReagents.cs`
- Scrolls: `D:\UO\ServUO\Scripts\Items\Vystia\Scrolls\DivinationScrolls.cs`
- Spellbook: `D:\UO\ServUO\Scripts\Items\Equipment\Spellbooks\VystiaSpellbooks.cs`

**Documentation:**
- This file: `D:\UO\Vystia\design\magic\DivinationMagic.md`
- Complete spell list: `D:\UO\Vystia\reference\SPELLS.md`

---

## Implementation Notes

**Common Effect IDs Used:**
- 0x376A - Crystal particles (most spells)
- 0x481 - Crystal blue hue
- 0x1F2 - Magic sound effect

**Mana Costs by Circle:**
| Circle | Designed | Actual (in comments) |
|--------|----------|---------------------|
| 1 | 4 | 4 |
| 2 | 6 | 8 |
| 3 | 9 | 12 |
| 4 | 11 | 16 |
| 5 | 14 | 20 |
| 6 | 20 | 24 |
| 7 | 40 | 28 |
| 8 | 50 | 32 |

**Skills:**
- All spells use `Caster.Skills.Conjuration.Value` for scaling (skill ID needs verification)
- Should use Divination custom skill when implemented

---

*Last Updated: 2025-12-28*
*Status: Documentation matches actual implementation - all spells compile but use placeholder effects*
