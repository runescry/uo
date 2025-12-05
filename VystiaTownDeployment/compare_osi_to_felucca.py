"""
Compare OSI Original Map to Server's Felucca Map
Extracts statics from both maps and compares them to see differences.
"""

import struct
import json
from pathlib import Path
from dataclasses import dataclass, asdict
from typing import List, Dict, Set
from collections import defaultdict

@dataclass
class StaticItem:
    tile_id: int
    x: int
    y: int
    z: int
    hue: int = 0

@dataclass
class ComparisonResult:
    osi_total: int
    felucca_total: int
    osi_categories: Dict[str, int]
    felucca_categories: Dict[str, int]
    differences: Dict[str, int]
    unique_to_osi: int
    unique_to_felucca: int
    common_locations: int

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
        except (FileNotFoundError, IOError):
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
        except (FileNotFoundError, IOError):
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

def compare_maps(osi_statics: List[StaticItem], felucca_statics: List[StaticItem]) -> ComparisonResult:
    """Compare two sets of statics"""
    # Categorize statics
    osi_categories = defaultdict(int)
    felucca_categories = defaultdict(int)
    
    for static in osi_statics:
        category = categorize_static(static.tile_id)
        osi_categories[category] += 1
    
    for static in felucca_statics:
        category = categorize_static(static.tile_id)
        felucca_categories[category] += 1
    
    # Create location sets for comparison
    osi_locations = {(s.x, s.y, s.z, s.tile_id) for s in osi_statics}
    felucca_locations = {(s.x, s.y, s.z, s.tile_id) for s in felucca_statics}
    
    common = osi_locations & felucca_locations
    unique_osi = osi_locations - felucca_locations
    unique_felucca = felucca_locations - osi_locations
    
    # Calculate differences by category
    differences = {}
    all_categories = set(osi_categories.keys()) | set(felucca_categories.keys())
    for category in all_categories:
        osi_count = osi_categories.get(category, 0)
        felucca_count = felucca_categories.get(category, 0)
        diff = osi_count - felucca_count
        if diff != 0:
            differences[category] = diff
    
    return ComparisonResult(
        osi_total=len(osi_statics),
        felucca_total=len(felucca_statics),
        osi_categories=dict(osi_categories),
        felucca_categories=dict(felucca_categories),
        differences=differences,
        unique_to_osi=len(unique_osi),
        unique_to_felucca=len(unique_felucca),
        common_locations=len(common)
    )

def main():
    """Main comparison function"""
    import sys
    
    # Paths
    OSI_PATH = "C:/Program Files (x86)/Ultima Online"
    FELUCCA_PATH = "C:/Program Files (x86)/Ultima Online"  # Same for now, but can be different
    
    if len(sys.argv) > 1:
        OSI_PATH = sys.argv[1]
    if len(sys.argv) > 2:
        FELUCCA_PATH = sys.argv[2]
    
    MAP_NUM = 0
    MAP_WIDTH = 7168
    MAP_HEIGHT = 4096
    
    print("="*80)
    print("OSI vs FELUCCA MAP COMPARISON")
    print("="*80)
    print(f"OSI Map Path: {OSI_PATH}")
    print(f"Felucca Map Path: {FELUCCA_PATH}")
    print(f"Map Size: {MAP_WIDTH} x {MAP_HEIGHT}")
    print("="*80)
    
    # Check if files exist
    osi_path = Path(OSI_PATH)
    felucca_path = Path(FELUCCA_PATH)
    
    if not (osi_path / f"map{MAP_NUM}.mul").exists():
        print(f"ERROR: OSI map file not found: {osi_path / f'map{MAP_NUM}.mul'}")
        return 1
    
    if not (felucca_path / f"map{MAP_NUM}.mul").exists():
        print(f"ERROR: Felucca map file not found: {felucca_path / f'map{MAP_NUM}.mul'}")
        return 1
    
    # Extract OSI statics
    print("\n[1/2] Extracting statics from OSI map...")
    osi_reader = UOMapReader(OSI_PATH, MAP_NUM, MAP_WIDTH, MAP_HEIGHT)
    def progress(current, total):
        percent = (current / total) * 100
        print(f"  Progress: {current:,}/{total:,} blocks ({percent:.1f}%)", end='\r')
    
    osi_statics = osi_reader.scan_all_statics(progress_callback=progress)
    print(f"\n  Found {len(osi_statics):,} statics in OSI map")
    
    # Extract Felucca statics
    print("\n[2/2] Extracting statics from Felucca map...")
    felucca_reader = UOMapReader(FELUCCA_PATH, MAP_NUM, MAP_WIDTH, MAP_HEIGHT)
    felucca_statics = felucca_reader.scan_all_statics(progress_callback=progress)
    print(f"\n  Found {len(felucca_statics):,} statics in Felucca map")
    
    # Compare
    print("\n" + "="*80)
    print("COMPARING MAPS...")
    print("="*80)
    result = compare_maps(osi_statics, felucca_statics)
    
    # Print comparison
    print("\n" + "="*80)
    print("COMPARISON RESULTS")
    print("="*80)
    print(f"OSI Total Statics:     {result.osi_total:,}")
    print(f"Felucca Total Statics:  {result.felucca_total:,}")
    print(f"Difference:            {result.osi_total - result.felucca_total:,}")
    print(f"\nCommon Locations:      {result.common_locations:,}")
    print(f"Unique to OSI:         {result.unique_to_osi:,}")
    print(f"Unique to Felucca:      {result.unique_to_felucca:,}")
    
    print("\n" + "="*80)
    print("CATEGORY COMPARISON")
    print("="*80)
    print(f"{'Category':<20} {'OSI':<12} {'Felucca':<12} {'Difference':<12}")
    print("-"*80)
    
    all_categories = sorted(set(result.osi_categories.keys()) | set(result.felucca_categories.keys()))
    for category in all_categories:
        osi_count = result.osi_categories.get(category, 0)
        felucca_count = result.felucca_categories.get(category, 0)
        diff = osi_count - felucca_count
        diff_str = f"{diff:+,}" if diff != 0 else "0"
        print(f"{category:<20} {osi_count:<12,} {felucca_count:<12,} {diff_str:<12}")
    
    # Export results
    output_dir = Path("Vystia Town Generator/extracted_osi_data")
    output_dir.mkdir(parents=True, exist_ok=True)
    
    comparison_data = {
        'osi_total': result.osi_total,
        'felucca_total': result.felucca_total,
        'osi_categories': result.osi_categories,
        'felucca_categories': result.felucca_categories,
        'differences': result.differences,
        'unique_to_osi': result.unique_to_osi,
        'unique_to_felucca': result.unique_to_felucca,
        'common_locations': result.common_locations,
    }
    
    with open(output_dir / "osi_felucca_comparison.json", 'w') as f:
        json.dump(comparison_data, f, indent=2)
    
    print("\n" + "="*80)
    print(f"Comparison data saved to: {output_dir / 'osi_felucca_comparison.json'}")
    print("="*80)
    
    return 0

if __name__ == "__main__":
    exit(main())

