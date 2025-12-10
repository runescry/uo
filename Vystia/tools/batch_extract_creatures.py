"""
Batch Creature Animation Extractor
Reads a list of body IDs (including ranges) and extracts all animations
"""

import os
import sys
import subprocess
from pathlib import Path

# Path to the body ID list file
BODY_LIST_FILE = r"c:\Users\User\Downloads\Monsters to extract.txt"
EXTRACTOR_SCRIPT = Path(__file__).parent / "creature_animation_extractor.py"

def parse_body_list(filepath):
    """Parse body ID list file, expanding ranges"""
    body_ids = []

    with open(filepath, 'r') as f:
        for line in f:
            line = line.strip()
            if not line:
                continue

            # Handle ranges (e.g., "97-120")
            if '-' in line:
                start, end = line.split('-')
                start, end = int(start), int(end)
                body_ids.extend(range(start, end + 1))
            else:
                # Individual body ID
                try:
                    body_ids.append(int(line))
                except ValueError:
                    print(f"Warning: Skipping invalid line: {line}")

    return sorted(set(body_ids))  # Remove duplicates and sort

def main():
    print("=" * 80)
    print("BATCH CREATURE ANIMATION EXTRACTOR")
    print("=" * 80)

    # Parse body list
    print(f"\nReading body list from: {BODY_LIST_FILE}")
    body_ids = parse_body_list(BODY_LIST_FILE)
    print(f"Found {len(body_ids)} body IDs to extract")

    # Auto-confirm if -y flag provided
    auto_confirm = len(sys.argv) > 1 and sys.argv[1] == '-y'

    if not auto_confirm:
        # Ask for confirmation
        response = input(f"\nExtract {len(body_ids)} creatures? This may take a while. (y/n): ")
        if response.lower() != 'y':
            print("Extraction cancelled.")
            return
    else:
        print(f"\nAuto-confirmed: Extracting {len(body_ids)} creatures...")

    # Extract each body
    success_count = 0
    fail_count = 0
    empty_count = 0

    OUTPUT_PATH = Path(r"C:\DevEnv\GIT\UO\Vystia\tools\creature_animations")

    for i, body_id in enumerate(body_ids, 1):
        print(f"\n[{i}/{len(body_ids)}] Extracting body {body_id}...")

        try:
            # Run extractor script
            result = subprocess.run(
                [sys.executable, str(EXTRACTOR_SCRIPT), str(body_id)],
                capture_output=True,
                text=True,
                encoding='utf-8',
                errors='replace'
            )

            # Check if output directory was created (more reliable than return code)
            output_dir = OUTPUT_PATH / f"body_{body_id}"

            if "No animations found" in result.stdout or "WARNING: No animations found" in result.stdout:
                print(f"  -> Body {body_id}: No animations (unused body ID)")
                empty_count += 1
            elif output_dir.exists() and any(output_dir.iterdir()):
                # Directory exists and has files = success
                print(f"  -> Body {body_id}: Success")
                success_count += 1
            else:
                print(f"  -> Body {body_id}: Failed (no output created)")
                fail_count += 1

        except Exception as e:
            print(f"  -> Body {body_id}: Error - {e}")
            fail_count += 1

    # Summary
    print("\n" + "=" * 80)
    print("BATCH EXTRACTION COMPLETE")
    print("=" * 80)
    print(f"Total bodies processed: {len(body_ids)}")
    print(f"  Success: {success_count}")
    print(f"  Empty/Unused: {empty_count}")
    print(f"  Failed: {fail_count}")
    print(f"\nOutput directory: C:\\DevEnv\\GIT\\UO\\Vystia\\tools\\creature_animations\\")

if __name__ == "__main__":
    main()
