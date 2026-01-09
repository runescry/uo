#!/usr/bin/env python3
"""
Martial Ability Alignment Analysis Tool
Analyzes all 14 martial classes (224 abilities total)

Usage: python analyze_martial_alignment.py
Output: Vystia/admin/*_MARTIAL_ALIGNMENT_ANALYSIS.md files
"""

import os
import re
from pathlib import Path

# Paths
ROOT = Path(r"D:\UO")
MARTIAL_DIR = ROOT / "ServUO/Scripts/Custom/VystiaClasses/Abilities/Generated/Martial"
OUTPUT_DIR = ROOT / "Vystia/admin"
DOCS_DIR = ROOT / "Vystia/design"

# Martial class definitions
MARTIAL_CLASSES = [
    {"name": "Fighter", "ids": (2000, 2015), "file": "FighterAbilities.cs"},
    {"name": "Barbarian", "ids": (2016, 2031), "file": "BarbarianAbilities.cs"},
    {"name": "Monk", "ids": (2032, 2047), "file": "MonkAbilities.cs"},
    {"name": "Rogue", "ids": (2048, 2063), "file": "RogueAbilities.cs"},
    {"name": "Ranger", "ids": (2064, 2079), "file": "RangerAbilities.cs"},
    {"name": "Knight", "ids": (2080, 2095), "file": "KnightAbilities.cs"},
    {"name": "Paladin", "ids": (2096, 2111), "file": "PaladinAbilities.cs"},
    {"name": "Templar", "ids": (2112, 2127), "file": "TemplarAbilities.cs"},
    {"name": "Bounty Hunter", "ids": (2128, 2143), "file": "BountyHunterAbilities.cs"},
    {"name": "Beastmaster", "ids": (2144, 2159), "file": "BeastmasterAbilities.cs"},
    {"name": "Artificer", "ids": (2160, 2175), "file": "ArtificerAbilities.cs"},
    {"name": "Alchemist", "ids": (2176, 2191), "file": "AlchemistAbilities.cs"},
    {"name": "Cleric", "ids": (2192, 2207), "file": "ClericAbilities.cs"},
    {"name": "Wizard", "ids": (2208, 2223), "file": "WizardAbilities.cs"},
]

def extract_abilities(class_info):
    """Extract ability registrations from martial class file"""
    abilities = []
    file_path = MARTIAL_DIR / class_info["file"]
    
    if not file_path.exists():
        return abilities
    
    with open(file_path, 'r', encoding='utf-8') as f:
        content = f.read()
    
    # Extract RegisterAbility calls
    # Pattern: RegisterAbility(...WithId(ID)...WithName("Name")...InCircle(CIRCLE)...
    pattern = r'RegisterAbility.*?WithId\((\d+)\).*?WithName\("([^"]+)"\).*?InCircle\((\d+)\)'
    
    for match in re.finditer(pattern, content, re.DOTALL):
        ability_id = int(match.group(1))
        ability_name = match.group(2)
        circle = int(match.group(3))
        abilities.append({"id": ability_id, "name": ability_name, "circle": circle})
    
    # Also try alternative pattern for different formats
    alt_pattern = r'(\d+),\s*"([^"]+)",\s*AbilitySchool\.\w+,\s*(\d+)'
    for match in re.finditer(alt_pattern, content):
        ability_id = int(match.group(1))
        ability_name = match.group(2)
        circle = int(match.group(3))
        # Avoid duplicates
        if not any(a["id"] == ability_id for a in abilities):
            abilities.append({"id": ability_id, "name": ability_name, "circle": circle})
    
    return sorted(abilities, key=lambda x: x["id"])

def analyze_class(class_info):
    """Analyze a single martial class"""
    print(f"Analyzing {class_info['name']}...")
    
    abilities = extract_abilities(class_info)
    expected_start, expected_end = class_info["ids"]
    
    # Create analysis document
    output_file = OUTPUT_DIR / f"{class_info['name'].upper().replace(' ', '_')}_MARTIAL_ALIGNMENT_ANALYSIS.md"
    
    with open(output_file, 'w', encoding='utf-8') as f:
        f.write(f"# {class_info['name']} Martial Abilities Alignment Analysis\n\n")
        f.write(f"**Date:** 2025-12-13\n")
        f.write(f"**Expected ID Range:** {expected_start}-{expected_end}\n")
        f.write(f"**Total Abilities:** 16 (4 per tier)\n\n")
        f.write("---\n\n")
        
        f.write("## Ability ID Mapping\n\n")
        f.write("| # | Ability Name | ID | Circle | Status |\n")
        f.write("|---|--------------|----|----|--------|\n")
        
        issues = []
        for i, ability in enumerate(abilities, 1):
            expected_id = expected_start + i - 1
            status = "✅ Match"
            
            if ability["id"] != expected_id:
                status = "⚠️ **ID MISMATCH**"
                issues.append(f"Ability {i} ({ability['name']}): Expected ID {expected_id}, got {ability['id']}")
            
            f.write(f"| {i} | {ability['name']} | {ability['id']} | {ability['circle']} | {status} |\n")
        
        # Check for missing abilities
        if len(abilities) < 16:
            missing = 16 - len(abilities)
            issues.append(f"Missing {missing} abilities (expected 16, found {len(abilities)})")
        
        f.write("\n---\n\n")
        f.write("## Issues Found\n\n")
        if issues:
            for issue in issues:
                f.write(f"- {issue}\n")
        else:
            f.write("✅ No issues found!\n")
        
        f.write("\n---\n\n")
        f.write("## Balance Analysis\n\n")
        f.write("### By Tier\n\n")
        
        # Group by circle
        by_circle = {}
        for ability in abilities:
            circle = ability["circle"]
            if circle not in by_circle:
                by_circle[circle] = []
            by_circle[circle].append(ability)
        
        for circle in sorted(by_circle.keys()):
            f.write(f"#### Tier {circle} ({len(by_circle[circle])} abilities)\n\n")
            for ability in by_circle[circle]:
                f.write(f"- **{ability['name']}** (ID: {ability['id']})\n")
            f.write("\n")
    
    print(f"  Created: {output_file.name}")
    return len(issues)

def main():
    """Main analysis function"""
    print("Starting martial ability alignment analysis...\n")
    
    total_issues = 0
    for class_info in MARTIAL_CLASSES:
        issues = analyze_class(class_info)
        total_issues += issues
    
    print(f"\nAnalysis complete! Found {total_issues} total issues across all martial classes.")
    print(f"Reports saved to: {OUTPUT_DIR}")

if __name__ == "__main__":
    main()

