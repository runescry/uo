"""
Smart Town Generator - Uses extracted OSI building templates
Generates realistic towns based on real OSI Britain structures
"""

import struct
import json
import random
from pathlib import Path
from typing import List, Dict

# Map dimensions
MAP_WIDTH_BLOCKS = 896
MAP_HEIGHT_BLOCKS = 512

def get_block_number(block_x: int, block_y: int) -> int:
    """CentrED's block calculation"""
    return block_x * MAP_HEIGHT_BLOCKS + block_y

class SmartTownGenerator:
    """Generates towns using real OSI building templates"""

    def __init__(self, name: str, center_x: int, center_y: int):
        self.name = name
        self.center_x = center_x
        self.center_y = center_y
        self.statics = []
        self.templates = []
        self.load_templates()

    def load_templates(self):
        """Load extracted building templates"""
        template_file = Path('extracted_building_templates.json')
        if template_file.exists():
            with open(template_file, 'r') as f:
                data = json.load(f)
                self.templates = data['templates']
            print(f"Loaded {len(self.templates)} building templates")
        else:
            print("WARNING: No building templates found!")

    def place_building(self, template_id: int, x: int, y: int, z_offset: int = 0):
        """Place a building template at specified coordinates"""
        if template_id >= len(self.templates):
            return

        template = self.templates[template_id]

        for static in template['statics']:
            # Adjust Z-level: OSI Britain was at Z=-5, Vystia is at Z=0
            # So add +5 to all Z-levels to bring buildings above ground
            adjusted_z = static['z'] + 5 + z_offset

            # Clamp to valid range
            if adjusted_z < -128:
                adjusted_z = -128
            elif adjusted_z > 127:
                adjusted_z = 127

            self.statics.append({
                'tile_id': static['tile_id'],
                'x': x + static['x'],
                'y': y + static['y'],
                'z': adjusted_z,
                'hue': static.get('hue', 0)
            })

    def create_road_network(self, road_tile: int = 0x0520):
        """Create main road network"""
        print("Creating road network...")

        # Main north-south road
        for y in range(self.center_y - 80, self.center_y + 80):
            for dx in range(-2, 3):  # 5 tiles wide
                self.statics.append({
                    'tile_id': road_tile,
                    'x': self.center_x + dx,
                    'y': y,
                    'z': 0,
                    'hue': 0
                })

        # Main east-west road
        for x in range(self.center_x - 80, self.center_x + 80):
            for dy in range(-2, 3):  # 5 tiles wide
                self.statics.append({
                    'tile_id': road_tile,
                    'x': x,
                    'y': self.center_y + dy,
                    'z': 0,
                    'hue': 0
                })

        # Secondary roads
        # North-south roads at +/- 40 tiles
        for offset in [-40, 40]:
            for y in range(self.center_y - 80, self.center_y + 80):
                for dx in range(-1, 2):  # 3 tiles wide
                    self.statics.append({
                        'tile_id': road_tile,
                        'x': self.center_x + offset + dx,
                        'y': y,
                        'z': 0,
                        'hue': 0
                    })

        # East-west roads at +/- 40 tiles
        for offset in [-40, 40]:
            for x in range(self.center_x - 80, self.center_x + 80):
                for dy in range(-1, 2):  # 3 tiles wide
                    self.statics.append({
                        'tile_id': road_tile,
                        'x': x,
                        'y': self.center_y + offset + dy,
                        'z': 0,
                        'hue': 0
                    })

        print(f"  Created road network with {len(self.statics)} road tiles")

    def place_buildings_in_grid(self):
        """Place buildings in a grid pattern using templates"""
        print("Placing buildings...")

        if not self.templates:
            print("  ERROR: No templates available!")
            return

        building_count = 0

        # Define building placement zones (between roads)
        zones = [
            # NW quadrant
            {'x_range': (-70, -45), 'y_range': (-70, -45), 'spacing': 12},
            {'x_range': (-35, -10), 'y_range': (-70, -45), 'spacing': 12},
            {'x_range': (-70, -45), 'y_range': (-35, -10), 'spacing': 12},
            {'x_range': (-35, -10), 'y_range': (-35, -10), 'spacing': 12},

            # NE quadrant
            {'x_range': (10, 35), 'y_range': (-70, -45), 'spacing': 12},
            {'x_range': (45, 70), 'y_range': (-70, -45), 'spacing': 12},
            {'x_range': (10, 35), 'y_range': (-35, -10), 'spacing': 12},
            {'x_range': (45, 70), 'y_range': (-35, -10), 'spacing': 12},

            # SW quadrant
            {'x_range': (-70, -45), 'y_range': (10, 35), 'spacing': 12},
            {'x_range': (-35, -10), 'y_range': (10, 35), 'spacing': 12},
            {'x_range': (-70, -45), 'y_range': (45, 70), 'spacing': 12},
            {'x_range': (-35, -10), 'y_range': (45, 70), 'spacing': 12},

            # SE quadrant
            {'x_range': (10, 35), 'y_range': (10, 35), 'spacing': 12},
            {'x_range': (45, 70), 'y_range': (10, 35), 'spacing': 12},
            {'x_range': (10, 35), 'y_range': (45, 70), 'spacing': 12},
            {'x_range': (45, 70), 'y_range': (45, 70), 'spacing': 12},
        ]

        for zone in zones:
            x_start, x_end = zone['x_range']
            y_start, y_end = zone['y_range']
            spacing = zone['spacing']

            # Place 1-2 buildings per zone
            num_buildings = random.randint(1, 2)

            for _ in range(num_buildings):
                # Random position in zone
                x_offset = random.randint(x_start, x_end - 10)
                y_offset = random.randint(y_start, y_end - 10)

                x = self.center_x + x_offset
                y = self.center_y + y_offset

                # Pick a random template
                template_id = random.randint(0, len(self.templates) - 1)

                self.place_building(template_id, x, y, z_offset=0)
                building_count += 1

        print(f"  Placed {building_count} buildings using OSI templates")

    def generate_town(self):
        """Generate complete town"""
        print(f"\n{'='*60}")
        print(f"GENERATING SMART TOWN: {self.name}")
        print(f"{'='*60}")
        print(f"Center: ({self.center_x}, {self.center_y})")
        print(f"Using {len(self.templates)} OSI building templates")
        print()

        self.statics = []

        # Create road network first
        self.create_road_network()

        # Place buildings in grid
        self.place_buildings_in_grid()

        print()
        print(f"{'='*60}")
        print(f"GENERATION COMPLETE")
        print(f"{'='*60}")
        print(f"Total statics: {len(self.statics)}")

        return self.statics


def deploy_town_to_mul(statics: List[Dict], target_statics: Path, target_staidx: Path):
    """Deploy to MUL files"""
    print(f"\n{'='*60}")
    print(f"DEPLOYING TO MUL FILES")
    print(f"{'='*60}")

    # Load existing
    print(f"Loading existing map...")
    vystia_blocks = {}

    with open(target_staidx, 'rb') as idx, open(target_statics, 'rb') as sta:
        for i in range(MAP_WIDTH_BLOCKS * MAP_HEIGHT_BLOCKS):
            idx.seek(i * 12)
            offset = struct.unpack('<I', idx.read(4))[0]
            length = struct.unpack('<I', idx.read(4))[0]

            if offset == 0xFFFFFFFF or length == 0:
                continue

            sta.seek(offset)
            count = length // 7

            vystia_blocks[i] = []
            for _ in range(count):
                tile_id = struct.unpack('<H', sta.read(2))[0]
                x = struct.unpack('B', sta.read(1))[0]
                y = struct.unpack('B', sta.read(1))[0]
                z = struct.unpack('b', sta.read(1))[0]
                hue = struct.unpack('<H', sta.read(2))[0]
                vystia_blocks[i].append({'tile_id': tile_id, 'x': x, 'y': y, 'z': z, 'hue': hue})

    print(f"Loaded {len(vystia_blocks)} existing blocks")

    # Group new statics
    print(f"Grouping {len(statics)} statics...")
    new_blocks = {}

    for static in statics:
        block_x = static['x'] // 8
        block_y = static['y'] // 8
        local_x = static['x'] & 0x7
        local_y = static['y'] & 0x7
        block_id = get_block_number(block_x, block_y)

        if block_id not in new_blocks:
            new_blocks[block_id] = []

        new_blocks[block_id].append({
            'tile_id': static['tile_id'],
            'x': local_x,
            'y': local_y,
            'z': static['z'],
            'hue': static['hue']
        })

    print(f"Grouped into {len(new_blocks)} blocks")

    # Merge
    for block_id, statics_list in new_blocks.items():
        if block_id in vystia_blocks:
            vystia_blocks[block_id].extend(statics_list)
        else:
            vystia_blocks[block_id] = statics_list

    print(f"Total blocks: {len(vystia_blocks)}")

    # Write
    print(f"Writing to files...")
    with open(target_statics, 'wb') as sta, open(target_staidx, 'wb') as idx:
        current_offset = 0

        for block_id in range(MAP_WIDTH_BLOCKS * MAP_HEIGHT_BLOCKS):
            if block_id in vystia_blocks and len(vystia_blocks[block_id]) > 0:
                statics_list = vystia_blocks[block_id]

                for s in statics_list:
                    sta.write(struct.pack('<H', s['tile_id']))
                    sta.write(struct.pack('B', s['x']))
                    sta.write(struct.pack('B', s['y']))
                    sta.write(struct.pack('b', s['z']))
                    sta.write(struct.pack('<H', s['hue']))

                length = len(statics_list) * 7
                idx.write(struct.pack('<I', current_offset))
                idx.write(struct.pack('<I', length))
                idx.write(struct.pack('<I', 0))
                current_offset += length
            else:
                idx.write(struct.pack('<I', 0xFFFFFFFF))
                idx.write(struct.pack('<I', 0xFFFFFFFF))
                idx.write(struct.pack('<I', 0))

            if block_id % 50000 == 0 and block_id > 0:
                print(f"  Progress: {block_id}/{MAP_WIDTH_BLOCKS * MAP_HEIGHT_BLOCKS}...")

    print(f"\n{'='*60}")
    print(f"DEPLOYMENT COMPLETE!")
    print(f"{'='*60}")


if __name__ == '__main__':
    import shutil

    # Generate town at (1750, 800) using OSI templates
    generator = SmartTownGenerator(
        name="Vystia Capital",
        center_x=1750,
        center_y=800
    )

    statics = generator.generate_town()

    # Save JSON
    with open('smart_town.json', 'w') as f:
        json.dump({'name': generator.name, 'statics': statics}, f, indent=2)

    # Deploy
    target_statics = Path(r'C:\Program Files (x86)\Electronic Arts\Ultima Online Classic\statics0.mul')
    target_staidx = Path(r'C:\Program Files (x86)\Electronic Arts\Ultima Online Classic\staidx0.mul')

    deploy_town_to_mul(statics, target_statics, target_staidx)

    # Copy to CentrED
    print(f"\nCopying to CentrED...")
    shutil.copy(target_statics, r'C:\DevEnv\GIT\UO\ServUO-fresh\Data\Maps\statics0.mul')
    shutil.copy(target_staidx, r'C:\DevEnv\GIT\UO\ServUO-fresh\Data\Maps\staidx0.mul')

    print(f"\nDone! Smart town at (1750, 800)")
