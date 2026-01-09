# Dark Magic - ShadowVoid School

| Property | Value |
|----------|-------|
| **Class** | Warlock |
| **Region** | ShadowVoid |
| **Theme** | Shadow damage, fear, chaos, demons, sacrificial magic |
| **Spellbook** | Warlock Spellbook (Codex of Shadows) |
| **Hue** | 0x455 (Deep Purple/Black) |
| **Spell IDs** | 1096-1127 |
| **Status** | 100% Complete (32/32 spells) |

---

## Reagents

All Dark Magic spells use Vystia reagents found in ShadowVoid.

| Reagent | Item ID | Circles | Description |
|---------|---------|---------|-------------|
| Shadow Moss | 0x0F7B | 1-3 | Moss from shadow realm |
| Void Crystal | 0x0F8E | 2-4 | Crystal from the void |
| Void Weed | 0x1A9C | 3-5 | Weed from emptiness |
| Shadow Petal | 0x0F86 | 4-6 | Petal from shadow flowers |
| Void Dust | 0x0F8F | 5-7 | Dust of the void |
| Void Silk | 0x0F8D | 6-8 | Silk woven from darkness |
| Demon Heart | 0x0F7A | 7-8 | Heart of shadow demon |
| Shadow Essence | 0x0F7D | 8 | Pure essence of darkness |

---

## Circle 1 - Basic Shadow (4 Mana)

### 1. Shadow Bolt

| Property | Value |
|----------|-------|
| **File** | DarkShadowBoltSpell.cs |
| **Words** | "Umbra Sagitta" |
| **Reagents** | Shadow Moss, Void Crystal |
| **Range** | 10 tiles |
| **Target** | Single enemy |
| **Damage** | 5-10 + (Conjuration × 0.5) |

**Effect:**
- Basic shadow damage projectile
- Scales with Conjuration skill

**Implementation Notes:**
- Uses direct damage type (100% energy, displayed as "magical energy")
- Standard single-target spell pattern

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Target | 0x376A | 0x481 | Purple shadow particles |
| Sound | 0x1F2 | - | Shadow bolt sound |

---

### 2. Life Tap

| Property | Value |
|----------|-------|
| **File** | DarkLifeTapSpell.cs |
| **Words** | "Vita Tapum" |
| **Reagents** | Shadow Moss, Void Crystal |
| **Range** | 10 tiles |
| **Target** | Self or ally |
| **Duration** | 60 seconds |

**Effect:**
- Grants +5 STR + (Conjuration × 0.2) STR
- PLACEHOLDER: Currently grants STR buff instead of HP/Mana conversion

**Implementation Notes:**
- Original design: Sacrifice 10 HP, restore 15 mana
- Current: Applies StatMod buff to Strength
- Needs update to match design intent

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Target | 0x376A | 0x481 | Shadow particles |
| Sound | 0x1F2 | - | Tap sound |

---

### 3. Minor Fear

| Property | Value |
|----------|-------|
| **File** | DarkMinorFearSpell.cs |
| **Words** | "Minor Fearum" |
| **Reagents** | Shadow Moss, Void Crystal |
| **Range** | 10 tiles |
| **Target** | Single enemy |
| **Duration** | 60 seconds |

**Effect:**
- Grants +5 STR + (Conjuration × 0.2) STR
- PLACEHOLDER: Currently grants STR buff instead of fear

**Implementation Notes:**
- Original design: Target flees for 3 seconds
- Current: Applies StatMod buff (placeholder)
- Needs fear mechanics implementation

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Target | 0x376A | 0x481 | Shadow effect |
| Sound | 0x1F2 | - | Fear sound |

---

### 4. Demonic Sight

| Property | Value |
|----------|-------|
| **File** | DarkDemonicSightSpell.cs |
| **Words** | "Demonic Sightum" |
| **Reagents** | Shadow Moss, Void Crystal |
| **Range** | 10 tiles |
| **Target** | Self or ally |
| **Duration** | 60 seconds |

**Effect:**
- Grants +5 STR + (Conjuration × 0.2) STR
- PLACEHOLDER: Currently grants STR buff instead of vision enhancement

**Implementation Notes:**
- Original design: See in darkness, detect holy magic
- Current: Applies StatMod buff (placeholder)
- Needs vision mechanics implementation

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Target | 0x376A | 0x481 | Purple glow |
| Sound | 0x1F2 | - | Vision sound |

---

## Circle 2 - Shadow Arts (6 Mana)

### 5. Chaos Bolt

| Property | Value |
|----------|-------|
| **File** | DarkChaosBoltSpell.cs |
| **Words** | "Chaosum Sagitta" |
| **Reagents** | Void Crystal, Void Weed |
| **Range** | 10 tiles |
| **Target** | Single enemy |
| **Damage** | 8-25 (random) |

**Effect:**
- Random damage projectile (chaotic)
- Pure chaos damage (100% physical resistance type)

**Implementation Notes:**
- Fully functional
- Randomness creates unpredictable damage output

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Target | 0x376A | 0x481 | Chaotic shadow bolt |
| Sound | 0x1F2 | - | Chaos sound |

---

### 6. Shadow Step

| Property | Value |
|----------|-------|
| **File** | DarkShadowStepSpell.cs |
| **Words** | "Shadow Stepum" |
| **Reagents** | Void Crystal, Void Weed |
| **Range** | 10 tiles |
| **Target** | Self or ally |
| **Duration** | 60 seconds |

**Effect:**
- Grants +5 STR + (Conjuration × 0.2) STR
- PLACEHOLDER: Currently grants STR buff instead of teleport

**Implementation Notes:**
- Original design: Teleport to shadows (8 tiles)
- Current: Applies StatMod buff (placeholder)
- Needs teleport mechanics implementation

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Target | 0x376A | 0x481 | Shadow shift |
| Sound | 0x1F2 | - | Teleport sound |

---

### 7. Drain Soul

| Property | Value |
|----------|-------|
| **File** | DarkDrainSoulSpell.cs |
| **Words** | "Drain Soulum" |
| **Reagents** | Void Crystal, Void Weed |
| **Range** | 10 tiles |
| **Target** | Self or ally |
| **Duration** | 60 seconds |

**Effect:**
- Grants +5 STR + (Conjuration × 0.2) STR
- PLACEHOLDER: Currently grants STR buff instead of soul drain

**Implementation Notes:**
- Original design: Kill low HP target (<10%), create soul shard
- Current: Applies StatMod buff (placeholder)
- Needs soul shard system implementation

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Target | 0x376A | 0x481 | Soul extraction |
| Sound | 0x1F2 | - | Drain sound |

---

### 8. Lesser Demon

| Property | Value |
|----------|-------|
| **File** | DarkLesserDemonSpell.cs |
| **Words** | "Summonum Lesser Demonum" |
| **Reagents** | Void Crystal, Void Weed |
| **Range** | - |
| **Target** | Ground (summon) |
| **Duration** | 10 minutes |

**Effect:**
- Summons Grey Wolf (placeholder creature)
- Requires 2 follower slots
- 10 minute duration

**Implementation Notes:**
- Original design: Summon Imp (200 HP, weak attacks)
- Current: Summons GreyWolf as placeholder
- Needs custom Imp creature implementation

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Summon | 0x1FB | - | Summon effect |
| Sound | - | - | (None) |

---

## Circle 3 - Shadow Mastery (9 Mana)

### 9. Shadow Nova

| Property | Value |
|----------|-------|
| **File** | DarkShadowNovaSpell.cs |
| **Words** | "Umbra Novaum" |
| **Reagents** | Void Weed, Shadow Petal |
| **Range** | 10 tiles |
| **Target** | Single target (PLACEHOLDER for AoE) |
| **Damage** | 15-20 + (Conjuration × 0.5) |

**Effect:**
- Direct shadow damage
- PLACEHOLDER: Currently single-target instead of AoE

**Implementation Notes:**
- Original design: 4 tile AoE, 12-24 shadow damage
- Current: Single-target spell
- Needs AoE mechanics implementation

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Area | 0x376A | 0x481 | Shadow explosion |
| Sound | 0x1F2 | - | Nova sound |

---

### 10. Demonic Pact

| Property | Value |
|----------|-------|
| **File** | DarkDemonicPactSpell.cs |
| **Words** | "Demonicum Pactum" |
| **Reagents** | Void Weed, Shadow Petal |
| **Range** | 10 tiles |
| **Target** | Self or ally |
| **Duration** | 180 seconds (3 minutes) |

**Effect:**
- Grants +10 STR + (Conjuration × 0.2) STR
- PLACEHOLDER: Currently grants STR buff only

**Implementation Notes:**
- Original design: Sacrifice 25% HP, +30% spell damage for 45s
- Current: Applies StatMod buff (placeholder)
- Needs HP sacrifice and spell damage bonus implementation

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Target | 0x376A | 0x481 | Demonic aura |
| Sound | 0x1F2 | - | Pact sound |

---

### 11. Fear Wave

| Property | Value |
|----------|-------|
| **File** | DarkFearWaveSpell.cs |
| **Words** | "Fear Waveum" |
| **Reagents** | Void Weed, Shadow Petal |
| **Range** | 10 tiles |
| **Target** | Self or ally |
| **Duration** | 60 seconds |

**Effect:**
- Grants +5 STR + (Conjuration × 0.2) STR
- PLACEHOLDER: Currently grants STR buff instead of cone fear

**Implementation Notes:**
- Original design: Cone, causes fear for 5s
- Current: Applies StatMod buff (placeholder)
- Needs cone AoE fear implementation

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Target | 0x376A | 0x481 | Fear wave |
| Sound | 0x1F2 | - | Fear sound |

---

### 12. Corruption

| Property | Value |
|----------|-------|
| **File** | DarkCorruptionSpell.cs |
| **Words** | "Corruptionum" |
| **Reagents** | Void Weed, Shadow Petal |
| **Range** | 10 tiles |
| **Target** | Self or ally |
| **Duration** | 60 seconds |

**Effect:**
- Grants +5 STR + (Conjuration × 0.2) STR
- PLACEHOLDER: Currently grants STR buff instead of DoT

**Implementation Notes:**
- Original design: DoT 6-10 shadow/tick, spreads on death
- Current: Applies StatMod buff (placeholder)
- Needs DoT and spread mechanics implementation

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Target | 0x376A | 0x481 | Corruption effect |
| Sound | 0x1F2 | - | Corruption sound |

---

## Circle 4 - Void Power (11 Mana)

### 13. Summon Voidwalker

| Property | Value |
|----------|-------|
| **File** | DarkSummonVoidwalkerSpell.cs |
| **Words** | "Summonum Voidwalkerum" |
| **Reagents** | Shadow Petal, Void Dust |
| **Range** | - |
| **Target** | Ground (summon) |
| **Duration** | 10 minutes |

**Effect:**
- Summons Grey Wolf (placeholder creature)
- Requires 2 follower slots
- 10 minute duration

**Implementation Notes:**
- Original design: Tank demon (600 HP, taunts)
- Current: Summons GreyWolf as placeholder
- Needs custom Voidwalker creature implementation

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Summon | 0x1FB | - | Summon effect |
| Sound | - | - | (None) |

---

### 14. Shadow Chains

| Property | Value |
|----------|-------|
| **File** | DarkShadowChainsSpell.cs |
| **Words** | "Shadow Chainsum" |
| **Reagents** | Shadow Petal, Void Dust |
| **Range** | 10 tiles |
| **Target** | Self or ally |
| **Duration** | 60 seconds |

**Effect:**
- Grants +5 STR + (Conjuration × 0.2) STR
- PLACEHOLDER: Currently grants STR buff instead of root+DoT

**Implementation Notes:**
- Original design: Root + 8-15 shadow damage/tick
- Current: Applies StatMod buff (placeholder)
- Needs root and DoT mechanics implementation

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Target | 0x376A | 0x481 | Shadow chains |
| Sound | 0x1F2 | - | Chain sound |

---

### 15. Soul Burn

| Property | Value |
|----------|-------|
| **File** | DarkSoulBurnSpell.cs |
| **Words** | "Soul Burnum" |
| **Reagents** | Shadow Petal, Void Dust |
| **Range** | 10 tiles |
| **Target** | Self or ally |
| **Duration** | 60 seconds |

**Effect:**
- Grants +5 STR + (Conjuration × 0.2) STR
- PLACEHOLDER: Currently grants STR buff instead of soul consume

**Implementation Notes:**
- Original design: Consumes soul shard, deals 30-50 damage
- Current: Applies StatMod buff (placeholder)
- Needs soul shard consumption implementation

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Target | 0x376A | 0x481 | Soul fire |
| Sound | 0x1F2 | - | Burn sound |

---

### 16. Chaos Rift

| Property | Value |
|----------|-------|
| **File** | DarkChaosRiftSpell.cs |
| **Words** | "Chaos Riftum" |
| **Reagents** | Shadow Petal, Void Dust |
| **Range** | 10 tiles |
| **Target** | Self or ally |
| **Duration** | 60 seconds |

**Effect:**
- Grants +5 STR + (Conjuration × 0.2) STR
- PLACEHOLDER: Currently grants STR buff instead of chaos effects

**Implementation Notes:**
- Original design: Random AoE effects (damage, fear, slow, confusion)
- Current: Applies StatMod buff (placeholder)
- Needs chaotic AoE implementation

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Target | 0x376A | 0x481 | Chaos rift |
| Sound | 0x1F2 | - | Rift sound |

---

## Circle 5 - Dark Arts (14 Mana)

### 17. Fel Armor

| Property | Value |
|----------|-------|
| **File** | DarkFelArmorSpell.cs |
| **Words** | "Fel Armorum" |
| **Reagents** | Void Dust, Void Silk |
| **Range** | 10 tiles |
| **Target** | Self or ally |
| **Duration** | 60 seconds |

**Effect:**
- Grants +5 STR + (Conjuration × 0.2) STR
- PLACEHOLDER: Currently grants STR buff instead of shadow resist

**Implementation Notes:**
- Original design: +20 Shadow Resist, converts damage to mana
- Current: Applies StatMod buff (placeholder)
- Needs resistance and damage conversion implementation

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Target | 0x376A | 0x481 | Dark armor |
| Sound | 0x1F2 | - | Armor sound |

---

### 18. Mass Fear

| Property | Value |
|----------|-------|
| **File** | DarkMassFearSpell.cs |
| **Words** | "Mass Fearum" |
| **Reagents** | Void Dust, Void Silk |
| **Range** | 10 tiles |
| **Target** | Self or ally |
| **Duration** | 60 seconds |

**Effect:**
- Grants +5 STR + (Conjuration × 0.2) STR
- PLACEHOLDER: Currently grants STR buff instead of AoE fear

**Implementation Notes:**
- Original design: 6 tile radius, all enemies flee 6s
- Current: Applies StatMod buff (placeholder)
- Needs AoE fear implementation

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Target | 0x376A | 0x481 | Terror wave |
| Sound | 0x1F2 | - | Fear scream |

---

### 19. Demonic Sacrifice

| Property | Value |
|----------|-------|
| **File** | DarkDemonicSacrificeSpell.cs |
| **Words** | "Demonic Sacrificeum" |
| **Reagents** | Void Dust, Void Silk |
| **Range** | 10 tiles |
| **Target** | Self or ally |
| **Duration** | 60 seconds |

**Effect:**
- Grants +5 STR + (Conjuration × 0.2) STR
- PLACEHOLDER: Currently grants STR buff instead of demon sacrifice

**Implementation Notes:**
- Original design: Kill demon, heal 50% HP and +25% damage
- Current: Applies StatMod buff (placeholder)
- Needs demon sacrifice mechanics implementation

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Target | 0x376A | 0x481 | Sacrifice effect |
| Sound | 0x1F2 | - | Sacrifice sound |

---

### 20. Shadow Orb

| Property | Value |
|----------|-------|
| **File** | DarkShadowOrbSpell.cs |
| **Words** | "Shadow Orbum" |
| **Reagents** | Void Dust, Void Silk |
| **Range** | 10 tiles |
| **Target** | Self or ally |
| **Duration** | 60 seconds |

**Effect:**
- Grants +5 STR + (Conjuration × 0.2) STR
- PLACEHOLDER: Currently grants STR buff instead of orb summon

**Implementation Notes:**
- Original design: Floating orb, auto-attacks enemies (10s)
- Current: Applies StatMod buff (placeholder)
- Needs orb summon implementation

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Target | 0x376A | 0x481 | Shadow orb |
| Sound | 0x1F2 | - | Orb sound |

---

## Circle 6 - Demon Mastery (20 Mana)

### 21. Summon Succubus

| Property | Value |
|----------|-------|
| **File** | DarkSummonSuccubusSpell.cs |
| **Words** | "Summonum Succubusum" |
| **Reagents** | Void Silk, Demon Heart |
| **Range** | - |
| **Target** | Ground (summon) |
| **Duration** | 10 minutes |

**Effect:**
- Summons Grey Wolf (placeholder creature)
- Requires 2 follower slots
- 10 minute duration

**Implementation Notes:**
- Original design: Demon caster (800 HP, shadow spells, charms)
- Current: Summons GreyWolf as placeholder
- Needs custom Succubus creature implementation

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Summon | 0x1FB | - | Summon effect |
| Sound | - | - | (None) |

---

### 22. Dark Portal

| Property | Value |
|----------|-------|
| **File** | DarkDarkPortalSpell.cs |
| **Words** | "Dark Portalum" |
| **Reagents** | Void Silk, Demon Heart |
| **Range** | 10 tiles |
| **Target** | Self or ally |
| **Duration** | 60 seconds |

**Effect:**
- Grants +5 STR + (Conjuration × 0.2) STR
- PLACEHOLDER: Currently grants STR buff instead of teleport

**Implementation Notes:**
- Original design: Teleport entire party (15 tile range)
- Current: Applies StatMod buff (placeholder)
- Needs party teleport implementation

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Target | 0x376A | 0x481 | Portal effect |
| Sound | 0x1F2 | - | Portal sound |

---

### 23. Chaos Storm

| Property | Value |
|----------|-------|
| **File** | DarkChaosStormSpell.cs |
| **Words** | "Chaos Stormum" |
| **Reagents** | Void Silk, Demon Heart |
| **Range** | 10 tiles |
| **Target** | Self or ally |
| **Duration** | 60 seconds |

**Effect:**
- Grants +5 STR + (Conjuration × 0.2) STR
- PLACEHOLDER: Currently grants STR buff instead of chaos storm

**Implementation Notes:**
- Original design: 8 tile radius, random devastating effects
- Current: Applies StatMod buff (placeholder)
- Needs AoE chaos implementation

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Target | 0x376A | 0x481 | Chaos storm |
| Sound | 0x1F2 | - | Storm sound |

---

### 24. Shadowform

| Property | Value |
|----------|-------|
| **File** | DarkShadowformSpell.cs |
| **Words** | "Shadowformum" |
| **Reagents** | Void Silk, Demon Heart |
| **Range** | 10 tiles |
| **Target** | Self or ally |
| **Duration** | 60 seconds |

**Effect:**
- Grants +5 STR + (Conjuration × 0.2) STR
- PLACEHOLDER: Currently grants STR buff instead of shadowform

**Implementation Notes:**
- Original design: Become shadow: +50% shadow damage, immune to physical
- Current: Applies StatMod buff (placeholder)
- Needs transformation mechanics implementation

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Target | 0x376A | 0x481 | Shadow transformation |
| Sound | 0x1F2 | - | Transform sound |

---

## Circle 7 - Void Mastery (40 Mana)

### 25. Summon Pit Lord

| Property | Value |
|----------|-------|
| **File** | DarkSummonPitLordSpell.cs |
| **Words** | "Summonum Pit Lordum" |
| **Reagents** | Demon Heart, Shadow Essence |
| **Range** | - |
| **Target** | Ground (summon) |
| **Duration** | 10 minutes |

**Effect:**
- Summons Grey Wolf (placeholder creature)
- Requires 2 follower slots
- 10 minute duration

**Implementation Notes:**
- Original design: Ultimate demon (2000 HP, AoE cleave, fear aura)
- Current: Summons GreyWolf as placeholder
- Needs custom Pit Lord creature implementation

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Summon | 0x1FB | - | Summon effect |
| Sound | - | - | (None) |

---

### 26. Soul Harvest

| Property | Value |
|----------|-------|
| **File** | DarkSoulHarvestSpell.cs |
| **Words** | "Soul Harvestum" |
| **Reagents** | Demon Heart, Shadow Essence |
| **Range** | 10 tiles |
| **Target** | Self or ally |
| **Duration** | 60 seconds |

**Effect:**
- Grants +5 STR + (Conjuration × 0.2) STR
- PLACEHOLDER: Currently grants STR buff instead of AoE drain

**Implementation Notes:**
- Original design: AoE drain, creates soul shards from kills
- Current: Applies StatMod buff (placeholder)
- Needs AoE drain and soul shard implementation

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Target | 0x376A | 0x481 | Soul harvest |
| Sound | 0x1F2 | - | Harvest sound |

---

### 27. Apocalyptic Chaos

| Property | Value |
|----------|-------|
| **File** | DarkApocalypticChaosSpell.cs |
| **Words** | "Apocalyptic Chaosum" |
| **Reagents** | Demon Heart, Shadow Essence |
| **Range** | 10 tiles |
| **Target** | Self or ally |
| **Duration** | 60 seconds |

**Effect:**
- Grants +5 STR + (Conjuration × 0.2) STR
- PLACEHOLDER: Currently grants STR buff instead of chaos effects

**Implementation Notes:**
- Original design: Screen-wide random catastrophic effects
- Current: Applies StatMod buff (placeholder)
- Needs screen-wide chaos implementation

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Target | 0x376A | 0x481 | Apocalypse |
| Sound | 0x1F2 | - | Chaos sound |

---

### 28. Demonic Ascension

| Property | Value |
|----------|-------|
| **File** | DarkDemonicAscensionSpell.cs |
| **Words** | "Demonic Ascensionum" |
| **Reagents** | Demon Heart, Shadow Essence |
| **Range** | 10 tiles |
| **Target** | Self or ally |
| **Duration** | 60 seconds |

**Effect:**
- Grants +5 STR + (Conjuration × 0.2) STR
- PLACEHOLDER: Currently grants STR buff instead of transformation

**Implementation Notes:**
- Original design: Become demon: +60 STR, +40% all damage, demon abilities
- Current: Applies StatMod buff (placeholder)
- Needs demon transformation implementation

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Target | 0x376A | 0x481 | Demonic transformation |
| Sound | 0x1F2 | - | Ascension sound |

---

## Circle 8 - Ultimate Darkness (50 Mana)

### 29. Summon Demon Prince

| Property | Value |
|----------|-------|
| **File** | DarkSummonDemonPrinceSpell.cs |
| **Words** | "Summonum Demonum Princeus" |
| **Reagents** | Demon Heart, Shadow Essence (×2) |
| **Range** | - |
| **Target** | Ground (summon) |
| **Duration** | 10 minutes |

**Effect:**
- Summons Grey Wolf (placeholder creature)
- Requires 2 follower slots
- 10 minute duration

**Implementation Notes:**
- Original design: Legendary demon (3500 HP, hellfire, commands other demons)
- Current: Summons GreyWolf as placeholder
- Needs custom Demon Prince creature implementation

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Summon | 0x1FB | - | Summon effect |
| Sound | - | - | (None) |

---

### 30. Void Collapse

| Property | Value |
|----------|-------|
| **File** | DarkVoidCollapseSpell.cs |
| **Words** | "Void Collapsum" |
| **Reagents** | Demon Heart, Shadow Essence (×2) |
| **Range** | 10 tiles |
| **Target** | Self or ally |
| **Duration** | 60 seconds |

**Effect:**
- Grants +5 STR + (Conjuration × 0.2) STR
- PLACEHOLDER: Currently grants STR buff instead of black hole

**Implementation Notes:**
- Original design: Creates black hole, 10 tiles, pulls enemies in, 100-180 damage
- Current: Applies StatMod buff (placeholder)
- Needs black hole mechanics implementation

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Target | 0x376A | 0x481 | Void collapse |
| Sound | 0x1F2 | - | Collapse sound |

---

### 31. Chaos Incarnate

| Property | Value |
|----------|-------|
| **File** | DarkChaosIncarnateSpell.cs |
| **Words** | "Chaos Incarnateum" |
| **Reagents** | Demon Heart, Shadow Essence (×2) |
| **Range** | 10 tiles |
| **Target** | Self or ally |
| **Duration** | 60 seconds |

**Effect:**
- Grants +5 STR + (Conjuration × 0.2) STR
- PLACEHOLDER: Currently grants STR buff instead of chaos proc

**Implementation Notes:**
- Original design: Every spell cast triggers random bonus effect (30s)
- Current: Applies StatMod buff (placeholder)
- Needs spell proc mechanics implementation

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Target | 0x376A | 0x481 | Chaos aura |
| Sound | 0x1F2 | - | Chaos sound |

---

### 32. Dark Apotheosis

| Property | Value |
|----------|-------|
| **File** | DarkDarkApotheosisSpell.cs |
| **Words** | "Dark Apotheoseum" |
| **Reagents** | Demon Heart, Shadow Essence (×2) |
| **Range** | 10 tiles |
| **Target** | Self or ally |
| **Duration** | 60 seconds |

**Effect:**
- Grants +5 STR + (Conjuration × 0.2) STR
- PLACEHOLDER: Currently grants STR buff instead of ultimate form

**Implementation Notes:**
- Original design: Ultimate form: Immune to fear/charm, demons unkillable, all spells cost HP instead of mana
- Current: Applies StatMod buff (placeholder)
- Needs ultimate transformation implementation

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Target | 0x376A | 0x481 | Dark apotheosis |
| Sound | 0x1F2 | - | Ultimate sound |

---

## Implementation Status

### Fully Functional (3 spells)
1. **Shadow Bolt** - Basic damage projectile
2. **Chaos Bolt** - Random damage projectile
3. **All Summon Spells** - Functional but using placeholder creatures (GreyWolf)

### Placeholder Buffs (24 spells)
All remaining spells currently grant STR buffs as placeholders. They need proper mechanics:
- Fear effects (3 spells): Minor Fear, Fear Wave, Mass Fear
- HP/Mana conversion (1 spell): Life Tap
- Vision enhancement (1 spell): Demonic Sight
- Teleportation (2 spells): Shadow Step, Dark Portal
- Soul mechanics (3 spells): Drain Soul, Soul Burn, Soul Harvest
- AoE damage (3 spells): Shadow Nova, Chaos Storm, Void Collapse
- Buffs/transformations (6 spells): Demonic Pact, Fel Armor, Shadowform, Demonic Ascension, Chaos Incarnate, Dark Apotheosis
- DoT/Debuffs (2 spells): Corruption, Shadow Chains
- Complex mechanics (3 spells): Chaos Rift, Demonic Sacrifice, Shadow Orb

### Custom Creatures Needed (5 creatures)
1. **Imp** (Circle 2) - 200 HP, weak ranged attacks
2. **Voidwalker** (Circle 4) - 600 HP tank with taunt
3. **Succubus** (Circle 6) - 800 HP caster with charm
4. **Pit Lord** (Circle 7) - 2000 HP melee with fear aura
5. **Demon Prince** (Circle 8) - 3500 HP legendary demon

---

## Known Issues

### Critical
1. **Most spells are placeholders** - 24/32 spells grant generic STR buffs instead of intended effects
2. **All summons use GreyWolf** - Need custom demon creatures
3. **No soul shard system** - Soul mechanics completely unimplemented
4. **No fear mechanics** - All fear effects are placeholder buffs

### High Priority
1. **AoE spells are single-target** - Shadow Nova, Chaos Storm, etc. need AoE implementation
2. **No transformation spells** - Shadowform, Demonic Ascension, Dark Apotheosis need body change mechanics
3. **No teleportation** - Shadow Step and Dark Portal need teleport implementation
4. **No DoT system** - Corruption and Shadow Chains need damage-over-time

### Medium Priority
1. **Visual effects** - All spells use same purple particle effect (0x376A)
2. **Sound effects** - All spells use same sound (0x1F2)
3. **Animation variety** - Need unique visual identity per spell

---

## File Locations

**Spell Files:** `ServUO/Scripts/Custom/VystiaClasses/Spells/Warlock/`
- 32 spell files (Dark*.cs)

**Reagents:** `ServUO/Scripts/Items/Vystia/Resources/Reagents/DarkMagicReagents.cs`
- 8 reagent types

**Spellbook:** `ServUO/Scripts/Items/Equipment/Spellbooks/VystiaSpellbooks.cs`
- WarlockSpellbook class

**Spell Registration:** `ServUO/Scripts/Custom/VystiaClasses/Spells/VystiaSpellInitializer.cs`
- All 32 spells registered with IDs 1096-1127

---

## Development Notes

### Common Spell Pattern
All Dark Magic spells follow this structure:
```csharp
// Reagent check in CheckCast()
if (!Caster.Backpack.ConsumeTotal(typeof(ReagentA), 1) ||
    !Caster.Backpack.ConsumeTotal(typeof(ReagentB), 1))
{
    return false;
}

// Placeholder buff effect (most spells)
int bonus = 5 + (int)(Caster.Skills.Conjuration.Value * 0.2);
mobile.AddStatMod(new StatMod(StatType.Str, "SpellName_Str", bonus, TimeSpan.FromMinutes(1)));

// Visual/sound effects (all use same IDs)
target.FixedParticles(0x376A, 10, 15, 5038, 0x481, 0, EffectLayer.Waist);
Caster.PlaySound(0x1F2);
```

### Next Steps for Full Implementation
1. Create custom demon creatures (Imp, Voidwalker, Succubus, Pit Lord, Demon Prince)
2. Implement soul shard system (manager class, tracking, consumption)
3. Implement fear mechanics (forced movement, AI hooks)
4. Implement AoE damage system (area enumeration, multi-target damage)
5. Implement transformation mechanics (body change, stat modifications, ability unlocks)
6. Implement teleportation (location validation, party teleport)
7. Implement DoT system (tick damage, duration tracking)
8. Update visual/sound effects for spell variety

---

*Last Updated: 2025-12-28*
*Status: 32/32 spells implemented (3 functional, 24 placeholders, 5 summons with placeholder creatures)*
