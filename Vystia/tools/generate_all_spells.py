#!/usr/bin/env python3
"""
Vystia Spell Generator
Generates all 352 pending spell implementations from documentation.

This script:
1. Parses spell documentation from Vystia/Magic/*.md files
2. Generates C# spell files using Ice Magic template
3. Creates spell directories for each school
4. Generates spell initializer registration code
5. Outputs 352 spell files ready to compile

Usage:
    python generate_all_spells.py [options]

Options:
    --school <name>    Generate only specified school (e.g., "Nature")
    --dry-run          Show what would be generated without creating files
    --output <path>    Output directory (default: ServUO/Scripts/Custom/VystiaClasses/Spells/)
"""

import os
import re
import sys
from pathlib import Path
from dataclasses import dataclass
from typing import List, Dict, Tuple

@dataclass
class Spell:
    """Represents a single spell"""
    name: str
    class_name: str  # PascalCase for class name
    circle: int
    mana: int
    reagents: List[str]
    cast_time: float
    range_tiles: int
    duration: int  # seconds, 0 if instant
    damage_min: int
    damage_max: int
    damage_type: str  # "cold", "poison", "fire", "energy", "physical"
    effect_description: str
    visual_effect: str
    sound_effect: str
    is_aoe: bool
    aoe_radius: int
    is_heal: bool
    heal_min: int
    heal_max: int
    is_buff: bool
    buff_description: str
    is_debuff: bool
    debuff_description: str
    spell_id: int  # Unique spell ID (999-1382)
    words_of_power: str  # Latin incantation

@dataclass
class MagicSchool:
    """Represents a magic school with all its spells"""
    name: str
    class_name: str  # e.g., "IceMage", "Druid"
    folder_name: str  # e.g., "IceMage", "Druid"
    namespace: str  # e.g., "IceMage", "Nature"
    region: str
    theme: str
    hue: str
    spell_id_start: int  # Server-side ID (e.g., 999 for Ice, 1031 for Nature)
    spell_id_end: int
    spells: List[Spell]

# Magic school configuration
MAGIC_SCHOOLS = {
    "Nature": {
        "class_name": "Druid",
        "folder_name": "Druid",
        "namespace": "Nature",
        "region": "Verdantpeak",
        "theme": "Shapeshifting, healing, poison, plant control",
        "hue": "0x7D6",
        "spell_id_start": 1031,
        "spell_id_end": 1062,
        "doc_file": "NatureMagic.md"
    },
    "Hex": {
        "class_name": "Witch",
        "folder_name": "Witch",
        "namespace": "Hex",
        "region": "Shadowfen",
        "theme": "Curses, debuffs, life drain, hexes",
        "hue": "0x81D",
        "spell_id_start": 1063,
        "spell_id_end": 1094,
        "doc_file": "HexMagic.md"
    },
    "Elemental": {
        "class_name": "Sorcerer",
        "folder_name": "Sorcerer",
        "namespace": "Elemental",
        "region": "Emberlands",
        "theme": "Fire, lava, explosive damage",
        "hue": "0x54E",
        "spell_id_start": 1095,
        "spell_id_end": 1126,
        "doc_file": "ElementalMagic.md"
    },
    "Dark": {
        "class_name": "Warlock",
        "folder_name": "Warlock",
        "namespace": "Dark",
        "region": "ShadowVoid",
        "theme": "Demons, shadow damage, fear",
        "hue": "0x455",
        "spell_id_start": 1127,
        "spell_id_end": 1158,
        "doc_file": "DarkMagic.md"
    },
    "Divination": {
        "class_name": "Oracle",
        "folder_name": "Oracle",
        "namespace": "Divination",
        "region": "Crystal Barrens",
        "theme": "Foresight, energy, time manipulation",
        "hue": "0x482",
        "spell_id_start": 1159,
        "spell_id_end": 1190,
        "doc_file": "DivinationMagic.md"
    },
    "Necromancy": {
        "class_name": "Necromancer",
        "folder_name": "Necromancer",
        "namespace": "Necromancy",
        "region": "ShadowVoid",
        "theme": "Undead, death, decay",
        "hue": "0x455",
        "spell_id_start": 1191,
        "spell_id_end": 1222,
        "doc_file": "Necromancy.md"
    },
    "Summoning": {
        "class_name": "Summoner",
        "folder_name": "Summoner",
        "namespace": "Summoning",
        "region": "Underwater",
        "theme": "Creature summoning, binding",
        "hue": "0x555",
        "spell_id_start": 1223,
        "spell_id_end": 1254,
        "doc_file": "SummoningMagic.md"
    },
    "Shamanic": {
        "class_name": "Shaman",
        "folder_name": "Shaman",
        "namespace": "Shamanic",
        "region": "Skyreach/Wilderlands",
        "theme": "Elemental totems, spirits",
        "hue": "0x501",
        "spell_id_start": 1255,
        "spell_id_end": 1286,
        "doc_file": "ShamanicMagic.md"
    },
    "Bardic": {
        "class_name": "Bard",
        "folder_name": "Bard",
        "namespace": "Bardic",
        "region": "Multi-regional",
        "theme": "Songs, buffs, channeled magic",
        "hue": "0x8A5",
        "spell_id_start": 1287,
        "spell_id_end": 1318,
        "doc_file": "BardicMagic.md"
    },
    "Enchanting": {
        "class_name": "Enchanter",
        "folder_name": "Enchanter",
        "namespace": "Enchanting",
        "region": "Multi-regional",
        "theme": "Item enhancement, buffs",
        "hue": "0x8FD",
        "spell_id_start": 1319,
        "spell_id_end": 1350,
        "doc_file": "EnchantingMagic.md"
    },
    "Illusion": {
        "class_name": "Illusionist",
        "folder_name": "Illusionist",
        "namespace": "Illusion",
        "region": "Multi-regional",
        "theme": "Illusions, deception, mind control",
        "hue": "0x47E",
        "spell_id_start": 1351,
        "spell_id_end": 1382,
        "doc_file": "IllusionMagic.md"
    }
}

def parse_spell_documentation(doc_path: Path, school_name: str) -> List[Spell]:
    """
    Parse spell documentation markdown file to extract spell data.

    Returns list of Spell objects.
    """
    print(f"Parsing {doc_path}...")

    spells = []
    with open(doc_path, 'r', encoding='utf-8') as f:
        content = f.read()

    # Extract spell sections (### Circle X)
    circle_pattern = r'### Circle (\d+).*?(?=### Circle|\Z)'
    circles = re.findall(circle_pattern, content, re.DOTALL)

    spell_id_offset = MAGIC_SCHOOLS[school_name]["spell_id_start"]
    current_spell_id = spell_id_offset

    for circle_num in range(1, 9):  # Circles 1-8
        # Find spells in this circle
        circle_section_pattern = f'### Circle {circle_num}.*?(?=### Circle|\\Z)'
        circle_match = re.search(circle_section_pattern, content, re.DOTALL)

        if not circle_match:
            print(f"  Warning: Circle {circle_num} not found in {doc_path}")
            continue

        circle_content = circle_match.group(0)

        # Extract individual spells - try both formats
        # Format 1: #### 1. Spell Name (detailed format)
        spell_pattern_detailed = r'####\s+\d+\.\s+([^\n]+)'
        spell_names_detailed = re.findall(spell_pattern_detailed, circle_content)

        # Format 2: 1. **Spell Name** - description (condensed format)
        spell_pattern_condensed = r'^\d+\.\s+\*\*([^*]+)\*\*'
        spell_names_condensed = re.findall(spell_pattern_condensed, circle_content, re.MULTILINE)

        # Use whichever format found spells
        spell_names = spell_names_detailed if spell_names_detailed else spell_names_condensed

        # Limit to first 4 spells per circle (to match 32 spell ID allocation)
        spell_names = spell_names[:4]

        for spell_name in spell_names:
            spell = parse_single_spell(circle_content, spell_name.strip(), circle_num, current_spell_id, school_name)
            if spell:
                spells.append(spell)
                current_spell_id += 1

    print(f"  Parsed {len(spells)} spells from {doc_path}")
    return spells

def parse_single_spell(content: str, spell_name: str, circle: int, spell_id: int, school_name: str) -> Spell:
    """Parse a single spell from documentation content."""

    # Try detailed format first (#### 1. Spell Name with multi-line details)
    spell_section_pattern = f'####\\s+\\d+\\.\\s+{re.escape(spell_name)}.*?(?=####|###|\\Z)'
    spell_match = re.search(spell_section_pattern, content, re.DOTALL)

    # If not found, try condensed format (1. **Spell Name** - details on same line)
    if not spell_match:
        # Look for: 1. **Spell Name** - description
        condensed_pattern = f'^\\d+\\.\\s+\\*\\*{re.escape(spell_name)}\\*\\*.*$'
        spell_match = re.search(condensed_pattern, content, re.MULTILINE)

    if not spell_match:
        return None

    spell_content = spell_match.group(0)

    # Extract spell data
    mana = extract_value(spell_content, r'\*\*Mana:\*\*\s*(\d+)', int) or (circle * 4)
    reagents_str = extract_value(spell_content, r'\*\*Reagents:\*\*\s*([^\n]+)', str) or ""
    reagents = [r.strip() for r in reagents_str.split(',') if r.strip()]

    cast_time = extract_value(spell_content, r'\*\*Cast Time:\*\*\s*([\d.]+)', float) or 1.0
    range_tiles = extract_value(spell_content, r'\*\*Range:\*\*\s*(\d+)', int) or 10
    duration = extract_value(spell_content, r'\*\*Duration:\*\*\s*(\d+)', int) or 0

    # Extract damage
    damage_match = re.search(r'(\d+)-(\d+)\s+damage', spell_content)
    damage_min = int(damage_match.group(1)) if damage_match else 0
    damage_max = int(damage_match.group(2)) if damage_match else 0

    # Determine damage type from school
    damage_type = get_damage_type(school_name, spell_content)

    # Extract visual/sound effects
    visual_effect = extract_value(spell_content, r'\*\*Visual:\*\*\s*([^\n]+)', str) or "0x376A"
    sound_effect_raw = extract_value(spell_content, r'\*\*Sound:\*\*\s*(0x[0-9A-Fa-f]+)', str)
    # Default to 0x1F2 if no valid hex sound found
    sound_effect = sound_effect_raw if sound_effect_raw else "0x1F2"

    # Detect spell type
    is_aoe = 'AoE' in spell_content or 'radius' in spell_content.lower()
    aoe_radius = extract_value(spell_content, r'(\d+)\s+tile radius', int) or 0

    is_heal = 'heal' in spell_content.lower()
    heal_match = re.search(r'heals?\s+(\d+)-(\d+)', spell_content, re.IGNORECASE)
    heal_min = int(heal_match.group(1)) if heal_match else 0
    heal_max = int(heal_match.group(2)) if heal_match else 0

    is_buff = any(word in spell_content.lower() for word in ['buff', '+', 'increase', 'enhance'])
    is_debuff = any(word in spell_content.lower() for word in ['debuff', 'curse', 'reduce', 'weaken'])

    # Extract effect description
    effect_match = re.search(r'\*\*Effect:\*\*\s*([^\n]+)', spell_content)
    effect_description = effect_match.group(1) if effect_match else spell_name

    # Generate class name and words of power
    # Remove invalid characters for C# class names
    base_class_name = spell_name.replace(" ", "").replace("'", "").replace("-", "")
    base_class_name = base_class_name.replace("(", "").replace(")", "").replace(":", "")
    base_class_name = base_class_name.replace("/", "").replace(",", "").replace(".", "")

    # Add school prefix to prevent naming conflicts between schools
    school_prefix = school_name.replace(" ", "")
    class_name = f"{school_prefix}{base_class_name}"
    words_of_power = generate_words_of_power(spell_name)

    return Spell(
        name=spell_name,
        class_name=class_name,
        circle=circle,
        mana=mana,
        reagents=reagents,
        cast_time=cast_time,
        range_tiles=range_tiles,
        duration=duration,
        damage_min=damage_min,
        damage_max=damage_max,
        damage_type=damage_type,
        effect_description=effect_description,
        visual_effect=visual_effect,
        sound_effect=sound_effect,
        is_aoe=is_aoe,
        aoe_radius=aoe_radius,
        is_heal=is_heal,
        heal_min=heal_min,
        heal_max=heal_max,
        is_buff=is_buff,
        buff_description="",
        is_debuff=is_debuff,
        debuff_description="",
        spell_id=spell_id,
        words_of_power=words_of_power
    )

def extract_value(content: str, pattern: str, value_type):
    """Extract value from content using regex pattern."""
    match = re.search(pattern, content, re.IGNORECASE)
    if match:
        try:
            return value_type(match.group(1))
        except:
            return None
    return None

def get_damage_type(school_name: str, spell_content: str) -> str:
    """Determine damage type based on school and spell description."""
    damage_types = {
        "Ice": "cold",
        "Nature": "poison",
        "Hex": "poison",
        "Elemental": "fire",
        "Dark": "energy",
        "Divination": "energy",
        "Necromancy": "cold",
        "Summoning": "physical",
        "Shamanic": "energy",
        "Bardic": "energy",
        "Enchanting": "energy",
        "Illusion": "energy"
    }

    # Check spell content for explicit damage type
    if 'cold' in spell_content.lower() or 'ice' in spell_content.lower() or 'frost' in spell_content.lower():
        return "cold"
    elif 'poison' in spell_content.lower():
        return "poison"
    elif 'fire' in spell_content.lower() or 'flame' in spell_content.lower():
        return "fire"
    elif 'energy' in spell_content.lower():
        return "energy"
    elif 'physical' in spell_content.lower():
        return "physical"

    return damage_types.get(school_name.split()[0], "energy")

def generate_words_of_power(spell_name: str) -> str:
    """Generate Latin-sounding words of power from spell name."""
    # Simple translation of common words
    translations = {
        "ice": "Glacius",
        "frost": "Frigus",
        "fire": "Ignis",
        "heal": "Sanitas",
        "poison": "Venenum",
        "shadow": "Umbra",
        "light": "Lux",
        "death": "Mors",
        "life": "Vita",
        "bolt": "Sagitta",
        "shield": "Scutum",
        "armor": "Armatura",
        "touch": "Tactus",
        "blast": "Eruptio"
    }

    words = spell_name.lower().split()
    latin_words = []

    for word in words:
        if word in translations:
            latin_words.append(translations[word])
        else:
            # Latinize the word
            if word.endswith('e'):
                latin_words.append(word.capitalize() + "us")
            else:
                latin_words.append(word.capitalize() + "um")

    return " ".join(latin_words)

def generate_spell_file(spell: Spell, school: MagicSchool, output_dir: Path) -> str:
    """Generate C# spell file content from template."""

    template = f'''using System;
using Server;
using Server.Targeting;
using Server.Mobiles;
using Server.Spells;
using Server.Items;

namespace Server.Spells.VystiaSpells.{school.namespace}
{{
    /// <summary>
    /// {spell.name} - {spell.effect_description}
    /// Circle: {spell.circle} ({spell.mana} mana)
    /// </summary>
    public class {spell.class_name}Spell : MagerySpell
    {{
        private static readonly SpellInfo m_Info = new SpellInfo(
            "{spell.name}", "{spell.words_of_power}",
            203,
            9041,
            false
        );

        public override SpellCircle Circle => SpellCircle.{get_circle_name(spell.circle)};

        public {spell.class_name}Spell(Mobile caster, Item scroll) : base(caster, scroll, m_Info)
        {{
        }}

        public override bool CheckCast()
        {{
            if (!base.CheckCast())
                return false;

            // TODO: Check for Vystia reagents
            // Example: if (!HasReagent(typeof(ReagentName), 1))
            //     return false;

            return true;
        }}

        public override void OnCast()
        {{
            Caster.Target = new InternalTarget(this);
        }}

        public void Target(IDamageable target)
        {{
            if (!Caster.CanSee(target))
            {{
                Caster.SendLocalizedMessage(500237); // Target can not be seen.
            }}
            else if (CheckHSequence(target))
            {{
                SpellHelper.Turn(Caster, target);

                // Visual effect
                {generate_visual_effect(spell)}

                // Sound effect
                {generate_sound_effect(spell)}

                // Spell effect
                {generate_spell_effect(spell)}
            }}

            FinishSequence();
        }}

        private class InternalTarget : Target
        {{
            private readonly {spell.class_name}Spell m_Owner;

            public InternalTarget({spell.class_name}Spell owner)
                : base({spell.range_tiles}, false, TargetFlags.Harmful)
            {{
                m_Owner = owner;
            }}

            protected override void OnTarget(Mobile from, object o)
            {{
                if (o is IDamageable)
                    m_Owner.Target((IDamageable)o);
            }}

            protected override void OnTargetFinish(Mobile from)
            {{
                m_Owner.FinishSequence();
            }}
        }}
    }}
}}
'''

    return template

def generate_visual_effect(spell: Spell) -> str:
    """Generate visual effect code."""
    if spell.is_aoe:
        return f'''Effects.SendLocationParticles(
                    EffectItem.Create(target.Location, target.Map, EffectItem.DefaultDuration),
                    0x376A, 9, 32, {MAGIC_SCHOOLS[spell.damage_type]["hue"] if spell.damage_type in MAGIC_SCHOOLS else "0x481"});'''
    else:
        return f'''target.FixedParticles(0x376A, 10, 15, 5038, {MAGIC_SCHOOLS[spell.damage_type]["hue"] if spell.damage_type in MAGIC_SCHOOLS else "0x481"}, 0, EffectLayer.Waist);'''

def generate_sound_effect(spell: Spell) -> str:
    """Generate sound effect code."""
    sound_hex = spell.sound_effect.replace("0x", "")
    return f'Caster.PlaySound(0x{sound_hex});'

def generate_spell_effect(spell: Spell) -> str:
    """Generate main spell effect code."""
    if spell.is_heal:
        return f'''// Heal target
                if (target is Mobile mobile)
                {{
                    double healAmount = Utility.RandomMinMax({spell.heal_min}, {spell.heal_max});
                    mobile.Heal((int)healAmount);
                    mobile.SendMessage("You have been healed!");
                }}'''

    elif spell.damage_min > 0:
        damage_code = get_damage_split(spell.damage_type)
        return f'''// Deal damage
                double damage = Utility.RandomMinMax({spell.damage_min}, {spell.damage_max});
                SpellHelper.Damage(this, target, damage, {damage_code});'''

    else:
        return f'''// TODO: Implement {spell.name} effect
                // {spell.effect_description}'''

def get_damage_split(damage_type: str) -> str:
    """Get damage split percentages for SpellHelper.Damage()."""
    splits = {
        "physical": "100, 0, 0, 0, 0",
        "fire": "0, 100, 0, 0, 0",
        "cold": "0, 0, 100, 0, 0",
        "poison": "0, 0, 0, 100, 0",
        "energy": "0, 0, 0, 0, 100"
    }
    return splits.get(damage_type, "0, 0, 0, 0, 100")

def get_circle_name(circle_num: int) -> str:
    """Convert circle number to ServUO enum name."""
    names = {
        1: "First",
        2: "Second",
        3: "Third",
        4: "Fourth",
        5: "Fifth",
        6: "Sixth",
        7: "Seventh",
        8: "Eighth"
    }
    return names.get(circle_num, "First")

def generate_spell_initializer(schools: List[MagicSchool], output_path: Path):
    """Generate VystiaSpellInitializer.cs with all spell registrations."""

    print(f"Generating spell initializer at {output_path}...")

    # Build using statements for all schools
    using_statements = "\n".join([
        f"using Server.Spells.VystiaSpells.{school.namespace};"
        for school in schools
    ])

    # Build registration code for each school
    registration_blocks = []

    for school in schools:
        block = f"\n            // {school.name} Magic Spells (IDs {school.spell_id_start}-{school.spell_id_end})\n"
        for spell in school.spells:
            block += f"            Register({spell.spell_id}, typeof({spell.class_name}Spell));\n"
        registration_blocks.append(block)

    content = f'''using System;
using Server.Spells.VystiaSpells.IceMage;
{using_statements}

namespace Server.Spells
{{
    /// <summary>
    /// Initializes all Vystia custom spells and registers them with the spell registry
    /// Generated by: generate_all_spells.py
    /// </summary>
    public class VystiaSpellInitializer
    {{
        private static bool _initialized = false;

        public static void Initialize()
        {{
            if (_initialized)
            {{
                Console.WriteLine("[VYSTIA] WARNING: Initialize() called again, skipping duplicate registration!");
                return;
            }}

            _initialized = true;
            Console.WriteLine("[VYSTIA] Initializing Vystia spells...");

            // Ice Magic Spells (IDs 999-1030) - Already implemented
            RegisterIceMageSpells();
            {''.join(registration_blocks)}

            Console.WriteLine("[VYSTIA] Spell registration complete. Total: 384 spells");
        }}

        private static void RegisterIceMageSpells()
        {{
            // Ice Magic spells already registered
            // See existing VystiaSpellInitializer.cs
        }}

        private static void Register(int spellID, Type type)
        {{
            SpellRegistry.Register(spellID, type);
            Console.WriteLine($"[VYSTIA] Registered spell ID {{spellID}}: {{type.Name}}");
        }}
    }}
}}
'''

    with open(output_path, 'w', encoding='utf-8') as f:
        f.write(content)

    print(f"  Generated spell initializer with {sum(len(s.spells) for s in schools)} spells")

def main():
    """Main execution function."""
    print("=" * 80)
    print("VYSTIA SPELL GENERATOR")
    print("=" * 80)
    print()

    # Parse command line args
    dry_run = '--dry-run' in sys.argv
    school_filter = None
    if '--school' in sys.argv:
        idx = sys.argv.index('--school')
        if idx + 1 < len(sys.argv):
            school_filter = sys.argv[idx + 1]

    # Set paths
    base_dir = Path(__file__).parent.parent
    docs_dir = base_dir / "Magic"
    output_dir = base_dir.parent / "ServUO" / "Scripts" / "Custom" / "VystiaClasses" / "Spells"

    if '--output' in sys.argv:
        idx = sys.argv.index('--output')
        if idx + 1 < len(sys.argv):
            output_dir = Path(sys.argv[idx + 1])

    print(f"Documentation directory: {docs_dir}")
    print(f"Output directory: {output_dir}")
    print(f"Dry run: {dry_run}")
    if school_filter:
        print(f"School filter: {school_filter}")
    print()

    # Parse all schools
    schools_to_generate = []
    total_spells = 0

    for school_name, school_config in MAGIC_SCHOOLS.items():
        if school_filter and school_name != school_filter:
            continue

        doc_file = docs_dir / school_config["doc_file"]
        if not doc_file.exists():
            print(f"WARNING: Documentation file not found: {doc_file}")
            continue

        spells = parse_spell_documentation(doc_file, school_name)

        if len(spells) > 0:
            school = MagicSchool(
                name=school_name,
                class_name=school_config["class_name"],
                folder_name=school_config["folder_name"],
                namespace=school_config["namespace"],
                region=school_config["region"],
                theme=school_config["theme"],
                hue=school_config["hue"],
                spell_id_start=school_config["spell_id_start"],
                spell_id_end=school_config["spell_id_end"],
                spells=spells
            )
            schools_to_generate.append(school)
            total_spells += len(spells)

    print()
    print(f"TOTAL: {len(schools_to_generate)} schools, {total_spells} spells")
    print()

    if dry_run:
        print("DRY RUN - No files will be created")
        print()
        for school in schools_to_generate:
            print(f"{school.name}: {len(school.spells)} spells")
            for spell in school.spells[:3]:  # Show first 3
                print(f"  - {spell.name} (Circle {spell.circle}, {spell.mana} mana)")
            if len(school.spells) > 3:
                print(f"  ... and {len(school.spells) - 3} more")
            print()
        return

    # Generate spell files
    print("Generating spell files...")
    files_created = 0

    for school in schools_to_generate:
        school_dir = output_dir / school.folder_name
        school_dir.mkdir(parents=True, exist_ok=True)
        print(f"  Creating {school.name} spells in {school_dir}...")

        for spell in school.spells:
            spell_file = school_dir / f"{spell.class_name}Spell.cs"
            spell_content = generate_spell_file(spell, school, school_dir)

            with open(spell_file, 'w', encoding='utf-8') as f:
                f.write(spell_content)

            files_created += 1

    print(f"  Created {files_created} spell files")
    print()

    # Generate spell initializer
    initializer_path = output_dir / "VystiaSpellInitializer_Generated.cs"
    generate_spell_initializer(schools_to_generate, initializer_path)
    print()

    print("=" * 80)
    print("GENERATION COMPLETE!")
    print("=" * 80)
    print(f"Created {files_created} spell files")
    print(f"Created 1 spell initializer")
    print()
    print("Next steps:")
    print("1. Review generated files")
    print("2. Add Vystia reagent checks to spells")
    print("3. Build with: cd ServUO && dotnet build")
    print("4. Test spells in-game")
    print()

if __name__ == "__main__":
    main()
