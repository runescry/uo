#!/usr/bin/env python3
import re
from pathlib import Path

# Get all Illusion spell files
spell_dir = Path("C:/DevEnv/GIT/UO/ServUO/Scripts/Custom/VystiaClasses/Spells/Illusionist")
spell_files = sorted([f.stem for f in spell_dir.glob("*.cs")])  # Remove .cs extension

print(f"Found {len(spell_files)} Illusion spell files:")
for spell in spell_files:
    print(f"  {spell}")

# Get registered Illusion spells
initializer_path = Path("C:/DevEnv/GIT/UO/ServUO/Scripts/Custom/VystiaClasses/Spells/VystiaSpellInitializer.cs")

with open(initializer_path, 'r', encoding='utf-8') as f:
    content = f.read()

pattern = r'private static void RegisterIllusionistSpells\(\).*?^        \}'
match = re.search(pattern, content, re.DOTALL | re.MULTILINE)

if match:
    registered = re.findall(r'typeof\((\w+)\)', match.group(0))
    print(f"\nFound {len(registered)} registered Illusion spells:")
    for spell in registered:
        print(f"  {spell}")

    # Find missing spells
    registered_set = set(registered)
    file_set = set(spell_files)

    missing = file_set - registered_set
    extra = registered_set - file_set

    if missing:
        print(f"\nMissing from registration ({len(missing)}):")
        for spell in sorted(missing):
            print(f"  {spell}")

    if extra:
        print(f"\nRegistered but no file exists ({len(extra)}):")
        for spell in sorted(extra):
            print(f"  {spell}")
else:
    print("RegisterIllusionistSpells() method not found!")
