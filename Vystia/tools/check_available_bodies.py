#!/usr/bin/env python3
"""
Check which body IDs are available in anim.idx/mul
"""
import struct

idx_path = r"C:\DevEnv\GIT\UO\UO Adventures\Client\Data Files\anim.idx"

print("Scanning for available body IDs in UO Adventures client...")
print()

available = []
for body_id in range(1000):  # Check first 1000 bodies
    if body_id < 200:
        base = body_id * 110
    elif body_id < 400:
        base = 22000 + ((body_id - 200) * 65)
    else:
        base = 35000 + ((body_id - 400) * 175)

    idx_offset = base * 12

    try:
        with open(idx_path, 'rb') as idx_file:
            idx_file.seek(idx_offset)
            offset, length, extra = struct.unpack('<III', idx_file.read(12))

            # Valid entry has non-FFFFFFFF values
            if offset != 0xFFFFFFFF and length != 0xFFFFFFFF and offset > 0 and length > 0:
                available.append(body_id)
    except:
        break

print(f"Found {len(available)} available body IDs:")
print()

# Show in groups of 10
for i in range(0, len(available), 10):
    group = available[i:i+10]
    print(f"{i:4d}-{i+9:4d}: {', '.join(str(x) for x in group)}")

print()
print(f"Body 64 available: {64 in available}")
print(f"Body 259 available: {259 in available}")
