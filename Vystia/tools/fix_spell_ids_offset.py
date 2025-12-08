"""
Fix spell ID offset by shifting all registrations back by 1

According to VYSTIA_SPELLBOOK_INTEGRATION_GUIDE.md:
- UO protocol: client sends (ID+1), server receives ID
- Server should register at 999-1382 (not 1000-1383)

This script updates VystiaSpellInitializer.cs with correct IDs.
"""

import re

input_file = r"C:\DevEnv\GIT\UO\ServUO\Scripts\Custom\VystiaClasses\Spells\VystiaSpellInitializer.cs"
output_file = input_file  # Overwrite in place

# Read the file
with open(input_file, 'r', encoding='utf-8') as f:
    content = f.read()

# Pattern: Register(1234, typeof(...))
# Replace with Register(1233, typeof(...))
def decrement_register_id(match):
    current_id = int(match.group(1))
    new_id = current_id - 1
    return f"Register({new_id}, typeof({match.group(2)}))"

# Find all Register(ID, typeof(...)) calls and decrement ID by 1
pattern = r'Register\((\d+),\s*typeof\(([^)]+)\)\)'
new_content = re.sub(pattern, decrement_register_id, content)

# Also fix the comment ranges in the file
comment_replacements = [
    ("Ice Magic Spells (IDs 1000-1031)", "Ice Magic Spells (IDs 999-1030)"),
    ("Nature Magic Spells (IDs 1032-1063)", "Nature Magic Spells (IDs 1031-1062)"),
    ("Hex Magic Spells (IDs 1064-1095)", "Hex Magic Spells (IDs 1063-1094)"),
    ("Elemental Magic Spells (IDs 1096-1127)", "Elemental Magic Spells (IDs 1095-1126)"),
    ("Dark Magic Spells (IDs 1128-1159)", "Dark Magic Spells (IDs 1127-1158)"),
    ("Divination Magic Spells (IDs 1160-1191)", "Divination Magic Spells (IDs 1159-1190)"),
    ("Necromancy Spells (IDs 1192-1223)", "Necromancy Spells (IDs 1191-1222)"),
    ("Summoning Magic Spells (IDs 1224-1255)", "Summoning Magic Spells (IDs 1223-1254)"),
    ("Shamanic Magic Spells (IDs 1256-1287)", "Shamanic Magic Spells (IDs 1255-1286)"),
    ("Bardic Magic Spells (IDs 1288-1319)", "Bardic Magic Spells (IDs 1287-1318)"),
    ("Enchanting Magic Spells (IDs 1320-1351)", "Enchanting Magic Spells (IDs 1319-1350)"),
    ("Illusion Magic Spells (IDs 1352-1383)", "Illusion Magic Spells (IDs 1351-1382)"),
]

for old, new in comment_replacements:
    new_content = new_content.replace(old, new)

# Write back
with open(output_file, 'w', encoding='utf-8') as f:
    f.write(new_content)

print("[OK] Fixed VystiaSpellInitializer.cs")
print("   Shifted all spell IDs back by 1 (999-1382)")
print("")
print("Spell ID Ranges (OLD -> NEW):")
print("  Ice Magic:     1000-1031 -> 999-1030")
print("  Druid:         1032-1063 -> 1031-1062")
print("  Witch:         1064-1095 -> 1063-1094")
print("  Sorcerer:      1096-1127 -> 1095-1126")
print("  Warlock:       1128-1159 -> 1127-1158")
print("  Oracle:        1160-1191 -> 1159-1190")
print("  Necromancer:   1192-1223 -> 1191-1222")
print("  Summoner:      1224-1255 -> 1223-1254")
print("  Shaman:        1256-1287 -> 1255-1286")
print("  Bard:          1288-1319 -> 1287-1318")
print("  Enchanter:     1320-1351 -> 1319-1350")
print("  Illusionist:   1352-1383 -> 1351-1382")
