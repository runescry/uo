# VYSTIA: COMPLETE DESIGN DOCUMENT
## Unified Master Reference

**Version:** 5.0 (Reconciled)  
**Status:** Implementation Ready  
**Date:** January 2026

---

# TABLE OF CONTENTS

1. [Executive Summary](#executive-summary)
2. [World Foundation](#part-i-world-foundation)
3. [The 25 Character Classes](#part-ii-the-25-character-classes)
4. [Skills and Professions](#part-iii-skills-and-professions)
5. [Magic System](#part-iv-magic-system)
6. [Religion System](#part-v-religion-system)
7. [Economy System](#part-vi-economy-system)
8. [Crafting System](#part-vii-crafting-system)
9. [Creatures and Bosses](#part-viii-creatures-and-bosses)
10. [Implementation Reference](#part-ix-implementation-reference)
11. [Appendices](#appendices)

---

# EXECUTIVE SUMMARY

## System Completion Status

| System | Components | Status |
|--------|------------|--------|
| Character Classes | 25 classes | ✅ Complete |
| Magic Schools | 12 schools, 384 spells | ✅ Complete |
| Skills | 52 standard + 26 custom = 78 total | ✅ Complete |
| Secondary Resources | 25 resource types | ✅ Complete |
| Creatures | 138 creatures | ✅ Complete |
| Regional Bosses | 10 bosses | ✅ Complete |
| Ancient Beings | 12 beings | ✅ Complete |
| Factions | 7 factions | ✅ Complete |
| Religion System | 6 religions | ✅ Complete |
| Economy System | Gold flow balanced | ✅ Complete |
| Crafting Disciplines | 10 disciplines | ✅ Complete |

## Design Principles

1. **Every skill answers:** "Why would I spend points here instead of combat skills?"
2. **Religion is meaningful but not mandatory:** 5-8% power contribution at maximum
3. **Classes have identity:** Unique primary skills, secondary resources, and playstyles
4. **Economy is balanced:** Clear gold sources and sinks with inflation controls
5. **ServUO compatible:** All systems implementable within existing framework

---

# PART I: WORLD FOUNDATION

## 1.1 Continental Geography

Vystia is a single continent with 15 distinct regions.

### Regional Overview

| Region | Climate | Primary Faction | Primary Religion | Danger Level |
|--------|---------|-----------------|------------------|--------------|
| **Frosthold** | Arctic | Polar Alliance | Frosthelm Faith | High |
| **Winterguard** | Subarctic | Polar Alliance | Frosthelm Faith | Medium |
| **Verdantpeak** | Temperate Forest | Sylvan Concord | Lunara's Covenant | Medium |
| **Shadowfen** | Swamp | Sylvan Concord | Lunara's Covenant | High |
| **Crystal Barrens** | Arid Wasteland | Arcane Coalition | Celestis Arcanum | High |
| **Deepforge** | Volcanic Caves | Ironclad Alliance | Cogsmith Creed | High |
| **The Emberlands** | Volcanic | Ironclad Alliance | Cogsmith Creed | Extreme |
| **Whispering Sands** | Desert | League of Sands | Surya's Sandscript | Medium |
| **Ironclad Empire** | Temperate Plains | Ironclad Alliance | Cogsmith Creed | Low |
| **Skyreach Peak** | Alpine | Highland Compact | Celestis Arcanum | High |
| **ShadowVoid** | Corrupted | None (Hostile) | None | Extreme |
| **Sunken Isles** | Tropical Maritime | Maritime Sovereignty | Oceana's Covenant | Medium |
| **Glimmering Archipelago** | Tropical | Maritime Sovereignty | Oceana's Covenant | Low |
| **Forgotten Depths** | Underwater | Maritime Sovereignty | Oceana's Covenant | High |
| **Wilderlands** | Variable | Highland Compact | Variable | Variable |

### Zone Control System

| Zone Type | Color | PvP Rules | Loot Rules | Death Penalty | Services |
|-----------|-------|-----------|------------|---------------|----------|
| **Sanctuary** | Green | No PvP | None | Healer resurrection | Full NPC services |
| **Contested** | Yellow | Flagging-based | Backpack only | Shrine resurrection | Limited services |
| **Lawless** | Red | Open PvP | Full loot | Distant shrine | None |
| **Extreme** | Black | Open PvP | Full loot | 0.5% skill loss, player rez only | None |

---

## 1.2 The Seven Factions

| Faction | Theme | Allied Regions | Opposed Faction | Primary Benefits |
|---------|-------|----------------|-----------------|------------------|
| **Ironclad Alliance** | Industry, Engineering | Ironclad, Emberlands, Deepforge | Sylvan Concord | Engineering recipes, construct vendors |
| **Sylvan Concord** | Nature, Preservation | Verdantpeak, Shadowfen | Ironclad Alliance | Nature recipes, druidic items |
| **League of Sands** | Trade, Desert Survival | Whispering Sands | Polar Alliance | Trade routes, desert gear |
| **Polar Alliance** | Frost, Endurance | Frosthold, Winterguard | League of Sands | Cold resist gear, survival items |
| **Maritime Sovereignty** | Naval, Underwater | Sunken Isles, Archipelago, Depths | None | Ships, underwater equipment |
| **Highland Compact** | Mountain, Rangers | Skyreach, Wilderlands | None | Scouting gear, alpine recipes |
| **Arcane Coalition** | Magic, Research | Crystal Barrens | None | Rare reagents, runic tools |

### Reputation Tiers

| Tier | Points Required | Discount | Vendor Access | Special Benefits |
|------|-----------------|----------|---------------|------------------|
| Hostile | -3000 to -1500 | +25% prices | None | Attacked on sight |
| Unfriendly | -1500 to 0 | +10% prices | Basic only | Suspicion |
| Neutral | 0 | None | Basic | Default state |
| Friendly | 1-1,500 | 5% | Standard | Basic faction quests |
| Allied | 1,501-4,500 | 10% | Advanced | Regional recipes |
| Honored | 4,501-9,000 | 12% | Rare | Legendary recipe access |
| Exalted | 9,001-15,000 | 15% | All | Title, unique items |

### Faction Reputation Gains

| Activity | Reputation Gain | Cooldown |
|----------|-----------------|----------|
| Minor quest | +50-100 | None |
| Major quest | +200-500 | Weekly |
| Boss kill (aligned) | +100 | Per kill |
| Donation (1,000g) | +50 | Daily cap: 500 |
| Opposing faction kill (PvP) | +25 | None |

---

# PART II: THE 25 CHARACTER CLASSES

## 2.1 Class Roster by Region

| Region | Classes | Theme |
|--------|---------|-------|
| **Frosthold** | Barbarian, Beastmaster, Ice Mage | Survival, cold, primal |
| **Emberlands** | Sorcerer | Elemental fire |
| **Whispering Sands** | Ranger, Illusionist | Desert survival, deception |
| **Shadowfen** | Witch | Hexes, corruption |
| **ShadowVoid** | Warlock, Necromancer | Dark pacts, death |
| **Verdantpeak** | Druid, Alchemist | Nature, transformation |
| **Crystal Barrens** | Wizard, Oracle | Arcane power, foresight |
| **Ironclad Empire** | Artificer, Fighter, Monk, Templar | Industry, discipline |
| **Underwater** | Summoner | Conjuration |
| **Multi-Regional** | Bounty Hunter, Knight, Shaman, Cleric, Paladin, Bard, Enchanter | Universal archetypes |

---

## 2.2 Complete Class Reference

| # | Class | Primary Skill | Skill ID | Secondary Resource | Combat Role |
|---|-------|---------------|----------|-------------------|-------------|
| 1 | Barbarian | Brutality | 70 | Fury (0-100) | Melee DPS |
| 2 | Beastmaster | Wildkin | 71 | Pack Bond (0-5 pets) | Pet DPS |
| 3 | Ice Mage | Cryomancy | 58 | ChillStacks (0-5) | Ranged DPS/CC |
| 4 | Sorcerer | Elemental Mastery | 61 | Elemental Resonance | Ranged DPS |
| 5 | Ranger | Archery | 75 | Focus (0-100) | Ranged DPS |
| 6 | Illusionist | Illusion | 69 | Mirage Energy | Control/Support |
| 7 | Witch | Hexcraft | 60 | Hex Power (0-100) | DoT/Debuff |
| 8 | Warlock | Dark Covenant | 62 | SoulShards (0-5) | Burst DPS |
| 9 | Necromancer | Necromancy | 64 | LifeForce (0-100) | Pet DPS/DoT |
| 10 | Druid | Wildcraft | 59 | Shapeshift Duration | Hybrid |
| 11 | Alchemist | Alchemy Mastery | 81 | Reagent Stock | Support/Crafting |
| 12 | Wizard | Arcana | 72 | Spell Slots | Burst DPS |
| 13 | Oracle | Divination | 63 | Foresight | Support/Control |
| 14 | Artificer | Engineering | 80 | Steam (0-100), Charges | Pet DPS/Utility |
| 15 | Fighter | Warfare | 76 | Discipline (0-100) | Tank/Melee DPS |
| 16 | Monk | Martial Arts | 77 | Chi (0-5) | Melee DPS |
| 17 | Templar | Divine Judgment | 78 | Zeal (0-100) | Tank/Support |
| 18 | Summoner | Conjuration | 65 | Summoning Power | Pet DPS |
| 19 | Bounty Hunter | Manhunting | 75 | Pursuit (0-5) | Melee DPS/Utility |
| 20 | Knight | Chivalry | 73 | Fortitude (0-10) | Tank |
| 21 | Shaman | Spiritcalling | 66 | Spirit Energy | Hybrid Support |
| 22 | Cleric | Benediction | 74 | Faith (0-100) | Healer |
| 23 | Paladin | Sacred Oath | 82 | Virtues (0-3 each) | Tank/Healer |
| 24 | Bard | Songweaving | 67 | Crescendo (0-20) | Support/Buff |
| 25 | Enchanter | Runeweaving | 68 | Runic Power | Support/Crafting |

---

## 2.3 Secondary Resources - Complete Reference

### Combat Resources

| Resource | Class | Range | Generation | Decay |
|----------|-------|-------|------------|-------|
| **Fury** | Barbarian | 0-100 | +8 hit, +15 crit, +20 kill | -5/sec OOC (reduced by Brutality) |
| **Focus** | Ranger | 0-100 | +5/sec standing still | -10 on miss, -3/sec moving |
| **Discipline** | Fighter | 0-100 | +5 on successful block | -2/sec |
| **Chi** | Monk | 0-5 | +1 per combo finisher | -1 every 30 sec OOC |
| **Zeal** | Templar | 0-100 | +15 on holy kill, +5 on heal | -3/sec out of combat |
| **Pursuit** | Bounty Hunter | 0-5 | +1 on marked target hit | Resets on mark death |
| **Fortitude** | Knight | 0-10 | +1 per 50 damage blocked | -1 every 60 sec OOC |

### Magic Resources

| Resource | Class | Range | Generation | Decay |
|----------|-------|-------|------------|-------|
| **ChillStacks** | Ice Mage | 0-5 | +1 per ice spell hit (on target) | -1 every 10 sec per target |
| **Elemental Resonance** | Sorcerer | 4 types | Builds per element cast | Shifts on element change |
| **Hex Power** | Witch | 0-100 | +10 per curse, +3 per tick | -5/sec OOC |
| **SoulShards** | Warlock | 0-5 | +1 on kill (chance) | Persist until spent |
| **LifeForce** | Necromancer | 0-100 | +15 on death nearby | -2/sec, -cost on summon |
| **Summoning Power** | Summoner | 0-100 | +10/sec meditating | -cost per summon |
| **Mirage Energy** | Illusionist | 0-100 | +5/sec hidden | -cost per illusion |
| **Spirit Energy** | Shaman | 0-100 | +10 near totem | -cost per ability |
| **Runic Power** | Enchanter | 0-100 | +15 per enchant | -decay over time |

### Divine Resources

| Resource | Class | Range | Generation | Decay |
|----------|-------|-------|------------|-------|
| **Faith** | Cleric | 0-100 | +10 per heal, +5 per prayer | -3/sec out of combat |
| **Virtues** | Paladin | 0-3 each | +1 per virtuous act | Reset on sin |
| **Foresight** | Oracle | 0-100 | +10 per divination | -5/sec |

### Hybrid/Technical Resources

| Resource | Class | Range | Generation | Decay |
|----------|-------|-------|------------|-------|
| **Shapeshift Duration** | Druid | Timer | Set on shapeshift | Counts down |
| **Spell Slots** | Wizard | Varies | Restored on rest | Spent on cast |
| **Crescendo** | Bard | 0-20 | +1 per song tick | Resets on song end |
| **Pack Bond** | Beastmaster | 0-5 pets | Per controlled pet | N/A |
| **Steam** | Artificer | 0-100 | +5/sec near boiler | -cost per gadget |
| **Charges** | Artificer | Stacks | Crafted items | Spent on use |
| **Reagent Stock** | Alchemist | Inventory | Gathered/purchased | Consumed on craft |

---

## 2.4 Class Templates

### Ice Mage

**Region:** Frosthold | **Role:** Ranged DPS / Crowd Control  
**Armor:** Cloth, Light Leather | **Weapons:** Staves, Wands, Daggers

```
SKILL RESERVATION:
Cryomancy 100, Magery 100, Arcane Insight 80, Meditation 80 = 360
Free Points: 340
```

**Recommended Build (Pure Frost):**
| Skill | Points |
|-------|--------|
| Cryomancy | 100 ® |
| Magery | 100 ® |
| Arcane Insight | 80 ® |
| Meditation | 80 ® |
| Inscription | 100 |
| Concentration | 80 |
| Alchemy | 60 |
| **Total** | **700** |

**ChillStack Rotation:**
1. Frostbolt (circle 3) → +1 ChillStack
2. Ice Lance spam → +1 per hit
3. At 4 stacks: Glacial Spike → bonus damage
4. At 5 stacks: Shatter (auto-freeze) → reset

**Natural Religion:** Frosthelm Faith (+10% freeze duration)

---

### Barbarian

**Region:** Frosthold | **Role:** Melee DPS / Bruiser  
**Armor:** Light/Medium (leather, hide) | **Weapons:** Two-handed, dual wield

```
SKILL RESERVATION:
Brutality 100, Tactics 100, Anatomy 80, Blades 80 = 360
Free Points: 340
```

**Recommended Build (Berserker):**
| Skill | Points |
|-------|--------|
| Brutality | 100 ® |
| Tactics | 100 ® |
| Anatomy | 80 ® |
| Blades | 80 ® |
| Bludgeoning | 100 |
| Parrying | 80 |
| Healing | 80 |
| Camping | 80 |
| **Total** | **700** |

**Fury Rotation:**
1. Engage → start generating Fury
2. At 50+ Fury → Enraged state active
3. War Cry → Reckless Swing → Savage Strike
4. At 80+ Fury → Berserker Rage
5. Deathblow on targets <25% HP

**Natural Religion:** Frosthelm Faith (-15% Fury decay)

---

### Witch

**Region:** Shadowfen | **Role:** DoT DPS / Debuffer  
**Armor:** Cloth, Light Leather | **Weapons:** Staves, Wands, Ritual Knives

```
SKILL RESERVATION:
Hexcraft 100, Magery 100, Herbalism 80, Spiritcalling 70 = 350
Free Points: 350
```

**Recommended Build (Curse Weaver):**
| Skill | Points |
|-------|--------|
| Hexcraft | 100 ® |
| Magery | 100 ® |
| Herbalism | 80 ® |
| Spirit Communion | 70 ® |
| Alchemy | 100 |
| Meditation | 100 |
| Inscription | 50 |
| **Total** | **700** |

**Hex Rotation:**
1. Curse of Weakness → -stats, +10 Hex Power
2. Wither → DoT, +3 per tick
3. Plague → spreading DoT
4. At 70+ Hex Power → Coven's Grasp
5. Witch's Doom → consume hexes for burst

**Natural Religion:** Lunara's Covenant (+15% hex duration)

---

### Wizard

**Region:** Crystal Barrens | **Role:** Burst DPS  
**Armor:** Cloth | **Weapons:** Staves, Wands

```
SKILL RESERVATION:
Arcana 100, Magery 100, Arcane Insight 100, Meditation 100 = 400
Free Points: 300
```

**Natural Religion:** Celestis Arcanum (+10% mana regen)

---

### Sorcerer

**Region:** Emberlands | **Role:** Ranged DPS  
**Armor:** Cloth, Light Leather | **Weapons:** Staves, Orbs

```
SKILL RESERVATION:
Elemental Mastery 100, Magery 100, Meditation 90 = 290
Free Points: 410
```

**Natural Religion:** Cogsmith Creed (+5% fire damage)

---

### Necromancer

**Region:** ShadowVoid | **Role:** Pet DPS / DoT  
**Armor:** Cloth, Bone | **Weapons:** Staves, Ritual Items

```
SKILL RESERVATION:
Necromancy 100, Spiritcalling 100, Necromancy 80 = 280
Free Points: 420
```

**Natural Religion:** Celestis Arcanum (+15 max LifeForce)

---

### Warlock

**Region:** ShadowVoid | **Role:** Burst DPS  
**Armor:** Cloth, Light Leather | **Weapons:** Staves, Daggers

```
SKILL RESERVATION:
Dark Covenant 100, Hexcraft 100, Spiritcalling 80 = 280
Free Points: 420
```

**Natural Religion:** Celestis Arcanum (+5% SoulShard generation)

---

### Druid

**Region:** Verdantpeak | **Role:** Hybrid  
**Armor:** Leather, Hides | **Weapons:** Staves, Natural Weapons

```
SKILL RESERVATION:
Wildcraft 100, Shapeshifting 100, Herbalism 80 = 280
Free Points: 420
```

**Natural Religion:** Lunara's Covenant (+25% shapeshift duration)

---

### Shaman

**Region:** Multi-Regional | **Role:** Hybrid Support  
**Armor:** Leather, Bone | **Weapons:** Totems, Staves

```
SKILL RESERVATION:
Spiritcalling 100, Elemental Mastery 100, Animal Spirit 80 = 280
Free Points: 420
```

**Natural Religion:** Lunara's Covenant (+25% totem duration)

---

### Beastmaster

**Region:** Frosthold | **Role:** Pet DPS  
**Armor:** Leather, Hides | **Weapons:** Spears, Bows

```
SKILL RESERVATION:
Wildkin 100, Beast Wisdom 100, Beast Command 100, Animal Spirit 80 = 380
Free Points: 320
```

**Natural Religion:** Frosthelm Faith (+10% pet cold resist)

---

### Illusionist

**Region:** Whispering Sands | **Role:** Control / Support  
**Armor:** Cloth | **Weapons:** Wands, Daggers

```
SKILL RESERVATION:
Illusion 100, Magery 100, Hiding 80, Stealth 80 = 360
Free Points: 340
```

**Natural Religion:** Surya's Sandscript (+15% illusion duration)

---

### Ranger

**Region:** Whispering Sands | **Role:** Ranged DPS  
**Armor:** Leather | **Weapons:** Bows, Crossbows

```
SKILL RESERVATION:
Archery 100, Tracking 100, Camping 80, Wildkin 70 = 350
Free Points: 350
```

**Natural Religion:** Surya's Sandscript (+10 max Focus)

---

### Fighter

**Region:** Ironclad Empire | **Role:** Tank / Melee DPS  
**Armor:** Any | **Weapons:** Any melee

```
SKILL RESERVATION:
Weapon Mastery 100, Warfare 100, Tactics 80, Anatomy 80 = 360
Free Points: 340
```

**Natural Religion:** Cogsmith Creed (-15% repair costs)

---

### Knight

**Region:** Multi-Regional | **Role:** Tank  
**Armor:** Plate | **Weapons:** Sword & Shield, Lance

```
SKILL RESERVATION:
Chivalry 100, Warfare 100, Parrying 100, Blades 80 = 380
Free Points: 320
```

**Natural Religion:** Frosthelm Faith (+2 max Fortitude, +5% block)

---

### Monk

**Region:** Ironclad Empire | **Role:** Melee DPS  
**Armor:** Cloth, Light | **Weapons:** Unarmed, Staff

```
SKILL RESERVATION:
Martial Arts 100, Combat Meditation 100, Concentration 80 = 280
Free Points: 420
```

**Natural Religion:** Cogsmith Creed (-10% Chi ability cost)

---

### Templar

**Region:** Ironclad Empire | **Role:** Tank / Support  
**Armor:** Plate | **Weapons:** Maces, Shields

```
SKILL RESERVATION:
Divine Judgment 100, Warfare 100, Chivalry 80, Parrying 70 = 350
Free Points: 350
```

**Natural Religion:** Cogsmith Creed (+15% Zeal generation)

---

### Paladin

**Region:** Multi-Regional | **Role:** Tank / Healer  
**Armor:** Plate | **Weapons:** Swords, Maces, Shields

```
SKILL RESERVATION:
Chivalry 100, Divine Judgment 100, Blades 80, Tactics 80 = 360
Free Points: 340
```

**Natural Religion:** Surya's Sandscript (+15% Virtue accumulation)

---

### Cleric

**Region:** Multi-Regional | **Role:** Healer  
**Armor:** Cloth, Chain | **Weapons:** Maces, Staves

```
SKILL RESERVATION:
Benediction 100, Concentration 100, Meditation 80 = 280
Free Points: 420
```

**Natural Religion:** Any (+15% Faith generation)

---

### Oracle

**Region:** Crystal Barrens | **Role:** Support / Control  
**Armor:** Cloth | **Weapons:** Staves, Orbs

```
SKILL RESERVATION:
Divination 100, Spiritcalling 100, Concentration 90 = 290
Free Points: 410
```

**Natural Religion:** Surya's Sandscript (+5 Divination skill)

---

### Summoner

**Region:** Underwater | **Role:** Pet DPS  
**Armor:** Cloth | **Weapons:** Staves, Orbs

```
SKILL RESERVATION:
Conjuration 100, Magery 100, Meditation 80, Spiritcalling 70 = 350
Free Points: 350
```

**Natural Religion:** Oceana's Covenant (+15% summon HP)

---

### Bard

**Region:** Multi-Regional | **Role:** Support / Buff  
**Armor:** Cloth, Light Leather | **Weapons:** Instruments, Rapiers

```
SKILL RESERVATION:
Songweaving 100, Musicianship 100, Provocation 80, Discordance 70 = 350
Free Points: 350
```

**Natural Religion:** Oceana's Covenant (+3 max Crescendo)

---

### Enchanter

**Region:** Multi-Regional | **Role:** Support / Crafting  
**Armor:** Cloth | **Weapons:** Staves, Wands

```
SKILL RESERVATION:
Runeweaving 100, Transmutation 100, Magery 80, Inscription 70 = 350
Free Points: 350
```

**Natural Religion:** Celestis Arcanum (+5% enchant success)

---

### Artificer

**Region:** Ironclad Empire | **Role:** Pet DPS / Utility  
**Armor:** Leather, Chain | **Weapons:** Crossbows, Gadgets

```
SKILL RESERVATION:
Engineering 100, Tinkering 80, Blacksmithy 70 = 250
Free Points: 450
```

**Natural Religion:** Cogsmith Creed (+15% Steam regen)

---

### Alchemist

**Region:** Verdantpeak | **Role:** Support / Crafting  
**Armor:** Cloth, Leather | **Weapons:** Staves, Thrown

```
SKILL RESERVATION:
Alchemy Mastery 100, Alchemy 100, Herbalism 80 = 280
Free Points: 420
```

**Natural Religion:** Lunara's Covenant (+10% potion effectiveness)

---

### Bounty Hunter

**Region:** Multi-Regional | **Role:** Melee DPS / Utility  
**Armor:** Leather | **Weapons:** Daggers, Crossbows

```
SKILL RESERVATION:
Manhunting 100, Tracking 100, Hiding 80, Stealth 80 = 360
Free Points: 340
```

**Natural Religion:** Surya's Sandscript (+1 max Pursuit, +10% mark damage)

---

# PART III: SKILLS AND PROFESSIONS

## 3.1 Skill Economy

| Element | Value |
|---------|-------|
| **Total Skill Points** | 700 |
| **Skill Cap** | 100 per skill |
| **Standard Skills** | 52 |
| **Vystia Custom Skills** | 26 (IDs 58-83) |
| **Total Available** | 78 skills |

### Skill Progression

| Skill Level | Title | Gain Chance |
|-------------|-------|-------------|
| 0-29 | Novice | 7-10% |
| 30-49 | Apprentice | 5-7% |
| 50-69 | Journeyman | 3-5% |
| 70-89 | Expert | 1-3% |
| 90-99 | Master | 0.1-1% |
| 100 | Grandmaster | — |

**Gain Formula:**
```
Gain Chance = (100 - CurrentSkill) / 10 %
```

**Gain Modifiers:**
- Appropriate difficulty: ×1.0
- Too easy: ×0.1 or no gain
- Too hard: Fail, no gain
- Power Hour (first hour daily): ×2.0
- Training dummy/NPC: ×0.5, capped at 50

---

## 3.2 Vystia Custom Skills (IDs 58-83)

### Magic School Skills (12)

| ID | Skill | Primary Class | Spell IDs | Function |
|----|-------|---------------|-----------|----------|
| 58 | **Cryomancy** | Ice Mage | 1000-1031 | Ice magic, freeze, ChillStacks |
| 59 | **Wildcraft** | Druid | 1032-1063 | Nature magic, growth, healing |
| 60 | **Hexcraft** | Witch | 1064-1095 | Curses, hexes, decay |
| 61 | **Elemental Mastery** | Sorcerer | 1096-1127 | Fire/ice/lightning/earth |
| 62 | **Dark Covenant** | Warlock | 1128-1159 | Dark pacts, life drain |
| 63 | **Divination** | Oracle | 1160-1191 | Foresight, prophecy |
| 64 | **Necromancy** | Necromancer | 1192-1223 | Undead control, death magic |
| 65 | **Conjuration** | Summoner | 1224-1255 | Summoning creatures |
| 66 | **Spiritcalling** | Shaman | 1256-1287 | Ancestral spirits, totems |
| 67 | **Songweaving** | Bard | 1288-1319 | Bardic music, inspiration |
| 68 | **Runeweaving** | Enchanter | 1320-1351 | Protection, warding, enchantment |
| 69 | **Illusion** | Illusionist | 1352-1383 | Deception, misdirection |

### Combat & Class Skills (14)

| ID | Skill | Primary Class | Function |
|----|-------|---------------|----------|
| 70 | **Brutality** | Barbarian | Fury generation, rage abilities |
| 71 | **Wildkin** | Beastmaster | Pet taming, control |
| 72 | **Arcana** | Wizard | Direct damage magic scaling |
| 73 | **Chivalry** | Knight | Honor abilities, mounted combat |
| 74 | **Benediction** | Cleric | Divine healing, buffs |
| 75 | **Manhunting** | Bounty Hunter | Stealth combat, ambush, tracking |
| 76 | **Warfare** | Fighter | Combat strategy, weapon techniques |
| 77 | **Martial Arts** | Monk | Unarmed combat, chi |
| 78 | **Divine Judgment** | Templar | Holy damage, smiting |
| 79 | **Shapeshifting** | Druid | Animal/elemental forms |
| 80 | **Engineering** | Artificer | Constructs, gadgets |
| 81 | **Transmutation** | Alchemist | Potion enhancement |
| 82 | **Sacred Oath** | Paladin | Self-cost for power, virtues |
| 83 | **Animal Spirit** | Beastmaster/Shaman | Animal trait bonuses |

---

## 3.3 Class Skill Definitions

### Cryomancy (ID 58)
*"Mastery of ice and cold. The art of freezing, slowing, and shattering."*

| Cryomancy | Capability |
|-----------|------------|
| 0 | Cannot cast ice spells |
| 30 | Cast ice circles 1-3, +5% cold damage |
| 50 | Cast ice circles 1-5, +10% cold damage, freeze duration +10% |
| 70 | Cast ice circles 1-7, +15% cold damage, freeze duration +20% |
| 90 | Cast all ice circles, +20% cold damage, freeze duration +30% |
| 100 | **Frostmaster:** +25% cold damage, freeze duration +40%, immune to freeze/slow |

**ChillStack Scaling:**
```
Max ChillStacks = 3 + (Cryomancy / 50)
```

| ChillStacks | Effect on Target |
|-------------|------------------|
| 1 | -5% movement speed |
| 2 | -10% movement speed |
| 3 | -15% movement, -5% attack speed |
| 4 | -20% movement, -10% attack speed |
| 5 | **Frozen:** Immobilized 2 sec, reset |

---

### Brutality (ID 70)
*"Savage combat fueled by primal rage. The way of the berserker."*

| Brutality | Capability |
|-----------|------------|
| 0 | Base Fury generation only |
| 30 | Fury decay -10%, +5% damage while Fury >50, basic Rage abilities |
| 50 | Fury decay -20%, +10% damage, tier 2 Rage abilities |
| 70 | Fury decay -30%, +15% damage, Rage cost -15% |
| 90 | Fury decay -40%, +20% damage, Rage cost -25% |
| 100 | **Berserker:** Fury decay -50%, +25% damage, Fury gen +25%, Rage cost -35% |

**Rage Abilities:**

| Ability | Fury Cost | Brutality | Effect |
|---------|-----------|-----------|--------|
| Savage Strike | 20 | 30 | +50% damage next attack |
| War Cry | 30 | 30 | +10% damage/speed 10s |
| Reckless Swing | 25 | 50 | AoE, -20% defense 5s |
| Blood Frenzy | 40 | 50 | +3% life steal 15s |
| Unstoppable | 50 | 70 | CC immune 5s |
| Rampage | 35 | 70 | Kill extends duration |
| Berserker Rage | 60 | 90 | +30% damage, +20% speed, -25% def |
| Deathblow | 80 | 100 | +100% damage vs <25% HP |

---

### Hexcraft (ID 60)
*"The weaving of curses, hexes, and malevolent magic."*

| Hexcraft | Capability |
|----------|------------|
| 0 | Cannot cast hex spells |
| 30 | Circles 1-3, +5% curse damage, hex duration +10% |
| 50 | Circles 1-5, +10% damage, +20% duration, max 2 hexes/target |
| 70 | Circles 1-7, +15% damage, +30% duration, max 3 hexes |
| 90 | All circles, +20% damage, +40% duration, max 4 hexes |
| 100 | **Coven Master:** +25% damage, +50% duration, 5 hexes, spread on death |

**Hex Power Abilities:**

| Ability | Cost | Hexcraft | Effect |
|---------|------|----------|--------|
| Curse Amplification | 30 | 30 | All hexes +25% effect 10s |
| Hex Transfer | 40 | 50 | Move hexes to new target |
| Coven's Grasp | 50 | 70 | Root + +50% hex damage 5s |
| Mass Affliction | 60 | 70 | AoE hex application |
| Soul Rot | 70 | 90 | Massive DoT, -50% healing |
| Witch's Doom | 100 | 100 | Consume all hexes for burst |

---

## 3.4 Standard Skills (Modified for Vystia)

### Combat Skills (12)

| Skill | Vystia Name | Key Capability at GM |
|-------|-------------|---------------------|
| Swordsmanship | **Blades** | +35% accuracy, +25% special damage |
| Mace Fighting | **Bludgeoning** | +35% accuracy, armor penetration |
| Fencing | **Piercing** | +35% accuracy, +5% crit |
| Archery | **Archery** | +35% accuracy, +8 range, moving shot |
| Wrestling | Wrestling | Unarmed +50%, Takedown |
| Tactics | Tactics | +25% physical damage |
| Anatomy | Anatomy | +25% damage and bandage healing |
| Parrying | Parrying | 50% block, Riposte +25% |
| Magic Resist | **Spell Resistance** | 50% magic damage reduction |
| Focus | **Concentration** | +5 mana/stam regen, +25% CC resist |
| Detecting Hidden | **Perception** | See Invisible, 16 tile detect |
| Remove Trap | **Disarmament** | All traps, Redirect |

### Crafting Skills (9)

| Skill | Key Capability at GM |
|-------|---------------------|
| **Alchemy** | Legendary potions, 4 stacking, Void Poison |
| **Blacksmithy** | Void Iron, legendary smithing |
| **Carpentry** | Legendary bows, flagship hulls |
| **Tailoring** | Bag of Holding, Void Silk |
| **Tinkering** | Titan Construct components |
| **Inscription** | Permanent enchantments |
| **Cooking** | Feast cooking (raids) |
| **Cartography** | Void charts, hidden locations |
| **Appraisal** | +50% vendor prices, curse detection |

### Gathering Skills (6)

| Skill | Key Capability at GM |
|-------|---------------------|
| **Mining** | Void Iron, legendary gems, +50% yield |
| **Lumberjacking** | Void Oak, ancient groves, +50% yield |
| **Fishing** | Void Sea, Leviathan, best buff fish |
| **Herbalism** | 4 garden plots, legendary reagents |
| **Butchery** | All exclusive materials, Mass Harvest |
| **Seamanship** | Void Sea, flagships, +25% speed |

### Animal Skills (4)

| Skill | Vystia Name | Key Capability at GM |
|-------|-------------|---------------------|
| Animal Taming | **Beast Bonding** | Dragon taming, 5 slots |
| Animal Lore | **Beast Wisdom** | +50% pet stats, Perfect Bond |
| Herding | **Beast Command** | 100% success, Recall |
| Veterinary | Veterinary | Pet resurrection |

### Stealth Skills (4)

| Skill | Key Capability at GM |
|-------|---------------------|
| **Hiding** | Combat vanish (30s CD) |
| **Stealth** | 100% speed, no sound |
| **Lockpicking** | Legendary locks, 50% faster |
| **Sleight of Hand** | Plant items on targets |

### Investigation Skills (3)

| Skill | Vystia Name | Key Capability at GM |
|-------|-------------|---------------------|
| Forensic Eval | **Analysis** | +20% party-wide damage debuff |
| Tracking | **Tracking** | Execute Mark (+50% finisher) |
| Begging | **Persuasion** | 3 Legendary sidekicks, -10% prices |

### Survival Skills (2)

| Skill | Key Capability at GM |
|-------|---------------------|
| **Camping** | Hike anywhere (5 min CD), Outpost |
| **Weapon Expertise** | +10% special chance (20% total) |

---

## 3.5 Camping/Hiking System

| Camping | Capability |
|---------|------------|
| 0-29 | Light campfire |
| 30-39 | Hike to: Discovered towns (15 min CD) |
| 40-49 | +5% wilderness damage |
| 50-59 | Hike to: Discovered dungeons (12 min CD) |
| 60-69 | +10% wilderness damage |
| 70-79 | Hike to: Wilderness waypoints (10 min CD) |
| 80-89 | +15% wilderness damage, Hike to: Player houses |
| 90-99 | Safe Camp (no spawns 8 tiles) |
| 100 | Hike anywhere (5 min CD), **Outpost** creation |

**Hiking Restrictions:**
- Interruptible by damage
- Cannot hike into Extreme (black) zones
- Cannot hike while criminal/murderer flagged
- Cannot hike within 60 sec of PvP
- Cannot hike from inside dungeons
- Must have discovered destination

---

# PART IV: MAGIC SYSTEM

## 4.1 The 12 Magic Schools

| School | Skill | Skill ID | Spells | Spell IDs | Primary Class |
|--------|-------|----------|--------|-----------|---------------|
| Ice Magic | Cryomancy | 58 | 32 | 1000-1031 | Ice Mage |
| Nature Magic | Wildcraft | 59 | 32 | 1032-1063 | Druid |
| Hex Magic | Hexcraft | 60 | 32 | 1064-1095 | Witch |
| Elemental Magic | Elemental Mastery | 61 | 32 | 1096-1127 | Sorcerer |
| Dark Magic | Dark Covenant | 62 | 32 | 1128-1159 | Warlock |
| Divination | Divination | 63 | 32 | 1160-1191 | Oracle |
| Necromancy | Necromancy | 64 | 32 | 1192-1223 | Necromancer |
| Summoning | Conjuration | 65 | 32 | 1224-1255 | Summoner |
| Shamanic | Spiritcalling | 66 | 32 | 1256-1287 | Shaman |
| Bardic | Songweaving | 67 | 32 | 1288-1319 | Bard |
| Enchanting | Runeweaving | 68 | 32 | 1320-1351 | Enchanter |
| Illusion | Illusion | 69 | 32 | 1352-1383 | Illusionist |

**Total: 384 Spells**

## 4.2 Spell Circle Requirements

| Circle | Min Skill | Mana Cost | Cast Time | Spells per School |
|--------|-----------|-----------|-----------|-------------------|
| 1st | 0 | 4 | 0.5s | 4 |
| 2nd | 10 | 6 | 0.75s | 4 |
| 3rd | 20 | 9 | 1.0s | 4 |
| 4th | 30 | 11 | 1.5s | 4 |
| 5th | 40 | 14 | 2.0s | 4 |
| 6th | 50 | 20 | 2.25s | 4 |
| 7th | 60 | 40 | 2.5s | 4 |
| 8th | 70 | 50 | 3.0s | 4 |

## 4.3 Reagents (104 Total)

Each magic school has 8 unique reagents (96 school-specific + 8 universal):

| School | Example Reagents |
|--------|------------------|
| Ice | Frostbloom, Glacier Crystal, Eternal Ice, Frozen Tear |
| Nature | Wild Moss, Moonpetal, Ancient Root, Treant Sap |
| Hex | Shadow Petal, Cursed Pearl, Bog Moss, Hag's Finger |
| Elemental | Primordial Ember, Lava Glass, Storm Feather, Living Ore |
| Dark | Void Weed, Void Crystal, Demon Blood, Brimstone |
| Divination | Time Sand, Fate Crystal, Divination Dust, Sphinx Scale |
| Necromancy | Grave Moss, Lich Dust, Bone Dust, Soul Essence |
| Summoning | Deep Pearl, Binding Salt, Planar Fragment, Anchor Stone |
| Shamanic | Spirit Feather, Primal Thunder, Ancestor Ash, Thunder Moss |
| Bardic | Echo Dust, Song Petal, Eternal Note, Resonance Crystal |
| Enchanting | Arcane Dust, Rune Fragment, Essence of Magic, Blank Rune |
| Illusion | Mirror Dust, Chaos Prism, Dream Essence, Phantom Thread |

---

# PART V: RELIGION SYSTEM

## 5.1 The Six Religions

| Religion | Deity | Domains | Aligned Faction | Opposed Religion |
|----------|-------|---------|-----------------|------------------|
| **Cogsmith Creed** | The Great Machinist | Progress, Fire, Industry | Ironclad Alliance | Lunara's Covenant |
| **Lunara's Covenant** | Lunara (Moon Goddess) | Nature, Healing, Moon | Sylvan Concord | Cogsmith Creed |
| **Surya's Sandscript** | Surya (Sun God) | Sun, Wisdom, Time | League of Sands | Oceana's Covenant |
| **Oceana's Covenant** | Oceana (Sea Mother) | Sea, Tides, Depths | Maritime Sovereignty | Surya's Sandscript |
| **Celestis Arcanum** | The Celestial Order | Magic, Stars, Balance | Arcane Coalition | Frosthelm Faith |
| **Frosthelm Faith** | The Frost Father | Winter, Endurance, Survival | Polar Alliance | Celestis Arcanum |

## 5.2 Piety System

### Piety Tiers

| Tier | Piety Range | Title | Unlocks |
|------|-------------|-------|---------|
| 0 | 0 | Faithless | No benefits |
| 1 | 1-49 | Initiate | Basic prayer, shrine access |
| 2 | 50-199 | Adherent | Passive bonus tier 1 |
| 3 | 200-499 | Devoted | Devotion Power 1, passive tier 2 |
| 4 | 500-899 | Zealot | Devotion Power 2, blessed crafting |
| 5 | 900-1000 | Champion | Devotion Power 3, divine title, shrine resurrection |

### Gaining Piety

| Action | Piety Gain | Cooldown |
|--------|------------|----------|
| Daily Prayer at Shrine | +10 | 20 hours |
| Tithe (gold donation) | +1 per 100g | Daily max +30 |
| Kill opposing religion (PvP) | +15 | None |
| Complete Pilgrimage | +75 | Weekly |
| Defeat Religion's Enemy Boss | +35 | Per kill |
| Craft Blessed Item | +3 | None |

### Losing Piety

| Action | Piety Loss |
|--------|------------|
| Kill same-religion follower | -100 |
| Desecrate shrine | -200 |
| 7 days without prayer | -5/day |
| Die to opposing religion (PvP) | -15 |

### Religion Change

| Change Number | Piety Retained | Maximum |
|---------------|----------------|---------|
| First | 50% | Cap at 200 |
| Second | 25% | Cap at 100 |
| Third+ | 0% | Full reset |

---

## 5.3 Passive Bonuses

### Tier 1 (Adherent - 50 Piety)

| Religion | Passive Bonus |
|----------|---------------|
| Cogsmith Creed | +3% Fire Resist, +3 Engineering |
| Lunara's Covenant | +3% Healing Received, +3 Wildcraft |
| Surya's Sandscript | +3% Fire Resist, +3 Divination |
| Oceana's Covenant | Water Breathing, +3 Conjuration |
| Celestis Arcanum | +3% Mana Regen, +3 Runeweaving |
| Frosthelm Faith | +3% Cold Resist, +5 HP |

### Tier 2 (Devoted - 200 Piety)

Stacks with Tier 1:

| Religion | Additional | Total |
|----------|------------|-------|
| Cogsmith Creed | +2% Fire, +2 Engineering | +5% Fire, +5 Eng |
| Lunara's Covenant | +2% Healing, +2 Wildcraft | +5% Heal, +5 WC |
| Surya's Sandscript | +2% Fire, +2 Divination | +5% Fire, +5 Div |
| Oceana's Covenant | +2 Conjuration, +15% Swim | Water Breath, +5 Conj |
| Celestis Arcanum | +2% Mana Regen, +2 Runeweaving | +5% Mana, +5 RW |
| Frosthelm Faith | +2% Cold, +5 HP | +5% Cold, +10 HP |

---

## 5.4 Devotion Powers

### Power 1 (200 Piety) - Utility

| Religion | Power | CD | Effect |
|----------|-------|----|----|
| Cogsmith Creed | Forge Blessing | 30 min | +10% exceptional craft chance |
| Lunara's Covenant | Moonlight Healing | 15 min | Heal 30-50 HP in 5 tiles |
| Surya's Sandscript | Sun's Revelation | 10 min | Reveal hidden 8 tiles, 20s |
| Oceana's Covenant | Tidal Surge | 15 min | Push enemies back 3 tiles |
| Celestis Arcanum | Arcane Insight | 10 min | See enemy stats 1 min |
| Frosthelm Faith | Frost Shield | 15 min | Absorb 50 damage, reflect 15% cold |

### Power 2 (500 Piety) - Combat

| Religion | Power | CD | Effect |
|----------|-------|----|----|
| Cogsmith Creed | Steam Burst | 10 min | AoE 30-50 fire, knockback |
| Lunara's Covenant | Nature's Sanctuary | 20 min | 4-tile zone +25% healing, 20s |
| Surya's Sandscript | Time Dilation | 20 min | +25% attack/cast speed, 10s |
| Oceana's Covenant | Abyssal Call | 30 min | Summon water elemental (200 HP, 2 min) |
| Celestis Arcanum | Mana Constellation | 20 min | Restore 35% mana to party |
| Frosthelm Faith | Endurance of Winter | 30 min | Cannot die 5s (HP min 1), -50% damage |

### Power 3 (900 Piety) - Ultimate

| Religion | Power | CD | Effect |
|----------|-------|----|----|
| Cogsmith Creed | Machinist's Grace | 60 min | Repair all gear, +15% damage 90s |
| Lunara's Covenant | Lunara's Embrace | 60 min | Full heal + cleanse + 5s damage immunity |
| Surya's Sandscript | Solar Judgment | 60 min | Cone 75-100 fire, blind 3s |
| Oceana's Covenant | Oceana's Wrath | 60 min | Tidal wave line damage, drowning DoT |
| Celestis Arcanum | Celestial Alignment | 60 min | 0 mana cost 8s (max 4 spells) |
| Frosthelm Faith | Absolute Zero | 60 min | Freeze enemies 5 tiles, 3s |

### Boss Interactions

| Rule | Effect |
|------|--------|
| Devotion Power Duration | 50% vs bosses |
| Absolute Zero | Bosses immune; adds 50% duration |
| Endurance of Winter | Cannot use when boss <25% HP |
| Celestial Alignment | 2-spell cap vs bosses |

---

## 5.5 Class-Religion Synergy

| Class | Natural Religion | Synergy Bonus |
|-------|------------------|---------------|
| Barbarian | Frosthelm Faith | Fury decay -15% |
| Ice Mage | Frosthelm Faith | Freeze duration +10% |
| Beastmaster | Frosthelm Faith | Pets +10% cold resist |
| Knight | Frosthelm Faith | +2 max Fortitude, +5% block |
| Sorcerer | Cogsmith Creed | Fire spells +5% damage |
| Artificer | Cogsmith Creed | Steam regen +15% |
| Fighter | Cogsmith Creed | Repair costs -15% |
| Monk | Cogsmith Creed | Chi ability cost -10% |
| Templar | Cogsmith Creed | Zeal generation +15% |
| Druid | Lunara's Covenant | Shapeshift duration +25% |
| Alchemist | Lunara's Covenant | Potion effectiveness +10% |
| Witch | Lunara's Covenant | Hex duration +15% |
| Shaman | Lunara's Covenant | Totem duration +25% |
| Ranger | Surya's Sandscript | +10 maximum Focus |
| Illusionist | Surya's Sandscript | Illusion duration +15% |
| Oracle | Surya's Sandscript | +5 Devotion skill |
| Paladin | Surya's Sandscript | Virtue accumulation +15% |
| Bounty Hunter | Surya's Sandscript | +1 max Pursuit, +10% mark damage |
| Summoner | Oceana's Covenant | Summon HP +15% |
| Bard | Oceana's Covenant | +3 max Crescendo |
| Necromancer | Celestis Arcanum | +15 max LifeForce |
| Warlock | Celestis Arcanum | SoulShard gen +5% |
| Wizard | Celestis Arcanum | Mana regen +10% |
| Enchanter | Celestis Arcanum | Enchant success +5% |
| Cleric | Any Religion | Faith generation +15% |

---

## 5.6 Religious PvP

### Damage Bonuses vs Opposed Religion

| Piety Tier | Damage Bonus | Damage Reduction |
|------------|--------------|------------------|
| Initiate | +2% | None |
| Adherent | +4% | None |
| Devoted | +6% | None |
| Zealot | +8% | None |
| Champion | +10% | -3% damage taken |

### Grouping Rules

| Relationship | Can Group? | Healing/Buff Effectiveness |
|--------------|------------|---------------------------|
| Same religion | Yes | 100% |
| Neutral religions | Yes | 100% |
| Opposed religions | Yes | 50% |

---

## 5.7 Blessed Items

### Blessing Process
1. Bring craftable item to shrine of your religion
2. Pay tithe = 5% of item base value
3. Piety check determines success
4. Item gains religion-specific blessing

### Success Rates

| Piety | Success | Critical |
|-------|---------|----------|
| 500-599 | 50% | 3% |
| 600-699 | 60% | 5% |
| 700-799 | 70% | 8% |
| 800-899 | 80% | 12% |
| 900-1000 | 90% | 18% |

### Blessing Effects

| Religion | Standard | Critical |
|----------|----------|----------|
| Cogsmith Creed | +5% Fire Damage | +10% Fire, Self-Repair 2 |
| Lunara's Covenant | +5% Healing Power | +10% Healing, HP Regen 2 |
| Surya's Sandscript | +5% Hit Chance | +10% Hit Chance |
| Oceana's Covenant | +5 Mana | +10 Mana |
| Celestis Arcanum | +5% Spell Damage | +10% Spell Damage |
| Frosthelm Faith | +5 HP | +10 HP |

### Usage by Religion

| User Type | Effect |
|-----------|--------|
| Same religion | Full effect |
| Faithless | 50% effect |
| Different (non-opposed) | 25% effect |
| Opposed religion | Cannot equip |

---

## 5.8 Shrines and Temples

### Major Temples (1 per religion)

| Religion | Temple | Location | Special |
|----------|--------|----------|---------|
| Cogsmith Creed | The Grand Foundry | Ironclad Capital | Engineering legendary blessing |
| Lunara's Covenant | Grove of the Moon | Verdantpeak Heart | Moonlight healing pool |
| Surya's Sandscript | Temple of the Sun | Desert Oasis | Solar prophecy chamber |
| Oceana's Covenant | Abyssal Cathedral | Forgotten Depths | Underwater resurrection |
| Celestis Arcanum | Astral Observatory | Crystal Barrens Peak | Starlight enchanting |
| Frosthelm Faith | Frost Father's Sanctum | Frosthold Summit | Endurance trials |

### Shrine Functions

| Function | Piety Required |
|----------|----------------|
| Prayer (+10 piety) | Initiate (1+) |
| Tithe | Initiate (1+) |
| Blessing Refresh (8 hr) | Adherent (50+) |
| Power Recharge | Devoted (200+) |
| Item Blessing | Zealot (500+) |
| Free Resurrection | Champion (900+) |

### Portable Shrines

| Item | Religion | Materials | Uses |
|------|----------|-----------|------|
| Cogsmith's Portable Anvil | Cogsmith | 15 Clockwork Ingot, 5 Steam Core | 5 |
| Moonstone Circle | Lunara | 15 Livingwood, 5 Moonpetal | 5 |
| Sun Dial | Surya | 15 Sunforged Ingot, 5 Time Sand | 5 |
| Tide Pool Basin | Oceana | 15 Abyssal Ingot, 5 Deep Pearl | 5 |
| Star Chart | Celestis | 15 Crystalline Ingot, 5 Arcane Dust | 5 |
| Frost Totem | Frosthelm | 15 Frostforged Ingot, 5 Eternal Ice | 5 |

---

# PART VI: ECONOMY SYSTEM

## 6.1 Gold Sources

### Creature Loot by Region

| Region | Low Tier | Medium Tier | High Tier |
|--------|----------|-------------|-----------|
| Frosthold | 10-30g | 50-100g | 150-300g |
| Emberlands | 15-35g | 60-120g | 200-400g |
| Shadowfen | 10-25g | 40-90g | 150-300g |
| Verdantpeak | 10-30g | 50-100g | 150-350g |
| Crystal Barrens | — | 75-150g | 250-500g |
| Ironclad | 20-40g | 80-150g | 300-500g |
| ShadowVoid | — | 100-200g | 300-600g |
| Underwater | 15-35g | 60-120g | 200-400g |
| Skyreach | — | 50-120g | 200-400g |

### Boss Rewards

| Boss | Gold | Guaranteed Materials | Rare Drop (15%) |
|------|------|---------------------|-----------------|
| Frost Father | 3,000-4,000g | 5 Frostforged, 3 Eternal Ice | Heart of Winter |
| Volcano Wyrm | 3,500-4,500g | 5 Emberforged, 3 Everburning Coal | Lava Pearl |
| Sphinx of Surya | 3,000-4,000g | 5 Sunforged, 3 Time Sand | Sphinx Eye |
| Coven Matriarch | 2,800-3,800g | 5 Shadowforged, 3 Cursed Pearl | Hag's Heart |
| Ancient Treant | 3,200-4,200g | 5 Natureforged, 3 Treant Heart | Living Heartwood |
| Crystal Drake Alpha | 3,500-4,500g | 5 Crystalline, 3 Prismatic Shard | Prism Core |
| Forge Master | 4,000-5,000g | 5 Clockwork, 5 Steam Core | Forge Heart |
| Griffin Lord | 3,000-4,000g | 5 Skyforged, 3 Storm Feather | Griffin Heart |
| Ancient Kraken | 3,500-4,500g | 5 Abyssal, 3 Deep Pearl | Kraken Ink |
| Timeworn Lich | 4,500-6,000g | 5 Voidforged, 3 Lich Dust | Phylactery Fragment |

### Quest Rewards

| Quest Tier | Gold Reward | Level Range |
|------------|-------------|-------------|
| Tier 1 (Initiation) | 100-500g | 1-30 |
| Tier 2 (Apprentice) | 500-2,000g | 30-60 |
| Tier 3 (Journeyman) | 2,000-5,000g | 60-90 |
| Tier 4 (Master) | 5,000-15,000g | 90+ |
| Daily Quests | 200-800g | Repeatable |
| Faction Quests | 300-2,500g | +Reputation |

---

## 6.2 Gold Sinks

### Service Costs

| Service | Cost |
|---------|------|
| Cure Poison | 25g |
| Cure Disease | 50g |
| Remove Curse | 100g |
| Resurrection | 50g |
| Full Heal | 15g |
| Moongate (regional) | 100g |
| Moongate (cross-continental) | 250g |
| Ship Passage | 200-500g |
| Pet Stabling (week) | 30-100g |

### Repair Costs

| Material | Cost per Durability |
|----------|---------------------|
| Iron (Standard) | 2g |
| Regional Tier 1 | 35g |
| Regional Tier 2 | 50g |
| Voidforged/Legendary | 75g |

### Training Costs (Vystia Skills)

| Skill Range | Cost | Trainer |
|-------------|------|---------|
| 0-30 | 500g | Any class trainer |
| 30-50 | 2,000g | Regional trainer |
| 50-70 | 5,000g | Faction (Allied+) |
| 70-85 | 10,000g | Master (Honored+) |
| 85-100 | Cannot train | Earn through use |

### Housing Costs

| Size | Purchase | Weekly | Storage |
|------|----------|--------|---------|
| Small (7×7) | 50,000g | 500g | 500 items |
| Medium (11×11) | 150,000g | 1,500g | 1,000 items |
| Large (15×15) | 400,000g | 4,000g | 2,000 items |
| Keep (18×18) | 1,000,000g | 10,000g | 3,500 items |
| Castle (31×31) | 3,000,000g | 30,000g | 5,000 items |

### Religious Gold Sinks

| Sink | Cost | Frequency |
|------|------|-----------|
| Tithe for piety | 100g per piety | Daily max 30 |
| Blessing items | 5% item value | Per item |
| Portable shrines | ~1,500g materials | As needed |

---

## 6.3 Economy Balance

### Gold Flow Per Hour

| Activity | Gold In | Gold Out | Net |
|----------|---------|----------|-----|
| Casual PvE | 500-1,500g | 200-400g | +300-1,100g |
| Dungeon run | 2,000-5,000g | 500-1,000g | +1,500-4,000g |
| Boss kill | 3,500-5,500g | 800-1,500g | +2,000-4,000g |
| PvP session | 0-500g | 500-2,000g | -500 to -1,500g |

### Weekly Targets by Level

| Level | Gold/Hour | Weekly Target |
|-------|-----------|---------------|
| New (1-30) | 200-500g | 5,000-15,000g |
| Mid (30-60) | 500-1,500g | 15,000-50,000g |
| High (60-90) | 1,500-3,000g | 50,000-100,000g |
| Endgame (90+) | 3,000-10,000g | 100,000-300,000g |

---

# PART VII: CRAFTING SYSTEM

## 7.1 Ten Crafting Disciplines

| # | Discipline | Primary Class | Driving Skill | Workstation |
|---|------------|---------------|---------------|-------------|
| 1 | Transmutation | Alchemist | Alchemy Mastery | Alchemist's Lab |
| 2 | Engineering | Artificer | Engineering | Steam Forge |
| 3 | Runecrafting | Enchanter | Runeweaving | Runic Altar |
| 4 | Inscription | Oracle | Inscription | Scriptorium |
| 5 | Smithing | Fighter | Blacksmithy | Regional Forges |
| 6 | Leathercraft | Ranger | Tailoring | Tanning Rack |
| 7 | Woodshaping | Druid | Carpentry | Living Workshop |
| 8 | Clothcraft | Bard | Tailoring | Loom |
| 9 | Necrocraft | Necromancer | Necromancy | Ossuary |
| 10 | Jewelcraft | Sorcerer | Tinkering | Jeweler's Bench |

---

## 7.2 Transmutation (Alchemist)

### Secondary Resource Potions

| Recipe | Materials | Skill | Target Class | Effect |
|--------|-----------|-------|--------------|--------|
| Fury Draught | 5 Primordial Ember, 3 Mandrake | 50 | Barbarian | +15 max Fury, 10 min |
| Berserker's Blood | 10 Primordial Ember, 5 Sulfur | 75 | Barbarian | Fury decay -35%, 5 min |
| Chi Elixir | 5 Moonpetal, 3 Spirit Feather | 50 | Monk | +1 max Chi, 10 min |
| Focused Serum | 5 Time Sand, 3 Spider Silk | 55 | Ranger | Focus decay -35%, 5 min |
| Zealot's Tonic | 5 Lava Glass, 3 Sulfur | 55 | Templar | +1 Zeal per kill, 5 min |
| Knight's Fortifier | 5 Iron Ore, 5 Moonpetal | 60 | Knight | +2 max Fortitude, 10 min |
| Hunter's Mark Oil | 5 Shadow Petal, 3 Blood Moss | 60 | Bounty Hunter | Mark duration +35% |
| Shard Catalyst | 5 Void Weed, 3 Nightshade | 65 | Warlock | +5% Soul Shard gen |
| LifeForce Flask | 10 Grave Moss, 5 Bone Dust | 70 | Necromancer | Store 35 LifeForce |
| Chill Enhancer | 5 Glacier Crystal, 3 Frostbloom | 55 | Ice Mage | +5% freeze duration |
| Crescendo Catalyst | 5 Echo Dust, 3 Song Petal | 60 | Bard | Crescendo gen +35% |
| Faith Vessel | 5 Divination Dust, 5 Wild Moss | 65 | Cleric | Store 35 Faith |
| Steam Concentrate | 10 Steam Core, 5 Sulfur | 70 | Artificer | Portable Steam (35 charges) |
| Virtue Essence | 5 Divination Dust, 5 Fate Crystal | 75 | Paladin | +1 to one Virtue |

---

## 7.3 Engineering (Artificer)

### Constructs

| Recipe | Materials | Skill | Stats | Slots |
|--------|-----------|-------|-------|-------|
| Clockwork Spider | 15 Gear, 10 Spring, 1 Steam Core | 65 | 50 HP, Scout | 1 |
| Repair Drone | 20 Gear, 10 Spring, 1 Steam Core | 70 | Heals constructs | 1 |
| Steam Turret | 25 Gear, 15 Iron, 2 Steam Core | 75 | 100 HP, Ranged | 2 |
| Iron Golem | 50 Gear, 30 Iron, 3 Steam Core | 85 | 500 HP, Tank | 3 |
| Siege Engine | 100 Gear, 50 Iron, 5 Steam Core | 95 | Territory warfare | 5 |

---

## 7.4 Smithing (Fighter)

### Regional Weapons

| Recipe | Materials | Skill | Region | Bonus |
|--------|-----------|-------|--------|-------|
| Frostforged Longsword | 15 Frostforged | 75 | Frosthold | +8% Cold |
| Emberforged Katana | 15 Emberforged | 75 | Emberlands | +8% Fire |
| Clockwork Mace | 15 Clockwork, 5 Gear | 80 | Ironclad | +4% Speed |
| Voidforged Scythe | 20 Voidforged | 85 | ShadowVoid | +8% Energy |

### Legendary Weapons (100 Skill)

| Recipe | Materials | Weapon |
|--------|-----------|--------|
| The Eternal Winter | 20 Frostforged, 10 Eternal Ice, 1 Heart of Winter | Legendary Halberd |
| Phoenix Ascension | 20 Emberforged, 10 Everburning Coal, 1 Lava Pearl | Legendary Katana |
| The Cogmaster | 20 Clockwork, 10 Steam Core, 1 Forge Heart | Legendary War Hammer |
| Voidcaller | 20 Voidforged, 10 Lich Dust, 1 Phylactery Fragment | Legendary Staff |

---

## 7.5 Regional Materials

| Region | Ingot | Wood | Leather | Special |
|--------|-------|------|---------|---------|
| Frosthold | Frostforged | Frostwood | Polar | Eternal Ice |
| Emberlands | Emberforged | Emberwood | Volcanic | Everburning Coal |
| Shadowfen | Shadowforged | Shadowwood | Swamp | Cursed Pearl |
| Verdantpeak | Natureforged | Livingwood | — | Treant Heart |
| Crystal Barrens | Crystalline | — | — | Prismatic Shard |
| Ironclad | Clockwork | — | — | Steam Core |
| ShadowVoid | Voidforged | — | Shadow | Lich Dust |
| Underwater | Abyssal | — | — | Deep Pearl |
| Skyreach | Skyforged | — | — | Storm Feather |
| Desert | Sunforged | — | Desert | Time Sand |

---

## 7.6 Crafted Item Quality

| Quality | Value | Requirements |
|---------|-------|--------------|
| Standard | 1.0× | Base skill |
| Quality | 1.4× | Skill + 10 |
| Exceptional | 1.8× | Skill + 20, quality materials |
| Masterwork | 2.5× | GM skill, runic tool |
| Legendary | 5.0× | GM skill, boss materials, faction rep |

---

# PART VIII: CREATURES AND BOSSES

## 8.1 Creature Distribution (138 Total)

| Region | Low | Medium | High | Total |
|--------|-----|--------|------|-------|
| Frosthold | 5 | 4 | 3 | 12 |
| Emberlands | 4 | 4 | 3 | 11 |
| Shadowfen | 5 | 4 | 2 | 11 |
| Verdantpeak | 5 | 5 | 3 | 13 |
| Crystal Barrens | 0 | 4 | 3 | 7 |
| Ironclad | 4 | 4 | 3 | 11 |
| ShadowVoid | 0 | 5 | 4 | 9 |
| Underwater | 5 | 5 | 3 | 13 |
| Skyreach | 0 | 5 | 3 | 8 |
| Desert | 4 | 4 | 2 | 10 |
| Multi-region | 10 | 15 | 8 | 33 |
| **Total** | **42** | **59** | **37** | **138** |

## 8.2 Regional Bosses (10)

| Boss | Region | HP | Phases | Enrage |
|------|--------|----|---------| -------|
| Frost Father | Frosthold | 50,000 | 3 | 15 min |
| Volcano Wyrm | Emberlands | 55,000 | 3 | 12 min |
| Sphinx of Surya | Desert | 45,000 | 4 | 20 min |
| Coven Matriarch | Shadowfen | 40,000 | 3 | 15 min |
| Ancient Treant | Verdantpeak | 60,000 | 3 | 18 min |
| Crystal Drake Alpha | Crystal Barrens | 52,000 | 3 | 15 min |
| Forge Master | Ironclad | 58,000 | 4 | 12 min |
| Griffin Lord | Skyreach | 48,000 | 3 | 15 min |
| Ancient Kraken | Underwater | 55,000 | 3 | 15 min |
| Timeworn Lich | ShadowVoid | 65,000 | 5 | 20 min |

## 8.3 Ancient Beings (12)

| Ancient Being | Region | Role | Religion |
|---------------|--------|------|----------|
| Frosthelm the Eternal Winter | Frosthold | Quest giver | Frost Father's Avatar |
| Emberflame the Ashen Tyrant | Emberlands | Quest giver | None |
| Verdantheart the Forest Guardian | Verdantpeak | Quest giver | Lunara's Herald |
| Crystalwing the Prismatic Oracle | Crystal Barrens | Quest giver | Celestis |
| Abyssus the Depth King | Underwater | Quest giver | Oceana |
| Elder Oakbark | Verdantpeak | Recipe teacher | Lunara |
| Sphynx of Emberlands | Emberlands | Riddle master | Surya |
| Frost Father's Avatar | Frosthold | Divine blessing | Frosthelm |
| Great Machinist's Construct | Ironclad | Divine blessing | Cogsmith |
| Lunara's Dryad Herald | Verdantpeak | Divine blessing | Lunara |
| The Crystal Sphinx | Crystal Barrens | Recipe teacher | Celestis |
| Ironbark the War-Ancient | Verdantpeak | Recipe teacher | None |

### Divine Blessings (Champion Only)

| Blessing | Ancient Being | Effect | Duration |
|----------|---------------|--------|----------|
| Machinist's Perfection | Great Machinist's Construct | +15% exceptional craft | 24 hr |
| Lunar Radiance | Lunara's Dryad Herald | +15% healing done/received | 24 hr |
| Solar Clarity | Sphynx of Emberlands | Immune to illusions/blind | 24 hr |
| Abyssal Favor | Abyssus | Water breathing, +35% swim | 24 hr |
| Starlight | Crystalwing | +15% spell power | 24 hr |
| Frost Father's Endurance | Frost Father's Avatar | +50 HP, immune to slow | 24 hr |

---

# PART IX: IMPLEMENTATION REFERENCE

## 9.1 Directory Structure

```
ServUO/Scripts/
├── Custom/VystiaClasses/
│   ├── Core/                    # Base class system
│   ├── Classes/                 # 25 class files
│   ├── Systems/                 # Resource, buff, damage systems
│   ├── Abilities/               
│   │   └── Generated/           # ~512 abilities
│   ├── Spells/                  # 384 spells (12 schools)
│   ├── Items/                   # Class items, foci
│   ├── Crafting/                # 10 crafting systems
│   ├── Religion/                # Religion system
│   ├── Gumps/                   # UI gumps
│   └── Commands/                # GM commands
├── Mobiles/Vystia/
│   ├── Bosses/                  # 10 regional bosses
│   ├── Ancients/                # 12 ancient beings
│   ├── Creatures/               # 138 creatures
│   ├── Trainers/                # 25 class trainers
│   ├── Priests/                 # 6 high priests
│   └── Vendors/                 # Faction vendors
├── Items/Vystia/
│   ├── Equipment/               # Weapons, armor
│   ├── Resources/               # Ores, ingots, woods
│   ├── Scrolls/                 # 384 spell scrolls
│   ├── Spellbooks/              # 12 spellbooks
│   └── Religious/               # Blessed items, shrines
└── Services/
    ├── AISidekicks/             # Sidekick system
    └── LLM/                     # LLM NPC integration
```

## 9.2 Skill ID Reference

| ID | Skill | Class |
|----|-------|-------|
| 58 | Cryomancy | Ice Mage |
| 59 | Wildcraft | Druid |
| 60 | Hexcraft | Witch |
| 61 | Elemental Mastery | Sorcerer |
| 62 | Dark Covenant | Warlock |
| 63 | Divination | Oracle |
| 64 | Necromancy | Necromancer |
| 65 | Conjuration | Summoner |
| 66 | Spiritcalling | Shaman |
| 67 | Songweaving | Bard |
| 68 | Runeweaving | Enchanter |
| 69 | Illusion | Illusionist |
| 70 | Brutality | Barbarian |
| 71 | Wildkin | Beastmaster |
| 72 | Arcana | Wizard |
| 73 | Chivalry | Knight |
| 74 | Benediction | Cleric |
| 75 | Manhunting | Bounty Hunter |
| 76 | Warfare | Fighter |
| 77 | Martial Arts | Monk |
| 78 | Divine Judgment | Templar |
| 79 | Shapeshifting | Druid |
| 80 | Engineering | Artificer |
| 81 | Transmutation | Alchemist |
| 82 | Sacred Oath | Paladin |
| 83 | Animal Spirit | Beastmaster/Shaman |

## 9.3 PlayerMobile Extensions

| Property | Type | Purpose |
|----------|------|---------|
| `VystiaClass` | enum | Current class (0-24) |
| `Religion` | enum | Current religion (0-6) |
| `Piety` | int | Current piety (0-1000) |
| `LastPrayer` | DateTime | Prayer cooldown |
| `BlessingExpires` | DateTime | Active blessing timer |
| `SecondaryResource` | int | Class resource value |

## 9.4 GM Commands

| Command | Effect |
|---------|--------|
| `[SetReligion <player> <religion>` | Set player religion |
| `[SetPiety <player> <amount>` | Set player piety |
| `[AddPiety <player> <amount>` | Add piety |
| `[SpawnShrine <religion>` | Spawn shrine |
| `[SpawnHighPriest <religion>` | Spawn high priest |
| `[VA` | Vystia Admin Gump |
| `[QE` | Quest Editor |
| `[spawnvystia` | Spawn Vystia creatures |

---

# PART X: BALANCE SUMMARY

## 10.1 Power Budget

| Component | Power % |
|-----------|---------|
| Base stats (STR/DEX/INT) | 15% |
| Primary class skill | 25% |
| Secondary resource | 20% |
| Abilities/spells | 25% |
| Equipment | 10% |
| **Class Total** | **95%** |
| Religion (max) | **5%** |
| **Total** | **100%** |

## 10.2 PvP Balance Targets

| Scenario | Expected Outcome |
|----------|------------------|
| Same class, same religion | 50/50 |
| Same class, opposed religion (Champion) | 55/45 to attacker |
| Religious vs Faithless | 52/48 to religious |
| Optimal class-religion vs suboptimal | 53/47 |

## 10.3 PvE Balance Targets

| Metric | Target | Tolerance |
|--------|--------|-----------|
| Dungeon clear (religious vs non) | +10% faster | Max +15% |
| Boss kill time (religious party) | -8% | Max -12% |
| Devotion Power impact | Utility only | No trivializing |

## 10.4 Economy Targets

| Metric | Target |
|--------|--------|
| New player to Adherent | ~5 hours |
| New player to Devoted | ~15 hours |
| New player to Champion | ~60 hours |
| Gold inflation rate | <5% per month |
| Crafted vs dropped value | ±20% |

---

# APPENDICES

## Appendix A: Religion Quick Reference

| Religion | Deity | Region | Faction | Opposed | Theme |
|----------|-------|--------|---------|---------|-------|
| Cogsmith Creed | Great Machinist | Ironclad | Ironclad Alliance | Lunara | Fire, repair |
| Lunara's Covenant | Moon Goddess | Verdantpeak | Sylvan Concord | Cogsmith | Healing, nature |
| Surya's Sandscript | Sun God | Desert | League of Sands | Oceana | Speed, revelation |
| Oceana's Covenant | Sea Mother | Underwater | Maritime Sovereignty | Surya | Waves, summoning |
| Celestis Arcanum | Celestial Order | Crystal Barrens | Arcane Coalition | Frosthelm | Magic, insight |
| Frosthelm Faith | Frost Father | Frosthold | Polar Alliance | Celestis | Endurance, defense |

## Appendix B: Piety Quick Reference

| Piety | Tier | Key Unlocks |
|-------|------|-------------|
| 0 | Faithless | Nothing |
| 1-49 | Initiate | Prayer, shrine access |
| 50-199 | Adherent | Passive tier 1 |
| 200-499 | Devoted | Power 1, passive tier 2 |
| 500-899 | Zealot | Power 2, blessed crafting |
| 900-1000 | Champion | Power 3, divine blessings |

## Appendix C: Crafting Quick Reference

| Discipline | Primary | Secondary | Key Products |
|------------|---------|-----------|--------------|
| Transmutation | Alchemist | Druid | Resource potions, mutagens |
| Engineering | Artificer | Monk | Constructs, gadgets |
| Runecrafting | Enchanter | Wizard | Enchantments, runes |
| Inscription | Oracle | Bard | Scrolls, spellbooks |
| Smithing | Fighter | Knight | Weapons, plate armor |
| Leathercraft | Ranger | Bounty Hunter | Leather armor, quivers |
| Woodshaping | Druid | Shaman | Staves, totems, bows |
| Clothcraft | Bard | Illusionist | Robes, cloaks |
| Necrocraft | Necromancer | Warlock | Bone items, soul vessels |
| Jewelcraft | Sorcerer | Ice Mage | Jewelry, foci |  
**Consolidates:** Master Inventory v4.0, Part VIII Skills, Religion System v1.0, Economy v1.0, Part VIII Additions  
**Status:** Implementation Ready  
**Date:** January 2026


---

**Document Version:** 5.1 (Skill Names Updated)  
**Consolidates:** Master Inventory v4.0, Part VIII Skills, Religion System v1.0, Economy v1.0  
**Status:** Implementation Ready  
**Date:** January 2026
