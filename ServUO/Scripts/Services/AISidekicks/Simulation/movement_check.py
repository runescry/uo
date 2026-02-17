#!/usr/bin/env python3
r"""
Movement Check for Combat Simulation

Port of ServUO's Movement.CheckMovement() for validating directional movement.
Handles diagonal movement rules, Z-level changes, and step height limits.

Based on ServUO Scripts/Services/Pathing/Movement.cs

Usage:
    from movement_check import MovementChecker, Direction
    from map_data_extractor import MapDataExtractor
    from walkability_check import WalkabilityChecker

    extractor = MapDataExtractor("C:\\UO\\Client")
    walk_checker = WalkabilityChecker(extractor)
    move_checker = MovementChecker(extractor, walk_checker)

    # Check if movement is valid
    can_move, new_z = move_checker.check_movement(5400, 50, 33, Direction.North)
"""

from dataclasses import dataclass
from enum import IntEnum
from typing import List, Optional, Tuple, TYPE_CHECKING

if TYPE_CHECKING:
    from map_data_extractor import MapDataExtractor
    from walkability_check import WalkabilityChecker

from tiledata_reference import TileDataReference, TileFlags


# Constants from ServUO Movement.cs
PERSON_HEIGHT = 16
STEP_HEIGHT = 2  # Maximum Z step up/down


class Direction(IntEnum):
    """Movement directions (matches ServUO Direction enum)"""
    North = 0x0      # -Y
    Right = 0x1      # +X, -Y (Northeast)
    East = 0x2       # +X
    Down = 0x3       # +X, +Y (Southeast)
    South = 0x4      # +Y
    Left = 0x5       # -X, +Y (Southwest)
    West = 0x6       # -X
    Up = 0x7         # -X, -Y (Northwest)

    # Aliases
    Northeast = 0x1
    Southeast = 0x3
    Southwest = 0x5
    Northwest = 0x7


# Direction offsets (dx, dy)
DIRECTION_OFFSETS = {
    Direction.North: (0, -1),
    Direction.Right: (1, -1),   # Northeast
    Direction.East: (1, 0),
    Direction.Down: (1, 1),     # Southeast
    Direction.South: (0, 1),
    Direction.Left: (-1, 1),    # Southwest
    Direction.West: (-1, 0),
    Direction.Up: (-1, -1),     # Northwest
}


@dataclass
class MovementResult:
    """Result of movement check"""
    can_move: bool
    new_z: int
    blocking_reason: Optional[str] = None


class MovementChecker:
    """
    Validates directional movement.

    Port of ServUO's MovementImpl.CheckMovement()
    """

    def __init__(self,
                 map_extractor: 'MapDataExtractor',
                 walkability_checker: 'WalkabilityChecker',
                 tiledata: Optional[TileDataReference] = None,
                 map_width: int = 7168,
                 map_height: int = 4096):
        """
        Initialize the movement checker.

        Args:
            map_extractor: MapDataExtractor instance
            walkability_checker: WalkabilityChecker instance
            tiledata: TileDataReference instance (creates default if None)
            map_width: Map width in tiles
            map_height: Map height in tiles
        """
        self.map = map_extractor
        self.walk_checker = walkability_checker
        self.tiledata = tiledata or TileDataReference()
        self.map_width = map_width
        self.map_height = map_height

    def get_direction_offset(self, direction: Direction) -> Tuple[int, int]:
        """Get X,Y offset for a direction"""
        return DIRECTION_OFFSETS.get(direction, (0, 0))

    def is_diagonal(self, direction: Direction) -> bool:
        """Check if direction is diagonal"""
        return (int(direction) & 0x1) == 0x1

    def get_adjacent_directions(self, direction: Direction) -> Tuple[Direction, Direction]:
        """
        Get the two cardinal directions adjacent to a diagonal.

        For diagonal movement, both adjacent tiles must be passable.
        """
        d = int(direction)
        left = Direction((d - 1) & 0x7)
        right = Direction((d + 1) & 0x7)
        return left, right

    def check_movement(self, x: int, y: int, z: int,
                       direction: Direction,
                       can_swim: bool = False,
                       cant_walk: bool = False) -> Tuple[bool, int]:
        """
        Check if movement in the given direction is valid.

        Port of ServUO MovementImpl.CheckMovement()

        Args:
            x: Current X position
            y: Current Y position
            z: Current Z level
            direction: Direction to move
            can_swim: Whether creature can swim (water tiles)
            cant_walk: Whether creature can only be in water

        Returns:
            Tuple of (can_move, new_z)
        """
        result = self.check_movement_detailed(x, y, z, direction, can_swim, cant_walk)
        return result.can_move, result.new_z

    def check_movement_detailed(self, x: int, y: int, z: int,
                                direction: Direction,
                                can_swim: bool = False,
                                cant_walk: bool = False) -> MovementResult:
        """
        Detailed check if movement in the given direction is valid.

        Args:
            x: Current X position
            y: Current Y position
            z: Current Z level
            direction: Direction to move
            can_swim: Whether creature can swim
            cant_walk: Whether creature can only be in water

        Returns:
            MovementResult with detailed information
        """
        # Get destination coordinates
        dx, dy = self.get_direction_offset(direction)
        dest_x = x + dx
        dest_y = y + dy

        # Check map bounds
        if dest_x < 0 or dest_y < 0 or dest_x >= self.map_width or dest_y >= self.map_height:
            return MovementResult(
                can_move=False,
                new_z=z,
                blocking_reason="Destination out of bounds"
            )

        # Get start Z info
        start_z, start_top = self._get_start_z(x, y, z)

        # Check forward movement
        can_move, new_z = self._check_position(
            dest_x, dest_y, start_z, start_top, can_swim, cant_walk
        )

        if not can_move:
            return MovementResult(
                can_move=False,
                new_z=z,
                blocking_reason=f"Cannot move to ({dest_x}, {dest_y})"
            )

        # For diagonal movement, check adjacent tiles
        if self.is_diagonal(direction):
            left_dir, right_dir = self.get_adjacent_directions(direction)

            # Get adjacent tile positions
            left_dx, left_dy = self.get_direction_offset(left_dir)
            right_dx, right_dy = self.get_direction_offset(right_dir)

            left_x, left_y = x + left_dx, y + left_dy
            right_x, right_y = x + right_dx, y + right_dy

            # Check both adjacent tiles
            left_ok, _ = self._check_position(
                left_x, left_y, start_z, start_top, can_swim, cant_walk
            )
            right_ok, _ = self._check_position(
                right_x, right_y, start_z, start_top, can_swim, cant_walk
            )

            # For players, both sides must be passable
            # For NPCs, at least one side must be passable
            # We'll use the NPC rule (at least one) for AI simulation
            if not left_ok and not right_ok:
                return MovementResult(
                    can_move=False,
                    new_z=z,
                    blocking_reason="Diagonal blocked: both adjacent tiles impassable"
                )

        return MovementResult(
            can_move=True,
            new_z=new_z
        )

    def _get_start_z(self, x: int, y: int, z: int) -> Tuple[int, int]:
        """
        Get starting Z level info for movement.

        Port of ServUO MovementImpl.GetStartZ()

        Returns:
            Tuple of (z_low, z_top)
        """
        avg = self.walk_checker.get_average_z(x, y)
        terrain = self.map.get_terrain(x, y)

        z_low = z
        z_top = z

        if terrain:
            land_flags = self.tiledata.get_land_flags(terrain.tile_id)
            land_blocks = bool(land_flags & TileFlags.Impassable)

            if not land_blocks and z >= avg.avg_z:
                z_low = avg.low_z
                z_top = avg.top_z

        # Check statics for surfaces
        statics = self.map.get_statics(x, y)

        for static in statics:
            static_flags = self.tiledata.get_static_flags(static.tile_id)
            static_height = self.tiledata.get_static_height(static.tile_id)

            is_surface = bool(static_flags & TileFlags.Surface)
            calc_top = static.z + static_height

            if is_surface and z >= calc_top:
                if calc_top >= z_low:
                    z_low = static.z
                    top = static.z + static_height
                    if top > z_top:
                        z_top = top

        return z_low, z_top

    def _check_position(self, x: int, y: int,
                        start_z: int, start_top: int,
                        can_swim: bool, cant_walk: bool) -> Tuple[bool, int]:
        """
        Check if a position can be moved to.

        Port of ServUO MovementImpl.Check()

        Returns:
            Tuple of (can_move, new_z)
        """
        new_z = 0

        # Get terrain info
        terrain = self.map.get_terrain(x, y)
        if terrain is None:
            return False, start_z

        avg = self.walk_checker.get_average_z(x, y)

        land_flags = self.tiledata.get_land_flags(terrain.tile_id)
        land_blocks = bool(land_flags & TileFlags.Impassable)

        # Handle swimming
        if land_blocks and can_swim and (land_flags & TileFlags.Wet):
            land_blocks = False
        elif cant_walk and not (land_flags & TileFlags.Wet):
            land_blocks = True

        move_is_ok = False
        step_top = start_top + STEP_HEIGHT
        check_top = start_z + PERSON_HEIGHT

        # Check static tiles for surfaces
        statics = self.map.get_statics(x, y)

        for static in statics:
            static_flags = self.tiledata.get_static_flags(static.tile_id)
            static_height = self.tiledata.get_static_height(static.tile_id)

            is_surface = bool(static_flags & TileFlags.Surface)
            is_impassable = bool(static_flags & TileFlags.Impassable)
            is_wet = bool(static_flags & TileFlags.Wet)

            # Surface and not impassable (or wet and can swim)
            if (is_surface and not is_impassable) or (can_swim and is_wet):
                if cant_walk and not is_wet:
                    continue

                item_z = static.z
                our_z = item_z + static_height
                our_top = our_z + PERSON_HEIGHT

                if move_is_ok:
                    # Choose position closest to start_z
                    if abs(our_z - start_z) > abs(new_z - start_z):
                        continue

                test_top = max(check_top, our_top)

                # Calculate item top for step check
                item_top = item_z
                is_bridge = bool(static_flags & TileFlags.Bridge)
                if not is_bridge:
                    item_top += static_height

                # Can we step onto this?
                if step_top >= item_top:
                    # Check Z range for blocking
                    if self._is_ok_z_range(our_z, test_top, statics):
                        new_z = our_z
                        move_is_ok = True

        # Check land tile
        if not land_blocks and step_top >= avg.low_z:
            our_z = avg.avg_z
            our_top = our_z + PERSON_HEIGHT
            test_top = max(check_top, our_top)

            should_check = True
            if move_is_ok:
                if abs(our_z - start_z) > abs(new_z - start_z):
                    should_check = False

            if should_check and self._is_ok_z_range(our_z, test_top, statics):
                new_z = our_z
                move_is_ok = True

        if not move_is_ok:
            new_z = start_z

        return move_is_ok, new_z

    def _is_ok_z_range(self, our_z: int, our_top: int,
                       statics: list) -> bool:
        """
        Check if the Z range is clear of blocking tiles.

        Port of ServUO MovementImpl.IsOk()
        """
        for static in statics:
            static_flags = self.tiledata.get_static_flags(static.tile_id)
            static_height = self.tiledata.get_static_height(static.tile_id)

            is_surface = bool(static_flags & TileFlags.Surface)
            is_impassable = bool(static_flags & TileFlags.Impassable)

            # Check impassable or surface tiles
            if is_impassable or is_surface:
                check_z = static.z
                check_top = check_z + static_height

                # Does this tile block our Z range?
                if check_top > our_z and our_top > check_z:
                    return False

        return True

    def get_valid_directions(self, x: int, y: int, z: int,
                             can_swim: bool = False,
                             cant_walk: bool = False) -> List[Direction]:
        """
        Get all valid movement directions from a position.

        Args:
            x: Current X position
            y: Current Y position
            z: Current Z level
            can_swim: Whether creature can swim
            cant_walk: Whether creature can only be in water

        Returns:
            List of valid directions
        """
        valid = []

        for direction in Direction:
            can_move, _ = self.check_movement(x, y, z, direction, can_swim, cant_walk)
            if can_move:
                valid.append(direction)

        return valid

    def get_best_retreat_direction(self, x: int, y: int, z: int,
                                   enemy_x: int, enemy_y: int) -> Optional[Direction]:
        """
        Get the best direction to retreat from an enemy.

        Chooses the direction that moves furthest from enemy.

        Args:
            x, y, z: Current position
            enemy_x, enemy_y: Enemy position

        Returns:
            Best retreat direction, or None if cornered
        """
        valid_dirs = self.get_valid_directions(x, y, z)

        if not valid_dirs:
            return None

        best_dir = None
        best_dist_sq = -1

        for direction in valid_dirs:
            dx, dy = self.get_direction_offset(direction)
            new_x, new_y = x + dx, y + dy

            # Calculate squared distance to enemy
            dist_sq = (new_x - enemy_x) ** 2 + (new_y - enemy_y) ** 2

            if dist_sq > best_dist_sq:
                best_dist_sq = dist_sq
                best_dir = direction

        return best_dir

    def get_direction_toward(self, x: int, y: int,
                             target_x: int, target_y: int) -> Direction:
        """
        Get the direction from current position toward target.

        Args:
            x, y: Current position
            target_x, target_y: Target position

        Returns:
            Direction toward target
        """
        dx = target_x - x
        dy = target_y - y

        # Determine primary direction
        if dx == 0 and dy == 0:
            return Direction.North  # No movement needed

        # Convert to direction
        if dx > 0:
            if dy > 0:
                return Direction.Down      # Southeast
            elif dy < 0:
                return Direction.Right     # Northeast
            else:
                return Direction.East
        elif dx < 0:
            if dy > 0:
                return Direction.Left      # Southwest
            elif dy < 0:
                return Direction.Up        # Northwest
            else:
                return Direction.West
        else:  # dx == 0
            if dy > 0:
                return Direction.South
            else:
                return Direction.North


# Test function
def main():
    """Test the movement checker"""
    import sys
    from map_data_extractor import MapDataExtractor
    from walkability_check import WalkabilityChecker

    default_path = r"C:\Program Files (x86)\Electronic Arts\Ultima Online Classic"

    if len(sys.argv) > 1:
        data_path = sys.argv[1]
    else:
        data_path = default_path

    print("=" * 80)
    print("MOVEMENT CHECKER TEST")
    print("=" * 80)
    print(f"Data path: {data_path}")

    try:
        # Initialize
        extractor = MapDataExtractor(data_path)
        walk_checker = WalkabilityChecker(extractor)
        move_checker = MovementChecker(extractor, walk_checker)

        # Test positions from dungeon region
        test_pos = (5400, 50)
        avg = walk_checker.get_average_z(test_pos[0], test_pos[1])
        start_z = avg.avg_z

        print(f"\nTest position: ({test_pos[0]}, {test_pos[1]}, {start_z})")

        # Test all directions
        print("\n--- Direction Tests ---")
        for direction in Direction:
            result = move_checker.check_movement_detailed(
                test_pos[0], test_pos[1], start_z, direction
            )
            dx, dy = move_checker.get_direction_offset(direction)
            dest = (test_pos[0] + dx, test_pos[1] + dy)
            status = "OK" if result.can_move else "BLOCKED"
            print(f"  {direction.name:10s} -> {dest}: {status} (new_z={result.new_z})")
            if result.blocking_reason:
                print(f"             Reason: {result.blocking_reason}")

        # Test valid directions
        valid = move_checker.get_valid_directions(test_pos[0], test_pos[1], start_z)
        print(f"\nValid directions: {[d.name for d in valid]}")

        # Test retreat direction
        enemy_pos = (5405, 55)
        retreat_dir = move_checker.get_best_retreat_direction(
            test_pos[0], test_pos[1], start_z, enemy_pos[0], enemy_pos[1]
        )
        print(f"\nEnemy at {enemy_pos}")
        print(f"Best retreat direction: {retreat_dir.name if retreat_dir else 'CORNERED'}")

        # Test movement path
        print("\n--- Simulated Movement Path ---")
        cur_x, cur_y, cur_z = test_pos[0], test_pos[1], start_z
        target_x, target_y = 5410, 60

        print(f"Moving from ({cur_x}, {cur_y}) to ({target_x}, {target_y})")

        for step in range(15):
            if cur_x == target_x and cur_y == target_y:
                print(f"  Step {step}: ARRIVED!")
                break

            direction = move_checker.get_direction_toward(cur_x, cur_y, target_x, target_y)
            can_move, new_z = move_checker.check_movement(cur_x, cur_y, cur_z, direction)

            if can_move:
                dx, dy = move_checker.get_direction_offset(direction)
                cur_x += dx
                cur_y += dy
                cur_z = new_z
                print(f"  Step {step}: {direction.name:10s} -> ({cur_x}, {cur_y}, {cur_z})")
            else:
                print(f"  Step {step}: {direction.name:10s} -> BLOCKED")
                # Try alternate directions
                valid = move_checker.get_valid_directions(cur_x, cur_y, cur_z)
                if valid:
                    alt_dir = valid[0]
                    dx, dy = move_checker.get_direction_offset(alt_dir)
                    can_move, new_z = move_checker.check_movement(cur_x, cur_y, cur_z, alt_dir)
                    if can_move:
                        cur_x += dx
                        cur_y += dy
                        cur_z = new_z
                        print(f"           {alt_dir.name:10s} -> ({cur_x}, {cur_y}, {cur_z}) [alternate]")
                else:
                    print("           No valid directions - STUCK!")
                    break

        print("\n" + "=" * 80)
        print("TEST COMPLETE")
        print("=" * 80)

    except FileNotFoundError as e:
        print(f"\nError: {e}")
        sys.exit(1)


if __name__ == "__main__":
    main()
