"""
Fix Altitude to Match Terrain Exactly
======================================
Ensures every pixel in altitude corresponds to terrain:
- Where terrain = ocean (ID 5) → altitude = 1 (low)
- Where terrain = land (not 5) → altitude = elevation based on brightness
"""
import os
import sys
import struct
import numpy as np
from PIL import Image
import cv2

# ============================================================
# PATHS
# ============================================================
ROOT = os.path.dirname(os.path.abspath(__file__))
BASE = os.path.abspath(os.path.join(ROOT, ".."))
EXAMPLES = os.path.join(BASE, "examples")

TERRAIN_FILE = os.path.join(EXAMPLES, "terrain_converted.bmp")
ALTITUDE_RGB = os.path.join(EXAMPLES, "altitude_new.bmp")  # Original RGB
OUTPUT_ALTITUDE = os.path.join(EXAMPLES, "altitude_matched.bmp")

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
print("Fix Altitude to Match Terrain Exactly")
print("="*70)

# Load terrain (palette mode)
print("\nLoading terrain (converted)...")
terrain = np.array(Image.open(TERRAIN_FILE))
h, w = terrain.shape
print(f"  Terrain dimensions: {w}x{h}")

# Identify ocean vs land from terrain
ocean_mask = (terrain == 5)  # Deep Water = ID 5
land_mask = ~ocean_mask

ocean_pixels = np.sum(ocean_mask)
land_pixels = np.sum(land_mask)

print(f"  Ocean pixels (terrain ID 5): {ocean_pixels:,}")
print(f"  Land pixels (other IDs): {land_pixels:,}")

# Load altitude RGB to get elevation data
print("\nLoading altitude RGB (original)...")
altitude_rgb = np.array(Image.open(ALTITUDE_RGB).convert('RGB'))
print(f"  Altitude dimensions: {altitude_rgb.shape[1]}x{altitude_rgb.shape[0]}")

# Convert to grayscale for elevation
gray = cv2.cvtColor(altitude_rgb, cv2.COLOR_RGB2GRAY)

# Create altitude map
altitude_map = np.zeros((h, w), dtype=np.uint8)

# CRITICAL: Set ocean to index 1
altitude_map[ocean_mask] = 1

# For land areas, quantize brightness to elevation levels
# Extract only land pixels
land_brightness = gray[land_mask]

if len(land_brightness) > 0:
    # Quantize land to 6 elevation levels (10, 20, 30, 50, 80, 100)
    # Skip index 1 since that's reserved for ocean
    elevation_bands = [
        (0, 42),      # Very low coastal -> 10
        (43, 85),     # Low -> 20
        (86, 127),    # Medium low -> 30
        (128, 170),   # Medium -> 50
        (171, 212),   # Medium high -> 80
        (213, 255),   # High/peaks -> 100
    ]

    palette_indices = [10, 20, 30, 50, 80, 100]

    print("\nQuantizing land elevation:")
    for i, (min_val, max_val) in enumerate(elevation_bands):
        # Find land pixels in this brightness range
        range_mask = land_mask & (gray >= min_val) & (gray <= max_val)
        pixel_count = np.sum(range_mask)

        if pixel_count > 0:
            altitude_map[range_mask] = palette_indices[i]
            pct = (pixel_count / land_pixels) * 100
            print(f"  Level {i+1} (idx {palette_indices[i]:3d}): {min_val:3d}-{max_val:3d} -> {pixel_count:10,} pixels ({pct:5.2f}%)")

# Verify matching
print("\nVerifying terrain/altitude matching:")
terrain_land_count = np.sum(terrain != 5)
altitude_land_count = np.sum(altitude_map != 1)

print(f"  Terrain land pixels: {terrain_land_count:,}")
print(f"  Altitude land pixels: {altitude_land_count:,}")
print(f"  Match: {terrain_land_count == altitude_land_count}")

# Check for mismatches
mismatches = np.sum((terrain == 5) != (altitude_map == 1))
print(f"  Pixel mismatches: {mismatches:,}")

if mismatches > 0:
    print("\n  ERROR: Terrain and altitude still don't match!")
else:
    print("\n  SUCCESS: Perfect pixel-for-pixel match!")

# Build altitude palette
altitude_palette = []
for i in range(256):
    if i == 1:
        altitude_palette.append((0, 0, 130))  # Ocean blue
    elif i < 50:
        altitude_palette.append((0, 100, 0))   # Green lowlands
    elif i < 100:
        altitude_palette.append((139, 69, 19))  # Brown hills
    else:
        altitude_palette.append((200, 200, 200))  # Gray mountains

# Write BMP
print(f"\nWriting matched altitude BMP...")
write_bmp_v3_manual(altitude_map, altitude_palette, OUTPUT_ALTITUDE)

# Final stats
unique_indices = np.unique(altitude_map)
print(f"\nAltitude statistics:")
print(f"  Unique indices: {list(unique_indices)}")
print(f"  Uses index 0: {0 in unique_indices}")
print(f"  Min index: {np.min(altitude_map)}")
print(f"  Max index: {np.max(altitude_map)}")

print(f"\n[OK] Matched altitude saved: {OUTPUT_ALTITUDE}")
print("\n" + "="*70)
print("Test terrain_converted.bmp + altitude_matched.bmp with UOMapMake")
print("="*70)
