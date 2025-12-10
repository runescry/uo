"""
Fix spell registration order for all 12 Vystia magic schools.
The bug: All schools (except Ice) registered spells using CLIENT IDs instead of SERVER IDs.
The fix: Server ID = Client ID - 1 (UO protocol offset).
"""

import re
import os

# School configurations
SCHOOLS = [
    # (client_file, server_method_name, client_base_id, server_base_id, class_prefix)
    ("SpellsVystiaIceMagic.cs", "RegisterIceMageSpells", 1000, 999, "IceMage", "Ice"),  # Already correct
    ("SpellsVystiaNature.cs", "RegisterDruidSpells", 1032, 1031, "Nature", "Druid"),  # Already fixed
    ("SpellsVystiaHex.cs", "RegisterWitchSpells", 1064, 1063, "Hex", "Witch"),
    ("SpellsVystiaElemental.cs", "RegisterSorcererSpells", 1096, 1095, "Elemental", "Sorcerer"),
    ("SpellsVystiaDark.cs", "RegisterWarlockSpells", 1128, 1127, "Dark", "Warlock"),
    ("SpellsVystiaDivination.cs", "RegisterOracleSpells", 1160, 1159, "Divination", "Oracle"),
    ("SpellsVystiaNecromancy.cs", "RegisterNecromancerSpells", 1192, 1191, "Necromancy", "Necromancer"),
    ("SpellsVystiaSummoning.cs", "RegisterSummonerSpells", 1224, 1223, "Summoning", "Summoner"),
    ("SpellsVystiaShamanic.cs", "RegisterShamanSpells", 1256, 1255, "Shamanic", "Shaman"),
    ("SpellsVystiaBardic.cs", "RegisterBardSpells", 1288, 1287, "Bardic", "Bard"),
    ("SpellsVystiaEnchanting.cs", "RegisterEnchanterSpells", 1320, 1319, "Enchanting", "Enchanter"),
    ("SpellsVystiaIllusion.cs", "RegisterIllusionistSpells", 1352, 1351, "Illusion", "Illusionist"),
]

CLIENT_PATH = r"C:\DevEnv\GIT\UO\ClassicUO\src\ClassicUO.Client\Game\Data"

def extract_spells_from_client(file_path):
    """Extract spell names and IDs from client-side spell definition file."""
    spells = []  # List of (index, name, client_id)

    with open(file_path, 'r', encoding='utf-8') as f:
        content = f.read()

    # Find all spell definitions
    # Pattern: {index, new SpellDefinition("Spell Name", client_id, ...)}
    pattern = r'\{\s*(\d+),\s*new SpellDefinition\s*\(\s*"([^"]+)",\s*(\d+),'
    matches = re.findall(pattern, content)

    for match in matches:
        index = int(match[0])
        name = match[1]
        client_id = int(match[2])
        spells.append((index, name, client_id))

    # Sort by index to ensure correct order
    spells.sort(key=lambda x: x[0])
    return spells

def spell_name_to_class_name(spell_name, prefix):
    """Convert spell name to C# class name."""
    # Remove special characters and convert to PascalCase
    # Example: "Nature's Touch" → "NaturesTouchSpell"
    # Example: "Animal Aspect: Speed" → "AnimalAspectSpeedSpell"

    # Remove apostrophes and colons
    name = spell_name.replace("'", "").replace(":", "")

    # Split on spaces and capitalize each word
    words = name.split()
    class_name = "".join(word.capitalize() for word in words)

    return f"{prefix}{class_name}Spell"

def generate_registration_code(spells, server_base_id, class_prefix, school_name):
    """Generate corrected Register() calls for server-side."""
    lines = []
    lines.append(f"        private static void Register{school_name}Spells()")
    lines.append("        {")
    lines.append(f"            // {school_name} Magic Spells (Server IDs {server_base_id}-{server_base_id+31}, Client sends {server_base_id+1}-{server_base_id+32})")
    lines.append("            // CRITICAL: Server IDs = Client IDs - 1 (UO protocol offset)")

    for i, (index, spell_name, client_id) in enumerate(spells):
        server_id = client_id - 1
        class_name = spell_name_to_class_name(spell_name, class_prefix)

        # Format with proper spacing
        line = f"            Register({server_id}, typeof({class_name}));"
        padding = 70 - len(line)
        if padding > 0:
            line += " " * padding
        line += f"// Client: {client_id} - {spell_name}"

        lines.append(line)

    lines.append("        }")
    lines.append("")

    return "\n".join(lines)

def main():
    print("=" * 80)
    print("FIXING ALL VYSTIA SPELL REGISTRATIONS")
    print("=" * 80)
    print()

    all_registrations = []

    for i, (client_file, method_name, client_base, server_base, class_prefix, school_name) in enumerate(SCHOOLS, 1):
        print(f"{i}/12 Processing {school_name} Magic ({client_file})...")

        client_path = os.path.join(CLIENT_PATH, client_file)

        if not os.path.exists(client_path):
            print(f"  [WARNING] Client file not found: {client_path}")
            continue

        # Extract spells from client
        spells = extract_spells_from_client(client_path)
        print(f"  [OK] Extracted {len(spells)} spells")

        # Generate corrected registration code
        code = generate_registration_code(spells, server_base, class_prefix, school_name)
        all_registrations.append((school_name, code))

        # Verify spell order
        if spells:
            first_spell = spells[0]
            expected_server_id = first_spell[2] - 1
            print(f"  [OK] First spell: '{first_spell[1]}' (Client {first_spell[2]} -> Server {expected_server_id})")

        print()

    # Write all corrected registrations to output file
    output_path = r"C:\DevEnv\GIT\UO\Vystia\CORRECTED_SPELL_REGISTRATIONS.txt"
    with open(output_path, 'w', encoding='utf-8') as f:
        f.write("// CORRECTED SPELL REGISTRATIONS FOR ALL 12 VYSTIA MAGIC SCHOOLS\n")
        f.write("// Generated by fix_all_spell_registrations.py\n")
        f.write("// Copy-paste each method into VystiaSpellInitializer.cs\n\n")

        for school_name, code in all_registrations:
            f.write(f"// ============================================================\n")
            f.write(f"// {school_name} Magic\n")
            f.write(f"// ============================================================\n\n")
            f.write(code)
            f.write("\n\n")

    print("=" * 80)
    print(f"[COMPLETE!] Corrected registrations written to:")
    print(f"  {output_path}")
    print("=" * 80)
    print()
    print("NEXT STEPS:")
    print("1. Review the generated file")
    print("2. Copy each method into VystiaSpellInitializer.cs (replacing old versions)")
    print("3. Rebuild ServUO server")
    print("4. Test all 12 spellbooks in-game")

if __name__ == "__main__":
    main()
