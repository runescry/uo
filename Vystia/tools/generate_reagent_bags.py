#!/usr/bin/env python3
"""
Generate ReagentBag classes for all 12 magic schools
Each bag contains all 8 reagents for that school
"""

from pathlib import Path

OUTPUT_FILE = Path("C:/DevEnv/GIT/UO/ServUO/Scripts/Items/Vystia/Resources/Reagents/VystiaReagentBags.cs")

# School data with reagent class names
SCHOOLS = {
    "IceMagic": {
        "name": "Ice Magic",
        "hue": "0x481",
        "reagents": ["Frostbloom", "GlacierCrystal", "Winterleaf", "PermafrostEssence",
                     "ArcticPearl", "FrozenSoul", "FrostEssence", "HeartOfWinter"]
    },
    "NatureMagic": {
        "name": "Nature Magic",
        "hue": "0x7D6",
        "reagents": ["WildMoss", "Moonpetal", "DruidBark", "TreantSap",
                     "ElderwoodSeed", "PrimalVine", "LivingBark", "AncientRoot"]
    },
    "HexMagic": {
        "name": "Hex Magic",
        "hue": "0x81D",
        "reagents": ["BogMoss", "ViperFang", "Witchweed", "ToadsEye",
                     "SwampLotus", "HagsHair", "CursedPearl", "CursedSalt"]
    },
    "ElementalMagic": {
        "name": "Elemental Magic",
        "hue": "0x54E",
        "reagents": ["AshPetal", "LavaGlass", "Flameweed", "MagmaEssence",
                     "PhoenixFeather", "DragonHeart", "PrimordialEmber", "ElementalCore"]
    },
    "DarkMagic": {
        "name": "Dark Magic",
        "hue": "0x455",
        "reagents": ["ShadowMoss", "VoidCrystal", "VoidWeed", "ShadowPetal",
                     "VoidDust", "VoidSilk", "DemonHeart", "ShadowEssence"]
    },
    "DivinationMagic": {
        "name": "Divination Magic",
        "hue": "0x482",
        "reagents": ["TimeSand", "TimeDust", "DivinationDust", "FateCrystal",
                     "StarlightCrystal", "PropheticLeaf", "SeeingStone", "FateThread"]
    },
    "Necromancy": {
        "name": "Necromancy",
        "hue": "0x455",
        "reagents": ["GraveMoss", "BoneDust", "CorpseAsh", "SoulFragment",
                     "NecroticShroud", "LichDust", "PhylacteryShard", "ReaperEssence"]
    },
    "SummoningMagic": {
        "name": "Summoning Magic",
        "hue": "0x555",
        "reagents": ["PlanarDust", "EtherShard", "AetherShard", "SummoningCrystal",
                     "ChaosShard", "BindingRune", "DimensionalKey", "SummoningSalt"]
    },
    "ShamanicMagic": {
        "name": "Shamanic Magic",
        "hue": "0x501",
        "reagents": ["LightningRoot", "ThunderMoss", "StormCrystal", "StormEssence",
                     "SpiritFeather", "PrimalThunder", "TotemCarving", "WindEssence"]
    },
    "BardicMagic": {
        "name": "Bardic Magic",
        "hue": "0x8A5",
        "reagents": ["SongPetal", "EchoDust", "VoiceCrystal", "MuseEssence",
                     "HarmonyGem", "EternalNote", "GoldenString", "DragonScale"]
    },
    "EnchantingMagic": {
        "name": "Enchanting Magic",
        "hue": "0x8FD",
        "reagents": ["ArcaneDust", "EssenceOfMagic", "ManaCrystal", "LeyLineEssence",
                     "LeyLineShard", "RuneFragment", "RunicPowder", "TitanRune"]
    },
    "IllusionMagic": {
        "name": "Illusion Magic",
        "hue": "0x47E",
        "reagents": ["MirrorDust", "PhantomSilk", "MirageEssence", "DreamCrystal",
                     "RealitySplinter", "VoidMirror", "ChaosPrism", "PhantomPetal"]
    }
}

def generate_bag_class(school_key, school_data):
    """Generate a ReagentBag class for one school"""

    # Build PackItem calls for all 8 reagents
    pack_items = []
    for reagent in school_data["reagents"]:
        pack_items.append(f"            DropItem(new {reagent}(10));")

    pack_items_str = "\n".join(pack_items)

    return f"""    #region {school_key}ReagentBag
    /// <summary>
    /// {school_data['name']} Reagent Bag - Contains all 8 reagents for {school_data['name']}
    /// </summary>
    public class {school_key}ReagentBag : Bag
    {{
        [Constructable]
        public {school_key}ReagentBag() : this(1)
        {{
        }}

        [Constructable]
        public {school_key}ReagentBag(int amount)
        {{
            Name = "{school_data['name']} Reagent Bag";
            Hue = {school_data['hue']};

{pack_items_str}
        }}

        public {school_key}ReagentBag(Serial serial) : base(serial)
        {{
        }}

        public override void Serialize(GenericWriter writer)
        {{
            base.Serialize(writer);
            writer.Write((int)0); // version
        }}

        public override void Deserialize(GenericReader reader)
        {{
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }}
    }}
    #endregion
"""

def main():
    print("="*80)
    print("GENERATING 12 REAGENT BAG CLASSES")
    print("="*80)
    print()

    bag_classes = []
    for school_key, school_data in SCHOOLS.items():
        bag_classes.append(generate_bag_class(school_key, school_data))

    file_content = f"""using System;
using Server;

namespace Server.Items
{{
    // ============================================
    // VYSTIA MAGIC REAGENT BAGS
    // ============================================
    // One bag per school containing all 8 reagents
    // Total: 12 bags (one per magic school)
    // ============================================

{"".join(bag_classes)}}}
"""

    OUTPUT_FILE.parent.mkdir(parents=True, exist_ok=True)

    with open(OUTPUT_FILE, 'w', encoding='utf-8') as f:
        f.write(file_content)

    print(f"[OK] Generated VystiaReagentBags.cs")
    print(f"     Location: {OUTPUT_FILE}")
    print()
    print("="*80)
    print("[SUCCESS] All 12 reagent bag classes created!")
    print("="*80)
    print()
    print("[NEXT] Build ServUO to verify compilation")
    print()

if __name__ == "__main__":
    main()
