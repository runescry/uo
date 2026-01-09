# Elemental Magic - Emberlands School

**Class:** Sorcerer
**Region:** Emberlands
**Theme:** Fire/lava, earth/magma, combined elemental fury
**Spellbook:** Tome of Elemental Fury (SorcererSpellbook.cs)
**Primary Stat:** Intelligence
**Hue:** 0x54E (Fiery Orange)

**Implementation Status:** ✅ **32/32 spells implemented**

---

## Reagent Summary

| Reagent | Circles | Source | Class |
|---------|---------|--------|-------|
| Ash Petal | 1-3 | Emberlands volcanoes | AshPetal |
| Lava Glass | 2-4 | Emberlands volcanoes | LavaGlass |
| Flameweed | 3-5 | Emberlands volcanoes | Flameweed |
| Magma Essence | 4-6 | Emberlands volcanoes | MagmaEssence |
| Phoenix Feather | 5-7 | Emberlands volcanoes | PhoenixFeather |
| Dragon Heart | 6-8 | Emberlands volcanoes | DragonHeart |
| Primordial Ember | 7-8 | Emberlands volcanoes | PrimordialEmber |
| Elemental Core | 8 | Emberlands volcanoes | ElementalCore |

**File:** `ServUO/Scripts/Items/Vystia/Resources/Reagents/ElementalMagicReagents.cs`

---

## Circle 1 - Apprentice (4 mana)

### Flame Bolt
| Property | Value |
|----------|-------|
| **Power Words** | "Flameus Sagitta" |
| **Target** | Single, Harmful |
| **Range** | 10 tiles |
| **Damage** | 5-10 + (Conjuration x 0.5) fire |
| **Reagents** | Ash Petal (1), Lava Glass (1) |
| **File** | ElementalFlameBoltSpell.cs |

| Animation | Type | ID | Hue | Description |
|-----------|------|-----|-----|-------------|
| Particle | FixedParticles | 0x376A | 0x481 | Fire burst at target |
| Sound | PlaySound | 0x1F2 | - | Fire impact |

---

### Molten Touch
| Property | Value |
|----------|-------|
| **Power Words** | "Moltenus Touchum" |
| **Target** | Single, Harmful |
| **Range** | 10 tiles |
| **Effect** | STR buff (+bonus, 1 min) |
| **Reagents** | Ash Petal (1), Lava Glass (1) |
| **File** | ElementalMoltenTouchSpell.cs |

| Animation | Type | ID | Hue | Description |
|-----------|------|-----|-----|-------------|
| Particle | FixedParticles | 0x376A | 0x481 | Fire burst at target |
| Sound | PlaySound | 0x1F2 | - | Fire impact |

> **Implementation Note:** Currently applies STR buff. Design calls for melee fire attack with ignite effect.

---

### Heat Shield
| Property | Value |
|----------|-------|
| **Power Words** | "Heatus Shieldus" |
| **Target** | Single, Beneficial |
| **Range** | 10 tiles |
| **Effect** | STR buff (+4 + Conjuration x 0.2, 1 min) |
| **Reagents** | Ash Petal (1), Lava Glass (1) |
| **File** | ElementalHeatShieldSpell.cs |

| Animation | Type | ID | Hue | Description |
|-----------|------|-----|-----|-------------|
| Particle | FixedParticles | 0x376A | 0x481 | Fire aura |
| Sound | PlaySound | 0x1F2 | - | Shield activation |

> **Implementation Note:** Currently applies STR buff. Design calls for +5 Fire Resist for 30s.

---

### Smoke Screen
| Property | Value |
|----------|-------|
| **Power Words** | "Smokeus Screenum" |
| **Target** | Single, Harmful |
| **Range** | 10 tiles |
| **Effect** | STR buff (+4 + Conjuration x 0.2, 1 min) |
| **Reagents** | Ash Petal (1), Lava Glass (1) |
| **File** | ElementalSmokeScreenSpell.cs |

| Animation | Type | ID | Hue | Description |
|-----------|------|-----|-----|-------------|
| Particle | FixedParticles | 0x376A | 0x481 | Smoke particles |
| Sound | PlaySound | 0x1F2 | - | Smoke sound |

> **Implementation Note:** Currently applies STR buff. Design calls for obscuring smoke, -20% accuracy.

---

## Circle 2 - Journeyman (6 mana)

### Fireball
| Property | Value |
|----------|-------|
| **Power Words** | "Fireballum" |
| **Target** | Single, Harmful |
| **Range** | 10 tiles |
| **Effect** | STR buff (+8 + Conjuration x 0.2, 2 min) |
| **Reagents** | Lava Glass (1), Flameweed (1) |
| **File** | ElementalFireballSpell.cs |

| Animation | Type | ID | Hue | Description |
|-----------|------|-----|-----|-------------|
| Particle | SendLocationParticles | 0x376A | 0x481 | Fire explosion |
| Sound | PlaySound | 0x1F2 | - | Fire impact |

> **Implementation Note:** Currently applies STR buff. Design calls for 10-18 fire damage, small AoE (2 tiles).

---

### Lava Puddle
| Property | Value |
|----------|-------|
| **Power Words** | "Lavaus Puddleum" |
| **Target** | Single, Harmful |
| **Range** | 10 tiles |
| **Effect** | STR buff (+6 + Conjuration x 0.2, 2 min) |
| **Reagents** | Lava Glass (1), Flameweed (1) |
| **File** | ElementalLavaPuddleSpell.cs |

| Animation | Type | ID | Hue | Description |
|-----------|------|-----|-----|-------------|
| Particle | FixedParticles | 0x376A | 0x481 | Lava effect |
| Sound | PlaySound | 0x1F2 | - | Bubbling lava |

> **Implementation Note:** Currently applies STR buff. Design calls for ground effect, 4-7 damage/tick.

---

### Ember Burst
| Property | Value |
|----------|-------|
| **Power Words** | "Emberus Burstum" |
| **Target** | Single, Harmful |
| **Range** | 10 tiles |
| **Effect** | STR buff (+6 + Conjuration x 0.2, 2 min) |
| **Reagents** | Lava Glass (1), Flameweed (1) |
| **File** | ElementalEmberBurstSpell.cs |

| Animation | Type | ID | Hue | Description |
|-----------|------|-----|-----|-------------|
| Particle | SendLocationParticles | 0x376A | 0x481 | Ember explosion |
| Sound | PlaySound | 0x1F2 | - | Burst sound |

> **Implementation Note:** Currently applies STR buff. Design calls for short range cone, 8-15 damage.

---

### Flame Ward
| Property | Value |
|----------|-------|
| **Power Words** | "Flameus Wardus" |
| **Target** | Single, Beneficial |
| **Range** | 10 tiles |
| **Effect** | STR buff (+6 + Conjuration x 0.2, 2 min) |
| **Reagents** | Lava Glass (1), Flameweed (1) |
| **File** | ElementalFlameWardSpell.cs |

| Animation | Type | ID | Hue | Description |
|-----------|------|-----|-----|-------------|
| Particle | FixedParticles | 0x376A | 0x481 | Fire ward aura |
| Sound | PlaySound | 0x1F2 | - | Ward activation |

> **Implementation Note:** Currently applies STR buff. Design calls for 10 fire damage reflection on melee hits.

---

## Circle 3 - Expert (9 mana)

### Incinerate
| Property | Value |
|----------|-------|
| **Power Words** | "Incineratum" |
| **Target** | Single, Harmful |
| **Range** | 10 tiles |
| **Effect** | STR buff (+8 + Conjuration x 0.2, 3 min) |
| **Reagents** | Flameweed (1), Magma Essence (1) |
| **File** | ElementalIncinerateSpell.cs |

| Animation | Type | ID | Hue | Description |
|-----------|------|-----|-----|-------------|
| Particle | SendLocationParticles | 0x376A | 0x481 | Incineration effect |
| Sound | PlaySound | 0x1F2 | - | Fire roar |

> **Implementation Note:** Currently applies STR buff. Design calls for 20-35 fire damage.

---

### Volcanic Rock
| Property | Value |
|----------|-------|
| **Power Words** | "Volcanicus Rockum" |
| **Target** | Single, Harmful |
| **Range** | 10 tiles |
| **Effect** | STR buff (+8 + Conjuration x 0.2, 3 min) |
| **Reagents** | Flameweed (1), Magma Essence (1) |
| **File** | ElementalVolcanicRockSpell.cs |

| Animation | Type | ID | Hue | Description |
|-----------|------|-----|-----|-------------|
| Particle | FixedParticles | 0x376A | 0x481 | Rock projectile |
| Sound | PlaySound | 0x1F2 | - | Rock impact |

> **Implementation Note:** Currently applies STR buff. Design calls for 15-25 damage with 1s stun.

---

### Ring of Fire
| Property | Value |
|----------|-------|
| **Power Words** | "Ringum Ofum Fireus" |
| **Target** | Single, Harmful |
| **Range** | 10 tiles |
| **Effect** | STR buff (+8 + Conjuration x 0.2, 3 min) |
| **Reagents** | Flameweed (1), Magma Essence (1) |
| **File** | ElementalRingofFireSpell.cs |

| Animation | Type | ID | Hue | Description |
|-----------|------|-----|-----|-------------|
| Particle | SendLocationParticles | 0x376A | 0x481 | Ring of fire |
| Sound | PlaySound | 0x1F2 | - | Fire whoosh |

> **Implementation Note:** Currently applies STR buff. Design calls for circle around caster, 3-6 damage/tick.

---

### Ignite
| Property | Value |
|----------|-------|
| **Power Words** | "Igniteus" |
| **Target** | Single, Harmful |
| **Range** | 10 tiles |
| **Effect** | STR buff (+10 + Conjuration x 0.2, 3 min) |
| **Reagents** | Flameweed (1), Magma Essence (1) |
| **File** | ElementalIgniteSpell.cs |

| Animation | Type | ID | Hue | Description |
|-----------|------|-----|-----|-------------|
| Particle | FixedParticles | 0x376A | 0x481 | Ignite flames |
| Sound | PlaySound | 0x1F2 | - | Ignition |

> **Implementation Note:** Currently applies STR buff. Design calls for burning DoT (5-10/tick, 10s) that spreads to nearby enemies.

---

## Circle 4 - Adept (11 mana)

### Flame Pillar
| Property | Value |
|----------|-------|
| **Power Words** | "Flameus Pillarum" |
| **Target** | Single, Harmful |
| **Range** | 10 tiles |
| **Effect** | STR buff (+10 + Conjuration x 0.2, 4 min) |
| **Reagents** | Magma Essence (1), Phoenix Feather (1) |
| **File** | ElementalFlamePillarSpell.cs |

| Animation | Type | ID | Hue | Description |
|-----------|------|-----|-----|-------------|
| Particle | SendLocationParticles | 0x376A | 0x481 | Flame pillar |
| Sound | PlaySound | 0x1F2 | - | Fire eruption |

> **Implementation Note:** Currently applies STR buff. Design calls for 18-30 damage with knockup.

---

### Magma Armor
| Property | Value |
|----------|-------|
| **Power Words** | "Magmaum Armatura" |
| **Target** | Single, Beneficial |
| **Range** | 10 tiles |
| **Effect** | STR buff (+12 + Conjuration x 0.2, 4 min) |
| **Reagents** | Magma Essence (1), Phoenix Feather (1) |
| **File** | ElementalMagmaArmorSpell.cs |

| Animation | Type | ID | Hue | Description |
|-----------|------|-----|-----|-------------|
| Particle | FixedParticles | 0x376A | 0x481 | Magma armor aura |
| Sound | PlaySound | 0x1F2 | - | Armor activation |

> **Implementation Note:** Currently applies STR buff. Design calls for +15 Physical Resist and fire damage reflection.

---

### Pyroclasm
| Property | Value |
|----------|-------|
| **Power Words** | "Pyroclasmum" |
| **Target** | Single, Harmful |
| **Range** | 10 tiles |
| **Effect** | STR buff (+10 + Conjuration x 0.2, 4 min) |
| **Reagents** | Magma Essence (1), Phoenix Feather (1) |
| **File** | ElementalPyroclasmSpell.cs |

| Animation | Type | ID | Hue | Description |
|-----------|------|-----|-----|-------------|
| Particle | SendLocationParticles | 0x376A | 0x481 | Volcanic explosion |
| Sound | PlaySound | 0x1F2 | - | Eruption |

> **Implementation Note:** Currently applies STR buff. Design calls for 4-tile radius AoE, 20-35 damage.

---

### Meteor Strike
| Property | Value |
|----------|-------|
| **Power Words** | "Meteorum Strikeum" |
| **Target** | Single, Harmful |
| **Range** | 10 tiles |
| **Effect** | STR buff (+10 + Conjuration x 0.2, 4 min) |
| **Reagents** | Magma Essence (1), Phoenix Feather (1) |
| **File** | ElementalMeteorStrikeSpell.cs |

| Animation | Type | ID | Hue | Description |
|-----------|------|-----|-----|-------------|
| Particle | FixedParticles | 0x376A | 0x481 | Meteor impact |
| Sound | PlaySound | 0x1F2 | - | Meteor crash |

> **Implementation Note:** Currently applies STR buff. Design calls for 25-40 damage, leaves lava terrain.

---

## Circle 5 - Master (14 mana)

### Inferno
| Property | Value |
|----------|-------|
| **Power Words** | "Infernum" |
| **Target** | Single, Harmful |
| **Range** | 10 tiles |
| **Effect** | STR buff (+14 + Conjuration x 0.2, 5 min) |
| **Reagents** | Phoenix Feather (1), Dragon Heart (1) |
| **File** | ElementalInfernoSpell.cs |

| Animation | Type | ID | Hue | Description |
|-----------|------|-----|-----|-------------|
| Particle | SendLocationParticles | 0x376A | 0x481 | Inferno flames |
| Sound | PlaySound | 0x1F2 | - | Roaring fire |

> **Implementation Note:** Currently applies STR buff. Design calls for 6-tile radius, 15-28 damage/tick for 8s.

---

### Lava Flow
| Property | Value |
|----------|-------|
| **Power Words** | "Lavaus Flowum" |
| **Target** | Single, Harmful |
| **Range** | 10 tiles |
| **Effect** | STR buff (+14 + Conjuration x 0.2, 5 min) |
| **Reagents** | Phoenix Feather (1), Dragon Heart (1) |
| **File** | ElementalLavaFlowSpell.cs |

| Animation | Type | ID | Hue | Description |
|-----------|------|-----|-----|-------------|
| Particle | FixedParticles | 0x376A | 0x481 | Flowing lava |
| Sound | PlaySound | 0x1F2 | - | Lava bubbling |

> **Implementation Note:** Currently applies STR buff. Design calls for 8-tile long lava stream.

---

### Combustion
| Property | Value |
|----------|-------|
| **Power Words** | "Combustionum" |
| **Target** | Single, Harmful |
| **Range** | 10 tiles |
| **Effect** | STR buff (+14 + Conjuration x 0.2, 5 min) |
| **Reagents** | Phoenix Feather (1), Dragon Heart (1) |
| **File** | ElementalCombustionSpell.cs |

| Animation | Type | ID | Hue | Description |
|-----------|------|-----|-----|-------------|
| Particle | SendLocationParticles | 0x376A | 0x481 | Explosion |
| Sound | PlaySound | 0x1F2 | - | Detonation |

> **Implementation Note:** Currently applies STR buff. Design calls for detonating burning targets for 30-50 damage.

---

### Phoenix Shield
| Property | Value |
|----------|-------|
| **Power Words** | "Phoenixum Shieldus" |
| **Target** | Single, Beneficial |
| **Range** | 10 tiles |
| **Effect** | STR buff (+14 + Conjuration x 0.2, 5 min) |
| **Reagents** | Phoenix Feather (1), Dragon Heart (1) |
| **File** | ElementalPhoenixShieldSpell.cs |

| Animation | Type | ID | Hue | Description |
|-----------|------|-----|-----|-------------|
| Particle | FixedParticles | 0x376A | 0x481 | Phoenix aura |
| Sound | PlaySound | 0x1F2 | - | Wings sound |

> **Implementation Note:** Currently applies STR buff. Design calls for revival at 30% HP upon fatal damage.

---

## Circle 6 - Grandmaster (20 mana)

### Hellfire Nova
| Property | Value |
|----------|-------|
| **Power Words** | "Hellfireus Novaus" |
| **Target** | Single, Harmful |
| **Range** | 10 tiles |
| **Effect** | STR buff (+18 + Conjuration x 0.2, 6 min) |
| **Reagents** | Dragon Heart (1), Primordial Ember (1) |
| **File** | ElementalHellfireNovaSpell.cs |

| Animation | Type | ID | Hue | Description |
|-----------|------|-----|-----|-------------|
| Particle | FixedParticles | 0x376A | 0x481 | Hellfire burst |
| Sound | PlaySound | 0x1F2 | - | Hellfire roar |

> **Implementation Note:** Currently applies STR buff. Design calls for 8-tile radius, 35-60 damage, ignites all.

---

### Molten Titan Form
| Property | Value |
|----------|-------|
| **Power Words** | "Moltenus Titanum Formum" |
| **Target** | Single, Beneficial |
| **Range** | 10 tiles |
| **Effect** | STR buff (+18 + Conjuration x 0.2, 6 min) |
| **Reagents** | Dragon Heart (1), Primordial Ember (1) |
| **File** | ElementalMoltenTitanFormSpell.cs |

| Animation | Type | ID | Hue | Description |
|-----------|------|-----|-----|-------------|
| Particle | FixedParticles | 0x376A | 0x481 | Transformation |
| Sound | PlaySound | 0x1F2 | - | Rumbling |

> **Implementation Note:** Currently applies STR buff. Design calls for +40 STR, +30 Fire Resist, ignite immunity, body transformation.

---

### Volcanic Eruption
| Property | Value |
|----------|-------|
| **Power Words** | "Volcanicus Eruptionum" |
| **Target** | Single, Harmful |
| **Range** | 10 tiles |
| **Effect** | STR buff (+18 + Conjuration x 0.2, 6 min) |
| **Reagents** | Dragon Heart (1), Primordial Ember (1) |
| **File** | ElementalVolcanicEruptionSpell.cs |

| Animation | Type | ID | Hue | Description |
|-----------|------|-----|-----|-------------|
| Particle | SendLocationParticles | 0x376A | 0x481 | Volcanic explosion |
| Sound | PlaySound | 0x1F2 | - | Eruption |

> **Implementation Note:** Currently applies STR buff. Design calls for 10-tile massive AoE, 40-70 damage.

---

### Flame Tempest
| Property | Value |
|----------|-------|
| **Power Words** | "Flameus Tempestum" |
| **Target** | Single, Harmful |
| **Range** | 10 tiles |
| **Effect** | STR buff (+18 + Conjuration x 0.2, 6 min) |
| **Reagents** | Dragon Heart (1), Primordial Ember (1) |
| **File** | ElementalFlameTempestSpell.cs |

| Animation | Type | ID | Hue | Description |
|-----------|------|-----|-----|-------------|
| Particle | FixedParticles | 0x376A | 0x481 | Fire tornado |
| Sound | PlaySound | 0x1F2 | - | Wind roar |

> **Implementation Note:** Currently applies STR buff. Design calls for moving fire tornado, 8-15 damage/tick.

---

## Circle 7 - Archmage (40 mana)

### Cataclysm
| Property | Value |
|----------|-------|
| **Power Words** | "Cataclysmum" |
| **Target** | Single, Harmful |
| **Range** | 10 tiles |
| **Damage** | 50-85 fire |
| **Reagents** | Primordial Ember (1), Elemental Core (1) |
| **File** | ElementalCataclysmSpell.cs |

| Animation | Type | ID | Hue | Description |
|-----------|------|-----|-----|-------------|
| Particle | SendLocationParticles | 0x376A | 0x481 | Cataclysmic explosion |
| Sound | PlaySound | 0x1F2 | - | Earth-shattering |

> **Implementation Note:** Currently single-target. Design calls for screen-wide AoE with lava terrain.

---

### Summon Fire Elemental Lord
| Property | Value |
|----------|-------|
| **Power Words** | "Summonum Fireus Elementalum Lordum" |
| **Target** | Single, Harmful |
| **Range** | 10 tiles |
| **Effect** | STR buff (+22 + Conjuration x 0.2, 10 min) |
| **Reagents** | Primordial Ember (1), Elemental Core (1) |
| **File** | ElementalSummonFireElementalLordSpell.cs |

| Animation | Type | ID | Hue | Description |
|-----------|------|-----|-----|-------------|
| Particle | FixedParticles | 0x376A | 0x481 | Summoning circle |
| Sound | PlaySound | 0x1F2 | - | Summoning sound |

> **Implementation Note:** Currently applies STR buff. Design calls for 1500 HP fire elemental summon.

---

### Pyroclastic Flow
| Property | Value |
|----------|-------|
| **Power Words** | "Pyroclasticum Flowum" |
| **Target** | Single, Harmful |
| **Range** | 10 tiles |
| **Effect** | STR buff (+22 + Conjuration x 0.2, 10 min) |
| **Reagents** | Primordial Ember (1), Elemental Core (1) |
| **File** | ElementalPyroclasticFlowSpell.cs |

| Animation | Type | ID | Hue | Description |
|-----------|------|-----|-----|-------------|
| Particle | SendLocationParticles | 0x376A | 0x481 | Flowing destruction |
| Sound | PlaySound | 0x1F2 | - | Rushing lava |

> **Implementation Note:** Currently applies STR buff. Design calls for 15-tile cone, 60-100 damage, lethal ignite.

---

### Avatar of Flame
| Property | Value |
|----------|-------|
| **Power Words** | "Avatarum Ofum Flameus" |
| **Target** | Single, Beneficial |
| **Range** | 10 tiles |
| **Effect** | STR buff (+22 + Conjuration x 0.2, 10 min) |
| **Reagents** | Primordial Ember (1), Elemental Core (1) |
| **File** | ElementalAvatarofFlameSpell.cs |

| Animation | Type | ID | Hue | Description |
|-----------|------|-----|-----|-------------|
| Particle | FixedParticles | 0x376A | 0x481 | Avatar transformation |
| Sound | PlaySound | 0x1F2 | - | Divine fire |

> **Implementation Note:** Currently applies STR buff. Design calls for +100% fire damage, fire immunity, 50% mana cost reduction.

---

## Circle 8 - Legendary (50 mana)

### Apocalypse
| Property | Value |
|----------|-------|
| **Power Words** | "Apocalypseus" |
| **Target** | Single, Harmful |
| **Range** | 10 tiles |
| **Damage** | 80-140 fire |
| **Reagents** | Primordial Ember (1), Elemental Core (2) |
| **File** | ElementalApocalypseSpell.cs |

| Animation | Type | ID | Hue | Description |
|-----------|------|-----|-----|-------------|
| Particle | FixedParticles | 0x376A | 0x481 | Apocalyptic fire |
| Sound | PlaySound | 0x1F2 | - | End of days |

> **Implementation Note:** Currently single-target. Design calls for screen-wide AoE with burning DoT 20-35/tick.

---

### Magma Core
| Property | Value |
|----------|-------|
| **Power Words** | "Magmaum Coreum" |
| **Target** | Single, Harmful |
| **Range** | 10 tiles |
| **Effect** | STR buff (+26 + Conjuration x 0.2, 12 min) |
| **Reagents** | Primordial Ember (1), Elemental Core (2) |
| **File** | ElementalMagmaCoreSpell.cs |

| Animation | Type | ID | Hue | Description |
|-----------|------|-----|-----|-------------|
| Particle | FixedParticles | 0x376A | 0x481 | Magma core creation |
| Sound | PlaySound | 0x1F2 | - | Core activation |

> **Implementation Note:** Currently applies STR buff. Design calls for permanent 20-tile lava zone, 15-25 damage/tick.

---

### Solar Flare
| Property | Value |
|----------|-------|
| **Power Words** | "Solarus Flarum" |
| **Target** | Single, Harmful |
| **Range** | 10 tiles |
| **Effect** | STR buff (+26 + Conjuration x 0.2, 12 min) |
| **Reagents** | Primordial Ember (1), Elemental Core (2) |
| **File** | ElementalSolarFlareSpell.cs |

| Animation | Type | ID | Hue | Description |
|-----------|------|-----|-----|-------------|
| Particle | FixedParticles | 0x376A | 0x481 | Blinding light |
| Sound | PlaySound | 0x1F2 | - | Solar burst |

> **Implementation Note:** Currently applies STR buff. Design calls for AoE blind, 100-160 damage, 10s disarm.

---

### Primordial Inferno
| Property | Value |
|----------|-------|
| **Power Words** | "Primordialum Infernum" |
| **Target** | Single, Harmful |
| **Range** | 10 tiles |
| **Effect** | STR buff (+26 + Conjuration x 0.2, 12 min) |
| **Reagents** | Primordial Ember (1), Elemental Core (2) |
| **File** | ElementalPrimordialInfernoSpell.cs |

| Animation | Type | ID | Hue | Description |
|-----------|------|-----|-----|-------------|
| Particle | FixedParticles | 0x376A | 0x481 | Primordial flames |
| Sound | PlaySound | 0x1F2 | - | Ancient fire |

> **Implementation Note:** Currently applies STR buff. Design calls for 25-40 damage/tick for 30s, irremovable.

---

## File Locations

| Type | Path |
|------|------|
| Spells | `ServUO/Scripts/Custom/VystiaClasses/Spells/Sorcerer/` |
| Reagents | `ServUO/Scripts/Items/Vystia/Resources/Reagents/ElementalMagicReagents.cs` |
| Spellbook | `ServUO/Scripts/Items/Equipment/Spellbooks/VystiaSpellbooks.cs` |
| Scrolls | `ServUO/Scripts/Items/Vystia/Scrolls/ElementalMagicScrolls.cs` |
| Vendor | `ServUO/Scripts/Mobiles/Vystia/Vendors/MagicSchoolVendors.cs` |

---

## Known Issues

| Issue | Status | Notes |
|-------|--------|-------|
| Most spells use placeholder STR buff | Needs Fix | 29/32 spells need proper effect implementation |
| Flame Bolt, Cataclysm, Apocalypse work | Working | These deal actual fire damage |
| No ignite/DoT system | Needs Fix | IgniteManager not implemented |
| No lava terrain system | Needs Fix | LavaPuddleItem not implemented |
| Reagents are correct | Working | All spells use Vystia reagents |

---

*Last Updated: 2025-12-28*
*Status: Documented - Effects need implementation*
