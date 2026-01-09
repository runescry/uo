#!/usr/bin/env python3
"""
Analyze reagent ItemID usage - show which reagents share the same graphics
"""

import os
import re
from collections import defaultdict

# School configurations
SCHOOLS = {
    'IceMagic': 'IceMagicReagents.cs',
    'NatureMagic': 'NatureMagicReagents.cs',
    'HexMagic': 'HexMagicReagents.cs',
    'ElementalMagic': 'ElementalMagicReagents.cs',
    'DarkMagic': 'DarkMagicReagents.cs',
    'DivinationMagic': 'DivinationMagicReagents.cs',
    'Necromancy': 'NecromancyReagents.cs',
    'SummoningMagic': 'SummoningMagicReagents.cs',
    'ShamanicMagic': 'ShamanicMagicReagents.cs',
    'BardicMagic': 'BardicMagicReagents.cs',
    'EnchantingMagic': 'EnchantingMagicReagents.cs',
    'IllusionMagic': 'IllusionMagicReagents.cs',
}

BASE_PATH = r'D:\UO\ServUO\Scripts\Items\Vystia\Resources\Reagents'

# ItemID descriptions (common UO graphics)
ITEMID_DESCRIPTIONS = {
    0x0F00: "Garlic (herb)",
    0x11EA: "Sand/Salt (pile of granules)",
    0x18E0: "Nightshade (herb)",
    0x18E1: "Leaf/herb (small plant)",
    0x18E9: "Petal/flower (blooming flower)",
    0x1AA2: "Moss (clump of moss)",
    0x1AA3: "Vine (green tendril)",
    0x1AA4: "Silk/thread (stringy material)",
    0x1BD7: "Bark (wood piece)",
    0x1CFF: "Feather (large feather)",
    0x19AC: "Ember/coal (glowing coal)",
    0x1015: "Dust/powder (fine powder pile)",
    0x1F13: "Crystal/gem (small crystal)",
    0x1F14: "Rune stone (carved rune)",
    0x1F19: "Large crystal (crystal shard)",
    0x1F1C: "Shard/fragment (crystal piece)",
    0x1F26: "Fang/tooth (creature part)",
    0x1F2C: "Seed (small seed)",
    0x1F2F: "Eye/orb (spherical object)",
    0x1F47: "Pearl (spherical gem)",
    0x1F9D: "Essence/potion ingredient (vial)",
}

def extract_reagent_itemids(school_name, filename):
    """Extract all reagent class names and ItemIDs from a reagent file"""
    file_path = os.path.join(BASE_PATH, filename)
    if not os.path.exists(file_path):
        return []

    with open(file_path, 'r', encoding='utf-8') as f:
        content = f.read()

    reagents = []
    class_pattern = r'public class (\w+) : BaseVystiaReagent'
    classes = re.findall(class_pattern, content)

    for class_name in classes:
        constructor_pattern = rf'{class_name}\(int amount\)\s*:\s*base\(amount,\s*(0x[0-9a-fA-F]+)'
        match = re.search(constructor_pattern, content)

        if match:
            item_id_str = match.group(1)
            item_id = int(item_id_str, 16)
            reagents.append((class_name, item_id, school_name))

    return reagents

def main():
    """Analyze ItemID usage across all reagents"""
    print("="*80)
    print("  VYSTIA REAGENT ITEMID USAGE ANALYSIS")
    print("="*80)
    print()

    # Collect all reagents
    all_reagents = []
    for school_name, filename in SCHOOLS.items():
        reagents = extract_reagent_itemids(school_name, filename)
        all_reagents.extend(reagents)

    # Group by ItemID
    itemid_groups = defaultdict(list)
    for reagent_name, item_id, school_name in all_reagents:
        itemid_groups[item_id].append((reagent_name, school_name))

    # Sort by ItemID
    sorted_itemids = sorted(itemid_groups.keys())

    print(f"Total Reagents: {len(all_reagents)}")
    print(f"Unique ItemIDs Used: {len(sorted_itemids)}")
    print()
    print("="*80)
    print()

    # Show each ItemID and its reagents
    for item_id in sorted_itemids:
        reagents = itemid_groups[item_id]
        count = len(reagents)
        desc = ITEMID_DESCRIPTIONS.get(item_id, "Unknown graphic")

        print(f"ItemID 0x{item_id:04X} - {desc}")
        print(f"  Used by {count} reagent(s):")

        for reagent_name, school_name in sorted(reagents):
            print(f"    - {reagent_name:25s} ({school_name})")

        print()

    # Summary statistics
    print("="*80)
    print("  SUMMARY STATISTICS")
    print("="*80)
    print()

    # Find most reused ItemIDs
    reuse_counts = [(item_id, len(reagents)) for item_id, reagents in itemid_groups.items()]
    reuse_counts.sort(key=lambda x: x[1], reverse=True)

    print("Most Reused ItemIDs:")
    for i, (item_id, count) in enumerate(reuse_counts[:10], 1):
        desc = ITEMID_DESCRIPTIONS.get(item_id, "Unknown")
        print(f"  {i}. 0x{item_id:04X} ({desc:40s}) - {count} reagents")

    print()
    print(f"Total unique graphics: {len(sorted_itemids)}")
    print(f"Average reuse: {len(all_reagents) / len(sorted_itemids):.2f} reagents per graphic")
    print()

if __name__ == "__main__":
    main()
