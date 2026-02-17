# Predictive Kiting Optimization

This directory contains the simulation and optimization system for the mage sidekick's predictive kiting AI.

## Directory Structure

```
Simulation/
├── README.md                    # This file - overview and quick start
├── mage_combat_simulator.py     # Core simulation engine
├── predictive_kiting_optimizer.py  # Main optimization script
├── docs/                        # Documentation
│   ├── STRUCTURE.md            # Detailed directory structure
│   ├── CURRENT_STATE.md        # Current project status and issues
│   └── README_DETAILED.md      # Detailed documentation
├── tools/                       # Analysis and utility scripts
│   └── analyze_predictive_kiting.py  # Results analysis tool
└── scripts/                     # Additional utility scripts (reserved)
```

See `docs/STRUCTURE.md` for detailed file descriptions and usage.

## Quick Start

### Running Optimization

```bash
# Run 1000 simulations (10 parameter sets, 100 simulations each)
python predictive_kiting_optimizer.py --simulations 1000 --per-set 100

# Run full 100,000 simulation optimization
python predictive_kiting_optimizer.py --simulations 100000 --per-set 1000
```

### Analyzing Results

```bash
python tools/analyze_predictive_kiting.py predictive_kiting_results.json
```

## Core Components

### mage_combat_simulator.py
Core simulation engine that models:
- Predictive kiting behavior
- Spell casting, sequencing, and damage application
- Enemy movement and combat
- Two map types: tunnel loop and open area

### predictive_kiting_optimizer.py
Main optimization script that:
- Randomizes 16 parameters across defined ranges
- Runs batch simulations
- Tracks victory rates, survival time, distance, spells cast
- Saves results with rolling 5-copy backup system

## Map Types

### Courtyard (Tunnel Loop)
- Continuous square tunnel loop
- Each side: 20 tiles long
- Tunnel width: 3 tiles
- Realistic blocking pattern: irregular blocks (2-6 tiles) spaced ~20 tiles apart

### Open Area
- 1000x1000 tiles
- No obstacles
- Pure open space for kiting

## Output

Results are saved to `predictive_kiting_results.json` in the project root with:
- Rolling 5-copy backup system (.1, .2, .3, .4, .5)
- Complete parameter sets and simulation results
- Summary statistics per parameter set

## Documentation

- **`docs/STRUCTURE.md`** - Detailed directory structure and file descriptions
- **`docs/CURRENT_STATE.md`** - Current project status, issues, and fixes
- **`docs/QUICK_REFERENCE.md`** - Quick command reference and troubleshooting
- **`docs/README_DETAILED.md`** - Detailed usage and feature documentation

