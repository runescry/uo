"""
Increment all spell IDs by 1 to fix over-decrement

Current state (WRONG after double-decrement):
- Ice Magic: 998-1029
- Druid: 1030-1061
- etc.

Target state (CORRECT):
- Ice Magic: 999-1030
- Druid: 1031-1062
- etc.
"""

import re

input_file = r"C:\DevEnv\GIT\UO\ServUO\Scripts\Custom\VystiaClasses\Spells\VystiaSpellInitializer.cs"
output_file = input_file

# Read the file
with open(input_file, 'r', encoding='utf-8') as f:
    content = f.read()

# Pattern: Register(1234, typeof(...))
# Replace with Register(1235, typeof(...))
def increment_register_id(match):
    current_id = int(match.group(1))
    new_id = current_id + 1
    return f"Register({new_id}, typeof({match.group(2)}))"

# Find all Register(ID, typeof(...)) calls and increment ID by 1
pattern = r'Register\((\d+),\s*typeof\(([^)]+)\)\)'
new_content = re.sub(pattern, increment_register_id, content)

# Fix the comment ranges
comment_replacements = [
    ("Ice Magic Spells (IDs 998-1029)", "Ice Magic Spells (IDs 999-1030)"),
    ("Nature Magic Spells (IDs 1030-1061)", "Nature Magic Spells (IDs 1031-1062)"),
    ("Hex Magic Spells (IDs 1062-1093)", "Hex Magic Spells (IDs 1063-1094)"),
    ("Elemental Magic Spells (IDs 1094-1125)", "Elemental Magic Spells (IDs 1095-1126)"),
    ("Dark Magic Spells (IDs 1126-1157)", "Dark Magic Spells (IDs 1127-1158)"),
    ("Divination Magic Spells (IDs 1158-1189)", "Divination Magic Spells (IDs 1159-1190)"),
    ("Necromancy Spells (IDs 1190-1221)", "Necromancy Spells (IDs 1191-1222)"),
    ("Summoning Magic Spells (IDs 1222-1253)", "Summoning Magic Spells (IDs 1223-1254)"),
    ("Shamanic Magic Spells (IDs 1254-1285)", "Shamanic Magic Spells (IDs 1255-1286)"),
    ("Bardic Magic Spells (IDs 1286-1317)", "Bardic Magic Spells (IDs 1287-1318)"),
    ("Enchanting Magic Spells (IDs 1318-1349)", "Enchanting Magic Spells (IDs 1319-1350)"),
    ("Illusion Magic Spells (IDs 1350-1381)", "Illusion Magic Spells (IDs 1351-1382)"),
]

for old, new in comment_replacements:
    new_content = new_content.replace(old, new)

# Write back
with open(output_file, 'w', encoding='utf-8') as f:
    f.write(new_content)

print("[OK] Fixed VystiaSpellInitializer.cs - incremented all IDs by 1")
print("Final Spell ID Ranges (CORRECT):")
print("  Ice Magic:     999-1030")
print("  Druid:         1031-1062")
print("  Witch:         1063-1094")
print("  Sorcerer:      1095-1126")
print("  Warlock:       1127-1158")
print("  Oracle:        1159-1190")
print("  Necromancer:   1191-1222")
print("  Summoner:      1223-1254")
print("  Shaman:        1255-1286")
print("  Bard:          1287-1318")
print("  Enchanter:     1319-1350")
print("  Illusionist:   1351-1382")
