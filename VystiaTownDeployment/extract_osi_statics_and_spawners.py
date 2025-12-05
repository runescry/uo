"""
Extract Statics and Identify Potential Spawner Locations from Original OSI Map
Scans the original UO map files to extract all statics and identify areas that likely have spawners.
"""

import struct
import json
from pathlib import Path
from dataclasses import dataclass, asdict
from typing import List, Dict, Set
from collections import defaultdict

# Known spawner-related static IDs (these are items that might indicate spawn areas)
# Note: Actual spawners (0x1f13) are server-side items, not in map files
# But we can look for static items that commonly appear near spawners

SPAWNER_INDICATORS = {
    # Dungeon markers
    'graves': set([0x0ED3, 0x0ED4, 0x0ED5, 0x0ED6]),  # Graves
    'bones': set([0x0ECA, 0x0ECB, 0x0ECC, 0x0ECD, 0x0ECE, 0x0ECF, 0x0ED0, 0x0ED1, 0x0ED2]),  # Bones
    'skulls': set([0x0ECA, 0x0ECB]),  # Skulls
    'cages': set([0x1A97, 0x1A98, 0x1A99, 0x1A9A]),  # Cages
    'barrels': set([0x0E77, 0x0E78, 0x0E7A, 0x0E7B]),  # Barrels
    'crates': set([0x0E3D, 0x0E3E, 0x0E3F, 0x0E40]),  # Crates
    'chests': set([0x09AB, 0x09AC, 0x09AD, 0x09AE, 0x09AF, 0x09B0, 0x09B1, 0x09B2]),  # Chests
}

# Important static categories
IMPORTANT_STATICS = {
    'moongates': set([0x0F6C, 0x0F6D, 0x0F6E, 0x0F6F, 0x0F70, 0x0F71, 0x0F72, 0x0F73]),  # Moongates
    'shrines': set([0x122A, 0x122B, 0x122C, 0x122D, 0x122E, 0x122F]),  # Shrines
    'banks': set([0x0E10, 0x0E11, 0x0E12, 0x0E13, 0x0E14, 0x0E15]),  # Bank items
    'forges': set([0x0FB1, 0x0FB2]),  # Forges
    'anvils': set([0x0FAF, 0x0FB0]),  # Anvils
    'signs': set([0x0BD2, 0x0BD3, 0x0BD4, 0x0BD5, 0x0B95, 0x0B96, 0x0B97, 0x0B98]),  # Signs
}

@dataclass
class StaticItem:
    tile_id: int
    x: int
    y: int
    z: int
    hue: int = 0
    category: str = "unknown"

@dataclass
class SpawnArea:
    center_x: int
    center_y: int
    radius: int
    static_count: int
    indicator_count: int
    indicators: Dict[str, int]
    confidence: float

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
                    
                    # Categorize static
                    category = self._categorize_static(tile_id)
                    
                    statics.append(StaticItem(tile_id, abs_x, abs_y, z, hue, category))
        except (FileNotFoundError, IOError):
            pass
            
        return statics
    
    def _categorize_static(self, tile_id: int) -> str:
        """Categorize a static by its tile ID"""
        for category, tile_set in SPAWNER_INDICATORS.items():
            if tile_id in tile_set:
                return category
        for category, tile_set in IMPORTANT_STATICS.items():
            if tile_id in tile_set:
                return category
        return "other"
    
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

class SpawnAreaFinder:
    """Finds potential spawner locations based on static patterns"""
    
    def __init__(self, statics: List[StaticItem]):
        self.statics = statics
        self.spawn_indicators = set()
        for tile_set in SPAWNER_INDICATORS.values():
            self.spawn_indicators.update(tile_set)
    
    def find_spawn_areas(self, min_indicators: int = 5, radius: int = 20) -> List[SpawnArea]:
        """Find areas with high concentration of spawn indicators"""
        # Group statics by location
        static_grid = defaultdict(list)
        for static in self.statics:
            if static.tile_id in self.spawn_indicators:
                # Round to grid cells
                grid_x = static.x // radius
                grid_y = static.y // radius
                static_grid[(grid_x, grid_y)].append(static)
        
        # Find clusters
        spawn_areas = []
        for (grid_x, grid_y), indicators in static_grid.items():
            if len(indicators) >= min_indicators:
                # Calculate center and count all statics in area
                center_x = sum(s.x for s in indicators) // len(indicators)
                center_y = sum(s.y for s in indicators) // len(indicators)
                
                # Count all statics in radius
                nearby_statics = [s for s in self.statics 
                                if abs(s.x - center_x) <= radius and abs(s.y - center_y) <= radius]
                
                # Count indicators by category
                indicator_counts = defaultdict(int)
                for static in nearby_statics:
                    if static.category != "other" and static.category != "unknown":
                        indicator_counts[static.category] += 1
                
                # Calculate confidence
                confidence = min(1.0, len(indicators) / 20.0)
                
                area = SpawnArea(
                    center_x=center_x,
                    center_y=center_y,
                    radius=radius,
                    static_count=len(nearby_statics),
                    indicator_count=len(indicators),
                    indicators=dict(indicator_counts),
                    confidence=confidence
                )
                spawn_areas.append(area)
        
        # Sort by confidence
        spawn_areas.sort(key=lambda a: a.confidence, reverse=True)
        return spawn_areas

def main():
    """Main extraction function"""
    import sys
    
    # Default to original OSI installation
    UO_PATH = "C:/Program Files (x86)/Ultima Online"
    if len(sys.argv) > 1:
        UO_PATH = sys.argv[1]
    
    MAP_NUM = 0
    MAP_WIDTH = 7168
    MAP_HEIGHT = 4096
    
    print("="*80)
    print("OSI STATICS AND SPAWNER LOCATION EXTRACTOR")
    print("="*80)
    print(f"Map Path: {UO_PATH}")
    print(f"Map Number: {MAP_NUM}")
    print(f"Map Size: {MAP_WIDTH} x {MAP_HEIGHT}")
    print("="*80)
    
    # Check if files exist
    uo_path = Path(UO_PATH)
    if not (uo_path / f"map{MAP_NUM}.mul").exists():
        print(f"ERROR: Map file not found: {uo_path / f'map{MAP_NUM}.mul'}")
        print(f"Usage: python extract_osi_statics_and_spawners.py [UO_PATH]")
        return 1
    
    # Create reader
    reader = UOMapReader(UO_PATH, MAP_NUM, MAP_WIDTH, MAP_HEIGHT)
    
    # Scan all statics
    print("\nScanning all statics...")
    def progress(current, total):
        percent = (current / total) * 100
        print(f"Progress: {current:,}/{total:,} blocks ({percent:.1f}%)", end='\r')
    
    statics = reader.scan_all_statics(progress_callback=progress)
    print(f"\n\nFound {len(statics):,} total statics")
    
    # Categorize statics
    categories = defaultdict(int)
    for static in statics:
        categories[static.category] += 1
    
    print("\nStatic Categories:")
    for category, count in sorted(categories.items(), key=lambda x: x[1], reverse=True):
        print(f"  {category}: {count:,}")
    
    # Find spawn areas
    print("\nAnalyzing spawn areas...")
    finder = SpawnAreaFinder(statics)
    spawn_areas = finder.find_spawn_areas(min_indicators=5, radius=20)
    
    print(f"\nFound {len(spawn_areas)} potential spawn areas")
    
    # Export results
    output_dir = Path("Vystia Town Generator/extracted_osi_data")
    output_dir.mkdir(parents=True, exist_ok=True)
    
    # Export all statics (sample - first 100k for size)
    print("\nExporting statics data...")
    sample_size = min(100000, len(statics))
    statics_data = {
        'total_statics': len(statics),
        'sample_size': sample_size,
        'statics': [asdict(s) for s in statics[:sample_size]],
        'categories': dict(categories)
    }
    
    with open(output_dir / "osi_statics_sample.json", 'w') as f:
        json.dump(statics_data, f, indent=2)
    print(f"  Exported {sample_size:,} statics to osi_statics_sample.json")
    
    # Export spawn areas
    spawn_data = {
        'total_areas': len(spawn_areas),
        'areas': [asdict(area) for area in spawn_areas[:100]]  # Top 100
    }
    
    with open(output_dir / "osi_spawn_areas.json", 'w') as f:
        json.dump(spawn_data, f, indent=2)
    print(f"  Exported {len(spawn_areas)} spawn areas to osi_spawn_areas.json")
    
    # Print top spawn areas
    print("\n" + "="*80)
    print("TOP 20 POTENTIAL SPAWN AREAS")
    print("="*80)
    print(f"{'Location':<20} {'Indicators':<12} {'Statics':<10} {'Confidence':<12} {'Types'}")
    print("-"*80)
    
    for i, area in enumerate(spawn_areas[:20], 1):
        loc = f"({area.center_x},{area.center_y})"
        types = ", ".join([f"{k}:{v}" for k, v in list(area.indicators.items())[:3]])
        print(f"{loc:<20} {area.indicator_count:<12} {area.static_count:<10} {area.confidence:.2f}        {types}")
    
    print("="*80)
    print(f"\nExtraction complete! Data saved to: {output_dir}")
    
    return 0

if __name__ == "__main__":
    exit(main())

