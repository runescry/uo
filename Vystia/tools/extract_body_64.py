#!/usr/bin/env python3
"""
Extract Body 64 from UO Adventures client
Saves all animation frames as PNG files for preview
"""

import struct
import os
from PIL import Image
from pathlib import Path

# UO Color conversion
def convert_uo_color(color_16):
    """Convert UO 16-bit color (ARGB1555) to RGBA"""
    color_16 ^= 0x8000  # XOR with 0x8000

    r = ((color_16 >> 10) & 0x1F) << 3
    g = ((color_16 >> 5) & 0x1F) << 3
    b = (color_16 & 0x1F) << 3
    a = 255 if (color_16 & 0x8000) else 0

    return (r, g, b, a)

class AnimationExtractor:
    DOUBLE_XOR = 0x200  # For run header decoding

    def __init__(self, mul_path, idx_path):
        self.mul_path = mul_path
        self.idx_path = idx_path

    def get_index_offset(self, body_id, action, direction):
        """Calculate index entry position"""
        if body_id < 200:
            base = body_id * 110
        elif body_id < 400:
            base = 22000 + ((body_id - 200) * 65)
        else:
            base = 35000 + ((body_id - 400) * 175)

        return (base + (action * 5) + direction) * 12

    def read_animation_data(self, body_id, action, direction):
        """Read raw animation data from mul file"""
        idx_offset = self.get_index_offset(body_id, action, direction)

        # Read index entry
        with open(self.idx_path, 'rb') as idx_file:
            idx_file.seek(idx_offset)
            offset, length, extra = struct.unpack('<III', idx_file.read(12))

        if offset == 0 or length == 0 or offset == 0xFFFFFFFF:
            return None

        # Read animation data
        with open(self.mul_path, 'rb') as mul_file:
            mul_file.seek(offset)
            return mul_file.read(length)

    def decode_animation(self, anim_data):
        """Decode animation data into frames"""
        if not anim_data:
            return None

        offset = 0

        # Read palette (256 colors × 2 bytes)
        palette = []
        for i in range(256):
            color_16 = struct.unpack_from('<H', anim_data, offset)[0]
            palette.append(convert_uo_color(color_16))
            offset += 2

        # Read frame count
        frame_count = struct.unpack_from('<I', anim_data, offset)[0]
        offset += 4

        # Read frame lookups (offsets)
        frame_lookups = []
        for i in range(frame_count):
            lookup = struct.unpack_from('<I', anim_data, offset)[0]
            frame_lookups.append(lookup)
            offset += 4

        # Decode frames
        frames = []
        lookup_base = 512 + 4  # After palette and frame count

        for frame_idx in range(frame_count):
            frame_offset = lookup_base + frame_lookups[frame_idx]
            frame_img = self.decode_frame(anim_data, frame_offset, palette)
            if frame_img:
                frames.append(frame_img)

        return frames

    def decode_frame(self, data, offset, palette):
        """Decode a single animation frame"""
        try:
            # Read frame header
            center_x = struct.unpack_from('<h', data, offset)[0]
            offset += 2
            center_y = struct.unpack_from('<h', data, offset)[0]
            offset += 2
            width = struct.unpack_from('<H', data, offset)[0]
            offset += 2
            height = struct.unpack_from('<H', data, offset)[0]
            offset += 2

            if width == 0 or height == 0 or width > 1024 or height > 1024:
                return None

            # Create image
            img = Image.new('RGBA', (width, height), (0, 0, 0, 0))
            pixels = img.load()

            # Decode runs
            while True:
                # Read run header (4 bytes)
                if offset + 4 > len(data):
                    break

                run_header = struct.unpack_from('<I', data, offset)[0]
                offset += 4

                # Check for end marker
                if run_header == 0x7FFF7FFF:
                    break

                # Decode run header (XOR with DOUBLE_XOR)
                run_header ^= self.DOUBLE_XOR

                x_offset = (run_header >> 22) & 0x3FF
                y_offset = (run_header >> 12) & 0x3FF
                run_length = run_header & 0xFFF

                # Sign extend if needed
                if x_offset & 0x200:
                    x_offset |= 0xFFFFFC00
                if y_offset & 0x200:
                    y_offset |= 0xFFFFFC00

                # Convert to signed
                if x_offset >= 0x200:
                    x_offset = x_offset - 0x400
                if y_offset >= 0x200:
                    y_offset = y_offset - 0x400

                # Read pixel data
                for i in range(run_length):
                    if offset >= len(data):
                        break

                    palette_idx = data[offset]
                    offset += 1

                    x = x_offset + i
                    y = y_offset

                    if 0 <= x < width and 0 <= y < height:
                        pixels[x, y] = palette[palette_idx]

            return img

        except Exception as e:
            print(f"Error decoding frame: {e}")
            return None

    def extract_body(self, body_id, output_dir, max_actions=22):
        """Extract all animations for a body ID"""
        output_path = Path(output_dir)
        output_path.mkdir(parents=True, exist_ok=True)

        print(f"Extracting body {body_id} to {output_dir}")

        total_frames = 0
        for action in range(max_actions):
            for direction in range(5):
                # Read animation data
                anim_data = self.read_animation_data(body_id, action, direction)

                if not anim_data:
                    continue

                # Decode frames
                frames = self.decode_animation(anim_data)

                if not frames:
                    continue

                # Save frames
                action_dir = output_path / f"action_{action:02d}" / f"dir_{direction}"
                action_dir.mkdir(parents=True, exist_ok=True)

                for frame_idx, frame_img in enumerate(frames):
                    filename = action_dir / f"frame_{frame_idx:03d}.png"
                    frame_img.save(filename)
                    total_frames += 1

                print(f"  Action {action:02d} Dir {direction}: {len(frames)} frames")

        print(f"\nTotal frames extracted: {total_frames}")
        return total_frames

if __name__ == "__main__":
    # Source: UO Adventures client
    base_path = r"C:\DevEnv\GIT\UO\UO Adventures\Client\Data Files"

    # Try all anim files (anim, anim2, anim3, anim4, anim5)
    anim_files = [
        ("anim.mul", "anim.idx"),
        ("anim2.mul", "anim2.idx"),
        ("anim3.mul", "anim3.idx"),
        ("anim4.mul", "anim4.idx"),
        ("anim5.mul", "anim5.idx"),
    ]

    output_dir = r"C:\DevEnv\GIT\UO\Vystia\extracted_animations\body_64_CovenMatriarch"

    body_id = 64
    total_extracted = 0

    print(f"Searching for body {body_id} across all anim files...")
    print()

    for mul_name, idx_name in anim_files:
        source_mul = f"{base_path}\\{mul_name}"
        source_idx = f"{base_path}\\{idx_name}"

        # Check if files exist
        import os
        if not os.path.exists(source_mul) or not os.path.exists(source_idx):
            print(f"Skipping {mul_name} (not found)")
            continue

        print(f"Checking {mul_name}...")

        # Try to read first frame to see if body exists
        extractor = AnimationExtractor(source_mul, source_idx)
        test_data = extractor.read_animation_data(body_id, 0, 0)

        if test_data:
            print(f"  Found body {body_id} in {mul_name}!")
            frames = extractor.extract_body(body_id, output_dir, max_actions=22)
            total_extracted += frames
            break
        else:
            print(f"  Body {body_id} not found in {mul_name}")

    print()
    if total_extracted > 0:
        print(f"Extraction complete! Total frames: {total_extracted}")
        print(f"Review frames in: {output_dir}")
    else:
        print(f"ERROR: Body {body_id} not found in any anim files!")
