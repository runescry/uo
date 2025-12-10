"""
Smart batch extractor that finds which anim file contains each body
and extracts with correct index calculation
"""

import struct
import subprocess
import sys
from pathlib import Path

UO_CLIENT_PATH = r"C:\DevEnv\GIT\UO\UO Adventures\Client\Data Files"
BODY_LIST_FILE = r"c:\Users\User\Downloads\Monsters to extract.txt"
EXTRACTOR_SCRIPT = Path(__file__).parent / "creature_animation_extractor.py"
OUTPUT_PATH = Path(r"C:\DevEnv\GIT\UO\Vystia\tools\creature_animations")

def find_body_in_files(body_id):
    """Find which anim file contains this body ID"""
    client_path = Path(UO_CLIENT_PATH)

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

                for entry_idx in range(num_entries):
                    offset = entry_idx * 12
                    if offset + 12 > len(idx_data):
                        break

                    entry_offset, length, extra = struct.unpack('<III', idx_data[offset:offset+12])

                    if entry_offset != 0xFFFFFFFF and length > 0:
                        # Try to reverse-calculate body from index
                        if entry_idx < 22000:
                            calc_body = entry_idx // 110
                        elif entry_idx < 35000:
                            calc_body = 200 + ((entry_idx - 22000) // 65)
                        else:
                            calc_body = 400 + ((entry_idx - 35000) // 175)

                        if calc_body == body_id:
                            return i
        except:
            pass

    return None

def parse_body_list(filepath):
    """Parse body ID list"""
    body_ids = []
    with open(filepath, 'r') as f:
        for line in f:
            line = line.strip()
            if line:
                try:
                    body_ids.append(int(line))
                except:
                    pass
    return sorted(set(body_ids))

def main():
    print("=" * 70)
    print("SMART BATCH EXTRACTOR - Finding Bodies in Anim Files")
    print("=" * 70)

    body_ids = parse_body_list(BODY_LIST_FILE)
    print(f"\nRequested bodies: {len(body_ids)}\n")

    success = 0
    failed = 0
    not_found = 0

    for i, body_id in enumerate(body_ids, 1):
        anim_file = find_body_in_files(body_id)

        if anim_file is None:
            print(f"[{i:3d}/{len(body_ids)}] Body {body_id:4d}: NOT FOUND in any anim file")
            not_found += 1
            continue

        try:
            result = subprocess.run(
                [sys.executable, str(EXTRACTOR_SCRIPT), str(body_id)],
                capture_output=True,
                text=True,
                encoding='utf-8',
                errors='replace',
                timeout=30
            )

            output_dir = OUTPUT_PATH / f"body_{body_id}"
            if output_dir.exists() and any(output_dir.iterdir()):
                print(f"[{i:3d}/{len(body_ids)}] Body {body_id:4d}: SUCCESS (anim{anim_file})")
                success += 1
            else:
                print(f"[{i:3d}/{len(body_ids)}] Body {body_id:4d}: FAILED")
                failed += 1
        except Exception as e:
            print(f"[{i:3d}/{len(body_ids)}] Body {body_id:4d}: ERROR - {e}")
            failed += 1

    print("\n" + "=" * 70)
    print(f"Success: {success}")
    print(f"Failed: {failed}")
    print(f"Not found: {not_found}")
    print(f"Total: {len(body_ids)}")

if __name__ == "__main__":
    main()
