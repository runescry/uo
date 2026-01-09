#!/usr/bin/env python3
"""
Generate additional legendary armor sets for Vystia by ROLE.

Existing sets:
- Glacial Aegis (Plate) - Tank role: +STR, +Defend, +Reflect, +Parry
- Steamwork Exosuit (Plate) - MeleeDPS role: +STR, +Weapon Speed
- Shadow Shroud (Leather) - Stealth/Rogue: +DEX, +Stealth, +Hiding

New sets to create (by role):
1. Celestial Raiment (Chain) - Healer role: +INT, +HP Regen, +Heal bonus
2. Stormrider Garb (Leather) - RangedDPS role: +DEX, +Archery, +Attack Chance
3. Arcanist Regalia (Studded) - CasterDPS role: +INT, +Spell Damage, +Mana Regen
4. Harmonist Vestments (Leather) - Support role: +All Stats, +Luck, +Lower Reagent Cost
"""

SETS = [
    {
        "name": "Celestial Raiment",
        "role": "Healer",
        "hue": 1153,  # White/gold holy
        "lore": "Blessed by the Divine Light",
        "stats": {
            "BonusInt": 4,
            "RegenHits": 4,
            "RegenMana": 3,
        },
        "skill": ("Healing", 5.0),
        "pieces": [
            ("Coif", "ChainCoif", 50),
            ("Gorget", "PlateGorget", 52),  # Use plate gorget for chain
            ("Tunic", "ChainChest", 54),
            ("Sleeves", "PlateArms", 51),  # Use plate arms for chain
            ("Gloves", "PlateGloves", 50),  # Use plate gloves
            ("Leggings", "ChainLegs", 53),
        ]
    },
    {
        "name": "Stormrider Garb",
        "role": "RangedDPS",
        "hue": 1165,  # Storm blue/grey
        "lore": "Woven from windswept clouds",
        "stats": {
            "BonusDex": 5,
            "AttackChance": 10,
            "WeaponSpeed": 5,
        },
        "skill": ("Archery", 5.0),
        "pieces": [
            ("Cap", "LeatherCap", 50),
            ("Gorget", "LeatherGorget", 52),
            ("Tunic", "LeatherChest", 54),
            ("Sleeves", "LeatherArms", 51),
            ("Gloves", "LeatherGloves", 50),
            ("Leggings", "LeatherLegs", 53),
        ]
    },
    {
        "name": "Arcanist Regalia",
        "role": "CasterDPS",
        "hue": 1154,  # Arcane blue
        "lore": "Infused with pure arcane essence",
        "stats": {
            "BonusInt": 5,
            "SpellDamage": 10,
            "RegenMana": 3,
        },
        "skill": ("Magery", 5.0),
        "pieces": [
            ("Cap", "LeatherCap", 48),
            ("Gorget", "LeatherGorget", 50),
            ("Tunic", "LeatherChest", 52),
            ("Sleeves", "LeatherArms", 49),
            ("Gloves", "LeatherGloves", 48),
            ("Leggings", "LeatherLegs", 51),
        ]
    },
    {
        "name": "Harmonist Vestments",
        "role": "Support",
        "hue": 2010,  # Green/gold harmonious
        "lore": "Crafted to enhance all who stand near",
        "stats": {
            "BonusStr": 2,
            "BonusDex": 2,
            "BonusInt": 2,
            "Luck": 50,
        },
        "skill": ("Musicianship", 5.0),
        "pieces": [
            ("Cap", "LeatherCap", 48),
            ("Gorget", "LeatherGorget", 50),
            ("Tunic", "LeatherChest", 52),
            ("Sleeves", "LeatherArms", 49),
            ("Gloves", "LeatherGloves", 48),
            ("Leggings", "LeatherLegs", 51),
        ]
    },
]

def generate_piece(set_info, piece_info, piece_num):
    piece_name, base_class, armor_rating = piece_info
    set_name = set_info["name"].replace(" ", "")
    class_name = f"{set_name}{piece_name}"

    # Build stats string
    stats_lines = []
    for attr, val in set_info["stats"].items():
        stats_lines.append(f"            Attributes.{attr} = {val};")
    stats_str = "\n".join(stats_lines)

    skill_name, skill_val = set_info["skill"]

    return f'''    public class {class_name} : {base_class}
    {{
        [Constructable]
        public {class_name}()
        {{
            Name = "{set_info["name"]} {piece_name}";
            Hue = {set_info["hue"]};

            // Legendary stats for {set_info["role"]} role
            SkillBonuses.SetValues(0, SkillName.{skill_name}, {skill_val});

{stats_str}

            BaseArmorRating = {armor_rating};
        }}

        public override void GetProperties(ObjectPropertyList list)
        {{
            base.GetProperties(list);
            list.Add(1060658, "Artifact\\tLegendary");
            list.Add("{set_info["lore"]}");
            list.Add("Set: {set_info["name"]} ({piece_num}/6)");
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
'''

def generate_set(set_info):
    lines = []
    lines.append(f'''    // ============================================================================
    // {set_info["name"].upper()}
    // Role: {set_info["role"]}
    // Hue: {set_info["hue"]}
    // Bonuses: {", ".join(f"+{v} {k}" for k, v in set_info["stats"].items())}
    // ============================================================================
''')

    for i, piece in enumerate(set_info["pieces"], 1):
        lines.append(generate_piece(set_info, piece, i))

    return "\n".join(lines)

def main():
    output = '''using System;
using Server;

namespace Server.Items.Vystia
{
    // ============================================================================
    // ADDITIONAL LEGENDARY ARMOR SETS (Role-Based)
    //
    // Complete Role Coverage:
    // - Tank: Glacial Aegis (existing)
    // - MeleeDPS: Steamwork Exosuit (existing)
    // - Stealth/Rogue: Shadow Shroud (existing)
    // - Healer: Celestial Raiment (NEW)
    // - RangedDPS: Stormrider Garb (NEW)
    // - CasterDPS: Arcanist Regalia (NEW)
    // - Support: Harmonist Vestments (NEW)
    // - Hybrid: Uses appropriate set based on primary role
    // ============================================================================

'''

    for set_info in SETS:
        output += generate_set(set_info)
        output += "\n"

    output += "}\n"

    output_path = r"D:\UO\ServUO\Scripts\Items\Vystia\Equipment\Armor\AdditionalLegendaryArmorSets.cs"
    with open(output_path, 'w', encoding='utf-8') as f:
        f.write(output)

    print(f"Generated {output_path}")
    print(f"Created {len(SETS)} armor sets with 6 pieces each = {len(SETS) * 6} items")
    print("\nRole to Set Mapping:")
    print("  Tank       -> Glacial Aegis (existing)")
    print("  MeleeDPS   -> Steamwork Exosuit (existing)")
    print("  RangedDPS  -> Stormrider Garb (NEW)")
    print("  CasterDPS  -> Arcanist Regalia (NEW)")
    print("  Healer     -> Celestial Raiment (NEW)")
    print("  Support    -> Harmonist Vestments (NEW)")
    print("  Hybrid     -> Steamwork Exosuit (melee default)")

if __name__ == "__main__":
    main()
