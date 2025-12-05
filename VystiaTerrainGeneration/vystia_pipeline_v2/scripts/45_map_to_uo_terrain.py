"""
Stage 4.5: Map Regions to UO Terrain Types
===========================================
Converts regions_final.png to UO terrain palette indices.
Uses exact RGB colors from UOL's Terrain.xml.
"""
import os, json
import numpy as np
from PIL import Image
import cv2
from uo_terrain_palette import UO_TERRAIN_PALETTE, REGION_TO_TERRAIN, get_terrain_rgb

ROOT = os.path.dirname(os.path.abspath(__file__))
BASE = os.path.abspath(os.path.join(ROOT, ".."))

EXPORT_DIR = os.path.join(BASE, "exports")
PREVIEW_DIR = os.path.join(BASE, "preview")

REGIONS_IN = os.path.join(EXPORT_DIR, "regions_final.png")
REGIONS_KEY = os.path.join(EXPORT_DIR, "regions_key.json")
TERRAIN_UO = os.path.join(EXPORT_DIR, "terrain_uo.png")
PREVIEW_OUT = os.path.join(PREVIEW_DIR, "45_uo_terrain.png")

print("="*60)
print("Stage 4.5: Mapping Regions to UO Terrain Types")
print("="*60)

# Load regions
regions = cv2.imread(REGIONS_IN)
if regions is None:
    raise FileNotFoundError(f"Missing {REGIONS_IN}")

h, w, _ = regions.shape
print(f"Loaded regions: {w}x{h}")

# Load regions key to get color-to-region mapping
with open(REGIONS_KEY, 'r') as f:
    regions_data = json.load(f)

# Build color to region name mapping
color_to_region = {}
for region_info in regions_data:
    hex_color = region_info["color"].lstrip('#')
    r = int(hex_color[0:2], 16)
    g = int(hex_color[2:4], 16)
    b = int(hex_color[4:6], 16)
    color_to_region[(b, g, r)] = region_info["region"]  # BGR format

print(f"Found {len(color_to_region)} region colors")

# Create output image with UO terrain indices
terrain_uo = np.zeros((h, w), dtype=np.uint8)

# Convert BGR to RGB for easier matching
regions_rgb = cv2.cvtColor(regions, cv2.COLOR_BGR2RGB)

# Map each pixel to UO terrain type
print("Mapping regions to UO terrain types...")
for region_color_bgr, region_name in color_to_region.items():
    # Find pixels of this region
    r, g, b = region_color_bgr[2], region_color_bgr[1], region_color_bgr[0]
    mask = (regions_rgb[:,:,0] == r) & (regions_rgb[:,:,1] == g) & (regions_rgb[:,:,2] == b)

    # Get UO terrain ID for this region
    if region_name in REGION_TO_TERRAIN:
        terrain_id = REGION_TO_TERRAIN[region_name]
    else:
        # Default to grass for unmapped regions
        terrain_id = 1
        print(f"  WARNING: {region_name} not mapped, using Grass (ID=1)")

    terrain_uo[mask] = terrain_id
    pixel_count = np.sum(mask)
    if pixel_count > 0:
        print(f"  {region_name}: UO Terrain ID {terrain_id} ({UO_TERRAIN_PALETTE[terrain_id][0]}) - {pixel_count:,} pixels")

# Handle ocean (black pixels)
ocean_mask = np.all(regions == 0, axis=2)
terrain_uo[ocean_mask] = 5  # Deep Water
print(f"  Ocean: UO Terrain ID 5 (Deep Water) - {np.sum(ocean_mask):,} pixels")

print(f"\nUnique terrain IDs: {sorted(np.unique(terrain_uo))}")

# Save as PNG (will convert to proper BMP in Stage 5)
terrain_uo_img = Image.fromarray(terrain_uo, mode='L')
terrain_uo_img.save(TERRAIN_UO)
print(f"[OK] Saved: {TERRAIN_UO}")

# Create visual preview using UO palette colors
preview = np.zeros((h, w, 3), dtype=np.uint8)
for terrain_id in np.unique(terrain_uo):
    if terrain_id in UO_TERRAIN_PALETTE:
        mask = terrain_uo == terrain_id
        r, g, b = get_terrain_rgb(terrain_id)
        preview[mask] = [b, g, r]  # BGR for OpenCV

cv2.imwrite(PREVIEW_OUT, preview)
print(f"[OK] Preview: {PREVIEW_OUT}")

print("\n" + "="*60)
print("Stage 4.5 Complete!")
print("Regions mapped to UO terrain types with exact palette colors")
print("="*60)
