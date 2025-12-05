"""
Dwarf Beard Sprite Creator for Ultima Online
Extracts long beard animations, resizes to 75%, and writes to new animation IDs.

Usage:
    python dwarf_beard_creator.py
"""

import os
import struct
import shutil
from pathlib import Path
from datetime import datetime
from PIL import Image

# Configuration
UO_CLIENT_PATH = r"C:\Program Files (x86)\Electronic Arts\Ultima Online Classic"
OUTPUT_PATH = r"C:\DevEnv\GIT\UO\Vystia\tools\patched_client"
BACKUP_PATH = r"C:\DevEnv\GIT\UO\Vystia\tools\backups"
SPRITES_PATH = r"C:\DevEnv\GIT\UO\Vystia\tools\dwarf_beard_sprites"

SCALE_FACTOR = 0.75

# Source beard AnimIDs and target IDs
# Using 921, 922, 923 for scaled versions
BEARD_MAPPING = {
    801: 921,   # Long Beard (0x203E) -> Dwarf Long Beard
    904: 922,   # Medium Long Beard (0x204B) -> Dwarf Medium Long Beard
    906: 923,   # Long Beard 2 (0x204D) -> Dwarf Long Beard 2
}

BEARD_NAMES = {
    801: "long_beard",
    904: "medium_long_beard",
    906: "long_beard_2",
}


class AnimationReader:
    def __init__(self, client_path):
        self.client_path = Path(client_path)
        self.idx_path = self.client_path / "anim.idx"
        self.mul_path = self.client_path / "anim.mul"

    def get_index_offset(self, body, action, direction):
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
        index = self.get_index_offset(body, action, direction)
        offset, length, extra = self.read_index_entry(index)
        if offset is None:
            return None
        with open(self.mul_path, 'rb') as f:
            f.seek(offset)
            return f.read(length)

    def decode_animation(self, data):
        if data is None or len(data) < 512:
            return []
        palette = []
        for i in range(256):
            color = struct.unpack('<H', data[i*2:(i+1)*2])[0]
            color ^= 0x8000
            a = 255 if (color & 0x8000) else 0
            r = ((color >> 10) & 0x1F) * 8
            g = ((color >> 5) & 0x1F) * 8
            b = (color & 0x1F) * 8
            palette.append((r, g, b, a))

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
        except:
            return None


def threshold_alpha(img, threshold=128):
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
                x_offset = (run_start - x_base) & 0x3FF
                y_offset = (y - y_base) & 0x3FF
                header = len(run_data) & 0xFFF
                header |= (y_offset & 0x3FF) << 12
                header |= (x_offset & 0x3FF) << 22
                header ^= DOUBLE_XOR
                frame_data.extend(struct.pack('<I', header))
                frame_data.extend(run_data)

    frame_data.extend(struct.pack('<I', 0x7FFF7FFF))
    return bytes(frame_data)


def get_index_offset(body, action, direction):
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


def write_to_mul_files(client_path, body_id, all_frames):
    idx_path = Path(client_path) / "anim.idx"
    mul_path = Path(client_path) / "anim.mul"

    idx_file = open(idx_path, 'r+b')
    mul_file = open(mul_path, 'r+b')

    try:
        written_count = 0
        for (action, direction), frames in sorted(all_frames.items()):
            anim_data = encode_animation(frames)
            if anim_data is None:
                continue
            mul_file.seek(0, 2)
            offset = mul_file.tell()
            mul_file.write(anim_data)

            index = get_index_offset(body_id, action, direction)
            idx_offset = index * 12

            idx_file.seek(0, 2)
            idx_size = idx_file.tell()
            if idx_offset + 12 > idx_size:
                idx_file.seek(idx_size)
                while idx_file.tell() < idx_offset + 12:
                    idx_file.write(struct.pack('<III', 0xFFFFFFFF, 0, 0))

            idx_file.seek(idx_offset)
            idx_file.write(struct.pack('<III', offset, len(anim_data), 0))
            written_count += 1

        print(f"  Wrote {written_count} animations for body {body_id}")
    finally:
        idx_file.close()
        mul_file.close()


def main():
    print("=" * 60)
    print("Dwarf Beard Sprite Creator for Ultima Online")
    print("=" * 60)

    if not os.path.exists(UO_CLIENT_PATH):
        print(f"ERROR: UO client not found at {UO_CLIENT_PATH}")
        return

    output_path = Path(OUTPUT_PATH)
    if not output_path.exists() or not (output_path / "anim.mul").exists():
        print(f"ERROR: Patched client not found at {OUTPUT_PATH}")
        return

    # Backup
    timestamp = datetime.now().strftime("%Y%m%d_%H%M%S")
    backup_dir = Path(BACKUP_PATH) / timestamp
    backup_dir.mkdir(parents=True, exist_ok=True)
    for f in ["anim.mul", "anim.idx"]:
        src = output_path / f
        if src.exists():
            shutil.copy2(src, backup_dir / f)
    print(f"Backups created in: {backup_dir}")

    # Reader from ORIGINAL client
    reader = AnimationReader(UO_CLIENT_PATH)

    for source_id, target_id in BEARD_MAPPING.items():
        name = BEARD_NAMES[source_id]
        print(f"\n--- Processing {name} (AnimID {source_id} -> {target_id}) ---")

        all_frames = {}
        total_frames = 0

        for action in range(35):
            for direction in range(5):
                data = reader.read_animation_data(source_id, action, direction)
                if data:
                    frames = reader.decode_animation(data)
                    if frames:
                        resized_frames = []
                        for img, cx, cy in frames:
                            new_img, new_cx, new_cy = resize_frame(img, cx, cy, SCALE_FACTOR)
                            resized_frames.append((new_img, new_cx, new_cy))
                        all_frames[(action, direction)] = resized_frames
                        total_frames += len(frames)

        if total_frames > 0:
            print(f"  Found {len(all_frames)} animation sets, {total_frames} total frames")

            # Save sprites for review
            sprites_path = Path(SPRITES_PATH) / f"{name}_anim_{target_id}"
            sprites_path.mkdir(parents=True, exist_ok=True)
            for (action, direction), frames in sorted(all_frames.items()):
                dir_path = sprites_path / f"action_{action:02d}_dir_{direction}"
                dir_path.mkdir(parents=True, exist_ok=True)
                for i, (img, cx, cy) in enumerate(frames):
                    img.save(dir_path / f"frame_{i:02d}_c{cx}_{cy}.png")
            print(f"  Sprites saved to: {sprites_path}")

            # Write to anim.mul
            write_to_mul_files(str(output_path), target_id, all_frames)
        else:
            print(f"  No animation data found")

    print("\n" + "=" * 60)
    print("DONE!")
    print("\nBeard mappings created:")
    for source_id, target_id in BEARD_MAPPING.items():
        print(f"  {source_id} -> {target_id} ({BEARD_NAMES[source_id]})")
    print("\nAdd to equipconv.def:")
    print("987\t801\t921\t0\t0\t# Long Beard -> Dwarf Long Beard")
    print("987\t904\t922\t0\t0\t# Medium Long Beard -> Dwarf Medium Long Beard")
    print("987\t906\t923\t0\t0\t# Long Beard 2 -> Dwarf Long Beard 2")
    print("=" * 60)


if __name__ == "__main__":
    main()
