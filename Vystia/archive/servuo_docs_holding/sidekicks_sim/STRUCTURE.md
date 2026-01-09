# Simulation Directory Structure

## Overview

This directory contains the complete simulation and optimization system for the mage sidekick's predictive kiting AI.

## Directory Layout

```
Simulation/
├── README.md                          # Quick start guide and overview
├── mage_combat_simulator.py           # Core simulation engine
├── predictive_kiting_optimizer.py     # Main optimization script
│
├── docs/                              # Documentation
│   ├── STRUCTURE.md                   # This file - directory structure
│   ├── CURRENT_STATE.md              # Current project status, issues, fixes
│   └── README_DETAILED.md            # Detailed documentation
│
├── tools/                             # Analysis and utility scripts
│   └── analyze_predictive_kiting.py  # Results analysis tool
│
└── scripts/                           # Additional utility scripts (reserved)
```

## File Descriptions

### Core Files (Root Level)

#### `mage_combat_simulator.py`
**Purpose:** Core simulation engine  
**Key Features:**
- Models predictive kiting behavior
- Simulates spell casting, sequencing, and damage application
- Handles enemy movement and combat AI
- Supports two map types: tunnel loop and open area
- Tick-based cooldown system
- Spell state progression (cast → casting → sequencing → damage)

**Usage:**
```python
from mage_combat_simulator import MageCombatSimulator, CombatState

simulator = MageCombatSimulator(initial_state, map_type='courtyard')
result = simulator.run(max_ticks=1000)
```

#### `predictive_kiting_optimizer.py`
**Purpose:** Main optimization script for parameter tuning  
**Key Features:**
- Randomizes 16 AI parameters across defined ranges
- Runs batch simulations with multiple parameter sets
- Tracks victory rates, survival time, distance, spells cast
- Rolling 5-copy backup system for results
- Saves results to JSON in project root

**Usage:**
```bash
# Run 1000 simulations
python predictive_kiting_optimizer.py --simulations 1000 --per-set 100

# Full optimization (100,000 simulations)
python predictive_kiting_optimizer.py --simulations 100000 --per-set 1000
```

### Documentation (`docs/`)

#### `README.md`
Quick start guide with:
- Directory structure overview
- Quick start commands
- Core component descriptions
- Map type descriptions
- Output format

#### `CURRENT_STATE.md`
**Purpose:** Track current project status  
**Contains:**
- Current issues and fixes
- Bug reports and resolutions
- Next steps and pending tasks
- Code structure explanations
- Implementation details

#### `STRUCTURE.md`
This file - detailed directory structure and file descriptions.

#### `README_DETAILED.md`
Detailed documentation covering:
- Full feature list
- Usage examples
- Output format
- Integration with C# code
- Extending the simulator

### Tools (`tools/`)

#### `analyze_predictive_kiting.py`
**Purpose:** Analyze optimization results  
**Features:**
- Reads JSON results from optimizer
- Identifies best parameter sets
- Generates statistics and reports
- Sorts by victory rate and other metrics

**Usage:**
```bash
python tools/analyze_predictive_kiting.py predictive_kiting_results.json
```

### Scripts (`scripts/`)
Reserved for additional utility scripts as needed.

## Import Structure

### From Root Level
Core modules are imported directly:
```python
from mage_combat_simulator import (
    MageCombatSimulator, 
    CombatState, 
    MAP_TYPE_COURTYARD, 
    MAP_TYPE_OPEN_AREA
)
```

### From Tools
Tools import from parent directory:
```python
import sys
import os
sys.path.insert(0, os.path.dirname(os.path.dirname(os.path.abspath(__file__))))
```

## Output Files

### Results
- **Location:** Project root (`D:\UO\`)
- **Filename:** `predictive_kiting_results.json`
- **Backups:** Rolling 5 copies (`.1`, `.2`, `.3`, `.4`, `.5`)

### Format
```json
{
  "timestamp": "2025-01-27T...",
  "total_simulations": 1000,
  "simulations_per_set": 100,
  "duration_seconds": 123.45,
  "results": [
    {
      "parameters": { ... },
      "victory_rate": 0.75,
      "avg_survival_ticks": 450,
      ...
    }
  ]
}
```

## Map Types

### Courtyard (Tunnel Loop)
- **Type:** Continuous square tunnel loop
- **Side Length:** 20 tiles per side
- **Tunnel Width:** 3 tiles
- **Blocking Pattern:** Irregular blocks (2-6 tiles) spaced ~20 tiles apart
- **Block Percentage:** ~15.9% of tunnel blocked
- **Corners:** Always passable

### Open Area
- **Type:** Pure open space
- **Size:** 1000x1000 tiles
- **Obstacles:** None

## Workflow

1. **Run Optimization**
   ```bash
   python predictive_kiting_optimizer.py --simulations 1000 --per-set 100
   ```

2. **Analyze Results**
   ```bash
   python tools/analyze_predictive_kiting.py predictive_kiting_results.json
   ```

3. **Review Documentation**
   - Check `docs/CURRENT_STATE.md` for latest status
   - Review `docs/README_DETAILED.md` for detailed usage

4. **Update C# Code**
   - Apply optimal parameters to `MageCombatAI.cs`
   - Test in-game
   - Iterate as needed

## Maintenance

### Adding New Tools
1. Place in `tools/` directory
2. Update this file with description
3. Update root `README.md` if it's a main tool

### Adding Documentation
1. Place in `docs/` directory
2. Update this file
3. Link from root `README.md` if relevant

### Adding Scripts
1. Place in `scripts/` directory
2. Update this file with description

## Notes

- All Python scripts use Python 3
- Results are saved to project root, not simulation directory
- Backup system automatically manages result file versions
- Core simulator is standalone and can be used independently

