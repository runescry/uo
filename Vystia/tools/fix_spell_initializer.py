#!/usr/bin/env python3
"""
Fix VystiaSpellInitializer.cs with correct spell ID ranges.
Reads the existing inline registrations and reorganizes them into proper methods.
"""

import re
from pathlib import Path

# Read the current file
initializer_path = Path(__file__).parent.parent.parent / "ServUO" / "Scripts" / "Custom" / "VystiaClasses" / "Spells" / "VystiaSpellInitializer.cs"

with open(initializer_path, 'r', encoding='utf-8') as f:
    content = f.read()

# Extract all Register() calls with their spell IDs and types
register_pattern = r'Register\((\d+),\s*typeof\((\w+)\)\);'
all_registrations = re.findall(register_pattern, content)

print(f"Found {len(all_registrations)} spell registrations")

# Group registrations by spell ID range (each school is 32 spells)
schools = {
    "IceMage": {"start": 1000, "end": 1031, "namespace": "IceMage", "display": "Ice Magic", "registrations": []},
    "Druid": {"start": 1032, "end": 1063, "namespace": "Nature", "display": "Nature Magic", "registrations": []},
    "Witch": {"start": 1064, "end": 1095, "namespace": "Hex", "display": "Hex Magic", "registrations": []},
    "Sorcerer": {"start": 1096, "end": 1127, "namespace": "Elemental", "display": "Elemental Magic", "registrations": []},
    "Warlock": {"start": 1128, "end": 1159, "namespace": "Dark", "display": "Dark Magic", "registrations": []},
    "Oracle": {"start": 1160, "end": 1191, "namespace": "Divination", "display": "Divination Magic", "registrations": []},
    "Necromancer": {"start": 1192, "end": 1223, "namespace": "Necromancy", "display": "Necromancy", "registrations": []},
    "Summoner": {"start": 1224, "end": 1255, "namespace": "Summoning", "display": "Summoning Magic", "registrations": []},
    "Shaman": {"start": 1256, "end": 1287, "namespace": "Shamanic", "display": "Shamanic Magic", "registrations": []},
    "Bard": {"start": 1288, "end": 1319, "namespace": "Bardic", "display": "Bardic Magic", "registrations": []},
    "Enchanter": {"start": 1320, "end": 1351, "namespace": "Enchanting", "display": "Enchanting Magic", "registrations": []},
    "Illusionist": {"start": 1352, "end": 1383, "namespace": "Illusion", "display": "Illusion Magic", "registrations": []},
}

# Assign registrations to schools based on spell ID
for spell_id_str, class_name in all_registrations:
    spell_id = int(spell_id_str)

    for school_name, school_data in schools.items():
        if school_data["start"] <= spell_id <= school_data["end"]:
            school_data["registrations"].append((spell_id, class_name))
            break

# Verify counts
for school_name, school_data in schools.items():
    count = len(school_data["registrations"])
    expected = 32
    if count != expected:
        print(f"WARNING: {school_name} has {count} spells, expected {expected}")

# Generate new file content
using_statements = "\n".join([
    f"using Server.Spells.VystiaSpells.{school['namespace']};"
    for school in schools.values()
])

# Generate registration methods
registration_methods = []
method_calls = []

for school_name, school_data in schools.items():
    if not school_data["registrations"]:
        continue

    method_name = f"Register{school_name}Spells"
    method_calls.append(f"            {method_name}();")

    lines = []
    lines.append(f"        private static void {method_name}()")
    lines.append("        {")
    lines.append(f"            // {school_data['display']} Spells (IDs {school_data['start']}-{school_data['end']})")

    # Sort by spell ID
    sorted_regs = sorted(school_data["registrations"], key=lambda x: x[0])
    for spell_id, class_name in sorted_regs:
        lines.append(f"            Register({spell_id}, typeof({class_name}));")

    lines.append("        }")
    lines.append("")

    registration_methods.append("\n".join(lines))

# Generate complete file
new_content = f'''using System;
{using_statements}

namespace Server.Spells
{{
    /// <summary>
    /// Initializes all Vystia custom spells and registers them with the spell registry
    /// Auto-generated to fix spell ID conflicts
    /// </summary>
    public class VystiaSpellInitializer
    {{
        private static bool _initialized = false;

        public static void Initialize()
        {{
            if (_initialized)
            {{
                Console.WriteLine("[VYSTIA] WARNING: Initialize() called again, skipping duplicate registration!");
                return;
            }}

            _initialized = true;
            Console.WriteLine("[VYSTIA] Initializing Vystia spells...");

{chr(10).join(method_calls)}

            Console.WriteLine("[VYSTIA] Spell registration complete. Total: 384 spells");
        }}

{chr(10).join(registration_methods)}
        private static void Register(int spellID, Type type)
        {{
            SpellRegistry.Register(spellID, type);
        }}
    }}
}}
'''

# Write the new file
with open(initializer_path, 'w', encoding='utf-8') as f:
    f.write(new_content)

print(f"\nGenerated new VystiaSpellInitializer.cs")
print(f"Total registration methods: {len(registration_methods)}")
print(f"Total method calls: {len(method_calls)}")
print("\nSpell counts by school:")
for school_name, school_data in schools.items():
    print(f"  {school_name}: {len(school_data['registrations'])} spells")
