#!/usr/bin/env python3
"""
Add missing reagents to reach exactly 8 per school (96 total)
Keeps existing reagents, adds new ones where needed
"""

import os
import re
from pathlib import Path

REAGENT_DIR = Path("C:/DevEnv/GIT/UO/ServUO/Scripts/Items/Vystia/Resources/Reagents")

# Schools that need additional reagents
REAGENTS_TO_ADD = {
    "IceMagic": [
        ("EternalIce", 0x1F19, "Essence of eternal winter, never melting")
    ],
    "NatureMagic": [
        ("LivingBark", 0x1BD7, "Bark from a living treant"),
        ("TreantHeart", 0x1F1C, "Heart of an ancient treant")
    ],
    "HexMagic": [
        ("SwampLotus", 0x18E9, "Rare lotus from toxic swamps"),
        ("CursedSalt", 0x11EA, "Salt cursed by dark rituals")
    ],
    "ElementalMagic": [
        ("DragonHeart", 0x1F13, "Heart of a fire dragon"),
        ("PhoenixFeather", 0x1CFF, "Feather of a phoenix")
    ],
    "SummoningMagic": [
        ("BindingRune", 0x1F14, "Rune for binding summoned creatures"),
        ("DimensionalKey", 0x1F47, "Key to other dimensions"),
        ("SummoningSalt", 0x11EA, "Enchanted salt for summoning circles")
    ]
}

# Hues by school
SCHOOL_HUES = {
    "IceMagic": "0x481",
    "NatureMagic": "0x7D6",
    "HexMagic": "0x81D",
    "ElementalMagic": "0x54E",
    "SummoningMagic": "0x555"
}

def read_existing_reagents(filepath):
    """Read existing reagent class names from file"""
    reagents = []
    try:
        with open(filepath, 'r', encoding='utf-8') as f:
            content = f.read()
            pattern = r'public class (\w+)\s*:\s*BaseVystiaReagent'
            reagents = re.findall(pattern, content)
    except Exception as e:
        print(f"[ERROR] Reading {filepath}: {e}")
    return reagents

def generate_reagent_class(name, item_id, description):
    """Generate a reagent class"""
    return f"""    public class {name} : BaseVystiaReagent
    {{
        [Constructable]
        public {name}() : this(1)
        {{
        }}

        [Constructable]
        public {name}(int amount) : base(amount)
        {{
            ItemID = {item_id};
            Name = "{format_name(name)}";
        }}

        public override string DefaultName => "{format_name(name)}";
        public override string Description => "{description}";

        public {name}(Serial serial) : base(serial)
        {{
        }}

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

def format_name(class_name):
    """Convert CamelCase to Title Case with spaces"""
    result = re.sub(r'([a-z])([A-Z])', r'\1 \2', class_name)
    return result

def add_reagents_to_file(filepath, reagents_to_add, school_hue):
    """Add new reagents to existing file"""

    # Read existing file
    with open(filepath, 'r', encoding='utf-8') as f:
        content = f.read()

    # Find the namespace closing brace (last line)
    lines = content.split('\n')

    # Find insertion point (before the closing brace of namespace)
    insert_index = -1
    for i in range(len(lines) - 1, -1, -1):
        if lines[i].strip() == '}':
            insert_index = i
            break

    if insert_index == -1:
        print(f"[ERROR] Could not find namespace closing brace in {filepath}")
        return False

    # Generate new reagent classes
    new_classes = []
    for name, item_id, desc in reagents_to_add:
        new_classes.append(generate_reagent_class(name, hex(item_id), desc))

    # Insert before the closing brace
    lines.insert(insert_index, '\n'.join(new_classes))

    # Write updated file
    new_content = '\n'.join(lines)
    with open(filepath, 'w', encoding='utf-8') as f:
        f.write(new_content)

    return True

def main():
    print("="*80)
    print("ADDING REAGENTS TO REACH 96 TOTAL (8 per school)")
    print("="*80)
    print()

    total_added = 0

    for school, reagents in REAGENTS_TO_ADD.items():
        filepath = REAGENT_DIR / f"{school}Reagents.cs"

        if not filepath.exists():
            print(f"[ERROR] File not found: {filepath}")
            continue

        # Read existing reagents
        existing = read_existing_reagents(filepath)
        print(f"[INFO] {school}: {len(existing)} existing reagents")

        # Add new reagents
        print(f"[INFO] Adding {len(reagents)} new reagents to {school}...")
        for name, item_id, desc in reagents:
            print(f"       - {format_name(name)}")

        if add_reagents_to_file(filepath, reagents, SCHOOL_HUES.get(school, "0x0")):
            total_added += len(reagents)
            print(f"[OK]   {school} now has {len(existing) + len(reagents)} reagents")
        else:
            print(f"[FAIL] Failed to update {school}")

        print()

    print("="*80)
    print(f"[SUCCESS] Added {total_added} reagents!")
    print(f"           Total should now be: 82 + {total_added} = {82 + total_added} reagents")
    print("="*80)
    print()
    print("[NEXT STEPS]:")
    print("1. Verify all files compile")
    print("2. Update spell reagent requirements")
    print("3. Regenerate vendors")
    print()

if __name__ == "__main__":
    main()
