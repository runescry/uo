#!/usr/bin/env python3
r"""
Fast A* Pathfinding for Combat Simulation

Port of ServUO's FastAStarAlgorithm for efficient pathfinding.
Uses a fixed-size 38x38 search area with 13 Z-planes.

Based on ServUO Scripts/Services/Pathing/FastAStarAlgorithm.cs

Usage:
    from fast_astar import FastAStar
    from map_data_extractor import MapDataExtractor
    from walkability_check import WalkabilityChecker
    from movement_check import MovementChecker

    extractor = MapDataExtractor("C:\\UO\\Client")
    walk_checker = WalkabilityChecker(extractor)
    move_checker = MovementChecker(extractor, walk_checker)
    pathfinder = FastAStar(move_checker)

    # Find path
    path = pathfinder.find_path(5400, 50, 33, 5410, 60, -21)
    for direction in path:
        print(direction.name)
"""

from dataclasses import dataclass
from typing import List, Optional, Tuple, TYPE_CHECKING
import heapq

if TYPE_CHECKING:
    from movement_check import MovementChecker

from movement_check import Direction


# Constants from ServUO FastAStarAlgorithm
MAX_DEPTH = 300       # Maximum search iterations
AREA_SIZE = 38        # Search area size (38x38 tiles)
PLANE_OFFSET = 128    # Z offset for plane calculation
PLANE_COUNT = 13      # Number of Z planes
PLANE_HEIGHT = 20     # Z range per plane
NODE_COUNT = AREA_SIZE * AREA_SIZE * PLANE_COUNT


@dataclass
class PathNode:
    """A* pathfinding node"""
    x: int
    y: int
    z: int
    cost: int           # g(n) - cost from start
    total: int          # f(n) = g(n) + h(n)
    parent: Optional['PathNode'] = None

    def __lt__(self, other):
        """For heap ordering"""
        return self.total < other.total

    def __eq__(self, other):
        if other is None:
            return False
        return self.x == other.x and self.y == other.y and self.z == other.z

    def __hash__(self):
        return hash((self.x, self.y, self.z))


class FastAStar:
    """
    Fast A* pathfinding algorithm.

    Port of ServUO's FastAStarAlgorithm.
    Uses fixed 38x38 search area centered between start and goal.
    """

    def __init__(self, movement_checker: 'MovementChecker'):
        """
        Initialize the pathfinder.

        Args:
            movement_checker: MovementChecker instance for movement validation
        """
        self.move_checker = movement_checker

    def in_range(self, start_x: int, start_y: int,
                 goal_x: int, goal_y: int) -> bool:
        """
        Check if goal is within pathfinding range.

        Args:
            start_x, start_y: Start position
            goal_x, goal_y: Goal position

        Returns:
            True if goal is within AREA_SIZE tiles
        """
        dx = abs(goal_x - start_x)
        dy = abs(goal_y - start_y)
        return dx <= AREA_SIZE and dy <= AREA_SIZE

    def heuristic(self, x: int, y: int, z: int,
                  goal_x: int, goal_y: int, goal_z: int,
                  x_offset: int, y_offset: int) -> int:
        """
        Calculate heuristic distance to goal.

        Uses weighted squared distance (ServUO formula).

        Args:
            x, y, z: Current position (relative to offset)
            goal_x, goal_y, goal_z: Goal position
            x_offset, y_offset: Search area offset

        Returns:
            Heuristic value
        """
        dx = (x + x_offset) - goal_x
        dy = (y + y_offset) - goal_y
        dz = z - goal_z

        # ServUO weights X/Y by 11
        dx *= 11
        dy *= 11

        return (dx * dx) + (dy * dy) + (dz * dz)

    def get_index(self, x: int, y: int, z: int,
                  x_offset: int, y_offset: int) -> int:
        """
        Convert position to node index.

        Args:
            x, y, z: World position
            x_offset, y_offset: Search area offset

        Returns:
            Node index, or -1 if out of bounds
        """
        local_x = x - x_offset
        local_y = y - y_offset
        local_z = (z + PLANE_OFFSET) // PLANE_HEIGHT

        if local_x < 0 or local_x >= AREA_SIZE:
            return -1
        if local_y < 0 or local_y >= AREA_SIZE:
            return -1
        if local_z < 0 or local_z >= PLANE_COUNT:
            return -1

        return local_x + (local_y * AREA_SIZE) + (local_z * AREA_SIZE * AREA_SIZE)

    def get_direction(self, from_x: int, from_y: int,
                      to_x: int, to_y: int) -> Direction:
        """
        Get direction from one position to another.

        Args:
            from_x, from_y: Source position
            to_x, to_y: Destination position

        Returns:
            Direction enum
        """
        dx = to_x - from_x
        dy = to_y - from_y

        if dx == 0:
            if dy < 0:
                return Direction.North
            elif dy > 0:
                return Direction.South
            else:
                return Direction.North  # No movement
        elif dx > 0:
            if dy < 0:
                return Direction.Right  # Northeast
            elif dy > 0:
                return Direction.Down   # Southeast
            else:
                return Direction.East
        else:  # dx < 0
            if dy < 0:
                return Direction.Up     # Northwest
            elif dy > 0:
                return Direction.Left   # Southwest
            else:
                return Direction.West

    def find_path(self, start_x: int, start_y: int, start_z: int,
                  goal_x: int, goal_y: int, goal_z: int,
                  can_swim: bool = False,
                  cant_walk: bool = False) -> Optional[List[Direction]]:
        """
        Find path from start to goal.

        Args:
            start_x, start_y, start_z: Start position
            goal_x, goal_y, goal_z: Goal position
            can_swim: Whether creature can swim
            cant_walk: Whether creature can only be in water

        Returns:
            List of directions to follow, or None if no path found
        """
        # Check range
        if not self.in_range(start_x, start_y, goal_x, goal_y):
            return None

        # Calculate search area offset (centered between start and goal)
        x_offset = (start_x + goal_x - AREA_SIZE) // 2
        y_offset = (start_y + goal_y - AREA_SIZE) // 2

        # Initialize data structures
        open_list: List[PathNode] = []
        closed_set: set = set()
        node_map: dict = {}  # (x, y, z) -> PathNode

        # Create start node
        start_h = self.heuristic(
            start_x - x_offset, start_y - y_offset, start_z,
            goal_x, goal_y, goal_z, x_offset, y_offset
        )
        start_node = PathNode(
            x=start_x, y=start_y, z=start_z,
            cost=0, total=start_h, parent=None
        )

        heapq.heappush(open_list, start_node)
        node_map[(start_x, start_y, start_z)] = start_node

        depth = 0

        while open_list and depth < MAX_DEPTH:
            depth += 1

            # Get node with lowest f(n)
            current = heapq.heappop(open_list)

            # Check if we reached the goal
            if current.x == goal_x and current.y == goal_y:
                # Goal Z doesn't need exact match in original algorithm
                return self._reconstruct_path(current)

            # Add to closed set
            closed_set.add((current.x, current.y, current.z))

            # Get successors (valid moves from current position)
            successors = self._get_successors(
                current.x, current.y, current.z,
                x_offset, y_offset,
                can_swim, cant_walk
            )

            for succ_x, succ_y, succ_z in successors:
                # Skip if in closed set
                if (succ_x, succ_y, succ_z) in closed_set:
                    continue

                # Calculate costs
                new_cost = current.cost + 1
                new_h = self.heuristic(
                    succ_x - x_offset, succ_y - y_offset, succ_z,
                    goal_x, goal_y, goal_z, x_offset, y_offset
                )
                new_total = new_cost + new_h

                # Check if node exists
                existing = node_map.get((succ_x, succ_y, succ_z))

                if existing is None or new_total < existing.total:
                    # Create or update node
                    new_node = PathNode(
                        x=succ_x, y=succ_y, z=succ_z,
                        cost=new_cost, total=new_total,
                        parent=current
                    )
                    node_map[(succ_x, succ_y, succ_z)] = new_node
                    heapq.heappush(open_list, new_node)

        # No path found
        return None

    def _get_successors(self, x: int, y: int, z: int,
                        x_offset: int, y_offset: int,
                        can_swim: bool, cant_walk: bool) -> List[Tuple[int, int, int]]:
        """
        Get valid successor positions from current position.

        Args:
            x, y, z: Current position
            x_offset, y_offset: Search area offset
            can_swim, cant_walk: Movement flags

        Returns:
            List of (x, y, z) successor positions
        """
        successors = []

        for direction in Direction:
            dx, dy = self.move_checker.get_direction_offset(direction)
            new_x = x + dx
            new_y = y + dy

            # Check search area bounds
            local_x = new_x - x_offset
            local_y = new_y - y_offset

            if local_x < 0 or local_x >= AREA_SIZE:
                continue
            if local_y < 0 or local_y >= AREA_SIZE:
                continue

            # Check if movement is valid
            can_move, new_z = self.move_checker.check_movement(
                x, y, z, direction, can_swim, cant_walk
            )

            if can_move:
                # Validate Z is in valid plane range
                index = self.get_index(new_x, new_y, new_z, x_offset, y_offset)
                if index >= 0 and index < NODE_COUNT:
                    successors.append((new_x, new_y, new_z))

        return successors

    def _reconstruct_path(self, goal_node: PathNode) -> List[Direction]:
        """
        Reconstruct path from goal to start.

        Args:
            goal_node: The goal node reached

        Returns:
            List of directions from start to goal
        """
        directions = []
        current = goal_node

        while current.parent is not None:
            parent = current.parent
            direction = self.get_direction(
                parent.x, parent.y, current.x, current.y
            )
            directions.append(direction)
            current = parent

        # Reverse to get start-to-goal order
        directions.reverse()
        return directions

    def find_path_with_positions(self, start_x: int, start_y: int, start_z: int,
                                  goal_x: int, goal_y: int, goal_z: int,
                                  can_swim: bool = False,
                                  cant_walk: bool = False) -> Optional[List[Tuple[int, int, int]]]:
        """
        Find path and return list of positions instead of directions.

        Args:
            start_x, start_y, start_z: Start position
            goal_x, goal_y, goal_z: Goal position
            can_swim: Whether creature can swim
            cant_walk: Whether creature can only be in water

        Returns:
            List of (x, y, z) positions along path, or None if no path found
        """
        directions = self.find_path(
            start_x, start_y, start_z,
            goal_x, goal_y, goal_z,
            can_swim, cant_walk
        )

        if directions is None:
            return None

        # Convert directions to positions
        positions = [(start_x, start_y, start_z)]
        x, y, z = start_x, start_y, start_z

        for direction in directions:
            dx, dy = self.move_checker.get_direction_offset(direction)
            # Get the actual Z from movement check
            can_move, new_z = self.move_checker.check_movement(x, y, z, direction)
            if can_move:
                x, y, z = x + dx, y + dy, new_z
                positions.append((x, y, z))

        return positions

    def path_length(self, start_x: int, start_y: int, start_z: int,
                    goal_x: int, goal_y: int, goal_z: int) -> int:
        """
        Calculate path length without returning full path.

        Args:
            start_x, start_y, start_z: Start position
            goal_x, goal_y, goal_z: Goal position

        Returns:
            Path length in steps, or -1 if no path
        """
        path = self.find_path(start_x, start_y, start_z, goal_x, goal_y, goal_z)
        return len(path) if path else -1


class PathFollower:
    """
    Helper class for following a computed path.

    Tracks current position along path and provides next direction.
    """

    def __init__(self, pathfinder: FastAStar):
        """
        Initialize the path follower.

        Args:
            pathfinder: FastAStar instance
        """
        self.pathfinder = pathfinder
        self.path: Optional[List[Direction]] = None
        self.path_index: int = 0
        self.goal: Optional[Tuple[int, int, int]] = None

    def set_goal(self, start_x: int, start_y: int, start_z: int,
                 goal_x: int, goal_y: int, goal_z: int) -> bool:
        """
        Calculate path to goal.

        Args:
            start_x, start_y, start_z: Current position
            goal_x, goal_y, goal_z: Goal position

        Returns:
            True if path found
        """
        self.path = self.pathfinder.find_path(
            start_x, start_y, start_z,
            goal_x, goal_y, goal_z
        )
        self.path_index = 0
        self.goal = (goal_x, goal_y, goal_z)

        return self.path is not None

    def get_next_direction(self) -> Optional[Direction]:
        """
        Get next direction to follow.

        Returns:
            Next Direction, or None if path complete or not set
        """
        if self.path is None or self.path_index >= len(self.path):
            return None

        direction = self.path[self.path_index]
        self.path_index += 1
        return direction

    def has_path(self) -> bool:
        """Check if a valid path exists"""
        return self.path is not None and len(self.path) > 0

    def is_complete(self) -> bool:
        """Check if path following is complete"""
        return self.path is None or self.path_index >= len(self.path)

    def remaining_steps(self) -> int:
        """Get remaining steps in path"""
        if self.path is None:
            return 0
        return max(0, len(self.path) - self.path_index)


# Test function
def main():
    """Test the Fast A* pathfinder"""
    import sys
    from map_data_extractor import MapDataExtractor
    from walkability_check import WalkabilityChecker
    from movement_check import MovementChecker

    default_path = r"C:\Program Files (x86)\Electronic Arts\Ultima Online Classic"

    if len(sys.argv) > 1:
        data_path = sys.argv[1]
    else:
        data_path = default_path

    print("=" * 80)
    print("FAST A* PATHFINDER TEST")
    print("=" * 80)
    print(f"Data path: {data_path}")

    try:
        # Initialize
        extractor = MapDataExtractor(data_path)
        walk_checker = WalkabilityChecker(extractor)
        move_checker = MovementChecker(extractor, walk_checker)
        pathfinder = FastAStar(move_checker)

        # Test path in dungeon region
        start = (5400, 50)
        goal = (5420, 70)

        start_avg = walk_checker.get_average_z(start[0], start[1])
        goal_avg = walk_checker.get_average_z(goal[0], goal[1])

        start_z = start_avg.avg_z
        goal_z = goal_avg.avg_z

        print(f"\nStart: ({start[0]}, {start[1]}, {start_z})")
        print(f"Goal:  ({goal[0]}, {goal[1]}, {goal_z})")
        print(f"Manhattan distance: {abs(goal[0]-start[0]) + abs(goal[1]-start[1])}")

        # Find path
        print("\n--- Finding Path ---")
        path = pathfinder.find_path(
            start[0], start[1], start_z,
            goal[0], goal[1], goal_z
        )

        if path:
            print(f"Path found! Length: {len(path)} steps")
            print("\nPath directions:")
            for i, direction in enumerate(path[:20]):  # Show first 20
                print(f"  Step {i+1}: {direction.name}")
            if len(path) > 20:
                print(f"  ... and {len(path) - 20} more steps")

            # Get positions
            positions = pathfinder.find_path_with_positions(
                start[0], start[1], start_z,
                goal[0], goal[1], goal_z
            )

            print("\nPath positions (first 10):")
            for i, (x, y, z) in enumerate(positions[:10]):
                print(f"  {i}: ({x}, {y}, {z})")
        else:
            print("No path found!")

        # Test PathFollower
        print("\n--- Path Follower Test ---")
        follower = PathFollower(pathfinder)

        if follower.set_goal(start[0], start[1], start_z, goal[0], goal[1], goal_z):
            print(f"Path set with {follower.remaining_steps()} steps")

            # Follow first 5 steps
            x, y, z = start[0], start[1], start_z
            for i in range(5):
                direction = follower.get_next_direction()
                if direction:
                    dx, dy = move_checker.get_direction_offset(direction)
                    can_move, new_z = move_checker.check_movement(x, y, z, direction)
                    if can_move:
                        x, y, z = x + dx, y + dy, new_z
                        print(f"  Step {i+1}: {direction.name} -> ({x}, {y}, {z})")

            print(f"Remaining steps: {follower.remaining_steps()}")
        else:
            print("Failed to set goal!")

        # Test unreachable goal
        print("\n--- Unreachable Goal Test ---")
        far_goal = (5500, 200)  # Outside AREA_SIZE range
        far_path = pathfinder.find_path(
            start[0], start[1], start_z,
            far_goal[0], far_goal[1], 0
        )
        print(f"Far goal ({far_goal[0]}, {far_goal[1]}): {'Path found' if far_path else 'Out of range'}")

        print("\n" + "=" * 80)
        print("TEST COMPLETE")
        print("=" * 80)

    except FileNotFoundError as e:
        print(f"\nError: {e}")
        sys.exit(1)


if __name__ == "__main__":
    main()
