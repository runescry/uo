"""
Creature Animation Extractor for Ultima Online
Extracts creature animations from anim.mul/anim.idx and saves as PNG images.

Usage:
    python creature_animation_extractor.py <body_id>

    Example: python creature_animation_extractor.py 1  (extracts Ogre animations)

Requirements:
    pip install Pillow
"""

import os
import sys
import struct
from pathlib import Path
from PIL import Image

# Configuration
UO_CLIENT_PATH = r"C:\DevEnv\GIT\UO\UO Adventures\Client\Data Files"
OUTPUT_PATH = r"C:\DevEnv\GIT\UO\Vystia\tools\creature_animations"


class AnimationReader:
    """Reads UO animation files (anim.idx, anim.mul) - supports multiple anim files"""

    def __init__(self, client_path):
        self.client_path = Path(client_path)

        # Try to find all anim files (anim.mul, anim2.mul, anim3.mul, etc.)
        self.anim_files = []
        for i in range(1, 6):  # Check anim.mul through anim5.mul
            if i == 1:
                idx_path = self.client_path / "anim.idx"
                mul_path = self.client_path / "anim.mul"
            else:
                idx_path = self.client_path / f"anim{i}.idx"
                mul_path = self.client_path / f"anim{i}.mul"

            if idx_path.exists() and mul_path.exists():
                self.anim_files.append((idx_path, mul_path, i))
                print(f"Found anim file {i}: {mul_path.name}")

        if not self.anim_files:
            raise FileNotFoundError(f"No anim files found at {self.client_path}")

    def get_index_offset(self, body, action, direction):
        """
        Calculate the index offset for a body/action/direction combo.

        UO Animation Index Structure:
        - Bodies 0-199: High detail monsters (22 actions × 5 directions = 110 entries)
        - Bodies 200-399: Low detail monsters (13 actions × 5 directions = 65 entries)
        - Bodies 400+: Humans/equipment (35 actions × 5 directions = 175 entries)
        """
        if body < 200:
            # High detail monsters: 22 actions × 5 directions = 110
            base_index = body * 110
            max_actions = 22
        elif body < 400:
            # Low detail monsters: 13 actions × 5 directions = 65
            base_index = 22000 + ((body - 200) * 65)
            max_actions = 13
        else:
            # Humans/equipment: 35 actions × 5 directions = 175
            base_index = 35000 + ((body - 400) * 175)
            max_actions = 35

        index = base_index + (action * 5)

        # Direction mapping (UO stores 5 directions, mirrors the rest)
        if direction <= 4:
            index += direction
        else:
            index += direction - (direction - 4) * 2

        return index, max_actions

    def read_index_entry(self, index):
        """Read a single index entry (12 bytes: offset, length, extra)"""
        with open(self.idx_path, 'rb') as f:
            f.seek(index * 12)
            data = f.read(12)
            if len(data) < 12:
                return None, None, None
            offset, length, extra = struct.unpack('<III', data)
            if offset == 0xFFFFFFFF or length == 0:
                return None, None, None
            return offset, length, extra

    def read_animation_data(self, body, action, direction):
        """Read raw animation data for a body/action/direction from any available anim file"""
        index, _ = self.get_index_offset(body, action, direction)

        # Try each anim file until we find the animation
        for idx_path, mul_path, file_num in self.anim_files:
            offset, length, extra = self.read_index_entry_from_file(idx_path, index)

            if offset is not None:
                with open(mul_path, 'rb') as f:
                    f.seek(offset)
                    return f.read(length)

        return None

    def read_index_entry_from_file(self, idx_path, index):
        """Read a single index entry from a specific idx file"""
        try:
            with open(idx_path, 'rb') as f:
                f.seek(index * 12)
                data = f.read(12)
                if len(data) < 12:
                    return None, None, None
                offset, length, extra = struct.unpack('<III', data)
                if offset == 0xFFFFFFFF or length == 0:
                    return None, None, None
                return offset, length, extra
        except:
            return None, None, None

    def decode_animation(self, data):
        """
        Decode animation data into frames.
        Returns list of (bitmap, center_x, center_y) tuples.
        """
        if data is None or len(data) < 512:
            return []

        # Read palette (256 colors, 2 bytes each)
        palette = []
        for i in range(256):
            color = struct.unpack('<H', data[i*2:(i+1)*2])[0]
            color ^= 0x8000  # UO color format conversion
            # Convert 16-bit ARGB1555 to RGBA
            a = 255 if (color & 0x8000) else 0
            r = ((color >> 10) & 0x1F) * 8
            g = ((color >> 5) & 0x1F) * 8
            b = (color & 0x1F) * 8
            palette.append((r, g, b, a))

        # Read frame count and lookups
        pos = 512
        frame_count = struct.unpack('<I', data[pos:pos+4])[0]
        pos += 4

        lookups = []
        for i in range(frame_count):
            lookup = struct.unpack('<I', data[pos:pos+4])[0]
            lookups.append(512 + lookup)  # Offset from start of palette
            pos += 4

        frames = []
        for lookup in lookups:
            frame = self.decode_frame(data, lookup, palette)
            if frame:
                frames.append(frame)

        return frames

    def decode_frame(self, data, offset, palette):
        """Decode a single frame from animation data"""
        try:
            pos = offset
            center_x = struct.unpack('<h', data[pos:pos+2])[0]
            center_y = struct.unpack('<h', data[pos+2:pos+4])[0]
            width = struct.unpack('<H', data[pos+4:pos+6])[0]
            height = struct.unpack('<H', data[pos+6:pos+8])[0]
            pos += 8

            if width == 0 or height == 0:
                return None

            # Create RGBA image
            img = Image.new('RGBA', (width, height), (0, 0, 0, 0))
            pixels = img.load()

            DOUBLE_XOR = (0x200 << 22) | (0x200 << 12)
            x_base = center_x - 0x200
            y_base = (center_y + height) - 0x200

            while True:
                if pos + 4 > len(data):
                    break
                header = struct.unpack('<I', data[pos:pos+4])[0]
                pos += 4

                if header == 0x7FFF7FFF:
                    break

                header ^= DOUBLE_XOR

                run_length = header & 0xFFF
                y_offset = (header >> 12) & 0x3FF
                x_offset = (header >> 22) & 0x3FF

                x = x_base + x_offset
                y = y_base + y_offset

                for i in range(run_length):
                    if pos >= len(data):
                        break
                    color_idx = data[pos]
                    pos += 1

                    px = x + i
                    py = y

                    if 0 <= px < width and 0 <= py < height:
                        pixels[px, py] = palette[color_idx]

            return (img, center_x, center_y)

        except Exception as e:
            print(f"Error decoding frame at offset {offset}: {e}")
            return None


def process_creature(reader, body_id, output_path):
    """Extract all animations for a creature body ID"""
    print(f"\n{'='*60}")
    print(f"Extracting animations for body ID: {body_id}")
    print(f"{'='*60}\n")

    # Determine number of actions based on body range
    if body_id < 200:
        num_actions = 22  # High detail monsters
        body_type = "High Detail Monster"
    elif body_id < 400:
        num_actions = 13  # Low detail monsters
        body_type = "Low Detail Monster"
    else:
        num_actions = 35  # Humans/equipment
        body_type = "Human/Equipment"

    print(f"Body Type: {body_type} ({num_actions} actions)")

    num_directions = 5
    all_frames = {}
    total_frames = 0

    for action in range(num_actions):
        for direction in range(num_directions):
            data = reader.read_animation_data(body_id, action, direction)
            if data:
                frames = reader.decode_animation(data)
                if frames:
                    all_frames[(action, direction)] = frames
                    total_frames += len(frames)
                    print(f"  Action {action:2d}, Dir {direction}: {len(frames)} frames")

    if not all_frames:
        print(f"\n⚠️  WARNING: No animations found for body {body_id}")
        print("   This body ID may be unused or invalid.")
        return None

    # Save all frames as PNG images
    base_path = Path(output_path) / f"body_{body_id}"
    base_path.mkdir(parents=True, exist_ok=True)

    for (action, direction), frames in all_frames.items():
        dir_path = base_path / f"action_{action:02d}_dir_{direction}"
        dir_path.mkdir(parents=True, exist_ok=True)

        for i, (img, cx, cy) in enumerate(frames):
            # Save image with center info in filename
            img.save(dir_path / f"frame_{i:02d}_c{cx}_{cy}.png")

    print(f"\n✓ Extracted {len(all_frames)} animation sets ({total_frames} total frames)")
    print(f"✓ Saved to: {base_path}")

    return all_frames


def main():
    if len(sys.argv) < 2:
        print("Usage: python creature_animation_extractor.py <body_id>")
        print("\nExample:")
        print("  python creature_animation_extractor.py 1")
        print("\nCommon creature body IDs:")
        print("  1   = Ogre")
        print("  2   = Ettin")
        print("  3   = Zombie")
        print("  4   = Gargoyle")
        print("  5   = Eagle")
        print("  6   = Bird")
        print("  7   = Orc")
        print("  8   = Corpser")
        print("  9   = Daemon")
        print("  50  = Dragon")
        return

    try:
        body_id = int(sys.argv[1])
    except ValueError:
        print(f"ERROR: Invalid body ID '{sys.argv[1]}'. Must be a number.")
        return

    # Check paths
    if not os.path.exists(UO_CLIENT_PATH):
        print(f"ERROR: UO client data not found at {UO_CLIENT_PATH}")
        print("Please edit UO_CLIENT_PATH in this script.")
        return

    # Create reader
    try:
        reader = AnimationReader(UO_CLIENT_PATH)
    except FileNotFoundError as e:
        print(f"ERROR: {e}")
        return

    # Create output directory
    os.makedirs(OUTPUT_PATH, exist_ok=True)

    # Extract creature animations
    process_creature(reader, body_id, OUTPUT_PATH)

    print(f"\n{'='*60}")
    print("DONE!")
    print(f"{'='*60}\n")


if __name__ == "__main__":
    main()
