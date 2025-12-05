#!/usr/bin/env python3
"""Check all actions/directions available for body 64"""
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

body_id = 64
print(f'Checking all actions for body {body_id} in anim2.mul...')
print()

for action in range(35):
    for direction in range(5):
        idx_offset = get_index_offset(body_id, action, direction)
        with open(idx, 'rb') as f:
            f.seek(idx_offset)
            offset, length, extra = struct.unpack('<III', f.read(12))

            if offset != 0xFFFFFFFF and length > 0:
                print(f'Action {action:2d}, Dir {direction}: offset={offset}, length={length}')
