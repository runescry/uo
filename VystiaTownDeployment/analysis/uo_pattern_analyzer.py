"""
UO Town Pattern Analyzer
Extracts spatial patterns, relationships, and generation rules from town data
"""

import json
import numpy as np
from pathlib import Path
from typing import List, Dict, Tuple
from collections import defaultdict, Counter
from dataclasses import dataclass, asdict
import math

@dataclass
class TownPattern:
    """Extracted pattern data for a town"""
    name: str
    total_area: int
    building_count: int
    density: float  # buildings per 100 tiles
    
    # Spatial patterns
    road_network: Dict
    building_clusters: List[Dict]
    district_zones: List[Dict]
    
    # Building relationships
    building_distances: Dict[str, Dict[str, float]]  # avg distance between building types
    building_colocation: Dict[str, List[str]]  # what's typically near what
    
    # Layout metrics
    center_point: Tuple[int, int]
    density_gradient: List[float]  # density at different radii from center
    road_grid_type: str  # "organic", "grid", "radial"
    
    # Generation rules
    placement_rules: List[Dict]

class PatternAnalyzer:
    """Analyze spatial patterns in extracted town data"""
    
    def __init__(self):
        self.patterns = {}
        
    def analyze_town(self, town_file: str) -> TownPattern:
        """Analyze a single town's patterns"""
        with open(town_file, 'r') as f:
            data = json.load(f)
        
        bounds = data['bounds']
        area = (bounds['x2'] - bounds['x1']) * (bounds['y2'] - bounds['y1'])
        
        # Detect buildings first (you'd use BuildingDetector from previous artifact)
        buildings = self._detect_buildings_simple(data['statics'])
        
        # Analyze road network
        road_network = self._analyze_roads(data['statics'], bounds)
        
        # Find building clusters and districts
        clusters = self._find_clusters(buildings)
        districts = self._identify_districts(buildings, clusters)
        
        # Calculate center point
        center = self._find_town_center(buildings)
        
        # Analyze density gradient
        density_grad = self._calculate_density_gradient(buildings, center)
        
        # Analyze building relationships
        distances = self._calculate_building_distances(buildings)
        colocation = self._find_colocation_patterns(buildings, distances)
        
        # Determine road grid type
        grid_type = self._classify_road_pattern(road_network)
        
        # Extract placement rules
        rules = self._extract_placement_rules(buildings, road_network, districts)
        
        pattern = TownPattern(
            name=data['name'],
            total_area=area,
            building_count=len(buildings),
            density=len(buildings) / (area / 10000),
            road_network=road_network,
            building_clusters=clusters,
            district_zones=districts,
            building_distances=distances,
            building_colocation=colocation,
            center_point=center,
            density_gradient=density_grad,
            road_grid_type=grid_type,
            placement_rules=rules
        )
        
        return pattern
    
    def _detect_buildings_simple(self, statics: List[Dict]) -> List[Dict]:
        """Simplified building detection for analysis"""
        # Filter to building-like tiles (walls, floors, etc.)
        building_tiles = [s for s in statics if self._is_building_tile(s['tile_id'])]
        
        # Simple clustering
        buildings = []
        visited = set()
        
        for i, tile in enumerate(building_tiles):
            if i in visited:
                continue
            
            cluster = [tile]
            visited.add(i)
            self._grow_cluster(cluster, building_tiles, visited, i)
            
            if len(cluster) >= 4:
                buildings.append(self._cluster_to_building(cluster))
        
        return buildings
    
    def _is_building_tile(self, tile_id: int) -> bool:
        """Quick check if tile is building component"""
        # Simplified - expand based on your tile ranges
        return (0x0000 <= tile_id <= 0x0100 or  # Walls
                0x031F <= tile_id <= 0x0550 or  # Floors
                0x1500 <= tile_id <= 0x1600)    # Roofs
    
    def _grow_cluster(self, cluster, all_tiles, visited, current_idx, max_dist=5):
        """Grow building cluster"""
        current = all_tiles[current_idx]
        
        for i, tile in enumerate(all_tiles):
            if i in visited:
                continue
            
            dx = abs(tile['x'] - current['x'])
            dy = abs(tile['y'] - current['y'])
            
            if dx <= max_dist and dy <= max_dist:
                cluster.append(tile)
                visited.add(i)
    
    def _cluster_to_building(self, cluster: List[Dict]) -> Dict:
        """Convert tile cluster to building object"""
        xs = [t['x'] for t in cluster]
        ys = [t['y'] for t in cluster]
        
        return {
            'bounds': {
                'min_x': min(xs), 'max_x': max(xs),
                'min_y': min(ys), 'max_y': max(ys),
            },
            'center': {'x': sum(xs) // len(xs), 'y': sum(ys) // len(ys)},
            'size': len(cluster),
            'type': 'unknown',  # Would classify based on components
        }
    
    def _analyze_roads(self, statics: List[Dict], bounds: Dict) -> Dict:
        """Analyze road network patterns"""
        # Filter road tiles (simplified)
        roads = [s for s in statics if 0x0070 <= s['tile_id'] <= 0x00B0]
        
        if not roads:
            return {'exists': False}
        
        # Create road grid
        width = bounds['x2'] - bounds['x1']
        height = bounds['y2'] - bounds['y1']
        road_grid = np.zeros((height, width), dtype=bool)
        
        for road in roads:
            x = road['x'] - bounds['x1']
            y = road['y'] - bounds['y1']
            if 0 <= x < width and 0 <= y < height:
                road_grid[y, x] = True
        
        # Analyze road connectivity and structure
        total_road_tiles = len(roads)
        
        # Find major road axes
        horizontal_density = np.sum(road_grid, axis=1)
        vertical_density = np.sum(road_grid, axis=0)
        
        # Detect main roads (high density lines)
        h_roads = [i for i, d in enumerate(horizontal_density) if d > width * 0.3]
        v_roads = [i for i, d in enumerate(vertical_density) if d > height * 0.3]
        
        return {
            'exists': True,
            'total_tiles': total_road_tiles,
            'coverage': total_road_tiles / (width * height),
            'main_horizontal': len(h_roads),
            'main_vertical': len(v_roads),
            'has_grid': len(h_roads) >= 2 and len(v_roads) >= 2,
        }
    
    def _find_clusters(self, buildings: List[Dict]) -> List[Dict]:
        """Find building clusters (neighborhoods)"""
        if not buildings:
            return []
        
        clusters = []
        cluster_threshold = 20  # tiles between buildings
        
        # Simple density-based clustering
        visited = set()
        
        for i, b1 in enumerate(buildings):
            if i in visited:
                continue
            
            cluster = [b1]
            visited.add(i)
            
            # Find nearby buildings
            for j, b2 in enumerate(buildings):
                if j in visited:
                    continue
                
                dist = self._distance(b1['center'], b2['center'])
                if dist <= cluster_threshold:
                    cluster.append(b2)
                    visited.add(j)
            
            if len(cluster) >= 2:
                clusters.append({
                    'buildings': len(cluster),
                    'center': self._cluster_center(cluster),
                    'radius': self._cluster_radius(cluster),
                })
        
        return clusters
    
    def _identify_districts(self, buildings: List[Dict], clusters: List[Dict]) -> List[Dict]:
        """Identify functional districts (commercial, residential, etc.)"""
        # Simplified - would use building types
        districts = []
        
        for cluster in clusters:
            # Analyze building types in cluster
            # For now, just mark high-density areas as commercial
            is_dense = cluster['buildings'] > 5
            
            districts.append({
                'type': 'commercial' if is_dense else 'residential',
                'center': cluster['center'],
                'building_count': cluster['buildings'],
            })
        
        return districts
    
    def _find_town_center(self, buildings: List[Dict]) -> Tuple[int, int]:
        """Find the town center (highest density point)"""
        if not buildings:
            return (0, 0)
        
        # Use centroid of all buildings
        centers = [b['center'] for b in buildings]
        avg_x = sum(c['x'] for c in centers) // len(centers)
        avg_y = sum(c['y'] for c in centers) // len(centers)
        
        return (avg_x, avg_y)
    
    def _calculate_density_gradient(self, buildings: List[Dict], center: Tuple[int, int]) -> List[float]:
        """Calculate building density at different radii from center"""
        radii = [10, 20, 30, 50, 75, 100]
        densities = []
        
        for radius in radii:
            count = 0
            for building in buildings:
                dist = math.sqrt(
                    (building['center']['x'] - center[0])**2 +
                    (building['center']['y'] - center[1])**2
                )
                if dist <= radius:
                    count += 1
            
            area = math.pi * radius * radius
            density = count / area * 1000  # per 1000 sq tiles
            densities.append(density)
        
        return densities
    
    def _calculate_building_distances(self, buildings: List[Dict]) -> Dict:
        """Calculate average distances between building types"""
        distances = defaultdict(lambda: defaultdict(list))
        
        for i, b1 in enumerate(buildings):
            for b2 in buildings[i+1:]:
                dist = self._distance(b1['center'], b2['center'])
                type1, type2 = b1['type'], b2['type']
                
                distances[type1][type2].append(dist)
                distances[type2][type1].append(dist)
        
        # Calculate averages
        avg_distances = {}
        for t1 in distances:
            avg_distances[t1] = {}
            for t2 in distances[t1]:
                avg_distances[t1][t2] = np.mean(distances[t1][t2])
        
        return avg_distances
    
    def _find_colocation_patterns(self, buildings: List[Dict], distances: Dict) -> Dict:
        """Find which building types are typically near each other"""
        colocation = defaultdict(list)
        
        for t1 in distances:
            # Find building types that are typically close
            close_types = [t2 for t2, dist in distances[t1].items() if dist < 30]
            colocation[t1] = close_types
        
        return dict(colocation)
    
    def _classify_road_pattern(self, road_network: Dict) -> str:
        """Classify the road layout pattern"""
        if not road_network.get('exists'):
            return 'none'
        
        if road_network.get('has_grid'):
            return 'grid'
        elif road_network.get('main_horizontal', 0) > 0 or road_network.get('main_vertical', 0) > 0:
            return 'radial'
        else:
            return 'organic'
    
    def _extract_placement_rules(self, buildings: List[Dict], 
                                 roads: Dict, districts: List[Dict]) -> List[Dict]:
        """Extract concrete placement rules from patterns"""
        rules = []
        
        # Rule: Building spacing
        if len(buildings) > 1:
            spacings = []
            for i, b1 in enumerate(buildings):
                nearest = min(
                    [self._distance(b1['center'], b2['center']) 
                     for j, b2 in enumerate(buildings) if i != j],
                    default=0
                )
                if nearest > 0:
                    spacings.append(nearest)
            
            if spacings:
                rules.append({
                    'type': 'minimum_spacing',
                    'value': np.percentile(spacings, 25),  # 25th percentile
                    'description': 'Minimum distance between buildings'
                })
        
        # Rule: Road proximity
        if roads.get('exists'):
            rules.append({
                'type': 'road_proximity',
                'value': 5,  # tiles
                'description': 'Buildings should be within N tiles of roads'
            })
        
        # Rule: District clustering
        for district in districts:
            rules.append({
                'type': 'district_density',
                'district': district['type'],
                'center': district['center'],
                'target_count': district['building_count'],
                'description': f"{district['type']} district density"
            })
        
        return rules
    
    def _distance(self, p1: Dict, p2: Dict) -> float:
        """Calculate distance between two points"""
        return math.sqrt((p1['x'] - p2['x'])**2 + (p1['y'] - p2['y'])**2)
    
    def _cluster_center(self, cluster: List[Dict]) -> Dict:
        """Calculate cluster center"""
        centers = [b['center'] for b in cluster]
        return {
            'x': sum(c['x'] for c in centers) // len(centers),
            'y': sum(c['y'] for c in centers) // len(centers),
        }
    
    def _cluster_radius(self, cluster: List[Dict]) -> float:
        """Calculate cluster radius"""
        center = self._cluster_center(cluster)
        return max(self._distance(center, b['center']) for b in cluster)
    
    def compare_towns(self, town_patterns: List[TownPattern]) -> Dict:
        """Compare patterns across multiple towns"""
        comparison = {
            'town_count': len(town_patterns),
            'density_range': (
                min(p.density for p in town_patterns),
                max(p.density for p in town_patterns)
            ),
            'layout_types': Counter(p.road_grid_type for p in town_patterns),
            'common_rules': self._find_common_rules(town_patterns),
        }
        
        return comparison
    
    def _find_common_rules(self, patterns: List[TownPattern]) -> List[Dict]:
        """Find rules that appear across multiple towns"""
        rule_types = defaultdict(list)
        
        for pattern in patterns:
            for rule in pattern.placement_rules:
                rule_types[rule['type']].append(rule)
        
        # Find rules that appear in most towns
        common = []
        threshold = len(patterns) * 0.7  # 70% of towns
        
        for rule_type, rules in rule_types.items():
            if len(rules) >= threshold:
                # Average the values
                if rule_type == 'minimum_spacing':
                    avg_value = np.mean([r['value'] for r in rules])
                    common.append({
                        'type': rule_type,
                        'value': avg_value,
                        'frequency': len(rules) / len(patterns)
                    })
        
        return common
    
    def export_patterns(self, output_file: str):
        """Export all analyzed patterns to JSON"""
        export_data = {
            'patterns': {name: asdict(pattern) for name, pattern in self.patterns.items()}
        }
        
        with open(output_file, 'w') as f:
            json.dump(export_data, f, indent=2)

# Usage
if __name__ == "__main__":
    analyzer = PatternAnalyzer()
    
    # Analyze all extracted towns
    town_files = Path('town_data').glob('*.json')
    patterns = []
    
    for town_file in town_files:
        print(f"Analyzing {town_file.stem}...")
        pattern = analyzer.analyze_town(str(town_file))
        analyzer.patterns[pattern.name] = pattern
        patterns.append(pattern)
        
        print(f"  Density: {pattern.density:.2f} buildings/100 tiles")
        print(f"  Layout: {pattern.road_grid_type}")
        print(f"  Districts: {len(pattern.district_zones)}")
    
    # Compare all towns
    comparison = analyzer.compare_towns(patterns)
    print(f"\nAnalyzed {comparison['town_count']} towns")
    print(f"Density range: {comparison['density_range']}")
    print(f"Common rules found: {len(comparison['common_rules'])}")
    
    # Export
    analyzer.export_patterns('town_patterns.json')
    print("\nPattern analysis complete!")
