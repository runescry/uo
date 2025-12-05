#!/usr/bin/env python3
"""Debug body 64 extraction"""
import struct

mul = r'C:\DevEnv\GIT\UO\UO Adventures\Client\Data Files\anim2.mul'
idx = r'C:\DevEnv\GIT\UO\UO Adventures\Client\Data Files\anim2.idx'

def get_index_offset(body_id, action, direction):
    if body_id < 200:
        base = body_id * 110
    elif body_id < 400:
        base = 22000 + ((body_id - 200) * 65)
    else:
        base = 35000 + ((body_id - 400) * 175)
    return (base + (action * 5) + direction) * 12

# Try action 0, direction 0
body_id = 64
action = 0
direction = 0

idx_offset = get_index_offset(body_id, action, direction)
with open(idx, 'rb') as f:
    f.seek(idx_offset)
    offset, length, extra = struct.unpack('<III', f.read(12))

print(f'Body {body_id}, Action {action}, Dir {direction}')
print(f'Offset: {offset}, Length: {length}, Extra: {extra}')
print()

# Read the data
with open(mul, 'rb') as f:
    f.seek(offset)
    data = f.read(length)

print(f'Data length: {len(data)} bytes')
print()

# Try to decode
# First 512 bytes = palette (256 colors × 2 bytes)
print('Reading palette...')
offset_in_data = 0
for i in range(256):
    color_16 = struct.unpack_from('<H', data, offset_in_data)[0]
    offset_in_data += 2

print(f'Palette read complete. Offset now at: {offset_in_data}')
print()

# Next 4 bytes = frame count
frame_count = struct.unpack_from('<I', data, offset_in_data)[0]
print(f'Frame count: {frame_count}')
offset_in_data += 4

if frame_count > 100 or frame_count == 0:
    print('ERROR: Invalid frame count!')
else:
    print('Frame count looks valid')

# Next frame_count × 4 bytes = frame offsets
print(f'Reading {frame_count} frame offsets...')
for i in range(min(frame_count, 10)):  # Show first 10
    lookup = struct.unpack_from('<I', data, offset_in_data + i*4)[0]
    print(f'  Frame {i}: lookup offset = {lookup}')
