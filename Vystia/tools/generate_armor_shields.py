"""
Vystia Armor & Shield Generator
Generates all regional armor sets and shields with full stats
"""

import os

# Base paths
BASE_PATH = r"C:\DevEnv\GIT\UO"
SERVUO_PATH = os.path.join(BASE_PATH, "ServUO", "Scripts", "Items", "Vystia", "Equipment")

# ============================================================================
# PLATE ARMOR SETS
# ============================================================================

PLATE_SETS = [
    {
        "name": "Frostforged Plate",
        "region": "FROSTHOLD",
        "hue": 1152,
        "ingots": 100,
        "set_bonus": "Cold immunity, +15 Cold Resist",
        "per_piece": "Cold resistance, durability",
        "stats_code": "Attributes.DefendChance = 5;\n            // TODO: Add cold resist via BaseResistance override"
    },
    {
        "name": "Emberforged Plate",
        "region": "EMBERLANDS",
        "hue": 1358,
        "ingots": 100,
        "set_bonus": "Fire immunity, +15 Fire Resist",
        "per_piece": "Fire resistance, durability",
        "stats_code": "Attributes.DefendChance = 5;\n            // TODO: Add fire resist via BaseResistance override"
    },
    {
        "name": "Clockwork Plate",
        "region": "IRONCLAD",
        "hue": 2401,
        "ingots": 100,
        "set_bonus": "+10 STR, +10 Stamina Regen",
        "per_piece": "STR bonus, stamina regen",
        "stats_code": "Attributes.BonusStr = 2;\n            Attributes.RegenStam = 1;"
    },
    {
        "name": "Voidforged Plate",
        "region": "SHADOWVOID",
        "hue": 1109,
        "ingots": 100,
        "set_bonus": "Mage Armor, +10 Magic Resist",
        "per_piece": "Spell Channeling, mage friendly",
        "stats_code": "Attributes.SpellChanneling = 1;\n            Attributes.CastSpeed = -1;"
    }
]

PLATE_PIECES = [
    ("Helm", "PlateHelm", 15),
    ("Gorget", "PlateGorget", 10),
    ("Chest", "PlateChest", 25),
    ("Arms", "PlateArms", 18),
    ("Gloves", "PlateGloves", 12),
    ("Legs", "PlateLegs", 20)
]

# ============================================================================
# REGIONAL SHIELDS
# ============================================================================

SHIELDS = [
    {
        "name": "Ice Wall",
        "base_class": "HeaterShield",
        "region": "FROSTHOLD",
        "hue": 1152,
        "ingots": 18,
        "stats_code": "Attributes.DefendChance = 10;\n            Attributes.ReflectPhysical = 5;\n            // TODO: Add cold resist via BaseResistance override"
    },
    {
        "name": "Flame Guard",
        "base_class": "MetalKiteShield",
        "region": "EMBERLANDS",
        "hue": 1358,
        "ingots": 16,
        "stats_code": "Attributes.DefendChance = 10;\n            // TODO: Add fire resist + counter-attack via BaseResistance & OnHit"
    },
    {
        "name": "Prism Shield",
        "base_class": "MetalShield",
        "region": "CRYSTAL",
        "hue": 1154,
        "ingots": 14,
        "stats_code": "Attributes.SpellChanneling = 1;\n            Attributes.DefendChance = 10;\n            // TODO: Add energy resist via BaseResistance override"
    },
    {
        "name": "Clockwork Shield",
        "base_class": "OrderShield",
        "region": "IRONCLAD",
        "hue": 2401,
        "ingots": 20,
        "stats_code": "Attributes.BonusDex = 5;\n            Attributes.DefendChance = 15;"
    },
    {
        "name": "Bog Shield",
        "base_class": "WoodenShield",
        "region": "SHADOWFEN",
        "hue": 2212,
        "ingots": 0,
        "boards": 8,
        "stats_code": "Attributes.DefendChance = 10;\n            // TODO: Add poison resist + counter via BaseResistance & OnHit"
    },
    {
        "name": "Sand Shield",
        "base_class": "Buckler",
        "region": "DESERT",
        "hue": 2305,
        "ingots": 10,
        "stats_code": "Attributes.BonusDex = 10;\n            Weight = 2.0;"
    },
    {
        "name": "Void Shield",
        "base_class": "ChaosShield",
        "region": "OBSIDIAN",
        "hue": 1109,
        "ingots": 22,
        "stats_code": "Attributes.SpellChanneling = 1;\n            Attributes.CastSpeed = -1;\n            Attributes.DefendChance = 10;"
    },
    {
        "name": "Living Shield",
        "base_class": "WoodenKiteShield",
        "region": "VERDANTPEAK",
        "hue": 2010,
        "ingots": 0,
        "boards": 12,
        "stats_code": "Attributes.RegenHits = 2;\n            Attributes.DefendChance = 5;"
    }
]

# ============================================================================
# CODE GENERATION FUNCTIONS
# ============================================================================

def generate_plate_piece(set_def: dict, piece_name: str, base_class: str, ingots: int) -> str:
    """Generate a single plate armor piece"""

    class_name = set_def["name"].replace(" ", "") + piece_name
    full_name = f"{set_def['name']} {piece_name}"

    return f"""    public class {class_name} : {base_class}
    {{
        [Constructable]
        public {class_name}()
        {{
            Name = "{full_name}";
            Hue = {set_def['hue']};

            // Regional armor stats
            {set_def['stats_code']}
        }}

        public override void GetProperties(ObjectPropertyList list)
        {{
            base.GetProperties(list);
            list.Add("{set_def['region']} Crafted");
            list.Add("Part of {set_def['name']} Set");
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

def generate_shield(shield_def: dict) -> str:
    """Generate a regional shield"""

    class_name = shield_def["name"].replace(" ", "")

    return f"""    public class {class_name} : {shield_def['base_class']}
    {{
        [Constructable]
        public {class_name}()
        {{
            Name = "{shield_def['name']}";
            Hue = {shield_def['hue']};

            // Regional shield stats
            {shield_def['stats_code']}
        }}

        public override void GetProperties(ObjectPropertyList list)
        {{
            base.GetProperties(list);
            list.Add("{shield_def['region']} Crafted");
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

def generate_plate_armor_file():
    """Generate complete plate armor sets file"""

    code = """using System;
using Server;

namespace Server.Items.Vystia
{
"""

    for plate_set in PLATE_SETS:
        code += f"""    // ============================================================================
    // {plate_set['name'].upper()}
    // Set Bonus: {plate_set['set_bonus']}
    // ============================================================================

"""
        for piece_name, base_class, ingots in PLATE_PIECES:
            code += generate_plate_piece(plate_set, piece_name, base_class, ingots) + "\n"

    code += "}\n"
    return code

def generate_shields_file():
    """Generate regional shields file"""

    code = """using System;
using Server;

namespace Server.Items.Vystia
{
    // ============================================================================
    // REGIONAL SHIELDS
    // ============================================================================

"""

    for shield in SHIELDS:
        code += generate_shield(shield) + "\n"

    code += "}\n"
    return code

# ============================================================================
# LEGENDARY ARMOR
# ============================================================================

LEGENDARY_ARMOR = [
    {
        "name": "Glacial Aegis",
        "type": "Plate Set",
        "hue": 1152,
        "pieces": 6,
        "description": "Complete plate set - Cold immunity, 15% reflect damage"
    },
    {
        "name": "Molten Core",
        "type": "PlateChest",
        "hue": 1358,
        "description": "Legendary platemail - Fire Resist +30, STR +10"
    },
    {
        "name": "Steamwork Exosuit",
        "type": "Plate Set",
        "hue": 2401,
        "pieces": 6,
        "description": "Complete set - STR +20, DEX -10, Self-Repair 5"
    },
    {
        "name": "Shadow Shroud",
        "type": "Leather Set",
        "hue": 1109,
        "pieces": 6,
        "description": "Complete leather set - Stealth +30, Hiding +30"
    }
]

def generate_legendary_chest(name: str, hue: int, description: str) -> str:
    """Generate a legendary chest piece"""

    class_name = name.replace(" ", "")

    return f"""    public class {class_name} : PlateChest
    {{
        [Constructable]
        public {class_name}()
        {{
            Name = "{name}";
            Hue = {hue};

            // Legendary stats
            SkillBonuses.SetValues(0, SkillName.Parry, 15.0);

            Attributes.BonusStr = 10;
            Attributes.RegenHits = 3;
            Attributes.DefendChance = 15;

            BaseArmorRating = 60;

            Resistances.Fire = 30;
            Resistances.Physical = 20;
        }}

        public override void GetProperties(ObjectPropertyList list)
        {{
            base.GetProperties(list);
            list.Add(1060658, "Artifact\\tLegendary");
            list.Add("{description}");
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

def generate_legendary_armor_file():
    """Generate legendary armor file"""

    code = """using System;
using Server;

namespace Server.Items.Vystia
{
    // ============================================================================
    // LEGENDARY ARMOR
    // Note: Full sets need individual piece implementation
    // ============================================================================

"""

    # Generate Molten Core as example
    code += generate_legendary_chest(
        "Molten Core",
        1358,
        "Forged in the heart of the Volcano Wyrm"
    )

    code += """
    // TODO: Implement full sets for:
    // - Glacial Aegis (6 pieces)
    // - Steamwork Exosuit (6 pieces)
    // - Shadow Shroud (6 pieces)
"""

    code += "}\n"
    return code

# ============================================================================
# MAIN EXECUTION
# ============================================================================

if __name__ == "__main__":
    print("Vystia Armor & Shield Generator")
    print("=" * 60)

    # Create output directories
    armor_dir = os.path.join(SERVUO_PATH, "Armor")
    shields_dir = os.path.join(SERVUO_PATH, "Shields")
    os.makedirs(armor_dir, exist_ok=True)
    os.makedirs(shields_dir, exist_ok=True)

    # Generate plate armor sets
    total_pieces = len(PLATE_SETS) * len(PLATE_PIECES)
    print(f"\nGenerating {total_pieces} plate armor pieces ({len(PLATE_SETS)} sets)...")
    plate_code = generate_plate_armor_file()
    plate_path = os.path.join(armor_dir, "RegionalPlateArmor.cs")
    with open(plate_path, 'w', encoding='utf-8') as f:
        f.write(plate_code)
    print(f"[OK] Created: {plate_path}")
    for plate_set in PLATE_SETS:
        print(f"  - {plate_set['name']} (6 pieces)")

    # Generate shields
    print(f"\nGenerating {len(SHIELDS)} regional shields...")
    shields_code = generate_shields_file()
    shields_path = os.path.join(shields_dir, "RegionalShields.cs")
    with open(shields_path, 'w', encoding='utf-8') as f:
        f.write(shields_code)
    print(f"[OK] Created: {shields_path}")
    for shield in SHIELDS:
        print(f"  - {shield['name']} ({shield['base_class']})")

    # Generate legendary armor
    print(f"\nGenerating legendary armor...")
    legendary_code = generate_legendary_armor_file()
    legendary_path = os.path.join(armor_dir, "LegendaryArmor.cs")
    with open(legendary_path, 'w', encoding='utf-8') as f:
        f.write(legendary_code)
    print(f"[OK] Created: {legendary_path}")
    print(f"  - Molten Core (PlateChest)")
    print(f"  - TODO: 3 more legendary sets")

    print("\n" + "=" * 60)
    print(f"[OK] Generated {total_pieces + len(SHIELDS) + 1} total armor/shield items")
    print(f"[OK] Armor files in: {armor_dir}")
    print(f"[OK] Shield files in: {shields_dir}")
    print("\nNext: Run this script, then build ServUO to test")
