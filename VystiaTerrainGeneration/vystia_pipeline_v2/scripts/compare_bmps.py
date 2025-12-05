"""
BMP File Comparison Tool
=========================
Compares working vs non-working BMP files to identify structural differences.
"""
import os
import struct
import numpy as np
from PIL import Image

def read_bmp_header(filepath):
    """Read and parse BMP file headers."""
    with open(filepath, 'rb') as f:
        # BMP File Header (14 bytes)
        bmp_signature = f.read(2)
        file_size = struct.unpack('<I', f.read(4))[0]
        reserved1 = struct.unpack('<H', f.read(2))[0]
        reserved2 = struct.unpack('<H', f.read(2))[0]
        pixel_data_offset = struct.unpack('<I', f.read(4))[0]

        # DIB Header (BITMAPINFOHEADER - 40 bytes)
        dib_header_size = struct.unpack('<I', f.read(4))[0]
        width = struct.unpack('<i', f.read(4))[0]
        height = struct.unpack('<i', f.read(4))[0]
        color_planes = struct.unpack('<H', f.read(2))[0]
        bits_per_pixel = struct.unpack('<H', f.read(2))[0]
        compression = struct.unpack('<I', f.read(4))[0]
        image_size = struct.unpack('<I', f.read(4))[0]
        x_pixels_per_meter = struct.unpack('<i', f.read(4))[0]
        y_pixels_per_meter = struct.unpack('<i', f.read(4))[0]
        colors_in_palette = struct.unpack('<I', f.read(4))[0]
        important_colors = struct.unpack('<I', f.read(4))[0]

        # Read palette if present (for 8-bit images)
        palette = None
        if bits_per_pixel == 8:
            f.seek(14 + dib_header_size)
            palette_size = 256 if colors_in_palette == 0 else colors_in_palette
            palette = []
            for i in range(palette_size):
                b = struct.unpack('B', f.read(1))[0]
                g = struct.unpack('B', f.read(1))[0]
                r = struct.unpack('B', f.read(1))[0]
                reserved = struct.unpack('B', f.read(1))[0]
                palette.append((r, g, b))

        # Read first 100 pixel values
        f.seek(pixel_data_offset)
        pixel_sample = f.read(100)

    return {
        'signature': bmp_signature,
        'file_size': file_size,
        'reserved1': reserved1,
        'reserved2': reserved2,
        'pixel_data_offset': pixel_data_offset,
        'dib_header_size': dib_header_size,
        'width': width,
        'height': height,
        'color_planes': color_planes,
        'bits_per_pixel': bits_per_pixel,
        'compression': compression,
        'compression_name': {0: 'BI_RGB', 1: 'BI_RLE8', 2: 'BI_RLE4', 3: 'BI_BITFIELDS'}.get(compression, 'Unknown'),
        'image_size': image_size,
        'x_ppm': x_pixels_per_meter,
        'y_ppm': y_pixels_per_meter,
        'colors_in_palette': colors_in_palette,
        'important_colors': important_colors,
        'palette': palette,
        'pixel_sample': pixel_sample
    }

def analyze_pixel_data(filepath):
    """Analyze pixel data statistics."""
    img = Image.open(filepath)
    arr = np.array(img)

    unique_vals = np.unique(arr)

    return {
        'mode': img.mode,
        'size': img.size,
        'unique_values': unique_vals,
        'num_unique': len(unique_vals),
        'min_value': np.min(arr),
        'max_value': np.max(arr),
        'uses_zero': 0 in unique_vals,
        'value_histogram': {int(v): int(np.sum(arr == v)) for v in unique_vals[:20]}  # First 20 values
    }

def compare_files(file1, file2, name):
    """Compare two BMP files and report differences."""
    print(f"\n{'='*70}")
    print(f"COMPARING: {name}")
    print(f"{'='*70}")
    print(f"Working:     {os.path.basename(file1)}")
    print(f"Non-working: {os.path.basename(file2)}")
    print()

    # Read headers
    h1 = read_bmp_header(file1)
    h2 = read_bmp_header(file2)

    # Compare headers
    print("HEADER COMPARISON:")
    print("-" * 70)

    differences = []

    for key in h1.keys():
        if key in ['palette', 'pixel_sample']:
            continue

        v1 = h1[key]
        v2 = h2[key]

        if v1 != v2:
            differences.append(key)
            print(f"  {key:25s}: {str(v1):20s} vs {str(v2):20s} *** DIFFERENT ***")
        else:
            print(f"  {key:25s}: {str(v1):20s} (same)")

    # Compare palettes
    if h1['palette'] and h2['palette']:
        print("\nPALETTE COMPARISON:")
        print("-" * 70)

        palette_diffs = []
        for i in range(min(20, len(h1['palette']), len(h2['palette']))):
            if h1['palette'][i] != h2['palette'][i]:
                palette_diffs.append(i)
                print(f"  Index {i:3d}: RGB{h1['palette'][i]} vs RGB{h2['palette'][i]} *** DIFFERENT ***")

        if not palette_diffs:
            print(f"  First 20 palette entries are identical")
        else:
            print(f"\n  Total palette differences in first 20: {len(palette_diffs)}")

    # Compare pixel data
    print("\nPIXEL DATA ANALYSIS:")
    print("-" * 70)

    p1 = analyze_pixel_data(file1)
    p2 = analyze_pixel_data(file2)

    print(f"  Mode:           {p1['mode']:15s} vs {p2['mode']}")
    print(f"  Size:           {str(p1['size']):15s} vs {str(p2['size'])}")
    print(f"  Unique values:  {p1['num_unique']:15d} vs {p2['num_unique']}")
    print(f"  Min value:      {p1['min_value']:15d} vs {p2['min_value']}")
    print(f"  Max value:      {p1['max_value']:15d} vs {p2['max_value']}")
    print(f"  Uses zero:      {str(p1['uses_zero']):15s} vs {str(p2['uses_zero'])}")

    if p1['uses_zero']:
        print(f"  Zero count:     {p1['value_histogram'].get(0, 0):15,} pixels")
    if p2['uses_zero']:
        print(f"                  vs {p2['value_histogram'].get(0, 0):,} pixels")

    # Compare pixel sample
    print("\nFIRST 100 PIXEL BYTES:")
    print("-" * 70)
    sample1 = list(h1['pixel_sample'][:50])
    sample2 = list(h2['pixel_sample'][:50])

    diffs = sum(1 for a, b in zip(sample1, sample2) if a != b)
    print(f"  Working:     {sample1[:20]}")
    print(f"  Non-working: {sample2[:20]}")
    print(f"  Differences in first 50 bytes: {diffs}")

    # Summary
    print("\n" + "="*70)
    print("SUMMARY:")
    print("="*70)
    if differences:
        print(f"  Header differences found in: {', '.join(differences)}")
    else:
        print(f"  Headers are structurally identical")

    if p1['uses_zero'] != p2['uses_zero']:
        print(f"  *** CRITICAL: Palette index 0 usage differs! ***")

    print()

# Main comparison
EXAMPLES = "C:\\DevEnv\\GIT\\UO\\VystiaGeneration\\vystia_pipeline_v2\\examples"

terrain_working = os.path.join(EXAMPLES, "Terrain.bmp")
terrain_broken = os.path.join(EXAMPLES, "terrain_new.bmp")
altitude_working = os.path.join(EXAMPLES, "Altitude.bmp")
altitude_broken = os.path.join(EXAMPLES, "altitude_new.bmp")

print("BMP FILE COMPARISON ANALYSIS")
print("="*70)

compare_files(terrain_working, terrain_broken, "TERRAIN")
compare_files(altitude_working, altitude_broken, "ALTITUDE")

print("\n" + "="*70)
print("ANALYSIS COMPLETE")
print("="*70)
