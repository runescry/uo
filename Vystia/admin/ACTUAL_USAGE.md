# Vystia Active Admin Commands - Current Status

**Last Updated:** 2025-12-13

This document lists **all currently active Vystia admin commands** that are implemented and working in the server.

---

## ✅ **ACTIVE ADMIN COMMANDS (89 Total)**

### **Master Admin Interface**

| Command | Description |
|---------|-------------|
| `[va` | **VystiaAdmin Gump** - Master admin interface for all systems |
| `[VystiaAdmin` | Same as above (full name) |

**Features:** Class assignment, resource management, buff/debuff application, stance management, ability testing, equipment spawning, system diagnostics

---

### **Class System Commands**

| Command | Description | Example |
|---------|-------------|---------|
| `[SetClassV2 <class>` | Set target's class | `[SetClassV2 IceMage` |
| `[RemoveClassV2` | Remove target's class | |
| `[ClassInfoV2` | Show target's class info | |
| `[ListClassesV2` | List all 26 classes | |

**Available Classes (26):** Barbarian, Beastmaster, IceMage, Sorcerer, Ranger, Illusionist, Witch, Warlock, Necromancer, Druid, Alchemist, Wizard, Oracle, Artificer, Fighter, Monk, Templar, Summoner, BountyHunter, Knight, Shaman, Cleric, Paladin, Bard, Enchanter, Rogue

---

### **Class Trainer Commands**

| Command | Description | Example |
|---------|-------------|---------|
| `[spawntrainer <class>` | Spawn specific trainer | `[spawntrainer IceMage` |
| `[str <class>` | Same as above (shortcut) | `[str Barbarian` |
| `[spawnalltrainers` | Spawn all 25 trainers in circle | |
| `[sat` | Same as above (shortcut) | |

---

### **Skill Commands**

| Command | Description | Example |
|---------|-------------|---------|
| `[rvs` | Reset all 26 Vystia skills to 0.0 | |
| `[resetvystiaskills` | Same as above (full name) | |
| `[svs <value>` | Set all Vystia skills to value (0-120) | `[svs 100` |
| `[setvystiaskills <value>` | Same as above (full name) | `[setvystiaskills 100` |
| `[skillcap` | Show current skill cap | |
| `[skillcap <value>` | Set skill cap | `[skillcap 84000` |
| `[skillinfo` | Show skill total breakdown | |

**Recommended skill cap:** 84000 (8400 total points)

---

### **Secondary Resource Commands**

| Command | Description | Example |
|---------|-------------|---------|
| `[SetResource <type> <value>` | Set resource to value | `[SetResource Fury 100` |
| `[GetResources` | Show all resources for target | |
| `[ResetResources` | Reset all resources to 0 | |
| `[TestResource <type>` | Test resource generation | `[TestResource Chi` |

**Resource Types (15):** Fury, Chi, Combo, Energy, Focus, Essence, Mana, Nature, Shadow, Soul, Spirit, Rage, Heat, Chill, Harmony

---

### **Stance Commands**

| Command | Description | Example |
|---------|-------------|---------|
| `[SetStance <stance>` | Set active stance | `[SetStance Berserker` |
| `[RemoveStance` | Remove current stance | |
| `[ListStances` | List all 28 available stances | |
| `[ClearStances` | Clear all stance effects | |
| `[StanceInfo` | Show current stance details | |
| `[ResetStanceCooldowns` | Reset all stance cooldowns | |

---

### **Buff/Debuff Commands**

| Command | Description | Example |
|---------|-------------|---------|
| `[ApplyBuff <type> <duration> [power]` | Apply buff to target | `[ApplyBuff StrengthBoost 60 10` |
| `[RemoveBuff <type>` | Remove specific buff | |
| `[ListBuffs` | List all active buffs on target | |
| `[ClearBuffs` | Remove all buffs from target | |

**25+ Buff Types:** StrengthBoost, DexterityBoost, IntelligenceBoost, DamageShield, HealOverTime, DamageOverTime, SpeedBoost, DefenseBoost, etc.

---

### **Crowd Control Commands**

| Command | Description | Example |
|---------|-------------|---------|
| `[ApplyCC <type> <duration>` | Apply CC to target | `[ApplyCC Stun 5` |
| `[RemoveCC <type>` | Remove specific CC | |
| `[ListCC` | List active CC effects | |
| `[CheckDR <type>` | Check diminishing returns | |
| `[ResetDR` | Reset all diminishing returns | |

**15 CC Types:** Stun, Root, Silence, Fear, Sleep, Charm, Slow, Disarm, Blind, Polymorph, Knockback, Knockdown, Taunt, Confuse, Freeze

---

### **Ability Commands**

| Command | Description | Example |
|---------|-------------|---------|
| `[TestAbility <id>` | Execute an ability | `[TestAbility 2001` |
| `[ListAbilities [class]` | List abilities (optionally by class) | `[ListAbilities Fighter` |
| `[AbilityEditor` | Open ability editor gump | |
| `[AE` | Same as above (shortcut) | |
| `[TestDummy` | Spawn combat test dummy | |
| `[TD` | Same as above (shortcut) | |

**608 Total Abilities:**
- Magic Spells: 384 (IDs 1000-1383)
- Martial Abilities: 224 (IDs 2000-2223)

---

### **Damage System Commands**

| Command | Description | Example |
|---------|-------------|---------|
| `[TestDamage <amount> [type]` | Deal test damage | `[TestDamage 50 Fire` |
| `[TestHeal <amount>` | Heal target | `[TestHeal 100` |
| `[TestCrit` | Force critical hit test | |

**Damage Types:** Physical, Fire, Cold, Poison, Energy

---

### **Target Tracker Commands**

| Command | Description | Example |
|---------|-------------|---------|
| `[GetTargetStacks <type>` | Get stacks on target | `[GetTargetStacks Bleed` |
| `[SetTargetStacks <type> <count>` | Set stacks on target | `[SetTargetStacks Poison 5` |
| `[ClearTargetStacks` | Clear all stacks | |
| `[TestMark <type>` | Apply test mark | `[TestMark Hunter` |

---

### **Spawning Commands**

| Command | Description | Example |
|---------|-------------|---------|
| `[spawnvystia` | Open spawn gump (creatures, items, vendors) | |
| `[spawnvystia <radius>` | Open spawn gump with radius | `[spawnvystia 20` |
| `[clearvystia` | Delete Vystia creatures in area | |
| `[clearvystia <radius>` | Delete within radius | `[clearvystia 50` |

**Spawn Gump Pages:**
1. **Creatures** - All 138 Vystia creatures by region
2. **Magic Items** - Spellbooks, reagents, scrolls
3. **Vendors** - All 15 Vystia vendors

**Direct Spawning (using `[add`):**
```
[add FrostFather      # Spawn Frosthold boss
[add AncientTreant    # Spawn Verdantpeak boss
[add IceTroll         # Spawn Frosthold creature
[add FrozenOre        # Spawn Frosthold ore
[add FrostforgedIngot # Spawn Frosthold ingot
```

---

### **Spellbook Commands**

| Command | Description | Example |
|---------|-------------|---------|
| `[spellbook <type>` | Give specific spellbook | `[spellbook ice` |
| `[sb <type>` | Same as above (short form) | `[sb druid` |

**Spellbook Types:** ice, druid/nature, witch/hex, sorcerer/elemental, warlock/dark, oracle/divination, necromancer/necro, summoner/summon, shaman/shamanic, bard/bardic, enchanter/enchant, illusionist/illusion

---

### **Vendor Commands**

| Command | Description |
|---------|-------------|
| `[VystiaReagents` | Spawn reagent vendor (all 96 reagents) |
| `[vreag` | Same as above (shortcut) |
| `[VystiaResources` | Spawn resource vendor (ores, ingots) |
| `[vres` | Same as above (shortcut) |
| `[spawnreagentbags` | Spawn all 12 reagent bags (one per school) |
| `[srb` | Same as above (shortcut) |

---

### **AI Sidekick Commands**

| Command | Description |
|---------|-------------|
| `[st` | Spawn Tamer sidekick |
| `[SpawnMage` | Spawn Mage sidekick |
| `[SpawnSidekick` | Spawn any archetype sidekick |
| `[ol` | Spawn Arctic Ogre Lord for testing |

---

### **Dwarf Race Commands**

| Command | Description |
|---------|-------------|
| `[sd` | Spawn Dwarf (random gender) |
| `[sdm` | Spawn Male Dwarf |
| `[sdf` | Spawn Female Dwarf |

**Note:** Dwarf race uses custom bodies 987 (male) and 988 (female) with 75% scale sprites.

---

### **LLM NPC Commands**

| Command | Description | Example |
|---------|-------------|---------|
| `[SpawnPersonalityNPC <type> <pattern>` | Spawn NPC with personality | `[SpawnPersonalityNPC Mage Archaic` |
| `[SpawnTownNPCs` | Spawn town NPC group | |
| `[SpawnMagicNPCs` | Spawn magic NPC group | |
| `[SpawnAdventurerNPCs` | Spawn adventurer NPC group | |
| `[KnowledgeTest` | Run comprehensive knowledge test | |
| `[LocationTest` | Run location-based response test | |
| `[MemoryTest` | Run memory system validation | |
| `[MemoryStats` | Show memory system stats | |
| `[ViewMemories <npc> <player>` | View NPC memories of player | |
| `[ClearMemories <npc> <player>` | Clear memories | |
| `[LoreStats` | Show lore system stats | |
| `[CacheStats` | Show cache statistics | |

---

### **Quest Management Commands**

| Command | Description | Example |
|---------|-------------|---------|
| `[QuestEditor` | Open quest editor gump | |
| `[QE` | Same as above (shortcut) | |
| `[addquestNPC` | Open quest NPC spawn wizard | |
| `[aqn` | Same as above (shortcut) | |
| `[FindQuestNPC` | Find quest NPCs for active quests | |
| `[FQNPC` | Same as above (shortcut) | |
| `[FindQuestNPC respawn` | Respawn missing quest NPCs | |
| `[ClearQuests` | Clear all your own quests | |
| `[ClearQuests <playerName>` | Clear all quests for a player | `[ClearQuests Runescry` |
| `[GenLLMQuest [poiId]` | Generate quest using LLM | `[GenLLMQuest VERDANTPEAK_GROVE` |
| `[DeleteQuest <questId>` | Delete a quest by ID | |

**Note:** These commands are for **Vystia Dynamic Quests** (QuestNPC/Chronicler).  
The **Mondain/BaseQuest** system uses classic ServUO quest givers (MondainQuester/LLMQuester) and does not use the Quest Wizard.

**Quest Editor Features:**
- Create quests without coding
- Set title, description, class requirement, tier
- Add waypoints (Origin, Waypoint, BossCompletion, NPCCompletion)
- Add objectives (TalkToNPC, ReachLocation, DefeatBoss, etc.)
- Add rewards (gold, skill points, items, titles)
- Give quests directly to players
- Quests auto-save to `Data/VystiaQuests.xml`

---

### **Test Spell Commands (Player Level)**

| Command | Description |
|---------|-------------|
| `[frostmeteor` | Cast Frost Meteor (Circle 8) |
| `[iceage` | Cast Ice Age (Circle 8) |
| `[blizzard` | Cast Blizzard (Circle 7) |
| `[absolutezero` | Cast Absolute Zero (Circle 8) |

---

## 📊 **Command Count Summary**

| Category | Count |
|----------|-------|
| VystiaAdmin | 2 |
| Class System | 4 |
| Class Trainers | 4 |
| Skills | 6 |
| Resources | 4 |
| Stances | 6 |
| Buffs | 4 |
| Crowd Control | 5 |
| Abilities | 6 |
| Damage | 3 |
| Target Tracker | 4 |
| Spawning | 4 |
| Spellbooks | 2 |
| Vendors | 4 |
| Reagent Bags | 2 |
| AI Sidekicks | 4 |
| Dwarves | 3 |
| LLM NPCs | 12 |
| Quest Management | 7 |
| Test Spells | 4 |
| **Total** | **89** |

---

## 📚 **Documentation References**

- **Complete Command Reference:** `Vystia/reference/COMMANDS.md`
- **GM Testing Guide:** `Vystia/gm/GM_TESTING_GUIDE.md`
- **Quest System:** `Vystia/implementation/QUEST_SYSTEM_COMPLETE.md`
- **Class Guides:** `Vystia/player/guides/`

---

## ⚠️ **What Does NOT Exist**

### **World Generation Commands (Not Implemented)**
- ❌ `[VystiaStatus`
- ❌ `[VystiaDeploy`
- ❌ `[GenVystiaWorld`
- ❌ `[ClearVystiaWorld`
- ❌ `[GenVystiaSpawners`
- ❌ `[ClearVystiaSpawners`

### **World Map Status**
- No Vystia-specific world map has been generated
- NPCs spawn in default UO world (Britannia map)
- No cities or dungeons have been placed in-game
- Coordinates in design docs are conceptual only

---

*Last Updated: 2025-12-13 - All commands verified as active in codebase*
