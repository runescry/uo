# Nature Magic - Verdantpeak School

| Property | Value |
|----------|-------|
| **Class** | Druid |
| **Region** | Verdantpeak |
| **Theme** | Shapeshifting, healing, poison/earth damage, plant control |
| **Spellbook** | Codex of the Wild (DruidSpellbook.cs) |
| **Hue** | 0x7D6 (Forest Green) |
| **Spell IDs** | 1064-1095 |
| **Status** | 32/32 spells implemented (placeholder effects) |

---

## Reagents

All Nature Magic spells use Vystia reagents from Verdantpeak forests.

| Reagent | Item ID | Circles | Description |
|---------|---------|---------|-------------|
| WildMoss | 0x0F86 | 1-3 | Luminescent moss from ancient forests |
| Moonpetal | 0x0F8E | 2-4 | Silver flower from moonlit groves |
| DruidBark | 0x1A9C | 1-4 | Sacred tree bark from sacred groves |
| TreantSap | 0x0F0E | 3-6 | Living tree essence from treant groves |
| ElderwoodSeed | 0x0F7A | 5-7 | Ancient tree seed from Verdantpeak heart |
| PrimalVine | 0x0F7D | 6-8 | Magical living vine from primordial forests |
| LivingBark | 0x1C18 | 7-8 | Regenerating bark (existing resource) |
| AncientRoot | 0x0F7B | 8 | Root of ancient treant (ultimate reagent) |

**File:** `ServUO/Scripts/Items/Vystia/Resources/Reagents/NatureMagicReagents.cs`

---

## Circle 1 - Seedling Magic (4 Mana)

### 1. Nature's Touch

| Property | Value |
|----------|-------|
| **File** | NatureNaturesTouchSpell.cs |
| **Words** | "Nature'sum Tactus" |
| **Reagents** | WildMoss (1), Moonpetal (1) |
| **Range** | 8 tiles |
| **Target** | Single, Beneficial |
| **Healing** | 8-15 HP |

**Effect:**
- Direct healing spell (IMPLEMENTED)

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Target | 0x376A | 0x481 | Green sparkles |
| Sound | 0x1F2 | - | Healing sound |

---

### 2. Thorn Dart

| Property | Value |
|----------|-------|
| **File** | NatureThornDartSpell.cs |
| **Words** | "Thornum Dartum" |
| **Reagents** | WildMoss (1), Moonpetal (1) |
| **Range** | 10 tiles |
| **Target** | Single, Harmful |
| **Damage** | 4-10 poison |

**Effect:**
- Basic ranged poison attack (IMPLEMENTED)
- 100% poison damage

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Target | 0x376A | 0x481 | Green particles |
| Sound | 0x5D3 | - | Thorn sound |

---

### 3. Barkskin

| Property | Value |
|----------|-------|
| **File** | NatureBarkskinSpell.cs |
| **Words** | "Barkskinum" |
| **Reagents** | WildMoss (1), Moonpetal (1) |
| **Range** | 10 tiles |
| **Target** | Self or ally |
| **Duration** | 1 minute |

**Effect:**
- Currently: STR buff +5 + (Conjuration x 0.2)
- Design: +5 Physical Resist, +5 Poison Resist

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Target | 0x376A | 0x481 | Brown bark texture |
| Sound | 0x22F | - | Bark sound |

> **Implementation Note:** Currently applies STR buff. Design calls for resistance bonuses.

---

### 4. Detect Life

| Property | Value |
|----------|-------|
| **File** | NatureDetectLifeSpell.cs |
| **Words** | "Detectum Lifeus" |
| **Reagents** | WildMoss (1), Moonpetal (1) |
| **Range** | Self (12 tile radius) |
| **Duration** | 60 seconds |

**Effect:**
- Currently: STR buff +5 + (Conjuration x 0.2)
- Design: Reveals hidden creatures, shows HP bars

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Target | 0x376A | 0x481 | Green pulses |
| Sound | 0x1EA | - | Detection sound |

> **Implementation Note:** Currently applies STR buff. Design calls for detect hidden effect.

---

## Circle 2 - Grove Magic (6 Mana)

### 5. Entangle

| Property | Value |
|----------|-------|
| **File** | NatureEntangleSpell.cs |
| **Words** | "Entangleus" |
| **Reagents** | Moonpetal (1), DruidBark (1) |
| **Range** | 10 tiles |
| **Target** | Single enemy |
| **Duration** | 5 seconds |

**Effect:**
- Currently: STR buff +8 + (Conjuration x 0.2)
- Design: Root (cannot move, can cast/attack)

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Target | 0x376A | 0x481 | Green particles |
| Sound | 0x5D1 | - | Vines sound |

> **Implementation Note:** Currently applies STR buff. Design calls for Frozen = true root effect.

---

### 6. Poison Spores

| Property | Value |
|----------|-------|
| **File** | NaturePoisonSporesSpell.cs |
| **Words** | "Venenum Sporesum" |
| **Reagents** | Moonpetal (1), DruidBark (1) |
| **Range** | 12 tiles |
| **Target** | Single enemy |
| **Duration** | 30 seconds (6 ticks) |

**Effect:**
- DoT: 5 + (Conjuration x 0.1) damage per tick (IMPLEMENTED)
- 6 ticks over 30 seconds

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Initial | 0x376A | 0x481 | Green spore cloud |
| Tick | 0x376A | - | Spore particles |
| Sound | 0x205 | - | Poison sound |

---

### 7. Rejuvenation

| Property | Value |
|----------|-------|
| **File** | NatureRejuvenationSpell.cs |
| **Words** | "Rejuvenationum" |
| **Reagents** | Moonpetal (1), DruidBark (1) |
| **Range** | 8 tiles |
| **Target** | Self or ally |

**Effect:**
- Currently: Direct heal 3-6 HP (single tick)
- Design: HoT 3-6 HP/tick for 10 ticks (30-60 total)

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Target | 0x376A | 0x481 | Green glow |
| Sound | 0x1F2 | - | Healing sound |

> **Implementation Note:** Currently single heal. Design calls for HoT over time.

---

### 8. Animal Aspect: Speed

| Property | Value |
|----------|-------|
| **File** | NatureAnimalAspectSpeedSpell.cs |
| **Words** | "Animalum Aspectum: Speedum" |
| **Reagents** | Moonpetal (1), DruidBark (1) |
| **Range** | 10 tiles |
| **Target** | Self or ally |
| **Duration** | 2 minutes |

**Effect:**
- Currently: STR buff +8 + (Conjuration x 0.2)
- Design: +20 DEX, +15% movement speed

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Target | 0x376A | 0x481 | Rabbit aura |
| Sound | 0x1EA | - | Speed sound |

> **Implementation Note:** Currently applies STR buff. Design calls for DEX + speed buff.

---

## Circle 3 - Wildwood Magic (9 Mana)

### 9. Wild Growth

| Property | Value |
|----------|-------|
| **File** | NatureWildGrowthSpell.cs |
| **Words** | "Wildum Growthum" |
| **Reagents** | DruidBark (1), TreantSap (1) |
| **Range** | 12 tiles (ground) |
| **Area** | 4x4 tiles |
| **Duration** | 20 seconds |

**Effect:**
- Currently: STR buff +10 + (Conjuration x 0.2)
- Design: Blocks LOS, slows enemies 30%

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Area | 0x376A | 0x481 | Dense plants |
| Sound | 0x5D0 | - | Growth sound |

> **Implementation Note:** Currently applies STR buff. Design calls for terrain blocking + slow.

---

### 10. Bear Form (Lesser Shapeshift)

| Property | Value |
|----------|-------|
| **File** | NatureBearFormLesserShapeshiftSpell.cs |
| **Words** | "Bearum Formum (lesserum Shapeshift)um" |
| **Reagents** | DruidBark (1), TreantSap (1) |
| **Range** | Self |
| **Duration** | 180 seconds (3 minutes) |

**Effect:**
- Currently: STR buff +10 + (Conjuration x 0.2)
- Design: +30 STR, +15 Physical Resist, body transform, CANNOT CAST

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Transform | 0x376A | 0x481 | Green particles |
| Sound | 0x22E | - | Bear roar |

> **Implementation Note:** Currently applies STR buff. Design calls for full shapeshift system.

---

### 11. Thorn Volley

| Property | Value |
|----------|-------|
| **File** | NatureThornVolleySpell.cs |
| **Words** | "Thornum Volleyum" |
| **Reagents** | DruidBark (1), TreantSap (1) |
| **Range** | 12 tiles |
| **Area** | 3 tile radius |
| **Damage** | 12-22 poison/physical |

**Effect:**
- Currently: STR buff +10 + (Conjuration x 0.2)
- Design: AoE 50% poison, 50% physical damage

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Area | 0x376A | 0x481 | Multiple thorns |
| Sound | 0x5D3 | - | Volley sound |

> **Implementation Note:** Currently applies STR buff. Design calls for AoE damage.

---

### 12. Nature's Blessing

| Property | Value |
|----------|-------|
| **File** | NatureNaturesBlessingSpell.cs |
| **Words** | "Nature'sum Blessingum" |
| **Reagents** | DruidBark (1), TreantSap (1) |
| **Range** | 8 tiles |
| **Target** | Self or ally |
| **Duration** | 90 seconds |

**Effect:**
- Currently: STR buff +10 + (Conjuration x 0.2)
- Design: +10% max HP, +5 HP regen/tick, +10 Poison Resist

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Target | 0x376A | 0x481 | Green aura |
| Sound | 0x1F2 | - | Blessing sound |

> **Implementation Note:** Currently applies STR buff. Design calls for HP + regen + resist.

---

## Circle 4 - Ancient Grove (11 Mana)

### 13. Wolf Form (Advanced Shapeshift)

| Property | Value |
|----------|-------|
| **File** | NatureWolfFormAdvancedShapeshiftSpell.cs |
| **Words** | "Wolfum Formum (advancedum Shapeshift)um" |
| **Reagents** | TreantSap (1), ElderwoodSeed (1) |
| **Range** | Self |
| **Duration** | 180 seconds (3 minutes) |

**Effect:**
- Currently: STR buff +12 + (Conjuration x 0.2)
- Design: +25 DEX, +30% speed, +15% attack speed, bleed attacks

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Transform | 0x376A | 0x481 | Green flash |
| Sound | 0xE5 | - | Wolf howl |

> **Implementation Note:** Currently applies STR buff. Design calls for DEX + speed shapeshift.

---

### 14. Strangling Vines

| Property | Value |
|----------|-------|
| **File** | NatureStranglingVinesSpell.cs |
| **Words** | "Stranglingum Vinesum" |
| **Reagents** | TreantSap (1), ElderwoodSeed (1) |
| **Range** | 10 tiles |
| **Target** | Single enemy |
| **Duration** | 8 seconds |

**Effect:**
- Currently: STR buff +12 + (Conjuration x 0.2)
- Design: Root + 6-10 damage/tick + -20% attack speed

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Target | 0x376A | 0x481 | Constricting vines |
| Sound | 0x5D1 | - | Vine sound |

> **Implementation Note:** Currently applies STR buff. Design calls for root + DoT + debuff.

---

### 15. Healing Grove

| Property | Value |
|----------|-------|
| **File** | NatureHealingGroveSpell.cs |
| **Words** | "Healingum Groveum" |
| **Reagents** | TreantSap (1), ElderwoodSeed (1) |
| **Range** | 12 tiles (ground) |
| **Area** | 5 tile radius |
| **Duration** | 15 seconds |

**Effect:**
- Currently: STR buff +12 + (Conjuration x 0.2)
- Design: AoE HoT zone, 5-8 HP/tick to allies

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Area | 0x376A | 0x481 | Green healing circle |
| Sound | 0x1F2 | - | Grove sound |

> **Implementation Note:** Currently applies STR buff. Design calls for ground HoT zone.

---

### 16. Toxic Bloom

| Property | Value |
|----------|-------|
| **File** | NatureToxicBloomSpell.cs |
| **Words** | "Toxicum Bloomum" |
| **Reagents** | TreantSap (1), ElderwoodSeed (1) |
| **Range** | 12 tiles |
| **Area** | 4 tile radius |
| **Damage** | 15-25 poison + DoT |

**Effect:**
- Currently: STR buff +12 + (Conjuration x 0.2)
- Design: AoE poison burst + 5 damage/tick for 10s

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Explosion | 0x376A | 0x481 | Poisonous flower burst |
| Sound | 0x205 | - | Bloom sound |

> **Implementation Note:** Currently applies STR buff. Design calls for AoE + poison DoT.

---

## Circle 5 - Primal Magic (14 Mana)

### 17. Hawk Form (Aerial Shapeshift)

| Property | Value |
|----------|-------|
| **File** | NatureHawkFormAerialShapeshiftSpell.cs |
| **Words** | "Hawkum Formum (aerialum Shapeshift)um" |
| **Reagents** | ElderwoodSeed (1), PrimalVine (1) |
| **Range** | Self |
| **Duration** | 120 seconds (2 minutes) |

**Effect:**
- Currently: STR buff +15 + (Conjuration x 0.2)
- Design: +40 DEX, flying, -50% ranged damage taken, weak melee

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Transform | 0x376A | 0x481 | Green flash |
| Sound | 0x1E3 | - | Bird cry |

> **Implementation Note:** Currently applies STR buff. Design calls for flying shapeshift.

---

### 18. Earthquake

| Property | Value |
|----------|-------|
| **File** | NatureEarthquakeSpell.cs |
| **Words** | "Earthquakeus" |
| **Reagents** | ElderwoodSeed (1), PrimalVine (1) |
| **Range** | Self (8 tile radius) |
| **Damage** | 20-35 physical |

**Effect:**
- Currently: STR buff +15 + (Conjuration x 0.2)
- Design: AoE damage, 2s knockdown, destroys barriers

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Area | 0x376A | 0x481 | Ground shaking |
| Sound | 0x307 | - | Earthquake rumble |

> **Implementation Note:** Currently applies STR buff. Design calls for AoE stun + damage.

---

### 19. Greater Regeneration

| Property | Value |
|----------|-------|
| **File** | NatureGreaterRegenerationSpell.cs |
| **Words** | "Greaterum Regenerationum" |
| **Reagents** | ElderwoodSeed (1), PrimalVine (1) |
| **Range** | 8 tiles |
| **Target** | Self or ally |
| **Duration** | 45 seconds (15 ticks) |

**Effect:**
- Currently: STR buff +15 + (Conjuration x 0.2)
- Design: HoT 8-12 HP/tick for 15 ticks (120-180 total)

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Target | 0x376A | 0x481 | Intense green glow |
| Sound | 0x1F2 | - | Regeneration sound |

> **Implementation Note:** Currently applies STR buff. Design calls for powerful HoT.

---

### 20. Spore Cloud

| Property | Value |
|----------|-------|
| **File** | NatureSporeCloudSpell.cs |
| **Words** | "Sporeum Cloudum" |
| **Reagents** | ElderwoodSeed (1), PrimalVine (1) |
| **Range** | 12 tiles (ground) |
| **Area** | 6 tile radius |
| **Duration** | 12 seconds |

**Effect:**
- Currently: STR buff +15 + (Conjuration x 0.2)
- Design: Poison cloud 5-9 damage/tick, -25% accuracy, obscures vision

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Cloud | 0x376A | 0x481 | Thick green fog |
| Sound | 0x205 | - | Cloud sound |

> **Implementation Note:** Currently applies STR buff. Design calls for zone control DoT.

---

## Circle 6 - Elder Grove (20 Mana)

### 21. Treant Form (Greater Shapeshift)

| Property | Value |
|----------|-------|
| **File** | NatureTreantFormGreaterShapeshiftSpell.cs |
| **Words** | "Treantum Formum (greaterum Shapeshift)um" |
| **Reagents** | PrimalVine (1), LivingBark (1) |
| **Range** | Self |
| **Duration** | 180 seconds (3 minutes) |

**Effect:**
- Currently: STR buff +18 + (Conjuration x 0.2)
- Design: +50 STR, +30 Physical Resist, +25% max HP, AoE root on attacks

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Transform | 0x376A | 0x481 | Massive growth |
| Sound | 0x5DC | - | Treant roar |

> **Implementation Note:** Currently applies STR buff. Design calls for tank shapeshift.

---

### 22. Swarm

| Property | Value |
|----------|-------|
| **File** | NatureSwarmSpell.cs |
| **Words** | "Swarmum" |
| **Reagents** | PrimalVine (1), LivingBark (1) |
| **Range** | 14 tiles |
| **Area** | 5 tile radius |
| **Duration** | 10 seconds |

**Effect:**
- Currently: STR buff +18 + (Conjuration x 0.2)
- Design: 6-10 damage/tick, -40% accuracy, interrupts spellcasting

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Swarm | 0x376A | 0x481 | Insect cloud |
| Sound | 0x5D4 | - | Buzzing |

> **Implementation Note:** Currently applies STR buff. Design calls for anti-caster AoE.

---

### 23. Living Fortress

| Property | Value |
|----------|-------|
| **File** | NatureLivingFortressSpell.cs |
| **Words** | "Livingum Fortressum" |
| **Reagents** | PrimalVine (1), LivingBark (1) |
| **Range** | Self |
| **Duration** | 60 seconds |

**Effect:**
- Currently: STR buff +18 + (Conjuration x 0.2)
- Design: +20 Physical/Poison Resist, +15% max HP, immune to root/slow, 5 HP/tick

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Caster | 0x376A | 0x481 | Thick bark covering |
| Sound | 0x22F | - | Bark sound |

> **Implementation Note:** Currently applies STR buff. Design calls for ultimate defense buff.

---

### 24. Nature's Wrath

| Property | Value |
|----------|-------|
| **File** | NatureNaturesWrathSpell.cs |
| **Words** | "Nature'sum Wrathum" |
| **Reagents** | PrimalVine (1), LivingBark (1) |
| **Range** | 14 tiles (direction) |
| **Area** | 8 tile cone |
| **Damage** | 35-55 poison/physical |

**Effect:**
- Currently: STR buff +18 + (Conjuration x 0.2)
- Design: Cone AoE damage + poison DoT + root all targets

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Cone | 0x376A | 0x481 | Massive plant surge |
| Sound | 0x5D0 | - | Nature's fury |

> **Implementation Note:** Currently applies STR buff. Design calls for cone AoE combo.

---

## Circle 7 - Primordial Magic (40 Mana)

### 25. Force of Nature

| Property | Value |
|----------|-------|
| **File** | NatureForceofNatureSpell.cs |
| **Words** | "Forceum Ofum Natureus" |
| **Reagents** | LivingBark (1), AncientRoot (1) |
| **Range** | Self |
| **Duration** | 30 seconds |

**Effect:**
- Currently: STR buff +22 + (Conjuration x 0.2)
- Design: Rapid form cycling (changes every 5s), all bonuses stacked, CAN CAST

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Transform | 0x376A | 0x481 | Flickering forms |
| Sound | - | - | Multiple transform sounds |

> **Implementation Note:** Currently applies STR buff. Design calls for hybrid shapeshift.

---

### 26. Summon Ancient Treant

| Property | Value |
|----------|-------|
| **File** | NatureSummonAncientTreantSpell.cs |
| **Words** | "Summonum Ancientum Treantum" |
| **Reagents** | LivingBark (1), AncientRoot (1) |
| **Range** | 12 tiles |
| **Duration** | 240 seconds (4 minutes) |

**Effect:**
- Currently: STR buff +22 + (Conjuration x 0.2)
- Design: Summons Ancient Treant (1200 HP, AoE root, high resists)

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Summon | 0x376A | 0x481 | Tree grows from ground |
| Sound | 0x5DC | - | Treant awakening |

> **Implementation Note:** Currently applies STR buff. Design calls for powerful summon.

---

### 27. Plague

| Property | Value |
|----------|-------|
| **File** | NaturePlagueSpell.cs |
| **Words** | "Plagueaus" |
| **Reagents** | LivingBark (1), AncientRoot (1) |
| **Range** | 12 tiles |
| **Area** | 8 tile radius |
| **Duration** | 20 seconds |

**Effect:**
- Currently: STR buff +22 + (Conjuration x 0.2)
- Design: 10-15 poison damage/tick, spreads to nearby enemies within 3 tiles

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Initial | 0x376A | 0x481 | Sickly green aura |
| Spread | 0x376A | - | Infection particles |
| Sound | 0x205 | - | Plague sound |

> **Implementation Note:** Currently applies STR buff. Design calls for spreading plague mechanic.

---

### 28. Primordial Restoration

| Property | Value |
|----------|-------|
| **File** | NaturePrimordialRestorationSpell.cs |
| **Words** | "Primordialum Restorationum" |
| **Reagents** | LivingBark (1), AncientRoot (1) |
| **Range** | 10 tiles |
| **Area** | 8 tile radius |
| **Healing** | 80-120 HP |

**Effect:**
- Currently: STR buff +22 + (Conjuration x 0.2)
- Design: AoE heal, removes all poisons/curses, 15s debuff immunity

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Explosion | 0x376A | 0x481 | Massive green burst |
| Sound | 0x1F2 | - | Restoration sound |

> **Implementation Note:** Currently applies STR buff. Design calls for AoE cleanse + heal.

---

## Circle 8 - Legendary Nature (50 Mana)

### 29. World Tree's Embrace

| Property | Value |
|----------|-------|
| **File** | NatureWorldTreesEmbraceSpell.cs |
| **Words** | "Worldum Tree'sum Embraceum" |
| **Reagents** | LivingBark (1), AncientRoot (2) |
| **Range** | Self (12 tile radius) |
| **Duration** | 45 seconds |

**Effect:**
- Currently: STR buff +26 + (Conjuration x 0.2)
- Design: Massive tree zone - Allies: +50% HP, 10 HP/tick, +25 all resists / Enemies: 8-15 damage/tick, -75% speed

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Tree | 0x376A | 0x481 | Enormous tree |
| Sound | 0x5DC | - | World Tree sound |

> **Implementation Note:** Currently applies STR buff. Design calls for zone transformation.

---

### 30. Hydra Form (Legendary Shapeshift)

| Property | Value |
|----------|-------|
| **File** | NatureHydraFormLegendaryShapeshiftSpell.cs |
| **Words** | "Hydraum Formum (legendaryum Shapeshift)um" |
| **Reagents** | LivingBark (1), AncientRoot (2) |
| **Range** | Self |
| **Duration** | 120 seconds (2 minutes) |

**Effect:**
- Currently: STR buff +26 + (Conjuration x 0.2)
- Design: +70 STR, +30 DEX, triple attack, 15 HP/tick regen, poison immunity

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Transform | 0x376A | 0x481 | Massive transformation |
| Sound | 0x16A | - | Hydra roar |

> **Implementation Note:** Currently applies STR buff. Design calls for ultimate combat form.

---

### 31. Thorn Apocalypse

| Property | Value |
|----------|-------|
| **File** | NatureThornApocalypseSpell.cs |
| **Words** | "Thornum Apocalypseaus" |
| **Reagents** | LivingBark (1), AncientRoot (2) |
| **Range** | Self (30 tile radius) |
| **Damage** | 60-100 poison/physical |

**Effect:**
- Currently: STR buff +26 + (Conjuration x 0.2)
- Design: Screen-wide devastation, bleed + poison DoTs, 5s root

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Area | 0x376A | 0x481 | Thorns everywhere |
| Sound | 0x5D3 | - | Apocalypse sound |

> **Implementation Note:** Currently applies STR buff. Design calls for screen-wide AoE.

---

### 32. Avatar of the Forest

| Property | Value |
|----------|-------|
| **File** | NatureAvataroftheForestSpell.cs |
| **Words** | "Avatarum Ofum Theum Forestum" |
| **Reagents** | LivingBark (1), AncientRoot (2) |
| **Range** | Self |
| **Duration** | 60 seconds |

**Effect:**
- Currently: STR buff +26 + (Conjuration x 0.2)
- Design: Instant shapeshifting while casting, -50% spell costs, immune poison/fire, +40 all resists, summons 3 treants

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Aura | 0x376A | 0x481 | Forest imagery aura |
| Sound | 0x5DC | - | Avatar transformation |

> **Implementation Note:** Currently applies STR buff. Design calls for transformation ultimate.

---

## Testing Commands

```
[spellbook druid              - Spawn Druid Spellbook
[add WildMoss 100             - Circle 1-3 reagent
[add Moonpetal 100            - Circle 2-4 reagent
[add DruidBark 100            - Circle 1-4 reagent
[add TreantSap 100            - Circle 3-6 reagent
[add ElderwoodSeed 100        - Circle 5-7 reagent
[add PrimalVine 100           - Circle 6-8 reagent
[add LivingBark 100           - Circle 7-8 reagent
[add AncientRoot 100          - Circle 8 reagent
```

---

## Known Issues

| Issue | Status | Notes |
|-------|--------|-------|
| Most spells use placeholder STR buff | Needs Fix | Only Nature's Touch, Thorn Dart, and Poison Spores have proper effects |
| No shapeshift system | Needs Fix | All 5 shapeshift spells need ShapeshiftManager implementation |
| No terrain/zone system | Needs Fix | Wild Growth, Healing Grove, Spore Cloud, World Tree need custom items |
| Rejuvenation is single heal | Needs Fix | Should be HoT over 10 ticks |
| No plague spreading mechanic | Needs Fix | Plague needs custom spreading DoT system |
| Reagents are correct | Working | All spells use proper Vystia reagents |

---

## File Locations

| Type | Path |
|------|------|
| Spells | `ServUO/Scripts/Custom/VystiaClasses/Spells/Druid/` |
| Reagents | `ServUO/Scripts/Items/Vystia/Resources/Reagents/NatureMagicReagents.cs` |
| Spellbook | `ServUO/Scripts/Items/Equipment/Spellbooks/VystiaSpellbooks.cs` |
| Scrolls | `ServUO/Scripts/Items/Vystia/Scrolls/NatureMagicScrolls.cs` |
| Vendor | `ServUO/Scripts/Mobiles/Vystia/Vendors/MagicSchoolVendors.cs` |

---

*Last Updated: 2025-12-28*
*Status: Documented - Effects need implementation*
