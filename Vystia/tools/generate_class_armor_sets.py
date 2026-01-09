#!/usr/bin/env python3
"""
Generate legendary armor sets for each Vystia CLASS.

26 classes = 26 unique armor sets, each with 6 pieces.
Total: 156 armor pieces.

Each class gets armor themed to their playstyle and abilities.
"""

# Class definitions with armor stats
# NOTE: Elemental resists cannot be set via Attributes - use non-resist stats instead
CLASSES = [
    # Magic Classes - Generally leather (light armor for casters)
    {"name": "Ice Mage", "prefix": "IceMage", "hue": 1152, "base": "Leather", "lore": "Crystallized from eternal ice",
     "stats": {"BonusInt": 5, "SpellDamage": 8, "RegenMana": 3}, "skill": "Magery"},
    {"name": "Druid", "prefix": "Druid", "hue": 2010, "base": "Leather", "lore": "Grown from the Living Forest",
     "stats": {"BonusInt": 4, "BonusDex": 2, "RegenHits": 3}, "skill": "AnimalLore"},
    {"name": "Witch", "prefix": "Witch", "hue": 1109, "base": "Leather", "lore": "Woven with dark hexcraft",
     "stats": {"BonusInt": 5, "SpellDamage": 6, "RegenMana": 2}, "skill": "Magery"},
    {"name": "Sorcerer", "prefix": "Sorcerer", "hue": 1161, "base": "Leather", "lore": "Forged in volcanic flames",
     "stats": {"BonusInt": 5, "SpellDamage": 10, "RegenMana": 2}, "skill": "Magery"},
    {"name": "Warlock", "prefix": "Warlock", "hue": 1175, "base": "Leather", "lore": "Infused with void essence",
     "stats": {"BonusInt": 5, "SpellDamage": 8, "RegenMana": 3}, "skill": "Magery"},
    {"name": "Oracle", "prefix": "Oracle", "hue": 1154, "base": "Leather", "lore": "Shimmering with foresight",
     "stats": {"BonusInt": 6, "LowerManaCost": 8, "Luck": 40}, "skill": "Magery"},
    {"name": "Necromancer", "prefix": "Necro", "hue": 1109, "base": "Leather", "lore": "Bound with death magic",
     "stats": {"BonusInt": 5, "SpellDamage": 7, "RegenMana": 2}, "skill": "Necromancy"},
    {"name": "Summoner", "prefix": "Summoner", "hue": 1266, "base": "Leather", "lore": "Blessed by the Deep Ones",
     "stats": {"BonusInt": 5, "BonusMana": 15, "RegenMana": 3}, "skill": "Magery"},
    {"name": "Shaman", "prefix": "Shaman", "hue": 2212, "base": "Leather", "lore": "Blessed by ancestor spirits",
     "stats": {"BonusInt": 4, "BonusStr": 2, "RegenHits": 2}, "skill": "SpiritSpeak"},
    {"name": "Bard", "prefix": "Bard", "hue": 1281, "base": "Leather", "lore": "Resonant with harmonic magic",
     "stats": {"BonusDex": 3, "BonusInt": 3, "Luck": 50}, "skill": "Musicianship"},
    {"name": "Enchanter", "prefix": "Enchanter", "hue": 1155, "base": "Leather", "lore": "Infused with runic power",
     "stats": {"BonusInt": 5, "LowerManaCost": 10, "EnhancePotions": 15}, "skill": "Magery"},
    {"name": "Illusionist", "prefix": "Illusionist", "hue": 1153, "base": "Leather", "lore": "Shifting like desert mirages",
     "stats": {"BonusInt": 4, "BonusDex": 3, "DefendChance": 10}, "skill": "Magery"},

    # Martial Classes - Mix of plate, chain, leather based on class
    {"name": "Barbarian", "prefix": "Barbarian", "hue": 1150, "base": "Plate", "lore": "Forged in Frosthold's fury",
     "stats": {"BonusStr": 6, "AttackChance": 10, "RegenStam": 3}, "skill": "Swords"},
    {"name": "Beastmaster", "prefix": "Beastmaster", "hue": 2301, "base": "Leather", "lore": "Blessed by wild spirits",
     "stats": {"BonusStr": 3, "BonusDex": 3, "RegenHits": 2}, "skill": "AnimalTaming"},
    {"name": "Fighter", "prefix": "Fighter", "hue": 2401, "base": "Plate", "lore": "Tempered in endless combat",
     "stats": {"BonusStr": 5, "BonusDex": 2, "AttackChance": 8}, "skill": "Tactics"},
    {"name": "Monk", "prefix": "Monk", "hue": 2213, "base": "Leather", "lore": "Woven with inner chi",
     "stats": {"BonusDex": 5, "BonusStr": 2, "DefendChance": 12}, "skill": "Wrestling"},
    {"name": "Templar", "prefix": "Templar", "hue": 1153, "base": "Plate", "lore": "Blessed by divine light",
     "stats": {"BonusStr": 4, "BonusInt": 2, "RegenHits": 3}, "skill": "Chivalry"},
    {"name": "Ranger", "prefix": "Ranger", "hue": 2305, "base": "Leather", "lore": "Crafted from desert winds",
     "stats": {"BonusDex": 6, "AttackChance": 10, "WeaponSpeed": 5}, "skill": "Archery"},
    {"name": "Knight", "prefix": "Knight", "hue": 2406, "base": "Plate", "lore": "Forged with honor",
     "stats": {"BonusStr": 4, "DefendChance": 15, "ReflectPhysical": 5}, "skill": "Parry"},
    {"name": "Paladin", "prefix": "Paladin", "hue": 1153, "base": "Plate", "lore": "Sanctified by virtue",
     "stats": {"BonusStr": 3, "BonusInt": 3, "RegenHits": 4}, "skill": "Chivalry"},
    {"name": "Rogue", "prefix": "Rogue", "hue": 1109, "base": "Leather", "lore": "Sewn with shadow threads",
     "stats": {"BonusDex": 6, "AttackChance": 8, "WeaponSpeed": 8}, "skill": "Stealth"},
    {"name": "Bounty Hunter", "prefix": "BountyHunter", "hue": 2118, "base": "Leather", "lore": "Marked with pursuit runes",
     "stats": {"BonusDex": 4, "BonusStr": 2, "AttackChance": 10}, "skill": "Tracking"},
    {"name": "Artificer", "prefix": "Artificer", "hue": 2401, "base": "Plate", "lore": "Powered by steam cores",
     "stats": {"BonusStr": 4, "BonusInt": 2, "WeaponSpeed": 8}, "skill": "Tinkering"},
    {"name": "Alchemist", "prefix": "Alchemist", "hue": 2010, "base": "Leather", "lore": "Infused with reagent essence",
     "stats": {"BonusInt": 4, "BonusDex": 2, "EnhancePotions": 25}, "skill": "Alchemy"},
    {"name": "Cleric", "prefix": "Cleric", "hue": 1153, "base": "Chain", "lore": "Blessed with divine grace",
     "stats": {"BonusInt": 4, "RegenHits": 4, "RegenMana": 2}, "skill": "Healing"},
    {"name": "Wizard", "prefix": "Wizard", "hue": 1154, "base": "Leather", "lore": "Woven with arcane threads",
     "stats": {"BonusInt": 6, "SpellDamage": 8, "LowerManaCost": 5}, "skill": "Magery"},
]

# Base armor types and pieces
ARMOR_BASES = {
    "Plate": [
        ("Helm", "PlateHelm", 50),
        ("Gorget", "PlateGorget", 52),
        ("Chest", "PlateChest", 54),
        ("Arms", "PlateArms", 51),
        ("Gloves", "PlateGloves", 50),
        ("Legs", "PlateLegs", 53),
    ],
    "Leather": [
        ("Cap", "LeatherCap", 48),
        ("Gorget", "LeatherGorget", 50),
        ("Tunic", "LeatherChest", 52),
        ("Sleeves", "LeatherArms", 49),
        ("Gloves", "LeatherGloves", 48),
        ("Leggings", "LeatherLegs", 51),
    ],
    "Chain": [
        ("Coif", "ChainCoif", 49),
        ("Gorget", "PlateGorget", 51),  # Chain doesn't have gorget
        ("Tunic", "ChainChest", 53),
        ("Sleeves", "PlateArms", 50),  # Chain doesn't have arms
        ("Gloves", "PlateGloves", 49),
        ("Leggings", "ChainLegs", 52),
    ],
}

def generate_piece(class_info, piece_info, piece_num):
    piece_name, base_class, armor_rating = piece_info
    class_name = f"{class_info['prefix']}Legendary{piece_name}"
    display_name = f"{class_info['name']} Legendary {piece_name}"

    # Build stats string
    stats_lines = []
    for attr, val in class_info["stats"].items():
        stats_lines.append(f"            Attributes.{attr} = {val};")
    stats_str = "\n".join(stats_lines)

    return f'''    public class {class_name} : {base_class}
    {{
        [Constructable]
        public {class_name}()
        {{
            Name = "{display_name}";
            Hue = {class_info["hue"]};

            // Legendary {class_info["name"]} stats
            SkillBonuses.SetValues(0, SkillName.{class_info["skill"]}, 5.0);

{stats_str}

            BaseArmorRating = {armor_rating};
        }}

        public override void GetProperties(ObjectPropertyList list)
        {{
            base.GetProperties(list);
            list.Add(1060658, "Artifact\\tLegendary");
            list.Add("{class_info["lore"]}");
            list.Add("Set: {class_info["name"]} Legendary ({piece_num}/6)");
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

def generate_class_set(class_info):
    lines = []
    lines.append(f'''    // ============================================================================
    // {class_info["name"].upper()} LEGENDARY SET
    // Base: {class_info["base"]}
    // Hue: {class_info["hue"]}
    // Bonuses: {", ".join(f"+{v} {k}" for k, v in class_info["stats"].items())}
    // ============================================================================
''')

    pieces = ARMOR_BASES[class_info["base"]]
    for i, piece in enumerate(pieces, 1):
        lines.append(generate_piece(class_info, piece, i))

    return "\n".join(lines)

def main():
    output = '''using System;
using Server;

namespace Server.Items.Vystia
{
    // ============================================================================
    // CLASS-SPECIFIC LEGENDARY ARMOR SETS
    //
    // Each of the 26 Vystia classes has its own unique legendary armor set.
    // Sets are themed to match class abilities and playstyle.
    //
    // Total: 26 classes x 6 pieces = 156 items
    // ============================================================================

'''

    for class_info in CLASSES:
        output += generate_class_set(class_info)
        output += "\n"

    output += "}\n"

    output_path = r"D:\UO\ServUO\Scripts\Items\Vystia\Equipment\Armor\ClassLegendaryArmorSets.cs"
    with open(output_path, 'w', encoding='utf-8') as f:
        f.write(output)

    print(f"Generated {output_path}")
    print(f"Created {len(CLASSES)} class armor sets with 6 pieces each = {len(CLASSES) * 6} items")

if __name__ == "__main__":
    main()
