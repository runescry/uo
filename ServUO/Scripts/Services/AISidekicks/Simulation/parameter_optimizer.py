#!/usr/bin/env python3
"""
Parameter Optimizer

Automatically tests different AI parameter combinations to find optimal values.
Uses batch simulation to evaluate each configuration.
"""

import json
import itertools
from dataclasses import dataclass, asdict
from typing import List, Dict, Any, Tuple
from datetime import datetime
import os

from full_combat_simulator import (
    FullCombatSimulator, SimulationConfig, CombatStats,
    run_batch_simulation, EnemyType
)


@dataclass
class ParameterRange:
    """Define a parameter to sweep"""
    name: str
    values: List[Any]


@dataclass
class OptimizationResult:
    """Result of parameter optimization"""
    best_config: Dict
    best_win_rate: float
    best_score: float
    all_results: List[Dict]
    timestamp: str


class ParameterOptimizer:
    """
    Sweeps through parameter combinations to find optimal values.
    """

    def __init__(self, base_config: SimulationConfig = None,
                 simulations_per_config: int = 50):
        self.base_config = base_config or SimulationConfig()
        self.simulations_per_config = simulations_per_config
        self.results: List[Dict] = []

    def optimize(self, parameters: List[ParameterRange],
                  verbose: bool = True) -> OptimizationResult:
        """
        Run parameter sweep and find best configuration.

        Args:
            parameters: List of ParameterRange objects defining what to test
            verbose: Print progress

        Returns:
            OptimizationResult with best configuration
        """
        # Generate all combinations
        param_names = [p.name for p in parameters]
        param_values = [p.values for p in parameters]
        combinations = list(itertools.product(*param_values))

        total = len(combinations)
        if verbose:
            print(f"\n{'='*70}")
            print(f"PARAMETER OPTIMIZATION")
            print(f"{'='*70}")
            print(f"Testing {total} parameter combinations")
            print(f"Simulations per config: {self.simulations_per_config}")
            print(f"Total simulations: {total * self.simulations_per_config}")
            print(f"{'='*70}\n")

        best_result = None
        best_score = -float('inf')

        for i, combo in enumerate(combinations):
            # Create config with this combination
            config_dict = asdict(self.base_config)
            for name, value in zip(param_names, combo):
                config_dict[name] = value

            config = SimulationConfig(**config_dict)

            # Run batch simulation
            if verbose:
                param_str = ", ".join(f"{n}={v}" for n, v in zip(param_names, combo))
                print(f"[{i+1}/{total}] Testing: {param_str}")

            result = run_batch_simulation(config, self.simulations_per_config, verbose=False)

            # Calculate score (weighted metrics)
            score = self._calculate_score(result)
            result['score'] = score
            result['param_combo'] = dict(zip(param_names, combo))

            self.results.append(result)

            if score > best_score:
                best_score = score
                best_result = result

            if verbose:
                print(f"    Win rate: {result['win_rate']*100:.1f}%, "
                      f"Avg dmg: {result['avg_damage_dealt']:.0f}, "
                      f"Score: {score:.2f}")

        if verbose:
            print(f"\n{'='*70}")
            print(f"OPTIMIZATION COMPLETE")
            print(f"{'='*70}")
            print(f"\nBest configuration:")
            for name, value in best_result['param_combo'].items():
                print(f"  {name}: {value}")
            print(f"\nBest win rate: {best_result['win_rate']*100:.1f}%")
            print(f"Best score: {best_score:.2f}")

        return OptimizationResult(
            best_config=best_result['config'],
            best_win_rate=best_result['win_rate'],
            best_score=best_score,
            all_results=self.results,
            timestamp=datetime.now().isoformat(),
        )

    def _calculate_score(self, result: Dict) -> float:
        """
        Calculate composite score from simulation results.
        Higher is better.
        """
        # Weights for different metrics
        WIN_WEIGHT = 100.0      # Win rate is most important
        DAMAGE_WEIGHT = 0.1    # Damage dealt
        SURVIVAL_WEIGHT = -0.2  # Damage taken (negative = less is better)
        EFFICIENCY_WEIGHT = 2.0  # Spells per fight (efficiency)
        CORNER_PENALTY = -5.0   # Getting cornered is bad

        score = (
            result['win_rate'] * WIN_WEIGHT +
            result['avg_damage_dealt'] * DAMAGE_WEIGHT +
            result['avg_damage_taken'] * SURVIVAL_WEIGHT +
            result['avg_spells_cast'] * EFFICIENCY_WEIGHT +
            result['avg_times_cornered'] * CORNER_PENALTY
        )

        return score

    def save_results(self, filepath: str):
        """Save optimization results to JSON"""
        with open(filepath, 'w') as f:
            json.dump(self.results, f, indent=2, default=str)

    def generate_report(self) -> str:
        """Generate human-readable optimization report"""
        if not self.results:
            return "No results to report"

        # Sort by score
        sorted_results = sorted(self.results, key=lambda x: x['score'], reverse=True)

        lines = [
            "=" * 70,
            "PARAMETER OPTIMIZATION REPORT",
            "=" * 70,
            "",
            f"Total configurations tested: {len(self.results)}",
            f"Simulations per config: {self.simulations_per_config}",
            "",
            "-" * 70,
            "TOP 5 CONFIGURATIONS",
            "-" * 70,
        ]

        for i, result in enumerate(sorted_results[:5]):
            lines.append(f"\n#{i+1}:")
            for name, value in result['param_combo'].items():
                lines.append(f"  {name}: {value}")
            lines.append(f"  Win rate: {result['win_rate']*100:.1f}%")
            lines.append(f"  Avg damage dealt: {result['avg_damage_dealt']:.0f}")
            lines.append(f"  Avg damage taken: {result['avg_damage_taken']:.0f}")
            lines.append(f"  Avg times cornered: {result['avg_times_cornered']:.1f}")
            lines.append(f"  Score: {result['score']:.2f}")

        lines.extend([
            "",
            "-" * 70,
            "WORST 3 CONFIGURATIONS",
            "-" * 70,
        ])

        for i, result in enumerate(sorted_results[-3:]):
            lines.append(f"\n#{len(sorted_results)-2+i}:")
            for name, value in result['param_combo'].items():
                lines.append(f"  {name}: {value}")
            lines.append(f"  Win rate: {result['win_rate']*100:.1f}%")
            lines.append(f"  Score: {result['score']:.2f}")

        return "\n".join(lines)


def quick_optimization():
    """Quick optimization with limited parameter ranges"""
    print("Running quick parameter optimization...")

    optimizer = ParameterOptimizer(
        base_config=SimulationConfig(max_ticks=300),
        simulations_per_config=20  # Fewer sims for speed
    )

    # Parameters to test
    parameters = [
        ParameterRange("min_retreat_distance", [3, 4, 5, 6]),
        ParameterRange("casting_range", [10, 12, 14]),
    ]

    result = optimizer.optimize(parameters)

    print("\n" + optimizer.generate_report())

    return result


def full_optimization():
    """Full optimization with more parameter combinations"""
    print("Running full parameter optimization...")
    print("(This will take a while...)")

    optimizer = ParameterOptimizer(
        base_config=SimulationConfig(max_ticks=500),
        simulations_per_config=50
    )

    # More parameters to test
    parameters = [
        ParameterRange("min_retreat_distance", [3, 4, 5, 6]),
        ParameterRange("casting_range", [8, 10, 12, 14, 16]),
        ParameterRange("health_heal_threshold", [0.50, 0.60, 0.70]),
        ParameterRange("critical_health_threshold", [0.25, 0.30, 0.35]),
    ]

    result = optimizer.optimize(parameters)

    # Save results
    output_dir = os.path.dirname(os.path.abspath(__file__))
    output_file = os.path.join(output_dir, "optimization_results.json")
    optimizer.save_results(output_file)
    print(f"\nResults saved to: {output_file}")

    print("\n" + optimizer.generate_report())

    return result


def dual_enemy_optimization():
    """Optimization that tests against BOTH Ogre Lord and Lich"""
    print("Running dual-enemy parameter optimization...")
    print("Testing against both Ogre Lord (melee) and Lich (mage)\n")

    # Test parameters
    parameters = [
        ParameterRange("min_retreat_distance", [3, 4, 5, 6]),
        ParameterRange("casting_range", [10, 12, 14]),
        ParameterRange("health_heal_threshold", [0.50, 0.62, 0.70]),
    ]

    param_names = [p.name for p in parameters]
    param_values = [p.values for p in parameters]
    combinations = list(itertools.product(*param_values))

    simulations_per_config = 15  # 15 each against 2 enemies = 30 total per config
    total_configs = len(combinations)

    print(f"Testing {total_configs} configurations")
    print(f"Simulations per config per enemy: {simulations_per_config}")
    print(f"Total simulations: {total_configs * simulations_per_config * 2}")
    print("=" * 70)

    results = []
    best_result = None
    best_combined_score = -float('inf')

    for i, combo in enumerate(combinations):
        # Create config
        config_dict = {
            "max_ticks": 400,
        }
        for name, value in zip(param_names, combo):
            config_dict[name] = value

        param_str = ", ".join(f"{n}={v}" for n, v in zip(param_names, combo))
        print(f"\n[{i+1}/{total_configs}] Testing: {param_str}")

        # Test vs Ogre Lord
        ogre_config = SimulationConfig(**config_dict, enemy_type=EnemyType.OgreLord)
        ogre_result = run_batch_simulation(ogre_config, simulations_per_config, verbose=False)

        # Test vs Lich
        lich_config = SimulationConfig(**config_dict, enemy_type=EnemyType.Lich)
        lich_result = run_batch_simulation(lich_config, simulations_per_config, verbose=False)

        # Combined score (weighted - Lich is harder, worth more)
        # Ogre Lord is easy to kite, so we weight damage taken more than win rate
        ogre_score = (
            ogre_result['win_rate'] * 30 -       # Win rate
            ogre_result['avg_damage_taken'] * 0.5 -  # Damage taken penalty
            ogre_result['avg_duration_seconds'] * 0.1  # Efficiency (faster is better)
        )

        # Lich tests mage combat - weight win rate and survival higher
        lich_score = (
            lich_result['win_rate'] * 70 -       # Win rate (more important)
            lich_result['avg_damage_taken'] * 0.3 +  # Damage taken penalty
            lich_result['avg_damage_dealt'] * 0.1    # Damage efficiency bonus
        )

        combined_score = ogre_score + lich_score

        result = {
            'param_combo': dict(zip(param_names, combo)),
            'ogre_win_rate': ogre_result['win_rate'],
            'ogre_dmg_taken': ogre_result['avg_damage_taken'],
            'ogre_duration': ogre_result['avg_duration_seconds'],
            'lich_win_rate': lich_result['win_rate'],
            'lich_dmg_taken': lich_result['avg_damage_taken'],
            'lich_duration': lich_result['avg_duration_seconds'],
            'combined_score': combined_score,
        }
        results.append(result)

        print(f"  vs Ogre Lord: {ogre_result['win_rate']*100:.0f}% win, "
              f"{ogre_result['avg_damage_taken']:.0f} dmg taken, "
              f"{ogre_result['avg_duration_seconds']:.0f}s")
        print(f"  vs Lich:      {lich_result['win_rate']*100:.0f}% win, "
              f"{lich_result['avg_damage_taken']:.0f} dmg taken, "
              f"{lich_result['avg_duration_seconds']:.1f}s")
        print(f"  Combined score: {combined_score:.1f}")

        if combined_score > best_combined_score:
            best_combined_score = combined_score
            best_result = result

    # Print results
    print("\n" + "=" * 70)
    print("DUAL-ENEMY OPTIMIZATION RESULTS")
    print("=" * 70)

    # Sort by combined score
    sorted_results = sorted(results, key=lambda x: x['combined_score'], reverse=True)

    print("\nTOP 5 CONFIGURATIONS:")
    print("-" * 70)
    for i, r in enumerate(sorted_results[:5]):
        print(f"\n#{i+1}: Score={r['combined_score']:.1f}")
        for name, value in r['param_combo'].items():
            print(f"  {name}: {value}")
        print(f"  vs Ogre Lord: {r['ogre_win_rate']*100:.0f}% win, {r['ogre_dmg_taken']:.0f} dmg")
        print(f"  vs Lich: {r['lich_win_rate']*100:.0f}% win, {r['lich_dmg_taken']:.0f} dmg")

    print("\n" + "=" * 70)
    print("BEST CONFIGURATION:")
    print("=" * 70)
    for name, value in best_result['param_combo'].items():
        print(f"  {name}: {value}")

    # Save results
    output_dir = os.path.dirname(os.path.abspath(__file__))
    output_file = os.path.join(output_dir, "dual_enemy_results.json")
    with open(output_file, 'w') as f:
        json.dump(sorted_results, f, indent=2)
    print(f"\nResults saved to: {output_file}")

    return best_result


def compare_configs(config1: SimulationConfig, config2: SimulationConfig,
                     simulations: int = 100):
    """Compare two configurations head-to-head"""
    print(f"\n{'='*70}")
    print("CONFIGURATION COMPARISON")
    print(f"{'='*70}\n")

    print("Config 1:")
    print(f"  min_retreat_distance: {config1.min_retreat_distance}")
    print(f"  casting_range: {config1.casting_range}")
    print(f"  health_heal_threshold: {config1.health_heal_threshold}")

    print("\nConfig 2:")
    print(f"  min_retreat_distance: {config2.min_retreat_distance}")
    print(f"  casting_range: {config2.casting_range}")
    print(f"  health_heal_threshold: {config2.health_heal_threshold}")

    print(f"\nRunning {simulations} simulations each...")

    result1 = run_batch_simulation(config1, simulations, verbose=False)
    result2 = run_batch_simulation(config2, simulations, verbose=False)

    print(f"\n{'='*70}")
    print("RESULTS")
    print(f"{'='*70}")

    print(f"\n{'Metric':<25} {'Config 1':<15} {'Config 2':<15} {'Diff':<10}")
    print("-" * 65)

    metrics = [
        ("Win Rate", "win_rate", lambda x: f"{x*100:.1f}%"),
        ("Avg Duration (s)", "avg_duration_seconds", lambda x: f"{x:.1f}"),
        ("Avg Damage Dealt", "avg_damage_dealt", lambda x: f"{x:.0f}"),
        ("Avg Damage Taken", "avg_damage_taken", lambda x: f"{x:.0f}"),
        ("Avg Times Cornered", "avg_times_cornered", lambda x: f"{x:.1f}"),
    ]

    for name, key, fmt in metrics:
        v1 = result1[key]
        v2 = result2[key]
        diff = v2 - v1
        diff_str = f"+{diff:.1f}" if diff > 0 else f"{diff:.1f}"
        print(f"{name:<25} {fmt(v1):<15} {fmt(v2):<15} {diff_str:<10}")

    # Declare winner
    if result1['win_rate'] > result2['win_rate']:
        print(f"\n>>> Config 1 is better (higher win rate)")
    elif result2['win_rate'] > result1['win_rate']:
        print(f"\n>>> Config 2 is better (higher win rate)")
    else:
        print(f"\n>>> Configs are equivalent")


def main():
    """Main entry point"""
    import sys

    if len(sys.argv) > 1:
        if sys.argv[1] == "full":
            full_optimization()
        elif sys.argv[1] == "dual":
            dual_enemy_optimization()
        elif sys.argv[1] == "compare":
            # Example comparison
            config1 = SimulationConfig(
                min_retreat_distance=4,
                casting_range=12,
            )
            config2 = SimulationConfig(
                min_retreat_distance=5,
                casting_range=14,
            )
            compare_configs(config1, config2, simulations=50)
        else:
            print("Usage: python parameter_optimizer.py [quick|full|dual|compare]")
            print("  quick  - Quick optimization with basic parameters")
            print("  full   - Full optimization with more parameters")
            print("  dual   - Test against both Ogre Lord and Lich")
            print("  compare - Compare two specific configurations")
    else:
        quick_optimization()


if __name__ == "__main__":
    main()
