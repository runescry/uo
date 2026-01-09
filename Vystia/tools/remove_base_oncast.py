#!/usr/bin/env python3
"""
Remove base.OnCast() calls from Vystia spells.
These were added in error - Spell.OnCast() is abstract.
"""

import os
import re

SPELL_FOLDERS = [
    r"D:\UO\ServUO\Scripts\Custom\VystiaClasses\Spells\IceMage",
    r"D:\UO\ServUO\Scripts\Custom\VystiaClasses\Spells\Druid",
    r"D:\UO\ServUO\Scripts\Custom\VystiaClasses\Spells\Witch",
    r"D:\UO\ServUO\Scripts\Custom\VystiaClasses\Spells\Sorcerer",
    r"D:\UO\ServUO\Scripts\Custom\VystiaClasses\Spells\Warlock",
    r"D:\UO\ServUO\Scripts\Custom\VystiaClasses\Spells\Oracle",
    r"D:\UO\ServUO\Scripts\Custom\VystiaClasses\Spells\Necromancer",
    r"D:\UO\ServUO\Scripts\Custom\VystiaClasses\Spells\Summoner",
    r"D:\UO\ServUO\Scripts\Custom\VystiaClasses\Spells\Shaman",
    r"D:\UO\ServUO\Scripts\Custom\VystiaClasses\Spells\Bard",
    r"D:\UO\ServUO\Scripts\Custom\VystiaClasses\Spells\Enchanter",
    r"D:\UO\ServUO\Scripts\Custom\VystiaClasses\Spells\Illusionist",
]

def remove_base_oncast(filepath):
    """Remove base.OnCast(); line from file"""
    with open(filepath, 'r', encoding='utf-8') as f:
        content = f.read()

    original = content

    # Remove "base.OnCast();" with optional whitespace and newline
    content = re.sub(r'\s*base\.OnCast\(\);\s*\n?', '\n', content)

    if content != original:
        with open(filepath, 'w', encoding='utf-8') as f:
            f.write(content)
        return True
    return False

def main():
    updated = 0
    skipped = 0

    for folder in SPELL_FOLDERS:
        if not os.path.exists(folder):
            print(f"Folder not found: {folder}")
            continue

        for filename in os.listdir(folder):
            if not filename.endswith('.cs'):
                continue

            filepath = os.path.join(folder, filename)
            if remove_base_oncast(filepath):
                print(f"Fixed: {filename}")
                updated += 1
            else:
                skipped += 1

    print(f"\n=== Summary ===")
    print(f"Updated: {updated}")
    print(f"Skipped: {skipped}")

if __name__ == "__main__":
    main()
