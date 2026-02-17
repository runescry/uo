#!/usr/bin/env python3
r"""
Sidekick AI Movement for Combat Simulation

Port of ServUO's SidekickAI movement logic for mage combat simulation.
Handles MoveTo, RunFrom, WalkMobileRange, and casting range maintenance.

Based on ServUO Scripts/Services/AISidekicks/AI/SidekickAI.cs

Usage:
    from sidekick_ai_movement import SidekickMovement
    from fast_astar import FastAStar
    from movement_check import MovementChecker

    ai = SidekickMovement(pathfinder, move_checker)
    ai.set_position(5400, 50, 33)

    # Move to target within casting range
    ai.move_to_target(enemy_x, enemy_y, enemy_z, casting_range=12)

    # Run from enemy
    ai.run_from(enemy_x, enemy_y)
"""

from dataclasses import dataclass, field
from enum import Enum, auto
from typing import List, Optional, Tuple, TYPE_CHECKING
import math

if TYPE_CHECKING:
    from fast_astar import FastAStar, PathFollower
    from movement_check import MovementChecker

from movement_check import Direction


# Spell casting ranges
SPELL_RANGE_MELEE = 1
SPELL_RANGE_CLOSE = 8
SPELL_RANGE_MEDIUM = 12
SPELL_RANGE_FAR = 20

# Movement constants
DEFAULT_CASTING_RANGE = 12  # Typical magery spell range
MIN_RETREAT_DISTANCE = 4   # Minimum distance to maintain when retreating


class MoveResult(Enum):
    """Result of a movement attempt"""
    Success = auto()
    Blocked = auto()
    AtDestination = auto()
    PathNotFound = auto()
    BadState = auto()


@dataclass
class Position:
    """3D position"""
    x: int
    y: int
    z: int

    def distance_to(self, other: 'Position') -> float:
        """Calculate Euclidean distance to another position"""
        dx = self.x - other.x
        dy = self.y - other.y
        dz = self.z - other.z
        return math.sqrt(dx*dx + dy*dy + dz*dz)

    def distance_2d(self, other: 'Position') -> float:
        """Calculate 2D distance (ignoring Z)"""
        dx = self.x - other.x
        dy = self.y - other.y
        return math.sqrt(dx*dx + dy*dy)

    def manhattan_distance(self, other: 'Position') -> int:
        """Calculate Manhattan distance"""
        return abs(self.x - other.x) + abs(self.y - other.y)

    def as_tuple(self) -> Tuple[int, int, int]:
        return (self.x, self.y, self.z)


@dataclass
class SidekickState:
    """State of the sidekick for movement decisions"""
    position: Position
    can_move: bool = True
    is_casting: bool = False
    is_running: bool = False
    last_target: Optional[Position] = None
    current_path: Optional[List[Direction]] = None
    path_index: int = 0


class SidekickMovement:
    """
    Manages Sidekick AI movement in combat simulation.

    Provides movement primitives similar to ServUO's SidekickAI.
    """

    def __init__(self,
                 pathfinder: 'FastAStar',
                 move_checker: 'MovementChecker'):
        """
        Initialize the sidekick movement system.

        Args:
            pathfinder: FastAStar pathfinder instance
            move_checker: MovementChecker for movement validation
        """
        self.pathfinder = pathfinder
        self.move_checker = move_checker

        # State
        self.state = SidekickState(Position(0, 0, 0))

        # Movement stats
        self.moves_made = 0
        self.paths_calculated = 0
        self.blocked_moves = 0

    def set_position(self, x: int, y: int, z: int):
        """Set current position"""
        self.state.position = Position(x, y, z)

    def get_position(self) -> Tuple[int, int, int]:
        """Get current position as tuple"""
        return self.state.position.as_tuple()

    def get_direction_to(self, target_x: int, target_y: int) -> Direction:
        """Get direction from current position toward target"""
        return self.move_checker.get_direction_toward(
            self.state.position.x, self.state.position.y,
            target_x, target_y
        )

    def get_distance_to(self, target_x: int, target_y: int, target_z: int = None) -> float:
        """Get distance to target"""
        target = Position(target_x, target_y, target_z or self.state.position.z)
        return self.state.position.distance_2d(target)

    def do_move(self, direction: Direction, run: bool = False) -> MoveResult:
        """
        Attempt to move in a direction.

        Port of SidekickAI.DoMove()

        Args:
            direction: Direction to move
            run: Whether to run (affects speed, not pathfinding)

        Returns:
            MoveResult indicating success/failure
        """
        if not self.state.can_move:
            return MoveResult.BadState

        if self.state.is_casting:
            return MoveResult.BadState

        # Check movement
        can_move, new_z = self.move_checker.check_movement(
            self.state.position.x,
            self.state.position.y,
            self.state.position.z,
            direction
        )

        if can_move:
            # Update position
            dx, dy = self.move_checker.get_direction_offset(direction)
            self.state.position.x += dx
            self.state.position.y += dy
            self.state.position.z = new_z
            self.state.is_running = run
            self.moves_made += 1
            return MoveResult.Success
        else:
            self.blocked_moves += 1
            return MoveResult.Blocked

    def move_to(self, target_x: int, target_y: int, target_z: int,
                run: bool = True, range: int = 1) -> MoveResult:
        """
        Move toward a target position within specified range.

        Port of SidekickAI.MoveTo()

        Args:
            target_x, target_y, target_z: Target position
            run: Whether to run
            range: Stop when within this range

        Returns:
            MoveResult
        """
        if not self.state.can_move:
            return MoveResult.BadState

        target = Position(target_x, target_y, target_z)
        distance = self.state.position.distance_2d(target)

        # Already in range
        if distance <= range:
            self.state.current_path = None
            return MoveResult.AtDestination

        # Check if we have an existing path to this target
        if (self.state.current_path is not None and
            self.state.last_target == target and
            self.state.path_index < len(self.state.current_path)):
            # Follow existing path
            return self._follow_path(run)

        # Try direct movement first
        direction = self.get_direction_to(target_x, target_y)
        result = self.do_move(direction, run)

        if result == MoveResult.Success:
            return result

        # Direct movement blocked, calculate path
        path = self.pathfinder.find_path(
            self.state.position.x, self.state.position.y, self.state.position.z,
            target_x, target_y, target_z
        )
        self.paths_calculated += 1

        if path is None or len(path) == 0:
            return MoveResult.PathNotFound

        # Store path and follow first step
        self.state.current_path = path
        self.state.last_target = target
        self.state.path_index = 0

        return self._follow_path(run)

    def _follow_path(self, run: bool) -> MoveResult:
        """Follow the current stored path"""
        if (self.state.current_path is None or
            self.state.path_index >= len(self.state.current_path)):
            return MoveResult.AtDestination

        direction = self.state.current_path[self.state.path_index]
        result = self.do_move(direction, run)

        if result == MoveResult.Success:
            self.state.path_index += 1
            # Clear completed path
            if self.state.path_index >= len(self.state.current_path):
                self.state.current_path = None

        return result

    def walk_mobile_range(self, target_x: int, target_y: int, target_z: int,
                          steps: int, run: bool,
                          min_range: int, max_range: int) -> MoveResult:
        """
        Move to stay within a range of a target.

        Port of SidekickAI.WalkMobileRange()

        Args:
            target_x, target_y, target_z: Target position
            steps: Maximum steps to take
            run: Whether to run
            min_range: Minimum desired distance
            max_range: Maximum desired distance

        Returns:
            MoveResult
        """
        if not self.state.can_move:
            return MoveResult.BadState

        target = Position(target_x, target_y, target_z)

        for _ in range(steps):
            distance = int(self.state.position.distance_2d(target))

            if min_range <= distance <= max_range:
                return MoveResult.AtDestination

            if distance > max_range:
                # Need to get closer
                return self.move_to(target_x, target_y, target_z, run, max_range)
            elif distance < min_range:
                # Need to retreat
                return self.run_from(target_x, target_y, run)

        # Check final distance
        distance = int(self.state.position.distance_2d(target))
        if min_range <= distance <= max_range:
            return MoveResult.AtDestination

        return MoveResult.Blocked

    def run_from(self, target_x: int, target_y: int,
                 run: bool = True,
                 preferred_corner: Tuple[int, int, int] = None) -> MoveResult:
        """
        Run away from a target.

        Port of SidekickAI.RunFrom()

        Args:
            target_x, target_y: Target to run from
            run: Whether to run
            preferred_corner: Optional preferred retreat position

        Returns:
            MoveResult
        """
        if not self.state.can_move:
            return MoveResult.BadState

        # Calculate retreat direction (opposite of direction to target)
        direction_to_target = self.get_direction_to(target_x, target_y)
        retreat_direction = Direction((int(direction_to_target) + 4) & 0x7)

        # If we have a preferred corner and it's close, navigate there
        if preferred_corner is not None:
            corner_x, corner_y, corner_z = preferred_corner
            corner_dist = self.get_distance_to(corner_x, corner_y)

            if corner_dist <= 8:
                return self.move_to(corner_x, corner_y, corner_z, run, range=1)

        # Try direct retreat
        result = self.do_move(retreat_direction, run)

        if result == MoveResult.Success:
            return result

        # Try alternative directions
        # First try the two directions adjacent to retreat
        left_dir = Direction((int(retreat_direction) - 1) & 0x7)
        right_dir = Direction((int(retreat_direction) + 1) & 0x7)

        for alt_dir in [left_dir, right_dir]:
            result = self.do_move(alt_dir, run)
            if result == MoveResult.Success:
                return result

        # Try perpendicular directions (90 degrees)
        left_90 = Direction((int(retreat_direction) - 2) & 0x7)
        right_90 = Direction((int(retreat_direction) + 2) & 0x7)

        for alt_dir in [left_90, right_90]:
            result = self.do_move(alt_dir, run)
            if result == MoveResult.Success:
                return result

        # Completely blocked
        return MoveResult.Blocked

    def maintain_casting_range(self, target_x: int, target_y: int, target_z: int,
                               casting_range: int = DEFAULT_CASTING_RANGE,
                               min_distance: int = MIN_RETREAT_DISTANCE) -> MoveResult:
        """
        Maintain optimal casting range from target.

        Moves closer if too far, retreats if too close.

        Args:
            target_x, target_y, target_z: Target position
            casting_range: Maximum spell range
            min_distance: Minimum safe distance

        Returns:
            MoveResult
        """
        return self.walk_mobile_range(
            target_x, target_y, target_z,
            steps=1,
            run=True,
            min_range=min_distance,
            max_range=casting_range
        )

    def move_to_target(self, target_x: int, target_y: int, target_z: int,
                       casting_range: int = DEFAULT_CASTING_RANGE) -> MoveResult:
        """
        Move toward target, stopping at casting range.

        Convenience method for typical mage movement.

        Args:
            target_x, target_y, target_z: Target position
            casting_range: Stop at this range

        Returns:
            MoveResult
        """
        distance = self.get_distance_to(target_x, target_y)

        if distance <= casting_range:
            return MoveResult.AtDestination

        return self.move_to(target_x, target_y, target_z, run=True, range=casting_range)

    def can_reach(self, target_x: int, target_y: int, target_z: int) -> bool:
        """
        Check if target is reachable.

        Args:
            target_x, target_y, target_z: Target position

        Returns:
            True if a path exists to target
        """
        # Check range first
        if not self.pathfinder.in_range(
            self.state.position.x, self.state.position.y,
            target_x, target_y
        ):
            return False

        # Try to find path
        path = self.pathfinder.find_path(
            self.state.position.x, self.state.position.y, self.state.position.z,
            target_x, target_y, target_z
        )

        return path is not None

    def get_valid_directions(self) -> List[Direction]:
        """Get all valid movement directions from current position"""
        return self.move_checker.get_valid_directions(
            self.state.position.x,
            self.state.position.y,
            self.state.position.z
        )

    def is_cornered(self, enemy_x: int, enemy_y: int) -> bool:
        """
        Check if cornered (limited retreat options).

        Args:
            enemy_x, enemy_y: Enemy position

        Returns:
            True if cornered (fewer than 2 retreat options)
        """
        retreat_options = 0
        direction_to_enemy = self.get_direction_to(enemy_x, enemy_y)

        # Check retreat direction and adjacent
        for offset in [-2, -1, 0, 1, 2]:
            test_dir = Direction((int(direction_to_enemy) + 4 + offset) & 0x7)
            can_move, _ = self.move_checker.check_movement(
                self.state.position.x, self.state.position.y, self.state.position.z,
                test_dir
            )
            if can_move:
                retreat_options += 1

        return retreat_options < 2

    def get_stats(self) -> dict:
        """Get movement statistics"""
        return {
            'moves_made': self.moves_made,
            'paths_calculated': self.paths_calculated,
            'blocked_moves': self.blocked_moves,
            'current_position': self.get_position(),
            'has_path': self.state.current_path is not None
        }

    def reset_stats(self):
        """Reset movement statistics"""
        self.moves_made = 0
        self.paths_calculated = 0
        self.blocked_moves = 0


# Test function
def main():
    """Test the sidekick AI movement"""
    import sys
    from map_data_extractor import MapDataExtractor
    from walkability_check import WalkabilityChecker
    from movement_check import MovementChecker
    from fast_astar import FastAStar

    default_path = r"C:\Program Files (x86)\Electronic Arts\Ultima Online Classic"

    if len(sys.argv) > 1:
        data_path = sys.argv[1]
    else:
        data_path = default_path

    print("=" * 80)
    print("SIDEKICK AI MOVEMENT TEST")
    print("=" * 80)
    print(f"Data path: {data_path}")

    try:
        # Initialize
        extractor = MapDataExtractor(data_path)
        walk_checker = WalkabilityChecker(extractor)
        move_checker = MovementChecker(extractor, walk_checker)
        pathfinder = FastAStar(move_checker)

        # Create AI movement
        ai = SidekickMovement(pathfinder, move_checker)

        # Set starting position
        start_x, start_y = 5400, 50
        start_avg = walk_checker.get_average_z(start_x, start_y)
        ai.set_position(start_x, start_y, start_avg.avg_z)

        print(f"\nStarting position: {ai.get_position()}")

        # Test 1: Move to target
        print("\n--- Test 1: Move To Target ---")
        target = (5420, 70, 0)
        target_avg = walk_checker.get_average_z(target[0], target[1])

        print(f"Target: ({target[0]}, {target[1]}, {target_avg.avg_z})")
        print(f"Initial distance: {ai.get_distance_to(target[0], target[1]):.1f}")

        for step in range(10):
            result = ai.move_to_target(target[0], target[1], target_avg.avg_z, casting_range=12)
            pos = ai.get_position()
            dist = ai.get_distance_to(target[0], target[1])
            print(f"  Step {step+1}: {result.name} -> {pos}, distance: {dist:.1f}")

            if result == MoveResult.AtDestination:
                print("  Reached casting range!")
                break

        # Test 2: Run from enemy
        print("\n--- Test 2: Run From Enemy ---")
        enemy = (5415, 65)
        print(f"Enemy position: {enemy}")

        for step in range(5):
            pos_before = ai.get_position()
            dist_before = ai.get_distance_to(enemy[0], enemy[1])

            result = ai.run_from(enemy[0], enemy[1])

            pos_after = ai.get_position()
            dist_after = ai.get_distance_to(enemy[0], enemy[1])

            print(f"  Step {step+1}: {result.name}")
            print(f"    Before: {pos_before}, dist={dist_before:.1f}")
            print(f"    After:  {pos_after}, dist={dist_after:.1f}")

        # Test 3: Maintain casting range
        print("\n--- Test 3: Maintain Casting Range ---")
        ai.set_position(5410, 60, -21)  # Reset position
        enemy = (5420, 70)
        casting_range = 12
        min_dist = 4

        print(f"Enemy: {enemy}, Range: {min_dist}-{casting_range}")

        for step in range(10):
            pos = ai.get_position()
            dist = ai.get_distance_to(enemy[0], enemy[1])
            in_range = min_dist <= dist <= casting_range

            result = ai.maintain_casting_range(
                enemy[0], enemy[1], 0,
                casting_range=casting_range,
                min_distance=min_dist
            )

            new_pos = ai.get_position()
            new_dist = ai.get_distance_to(enemy[0], enemy[1])

            print(f"  Step {step+1}: {result.name}")
            print(f"    Position: {pos} -> {new_pos}")
            print(f"    Distance: {dist:.1f} -> {new_dist:.1f} (in range: {in_range})")

            if result == MoveResult.AtDestination:
                print("  Optimal range maintained!")
                break

        # Test 4: Cornered detection
        print("\n--- Test 4: Cornered Detection ---")
        ai.set_position(5400, 50, 33)

        test_positions = [
            (5400, 50),  # Center area
            (5380, 10),  # Near edge
        ]

        for tx, ty in test_positions:
            ai.set_position(tx, ty, walk_checker.get_average_z(tx, ty).avg_z)
            enemy = (tx + 5, ty + 5)
            cornered = ai.is_cornered(enemy[0], enemy[1])
            valid = len(ai.get_valid_directions())
            print(f"  At ({tx}, {ty}): cornered={cornered}, valid_dirs={valid}")

        # Stats
        print("\n--- Movement Stats ---")
        stats = ai.get_stats()
        for key, value in stats.items():
            print(f"  {key}: {value}")

        print("\n" + "=" * 80)
        print("TEST COMPLETE")
        print("=" * 80)

    except FileNotFoundError as e:
        print(f"\nError: {e}")
        sys.exit(1)


if __name__ == "__main__":
    main()
