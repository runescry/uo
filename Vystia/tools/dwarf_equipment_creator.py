"""
Dwarf Equipment Sprite Creator for Ultima Online
Extracts equipment animations (plate armor, warhammer), resizes them to 75%,
and writes them to new equipment IDs for dwarf-sized gear.

Equipment animations work differently from mob animations:
- Equipment uses item graphic IDs as body IDs
- Equipment type in mobtypes.txt has flag 0 (reads from anim.mul)
- Equipment animations are layered on top of the body animation

Usage:
    python dwarf_equipment_creator.py

Requirements:
    pip install Pillow
"""

import os
import sys
import struct
from pathlib import Path
from PIL import Image

# Configuration
UO_CLIENT_PATH = r"C:\Program Files (x86)\Electronic Arts\Ultima Online Classic"
OUTPUT_PATH = r"C:\DevEnv\GIT\UO\Vystia\tools\dwarf_equipment_sprites"

# Scale factor (75% = same as dwarf body)
SCALE_FACTOR = 0.75

# Source equipment animation body IDs (from anim.mul)
# These are the animation slot IDs, not item graphic IDs

# Male plate armor
PLATE_CHEST_ID = 527         # Platemail chest
PLATE_ARMS_ID = 528          # Platemail arms
PLATE_LEGS_ID = 529          # Platemail legs
PLATE_GLOVES_ID = 530        # Platemail gloves
WARHAMMER_ID = 646           # Warhammer

# Female leather armor
LEATHER_TUNIC_ID = 542       # Leather tunic
LEATHER_LEGGINGS_ID = 543    # Leather leggings
LEATHER_SLEEVES_ID = 544     # Leather sleeves
LEATHER_GLOVES_ID = 545      # Leather gloves
LEATHER_GORGET_ID = 546      # Leather gorget

# All source equipment IDs to process
SOURCE_EQUIPMENT = {
    'plate_chest': PLATE_CHEST_ID,
    'plate_arms': PLATE_ARMS_ID,
    'plate_legs': PLATE_LEGS_ID,
    'plate_gloves': PLATE_GLOVES_ID,
    'warhammer': WARHAMMER_ID,
    'leather_tunic': LEATHER_TUNIC_ID,
    'leather_leggings': LEATHER_LEGGINGS_ID,
    'leather_sleeves': LEATHER_SLEEVES_ID,
    'leather_gloves': LEATHER_GLOVES_ID,
    'leather_gorget': LEATHER_GORGET_ID,
}


class AnimationReader:
    """Reads UO animation files (anim.idx, anim.mul)"""

    def __init__(self, client_path):
        self.client_path = Path(client_path)
        self.idx_path = self.client_path / "anim.idx"
        self.mul_path = self.client_path / "anim.mul"

        if not self.idx_path.exists():
            raise FileNotFoundError(f"anim.idx not found at {self.idx_path}")
        if not self.mul_path.exists():
            raise FileNotFoundError(f"anim.mul not found at {self.mul_path}")

    def get_index_offset(self, body, action, direction):
        """
        Calculate the index offset for a body/action/direction combo.
        Equipment bodies use the same formula as other bodies.
        """
        if body < 200:
            # High detail monsters: 22 actions * 5 directions = 110
            base_index = body * 110
        elif body < 400:
            # Low detail monsters: 13 actions * 5 directions = 65
            base_index = 22000 + ((body - 200) * 65)
        else:
            # Humans/equipment: 35 actions * 5 directions = 175
            base_index = 35000 + ((body - 400) * 175)

        index = base_index + (action * 5)

        # Direction mapping (UO stores 5 directions, mirrors the rest)
        if direction <= 4:
            index += direction
        else:
            index += direction - (direction - 4) * 2

        return index

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
        """Read raw animation data for a body/action/direction"""
        index = self.get_index_offset(body, action, direction)
        offset, length, extra = self.read_index_entry(index)

        if offset is None:
            return None

        with open(self.mul_path, 'rb') as f:
            f.seek(offset)
            return f.read(length)

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


def threshold_alpha(img, threshold=128):
    """
    Force all pixels to be either fully transparent or fully opaque.
    This removes semi-transparent edge pixels that cause halo artifacts.
    """
    pixels = img.load()
    width, height = img.size

    for y in range(height):
        for x in range(width):
            r, g, b, a = pixels[x, y]
            if a < threshold:
                pixels[x, y] = (0, 0, 0, 0)
            else:
                pixels[x, y] = (r, g, b, 255)

    return img


def resize_frame(img, center_x, center_y, scale):
    """Resize a frame while maintaining center point"""
    # First, threshold the alpha to remove semi-transparent pixels
    img = threshold_alpha(img.copy(), threshold=128)

    width, height = img.size
    new_width = max(1, int(width * scale))
    new_height = max(1, int(height * scale))

    # Use NEAREST neighbor for pixel art
    resized = img.resize((new_width, new_height), Image.Resampling.NEAREST)

    # Threshold again after resize
    resized = threshold_alpha(resized, threshold=128)

    # Adjust center point
    new_center_x = int(center_x * scale)
    new_center_y = int(center_y * scale)

    return resized, new_center_x, new_center_y


def process_equipment(reader, body_id, name, scale, output_path):
    """Process all animations for an equipment body ID"""
    print(f"\nProcessing {name} (body {body_id} / 0x{body_id:04X})...")

    # Equipment uses same action/direction structure as humans (35 actions, 5 directions)
    num_actions = 35
    num_directions = 5

    all_frames = {}
    total_frames = 0

    for action in range(num_actions):
        for direction in range(num_directions):
            data = reader.read_animation_data(body_id, action, direction)
            if data:
                frames = reader.decode_animation(data)
                if frames:
                    # Resize each frame
                    resized_frames = []
                    for img, cx, cy in frames:
                        new_img, new_cx, new_cy = resize_frame(img, cx, cy, scale)
                        resized_frames.append((new_img, new_cx, new_cy))

                    all_frames[(action, direction)] = resized_frames
                    total_frames += len(frames)

    if total_frames > 0:
        print(f"  Found {len(all_frames)} animation sets, {total_frames} total frames")
    else:
        print(f"  No animation data found")

    return all_frames


def save_as_images(frames_dict, name, body_id, output_path):
    """Save all frames as PNG images"""
    base_path = Path(output_path) / f"{name}_body_{body_id}"
    base_path.mkdir(parents=True, exist_ok=True)

    for (action, direction), frames in frames_dict.items():
        dir_path = base_path / f"action_{action:02d}_dir_{direction}"
        dir_path.mkdir(parents=True, exist_ok=True)

        for i, (img, cx, cy) in enumerate(frames):
            img.save(dir_path / f"frame_{i:02d}_c{cx}_{cy}.png")

    print(f"  Saved to {base_path}")


def main():
    print("=" * 60)
    print("Dwarf Equipment Sprite Creator for Ultima Online")
    print("=" * 60)

    # Check paths
    if not os.path.exists(UO_CLIENT_PATH):
        print(f"ERROR: UO client not found at {UO_CLIENT_PATH}")
        return

    # Create reader
    try:
        reader = AnimationReader(UO_CLIENT_PATH)
    except FileNotFoundError as e:
        print(f"ERROR: {e}")
        return

    # Create output directory
    os.makedirs(OUTPUT_PATH, exist_ok=True)

    # Process each equipment piece
    for name, body_id in SOURCE_EQUIPMENT.items():
        frames = process_equipment(reader, body_id, name, SCALE_FACTOR, OUTPUT_PATH)
        if frames:
            save_as_images(frames, name, body_id, OUTPUT_PATH)

    print("\n" + "=" * 60)
    print("DONE!")
    print(f"Output saved to: {OUTPUT_PATH}")
    print("\nNext steps:")
    print("1. Review the extracted PNG frames")
    print("2. Run dwarf_equipment_writer.py to write to anim.mul")
    print("=" * 60)


if __name__ == "__main__":
    main()
