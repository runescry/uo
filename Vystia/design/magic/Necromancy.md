# Necromancy - ShadowVoid School

| Property | Value |
|----------|-------|
| **Class** | Necromancer |
| **Region** | ShadowVoid |
| **Theme** | Death, undead, bone, decay, soul manipulation |
| **Spellbook** | Necronomicon (VystiaNecromancerSpellbook) |
| **Hue** | 0x481 (Deathly Black/Purple) |
| **Spell IDs** | 1224-1255 |
| **Status** | 100% Complete (32/32 spells) |

---

## Reagents

All Necromancy spells use Vystia reagents found in ShadowVoid crypts.

| Reagent | Item ID | Circles | Description |
|---------|---------|---------|-------------|
| Grave Moss | 0x0F7B | 1-3 | Moss from graves |
| Bone Dust | 0x0F8F | 2-4 | Ground bone powder |
| Corpse Ash | 0x0DE1 | 3-5 | Ash from cremated dead |
| Soul Fragment | 0x0F0E | 4-6 | Fragment of captured soul |
| Necrotic Shroud | 0x1422 | 5-7 | Cloth from the dead |
| Lich Dust | 0x0F86 | 6-8 | Dust from lich phylactery |
| Phylactery Shard | 0x0F8A | 7-8 | Shard of phylactery |
| Reaper Essence | 0x0F7D | 8 | Essence of death itself |

---

## Circle 1 - Basic Death Magic (4 Mana)

### 1. Death Bolt

| Property | Value |
|----------|-------|
| **File** | NecromancyDeathBoltSpell.cs |
| **Words** | "Mors Sagitta" |
| **Reagents** | Grave Moss, Bone Dust |
| **Range** | 10 tiles |
| **Target** | Single enemy |
| **Damage** | 5-10 + (Conjuration × 0.5) |

**Effect:**
- Basic ranged death damage (100% energy)
- Supports spell reflection

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Target | 0x376A | 0x481 | Purple particles at waist |
| Sound | 0x1F2 | - | Death bolt sound |

**Implementation:**
- ✅ PLACEHOLDER: Uses energy damage (should be decay/poison mix)
- ✅ Skill: Conjuration (should be Necromancy custom skill)

---

### 2. Animate Bone

| Property | Value |
|----------|-------|
| **File** | NecromancyAnimateBoneSpell.cs |
| **Words** | "Anim Ossus" |
| **Reagents** | Grave Moss, Bone Dust |
| **Range** | 10 tiles |
| **Target** | Single target |
| **Effect Type** | STR Buff (placeholder) |

**Effect:**
- ✅ PLACEHOLDER: Currently applies STR buff
- ❌ NOT IMPLEMENTED: Corpse-based skeleton raising
- Designed: Should raise skeleton from corpse (150 HP)

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Target | 0x376A | 0x481 | Purple particles |
| Sound | 0x1F2 | - | Spell sound |

**Implementation:**
- ⚠️ NEEDS WORK: Requires corpse detection system
- ⚠️ NEEDS WORK: Requires undead summon creatures
- ⚠️ NEEDS WORK: Requires summon limit manager (max 5)

---

### 3. Life Siphon

| Property | Value |
|----------|-------|
| **File** | NecromancyLifeSiphonSpell.cs |
| **Words** | "Vita Siphonum" |
| **Reagents** | Grave Moss, Bone Dust |
| **Range** | 10 tiles |
| **Target** | Single enemy |
| **Healing** | 0 (placeholder) |

**Effect:**
- ✅ PLACEHOLDER: Currently heals 0 HP
- ❌ NOT IMPLEMENTED: Life drain mechanics
- Designed: Should drain 8-15 HP and heal caster

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Target | 0x376A | 0x481 | Purple particles |
| Sound | 0x1F2 | - | Spell sound |

**Implementation:**
- ⚠️ NEEDS WORK: Requires damage + heal combo
- ⚠️ NEEDS WORK: Should use Hex Magic LifeDrainHelper pattern

---

### 4. Deathsight

| Property | Value |
|----------|-------|
| **File** | NecromancyDeathsightSpell.cs |
| **Words** | "Mortis Visus" |
| **Reagents** | Grave Moss, Bone Dust |
| **Range** | 10 tiles |
| **Target** | Self or ally |
| **Duration** | 5 minutes |

**Effect:**
- ✅ PLACEHOLDER: +15 STR buff
- ❌ NOT IMPLEMENTED: Darkness vision
- ❌ NOT IMPLEMENTED: Undead detection
- Designed: See in darkness, detect undead

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Target | 0x376A | 0x481 | Purple particles |
| Sound | 0x1F2 | - | Spell sound |

---

## Circle 2 - Bone Magic (6 Mana)

### 5. Bone Shard

| Property | Value |
|----------|-------|
| **File** | NecromancyBoneShardSpell.cs |
| **Words** | "Ossus Fragmentum" |
| **Reagents** | Bone Dust, Corpse Ash |
| **Range** | 10 tiles |
| **Target** | Single enemy |
| **Damage** | 8-13 + (Conjuration × 0.5) |

**Effect:**
- Ranged bone damage (100% energy)
- Designed: Should pierce armor

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Target | 0x376A | 0x481 | Purple particles |
| Sound | 0x1F2 | - | Bone shard sound |

---

### 6. Decay

| Property | Value |
|----------|-------|
| **File** | NecromancyDecaySpell.cs |
| **Words** | "Mortis Corruptus" |
| **Reagents** | Bone Dust, Corpse Ash |
| **Range** | 10 tiles |
| **Target** | Single enemy |
| **Duration** | 3 minutes |

**Effect:**
- ✅ PLACEHOLDER: +10 STR buff
- ❌ NOT IMPLEMENTED: DoT (4-8 decay/tick)
- ❌ NOT IMPLEMENTED: Max HP reduction
- Designed: Decay DoT that reduces max HP

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Target | 0x376A | 0x481 | Purple particles |
| Sound | 0x1F2 | - | Spell sound |

---

### 7. Raise Zombie

| Property | Value |
|----------|-------|
| **File** | NecromancyRaiseZombieSpell.cs |
| **Words** | "Anim Mortus" |
| **Reagents** | Bone Dust, Corpse Ash |
| **Range** | 10 tiles |
| **Target** | Single target |
| **Duration** | 3 minutes |

**Effect:**
- ✅ PLACEHOLDER: +10 STR buff
- ❌ NOT IMPLEMENTED: Zombie summon
- Designed: Raise zombie from corpse (300 HP, tanky)

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Target | 0x376A | 0x481 | Purple particles |
| Sound | 0x1F2 | - | Spell sound |

---

### 8. Soul Shield

| Property | Value |
|----------|-------|
| **File** | NecromancySoulShieldSpell.cs |
| **Words** | "Spiritus Scutum" |
| **Reagents** | Bone Dust, Corpse Ash |
| **Range** | 10 tiles |
| **Target** | Self or ally |
| **Duration** | 3 minutes |

**Effect:**
- ✅ PLACEHOLDER: +10 STR buff
- ❌ NOT IMPLEMENTED: Damage absorption (25 HP)
- ❌ NOT IMPLEMENTED: Heal on break
- Designed: Absorbs 25 damage, heals when broken

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Target | 0x376A | 0x481 | Purple particles |
| Sound | 0x1F2 | - | Spell sound |

---

## Circle 3 - Advanced Death Magic (9 Mana)

### 9. Death Coil

| Property | Value |
|----------|-------|
| **File** | NecromancyDeathCoilSpell.cs |
| **Words** | "Mortis Spiralus" |
| **Reagents** | Corpse Ash, Soul Fragment |
| **Range** | 10 tiles |
| **Target** | Single target |
| **Duration** | 4 minutes |

**Effect:**
- ✅ PLACEHOLDER: +12 STR buff
- ❌ NOT IMPLEMENTED: Damage to living (20-32)
- ❌ NOT IMPLEMENTED: Heal undead ally
- Designed: Smart targeting (damage OR heal)

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Target | 0x376A | 0x481 | Purple particles |
| Sound | 0x1F2 | - | Spell sound |

---

### 10. Bone Armor

| Property | Value |
|----------|-------|
| **File** | NecromancyBoneArmorSpell.cs |
| **Words** | "Ossus Armatura" |
| **Reagents** | Corpse Ash, Soul Fragment |
| **Range** | 10 tiles |
| **Target** | Self or ally |
| **Duration** | 4 minutes |

**Effect:**
- ✅ PLACEHOLDER: +12 STR buff
- ❌ NOT IMPLEMENTED: +12 Physical Resist
- ❌ NOT IMPLEMENTED: Damage reflection spikes
- Designed: Bone spikes reflect melee damage

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Target | 0x376A | 0x481 | Purple particles |
| Sound | 0x1F2 | - | Spell sound |

---

### 11. Mass Raise

| Property | Value |
|----------|-------|
| **File** | NecromancyMassRaiseSpell.cs |
| **Words** | "Anim Multi Ossus" |
| **Reagents** | Corpse Ash, Soul Fragment |
| **Range** | 10 tiles |
| **Area** | 5 tiles (corpse search) |
| **Duration** | 4 minutes |

**Effect:**
- ✅ PLACEHOLDER: +12 STR buff
- ❌ NOT IMPLEMENTED: Multi-corpse raising
- Designed: Raise 3 skeletons from nearby corpses

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Target | 0x376A | 0x481 | Purple particles |
| Sound | 0x1F2 | - | Spell sound |

---

### 12. Soul Harvest

| Property | Value |
|----------|-------|
| **File** | NecromancySoulHarvestSpell.cs |
| **Words** | "Spiritus Collectum" |
| **Reagents** | Corpse Ash, Soul Fragment |
| **Range** | 10 tiles |
| **Area** | AoE (design unclear) |
| **Duration** | 4 minutes |

**Effect:**
- ✅ PLACEHOLDER: +12 STR buff
- ❌ NOT IMPLEMENTED: AoE life drain
- Designed: Drain 10-18 HP from each enemy in area

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Target | 0x376A | 0x481 | Purple particles |
| Sound | 0x1F2 | - | Spell sound |

---

## Circle 4 - Master Necromancy (11 Mana)

### 13. Skeletal Mage

| Property | Value |
|----------|-------|
| **File** | NecromancySkeletalMageSpell.cs |
| **Words** | "Ossus Magus" |
| **Reagents** | Soul Fragment, Necrotic Shroud |
| **Range** | 10 tiles |
| **Target** | Corpse |
| **Duration** | 5 minutes |

**Effect:**
- ✅ PLACEHOLDER: +13 STR buff
- ❌ NOT IMPLEMENTED: Skeletal mage summon
- Designed: Raise skeletal mage (400 HP, casts death bolts)

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Target | 0x376A | 0x481 | Purple particles |
| Sound | 0x1F2 | - | Spell sound |

---

### 14. Corpse Explosion

| Property | Value |
|----------|-------|
| **File** | NecromancyCorpseExplosionSpell.cs |
| **Words** | "Corpseus Explosionum" |
| **Reagents** | Soul Fragment, Necrotic Shroud |
| **Range** | 10 tiles |
| **Area** | Location AoE |
| **Damage** | 20-25 + (Conjuration × 0.5) |

**Effect:**
- Ranged damage (100% energy)
- ❌ NOT IMPLEMENTED: Corpse requirement
- ❌ NOT IMPLEMENTED: True AoE blast
- Designed: Explode corpse for 25-45 AoE damage (4 tile radius)

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Location | 0x376A | 0x481 | Purple explosion particles |
| Sound | 0x1F2 | - | Explosion sound |

---

### 15. Death Grip

| Property | Value |
|----------|-------|
| **File** | NecromancyDeathGripSpell.cs |
| **Words** | "Mortis Tractum" |
| **Reagents** | Soul Fragment, Necrotic Shroud |
| **Range** | 10 tiles |
| **Target** | Single enemy |
| **Duration** | 5 minutes |

**Effect:**
- ✅ PLACEHOLDER: +13 STR buff
- ❌ NOT IMPLEMENTED: Pull enemy to caster
- ❌ NOT IMPLEMENTED: Damage (15-25)
- Designed: Pull + damage combo

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Target | 0x376A | 0x481 | Purple particles |
| Sound | 0x1F2 | - | Spell sound |

---

### 16. Vampiric Touch

| Property | Value |
|----------|-------|
| **File** | NecromancyVampiricTouchSpell.cs |
| **Words** | "Sanguis Tactus" |
| **Reagents** | Soul Fragment, Necrotic Shroud |
| **Range** | 10 tiles |
| **Target** | Single enemy |
| **Duration** | 5 minutes |

**Effect:**
- ✅ PLACEHOLDER: +13 STR buff
- ❌ NOT IMPLEMENTED: Life drain (20-35 HP)
- ❌ NOT IMPLEMENTED: Heal for 150% of damage
- Designed: Powerful life drain with amplified healing

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Target | 0x376A | 0x481 | Purple particles |
| Sound | 0x1F2 | - | Spell sound |

---

## Circle 5 - Elite Necromancy (14 Mana)

### 17. Bone Wall

| Property | Value |
|----------|-------|
| **File** | NecromancyBoneWallSpell.cs |
| **Words** | "Ossus Murum" |
| **Reagents** | Necrotic Shroud, Lich Dust |
| **Range** | 10 tiles |
| **Area** | Location |
| **Duration** | 5 minutes |

**Effect:**
- ✅ PLACEHOLDER: +15 STR buff
- ❌ NOT IMPLEMENTED: Bone wall creation
- Designed: Wall of bones (blocks movement, 150 HP)

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Location | 0x376A | 0x481 | Purple particles |
| Sound | 0x1F2 | - | Spell sound |

---

### 18. Death and Decay

| Property | Value |
|----------|-------|
| **File** | NecromancyDeathandDecaySpell.cs |
| **Words** | "Mors Andum Decayum" |
| **Reagents** | Necrotic Shroud, Lich Dust |
| **Range** | 10 tiles |
| **Area** | Location AoE |
| **Duration** | 5 minutes |

**Effect:**
- ✅ PLACEHOLDER: +15 STR buff
- ❌ NOT IMPLEMENTED: Ground DoT (8-14 decay/tick for 12s)
- Designed: Creates persistent decay zone

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Location | 0x376A | 0x481 | Purple particles |
| Sound | 0x1F2 | - | Spell sound |

**Implementation:**
- ⚠️ NEEDS WORK: Requires DeathAndDecayItem persistent ground effect
- ⚠️ NEEDS WORK: Requires tick timer (2s intervals)

---

### 19. Raise Bone Golem

| Property | Value |
|----------|-------|
| **File** | NecromancyRaiseBoneGolemSpell.cs |
| **Words** | "Anim Magnus Ossus" |
| **Reagents** | Necrotic Shroud, Lich Dust |
| **Range** | 10 tiles |
| **Target** | Corpse |
| **Duration** | 5 minutes |

**Effect:**
- ✅ PLACEHOLDER: +15 STR buff
- ❌ NOT IMPLEMENTED: Bone golem summon
- Designed: Summon bone construct (900 HP, high damage, taunt)

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Target | 0x376A | 0x481 | Purple particles |
| Sound | 0x1F2 | - | Spell sound |

---

### 20. Soul Link

| Property | Value |
|----------|-------|
| **File** | NecromancySoulLinkSpell.cs |
| **Words** | "Spiritus Vinculum" |
| **Reagents** | Necrotic Shroud, Lich Dust |
| **Range** | 10 tiles |
| **Target** | Ally |
| **Duration** | 5 minutes |

**Effect:**
- ✅ PLACEHOLDER: +15 STR buff
- ❌ NOT IMPLEMENTED: Soul link (share damage/healing)
- Designed: Link souls with ally

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Target | 0x376A | 0x481 | Purple particles |
| Sound | 0x1F2 | - | Spell sound |

---

## Circle 6 - Legendary Necromancy (20 Mana)

### 21. Death Knights

| Property | Value |
|----------|-------|
| **File** | NecromancyDeathKnightsSpell.cs |
| **Words** | "Mortis Equites" |
| **Reagents** | Lich Dust, Phylactery Shard |
| **Range** | 10 tiles |
| **Target** | Area |
| **Duration** | 7 minutes |

**Effect:**
- ✅ PLACEHOLDER: +18 STR buff
- ❌ NOT IMPLEMENTED: Death knight summons
- Designed: Summon 2 death knights (700 HP each, mounted)

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Target | 0x376A | 0x481 | Purple particles |
| Sound | 0x1F2 | - | Spell sound |

---

### 22. Plague Cloud

| Property | Value |
|----------|-------|
| **File** | NecromancyPlagueCloudSpell.cs |
| **Words** | "Pestis Nubis" |
| **Reagents** | Lich Dust, Phylactery Shard |
| **Range** | 10 tiles |
| **Area** | 8 tile AoE |
| **Duration** | 7 minutes |

**Effect:**
- ✅ PLACEHOLDER: +18 STR buff
- ❌ NOT IMPLEMENTED: Plague cloud (decay + poison)
- ❌ NOT IMPLEMENTED: Spreading mechanic
- Designed: 8 tile AoE with spreading

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Target | 0x376A | 0x481 | Purple particles |
| Sound | 0x1F2 | - | Spell sound |

---

### 23. Unholy Frenzy

| Property | Value |
|----------|-------|
| **File** | NecromancyUnholyFrenzySpell.cs |
| **Words** | "Profanus Frenesis" |
| **Reagents** | Lich Dust, Phylactery Shard |
| **Range** | 10 tiles |
| **Target** | Undead allies |
| **Duration** | 7 minutes |

**Effect:**
- ✅ PLACEHOLDER: +18 STR buff
- ❌ NOT IMPLEMENTED: +50% attack speed for undead
- ❌ NOT IMPLEMENTED: DoT to undead allies
- Designed: Power boost with cost

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Target | 0x376A | 0x481 | Purple particles |
| Sound | 0x1F2 | - | Spell sound |

---

### 24. Lich Form

| Property | Value |
|----------|-------|
| **File** | NecromancyLichFormSpell.cs |
| **Words** | "Lichum Formum" |
| **Reagents** | Lich Dust, Phylactery Shard |
| **Range** | 10 tiles |
| **Target** | Self |
| **Duration** | 7 minutes |

**Effect:**
- ✅ PLACEHOLDER: +18 STR buff
- ❌ NOT IMPLEMENTED: Lich transformation (body change)
- ❌ NOT IMPLEMENTED: Poison immunity
- ❌ NOT IMPLEMENTED: +30% necro damage
- Designed: Transform into lich (body 0x0018)

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Target | 0x376A | 0x481 | Purple particles |
| Sound | 0x1F2 | - | Spell sound |

**Implementation:**
- ⚠️ NEEDS WORK: Requires body change (0x0018)
- ⚠️ NEEDS WORK: Requires resistance mod (+100 poison)
- ⚠️ NEEDS WORK: Requires damage bonus system

---

## Circle 7 - Ultimate Necromancy (40 Mana)

### 25. Army of the Dead

| Property | Value |
|----------|-------|
| **File** | NecromancyArmyoftheDeadSpell.cs |
| **Words** | "Exercitus Mortuorum" |
| **Reagents** | Phylactery Shard, Reaper Essence |
| **Range** | 10 tiles |
| **Area** | 5 tile radius |
| **Duration** | Permanent (until killed) |

**Effect:**
- ✅ PLACEHOLDER: +22 STR buff
- ❌ NOT IMPLEMENTED: Mass summon (10 undead)
- Designed: Summon 10 varied undead warriors

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Target | 0x376A | 0x481 | Purple particles |
| Sound | 0x1F2 | - | Spell sound |

**Implementation:**
- ⚠️ NEEDS WORK: Requires varied undead types
- ⚠️ NEEDS WORK: Mix of skeleton, zombie, mage types

---

### 26. Death Wave

| Property | Value |
|----------|-------|
| **File** | NecromancyDeathWaveSpell.cs |
| **Words** | "Mortis Unda" |
| **Reagents** | Phylactery Shard, Reaper Essence |
| **Range** | Screen-wide cone |
| **Area** | Cone AoE |
| **Duration** | Instant |

**Effect:**
- ✅ PLACEHOLDER: +22 STR buff
- ❌ NOT IMPLEMENTED: Cone damage (50-85)
- ❌ NOT IMPLEMENTED: Corpse raising
- Designed: Screen-wide cone, raises corpses

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Target | 0x376A | 0x481 | Purple particles |
| Sound | 0x1F2 | - | Spell sound |

---

### 27. Bone Prison

| Property | Value |
|----------|-------|
| **File** | NecromancyBonePrisonSpell.cs |
| **Words** | "Ossus Carcerem" |
| **Reagents** | Phylactery Shard, Reaper Essence |
| **Range** | 10 tiles |
| **Target** | Single enemy |
| **Duration** | 10 seconds (or break) |

**Effect:**
- ✅ PLACEHOLDER: +22 STR buff
- ❌ NOT IMPLEMENTED: Bone prison (8 bone walls around target)
- ❌ NOT IMPLEMENTED: 300 HP to break
- Designed: Trap enemy in bones, 10s disable

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Target | 0x376A | 0x481 | Purple particles |
| Sound | 0x1F2 | - | Spell sound |

---

### 28. Demi-Lich Transformation

| Property | Value |
|----------|-------|
| **File** | NecromancyDemiLichTransformationSpell.cs |
| **Words** | "Demi Lichum Transformus" |
| **Reagents** | Phylactery Shard, Reaper Essence |
| **Range** | Self |
| **Target** | Self |
| **Duration** | 10 minutes |

**Effect:**
- ✅ PLACEHOLDER: +22 STR buff
- ❌ NOT IMPLEMENTED: Flying lich form
- ❌ NOT IMPLEMENTED: +50% power
- ❌ NOT IMPLEMENTED: Physical immunity
- Designed: Ultimate lich transformation

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Target | 0x376A | 0x481 | Purple particles |
| Sound | 0x1F2 | - | Spell sound |

---

## Circle 8 - Archlich Powers (50 Mana)

### 29. Apocalypse of Death

| Property | Value |
|----------|-------|
| **File** | NecromancyApocalypseofDeathSpell.cs |
| **Words** | "Apocalypsis Mortis" |
| **Reagents** | Phylactery Shard, Reaper Essence (×2) |
| **Range** | Screen-wide |
| **Area** | All enemies on screen |
| **Duration** | Instant |

**Effect:**
- ✅ PLACEHOLDER: +25 STR buff (15 min)
- ❌ NOT IMPLEMENTED: Screen-wide damage (80-140)
- ❌ NOT IMPLEMENTED: Raise all corpses
- Designed: Ultimate AoE + mass resurrection

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Target | 0x376A | 0x481 | Purple particles |
| Sound | 0x1F2 | - | Spell sound |

---

### 30. Summon Undead Dragon

| Property | Value |
|----------|-------|
| **File** | NecromancySummonUndeadDragonSpell.cs |
| **Words** | "Anim Draconis Mortis" |
| **Reagents** | Phylactery Shard, Reaper Essence (×2) |
| **Range** | 10 tiles |
| **Target** | Summon location |
| **Duration** | Permanent (until killed) |

**Effect:**
- ✅ PLACEHOLDER: +25 STR buff (15 min)
- ❌ NOT IMPLEMENTED: Undead dragon summon
- Designed: Legendary summon (3000 HP, breath attack, flying)

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Target | 0x376A | 0x481 | Purple particles |
| Sound | 0x1F2 | - | Spell sound |

---

### 31. Death's Door

| Property | Value |
|----------|-------|
| **File** | NecromancyDeathsDoorSpell.cs |
| **Words** | "Ianua Mortis" |
| **Reagents** | Phylactery Shard, Reaper Essence (×2) |
| **Range** | Screen-wide |
| **Area** | All enemies |
| **Duration** | Instant |

**Effect:**
- ✅ PLACEHOLDER: +25 STR buff (15 min)
- ❌ NOT IMPLEMENTED: Reduce all to 1 HP
- ❌ NOT IMPLEMENTED: High stat resistance check
- Designed: All enemies reduced to 1 HP (resistable)

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Target | 0x376A | 0x481 | Purple particles |
| Sound | 0x1F2 | - | Spell sound |

---

### 32. Archlich Ascension

| Property | Value |
|----------|-------|
| **File** | NecromancyArchlichAscensionSpell.cs |
| **Words** | "Archlichum Ascensionum" |
| **Reagents** | Phylactery Shard, Reaper Essence (×2) |
| **Range** | Self |
| **Target** | Self |
| **Duration** | 15 minutes |

**Effect:**
- ✅ PLACEHOLDER: +25 STR buff (15 min)
- ❌ NOT IMPLEMENTED: Archlich form with phylactery
- ❌ NOT IMPLEMENTED: Revive on death from phylactery
- ❌ NOT IMPLEMENTED: Control all undead
- ❌ NOT IMPLEMENTED: Life drain aura
- Designed: Become archlich with phylactery revival

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Target | 0x376A | 0x481 | Purple particles |
| Sound | 0x1F2 | - | Spell sound |

**Implementation:**
- ⚠️ NEEDS WORK: Requires phylactery item creation
- ⚠️ NEEDS WORK: Requires death hook for revival
- ⚠️ NEEDS WORK: Requires undead control system
- ⚠️ NEEDS WORK: Requires life drain aura (passive damage)

---

## Known Issues

### Critical Issues
1. **All spells use placeholder STR buffs** instead of intended effects
2. **Skill system uses Conjuration** instead of custom Necromancy skill
3. **Damage type is 100% energy** instead of decay/poison mix (50/0/0/50/0)
4. **No corpse detection system** - spells requiring corpses cannot function
5. **No undead summon creatures** - skeleton, zombie, golem, etc. don't exist
6. **No summon limit manager** - would allow infinite summons

### Medium Priority Issues
7. **No life drain mechanics** - Life Siphon, Vampiric Touch, Soul Harvest
8. **No DoT system** - Decay, Death and Decay
9. **No transformation system** - Lich Form, Demi-Lich, Archlich
10. **No AoE mechanics** - Most area spells are single-target
11. **No buff/debuff stacking** - Bone Armor, Soul Shield
12. **TargetFlags.Harmful on buff spells** - Should be TargetFlags.Beneficial

### Low Priority Issues
13. **No visual effects variation** - All spells use same 0x376A effect
14. **No sound variation** - All spells use same 0x1F2 sound
15. **Missing special mechanics** - Corpse explosion, bone prison, phylactery

---

## Implementation Roadmap

### Phase 1: Core Systems (HIGH PRIORITY)
- [ ] Create custom Necromancy skill (ID 58-83 range)
- [ ] Implement decay damage type (50% poison, 50% physical)
- [ ] Create corpse detection helper (FindNearestCorpse)
- [ ] Create 6 undead summon creatures (Skeleton, Zombie, Skeletal Mage, Bone Golem, Death Knight, Undead Dragon)
- [ ] Create NecromancerSummonManager (5 summon limit)
- [ ] Create LifeDrainHelper (damage + heal combo)

### Phase 2: Basic Spells (MEDIUM PRIORITY)
- [ ] Death Bolt - Change to decay damage
- [ ] Animate Bone - Implement corpse-based skeleton raising
- [ ] Life Siphon - Implement life drain (8-15 HP)
- [ ] Bone Shard - Add armor piercing mechanic
- [ ] Corpse Explosion - Add corpse requirement + true AoE

### Phase 3: Advanced Mechanics (LOW PRIORITY)
- [ ] Decay - Implement DoT + max HP reduction
- [ ] Death and Decay - Create persistent ground effect
- [ ] Bone Armor - Add damage reflection
- [ ] Soul Shield - Add absorption + heal on break
- [ ] Death Coil - Smart targeting (damage living OR heal undead)

### Phase 4: Transformations (FUTURE)
- [ ] Lich Form - Body change + immunities + damage bonus
- [ ] Demi-Lich - Flying + power boost + physical immunity
- [ ] Archlich Ascension - Phylactery + revival + control + aura

---

## File Locations

**Spell Files:**
```
ServUO/Scripts/Custom/VystiaClasses/Spells/Necromancer/
├── NecromancyDeathBoltSpell.cs
├── NecromancyAnimateBoneSpell.cs
├── NecromancyLifeSiphonSpell.cs
├── NecromancyDeathsightSpell.cs
├── NecromancyBoneShardSpell.cs
├── NecromancyDecaySpell.cs
├── NecromancyRaiseZombieSpell.cs
├── NecromancySoulShieldSpell.cs
├── NecromancyDeathCoilSpell.cs
├── NecromancyBoneArmorSpell.cs
├── NecromancyMassRaiseSpell.cs
├── NecromancySoulHarvestSpell.cs
├── NecromancySkeletalMageSpell.cs
├── NecromancyCorpseExplosionSpell.cs
├── NecromancyDeathGripSpell.cs
├── NecromancyVampiricTouchSpell.cs
├── NecromancyBoneWallSpell.cs
├── NecromancyDeathandDecaySpell.cs
├── NecromancyRaiseBoneGolemSpell.cs
├── NecromancySoulLinkSpell.cs
├── NecromancyDeathKnightsSpell.cs
├── NecromancyPlagueCloudSpell.cs
├── NecromancyUnholyFrenzySpell.cs
├── NecromancyLichFormSpell.cs
├── NecromancyArmyoftheDeadSpell.cs
├── NecromancyDeathWaveSpell.cs
├── NecromancyBonePrisonSpell.cs
├── NecromancyDemiLichTransformationSpell.cs
├── NecromancyApocalypseofDeathSpell.cs
├── NecromancySummonUndeadDragonSpell.cs
├── NecromancyDeathsDoorSpell.cs
└── NecromancyArchlichAscensionSpell.cs
```

**Reagent File:**
```
ServUO/Scripts/Items/Vystia/Resources/Reagents/NecromancyReagents.cs
```

**Spellbook:**
```
ServUO/Scripts/Items/Equipment/Spellbooks/VystiaSpellbooks.cs
```

**Scroll File:**
```
ServUO/Scripts/Items/Vystia/Scrolls/NecromancyScrolls.cs
```

---

## Testing Commands

```
[spellbook necromancer     - Give Necronomicon
[sb necromancer            - Give Necronomicon (short)
[add GraveMoss            - Add reagents
[add BoneDust
[add CorpseAsh
[add SoulFragment
[add NecroticShroud
[add LichDust
[add PhylacteryShard
[add ReaperEssence
```

---

*Last Updated: 2025-12-28*
*Status: 32/32 spells implemented (100% placeholders - 0% functional)*
*File: Vystia/design/magic/Necromancy.md*
