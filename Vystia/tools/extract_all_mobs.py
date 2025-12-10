"""
Extract ALL mob animations (bodies 0-399) from all anim files
Excludes equipment/human bodies (400+)
"""

import os
import sys
import subprocess
from pathlib import Path

EXTRACTOR_SCRIPT = Path(__file__).parent / "creature_animation_extractor.py"
OUTPUT_PATH = Path(r"C:\DevEnv\GIT\UO\Vystia\tools\creature_animations")

# All bodies found across all anim files (from diagnostic)
ALL_BODIES = {
    'anim.mul': [1, 2, 3, 4, 5, 6, 7, 8, 9, 12, 13, 14, 15, 16, 17, 18, 21, 22, 24, 26, 28, 29, 30, 31, 33, 35, 36, 39, 41, 42, 44, 45, 47, 48, 50, 51, 52, 53, 54, 56, 57, 58, 59, 60, 61, 70, 71, 72, 75, 80, 81, 85, 86, 87, 150, 151, 200, 201, 202, 203, 204, 205, 206, 207, 208, 209, 210, 211, 212, 213, 214, 215, 216, 217, 218, 219, 220, 221, 223, 225, 226, 228, 231, 232, 233, 234, 237, 238, 290, 291, 292],
    'anim2.mul': [1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 40, 41, 42, 43, 44, 45, 46, 47, 48, 49, 50, 51, 52, 53, 54, 55, 56, 57, 59, 60, 61, 62, 63, 64, 65, 66, 67, 69, 70, 71, 72, 73, 74, 75, 76, 77, 122, 200, 201, 202, 203, 204, 205, 206],
    'anim3.mul': [369, 370, 371, 372, 373, 374, 375, 376, 377, 378, 379, 380, 381, 382, 383, 384, 385, 386, 387, 388, 389, 390, 391, 392, 393, 394, 395, 396, 397, 398, 399],
    'anim4.mul': [1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 40, 41, 42, 43, 44, 45, 46, 47, 48, 49, 50, 51, 52, 53, 54, 55, 56, 57, 58, 59, 60, 61, 62, 63, 64, 65, 66, 67, 68, 69, 70, 71, 72, 73, 74, 75, 77, 78, 79, 80, 81, 82, 83, 84, 85, 201, 202, 203, 204, 205, 206, 207, 208, 209, 210, 211, 212, 213, 214, 215, 216, 217, 218, 219, 220, 221, 222, 223, 224, 225, 226, 227, 228, 229],
    'anim5.mul': [1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32, 33, 35, 36, 37, 38, 39, 40, 41, 42, 43, 44, 45, 46, 47, 48, 49, 50, 51, 52, 53, 54, 55, 56, 57, 58, 59, 60, 61, 62, 63, 64, 65, 66, 67, 68, 69, 70, 71, 72, 73, 74, 75, 76, 101, 102, 117, 118, 119, 120, 121, 195, 196, 197, 198, 199, 200, 201, 202, 203, 204, 205, 206, 207, 208, 209, 210, 211, 212, 213, 214, 215, 216, 217, 218, 219, 220, 221, 222, 223, 230, 303, 304, 305, 306, 307, 308, 309, 310, 311, 312, 314]
}

def main():
    print("=" * 80)
    print("EXTRACT ALL MOB ANIMATIONS (Bodies 0-399)")
    print("=" * 80)

    # Collect all unique mob bodies (0-399 only, exclude equipment 400+)
    all_mob_bodies = set()
    for file_bodies in ALL_BODIES.values():
        for body in file_bodies:
            if 0 <= body <= 399:  # Only mobs, not equipment
                all_mob_bodies.add(body)

    all_mob_bodies = sorted(all_mob_bodies)

    print(f"\nFound {len(all_mob_bodies)} unique mob bodies across all anim files")
    print(f"Body range: {min(all_mob_bodies)} to {max(all_mob_bodies)}")

    # Check which have already been extracted
    already_extracted = []
    for body in all_mob_bodies:
        output_dir = OUTPUT_PATH / f"body_{body}"
        if output_dir.exists() and any(output_dir.iterdir()):
            already_extracted.append(body)

    remaining = [b for b in all_mob_bodies if b not in already_extracted]

    print(f"\nAlready extracted: {len(already_extracted)} bodies")
    print(f"Remaining to extract: {len(remaining)} bodies")

    if not remaining:
        print("\n✓ All mob bodies already extracted!")
        return

    # Auto-confirm if -y flag provided
    auto_confirm = len(sys.argv) > 1 and sys.argv[1] == '-y'

    if not auto_confirm:
        response = input(f"\nExtract {len(remaining)} remaining mob bodies? (y/n): ")
        if response.lower() != 'y':
            print("Extraction cancelled.")
            return
    else:
        print(f"\nAuto-confirmed: Extracting {len(remaining)} mob bodies...")

    # Extract each remaining body
    success_count = 0
    fail_count = 0

    for i, body_id in enumerate(remaining, 1):
        print(f"\n[{i}/{len(remaining)}] Extracting body {body_id}...")

        try:
            result = subprocess.run(
                [sys.executable, str(EXTRACTOR_SCRIPT), str(body_id)],
                capture_output=True,
                text=True,
                encoding='utf-8',
                errors='replace'
            )

            # Check if output directory was created
            output_dir = OUTPUT_PATH / f"body_{body_id}"
            if output_dir.exists() and any(output_dir.iterdir()):
                print(f"  -> Body {body_id}: Success")
                success_count += 1
            else:
                print(f"  -> Body {body_id}: Failed (no output)")
                fail_count += 1

        except Exception as e:
            print(f"  -> Body {body_id}: Error - {e}")
            fail_count += 1

    # Final summary
    print("\n" + "=" * 80)
    print("EXTRACTION COMPLETE")
    print("=" * 80)
    print(f"Total mob bodies: {len(all_mob_bodies)}")
    print(f"  Already had: {len(already_extracted)}")
    print(f"  Newly extracted: {success_count}")
    print(f"  Failed: {fail_count}")
    print(f"  Grand total extracted: {len(already_extracted) + success_count}")
    print(f"\nOutput directory: {OUTPUT_PATH}")

if __name__ == "__main__":
    main()
