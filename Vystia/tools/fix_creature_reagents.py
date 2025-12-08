#!/usr/bin/env python3
"""
Remove old exotic reagent references from creature files
These old reagents don't exist in the new 96-reagent magic school system
"""

import os
import re
from pathlib import Path

CREATURE_DIR = Path("C:/DevEnv/GIT/UO/ServUO/Scripts/Mobiles/Vystia")

# Old reagents to remove (not part of 96-reagent magic school system)
OLD_REAGENTS = [
    "AbyssalInk",
    "CoralFragment",
    "CrystalPollen",
    "DarkVoidDust",
    "DemonScale",
    "DesertRose",
    "DragonScalePowder",
    "EmberBloom",
    "EmberFeather",
    "EnchantersInk",
    "KelpStrand",
    "KrakenInk",
    "LeviathanTooth",
    "RainbowCrystal",
    "SeaGlass",
    "WindCrystal"
]

def remove_old_reagent_lines(filepath):
    """Remove lines that reference old reagents"""

    with open(filepath, 'r', encoding='utf-8') as f:
        lines = f.readlines()

    modified = False
    new_lines = []

    for line in lines:
        # Check if line contains any old reagent reference
        contains_old_reagent = any(reagent in line for reagent in OLD_REAGENTS)

        if contains_old_reagent:
            # Comment out the line instead of deleting
            new_lines.append(f"            // REMOVED OLD REAGENT: {line.strip()}\n")
            modified = True
        else:
            new_lines.append(line)

    if modified:
        with open(filepath, 'w', encoding='utf-8') as f:
            f.writelines(new_lines)

    return modified

def main():
    print("="*80)
    print("REMOVING OLD EXOTIC REAGENT REFERENCES FROM CREATURE FILES")
    print("="*80)
    print()

    files_modified = 0

    # Process all .cs files in creature directories
    for cs_file in CREATURE_DIR.rglob("*.cs"):
        if cs_file.name == "MagicSchoolVendors.cs" or cs_file.name.endswith("Vendor.cs"):
            # Skip vendor files - we'll handle those separately
            continue

        if remove_old_reagent_lines(cs_file):
            files_modified += 1
            print(f"[MODIFIED] {cs_file.relative_to(CREATURE_DIR)}")

    print()
    print("="*80)
    print(f"[SUCCESS] Modified {files_modified} creature files")
    print("="*80)
    print()
    print("[NOTE] Old reagent lines have been commented out with // REMOVED OLD REAGENT:")
    print("[NOTE] Vendor files skipped - will be regenerated separately")
    print()

if __name__ == "__main__":
    main()
