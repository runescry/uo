#!/usr/bin/env python3
"""
Replace ALL Vystia reagent ItemIDs with confirmed stackable ones
"""

import os
import re

REAGENT_PATH = r'D:\UO\ServUO\Scripts\Items\Vystia\Resources\Reagents'

# Confirmed stackable ItemIDs
STACKABLE_ITEMIDS = {
    # Standard UO Magery Reagents
    '0x0F7A': 'black pearl',
    '0x0F7B': 'bloodmoss',
    '0x0F84': 'garlic',
    '0x0F85': 'ginseng',
    '0x0F86': 'mandrake root',
    '0x0F88': 'nightshade',
    '0x0F8C': 'sulfurous ash',
    '0x0F8D': 'spider silk',

    # Standard UO Necromancy Reagents
    '0x0F7D': 'daemon blood (vial/liquid)',
    '0x0F78': 'batwing (wing/feather)',
    '0x0F8A': 'pig iron (metal/ore)',
    '0x0F8E': 'nox crystal (crystal/gem)',
    '0x0F8F': 'grave dust (powder/dust)',

    # Crafting Resources
    '0x1422': 'beeswax (wax/organic)',
    '0x1A9C': 'flax bundle (fiber/plant)',
    '0x0DE1': 'kindling (wood/bark)',
    '0x1C18': 'oil flask (flask/bottle)',

    # Potion Bottles (user requested for vials/essences)
    '0x0F0E': 'empty bottle (potion bottle)',
}

# Mapping strategy based on reagent names
# Now using ALL confirmed stackable ItemIDs including standard magery reagents
ITEMID_MAPPING = {
    # Dusts/Powders -> Distribute between Grave Dust, Sulfurous Ash
    '0x0F8F': '0x0F8F',  # Keep grave dust
    '0x0F8C': '0x0F8C',  # Keep sulfurous ash

    # Crystals/Gems -> Distribute between Nox Crystal, Black Pearl
    '0x1F19': '0x0F8E',  # Large crystals -> Nox Crystal
    '0x1F13': '0x0F7A',  # Crystal/gem -> Black Pearl
    '0x0F8E': '0x0F8E',  # Keep nox crystal
    '0x0F7A': '0x0F7A',  # Keep black pearl

    # Shards/Fragments -> Pig Iron
    '0x1F1C': '0x0F8A',  # Shards/fragments
    '0x1F14': '0x0F8A',  # Rune stones

    # Herbs/Leaves/Plants -> Distribute between Flax, Nightshade, Ginseng, Garlic
    '0x18E1': '0x0F88',  # Leaf/herb -> Nightshade
    '0x1A9C': '0x1A9C',  # Keep flax
    '0x0F88': '0x0F88',  # Keep nightshade
    '0x0F85': '0x0F85',  # Keep ginseng
    '0x0F84': '0x0F84',  # Keep garlic

    # Petals/Flowers -> Mandrake Root
    '0x18E9': '0x0F86',  # Petal/flower -> Mandrake Root
    '0x0F86': '0x0F86',  # Keep mandrake root

    # Moss -> Bloodmoss
    '0x1AA2': '0x0F7B',  # Moss -> Bloodmoss
    '0x0F7B': '0x0F7B',  # Keep bloodmoss

    # Silk/Thread -> Spider Silk
    '0x1AA4': '0x0F8D',  # Silk/thread -> Spider Silk
    '0x1AA3': '0x0F8D',  # Vine -> Spider Silk
    '0x0F8D': '0x0F8D',  # Keep spider silk

    # Bark/Wood -> Kindling
    '0x1BD7': '0x0DE1',  # Bark
    '0x0DE1': '0x0DE1',  # Keep kindling

    # Feathers -> Batwing
    '0x1CFF': '0x0F78',  # Feather
    '0x0F78': '0x0F78',  # Keep batwing

    # Fangs/Teeth -> Batwing
    '0x1F26': '0x0F78',  # Fang/tooth

    # Seeds -> Beeswax
    '0x1F2C': '0x1422',  # Seed
    '0x1422': '0x1422',  # Keep beeswax

    # Eyes/Orbs -> Black Pearl
    '0x1F2F': '0x0F7A',  # Eye/orb -> Black Pearl

    # Pearls -> Black Pearl
    '0x1F47': '0x0F7A',  # Pearl -> Black Pearl

    # Essences/Vials -> Potion Bottle
    '0x1F9D': '0x0F0E',  # Essence/vial -> Potion Bottle
    '0x0F7D': '0x0F7D',  # Keep daemon blood
    '0x1C18': '0x1C18',  # Keep oil flask
    '0x0F0E': '0x0F0E',  # Keep potion bottle

    # Sand/Salt/Granules -> Sulfurous Ash
    '0x11EA': '0x0F8C',  # Sand/salt -> Sulfurous Ash

    # Ember/Coal -> Pig Iron
    '0x19AC': '0x0F8A',  # Ember/coal
    '0x0F8A': '0x0F8A',  # Keep pig iron
}

def fix_file(filepath):
    """Replace all non-stackable ItemIDs in a file"""
    with open(filepath, 'r', encoding='utf-8') as f:
        content = f.read()

    original = content
    replacements = 0

    # Replace each ItemID
    for old_id, new_id in ITEMID_MAPPING.items():
        if old_id != new_id:  # Skip if already correct
            # Case-insensitive replacement
            pattern = re.compile(rf'base\(amount,\s*{old_id}', re.IGNORECASE)
            new_content = pattern.sub(f'base(amount, {new_id}', content)
            if new_content != content:
                count = len(pattern.findall(content))
                replacements += count
                content = new_content
                print(f"  Replaced {old_id} -> {new_id} ({count} occurrences)")

    if content != original:
        with open(filepath, 'w', encoding='utf-8') as f:
            f.write(content)
        return replacements
    return 0

def main():
    print("="*70)
    print("  FIXING ALL VYSTIA REAGENT ITEMIDS TO CONFIRMED STACKABLE")
    print("="*70)
    print()
    print("Using ONLY these confirmed stackable ItemIDs:")
    for itemid, desc in STACKABLE_ITEMIDS.items():
        print(f"  {itemid} - {desc}")
    print()
    print("="*70)
    print()

    total_replacements = 0

    for filename in os.listdir(REAGENT_PATH):
        if filename.endswith('Reagents.cs'):
            filepath = os.path.join(REAGENT_PATH, filename)
            print(f"Processing {filename}...")
            replacements = fix_file(filepath)
            total_replacements += replacements
            if replacements == 0:
                print("  (no changes needed)")
            print()

    print("="*70)
    print(f"  TOTAL REPLACEMENTS: {total_replacements}")
    print("="*70)
    print()
    print("All reagents now use ONLY confirmed stackable ItemIDs!")
    print()

if __name__ == "__main__":
    main()
