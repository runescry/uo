#!/usr/bin/env python3
"""
Update all magic school .md files with implementation status and reagent notes.
"""

import os
import re

# School configuration: (filename, class_name, reagent_file, tested_status)
SCHOOLS = [
    ("HexMagic.md", "Witch", "HexMagicReagents.cs", "ready for testing"),
    ("ElementalMagic.md", "Sorcerer", "ElementalMagicReagents.cs", "ready for testing"),
    ("DarkMagic.md", "Warlock", "DarkMagicReagents.cs", "ready for testing"),
    ("DivinationMagic.md", "Oracle", "DivinationMagicReagents.cs", "ready for testing"),
    ("Necromancy.md", "Necromancer", "NecromancyReagents.cs", "ready for testing"),
    ("SummoningMagic.md", "Summoner", "SummoningMagicReagents.cs", "ready for testing"),
    ("ShamanicMagic.md", "Shaman", "ShamanicMagicReagents.cs", "ready for testing"),
    ("BardicMagic.md", "Bard", "BardicMagicReagents.cs", "ready for testing"),
    ("EnchantingMagic.md", "Enchanter", "EnchantingMagicReagents.cs", "ready for testing"),
    ("IllusionMagic.md", "Illusionist", "IllusionMagicReagents.cs", "ready for testing"),
]

MAGIC_DIR = r"C:\DevEnv\GIT\UO\Vystia\Magic"

REAGENT_NOTE = """> **📝 Reagent Note:** Individual spell entries below use placeholder UO reagent names for design reference. The actual ServUO implementation uses custom Vystia reagents as documented in the [Reagents](#reagents) section at the end of this file. See `{reagent_file}` for implementation.

---"""

def update_school_file(filename, class_name, reagent_file, tested_status):
    """Update a single school file."""
    filepath = os.path.join(MAGIC_DIR, filename)

    if not os.path.exists(filepath):
        print(f"[ERROR] File not found: {filepath}")
        return False

    with open(filepath, 'r', encoding='utf-8') as f:
        content = f.read()

    # Find the header section (up to first ---)
    header_match = re.search(r'^(#[^\n]+\n\n.+?)\n---', content, re.MULTILINE | re.DOTALL)
    if not header_match:
        print(f"[ERROR] Could not find header in {filename}")
        return False

    header_section = header_match.group(1)

    # Check if implementation status already exists
    if "**Implementation Status:**" in header_section or "**Status:**" in header_section:
        # Update existing status
        if "**Implementation Status:**" in header_section:
            new_header = re.sub(
                r'\*\*Implementation Status:\*\* .*',
                f'**Implementation Status:** ✅ **32/32 spells implemented (100% COMPLETE - {tested_status})**',
                header_section
            )
        else:
            new_header = re.sub(
                r'\*\*Status:\*\* .*',
                f'**Status:** ✅ **32/32 spells implemented (100% COMPLETE - {tested_status})**',
                header_section
            )
    else:
        # Add implementation status before closing of header
        new_header = header_section + f'\n**Implementation Status:** ✅ **32/32 spells implemented (100% COMPLETE - {tested_status})**'

    # Check if reagent note already exists
    if "Reagent Note:" in content:
        print(f"[OK] {filename} already has reagent note - skipping")
        return True

    # Add reagent note after header
    reagent_note = REAGENT_NOTE.format(reagent_file=reagent_file)
    new_content = content.replace(
        header_match.group(0),
        new_header + '\n\n' + reagent_note
    )

    # Write back
    with open(filepath, 'w', encoding='utf-8') as f:
        f.write(new_content)

    print(f"[OK] Updated {filename}")
    return True

def main():
    """Update all school files."""
    print("Updating magic school files...")
    print("=" * 60)

    success_count = 0
    for filename, class_name, reagent_file, tested_status in SCHOOLS:
        if update_school_file(filename, class_name, reagent_file, tested_status):
            success_count += 1

    print("=" * 60)
    print(f"[DONE] Successfully updated {success_count}/{len(SCHOOLS)} files")

if __name__ == "__main__":
    main()
