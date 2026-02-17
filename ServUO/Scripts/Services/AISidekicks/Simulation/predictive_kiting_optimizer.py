#!/usr/bin/env python3
"""
Predictive Kiting Optimizer

Runs 100,000 simulations with randomized parameters to optimize victory rate.
Primary metric: Victory rate (% of simulations where enemy defeated)
Secondary metrics: Survival time, distance maintained, spells cast
"""

import sys
import json
import random
import math
from typing import Dict, List, Tuple
from datetime import datetime
from mage_combat_simulator import (
    MageCombatSimulator, CombatState, MAP_TYPE_COURTYARD, MAP_TYPE_OPEN_AREA
)

# Parameter ranges to randomize
PARAMETER_RANGES = {
    'MIN_SAFE_RANGE': (12, 18),
    'MAX_CAST_RANGE': (20, 30),
    'OPTIMAL_CAST_RANGE': (18, 25),
    'SPELL_RELEASE_RANGE_MIN': (6, 10),
    'SPELL_RELEASE_RANGE_MAX': (8, 12),
    'MEDITATION_FLEE_MIN': (25, 35),
    'MEDITATION_FLEE_MAX': (35, 45),
    'HEAL_SAFE_DISTANCE': (15, 25),
    'CRITICAL_HEALTH': (0.25, 0.45),
    'LOW_HEALTH': (0.45, 0.70),
    'LOW_MANA': (15, 30),
    'CRITICAL_MANA': (8, 18),
    'STUCK_THRESHOLD': (1, 4),
    'ENEMY_MOVE_SPEED': (0.8, 1.2),  # tiles per tick
    'CAST_TIME_EXPLOSION': (1.8, 2.2),
    'CAST_TIME_ENERGY_BOLT': (1.3, 1.7),
}

# Enemy types for testing
# REALISTIC damage values based on actual UO mob stats:
# - Arctic Ogre Lord: High damage melee mob, hits for 25-40+ per swing
# - Lich: Caster mob with moderate melee, hits for 15-25
# - Ghoul: Weak undead, hits for 10-15
ENEMY_TYPES = ['ArcticOgreLord', 'Lich', 'Ghoul']
ENEMY_HEALTH = {
    'ArcticOgreLord': 200,
    'Lich': 150,
    'Ghoul': 100,
}
ENEMY_DAMAGE_RANGE = {
    'ArcticOgreLord': (25, 40),  # REALISTIC: High damage melee mob
    'Lich': (15, 25),             # Moderate damage
    'Ghoul': (10, 15),            # Low damage
}
# Enemy melee range - enemy can only hit when within this distance
ENEMY_MELEE_RANGE = 2  # Melee attacks require being close


def generate_random_parameters() -> Dict:
    """Generate a random parameter set within the specified ranges"""
    params = {}
    for param_name, (min_val, max_val) in PARAMETER_RANGES.items():
        if isinstance(min_val, float) or isinstance(max_val, float):
            params[param_name] = round(random.uniform(min_val, max_val), 2)
        else:
            params[param_name] = random.randint(min_val, max_val)
    return params


def create_initial_state(params: Dict, enemy_type: str, start_distance: int, map_type: str) -> CombatState:
    """Create initial combat state for simulation"""
    from mage_combat_simulator import COURTYARD_BOUNDS, OPEN_AREA_BOUNDS, TUNNEL_SIDE_LENGTH, TUNNEL_WIDTH
    
    # Random starting positions
    if map_type == MAP_TYPE_COURTYARD:
        # Start in tunnel (along perimeter of square)
        tunnel_outer = TUNNEL_SIDE_LENGTH // 2
        tunnel_inner = tunnel_outer - TUNNEL_WIDTH
        
        # Randomly choose which tunnel side to start in
        side = random.choice(['north', 'south', 'east', 'west'])
        if side == 'north':
            start_x = random.randint(-tunnel_outer, tunnel_outer)
            start_y = random.randint(tunnel_inner, tunnel_outer)
        elif side == 'south':
            start_x = random.randint(-tunnel_outer, tunnel_outer)
            start_y = random.randint(-tunnel_outer, -tunnel_inner)
        elif side == 'east':
            start_x = random.randint(tunnel_inner, tunnel_outer)
            start_y = random.randint(-tunnel_outer, tunnel_outer)
        else:  # west
            start_x = random.randint(-tunnel_outer, -tunnel_inner)
            start_y = random.randint(-tunnel_outer, tunnel_outer)
        
        # Enemy at start_distance away (within tunnel)
        angle = random.uniform(0, 2 * math.pi)
        enemy_x = int(start_x + start_distance * math.cos(angle))
        enemy_y = int(start_y + start_distance * math.sin(angle))
        
        # Clamp to tunnel bounds
        min_x, min_y, max_x, max_y = COURTYARD_BOUNDS
        enemy_x = max(min_x, min(max_x, enemy_x))
        enemy_y = max(min_y, min(max_y, enemy_y))
    else:  # Open area (with realistic obstacles in combat zone)
        # Combat typically happens in a smaller area with obstacles
        # Use the combat area (-50 to 50) where obstacles are generated
        combat_min, combat_max = -40, 40  # Slightly inside the obstacle area
        
        # 30% chance to start near an edge (more realistic - sidekicks often get cornered at edges)
        if random.random() < 0.3:
            # Start near an edge
            edge = random.choice(['north', 'south', 'east', 'west'])
            if edge == 'north':
                start_x = random.randint(combat_min, combat_max)
                start_y = random.randint(combat_max - 10, combat_max)
            elif edge == 'south':
                start_x = random.randint(combat_min, combat_max)
                start_y = random.randint(combat_min, combat_min + 10)
            elif edge == 'east':
                start_x = random.randint(combat_max - 10, combat_max)
                start_y = random.randint(combat_min, combat_max)
            else:  # west
                start_x = random.randint(combat_min, combat_min + 10)
                start_y = random.randint(combat_min, combat_max)
        else:
            # Random position in combat area
            start_x = random.randint(combat_min, combat_max)
            start_y = random.randint(combat_min, combat_max)
        
        # Enemy at start_distance away
        angle = random.uniform(0, 2 * math.pi)
        enemy_x = int(start_x + start_distance * math.cos(angle))
        enemy_y = int(start_y + start_distance * math.sin(angle))
        # Clamp to combat area bounds
        enemy_x = max(combat_min, min(combat_max, enemy_x))
        enemy_y = max(combat_min, min(combat_max, enemy_y))
    
    state = CombatState(
        health=100,
        health_max=100,
        mana=100,
        mana_max=100,
        position=(start_x, start_y),
        enemy_position=(enemy_x, enemy_y),
        distance=start_distance,
        enemy_health=ENEMY_HEALTH[enemy_type],
        enemy_health_max=ENEMY_HEALTH[enemy_type],
    )
    # Store enemy type for damage calculation
    state.enemy_type = enemy_type
    return state


def run_simulation(params: Dict, enemy_type: str, start_distance: int, map_type: str, max_ticks: int = 500) -> Dict:
    """Run a single simulation and return results"""
    initial_state = create_initial_state(params, enemy_type, start_distance, map_type)
    
    # Create simulator with parameter overrides
    simulator = MageCombatSimulator(initial_state, param_overrides=params, map_type=map_type)
    
    # Run simulation
    victory = False
    death = False
    ticks_survived = max_ticks  # Default to max if no victory/death
    total_distance = 0
    distance_samples = 0
    spells_cast = 0
    
    for tick in range(max_ticks):
        # Update metrics before simulating (to capture initial state)
        total_distance += simulator.state.distance
        distance_samples += 1
        
        # Simulate one tick
        try:
            action = simulator.simulate_tick()
            
            # Check if spell was started this tick (action type is CAST_SPELL)
            from mage_combat_simulator import ActionType
            if action and action.action_type == ActionType.CAST_SPELL:
                spells_cast += 1
        except Exception as e:
            # If simulation fails, log and break
            print(f"Simulation error at tick {tick}: {e}")
            import traceback
            traceback.print_exc()
            ticks_survived = tick
            break
        
        # Check victory condition after simulating
        if simulator.state.enemy_health <= 0:
            victory = True
            ticks_survived = tick + 1  # +1 because we completed this tick
            break
        
        # Check death condition after simulating
        if simulator.state.health <= 0:
            death = True
            ticks_survived = tick + 1  # +1 because we completed this tick
            break
    
    # Calculate average distance
    avg_distance = total_distance / distance_samples if distance_samples > 0 else 0
    
    return {
        'victory': victory,
        'death': death,
        'ticks_survived': ticks_survived,
        'avg_distance': avg_distance,
        'spells_cast': spells_cast,
        'final_health': simulator.state.health,
        'final_mana': simulator.state.mana,
        'final_enemy_health': simulator.state.enemy_health,
    }


def run_batch_simulations(num_simulations: int = 100000, simulations_per_set: int = 1000) -> List[Dict]:
    """Run batch simulations with randomized parameters"""
    num_parameter_sets = num_simulations // simulations_per_set
    results = []
    
    print(f"Running {num_simulations} simulations across {num_parameter_sets} parameter sets...")
    print(f"Simulations per set: {simulations_per_set}")
    
    for set_idx in range(num_parameter_sets):
        params = generate_random_parameters()
        
        print(f"\nParameter Set {set_idx + 1}/{num_parameter_sets}")
        print(f"Parameters: {json.dumps(params, indent=2)}")
        
        set_results = {
            'parameter_set': set_idx + 1,
            'parameters': params,
            'simulations': [],
            'summary': {}
        }
        
        victories = 0
        deaths = 0
        total_ticks = 0
        total_distance = 0.0
        total_spells = 0
        
        # Run simulations for this parameter set
        for sim_idx in range(simulations_per_set):
            # Randomize scenario with REALISTIC starting conditions
            # Based on log analysis: combat often starts at close range (5-10 tiles)
            enemy_type = random.choice(ENEMY_TYPES)
            # More realistic start distances: 40% close (5-10), 40% medium (10-15), 20% far (15-25)
            distance_roll = random.random()
            if distance_roll < 0.4:
                start_distance = random.randint(5, 10)  # Close range (common in-game)
            elif distance_roll < 0.8:
                start_distance = random.randint(10, 15)  # Medium range
            else:
                start_distance = random.randint(15, 25)  # Far range (less common)
            map_type = MAP_TYPE_COURTYARD  # Courtyard (tunnel) only
            
            result = run_simulation(params, enemy_type, start_distance, map_type)
            set_results['simulations'].append(result)
            
            if result['victory']:
                victories += 1
            if result['death']:
                deaths += 1
            
            total_ticks += result['ticks_survived']
            total_distance += result['avg_distance']
            total_spells += result['spells_cast']
            
            if (sim_idx + 1) % 100 == 0:
                print(f"  Completed {sim_idx + 1}/{simulations_per_set} simulations...")
        
        # Calculate summary statistics
        set_results['summary'] = {
            'victory_rate': victories / simulations_per_set,
            'death_rate': deaths / simulations_per_set,
            'avg_survival_ticks': total_ticks / simulations_per_set,
            'avg_distance': total_distance / simulations_per_set,
            'avg_spells_cast': total_spells / simulations_per_set,
        }
        
        print(f"  Victory Rate: {set_results['summary']['victory_rate']:.2%}")
        print(f"  Death Rate: {set_results['summary']['death_rate']:.2%}")
        print(f"  Avg Survival: {set_results['summary']['avg_survival_ticks']:.1f} ticks")
        print(f"  Avg Distance: {set_results['summary']['avg_distance']:.1f} tiles")
        print(f"  Avg Spells: {set_results['summary']['avg_spells_cast']:.1f}")
        
        results.append(set_results)
    
    return results


def main():
    """Main entry point"""
    import argparse
    
    parser = argparse.ArgumentParser(description='Optimize predictive kiting parameters')
    parser.add_argument('--simulations', type=int, default=100000, help='Total number of simulations')
    parser.add_argument('--per-set', type=int, default=1000, help='Simulations per parameter set')
    parser.add_argument('--output', type=str, default='results/predictive_kiting_results.json', help='Output file (relative to Simulation directory)')
    
    args = parser.parse_args()
    
    print("=" * 80)
    print("Predictive Kiting Optimizer")
    print("=" * 80)
    print(f"Total simulations: {args.simulations}")
    print(f"Simulations per set: {args.per_set}")
    print(f"Parameter sets: {args.simulations // args.per_set}")
    print("=" * 80)
    
    start_time = datetime.now()
    results = run_batch_simulations(args.simulations, args.per_set)
    end_time = datetime.now()
    
    # Manage rolling backups (keep only 5 most recent copies)
    import os
    MAX_BACKUPS = 5
    
    # Ensure output directory exists
    output_dir = os.path.dirname(args.output)
    if output_dir and not os.path.exists(output_dir):
        os.makedirs(output_dir)
    
    # Rotate existing backups (newest becomes .1, .1 becomes .2, etc.)
    for i in range(MAX_BACKUPS - 1, 0, -1):
        old_backup = f"{args.output}.{i}"
        new_backup = f"{args.output}.{i + 1}"
        if os.path.exists(old_backup):
            if os.path.exists(new_backup):
                os.remove(new_backup)  # Remove oldest backup if it exists
            os.rename(old_backup, new_backup)
    
    # Move current file to .1 if it exists
    if os.path.exists(args.output):
        backup_name = f"{args.output}.1"
        if os.path.exists(backup_name):
            os.remove(backup_name)
        os.rename(args.output, backup_name)
    
    # Save new results
    with open(args.output, 'w') as f:
        json.dump({
            'timestamp': datetime.now().isoformat(),
            'total_simulations': args.simulations,
            'simulations_per_set': args.per_set,
            'duration_seconds': (end_time - start_time).total_seconds(),
            'results': results
        }, f, indent=2)
    
    print(f"\nResults saved to {args.output}")
    print(f"Total duration: {(end_time - start_time).total_seconds():.1f} seconds")
    
    # Print top 10 parameter sets by victory rate
    sorted_results = sorted(results, key=lambda x: x['summary']['victory_rate'], reverse=True)
    print("\n" + "=" * 80)
    print("Top 10 Parameter Sets by Victory Rate:")
    print("=" * 80)
    for i, result in enumerate(sorted_results[:10], 1):
        print(f"\n{i}. Parameter Set {result['parameter_set']}")
        print(f"   Victory Rate: {result['summary']['victory_rate']:.2%}")
        print(f"   Death Rate: {result['summary']['death_rate']:.2%}")
        print(f"   Avg Survival: {result['summary']['avg_survival_ticks']:.1f} ticks")
        print(f"   Avg Distance: {result['summary']['avg_distance']:.1f} tiles")
        print(f"   Avg Spells: {result['summary']['avg_spells_cast']:.1f}")
        print(f"   Parameters:")
        for key, value in result['parameters'].items():
            print(f"     {key}: {value}")


if __name__ == '__main__':
    main()

