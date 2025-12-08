#!/usr/bin/env python3
"""
Update VYSTIA_MASTER_INVENTORY.md to match actual implemented reagents
"""

import os
import re
from pathlib import Path

# Project paths
SERVUO_ROOT = Path("C:/DevEnv/GIT/UO/ServUO")
REAGENT_DIR = SERVUO_ROOT / "Scripts/Items/Vystia/Resources/Reagents"
INVENTORY_FILE = Path("C:/DevEnv/GIT/UO/Vystia/VYSTIA_MASTER_INVENTORY.md")

def extract_classes_from_file(filepath):
    """Extract all class names that inherit from BaseVystiaReagent"""
    classes = []
    try:
        with open(filepath, 'r', encoding='utf-8') as f:
            content = f.read()
            pattern = r'public class (\w+)\s*:\s*BaseVystiaReagent'
            matches = re.findall(pattern, content)
            classes.extend(matches)
    except Exception as e:
        print(f"Error reading {filepath}: {e}")
    return classes

def get_actual_reagents():
    """Get all actually implemented reagents by school"""
    school_mapping = {
        "IceMagic": ("Ice", "🧊"),
        "NatureMagic": ("Nature", "🌿"),
        "HexMagic": ("Hex", "☠️"),
        "ElementalMagic": ("Elemental", "🔥"),
        "DarkMagic": ("Dark", "🌑"),
        "DivinationMagic": ("Divination", "🔮"),
        "Necromancy": ("Necromancy", "💀"),
        "SummoningMagic": ("Summoning", "🐉"),
        "ShamanicMagic": ("Shamanic", "⚡"),
        "BardicMagic": ("Bardic", "🎵"),
        "EnchantingMagic": ("Enchanting", "✨"),
        "IllusionMagic": ("Illusion", "👁️")
    }

    reagents_by_school = {}

    for filename, (school, emoji) in school_mapping.items():
        filepath = REAGENT_DIR / f"{filename}Reagents.cs"
        if filepath.exists():
            classes = extract_classes_from_file(filepath)
            # Convert CamelCase to Title Case with spaces
            formatted = [re.sub(r'([a-z])([A-Z])', r'\1 \2', c) for c in classes]
            reagents_by_school[school] = {
                'classes': classes,
                'formatted': formatted,
                'emoji': emoji
            }
        else:
            print(f"[WARN] File not found: {filepath}")
            reagents_by_school[school] = {'classes': [], 'formatted': [], 'emoji': emoji}

    return reagents_by_school

def generate_reagent_section(school_data):
    """Generate the reagent section markdown for a school"""
    school = school_data['name']
    emoji = school_data['emoji']
    reagents = school_data['formatted']

    lines = []
    lines.append(f"#### {emoji} {school} Magic ({len(reagents)} reagents)")

    for i, reagent in enumerate(reagents, 1):
        lines.append(f"{i}. {reagent}")

    return "\n".join(lines) + "\n"

def update_inventory_file():
    """Update the VYSTIA_MASTER_INVENTORY.md file"""
    print("[INFO] Reading actual implemented reagents...")
    actual_reagents = get_actual_reagents()

    print("[INFO] Generating new reagent sections...")
    new_sections = {}

    for school, data in actual_reagents.items():
        new_sections[school] = generate_reagent_section({
            'name': school,
            'emoji': data['emoji'],
            'formatted': data['formatted']
        })

    # Read current file
    print(f"[INFO] Reading {INVENTORY_FILE}...")
    with open(INVENTORY_FILE, 'r', encoding='utf-8') as f:
        content = f.read()

    # Replace each school's reagent section
    school_order = ['Ice', 'Nature', 'Hex', 'Elemental', 'Dark', 'Divination',
                    'Necromancy', 'Summoning', 'Shamanic', 'Bardic', 'Enchanting', 'Illusion']

    for school in school_order:
        if school in new_sections:
            # Find and replace the school section
            # Pattern: #### emoji SchoolName Magic (X reagents) ... up to next ####
            emoji = actual_reagents[school]['emoji']
            pattern = rf'(####\s*{re.escape(emoji)}\s*{school}\s+Magic\s*\(\d+\s+reagents\))(.*?)((?=####\s*[\U0001F300-\U0001F9FF])|(?=---)|$)'

            replacement = new_sections[school]

            content = re.sub(pattern, replacement, content, flags=re.DOTALL)

    # Update total count
    total_reagents = sum(len(data['classes']) for data in actual_reagents.values())
    content = re.sub(r'\*\*Total: \d+ unique Vystia reagents\*\*',
                     f'**Total: {total_reagents} unique Vystia reagents**', content)

    # Backup original
    backup_path = INVENTORY_FILE.with_suffix('.md.backup')
    print(f"[INFO] Creating backup: {backup_path}")
    with open(backup_path, 'w', encoding='utf-8') as f:
        f.write(content)

    # Write updated file
    print(f"[INFO] Updating {INVENTORY_FILE}...")
    with open(INVENTORY_FILE, 'w', encoding='utf-8') as f:
        f.write(content)

    print(f"[SUCCESS] Updated VYSTIA_MASTER_INVENTORY.md")
    print(f"[INFO] Total reagents documented: {total_reagents}")

    # Print summary
    print("\n" + "="*80)
    print("REAGENT COUNT BY SCHOOL:")
    print("="*80)
    for school in school_order:
        count = len(actual_reagents[school]['classes'])
        print(f"  {school:12} - {count} reagents")
    print(f"\n  TOTAL: {total_reagents} reagents")
    print("="*80)

if __name__ == "__main__":
    update_inventory_file()
