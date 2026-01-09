#!/usr/bin/env python3
"""
Updates all Vystia spell files to inherit from VystiaSpell base class
and adds the MagicSchool property.
"""

import os
import re
from pathlib import Path

# Map folder names to magic school enum values
FOLDER_TO_SCHOOL = {
    'IceMage': 'Ice',
    'Druid': 'Nature',
    'Witch': 'Hex',
    'Sorcerer': 'Elemental',
    'Warlock': 'Dark',
    'Oracle': 'Divination',
    'Necromancer': 'Necromancy',
    'Summoner': 'Summoning',
    'Shaman': 'Shamanic',
    'Bard': 'Bardic',
    'Enchanter': 'Enchanting',
    'Illusionist': 'Illusion'
}

def get_magic_school(file_path):
    """Determine magic school from file path"""
    path_str = str(file_path)
    for folder, school in FOLDER_TO_SCHOOL.items():
        if f'\\{folder}\\' in path_str or f'/{folder}/' in path_str:
            return school
    return None

def update_spell_file(file_path):
    """Update a single spell file"""
    try:
        with open(file_path, 'r', encoding='utf-8-sig') as f:
            content = f.read()
    except:
        with open(file_path, 'r', encoding='utf-8') as f:
            content = f.read()

    original_content = content

    # Skip if already updated
    if 'VystiaSpell' in content and 'MagicSchool' in content:
        return False, "Already updated"

    # Get magic school for this file
    school = get_magic_school(file_path)
    if not school:
        return False, "Could not determine magic school"

    # 1. Change inheritance from MagerySpell to VystiaSpell
    content = re.sub(
        r': MagerySpell\b',
        ': VystiaSpell',
        content
    )

    # 2. Add using statement if not present
    if 'using Server.Spells.VystiaSpells;' not in content:
        # Find the last using statement and add after it
        match = re.search(r'(using [^;]+;\s*\n)(?=\s*namespace)', content)
        if match:
            insert_pos = match.end()
            content = content[:insert_pos] + 'using Server.Spells.VystiaSpells;\n' + content[insert_pos:]

    # 3. Add MagicSchool property after the Circle property
    # Find the class body and add the property
    circle_pattern = r'(public override SpellCircle Circle => SpellCircle\.\w+;)'
    match = re.search(circle_pattern, content)
    if match:
        circle_line = match.group(1)
        magic_school_property = f'\n        public override VystiaMagicSchool MagicSchool => VystiaMagicSchool.{school};'
        content = content.replace(circle_line, circle_line + magic_school_property)

    # 4. Update OnCast to call base.OnCast() first
    # Find OnCast method and add base call at the beginning
    oncast_pattern = r'(public override void OnCast\(\)\s*\{)'
    match = re.search(oncast_pattern, content)
    if match:
        # Check if base.OnCast() is already called
        if 'base.OnCast()' not in content:
            oncast_start = match.group(1)
            content = content.replace(
                oncast_start,
                oncast_start + '\n            base.OnCast(); // Check fizzle and trigger skill gain\n'
            )

    if content == original_content:
        return False, "No changes made"

    # Write updated content
    with open(file_path, 'w', encoding='utf-8') as f:
        f.write(content)

    return True, f"Updated (School: {school})"

def main():
    spells_dir = Path(r'D:\UO\ServUO\Scripts\Custom\VystiaClasses\Spells')

    updated = 0
    skipped = 0
    errors = 0

    # Find all spell files (excluding base classes and initializer)
    for spell_file in spells_dir.rglob('*Spell.cs'):
        # Skip non-spell files
        filename = spell_file.name
        if filename in ['VystiaSpellBase.cs', 'VystiaSpellInitializer.cs']:
            continue

        success, message = update_spell_file(spell_file)

        if success:
            print(f"[OK] {spell_file.name}: {message}")
            updated += 1
        else:
            print(f"[--] {spell_file.name}: {message}")
            if "error" in message.lower():
                errors += 1
            else:
                skipped += 1

    print(f"\n=== Summary ===")
    print(f"Updated: {updated}")
    print(f"Skipped: {skipped}")
    print(f"Errors: {errors}")

if __name__ == '__main__':
    main()
