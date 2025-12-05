# Ultima Online Development Environment

**Last Updated:** 2025-12-05

Complete development environment for Ultima Online custom shard development, including ServUO server, custom content systems, and world generation tools.

---

## 📂 Project Structure

### Core Server
- **[ServUO/](ServUO/)** - Main UO server emulator
  - See [ServUO/CLAUDE.md](ServUO/CLAUDE.md) for detailed project context
  - See [ServUO/README.md](ServUO/README.md) for ServUO documentation
  - See [ServUO/QUICK_BUILD.md](ServUO/QUICK_BUILD.md) for build instructions

### Custom Content Systems

The Vystia project consists of three complementary systems:

#### 1. Vystia Content System - **WHAT** populates the world
- **[Vystia/](Vystia/)** - Documentation and design workspace
  - **World lore, factions, and quest systems** (documentation)
  - **Creature bestiary** (design specs for 131 creatures)
  - **Regional resources** and crafting materials (design)
  - **Implementation guides** (how-to documentation)
  - See [Vystia/README.md](Vystia/README.md) for full documentation
  - See [Vystia/DOCS_INDEX.md](Vystia/DOCS_INDEX.md) for documentation index

- **ServUO/Scripts/** - Active game code (what actually runs)
  - `Scripts/Mobiles/Vystia/` - **131 creatures** ✅ Compiled and active
  - `Scripts/Items/Vystia/` - **Regional items** ✅ Compiled and active
  - `Scripts/Commands/SpawnVystiaGump.cs` - **Spawn command** ✅ Active
  - **In-game:** `[spawnvystia]` command

#### 2. Terrain Generation - **WHERE** the world exists (Step 1)
- **[VystiaTerrainGeneration/](VystiaTerrainGeneration/)** - Creates the base map terrain
  - Procedural terrain generation (mountains, water, land)
  - Voronoi region segmentation (21 regions)
  - Procedural island generation (78 islands)
  - **Outputs:** Terrain.bmp + Altitude.bmp → UOMapMake → `map0.mul`
  - See [VystiaTerrainGeneration/vystia_pipeline_v2/README.md](VystiaTerrainGeneration/vystia_pipeline_v2/README.md)

#### 3. Town Deployment - **HOW** structures are placed (Step 2)
- **[VystiaTownDeployment/](VystiaTownDeployment/)** - Places buildings and towns on the map
  - Extracts towns from OSI maps (13 towns available)
  - Deploys towns to custom maps with coordinate transformation
  - City wall generation and multi-building placement
  - **Modifies:** `statics0.mul`, `staidx0.mul` (adds buildings/decorations)
  - See [VystiaTownDeployment/README.md](VystiaTownDeployment/README.md) for usage guide

**Complete Workflow:**
```
Step 1: VystiaTerrainGeneration → Creates terrain (map0.mul)
Step 2: VystiaTownDeployment → Adds structures (statics0.mul)
Step 3: Use Vystia/ docs → Reference designs and lore
Step 4: ServUO/Scripts/Mobiles/Vystia/ → Active creatures spawn in-game
```

**Active Game Content:**
- ✅ 131 creatures compiled in `ServUO/Scripts/Mobiles/Vystia/`
- ✅ Spawn command: `[spawnvystia <radius>]`
- ✅ Items in `ServUO/Scripts/Items/Vystia/`

### Map Editing Tools
- **[centredsharp/](centredsharp/)** - CentrED map editor (C#)
- **[UO Fiddler/](UO%20Fiddler/)** - UO file editor
- **[UO Landscaper/](UO%20Landscaper/)** - Terrain editing tool
- **[ClassicUO/](ClassicUO/)** - Modern UO client

### Client Launchers
- **[ClassicUOLauncher/](ClassicUOLauncher/)** - ClassicUO launcher
- **[Razor/](Razor/)** - UO assistant tool

### Technical Documentation
- **[Documentation/](Documentation/)** - Technical guides and workflows
  - [CENTRED_SETUP.md](Documentation/CENTRED_SETUP.md) - CentrED configuration
  - [MAP_MERGE_PIPELINE.md](Documentation/MAP_MERGE_PIPELINE.md) - Map merging workflow
  - [MULTI_IMPORT_PIPELINE.md](Documentation/MULTI_IMPORT_PIPELINE.md) - Building import guide
  - [MAP_MERGING.md](Documentation/MAP_MERGING.md) - Map merging technical details
  - [QA_CHECKLIST_MAP_MERGE.md](Documentation/QA_CHECKLIST_MAP_MERGE.md) - Quality assurance

### Utility Scripts
Top-level Python scripts for various tasks:
- `analyze_towns.py` - Town data analysis
- `extract_spells.py` - Spell data extraction
- `split_lore_by_domain.py` - Lore organization
- `fix_map0_size.ps1` - Map file size correction
- `fix-redis-dll.ps1` - Redis DLL configuration

### Working Directories
- **[temp_map/](temp_map/)** - Temporary map processing workspace (200+ diagnostic scripts)
- **[ServUO_Map_Backups/](ServUO_Map_Backups/)** - Map file backups
- **[build/](build/)** - Build output directory

---

## 🚀 Quick Start

### 1. Build and Run ServUO
```bash
cd ServUO
dotnet restore
dotnet build
./ServUO.exe
```

See [ServUO/QUICK_BUILD.md](ServUO/QUICK_BUILD.md) for detailed build instructions.

### 2. Explore Vystia Content
Start with [Vystia/README.md](Vystia/README.md) to understand the complete world system including:
- 131 creatures across 12 biomes
- Regional resources and crafting
- Dungeon bosses and legendary items

### 3. Edit Maps
- **CentrED:** Use [centredsharp/](centredsharp/) for map editing
- **Town Generation:** Use [Vystia Town Generator/](Vystia%20Town%20Generator/) for deploying towns

---

## 📚 Key Documentation

### Getting Started
1. **[ServUO/CLAUDE.md](ServUO/CLAUDE.md)** - Complete project context and AI sidekick system
2. **[Vystia/README.md](Vystia/README.md)** - Vystia world system overview
3. **[Documentation/CENTRED_SETUP.md](Documentation/CENTRED_SETUP.md)** - Map editor setup

### Content Creation
- **[Vystia/DOCS_INDEX.md](Vystia/DOCS_INDEX.md)** - Complete Vystia documentation index
- **[Vystia/docs/VYSTIA_TODO.md](Vystia/docs/VYSTIA_TODO.md)** - Project TODO list
- **[Documentation/MULTI_IMPORT_PIPELINE.md](Documentation/MULTI_IMPORT_PIPELINE.md)** - Building import workflow

### Technical Guides
- **[Documentation/MAP_MERGE_PIPELINE.md](Documentation/MAP_MERGE_PIPELINE.md)** - Map merging process
- **[Vystia Town Generator/README.md](Vystia%20Town%20Generator/README.md)** - Town deployment guide
- **[ServUO/QUICK_BUILD.md](ServUO/QUICK_BUILD.md)** - Build troubleshooting

---

## 🛠️ Development Tools

### Installed Software
- **ServUO** - UO server emulator (.NET 6.0)
- **ClassicUO** - Modern client
- **CentrED** - Map editor
- **UO Fiddler** - File editor
- **Python 3** - Scripting environment
- **PowerShell** - Windows scripting

### File Locations
- **UO Client Files:** `C:\Program Files (x86)\Electronic Arts\Ultima Online Classic\`
- **Map Files:** `C:\DevEnv\GIT\UO\ServUO\Data\Maps\`
- **Server Data:** `C:\DevEnv\GIT\UO\ServUO\Data\`

---

## 🎯 Current Status

### ✅ Completed Systems
- ServUO server (fully functional)
- Vystia creatures (131 implemented)
- Vystia loot packs (complete)
- AI Sidekick system (mage, warrior, tamer archetypes)
- Town extraction and deployment tools
- Map generation pipeline

### 🔄 In Progress
- Vystia resources and crafting system
- Vystia equipment implementation
- Town deployment (13 OSI towns extracted and ready)
- Town regions and guard systems

### 📋 Planned
- Crafting system integration
- Quest system
- Faction mechanics
- Additional town deployments

---

## 📖 Additional Resources

### Reference Guides
- [MEMORY_SYSTEM_STATUS.md](MEMORY_SYSTEM_STATUS.md) - Memory system status
- [UOP_EDITING_GUIDE.md](UOP_EDITING_GUIDE.md) - UOP file editing guide

### Data Files
- `schema.sql` - Database schema
- `settings.json` - Development settings
- `UO.code-workspace` - VS Code workspace configuration

---

## 🤝 Contributing

This is a custom UO shard development environment. When making changes:
1. Document all modifications in relevant README files
2. Update the [Vystia/docs/VYSTIA_TODO.md](Vystia/docs/VYSTIA_TODO.md) for content changes
3. Keep dates updated (format: YYYY-MM-DD)
4. Test changes in both ServUO and CentrED before committing

---

## 📞 Support

- **ServUO Documentation:** See [ServUO/README.md](ServUO/README.md)
- **Vystia Documentation:** See [Vystia/DOCS_INDEX.md](Vystia/DOCS_INDEX.md)
- **Technical Issues:** Check [Documentation/](Documentation/) for guides

---

*Development Environment - December 2025*
