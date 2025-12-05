"""
Dwarf Sprite Creator for Ultima Online
Extracts human body animations, resizes them to create dwarf sprites,
and writes them to a new body ID.

Usage:
    python dwarf_sprite_creator.py

Requirements:
    pip install Pillow numpy
"""

import os
import sys
import struct
from pathlib import Path
from PIL import Image
import numpy as np

# Configuration
UO_CLIENT_PATH = r"C:\Program Files (x86)\Electronic Arts\Ultima Online Classic"
OUTPUT_PATH = r"C:\DevEnv\GIT\UO\Vystia\tools\dwarf_sprites"

# Source body IDs (human male/female)
# Bodies 400/401 in anim.mul have robed figures but fit in 987/988 slots
# The plate armor will cover the robes
SOURCE_MALE_BODY = 400
SOURCE_FEMALE_BODY = 401

# Target body IDs for dwarves (unused range)
TARGET_MALE_BODY = 1600
TARGET_FEMALE_BODY = 1601

# Scale factor (75% = smaller dwarf)
SCALE_FACTOR = 0.75


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
        Human bodies (400+) use 35 actions * 5 directions = 175 entries per body.
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


class AnimationWriter:
    """Writes UO animation files"""

    def __init__(self, output_path):
        self.output_path = Path(output_path)
        self.output_path.mkdir(parents=True, exist_ok=True)

    def encode_frame(self, img, center_x, center_y):
        """Encode a single frame back to UO format"""
        width, height = img.size
        pixels = img.load()

        # Build palette from image colors
        colors = {}
        color_list = [(0, 0, 0, 0)]  # Index 0 = transparent

        for y in range(height):
            for x in range(width):
                c = pixels[x, y]
                if c[3] > 0 and c not in colors:  # Non-transparent
                    colors[c] = len(color_list)
                    color_list.append(c)

        # Pad palette to 256 entries
        while len(color_list) < 256:
            color_list.append((0, 0, 0, 0))

        # Encode palette
        palette_data = bytearray()
        for r, g, b, a in color_list:
            # Convert RGBA to UO 16-bit format
            color = 0x8000 if a > 0 else 0
            color |= ((r // 8) & 0x1F) << 10
            color |= ((g // 8) & 0x1F) << 5
            color |= (b // 8) & 0x1F
            color ^= 0x8000
            palette_data.extend(struct.pack('<H', color))

        # Encode frame header
        frame_data = bytearray()
        frame_data.extend(struct.pack('<h', center_x))
        frame_data.extend(struct.pack('<h', center_y))
        frame_data.extend(struct.pack('<H', width))
        frame_data.extend(struct.pack('<H', height))

        # Encode pixel runs
        DOUBLE_XOR = (0x200 << 22) | (0x200 << 12)
        x_base = center_x - 0x200
        y_base = (center_y + height) - 0x200

        for y in range(height):
            x = 0
            while x < width:
                # Skip transparent pixels
                while x < width and pixels[x, y][3] == 0:
                    x += 1

                if x >= width:
                    break

                # Find run of non-transparent pixels
                run_start = x
                run_data = bytearray()
                while x < width and pixels[x, y][3] > 0:
                    c = pixels[x, y]
                    if c in colors:
                        run_data.append(colors[c])
                    else:
                        run_data.append(0)
                    x += 1

                if len(run_data) > 0:
                    x_offset = run_start - x_base
                    y_offset = y - y_base

                    header = len(run_data) & 0xFFF
                    header |= (y_offset & 0x3FF) << 12
                    header |= (x_offset & 0x3FF) << 22
                    header ^= DOUBLE_XOR

                    frame_data.extend(struct.pack('<I', header))
                    frame_data.extend(run_data)

        # End marker
        frame_data.extend(struct.pack('<I', 0x7FFF7FFF))

        return palette_data, frame_data

    def save_frames_as_images(self, frames, body, action, direction):
        """Save frames as PNG images for inspection"""
        dir_path = self.output_path / f"body_{body}" / f"action_{action}" / f"dir_{direction}"
        dir_path.mkdir(parents=True, exist_ok=True)

        for i, (img, cx, cy) in enumerate(frames):
            img.save(dir_path / f"frame_{i:02d}.png")

        return dir_path


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
            # If alpha is below threshold, make fully transparent
            # Otherwise make fully opaque
            if a < threshold:
                pixels[x, y] = (0, 0, 0, 0)
            else:
                pixels[x, y] = (r, g, b, 255)

    return img


def resize_frame(img, center_x, center_y, scale):
    """Resize a frame while maintaining center point"""
    # First, threshold the alpha to remove semi-transparent pixels from source
    img = threshold_alpha(img.copy(), threshold=128)

    width, height = img.size
    new_width = max(1, int(width * scale))
    new_height = max(1, int(height * scale))

    # Use NEAREST neighbor for pixel art - preserves hard edges without anti-aliasing
    resized = img.resize((new_width, new_height), Image.Resampling.NEAREST)

    # Threshold again after resize to be safe
    resized = threshold_alpha(resized, threshold=128)

    # Adjust center point
    new_center_x = int(center_x * scale)
    new_center_y = int(center_y * scale)

    return resized, new_center_x, new_center_y


def process_body(reader, body_id, scale, output_path):
    """Process all animations for a body ID"""
    print(f"\nProcessing body {body_id}...")

    # Humans have 35 actions
    num_actions = 35
    num_directions = 5

    all_frames = {}

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
                    print(f"  Action {action:2d}, Dir {direction}: {len(frames)} frames")

    return all_frames


def save_as_images(frames_dict, body_id, output_path):
    """Save all frames as PNG images"""
    base_path = Path(output_path) / f"body_{body_id}"
    base_path.mkdir(parents=True, exist_ok=True)

    for (action, direction), frames in frames_dict.items():
        dir_path = base_path / f"action_{action:02d}_dir_{direction}"
        dir_path.mkdir(parents=True, exist_ok=True)

        for i, (img, cx, cy) in enumerate(frames):
            # Save image with center info in filename
            img.save(dir_path / f"frame_{i:02d}_c{cx}_{cy}.png")

    print(f"Saved images to {base_path}")


def main():
    print("=" * 60)
    print("Dwarf Sprite Creator for Ultima Online")
    print("=" * 60)

    # Check paths
    if not os.path.exists(UO_CLIENT_PATH):
        print(f"ERROR: UO client not found at {UO_CLIENT_PATH}")
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

    # Process male human body
    print(f"\n--- Processing Male Human (Body {SOURCE_MALE_BODY}) ---")
    male_frames = process_body(reader, SOURCE_MALE_BODY, SCALE_FACTOR, OUTPUT_PATH)

    if male_frames:
        save_as_images(male_frames, TARGET_MALE_BODY, OUTPUT_PATH)
        print(f"\nExtracted and resized {len(male_frames)} animation sets for male dwarf")
    else:
        print("WARNING: No frames extracted for male body")

    # Process female human body
    print(f"\n--- Processing Female Human (Body {SOURCE_FEMALE_BODY}) ---")
    female_frames = process_body(reader, SOURCE_FEMALE_BODY, SCALE_FACTOR, OUTPUT_PATH)

    if female_frames:
        save_as_images(female_frames, TARGET_FEMALE_BODY, OUTPUT_PATH)
        print(f"\nExtracted and resized {len(female_frames)} animation sets for female dwarf")
    else:
        print("WARNING: No frames extracted for female body")

    print("\n" + "=" * 60)
    print("DONE!")
    print(f"Output saved to: {OUTPUT_PATH}")
    print("\nNext steps:")
    print("1. Review the extracted PNG frames")
    print("2. Import into UO Fiddler at body IDs 1600/1601")
    print("3. Update bodyTable.cfg to add the new body IDs")
    print("4. Update Dwarf.cs to use Body = 1600/1601")
    print("=" * 60)


if __name__ == "__main__":
    main()
