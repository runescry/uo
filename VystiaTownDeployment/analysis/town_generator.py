"""
Pattern-based town generator (prototype).
Generates a simple layout from learned patterns and exports statics JSON.
"""

import argparse
import json
import random
import math
from pathlib import Path
from typing import Dict, List, Tuple, Set


ROAD_TILE = 0x0071  # Stone road
DIRT_TILE = 0x009C  # Dirt path


def extract_templates_from_clusters(town_data: Dict, min_size: int = 10) -> List[Dict]:
    """Extract building templates from precomputed clusters in town data."""
    clusters = town_data.get('analysis', {}).get('building_clusters', [])
    if not clusters:
        return []

    statics = town_data.get('statics', [])
    templates = []

    for cluster in clusters:
        bounds = cluster.get('bounds', {})
        min_x = bounds.get('min_x')
        max_x = bounds.get('max_x')
        min_y = bounds.get('min_y')
        max_y = bounds.get('max_y')

        if None in (min_x, max_x, min_y, max_y):
            continue

        items = [s for s in statics if min_x <= s['x'] <= max_x and min_y <= s['y'] <= max_y]
        if len(items) < min_size:
            continue

        width = max_x - min_x + 1
        height = max_y - min_y + 1
        template_items = []

        for item in items:
            template_items.append({
                'tile_id': item['tile_id'],
                'x': item['x'] - min_x,
                'y': item['y'] - min_y,
                'z': item['z'],
                'hue': item.get('hue', 0)
            })

        templates.append({
            'width': width,
            'height': height,
            'items': template_items
        })

    return templates


def load_templates_from_town(town_path: Path, min_size: int = 10) -> List[Dict]:
    with town_path.open() as f:
        data = json.load(f)
    return extract_templates_from_clusters(data, min_size=min_size)


def load_patterns(path: Path) -> Dict[str, Dict]:
    with path.open() as f:
        return json.load(f).get("patterns", {})


def generate_roads(bounds: Tuple[int, int, int, int], road_type: str, seed: int) -> List[Dict[str, int]]:
    x1, y1, x2, y2 = bounds
    width = x2 - x1
    height = y2 - y1
    roads = []
    rng = random.Random(seed)

    if road_type == "grid":
        spacing = 20
        for y in range(y1 + spacing, y2, spacing):
            for x in range(x1, x2):
                roads.append({'tile_id': ROAD_TILE, 'x': x, 'y': y, 'z': 0, 'hue': 0})
        for x in range(x1 + spacing, x2, spacing):
            for y in range(y1, y2):
                roads.append({'tile_id': ROAD_TILE, 'x': x, 'y': y, 'z': 0, 'hue': 0})
    elif road_type == "radial":
        center_x = (x1 + x2) // 2
        center_y = (y1 + y2) // 2
        spokes = 6
        radius = min(width, height) // 2
        for spoke in range(spokes):
            angle = (spoke * 2 * math.pi) / spokes
            for r in range(radius):
                x = int(center_x + r * math.cos(angle))
                y = int(center_y + r * math.sin(angle))
                if x1 <= x < x2 and y1 <= y < y2:
                    roads.append({'tile_id': ROAD_TILE, 'x': x, 'y': y, 'z': 0, 'hue': 0})
    else:
        center_x = (x1 + x2) // 2
        center_y = (y1 + y2) // 2
        for _ in range(rng.randint(3, 6)):
            x, y = center_x, center_y
            for _ in range(rng.randint(30, 60)):
                x += rng.randint(-1, 1)
                y += rng.randint(-1, 1)
                if x1 <= x < x2 and y1 <= y < y2:
                    roads.append({'tile_id': DIRT_TILE, 'x': x, 'y': y, 'z': 0, 'hue': 0})

    return roads


def build_occupied(road_tiles: List[Dict[str, int]]) -> Set[Tuple[int, int]]:
    return {(r['x'], r['y']) for r in road_tiles}


def can_place(x: int, y: int, size: int, bounds: Tuple[int, int, int, int], occupied: Set[Tuple[int, int]]) -> bool:
    x1, y1, x2, y2 = bounds
    for dx in range(-size // 2, size // 2 + 1):
        for dy in range(-size // 2, size // 2 + 1):
            px = x + dx
            py = y + dy
            if px < x1 or px >= x2 or py < y1 or py >= y2:
                return False
            if (px, py) in occupied:
                return False
    return True


def place_house(x: int, y: int, template: List[Dict[str, int]], occupied: Set[Tuple[int, int]]) -> List[Dict[str, int]]:
    placed = []
    for tile in template:
        px = x + tile['x']
        py = y + tile['y']
        placed.append({'tile_id': tile['tile_id'], 'x': px, 'y': py, 'z': tile['z'], 'hue': tile['hue']})
        occupied.add((px, py))
    return placed


def generate_town(name: str, bounds: Tuple[int, int, int, int], pattern: Dict, templates: List[Dict], seed: int) -> Dict:
    road_type = pattern.get("road_grid_type", "organic")
    density = pattern.get("density", 1.0)

    roads = generate_roads(bounds, road_type, seed)
    occupied = build_occupied(roads)

    width = bounds[2] - bounds[0]
    height = bounds[3] - bounds[1]
    area = width * height
    target_buildings = max(1, int((area / 10000) * density))

    if not templates:
        raise ValueError("No building templates available")

    statics = list(roads)
    attempts = 0
    placed = 0
    rng = random.Random(seed)
    road_coords = list(occupied)

    while placed < target_buildings and attempts < target_buildings * 80:
        attempts += 1
        template = rng.choice(templates)
        footprint = max(template['width'], template['height'])
        if road_coords:
            rx, ry = rng.choice(road_coords)
            x = rx + rng.randint(-10, 10)
            y = ry + rng.randint(-10, 10)
        else:
            x = rng.randint(bounds[0], bounds[2] - 1)
            y = rng.randint(bounds[1], bounds[3] - 1)

        if can_place(x, y, footprint, bounds, occupied):
            statics.extend(place_template(x, y, template, occupied))
            placed += 1

    return {
        'name': name,
        'bounds': {'x1': bounds[0], 'y1': bounds[1], 'x2': bounds[2], 'y2': bounds[3]},
        'pattern': pattern.get('name', 'unknown'),
        'statics': statics,
        'statistics': {
            'road_tiles': len(roads),
            'buildings': placed,
            'target_buildings': target_buildings
        }
    }


def place_template(x: int, y: int, template: Dict, occupied: Set[Tuple[int, int]]) -> List[Dict[str, int]]:
    """Place a building template at a location."""
    items = []
    for item in template['items']:
        px = x + item['x']
        py = y + item['y']
        items.append({'tile_id': item['tile_id'], 'x': px, 'y': py, 'z': item['z'], 'hue': item.get('hue', 0)})
        occupied.add((px, py))
    return items


def main():
    parser = argparse.ArgumentParser(description="Generate a prototype town from learned patterns")
    parser.add_argument("--patterns", type=Path, default=Path(__file__).parent.parent / "town_patterns.json",
                        help="Path to town_patterns.json")
    parser.add_argument("--pattern", default="britain", help="Pattern name to use")
    parser.add_argument("--source-town", type=Path, default=Path(__file__).parent.parent / "town_data" / "britain.json",
                        help="Town JSON used for building templates")
    parser.add_argument("--min-template-size", type=int, default=10,
                        help="Minimum static count per template")
    parser.add_argument("--bounds", nargs=4, type=int, default=[1400, 1500, 1750, 1800],
                        help="Bounds x1 y1 x2 y2")
    parser.add_argument("--output", type=Path, default=Path(__file__).parent.parent / "outputs" / "generated_town.json",
                        help="Output JSON file")
    parser.add_argument("--seed", type=int, default=42, help="Random seed")

    args = parser.parse_args()
    patterns = load_patterns(Path(args.patterns))
    pattern = patterns.get(args.pattern) or next(iter(patterns.values()))
    pattern['name'] = pattern.get('name', args.pattern)

    output_path = Path(args.output)
    output_path.parent.mkdir(parents=True, exist_ok=True)

    templates = load_templates_from_town(args.source_town, min_size=args.min_template_size)
    if not templates:
        raise SystemExit("No templates found in source town data.")

    town = generate_town("PrototypeTown", tuple(args.bounds), pattern, templates, args.seed)

    with output_path.open('w') as f:
        json.dump(town, f, indent=2)

    print(f"Wrote {output_path} ({town['statistics']['buildings']} buildings, {town['statistics']['road_tiles']} roads)")


if __name__ == "__main__":
    main()
