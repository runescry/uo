"""
Fix spell registration order for all 10 remaining schools by matching client-side order.
"""

import os
import re

CLIENT_PATH = r"C:\DevEnv\GIT\UO\ClassicUO\src\ClassicUO.Client\Game\Data"
SERVER_FILE = r"C:\DevEnv\GIT\UO\ServUO\Scripts\Custom\VystiaClasses\Spells\VystiaSpellInitializer.cs"

# Schools to fix (Ice and Druid already correct)
SCHOOLS = [
    ("SpellsVystiaHex.cs", "RegisterWitchSpells", "Hex", 1063),
    ("SpellsVystiaElemental.cs", "RegisterSorcererSpells", "Elemental", 1095),
    ("SpellsVystiaDark.cs", "RegisterWarlockSpells", "Dark", 1127),
    ("SpellsVystiaDivination.cs", "RegisterOracleSpells", "Divination", 1159),
    ("SpellsVystiaNecromancy.cs", "RegisterNecromancerSpells", "Necromancy", 1191),
    ("SpellsVystiaSummoning.cs", "RegisterSummonerSpells", "Summoning", 1223),
    ("SpellsVystiaShamanic.cs", "RegisterShamanSpells", "Shamanic", 1255),
    ("SpellsVystiaBardic.cs", "RegisterBardSpells", "Bardic", 1287),
    ("SpellsVystiaEnchanting.cs", "RegisterEnchanterSpells", "Enchanting", 1319),
    ("SpellsVystiaIllusion.cs", "RegisterIllusionistSpells", "Illusion", 1351),
]

def extract_spell_classes_from_client(client_file):
    """Extract spell class names in order from client spell definition file."""
    with open(client_file, 'r', encoding='utf-8') as f:
        content = f.read()

    # Find all spell definitions in order
    # Pattern: {index, new SpellDefinition("Spell Name", id, ...)}
    pattern = r'\{\s*(\d+),\s*new SpellDefinition\s*\(\s*"([^"]+)"'
    matches = re.findall(pattern, content)

    # Sort by index to ensure correct order
    spells = [(int(idx), name) for idx, name in matches]
    spells.sort(key=lambda x: x[0])

    return [name for _, name in spells]

def spell_name_to_class_name(spell_name, prefix):
    """Convert spell name to actual C# class name (lowercase 'of', 'the', 'and')."""
    # Remove special characters
    name = spell_name.replace("'", "").replace(":", "").replace("-", "")

    # Split on spaces
    words = name.split()

    # Capitalize, but keep small words lowercase
    small_words = {"of", "the", "and", "a", "an", "to", "in", "on", "at"}
    capitalized = []
    for i, word in enumerate(words):
        if i == 0 or word.lower() not in small_words:
            capitalized.append(word.capitalize())
        else:
            capitalized.append(word.lower())

    class_name = "".join(capitalized)
    return f"{prefix}{class_name}Spell"

def extract_current_server_registrations(method_name, server_content):
    """Extract current spell class registrations from server file."""
    # Find the method
    pattern = rf"private static void {method_name}\(\).*?\{{(.*?)\}}"
    match = re.search(pattern, server_content, re.DOTALL)

    if not match:
        return None

    method_body = match.group(1)

    # Extract all Register() calls
    reg_pattern = r'Register\((\d+),\s*typeof\((\w+)\)\);'
    registrations = re.findall(reg_pattern, method_body)

    return {class_name: int(spell_id) for spell_id, class_name in registrations}

def generate_corrected_method(method_name, spell_names, prefix, base_id, current_registrations):
    """Generate corrected registration method with spells in client order."""
    lines = []
    lines.append(f"        private static void {method_name}()")
    lines.append("        {")
    lines.append(f"            // {prefix} Magic Spells (IDs {base_id}-{base_id+31})")

    for i, spell_name in enumerate(spell_names):
        spell_id = base_id + i
        class_name = spell_name_to_class_name(spell_name, prefix)

        # Check if this class exists in current registrations
        if class_name not in current_registrations:
            print(f"  [WARNING] Class not found: {class_name}")
            continue

        line = f"            Register({spell_id}, typeof({class_name}));"
        lines.append(line)

    lines.append("        }")
    return "\n".join(lines)

def replace_method_in_server(server_content, method_name, new_method):
    """Replace a method in the server file."""
    pattern = rf"(        private static void {method_name}\(\).*?\n        \}})"
    match = re.search(pattern, server_content, re.DOTALL)

    if match:
        old_method = match.group(1)
        return server_content.replace(old_method, new_method)
    return server_content

def main():
    print("=" * 80)
    print("FIXING SPELL REGISTRATION ORDER FOR 10 REMAINING SCHOOLS")
    print("=" * 80)
    print()

    # Read server file
    with open(SERVER_FILE, 'r', encoding='utf-8') as f:
        server_content = f.read()

    fixes_applied = 0

    for client_file, method_name, prefix, base_id in SCHOOLS:
        print(f"Processing {prefix} Magic...")

        # Extract client-side spell order
        client_path = os.path.join(CLIENT_PATH, client_file)
        if not os.path.exists(client_path):
            print(f"  [ERROR] Client file not found: {client_path}")
            continue

        spell_names = extract_spell_classes_from_client(client_path)
        print(f"  [OK] Extracted {len(spell_names)} spells from client")

        # Extract current server registrations
        current_regs = extract_current_server_registrations(method_name, server_content)
        if not current_regs:
            print(f"  [ERROR] Could not find {method_name} in server file")
            continue

        print(f"  [OK] Found {len(current_regs)} current registrations")

        # Generate corrected method
        new_method = generate_corrected_method(method_name, spell_names, prefix, base_id, current_regs)

        # Replace in server content
        server_content = replace_method_in_server(server_content, method_name, new_method)
        fixes_applied += 1
        print(f"  [OK] Generated corrected registration")
        print()

    # Write updated server file
    if fixes_applied > 0:
        with open(SERVER_FILE, 'w', encoding='utf-8') as f:
            f.write(server_content)
        print("=" * 80)
        print(f"[COMPLETE!] Fixed {fixes_applied}/10 schools")
        print("=" * 80)
        print("\nNext: Rebuild ServUO with 'dotnet build'")
    else:
        print("[ERROR] No fixes applied")

if __name__ == "__main__":
    main()
