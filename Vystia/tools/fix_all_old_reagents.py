#!/usr/bin/env python3
"""
Comment out all old exotic reagent references in remaining files
"""

from pathlib import Path

FILES_TO_FIX = [
    "C:/DevEnv/GIT/UO/ServUO/Scripts/Commands/VystiaLootCrate.cs",
    "C:/DevEnv/GIT/UO/ServUO/Scripts/Misc/VystiaLootPack.cs",
    "C:/DevEnv/GIT/UO/ServUO/Scripts/Mobiles/Vystia/Vendors/VystiaReagentVendor.cs",
    "C:/DevEnv/GIT/UO/ServUO/Scripts/Mobiles/Vystia/Vendors/VystiaResourceVendor.cs"
]

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

def fix_file(filepath):
    """Comment out lines containing old reagents"""
    filepath = Path(filepath)

    if not filepath.exists():
        return False

    with open(filepath, 'r', encoding='utf-8') as f:
        lines = f.readlines()

    modified = False
    new_lines = []

    for line in lines:
        # Check if line contains any old reagent
        contains_old_reagent = any(reagent in line for reagent in OLD_REAGENTS)

        if contains_old_reagent and not line.strip().startswith('//'):
            # Comment out the line
            # Preserve indentation
            stripped = line.lstrip()
            indent = line[:len(line) - len(stripped)]
            new_lines.append(f'{indent}// REMOVED OLD REAGENT: {stripped}')
            modified = True
        else:
            new_lines.append(line)

    if modified:
        with open(filepath, 'w', encoding='utf-8') as f:
            f.writelines(new_lines)

    return modified

def main():
    print("="*80)
    print("COMMENTING OUT ALL OLD REAGENT REFERENCES")
    print("="*80)
    print()

    files_modified = 0

    for filepath in FILES_TO_FIX:
        filepath = Path(filepath)
        if filepath.exists():
            if fix_file(filepath):
                files_modified += 1
                print(f"[MODIFIED] {filepath.name}")
            else:
                print(f"[OK] {filepath.name} (no old reagents found)")
        else:
            print(f"[SKIP] {filepath.name} (not found)")

    print()
    print("="*80)
    print(f"[SUCCESS] Modified {files_modified} files")
    print("="*80)
    print()

if __name__ == "__main__":
    main()
