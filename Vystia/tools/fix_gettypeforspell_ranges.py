"""
Fix all 12 spell ID ranges in Spellbook.cs GetTypeForSpell() method
"""

import re

input_file = r"C:\DevEnv\GIT\UO\ServUO\Scripts\Items\Equipment\Spellbooks\Spellbook.cs"
output_file = input_file

# Read the file
with open(input_file, 'r', encoding='utf-8') as f:
    content = f.read()

# All replacements - each school's range
replacements = [
    # Ice Magic
    ("else if (spellID >= 1000 && spellID <= 1031)", "else if (spellID >= 999 && spellID <= 1030)"),
    ("return SpellbookType.VystiaIceMage;  // Ice Magic (1000-1031)", "return SpellbookType.VystiaIceMage;  // Ice Magic (999-1030)"),

    # Druid
    ("else if (spellID >= 1032 && spellID <= 1063)", "else if (spellID >= 1031 && spellID <= 1062)"),
    ("return SpellbookType.VystiaDruid;  // Nature Magic (1032-1063)", "return SpellbookType.VystiaDruid;  // Nature Magic (1031-1062)"),

    # Witch
    ("else if (spellID >= 1064 && spellID <= 1095)", "else if (spellID >= 1063 && spellID <= 1094)"),
    ("return SpellbookType.VystiaWitch;  // Hex Magic (1064-1095)", "return SpellbookType.VystiaWitch;  // Hex Magic (1063-1094)"),

    # Sorcerer
    ("else if (spellID >= 1096 && spellID <= 1127)", "else if (spellID >= 1095 && spellID <= 1126)"),
    ("return SpellbookType.VystiaSorcerer;  // Elemental Magic (1096-1127)", "return SpellbookType.VystiaSorcerer;  // Elemental Magic (1095-1126)"),

    # Warlock
    ("else if (spellID >= 1128 && spellID <= 1159)", "else if (spellID >= 1127 && spellID <= 1158)"),
    ("return SpellbookType.VystiaWarlock;  // Dark Magic (1128-1159)", "return SpellbookType.VystiaWarlock;  // Dark Magic (1127-1158)"),

    # Oracle
    ("else if (spellID >= 1160 && spellID <= 1191)", "else if (spellID >= 1159 && spellID <= 1190)"),
    ("return SpellbookType.VystiaOracle;  // Divination Magic (1160-1191)", "return SpellbookType.VystiaOracle;  // Divination Magic (1159-1190)"),

    # Necromancer
    ("else if (spellID >= 1192 && spellID <= 1223)", "else if (spellID >= 1191 && spellID <= 1222)"),
    ("return SpellbookType.VystiaNecromancer;  // Necromancy (1192-1223)", "return SpellbookType.VystiaNecromancer;  // Necromancy (1191-1222)"),

    # Summoner
    ("else if (spellID >= 1224 && spellID <= 1255)", "else if (spellID >= 1223 && spellID <= 1254)"),
    ("return SpellbookType.VystiaSummoner;  // Summoning Magic (1224-1255)", "return SpellbookType.VystiaSummoner;  // Summoning Magic (1223-1254)"),

    # Shaman
    ("else if (spellID >= 1256 && spellID <= 1287)", "else if (spellID >= 1255 && spellID <= 1286)"),
    ("return SpellbookType.VystiaShaman;  // Shamanic Magic (1256-1287)", "return SpellbookType.VystiaShaman;  // Shamanic Magic (1255-1286)"),

    # Bard
    ("else if (spellID >= 1288 && spellID <= 1319)", "else if (spellID >= 1287 && spellID <= 1318)"),
    ("return SpellbookType.VystiaBard;  // Bardic Magic (1288-1319)", "return SpellbookType.VystiaBard;  // Bardic Magic (1287-1318)"),

    # Enchanter
    ("else if (spellID >= 1320 && spellID <= 1351)", "else if (spellID >= 1319 && spellID <= 1350)"),
    ("return SpellbookType.VystiaEnchanter;  // Enchanting Magic (1320-1351)", "return SpellbookType.VystiaEnchanter;  // Enchanting Magic (1319-1350)"),

    # Illusionist
    ("else if (spellID >= 1352 && spellID <= 1383)", "else if (spellID >= 1351 && spellID <= 1382)"),
    ("return SpellbookType.VystiaIllusionist;  // Illusion Magic (1352-1383)", "return SpellbookType.VystiaIllusionist;  // Illusion Magic (1351-1382)"),

    # Update comment at top
    ("// Vystia Custom Spellbooks (12 schools, 384 spells total, IDs 1000-1383)", "// Vystia Custom Spellbooks (12 schools, 384 spells total, IDs 999-1382)"),
]

for old, new in replacements:
    if old not in content:
        print(f"[WARN] Could not find: {old[:50]}...")
    else:
        content = content.replace(old, new)

# Write back
with open(output_file, 'w', encoding='utf-8') as f:
    f.write(content)

print("[OK] Fixed all 12 GetTypeForSpell() ranges in Spellbook.cs")
print("Spell ID range changes:")
print("  Ice Magic:     999-1030 (was 1000-1031)")
print("  Druid:         1031-1062 (was 1032-1063)")
print("  Witch:         1063-1094 (was 1064-1095)")
print("  Sorcerer:      1095-1126 (was 1096-1127)")
print("  Warlock:       1127-1158 (was 1128-1159)")
print("  Oracle:        1159-1190 (was 1160-1191)")
print("  Necromancer:   1191-1222 (was 1192-1223)")
print("  Summoner:      1223-1254 (was 1224-1255)")
print("  Shaman:        1255-1286 (was 1256-1287)")
print("  Bard:          1287-1318 (was 1288-1319)")
print("  Enchanter:     1319-1350 (was 1320-1351)")
print("  Illusionist:   1351-1382 (was 1352-1383)")
