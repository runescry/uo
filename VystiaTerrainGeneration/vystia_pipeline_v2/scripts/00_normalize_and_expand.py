"""
Stage 0: Normalize Inputs and Generate Procedural Ocean Islands
================================================================
Takes 3200x2400 core continent and centers it in 7168x4096 canvas.
Generates random procedural islands in the ocean padding areas.
No resizing - native 7168x4096 from the start (Felucca/Trammel map size).
"""
import os, cv2, numpy as np
from PIL import Image
import random

# ============================================================
# PATH CONFIGURATION
# ============================================================
ROOT = os.path.dirname(os.path.abspath(__file__))
BASE = os.path.abspath(os.path.join(ROOT, ".."))

INPUT_DIR = os.path.join(BASE, "input")
INPUT_NORM_DIR = os.path.join(BASE, "input_norm")
PREVIEW_DIR = os.path.join(BASE, "preview")

for d in [INPUT_NORM_DIR, PREVIEW_DIR]:
    os.makedirs(d, exist_ok=True)

# ============================================================
# CONFIG
# ============================================================
TARGET_SIZE = (7168, 4096)  # UO Felucca/Trammel map size (width, height)
CORE_SIZE = (3200, 2400)    # Current Vystia core continent size

# Input files (original size)
CONTINENTS_IN = os.path.join(INPUT_DIR, "continents.png")
REGIONS_IN = os.path.join(INPUT_DIR, "regions.png") if os.path.exists(os.path.join(INPUT_DIR, "regions.png")) else None
MOUNTAINS_IN = os.path.join(INPUT_DIR, "mountains.png")
BASEMAP_IN = os.path.join(INPUT_DIR, "HvMVAcKMEg7dADyZja2SYQ.jpeg")

# Output files (7168x4096)
CONTINENTS_OUT = os.path.join(INPUT_NORM_DIR, "continents.png")
MOUNTAINS_OUT = os.path.join(INPUT_NORM_DIR, "mountains.png")
BASEMAP_OUT = os.path.join(INPUT_NORM_DIR, "basemap.png")
PREVIEW_OUT = os.path.join(PREVIEW_DIR, "00_expanded_world.png")

# Procedural island generation parameters
ISLAND_SEED = 42
NUM_ISLAND_CLUSTERS = 15  # Number of island cluster zones
ISLANDS_PER_CLUSTER = (3, 8)  # Random range
ISLAND_SIZE_RANGE = (50, 400)  # Island radius range
ISLAND_IRREGULARITY = 0.4  # 0=circle, 1=very irregular

random.seed(ISLAND_SEED)
np.random.seed(ISLAND_SEED)

# ============================================================
# HELPER: Generate procedural island shape
# ============================================================
def generate_island(center_x, center_y, base_radius, irregularity, canvas_shape):
    """
    Generate an irregular island shape using noise-based radial displacement.
    Returns a binary mask of the island.
    """
    h, w = canvas_shape
    mask = np.zeros((h, w), dtype=np.uint8)

    # Generate points around circumference with noise
    num_points = max(8, int(base_radius / 10))
    angles = np.linspace(0, 2 * np.pi, num_points, endpoint=False)

    points = []
    for angle in angles:
        # Add random radial noise
        noise = np.random.uniform(1.0 - irregularity, 1.0 + irregularity)
        r = base_radius * noise
        x = int(center_x + r * np.cos(angle))
        y = int(center_y + r * np.sin(angle))

        # Clamp to canvas
        x = max(0, min(x, w - 1))
        y = max(0, min(y, h - 1))
        points.append([x, y])

    # Fill polygon
    points = np.array(points, dtype=np.int32)
    cv2.fillPoly(mask, [points], 255)

    # Smooth edges slightly
    mask = cv2.GaussianBlur(mask, (5, 5), 0)
    _, mask = cv2.threshold(mask, 128, 255, cv2.THRESH_BINARY)

    return mask

# ============================================================
# STEP 1: Load core continent
# ============================================================
print("Stage 0: Normalizing inputs and generating procedural ocean islands...")
print(f"Target canvas: {TARGET_SIZE[0]}x{TARGET_SIZE[1]}")

continents = cv2.imread(CONTINENTS_IN, cv2.IMREAD_GRAYSCALE)
if continents is None:
    raise FileNotFoundError(f"Missing {CONTINENTS_IN}")

# Validate it's binary
_, continents = cv2.threshold(continents, 128, 255, cv2.THRESH_BINARY)

# ============================================================
# STEP 2: Create 7168x4096 canvas and center core continent
# ============================================================
target_w, target_h = TARGET_SIZE
core_h, core_w = continents.shape

# Calculate centering offsets
offset_x = (target_w - core_w) // 2
offset_y = (target_h - core_h) // 2

print(f"Core continent size: {core_w}x{core_h}")
print(f"Centering at offset: ({offset_x}, {offset_y})")

# Create full-size black canvas (ocean)
continents_full = np.zeros((target_h, target_w), dtype=np.uint8)

# Place core continent in center
continents_full[offset_y:offset_y+core_h, offset_x:offset_x+core_w] = continents

# ============================================================
# STEP 3: Define padding zones for island generation
# ============================================================
# We have 4 padding zones: top, bottom, left, right
# Top: y=[0, offset_y], x=[0, target_w]
# Bottom: y=[offset_y+core_h, target_h], x=[0, target_w]
# Left: y=[offset_y, offset_y+core_h], x=[0, offset_x]
# Right: y=[offset_y, offset_y+core_h], x=[offset_x+core_w, target_w]

padding_zones = [
    ("top", 0, target_w, 0, offset_y),
    ("bottom", 0, target_w, offset_y + core_h, target_h),
    ("left", 0, offset_x, offset_y, offset_y + core_h),
    ("right", offset_x + core_w, target_w, offset_y, offset_y + core_h),
]

print(f"\nGenerating {NUM_ISLAND_CLUSTERS} island clusters in padding zones...")

# ============================================================
# STEP 4: Generate procedural islands
# ============================================================
island_count = 0

for cluster_idx in range(NUM_ISLAND_CLUSTERS):
    # Pick random padding zone
    zone_name, x_min, x_max, y_min, y_max = random.choice(padding_zones)

    # Skip if zone is too small
    if x_max - x_min < 100 or y_max - y_min < 100:
        continue

    # Generate cluster center
    cluster_x = random.randint(x_min + 100, x_max - 100)
    cluster_y = random.randint(y_min + 100, y_max - 100)

    # Generate islands in cluster
    num_islands = random.randint(*ISLANDS_PER_CLUSTER)

    for i in range(num_islands):
        # Offset from cluster center
        offset_range = 200
        island_x = cluster_x + random.randint(-offset_range, offset_range)
        island_y = cluster_y + random.randint(-offset_range, offset_range)

        # Clamp to zone
        island_x = max(x_min + 50, min(island_x, x_max - 50))
        island_y = max(y_min + 50, min(island_y, y_max - 50))

        # Random island size
        radius = random.randint(*ISLAND_SIZE_RANGE)

        # Generate island
        island_mask = generate_island(island_x, island_y, radius, ISLAND_IRREGULARITY, (target_h, target_w))

        # Add to continent map
        continents_full = cv2.bitwise_or(continents_full, island_mask)
        island_count += 1

print(f"Generated {island_count} procedural islands in ocean zones")

# Save normalized continents
cv2.imwrite(CONTINENTS_OUT, continents_full)
print(f"[OK] Saved: {CONTINENTS_OUT}")

# ============================================================
# STEP 5: Expand mountains and basemap to same canvas
# ============================================================
# Mountains: center existing content, fill ocean with black
mountains = cv2.imread(MOUNTAINS_IN)
if mountains is not None:
    mountains = cv2.resize(mountains, (core_w, core_h))
    mountains_full = np.zeros((target_h, target_w, 3), dtype=np.uint8)
    mountains_full[offset_y:offset_y+core_h, offset_x:offset_x+core_w] = mountains
    cv2.imwrite(MOUNTAINS_OUT, mountains_full)
    print(f"[OK] Saved: {MOUNTAINS_OUT}")

# Basemap: center existing, fill ocean with ocean blue
basemap = cv2.imread(BASEMAP_IN)
if basemap is not None:
    basemap = cv2.resize(basemap, (core_w, core_h))
    basemap_full = np.full((target_h, target_w, 3), (139, 69, 19), dtype=np.uint8)  # Ocean color (BGR)
    basemap_full[offset_y:offset_y+core_h, offset_x:offset_x+core_w] = basemap
    cv2.imwrite(BASEMAP_OUT, basemap_full)
    print(f"[OK] Saved: {BASEMAP_OUT}")

# ============================================================
# STEP 6: Create preview
# ============================================================
preview = cv2.cvtColor(continents_full, cv2.COLOR_GRAY2BGR)
# Highlight core continent in green
core_mask = np.zeros((target_h, target_w), dtype=np.uint8)
core_mask[offset_y:offset_y+core_h, offset_x:offset_x+core_w] = 255
green_overlay = np.zeros_like(preview)
green_overlay[:, :] = [0, 255, 0]
preview[core_mask > 0] = cv2.addWeighted(preview[core_mask > 0], 0.6, green_overlay[core_mask > 0], 0.4, 0)

# Highlight islands in cyan
island_mask = cv2.bitwise_and(continents_full, cv2.bitwise_not(core_mask))
cyan_overlay = np.zeros_like(preview)
cyan_overlay[:, :] = [255, 255, 0]
preview[island_mask > 0] = cv2.addWeighted(preview[island_mask > 0], 0.6, cyan_overlay[island_mask > 0], 0.4, 0)

# Draw boundary box around core
cv2.rectangle(preview, (offset_x, offset_y), (offset_x + core_w, offset_y + core_h), (0, 255, 0), 4)

# Add text
cv2.putText(preview, f"Core: {core_w}x{core_h}", (offset_x + 20, offset_y + 40),
            cv2.FONT_HERSHEY_SIMPLEX, 1.5, (255, 255, 255), 3)
cv2.putText(preview, f"Full: {target_w}x{target_h}", (20, 60),
            cv2.FONT_HERSHEY_SIMPLEX, 1.5, (255, 255, 255), 3)
cv2.putText(preview, f"Islands: {island_count}", (20, 120),
            cv2.FONT_HERSHEY_SIMPLEX, 1.5, (255, 255, 255), 3)

cv2.imwrite(PREVIEW_OUT, preview)
print(f"[OK] Preview: {PREVIEW_OUT}")

print("\n" + "="*60)
print("Stage 0 complete!")
print(f"  Output resolution: {target_w}x{target_h}")
print(f"  Core continent: centered at ({offset_x}, {offset_y})")
print(f"  Procedural islands: {island_count} generated")
print("="*60)
