"""
Stage 0.5: Map Source Image Colors to UO Terrain Types
=======================================================
Analyzes the source map's colors and automatically assigns UO terrain types
based on color similarity (e.g., yellow→sand, green→forest, blue→water).
"""
import os, sys
import numpy as np
from PIL import Image
import cv2
from sklearn.cluster import KMeans

# Import UO terrain palette
sys.path.insert(0, os.path.dirname(__file__))
from uo_terrain_palette import UO_TERRAIN_PALETTE, get_terrain_rgb

# ============================================================
# PATH CONFIGURATION
# ============================================================
ROOT = os.path.dirname(os.path.abspath(__file__))
BASE = os.path.abspath(os.path.join(ROOT, ".."))

INPUT_DIR = os.path.join(BASE, "input")
EXPORT_DIR = os.path.join(BASE, "exports")
PREVIEW_DIR = os.path.join(BASE, "preview")

for d in [EXPORT_DIR, PREVIEW_DIR]:
    os.makedirs(d, exist_ok=True)

SOURCE_IMAGE = os.path.join(INPUT_DIR, "HvMVAcKMEg7dADyZja2SYQ.jpeg")
OUTPUT_TERRAIN = os.path.join(EXPORT_DIR, "terrain_from_source.png")
OUTPUT_PREVIEW = os.path.join(PREVIEW_DIR, "05_color_mapped_terrain.png")
COLOR_MAP_REPORT = os.path.join(EXPORT_DIR, "color_mapping_report.txt")

# ============================================================
# COLOR TO TERRAIN RULES
# ============================================================
def rgb_distance(c1, c2):
    """Calculate Euclidean distance between two RGB colors."""
    return np.sqrt(sum((a - b) ** 2 for a, b in zip(c1, c2)))

def find_closest_terrain(rgb_color, exclude_water=False):
    """
    Find the closest matching UO terrain type for a given RGB color.
    Uses Euclidean distance in RGB color space.
    """
    best_terrain_id = 1  # Default to grass
    best_distance = float('inf')

    for terrain_id, (name, r, g, b, _, _) in UO_TERRAIN_PALETTE.items():
        # Skip water types if analyzing land
        if exclude_water and terrain_id in [5, 6, 9]:
            continue

        terrain_rgb = (r, g, b)
        distance = rgb_distance(rgb_color, terrain_rgb)

        if distance < best_distance:
            best_distance = distance
            best_terrain_id = terrain_id

    return best_terrain_id

def classify_color(rgb):
    """
    Classify a color into terrain type using heuristics.

    Color heuristics:
    - Blue (high B, low R/G) → Water
    - White/Light gray (high R=G=B) → Snow
    - Dark gray/brown (low saturation, medium value) → Rock/Mountains
    - Green (high G, low R/B) → Forest/Grass
    - Yellow/Tan (high R+G, low B) → Sand/Desert
    - Dark green/brown → Swamp/Jungle
    """
    r, g, b = rgb

    # Calculate HSV-like values for classification
    max_val = max(r, g, b)
    min_val = min(r, g, b)
    saturation = 0 if max_val == 0 else (max_val - min_val) / max_val

    # Water: Blue dominant
    if b > r and b > g and b > 100:
        if b > 150:
            return 5  # Deep Water
        else:
            return 6  # Shallow Water

    # Snow: High brightness, low saturation
    if r > 200 and g > 200 and b > 200:
        return 12  # Snow

    # Rock/Mountains: Gray/brown, low saturation
    if saturation < 0.3 and max_val > 60 and max_val < 180:
        return 18  # Rock

    # Sand/Desert: Yellow/tan (high R+G, low B)
    if r > 150 and g > 120 and b < 120:
        return 4  # Sand

    # Green terrain classification (more nuanced)
    # Check if this is a green/brownish color
    if g >= r and g >= b:
        # Vibrant green (high saturation, high green) → Forest
        if saturation > 0.4 and g > 100:
            return 11  # Forest (vibrant green)

        # Light/pale green (high brightness) → Grass
        if max_val > 140 and g > r + 20:
            return 1  # Grass (light green)

        # Yellowish-green (high R and G, similar values) → Grass
        if abs(r - g) < 40 and r > 120 and g > 120 and b < 110:
            return 1  # Grass (yellowish-green)

    # Brownish-green (green slightly dominant but low saturation) → Swamp
    if g > r - 20 and g > b and saturation < 0.35 and max_val < 160 and max_val > 60:
        return 41  # Swamp (muddy brown-green)

    # Lava: Red dominant
    if r > 180 and r > g * 2 and r > b * 2:
        return 13  # Lava

    # Default: Use closest match from palette
    return find_closest_terrain(rgb, exclude_water=False)

print("="*60)
print("Stage 0.5: Mapping Source Colors to UO Terrain Types")
print("="*60)

# ============================================================
# STEP 1: Load and analyze source image
# ============================================================
print("\nStep 1: Loading source image...")
source = Image.open(SOURCE_IMAGE)
source_rgb = source.convert("RGB")
source_array = np.array(source_rgb)
h, w, _ = source_array.shape

print(f"  Source dimensions: {w}x{h}")

# ============================================================
# STEP 2: Detect textured/forest areas using variance
# ============================================================
print("\nStep 2: Detecting textured areas (forests)...")

# Convert to grayscale for texture analysis
gray = cv2.cvtColor(source_array, cv2.COLOR_RGB2GRAY)

# Calculate local variance using a sliding window
# High variance = textured (trees/bumps) = Forest
# Low variance = smooth = Other terrain
kernel_size = 15
variance = cv2.blur(gray.astype(np.float32)**2, (kernel_size, kernel_size)) - \
           cv2.blur(gray.astype(np.float32), (kernel_size, kernel_size))**2

# Threshold to find textured areas
variance_threshold = 200  # Adjust based on testing
forest_texture_mask = variance > variance_threshold

print(f"  Found {np.sum(forest_texture_mask):,} textured pixels (potential forests)")

# ============================================================
# STEP 3: Extract dominant colors using K-means clustering
# ============================================================
print("\nStep 3: Analyzing dominant colors...")

# Reshape for clustering
pixels = source_array.reshape(-1, 3)

# Use K-means to find dominant colors (k=20 clusters)
n_colors = 20
print(f"  Clustering into {n_colors} dominant colors...")
kmeans = KMeans(n_clusters=n_colors, random_state=42, n_init=10)
kmeans.fit(pixels)

# Get cluster centers (dominant colors)
dominant_colors = kmeans.cluster_centers_.astype(int)
labels = kmeans.labels_
label_counts = np.bincount(labels)

print(f"  Found {n_colors} dominant color clusters")

# ============================================================
# STEP 3: Map each dominant color to UO terrain type
# ============================================================
print("\nStep 3: Mapping colors to UO terrain types...")

color_to_terrain = {}
report_lines = []
report_lines.append("COLOR MAPPING REPORT")
report_lines.append("=" * 60)
report_lines.append("")

for i, color in enumerate(dominant_colors):
    pixel_count = label_counts[i]
    percentage = (pixel_count / len(pixels)) * 100

    # Classify this color
    terrain_id = classify_color(tuple(color))
    terrain_name = UO_TERRAIN_PALETTE[terrain_id][0]
    terrain_rgb = get_terrain_rgb(terrain_id)

    color_to_terrain[i] = terrain_id

    report_line = f"Cluster {i:2d}: RGB{tuple(color)} ({percentage:5.2f}%) -> ID {terrain_id:2d} {terrain_name:20s} RGB{terrain_rgb}"
    print(f"  {report_line}")
    report_lines.append(report_line)

# ============================================================
# STEP 4: Create terrain map
# ============================================================
print("\nStep 4: Generating terrain map...")

# Map each pixel to its terrain ID
terrain_map = np.zeros((h, w), dtype=np.uint8)
labels_2d = labels.reshape(h, w)

for cluster_id, terrain_id in color_to_terrain.items():
    mask = labels_2d == cluster_id
    terrain_map[mask] = terrain_id

# CRITICAL: Override with Forest wherever texture indicates it
# Any textured area that isn't water/ocean should be Forest
print(f"  Applying texture-based forest override...")
ocean_mask = (terrain_map == 5) | (terrain_map == 6)  # Don't override water
forest_override_mask = forest_texture_mask & ~ocean_mask
terrain_map[forest_override_mask] = 11  # Force to Forest

forest_pixels = np.sum(terrain_map == 11)
print(f"  Forest pixels after texture override: {forest_pixels:,}")

# Count terrain distribution
unique_terrains, counts = np.unique(terrain_map, return_counts=True)
report_lines.append("")
report_lines.append("TERRAIN DISTRIBUTION")
report_lines.append("=" * 60)

print("\n  Terrain distribution:")
for terrain_id, count in zip(unique_terrains, counts):
    terrain_name = UO_TERRAIN_PALETTE[terrain_id][0]
    percentage = (count / (h * w)) * 100
    print(f"    ID {terrain_id:2d} {terrain_name:20s}: {count:8,} pixels ({percentage:5.2f}%)")
    report_lines.append(f"ID {terrain_id:2d} {terrain_name:20s}: {count:8,} pixels ({percentage:5.2f}%)")

# Save terrain map
terrain_img = Image.fromarray(terrain_map, mode='L')
terrain_img.save(OUTPUT_TERRAIN)
print(f"\n[OK] Saved: {OUTPUT_TERRAIN}")

# ============================================================
# STEP 5: Create preview with UO colors
# ============================================================
print("\nStep 5: Generating preview...")

preview = np.zeros((h, w, 3), dtype=np.uint8)
for terrain_id in unique_terrains:
    mask = terrain_map == terrain_id
    r, g, b = get_terrain_rgb(terrain_id)
    preview[mask] = [b, g, r]  # BGR for OpenCV

cv2.imwrite(OUTPUT_PREVIEW, preview)
print(f"[OK] Preview: {OUTPUT_PREVIEW}")

# ============================================================
# STEP 6: Save report
# ============================================================
with open(COLOR_MAP_REPORT, "w") as f:
    f.write("\n".join(report_lines))
print(f"[OK] Report: {COLOR_MAP_REPORT}")

print("\n" + "="*60)
print("Stage 0.5 Complete!")
print("="*60)
print(f"Source image colors mapped to {len(unique_terrains)} UO terrain types")
print("Next: Use terrain_from_source.png as input to the pipeline")
print("="*60)
