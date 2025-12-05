"""
Rebuild Altitude from Terrain Using Base Altitude Values
=========================================================
Uses the exact Base altitude values from Terrain.xml for each terrain type.
This ensures altitude matches UOMapMake's expectations.
"""
import os
import sys
import struct
import numpy as np
from PIL import Image

# Import UO terrain palette
sys.path.insert(0, os.path.dirname(__file__))
from uo_terrain_palette import UO_TERRAIN_PALETTE

# ============================================================
# PATHS
# ============================================================
ROOT = os.path.dirname(os.path.abspath(__file__))
BASE = os.path.abspath(os.path.join(ROOT, ".."))
EXAMPLES = os.path.join(BASE, "examples")

TERRAIN_FILE = os.path.join(EXAMPLES, "terrain_converted.bmp")
OUTPUT_ALTITUDE = os.path.join(EXAMPLES, "altitude_from_terrain.bmp")

# ============================================================
# BMP WRITER
# ============================================================
def write_bmp_v3_manual(data, palette, filepath):
    """Write 8-bit palette BMP manually."""
    h, w = data.shape

    # Ensure 256 palette entries
    full_palette = []
    for i in range(256):
        if i < len(palette):
            full_palette.append(palette[i])
        else:
            full_palette.append((0, 0, 0))

    # Row padding
    row_size = w
    padding = (4 - (row_size % 4)) % 4
    padded_row_size = row_size + padding
    pixel_data_size = padded_row_size * h
    palette_size = 256 * 4

    file_size = 14 + 40 + palette_size + pixel_data_size
    pixel_data_offset = 14 + 40 + palette_size

    # BMP header
    bmp_header = struct.pack('<2sIHHI',
        b'BM', file_size, 0, 0, pixel_data_offset
    )

    # DIB header
    dib_header = struct.pack('<IiiHHIIiiII',
        40, w, h, 1, 8, 0, pixel_data_size,
        3779, 3779, 0, 0
    )

    # Palette
    palette_data = bytearray()
    for r, g, b in full_palette:
        palette_data.extend([b, g, r, 0])

    # Pixel data (bottom-up)
    pixel_data = bytearray()
    for y in range(h - 1, -1, -1):
        row = data[y, :].tobytes()
        pixel_data.extend(row)
        pixel_data.extend(b'\x00' * padding)

    # Write
    with open(filepath, 'wb') as f:
        f.write(bmp_header)
        f.write(dib_header)
        f.write(palette_data)
        f.write(pixel_data)

# ============================================================
# MAIN
# ============================================================
print("="*70)
print("Rebuild Altitude from Terrain Using Base Altitudes")
print("="*70)

# Load terrain
print("\nLoading terrain...")
terrain = np.array(Image.open(TERRAIN_FILE))
h, w = terrain.shape
print(f"  Terrain dimensions: {w}x{h}")

# Get unique terrain IDs
unique_terrains = np.unique(terrain)
print(f"  Unique terrain IDs: {list(unique_terrains)}")

# Build altitude map using Base altitude from Terrain.xml
print("\nMapping terrain to base altitudes:")
altitude_map = np.zeros((h, w), dtype=np.uint8)

terrain_to_altitude = {}

# Special override: ID 19 (Blank) should use altitude 152, not 88
# This matches the working example files
TERRAIN_ALTITUDE_OVERRIDES = {
    19: 152,  # Blank -> 152 (not 88)
}

for terrain_id in unique_terrains:
    if terrain_id in UO_TERRAIN_PALETTE:
        name, r, g, b, base_altitude, tile = UO_TERRAIN_PALETTE[terrain_id]

        # Apply override if exists
        if terrain_id in TERRAIN_ALTITUDE_OVERRIDES:
            actual_altitude = TERRAIN_ALTITUDE_OVERRIDES[terrain_id]
            print(f"  ID {terrain_id:2d} {name:20s} -> Altitude {actual_altitude:3d} (OVERRIDE from {base_altitude}): ", end='')
        else:
            actual_altitude = base_altitude
            print(f"  ID {terrain_id:2d} {name:20s} -> Altitude {actual_altitude:3d}: ", end='')

        terrain_to_altitude[terrain_id] = actual_altitude

        mask = terrain == terrain_id
        pixel_count = np.sum(mask)
        pct = (pixel_count / (h*w)) * 100

        altitude_map[mask] = actual_altitude

        print(f"{pixel_count:10,} pixels ({pct:5.2f}%)")
    else:
        print(f"  ID {terrain_id:2d} UNKNOWN - using altitude 88")
        terrain_to_altitude[terrain_id] = 88
        altitude_map[terrain == terrain_id] = 88

# Build altitude palette
# Key insight: Palette colors should create a visual gradient
# but the INDICES are what matter (they represent actual Z heights)
print("\nBuilding altitude palette...")
altitude_palette = []
for i in range(256):
    # Special colors for specific altitude indices to match working files
    if i == 24:
        # Water altitude - dark blue
        altitude_palette.append((0, 0, 132))
    elif i >= 87 and i <= 90:
        # Land altitude (88 base) - green gradient
        green_val = 129 + (i - 87) * 3
        altitude_palette.append((0, green_val, 0))
    elif i == 152:
        # Rock/Blank altitude - gray (matches working files)
        altitude_palette.append((132, 132, 132))  # GRAY
    elif i == 216:
        # High altitude - purple/magenta
        altitude_palette.append((132, 0, 132))
    # Generic gradient for other values
    elif i < 30:
        altitude_palette.append((0, 0, 132))
    elif i < 100:
        green_val = 100 + (i - 30)
        altitude_palette.append((0, min(255, green_val), 0))
    elif i < 200:
        gray_val = 100 + ((i - 100) // 2)
        altitude_palette.append((gray_val, gray_val, gray_val))
    else:
        altitude_palette.append((132, 0, 132))

# Statistics
unique_altitudes = np.unique(altitude_map)
print(f"\nAltitude statistics:")
print(f"  Unique altitude values: {list(unique_altitudes)}")
print(f"  Min: {np.min(altitude_map)}, Max: {np.max(altitude_map)}")
print(f"  Uses index 0: {0 in unique_altitudes}")

# Verify matching
terrain_nonzero = np.sum(terrain > 0)
altitude_nonzero = np.sum(altitude_map > 0)
print(f"\nVerifying match:")
print(f"  Terrain non-zero pixels: {terrain_nonzero:,}")
print(f"  Altitude non-zero pixels: {altitude_nonzero:,}")
print(f"  Perfect match: {terrain_nonzero == altitude_nonzero}")

# Write BMP
print(f"\nWriting altitude BMP...")
write_bmp_v3_manual(altitude_map, altitude_palette, OUTPUT_ALTITUDE)

print(f"\n[OK] Saved: {OUTPUT_ALTITUDE}")
print("\n" + "="*70)
print("This altitude file uses exact Base altitude values from Terrain.xml")
print("Test with terrain_converted.bmp + altitude_from_terrain.bmp")
print("="*70)
