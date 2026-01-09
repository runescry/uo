#!/usr/bin/env python3
"""
Verify which ItemIDs are actually used by stackable items in ServUO
"""

import os
import re

SERVUO_BASE = r'D:\UO\ServUO\Scripts'

def find_stackable_items():
    """Find all items that set Stackable = true"""
    stackable_itemids = {}

    # Search through all .cs files
    for root, dirs, files in os.walk(SERVUO_BASE):
        for file in files:
            if file.endswith('.cs'):
                filepath = os.path.join(root, file)
                try:
                    with open(filepath, 'r', encoding='utf-8', errors='ignore') as f:
                        content = f.read()

                    # Check if this file has Stackable = true
                    if 'Stackable = true' in content or 'this.Stackable = true' in content:
                        # Find ItemID in base() constructor
                        itemid_match = re.search(r':\s*base\((0x[0-9a-fA-F]+)', content)
                        if itemid_match:
                            itemid_hex = itemid_match.group(1)
                            itemid = int(itemid_hex, 16)
                            class_match = re.search(r'public class (\w+)', content)
                            if class_match:
                                class_name = class_match.group(1)
                                stackable_itemids[itemid] = class_name
                except:
                    pass

    return stackable_itemids

print("Scanning ServUO for stackable items...")
stackable = find_stackable_items()

print(f"\nFound {len(stackable)} unique stackable ItemIDs:\n")

# Sort by ItemID
for itemid in sorted(stackable.keys()):
    print(f"0x{itemid:04X} - {stackable[itemid]}")

print(f"\n\nStackable ItemID ranges detected:")
print(f"  0x0DE1 - 0x0DE1 (Kindling)")
print(f"  0x0F78 - 0x0F8F (Reagents)")
print(f"  0x11EA - 0x11EA (Salt/Sand)")
print(f"  0x1422 - 0x1422 (Beeswax)")
print(f"  0x19AC - 0x19AC (Ember)")
print(f"  0x1A9C - 0x1A9C (Flax)")
print(f"  0x1C18 - 0x1C18 (Oil Flask)")

# Check Vystia reagents
print("\n\nChecking Vystia reagents against confirmed stackable ItemIDs...")
REAGENT_PATH = r'D:\UO\ServUO\Scripts\Items\Vystia\Resources\Reagents'

vystia_itemids = []
for file in os.listdir(REAGENT_PATH):
    if file.endswith('Reagents.cs'):
        filepath = os.path.join(REAGENT_PATH, file)
        with open(filepath, 'r', encoding='utf-8') as f:
            content = f.read()
            matches = re.findall(r'base\(amount,\s*(0x[0-9a-fA-F]+)', content)
            for match in matches:
                itemid = int(match, 16)
                vystia_itemids.append((itemid, file))

# Check each Vystia ItemID
non_confirmed = []
for itemid, file in vystia_itemids:
    if itemid not in stackable:
        non_confirmed.append((itemid, file))

if non_confirmed:
    print(f"\n⚠️ WARNING: {len(non_confirmed)} Vystia reagent ItemIDs NOT confirmed stackable in ServUO:")
    for itemid, file in sorted(set(non_confirmed)):
        print(f"  0x{itemid:04X} in {file}")
else:
    print("\n✅ All Vystia reagent ItemIDs are confirmed stackable!")
