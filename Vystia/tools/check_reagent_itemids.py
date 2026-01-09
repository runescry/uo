#!/usr/bin/env python3
"""
Check all 96 Vystia reagent ItemIDs for stackability
Stackable ItemID ranges in UO: 0x0F00-0x0FFF, 0x1000-0x1FFF, 0x18E0-0x18FF
"""

import os
import re

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

# Stackable ItemID ranges in UO
STACKABLE_RANGES = [
    (0x0F00, 0x0FFF),  # Main stackable range
    (0x1000, 0x1FFF),  # Extended stackable range
    (0x18E0, 0x18FF),  # Smaller stackable range
]

def is_stackable(item_id):
    """Check if ItemID falls within stackable ranges"""
    for start, end in STACKABLE_RANGES:
        if start <= item_id <= end:
            return True
    return False

def extract_reagent_itemids(school_name, filename):
    """Extract all reagent class names and ItemIDs from a reagent file"""
    file_path = os.path.join(BASE_PATH, filename)
    if not os.path.exists(file_path):
        return []

    with open(file_path, 'r', encoding='utf-8') as f:
        content = f.read()

    # Pattern: public class [ClassName] : BaseVystiaReagent { ... base(amount, 0xXXXX, ...
    reagents = []

    # Find all class definitions
    class_pattern = r'public class (\w+) : BaseVystiaReagent'
    classes = re.findall(class_pattern, content)

    for class_name in classes:
        # Find the constructor for this class
        constructor_pattern = rf'{class_name}\(int amount\)\s*:\s*base\(amount,\s*(0x[0-9a-fA-F]+)'
        match = re.search(constructor_pattern, content)

        if match:
            item_id_str = match.group(1)
            item_id = int(item_id_str, 16)
            reagents.append((class_name, item_id))

    return reagents

def main():
    """Check all reagent ItemIDs for stackability"""
    print("="*70)
    print("  VYSTIA REAGENT ITEMID STACKABILITY CHECK")
    print("="*70)
    print()
    print("Stackable ItemID ranges:")
    print("  0x0F00 - 0x0FFF  (Main range)")
    print("  0x1000 - 0x1FFF  (Extended range)")
    print("  0x18E0 - 0x18FF  (Small range)")
    print()
    print("="*70)
    print()

    all_stackable = True
    non_stackable_count = 0
    total_count = 0

    for school_name, filename in SCHOOLS.items():
        print(f"{school_name} (from {filename}):")

        reagents = extract_reagent_itemids(school_name, filename)

        if not reagents:
            print(f"  [!] Could not extract reagents from {filename}")
            continue

        for reagent_name, item_id in reagents:
            total_count += 1
            stackable = is_stackable(item_id)

            if stackable:
                status = 'OK'
                stackable_text = 'STACKABLE'
            else:
                status = '(!)'
                stackable_text = 'NON-STACKABLE'
                all_stackable = False
                non_stackable_count += 1

            print(f"  [{status}] {reagent_name:25s} ItemID: 0x{item_id:04X}  {stackable_text}")

        print()

    # Summary
    print("="*70)
    print("  SUMMARY")
    print("="*70)
    print(f"Total Reagents Checked: {total_count}/96")
    print(f"Stackable: {total_count - non_stackable_count}")
    print(f"Non-Stackable: {non_stackable_count}")
    print()

    if all_stackable:
        print("[OK] ALL REAGENTS USE STACKABLE ITEMIDS!")
    else:
        print(f"[!] {non_stackable_count} reagent(s) need ItemID changes to stackable graphics")
    print()

if __name__ == "__main__":
    main()
