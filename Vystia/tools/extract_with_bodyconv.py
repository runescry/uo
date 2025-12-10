"""
Extract creatures using Bodyconv.def mapping
Maps display body IDs to actual animation IDs
"""

import subprocess
import sys
from pathlib import Path

BODYCONV_FILE = r"C:\DevEnv\GIT\UO\UO Adventures\Client\Data Files\Bodyconv.def"
BODY_LIST_FILE = r"c:\Users\User\Downloads\Monsters to extract.txt"
EXTRACTOR_SCRIPT = Path(__file__).parent / "creature_animation_extractor.py"
OUTPUT_PATH = Path(r"C:\DevEnv\GIT\UO\Vystia\tools\creature_animations")

def parse_bodyconv():
    """Parse Bodyconv.def and create display_id -> actual_id mappings"""
    mapping = {}
    try:
        with open(BODYCONV_FILE, 'r') as f:
            for line in f:
                line = line.strip()
                if not line or line.startswith('#'):
                    continue
                parts = line.split()
                if len(parts) >= 2:
                    try:
                        display_id = int(parts[0])
                        actual_id = int(parts[1])
                        # Store the mapping
                        mapping[display_id] = actual_id if actual_id != -1 else display_id
                    except ValueError:
                        pass
    except:
        pass
    return mapping

def get_animation_id(display_id, bodyconv_map):
    """Get the actual animation ID for a display ID"""
    if display_id in bodyconv_map:
        return bodyconv_map[display_id]
    return display_id

def parse_body_list(filepath):
    """Parse body list"""
    bodies = []
    with open(filepath, 'r') as f:
        for line in f:
            line = line.strip()
            if line:
                try:
                    bodies.append(int(line))
                except:
                    pass
    return sorted(set(bodies))

def main():
    print("=" * 70)
    print("EXTRACTION USING BODYCONV.DEF MAPPINGS")
    print("=" * 70)

    # Load bodyconv mappings
    bodyconv = parse_bodyconv()
    print(f"\nLoaded bodyconv.def with {len(bodyconv)} mappings")

    # Parse body list
    body_ids = parse_body_list(BODY_LIST_FILE)
    print(f"Requested bodies: {len(body_ids)}\n")

    success = 0
    failed = 0

    for i, display_id in enumerate(body_ids, 1):
        # Get the actual animation ID
        anim_id = get_animation_id(display_id, bodyconv)

        if anim_id != display_id:
            print(f"[{i:3d}/{len(body_ids)}] Body {display_id:4d} (maps to anim {anim_id:4d}): ", end="", flush=True)
        else:
            print(f"[{i:3d}/{len(body_ids)}] Body {display_id:4d}: ", end="", flush=True)

        try:
            # Extract using the animation ID
            result = subprocess.run(
                [sys.executable, str(EXTRACTOR_SCRIPT), str(anim_id)],
                capture_output=True,
                text=True,
                encoding='utf-8',
                errors='replace',
                timeout=30
            )

            output_dir = OUTPUT_PATH / f"body_{display_id}"

            # Check if extraction succeeded
            if output_dir.exists() and any(output_dir.iterdir()):
                print("SUCCESS")
                success += 1
            elif f"body_{anim_id}" in str(result.stdout) or result.returncode == 0:
                # Also check if anim_id directory was created (might have different ID)
                anim_dir = OUTPUT_PATH / f"body_{anim_id}"
                if anim_dir.exists() and any(anim_dir.iterdir()) and display_id != anim_id:
                    # Rename it to display_id
                    if output_dir.exists():
                        import shutil
                        shutil.rmtree(output_dir)
                    anim_dir.rename(output_dir)
                    print("SUCCESS (remapped)")
                    success += 1
                else:
                    print("FAILED")
                    failed += 1
            else:
                print("FAILED")
                failed += 1
        except Exception as e:
            print(f"ERROR - {e}")
            failed += 1

    print("\n" + "=" * 70)
    print(f"Success: {success}")
    print(f"Failed: {failed}")
    print(f"Total: {len(body_ids)}")

if __name__ == "__main__":
    main()
