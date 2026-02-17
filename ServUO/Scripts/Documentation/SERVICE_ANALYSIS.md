# ServUO Services Analysis Documentation

This document provides a comprehensive overview of all services in the ServUO Services folder, including what each service does, how it works, how to enable it, and how it interacts with other systems.

**Last Updated:** 2025-01-XX

---

## Table of Contents

1. [Standalone Service Files](#standalone-service-files)
2. [Core Systems](#core-systems)
3. [Gameplay Systems](#gameplay-systems)
4. [Content Systems](#content-systems)
5. [Feature Systems](#feature-systems)
6. [Utility Systems](#utility-systems)

---

## Standalone Service Files

### Assistants

**Location:** `ServUO/Scripts/Services/Assistants.cs`

**Description:** Manages assistant program negotiation (Razor, AssistUO) to control which client features are allowed or disallowed on the server. Prevents players from using unauthorized assistant features.

**Key Files:**
- `Assistants.cs` - Main implementation

**Enable/Disable:**
- Method: Static property `Assistants.Settings.Enabled`
- Default: `false` (disabled)
- Configuration: Set `Assistants.Settings.Enabled = true` in code or via `Configure()` method

**Initialization:**
- Called via: `Assistants.Negotiator.Initialize()` during `ScriptCompiler.Invoke("Initialize")`
- Timing: Server startup, after scripts compile

**Core Mechanics:**
- Registers protocol extension (0xFF) for assistant handshake
- On player login, sends handshake packet with disallowed features list
- If handshake fails or times out, can disconnect player (if `KickOnFailure` is true)
- Features can be disallowed via `DisallowFeature()` method (e.g., autoloot, macros, etc.)
- Supports both Razor and AssistUO assistants

**Dependencies:**
- None

**Interactions:**
- Uses `EventSink.Login` to detect player logins
- Uses `ProtocolExtensions.Register()` for custom packet handling
- Can interact with `NetState` to disconnect players

**Configuration Options:**
- `Enabled` - Enable/disable the negotiator
- `KickOnFailure` - Disconnect players who fail handshake (default: true)
- `HandshakeTimeout` - Time to wait for handshake response (default: 30 seconds)
- `DisconnectDelay` - Warning message display time before disconnect (default: 15 seconds)
- `DisallowedFeatures` - Bit flags for disallowed features (FilterWeather, AutoOpenDoors, etc.)

---

### DailyRares

**Location:** `ServUO/Scripts/Services/DailyRares.cs`

**Description:** Spawns decorative rare items at fixed locations across multiple maps. These items are meant to be decorative collectibles that spawn once and persist.

**Key Files:**
- `DailyRares.cs` - Contains `DailyRaresSpawner` and item definitions

**Enable/Disable:**
- Method: `Config.Get("DailyRares.Enabled", true)`
- Default: `true` (enabled)
- Configuration: Add `DailyRares.Enabled=true` or `DailyRares.Enabled=false` to `Config/` configuration files

**Initialization:**
- Called via: `DailyRaresSpawner.Initialize()` during `ScriptCompiler.Invoke("Initialize")`
- Timing: Server startup

**Core Mechanics:**
- Checks if items exist at predefined locations on each map
- If items don't exist, spawns them at fixed coordinates
- Items are set with `LastMoved` far in the future to prevent decay
- Spawns items on Felucca, Trammel, Ilshenar, and Malas maps
- Items include: DailyRocks, DailyRock, FruitBasket, ClosedBarrel, CandleLarge, DailyFullJars, DecoHay2, DailyBrokenChair, DailyMeatPie, DailySeaweed

**Dependencies:**
- None

**Interactions:**
- Uses `Map.FindItem<T>()` to check for existing items
- Spawns items directly to world locations

**Configuration Options:**
- `DailyRares.Enabled` - Enable/disable the spawner (default: true)

---

### DisguisePersistence

**Location:** `ServUO/Scripts/Services/DisguisePersistence.cs`

**Description:** Persists player disguise timers and name modifications across server restarts. Saves disguise state to disk so players maintain their disguises after server reboots.

**Key Files:**
- `DisguisePersistence.cs` - Main implementation

**Enable/Disable:**
- Method: Always enabled (no configuration flag)
- Default: Enabled
- Configuration: N/A

**Initialization:**
- Called via: `DisguisePersistence.Configure()` during `ScriptCompiler.Invoke("Configure")`
- Timing: Server startup, before world load

**Core Mechanics:**
- Registers `EventSink.WorldSave` and `EventSink.WorldLoad` handlers
- On save: Serializes all active disguise timers, remaining time, and name modifications to `Saves/Disguises/Persistence.bin`
- On load: Deserializes disguise data and recreates timers for players
- Integrates with `DisguiseTimers` system to track active disguises

**Dependencies:**
- `DisguiseTimers` system (from disguise kit functionality)

**Interactions:**
- Uses `EventSink.WorldSave` and `EventSink.WorldLoad` events
- Reads/writes to `DisguiseTimers.Timers` dictionary
- Modifies `Mobile.NameMod` property

**Configuration Options:**
- None (always active)

---

### HolidaySettings

**Location:** `ServUO/Scripts/Services/HolidaySettings.cs`

**Description:** Defines settings and item lists for Halloween holiday events. Provides static configuration for trick-or-treat treats and GM beggar items.

**Key Files:**
- `HolidaySettings.cs` - Configuration class

**Enable/Disable:**
- Method: Date-based (checks current date against `StartHalloween` and `FinishHalloween`)
- Default: Active during Halloween period (Oct 24 - Nov 15, 2012)
- Configuration: Modify `StartHalloween` and `FinishHalloween` DateTime properties

**Initialization:**
- Called via: Referenced by other Halloween services (TrickOrTreat, PlayerZombies, PumpkinPatch)
- Timing: N/A (static configuration class)

**Core Mechanics:**
- Provides static properties for Halloween start/end dates
- Contains arrays of treat item types and GM beggar item types
- `RandomTreat` property returns random treat item
- `RandomGMBeggerItem` property returns random GM beggar item

**Dependencies:**
- None (used by other services)

**Interactions:**
- Used by `TrickOrTreat`, `PlayerZombies`, and `PumpkinPatch` services
- Provides item type lists for Halloween events

**Configuration Options:**
- `StartHalloween` - DateTime when Halloween starts (default: Oct 24, 2012)
- `FinishHalloween` - DateTime when Halloween ends (default: Nov 15, 2012)

---

### ItemFixes

**Location:** `ServUO/Scripts/Services/ItemFixes.cs`

**Description:** Applies fixes to tile data flags and properties for specific item IDs. Corrects missing or incorrect flags in the tile data table.

**Key Files:**
- `ItemFixes.cs` - Main implementation

**Enable/Disable:**
- Method: Always enabled (no configuration flag)
- Default: Enabled
- Configuration: N/A

**Initialization:**
- Called via: `ItemFixes.Initialize()` during `ScriptCompiler.Invoke("Initialize")`
- Timing: Server startup

**Core Mechanics:**
- Directly modifies `TileData.ItemTable` entries
- Adds `NoShoot` flags to specific item IDs (0x2A0, 0x3E0, 0x3E1)
- Fixes incorrect height for item ID 0x34D2
- Removes `Wall` flag and adds `Surface` flag to various item IDs (0x1910-0x191F)

**Dependencies:**
- None

**Interactions:**
- Modifies global `TileData.ItemTable` directly
- Affects how items are rendered and behave in-game

**Configuration Options:**
- None (hardcoded fixes)

---

### Paragon

**Location:** `ServUO/Scripts/Services/Paragon.cs`

**Description:** Converts regular creatures into powerful "paragon" versions with enhanced stats, skills, and loot. Paragons have a special hue and can drop artifacts.

**Key Files:**
- `Paragon.cs` - Main implementation

**Enable/Disable:**
- Method: Requires `Core.AOS` expansion
- Default: Enabled if AOS expansion is active
- Configuration: Modify static properties for buffs, maps, artifacts

**Initialization:**
- Called via: Referenced by creature spawn/combat systems
- Timing: Runtime (when creatures spawn or are checked)

**Core Mechanics:**
- `CheckConvert()` - Determines if a creature should become a paragon based on fame and map
- `Convert()` - Applies paragon buffs: 5x hits, 1.05x STR, 1.20x INT/DEX, 1.20x skills, 1.20x speed, 1.40x fame/karma, +5 damage
- `UnConvert()` - Removes paragon status
- `CheckArtifactChance()` - Calculates artifact drop chance based on fame and player luck
- `GiveArtifactTo()` - Awards random artifact from predefined list
- Only works on specific maps (default: Ilshenar)
- Excludes champions, vendors, escorts, clones

**Dependencies:**
- Requires `Core.AOS` expansion
- Uses `BaseCreature` class

**Interactions:**
- Called by creature spawn/combat systems
- Modifies creature stats and properties
- Integrates with loot generation for artifacts

**Configuration Options:**
- `ChestChance` - Chance for paragon to carry paragon chest (default: 0.10)
- `ChocolateIngredientChance` - Chance for chocolate ingredient drop (default: 0.20)
- `Maps` - Array of maps where paragons can spawn (default: Ilshenar)
- `Artifacts` - Array of artifact types that can drop
- `Hue` - Paragon creature hue (default: 0x501)
- Buff multipliers (HitsBuff, StrBuff, IntBuff, DexBuff, SkillsBuff, SpeedBuff, FameBuff, KarmaBuff, DamageBuff)

---

### PlayerZombies (HalloweenHauntings)

**Location:** `ServUO/Scripts/Services/PlayerZombies.cs`

**Description:** Halloween event system that reanimates dead players as zombie skeletons in cemeteries. Only active during Halloween period.

**Key Files:**
- `PlayerZombies.cs` - Contains `HalloweenHauntings` class

**Enable/Disable:**
- Method: Date-based (checks `HolidaySettings.StartHalloween` and `FinishHalloween`)
- Default: Active during Halloween period
- Configuration: Modify Halloween dates in `HolidaySettings`

**Initialization:**
- Called via: `HalloweenHauntings.Initialize()` during `ScriptCompiler.Invoke("Initialize")`
- Timing: Server startup

**Core Mechanics:**
- Registers `EventSink.PlayerDeath` handler during Halloween period
- When player dies, adds them to death queue
- Timer processes queue every 2 minutes, spawning zombie skeletons in cemeteries
- Zombie skeletons are named after the dead player
- Maximum 200 zombies total, 200 players in death queue
- Clear timer removes all zombies every 30 minutes
- Zombies spawn in predefined cemetery rectangles across multiple maps

**Dependencies:**
- `HolidaySettings` for date checking
- `ZombieSkeleton` creature class

**Interactions:**
- Uses `EventSink.PlayerDeath` to detect player deaths
- Spawns `ZombieSkeleton` creatures
- Tracks reanimated players in dictionary

**Configuration Options:**
- `m_TotalZombieLimit` - Maximum zombies (default: 200)
- `m_DeathQueueLimit` - Maximum queued deaths (default: 200)
- `m_QueueDelaySeconds` - Delay between zombie spawns (default: 120)
- `m_QueueClearIntervalSeconds` - Time between zombie clears (default: 1800)

---

### PreventInaccess

**Location:** `ServUO/Scripts/Services/PreventInaccess.cs`

**Description:** Prevents staff members from being unable to access the server due to data overflows during login. Moves staff to safe locations if client crashes during login.

**Key Files:**
- `PreventInaccess.cs` - Main implementation

**Enable/Disable:**
- Method: Static property `Enabled` (hardcoded to `true`)
- Default: Enabled
- Configuration: Modify `Enabled` property in code

**Initialization:**
- Called via: `PreventInaccess.Initialize()` during `ScriptCompiler.Invoke("Initialize")`
- Timing: Server startup

**Core Mechanics:**
- Registers `EventSink.Login` handler
- On staff login, checks if NetState is null (indicating crash/disconnect)
- If disconnected, moves staff to random safe location (Jail or Green Acres)
- Stores original location in history dictionary
- On next successful login, informs player of previous move

**Dependencies:**
- None

**Interactions:**
- Uses `EventSink.Login` to detect logins
- Only affects non-player access levels (staff)
- Modifies `Mobile.Location` and `Mobile.Map`

**Configuration Options:**
- `Enabled` - Enable/disable system (default: true, hardcoded)
- Safe destinations defined in `m_Destinations` array

---

### PumpkinPatch

**Location:** `ServUO/Scripts/Services/PumpkinPatch.cs`

**Description:** Spawns Halloween pumpkins in designated fields during Halloween period. Maintains pumpkin population in predefined areas.

**Key Files:**
- `PumpkinPatch.cs` - Contains `PumpkinPatchSpawner` class

**Enable/Disable:**
- Method: Date-based (checks `HolidaySettings.StartHalloween` and `FinishHalloween`)
- Default: Active during Halloween period
- Configuration: Modify Halloween dates in `HolidaySettings`

**Initialization:**
- Called via: `PumpkinPatchSpawner.Initialize()` during `ScriptCompiler.Invoke("Initialize")`
- Timing: Server startup

**Core Mechanics:**
- Checks if current date is within Halloween period
- If active, starts timer that runs every 30 minutes
- Timer checks predefined pumpkin field rectangles
- Calculates spawn count based on field size (area / 20)
- Counts existing pumpkins in each field
- Spawns new pumpkins if count is below target
- Works on both Felucca and Trammel maps

**Dependencies:**
- `HolidaySettings` for date checking
- `HalloweenPumpkin` item class

**Interactions:**
- Uses `Map.GetItemsInBounds()` to count existing pumpkins
- Spawns `HalloweenPumpkin` items

**Configuration Options:**
- Pumpkin field rectangles defined in `m_PumpkinFields` array
- Timer interval: 30 minutes (0.50 hours)

---

### TestCenter

**Location:** `ServUO/Scripts/Services/TestCenter.cs`

**Description:** Provides test center functionality allowing players to set stats and skills via speech commands. Fills bank boxes with test items and equipment.

**Key Files:**
- `TestCenter.cs` - Main implementation

**Enable/Disable:**
- Method: `Config.Get("TestCenter.Enabled", false)`
- Default: `false` (disabled)
- Configuration: Add `TestCenter.Enabled=true` to `Config/` configuration files

**Initialization:**
- Called via: `TestCenter.Initialize()` during `ScriptCompiler.Invoke("Initialize")`
- Timing: Server startup

**Core Mechanics:**
- Registers `EventSink.Speech` handler when enabled
- Speech commands:
  - `set [stat/skill] [value]` - Sets stat (str/dex/int) or skill value
  - `help` - Shows help gump with commands and skill list
- `FillBankAOS()` / `FillBankbox()` - Fills player bank with test items:
  - Money (bank checks, gold, silver)
  - Potion kegs
  - Tools (all crafting tools)
  - Raw materials (ingots, leather, cloth, etc.)
  - Spellbooks and reagents
  - Artifacts and equipment
  - Ethereal mounts
  - Power scrolls (AOS only)

**Dependencies:**
- None

**Interactions:**
- Uses `EventSink.Speech` for command handling
- Modifies player stats and skills directly
- Adds items to player bank boxes

**Configuration Options:**
- `TestCenter.Enabled` - Enable/disable test center (default: false)

---

### TrickOrTreat

**Location:** `ServUO/Scripts/Services/TrickOrTreat.cs`

**Description:** Halloween event allowing players to say "trick or treat" to vendors for candy or tricks. Includes naughty twin mechanics.

**Key Files:**
- `TrickOrTreat.cs` - Main implementation

**Enable/Disable:**
- Method: Date-based (checks `HolidaySettings.StartHalloween` and `FinishHalloween`)
- Default: Active during Halloween period
- Configuration: Modify Halloween dates in `HolidaySettings`

**Initialization:**
- Called via: `TrickOrTreat.Initialize()` during `ScriptCompiler.Invoke("Initialize")`
- Timing: Server startup

**Core Mechanics:**
- Registers `EventSink.Speech` handler during Halloween
- When player says "trick or treat", opens targeting for vendor selection
- 90% chance: Vendor gives candy (regular treat or special GM item if begging skill 100+)
- 10% chance: Vendor plays trick:
  - Bleeding effect (spawns blood items)
  - Color change (random hue for 10 seconds)
  - Naughty twin spawn (creates evil copy of player)
- Naughty twin can steal candy or teleport player to random moongate
- Vendors have cooldown (5-10 minutes) between treats

**Dependencies:**
- `HolidaySettings` for date checking and item lists
- `BaseVendor` class
- `NaughtyTwin` creature class

**Interactions:**
- Uses `EventSink.Speech` for command detection
- Interacts with `BaseVendor.NextTrickOrTreat` property
- Spawns `NaughtyTwin` creatures
- Modifies player appearance and location

**Configuration Options:**
- None (date-based activation)

---

## Core Systems

### Chat System

**Location:** `ServUO/Scripts/Services/Chat/`

**Description:** Implements in-game chat system with channels, allowing players to communicate via chat interface similar to modern MMOs.

**Key Files:**
- `ChatSystem.cs` - Main system
- `ChatUser.cs` - User management
- `Channel.cs` - Channel management
- `ChatActionHandlers.cs` - Command handlers

**Enable/Disable:**
- Method: `Config.Get("Chat.Enabled", true)`
- Default: `true` (enabled)
- Configuration: Add `Chat.Enabled=true/false` to config files

**Initialization:**
- Called via: `ChatSystem.Initialize()` during `ScriptCompiler.Invoke("Initialize")`
- Timing: Server startup

**Core Mechanics:**
- Registers packet handlers (0xB5 for chat window, 0xB3 for chat actions)
- Manages chat users and channels
- Default channel: "Help"
- Players can create channels if `AllowCreateChannels` is true
- Handles chat commands (join, leave, create channel, etc.)

**Dependencies:**
- None

**Interactions:**
- Uses `PacketHandlers.Register()` for custom packets
- Manages `ChatUser` instances per player

**Configuration Options:**
- `Chat.Enabled` - Enable/disable chat system (default: true)
- `Chat.AllowCreateChannels` - Allow players to create channels (default: true)
- `DefaultChannel` - Default channel name (default: "Help")

---

### Bulk Order System

**Location:** `ServUO/Scripts/Services/BulkOrders/`

**Description:** Manages bulk order deeds (BODs) for crafting professions. Tracks player BOD history, points, and reward eligibility.

**Key Files:**
- `BulkOrderSystem.cs` - Main system
- `SmallBODs/` - Small bulk order definitions
- `LargeBODs/` - Large bulk order definitions
- `Rewards/` - Reward calculators

**Enable/Disable:**
- Method: `NewSystemEnabled = Core.TOL` (automatically enabled for Time of Legends expansion)
- Default: Enabled if TOL expansion active
- Configuration: N/A (expansion-based)

**Initialization:**
- Called via: Static constructor creates singleton instance
- Timing: Server startup

**Core Mechanics:**
- Maintains `BODPlayerData` dictionary tracking each player's BOD context
- Tracks cached deeds (max 2), last BOD time, and points per profession
- 6-hour delay between BOD requests
- Points system for reward redemption
- Supports multiple BOD types: Smith, Tailor, Alchemy, Inscription, Tinkering, Fletching, Carpentry, Cooking

**Dependencies:**
- Requires `Core.TOL` expansion for new system
- Integrates with crafting systems

**Interactions:**
- Used by crafting vendors to issue BODs
- Tracks player progress for rewards
- Serializes/deserializes with world save/load

**Configuration Options:**
- `MaxCachedDeeds` - Maximum cached deeds (default: 2)
- `Delay` - Hours between BOD requests (default: 6)

---

### Champion System

**Location:** `ServUO/Scripts/Services/ChampionSystem/`

**Description:** Manages champion spawns - special boss encounters that require multiple players to defeat. Spawns are configured via XML and rotate periodically.

**Key Files:**
- `ChampionSystem.cs` - Main system
- `ChampionSpawn.cs` - Individual spawn logic
- `ChampionSpawnType.cs` - Spawn type definitions
- `ChampionSpawnController/` - Spawn management

**Enable/Disable:**
- Method: `Config.Get("Champions.Enabled", true)`
- Default: `true` (enabled)
- Configuration: Add `Champions.Enabled=true/false` to config files

**Initialization:**
- Called via: `ChampionSystem.Initialize()` during `ScriptCompiler.Invoke("Initialize")`
- Timing: Server startup

**Core Mechanics:**
- Loads spawn configuration from `Config/ChampionSpawns.xml`
- Spawns rotate on configurable schedule (default: daily)
- Spawns progress through levels (0-15) based on creature kills
- Higher levels spawn stronger creatures and require more kills
- When champion is defeated, awards power scrolls, stat scrolls, and gold
- Supports multiple spawn types (Arachnid, Cold Blood, Forest Lord, etc.)
- Harrower spawn type has special mechanics

**Dependencies:**
- XML configuration file: `Config/ChampionSpawns.xml`
- Persistence system for save/load

**Interactions:**
- Uses `EventSink.WorldSave` and `EventSink.WorldLoad` for persistence
- Spawns creatures and manages combat
- Awards items to players on champion death

**Configuration Options:**
- `Champions.Enabled` - Enable/disable system (default: true)
- `Champions.RotateDelay` - Time between spawn rotations (default: 1 day)
- `Champions.GoldPiles` - Gold shower piles (default: 50)
- `Champions.GoldMin/Max` - Gold amounts (default: 4000-5500)
- `Champions.PowerScrolls` - Power scroll count (default: 6)
- `Champions.StatScrolls` - Stat scroll count (default: 16)
- `Champions.ScrollChance` - Chance for scroll drops (default: 0.1%)
- `Champions.TranscendenceChance` - Transcendence chance (default: 50%)
- Rank kill requirements configurable

---

## Gameplay Systems

### Astronomy System

**Location:** `ServUO/Scripts/Services/Astronomy/`

**Description:** Implements astronomy system where players can discover constellations using telescopes. Tracks discovered constellations and interstellar objects.

**Key Files:**
- `AstronomySystem.cs` - Main system
- `Telescope.cs` - Telescope item
- `ConstellationInfo.cs` - Constellation data
- `StarChart.cs` - Star chart item

**Enable/Disable:**
- Method: `Enabled = Core.EJ` (automatically enabled for Endless Journey expansion)
- Default: Enabled if EJ expansion active
- Configuration: N/A (expansion-based)

**Initialization:**
- Called via: `AstronomySystem.Configure()` and `Initialize()` during server startup
- Timing: `Configure()` during `ScriptCompiler.Invoke("Configure")`, `Initialize()` during `ScriptCompiler.Invoke("Initialize")`

**Core Mechanics:**
- Creates up to 1000 constellations with random coordinates
- Constellations organized by time coordinates (FiveToEight, NineToEleven, Midnight, OneToFour, Day)
- Tracks discovered constellations per player
- Interstellar objects include comets, Felucca/Trammel moons, galaxies, planets
- Uses Right Ascension (RA) and Declination (DEC) coordinates
- Saves/loads constellation data and discoveries

**Dependencies:**
- Requires `Core.EJ` expansion
- Persistence system

**Interactions:**
- Uses `EventSink.WorldSave` and `EventSink.WorldLoad` for persistence
- Integrates with telescope items for player interaction

**Configuration Options:**
- `MaxConstellations` - Maximum constellations (default: 1000)
- `MaxRA` - Maximum right ascension (default: 24)
- `MaxDEC` - Maximum declination (default: 90)

---

### City Loyalty System

**Location:** `ServUO/Scripts/Services/City Loyalty System/`

**Description:** Manages city loyalty, elections, trade deals, and city-specific content for major cities in Britannia.

**Key Files:**
- `CityLoyaltySystem.cs` - Main system
- `CityDefinition.cs` - City definitions
- `Election.cs` - Election mechanics
- `Trading/` - Trade deal system

**Enable/Disable:**
- Method: `Config.Get("CityLoyalty.Enabled", true)`
- Default: `true` (enabled)
- Configuration: Add `CityLoyalty.Enabled=true/false` to config files

**Initialization:**
- Called via: System initialization during server startup
- Timing: Server startup

**Core Mechanics:**
- Tracks loyalty ratings for players in each city (Disfavored to Venerated)
- City elections for governor positions
- Trade deals with various guilds
- City-specific titles and rewards
- Treasury system for city funds
- Banner system for city defense
- Supports cities: Moonglow, Britain, Jhelom, Yew, Minoc, Trinsic, SkaraBrae, NewMagincia, Vesper

**Dependencies:**
- Points system integration
- Siege system (determines which map to use)

**Interactions:**
- Integrates with `PointsSystem` for loyalty tracking
- Uses `Siege.SiegeShard` to determine map (Felucca vs Trammel)
- Manages city NPCs (Guard Captain, Herald, Trade Minister)

**Configuration Options:**
- `CityLoyalty.Enabled` - Enable/disable system (default: true)
- `CityLoyalty.JoinWait` - Days before joining city (default: 7)
- `CityLoyalty.BannerCost` - Banner deployment cost (default: 250000)
- `CityLoyalty.BannerCooldown` - Banner cooldown hours (default: 24)
- `CityLoyalty.TradeDealPeriod` - Trade deal period days (default: 7)
- `CityLoyalty.TradeDealCost` - Trade deal cost (default: 2000000)
- `CityLoyalty.MaxBallotBoxes` - Maximum ballot boxes (default: 10)
- `CityLoyalty.AnnouncementPeriod` - Announcement period hours (default: 48)

---

## Feature Systems

### LLM (Large Language Model) Service

**Location:** `ServUO/Scripts/Services/LLM/`

**Description:** Advanced AI-powered NPC system that uses Large Language Models (OpenAI or local Ollama) to generate conversational responses. NPCs can remember players, have personalities, and provide quests.

**Key Files:**
- `Core/LLMInitializer.cs` - System initialization
- `Core/LLMNpc.cs` - Main conversational NPC
- `Core/LLMQuester.cs` - Quest-giving NPC
- `Services/LLMService.cs` - OpenAI integration
- `Services/LocalLLMService.cs` - Ollama integration
- `Services/LLMMemoryService.cs` - Memory system
- `Data/VectorLoreSystem.cs` - Knowledge base (RAG)
- `Data/NPCPersonalities.cs` - Personality system

**Enable/Disable:**
- Method: Multiple configuration points
  - `LLMConversationPlugin.Enabled` - Global enable/disable (default: true)
  - `LLMMemoryConfig.Enabled` - Memory system enable (default: true)
  - API configuration in `Config/LLM.cfg` or `Config/LocalLLM.cfg`
- Default: Enabled but requires API configuration
- Configuration: 
  - OpenAI: Set API key in `Config/LLM.cfg`
  - Ollama: Configure in `Config/LocalLLM.cfg` (default: http://localhost:11434)

**Initialization:**
- Called via: `LLMInitializer.Initialize()` during `ScriptCompiler.Invoke("Initialize")`
- Timing: Server startup, multi-phase initialization

**Core Mechanics:**
- **Phase 1**: Initializes LLM providers (OpenAI and/or Ollama)
- **Phase 2**: Loads knowledge base (RAG system) with 412+ lore entries
- **Phase 2.5**: Initializes NPC location database
- **Phase 3**: Sets up memory system (SQLite database for persistent memories)
- **Phase 4**: Starts background services (conversation cleanup, cache cleanup)
- **Phase 5**: Registers commands for spawning and managing LLM NPCs
- NPCs can have 54+ personality types and 6 speech patterns
- Supports vendor integration with natural language buying/selling
- Response caching with semantic matching (24-hour TTL)
- Request throttling (max 5 pending requests per NPC)

**Dependencies:**
- SQLite for memory persistence (optional, has in-memory fallback)
- OpenAI API key OR local Ollama installation
- Vector embeddings for knowledge base (uses simple keyword matching as fallback)

**Interactions:**
- Integrates with `BaseVendor` for vendor functionality
- Uses `EventSink.Speech` for conversation detection
- Stores memories in SQLite database
- Can spawn quest-giving NPCs with traditional quest mechanics

**Configuration Options:**
- `LLMConversationPlugin.Enabled` - Global enable/disable
- `LLMMemoryConfig.Enabled` - Memory system enable
- API endpoint URLs and keys
- Model selection (OpenAI: gpt-4o-mini, Ollama: phi3:mini)
- Max tokens, temperature, cache settings
- Memory limits, cleanup intervals

**Note:** Extensive documentation available in `LLM/Documentation/` folder.

---

### Vendor Searching

**Location:** `ServUO/Scripts/Services/Vendor Searching/`

**Description:** Allows players to search for items across all player vendors and auctions. Provides gump interface for filtering and sorting vendor items.

**Key Files:**
- `VendorSearch.cs` - Main search logic
- `VendorSearchGump.cs` - User interface

**Enable/Disable:**
- Method: Always enabled (no configuration flag)
- Default: Enabled
- Configuration: N/A

**Initialization:**
- Called via: `VendorSearch.Initialize()` and `VendorSearchGump.Initialize()` during `ScriptCompiler.Invoke("Initialize")`
- Timing: Server startup

**Core Mechanics:**
- Searches all `PlayerVendor` instances that have `VendorSearch` enabled
- Supports searching by item name, type, price range, attributes, and more
- Can search auctions if auction system is active
- Filters by map (can exclude Felucca)
- Sorts results by price (low to high or high to low)
- Saves/loads search criteria and favorites

**Dependencies:**
- `PlayerVendor` system
- Optional: Auction system integration

**Interactions:**
- Reads from `PlayerVendor.PlayerVendors` collection
- Integrates with auction system if available
- Uses Ultima.StringList for item name lookups

**Configuration Options:**
- None (always active)

---

### Points System

**Location:** `ServUO/Scripts/Services/PointsSystems/`

**Description:** Base system for tracking points/loyalty across multiple game systems. Provides unified framework for points-based rewards and progression.

**Key Files:**
- `PointsSystem.cs` - Base system
- `VirtueArtifactsSystem.cs` - Virtue artifacts integration

**Enable/Disable:**
- Method: Individual systems register themselves
- Default: Enabled
- Configuration: Each points system can be enabled/disabled independently

**Initialization:**
- Called via: Systems register themselves via constructor
- Timing: Server startup

**Core Mechanics:**
- Abstract base class for all points systems
- Tracks points per player in `PlayerTable`
- Supports multiple points types (City Loyalty, VvV, CleanUpBritannia, etc.)
- Handles point awards, deductions, and conversions
- Saves/loads all points data to `Saves/PointsSystem/Persistence.bin`
- Provides gump interface for viewing points

**Dependencies:**
- Persistence system for save/load

**Interactions:**
- Used by: City Loyalty, ViceVsVirtue, CleanUpBritannia, and other systems
- Integrates with quest systems for point awards
- Used by reward systems for point redemption

**Configuration Options:**
- Each points system has its own configuration
- `MaxPoints` - Maximum points per system (varies by system)
- `AutoAdd` - Automatically add players to system (varies)

---

### Virtues System

**Location:** `ServUO/Scripts/Services/Virtues/`

**Description:** Implements the eight virtues system (Compassion, Honesty, Honor, Humility, Justice, Sacrifice, Spirituality, Valor). Tracks virtue gains and provides virtue gump interface.

**Key Files:**
- `VirtueGump.cs` - Virtue interface
- `Compassion.cs`, `Honesty.cs`, `Honor.cs`, `Humility.cs`, `Justice.cs`, `Sacrifice.cs`, `Spirituality.cs`, `Valor.cs` - Individual virtue implementations

**Enable/Disable:**
- Method: Individual virtues can be enabled via config
- Default: Most enabled (varies by virtue)
- Configuration: `[VirtueName].Enabled` config entries (e.g., `Honesty.Enabled=true`)

**Initialization:**
- Called via: Each virtue's `Initialize()` method during `ScriptCompiler.Invoke("Initialize")`
- Timing: Server startup

**Core Mechanics:**
- Tracks virtue values per player (0-10000)
- Virtues can atrophy over time if not used
- Provides virtue gump (Alt+V) for viewing and using virtues
- Each virtue has unique activation mechanics
- Some virtues provide special abilities when used

**Dependencies:**
- `VirtueHelper` for common virtue operations
- `PlayerMobile` for virtue storage

**Interactions:**
- Integrates with combat systems for Honor/Justice
- Used by quest systems for virtue checks
- Provides abilities through virtue gump

**Configuration Options:**
- `[VirtueName].Enabled` - Enable/disable individual virtues
- `[VirtueName].MaxGeneration` - Maximum virtue generation (varies)
- `[VirtueName].TrammelGeneration` - Allow virtue gain on Trammel (varies)

---

### ViceVsVirtue System

**Location:** `ServUO/Scripts/Services/ViceVsVirtue/`

**Description:** PvP faction system where players join Virtue or Vice teams and battle for control of cities. Guild-based system with battles and rewards.

**Key Files:**
- `ViceVsVirtueSystem.cs` - Main system
- `VvVBattle.cs` - Battle mechanics
- `VvVRewards.cs` - Reward system

**Enable/Disable:**
- Method: `Config.Get("VvV.Enabled", true)`
- Default: `true` (enabled)
- Configuration: Add `VvV.Enabled=true/false` to config files

**Initialization:**
- Called via: System initialization during server startup
- Timing: Server startup

**Core Mechanics:**
- Players join Virtue or Vice teams
- Guild-based participation
- Battles for control of cities (Britain, Jhelom, Minoc, Moonglow, Ocllo, SkaraBrae, Trinsic, Yew)
- Points system for participation and victories
- Special VvV items and rewards
- Enhanced rules option for additional mechanics

**Dependencies:**
- Points system integration
- Guild system
- City system

**Interactions:**
- Integrates with `PointsSystem` for tracking
- Uses guild system for team organization
- Interacts with city systems for battles
- Awards items and titles

**Configuration Options:**
- `VvV.Enabled` - Enable/disable system (default: true)
- `VvV.StartSilver` - Starting silver amount (default: 2000)
- `VvV.EnhancedRules` - Enable enhanced rules (default: false)

---

### Veteran Rewards System

**Location:** `ServUO/Scripts/Services/VeteranRewards/`

**Description:** Rewards players based on account age. Provides veteran reward tokens and access to special items based on account creation date.

**Key Files:**
- `RewardSystem.cs` - Main system
- `RewardCategory.cs` - Reward categories
- `RewardEntry.cs` - Individual rewards

**Enable/Disable:**
- Method: `Config.Get("VetRewards.Enabled", true)`
- Default: `true` (enabled)
- Configuration: Add `VetRewards.Enabled=true/false` to config files

**Initialization:**
- Called via: `RewardSystem.Initialize()` during `ScriptCompiler.Invoke("Initialize")`
- Timing: Server startup

**Core Mechanics:**
- Calculates reward level based on account age
- Reward interval: 30 days (configurable)
- Starting level can be set (default: 0)
- Provides skill cap bonuses for veteran accounts
- Reward categories: Housing, Character Customization, Ethereal Mounts, etc.
- Each reward requires specific expansion level

**Dependencies:**
- Account system for age calculation
- Expansion system for reward availability

**Interactions:**
- Uses `Account.Created` date for age calculation
- Integrates with skill cap system for bonuses
- Provides gump interface for reward selection

**Configuration Options:**
- `VetRewards.Enabled` - Enable/disable system (default: true)
- `VetRewards.SkillCapRewards` - Enable skill cap bonuses (default: true)
- `VetRewards.SkillCapBonus` - Skill cap bonus amount (default: 200)
- `VetRewards.SkillCapBonusLevels` - Number of bonus levels (default: 4)
- `VetRewards.RewardInterval` - Days between reward levels (default: 30)
- `VetRewards.StartingLevel` - Starting reward level (default: 0)

---

### Quest System

**Location:** `ServUO/Scripts/Services/Quests/`

**Description:** Base framework for quest implementation. Manages quest objectives, conversations, and completion tracking.

**Key Files:**
- `QuestSystem.cs` - Base quest system
- Individual quest implementations in subdirectories

**Enable/Disable:**
- Method: Always enabled (base framework)
- Default: Enabled
- Configuration: Individual quests can be enabled/disabled

**Initialization:**
- Called via: `QuestSystem.Configure()` during `ScriptCompiler.Invoke("Configure")`
- Timing: Server startup

**Core Mechanics:**
- Abstract base class for all quests
- Tracks quest objectives and completion status
- Manages quest conversations and dialogue
- Handles quest restart delays
- Supports tutorial quests
- Integrates with kill tracking for quest objectives

**Dependencies:**
- None (base framework)

**Interactions:**
- Used by all quest implementations
- Integrates with `EventSink.OnKilledBy` for kill tracking
- Used by NPCs to offer and track quests

**Configuration Options:**
- Individual quests have their own configuration
- `RestartDelay` - Time before quest can be restarted (varies by quest)

---

### Plant System

**Location:** `ServUO/Scripts/Services/Plants/`

**Description:** Manages plant growth, health, and resource generation. Tracks water, disease, infestation, and other plant states.

**Key Files:**
- `PlantSystem.cs` - Main plant logic
- `PlantItem.cs` - Plant item implementation

**Enable/Disable:**
- Method: Always enabled (no configuration flag)
- Default: Enabled
- Configuration: N/A

**Initialization:**
- Called via: System initialization during server startup
- Timing: Server startup

**Core Mechanics:**
- Tracks plant health (Dying, Wilted, Healthy, Vibrant)
- Manages water levels (0-4)
- Tracks diseases, infestations, fungus, poison
- Growth check every 23 hours
- Generates seeds and resources based on plant type
- Supports fertile dirt for improved growth
- Multiple plant types and hues

**Dependencies:**
- None

**Interactions:**
- Used by plant items in-game
- Integrates with gardening tools
- Provides resources for crafting

**Configuration Options:**
- `CheckDelay` - Time between growth checks (default: 23 hours)

---

### Seasonal Events System

**Location:** `ServUO/Scripts/Services/Seasonal Events/`

**Description:** Manages seasonal events that can be active, inactive, or seasonal (date-based). Controls availability of special content.

**Key Files:**
- `SeasonalEventSystem.cs` - Main system
- Individual event implementations in subdirectories

**Enable/Disable:**
- Method: Per-event configuration via gump or code
- Default: Varies by event
- Configuration: Use `[SeasonSystemGump` command or modify `LoadEntries()`

**Initialization:**
- Called via: `SeasonalEventSystem.Configure()` during `ScriptCompiler.Invoke("Configure")`
- Timing: Server startup

**Core Mechanics:**
- Manages event status: Inactive, Active, or Seasonal
- Seasonal events activate during specific date ranges (month/day)
- Events include: Treasures of Tokuno, Virtue Artifacts, Treasures of Kotl City, Sorcerer's Dungeon, Treasures of Doom, Treasures of Khaldun, Krampus Encounter, Rising Tide, Fellowship
- Saves/loads event status
- Provides admin gump for configuration

**Dependencies:**
- Persistence system for save/load
- Individual event systems

**Interactions:**
- Used by event-specific systems to check if active
- Integrates with quest and reward systems
- Controls spawn and reward availability

**Configuration Options:**
- Event status (Inactive/Active/Seasonal)
- Seasonal date ranges (month/day) for seasonal events
- Per-event configuration via admin gump

---

## Additional Services

### 18th Anniversary / 22nd Anniversary

**Location:** `ServUO/Scripts/Services/18th Anniversary/`, `22nd Anniversary/`

**Description:** Anniversary gift systems that spawn gift tokens and special items for server anniversaries.

**Key Files:**
- `Giver.cs` - Gift spawner
- Anniversary-specific items

**Enable/Disable:**
- Method: Date-based or manual spawning
- Default: Manual activation
- Configuration: Spawn via commands or date checks

**Initialization:**
- Called via: `Giver.Initialize()` during server startup
- Timing: Server startup

**Core Mechanics:**
- Spawns anniversary gift tokens at specific locations
- Players can redeem tokens for special items
- Includes anniversary-specific decorative items

**Dependencies:**
- None

**Interactions:**
- Spawns items in world
- Players interact with gift tokens

---

### Armor Refinement

**Location:** `ServUO/Scripts/Services/Armor Refinement/`

**Description:** Allows players to refine armor with special materials to add properties and increase durability.

**Key Files:**
- `ArmorRefiner.cs` - Refinement logic
- `Gumps.cs` - User interface
- `Items.cs` - Refinement materials

**Enable/Disable:**
- Method: Always enabled (no configuration flag)
- Default: Enabled
- Configuration: N/A

**Initialization:**
- Called via: System initialization during server startup
- Timing: Server startup

**Core Mechanics:**
- Players use refinement materials on armor
- Adds special properties to armor
- Increases durability and effectiveness
- Requires specific materials for each refinement type

**Dependencies:**
- None

**Interactions:**
- Used by players on armor items
- Integrates with crafting systems

---

### Basket Weaving

**Location:** `ServUO/Scripts/Services/BasketWeaving/`

**Description:** Crafting system for creating baskets and related items. Includes natural dyes and plant pigments.

**Key Files:**
- `BasketWeavingBook.cs` - Crafting book
- `Clippers.cs` - Harvesting tool
- `NaturalDye.cs`, `PlantPigment.cs` - Dye materials
- `Baskets/` - Basket item definitions

**Enable/Disable:**
- Method: Always enabled (no configuration flag)
- Default: Enabled
- Configuration: N/A

**Initialization:**
- Called via: System initialization during server startup
- Timing: Server startup

**Core Mechanics:**
- Crafting skill for creating baskets
- Uses plant materials and dyes
- Creates decorative and functional baskets
- Includes color system for basket customization

**Dependencies:**
- Craft system integration

**Interactions:**
- Integrates with crafting system
- Uses plant materials from plant system

---

### Craft System

**Location:** `ServUO/Scripts/Services/Craft/`

**Description:** Core crafting system definitions for all crafting professions. Defines recipes, materials, and crafting mechanics.

**Key Files:**
- `Core/` - Core crafting framework
- `DefAlchemy.cs`, `DefBlacksmithy.cs`, `DefBowFletching.cs`, etc. - Profession definitions

**Enable/Disable:**
- Method: Always enabled (core system)
- Default: Enabled
- Configuration: N/A

**Initialization:**
- Called via: System initialization during server startup
- Timing: Server startup

**Core Mechanics:**
- Defines all crafting recipes and materials
- Manages skill requirements and success chances
- Handles resource consumption
- Supports exceptional and legendary items
- Integrates with bulk order system

**Dependencies:**
- None (core system)

**Interactions:**
- Used by all crafting professions
- Integrates with bulk order system
- Provides materials for other systems

---

### XmlSpawner

**Location:** `ServUO/Scripts/Services/XmlSpawner/`

**Description:** Advanced spawner system using XML configuration. Allows spawning of creatures, items, and complex scenarios with triggers, conditions, and quest integration.

**Key Files:**
- `XmlSpawner Core/BaseXmlSpawner.cs` - Main spawner class
- `XmlSpawner Core/XmlSpawner2.cs` - Enhanced spawner
- `XmlSpawner Core/XmlAttach/` - Attachment system

**Enable/Disable:**
- Method: `Config.Get("XmlSpawner2.Points", false)` and related flags
- Default: Enabled (spawner functionality)
- Configuration: Multiple config options for features

**Initialization:**
- Called via: `BaseXmlSpawner.Initialize()` and related systems during `ScriptCompiler.Invoke("Initialize")`
- Timing: Server startup

**Core Mechanics:**
- XML-based spawner configuration
- Supports spawning creatures, items, and complex scenarios
- Trigger system for conditional spawning
- Quest integration
- Attachment system for adding properties to items/mobiles
- Points system integration (optional)
- Faction integration (optional)
- Socket system integration (optional)

**Dependencies:**
- Optional: Points system, Factions, Sockets

**Interactions:**
- Can integrate with quest systems
- Supports points system for rewards
- Can attach properties to spawned items/mobiles

**Configuration Options:**
- `XmlSpawner2.Points` - Enable points integration (default: false)
- `XmlSpawner2.Factions` - Enable faction integration (default: false)
- `XmlSpawner2.Sockets` - Enable socket integration (default: false)

---

### Ultima Store

**Location:** `ServUO/Scripts/Services/UltimaStore/`

**Description:** Implements the Ultima Online store system where players can purchase items using Sovereigns (premium currency).

**Key Files:**
- `UltimaStore.cs` - Main store system
- `SystemConfig.cs` - Configuration

**Enable/Disable:**
- Method: `Config.Get("Store.Enabled", true)`
- Default: `true` (enabled)
- Configuration: Add `Store.Enabled=true/false` to config files

**Initialization:**
- Called via: `UltimaStore.Initialize()` during `ScriptCompiler.Invoke("Initialize")`
- Timing: Server startup

**Core Mechanics:**
- Provides in-game store interface
- Uses Sovereigns as currency
- Players can browse and purchase items
- Cart system for multiple items
- Cost multiplier for pricing adjustments
- Links to official UO store website

**Dependencies:**
- None

**Interactions:**
- Provides gump interface for players
- Manages currency (Sovereigns)
- Adds purchased items to player inventory

**Configuration Options:**
- `Store.Enabled` - Enable/disable store (default: true)
- `Store.Website` - Store website URL (default: https://uo.com/ultima-store/)
- `Store.CurrencyName` - Currency name (default: "Sovereigns")
- `Store.CurrencyDisplay` - Show currency (default: true)
- `Store.CostMultiplier` - Price multiplier (default: 1.0)
- `Store.CartCapacity` - Max items in cart (default: 10)

---

## Summary

This documentation covers the major services in the ServUO Services folder. Each service is designed to be modular and can typically be enabled or disabled independently. Most services use configuration files for settings, while some are date-based (seasonal events) or expansion-based (requiring specific expansion flags).

### Common Patterns

1. **Initialization**: Most services initialize via `ScriptCompiler.Invoke("Initialize")` or `ScriptCompiler.Invoke("Configure")` during server startup.

2. **Configuration**: Services typically use `Config.Get("ServiceName.Setting", defaultValue)` for configuration, with settings stored in `Config/` directory files.

3. **Enable/Disable**: Many services have an `Enabled` flag that can be set via configuration or code.

4. **Persistence**: Services that need to save data use `EventSink.WorldSave` and `EventSink.WorldLoad` events, saving to `Saves/` directory.

5. **Event Integration**: Services commonly use `EventSink` events (Login, Speech, PlayerDeath, etc.) for integration with game systems.

### Service Categories

- **Core Systems**: Essential game systems (Chat, BulkOrders, Craft)
- **Gameplay Systems**: Player progression and mechanics (Virtues, Paragon, Points)
- **Content Systems**: Quests, events, and world content (Quests, Seasonal Events, Dungeons)
- **Feature Systems**: Advanced features (LLM, Vendor Searching, Ultima Store)
- **Utility Systems**: Helper systems (TestCenter, ItemFixes, PreventInaccess)

For services not fully documented here, refer to the individual service directories for implementation details and configuration options. Each service directory typically contains the main implementation files and may include documentation or configuration examples.

