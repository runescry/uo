# VYSTIA DEPLOYMENT GUIDE
## Three Ways to Add Vystia to Your Server

Choose the deployment method that works best for your server!

## 📋 Deployment Options

### Option 1: New Map (RECOMMENDED) ⭐
- Creates Vystia as a **separate map** (like Trammel/Felucca)
- **No impact** on existing world
- Players use gates/teleporters to travel there
- **Best for**: Servers with established worlds

### Option 2: Replace Felucca
- **Overwrites** Felucca map with Vystia
- Felucca becomes the Vystia world
- **Best for**: New servers or servers not using Felucca

### Option 3: Expansion Pack
- Adds Vystia to **unused areas** of existing map
- Coexists with current content
- **Best for**: Servers wanting seamless integration

---

## 🌍 OPTION 1: NEW MAP (Recommended)

This creates Vystia as Map ID 5 (separate from Felucca/Trammel).

### Files Needed
- VystiaDeployment.cs (NEW!)
- VystiaWorldGenerator.cs (UPDATED!)
- VystiaCreatures_Part1.cs
- VystiaCreatures_Part2.cs
- VystiaDungeonSpawners.cs
- Vystia_World_Config.json

### Installation Steps

#### 1. Copy All Files
```bash
cp VystiaDeployment.cs /ServUO/Scripts/ProceduralGeneration/
cp VystiaWorldGenerator.cs /ServUO/Scripts/ProceduralGeneration/
cp VystiaCreatures_Part1.cs /ServUO/Scripts/Mobiles/Vystia/
cp VystiaCreatures_Part2.cs /ServUO/Scripts/Mobiles/Vystia/
cp VystiaDungeonSpawners.cs /ServUO/Scripts/Systems/VystiaDungeons/
cp Vystia_World_Config.json /ServUO/Data/WorldGen/
```

#### 2. Install Dependencies
```bash
cd /ServUO
dotnet add package Newtonsoft.Json
```

#### 3. Restart Server
Server auto-compiles all scripts

#### 4. Create Vystia Map
```csharp
[CreateVystiaMap
```
This creates Map ID 5 named "Vystia"

#### 5. Generate World on New Map
```csharp
[GenerateWorld Vystia_World_Config.json 5
```
The "5" tells it to generate on Map 5 (Vystia)

Takes ~30 minutes. Watch console for progress.

#### 6. Generate Dungeon Spawners
```csharp
[GenVystiaSpawners
```
Takes ~1 minute. Creates 45+ spawners.

#### 7. Create Gates to Vystia

Place gates in your existing cities:

```csharp
// In Britain
[VystiaGate

// In Trinsic  
[VystiaGate

// In Moonglow
[VystiaGate
```

Or create teleporter pads:
```csharp
[VystiaTeleporter
```

#### 8. Test It!
```csharp
// Use the gate you created, or teleport directly
[go 3500 2000 0 5  // Teleport to center of Vystia map

// Check map
[worldstats

// Travel to cities
[gotocity Ironheart Capital
[gotocity Frostholm
```

### Creating Return Gates

Players need a way back! At Vystia locations:

```csharp
// Create return gate to Britain
[add Moongate
[set Target Britain

// Or use teleporter
[add Teleporter
[set PointDest 1500 1750 0  // Britain coords
[set MapDest Felucca
```

### Advantages
✅ No impact on existing world
✅ Easy to remove if needed
✅ Clean separation
✅ Players choose when to visit
✅ Can have different rulesets

### Disadvantages  
❌ Requires gates/teleporters
❌ Separate from main world
❌ Players might not discover it

---

## 🗺️ OPTION 2: REPLACE FELUCCA

This **overwrites** the Felucca map entirely with Vystia.

### ⚠️ WARNING
**This permanently replaces Felucca!** Backup first!

### Installation Steps

#### 1. Backup Your World!
```bash
cd /ServUO/Saves
tar -czf backup_$(date +%Y%m%d).tar.gz *
```

#### 2. Copy Files (same as Option 1)
```bash
cp VystiaWorldGenerator.cs /ServUO/Scripts/ProceduralGeneration/
cp VystiaCreatures_Part1.cs /ServUO/Scripts/Mobiles/Vystia/
cp VystiaCreatures_Part2.cs /ServUO/Scripts/Mobiles/Vystia/
cp VystiaDungeonSpawners.cs /ServUO/Scripts/Systems/VystiaDungeons/
cp Vystia_World_Config.json /ServUO/Data/WorldGen/
```

#### 3. Restart Server

#### 4. Generate World (Replaces Felucca)
```csharp
[GenerateWorld Vystia_World_Config.json
```
Or explicitly:
```csharp
[GenerateWorld Vystia_World_Config.json 0
```

This generates on Map 0 (Felucca), replacing everything!

#### 5. Generate Spawners
```csharp
[GenVystiaSpawners
```

#### 6. Done!
```csharp
// Players spawn in Vystia automatically
// Old Felucca is gone
[gotocity Ironheart Capital
```

### Advantages
✅ No gates needed
✅ Players start in Vystia
✅ Seamless experience
✅ Simpler setup

### Disadvantages
❌ Destroys existing Felucca
❌ Can't coexist with original world
❌ Permanent change

### Best For
- Brand new servers
- Servers not using Felucca
- Servers wanting a complete conversion
- Vystia-only shards

---

## 🧩 OPTION 3: EXPANSION PACK

This adds Vystia to **unused areas** of your existing map.

### How It Works
Vystia generates at coordinates **5000, 3000** (or wherever you specify) on your existing map, leaving your current content intact.

### Installation Steps

#### 1. Find Empty Map Space

Check your map for unused areas:
```csharp
[go 5000 3000 0  // Check if area is empty
[go 6000 3500 0  // Check surrounding areas
```

#### 2. Edit Config File

Open `Vystia_World_Config.json` and offset all coordinates:

```json
{
  "cities": [
    {
      "name": "Ironheart Capital",
      "location": {
        "x": 8500,  // CHANGED: was 3500, now 3500+5000
        "y": 5048,  // CHANGED: was 2048, now 2048+3000
        "z": 0
      }
    }
  ]
}
```

**Quick method**: Use find/replace to add 5000 to all X coords and 3000 to all Y coords.

#### 3. Copy Files & Restart
Same as Option 1

#### 4. Generate World
```csharp
[GenerateWorld Vystia_World_Config.json
```

Vystia generates at offset location, leaving existing content untouched!

#### 5. Generate Spawners
```csharp
[GenVystiaSpawners
```

#### 6. Create Travel Methods

Add gates/teleporters from your cities to Vystia:

```csharp
// In Britain, create gate to Ironheart
[add Moongate
[set PointDest 8500 5048 0
[set MapDest Felucca
```

### Advantages
✅ Coexists with existing world
✅ No separate map needed
✅ Seamless exploration
✅ Can walk between worlds

### Disadvantages
❌ Requires manual coordinate offset
❌ Needs empty map space
❌ Can feel disconnected
❌ More complex setup

### Best For
- Servers with space on existing map
- Wanting connected world feel
- Gradual expansion approach

---

## 🔧 Advanced Configuration

### Changing Map for New Map Mode

Edit `VystiaDeployment.cs`:

```csharp
public const int NEW_MAP_ID = 5;  // Change to 6, 7, etc.
public const string NEW_MAP_NAME = "Vystia";
```

### Custom Map Size

Edit in `VystiaDeployment.cs`:

```csharp
public const int MAP_WIDTH = 7168;   // Make smaller: 5000
public const int MAP_HEIGHT = 4096;  // Make smaller: 3000
```

### Different Ruleset

```csharp
MapRules.FeluccaRules  // PvP enabled
MapRules.TrammelRules  // PvP disabled
```

---

## 📊 Comparison Chart

| Feature | New Map | Replace Felucca | Expansion Pack |
|---------|---------|-----------------|----------------|
| **Preserves Existing World** | ✅ Yes | ❌ No | ✅ Yes |
| **Setup Difficulty** | Easy | Easy | Medium |
| **Requires Gates** | ✅ Yes | ❌ No | ✅ Yes |
| **Separate Ruleset** | ✅ Yes | ❌ No | ❌ No |
| **Seamless Travel** | ❌ No | ✅ Yes | ✅ Yes |
| **Can Remove Easily** | ✅ Yes | ❌ No | Medium |
| **Best For New Servers** | ⭐⭐⭐ | ⭐⭐⭐⭐⭐ | ⭐⭐ |
| **Best For Existing Servers** | ⭐⭐⭐⭐⭐ | ⭐ | ⭐⭐⭐⭐ |

---

## 🎯 Our Recommendation

### For Most Servers: **Option 1 (New Map)** ⭐

**Why?**
- Safest option
- No risk to existing content
- Easy to add/remove
- Professional presentation
- Clear separation

**How to sell it to players:**
- "New realm to explore!"
- "Travel through mystical gates"
- "Separate adventure area"
- Like Lost Lands, Ilshenar, Tokuno, etc.

### For New/Fresh Servers: **Option 2 (Replace)**

**Why?**
- Clean start
- No gates needed
- Simpler for players
- Vystia becomes your world

### For Seamless Integration: **Option 3 (Expansion)**

**Why?**
- Feels like natural expansion
- No teleport immersion break
- Can walk between areas

---

## 🚀 Quick Start By Server Type

### Brand New Server
```bash
# Use Option 2
cp files...
restart
[GenerateWorld Vystia_World_Config.json 0
[GenVystiaSpawners
```

### Established Server
```bash
# Use Option 1
cp files...
restart
[CreateVystiaMap
[GenerateWorld Vystia_World_Config.json 5
[GenVystiaSpawners
[VystiaGate  # Place in each city
```

### Expansion-Focused Server
```bash
# Use Option 3
# Edit Vystia_World_Config.json coords first!
cp files...
restart
[GenerateWorld Vystia_World_Config.json
[GenVystiaSpawners
```

---

## 🎮 Player Experience

### Option 1 (New Map)
1. Player hears about Vystia
2. Finds moongate in Britain
3. Steps through gate
4. Arrives in Ironheart Capital
5. Explores new world
6. Uses return gate when done

### Option 2 (Replace)
1. Player logs in
2. Already in Vystia (Ironheart Capital)
3. Explores naturally
4. No gates needed

### Option 3 (Expansion)
1. Player exploring world
2. Discovers road leading northeast
3. Terrain changes
4. Enters Vystia regions naturally
5. Finds new cities

---

## 📝 Post-Installation

After deploying with any method:

### 1. Test Each City
```csharp
[gotocity Ironheart Capital
[gotocity Frostholm  
[gotocity Sandara
```

### 2. Test Each Dungeon
```csharp
[gotodungeon Frozen Halls
[gotodungeon Pyramid of Surya
[xmlrespawn  # Check creatures spawn
```

### 3. Verify Creatures
```csharp
[add WinterWolf  // Should work
[add LavaHound   // Should work
```

### 4. Player Communication

**For New Map:**
```
"Explore the new realm of Vystia! Use the mystical moongates 
in Britain, Trinsic, and Moonglow to travel to this 
steampunk world of magic and machinery!"
```

**For Replace:**
```
"We've completely reimagined our world! Welcome to Vystia, 
a realm of steampunk technology and elemental magic. 
Explore 23 new cities and 18 dangerous dungeons!"
```

**For Expansion:**
```
"New lands discovered to the northeast! The mysterious 
continent of Vystia awaits, filled with new cities, 
dungeons, and strange mechanical beasts!"
```

---

## 🛠️ Troubleshooting

### Map Creation Fails
```csharp
// Check map ID not in use
[props Map.Maps[5]  // Should be null or show Vystia

// Try different ID
Edit VystiaDeployment.cs: NEW_MAP_ID = 6
```

### World Generates on Wrong Map
```csharp
// Specify map explicitly
[GenerateWorld Vystia_World_Config.json 5  // For Map 5
```

### Gates Don't Work
```csharp
// Check destination
[props <gate>
// Verify PointDest and MapDest

// Recreate gate
[VystiaGate
```

### Can't Find Vystia After Generation
```csharp
// Check which map was used
[worldstats

// Teleport to map directly
[go 3500 2000 0 5  // x y z mapId
```

---

## 💾 Backup & Recovery

### Before Deployment
```bash
# Backup saves
cd /ServUO/Saves
tar -czf pre-vystia-backup.tar.gz *

# Backup scripts (if modifying)
cd /ServUO/Scripts
tar -czf scripts-backup.tar.gz *
```

### To Restore
```bash
# Stop server
# Restore saves
cd /ServUO/Saves
rm -rf *
tar -xzf pre-vystia-backup.tar.gz

# Start server
```

### To Remove Vystia (New Map Only)
```csharp
// Delete all Vystia items/mobiles
[wipe 5  // Wipes Map 5

// Or manually
[ClearVystiaSpawners
[global delete where map=5
```

---

## 📚 Summary

**We recommend Option 1 (New Map)** for most servers because:

1. ✅ Safe - doesn't touch existing world
2. ✅ Professional - like official UO expansions
3. ✅ Flexible - easy to modify or remove
4. ✅ Clear - players know it's a separate realm

**Steps:**
```bash
1. Copy files
2. Restart server
3. [CreateVystiaMap
4. [GenerateWorld Vystia_World_Config.json 5
5. [GenVystiaSpawners
6. [VystiaGate  (place in cities)
7. Announce to players!
```

Your choice depends on your server's needs, but all three options are fully supported!

Ready to deploy Vystia? Choose your option and follow the guide! 🚀🗺️
