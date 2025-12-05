# What Actually Exists - Vystia Reality Check

**Last Updated:** 2025-12-05

## ✅ **WHAT IS REAL AND WORKING**

### **1. Creatures (131 Total)**
- **Location:** `ServUO/Scripts/Mobiles/Vystia/`
- **Status:** ✅ Compiled and functional
- **Categories:**
  - 10 Regional Bosses
  - 121 Regional Creatures across 12 biomes

### **2. Items & Resources (100+ Items)**
- **Location:** `ServUO/Scripts/Items/Vystia/`
- **Status:** ✅ Compiled and functional
- **Categories:**
  - 8 Regional Ores + 8 Ingots
  - 3 Leathers + Woods
  - 10+ Reagents (elemental, nature, exotic)
  - 8 Regional Weapon Sets
  - 8 Regional Armor Sets
  - Legendary Weapons & Artifacts
  - Special Resources & Components
- **See:** [ITEMS_INVENTORY.md](ITEMS_INVENTORY.md) for complete list

**Creature Spawn Commands:**
```
[spawnvystia          # Opens gump to spawn creatures (10 tile radius)
[spawnvystia 20       # Opens gump with custom radius (5-50)
[clearvystia          # Delete Vystia creatures in 10 tile radius
[clearvystia 50       # Delete with custom radius (5-100)
```

**Direct Creature Spawning:**
```
[add FrostFather      # Spawn Frosthold boss
[add AncientTreant    # Spawn Verdantpeak boss
[add IceTroll         # Spawn Frosthold creature
```

**Direct Item Spawning:**
```
[add FrozenOre              # Spawn Frosthold ore
[add FrostforgedIngot       # Spawn Frosthold ingot
[add FrostEssence           # Spawn frost reagent
[add TheEternalWinter       # Spawn legendary weapon
[add GlacialLeather         # Spawn Frosthold leather
```

See [DOCS_INDEX.md](DOCS_INDEX.md) for complete creature list.
See [ITEMS_INVENTORY.md](ITEMS_INVENTORY.md) for complete item list.

---

## ❌ **WHAT DOES NOT EXIST**

### **No Vystia Map**
- No world map has been generated
- Coordinates in other docs lead nowhere
- No cities or dungeons have been placed in-game

### **Commands That Don't Exist**
These commands are in design docs but **NOT implemented:**
- ❌ `[VystiaStatus`
- ❌ `[VystiaDeploy`
- ❌ `[GenVystiaWorld`
- ❌ `[ClearVystiaWorld`
- ❌ `[GenVystiaSpawners`
- ❌ `[ClearVystiaSpawners`

### **Locations That Don't Exist**
These are design concepts only:
- ❌ 23 cities (Ironheart Capital, Frostholm, etc.)
- ❌ 18 dungeons (Frozen Halls, Deepforge Mines, etc.)
- ❌ Any specific coordinates for Vystia locations

---

## 🗺️ **WORLD GENERATION STATUS**

### **Terrain Generation**
- **Tool:** `VystiaTerrainGeneration/` directory
- **Status:** Pipeline exists but map NOT deployed to ServUO
- **Output:** Can generate Terrain.bmp and Altitude.bmp
- **Problem:** No integration with ServUO map system

### **Town Deployment**
- **Tool:** `VystiaTownDeployment/` directory
- **Status:** Tools exist to extract/deploy OSI towns
- **Deployed:** Nothing deployed yet
- **Available:** 13 extracted OSI towns ready to deploy

---

## 📋 **HOW TO USE WHAT EXISTS**

### **1. Spawn Creatures Anywhere**
```
# Start ServUO server
cd C:\DevEnv\GIT\UO\ServUO
./ServUO.exe

# In-game (as GM/Admin):
[spawnvystia          # Select creatures from gump
```

### **2. Test Individual Creatures**
```
[add FrostFather      # Spawn boss to test
[add IceTroll         # Spawn regular creature
```

### **3. Create Spawners (If Using XMLSpawner)**
```
[xmlspawner add       # Create XMLSpawner
# Set creature type to any Vystia creature name
# Examples: FrostFather, IceTroll, VolcanoWyrm
```

---

## 🎯 **WHAT THE OTHER DOCS ARE**

### **docs/ Directory**
- **PLANNED_COMMANDS.md** - Fantasy commands (NOT real)
- **PLANNED_LOCATIONS.md** - Fantasy map locations (NOT real)
- **PLANNED_MAP.md** - Fantasy world geography (NOT real)
- **VYSTIA_WORLD_LORE.md** - Story/lore (design only)
- **VYSTIA_CREATURES_BESTIARY.md** - Creature designs (creatures DO exist)
- Other docs - Design specs and planning

**These are world design documents**, not reality. They describe what SHOULD exist, not what DOES exist.

---

## 💡 **TO BUILD A VYSTIA WORLD**

You would need to:

1. **Generate Terrain**
   - Use `VystiaTerrainGeneration/` to create map files
   - Convert to .mul format
   - Replace ServUO's map0.mul (or use a different map slot)

2. **Deploy Towns**
   - Use `VystiaTownDeployment/` to place towns
   - Modify statics0.mul to add buildings

3. **Create World Commands**
   - Write C# commands in ServUO to:
     - Generate spawners
     - Place dungeons
     - Create regions
     - Add NPCs

4. **Integrate Creatures**
   - Creatures already exist and work
   - Just need spawners placed in the world

**Current Status:** None of this integration has been done yet.

---

## ✅ **SUMMARY**

**Real:**
- 131 creatures (compiled and spawnable)
- `[spawnvystia]` command
- Creature design docs

**Not Real:**
- Vystia world map
- 23 cities
- 18 dungeons
- World generation commands
- Any placed content in-game

**Use this document** as your reality check. If something isn't listed here as "Real", it doesn't exist yet.

---

*This is the truth about Vystia. The other docs are aspirational.*
