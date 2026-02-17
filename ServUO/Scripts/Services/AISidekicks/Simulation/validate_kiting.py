#!/usr/bin/env python3
"""
Validate Kiting and Retreating Behavior

This script tests and visualizes the AI movement to verify
that retreating actually increases distance from enemy.
"""

from map_data_extractor import MapDataExtractor
from walkability_check import WalkabilityChecker
from movement_check import MovementChecker
from fast_astar import FastAStar
from sidekick_ai_movement import SidekickMovement, MoveResult

DEFAULT_UO_PATH = r"C:\Program Files (x86)\Electronic Arts\Ultima Online Classic"


def test_retreat_behavior():
    """Test that run_from actually increases distance"""
    print("=" * 70)
    print("TEST 1: RETREAT BEHAVIOR")
    print("=" * 70)

    # Initialize
    extractor = MapDataExtractor(DEFAULT_UO_PATH)
    walk_checker = WalkabilityChecker(extractor)
    move_checker = MovementChecker(extractor, walk_checker)
    pathfinder = FastAStar(move_checker)
    ai = SidekickMovement(pathfinder, move_checker)

    # Start position
    start_x, start_y = 5400, 50
    start_z = walk_checker.get_average_z(start_x, start_y).avg_z
    ai.set_position(start_x, start_y, start_z)

    # Enemy position (close by)
    enemy_x, enemy_y = 5405, 55

    print(f"\nStarting position: {ai.get_position()}")
    print(f"Enemy position: ({enemy_x}, {enemy_y})")
    print(f"Initial distance: {ai.get_distance_to(enemy_x, enemy_y):.1f}")

    # Track positions
    positions = [ai.get_position()]
    distances = [ai.get_distance_to(enemy_x, enemy_y)]

    print("\n--- Retreating 10 steps ---")
    print(f"{'Step':<6} {'Position':<25} {'Distance':<10} {'Result':<12} {'Moved Away?'}")
    print("-" * 70)

    successful_retreats = 0
    for step in range(10):
        old_dist = ai.get_distance_to(enemy_x, enemy_y)

        result = ai.run_from(enemy_x, enemy_y)

        new_pos = ai.get_position()
        new_dist = ai.get_distance_to(enemy_x, enemy_y)

        moved_away = new_dist > old_dist
        if moved_away:
            successful_retreats += 1

        positions.append(new_pos)
        distances.append(new_dist)

        status = "YES (+{:.1f})".format(new_dist - old_dist) if moved_away else "NO (blocked)"
        print(f"{step+1:<6} {str(new_pos):<25} {new_dist:<10.1f} {result.name:<12} {status}")

    print("\n--- Distance Analysis ---")
    print(f"Starting distance: {distances[0]:.1f}")
    print(f"Final distance: {distances[-1]:.1f}")
    print(f"Distance gained: {distances[-1] - distances[0]:.1f}")
    print(f"Successful retreats: {successful_retreats}/10")

    # Verdict
    if distances[-1] > distances[0]:
        print("\n[PASS] AI successfully retreated (distance increased)")
    else:
        print("\n[FAIL] AI did not retreat effectively")

    return positions, distances


def test_kiting_behavior():
    """Test maintaining casting range while kiting"""
    print("\n" + "=" * 70)
    print("TEST 2: KITING BEHAVIOR (Maintain Casting Range)")
    print("=" * 70)

    # Initialize
    extractor = MapDataExtractor(DEFAULT_UO_PATH)
    walk_checker = WalkabilityChecker(extractor)
    move_checker = MovementChecker(extractor, walk_checker)
    pathfinder = FastAStar(move_checker)
    ai = SidekickMovement(pathfinder, move_checker)

    # Start too close
    start_x, start_y = 5400, 50
    start_z = walk_checker.get_average_z(start_x, start_y).avg_z
    ai.set_position(start_x, start_y, start_z)

    # Enemy nearby
    enemy_x, enemy_y = 5403, 53
    enemy_z = walk_checker.get_average_z(enemy_x, enemy_y).avg_z

    casting_range = 12
    min_distance = 4

    print(f"\nStarting position: {ai.get_position()}")
    print(f"Enemy position: ({enemy_x}, {enemy_y})")
    print(f"Target range: {min_distance}-{casting_range} tiles")
    print(f"Initial distance: {ai.get_distance_to(enemy_x, enemy_y):.1f}")

    print("\n--- Maintaining casting range for 15 steps ---")
    print(f"{'Step':<6} {'Position':<25} {'Distance':<10} {'In Range?':<12} {'Action'}")
    print("-" * 70)

    in_range_count = 0
    for step in range(15):
        pos = ai.get_position()
        dist = ai.get_distance_to(enemy_x, enemy_y)
        in_range = min_distance <= dist <= casting_range

        if in_range:
            in_range_count += 1

        # Determine action
        if dist < min_distance:
            action = "Retreating (too close)"
            result = ai.run_from(enemy_x, enemy_y)
        elif dist > casting_range:
            action = "Approaching (too far)"
            result = ai.move_to_target(enemy_x, enemy_y, enemy_z, casting_range)
        else:
            action = "In range (can cast)"
            result = MoveResult.AtDestination

        new_pos = ai.get_position()
        new_dist = ai.get_distance_to(enemy_x, enemy_y)

        range_status = "YES" if in_range else "NO"
        print(f"{step+1:<6} {str(new_pos):<25} {new_dist:<10.1f} {range_status:<12} {action}")

        # Simulate enemy moving closer
        enemy_x += 1 if enemy_x < new_pos[0] else -1
        enemy_y += 1 if enemy_y < new_pos[1] else -1

    print(f"\n--- Kiting Analysis ---")
    print(f"Steps in casting range: {in_range_count}/15")
    print(f"Kiting efficiency: {in_range_count/15*100:.0f}%")

    if in_range_count >= 8:
        print("\n[PASS] AI maintained casting range effectively")
    else:
        print("\n[FAIL] AI struggled to maintain casting range")


def test_approach_behavior():
    """Test moving toward target"""
    print("\n" + "=" * 70)
    print("TEST 3: APPROACH BEHAVIOR")
    print("=" * 70)

    # Initialize
    extractor = MapDataExtractor(DEFAULT_UO_PATH)
    walk_checker = WalkabilityChecker(extractor)
    move_checker = MovementChecker(extractor, walk_checker)
    pathfinder = FastAStar(move_checker)
    ai = SidekickMovement(pathfinder, move_checker)

    # Start far away
    start_x, start_y = 5400, 50
    start_z = walk_checker.get_average_z(start_x, start_y).avg_z
    ai.set_position(start_x, start_y, start_z)

    # Target far away
    target_x, target_y = 5420, 70
    target_z = walk_checker.get_average_z(target_x, target_y).avg_z

    print(f"\nStarting position: {ai.get_position()}")
    print(f"Target position: ({target_x}, {target_y})")
    print(f"Initial distance: {ai.get_distance_to(target_x, target_y):.1f}")

    print("\n--- Approaching target (casting range = 12) ---")
    print(f"{'Step':<6} {'Position':<25} {'Distance':<10} {'Result'}")
    print("-" * 70)

    distances = [ai.get_distance_to(target_x, target_y)]

    for step in range(15):
        result = ai.move_to_target(target_x, target_y, target_z, casting_range=12)

        new_pos = ai.get_position()
        new_dist = ai.get_distance_to(target_x, target_y)
        distances.append(new_dist)

        print(f"{step+1:<6} {str(new_pos):<25} {new_dist:<10.1f} {result.name}")

        if result == MoveResult.AtDestination:
            print(f"\n  Reached casting range at step {step+1}!")
            break

    print(f"\n--- Approach Analysis ---")
    print(f"Starting distance: {distances[0]:.1f}")
    print(f"Final distance: {distances[-1]:.1f}")
    print(f"Distance closed: {distances[0] - distances[-1]:.1f}")

    if distances[-1] <= 12:
        print("\n[PASS] AI reached casting range")
    else:
        print("\n[FAIL] AI didn't reach casting range")


def visualize_movement_path():
    """Generate ASCII visualization of movement"""
    print("\n" + "=" * 70)
    print("MOVEMENT PATH VISUALIZATION")
    print("=" * 70)

    # Initialize
    extractor = MapDataExtractor(DEFAULT_UO_PATH)
    walk_checker = WalkabilityChecker(extractor)
    move_checker = MovementChecker(extractor, walk_checker)
    pathfinder = FastAStar(move_checker)
    ai = SidekickMovement(pathfinder, move_checker)

    # Start position
    start_x, start_y = 5400, 50
    start_z = walk_checker.get_average_z(start_x, start_y).avg_z
    ai.set_position(start_x, start_y, start_z)

    # Enemy position
    enemy_x, enemy_y = 5405, 55

    # Collect retreat path
    path = [(start_x, start_y)]
    for _ in range(10):
        ai.run_from(enemy_x, enemy_y)
        pos = ai.get_position()
        path.append((pos[0], pos[1]))

    # Find bounds
    all_x = [p[0] for p in path] + [enemy_x]
    all_y = [p[1] for p in path] + [enemy_y]
    min_x, max_x = min(all_x) - 2, max(all_x) + 2
    min_y, max_y = min(all_y) - 2, max(all_y) + 2

    print(f"\nLegend: S=Start, E=Enemy, .=Path, #=Final position")
    print(f"Coordinates: X={min_x}-{max_x}, Y={min_y}-{max_y}")
    print()

    # Draw grid
    for y in range(min_y, max_y + 1):
        row = ""
        for x in range(min_x, max_x + 1):
            if (x, y) == (enemy_x, enemy_y):
                row += "E "
            elif (x, y) == path[0]:
                row += "S "
            elif (x, y) == path[-1]:
                row += "# "
            elif (x, y) in path:
                row += ". "
            else:
                row += "  "
        print(f"  {row}")

    print(f"\nPath: S -> ", end="")
    for i, (x, y) in enumerate(path[1:], 1):
        if i < len(path) - 1:
            print(f"({x},{y}) -> ", end="")
        else:
            print(f"# ({x},{y})")


def main():
    print("=" * 70)
    print("KITING AND RETREATING VALIDATION")
    print("=" * 70)
    print("\nThis script validates that the AI movement system correctly:")
    print("  1. Retreats (increases distance from enemy)")
    print("  2. Kites (maintains casting range)")
    print("  3. Approaches (decreases distance to target)")

    test_retreat_behavior()
    test_kiting_behavior()
    test_approach_behavior()
    visualize_movement_path()

    print("\n" + "=" * 70)
    print("VALIDATION COMPLETE")
    print("=" * 70)


if __name__ == "__main__":
    main()
