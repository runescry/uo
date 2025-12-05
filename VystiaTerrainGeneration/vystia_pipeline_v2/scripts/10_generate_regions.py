"""
Stage 1: Generate Voronoi Region Segmentation
==============================================
Uses lore-aligned anchored seeds and Voronoi segmentation.
Works natively with 7168x4096 normalized continents.
"""
import os, cv2, json, random, numpy as np

ROOT = os.path.dirname(os.path.abspath(__file__))
BASE = os.path.abspath(os.path.join(ROOT, ".."))
INPUT_NORM_DIR = os.path.join(BASE, "input_norm")
EXPORT_DIR = os.path.join(BASE, "exports")
PREVIEW_DIR = os.path.join(BASE, "preview")

for d in [EXPORT_DIR, PREVIEW_DIR]:
    os.makedirs(d, exist_ok=True)

CONTINENT_MASK = os.path.join(INPUT_NORM_DIR, "continents.png")
BASE_MAP = os.path.join(INPUT_NORM_DIR, "basemap.png")
OUTPUT_FINAL = os.path.join(EXPORT_DIR, "regions_final.png")
OUTPUT_PREVIEW = os.path.join(PREVIEW_DIR, "10_regions_preview.png")
OUTPUT_KEY = os.path.join(EXPORT_DIR, "regions_key.json")

random.seed(42)
np.random.seed(42)

# ============================================================
# LORE REGION COLOURS (LOCKED)
# ============================================================
REGION_COLOURS = {
    "Frosthold": "#B3D9FF",
    "Winterguard": "#6E93B0",
    "Verdantpeak": "#3E8B5E",
    "Skyreach Peak": "#9A7B4F",
    "Ironclad Empire": "#A0522D",
    "Radiant Plains": "#C7B87B",
    "Hollow Forests": "#556B2F",
    "Mystic Canyons": "#B8860B",
    "Deepforge": "#A0522D",
    "Emberlands": "#D2691E",
    "Whispering Sands": "#EEDC82",
    "Blazing Frontier": "#E9967A",
    "Golden Steppe": "#DAA520",
    "Shadowfen": "#2E8B57",
    "Wilderlands": "#228B22",
    "Forgotten Depths": "#483D8B",
    "Verdant Isles": "#66CDAA",
    "Glimmering Archipelago": "#87CEFA",
    "Eternal Twilight": "#4B0082",
    "Crystal Barrens": "#708090",
    "Sunken Isles": "#C2B280"
}

# ============================================================
# ANCHORED SEED COORDINATES (based on 3200x2400 core)
# These will be offset to account for centering in 7168x4096
# ============================================================
CORE_SIZE = (3200, 2400)
TARGET_SIZE = (7168, 4096)

# Original anchors for 3200x2400 core
ANCHOR_COORDS_CORE = {
    "Frosthold": (250, 150),
    "Winterguard": (400, 250),
    "Verdantpeak": (600, 200),
    "Skyreach Peak": (1050, 130),
    "Ironclad Empire": (800, 400),
    "Radiant Plains": (900, 500),
    "Hollow Forests": (700, 550),
    "Mystic Canyons": (820, 650),
    "Deepforge": (1050, 600),
    "Emberlands": (1000, 700),
    "Whispering Sands": (1050, 850),
    "Blazing Frontier": (950, 800),
    "Golden Steppe": (700, 850),
    "Shadowfen": (250, 500),
    "Wilderlands": (400, 650),
    "Forgotten Depths": (450, 750),
    "Verdant Isles": (200, 300),
    "Glimmering Archipelago": (1150, 900),
    "Eternal Twilight": (1100, 250),
    "Crystal Barrens": (950, 200),
    "Sunken Isles": (500, 950)
}

# Calculate offset for centering
offset_x = (TARGET_SIZE[0] - CORE_SIZE[0]) // 2
offset_y = (TARGET_SIZE[1] - CORE_SIZE[1]) // 2

print(f"Stage 1: Generating regions with Voronoi segmentation...")
print(f"Canvas: {TARGET_SIZE[0]}x{TARGET_SIZE[1]}")
print(f"Core offset: ({offset_x}, {offset_y})")

# Offset anchors to match centered core continent
ANCHOR_COORDS = {
    name: (x + offset_x, y + offset_y)
    for name, (x, y) in ANCHOR_COORDS_CORE.items()
}

# ============================================================
# STEP 1: Load continent mask
# ============================================================
mask_img = cv2.imread(CONTINENT_MASK, cv2.IMREAD_GRAYSCALE)
if mask_img is None:
    raise FileNotFoundError(f"Missing {CONTINENT_MASK}")

_, land_mask = cv2.threshold(mask_img, 128, 255, cv2.THRESH_BINARY)
h, w = mask_img.shape
print(f"Loaded continents: {w}x{h}")

region_map = np.zeros((h, w, 3), dtype=np.uint8)

# ============================================================
# STEP 2: Clamp seeds to nearest land pixels
# ============================================================
def nearest_land(x, y, mask):
    """Find nearest land pixel to given coordinates."""
    if mask[y, x] > 0:
        return x, y
    yy, xx = np.where(mask > 0)
    idx = np.argmin((xx - x) ** 2 + (yy - y) ** 2)
    return int(xx[idx]), int(yy[idx])

seeds = []
for name, (x, y) in ANCHOR_COORDS.items():
    x, y = max(0, min(x, w - 1)), max(0, min(y, h - 1))
    lx, ly = nearest_land(x, y, land_mask)
    seeds.append((name, (lx, ly)))
    print(f"  {name}: ({lx}, {ly})")

# ============================================================
# STEP 3: Voronoi segmentation (memory-efficient chunked version)
# ============================================================
seed_points = np.array([p for _, p in seeds])
region_names = [n for n, _ in seeds]
region_colors_bgr = [tuple(int(REGION_COLOURS[name].lstrip('#')[i:i+2], 16)
                           for i in (4, 2, 0)) for name in region_names]

print("Generating Voronoi segmentation (chunked for memory efficiency)...")
region_indices = np.zeros((h, w), dtype=np.uint8)

# Process in horizontal chunks to avoid memory overflow
CHUNK_SIZE = 512
for chunk_start in range(0, h, CHUNK_SIZE):
    chunk_end = min(chunk_start + CHUNK_SIZE, h)
    chunk_h = chunk_end - chunk_start

    # Create pixel grid for this chunk
    pixel_y, pixel_x = np.indices((chunk_h, w))
    pixel_y += chunk_start  # Offset to actual y position
    pixels = np.stack((pixel_x, pixel_y), axis=-1).reshape(-1, 2)

    # Calculate distances to all seeds
    distances = np.linalg.norm(pixels[:, None, :] - seed_points[None, :, :], axis=2)
    nearest_seed_idx = np.argmin(distances, axis=1)
    region_indices[chunk_start:chunk_end, :] = nearest_seed_idx.reshape(chunk_h, w)

    if (chunk_start // CHUNK_SIZE) % 2 == 0:
        print(f"  Processed rows {chunk_start}-{chunk_end}/{h}")

for i, color in enumerate(region_colors_bgr):
    region_mask = (region_indices == i) & (land_mask > 0)
    region_map[region_mask] = color

cv2.imwrite(OUTPUT_FINAL, region_map)
print(f"[OK] Saved: {OUTPUT_FINAL}")

# ============================================================
# STEP 4: Preview overlay
# ============================================================
try:
    base_map = cv2.imread(BASE_MAP)
    if base_map is not None:
        blended = cv2.addWeighted(base_map, 0.45, region_map, 0.65, 0)
        for i, (name, (x, y)) in enumerate(seeds):
            cv2.putText(blended, name, (x - 40, y),
                        cv2.FONT_HERSHEY_SIMPLEX, 0.4, (255, 255, 255), 1, cv2.LINE_AA)
        cv2.imwrite(OUTPUT_PREVIEW, blended)
        print(f"[OK] Preview: {OUTPUT_PREVIEW}")
except Exception as e:
    print(f"Skipping preview: {e}")

# ============================================================
# STEP 5: Region Key JSON
# ============================================================
region_key = []
for i, (name, (x, y)) in enumerate(seeds):
    region_key.append({
        "region": name,
        "color": REGION_COLOURS[name],
        "centroid": [int(x), int(y)],
        "anchored": True
    })

with open(OUTPUT_KEY, "w") as f:
    json.dump(region_key, f, indent=2)
print(f"[OK] Saved: {OUTPUT_KEY}")

print("\n" + "="*60)
print("Stage 1 complete - lore-aligned Voronoi regions generated")
print("="*60)
