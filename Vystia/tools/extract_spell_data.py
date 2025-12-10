"""
Extract spell data from C# spell files and output to CSV
Usage: python extract_spell_data.py
"""

import os
import re
import csv

# Map school directory names to display names
SCHOOL_NAMES = {
    "IceMage": "Ice Magic",
    "Druid": "Nature Magic",
    "Witch": "Hex Magic",
    "Sorcerer": "Elemental Magic",
    "Warlock": "Dark Magic",
    "Oracle": "Divination",
    "Necromancer": "Necromancy",
    "Summoner": "Summoning",
    "Shaman": "Shamanic",
    "Bard": "Bardic",
    "Enchanter": "Enchanting",
    "Illusionist": "Illusion"
}

def extract_spell_name(class_name):
    """Convert CamelCase spell class name to readable name"""
    # Remove 'Spell' suffix
    name = class_name.replace("Spell", "")

    # Remove school prefixes (Nature, Elemental, Ice, etc.)
    for prefix in ["Nature", "Elemental", "Ice", "Hex", "Dark", "Divination",
                   "Necro", "Summon", "Shaman", "Bardic", "Enchant", "Illusion"]:
        if name.startswith(prefix):
            name = name[len(prefix):]
            break

    # Split on capital letters
    words = re.sub('([A-Z][a-z]+)', r' \1', re.sub('([A-Z]+)', r' \1', name)).split()
    return ' '.join(words).strip()

def determine_spell_type(comment_text, todo_text):
    """Determine spell type from comments and TODO"""
    text = (comment_text + " " + todo_text).lower()

    # Check for spell types in order of priority
    if any(word in text for word in ["shapeshift", "transform", "form"]):
        return "shapeshift"
    elif any(word in text for word in ["summon", "conjure", "animate"]):
        return "summon"
    elif any(word in text for word in ["heal", "restore", "regenerate", "mend"]):
        return "heal"
    elif any(word in text for word in ["cure", "remove", "cleanse"]):
        return "cure"
    elif any(word in text for word in ["paralyze", "stun", "freeze", "immobilize", "root"]):
        return "paralyze"
    elif any(word in text for word in ["slow", "-dex", "reduce dex"]):
        return "slow"
    elif any(word in text for word in ["damage over time", "dot", "tick", "bleeding", "burn"]):
        return "dot"
    elif any(word in text for word in ["aoe", "area", "radius"]):
        return "aoe_damage"
    elif any(word in text for word in ["damage", "deals", "hp damage", "cold damage", "fire damage"]):
        return "damage"
    elif any(word in text for word in ["buff", "+str", "+dex", "+int", "increase", "resist", "armor"]):
        return "buff"
    elif any(word in text for word in ["debuff", "curse", "weaken", "-str", "-dex", "-int"]):
        return "debuff"
    elif any(word in text for word in ["teleport", "blink", "recall"]):
        return "teleport"
    elif any(word in text for word in ["detect", "reveal", "sense", "locate"]):
        return "detection"
    elif any(word in text for word in ["illusion", "invisibility", "stealth", "hide"]):
        return "illusion"
    elif any(word in text for word in ["charm", "control", "dominate"]):
        return "charm"
    elif any(word in text for word in ["resurrect", "revive"]):
        return "resurrect"
    else:
        return "utility"

def parse_spell_file(file_path, school_name):
    """Parse a single C# spell file"""
    try:
        with open(file_path, 'r', encoding='utf-8') as f:
            content = f.read()

        # Extract class name
        class_match = re.search(r'public class (\w+Spell) : MagerySpell', content)
        if not class_match:
            return None

        class_name = class_match.group(1)
        spell_name = extract_spell_name(class_name)

        # Extract comment block (/// <summary>)
        comment_match = re.search(r'/// <summary>(.*?)/// </summary>', content, re.DOTALL)
        comment_text = comment_match.group(1) if comment_match else ""

        # Extract TODO comment
        todo_match = re.search(r'// TODO: Implement (.+?)effect', content)
        todo_text = todo_match.group(1) if todo_match else ""

        # Extract circle and mana from comment (handle both "3rd" and "3" formats)
        circle_match = re.search(r'Circle:\s*(\d+)(?:st|nd|rd|th)?\s*\((\d+) mana\)', comment_text)
        if circle_match:
            circle = circle_match.group(1)
            mana = circle_match.group(2)
        else:
            # Try alternate format without ordinal suffix
            circle_match2 = re.search(r'Circle:\s*(\d+)[^\d]*\((\d+) mana\)', comment_text)
            if circle_match2:
                circle = circle_match2.group(1)
                mana = circle_match2.group(2)
            else:
                circle = "?"
                mana = "?"

        # Build description from comment and TODO
        description_parts = []

        # Get all description lines from comment (skip Circle: and Reagents: lines)
        comment_lines = [line.strip().replace("///", "").strip()
                        for line in comment_text.split('\n')
                        if line.strip()
                        and not line.strip().startswith("Circle:")
                        and not line.strip().startswith("Reagents:")]

        # Join all description lines
        if comment_lines:
            description_parts.append(" ".join(comment_lines))

        # Add TODO text if available and different from comment
        if todo_text and todo_text.strip() not in " ".join(comment_lines):
            description_parts.append("Effect: " + todo_text.strip())

        description = " | ".join(description_parts) if description_parts else "No description"

        # Determine type
        spell_type = determine_spell_type(comment_text, todo_text)

        return {
            "school": school_name,
            "spell_name": spell_name,
            "type": spell_type,
            "circle": circle,
            "mana": mana,
            "description": description,
            "class_name": class_name
        }

    except Exception as e:
        print(f"Error parsing {file_path}: {e}")
        return None

def main():
    base_path = r"C:\DevEnv\GIT\UO\ServUO\Scripts\Custom\VystiaClasses\Spells"

    spells = []

    print("Extracting spell data from C# files...\n")

    # Process each school
    for school_dir, school_name in SCHOOL_NAMES.items():
        school_path = os.path.join(base_path, school_dir)

        if not os.path.exists(school_path):
            print(f"[SKIP] {school_name}: Directory not found")
            continue

        # Get all .cs files
        spell_files = [f for f in os.listdir(school_path) if f.endswith(".cs")]

        print(f"Processing {school_name}: {len(spell_files)} files")

        for spell_file in spell_files:
            file_path = os.path.join(school_path, spell_file)
            spell_data = parse_spell_file(file_path, school_name)

            if spell_data:
                spells.append(spell_data)

    # Sort by school and spell name
    spells.sort(key=lambda x: (x["school"], x["spell_name"]))

    # Write to CSV
    output_file = "vystia_spells_complete.csv"
    with open(output_file, 'w', newline='', encoding='utf-8') as f:
        writer = csv.DictWriter(f, fieldnames=["school", "spell_name", "type", "circle", "mana", "description", "class_name"])
        writer.writeheader()
        writer.writerows(spells)

    print(f"\n{'='*70}")
    print(f"Total spells extracted: {len(spells)}")
    print(f"Output saved to: {output_file}")
    print(f"{'='*70}\n")

    # Print summary by school
    print("Spell count by school:")
    print("-" * 70)

    school_counts = {}
    for spell in spells:
        school = spell["school"]
        school_counts[school] = school_counts.get(school, 0) + 1

    for school in sorted(school_counts.keys()):
        print(f"  {school:<20} {school_counts[school]:>3} spells")

    # Print type distribution
    print("\nSpell type distribution:")
    print("-" * 70)

    type_counts = {}
    for spell in spells:
        spell_type = spell["type"]
        type_counts[spell_type] = type_counts.get(spell_type, 0) + 1

    for spell_type in sorted(type_counts.keys(), key=lambda x: type_counts[x], reverse=True):
        print(f"  {spell_type:<20} {type_counts[spell_type]:>3} spells")

if __name__ == "__main__":
    main()
