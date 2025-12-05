# Vystia Town Generation Workflow

Complete pipeline for extracting OSI town patterns and generating Vystia settlements.

## Overview

This system learns spatial patterns from existing UO towns (Britain, Trinsic, etc.) and uses them to generate new towns that feel authentic while matching your Vystia world design.

## Phase 1: Data Extraction

### Step 1: Extract OSI Towns

```bash
python uo_town_extractor.py
```

**What it does:**
- Reads `.mul` files from your UO installation
- Extracts all terrain and static placements for 8 major towns
- Saves raw data as JSON files in `town_data/`

**Requirements:**
- Point `UO_PATH` to your UO installation (containing map0.mul, statics0.mul, etc.)
- Or use UO:Renaissance files if targeting that era

**Output:** `town_data/britain.json`, `town_data/trinsic.json`, etc.

### Step 2: Classify Tiles

```bash
python uo_tile_classifier.py
```

**What it does:**
- Categorizes every tile ID (wall, floor, roof, road, etc.)
- Clusters statics into discrete buildings
- Identifies building types (shop, residence, crafting, etc.)

**Customization needed:**
- Update tile ID ranges for your specific server/era
- Add custom tile categories if you have custom art
- Refine building type detection based on your needs

## Phase 2: Pattern Analysis

### Step 3: Analyze Patterns

```bash
python uo_pattern_analyzer.py
```

**What it does:**
- Extracts spatial relationships between buildings
- Analyzes road network patterns (grid vs organic vs radial)
- Identifies building clusters and districts
- Calculates density gradients from town centers
- Discovers placement rules (minimum spacing, road proximity, etc.)

**Output:** `town_patterns.json` containing:
- Building density metrics
- Road network characteristics
- District definitions
- Placement constraints
- Co-location patterns

### Step 4: Review Patterns

Open `town_patterns.json` and review the extracted patterns:

```json
{
  "britain": {
    "density": 3.8,
    "road_grid_type": "grid",
    "building_distances": {
      "smithy": {"stable": 25.3, "bank": 45.7},
      ...
    },
    "placement_rules": [
      {"type": "minimum_spacing", "value": 8.5},
      {"type": "road_proximity", "value": 5},
      ...
    ]
  }
}
```

**Key insights to look for:**
- How far apart are buildings typically placed?
- Which building types cluster together?
- What's the density gradient (dense center, sparse edges)?
- Are roads structured or organic?

## Phase 3: Generation

### Step 5: Generate Vystia Towns

```python
from uo_town_generator import TownGenerator, GenerationConfig

generator = TownGenerator()

# Configure your Vystia town
config = GenerationConfig(
    name="Frosthold",
    target_size="large",        # small/medium/large
    style="britain",            # which pattern to follow
    terrain_bounds=(1000, 1000, 1200, 1200),  # your map coordinates
    coastal=False,
    fortified=True,
    has_castle=False,
    building_density=4.0,       # buildings per 100 tiles
)

town = generator.generate_town(config)
generator.export_to_centred(town, 'frosthold.json')
```

**Generation process:**
1. Selects pattern template based on style
2. Generates road network matching template type
3. Identifies buildable zones (not roads, not edges)
4. Places anchor buildings (banks, town halls, etc.)
5. Fills districts with appropriate building mix
6. Adds decoration (vegetation, furniture, etc.)

### Step 6: Import to CentrED+

1. Load your Vystia map in CentrED+
2. Import generated JSON (you'll need to convert to CentrED+ format)
3. Review and manually adjust as needed

**Manual refinements:**
- Adjust individual building placements
- Swap out building types
- Add region-specific details
- Fine-tune terrain integration

## Customization Guide

### For Each Vystia Region

**Frosthold (frozen north):**
```python
config = GenerationConfig(
    name="Frosthold_Capital",
    target_size="large",
    style="britain",  # Formal, organized
    fortified=True,
    building_density=3.5,
)
```

**Steampunk cities:**
```python
config = GenerationConfig(
    name="Gearford",
    target_size="large", 
    style="vesper",  # Industrial feel
    coastal=True,
    building_density=5.0,  # Dense urban
)
```

**Small settlements:**
```python
config = GenerationConfig(
    name="Hamlet_07",
    target_size="small",
    style="organic",  # Natural growth
    building_density=2.0,
)
```

### Advanced: Custom Building Templates

Instead of random building placement, use actual extracted buildings:

```python
# Extract specific building from Britain
britain_bank = extract_building_from_town(
    'town_data/britain.json',
    center=(1492, 1639),  # Britain bank coordinates
    radius=20
)

# Use it in generation
generator.building_library['bank'] = britain_bank
```

This gives you **real UO buildings** placed according to **learned spatial rules**.

## Iteration Workflow

1. **Generate** → Export to JSON
2. **Import to CentrED+** → Review
3. **Identify issues** (too sparse? roads weird? wrong building types?)
4. **Adjust config** → Regenerate
5. **Repeat** until satisfied

**Common adjustments:**
- Increase/decrease `building_density`
- Change `style` (britain vs vesper vs organic)
- Add constraints (coastal, fortified, etc.)
- Tweak placement rules in the generator code

## Benefits Over Pure Procedural

**Why this works better:**

1. **Learned from proven designs** - OSI towns were hand-crafted by pros
2. **Captures implicit rules** - spacing, relationships, flow
3. **Feels authentic** - because it's based on real UO architecture
4. **Highly controllable** - can tune density, style, constraints
5. **Saves time** - generates 80% of the work, you refine 20%

**Versus your original approach:**
- ❌ Made-up rules that might not work
- ❌ No reference to proven designs
- ❌ Hard to match your hand-drawn map style
- ✅ **This learns from the best examples**

## Next Steps

1. Run extraction on your UO installation
2. Analyze patterns from 5-10 towns
3. Generate your first Vystia settlement
4. Import and review in CentrED+
5. Iterate on config until it matches your vision

Then scale to all your Vystia locations!

## File Structure

```
vystia_generation/
├── uo_town_extractor.py      # Extract from .mul files
├── uo_tile_classifier.py     # Categorize tiles
├── uo_pattern_analyzer.py    # Learn patterns
├── uo_town_generator.py      # Generate new towns
├── town_data/                 # Extracted OSI towns
│   ├── britain.json
│   ├── trinsic.json
│   └── ...
├── town_patterns.json         # Analyzed patterns
└── vystia_towns/              # Generated towns
    ├── frosthold.json
    ├── gearford.json
    └── ...
```

## Troubleshooting

**"Can't read .mul files"**
- Check UO_PATH points to correct directory
- Ensure you have map0.mul, statics0.mul, staidx0.mul
- May need to convert newer .uop files to .mul format

**"Generated towns look wrong"**
- Check which pattern template is being used
- Review extracted pattern data for that template
- Adjust tile ID ranges in classifier
- Increase/decrease density and spacing parameters

**"Buildings overlap roads"**
- Ensure roads are marked in occupied grid
- Increase road proximity constraint
- Adjust buildable zone detection

**"Not enough buildings placed"**
- Increase target_density
- Expand buildable zones (reduce margins)
- Relax placement constraints

Ready to start? Run the extractor first!
