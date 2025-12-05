"""
Compare OSI T2A map0 to ServUO map0
Compares the original T2A installation map to ServUO's current map content.
"""

import struct
import json
import os
from pathlib import Path
from dataclasses import dataclass, asdict
from typing import List, Dict, Set, Tuple, Optional
from collections import defaultdict

@dataclass
class StaticItem:
    tile_id: int
    x: int
    y: int
    z: int
    hue: int = 0
    
    def location_key(self) -> Tuple[int, int, int, int]:
        """Return a unique key for this static's location"""
        return (self.x, self.y, self.z, self.tile_id)

@dataclass
class TerrainTile:
    tile_id: int
    z: int

@dataclass
class MapInfo:
    """Information about a map file"""
    path: Path
    width: int
    height: int
    file_size: int
    exists: bool

@dataclass
class ComparisonResult:
    osi_map_info: MapInfo
    servuo_map_info: MapInfo
    osi_total_statics: int
    servuo_total_statics: int
    osi_total_terrain: int
    servuo_total_terrain: int
    osi_categories: Dict[str, int]
    servuo_categories: Dict[str, int]
    differences: Dict[str, int]
    unique_to_osi: int
    unique_to_servuo: int
    common_locations: int
    terrain_differences: int
    dimension_match: bool

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
        
        self.width_blocks = map_width // 8
        self.height_blocks = map_height // 8
        
    def get_map_info(self) -> MapInfo:
        """Get information about the map files"""
        exists = self.map_path.exists()
        file_size = self.map_path.stat().st_size if exists else 0
        return MapInfo(
            path=self.map_path,
            width=self.map_width,
            height=self.map_height,
            file_size=file_size,
            exists=exists
        )
    
    def read_terrain_block(self, block_x: int, block_y: int) -> List[TerrainTile]:
        """Read terrain for an 8x8 block"""
        terrain = []
        block_id = block_x * self.height_blocks + block_y
        
        try:
            with open(self.map_path, 'rb') as f:
                # Each block is 196 bytes (64 tiles * 3 bytes + 4 bytes header)
                f.seek(block_id * 196)
                header = f.read(4)  # Skip header
                
                for _ in range(64):  # 8x8 = 64 tiles
                    tile_id = struct.unpack('<H', f.read(2))[0]
                    z = struct.unpack('b', f.read(1))[0]
                    terrain.append(TerrainTile(tile_id, z))
        except (FileNotFoundError, IOError, struct.error):
            pass
            
        return terrain
    
    def read_statics_block(self, block_x: int, block_y: int) -> List[StaticItem]:
        """Read statics for an 8x8 block"""
        statics = []
        block_id = block_x * self.height_blocks + block_y
        
        try:
            with open(self.staidx_path, 'rb') as idx:
                idx.seek(block_id * 12)
                offset = struct.unpack('<I', idx.read(4))[0]
                length = struct.unpack('<I', idx.read(4))[0]
                
                if offset == 0xFFFFFFFF or length == 0:
                    return statics
        except (FileNotFoundError, IOError, struct.error):
            return statics
        
        try:
            with open(self.statics_path, 'rb') as sta:
                sta.seek(offset)
                count = length // 7
                
                for _ in range(count):
                    tile_id = struct.unpack('<H', sta.read(2))[0]
                    x = struct.unpack('B', sta.read(1))[0]
                    y = struct.unpack('B', sta.read(1))[0]
                    z = struct.unpack('b', sta.read(1))[0]
                    hue = struct.unpack('<H', sta.read(2))[0]
                    
                    abs_x = block_x * 8 + x
                    abs_y = block_y * 8 + y
                    
                    statics.append(StaticItem(tile_id, abs_x, abs_y, z, hue))
        except (FileNotFoundError, IOError, struct.error):
            pass
            
        return statics
    
    def scan_all_statics(self, progress_callback=None) -> List[StaticItem]:
        """Scan all blocks and collect statics"""
        all_statics = []
        total_blocks = self.width_blocks * self.height_blocks
        scanned = 0
        
        for block_y in range(self.height_blocks):
            for block_x in range(self.width_blocks):
                statics = self.read_statics_block(block_x, block_y)
                all_statics.extend(statics)
                
                scanned += 1
                if scanned % 1000 == 0 and progress_callback:
                    progress_callback(scanned, total_blocks)
        
        return all_statics
    
    def scan_all_terrain(self, progress_callback=None) -> List[TerrainTile]:
        """Scan all blocks and collect terrain"""
        all_terrain = []
        total_blocks = self.width_blocks * self.height_blocks
        scanned = 0
        
        for block_y in range(self.height_blocks):
            for block_x in range(self.width_blocks):
                terrain = self.read_terrain_block(block_x, block_y)
                all_terrain.extend(terrain)
                
                scanned += 1
                if scanned % 1000 == 0 and progress_callback:
                    progress_callback(scanned, total_blocks)
        
        return all_terrain

def categorize_static(tile_id: int) -> str:
    """Categorize a static by its tile ID"""
    # Building components
    if 0x0080 <= tile_id <= 0x008F or 0x003D <= tile_id <= 0x0044:
        return "walls"
    if 0x0495 <= tile_id <= 0x04B0:
        return "floors"
    if 0x06A5 <= tile_id <= 0x06E0:
        return "roofs"
    if 0x0675 <= tile_id <= 0x0684 or tile_id in [0x0BD2, 0x0BD3, 0x0BD4, 0x0BD5]:
        return "doors"
    # Spawn indicators
    if 0x0ED3 <= tile_id <= 0x0ED6:
        return "graves"
    if 0x0ECA <= tile_id <= 0x0ED2:
        return "bones"
    if 0x122A <= tile_id <= 0x122F:
        return "shrines"
    # Important items
    if tile_id in [0x0F6C, 0x0F6D, 0x0F6E, 0x0F6F, 0x0F70, 0x0F71, 0x0F72, 0x0F73]:
        return "moongates"
    if 0x09AB <= tile_id <= 0x09B2:
        return "chests"
    if 0x0E3D <= tile_id <= 0x0E40:
        return "crates"
    if 0x0E77 <= tile_id <= 0x0E7B:
        return "barrels"
    if 0x0FAF <= tile_id <= 0x0FB2:
        return "forges_anvils"
    if 0x0BD2 <= tile_id <= 0x0BD5 or 0x0B95 <= tile_id <= 0x0B98:
        return "signs"
    # Trees
    if 0x0C9E <= tile_id <= 0x0D03 or 0x0CCA <= tile_id <= 0x0D2E:
        return "trees"
    # Rocks
    if 0x1773 <= tile_id <= 0x17DA:
        return "rocks"
    return "other"

def compare_maps(osi_reader: UOMapReader, servuo_reader: UOMapReader) -> ComparisonResult:
    """Compare OSI T2A map to ServUO map"""
    print("\n" + "="*80)
    print("EXTRACTING DATA FROM MAPS...")
    print("="*80)
    
    # Get map info
    osi_info = osi_reader.get_map_info()
    servuo_info = servuo_reader.get_map_info()
    
    # Extract statics
    print("\n[1/3] Extracting statics from OSI T2A map...")
    def progress(current, total):
        percent = (current / total) * 100
        print(f"  Progress: {current:,}/{total:,} blocks ({percent:.1f}%)", end='\r')
    
    osi_statics = osi_reader.scan_all_statics(progress_callback=progress)
    print(f"\n  Found {len(osi_statics):,} statics in OSI T2A map")
    
    print("\n[2/3] Extracting statics from ServUO map...")
    servuo_statics = servuo_reader.scan_all_statics(progress_callback=progress)
    print(f"\n  Found {len(servuo_statics):,} statics in ServUO map")
    
    # Extract terrain (sample - first 1000 blocks for speed)
    print("\n[3/3] Sampling terrain from both maps (first 1000 blocks)...")
    osi_terrain_sample = []
    servuo_terrain_sample = []
    sample_blocks = min(1000, osi_reader.width_blocks * osi_reader.height_blocks)
    scanned = 0
    for block_y in range(min(100, osi_reader.height_blocks)):
        for block_x in range(min(10, osi_reader.width_blocks)):
            osi_terrain = osi_reader.read_terrain_block(block_x, block_y)
            servuo_terrain = servuo_reader.read_terrain_block(block_x, block_y)
            osi_terrain_sample.extend(osi_terrain)
            servuo_terrain_sample.extend(servuo_terrain)
            scanned += 1
            if scanned >= sample_blocks:
                break
        if scanned >= sample_blocks:
            break
    
    print(f"  Sampled {len(osi_terrain_sample):,} terrain tiles from OSI")
    print(f"  Sampled {len(servuo_terrain_sample):,} terrain tiles from ServUO")
    
    # Categorize statics
    osi_categories = defaultdict(int)
    servuo_categories = defaultdict(int)
    
    for static in osi_statics:
        category = categorize_static(static.tile_id)
        osi_categories[category] += 1
    
    for static in servuo_statics:
        category = categorize_static(static.tile_id)
        servuo_categories[category] += 1
    
    # Create location sets for comparison
    osi_locations = {s.location_key() for s in osi_statics}
    servuo_locations = {s.location_key() for s in servuo_statics}
    
    common = osi_locations & servuo_locations
    unique_osi = osi_locations - servuo_locations
    unique_servuo = servuo_locations - osi_locations
    
    # Calculate differences by category
    differences = {}
    all_categories = set(osi_categories.keys()) | set(servuo_categories.keys())
    for category in all_categories:
        osi_count = osi_categories.get(category, 0)
        servuo_count = servuo_categories.get(category, 0)
        diff = osi_count - servuo_count
        if diff != 0:
            differences[category] = diff
    
    # Compare terrain (sample)
    terrain_differences = 0
    min_len = min(len(osi_terrain_sample), len(servuo_terrain_sample))
    for i in range(min_len):
        if osi_terrain_sample[i].tile_id != servuo_terrain_sample[i].tile_id or \
           osi_terrain_sample[i].z != servuo_terrain_sample[i].z:
            terrain_differences += 1
    
    # Check dimension match
    dimension_match = (osi_info.width == servuo_info.width and 
                     osi_info.height == servuo_info.height)
    
    return ComparisonResult(
        osi_map_info=osi_info,
        servuo_map_info=servuo_info,
        osi_total_statics=len(osi_statics),
        servuo_total_statics=len(servuo_statics),
        osi_total_terrain=osi_info.width * osi_info.height,
        servuo_total_terrain=servuo_info.width * servuo_info.height,
        osi_categories=dict(osi_categories),
        servuo_categories=dict(servuo_categories),
        differences=differences,
        unique_to_osi=len(unique_osi),
        unique_to_servuo=len(unique_servuo),
        common_locations=len(common),
        terrain_differences=terrain_differences,
        dimension_match=dimension_match
    )

def find_osi_path() -> Optional[str]:
    """Try to find the OSI T2A installation path"""
    possible_paths = [
        "C:/Program Files (x86)/Ultima Online",
        "C:/Program Files (x86)/Electronic Arts/Ultima Online Classic",
        "D:/client",
    ]
    
    for path in possible_paths:
        map_path = Path(path) / "map0.mul"
        if map_path.exists():
            return path
    
    return None

def find_servuo_path() -> Optional[str]:
    """Try to find ServUO map files"""
    possible_paths = [
        "C:/DevEnv/GIT/UO/ServUO/Data/Maps",
        "C:/DevEnv/GIT/UO/ServUO/Data",
    ]
    
    for path in possible_paths:
        map_path = Path(path) / "map0.mul"
        if map_path.exists():
            return path
    
    # Check if ServUO is using client path
    servuo_config = Path("C:/DevEnv/GIT/UO/ServUO/Config/DataPath.cfg")
    if servuo_config.exists():
        # Try to read custom path from config
        with open(servuo_config, 'r') as f:
            for line in f:
                if line.startswith('CustomPath='):
                    custom_path = line.split('=', 1)[1].strip()
                    map_path = Path(custom_path) / "map0.mul"
                    if map_path.exists():
                        return custom_path
    
    return None

def main():
    """Main comparison function"""
    import sys
    
    # Find paths
    OSI_PATH = find_osi_path()
    SERVUO_PATH = find_servuo_path()
    
    if len(sys.argv) > 1:
        OSI_PATH = sys.argv[1]
    if len(sys.argv) > 2:
        SERVUO_PATH = sys.argv[2]
    
    MAP_NUM = 0
    MAP_WIDTH = 7168  # T2A standard
    MAP_HEIGHT = 4096  # T2A standard
    
    print("="*80)
    print("OSI T2A vs SERVUO MAP COMPARISON")
    print("="*80)
    
    if not OSI_PATH:
        print("ERROR: Could not find OSI T2A installation")
        print("Please specify path: python compare_t2a_to_servuo.py <OSI_PATH> [SERVUO_PATH]")
        return 1
    
    if not SERVUO_PATH:
        print("ERROR: Could not find ServUO map files")
        print("Please specify path: python compare_t2a_to_servuo.py <OSI_PATH> <SERVUO_PATH>")
        return 1
    
    print(f"OSI T2A Path: {OSI_PATH}")
    print(f"ServUO Path: {SERVUO_PATH}")
    print(f"Map Number: {MAP_NUM}")
    print(f"Expected Size: {MAP_WIDTH} x {MAP_HEIGHT}")
    print("="*80)
    
    # Check if files exist
    osi_path = Path(OSI_PATH)
    servuo_path = Path(SERVUO_PATH)
    
    if not (osi_path / f"map{MAP_NUM}.mul").exists():
        print(f"ERROR: OSI map file not found: {osi_path / f'map{MAP_NUM}.mul'}")
        return 1
    
    servuo_map_exists = (servuo_path / f"map{MAP_NUM}.mul").exists()
    if not servuo_map_exists:
        print(f"WARNING: ServUO map file not found: {servuo_path / f'map{MAP_NUM}.mul'}")
        print(f"ServUO may be using the OSI client path directly.")
        print(f"Will analyze OSI T2A map only and show what ServUO would use if configured.")
        print()
    
    # Create readers
    osi_reader = UOMapReader(OSI_PATH, MAP_NUM, MAP_WIDTH, MAP_HEIGHT)
    
    if servuo_map_exists:
        servuo_reader = UOMapReader(SERVUO_PATH, MAP_NUM, MAP_WIDTH, MAP_HEIGHT)
        # Compare
        result = compare_maps(osi_reader, servuo_reader)
    else:
        # ServUO doesn't have its own maps, so analyze OSI only
        print("\n" + "="*80)
        print("ANALYZING OSI T2A MAP (ServUO would use this if configured)")
        print("="*80)
        
        def progress(current, total):
            percent = (current / total) * 100
            print(f"  Progress: {current:,}/{total:,} blocks ({percent:.1f}%)", end='\r')
        
        print("\nExtracting statics from OSI T2A map...")
        osi_statics = osi_reader.scan_all_statics(progress_callback=progress)
        print(f"\n  Found {len(osi_statics):,} statics in OSI T2A map")
        
        # Categorize
        osi_categories = defaultdict(int)
        for static in osi_statics:
            category = categorize_static(static.tile_id)
            osi_categories[category] += 1
        
        osi_info = osi_reader.get_map_info()
        
        # Create a dummy result for display
        result = ComparisonResult(
            osi_map_info=osi_info,
            servuo_map_info=MapInfo(
                path=Path(SERVUO_PATH) / f"map{MAP_NUM}.mul",
                width=MAP_WIDTH,
                height=MAP_HEIGHT,
                file_size=0,
                exists=False
            ),
            osi_total_statics=len(osi_statics),
            servuo_total_statics=0,
            osi_total_terrain=osi_info.width * osi_info.height,
            servuo_total_terrain=0,
            osi_categories=dict(osi_categories),
            servuo_categories={},
            differences={},
            unique_to_osi=len(osi_statics),
            unique_to_servuo=0,
            common_locations=0,
            terrain_differences=0,
            dimension_match=False
        )
    
    # Print results
    print("\n" + "="*80)
    print("COMPARISON RESULTS")
    print("="*80)
    
    print("\nMAP FILE INFORMATION:")
    print(f"  OSI T2A:")
    print(f"    Path: {result.osi_map_info.path}")
    print(f"    Size: {result.osi_map_info.width} x {result.osi_map_info.height}")
    print(f"    File Size: {result.osi_map_info.file_size / (1024*1024):.2f} MB")
    print(f"    Exists: {result.osi_map_info.exists}")
    print(f"  ServUO:")
    print(f"    Path: {result.servuo_map_info.path}")
    print(f"    Size: {result.servuo_map_info.width} x {result.servuo_map_info.height}")
    print(f"    File Size: {result.servuo_map_info.file_size / (1024*1024):.2f} MB")
    print(f"    Exists: {result.servuo_map_info.exists}")
    print(f"  Dimensions Match: {result.dimension_match}")
    
    print("\n" + "="*80)
    print("STATICS COMPARISON")
    print("="*80)
    print(f"OSI T2A Total Statics:     {result.osi_total_statics:,}")
    if result.servuo_map_info.exists:
        print(f"ServUO Total Statics:      {result.servuo_total_statics:,}")
        print(f"Difference:                 {result.osi_total_statics - result.servuo_total_statics:,}")
        print(f"\nCommon Locations:          {result.common_locations:,}")
        print(f"Unique to OSI T2A:         {result.unique_to_osi:,}")
        print(f"Unique to ServUO:          {result.unique_to_servuo:,}")
    else:
        print(f"ServUO:                    Not configured (would use OSI T2A map if configured)")
        print(f"\nNote: ServUO doesn't have its own map files.")
        print(f"      If configured to use OSI client path, ServUO would use the same map.")
    
    print("\n" + "="*80)
    print("CATEGORY BREAKDOWN (OSI T2A)")
    print("="*80)
    if result.servuo_map_info.exists:
        print(f"{'Category':<20} {'OSI T2A':<12} {'ServUO':<12} {'Difference':<12}")
        print("-"*80)
        all_categories = sorted(set(result.osi_categories.keys()) | set(result.servuo_categories.keys()))
        for category in all_categories:
            osi_count = result.osi_categories.get(category, 0)
            servuo_count = result.servuo_categories.get(category, 0)
            diff = osi_count - servuo_count
            diff_str = f"{diff:+,}" if diff != 0 else "0"
            print(f"{category:<20} {osi_count:<12,} {servuo_count:<12,} {diff_str:<12}")
    else:
        print(f"{'Category':<20} {'Count':<12}")
        print("-"*80)
        all_categories = sorted(result.osi_categories.keys())
        for category in all_categories:
            count = result.osi_categories.get(category, 0)
            print(f"{category:<20} {count:<12,}")
    
    print("\n" + "="*80)
    print("TERRAIN COMPARISON (Sample)")
    print("="*80)
    print(f"Terrain Differences (sampled): {result.terrain_differences:,}")
    
    # Export results
    output_dir = Path("Vystia Town Generator/comparison_results")
    output_dir.mkdir(parents=True, exist_ok=True)
    
    comparison_data = asdict(result)
    # Convert Path objects to strings for JSON
    comparison_data['osi_map_info']['path'] = str(comparison_data['osi_map_info']['path'])
    comparison_data['servuo_map_info']['path'] = str(comparison_data['servuo_map_info']['path'])
    
    with open(output_dir / "t2a_servuo_comparison.json", 'w') as f:
        json.dump(comparison_data, f, indent=2)
    
    print("\n" + "="*80)
    print(f"Comparison data saved to: {output_dir / 't2a_servuo_comparison.json'}")
    print("="*80)
    
    return 0

if __name__ == "__main__":
    exit(main())

