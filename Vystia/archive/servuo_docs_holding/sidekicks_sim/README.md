# Combat Simulation System Documentation

This documentation covers the Python-based combat simulation system that ports ServUO's core movement, pathfinding, and combat systems for AI behavior optimization.

## Overview

The simulation system extracts real UO map data and implements accurate game mechanics to:
1. Test AI behavior without launching the game server
2. Optimize combat parameters through batch simulation
3. Find optimal values for kiting, healing, and spell casting

## Module Architecture

```
Simulation/
├── Core Data Layer
│   ├── map_data_extractor.py    # Reads .mul files (terrain, statics)
│   └── tiledata_reference.py    # Tile flags (Impassable, Surface, etc.)
│
├── Movement Layer
│   ├── walkability_check.py     # Map.CanFit() port
│   └── movement_check.py        # Movement.CheckMovement() port
│
├── Pathfinding Layer
│   ├── fast_astar.py            # FastAStarAlgorithm port
│   └── sidekick_ai_movement.py  # SidekickAI movement logic
│
├── Combat Mechanics Layer
│   └── combat_mechanics.py      # Spell damage, healing, poison formulas
│
├── Simulation Layer
│   ├── full_combat_simulator.py # Complete combat simulation
│   └── validate_kiting.py       # Movement validation tests
│
└── Optimization Layer
    └── parameter_optimizer.py   # Parameter sweep optimization
```

## Dependency Graph

```
tiledata_reference ─┐
                    ├─► walkability_check ─► movement_check ─► fast_astar
map_data_extractor ─┘                                              │
                                                                   ▼
                                                        sidekick_ai_movement
                                                                   │
                                                                   ▼
                                                      mage_combat_simulator
```

## Test Region

Primary dungeon test area: **(5379, 4) to (5499, 125)**
- 120x121 tiles
- Contains dungeon corridors, tight spaces, corners, dead ends
- Used for all validation and training data collection

## Documentation Index

1. [Map Data Extractor](./map_data_extractor.md) - Reading UO map files
2. [TileData Reference](./tiledata_reference.md) - Tile flag lookups
3. [Walkability Check](./walkability_check.md) - Map.CanFit() implementation
4. [Movement Check](./movement_check.md) - Movement validation
5. [Fast AStar](./fast_astar.md) - Pathfinding algorithm
6. [Sidekick AI Movement](./sidekick_ai_movement.md) - AI movement logic

## Quick Start

```python
from map_data_extractor import MapDataExtractor
from walkability_check import WalkabilityChecker
from movement_check import MovementChecker, Direction
from fast_astar import FastAStar
from sidekick_ai_movement import SidekickMovement

# Initialize all components
extractor = MapDataExtractor(r"C:\Program Files (x86)\Electronic Arts\Ultima Online Classic")
walk_checker = WalkabilityChecker(extractor)
move_checker = MovementChecker(extractor, walk_checker)
pathfinder = FastAStar(move_checker)

# Create AI movement
ai = SidekickMovement(pathfinder, move_checker)
ai.set_position(5400, 50, 33)

# Move toward target within casting range
ai.move_to_target(5420, 70, 38, casting_range=12)

# Run from enemy
ai.run_from(5405, 55)
```

## Running Simulations

### Validate Movement
```bash
# Test kiting and retreat behavior
python validate_kiting.py
```

### Run Combat Simulation
```bash
# Single combat simulation
python full_combat_simulator.py
```

### Parameter Optimization
```bash
# Quick optimization (12 combinations)
python parameter_optimizer.py

# Full optimization (many combinations)
python parameter_optimizer.py full

# Compare two configs head-to-head
python parameter_optimizer.py compare
```

## Key Optimizable Parameters

These are the SidekickAI parameters that can be tuned:

| Parameter | Default | Description |
|-----------|---------|-------------|
| `min_retreat_distance` | 4 | Distance at which to start retreating |
| `casting_range` | 12 | Optimal distance for spell casting |
| `health_heal_threshold` | 62% | HP% to start healing |
| `critical_health_threshold` | 29% | HP% for emergency actions |
| `mana_meditation_threshold` | 27 | Mana to consider meditation |
| `bandage_cooldown` | 10s | Time between bandage attempts |

## ServUO Reference Files

The Python implementations are ports of these C# files:
- `Server/Map.cs` - Map.CanFit()
- `Server/Movement.cs` - Movement.CheckMovement()
- `Server/PathAlgorithms/FastAStarAlgorithm.cs` - Pathfinding
- `Scripts/Services/AISidekicks/SidekickAI.cs` - AI movement
- `Scripts/Spells/Base/Spell.cs` - Spell casting mechanics
- `Scripts/Items/Resource/Bandage.cs` - Bandage healing
