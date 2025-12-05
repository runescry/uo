"""Utility to shift town JSON statics to a new coordinate space.

Typical usage when moving an OSI town extract into Vystia coordinates:

    python utilities/shift_town_json.py \
        --input town_data/cove.json \
        --output outputs/cove_shifted.json \
        --target-x 3600 --target-y 1850

You can also supply explicit deltas (for manual offsets) or override the
detected source centre. Bounds, statics, and door entries are shifted in
tandem, and metadata centre fields (if present) are updated.
"""

from __future__ import annotations

import argparse
import json
from pathlib import Path
from typing import Dict, Tuple


def parse_args() -> argparse.Namespace:
    parser = argparse.ArgumentParser(description="Shift town statics JSON")
    parser.add_argument("--input", required=True, help="Path to the source town JSON")
    parser.add_argument("--output", required=True, help="Path for the shifted JSON")

    parser.add_argument(
        "--delta-x",
        type=int,
        help="X offset to apply (mutually exclusive with target/centre options)",
    )
    parser.add_argument(
        "--delta-y",
        type=int,
        help="Y offset to apply (mutually exclusive with target/centre options)",
    )
    parser.add_argument(
        "--source-x",
        type=float,
        help="Override detected source centre X (used with --target-x)",
    )
    parser.add_argument(
        "--source-y",
        type=float,
        help="Override detected source centre Y (used with --target-y)",
    )
    parser.add_argument(
        "--target-x",
        type=float,
        help="Desired centre X after shifting (alternatively specify delta)",
    )
    parser.add_argument(
        "--target-y",
        type=float,
        help="Desired centre Y after shifting (alternatively specify delta)",
    )
    parser.add_argument(
        "--z-adjust",
        type=int,
        default=0,
        help="Optional Z offset to apply to all statics/doors",
    )

    return parser.parse_args()


def load_json(path: Path) -> Dict:
    return json.loads(path.read_text(encoding="utf-8"))


def compute_bounds(statics: list[dict]) -> Tuple[int, int, int, int]:
    xs = [s["x"] for s in statics]
    ys = [s["y"] for s in statics]
    return min(xs), max(xs), min(ys), max(ys)


def compute_centre(statics: list[dict]) -> Tuple[float, float]:
    min_x, max_x, min_y, max_y = compute_bounds(statics)
    centre_x = (min_x + max_x) / 2.0
    centre_y = (min_y + max_y) / 2.0
    return centre_x, centre_y


def determine_shift(args: argparse.Namespace, statics: list[dict]) -> Tuple[int, int]:
    if args.delta_x is not None or args.delta_y is not None:
        if args.delta_x is None or args.delta_y is None:
            raise SystemExit("Both --delta-x and --delta-y must be provided together")
        if args.target_x is not None or args.target_y is not None:
            raise SystemExit("Specify either deltas OR target coordinates, not both")
        return args.delta_x, args.delta_y

    centre_x, centre_y = compute_centre(statics)
    source_x = args.source_x if args.source_x is not None else centre_x
    source_y = args.source_y if args.source_y is not None else centre_y

    if args.target_x is None or args.target_y is None:
        raise SystemExit("Provide --target-x and --target-y (or explicit deltas)")

    delta_x = int(round(args.target_x - source_x))
    delta_y = int(round(args.target_y - source_y))

    return delta_x, delta_y


def shift_statics(statics: list[dict], delta_x: int, delta_y: int, z_adjust: int) -> None:
    for entry in statics:
        entry["x"] += delta_x
        entry["y"] += delta_y
        entry["z"] += z_adjust


def shift_bounds(data: Dict, delta_x: int, delta_y: int) -> None:
    bounds = data.get("bounds")
    if not isinstance(bounds, dict):
        return

    for key in ("x1", "x2"):
        if key in bounds:
            bounds[key] += delta_x
    for key in ("y1", "y2"):
        if key in bounds:
            bounds[key] += delta_y


def shift_metadata_centre(data: Dict, target_x: float, target_y: float) -> None:
    metadata = data.get("metadata")
    if isinstance(metadata, dict) and "center" in metadata:
        metadata["center"] = [target_x, target_y]


def main() -> None:
    args = parse_args()

    input_path = Path(args.input)
    output_path = Path(args.output)

    data = load_json(input_path)
    statics = data.get("statics")
    if not statics:
        raise SystemExit("Input JSON does not contain any statics")

    delta_x, delta_y = determine_shift(args, statics)

    shift_statics(statics, delta_x, delta_y, args.z_adjust)

    if "doors" in data and isinstance(data["doors"], list):
        shift_statics(data["doors"], delta_x, delta_y, args.z_adjust)

    shift_bounds(data, delta_x, delta_y)

    if args.target_x is not None and args.target_y is not None:
        shift_metadata_centre(data, args.target_x, args.target_y)

    output_path.parent.mkdir(parents=True, exist_ok=True)
    output_path.write_text(json.dumps(data, indent=2), encoding="utf-8")

    print(f"Shifted town written to {output_path}")
    print(f"Applied delta: (dx={delta_x}, dy={delta_y}, dz={args.z_adjust})")


if __name__ == "__main__":  # pragma: no cover - CLI entry point
    main()


