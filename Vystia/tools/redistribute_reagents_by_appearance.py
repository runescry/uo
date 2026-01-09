#!/usr/bin/env python3
"""
Redistribute all 96 Vystia reagents across 14 confirmed stackable ItemIDs
Based on reagent name/appearance/description
"""

import os
import re

REAGENT_PATH = r'D:\UO\ServUO\Scripts\Items\Vystia\Resources\Reagents'

# All 14 confirmed stackable ItemIDs
STACKABLE_ITEMIDS = {
    '0x0F7D': 'daemon blood (vial/blood)',
    '0x0F78': 'batwing (wing)',
    '0x0F8A': 'pig iron (metal/iron)',
    '0x0F8E': 'nox crystal (green crystal)',
    '0x1422': 'beeswax (wax/yellow)',
    '0x1A9C': 'flax bundle (fiber/plant bundle)',
    '0x0F8F': 'grave dust (gray dust/powder)',
    '0x0DE1': 'kindling (wood sticks)',
    '0x1C18': 'oil flask (flask/bottle)',
    '0x0F7A': 'black pearl (dark gem/pearl)',
    '0x0F7B': 'bloodmoss (red moss)',
    '0x0F8D': 'spider silk (white silk/thread)',
    '0x0F0E': 'potion bottle (empty bottle)',
    '0x0F86': 'mandrake root (root/plant)',
}

# Map reagent names to appropriate ItemIDs based on appearance/description
REAGENT_TO_ITEMID = {
    # Ice Magic (8 reagents)
    'Frostbloom': '0x0F86',        # flower/bloom -> mandrake root
    'GlacierCrystal': '0x0F8E',    # crystal -> nox crystal
    'Winterleaf': '0x1A9C',        # leaf -> flax bundle
    'PermafrostEssence': '0x0F0E', # essence -> potion bottle
    'ArcticPearl': '0x0F7A',       # pearl -> black pearl
    'FrozenSoul': '0x0F7D',        # soul/essence -> potion bottle
    'FrostEssence': '0x0F0E',      # essence -> potion bottle
    'HeartOfWinter': '0x0F7A',     # heart/gem -> black pearl

    # Nature Magic (8 reagents)
    'WildMoss': '0x0F7B',          # moss -> bloodmoss
    'Moonpetal': '0x0F86',         # petal -> mandrake root
    'DruidBark': '0x0DE1',         # bark -> kindling
    'TreantSap': '0x1C18',         # sap/liquid -> oil flask
    'ElderwoodSeed': '0x1422',     # seed -> beeswax
    'PrimalVine': '0x1A9C',        # vine -> flax bundle
    'LivingBark': '0x0DE1',        # bark -> kindling
    'AncientRoot': '0x0F86',       # root -> mandrake root

    # Hex Magic (8 reagents)
    'BogMoss': '0x0F7B',           # moss -> bloodmoss
    'ViperFang': '0x0F78',         # fang -> batwing
    'Witchweed': '0x1A9C',         # weed -> flax bundle
    'ToadsEye': '0x0F7A',          # eye/orb -> black pearl
    'SwampLotus': '0x0F86',        # lotus/flower -> mandrake root
    'HagsHair': '0x1422',          # hair/thread -> spider silk
    'CursedPearl': '0x0F7A',       # pearl -> black pearl
    'CursedSalt': '0x0F8F',        # salt -> grave dust

    # Elemental Magic (8 reagents)
    'AshPetal': '0x0F86',          # petal -> mandrake root
    'LavaGlass': '0x0F8E',         # glass/crystal -> nox crystal
    'Flameweed': '0x1A9C',         # weed -> flax bundle
    'MagmaEssence': '0x1C18',      # essence/liquid -> oil flask
    'PhoenixFeather': '0x0F78',    # feather -> batwing
    'DragonHeart': '0x0F7A',       # heart/gem -> black pearl
    'PrimordialEmber': '0x0F8A',   # ember/coal -> pig iron
    'ElementalCore': '0x0F8E',     # core/gem -> nox crystal

    # Dark Magic (8 reagents)
    'ShadowMoss': '0x0F7B',        # moss -> bloodmoss
    'VoidCrystal': '0x0F8E',       # crystal -> nox crystal
    'VoidWeed': '0x1A9C',          # weed -> flax bundle
    'ShadowPetal': '0x0F86',       # petal -> mandrake root
    'VoidDust': '0x0F8F',          # dust -> grave dust
    'VoidSilk': '0x0F8D',          # silk -> spider silk
    'DemonHeart': '0x0F7A',        # heart/gem -> black pearl
    'ShadowEssence': '0x0F7D',     # essence -> potion bottle

    # Divination Magic (8 reagents)
    'TimeSand': '0x0F8F',          # sand -> grave dust
    'TimeDust': '0x0F8F',          # dust -> grave dust
    'DivinationDust': '0x0F8F',    # dust -> grave dust
    'FateCrystal': '0x0F8E',       # crystal -> nox crystal
    'StarlightCrystal': '0x0F8E',  # crystal -> nox crystal
    'PropheticLeaf': '0x1A9C',     # leaf -> flax bundle
    'SeeingStone': '0x0F7A',       # stone/gem -> black pearl
    'FateThread': '0x0F8D',        # thread -> spider silk

    # Necromancy (8 reagents)
    'GraveMoss': '0x0F7B',         # moss -> bloodmoss
    'BoneDust': '0x0F8F',          # dust -> grave dust
    'CorpseAsh': '0x0F8F',         # ash/dust -> grave dust
    'SoulFragment': '0x0F0E',      # fragment/essence -> potion bottle
    'NecroticShroud': '0x1422',    # shroud/cloth -> spider silk
    'LichDust': '0x0F8F',          # dust -> grave dust
    'PhylacteryShard': '0x0F8A',   # shard/metal -> pig iron
    'ReaperEssence': '0x0F7D',     # essence -> potion bottle

    # Summoning Magic (8 reagents)
    'PlanarDust': '0x0F8F',        # dust -> grave dust
    'EtherShard': '0x0F8A',        # shard -> pig iron
    'AetherShard': '0x0F8A',       # shard -> pig iron
    'SummoningCrystal': '0x0F8E',  # crystal -> nox crystal
    'ChaosShard': '0x0F8A',        # shard -> pig iron
    'BindingRune': '0x0F8A',       # rune/stone -> pig iron
    'DimensionalKey': '0x0F7A',    # key/metal -> black pearl
    'SummoningSalt': '0x0F8F',     # salt -> grave dust

    # Shamanic Magic (8 reagents)
    'LightningRoot': '0x0F86',     # root -> mandrake root
    'ThunderMoss': '0x0F7B',       # moss -> bloodmoss
    'StormCrystal': '0x0F8E',      # crystal -> nox crystal
    'StormEssence': '0x1C18',      # essence -> potion bottle
    'SpiritFeather': '0x0F78',     # feather -> batwing
    'PrimalThunder': '0x0F8A',     # thunder/energy -> pig iron
    'TotemCarving': '0x0DE1',      # carving/wood -> kindling
    'WindEssence': '0x1C18',       # essence -> potion bottle

    # Bardic Magic (8 reagents)
    'SongPetal': '0x0F86',         # petal -> mandrake root
    'EchoDust': '0x0F8F',          # dust -> grave dust
    'VoiceCrystal': '0x0F8E',      # crystal -> nox crystal
    'MuseEssence': '0x1C18',       # essence -> potion bottle
    'HarmonyGem': '0x0F7A',        # gem -> black pearl
    'EternalNote': '0x0F8D',       # note/paper -> spider silk
    'GoldenString': '0x0F8D',      # string -> spider silk
    'DragonScale': '0x0F78',       # scale -> batwing

    # Enchanting Magic (8 reagents)
    'ArcaneDust': '0x0F8F',        # dust -> grave dust
    'EssenceOfMagic': '0x1C18',    # essence -> potion bottle
    'ManaCrystal': '0x0F8E',       # crystal -> nox crystal
    'LeyLineEssence': '0x0F7D',    # essence -> potion bottle
    'LeyLineShard': '0x0F8A',      # shard -> pig iron
    'RuneFragment': '0x0F8A',      # fragment -> pig iron
    'RunicPowder': '0x0F8F',       # powder -> grave dust
    'TitanRune': '0x0F8A',         # rune/stone -> pig iron

    # Illusion Magic (8 reagents)
    'MirrorDust': '0x0F8F',        # dust -> grave dust
    'PhantomSilk': '0x0F8D',       # silk -> spider silk
    'MirageEssence': '0x0F0E',     # essence -> potion bottle
    'DreamCrystal': '0x0F8E',      # crystal -> nox crystal
    'RealitySplinter': '0x0F8A',   # splinter/shard -> pig iron
    'VoidMirror': '0x0F7A',        # mirror/glass -> black pearl
    'ChaosPrism': '0x0F8E',        # prism/crystal -> nox crystal
    'PhantomPetal': '0x0F86',      # petal -> mandrake root
}

def redistribute_reagents():
    """Apply the new ItemID mapping to all reagent files"""

    # Count usage per ItemID
    itemid_counts = {itemid: 0 for itemid in STACKABLE_ITEMIDS.keys()}

    for reagent, itemid in REAGENT_TO_ITEMID.items():
        itemid_counts[itemid] += 1

    print("="*70)
    print("  REAGENT DISTRIBUTION BY ITEMID (Target: ~7 per ItemID)")
    print("="*70)
    for itemid in sorted(STACKABLE_ITEMIDS.keys()):
        count = itemid_counts[itemid]
        desc = STACKABLE_ITEMIDS[itemid]
        print(f"{itemid} ({count:2d} reagents) - {desc}")
    print()

    # Apply changes to files
    total_replacements = 0

    for filename in os.listdir(REAGENT_PATH):
        if filename.endswith('Reagents.cs'):
            filepath = os.path.join(REAGENT_PATH, filename)

            with open(filepath, 'r', encoding='utf-8') as f:
                content = f.read()

            original = content
            replacements_in_file = 0

            # Find all reagent classes in this file
            class_pattern = r'public class (\w+) : BaseVystiaReagent'
            classes = re.findall(class_pattern, content)

            for class_name in classes:
                if class_name in REAGENT_TO_ITEMID:
                    target_itemid = REAGENT_TO_ITEMID[class_name]

                    # Find and replace the ItemID for this specific class
                    pattern = rf'(public class {class_name}.*?:\s*base\(amount,\s*)0x[0-9a-fA-F]+'
                    replacement = rf'\g<1>{target_itemid}'
                    new_content = re.sub(pattern, replacement, content, flags=re.DOTALL)

                    if new_content != content:
                        replacements_in_file += 1
                        content = new_content

            if content != original:
                with open(filepath, 'w', encoding='utf-8') as f:
                    f.write(content)
                print(f"{filename}: {replacements_in_file} reagents updated")
                total_replacements += replacements_in_file

    print()
    print("="*70)
    print(f"  TOTAL: {total_replacements} reagents redistributed")
    print("="*70)

if __name__ == "__main__":
    redistribute_reagents()
