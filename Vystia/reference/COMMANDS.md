# PLANNED VYSTIA COMMANDS (NOT IMPLEMENTED)

⚠️ **WARNING: THIS IS DESIGN DOCUMENTATION - THESE COMMANDS DO NOT EXIST**

This document describes **planned/aspirational** commands for a future Vystia world system. Currently, none of these commands are implemented in ServUO.

## 📋 **WHAT ACTUALLY EXISTS**

**Current working commands:**
- `[spawnvystia]` - Spawn Vystia creatures via gump
- `[spawnvystia <radius>]` - Spawn with custom radius
- `[clearvystia]` - Remove Vystia creatures
- `[add <CreatureName>]` - Direct spawn (e.g., `[add FrostFather`)

**What this document describes (NOT REAL):**

## 🎮 **COMMAND SYNTAX**

**IMPORTANT:** ServUO commands use **one opening bracket** syntax:
- ✅ **Correct:** `[CommandName`
- ❌ **Incorrect:** `[CommandName]` or `CommandName`

## 🚀 **QUICK START**

1. **Start ServUO Server** from `C:\DevEnv\GIT\UO\ServUO\`
2. **Connect to server** (default port)
3. **Use commands** with single bracket syntax
4. **Check status** with `[VystiaStatus` first

---

## 📚 **COMPLETE COMMAND LIST**

### **1. STATUS & DEPLOYMENT COMMANDS**

#### **`[VystiaStatus`**
- **Description:** Shows current Vystia deployment status
- **Access Level:** Administrator
- **Usage:** `[VystiaStatus`
- **Expected Output:** Lists what Vystia content is currently deployed
- **Purpose:** Check system status before making changes

#### **`[VystiaDeploy full`**
- **Description:** Deploys complete Vystia content (creatures, world, dungeons)
- **Access Level:** Administrator
- **Usage:** `[VystiaDeploy full`
- **Expected Output:** Complete deployment of all Vystia systems
- **Purpose:** One-command setup of entire Vystia world

#### **`[VystiaDeploy creatures`**
- **Description:** Deploys only Vystia creatures (150+ custom creatures)
- **Access Level:** Administrator
- **Usage:** `[VystiaDeploy creatures`
- **Expected Output:** All Vystia creatures become available for spawning
- **Purpose:** Deploy creature system without world generation

#### **`[VystiaDeploy world`**
- **Description:** Deploys only world generation (terrain, cities, roads)
- **Access Level:** Administrator
- **Usage:** `[VystiaDeploy world`
- **Expected Output:** Vystia terrain and cities generated
- **Purpose:** Deploy world without creatures or dungeons

#### **`[VystiaDeploy dungeons`**
- **Description:** Deploys only dungeon spawners
- **Access Level:** Administrator
- **Usage:** `[VystiaDeploy dungeons`
- **Expected Output:** All 18 dungeon spawners created
- **Purpose:** Deploy dungeons without world generation

---

### **2. WORLD GENERATION COMMANDS**

#### **`[GenVystiaWorld`**
- **Description:** Generates the complete Vystia world map with cities, dungeons, and spawners
- **Access Level:** Administrator
- **Usage:** `[GenVystiaWorld`
- **Expected Output:** 
  - Terrain generation messages
  - City creation notifications
  - Dungeon spawner placement
  - Road network creation
- **Purpose:** Create the entire Vystia world in one command
- **Time:** May take several minutes for complete generation

#### **`[ClearVystiaWorld`**
- **Description:** Clears the Vystia world (removes all generated content)
- **Access Level:** Administrator
- **Usage:** `[ClearVystiaWorld`
- **Expected Output:** Confirmation of cleared items
- **Purpose:** Reset world to clean state
- **Warning:** This removes ALL Vystia-generated content

---

### **3. DUNGEON SPAWNER COMMANDS**

#### **`[GenVystiaSpawners`**
- **Description:** Generates all 18 Vystia dungeon spawners across 8 biomes
- **Access Level:** Administrator
- **Usage:** `[GenVystiaSpawners`
- **Expected Output:** 
  - Spawner generation for each dungeon
  - Confirmation messages for each biome
  - Total count of spawners created
- **Purpose:** Create dungeon spawners without world generation
- **Dungeons Created (18 Total) with Coordinates:**
  - **Frozen Halls** (1050, 150) - Frosthold - 4 levels + Boss
  - **Deepforge Mines** (4200, 1400) - Deepforge - 5 levels + Boss
  - **Pyramid of Surya** (4200, 2600) - Whispering Sands - 3 levels + Boss
  - **Emberdeep Caldera** (5400, 1800) - Emberlands - 3 levels + Boss
  - **Shadowfen Crypts** (5800, 3200) - Shadowfen - 3 levels + Boss
  - **Verdant Depths** (2800, 1200) - Verdantpeak - 2 levels + Boss
  - **Crystal Caverns** (1600, 2800) - Crystal Barrens - 4 levels + Boss
  - **Skyreach Summit** (1200, 600) - Skyreach Mountains - 3 levels + Boss
  - **Abyssal Trench** (2200, 3200) - Forgotten Depths - 5 levels + Boss
  - **Twilight Labyrinth** (2800, 2400) - Eternal Twilight - 3 levels + Boss
  - **Ironforge Foundry** (3800, 2200) - Ironclad Empire - 3 levels + Boss
  - **Sunken Temple** (2300, 3400) - Sunken Isles - 3 levels + Boss
  - **Hollow Tree Sanctum** (2600, 1800) - Hollow Forests - 2 levels + Boss
  - **Obsidian Spire** (3200, 3800) - Emberlands - 4 levels + Boss
  - **Radiant Sanctum** (4500, 1200) - Radiant Plains - 2 levels + Boss
  - **Mirage Palace** (5800, 2900) - Blazing Frontier - 3 levels + Boss
  - **Stormspear Citadel** (1800, 900) - Wilderlands - 3 levels + Boss
  - **Bone Cathedral** (3900, 1500) - Shadowfen - 2 levels + Boss

#### **`[ClearVystiaSpawners`**
- **Description:** Removes all Vystia spawners from the world
- **Access Level:** Administrator
- **Usage:** `[ClearVystiaSpawners`
- **Expected Output:** Confirmation of removed spawners
- **Purpose:** Clean up dungeon spawners
- **Warning:** This removes ALL Vystia spawners

---

### **4. TEST COMMANDS**

#### **`[TestCommand`**
- **Description:** Test command to verify Vystia system is working
- **Access Level:** Administrator
- **Usage:** `[TestCommand`
- **Expected Output:** "Test command is working! Initialize was called."
- **Purpose:** Verify command system functionality
- **Note:** This is a diagnostic command for troubleshooting

---

## 🗺️ **VYSTIA WORLD STRUCTURE**

### **Biomes & Regions (12 Total):**
1. **Frosthold** (Northern Tundra) - Ice-themed creatures and dungeons
2. **Winterguard** (Frozen Fortress) - Military outpost in tundra
3. **Verdantpeak** (Ancient Woodland) - Nature-themed creatures and dungeons
4. **Deepforge** (Subterranean) - Mining-themed creatures and dungeons
5. **Whispering Sands** (Arid Wasteland) - Sand-themed creatures and dungeons
6. **Emberlands** (Volcanic Region) - Fire-themed creatures and dungeons
7. **Shadowfen** (Marshland) - Poison-themed creatures and dungeons
8. **Skyreach Mountains** (Snow-capped Peaks) - Air-themed creatures and dungeons
9. **Crystal Barrens** (Crystalline Mountains) - Crystal-themed creatures and dungeons
10. **Eternal Twilight** (Mystical Forest) - Shadow-themed creatures and dungeons
11. **Sunken Isles** (Coastal Archipelago) - Water-themed creatures and dungeons
12. **Ironclad Empire** (Industrial Region) - Metal-themed creatures and dungeons

### **Cities (23 Total):**

#### **Major Cities (8):**
- **Ironheart Capital** (800, 600) - Ironclad Empire - Castle, Bank, Market, Mage Shop, Docks, Factory, Smithy
- **Frostholm** (1200, 200) - Frosthold - Great Hall, Ice Shrine, Fur Trader, Inn
- **Verdant Grove** (1800, 400) - Verdantpeak - Tree Houses, Grove Shrine, Market, Herbalist
- **Deepforge City** (2400, 1400) - Deepforge - Great Forge, Mine, Bank, Armory, Tavern (Underground)
- **Sandara** (4200, 2600) - Whispering Sands - Pyramid, Bazaar, Oasis, Tomb Entrance
- **Emberforge** (5400, 1800) - Emberlands - Lava Forge, Fire Shrine, Obsidian Market
- **Pearl Harbor** (3600, 3400) - Sunken Isles - Docks, Shipwright, Market, Diving Guild, Underwater Entrance
- **Crystal Spire** (4800, 2200) - Crystal Barrens - Crystal Market, Mage Guild, Gem Cutters

#### **Minor Cities (15):**
- **Winterguard Citadel** (800, 100) - Winterguard - Fortress, Hot Springs, Armory
- **Mistwood** (1200, 2800) - Shadowfen - Witch Hut, Alchemist, Bog Shrine
- **Skyhold Monastery** (3200, 800) - Skyreach Mountains - Monastery, Shrine, Meditation Gardens
- **Duskhaven** (2800, 2400) - Eternal Twilight - Twilight Market, Shadow Shrine, Illusion Academy
- **Steppeheart** (4400, 1200) - Golden Steppe - Yurt Camp, Horse Market, Nomad Shrine
- **Wilderhaven** (3800, 1600) - Wilderlands - Tribal Hall, Totem Circle, Beast Pens
- **Abyssal Gate** (2200, 3200) - Forgotten Depths - Underwater Portal, Merfolk Embassy, Diving Station
- **Sunfire Outpost** (5200, 2800) - Blazing Frontier - Sun Temple, Mirage Market, Water Shrine
- **Verdant Port** (2600, 3600) - Verdant Isles - Docks, Jungle Market, Shipwright, Pirate Haven
- **Canyon Sanctum** (4600, 2000) - Mystic Canyons - Observatory, Rune Library, Mystic Shrine
- **Hollowshade** (1600, 2000) - Hollow Forests - Hollow Trees, Fae Circle, Shadow Market
- **Radiance** (5600, 1200) - Radiant Plains - Sun Farm, Solar Temple, Light Market
- **Glimmer Cove** (4800, 3400) - Glimmering Archipelago - Docks, Bioluminescent Gardens, Night Market
- **Shadowfen Dock** (900, 2400) - Shadowfen - Bog Docks, Potion Shop, Curse Market
- **Northwatch** (600, 300) - Frosthold - Watchtower, Shipyard, Ice Market

---

## ⚙️ **TECHNICAL DETAILS**

### **File Locations:**
- **Main Server:** `C:\DevEnv\GIT\UO\ServUO\`
- **Scripts:** `C:\DevEnv\GIT\UO\ServUO\Scripts\`
- **Vystia Source:** `C:\DevEnv\GIT\UO\Vystia\src\`
- **Compiled Scripts:** `C:\DevEnv\GIT\UO\ServUO\Scripts.dll`

### **Command Registration:**
- Commands are registered via `Initialize()` methods
- Called automatically by ServUO's `ScriptCompiler.Invoke("Initialize")`
- All commands require Administrator access level

### **Dependencies:**
- **XmlSpawner2** - For dungeon spawner functionality
- **Server.Items.Static** - For world generation
- **Server.Mobiles** - For creature spawning
- **Server.Commands** - For command system

---

## 🚨 **TROUBLESHOOTING**

### **Command Not Recognized:**
1. **Check syntax:** Use `[CommandName` (single bracket)
2. **Verify server:** Ensure ServUO is running from correct directory
3. **Check access:** Commands require Administrator level
4. **Restart server:** If commands were added recently

### **Server Crashes:**
1. **Check logs:** Look for compilation errors
2. **Verify files:** Ensure all Vystia files are in correct locations
3. **Start from main directory:** `C:\DevEnv\GIT\UO\ServUO\`

### **Commands Not Working:**
1. **Test with `[TestCommand`:** Verify basic functionality
2. **Check `[VystiaStatus`:** Verify system state
3. **Rebuild Scripts:** Run `dotnet build Scripts/Scripts.csproj`

---

## 📖 **USAGE EXAMPLES**

### **First Time Setup:**
```
[VystiaStatus
[VystiaDeploy full
[GenVystiaWorld
```

### **Dungeon Only Setup:**
```
[GenVystiaSpawners
```

### **World Only Setup:**
```
[GenVystiaWorld
```

### **Clean Reset:**
```
[ClearVystiaSpawners
[ClearVystiaWorld
```

### **Status Check:**
```
[VystiaStatus
```

---

## 🎯 **RECOMMENDED WORKFLOW**

1. **Start Server** from `C:\DevEnv\GIT\UO\ServUO\`
2. **Connect** to server
3. **Check Status:** `[VystiaStatus`
4. **Deploy Content:** `[VystiaDeploy full`
5. **Generate World:** `[GenVystiaWorld`
6. **Verify:** Check for generated content in-game

---

## 📞 **SUPPORT**

If commands are not working:
1. **Verify server is running** from correct directory
2. **Check command syntax** (single bracket)
3. **Test with `[TestCommand`** first
4. **Check server logs** for errors
5. **Restart server** if needed

---

*Last Updated: October 2025*
*Vystia System Version: 1.0*
*ServUO Compatibility: Current*
