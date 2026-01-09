# Complete Class Testing Guide

**Purpose:** Systematic testing guide for all 26 classes, covering every spell, ability, and stance.

**Last Updated:** 2025-01-02

---

## Class Starting Stats (Updated 2025-01-02)

Each class now has differentiated starting stats based on their role:

### Caster Classes
| Class | STR | DEX | INT | Role |
|-------|-----|-----|-----|------|
| Ice Mage | 15 | 20 | 45 | Caster DPS |
| Sorcerer | 15 | 20 | 45 | Caster DPS |
| Warlock | 15 | 20 | 45 | Caster DPS |
| Necromancer | 18 | 17 | 45 | Pet/Caster |
| Summoner | 18 | 17 | 45 | Pet/Caster |
| Illusionist | 15 | 23 | 42 | Caster CC |
| Wizard | 15 | 20 | 45 | Multi-school |
| Oracle | 15 | 22 | 43 | Utility Caster |
| Witch | 18 | 22 | 40 | Debuff Caster |

### Healer/Support Classes
| Class | STR | DEX | INT | Role |
|-------|-----|-----|-----|------|
| Cleric | 22 | 20 | 38 | Healer |
| Druid | 20 | 25 | 35 | Healer/Hybrid |
| Shaman | 23 | 22 | 35 | Healer/Hybrid |
| Paladin | 35 | 20 | 25 | Tank/Healer |
| Bard | 18 | 32 | 30 | Support/CC |
| Alchemist | 22 | 28 | 30 | Support Balanced |
| Enchanter | 20 | 25 | 35 | Utility/Buff |

### Melee/Tank Classes
| Class | STR | DEX | INT | Role |
|-------|-----|-----|-----|------|
| Barbarian | 45 | 20 | 15 | Melee DPS |
| Fighter | 40 | 25 | 15 | Tank |
| Knight | 38 | 27 | 15 | Tank |
| Templar | 40 | 20 | 20 | Tank/DPS |
| Monk | 30 | 35 | 15 | DEX Melee |

### Ranged/Pet Classes
| Class | STR | DEX | INT | Role |
|-------|-----|-----|-----|------|
| Ranger | 25 | 45 | 10 | Ranged DPS |
| Beastmaster | 25 | 40 | 15 | Pet/Ranged |
| Bounty Hunter | 28 | 38 | 14 | Ranged/Melee DPS |
| Artificer | 27 | 28 | 25 | Balanced Crafter |

---

## Ability Cost Scaling (Updated 2025-01-02)

All abilities now auto-scale costs based on their circle:

### Mana Costs by Circle
| Circle | Min | Max | Stamina |
|--------|-----|-----|---------|
| 1 | 4 | 6 | 3-5 |
| 2 | 8 | 10 | 6-8 |
| 3 | 12 | 15 | 9-12 |
| 4 | 18 | 22 | 14-18 |
| 5 | 25 | 30 | 20-24 |
| 6 | 35 | 42 | 28-34 |
| 7 | 48 | 55 | 38-44 |
| 8 | 60 | 75 | 48-60 |

### Cooldowns by Circle
| Circle | Min (sec) | Max (sec) | Cast Time |
|--------|-----------|-----------|-----------|
| 1 | 0.0 | 1.0 | Instant |
| 2 | 1.0 | 2.0 | 0.5s |
| 3 | 2.0 | 4.0 | 1.0s |
| 4 | 4.0 | 6.0 | 1.5s |
| 5 | 6.0 | 10.0 | 2.0s |
| 6 | 10.0 | 15.0 | 2.25s |
| 7 | 15.0 | 25.0 | 2.5s |
| 8 | 30.0 | 60.0 | 3.0s |

---

## Quick Setup

### Initial Setup
1. Log in as GM/Admin
2. Type `[VA` to open Vystia Admin Gump
3. Go to **Classes** tab
4. Click **"M"** (Maxed) next to the class you want to test
5. This gives you:
   - Class assigned with all abilities unlocked
   - Skills set to 100
   - Spellbook (magic classes) with all 32 spells
   - Legendary weapon and armor
   - Full resources (Fury, Chi, etc.)
   - Potions

### Spawn Test Target
```
[VA → Creatures tab → Spawn Practice Target
```
Or use `[PT` / `[PracticeTarget` to spawn a red invulnerable practice dummy.

### Resource Commands
```
[SetResource <type> <value>  - Set resource (Fury, Chi, etc.)
[GetResources                - Show all resources
[ResetResources              - Reset all to 0
```

### Stance Commands
```
[SetStance <stance>          - Activate stance
[RemoveStance                - Deactivate stance
[ListStances                 - List all stances
[StanceInfo                  - Current stance details
```

---

## Testing Checklist Format

For each spell/ability, test:
- [ ] **Casting/Activation** - Does it cast/activate?
- [ ] **Visual Effects** - Do particles/animations appear?
- [ ] **Sound Effects** - Does sound play?
- [ ] **Targeting** - Does targeting work correctly?
- [ ] **Damage/Effect** - Does it deal damage or apply effect?
- [ ] **Duration** - Does buff/debuff last correct duration?
- [ ] **Mana/Resource Cost** - Is cost deducted correctly?
- [ ] **Cooldown** - Does cooldown apply correctly?

---

## Magic Classes (12)

### 1. Ice Mage
**Resource:** Chill | **Skill:** Cryomancy | **Spell IDs:** 1000-1031

#### Circle 1 (4 mana)
- [ ] **1000 - Frost Touch** - Melee range, 2s paralyze, ice blue effect
- [ ] **1001 - Ice Shard** - 5-10 cold damage projectile
- [ ] **1002 - Frost Ward** - +15 Cold Resist buff
- [ ] **1003 - Avalanche** - 8-tile cone AoE, 30-50 damage, 3-tile knockback, -20 DEX slow 5s

#### Circle 2 (6 mana)
- [ ] **1004 - Freezing Grasp** - -20 DEX debuff, paralyze field animation
- [ ] **1005 - Ice Shield** - 15% chance to reflect physical damage as cold
- [ ] **1006 - Frost Slick** - Ground AoE, slows movement
- [ ] **1007 - Glacial Mend** - Self heal (cold-themed)

#### Circle 3 (9 mana)
- [ ] **1008 - Ice Bolt** - Energy bolt animation in ice blue, cold damage
- [ ] **1009 - Frostbite** - Cold DoT, damages HP and Stam, blocks healing
- [ ] **1010 - Frozen Ground** - Ground AoE, slows all enemies in area
- [ ] **1011 - Ice Spear** - Line damage spell, pierces multiple targets

#### Circle 4 (11 mana)
- [ ] **1012 - Frost Armor** - +15 Physical Resist, +20 Cold Resist, temporary plate armor
- [ ] **1013 - Ice Wall** - Creates 5x5 ice wall barrier, blocks line of sight
- [ ] **1014 - Icicle Barrage** - Multiple projectile attack, several ice bolts
- [ ] **1015 - Permafrost** - Area freeze effect, slows and damages over time

#### Circle 5 (14 mana)
- [ ] **1016 - Glacial Strike** - High damage single target cold strike
- [ ] **1017 - Frozen Tomb** - Emergency invulnerability, self-freeze 5s
- [ ] **1018 - Shatter** - Bonus damage to frozen targets, AoE
- [ ] **1019 - Hypothermia** - -30 DEX, -20% attack speed debuff

#### Circle 6 (20 mana)
- [ ] **1020 - Blizzard** - 5-tile radius, 3-8 cold damage/tick for 10s, slows
- [ ] **1021 - Glacial Fortress** - +25 all resistances buff
- [ ] **1022 - Deep Freeze** - 5s stun + cold vulnerability debuff
- [ ] **1023 - Chill Aura** - 2-tile aura, freezes on entry, drains DEX

#### Circle 7 (40 mana)
- [ ] **1024 - Absolute Zero** - 6-tile radius AoE, 50-80 damage, 3s freeze, frozen ground
- [ ] **1025 - Glacier Summon** - Summons Ice Elemental (700 HP, 20-30 damage, 180s)
- [ ] **1026 - Eternal Winter** - Long duration AoE cold DoT field
- [ ] **1027 - Fimbulwinter's Wrath** - Massive cold damage nuke

#### Circle 8 (50 mana)
- [ ] **1028 - Frost Meteor** - Meteor strike with ice theme, massive AoE
- [ ] **1029 - Ice Age** - Screen-wide freeze, all enemies frozen
- [ ] **1030 - Rime Reaper** - Execute: high damage to low HP targets
- [ ] **1031 - Cocytus Prison** - Channel: target frozen+invulnerable, caster also frozen, 60s max

---

### 2. Druid
**Resource:** Nature | **Skill:** Druidism | **Spell IDs:** 1032-1063

#### Shapeshifting Forms
- [ ] **Bear Form** (Circle 3) - Tank: +30 STR, +15 Physical Resist, bonus melee damage, CANNOT CAST
- [ ] **Wolf Form** (Circle 4) - DPS: +25 DEX, +30% speed, +15% attack speed, bleed attacks, CANNOT CAST
- [ ] **Hawk Form** (Circle 5) - Scout: fly over obstacles, +40 DEX, -50% ranged damage taken, CANNOT CAST
- [ ] **Treant Form** (Circle 6) - Ultimate Tank: +50 STR, +30 Physical Resist, +25% max HP, AoE root, CANNOT CAST
- [ ] **Hydra Form** (Circle 8) - Legendary: +70 STR, +30 DEX, triple attack, 15 HP/tick regen, poison immune

#### Circle 1 (4 mana)
- [ ] **1032 - Nature's Touch** - Heal target 8-15 HP
- [ ] **1033 - Thorn Dart** - 4-10 damage (50% poison, 50% physical)
- [ ] **1034 - Barkskin** - +5 Physical Resist, +5 Poison Resist buff
- [ ] **1035 - Detect Life** - Reveals hidden creatures within 12 tiles, shows HP bars

#### Circle 2 (6 mana)
- [ ] **1036 - Entangle** - Root target (cannot move, can still cast/attack)
- [ ] **1037 - Poison Spores** - DoT: 3-5 poison damage/tick (6 ticks)
- [ ] **1038 - Rejuvenation** - HoT: 3-6 HP/tick (10 ticks = 30-60 total)
- [ ] **1039 - Animal Aspect: Speed** - +20 DEX, +15% movement speed buff

#### Circle 3 (9 mana)
- [ ] **1040 - Wild Growth** - Creates vegetation, blocks line of sight, slows enemies 30%
- [ ] **1041 - Bear Form** - Shapeshift: +30 STR, +15 Physical Resist, melee bonus, CANNOT CAST
- [ ] **1042 - Thorn Volley** - AoE 12-22 damage (50% poison, 50% physical)
- [ ] **1043 - Nature's Blessing** - +10% max HP, +5 HP regen/tick, +10 Poison Resist

#### Circle 4 (11 mana)
- [ ] **1044 - Wolf Form** - Shapeshift: +25 DEX, +30% speed, +15% attack speed, bleed, CANNOT CAST
- [ ] **1045 - Strangling Vines** - Root + 6-10 damage/tick DoT, -20% attack speed debuff
- [ ] **1046 - Healing Grove** - AoE HoT: allies heal 5-8 HP/tick
- [ ] **1047 - Toxic Bloom** - 15-25 poison damage + poison DoT (5 damage/tick for 10s)

#### Circle 5 (14 mana)
- [ ] **1048 - Hawk Form** - Shapeshift: fly, +40 DEX, -50% ranged damage taken, weak melee
- [ ] **1049 - Earthquake** - AoE 20-35 physical damage, knockdown (2s stun), destroys ice walls
- [ ] **1050 - Greater Regeneration** - HoT: 8-12 HP/tick (15 ticks = 120-180 total)
- [ ] **1051 - Spore Cloud** - AoE poison cloud 5-9 damage/tick, reduces visibility, -25% accuracy

#### Circle 6 (20 mana)
- [ ] **1052 - Treant Form** - Shapeshift: +50 STR, +30 Phys Resist, +25% max HP, AoE root attacks
- [ ] **1053 - Swarm** - Insect swarm 6-10 damage/tick, -40% accuracy, interrupts spellcasting
- [ ] **1054 - Living Fortress** - +20 Phys/Poison Resist, +15% HP, root/slow immune, 5 HP/tick
- [ ] **1055 - Nature's Wrath** - 35-55 poison/physical damage, poison DoT, roots all targets

#### Circle 7 (40 mana)
- [ ] **1056 - Force of Nature** - Gain all shapeshift benefits simultaneously, CAN still cast spells
- [ ] **1057 - Summon Ancient Treant** - Summon: 1200 HP, powerful melee, AoE root, high resists
- [ ] **1058 - Plague** - Devastating DoT: 10-15 damage/tick, spreads to enemies within 3 tiles
- [ ] **1059 - Primordial Restoration** - AoE heal 80-120 HP, removes poisons/curses, 15s debuff immunity

#### Circle 8 (50 mana)
- [ ] **1060 - World Tree's Embrace** - Allies: +50% HP, 10 HP/tick, +25 resists. Enemies: 8-15 DoT, 75% slow
- [ ] **1061 - Hydra Form** - Legendary Shapeshift: +70 STR, +30 DEX, triple attack, 15 HP/tick, poison immune
- [ ] **1062 - Thorn Apocalypse** - 60-100 damage (poison/physical), bleed + poison DoTs, 5s root all
- [ ] **1063 - Avatar of the Forest** - Nature incarnate: instant shapeshift+cast, 50% mana, +40 resists, 3 treants

---

### 3. Witch
**Resource:** Hex | **Skill:** Hexcraft | **Spell IDs:** 1064-1095

#### Circle 1 (4 mana)
- [ ] **1064 - Evil Eye** - -5% accuracy debuff, 60s duration
- [ ] **1065 - Weak Curse** - -5 to all stats (STR/DEX/INT), 60s duration
- [ ] **1066 - Siphon Life** - Drains 8-12 HP from target, heals caster
- [ ] **1067 - Witch Sight** - Reveals hidden creatures, shows HP bars

#### Circle 2 (6 mana)
- [ ] **1068 - Wasting Curse** - Target loses 1% max HP every 3s (max 10% loss), DoT
- [ ] **1069 - Poison Touch** - Applies Greater Poison (5-8 poison damage/tick)
- [ ] **1070 - Enfeeble** - -15 STR debuff, 120s duration
- [ ] **1071 - Dark Pact** - Sacrifice HP to gain mana/resources buff

#### Circle 3 (9 mana)
- [ ] **1072 - Contagious Hex** - Cursed target spreads hex to allies within 3 tiles (-10% attack speed, 3-5 damage/tick)
- [ ] **1073 - Life Leech** - Drains 15-25 HP, heals caster for 150% of amount drained
- [ ] **1074 - Hex of Frailty** - -10 to all resistances, 180s duration
- [ ] **1075 - Voodoo Doll** - Links to target: 25% of damage caster takes is reflected to linked target

#### Circle 4 (11 mana)
- [ ] **1076 - Crippling Curse** - -30 DEX, -20% movement speed, 180s duration
- [ ] **1077 - Plague Bearer** - Target becomes plague carrier: spreads disease (4-7 damage/tick) to nearby allies
- [ ] **1078 - Drain Essence** - Drains 20-35 HP + 10-20 mana, heals/restores caster
- [ ] **1079 - Hex of Agony** - 6-10 damage/tick DoT, reduces healing by 25%, 240s duration

#### Circle 5 (14 mana)
- [ ] **1080 - Mass Hex** - All enemies in 5 tiles: -10 all stats, -10% damage, -5 all resists
- [ ] **1081 - Soul Siphon** - Drains 30-50 HP + 20-35 mana, heals/restores caster
- [ ] **1082 - Hex of Silence** - Silences target (cannot cast spells), 8s duration
- [ ] **1083 - Necrotic Touch** - 25-40 necrotic damage, applies Deadly Poison

#### Circle 6 (20 mana)
- [ ] **1084 - Curse of Mortality** - Removes ALL regeneration (HP/Mana/Stam), -20 all resists, -15% max HP
- [ ] **1085 - Hex Storm** - AoE: Multiple hexes hit all enemies (8-15 damage, various debuffs)
- [ ] **1086 - Vampiric Aura** - Aura: All damage dealt heals caster for 35%, allies heal 15%
- [ ] **1087 - Doomcurse** - After 12s, target takes 100-150 damage unless dispelled (cannot be removed by normal means)

#### Circle 7 (40 mana)
- [ ] **1088 - Plague of Sorrows** - AoE plague: 10-18 damage/tick, spreads between enemies, -30% healing received
- [ ] **1089 - Soul Harvest** - Drains 60-100 HP + 40-70 mana from all nearby enemies, heals/restores caster
- [ ] **1090 - Curse of the Hag** - -30 all stats, -25 all resists, -50% healing, -25% damage, transforms appearance
- [ ] **1091 - Hexblade Ritual** - Weapon enchant: All weapon hits apply hex effects (drain life, debuffs, poison)

#### Circle 8 (50 mana)
- [ ] **1092 - Curse of Undeath** - Target killed while cursed rises as hostile undead under witch's control
- [ ] **1093 - Voodoo Mastery** - Links to all enemies: 50% of damage caster takes reflected, caster immune to damage reflection
- [ ] **1094 - Apocalyptic Hex** - ALL enemies: -20 all stats, -20 all resists, Deadly Poison, -50% healing, 8-15 damage/tick
- [ ] **1095 - Witch Queen's Dominion** - Ultimate transformation: +40 INT, all hexes cost 50% less, all hex durations doubled, immune to curses

---

### 4. Sorcerer
**Resource:** Heat | **Skill:** Elementalism | **Spell IDs:** 1096-1127

#### Stances (Elemental Attunement)
- [ ] **Fire Stance** - Fire spells enhanced, +fire damage
- [ ] **Water Stance** - Water spells enhanced, +cold damage, healing
- [ ] **Earth Stance** - Earth spells enhanced, +armor, slow but strong
- [ ] **Air Stance** - Air spells enhanced, +speed, +evasion

#### Circle 1 (4 mana)
- [ ] **1096 - Flame Dart** - Fire damage
- [ ] **1097 - Stone Skin** - Armor buff
- [ ] **1098 - Gust** - Air damage
- [ ] **1099 - Water Bolt** - Water damage

#### Circle 2 (6 mana)
- [ ] **1100 - Fireball** - Fire AoE
- [ ] **1101 - Earth Shield** - Defense buff
- [ ] **1102 - Wind Slash** - Air damage
- [ ] **1103 - Tidal Wave** - Water AoE

#### Circle 3 (9 mana)
- [ ] **1104 - Flame Wave** - Fire line damage
- [ ] **1105 - Tremor** - Earth AoE
- [ ] **1106 - Cyclone** - Air AoE
- [ ] **1107 - Flood** - Water AoE

#### Circle 4 (11 mana)
- [ ] **1108 - Inferno** - Fire AoE
- [ ] **1109 - Stone Wall** - Earth barrier
- [ ] **1110 - Tornado** - Air AoE
- [ ] **1111 - Maelstrom** - Water AoE

#### Circle 5 (14 mana)
- [ ] **1112 - Meteor** - Fire meteor strike
- [ ] **1113 - Earthquake** - Earth AoE
- [ ] **1114 - Hurricane** - Air AoE
- [ ] **1115 - Tsunami** - Water AoE

#### Circle 6 (20 mana)
- [ ] **1116 - Firestorm** - Fire AoE
- [ ] **1117 - Lava Burst** - Fire/earth damage
- [ ] **1118 - Tempest** - Air storm
- [ ] **1119 - Deluge** - Water flood

#### Circle 7 (40 mana)
- [ ] **1120 - Phoenix Fire** - Fire rebirth
- [ ] **1121 - Volcanic Eruption** - Fire/earth AoE
- [ ] **1122 - Storm of Ages** - Air ultimate
- [ ] **1123 - Elemental Fury** - All elements

#### Circle 8 (50 mana)
- [ ] **1124 - Ragnarok** - Fire apocalypse
- [ ] **1125 - Worldbreaker** - Earth ultimate
- [ ] **1126 - Primordial Storm** - Air ultimate
- [ ] **1127 - Elemental Apocalypse** - All elements ultimate

---

### 5. Warlock
**Resource:** Shadow | **Skill:** Demonology | **Spell IDs:** 1128-1159

#### Circle 1 (4 mana)
- [ ] **1128 - Shadow Bolt** - Single target shadow damage 5-10 + skill bonus, shadow particle effect
- [ ] **1129 - Life Tap** - Beneficial buff, +5 STR + skill bonus for 1 minute, empowers caster
- [ ] **1130 - Minor Fear** - Harmful fear effect on single target, causes target to flee
- [ ] **1131 - Demonic Sight** - Neutral utility spell, reveals hidden creatures and grants enhanced vision

#### Circle 2 (6 mana)
- [ ] **1132 - Chaos Bolt** - Single target chaos damage, unpredictable shadow magic
- [ ] **1133 - Shadow Step** - Neutral teleport, blink short distance through shadows
- [ ] **1134 - Drain Soul** - Harmful soul drain, damages target and restores caster resources
- [ ] **1135 - Lesser Demon** - Summon lesser demon (10 min duration, 2 follower slots)

#### Circle 3 (9 mana)
- [ ] **1136 - Shadow Nova** - AoE shadow explosion 15-20 damage + skill bonus, nova particle effect
- [ ] **1137 - Demonic Pact** - Beneficial buff, +10 STR + skill bonus for 3 minutes
- [ ] **1138 - Fear Wave** - Harmful AoE fear, multiple targets flee in terror
- [ ] **1139 - Corruption** - Harmful DoT, shadow corruption damages over time

#### Circle 4 (11 mana)
- [ ] **1140 - Summon Voidwalker** - Summon voidwalker tank pet (10 min duration, 2 follower slots)
- [ ] **1141 - Shadow Chains** - Harmful immobilize, roots target with shadow chains for 4 minutes
- [ ] **1142 - Soul Burn** - Harmful burst damage, burns target's soul for heavy shadow damage
- [ ] **1143 - Chaos Rift** - Neutral ground target, creates unstable chaos portal

#### Circle 5 (14 mana)
- [ ] **1144 - Fel Armor** - Beneficial self-buff, +15 STR + skill bonus for 5 minutes, fel energy armor
- [ ] **1145 - Mass Fear** - Harmful AoE fear, terrorizes all nearby enemies
- [ ] **1146 - Demonic Sacrifice** - Beneficial sacrifice buff, sacrifice demon for temporary power
- [ ] **1147 - Shadow Orb** - Neutral shadow orb, floating orb that attacks enemies

#### Circle 6 (20 mana)
- [ ] **1148 - Summon Succubus** - Summon succubus (10 min duration, 2 follower slots), seduction abilities
- [ ] **1149 - Dark Portal** - Neutral teleportation portal, creates gateway to another location
- [ ] **1150 - Chaos Storm** - Harmful AoE chaos storm, swirling vortex of unpredictable damage
- [ ] **1151 - Shadowform** - Beneficial transformation, become shadow (increased evasion, shadow damage)

#### Circle 7 (40 mana)
- [ ] **1152 - Summon Pit Lord** - Summon powerful pit lord demon (10 min duration, 2 follower slots)
- [ ] **1153 - Soul Harvest** - Harmful soul drain, drains multiple targets' souls for massive damage
- [ ] **1154 - Apocalyptic Chaos** - Harmful ultimate AoE, apocalyptic chaos explosion
- [ ] **1155 - Demonic Ascension** - Beneficial ultimate transformation, +22 STR + skill bonus for 10 minutes

#### Circle 8 (50 mana)
- [ ] **1156 - Summon Demon Prince** - Summon legendary demon prince (10 min duration, 2 follower slots)
- [ ] **1157 - Void Collapse** - Harmful ultimate void spell, collapses reality causing massive damage
- [ ] **1158 - Chaos Incarnate** - Beneficial ultimate buff, become living chaos incarnation
- [ ] **1159 - Dark Apotheosis** - Beneficial ultimate ascension, +25 STR + skill bonus for 15 minutes, become demigod

---

### 6. Oracle
**Resource:** Foresight | **Skill:** Divination | **Spell IDs:** 1160-1191

#### Circle 1 (4 mana)
- [ ] **1160 - Minor Insight** - Buff spell
- [ ] **1161 - Crystal Gaze** - Reveal spell
- [ ] **1162 - Time Skip** - Movement spell
- [ ] **1163 - Fate's Touch** - Buff/debuff

#### Circle 2 (6 mana)
- [ ] **1164 - Foresight** - Prediction buff
- [ ] **1165 - Scrying** - Reveal target
- [ ] **1166 - Temporal Shift** - Movement
- [ ] **1167 - Fortune** - Luck buff

#### Circle 3 (9 mana)
- [ ] **1168 - Precognition** - See future
- [ ] **1169 - Crystal Shield** - Defense buff
- [ ] **1170 - Haste** - Speed buff
- [ ] **1171 - Destiny Bond** - Link spell

#### Circle 4 (11 mana)
- [ ] **1172 - Future Sight** - Prediction
- [ ] **1173 - Mirror Image** - Clone spell
- [ ] **1174 - Time Warp** - Movement
- [ ] **1175 - Fate Weave** - Control spell

#### Circle 5 (14 mana)
- [ ] **1176 - Prophecy** - Prediction buff
- [ ] **1177 - Crystal Storm** - Damage spell
- [ ] **1178 - Slow Time** - Slow debuff
- [ ] **1179 - Destiny's Call** - Control spell

#### Circle 6 (20 mana)
- [ ] **1180 - True Sight** - Reveal all
- [ ] **1181 - Prismatic Barrier** - Defense
- [ ] **1182 - Time Stop** - Ultimate CC
- [ ] **1183 - Fate's Judgment** - Damage spell

#### Circle 7 (40 mana)
- [ ] **1184 - Oracle's Vision** - Ultimate prediction
- [ ] **1185 - Crystal Apocalypse** - Damage
- [ ] **1186 - Temporal Mastery** - Time control
- [ ] **1187 - Weave Destiny** - Ultimate control

#### Circle 8 (50 mana)
- [ ] **1188 - Omniscience** - Ultimate knowledge
- [ ] **1189 - Prismatic Annihilation** - Damage
- [ ] **1190 - Time Rewind** - Reversal spell
- [ ] **1191 - Fate's End** - Ultimate fate

---

### 7. Necromancer
**Resource:** Soul | **Skill:** NecromancyArts | **Spell IDs:** 1192-1223

#### Circle 1 (4 mana)
- [ ] **1192 - Death Bolt** - Direct damage projectile, 5-10 damage + skill scaling, death energy
- [ ] **1193 - Animate Bone** - Raises basic bone creature, +5 STR buff for 1 minute
- [ ] **1194 - Life Siphon** - Drains health from target, heals caster for damage dealt
- [ ] **1195 - Deathsight** - Grants vision of death energy, +5 STR buff for 1 minute, reveals undead

#### Circle 2 (6 mana)
- [ ] **1196 - Bone Shard** - Sharp bone projectile, 10-18 physical damage, armor penetration
- [ ] **1197 - Decay** - Withers target, reduces stats and applies DoT
- [ ] **1198 - Raise Zombie** - Summons zombie minion, melee fighter with poison attacks
- [ ] **1199 - Soul Shield** - Protective barrier, absorbs incoming damage before HP

#### Circle 3 (9 mana)
- [ ] **1200 - Death Coil** - Dual-purpose spell: damages enemies or heals undead allies
- [ ] **1201 - Bone Armor** - Creates bone armor on target, +8 physical resistance, +5 STR
- [ ] **1202 - Mass Raise** - Raises multiple undead from nearby corpses
- [ ] **1203 - Soul Harvest** - Drains soul energy from target, restores mana to caster

#### Circle 4 (11 mana)
- [ ] **1204 - Skeletal Mage** - Summons skeleton caster, uses ranged magic attacks
- [ ] **1205 - Corpse Explosion** - Detonates corpse dealing AoE damage, 15-25 physical/poison
- [ ] **1206 - Death Grip** - Pulls target toward caster, roots them briefly
- [ ] **1207 - Vampiric Touch** - Melee range life drain, high healing and damage

#### Circle 5 (14 mana)
- [ ] **1208 - Bone Wall** - Creates wall of bones blocking movement, +15 STR buff 5 minutes
- [ ] **1209 - Death and Decay** - Ground AoE dealing damage over time to all in area
- [ ] **1210 - Raise Bone Golem** - Summons powerful bone construct, high HP tank
- [ ] **1211 - Soul Link** - Links caster to ally, shares damage and healing

#### Circle 6 (20 mana)
- [ ] **1212 - Death Knights** - Summons elite death knight warriors with heavy armor
- [ ] **1213 - Plague Cloud** - AoE disease cloud, spreads between enemies, DoT + debuff
- [ ] **1214 - Unholy Frenzy** - Buffs ally with attack speed and damage, drains HP over time
- [ ] **1215 - Lich Form** - Transforms caster into lich, +18 STR for 7 minutes, enhanced casting

#### Circle 7 (40 mana)
- [ ] **1216 - Army of the Dead** - Summons large group of undead minions, +22 STR 10 minutes
- [ ] **1217 - Death Wave** - Wave of death energy hitting all enemies in front, massive damage
- [ ] **1218 - Bone Prison** - Traps enemy in cage of bones, immobilized and vulnerable
- [ ] **1219 - Demi-Lich Transformation** - Advanced lich form, floating skull, greatly enhanced magic

#### Circle 8 (50 mana)
- [ ] **1220 - Apocalypse of Death** - Screen-wide death wave, devastating AoE damage
- [ ] **1221 - Summon Undead Dragon** - Ultimate summon: skeletal dragon with breath attacks
- [ ] **1222 - Death's Door** - Sends target to brink of death, massive damage + death mark
- [ ] **1223 - Archlich Ascension** - Ultimate transformation, +25 STR 15 minutes, peak power

---

### 8. Summoner
**Resource:** Essence | **Skill:** Conjuration | **Spell IDs:** 1224-1255

#### Circle 1 (4 mana)
- [ ] **1224 - Summon Rabbit** - Summons a weak Rabbit companion (10 min duration, 2 follower slots)
- [ ] **1225 - Arcane Bolt** - Launches an arcane projectile dealing 4-10 damage to target
- [ ] **1226 - Empower Summon** - Buffs target summoned creature with bonus stats for 3 minutes
- [ ] **1227 - Summon Wisp** - Summons a Wisp companion with magical abilities (10 min duration, 2 follower slots)

#### Circle 2 (6 mana)
- [ ] **1228 - Summon Wolf** - Summons a Wolf companion for combat (10 min duration, 2 follower slots)
- [ ] **1229 - Summon Fire Sprite** - Summons a Fire Sprite with fire damage abilities (10 min duration, 2 follower slots)
- [ ] **1230 - Mend Summon** - Heals target summoned creature
- [ ] **1231 - Summon Shield** - Summons an Earth Elemental for defensive purposes (10 min duration, 2 follower slots)

#### Circle 3 (9 mana)
- [ ] **1232 - Summon Bear** - Summons a powerful Bear companion (10 min duration, 2 follower slots)
- [ ] **1233 - Summon Air Elemental** - Summons an Air Elemental with wind-based attacks (10 min duration, 2 follower slots)
- [ ] **1234 - Mass Empower** - Buffs multiple summoned creatures with enhanced stats
- [ ] **1235 - Bind Beast** - Strengthens bond with target creature, granting +10 STR bonus for 3 minutes

#### Circle 4 (11 mana)
- [ ] **1236 - Summon Drake** - Summons a Drake with breath weapon attacks (10 min duration, 2 follower slots)
- [ ] **1237 - Summon Earth Elemental** - Summons a tanky Earth Elemental (10 min duration, 2 follower slots)
- [ ] **1238 - Summon Frenzy** - Enrages summoned creatures, increasing their combat effectiveness
- [ ] **1239 - Unsummon** - Dismisses target summoned creature instantly

#### Circle 5 (14 mana)
- [ ] **1240 - Summon Hydra** - Summons a multi-headed Hydra with powerful attacks (10 min duration, 2 follower slots)
- [ ] **1241 - Summon Storm Elemental** - Summons a Storm Elemental with lightning abilities (10 min duration, 2 follower slots)
- [ ] **1242 - Greater Heal Summon** - Restores large amount of HP to target summoned creature
- [ ] **1243 - Symbiotic Link** - Creates magical link with summon, granting +15 STR and shared bonuses for 5 minutes

#### Circle 6 (20 mana)
- [ ] **1244 - Summon Phoenix** - Summons a Phoenix with fire rebirth abilities (10 min duration, 2 follower slots)
- [ ] **1245 - Summon Void Elemental** - Summons a Void Elemental with dark void powers (10 min duration, 2 follower slots)
- [ ] **1246 - Army of Beasts** - Summons multiple creatures at once, granting +18 STR buff for 7 minutes
- [ ] **1247 - Mass Heal Summons** - Heals all summoned creatures in area

#### Circle 7 (40 mana)
- [ ] **1248 - Summon Greater Dragon** - Summons a powerful Greater Dragon (10 min duration, 2 follower slots)
- [ ] **1249 - Summon Elemental Lord** - Summons an Elemental Lord with devastating powers (10 min duration, 2 follower slots)
- [ ] **1250 - Sacrifice Summon** - Sacrifices summoned creature for burst damage to target
- [ ] **1251 - Swarm of Creatures** - Summons multiple weak creatures to overwhelm enemies

#### Circle 8 (50 mana)
- [ ] **1252 - Summon Titan** - Summons an epic Titan with immense power (10 min duration, 2 follower slots)
- [ ] **1253 - Planar Convergence** - Opens planar rift, summoning multiple powerful creatures
- [ ] **1254 - Summoner's Apocalypse** - Ultimate destructive spell channeling power of all summons
- [ ] **1255 - Avatar of Summoning** - Transforms summoner into avatar form with enhanced summoning abilities

---

### 9. Shaman
**Resource:** Spirit | **Skill:** SpiritCalling | **Spell IDs:** 1256-1287

#### Circle 1 (4 mana)
- [ ] **1256 - Lightning Bolt** - Direct lightning damage 5-10 + (Conjuration×0.5), single target energy damage
- [ ] **1257 - Strength Totem** - Buff target +5 STR (+Conjuration×0.2) for 1 minute, empowerment effect
- [ ] **1258 - Ghost Wolf Form** - Transform into ghost wolf form, gain +5 STR buff for 1 minute
- [ ] **1259 - Healing Stream** - Heal target for small amount, beneficial healing effect

#### Circle 2 (6 mana)
- [ ] **1260 - Chain Lightning** - Lightning chains to nearby enemies, 12-20 energy damage
- [ ] **1261 - Fire Totem** - Place fire totem that damages enemies in area
- [ ] **1262 - Spirit Strike** - Spirit-infused melee strike, direct spiritual damage
- [ ] **1263 - Purification** - Remove debuffs and cleanse negative effects from target

#### Circle 3 (9 mana)
- [ ] **1264 - Lightning Storm** - AoE lightning damage over area, sustained energy damage
- [ ] **1265 - Earth Shield** - Grant target +10 armor buff (+Conjuration×0.2) for 3 minutes
- [ ] **1266 - Totemic Recall** - Teleport to placed totem location instantly
- [ ] **1267 - Summon Spirit Wolf** - Summon Grey Wolf companion for 10 minutes (2 follower slots)

#### Circle 4 (11 mana)
- [ ] **1268 - Chain Heal** - Healing chains to nearby allies, restores HP to multiple targets
- [ ] **1269 - Mana Spring Totem** - Place totem that restores mana to nearby allies over time
- [ ] **1270 - Lava Burst** - Fire/earth burst damage 10-18, combines elemental forces
- [ ] **1271 - Flame Shock** - Apply fire DoT effect, burning damage over time

#### Circle 5 (14 mana)
- [ ] **1272 - Thunderstorm Totem** - Place powerful storm totem, continuous lightning strikes in area
- [ ] **1273 - Ancestral Spirit** - Summon ancestral guardian, +15 STR buff (+Conjuration×0.2) for 5 minutes
- [ ] **1274 - Earth Elemental** - Summon earth elemental companion to fight alongside caster
- [ ] **1275 - Maelstrom** - Massive AoE storm effect 10-18 damage, swirling vortex of destruction

#### Circle 6 (20 mana)
- [ ] **1276 - Mega Chain Lightning** - Enhanced chain lightning, multiple targets with high energy damage
- [ ] **1277 - Totem of Wrath** - Place totem granting damage bonus to all allies in range
- [ ] **1278 - Spirit Link Totem** - Link allies' health pools, share damage taken across linked targets
- [ ] **1279 - Elemental Fury** - Unleash elemental power, massive burst of combined element damage

#### Circle 7 (40 mana)
- [ ] **1280 - Summon Greater Earth Elemental** - Summon powerful earth elemental, tanky companion
- [ ] **1281 - Ancestor's Blessing** - Divine ancestral blessing, powerful stat buffs to all allies
- [ ] **1282 - Four Totems** - Place 4 totems simultaneously (Fire, Earth, Storm, Mana), +22 STR buff for 10 minutes
- [ ] **1283 - Ascendance** - Transform into ascended spirit form, become one with the elements

#### Circle 8 (50 mana)
- [ ] **1284 - Apocalyptic Chain Lightning** - Ultimate chain lightning, devastates all enemies in massive area
- [ ] **1285 - Spirit of the Wild** - Channel primal spirit forces, become avatar of nature's fury
- [ ] **1286 - Totem Army** - Summon massive array of all totem types, overwhelming battlefield control
- [ ] **1287 - Shaman Lord** - Ultimate ascension, +25 STR buff (+Conjuration×0.2) for 15 minutes, master of elements

---

### 10. Bard
**Resource:** Harmony | **Skill:** BardicLore | **Spell IDs:** 1288-1319

#### Circle 1 (4 mana)
- [ ] **1288 - Discordant Note** - Single target harmful sonic spell, applies STR debuff, visual particle effect
- [ ] **1289 - Song of Courage** - Beneficial buff, grants +STR to target, courage-themed golden particle effect
- [ ] **1290 - Lullaby** - Harmful sleep/crowd control spell, calming melody puts target to sleep temporarily
- [ ] **1291 - Inspire Accuracy** - Beneficial buff, grants +accuracy/DEX to target, inspiring musical notes

#### Circle 2 (6 mana)
- [ ] **1292 - Sonic Burst** - Harmful sonic damage spell, AoE burst of sound waves, location-based particle effect
- [ ] **1293 - Song of Resilience** - Beneficial buff, grants +physical resist/defense to target, protective harmonies
- [ ] **1294 - Dirge of Weakness** - Harmful debuff, reduces target's strength and combat effectiveness, mournful dirge
- [ ] **1295 - Rejuvenating Melody** - Beneficial healing spell, restores HP over time with soothing melody

#### Circle 3 (9 mana)
- [ ] **1296 - Thunderous Chord** - Harmful sonic damage, powerful chord deals sonic/thunder damage to single target
- [ ] **1297 - War Song** - Beneficial combat buff, grants +STR and combat bonuses to allies, war drums effect
- [ ] **1298 - Mesmerize** - Harmful crowd control, hypnotic melody confuses/charms target temporarily
- [ ] **1299 - Song of Swiftness** - Beneficial buff, grants +DEX and movement speed to target, fast-paced melody

#### Circle 4 (11 mana)
- [ ] **1300 - Sonic Wave** - Harmful sonic damage, wave of sound spreads in line/cone, multiple targets
- [ ] **1301 - Ballad of Health** - Beneficial healing spell, strong heal with heroic ballad theme
- [ ] **1302 - Cacophony** - Harmful debuff, chaotic discordant sounds reduce enemy accuracy and morale
- [ ] **1303 - Inspire Heroism** - Beneficial buff, grants +all stats and heroic courage to target

#### Circle 5 (14 mana)
- [ ] **1304 - Shattering Scream** - Harmful sonic damage, powerful scream deals heavy sonic damage, armor-piercing
- [ ] **1305 - Song of Sanctuary** - Beneficial protection spell, creates safe zone/reduces damage taken, sanctuary aura
- [ ] **1306 - Requiem** - Harmful death/necrotic spell, funeral dirge damages and weakens undead/living targets
- [ ] **1307 - Mass Inspire** - Beneficial AoE buff, grants inspiration bonuses to all nearby allies

#### Circle 6 (20 mana)
- [ ] **1308 - Destructive Resonance** - Harmful sonic damage, resonating frequencies shatter armor and deal heavy damage
- [ ] **1309 - Epic Ballad** - Beneficial epic-tier buff, grants powerful stat bonuses and combat advantages
- [ ] **1310 - Song of Fear** - Harmful fear/crowd control, terrifying melody causes enemies to flee in fear
- [ ] **1311 - Crescendo** - Harmful stacking damage spell, sonic damage that builds/crescendos with each cast

#### Circle 7 (40 mana)
- [ ] **1312 - Symphony of Destruction** - Harmful ultimate AoE damage, orchestrated destruction deals massive sonic damage
- [ ] **1313 - Song of the Ancients** - Beneficial ultimate buff, ancient powerful melodies grant legendary bonuses
- [ ] **1314 - Sonic Apocalypse** - Harmful cataclysmic damage, apocalyptic sonic wave devastates all enemies in area
- [ ] **1315 - Legendary Performance** - Beneficial ultimate buff, legendary bardic performance grants supreme bonuses

#### Circle 8 (50 mana)
- [ ] **1316 - Ode to Devastation** - Harmful ultimate damage, devastating ode deals catastrophic sonic/physical damage
- [ ] **1317 - Eternal Symphony** - Beneficial ultimate buff, eternal harmonies grant permanent-duration powerful bonuses
- [ ] **1318 - Voice of the Dragon** - Harmful ultimate damage, dragon's roar deals massive sonic damage with fear effect
- [ ] **1319 - Maestro Ascendant** - Beneficial ultimate transformation, ascend to maestro form with supreme bardic powers

---

### 11. Enchanter
**Resource:** Mana | **Skill:** Runeweaving | **Spell IDs:** 1320-1351

#### Circle 1 (4 mana)
- [ ] **1320 - Minor Enchant** - Weapon buff
- [ ] **1321 - Rune of Protection** - Defense rune
- [ ] **1322 - Empower** - Stat buff
- [ ] **1323 - Weaken Armor** - Armor debuff

#### Circle 2 (6 mana)
- [ ] **1324 - Enchant Weapon** - Weapon enhancement
- [ ] **1325 - Rune of Strength** - Strength rune
- [ ] **1326 - Greater Empower** - Strong buff
- [ ] **1327 - Shatter Armor** - Armor break

#### Circle 3 (9 mana)
- [ ] **1328 - Superior Enchant** - Superior weapon
- [ ] **1329 - Rune of Speed** - Speed rune
- [ ] **1330 - Mass Empower** - AoE buff
- [ ] **1331 - Armor Break** - Armor debuff

#### Circle 4 (11 mana)
- [ ] **1332 - Major Enchant** - Major weapon
- [ ] **1333 - Rune of Fortitude** - Fortitude rune
- [ ] **1334 - Aura of Power** - Power aura
- [ ] **1335 - Disenchant** - Remove enchant

#### Circle 5 (14 mana)
- [ ] **1336 - Greater Enchant** - Greater weapon
- [ ] **1337 - Rune of Might** - Might rune
- [ ] **1338 - Mass Aura** - AoE aura
- [ ] **1339 - Nullify** - Dispel spell

#### Circle 6 (20 mana)
- [ ] **1340 - Epic Enchant** - Epic weapon
- [ ] **1341 - Rune of Champions** - Champion rune
- [ ] **1342 - Divine Empowerment** - Divine buff
- [ ] **1343 - Dispel Magic** - Dispel

#### Circle 7 (40 mana)
- [ ] **1344 - Legendary Enchant** - Legendary weapon
- [ ] **1345 - Rune of Legends** - Legend rune
- [ ] **1346 - Godly Empowerment** - Godly buff
- [ ] **1347 - Spell Breaker** - Break spell

#### Circle 8 (50 mana)
- [ ] **1348 - Mythic Enchant** - Mythic weapon
- [ ] **1349 - Rune of Gods** - God rune
- [ ] **1350 - Ascension** - Ultimate buff
- [ ] **1351 - Absolute Nullification** - Ultimate dispel

---

### 12. Illusionist
**Resource:** Mirage | **Skill:** IllusionMagic | **Spell IDs:** 1352-1383

#### Circle 1 (4 mana)
- [ ] **1352 - Minor Illusion** - Create illusion
- [ ] **1353 - Daze** - Stun spell
- [ ] **1354 - Blur** - Evasion buff
- [ ] **1355 - Distract** - Distraction

#### Circle 2 (6 mana)
- [ ] **1356 - Phantom Image** - Create phantom
- [ ] **1357 - Confuse** - Confusion debuff
- [ ] **1358 - Invisibility** - Invisibility spell
- [ ] **1359 - Misdirect** - Misdirection

#### Circle 3 (9 mana)
- [ ] **1360 - Major Illusion** - Major illusion
- [ ] **1361 - Mesmerize** - Charm spell
- [ ] **1362 - Greater Invisibility** - Strong invisibility
- [ ] **1363 - Decoy** - Create decoy

#### Circle 4 (11 mana)
- [ ] **1364 - Illusory Double** - Create double
- [ ] **1365 - Charm** - Charm spell
- [ ] **1366 - Mass Invisibility** - AoE invisibility
- [ ] **1367 - Phantom Army** - Multiple phantoms

#### Circle 5 (14 mana)
- [ ] **1368 - Grand Illusion** - Grand illusion
- [ ] **1369 - Dominate** - Control spell
- [ ] **1370 - Perfect Invisibility** - Perfect invisibility
- [ ] **1371 - Mirage** - Mirage spell

#### Circle 6 (20 mana)
- [ ] **1372 - Phantasmal Killer** - Illusion damage
- [ ] **1373 - Mass Charm** - AoE charm
- [ ] **1374 - Cloak of Shadows** - Shadow cloak
- [ ] **1375 - Nightmare** - Fear spell

#### Circle 7 (40 mana)
- [ ] **1376 - Reality Warp** - Reality manipulation
- [ ] **1377 - Mind Control** - Ultimate control
- [ ] **1378 - Void Cloak** - Ultimate invisibility
- [ ] **1379 - Dream Walk** - Dream spell

#### Circle 8 (50 mana)
- [ ] **1380 - True Illusion** - Ultimate illusion
- [ ] **1381 - Absolute Domination** - Ultimate control
- [ ] **1382 - Nonexistence** - Ultimate invisibility
- [ ] **1383 - Eternal Dream** - Ultimate dream

---

## Martial Classes (14)

### 13. Fighter
**Resource:** Stamina | **Skill:** Swordplay | **Ability IDs:** 2000-2015

#### Stances
- [ ] **Aggressive Stance** - +damage, -defense
- [ ] **Defensive Stance** - +defense, -damage
- [ ] **Balanced Stance** - Balanced stats
- [ ] **Berserker Stance** - High risk/reward

#### Circle 1 (4 resource)
- [ ] **2000 - Power Strike** - High damage single target
- [ ] **2001 - Shield Bash** - Stun target for 2s
- [ ] **2002 - Battle Shout** - Buff allies +10 STR
- [ ] **2003 - Defensive Stance** - +20 all resists for 30s

#### Circle 2 (7 resource)
- [ ] **2004 - Cleave** - Hit all enemies in front
- [ ] **2005 - Shield Wall** - Block 50% damage for 10s
- [ ] **2006 - Disarm** - Disarm target for 5s
- [ ] **2007 - Charge** - Rush to target and stun

#### Circle 3 (10 resource)
- [ ] **2008 - Weapon Mastery** - +25% damage for 20s
- [ ] **2009 - Ironclad Stance** - Reduce damage taken 30%
- [ ] **2010 - Execute** - Bonus damage if target <30% HP
- [ ] **2011 - Rallying Cry** - Buff allies

#### Circle 4 (13 resource)
- [ ] **2012 - Whirlwind** - Spin attack all nearby
- [ ] **2013 - Last Stand** - Emergency defense
- [ ] **2014 - Overpower** - High damage attack
- [ ] **2015 - Victory Rush** - Heal on kill

---

### 14. Barbarian
**Resource:** Fury | **Skill:** Rage | **Ability IDs:** 2016-2031

#### Stances
- [ ] **Normal State** - Default state
- [ ] **Rage Transformation** - +STR, +damage, -INT

#### Circle 1 (4 resource)
- [ ] **2016 - Reckless Strike** - High damage, costs HP
- [ ] **2017 - Frost Fury** - Cold damage melee attack
- [ ] **2018 - War Cry** - Fear nearby enemies
- [ ] **2019 - Thick Skin** - +15 cold resist

#### Circle 2 (7 resource)
- [ ] **2020 - Whirlwind** - Spin attack all nearby
- [ ] **2021 - Berserker Rage** - +50% damage, -20% defense
- [ ] **2022 - Ground Slam** - Knockdown nearby enemies
- [ ] **2023 - Winter's Endurance** - HP regen in cold areas

#### Circle 3 (10 resource)
- [ ] **2024 - Avalanche Strike** - Ice shards from ground slam
- [ ] **2025 - Blood Rage** - Lifesteal on attacks
- [ ] **2026 - Intimidating Roar** - Reduce enemy damage 25%
- [ ] **2027 - Frenzy** - +50% attack speed

#### Circle 4 (13 resource)
- [ ] **2028 - Wrath of the North** - Massive cold AoE
- [ ] **2029 - Deathwish** - More damage when low HP
- [ ] **2030 - Rampage** - Chain of 5 attacks
- [ ] **2031 - Primal Avatar** - Ultimate transformation

---

### 15. Monk
**Resource:** Chi | **Skill:** Discipline | **Ability IDs:** 2032-2047

#### Stances
- [ ] **Windwalker** - Speed and evasion
- [ ] **Brewmaster** - Tank with stagger
- [ ] **Mistweaver** - Healing stance

#### Circle 1 (4 resource)
- [ ] **2032 - Tiger Palm** - Chi builder
- [ ] **2033 - Roll** - Movement ability
- [ ] **2034 - Touch of Death** - Execute ability
- [ ] **2035 - Serenity** - Chi regen

#### Circle 2 (7 resource)
- [ ] **2036 - Fists of Fury** - Rapid attacks
- [ ] **2037 - Spinning Crane Kick** - AoE attack
- [ ] **2038 - Healing Sphere** - Heal spell
- [ ] **2039 - Paralysis** - Stun ability

#### Circle 3 (10 resource)
- [ ] **2040 - Rising Sun Kick** - High damage
- [ ] **2041 - Transcendence** - Movement ability
- [ ] **2042 - Revival** - Resurrection
- [ ] **2043 - Fortifying Brew** - Defense buff

#### Circle 4 (13 resource)
- [ ] **2044 - Storm, Earth, and Fire** - Split into 3
- [ ] **2045 - Touch of Karma** - Damage reflection
- [ ] **2046 - Whirling Dragon Punch** - Ultimate combo
- [ ] **2047 - Inner Peace** - Ultimate buff

---

### 16. Rogue
**Resource:** Combo | **Skill:** Assassination | **Ability IDs:** 2048-2063

#### Stances
- [ ] **Shadow** - Stealth and burst damage
- [ ] **Outlaw** - Dual wield, sustained damage
- [ ] **Subtlety** - Control and DoTs

#### Circle 1 (4 resource)
- [ ] **2048 - Sinister Strike** - Combo builder
- [ ] **2049 - Stealth** - Enter stealth
- [ ] **2050 - Eviscerate** - Finisher
- [ ] **2051 - Vanish** - Escape ability

#### Circle 2 (7 resource)
- [ ] **2052 - Blade Flurry** - AoE attack
- [ ] **2053 - Shadowstep** - Teleport behind target
- [ ] **2054 - Kidney Shot** - Stun ability
- [ ] **2055 - Cloak of Shadows** - Magic immunity

#### Circle 3 (10 resource)
- [ ] **2056 - Adrenaline Rush** - Attack speed buff
- [ ] **2057 - Shadow Dance** - Stealth in combat
- [ ] **2058 - Death from Above** - Leap attack
- [ ] **2059 - Feint** - Damage reduction

#### Circle 4 (13 resource)
- [ ] **2060 - Vendetta** - Mark target for death
- [ ] **2061 - Shadow Blades** - Weapon buff
- [ ] **2062 - Killing Spree** - Chain attacks
- [ ] **2063 - Master Assassin** - Ultimate buff

---

### 17. Ranger
**Resource:** Focus | **Skill:** Tracking | **Ability IDs:** 2064-2079

#### Stances (Aspects)
- [ ] **Hawk Aspect** - Ranged accuracy
- [ ] **Wolf Aspect** - Pack tactics, speed
- [ ] **Bear Aspect** - Survival, HP regen

#### Circle 1 (4 resource)
- [ ] **2064 - Aimed Shot** - High damage ranged
- [ ] **2065 - Hunter's Mark** - Mark target
- [ ] **2066 - Serpent Sting** - DoT
- [ ] **2067 - Track Beasts** - Tracking ability

#### Circle 2 (7 resource)
- [ ] **2068 - Multi-Shot** - Multiple targets
- [ ] **2069 - Disengage** - Backward movement
- [ ] **2070 - Freezing Trap** - Trap ability
- [ ] **2071 - Aspect of the Hawk** - Ranged buff

#### Circle 3 (10 resource)
- [ ] **2072 - Rapid Fire** - Attack speed buff
- [ ] **2073 - Camouflage** - Stealth ability
- [ ] **2074 - Kill Command** - Pet attack
- [ ] **2075 - Aspect of the Wolf** - Speed buff

#### Circle 4 (13 resource)
- [ ] **2076 - Trueshot** - Ultimate ranged
- [ ] **2077 - Bestial Wrath** - Pet buff
- [ ] **2078 - Barrage** - AoE ranged
- [ ] **2079 - Aspect of the Bear** - Tank buff

---

### 18. Knight
**Resource:** Honor | **Skill:** Chivalry | **Ability IDs:** 2080-2095

#### Circle 1 (4 resource)
- [ ] **2080 - Charge** - Mounted charge
- [ ] **2081 - Shield Block** - Block attack
- [ ] **2082 - Rally** - Buff allies
- [ ] **2083 - Honor's Call** - Resource gain

#### Circle 2 (7 resource)
- [ ] **2084 - Lance Strike** - Mounted attack
- [ ] **2085 - Dismount** - Dismount ability
- [ ] **2086 - Banner** - Place banner buff
- [ ] **2087 - Challenge** - Taunt ability

#### Circle 3 (10 resource)
- [ ] **2088 - Joust** - Mounted combat
- [ ] **2089 - Shield Wall** - Defense formation
- [ ] **2090 - Valorous Strike** - High damage
- [ ] **2091 - Inspiring Presence** - AoE buff

#### Circle 4 (13 resource)
- [ ] **2092 - Cavalry Charge** - Ultimate charge
- [ ] **2093 - Last Stand** - Emergency defense
- [ ] **2094 - Heroic Leap** - Leap ability
- [ ] **2095 - Knight's Vow** - Ultimate buff

---

### 19. Paladin
**Resource:** Devotion | **Skill:** Holiness | **Ability IDs:** 2096-2111

#### Stances (Auras)
- [ ] **Devotion Aura** - Healing aura
- [ ] **Retribution Aura** - Damage reflection
- [ ] **Protection Aura** - Damage reduction aura

#### Circle 1 (4 resource)
- [ ] **2096 - Holy Strike** - Holy damage
- [ ] **2097 - Lay on Hands** - Heal target
- [ ] **2098 - Consecration** - Ground effect
- [ ] **2099 - Devotion Aura** - Healing aura

#### Circle 2 (7 resource)
- [ ] **2100 - Hammer of Wrath** - Execute ability
- [ ] **2101 - Divine Shield** - Invulnerability
- [ ] **2102 - Retribution Aura** - Damage reflection
- [ ] **2103 - Turn Undead** - Undead CC

#### Circle 3 (10 resource)
- [ ] **2104 - Avenging Wrath** - Damage buff
- [ ] **2105 - Protection Aura** - Damage reduction
- [ ] **2106 - Hand of Protection** - Protect ally
- [ ] **2107 - Exorcism** - Undead damage

#### Circle 4 (13 resource)
- [ ] **2108 - Divine Storm** - AoE holy damage
- [ ] **2109 - Guardian of Ancient Kings** - Summon
- [ ] **2110 - Final Stand** - Ultimate defense
- [ ] **2111 - Divine Intervention** - Ultimate heal

---

### 20. Templar
**Resource:** Zeal | **Skill:** Totemry | **Ability IDs:** 2112-2127

#### Circle 1 (4 resource)
- [ ] **2112 - Zealous Strike** - Zeal damage
- [ ] **2113 - Shield of Faith** - Defense buff
- [ ] **2114 - Smite** - Holy damage
- [ ] **2115 - Zeal** - Resource builder

#### Circle 2 (7 resource)
- [ ] **2116 - Fanaticism** - Attack speed buff
- [ ] **2117 - Holy Ground** - Ground effect
- [ ] **2118 - Fist of the Heavens** - AoE damage
- [ ] **2119 - Conviction** - Debuff aura

#### Circle 3 (10 resource)
- [ ] **2120 - Blessed Hammer** - Hammer spell
- [ ] **2121 - Redemption** - Resurrection
- [ ] **2122 - Charge** - Movement ability
- [ ] **2123 - Vigor** - Speed buff

#### Circle 4 (13 resource)
- [ ] **2124 - Heaven's Fury** - Ultimate damage
- [ ] **2125 - Akarat's Champion** - Transformation
- [ ] **2126 - Falling Sword** - Leap attack
- [ ] **2127 - Ascendancy** - Ultimate buff

---

### 21. Bounty Hunter
**Resource:** Marks | **Skill:** Manhunting | **Ability IDs:** 2128-2143

#### Circle 1 (4 resource)
- [ ] **2128 - Mark Target** - Mark for bonus damage
- [ ] **2129 - Tracking Shot** - Ranged attack
- [ ] **2130 - Net** - Snare ability
- [ ] **2131 - Bounty** - Resource gain

#### Circle 2 (7 resource)
- [ ] **2132 - Explosive Trap** - Trap ability
- [ ] **2133 - Deadly Aim** - Accuracy buff
- [ ] **2134 - Capture** - CC ability
- [ ] **2135 - Track** - Tracking ability

#### Circle 3 (10 resource)
- [ ] **2136 - Kill Shot** - Execute ability
- [ ] **2137 - Smoke Bomb** - Escape ability
- [ ] **2138 - Bounty's Reward** - Buff on kill
- [ ] **2139 - Hunter's Mark** - Mark ability

#### Circle 4 (13 resource)
- [ ] **2140 - Death Mark** - Ultimate mark
- [ ] **2141 - Master Tracker** - Ultimate tracking
- [ ] **2142 - Bounty's Bane** - Ultimate damage
- [ ] **2143 - Legendary Hunter** - Ultimate buff

---

### 22. Beastmaster
**Resource:** Bond | **Skill:** BeastBonding | **Ability IDs:** 2144-2159

#### Circle 1 (4 resource)
- [ ] **2144 - Command Pet** - Pet attack
- [ ] **2145 - Beast Bond** - Link to pet
- [ ] **2146 - Mend Pet** - Heal pet
- [ ] **2147 - Call Pet** - Summon pet

#### Circle 2 (7 resource)
- [ ] **2148 - Bestial Wrath** - Pet buff
- [ ] **2149 - Intimidation** - Taunt ability
- [ ] **2150 - Pet Charge** - Pet movement
- [ ] **2151 - Beast's Strength** - Pet buff

#### Circle 3 (10 resource)
- [ ] **2152 - Stampede** - Pet AoE
- [ ] **2153 - Revive Pet** - Resurrection
- [ ] **2154 - Pack Leader** - Multiple pets
- [ ] **2155 - Beast Mastery** - Pet buff

#### Circle 4 (13 resource)
- [ ] **2156 - Primal Rage** - Ultimate pet buff
- [ ] **2157 - Alpha** - Ultimate pet
- [ ] **2158 - Pack Hunt** - Ultimate ability
- [ ] **2159 - Beast Lord** - Ultimate transformation

---

### 23. Artificer
**Resource:** Power | **Skill:** Engineering | **Ability IDs:** 2160-2175

#### Circle 1 (4 resource)
- [ ] **2160 - Construct** - Summon construct
- [ ] **2161 - Repair** - Heal construct
- [ ] **2162 - Overcharge** - Damage buff
- [ ] **2163 - Gadget** - Utility ability

#### Circle 2 (7 resource)
- [ ] **2164 - Turret** - Place turret
- [ ] **2165 - Shield Generator** - Defense ability
- [ ] **2166 - Rocket** - Ranged attack
- [ ] **2167 - Upgrade** - Construct buff

#### Circle 3 (10 resource)
- [ ] **2168 - Mech Suit** - Transformation
- [ ] **2169 - Explosive Trap** - Trap ability
- [ ] **2170 - Power Surge** - Burst damage
- [ ] **2171 - Advanced Construct** - Strong construct

#### Circle 4 (13 resource)
- [ ] **2172 - Titan Mech** - Ultimate construct
- [ ] **2173 - Nuclear Option** - Ultimate damage
- [ ] **2174 - Master Engineer** - Ultimate buff
- [ ] **2175 - Apocalypse Engine** - Ultimate ability

---

### 24. Alchemist
**Resource:** Reagent | **Skill:** Transmutation | **Ability IDs:** 2176-2191

#### Circle 1 (4 resource)
- [ ] **2176 - Potion** - Create potion
- [ ] **2177 - Transmute** - Transform item
- [ ] **2178 - Acid Flask** - Damage ability
- [ ] **2179 - Alchemical Fire** - Fire damage

#### Circle 2 (7 resource)
- [ ] **2180 - Healing Potion** - Heal ability
- [ ] **2181 - Explosive Flask** - AoE damage
- [ ] **2182 - Elixir** - Buff ability
- [ ] **2183 - Poison** - DoT ability

#### Circle 3 (10 resource)
- [ ] **2184 - Greater Potion** - Strong potion
- [ ] **2185 - Transmutation** - Transform ability
- [ ] **2186 - Volatile Mix** - Explosive ability
- [ ] **2187 - Philosopher's Stone** - Utility

#### Circle 4 (13 resource)
- [ ] **2188 - Ultimate Elixir** - Ultimate potion
- [ ] **2189 - Grand Transmutation** - Ultimate transform
- [ ] **2190 - Master Alchemist** - Ultimate buff
- [ ] **2191 - Alchemical Apocalypse** - Ultimate ability

---

### 25. Cleric
**Resource:** Faith | **Skill:** Divinity | **Ability IDs:** 2192-2207

#### Circle 1 (4 resource)
- [ ] **2192 - Heal** - Heal target
- [ ] **2193 - Smite** - Holy damage
- [ ] **2194 - Blessing** - Buff spell
- [ ] **2195 - Prayer** - Resource gain

#### Circle 2 (7 resource)
- [ ] **2196 - Greater Heal** - Strong heal
- [ ] **2197 - Holy Light** - AoE heal
- [ ] **2198 - Shield of Faith** - Defense buff
- [ ] **2199 - Turn Undead** - Undead CC

#### Circle 3 (10 resource)
- [ ] **2200 - Mass Heal** - AoE heal
- [ ] **2201 - Divine Wrath** - Holy damage
- [ ] **2202 - Resurrection** - Resurrection
- [ ] **2203 - Sanctuary** - Protection spell

#### Circle 4 (13 resource)
- [ ] **2204 - Divine Intervention** - Ultimate heal
- [ ] **2205 - Holy Word** - Ultimate damage
- [ ] **2206 - Divine Protection** - Ultimate defense
- [ ] **2207 - Ascension** - Ultimate buff

---

### 26. Wizard
**Resource:** Arcane | **Skill:** ArcaneStudies | **Ability IDs:** 2208-2223

#### Circle 1 (4 resource)
- [ ] **2208 - Arcane Missile** - Arcane damage
- [ ] **2209 - Arcane Intellect** - Buff spell
- [ ] **2210 - Polymorph** - Transform target
- [ ] **2211 - Counterspell** - Interrupt spell

#### Circle 2 (7 resource)
- [ ] **2212 - Arcane Blast** - Arcane damage
- [ ] **2213 - Teleport** - Movement ability
- [ ] **2214 - Arcane Shield** - Defense buff
- [ ] **2215 - Slow** - Slow debuff

#### Circle 3 (10 resource)
- [ ] **2216 - Arcane Barrage** - Arcane AoE
- [ ] **2217 - Time Warp** - Movement ability
- [ ] **2218 - Arcane Power** - Damage buff
- [ ] **2219 - Dispel Magic** - Dispel spell

#### Circle 4 (13 resource)
- [ ] **2220 - Arcane Orb** - Ultimate damage
- [ ] **2221 - Arcane Mastery** - Ultimate buff
- [ ] **2222 - Spellsteal** - Steal buff
- [ ] **2223 - Archmage** - Ultimate transformation

---

## Testing Notes

### General Testing Tips

1. **Test Each Circle** - Start with Circle 1, work your way up
2. **Test Resource Costs** - Verify correct resource is consumed (check scaling table above)
3. **Test Cooldowns** - Ensure cooldowns apply correctly (check scaling table above)
4. **Test Visuals** - Check particles, animations, sounds
5. **Test Targeting** - Single target, AoE, self-target, ground target
6. **Test Durations** - Buffs, debuffs, DoTs should last correct time
7. **Test Interactions** - Stances + abilities, combos, synergies
8. **Test Starting Stats** - Verify class starts with correct STR/DEX/INT (see tables above)
9. **Test Cast Times** - Verify cast times scale with circle (instant for C1, 3s for C8)

### Common Issues to Watch For

- Spells/abilities not casting
- Wrong resource cost (check circle-based scaling)
- Visual effects missing
- Sound effects missing
- Targeting not working
- Damage/effect not applying
- Duration incorrect
- Cooldown not applying (check circle-based scaling)
- Stance not activating
- Resource not regenerating
- Starting stats not matching class design (check tables above)
- Cast time not matching circle (should be instant for C1, 3s for C8)

### Reporting Issues

When reporting issues, include:
- Class name
- Spell/ability name and ID
- What you expected
- What actually happened
- Steps to reproduce

---

*Last Updated: 2025-01-02*

