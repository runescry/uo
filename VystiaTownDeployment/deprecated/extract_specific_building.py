"""
Extract a specific building from Britain by known coordinates
Britain Castle coordinates: approximately (1520, 1650) in OSI map
Let's grab a small house instead - easier to verify
"""

import json
from collections import Counter

# Load Britain data
with open('town_data/britain.json', 'r') as f:
    britain = json.load(f)

statics = britain['statics']

print(f"Total Britain statics: {len(statics)}")
print(f"Bounds: {britain['bounds']}")

# Let's look at a specific small area and see ALL tiles
# Britain coordinates (1400-1750, 1500-1800)
# Let's pick a 20x20 area in the middle: (1550-1570, 1650-1670)

test_x1, test_x2 = 1550, 1570
test_y1, test_y2 = 1650, 1670

area_statics = [s for s in statics
                if test_x1 <= s['x'] <= test_x2
                and test_y1 <= s['y'] <= test_y2]

print(f"\n{'='*60}")
print(f"AREA: ({test_x1}, {test_y1}) to ({test_x2}, {test_y2})")
print(f"{'='*60}")
print(f"Statics in this area: {len(area_statics)}")

if len(area_statics) > 0:
    # Show ALL statics with coordinates and tile IDs
    print(f"\nAll {len(area_statics)} statics:")

    # Sort by Y then X for readability
    area_statics.sort(key=lambda s: (s['y'], s['x']))

    for s in area_statics:
        print(f"  ({s['x']:4d}, {s['y']:4d}, Z={s['z']:3d}) - Tile 0x{s['tile_id']:04X} ({s['tile_id']:5d})")

    # Tile frequency
    tile_counts = Counter(s['tile_id'] for s in area_statics)
    print(f"\nTile frequency:")
    for tile_id, count in tile_counts.most_common(10):
        print(f"  0x{tile_id:04X} ({tile_id}): {count}x")

    # Save this specific area
    with open('britain_test_area.json', 'w') as f:
        json.dump({
            'bounds': {'x1': test_x1, 'y1': test_y1, 'x2': test_x2, 'y2': test_y2},
            'statics': area_statics
        }, f, indent=2)

    print(f"\nSaved to: britain_test_area.json")

# Let's also try the Britain bank area (well-known location)
# Britain bank is around (1430, 1690)
bank_x, bank_y = 1430, 1690
bank_radius = 15

bank_statics = [s for s in statics
                if abs(s['x'] - bank_x) <= bank_radius
                and abs(s['y'] - bank_y) <= bank_radius]

print(f"\n{'='*60}")
print(f"BRITAIN BANK AREA: ({bank_x}, {bank_y}) +/- {bank_radius}")
print(f"{'='*60}")
print(f"Statics near bank: {len(bank_statics)}")

if len(bank_statics) > 0:
    # Show first 50
    bank_statics.sort(key=lambda s: (s['y'], s['x']))
    print(f"\nFirst 50 statics near bank:")
    for s in bank_statics[:50]:
        print(f"  ({s['x']:4d}, {s['y']:4d}, Z={s['z']:3d}) - Tile 0x{s['tile_id']:04X}")

    # Save bank area
    with open('britain_bank_area.json', 'w') as f:
        json.dump({
            'center': {'x': bank_x, 'y': bank_y},
            'radius': bank_radius,
            'statics': bank_statics
        }, f, indent=2)

    print(f"\nSaved to: britain_bank_area.json")
