#!/usr/bin/env python3
"""
Fix reagent ItemID assignments to ensure no bag has duplicate ItemIDs
Each bag must have 8 reagents with 8 unique ItemIDs for visual clarity
"""

import os
import re

REAGENT_PATH = r'D:\UO\ServUO\Scripts\Items\Vystia\Resources\Reagents'

# All 14 confirmed stackable ItemIDs
ALL_ITEMIDS = [
    '0x0F7D',  # daemon blood
    '0x0F78',  # batwing
    '0x0F8A',  # pig iron
    '0x0F8E',  # nox crystal
    '0x1422',  # beeswax
    '0x1A9C',  # flax bundle
    '0x0F8F',  # grave dust
    '0x0DE1',  # kindling
    '0x1C18',  # oil flask
    '0x0F7A',  # black pearl
    '0x0F7B',  # bloodmoss
    '0x0F8D',  # spider silk
    '0x0F0E',  # potion bottle
    '0x0F86',  # mandrake root
]

# Define which reagents go in which bags
BAGS = {
    'IceMagic': ['Frostbloom', 'GlacierCrystal', 'Winterleaf', 'PermafrostEssence',
                 'ArcticPearl', 'FrozenSoul', 'FrostEssence', 'HeartOfWinter'],
    'NatureMagic': ['WildMoss', 'Moonpetal', 'DruidBark', 'TreantSap',
                    'ElderwoodSeed', 'PrimalVine', 'LivingBark', 'AncientRoot'],
    'HexMagic': ['BogMoss', 'ViperFang', 'Witchweed', 'ToadsEye',
                 'SwampLotus', 'HagsHair', 'CursedPearl', 'CursedSalt'],
    'ElementalMagic': ['AshPetal', 'LavaGlass', 'Flameweed', 'MagmaEssence',
                       'PhoenixFeather', 'DragonHeart', 'PrimordialEmber', 'ElementalCore'],
    'DarkMagic': ['ShadowMoss', 'VoidCrystal', 'VoidWeed', 'ShadowPetal',
                  'VoidDust', 'VoidSilk', 'DemonHeart', 'ShadowEssence'],
    'DivinationMagic': ['TimeSand', 'TimeDust', 'DivinationDust', 'FateCrystal',
                        'StarlightCrystal', 'PropheticLeaf', 'SeeingStone', 'FateThread'],
    'Necromancy': ['GraveMoss', 'BoneDust', 'CorpseAsh', 'SoulFragment',
                   'NecroticShroud', 'LichDust', 'PhylacteryShard', 'ReaperEssence'],
    'SummoningMagic': ['PlanarDust', 'EtherShard', 'AetherShard', 'SummoningCrystal',
                       'ChaosShard', 'BindingRune', 'DimensionalKey', 'SummoningSalt'],
    'ShamanicMagic': ['LightningRoot', 'ThunderMoss', 'StormCrystal', 'StormEssence',
                      'SpiritFeather', 'PrimalThunder', 'TotemCarving', 'WindEssence'],
    'BardicMagic': ['SongPetal', 'EchoDust', 'VoiceCrystal', 'MuseEssence',
                    'HarmonyGem', 'EternalNote', 'GoldenString', 'DragonScale'],
    'EnchantingMagic': ['ArcaneDust', 'EssenceOfMagic', 'ManaCrystal', 'LeyLineEssence',
                        'LeyLineShard', 'RuneFragment', 'RunicPowder', 'TitanRune'],
    'IllusionMagic': ['MirrorDust', 'PhantomSilk', 'MirageEssence', 'DreamCrystal',
                      'RealitySplinter', 'VoidMirror', 'ChaosPrism', 'PhantomPetal'],
}

# New mapping ensuring 8 unique ItemIDs per bag
NEW_MAPPING = {
    # Ice Magic - 8 unique ItemIDs
    'Frostbloom': '0x0F86',       # mandrake root
    'GlacierCrystal': '0x0F8E',   # nox crystal
    'Winterleaf': '0x1A9C',       # flax bundle
    'PermafrostEssence': '0x0F0E', # potion bottle
    'ArcticPearl': '0x0F7A',      # black pearl
    'FrozenSoul': '0x0F7D',       # daemon blood
    'FrostEssence': '0x1C18',     # oil flask (CHANGED from 0x0F0E)
    'HeartOfWinter': '0x0F7B',    # bloodmoss (CHANGED from 0x0F7A)

    # Nature Magic - 8 unique ItemIDs
    'WildMoss': '0x0F7B',         # bloodmoss
    'Moonpetal': '0x0F86',        # mandrake root
    'DruidBark': '0x0DE1',        # kindling
    'TreantSap': '0x1C18',        # oil flask
    'ElderwoodSeed': '0x1422',    # beeswax
    'PrimalVine': '0x1A9C',       # flax bundle
    'LivingBark': '0x0F78',       # batwing (CHANGED from 0x0DE1)
    'AncientRoot': '0x0F7A',      # black pearl (CHANGED from 0x0F86)

    # Hex Magic - 8 unique ItemIDs
    'BogMoss': '0x0F7B',          # bloodmoss
    'ViperFang': '0x0F78',        # batwing
    'Witchweed': '0x1A9C',        # flax bundle
    'ToadsEye': '0x0F7A',         # black pearl
    'SwampLotus': '0x0F86',       # mandrake root
    'HagsHair': '0x1422',         # beeswax
    'CursedPearl': '0x0F8E',      # nox crystal (CHANGED from 0x0F7A)
    'CursedSalt': '0x0F8F',       # grave dust

    # Elemental Magic - 8 unique ItemIDs
    'AshPetal': '0x0F86',         # mandrake root
    'LavaGlass': '0x0F8E',        # nox crystal
    'Flameweed': '0x1A9C',        # flax bundle
    'MagmaEssence': '0x1C18',     # oil flask
    'PhoenixFeather': '0x0F78',   # batwing
    'DragonHeart': '0x0F7A',      # black pearl
    'PrimordialEmber': '0x0F8A',  # pig iron
    'ElementalCore': '0x0F7D',    # daemon blood (CHANGED from 0x0F8E)

    # Dark Magic - 8 unique ItemIDs (already perfect!)
    'ShadowMoss': '0x0F7B',       # bloodmoss
    'VoidCrystal': '0x0F8E',      # nox crystal
    'VoidWeed': '0x1A9C',         # flax bundle
    'ShadowPetal': '0x0F86',      # mandrake root
    'VoidDust': '0x0F8F',         # grave dust
    'VoidSilk': '0x0F8D',         # spider silk
    'DemonHeart': '0x0F7A',       # black pearl
    'ShadowEssence': '0x0F7D',    # daemon blood

    # Divination Magic - 8 unique ItemIDs
    'TimeSand': '0x0F8F',         # grave dust
    'TimeDust': '0x0F7D',         # daemon blood (CHANGED from 0x0F8F)
    'DivinationDust': '0x0DE1',   # kindling (CHANGED from 0x0F8F)
    'FateCrystal': '0x0F8E',      # nox crystal
    'StarlightCrystal': '0x0F0E', # potion bottle (CHANGED from 0x0F8E)
    'PropheticLeaf': '0x1A9C',    # flax bundle
    'SeeingStone': '0x0F7A',      # black pearl
    'FateThread': '0x0F8D',       # spider silk

    # Necromancy - 8 unique ItemIDs
    'GraveMoss': '0x0F7B',        # bloodmoss
    'BoneDust': '0x0F8F',         # grave dust
    'CorpseAsh': '0x0DE1',        # kindling (CHANGED from 0x0F8F)
    'SoulFragment': '0x0F0E',     # potion bottle
    'NecroticShroud': '0x1422',   # beeswax
    'LichDust': '0x0F86',         # mandrake root (CHANGED from 0x0F8F)
    'PhylacteryShard': '0x0F8A',  # pig iron
    'ReaperEssence': '0x0F7D',    # daemon blood

    # Summoning Magic - 8 unique ItemIDs
    'PlanarDust': '0x0F8F',       # grave dust
    'EtherShard': '0x0F8A',       # pig iron
    'AetherShard': '0x0F7D',      # daemon blood (CHANGED from 0x0F8A)
    'SummoningCrystal': '0x0F8E', # nox crystal
    'ChaosShard': '0x0DE1',       # kindling (CHANGED from 0x0F8A)
    'BindingRune': '0x0F86',      # mandrake root (CHANGED from 0x0F8A)
    'DimensionalKey': '0x0F7A',   # black pearl
    'SummoningSalt': '0x1422',    # beeswax (CHANGED from 0x0F8F)

    # Shamanic Magic - 8 unique ItemIDs
    'LightningRoot': '0x0F86',    # mandrake root
    'ThunderMoss': '0x0F7B',      # bloodmoss
    'StormCrystal': '0x0F8E',     # nox crystal
    'StormEssence': '0x1C18',     # oil flask
    'SpiritFeather': '0x0F78',    # batwing
    'PrimalThunder': '0x0F8A',    # pig iron
    'TotemCarving': '0x0DE1',     # kindling
    'WindEssence': '0x0F0E',      # potion bottle (CHANGED from 0x1C18)

    # Bardic Magic - 8 unique ItemIDs
    'SongPetal': '0x0F86',        # mandrake root
    'EchoDust': '0x0F8F',         # grave dust
    'VoiceCrystal': '0x0F8E',     # nox crystal
    'MuseEssence': '0x1C18',      # oil flask
    'HarmonyGem': '0x0F7A',       # black pearl
    'EternalNote': '0x0F8D',      # spider silk
    'GoldenString': '0x1422',     # beeswax (CHANGED from 0x0F8D)
    'DragonScale': '0x0F78',      # batwing

    # Enchanting Magic - 8 unique ItemIDs
    'ArcaneDust': '0x0F8F',       # grave dust
    'EssenceOfMagic': '0x1C18',   # oil flask
    'ManaCrystal': '0x0F8E',      # nox crystal
    'LeyLineEssence': '0x0F7D',   # daemon blood
    'LeyLineShard': '0x0F8A',     # pig iron
    'RuneFragment': '0x0DE1',     # kindling (CHANGED from 0x0F8A)
    'RunicPowder': '0x0F86',      # mandrake root (CHANGED from 0x0F8F)
    'TitanRune': '0x0F7A',        # black pearl (CHANGED from 0x0F8A)

    # Illusion Magic - 8 unique ItemIDs
    'MirrorDust': '0x0F8F',       # grave dust
    'PhantomSilk': '0x0F8D',      # spider silk
    'MirageEssence': '0x0F0E',    # potion bottle
    'DreamCrystal': '0x0F8E',     # nox crystal
    'RealitySplinter': '0x0F8A',  # pig iron
    'VoidMirror': '0x0F7A',       # black pearl
    'ChaosPrism': '0x1C18',       # oil flask (CHANGED from 0x0F8E)
    'PhantomPetal': '0x0F86',     # mandrake root
}

def apply_new_mapping():
    """Apply the conflict-free ItemID mapping"""

    print("="*70)
    print("  FIXING REAGENT BAG ITEMID CONFLICTS")
    print("="*70)
    print()
    print("Ensuring each bag has 8 reagents with 8 unique ItemIDs")
    print()
    print("="*70)
    print()

    total_changes = 0

    for filename in os.listdir(REAGENT_PATH):
        if filename.endswith('Reagents.cs'):
            filepath = os.path.join(REAGENT_PATH, filename)

            with open(filepath, 'r', encoding='utf-8') as f:
                content = f.read()

            original = content
            changes_in_file = 0

            # Find all reagent classes in this file
            class_pattern = r'public class (\w+) : BaseVystiaReagent'
            classes = re.findall(class_pattern, content)

            for class_name in classes:
                if class_name in NEW_MAPPING:
                    target_itemid = NEW_MAPPING[class_name]

                    # Find and replace the ItemID for this specific class
                    pattern = rf'(public class {class_name}.*?:\s*base\(amount,\s*)0x[0-9a-fA-F]+'
                    replacement = rf'\g<1>{target_itemid}'
                    new_content = re.sub(pattern, replacement, content, flags=re.DOTALL)

                    if new_content != content:
                        changes_in_file += 1
                        content = new_content

            if content != original:
                with open(filepath, 'w', encoding='utf-8') as f:
                    f.write(content)
                print(f"{filename}: {changes_in_file} reagents updated")
                total_changes += changes_in_file

    print()
    print("="*70)
    print(f"  TOTAL: {total_changes} reagents remapped")
    print("="*70)
    print()
    print("All bags now have 8 unique ItemIDs!")
    print()

if __name__ == "__main__":
    apply_new_mapping()
