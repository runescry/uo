# Predictive Kiting Optimization - Current State

**Date:** 2025-01-27  
**Status:** Running 1,000 simulation test with tunnel layout (20 tiles length, 3 tiles width)

## Overview

We're optimizing the mage sidekick's kiting AI using large-scale simulations (target: 100,000 simulations) to find optimal parameters for predictive kiting. The goal is to emulate human PvP/PvE player behavior with dynamic predictive kiting that considers:
- Aggressor distance
- Spell cast time
- Spell release range
- Healing/retreat decisions

**Primary Success Metric:** Victory rate (% of simulations where enemy is defeated)

## Current Issues

### 1. Spells Not Completing / No Damage Applied
**Status:** FIXED ✅

**Symptoms:**
- Simulations show only 1.0 spell cast per simulation (on average)
- 0% victory rate across all parameter sets
- 0% death rate (simulations run to completion without either side winning)
- Spells are being **started** (CAST_SPELL action is taken) but not completing

**Root Cause:**
- **BUG FOUND:** In `execute_action()`, when a spell was cast, it was set to `is_casting = True`, then immediately in the same tick, the spell state progression logic would see `is_casting = True` and move it to `is_sequencing = True`
- This meant spells never actually stayed in the casting phase for a full tick
- The spell would then be in sequencing, but on the next tick, if the spell wasn't already sequencing before the action, it wouldn't progress to damage application

**Fix Applied:**
- Modified spell state progression to only advance if the spell was **already** in that state before the current action
- Added check: `was_casting_before = (state_before.current_spell and state_before.current_spell.is_casting)`
- Now spells properly progress: Cast (tick 1) → Casting (tick 2) → Sequencing (tick 3) → Damage (tick 4)
- Added `debug_mode` parameter to `MageCombatSimulator.__init__()` to enable debug output
- **FIXED COOLDOWN BUG:** Switched from `datetime.now()`-based cooldowns to tick-based cooldowns (cooldowns now work correctly in simulation)
- **UPDATED MAP SIZES:** 
  - Open area: 1000x1000 tiles (was 40x40)
  - Courtyard: 50x50 inner area with 3-tile wide walls (was 20x20)

**Status:**
- ✅ Spell completion verified - spells now complete and deal damage correctly
- ✅ Cooldown system fixed - multiple spells can be cast per simulation
- ✅ Test results: 5 spells cast, all completed, victory achieved in 23 ticks
- 🚀 **Running full 100,000 simulation optimization** (100 parameter sets × 1000 simulations each)

### 2. Simulation Architecture

**Status:** WORKING

**Components:**
- `predictive_kiting_optimizer.py`: Main optimization script
  - Randomizes 16 parameters across defined ranges
  - Runs 1000 simulations per parameter set (configurable)
  - Tracks victory rate, survival time, distance maintained, spells cast
  - Saves results to JSON for analysis

- `mage_combat_simulator.py`: Core simulation engine
  - Implements predictive kiting logic
  - Handles spell casting, sequencing, and damage application
  - Manages enemy movement and damage
  - Supports two map types: 
    - **Courtyard**: Continuous square tunnel loop (250 tiles per side, 10 tiles wide)
      - Realistic blocking pattern: Irregular blocks (2-6 tiles) spaced ~20 tiles apart
      - Forces navigation around obstacles
      - Total map: ~260x260 tiles
    - **Open Area**: 1000x1000 open space (no obstacles)

**Parameter Ranges Being Tested:**
- `MIN_SAFE_RANGE`: 12-18 tiles
- `MAX_CAST_RANGE`: 20-30 tiles
- `OPTIMAL_CAST_RANGE`: 18-25 tiles
- `SPELL_RELEASE_RANGE_MIN`: 6-10 tiles
- `SPELL_RELEASE_RANGE_MAX`: 8-12 tiles
- `MEDITATION_FLEE_MIN`: 25-35 tiles
- `MEDITATION_FLEE_MAX`: 35-45 tiles
- `HEAL_SAFE_DISTANCE`: 15-25 tiles
- Health/Mana thresholds, stuck detection, enemy speed, cast times

## What's Working

1. **Parameter Randomization:** Correctly generates random parameter sets
2. **Simulation Loop:** Runs for full duration (500 ticks) without crashing
3. **Spell Initiation:** Spells are being started when conditions are met
4. **Distance Tracking:** Average distance is being calculated correctly (~23-24 tiles)
5. **Action System:** Actions are being executed and recorded
6. **Enemy Movement:** Enemy is moving toward sidekick
7. **State Management:** Combat state is being tracked correctly

## What's Not Working

1. ~~**Spell Completion:** Spells start but may not complete their sequencing phase~~ ✅ FIXED
2. ~~**Damage Application:** Enemy health is not decreasing (or not enough to cause victory)~~ ✅ FIXED
3. **Victory Conditions:** Need to verify victory rates improve after fix
4. **Combat Balance:** May need to adjust damage values or enemy health after verifying fix works

## Code Structure

### Spell State Progression
```
1. CAST_SPELL action taken (tick N)
   → execute_action() sets current_spell.is_casting = True
   → Spell state progression checks: was_casting_before = False, so no progression
   
2. Next tick (tick N+1): is_casting = True
   → execute_action() checks: was_casting_before = True (from previous tick)
   → Sets is_casting = False, is_sequencing = True
   
3. Next tick (tick N+2): is_sequencing = True
   → execute_action() checks: was_sequencing_before = True (from previous tick)
   → Applies damage, clears current_spell

FIX: Only progress spell state if it was ALREADY in that state before the action
     (prevents immediate progression when spell is first cast)
```

### Predictive Kiting Logic
- `should_start_casting()`: Checks if predicted distance after cast time will be safe
  - Currently allows casting if `predicted_distance >= spell_release_range_min`
  - Intent: Cast from safe distance (15-25 tiles), move closer during sequencing to release (8-11 tiles)
  
- `handle_offensive()`: Main offensive decision logic
  - Checks if spell is casting → stay in place
  - Checks if spell is sequencing → move to release range (8-11 tiles)
  - Otherwise: Check distance, predict enemy position, decide to cast/retreat

## Test Results (Last Run)

**Test Configuration:**
- 100 simulations (10 per parameter set)
- 10 parameter sets
- 500 ticks per simulation

**Results:**
- Victory Rate: 0.00% (all parameter sets)
- Death Rate: 0.00% (all parameter sets)
- Avg Survival: 500.0 ticks (all simulations ran to completion)
- Avg Distance: 23.6-23.9 tiles
- Avg Spells: 1.0 per simulation

**Interpretation:**
- Sidekick is maintaining good distance (~24 tiles)
- Only 1 spell is being cast per simulation (should be many more)
- No victories or deaths suggests combat is not progressing (spells not dealing damage)

## Next Steps

1. **Immediate:**
   - ✅ Fixed spell state progression bug
   - Run test simulation with debug mode to verify fix
   - Verify spells now complete and deal damage correctly
   - Check victory rates improve

2. **Short-term:**
   - Run 1000 simulations to verify fix works across many scenarios
   - Analyze results to identify optimal parameter ranges
   - Verify average spells cast per simulation increases significantly

3. **Medium-term:**
   - Run full 100,000 simulation optimization
   - Analyze results using `analyze_predictive_kiting.py`
   - Apply best parameters to `MageCombatAI.cs`

## Files Modified

- `predictive_kiting_optimizer.py`: Main optimization script
- `mage_combat_simulator.py`: Core simulation engine with predictive kiting
- `analyze_predictive_kiting.py`: Results analysis script (created, not yet used)

## Key Constants

**Spell Damage:**
- Explosion: 40 damage
- Energy Bolt: 25 damage
- Magic Arrow: 15 damage

**Enemy Health:**
- ArcticOgreLord: 200 HP
- Lich: 150 HP
- Ghoul: 100 HP

**Enemy Damage:**
- ArcticOgreLord: 8-12 per hit
- Lich: 10-15 per hit
- Ghoul: 6-10 per hit

## Debugging Notes

- Debug output is limited to first 3-5 occurrences to avoid spam
- Spell counting tracks `CAST_SPELL` actions, not spell state
- Victory/death checks happen after each tick
- Enemy movement and damage are applied each tick
- Spell state progression happens in `execute_action()` at the end of each action

