# Bardic Magic - Multi-Regional School (DEPRECATED)

**Deprecated:** 2026-01-23  
**Replacement:** Songweaving system in `Vystia/design/system_design/VYSTIA_BARD_SYSTEM.md`  
**Plan:** `Vystia/design/system_design/VYSTIA_BARD_IMPLEMENTATION_PLAN.md`

| Property | Value |
|----------|-------|
| **Class** | Bard |
| **Region** | Multi-Regional |
| **Theme** | Songs, sonic damage, buffs, crowd control, inspiration |
| **Spellbook** | Songbook of Legends (BardSpellbook.cs) |
| **Hue** | 0x481 (Golden/Purple) |
| **Spell IDs** | 1288-1319 |
| **Status** | 100% Complete (32/32 spells) |

---

## Reagents

All Bardic Magic spells use Vystia reagents found across multiple regions.

| Reagent | Item ID | Circles | Description |
|---------|---------|---------|-------------|
| Song Petal | 0x0F86 | 1-3 | Petal that sings |
| Echo Dust | 0x0F8F | 2-4 | Dust of echoes |
| Voice Crystal | 0x0F8E | 3-5 | Crystal of voice |
| Muse Essence | 0x1C18 | 4-6 | Essence of muse |
| Harmony Gem | 0x0F7A | 5-7 | Gem of perfect harmony |
| Eternal Note | 0x0F8D | 6-8 | Note that never ends |
| Golden String | 0x1422 | 7-8 | String from golden lyre |
| Dragon Scale | 0x0F78 | 8 | Scale from ancient dragon |

---

## Circle 1 - Basic Songs (4 Mana)

### 1. Discordant Note

| Property | Value |
|----------|-------|
| **File** | BardicDiscordantNoteSpell.cs |
| **Words** | "Discordantum Noteus" |
| **Reagents** | Song Petal, Echo Dust |
| **Range** | 10 tiles |
| **Target** | Single enemy |

**Effect:**
- PLACEHOLDER: +STR buff (5 + skill*0.2 for 1 min)
- INTENDED: 5-11 sonic damage

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Target | 0x376A | 0x481 | Purple sparkles at waist |
| Sound | 0x1F2 | - | Spell sound |

---

### 2. Song of Courage

| Property | Value |
|----------|-------|
| **File** | BardicSongofCourageSpell.cs |
| **Words** | "Songum Ofum Courageus" |
| **Reagents** | Song Petal, Echo Dust |
| **Range** | 10 tiles |
| **Target** | Single ally |

**Effect:**
- PLACEHOLDER: +STR buff (5 + skill*0.2 for 1 min)
- INTENDED: Allies +5 to all stats (30s)

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Target | 0x376A | 0x481 | Purple sparkles at waist |
| Sound | 0x1F2 | - | Spell sound |

---

### 3. Lullaby

| Property | Value |
|----------|-------|
| **File** | BardicLullabySpell.cs |
| **Words** | "Lullabyum" |
| **Reagents** | Song Petal, Echo Dust |
| **Range** | 10 tiles |
| **Target** | Single enemy |

**Effect:**
- PLACEHOLDER: +STR buff (5 + skill*0.2 for 1 min)
- INTENDED: Enemy becomes drowsy, -20% attack speed (20s)

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Target | 0x376A | 0x481 | Purple sparkles at waist |
| Sound | 0x1F2 | - | Spell sound |

---

### 4. Inspire Accuracy

| Property | Value |
|----------|-------|
| **File** | BardicInspireAccuracySpell.cs |
| **Words** | "Inspireum Accuracyum" |
| **Reagents** | Song Petal, Echo Dust |
| **Range** | 10 tiles |
| **Target** | Single ally |

**Effect:**
- PLACEHOLDER: +STR buff (5 + skill*0.2 for 1 min)
- INTENDED: Ally +10% accuracy (30s)

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Target | 0x376A | 0x481 | Purple sparkles at waist |
| Sound | 0x1F2 | - | Spell sound |

---

## Circle 2 - Supportive Songs (6 Mana)

### 5. Sonic Burst

| Property | Value |
|----------|-------|
| **File** | BardicSonicBurstSpell.cs |
| **Words** | "Sonicum Burstum" |
| **Reagents** | Echo Dust, Voice Crystal |
| **Range** | 10 tiles |
| **Target** | Single enemy |

**Effect:**
- PLACEHOLDER: +STR buff (8 + skill*0.2 for 2 min)
- INTENDED: 10-18 sonic damage, 2 tile AoE

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Area | 0x376A | 0x481 | Location particles (9x32) |
| Sound | 0x1F2 | - | Spell sound |

---

### 6. Song of Resilience

| Property | Value |
|----------|-------|
| **File** | BardicSongofResilienceSpell.cs |
| **Words** | "Songum Ofum Resilienceus" |
| **Reagents** | Echo Dust, Voice Crystal |
| **Range** | 10 tiles |
| **Target** | Single ally |

**Effect:**
- PLACEHOLDER: +STR buff (8 + skill*0.2 for 2 min)
- INTENDED: Allies +10 Physical Resist (45s)

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Target | 0x376A | 0x481 | Purple sparkles at waist |
| Sound | 0x1F2 | - | Spell sound |

---

### 7. Dirge of Weakness

| Property | Value |
|----------|-------|
| **File** | BardicDirgeofWeaknessSpell.cs |
| **Words** | "Dirgeum Ofum Weaknessum" |
| **Reagents** | Echo Dust, Voice Crystal |
| **Range** | 10 tiles |
| **Target** | Single enemy |

**Effect:**
- PLACEHOLDER: +STR buff (8 + skill*0.2 for 2 min)
- INTENDED: Enemy -10 STR (25s)

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Target | 0x376A | 0x481 | Purple sparkles at waist |
| Sound | 0x1F2 | - | Spell sound |

---

### 8. Rejuvenating Melody

| Property | Value |
|----------|-------|
| **File** | BardicRejuvenatingMelodySpell.cs |
| **Words** | "Rejuvenatingum Melodyum" |
| **Reagents** | Echo Dust, Voice Crystal |
| **Range** | 10 tiles |
| **Target** | Single ally |

**Effect:**
- PLACEHOLDER: +STR buff (8 + skill*0.2 for 2 min)
- INTENDED: Ally heals 3-5 HP/tick for 30s

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Target | 0x376A | 0x481 | Purple sparkles at waist |
| Sound | 0x1F2 | - | Spell sound |

---

## Circle 3 - Battle Songs (9 Mana)

### 9. Thunderous Chord

| Property | Value |
|----------|-------|
| **File** | BardicThunderousChordSpell.cs |
| **Words** | "Thunderousum Chordum" |
| **Reagents** | Voice Crystal, Muse Essence |
| **Range** | 10 tiles |
| **Target** | Single enemy |

**Effect:**
- PLACEHOLDER: +STR buff (10 + skill*0.2 for 3 min)
- INTENDED: 18-30 sonic damage, stuns 2s

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Target | 0x376A | 0x481 | Purple sparkles at waist |
| Sound | 0x1F2 | - | Spell sound |

---

### 10. War Song

| Property | Value |
|----------|-------|
| **File** | BardicWarSongSpell.cs |
| **Words** | "Warum Songum" |
| **Reagents** | Voice Crystal, Muse Essence |
| **Range** | 10 tiles |
| **Target** | Single ally |

**Effect:**
- PLACEHOLDER: +STR buff (10 + skill*0.2 for 3 min)
- INTENDED: CHANNELED: Allies +15% damage while singing

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Target | 0x376A | 0x481 | Purple sparkles at waist |
| Sound | 0x1F2 | - | Spell sound |

---

### 11. Mesmerize

| Property | Value |
|----------|-------|
| **File** | BardicMesmerizeSpell.cs |
| **Words** | "Mesmerizeum" |
| **Reagents** | Voice Crystal, Muse Essence |
| **Range** | 10 tiles |
| **Target** | Single enemy |

**Effect:**
- PLACEHOLDER: +STR buff (10 + skill*0.2 for 3 min)
- INTENDED: Enemy charmed for 8s (attacks allies)

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Target | 0x376A | 0x481 | Purple sparkles at waist |
| Sound | 0x1F2 | - | Spell sound |

---

### 12. Song of Swiftness

| Property | Value |
|----------|-------|
| **File** | BardicSongofSwiftnessSpell.cs |
| **Words** | "Songum Ofum Swiftnessum" |
| **Reagents** | Voice Crystal, Muse Essence |
| **Range** | 10 tiles |
| **Target** | Single ally |

**Effect:**
- PLACEHOLDER: +STR buff (10 + skill*0.2 for 3 min)
- INTENDED: Allies +25% movement speed (45s)

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Target | 0x376A | 0x481 | Purple sparkles at waist |
| Sound | 0x1F2 | - | Spell sound |

---

## Circle 4 - Advanced Songs (11 Mana)

### 13. Sonic Wave

| Property | Value |
|----------|-------|
| **File** | BardicSonicWaveSpell.cs |
| **Words** | "Sonicum Waveum" |
| **Reagents** | Muse Essence, Harmony Gem |
| **Range** | 10 tiles |
| **Target** | Cone area |

**Effect:**
- PLACEHOLDER: +STR buff (12 + skill*0.2 for 4 min)
- INTENDED: Cone, 20-35 sonic damage, pushes enemies back

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Target | 0x376A | 0x481 | Purple sparkles at waist |
| Sound | 0x1F2 | - | Spell sound |

---

### 14. Ballad of Health

| Property | Value |
|----------|-------|
| **File** | BardicBalladofHealthSpell.cs |
| **Words** | "Balladum Ofum Healthum" |
| **Reagents** | Muse Essence, Harmony Gem |
| **Range** | 10 tiles |
| **Target** | Party area |

**Effect:**
- PLACEHOLDER: +STR buff (12 + skill*0.2 for 4 min)
- INTENDED: CHANNELED: Heals 6-10 HP/tick to allies

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Target | 0x376A | 0x481 | Purple sparkles at waist |
| Sound | 0x1F2 | - | Spell sound |

---

### 15. Cacophony

| Property | Value |
|----------|-------|
| **File** | BardicCacophonySpell.cs |
| **Words** | "Cacophonyum" |
| **Reagents** | Muse Essence, Harmony Gem |
| **Range** | 10 tiles |
| **Target** | AoE enemies |

**Effect:**
- PLACEHOLDER: +STR buff (12 + skill*0.2 for 4 min)
- INTENDED: AoE confusion, enemies attack randomly (10s)

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Target | 0x376A | 0x481 | Purple sparkles at waist |
| Sound | 0x1F2 | - | Spell sound |

---

### 16. Inspire Heroism

| Property | Value |
|----------|-------|
| **File** | BardicInspireHeroismSpell.cs |
| **Words** | "Inspireum Heroismum" |
| **Reagents** | Muse Essence, Harmony Gem |
| **Range** | 10 tiles |
| **Target** | Single ally |

**Effect:**
- PLACEHOLDER: +STR buff (12 + skill*0.2 for 4 min)
- INTENDED: Target ally +30% damage for 30s

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Target | 0x376A | 0x481 | Purple sparkles at waist |
| Sound | 0x1F2 | - | Spell sound |

---

## Circle 5 - Powerful Songs (14 Mana)

### 17. Shattering Scream

| Property | Value |
|----------|-------|
| **File** | BardicShatteringScreamSpell.cs |
| **Words** | "Shatteringum Screamum" |
| **Reagents** | Harmony Gem, Eternal Note |
| **Range** | 10 tiles |
| **Target** | Single enemy |

**Effect:**
- PLACEHOLDER: +STR buff (15 + skill*0.2 for 5 min)
- INTENDED: 28-48 sonic damage, destroys barriers/shields

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Target | 0x376A | 0x481 | Purple sparkles at waist |
| Sound | 0x1F2 | - | Spell sound |

---

### 18. Song of Sanctuary

| Property | Value |
|----------|-------|
| **File** | BardicSongofSanctuarySpell.cs |
| **Words** | "Songum Ofum Sanctuaryum" |
| **Reagents** | Harmony Gem, Eternal Note |
| **Range** | 10 tiles |
| **Target** | Party area |

**Effect:**
- PLACEHOLDER: +STR buff (15 + skill*0.2 for 5 min)
- INTENDED: CHANNELED: Allies take 30% less damage

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Target | 0x376A | 0x481 | Purple sparkles at waist |
| Sound | 0x1F2 | - | Spell sound |

---

### 19. Requiem

| Property | Value |
|----------|-------|
| **File** | BardicRequiemSpell.cs |
| **Words** | "Requiemum" |
| **Reagents** | Harmony Gem, Eternal Note |
| **Range** | 10 tiles |
| **Target** | Single enemy |

**Effect:**
- PLACEHOLDER: +STR buff (15 + skill*0.2 for 5 min)
- INTENDED: DoT, 8-14 sonic damage/tick, silences (12s)

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Target | 0x376A | 0x481 | Purple sparkles at waist |
| Sound | 0x1F2 | - | Spell sound |

---

### 20. Mass Inspire

| Property | Value |
|----------|-------|
| **File** | BardicMassInspireSpell.cs |
| **Words** | "Massum Inspireum" |
| **Reagents** | Harmony Gem, Eternal Note |
| **Range** | 10 tiles |
| **Target** | All allies |

**Effect:**
- PLACEHOLDER: +STR buff (15 + skill*0.2 for 5 min)
- INTENDED: All allies +15% attack speed (45s)

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Target | 0x376A | 0x481 | Purple sparkles at waist |
| Sound | 0x1F2 | - | Spell sound |

---

## Circle 6 - Epic Songs (20 Mana)

### 21. Destructive Resonance

| Property | Value |
|----------|-------|
| **File** | BardicDestructiveResonanceSpell.cs |
| **Words** | "Destructiveus Resonanceus" |
| **Reagents** | Eternal Note, Golden String |
| **Range** | 10 tiles |
| **Target** | 8 tile AoE |

**Effect:**
- PLACEHOLDER: +STR buff (18 + skill*0.2 for 7 min)
- INTENDED: 8 tile AoE, 35-60 sonic damage, shatters equipment

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Area | 0x376A | 0x481 | Location particles (9x32) |
| Sound | 0x1F2 | - | Spell sound |

---

### 22. Epic Ballad

| Property | Value |
|----------|-------|
| **File** | BardicEpicBalladSpell.cs |
| **Words** | "Epicum Balladum" |
| **Reagents** | Eternal Note, Golden String |
| **Range** | 10 tiles |
| **Target** | Party area |

**Effect:**
- PLACEHOLDER: +STR buff (18 + skill*0.2 for 7 min)
- INTENDED: CHANNELED: Allies +25% all stats, +20 all resists

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Target | 0x376A | 0x481 | Purple sparkles at waist |
| Sound | 0x1F2 | - | Spell sound |

---

### 23. Song of Fear

| Property | Value |
|----------|-------|
| **File** | BardicSongofFearSpell.cs |
| **Words** | "Songum Ofum Fearum" |
| **Reagents** | Eternal Note, Golden String |
| **Range** | 10 tiles |
| **Target** | AoE enemies |

**Effect:**
- PLACEHOLDER: +STR buff (18 + skill*0.2 for 7 min)
- INTENDED: AoE, all enemies flee in terror (8s)

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Target | 0x376A | 0x481 | Purple sparkles at waist |
| Sound | 0x1F2 | - | Spell sound |

---

### 24. Crescendo

| Property | Value |
|----------|-------|
| **File** | BardicCrescendoSpell.cs |
| **Words** | "Crescendoum" |
| **Reagents** | Eternal Note, Golden String |
| **Range** | 10 tiles |
| **Target** | AoE enemies |

**Effect:**
- PLACEHOLDER: +STR buff (18 + skill*0.2 for 7 min)
- INTENDED: Channeled AoE damage, ramps up: 5→10→15→20→25/tick

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Area | 0x376A | 0x481 | Location particles (9x32) |
| Sound | 0x1F2 | - | Spell sound |

---

## Circle 7 - Master Songs (40 Mana)

### 25. Symphony of Destruction

| Property | Value |
|----------|-------|
| **File** | BardicSymphonyofDestructionSpell.cs |
| **Words** | "Symphonyum Ofum Destructionum" |
| **Reagents** | Golden String, Dragon Scale |
| **Range** | 10 tiles |
| **Target** | 12 tile AoE |

**Effect:**
- PLACEHOLDER: +STR buff (22 + skill*0.2 for 10 min)
- INTENDED: 12 tile AoE, 50-85 sonic damage, AoE stun 4s

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Target | 0x376A | 0x481 | Purple sparkles at waist |
| Sound | 0x1F2 | - | Spell sound |

---

### 26. Song of the Ancients

| Property | Value |
|----------|-------|
| **File** | BardicSongoftheAncientsSpell.cs |
| **Words** | "Songum Ofum Theum Ancientsum" |
| **Reagents** | Golden String, Dragon Scale |
| **Range** | 10 tiles |
| **Target** | Party area |

**Effect:**
- PLACEHOLDER: +STR buff (22 + skill*0.2 for 10 min)
- INTENDED: CHANNELED: Massive party buff (+40% damage, +30 all resists, +5 HP regen)

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Target | 0x376A | 0x481 | Purple sparkles at waist |
| Sound | 0x1F2 | - | Spell sound |

---

### 27. Sonic Apocalypse

| Property | Value |
|----------|-------|
| **File** | BardicSonicApocalypseSpell.cs |
| **Words** | "Sonicum Apocalypsum" |
| **Reagents** | Golden String, Dragon Scale |
| **Range** | Screen-wide |
| **Target** | All enemies |

**Effect:**
- PLACEHOLDER: +STR buff (22 + skill*0.2 for 10 min)
- INTENDED: Screen-wide, 45-75 sonic damage, mass confusion

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Target | 0x376A | 0x481 | Purple sparkles at waist |
| Sound | 0x1F2 | - | Spell sound |

---

### 28. Legendary Performance

| Property | Value |
|----------|-------|
| **File** | BardicLegendaryPerformanceSpell.cs |
| **Words** | "Legendaryum Performanceum" |
| **Reagents** | Golden String, Dragon Scale |
| **Range** | Self |
| **Target** | Caster |
| **Duration** | 60 seconds |

**Effect:**
- PLACEHOLDER: +STR buff (22 + skill*0.2 for 10 min)
- INTENDED: Transform: All songs can be cast while moving/casting (60s)

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Target | 0x376A | 0x481 | Purple sparkles at waist |
| Sound | 0x1F2 | - | Spell sound |

---

## Circle 8 - Grandmaster Songs (50 Mana)

### 29. Ode to Devastation

| Property | Value |
|----------|-------|
| **File** | BardicOdetoDevastationSpell.cs |
| **Words** | "Odeum Toum Devastationum" |
| **Reagents** | Golden String, Dragon Scale (x2) |
| **Range** | Screen-wide |
| **Target** | All enemies |

**Effect:**
- PLACEHOLDER: +STR buff (25 + skill*0.2 for 15 min)
- INTENDED: Screen-wide, 80-140 sonic damage, destroys all barriers/summons

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Target | 0x376A | 0x481 | Purple sparkles at waist |
| Sound | 0x1F2 | - | Spell sound |

---

### 30. Eternal Symphony

| Property | Value |
|----------|-------|
| **File** | BardicEternalSymphonySpell.cs |
| **Words** | "Eternalum Symphonyum" |
| **Reagents** | Golden String, Dragon Scale (x2) |
| **Range** | 10 tiles |
| **Target** | Party area |

**Effect:**
- PLACEHOLDER: +STR buff (25 + skill*0.2 for 15 min)
- INTENDED: CHANNELED: Ultimate buff (party unkillable while channeling, max 15s)

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Target | 0x376A | 0x481 | Purple sparkles at waist |
| Sound | 0x1F2 | - | Spell sound |

---

### 31. Voice of the Dragon

| Property | Value |
|----------|-------|
| **File** | BardicVoiceoftheDragonSpell.cs |
| **Words** | "Voiceum Ofum Theum Dragonum" |
| **Reagents** | Golden String, Dragon Scale (x2) |
| **Range** | 20 tile cone |
| **Target** | Cone area |

**Effect:**
- PLACEHOLDER: +STR buff (25 + skill*0.2 for 15 min)
- INTENDED: Massive cone (20 tiles), 100-170 sonic damage, permanent deafen

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Target | 0x376A | 0x481 | Purple sparkles at waist |
| Sound | 0x1F2 | - | Spell sound |

---

### 32. Maestro Ascendant

| Property | Value |
|----------|-------|
| **File** | BardicMaestroAscendantSpell.cs |
| **Words** | "Maestroum Ascendantum" |
| **Reagents** | Golden String, Dragon Scale (x2) |
| **Range** | Self |
| **Target** | Caster |
| **Duration** | 60 seconds |

**Effect:**
- PLACEHOLDER: +STR buff (25 + skill*0.2 for 15 min)
- INTENDED: Ultimate form: All songs cost no mana, can channel 3 songs simultaneously

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Target | 0x376A | 0x481 | Purple sparkles at waist |
| Sound | 0x1F2 | - | Spell sound |

---

## Known Issues

### Critical Issues
1. **All Spells Are Placeholder STR Buffs**
   - ALL 32 spells currently apply a simple +STR buff instead of their intended effects
   - Spell power scales with Conjuration skill (bonus = base + skill*0.2)
   - Duration ranges from 1 minute (Circle 1) to 15 minutes (Circle 8)
   - This is a PYTHON GENERATION artifact - the spells need manual implementation

2. **No Sonic Damage Implementation**
   - No spells deal sonic/energy damage yet
   - Need to implement: `AOS.Damage(target, caster, damage, 0, 0, 0, 0, 100)` (100% energy)

3. **No Channeled Song System**
   - War Song, Ballad of Health, Song of Sanctuary, Epic Ballad, Song of the Ancients, Eternal Symphony need channeling
   - Requires custom ChanneledSongManager system (see design notes in old BardicMagic.md)

4. **No AoE Implementation**
   - Sonic Burst, Cacophony, Destructive Resonance, Song of Fear, Crescendo, Symphony of Destruction need area effects
   - Requires `Map.GetMobilesInRange()` loops

5. **Wrong Target Flags**
   - Most buff spells use `TargetFlags.Harmful` instead of `TargetFlags.Beneficial`
   - Many spells should be party-wide instead of single target

### Animation Issues
1. **Uniform Animation IDs**
   - All spells use effect 0x376A (sparkles) and sound 0x1F2
   - Need unique sonic/musical effects per spell

2. **Missing Cone/Line Effects**
   - Sonic Wave (cone), Voice of the Dragon (cone) need directional effects
   - No projectile animations for sonic attacks

### Balance Issues
1. **Mana Costs Not Implemented**
   - All spells inherit base mana from Circle (4/6/9/11/14/20/40/50)
   - May need adjustment when effects are implemented

2. **Reagent Consumption Works**
   - All spells correctly consume their Vystia reagents
   - Reagent combinations match design intent

---

## File Locations

### Spell Implementations
**Path:** `D:\UO\ServUO\Scripts\Custom\VystiaClasses\Spells\Bard\`

All 32 spell files follow naming pattern: `Bardic[SpellName]Spell.cs`

### Reagents
**File:** `D:\UO\ServUO\Scripts\Items\Vystia\Resources\Reagents\BardicMagicReagents.cs`

Contains all 8 Bardic reagent classes.

### Spellbook
**File:** `D:\UO\ServUO\Scripts\Items\Equipment\Spellbooks\VystiaSpellbooks.cs`

Contains `BardSpellbook` class (hue 0x481, holds 32 spells).

---

## Implementation Priority

### Phase 1: Core Damage/Healing (High Priority)
1. **Discordant Note** - Basic sonic damage (5-11)
2. **Sonic Burst** - AoE sonic damage (10-18, 2 tile radius)
3. **Rejuvenating Melody** - HoT (3-5 HP/tick, 30s)
4. **Thunderous Chord** - Damage + stun (18-30 damage, 2s stun)

### Phase 2: Buff System (High Priority)
5. **Song of Courage** - +5 all stats to allies (30s)
6. **Song of Resilience** - +10 Physical Resist (45s)
7. **Inspire Heroism** - +30% damage (30s)
8. **Mass Inspire** - +15% attack speed party-wide (45s)

### Phase 3: Advanced Mechanics (Medium Priority)
9. **War Song** - Channeled +15% damage buff (requires ChanneledSongManager)
10. **Mesmerize** - Charm enemy (8s)
11. **Crescendo** - Ramping AoE (5→10→15→20→25/tick)
12. **Destructive Resonance** - Equipment shattering (35-60 damage + 20% item break)

### Phase 4: Ultimate Spells (Low Priority)
13. **Eternal Symphony** - Party invulnerability while channeling (max 15s)
14. **Maestro Ascendant** - 3 simultaneous channels, no mana cost (60s)
15. **Voice of the Dragon** - 20 tile cone nuke (100-170 damage)

---

**Last Updated:** 2025-12-28
**Status:** 32/32 spells compile, 0/32 spells have correct effects (all are placeholder STR buffs)
