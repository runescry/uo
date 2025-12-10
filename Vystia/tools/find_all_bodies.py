"""
Find ALL body IDs present in all anim files
"""

import struct
from pathlib import Path

UO_CLIENT_PATH = r"C:\DevEnv\GIT\UO\UO Adventures\Client\Data Files"

def scan_anim_file(idx_path, mul_path, file_num):
    """Scan an anim file to find all valid bodies"""
    found_bodies = {}

    try:
        with open(idx_path, 'rb') as f:
            idx_data = f.read()
            num_entries = len(idx_data) // 12

            print(f"\nanim{file_num if file_num > 1 else ''}.mul: {num_entries:,} index entries")

            for i in range(num_entries):
                offset = i * 12
                if offset + 12 > len(idx_data):
                    break

                entry_offset, length, extra = struct.unpack('<III', idx_data[offset:offset+12])

                if entry_offset != 0xFFFFFFFF and length > 0:
                    # Try different body ranges
                    if i < 22000:  # High detail (0-199)
                        body = i // 110
                    elif i < 35000:  # Low detail (200-399)
                        body = 200 + ((i - 22000) // 65)
                    else:  # Human/equipment (400+)
                        body = 400 + ((i - 35000) // 175)

                    if body not in found_bodies:
                        found_bodies[body] = 0
                    found_bodies[body] += 1

        return found_bodies
    except Exception as e:
        print(f"  Error: {e}")
        return {}

def main():
    client_path = Path(UO_CLIENT_PATH)
    all_bodies = set()

    print("=" * 70)
    print("SCANNING ALL ANIM FILES FOR AVAILABLE BODIES")
    print("=" * 70)

    for i in range(1, 6):
        if i == 1:
            idx_path = client_path / "anim.idx"
            mul_path = client_path / "anim.mul"
        else:
            idx_path = client_path / f"anim{i}.idx"
            mul_path = client_path / f"anim{i}.mul"

        if idx_path.exists() and mul_path.exists():
            bodies = scan_anim_file(idx_path, mul_path, i)
            all_bodies.update(bodies.keys())
            if bodies:
                min_body = min(bodies.keys())
                max_body = max(bodies.keys())
                print(f"  Found {len(bodies)} bodies: {min_body} to {max_body}")

    # Summary
    print("\n" + "=" * 70)
    print("SUMMARY")
    print("=" * 70)
    print(f"Total unique bodies found: {len(all_bodies)}")

    sorted_bodies = sorted(all_bodies)
    print(f"Range: {min(sorted_bodies)} to {max(sorted_bodies)}")
    print(f"\nAll bodies: {sorted_bodies}")

    # Check against requested list
    print("\n" + "=" * 70)
    print("CHECKING AGAINST REQUESTED LIST")
    print("=" * 70)

    requested = [2, 10, 11, 19, 20, 23, 25, 27, 33, 36, 37, 38, 40, 43, 44, 45, 46, 55,
                 63, 64, 65, 66, 67, 69, 73, 74, 78, 79, 83, 84, 88, 89, 90, 91, 92, 93,
                 94, 96, 97, 98, 99, 100, 101, 102, 103, 104, 105, 106, 107, 108, 109, 110,
                 111, 112, 113, 114, 115, 116, 117, 118, 119, 120, 123, 125, 126, 127, 128,
                 129, 130, 131, 132, 133, 134, 135, 136, 137, 138, 139, 140, 141, 142, 143,
                 144, 145, 146, 147, 148, 149, 153, 155, 157, 158, 159, 160, 161, 162, 163,
                 164, 165, 166, 167, 168, 169, 170, 172, 173, 174, 175, 176, 177, 178, 179,
                 180, 181, 182, 183, 184, 185, 186, 187, 188, 189, 190, 191, 192, 193, 194,
                 195, 203, 240, 241, 242, 243, 244, 245, 246, 247, 248, 249, 250, 251, 252,
                 253, 254, 255, 256, 257, 258, 259, 260, 261, 262, 263, 264, 265, 266, 267,
                 268, 269, 270, 271, 272, 273, 274, 275, 276, 277, 278, 279, 280, 281, 282,
                 283, 284, 285, 286, 287, 293, 294, 295, 296, 297, 298, 299, 300, 301, 302,
                 303, 304, 305, 306, 307, 308, 309, 310, 311, 312, 313, 314, 315, 316, 317,
                 318, 319, 320, 321, 322, 323, 324, 325, 326, 327, 328, 329, 330, 331, 332,
                 333, 334, 689, 690, 691, 692, 693, 694, 695, 696, 697, 698, 699, 700, 701,
                 702, 703, 704, 705, 706, 707, 708, 709, 710, 711, 712, 713, 714, 715, 716,
                 717, 718, 719, 720, 721, 722, 723, 724, 725, 726, 727, 728, 729, 730, 731,
                 732, 733, 734, 735, 736, 737, 738, 739, 740, 741, 742, 743, 744, 745, 746,
                 747, 748, 752, 753, 754, 755, 758, 764, 765, 766, 767, 768, 769, 770, 771,
                 772, 773, 774, 775, 776, 777, 778, 779, 780, 786, 787, 788, 789, 790, 791,
                 792, 793, 794, 795, 796, 797, 798, 799, 800, 801, 802, 803, 804, 805, 806,
                 807, 808, 999]

    found = set(requested) & all_bodies
    missing = set(requested) - all_bodies
    extra_in_files = all_bodies - set(requested)

    print(f"Requested: {len(requested)} bodies")
    print(f"Found in files: {len(found)} bodies")
    print(f"Missing from files: {len(missing)} bodies")
    print(f"Extra in files (not requested): {len(extra_in_files)} bodies")

    if extra_in_files:
        print(f"\nExtra bodies available: {sorted(extra_in_files)}")

if __name__ == "__main__":
    main()
