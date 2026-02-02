"""
Multi extractor and catalog generator.
Scans multi.idx/multi.mul to find candidate house multis and extract entries.
"""

import argparse
import json
import struct
from pathlib import Path
from typing import Dict, List, Optional

from tiledata_reader import TileDataReader, TileFlags


def read_index_entries(idx_path: Path) -> List[Dict]:
    entries = []
    with idx_path.open('rb') as idx:
        index = 0
        while True:
            chunk = idx.read(12)
            if len(chunk) < 12:
                break
            offset = struct.unpack('<I', chunk[:4])[0]
            length = struct.unpack('<I', chunk[4:8])[0]
            extra = struct.unpack('<I', chunk[8:12])[0]
            entries.append({'id': index, 'offset': offset, 'length': length, 'extra': extra})
            index += 1
    return entries


def read_components(mul_file, offset: int, length: int) -> List[Dict]:
    components = []
    if offset == 0xFFFFFFFF or length == 0:
        return components

    mul_file.seek(offset)
    count = length // 12
    for _ in range(count):
        item_id = struct.unpack('<H', mul_file.read(2))[0]
        x = struct.unpack('<h', mul_file.read(2))[0]
        y = struct.unpack('<h', mul_file.read(2))[0]
        z = struct.unpack('<h', mul_file.read(2))[0]
        flags = struct.unpack('<I', mul_file.read(4))[0]
        components.append({'item_id': item_id, 'x': x, 'y': y, 'z': z, 'flags': flags})
    return components


def classify_components(components: List[Dict], tiledata: Optional[TileDataReader]) -> Dict:
    if not tiledata:
        return {'total': len(components)}

    counts = {
        'total': len(components),
        'wall': 0,
        'roof': 0,
        'door': 0,
        'floor': 0,
        'surface': 0
    }

    for comp in components:
        info = tiledata.get_static(comp['item_id'])
        if not info:
            continue
        if info.flags & TileFlags.Wall:
            counts['wall'] += 1
        if info.flags & TileFlags.Roof:
            counts['roof'] += 1
        if info.flags & TileFlags.Door:
            counts['door'] += 1
        if info.flags & TileFlags.Surface:
            counts['surface'] += 1

        if 'floor' in info.name.lower():
            counts['floor'] += 1

    return counts


def scan_multis(idx_path: Path, mul_path: Path, tiledata_path: Optional[Path],
                min_components: int, max_components: int) -> List[Dict]:
    tiledata = TileDataReader(str(tiledata_path)) if tiledata_path and tiledata_path.exists() else None
    entries = read_index_entries(idx_path)
    catalog = []

    with mul_path.open('rb') as mul:
        for entry in entries:
            length = entry['length']
            if entry['offset'] == 0xFFFFFFFF or length == 0:
                continue

            component_count = length // 12
            if component_count < min_components or component_count > max_components:
                continue

            components = read_components(mul, entry['offset'], length)
            stats = classify_components(components, tiledata)

            catalog.append({
                'id': entry['id'],
                'components': component_count,
                'stats': stats
            })

    if tiledata:
        tiledata.close()

    return catalog


def extract_multi(idx_path: Path, mul_path: Path, multi_id: int, output: Path):
    entries = read_index_entries(idx_path)
    if multi_id < 0 or multi_id >= len(entries):
        raise SystemExit(f"Multi {multi_id} is out of range (0-{len(entries)-1}).")

    entry = entries[multi_id]
    if entry['offset'] == 0xFFFFFFFF or entry['length'] == 0:
        raise SystemExit(f"Multi {multi_id} has no data.")

    with mul_path.open('rb') as mul:
        components = read_components(mul, entry['offset'], entry['length'])

    output.parent.mkdir(parents=True, exist_ok=True)
    with output.open('w') as f:
        for comp in components:
            f.write(f"0x{comp['item_id']:X} {comp['x']} {comp['y']} {comp['z']} 0\n")

    print(f"Wrote {output} ({len(components)} components)")


def main():
    parser = argparse.ArgumentParser(description="Scan and extract multi.mul entries")
    parser.add_argument("--mul", type=Path, required=True, help="Path to multi.mul")
    parser.add_argument("--idx", type=Path, required=True, help="Path to multi.idx")
    parser.add_argument("--tiledata", type=Path, help="Path to tiledata.mul for classification")
    parser.add_argument("--catalog", type=Path, help="Output JSON catalog path")
    parser.add_argument("--min-components", type=int, default=50, help="Minimum component count to include")
    parser.add_argument("--max-components", type=int, default=5000, help="Maximum component count to include")
    parser.add_argument("--extract", type=int, help="Extract a single multi id")
    parser.add_argument("--output", type=Path, help="Output path for extracted multi")

    args = parser.parse_args()

    if args.extract is not None:
        output = args.output or Path("outputs") / f"multi_{args.extract:04X}.txt"
        extract_multi(args.idx, args.mul, args.extract, output)
        return

    catalog = scan_multis(args.idx, args.mul, args.tiledata, args.min_components, args.max_components)
    catalog.sort(key=lambda c: c['components'], reverse=True)

    if args.catalog:
        args.catalog.parent.mkdir(parents=True, exist_ok=True)
        with args.catalog.open('w') as f:
            json.dump({'entries': catalog}, f, indent=2)
        print(f"Wrote catalog: {args.catalog} ({len(catalog)} entries)")

    print("Top candidate multis:")
    for entry in catalog[:10]:
        stats = entry['stats']
        print(f"  0x{entry['id']:04X}: comps={entry['components']}, wall={stats.get('wall')}, roof={stats.get('roof')}, door={stats.get('door')}")


if __name__ == "__main__":
    main()
