#!/usr/bin/env python3
"""Test decoding a single frame"""
import struct
from PIL import Image

mul = r'C:\DevEnv\GIT\UO\UO Adventures\Client\Data Files\anim2.mul'
idx = r'C:\DevEnv\GIT\UO\UO Adventures\Client\Data Files\anim2.idx'

def convert_uo_color(color_16):
    color_16 ^= 0x8000
    r = ((color_16 >> 10) & 0x1F) << 3
    g = ((color_16 >> 5) & 0x1F) << 3
    b = (color_16 & 0x1F) << 3
    a = 255 if (color_16 & 0x8000) else 0
    return (r, g, b, a)

def get_index_offset(body_id, action, direction):
    if body_id < 200:
        base = body_id * 110
    return (base + (action * 5) + direction) * 12

# Read animation data
body_id, action, direction = 64, 0, 0
idx_offset = get_index_offset(body_id, action, direction)

with open(idx, 'rb') as f:
    f.seek(idx_offset)
    offset, length, extra = struct.unpack('<III', f.read(12))

with open(mul, 'rb') as f:
    f.seek(offset)
    data = f.read(length)

print(f'Reading body {body_id}, action {action}, dir {direction}')
print(f'Data length: {len(data)} bytes')
print()

# Read palette
palette = []
for i in range(256):
    color_16 = struct.unpack_from('<H', data, i*2)[0]
    palette.append(convert_uo_color(color_16))

# Read frame count
frame_count = struct.unpack_from('<I', data, 512)[0]
print(f'Frame count: {frame_count}')

# Read first frame offset
frame_lookup = struct.unpack_from('<I', data, 516)[0]
print(f'Frame 0 lookup: {frame_lookup}')

# Calculate frame offset
lookup_base = 512 + 4  # After palette and frame count
frame_offset = lookup_base + frame_lookup
print(f'Frame 0 absolute offset: {frame_offset}')
print()

# Read frame header
center_x = struct.unpack_from('<h', data, frame_offset)[0]
frame_offset += 2
center_y = struct.unpack_from('<h', data, frame_offset)[0]
frame_offset += 2
width = struct.unpack_from('<H', data, frame_offset)[0]
frame_offset += 2
height = struct.unpack_from('<H', data, frame_offset)[0]
frame_offset += 2

print(f'Frame header:')
print(f'  Center X: {center_x}')
print(f'  Center Y: {center_y}')
print(f'  Width: {width}')
print(f'  Height: {height}')
print()

if width > 0 and height > 0 and width < 1024 and height < 1024:
    print('Frame dimensions are valid')
    print(f'Creating {width}x{height} image...')

    img = Image.new('RGBA', (width, height), (0, 0, 0, 0))
    pixels = img.load()

    DOUBLE_XOR = 0x200
    pixels_written = 0
    run_count = 0

    # Decode runs
    while frame_offset + 4 <= len(data):
        run_header = struct.unpack_from('<I', data, frame_offset)[0]
        frame_offset += 4

        if run_header == 0x7FFF7FFF:
            print(f'End marker found after {run_count} runs')
            break

        run_header ^= DOUBLE_XOR

        x_offset = (run_header >> 22) & 0x3FF
        y_offset = (run_header >> 12) & 0x3FF
        run_length = run_header & 0xFFF

        if x_offset & 0x200:
            x_offset |= 0xFFFFFC00
        if y_offset & 0x200:
            y_offset |= 0xFFFFFC00
        if x_offset >= 0x200:
            x_offset = x_offset - 0x400
        if y_offset >= 0x200:
            y_offset = y_offset - 0x400

        run_count += 1
        if run_count <= 5:
            print(f'Run {run_count}: x_offset={x_offset}, y_offset={y_offset}, length={run_length}')

        for i in range(run_length):
            if frame_offset >= len(data):
                break

            palette_idx = data[frame_offset]
            frame_offset += 1

            x = x_offset + i
            y = y_offset

            if 0 <= x < width and 0 <= y < height:
                pixels[x, y] = palette[palette_idx]
                pixels_written += 1

    print(f'Total runs: {run_count}')
    print(f'Pixels written: {pixels_written}')

    output_file = r'C:\DevEnv\GIT\UO\Vystia\extracted_animations\test_frame.png'
    img.save(output_file)
    print(f'Saved to: {output_file}')
else:
    print('ERROR: Invalid frame dimensions!')
