# Vystia Project - TODO List

**Last Updated**: 2025-12-05 (Moved to Vystia/docs/)
**Status**: Basic map generation working, world is blank

---

## ✅ COMPLETED

### Map Generation Pipeline
- [x] Stage 0: Normalize & Expand to 7168×4096
- [x] Stage 1: Voronoi region generation (21 named regions)
- [x] Stage 4.5: Map regions to UO terrain IDs (1-80)
- [x] Stage 5: Write UO-compatible BMPs (Terrain.bmp + Altitude.bmp)
- [x] Use exact UO terrain palette colors from Terrain.xml
- [x] Use Base altitude values (24, 88, 152, 216) from Terrain.xml
- [x] Pixel-perfect matching between terrain and altitude
- [x] UOMapMake conversion to map0.mul, statics0.mul, staidx0.mul

### ServUO Configuration
- [x] MapDefinitions.cs updated to 7168×4096 on Map 0
- [x] DataPath.cfg pointing to C:\UORenaissance\
- [x] Map files loading correctly (map0.mul, statics0.mul, staidx0.mul)
- [x] Scripts recompiling with updated commands
- [x] Region system loading without errors

### Commands
- [x] `[GoVystia` - Teleport to center of map (3584, 2048)
- [x] `[GoVystia <x> <y> <z>` - Teleport to specific coordinates
- [x] `[FillGrass <size>` - Fill area with grass (small/medium/large/huge)
- [x] `[GenIronheart` - Generate Ironheart Capital city
- [x] `[ClearIronheart` - Clear Ironheart Capital

---

## 🚧 IN PROGRESS

### World Content
- [ ] **Statics/Decorations** - Map has terrain but no trees, rocks, buildings
  - Current: Only raw terrain (grass, water, mountains)
  - Need: Trees, rocks, bushes, cliffs, natural features
  - Tool: Need to generate statics0.mul content or use UOFiddler

---

## 📋 TODO - HIGH PRIORITY

### 1. World Population - Statics & Decorations

#### Natural Features
- [ ] **Trees**
  - [ ] Forest regions need tree coverage
  - [ ] Different tree types per biome (oak, pine, palm, etc.)
  - [ ] Tree density variation (sparse to dense forests)

- [ ] **Rocks & Mountains**
  - [ ] Mountain regions need rock formations
  - [ ] Cave entrances
  - [ ] Cliffs and rocky terrain

- [ ] **Water Features**
  - [ ] Ocean decorations (kelp, rocks, shipwrecks?)
  - [ ] Lake shores with reeds/plants
  - [ ] Rivers with bridges

- [ ] **Ground Decorations**
  - [ ] Bushes and small plants
  - [ ] Flowers in grasslands
  - [ ] Dead trees in wastelands
  - [ ] Cacti in deserts (if applicable)

#### Tools & Methods
- [ ] Research UO static item IDs (trees, rocks, etc.)
- [ ] Write Python script to generate statics0.mul
- [ ] OR use UOFiddler to manually place decorations
- [ ] OR use CentrED+ for world editing
- [ ] Test static loading in ServUO

### 2. Towns & Cities

#### Ironheart Capital (Existing Command)
- [x] `[GenIronheart` command exists
- [ ] Test and verify Ironheart generation works
- [ ] Choose coordinates for Ironheart placement
- [ ] Document Ironheart location and features

#### Additional Settlements
- [ ] Plan 5-10 major cities based on Voronoi regions
- [ ] Plan 10-20 smaller villages/towns
- [ ] Create town generation scripts/commands
- [ ] Place buildings, NPCs, shops, banks

#### Infrastructure
- [ ] Roads connecting major cities
- [ ] Bridges over rivers
- [ ] Docks at coastal cities
- [ ] Guard towers and outposts

### 3. Regions System

#### Town Regions
- [ ] Create GuardedRegion definitions for major cities
- [ ] Set boundaries for each town
- [ ] Configure guard spawn points
- [ ] Set town music themes

#### Dungeon Regions
- [ ] Plan dungeon locations (caves, ruins, etc.)
- [ ] Create DungeonRegion definitions
- [ ] Disable recall/gate in dungeons
- [ ] Set dungeon music

#### Special Regions
- [ ] PvP zones (if desired)
- [ ] Safe zones (newbie areas)
- [ ] Resource gathering areas
- [ ] Quest zones

#### Configuration
- [ ] Create new C:\DevEnv\GIT\UO\ServUO\Data\Regions_Vystia.xml
- [ ] Replace old Felucca regions with Vystia regions
- [ ] Test region boundaries and transitions
- [ ] Verify guard system works

### 4. Spawning System

#### Creatures
- [ ] Define spawn zones for each creature type
- [ ] Plan creature difficulty by region
- [ ] Create spawn group definitions
- [ ] Balance spawn rates and quantities

#### NPCs
- [ ] Town guards in GuardedRegions
- [ ] Shopkeepers and vendors
- [ ] Quest NPCs
- [ ] Random wandering NPCs

#### Resources
- [ ] Ore spawns in mountain regions
- [ ] Lumber spawns in forest regions
- [ ] Fish spawns in water
- [ ] Reagent plants

### 5. Music System

- [ ] Assign music tracks to different regions
- [ ] Town themes (peaceful, bustling)
- [ ] Wilderness themes (forest, mountains, ocean)
- [ ] Dungeon themes (dark, mysterious)
- [ ] Combat music triggers

---

## 📋 TODO - MEDIUM PRIORITY

### 6. World Lore & Naming

- [ ] Name the 21 Voronoi regions (some already exist: Frosthold, Winterguard, etc.)
- [ ] Write lore for each region
- [ ] Create world history/backstory
- [ ] Design faction system (if applicable)

### 7. Transportation

- [ ] Moongate locations and network
- [ ] Healer locations
- [ ] Stable locations for mounts
- [ ] Ship routes (if naval system desired)

### 8. Quests & Content

- [ ] Design starter quests
- [ ] Create quest chains for each major city
- [ ] Design world bosses and raid content
- [ ] Create treasure hunting system

### 9. Economy

- [ ] Set resource distribution
- [ ] Balance vendor pricing
- [ ] Configure crafting resource availability
- [ ] Design trade routes

### 10. Player Housing

- [ ] Designate housing zones
- [ ] Configure plot sizes and pricing
- [ ] Set housing region boundaries
- [ ] Test house placement system

---

## 📋 TODO - LOW PRIORITY

### 11. Polish & Details

- [ ] Custom loading screen
- [ ] World map artwork
- [ ] Custom UI elements
- [ ] Server branding/theme

### 12. Testing & Balance

- [ ] Playtest navigation and travel times
- [ ] Balance resource spawn rates
- [ ] Test combat difficulty by region
- [ ] Verify all commands work
- [ ] Performance testing with players

### 13. Documentation

- [ ] Player guide (getting started)
- [ ] World map with city locations
- [ ] Quest guides
- [ ] Admin/GM documentation

---

## 🛠️ TECHNICAL DEBT

### Pipeline Improvements
- [ ] Automate full pipeline (Python → UOMapMake → Copy to UORenaissance)
- [ ] Add validation checks between stages
- [ ] Create backup system for map versions
- [ ] Version control for map iterations

### ServUO Improvements
- [ ] Custom startup script for Vystia
- [ ] Automated region generation from Voronoi data
- [ ] Bridge Python pipeline data with C# scripts
- [ ] Performance profiling for large world

---

## 📊 CURRENT STATE SUMMARY

**What Works:**
- Procedural terrain generation (7168×4096)
- UO-compatible map format (map0.mul)
- Server loads and runs
- Players can teleport around the world
- Basic terrain types (grass, water, mountains, etc.)

**What's Missing:**
- **Statics** - No trees, rocks, or decorations (world looks empty)
- **Towns** - No cities or buildings
- **Regions** - No guard zones, music, or special areas
- **Spawns** - No creatures, NPCs, or resources
- **Content** - No quests, dungeons, or points of interest

**Next Immediate Steps:**
1. Research UO static item system
2. Create statics generation script
3. Add trees and rocks to make world feel alive
4. Test Ironheart city generation
5. Create first town region with guards and music

---

## 📝 NOTES

### Static Item IDs (Research Needed)
- Trees: ItemID range ~0x0C93 - 0x0CE2
- Rocks: ItemID range ~0x1773 - 0x17DA
- Bushes: ItemID range varies
- Need to reference UO item database

### Tools Available
- **UOFiddler** - Manual world editing
- **CentrED+** - Advanced map editor
- **UOMapMake** - BMP to MUL conversion (already using)
- **Python Scripts** - Custom generation tools

### Region Generation Strategy
Could extract Voronoi region data from pipeline and auto-generate region XMLs:
- Each Voronoi region → TownRegion or WildernessRegion
- Region boundaries from Voronoi polygons
- Auto-assign music based on terrain type
- Generate spawn zones automatically

---

**End of TODO List**
