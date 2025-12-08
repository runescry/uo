"""
Fix all 12 spellbook BookOffset values to match corrected spell IDs
"""

import re

input_file = r"C:\DevEnv\GIT\UO\ServUO\Scripts\Items\Equipment\Spellbooks\VystiaSpellbooks.cs"
output_file = input_file

# Read the file
with open(input_file, 'r', encoding='utf-8') as f:
    content = f.read()

# All replacements (BookOffset and comments)
replacements = [
    # Ice Mage - already done manually, but include for completeness
    ("BookOffset => 1000; // Spell IDs 1000-1031", "BookOffset => 999; // Spell IDs 999-1030"),

    # Druid
    ("BookOffset => 1032; // Spell IDs 1032-1063", "BookOffset => 1031; // Spell IDs 1031-1062"),

    # Witch
    ("BookOffset => 1064; // Spell IDs 1064-1095", "BookOffset => 1063; // Spell IDs 1063-1094"),

    # Sorcerer
    ("BookOffset => 1096; // Spell IDs 1096-1127", "BookOffset => 1095; // Spell IDs 1095-1126"),

    # Warlock
    ("BookOffset => 1128; // Spell IDs 1128-1159", "BookOffset => 1127; // Spell IDs 1127-1158"),

    # Oracle
    ("BookOffset => 1160; // Spell IDs 1160-1191", "BookOffset => 1159; // Spell IDs 1159-1190"),

    # Necromancer
    ("BookOffset => 1192; // Spell IDs 1192-1223", "BookOffset => 1191; // Spell IDs 1191-1222"),

    # Summoner
    ("BookOffset => 1224; // Spell IDs 1224-1255", "BookOffset => 1223; // Spell IDs 1223-1254"),

    # Shaman
    ("BookOffset => 1256; // Spell IDs 1256-1287", "BookOffset => 1255; // Spell IDs 1255-1286"),

    # Bard
    ("BookOffset => 1288; // Spell IDs 1288-1319", "BookOffset => 1287; // Spell IDs 1287-1318"),

    # Enchanter
    ("BookOffset => 1320; // Spell IDs 1320-1351", "BookOffset => 1319; // Spell IDs 1319-1350"),

    # Illusionist
    ("BookOffset => 1352; // Spell IDs 1352-1383", "BookOffset => 1351; // Spell IDs 1351-1382"),
]

for old, new in replacements:
    content = content.replace(old, new)

# Write back
with open(output_file, 'w', encoding='utf-8') as f:
    f.write(content)

print("[OK] Fixed all 12 spellbook BookOffset values")
print("BookOffset changes:")
print("  IceMageSpellbook:          999 (was 1000)")
print("  DruidSpellbook:            1031 (was 1032)")
print("  WitchSpellbook:            1063 (was 1064)")
print("  SorcererSpellbook:         1095 (was 1096)")
print("  WarlockSpellbook:          1127 (was 1128)")
print("  OracleSpellbook:           1159 (was 1160)")
print("  VystiaNecromancerSpellbook: 1191 (was 1192)")
print("  SummonerSpellbook:         1223 (was 1224)")
print("  ShamanSpellbook:           1255 (was 1256)")
print("  BardSpellbook:             1287 (was 1288)")
print("  EnchanterSpellbook:        1319 (was 1320)")
print("  IllusionistSpellbook:      1351 (was 1352)")
