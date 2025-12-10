"""
Diagnostic script to check which body IDs exist in which anim files
"""

import struct
from pathlib import Path

UO_CLIENT_PATH = r"C:\DevEnv\GIT\UO\UO Adventures\Client\Data Files"

def check_anim_file(idx_path, mul_path, file_num):
    """Check which body IDs have animations in this anim file"""
    found_bodies = set()

    print(f"\n{'='*60}")
    print(f"Checking {mul_path.name}")
    print(f"{'='*60}")

    try:
        with open(idx_path, 'rb') as f:
            # Read all index entries
            idx_data = f.read()
            num_entries = len(idx_data) // 12

            print(f"Total index entries: {num_entries}")

            for i in range(num_entries):
                offset = i * 12
                if offset + 12 > len(idx_data):
                    break

                entry_offset, length, extra = struct.unpack('<III', idx_data[offset:offset+12])

                if entry_offset != 0xFFFFFFFF and length > 0:
                    # This index has data - try to figure out which body it corresponds to
                    # For high detail monsters (0-199): 110 entries per body
                    # For low detail monsters (200-399): 65 entries per body
                    # For humans/equipment (400+): 175 entries per body

                    if i < 22000:  # High detail range
                        body = i // 110
                        found_bodies.add(body)
                    elif i < 35000:  # Low detail range
                        body = 200 + ((i - 22000) // 65)
                        found_bodies.add(body)
                    else:  # Human/equipment range
                        body = 400 + ((i - 35000) // 175)
                        found_bodies.add(body)

            print(f"Found bodies: {sorted(found_bodies)}")
            print(f"Total unique bodies: {len(found_bodies)}")

            return found_bodies

    except Exception as e:
        print(f"Error reading {idx_path}: {e}")
        return set()

def main():
    client_path = Path(UO_CLIENT_PATH)

    all_bodies = {}

    # Check all anim files
    for i in range(1, 6):
        if i == 1:
            idx_path = client_path / "anim.idx"
            mul_path = client_path / "anim.mul"
        else:
            idx_path = client_path / f"anim{i}.idx"
            mul_path = client_path / f"anim{i}.mul"

        if idx_path.exists() and mul_path.exists():
            bodies = check_anim_file(idx_path, mul_path, i)
            all_bodies[i] = bodies
        else:
            print(f"\nSkipping anim{i} - files not found")

    # Summary
    print(f"\n{'='*60}")
    print("SUMMARY - Bodies by File")
    print(f"{'='*60}")

    for file_num, bodies in sorted(all_bodies.items()):
        print(f"\nanim{file_num if file_num > 1 else ''}.mul: {len(bodies)} bodies")
        if len(bodies) < 50:  # Only print if reasonable number
            print(f"  Bodies: {sorted(bodies)}")

    # Check for specific bodies we expected
    print(f"\n{'='*60}")
    print("Checking specific bodies that failed:")
    print(f"{'='*60}")

    test_bodies = [88, 89, 90, 91, 92, 93, 94, 96, 97, 98, 99, 100]

    for body in test_bodies:
        found_in = []
        for file_num, bodies in all_bodies.items():
            if body in bodies:
                found_in.append(file_num)

        if found_in:
            print(f"Body {body}: Found in anim{found_in[0] if found_in[0] > 1 else ''}.mul")
        else:
            print(f"Body {body}: NOT FOUND in any anim file")

if __name__ == "__main__":
    main()
