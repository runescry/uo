"""
Dwarf Hair Sprite Creator for Ultima Online
Extracts pigtails hair animation (AnimID 902), resizes to 75%,
and writes to a new animation ID for dwarf-sized hair.

Usage:
    python dwarf_hair_creator.py

Requirements:
    pip install Pillow
"""

import os
import sys
import struct
import shutil
from pathlib import Path
from datetime import datetime
from PIL import Image

# Configuration
UO_CLIENT_PATH = r"C:\Program Files (x86)\Electronic Arts\Ultima Online Classic"
OUTPUT_PATH = r"C:\DevEnv\GIT\UO\Vystia\tools\patched_client"
BACKUP_PATH = r"C:\DevEnv\GIT\UO\Vystia\tools\backups"
SPRITES_PATH = r"C:\DevEnv\GIT\UO\Vystia\tools\dwarf_hair_sprites"

# Scale factor (75% = same as dwarf body)
SCALE_FACTOR = 0.75

# Source hair animation ID (pigtails = AnimID 902 from tiledata)
PIGTAILS_ANIM_ID = 902

# Target animation ID for dwarf-sized pigtails
# Using 920 - after the dwarf equipment IDs (909-919)
DWARF_PIGTAILS_ANIM_ID = 920


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
        """Calculate the index offset for a body/action/direction combo."""
        if body < 200:
            base_index = body * 110
        elif body < 400:
            base_index = 22000 + ((body - 200) * 65)
        else:
            base_index = 35000 + ((body - 400) * 175)

        index = base_index + (action * 5)

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
        """Decode animation data into frames."""
        if data is None or len(data) < 512:
            return []

        # Read palette (256 colors, 2 bytes each)
        palette = []
        for i in range(256):
            color = struct.unpack('<H', data[i*2:(i+1)*2])[0]
            color ^= 0x8000
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
            lookups.append(512 + lookup)
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
    """Force all pixels to be either fully transparent or fully opaque."""
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
    img = threshold_alpha(img.copy(), threshold=128)

    width, height = img.size
    new_width = max(1, int(width * scale))
    new_height = max(1, int(height * scale))

    resized = img.resize((new_width, new_height), Image.Resampling.NEAREST)
    resized = threshold_alpha(resized, threshold=128)

    new_center_x = int(center_x * scale)
    new_center_y = int(center_y * scale)

    return resized, new_center_x, new_center_y


def encode_animation(frames):
    """Encode frames into UO animation format."""
    if not frames:
        return None

    all_colors = set()

    for img, cx, cy in frames:
        pixels = img.load()
        for y in range(img.height):
            for x in range(img.width):
                c = pixels[x, y]
                if c[3] > 0:
                    quantized = ((c[0] // 8) * 8, (c[1] // 8) * 8, (c[2] // 8) * 8, 255)
                    all_colors.add(quantized)

    color_list = [(0, 0, 0, 255)]
    color_to_idx = {(0, 0, 0, 255): 0}

    for c in sorted(all_colors):
        if c[3] > 0 and c not in color_to_idx and len(color_list) < 256:
            color_to_idx[c] = len(color_list)
            color_list.append(c)

    while len(color_list) < 256:
        color_list.append((0, 0, 0, 255))

    palette_data = bytearray()
    for r, g, b, a in color_list:
        color = 0x8000
        color |= ((r // 8) & 0x1F) << 10
        color |= ((g // 8) & 0x1F) << 5
        color |= (b // 8) & 0x1F
        color ^= 0x8000
        palette_data.extend(struct.pack('<H', color))

    frame_count = len(frames)
    header_data = struct.pack('<I', frame_count)

    lookup_size = frame_count * 4

    frame_data_list = []
    for img, center_x, center_y in frames:
        frame_bytes = encode_single_frame(img, center_x, center_y, color_to_idx)
        frame_data_list.append(frame_bytes)

    lookups = bytearray()
    current_offset = 4 + lookup_size
    for frame_bytes in frame_data_list:
        lookups.extend(struct.pack('<I', current_offset))
        current_offset += len(frame_bytes)

    result = palette_data + header_data + lookups
    for frame_bytes in frame_data_list:
        result.extend(frame_bytes)

    return bytes(result)


def encode_single_frame(img, center_x, center_y, color_to_idx):
    """Encode a single frame"""
    width, height = img.size
    pixels = img.load()

    frame_data = bytearray()
    frame_data.extend(struct.pack('<h', center_x))
    frame_data.extend(struct.pack('<h', center_y))
    frame_data.extend(struct.pack('<H', width))
    frame_data.extend(struct.pack('<H', height))

    DOUBLE_XOR = (0x200 << 22) | (0x200 << 12)
    x_base = center_x - 0x200
    y_base = (center_y + height) - 0x200

    for y in range(height):
        x = 0
        while x < width:
            while x < width and pixels[x, y][3] == 0:
                x += 1

            if x >= width:
                break

            run_start = x
            run_data = bytearray()
            while x < width and pixels[x, y][3] > 0 and len(run_data) < 0xFFF:
                c = pixels[x, y]
                quantized = ((c[0] // 8) * 8, (c[1] // 8) * 8, (c[2] // 8) * 8, 255)
                idx = color_to_idx.get(quantized, 0)
                run_data.append(idx)
                x += 1

            if len(run_data) > 0:
                x_offset = run_start - x_base
                y_offset = y - y_base

                x_offset = x_offset & 0x3FF
                y_offset = y_offset & 0x3FF

                header = len(run_data) & 0xFFF
                header |= (y_offset & 0x3FF) << 12
                header |= (x_offset & 0x3FF) << 22
                header ^= DOUBLE_XOR

                frame_data.extend(struct.pack('<I', header))
                frame_data.extend(run_data)

    frame_data.extend(struct.pack('<I', 0x7FFF7FFF))

    return bytes(frame_data)


def get_index_offset(body, action, direction):
    """Calculate the index offset for a body/action/direction combo"""
    if body < 200:
        base_index = body * 110
    elif body < 400:
        base_index = 22000 + ((body - 200) * 65)
    else:
        base_index = 35000 + ((body - 400) * 175)

    index = base_index + (action * 5)

    if direction <= 4:
        index += direction
    else:
        index += direction - (direction - 4) * 2

    return index


def backup_files(client_path, backup_path):
    """Create timestamped backups of anim.mul and anim.idx"""
    timestamp = datetime.now().strftime("%Y%m%d_%H%M%S")
    backup_dir = Path(backup_path) / timestamp
    backup_dir.mkdir(parents=True, exist_ok=True)

    files_to_backup = ["anim.mul", "anim.idx"]

    for filename in files_to_backup:
        src = Path(client_path) / filename
        if src.exists():
            dst = backup_dir / filename
            print(f"Backing up {src} -> {dst}")
            shutil.copy2(src, dst)

    print(f"Backups created in: {backup_dir}")
    return backup_dir


def write_to_mul_files(client_path, body_id, all_frames):
    """Write animation data to anim.mul"""
    idx_path = Path(client_path) / "anim.idx"
    mul_path = Path(client_path) / "anim.mul"

    print(f"\n--- Writing body {body_id} ---")

    idx_file = open(idx_path, 'r+b')
    mul_file = open(mul_path, 'r+b')

    try:
        written_count = 0

        for (action, direction), frames in sorted(all_frames.items()):
            anim_data = encode_animation(frames)
            if anim_data is None:
                continue

            new_length = len(anim_data)

            # Append to end of file
            mul_file.seek(0, 2)
            offset = mul_file.tell()
            mul_file.write(anim_data)

            # Update idx entry
            index = get_index_offset(body_id, action, direction)
            idx_offset = index * 12

            # Ensure idx file is large enough
            idx_file.seek(0, 2)
            idx_size = idx_file.tell()
            if idx_offset + 12 > idx_size:
                idx_file.seek(idx_size)
                while idx_file.tell() < idx_offset + 12:
                    idx_file.write(struct.pack('<III', 0xFFFFFFFF, 0, 0))

            idx_file.seek(idx_offset)
            idx_file.write(struct.pack('<III', offset, new_length, 0))

            written_count += 1

        print(f"Wrote {written_count} animations for body {body_id}")

    finally:
        idx_file.close()
        mul_file.close()


def main():
    print("=" * 60)
    print("Dwarf Hair Sprite Creator for Ultima Online")
    print("=" * 60)

    # Check paths
    if not os.path.exists(UO_CLIENT_PATH):
        print(f"ERROR: UO client not found at {UO_CLIENT_PATH}")
        return

    output_path = Path(OUTPUT_PATH)
    if not output_path.exists():
        print(f"ERROR: {OUTPUT_PATH} not found")
        print("Run dwarf_sprite_writer.py first to create the patched client!")
        return

    if not (output_path / "anim.mul").exists():
        print(f"ERROR: anim.mul not found in {OUTPUT_PATH}")
        return

    # Create backup
    print("\n--- Creating Backups ---")
    backup_dir = backup_files(str(output_path), BACKUP_PATH)

    # Create reader for source files (read from ORIGINAL client, not patched)
    try:
        reader = AnimationReader(UO_CLIENT_PATH)
    except FileNotFoundError as e:
        print(f"ERROR: {e}")
        return

    # Extract pigtails animation
    print(f"\n--- Extracting Pigtails (AnimID {PIGTAILS_ANIM_ID}) ---")

    # Hair uses same structure as equipment (35 actions, 5 directions)
    num_actions = 35
    num_directions = 5

    all_frames = {}
    total_frames = 0

    for action in range(num_actions):
        for direction in range(num_directions):
            data = reader.read_animation_data(PIGTAILS_ANIM_ID, action, direction)
            if data:
                frames = reader.decode_animation(data)
                if frames:
                    # Resize each frame
                    resized_frames = []
                    for img, cx, cy in frames:
                        new_img, new_cx, new_cy = resize_frame(img, cx, cy, SCALE_FACTOR)
                        resized_frames.append((new_img, new_cx, new_cy))

                    all_frames[(action, direction)] = resized_frames
                    total_frames += len(frames)

    if total_frames > 0:
        print(f"  Found {len(all_frames)} animation sets, {total_frames} total frames")
    else:
        print(f"  No animation data found for AnimID {PIGTAILS_ANIM_ID}")
        return

    # Save frames as images for review
    print(f"\n--- Saving frames to {SPRITES_PATH} ---")
    sprites_path = Path(SPRITES_PATH) / f"pigtails_anim_{DWARF_PIGTAILS_ANIM_ID}"
    sprites_path.mkdir(parents=True, exist_ok=True)

    for (action, direction), frames in sorted(all_frames.items()):
        dir_path = sprites_path / f"action_{action:02d}_dir_{direction}"
        dir_path.mkdir(parents=True, exist_ok=True)

        for i, (img, cx, cy) in enumerate(frames):
            img.save(dir_path / f"frame_{i:02d}_c{cx}_{cy}.png")

    print(f"  Saved to: {sprites_path}")

    # Write to target animation ID
    print(f"\n--- Writing to AnimID {DWARF_PIGTAILS_ANIM_ID} ---")
    write_to_mul_files(str(output_path), DWARF_PIGTAILS_ANIM_ID, all_frames)

    print("\n" + "=" * 60)
    print("DONE!")
    print(f"Dwarf pigtails written to AnimID {DWARF_PIGTAILS_ANIM_ID}")
    print(f"\nNext steps:")
    print(f"1. Add equipconv.def entries to map hair for dwarf bodies:")
    print(f"   987\t{PIGTAILS_ANIM_ID}\t{DWARF_PIGTAILS_ANIM_ID}\t0\t0\t# Pigtails -> Dwarf Pigtails (male)")
    print(f"   988\t{PIGTAILS_ANIM_ID}\t{DWARF_PIGTAILS_ANIM_ID}\t0\t0\t# Pigtails -> Dwarf Pigtails (female)")
    print(f"2. Update Dwarf.cs to use HairItemID = 0x2049 (pigtails)")
    print(f"3. Copy patched anim.mul/anim.idx to client")
    print("=" * 60)


if __name__ == "__main__":
    main()
