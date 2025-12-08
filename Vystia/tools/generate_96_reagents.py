#!/usr/bin/env python3
"""
Generate complete Vystia reagent system - 96 reagents (8 per school × 12 schools)
Clean implementation from scratch
"""

import os
from pathlib import Path

OUTPUT_DIR = Path("C:/DevEnv/GIT/UO/ServUO/Scripts/Items/Vystia/Resources/Reagents")

# Complete reagent definitions (8 per school × 12 schools = 96 total)
REAGENTS = {
    "IceMagic": {
        "hue": "0x481",
        "location": "Frosthold tundra",
        "school": "Ice Magic",
        "reagents": [
            ("Frostbloom", 0x18E9, "Circles 1-3", "Magical frozen flower"),
            ("GlacierCrystal", 0x1F19, "Circles 2-4", "Crystal formed from glacial ice"),
            ("Winterleaf", 0x18E1, "Circles 3-5", "Leaf from an eternal winter tree"),
            ("PermafrostEssence", 0x1F1C, "Circles 4-6", "Essence of永久冻土"),
            ("ArcticPearl", 0x1F47, "Circles 5-7", "Pearl from frozen seas"),
            ("FrozenSoul", 0x1F13, "Circles 6-8", "Captured soul of ice elemental"),
            ("FrostEssence", 0x1F9D, "Circles 7-8", "Pure essence of frost"),
            ("HeartOfWinter", 0x1F19, "Circle 8", "Heart of the winter itself")
        ]
    },
    "NatureMagic": {
        "hue": "0x7D6",
        "location": "Verdantpeak forests",
        "school": "Nature Magic",
        "reagents": [
            ("WildMoss", 0x1AA2, "Circles 1-3", "Moss from ancient forests"),
            ("Moonpetal", 0x18E9, "Circles 2-4", "Flower that blooms under moonlight"),
            ("DruidBark", 0x1BD7, "Circles 3-5", "Bark from sacred druid groves"),
            ("TreantSap", 0x1F9D, "Circles 4-6", "Sap from living treants"),
            ("ElderwoodSeed", 0x1F2C, "Circles 5-7", "Seed from eldest trees"),
            ("PrimalVine", 0x1AA3, "Circles 6-8", "Vine with primal life force"),
            ("LivingBark", 0x1BD7, "Circles 7-8", "Bark that lives without tree"),
            ("AncientRoot", 0x1F1C, "Circle 8", "Root from eldest tree")
        ]
    },
    "HexMagic": {
        "hue": "0x81D",
        "location": "Shadowfen swamps",
        "school": "Hex Magic",
        "reagents": [
            ("BogMoss", 0x1AA2, "Circles 1-3", "Moss from toxic bogs"),
            ("ViperFang", 0x1F26, "Circles 2-4", "Fang of swamp viper"),
            ("Witchweed", 0x18E1, "Circles 3-5", "Weed used in witch rituals"),
            ("ToadsEye", 0x1F2F, "Circles 4-6", "Eye of giant toad"),
            ("SwampLotus", 0x18E9, "Circles 5-7", "Rare lotus from swamps"),
            ("HagsHair", 0x1AA4, "Circles 6-8", "Hair from swamp hag"),
            ("CursedPearl", 0x1F47, "Circles 7-8", "Pearl cursed by hex magic"),
            ("CursedSalt", 0x11EA, "Circle 8", "Salt used in dark curses")
        ]
    },
    "ElementalMagic": {
        "hue": "0x54E",
        "location": "Emberlands volcanoes",
        "school": "Elemental Magic",
        "reagents": [
            ("AshPetal", 0x18E9, "Circles 1-3", "Flower petal from volcanic ash"),
            ("LavaGlass", 0x1F19, "Circles 2-4", "Glass formed from lava"),
            ("Flameweed", 0x18E1, "Circles 3-5", "Weed that grows in flames"),
            ("MagmaEssence", 0x1F9D, "Circles 4-6", "Essence of pure magma"),
            ("PhoenixFeather", 0x1CFF, "Circles 5-7", "Feather from phoenix"),
            ("DragonHeart", 0x1F13, "Circles 6-8", "Heart of fire dragon"),
            ("PrimordialEmber", 0x19AC, "Circles 7-8", "Ember from first flame"),
            ("ElementalCore", 0x1F13, "Circle 8", "Core of elemental power")
        ]
    },
    "DarkMagic": {
        "hue": "0x455",
        "location": "ShadowVoid",
        "school": "Dark Magic",
        "reagents": [
            ("ShadowMoss", 0x1AA2, "Circles 1-3", "Moss from shadow realm"),
            ("VoidCrystal", 0x1F19, "Circles 2-4", "Crystal from the void"),
            ("VoidWeed", 0x18E1, "Circles 3-5", "Weed from emptiness"),
            ("ShadowPetal", 0x18E9, "Circles 4-6", "Petal from shadow flowers"),
            ("VoidDust", 0x26B8, "Circles 5-7", "Dust of the void"),
            ("VoidSilk", 0x1AA4, "Circles 6-8", "Silk woven from darkness"),
            ("DemonHeart", 0x1F13, "Circles 7-8", "Heart of shadow demon"),
            ("ShadowEssence", 0x1F9D, "Circle 8", "Pure essence of darkness")
        ]
    },
    "DivinationMagic": {
        "hue": "0x482",
        "location": "Crystal Barrens",
        "school": "Divination Magic",
        "reagents": [
            ("TimeSand", 0x11EA, "Circles 1-3", "Sand that flows through time"),
            ("TimeDust", 0x26B8, "Circles 2-4", "Dust of past and future"),
            ("DivinationDust", 0x26B8, "Circles 3-5", "Dust for seeing truth"),
            ("FateCrystal", 0x1F19, "Circles 4-6", "Crystal showing fate"),
            ("StarlightCrystal", 0x1F19, "Circles 5-7", "Crystal of starlight"),
            ("PropheticLeaf", 0x18E1, "Circles 6-8", "Leaf showing prophecies"),
            ("SeeingStone", 0x1F19, "Circles 7-8", "Stone for scrying"),
            ("FateThread", 0x1AA4, "Circle 8", "Thread of destiny")
        ]
    },
    "Necromancy": {
        "hue": "0x455",
        "location": "ShadowVoid crypts",
        "school": "Necromancy",
        "reagents": [
            ("GraveMoss", 0x1AA2, "Circles 1-3", "Moss from graves"),
            ("BoneDust", 0x26B8, "Circles 2-4", "Ground bone powder"),
            ("CorpseAsh", 0x26B8, "Circles 3-5", "Ash from cremated dead"),
            ("SoulFragment", 0x1F1C, "Circles 4-6", "Fragment of captured soul"),
            ("NecroticShroud", 0x1AA4, "Circles 5-7", "Cloth from the dead"),
            ("LichDust", 0x26B8, "Circles 6-8", "Dust from lich phylactery"),
            ("PhylacteryShard", 0x1F1C, "Circles 7-8", "Shard of phylactery"),
            ("ReaperEssence", 0x1F9D, "Circle 8", "Essence of death itself")
        ]
    },
    "SummoningMagic": {
        "hue": "0x555",
        "location": "Underwater depths",
        "school": "Summoning Magic",
        "reagents": [
            ("PlanarDust", 0x26B8, "Circles 1-3", "Dust from other planes"),
            ("EtherShard", 0x1F1C, "Circles 2-4", "Shard from ethereal realm"),
            ("AetherShard", 0x1F1C, "Circles 3-5", "Shard from aether plane"),
            ("SummoningCrystal", 0x1F19, "Circles 4-6", "Crystal for summoning"),
            ("ChaosShard", 0x1F1C, "Circles 5-7", "Shard of chaos realm"),
            ("BindingRune", 0x1F14, "Circles 6-8", "Rune for binding summoned"),
            ("DimensionalKey", 0x1F47, "Circles 7-8", "Key to other dimensions"),
            ("SummoningSalt", 0x11EA, "Circle 8", "Salt for summoning circles")
        ]
    },
    "ShamanicMagic": {
        "hue": "0x501",
        "location": "Skyreach peaks",
        "school": "Shamanic Magic",
        "reagents": [
            ("LightningRoot", 0x18E1, "Circles 1-3", "Root struck by lightning"),
            ("ThunderMoss", 0x1AA2, "Circles 2-4", "Moss from storm clouds"),
            ("StormCrystal", 0x1F19, "Circles 3-5", "Crystal from storm"),
            ("StormEssence", 0x1F9D, "Circles 4-6", "Essence of tempest"),
            ("SpiritFeather", 0x1CFF, "Circles 5-7", "Feather from spirit animal"),
            ("PrimalThunder", 0x1F13, "Circles 6-8", "Primal thunder essence"),
            ("TotemCarving", 0x1BD7, "Circles 7-8", "Sacred totem wood"),
            ("WindEssence", 0x1F9D, "Circle 8", "Essence of wind itself")
        ]
    },
    "BardicMagic": {
        "hue": "0x8A5",
        "location": "Multi-regional",
        "school": "Bardic Magic",
        "reagents": [
            ("SongPetal", 0x18E9, "Circles 1-3", "Petal that sings"),
            ("EchoDust", 0x26B8, "Circles 2-4", "Dust of echoes"),
            ("VoiceCrystal", 0x1F19, "Circles 3-5", "Crystal of voice"),
            ("MuseEssence", 0x1F9D, "Circles 4-6", "Essence of muse"),
            ("HarmonyGem", 0x1F19, "Circles 5-7", "Gem of perfect harmony"),
            ("EternalNote", 0x1AA4, "Circles 6-8", "Note that never ends"),
            ("GoldenString", 0x1AA4, "Circles 7-8", "String from golden lyre"),
            ("DragonScale", 0x1F26, "Circle 8", "Scale from ancient dragon")
        ]
    },
    "EnchantingMagic": {
        "hue": "0x8FD",
        "location": "Multi-regional",
        "school": "Enchanting Magic",
        "reagents": [
            ("ArcaneDust", 0x26B8, "Circles 1-3", "Dust of arcane power"),
            ("EssenceOfMagic", 0x1F9D, "Circles 2-4", "Pure magical essence"),
            ("ManaCrystal", 0x1F19, "Circles 3-5", "Crystal of mana"),
            ("LeyLineEssence", 0x1F9D, "Circles 4-6", "Essence from ley lines"),
            ("LeyLineShard", 0x1F1C, "Circles 5-7", "Shard from ley nexus"),
            ("RuneFragment", 0x1F14, "Circles 6-8", "Fragment of power rune"),
            ("RunicPowder", 0x26B8, "Circles 7-8", "Powder of runes"),
            ("TitanRune", 0x1F14, "Circle 8", "Rune of titan power")
        ]
    },
    "IllusionMagic": {
        "hue": "0x47E",
        "location": "Desert mirages",
        "school": "Illusion Magic",
        "reagents": [
            ("MirrorDust", 0x26B8, "Circles 1-3", "Dust from mirrors"),
            ("PhantomSilk", 0x1AA4, "Circles 2-4", "Silk from phantoms"),
            ("MirageEssence", 0x1F9D, "Circles 3-5", "Essence of mirages"),
            ("DreamCrystal", 0x1F19, "Circles 4-6", "Crystal from dreams"),
            ("RealitySplinter", 0x1F1C, "Circles 5-7", "Splinter of reality"),
            ("VoidMirror", 0x1F47, "Circles 6-8", "Mirror showing void"),
            ("ChaosPrism", 0x1F19, "Circles 7-8", "Prism of chaos"),
            ("PhantomPetal", 0x18E9, "Circle 8", "Petal from illusion realm")
        ]
    }
}

def format_class_name(name):
    """Ensure proper class name (no spaces)"""
    return name.replace(" ", "")

def format_display_name(name):
    """Convert CamelCase to display name with spaces"""
    import re
    result = re.sub(r'([a-z])([A-Z])', r'\1 \2', name)
    return result.lower()

def generate_reagent_class(class_name, item_id, hue, location, circle_usage, description):
    """Generate a single reagent class"""
    display_name = format_display_name(class_name)

    return f"""    #region {class_name} ({circle_usage})
    /// <summary>
    /// {class_name} - {description}
    /// Found in: {location}
    /// Used in: {circle_usage}
    /// </summary>
    public class {class_name} : BaseVystiaReagent
    {{
        [Constructable]
        public {class_name}() : this(1) {{ }}

        [Constructable]
        public {class_name}(int amount)
            : base(amount, {hex(item_id)}, {hue}, "{location}", "{circle_usage}")
        {{
            Weight = 0.1;
        }}

        public {class_name}(Serial serial) : base(serial) {{ }}

        public override string DefaultName {{ get {{ return "{display_name}"; }} }}

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

def generate_school_file(school_key, school_data):
    """Generate a complete reagent file for one school"""

    reagent_classes = []
    for name, item_id, circles, desc in school_data["reagents"]:
        class_name = format_class_name(name)
        reagent_classes.append(generate_reagent_class(
            class_name,
            item_id,
            school_data["hue"],
            school_data["location"],
            school_data["school"] + " (" + circles + ")",
            desc
        ))

    file_content = f"""using System;

namespace Server.Items
{{
    // ============================================
    // {school_data["school"].upper()} REAGENTS
    // ============================================
    // Used by {school_key.replace("Magic", " ")} spells
    // Total: 8 reagents
    // ============================================

{"".join(reagent_classes)}}}
"""

    return file_content

def main():
    print("="*80)
    print("GENERATING 96 VYSTIA REAGENTS (8 per school × 12 schools)")
    print("="*80)
    print()

    OUTPUT_DIR.mkdir(parents=True, exist_ok=True)

    total_reagents = 0

    for school_key, school_data in REAGENTS.items():
        filename = f"{school_key}Reagents.cs"
        filepath = OUTPUT_DIR / filename

        content = generate_school_file(school_key, school_data)

        with open(filepath, 'w', encoding='utf-8') as f:
            f.write(content)

        reagent_count = len(school_data["reagents"])
        total_reagents += reagent_count

        print(f"[OK] {filename:30} - {reagent_count} reagents")

    print()
    print("="*80)
    print(f"[SUCCESS] Generated {total_reagents} reagents across 12 schools!")
    print("="*80)
    print()
    print("[NEXT] Build ServUO to verify compilation")
    print()

if __name__ == "__main__":
    main()
