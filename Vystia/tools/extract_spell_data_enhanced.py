"""
Extract spell data from both C# files AND design documents
Combines implementation details with design descriptions
Usage: python extract_spell_data_enhanced.py
"""

import os
import re
import csv

# Map school directory names to display names and MD files
SCHOOL_INFO = {
    "IceMage": {"name": "Ice Magic", "md_file": "IceMagic.md"},
    "Druid": {"name": "Nature Magic", "md_file": "NatureMagic.md"},
    "Witch": {"name": "Hex Magic", "md_file": "HexMagic.md"},
    "Sorcerer": {"name": "Elemental Magic", "md_file": "ElementalMagic.md"},
    "Warlock": {"name": "Dark Magic", "md_file": "DarkMagic.md"},
    "Oracle": {"name": "Divination", "md_file": "DivinationMagic.md"},
    "Necromancer": {"name": "Necromancy", "md_file": "Necromancy.md"},
    "Summoner": {"name": "Summoning", "md_file": "SummoningMagic.md"},
    "Shaman": {"name": "Shamanic", "md_file": "ShamanicMagic.md"},
    "Bard": {"name": "Bardic", "md_file": "BardicMagic.md"},
    "Enchanter": {"name": "Enchanting", "md_file": "EnchantingMagic.md"},
    "Illusionist": {"name": "Illusion", "md_file": "IllusionMagic.md"}
}

def normalize_spell_name(name):
    """Normalize spell name for matching"""
    # Remove special characters and convert to lowercase
    name = name.lower()
    name = re.sub(r'[^\w\s]', '', name)
    name = re.sub(r'\s+', ' ', name).strip()
    return name

def load_design_descriptions(md_file_path):
    """Load spell descriptions from design document"""
    descriptions = {}

    try:
        with open(md_file_path, 'r', encoding='utf-8') as f:
            content = f.read()

        # Method 1: Simple format "N. **Spell Name** - Description"
        pattern1 = r'(?:####)?\s*\d+\.\s*\*\*([^*]+)\*\*\s*-\s*([^\n]+)'
        matches1 = re.finditer(pattern1, content)

        for match in matches1:
            spell_name = match.group(1).strip()
            description = match.group(2).strip()

            normalized = normalize_spell_name(spell_name)
            descriptions[normalized] = {
                "original_name": spell_name,
                "description": description,
                "gameplay": ""
            }

        # Method 2: Detailed format "#### N. Spell Name" with bullet points
        # Split into spell sections
        spell_sections = re.split(r'####\s*\d+\.', content)[1:]  # Skip header
        spell_headers = re.findall(r'####\s*\d+\.\s*([^\n]+)', content)

        for header, section in zip(spell_headers, spell_sections):
            # Remove checkmark and extra whitespace
            spell_name = header.replace('✅', '').replace('⚠️', '').strip()

            # Skip if already found by Method 1
            normalized = normalize_spell_name(spell_name)
            if normalized in descriptions:
                continue

            # Extract description from bullet points
            desc_parts = []

            # Extract damage
            damage_match = re.search(r'\*\*Damage:\*\*\s*([^\n]+)', section)
            if damage_match:
                desc_parts.append(damage_match.group(1).strip())

            # Extract special
            special_match = re.search(r'\*\*Special:\*\*\s*([^\n]+)', section)
            if special_match:
                desc_parts.append(special_match.group(1).strip())

            # Extract effect (alternative to special)
            effect_match = re.search(r'\*\*Effect:\*\*\s*([^\n]+)', section)
            if effect_match:
                desc_parts.append(effect_match.group(1).strip())

            # Extract healing
            heal_match = re.search(r'\*\*Healing:\*\*\s*([^\n]+)', section)
            if heal_match:
                desc_parts.append(heal_match.group(1).strip())

            # Extract buff/debuff
            buff_match = re.search(r'\*\*Buff:\*\*\s*([^\n]+)', section)
            if buff_match:
                desc_parts.append(buff_match.group(1).strip())

            # Extract radius/area
            area_match = re.search(r'\*\*(?:Area|Radius):\*\*\s*([^\n]+)', section)
            if area_match:
                desc_parts.append(f"Area: {area_match.group(1).strip()}")

            # Extract duration
            duration_match = re.search(r'\*\*Duration:\*\*\s*([^\n]+)', section)
            if duration_match:
                desc_parts.append(f"Duration: {duration_match.group(1).strip()}")

            # Extract gameplay mechanic from "How It Works" section
            gameplay = ""
            how_it_works = re.search(r'\*\*How It Works:\*\*\s*\n((?:- [^\n]+\n?)+)', section)
            if how_it_works:
                # Extract bullet points and combine into gameplay description
                bullets = re.findall(r'- ([^\n]+)', how_it_works.group(1))
                if bullets:
                    gameplay = " ".join(bullets[:2])  # Take first 2 bullets for conciseness

            if desc_parts:
                description = " | ".join(desc_parts)
                descriptions[normalized] = {
                    "original_name": spell_name,
                    "description": description,
                    "gameplay": gameplay
                }
            elif gameplay:
                # If no stats but has gameplay, still include it
                descriptions[normalized] = {
                    "original_name": spell_name,
                    "description": "",
                    "gameplay": gameplay
                }

        return descriptions

    except Exception as e:
        print(f"Warning: Could not load {md_file_path}: {e}")
        return {}

def extract_spell_name_from_class(class_name):
    """Convert CamelCase spell class name to readable name"""
    # Remove 'Spell' suffix
    name = class_name.replace("Spell", "")

    # Remove school prefixes (try longest matches first to avoid partial matches)
    prefixes = ["Summoning", "Shamanic", "Elemental", "Divination", "Enchanting",
                "Illusion", "Nature", "Bardic", "Necro", "Ice", "Hex", "Dark"]
    for prefix in prefixes:
        if name.startswith(prefix):
            name = name[len(prefix):]
            break

    # Handle common lowercase connectors
    name = re.sub(r'([a-z])(of|the|a|to|in|on|at|by|for)([A-Z])', r'\1 \2 \3', name)

    # Split on capital letters
    words = re.sub('([A-Z][a-z]+)', r' \1', re.sub('([A-Z]+)', r' \1', name)).split()
    return ' '.join(words).strip()

def extract_damage(text):
    """Extract damage range and type from text"""
    # Look for damage patterns like "4-8 cold damage", "30-50 fire damage", etc.
    damage_match = re.search(r'(\d+)-(\d+)\s*(?:HP\s*)?(?:(cold|fire|lightning|energy|poison|physical|sonic|nature|dark|void|ice)\s*)?damage', text, re.IGNORECASE)
    if damage_match:
        damage_range = f"{damage_match.group(1)}-{damage_match.group(2)}"
        damage_type = damage_match.group(3).lower() if damage_match.group(3) else ""
        return damage_range, damage_type
    return "", ""

def extract_healing(text):
    """Extract healing amount from text"""
    heal_match = re.search(r'(?:Heal[s]?|Restore[s]?)\s*(\d+)-(\d+)\s*HP', text, re.IGNORECASE)
    if heal_match:
        return f"{heal_match.group(1)}-{heal_match.group(2)}"

    # Also check for HP/tick patterns
    heal_tick_match = re.search(r'(\d+)-(\d+)\s*HP/tick', text, re.IGNORECASE)
    if heal_tick_match:
        return f"{heal_tick_match.group(1)}-{heal_tick_match.group(2)}/tick"

    return ""

def extract_duration(text):
    """Extract duration from text"""
    duration_match = re.search(r'(?:Duration:|for|lasts)\s*(\d+(?:\.\d+)?)\s*(?:s|seconds)', text, re.IGNORECASE)
    if duration_match:
        return f"{duration_match.group(1)}s"
    return ""

def extract_area(text):
    """Extract area/radius from text"""
    # Look for tile radius/area
    area_match = re.search(r'(\d+)\s*tile\s*(radius|area|cone|range)', text, re.IGNORECASE)
    if area_match:
        return f"{area_match.group(1)} tile {area_match.group(2)}"

    # Look for general AoE
    if re.search(r'\bAoE\b', text, re.IGNORECASE):
        return "AoE"

    return ""

def extract_cooldown(text):
    """Extract cooldown from text"""
    cooldown_match = re.search(r'Cooldown:\s*(\d+(?:\.\d+)?)\s*(?:s|seconds)', text, re.IGNORECASE)
    if cooldown_match:
        return f"{cooldown_match.group(1)}s"
    return ""

def extract_effects(text, spell_type):
    """Extract special effects from text"""
    effects = []

    # Stat modifiers
    stat_patterns = [
        (r'([+-]\d+)\s*(STR|DEX|INT)', lambda m: f"{m.group(1)} {m.group(2)}"),
        (r'([+-]\d+%?)\s*(all stats|damage|attack speed|movement speed|accuracy)', lambda m: f"{m.group(1)} {m.group(2)}"),
        (r'([+-]\d+)\s*(Physical|Cold|Fire|Poison|Energy)\s*Resist', lambda m: f"{m.group(1)} {m.group(2)} Resist"),
    ]

    for pattern, formatter in stat_patterns:
        matches = re.finditer(pattern, text, re.IGNORECASE)
        for match in matches:
            effect = formatter(match)
            if effect not in effects:
                effects.append(effect)

    # Special status effects
    status_effects = [
        'slow', 'stun', 'freeze', 'root', 'paralyze', 'fear', 'charm', 'confusion',
        'silence', 'blind', 'poison', 'bleed', 'burn', 'knockback', 'vulnerability'
    ]

    for effect_name in status_effects:
        if re.search(rf'\b{effect_name}\b', text, re.IGNORECASE):
            if effect_name.capitalize() not in effects:
                effects.append(effect_name.capitalize())

    # Channeled spells
    if re.search(r'\bCHANNELED\b', text, re.IGNORECASE):
        effects.insert(0, "CHANNELED")

    # Instant cast
    if re.search(r'\binstant cast\b', text, re.IGNORECASE):
        effects.insert(0, "Instant Cast")

    return effects[:3]  # Return max 3 effects

def generate_gameplay_from_description(spell_type, spell_name, description, design_desc):
    """Generate gameplay description based on spell type and available info"""
    text = description + " " + design_desc

    gameplay_templates = {
        "damage": "Target enemy and cast to deal {} damage.",
        "dot": "Apply to enemy. Target takes damage over time for the duration.",
        "aoe_damage": "Cast at location. All enemies in area take damage.",
        "heal": "Target friendly player or self. Restores health.",
        "buff": "Cast on friendly target or self. Enhances stats or resistances for the duration.",
        "debuff": "Target enemy. Reduces their stats or resistances for the duration.",
        "paralyze": "Target enemy. Subject is rooted to the ground and cannot move for the duration.",
        "slow": "Target enemy. Subject's movement and attack speed are reduced for the duration.",
        "summon": "Cast to summon creature. Summoned creature fights for you.",
        "shapeshift": "Cast on self. Caster transforms into different form with new abilities.",
        "cure": "Target friendly player. Removes negative status effects.",
        "resurrect": "Target dead player. Brings them back to life.",
        "teleport": "Cast to instantly move to target location.",
        "detection": "Cast to reveal hidden enemies or objects in area.",
        "illusion": "Cast on self or target. Creates illusion or grants invisibility.",
        "charm": "Target enemy creature. Subject fights for you temporarily.",
        "utility": "Cast to activate spell effect."
    }

    # Get base template
    base_gameplay = gameplay_templates.get(spell_type, "Cast to activate spell effect.")

    # Try to extract damage range for damage spells
    if spell_type in ["damage", "aoe_damage", "dot"]:
        damage_match = re.search(r'(\d+-\d+)\s*(?:cold|fire|lightning|energy|poison|physical|sonic|damage)', text.lower())
        if damage_match:
            base_gameplay = base_gameplay.format(damage_match.group(1))
        else:
            base_gameplay = base_gameplay.replace("{}", "")

    # Extract duration if available
    duration_match = re.search(r'(?:Duration:|for)\s*(\d+(?:\.\d+)?)\s*(?:s|seconds)', text, re.IGNORECASE)
    if duration_match and "duration" in base_gameplay:
        duration = duration_match.group(1)
        base_gameplay = base_gameplay.replace("for the duration", f"for {duration} seconds")

    # Extract area/radius
    area_match = re.search(r'(\d+)\s*tile\s*(?:radius|area)', text.lower())
    if area_match and "area" in base_gameplay:
        area = area_match.group(1)
        base_gameplay = base_gameplay.replace("in area", f"in {area} tile radius")

    return base_gameplay

def determine_spell_type(comment_text, todo_text, design_desc):
    """Determine spell type from all available info"""
    text = (comment_text + " " + todo_text + " " + design_desc).lower()

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
    elif any(word in text for word in ["dot", "tick", "bleeding", "burn", "damage/tick", "fire/tick"]):
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

def parse_spell_file(file_path, school_name, design_descriptions):
    """Parse a single C# spell file and merge with design description"""
    try:
        with open(file_path, 'r', encoding='utf-8') as f:
            content = f.read()

        # Extract class name
        class_match = re.search(r'public class (\w+Spell) : MagerySpell', content)
        if not class_match:
            return None

        class_name = class_match.group(1)
        spell_name = extract_spell_name_from_class(class_name)

        # Extract comment block
        comment_match = re.search(r'/// <summary>(.*?)/// </summary>', content, re.DOTALL)
        comment_text = comment_match.group(1) if comment_match else ""

        # Extract TODO comment
        todo_match = re.search(r'// TODO: Implement (.+?)effect', content)
        todo_text = todo_match.group(1) if todo_match else ""

        # Extract circle and mana
        circle_match = re.search(r'Circle:\s*(\d+)(?:st|nd|rd|th)?\s*\((\d+) mana\)', comment_text)
        if circle_match:
            circle = circle_match.group(1)
            mana = circle_match.group(2)
        else:
            circle = "?"
            mana = "?"

        # Try to find design description
        normalized_name = normalize_spell_name(spell_name)
        design_info = design_descriptions.get(normalized_name, {})
        design_desc = design_info.get("description", "")
        design_gameplay = design_info.get("gameplay", "")

        # Build comprehensive description
        description_parts = []

        # Get all description lines from C# comment
        comment_lines = [line.strip().replace("///", "").strip()
                        for line in comment_text.split('\n')
                        if line.strip()
                        and not line.strip().startswith("Circle:")
                        and not line.strip().startswith("Reagents:")]

        if comment_lines:
            # Filter out redundant spell name repetitions
            filtered_lines = [line for line in comment_lines
                            if not (line == spell_name or line.startswith(spell_name + " - " + spell_name))]
            if filtered_lines:
                description_parts.append(" ".join(filtered_lines))

        # Add design description if available and different
        if design_desc:
            description_parts.append(f"[Design] {design_desc}")

        # Add TODO text if meaningful
        if todo_text and todo_text.strip() not in " ".join(description_parts):
            # Skip if TODO just repeats spell name
            if not (todo_text.strip() == spell_name or
                   todo_text.strip().startswith(spell_name + " ")):
                description_parts.append(f"[TODO] {todo_text.strip()}")

        description = " | ".join(description_parts) if description_parts else "No description available"

        # Determine type using all available info
        spell_type = determine_spell_type(comment_text, todo_text, design_desc)

        # Generate gameplay if not provided
        if not design_gameplay:
            gameplay = generate_gameplay_from_description(spell_type, spell_name, description, design_desc)
        else:
            gameplay = design_gameplay

        # Extract structured data
        full_text = description + " " + design_desc
        damage, damage_type = extract_damage(full_text)
        healing = extract_healing(full_text)
        duration = extract_duration(full_text)
        area = extract_area(full_text)
        cooldown = extract_cooldown(full_text)
        effects = extract_effects(full_text, spell_type)

        return {
            "school": school_name,
            "spell_name": spell_name,
            "type": spell_type,
            "circle": circle,
            "mana": mana,
            "damage": damage,
            "damage_type": damage_type,
            "healing": healing,
            "duration": duration,
            "area": area,
            "cooldown": cooldown,
            "effect_1": effects[0] if len(effects) > 0 else "",
            "effect_2": effects[1] if len(effects) > 1 else "",
            "effect_3": effects[2] if len(effects) > 2 else "",
            "description": description,
            "gameplay": gameplay,
            "class_name": class_name
        }

    except Exception as e:
        print(f"Error parsing {file_path}: {e}")
        return None

def main():
    spells_path = r"C:\DevEnv\GIT\UO\ServUO\Scripts\Custom\VystiaClasses\Spells"
    design_path = r"C:\DevEnv\GIT\UO\Vystia\Magic"

    spells = []

    print("Loading design documents...\n")

    # Process each school
    for school_dir, info in SCHOOL_INFO.items():
        school_name = info["name"]
        md_file = info["md_file"]

        # Load design descriptions
        md_file_path = os.path.join(design_path, md_file)
        design_descriptions = load_design_descriptions(md_file_path)
        print(f"Loaded {school_name}: {len(design_descriptions)} descriptions from design doc")

        # Process C# spell files
        school_path = os.path.join(spells_path, school_dir)

        if not os.path.exists(school_path):
            print(f"[SKIP] {school_name}: Spell directory not found")
            continue

        spell_files = [f for f in os.listdir(school_path) if f.endswith(".cs")]

        for spell_file in spell_files:
            file_path = os.path.join(school_path, spell_file)
            spell_data = parse_spell_file(file_path, school_name, design_descriptions)

            if spell_data:
                spells.append(spell_data)

    # Sort by school and spell name
    spells.sort(key=lambda x: (x["school"], x["spell_name"]))

    # Write to CSV
    output_file = "vystia_spells_complete_enhanced.csv"
    fieldnames = [
        "school", "spell_name", "type", "circle", "mana",
        "damage", "damage_type", "healing", "duration", "area", "cooldown",
        "effect_1", "effect_2", "effect_3",
        "gameplay", "description", "class_name"
    ]
    with open(output_file, 'w', newline='', encoding='utf-8') as f:
        writer = csv.DictWriter(f, fieldnames=fieldnames)
        writer.writeheader()
        writer.writerows(spells)

    print(f"\n{'='*70}")
    print(f"Total spells extracted: {len(spells)}")
    print(f"Output saved to: {output_file}")
    print(f"{'='*70}\n")

    # Count spells with design descriptions
    with_design = len([s for s in spells if "[Design]" in s["description"]])
    print(f"Spells with design descriptions: {with_design}/{len(spells)} ({100*with_design//len(spells)}%)")

if __name__ == "__main__":
    main()
