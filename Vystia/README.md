# 🌍 VYSTIA - Ultima Online World System Documentation

**Design and documentation workspace** for the Vystia fantasy world content system.

⚠️ **IMPORTANT:** This directory contains **design documents and aspirational content**. Most features described here are **NOT implemented yet**.

## ✅ **What Actually Exists:**
- **131 creatures** - compiled and spawnable via `[spawnvystia]` command
- **100+ items** - ores, ingots, weapons, armor, reagents, artifacts
- **Complete item system** - all resources and equipment functional
- **Creature & item designs** - documented in bestiary and guides

## ❌ **What Doesn't Exist Yet:**
- **No Vystia map** - not deployed to ServUO
- **No cities** (23 planned but not built)
- **No dungeons** (18 planned but not placed)
- **No world generation commands**

📖 **READ FIRST:** [ACTUAL_USAGE.md](ACTUAL_USAGE.md) - Reality check of what works vs what's planned

---

## 📁 Directory Structure

```
Vystia/
├── README.md                    # This file - main navigation
├── DOCS_INDEX.md                # Documentation index
├── config/                      # Configuration files (design specs)
│   └── VYSTIA_WORLD_CONFIG.json # World generation parameters
├── docs/                        # Core documentation
│   ├── VYSTIA_WORLD_LORE.md    # Complete world lore
│   ├── VYSTIA_CREATURES_BESTIARY.md # Master creature list (design)
│   ├── VYSTIA_ITEMS_GUIDE.md   # Items and equipment (design)
│   ├── VYSTIA_CONTENT_ROADMAP.md # Implementation roadmap
│   ├── VYSTIA_ART_IMPLEMENTATION.md # Art system guide
│   ├── VYSTIA_DUNGEON_BOSSES.md # Dungeon boss mechanics
│   ├── VYSTIA_SEA_SYSTEMS.md   # Underwater systems
│   ├── VYSTIA_FACTIONS_GUIDE.md # Faction system
│   └── VYSTIA_TODO.md          # Project TODO list
├── guides/                      # Implementation guides
│   ├── VYSTIA_INSTALLATION_GUIDE.md # Setup instructions
│   ├── VYSTIA_DEPLOYMENT_GUIDE.md # Deployment options
│   ├── VYSTIA_CREATURES_IMPLEMENTATION.md # Creature implementation
│   ├── VYSTIA_WORLD_GENERATION.md # World generation
│   ├── VYSTIA_RESOURCES_GUIDE.md # Resource management
│   ├── VYSTIA_EQUIPMENT_GUIDE.md # Equipment system
│   └── VYSTIA_MAP_SETUP.md     # Map configuration
├── tools/                       # Sprite creation tools (Python)
│   └── dwarf_sprite_*.py       # Dwarf sprite generation scripts
├── data/                        # Data files
└── services/                    # Service definitions
```

---

## 🎮 Where Is The Actual Code?

**The functional game code is in ServUO:**

- **Creatures:** `ServUO/Scripts/Mobiles/Vystia/` (131 creatures ✅ ACTIVE)
- **Items:** `ServUO/Scripts/Items/Vystia/` (✅ ACTIVE)
- **Commands:** `ServUO/Scripts/Commands/SpawnVystiaGump.cs` (✅ ACTIVE)

**In-game command:** `[spawnvystia <radius>]`

---

## 🎯 What Actually Works

Currently implemented in ServUO:

✅ **131 Creatures** - Compiled and spawnable (see creature list below)
✅ **10 Regional Bosses** - Fully functional with unique mechanics
✅ **100+ Items** - Ores, ingots, weapons, armor, reagents, artifacts
✅ **8 Regional Resource Sets** - Complete crafting material chains
✅ **8 Regional Weapon Sets** - Themed weapons with unique effects
✅ **8 Regional Armor Sets** - Matching armor with regional bonuses
✅ **Legendary Artifacts** - Rare boss drops and special items
✅ **Spawn Commands** - `[spawnvystia]` for creatures, `[add]` for items
✅ **Complete Documentation** - Bestiary and item inventory

**See:** [ITEMS_INVENTORY.md](ITEMS_INVENTORY.md) for complete item list

## 📋 What's Planned (NOT Implemented)

Future aspirational content:

❌ **23 Cities** - Designed but not built
❌ **18 Dungeons** - Designed but not placed
❌ **World Map** - Terrain generator exists but not deployed
❌ **Spawners** - Would need to be placed manually
❌ **Regions** - Need to be configured in ServUO
✅ **3 Deployment Options** - Choose how to add Vystia to your server
✅ **Procedural Generation** - JSON-driven world creation
✅ **UO:R Compatible** - All creatures use correct body types and resistances
✅ **Regional Resources** - Custom ores, ingots, and crafting materials per region  

---

## 🚀 Quick Start

### 1. Installation
1. **Read:** `guides/VYSTIA_INSTALLATION_GUIDE.md`
2. **Configure:** `config/VYSTIA_WORLD_CONFIG.json`
3. **Build:** ServUO project (files already linked)

### 2. Deployment Options
Choose your deployment method (see `guides/VYSTIA_DEPLOYMENT_GUIDE.md`):

#### **Option 1: New Map (RECOMMENDED)** ⭐
- Creates Vystia as **Map ID 5** (separate from Felucca/Trammel)
- No impact on existing world
- Players use moongates to travel
- Professional presentation

#### **Option 2: Replace Felucca**
- Replaces existing Felucca map
- Immediate access for all players
- Requires backup of existing world

#### **Option 3: Expansion Pack**
- Adds Vystia content to existing Felucca
- Integrates with current world
- Gradual implementation

### 3. World Generation
- **Use:** `[GenVystiaWorld]` to generate terrain and cities
- **Use:** `[GenVystiaSpawners]` to populate dungeons
- **Use:** `[VystiaStatus]` to check deployment status

---

## 🌍 World Features

### **Regions (23 Total)**
- **Frosthold & Winterguard** - Frozen tundra with ice giants
- **Emberlands** - Volcanic wasteland with fire creatures
- **Whispering Sands** - Desert kingdom with ancient tombs
- **Shadowfen** - Swamp wetlands with bog creatures
- **Verdantpeak** - Ancient forests with fey creatures
- **Crystal Barrens** - Glowing formations with crystal beings
- **Deepforge** - Underground dwarven city
- **Ironclad Empire** - Industrial steampunk nation
- **Skyreach Mountains** - Snow-capped peaks with griffins
- **Sunken Isles** - Underwater realm with sea creatures
- **Eternal Twilight** - Perpetual dusk with shadow beings
- **Golden Steppe** - Nomadic plains with horse tribes
- **Wilderlands** - Untamed wilderness with dire beasts
- **Forgotten Depths** - Abyssal trenches with krakens
- **Blazing Frontier** - Desert outpost with fire elementals
- **Verdant Isles** - Tropical paradise with jungle creatures
- **Mystic Canyons** - Desert canyons with sphinxes
- **Hollow Forests** - Shadowy woodlands with dark fey
- **Radiant Plains** - Sunlit grasslands with light creatures
- **Glimmering Archipelago** - Island chain with bioluminescent life

### **Major Cities (23 Total)**
1. **Ironheart Capital** - Industrial steampunk metropolis
2. **Frostholm** - Orcish ice fortress
3. **Verdant Grove** - Elven forest city
4. **Deepforge City** - Underground dwarven kingdom
5. **Emberforge** - Volcanic lava forge
6. **Sandara** - Desert trade hub with pyramids
7. **Crystal Spire** - Living crystal city
8. **Pearl Harbor** - Gateway to underwater realms
9. **Skyhold Monastery** - Mountain peak sanctuary
10. **Mistwood** - Swamp witch settlement
11. **Duskhaven** - Twilight shadow market
12. **Steppeheart** - Nomadic horse camp
13. **Wilderhaven** - Tribal beast pens
14. **Abyssal Gate** - Underwater portal station
15. **Sunfire Outpost** - Desert sun temple
16. **Verdant Port** - Jungle pirate haven
17. **Canyon Sanctum** - Desert observatory
18. **Hollowshade** - Shadow fae circle
19. **Radiance** - Solar farm settlement
20. **Glimmer Cove** - Bioluminescent gardens
21. **Shadowfen Dock** - Bog curse market
22. **Northwatch** - Ice market outpost
23. **Winterguard Citadel** - Frozen fortress

### **Dungeons (10 Total)**
1. **Frozen Halls** - Frost Giant fortress (4 levels)
2. **Deepforge Mines** - Dwarven underground excavation (5 levels)
3. **Pyramid of Surya** - Ancient desert tomb (3 levels)
4. **Emberdeep Caldera** - Volcanic dragon lair (3 levels)
5. **Shadowfen Crypts** - Cursed swamp tomb (3 levels)
6. **Verdant Depths** - Forest fey maze (2 levels)
7. **Crystal Caverns** - Living crystal maze (4 levels)
8. **Skyreach Summit** - Mountain peak sanctuary (3 levels)
9. **Abyssal Trench** - Deep ocean horror (5 levels)
10. **Twilight Labyrinth** - Shadow illusion maze (3 levels)

---

## 🎮 In-Game Commands

### **✅ Creature Spawning (WORKING)**
- `[spawnvystia]` - Opens gump to spawn Vystia creatures (10 tile radius)
- `[spawnvystia 20]` - Opens gump with custom radius (5-50)
- `[clearvystia]` - Delete all Vystia creatures in 10 tile radius
- `[clearvystia 50]` - Delete Vystia creatures in custom radius (5-100)

### **✅ Direct Spawning (WORKING)**
- `[add FrostFather]` - Spawn Frosthold boss
- `[add AncientTreant]` - Spawn Verdantpeak boss
- `[add IceTroll]` - Spawn Frosthold creature
- See [ACTUAL_USAGE.md](ACTUAL_USAGE.md) for complete working commands

### **❌ Commands That Don't Exist**
These are in design docs but NOT implemented:
- `[VystiaStatus`, `[VystiaDeploy`, `[GenVystiaWorld`, `[GenVystiaSpawners]`
- See [docs/PLANNED_COMMANDS.md](docs/PLANNED_COMMANDS.md) for aspirational commands

---

## 📚 Documentation

### **Core Documentation (`docs/`)**
- **[VYSTIA_WORLD_LORE.md](docs/VYSTIA_WORLD_LORE.md)** - Complete world background, geography, and lore
- **[VYSTIA_CREATURES_BESTIARY.md](docs/VYSTIA_CREATURES_BESTIARY.md)** - Master creature database (150+ creatures)
- **[VYSTIA_ITEMS_GUIDE.md](docs/VYSTIA_ITEMS_GUIDE.md)** - Items, equipment, and artifacts
- **[VYSTIA_CONTENT_ROADMAP.md](docs/VYSTIA_CONTENT_ROADMAP.md)** - Implementation roadmap

### **Implementation Guides (`guides/`)**
- **[VYSTIA_INSTALLATION_GUIDE.md](guides/VYSTIA_INSTALLATION_GUIDE.md)** - Setup and configuration
- **[VYSTIA_DEPLOYMENT_GUIDE.md](guides/VYSTIA_DEPLOYMENT_GUIDE.md)** - Deployment options
- **[VYSTIA_CREATURES_IMPLEMENTATION.md](guides/VYSTIA_CREATURES_IMPLEMENTATION.md)** - Creature implementation
- **[VYSTIA_WORLD_GENERATION.md](guides/VYSTIA_WORLD_GENERATION.md)** - World generation system

---

## 🔧 Technical Details

### **Compatibility**
- **ServUO** - Server emulator
- **UO:Renaissance** - Client compatibility (correct body types and resistances)
- **XMLSpawner2** - Spawning system (optional)

### **File Locations in ServUO**
- **Creatures:** `ServUO/Scripts/Mobiles/Vystia/` (131 creatures)
- **Items:** `ServUO/Scripts/Items/Vystia/` (resources, artifacts)
- **Commands:** `ServUO/Scripts/Commands/SpawnVystiaGump.cs` (spawn command)
- **Documentation:** `Vystia/docs/` (this directory - design docs only)

### **Creature Statistics**
- **Total Creatures:** 131 implemented
- **Regional Creatures:** 106 (across all biomes)
- **Dungeon Bosses:** 10 (unique encounters)
- **Misc/Shared Creatures:** 15
- **All UO:R Compatible:** Correct body types, resistances, and stats

### **Implemented Creature Regions**
| Region | Count | Theme |
|--------|-------|-------|
| Frosthold | 12 | Ice/cold creatures |
| Emberlands | 8 | Fire/lava creatures |
| Desert | 11 | Sand/heat creatures |
| Shadowfen | 13 | Swamp/poison creatures |
| Verdantpeak | 13 | Forest/nature creatures |
| Crystal Barrens | 4 | Crystal/energy creatures |
| Ironclad | 9 | Mechanical/steam creatures |
| Skyreach | 15 | Wind/lightning creatures |
| Underwater | 12 | Aquatic/deep sea creatures |
| ShadowVoid | 9 | Void/darkness creatures |
| Misc | 15 | Generic shared creatures |
| Bosses | 10 | Regional champions |

---

## 🎯 Next Steps

1. **Read the lore:** Start with `docs/Vystia World Lore.md`
2. **Check creatures:** Review `docs/VYSTIA_BESTIARY_UOR_CORRECTED.md`
3. **Plan implementation:** See `docs/VYSTIA_MISSING_CONTENT.md`
4. **Follow guides:** Use `guides/` for step-by-step implementation
5. **Generate world:** Use `[GenVystiaWorld]` in-game
6. **Populate dungeons:** Use `[GenVystiaSpawners]` in-game

---

## 📖 Additional Resources

- **Body Art Guide:** `docs/VYSTIA_ART_IMPLEMENTATION.md`
- **Faction System:** `docs/VYSTIA_FACTIONS_GUIDE.md`
- **Sea Systems:** `docs/VYSTIA_SEA_SYSTEMS.md`
- **Dungeon Bosses:** `docs/VYSTIA_DUNGEON_BOSSES.md`
- **Resource Management:** `guides/VYSTIA_RESOURCES_GUIDE.md`
- **Equipment System:** `guides/VYSTIA_EQUIPMENT_GUIDE.md`

---

**Vystia** - A world of endless adventure awaits!

*Last Updated: 2025-12-05*

---

## 📦 Creature Implementation Details

All creatures are located in `ServUO/Scripts/Mobiles/Vystia/` with the following structure:

```
Vystia/
├── Bosses/          # 10 regional boss creatures
├── Frosthold/       # 12 ice/cold creatures
├── Emberlands/      # 8 fire/lava creatures
├── Desert/          # 11 sand/heat creatures
├── Shadowfen/       # 13 swamp/poison creatures
├── Verdantpeak/     # 13 forest/nature creatures
├── CrystalBarrens/  # 4 crystal/energy creatures
├── Ironclad/        # 9 mechanical/steam creatures
├── Skyreach/        # 15 wind/lightning creatures
├── Underwater/      # 12 aquatic creatures
├── ShadowVoid/      # 9 void/darkness creatures
├── Misc/            # 15 generic shared creatures
└── Races/           # Custom race NPCs (Dwarf)
```

### Regional Resources
Each region drops unique crafting materials:

| Region | Ore | Ingot | Special Items |
|--------|-----|-------|---------------|
| Frosthold | FrostOre | GlacialIngot | PermafrostShard, EternalIce, FrostLotus |
| Emberlands | MoltenOre | EmberforgedIngot | EverburningCoal, LavaPearl, PhoenixFeather |
| Desert | SunstoneOre | DesertforgedIngot | ScarabCarapace, CactusPulp, SunblazeShard |
| Shadowfen | BogIronOre | MireforgedIngot | SwampMoss, WillowispEssence, RotlungSpore |
| Verdantpeak | LivingwoodLog | NatureforgedIngot | LivingBark, TreantHeart |
| Crystal Barrens | CrystalOre | CrystallineIngot | CrystalPollen, PrismaticShard |
| Ironclad | SteamworkOre | ClockworkIngot | GearSpring, PowerCore |
| Skyreach | WindstoneOre | ZephyrforgedIngot | CloudDown, StormEssence |
| Underwater | DeepwaterOre | TidalforgedIngot | SirenScale, AbyssalPearl |
| ShadowVoid | VoidstoneOre | ShadowforgedIngot | EchoingShard, VoidEssence |

### Legendary Artifacts
Rare drops from boss creatures:
- **HeartwoodCore** - Nature-themed shield (AncientTreant)
- **MagmaHeart** - Fire damage amulet (VolcanoWyrm)
- **LuminousScepter** - Energy mage weapon (CrystalDrakeAlpha)
- **MirrorOfTruth** - Reflection shield (TimewornLich)

---

## 🎓 Character Classes & Magic Systems

**New in 2025-12-05:** Complete character class system with custom magic schools!

### Character Classes

**Status:** 10/26 classes implemented (38.5%)

**Location:** `ServUO/Scripts/Custom/VystiaClasses/`
**Documentation:** See [IMPLEMENTATION_STATUS.md](IMPLEMENTATION_STATUS.md) for complete details

#### ✅ Implemented Classes (10)
1. **Barbarian** (Frosthold) - Rage Totem, melee DPS
2. **Ice Mage** (Frosthold) - 3 ice spells functional ✅
3. **Artificer** (Ironclad) - Clockwork constructs
4. **Fighter** (Ironclad) - Tank specialist
5. **Druid** (Verdantpeak) - Shapeshifting, nature magic
6. **Ranger** (Desert) - Archery, tracking
7. **Wizard** (Crystal Barrens) - Standard magery
8. **Cleric** (Multi-Regional) - AoE healing
9. **Paladin** (Multi-Regional) - Chivalry focus
10. **Witch** (Shadowfen) - Hex magic

#### ⏳ Pending Classes (16)
Beastmaster, Sorcerer, Illusionist, Warlock, Alchemist, Oracle, Monk, Templar, Necromancer, Summoner, Bounty Hunter, Knight, Shaman, Bard, Enchanter, Rogue

### Magic Spell System

**Status:** ✅ All 12 magic schools completely designed (384 total spells)

**Design Location:** `Vystia/Magic/`
**Implementation Location:** `ServUO/Scripts/Custom/VystiaClasses/Spells/`

| School | Class | Spells Designed | Spells Implemented | Design Doc |
|--------|-------|-----------------|-------------------|------------|
| Ice Magic | Ice Mage | 32 | 3 ✅ | [IceMagic.md](Magic/IceMagic.md) |
| Nature Magic | Druid | 32 | 0 | [NatureMagic.md](Magic/NatureMagic.md) |
| Hex Magic | Witch | 32 | 0 | [HexMagic.md](Magic/HexMagic.md) |
| Elemental Magic | Sorcerer | 32 | 0 | [ElementalMagic.md](Magic/ElementalMagic.md) |
| Dark Magic | Warlock | 32 | 0 | [DarkMagic.md](Magic/DarkMagic.md) |
| Divination | Oracle | 32 | 0 | [DivinationMagic.md](Magic/DivinationMagic.md) |
| Necromancy | Necromancer | 32 | 0 | [Necromancy.md](Magic/Necromancy.md) |
| Summoning | Summoner | 32 | 0 | [SummoningMagic.md](Magic/SummoningMagic.md) |
| Shamanic | Shaman | 32 | 0 | [ShamanicMagic.md](Magic/ShamanicMagic.md) |
| Bardic | Bard | 32 | 0 | [BardicMagic.md](Magic/BardicMagic.md) |
| Enchanting | Enchanter | 32 | 0 | [EnchantingMagic.md](Magic/EnchantingMagic.md) |
| Illusion | Illusionist | 32 | 0 | [IllusionMagic.md](Magic/IllusionMagic.md) |

**Total:** 384 spells designed, 3 implemented (0.8%)

**See:** [Magic/README.md](Magic/README.md) for complete spell index

### Implemented Ice Mage Spells ✅

1. **Ice Bolt** - Circle 3, single target cold damage + slow
2. **Frost Armor** - Circle 4, resistance buff
3. **Blizzard** - Circle 6, AoE DoT storm

**Status:** All functional (API fixed 2025-12-05)

### Custom Race: Dwarves ✅

**Status:** ✅ Complete with custom sprites

- Body IDs: 987 (male), 988 (female)
- 75% human scale
- Custom animations for equipment
- Commands: `[sd]`, `[sdm]`, `[sdf]`

**Tools:** Sprite creation tools in `tools/dwarf_sprite_*.py`

---

## 📊 Implementation Status

### What's Complete ✅
- ✅ 131 Creatures (100%)
- ✅ Resource System (100%)
- ✅ Dwarf Race (100%)
- ✅ Class System Core (100%)
- ✅ 10 Character Classes (38.5%)
- ✅ 6 Custom Ability Items (100%)
- ✅ Magic Spell Designs (100% - all 12 schools)
- ✅ 3 Ice Mage Spells (0.8% of total spells)

### What's In Progress 🟡
- 🟡 16 Character Classes pending
- 🟡 381 Spells pending implementation
- 🟡 Magic school systems (shapeshifting, summoning, etc.)

### Build Status
**Current:** ✅ CLEAN BUILD (0 errors, 0 warnings)

---

## 📖 Complete Documentation

**Primary Documentation:**
- **[IMPLEMENTATION_STATUS.md](IMPLEMENTATION_STATUS.md)** - Complete implementation overview
- **[Magic/README.md](Magic/README.md)** - Magic spell system index

**ServUO Code Documentation:**
- **[VystiaClasses/CLAUDE.md](../ServUO/Scripts/Custom/VystiaClasses/CLAUDE.md)** - AI assistant guide
- **[VystiaClasses/README.md](../ServUO/Scripts/Custom/VystiaClasses/README.md)** - Class implementation guide
- **[VystiaClasses/KNOWN_ISSUES.md](../ServUO/Scripts/Custom/VystiaClasses/KNOWN_ISSUES.md)** - Bug tracking (all resolved!)

---

*Last Updated: 2025-12-05 - Added Character Classes & Magic Systems*