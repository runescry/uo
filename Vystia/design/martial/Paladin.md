# Paladin - Holy Warrior

| Property | Value |
|----------|-------|
| **Class** | Paladin |
| **Region** | Multi-Regional |
| **Theme** | Holy warrior, tank/support, divine protection |
| **Archetype** | Tank/Support |
| **Ability IDs** | 2096-2111 |
| **Status** | 100% Complete (16/16 abilities) |

---

## Overview

Paladins are holy warriors who blend melee combat with divine magic. They excel at protecting allies, healing, and smiting evil foes with holy power. Their abilities focus on building and spending holy power, providing immunity effects, and dealing bonus damage to undead and demons.

---

## Circle 1 - Basic Holy (4 Mana)

### 1. Crusader Strike

| Property | Value |
|----------|-------|
| **ID** | 2096 |
| **File** | PaladinAbilities.cs |
| **Type** | Damage (Holy Power Builder) |
| **Mana** | 4 |
| **Target** | Single enemy |
| **Damage** | 14-20 holy |

**Effect:**
- Primary holy power builder
- Melee range attack
- Generates holy power for finisher abilities

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Impact | 0x36D4 | - | Holy impact |
| Sound | 0x1E5 | - | Holy strike |
| Secondary | 0x481 | - | Additional effect |

---

### 2. Judgment

| Property | Value |
|----------|-------|
| **ID** | 2097 |
| **File** | PaladinAbilities.cs |
| **Type** | Damage (Ranged) |
| **Mana** | 4 |
| **Target** | Single enemy (ranged) |
| **Damage** | 12-18 holy |

**Effect:**
- Ranged holy damage
- Allows Paladin to engage from distance
- Basic holy power builder

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Impact | 0x36D4 | - | Holy impact |
| Sound | 0x1E5 | - | Judgment sound |
| Secondary | 0x481 | - | Additional effect |

---

### 3. Lay on Hands

| Property | Value |
|----------|-------|
| **ID** | 2098 |
| **File** | PaladinAbilities.cs |
| **Type** | Healing (Emergency) |
| **Mana** | 4 |
| **Target** | Self or ally |
| **Healing** | 50-80 HP |

**Effect:**
- Large instant heal
- Emergency healing ability
- Core survival tool

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Target | 0x376A | - | Healing sparkles |
| Sound | 0x1F2 | - | Healing sound |
| Secondary | 0x481 | - | Additional effect |

---

### 4. Blessing of Might

| Property | Value |
|----------|-------|
| **ID** | 2099 |
| **File** | PaladinAbilities.cs |
| **Type** | Buff (Damage) |
| **Mana** | 4 |
| **Target** | Self or ally |
| **Duration** | 30 seconds |

**Effect:**
- +15% damage output
- Buff value: 10
- Enhances offensive capabilities

**Note:** Implementation uses AllStatsBuff placeholder type.

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Target | (No animation specified) | - | Buff applied |
| Sound | (No sound specified) | - | - |

---

## Circle 2 - Divine Protection (7 Mana)

### 5. Divine Storm

| Property | Value |
|----------|-------|
| **ID** | 2100 |
| **File** | PaladinAbilities.cs |
| **Type** | AoE Damage |
| **Mana** | 7 |
| **Area** | 4 tile radius |
| **Damage** | 10-16 holy |

**Effect:**
- Area-of-effect holy damage
- Hits all enemies within range
- Lower damage due to AoE nature

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Impact | 0x36D4 | - | Holy impact |
| Sound | 0x1E5 | - | Storm sound |
| Secondary | 0x481 | - | Additional effect |

---

### 6. Word of Glory

| Property | Value |
|----------|-------|
| **ID** | 2101 |
| **File** | PaladinAbilities.cs |
| **Type** | Healing (Holy Power Spender) |
| **Mana** | 7 |
| **Target** | Self or ally |
| **Healing** | 20-30 HP (per holy power) |

**Effect:**
- Scales with holy power spent
- Primary healing spender
- More holy power = more healing

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Target | 0x376A | - | Healing sparkles |
| Sound | 0x1F2 | - | Healing sound |
| Secondary | 0x481 | - | Additional effect |

---

### 7. Divine Shield

| Property | Value |
|----------|-------|
| **ID** | 2102 |
| **File** | PaladinAbilities.cs |
| **Type** | Buff (Immunity) |
| **Mana** | 7 |
| **Target** | Self |
| **Duration** | 30 seconds |

**Effect:**
- Immune to all damage for 8 seconds (per description)
- Duration: 30 seconds (buff duration)
- Major defensive cooldown

**Note:** Description says "8s" but buff duration is 30s. Immunity mechanic may need implementation verification.

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Target | (No animation specified) | - | Shield applied |
| Sound | (No sound specified) | - | - |

---

### 8. Hand of Protection

| Property | Value |
|----------|-------|
| **ID** | 2103 |
| **File** | PaladinAbilities.cs |
| **Type** | Buff (Ally Immunity) |
| **Mana** | 7 |
| **Target** | Ally |
| **Duration** | 30 seconds |

**Effect:**
- Ally immune for 10 seconds (per description)
- Duration: 30 seconds (buff duration)
- Protective support ability

**Note:** Description says "10s" but buff duration is 30s. Immunity mechanic may need implementation verification.

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Target | (No animation specified) | - | Protection applied |
| Sound | (No sound specified) | - | - |

---

## Circle 3 - Holy Judgment (10 Mana)

### 9. Templar's Verdict

| Property | Value |
|----------|-------|
| **ID** | 2104 |
| **File** | PaladinAbilities.cs |
| **Type** | Finisher (Holy Power Spender) |
| **Mana** | 10 |
| **Target** | Single enemy |
| **Damage** | 12-18 (per holy power) |

**Effect:**
- Primary damage finisher
- Scales with holy power spent
- More holy power = more damage

**Note:** Uses CreateFinisher helper with no explicit school parameter.

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Impact | (No animation specified) | - | Finisher impact |
| Sound | (No sound specified) | - | - |

---

### 10. Aura of Protection

| Property | Value |
|----------|-------|
| **ID** | 2105 |
| **File** | PaladinAbilities.cs |
| **Type** | Buff (Resistance) |
| **Mana** | 10 |
| **Target** | Self or ally |
| **Duration** | 60 seconds |

**Effect:**
- +15 to all resistances
- Longer duration (60s vs standard 30s)
- Defensive aura

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Target | (No animation specified) | - | Aura applied |
| Sound | (No sound specified) | - | - |

---

### 11. Cleanse

| Property | Value |
|----------|-------|
| **ID** | 2106 |
| **File** | PaladinAbilities.cs |
| **Type** | Utility (Dispel) |
| **Mana** | 10 |
| **Range** | 12 tiles |
| **Target** | Single ally |

**Effect:**
- Removes debuffs from target
- Applies AllStatsBuff (placeholder)
- Duration: 30 seconds

**Note:** Debuff removal mechanic may be placeholder implementation.

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Impact | 0x36D4 | - | Holy impact |
| Sound | 0x1E5 | - | Cleanse sound |
| Secondary | 0x481 | - | Additional effect |

---

### 12. Turn Evil

| Property | Value |
|----------|-------|
| **ID** | 2107 |
| **File** | PaladinAbilities.cs |
| **Type** | Utility (Fear) |
| **Mana** | 10 |
| **Range** | 12 tiles |
| **Target** | Single undead/demon |

**Effect:**
- Fear undead and demon enemies
- Forces enemy to flee
- Duration: 30 seconds (buff duration)

**Note:** Enemy type checking (undead/demon) may be placeholder implementation.

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Impact | 0x36D4 | - | Holy impact |
| Sound | 0x1E5 | - | Turn evil sound |
| Secondary | 0x481 | - | Additional effect |

---

## Circle 4 - Divine Wrath (13 Mana)

### 13. Smite Evil

| Property | Value |
|----------|-------|
| **ID** | 2108 |
| **File** | PaladinAbilities.cs |
| **Type** | Damage (Anti-Evil) |
| **Mana** | 13 |
| **Target** | Single enemy |
| **Damage** | 55-85 holy |

**Effect:**
- Massive holy damage
- Bonus damage vs evil targets (per description)
- High mana cost (13)

**Note:** Bonus damage vs evil mechanic may be placeholder implementation.

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Impact | 0x36D4 | - | Holy impact |
| Sound | 0x1E5 | - | Smite sound |
| Secondary | 0x481 | - | Additional effect |

---

### 14. Divine Weapon

| Property | Value |
|----------|-------|
| **ID** | 2109 |
| **File** | PaladinAbilities.cs |
| **Type** | Buff (Weapon Enhancement) |
| **Mana** | 13 |
| **Target** | Self |
| **Duration** | 60 seconds |

**Effect:**
- Weapon deals holy damage
- Duration: 60 seconds
- Buff value: 10

**Note:** Holy damage conversion mechanic may be placeholder implementation.

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Target | (No animation specified) | - | Weapon glows |
| Sound | (No sound specified) | - | - |

---

### 15. Guardian Angel

| Property | Value |
|----------|-------|
| **ID** | 2110 |
| **File** | PaladinAbilities.cs |
| **Type** | Buff (Death Prevention) |
| **Mana** | 13 |
| **Target** | Ally |
| **Duration** | 60 seconds |

**Effect:**
- Resurrects ally if they die within duration
- Duration: 60 seconds
- Buff value: 10

**Note:** Resurrection mechanic is placeholder implementation requiring OnDeath event handling.

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Target | (No animation specified) | - | Angel aura |
| Sound | (No sound specified) | - | - |

---

### 16. Wrath of the Righteous

| Property | Value |
|----------|-------|
| **ID** | 2111 |
| **File** | PaladinAbilities.cs |
| **Type** | AoE Damage (Ultimate) |
| **Mana** | 13 |
| **Area** | 4 tile radius |
| **Damage** | 50-75 holy |

**Effect:**
- Massive AoE holy judgment
- Highest damage AoE ability
- Ultimate ability

**Animations:**
| Type | ID | Hue | Description |
|------|-----|-----|-------------|
| Impact | 0x36D4 | - | Holy impact |
| Sound | 0x1E5 | - | Wrath sound |
| Secondary | 0x481 | - | Additional effect |

---

## Ability Breakdown by Type

### Damage Abilities (6 total - 37.5%)
1. Crusader Strike (Circle 1) - 14-20 holy
2. Judgment (Circle 1) - 12-18 holy
5. Divine Storm (Circle 2) - 10-16 AoE holy
9. Templar's Verdict (Circle 3) - 12-18 finisher
13. Smite Evil (Circle 4) - 55-85 holy
16. Wrath of the Righteous (Circle 4) - 50-75 AoE holy

### Healing Abilities (2 total - 12.5%)
3. Lay on Hands (Circle 1) - 50-80 HP
6. Word of Glory (Circle 2) - 20-30 HP

### Buff Abilities (6 total - 37.5%)
4. Blessing of Might (Circle 1) - +15% damage
7. Divine Shield (Circle 2) - Immunity 8s
8. Hand of Protection (Circle 2) - Ally immunity 10s
10. Aura of Protection (Circle 3) - +15 all resists
14. Divine Weapon (Circle 4) - Holy weapon
15. Guardian Angel (Circle 4) - Death prevention

### Utility Abilities (2 total - 12.5%)
11. Cleanse (Circle 3) - Remove debuffs
12. Turn Evil (Circle 3) - Fear undead/demons

---

## Core Mechanics

### Holy Power System
**Status:** Not yet implemented in ability framework

Paladins use holy power as a resource:
- **Builders:** Crusader Strike, Judgment generate holy power
- **Spenders:** Templar's Verdict (damage), Word of Glory (healing)

**Note:** Holy power resource type exists in VystiaClasses system but may need integration with ability framework.

### Immunity Effects
**Status:** Placeholder implementation

Multiple abilities grant immunity:
- Divine Shield (self immunity)
- Hand of Protection (ally immunity)

**Note:** Immunity mechanics require integration with damage system.

### Anti-Evil Mechanics
**Status:** Placeholder implementation

Abilities targeting evil creatures:
- Smite Evil (bonus damage)
- Turn Evil (fear effect)

**Note:** Requires enemy type detection system for undead/demons.

---

## Known Issues

| Issue | Ability | Status |
|-------|---------|--------|
| Holy power resource not integrated | Crusader Strike, Judgment, Templar's Verdict, Word of Glory | Placeholder |
| Immunity mechanics placeholder | Divine Shield, Hand of Protection | Needs implementation |
| Anti-evil detection placeholder | Smite Evil, Turn Evil | Needs enemy type checking |
| Death prevention placeholder | Guardian Angel | Needs OnDeath event hook |
| Holy damage conversion placeholder | Divine Weapon | Needs damage type override |
| Debuff removal placeholder | Cleanse | Needs buff/debuff system integration |
| Generic buff types used | Most buff abilities | Should use specific buff types |
| Missing animations | Most abilities | Uses placeholder effects |
| Duration inconsistencies | Divine Shield, Hand of Protection | Description vs buff duration mismatch |

---

## File Locations

| Type | Path |
|------|------|
| Ability Definitions | `ServUO/Scripts/Custom/VystiaClasses/Abilities/Generated/Martial/PaladinAbilities.cs` |
| Class Definition | `ServUO/Scripts/Custom/VystiaClasses/Classes/Paladin.cs` |
| Python Generator | `ServUO/Scripts/Custom/VystiaClasses/Abilities/Scripts/generate_martial_abilities.py` |

---

## Design Philosophy

**Tank/Support Hybrid:**
- 37.5% damage (6 abilities) - Below pure DPS, appropriate for tank
- 37.5% buffs (6 abilities) - High support capability
- 12.5% healing (2 abilities) - Emergency healing, not primary healer
- 12.5% utility (2 abilities) - Cleanse and crowd control

**Holy Power Economy:**
- 2 builders (Crusader Strike, Judgment) for resource generation
- 2 spenders (Templar's Verdict, Word of Glory) for damage/healing
- Creates engaging rotation: Build → Spend → Build → Spend

**Defensive Focus:**
- Multiple immunity abilities (Divine Shield, Hand of Protection)
- Resistance buffs (Aura of Protection)
- Death prevention (Guardian Angel)
- Supports tank role

**Righteous Wrath:**
- High single-target burst (Smite Evil: 55-85)
- Strong AoE (Wrath of the Righteous: 50-75)
- Anti-evil specialization (Smite Evil, Turn Evil)

---

*Last Updated: 2025-12-28*
*Documentation synced with ServUO implementation*
