#!/usr/bin/env python3
r"""
Walkability Check for Combat Simulation

Port of ServUO's Map.CanFit() logic for checking if a position can be occupied.
Validates terrain flags, static flags, and height calculations.

Based on ServUO Server/Map.cs

Usage:
    from walkability_check import WalkabilityChecker
    from map_data_extractor import MapDataExtractor

    extractor = MapDataExtractor("C:\\UO\\Client")
    checker = WalkabilityChecker(extractor)

    # Check if position is walkable
    can_walk = checker.can_fit(5400, 50, 0)

    # Get surface Z at position
    surface_z = checker.get_surface_z(5400, 50)
"""

from dataclasses import dataclass
from typing import List, Optional, Tuple, TYPE_CHECKING

if TYPE_CHECKING:
    from map_data_extractor import MapDataExtractor

from tiledata_reference import TileDataReference, TileFlags


# Default creature height (human-sized)
DEFAULT_HEIGHT = 16

# Maximum Z difference for stepping up/down
MAX_Z_STEP = 8


@dataclass
class AverageZ:
    """Result of GetAverageZ calculation"""
    low_z: int    # Minimum Z of 4 corners
    avg_z: int    # Average Z (for surface detection)
    top_z: int    # Maximum Z of 4 corners


@dataclass
class CanFitResult:
    """Detailed result of CanFit check"""
    can_fit: bool
    has_surface: bool
    surface_z: int
    blocking_reason: Optional[str] = None


class WalkabilityChecker:
    """
    Checks if positions can be walked on/through.

    Port of ServUO's Map.CanFit() and related methods.
    """

    def __init__(self,
                 map_extractor: 'MapDataExtractor',
                 tiledata: Optional[TileDataReference] = None,
                 map_width: int = 7168,
                 map_height: int = 4096):
        """
        Initialize the walkability checker.

        Args:
            map_extractor: MapDataExtractor instance for reading map data
            tiledata: TileDataReference instance (creates default if None)
            map_width: Map width in tiles
            map_height: Map height in tiles
        """
        self.map = map_extractor
        self.tiledata = tiledata or TileDataReference()
        self.map_width = map_width
        self.map_height = map_height

    def get_average_z(self, x: int, y: int) -> AverageZ:
        """
        Calculate average Z from 4 adjacent terrain tiles.

        Port of ServUO Map.GetAverageZ()

        This samples the terrain at:
        - (x, y) - top
        - (x, y+1) - left
        - (x+1, y) - right
        - (x+1, y+1) - bottom

        Args:
            x: World X coordinate
            y: World Y coordinate

        Returns:
            AverageZ with low, avg, and top values
        """
        # Get Z values from 4 corners
        z_top = self.map.get_terrain_z(x, y)
        z_left = self.map.get_terrain_z(x, y + 1)
        z_right = self.map.get_terrain_z(x + 1, y)
        z_bottom = self.map.get_terrain_z(x + 1, y + 1)

        # Calculate low (minimum)
        low_z = min(z_top, z_left, z_right, z_bottom)

        # Calculate top (maximum)
        top_z = max(z_top, z_left, z_right, z_bottom)

        # Calculate average using ServUO's method
        # Uses the pair with smaller difference for smoother terrain
        if abs(z_top - z_bottom) > abs(z_left - z_right):
            avg_z = self._floor_average(z_left, z_right)
        else:
            avg_z = self._floor_average(z_top, z_bottom)

        return AverageZ(low_z=low_z, avg_z=avg_z, top_z=top_z)

    def _floor_average(self, a: int, b: int) -> int:
        """
        Calculate floor average of two values.

        Port of ServUO Map.FloorAverage()
        """
        v = a + b
        if v < 0:
            v -= 1
        return v // 2

    def can_fit(self, x: int, y: int, z: int,
                height: int = DEFAULT_HEIGHT,
                check_blocks_fit: bool = False,
                require_surface: bool = True) -> bool:
        """
        Check if a creature can fit at the specified location.

        Port of ServUO Map.CanFit()

        Args:
            x: World X coordinate
            y: World Y coordinate
            z: Z level to check
            height: Height of creature (default 16 for human)
            check_blocks_fit: Whether to check BlocksFit items
            require_surface: Whether a surface is required to stand on

        Returns:
            True if the position can be occupied
        """
        result = self.can_fit_detailed(x, y, z, height, check_blocks_fit, require_surface)
        return result.can_fit

    def can_fit_detailed(self, x: int, y: int, z: int,
                         height: int = DEFAULT_HEIGHT,
                         check_blocks_fit: bool = False,
                         require_surface: bool = True) -> CanFitResult:
        """
        Detailed check if a creature can fit at the specified location.

        Returns detailed information about why a position is/isn't walkable.

        Args:
            x: World X coordinate
            y: World Y coordinate
            z: Z level to check
            height: Height of creature (default 16 for human)
            check_blocks_fit: Whether to check BlocksFit items
            require_surface: Whether a surface is required to stand on

        Returns:
            CanFitResult with detailed information
        """
        # Check map bounds
        if x < 0 or y < 0 or x >= self.map_width or y >= self.map_height:
            return CanFitResult(
                can_fit=False,
                has_surface=False,
                surface_z=z,
                blocking_reason="Out of map bounds"
            )

        # Get terrain data
        terrain = self.map.get_terrain(x, y)
        if terrain is None:
            return CanFitResult(
                can_fit=False,
                has_surface=False,
                surface_z=z,
                blocking_reason="No terrain data"
            )

        # Get average Z from 4 corners
        avg = self.get_average_z(x, y)

        # Check land tile flags
        land_flags = self.tiledata.get_land_flags(terrain.tile_id)

        # Check if land is impassable and blocks our Z range
        if (land_flags & TileFlags.Impassable) and avg.avg_z > z and (z + height) > avg.low_z:
            return CanFitResult(
                can_fit=False,
                has_surface=False,
                surface_z=avg.avg_z,
                blocking_reason=f"Impassable land tile {terrain.tile_id} at Z={avg.avg_z}"
            )

        # Check if we have a surface (walkable land at our Z)
        has_surface = (
            not (land_flags & TileFlags.Impassable) and
            z == avg.avg_z
        )
        surface_z = avg.avg_z if has_surface else z

        # Check static tiles
        statics = self.map.get_statics(x, y)

        for static in statics:
            static_flags = self.tiledata.get_static_flags(static.tile_id)
            static_height = self.tiledata.get_static_height(static.tile_id)

            is_surface = bool(static_flags & TileFlags.Surface)
            is_impassable = bool(static_flags & TileFlags.Impassable)

            # Calculate top of static
            static_top = static.z + static_height

            # Check if this static blocks our position
            # (surface or impassable) AND overlaps our Z range
            if (is_surface or is_impassable):
                if static_top > z and (z + height) > static.z:
                    return CanFitResult(
                        can_fit=False,
                        has_surface=has_surface,
                        surface_z=surface_z,
                        blocking_reason=f"Blocking static {static.tile_id} at Z={static.z}-{static_top}"
                    )

            # Check if this static provides a surface at our Z
            if is_surface and not is_impassable:
                if z == static_top:
                    has_surface = True
                    surface_z = static_top

        # Final check: do we have a surface if required?
        if require_surface and not has_surface:
            return CanFitResult(
                can_fit=False,
                has_surface=False,
                surface_z=surface_z,
                blocking_reason="No surface to stand on"
            )

        return CanFitResult(
            can_fit=True,
            has_surface=has_surface,
            surface_z=surface_z
        )

    def get_surface_z(self, x: int, y: int, start_z: int = 0) -> int:
        """
        Find the surface Z level at a position.

        Scans from start_z upward to find a walkable surface.

        Args:
            x: World X coordinate
            y: World Y coordinate
            start_z: Z level to start searching from

        Returns:
            Surface Z level, or start_z if no surface found
        """
        # Get terrain average Z
        avg = self.get_average_z(x, y)
        terrain = self.map.get_terrain(x, y)

        best_z = start_z

        # Check if terrain is a valid surface
        if terrain:
            land_flags = self.tiledata.get_land_flags(terrain.tile_id)
            if not (land_flags & TileFlags.Impassable):
                if avg.avg_z >= start_z:
                    best_z = avg.avg_z

        # Check statics for surfaces
        statics = self.map.get_statics(x, y)

        for static in statics:
            static_flags = self.tiledata.get_static_flags(static.tile_id)
            static_height = self.tiledata.get_static_height(static.tile_id)

            is_surface = bool(static_flags & TileFlags.Surface)
            is_impassable = bool(static_flags & TileFlags.Impassable)

            if is_surface and not is_impassable:
                surface_z = static.z + static_height
                if surface_z >= start_z and surface_z > best_z:
                    # Verify we can actually fit here
                    if self.can_fit(x, y, surface_z):
                        best_z = surface_z

        return best_z

    def find_valid_z(self, x: int, y: int, z: int,
                     height: int = DEFAULT_HEIGHT,
                     max_z_range: int = 20) -> Optional[int]:
        """
        Find a valid Z level near the specified position.

        Searches in a range around the given Z for a walkable position.

        Args:
            x: World X coordinate
            y: World Y coordinate
            z: Target Z level
            height: Height of creature
            max_z_range: Maximum Z difference to search

        Returns:
            Valid Z level, or None if no valid position found
        """
        # First, try the exact Z
        if self.can_fit(x, y, z, height):
            return z

        # Search upward and downward
        for offset in range(1, max_z_range + 1):
            # Try above
            test_z = z + offset
            if self.can_fit(x, y, test_z, height):
                return test_z

            # Try below
            test_z = z - offset
            if self.can_fit(x, y, test_z, height):
                return test_z

        return None

    def is_walkable(self, x: int, y: int) -> bool:
        """
        Simple check if a coordinate has any walkable surface.

        This is a convenience method that searches for any valid Z.

        Args:
            x: World X coordinate
            y: World Y coordinate

        Returns:
            True if there's any walkable surface at this coordinate
        """
        # Get terrain Z as starting point
        avg = self.get_average_z(x, y)

        # Try to find a valid Z in reasonable range
        return self.find_valid_z(x, y, avg.avg_z) is not None

    def get_blocking_statics(self, x: int, y: int, z: int,
                             height: int = DEFAULT_HEIGHT) -> List[dict]:
        """
        Get list of statics that would block movement at a position.

        Useful for debugging and understanding why a position is blocked.

        Args:
            x: World X coordinate
            y: World Y coordinate
            z: Z level to check
            height: Height of creature

        Returns:
            List of blocking static info dicts
        """
        blocking = []
        statics = self.map.get_statics(x, y)

        for static in statics:
            static_flags = self.tiledata.get_static_flags(static.tile_id)
            static_height = self.tiledata.get_static_height(static.tile_id)

            is_surface = bool(static_flags & TileFlags.Surface)
            is_impassable = bool(static_flags & TileFlags.Impassable)

            static_top = static.z + static_height

            if (is_surface or is_impassable):
                if static_top > z and (z + height) > static.z:
                    blocking.append({
                        'tile_id': static.tile_id,
                        'z': static.z,
                        'height': static_height,
                        'top': static_top,
                        'is_surface': is_surface,
                        'is_impassable': is_impassable
                    })

        return blocking


# Convenience function for quick checks
def can_walk_at(map_extractor: 'MapDataExtractor', x: int, y: int, z: int = None) -> bool:
    """
    Quick check if a position is walkable.

    Args:
        map_extractor: MapDataExtractor instance
        x: World X coordinate
        y: World Y coordinate
        z: Z level (if None, uses terrain surface Z)

    Returns:
        True if walkable
    """
    checker = WalkabilityChecker(map_extractor)

    if z is None:
        avg = checker.get_average_z(x, y)
        z = avg.avg_z

    return checker.can_fit(x, y, z)


# Test function
def main():
    """Test the walkability checker"""
    import sys
    from map_data_extractor import MapDataExtractor

    # Default UO client path
    default_path = r"C:\Program Files (x86)\Electronic Arts\Ultima Online Classic"

    if len(sys.argv) > 1:
        data_path = sys.argv[1]
    else:
        data_path = default_path

    print("=" * 80)
    print("WALKABILITY CHECKER TEST")
    print("=" * 80)
    print(f"Data path: {data_path}")

    try:
        # Initialize
        extractor = MapDataExtractor(data_path)
        checker = WalkabilityChecker(extractor)

        # Test region from Combat Sim Plan: (5379, 4) to (5499, 125)
        test_coords = [
            (5400, 50),   # Middle of dungeon region
            (5379, 4),    # Top-left corner
            (5499, 125),  # Bottom-right corner
            (5420, 60),   # Random point
            (5450, 80),   # Random point
        ]

        print("\n--- Walkability Tests ---")
        for x, y in test_coords:
            terrain = extractor.get_terrain(x, y)
            avg = checker.get_average_z(x, y)

            print(f"\nPosition ({x}, {y}):")
            print(f"  Terrain tile: {terrain.tile_id if terrain else 'None'}, raw Z: {terrain.z if terrain else 'N/A'}")
            print(f"  Average Z: low={avg.low_z}, avg={avg.avg_z}, top={avg.top_z}")

            # Check walkability at average Z
            result = checker.can_fit_detailed(x, y, avg.avg_z)
            print(f"  Can fit at Z={avg.avg_z}: {result.can_fit}")
            print(f"    Has surface: {result.has_surface}")
            print(f"    Surface Z: {result.surface_z}")
            if result.blocking_reason:
                print(f"    Blocking reason: {result.blocking_reason}")

            # Check statics
            statics = extractor.get_statics(x, y)
            if statics:
                print(f"  Statics at position: {len(statics)}")
                for s in statics[:3]:
                    flags = checker.tiledata.get_static_flags(s.tile_id)
                    height = checker.tiledata.get_static_height(s.tile_id)
                    print(f"    - ID={s.tile_id}, Z={s.z}, height={height}, impass={bool(flags & TileFlags.Impassable)}")

            # Find valid Z
            valid_z = checker.find_valid_z(x, y, avg.avg_z)
            if valid_z is not None:
                print(f"  Valid Z found: {valid_z}")
            else:
                print(f"  No valid Z found in range!")

        # Scan a small area for walkability stats
        print("\n--- Area Walkability Scan ---")
        scan_x1, scan_y1 = 5390, 40
        scan_x2, scan_y2 = 5410, 60

        total = 0
        walkable = 0
        blocked_by_terrain = 0
        blocked_by_static = 0

        for y in range(scan_y1, scan_y2 + 1):
            for x in range(scan_x1, scan_x2 + 1):
                total += 1
                avg = checker.get_average_z(x, y)
                result = checker.can_fit_detailed(x, y, avg.avg_z)

                if result.can_fit:
                    walkable += 1
                elif result.blocking_reason and "land" in result.blocking_reason.lower():
                    blocked_by_terrain += 1
                elif result.blocking_reason and "static" in result.blocking_reason.lower():
                    blocked_by_static += 1

        print(f"Scanned area: ({scan_x1}, {scan_y1}) to ({scan_x2}, {scan_y2})")
        print(f"Total tiles: {total}")
        print(f"Walkable: {walkable} ({walkable/total*100:.1f}%)")
        print(f"Blocked by terrain: {blocked_by_terrain} ({blocked_by_terrain/total*100:.1f}%)")
        print(f"Blocked by statics: {blocked_by_static} ({blocked_by_static/total*100:.1f}%)")

        print("\n" + "=" * 80)
        print("TEST COMPLETE")
        print("=" * 80)

    except FileNotFoundError as e:
        print(f"\nError: {e}")
        sys.exit(1)


if __name__ == "__main__":
    main()
