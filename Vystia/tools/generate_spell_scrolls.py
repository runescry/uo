#!/usr/bin/env python3
"""
Generate Spell Scroll classes for all 384 Vystia spells
Each scroll casts its corresponding spell when used
"""

import os
from pathlib import Path

# Spell school configuration
SPELL_SCHOOLS = {
    "IceMage": {
        "class": "IceMage",
        "namespace": "Server.Custom.VystiaClasses.Spells.IceMage",
        "hue": "0x481",  # Ice blue
        "spell_ids": range(1000, 1032),  # 32 spells
        "circles": 8
    },
    "Druid": {
        "class": "Druid",
        "namespace": "Server.Custom.VystiaClasses.Spells.Druid",
        "hue": "0x7D6",  # Nature green
        "spell_ids": range(1032, 1064),
        "circles": 8
    },
    "Witch": {
        "class": "Witch",
        "namespace": "Server.Custom.VystiaClasses.Spells.Witch",
        "hue": "0x81D",  # Hex purple
        "spell_ids": range(1064, 1096),
        "circles": 8
    },
    "Sorcerer": {
        "class": "Sorcerer",
        "namespace": "Server.Custom.VystiaClasses.Spells.Sorcerer",
        "hue": "0x54E",  # Elemental orange
        "spell_ids": range(1096, 1128),
        "circles": 8
    },
    "Warlock": {
        "class": "Warlock",
        "namespace": "Server.Custom.VystiaClasses.Spells.Warlock",
        "hue": "0x455",  # Dark purple
        "spell_ids": range(1128, 1160),
        "circles": 8
    },
    "Oracle": {
        "class": "Oracle",
        "namespace": "Server.Custom.VystiaClasses.Spells.Oracle",
        "hue": "0x482",  # Divination cyan
        "spell_ids": range(1160, 1192),
        "circles": 8
    },
    "Necromancer": {
        "class": "Necromancer",
        "namespace": "Server.Custom.VystiaClasses.Spells.Necromancer",
        "hue": "0x455",  # Necromancy dark
        "spell_ids": range(1192, 1224),
        "circles": 8
    },
    "Summoner": {
        "class": "Summoner",
        "namespace": "Server.Custom.VystiaClasses.Spells.Summoner",
        "hue": "0x555",  # Summoning teal
        "spell_ids": range(1224, 1256),
        "circles": 8
    },
    "Shaman": {
        "class": "Shaman",
        "namespace": "Server.Custom.VystiaClasses.Spells.Shaman",
        "hue": "0x501",  # Shamanic yellow
        "spell_ids": range(1256, 1288),
        "circles": 8
    },
    "Bard": {
        "class": "Bard",
        "namespace": "Server.Custom.VystiaClasses.Spells.Bard",
        "hue": "0x8A5",  # Bardic gold
        "spell_ids": range(1288, 1320),
        "circles": 8
    },
    "Enchanter": {
        "class": "Enchanter",
        "namespace": "Server.Custom.VystiaClasses.Spells.Enchanter",
        "hue": "0x8FD",  # Enchanting bright blue
        "spell_ids": range(1320, 1352),
        "circles": 8
    },
    "Illusionist": {
        "class": "Illusionist",
        "namespace": "Server.Custom.VystiaClasses.Spells.Illusionist",
        "hue": "0x47E",  # Illusion pink
        "spell_ids": range(1352, 1384),
        "circles": 8
    }
}

SCROLL_TEMPLATE = """using System;
using Server;
using Server.Items;

namespace {namespace}
{{
    /// <summary>
    /// Spell scrolls for {school_name} magic
    /// </summary>

{scroll_classes}
}}
"""

SCROLL_CLASS_TEMPLATE = """    public class {spell_name}Scroll : SpellScroll
    {{
        [Constructable]
        public {spell_name}Scroll() : this(1)
        {{
        }}

        [Constructable]
        public {spell_name}Scroll(int amount) : base({spell_id}, 0x1F2D, amount)
        {{
            Hue = {hue};
        }}

        public {spell_name}Scroll(Serial serial) : base(serial)
        {{
        }}

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
"""

def get_spell_names_from_directory(school_dir):
    """Extract spell class names from spell files"""
    spells = []

    if not school_dir.exists():
        print(f"[WARN] Directory not found: {school_dir}")
        return spells

    # Look for Circle1.cs, Circle2.cs, etc.
    for circle in range(1, 9):
        circle_file = school_dir / f"Circle{circle}.cs"
        if circle_file.exists():
            with open(circle_file, 'r', encoding='utf-8') as f:
                content = f.read()
                # Find class declarations: public class SpellName : BaseVystiaSpell
                import re
                pattern = r'public class (\w+)\s*:\s*BaseVystiaSpell'
                matches = re.findall(pattern, content)
                spells.extend(matches)

    return spells

def generate_scroll_file(school_key, school_data, output_dir):
    """Generate scroll file for a magic school"""
    school_name = school_data['class']
    hue = school_data['hue']
    spell_ids = list(school_data['spell_ids'])

    # Try to get actual spell names from spell files
    spell_dir = Path(f"C:/DevEnv/GIT/UO/ServUO/Scripts/Custom/VystiaClasses/Spells/{school_name}")
    spell_names = get_spell_names_from_directory(spell_dir)

    # If we couldn't get spell names, generate generic names
    if not spell_names or len(spell_names) != 32:
        print(f"[INFO] Generating generic spell names for {school_name} ({len(spell_names)} found)")
        spell_names = []
        for circle in range(1, 9):
            for spell_num in range(1, 5):
                spell_names.append(f"{school_name}Spell{circle}_{spell_num}")

    print(f"[INFO] Generating {len(spell_names)} scrolls for {school_name} magic...")

    # Generate scroll classes
    scroll_classes = []
    for i, (spell_name, spell_id) in enumerate(zip(spell_names, spell_ids)):
        scroll_class = SCROLL_CLASS_TEMPLATE.format(
            spell_name=spell_name,
            spell_id=spell_id,
            hue=hue
        )
        scroll_classes.append(scroll_class)

    # Generate file content
    file_content = SCROLL_TEMPLATE.format(
        namespace=f"Server.Items.VystiaScrolls",
        school_name=school_name,
        scroll_classes="\n".join(scroll_classes)
    )

    # Write file
    output_file = output_dir / f"{school_name}Scrolls.cs"
    with open(output_file, 'w', encoding='utf-8') as f:
        f.write(file_content)

    print(f"[SUCCESS] Created {output_file.name} ({len(spell_names)} scrolls)")
    return len(spell_names)

def generate_all_scrolls():
    """Generate scroll files for all spell schools"""
    output_dir = Path("C:/DevEnv/GIT/UO/ServUO/Scripts/Items/Vystia/Scrolls")
    output_dir.mkdir(parents=True, exist_ok=True)

    print("="*80)
    print("GENERATING VYSTIA SPELL SCROLLS")
    print("="*80)
    print()

    total_scrolls = 0

    for school_key, school_data in SPELL_SCHOOLS.items():
        count = generate_scroll_file(school_key, school_data, output_dir)
        total_scrolls += count

    print()
    print("="*80)
    print(f"COMPLETE: Generated {total_scrolls} spell scroll classes in {len(SPELL_SCHOOLS)} files")
    print(f"Location: {output_dir}")
    print("="*80)

    return total_scrolls

if __name__ == "__main__":
    total = generate_all_scrolls()
    print(f"\n[SUCCESS] {total} spell scrolls created!")
