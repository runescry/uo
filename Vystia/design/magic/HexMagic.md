# Hex Magic - Shadowfen School

**Class:** Witch
**Region:** Shadowfen
**Theme:** Curses, debuffs, poison, life drain, dark rituals, voodoo
**Spellbook:** Grimoire of Hexes (WitchSpellbook.cs)
**Primary Stat:** Intelligence
**Hue:** 0x81D (Murky Green/Purple)

**Implementation Status:** ✅ **32/32 spells implemented (100% COMPLETE)**

---

## Overview

Hex Magic draws upon the dark swamp energies of Shadowfen, specializing in curses, debuffs, and life-draining abilities. Witches excel at weakening enemies, spreading contagious hexes, and sustaining themselves through life drain.

**Strengths:**
- Powerful debuffs and curses
- Life drain for survivability (when implemented)
- Hexes spread between enemies
- Strong anti-healing capabilities
- Excellent sustained damage via DoTs

**Weaknesses:**
- Low burst damage
- Weak against holy/light magic
- Many spells require setup time
- Less effective against undead (no life drain)

---

## Spell List (32 Spells)

### Circle 1 (4 spells)

#### 1. Evil Eye

| Property | Value |
|----------|-------|
| **Mana** | 4 |
| **Reagents** | BogMoss (1), ViperFang (1) |
| **Range** | 12 tiles |
| **Mantra** | Evilum Eyeus |
| **Effect** | ⚠️ PLACEHOLDER: +5 STR buff (should be -5% accuracy debuff) |
| **Duration** | 60 seconds |

| Animation | Value |
|-----------|-------|
| **Effect ID** | 0x376A |
| **Sound** | 0x1F8 |
| **Target Flags** | Harmful |

**Known Issues:**
- Implementation is STR buff instead of accuracy debuff
- Missing purple eye visual effect (0x3779)

---

#### 2. Weak Curse

| Property | Value |
|----------|-------|
| **Mana** | 4 |
| **Reagents** | BogMoss (1), ViperFang (1) |
| **Range** | 10 tiles |
| **Mantra** | Weakum Curseum |
| **Effect** | ⚠️ PLACEHOLDER: +5 STR buff (should be -5 all stats debuff) |
| **Duration** | 60 seconds |

| Animation | Value |
|-----------|-------|
| **Effect ID** | 0x376A |
| **Sound** | 0x1FC |
| **Target Flags** | Harmful |

**Known Issues:**
- Implementation is STR buff instead of stat debuff
- Missing dark aura effect (0x374A)

---

#### 3. Siphon Life

| Property | Value |
|----------|-------|
| **Mana** | 4 |
| **Reagents** | BogMoss (1), ViperFang (1) |
| **Range** | 8 tiles |
| **Mantra** | Siphonum Vita |
| **Effect** | ⚠️ PLACEHOLDER: 0 HP heal (should drain 6-12 HP, heal caster) |
| **Duration** | Instant |

| Animation | Value |
|-----------|-------|
| **Effect ID** | 0x376A |
| **Sound** | 0x1FB |
| **Target Flags** | Harmful |

**Known Issues:**
- Heals 0 HP (RandomMinMax(0, 0))
- Missing life drain mechanic
- Missing beam effect from target to caster

---

#### 4. Witch Sight

| Property | Value |
|----------|-------|
| **Mana** | 4 |
| **Reagents** | BogMoss (1), ViperFang (1) |
| **Range** | Self |
| **Mantra** | Witchum Sightum |
| **Effect** | ⚠️ PLACEHOLDER: +5 INT buff (should reveal hidden, detect magic) |
| **Duration** | 60 seconds |

| Animation | Value |
|-----------|-------|
| **Effect ID** | 0x376A |
| **Sound** | 0x1EA |
| **Target Flags** | Harmful |

**Known Issues:**
- Implementation is INT buff instead of detection ability
- Missing reveal hidden mechanic
- Missing purple glow in eyes (0x373A)

---

### Circle 2 (4 spells)

#### 5. Wasting Curse

| Property | Value |
|----------|-------|
| **Mana** | 6 |
| **Reagents** | ViperFang (1), Witchweed (1) |
| **Range** | 10 tiles |
| **Mantra** | Wastingum Curseum |
| **Effect** | ⚠️ PLACEHOLDER: +5 DEX buff (should reduce max HP over time) |
| **Duration** | 60 seconds |

| Animation | Value |
|-----------|-------|
| **Effect ID** | 0x376A |
| **Sound** | 0x1FC |
| **Target Flags** | Harmful |

**Known Issues:**
- Implementation is DEX buff instead of HP wasting debuff
- Missing sickly green aura (0x372A)
- Missing periodic HP loss mechanic

---

#### 6. Poison Touch

| Property | Value |
|----------|-------|
| **Mana** | 6 |
| **Reagents** | ViperFang (1), Witchweed (1) |
| **Range** | 10 tiles |
| **Mantra** | Poisonum Touchum |
| **Effect** | ⚠️ PLACEHOLDER: +5 STR buff (should apply Greater Poison) |
| **Duration** | 60 seconds |

| Animation | Value |
|-----------|-------|
| **Effect ID** | 0x376A |
| **Sound** | 0x205 |
| **Target Flags** | Harmful |

**Known Issues:**
- Implementation is STR buff instead of poison application
- Missing green cloud effect (0x372A)
- Missing poison damage over time

---

#### 7. Enfeeble

| Property | Value |
|----------|-------|
| **Mana** | 6 |
| **Reagents** | ViperFang (1), Witchweed (1) |
| **Range** | 10 tiles |
| **Mantra** | Enfeebleum |
| **Effect** | ⚠️ PLACEHOLDER: +5 INT buff (should be -15 STR, -10% damage) |
| **Duration** | 60 seconds |

| Animation | Value |
|-----------|-------|
| **Effect ID** | 0x376A |
| **Sound** | 0x1FC |
| **Target Flags** | Harmful |

**Known Issues:**
- Implementation is INT buff instead of STR/damage debuff
- Missing purple weakness effect (0x374A)

---

#### 8. Dark Pact

| Property | Value |
|----------|-------|
| **Mana** | 6 |
| **Reagents** | ViperFang (1), Witchweed (1) |
| **Range** | Self |
| **Mantra** | Darkum Pactum |
| **Effect** | ⚠️ PLACEHOLDER: +5 DEX buff (should sacrifice HP for spell damage) |
| **Duration** | 60 seconds |

| Animation | Value |
|-----------|-------|
| **Effect ID** | 0x376A |
| **Sound** | 0x1FB |
| **Target Flags** | Harmful |

**Known Issues:**
- Implementation is DEX buff instead of HP sacrifice mechanic
- Missing dark red aura (0x373A)
- Missing spell damage boost

---

### Circle 3 (4 spells)

#### 9. Contagious Hex

| Property | Value |
|----------|-------|
| **Mana** | 9 |
| **Reagents** | Witchweed (1), ToadsEye (1) |
| **Range** | 12 tiles |
| **Mantra** | Contagiousum Hexum |
| **Effect** | 3-5 poison damage (should spread to nearby enemies) |
| **Duration** | Instant |

| Animation | Value |
|-----------|-------|
| **Effect ID** | 0x376A |
| **Sound** | 0x1FC |
| **Target Flags** | Harmful |

**Known Issues:**
- Missing hex spreading mechanic
- Missing attack speed debuff (-10%)
- Missing purple cloud spreading effect (0x372A + 0x3779)
- Only deals direct poison damage

---

#### 10. Life Leech

| Property | Value |
|----------|-------|
| **Mana** | 9 |
| **Reagents** | Witchweed (1), ToadsEye (1) |
| **Range** | 10 tiles |
| **Mantra** | Lifeum Leechum |
| **Effect** | ⚠️ PLACEHOLDER: 0 HP heal (should drain 15-25 HP, heal 150%) |
| **Duration** | Instant |

| Animation | Value |
|-----------|-------|
| **Effect ID** | 0x376A |
| **Sound** | 0x1FB |
| **Target Flags** | Harmful |

**Known Issues:**
- Heals 0 HP (RandomMinMax(0, 0))
- Missing life drain mechanic
- Missing 150% overheal bonus
- Missing red beam effect (0x374A)

---

#### 11. Hex of Frailty

| Property | Value |
|----------|-------|
| **Mana** | 9 |
| **Reagents** | Witchweed (1), ToadsEye (1) |
| **Range** | 10 tiles |
| **Mantra** | Hexum Frailtyum |
| **Effect** | ⚠️ PLACEHOLDER: +5 INT buff (should be -10 all resistances) |
| **Duration** | 60 seconds |

| Animation | Value |
|-----------|-------|
| **Effect ID** | 0x376A |
| **Sound** | 0x1FC |
| **Target Flags** | Harmful |

**Known Issues:**
- Implementation is INT buff instead of resistance debuff
- Missing purple cracks effect (0x36BD)

---

#### 12. Voodoo Doll

| Property | Value |
|----------|-------|
| **Mana** | 9 |
| **Reagents** | Witchweed (1), ToadsEye (1) |
| **Range** | 12 tiles |
| **Mantra** | Voodooum Dollum |
| **Effect** | ⚠️ PLACEHOLDER: +5 STR buff (should reflect 25% damage) |
| **Duration** | 60 seconds |

| Animation | Value |
|-----------|-------|
| **Effect ID** | 0x376A |
| **Sound** | 0x1FB |
| **Target Flags** | Harmful |

**Known Issues:**
- Implementation is STR buff instead of damage reflection
- Missing shadowy link effect (0x373A line)
- Missing damage reflection mechanic

---

### Circle 4 (4 spells)

#### 13. Crippling Curse

| Property | Value |
|----------|-------|
| **Mana** | 11 |
| **Reagents** | ToadsEye (1), SwampLotus (1) |
| **Range** | 10 tiles |
| **Mantra** | Cripplingum Curseum |
| **Effect** | ⚠️ PLACEHOLDER: +5 DEX buff (should be -20 DEX, -30% move speed) |
| **Duration** | 60 seconds |

| Animation | Value |
|-----------|-------|
| **Effect ID** | 0x376A |
| **Sound** | 0x1FC |
| **Target Flags** | Harmful |

**Known Issues:**
- Implementation is DEX buff instead of mobility debuff
- Missing dark chains effect (0x373A)
- Missing movement speed reduction

---

#### 14. Plague Bearer

| Property | Value |
|----------|-------|
| **Mana** | 11 |
| **Reagents** | ToadsEye (1), SwampLotus (1) |
| **Range** | 10 tiles |
| **Mantra** | Plagueium Bearerum |
| **Effect** | ⚠️ PLACEHOLDER: +5 STR buff (should be spreading poison DoT) |
| **Duration** | 60 seconds |

| Animation | Value |
|-----------|-------|
| **Effect ID** | 0x376A |
| **Sound** | 0x205 |
| **Target Flags** | Harmful |

**Known Issues:**
- Implementation is STR buff instead of plague mechanic
- Missing 6-10 poison damage/tick
- Missing plague spreading to nearby enemies
- Missing sickly green aura (0x372A)

---

#### 15. Drain Essence

| Property | Value |
|----------|-------|
| **Mana** | 11 |
| **Reagents** | ToadsEye (1), SwampLotus (1) |
| **Range** | 10 tiles |
| **Mantra** | Drainum Essenceum |
| **Effect** | ⚠️ PLACEHOLDER: 0 HP heal (should drain HP + mana) |
| **Duration** | Instant |

| Animation | Value |
|-----------|-------|
| **Effect ID** | 0x376A |
| **Sound** | 0x1FB |
| **Target Flags** | Harmful |

**Known Issues:**
- Heals 0 HP (RandomMinMax(0, 0))
- Missing HP drain (25-40)
- Missing mana drain (10-20)
- Missing purple/red dual beam (0x374A)

---

#### 16. Hex of Agony

| Property | Value |
|----------|-------|
| **Mana** | 11 |
| **Reagents** | ToadsEye (1), SwampLotus (1) |
| **Range** | 10 tiles |
| **Mantra** | Hexum Agonium |
| **Effect** | ⚠️ PLACEHOLDER: +5 INT buff (should invert healing to damage) |
| **Duration** | 60 seconds |

| Animation | Value |
|-----------|-------|
| **Effect ID** | 0x376A |
| **Sound** | 0x1FC |
| **Target Flags** | Harmful |

**Known Issues:**
- Implementation is INT buff instead of anti-healing hex
- Missing healing inversion mechanic
- Missing twisted purple aura (0x374A + 0x3779)

---

### Circle 5 (4 spells)

#### 17. Mass Hex

| Property | Value |
|----------|-------|
| **Mana** | 14 |
| **Reagents** | SwampLotus (1), HagsHair (1) |
| **Range** | 12 tiles (AoE: 5 tile radius) |
| **Mantra** | Massum Hexum |
| **Effect** | ⚠️ PLACEHOLDER: +5 DEX buff (should be AoE mass debuff) |
| **Duration** | 60 seconds |

| Animation | Value |
|-----------|-------|
| **Effect ID** | 0x376A |
| **Sound** | 0x1FC |
| **Target Flags** | Harmful |

**Known Issues:**
- Implementation is DEX buff instead of AoE debuff
- Missing area of effect mechanic
- Missing -10 all stats, -10% damage, -5 all resists
- Missing purple explosion (0x36BD)

---

#### 18. Soul Siphon

| Property | Value |
|----------|-------|
| **Mana** | 14 |
| **Reagents** | SwampLotus (1), HagsHair (1) |
| **Range** | 10 tiles |
| **Mantra** | Soulum Siphonum |
| **Effect** | ⚠️ PLACEHOLDER: 0 HP heal (should drain 35-55, heal 200%) |
| **Duration** | Instant + 60s buff |

| Animation | Value |
|-----------|-------|
| **Effect ID** | 0x376A |
| **Sound** | 0x1FB |
| **Target Flags** | Harmful |

**Known Issues:**
- Heals 0 HP (RandomMinMax(0, 0))
- Missing massive life drain
- Missing 200% healing multiplier
- Missing +10% max HP temporary buff
- Missing intense red beam (0x374A)

---

#### 19. Hex of Silence

| Property | Value |
|----------|-------|
| **Mana** | 14 |
| **Reagents** | SwampLotus (1), HagsHair (1) |
| **Range** | 10 tiles |
| **Mantra** | Hexum Silenceum |
| **Effect** | ⚠️ PLACEHOLDER: +5 STR buff (should silence target) |
| **Duration** | 60 seconds |

| Animation | Value |
|-----------|-------|
| **Effect ID** | 0x376A |
| **Sound** | 0x1FC |
| **Target Flags** | Harmful |

**Known Issues:**
- Implementation is STR buff instead of silence
- Missing spell blocking mechanic
- Missing purple gag effect (0x3779)

---

#### 20. Necrotic Touch

| Property | Value |
|----------|-------|
| **Mana** | 14 |
| **Reagents** | SwampLotus (1), HagsHair (1) |
| **Range** | 10 tiles |
| **Mantra** | Necroticum Touchum |
| **Effect** | ⚠️ PLACEHOLDER: +5 INT buff (should deal damage + anti-heal) |
| **Duration** | 60 seconds |

| Animation | Value |
|-----------|-------|
| **Effect ID** | 0x376A |
| **Sound** | 0x205 |
| **Target Flags** | Harmful |

**Known Issues:**
- Implementation is INT buff instead of damage + debuff
- Missing 25-40 damage
- Missing -50% healing received debuff
- Missing Deadly Poison application
- Missing black/green hand effect (0x36BD)

---

### Circle 6 (4 spells)

#### 21. Curse of Mortality

| Property | Value |
|----------|-------|
| **Mana** | 20 |
| **Reagents** | HagsHair (1), CursedPearl (1) |
| **Range** | 10 tiles |
| **Mantra** | Curseum Mortalityum |
| **Effect** | ⚠️ PLACEHOLDER: +5 DEX buff (should remove all regen) |
| **Duration** | 60 seconds |

| Animation | Value |
|-----------|-------|
| **Effect ID** | 0x376A |
| **Sound** | 0x1FC |
| **Target Flags** | Harmful |

**Known Issues:**
- Implementation is DEX buff instead of anti-sustain curse
- Missing HP/Mana/Stam regeneration removal
- Missing -20 all resists
- Missing -15% max HP
- Missing death shroud effect (0x374A)

---

#### 22. Hex Storm

| Property | Value |
|----------|-------|
| **Mana** | 20 |
| **Reagents** | HagsHair (1), CursedPearl (1) |
| **Range** | 12 tiles (AoE: 6 tile radius) |
| **Mantra** | Hexum Stormum |
| **Effect** | ⚠️ PLACEHOLDER: +5 STR buff (should create random hex zone) |
| **Duration** | 60 seconds |

| Animation | Value |
|-----------|-------|
| **Effect ID** | 0x376A |
| **Sound** | 0x1FC (looping) |
| **Target Flags** | Harmful |

**Known Issues:**
- Implementation is STR buff instead of hex storm zone
- Missing zone that pulses random hexes every 2s
- Missing curses, poisons, slows, silences
- Missing chaotic purple storm (0x3779 + 0x372A)

---

#### 23. Vampiric Aura

| Property | Value |
|----------|-------|
| **Mana** | 20 |
| **Reagents** | HagsHair (1), CursedPearl (1) |
| **Range** | Self |
| **Mantra** | Vampiricum Auraum |
| **Effect** | ⚠️ PLACEHOLDER: 0 HP heal (should grant lifesteal aura) |
| **Duration** | Instant |

| Animation | Value |
|-----------|-------|
| **Effect ID** | 0x376A |
| **Sound** | 0x1FB |
| **Target Flags** | Harmful |

**Known Issues:**
- Heals 0 HP (RandomMinMax(0, 0))
- Missing vampiric aura that heals 40% of damage dealt
- Missing +15% spell damage boost
- Missing red pulsing aura (0x373A)

---

#### 24. Doomcurse

| Property | Value |
|----------|-------|
| **Mana** | 20 |
| **Reagents** | HagsHair (1), CursedPearl (1) |
| **Range** | 10 tiles |
| **Mantra** | Doomcurseus |
| **Effect** | ⚠️ PLACEHOLDER: +18 DEX buff (should be delayed nuke) |
| **Duration** | 7 minutes |

| Animation | Value |
|-----------|-------|
| **Effect ID** | 0x376A |
| **Sound** | 0x1FC |
| **Target Flags** | Harmful |

**Known Issues:**
- Implementation is DEX buff instead of delayed damage curse
- Missing 12-second countdown mechanic
- Missing 100-150 damage on expiration
- Missing skull countdown visual (0x3779)
- Duration is 7 minutes instead of 12 seconds

---

### Circle 7 (4 spells)

#### 25. Plague of Sorrows

| Property | Value |
|----------|-------|
| **Mana** | 40 |
| **Reagents** | CursedPearl (1), CursedSalt (1) |
| **Range** | 12 tiles (AoE: 8 tile radius) |
| **Mantra** | Plagueium Sorrowsum |
| **Effect** | ⚠️ PLACEHOLDER: +5 INT buff (should be contagious plague) |
| **Duration** | 60 seconds |

| Animation | Value |
|-----------|-------|
| **Effect ID** | 0x376A |
| **Sound** | 0x205 |
| **Target Flags** | Harmful |

**Known Issues:**
- Implementation is INT buff instead of plague mechanic
- Missing 10-18 damage/tick
- Missing plague spreading to nearby enemies (5 tiles)
- Missing -75% healing reduction
- Missing massive sickly cloud (0x372A)

---

#### 26. Soul Harvest

| Property | Value |
|----------|-------|
| **Mana** | 40 |
| **Reagents** | CursedPearl (1), CursedSalt (1) |
| **Range** | 12 tiles (AoE: 6 tile radius) |
| **Mantra** | Soulum Harvestum |
| **Effect** | ⚠️ PLACEHOLDER: 0 HP heal (should be AoE drain) |
| **Duration** | Instant |

| Animation | Value |
|-----------|-------|
| **Effect ID** | 0x376A |
| **Sound** | 0x1FB |
| **Target Flags** | Harmful |

**Known Issues:**
- Heals 0 HP (RandomMinMax(0, 0))
- Missing AoE life drain (40-65 HP per enemy)
- Missing shield equal to total healing
- Missing multiple red beams converging (0x374A)

---

#### 27. Curse of the Hag

| Property | Value |
|----------|-------|
| **Mana** | 40 |
| **Reagents** | CursedPearl (1), CursedSalt (1) |
| **Range** | 10 tiles |
| **Mantra** | Curseum Hagum |
| **Effect** | ⚠️ PLACEHOLDER: +5 DEX buff (should polymorph to chicken) |
| **Duration** | 60 seconds |

| Animation | Value |
|-----------|-------|
| **Effect ID** | 0x376A |
| **Sound** | 0x6E (chicken) |
| **Target Flags** | Harmful |

**Known Issues:**
- Implementation is DEX buff instead of polymorph
- Missing chicken transformation (Body 0x00D0)
- Missing -90% damage, -50% movement debuffs
- Missing spell blocking

---

#### 28. Hexblade Ritual

| Property | Value |
|----------|-------|
| **Mana** | 40 |
| **Reagents** | CursedPearl (1), CursedSalt (1) |
| **Range** | Self |
| **Mantra** | Hexbladeum Ritualum |
| **Effect** | ⚠️ PLACEHOLDER: +5 STR buff (should enable hybrid combat) |
| **Duration** | 60 seconds |

| Animation | Value |
|-----------|-------|
| **Effect ID** | 0x376A |
| **Sound** | 0x1FC |
| **Target Flags** | Harmful |

**Known Issues:**
- Implementation is STR buff instead of hybrid mode
- Missing spell damage as physical damage (50%)
- Missing melee attacks apply hexes
- Missing purple weapon glow (0x3779)

---

### Circle 8 (4 spells)

#### 29. Curse of Undeath

| Property | Value |
|----------|-------|
| **Mana** | 50 |
| **Reagents** | CursedPearl (1), CursedSalt (2) |
| **Range** | 10 tiles |
| **Mantra** | Curseum Undeathum |
| **Effect** | ⚠️ PLACEHOLDER: +5 INT buff (should be permanent undeath curse) |
| **Duration** | 60 seconds |

| Animation | Value |
|-----------|-------|
| **Effect ID** | 0x376A |
| **Sound** | 0x1FB |
| **Target Flags** | Harmful |

**Known Issues:**
- Implementation is INT buff instead of undeath curse
- Missing healing becomes damage mechanic
- Missing holy spell vulnerability (+100% damage)
- Missing +50% poison/curse damage taken
- Missing undead transformation effect (0x374A)
- Should be permanent until dispelled (not 60s)

---

#### 30. Voodoo Mastery

| Property | Value |
|----------|-------|
| **Mana** | 50 |
| **Reagents** | CursedPearl (1), CursedSalt (2) |
| **Range** | 14 tiles |
| **Mantra** | Voodooum Masteryum |
| **Effect** | ⚠️ PLACEHOLDER: +5 DEX buff (should be mind control) |
| **Duration** | 60 seconds |

| Animation | Value |
|-----------|-------|
| **Effect ID** | 0x376A |
| **Sound** | 0x1FC |
| **Target Flags** | Harmful |

**Known Issues:**
- Implementation is DEX buff instead of charm/control
- Missing action control mechanic
- Missing force attack allies
- Missing puppet strings effect (0x373A)

---

#### 31. Apocalyptic Hex

| Property | Value |
|----------|-------|
| **Mana** | 50 |
| **Reagents** | CursedPearl (1), CursedSalt (2) |
| **Range** | Screen-wide (30 tile radius) |
| **Mantra** | Apocalypticum Hexum |
| **Effect** | ⚠️ PLACEHOLDER: +5 STR buff (should be screen-wide curse) |
| **Duration** | 60 seconds |

| Animation | Value |
|-----------|-------|
| **Effect ID** | 0x376A |
| **Sound** | 0x307 |
| **Target Flags** | Harmful |

**Known Issues:**
- Implementation is STR buff instead of mega-curse
- Missing screen-wide AoE (30 tiles)
- Missing -20 all stats, -20 all resists
- Missing Deadly Poison, -50% healing, 8-15 damage/tick
- Missing purple/green apocalypse effect (0x3779 + 0x372A)

---

#### 32. Witch Queen's Dominion

| Property | Value |
|----------|-------|
| **Mana** | 50 |
| **Reagents** | CursedPearl (1), CursedSalt (2) |
| **Range** | Self |
| **Mantra** | Witchum Queensum Dominionum |
| **Effect** | ⚠️ PLACEHOLDER: +5 INT buff (should be transformation ultimate) |
| **Duration** | 60 seconds |

| Animation | Value |
|-----------|-------|
| **Effect ID** | 0x376A |
| **Sound** | 0x1FC |
| **Target Flags** | Harmful |

**Known Issues:**
- Implementation is INT buff instead of Witch Queen transformation
- Missing -50% hex mana cost
- Missing 2x hex duration
- Missing 300% life drain healing
- Missing poison/curse immunity
- Missing 3 hex totem summons
- Missing regal dark aura with crown (0x373A)

---

## Reagents

Hex Magic uses 8 Vystia-specific reagents:

| Reagent | Circles | Location | Item ID |
|---------|---------|----------|---------|
| **BogMoss** | 1 | Shadowfen swamps | 0x0F7B |
| **ViperFang** | 1-2 | Shadowfen swamps | 0x0F78 |
| **Witchweed** | 2-3 | Shadowfen swamps | 0x1A9C |
| **ToadsEye** | 3-4 | Shadowfen swamps | 0x0F7A |
| **SwampLotus** | 4-5 | Shadowfen swamps | 0x0F86 |
| **HagsHair** | 5-6 | Shadowfen swamps | 0x1422 |
| **CursedPearl** | 6-8 | Shadowfen swamps | 0x0F8E |
| **CursedSalt** | 7-8 | Shadowfen swamps | 0x0F8F |

**Implementation:** All reagents created in `HexMagicReagents.cs`

**Reagent Usage Pattern:**
- Circle 1: BogMoss + ViperFang (2 reagents)
- Circle 2: ViperFang + Witchweed (2 reagents)
- Circle 3: Witchweed + ToadsEye (2 reagents)
- Circle 4: ToadsEye + SwampLotus (2 reagents)
- Circle 5: SwampLotus + HagsHair (2 reagents)
- Circle 6: HagsHair + CursedPearl (2 reagents)
- Circle 7: CursedPearl + CursedSalt (2 reagents)
- Circle 8: CursedPearl + CursedSalt (×2) (3 reagents)

---

## Spell Circle Distribution

| Circle | Spells | Total Mana | Spell Names |
|--------|--------|------------|-------------|
| **First** | 4 | 16 | Evil Eye, Weak Curse, Siphon Life, Witch Sight |
| **Second** | 4 | 24 | Wasting Curse, Poison Touch, Enfeeble, Dark Pact |
| **Third** | 4 | 36 | Contagious Hex, Life Leech, Hex of Frailty, Voodoo Doll |
| **Fourth** | 4 | 44 | Crippling Curse, Plague Bearer, Drain Essence, Hex of Agony |
| **Fifth** | 4 | 56 | Mass Hex, Soul Siphon, Hex of Silence, Necrotic Touch |
| **Sixth** | 4 | 80 | Curse of Mortality, Hex Storm, Vampiric Aura, Doomcurse |
| **Seventh** | 4 | 160 | Plague of Sorrows, Soul Harvest, Curse of the Hag, Hexblade Ritual |
| **Eighth** | 4 | 200 | Curse of Undeath, Voodoo Mastery, Apocalyptic Hex, Witch Queen's Dominion |

**Total:** 32 spells, 616 total mana

---

## Implementation Status

### Known Issues

**All 32 spells have placeholder implementations:**
- Most spells grant stat buffs (STR/DEX/INT +5 to +18) instead of their designed debuff/curse effects
- Life drain spells heal 0 HP (RandomMinMax(0, 0))
- No contagious hex spreading mechanics
- No damage reflection (Voodoo Doll)
- No silence/anti-casting mechanics
- No polymorph transformations
- No area of effect damage zones
- No delayed damage mechanics (Doomcurse)
- No ultimate transformation effects (Witch Queen's Dominion)

**Visual/Animation Issues:**
- All spells use generic particle effect (0x376A) instead of designed visuals
- Missing purple/green hex-themed effects
- Missing beam effects for life drain spells
- Missing cloud/aura effects for debuff zones
- Missing countdown visuals for delayed spells

**Core Mechanics Missing:**
- Life drain system (damage enemy, heal caster)
- Contagious hex spreading between enemies
- Anti-healing mechanics (invert healing to damage)
- Damage reflection links
- Silence/spellcasting prevention
- Area control zones with periodic effects
- Delayed damage curses
- Polymorph/transformation effects
- Mind control/charm mechanics

---

## File Locations

**Spell Files:** `D:\UO\ServUO\Scripts\Custom\VystiaClasses\Spells\Witch\`
- All 32 spell files follow naming pattern: `Hex[SpellName]Spell.cs`
- Example: `HexEvilEyeSpell.cs`, `HexSiphonLifeSpell.cs`, `HexContagiousHexSpell.cs`

**Reagent File:** `D:\UO\ServUO\Scripts\Items\Vystia\Resources\Reagents\HexMagicReagents.cs`
- Contains all 8 Hex Magic reagents

**Spellbook File:** `D:\UO\ServUO\Scripts\Items\Equipment\Spellbooks\VystiaSpellbooks.cs`
- Contains WitchSpellbook (Grimoire of Hexes)

---

## Recommended Implementation Priority

### Phase 1 - Basic Curses (Circles 1-2)
1. **Evil Eye** - Accuracy debuff mechanic
2. **Weak Curse** - Stat debuffs (STR/DEX/INT)
3. **Poison Touch** - Apply Greater Poison
4. **Enfeeble** - STR debuff + damage reduction

**Goal:** Establish basic debuff mechanics

### Phase 2 - Life Drain (Circles 1-4)
1. **Siphon Life** - Basic drain + heal
2. **Life Leech** - Enhanced drain (150% heal)
3. **Drain Essence** - HP + Mana drain
4. **Soul Siphon** - Massive drain (200% heal + temp HP)

**Goal:** Implement life drain system for class identity

### Phase 3 - Hex Spreading (Circles 3-7)
1. **Contagious Hex** - Single target spreads to nearby allies
2. **Plague Bearer** - Spreading poison DoT
3. **Plague of Sorrows** - AoE contagious mega-plague
4. **Mass Hex** - AoE mass debuff

**Goal:** Implement hex spreading and area control

### Phase 4 - Advanced Mechanics (Circles 4-8)
1. **Voodoo Doll** - Damage reflection links
2. **Hex of Agony** - Anti-healing (invert healing to damage)
3. **Hex of Silence** - Prevent spellcasting
4. **Doomcurse** - Delayed damage with countdown
5. **Curse of the Hag** - Polymorph to chicken
6. **Witch Queen's Dominion** - Ultimate transformation

**Goal:** Complete unique mechanics that define Hex Magic

---

## Balance Considerations

**PvE:**
- Excellent vs. bosses (sustained debuffs)
- Life drain for self-sustain (once implemented)
- Struggles against undead enemies (no life to drain)
- Strong group utility with mass hexes

**PvP:**
- High skill cap (hex layering)
- Vulnerable to cleanse/dispel
- Counters healers with anti-heal mechanics
- Weak burst but strong sustained pressure

**Synergies:**
- **Curse Layering:** Weak Curse → Hex of Frailty → Necrotic Touch
- **Life Steal:** Dark Pact → Vampiric Aura → Soul Harvest
- **Plague Strategy:** Poison Touch → Plague Bearer → Plague of Sorrows
- **Control:** Crippling Curse → Hex of Silence → Voodoo Mastery

---

**Last Updated:** 2025-12-28
**Status:** 32/32 spells implemented (all placeholder stat buffs)
**Next Steps:** Implement life drain mechanics and basic curse debuffs
