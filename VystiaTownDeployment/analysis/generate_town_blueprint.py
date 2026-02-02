"""
Convert generated town JSON (statics list) into CentrED blueprint format.
"""

import argparse
import json
from pathlib import Path


def export_blueprint(name: str, statics, output_path: Path) -> Path:
    output_path.parent.mkdir(parents=True, exist_ok=True)

    with output_path.open('w') as f:
        f.write('6 version\n')
        f.write('0 template id\n')
        f.write('0 item version\n')
        f.write(f'{len(statics)} num components\n')

        sorted_statics = sorted(statics, key=lambda s: (s.get('z', 0), s.get('y', 0), s.get('x', 0)))
        for item in sorted_statics:
            tile_id = item.get('tile_id', 0)
            x = item.get('x', 0)
            y = item.get('y', 0)
            z = item.get('z', 0)
            f.write(f"{tile_id} {x} {y} {z} 1\n")

    return output_path


def main():
    parser = argparse.ArgumentParser(description="Convert generated town JSON to CentrED blueprint")
    parser.add_argument("json_file", help="Path to generated town JSON file")
    parser.add_argument("--output", help="Output blueprint path")
    parser.add_argument("--name", help="Override blueprint name")
    parser.add_argument("--blueprints-dir", default=r"D:\UO\centredsharp\Blueprints\Vystia",
                        help="CentrED blueprints directory")
    parser.add_argument("--keep-absolute", action="store_true",
                        help="Keep absolute coordinates instead of centering")
    args = parser.parse_args()

    json_path = Path(args.json_file)
    with json_path.open() as f:
        data = json.load(f)

    statics = data.get('statics', [])
    name = args.name or data.get('name', json_path.stem)

    if not args.keep_absolute and statics:
        bounds = data.get('bounds', {})
        if isinstance(bounds, dict) and all(k in bounds for k in ("x1", "y1", "x2", "y2")):
            center_x = (bounds["x1"] + bounds["x2"]) // 2
            center_y = (bounds["y1"] + bounds["y2"]) // 2
        else:
            xs = [s.get('x', 0) for s in statics]
            ys = [s.get('y', 0) for s in statics]
            center_x = sum(xs) // len(xs)
            center_y = sum(ys) // len(ys)

        for item in statics:
            item['x'] = item.get('x', 0) - center_x
            item['y'] = item.get('y', 0) - center_y

    if args.output:
        output_path = Path(args.output)
    else:
        output_path = Path(args.blueprints_dir) / f"{name}.txt"

    export_blueprint(name, statics, output_path)
    print(f"Wrote blueprint: {output_path}")


if __name__ == "__main__":
    main()
