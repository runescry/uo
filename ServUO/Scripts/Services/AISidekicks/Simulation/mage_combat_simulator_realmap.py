#!/usr/bin/env python3
r"""
Mage Combat Simulator with Real Map Integration

Enhanced version of mage_combat_simulator.py that uses real UO map data
instead of simplified courtyard/open area maps.

This integrates:
- map_data_extractor.py: Real terrain and static data from .mul files
- walkability_check.py: Real Map.CanFit() logic
- movement_check.py: Real Movement.CheckMovement() logic
- fast_astar.py: Real pathfinding
- sidekick_ai_movement.py: Real SidekickAI movement

Usage:
    python mage_combat_simulator_realmap.py --ticks 100
    python mage_combat_simulator_realmap.py --simulations 100 --output results.json
"""

import sys
import argparse
import json
import random
from dataclasses import dataclass, field
from enum import Enum, auto
from typing import Optional, List, Tuple, Dict
from pathlib import Path

# Import simulation components
from map_data_extractor import MapDataExtractor
from walkability_check import WalkabilityChecker
from tiledata_reference import TileDataReference
from movement_check import MovementChecker, Direction
from fast_astar import FastAStar
from sidekick_ai_movement import SidekickMovement, MoveResult, Position

# Test region from Combat Sim Plan
TEST_REGION = {
    'x1': 5379, 'y1': 4,
    'x2': 5499, 'y2': 125
}

# Default UO client path
DEFAULT_UO_PATH = r"C:\Program Files (x86)\Electronic Arts\Ultima Online Classic"

# Combat constants (matching original simulator)
MIN_SAFE_RANGE = 15
MAX_CAST_RANGE = 25
OPTIMAL_CAST_RANGE = 20
SPELL_RELEASE_RANGE_MIN = 8
SPELL_RELEASE_RANGE_MAX = 10

CRITICAL_HEALTH = 0.25
LOW_HEALTH = 0.45
LOW_MANA = 15

SPELL_COST_EXPLOSION = 20
SPELL_COST_ENERGY_BOLT = 4
SPELL_COST_GREATER_HEAL = 11
SPELL_DAMAGE_EXPLOSION = 40
SPELL_DAMAGE_ENERGY_BOLT = 25

ENEMY_MELEE_RANGE = 2
ENEMY_DAMAGE_MIN = 25
ENEMY_DAMAGE_MAX = 40


class CombatPriority(Enum):
    CRITICAL = 1
    DEFENSIVE = 2
    OFFENSIVE = 3
    POSITIONING = 4


class ActionType(Enum):
    CAST_SPELL = "cast_spell"
    RETREAT = "retreat"
    ADVANCE = "advance"
    HEAL = "heal"
    STAY = "stay"


@dataclass
class CombatState:
    """Combat state for simulation"""
    mage_x: int
    mage_y: int
    mage_z: int
    mage_health: int = 100
    mage_health_max: int = 100
    mage_mana: int = 100
    mage_mana_max: int = 100

    enemy_x: int = 0
    enemy_y: int = 0
    enemy_z: int = 0
    enemy_health: int = 150
    enemy_health_max: int = 150

    is_casting: bool = False
    cast_ticks_remaining: int = 0
    spell_cooldown: int = 0

    def mage_position(self) -> Tuple[int, int, int]:
        return (self.mage_x, self.mage_y, self.mage_z)

    def enemy_position(self) -> Tuple[int, int, int]:
        return (self.enemy_x, self.enemy_y, self.enemy_z)

    def distance(self) -> float:
        dx = self.mage_x - self.enemy_x
        dy = self.mage_y - self.enemy_y
        return (dx*dx + dy*dy) ** 0.5

    def health_percent(self) -> float:
        return self.mage_health / self.mage_health_max


@dataclass
class SimulationResult:
    """Result of a single simulation"""
    ticks: int
    mage_survived: bool
    enemy_defeated: bool
    final_health: int
    final_mana: int
    final_enemy_health: int
    final_distance: float
    spells_cast: int
    retreats: int
    advances: int
    damage_dealt: int
    damage_taken: int
    cornered_ticks: int
    path_failures: int

    def to_dict(self) -> dict:
        return {
            'ticks': self.ticks,
            'mage_survived': self.mage_survived,
            'enemy_defeated': self.enemy_defeated,
            'final_health': self.final_health,
            'final_mana': self.final_mana,
            'final_enemy_health': self.final_enemy_health,
            'final_distance': self.final_distance,
            'spells_cast': self.spells_cast,
            'retreats': self.retreats,
            'advances': self.advances,
            'damage_dealt': self.damage_dealt,
            'damage_taken': self.damage_taken,
            'cornered_ticks': self.cornered_ticks,
            'path_failures': self.path_failures
        }


class RealMapCombatSimulator:
    """
    Combat simulator using real UO map data.

    Uses the ported movement and pathfinding systems for accurate simulation.
    """

    def __init__(self, data_path: str = DEFAULT_UO_PATH):
        """
        Initialize the simulator with real map data.

        Args:
            data_path: Path to UO client directory
        """
        print(f"Loading map data from: {data_path}")

        # Initialize map systems
        self.extractor = MapDataExtractor(data_path)
        self.tiledata = TileDataReference()
        self.walk_checker = WalkabilityChecker(self.extractor, self.tiledata)
        self.move_checker = MovementChecker(self.extractor, self.walk_checker)
        self.pathfinder = FastAStar(self.move_checker)

        # Preload test region
        print(f"Preloading region ({TEST_REGION['x1']}, {TEST_REGION['y1']}) to ({TEST_REGION['x2']}, {TEST_REGION['y2']})")
        self.extractor.preload_region(
            TEST_REGION['x1'], TEST_REGION['y1'],
            TEST_REGION['x2'], TEST_REGION['y2']
        )

        # Create mage movement AI
        self.mage_ai = SidekickMovement(self.pathfinder, self.move_checker)

        # Statistics
        self.total_simulations = 0
        self.total_mage_wins = 0
        self.total_enemy_wins = 0

    def find_valid_spawn_point(self) -> Tuple[int, int, int]:
        """
        Find a valid spawn point in the test region.

        Returns:
            (x, y, z) of a walkable position
        """
        for _ in range(100):  # Try 100 times
            x = random.randint(TEST_REGION['x1'] + 10, TEST_REGION['x2'] - 10)
            y = random.randint(TEST_REGION['y1'] + 10, TEST_REGION['y2'] - 10)

            avg = self.walk_checker.get_average_z(x, y)
            z = avg.avg_z

            if self.walk_checker.can_fit(x, y, z):
                return (x, y, z)

        # Fallback to center of region
        x = (TEST_REGION['x1'] + TEST_REGION['x2']) // 2
        y = (TEST_REGION['y1'] + TEST_REGION['y2']) // 2
        avg = self.walk_checker.get_average_z(x, y)
        return (x, y, avg.avg_z)

    def spawn_combatants(self, min_distance: int = 15, max_distance: int = 25) -> CombatState:
        """
        Spawn mage and enemy at appropriate positions.

        Args:
            min_distance: Minimum starting distance
            max_distance: Maximum starting distance

        Returns:
            Initial CombatState
        """
        # Spawn mage
        mage_x, mage_y, mage_z = self.find_valid_spawn_point()

        # Find enemy spawn point at appropriate distance
        for _ in range(50):
            angle = random.uniform(0, 2 * 3.14159)
            distance = random.uniform(min_distance, max_distance)

            enemy_x = int(mage_x + distance * __import__('math').cos(angle))
            enemy_y = int(mage_y + distance * __import__('math').sin(angle))

            # Check bounds
            if not (TEST_REGION['x1'] <= enemy_x <= TEST_REGION['x2']):
                continue
            if not (TEST_REGION['y1'] <= enemy_y <= TEST_REGION['y2']):
                continue

            # Check walkability
            avg = self.walk_checker.get_average_z(enemy_x, enemy_y)
            enemy_z = avg.avg_z

            if self.walk_checker.can_fit(enemy_x, enemy_y, enemy_z):
                return CombatState(
                    mage_x=mage_x, mage_y=mage_y, mage_z=mage_z,
                    enemy_x=enemy_x, enemy_y=enemy_y, enemy_z=enemy_z
                )

        # Fallback: spawn enemy nearby
        enemy_x = mage_x + 20
        enemy_y = mage_y + 20
        avg = self.walk_checker.get_average_z(enemy_x, enemy_y)
        return CombatState(
            mage_x=mage_x, mage_y=mage_y, mage_z=mage_z,
            enemy_x=enemy_x, enemy_y=enemy_y, enemy_z=avg.avg_z
        )

    def evaluate_priority(self, state: CombatState) -> CombatPriority:
        """Evaluate combat priority based on state"""
        distance = state.distance()
        health_pct = state.health_percent()

        # Critical: in melee or very low health
        if distance <= ENEMY_MELEE_RANGE or health_pct < CRITICAL_HEALTH:
            return CombatPriority.CRITICAL

        # Defensive: low health
        if health_pct < LOW_HEALTH:
            return CombatPriority.DEFENSIVE

        # Offensive: in good casting range
        if MIN_SAFE_RANGE <= distance <= MAX_CAST_RANGE:
            return CombatPriority.OFFENSIVE

        return CombatPriority.POSITIONING

    def mage_action(self, state: CombatState) -> Tuple[ActionType, str]:
        """
        Determine and execute mage action.

        Args:
            state: Current combat state

        Returns:
            (ActionType, description)
        """
        # Update mage AI position
        self.mage_ai.set_position(state.mage_x, state.mage_y, state.mage_z)

        priority = self.evaluate_priority(state)
        distance = state.distance()

        # CRITICAL: Retreat immediately
        if priority == CombatPriority.CRITICAL:
            result = self.mage_ai.run_from(state.enemy_x, state.enemy_y)
            if result == MoveResult.Success:
                pos = self.mage_ai.get_position()
                state.mage_x, state.mage_y, state.mage_z = pos
                return (ActionType.RETREAT, f"Critical: retreating to {pos}")
            return (ActionType.STAY, "Critical: cornered!")

        # DEFENSIVE: Heal and retreat
        if priority == CombatPriority.DEFENSIVE:
            if state.mage_mana >= SPELL_COST_GREATER_HEAL and state.spell_cooldown == 0:
                state.mage_health = min(state.mage_health_max, state.mage_health + 30)
                state.mage_mana -= SPELL_COST_GREATER_HEAL
                state.spell_cooldown = 5
                return (ActionType.HEAL, "Defensive: casting Greater Heal")

            result = self.mage_ai.run_from(state.enemy_x, state.enemy_y)
            if result == MoveResult.Success:
                pos = self.mage_ai.get_position()
                state.mage_x, state.mage_y, state.mage_z = pos
                return (ActionType.RETREAT, f"Defensive: retreating to {pos}")
            return (ActionType.STAY, "Defensive: cannot retreat")

        # OFFENSIVE: Cast spells
        if priority == CombatPriority.OFFENSIVE:
            if state.spell_cooldown == 0:
                # Choose spell based on mana
                if state.mage_mana >= SPELL_COST_EXPLOSION:
                    state.mage_mana -= SPELL_COST_EXPLOSION
                    state.enemy_health -= SPELL_DAMAGE_EXPLOSION
                    state.spell_cooldown = 5
                    return (ActionType.CAST_SPELL, f"Offensive: Explosion for {SPELL_DAMAGE_EXPLOSION} damage")
                elif state.mage_mana >= SPELL_COST_ENERGY_BOLT:
                    state.mage_mana -= SPELL_COST_ENERGY_BOLT
                    state.enemy_health -= SPELL_DAMAGE_ENERGY_BOLT
                    state.spell_cooldown = 4
                    return (ActionType.CAST_SPELL, f"Offensive: Energy Bolt for {SPELL_DAMAGE_ENERGY_BOLT} damage")

            # Kite while on cooldown
            if distance < OPTIMAL_CAST_RANGE:
                result = self.mage_ai.run_from(state.enemy_x, state.enemy_y)
                if result == MoveResult.Success:
                    pos = self.mage_ai.get_position()
                    state.mage_x, state.mage_y, state.mage_z = pos
                    return (ActionType.RETREAT, f"Offensive: kiting to {pos}")

            return (ActionType.STAY, "Offensive: waiting for cooldown")

        # POSITIONING: Adjust distance
        if distance < MIN_SAFE_RANGE:
            result = self.mage_ai.run_from(state.enemy_x, state.enemy_y)
            if result == MoveResult.Success:
                pos = self.mage_ai.get_position()
                state.mage_x, state.mage_y, state.mage_z = pos
                return (ActionType.RETREAT, f"Positioning: too close, retreating")

        if distance > MAX_CAST_RANGE:
            result = self.mage_ai.move_to_target(
                state.enemy_x, state.enemy_y, state.enemy_z,
                casting_range=OPTIMAL_CAST_RANGE
            )
            if result == MoveResult.Success:
                pos = self.mage_ai.get_position()
                state.mage_x, state.mage_y, state.mage_z = pos
                return (ActionType.ADVANCE, f"Positioning: too far, advancing")

        return (ActionType.STAY, "Positioning: at good range")

    def enemy_action(self, state: CombatState):
        """
        Execute enemy action (simple AI: chase and attack).

        Args:
            state: Current combat state
        """
        distance = state.distance()

        # Attack if in melee range
        if distance <= ENEMY_MELEE_RANGE:
            damage = random.randint(ENEMY_DAMAGE_MIN, ENEMY_DAMAGE_MAX)
            state.mage_health -= damage
            return

        # Move toward mage
        dx = state.mage_x - state.enemy_x
        dy = state.mage_y - state.enemy_y
        dist = (dx*dx + dy*dy) ** 0.5

        if dist > 0:
            step_x = int(round(dx / dist))
            step_y = int(round(dy / dist))

            new_x = state.enemy_x + step_x
            new_y = state.enemy_y + step_y

            # Check if can move there
            avg = self.walk_checker.get_average_z(new_x, new_y)
            if self.walk_checker.can_fit(new_x, new_y, avg.avg_z):
                state.enemy_x = new_x
                state.enemy_y = new_y
                state.enemy_z = avg.avg_z

    def simulate_combat(self, max_ticks: int = 200, verbose: bool = False) -> SimulationResult:
        """
        Run a single combat simulation.

        Args:
            max_ticks: Maximum number of ticks
            verbose: Print detailed output

        Returns:
            SimulationResult
        """
        state = self.spawn_combatants()
        self.mage_ai.reset_stats()

        spells_cast = 0
        retreats = 0
        advances = 0
        damage_dealt = 0
        damage_taken = 0
        cornered_ticks = 0
        path_failures = 0

        initial_enemy_health = state.enemy_health

        for tick in range(max_ticks):
            # Check end conditions
            if state.mage_health <= 0:
                if verbose:
                    print(f"Tick {tick}: MAGE DIED")
                break

            if state.enemy_health <= 0:
                if verbose:
                    print(f"Tick {tick}: ENEMY DEFEATED")
                break

            # Mage turn
            old_health = state.mage_health
            action_type, desc = self.mage_action(state)

            if action_type == ActionType.CAST_SPELL:
                spells_cast += 1
            elif action_type == ActionType.RETREAT:
                retreats += 1
            elif action_type == ActionType.ADVANCE:
                advances += 1

            # Check if cornered
            if self.mage_ai.is_cornered(state.enemy_x, state.enemy_y):
                cornered_ticks += 1

            # Decrease cooldown
            if state.spell_cooldown > 0:
                state.spell_cooldown -= 1

            if verbose and tick < 20:
                print(f"Tick {tick}: {action_type.value:12} | H:{state.mage_health:3d} M:{state.mage_mana:3d} "
                      f"D:{state.distance():.1f} EH:{state.enemy_health:3d} | {desc}")

            # Enemy turn
            old_mage_health = state.mage_health
            self.enemy_action(state)
            damage_taken += old_mage_health - state.mage_health

        # Calculate damage dealt
        damage_dealt = initial_enemy_health - state.enemy_health

        # Get path failure stats
        stats = self.mage_ai.get_stats()
        path_failures = stats['blocked_moves']

        return SimulationResult(
            ticks=tick + 1,
            mage_survived=state.mage_health > 0,
            enemy_defeated=state.enemy_health <= 0,
            final_health=max(0, state.mage_health),
            final_mana=state.mage_mana,
            final_enemy_health=max(0, state.enemy_health),
            final_distance=state.distance(),
            spells_cast=spells_cast,
            retreats=retreats,
            advances=advances,
            damage_dealt=damage_dealt,
            damage_taken=damage_taken,
            cornered_ticks=cornered_ticks,
            path_failures=path_failures
        )

    def run_batch(self, num_simulations: int, max_ticks: int = 200) -> List[dict]:
        """
        Run multiple simulations.

        Args:
            num_simulations: Number of simulations
            max_ticks: Max ticks per simulation

        Returns:
            List of result dictionaries
        """
        results = []

        for i in range(num_simulations):
            if (i + 1) % 10 == 0:
                print(f"Simulation {i + 1}/{num_simulations}")

            result = self.simulate_combat(max_ticks)
            results.append(result.to_dict())

            if result.mage_survived:
                self.total_mage_wins += 1
            else:
                self.total_enemy_wins += 1

            self.total_simulations += 1

        return results

    def print_summary(self, results: List[dict]):
        """Print summary statistics"""
        print("\n" + "=" * 80)
        print("SIMULATION SUMMARY")
        print("=" * 80)

        mage_wins = sum(1 for r in results if r['mage_survived'])
        enemy_wins = sum(1 for r in results if not r['mage_survived'])
        enemy_defeated = sum(1 for r in results if r['enemy_defeated'])

        print(f"Total Simulations: {len(results)}")
        print(f"Mage Wins: {mage_wins} ({mage_wins/len(results)*100:.1f}%)")
        print(f"Enemy Wins: {enemy_wins} ({enemy_wins/len(results)*100:.1f}%)")
        print(f"Enemies Defeated: {enemy_defeated} ({enemy_defeated/len(results)*100:.1f}%)")

        avg_ticks = sum(r['ticks'] for r in results) / len(results)
        avg_health = sum(r['final_health'] for r in results) / len(results)
        avg_spells = sum(r['spells_cast'] for r in results) / len(results)
        avg_retreats = sum(r['retreats'] for r in results) / len(results)
        avg_cornered = sum(r['cornered_ticks'] for r in results) / len(results)

        print(f"\nAverage Combat Length: {avg_ticks:.1f} ticks")
        print(f"Average Final Health: {avg_health:.1f}")
        print(f"Average Spells Cast: {avg_spells:.1f}")
        print(f"Average Retreats: {avg_retreats:.1f}")
        print(f"Average Cornered Ticks: {avg_cornered:.1f}")

        # Cache stats
        cache_stats = self.extractor.get_cache_stats()
        print(f"\nMap Cache: {cache_stats['blocks_cached']} blocks, "
              f"{cache_stats['hit_rate']*100:.1f}% hit rate")


def main():
    parser = argparse.ArgumentParser(description="Mage Combat Simulator with Real Map Data")
    parser.add_argument("--ticks", type=int, default=200, help="Max ticks per simulation")
    parser.add_argument("--simulations", type=int, default=1, help="Number of simulations")
    parser.add_argument("--output", help="Output file for results (JSON)")
    parser.add_argument("--verbose", action="store_true", help="Verbose output")
    parser.add_argument("--data-path", default=DEFAULT_UO_PATH, help="Path to UO client")

    args = parser.parse_args()

    try:
        simulator = RealMapCombatSimulator(args.data_path)

        if args.simulations == 1:
            # Single simulation with verbose output
            result = simulator.simulate_combat(args.ticks, verbose=True)
            print("\n" + "=" * 80)
            print("SIMULATION RESULT")
            print("=" * 80)
            for key, value in result.to_dict().items():
                print(f"  {key}: {value}")
        else:
            # Batch simulations
            results = simulator.run_batch(args.simulations, args.ticks)
            simulator.print_summary(results)

            if args.output:
                with open(args.output, 'w') as f:
                    json.dump(results, f, indent=2)
                print(f"\nResults saved to {args.output}")

    except FileNotFoundError as e:
        print(f"Error: {e}")
        print(f"\nPlease provide the path to your UO client with --data-path")
        sys.exit(1)


if __name__ == "__main__":
    main()
