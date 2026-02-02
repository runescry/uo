# Vystia GM Commands Reference

> Source of Truth: Command availability is defined by ServUO/Scripts/ command registrations.

Last Updated: 2026-01-23


Complete list of GM commands for testing and administration.

**Legacy Updated:** 2026-01-02

---

## Quick Reference by Category

| Category | Primary Commands |
|----------|------------------|
| [VystiaAdmin Gump](#vystiaadmin-gump) | `[va`, `[VystiaAdmin` |
| [Class System](#class-system-commands) | `[SetClassV2`, `[ClassInfoV2`, `[ListClassesV2` |
| [Class Trainers](#class-trainer-commands) | `[spawntrainer`, `[str`, `[spawnalltrainers`, `[sat` |
| [Skills](#skill-commands) | `[rvs`, `[svs`, `[skillcap`, `[skillinfo` |
| [Secondary Resources](#secondary-resource-commands) | `[SetResource`, `[GetResources`, `[ResetResources` |
| [Stances](#stance-commands) | `[SetStance`, `[ListStances`, `[StanceInfo` |
| [Buffs/Debuffs](#buff-commands) | `[ApplyBuff`, `[RemoveBuff`, `[ListBuffs`, `[ClearBuffs` |
| [Crowd Control](#crowd-control-commands) | `[ApplyCC`, `[RemoveCC`, `[ListCC`, `[CheckDR` |
| [Abilities](#ability-commands) | `[TestAbility`, `[ListAbilities`, `[AbilityEditor`, `[AE` |
| [Damage System](#damage-system-commands) | `[TestDamage`, `[TestHeal`, `[TestCrit` |
| [Target Tracker](#target-tracker-commands) | `[GetTargetStacks`, `[SetTargetStacks`, `[TestMark` |
| [Spawning](#spawn-commands) | `[spawnvystia`, `[clearvystia` |
| [Spellbooks](#spellbook-commands) | `[spellbook`, `[sb` |
| [Vendors](#vendor-commands) | `[VystiaReagents`, `[vreag`, `[VystiaResources`, `[vres` |
| [Reagent Bags](#reagent-bag-commands) | `[spawnreagentbags`, `[srb` |
| [AI Sidekicks](#ai-sidekick-commands) | `[st`, `[SpawnMage`, `[SpawnSidekick` |
| [Dwarves](#dwarf-commands) | `[sd`, `[sdm`, `[sdf` |
| [LLM NPCs](#llm-npc-commands) | `[SpawnPersonalityNPC`, `[KnowledgeTest` |
| [Quest Management](#quest-management-commands) | `[QuestEditor`, `[FindQuestNPC`, `[ClearQuests`, `[GenLLMQuest` |
| [Pet System](#pet-system-commands) | `[SummonPet`, `[DismissPets`, `[PetInfo` |
| [Housing System](#housing-system-commands) | `[HouseCosts`, `[TaxInfo`, `[PayTax`, `[TaxExempt` |
| [Zone Control](#zone-control-commands) | `[ZoneInfo`, `[TogglePvP`, `[SetZone`, `[ZoneList` |
| [Faction System](#faction-system-commands) | `[Factions`, `[SetReputation`, `[DonateFaction` |
| [Economy/Services](#economy-commands) | `[RepairCost`, `[SpawnBlacksmith`, `[SpawnHealer`, `[ServiceFees` |
| [Religion System](#religion-commands) | `[Religion`, `[SetReligion`, `[Pray`, `[Tithe` |

---

## VystiaAdmin Gump

The master admin interface for all Vystia systems.

| Command | Description |
|---------|-------------|
| `[va` | Open VystiaAdmin Gump (recommended) |
| `[VystiaAdmin` | Same as above (full name) |

**Features:**
- Class assignment and management
- Resource monitoring and modification
- Buff/debuff application
- Stance management
- Ability testing
- Equipment spawning
- System diagnostics

---

## Class System Commands

| Command | Description | Example |
|---------|-------------|---------|
| `[SetClassV2 <class>` | Set target's class | `[SetClassV2 IceMage` |
| `[RemoveClassV2` | Remove target's class | `[RemoveClassV2` |
| `[ClassInfoV2` | Show target's class info | `[ClassInfoV2` |
| `[ListClassesV2` | List all 26 classes | `[ListClassesV2` |

**Available Classes (26):**
- **Frosthold:** Barbarian, Beastmaster, IceMage
- **Emberlands:** Sorcerer
- **Desert:** Ranger, Illusionist
- **Shadowfen:** Witch
- **ShadowVoid:** Warlock, Necromancer
- **Verdantpeak:** Druid, Alchemist
- **Crystal Barrens:** Wizard, Oracle
- **Ironclad:** Artificer, Fighter, Monk, Templar
- **Underwater:** Summoner
- **Multi-Regional:** BountyHunter, Knight, Shaman, Cleric, Paladin, Bard, Enchanter

---

## Class Trainer Commands

| Command | Description | Example |
|---------|-------------|---------|
| `[spawntrainer <class>` | Spawn specific trainer at your location | `[spawntrainer IceMage` |
| `[str <class>` | Same as above (shortcut) | `[str Barbarian` |
| `[spawnalltrainers` | Spawn all 25 trainers in circle around you | `[spawnalltrainers` |
| `[sat` | Same as above (shortcut) | `[sat` |

**Notes:**
- Trainers handle class selection and quest giving
- Class names are case-insensitive
- All 25 trainers available (one per class, Rogue excluded)

---

## Skill Commands

| Command | Description | Example |
|---------|-------------|---------|
| `[rvs` | Reset all 26 Vystia skills to 0.0 | `[rvs` |
| `[resetvystiaskills` | Same as above (full name) | `[resetvystiaskills` |
| `[svs <value>` | Set all Vystia skills to value (0-120) | `[svs 50` |
| `[setvystiaskills <value>` | Same as above (full name) | `[setvystiaskills 100` |
| `[skillcap` | Show current skill cap and total | `[skillcap` |
| `[skillcap <value>` | Set skill cap | `[skillcap 84000` |
| `[skillinfo` | Show skill total breakdown | `[skillinfo` |

**Recommended skill cap for Vystia:** 84000 (8400 total points)

---

## Secondary Resource Commands

Manage class secondary resources (Fury, Chi, Soul Shards, etc.)

| Command | Description | Example |
|---------|-------------|---------|
| `[SetResource <type> <value>` | Set resource to specific value | `[SetResource Fury 100` |
| `[GetResources` | Show all resources for target | `[GetResources` |
| `[ResetResources` | Reset all resources to 0 | `[ResetResources` |
| `[TestResource <type>` | Test resource generation | `[TestResource Chi` |

**Resource Types (15):**
Fury, Chi, Combo, Energy, Focus, Essence, Mana, Nature, Shadow, Soul, Spirit, Rage, Heat, Chill, Harmony

---

## Stance Commands

| Command | Description | Example |
|---------|-------------|---------|
| `[SetStance <stance>` | Set active stance | `[SetStance Berserker` |
| `[RemoveStance` | Remove current stance | `[RemoveStance` |
| `[ListStances` | List all available stances | `[ListStances` |
| `[ClearStances` | Clear all stance effects | `[ClearStances` |
| `[StanceInfo` | Show current stance details | `[StanceInfo` |
| `[ResetStanceCooldowns` | Reset all stance cooldowns | `[ResetStanceCooldowns` |

**28 Stances Available** across 8 categories (Warrior, Rogue, Caster, etc.)

---

## Buff Commands

| Command | Description | Example |
|---------|-------------|---------|
| `[ApplyBuff <type> <duration> [power]` | Apply buff to target | `[ApplyBuff StrengthBoost 60 10` |
| `[RemoveBuff <type>` | Remove specific buff | `[RemoveBuff StrengthBoost` |
| `[ListBuffs` | List all active buffs on target | `[ListBuffs` |
| `[ClearBuffs` | Remove all buffs from target | `[ClearBuffs` |

**25+ Buff Types:** StrengthBoost, DexterityBoost, IntelligenceBoost, DamageShield, HealOverTime, DamageOverTime, SpeedBoost, DefenseBoost, etc.

---

## Crowd Control Commands

| Command | Description | Example |
|---------|-------------|---------|
| `[ApplyCC <type> <duration>` | Apply CC to target | `[ApplyCC Stun 5` |
| `[RemoveCC <type>` | Remove specific CC | `[RemoveCC Stun` |
| `[ListCC` | List active CC effects on target | `[ListCC` |
| `[CheckDR <type>` | Check diminishing returns for CC type | `[CheckDR Stun` |
| `[ResetDR` | Reset all diminishing returns | `[ResetDR` |

**15 CC Types:** Stun, Root, Silence, Fear, Sleep, Charm, Slow, Disarm, Blind, Polymorph, Knockback, Knockdown, Taunt, Confuse, Freeze

**Diminishing Returns:** Full → 50% → 25% → Immune (resets after 15 seconds)

---

## Ability Commands

| Command | Description | Example |
|---------|-------------|---------|
| `[TestAbility <id>` | Test execute an ability | `[TestAbility 2001` |
| `[ListAbilities [class]` | List abilities (optionally by class) | `[ListAbilities Fighter` |
| `[AbilityEditor` | Open ability editor gump | `[AbilityEditor` |
| `[AE` | Same as above (shortcut) | `[AE` |
| `[TestDummy` | Spawn combat test dummy | `[TestDummy` |
| `[TD` | Same as above (shortcut) | `[TD` |

**608 Total Abilities:**
- Magic Spells: 384 (IDs 1000-1383)
- Martial Abilities: 224 (IDs 2000-2223)

---

## Damage System Commands

| Command | Description | Example |
|---------|-------------|---------|
| `[TestDamage <amount> [type]` | Deal test damage to target | `[TestDamage 50 Fire` |
| `[TestHeal <amount>` | Heal target for amount | `[TestHeal 100` |
| `[TestCrit` | Force critical hit test | `[TestCrit` |

**Damage Types:** Physical, Fire, Cold, Poison, Energy

---

## Target Tracker Commands

| Command | Description | Example |
|---------|-------------|---------|
| `[GetTargetStacks <type>` | Get stacks on target | `[GetTargetStacks Bleed` |
| `[SetTargetStacks <type> <count>` | Set stacks on target | `[SetTargetStacks Poison 5` |
| `[ClearTargetStacks` | Clear all stacks on target | `[ClearTargetStacks` |
| `[TestMark <type>` | Apply test mark | `[TestMark Hunter` |

---

## Spawn Commands

### Creature Spawning

| Command | Description | Example |
|---------|-------------|---------|
| `[spawnvystia` | Open spawn gump (creatures, items, vendors) | `[spawnvystia` |
| `[spawnvystia <radius>` | Open spawn gump with radius | `[spawnvystia 10` |
| `[clearvystia` | Delete Vystia creatures in area | `[clearvystia` |
| `[clearvystia <radius>` | Delete within radius | `[clearvystia 20` |

### Spawn Gump Pages

The `[spawnvystia` gump has 3 pages:
1. **Creatures** - All 138 Vystia creatures by region
2. **Magic Items** - Spellbooks, reagents, scrolls
3. **Vendors** - All 15 Vystia vendors

---

## Spellbook Commands

| Command | Description | Example |
|---------|-------------|---------|
| `[spellbook <type>` | Give specific spellbook | `[spellbook ice` |
| `[sb <type>` | Same as above (short form) | `[sb druid` |

### Spellbook Types

| Type | Spellbook | Spell IDs |
|------|-----------|-----------|
| `ice` | IceMageSpellbook | 1000-1031 |
| `druid` / `nature` | DruidSpellbook | 1032-1063 |
| `witch` / `hex` | WitchSpellbook | 1064-1095 |
| `sorcerer` / `elemental` | SorcererSpellbook | 1096-1127 |
| `warlock` / `dark` | WarlockSpellbook | 1128-1159 |
| `oracle` / `divination` | OracleSpellbook | 1160-1191 |
| `necromancer` / `necro` | NecromancerSpellbook | 1192-1223 |
| `summoner` / `summon` | SummonerSpellbook | 1224-1255 |
| `shaman` / `shamanic` | ShamanSpellbook | 1256-1287 |
| `bard` / `bardic` | BardSpellbook | 1288-1319 |
| `enchanter` / `enchant` | EnchanterSpellbook | 1320-1351 |
| `illusionist` / `illusion` | IllusionistSpellbook | 1352-1383 |

---

## Vendor Commands

| Command | Description |
|---------|-------------|
| `[VystiaReagents` | Spawn reagent vendor (all 96 reagents) |
| `[vreag` | Same as above (shortcut) |
| `[VystiaResources` | Spawn resource vendor (ores, ingots) |
| `[vres` | Same as above (shortcut) |

**Note:** Magic school vendors can be spawned via `[spawnvystia` gump > Vendors page.

---

## Reagent Bag Commands

| Command | Description |
|---------|-------------|
| `[spawnreagentbags` | Spawn all 12 reagent bags (one per school) |
| `[srb` | Same as above (shortcut) |

Each bag contains a full set of reagents for one magic school.

---

## AI Sidekick Commands

| Command | Description |
|---------|-------------|
| `[st` | Spawn Tamer sidekick |
| `[SpawnMage` | Spawn Mage sidekick |
| `[SpawnSidekick` | Spawn any archetype sidekick |
| `[ol` | Spawn Arctic Ogre Lord for testing |

### Sidekick Archetypes
- **Mage** - Kiting, ranged magic attacks
- **Warrior** - Tank, melee combat
- **Tamer** - Pet support and healing

---

## Dwarf Commands

| Command | Description |
|---------|-------------|
| `[sd` | Spawn Dwarf (random gender) |
| `[sdm` | Spawn Male Dwarf |
| `[sdf` | Spawn Female Dwarf |

**Note:** Dwarf race uses custom bodies 987 (male) and 988 (female) with 75% scale sprites.

---

## LLM NPC Commands

### Spawning

| Command | Description | Example |
|---------|-------------|---------|
| `[SpawnPersonalityNPC <type> <pattern>` | Spawn NPC with personality | `[SpawnPersonalityNPC Mage Archaic` |
| `[SpawnTownNPCs` | Spawn town NPC group | `[SpawnTownNPCs` |
| `[SpawnMagicNPCs` | Spawn magic NPC group | `[SpawnMagicNPCs` |
| `[SpawnAdventurerNPCs` | Spawn adventurer NPC group | `[SpawnAdventurerNPCs` |

### Testing

| Command | Description |
|---------|-------------|
| `[KnowledgeTest` | Run comprehensive knowledge test |
| `[LocationTest` | Run location-based response test |
| `[MemoryTest` | Run memory system validation |
| `[IntegrationTest` | Run full integration test |

### Memory & Stats

| Command | Description | Example |
|---------|-------------|---------|
| `[MemoryStats` | Show memory system stats | `[MemoryStats` |
| `[ViewMemories <npc> <player>` | View NPC memories of player | `[ViewMemories` |
| `[ViewRelationship <npc> <player>` | View relationship score | `[ViewRelationship` |
| `[ClearMemories <npc> <player>` | Clear memories | `[ClearMemories` |
| `[LoreStats` | Show lore system stats | `[LoreStats` |
| `[CacheStats` | Show cache statistics | `[CacheStats` |

### Personality Types (54 total)

Common types: Mage, Blacksmith, Merchant, Guard, Healer, Innkeeper, Farmer, Sailor, etc.

### Speech Patterns (6 types)

- **Archaic** - Old English style
- **Formal** - Proper, educated
- **Casual** - Relaxed, friendly
- **Gruff** - Short, blunt
- **Mystical** - Cryptic, prophetic
- **OldEnglish** - Ye olde style

---

## Quest Management Commands

**Note:** These commands target **Vystia Dynamic Quests** (QuestNPC/Chronicler).  
The **Mondain/BaseQuest** system uses classic quest givers (MondainQuester/LLMQuester).

### Quest Editor & Creation

| Command | Description | Example |
|---------|-------------|---------|
| `[QuestEditor` / `[QE` | Open quest editor gump | `[QE` |
| `[addquestNPC` / `[aqn` | Open quest NPC spawn wizard | `[aqn` |

**Quest Editor Features:**
- Create quests without coding
- Set title, description, class requirement, tier
- Add waypoints (Origin, Waypoint, BossCompletion, NPCCompletion)
- Add objectives (TalkToNPC, ReachLocation, DefeatBoss, etc.)
- Add rewards (gold, skill points, items, titles)
- Give quests directly to players
- Quests auto-save to Data/VystiaQuests.xml

### Quest NPC Management

| Command | Description | Example |
|---------|-------------|---------|
| `[FindQuestNPC` / `[FQNPC` | Find quest NPCs for active quests | `[FQNPC` |
| `[FindQuestNPC respawn` | Respawn missing quest NPCs | `[FQNPC respawn` |

**Usage:**
- Shows all NPCs for your active quests and their locations
- Displays distance to each NPC
- Use `respawn` parameter to recreate missing NPCs at waypoint locations

### Quest Clearing

| Command | Description | Example |
|---------|-------------|---------|
| `[ClearQuests` | Clear all your own quests | `[ClearQuests` |
| `[ClearQuests <playerName>` | Clear all quests for a player | `[ClearQuests Runescry` |

**What it clears:**
- All active Vystia quests
- All completed Vystia quests
- All LLM-generated ephemeral quest instances

### LLM Quest Generation

| Command | Description | Example |
|---------|-------------|---------|
| `[GenLLMQuest [poiId]` | Generate quest using LLM | `[GenLLMQuest VERDANTPEAK_GROVE` |

**Usage:**
- Generates a dynamic quest plan using LLM
- Optionally specify a POI ID to anchor the quest
- Creates ephemeral quests that expire after set time
- See `QUEST_GENERATION_QUICK_REF.md` for POI list

---

## Pet System Commands

Manage class pets (Summoner, Necromancer, Beastmaster, Artificer).

### Pet Spawning

| Command | Description | Example |
|---------|-------------|---------|
| `[SummonPet <type>` | Summon a class pet | `[SummonPet WaterElemental` |
| `[SP <type>` | Same as above (shortcut) | `[SP SteamGolem` |
| `[DismissPets` | Dismiss all your pets | `[DismissPets` |
| `[DP` | Same as above (shortcut) | `[DP` |

### Pet Information

| Command | Description | Example |
|---------|-------------|---------|
| `[PetInfo` | Show info about your current pets | `[PetInfo` |
| `[PI` | Same as above (shortcut) | `[PI` |
| `[PetList <class>` | List available pets for a class | `[PetList Summoner` |
| `[PL <class>` | Same as above (shortcut) | `[PL Necromancer` |

### Pet Types by Class

| Class | Available Pets |
|-------|----------------|
| **Summoner** | WaterElemental, EarthElemental, FireElemental, AirElemental |
| **Necromancer** | SkeletonWarrior, SkeletonMage, Zombie, Wraith, BoneKnight |
| **Beastmaster** | Wolf, Bear, Boar, SnowLeopard, IceWyrm |
| **Artificer** | ClockworkScout, SteamGolem, GearConstruct, AutomatonGuard |

**Notes:**
- Pet limits vary by class (1-4 active pets)
- Pets scale with owner's skills
- Dismissing pets refunds partial resource cost

---

## Housing System Commands

Manage Vystia housing costs and property taxes.

### Player Commands

| Command | Description |
|---------|-------------|
| `[HouseCosts` | Display housing prices and weekly taxes |
| `[HC` | Same as above (shortcut) |
| `[TaxInfo` | Show your current tax status |
| `[TI` | Same as above (shortcut) |
| `[PayTax` | Pay outstanding property taxes |
| `[PT` | Same as above (shortcut) |

### GM Commands

| Command | Description | Example |
|---------|-------------|---------|
| `[SetHousePrice <amount>` | Set price of targeted house | `[SetHousePrice 100000` |
| `[SHP <amount>` | Same as above (shortcut) | `[SHP 50000` |
| `[HouseInfo` | Display full info for targeted house | `[HouseInfo` |
| `[HI` | Same as above (shortcut) | `[HI` |
| `[TaxExempt` | Toggle tax exemption for targeted house | `[TaxExempt` |
| `[TE` | Same as above (shortcut) | `[TE` |
| `[ForceTaxCollection` | Force immediate tax collection cycle | `[ForceTaxCollection` |
| `[FTC` | Same as above (shortcut) | `[FTC` |
| `[TaxStatus` | Show server-wide tax statistics | `[TaxStatus` |
| `[TS` | Same as above (shortcut) | `[TS` |

### Housing Costs

| Size | Dimensions | Purchase Price | Weekly Tax |
|------|------------|----------------|------------|
| Small | 7×7 | 50,000g | 500g |
| Medium | 11×11 | 150,000g | 1,500g |
| Large | 15×15 | 400,000g | 4,000g |
| Keep | 18×18 | 1,000,000g | 10,000g |
| Castle | 31×31 | 3,000,000g | 30,000g |

**Tax Notes:**
- Taxes collected weekly on Sunday 00:00 UTC
- 7-day grace period before house condemnation
- Taxes deducted from bank account automatically

---

## Zone Control Commands

Manage PvP zones and regional rules.

### Player Commands

| Command | Description |
|---------|-------------|
| `[ZoneInfo` | Show current zone type and rules |
| `[ZI` | Same as above (shortcut) |
| `[TogglePvP` | Toggle PvP consent (for Contested zones) |

### GM Commands

| Command | Description | Example |
|---------|-------------|---------|
| `[SetZone <type>` | Set zone type for region | `[SetZone Lawless` |
| `[SZ <type>` | Same as above (shortcut) | `[SZ Sanctuary` |
| `[ZoneList` | List all Vystia zones and their types | `[ZoneList` |
| `[ZL` | Same as above (shortcut) | `[ZL` |
| `[CreateZone <name> <type>` | Create new zone region | `[CreateZone Bandit_Camp Lawless` |
| `[DeleteZone <name>` | Delete a zone region | `[DeleteZone Bandit_Camp` |

### Zone Types

| Zone | PvP | Death Penalty | Loot Drop | XP/Gold Multiplier |
|------|-----|---------------|-----------|-------------------|
| **Sanctuary** (Green) | No PvP | None | 0% | 0.75x |
| **Contested** (Yellow) | Consent | 5% skill loss | 10% | 1.0x |
| **Lawless** (Red) | Always On | 10% skill loss | 25% | 1.25x |
| **Extreme** (Black) | Always On | 15% skill loss | 50% | 1.5x |

**Zone Features:**
- Guards only respond in Sanctuary zones
- Contested zones require both players to flag for PvP
- Higher risk = higher rewards in dangerous zones

---

## Faction System Commands

Manage faction reputation and vendors.

### Player Commands

| Command | Description |
|---------|-------------|
| `[Factions` | Show your reputation with all factions |
| `[FactionInfo <faction>` | Show detailed faction info |
| `[DonateFaction <faction> <amount>` | Donate gold for reputation |

### GM Commands

| Command | Description | Example |
|---------|-------------|---------|
| `[SetReputation <faction> <value>` | Set player reputation | `[SetReputation Ironclad 5000` |
| `[SR <faction> <value>` | Same as above (shortcut) | `[SR Frosthold 10000` |
| `[AddReputation <faction> <value>` | Add to reputation | `[AddReputation Verdantpeak 500` |
| `[AR <faction> <value>` | Same as above (shortcut) | `[AR Emberlands 1000` |
| `[ResetFactions` | Reset all faction reputations | `[ResetFactions` |
| `[SpawnFactionVendor <faction>` | Spawn faction vendor | `[SpawnFactionVendor Ironclad` |

### Reputation Tiers

| Tier | Reputation | Vendor Discount |
|------|------------|-----------------|
| Hostile | -3000 to -1000 | +25% prices |
| Unfriendly | -999 to -1 | +10% prices |
| Neutral | 0 to 999 | No discount |
| Friendly | 1000 to 2999 | -5% discount |
| Honored | 3000 to 7999 | -10% discount |
| Revered | 8000 to 14999 | -12% discount |
| Exalted | 15000+ | -15% discount |

### Major Factions

| Faction | Region | Specialty |
|---------|--------|-----------|
| Frosthold Clans | Frosthold | Ice magic, furs |
| Ember Legion | Emberlands | Fire magic, smithing |
| Desert Traders | Desert of Surya | Trade goods, artifacts |
| Shadowfen Circle | Shadowfen | Poisons, dark magic |
| Verdant Keepers | Verdantpeak | Nature magic, herbs |
| Crystal Seekers | Crystal Barrens | Crystals, divination |
| Ironclad Engineers | Ironclad Wastes | Engineering, constructs |

**Reputation Gains:**
- Quest completion: +50 to +500
- Boss kills: +100 to +500
- Donations: +1 per 20g donated
- Wearing faction tabard: +10% quest rep

---

## Test Spell Commands (Player Level)

| Command | Description |
|---------|-------------|
| `[frostmeteor` | Cast Frost Meteor (Circle 8) |
| `[iceage` | Cast Ice Age (Circle 8) |
| `[blizzard` | Cast Blizzard (Circle 7) |
| `[absolutezero` | Cast Absolute Zero (Circle 8) |

**Note:** These are player-level commands for testing high-circle ice spells.

---

## Standard ServUO Commands (Useful)

| Command | Description |
|---------|-------------|
| `[skills` | Open GM skill editor for target |
| `[set <property> <value>` | Set property on target |
| `[get <property>` | Get property from target |
| `[add <type>` | Add item/mobile to world |
| `[delete` | Delete target |
| `[props` | Open properties gump for target |
| `[move` | Move target to cursor location |
| `[tele` | Teleport to cursor location |
| `[go <location>` | Go to named location |

---

## Economy Commands

Manage gold sinks: repair costs, service fees, and NPC services.

### Repair Service

| Command | Description | Example |
|---------|-------------|---------|
| `[RepairCost` | Display repair cost formula | `[RepairCost` |
| `[RC` | Same as above (shortcut) | `[RC` |
| `[SpawnBlacksmith` | Spawn blacksmith with Vystia repair service | `[SpawnBlacksmith` |
| `[SBS` | Same as above (shortcut) | `[SBS` |
| `[TestRepair` | Test repair on equipped weapon | `[TestRepair` |
| `[TR` | Same as above (shortcut) | `[TR` |

### Service NPCs

| Command | Description | Example |
|---------|-------------|---------|
| `[SpawnHealer` | Spawn healer with resurrection service | `[SpawnHealer` |
| `[SH` | Same as above (shortcut) | `[SH` |
| `[SpawnStablemaster` | Spawn stablemaster with pet stabling | `[SpawnStablemaster` |
| `[SSM` | Same as above (shortcut) | `[SSM` |
| `[ServiceFees` | Display all service fee rates | `[ServiceFees` |
| `[SF` | Same as above (shortcut) | `[SF` |

### Service Fee Rates

| Service | Cost | Notes |
|---------|------|-------|
| **Resurrection** | 50g | Base cost at NPC healer |
| **Full Heal** | 25g | Restore full HP at healer |
| **Cure Poison** | 15g | Remove poison at healer |
| **Moongate Travel** | 100-250g | Based on distance |
| **Pet Stabling** | 30g/week | Per pet, auto-deducted |
| **Repair (Standard)** | 2g/durability | Iron/standard equipment |
| **Repair (Regional T1)** | 35g/durability | Regional materials |
| **Repair (Regional T2)** | 50g/durability | Higher tier materials |
| **Repair (Legendary)** | 75g/durability | Legendary equipment |

**File Locations:**
- Repair Service: `Scripts/Custom/VystiaClasses/Economy/VystiaRepairService.cs`
- Service Fees: `Scripts/Custom/VystiaClasses/Economy/VystiaServiceFees.cs`
- Commands: `Scripts/Custom/VystiaClasses/Economy/Commands/`

---

## Religion Commands

Manage the piety/religion system with 6 religions and devotion powers.

### Player Commands

| Command | Description | Example |
|---------|-------------|---------|
| `[Religion` | Show your religion and piety status | `[Religion` |
| `[Pray` | Perform daily prayer (+10 piety) | `[Pray` |
| `[Tithe <amount>` | Donate gold for piety (+1 per 100g) | `[Tithe 1000` |

### GM Commands

| Command | Description | Example |
|---------|-------------|---------|
| `[SetReligion <religion>` | Set player's religion | `[SetReligion Solarius` |
| `[SetPiety <value>` | Set player's piety level | `[SetPiety 500` |
| `[AddPiety <value>` | Add to player's piety | `[AddPiety 100` |
| `[SpawnShrine <religion>` | Spawn religion shrine | `[SpawnShrine Lunara` |
| `[ReligionInfo` | Show detailed religion info | `[ReligionInfo` |
| `[RI` | Same as above (shortcut) | `[RI` |

### Religions & Domains

| Religion | Domain | Bonus at 200 Piety |
|----------|--------|-------------------|
| **Solarius** | Sun, Light, Fire | +5% Fire damage |
| **Lunara** | Moon, Night, Dreams | +5% Mana regen |
| **Terrath** | Earth, Stone, Mountains | +5 Physical resist |
| **Aquos** | Water, Ocean, Storms | +5% Cold resist |
| **Sylvanis** | Nature, Forest, Animals | +5% Healing received |
| **Mortis** | Death, Spirits, Transition | +5% Necro damage |

### Piety Tiers

| Tier | Piety | Benefits |
|------|-------|----------|
| **Initiate** | 0-49 | None |
| **Faithful** | 50-199 | Access to daily prayer |
| **Devoted** | 200-499 | Passive bonus active |
| **Blessed** | 500-899 | Can craft blessed items |
| **Exalted** | 900-1000 | Devotion Power unlocked |

### Piety Gains

| Action | Piety Gain | Notes |
|--------|------------|-------|
| Daily Prayer | +10 | Once per real day |
| Tithe Gold | +1 per 100g | Daily cap: 3,000g (30 piety) |
| Pilgrimage | +75 | Visit specific shrine |
| Quest (religion) | +25 to +100 | Religion-specific quests |

**File Locations:**
- Religion System: `Scripts/Custom/VystiaClasses/Religion/VystiaReligionSystem.cs`
- Shrine NPCs: `Scripts/Custom/VystiaClasses/Religion/VystiaShrine.cs`
- Commands: `Scripts/Custom/VystiaClasses/Religion/Commands/`

---

## File Locations

| Category | Path |
|----------|------|
| VystiaAdmin Gump | `Scripts/Custom/VystiaClasses/Gumps/VystiaAdminGump.cs` |
| Skill Commands | `Scripts/Custom/VystiaClasses/Commands/VystiaSkillCommands.cs` |
| Spawn Commands | `Scripts/Custom/Commands/SpawnVystiaCommand.cs` |
| Spellbook Commands | `Scripts/Custom/Commands/VystiaSpellbookCommands.cs` |
| Trainer Commands | `Scripts/Custom/Commands/SpawnClassTrainersCommand.cs` |
| Vendor Commands | `Scripts/Custom/Commands/VystiaVendorCommands.cs` |
| Reagent Bags | `Scripts/Custom/Commands/SpawnReagentBagsCommand.cs` |
| Class System | `Scripts/Custom/VystiaClasses/Classes/PlayerClassV2.cs` |
| Resource System | `Scripts/Custom/VystiaClasses/Systems/VystiaResourceManager.cs` |
| Buff System | `Scripts/Custom/VystiaClasses/Systems/VystiaBuffSystem.cs` |
| Stance System | `Scripts/Custom/VystiaClasses/Systems/StanceSystem.cs` |
| CC System | `Scripts/Custom/VystiaClasses/Systems/CrowdControlSystem.cs` |
| Damage System | `Scripts/Custom/VystiaClasses/Systems/VystiaDamageSystem.cs` |
| Target Tracker | `Scripts/Custom/VystiaClasses/Systems/TargetTracker.cs` |
| Ability System | `Scripts/Custom/VystiaClasses/Abilities/AbilityExecutor.cs` |
| Sidekick Commands | `Scripts/Services/AISidekicks/Commands/` |
| Dwarf Commands | `Scripts/Mobiles/Vystia/Races/` |
| LLM Commands | `Scripts/Services/LLM/Commands/` |
| Quest Commands | `Scripts/Custom/VystiaClasses/Quests/Commands/` |
| Quest Generation | `Scripts/Custom/VystiaClasses/Quests/Generation/Commands/` |
| Pet System | `Scripts/Custom/VystiaClasses/Pets/` |
| Housing System | `Scripts/Custom/VystiaClasses/Housing/` |
| Zone Control | `Scripts/Custom/VystiaClasses/Zones/` |
| Faction System | `Scripts/Custom/VystiaClasses/Factions/` |
| Economy System | `Scripts/Custom/VystiaClasses/Economy/` |
| Religion System | `Scripts/Custom/VystiaClasses/Religion/` |

---

## Command Count Summary

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
| Quest Management | 6 |
| Pet System | 8 |
| Housing System | 12 |
| Zone Control | 8 |
| Faction System | 9 |
| Economy/Services | 12 |
| Religion System | 9 |
| Test Spells | 4 |
| **Total** | **147** |

---

*Source of Truth: This file for Vystia-specific commands.*
*For quick class/spell/skill lookups, see other files in `Vystia/reference/`*
