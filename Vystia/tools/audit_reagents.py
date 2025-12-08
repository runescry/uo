#!/usr/bin/env python3
"""
Wystia Reagent Audit Script
Compares documented reagents vs actual implementation
"""

import os
import re
from pathlib import Path

# Project paths
SERVUO_ROOT = Path("C:/DevEnv/GIT/UO/ServUO")
REAGENT_DIR = SERVUO_ROOT / "Scripts/Items/Vystia/Resources/Reagents"
DOCS_DIR = Path("C:/DevEnv/GIT/UO/Vystia/Magic")

# Expected reagents per school (from VYSTIA_MASTER_INVENTORY.md)
DOCUMENTED_REAGENTS = {
    "Ice": ["Frostbloom", "GlacierCrystal", "Winterleaf", "PermafrostEssence",
            "ArcticPearl", "FrozenSoul", "EternalIce", "HeartOfWinter"],
    "Nature": ["WildMoss", "Moonpetal", "DruidBark", "TreantSap",
               "ElderwoodSeed", "PrimalVine", "TreantHeart", "LivingBark"],
    "Hex": ["BogMoss", "ViperFang", "Witchweed", "ToadsEye",
            "SwampLotus", "HagsHair", "BogIronOre", "CursedPearl"],
    "Elemental": ["AshPetal", "LavaGlass", "Flameweed", "MagmaEssence",
                  "PhoenixFeather", "MoltenCore", "EverburningCoal", "DragonHeart"],
    "Dark": ["ShadowMoss", "VoidCrystal", "Darkroot", "SoulEssence",
             "ObsidianShard", "NightmareDust", "VoidHeart", "AbyssalPearl"],
    "Divination": ["SeeingStone", "OracleSand", "CrystalDust", "TimeShard",
                   "PropheticLeaf", "FateThread", "PrismaticShard", "EyeOfTruth"],
    "Necromancy": ["GraveDust", "BoneMeal", "DeathCap", "SoulStone",
                   "CorpseHair", "SpiritEssence", "PhylacteryShard", "ReapersHeart"],
    "Summoning": ["BindingRune", "SummoningSalt", "EtherWeed", "PlanarDust",
                  "VoidGem", "SpiritAnchor", "DimensionalKey", "CosmicPearl"],
    "Shamanic": ["ThunderRoot", "LightningMoss", "StormCrystal", "WindFeather",
                 "TotemWood", "SpiritDust", "ElementalCore", "SkyHeart"],
    "Bardic": ["Songflower", "EchoCrystal", "HarmonyLeaf", "MuseDust",
               "ResonanceStone", "VoiceShard", "SymphonyPearl", "LegendaryString"],
    "Enchanting": ["ArcaneDust", "EnchantCrystal", "RuneLeaf", "MagicEssence",
                   "PowerShard", "MysticThread", "EnchantmentCore", "InfinityStone"],
    "Illusion": ["MirageDust", "PhantomCrystal", "IllusionLeaf", "DreamEssence",
                 "ReflectionShard", "TricksterRoot", "MindPearl", "RealityFragment"]
}

def extract_classes_from_file(filepath):
    """Extract all class names that inherit from BaseVystiaReagent"""
    classes = []
    try:
        with open(filepath, 'r', encoding='utf-8') as f:
            content = f.read()
            # Find all class declarations
            pattern = r'public class (\w+)\s*:\s*BaseVystiaReagent'
            matches = re.findall(pattern, content)
            classes.extend(matches)
    except Exception as e:
        print(f"Error reading {filepath}: {e}")
    return classes

def scan_reagent_files():
    """Scan all reagent files and extract implemented classes"""
    implemented = {}

    school_mapping = {
        "IceMagic": "Ice",
        "NatureMagic": "Nature",
        "HexMagic": "Hex",
        "ElementalMagic": "Elemental",
        "DarkMagic": "Dark",
        "DivinationMagic": "Divination",
        "Necromancy": "Necromancy",
        "SummoningMagic": "Summoning",
        "ShamanicMagic": "Shamanic",
        "BardicMagic": "Bardic",
        "EnchantingMagic": "Enchanting",
        "IllusionMagic": "Illusion"
    }

    for filename, school in school_mapping.items():
        filepath = REAGENT_DIR / f"{filename}Reagents.cs"
        if filepath.exists():
            classes = extract_classes_from_file(filepath)
            implemented[school] = classes
        else:
            implemented[school] = []
            print(f"[WARN] File not found: {filepath}")

    return implemented

def compare_reagents():
    """Compare documented vs implemented reagents"""
    print("=" * 80)
    print("VYSTIA REAGENT AUDIT")
    print("=" * 80)
    print()

    implemented = scan_reagent_files()

    total_documented = 0
    total_implemented = 0
    total_missing = 0
    total_extra = 0

    missing_by_school = {}
    extra_by_school = {}

    for school in DOCUMENTED_REAGENTS.keys():
        doc_reagents = set(DOCUMENTED_REAGENTS[school])
        impl_reagents = set(implemented.get(school, []))

        missing = doc_reagents - impl_reagents
        extra = impl_reagents - doc_reagents

        total_documented += len(doc_reagents)
        total_implemented += len(impl_reagents)
        total_missing += len(missing)
        total_extra += len(extra)

        if missing:
            missing_by_school[school] = missing
        if extra:
            extra_by_school[school] = extra

        # Print school status
        status = "[OK]" if not missing else "[!!]"
        print(f"{status} {school:12} - Doc: {len(doc_reagents)}, Impl: {len(impl_reagents)}, Missing: {len(missing)}, Extra: {len(extra)}")

    print()
    print("=" * 80)
    print(f"SUMMARY:")
    print(f"  Total Documented: {total_documented}")
    print(f"  Total Implemented: {total_implemented}")
    print(f"  Total Missing: {total_missing}")
    print(f"  Total Extra: {total_extra}")
    print("=" * 80)
    print()

    # Detailed missing reagents
    if missing_by_school:
        print("MISSING REAGENTS (Documented but not implemented):")
        print("-" * 80)
        for school, missing in sorted(missing_by_school.items()):
            print(f"\n{school} Magic ({len(missing)} missing):")
            for reagent in sorted(missing):
                print(f"  - {reagent}")
        print()

    # Detailed extra reagents
    if extra_by_school:
        print("EXTRA REAGENTS (Implemented but not documented):")
        print("-" * 80)
        for school, extra in sorted(extra_by_school.items()):
            print(f"\n{school} Magic ({len(extra)} extra):")
            for reagent in sorted(extra):
                print(f"  - {reagent}")
        print()

    # Generate report file
    report_path = Path("C:/DevEnv/GIT/UO/Vystia") / "reagent_audit_report.txt"
    with open(report_path, 'w') as f:
        f.write("VYSTIA REAGENT AUDIT REPORT\n")
        f.write("=" * 80 + "\n\n")
        f.write(f"Total Documented: {total_documented}\n")
        f.write(f"Total Implemented: {total_implemented}\n")
        f.write(f"Total Missing: {total_missing}\n")
        f.write(f"Total Extra: {total_extra}\n\n")

        if missing_by_school:
            f.write("MISSING REAGENTS:\n")
            f.write("-" * 80 + "\n")
            for school, missing in sorted(missing_by_school.items()):
                f.write(f"\n{school} Magic:\n")
                for reagent in sorted(missing):
                    f.write(f"  - {reagent}\n")

        if extra_by_school:
            f.write("\n\nEXTRA REAGENTS:\n")
            f.write("-" * 80 + "\n")
            for school, extra in sorted(extra_by_school.items()):
                f.write(f"\n{school} Magic:\n")
                for reagent in sorted(extra):
                    f.write(f"  - {reagent}\n")

    print(f"[REPORT] Saved to: {report_path}")

    return {
        'missing': missing_by_school,
        'extra': extra_by_school,
        'total_missing': total_missing,
        'total_extra': total_extra
    }

if __name__ == "__main__":
    results = compare_reagents()

    if results['total_missing'] == 0:
        print("\n[SUCCESS] All documented reagents are implemented!")
    else:
        print(f"\n[ACTION] {results['total_missing']} reagents need to be created")

    if results['total_extra'] > 0:
        print(f"[INFO] {results['total_extra']} extra reagents exist (update docs or remove)")
