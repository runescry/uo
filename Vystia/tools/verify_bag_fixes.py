#!/usr/bin/env python3
"""
Verify that all reagent bags now have 8 unique ItemIDs
Read directly from the files instead of hardcoded mapping
"""

import os
import re

REAGENT_PATH = r'D:\UO\ServUO\Scripts\Items\Vystia\Resources\Reagents'

# Map filename to bag name
FILE_TO_BAG = {
    'IceMagicReagents.cs': 'Ice Magic',
    'NatureMagicReagents.cs': 'Nature Magic',
    'HexMagicReagents.cs': 'Hex Magic',
    'ElementalMagicReagents.cs': 'Elemental Magic',
    'DarkMagicReagents.cs': 'Dark Magic',
    'DivinationMagicReagents.cs': 'Divination Magic',
    'NecromancyReagents.cs': 'Necromancy',
    'SummoningMagicReagents.cs': 'Summoning Magic',
    'ShamanicMagicReagents.cs': 'Shamanic Magic',
    'BardicMagicReagents.cs': 'Bardic Magic',
    'EnchantingMagicReagents.cs': 'Enchanting Magic',
    'IllusionMagicReagents.cs': 'Illusion Magic',
}

def extract_itemids_from_file(filepath):
    """Extract all ItemIDs from a reagent file"""
    with open(filepath, 'r', encoding='utf-8') as f:
        content = f.read()

    # Find all ItemIDs in base() calls
    pattern = r'base\(amount,\s*(0x[0-9a-fA-F]+)'
    itemids = re.findall(pattern, content)

    return itemids

def main():
    print("="*70)
    print("  VERIFYING REAGENT BAG ITEMID FIXES")
    print("="*70)
    print()

    all_good = True
    bags_with_issues = []

    for filename, bag_name in FILE_TO_BAG.items():
        filepath = os.path.join(REAGENT_PATH, filename)

        if not os.path.exists(filepath):
            print(f"{bag_name}: FILE NOT FOUND")
            all_good = False
            continue

        itemids = extract_itemids_from_file(filepath)

        # Check for duplicates
        unique_itemids = set(itemids)

        if len(itemids) != 8:
            print(f"{bag_name}: ERROR - Found {len(itemids)} ItemIDs (expected 8)")
            all_good = False
            bags_with_issues.append(bag_name)
        elif len(unique_itemids) != 8:
            print(f"{bag_name}: CONFLICT - {len(itemids)} ItemIDs but only {len(unique_itemids)} unique")
            # Show which are duplicated
            from collections import Counter
            counts = Counter(itemids)
            for itemid, count in sorted(counts.items()):
                status = f"({count}x)" if count > 1 else "OK"
                print(f"  {status} {itemid}")
            all_good = False
            bags_with_issues.append(bag_name)
        else:
            print(f"{bag_name}: OK - 8 unique ItemIDs")
            for itemid in sorted(unique_itemids):
                print(f"  {itemid}")

        print()

    print("="*70)
    print("  SUMMARY")
    print("="*70)

    if all_good:
        print("SUCCESS: All 12 bags have 8 unique ItemIDs!")
    else:
        print(f"ISSUES FOUND: {len(bags_with_issues)} bags have problems")
        for bag in bags_with_issues:
            print(f"  - {bag}")

    print()

if __name__ == "__main__":
    main()
