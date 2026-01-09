# Illusion Magic - Desert School

| Property | Value |
|----------|-------|
| **Class** | Illusionist |
| **Region** | Desert (Sunbaked Expanse) |
| **Theme** | Illusions, mind control, invisibility, trickery, confusion |
| **Spellbook** | Illusionist Spellbook |
| **Hue** | 0x8B0 (Shimmering Gold) |
| **Spell IDs** | 1320-1351 |
| **Status** | 100% Complete (32/32 spells) - **ALL PLACEHOLDER IMPLEMENTATIONS** |

---

## Reagents

All Illusion Magic spells use Vystia reagents found in the Desert of Surya.

| Reagent | Item ID | Circles | Description |
|---------|---------|---------|-------------|
| Mirror Dust | 0x0F8F | 1-3 | Dust from mirrors found in desert mirages |
| Phantom Silk | 0x0F8D | 2-4 | Silk from phantoms in the ethereal desert |
| Mirage Essence | 0x0F0E | 3-5 | Essence of mirages from the desert |
| Dream Crystal | 0x0F8E | 4-6 | Crystal from the dream realm |
| Reality Splinter | 0x0F8A | 5-7 | Splinter of broken reality |
| Void Mirror | 0x0F7A | 6-8 | Mirror showing the void between worlds |
| Chaos Prism | 0x1C18 | 7-8 | Reality-warping prism of chaos |
| Phantom Petal | 0x0F86 | 8 | Petal from the illusion realm itself |

---

## Circle 1 - Basic Illusions (4 Mana)

### 1. Mind Spike

| Property | Value |
|----------|-------|
| **File** | IllusionMindSpikeSpell.cs |
| **Words** | "Mindum Spikeus" |
| **Reagents** | Mirror Dust, Phantom Silk |
| **Range** | 10 tiles |
| **Target** | Single enemy |

**Implemented Effect:**
- Placeholder STR buff (+5 base + 0.2 per Conjuration skill)
- Duration: 1 minute

**Designed Effect:**
- 5-11 psychic damage (energy)

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Target | 0x376A | 0x481 | Waist effect |
| Sound | 0x1F2 | - | Generic effect sound |

---

### 2. Blur

| Property | Value |
|----------|-------|
| **File** | IllusionBlurSpell.cs |
| **Words** | "Blurum" |
| **Reagents** | Mirror Dust, Phantom Silk |
| **Range** | 10 tiles |
| **Target** | Self or ally |

**Implemented Effect:**
- Placeholder STR buff (+5 base + 0.2 per Conjuration skill)
- Duration: 1 minute

**Designed Effect:**
- +10% dodge chance for 30 seconds
- Makes caster partially translucent

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Target | 0x376A | 0x481 | Waist effect |
| Sound | 0x1F2 | - | Generic effect sound |

---

### 3. Minor Illusion

| Property | Value |
|----------|-------|
| **File** | IllusionMinorIllusionSpell.cs |
| **Words** | "Minoris Illusionum" |
| **Reagents** | Mirror Dust, Phantom Silk |
| **Range** | 10 tiles |
| **Target** | Location |

**Implemented Effect:**
- Placeholder STR buff (+5 base + 0.2 per Conjuration skill)
- Duration: 1 minute

**Designed Effect:**
- Create small illusory object (cosmetic only)
- Lasts 60 seconds
- No combat effect

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Target | 0x376A | 0x481 | Waist effect |
| Sound | 0x1F2 | - | Generic effect sound |

---

### 4. Detect Thoughts

| Property | Value |
|----------|-------|
| **File** | IllusionDetectThoughtsSpell.cs |
| **Words** | "Detectus Thoughtum" |
| **Reagents** | Mirror Dust, Phantom Silk |
| **Range** | 10 tiles |
| **Target** | Single creature |

**Implemented Effect:**
- Placeholder STR buff (+5 base + 0.2 per Conjuration skill)
- Duration: 1 minute

**Designed Effect:**
- Read surface thoughts of target
- See intentions (attack/flee/neutral)
- 30 second duration

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Target | 0x376A | 0x481 | Waist effect |
| Sound | 0x1F2 | - | Generic effect sound |

---

## Circle 2 - Trickery (6 Mana)

### 5. Phantom Bolt

| Property | Value |
|----------|-------|
| **File** | IllusionPhantomBoltSpell.cs |
| **Words** | "Phantomum Boltus" |
| **Reagents** | Phantom Silk, Mirage Essence |
| **Range** | 10 tiles |
| **Target** | Single enemy |

**Implemented Effect:**
- Placeholder STR buff (+8 base + 0.2 per Conjuration skill)
- Duration: 2 minutes

**Designed Effect:**
- 10-18 psychic damage (energy)

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Target | 0x376A | 0x481 | Waist effect |
| Sound | 0x1F2 | - | Generic effect sound |

---

### 6. Invisibility

| Property | Value |
|----------|-------|
| **File** | IllusionInvisibilitySpell.cs |
| **Words** | "Invisibilityum" |
| **Reagents** | Phantom Silk, Mirage Essence |
| **Range** | 10 tiles |
| **Target** | Self |

**Implemented Effect:**
- Placeholder STR buff (+8 base + 0.2 per Conjuration skill)
- Duration: 2 minutes

**Designed Effect:**
- Become invisible for 30 seconds
- Breaks on attack/spell cast
- Standard invisibility level

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Target | 0x376A | 0x481 | Waist effect |
| Sound | 0x1F2 | - | Generic effect sound |

---

### 7. Illusory Double

| Property | Value |
|----------|-------|
| **File** | IllusionIllusoryDoubleSpell.cs |
| **Words** | "Illusoryum Doubleum" |
| **Reagents** | Phantom Silk, Mirage Essence |
| **Range** | 3 tiles |
| **Target** | Self |

**Implemented Effect:**
- Placeholder STR buff (+8 base + 0.2 per Conjuration skill)
- Duration: 2 minutes

**Designed Effect:**
- Create 1 illusory copy
- 100 HP, 20% damage
- Lasts 60 seconds or until killed

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Target | 0x376A | 0x481 | Waist effect |
| Sound | 0x1F2 | - | Generic effect sound |

---

### 8. Confuse

| Property | Value |
|----------|-------|
| **File** | IllusionConfuseSpell.cs |
| **Words** | "Confuseum" |
| **Reagents** | Phantom Silk, Mirage Essence |
| **Range** | 10 tiles |
| **Target** | Single enemy |

**Implemented Effect:**
- Placeholder STR buff (+8 base + 0.2 per Conjuration skill)
- Duration: 2 minutes

**Designed Effect:**
- Enemy attacks random target for 8 seconds
- 50% chance to attack allies vs enemies

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Target | 0x376A | 0x481 | Waist effect |
| Sound | 0x1F2 | - | Generic effect sound |

---

## Circle 3 - Mind Games (9 Mana)

### 9. Psychic Scream

| Property | Value |
|----------|-------|
| **File** | IllusionPsychicScreamSpell.cs |
| **Words** | "Psychicum Screamum" |
| **Reagents** | Mirage Essence, Dream Crystal |
| **Range** | 10 tiles |
| **Area** | 4 tile radius |

**Implemented Effect:**
- Placeholder STR buff (+10 base + 0.2 per Conjuration skill)
- Duration: 3 minutes

**Designed Effect:**
- 15-26 psychic damage (AoE)
- Fear for 3 seconds (targets flee)

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Location | 0x376A | 0x481 | Area effect |
| Sound | 0x1F2 | - | Generic effect sound |

---

### 10. Greater Invisibility

| Property | Value |
|----------|-------|
| **File** | IllusionGreaterInvisibilitySpell.cs |
| **Words** | "Majoris Invisibilityum" |
| **Reagents** | Mirage Essence, Dream Crystal |
| **Range** | 10 tiles |
| **Target** | Self |

**Implemented Effect:**
- Placeholder STR buff (+10 base + 0.2 per Conjuration skill)
- Duration: 3 minutes

**Designed Effect:**
- Invisible for 45 seconds
- Only breaks when dealing damage (not on spell cast)

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Target | 0x376A | 0x481 | Waist effect |
| Sound | 0x1F2 | - | Generic effect sound |

---

### 11. Mirror Image

| Property | Value |
|----------|-------|
| **File** | IllusionMirrorImageSpell.cs |
| **Words** | "Mirrorium Imageus" |
| **Reagents** | Mirage Essence, Dream Crystal |
| **Range** | 3 tiles |
| **Target** | Self |

**Implemented Effect:**
- Placeholder STR buff (+10 base + 0.2 per Conjuration skill)
- Duration: 3 minutes

**Designed Effect:**
- Create 2 illusory copies
- 200 HP each
- Lasts 90 seconds or until killed

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Target | 0x376A | 0x481 | Waist effect |
| Sound | 0x1F2 | - | Generic effect sound |

---

### 12. Charm Beast

| Property | Value |
|----------|-------|
| **File** | IllusionCharmBeastSpell.cs |
| **Words** | "Charmum Beastus" |
| **Reagents** | Mirage Essence, Dream Crystal |
| **Range** | 10 tiles |
| **Target** | Beast enemy |

**Implemented Effect:**
- Placeholder STR buff (+10 base + 0.2 per Conjuration skill)
- Duration: 3 minutes

**Designed Effect:**
- Control beast enemy for 30 seconds
- Only works on creature type "Animal"

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Target | 0x376A | 0x481 | Waist effect |
| Sound | 0x1F2 | - | Generic effect sound |

---

## Circle 4 - Advanced Illusions (11 Mana)

### 13. Mind Blast

| Property | Value |
|----------|-------|
| **File** | IllusionMindBlastSpell.cs |
| **Words** | "Mindum Blastus" |
| **Reagents** | Dream Crystal, Reality Splinter |
| **Range** | 10 tiles |
| **Target** | Single enemy |

**Implemented Effect:**
- Placeholder STR buff (+12 base + 0.2 per Conjuration skill)
- Duration: 4 minutes

**Designed Effect:**
- 25-42 psychic damage
- Silence for 4 seconds (cannot cast spells)

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Target | 0x376A | 0x481 | Waist effect |
| Sound | 0x1F2 | - | Generic effect sound |

---

### 14. Illusory Terrain

| Property | Value |
|----------|-------|
| **File** | IllusionIllusoryTerrainSpell.cs |
| **Words** | "Illusoryum Terrainum" |
| **Reagents** | Dream Crystal, Reality Splinter |
| **Range** | 10 tiles |
| **Area** | 8x8 area |

**Implemented Effect:**
- Placeholder STR buff (+12 base + 0.2 per Conjuration skill)
- Duration: 4 minutes

**Designed Effect:**
- Create fake walls/obstacles
- Blocks line of sight
- Lasts 60 seconds

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Target | 0x376A | 0x481 | Waist effect |
| Sound | 0x1F2 | - | Generic effect sound |

---

### 15. Phantasmal Killer

| Property | Value |
|----------|-------|
| **File** | IllusionPhantasmalKillerSpell.cs |
| **Words** | "Phantasmalus Killerus" |
| **Reagents** | Dream Crystal, Reality Splinter |
| **Range** | 10 tiles |
| **Target** | Location |

**Implemented Effect:**
- Placeholder STR buff (+12 base + 0.2 per Conjuration skill)
- Duration: 4 minutes

**Designed Effect:**
- Summon nightmare creature
- 300 HP, fear aura
- Lasts 120 seconds

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Target | 0x376A | 0x481 | Waist effect |
| Sound | 0x1F2 | - | Generic effect sound |

---

### 16. Mass Confusion

| Property | Value |
|----------|-------|
| **File** | IllusionMassConfusionSpell.cs |
| **Words** | "Massus Confusionum" |
| **Reagents** | Dream Crystal, Reality Splinter |
| **Range** | 10 tiles |
| **Area** | 5 tile radius |

**Implemented Effect:**
- Placeholder STR buff (+12 base + 0.2 per Conjuration skill)
- Duration: 4 minutes

**Designed Effect:**
- All enemies confused for 10 seconds
- Enemies attack random targets

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Target | 0x376A | 0x481 | Waist effect |
| Sound | 0x1F2 | - | Generic effect sound |

---

## Circle 5 - Master Illusionist (14 Mana)

### 17. Mind Control

| Property | Value |
|----------|-------|
| **File** | IllusionMindControlSpell.cs |
| **Words** | "Mindum Controlum" |
| **Reagents** | Reality Splinter, Void Mirror |
| **Range** | 10 tiles |
| **Target** | Humanoid enemy |

**Implemented Effect:**
- Placeholder STR buff (+15 base + 0.2 per Conjuration skill)
- Duration: 5 minutes

**Designed Effect:**
- Control humanoid enemy for 20 seconds
- Cannot control players or bosses

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Target | 0x376A | 0x481 | Waist effect |
| Sound | 0x1F2 | - | Generic effect sound |

---

### 18. Perfect Invisibility

| Property | Value |
|----------|-------|
| **File** | IllusionPerfectInvisibilitySpell.cs |
| **Words** | "Perfectum Invisibilityum" |
| **Reagents** | Reality Splinter, Void Mirror |
| **Range** | 10 tiles |
| **Target** | Self |

**Implemented Effect:**
- Placeholder STR buff (+15 base + 0.2 per Conjuration skill)
- Duration: 5 minutes

**Designed Effect:**
- Invisible for 60 seconds
- Does NOT break on any action
- Timed duration only

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Target | 0x376A | 0x481 | Waist effect |
| Sound | 0x1F2 | - | Generic effect sound |

---

### 19. Illusory Army

| Property | Value |
|----------|-------|
| **File** | IllusionIllusoryArmySpell.cs |
| **Words** | "Illusoryum Armyum" |
| **Reagents** | Reality Splinter, Void Mirror |
| **Range** | 5 tiles |
| **Target** | Self |

**Implemented Effect:**
- Placeholder STR buff (+15 base + 0.2 per Conjuration skill)
- Duration: 5 minutes

**Designed Effect:**
- Create 5 illusory warriors
- 250 HP each, 30% damage
- Lasts 120 seconds

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Target | 0x376A | 0x481 | Waist effect |
| Sound | 0x1F2 | - | Generic effect sound |

---

### 20. Psychic Storm

| Property | Value |
|----------|-------|
| **File** | IllusionPsychicStormSpell.cs |
| **Words** | "Psychicum Stormum" |
| **Reagents** | Reality Splinter, Void Mirror |
| **Range** | 12 tiles |
| **Area** | Ground AoE (5 tile radius) |

**Implemented Effect:**
- Placeholder STR buff (+15 base + 0.2 per Conjuration skill)
- Duration: 5 minutes

**Designed Effect:**
- 8-14 psychic damage per tick
- Mass confusion (all targets)
- Lasts 10 seconds (3 ticks)

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Target | 0x376A | 0x481 | Waist effect |
| Sound | 0x1F2 | - | Generic effect sound |

---

## Circle 6 - Grand Deception (20 Mana)

### 21. Dominate Mind

| Property | Value |
|----------|-------|
| **File** | IllusionDominateMindSpell.cs |
| **Words** | "Dominateus Mindum" |
| **Reagents** | Void Mirror, Chaos Prism |
| **Range** | 10 tiles |
| **Target** | Any non-boss enemy |

**Implemented Effect:**
- Placeholder STR buff (+18 base + 0.2 per Conjuration skill)
- Duration: 7 minutes

**Designed Effect:**
- Control any non-boss enemy for 40 seconds
- Works on all creature types except bosses

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Target | 0x376A | 0x481 | Waist effect |
| Sound | 0x1F2 | - | Generic effect sound |

---

### 22. Phase Shift

| Property | Value |
|----------|-------|
| **File** | IllusionPhaseShiftSpell.cs |
| **Words** | "Phaseum Shiftus" |
| **Reagents** | Void Mirror, Chaos Prism |
| **Range** | 10 tiles |
| **Target** | Self |

**Implemented Effect:**
- Placeholder STR buff (+18 base + 0.2 per Conjuration skill)
- Duration: 7 minutes

**Designed Effect:**
- Become intangible
- Immune to physical damage for 20 seconds
- Can still cast spells

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Target | 0x376A | 0x481 | Waist effect |
| Sound | 0x1F2 | - | Generic effect sound |

---

### 23. Legion of Mirrors

| Property | Value |
|----------|-------|
| **File** | IllusionLegionofMirrorsSpell.cs |
| **Words** | "Legionum Mirroris" |
| **Reagents** | Void Mirror, Chaos Prism |
| **Range** | 5 tiles |
| **Target** | Self |

**Implemented Effect:**
- Placeholder STR buff (+18 base + 0.2 per Conjuration skill)
- Duration: 7 minutes

**Designed Effect:**
- Create 10 illusory copies
- 300 HP each
- Lasts 90 seconds

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Target | 0x376A | 0x481 | Waist effect |
| Sound | 0x1F2 | - | Generic effect sound |

---

### 24. Mass Charm

| Property | Value |
|----------|-------|
| **File** | IllusionMassCharmSpell.cs |
| **Words** | "Massus Charmus" |
| **Reagents** | Void Mirror, Chaos Prism |
| **Range** | 10 tiles |
| **Area** | 8 tile radius |

**Implemented Effect:**
- Placeholder STR buff (+18 base + 0.2 per Conjuration skill)
- Duration: 7 minutes

**Designed Effect:**
- Charm all enemies in area for 15 seconds
- Temporary control of multiple enemies

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Target | 0x376A | 0x481 | Waist effect |
| Sound | 0x1F2 | - | Generic effect sound |

---

## Circle 7 - Reality Bending (40 Mana)

### 25. Mind Shatter

| Property | Value |
|----------|-------|
| **File** | IllusionMindShatterSpell.cs |
| **Words** | "Mindum Shatterus" |
| **Reagents** | Chaos Prism, Phantom Petal |
| **Range** | 10 tiles |
| **Target** | Single enemy |

**Implemented Effect:**
- Placeholder STR buff (+22 base + 0.2 per Conjuration skill)
- Duration: 10 minutes

**Designed Effect:**
- 50-85 psychic damage
- Permanent confusion until dispelled

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Target | 0x376A | 0x481 | Waist effect |
| Sound | 0x1F2 | - | Generic effect sound |

---

### 26. True Invisibility

| Property | Value |
|----------|-------|
| **File** | IllusionTrueInvisibilitySpell.cs |
| **Words** | "Trueus Invisibilityum" |
| **Reagents** | Chaos Prism, Phantom Petal |
| **Range** | 10 tiles |
| **Target** | Self |

**Implemented Effect:**
- Placeholder STR buff (+22 base + 0.2 per Conjuration skill)
- Duration: 10 minutes

**Designed Effect:**
- Invisible permanently
- Does NOT break automatically
- Manual cancel only

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Target | 0x376A | 0x481 | Waist effect |
| Sound | 0x1F2 | - | Generic effect sound |

---

### 27. Phantasmal Dragon

| Property | Value |
|----------|-------|
| **File** | IllusionPhantasmalDragonSpell.cs |
| **Words** | "Phantasmalus Draconum" |
| **Reagents** | Chaos Prism, Phantom Petal |
| **Range** | 10 tiles |
| **Target** | Location |

**Implemented Effect:**
- Placeholder STR buff (+22 base + 0.2 per Conjuration skill)
- Duration: 10 minutes

**Designed Effect:**
- Summon illusory dragon
- 2000 HP, fear aura, breath weapon
- Lasts 180 seconds

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Target | 0x376A | 0x481 | Waist effect |
| Sound | 0x1F2 | - | Generic effect sound |

---

### 28. Reality Warp

| Property | Value |
|----------|-------|
| **File** | IllusionRealityWarpSpell.cs |
| **Words** | "Realityum Warpum" |
| **Reagents** | Chaos Prism, Phantom Petal |
| **Range** | 20 tiles |
| **Area** | All visible targets |

**Implemented Effect:**
- Placeholder STR buff (+22 base + 0.2 per Conjuration skill)
- Duration: 10 minutes

**Designed Effect:**
- Swap positions of all enemies and allies
- Random teleportation within 20 tiles
- Chaos repositioning

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Target | 0x376A | 0x481 | Waist effect |
| Sound | 0x1F2 | - | Generic effect sound |

---

## Circle 8 - Ultimate Illusions (50 Mana)

### 29. Apocalyptic Nightmare

| Property | Value |
|----------|-------|
| **File** | IllusionApocalypticNightmareSpell.cs |
| **Words** | "Apocalypseus Nightmareus" |
| **Reagents** | Chaos Prism, Phantom Petal (x2) |
| **Range** | Screen-wide |
| **Area** | All visible enemies |

**Implemented Effect:**
- Placeholder STR buff (+25 base + 0.2 per Conjuration skill)
- Duration: 15 minutes

**Designed Effect:**
- 80-140 psychic damage (screen-wide)
- Mass fear and confusion
- Ultimate AoE attack

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Target | 0x376A | 0x481 | Waist effect |
| Sound | 0x1F2 | - | Generic effect sound |

---

### 30. Master of Puppets

| Property | Value |
|----------|-------|
| **File** | IllusionMasterofPuppetsSpell.cs |
| **Words** | "Masterus Puppetus" |
| **Reagents** | Chaos Prism, Phantom Petal (x2) |
| **Range** | 10 tiles |
| **Area** | 5 tile radius |

**Implemented Effect:**
- Placeholder STR buff (+25 base + 0.2 per Conjuration skill)
- Duration: 15 minutes

**Designed Effect:**
- Control up to 5 enemies simultaneously
- 30 second duration
- Ultimate crowd control

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Target | 0x376A | 0x481 | Waist effect |
| Sound | 0x1F2 | - | Generic effect sound |

---

### 31. Illusory Apocalypse

| Property | Value |
|----------|-------|
| **File** | IllusionIllusoryApocalypseSpell.cs |
| **Words** | "Illusoryum Apocalypseum" |
| **Reagents** | Chaos Prism, Phantom Petal (x2) |
| **Range** | Screen-wide |
| **Area** | All visible enemies |

**Implemented Effect:**
- Placeholder STR buff (+25 base + 0.2 per Conjuration skill)
- Duration: 15 minutes

**Designed Effect:**
- Creates massive illusion of destruction
- All enemies flee in terror for 45 seconds
- Ultimate fear effect

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Target | 0x376A | 0x481 | Waist effect |
| Sound | 0x1F2 | - | Generic effect sound |

---

### 32. Perfect Illusion

| Property | Value |
|----------|-------|
| **File** | IllusionPerfectIllusionSpell.cs |
| **Words** | "Perfectum Illusionum" |
| **Reagents** | Chaos Prism, Phantom Petal (x2) |
| **Range** | Self |
| **Target** | Self |

**Implemented Effect:**
- Placeholder STR buff (+25 base + 0.2 per Conjuration skill)
- Duration: 15 minutes

**Designed Effect:**
- ULTIMATE: All illusion abilities cost no mana
- Infinite illusions
- Unkillable copies
- Lasts 60 seconds

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Target | 0x376A | 0x481 | Waist effect |
| Sound | 0x1F2 | - | Generic effect sound |

---

## Known Issues

### Critical Implementation Issues

1. **ALL SPELLS ARE PLACEHOLDER IMPLEMENTATIONS**
   - Current Effect: All 32 spells only apply temporary STR buff
   - Expected Effect: See "Designed Effect" section for each spell
   - Status: Requires complete reimplementation

2. **No Actual Illusion/Mind Control Mechanics**
   - Missing: Invisibility system
   - Missing: Illusory copy spawning
   - Missing: Mind control/charm mechanics
   - Missing: Confusion/random target mechanics
   - Missing: Fear/flee mechanics

3. **No Custom Animations**
   - All spells use same generic effect (0x376A, 0x1F2)
   - No unique visual identity per spell

4. **No Damage Implementation**
   - Psychic damage spells do no damage
   - Only apply STR buffs

5. **Incorrect Target Flags**
   - Most spells use `TargetFlags.Harmful` even for buffs
   - Should be `TargetFlags.Beneficial` for self-buffs

### File Locations

**Spell Files:** `ServUO/Scripts/Custom/VystiaClasses/Spells/Illusionist/`
- 32 spell files (IllusionMindSpikeSpell.cs through IllusionPerfectIllusionSpell.cs)

**Reagent File:** `ServUO/Scripts/Items/Vystia/Resources/Reagents/IllusionMagicReagents.cs`
- 8 reagent types implemented

**Spellbook:** `ServUO/Scripts/Items/Equipment/Spellbooks/VystiaSpellbooks.cs`
- IllusionistSpellbook class

**Scroll File:** `ServUO/Scripts/Items/Vystia/Scrolls/IllusionScrolls.cs`
- 32 spell scrolls

---

## Implementation Priority

### Phase 1: Core Mechanics (Required)
1. Implement proper psychic damage (energy type)
2. Implement basic invisibility (breaks on action)
3. Implement confusion (random target selection)
4. Implement illusory copy spawning (1-2 copies)

### Phase 2: Advanced Features
1. Implement mind control (temporary control)
2. Implement greater invisibility (breaks on damage only)
3. Implement mass confusion (AoE)
4. Implement fear mechanics (forced movement)

### Phase 3: Ultimate Abilities
1. Implement Legion of Mirrors (10 copies)
2. Implement Master of Puppets (5 controlled)
3. Implement Perfect Illusion (ultimate buff)
4. Implement Reality Warp (position swapping)

### Phase 4: Polish
1. Unique animations per spell
2. Balance tuning
3. Visual effects overhaul
4. Sound effect variety

---

## Design Notes

### Illusion Magic Theme
- **Deception:** Misdirection and trickery
- **Mind Control:** Temporary enemy control
- **Invisibility:** Multiple tiers of stealth
- **Psychic Damage:** Energy-based mental attacks
- **Crowd Control:** Confusion and fear

### Playstyle
- **Stealth Approach:** Use invisibility tiers
- **Confusion Tactics:** Turn enemies against each other
- **Clone Army:** Overwhelm with numbers
- **Mind Control:** Use strongest enemy as ally
- **Escape Artist:** Phase shift and teleportation

---

**Last Updated:** 2025-12-28
**Status:** 100% Complete (All Placeholder) - Requires Full Reimplementation
**Build Status:** 0 errors, compiles successfully
