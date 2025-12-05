# VYSTIA INSTALLATION GUIDE

Complete setup instructions for the Vystia world system.

---

## 📋 Prerequisites

### **System Requirements**
- **ServUO** - Ultima Online server emulator
- **UO:Renaissance Client** - For proper creature display
- **.NET Framework** - For ServUO compilation
- **Administrator Access** - For server commands

### **File Structure**
Ensure you have the following directory structure:
```
ServUO/
├── Scripts/
│   ├── Systems/
│   │   ├── Vystia/           # Creature files
│   │   └── VystiaDungeons/   # Dungeon spawners
│   └── Scripts.csproj       # Project file
└── Vystia/                   # World system files
    ├── config/
    ├── src/
    ├── docs/
    └── guides/
```

---

## 🚀 Installation Steps

### **Step 1: Verify File Locations**

**Creature Files (Already in ServUO):**
- `ServUO/Scripts/Systems/Vystia/VystiaCreatures_Part1.cs`
- `ServUO/Scripts/Systems/Vystia/VystiaCreatures_Part2.cs`

**Dungeon Spawners (Already in ServUO):**
- `ServUO/Scripts/Systems/VystiaDungeons/VystiaDungeonSpawners.cs`

**World Generator (Linked from Vystia):**
- `Vystia/src/VystiaWorldGenerator.cs` → `ServUO/Scripts/Systems/Vystia/`
- `Vystia/src/VystiaDeployment.cs` → `ServUO/Scripts/Systems/Vystia/`

**Configuration:**
- `Vystia/config/VYSTIA_WORLD_CONFIG.json`

### **Step 2: Build ServUO**

1. **Open Command Prompt** as Administrator
2. **Navigate to ServUO directory:**
   ```cmd
   cd C:\DevEnv\GIT\UO\ServUO
   ```
3. **Build the project:**
   ```cmd
   dotnet build Scripts/Scripts.csproj
   ```
4. **Verify build success:**
   - Should show "Build succeeded" with 0 errors
   - If errors occur, check file paths in `Scripts.csproj`

### **Step 3: Start Server**

1. **Navigate to server directory:**
   ```cmd
   cd Server/bin/Release
   ```
2. **Start ServUO:**
   ```cmd
   ServUO.exe
   ```
3. **Verify server startup:**
   - Server should start without errors
   - Commands should be registered automatically

---

## ⚙️ Configuration

### **World Configuration**

Edit `Vystia/config/VYSTIA_WORLD_CONFIG.json` to customize:

**Basic Settings:**
```json
{
  "world_config": {
    "name": "Vystia",
    "seed": 777333,
    "map": "Felucca",
    "dimensions": {
      "width": 7168,
      "height": 4096
    }
  }
}
```

**Biome Customization:**
- Modify `terrain_generation.biomes` array
- Adjust tile IDs, elevation ranges, and frequencies
- Add or remove biome types

**City Placement:**
- Modify `city_generation.cities` array
- Adjust city sizes, features, and forced locations
- Add or remove cities

**Dungeon Configuration:**
- Modify `dungeon_generation.dungeons` array
- Adjust difficulty levels and themes
- Add or remove dungeons

### **Creature Customization**

**Stats Adjustment:**
- Edit creature files in `ServUO/Scripts/Systems/Vystia/`
- Modify `SetStr()`, `SetDex()`, `SetInt()` values
- Adjust `SetHits()`, `SetDamage()` values

**Resistance Types (UO:R Compatible):**
```csharp
SetResistance(ResistanceType.Physical, 60, 70);
SetResistance(ResistanceType.Poison, 40, 50);
SetResistance(ResistanceType.Energy, 40, 50);
SetSkill(SkillName.MagicResist, 80.0, 100.0);
```

**Body Types:**
- Use only UO:R compatible body IDs (under 200)
- Avoid Third Dawn bodies (752, 301, 317, 788)
- Reference `docs/BODY_ART_IMPLEMENTATION_GUIDE.md`

---

## 🎮 Deployment Options

### **Option 1: New Map (RECOMMENDED)**

**Best for:** Existing servers with established worlds

**Process:**
1. Use `[VystiaDeploy full]` in-game
2. Vystia generates as Map ID 5
3. Create moongates for player access
4. No impact on existing Felucca/Trammel

**Advantages:**
- Safe for existing servers
- Professional presentation
- Easy to remove if needed

### **Option 2: Replace Felucca**

**Best for:** New servers or test environments

**Process:**
1. **BACKUP existing Felucca** (critical!)
2. Use `[GenVystiaWorld]` to replace map
3. All players immediately access Vystia

**Advantages:**
- Immediate access for all players
- No moongate setup needed
- Full world replacement

**Disadvantages:**
- Destroys existing world
- Requires backup/restore process

### **Option 3: Expansion Pack**

**Best for:** Gradual integration

**Process:**
1. Use `[VystiaDeploy creatures]` first
2. Gradually add cities and dungeons
3. Integrate with existing content

**Advantages:**
- Gradual implementation
- Integrates with existing world
- Flexible deployment

---

## 🔧 Troubleshooting

### **Build Errors**

**Error: "Duplicate 'Compile' items"**
- **Solution:** Check `Scripts.csproj` for duplicate entries
- **Fix:** Remove duplicate `<Compile Include>` lines

**Error: "The type or namespace name '...' could not be found"**
- **Solution:** Check using statements in source files
- **Fix:** Add missing `using Server.Systems.VystiaDungeons;`

**Error: "StaticTile does not contain a constructor"**
- **Solution:** Use `Static` instead of `StaticTile`
- **Fix:** Change `new StaticTile(id)` to `new Static(id)`

### **Runtime Errors**

**Error: "Command not recognized"**
- **Solution:** Ensure server compiled successfully
- **Fix:** Rebuild project and restart server

**Error: "Map not found"**
- **Solution:** Check map configuration in JSON
- **Fix:** Verify `"map": "Felucca"` setting

**Error: "Spawner creation failed"**
- **Solution:** Check dungeon coordinates
- **Fix:** Ensure coordinates are within map bounds

### **Performance Issues**

**Slow World Generation:**
- **Solution:** Reduce world dimensions in JSON
- **Fix:** Lower `width` and `height` values

**Too Many Creatures:**
- **Solution:** Adjust spawn density
- **Fix:** Modify `spawners.spawn_density` in JSON

**Memory Usage:**
- **Solution:** Limit city and dungeon counts
- **Fix:** Reduce arrays in JSON configuration

---

## ✅ Verification

### **Post-Installation Checklist**

1. **Build Success:**
   - [ ] ServUO builds without errors
   - [ ] All Vystia files compile correctly
   - [ ] No missing dependencies

2. **Server Startup:**
   - [ ] ServUO starts without errors
   - [ ] Commands are registered
   - [ ] No runtime exceptions

3. **Command Testing:**
   - [ ] `[VystiaStatus]` shows deployment status
   - [ ] `[GenVystiaWorld]` generates world
   - [ ] `[GenVystiaSpawners]` creates spawners

4. **Content Verification:**
   - [ ] Creatures spawn correctly
   - [ ] Cities generate properly
   - [ ] Dungeons populate with spawners

### **Test Commands**

**Basic Testing:**
```
[VystiaStatus]           # Check deployment status
[GenVystiaWorld]         # Generate world (test)
[ClearVystiaWorld]       # Clear world (cleanup)
```

**Advanced Testing:**
```
[GenVystiaSpawners]      # Generate dungeon spawners
[ClearVystiaSpawners]    # Clear dungeon spawners
[VystiaDeploy full]      # Full deployment
```

---

## 📞 Support

### **Common Issues**
- **Build Problems:** Check file paths and dependencies
- **Runtime Errors:** Verify server compilation and startup
- **Performance Issues:** Adjust configuration parameters

### **Documentation**
- **World Lore:** `docs/Vystia World Lore.md`
- **Creature Guide:** `docs/VYSTIA_BESTIARY_UOR_CORRECTED.md`
- **Implementation:** `docs/VYSTIA_MISSING_CONTENT.md`

### **File Locations**
- **Source Code:** `Vystia/src/`
- **Configuration:** `Vystia/config/`
- **Documentation:** `Vystia/docs/`
- **Guides:** `Vystia/guides/`

---

**Installation Complete!** 🎉

Your Vystia world is ready for adventure. Use `[GenVystiaWorld]` to begin!