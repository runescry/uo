# Quick Reference Guide

## Common Commands

### Run Optimization
```bash
# From project root (D:\UO)
cd ServUO\Scripts\Services\AISidekicks\Simulation

# Quick test (100 simulations)
python predictive_kiting_optimizer.py --simulations 100 --per-set 10

# Standard run (1000 simulations)
python predictive_kiting_optimizer.py --simulations 1000 --per-set 100

# Full optimization (100,000 simulations)
python predictive_kiting_optimizer.py --simulations 100000 --per-set 1000
```

### Analyze Results
```bash
# From project root
python ServUO\Scripts\Services\AISidekicks\Simulation\tools\analyze_predictive_kiting.py predictive_kiting_results.json
```

### Check Current Status
```bash
# View current state document
cat ServUO\Scripts\Services\AISidekicks\Simulation\docs\CURRENT_STATE.md
```

## File Locations

### Results
- **Location:** `D:\UO\predictive_kiting_results.json`
- **Backups:** `predictive_kiting_results.json.1` through `.5` (rolling)

### Core Scripts
- **Simulator:** `Simulation/mage_combat_simulator.py`
- **Optimizer:** `Simulation/predictive_kiting_optimizer.py`
- **Analyzer:** `Simulation/tools/analyze_predictive_kiting.py`

### Documentation
- **Quick Start:** `Simulation/README.md`
- **Structure:** `Simulation/docs/STRUCTURE.md`
- **Current State:** `Simulation/docs/CURRENT_STATE.md`
- **Detailed Docs:** `Simulation/docs/README_DETAILED.md`

## Parameter Ranges

The optimizer randomizes these 16 parameters:

1. `MIN_SAFE_RANGE` - 6.0 to 8.0 tiles
2. `MAX_SAFE_RANGE` - 10.0 to 12.0 tiles
3. `SPELL_RELEASE_RANGE_MIN` - 6.0 to 8.0 tiles
4. `SPELL_RELEASE_RANGE_MAX` - 10.0 to 12.0 tiles
5. `RETREAT_DISTANCE` - 3.0 to 5.0 tiles
6. `LOW_MANA_THRESHOLD` - 20.0 to 40.0
7. `LOW_HEALTH_THRESHOLD` - 30.0 to 50.0
8. `CRITICAL_HEALTH_THRESHOLD` - 15.0 to 25.0
9. `MEDITATION_SPRINT_TICKS` - 3 to 7 ticks
10. `HEAL_BANDAGE_THRESHOLD` - 50.0 to 70.0
11. `PREDICTIVE_KITING_ENABLED` - True/False
12. `PREDICTIVE_KITING_DISTANCE` - 8.0 to 12.0 tiles
13. `PREDICTIVE_KITING_LOOKAHEAD` - 2.0 to 4.0 ticks
14. `SPELL_COOLDOWN` - 2.0 to 3.0 seconds
15. `ENEMY_SPEED` - 0.8 to 1.2 tiles/tick
16. `SIDEKICK_SPEED` - 0.8 to 1.2 tiles/tick

## Map Types

### Courtyard (Tunnel Loop)
- **Code:** `MAP_TYPE_COURTYARD` or `'courtyard'`
- **Layout:** Continuous square tunnel loop
- **Dimensions:** 20 tiles per side, 3 tiles wide
- **Blocking:** ~15.9% blocked with irregular pattern

### Open Area
- **Code:** `MAP_TYPE_OPEN_AREA` or `'open_area'`
- **Layout:** Pure open space
- **Dimensions:** 1000x1000 tiles
- **Blocking:** None

## Troubleshooting

### Import Errors
If you get import errors when running tools:
```bash
# Make sure you're in the Simulation directory or project root
cd D:\UO\ServUO\Scripts\Services\AISidekicks\Simulation
```

### Results Not Found
Results are saved to project root, not simulation directory:
```bash
# Check project root
ls D:\UO\predictive_kiting_results.json
```

### Background Process
To check if optimization is still running:
```powershell
Get-Process python | Where-Object {$_.CommandLine -like "*predictive_kiting_optimizer*"}
```

## Next Steps After Optimization

1. **Analyze Results**
   ```bash
   python tools/analyze_predictive_kiting.py predictive_kiting_results.json
   ```

2. **Review Top Parameters**
   - Check top 10 parameter sets
   - Note parameter sensitivity analysis

3. **Update C# Code**
   - Apply optimal parameters to `MageCombatAI.cs`
   - Test in-game

4. **Iterate**
   - Run more simulations if needed
   - Adjust parameter ranges based on results

