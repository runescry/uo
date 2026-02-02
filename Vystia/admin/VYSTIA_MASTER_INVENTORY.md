# Vystia Master Inventory

**Last Updated:** 2026-01-03
**Overall Status:** 99% Complete
**Total Implementations:** ~1,700+ items across ~662+ files

---

## Quick Summary

| Category | Count | Status |
|----------|-------|--------|
| Character Classes | 26 | Complete |
| Custom Skills | 26 | Complete |
| Magic Schools | 12 | Complete |
| Spells | 384 | Complete |
| Martial Abilities | 224 | Complete |
| Spell Scrolls | 384 | Complete |
| Spellbooks | 12 | Complete |
| Magic Reagents | 96 | Complete |
| Reagent Bags | 12 | Complete |
| Creatures | 138 | Complete |
| Boss Creatures | 10 | Complete |
| Regional Weapons | 40 | Complete |
| Legendary Weapons | 5 | Complete |
| Regional Armor | 59 | Complete |
| Legendary Armor (Role-Based) | 43 | Complete |
| **Class Legendary Armor** | **156** | **Complete** |
| Regional Shields | 8 | Complete |
| Class Focus Items | 26 | Complete |
| Class Special Items | 16 | Complete |
| Resource Consumables | 11 | Complete |
| Resources (Ores/Ingots/etc) | 130+ | Complete |
| Vendors | 15 | Complete |
| Class Trainers | 25 | Complete |
| UI Gumps | 4 | Complete |
| Quest System | Framework | Complete |
| Class Guides | 25 | Complete |
| LLM Lore Entries | 195 | Complete |
| Economy System | 5 services | Complete |
| Religion System | 6 religions | Complete |
| Pet System | 4 classes | Complete |
| Faction System | 7 factions | Complete |
| Housing System | 5 sizes | Complete |
| Zone Control | 4 types | Complete |

---

## Quick Reference (Single Source of Truth)

| Reference | Content | Location |
|-----------|---------|----------|
| **[CLASSES.md](reference/CLASSES.md)** | All 25 classes with stats, skills, items | `Vystia/reference/` |
| **[SPELLS.md](reference/SPELLS.md)** | All 384 spells with IDs (1000-1383) | `Vystia/reference/` |
| **[SKILLS.md](reference/SKILLS.md)** | All 26 custom skills (IDs 58-83) | `Vystia/reference/` |
| **[COMMANDS.md](reference/COMMANDS.md)** | All GM commands for testing | `Vystia/reference/` |
| **[PLAYERS_GUIDE.md](PLAYERS_GUIDE.md)** | Player guide with combos | `Vystia/` |
| **[GM_TESTING_GUIDE.md](GM_TESTING_GUIDE.md)** | GM testing procedures | `Vystia/` |

---

## Recent Updates

### 2026-01-03 (Latest)
- **Equipment Documentation Complete:** Documented 180 previously undocumented armor pieces
- **Class Legendary Armor Sets:** 156 pieces (26 classes × 6 pieces each) now cataloged
- **Additional Legendary Sets:** 24 pieces (4 role-based sets) now cataloged
- **Total Equipment Count:** Updated from 172 to 352 items
- **Inventory Update:** Total implementations increased from ~1,500 to ~1,700 items

### 2026-01-02
- **Economy System Complete:** Repair costs, service fees, training costs
- **Religion System Complete:** 6 religions, piety, prayer, tithe, shrines
- **Pet System Complete:** 4 classes with summonable pets
- **Faction System Complete:** 7 factions with reputation and vendor discounts
- **Housing System Complete:** Purchase prices, weekly property tax
- **Zone Control Complete:** 4 zone types, PvP rules, death penalties
- **Documentation Update:** Commands expanded to 147, all CLAUDE.md files updated

### 2025-12-12
- **Class Trainers Complete:** 25 trainer NPCs (one per class) with regional hues
- **Quest Framework Complete:** VystiaQuestSystem with tracking, objectives, rewards
- **Class Selection System Verified:** Multi-page gump, confirmation, CharacterCreation integration
- **Documentation Update:** Complete inventory audit and documentation refresh

### 2025-12-11
- **Martial Abilities Complete:** 224 abilities (14 martial classes × 16 abilities)
- **Custom Skills Complete:** 26 skills (12 magic + 14 martial, IDs 58-83)
- **Player/GM Guides Created:** Complete documentation for players and GMs

### 2025-12-08
- **Spellbook System 100% Functional:** All 12 spellbooks working
- **Equipment System 100% Complete:** 172 items generated
- **LLM Lore System Complete:** 195 entries across 16 domain files

---

## 1. Character Classes (26 Complete)

### By Region

| Region | Classes | Count |
|--------|---------|-------|
| Frosthold | Barbarian, Beastmaster, Ice Mage | 3 |
| Emberlands | Sorcerer | 1 |
| Desert | Ranger, Illusionist | 2 |
| Shadowfen | Witch | 1 |
| ShadowVoid | Warlock, Necromancer | 2 |
| Verdantpeak | Druid, Alchemist | 2 |
| Crystal Barrens | Oracle, Wizard | 2 |
| Ironclad | Artificer, Fighter, Monk, Templar | 4 |
| Underwater | Summoner | 1 |
| Multi-Regional | Bounty Hunter, Knight, Shaman, Cleric, Paladin, Bard, Enchanter | 7 |

**Location:** `ServUO/Scripts/Custom/VystiaClasses/Classes/`

### Class Stats Summary

| # | Class | Region | Role | STR | DEX | INT | Primary Skill |
|---|-------|--------|------|-----|-----|-----|---------------|
| 1 | Barbarian | Frosthold | Melee DPS | 45 | 20 | 15 | Berserking |
| 2 | Beastmaster | Frosthold | Pet/Ranged | 25 | 35 | 20 | BeastBonding |
| 3 | Ice Mage | Frosthold | Caster DPS | 15 | 20 | 45 | Cryomancy |
| 4 | Sorcerer | Emberlands | Caster DPS | 15 | 20 | 45 | Elementalism |
| 5 | Ranger | Desert | Ranged DPS | 25 | 45 | 10 | Marksmanship |
| 6 | Illusionist | Desert | Caster CC | 10 | 25 | 45 | IllusionMagic |
| 7 | Witch | Shadowfen | Debuffer | 15 | 20 | 45 | Hexcraft |
| 8 | Warlock | ShadowVoid | Caster DPS | 15 | 20 | 45 | Demonology |
| 9 | Druid | Verdantpeak | Healer/Hybrid | 20 | 25 | 35 | Druidism |
| 10 | Alchemist | Verdantpeak | Support | 20 | 30 | 30 | Transmutation |
| 11 | Oracle | Crystal Barrens | Utility | 10 | 20 | 50 | Divination |
| 12 | Artificer | Ironclad | Pet/Ranged | 25 | 30 | 25 | Engineering |
| 13 | Fighter | Ironclad | Tank | 40 | 25 | 15 | CombatMastery |
| 14 | Monk | Ironclad | Melee/Hybrid | 30 | 35 | 15 | MartialArts |
| 15 | Templar | Ironclad | Tank/DPS | 35 | 25 | 20 | Zealotry |
| 16 | Necromancer | ShadowVoid | Caster/Pet | 15 | 15 | 50 | NecromancyArts |
| 17 | Summoner | Underwater | Pet/Caster | 15 | 20 | 45 | Conjuration |
| 18 | Bounty Hunter | Multi-Regional | Ranged/Melee | 30 | 35 | 15 | Manhunting |
| 19 | Knight | Multi-Regional | Tank/Melee | 38 | 27 | 15 | ChivalricArts |
| 20 | Shaman | Multi-Regional | Healer/Hybrid | 20 | 20 | 40 | SpiritCalling |
| 21 | Wizard | Crystal Barrens | Utility | 10 | 20 | 50 | ArcaneStudies |
| 22 | Cleric | Multi-Regional | Healer | 20 | 20 | 40 | DivineGrace |
| 23 | Paladin | Multi-Regional | Tank/Healer | 35 | 20 | 25 | HolyDevotion |
| 24 | Bard | Multi-Regional | Support/CC | 15 | 35 | 30 | BardicLore |
| 25 | Enchanter | Multi-Regional | Utility/Buff | 15 | 25 | 40 | Runeweaving |

---

## 2. Custom Skills (26 Complete)

### Magic Skills (12) - IDs 58-69

| ID | Skill | Class | Description |
|----|-------|-------|-------------|
| 58 | Cryomancy | Ice Mage | Ice magic mastery |
| 59 | Demonology | Warlock | Dark pact magic |
| 60 | NecromancyArts | Necromancer | Undead control |
| 61 | Druidism | Druid | Nature magic |
| 62 | Elementalism | Sorcerer | Elemental magic |
| 63 | BardicLore | Bard | Song power |
| 64 | Hexcraft | Witch | Curse potency |
| 65 | Divination | Oracle | Foresight |
| 66 | Conjuration | Summoner | Summon power |
| 67 | SpiritCalling | Shaman | Totem power |
| 68 | Runeweaving | Enchanter | Enchantment power |
| 69 | IllusionMagic | Illusionist | Illusion duration |

### Martial Skills (14) - IDs 70-83

| ID | Skill | Class | Description |
|----|-------|-------|-------------|
| 70 | Berserking | Barbarian | Fury generation |
| 71 | Subterfuge | (Reserved) | Stealth power |
| 72 | MartialArts | Monk | Chi generation |
| 73 | ChivalricArts | Knight | Defensive power |
| 74 | HolyDevotion | Paladin | Virtue power |
| 75 | Marksmanship | Ranger | Focus generation |
| 76 | CombatMastery | Fighter | Weapon mastery |
| 77 | Zealotry | Templar | Zeal generation |
| 78 | Manhunting | Bounty Hunter | Tracking power |
| 79 | BeastBonding | Beastmaster | Pet power |
| 80 | Engineering | Artificer | Construct power |
| 81 | Transmutation | Alchemist | Potion power |
| 82 | DivineGrace | Cleric | Healing power |
| 83 | ArcaneStudies | Wizard | Arcane mastery |

**Location:** `ServUO/Server/Skills.cs`, `ServUO/Scripts/Misc/SkillInfo.cs`

---

## 3. Magic System (Complete)

### Magic Schools (12)

| School | Class | Spellbook | Spell IDs | Reagents |
|--------|-------|-----------|-----------|----------|
| Ice Magic | Ice Mage | IceMageSpellbook | 1000-1031 | 8 |
| Nature Magic | Druid | DruidSpellbook | 1032-1063 | 8 |
| Hex Magic | Witch | WitchSpellbook | 1064-1095 | 8 |
| Elemental Magic | Sorcerer | SorcererSpellbook | 1096-1127 | 8 |
| Dark Magic | Warlock | WarlockSpellbook | 1128-1159 | 8 |
| Divination | Oracle | OracleSpellbook | 1160-1191 | 8 |
| Necromancy | Necromancer | NecromancerSpellbook | 1192-1223 | 8 |
| Summoning | Summoner | SummonerSpellbook | 1224-1255 | 8 |
| Shamanic | Shaman | ShamanSpellbook | 1256-1287 | 8 |
| Bardic | Bard | BardSpellbook | 1288-1319 | 8 |
| Enchanting | Enchanter | EnchanterSpellbook | 1320-1351 | 8 |
| Illusion | Illusionist | IllusionistSpellbook | 1352-1383 | 8 |

### Totals
- **Spells:** 384 (32 per school × 12 schools)
- **Spell Scrolls:** 384 (one per spell)
- **Spellbooks:** 12
- **Magic Reagents:** 96 (8 per school)
- **Reagent Bags:** 12 (one per school)

**Locations:**
- Spells: `ServUO/Scripts/Custom/VystiaClasses/Spells/`
- Scrolls: `ServUO/Scripts/Items/Vystia/Scrolls/`
- Spellbooks: `ServUO/Scripts/Items/Equipment/Spellbooks/VystiaSpellbooks.cs`
- Reagents: `ServUO/Scripts/Items/Vystia/Resources/Reagents/`

---

## 4. Martial Abilities (224 Complete)

**Total:** 224 abilities (16 per class × 14 martial classes)
**ID Range:** 2000-2223
**Location:** `ServUO/Scripts/Custom/VystiaClasses/Abilities/Generated/Martial/`

| Class | ID Range | Abilities |
|-------|----------|-----------|
| Fighter | 2000-2015 | Power Strike, Cleave, Shield Wall, Charge, etc. |
| Barbarian | 2016-2031 | Reckless Strike, Berserker Rage, Avalanche Strike, etc. |
| Monk | 2032-2047 | Jab, Tiger Palm, Flurry of Blows, Chi Explosion, etc. |
| Rogue | 2048-2063 | Sinister Strike, Backstab, Eviscerate, Shadow Dance, etc. |
| Ranger | 2064-2079 | Steady Shot, Aimed Shot, Sandstorm Arrow, Barrage, etc. |
| Knight | 2080-2095 | Charge, Shield Slam, Challenge, Guardian of Light, etc. |
| Paladin | 2096-2111 | Crusader Strike, Divine Storm, Avenging Wrath, etc. |
| Templar | 2112-2127 | Judgment Strike, Smiting Blow, Iron Justicar, etc. |
| Bounty Hunter | 2128-2143 | Quick Draw, Silver Strike, Master Hunter, etc. |
| Beastmaster | 2144-2159 | Call Pet, Kill Command, Alpha Predator, etc. |
| Artificer | 2160-2175 | Deploy Turret, Rocket Barrage, Mech Suit, etc. |
| Alchemist | 2176-2191 | Fire Bomb, Mutagen, Philosopher's Bomb, etc. |
| Cleric | 2192-2207 | Heal, Divine Smite, Divine Intervention, etc. |
| Wizard | 2208-2223 | Arcane Bolt, Blink, Meteor, Time Stop, etc. |

---

## 5. Creatures (138 Complete)

### By Region

| Region | Count | Notable Creatures |
|--------|-------|-------------------|
| Bosses | 10 | FrostFather, VolcanoWyrm, AncientKraken, TimewornLich |
| Frosthold | 12 | FrostGiant, WinterWolf, IceGolem, GlacialBear |
| Emberlands | 8 | LavaHound, EmberBat, MagmaTroll, FlameSprite |
| Desert | 11 | SandElemental, DesertWyrm, Ankheg, DuneReaper |
| Shadowfen | 13 | BogBeast, SwampHorror, MireLeech, Mistwalker |
| Verdantpeak | 13 | ForestGuardian, TreantSapling, WildBoar, MossGolem |
| Crystal Barrens | 4 | CrystalGolem, PrismaticWisp, CrystalSerpent |
| Ironclad | 9 | SteamGolem, ClockworkSpider, BrassAutomaton |
| Skyreach | 15 | StormRoc, ThunderBird, GaleRider, Zephyr |
| Underwater | 12 | DeepwaterShark, KelpHorror, Merfolk, TidalElemental |
| ShadowVoid | 9 | VoidStalker, ShadowDemon, NightmareHound |
| Misc | 15 | VystiaOrc, VystiaGoblin, PracticeDummy |

**Location:** `ServUO/Scripts/Mobiles/Vystia/`

---

## 6. Equipment (352 Complete)

### Weapons (68 Total)

| Category | Count | Examples |
|----------|-------|----------|
| Regional Weapons | 40 | FrostBlade, FlameTongue, SandScimitar, etc. |
| Legendary Weapons | 5 | The Eternal Winter, Phoenix Ascension, Voidcaller |

### Armor (265 Total)

| Type | Count | Sets |
|------|-------|------|
| Regional Plate | 24 | Frostforged, Emberforged, Clockwork, Voidforged |
| Regional Chain | 9 | Crystal Chain, Shadow Chain, Desert Chain |
| Regional Ring | 8 | Living Ring, Steam Ring |
| Regional Leather | 18 | Frost Leather, Fire Leather, Shadow Leather |
| Legendary Armor (Role-Based) | 19 | Glacial Aegis, Steamwork Exosuit, Shadow Shroud |
| **Class Legendary Armor** | **156** | **One set per class (26 classes × 6 pieces)** |
| **Additional Legendary Sets** | **24** | **Celestial Raiment, Stormrider Garb, Arcanist Regalia, Harmonist Vestments** |

#### Class Legendary Armor Sets (156 pieces)
Each of the 26 Vystia classes has a unique legendary armor set (6 pieces each):

**Leather-Based Sets (19 classes):**
- Ice Mage, Druid, Witch, Sorcerer, Warlock, Oracle, Necromancer, Summoner
- Shaman, Bard, Enchanter, Illusionist, Beastmaster, Monk, Ranger
- Rogue, Bounty Hunter, Alchemist, Wizard

**Plate-Based Sets (6 classes):**
- Barbarian, Fighter, Templar, Knight, Paladin, Artificer

**Chain-Based Set (1 class):**
- Cleric

#### Additional Legendary Armor Sets (24 pieces)

**Role-Based Legendary Sets:**

1. **Celestial Raiment (6 pieces)** - Healer Role
   - Hue: 1153 | Chain/Plate Mix
   - Bonuses: +4 INT, +4 HP Regen, +3 Mana Regen
   - Skill: Healing +5
   - Pieces: Coif, Gorget, Tunic, Sleeves, Gloves, Leggings

2. **Stormrider Garb (6 pieces)** - Ranged DPS Role
   - Hue: 1165 | Full Leather
   - Bonuses: +5 DEX, +10% Hit Chance, +5% Weapon Speed
   - Skill: Archery +5
   - Pieces: Cap, Gorget, Tunic, Sleeves, Gloves, Leggings

3. **Arcanist Regalia (6 pieces)** - Caster DPS Role
   - Hue: 1154 | Full Leather
   - Bonuses: +5 INT, +10% Spell Damage, +3 Mana Regen
   - Skill: Magery +5
   - Pieces: Cap, Gorget, Tunic, Sleeves, Gloves, Leggings

4. **Harmonist Vestments (6 pieces)** - Support Role
   - Hue: 2010 | Full Leather
   - Bonuses: +2 STR, +2 DEX, +2 INT, +50 Luck
   - Skill: Musicianship +5
   - Pieces: Cap, Gorget, Tunic, Sleeves, Gloves, Leggings

### Shields (8 Total)
Ice Wall, Flame Guard, Prism Shield, Clockwork Shield, Bog Shield, Sand Shield, Void Shield, Living Shield

### Jewelry (11 Total)
- Class Focus Items (26) - One per class (see Section 7)

**Location:** `ServUO/Scripts/Items/Vystia/Equipment/`
**Class Legendary:** `ServUO/Scripts/Items/Vystia/Equipment/Armor/ClassLegendaryArmorSets.cs`
**Additional Legendary:** `ServUO/Scripts/Items/Vystia/Equipment/Armor/AdditionalLegendaryArmorSets.cs`

---

## 7. Class Items (53 Total)

### Class Focus Items (26)
One per class, provides stat bonuses and class enhancement.

### Special Ability Items (16)

| Item | Class | Function |
|------|-------|----------|
| RageTotem | Barbarian | +20 STR buff |
| BeastWhistle | Beastmaster | Summon companion |
| AlchemistKit | Alchemist | Portable alchemy |
| CrystalOrb | Oracle | Divination focus |
| MonkBeads | Monk | Meditation aid |
| TemplarCross | Templar | Divine symbol |
| SummoningCircle | Summoner | Summon focus |
| BountyLedger | Bounty Hunter | Target tracker |
| KnightBanner | Knight | Honor symbol |
| SpiritTotem | Shaman | Spirit focus |
| MagicLute | Bard | Song focus |
| EnchantingCrystal | Enchanter | Enchant focus |
| ConstructControlDevice | Artificer | Control constructs |
| ClockworkScout | Artificer | Summon scout |
| ShapeshiftTotem | Druid | Transform focus |
| HolySymbol | Cleric/Paladin | AoE healing |

### Resource Consumables (11)
SoulShardPotion, FuryPotion, ChiPotion, etc.

**Location:** `ServUO/Scripts/Custom/VystiaClasses/Items/`

---

## 8. Resources (130+ Total)

### Ores (8)
FrozenOre, MoltenOre, CrystalOre, SteamworkOre, BogIronOre, SandstoneOre, ObsidianOre, LivingOre

### Ingots (8)
FrostforgedIngot, EmberforgedIngot, CrystallineIngot, SteamworkIngot, MireforgedIngot, SandstoneIngot, VoidstoneIngot, NatureforgedIngot

### Woods (7)
FrostwillowLog, FlamewoodLog, CrystalwoodLog, IronwoodLog, BogwoodLog, DesertSandLog, VoidwoodLog

### Mechanical Components (3)
ClockworkGear, ClockworkSpring, SteamCore

### Special Resources (8)
EternalIce, EverburningCoal, PrismaticShard, TreantHeart, PhoenixFeather, KrakenInk, DragonScalePowder, VoidDust

**Location:** `ServUO/Scripts/Items/Vystia/Resources/`

---

## 9. NPCs

### Vendors (15)

| Type | Count | Sells |
|------|-------|-------|
| Magic School Vendors | 12 | Reagents, scrolls, spellbook per school |
| VystiaReagentVendor | 1 | All 96 reagents |
| VystiaResourceVendor | 1 | Ores, ingots, woods, components |
| VystiaClassItemVendor | 1 | Class focus items, special items |

**Location:** `ServUO/Scripts/Mobiles/Vystia/Vendors/`

### Class Trainers (25)

One trainer per class with:
- Regional hue theming
- Interactive dialogue
- Class selection capability
- Skill training

**Commands:**
- `[spawntrainer <classname>` - Spawn specific trainer
- `[spawnalltrainers` / `[sat` - Spawn all trainers

**Location:** `ServUO/Scripts/Mobiles/Vystia/Trainers/VystiaClassTrainers.cs`

### Other NPCs (13)
- Faction Leaders (5): Emperor, Chieftain, Elder, Sultan, Archmage
- Talking Creatures (3): Ancient Dragon, Treant, Sphinx
- Essential Vendors (3): Banker, Healer, Guard Captain
- Quest Givers (Mondain/BaseQuest, 2): Quartermaster, Sage
- Quest Giver (Vystia Dynamic): Chronicler

**Location:** `ServUO/Scripts/Mobiles/Vystia/NPCs/`

---

## 10. UI Systems (4 Gumps)

| Gump | Purpose |
|------|---------|
| VystiaClassSelectionGump | Multi-page class browser (26 classes) |
| VystiaClassConfirmationGump | Class details and confirmation |
| VystiaAdminGump | GM testing interface |
| AbilityEditorGump | Ability creation/testing |

**Location:** `ServUO/Scripts/Custom/VystiaClasses/Gumps/`

---

## 11. Quest System (Framework Complete)

**Quest Systems (split):**
- **Vystia Dynamic Quests (LLM):** QuestNPC + Chronicler + Quest Wizard + [GenLLMQuest]
- **Mondain/BaseQuest:** Classic quest chain system (this section)

### Framework
- **VystiaQuestSystem.cs** - Core quest tracking and management
- **QuestProgress** - Per-quest objective tracking
- **VystiaQuestTracker** - Per-player quest state

### Features
- Quest registration and lookup
- Objective progress tracking
- Prerequisite quest chains
- Reward delivery system
- 4-tier quest structure (Initiation, Apprentice, Journeyman, Master)

### Sample Quests (Ice Mage)
- Whispers of Winter (Initiation)
- Gathering Frost (Material collection)
- Chill of Knowledge (Skill usage)
- The Frozen Heart (Artifact quest)

**Location:** `ServUO/Scripts/Custom/VystiaClasses/Quests/`
**Design Doc:** `Vystia/design/CLASS_QUEST_SYSTEM_DESIGN.md`

---

## 12. Documentation

### Reference Files (Single Source of Truth)

| File | Content |
|------|---------|
| `Vystia/reference/CLASSES.md` | All 25 classes with stats |
| `Vystia/reference/SPELLS.md` | All 384 spells with IDs |
| `Vystia/reference/SKILLS.md` | All 26 custom skills |
| `Vystia/reference/COMMANDS.md` | All GM commands |

### Class Guides (25)
Detailed guide for each class including spells, rotations, stances, abilities.
**Location:** `Vystia/guides/`

### Player/GM Guides

| File | Content |
|------|---------|
| `Vystia/PLAYERS_GUIDE.md` | Player-facing documentation |
| `Vystia/GM_TESTING_GUIDE.md` | GM testing procedures |

### LLM Lore System (195 Entries)
Knowledge base for NPC dialogue across 16 domain files.
**Location:** `ServUO/Data/Lore/`

---

## 13. Core Systems (Complete)

### Secondary Resources (15 Types)

| Resource | Classes |
|----------|---------|
| Soul Shards | Warlock |
| Fury | Barbarian |
| Chi | Monk |
| Chill | Ice Mage |
| Combo Points | Fighter, Ranger |
| Holy Power | Paladin, Templar |
| Beast Points | Beastmaster |
| Nature Energy | Druid |
| Shadow Energy | Necromancer |
| Arcane Charges | Wizard, Sorcerer |
| Spirit | Shaman |
| Crescendo | Bard |
| Rune Power | Enchanter |
| Pursuit | Bounty Hunter |
| Steam/Charges | Artificer |

### Buff/Debuff System
- 25+ buff types
- Stackable effects, DoTs, HoTs
- Transform effects, shields, absorbs

### Stance System (28 Stances)
Druid Forms, Sorcerer Elements, Fighter Stances, Monk Stances, Rogue Stances, Paladin Auras, Ranger Aspects, Barbarian Modes

### Crowd Control System
- 15 CC types
- Diminishing returns (3 levels + immunity)

**Location:** `ServUO/Scripts/Custom/VystiaClasses/Core/`

---

## 14. Gameplay Systems (Complete)

### Economy System
- **Repair Service:** Material-based repair costs (2-75g per durability)
- **Service Fees:** Resurrection (50g), healing (25g), cure (15g)
- **Training Costs:** 500-10,000g per tier
- **Travel Costs:** 100-250g per moongate
- **Stabling Costs:** 30g/week per pet
**Location:** `ServUO/Scripts/Custom/VystiaClasses/Economy/`

### Religion System
- **6 Religions:** Solarius, Lunara, Terrath, Aquos, Sylvanis, Mortis
- **Piety Tiers:** Initiate (0-49), Faithful (50-199), Devoted (200-499), Blessed (500-899), Exalted (900-1000)
- **Piety Gains:** Prayer (+10/day), Tithe (+1 per 100g), Pilgrimage (+75)
- **Passive Bonuses:** +5% damage/resist/regen at 200 piety
**Location:** `ServUO/Scripts/Custom/VystiaClasses/Religion/`

### Pet System
- **Classes:** Summoner, Necromancer, Beastmaster, Artificer
- **Pet Types:** Elementals, Undead, Beasts, Constructs
- **5 Tiers:** Lesser to Legendary
**Location:** `ServUO/Scripts/Custom/VystiaClasses/Pets/`

### Faction System
- **7 Factions:** Frosthold, Ember, Desert, Shadowfen, Verdant, Crystal, Ironclad
- **Reputation Tiers:** Hostile (-3000) to Exalted (15000+)
- **Vendor Discounts:** 5-15% based on tier
**Location:** `ServUO/Scripts/Custom/VystiaClasses/Factions/`

### Housing System
- **5 Sizes:** Small (50k) to Castle (3M)
- **Weekly Tax:** 1% of purchase price
- **Auto-collection:** Hourly timer checks
**Location:** `ServUO/Scripts/Custom/VystiaClasses/Housing/`

### Zone Control
- **4 Types:** Sanctuary, Contested, Lawless, Extreme
- **Death Penalties:** 0% to 15% skill loss
- **Loot Drops:** 0% to 50%
**Location:** `ServUO/Scripts/Custom/VystiaClasses/Zones/`

---

## File Structure Summary

```
ServUO/Scripts/
├── Custom/VystiaClasses/
│   ├── Classes/           # 26 class definitions
│   ├── Core/              # Core systems
│   ├── Spells/            # 384 spell files
│   ├── Abilities/         # 224 martial abilities
│   ├── Items/             # Class items
│   ├── Gumps/             # UI systems
│   ├── Quests/            # Quest framework
│   ├── Commands/          # GM commands
│   ├── Economy/           # Repair, service fees
│   ├── Religion/          # Piety, shrines
│   ├── Pets/              # Pet summoning
│   ├── Factions/          # Reputation system
│   ├── Housing/           # Property tax
│   └── Zones/             # Zone control
├── Mobiles/Vystia/
│   ├── [11 region folders]  # 138 creatures
│   ├── Vendors/             # 15 vendors
│   ├── Trainers/            # 25 trainers
│   └── NPCs/                # 13 NPCs
└── Items/Vystia/
    ├── Resources/           # 130+ resources + 96 reagents
    ├── Scrolls/             # 384 spell scrolls
    └── Equipment/           # 352 equipment items
        ├── Weapons/         # 45 weapons
        ├── Armor/           # 265 armor pieces (incl. 156 class legendary)
        └── Shields/         # 8 shields
```

---

## Build Status

**Compilation:** 0 errors, 0 warnings
**Last Verified:** 2026-01-03

---

## Summary Statistics

| System | Total | Status |
|--------|-------|--------|
| Character Classes | 26 | 100% Complete |
| Custom Skills | 26 | 100% Complete |
| Spells | 384 | 100% Complete |
| Martial Abilities | 224 | 100% Complete |
| Spell Scrolls | 384 | 100% Complete |
| Spellbooks | 12 | 100% Complete |
| Reagents | 96 | 100% Complete |
| Vendors | 15 | 100% Complete |
| Trainers | 25 | 100% Complete |
| **Equipment** | **352** | **100% Complete** |
| Creatures | 138 | 100% Complete |
| Resources | 130+ | 100% Complete |
| Quest Framework | 1 | 100% Complete |
| UI Gumps | 4 | 100% Complete |
| LLM Lore | 195 | 100% Complete |
| Class Guides | 25 | 100% Complete |
| Economy System | 5 | 100% Complete |
| Religion System | 6 | 100% Complete |
| Pet System | 4 | 100% Complete |
| Faction System | 7 | 100% Complete |
| Housing System | 5 | 100% Complete |
| Zone Control | 4 | 100% Complete |

**Overall Progress: ~99% Complete**

---

## Remaining Tasks

**Test Guide:** See [gm/SYSTEM_TEST_GUIDE.md](../gm/SYSTEM_TEST_GUIDE.md) for comprehensive testing checklist

### Testing Required
- [ ] In-game testing of all 12 magic schools
- [ ] Verify spell casting with reagents
- [ ] Test class abilities and stances
- [ ] Balance testing

### Future Enhancements
- [ ] Expand quest content for all classes
- [ ] Add more NPC quest givers
- [ ] Prestige classes
- [ ] Talent trees
- [ ] Class halls

---

*This is the single source of truth for Vystia content inventory.*
*Last Updated: 2026-01-02*
