#!/usr/bin/env python3
"""
Add reagent requirements to all Vystia spell files
Each spell will check for and consume appropriate reagents based on circle
"""

from pathlib import Path
import re

SPELLS_DIR = Path("C:/DevEnv/GIT/UO/ServUO/Scripts/Custom/VystiaClasses/Spells")

# Reagent mapping: school -> list of 8 reagents (ordered cheap to expensive)
REAGENTS = {
    "Bardic": ["SongPetal", "EchoDust", "VoiceCrystal", "MuseEssence", "HarmonyGem", "EternalNote", "GoldenString", "DragonScale"],
    "Dark": ["ShadowMoss", "VoidCrystal", "VoidWeed", "ShadowPetal", "VoidDust", "VoidSilk", "DemonHeart", "ShadowEssence"],
    "Divination": ["TimeSand", "TimeDust", "DivinationDust", "FateCrystal", "StarlightCrystal", "PropheticLeaf", "SeeingStone", "FateThread"],
    "Elemental": ["AshPetal", "LavaGlass", "Flameweed", "MagmaEssence", "PhoenixFeather", "DragonHeart", "PrimordialEmber", "ElementalCore"],
    "Enchanting": ["ArcaneDust", "EssenceOfMagic", "ManaCrystal", "LeyLineEssence", "LeyLineShard", "RuneFragment", "RunicPowder", "TitanRune"],
    "Hex": ["BogMoss", "ViperFang", "Witchweed", "ToadsEye", "SwampLotus", "HagsHair", "CursedPearl", "CursedSalt"],
    "IceMage": ["Frostbloom", "GlacierCrystal", "Winterleaf", "PermafrostEssence", "ArcticPearl", "FrozenSoul", "FrostEssence", "HeartOfWinter"],
    "Illusion": ["MirrorDust", "PhantomSilk", "MirageEssence", "DreamCrystal", "RealitySplinter", "VoidMirror", "ChaosPrism", "PhantomPetal"],
    "Nature": ["WildMoss", "Moonpetal", "DruidBark", "TreantSap", "ElderwoodSeed", "PrimalVine", "LivingBark", "AncientRoot"],
    "Necromancy": ["GraveMoss", "BoneDust", "CorpseAsh", "SoulFragment", "NecroticShroud", "LichDust", "PhylacteryShard", "ReaperEssence"],
    "Shamanic": ["LightningRoot", "ThunderMoss", "StormCrystal", "StormEssence", "SpiritFeather", "PrimalThunder", "TotemCarving", "WindEssence"],
    "Summoning": ["PlanarDust", "EtherShard", "AetherShard", "SummoningCrystal", "ChaosShard", "BindingRune", "DimensionalKey", "SummoningSalt"],
}

# Map circle to reagent indices (which reagents to use)
# Lower circles use cheaper reagents, higher circles use more/rarer reagents
CIRCLE_REAGENTS = {
    "First": [0, 1],        # 2 cheapest reagents
    "Second": [1, 2],       # Mid-low reagents
    "Third": [2, 3],        # Mid reagents
    "Fourth": [3, 4],       # Mid-high reagents
    "Fifth": [4, 5],        # High reagents
    "Sixth": [5, 6],        # Higher reagents
    "Seventh": [6, 7],      # Rare reagents
    "Eighth": [6, 7, 7],    # Most rare reagents (3x)
}

def get_school_from_namespace(namespace):
    """Extract school name from namespace"""
    match = re.search(r'VystiaSpells\.(\w+)', namespace)
    if match:
        school = match.group(1)
        # Handle special cases
        if school == "Nature":
            return "Nature"
        return school
    return None

def get_circle_from_property(content):
    """Extract spell circle from content"""
    match = re.search(r'public override SpellCircle Circle.*?SpellCircle\.(\w+)', content, re.DOTALL)
    if match:
        return match.group(1)
    return None

def generate_reagent_check_code(school, circle):
    """Generate reagent checking code for a spell"""
    if school not in REAGENTS:
        return "// REAGENT CHECK NOT IMPLEMENTED - UNKNOWN SCHOOL\n"

    if circle not in CIRCLE_REAGENTS:
        return "// REAGENT CHECK NOT IMPLEMENTED - UNKNOWN CIRCLE\n"

    school_reagents = REAGENTS[school]
    reagent_indices = CIRCLE_REAGENTS[circle]

    # Build check code
    checks = []
    consumes = []

    for idx in reagent_indices:
        reagent = school_reagents[idx]
        checks.append(f"!Caster.Backpack.ConsumeTotal(typeof({reagent}), 1)")
        consumes.append(f"{reagent} (1)")

    check_condition = " || ".join(checks)
    consume_list = ", ".join(consumes)

    return f"""            // Vystia Reagent Check
            if ({check_condition})
            {{
                Caster.SendMessage("You do not have enough reagents to cast this spell.");
                Caster.SendMessage("Required: {consume_list}");
                return false;
            }}

"""

def process_spell_file(filepath):
    """Add reagent checking to a spell file"""
    with open(filepath, 'r', encoding='utf-8') as f:
        content = f.read()

    # Check if already has reagent check
    if "Vystia Reagent Check" in content:
        return False  # Already processed

    # Extract school and circle
    school = get_school_from_namespace(content)
    circle = get_circle_from_property(content)

    if not school or not circle:
        return False  # Can't determine school/circle

    # Find the TODO comment and replace it
    todo_pattern = r'(\s+)// TODO: Check for Vystia reagents\s*\n\s*// Example: if \(!HasReagent\(typeof\(ReagentName\), 1\)\)\s*\n\s*//\s+return false;'

    reagent_code = generate_reagent_check_code(school, circle)

    # Replace TODO with actual code
    new_content = re.sub(
        todo_pattern,
        lambda m: reagent_code,
        content
    )

    if new_content != content:
        with open(filepath, 'w', encoding='utf-8') as f:
            f.write(new_content)
        return True

    return False

def main():
    print("="*80)
    print("ADDING REAGENT REQUIREMENTS TO VYSTIA SPELLS")
    print("="*80)
    print()

    files_processed = 0
    files_skipped = 0

    for spell_file in SPELLS_DIR.rglob("*Spell.cs"):
        if process_spell_file(spell_file):
            files_processed += 1
            print(f"[UPDATED] {spell_file.relative_to(SPELLS_DIR)}")
        else:
            files_skipped += 1

    print()
    print("="*80)
    print(f"[SUCCESS] Processed {files_processed} spell files")
    print(f"[SKIPPED] {files_skipped} files (already processed or can't determine school/circle)")
    print("="*80)
    print()
    print("[NEXT] Build ServUO to verify compilation")
    print()

if __name__ == "__main__":
    main()
