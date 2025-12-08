#!/usr/bin/env python3
"""
Vystia Class Generator
Generates complete class implementations for all 15 pending Vystia classes.

This script:
1. Reads class data from documentation
2. Generates C# class files using existing class templates
3. Creates proper equipment, stats, skills setup
4. Handles regional hues and starting items

Usage:
    python generate_all_classes.py [options]

Options:
    --class <name>     Generate only specified class (e.g., "Beastmaster")
    --dry-run          Show what would be generated without creating files
    --output <path>    Output directory (default: ServUO/Scripts/Custom/VystiaClasses/Classes/)
"""

import os
import re
import sys
from pathlib import Path
from dataclasses import dataclass
from typing import List, Dict, Tuple

@dataclass
class ClassData:
    """Represents a Vystia character class"""
    name: str
    class_type: str  # PascalCase enum name
    region: str
    role: str
    description: str

    # Stats
    start_str: int
    start_dex: int
    start_int: int
    str_cap: int
    dex_cap: int
    int_cap: int

    # Skills (6 primary skills)
    skills: List[Tuple[str, float]]  # (SkillName, starting value)

    # Equipment
    hue: int
    armor_type: str  # "Plate", "Chain", "Leather", "Cloth"
    weapon: str

    # Special items/abilities
    special_item: str  # Class-specific item name (if any)
    spellbook: str  # Spellbook name (if caster)

    # Resources
    starting_resources: List[Tuple[str, int]]  # (Resource name, quantity)

# Regional hue mapping
REGIONAL_HUES = {
    "Frosthold": 1150,
    "Emberlands": 1358,
    "Desert": 1719,
    "Shadowfen": 2073,
    "Verdantpeak": 2010,
    "Crystal Barrens": 1154,
    "Ironclad": 2305,
    "Skyreach": 1281,
    "ShadowVoid": 1109,
    "Underwater": 1365,
    "Multi-Regional": 1153
}

# Class definitions based on VYSTIA_MASTER_INVENTORY.md
CLASS_DEFINITIONS = {
    "Beastmaster": {
        "class_type": "Beastmaster",
        "region": "Frosthold",
        "role": "Pet/Ranged",
        "description": "Master of beasts who commands animals and excels at archery",
        "start_str": 25, "start_dex": 35, "start_int": 20,
        "str_cap": 110, "dex_cap": 130, "int_cap": 100,
        "skills": [
            ("AnimalTaming", 50.0),
            ("AnimalLore", 50.0),
            ("Veterinary", 40.0),
            ("Archery", 35.0),
            ("Tactics", 35.0),
            ("Tracking", 30.0)
        ],
        "armor_type": "Leather",
        "weapon": "Bow",
        "special_item": "BeastWhistle",
        "spellbook": "",
        "resources": [("FrozenOre", 20), ("Arrow", 100)]
    },

    "Sorcerer": {
        "class_type": "Sorcerer",
        "region": "Emberlands",
        "role": "Caster DPS",
        "description": "Master of elemental fire magic with devastating AoE damage",
        "start_str": 15, "start_dex": 20, "start_int": 45,
        "str_cap": 100, "dex_cap": 110, "int_cap": 140,
        "skills": [
            ("Magery", 50.0),
            ("EvalInt", 50.0),
            ("Meditation", 40.0),
            ("Wrestling", 30.0),
            ("MagicResist", 40.0),
            ("Tactics", 30.0)
        ],
        "armor_type": "Cloth",
        "weapon": "QuarterStaff",
        "special_item": "",
        "spellbook": "SorcererSpellbook",
        "resources": [("MoltenOre", 20)]
    },

    "Illusionist": {
        "class_type": "Illusionist",
        "region": "Desert",
        "role": "Caster CC",
        "description": "Master of illusions and mind control magic",
        "start_str": 10, "start_dex": 25, "start_int": 45,
        "str_cap": 95, "dex_cap": 115, "int_cap": 140,
        "skills": [
            ("Magery", 50.0),
            ("EvalInt", 50.0),
            ("Meditation", 40.0),
            ("Wrestling", 30.0),
            ("MagicResist", 40.0),
            ("Stealth", 30.0)
        ],
        "armor_type": "Cloth",
        "weapon": "QuarterStaff",
        "special_item": "",
        "spellbook": "IllusionistSpellbook",
        "resources": [("SunstoneOre", 20)]
    },

    "Warlock": {
        "class_type": "Warlock",
        "region": "ShadowVoid",
        "role": "Caster DPS",
        "description": "Dark magic wielder who channels demonic power",
        "start_str": 15, "start_dex": 20, "start_int": 45,
        "str_cap": 100, "dex_cap": 110, "int_cap": 140,
        "skills": [
            ("Magery", 50.0),
            ("EvalInt", 50.0),
            ("Meditation", 40.0),
            ("SpiritSpeak", 35.0),
            ("MagicResist", 40.0),
            ("Necromancy", 35.0)
        ],
        "armor_type": "Cloth",
        "weapon": "GnarledStaff",
        "special_item": "",
        "spellbook": "WarlockSpellbook",
        "resources": [("VoidstoneOre", 20)]
    },

    "Alchemist": {
        "class_type": "Alchemist",
        "region": "Verdantpeak",
        "role": "Support",
        "description": "Potion master and chemical warfare specialist",
        "start_str": 20, "start_dex": 30, "start_int": 30,
        "str_cap": 105, "dex_cap": 120, "int_cap": 115,
        "skills": [
            ("Alchemy", 60.0),
            ("TasteID", 50.0),
            ("Magery", 35.0),
            ("Poisoning", 40.0),
            ("Cooking", 35.0),
            ("Meditation", 20.0)
        ],
        "armor_type": "Leather",
        "weapon": "Dagger",
        "special_item": "AlchemistKit",
        "spellbook": "",
        "resources": [("LivingBark", 20)]
    },

    "Oracle": {
        "class_type": "Oracle",
        "region": "Crystal Barrens",
        "role": "Utility",
        "description": "Diviner who manipulates fate and foresees the future",
        "start_str": 10, "start_dex": 20, "start_int": 50,
        "str_cap": 95, "dex_cap": 110, "int_cap": 145,
        "skills": [
            ("Magery", 55.0),
            ("EvalInt", 50.0),
            ("Meditation", 45.0),
            ("ItemID", 35.0),
            ("MagicResist", 40.0),
            ("DetectHidden", 15.0)
        ],
        "armor_type": "Cloth",
        "weapon": "QuarterStaff",
        "special_item": "CrystalOrb",
        "spellbook": "OracleSpellbook",
        "resources": [("CrystalOre", 20)]
    },

    "Monk": {
        "class_type": "Monk",
        "region": "Ironclad",
        "role": "Melee/Hybrid",
        "description": "Unarmed martial artist with spiritual abilities",
        "start_str": 30, "start_dex": 35, "start_int": 15,
        "str_cap": 120, "dex_cap": 130, "int_cap": 100,
        "skills": [
            ("Wrestling", 55.0),
            ("Tactics", 50.0),
            ("Anatomy", 45.0),
            ("Healing", 40.0),
            ("Focus", 35.0),
            ("Meditation", 15.0)
        ],
        "armor_type": "Cloth",
        "weapon": "",  # Unarmed
        "special_item": "MonkBeads",
        "spellbook": "",
        "resources": [("SteamworkOre", 20)]
    },

    "Templar": {
        "class_type": "Templar",
        "region": "Ironclad",
        "role": "Tank/DPS",
        "description": "Holy warrior combining heavy armor with divine magic",
        "start_str": 35, "start_dex": 25, "start_int": 20,
        "str_cap": 130, "dex_cap": 115, "int_cap": 105,
        "skills": [
            ("Swords", 50.0),
            ("Tactics", 50.0),
            ("Parry", 45.0),
            ("Chivalry", 40.0),
            ("MagicResist", 35.0),
            ("Healing", 20.0)
        ],
        "armor_type": "Plate",
        "weapon": "Longsword",
        "special_item": "TemplarCross",
        "spellbook": "",
        "resources": [("SteamworkOre", 20)]
    },

    "Necromancer": {
        "class_type": "Necromancer",
        "region": "ShadowVoid",
        "role": "Caster/Pet",
        "description": "Master of death magic who commands undead minions",
        "start_str": 15, "start_dex": 15, "start_int": 50,
        "str_cap": 100, "dex_cap": 105, "int_cap": 145,
        "skills": [
            ("Necromancy", 55.0),
            ("SpiritSpeak", 50.0),
            ("Magery", 40.0),
            ("Meditation", 40.0),
            ("MagicResist", 35.0),
            ("EvalInt", 20.0)
        ],
        "armor_type": "Cloth",
        "weapon": "BoneHarvester",
        "special_item": "",
        "spellbook": "VystiaNecromancerSpellbook",
        "resources": [("VoidstoneOre", 20)]
    },

    "Summoner": {
        "class_type": "Summoner",
        "region": "Underwater",
        "role": "Pet/Caster",
        "description": "Conjurer who binds creatures from other planes",
        "start_str": 15, "start_dex": 20, "start_int": 45,
        "str_cap": 100, "dex_cap": 110, "int_cap": 140,
        "skills": [
            ("Magery", 50.0),
            ("EvalInt", 45.0),
            ("Meditation", 45.0),
            ("AnimalLore", 40.0),
            ("MagicResist", 40.0),
            ("Focus", 20.0)
        ],
        "armor_type": "Cloth",
        "weapon": "QuarterStaff",
        "special_item": "SummoningCircle",
        "spellbook": "SummonerSpellbook",
        "resources": [("DeepwaterOre", 20)]
    },

    "BountyHunter": {
        "class_type": "BountyHunter",
        "region": "Multi-Regional",
        "role": "Ranged/Melee",
        "description": "Tracker and assassin who hunts down targets",
        "start_str": 30, "start_dex": 35, "start_int": 15,
        "str_cap": 120, "dex_cap": 130, "int_cap": 100,
        "skills": [
            ("Archery", 50.0),
            ("Tactics", 50.0),
            ("Tracking", 50.0),
            ("Stealth", 40.0),
            ("Hiding", 35.0),
            ("DetectHidden", 15.0)
        ],
        "armor_type": "Leather",
        "weapon": "Crossbow",
        "special_item": "BountyLedger",
        "spellbook": "",
        "resources": [("Gold", 200), ("Bolt", 100)]
    },

    "Knight": {
        "class_type": "Knight",
        "region": "Multi-Regional",
        "role": "Tank/Melee",
        "description": "Honorable armored warrior trained in chivalry",
        "start_str": 38, "start_dex": 27, "start_int": 15,
        "str_cap": 135, "dex_cap": 115, "int_cap": 100,
        "skills": [
            ("Swords", 55.0),
            ("Tactics", 50.0),
            ("Parry", 50.0),
            ("Chivalry", 40.0),
            ("Anatomy", 30.0),
            ("MagicResist", 15.0)
        ],
        "armor_type": "Plate",
        "weapon": "Broadsword",
        "special_item": "KnightBanner",
        "spellbook": "",
        "resources": [("Gold", 150)]
    },

    "Shaman": {
        "class_type": "Shaman",
        "region": "Multi-Regional",
        "role": "Healer/Hybrid",
        "description": "Spiritual guide who channels elemental spirits",
        "start_str": 20, "start_dex": 20, "start_int": 40,
        "str_cap": 110, "dex_cap": 110, "int_cap": 130,
        "skills": [
            ("Magery", 45.0),
            ("SpiritSpeak", 50.0),
            ("Meditation", 45.0),
            ("Healing", 40.0),
            ("MagicResist", 40.0),
            ("AnimalLore", 20.0)
        ],
        "armor_type": "Leather",
        "weapon": "ShepherdsCrook",
        "special_item": "SpiritTotem",
        "spellbook": "ShamanSpellbook",
        "resources": [("Gold", 100)]
    },

    "Bard": {
        "class_type": "Bard",
        "region": "Multi-Regional",
        "role": "Support/CC",
        "description": "Musical enchanter who buffs allies and debuffs enemies",
        "start_str": 15, "start_dex": 35, "start_int": 30,
        "str_cap": 100, "dex_cap": 125, "int_cap": 115,
        "skills": [
            ("Musicianship", 60.0),
            ("Peacemaking", 50.0),
            ("Provocation", 45.0),
            ("Discordance", 40.0),
            ("Magery", 30.0),
            ("MagicResist", 15.0)
        ],
        "armor_type": "Leather",
        "weapon": "Dagger",
        "special_item": "MagicLute",
        "spellbook": "BardSpellbook",
        "resources": [("Gold", 100)]
    },

    "Enchanter": {
        "class_type": "Enchanter",
        "region": "Multi-Regional",
        "role": "Utility/Buff",
        "description": "Magical crafter who enhances items and allies",
        "start_str": 15, "start_dex": 25, "start_int": 40,
        "str_cap": 100, "dex_cap": 115, "int_cap": 130,
        "skills": [
            ("Magery", 50.0),
            ("Inscription", 50.0),
            ("ItemID", 45.0),
            ("Meditation", 40.0),
            ("EvalInt", 35.0),
            ("MagicResist", 20.0)
        ],
        "armor_type": "Cloth",
        "weapon": "QuarterStaff",
        "special_item": "EnchantingCrystal",
        "spellbook": "EnchanterSpellbook",
        "resources": [("Gold", 100)]
    }
}

def create_class_data(class_name: str) -> ClassData:
    """Create ClassData object from definition."""
    data = CLASS_DEFINITIONS[class_name]

    hue = REGIONAL_HUES.get(data["region"], 1153)

    return ClassData(
        name=class_name,
        class_type=data["class_type"],
        region=data["region"],
        role=data["role"],
        description=data["description"],
        start_str=data["start_str"],
        start_dex=data["start_dex"],
        start_int=data["start_int"],
        str_cap=data["str_cap"],
        dex_cap=data["dex_cap"],
        int_cap=data["int_cap"],
        skills=data["skills"],
        hue=hue,
        armor_type=data["armor_type"],
        weapon=data["weapon"],
        special_item=data["special_item"],
        spellbook=data["spellbook"],
        starting_resources=data["resources"]
    )

def generate_armor_equipment(class_data: ClassData) -> str:
    """Generate armor equipment code based on armor type."""
    hue = class_data.hue
    name_prefix = f"{class_data.name}'s"

    if class_data.armor_type == "Plate":
        return f'''m.AddItem(new PlateChest() {{ Name = "{name_prefix} Plate Chest", Hue = {hue} }});
        m.AddItem(new PlateLegs() {{ Hue = {hue} }});
        m.AddItem(new PlateArms() {{ Hue = {hue} }});
        m.AddItem(new PlateGloves() {{ Hue = {hue} }});
        m.AddItem(new PlateGorget() {{ Hue = {hue} }});
        m.AddItem(new PlateHelm() {{ Hue = {hue} }});
        m.AddItem(new MetalKiteShield() {{ Hue = {hue} }});'''

    elif class_data.armor_type == "Chain":
        return f'''m.AddItem(new ChainChest() {{ Name = "{name_prefix} Chain Tunic", Hue = {hue} }});
        m.AddItem(new ChainLegs() {{ Hue = {hue} }});
        m.AddItem(new RingmailArms() {{ Hue = {hue} }});
        m.AddItem(new RingmailGloves() {{ Hue = {hue} }});
        m.AddItem(new ChainCoif() {{ Hue = {hue} }});
        m.AddItem(new MetalShield() {{ Hue = {hue} }});'''

    elif class_data.armor_type == "Leather":
        return f'''m.AddItem(new LeatherChest() {{ Name = "{name_prefix} Jerkin", Hue = {hue} }});
        m.AddItem(new LeatherLegs() {{ Hue = {hue} }});
        m.AddItem(new LeatherArms() {{ Hue = {hue} }});
        m.AddItem(new LeatherGloves() {{ Hue = {hue} }});
        m.AddItem(new LeatherGorget() {{ Hue = {hue} }});
        m.AddItem(new LeatherCap() {{ Hue = {hue} }});'''

    else:  # Cloth
        return f'''m.AddItem(new Robe() {{ Name = "{name_prefix} Robe", Hue = {hue} }});
        m.AddItem(new Sandals() {{ Hue = {hue} }});
        m.AddItem(new WizardsHat() {{ Hue = {hue} }});'''

def generate_weapon_equipment(class_data: ClassData) -> str:
    """Generate weapon equipment code."""
    if not class_data.weapon:
        return "        // Unarmed combat - no weapon"

    hue = class_data.hue
    return f'''m.AddItem(new {class_data.weapon}() {{ Name = "{class_data.name}'s {class_data.weapon}", Hue = {hue} }});'''

def generate_resources_code(class_data: ClassData) -> str:
    """Generate starting resources code."""
    code_lines = []
    for resource_name, quantity in class_data.starting_resources:
        code_lines.append(f"m.Backpack.DropItem(new {resource_name}({quantity}));")

    return "\n        ".join(code_lines)

def generate_special_item_code(class_data: ClassData) -> str:
    """Generate special item giving code."""
    if not class_data.special_item:
        return "        // No special class item"

    return f'''{class_data.special_item} item = new {class_data.special_item}();
        m.Backpack.DropItem(item);'''

def generate_spellbook_code(class_data: ClassData) -> str:
    """Generate spellbook giving code."""
    if not class_data.spellbook:
        return ""

    return f'''
        // Give class spellbook
        {class_data.spellbook} spellbook = new {class_data.spellbook}();
        m.Backpack.DropItem(spellbook);'''

def generate_class_file(class_data: ClassData, output_dir: Path) -> str:
    """Generate complete C# class file."""

    # Generate skills array and values array
    skills_array = ", ".join([f"SkillName.{skill}" for skill, _ in class_data.skills])
    values_array = ", ".join([f"{value}" for _, value in class_data.skills])

    template = f'''using System;
using Server;
using Server.Items;
using Server.Items.VystiaClassItems;

namespace Server.Custom.VystiaClasses
{{
    /// <summary>
    /// {class_data.name} - {class_data.description}
    /// Region: {class_data.region}
    /// Role: {class_data.role}
    /// </summary>
    public class {class_data.class_type}Class : PlayerClass
    {{
        public override PlayerClassType ClassType => PlayerClassType.{class_data.class_type};
        public override string ClassName => "{class_data.name}";
        public override string ClassDescription => "{class_data.description}";
        public override string Region => "{class_data.region}";

        // Stats (Total: {class_data.start_str + class_data.start_dex + class_data.start_int})
        public override int StartStr => {class_data.start_str};
        public override int StartDex => {class_data.start_dex};
        public override int StartInt => {class_data.start_int};

        // Stat Caps
        public override int StrCap => {class_data.str_cap};
        public override int DexCap => {class_data.dex_cap};
        public override int IntCap => {class_data.int_cap};

        // Primary Skills (6 skills, total: {sum([value for _, value in class_data.skills])})
        public override SkillName[] PrimarySkills => new SkillName[]
        {{
            {skills_array}
        }};

        public override double[] StartingSkillValues => new double[]
        {{
            {values_array}
        }};

        public override void EquipStartingGear(Mobile m)
        {{
            if (m == null || m.Backpack == null)
                return;

            // {class_data.armor_type} Armor
            {generate_armor_equipment(class_data)}

            // Weapon
            {generate_weapon_equipment(class_data)}

            // Starting Resources
            {generate_resources_code(class_data)}
        }}

        public override void GiveStartingAbilities(Mobile m)
        {{
            if (m == null || m.Backpack == null)
                return;

            // Special Class Item
            {generate_special_item_code(class_data)}{generate_spellbook_code(class_data)}
        }}
    }}
}}
'''

    return template

def main():
    """Main execution function."""
    print("=" * 80)
    print("VYSTIA CLASS GENERATOR")
    print("=" * 80)
    print()

    # Parse command line args
    dry_run = '--dry-run' in sys.argv
    class_filter = None
    if '--class' in sys.argv:
        idx = sys.argv.index('--class')
        if idx + 1 < len(sys.argv):
            class_filter = sys.argv[idx + 1]

    # Set paths
    base_dir = Path(__file__).parent.parent
    output_dir = base_dir.parent / "ServUO" / "Scripts" / "Custom" / "VystiaClasses" / "Classes"

    if '--output' in sys.argv:
        idx = sys.argv.index('--output')
        if idx + 1 < len(sys.argv):
            output_dir = Path(sys.argv[idx + 1])

    print(f"Output directory: {output_dir}")
    print(f"Dry run: {dry_run}")
    if class_filter:
        print(f"Class filter: {class_filter}")
    print()

    # Generate classes
    classes_to_generate = []
    total_classes = 0

    for class_name in CLASS_DEFINITIONS.keys():
        if class_filter and class_name != class_filter:
            continue

        class_data = create_class_data(class_name)
        classes_to_generate.append(class_data)
        total_classes += 1

        print(f"Processing {class_name}...")
        print(f"  Region: {class_data.region}")
        print(f"  Stats: {class_data.start_str}/{class_data.start_dex}/{class_data.start_int} (Total: {class_data.start_str + class_data.start_dex + class_data.start_int})")
        print(f"  Skills: {len(class_data.skills)} primary skills")

    print()
    print(f"TOTAL: {total_classes} classes to generate")
    print()

    if dry_run:
        print("DRY RUN - No files will be created")
        print()
        for class_data in classes_to_generate:
            print(f"{class_data.name}:")
            print(f"  File: {class_data.class_type}Class.cs")
            print(f"  Role: {class_data.role}")
            print(f"  Armor: {class_data.armor_type}")
            print(f"  Weapon: {class_data.weapon or 'Unarmed'}")
            if class_data.spellbook:
                print(f"  Spellbook: {class_data.spellbook}")
            if class_data.special_item:
                print(f"  Special Item: {class_data.special_item}")
            print()
        return

    # Generate class files
    print("Generating class files...")
    files_created = 0

    output_dir.mkdir(parents=True, exist_ok=True)

    for class_data in classes_to_generate:
        class_file = output_dir / f"{class_data.class_type}Class.cs"
        class_content = generate_class_file(class_data, output_dir)

        with open(class_file, 'w', encoding='utf-8') as f:
            f.write(class_content)

        files_created += 1
        print(f"  Created {class_data.class_type}Class.cs")

    print()
    print(f"Created {files_created} class files")
    print()

    print("=" * 80)
    print("GENERATION COMPLETE!")
    print("=" * 80)
    print(f"Created {files_created} class files")
    print()
    print("Next steps:")
    print("1. Review generated files")
    print("2. Create special class items (if needed)")
    print("3. Update PlayerClass.GetClass() factory method")
    print("4. Build with: cd ServUO && dotnet build")
    print("5. Test classes in-game")
    print()
    print("Note: Some classes may need special items implemented:")
    for class_data in classes_to_generate:
        if class_data.special_item and class_data.special_item not in ["", "Gold"]:
            print(f"  - {class_data.special_item} ({class_data.name})")
    print()

if __name__ == "__main__":
    main()
