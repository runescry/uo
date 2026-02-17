# ServUO AI Sidekick Project Context

## Project Overview
This is a ServUO (Ultima Online server emulator) project with a custom **AI Sidekick system** - autonomous NPC companions that fight alongside players using intelligent combat AI.

## Key Directories

### AI Sidekick System
- `Scripts/Services/AISidekicks/` - Main sidekick system
  - `AI/SidekickAI.cs` - Base AI controller
  - `AI/MageCombatAI.cs` - Mage combat logic (kiting, spell combos, healing)
  - `AI/PlayerLikeCombatAI.cs` - Melee combat logic
  - `Archetypes/` - Sidekick class definitions (mage, warrior, etc.)

### Combat Simulation System
- `Scripts/Services/AISidekicks/Simulation/` - Python-based combat simulator
  - `full_combat_simulator.py` - Complete combat simulation
  - `parameter_optimizer.py` - Parameter sweep optimization
  - `combat_mechanics.py` - UO combat formulas (spell damage, healing, etc.)
  - `map_data_extractor.py` - Reads UO .mul map files
  - `fast_astar.py` - A* pathfinding port from ServUO
  - `walkability_check.py` - Map.CanFit() port
  - `movement_check.py` - Movement validation

## Current Focus: Mage AI Optimization

The combat simulation system tests Sidekick AI parameters against:
1. **Ogre Lord** - High HP melee enemy (tests kiting)
2. **Lich** - Enemy mage (tests mage vs mage combat)

### Key Optimizable Parameters (in MageCombatAI.cs)
| Parameter | Current | Description |
|-----------|---------|-------------|
| `MIN_RETREAT_DISTANCE` | 4 | Start retreating when enemy within X tiles |
| `SPELL_RELEASE_RANGE_MAX` | 14 | Maximum casting distance |
| `LOW_HEALTH` | 0.70 | HP% threshold to start healing |
| `CRITICAL_HEALTH` | 0.29 | HP% for emergency actions |

### Workflow
1. Modify parameters in simulation (`full_combat_simulator.py`)
2. Run `python parameter_optimizer.py dual` to test against both enemies
3. Apply best parameters to `MageCombatAI.cs`
4. Build with `dotnet build`
5. Test in-game in dungeon area (5379,4) to (5499,125)

## UO Map Data
- Client files: `C:\Program Files (x86)\Electronic Arts\Ultima Online Classic`
- Test dungeon region: (5379, 4) to (5499, 125) - 120x121 tiles

## Build Commands
```bash
cd D:\UO\ServUO
dotnet restore
dotnet build
```

## Simulation Commands
```bash
cd Scripts/Services/AISidekicks/Simulation
python full_combat_simulator.py          # Single test
python parameter_optimizer.py dual       # Optimize for both enemies
python parameter_optimizer.py quick      # Quick optimization
```

## Important Notes
- The simulation ports real ServUO C# code to Python for testing
- Sidekick archetypes start with Mages, will expand to other classes
- Parameters found in simulation should be validated in-game before finalizing
