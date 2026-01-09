#!/usr/bin/env python3
"""
Comprehensive reagent audit for all 12 Vystia magic schools
Checks: Defined reagents vs Bag contents vs Spell requirements
"""

import os
import re
from collections import Counter

# School configurations
SCHOOLS = {
    'IceMage': {'spell_dir': 'IceMage', 'reagent_file': 'IceMagicReagents.cs', 'bag_class': 'IceMagicReagentBag'},
    'Druid': {'spell_dir': 'Druid', 'reagent_file': 'NatureMagicReagents.cs', 'bag_class': 'NatureMagicReagentBag'},
    'Witch': {'spell_dir': 'Witch', 'reagent_file': 'HexMagicReagents.cs', 'bag_class': 'HexMagicReagentBag'},
    'Sorcerer': {'spell_dir': 'Sorcerer', 'reagent_file': 'ElementalMagicReagents.cs', 'bag_class': 'ElementalMagicReagentBag'},
    'Warlock': {'spell_dir': 'Warlock', 'reagent_file': 'DarkMagicReagents.cs', 'bag_class': 'DarkMagicReagentBag'},
    'Oracle': {'spell_dir': 'Oracle', 'reagent_file': 'DivinationMagicReagents.cs', 'bag_class': 'DivinationMagicReagentBag'},
    'Necromancer': {'spell_dir': 'Necromancer', 'reagent_file': 'NecromancyReagents.cs', 'bag_class': 'NecromancyReagentBag'},
    'Summoner': {'spell_dir': 'Summoner', 'reagent_file': 'SummoningMagicReagents.cs', 'bag_class': 'SummoningMagicReagentBag'},
    'Shaman': {'spell_dir': 'Shaman', 'reagent_file': 'ShamanicMagicReagents.cs', 'bag_class': 'ShamanicMagicReagentBag'},
    'Bard': {'spell_dir': 'Bard', 'reagent_file': 'BardicMagicReagents.cs', 'bag_class': 'BardicMagicReagentBag'},
    'Enchanter': {'spell_dir': 'Enchanter', 'reagent_file': 'EnchantingMagicReagents.cs', 'bag_class': 'EnchantingMagicReagentBag'},
    'Illusionist': {'spell_dir': 'Illusionist', 'reagent_file': 'IllusionMagicReagents.cs', 'bag_class': 'IllusionMagicReagentBag'},
}

BASE_PATH = r'D:\UO\ServUO\Scripts'
SPELLS_PATH = os.path.join(BASE_PATH, 'Custom', 'VystiaClasses', 'Spells')
REAGENTS_PATH = os.path.join(BASE_PATH, 'Items', 'Vystia', 'Resources', 'Reagents')
BAG_FILE = os.path.join(REAGENTS_PATH, 'VystiaReagentBags.cs')

def get_defined_reagents(school_config):
    """Extract reagent class names from reagent definition file"""
    reagent_file = os.path.join(REAGENTS_PATH, school_config['reagent_file'])
    if not os.path.exists(reagent_file):
        return []

    with open(reagent_file, 'r', encoding='utf-8') as f:
        content = f.read()

    # Find all class definitions
    classes = re.findall(r'public class (\w+) : BaseVystiaReagent', content)
    return classes

def get_bag_reagents(school_config):
    """Extract reagents from bag DropItem calls"""
    if not os.path.exists(BAG_FILE):
        return []

    with open(BAG_FILE, 'r', encoding='utf-8') as f:
        content = f.read()

    # Find the bag class section
    bag_pattern = rf'public class {school_config["bag_class"]}.*?DropItem\(new ([^(]+)\(\d+\)\);'
    matches = re.findall(bag_pattern, content, re.DOTALL)

    # Extract individual DropItem calls
    bag_section_match = re.search(rf'public class {school_config["bag_class"]}[^{{]*\{{[^{{]*\{{[^}}]*\{{([^}}]+)\}}', content, re.DOTALL)
    if bag_section_match:
        bag_content = bag_section_match.group(1)
        reagents = re.findall(r'DropItem\(new (\w+)\(\d+\)\);', bag_content)
        return reagents

    return []

def get_spell_reagents(school_config):
    """Extract reagent types used in spells"""
    spell_dir = os.path.join(SPELLS_PATH, school_config['spell_dir'])
    if not os.path.exists(spell_dir):
        return Counter()

    reagents = []
    for file in os.listdir(spell_dir):
        if file.endswith('.cs'):
            with open(os.path.join(spell_dir, file), 'r', encoding='utf-8') as f:
                content = f.read()
                # Find typeof() calls in reagent checks
                matches = re.findall(r'typeof\(([A-Z][a-zA-Z]+)\)', content)
                # Filter out non-reagent types
                reagents.extend([m for m in matches if not m.endswith('Spell') and
                                m not in ['InternalTarget', 'Mobile', 'IDamageable',
                                         'BaseCreature', 'Container', 'StatMod', 'Timer']])

    return Counter(reagents)

def audit_school(school_name, school_config):
    """Audit a single magic school"""
    print(f"\n{'='*70}")
    print(f"  {school_name}")
    print(f"{'='*70}")

    # Get all three sources
    defined = set(get_defined_reagents(school_config))
    in_bag = set(get_bag_reagents(school_config))
    used_by_spells = get_spell_reagents(school_config)

    print(f"\nDefined Reagents ({len(defined)}):")
    for reagent in sorted(defined):
        print(f"  ✓ {reagent}")

    print(f"\nIn Spawn Bag ({len(in_bag)}):")
    for reagent in sorted(in_bag):
        status = "✓" if reagent in defined else "❌ NOT DEFINED"
        print(f"  {status} {reagent}")

    print(f"\nUsed by Spells ({len(used_by_spells)} unique):")
    for reagent, count in used_by_spells.most_common():
        in_def = "✓" if reagent in defined else "❌"
        in_bag_status = "✓" if reagent in in_bag else "❌"
        print(f"  {count:3d} spells | Defined:{in_def} | In Bag:{in_bag_status} | {reagent}")

    # Check for issues
    issues = []

    # Check if bag has all defined reagents
    missing_from_bag = defined - in_bag
    if missing_from_bag:
        issues.append(f"❌ Missing from bag: {', '.join(missing_from_bag)}")

    # Check if bag has undefined reagents
    undefined_in_bag = in_bag - defined
    if undefined_in_bag:
        issues.append(f"❌ In bag but not defined: {', '.join(undefined_in_bag)}")

    # Check if spells use undefined reagents
    used_reagents = set(used_by_spells.keys())
    undefined_used = used_reagents - defined
    if undefined_used:
        issues.append(f"❌ Used by spells but not defined: {', '.join(undefined_used)}")

    # Check if defined reagents are unused
    unused_defined = defined - used_reagents
    if unused_defined:
        issues.append(f"⚠️  Defined but never used: {', '.join(unused_defined)}")

    # Report
    if issues:
        print(f"\n{'!'*70}")
        print("ISSUES FOUND:")
        for issue in issues:
            print(f"  {issue}")
        print(f"{'!'*70}")
        return False
    else:
        print(f"\n{'✓'*70}")
        print("✅ ALL REAGENTS MATCH - No issues found")
        print(f"{'✓'*70}")
        return True

def main():
    """Audit all schools"""
    print("="*70)
    print("  VYSTIA MAGIC REAGENT AUDIT - ALL 12 SCHOOLS")
    print("="*70)

    results = {}
    for school_name, school_config in SCHOOLS.items():
        try:
            results[school_name] = audit_school(school_name, school_config)
        except Exception as e:
            print(f"\n❌ ERROR auditing {school_name}: {e}")
            results[school_name] = False

    # Summary
    print(f"\n\n{'='*70}")
    print("  AUDIT SUMMARY")
    print(f"{'='*70}\n")

    passed = sum(1 for v in results.values() if v)
    failed = len(results) - passed

    for school, status in results.items():
        icon = "✅" if status else "❌"
        print(f"  {icon} {school}")

    print(f"\n{'='*70}")
    print(f"  PASSED: {passed}/12 schools")
    print(f"  FAILED: {failed}/12 schools")
    print(f"{'='*70}\n")

    if failed == 0:
        print("✅ ALL SCHOOLS PASSED - Reagent system is consistent!")
    else:
        print(f"⚠️  {failed} school(s) need fixes")

if __name__ == "__main__":
    main()
