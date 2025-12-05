"""
Visualize Detected Cities on UO Map
Creates a visual map showing all detected cities with their boundaries and labels.
"""

import json
import math
from pathlib import Path
from typing import List, Dict
try:
    from PIL import Image, ImageDraw, ImageFont
    HAS_PIL = True
except ImportError:
    HAS_PIL = False
    print("Warning: PIL/Pillow not installed. Install with: pip install pillow")

# Color scheme for visualization
COLORS = {
    'background': (50, 50, 75),      # Dark blue-gray
    'water': (0, 50, 150),            # Dark blue
    'land': (100, 150, 100),          # Green-gray
    'city_border': (255, 255, 0),      # Yellow
    'city_fill': (255, 200, 0, 100),  # Orange-yellow with transparency
    'city_label': (255, 255, 255),    # White
    'grid': (100, 100, 100),          # Gray
}

class CityVisualizer:
    """Visualizes detected cities on a map"""
    
    def __init__(self, map_width: int = 7168, map_height: int = 4096, 
                 output_scale: float = 0.1):
        """
        Args:
            map_width: Full map width in tiles
            map_height: Full map height in tiles
            output_scale: Scale factor for output image (0.1 = 10% size)
        """
        self.map_width = map_width
        self.map_height = map_height
        self.scale = output_scale
        self.output_width = int(map_width * output_scale)
        self.output_height = int(map_height * output_scale)
        
    def load_cities(self, json_file: str) -> List[Dict]:
        """Load cities from JSON file"""
        with open(json_file, 'r') as f:
            data = json.load(f)
        
        if 'cities' in data:
            return data['cities']
        elif 'towns' in data:
            # Convert extractable format to city format
            cities = []
            for name, bounds in data['towns'].items():
                cities.append({
                    'name': name,
                    'min_x': bounds[0],
                    'min_y': bounds[1],
                    'max_x': bounds[2],
                    'max_y': bounds[3],
                    'center_x': (bounds[0] + bounds[2]) // 2,
                    'center_y': (bounds[1] + bounds[3]) // 2,
                })
            return cities
        return []
    
    def scale_coord(self, coord: int) -> int:
        """Scale coordinate to output size"""
        return int(coord * self.scale)
    
    def create_map_image(self, cities: List[Dict], output_file: str):
        """Create visualization image"""
        if not HAS_PIL:
            print("ERROR: PIL/Pillow required for visualization")
            print("Install with: pip install pillow")
            return False
        
        # Create base image
        img = Image.new('RGB', (self.output_width, self.output_height), COLORS['background'])
        draw = ImageDraw.Draw(img)
        
        # Draw grid (every 1000 tiles)
        grid_spacing = 1000
        for x in range(0, self.map_width, grid_spacing):
            sx = self.scale_coord(x)
            draw.line([(sx, 0), (sx, self.output_height)], fill=COLORS['grid'], width=1)
        
        for y in range(0, self.map_height, grid_spacing):
            sy = self.scale_coord(y)
            draw.line([(0, sy), (self.output_width, sy)], fill=COLORS['grid'], width=1)
        
        # Draw coordinate labels at corners
        try:
            font = ImageFont.truetype("arial.ttf", 12)
        except:
            try:
                font = ImageFont.truetype("/usr/share/fonts/truetype/dejavu/DejaVuSans.ttf", 12)
            except:
                font = ImageFont.load_default()
        
        # Corner coordinates
        draw.text((10, 10), f"(0, 0)", fill=COLORS['city_label'], font=font)
        draw.text((self.output_width - 80, 10), f"({self.map_width}, 0)", fill=COLORS['city_label'], font=font)
        draw.text((10, self.output_height - 20), f"(0, {self.map_height})", fill=COLORS['city_label'], font=font)
        draw.text((self.output_width - 100, self.output_height - 20), 
                 f"({self.map_width}, {self.map_height})", fill=COLORS['city_label'], font=font)
        
        # Draw each city
        for i, city in enumerate(cities):
            self._draw_city(draw, city, i, font)
        
        # Draw legend
        self._draw_legend(draw, len(cities), font)
        
        # Save image
        img.save(output_file)
        print(f"Visualization saved to: {output_file}")
        print(f"Image size: {self.output_width}x{self.output_height} pixels")
        print(f"Scale: 1 pixel = {1/self.scale:.1f} tiles")
        
        return True
    
    def _draw_city(self, draw: ImageDraw.Draw, city: Dict, index: int, font):
        """Draw a single city on the map"""
        # Get coordinates
        min_x = self.scale_coord(city['min_x'])
        min_y = self.scale_coord(city['min_y'])
        max_x = self.scale_coord(city['max_x'])
        max_y = self.scale_coord(city['max_y'])
        center_x = self.scale_coord(city['center_x'])
        center_y = self.scale_coord(city['center_y'])
        
        # Draw filled rectangle (semi-transparent)
        # Note: PIL doesn't support alpha in RGB mode, so we'll use a lighter color
        fill_color = (255, 200, 0)  # Orange-yellow
        draw.rectangle([(min_x, min_y), (max_x, max_y)], 
                      fill=fill_color, outline=COLORS['city_border'], width=2)
        
        # Draw center point
        point_size = max(3, int(5 * self.scale))
        draw.ellipse([(center_x - point_size, center_y - point_size),
                     (center_x + point_size, center_y + point_size)],
                    fill=COLORS['city_border'], outline=COLORS['city_border'])
        
        # Draw label
        name = city.get('name', f'city_{index+1}')
        # Try to extract city number for shorter label
        if 'city_' in name:
            label = name.replace('scanned_', '').replace('city_', 'C')
        else:
            label = name[:8]  # Truncate long names
        
        # Position label above city (or below if too close to top)
        label_y = min_y - 15 if min_y > 20 else max_y + 5
        label_x = center_x - 20
        
        # Draw text with background for readability
        text_bbox = draw.textbbox((label_x, label_y), label, font=font)
        text_width = text_bbox[2] - text_bbox[0]
        text_height = text_bbox[3] - text_bbox[1]
        
        # Background rectangle for text
        draw.rectangle([(label_x - 2, label_y - 2), 
                      (label_x + text_width + 2, label_y + text_height + 2)],
                     fill=(0, 0, 0, 200), outline=COLORS['city_border'])
        
        draw.text((label_x, label_y), label, fill=COLORS['city_label'], font=font)
        
        # Draw coordinates below label
        coords_text = f"({city['center_x']},{city['center_y']})"
        coords_y = label_y + text_height + 3
        coords_bbox = draw.textbbox((label_x, coords_y), coords_text, font=font)
        coords_width = coords_bbox[2] - coords_bbox[0]
        draw.rectangle([(label_x - 2, coords_y - 2),
                       (label_x + coords_width + 2, coords_y + text_height + 2)],
                      fill=(0, 0, 0, 200), outline=COLORS['city_border'])
        draw.text((label_x, coords_y), coords_text, fill=COLORS['city_label'], font=font)
    
    def _draw_legend(self, draw: ImageDraw.Draw, city_count: int, font):
        """Draw legend in corner"""
        legend_x = self.output_width - 200
        legend_y = 10
        
        # Legend background
        draw.rectangle([(legend_x - 10, legend_y - 5), 
                       (self.output_width - 10, legend_y + 100)],
                      fill=(0, 0, 0, 200), outline=COLORS['city_border'])
        
        # Title
        draw.text((legend_x, legend_y), "Detected Cities", 
                 fill=COLORS['city_label'], font=font)
        
        # Count
        draw.text((legend_x, legend_y + 20), f"Total: {city_count}", 
                 fill=COLORS['city_label'], font=font)
        
        # Scale info
        scale_text = f"Scale: 1px = {1/self.scale:.0f} tiles"
        draw.text((legend_x, legend_y + 40), scale_text, 
                 fill=COLORS['city_label'], font=font)
        
        # Color key
        draw.rectangle([(legend_x, legend_y + 60), (legend_x + 20, legend_y + 75)],
                       fill=COLORS['city_border'], outline=COLORS['city_border'])
        draw.text((legend_x + 25, legend_y + 62), "City", 
                 fill=COLORS['city_label'], font=font)

def main():
    """Main visualization function"""
    import sys
    
    # Default paths
    cities_file = Path("Vystia Town Generator/scanned_cities/map0_cities.json")
    if not cities_file.exists():
        cities_file = Path("Vystia Town Generator/scanned_cities/map0_extractable.json")
    
    if len(sys.argv) > 1:
        cities_file = Path(sys.argv[1])
    
    if not cities_file.exists():
        print(f"ERROR: Cities file not found: {cities_file}")
        print("Usage: python visualize_detected_cities.py [cities_json_file] [output_file] [scale]")
        print("\nArguments:")
        print("  cities_json_file - Path to cities JSON (default: auto-detect)")
        print("  output_file      - Output PNG path (default: cities_file_visualization.png)")
        print("  scale            - Scale factor 0.01-1.0 (default: 0.1 = 10%)")
        print("\nExamples:")
        print("  python visualize_detected_cities.py")
        print("  python visualize_detected_cities.py cities.json output.png 0.2")
        print("\nLooking for files in:")
        print("  - Vystia Town Generator/scanned_cities/map0_cities.json")
        print("  - Vystia Town Generator/scanned_cities/map0_extractable.json")
        return 1
    
    # Output file
    output_file = cities_file.parent / f"{cities_file.stem}_visualization.png"
    if len(sys.argv) > 2:
        output_file = Path(sys.argv[2])
    
    # Scale factor
    scale = 0.1  # Default 10% scale
    if len(sys.argv) > 3:
        try:
            scale = float(sys.argv[3])
            if scale <= 0 or scale > 1:
                print(f"Warning: Scale {scale} out of range, using 0.1")
                scale = 0.1
        except ValueError:
            print(f"Warning: Invalid scale '{sys.argv[3]}', using 0.1")
            scale = 0.1
    
    print("="*80)
    print("CITY VISUALIZATION GENERATOR")
    print("="*80)
    print(f"Input file: {cities_file}")
    print(f"Output file: {output_file}")
    print(f"Scale: {scale} ({scale*100:.1f}% of original size)")
    print("="*80)
    
    # Create visualizer
    visualizer = CityVisualizer(
        map_width=7168,
        map_height=4096,
        output_scale=scale
    )
    
    # Load cities
    print(f"Loading cities from {cities_file}...")
    cities = visualizer.load_cities(str(cities_file))
    print(f"Loaded {len(cities)} cities")
    
    # Create visualization
    print("Generating visualization...")
    success = visualizer.create_map_image(cities, str(output_file))
    
    if success:
        print("\n" + "="*80)
        print("VISUALIZATION COMPLETE")
        print("="*80)
        print(f"Open the image to see all detected cities:")
        print(f"  {output_file.absolute()}")
        print("\nCity locations are marked with:")
        print("  - Yellow borders showing city boundaries")
        print("  - Orange fill showing city area")
        print("  - Labels with city names and coordinates")
        print("  - Grid lines every 1000 tiles")
        return 0
    else:
        return 1

if __name__ == "__main__":
    exit(main())

