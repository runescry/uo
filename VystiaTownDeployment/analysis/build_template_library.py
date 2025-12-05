"""
Building Template Library Builder
Extracts reusable building templates of various sizes from OSI towns
Uses a grid-based extraction approach instead of clustering
"""

import json
from pathlib import Path
from typing import List, Dict, Tuple
from collections import defaultdict

class TemplateExtractor:
    """Extracts building templates from OSI town data"""

    def __init__(self):
        self.templates = defaultdict(list)

    def extract_templates_from_town(self, town_name: str, town_data: Dict,
                                   template_sizes: List[Tuple[int, int]]):
        """
        Extract templates of specified sizes from a town
        Uses a sliding window approach to extract rectangular regions
        """
        statics = town_data['statics']
        bounds = town_data['bounds']

        print(f"\nExtracting from {town_name}...")
        print(f"  Total statics: {len(statics)}")

        # Build spatial index
        spatial_grid = self._build_spatial_grid(statics)

        extracted_count = 0

        for width, height in template_sizes:
            templates = self._extract_size(spatial_grid, bounds, width, height, town_name)
            self.templates[f"{width}x{height}"].extend(templates)
            extracted_count += len(templates)
            if templates:
                print(f"  {width}x{height}: Extracted {len(templates)} templates")

        print(f"  Total extracted: {extracted_count} templates")

    def _build_spatial_grid(self, statics: List[Dict]) -> Dict:
        """Build a grid for fast spatial lookups"""
        grid = defaultdict(list)
        for static in statics:
            grid[(static['x'], static['y'])].append(static)
        return grid

    def _extract_size(self, spatial_grid: Dict, bounds: Dict,
                     width: int, height: int, town_name: str) -> List[Dict]:
        """Extract templates of a specific size using sliding window"""
        templates = []
        x1, y1, x2, y2 = bounds['x1'], bounds['y1'], bounds['x2'], bounds['y2']

        # Sample every N tiles to avoid extracting too many overlapping templates
        step = max(width // 2, height // 2, 5)

        for y in range(y1, y2 - height, step):
            for x in range(x1, x2 - width, step):
                template = self._extract_region(spatial_grid, x, y, width, height)

                # Only keep templates with a minimum number of tiles
                min_tiles = (width * height) // 20  # At least 5% coverage
                if len(template['statics']) >= min_tiles:
                    template['source_town'] = town_name
                    template['width'] = width
                    template['height'] = height
                    templates.append(template)

        return templates

    def _extract_region(self, spatial_grid: Dict, x: int, y: int,
                       width: int, height: int) -> Dict:
        """Extract all statics in a rectangular region"""
        statics = []

        for dy in range(height):
            for dx in range(width):
                px, py = x + dx, y + dy
                if (px, py) in spatial_grid:
                    for static in spatial_grid[(px, py)]:
                        # Make coordinates relative to template origin
                        relative_static = static.copy()
                        relative_static['x'] = dx
                        relative_static['y'] = dy
                        statics.append(relative_static)

        return {
            'origin': {'x': x, 'y': y},
            'statics': statics,
            'tile_count': len(statics),
        }

    def categorize_templates(self):
        """Categorize templates by type and quality"""
        categorized = {
            'small': [],      # 5x5 to 10x10
            'medium': [],     # 10x10 to 20x20
            'large': [],      # 20x20 to 35x35
            'complex': [],    # 35x35+
        }

        for size_key, templates in self.templates.items():
            for template in templates:
                w, h = template['width'], template['height']
                area = w * h
                density = template['tile_count'] / area

                # Only keep templates with reasonable density (not too sparse)
                if density < 0.05:  # Less than 5% density
                    continue

                # Categorize by size
                max_dim = max(w, h)
                if max_dim <= 10:
                    category = 'small'
                elif max_dim <= 20:
                    category = 'medium'
                elif max_dim <= 35:
                    category = 'large'
                else:
                    category = 'complex'

                template['density'] = density
                template['category'] = category
                categorized[category].append(template)

        return categorized

    def save_library(self, output_file: str):
        """Save the template library to file"""
        categorized = self.categorize_templates()

        library = {
            'version': '1.0',
            'description': 'Building template library extracted from OSI towns',
            'templates': categorized,
            'statistics': {
                category: len(templates)
                for category, templates in categorized.items()
            }
        }

        with open(output_file, 'w') as f:
            json.dump(library, f, indent=2)

        print(f"\n=== Template Library Saved ===")
        print(f"Output: {output_file}")
        for category, count in library['statistics'].items():
            print(f"  {category}: {count} templates")


if __name__ == "__main__":
    extractor = TemplateExtractor()

    # Define template sizes to extract
    # (width, height) tuples
    template_sizes = [
        (8, 8),    # Small buildings
        (10, 10),
        (12, 12),
        (15, 15),  # Medium buildings
        (18, 18),
        (20, 20),
        (25, 25),  # Large buildings
        (30, 30),
    ]

    # Extract from all towns
    for town_file in Path('town_data').glob('*.json'):
        town_name = town_file.stem

        with open(town_file, 'r') as f:
            town_data = json.load(f)

        extractor.extract_templates_from_town(town_name, town_data, template_sizes)

    # Save library
    extractor.save_library('template_library.json')
