#!/usr/bin/env python3
"""
Remove all hues from Vystia reagents
Keep Vystia-themed names, just remove the color distinction
"""

import os
import re

REAGENT_PATH = r'D:\UO\ServUO\Scripts\Items\Vystia\Resources\Reagents'

def remove_hues_from_file(filepath):
    """Remove hue parameters from reagent constructors"""
    with open(filepath, 'r', encoding='utf-8') as f:
        content = f.read()

    original = content

    # Pattern: base(amount, 0xHEX, 0xHUE, "location", "usage")
    # Replace with: base(amount, 0xHEX, 0, "location", "usage")
    pattern = r'(base\(amount,\s*0x[0-9a-fA-F]+,\s*)0x[0-9a-fA-F]+'
    replacement = r'\g<1>0'

    content = re.sub(pattern, replacement, content)

    if content != original:
        with open(filepath, 'w', encoding='utf-8') as f:
            f.write(content)
        return True
    return False

def main():
    print("="*70)
    print("  REMOVING HUES FROM ALL VYSTIA REAGENTS")
    print("="*70)
    print()
    print("All reagents will keep their Vystia-themed names")
    print("but will no longer be recolored (hue = 0)")
    print()
    print("="*70)
    print()

    files_modified = 0

    for filename in os.listdir(REAGENT_PATH):
        if filename.endswith('Reagents.cs'):
            filepath = os.path.join(REAGENT_PATH, filename)
            print(f"Processing {filename}...")

            if remove_hues_from_file(filepath):
                print(f"  Hues removed")
                files_modified += 1
            else:
                print(f"  (no hues found or already 0)")

    print()
    print("="*70)
    print(f"  COMPLETE: {files_modified} files modified")
    print("="*70)
    print()
    print("All reagents now use hue = 0 (no recoloring)")
    print("Players will distinguish reagents by name only")
    print()

if __name__ == "__main__":
    main()
