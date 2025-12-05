"""
Simple test: Resize fancy dress (body 448) to 75% and write to body 919
"""
import struct
import os
import shutil
from pathlib import Path
from PIL import Image

# Paths
BACKUP_PATH = r"C:\DevEnv\GIT\UO\Vystia\tools\backups\20251204_103008"
OUTPUT_PATH = r"C:\DevEnv\GIT\UO\Vystia\tools\patched_client"
SCALE_FACTOR = 0.75

SOURCE_BODY = 448  # Fancy dress
TARGET_BODY = 919  # Target for dwarf fancy dress


def get_index_offset(body, action=0, direction=0):
    if body < 200:
        base_index = body * 110
    elif body < 400:
        base_index = 22000 + ((body - 200) * 65)
    else:
        base_index = 35000 + ((body - 400) * 175)
    index = base_index + (action * 5)
    if direction <= 4:
        index += direction
    return index


def decode_animation(data):
    """Decode animation data into frames"""
    if data is None or len(data) < 512:
        return [], None

    # Read palette
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
        frame = decode_frame(data, lookup, palette)
        if frame:
            frames.append(frame)

    return frames, palette


def decode_frame(data, offset, palette):
    """Decode a single frame"""
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
                px, py = x + i, y
                if 0 <= px < width and 0 <= py < height:
                    pixels[px, py] = palette[color_idx]

        return (img, center_x, center_y)
    except Exception as e:
        print(f"Error decoding: {e}")
        return None


def resize_frame(img, center_x, center_y, scale):
    """Resize frame with alpha threshold"""
    pixels = img.load()
    for y in range(img.height):
        for x in range(img.width):
            r, g, b, a = pixels[x, y]
            if a < 128:
                pixels[x, y] = (0, 0, 0, 0)
            else:
                pixels[x, y] = (r, g, b, 255)

    new_width = max(1, int(img.width * scale))
    new_height = max(1, int(img.height * scale))
    resized = img.resize((new_width, new_height), Image.Resampling.NEAREST)

    new_center_x = int(center_x * scale)
    new_center_y = int(center_y * scale)

    return resized, new_center_x, new_center_y


def encode_animation(frames):
    """Encode frames into UO animation format"""
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


def main():
    print("=" * 60)
    print(f"Resizing fancy dress (body {SOURCE_BODY}) to 75%")
    print(f"Writing to body {TARGET_BODY}")
    print("=" * 60)

    backup_idx = Path(BACKUP_PATH) / "anim.idx"
    backup_mul = Path(BACKUP_PATH) / "anim.mul"
    output_idx = Path(OUTPUT_PATH) / "anim.idx"
    output_mul = Path(OUTPUT_PATH) / "anim.mul"

    if not output_idx.exists():
        print("Copying backup to output...")
        shutil.copy2(backup_idx, output_idx)
        shutil.copy2(backup_mul, output_mul)

    # Read source animation
    with open(backup_idx, 'rb') as f:
        idx = get_index_offset(SOURCE_BODY, 0, 0)
        f.seek(idx * 12)
        data = f.read(12)
        offset, length, extra = struct.unpack('<III', data)
        print(f"Source body {SOURCE_BODY}: offset={offset:,}, length={length:,}")

    # Process all actions/directions
    with open(backup_idx, 'rb') as idx_f, \
         open(backup_mul, 'rb') as mul_f, \
         open(output_idx, 'r+b') as out_idx_f, \
         open(output_mul, 'r+b') as out_mul_f:

        total_written = 0

        for action in range(35):  # Human/equipment has 35 actions
            for direction in range(5):
                # Read source
                idx = get_index_offset(SOURCE_BODY, action, direction)
                idx_f.seek(idx * 12)
                data = idx_f.read(12)
                offset, length, extra = struct.unpack('<III', data)

                if offset == 0xFFFFFFFF or length == 0:
                    continue

                mul_f.seek(offset)
                anim_data = mul_f.read(length)

                # Decode
                frames, palette = decode_animation(anim_data)
                if not frames:
                    continue

                # Resize
                resized_frames = []
                for img, cx, cy in frames:
                    new_img, new_cx, new_cy = resize_frame(img, cx, cy, SCALE_FACTOR)
                    resized_frames.append((new_img, new_cx, new_cy))

                # Encode
                new_data = encode_animation(resized_frames)
                if not new_data:
                    continue

                # Append to output mul
                out_mul_f.seek(0, 2)
                new_offset = out_mul_f.tell()
                out_mul_f.write(new_data)

                # Update output idx
                target_idx = get_index_offset(TARGET_BODY, action, direction)
                out_idx_f.seek(target_idx * 12)
                out_idx_f.write(struct.pack('<III', new_offset, len(new_data), 0))

                total_written += 1

        print(f"Wrote {total_written} animations to body {TARGET_BODY}")

    print("\nDone! Now add this to equipconv.def:")
    print(f"987\t448\t{TARGET_BODY}\t0\t0\t# Dwarf male fancy dress")
    print(f"988\t448\t{TARGET_BODY}\t0\t0\t# Dwarf female fancy dress")
    print("\nAnd add to mobtypes.txt:")
    print(f"{TARGET_BODY}\tEQUIPMENT\t0")


if __name__ == "__main__":
    main()
