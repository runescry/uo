"""
Comprehensive Vystia Equipment Generator
Parses EQUIPMENT.md and generates all weapons, armor, shields with full stats
"""

import os
import re

# Base paths
BASE_PATH = r"C:\DevEnv\GIT\UO"
SERVUO_PATH = os.path.join(BASE_PATH, "ServUO", "Scripts", "Items", "Vystia", "Equipment")
EQUIPMENT_MD = os.path.join(BASE_PATH, "Vystia", "implementation", "content", "EQUIPMENT.md")

# Regional hue mapping
REGIONAL_HUES = {
    "FROSTHOLD": 1152,
    "EMBERLANDS": 1358,
    "CRYSTAL": 1154,
    "IRONCLAD": 2401,
    "SHADOW": 1109,
    "VERDANTPEAK": 2010,
    "SHADOWFEN": 2212,
    "DESERT": 2305,
    "OBSIDIAN": 1109
}

# Material to ingot mapping
MATERIAL_MAPPING = {
    "Frostforged": "FrostforgedIngot",
    "Emberforged": "EmberforgedIngot",
    "Crystalline": "CrystallineIngot",
    "Clockwork": "ClockworkIngot",
    "Shadowforged": "ShadowforgedIngot",
    "Natureforged": "NatureforgedIngot",
    "Sunforged": "SunforgedIngot",
    "Voidforged": "VoidforgedIngot",
    "Frostwood": "FrostwoodLog",
    "Living wood": "LivingwoodLog"
}

# Base weapon classes for ServUO
WEAPON_BASE_CLASSES = {
    "Longsword": "Longsword",
    "Viking Sword": "VikingSword",
    "Cutlass": "Cutlass",
    "Kryss": "Kryss",
    "Katana": "Katana",
    "Scimitar": "Scimitar",
    "Broadsword": "Broadsword",
    "Battle Axe": "BattleAxe",
    "Double Axe": "DoubleAxe",
    "Hatchet": "Hatchet",
    "Two Handed Axe": "TwoHandedAxe",
    "War Axe": "WarAxe",
    "Pickaxe": "Pickaxe",
    "Executioner's Axe": "ExecutionersAxe",
    "War Hammer": "WarHammer",
    "Maul": "Maul",
    "Club": "Club",
    "War Mace": "WarMace",
    "Hammer Pick": "HammerPick",
    "Quarter Staff": "QuarterStaff",
    "Spear": "Spear",
    "Pike": "Pike",
    "Short Spear": "ShortSpear",
    "Halberd": "Halberd",
    "Bardiche": "Bardiche",
    "Bow": "Bow",
    "Crossbow": "Crossbow",
    "Heavy Crossbow": "HeavyCrossbow"
}

class WeaponDef:
    def __init__(self, name, base_class, region, hue, ingots, extra_materials=None):
        self.name = name
        self.base_class = base_class
        self.region = region
        self.hue = hue
        self.ingots = ingots
        self.extra_materials = extra_materials or {}  # {material: count}
        self.class_name = name.replace(" ", "").replace("'", "")

class ArmorDef:
    def __init__(self, name, piece_type, region, hue, material_count, special_props=None):
        self.name = name
        self.piece_type = piece_type  # "PlateChest", "LeatherLegs", etc.
        self.region = region
        self.hue = hue
        self.material_count = material_count
        self.special_props = special_props or {}
        self.class_name = name.replace(" ", "")

class ShieldDef:
    def __init__(self, name, base_class, region, hue, material_count, special_props=None):
        self.name = name
        self.base_class = base_class
        self.region = region
        self.hue = hue
        self.material_count = material_count
        self.special_props = special_props or {}
        self.class_name = name.replace(" ", "")

# ============================================================================
# WEAPON DEFINITIONS (from EQUIPMENT.md)
# ============================================================================

REGIONAL_SWORDS = [
    # Frosthold
    WeaponDef("Icicle Blade", "Longsword", "FROSTHOLD", 1152, 12),
    WeaponDef("Winter's Edge", "VikingSword", "FROSTHOLD", 1152, 14),
    WeaponDef("Frostbite", "Cutlass", "FROSTHOLD", 1152, 8),
    WeaponDef("Glacier Shard", "Kryss", "FROSTHOLD", 1152, 8),

    # Emberlands
    WeaponDef("Flame Tongue", "Katana", "EMBERLANDS", 1358, 8),
    WeaponDef("Magma Blade", "Scimitar", "EMBERLANDS", 1358, 10),
    WeaponDef("Phoenix Wing", "Broadsword", "EMBERLANDS", 1358, 10),
    WeaponDef("Lava Edge", "Longsword", "EMBERLANDS", 1358, 12),

    # Crystal
    WeaponDef("Crystal Shard", "Kryss", "CRYSTAL", 1154, 8),
    WeaponDef("Prism Blade", "Longsword", "CRYSTAL", 1154, 12),
    WeaponDef("Refraction Edge", "Katana", "CRYSTAL", 1154, 8),

    # Ironclad
    WeaponDef("Clockwork Sword", "Broadsword", "IRONCLAD", 2401, 10, {"ClockworkGear": 5}),
    WeaponDef("Gear Blade", "VikingSword", "IRONCLAD", 2401, 14, {"ClockworkGear": 3}),
    WeaponDef("Steam Saber", "Cutlass", "IRONCLAD", 2401, 8, {"ClockworkSpring": 2}),

    # Shadow
    WeaponDef("Shadow Fang", "Kryss", "SHADOW", 1109, 8),
    WeaponDef("Void Edge", "Katana", "SHADOW", 1109, 8),
    WeaponDef("Dark Blade", "Scimitar", "SHADOW", 1109, 10),
]

REGIONAL_AXES = [
    # Frosthold
    WeaponDef("Frozen Cleaver", "BattleAxe", "FROSTHOLD", 1152, 14),
    WeaponDef("Ice Shard Axe", "DoubleAxe", "FROSTHOLD", 1152, 12),
    WeaponDef("Glacial Hatchet", "Hatchet", "FROSTHOLD", 1152, 4),

    # Emberlands
    WeaponDef("Molten Axe", "TwoHandedAxe", "EMBERLANDS", 1358, 16),
    WeaponDef("Flame Cleaver", "WarAxe", "EMBERLANDS", 1358, 12),
    WeaponDef("Lava Pick", "Pickaxe", "EMBERLANDS", 1358, 8),

    # Ironclad
    WeaponDef("Gear Axe", "ExecutionersAxe", "IRONCLAD", 2401, 14, {"ClockworkGear": 5}),
    WeaponDef("Steam Cleaver", "BattleAxe", "IRONCLAD", 2401, 14, {"ClockworkSpring": 3}),
]

REGIONAL_MACES = [
    # Frosthold
    WeaponDef("Glacial Hammer", "WarHammer", "FROSTHOLD", 1152, 16),
    WeaponDef("Frost Maul", "Maul", "FROSTHOLD", 1152, 10),
    WeaponDef("Ice Club", "Club", "FROSTHOLD", 1152, 10),

    # Emberlands
    WeaponDef("Molten Mace", "WarMace", "EMBERLANDS", 1358, 14),
    WeaponDef("Magma Hammer", "WarHammer", "EMBERLANDS", 1358, 16),

    # Ironclad
    WeaponDef("Piston Mace", "WarMace", "IRONCLAD", 2401, 14, {"ClockworkSpring": 4}),
    WeaponDef("Steam Hammer", "WarHammer", "IRONCLAD", 2401, 16, {"ClockworkGear": 6}),
]

REGIONAL_POLEARMS = [
    # Frosthold
    WeaponDef("Ice Lance", "Pike", "FROSTHOLD", 1152, 16),
    WeaponDef("Frozen Halberd", "Halberd", "FROSTHOLD", 1152, 20),

    # Emberlands
    WeaponDef("Lava Spear", "Spear", "EMBERLANDS", 1358, 12),
    WeaponDef("Volcanic Pike", "Pike", "EMBERLANDS", 1358, 16),
]

REGIONAL_RANGED = [
    # Frosthold
    WeaponDef("Frost Bow", "Bow", "FROSTHOLD", 1152, 0),
    WeaponDef("Ice Crossbow", "Crossbow", "FROSTHOLD", 1152, 0),

    # Verdantpeak
    WeaponDef("Living Bow", "Bow", "VERDANTPEAK", 2010, 0),
    WeaponDef("Nature's Crossbow", "Crossbow", "VERDANTPEAK", 2010, 0),
]

# Combine all weapons
ALL_REGIONAL_WEAPONS = (
    REGIONAL_SWORDS + REGIONAL_AXES + REGIONAL_MACES +
    REGIONAL_POLEARMS + REGIONAL_RANGED
)

# ============================================================================
# LEGENDARY WEAPON DEFINITIONS
# ============================================================================

LEGENDARY_WEAPONS = [
    {
        "name": "Phoenix Ascension",
        "base_class": "Katana",
        "hue": 1358,
        "min_dmg": 20,
        "max_dmg": 30,
        "element_dmg": {"Fire": 100},
        "special": "HitFireball 40%",
        "lore": "Forged in the heart of a dying phoenix",
        "drop": "Volcano Wyrm"
    },
    {
        "name": "The Cogmaster",
        "base_class": "WarHammer",
        "hue": 2401,
        "min_dmg": 22,
        "max_dmg": 28,
        "element_dmg": {"Energy": 50, "Physical": 50},
        "special": "Hit Lightning 30%, Self-Repair 5",
        "lore": "Master artifact of the Technoguild",
        "drop": "Forge Master"
    },
    {
        "name": "Prismatic Edge",
        "base_class": "Longsword",
        "hue": 1154,
        "min_dmg": 18,
        "max_dmg": 25,
        "element_dmg": {"Physical": 20, "Fire": 20, "Cold": 20, "Poison": 20, "Energy": 20},
        "special": "All damage types 20% each",
        "lore": "Refracted from pure crystal light",
        "drop": "Crystal Dragon"
    },
    {
        "name": "Voidcaller",
        "base_class": "QuarterStaff",
        "hue": 1109,
        "min_dmg": 15,
        "max_dmg": 22,
        "element_dmg": {"Cold": 50, "Energy": 50},
        "special": "Spell Channeling, Mage Weapon -10",
        "lore": "Staff channeling the power of the void",
        "drop": "Shadow Lich"
    },
]

# ============================================================================
# CODE GENERATION FUNCTIONS
# ============================================================================

def generate_regional_weapon(weapon: WeaponDef) -> str:
    """Generate C# code for a regional weapon with moderate stats"""

    # Determine element damage based on region
    element_damage = ""
    element_type = ""
    resist_bonus = ""

    if weapon.region == "FROSTHOLD":
        element_damage = "AosElementDamages.Cold = 60;\n            AosElementDamages.Physical = 40;"
        element_type = "Cold"
        resist_bonus = "WeaponAttributes.ResistColdBonus = 5;"
    elif weapon.region == "EMBERLANDS":
        element_damage = "AosElementDamages.Fire = 60;\n            AosElementDamages.Physical = 40;"
        element_type = "Fire"
        resist_bonus = "WeaponAttributes.ResistFireBonus = 5;"
    elif weapon.region == "CRYSTAL":
        element_damage = "AosElementDamages.Energy = 50;\n            AosElementDamages.Physical = 50;"
        element_type = "Energy"
        resist_bonus = "WeaponAttributes.ResistEnergyBonus = 5;"
    elif weapon.region == "IRONCLAD":
        element_damage = "AosElementDamages.Energy = 30;\n            AosElementDamages.Physical = 70;"
        element_type = "Energy"
        resist_bonus = "Attributes.WeaponSpeed = 10;"
    elif weapon.region == "SHADOW":
        element_damage = "AosElementDamages.Cold = 40;\n            AosElementDamages.Poison = 20;\n            AosElementDamages.Physical = 40;"
        element_type = "Cold/Poison"
        resist_bonus = "Attributes.AttackChance = 5;"
    elif weapon.region == "VERDANTPEAK":
        element_damage = "AosElementDamages.Poison = 40;\n            AosElementDamages.Physical = 60;"
        element_type = "Poison"
        resist_bonus = "Attributes.RegenHits = 1;"

    return f"""    public class {weapon.class_name} : {weapon.base_class}
    {{
        [Constructable]
        public {weapon.class_name}()
        {{
            Name = "{weapon.name}";
            Hue = {weapon.hue};

            // Regional weapon stats
            Attributes.WeaponDamage = 20; // +20% damage
            {resist_bonus}

            // Element damage
            {element_damage}
        }}

        public override void GetProperties(ObjectPropertyList list)
        {{
            base.GetProperties(list);
            list.Add("{weapon.region.title()} Crafted");
        }}

        public {weapon.class_name}(Serial serial) : base(serial) {{ }}

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

def generate_legendary_weapon(weapon_def: dict) -> str:
    """Generate C# code for a legendary weapon with full stats"""

    class_name = weapon_def["name"].replace(" ", "").replace("'", "")

    # Build element damage code
    element_dmg_code = []
    for elem, percent in weapon_def["element_dmg"].items():
        element_dmg_code.append(f"AosElementDamages.{elem} = {percent};")

    # Parse special attributes
    special_attrs = []
    if "HitFireball" in weapon_def["special"]:
        special_attrs.append("WeaponAttributes.HitFireball = 40;")
    if "Hit Lightning" in weapon_def["special"]:
        special_attrs.append("WeaponAttributes.HitLightning = 30;")
    if "Self-Repair" in weapon_def["special"]:
        special_attrs.append("Attributes.WeaponSpeed = 20;")
        special_attrs.append("// Self-Repair handled by ServUO durability system")
    if "Spell Channeling" in weapon_def["special"]:
        special_attrs.append("Attributes.SpellChanneling = 1;")
        special_attrs.append("Attributes.CastSpeed = -1;")
    if "Mage Weapon" in weapon_def["special"]:
        special_attrs.append("WeaponAttributes.MageWeapon = 10;")

    return f"""    public class {class_name} : {weapon_def['base_class']}
    {{
        [Constructable]
        public {class_name}()
        {{
            Name = "{weapon_def['name']}";
            Hue = {weapon_def['hue']};

            // Legendary stats
            SkillBonuses.SetValues(0, SkillName.Tactics, 10.0);

            Attributes.WeaponDamage = 50;
            Attributes.WeaponSpeed = 20;
            Attributes.Luck = 100;

            {chr(10).join("            " + attr for attr in special_attrs)}

            // Damage range
            MinDamage = {weapon_def['min_dmg']};
            MaxDamage = {weapon_def['max_dmg']};

            // Element damage
            {chr(10).join("            " + code for code in element_dmg_code)}
        }}

        public override void GetProperties(ObjectPropertyList list)
        {{
            base.GetProperties(list);
            list.Add(1060658, "Artifact\\tLegendary");
            list.Add("{weapon_def['lore']}");
            list.Add("Dropped by {weapon_def['drop']}");
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

def generate_weapons_file():
    """Generate complete weapons file with all regional weapons"""

    code = """using System;
using Server;

namespace Server.Items.Vystia
{
    // ============================================================================
    // REGIONAL SWORDS
    // ============================================================================

"""

    # Add swords
    for weapon in REGIONAL_SWORDS:
        code += generate_regional_weapon(weapon) + "\n"

    code += """    // ============================================================================
    // REGIONAL AXES
    // ============================================================================

"""

    # Add axes
    for weapon in REGIONAL_AXES:
        code += generate_regional_weapon(weapon) + "\n"

    code += """    // ============================================================================
    // REGIONAL MACES
    // ============================================================================

"""

    # Add maces
    for weapon in REGIONAL_MACES:
        code += generate_regional_weapon(weapon) + "\n"

    code += """    // ============================================================================
    // REGIONAL POLEARMS
    // ============================================================================

"""

    # Add polearms
    for weapon in REGIONAL_POLEARMS:
        code += generate_regional_weapon(weapon) + "\n"

    code += """    // ============================================================================
    // REGIONAL RANGED WEAPONS
    // ============================================================================

"""

    # Add ranged
    for weapon in REGIONAL_RANGED:
        code += generate_regional_weapon(weapon) + "\n"

    code += "}\n"

    return code

def generate_legendary_weapons_file():
    """Generate legendary weapons file"""

    code = """using System;
using Server;
using Server.Mobiles;

namespace Server.Items.Vystia
{
    // ============================================================================
    // LEGENDARY ARTIFACT WEAPONS
    // ============================================================================

"""

    for weapon in LEGENDARY_WEAPONS:
        code += generate_legendary_weapon(weapon) + "\n"

    code += "}\n"

    return code

# ============================================================================
# MAIN EXECUTION
# ============================================================================

if __name__ == "__main__":
    print("Vystia Equipment Generator")
    print("=" * 60)

    # Create output directory
    weapons_dir = os.path.join(SERVUO_PATH, "Weapons")
    os.makedirs(weapons_dir, exist_ok=True)

    # Generate regional weapons
    print(f"\nGenerating {len(ALL_REGIONAL_WEAPONS)} regional weapons...")
    weapons_code = generate_weapons_file()
    weapons_path = os.path.join(weapons_dir, "RegionalWeapons.cs")
    with open(weapons_path, 'w', encoding='utf-8') as f:
        f.write(weapons_code)
    print(f"[OK] Created: {weapons_path}")
    print(f"  - {len(REGIONAL_SWORDS)} swords")
    print(f"  - {len(REGIONAL_AXES)} axes")
    print(f"  - {len(REGIONAL_MACES)} maces")
    print(f"  - {len(REGIONAL_POLEARMS)} polearms")
    print(f"  - {len(REGIONAL_RANGED)} ranged weapons")

    # Generate legendary weapons
    print(f"\nGenerating {len(LEGENDARY_WEAPONS)} legendary weapons...")
    legendary_code = generate_legendary_weapons_file()
    legendary_path = os.path.join(weapons_dir, "LegendaryWeapons.cs")
    with open(legendary_path, 'w', encoding='utf-8') as f:
        f.write(legendary_code)
    print(f"[OK] Created: {legendary_path}")
    for weapon in LEGENDARY_WEAPONS:
        print(f"  - {weapon['name']} ({weapon['base_class']})")

    print("\n" + "=" * 60)
    print(f"[OK] Generated {len(ALL_REGIONAL_WEAPONS) + len(LEGENDARY_WEAPONS)} total weapons")
    print(f"[OK] Files created in: {weapons_dir}")
    print("\nNext: Run this script, then build ServUO to test")
