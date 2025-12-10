#!/usr/bin/env python3
"""
Generate Ability Registrations for Vystia Class System v2.0

This script parses the magic school markdown files and generates C# code
that registers abilities with the AbilityRegistry system.

Usage:
    python generate_ability_registrations.py [--school SCHOOL] [--all] [--output FILE]

Examples:
    python generate_ability_registrations.py --all
    python generate_ability_registrations.py --school Ice
    python generate_ability_registrations.py --school Ice --output IceAbilities.cs
"""

import os
import re
import argparse
from dataclasses import dataclass, field
from typing import List, Dict, Optional
from pathlib import Path

# Paths
SCRIPT_DIR = Path(__file__).parent
MAGIC_DIR = SCRIPT_DIR.parent / "Magic"
OUTPUT_DIR = SCRIPT_DIR.parent.parent / "ServUO" / "Scripts" / "Custom" / "VystiaClasses" / "Abilities" / "Generated"

# School mappings
SCHOOL_TO_FILE = {
    "Ice": "IceMagic.md",
    "Nature": "NatureMagic.md",
    "Hex": "HexMagic.md",
    "Elemental": "ElementalMagic.md",
    "Dark": "DarkMagic.md",
    "Divination": "DivinationMagic.md",
    "Necromancy": "Necromancy.md",
    "Summoning": "SummoningMagic.md",
    "Shamanic": "ShamanicMagic.md",
    "Bardic": "BardicMagic.md",
    "Enchanting": "EnchantingMagic.md",
    "Illusion": "IllusionMagic.md",
}

# ID ranges per school
SCHOOL_ID_RANGES = {
    "Ice": (1000, 1031),
    "Nature": (1032, 1063),
    "Hex": (1064, 1095),
    "Elemental": (1096, 1127),
    "Dark": (1128, 1159),
    "Divination": (1160, 1191),
    "Necromancy": (1192, 1223),
    "Summoning": (1224, 1255),
    "Shamanic": (1256, 1287),
    "Bardic": (1288, 1319),
    "Enchanting": (1320, 1351),
    "Illusion": (1352, 1383),
}

# Circle to mana cost mapping
CIRCLE_MANA = {
    1: 4,
    2: 6,
    3: 9,
    4: 11,
    5: 14,
    6: 20,
    7: 40,
    8: 50,
}

@dataclass
class SpellData:
    """Parsed spell data from markdown"""
    name: str
    circle: int
    spell_index: int  # 0-31 within the school
    mana: int
    range_tiles: int = 12
    min_damage: int = 0
    max_damage: int = 0
    damage_type: str = "Cold"  # Cold, Fire, Poison, Energy, Physical, Shadow, Holy, etc.
    duration_seconds: int = 0
    area_radius: int = 0
    cooldown_seconds: int = 0
    is_buff: bool = False
    is_heal: bool = False
    is_dot: bool = False
    is_aoe: bool = False
    is_summon: bool = False
    buff_type: str = ""
    buff_value: int = 0
    slow_chance: int = 0
    slow_dex: int = 0
    slow_duration: int = 0
    special_effects: List[str] = field(default_factory=list)

def parse_spell_section(text: str, circle: int, spell_index_in_circle: int) -> Optional[SpellData]:
    """Parse a single spell section from markdown"""

    # Extract spell name from header - try multiple formats
    # Format 1: #### 1. Frost Touch (detailed format)
    name_match = re.search(r'####\s*\d+\.\s*([^✅\n]+)', text)
    if not name_match:
        # Format 2: 1. **Shadow Bolt** - description (simple format)
        name_match = re.search(r'^\d+\.\s*\*\*([^*]+)\*\*', text, re.MULTILINE)
    if not name_match:
        return None
    name = name_match.group(1).strip()

    # Calculate absolute spell index (0-31)
    spells_per_circle = 4
    spell_index = (circle - 1) * spells_per_circle + spell_index_in_circle

    # Extract mana
    mana_match = re.search(r'\*\*Mana:\*\*\s*(\d+)', text)
    mana = int(mana_match.group(1)) if mana_match else CIRCLE_MANA.get(circle, 4)

    # Extract range
    range_match = re.search(r'\*\*Range:\*\*\s*(\d+)\s*tile', text, re.IGNORECASE)
    range_tiles = int(range_match.group(1)) if range_match else 12

    # Check for melee range
    if re.search(r'\*\*Range:\*\*\s*Melee', text, re.IGNORECASE):
        range_tiles = 1

    # Extract damage
    damage_match = re.search(r'\*\*Damage:\*\*\s*(\d+)-(\d+)\s*(cold|fire|poison|energy|physical|shadow|holy|arcane|nature)?', text, re.IGNORECASE)
    if damage_match:
        min_damage = int(damage_match.group(1))
        max_damage = int(damage_match.group(2))
        damage_type = damage_match.group(3).capitalize() if damage_match.group(3) else "Cold"
    else:
        min_damage = 0
        max_damage = 0
        damage_type = "Cold"

    # Extract duration
    duration_match = re.search(r'\*\*Duration:\*\*\s*(\d+)\s*second', text, re.IGNORECASE)
    duration_seconds = int(duration_match.group(1)) if duration_match else 0

    # Extract area
    area_match = re.search(r'\*\*Area:\*\*\s*(\d+)\s*tile\s*radius', text, re.IGNORECASE)
    if not area_match:
        area_match = re.search(r'(\d+)\s*tile\s*radius', text, re.IGNORECASE)
    area_radius = int(area_match.group(1)) if area_match else 0

    # Extract cooldown
    cooldown_match = re.search(r'\*\*Cooldown:\*\*\s*(\d+)\s*second', text, re.IGNORECASE)
    cooldown_seconds = int(cooldown_match.group(1)) if cooldown_match else 0

    # Determine spell type
    is_buff = any(word in text.lower() for word in ['buff', 'resistance', 'protection', 'shield', 'armor', 'ward'])
    is_heal = any(word in text.lower() for word in ['heal', 'restore', 'regenerate', 'mend'])
    is_dot = any(word in text.lower() for word in ['damage over time', 'dot', 'per second', 'ticking'])
    is_aoe = area_radius > 0 or any(word in text.lower() for word in ['area', 'aoe', 'radius', 'all enemies'])
    is_summon = any(word in text.lower() for word in ['summon', 'conjure', 'create', 'elemental'])

    # Extract buff details
    buff_value = 0
    buff_type = ""
    if is_buff:
        buff_match = re.search(r'\+(\d+)\s*(Cold|Fire|Physical|Energy|Poison)?\s*Resist', text, re.IGNORECASE)
        if buff_match:
            buff_value = int(buff_match.group(1))
            buff_type = f"{buff_match.group(2)}Resist" if buff_match.group(2) else "AllResist"

    # Extract slow effect
    slow_match = re.search(r'(\d+)%\s*chance\s*to\s*slow.*?-(\d+)\s*DEX.*?(\d+)s', text, re.IGNORECASE)
    if slow_match:
        slow_chance = int(slow_match.group(1))
        slow_dex = int(slow_match.group(2))
        slow_duration = int(slow_match.group(3))
    else:
        slow_chance = slow_dex = slow_duration = 0

    return SpellData(
        name=name,
        circle=circle,
        spell_index=spell_index,
        mana=mana,
        range_tiles=range_tiles,
        min_damage=min_damage,
        max_damage=max_damage,
        damage_type=damage_type,
        duration_seconds=duration_seconds,
        area_radius=area_radius,
        cooldown_seconds=cooldown_seconds,
        is_buff=is_buff,
        is_heal=is_heal,
        is_dot=is_dot,
        is_aoe=is_aoe,
        is_summon=is_summon,
        buff_type=buff_type,
        buff_value=buff_value,
        slow_chance=slow_chance,
        slow_dex=slow_dex,
        slow_duration=slow_duration,
    )

def parse_magic_school(school: str) -> List[SpellData]:
    """Parse all spells from a magic school markdown file"""

    filename = SCHOOL_TO_FILE.get(school)
    if not filename:
        print(f"Unknown school: {school}")
        return []

    filepath = MAGIC_DIR / filename
    if not filepath.exists():
        print(f"File not found: {filepath}")
        return []

    with open(filepath, 'r', encoding='utf-8') as f:
        content = f.read()

    spells = []

    # Find circle sections
    circle_pattern = r'###\s*Circle\s*(\d+)'
    circle_matches = list(re.finditer(circle_pattern, content))

    for i, circle_match in enumerate(circle_matches):
        circle = int(circle_match.group(1))

        # Get the text between this circle and the next (or end)
        start = circle_match.end()
        end = circle_matches[i + 1].start() if i + 1 < len(circle_matches) else len(content)
        circle_text = content[start:end]

        # Try detailed format first (#### N. Name)
        spell_pattern = r'####\s*\d+\.'
        spell_matches = list(re.finditer(spell_pattern, circle_text))

        if spell_matches:
            # Detailed format
            for j, spell_match in enumerate(spell_matches):
                spell_start = spell_match.start()
                spell_end = spell_matches[j + 1].start() if j + 1 < len(spell_matches) else len(circle_text)
                spell_text = circle_text[spell_start:spell_end]

                spell = parse_spell_section(spell_text, circle, j)
                if spell:
                    spells.append(spell)
        else:
            # Try simple format (N. **Name** - description)
            simple_pattern = r'^\d+\.\s*\*\*[^*]+\*\*'
            simple_matches = list(re.finditer(simple_pattern, circle_text, re.MULTILINE))

            for j, spell_match in enumerate(simple_matches):
                spell_start = spell_match.start()
                spell_end = simple_matches[j + 1].start() if j + 1 < len(simple_matches) else len(circle_text)
                spell_text = circle_text[spell_start:spell_end]

                spell = parse_simple_spell(spell_text, circle, j, school)
                if spell:
                    spells.append(spell)

    return spells

def parse_simple_spell(text: str, circle: int, spell_index_in_circle: int, school: str) -> Optional[SpellData]:
    """Parse a simple format spell (N. **Name** - description)"""

    # Extract name
    name_match = re.search(r'^\d+\.\s*\*\*([^*]+)\*\*', text, re.MULTILINE)
    if not name_match:
        return None
    name = name_match.group(1).strip()

    # Calculate absolute spell index (0-31)
    spells_per_circle = 4
    spell_index = (circle - 1) * spells_per_circle + spell_index_in_circle

    # Extract info from the description line
    description = text[name_match.end():].strip()

    # Try to extract damage from description (e.g., "6-12 shadow damage")
    damage_match = re.search(r'(\d+)-(\d+)\s*(shadow|fire|cold|poison|energy|physical|holy|arcane|nature)?\s*damage', description, re.IGNORECASE)
    if damage_match:
        min_damage = int(damage_match.group(1))
        max_damage = int(damage_match.group(2))
        damage_type = damage_match.group(3).capitalize() if damage_match.group(3) else get_school_damage_type(school)
    else:
        min_damage = 0
        max_damage = 0
        damage_type = get_school_damage_type(school)

    # Extract duration
    duration_match = re.search(r'(\d+)s|(\d+)\s*seconds?', description, re.IGNORECASE)
    duration_seconds = int(duration_match.group(1) or duration_match.group(2)) if duration_match else 0

    # Extract radius
    area_match = re.search(r'(\d+)\s*tile\s*(radius|AoE|area)', description, re.IGNORECASE)
    area_radius = int(area_match.group(1)) if area_match else 0

    # Determine spell type from description
    is_buff = any(word in description.lower() for word in ['resist', 'armor', 'shield', 'protection', 'buff', '+'])
    is_heal = any(word in description.lower() for word in ['heal', 'restore'])
    is_dot = any(word in description.lower() for word in ['/tick', 'dot', 'over time'])
    is_aoe = area_radius > 0 or any(word in description.lower() for word in ['aoe', 'radius', 'area', 'wave', 'nova'])
    is_summon = any(word in description.lower() for word in ['summon', 'demon', 'elemental', 'creature'])

    return SpellData(
        name=name,
        circle=circle,
        spell_index=spell_index,
        mana=CIRCLE_MANA.get(circle, 4),
        range_tiles=12,
        min_damage=min_damage,
        max_damage=max_damage,
        damage_type=damage_type,
        duration_seconds=duration_seconds,
        area_radius=area_radius,
        cooldown_seconds=0,
        is_buff=is_buff,
        is_heal=is_heal,
        is_dot=is_dot,
        is_aoe=is_aoe,
        is_summon=is_summon,
    )

def get_school_damage_type(school: str) -> str:
    """Get the primary damage type for a school"""
    school_damage = {
        "Ice": "Cold",
        "Nature": "Nature",
        "Hex": "Poison",
        "Elemental": "Fire",
        "Dark": "Shadow",
        "Divination": "Energy",
        "Necromancy": "Shadow",
        "Summoning": "Arcane",
        "Shamanic": "Nature",
        "Bardic": "Energy",
        "Enchanting": "Arcane",
        "Illusion": "Energy",
    }
    return school_damage.get(school, "Arcane")

def damage_type_to_enum(damage_type: str) -> str:
    """Convert damage type string to VystiaDamageType enum value"""
    mapping = {
        "Cold": "VystiaDamageType.Cold",
        "Fire": "VystiaDamageType.Fire",
        "Poison": "VystiaDamageType.Poison",
        "Energy": "VystiaDamageType.Energy",
        "Physical": "VystiaDamageType.Physical",
        "Shadow": "VystiaDamageType.Shadow",
        "Holy": "VystiaDamageType.Holy",
        "Arcane": "VystiaDamageType.Arcane",
        "Nature": "VystiaDamageType.Nature",
    }
    return mapping.get(damage_type, "VystiaDamageType.Cold")

def generate_ability_code(spell: SpellData, school: str) -> str:
    """Generate C# code for a single ability registration"""

    id_start, _ = SCHOOL_ID_RANGES.get(school, (1000, 1031))
    ability_id = id_start + spell.spell_index

    school_enum = f"AbilitySchool.{school}"
    damage_type_enum = damage_type_to_enum(spell.damage_type)

    # Determine which factory method or builder to use
    lines = []

    if spell.is_heal:
        # Heal spell
        lines.append(f"            // {spell.name} (Circle {spell.circle})")
        lines.append(f"            AbilityRegistry.RegisterAbility(AbilityDefinition.CreateHealSpell(")
        lines.append(f"                {ability_id}, \"{spell.name}\", {school_enum}, {spell.circle}, {spell.min_damage}, {spell.max_damage}, {spell.mana})")
        if spell.cooldown_seconds > 0:
            lines.append(f"                .WithCooldown({spell.cooldown_seconds})")
        lines.append(f"                .WithImpactEffect(0x376A, 0x1F2, 0x481));")

    elif spell.is_buff and spell.buff_value > 0:
        # Buff spell
        lines.append(f"            // {spell.name} (Circle {spell.circle})")
        buff_type = f"VystiaBuffType.{spell.buff_type}" if spell.buff_type else "VystiaBuffType.ColdResist"
        lines.append(f"            AbilityRegistry.RegisterAbility(AbilityDefinition.CreateBuffSpell(")
        lines.append(f"                {ability_id}, \"{spell.name}\", {school_enum}, {spell.circle}, {buff_type}, {spell.buff_value}, {spell.duration_seconds}, {spell.mana}));")

    elif spell.is_aoe and spell.min_damage > 0:
        # AoE damage spell
        lines.append(f"            // {spell.name} (Circle {spell.circle})")
        lines.append(f"            AbilityRegistry.RegisterAbility(AbilityDefinition.CreateAoESpell(")
        lines.append(f"                {ability_id}, \"{spell.name}\", {school_enum}, {spell.circle}, {spell.min_damage}, {spell.max_damage}, {damage_type_enum}, {spell.area_radius}, {spell.mana})")
        if spell.cooldown_seconds > 0:
            lines.append(f"                .WithCooldown({spell.cooldown_seconds})")
        if spell.slow_chance > 0:
            lines.append(f"                .WithCC(CCType.Slow, {spell.slow_duration})")
        lines.append(f"                .WithImpactEffect(0x36D4, 0x1E5, 0x481));")

    elif spell.min_damage > 0:
        # Single target damage spell
        lines.append(f"            // {spell.name} (Circle {spell.circle})")
        lines.append(f"            AbilityRegistry.RegisterAbility(AbilityDefinition.CreateDamageSpell(")
        lines.append(f"                {ability_id}, \"{spell.name}\", {school_enum}, {spell.circle}, {spell.min_damage}, {spell.max_damage}, {damage_type_enum}, {spell.mana})")
        if spell.slow_chance > 0:
            lines.append(f"                .WithStack(StackType.Chill, 1, {spell.slow_duration})")
        if spell.cooldown_seconds > 0:
            lines.append(f"                .WithCooldown({spell.cooldown_seconds})")
        lines.append(f"                .WithImpactEffect(0x36D4, 0x1E5, 0x481));")

    else:
        # Generic spell (utility, etc.)
        lines.append(f"            // {spell.name} (Circle {spell.circle})")
        lines.append(f"            AbilityRegistry.RegisterAbility(new AbilityDefinition()")
        lines.append(f"                .WithId({ability_id})")
        lines.append(f"                .WithName(\"{spell.name}\")")
        lines.append(f"                .InSchool({school_enum})")
        lines.append(f"                .InCircle({spell.circle})")
        lines.append(f"                .WithManaCost({spell.mana})")
        lines.append(f"                .Targeting(AbilityTargetType.{('SingleTarget' if spell.range_tiles > 1 else 'Self')}, {spell.range_tiles})")
        if spell.cooldown_seconds > 0:
            lines.append(f"                .WithCooldown({spell.cooldown_seconds})")
        # For duration-based utility spells, add a buff effect
        if spell.duration_seconds > 0:
            lines.append(f"                .WithBuff(VystiaBuffType.AllStatsBuff, {spell.duration_seconds}, 5)")
        lines.append(f"                .WithImpactEffect(0x36D4, 0x1E5, 0x481));")

    return "\n".join(lines)

def generate_school_file(school: str, spells: List[SpellData]) -> str:
    """Generate a complete C# file for a school's abilities"""

    lines = [
        "// Auto-generated by generate_ability_registrations.py",
        "// Do not edit manually - regenerate using the script",
        "",
        "using System;",
        "using Server;",
        "using Server.Custom.VystiaClasses.Systems;",
        "",
        "namespace Server.Custom.VystiaClasses.Abilities",
        "{",
        f"    public static class {school}Abilities",
        "    {",
        "        public static void RegisterAll()",
        "        {",
    ]

    for spell in spells:
        lines.append(generate_ability_code(spell, school))
        lines.append("")

    lines.extend([
        "        }",
        "    }",
        "}",
    ])

    return "\n".join(lines)

def generate_initializer_file(schools: List[str]) -> str:
    """Generate the main initializer that calls all school registration methods"""

    lines = [
        "// Auto-generated by generate_ability_registrations.py",
        "// Do not edit manually - regenerate using the script",
        "",
        "using System;",
        "using Server;",
        "",
        "namespace Server.Custom.VystiaClasses.Abilities",
        "{",
        "    public static class GeneratedAbilityInitializer",
        "    {",
        "        public static void RegisterAllAbilities()",
        "        {",
        "            Console.WriteLine(\"[Vystia] Registering generated abilities...\");",
        "",
    ]

    for school in schools:
        lines.append(f"            {school}Abilities.RegisterAll();")

    lines.extend([
        "",
        f"            Console.WriteLine(\"[Vystia] Registered abilities from {len(schools)} schools\");",
        "        }",
        "    }",
        "}",
    ])

    return "\n".join(lines)

def main():
    parser = argparse.ArgumentParser(description='Generate ability registrations from markdown')
    parser.add_argument('--school', help='Generate for a specific school')
    parser.add_argument('--all', action='store_true', help='Generate for all schools')
    parser.add_argument('--output', help='Output directory (default: Generated/)')
    parser.add_argument('--dry-run', action='store_true', help='Print output instead of writing files')
    args = parser.parse_args()

    output_dir = Path(args.output) if args.output else OUTPUT_DIR

    if args.all:
        schools = list(SCHOOL_TO_FILE.keys())
    elif args.school:
        schools = [args.school]
    else:
        print("Please specify --school SCHOOL or --all")
        return

    generated_schools = []
    total_spells = 0

    for school in schools:
        print(f"\nParsing {school} magic...")
        spells = parse_magic_school(school)

        if not spells:
            print(f"  No spells found for {school}")
            continue

        print(f"  Found {len(spells)} spells")
        total_spells += len(spells)

        code = generate_school_file(school, spells)

        if args.dry_run:
            print(f"\n=== {school}Abilities.cs ===")
            print(code[:2000] + "..." if len(code) > 2000 else code)
        else:
            output_dir.mkdir(parents=True, exist_ok=True)
            output_file = output_dir / f"{school}Abilities.cs"
            with open(output_file, 'w', encoding='utf-8') as f:
                f.write(code)
            print(f"  Written to {output_file}")

        generated_schools.append(school)

    # Generate initializer
    if generated_schools:
        init_code = generate_initializer_file(generated_schools)

        if args.dry_run:
            print(f"\n=== GeneratedAbilityInitializer.cs ===")
            print(init_code)
        else:
            init_file = output_dir / "GeneratedAbilityInitializer.cs"
            with open(init_file, 'w', encoding='utf-8') as f:
                f.write(init_code)
            print(f"\nWritten initializer to {init_file}")

    print(f"\n=== Summary ===")
    print(f"Schools processed: {len(generated_schools)}")
    print(f"Total spells parsed: {total_spells}")
    if not args.dry_run:
        print(f"Output directory: {output_dir}")

if __name__ == '__main__':
    main()
