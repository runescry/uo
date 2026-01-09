#!/usr/bin/env python3
"""
Spell Balance Analysis Tool
Creates detailed balance analysis for all 12 magic schools

Usage: python analyze_spell_balance.py
Output: Vystia/admin/*_BALANCE_ANALYSIS.md files
"""

import os
import re
from pathlib import Path
from collections import defaultdict

# Paths
ROOT = Path(r"D:\UO")
DOCS_DIR = ROOT / "Vystia/design/magic"
OUTPUT_DIR = ROOT / "Vystia/admin"

# Magic school definitions
MAGIC_SCHOOLS = [
    {"name": "Ice Magic", "file": "IceMagic.md", "class": "Ice Mage"},
    {"name": "Nature Magic", "file": "NatureMagic.md", "class": "Druid"},
    {"name": "Hex Magic", "file": "HexMagic.md", "class": "Witch"},
    {"name": "Elemental Magic", "file": "ElementalMagic.md", "class": "Sorcerer"},
    {"name": "Dark Magic", "file": "DarkMagic.md", "class": "Warlock"},
    {"name": "Divination Magic", "file": "DivinationMagic.md", "class": "Oracle"},
    {"name": "Necromancy", "file": "Necromancy.md", "class": "Necromancer"},
    {"name": "Summoning Magic", "file": "SummoningMagic.md", "class": "Summoner"},
    {"name": "Shamanic Magic", "file": "ShamanicMagic.md", "class": "Shaman"},
    {"name": "Bardic Magic", "file": "BardicMagic.md", "class": "Bard"},
    {"name": "Enchanting Magic", "file": "EnchantingMagic.md", "class": "Enchanter"},
    {"name": "Illusion Magic", "file": "IllusionMagic.md", "class": "Illusionist"},
]

def parse_spell_from_doc(doc_file):
    """Parse spell information from documentation"""
    spells = []
    file_path = DOCS_DIR / doc_file
    
    if not file_path.exists():
        return spells
    
    with open(file_path, 'r', encoding='utf-8') as f:
        content = f.read()
    
    # Extract spell sections - handle multiple formats
    # Format 1: #### N. Spell Name (with optional ✅)
    # Format 2: N. **Spell Name** - description
    spells_found = []
    
    # Try format 1 first (detailed format with ####)
    spell_pattern1 = r'####\s+(\d+)\.\s+([^\n]+?)(?:\s*✅)?\s*\n(.*?)(?=####|\Z)'
    for match in re.finditer(spell_pattern1, content, re.DOTALL):
        spell_num = int(match.group(1))
        spell_name = match.group(2).strip()
        spell_content = match.group(3)
        spells_found.append((spell_num, spell_name, spell_content))
    
    # Try format 2 (compact format: N. **Spell Name** - description)
    if not spells_found:
        spell_pattern2 = r'(\d+)\.\s+\*\*([^\*]+)\*\*\s*-\s*([^\n]+)'
        for match in re.finditer(spell_pattern2, content):
            spell_num = int(match.group(1))
            spell_name = match.group(2).strip()
            spell_content = match.group(3).strip()
            spells_found.append((spell_num, spell_name, spell_content))
    
    for spell_num, spell_name, spell_content in spells_found:
        
        # Extract mana cost
        mana_match = re.search(r'\*\*Mana:\*\*\s*(\d+)', spell_content)
        mana = int(mana_match.group(1)) if mana_match else 0
        
        # Extract damage
        damage_match = re.search(r'\*\*Damage:\*\*\s*([^\n]+)', spell_content)
        damage = damage_match.group(1).strip() if damage_match else None
        
        # Extract effect type
        spell_type = "Unknown"
        if "damage" in spell_content.lower():
            spell_type = "Damage"
        if "heal" in spell_content.lower() or "healing" in spell_content.lower():
            spell_type = "Healing"
        if "buff" in spell_content.lower() or "resistance" in spell_content.lower():
            spell_type = "Buff"
        if "slow" in spell_content.lower() or "debuff" in spell_content.lower():
            spell_type = "Debuff"
        if "summon" in spell_content.lower():
            spell_type = "Summon"
        if "area" in spell_content.lower() or "radius" in spell_content.lower():
            if spell_type == "Damage":
                spell_type = "AoE Damage"
            else:
                spell_type = "Area Control"
        
        # Extract circle - find the circle header before this spell
        # Look backwards from the spell position to find the nearest Circle header
        spell_pos = content.find(match.group(0))
        before_spell = content[:spell_pos]
        circle_matches = list(re.finditer(r'###\s+Circle\s+(\d+)', before_spell))
        if circle_matches:
            # Get the last (most recent) circle before this spell
            circle = int(circle_matches[-1].group(1))
        else:
            # Fallback: calculate from spell number (4 spells per circle)
            circle = ((spell_num - 1) // 4) + 1
        
        spells.append({
            "number": spell_num,
            "name": spell_name,
            "mana": mana,
            "damage": damage,
            "type": spell_type,
            "circle": circle
        })
    
    return spells

def analyze_balance(school_info):
    """Create balance analysis for a magic school"""
    print(f"Analyzing balance for {school_info['name']}...")
    
    spells = parse_spell_from_doc(school_info["file"])
    
    if not spells:
        print(f"  Warning: Could not parse spells from {school_info['file']}")
        return
    
    # Categorize spells
    by_type = defaultdict(list)
    by_circle = defaultdict(list)
    
    for spell in spells:
        by_type[spell["type"]].append(spell)
        by_circle[spell["circle"]].append(spell)
    
    # Create analysis document
    output_file = OUTPUT_DIR / f"{school_info['name'].upper().replace(' ', '_')}_BALANCE_ANALYSIS.md"
    
    with open(output_file, 'w', encoding='utf-8') as f:
        f.write(f"# {school_info['name']} Balance Analysis\n\n")
        f.write(f"**Date:** 2025-12-13\n")
        f.write(f"**Class:** {school_info['class']}\n")
        f.write(f"**Total Spells:** {len(spells)}\n\n")
        f.write("---\n\n")
        
        f.write("## Spell Distribution by Type\n\n")
        f.write("| Type | Count | Percentage |\n")
        f.write("|------|-------|------------|\n")
        
        total = len(spells)
        for spell_type in sorted(by_type.keys()):
            count = len(by_type[spell_type])
            percentage = (count / total * 100) if total > 0 else 0
            f.write(f"| {spell_type} | {count} | {percentage:.1f}% |\n")
        
        f.write("\n---\n\n")
        f.write("## Spell Distribution by Circle\n\n")
        f.write("| Circle | Spells | Avg Mana | Types |\n")
        f.write("|--------|--------|----------|-------|\n")
        
        for circle in sorted(by_circle.keys()):
            circle_spells = by_circle[circle]
            avg_mana = sum(s["mana"] for s in circle_spells) / len(circle_spells) if circle_spells else 0
            types = ", ".join(set(s["type"] for s in circle_spells))
            f.write(f"| {circle} | {len(circle_spells)} | {avg_mana:.1f} | {types} |\n")
        
        f.write("\n---\n\n")
        f.write("## Balance Assessment\n\n")
        
        # Check for balance issues
        issues = []
        
        # Check healing availability
        healing_count = len(by_type.get("Healing", []))
        if healing_count == 0:
            issues.append("⚠️ **No healing spells** - Class has no self-healing capability")
        elif healing_count == 1:
            issues.append("⚠️ **Limited healing** - Only 1 healing spell available")
        
        # Check damage progression
        damage_spells = by_type.get("Damage", []) + by_type.get("AoE Damage", [])
        if len(damage_spells) < 8:
            issues.append("⚠️ **Low damage spell count** - May struggle with damage output")
        
        # Check defense availability
        defense_count = len(by_type.get("Buff", []))
        if defense_count == 0:
            issues.append("⚠️ **No defensive buffs** - No protection spells available")
        
        if issues:
            f.write("### Issues Found\n\n")
            for issue in issues:
                f.write(f"- {issue}\n")
        else:
            f.write("✅ **Well Balanced** - Good distribution of spell types\n")
        
        f.write("\n---\n\n")
        f.write("## Detailed Spell List\n\n")
        
        for circle in sorted(by_circle.keys()):
            f.write(f"### Circle {circle}\n\n")
            for spell in sorted(by_circle[circle], key=lambda x: x["number"]):
                f.write(f"- **{spell['name']}** ({spell['mana']} mana) - {spell['type']}\n")
            f.write("\n")
    
    print(f"  Created: {output_file.name}")

def main():
    """Main analysis function"""
    print("Starting spell balance analysis...\n")
    
    for school in MAGIC_SCHOOLS:
        analyze_balance(school)
    
    print(f"\nBalance analysis complete!")
    print(f"Reports saved to: {OUTPUT_DIR}")

if __name__ == "__main__":
    main()

