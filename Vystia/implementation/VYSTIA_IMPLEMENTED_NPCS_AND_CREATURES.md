# Vystia Implemented NPCs and Creatures

**Generated:** 2025-12-12  
**Source:** ServUO Scripts/Mobiles/Vystia  
**Purpose:** Complete catalog of all NPCs and creatures actually implemented in the codebase

---

## Table of Contents

1. [Overview](#overview)
2. [Faction Leaders](#faction-leaders)
3. [Talking Creatures (Ancient Beings)](#talking-creatures-ancient-beings)
4. [Quest Givers](#quest-givers)
5. [Vendors](#vendors)
6. [Bosses](#bosses)
7. [Regional Creatures](#regional-creatures)
8. [Miscellaneous Creatures](#miscellaneous-creatures)
9. [Implementation Details](#implementation-details)

---

## Overview

This document catalogs all NPCs and creatures that are **actually implemented** in the ServUO codebase under `Scripts/Mobiles/Vystia/`. This is distinct from design documents or registry files - these are the actual C# classes that exist and can be spawned in-game.

### Key Features

- **LLM Integration**: Many NPCs implement `ILLMConversational` for dynamic dialogue
- **Quest System**: NPCs can be linked to quest waypoints via `QuestNPC` class
- **Regional Identity**: Creatures are organized by Vystia regions
- **Boss Mechanics**: Bosses have phase-based combat systems
- **Vendor Support**: NPCs can serve as vendors using `BaseVendor`

---

## Faction Leaders

**Location:** `NPCs/FactionLeaders/`  
**Base Class:** `BaseVendor`  
**LLM Support:** ✅ All implement `ILLMConversational`

### 1. Emperor Garrick Steelarm
- **Faction:** Ironclad Alliance
- **Location:** Imperial Palace, Ironhaven
- **Personality:** Visionary leader, strategic genius, pragmatic
- **Speech Pattern:** Formal
- **Body:** Human (0x190)
- **Hue:** 2213 (Ironclad colors)
- **Stats:** High (Str 150-200, Int 150-200, Hits 500-700)
- **Skills:** Magery 100, MagicResist 100, Tactics 100, Wrestling 100
- **Keywords:** "greetings", "hail", "hello", "faction", "alliance"

### 2. Chieftain Bjorn Frostbeard
- **Faction:** Polar Alliance (Frosthold)
- **Location:** Frost Palace, Frostholm
- **Personality:** Legendary warrior, honorable
- **Speech Pattern:** Gruff
- **Note:** ⚠️ **LORE MISMATCH** - Registry says "King Frostbeard" but code has "Chieftain Bjorn Frostbeard"

### 3. Elder Seraphina Leafwhisper
- **Faction:** Sylvan Concord (Verdantpeak)
- **Location:** Heart Tree, Verdantheart
- **Personality:** Ancient elf, wise, protective
- **Speech Pattern:** Formal
- **Note:** ⚠️ **LORE MISMATCH** - Registry says "Queen Amaryllis" but code has "Elder Seraphina Leafwhisper"

### 4. Sultan Azir al-Rashid
- **Faction:** League of Sands
- **Location:** Palace of Sun and Sand, Sunspire
- **Personality:** Shrewd merchant, neutral diplomat
- **Speech Pattern:** Formal
- **Note:** ⚠️ **LORE MISMATCH** - Registry says "Sheikh Tarik" but code has "Sultan Azir al-Rashid"

### 5. Archmage Pyrus Ashborn
- **Faction:** Ironclad Alliance (Emberlands)
- **Location:** Magma Citadel, Emberforge
- **Personality:** Powerful fire sorcerer, ambitious
- **Speech Pattern:** Formal
- **Note:** ⚠️ **LORE MISMATCH** - Registry says "Warlord Emberon Flamefist" but code has "Archmage Pyrus Ashborn"

**GM Command:** `[SpawnVystiaLeader <name>` where name is: garrick, bjorn, seraphina, azir, pyrus

---

## Talking Creatures (Ancient Beings)

**Location:** `NPCs/TalkingCreatures/`  
**Base Class:** `BaseCreature`  
**LLM Support:** ✅ All implement `ILLMConversational`  
**Combat:** Most are non-aggressive (`FightMode.Aggressor`)

### 1. Abyssus the Depth King
- **Type:** Sea Dragon
- **Location:** Underwater/Forgotten Depths
- **Personality:** Ancient
- **Speech Pattern:** OldEnglish
- **Body:** Dragon (varies)
- **Age:** Ancient (3000+ years)

### 2. Crystalwing the Prismatic Oracle
- **Type:** Crystal Dragon
- **Location:** Crystal Barrens
- **Personality:** Sage
- **Speech Pattern:** Cryptic
- **Special:** Oracle abilities, crystal-themed

### 3. Elder Oakbark
- **Type:** Ancient Treant
- **Location:** Deep Verdantpeak Forest
- **Personality:** Hermit (patient, wise, protective)
- **Speech Pattern:** OldEnglish
- **Body:** 0x2F (Treant)
- **Hue:** 2010 (forest green)
- **Stats:** Very High (Str 800-1000, Int 500-700, Hits 5000-7000)
- **Skills:** MagicResist 120, Magery 120, EvalInt 120
- **Keywords:** "hello", "greetings", "age", "old"
- **Age:** 2000+ years

### 4. Emberflame the Ashen Tyrant
- **Type:** Fire Dragon
- **Location:** Emberlands
- **Personality:** Tyrant
- **Speech Pattern:** Formal
- **Special:** Fire-themed, volcanic

### 5. Frost Father's Avatar
- **Type:** Divine Ice Spirit
- **Location:** Frosthold
- **Personality:** Divine
- **Speech Pattern:** OldEnglish
- **Special:** Avatar of The Frost Father god

### 6. Frosthelm the Eternal Winter
- **Type:** White Ancient Dragon
- **Location:** Frozen Peak, Frosthold
- **Personality:** Ancient, wise, protective
- **Speech Pattern:** OldEnglish
- **Age:** 3000+ years

### 7. Great Machinist's Construct
- **Type:** Divine Clockwork Avatar
- **Location:** Ironclad Empire
- **Personality:** Analytical
- **Speech Pattern:** Formal
- **Special:** Avatar of The Great Machinist god, mechanical construct

### 8. Ironbark the War-Ancient
- **Type:** War Treant
- **Location:** Verdantpeak
- **Personality:** Warrior
- **Speech Pattern:** Gruff
- **Special:** Combat-focused treant

### 9. Lunara's Dryad Herald
- **Type:** Divine Nature Spirit (Dryad)
- **Location:** Verdantpeak
- **Personality:** Divine
- **Speech Pattern:** Formal
- **Special:** Herald of Lunara goddess

### 10. Sphynx of Emberlands
- **Type:** Desert Sphinx
- **Location:** Emberlands
- **Personality:** Riddler
- **Speech Pattern:** Cryptic
- **Special:** Riddle master, fire-themed

### 11. The Crystal Sphinx
- **Type:** Crystalline Riddle Master
- **Location:** Crystal Barrens
- **Personality:** Riddler
- **Speech Pattern:** Cryptic
- **Special:** Crystal-themed sphinx

### 12. Verdantheart Forest Guardian
- **Type:** Forest Guardian
- **Location:** Verdantpeak
- **Personality:** Guardian
- **Speech Pattern:** Formal
- **Special:** Protector of the forest

**GM Spawn:** Available via `VystiaNPCSpawnPickerGump` (TalkingCreatures category)

---

## Quest Givers

**Location:** `NPCs/QuestGivers/`  
**Base Class:** `MondainQuester` (Mondain's Legacy quest system)

### 1. Quartermaster Grimwald
- **Location:** Ironhaven Barracks
- **Personality:** Gruff military veteran
- **Quest:** `SupplyLineQuest`
- **Body:** Human (0x190)
- **Hue:** 2213
- **Appearance:** Robe, boots, quarterstaff
- **Lore Context:** Veteran quartermaster overseeing supply lines for Ironclad military

### 2. Sage Theron
- **Location:** (Not specified in code)
- **Personality:** Sage
- **Quest:** (Not specified in code)
- **Lore Context:** Scholar and knowledge keeper

**Note:** These use the Mondain's Legacy quest system, not the Dynamic Quest system. For Dynamic Quests, use `QuestNPC` class instead.

---

## Vendors

**Location:** `NPCs/Vendors/` and `Vendors/`  
**Base Class:** `BaseVendor`

### Essential City NPCs

#### 1. Frostholm Healer
- **Location:** Frostholm
- **Type:** Healer
- **Services:** Healing, resurrections

#### 2. Ironhaven Banker
- **Location:** Ironhaven
- **Type:** Banker
- **Services:** Gold storage, banking

#### 3. Ironhaven Guard Captain
- **Location:** Ironhaven
- **Type:** Guard Captain
- **Services:** City protection, law enforcement

### Class-Specific Vendors

**Location:** `Vendors/MagicSchoolVendors.cs`

All implement `BaseVendor` and sell class-specific items:

1. **IceMageVendor** - Frostweavers of Winterguard
2. **DruidVendor** - Guardians of Verdantpeak
3. **WitchVendor** - Covens of Shadowfen
4. **SorcererVendor** - Elemental Savants of The Emberlands
5. **WarlockVendor** - Covenant of the Hidden Eye
6. **OracleVendor** - Seers of the Crystal Barrens
7. **NecromancerVendor** - Shadowbinders of Eternal Twilight
8. **SummonerVendor** - Arcane Summoners of the Forgotten Depths
9. **ShamanVendor** - Spiritcallers of The Wilderlands
10. **BardVendor** - Steam Harmonic Virtuosos
11. **EnchanterVendor** - Sigilmasters of the Mystic Canyons
12. **IllusionistVendor** - Mirage Weavers of The Whispering Sands

### Resource Vendors

#### VystiaResourceVendor
- **Type:** General resource vendor
- **Services:** Sells common resources

#### VystiaReagentVendor
- **Type:** Reagent vendor
- **Services:** Sells magical reagents

#### VystiaClassItemVendor
- **Type:** Class-specific item vendor
- **Services:** Sells items for Vystia classes

---

## Bosses

**Location:** `Bosses/`  
**Base Class:** `BaseCreature`  
**Combat:** Aggressive, high-level encounters

### 1. Frost Father
- **Location:** Frozen Halls dungeon, Frosthold
- **Title:** Guardian of the Frozen Halls / Eternal Winter
- **Body:** Titan (0x76)
- **Hue:** 1152 (ice blue)
- **Stats:** Very High (Str 600-700, Int 100-150, Hits 1200-1500)
- **Damage:** 35-45 (50% Physical, 50% Cold)
- **Resistances:** Cold 90-100, Physical 65-75, Fire 20-30 (weakness)
- **Special Abilities:**
  - **Phase 1 (100-66% HP):** Normal combat with cold aura
  - **Phase 2 (66-33% HP):** Frost Shield activates, summons Frost Wraiths (2), increased resistances
  - **Phase 3 (33-0% HP):** Cone Freeze attack, increased damage (40-55), summons 3 Frost Wraiths
  - **Cold Aura:** Damages nearby enemies (8-15 damage, 12-20 in Phase 3)
  - **Cone Freeze:** Freezes targets in front for 3 seconds, deals 50-70 damage
  - **Frostbite:** 25% chance on melee to reduce Dex by 20 for 10 seconds
- **Loot:**
  - Guaranteed: 1-3 Frozen Artifacts
  - 10% chance: Additional 2-4 Frozen Artifacts
  - 1% chance: Heartwood Core Fragment (ultra-rare)
  - 2% chance: The Eternal Winter (legendary weapon)
  - VystiaLootPack.FrostholdBoss
  - LootPack.FilthyRich x2
- **Treasure Map Level:** 5
- **Immunities:** Unprovokable, BardImmune, PoisonImmune (Lethal)

### 2. Forge Master
- **Location:** Great Forge, Ironclad Empire
- **Title:** Master of the Great Forge
- **Special:** Represents The Forgemaster god

### 3. Coven Matriarch
- **Location:** Shadowfen
- **Title:** Witch Queen of Shadowfen
- **Special:** Leader of the Covens

### 4. Volcano Wyrm
- **Location:** Emberlands
- **Title:** Guardian of the Ember Core
- **Special:** Fire/volcanic themed

### 5. Timeworn Lich
- **Location:** ShadowVoid
- **Title:** Ancient Necromancer
- **Special:** Undead, necromancy-themed

### 6. Ancient Treant
- **Location:** Verdantpeak
- **Title:** Forest Guardian
- **Special:** Nature/forest themed

### 7. Crystal Drake Alpha
- **Location:** Crystal Barrens
- **Title:** Prismatic Guardian
- **Special:** Crystal-themed drake

### 8. Sphinx of Surya
- **Location:** The Whispering Sands
- **Title:** Desert Riddle Master
- **Special:** Represents Surya god, riddle-themed

### 9. Ancient Kraken
- **Location:** Underwater/Forgotten Depths
- **Title:** Terror of the Depths
- **Special:** Ocean/underwater themed

### 10. Griffin Lord
- **Location:** Skyreach Mountains
- **Title:** Sky Sovereign
- **Special:** Sky/wind themed

**GM Spawn:** Available via `VystiaNPCSpawnPickerGump` (Bosses category)

---

## Regional Creatures

Creatures are organized by Vystia region. All extend `BaseCreature`.

### Frosthold (Frozen Tundra)
**Location:** `Frosthold/`

1. **FrostElemental** - Crystalline ice beings
2. **FrostGiant** - Massive humanoids (15ft tall, blue/white skin)
3. **FrostWraith** - Ethereal ice spirits
4. **FrozenHorror** - Colossal beast with white fur, wolf-like face
5. **GlacialBear** - Arctic bear
6. **IceGolem** - Construct of ice
7. **IceSprite** - Small ice spirits
8. **IceTroll** - Bulky trolls with glacier-blue skin
9. **Snowdrifter** - Snow-themed creature
10. **VystiaArcticOgre** - Arctic ogre variant
11. **VystiaFrostDragon** - Frost-themed dragon
12. **WinterWolf** - Large wolves with white fur, blue eyes

### Verdantpeak (Forests)
**Location:** `Verdantpeak/`

1. **ForestBear** - Forest-dwelling bear
2. **ForestGuardian** - Protector of the forest
3. **ForestSerpent** - Forest snake
4. **ForestSprite** - Small forest spirits
5. **GiantStag** - Large deer
6. **MossGolem** - Golem covered in moss
7. **NatureElemental** - Elemental of nature
8. **ThornVine** - Animated thorny vines
9. **TreantSapling** - Young treant
10. **VineCreeper** - Creeping vines
11. **VystiaWoodlandDrake** - Forest drake
12. **WildBoar** - Wild pig
13. **WildcatAlpha** - Alpha wildcat

### The Whispering Sands (Desert)
**Location:** `Desert/`

1. **Ankheg** - Large burrowing insects
2. **DesertHarpy** - Desert harpy
3. **DesertMummy** - Undead preserved by desert magic
4. **DuneReaper** - Reaper of the dunes
5. **OasisGuardian** - Protector of oases
6. **SandElemental** - Beings of living sand
7. **ScarabBeetle** - Large scarab beetles
8. **SunlitSerpent** - Sun-themed serpent
9. **VystiaDesertScorpion** - Giant scorpion
10. **VystiaDesertWyrm** - Desert wyrm
11. **VystiaSandVortex** - Sand vortex creature

### The Emberlands (Volcanic)
**Location:** `Emberlands/`

1. **AshGolem** - Golem made of ash
2. **EmberBat** - Fire bat
3. **FlameSprite** - Fire sprite
4. **LavaHound** - Hound of lava
5. **MagmaTroll** - Troll adapted to volcanic heat
6. **VystiaEmberPhoenix** - Phoenix of embers
7. **VystiaFireAnt** - Fire ant
8. **VystiaLavaElemental** - Lava elemental

### Ironclad Empire (Steampunk/Mechanical)
**Location:** `Ironclad/`

1. **BrassAutomaton** - Brass mechanical servant
2. **ClockworkSpider** - Mechanical spider
3. **CopperSentinel** - Copper guardian
4. **GearWolf** - Mechanical wolf
5. **IronElemental** - Elemental of iron
6. **OilSlime** - Oil-based slime
7. **SteamElemental** - Steam elemental
8. **SteamGolem** - Steam-powered golem
9. **VystiaMechanicalDrake** - Mechanical drake

### Shadowfen (Swamp)
**Location:** `Shadowfen/`

1. **BogBeast** - Creature of the bog
2. **BogWitch** - Swamp witch
3. **FenStalker** - Stalker of the fen
4. **MireLeech** - Leech of the mire
5. **Mistwalker** - Walker in the mist
6. **ShadowWolf** - Shadow-themed wolf
7. **SwampHorror** - Horror of the swamp
8. **SwampTroll** - Troll adapted to swamps
9. **SwampWisp** - Wisp of the swamp
10. **ToxicToad** - Poisonous toad
11. **VystiaMireElemental** - Mire elemental
12. **VystiaShadowBogling** - Shadow bogling
13. **VystiaSwampDrake** - Swamp drake

### Skyreach Mountains (Sky/Wind)
**Location:** `Skyreach/`

1. **AirSprite** - Air spirit
2. **GaleRider** - Rider of the gale
3. **NimbusWraith** - Cloud wraith
4. **SkyEagle** - Eagle of the sky
5. **SkyGolem** - Sky-themed golem
6. **StormGiant** - Giant of storms
7. **StormHarpy** - Harpy of storms
8. **StormRoc** - Roc of storms
9. **StormWisp** - Wisp of storms
10. **ThunderBird** - Bird of thunder
11. **VystiaCloudDrake** - Cloud drake
12. **VystiaCloudSerpent** - Cloud serpent
13. **VystiaGaleElemental** - Gale elemental
14. **VystiaLightningDrake** - Lightning drake
15. **Zephyr** - Wind spirit

### Crystal Barrens (Crystal)
**Location:** `CrystalBarrens/`

1. **CrystalGolem** - Golem of crystal
2. **CrystalSerpent** - Crystal serpent
3. **PrismaticWisp** - Prismatic wisp
4. **VystiaCrystalElemental** - Crystal elemental

### Underwater (Ocean/Depths)
**Location:** `Underwater/`

1. **AbyssalCrab** - Deep sea crab
2. **AbyssalJellyfish** - Deep sea jellyfish
3. **AbyssalSquid** - Deep sea squid
4. **CoralGolem** - Golem of coral
5. **DeepwaterShark** - Deep sea shark
6. **KelpHorror** - Horror of kelp
7. **Merfolk** - Humanoid sea creatures
8. **SeaHag** - Hag of the sea
9. **SirenWraith** - Wraith siren
10. **TidalElemental** - Tidal elemental
11. **VystiaDeepSeaSerpent** - Deep sea serpent
12. **VystiaSeaWyrm** - Sea wyrm

### ShadowVoid (Void/Shadow)
**Location:** `ShadowVoid/`

1. **AbyssalHorror** - Horror of the abyss
2. **NightmareHound** - Hound of nightmares
3. **ShadowElemental** - Shadow elemental
4. **VoidStalker** - Stalker of the void
5. **VoidTentacle** - Tentacle of the void
6. **VoidWraith** - Wraith of the void
7. **VystiaDarkWisp** - Dark wisp
8. **VystiaShadowDemon** - Shadow demon
9. **VystiaVoidDrake** - Void drake

---

## Miscellaneous Creatures

**Location:** `Misc/`

Generic creatures that can appear in multiple regions:

1. **VystiaBandit** - Bandit
2. **VystiaEttin** - Two-headed giant
3. **VystiaGhoul** - Undead ghoul
4. **VystiaGiantSpider** - Large spider
5. **VystiaGoblin** - Goblin
6. **VystiaOgre** - Ogre
7. **VystiaOrc** - Orc
8. **VystiaOrcCaptain** - Orc captain
9. **VystiaSkeleton** - Skeleton
10. **VystiaSpectre** - Spectre
11. **VystiaSpider** - Spider
12. **VystiaTroll** - Troll
13. **VystiaWolf** - Wolf
14. **VystiaZombie** - Zombie
15. **WildHorse** - Wild horse

---

## Implementation Details

### LLM Integration

NPCs that implement `ILLMConversational` have:
- `LLMConversationEnabled` - Toggle for LLM dialogue
- `PersonalityType` - Personality profile (Emperor, Sage, Warrior, etc.)
- `SpeechPattern` - Speech style (Formal, Gruff, OldEnglish, Cryptic, etc.)
- `HearingRange` - Range for detecting player speech (default 8-10 tiles)
- `ShouldHandleConversation()` - Checks if NPC should respond
- `HandleConversation()` - Processes LLM conversation

### Quest System Integration

- **Dynamic Quests:** Use `QuestNPC` class (spawned via quest wizard)
- **Mondain's Legacy:** Use `MondainQuester` base class (QuartermasterGrimwald, SageTheron)
- **Quest Linking:** NPCs can be linked to quest waypoints via `LinkToWaypoint()`

### Spawning

**GM Commands:**
- `[SpawnVystiaLeader <name>` - Spawns faction leaders
- `[SpawnVystiaCreature <name>` - Spawns creatures (if implemented)

**GM Gumps:**
- `VystiaNPCSpawnPickerGump` - Visual picker for:
  - Bosses (10)
  - Leaders (5)
  - TalkingCreatures (12)
  - QuestGivers (2)
  - Vendors (3)

### Base Classes

- **NPCs:** `BaseVendor` (for vendors/leaders) or `BaseCreature` (for talking creatures)
- **Creatures:** `BaseCreature`
- **Quest NPCs:** `QuestNPC` (dynamic quests) or `MondainQuester` (legacy quests)

### Regional Organization

All creatures are organized by region folder:
- `Bosses/` - Boss encounters
- `NPCs/` - Humanoid NPCs (FactionLeaders, TalkingCreatures, QuestGivers, Vendors)
- `Frosthold/`, `Verdantpeak/`, `Desert/`, etc. - Regional creatures
- `Misc/` - Generic creatures
- `Trainers/` - Class trainers
- `Vendors/` - Vendor NPCs
- `Races/` - Playable races (Dwarf)

---

## Notes and Discrepancies

### Registry Remediation (2025-12-12)

**Status:** ✅ **REMEDIATED** - The NPC registry (`ServUO/Data/Vystia/npc_templates.json`) has been updated to match the actual codebase implementations.

**Changes Made:**
1. **Faction Leaders:** Registry now matches code implementations:
   - Frosthold: "Chieftain Bjorn Frostbeard" (matches code)
   - Verdantpeak: "Elder Seraphina Leafwhisper" (matches code)
   - Desert: "Sultan Azir al-Rashid" (matches code)
   - Emberlands: "Archmage Pyrus Ashborn" (matches code)

2. **Complete Creature Registry:** All 150+ creatures from the codebase are now registered:
   - All 12 Talking Creatures (Ancient Beings)
   - All 10 Bosses
   - All 12 Frosthold creatures
   - All 13 Verdantpeak creatures
   - All 11 Desert creatures
   - All 8 Emberlands creatures
   - All 9 Ironclad creatures
   - All 13 Shadowfen creatures
   - All 15 Skyreach creatures
   - All 4 Crystal Barrens creatures
   - All 12 Underwater creatures
   - All 9 ShadowVoid creatures
   - All 15 Misc creatures

3. **Waypoint Support:** All creatures are now registered with `mobileTypeName` matching their C# class names, enabling quest waypoints to spawn them via `TargetTypeName`.

**Registry Location:** `ServUO/Data/Vystia/npc_templates.json`  
**Total Entries:** 200+ (NPCs + Creatures)

### Implementation Status (2025-12-12)

**Status:** ✅ **ALL FACTION LEADERS IMPLEMENTED**

All 20 faction leaders from the registry are now implemented in code:
- ✅ Emperor Garrick Steelarm (Ironclad)
- ✅ Chieftain Bjorn Frostbeard (Frosthold)
- ✅ Elder Seraphina Leafwhisper (Verdantpeak)
- ✅ Sultan Azir al-Rashid (Desert)
- ✅ Archmage Pyrus Ashborn (Emberlands)
- ✅ Admiral Maris Hawkseye (Sunken Isles) - **NEW**
- ✅ Lord Mariner Tideseeker (Glimmering Archipelago) - **NEW**
- ✅ High Guardian Eldur Mountainborn (Skyreach) - **NEW**
- ✅ Chief Mara Wildsong (Wilderlands) - **NEW**
- ✅ Archmage Lumis (Mystic Canyons) - **NEW**
- ✅ Sorceress Nocturna (Eternal Twilight) - **NEW**
- ✅ Sage Orin (Forgotten Depths) - **NEW**
- ✅ Druid Lord Faelar (Shadowfen) - **NEW**
- ✅ Guardian Sylas (Hollow Forests) - **NEW**
- ✅ Queen Iceshadow (Winterguard) - **NEW**
- ✅ Queen Amaryllis (Verdantpeak) - **NEW**
- ✅ Warlord Emberon Flamefist (Emberlands) - **NEW**
- ✅ Sheikh Tarik (Whispering Sands) - **NEW**
- ✅ Warlord Kael (Blazing Frontier) - **NEW**
- ✅ King Frostbeard (Frosthold) - **NEW**

**GM Commands:**
- `[SpawnVystiaLeader <name>` - Spawns any of the 20 faction leaders
- Available names: garrick, bjorn, seraphina, azir, pyrus, maris, tideseeker, eldur, mara, lumis, nocturna, orin, faelar, sylas, iceshadow, amaryllis, emberon, tarik, kael, frostbeard

**Gump Access:**
- All 20 leaders are available in `VystiaNPCSpawnPickerGump` under the "Leaders" category

---

## Summary Statistics

- **Faction Leaders:** 5 implemented
- **Talking Creatures:** 12 implemented
- **Quest Givers:** 2 implemented (Mondain's Legacy)
- **Vendors:** 15+ implemented (3 essential + 12 class vendors + resource vendors)
- **Bosses:** 10 implemented
- **Regional Creatures:** 100+ implemented across all regions
- **Total NPCs/Creatures:** 150+ implemented

---

**Last Updated:** 2025-12-12  
**Registry Status:** ✅ Remediated - All implemented NPCs and creatures are registered in `ServUO/Data/Vystia/npc_templates.json`  
**Next Review:** When new NPCs/creatures are added to the codebase

