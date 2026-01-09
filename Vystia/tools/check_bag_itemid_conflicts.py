#!/usr/bin/env python3
"""
Check if any reagent bag contains multiple reagents with the same ItemID
This would be visually confusing for players
"""

import os
import re

REAGENT_PATH = r'D:\UO\ServUO\Scripts\Items\Vystia\Resources\Reagents'

# Map reagent class names to their ItemIDs (from redistribute_reagents_by_appearance.py)
REAGENT_TO_ITEMID = {
    # Ice Magic (8 reagents)
    'Frostbloom': '0x0F86',
    'GlacierCrystal': '0x0F8E',
    'Winterleaf': '0x1A9C',
    'PermafrostEssence': '0x0F0E',
    'ArcticPearl': '0x0F7A',
    'FrozenSoul': '0x0F7D',
    'FrostEssence': '0x0F0E',
    'HeartOfWinter': '0x0F7A',

    # Nature Magic (8 reagents)
    'WildMoss': '0x0F7B',
    'Moonpetal': '0x0F86',
    'DruidBark': '0x0DE1',
    'TreantSap': '0x1C18',
    'ElderwoodSeed': '0x1422',
    'PrimalVine': '0x1A9C',
    'LivingBark': '0x0DE1',
    'AncientRoot': '0x0F86',

    # Hex Magic (8 reagents)
    'BogMoss': '0x0F7B',
    'ViperFang': '0x0F78',
    'Witchweed': '0x1A9C',
    'ToadsEye': '0x0F7A',
    'SwampLotus': '0x0F86',
    'HagsHair': '0x1422',
    'CursedPearl': '0x0F7A',
    'CursedSalt': '0x0F8F',

    # Elemental Magic (8 reagents)
    'AshPetal': '0x0F86',
    'LavaGlass': '0x0F8E',
    'Flameweed': '0x1A9C',
    'MagmaEssence': '0x1C18',
    'PhoenixFeather': '0x0F78',
    'DragonHeart': '0x0F7A',
    'PrimordialEmber': '0x0F8A',
    'ElementalCore': '0x0F8E',

    # Dark Magic (8 reagents)
    'ShadowMoss': '0x0F7B',
    'VoidCrystal': '0x0F8E',
    'VoidWeed': '0x1A9C',
    'ShadowPetal': '0x0F86',
    'VoidDust': '0x0F8F',
    'VoidSilk': '0x0F8D',
    'DemonHeart': '0x0F7A',
    'ShadowEssence': '0x0F7D',

    # Divination Magic (8 reagents)
    'TimeSand': '0x0F8F',
    'TimeDust': '0x0F8F',
    'DivinationDust': '0x0F8F',
    'FateCrystal': '0x0F8E',
    'StarlightCrystal': '0x0F8E',
    'PropheticLeaf': '0x1A9C',
    'SeeingStone': '0x0F7A',
    'FateThread': '0x0F8D',

    # Necromancy (8 reagents)
    'GraveMoss': '0x0F7B',
    'BoneDust': '0x0F8F',
    'CorpseAsh': '0x0F8F',
    'SoulFragment': '0x0F0E',
    'NecroticShroud': '0x1422',
    'LichDust': '0x0F8F',
    'PhylacteryShard': '0x0F8A',
    'ReaperEssence': '0x0F7D',

    # Summoning Magic (8 reagents)
    'PlanarDust': '0x0F8F',
    'EtherShard': '0x0F8A',
    'AetherShard': '0x0F8A',
    'SummoningCrystal': '0x0F8E',
    'ChaosShard': '0x0F8A',
    'BindingRune': '0x0F8A',
    'DimensionalKey': '0x0F7A',
    'SummoningSalt': '0x0F8F',

    # Shamanic Magic (8 reagents)
    'LightningRoot': '0x0F86',
    'ThunderMoss': '0x0F7B',
    'StormCrystal': '0x0F8E',
    'StormEssence': '0x1C18',
    'SpiritFeather': '0x0F78',
    'PrimalThunder': '0x0F8A',
    'TotemCarving': '0x0DE1',
    'WindEssence': '0x1C18',

    # Bardic Magic (8 reagents)
    'SongPetal': '0x0F86',
    'EchoDust': '0x0F8F',
    'VoiceCrystal': '0x0F8E',
    'MuseEssence': '0x1C18',
    'HarmonyGem': '0x0F7A',
    'EternalNote': '0x0F8D',
    'GoldenString': '0x0F8D',
    'DragonScale': '0x0F78',

    # Enchanting Magic (8 reagents)
    'ArcaneDust': '0x0F8F',
    'EssenceOfMagic': '0x1C18',
    'ManaCrystal': '0x0F8E',
    'LeyLineEssence': '0x0F7D',
    'LeyLineShard': '0x0F8A',
    'RuneFragment': '0x0F8A',
    'RunicPowder': '0x0F8F',
    'TitanRune': '0x0F8A',

    # Illusion Magic (8 reagents)
    'MirrorDust': '0x0F8F',
    'PhantomSilk': '0x0F8D',
    'MirageEssence': '0x0F0E',
    'DreamCrystal': '0x0F8E',
    'RealitySplinter': '0x0F8A',
    'VoidMirror': '0x0F7A',
    'ChaosPrism': '0x0F8E',
    'PhantomPetal': '0x0F86',
}

# Define which reagents go in which bags (from VystiaReagentBags.cs)
BAGS = {
    'Ice Magic': ['Frostbloom', 'GlacierCrystal', 'Winterleaf', 'PermafrostEssence',
                  'ArcticPearl', 'FrozenSoul', 'FrostEssence', 'HeartOfWinter'],
    'Nature Magic': ['WildMoss', 'Moonpetal', 'DruidBark', 'TreantSap',
                     'ElderwoodSeed', 'PrimalVine', 'LivingBark', 'AncientRoot'],
    'Hex Magic': ['BogMoss', 'ViperFang', 'Witchweed', 'ToadsEye',
                  'SwampLotus', 'HagsHair', 'CursedPearl', 'CursedSalt'],
    'Elemental Magic': ['AshPetal', 'LavaGlass', 'Flameweed', 'MagmaEssence',
                        'PhoenixFeather', 'DragonHeart', 'PrimordialEmber', 'ElementalCore'],
    'Dark Magic': ['ShadowMoss', 'VoidCrystal', 'VoidWeed', 'ShadowPetal',
                   'VoidDust', 'VoidSilk', 'DemonHeart', 'ShadowEssence'],
    'Divination Magic': ['TimeSand', 'TimeDust', 'DivinationDust', 'FateCrystal',
                         'StarlightCrystal', 'PropheticLeaf', 'SeeingStone', 'FateThread'],
    'Necromancy': ['GraveMoss', 'BoneDust', 'CorpseAsh', 'SoulFragment',
                   'NecroticShroud', 'LichDust', 'PhylacteryShard', 'ReaperEssence'],
    'Summoning Magic': ['PlanarDust', 'EtherShard', 'AetherShard', 'SummoningCrystal',
                        'ChaosShard', 'BindingRune', 'DimensionalKey', 'SummoningSalt'],
    'Shamanic Magic': ['LightningRoot', 'ThunderMoss', 'StormCrystal', 'StormEssence',
                       'SpiritFeather', 'PrimalThunder', 'TotemCarving', 'WindEssence'],
    'Bardic Magic': ['SongPetal', 'EchoDust', 'VoiceCrystal', 'MuseEssence',
                     'HarmonyGem', 'EternalNote', 'GoldenString', 'DragonScale'],
    'Enchanting Magic': ['ArcaneDust', 'EssenceOfMagic', 'ManaCrystal', 'LeyLineEssence',
                         'LeyLineShard', 'RuneFragment', 'RunicPowder', 'TitanRune'],
    'Illusion Magic': ['MirrorDust', 'PhantomSilk', 'MirageEssence', 'DreamCrystal',
                       'RealitySplinter', 'VoidMirror', 'ChaosPrism', 'PhantomPetal'],
}

def check_bag_conflicts():
    """Check each bag for duplicate ItemIDs"""

    print("="*70)
    print("  CHECKING REAGENT BAG ITEMID CONFLICTS")
    print("="*70)
    print()

    total_conflicts = 0
    bags_with_conflicts = []

    for school, reagents in BAGS.items():
        print(f"{school} Bag:")

        # Count ItemIDs in this bag
        itemid_counts = {}
        for reagent in reagents:
            itemid = REAGENT_TO_ITEMID.get(reagent, 'UNKNOWN')
            if itemid not in itemid_counts:
                itemid_counts[itemid] = []
            itemid_counts[itemid].append(reagent)

        # Check for duplicates
        has_conflict = False
        for itemid, reagent_list in sorted(itemid_counts.items()):
            if len(reagent_list) > 1:
                print(f"  [CONFLICT] {itemid}: {', '.join(reagent_list)}")
                has_conflict = True
                total_conflicts += len(reagent_list) - 1
            else:
                print(f"  [OK] {itemid}: {reagent_list[0]}")

        if has_conflict:
            bags_with_conflicts.append(school)

        print()

    print("="*70)
    print(f"  SUMMARY")
    print("="*70)
    print(f"Bags with conflicts: {len(bags_with_conflicts)}/12")
    print(f"Total ItemID conflicts: {total_conflicts}")
    print()

    if bags_with_conflicts:
        print("BAGS WITH CONFLICTS:")
        for school in bags_with_conflicts:
            print(f"  - {school}")
        print()
        print("ACTION REQUIRED: Reagents need to be reassigned to use unique ItemIDs per bag")
    else:
        print("All bags are conflict-free!")
    print()

if __name__ == "__main__":
    check_bag_conflicts()
