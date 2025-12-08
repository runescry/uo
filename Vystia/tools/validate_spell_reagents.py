#!/usr/bin/env python3
"""
Validate that spell reagent requirements match actual implemented reagents
"""

import os
import re
from pathlib import Path
from collections import defaultdict

# Paths
SERVUO_ROOT = Path("C:/DevEnv/GIT/UO/ServUO")
SPELL_DIR = SERVUO_ROOT / "Scripts/Custom/VystiaClasses/Spells"
REAGENT_DIR = SERVUO_ROOT / "Scripts/Items/Vystia/Resources/Reagents"

# School mapping
SCHOOL_MAPPING = {
    "IceMage": "IceMagic",
    "Druid": "NatureMagic",
    "Witch": "HexMagic",
    "Sorcerer": "ElementalMagic",
    "Warlock": "DarkMagic",
    "Oracle": "DivinationMagic",
    "Necromancer": "Necromancy",
    "Summoner": "SummoningMagic",
    "Shaman": "ShamanicMagic",
    "Bard": "BardicMagic",
    "Enchanter": "EnchantingMagic",
    "Illusionist": "IllusionMagic"
}

def get_available_reagents(school):
    """Get list of available reagents for a school"""
    reagent_file = REAGENT_DIR / f"{school}Reagents.cs"
    reagents = []

    if not reagent_file.exists():
        return reagents

    with open(reagent_file, 'r', encoding='utf-8') as f:
        content = f.read()
        # Find all reagent class names
        pattern = r'public class (\w+)\s*:\s*BaseVystiaReagent'
        reagents = re.findall(pattern, content)

    return reagents

def extract_reagents_from_spell(filepath):
    """Extract reagent requirements from a spell file"""
    try:
        with open(filepath, 'r', encoding='utf-8') as f:
            content = f.read()

            # Look for Reagents property or similar
            # Common patterns:
            # - public override Type[] ReagentTypes => new[] { typeof(ReagentName), ... };
            # - Reagents.Add(typeof(ReagentName));
            # - new[] { typeof(ReagentName1), typeof(ReagentName2) }

            reagents = []

            # Pattern 1: typeof(ReagentName)
            typeof_pattern = r'typeof\s*\(\s*(\w+)\s*\)'
            matches = re.findall(typeof_pattern, content)
            reagents.extend(matches)

            # Filter out non-reagent types (Item, Mobile, etc.)
            common_types = ['Item', 'Mobile', 'PlayerMobile', 'BaseCreature', 'Container',
                           'Server', 'int', 'bool', 'string', 'double', 'object']
            reagents = [r for r in reagents if r not in common_types and not r.startswith('Base')]

            return list(set(reagents))  # Remove duplicates

    except Exception as e:
        print(f"[ERROR] Reading {filepath}: {e}")
        return []

def scan_spell_directory(spell_school_dir):
    """Scan all spell files in a school directory"""
    spell_reagents = defaultdict(list)

    if not spell_school_dir.exists():
        return spell_reagents

    # Scan all circle files
    for circle in range(1, 9):
        circle_file = spell_school_dir / f"Circle{circle}.cs"
        if circle_file.exists():
            reagents = extract_reagents_from_spell(circle_file)
            if reagents:
                spell_reagents[f"Circle{circle}"] = reagents

    return spell_reagents

def validate_all_schools():
    """Validate reagent requirements for all spell schools"""
    print("="*80)
    print("SPELL -> REAGENT VALIDATION")
    print("="*80)
    print()

    total_schools = 0
    schools_with_issues = 0
    total_missing_reagents = set()

    for class_name, school_name in SCHOOL_MAPPING.items():
        total_schools += 1
        print(f"\n{class_name} ({school_name}):")
        print("-"*80)

        # Get available reagents for this school
        available_reagents = get_available_reagents(school_name)
        print(f"  Available reagents: {len(available_reagents)}")

        if available_reagents:
            print(f"    {', '.join(available_reagents[:5])}{'...' if len(available_reagents) > 5 else ''}")

        # Scan spell directory for this school
        spell_dir = SPELL_DIR / class_name
        spell_reagents = scan_spell_directory(spell_dir)

        if not spell_reagents:
            print(f"  [INFO] No reagent requirements found in spell files (may be using default pattern)")
            continue

        # Check for missing reagents
        all_required = set()
        for circle, reagents in spell_reagents.items():
            all_required.update(reagents)

        missing = all_required - set(available_reagents)

        if missing:
            schools_with_issues += 1
            print(f"  [WARN] {len(missing)} referenced reagents not found:")
            for reagent in sorted(missing):
                print(f"    - {reagent}")
                total_missing_reagents.add(reagent)
        else:
            print(f"  [OK] All referenced reagents exist")

    print()
    print("="*80)
    print("SUMMARY:")
    print("="*80)
    print(f"  Schools checked: {total_schools}")
    print(f"  Schools with missing reagents: {schools_with_issues}")
    print(f"  Unique missing reagents: {len(total_missing_reagents)}")

    if total_missing_reagents:
        print(f"\n  All missing reagents:")
        for reagent in sorted(total_missing_reagents):
            print(f"    - {reagent}")

    print("="*80)

    # Generate fix recommendations
    print("\nRECOMMENDATIONS:")
    print("-"*80)

    if schools_with_issues == 0:
        print("  [SUCCESS] All spell reagent references are valid!")
    else:
        print(f"  1. Either update spell files to use existing reagents")
        print(f"  2. Or create the {len(total_missing_reagents)} missing reagent classes")
        print(f"  3. Most likely: Spells are using generic/placeholder reagent names")
        print(f"     that need to be updated to actual implemented reagent names")

    return {
        'total_schools': total_schools,
        'schools_with_issues': schools_with_issues,
        'missing_reagents': total_missing_reagents
    }

if __name__ == "__main__":
    results = validate_all_schools()

    print(f"\n[INFO] Validation complete")
    print(f"[INFO] Check spell files and update reagent references as needed")
