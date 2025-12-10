"""
Vystia Remaining Armor Generator
Generates Chain, Ring, Leather armor sets + Legendary armor sets
"""

import os

# Base paths
BASE_PATH = r"C:\DevEnv\GIT\UO"
SERVUO_PATH = os.path.join(BASE_PATH, "ServUO", "Scripts", "Items", "Vystia", "Equipment", "Armor")

# ============================================================================
# CHAIN ARMOR SETS
# ============================================================================

CHAIN_SETS = [
    {
        "name": "Crystal Chain",
        "region": "CRYSTAL",
        "hue": 1154,
        "description": "Energy-infused chain mail",
        "stats_code": "Attributes.BonusInt = 3;\n            Attributes.LowerManaCost = 5;\n            // TODO: Energy resist via BaseEnergyResistance override"
    },
    {
        "name": "Shadow Chain",
        "region": "SHADOW",
        "hue": 1109,
        "description": "Stealth-enhancing dark chain",
        "stats_code": "Attributes.NightSight = 1;\n            Attributes.RegenStam = 2;\n            // Stealth bonus - TODO: SkillBonuses"
    },
    {
        "name": "Desert Chain",
        "region": "DESERT",
        "hue": 2305,
        "description": "Lightweight sand-forged chain",
        "stats_code": "Attributes.BonusDex = 3;\n            Weight = Weight * 0.7; // 30% lighter"
    }
]

CHAIN_PIECES = [
    ("Coif", "ChainCoif", 10),
    ("Tunic", "ChainChest", 20),
    ("Legs", "ChainLegs", 18)
]

# ============================================================================
# RING ARMOR SETS
# ============================================================================

RING_SETS = [
    {
        "name": "Living Ring",
        "region": "VERDANTPEAK",
        "hue": 2010,
        "description": "Nature-blessed ring mail",
        "stats_code": "Attributes.RegenHits = 2;\n            Attributes.Luck = 50;"
    },
    {
        "name": "Steam Ring",
        "region": "IRONCLAD",
        "hue": 2401,
        "description": "Clockwork-enhanced ring mail",
        "stats_code": "Attributes.BonusStr = 3;\n            Attributes.WeaponSpeed = 5;"
    }
]

RING_PIECES = [
    ("Tunic", "RingmailChest", 18),
    ("Sleeves", "RingmailArms", 14),
    ("Gloves", "RingmailGloves", 10),
    ("Legs", "RingmailLegs", 16)
]

# ============================================================================
# LEATHER ARMOR SETS
# ============================================================================

LEATHER_SETS = [
    {
        "name": "Frost Leather",
        "region": "FROSTHOLD",
        "hue": 1152,
        "description": "Winter wolf hide armor",
        "stats_code": "Attributes.BonusDex = 5;\n            // TODO: Cold resist via BaseColdResistance override"
    },
    {
        "name": "Fire Leather",
        "region": "EMBERLANDS",
        "hue": 1358,
        "description": "Lava hound hide armor",
        "stats_code": "Attributes.RegenStam = 3;\n            // TODO: Fire resist via BaseFireResistance override"
    },
    {
        "name": "Shadow Leather",
        "region": "SHADOWFEN",
        "hue": 1109,
        "description": "Shadow wolf hide armor",
        "stats_code": "Attributes.NightSight = 1;\n            Attributes.BonusDex = 3;\n            // Stealth +20 - TODO: SkillBonuses"
    }
]

LEATHER_PIECES = [
    ("Cap", "LeatherCap", 4),
    ("Gorget", "LeatherGorget", 3),
    ("Tunic", "LeatherChest", 12),
    ("Sleeves", "LeatherArms", 8),
    ("Gloves", "LeatherGloves", 3),
    ("Legs", "LeatherLegs", 10)
]

# ============================================================================
# LEGENDARY ARMOR SETS
# ============================================================================

LEGENDARY_SETS = [
    {
        "name": "Glacial Aegis",
        "type": "plate",
        "hue": 1152,
        "pieces": [
            ("Helm", "PlateHelm"),
            ("Gorget", "PlateGorget"),
            ("Chest", "PlateChest"),
            ("Arms", "PlateArms"),
            ("Gloves", "PlateGloves"),
            ("Legs", "PlateLegs")
        ],
        "stats_per_piece": {
            "bonuses": "Attributes.BonusStr = 3;\n            Attributes.DefendChance = 10;\n            Attributes.ReflectPhysical = 5;",
            "skills": "SkillBonuses.SetValues(0, SkillName.Parry, 5.0);",
            "description": "Forged from eternal ice by the Frost Father"
        }
    },
    {
        "name": "Steamwork Exosuit",
        "type": "plate",
        "hue": 2401,
        "pieces": [
            ("Helm", "PlateHelm"),
            ("Gorget", "PlateGorget"),
            ("Chest", "PlateChest"),
            ("Arms", "PlateArms"),
            ("Gloves", "PlateGloves"),
            ("Legs", "PlateLegs")
        ],
        "stats_per_piece": {
            "bonuses": "Attributes.BonusStr = 5;\n            Attributes.BonusDex = -2;\n            Attributes.WeaponSpeed = 10;",
            "skills": "SkillBonuses.SetValues(0, SkillName.Blacksmith, 5.0);",
            "description": "Mechanized armor of the Clockwork Titan"
        }
    },
    {
        "name": "Shadow Shroud",
        "type": "leather",
        "hue": 1109,
        "pieces": [
            ("Cap", "LeatherCap"),
            ("Gorget", "LeatherGorget"),
            ("Tunic", "LeatherChest"),
            ("Sleeves", "LeatherArms"),
            ("Gloves", "LeatherGloves"),
            ("Legs", "LeatherLegs")
        ],
        "stats_per_piece": {
            "bonuses": "Attributes.BonusDex = 5;\n            Attributes.NightSight = 1;\n            Attributes.RegenStam = 2;",
            "skills": "SkillBonuses.SetValues(0, SkillName.Stealth, 5.0);\n            SkillBonuses.SetValues(1, SkillName.Hiding, 5.0);",
            "description": "Shadowweave armor of the Shadow Master"
        }
    }
]

# ============================================================================
# CODE GENERATION FUNCTIONS
# ============================================================================

def generate_armor_piece(set_def: dict, piece_name: str, base_class: str, material: int, set_type: str) -> str:
    """Generate a single armor piece (chain/ring/leather)"""

    class_name = set_def["name"].replace(" ", "") + piece_name
    full_name = f"{set_def['name']} {piece_name}"

    return f"""    public class {class_name} : {base_class}
    {{
        [Constructable]
        public {class_name}()
        {{
            Name = "{full_name}";
            Hue = {set_def['hue']};

            // {set_def['description']}
            {set_def['stats_code']}
        }}

        public override void GetProperties(ObjectPropertyList list)
        {{
            base.GetProperties(list);
            list.Add("{set_def['region']} {set_type.title()} Armor");
        }}

        public {class_name}(Serial serial) : base(serial) {{ }}

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
"""

def generate_legendary_armor_piece(set_def: dict, piece_name: str, base_class: str, piece_index: int) -> str:
    """Generate a legendary armor piece"""

    class_name = set_def["name"].replace(" ", "") + piece_name
    full_name = f"{set_def['name']} {piece_name}"
    armor_rating = 50 + (piece_index * 2)
    set_position = piece_index + 1

    return f"""    public class {class_name} : {base_class}
    {{
        [Constructable]
        public {class_name}()
        {{
            Name = "{full_name}";
            Hue = {set_def['hue']};

            // Legendary stats
            {set_def['stats_per_piece']['skills']}

            {set_def['stats_per_piece']['bonuses']}

            BaseArmorRating = {armor_rating};
        }}

        public override void GetProperties(ObjectPropertyList list)
        {{
            base.GetProperties(list);
            list.Add(1060658, "Artifact\\tLegendary");
            list.Add("{set_def['stats_per_piece']['description']}");
            list.Add("Set: {set_def['name']} ({set_position}/6)");
        }}

        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;

        public {class_name}(Serial serial) : base(serial) {{ }}

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
"""

def generate_chain_armor_file():
    """Generate chain armor sets file"""

    code = """using System;
using Server;

namespace Server.Items.Vystia
{
"""

    for chain_set in CHAIN_SETS:
        code += f"""    // ============================================================================
    // {chain_set['name'].upper()}
    // {chain_set['description']}
    // ============================================================================

"""
        for piece_name, base_class, material in CHAIN_PIECES:
            code += generate_armor_piece(chain_set, piece_name, base_class, material, "chain") + "\n"

    code += "}\n"
    return code

def generate_ring_armor_file():
    """Generate ring armor sets file"""

    code = """using System;
using Server;

namespace Server.Items.Vystia
{
"""

    for ring_set in RING_SETS:
        code += f"""    // ============================================================================
    // {ring_set['name'].upper()}
    // {ring_set['description']}
    // ============================================================================

"""
        for piece_name, base_class, material in RING_PIECES:
            code += generate_armor_piece(ring_set, piece_name, base_class, material, "ring") + "\n"

    code += "}\n"
    return code

def generate_leather_armor_file():
    """Generate leather armor sets file"""

    code = """using System;
using Server;

namespace Server.Items.Vystia
{
"""

    for leather_set in LEATHER_SETS:
        code += f"""    // ============================================================================
    // {leather_set['name'].upper()}
    // {leather_set['description']}
    // ============================================================================

"""
        for piece_name, base_class, material in LEATHER_PIECES:
            code += generate_armor_piece(leather_set, piece_name, base_class, material, "leather") + "\n"

    code += "}\n"
    return code

def generate_legendary_sets_file():
    """Generate legendary armor sets file"""

    code = """using System;
using Server;

namespace Server.Items.Vystia
{
    // ============================================================================
    // LEGENDARY ARMOR SETS
    // Complete sets with powerful bonuses
    // ============================================================================

"""

    for legendary_set in LEGENDARY_SETS:
        code += f"""    // ============================================================================
    // {legendary_set['name'].upper()}
    // Type: {legendary_set['type'].title()} Armor
    // Hue: {legendary_set['hue']}
    // ============================================================================

"""
        for piece_index, (piece_name, base_class) in enumerate(legendary_set['pieces']):
            code += generate_legendary_armor_piece(legendary_set, piece_name, base_class, piece_index) + "\n"

    code += "}\n"
    return code

# ============================================================================
# MAIN EXECUTION
# ============================================================================

if __name__ == "__main__":
    print("Vystia Remaining Armor Generator")
    print("=" * 60)

    # Create output directory
    os.makedirs(SERVUO_PATH, exist_ok=True)

    # Generate chain armor
    chain_total = len(CHAIN_SETS) * len(CHAIN_PIECES)
    print(f"\nGenerating {chain_total} chain armor pieces ({len(CHAIN_SETS)} sets)...")
    chain_code = generate_chain_armor_file()
    chain_path = os.path.join(SERVUO_PATH, "RegionalChainArmor.cs")
    with open(chain_path, 'w', encoding='utf-8') as f:
        f.write(chain_code)
    print(f"[OK] Created: {chain_path}")
    for chain_set in CHAIN_SETS:
        print(f"  - {chain_set['name']} ({len(CHAIN_PIECES)} pieces)")

    # Generate ring armor
    ring_total = len(RING_SETS) * len(RING_PIECES)
    print(f"\nGenerating {ring_total} ring armor pieces ({len(RING_SETS)} sets)...")
    ring_code = generate_ring_armor_file()
    ring_path = os.path.join(SERVUO_PATH, "RegionalRingArmor.cs")
    with open(ring_path, 'w', encoding='utf-8') as f:
        f.write(ring_code)
    print(f"[OK] Created: {ring_path}")
    for ring_set in RING_SETS:
        print(f"  - {ring_set['name']} ({len(RING_PIECES)} pieces)")

    # Generate leather armor
    leather_total = len(LEATHER_SETS) * len(LEATHER_PIECES)
    print(f"\nGenerating {leather_total} leather armor pieces ({len(LEATHER_SETS)} sets)...")
    leather_code = generate_leather_armor_file()
    leather_path = os.path.join(SERVUO_PATH, "RegionalLeatherArmor.cs")
    with open(leather_path, 'w', encoding='utf-8') as f:
        f.write(leather_code)
    print(f"[OK] Created: {leather_path}")
    for leather_set in LEATHER_SETS:
        print(f"  - {leather_set['name']} ({len(LEATHER_PIECES)} pieces)")

    # Generate legendary sets
    legendary_total = sum(len(s['pieces']) for s in LEGENDARY_SETS)
    print(f"\nGenerating {legendary_total} legendary armor pieces ({len(LEGENDARY_SETS)} sets)...")
    legendary_code = generate_legendary_sets_file()
    legendary_path = os.path.join(SERVUO_PATH, "LegendaryArmorSets.cs")
    with open(legendary_path, 'w', encoding='utf-8') as f:
        f.write(legendary_code)
    print(f"[OK] Created: {legendary_path}")
    for legendary_set in LEGENDARY_SETS:
        print(f"  - {legendary_set['name']} ({len(legendary_set['pieces'])} pieces)")

    total_pieces = chain_total + ring_total + leather_total + legendary_total
    print("\n" + "=" * 60)
    print(f"[OK] Generated {total_pieces} total armor pieces")
    print(f"  - Chain: {chain_total}")
    print(f"  - Ring: {ring_total}")
    print(f"  - Leather: {leather_total}")
    print(f"  - Legendary: {legendary_total}")
    print(f"[OK] Files created in: {SERVUO_PATH}")
    print("\nNext: Run this script, then build ServUO to test")
