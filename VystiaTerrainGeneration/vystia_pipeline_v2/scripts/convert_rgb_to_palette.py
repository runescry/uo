"""
RGB to Palette BMP Converter
=============================
Converts 24-bit RGB BMP files to proper 8-bit palette-indexed BMPs
that work with UOMapMake.
"""
import os
import sys
import struct
import numpy as np
from PIL import Image
import cv2

# Import UO terrain palette
sys.path.insert(0, os.path.dirname(__file__))
from uo_terrain_palette import UO_TERRAIN_PALETTE, get_terrain_rgb

# ============================================================
# CONFIGURATION
# ============================================================
ROOT = os.path.dirname(os.path.abspath(__file__))
BASE = os.path.abspath(os.path.join(ROOT, ".."))

EXAMPLES_DIR = os.path.join(BASE, "examples")

INPUT_TERRAIN = os.path.join(EXAMPLES_DIR, "terrain_new.bmp")
INPUT_ALTITUDE = os.path.join(EXAMPLES_DIR, "altitude_new.bmp")

OUTPUT_TERRAIN = os.path.join(EXAMPLES_DIR, "terrain_converted.bmp")
OUTPUT_ALTITUDE = os.path.join(EXAMPLES_DIR, "altitude_converted.bmp")

# ============================================================
# BMP WRITER
# ============================================================
def write_bmp_v3_manual(data, palette, filepath):
    """
    Write 8-bit palette BMP manually with exact structure control.

    Args:
        data: 2D numpy array of palette indices (uint8)
        palette: List of 256 (R,G,B) tuples
        filepath: Output file path
    """
    h, w = data.shape

    # Ensure we have exactly 256 palette entries
    full_palette = []
    for i in range(256):
        if i < len(palette):
            full_palette.append(palette[i])
        else:
            full_palette.append((0, 0, 0))

    # BMP rows must be padded to 4-byte boundary
    row_size = w
    padding = (4 - (row_size % 4)) % 4
    padded_row_size = row_size + padding

    pixel_data_size = padded_row_size * h
    palette_size = 256 * 4  # 256 colors × 4 bytes (BGRA)

    # BMP File Header (14 bytes)
    file_size = 14 + 40 + palette_size + pixel_data_size
    pixel_data_offset = 14 + 40 + palette_size  # 1078

    bmp_header = struct.pack('<2sIHHI',
        b'BM',              # Signature
        file_size,          # File size
        0,                  # Reserved
        0,                  # Reserved
        pixel_data_offset   # Pixel data offset (1078)
    )

    # DIB Header (BITMAPINFOHEADER - 40 bytes)
    dib_header = struct.pack('<IiiHHIIiiII',
        40,                 # DIB header size
        w,                  # Width
        h,                  # Height (positive = bottom-up)
        1,                  # Color planes
        8,                  # Bits per pixel
        0,                  # Compression (BI_RGB)
        pixel_data_size,    # Image size
        3779,               # X pixels per meter
        3779,               # Y pixels per meter
        0,                  # Colors in palette (0 = use all 256)
        0                   # Important colors (0 = all)
    )

    # Palette (256 × 4 bytes = 1024 bytes)
    palette_data = bytearray()
    for r, g, b in full_palette:
        palette_data.extend([b, g, r, 0])  # BGRA format

    # Pixel data (bottom-up, padded rows)
    pixel_data = bytearray()
    for y in range(h - 1, -1, -1):  # Bottom-up
        row = data[y, :].tobytes()
        pixel_data.extend(row)
        pixel_data.extend(b'\x00' * padding)  # Row padding

    # Write file
    with open(filepath, 'wb') as f:
        f.write(bmp_header)
        f.write(dib_header)
        f.write(palette_data)
        f.write(pixel_data)

# ============================================================
# COLOR MAPPING
# ============================================================
def rgb_distance(c1, c2):
    """Euclidean distance between RGB colors."""
    return np.sqrt(sum((a - b) ** 2 for a, b in zip(c1, c2)))

def find_closest_terrain_id(rgb):
    """Find closest UO terrain type for an RGB color."""
    best_id = 1
    best_dist = float('inf')

    for terrain_id, (name, r, g, b, _, _) in UO_TERRAIN_PALETTE.items():
        if terrain_id == 0:  # Skip "No Draw"
            continue

        dist = rgb_distance(rgb, (r, g, b))
        if dist < best_dist:
            best_dist = dist
            best_id = terrain_id

    return best_id

# ============================================================
# TERRAIN CONVERTER
# ============================================================
def convert_terrain_to_palette(input_path, output_path):
    """
    Convert 24-bit RGB terrain BMP to 8-bit palette BMP.
    Maps each RGB color to the closest UO terrain type.
    """
    print(f"\nConverting Terrain: {os.path.basename(input_path)}")
    print("="*70)

    # Load RGB image
    img = Image.open(input_path)
    img_rgb = img.convert('RGB')
    arr = np.array(img_rgb)
    h, w, _ = arr.shape

    print(f"  Input dimensions: {w}×{h}")
    print(f"  Input mode: {img.mode}")

    # Build color-to-terrain mapping
    print(f"  Analyzing unique colors...")

    # Reshape to list of pixels
    pixels = arr.reshape(-1, 3)
    unique_colors = np.unique(pixels, axis=0)

    print(f"  Found {len(unique_colors)} unique RGB colors")
    print(f"  Mapping to UO terrain palette...")

    # Create lookup table
    color_to_terrain = {}
    for color in unique_colors:
        rgb = tuple(color)
        terrain_id = find_closest_terrain_id(rgb)
        color_to_terrain[rgb] = terrain_id

    # Map pixels to terrain IDs
    terrain_map = np.zeros((h, w), dtype=np.uint8)
    for y in range(h):
        for x in range(w):
            rgb = tuple(arr[y, x])
            terrain_map[y, x] = color_to_terrain[rgb]

    # Build UO terrain palette
    terrain_palette = []
    for i in range(256):
        if i in UO_TERRAIN_PALETTE:
            _, r, g, b, _, _ = UO_TERRAIN_PALETTE[i]
            terrain_palette.append((r, g, b))
        else:
            terrain_palette.append((0, 0, 0))

    # Statistics
    unique_ids, counts = np.unique(terrain_map, return_counts=True)
    print(f"\n  Terrain distribution:")
    for tid, count in zip(unique_ids, counts):
        if tid in UO_TERRAIN_PALETTE:
            name = UO_TERRAIN_PALETTE[tid][0]
            pct = (count / (h*w)) * 100
            print(f"    ID {tid:2d} {name:20s}: {count:10,} pixels ({pct:5.2f}%)")

    # Check for palette index 0
    uses_zero = 0 in unique_ids
    print(f"\n  Uses palette index 0: {uses_zero}")

    if uses_zero:
        print(f"  WARNING: Shifting all indices up by 1 to avoid index 0")
        terrain_map[terrain_map == 0] = 1  # Shift 0 to 1

    # Write BMP
    write_bmp_v3_manual(terrain_map, terrain_palette, output_path)
    print(f"\n[OK] Converted terrain saved: {output_path}")

# ============================================================
# ALTITUDE CONVERTER
# ============================================================
def convert_altitude_to_palette(input_path, output_path):
    """
    Convert 24-bit RGB altitude BMP to 8-bit palette BMP.
    Extracts brightness as elevation and quantizes to discrete levels.
    """
    print(f"\nConverting Altitude: {os.path.basename(input_path)}")
    print("="*70)

    # Load RGB image
    img = Image.open(input_path)
    img_rgb = img.convert('RGB')
    arr = np.array(img_rgb)
    h, w, _ = arr.shape

    print(f"  Input dimensions: {w}×{h}")
    print(f"  Input mode: {img.mode}")

    # Convert to grayscale (brightness = elevation)
    gray = cv2.cvtColor(arr, cv2.COLOR_RGB2GRAY)

    print(f"  Brightness range: {np.min(gray)} - {np.max(gray)}")

    # Quantize to 7 discrete elevation levels
    elevation_bands = [
        (0, 36),      # Ocean/lowest -> index 1
        (37, 73),     # Very low -> index 10
        (74, 110),    # Low -> index 20
        (111, 146),   # Medium low -> index 30
        (147, 183),   # Medium -> index 50
        (184, 220),   # Medium high -> index 80
        (221, 255),   # High/peaks -> index 100
    ]

    palette_indices = [1, 10, 20, 30, 50, 80, 100]

    altitude_map = np.zeros((h, w), dtype=np.uint8)

    for i, (min_val, max_val) in enumerate(elevation_bands):
        mask = (gray >= min_val) & (gray <= max_val)
        altitude_map[mask] = palette_indices[i]
        pixel_count = np.sum(mask)
        pct = (pixel_count / (h*w)) * 100
        print(f"  Level {i+1} (idx {palette_indices[i]:3d}): {min_val:3d}-{max_val:3d} -> {pixel_count:10,} pixels ({pct:5.2f}%)")

    # Build altitude palette (gradient for visualization)
    altitude_palette = []
    for i in range(256):
        # Create a gradient from blue (low) to white (high)
        if i < 10:
            altitude_palette.append((0, 0, 130))  # Deep water blue
        elif i < 50:
            altitude_palette.append((0, 100, 0))   # Green lowlands
        elif i < 100:
            altitude_palette.append((139, 69, 19))  # Brown hills
        else:
            altitude_palette.append((200, 200, 200))  # Gray mountains

    # Statistics
    unique_indices = np.unique(altitude_map)
    print(f"\n  Unique altitude indices: {list(unique_indices)}")
    print(f"  Uses palette index 0: {0 in unique_indices}")

    # Write BMP
    write_bmp_v3_manual(altitude_map, altitude_palette, output_path)
    print(f"\n[OK] Converted altitude saved: {output_path}")

# ============================================================
# MAIN
# ============================================================
print("="*70)
print("RGB to Palette BMP Converter")
print("="*70)

# Convert terrain
if os.path.exists(INPUT_TERRAIN):
    convert_terrain_to_palette(INPUT_TERRAIN, OUTPUT_TERRAIN)
else:
    print(f"\n[SKIP] Terrain file not found: {INPUT_TERRAIN}")

# Convert altitude
if os.path.exists(INPUT_ALTITUDE):
    convert_altitude_to_palette(INPUT_ALTITUDE, OUTPUT_ALTITUDE)
else:
    print(f"\n[SKIP] Altitude file not found: {INPUT_ALTITUDE}")

print("\n" + "="*70)
print("Conversion Complete!")
print("="*70)
print("\nConverted files:")
print(f"  {OUTPUT_TERRAIN}")
print(f"  {OUTPUT_ALTITUDE}")
print("\nTest these files with UOMapMake to verify they work.")
print("="*70)
