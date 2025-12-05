"""
Stage 5.1: Generate Altitude Matching Terrain Exactly
======================================================
UOMapMake expects Terrain and Altitude to have a 1:1 pixel correspondence.
This generates altitude based on the exact terrain data.
"""
import os
import numpy as np
from PIL import Image
import cv2

ROOT = os.path.dirname(os.path.abspath(__file__))
BASE = os.path.abspath(os.path.join(ROOT, ".."))

INPUT_NORM_DIR = os.path.join(BASE, "input_norm")
EXPORT_DIR = os.path.join(BASE, "exports")
PREVIEW_DIR = os.path.join(BASE, "preview")

TERRAIN_IN = os.path.join(EXPORT_DIR, "Terrain.bmp")
CONTINENTS_IN = os.path.join(INPUT_NORM_DIR, "continents.png")
ALTITUDE_OUT = os.path.join(EXPORT_DIR, "Altitude_matched.bmp")
PREVIEW_OUT = os.path.join(PREVIEW_DIR, "51_altitude_matched.png")

print("="*60)
print("Stage 5.1: Generating Altitude Matched to Terrain")
print("="*60)

# Load terrain
terrain_img = Image.open(TERRAIN_IN)
terrain_array = np.array(terrain_img, dtype=np.uint8)
h, w = terrain_array.shape

print(f"Terrain dimensions: {w}x{h}")
print(f"Terrain unique values: {len(np.unique(terrain_array))}")

# Load continent mask for ocean detection
continents = cv2.imread(CONTINENTS_IN, cv2.IMREAD_GRAYSCALE)
_, land_mask = cv2.threshold(continents, 128, 255, cv2.THRESH_BINARY)

print(f"Land pixels: {np.sum(land_mask > 0):,}")
print(f"Ocean pixels: {np.sum(land_mask == 0):,}")

# Create altitude map
altitude = np.zeros((h, w), dtype=np.uint8)

# Ocean = 0
altitude[land_mask == 0] = 0

# Land base elevation using distance transform
# This creates smooth elevation that rises from coast
land_only = land_mask.copy()
dist_from_ocean = cv2.distanceTransform(land_only, cv2.DIST_L2, 5)
dist_max = dist_from_ocean.max()

if dist_max > 0:
    # Normalize to reasonable elevation range
    # Coast: 40-60
    # Interior: 80-120
    normalized_dist = dist_from_ocean / dist_max
    altitude[land_mask > 0] = (40 + normalized_dist[land_mask > 0] * 80).astype(np.uint8)

# Add some variation based on terrain palette index
# Different regions can have slight elevation variations
terrain_land = terrain_array[land_mask > 0]
for palette_idx in np.unique(terrain_land):
    if palette_idx == 0:  # Skip background
        continue

    mask = (terrain_array == palette_idx) & (land_mask > 0)
    if np.any(mask):
        # Add small random variation per region (±5 elevation)
        variation = np.random.randint(-5, 6, size=altitude[mask].shape)
        altitude[mask] = np.clip(altitude[mask] + variation, 0, 255).astype(np.uint8)

# Smooth slightly to remove harsh boundaries
altitude_land = altitude.copy()
altitude_land[land_mask > 0] = cv2.GaussianBlur(
    altitude[land_mask > 0].reshape(-1, 1), (5, 1), 0
).flatten()
altitude = altitude_land

# Ensure ocean is exactly 0
altitude[land_mask == 0] = 0

print(f"Altitude range: {altitude.min()}-{altitude.max()}")
print(f"Ocean elevation: {altitude[land_mask == 0].max()}")
print(f"Land elevation: {altitude[land_mask > 0].min()}-{altitude[land_mask > 0].max()}")

# Save as 8-bit grayscale BMP
altitude_img = Image.fromarray(altitude, mode='L')
altitude_img.save(ALTITUDE_OUT, 'BMP')

# Force BMPv3 header
import struct
with open(ALTITUDE_OUT, 'r+b') as f:
    f.seek(14)
    f.write(struct.pack('<I', 40))

print(f"[OK] Saved: {ALTITUDE_OUT}")

# Create preview
preview = cv2.applyColorMap(altitude, cv2.COLORMAP_JET)
cv2.imwrite(PREVIEW_OUT, preview)
print(f"[OK] Preview: {PREVIEW_OUT}")

print("\n" + "="*60)
print("Stage 5.1 Complete!")
print("Altitude now matches Terrain pixel-for-pixel")
print("="*60)
