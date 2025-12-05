"""
Continent City Scanner
Scans entire UO maps to automatically detect cities by analyzing static density and building patterns.
"""

import struct
import json
from pathlib import Path
from dataclasses import dataclass, asdict
from typing import List, Dict, Tuple, Set
from collections import defaultdict
import math

# Building tile ID ranges (common building components)
BUILDING_TILES = {
    # Floors
    'floors': set(range(0x0495, 0x04B0)),  # Stone floors, wood floors
    # Walls (common wall IDs)
    'walls': set([0x0080, 0x0081, 0x0082, 0x0083, 0x0084, 0x0085, 0x0086, 0x0087,
                  0x0088, 0x0089, 0x008A, 0x008B, 0x008C, 0x008D, 0x008E, 0x008F,
                  0x003D, 0x003E, 0x003F, 0x0040, 0x0041, 0x0042, 0x0043, 0x0044]),
    # Doors
    'doors': set([0x0675, 0x0676, 0x0677, 0x0678, 0x0679, 0x067A, 0x067B, 0x067C,
                  0x067D, 0x067E, 0x067F, 0x0680, 0x0681, 0x0682, 0x0683, 0x0684,
                  0x0BD2, 0x0BD3, 0x0BD4, 0x0BD5]),
    # Roofs
    'roofs': set(range(0x06A5, 0x06C0)) | set(range(0x06C1, 0x06E0)),
    # Signs
    'signs': set([0x0BD2, 0x0BD3, 0x0BD4, 0x0BD5, 0x0B95, 0x0B96, 0x0B97, 0x0B98]),
    # Windows
    'windows': set(range(0x0063, 0x0067)) | set(range(0x0068, 0x006C)),
}

ALL_BUILDING_TILES = set()
for tile_set in BUILDING_TILES.values():
    ALL_BUILDING_TILES.update(tile_set)

@dataclass
class CityCandidate:
    """Represents a detected city"""
    name: str
    center_x: int
    center_y: int
    min_x: int
    min_y: int
    max_x: int
    max_y: int
    static_count: int
    building_tile_count: int
    block_count: int
    density: float  # statics per block
    building_ratio: float  # building tiles / total statics
    confidence: float  # 0.0 to 1.0

class UOMapReader:
    """Reads UO .mul files"""
    
    def __init__(self, uo_path: str, map_num: int = 0, map_width: int = 7168, map_height: int = 4096):
        self.uo_path = Path(uo_path)
        self.map_num = map_num
        self.map_width = map_width
        self.map_height = map_height
        self.map_path = self.uo_path / f"map{map_num}.mul"
        self.statics_path = self.uo_path / f"statics{map_num}.mul"
        self.staidx_path = self.uo_path / f"staidx{map_num}.mul"
        
        # Calculate block dimensions
        self.width_blocks = map_width // 8
        self.height_blocks = map_height // 8
        
    def read_statics_block(self, block_x: int, block_y: int) -> List[Tuple[int, int, int, int]]:
        """Read statics for an 8x8 block. Returns list of (tile_id, x, y, z)"""
        statics = []
        
        # Calculate block ID - CORRECT formula for 7168x4096 map
        # blockID = blockX * height_blocks + blockY
        block_id = block_x * self.height_blocks + block_y
        
        # Read index entry
        try:
            with open(self.staidx_path, 'rb') as idx:
                idx.seek(block_id * 12)
                offset = struct.unpack('<I', idx.read(4))[0]
                length = struct.unpack('<I', idx.read(4))[0]
                
                if offset == 0xFFFFFFFF or length == 0:
                    return statics  # No statics in this block
        except (FileNotFoundError, IOError):
            return statics
        
        # Read statics data
        try:
            with open(self.statics_path, 'rb') as sta:
                sta.seek(offset)
                count = length // 7  # Each static entry is 7 bytes
                
                for _ in range(count):
                    tile_id = struct.unpack('<H', sta.read(2))[0]
                    x = struct.unpack('B', sta.read(1))[0]
                    y = struct.unpack('B', sta.read(1))[0]
                    z = struct.unpack('b', sta.read(1))[0]
                    hue = struct.unpack('<H', sta.read(2))[0]
                    
                    abs_x = block_x * 8 + x
                    abs_y = block_y * 8 + y
                    
                    statics.append((tile_id, abs_x, abs_y, z))
        except (FileNotFoundError, IOError):
            pass
            
        return statics

class CityScanner:
    """Scans map for cities by analyzing static density"""
    
    def __init__(self, map_reader: UOMapReader):
        self.reader = map_reader
        self.block_density = {}  # (block_x, block_y) -> static_count
        self.block_buildings = {}  # (block_x, block_y) -> building_tile_count
        
    def scan_all_blocks(self, progress_callback=None):
        """Scan all blocks on the map"""
        print(f"Scanning {self.reader.width_blocks} x {self.reader.height_blocks} blocks...")
        total_blocks = self.reader.width_blocks * self.reader.height_blocks
        scanned = 0
        
        for block_y in range(self.reader.height_blocks):
            for block_x in range(self.reader.width_blocks):
                statics = self.reader.read_statics_block(block_x, block_y)
                
                static_count = len(statics)
                building_count = sum(1 for tile_id, _, _, _ in statics if tile_id in ALL_BUILDING_TILES)
                
                if static_count > 0:
                    self.block_density[(block_x, block_y)] = static_count
                    self.block_buildings[(block_x, block_y)] = building_count
                
                scanned += 1
                if scanned % 1000 == 0 and progress_callback:
                    progress_callback(scanned, total_blocks)
        
        print(f"Scanned {scanned} blocks. Found {len(self.block_density)} blocks with statics.")
    
    def find_city_clusters(self, min_density: int = 20, min_blocks: int = 10, 
                          min_building_ratio: float = 0.1) -> List[CityCandidate]:
        """
        Find clusters of high-density blocks that represent cities.
        
        Args:
            min_density: Minimum statics per block to consider
            min_blocks: Minimum number of blocks in a cluster
            min_building_ratio: Minimum ratio of building tiles to total statics
        """
        # Filter blocks by density and building ratio
        candidate_blocks = {}
        for (block_x, block_y), static_count in self.block_density.items():
            building_count = self.block_buildings.get((block_x, block_y), 0)
            building_ratio = building_count / static_count if static_count > 0 else 0
            
            if static_count >= min_density and building_ratio >= min_building_ratio:
                candidate_blocks[(block_x, block_y)] = (static_count, building_count)
        
        print(f"Found {len(candidate_blocks)} candidate blocks (density >= {min_density}, building_ratio >= {min_building_ratio})")
        
        # Cluster nearby blocks
        clusters = self._cluster_blocks(candidate_blocks.keys())
        
        # Convert clusters to CityCandidate objects
        cities = []
        for i, cluster in enumerate(clusters):
            if len(cluster) < min_blocks:
                continue
            
            # Calculate bounds
            block_coords = list(cluster)
            min_x = min(bx for bx, by in block_coords) * 8
            min_y = min(by for bx, by in block_coords) * 8
            max_x = (max(bx for bx, by in block_coords) + 1) * 8
            max_y = (max(by for bx, by in block_coords) + 1) * 8
            
            # Calculate statistics
            total_statics = sum(self.block_density[(bx, by)] for bx, by in block_coords)
            total_buildings = sum(self.block_buildings.get((bx, by), 0) for bx, by in block_coords)
            density = total_statics / len(cluster) if cluster else 0
            building_ratio = total_buildings / total_statics if total_statics > 0 else 0
            
            center_x = (min_x + max_x) // 2
            center_y = (min_y + max_y) // 2
            
            # Calculate confidence score
            confidence = min(1.0, (density / 100.0) * (building_ratio / 0.3) * (len(cluster) / 50.0))
            
            city = CityCandidate(
                name=f"city_{i+1}",
                center_x=center_x,
                center_y=center_y,
                min_x=min_x,
                min_y=min_y,
                max_x=max_x,
                max_y=max_y,
                static_count=total_statics,
                building_tile_count=total_buildings,
                block_count=len(cluster),
                density=density,
                building_ratio=building_ratio,
                confidence=confidence
            )
            cities.append(city)
        
        # Sort by confidence
        cities.sort(key=lambda c: c.confidence, reverse=True)
        
        return cities
    
    def _cluster_blocks(self, blocks: Set[Tuple[int, int]], max_distance: int = 3) -> List[List[Tuple[int, int]]]:
        """Cluster nearby blocks together"""
        clusters = []
        unprocessed = set(blocks)
        
        while unprocessed:
            # Start new cluster with first unprocessed block
            seed = unprocessed.pop()
            cluster = [seed]
            queue = [seed]
            
            # Expand cluster by finding nearby blocks
            while queue:
                current = queue.pop(0)
                bx, by = current
                
                # Check all blocks within max_distance
                for dx in range(-max_distance, max_distance + 1):
                    for dy in range(-max_distance, max_distance + 1):
                        if dx == 0 and dy == 0:
                            continue
                        
                        neighbor = (bx + dx, by + dy)
                        if neighbor in unprocessed:
                            unprocessed.remove(neighbor)
                            cluster.append(neighbor)
                            queue.append(neighbor)
            
            clusters.append(cluster)
        
        return clusters
    
    def export_cities(self, cities: List[CityCandidate], output_file: str):
        """Export detected cities to JSON"""
        output_path = Path(output_file)
        output_path.parent.mkdir(parents=True, exist_ok=True)
        
        data = {
            'scan_parameters': {
                'map_num': self.reader.map_num,
                'map_width': self.reader.map_width,
                'map_height': self.reader.map_height,
            },
            'cities': [asdict(city) for city in cities],
            'summary': {
                'total_cities': len(cities),
                'total_statics': sum(c.static_count for c in cities),
                'avg_density': sum(c.density for c in cities) / len(cities) if cities else 0,
            }
        }
        
        with open(output_path, 'w') as f:
            json.dump(data, f, indent=2)
        
        print(f"\nExported {len(cities)} cities to {output_path}")
    
    def print_summary(self, cities: List[CityCandidate]):
        """Print summary of detected cities"""
        print("\n" + "="*80)
        print("DETECTED CITIES SUMMARY")
        print("="*80)
        print(f"{'Name':<15} {'Center':<15} {'Size':<15} {'Statics':<10} {'Density':<10} {'Build%':<10} {'Conf':<10}")
        print("-"*80)
        
        for city in cities:
            size = f"{(city.max_x - city.min_x)}x{(city.max_y - city.min_y)}"
            center = f"({city.center_x},{city.center_y})"
            density_str = f"{city.density:.1f}"
            build_pct = f"{city.building_ratio*100:.1f}%"
            conf_str = f"{city.confidence:.2f}"
            print(f"{city.name:<15} {center:<15} {size:<15} {city.static_count:<10} "
                  f"{density_str:<10} {build_pct:<10} {conf_str:<10}")
        
        print("="*80)
        print(f"Total cities detected: {len(cities)}")
        print(f"Total statics: {sum(c.static_count for c in cities):,}")

def main():
    """Main scanning function"""
    import sys
    
    # Common UO installation paths to try
    COMMON_PATHS = [
        "C:/Program Files (x86)/Electronic Arts/Ultima Online Classic",
        "C:/Ultima Online",
        "C:/Program Files/Electronic Arts/Ultima Online Classic",
        "C:/DevEnv/GIT/UO/ClassicUO/ClientData",
        "C:/DevEnv/GIT/UO/UOL 1.5",
        "C:/DevEnv/GIT/UO/ServUO/Data/Maps",
    ]
    
    # Configuration
    UO_PATH = None
    if len(sys.argv) > 1:
        UO_PATH = sys.argv[1]
    else:
        # Try common paths
        for path in COMMON_PATHS:
            test_path = Path(path)
            if (test_path / "map0.mul").exists():
                UO_PATH = str(test_path)
                print(f"Auto-detected UO path: {UO_PATH}")
                break
    
    if not UO_PATH:
        print("ERROR: Could not find UO map files.")
        print("Please specify the path to your UO installation:")
        print(f"Usage: python scan_continents_for_cities.py [UO_PATH] [MAP_NUM] [WIDTH] [HEIGHT]")
        print(f"Example: python scan_continents_for_cities.py \"C:/UO\" 0 7168 4096")
        print(f"\nTried paths:")
        for path in COMMON_PATHS:
            print(f"  - {path}")
        return 1
    
    MAP_NUM = 0
    if len(sys.argv) > 2:
        MAP_NUM = int(sys.argv[2])
    
    MAP_WIDTH = 7168
    MAP_HEIGHT = 4096
    if len(sys.argv) > 4:
        MAP_WIDTH = int(sys.argv[3])
        MAP_HEIGHT = int(sys.argv[4])
    
    # Check if files exist
    uo_path = Path(UO_PATH)
    if not (uo_path / f"map{MAP_NUM}.mul").exists():
        print(f"ERROR: Map file not found: {uo_path / f'map{MAP_NUM}.mul'}")
        print(f"Usage: python scan_continents_for_cities.py [UO_PATH] [MAP_NUM] [WIDTH] [HEIGHT]")
        print(f"Example: python scan_continents_for_cities.py \"C:/UO\" 0 7168 4096")
        return 1
    
    print("="*80)
    print("UO CONTINENT CITY SCANNER")
    print("="*80)
    print(f"Map Path: {UO_PATH}")
    print(f"Map Number: {MAP_NUM}")
    print(f"Map Size: {MAP_WIDTH} x {MAP_HEIGHT}")
    print("="*80)
    
    # Create reader and scanner
    reader = UOMapReader(UO_PATH, MAP_NUM, MAP_WIDTH, MAP_HEIGHT)
    scanner = CityScanner(reader)
    
    # Scan all blocks
    def progress(current, total):
        percent = (current / total) * 100
        print(f"Progress: {current:,}/{total:,} blocks ({percent:.1f}%)", end='\r')
    
    scanner.scan_all_blocks(progress_callback=progress)
    print()  # New line after progress
    
    # Find cities
    print("\nAnalyzing clusters...")
    cities = scanner.find_city_clusters(
        min_density=20,      # At least 20 statics per block
        min_blocks=10,        # At least 10 blocks (80x80 tiles minimum)
        min_building_ratio=0.1  # At least 10% building tiles
    )
    
    # Print summary
    scanner.print_summary(cities)
    
    # Export to JSON
    output_file = Path("Vystia Town Generator") / "scanned_cities" / f"map{MAP_NUM}_cities.json"
    scanner.export_cities(cities, str(output_file))
    
    # Also export as extractable format (compatible with uo_town_extractor)
    extractable_file = Path("Vystia Town Generator") / "scanned_cities" / f"map{MAP_NUM}_extractable.json"
    extractable_data = {
        'towns': {}
    }
    
    for i, city in enumerate(cities):
        town_name = f"scanned_city_{i+1}"
        extractable_data['towns'][town_name] = (
            city.min_x, city.min_y, city.max_x, city.max_y
        )
    
    with open(extractable_file, 'w') as f:
        json.dump(extractable_data, f, indent=2)
    
    print(f"Extractable format saved to {extractable_file}")
    
    return 0

if __name__ == "__main__":
    exit(main())

