#!/usr/bin/env python3
"""
Regenerate MagicSchoolVendors.cs with correct 96-reagent system
Uses same reagent data as generate_96_reagents.py
"""

from pathlib import Path

OUTPUT_FILE = Path("C:/DevEnv/GIT/UO/ServUO/Scripts/Mobiles/Vystia/Vendors/MagicSchoolVendors.cs")

# Complete reagent definitions (8 per school × 12 schools = 96 total)
# Matches generate_96_reagents.py exactly
REAGENTS = {
    "IceMagic": {
        "hue": "0x481",
        "display": "Ice Magic",
        "vendor_title": "the ice mage vendor",
        "reagents": [
            ("Frostbloom", "Frostbloom", 0x18E9),
            ("GlacierCrystal", "Glacier Crystal", 0x1F19),
            ("Winterleaf", "Winterleaf", 0x18E1),
            ("PermafrostEssence", "Permafrost Essence", 0x1F1C),
            ("ArcticPearl", "Arctic Pearl", 0x1F47),
            ("FrozenSoul", "Frozen Soul", 0x1F13),
            ("FrostEssence", "Frost Essence", 0x1F9D),
            ("HeartOfWinter", "Heart Of Winter", 0x1F19)
        ]
    },
    "NatureMagic": {
        "hue": "0x7D6",
        "display": "Nature Magic",
        "vendor_title": "the druid vendor",
        "reagents": [
            ("WildMoss", "Wild Moss", 0x1AA2),
            ("Moonpetal", "Moonpetal", 0x18E9),
            ("DruidBark", "Druid Bark", 0x1BD7),
            ("TreantSap", "Treant Sap", 0x1F9D),
            ("ElderwoodSeed", "Elderwood Seed", 0x1F2C),
            ("PrimalVine", "Primal Vine", 0x1AA3),
            ("LivingBark", "Living Bark", 0x1BD7),
            ("AncientRoot", "Ancient Root", 0x1F1C)
        ]
    },
    "HexMagic": {
        "hue": "0x81D",
        "display": "Hex Magic",
        "vendor_title": "the witch vendor",
        "reagents": [
            ("BogMoss", "Bog Moss", 0x1AA2),
            ("ViperFang", "Viper Fang", 0x1F26),
            ("Witchweed", "Witchweed", 0x18E1),
            ("ToadsEye", "Toads Eye", 0x1F2F),
            ("SwampLotus", "Swamp Lotus", 0x18E9),
            ("HagsHair", "Hags Hair", 0x1AA4),
            ("CursedPearl", "Cursed Pearl", 0x1F47),
            ("CursedSalt", "Cursed Salt", 0x11EA)
        ]
    },
    "ElementalMagic": {
        "hue": "0x54E",
        "display": "Elemental Magic",
        "vendor_title": "the sorcerer vendor",
        "reagents": [
            ("AshPetal", "Ash Petal", 0x18E9),
            ("LavaGlass", "Lava Glass", 0x1F19),
            ("Flameweed", "Flameweed", 0x18E1),
            ("MagmaEssence", "Magma Essence", 0x1F9D),
            ("PhoenixFeather", "Phoenix Feather", 0x1CFF),
            ("DragonHeart", "Dragon Heart", 0x1F13),
            ("PrimordialEmber", "Primordial Ember", 0x19AC),
            ("ElementalCore", "Elemental Core", 0x1F13)
        ]
    },
    "DarkMagic": {
        "hue": "0x455",
        "display": "Dark Magic",
        "vendor_title": "the warlock vendor",
        "reagents": [
            ("ShadowMoss", "Shadow Moss", 0x1AA2),
            ("VoidCrystal", "Void Crystal", 0x1F19),
            ("VoidWeed", "Void Weed", 0x18E1),
            ("ShadowPetal", "Shadow Petal", 0x18E9),
            ("VoidDust", "Void Dust", 0x26B8),
            ("VoidSilk", "Void Silk", 0x1AA4),
            ("DemonHeart", "Demon Heart", 0x1F13),
            ("ShadowEssence", "Shadow Essence", 0x1F9D)
        ]
    },
    "DivinationMagic": {
        "hue": "0x482",
        "display": "Divination Magic",
        "vendor_title": "the oracle vendor",
        "reagents": [
            ("TimeSand", "Time Sand", 0x11EA),
            ("TimeDust", "Time Dust", 0x26B8),
            ("DivinationDust", "Divination Dust", 0x26B8),
            ("FateCrystal", "Fate Crystal", 0x1F19),
            ("StarlightCrystal", "Starlight Crystal", 0x1F19),
            ("PropheticLeaf", "Prophetic Leaf", 0x18E1),
            ("SeeingStone", "Seeing Stone", 0x1F19),
            ("FateThread", "Fate Thread", 0x1AA4)
        ]
    },
    "Necromancy": {
        "hue": "0x455",
        "display": "Necromancy",
        "vendor_title": "the necromancer vendor",
        "reagents": [
            ("GraveMoss", "Grave Moss", 0x1AA2),
            ("BoneDust", "Bone Dust", 0x26B8),
            ("CorpseAsh", "Corpse Ash", 0x26B8),
            ("SoulFragment", "Soul Fragment", 0x1F1C),
            ("NecroticShroud", "Necrotic Shroud", 0x1AA4),
            ("LichDust", "Lich Dust", 0x26B8),
            ("PhylacteryShard", "Phylactery Shard", 0x1F1C),
            ("ReaperEssence", "Reaper Essence", 0x1F9D)
        ]
    },
    "SummoningMagic": {
        "hue": "0x555",
        "display": "Summoning Magic",
        "vendor_title": "the summoner vendor",
        "reagents": [
            ("PlanarDust", "Planar Dust", 0x26B8),
            ("EtherShard", "Ether Shard", 0x1F1C),
            ("AetherShard", "Aether Shard", 0x1F1C),
            ("SummoningCrystal", "Summoning Crystal", 0x1F19),
            ("ChaosShard", "Chaos Shard", 0x1F1C),
            ("BindingRune", "Binding Rune", 0x1F14),
            ("DimensionalKey", "Dimensional Key", 0x1F47),
            ("SummoningSalt", "Summoning Salt", 0x11EA)
        ]
    },
    "ShamanicMagic": {
        "hue": "0x501",
        "display": "Shamanic Magic",
        "vendor_title": "the shaman vendor",
        "reagents": [
            ("LightningRoot", "Lightning Root", 0x18E1),
            ("ThunderMoss", "Thunder Moss", 0x1AA2),
            ("StormCrystal", "Storm Crystal", 0x1F19),
            ("StormEssence", "Storm Essence", 0x1F9D),
            ("SpiritFeather", "Spirit Feather", 0x1CFF),
            ("PrimalThunder", "Primal Thunder", 0x1F13),
            ("TotemCarving", "Totem Carving", 0x1BD7),
            ("WindEssence", "Wind Essence", 0x1F9D)
        ]
    },
    "BardicMagic": {
        "hue": "0x8A5",
        "display": "Bardic Magic",
        "vendor_title": "the bard vendor",
        "reagents": [
            ("SongPetal", "Song Petal", 0x18E9),
            ("EchoDust", "Echo Dust", 0x26B8),
            ("VoiceCrystal", "Voice Crystal", 0x1F19),
            ("MuseEssence", "Muse Essence", 0x1F9D),
            ("HarmonyGem", "Harmony Gem", 0x1F19),
            ("EternalNote", "Eternal Note", 0x1AA4),
            ("GoldenString", "Golden String", 0x1AA4),
            ("DragonScale", "Dragon Scale", 0x1F26)
        ]
    },
    "EnchantingMagic": {
        "hue": "0x8FD",
        "display": "Enchanting Magic",
        "vendor_title": "the enchanter vendor",
        "reagents": [
            ("ArcaneDust", "Arcane Dust", 0x26B8),
            ("EssenceOfMagic", "Essence Of Magic", 0x1F9D),
            ("ManaCrystal", "Mana Crystal", 0x1F19),
            ("LeyLineEssence", "Ley Line Essence", 0x1F9D),
            ("LeyLineShard", "Ley Line Shard", 0x1F1C),
            ("RuneFragment", "Rune Fragment", 0x1F14),
            ("RunicPowder", "Runic Powder", 0x26B8),
            ("TitanRune", "Titan Rune", 0x1F14)
        ]
    },
    "IllusionMagic": {
        "hue": "0x47E",
        "display": "Illusion Magic",
        "vendor_title": "the illusionist vendor",
        "reagents": [
            ("MirrorDust", "Mirror Dust", 0x26B8),
            ("PhantomSilk", "Phantom Silk", 0x1AA4),
            ("MirageEssence", "Mirage Essence", 0x1F9D),
            ("DreamCrystal", "Dream Crystal", 0x1F19),
            ("RealitySplinter", "Reality Splinter", 0x1F1C),
            ("VoidMirror", "Void Mirror", 0x1F47),
            ("ChaosPrism", "Chaos Prism", 0x1F19),
            ("PhantomPetal", "Phantom Petal", 0x18E9)
        ]
    }
}

# School to spellbook mapping
SPELLBOOKS = {
    "IceMagic": "IceMageSpellbook",
    "NatureMagic": "DruidSpellbook",
    "HexMagic": "WitchSpellbook",
    "ElementalMagic": "SorcererSpellbook",
    "DarkMagic": "WarlockSpellbook",
    "DivinationMagic": "OracleSpellbook",
    "Necromancy": "VystiaNecromancerSpellbook",
    "SummoningMagic": "SummonerSpellbook",
    "ShamanicMagic": "ShamanSpellbook",
    "BardicMagic": "BardSpellbook",
    "EnchantingMagic": "EnchanterSpellbook",
    "IllusionMagic": "IllusionistSpellbook"
}

# School to vendor class name mapping
VENDOR_CLASSES = {
    "IceMagic": "IceMageVendor",
    "NatureMagic": "DruidVendor",
    "HexMagic": "WitchVendor",
    "ElementalMagic": "SorcererVendor",
    "DarkMagic": "WarlockVendor",
    "DivinationMagic": "OracleVendor",
    "Necromancy": "NecromancerVendor",
    "SummoningMagic": "SummonerVendor",
    "ShamanicMagic": "ShamanVendor",
    "BardicMagic": "BardVendor",
    "EnchantingMagic": "EnchanterVendor",
    "IllusionMagic": "IllusionistVendor"
}

# School to SB class name mapping
SB_CLASSES = {
    "IceMagic": "SBIceMage",
    "NatureMagic": "SBDruid",
    "HexMagic": "SBWitch",
    "ElementalMagic": "SBSorcerer",
    "DarkMagic": "SBWarlock",
    "DivinationMagic": "SBOracle",
    "Necromancy": "SBVystiaNecromancer",  # Renamed to avoid conflict with ServUO SBNecromancer
    "SummoningMagic": "SBSummoner",
    "ShamanicMagic": "SBShaman",
    "BardicMagic": "SBVystiaBard",  # Renamed to avoid conflict with ServUO SBBard
    "EnchantingMagic": "SBEnchanter",
    "IllusionMagic": "SBIllusionist"
}

def generate_vendor(school_key, school_data):
    """Generate one magic school vendor"""

    spellbook_class = SPELLBOOKS[school_key]
    vendor_class = VENDOR_CLASSES[school_key]
    sb_class = SB_CLASSES[school_key]

    # Reagent buy info
    reagent_lines = []
    for class_name, display_name, item_id in school_data["reagents"]:
        reagent_lines.append(
            f'            m_BuyInfo.Add(new GenericBuyInfo("{display_name}", typeof({class_name}), 5, 999, {hex(item_id)}, {school_data["hue"]}));'
        )

    # Spell scroll buy info (32 spells)
    # Generate scroll class names based on vendor class
    scroll_prefix = vendor_class.replace("Vendor", "")  # IceMageVendor -> IceMage
    scroll_lines = []
    for circle in range(1, 9):
        for spell in range(1, 5):
            scroll_class = f"{scroll_prefix}Spell{circle}_{spell}Scroll"
            price = 10 + (circle - 1) * 5
            scroll_lines.append(
                f'            m_BuyInfo.Add(new GenericBuyInfo("{school_data["display"]} Spell{circle}_{spell} Scroll", typeof({scroll_class}), {price}, 999, 0x1F2D, {school_data["hue"]}));'
            )

    return f'''    /// <summary>
    /// {school_data["display"]} Vendor - Sells reagents, scrolls, and spellbooks
    /// </summary>
    public class {vendor_class} : BaseVendor
    {{
        private List<SBInfo> m_SBInfos = new List<SBInfo>();
        protected override List<SBInfo> SBInfos {{ get {{ return m_SBInfos; }} }}

        [Constructable]
        public {vendor_class}() : base("{school_data["vendor_title"]}")
        {{
            Hue = {school_data["hue"]};
            SetSkill(SkillName.Magery, 100.0);
            SetSkill(SkillName.EvalInt, 100.0);
            SetSkill(SkillName.Inscribe, 100.0);
        }}

        public override void InitSBInfo()
        {{
            m_SBInfos.Add(new {sb_class}());
        }}

        public {vendor_class}(Serial serial) : base(serial) {{ }}

        public override void Serialize(GenericWriter writer)
        {{
            base.Serialize(writer);
            writer.Write((int)0);
        }}

        public override void Deserialize(GenericReader reader)
        {{
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }}
    }}

    public class {sb_class} : SBInfo
    {{
        private List<GenericBuyInfo> m_BuyInfo = new List<GenericBuyInfo>();
        private IShopSellInfo m_SellInfo = new InternalSellInfo();

        public {sb_class}()
        {{
            // Spellbook (empty)
            m_BuyInfo.Add(new GenericBuyInfo("{school_data["display"]} Spellbook", typeof({spellbook_class}), 150, 20, 0x1F2D, {school_data["hue"]}));

            // Reagents (8 total)
{chr(10).join(reagent_lines)}

            // Spell Scrolls (32 total)
{chr(10).join(scroll_lines)}
        }}

        public override IShopSellInfo SellInfo {{ get {{ return m_SellInfo; }} }}
        public override List<GenericBuyInfo> BuyInfo {{ get {{ return m_BuyInfo; }} }}

        public class InternalSellInfo : GenericSellInfo
        {{
            public InternalSellInfo()
            {{
            }}
        }}
    }}

'''

def main():
    print("="*80)
    print("REGENERATING MAGIC SCHOOL VENDORS WITH 96-REAGENT SYSTEM")
    print("="*80)
    print()

    vendors = []
    for school_key, school_data in REAGENTS.items():
        vendors.append(generate_vendor(school_key, school_data))
        print(f"[OK] {school_key} vendor generated (8 reagents)")

    file_content = f"""using System;
using System.Collections.Generic;
using Server.Items;
using Server.Items.VystiaScrolls;

namespace Server.Mobiles
{{
    /// <summary>
    /// Magic School Vendors - One vendor per school
    /// Each sells: reagents (8), scrolls (32), and empty spellbook for their school
    /// Auto-generated with 96-reagent system - 12 vendors total
    /// </summary>

{"".join(vendors)}}}
"""

    with open(OUTPUT_FILE, 'w', encoding='utf-8') as f:
        f.write(file_content)

    print()
    print("="*80)
    print(f"[SUCCESS] Regenerated MagicSchoolVendors.cs with 96 reagents!")
    print(f"           Location: {OUTPUT_FILE}")
    print("="*80)
    print()
    print("[NEXT] Build ServUO to verify compilation")
    print()

if __name__ == "__main__":
    main()
