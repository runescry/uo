"""Production town generator using curated housing templates.

This script arranges the curated housing multis (located under
`multi_templates/housing`) into a configurable grid around a specified
center point, optionally adds a road cross, and emits a statics JSON that
can be injected with `place_statics.py`.
"""

from __future__ import annotations

import argparse
import json
import math
import random
import subprocess
from dataclasses import dataclass
from pathlib import Path
from typing import Iterable, List, Tuple

PROJECT_DIR = Path(__file__).resolve().parent
REPO_ROOT = PROJECT_DIR.parent
TEMPLATE_DIR = PROJECT_DIR / "multi_templates" / "housing"
ROAD_TILE_ID = 0x0520  # stone pavers


@dataclass
class HousingTemplate:
    name: str
    path: Path
    bounds: dict
    statics: list
    doors: list
    width: int
    height: int


def load_housing_templates(template_dir: Path = TEMPLATE_DIR) -> List[HousingTemplate]:
    if not template_dir.is_dir():
        raise FileNotFoundError(f"Template directory not found: {template_dir}")

    templates: List[HousingTemplate] = []
    for json_path in sorted(template_dir.glob("*.json")):
        payload = json.loads(json_path.read_text(encoding="utf-8"))
        bounds = payload.get("bounds", {"x1": 0, "y1": 0, "x2": 0, "y2": 0})
        width = bounds["x2"] - bounds["x1"] + 1
        height = bounds["y2"] - bounds["y1"] + 1

        templates.append(
            HousingTemplate(
                name=json_path.stem,
                path=json_path,
                bounds=bounds,
                statics=payload.get("statics", []),
                doors=payload.get("doors", []),
                width=width,
                height=height,
            )
        )

    if not templates:
        raise RuntimeError(f"No templates found in {template_dir}")

    return templates


class ProductionTownBuilder:
    def __init__(
        self,
        center_x: int,
        center_y: int,
        rows: int,
        cols: int,
        padding: int = 10,
        z_adjust: int = 0,
        seed: int | None = None,
    ) -> None:
        self.center_x = center_x
        self.center_y = center_y
        self.rows = rows
        self.cols = cols
        self.padding = padding
        self.z_adjust = z_adjust
        self.random = random.Random(seed)

        self.templates = load_housing_templates()
        self.random.shuffle(self.templates)

        self.statics: list[dict] = []
        self.door_positions: list[dict] = []
        self.occupied_tiles: set[Tuple[int, int]] = set()
        self.placement_extents = None

        self.spacing_x, self.spacing_y = self._compute_spacing()

    def _compute_spacing(self) -> Tuple[int, int]:
        max_width = max(t.width for t in self.templates)
        max_height = max(t.height for t in self.templates)
        spacing_x = max_width + self.padding
        spacing_y = max_height + self.padding
        return spacing_x, spacing_y

    def _iter_grid_positions(self) -> Iterable[Tuple[int, int, HousingTemplate]]:
        cycle_templates = self._template_cycle()

        total_width = (self.cols - 1) * self.spacing_x
        total_height = (self.rows - 1) * self.spacing_y

        left_anchor = self.center_x - total_width / 2
        top_anchor = self.center_y - total_height / 2

        for row in range(self.rows):
            anchor_y = top_anchor + row * self.spacing_y
            for col in range(self.cols):
                anchor_x = left_anchor + col * self.spacing_x
                template = next(cycle_templates)
                yield int(round(anchor_x)), int(round(anchor_y)), template

    def _template_cycle(self) -> Iterable[HousingTemplate]:
        while True:
            for template in self.templates:
                yield template

    def _apply_template(self, anchor_x: int, anchor_y: int, template: HousingTemplate) -> None:
        base_x = anchor_x - template.bounds["x1"]
        base_y = anchor_y - template.bounds["y1"]

        for static in template.statics:
            world_x = base_x + static["x"]
            world_y = base_y + static["y"]
            world_z = static["z"] + self.z_adjust
            hue = static.get("hue", 0)

            self.statics.append(
                {
                    "tile_id": static["tile_id"],
                    "x": world_x,
                    "y": world_y,
                    "z": world_z,
                    "hue": hue,
                }
            )
            self.occupied_tiles.add((world_x, world_y))

        for door in template.doors:
            self.door_positions.append(
                {
                    "tile_id": door["tile_id"],
                    "x": base_x + door["x"],
                    "y": base_y + door["y"],
                    "z": door["z"] + self.z_adjust,
                }
            )

        min_x = base_x + template.bounds["x1"]
        max_x = base_x + template.bounds["x2"]
        min_y = base_y + template.bounds["y1"]
        max_y = base_y + template.bounds["y2"]

        if self.placement_extents is None:
            self.placement_extents = [min_x, max_x, min_y, max_y]
        else:
            self.placement_extents[0] = min(self.placement_extents[0], min_x)
            self.placement_extents[1] = max(self.placement_extents[1], max_x)
            self.placement_extents[2] = min(self.placement_extents[2], min_y)
            self.placement_extents[3] = max(self.placement_extents[3], max_y)

    def _add_road_cross(self, width: int = 5, padding: int = 5) -> None:
        if not self.placement_extents:
            return

        min_x, max_x, min_y, max_y = self.placement_extents
        min_x -= padding
        max_x += padding
        min_y -= padding
        max_y += padding

        half_width = width // 2

        for x in range(min_x, max_x + 1):
            for dy in range(-half_width, half_width + 1):
                y = self.center_y + dy
                if (x, y) in self.occupied_tiles:
                    continue
                self.statics.append(
                    {
                        "tile_id": ROAD_TILE_ID,
                        "x": x,
                        "y": y,
                        "z": 0,
                        "hue": 0,
                    }
                )

        for y in range(min_y, max_y + 1):
            for dx in range(-half_width, half_width + 1):
                x = self.center_x + dx
                if (x, y) in self.occupied_tiles:
                    continue
                self.statics.append(
                    {
                        "tile_id": ROAD_TILE_ID,
                        "x": x,
                        "y": y,
                        "z": 0,
                        "hue": 0,
                    }
                )

    def generate(self) -> dict:
        for anchor_x, anchor_y, template in self._iter_grid_positions():
            self._apply_template(anchor_x, anchor_y, template)

        self._add_road_cross()

        return {
            "name": "ProductionTown",
            "statics": self.statics,
            "doors": self.door_positions,
            "metadata": {
                "center": [self.center_x, self.center_y],
                "rows": self.rows,
                "cols": self.cols,
                "spacing": [self.spacing_x, self.spacing_y],
                "templates_used": len(self.templates),
            },
        }


def parse_args() -> argparse.Namespace:
    parser = argparse.ArgumentParser(description="Generate Vystia production town")
    parser.add_argument("--center-x", type=int, default=3500, help="Town center X coordinate")
    parser.add_argument("--center-y", type=int, default=1700, help="Town center Y coordinate")
    parser.add_argument("--rows", type=int, default=4, help="Number of building rows")
    parser.add_argument("--cols", type=int, default=4, help="Number of building columns")
    parser.add_argument("--padding", type=int, default=12, help="Padding between buildings")
    parser.add_argument("--z-adjust", type=int, default=0, help="Z offset applied to all statics")
    parser.add_argument("--seed", type=int, default=42, help="Random seed for template ordering")
    default_output = PROJECT_DIR / "generated_production_town.json"
    parser.add_argument(
        "--output",
        default=str(default_output),
        help="Path to output JSON",
    )
    parser.add_argument(
        "--place",
        action="store_true",
        help="Immediately inject the generated JSON using place_statics.py",
    )
    default_map_dir = PROJECT_DIR / "temp_map"
    parser.add_argument(
        "--map-dir",
        default=str(default_map_dir),
        help="Map directory (needed when --place is set)",
    )
    return parser.parse_args()


def write_output(payload: dict, output_path: Path) -> None:
    output_path.parent.mkdir(parents=True, exist_ok=True)
    output_path.write_text(json.dumps(payload, indent=2), encoding="utf-8")
    print(f"Wrote {len(payload['statics'])} statics to {output_path}")
    if payload.get("doors"):
        print(f"Detected {len(payload['doors'])} doors (static positions logged)")


def place_statics(output_path: Path, map_dir: Path) -> None:
    place_script = REPO_ROOT / "UOFiddler_tools" / "place_statics.py"
    if not place_script.is_file():
        raise FileNotFoundError(place_script)

    cmd = [
        sys.executable,
        str(place_script),
        "--json",
        str(output_path),
        "--base-x",
        "0",
        "--base-y",
        "0",
        "--map-dir",
        str(map_dir),
    ]
    print(f"Running {' '.join(cmd)}")
    subprocess.run(cmd, check=True)


def main() -> None:
    args = parse_args()

    builder = ProductionTownBuilder(
        center_x=args.center_x,
        center_y=args.center_y,
        rows=args.rows,
        cols=args.cols,
        padding=args.padding,
        z_adjust=args.z_adjust,
        seed=args.seed,
    )

    payload = builder.generate()

    output_path = Path(args.output)
    write_output(payload, output_path)

    if args.place:
        place_statics(output_path, Path(args.map_dir))


if __name__ == "__main__":
    import sys

    try:
        main()
    except Exception as exc:  # pragma: no cover - simple CLI tool reporting
        print(f"ERROR: {exc}")
        raise


