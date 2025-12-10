"""
Extract ALL mob animations (bodies 0-999) from all anim files
Extended version to include the full body range
"""

import os
import sys
import struct
import subprocess
from pathlib import Path

EXTRACTOR_SCRIPT = Path(__file__).parent / "creature_animation_extractor.py"
OUTPUT_PATH = Path(r"C:\DevEnv\GIT\UO\Vystia\tools\creature_animations")
UO_CLIENT_PATH = r"C:\DevEnv\GIT\UO\UO Adventures\Client\Data Files"

def scan_all_bodies():
    """Scan all anim files to find all bodies 0-999"""
    client_path = Path(UO_CLIENT_PATH)
    all_bodies = set()

    for i in range(1, 6):
        if i == 1:
            idx_path = client_path / "anim.idx"
            mul_path = client_path / "anim.mul"
        else:
            idx_path = client_path / f"anim{i}.idx"
            mul_path = client_path / f"anim{i}.mul"

        if not (idx_path.exists() and mul_path.exists()):
            continue

        try:
            with open(idx_path, 'rb') as f:
                idx_data = f.read()
                num_entries = len(idx_data) // 12

                for j in range(num_entries):
                    offset = j * 12
                    if offset + 12 > len(idx_data):
                        break
                    entry_offset, length, extra = struct.unpack('<III', idx_data[offset:offset+12])
                    if entry_offset != 0xFFFFFFFF and length > 0:
                        # High detail range (0-199)
                        if j < 22000:
                            body = j // 110
                            if body < 200:
                                all_bodies.add(body)
                        # Low detail range (200-399)
                        elif j < 35000:
                            body = 200 + ((j - 22000) // 65)
                            if 200 <= body < 400:
                                all_bodies.add(body)
                        # Human/equipment range (400+)
                        else:
                            body = 400 + ((j - 35000) // 175)
                            if body < 1000:
                                all_bodies.add(body)
        except Exception as e:
            print(f"Error scanning {idx_path}: {e}")

    return sorted(all_bodies)

def main():
    print("=" * 80)
    print("EXTRACT ALL MOB ANIMATIONS (Bodies 0-999) - EXTENDED")
    print("=" * 80)

    # Scan for all bodies
    print("\nScanning anim files for all bodies 0-999...")
    all_mob_bodies = scan_all_bodies()

    print(f"\nFound {len(all_mob_bodies)} unique bodies across all anim files")
    print(f"Body range: {min(all_mob_bodies)} to {max(all_mob_bodies)}")

    # Count by range
    range_0_199 = len([b for b in all_mob_bodies if b < 200])
    range_200_399 = len([b for b in all_mob_bodies if 200 <= b < 400])
    range_400_999 = len([b for b in all_mob_bodies if b >= 400])
    print(f"  Bodies 0-199 (high detail): {range_0_199}")
    print(f"  Bodies 200-399 (low detail): {range_200_399}")
    print(f"  Bodies 400-999 (human/equipment): {range_400_999}")

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
        print("\n✓ All bodies already extracted!")
        return

    # Auto-confirm if -y flag provided
    auto_confirm = len(sys.argv) > 1 and sys.argv[1] == '-y'

    if not auto_confirm:
        response = input(f"\nExtract {len(remaining)} remaining bodies? (y/n): ")
        if response.lower() != 'y':
            print("Extraction cancelled.")
            return
    else:
        print(f"\nAuto-confirmed: Extracting {len(remaining)} bodies...")

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
    print(f"Total bodies found: {len(all_mob_bodies)}")
    print(f"  Already had: {len(already_extracted)}")
    print(f"  Newly extracted: {success_count}")
    print(f"  Failed: {fail_count}")
    print(f"  Grand total extracted: {len(already_extracted) + success_count}")
    print(f"\nOutput directory: {OUTPUT_PATH}")

if __name__ == "__main__":
    main()
