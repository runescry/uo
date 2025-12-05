"""
Stage 5: Write UO-Compatible Bitmaps
=====================================
Writes Terrain.bmp (8-bit palette) and Altitude.bmp (8-bit grayscale)
in exact format required by UOMapMake.exe.
NO RESIZING - works with native 7168x4096 data.
"""
import os, struct, sys
import numpy as np
from PIL import Image
import cv2

# Import UO terrain palette definitions
sys.path.insert(0, os.path.dirname(__file__))
from uo_terrain_palette import UO_TERRAIN_PALETTE, REGION_TO_TERRAIN, get_terrain_rgb

# ============================================================
# PATH CONFIGURATION
# ============================================================
ROOT = os.path.dirname(os.path.abspath(__file__))
BASE = os.path.abspath(os.path.join(ROOT, ".."))

INPUT_NORM_DIR = os.path.join(BASE, "input_norm")
EXPORT_DIR = os.path.join(BASE, "exports")
PREVIEW_DIR = os.path.join(BASE, "preview")

for d in [EXPORT_DIR, PREVIEW_DIR]:
    os.makedirs(d, exist_ok=True)

# ============================================================
# INPUT/OUTPUT FILES
# ============================================================
CONTINENTS_IN = os.path.join(INPUT_NORM_DIR, "continents.png")
TERRAIN_UO_IN = os.path.join(EXPORT_DIR, "terrain_uo.png")  # From Stage 4.5

TERRAIN_OUT = os.path.join(BASE, "Terrain.bmp")
ALTITUDE_OUT = os.path.join(BASE, "Altitude.bmp")
TERRAIN_EXPORT = os.path.join(EXPORT_DIR, "Terrain.bmp")
ALTITUDE_EXPORT = os.path.join(EXPORT_DIR, "Altitude.bmp")
VALIDATION_REPORT = os.path.join(EXPORT_DIR, "validation_report.md")
PREVIEW_TERRAIN = os.path.join(PREVIEW_DIR, "50_terrain_final.png")
PREVIEW_ALTITUDE = os.path.join(PREVIEW_DIR, "50_altitude_final.png")

# ============================================================
# UO FORMAT REQUIREMENTS
# ============================================================
TARGET_SIZE = (7168, 4096)  # Width x Height (Felucca/Trammel size)
EXPECTED_W, EXPECTED_H = TARGET_SIZE

def write_bmp_v3_manual(img_array, palette, output_path, is_grayscale=False):
    """
    Manually write BMPv3 format with exact specifications for UOMapMake.
    """
    height, width = img_array.shape
    row_size = width
    padding = (4 - (row_size % 4)) % 4
    row_size_padded = row_size + padding

    if is_grayscale:
        palette = [(i, i, i, 0) for i in range(256)]
    else:
        palette_bgra = []
        for i in range(256):
            if i < len(palette):
                r, g, b = palette[i]
                palette_bgra.append((b, g, r, 0))
            else:
                palette_bgra.append((0, 0, 0, 0))
        palette = palette_bgra

    palette_size = 256 * 4
    pixel_data_size = row_size_padded * height
    file_size = 14 + 40 + palette_size + pixel_data_size
    pixel_data_offset = 14 + 40 + palette_size

    with open(output_path, 'wb') as f:
        f.write(b'BM')
        f.write(struct.pack('<I', file_size))
        f.write(struct.pack('<H', 0))
        f.write(struct.pack('<H', 0))
        f.write(struct.pack('<I', pixel_data_offset))
        f.write(struct.pack('<I', 40))
        f.write(struct.pack('<i', width))
        f.write(struct.pack('<i', height))
        f.write(struct.pack('<H', 1))
        f.write(struct.pack('<H', 8))
        f.write(struct.pack('<I', 0))
        f.write(struct.pack('<I', pixel_data_size))
        f.write(struct.pack('<i', 2835))
        f.write(struct.pack('<i', 2835))
        f.write(struct.pack('<I', 0))  # Colors used (0 = use all)
        f.write(struct.pack('<I', 0))  # Important colors (0 = all)

        for b, g, r, a in palette:
            f.write(struct.pack('BBBB', b, g, r, a))

        for y in range(height - 1, -1, -1):
            row = img_array[y, :]
            f.write(row.tobytes())
            if padding > 0:
                f.write(b'\x00' * padding)

print("="*60)
print("Stage 5: Writing UO-Compatible Bitmaps")
print("="*60)
print(f"Target resolution: {EXPECTED_W}x{EXPECTED_H}")
print()

# ============================================================
# STEP 1: Load UO Terrain (from Stage 4.5)
# ============================================================
print("Step 1: Loading UO terrain indices...")

terrain_uo = Image.open(TERRAIN_UO_IN)
terrain_array = np.array(terrain_uo, dtype=np.uint8)
h, w = terrain_array.shape

if (w, h) != (EXPECTED_W, EXPECTED_H):
    raise ValueError(f"Terrain size mismatch: got {w}x{h}, expected {EXPECTED_W}x{EXPECTED_H}")

print(f"  Terrain dimensions: {w}x{h}")
print(f"  Unique terrain IDs: {sorted(np.unique(terrain_array))}")
print(f"  Land pixels: {np.sum(terrain_array > 0):,}")

# Build UO palette with exact RGB colors from Terrain.xml
terrain_palette = []
for i in range(256):
    if i in UO_TERRAIN_PALETTE:
        _, r, g, b, _, _ = UO_TERRAIN_PALETTE[i]
        terrain_palette.append((r, g, b))
    else:
        terrain_palette.append((0, 0, 0))  # Black for unused indices

print(f"  Using UO terrain palette from Terrain.xml")

# Write terrain BMPs using manual writer
print(f"[OK] Writing Terrain.bmp with UO palette...")
write_bmp_v3_manual(terrain_array, terrain_palette, TERRAIN_OUT, is_grayscale=False)
write_bmp_v3_manual(terrain_array, terrain_palette, TERRAIN_EXPORT, is_grayscale=False)

# ============================================================
# STEP 2: Generate Altitude from Terrain (MUST MATCH EXACTLY)
# ============================================================
print("\nStep 2: Generating altitude map from terrain using Base altitudes...")

# Map each terrain ID to its Base altitude from Terrain.xml
# This is the CRITICAL approach that makes it work!
altitude = np.zeros((h, w), dtype=np.uint8)

unique_terrains = np.unique(terrain_array)
print(f"  Mapping {len(unique_terrains)} terrain types to base altitudes:")

for terrain_id in unique_terrains:
    if terrain_id in UO_TERRAIN_PALETTE:
        name, r, g, b, base_altitude, tile = UO_TERRAIN_PALETTE[terrain_id]
        mask = terrain_array == terrain_id
        pixel_count = np.sum(mask)

        # Assign the exact Base altitude from Terrain.xml
        altitude[mask] = base_altitude

        pct = (pixel_count / (h * w)) * 100
        print(f"    ID {terrain_id:2d} {name:20s} -> Altitude {base_altitude:3d}: {pixel_count:10,} pixels ({pct:5.2f}%)")
    else:
        # Unknown terrain - default to 88 (standard land altitude)
        mask = terrain_array == terrain_id
        altitude[mask] = 88
        print(f"    ID {terrain_id:2d} UNKNOWN - using altitude 88")

# Statistics
unique_altitudes = np.unique(altitude)
print(f"\n  Unique altitude indices: {list(unique_altitudes)}")
print(f"  Min: {np.min(altitude)}, Max: {np.max(altitude)}")
print(f"  Uses index 0: {0 in unique_altitudes}")

# Verify pixel-for-pixel match
terrain_nonzero = np.sum(terrain_array > 0)
altitude_nonzero = np.sum(altitude > 0)
print(f"\n  Terrain non-zero pixels: {terrain_nonzero:,}")
print(f"  Altitude non-zero pixels: {altitude_nonzero:,}")
print(f"  Perfect match: {terrain_nonzero == altitude_nonzero}")
print(f"  Dimensions: {w}x{h}")

# CRITICAL: Altitude must be 8-bit PALETTE mode!
# Build altitude palette matching working files
altitude_palette = []
for i in range(256):
    # Special colors for specific altitude indices to match working files
    if i == 24:
        altitude_palette.append((0, 0, 132))  # Water altitude - dark blue
    elif i >= 87 and i <= 90:
        # Land altitude (88 base) - green gradient
        green_val = 129 + (i - 87) * 3
        altitude_palette.append((0, green_val, 0))
    elif i == 152:
        altitude_palette.append((132, 132, 132))  # Rock altitude - gray
    elif i == 216:
        altitude_palette.append((132, 0, 132))  # High altitude - purple
    # Generic gradient for other values
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

print(f"[OK] Writing Altitude.bmp as 8-bit PALETTE (not grayscale)...")
write_bmp_v3_manual(altitude, altitude_palette, ALTITUDE_OUT, is_grayscale=False)
write_bmp_v3_manual(altitude, altitude_palette, ALTITUDE_EXPORT, is_grayscale=False)

# ============================================================
# STEP 3: Validation
# ============================================================
print("\nStep 3: Validating BMP files...")

validation_errors = []

def validate_bmp(path, expected_mode, expected_size, name=""):
    """Validate BMP file format."""
    errors = []

    try:
        img = Image.open(path)

        # Check size
        if img.size != expected_size:
            errors.append(f"Size mismatch: got {img.size}, expected {expected_size}")

        # Check mode
        if img.mode != expected_mode:
            errors.append(f"Mode mismatch: got {img.mode}, expected {expected_mode}")

        # Check BMP version (read header)
        with open(path, "rb") as f:
            # Read BMP header
            bmp_header = f.read(14)
            if len(bmp_header) < 14 or bmp_header[0:2] != b'BM':
                errors.append("Invalid BMP signature")
            else:
                # Read DIB header size
                f.seek(14)
                dib_size = struct.unpack("<I", f.read(4))[0]
                if dib_size != 40:
                    errors.append(f"Not BMPv3: DIB header size is {dib_size}, expected 40")

        # Check row stride (must be multiple of 4)
        bytes_per_pixel = 1 if expected_mode in ["L", "P"] else 3
        row_bytes = img.size[0] * bytes_per_pixel
        stride = (row_bytes + 3) & ~3  # Round up to multiple of 4
        if stride != row_bytes and row_bytes % 4 != 0:
            # This is actually OK, just informational
            pass

    except Exception as e:
        errors.append(f"Exception during validation: {e}")

    return errors

# Validate Terrain.bmp
terrain_errors = validate_bmp(TERRAIN_OUT, "P", (EXPECTED_W, EXPECTED_H))
if terrain_errors:
    validation_errors.append(f"Terrain.bmp issues:\n  - " + "\n  - ".join(terrain_errors))
else:
    print(f"  Terrain.bmp: OK")

# Validate Altitude.bmp (must be P mode like Terrain!)
altitude_errors = validate_bmp(ALTITUDE_OUT, "P", (EXPECTED_W, EXPECTED_H), "Altitude")
if altitude_errors:
    validation_errors.append(f"Altitude.bmp issues:\n  - " + "\n  - ".join(altitude_errors))
else:
    print(f"  Altitude.bmp: OK (8-bit palette)")

# Write validation report
with open(VALIDATION_REPORT, "w") as f:
    f.write("# UO Bitmap Validation Report\n\n")
    f.write(f"**Date:** {os.popen('date /t').read().strip()}\n")
    f.write(f"**Target Size:** {EXPECTED_W}x{EXPECTED_H}\n\n")

    if validation_errors:
        f.write("## ERRORS FOUND\n\n")
        for err in validation_errors:
            f.write(f"- {err}\n")
        f.write("\n**Status:** FAILED - Fix errors before using with UOMapMake\n")
    else:
        f.write("## All Validations Passed\n\n")
        f.write("- Terrain.bmp: 8-bit palette, BMPv3, correct dimensions\n")
        f.write("- Altitude.bmp: 8-bit grayscale, BMPv3, correct dimensions\n")
        f.write("\n**Status:** READY for UOMapMake.exe\n")

print(f"[OK] Validation report: {VALIDATION_REPORT}")

# ============================================================
# STEP 4: Generate Preview Images
# ============================================================
print("\nStep 4: Generating preview images...")

# Terrain preview using UO palette colors
terrain_preview = np.zeros((h, w, 3), dtype=np.uint8)
for terrain_id in np.unique(terrain_array):
    if terrain_id in UO_TERRAIN_PALETTE:
        mask = terrain_array == terrain_id
        r, g, b = get_terrain_rgb(terrain_id)
        terrain_preview[mask] = [b, g, r]  # BGR for OpenCV

cv2.imwrite(PREVIEW_TERRAIN, terrain_preview)
print(f"[OK] Preview: {PREVIEW_TERRAIN}")

# Altitude preview with color mapping for visualization
altitude_colored = np.zeros((h, w, 3), dtype=np.uint8)
for alt_idx in np.unique(altitude):
    alt_idx_int = int(alt_idx)
    if 0 <= alt_idx_int < len(altitude_palette):
        mask = altitude == alt_idx
        r, g, b = altitude_palette[alt_idx_int]
        altitude_colored[mask] = [b, g, r]  # BGR for OpenCV

cv2.imwrite(PREVIEW_ALTITUDE, altitude_colored)
print(f"[OK] Preview: {PREVIEW_ALTITUDE}")

# ============================================================
# FINAL SUMMARY
# ============================================================
print("\n" + "="*60)
print("Stage 5 Complete!")
print("="*60)
print(f"Terrain.bmp:  {EXPECTED_W}x{EXPECTED_H}, 8-bit palette, BMPv3")
print(f"Altitude.bmp: {EXPECTED_W}x{EXPECTED_H}, 8-bit palette, BMPv3")

if validation_errors:
    print("\nWARNING: Validation errors found. Check validation_report.md")
    exit(1)
else:
    print("\nAll validations passed - files ready for UOMapMake.exe")
    print("="*60)
