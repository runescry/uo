# Realistic Mage Combat Simulator with ML Optimization

## Overview

Port ServUO's core systems to Python for realistic simulation, then train a lightweight ML model to optimize pathfinding, movement, walkability checks, and combat decisions using real map data.

## Test Area

**Primary Dungeon Test Region**: (5379, 4) to (5499, 125) - 120x121 tiles

- Use for map data extraction, training data collection, validation testing
- Contains dungeon corridors, tight spaces, corners, dead ends

## Phase 1: Core System Ports

### 1. Map Data Extractor (`map_data_extractor.py`)

Read UO map files (map0.mul, statics0.mul, staidx0.mul) with on-demand coordinate extraction and caching.

- Read terrain tiles (8x8 blocks, 196 bytes per block)
- Read static items (7 bytes per static via index lookup)
- Cache extracted data in memory
- **Primary test area**: Dungeon region (5379, 4) to (5499, 125)

### 2. FastAStar Pathfinding (`fast_astar.py`)

Port exact algorithm from `FastAStarAlgorithm.cs`:

- 38x38 tile search area, MaxDepth=300, 13 height planes
- Heuristic: `(x*11)^2 + (y*11)^2 + z^2`
- Uses `Movement.CheckMovement()` for validation

### 3. Walkability Check (`walkability_check.py`)

Port `Map.CanFit()` logic:

- Check land tile flags (Impassable)
- Check static tiles (surface/impassable, height calculations)
- Check dynamic items and mobiles in sector
- `GetAverageZ()` for terrain height

### 4. Movement Check (`movement_check.py`)

Port `Movement.CheckMovement()`:

- Validate direction movement
- Diagonal movement requires adjacent tiles passable
- Get start Z from current location

### 5. SidekickAI Movement (`sidekick_ai_movement.py`)

Port sidekick's custom movement (NOT standard RunTo which uses melee range):

- `MoveTo(p, run, range)` - maintains casting range (8-20 tiles), NOT melee (1-2 tiles)
- `RunFrom(enemy)` - retreats using PathFollower
- **Critical**: Uses casting ranges (OPTIMAL_CAST_RANGE=20, SPELL_RELEASE_RANGE_MAX=10)

### 6. Simulator Integration (`mage_combat_simulator.py`)

Integrate all systems:

- Replace simple movement with SidekickAI's `MoveTo()` using casting ranges
- Replace simple checks with Map.CanFit()
- Replace simple retreat with SidekickAI's `RunFrom()`
- Use real map data on-demand

### 7. TileData Reference (`tiledata_reference.py`)

Simplified tile flag lookups (Impassable, Surface) for common tiles affecting movement.

## Phase 2: ML Optimization Model

### 8. Training Data Collection (`collect_training_data.py`)

Generate training dataset from simulations:

- **Pathfinding**: (start, goal, map_context) → (path_exists, path_length, path_cost)
- **Walkability**: (x, y, z, map_context) → (can_fit, surface_z)
- **Movement Decisions**: (position, enemy_pos, terrain_features) → (optimal_range, retreat_direction, should_cast)
- **Combat Decisions**: (health, mana, distance, enemy_health, terrain) → (spell_type, cast_timing, positioning_action)

**Data Sources**:

- Run simulations on dungeon test region: (5379, 4) to (5499, 125)
- Extract map context (terrain type, obstacles, enclosed/open space, dungeon corridors)
- Generate diverse scenarios: open corridors, tight spaces, corners, dead ends
- Record all decisions and outcomes, label with success/failure metrics

### 9. ML Model Architecture (`combat_optimizer_model.py`)

Lightweight neural network for real-time predictions:

**Input Features**:

- Positional: (x, y, z, distance_to_enemy, distance_to_obstacles)
- Terrain: (terrain_type, enclosed_space_score, walkability_radius)
- Combat State: (health_pct, mana_pct, enemy_health_pct, spell_cooldown)
- Historical: (recent_movements, recent_spells, stuck_detection)

**Output Predictions**:

- Pathfinding: (path_exists_prob, estimated_path_length, path_cost)
- Walkability: (can_fit_prob, predicted_surface_z)
- Movement: (optimal_range, retreat_direction_8way, should_retreat_prob)
- Combat: (spell_selection, cast_timing_score, positioning_action)

**Architecture**:

- Shared encoder (terrain + positional features)
- Task-specific heads (pathfinding, walkability, movement, combat)
- Lightweight: <10MB model size for fast inference

### 10. Model Training (`train_combat_optimizer.py`)

Offline training pipeline:

- Load collected training data from dungeon region
- Train on map data from multiple regions
- Validate on held-out coordinates
- Optimize for accuracy vs inference speed tradeoff
- Export model for simulator integration

### 11. Simulator ML Integration (`mage_combat_simulator_ml.py`)

Integrate ML model into simulator:

- Use ML predictions to cache/skip expensive operations:
  - Pathfinding: Predict if path exists before running FastAStar
  - Walkability: Predict CanFit before full check
  - Movement: Use predicted optimal range/direction
  - Combat: Use predicted spell selection and timing
- Fallback to full computation if ML confidence is low
- Learn from failures (online adaptation)

## File Structure

```
ServUO/Scripts/Services/AISidekicks/Simulation/
├── Core Systems (Phase 1):
│   ├── map_data_extractor.py
│   ├── fast_astar.py
│   ├── walkability_check.py
│   ├── movement_check.py
│   ├── sidekick_ai_movement.py
│   ├── tiledata_reference.py
│   └── mage_combat_simulator.py (updated)
│
└── ML Optimization (Phase 2):
    ├── collect_training_data.py
    ├── combat_optimizer_model.py
    ├── train_combat_optimizer.py
    └── mage_combat_simulator_ml.py
```

## Success Criteria

**Phase 1**:

- Simulator produces paths matching in-game FastAStar
- Retreat distances match logs (15-25 tiles in open areas)
- Stuck detection triggers at same points
- Movement validation blocks same tiles as in-game
- Test on dungeon region (5379, 4) to (5499, 125)

**Phase 2**:

- ML model predicts pathfinding with >90% accuracy
- ML model predicts walkability with >95% accuracy
- ML model predicts optimal combat decisions with >85% accuracy
- Simulator runs 10-100x faster with ML caching
- Combat performance matches or exceeds non-ML simulator
- Model trained and validated on dungeon region (5379, 4) to (5499, 125)