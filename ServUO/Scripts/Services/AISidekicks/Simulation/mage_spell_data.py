"""
Wystia Mage Spell Data Extraction
Extracts spell information from design documentation for combat simulation
"""

import re
import os
from dataclasses import dataclass
from typing import List, Dict, Optional

@dataclass
class Spell:
    """Represents a single spell"""
    name: str
    circle: int
    mana: int
    cast_time: float
    cooldown: float
    range: int
    spell_type: str  # damage, heal, buff, debuff, dot, aoe, summon, utility

    # Damage spells
    damage_min: int = 0
    damage_max: int = 0
    damage_type: str = ""  # physical, fire, cold, poison, energy, etc.

    # Healing spells
    heal_min: int = 0
    heal_max: int = 0

    # DoT spells
    dot_damage: int = 0
    dot_duration: float = 0
    dot_tick_rate: float = 0

    # Buff/Debuff
    buff_stat: str = ""  # STR, DEX, INT, resist, etc.
    buff_amount: int = 0
    buff_duration: float = 0

    # AoE
    is_aoe: bool = False
    aoe_radius: int = 0

    # Special
    special_effect: str = ""  # slow, root, stun, fear, etc.

    def __repr__(self):
        return f"<Spell: {self.name} C{self.circle} ({self.mana}m, {self.cast_time}s)>"

@dataclass
class MagicSchool:
    """Represents a complete magic school"""
    name: str
    class_name: str
    theme: str
    primary_stat: str
    hue: str
    spells: List[Spell]

    # School stats
    total_damage_potential: int = 0
    total_heal_potential: int = 0
    burst_damage: int = 0
    sustained_dps: float = 0
    survivability_score: int = 0
    utility_score: int = 0

    def calculate_school_stats(self):
        """Calculate overall school statistics"""
        self.total_damage_potential = sum(s.damage_max for s in self.spells if s.spell_type == "damage")
        self.total_heal_potential = sum(s.heal_max for s in self.spells if s.spell_type == "heal")

        # Burst = highest damage spell
        damage_spells = [s for s in self.spells if s.spell_type == "damage"]
        self.burst_damage = max([s.damage_max for s in damage_spells]) if damage_spells else 0

        # Sustained DPS = average damage / (cast time + cooldown)
        if damage_spells:
            dps_values = []
            for s in damage_spells:
                avg_damage = (s.damage_min + s.damage_max) / 2
                time_per_cast = s.cast_time + s.cooldown
                if time_per_cast > 0:
                    dps_values.append(avg_damage / time_per_cast)
            self.sustained_dps = sum(dps_values) / len(dps_values) if dps_values else 0

        # Count defensive/utility spells
        self.survivability_score = len([s for s in self.spells if s.spell_type in ["heal", "buff"]])
        self.utility_score = len([s for s in self.spells if s.spell_type in ["debuff", "utility"]])

def parse_spell_from_md(spell_text: str, circle: int) -> Optional[Spell]:
    """Parse a spell from markdown text"""
    try:
        # Extract spell name
        name_match = re.search(r'####\s*\d+\.\s*(.+)', spell_text)
        if not name_match:
            return None
        name = name_match.group(1).strip()

        # Extract mana
        mana_match = re.search(r'\*\*Mana:\*\*\s*(\d+)', spell_text)
        mana = int(mana_match.group(1)) if mana_match else circle * 2 + 2

        # Extract cast time
        cast_match = re.search(r'\*\*Cast Time:\*\*\s*([\d.]+)s', spell_text)
        cast_time = float(cast_match.group(1)) if cast_match else 1.0

        # Extract range
        range_match = re.search(r'\*\*Range:\*\*\s*(\d+)', spell_text)
        spell_range = int(range_match.group(1)) if range_match else 10

        # Extract cooldown if present
        cooldown_match = re.search(r'\*\*Cooldown:\*\*\s*([\d.]+)s', spell_text)
        cooldown = float(cooldown_match.group(1)) if cooldown_match else 0.0

        # Determine spell type and extract relevant data
        spell_type = "utility"
        damage_min, damage_max = 0, 0
        heal_min, heal_max = 0, 0
        is_aoe = False

        # Check for damage
        damage_match = re.search(r'Deals?\s*(\d+)-(\d+)\s*(?:HP\s*)?damage', spell_text, re.IGNORECASE)
        if damage_match:
            spell_type = "damage"
            damage_min = int(damage_match.group(1))
            damage_max = int(damage_match.group(2))

        # Check for healing
        heal_match = re.search(r'Heals?\s*(\d+)-(\d+)\s*HP', spell_text, re.IGNORECASE)
        if heal_match:
            spell_type = "heal"
            heal_min = int(heal_match.group(1))
            heal_max = int(heal_match.group(2))

        # Check for AoE
        if re.search(r'(\d+)\s*tile\s*radius|AoE|area', spell_text, re.IGNORECASE):
            is_aoe = True
            aoe_match = re.search(r'(\d+)\s*tile\s*radius', spell_text, re.IGNORECASE)
            aoe_radius = int(aoe_match.group(1)) if aoe_match else 5
        else:
            aoe_radius = 0

        # Check for DoT
        dot_match = re.search(r'(\d+)-(\d+)\s*damage.*?(\d+)\s*seconds', spell_text, re.IGNORECASE)
        dot_damage = 0
        dot_duration = 0
        if dot_match:
            spell_type = "dot"
            dot_damage = (int(dot_match.group(1)) + int(dot_match.group(2))) // 2
            dot_duration = float(dot_match.group(3))

        # Check for buff/debuff
        if re.search(r'\+\d+.*?(STR|DEX|INT|Resist)', spell_text, re.IGNORECASE):
            spell_type = "buff" if spell_type == "utility" else spell_type

        return Spell(
            name=name,
            circle=circle,
            mana=mana,
            cast_time=cast_time,
            cooldown=cooldown,
            range=spell_range,
            spell_type=spell_type,
            damage_min=damage_min,
            damage_max=damage_max,
            heal_min=heal_min,
            heal_max=heal_max,
            dot_damage=dot_damage,
            dot_duration=dot_duration,
            is_aoe=is_aoe,
            aoe_radius=aoe_radius
        )

    except Exception as e:
        print(f"Error parsing spell: {e}")
        return None

def load_magic_school(md_file_path: str) -> Optional[MagicSchool]:
    """Load a complete magic school from its .md file"""
    try:
        with open(md_file_path, 'r', encoding='utf-8') as f:
            content = f.read()

        # Extract school metadata
        name_match = re.search(r'#\s*(.+?)\s*-\s*(.+?)\s*School', content)
        if not name_match:
            return None

        name = name_match.group(1).strip()

        class_match = re.search(r'\*\*Class:\*\*\s*(.+)', content)
        class_name = class_match.group(1).strip() if class_match else "Unknown"

        theme_match = re.search(r'\*\*Theme:\*\*\s*(.+)', content)
        theme = theme_match.group(1).strip() if theme_match else "Unknown"

        stat_match = re.search(r'\*\*Primary Stat:\*\*\s*(.+)', content)
        primary_stat = stat_match.group(1).strip() if stat_match else "Intelligence"

        # Parse all spells by circle
        spells = []
        for circle in range(1, 9):
            # Find circle section
            circle_pattern = rf'###\s*Circle {circle}.*?(?=###\s*Circle {circle+1}|###\s*Reagents|$)'
            circle_section = re.search(circle_pattern, content, re.DOTALL)

            if circle_section:
                # Find all spells in this circle
                spell_pattern = r'####\s*\d+\..*?(?=####\s*\d+\.|###\s*Circle|###\s*Reagents|$)'
                spell_matches = re.finditer(spell_pattern, circle_section.group(0), re.DOTALL)

                for match in spell_matches:
                    spell = parse_spell_from_md(match.group(0), circle)
                    if spell:
                        spells.append(spell)

        school = MagicSchool(
            name=name,
            class_name=class_name,
            theme=theme,
            primary_stat=primary_stat,
            hue="",
            spells=spells
        )

        school.calculate_school_stats()
        return school

    except Exception as e:
        print(f"Error loading magic school from {md_file_path}: {e}")
        return None

def load_all_schools(base_path: str = r"D:\UO\Vystia\Magic") -> Dict[str, MagicSchool]:
    """Load all 12 magic schools"""
    school_files = {
        "Ice Magic": "IceMagic.md",
        "Nature Magic": "NatureMagic.md",
        "Hex Magic": "HexMagic.md",
        "Elemental Magic": "ElementalMagic.md",
        "Dark Magic": "DarkMagic.md",
        "Divination": "DivinationMagic.md",
        "Necromancy": "Necromancy.md",
        "Summoning": "SummoningMagic.md",
        "Shamanic": "ShamanicMagic.md",
        "Bardic": "BardicMagic.md",
        "Enchanting": "EnchantingMagic.md",
        "Illusion": "IllusionMagic.md"
    }

    schools = {}
    for school_name, filename in school_files.items():
        file_path = os.path.join(base_path, filename)
        if os.path.exists(file_path):
            school = load_magic_school(file_path)
            if school:
                schools[school_name] = school
                print(f"[OK] Loaded {school_name}: {len(school.spells)} spells")
            else:
                print(f"[FAIL] Failed to load {school_name}")
        else:
            print(f"[FAIL] File not found: {file_path}")

    return schools

def print_school_summary(school: MagicSchool):
    """Print a summary of a magic school"""
    print(f"\n{'='*70}")
    print(f"School: {school.name} ({school.class_name})")
    print(f"Theme: {school.theme}")
    print(f"Primary Stat: {school.primary_stat}")
    print(f"Total Spells: {len(school.spells)}")
    print(f"\nStatistics:")
    print(f"  Total Damage Potential: {school.total_damage_potential}")
    print(f"  Total Heal Potential: {school.total_heal_potential}")
    print(f"  Burst Damage (Max): {school.burst_damage}")
    print(f"  Sustained DPS: {school.sustained_dps:.1f}")
    print(f"  Survivability Score: {school.survivability_score}")
    print(f"  Utility Score: {school.utility_score}")

    # Spell breakdown
    spell_types = {}
    for spell in school.spells:
        spell_types[spell.spell_type] = spell_types.get(spell.spell_type, 0) + 1

    print(f"\nSpell Breakdown:")
    for spell_type, count in sorted(spell_types.items()):
        print(f"  {spell_type.capitalize()}: {count}")

if __name__ == "__main__":
    print("Loading all Vystia magic schools...\n")
    schools = load_all_schools()

    print(f"\n{'='*70}")
    print(f"Loaded {len(schools)} magic schools successfully!")
    print(f"{'='*70}")

    # Print summary for each school
    for school_name in sorted(schools.keys()):
        print_school_summary(schools[school_name])

    # Quick balance overview
    print(f"\n{'='*70}")
    print("QUICK BALANCE OVERVIEW")
    print(f"{'='*70}")
    print(f"{'School':<20} {'Spells':<8} {'Burst':<8} {'DPS':<8} {'Healing':<8} {'Utility':<8}")
    print(f"{'-'*70}")

    for school_name in sorted(schools.keys()):
        school = schools[school_name]
        print(f"{school_name:<20} {len(school.spells):<8} {school.burst_damage:<8} "
              f"{school.sustained_dps:<8.1f} {school.survivability_score:<8} {school.utility_score:<8}")
