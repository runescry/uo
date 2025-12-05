"""
Validate terrain1.bmp and Generate Altitude
============================================
1. Validates terrain1.bmp uses correct colors from Terrain.xml
2. Generates matching altitude.bmp using Base altitude values
"""
import os
import sys
import struct
import numpy as np
from PIL import Image

sys.path.insert(0, os.path.dirname(__file__))
from uo_terrain_palette import UO_TERRAIN_PALETTE

# ============================================================
# PATHS
# ============================================================
ROOT = os.path.dirname(os.path.abspath(__file__))
BASE = os.path.abspath(os.path.join(ROOT, ".."))
EXAMPLES = os.path.join(BASE, "examples")

TERRAIN_FILE = os.path.join(EXAMPLES, "terrain1.bmp")
OUTPUT_ALTITUDE = os.path.join(EXAMPLES, "altitude1.bmp")

# ============================================================
# BMP WRITER
# ============================================================
def write_bmp_v3_manual(data, palette, filepath):
    """Write 8-bit palette BMP manually."""
    h, w = data.shape

    full_palette = []
    for i in range(256):
        if i < len(palette):
            full_palette.append(palette[i])
        else:
            full_palette.append((0, 0, 0))

    row_size = w
    padding = (4 - (row_size % 4)) % 4
    padded_row_size = row_size + padding
    pixel_data_size = padded_row_size * h
    palette_size = 256 * 4

    file_size = 14 + 40 + palette_size + pixel_data_size
    pixel_data_offset = 14 + 40 + palette_size

    bmp_header = struct.pack('<2sIHHI',
        b'BM', file_size, 0, 0, pixel_data_offset
    )

    dib_header = struct.pack('<IiiHHIIiiII',
        40, w, h, 1, 8, 0, pixel_data_size,
        3779, 3779, 0, 0
    )

    palette_data = bytearray()
    for r, g, b in full_palette:
        palette_data.extend([b, g, r, 0])

    pixel_data = bytearray()
    for y in range(h - 1, -1, -1):
        row = data[y, :].tobytes()
        pixel_data.extend(row)
        pixel_data.extend(b'\x00' * padding)

    with open(filepath, 'wb') as f:
        f.write(bmp_header)
        f.write(dib_header)
        f.write(palette_data)
        f.write(pixel_data)

# ============================================================
# STEP 1: VALIDATE TERRAIN COLORS
# ============================================================
print("="*70)
print("STEP 1: VALIDATING TERRAIN1.BMP")
print("="*70)

terrain = Image.open(TERRAIN_FILE)
terrain_arr = np.array(terrain)
h, w = terrain_arr.shape

print(f"\nFile: {TERRAIN_FILE}")
print(f"Size: {w}x{h}")
print(f"Mode: {terrain.mode}")
print(f"Index range: {np.min(terrain_arr)} - {np.max(terrain_arr)}")

if terrain.mode != 'P':
    print(f"\nERROR: File is {terrain.mode} mode, needs to be palette (P) mode!")
    sys.exit(1)

# Get palette
palette = terrain.getpalette()
palette_rgb = [(palette[i*3], palette[i*3+1], palette[i*3+2]) for i in range(256)]

# Validate each terrain ID
unique_ids = np.unique(terrain_arr)
print(f"\nFound {len(unique_ids)} unique terrain IDs: {list(unique_ids)}")
print("\nValidation against Terrain.xml:")
print("-"*70)

all_valid = True
terrain_details = []

for tid in unique_ids:
    if tid == 0:
        print(f"ID {tid:3d}: WARNING - Index 0 should not be used!")
        all_valid = False
        continue

    actual_rgb = palette_rgb[tid]
    pixel_count = np.sum(terrain_arr == tid)
    pct = (pixel_count / (h * w)) * 100

    if tid in UO_TERRAIN_PALETTE:
        name, expected_r, expected_g, expected_b, base_alt, tile = UO_TERRAIN_PALETTE[tid]
        expected_rgb = (expected_r, expected_g, expected_b)

        if actual_rgb == expected_rgb:
            status = "OK"
            terrain_details.append((tid, name, base_alt, pixel_count, pct))
        else:
            status = f"WRONG (expected RGB{expected_rgb})"
            all_valid = False

        print(f"ID {tid:3d} {name:20s}: RGB{str(actual_rgb):20s} [{status:>25s}] - {pixel_count:10,} px ({pct:5.2f}%)")
    else:
        print(f"ID {tid:3d} {'UNKNOWN':20s}: RGB{str(actual_rgb):20s} [NOT IN TERRAIN.XML      ] - {pixel_count:10,} px")
        all_valid = False

print("\n" + "="*70)
if all_valid:
    print("RESULT: All colors match Terrain.xml - VALID")
else:
    print("RESULT: Some colors do not match - INVALID")
    print("\nCannot proceed with altitude generation until colors are fixed.")
    sys.exit(1)

# ============================================================
# STEP 2: GENERATE ALTITUDE FROM TERRAIN
# ============================================================
print("\n" + "="*70)
print("STEP 2: GENERATING ALTITUDE FROM TERRAIN")
print("="*70)

altitude_map = np.zeros((h, w), dtype=np.uint8)

print("\nMapping terrain IDs to base altitudes:")
print("-"*70)

for tid, name, base_alt, pixel_count, pct in terrain_details:
    mask = terrain_arr == tid
    altitude_map[mask] = base_alt
    print(f"ID {tid:3d} {name:20s} -> Altitude {base_alt:3d}: {pixel_count:10,} pixels ({pct:5.2f}%)")

# Build altitude palette (matches working files)
print("\nBuilding altitude palette...")
altitude_palette = []
for i in range(256):
    if i == 24:
        altitude_palette.append((0, 0, 132))  # Water - dark blue
    elif i >= 87 and i <= 90:
        green_val = 129 + (i - 87) * 3
        altitude_palette.append((0, green_val, 0))  # Land - green gradient
    elif i == 152:
        altitude_palette.append((132, 132, 132))  # Mountains - gray
    elif i == 216:
        altitude_palette.append((132, 0, 132))  # High peaks - purple
    elif i < 30:
        altitude_palette.append((0, 0, 132))
    elif i < 100:
        green_val = min(255, 100 + (i - 30))
        altitude_palette.append((0, green_val, 0))
    elif i < 200:
        gray_val = 100 + ((i - 100) // 2)
        altitude_palette.append((gray_val, gray_val, gray_val))
    else:
        altitude_palette.append((132, 0, 132))

# Statistics
unique_altitudes = np.unique(altitude_map)
print(f"\nAltitude statistics:")
print(f"  Unique altitude indices: {list(unique_altitudes)}")
print(f"  Min: {np.min(altitude_map)}, Max: {np.max(altitude_map)}")
print(f"  Uses index 0: {0 in unique_altitudes}")

# Verify pixel-for-pixel match
terrain_nonzero = np.sum(terrain_arr > 0)
altitude_nonzero = np.sum(altitude_map > 0)
print(f"\nPixel matching:")
print(f"  Terrain non-zero: {terrain_nonzero:,}")
print(f"  Altitude non-zero: {altitude_nonzero:,}")
print(f"  Perfect match: {terrain_nonzero == altitude_nonzero}")

# Write altitude BMP
print(f"\nWriting altitude BMP...")
write_bmp_v3_manual(altitude_map, altitude_palette, OUTPUT_ALTITUDE)

print(f"\n[OK] Saved: {OUTPUT_ALTITUDE}")
print("\n" + "="*70)
print("COMPLETE!")
print("="*70)
print(f"\nGenerated files:")
print(f"  Terrain: {TERRAIN_FILE}")
print(f"  Altitude: {OUTPUT_ALTITUDE}")
print(f"\nThese files are ready for UOMapMake.exe")
print("="*70)
