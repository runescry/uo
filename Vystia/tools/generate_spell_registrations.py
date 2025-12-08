#!/usr/bin/env python3
"""
Generate spell registration code for all Vystia magic schools.
Extracts spell class names from existing spell files and generates VystiaSpellInitializer registration methods.
"""

import os
import re
from pathlib import Path

# Magic school configuration with spell ID ranges
SCHOOLS = [
    {"name": "IceMage", "namespace": "IceMage", "start_id": 1000, "display": "Ice Magic"},
    {"name": "Druid", "namespace": "Nature", "start_id": 1032, "display": "Nature Magic"},
    {"name": "Witch", "namespace": "Hex", "start_id": 1064, "display": "Hex Magic"},
    {"name": "Sorcerer", "namespace": "Elemental", "start_id": 1096, "display": "Elemental Magic"},
    {"name": "Warlock", "namespace": "Dark", "start_id": 1128, "display": "Dark Magic"},
    {"name": "Oracle", "namespace": "Divination", "start_id": 1160, "display": "Divination Magic"},
    {"name": "Necromancer", "namespace": "Necromancy", "start_id": 1192, "display": "Necromancy"},
    {"name": "Summoner", "namespace": "Summoning", "start_id": 1224, "display": "Summoning Magic"},
    {"name": "Shaman", "namespace": "Shamanic", "start_id": 1256, "display": "Shamanic Magic"},
    {"name": "Bard", "namespace": "Bardic", "start_id": 1288, "display": "Bardic Magic"},
    {"name": "Enchanter", "namespace": "Enchanting", "start_id": 1320, "display": "Enchanting Magic"},
    {"name": "Illusionist", "namespace": "Illusion", "start_id": 1352, "display": "Illusion Magic"},
]

def extract_spell_class_names(spell_dir):
    """Extract spell class names from .cs files in a directory."""
    spell_files = sorted(list(Path(spell_dir).glob("*.cs")))
    spell_classes = []

    for spell_file in spell_files:
        with open(spell_file, 'r', encoding='utf-8') as f:
            content = f.read()

        # Look for: public class ClassName : MagerySpell
        match = re.search(r'public\s+class\s+(\w+)\s*:', content)
        if match:
            class_name = match.group(1)
            spell_classes.append(class_name)

    return spell_classes

def generate_registration_method(school):
    """Generate a registration method for a single school."""
    # Fix path: go up from tools/ to UO/, then to ServUO/
    base_path = Path(__file__).parent.parent.parent
    spell_dir = base_path / "ServUO" / "Scripts" / "Custom" / "VystiaClasses" / "Spells" / school["name"]

    if not spell_dir.exists():
        print(f"WARNING: Spell directory not found: {spell_dir}")
        return None

    spell_classes = extract_spell_class_names(spell_dir)

    if not spell_classes:
        print(f"WARNING: No spell classes found in {spell_dir}")
        return None

    if len(spell_classes) != 32:
        print(f"WARNING: Expected 32 spells for {school['name']}, found {len(spell_classes)}")

    # Generate method
    lines = []
    lines.append(f"        private static void Register{school['name']}Spells()")
    lines.append("        {")
    lines.append(f"            // {school['display']} Spells (IDs {school['start_id']}-{school['start_id']+31})")

    for i, class_name in enumerate(spell_classes):
        spell_id = school['start_id'] + i
        lines.append(f"            Register({spell_id}, typeof({class_name}));")

    lines.append("        }")
    lines.append("")

    return "\n".join(lines), school['namespace'], school['name']

def main():
    print("=" * 80)
    print("VYSTIA SPELL REGISTRATION GENERATOR")
    print("=" * 80)
    print()

    # Generate registration methods for all schools
    registration_methods = []
    using_statements = set()
    method_calls = []

    for school in SCHOOLS:
        print(f"Processing {school['name']}...")
        result = generate_registration_method(school)

        if result:
            method_code, namespace, name = result
            registration_methods.append(method_code)
            using_statements.add(f"using Server.Spells.VystiaSpells.{namespace};")
            method_calls.append(f"            Register{name}Spells();")
            print(f"  [OK] Generated registration for {school['name']} ({namespace})")
        else:
            print(f"  [FAIL] Failed to generate registration for {school['name']}")

    print()
    print(f"Generated {len(registration_methods)} registration methods")
    print()

    # Output using statements
    print("=" * 80)
    print("USING STATEMENTS (add to top of VystiaSpellInitializer.cs):")
    print("=" * 80)
    for using in sorted(using_statements):
        print(using)

    print()
    print("=" * 80)
    print("METHOD CALLS (add to Initialize() method):")
    print("=" * 80)
    for call in method_calls:
        print(call)

    print()
    print("=" * 80)
    print("REGISTRATION METHODS (add to VystiaSpellInitializer class):")
    print("=" * 80)
    for method in registration_methods:
        print(method)

    # Save to file
    output_file = Path(__file__).parent / "spell_registrations_output.txt"
    with open(output_file, 'w', encoding='utf-8') as f:
        f.write("// USING STATEMENTS\n")
        for using in sorted(using_statements):
            f.write(using + "\n")
        f.write("\n// METHOD CALLS (add to Initialize())\n")
        for call in method_calls:
            f.write(call + "\n")
        f.write("\n// REGISTRATION METHODS\n")
        for method in registration_methods:
            f.write(method + "\n")

    print()
    print(f"Output saved to: {output_file}")
    print()

if __name__ == "__main__":
    main()
