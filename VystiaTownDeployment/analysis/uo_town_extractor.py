"""
UO Town Data Extractor
Extracts static placements, terrain, and multi data from UO map files
Requires: uotools or manual .mul file parsing
"""

import argparse
import json
import struct
from pathlib import Path
from dataclasses import dataclass, asdict
from typing import Any, Dict, List, Sequence, Tuple
from collections import defaultdict

from tiledata_reader import TileDataReader, TileFlags

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
        self.map_file = open(self.map_path, 'rb')
        self.staidx_file = open(self.staidx_path, 'rb')
        self.statics_file = open(self.statics_path, 'rb')
        
    def close(self):
        """Close open file handles"""
        for stream in (self.map_file, self.staidx_file, self.statics_file):
            try:
                stream.close()
            except Exception:
                pass
        
    def read_terrain_block(self, block_x: int, block_y: int) -> List[TerrainTile]:
        """Read an 8x8 terrain block"""
        tiles = []
        block_id = block_x + (block_y * 896)  # 896 blocks wide for map0 (7168 tiles / 8)
        
        self.map_file.seek(block_id * 196)  # 196 bytes per block (header + 64 tiles)
        self.map_file.read(4)  # Skip header (4 bytes)
        
        for cell in range(64):
            tile_id = struct.unpack('<H', self.map_file.read(2))[0]
            z = struct.unpack('b', self.map_file.read(1))[0]
            
            cell_x = block_x * 8 + (cell % 8)
            cell_y = block_y * 8 + (cell // 8)
            
            tiles.append(TerrainTile(tile_id, cell_x, cell_y, z))
                
        return tiles
    
    def read_statics_block(self, block_x: int, block_y: int) -> List[StaticItem]:
        """Read statics for an 8x8 block"""
        statics = []
        block_id = block_x + (block_y * 896)  # 896 blocks wide for map0 (7168 tiles / 8)
        
        self.staidx_file.seek(block_id * 12)
        offset = struct.unpack('<I', self.staidx_file.read(4))[0]
        length = struct.unpack('<I', self.staidx_file.read(4))[0]
        
        if offset == 0xFFFFFFFF or length == 0:
            return statics  # No statics in this block
        
        self.statics_file.seek(offset)
        count = length // 7  # Each static entry is 7 bytes
            
        for _ in range(count):
            tile_id = struct.unpack('<H', self.statics_file.read(2))[0]
            x = struct.unpack('B', self.statics_file.read(1))[0]
            y = struct.unpack('B', self.statics_file.read(1))[0]
            z = struct.unpack('b', self.statics_file.read(1))[0]
            hue = struct.unpack('<H', self.statics_file.read(2))[0]
            
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

DEFAULT_TOWNS = [
    {'name': 'britain', 'bounds': (1400, 1500, 1750, 1800)},
    {'name': 'trinsic', 'bounds': (1800, 2700, 2000, 2900)},
    {'name': 'vesper', 'bounds': (2700, 900, 2950, 1150)},
    {'name': 'moonglow', 'bounds': (4400, 1100, 4550, 1250)},
    {'name': 'skara_brae', 'bounds': (550, 2100, 700, 2250)},
    {'name': 'yew', 'bounds': (450, 950, 650, 1150)},
    {'name': 'minoc', 'bounds': (2450, 450, 2650, 650)},
    {'name': 'cove', 'bounds': (2200, 1150, 2350, 1300)},
]


class TownExtractor:
    """Extract and categorize town data"""
    
    def __init__(self, map_reader: UOMapReader, towns: Sequence[Dict[str, Any]] = None,
                 tiledata_reader: TileDataReader = None, taxonomy_rules: List[Dict[str, Any]] = None):
        self.reader = map_reader
        self.towns = towns or DEFAULT_TOWNS
        self.tiledata_reader = tiledata_reader
        self.taxonomy_rules = taxonomy_rules or []
        
    def extract_all_towns(self, output_dir: str = 'town_data'):
        """Extract all configured towns to JSON files"""
        output_path = Path(output_dir)
        output_path.mkdir(exist_ok=True)
        
        for town in self.towns:
            town_name = town.get('name')
            bounds = town.get('bounds')

            if not town_name or not bounds or len(bounds) != 4:
                print(f"Skipping invalid town entry: {town}")
                continue

            print(f"Extracting {town_name}...")
            data = self.reader.extract_region(*bounds)
            
            data['name'] = town_name
            data['static_count'] = len(data['statics'])
            data['terrain_count'] = len(data['terrain'])
            data['metadata'] = {
                'source_bounds': bounds,
                'map_num': town.get('map_num')
            }
            analysis = {
                'road_network': analyze_roads(data['statics'], data['bounds']),
                'building_clusters': cluster_building_tiles(data['statics']),
            }

            if self.tiledata_reader and self.taxonomy_rules:
                static_ids = [s['tile_id'] for s in data['statics']]
                land_ids = [t['tile_id'] for t in data['terrain']]
                static_map = build_class_map(static_ids, self.tiledata_reader, self.taxonomy_rules, "static")
                land_map = build_class_map(land_ids, self.tiledata_reader, self.taxonomy_rules, "land")
                analysis['tile_classes'] = {
                    'statics': static_map,
                    'terrain': land_map
                }
                analysis['class_counts'] = {
                    'statics': count_classes(data['statics'], static_map),
                    'terrain': count_classes(data['terrain'], land_map)
                }

            data['analysis'] = analysis
            
            output_file = output_path / f"{town_name}.json"
            with open(output_file, 'w') as f:
                json.dump(data, f, indent=2)
                
            print(f"  -> Saved {data['static_count']} statics, {data['terrain_count']} terrain tiles")

def load_manifest(manifest_path: Path) -> Dict[str, Any]:
    """Load town extraction manifest"""
    with manifest_path.open() as f:
        data = json.load(f)
    towns = data.get("towns")
    if towns and not isinstance(towns, list):
        raise ValueError("Manifest 'towns' must be a list")
    return data


def normalize_towns(towns: Sequence[Dict[str, Any]]) -> List[Dict[str, Any]]:
    """Normalize town entries (ensure bounds is tuple)"""
    normalized = []
    for town in towns:
        bounds = town.get('bounds')
        if not bounds or len(bounds) != 4:
            continue
        normalized.append({
            'name': town.get('name'),
            'bounds': tuple(int(v) for v in bounds),
            'map_num': town.get('map_num')
        })
    return normalized or []


def is_building_tile(tile_id: int) -> bool:
    """Quick check if a tile is a building component (simplified)."""
    return (0x0000 <= tile_id <= 0x0100 or  # Walls
            0x031F <= tile_id <= 0x0550 or  # Floors
            0x1500 <= tile_id <= 0x1600)    # Roofs


def analyze_roads(statics: List[Dict[str, Any]], bounds: Dict[str, int]) -> Dict[str, Any]:
    """Analyze basic road layout from statics."""
    roads = [s for s in statics if 0x0070 <= s['tile_id'] <= 0x00B0]
    if not roads:
        return {'exists': False}

    width = bounds['x2'] - bounds['x1']
    height = bounds['y2'] - bounds['y1']
    row_counts = defaultdict(int)
    col_counts = defaultdict(int)

    for road in roads:
        x = road['x'] - bounds['x1']
        y = road['y'] - bounds['y1']
        if 0 <= x < width and 0 <= y < height:
            row_counts[y] += 1
            col_counts[x] += 1

    horizontal_roads = [y for y, count in row_counts.items() if count > width * 0.3]
    vertical_roads = [x for x, count in col_counts.items() if count > height * 0.3]

    return {
        'exists': True,
        'total_tiles': len(roads),
        'coverage': len(roads) / max(width * height, 1),
        'main_horizontal': len(horizontal_roads),
        'main_vertical': len(vertical_roads),
        'has_grid': len(horizontal_roads) >= 2 and len(vertical_roads) >= 2
    }


def cluster_building_tiles(statics: List[Dict[str, Any]], max_dist: int = 5) -> List[Dict[str, Any]]:
    """Cluster building-like statics into rough building groups."""
    building_tiles = [s for s in statics if is_building_tile(s['tile_id'])]
    if not building_tiles:
        return []

    buckets = defaultdict(list)
    bucket_size = max_dist
    for idx, tile in enumerate(building_tiles):
        buckets[(tile['x'] // bucket_size, tile['y'] // bucket_size)].append(idx)

    visited = set()
    clusters = []

    def neighbors(tile_idx: int) -> List[int]:
        tile = building_tiles[tile_idx]
        bx = tile['x'] // bucket_size
        by = tile['y'] // bucket_size
        nearby = []
        for dx in (-1, 0, 1):
            for dy in (-1, 0, 1):
                nearby.extend(buckets.get((bx + dx, by + dy), []))
        return nearby

    for i, tile in enumerate(building_tiles):
        if i in visited:
            continue

        stack = [i]
        visited.add(i)
        cluster = []

        while stack:
            current = stack.pop()
            current_tile = building_tiles[current]
            cluster.append(current_tile)
            for j in neighbors(current):
                if j in visited:
                    continue
                candidate = building_tiles[j]
                if abs(candidate['x'] - current_tile['x']) <= max_dist and abs(candidate['y'] - current_tile['y']) <= max_dist:
                    visited.add(j)
                    stack.append(j)

        if len(cluster) >= 4:
            xs = [t['x'] for t in cluster]
            ys = [t['y'] for t in cluster]
            clusters.append({
                'bounds': {
                    'min_x': min(xs), 'max_x': max(xs),
                    'min_y': min(ys), 'max_y': max(ys)
                },
                'center': {
                    'x': sum(xs) // len(xs),
                    'y': sum(ys) // len(ys)
                },
                'size': len(cluster)
            })

    return clusters


def load_taxonomy(taxonomy_path: Path) -> List[Dict[str, Any]]:
    """Load tile taxonomy rules from JSON."""
    with taxonomy_path.open() as f:
        data = json.load(f)
    rules = data.get("rules", [])
    return rules if isinstance(rules, list) else []


def classify_tile(tile_info, rule: Dict[str, Any]) -> bool:
    """Determine if a tile matches a taxonomy rule."""
    flags_any = rule.get("flags_any", [])
    flags_all = rule.get("flags_all", [])
    name_contains = rule.get("name_contains", [])

    if flags_any:
        if not any(tile_info.flags & getattr(TileFlags, name, TileFlags.None_) for name in flags_any):
            return False

    if flags_all:
        if not all(tile_info.flags & getattr(TileFlags, name, TileFlags.None_) for name in flags_all):
            return False

    if name_contains:
        tile_name = tile_info.name.lower()
        if not any(token in tile_name for token in name_contains):
            return False

    return True


def build_class_map(tile_ids: List[int], tiledata: TileDataReader, rules: List[Dict[str, Any]], target: str) -> Dict[str, List[str]]:
    """Build a tile_id -> classes mapping for land or static tiles."""
    class_map: Dict[str, List[str]] = {}
    for tile_id in sorted(set(tile_ids)):
        info = tiledata.get_static(tile_id) if target == "static" else tiledata.get_land(tile_id)
        if not info:
            continue

        classes = []
        for rule in rules:
            rule_target = rule.get("target", "both")
            if rule_target not in (target, "both"):
                continue
            if classify_tile(info, rule):
                classes.append(rule.get("name"))

        if classes:
            class_map[str(tile_id)] = classes

    return class_map


def count_classes(tiles: List[Dict[str, Any]], class_map: Dict[str, List[str]]) -> Dict[str, int]:
    """Count class occurrences from tile list using a class map."""
    counts = defaultdict(int)
    for tile in tiles:
        for cls in class_map.get(str(tile['tile_id']), []):
            counts[cls] += 1
    return dict(counts)


def main():
    parser = argparse.ArgumentParser(description="Extract OSI towns into JSON")
    parser.add_argument("--uo-path", type=str, default="C:/Ultima Online",
                        help="Path to the original UO installation (map*.mul files)")
    parser.add_argument("--map-num", type=int, default=0,
                        help="Map number to read (e.g., 0 for Felucca, 6 for Vystia)")
    parser.add_argument("--output-dir", type=str, default="town_data",
                        help="Directory to write extracted town JSON files")
    parser.add_argument("--manifest", type=Path,
                        help="JSON manifest describing towns and overrides")
    parser.add_argument("--tiledata", type=Path,
                        help="Path to tiledata.mul for tile classification")
    parser.add_argument("--taxonomy", type=Path,
                        help="Path to tile taxonomy JSON rules")
    parser.add_argument("--towns", nargs="+",
                        help="List of town names to extract (defaults to manifest/default set)")
    parser.add_argument("--list", action="store_true",
                        help="List configured towns and exit")

    args = parser.parse_args()

    manifest_data = {}
    if args.manifest:
        manifest_data = load_manifest(args.manifest)

    towns = normalize_towns(manifest_data.get("towns", DEFAULT_TOWNS))
    if not towns:
        towns = normalize_towns(DEFAULT_TOWNS)

    if args.towns:
        desired = set(args.towns)
        towns = [t for t in towns if t.get("name") in desired]
        if not towns:
            raise SystemExit(f"No matching town entries found for {args.towns}")

    if args.list:
        print("Configured towns for extraction:")
        for town in towns:
            print(f"  {town['name']}: bounds={town['bounds']}")
        return

    uo_path = manifest_data.get("uo_path", args.uo_path)
    map_num = manifest_data.get("map_num", args.map_num)
    output_dir = manifest_data.get("output_dir", args.output_dir)

    tiledata_path = manifest_data.get("tiledata") or args.tiledata
    if tiledata_path is None:
        tiledata_path = Path(uo_path) / "tiledata.mul"
    taxonomy_path = manifest_data.get("taxonomy") or args.taxonomy
    if taxonomy_path is None:
        taxonomy_path = Path(__file__).parent / "tile_taxonomy.json"

    taxonomy_rules = load_taxonomy(Path(taxonomy_path)) if Path(taxonomy_path).exists() else []
    tiledata_reader = TileDataReader(str(tiledata_path)) if Path(tiledata_path).exists() else None

    reader = UOMapReader(uo_path, map_num=map_num)
    try:
        extractor = TownExtractor(reader, towns=towns, tiledata_reader=tiledata_reader, taxonomy_rules=taxonomy_rules)
        extractor.extract_all_towns(output_dir=output_dir)
    finally:
        reader.close()
        if tiledata_reader:
            tiledata_reader.close()

    print("\nExtraction complete!")


if __name__ == "__main__":
    main()
