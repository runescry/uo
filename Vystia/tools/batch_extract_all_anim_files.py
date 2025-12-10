"""
Batch Creature Animation Extractor - Enhanced for ALL anim files
Reads body list and extracts from anim.mul through anim5.mul
"""

import os
import sys
import subprocess
from pathlib import Path

# Path to the body ID list file
BODY_LIST_FILE = r"c:\Users\User\Downloads\Monsters to extract.txt"
EXTRACTOR_SCRIPT = Path(__file__).parent / "creature_animation_extractor.py"
OUTPUT_PATH = Path(r"C:\DevEnv\GIT\UO\Vystia\tools\creature_animations")

def parse_body_list(filepath):
    """Parse body ID list file"""
    body_ids = []
    with open(filepath, 'r') as f:
        for line in f:
            line = line.strip()
            if not line:
                continue
            if '-' in line:
                start, end = line.split('-')
                start, end = int(start), int(end)
                body_ids.extend(range(start, end + 1))
            else:
                try:
                    body_ids.append(int(line))
                except ValueError:
                    pass
    return sorted(set(body_ids))

def main():
    print("=" * 80)
    print("BATCH CREATURE ANIMATION EXTRACTOR - ALL ANIM FILES")
    print("=" * 80)

    # Parse body list
    print(f"\nReading body list from: {BODY_LIST_FILE}")
    body_ids = parse_body_list(BODY_LIST_FILE)
    print(f"Found {len(body_ids)} body IDs to extract\n")

    # Auto-confirm
    auto_confirm = len(sys.argv) > 1 and sys.argv[1] == '-y'
    if not auto_confirm:
        response = input(f"Extract {len(body_ids)} creatures? (y/n): ")
        if response.lower() != 'y':
            print("Extraction cancelled.")
            return
    else:
        print(f"Auto-confirmed: Extracting {len(body_ids)} creatures...\n")

    # Extract each body
    success_count = 0
    fail_count = 0
    results = {}

    for i, body_id in enumerate(body_ids, 1):
        try:
            # Run extractor - it handles all anim files internally
            result = subprocess.run(
                [sys.executable, str(EXTRACTOR_SCRIPT), str(body_id)],
                capture_output=True,
                text=True,
                encoding='utf-8',
                errors='replace',
                timeout=30
            )

            # Check if output directory was created
            output_dir = OUTPUT_PATH / f"body_{body_id}"
            if output_dir.exists() and any(output_dir.iterdir()):
                print(f"[{i:3d}/{len(body_ids)}] Body {body_id:3d}: SUCCESS")
                success_count += 1
                results[body_id] = "SUCCESS"
            else:
                print(f"[{i:3d}/{len(body_ids)}] Body {body_id:3d}: FAILED")
                fail_count += 1
                results[body_id] = "FAILED"

        except subprocess.TimeoutExpired:
            print(f"[{i:3d}/{len(body_ids)}] Body {body_id:3d}: TIMEOUT")
            fail_count += 1
            results[body_id] = "TIMEOUT"
        except Exception as e:
            print(f"[{i:3d}/{len(body_ids)}] Body {body_id:3d}: ERROR - {e}")
            fail_count += 1
            results[body_id] = "ERROR"

    # Summary
    print("\n" + "=" * 80)
    print("EXTRACTION COMPLETE")
    print("=" * 80)
    print(f"Total bodies: {len(body_ids)}")
    print(f"  Success: {success_count}")
    print(f"  Failed: {fail_count}")
    print(f"\nOutput directory: {OUTPUT_PATH}")

    # Show successful bodies
    successful = [b for b, r in results.items() if r == "SUCCESS"]
    if successful:
        print(f"\nSuccessfully extracted {len(successful)} bodies:")
        print(f"  {successful}")

if __name__ == "__main__":
    main()
