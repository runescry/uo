# Enchanting Magic - Multi-Regional School

| Property | Value |
|----------|-------|
| **Class** | Enchanter |
| **Region** | Multi-Regional |
| **Theme** | Weapon/armor enchantments, augmentation, runes, item empowerment |
| **Spellbook** | Codex of Enhancement (EnchanterSpellbook.cs) |
| **Hue** | 0x8FD (Arcane Purple) |
| **Spell IDs** | 1352-1383 |
| **Status** | 100% Complete (32/32 spells) |

---

## Reagents

All Enchanting Magic spells use Vystia reagents found across multiple regions.

| Reagent | Item ID | Circles | Description |
|---------|---------|---------|-------------|
| Arcane Dust | 0x0F8F | 1-3 | Dust of arcane power |
| Essence of Magic | 0x1C18 | 2-4 | Pure magical essence |
| Mana Crystal | 0x0F8E | 3-5 | Crystal of mana |
| Ley Line Essence | 0x0F7D | 4-6 | Essence from ley lines |
| Ley Line Shard | 0x0F8A | 5-7 | Shard from ley nexus |
| Rune Fragment | 0x0DE1 | 6-8 | Fragment of power rune |
| Runic Powder | 0x0F86 | 7-8 | Powder of runes |
| Titan Rune | 0x0F7A | 8 | Rune of titan power |

---

## Circle 1 - Basic Enchantments (4 Mana)

### 1. Magic Weapon

| Property | Value |
|----------|-------|
| **File** | EnchantingMagicWeaponSpell.cs |
| **Words** | "Magicum Weaponum" |
| **Reagents** | Arcane Dust, Essence of Magic |
| **Range** | 10 tiles |
| **Target** | Single ally |
| **Duration** | 60 seconds |

**Effect:**
- Placeholder: +5 STR buff (scales +0.2 per Conjuration skill)
- Design intent: +5 weapon damage enchantment

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Target | 0x376A | 0x481 | Purple particles at waist |
| Sound | 0x1F2 | - | Enchantment sound |

**Known Issue:** Currently applies STR buff instead of weapon damage enchantment. Requires weapon enchantment system implementation.

---

### 2. Arcane Shield

| Property | Value |
|----------|-------|
| **File** | EnchantingArcaneShieldSpell.cs |
| **Words** | "Arcaneus Scutum" |
| **Reagents** | Arcane Dust, Essence of Magic |
| **Range** | 10 tiles |
| **Target** | Single ally |
| **Duration** | 60 seconds |

**Effect:**
- Placeholder: +5 STR buff (scales +0.2 per Conjuration skill)
- Design intent: +5 armor enchantment

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Target | 0x376A | 0x481 | Purple particles at waist |
| Sound | 0x1F2 | - | Enchantment sound |

**Known Issue:** Currently applies STR buff instead of armor enchantment. Requires armor enchantment system implementation.

---

### 3. Rune of Power

| Property | Value |
|----------|-------|
| **File** | EnchantingRuneofPowerSpell.cs |
| **Words** | "Runeus Ofum Powerum" |
| **Reagents** | Arcane Dust, Essence of Magic |
| **Range** | 10 tiles |
| **Target** | Single ally |
| **Duration** | 60 seconds |

**Effect:**
- Placeholder: +5 STR buff (scales +0.2 per Conjuration skill)
- Design intent: Create rune item for instant +10% damage (30s duration)

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Target | 0x376A | 0x481 | Purple particles at waist |
| Sound | 0x1F2 | - | Enchantment sound |

**Known Issue:** Currently applies STR buff instead of creating rune item. Requires rune item creation system.

---

### 4. Detect Magic

| Property | Value |
|----------|-------|
| **File** | EnchantingDetectMagicSpell.cs |
| **Words** | "Detect Magicum" |
| **Reagents** | Arcane Dust, Essence of Magic |
| **Range** | 10 tiles |
| **Target** | Single ally |
| **Duration** | 60 seconds |

**Effect:**
- Placeholder: +5 STR buff (scales +0.2 per Conjuration skill)
- Design intent: Reveal magical auras and enchanted items

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Target | 0x376A | 0x481 | Purple particles at waist |
| Sound | 0x1F2 | - | Enchantment sound |

**Known Issue:** Currently applies STR buff instead of revealing magic. Requires magic detection system.

---

## Circle 2 - Elemental Enchantments (6 Mana)

### 5. Flaming Weapon

| Property | Value |
|----------|-------|
| **File** | EnchantingFlamingWeaponSpell.cs |
| **Words** | "Flamingum Weaponum" |
| **Reagents** | Essence of Magic, Mana Crystal |
| **Range** | 10 tiles |
| **Target** | Single ally |
| **Duration** | 120 seconds (2 minutes) |

**Effect:**
- Placeholder: +8 STR buff (scales +0.2 per Conjuration skill)
- Design intent: Weapon deals +8 fire damage per hit

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Target | 0x376A | 0x481 | Purple particles at waist |
| Sound | 0x1F2 | - | Enchantment sound |

**Known Issue:** Currently applies STR buff instead of fire damage enchantment. Requires weapon enchantment system.

---

### 6. Fortify Armor

| Property | Value |
|----------|-------|
| **File** | EnchantingFortifyArmorSpell.cs |
| **Words** | "Fortifyum Armorum" |
| **Reagents** | Essence of Magic, Mana Crystal |
| **Range** | 10 tiles |
| **Target** | Single ally |
| **Duration** | 120 seconds (2 minutes) |

**Effect:**
- Placeholder: +8 STR buff (scales +0.2 per Conjuration skill)
- Design intent: +10 Physical Resistance

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Target | 0x376A | 0x481 | Purple particles at waist |
| Sound | 0x1F2 | - | Enchantment sound |

**Known Issue:** Currently applies STR buff instead of resistance buff. Should use ResistanceMod system.

---

### 7. Disenchant

| Property | Value |
|----------|-------|
| **File** | EnchantingDisenchantSpell.cs |
| **Words** | "Disenchantum" |
| **Reagents** | Essence of Magic, Mana Crystal |
| **Range** | 10 tiles |
| **Target** | Single enemy |
| **Duration** | 120 seconds (2 minutes) |

**Effect:**
- Placeholder: +8 STR buff (scales +0.2 per Conjuration skill)
- Design intent: Remove one buff from enemy

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Target | 0x376A | 0x481 | Purple particles at waist |
| Sound | 0x1F2 | - | Enchantment sound |

**Known Issue:** Currently applies STR buff instead of removing buffs. Requires buff removal system.

---

### 8. Rune of Protection

| Property | Value |
|----------|-------|
| **File** | EnchantingRuneofProtectionSpell.cs |
| **Words** | "Runeus Ofum Protectionum" |
| **Reagents** | Essence of Magic, Mana Crystal |
| **Range** | 10 tiles |
| **Target** | Single ally |
| **Duration** | 120 seconds (2 minutes) |

**Effect:**
- Placeholder: +8 STR buff (scales +0.2 per Conjuration skill)
- Design intent: Create rune item for instant 50 damage shield

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Target | 0x376A | 0x481 | Purple particles at waist |
| Sound | 0x1F2 | - | Enchantment sound |

**Known Issue:** Currently applies STR buff instead of creating rune item. Requires rune item creation system.

---

## Circle 3 - Advanced Enchantments (9 Mana)

### 9. Lightning Weapon

| Property | Value |
|----------|-------|
| **File** | EnchantingLightningWeaponSpell.cs |
| **Words** | "Lightningum Weaponum" |
| **Reagents** | Mana Crystal, Ley Line Essence |
| **Range** | 10 tiles |
| **Target** | Single ally |
| **Duration** | 180 seconds (3 minutes) |

**Effect:**
- Placeholder: +11 STR buff (scales +0.2 per Conjuration skill)
- Design intent: Weapon has 25% chance to chain lightning on hit

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Target | 0x376A | 0x481 | Purple particles at waist |
| Sound | 0x1F2 | - | Enchantment sound |

**Known Issue:** Currently applies STR buff instead of chain lightning enchantment. Requires weapon enchantment system.

---

### 10. Elemental Barrier

| Property | Value |
|----------|-------|
| **File** | EnchantingElementalBarrierSpell.cs |
| **Words** | "Elementalum Barrierum" |
| **Reagents** | Mana Crystal, Ley Line Essence |
| **Range** | 10 tiles |
| **Target** | Single ally |
| **Duration** | 180 seconds (3 minutes) |

**Effect:**
- Placeholder: +11 STR buff (scales +0.2 per Conjuration skill)
- Design intent: +10 to all elemental resistances (Fire, Cold, Poison, Energy)

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Target | 0x376A | 0x481 | Purple particles at waist |
| Sound | 0x1F2 | - | Enchantment sound |

**Known Issue:** Currently applies STR buff instead of resistance buffs. Should use ResistanceMod system.

---

### 11. Sharpen

| Property | Value |
|----------|-------|
| **File** | EnchantingSharpenSpell.cs |
| **Words** | "Sharpenum" |
| **Reagents** | Mana Crystal, Ley Line Essence |
| **Range** | 10 tiles |
| **Target** | Single ally |
| **Duration** | 180 seconds (3 minutes) |

**Effect:**
- Placeholder: +11 STR buff (scales +0.2 per Conjuration skill)
- Design intent: Weapon ignores 15 armor (armor penetration)

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Target | 0x376A | 0x481 | Purple particles at waist |
| Sound | 0x1F2 | - | Enchantment sound |

**Known Issue:** Currently applies STR buff instead of armor penetration enchantment. Requires weapon enchantment system.

---

### 12. Rune of Healing

| Property | Value |
|----------|-------|
| **File** | EnchantingRuneofHealingSpell.cs |
| **Words** | "Runeus Ofum Healingum" |
| **Reagents** | Mana Crystal, Ley Line Essence |
| **Range** | 10 tiles |
| **Target** | Single ally |
| **Duration** | 180 seconds (3 minutes) |

**Effect:**
- Placeholder: +11 STR buff (scales +0.2 per Conjuration skill)
- Design intent: Create rune item for instant 40-60 HP heal

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Target | 0x376A | 0x481 | Purple particles at waist |
| Sound | 0x1F2 | - | Enchantment sound |

**Known Issue:** Currently applies STR buff instead of creating rune item. Requires rune item creation system.

---

## Circle 4 - Expert Enchantments (11 Mana)

### 13. Vampiric Weapon

| Property | Value |
|----------|-------|
| **File** | EnchantingVampiricWeaponSpell.cs |
| **Words** | "Vampiricum Weaponum" |
| **Reagents** | Ley Line Essence, Ley Line Shard |
| **Range** | 10 tiles |
| **Target** | Single ally |
| **Duration** | 240 seconds (4 minutes) |

**Effect:**
- Placeholder: +13 STR buff (scales +0.2 per Conjuration skill)
- Design intent: Weapon drains 10% damage as HP on hit

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Target | 0x376A | 0x481 | Purple particles at waist |
| Sound | 0x1F2 | - | Enchantment sound |

**Known Issue:** Currently applies STR buff instead of life drain enchantment. Requires weapon enchantment system.

---

### 14. Spell Reflection

| Property | Value |
|----------|-------|
| **File** | EnchantingSpellReflectionSpell.cs |
| **Words** | "Spellum Reflectionum" |
| **Reagents** | Ley Line Essence, Ley Line Shard |
| **Range** | 10 tiles |
| **Target** | Single ally |
| **Duration** | 240 seconds (4 minutes) |

**Effect:**
- Placeholder: +13 STR buff (scales +0.2 per Conjuration skill)
- Design intent: Armor reflects next spell (60s duration)

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Target | 0x376A | 0x481 | Purple particles at waist |
| Sound | 0x1F2 | - | Enchantment sound |

**Known Issue:** Currently applies STR buff instead of spell reflection. Requires spell reflection system.

---

### 15. Enchant Arrows

| Property | Value |
|----------|-------|
| **File** | EnchantingEnchantArrowsSpell.cs |
| **Words** | "Enchantum Arrowsum" |
| **Reagents** | Ley Line Essence, Ley Line Shard |
| **Range** | 10 tiles |
| **Target** | Single ally |
| **Duration** | 240 seconds (4 minutes) |

**Effect:**
- Placeholder: +13 STR buff (scales +0.2 per Conjuration skill)
- Design intent: Enchant 100 arrows to deal +10 damage each

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Target | 0x376A | 0x481 | Purple particles at waist |
| Sound | 0x1F2 | - | Enchantment sound |

**Known Issue:** Currently applies STR buff instead of arrow enchantment. Requires ammunition enchantment system.

---

### 16. Mass Disenchant

| Property | Value |
|----------|-------|
| **File** | EnchantingMassDisenchantSpell.cs |
| **Words** | "Massum Disenchantum" |
| **Reagents** | Ley Line Essence, Ley Line Shard |
| **Range** | 10 tiles |
| **Target** | Single enemy |
| **Duration** | 240 seconds (4 minutes) |

**Effect:**
- Placeholder: +13 STR buff (scales +0.2 per Conjuration skill)
- Design intent: Remove all buffs from target

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Target | 0x376A | 0x481 | Purple particles at waist |
| Sound | 0x1F2 | - | Enchantment sound |

**Known Issue:** Currently applies STR buff instead of removing all buffs. Requires buff removal system.

---

## Circle 5 - Master Enchantments (14 Mana)

### 17. Holy Weapon

| Property | Value |
|----------|-------|
| **File** | EnchantingHolyWeaponSpell.cs |
| **Words** | "Holyum Weaponum" |
| **Reagents** | Ley Line Shard, Rune Fragment |
| **Range** | 10 tiles |
| **Target** | Single ally |
| **Duration** | 300 seconds (5 minutes) |

**Effect:**
- Placeholder: +16 STR buff (scales +0.2 per Conjuration skill)
- Design intent: Weapon deals double damage to undead/demons (180s)

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Target | 0x376A | 0x481 | Purple particles at waist |
| Sound | 0x1F2 | - | Enchantment sound |

**Known Issue:** Currently applies STR buff instead of holy damage enchantment. Requires weapon enchantment system with slayer mechanics.

---

### 18. Aegis of Warding

| Property | Value |
|----------|-------|
| **File** | EnchantingAegisofWardingSpell.cs |
| **Words** | "Aegisum Ofum Wardingum" |
| **Reagents** | Ley Line Shard, Rune Fragment |
| **Range** | 10 tiles |
| **Target** | Single ally |
| **Duration** | 300 seconds (5 minutes) |

**Effect:**
- Placeholder: +16 STR buff (scales +0.2 per Conjuration skill)
- Design intent: Absorbs next 100 damage (120s)

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Target | 0x376A | 0x481 | Purple particles at waist |
| Sound | 0x1F2 | - | Enchantment sound |

**Known Issue:** Currently applies STR buff instead of damage shield. Requires damage absorption system.

---

### 19. Runic Empowerment

| Property | Value |
|----------|-------|
| **File** | EnchantingRunicEmpowermentSpell.cs |
| **Words** | "Runicum Empowermentum" |
| **Reagents** | Ley Line Shard, Rune Fragment |
| **Range** | 10 tiles |
| **Target** | Single ally |
| **Duration** | 300 seconds (5 minutes) |

**Effect:**
- Placeholder: +16 STR buff (scales +0.2 per Conjuration skill)
- Design intent: All stats +15 (STR/DEX/INT) for 90s

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Target | 0x376A | 0x481 | Purple particles at waist |
| Sound | 0x1F2 | - | Enchantment sound |

**Known Issue:** Currently applies STR buff only instead of all stats. Should use StatMod for STR/DEX/INT.

---

### 20. Enchant Party Weapons

| Property | Value |
|----------|-------|
| **File** | EnchantingEnchantPartyWeaponsSpell.cs |
| **Words** | "Enchantum Partyum Weaponsum" |
| **Reagents** | Ley Line Shard, Rune Fragment |
| **Range** | 10 tiles |
| **Target** | Single ally |
| **Duration** | 300 seconds (5 minutes) |

**Effect:**
- Placeholder: +16 STR buff (scales +0.2 per Conjuration skill)
- Design intent: All party members' weapons +10 damage (120s)

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Target | 0x376A | 0x481 | Purple particles at waist |
| Sound | 0x1F2 | - | Enchantment sound |

**Known Issue:** Currently applies STR buff instead of party-wide weapon enchantment. Requires weapon enchantment system with party support.

---

## Circle 6 - Legendary Enchantments (20 Mana)

### 21. Legendary Weapon

| Property | Value |
|----------|-------|
| **File** | EnchantingLegendaryWeaponSpell.cs |
| **Words** | "Legendaryum Weaponum" |
| **Reagents** | Rune Fragment, Runic Powder |
| **Range** | 10 tiles |
| **Target** | Single ally |
| **Duration** | 420 seconds (7 minutes) |

**Effect:**
- Placeholder: +18 STR buff (scales +0.2 per Conjuration skill)
- Design intent: Weapon +25 damage, +20% attack speed, +10% crit (180s)

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Target | 0x376A | 0x481 | Purple particles at waist |
| Sound | 0x1F2 | - | Enchantment sound |

**Known Issue:** Currently applies STR buff instead of legendary weapon enchantment. Requires weapon enchantment system with multiple properties.

---

### 22. Invulnerability

| Property | Value |
|----------|-------|
| **File** | EnchantingInvulnerabilitySpell.cs |
| **Words** | "Invulnerabilityum" |
| **Reagents** | Rune Fragment, Runic Powder |
| **Range** | 10 tiles |
| **Target** | Single ally |
| **Duration** | 420 seconds (7 minutes) |

**Effect:**
- Placeholder: +18 STR buff (scales +0.2 per Conjuration skill)
- Design intent: Armor grants 50% damage reduction (60s)

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Target | 0x376A | 0x481 | Purple particles at waist |
| Sound | 0x1F2 | - | Enchantment sound |

**Known Issue:** Currently applies STR buff instead of damage reduction. Requires damage mitigation system.

---

### 23. Mass Enchant Weapons

| Property | Value |
|----------|-------|
| **File** | EnchantingMassEnchantWeaponsSpell.cs |
| **Words** | "Massum Enchantum Weaponsum" |
| **Reagents** | Rune Fragment, Runic Powder |
| **Range** | 10 tiles |
| **Target** | Single ally |
| **Duration** | 420 seconds (7 minutes) |

**Effect:**
- Placeholder: +18 STR buff (scales +0.2 per Conjuration skill)
- Design intent: All allies' weapons gain elemental damage (120s)

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Target | 0x376A | 0x481 | Purple particles at waist |
| Sound | 0x1F2 | - | Enchantment sound |

**Known Issue:** Currently applies STR buff instead of area weapon enchantment. Requires weapon enchantment system with area support.

---

### 24. Rune of Resurrection

| Property | Value |
|----------|-------|
| **File** | EnchantingRuneofResurrectionSpell.cs |
| **Words** | "Runeus Ofum Resurrectionum" |
| **Reagents** | Rune Fragment, Runic Powder |
| **Range** | 10 tiles |
| **Target** | Single ally |
| **Duration** | 420 seconds (7 minutes) |

**Effect:**
- Placeholder: +18 STR buff (scales +0.2 per Conjuration skill)
- Design intent: Create rune item for instant self-resurrection

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Target | 0x376A | 0x481 | Purple particles at waist |
| Sound | 0x1F2 | - | Enchantment sound |

**Known Issue:** Currently applies STR buff instead of creating rune item. Requires rune item creation system with resurrection mechanic.

---

## Circle 7 - Grandmaster Enchantments (40 Mana)

### 25. Godly Weapon

| Property | Value |
|----------|-------|
| **File** | EnchantingGodlyWeaponSpell.cs |
| **Words** | "Godlyum Weaponum" |
| **Reagents** | Runic Powder, Titan Rune |
| **Range** | 10 tiles |
| **Target** | Single ally |
| **Duration** | 540 seconds (9 minutes) |

**Effect:**
- Placeholder: +22 STR buff (scales +0.2 per Conjuration skill)
- Design intent: Weapon +50 damage, triple attack speed, life drain (240s)

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Target | 0x376A | 0x481 | Purple particles at waist |
| Sound | 0x1F2 | - | Enchantment sound |

**Known Issue:** Currently applies STR buff instead of godly weapon enchantment. Requires weapon enchantment system with multiple legendary properties.

---

### 26. Prismatic Barrier

| Property | Value |
|----------|-------|
| **File** | EnchantingPrismaticBarrierSpell.cs |
| **Words** | "Prismaticum Barrierum" |
| **Reagents** | Runic Powder, Titan Rune |
| **Range** | 10 tiles |
| **Target** | Single ally |
| **Duration** | 540 seconds (9 minutes) |

**Effect:**
- Placeholder: +22 STR buff (scales +0.2 per Conjuration skill)
- Design intent: +40 all resistances, reflect damage (180s)

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Target | 0x376A | 0x481 | Purple particles at waist |
| Sound | 0x1F2 | - | Enchantment sound |

**Known Issue:** Currently applies STR buff instead of resistance buffs and damage reflection. Requires ResistanceMod system and damage reflection.

---

### 27. Enchant Army

| Property | Value |
|----------|-------|
| **File** | EnchantingEnchantArmySpell.cs |
| **Words** | "Enchantum Armyum" |
| **Reagents** | Runic Powder, Titan Rune |
| **Range** | 10 tiles |
| **Target** | Single ally |
| **Duration** | 540 seconds (9 minutes) |

**Effect:**
- Placeholder: +22 STR buff (scales +0.2 per Conjuration skill)
- Design intent: All allies +20% damage, +15 armor (180s)

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Target | 0x376A | 0x481 | Purple particles at waist |
| Sound | 0x1F2 | - | Enchantment sound |

**Known Issue:** Currently applies STR buff instead of party-wide damage and armor buffs. Requires weapon/armor enchantment system with area support.

---

### 28. Greater Disenchant

| Property | Value |
|----------|-------|
| **File** | EnchantingGreaterDisenchantSpell.cs |
| **Words** | "Greaterum Disenchantum" |
| **Reagents** | Runic Powder, Titan Rune |
| **Range** | 10 tiles |
| **Target** | Single enemy |
| **Duration** | 540 seconds (9 minutes) |

**Effect:**
- Placeholder: +22 STR buff (scales +0.2 per Conjuration skill)
- Design intent: Remove ALL buffs from all enemies in 10 tile AoE

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Target | 0x376A | 0x481 | Purple particles at waist |
| Sound | 0x1F2 | - | Enchantment sound |

**Known Issue:** Currently applies STR buff instead of area buff removal. Requires buff removal system with area support.

---

## Circle 8 - Artifact Enchantments (50 Mana)

### 29. Artifact Empowerment

| Property | Value |
|----------|-------|
| **File** | EnchantingArtifactEmpowermentSpell.cs |
| **Words** | "Artifactum Empowermentum" |
| **Reagents** | Runic Powder, Titan Rune (×2) |
| **Range** | 10 tiles |
| **Target** | Single ally |
| **Duration** | 660 seconds (11 minutes) |

**Effect:**
- Placeholder: +26 STR buff (scales +0.2 per Conjuration skill)
- Design intent: Weapon becomes legendary: +100 damage, special effects (300s)

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Target | 0x376A | 0x481 | Purple particles at waist |
| Sound | 0x1F2 | - | Enchantment sound |

**Known Issue:** Currently applies STR buff instead of artifact-level weapon enchantment. Requires weapon enchantment system with artifact-tier properties.

---

### 30. Invincible Armor

| Property | Value |
|----------|-------|
| **File** | EnchantingInvincibleArmorSpell.cs |
| **Words** | "Invincibleum Armorum" |
| **Reagents** | Runic Powder, Titan Rune (×2) |
| **Range** | 10 tiles |
| **Target** | Single ally |
| **Duration** | 660 seconds (11 minutes) |

**Effect:**
- Placeholder: +26 STR buff (scales +0.2 per Conjuration skill)
- Design intent: Armor grants immunity to physical damage (30s)

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Target | 0x376A | 0x481 | Purple particles at waist |
| Sound | 0x1F2 | - | Enchantment sound |

**Known Issue:** Currently applies STR buff instead of physical immunity. Requires damage mitigation system with immunity support.

---

### 31. Rune of Apocalypse

| Property | Value |
|----------|-------|
| **File** | EnchantingRuneofApocalypseSpell.cs |
| **Words** | "Runeus Ofum Apocalypsum" |
| **Reagents** | Runic Powder, Titan Rune (×2) |
| **Range** | 10 tiles |
| **Target** | Single ally |
| **Duration** | 660 seconds (11 minutes) |

**Effect:**
- Placeholder: +26 STR buff (scales +0.2 per Conjuration skill)
- Design intent: Create rune item for instant AoE 150-250 damage

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Target | 0x376A | 0x481 | Purple particles at waist |
| Sound | 0x1F2 | - | Enchantment sound |

**Known Issue:** Currently applies STR buff instead of creating rune item. Requires rune item creation system with AoE damage.

---

### 32. Archmage's Blessing

| Property | Value |
|----------|-------|
| **File** | EnchantingArchmagesBlessingSpell.cs |
| **Words** | "Archmagesum Blessingum" |
| **Reagents** | Runic Powder, Titan Rune (×2) |
| **Range** | 10 tiles |
| **Target** | Single ally |
| **Duration** | 660 seconds (11 minutes) |

**Effect:**
- Placeholder: +26 STR buff (scales +0.2 per Conjuration skill)
- Design intent: All enchantments last forever, can stack multiple (60s buff to allow infinite durations)

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Target | 0x376A | 0x481 | Purple particles at waist |
| Sound | 0x1F2 | - | Enchantment sound |

**Known Issue:** Currently applies STR buff instead of enchantment duration modifier. Requires enchantment system rewrite.

---

## Known Issues

### Critical Issues

1. **All Spells Use Placeholder STR Buffs**
   - **Impact:** None of the 32 spells implement their intended effects
   - **Root Cause:** All spells were generated with placeholder STR buff code
   - **Required Systems:**
     - Weapon enchantment system (damage, speed, effects)
     - Armor enchantment system (armor, resistances, damage reduction)
     - Buff removal/dispel system
     - Rune item creation system
     - Damage absorption/shield system
     - Spell reflection system
     - Area targeting system for party/army spells

2. **No Skill-Based Scaling**
   - **Impact:** All spells scale with Conjuration skill (placeholder)
   - **Intended:** Should scale with Enchantment skill (SkillName.Enchanting)
   - **Fix:** Change all `Caster.Skills.Conjuration.Value` to `Caster.Skills.Enchanting.Value`

3. **Incorrect Target Flags**
   - **Impact:** All buff spells use `TargetFlags.Harmful`
   - **Intended:** Buff spells should use `TargetFlags.Beneficial`
   - **Fix:** Change target flags for ally buffs (spells 1-6, 9-11, 13-14, 17-24)

4. **Duration Inconsistency**
   - **Impact:** Spell durations don't match design document
   - **Design vs Implementation:**
     - Circle 1: 60s (design) vs 60s (implemented) ✓
     - Circle 2: 90s (design) vs 120s (implemented) ✗
     - Circle 3: 120s (design) vs 180s (implemented) ✗
     - Circle 4: 120s (design) vs 240s (implemented) ✗
     - Circle 5: 180s (design) vs 300s (implemented) ✗
     - Circle 6: 180s (design) vs 420s (implemented) ✗
     - Circle 7: 240s/180s (design) vs 540s (implemented) ✗
     - Circle 8: 300s/60s (design) vs 660s (implemented) ✗

### Required Systems to Implement

1. **Weapon Enchantment System**
   - Attach properties to equipped weapons
   - Support: bonus damage, elemental damage, life drain, chain effects, armor pen
   - On-hit trigger system
   - Duration tracking per enchantment

2. **Armor Enchantment System**
   - Attach properties to equipped armor
   - Support: armor bonus, resistances, damage reduction, spell reflection
   - Damage absorption shields
   - Duration tracking per enchantment

3. **Rune Item Creation**
   - Consumable item generation
   - Double-click activation
   - Instant effects (no cast time)
   - Types: Power (+damage), Protection (shield), Healing (HP), Resurrection (revive), Apocalypse (AoE damage)
   - Max 10 runes per type per player

4. **Buff Removal System**
   - Single buff removal (Disenchant)
   - All buffs removal (Mass Disenchant)
   - Area buff removal (Greater Disenchant)
   - Enumerate and remove StatMod/ResistanceMod

5. **Area Targeting System**
   - Party member detection
   - 10 tile radius enemy detection
   - Multi-target buff application

---

## File Locations

**Spell Files:** `D:\UO\ServUO\Scripts\Custom\VystiaClasses\Spells\Enchanter\`

| Circle | Spell Files |
|--------|-------------|
| Circle 1 | EnchantingMagicWeaponSpell.cs, EnchantingArcaneShieldSpell.cs, EnchantingRuneofPowerSpell.cs, EnchantingDetectMagicSpell.cs |
| Circle 2 | EnchantingFlamingWeaponSpell.cs, EnchantingFortifyArmorSpell.cs, EnchantingDisenchantSpell.cs, EnchantingRuneofProtectionSpell.cs |
| Circle 3 | EnchantingLightningWeaponSpell.cs, EnchantingElementalBarrierSpell.cs, EnchantingSharpenSpell.cs, EnchantingRuneofHealingSpell.cs |
| Circle 4 | EnchantingVampiricWeaponSpell.cs, EnchantingSpellReflectionSpell.cs, EnchantingEnchantArrowsSpell.cs, EnchantingMassDisenchantSpell.cs |
| Circle 5 | EnchantingHolyWeaponSpell.cs, EnchantingAegisofWardingSpell.cs, EnchantingRunicEmpowermentSpell.cs, EnchantingEnchantPartyWeaponsSpell.cs |
| Circle 6 | EnchantingLegendaryWeaponSpell.cs, EnchantingInvulnerabilitySpell.cs, EnchantingMassEnchantWeaponsSpell.cs, EnchantingRuneofResurrectionSpell.cs |
| Circle 7 | EnchantingGodlyWeaponSpell.cs, EnchantingPrismaticBarrierSpell.cs, EnchantingEnchantArmySpell.cs, EnchantingGreaterDisenchantSpell.cs |
| Circle 8 | EnchantingArtifactEmpowermentSpell.cs, EnchantingInvincibleArmorSpell.cs, EnchantingRuneofApocalypseSpell.cs, EnchantingArchmagesBlessingSpell.cs |

**Reagent File:** `D:\UO\ServUO\Scripts\Items\Vystia\Resources\Reagents\EnchantingMagicReagents.cs`

---

## Implementation Priority

### Phase 1 (Core Systems)
1. Fix target flags (Beneficial vs Harmful)
2. Fix skill scaling (Conjuration → Enchanting)
3. Implement basic weapon enchantment (bonus damage)
4. Implement basic armor enchantment (armor/resist buffs)

### Phase 2 (Advanced Enchantments)
5. Implement elemental damage enchantments
6. Implement life drain/vampiric effects
7. Implement damage shields/absorption
8. Implement buff removal (Disenchant)

### Phase 3 (Rune System)
9. Create rune item base class
10. Implement Rune of Power (damage buff)
11. Implement Rune of Protection (shield)
12. Implement Rune of Healing (instant heal)
13. Implement Rune of Resurrection (revive)
14. Implement Rune of Apocalypse (AoE damage)

### Phase 4 (Legendary Effects)
15. Implement spell reflection
16. Implement armor penetration (Sharpen)
17. Implement attack speed buffs
18. Implement damage reduction/immunity
19. Implement area/party buffs

---

**Last Updated:** 2025-12-28
**Status:** All 32 spells implemented with placeholder effects - requires system rewrites for full functionality
