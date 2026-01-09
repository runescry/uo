# GM Commands Reference

All GM commands for Vystia administration (126+ commands including aliases).

**Full Reference:** See [../reference/COMMANDS.md](../reference/COMMANDS.md) for complete documentation.

**Last Updated:** 2026-01-02

---

## Essential Commands

| Command | Description |
|---------|-------------|
| `[va` | **VystiaAdmin Gump** - Master admin interface |
| `[spawnvystia` | Spawn creatures, items, vendors |
| `[spawnalltrainers` / `[sat` | Spawn all 25 class trainers |

---

## Economy & Services (NEW - 2026-01-02)

| Command | Description | Example |
|---------|-------------|---------|
| `[RepairCost` | Show repair costs for all damaged equipment | |
| `[SpawnBlacksmith` | Spawn Vystia blacksmith with repair service | |
| `[SpawnHealer` | Spawn Vystia healer with resurrection service | |
| `[SpawnMoongateAttendant` | Spawn moongate attendant NPC | |
| `[ServiceFees` | Display all service fee rates | |

**Service Fee Rates:**
- Resurrection: 50g base + 10g per fame level
- Travel: 100g (short), 175g (medium), 250g (long)
- Stabling: 30g/day, 150g/week
- Repair: 2-75g per durability based on item tier

---

## Class Management

| Command | Description | Example |
|---------|-------------|---------|
| `[SetClassV2 <class>` | Set player's class | `[SetClassV2 IceMage` |
| `[RemoveClassV2` | Remove player's class | |
| `[ClassInfoV2` | Show class info | |
| `[ListClassesV2` | List all 26 classes | |
| `[spawntrainer <class>` | Spawn specific trainer | `[spawntrainer Barbarian` |
| `[str <class>` | Shortcut for above | `[str IceMage` |

**Classes:** Barbarian, Beastmaster, IceMage, Sorcerer, Ranger, Illusionist, Witch, Warlock, Necromancer, Druid, Alchemist, Wizard, Oracle, Artificer, Fighter, Monk, Templar, Summoner, BountyHunter, Knight, Shaman, Cleric, Paladin, Bard, Enchanter, Rogue

---

## Skills

| Command | Description | Example |
|---------|-------------|---------|
| `[svs <value>` | Set all Vystia skills | `[svs 100` |
| `[rvs` | Reset all Vystia skills to 0 | |
| `[skillcap <value>` | Set skill cap | `[skillcap 84000` |
| `[skillinfo` | Show skill breakdown | |

---

## Secondary Resources

| Command | Description | Example |
|---------|-------------|---------|
| `[SetResource <type> <value>` | Set resource | `[SetResource Fury 100` |
| `[GetResources` | Show all resources | |
| `[ResetResources` | Reset to 0 | |

**Types:** Fury, Chi, Combo, Energy, Focus, Essence, Mana, Nature, Shadow, Soul, Spirit, Rage, Heat, Chill, Harmony

---

## Stances

| Command | Description | Example |
|---------|-------------|---------|
| `[SetStance <stance>` | Activate stance | `[SetStance Berserker` |
| `[RemoveStance` | Deactivate stance | |
| `[ListStances` | List all 28 stances | |
| `[StanceInfo` | Current stance details | |
| `[ResetStanceCooldowns` | Reset cooldowns | |

---

## Buffs & Debuffs

| Command | Description | Example |
|---------|-------------|---------|
| `[ApplyBuff <type> <duration> [power]` | Apply buff | `[ApplyBuff StrengthBoost 60 10` |
| `[RemoveBuff <type>` | Remove buff | |
| `[ListBuffs` | Show active buffs | |
| `[ClearBuffs` | Remove all buffs | |

---

## Crowd Control

| Command | Description | Example |
|---------|-------------|---------|
| `[ApplyCC <type> <duration>` | Apply CC | `[ApplyCC Stun 5` |
| `[RemoveCC <type>` | Remove CC | |
| `[ListCC` | Show active CC | |
| `[CheckDR <type>` | Check diminishing returns | |
| `[ResetDR` | Reset DR | |

**CC Types:** Stun, Root, Silence, Fear, Sleep, Charm, Slow, Disarm, Blind, Polymorph, Knockback, Knockdown, Taunt, Confuse, Freeze

---

## Abilities

| Command | Description | Example |
|---------|-------------|---------|
| `[TestAbility <id>` | Execute ability | `[TestAbility 2001` |
| `[ListAbilities [class]` | List abilities | `[ListAbilities Fighter` |
| `[AbilityEditor` / `[AE` | Open ability editor | |

---

## Damage Testing

| Command | Description | Example |
|---------|-------------|---------|
| `[TestDamage <amount> [type]` | Deal damage | `[TestDamage 50 Fire` |
| `[TestHeal <amount>` | Heal target | `[TestHeal 100` |
| `[TestCrit` | Force crit test | |

---

## Spawning

| Command | Description |
|---------|-------------|
| `[spawnvystia` | Open spawn gump (3 pages: Creatures, Magic, Vendors) |
| `[clearvystia [radius]` | Delete Vystia creatures |
| `[spawnalltrainers` / `[sat` | Spawn all 25 trainers |
| `[spawntrainer <class>` / `[str` | Spawn one trainer |

---

## Spellbooks

| Command | Description | Example |
|---------|-------------|---------|
| `[spellbook <type>` / `[sb` | Give spellbook | `[sb ice` |

**Types:** ice, druid/nature, witch/hex, sorcerer/elemental, warlock/dark, oracle/divination, necromancer/necro, summoner/summon, shaman/shamanic, bard/bardic, enchanter/enchant, illusionist/illusion

---

## Vendors

| Command | Description |
|---------|-------------|
| `[VystiaReagents` / `[vreag` | Spawn reagent vendor |
| `[VystiaResources` / `[vres` | Spawn resource vendor |
| `[spawnreagentbags` / `[srb` | Spawn all 12 reagent bags |

---

## Quest Management

| Command | Description | Example |
|---------|-------------|---------|
| `[QuestEditor` / `[QE` | Open quest editor gump | |
| `[addquestNPC` / `[aqn` | Add quest NPC wizard | |
| `[FindQuestNPC` / `[FQNPC` | Find quest NPCs for active quests | |
| `[FindQuestNPC respawn` | Respawn missing quest NPCs | |
| `[ClearQuests [playerName]` | Clear all quests for player | `[ClearQuests Runescry` |
| `[GenLLMQuest [poiId]` | Generate LLM quest | `[GenLLMQuest VERDANTPEAK_GROVE` |

**Quest Editor Features:**
- Create quests without coding
- Set title, description, class requirement, tier
- Add objectives (kill, collect, talk, cast, visit)
- Add rewards (gold, skill points, items, titles)
- Give quests directly to players
- Quests auto-save to Data/VystiaQuests.xml

**Quest NPC Management:**
- `[FindQuestNPC` shows all NPCs for your active quests and their locations
- Use `[FindQuestNPC respawn` to recreate missing NPCs
- `[addquestNPC` opens wizard to spawn and link NPCs to quest waypoints

---

## AI Sidekicks

| Command | Description |
|---------|-------------|
| `[st` | Spawn Tamer sidekick |
| `[SpawnMage` | Spawn Mage sidekick |
| `[SpawnSidekick` | Spawn any archetype |
| `[ol` | Spawn Ogre Lord for testing |

---

## Dwarf Race

| Command | Description |
|---------|-------------|
| `[sd` | Spawn Dwarf (random) |
| `[sdm` | Spawn Male Dwarf |
| `[sdf` | Spawn Female Dwarf |

---

## Target Tracker

| Command | Description |
|---------|-------------|
| `[GetTargetStacks <type>` | Get stacks |
| `[SetTargetStacks <type> <count>` | Set stacks |
| `[ClearTargetStacks` | Clear all stacks |
| `[TestMark <type>` | Apply test mark |

---

## Quick Reference

**Most Used:**
```
[va                    - Admin gump
[spawnvystia           - Spawn menu
[sat                   - All trainers
[SetClassV2 IceMage    - Set class
[svs 100               - Max skills
[sb ice                - Give spellbook
```

**Testing a Class:**
```
[SetClassV2 Barbarian
[svs 100
[SetResource Fury 100
[SetStance Berserker
[TestAbility 2016
```
