"""
Stage 5.5: Fix BMP Format for UOMapMake Strict Requirements
============================================================
UOMapMake is very picky about BMP format. This script ensures:
- Exact BMPv3 format with proper color table
- Correct stride alignment
- Proper palette entries (256 colors for indexed)
- No extra headers or data
"""
import os, struct
import numpy as np
from PIL import Image

ROOT = os.path.dirname(os.path.abspath(__file__))
BASE = os.path.abspath(os.path.join(ROOT, ".."))
EXPORT_DIR = os.path.join(BASE, "exports")

TERRAIN_IN = os.path.join(EXPORT_DIR, "Terrain.bmp")
ALTITUDE_IN = os.path.join(EXPORT_DIR, "Altitude.bmp")

TERRAIN_OUT = os.path.join(EXPORT_DIR, "Terrain_fixed.bmp")
ALTITUDE_OUT = os.path.join(EXPORT_DIR, "Altitude_fixed.bmp")

print("="*60)
print("Stage 5.5: Fixing BMP Format for UOMapMake")
print("="*60)

def write_bmp_v3_manual(img_array, palette, output_path, is_grayscale=False):
    """
    Manually write BMPv3 format with exact specifications for UOMapMake.

    Args:
        img_array: numpy array of pixel data (H x W), values 0-255
        palette: list of (R, G, B) tuples, or None for grayscale
        output_path: output BMP file path
        is_grayscale: if True, create grayscale palette
    """
    height, width = img_array.shape

    # BMP requires rows to be padded to multiple of 4 bytes
    row_size = width  # 1 byte per pixel (8-bit)
    padding = (4 - (row_size % 4)) % 4
    row_size_padded = row_size + padding

    # Palette: 256 colors x 4 bytes (BGRA)
    if is_grayscale:
        # Grayscale: R=G=B=index, A=0
        palette = [(i, i, i, 0) for i in range(256)]
    else:
        # Ensure palette has exactly 256 entries
        if palette is None:
            raise ValueError("Palette required for indexed color")

        # Pad palette to 256 colors if needed
        palette_bgra = []
        for i in range(256):
            if i < len(palette):
                r, g, b = palette[i]
                palette_bgra.append((b, g, r, 0))  # BGRA format
            else:
                palette_bgra.append((0, 0, 0, 0))  # Black padding
        palette = palette_bgra

    palette_size = 256 * 4  # 1024 bytes

    # Calculate file size
    pixel_data_size = row_size_padded * height
    file_header_size = 14
    dib_header_size = 40
    file_size = file_header_size + dib_header_size + palette_size + pixel_data_size
    pixel_data_offset = file_header_size + dib_header_size + palette_size

    with open(output_path, 'wb') as f:
        # ====== BMP File Header (14 bytes) ======
        f.write(b'BM')                              # Signature
        f.write(struct.pack('<I', file_size))       # File size
        f.write(struct.pack('<H', 0))               # Reserved1
        f.write(struct.pack('<H', 0))               # Reserved2
        f.write(struct.pack('<I', pixel_data_offset))  # Pixel data offset

        # ====== DIB Header - BITMAPINFOHEADER (40 bytes) ======
        f.write(struct.pack('<I', 40))              # DIB header size
        f.write(struct.pack('<i', width))           # Width
        f.write(struct.pack('<i', height))          # Height (positive = bottom-up)
        f.write(struct.pack('<H', 1))               # Planes (always 1)
        f.write(struct.pack('<H', 8))               # Bits per pixel (8-bit)
        f.write(struct.pack('<I', 0))               # Compression (0 = BI_RGB)
        f.write(struct.pack('<I', pixel_data_size)) # Image size
        f.write(struct.pack('<i', 2835))            # X pixels per meter (72 DPI)
        f.write(struct.pack('<i', 2835))            # Y pixels per meter (72 DPI)
        f.write(struct.pack('<I', 256))             # Colors in palette
        f.write(struct.pack('<I', 0))               # Important colors (0 = all)

        # ====== Color Table (1024 bytes) ======
        for b, g, r, a in palette:
            f.write(struct.pack('BBBB', b, g, r, a))

        # ====== Pixel Data ======
        # BMP stores rows bottom-to-top
        for y in range(height - 1, -1, -1):
            row = img_array[y, :]
            f.write(row.tobytes())
            # Write padding bytes
            if padding > 0:
                f.write(b'\x00' * padding)

    print(f"[OK] Written: {output_path}")
    print(f"     Size: {width}x{height}, {file_size} bytes, 8-bit, BMPv3")

# ============================================================
# Fix Terrain.bmp
# ============================================================
print("\nFixing Terrain.bmp...")
terrain_img = Image.open(TERRAIN_IN)

if terrain_img.mode != 'P':
    print(f"  Converting from {terrain_img.mode} to palette mode...")
    terrain_img = terrain_img.convert('P', palette=Image.ADAPTIVE, colors=256)

# Get pixel data
terrain_array = np.array(terrain_img, dtype=np.uint8)

# Get palette (PIL returns as flat list: R1,G1,B1,R2,G2,B2,...)
palette_flat = terrain_img.getpalette()
if palette_flat is None:
    print("  ERROR: No palette found, creating default")
    terrain_palette = [(i, i, i) for i in range(256)]
else:
    terrain_palette = []
    for i in range(0, min(len(palette_flat), 768), 3):
        terrain_palette.append((palette_flat[i], palette_flat[i+1], palette_flat[i+2]))

print(f"  Palette colors: {len(terrain_palette)}")
print(f"  Dimensions: {terrain_array.shape[1]}x{terrain_array.shape[0]}")

write_bmp_v3_manual(terrain_array, terrain_palette, TERRAIN_OUT, is_grayscale=False)

# ============================================================
# Fix Altitude.bmp
# ============================================================
print("\nFixing Altitude.bmp...")
altitude_img = Image.open(ALTITUDE_IN)

if altitude_img.mode != 'L':
    print(f"  Converting from {altitude_img.mode} to grayscale...")
    altitude_img = altitude_img.convert('L')

# Get pixel data
altitude_array = np.array(altitude_img, dtype=np.uint8)

print(f"  Dimensions: {altitude_array.shape[1]}x{altitude_array.shape[0]}")
print(f"  Range: {altitude_array.min()}-{altitude_array.max()}")

write_bmp_v3_manual(altitude_array, None, ALTITUDE_OUT, is_grayscale=True)

# ============================================================
# Copy to root for UOMapMake convenience
# ============================================================
import shutil

print("\nCopying to pipeline root for UOMapMake...")
shutil.copy(TERRAIN_OUT, os.path.join(BASE, "Terrain.bmp"))
shutil.copy(ALTITUDE_OUT, os.path.join(BASE, "Altitude.bmp"))
print(f"[OK] Copied to: {BASE}")

print("\n" + "="*60)
print("Stage 5.5 Complete!")
print("="*60)
print(f"Files ready in: {BASE}")
print("  - Terrain.bmp")
print("  - Altitude.bmp")
print("\nRun UOMapMake.exe from this directory.")
print("="*60)
