#!/usr/bin/env python3
"""
Spell Alignment Analysis Tool
Analyzes client/server/documentation alignment for all Vystia magic schools

Usage: python analyze_spell_alignment.py
Output: Vystia/admin/*_ALIGNMENT_ANALYSIS.md files
"""

import os
import re
from pathlib import Path

# Paths
ROOT = Path(r"D:\UO")
SERVER_INIT = ROOT / "ServUO/Scripts/Custom/VystiaClasses/Spells/VystiaSpellInitializer.cs"
CLIENT_DIR = ROOT / "ClassicUO/src/ClassicUO.Client/Game/Data"
DOCS_DIR = ROOT / "Vystia/design/magic"
OUTPUT_DIR = ROOT / "Vystia/admin"

# Magic school definitions
MAGIC_SCHOOLS = [
    {"name": "Ice Magic", "server_start": 1000, "client_file": "SpellsVystiaIceMagic.cs", "doc_file": "IceMagic.md", "class": "IceMage"},
    {"name": "Nature Magic", "server_start": 1031, "client_file": "SpellsVystiaNature.cs", "doc_file": "NatureMagic.md", "class": "Druid"},
    {"name": "Hex Magic", "server_start": 1063, "client_file": "SpellsVystiaHex.cs", "doc_file": "HexMagic.md", "class": "Witch"},
    {"name": "Elemental Magic", "server_start": 1095, "client_file": "SpellsVystiaElemental.cs", "doc_file": "ElementalMagic.md", "class": "Sorcerer"},
    {"name": "Dark Magic", "server_start": 1127, "client_file": "SpellsVystiaDark.cs", "doc_file": "DarkMagic.md", "class": "Warlock"},
    {"name": "Divination Magic", "server_start": 1159, "client_file": "SpellsVystiaDivination.cs", "doc_file": "DivinationMagic.md", "class": "Oracle"},
    {"name": "Necromancy", "server_start": 1191, "client_file": "SpellsVystiaNecromancy.cs", "doc_file": "Necromancy.md", "class": "Necromancer"},
    {"name": "Summoning Magic", "server_start": 1223, "client_file": "SpellsVystiaSummoning.cs", "doc_file": "SummoningMagic.md", "class": "Summoner"},
    {"name": "Shamanic Magic", "server_start": 1255, "client_file": "SpellsVystiaShamanic.cs", "doc_file": "ShamanicMagic.md", "class": "Shaman"},
    {"name": "Bardic Magic", "server_start": 1287, "client_file": "SpellsVystiaBardic.cs", "doc_file": "BardicMagic.md", "class": "Bard"},
    {"name": "Enchanting Magic", "server_start": 1319, "client_file": "SpellsVystiaEnchanting.cs", "doc_file": "EnchantingMagic.md", "class": "Enchanter"},
    {"name": "Illusion Magic", "server_start": 1351, "client_file": "SpellsVystiaIllusion.cs", "doc_file": "IllusionMagic.md", "class": "Illusionist"},
]

def extract_server_spells(school_info):
    """Extract spell registrations from server initializer"""
    spells = []
    with open(SERVER_INIT, 'r', encoding='utf-8') as f:
        content = f.read()
    
    # Find the registration method for this school
    method_pattern = rf"Register{school_info['class']}Spells\(\)"
    method_match = re.search(method_pattern, content)
    if not method_match:
        return spells
    
    # Extract all Register() calls in this method
    start_pos = method_match.end()
    # Find the next method or end of class
    next_method = re.search(r'\n\s+private static void Register\w+Spells\(\)', content[start_pos:])
    end_pos = start_pos + (next_method.start() if next_method else len(content) - start_pos)
    
    method_content = content[start_pos:end_pos]
    
    # Extract Register(ID, typeof(SpellName))
    register_pattern = r'Register\((\d+),\s+typeof\((\w+Spell)\)\)'
    for match in re.finditer(register_pattern, method_content):
        spell_id = int(match.group(1))
        spell_class = match.group(2)
        spells.append({"id": spell_id, "class": spell_class})
    
    return spells

def extract_client_spells(client_file):
    """Extract spell definitions from client file"""
    spells = []
    client_path = CLIENT_DIR / client_file
    if not client_path.exists():
        return spells
    
    with open(client_path, 'r', encoding='utf-8') as f:
        content = f.read()
    
    # Extract spell definitions: { key, new SpellDefinition("Name", ID, ...) }
    pattern = r'{\s*(\d+),\s*new SpellDefinition\s*\(\s*"([^"]+)",\s*(\d+),'
    for match in re.finditer(pattern, content):
        key = int(match.group(1))
        name = match.group(2)
        spell_id = int(match.group(3))
        spells.append({"key": key, "name": name, "id": spell_id})
    
    return spells

def analyze_school(school_info):
    """Analyze a single magic school"""
    print(f"Analyzing {school_info['name']}...")
    
    server_spells = extract_server_spells(school_info)
    client_spells = extract_client_spells(school_info['client_file'])
    
    # Create analysis document
    output_file = OUTPUT_DIR / f"{school_info['name'].upper().replace(' ', '_')}_ALIGNMENT_ANALYSIS.md"
    
    with open(output_file, 'w', encoding='utf-8') as f:
        f.write(f"# {school_info['name']} Alignment Analysis\n\n")
        f.write(f"**Date:** 2025-12-13\n")
        f.write(f"**Class:** {school_info['class']}\n")
        f.write(f"**Expected ID Range:** {school_info['server_start']}-{school_info['server_start'] + 31}\n\n")
        f.write("---\n\n")
        
        f.write("## Spell ID Mapping Comparison\n\n")
        f.write("| # | Spell Name | Server ID | Client Dict Key | Client ID | Circle | Status |\n")
        f.write("|---|------------|-----------|-----------------|-----------|--------|--------|\n")
        
        issues = []
        for i, server_spell in enumerate(server_spells, 1):
            expected_id = school_info['server_start'] + i - 1
            client_spell = client_spells[i - 1] if i - 1 < len(client_spells) else None
            
            status = "✅ Match"
            if server_spell['id'] != expected_id:
                status = "⚠️ **ID MISMATCH**"
                issues.append(f"Spell {i}: Expected ID {expected_id}, got {server_spell['id']}")
            elif client_spell and client_spell['id'] != server_spell['id']:
                status = "⚠️ **CLIENT MISMATCH**"
                issues.append(f"Spell {i}: Server ID {server_spell['id']}, Client ID {client_spell['id']}")
            
            client_key = client_spell['key'] if client_spell else "?"
            client_id = client_spell['id'] if client_spell else "?"
            spell_name = client_spell['name'] if client_spell else server_spell['class'].replace('Spell', '')
            
            f.write(f"| {i} | {spell_name} | {server_spell['id']} | {client_key} | {client_id} | ? | {status} |\n")
        
        f.write("\n---\n\n")
        f.write("## Issues Found\n\n")
        if issues:
            for issue in issues:
                f.write(f"- {issue}\n")
        else:
            f.write("✅ No issues found!\n")
    
    print(f"  Created: {output_file.name}")
    return len(issues)

def main():
    """Main analysis function"""
    print("Starting spell alignment analysis...\n")
    
    total_issues = 0
    for school in MAGIC_SCHOOLS:
        issues = analyze_school(school)
        total_issues += issues
    
    print(f"\nAnalysis complete! Found {total_issues} total issues across all schools.")
    print(f"Reports saved to: {OUTPUT_DIR}")

if __name__ == "__main__":
    main()

