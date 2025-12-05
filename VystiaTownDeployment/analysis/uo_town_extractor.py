"""
UO Town Data Extractor
Extracts static placements, terrain, and multi data from UO map files
Requires: uotools or manual .mul file parsing
"""

import struct
import json
from pathlib import Path
from dataclasses import dataclass, asdict
from typing import List, Dict, Tuple
from collections import defaultdict

@dataclass
class StaticItem:
    """Represents a static item placement"""
    tile_id: int
    x: int
    y: int
    z: int
    hue: int = 0
    
@dataclass
class TerrainTile:
    """Represents a terrain tile"""
    tile_id: int
    x: int
    y: int
    z: int

class UOMapReader:
    """Reads UO .mul files and extracts map data"""
    
    def __init__(self, uo_path: str, map_num: int = 0):
        self.uo_path = Path(uo_path)
        self.map_path = self.uo_path / f"map{map_num}.mul"
        self.statics_path = self.uo_path / f"statics{map_num}.mul"
        self.staidx_path = self.uo_path / f"staidx{map_num}.mul"
        
    def read_terrain_block(self, block_x: int, block_y: int) -> List[TerrainTile]:
        """Read an 8x8 terrain block"""
        tiles = []
        block_id = block_x + (block_y * 896)  # 896 blocks wide for map0 (7168 tiles / 8)
        
        with open(self.map_path, 'rb') as f:
            f.seek(block_id * 196)  # 196 bytes per block (header + 64 tiles)
            
            # Skip header (4 bytes)
            f.read(4)
            
            # Read 64 tiles (8x8)
            for cell in range(64):
                tile_id = struct.unpack('<H', f.read(2))[0]
                z = struct.unpack('b', f.read(1))[0]
                
                cell_x = block_x * 8 + (cell % 8)
                cell_y = block_y * 8 + (cell // 8)
                
                tiles.append(TerrainTile(tile_id, cell_x, cell_y, z))
                
        return tiles
    
    def read_statics_block(self, block_x: int, block_y: int) -> List[StaticItem]:
        """Read statics for an 8x8 block"""
        statics = []
        block_id = block_x + (block_y * 896)  # 896 blocks wide for map0 (7168 tiles / 8)
        
        # Read index entry
        with open(self.staidx_path, 'rb') as idx:
            idx.seek(block_id * 12)
            offset = struct.unpack('<I', idx.read(4))[0]
            length = struct.unpack('<I', idx.read(4))[0]
            
            if offset == 0xFFFFFFFF or length == 0:
                return statics  # No statics in this block
        
        # Read statics data
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
                
                statics.append(StaticItem(tile_id, abs_x, abs_y, z, hue))
                
        return statics
    
    def extract_region(self, x1: int, y1: int, x2: int, y2: int) -> Dict:
        """Extract all terrain and statics from a rectangular region"""
        terrain = []
        statics = []
        
        # Calculate block range
        block_x1 = x1 // 8
        block_y1 = y1 // 8
        block_x2 = x2 // 8
        block_y2 = y2 // 8
        
        for block_y in range(block_y1, block_y2 + 1):
            for block_x in range(block_x1, block_x2 + 1):
                # Get terrain
                block_terrain = self.read_terrain_block(block_x, block_y)
                terrain.extend([t for t in block_terrain if x1 <= t.x <= x2 and y1 <= t.y <= y2])
                
                # Get statics
                block_statics = self.read_statics_block(block_x, block_y)
                statics.extend([s for s in block_statics if x1 <= s.x <= x2 and y1 <= s.y <= y2])
        
        return {
            'bounds': {'x1': x1, 'y1': y1, 'x2': x2, 'y2': y2},
            'terrain': [asdict(t) for t in terrain],
            'statics': [asdict(s) for s in statics]
        }

class TownExtractor:
    """Extract and categorize town data"""
    
    # Known OSI town boundaries (map0 coordinates)
    TOWNS = {
        'britain': (1400, 1500, 1750, 1800),
        'trinsic': (1800, 2700, 2000, 2900),
        'vesper': (2700, 900, 2950, 1150),
        'moonglow': (4400, 1100, 4550, 1250),
        'skara_brae': (550, 2100, 700, 2250),
        'yew': (450, 950, 650, 1150),
        'minoc': (2450, 450, 2650, 650),
        'cove': (2200, 1150, 2350, 1300),
    }
    
    def __init__(self, map_reader: UOMapReader):
        self.reader = map_reader
        
    def extract_all_towns(self, output_dir: str = 'town_data'):
        """Extract all predefined towns to JSON files"""
        output_path = Path(output_dir)
        output_path.mkdir(exist_ok=True)
        
        for town_name, bounds in self.TOWNS.items():
            print(f"Extracting {town_name}...")
            data = self.reader.extract_region(*bounds)
            
            # Add metadata
            data['name'] = town_name
            data['static_count'] = len(data['statics'])
            data['terrain_count'] = len(data['terrain'])
            
            # Save to file
            output_file = output_path / f"{town_name}.json"
            with open(output_file, 'w') as f:
                json.dump(data, f, indent=2)
                
            print(f"  -> Saved {data['static_count']} statics, {data['terrain_count']} terrain tiles")

# Usage example
if __name__ == "__main__":
    # Point this to your UO installation directory with original OSI towns
    UO_PATH = "C:/Ultima Online"  # Original UO files

    # Use map 0 which has all the original OSI towns
    reader = UOMapReader(UO_PATH, map_num=0)
    extractor = TownExtractor(reader)
    
    # Extract all towns
    extractor.extract_all_towns()
    
    print("\nExtraction complete! Town data saved to town_data/")
