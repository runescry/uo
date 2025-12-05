"""
Smart Altitude Rebuilder
========================
Combines terrain type constraints with original elevation data to create
realistic altitude variation while matching terrain pixel-for-pixel.
"""
import os
import sys
import struct
import numpy as np
from PIL import Image
import cv2

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
ALTITUDE_RGB = os.path.join(EXAMPLES, "altitude_new.bmp")
OUTPUT_ALTITUDE = os.path.join(EXAMPLES, "altitude_smart.bmp")

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
# MAIN
# ============================================================
print("="*70)
print("Smart Altitude Rebuilder")
print("="*70)

# Load terrain
print("\nLoading terrain...")
terrain = np.array(Image.open(TERRAIN_FILE))
h, w = terrain.shape
print(f"  Terrain dimensions: {w}x{h}")

# Load original altitude RGB to get elevation data
print("\nLoading altitude RGB...")
altitude_rgb = np.array(Image.open(ALTITUDE_RGB).convert('RGB'))
gray = cv2.cvtColor(altitude_rgb, cv2.COLOR_RGB2GRAY)
print(f"  Brightness range: {np.min(gray)} - {np.max(gray)}")

# Create altitude map
altitude_map = np.zeros((h, w), dtype=np.uint8)

print("\nMapping elevation based on terrain type:")

# For each terrain type, map brightness to appropriate altitude range
for terrain_id in np.unique(terrain):
    mask = terrain == terrain_id
    pixel_count = np.sum(mask)

    if terrain_id in UO_TERRAIN_PALETTE:
        name, r, g, b, base_altitude, tile = UO_TERRAIN_PALETTE[terrain_id]
    else:
        name = "Unknown"
        base_altitude = 88

    # Extract brightness values for this terrain type
    brightness = gray[mask]

    # Map brightness to altitude range based on terrain type
    if terrain_id == 5:  # Deep Water
        # Water: mostly 24, but can vary slightly
        altitude_map[mask] = 24

    elif terrain_id in [18, 19]:  # Rock, Blank
        # These can have varied elevation: 88-152 range
        # Map brightness 0-255 to 88-152
        quantized = np.clip((brightness / 255.0 * 64) + 88, 88, 152).astype(np.uint8)
        # Snap to valid indices: 88 or 152
        quantized[quantized < 120] = 88
        quantized[quantized >= 120] = 152
        altitude_map[mask] = quantized

    elif terrain_id in [20, 21, 42, 80]:  # Caves, Embankment, Beach
        # High elevation terrain: 152-216 range
        quantized = np.clip((brightness / 255.0 * 64) + 152, 152, 216).astype(np.uint8)
        quantized[quantized < 184] = 152
        quantized[quantized >= 184] = 216
        altitude_map[mask] = quantized

    else:  # All other land (grass, forest, sand, etc.)
        # Standard land: mostly 88, but allow some variation
        altitude_map[mask] = 88

    # Report
    unique_alts = np.unique(altitude_map[mask])
    alt_str = ', '.join([str(a) for a in unique_alts])
    pct = (pixel_count / (h*w)) * 100
    print(f"  ID {terrain_id:2d} {name:20s}: {pixel_count:10,} pixels ({pct:5.2f}%) -> altitudes [{alt_str}]")

# Build altitude palette matching working files
print("\nBuilding altitude palette...")
altitude_palette = []
for i in range(256):
    if i == 24:
        altitude_palette.append((0, 0, 132))  # Dark blue (water)
    elif i >= 87 and i <= 90:
        green_val = 129 + (i - 87) * 3
        altitude_palette.append((0, green_val, 0))  # Green gradient (land)
    elif i == 152:
        altitude_palette.append((132, 132, 132))  # Gray (mountains/rock)
    elif i == 216:
        altitude_palette.append((132, 0, 132))  # Purple (high peaks)
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

# Verify
unique_altitudes = np.unique(altitude_map)
print(f"\nAltitude statistics:")
print(f"  Unique altitude values: {list(unique_altitudes)}")
print(f"  Min: {np.min(altitude_map)}, Max: {np.max(altitude_map)}")
print(f"  Uses index 0: {0 in unique_altitudes}")

terrain_nonzero = np.sum(terrain > 0)
altitude_nonzero = np.sum(altitude_map > 0)
print(f"\nVerifying match:")
print(f"  Terrain non-zero: {terrain_nonzero:,}")
print(f"  Altitude non-zero: {altitude_nonzero:,}")
print(f"  Perfect match: {terrain_nonzero == altitude_nonzero}")

# Write
write_bmp_v3_manual(altitude_map, altitude_palette, OUTPUT_ALTITUDE)
print(f"\n[OK] Saved: {OUTPUT_ALTITUDE}")
print("\n" + "="*70)
print("Test with terrain_converted.bmp + altitude_smart.bmp")
print("="*70)
