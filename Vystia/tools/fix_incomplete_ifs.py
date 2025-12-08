#!/usr/bin/env python3
"""
Fix incomplete if statements left by reagent removal
"""

import re
from pathlib import Path

FILES_TO_FIX = [
    "C:/DevEnv/GIT/UO/ServUO/Scripts/Mobiles/Vystia/Emberlands/VystiaEmberPhoenix.cs",
    "C:/DevEnv/GIT/UO/ServUO/Scripts/Mobiles/Vystia/Desert/OasisGuardian.cs",
    "C:/DevEnv/GIT/UO/ServUO/Scripts/Mobiles/Vystia/Bosses/AncientKraken.cs",
    "C:/DevEnv/GIT/UO/ServUO/Scripts/Mobiles/Vystia/Bosses/GriffinLord.cs",
    "C:/DevEnv/GIT/UO/ServUO/Scripts/Mobiles/Vystia/Bosses/SphinxOfSurya.cs"
]

def fix_file(filepath):
    """Fix incomplete if statements in a file"""
    filepath = Path(filepath)

    with open(filepath, 'r', encoding='utf-8') as f:
        content = f.read()

    # Pattern: if statement followed by commented PackItem
    # Replace with all lines commented
    pattern = r'(\s+)if \(Utility\.RandomDouble\(\) < [0-9.]+\)\n\1// REMOVED OLD REAGENT: PackItem\(new \w+\([^)]*\)\);'

    def replace_func(match):
        indent = match.group(1)
        return f'{indent}// REMOVED OLD REAGENT:\n{indent}{match.group(0).split("\\n")[0]}\n{indent}//     PackItem(new ...'

    # Simpler approach: just find and manually fix
    lines = content.split('\n')
    new_lines = []
    i = 0

    while i < len(lines):
        line = lines[i]

        # Check if this is an if statement
        if 'if (Utility.RandomDouble()' in line and i + 1 < len(lines):
            next_line = lines[i + 1]

            # Check if next line is the removed reagent comment
            if '// REMOVED OLD REAGENT: PackItem' in next_line:
                # Comment out both lines
                indent = len(line) - len(line.lstrip())
                indent_str = ' ' * indent

                new_lines.append(f'{indent_str}// REMOVED OLD REAGENT:')
                new_lines.append(f'{indent_str}// {line.strip()}')
                new_lines.append(f'{indent_str}//     {next_line.split("// REMOVED OLD REAGENT: ")[1]}')
                i += 2  # Skip next line
                continue

        new_lines.append(line)
        i += 1

    new_content = '\n'.join(new_lines)

    with open(filepath, 'w', encoding='utf-8') as f:
        f.write(new_content)

    return True

def main():
    print("="*80)
    print("FIXING INCOMPLETE IF STATEMENTS")
    print("="*80)
    print()

    for filepath in FILES_TO_FIX:
        if Path(filepath).exists():
            fix_file(filepath)
            print(f"[OK] {Path(filepath).name}")
        else:
            print(f"[SKIP] {Path(filepath).name} (not found)")

    print()
    print("="*80)
    print("[SUCCESS] All files fixed!")
    print("="*80)
    print()

if __name__ == "__main__":
    main()
