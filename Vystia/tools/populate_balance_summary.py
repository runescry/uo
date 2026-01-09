#!/usr/bin/env python3
"""
Extract spell type counts from balance analysis files and populate comprehensive summary
"""

import os
import re
from pathlib import Path

ROOT = Path(r"D:\UO")
ANALYSES_DIR = ROOT / "Vystia/admin/analyses"
SUMMARY_FILE = ROOT / "Vystia/admin/COMPREHENSIVE_BALANCE_SUMMARY.md"

SCHOOLS = [
    "Ice Magic", "Nature Magic", "Hex Magic", "Elemental Magic",
    "Dark Magic", "Divination Magic", "Necromancy", "Summoning Magic",
    "Shamanic Magic", "Bardic Magic", "Enchanting Magic", "Illusion Magic"
]

def extract_counts(analysis_file):
    """Extract spell type counts from a balance analysis file"""
    counts = {
        "Damage": 0,
        "Healing": 0,
        "Defense": 0,  # Buff
        "Debuff": 0,
        "Area": 0,  # Area Control + AoE Damage
        "Summon": 0,
        "CC": 0  # Crowd Control (paralyze, stun, etc.)
    }
    
    if not analysis_file.exists():
        return counts
    
    with open(analysis_file, 'r', encoding='utf-8') as f:
        content = f.read()
    
    # Extract counts from the "Spell Distribution by Type" table
    # Pattern: | Type | Count | Percentage |
    type_pattern = r'\|\s*([^\|]+?)\s*\|\s*(\d+)\s*\|'
    
    for match in re.finditer(type_pattern, content):
        spell_type = match.group(1).strip()
        count = int(match.group(2))
        
        # Map to our categories
        if "Damage" in spell_type and "AoE" not in spell_type:
            counts["Damage"] += count
        elif "AoE Damage" in spell_type:
            counts["Area"] += count
        elif "Healing" in spell_type:
            counts["Healing"] += count
        elif "Buff" in spell_type:
            counts["Defense"] += count
        elif "Debuff" in spell_type:
            counts["Debuff"] += count
        elif "Area Control" in spell_type:
            counts["Area"] += count
        elif "Summon" in spell_type:
            counts["Summon"] += count
        elif "CC" in spell_type or "Crowd Control" in spell_type:
            counts["CC"] += count
    
    return counts

def main():
    """Update the comprehensive balance summary table"""
    print("Extracting spell type counts from balance analyses...\n")
    
    # Read current summary
    with open(SUMMARY_FILE, 'r', encoding='utf-8') as f:
        summary_content = f.read()
    
    # Extract counts for each school
    school_data = []
    for school in SCHOOLS:
        file_name = school.upper().replace(' ', '_') + '_BALANCE_ANALYSIS.md'
        analysis_file = ANALYSES_DIR / file_name
        counts = extract_counts(analysis_file)
        
        # Calculate totals
        total = sum(counts.values())
        
        # Create notes
        notes = []
        if counts["Healing"] == 0:
            notes.append("No healing")
        elif counts["Healing"] == 1:
            notes.append("Limited healing")
        if counts["Defense"] == 0:
            notes.append("No defense")
        if counts["Summon"] > 0:
            notes.append(f"{counts['Summon']} summon(s)")
        
        note_str = ", ".join(notes) if notes else "Balanced"
        
        school_data.append({
            "name": school,
            "damage": counts["Damage"],
            "healing": counts["Healing"],
            "defense": counts["Defense"],
            "debuff": counts["Debuff"],
            "area": counts["Area"],
            "summon": counts["Summon"],
            "cc": counts["CC"],
            "notes": note_str
        })
    
    # Replace the table in the summary
    table_start = summary_content.find("## Magic School Balance Overview")
    table_end = summary_content.find("---", table_start + 1)
    
    if table_start != -1 and table_end != -1:
        # Build new table
        new_table = "## Magic School Balance Overview\n\n"
        new_table += "| School | Damage | Healing | Defense | Debuff | Area | Summon | CC | Notes |\n"
        new_table += "|--------|--------|---------|---------|--------|------|--------|----|----|\n"
        
        for data in school_data:
            new_table += f"| {data['name']} | {data['damage']} | {data['healing']} | {data['defense']} | {data['debuff']} | {data['area']} | {data['summon']} | {data['cc']} | {data['notes']} |\n"
        
        new_table += "\n"
        
        # Replace the old table section
        before_table = summary_content[:table_start]
        after_table = summary_content[table_end:]
        new_content = before_table + new_table + after_table
        
        # Write back
        with open(SUMMARY_FILE, 'w', encoding='utf-8') as f:
            f.write(new_content)
        
        print("[OK] Updated COMPREHENSIVE_BALANCE_SUMMARY.md with spell type counts")
        print("\nSummary:")
        for data in school_data:
            print(f"  {data['name']}: Dmg={data['damage']}, Heal={data['healing']}, Def={data['defense']}, Debuff={data['debuff']}, Area={data['area']}, Summon={data['summon']}, CC={data['cc']}")
    else:
        print("[ERROR] Could not find table section in summary file")

if __name__ == "__main__":
    main()

