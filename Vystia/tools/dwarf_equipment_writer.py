"""
Dwarf Equipment Sprite Writer for Ultima Online
Takes the resized equipment PNG frames and writes them to anim.mul/idx files.

Usage:
    python dwarf_equipment_writer.py

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
import re

# Configuration
UO_CLIENT_PATH = r"C:\Program Files (x86)\Electronic Arts\Ultima Online Classic"
SPRITES_PATH = r"C:\DevEnv\GIT\UO\Vystia\tools\dwarf_equipment_sprites"
BACKUP_PATH = r"C:\DevEnv\GIT\UO\Vystia\tools\backups"
OUTPUT_PATH = r"C:\DevEnv\GIT\UO\Vystia\tools\patched_client"

# Source -> Target body ID mapping
# Source = folder name from dwarf_equipment_creator.py
# Target = body ID to write to (must exist in anim.mul with EQUIPMENT type in mobtypes.txt)
# Using bodies 909-913 for male plate, 914-918 for female leather
EQUIPMENT_MAPPING = {
    # Male plate armor
    'plate_chest_body_527': 909,    # Dwarf plate chest
    'plate_arms_body_528': 910,     # Dwarf plate arms
    'plate_legs_body_529': 911,     # Dwarf plate legs
    'plate_gloves_body_530': 912,   # Dwarf plate gloves
    'warhammer_body_646': 913,      # Dwarf warhammer
    # Female leather armor
    'leather_tunic_body_542': 914,     # Dwarf leather tunic
    'leather_leggings_body_543': 915,  # Dwarf leather leggings
    'leather_sleeves_body_544': 916,   # Dwarf leather sleeves
    'leather_gloves_body_545': 917,    # Dwarf leather gloves
    'leather_gorget_body_546': 918,    # Dwarf leather gorget
}


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


def load_frames_from_images(sprites_path, folder_name):
    """Load all frames for equipment from PNG images"""
    body_path = Path(sprites_path) / folder_name

    if not body_path.exists():
        print(f"ERROR: {body_path} not found")
        return {}

    all_frames = {}

    # Find all action_XX_dir_Y folders
    for folder in sorted(body_path.iterdir()):
        if not folder.is_dir():
            continue

        # Parse folder name: action_XX_dir_Y
        match = re.match(r'action_(\d+)_dir_(\d+)', folder.name)
        if not match:
            continue

        action = int(match.group(1))
        direction = int(match.group(2))

        frames = []
        # Find all frame_XX_cY_Z.png files
        for img_file in sorted(folder.glob("frame_*.png")):
            # Parse filename: frame_XX_cY_Z.png (center_x, center_y)
            match = re.match(r'frame_(\d+)_c(-?\d+)_(-?\d+)\.png', img_file.name)
            if match:
                frame_num = int(match.group(1))
                center_x = int(match.group(2))
                center_y = int(match.group(3))

                img = Image.open(img_file).convert('RGBA')
                frames.append((img, center_x, center_y))

        if frames:
            all_frames[(action, direction)] = frames

    return all_frames


def encode_animation(frames):
    """
    Encode frames into UO animation format.
    Returns the raw bytes for the animation data.
    """
    if not frames:
        return None

    # Build a combined palette from all frames
    all_colors = set()

    for img, cx, cy in frames:
        pixels = img.load()
        for y in range(img.height):
            for x in range(img.width):
                c = pixels[x, y]
                if c[3] > 0:  # Non-transparent
                    # Quantize to 5-bit per channel
                    quantized = ((c[0] // 8) * 8, (c[1] // 8) * 8, (c[2] // 8) * 8, 255)
                    all_colors.add(quantized)

    # Build palette (max 256 colors)
    color_list = [(0, 0, 0, 255)]  # Index 0 = black (opaque)
    color_to_idx = {(0, 0, 0, 255): 0}

    for c in sorted(all_colors):
        if c[3] > 0 and c not in color_to_idx and len(color_list) < 256:
            color_to_idx[c] = len(color_list)
            color_list.append(c)

    # Pad to 256 with black
    while len(color_list) < 256:
        color_list.append((0, 0, 0, 255))

    # Encode palette (512 bytes)
    palette_data = bytearray()
    for r, g, b, a in color_list:
        color = 0x8000  # Alpha bit
        color |= ((r // 8) & 0x1F) << 10
        color |= ((g // 8) & 0x1F) << 5
        color |= (b // 8) & 0x1F
        color ^= 0x8000
        palette_data.extend(struct.pack('<H', color))

    # Encode frame count
    frame_count = len(frames)
    header_data = struct.pack('<I', frame_count)

    # Placeholder for frame lookups
    lookup_size = frame_count * 4

    # Encode each frame
    frame_data_list = []
    for img, center_x, center_y in frames:
        frame_bytes = encode_single_frame(img, center_x, center_y, color_to_idx)
        frame_data_list.append(frame_bytes)

    # Calculate lookups
    lookups = bytearray()
    current_offset = 4 + lookup_size
    for frame_bytes in frame_data_list:
        lookups.extend(struct.pack('<I', current_offset))
        current_offset += len(frame_bytes)

    # Combine everything
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
            # Skip transparent pixels
            while x < width and pixels[x, y][3] == 0:
                x += 1

            if x >= width:
                break

            # Find run of non-transparent pixels
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

    # End marker
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


def get_original_offset_and_length(idx_path, body_id, action, direction):
    """Read the original offset and length from the idx file"""
    index = get_index_offset(body_id, action, direction)
    idx_offset = index * 12

    with open(idx_path, 'rb') as f:
        f.seek(0, 2)
        idx_size = f.tell()

        if idx_offset + 12 > idx_size:
            return None, None

        f.seek(idx_offset)
        data = f.read(12)
        if len(data) < 12:
            return None, None

        offset, length, extra = struct.unpack('<III', data)

        if offset == 0xFFFFFFFF:
            return None, None

        return offset, length


def write_to_mul_files(client_path, body_id, all_frames):
    """Write animation data to anim.mul IN-PLACE at original offsets"""
    idx_path = Path(client_path) / "anim.idx"
    mul_path = Path(client_path) / "anim.mul"

    print(f"\n--- Writing body {body_id} IN-PLACE ---")

    # First pass: check all original offsets and space available
    print("Checking original animation slots...")
    space_info = {}
    for (action, direction), frames in sorted(all_frames.items()):
        orig_offset, orig_length = get_original_offset_and_length(idx_path, body_id, action, direction)
        if orig_offset is not None:
            space_info[(action, direction)] = (orig_offset, orig_length)

    print(f"Found {len(space_info)} existing animation slots for body {body_id}")

    # Open files for modification
    idx_file = open(idx_path, 'r+b')
    mul_file = open(mul_path, 'r+b')

    try:
        written_count = 0
        skipped_count = 0

        for (action, direction), frames in sorted(all_frames.items()):
            # Encode the animation
            anim_data = encode_animation(frames)
            if anim_data is None:
                continue

            new_length = len(anim_data)

            # Get original offset
            if (action, direction) in space_info:
                orig_offset, orig_length = space_info[(action, direction)]

                # Check if our data fits in original slot
                if new_length <= orig_length:
                    # Write IN-PLACE at original offset
                    mul_file.seek(orig_offset)
                    mul_file.write(anim_data)

                    # Pad remaining space with zeros
                    padding = orig_length - new_length
                    if padding > 0:
                        mul_file.write(b'\x00' * padding)

                    # Update idx entry with new length
                    index = get_index_offset(body_id, action, direction)
                    idx_offset = index * 12
                    idx_file.seek(idx_offset)
                    idx_file.write(struct.pack('<III', orig_offset, new_length, 0))

                    written_count += 1
                else:
                    # Data doesn't fit - APPEND to end of file
                    mul_file.seek(0, 2)  # Go to end
                    offset = mul_file.tell()
                    mul_file.write(anim_data)

                    # Update idx entry with new offset
                    index = get_index_offset(body_id, action, direction)
                    idx_offset = index * 12
                    idx_file.seek(idx_offset)
                    idx_file.write(struct.pack('<III', offset, new_length, 0))

                    written_count += 1
            else:
                # No original slot - append to end
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
                idx_file.write(struct.pack('<III', offset, new_length, 0))

                written_count += 1

        print(f"Wrote {written_count} animations for body {body_id} (skipped {skipped_count})")

    finally:
        idx_file.close()
        mul_file.close()


def main():
    print("=" * 60)
    print("Dwarf Equipment Sprite Writer for Ultima Online")
    print("=" * 60)

    # Check paths
    if not os.path.exists(UO_CLIENT_PATH):
        print(f"ERROR: UO client not found at {UO_CLIENT_PATH}")
        return

    if not os.path.exists(SPRITES_PATH):
        print(f"ERROR: Sprites not found at {SPRITES_PATH}")
        print("Run dwarf_equipment_creator.py first!")
        return

    # Use existing patched_client folder (already has dwarf body patches)
    print("\n--- Using existing patched_client directory ---")
    output_path = Path(OUTPUT_PATH)

    if not output_path.exists():
        print(f"ERROR: {OUTPUT_PATH} not found")
        print("Run dwarf_sprite_writer.py first to create the patched client!")
        return

    # Check that anim files exist
    if not (output_path / "anim.mul").exists():
        print(f"ERROR: anim.mul not found in {OUTPUT_PATH}")
        return

    # Create backup
    print("\n--- Creating Backups ---")
    backup_dir = backup_files(str(output_path), BACKUP_PATH)

    # Process each equipment piece
    print(f"\n--- Writing Equipment to {OUTPUT_PATH} ---")

    for source_folder, target_body in EQUIPMENT_MAPPING.items():
        print(f"\n--- Loading {source_folder} -> body {target_body} ---")
        frames = load_frames_from_images(SPRITES_PATH, source_folder)

        if frames:
            print(f"Loaded {len(frames)} animation sets")
            write_to_mul_files(str(output_path), target_body, frames)
        else:
            print(f"No frames found for {source_folder}")

    print("\n" + "=" * 60)
    print("DONE!")
    print(f"Patched files saved to: {OUTPUT_PATH}")
    print(f"Backups saved to: {backup_dir}")
    print("\nEquipment body IDs for dwarf items:")
    print("  909 = Dwarf Plate Chest")
    print("  910 = Dwarf Plate Arms")
    print("  911 = Dwarf Plate Legs")
    print("  912 = Dwarf Plate Gloves")
    print("  913 = Dwarf Warhammer")
    print("\nNext steps:")
    print("1. Copy patched files to your client")
    print("2. Create custom dwarf equipment items that use these animation body IDs")
    print("3. Add entries to bodyTable.cfg if needed")
    print("=" * 60)


if __name__ == "__main__":
    main()
