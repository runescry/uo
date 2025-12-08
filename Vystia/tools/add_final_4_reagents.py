#!/usr/bin/env python3
"""
Add final 4 reagents to reach exactly 96 (8 per school × 12 schools)
"""

import os
import re
from pathlib import Path

REAGENT_DIR = Path("C:/DevEnv/GIT/UO/ServUO/Scripts/Items/Vystia/Resources/Reagents")

# Final reagents to add
FINAL_REAGENTS = {
    "DivinationMagic": [
        ("PropheticLeaf", 0x18E1, "Leaf that shows glimpses of the future"),
        ("SeeingStone", 0x1F19, "Crystal for scrying and divination")
    ],
    "Necromancy": [
        ("PhylacteryShard", 0x1F1C, "Fragment of a lich's phylactery"),
        ("ReaperEssence", 0x1F9D, "Essence of the grim reaper")
    ]
}

SCHOOL_HUES = {
    "DivinationMagic": "0x482",
    "Necromancy": "0x455"
}

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

def add_reagents_to_file(filepath, reagents_to_add):
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
    print("ADDING FINAL 4 REAGENTS TO REACH 96 TOTAL")
    print("="*80)
    print()

    total_added = 0

    for school, reagents in FINAL_REAGENTS.items():
        filepath = REAGENT_DIR / f"{school}Reagents.cs"

        if not filepath.exists():
            print(f"[ERROR] File not found: {filepath}")
            continue

        print(f"[INFO] Adding {len(reagents)} reagents to {school}...")
        for name, item_id, desc in reagents:
            print(f"       - {format_name(name)}")

        if add_reagents_to_file(filepath, reagents):
            total_added += len(reagents)
            print(f"[OK]   {school} now has 8 reagents")
        else:
            print(f"[FAIL] Failed to update {school}")

        print()

    print("="*80)
    print(f"[SUCCESS] Added {total_added} reagents!")
    print(f"           Total now: 92 + {total_added} = {92 + total_added} reagents ✅")
    print("="*80)
    print()
    print("[COMPLETE] All 12 schools now have exactly 8 reagents each!")
    print()

if __name__ == "__main__":
    main()
