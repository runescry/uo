# ServUO Scripts Directory - Glossary & Overview

## Table of Contents
1. [Core Systems](#core-systems)
2. [Quest Systems](#quest-systems)
3. [Services & Features](#services--features)
4. [Items & Equipment](#items--equipment)
5. [Mobiles & NPCs](#mobiles--npcs)
6. [Skills & Spells](#skills--spells)
7. [Commands & Administration](#commands--administration)
8. [Expansions & Content](#expansions--content)

---

## Core Systems

### Abilities
**Location:** `Abilities/`  
**Purpose:** Weapon abilities and combat special moves  
**Key Features:**
- Weapon abilities (Whirlwind Attack, Armor Ignore, Mortal Strike, etc.)
- Slayer weapon groups and bonuses
- Enhancement system for weapons
- Special attack timers and effects

### Accounting
**Location:** `Accounting/`  
**Purpose:** Account management and security  
**Key Features:**
- Account creation and management
- Access restrictions and firewall
- IP limiting and attack prevention
- Account comments and tags

### Commands
**Location:** `Commands/`  
**Purpose:** Administrative and gameplay commands  
**Key Features:**
- World generation tools (CreateWorld, Decorate, DoorGenerator)
- Player management (Skills, Attributes, Properties)
- Debugging tools (Profiling, Logging, Docs)
- Generic command system with extensions and implementors

### Gumps
**Location:** `Gumps/`  
**Purpose:** User interface windows and dialogs  
**Key Features:**
- House management gumps
- Guild system interfaces
- Bank and vendor interfaces
- Quest and reward gumps
- Property editing interfaces

---

## Quest Systems

### Base Quest Framework
**Location:** `Quests/BaseQuest.cs`  
**Purpose:** Core quest system infrastructure  
**Features:**
- Quest chains and progression
- Objectives tracking
- Reward distribution
- Quest completion tracking

### Quest Types

#### Story Quests
- **Uzeraan Turmoil** - Classic new player quest chain
- **The Ritual** - Puzzle-based quest
- **Dark Tides** - Multi-objective story quest
- **Eminos Undertaking** - Complex narrative quest
- **Haochis Trials** - Trial-based progression

#### Collection Quests
- **Collector Quest** - Item collection objectives
- **Ambitious Solen Queen** - Hive-based collection
- **Cloak of Humility** - Virtue-based collection quest

#### Heritage Quests
- **Human Heritage** - Human race questline
- **Elf Heritage** - Elven race questline

#### Mastery Quests
- **Bard Mastery Quests** - Sir Berran, Sir Felean, Sir Hareus
- **Spellweaving** - Arcane magic quest
- **Knowledge of the Soulforge** - Crafting mastery

#### Eodon Quests
- **Hawkwind** - Time for Legends quest
- **Myrmidex Threat** - Invasion-based quest
- **Valley of One Quest** - Dragon turtle and volcano quests

#### Tiered Quests
- **Percolem Tiered Quests** - Progressive difficulty quests
- **Thepem Tiered Quests** - Tier-based objectives
- **Zosilem Tiered Quests** - Multi-tier progression

#### Special Quests
- **Escortables** - NPC escort missions
- **Bedlam** - Dungeon-based quest
- **Blighted Grove** - Area-specific quest
- **Ghost of Covetous** - Dungeon exploration quest

---

## Services & Features

### LLM (Large Language Model) Service
**Location:** `Services/LLM/`  
**Purpose:** AI-powered NPC conversations using OpenAI  
**Features:**
- 25+ pre-configured NPC personalities
- Dynamic conversation generation
- Personality-based responses (Blacksmith, Archmage, Bard, etc.)
- Conversation history and cleanup
- Commands: `[SpawnLLMPersonality`, `[SpawnTownNPCs`, etc.

### Champion System
**Location:** `Services/ChampionSystem/`  
**Purpose:** Boss spawn system with progression  
**Features:**
- Champion spawns at various locations
- Progression through ranks (0-3)
- Power scroll and stat scroll rewards
- Gold shower rewards
- Harrower boss encounter
- Rotation system for spawns

### Bulk Order System (BODs)
**Location:** `Services/BulkOrders/`  
**Purpose:** Crafting order system  
**Features:**
- Small and large bulk orders
- Material type requirements
- Exceptional item requirements
- BOD books for organization
- Reward system with points
- Cached deed system

### Craft System
**Location:** `Services/Craft/`  
**Purpose:** Comprehensive crafting system  
**Crafting Skills:**
- Alchemy
- Blacksmithy
- Bow Fletching
- Carpentry
- Cartography
- Cooking
- Glassblowing
- Inscription
- Masonry
- Tailoring
- Tinkering

### Factions
**Location:** `Services/Factions/`  
**Purpose:** Player vs. player faction warfare  
**Features:**
- Multiple competing factions
- Faction guards and vendors
- Faction-specific items
- Faction warfare mechanics

### Virtues System
**Location:** `Services/Virtues/`  
**Purpose:** Virtue-based gameplay mechanics  
**Features:**
- Virtue tracking and progression
- Virtue-based rewards
- Virtue artifacts

### City Loyalty System
**Location:** `Services/City Loyalty System/`  
**Purpose:** City-based loyalty and trading  
**Features:**
- City loyalty tracking
- Elections system
- Trading system
- City-specific rewards

### Vice vs. Virtue
**Location:** `Services/ViceVsVirtue/`  
**Purpose:** PvP battle system  
**Features:**
- Battle system mechanics
- Vice and Virtue alignment
- Battle rewards

### Seasonal Events
**Location:** `Services/Seasonal Events/`  
**Purpose:** Time-limited event content  
**Events:**
- Treasures of Tokuno
- Treasures of Sorcerer's Dungeon
- Treasures of Kotl City
- Treasures of Khaldun
- Treasures of Doom
- Rising Tide
- Krampus Encounter

### Revamped Dungeons
**Location:** `Services/Revamped Dungeons/`  
**Purpose:** Enhanced dungeon experiences  
**Dungeons:**
- **Blackthorn Dungeon** - Invasion spawner, unique artifacts
- **Covetous Void Spawn** - Void creature encounters
- **Despise Revamped** - Enhanced Despise dungeon
- **Shame Revamped** - Enhanced Shame dungeon
- **The Exodus Encounter** - Boss encounter system
- **Wrong Dungeon** - Quest-based dungeon

### Community Collections
**Location:** `Services/CommunityCollections/`  
**Purpose:** Community-driven collection system  
**Features:**
- Museum donation boxes
- Royal zoo donation boxes
- Collection rewards
- Title rewards

### Treasure Maps
**Location:** `Services/TreasureMaps/`  
**Purpose:** Treasure hunting system  
**Features:**
- Map levels and difficulty
- Treasure chest generation
- Map decoding mechanics

### Pet Training
**Location:** `Services/Pet Training/`  
**Purpose:** Animal training and progression  
**Features:**
- Pet skill training
- Pet stat progression
- Training points system

### Plants System
**Location:** `Services/Plants/`  
**Purpose:** Gardening and plant cultivation  
**Features:**
- Plant growth system
- Resource harvesting
- Plant breeding

### Harvest System
**Location:** `Services/Harvest/`  
**Purpose:** Resource gathering  
**Features:**
- Mining
- Lumberjacking
- Fishing
- Resource respawn mechanics

### Astronomy System
**Location:** `Services/Astronomy/`  
**Purpose:** Astronomical observation system  
**Features:**
- Constellation tracking
- Telescope mechanics
- Star charts
- Astronomy rewards

### Basket Weaving
**Location:** `Services/BasketWeaving/`  
**Purpose:** Crafting specialty system  
**Features:**
- Basket crafting
- Natural dyes
- Plant pigments
- Silver sapling rewards

### Armor Refinement
**Location:** `Services/Armor Refinement/`  
**Purpose:** Armor enhancement system  
**Features:**
- Armor refinement mechanics
- Refinement gumps
- Refinement items

### PvP Arena System
**Location:** `Services/PVP Arena System/`  
**Purpose:** Structured PvP combat  
**Features:**
- Arena matches
- Ranking system
- Arena rewards

### Huntmaster Challenge
**Location:** `Services/HuntmasterChallenge/`  
**Purpose:** Hunting challenge system  
**Features:**
- Hunting objectives
- Challenge tracking
- Rewards system

### Instanced Peerless
**Location:** `Services/InstancedPeerless/`  
**Purpose:** Instance-based boss encounters  
**Features:**
- Private instance generation
- Peerless boss encounters
- Instance cleanup

### Mini Champion System
**Location:** `Services/MiniChampionSystem/`  
**Purpose:** Smaller-scale champion encounters  
**Features:**
- Mini champion spawns
- Reduced difficulty encounters

### Points Systems
**Location:** `Services/PointsSystems/`  
**Purpose:** Various point-based reward systems  
**Features:**
- Virtue artifacts system
- Points tracking
- Reward redemption

### Veteran Rewards
**Location:** `Services/VeteranRewards/`  
**Purpose:** Rewards for veteran players  
**Features:**
- Account age tracking
- Reward tiers
- Veteran reward selection

### Town Cryer
**Location:** `Services/Town Cryer/`  
**Purpose:** News and information system  
**Features:**
- News articles
- Town cryer NPCs
- Information distribution

### Chat System
**Location:** `Services/Chat/`  
**Purpose:** In-game chat channels  
**Features:**
- Multiple chat channels
- Chat commands
- Chat logging

### Party System
**Location:** `Services/Party/`  
**Purpose:** Player grouping mechanics  
**Features:**
- Party formation
- Party sharing
- Party management

### Pathing System
**Location:** `Services/Pathing/`  
**Purpose:** NPC movement and pathfinding  
**Features:**
- Pathfinding algorithms
- Movement optimization

### Exploring the Deep
**Location:** `Services/ExploringTheDeep/`  
**Purpose:** Underwater exploration content  
**Features:**
- Underwater areas
- Deep sea creatures
- Underwater quests

### New Magincia
**Location:** `Services/New Magincia/`  
**Purpose:** Rebuilt Magincia content  
**Features:**
- Housing lotto system
- Plant system
- Distillation system

### Ultima Store
**Location:** `Services/UltimateStore/`  
**Purpose:** In-game store system  
**Features:**
- Store interface
- Item purchasing
- Currency management

### Reports System
**Location:** `Services/Reports/`  
**Purpose:** Player reporting system  
**Features:**
- Bug reports
- Staff reports
- Report tracking

### Remote Admin
**Location:** `Services/RemoteAdmin/`  
**Purpose:** Remote server administration  
**Features:**
- Remote access
- Admin commands
- Server monitoring

### XmlSpawner
**Location:** `Services/XmlSpawner/`  
**Purpose:** XML-based spawner system  
**Features:**
- XML configuration
- Spawn management
- Event triggers

### Spawner Service
**Location:** `Services/Spawner/`  
**Purpose:** Core spawner functionality  
**Features:**
- Creature spawning
- Spawn management
- Respawn mechanics

### Doom
**Location:** `Services/Doom/`  
**Purpose:** Doom dungeon mechanics  
**Features:**
- Gauntlet spawner
- Lever puzzle controller
- Guardian rooms

### Khaldun
**Location:** `Services/Khaldun/`  
**Purpose:** Khaldun dungeon content  
**Features:**
- Khaldun mechanics
- Special encounters

### Malas
**Location:** `Services/Malas/`  
**Purpose:** Malas facet content  
**Features:**
- Malas-specific content
- Regional mechanics

### Underworld
**Location:** `Services/Underworld/`  
**Purpose:** Underworld area content  
**Features:**
- Underworld encounters
- Special mechanics

### Tomb of Kings
**Location:** `Services/Tomb of Kings/`  
**Purpose:** Tomb of Kings dungeon  
**Features:**
- Tomb mechanics
- Special encounters

### Ethics System
**Location:** `Services/Ethics/`  
**Purpose:** Hero/Evil alignment system  
**Features:**
- Hero path
- Evil path
- Ethics-based abilities

### Clean Up Britannia
**Location:** `Services/CleanUpBritannia/`  
**Purpose:** Environmental cleanup quests  
**Features:**
- Cleanup objectives
- Point exchange
- Rewards system

### Gift Giving
**Location:** `Services/GiftGiving/`  
**Purpose:** Gift exchange system  
**Features:**
- Gift mechanics
- Gift rewards

### Help System
**Location:** `Services/Help/`  
**Purpose:** In-game help system  
**Features:**
- Help topics
- Help commands
- Documentation

### Loyalty System
**Location:** `Services/LoyaltySystem/`  
**Purpose:** Player loyalty tracking  
**Features:**
- Loyalty points
- Loyalty rewards

### Mondain's Legacy Quests
**Location:** `Services/MondainsLegacyQuests/`  
**Purpose:** Expansion quest content  
**Features:**
- Legacy quests
- Expansion storylines

### Monster Stealing
**Location:** `Services/Monster Stealing/`  
**Purpose:** Theft mechanics from monsters  
**Features:**
- Stealing from creatures
- Stealable items
- Stealing mechanics

### Player Zombies
**Location:** `Services/PlayerZombies.cs`  
**Purpose:** Zombie player mechanics  
**Features:**
- Zombie transformation
- Zombie gameplay

### Paragon System
**Location:** `Services/Paragon.cs`  
**Purpose:** Paragon creature system  
**Features:**
- Paragon spawns
- Enhanced creatures
- Paragon rewards

### Pumpkin Patch
**Location:** `Services/PumpkinPatch.cs`  
**Purpose:** Seasonal pumpkin content  
**Features:**
- Pumpkin growing
- Seasonal mechanics

### Trick or Treat
**Location:** `Services/TrickOrTreat.cs`  
**Purpose:** Halloween event mechanics  
**Features:**
- Trick or treat system
- Halloween rewards

### Daily Rares
**Location:** `Services/DailyRares.cs`  
**Purpose:** Daily rare item spawns  
**Features:**
- Rare item generation
- Daily rotation

### Assistants
**Location:** `Services/Assistants.cs`  
**Purpose:** Assistant NPC system  
**Features:**
- Assistant mechanics
- Helper NPCs

### Prevent Inaccess
**Location:** `Services/PreventInaccess.cs`  
**Purpose:** Prevents inaccessible locations  
**Features:**
- Location validation
- Access prevention

### Item Fixes
**Location:** `Services/ItemFixes.cs`  
**Purpose:** Item bug fixes and corrections  
**Features:**
- Item corrections
- Compatibility fixes

### Disguise Persistence
**Location:** `Services/DisguisePersistence.cs`  
**Purpose:** Disguise kit persistence  
**Features:**
- Disguise saving
- Appearance persistence

### Test Center
**Location:** `Services/TestCenter.cs`  
**Purpose:** Testing environment features  
**Features:**
- Test mechanics
- Debug features

### Vendor Searching
**Location:** `Services/Vendor Searching/`  
**Purpose:** Vendor search functionality  
**Features:**
- Vendor search
- Item location

### Fire Casino
**Location:** `Services/FireCasino/`  
**Purpose:** Casino gambling system  
**Features:**
- Casino games
- Gambling mechanics

### Loot Generation
**Location:** `Services/LootGeneration/`  
**Purpose:** Advanced loot system  
**Features:**
- Loot tables
- Item generation
- Loot distribution

---

## Items & Equipment

### Item Categories

#### Artifacts
**Location:** `Items/Artifacts/`  
**Purpose:** Unique powerful items  
**Types:**
- Consumables
- Decorative
- Equipment (Armor sets, Weapons, Jewelry, Talismans)
- Tools

#### Equipment
**Location:** `Items/Equipment/`  
**Purpose:** Wearable items  
**Types:**
- Armor (Multiple sets: Acolyte, Assassin, Bestial, DaemonBone, etc.)
- Clothing
- Glasses
- Instruments
- Jewelry
- Light sources
- Quivers
- Spellbooks
- Talismans
- Weapons
- Suits (Complete outfits)

#### Addons
**Location:** `Items/Addons/`  
**Purpose:** House addons and decorations  
**Types:**
- Aquarium (Fish, Rewards)
- Craft Addons
- Crystal Furniture Set
- Dawn's Music Box
- Shadow Furniture Set
- The King's Collection

#### Functional Items
**Location:** `Items/Functional/`  
**Purpose:** Utility items  
**Types:**
- Automaton
- Jewelry Box
- Seed Box

#### Consumables
**Location:** `Items/Consumables/`  
**Purpose:** Single-use items  
**Types:**
- Potions
- Food
- Scrolls

#### Containers
**Location:** `Items/Containers/`  
**Purpose:** Storage items  
**Types:**
- Bags
- Boxes
- Mahjong sets

#### Tools
**Location:** `Items/Tools/`  
**Purpose:** Crafting and utility tools  
**Types:**
- Crafting tools
- Repair tools
- Utility tools

#### Books
**Location:** `Items/Books/`  
**Purpose:** Reading materials  
**Types:**
- Recipe books
- Special scroll books
- Lore books

#### Resources
**Location:** `Items/Resource/`  
**Purpose:** Crafting materials  
**Types:**
- Ores
- Logs
- Hides
- Cloth
- Reagents

#### Quest Items
**Location:** `Items/Quest/`  
**Purpose:** Quest-specific items  
**Types:**
- Quest objectives
- Quest rewards
- Quest keys

#### Decorative
**Location:** `Items/Decorative/`  
**Purpose:** Decoration items  
**Types:**
- Artisan festival rewards
- Decorative stable set
- Various decorations

#### Damageable
**Location:** `Items/Damageable/`  
**Purpose:** Items that can be damaged  
**Types:**
- Breakable items
- Durability system

#### Corpses
**Location:** `Items/Corpses/`  
**Purpose:** Corpse handling  
**Types:**
- Creature corpses
- Loot containers

#### Internal
**Location:** `Items/Internal/`  
**Purpose:** System items  
**Types:**
- Item sockets
- Internal mechanics

#### Store Bought
**Location:** `Items/StoreBought/`  
**Purpose:** Purchasable items  
**Types:**
- Decorative dungeon set
- Store items

---

## Mobiles & NPCs

### Mobile Categories

#### Bosses
**Location:** `Mobiles/Bosses/`  
**Purpose:** Major encounter creatures  
**Types:**
- Harrower
- Navery
- Other major bosses

#### Named Creatures
**Location:** `Mobiles/Named/`  
**Purpose:** Unique named creatures  
**Types:**
- Named monsters
- Special encounters

#### Normal Creatures
**Location:** `Mobiles/Normal/`  
**Purpose:** Standard creatures  
**Types:**
- Common monsters
- Wildlife
- Standard NPCs

#### NPCs
**Location:** `Mobiles/NPCs/`  
**Purpose:** Non-player characters  
**Types:**
- Vendors
- Guards
- Quest givers
- Mannequins (with property system)

#### Summons
**Location:** `Mobiles/Summons/`  
**Purpose:** Summoned creatures  
**Types:**
- Spell summons
- Pet summons

#### Void Creatures
**Location:** `Mobiles/Void Creatures/`  
**Purpose:** Void-based creatures  
**Types:**
- Void spawns
- Dimensional creatures

#### Event Creatures
**Location:** `Mobiles/Event/`  
**Purpose:** Event-specific creatures  
**Types:**
- Seasonal creatures
- Event monsters

#### Faction Creatures
**Location:** `Mobiles/Factions/`  
**Purpose:** Faction-related creatures  
**Types:**
- Faction guards
- Faction NPCs

#### AI Systems
**Location:** `Mobiles/AI/`  
**Purpose:** Artificial intelligence  
**Types:**
- Magical AI
- Combat AI
- Behavior AI

---

## Skills & Spells

### Skills
**Location:** `Skills/`  
**Purpose:** Character skill system  
**Skills Include:**
- Anatomy
- Animal Lore
- Animal Taming
- Arms Lore
- Begging
- Detect Hidden
- Discordance
- Eval Int
- Forensic Eval
- Hiding
- Inscribe
- Item Identification
- Meditation
- Peacemaking
- Poisoning
- Provocation
- Remove Trap
- Snooping
- Spirit Speak
- Stealing
- Stealth
- Taste ID
- Tracking

### Spells
**Location:** `Spells/`  
**Purpose:** Magic spell system  
**Spell Circles:**
- First Circle
- Second Circle
- Third Circle
- Fourth Circle
- Fifth Circle
- Sixth Circle
- Seventh Circle
- Eighth Circle

**Special Spell Types:**
- Bushido
- Chivalry
- Necromancy
- Ninjitsu
- Mysticism
- Spellweaving
- Gargoyle Spells
- Skill Masteries (Bard Spells)

---

## Commands & Administration

### Command Categories

#### World Generation
- `[CreateWorld` - World creation
- `[Decorate` - Decoration placement
- `[DoorGenerator` - Door generation
- `[GenTeleporter` - Teleporter generation
- `[GenBounds` - Boundary generation
- `[GenCategorization` - Categorization generation

#### Player Management
- `[Skills` - Skill management
- `[Attributes` - Attribute management
- `[Properties` - Property editing
- `[GMbody` - GM body management

#### Debugging & Tools
- `[Profiling` - Performance profiling
- `[Logging` - Log management
- `[Docs` - Documentation generation
- `[GenerateGameDocs` - Game documentation

#### Administrative
- `[Add` - Item/mobile addition
- `[Dupe` - Item duplication
- `[Wipe` - World wiping
- `[Mark` - Location marking
- `[TimeStamp` - Timestamp management

#### Generic Command System
**Location:** `Commands/Generic/`  
**Purpose:** Extensible command framework  
**Features:**
- Base command system
- Command extensions (Distinct, Limit, Sort, Where)
- Command implementors (Area, Contained, Facet, Global, etc.)
- Conditional compilation

---

## Expansions & Content

### High Seas
**Location:** `Services/Expansions/High Seas/`  
**Purpose:** Naval expansion content  
**Features:**
- Ships and sailing
- Cannons and naval combat
- Fishing system
- SOS (Ships of the Sea) artifacts
- Professional bounty quests
- Profession fish quests
- Corgul boss encounter

### Time of Legends
**Location:** `Services/Expansions/Time Of Legends/`  
**Purpose:** Latest expansion content  
**Features:**
- Auction safe system
- Myrmidex Invasion
- Shadowguard content
- New items and mobiles

---

## Additional Systems

### Multis
**Location:** `Multis/`  
**Purpose:** Multi-tile structures  
**Types:**
- Houses
- Ships
- Large structures

### Regions
**Location:** `Regions/`  
**Purpose:** Area definitions  
**Types:**
- Dungeon regions
- Town regions
- Special areas

### Targets
**Location:** `Targets/`  
**Purpose:** Targeting system  
**Types:**
- Target definitions
- Target handlers

### Vendor Info
**Location:** `VendorInfo/`  
**Purpose:** Vendor system data  
**Types:**
- Vendor definitions
- Shop information
- Vendor behaviors

### Misc
**Location:** `Misc/`  
**Purpose:** Miscellaneous utilities  
**Types:**
- Helper classes
- Utility functions
- System utilities

### Context Menus
**Location:** `Context Menus/`  
**Purpose:** Right-click menu options  
**Types:**
- Add to party
- Eat entry
- Open bank
- Teach entry
- Eject player

### Deprecated
**Location:** `Deprecated/`  
**Purpose:** Legacy/removed features  
**Types:**
- Old systems
- Unused code

---

## Summary Statistics

- **Total Script Files:** ~6,000+ C# files
- **Quest Files:** 136 quest-related files
- **Item Files:** 2,714 item files
- **Mobile Files:** 977 mobile files
- **Service Files:** 1,641 service files
- **Spell Files:** 213 spell files
- **Skill Files:** 24 skill files

---

## Key Features Overview

### Core Gameplay
- Complete skill system (24+ skills)
- Full spell system (8 circles + specializations)
- Comprehensive crafting system (11+ crafts)
- Quest system with 100+ quests
- Champion spawn system
- Faction warfare

### Advanced Systems
- LLM-powered NPC conversations
- Bulk order system
- Pet training system
- Plant/gardening system
- Astronomy system
- Community collections

### Content
- Multiple expansions (High Seas, Time of Legends)
- Revamped dungeons (6+ major dungeons)
- Seasonal events (7+ event types)
- Heritage quests
- Mastery quests
- Tiered quests

### PvP & Social
- Faction system
- Vice vs. Virtue
- PvP Arena
- Party system
- Chat system
- Guild system

### Administration
- Comprehensive command system
- World generation tools
- Debugging tools
- Reporting system
- Remote admin

---

*This glossary represents the major systems and features in the ServUO scripts directory. For specific implementation details, refer to the individual script files.*

