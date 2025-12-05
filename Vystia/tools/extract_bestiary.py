import os
import re
from pathlib import Path
from collections import defaultdict

def extract_creature_info(file_path):
    """Extract creature information from a .cs file."""
    with open(file_path, 'r', encoding='utf-8') as f:
        content = f.read()

    info = {
        'file': os.path.basename(file_path),
        'class_name': '',
        'name': '',
        'title': '',
        'str': '',
        'dex': '',
        'int': '',
        'hits': '',
        'damage': '',
        'resist_phys': '',
        'resist_fire': '',
        'resist_cold': '',
        'resist_poison': '',
        'resist_energy': '',
        'skills': [],
        'fame': '',
        'karma': '',
        'tamable': 'No',
        'taming_skill': '',
        'special_abilities': [],
        'loot': [],
        'notes': ''
    }

    # Extract class name
    class_match = re.search(r'public class (\w+)\s*:', content)
    if class_match:
        info['class_name'] = class_match.group(1)

    # Extract in-game name
    name_match = re.search(r'Name\s*=\s*"([^"]+)"', content)
    if name_match:
        info['name'] = name_match.group(1)

    # Extract title
    title_match = re.search(r'Title\s*=\s*"([^"]+)"', content)
    if title_match:
        info['title'] = title_match.group(1)

    # Extract stats
    str_match = re.search(r'SetStr\((\d+),\s*(\d+)\)', content)
    if str_match:
        info['str'] = f"{str_match.group(1)}-{str_match.group(2)}"

    dex_match = re.search(r'SetDex\((\d+),\s*(\d+)\)', content)
    if dex_match:
        info['dex'] = f"{dex_match.group(1)}-{dex_match.group(2)}"

    int_match = re.search(r'SetInt\((\d+),\s*(\d+)\)', content)
    if int_match:
        info['int'] = f"{int_match.group(1)}-{int_match.group(2)}"

    hits_match = re.search(r'SetHits\((\d+),\s*(\d+)\)', content)
    if hits_match:
        info['hits'] = f"{hits_match.group(1)}-{hits_match.group(2)}"

    damage_match = re.search(r'SetDamage\((\d+),\s*(\d+)\)', content)
    if damage_match:
        info['damage'] = f"{damage_match.group(1)}-{damage_match.group(2)}"

    # Extract resistances
    resist_patterns = [
        (r'SetResistance\(ResistanceType\.Physical,\s*(\d+),\s*(\d+)\)', 'resist_phys'),
        (r'SetResistance\(ResistanceType\.Fire,\s*(\d+),\s*(\d+)\)', 'resist_fire'),
        (r'SetResistance\(ResistanceType\.Cold,\s*(\d+),\s*(\d+)\)', 'resist_cold'),
        (r'SetResistance\(ResistanceType\.Poison,\s*(\d+),\s*(\d+)\)', 'resist_poison'),
        (r'SetResistance\(ResistanceType\.Energy,\s*(\d+),\s*(\d+)\)', 'resist_energy'),
    ]

    for pattern, key in resist_patterns:
        match = re.search(pattern, content)
        if match:
            info[key] = f"{match.group(1)}-{match.group(2)}"

    # Extract skills
    skill_matches = re.findall(r'SetSkill\(SkillName\.(\w+),\s*([\d.]+),\s*([\d.]+)\)', content)
    for skill_name, min_val, max_val in skill_matches:
        info['skills'].append(f"{skill_name}: {min_val}-{max_val}")

    # Extract Fame/Karma
    fame_match = re.search(r'Fame\s*=\s*(-?\d+)', content)
    if fame_match:
        info['fame'] = fame_match.group(1)

    karma_match = re.search(r'Karma\s*=\s*(-?\d+)', content)
    if karma_match:
        info['karma'] = karma_match.group(1)

    # Extract tamable info
    tamable_match = re.search(r'Tamable\s*=\s*(true|false)', content, re.IGNORECASE)
    if tamable_match:
        if tamable_match.group(1).lower() == 'true':
            info['tamable'] = 'Yes'
            # Try to extract min taming requirement
            min_taming_match = re.search(r'MinTameSkill\s*=\s*([\d.]+)', content)
            if min_taming_match:
                info['taming_skill'] = min_taming_match.group(1)
        else:
            info['tamable'] = 'No'

    # Extract special abilities from comments
    comment_match = re.search(r'/\*\*\*.*?\*\*\*/', content, re.DOTALL)
    if comment_match:
        comment = comment_match.group(0)
        # Look for ability descriptions
        abilities = re.findall(r'- (.*?)(?:\n|$)', comment)
        info['special_abilities'] = abilities

    # Try to extract from methods
    if 'DragonBreath' in content or 'BreathAttack' in content:
        info['special_abilities'].append('Breath Attack')
    if 'AuraEffect' in content or 'AuraDamage' in content:
        info['special_abilities'].append('Damage Aura')
    if 'Summon' in content:
        info['special_abilities'].append('Summons Minions')
    if 'Freeze' in content and 'target.Freeze' in content:
        info['special_abilities'].append('Freeze Attack')
    if 'Poison' in content and 'ApplyPoison' in content:
        info['special_abilities'].append('Poison Attack')

    # Extract loot information
    if 'GenerateLoot' in content:
        # Look for PackItem calls
        pack_items = re.findall(r'PackItem\(new (\w+)', content)
        info['loot'] = list(set(pack_items))[:10]  # Limit to 10 items

    # Extract notes from summary comment
    summary_match = re.search(r'/// <summary>(.*?)/// </summary>', content, re.DOTALL)
    if summary_match:
        summary = summary_match.group(1)
        # Clean up
        summary = re.sub(r'///', '', summary)
        summary = summary.strip()
        info['notes'] = summary

    return info

def generate_markdown(creatures_by_region):
    """Generate markdown bestiary document."""
    md = []

    # Title and intro
    md.append("# Vystia Creatures Bestiary")
    md.append("")
    md.append("Complete guide to all creatures in the Vystia custom content expansion.")
    md.append("")
    md.append(f"**Total Creatures:** {sum(len(creatures) for creatures in creatures_by_region.values())}")
    md.append("")
    md.append("---")
    md.append("")

    # Table of Contents
    md.append("## Table of Contents")
    md.append("")
    for region in creatures_by_region.keys():
        md.append(f"- [{region}](#{region.lower().replace(' ', '-')})")
    md.append("")
    md.append("---")
    md.append("")

    # Process each region
    for region, creatures in creatures_by_region.items():
        md.append(f"## {region}")
        md.append("")
        md.append(f"**Count:** {len(creatures)} creatures")
        md.append("")

        # Sort creatures by name
        creatures.sort(key=lambda x: x['name'] or x['class_name'])

        for creature in creatures:
            # Creature header
            name = creature['name'] or creature['class_name']
            if creature['title']:
                md.append(f"### {name}, {creature['title']}")
            else:
                md.append(f"### {name}")
            md.append("")

            # Basic info table
            md.append("| Attribute | Value |")
            md.append("|-----------|-------|")
            md.append(f"| Class Name | `{creature['class_name']}` |")
            if creature['name']:
                md.append(f"| In-Game Name | {creature['name']} |")
            md.append(f"| Tameable | {creature['tamable']} |")
            if creature['taming_skill']:
                md.append(f"| Taming Skill Required | {creature['taming_skill']} |")
            md.append(f"| Fame | {creature['fame']} |")
            md.append(f"| Karma | {creature['karma']} |")
            md.append("")

            # Stats table
            md.append("**Stats:**")
            md.append("")
            md.append("| Stat | Value |")
            md.append("|------|-------|")
            md.append(f"| Strength | {creature['str']} |")
            md.append(f"| Dexterity | {creature['dex']} |")
            md.append(f"| Intelligence | {creature['int']} |")
            md.append(f"| Hit Points | {creature['hits']} |")
            md.append(f"| Damage | {creature['damage']} |")
            md.append("")

            # Resistances table
            md.append("**Resistances:**")
            md.append("")
            md.append("| Type | Value |")
            md.append("|------|-------|")
            md.append(f"| Physical | {creature['resist_phys']} |")
            md.append(f"| Fire | {creature['resist_fire']} |")
            md.append(f"| Cold | {creature['resist_cold']} |")
            md.append(f"| Poison | {creature['resist_poison']} |")
            md.append(f"| Energy | {creature['resist_energy']} |")
            md.append("")

            # Skills
            if creature['skills']:
                md.append("**Skills:**")
                md.append("")
                for skill in creature['skills']:
                    md.append(f"- {skill}")
                md.append("")

            # Special Abilities
            if creature['special_abilities']:
                md.append("**Special Abilities:**")
                md.append("")
                for ability in creature['special_abilities']:
                    md.append(f"- {ability}")
                md.append("")

            # Loot
            if creature['loot']:
                md.append("**Notable Loot:**")
                md.append("")
                for item in creature['loot'][:10]:
                    md.append(f"- {item}")
                md.append("")

            # Notes
            if creature['notes']:
                md.append("**Notes:**")
                md.append("")
                md.append(f"```")
                md.append(creature['notes'])
                md.append(f"```")
                md.append("")

            md.append("---")
            md.append("")

    return '\n'.join(md)

def main():
    # Base path to Vystia creatures
    base_path = Path(r"C:\DevEnv\GIT\UO\ServUO\Scripts\Mobiles\Vystia")

    # Region mappings
    regions = {
        'Bosses': 'Bosses',
        'Frosthold': 'Frosthold',
        'Emberlands': 'Emberlands',
        'Desert': 'Desert',
        'Shadowfen': 'Shadowfen',
        'Verdantpeak': 'Verdantpeak',
        'CrystalBarrens': 'Crystal Barrens',
        'Ironclad': 'Ironclad',
        'Skyreach': 'Skyreach',
        'Underwater': 'Underwater',
        'ShadowVoid': 'Shadow Void',
        'Misc': 'Miscellaneous',
        'Races': 'Races (NPCs)'
    }

    creatures_by_region = defaultdict(list)

    # Process each region
    for folder, display_name in regions.items():
        folder_path = base_path / folder
        if not folder_path.exists():
            print(f"Warning: {folder_path} does not exist")
            continue

        print(f"Processing {display_name}...")

        # Process all .cs files in the folder
        for cs_file in folder_path.glob('*.cs'):
            try:
                info = extract_creature_info(cs_file)
                if info['class_name']:
                    creatures_by_region[display_name].append(info)
                    print(f"  - {info['class_name']}")
            except Exception as e:
                print(f"  ERROR processing {cs_file.name}: {e}")

    # Generate markdown
    print("\nGenerating bestiary document...")
    markdown = generate_markdown(creatures_by_region)

    # Write to file
    output_path = Path(r"C:\DevEnv\GIT\UO\Vystia\docs\VYSTIA_CREATURES_BESTIARY.md")
    output_path.parent.mkdir(parents=True, exist_ok=True)

    with open(output_path, 'w', encoding='utf-8') as f:
        f.write(markdown)

    print(f"\nBestiary written to: {output_path}")
    print(f"Total creatures documented: {sum(len(creatures) for creatures in creatures_by_region.values())}")

if __name__ == '__main__':
    main()
