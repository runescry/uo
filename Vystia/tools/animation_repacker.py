"""
Animation Repacker - Convert extracted PNG frames back to UO binary animation format
Stores in a custom format that can be loaded by ServUO without overwriting core animations
"""

import os
import struct
from pathlib import Path
from PIL import Image
import json

EXTRACTED_DIR = Path(r"C:\DevEnv\GIT\UO\Vystia\tools\creature_animations")
OUTPUT_DIR = Path(r"C:\DevEnv\GIT\UO\ServUO\Bin\Debug\net6.0\Data\VystiaAnimations")

def parse_frame_filename(filename):
    """
    Parse filename format: frame_XX_c-Y_Z.png
    Returns: (frame_number, center_x, center_y)
    """
    name = filename.replace('.png', '')
    parts = name.split('_')
    frame_num = int(parts[1])
    center_x = int(parts[2][1:])  # Skip 'c'
    center_y = int(parts[3])
    return frame_num, center_x, center_y


def frame_to_uop_format(frame_data, width, height, center_x, center_y):
    """
    Convert frame PIL Image to UOP frame format
    Returns raw bytes in UOP animation frame structure
    """
    # Get pixel data
    pixels = list(frame_data.convert('RGBA').getdata())

    # UOP frame header
    frame_header = struct.pack('<HHHHH',
        0,  # lookup table address (0 = no lookup)
        width,
        height,
        center_x,
        center_y
    )

    # Encode pixels as 16-bit ARGB1555
    frame_data_bytes = bytearray()
    for pixel in pixels:
        r, g, b, a = pixel
        # ARGB1555: bit 15 = alpha, bits 14-10 = red, bits 9-5 = green, bits 4-0 = blue
        argb = 0
        if a > 127:  # Alpha threshold
            argb |= 0x8000
        argb |= ((r >> 3) & 0x1F) << 10
        argb |= ((g >> 3) & 0x1F) << 5
        argb |= (b >> 3) & 0x1F
        frame_data_bytes.extend(struct.pack('<H', argb))

    return frame_header + frame_data_bytes


def repack_body(body_id, body_dir):
    """
    Repack a single body's animations into binary format
    Returns: (action_count, frame_count_per_action)
    """
    body_data = {}
    action_dirs = sorted([d for d in body_dir.iterdir() if d.is_dir()])

    for action_dir in action_dirs:
        # Parse action_XX_dir_Y format
        dirname = action_dir.name
        action_num = int(dirname.split('_')[1])
        direction = int(dirname.split('_')[3])

        # Get all frames for this action/direction
        frames = sorted([f for f in action_dir.glob('*.png')])

        if not frames:
            continue

        # Create key: (action, direction)
        key = (action_num, direction)
        body_data[key] = []

        # Encode each frame
        for frame_file in frames:
            try:
                frame_img = Image.open(frame_file)
                frame_num, center_x, center_y = parse_frame_filename(frame_file.name)

                # Convert to binary format
                frame_binary = frame_to_uop_format(frame_img, frame_img.width, frame_img.height, center_x, center_y)
                body_data[key].append({
                    'number': frame_num,
                    'data': frame_binary
                })
            except Exception as e:
                print(f"  Error processing {frame_file.name}: {e}")

    return body_data


def create_compact_animation_bundle(body_id, body_data):
    """
    Create a compact binary bundle of all animations for a body
    Format:
    [Header]
    - Magic: 'VUAO' (4 bytes)
    - Version: 1 (2 bytes)
    - Body ID: (2 bytes)
    - Action count: (2 bytes)
    [Index Table] - (action_count * 4) bytes
    - Each entry: offset to first frame for this action (4 bytes)
    [Frame Data]
    - Sequential frame data
    """
    bundle = bytearray()

    # Header
    bundle.extend(b'VUAO')  # Magic
    bundle.extend(struct.pack('<H', 1))  # Version
    bundle.extend(struct.pack('<H', body_id))  # Body ID
    bundle.extend(struct.pack('<H', len(set(k[0] for k in body_data.keys()))))  # Action count

    # Build index and frame data
    action_index = {}
    frame_data = bytearray()

    for action_num in sorted(set(k[0] for k in body_data.keys())):
        action_index[action_num] = len(bundle) + len(frame_data)

        # Store all directions for this action
        for direction in range(8):
            key = (action_num, direction)
            if key in body_data:
                frames = sorted(body_data[key], key=lambda f: f['number'])
                for frame in frames:
                    frame_data.extend(frame['data'])

    # Write index table
    for action_num in sorted(action_index.keys()):
        bundle.extend(struct.pack('<I', action_index[action_num]))

    # Write frame data
    bundle.extend(frame_data)

    return bytes(bundle)


def main():
    print("=" * 70)
    print("ANIMATION REPACKER - PNG to UO Binary Format")
    print("=" * 70)

    # Create output directory
    OUTPUT_DIR.mkdir(parents=True, exist_ok=True)

    # Find all extracted bodies
    body_dirs = sorted([d for d in EXTRACTED_DIR.iterdir() if d.is_dir() and d.name.startswith('body_')])

    print(f"\nFound {len(body_dirs)} extracted bodies")
    print(f"Output: {OUTPUT_DIR}\n")

    success_count = 0
    failed_count = 0
    manifest = {}

    for i, body_dir in enumerate(body_dirs, 1):
        body_id = int(body_dir.name.split('_')[1])

        print(f"[{i:3d}/{len(body_dirs)}] Repacking body {body_id}: ", end="", flush=True)

        try:
            # Repack animations
            body_data = repack_body(body_id, body_dir)

            if not body_data:
                print("FAILED (no animations)")
                failed_count += 1
                continue

            # Create compact bundle
            bundle = create_compact_animation_bundle(body_id, body_data)

            # Write bundle
            output_file = OUTPUT_DIR / f"body_{body_id}.vuo"
            with open(output_file, 'wb') as f:
                f.write(bundle)

            # Track in manifest
            action_count = len(set(k[0] for k in body_data.keys()))
            total_frames = sum(len(frames) for frames in body_data.values())
            manifest[str(body_id)] = {
                'file': output_file.name,
                'actions': action_count,
                'frames': total_frames,
                'size': len(bundle)
            }

            print(f"SUCCESS ({action_count} actions, {total_frames} frames, {len(bundle)} bytes)")
            success_count += 1

        except Exception as e:
            print(f"ERROR - {e}")
            failed_count += 1

    # Write manifest
    manifest_file = OUTPUT_DIR / "manifest.json"
    with open(manifest_file, 'w') as f:
        json.dump(manifest, f, indent=2)

    # Summary
    print("\n" + "=" * 70)
    print("REPACKING COMPLETE")
    print("=" * 70)
    print(f"Success: {success_count}")
    print(f"Failed: {failed_count}")
    print(f"Total: {len(body_dirs)}")
    print(f"\nOutput directory: {OUTPUT_DIR}")
    print(f"Manifest: {manifest_file}")
    print(f"\nBodies repacked: {list(manifest.keys())}")


if __name__ == "__main__":
    main()
