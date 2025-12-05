"""
Analyze OSI town data to learn building patterns
"""

import json
from pathlib import Path
from collections import defaultdict, Counter

def analyze_town(town_file: Path):
    """Analyze a single town's structure"""
    print(f"\n{'='*60}")
    print(f"ANALYZING: {town_file.stem}")
    print(f"{'='*60}")

    with open(town_file, 'r') as f:
        data = json.load(f)

    statics = data.get('statics', [])

    print(f"Total statics: {len(statics)}")

    # Analyze tile ID distribution
    tile_counts = Counter(s['tile_id'] for s in statics)

    print(f"\nTop 20 most common tiles:")
    for tile_id, count in tile_counts.most_common(20):
        print(f"  Tile 0x{tile_id:04X} ({tile_id}): {count} times ({100*count/len(statics):.1f}%)")

    # Analyze Z-levels
    z_levels = [s['z'] for s in statics]
    print(f"\nZ-level range: {min(z_levels)} to {max(z_levels)}")
    print(f"Average Z: {sum(z_levels)/len(z_levels):.1f}")

    # Find building clusters (tiles at same Z-level in close proximity)
    return tile_counts

def find_building_templates(town_file: Path, output_file: Path):
    """Extract complete building structures from a town"""
    print(f"\n{'='*60}")
    print(f"EXTRACTING BUILDING TEMPLATES: {town_file.stem}")
    print(f"{'='*60}")

    with open(town_file, 'r') as f:
        data = json.load(f)

    statics = data.get('statics', [])

    # Group statics by approximate location (10x10 tile chunks)
    chunks = defaultdict(list)
    for s in statics:
        chunk_x = s['x'] // 10
        chunk_y = s['y'] // 10
        chunks[(chunk_x, chunk_y)].append(s)

    # Find interesting chunks (likely buildings - 5-50 statics)
    interesting_chunks = []
    for chunk_id, chunk_statics in chunks.items():
        if 5 <= len(chunk_statics) <= 50:
            interesting_chunks.append({
                'chunk_id': chunk_id,
                'count': len(chunk_statics),
                'statics': chunk_statics
            })

    # Sort by size
    interesting_chunks.sort(key=lambda x: x['count'], reverse=True)

    print(f"Found {len(interesting_chunks)} interesting building chunks")
    print(f"\nTop 10 largest building structures:")
    for i, chunk in enumerate(interesting_chunks[:10]):
        tile_types = Counter(s['tile_id'] for s in chunk['statics'])
        print(f"\n  Building #{i+1}: {chunk['count']} statics at chunk {chunk['chunk_id']}")
        print(f"    Tile variety: {len(tile_types)} different tile types")
        print(f"    Most common: ", end='')
        for tile_id, count in tile_types.most_common(3):
            print(f"0x{tile_id:04X}({count}x) ", end='')
        print()

    # Save top 20 building templates
    templates = []
    for i, chunk in enumerate(interesting_chunks[:20]):
        # Normalize coordinates relative to chunk min
        chunk_statics = chunk['statics']
        min_x = min(s['x'] for s in chunk_statics)
        min_y = min(s['y'] for s in chunk_statics)

        normalized = []
        for s in chunk_statics:
            normalized.append({
                'tile_id': s['tile_id'],
                'x': s['x'] - min_x,
                'y': s['y'] - min_y,
                'z': s['z'],
                'hue': s.get('hue', 0)
            })

        templates.append({
            'id': i,
            'source_town': town_file.stem,
            'size': len(normalized),
            'statics': normalized
        })

    with open(output_file, 'w') as f:
        json.dump({'templates': templates}, f, indent=2)

    print(f"\nSaved {len(templates)} building templates to {output_file}")
    return templates

if __name__ == '__main__':
    town_data_dir = Path('town_data')

    # Analyze Britain first
    britain = town_data_dir / 'britain.json'
    tile_counts = analyze_town(britain)

    # Analyze other towns
    print(f"\n\n{'='*60}")
    print("COMPARING ALL TOWNS")
    print(f"{'='*60}")

    all_towns = ['britain.json', 'vesper.json', 'moonglow.json', 'yew.json', 'cove.json', 'minoc.json', 'trinsic.json']

    for town_name in all_towns:
        town_file = town_data_dir / town_name
        if town_file.exists():
            with open(town_file, 'r') as f:
                data = json.load(f)
            statics_count = len(data.get('statics', []))
            print(f"{town_name:20s}: {statics_count:5d} statics")

    # Extract building templates from Britain
    templates = find_building_templates(britain, Path('extracted_building_templates.json'))

    print(f"\n{'='*60}")
    print("ANALYSIS COMPLETE")
    print(f"{'='*60}")
    print(f"\nBuilding templates extracted to: extracted_building_templates.json")
    print(f"Use these templates to generate realistic towns!")
