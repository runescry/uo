#!/usr/bin/env python3
"""
Batch implement Vystia spells based on patterns
Automatically detects spell type from name and applies appropriate template
"""

import os
import re
from typing import Dict, List, Tuple

SPELLS_DIR = r"D:\UO\ServUO\Scripts\Custom\VystiaClasses\Spells"

# Spell type detection patterns
SUMMON_KEYWORDS = ['summon', 'raise', 'conjure', 'call', 'invoke']
DAMAGE_KEYWORDS = ['bolt', 'blast', 'strike', 'burn', 'freeze', 'shock', 'arrow', 'lance', 'shard', 'beam']
BUFF_KEYWORDS = ['bless', 'enhance', 'strengthen', 'fortify', 'inspire', 'courage', 'song', 'shield', 'armor', 'ward']
DEBUFF_KEYWORDS = ['curse', 'weaken', 'hex', 'plague', 'doom', 'poison', 'fear', 'slow']
DOT_KEYWORDS = ['burn', 'bleed', 'poison', 'plague', 'corruption', 'rot']
HEAL_KEYWORDS = ['heal', 'cure', 'restore', 'rejuvenate', 'regenerate', 'mend']
AOE_KEYWORDS = ['storm', 'wave', 'explosion', 'nova', 'aura', 'field', 'rain']

# Visual effects by spell school
SCHOOL_EFFECTS = {
    'IceMage': {'particle': '0x376A', 'sound': '0x1F2', 'damage_type': '0, 0, 100, 0, 0'},  # 100% cold
    'Druid': {'particle': '0x376A', 'sound': '0x1EA', 'damage_type': '0, 0, 0, 100, 0'},  # 100% poison (nature)
    'Witch': {'particle': '0x374A', 'sound': '0x1DD', 'damage_type': '0, 0, 0, 100, 0'},  # 100% poison
    'Sorcerer': {'particle': '0x36BD', 'sound': '0x307', 'damage_type': '0, 100, 0, 0, 0'},  # 100% fire
    'Warlock': {'particle': '0x3779', 'sound': '0x1FB', 'damage_type': '100, 0, 0, 0, 0'},  # 100% physical (dark)
    'Necromancer': {'particle': '0x37C4', 'sound': '0x1FB', 'damage_type': '0, 0, 0, 0, 100'},  # 100% energy (necro)
    'Oracle': {'particle': '0x375A', 'sound': '0x1E5', 'damage_type': '0, 0, 0, 0, 100'},  # 100% energy
    'Summoner': {'particle': '0x3728', 'sound': '0x215', 'damage_type': '100, 0, 0, 0, 0'},  # Physical
    'Shaman': {'particle': '0x3728', 'sound': '0x1EA', 'damage_type': '0, 0, 0, 0, 100'},  # Energy
    'Bard': {'particle': '0x375A', 'sound': '0x1F5', 'damage_type': '0, 0, 0, 0, 100'},  # Energy (sonic)
    'Enchanter': {'particle': '0x373A', 'sound': '0x1F2', 'damage_type': '0, 0, 0, 0, 100'},  # Energy
    'Illusionist': {'particle': '0x375A', 'sound': '0x1E5', 'damage_type': '0, 0, 0, 0, 100'},  # Energy (psychic)
}

def detect_spell_type(spell_name: str) -> str:
    """Detect spell type from name"""
    name_lower = spell_name.lower()

    # Check in priority order
    if any(kw in name_lower for kw in SUMMON_KEYWORDS):
        return 'summon'
    if any(kw in name_lower for kw in HEAL_KEYWORDS):
        return 'heal'
    if any(kw in name_lower for kw in AOE_KEYWORDS):
        return 'aoe_damage'
    if any(kw in name_lower for kw in DOT_KEYWORDS):
        return 'dot'
    if any(kw in name_lower for kw in BUFF_KEYWORDS):
        return 'buff'
    if any(kw in name_lower for kw in DEBUFF_KEYWORDS):
        return 'debuff'
    if any(kw in name_lower for kw in DAMAGE_KEYWORDS):
        return 'damage'

    # Default to buff for unrecognized patterns
    return 'buff'

def get_creature_name(spell_name: str) -> str:
    """Extract creature name from summon spell name and map to known creatures"""
    # Map spell names to actual ServUO creature types
    creature_map = {
        'Wolf': 'GreyWolf',
        'Rabbit': 'Rabbit',
        'Bear': 'GrizzlyBear',
        'AirElemental': 'AirElemental',
        'EarthElemental': 'EarthElemental',
        'FireElemental': 'FireElemental',
        'WaterElemental': 'WaterElemental',
        'Drake': 'Drake',
        'Phoenix': 'Phoenix',
        'Titan': 'Titan',
        'Hydra': 'Swampdragon',  # Use existing creature
        'Wisp': 'Wisp',
        'StormElemental': 'AirElemental',  # Fallback to AirElemental
        'VoidElemental': 'EarthElemental',  # Fallback to EarthElemental
        'ElementalLord': 'EarthElemental',  # Fallback
        'FireSprite': 'FireElemental',  # Fallback
        'GreaterDragon': 'Dragon',
        'Frenzy': 'GreyWolf',  # Fallback
        'Shield': 'EarthElemental',  # "Summon Shield" -> protective elemental
    }

    for prefix in ['Summon ', 'Raise ', 'Conjure ', 'Call ', 'Invoke ']:
        if prefix in spell_name:
            creature = spell_name.replace(prefix, '').strip().replace(' ', '')
            return creature_map.get(creature, 'GreyWolf')  # Default fallback

    return 'GreyWolf'  # Ultimate default

def get_damage_values(circle: str) -> Tuple[int, int]:
    """Get min/max damage based on spell circle"""
    damage_map = {
        'First': (5, 10),
        'Second': (10, 15),
        'Third': (15, 20),
        'Fourth': (20, 25),
        'Fifth': (25, 30),
        'Sixth': (30, 40),
        'Seventh': (40, 50),
        'Eighth': (50, 65)
    }
    return damage_map.get(circle, (15, 25))

def get_buff_bonus(circle: str) -> int:
    """Get buff bonus based on spell circle"""
    bonus_map = {
        'First': 5,
        'Second': 8,
        'Third': 10,
        'Fourth': 12,
        'Fifth': 15,
        'Sixth': 18,
        'Seventh': 22,
        'Eighth': 25
    }
    return bonus_map.get(circle, 10)

def get_duration_minutes(circle: str) -> int:
    """Get buff duration in minutes based on circle"""
    duration_map = {
        'First': 1,
        'Second': 2,
        'Third': 3,
        'Fourth': 4,
        'Fifth': 5,
        'Sixth': 7,
        'Seventh': 10,
        'Eighth': 15
    }
    return duration_map.get(circle, 2)

def generate_damage_spell(spell_name: str, class_name: str, circle: str, school: str) -> str:
    """Generate direct damage spell implementation"""
    effects = SCHOOL_EFFECTS.get(school, SCHOOL_EFFECTS['Sorcerer'])
    min_dmg, max_dmg = get_damage_values(circle)

    return f'''
        public override void OnCast()
        {{
            if (CheckSequence())
            {{
                Caster.Target = new InternalTarget(this);
            }}
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
                target.FixedParticles({effects['particle']}, 20, 10, 5044, EffectLayer.Head);
                target.PlaySound({effects['sound']});

                // Calculate damage
                double damage = Utility.RandomMinMax({min_dmg}, {max_dmg});
                damage += Caster.Skills[CastSkill].Value * 0.5;

                // Apply damage
                SpellHelper.Damage(this, target, damage, {effects['damage_type']});
            }}

            FinishSequence();
        }}

        private class InternalTarget : Target
        {{
            private readonly {class_name} m_Owner;

            public InternalTarget({class_name} owner) : base(12, false, TargetFlags.Harmful)
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
        }}'''

def generate_buff_spell(spell_name: str, class_name: str, circle: str, school: str) -> str:
    """Generate buff spell implementation"""
    effects = SCHOOL_EFFECTS.get(school, SCHOOL_EFFECTS['Bard'])
    bonus = get_buff_bonus(circle)
    duration = get_duration_minutes(circle)

    return f'''
        public override void OnCast()
        {{
            if (CheckSequence())
            {{
                Caster.Target = new InternalTarget(this);
            }}
        }}

        public void Target(Mobile target)
        {{
            if (!Caster.CanSee(target))
            {{
                Caster.SendLocalizedMessage(500237); // Target can not be seen.
            }}
            else if (CheckBSequence(target))
            {{
                SpellHelper.Turn(Caster, target);

                // Visual/sound
                target.FixedParticles({effects['particle']}, 9, 32, 5008, EffectLayer.Waist);
                target.PlaySound({effects['sound']});

                // Apply stat mod
                int bonus = {bonus} + (int)(Caster.Skills[CastSkill].Value * 0.2);
                target.AddStatMod(new StatMod(StatType.Str, "{spell_name.replace(' ', '_')}_Str", bonus, TimeSpan.FromMinutes({duration})));

                target.SendMessage("You feel strengthened!");
            }}

            FinishSequence();
        }}

        private class InternalTarget : Target
        {{
            private readonly {class_name} m_Owner;

            public InternalTarget({class_name} owner) : base(12, false, TargetFlags.Beneficial)
            {{
                m_Owner = owner;
            }}

            protected override void OnTarget(Mobile from, object o)
            {{
                if (o is Mobile)
                    m_Owner.Target((Mobile)o);
            }}

            protected override void OnTargetFinish(Mobile from)
            {{
                m_Owner.FinishSequence();
            }}
        }}'''

def generate_summon_spell(spell_name: str, class_name: str, circle: str, school: str) -> str:
    """Generate summon spell implementation"""
    effects = SCHOOL_EFFECTS.get(school, SCHOOL_EFFECTS['Summoner'])
    creature = get_creature_name(spell_name)

    return f'''
        public override void OnCast()
        {{
            if (CheckSequence())
            {{
                // Check follower slots
                if (Caster.Followers + 2 > Caster.FollowersMax)
                {{
                    Caster.SendLocalizedMessage(1049645); // You have too many followers
                    FinishSequence();
                    return;
                }}

                // Create summon (adjust creature type as needed)
                BaseCreature summon = new {creature}();
                summon.Controlled = true;
                summon.ControlMaster = Caster;
                summon.Summoned = true;
                summon.SummonMaster = Caster;

                // Spawn location
                Point3D loc = Caster.Location;
                Map map = Caster.Map;

                SpellHelper.GetSurfaceTop(ref loc, map);

                summon.MoveToWorld(loc, map);

                // Visual effect
                Effects.SendLocationParticles(
                    EffectItem.Create(loc, map, EffectItem.DefaultDuration),
                    {effects['particle']}, 10, 10, 2023);

                summon.PlaySound({effects['sound']});

                // Schedule despawn (10 minutes for most summons)
                Timer.DelayCall(TimeSpan.FromMinutes(10), () =>
                {{
                    if (summon != null && !summon.Deleted)
                        summon.Delete();
                }});
            }}

            FinishSequence();
        }}'''

def generate_dot_spell(spell_name: str, class_name: str, circle: str, school: str) -> str:
    """Generate DoT (damage over time) spell implementation"""
    effects = SCHOOL_EFFECTS.get(school, SCHOOL_EFFECTS['Witch'])

    return f'''
        public override void OnCast()
        {{
            if (CheckSequence())
            {{
                Caster.Target = new InternalTarget(this);
            }}
        }}

        public void Target(Mobile target)
        {{
            if (!Caster.CanSee(target))
            {{
                Caster.SendLocalizedMessage(500237); // Target can not be seen.
            }}
            else if (CheckHSequence(target))
            {{
                SpellHelper.Turn(Caster, target);

                // Initial damage
                target.FixedParticles({effects['particle']}, 10, 15, 5021, EffectLayer.Waist);
                target.PlaySound({effects['sound']});

                // Apply DoT
                int ticks = 6; // 6 ticks over 30 seconds
                int damagePerTick = 5 + (int)(Caster.Skills[CastSkill].Value * 0.1);

                Timer.DelayCall(TimeSpan.Zero, TimeSpan.FromSeconds(5), ticks, () =>
                {{
                    if (target != null && !target.Deleted && target.Alive)
                    {{
                        target.Damage(damagePerTick, Caster);
                        target.FixedParticles({effects['particle']}, 10, 15, 5021, EffectLayer.Waist);
                    }}
                }});

                target.SendMessage("You have been afflicted with a curse!");
            }}

            FinishSequence();
        }}

        private class InternalTarget : Target
        {{
            private readonly {class_name} m_Owner;

            public InternalTarget({class_name} owner) : base(12, false, TargetFlags.Harmful)
            {{
                m_Owner = owner;
            }}

            protected override void OnTarget(Mobile from, object o)
            {{
                if (o is Mobile)
                    m_Owner.Target((Mobile)o);
            }}

            protected override void OnTargetFinish(Mobile from)
            {{
                m_Owner.FinishSequence();
            }}
        }}'''

def generate_heal_spell(spell_name: str, class_name: str, circle: str, school: str) -> str:
    """Generate heal spell implementation"""
    effects = SCHOOL_EFFECTS.get(school, SCHOOL_EFFECTS['Druid'])
    min_heal, max_heal = get_damage_values(circle)  # Same scaling as damage

    return f'''
        public override void OnCast()
        {{
            if (CheckSequence())
            {{
                Caster.Target = new InternalTarget(this);
            }}
        }}

        public void Target(Mobile target)
        {{
            if (!Caster.CanSee(target))
            {{
                Caster.SendLocalizedMessage(500237); // Target can not be seen.
            }}
            else if (CheckBSequence(target))
            {{
                SpellHelper.Turn(Caster, target);

                // Visual effect
                target.FixedParticles({effects['particle']}, 10, 15, 5013, EffectLayer.Waist);
                target.PlaySound(0x1F2); // Healing sound

                // Calculate healing
                int toHeal = Utility.RandomMinMax({min_heal}, {max_heal});
                toHeal += (int)(Caster.Skills[CastSkill].Value * 0.3);

                // Apply healing
                target.Heal(toHeal, Caster);
                target.SendMessage("You feel healed!");
            }}

            FinishSequence();
        }}

        private class InternalTarget : Target
        {{
            private readonly {class_name} m_Owner;

            public InternalTarget({class_name} owner) : base(12, false, TargetFlags.Beneficial)
            {{
                m_Owner = owner;
            }}

            protected override void OnTarget(Mobile from, object o)
            {{
                if (o is Mobile)
                    m_Owner.Target((Mobile)o);
            }}

            protected override void OnTargetFinish(Mobile from)
            {{
                m_Owner.FinishSequence();
            }}
        }}'''

def implement_spell_file(file_path: str, school: str, dry_run: bool = False) -> bool:
    """Implement a single spell file"""
    with open(file_path, 'r', encoding='utf-8') as f:
        content = f.read()

    # Check if already implemented
    if 'TODO: Implement' not in content:
        return False

    # Extract spell info
    class_match = re.search(r'public class (\w+Spell)', content)
    if not class_match:
        return False
    class_name = class_match.group(1)

    # Extract spell display name
    name_match = re.search(r'new SpellInfo\(\s*"([^"]+)"', content)
    spell_name = name_match.group(1) if name_match else "Unknown"

    # Extract circle
    circle_match = re.search(r'public override SpellCircle Circle => SpellCircle\.(\w+);', content)
    circle = circle_match.group(1) if circle_match else "First"

    # Detect spell type
    spell_type = detect_spell_type(spell_name)

    if dry_run:
        print(f"  {spell_name} ({circle}) -> {spell_type}")
        return True

    # Generate implementation based on type
    effects = SCHOOL_EFFECTS.get(school, SCHOOL_EFFECTS['Summoner'])

    if spell_type == 'summon':
        # For summons, replace entire Target method and OnCast
        creature = get_creature_name(spell_name)

        # Replace the OnCast method
        oncast_pattern = r'public override void OnCast\(\)\s*\{[^}]+\}'
        oncast_new = f'''public override void OnCast()
        {{
            if (CheckSequence())
            {{
                // Check follower slots
                if (Caster.Followers + 2 > Caster.FollowersMax)
                {{
                    Caster.SendLocalizedMessage(1049645); // You have too many followers
                    FinishSequence();
                    return;
                }}

                // Create and summon creature
                BaseCreature creature = new {creature}();
                TimeSpan duration = TimeSpan.FromMinutes(10);

                SpellHelper.Summon(creature, Caster, {effects['sound']}, duration, false, false);
            }}

            FinishSequence();
        }}'''

        content = re.sub(oncast_pattern, oncast_new, content, flags=re.DOTALL)

        # Remove the Target method and InternalTarget class entirely
        target_method_pattern = r'\s*public void Target\([^)]+\)[^{]*\{(?:[^{}]|\{[^{}]*\})*\}'
        internal_target_pattern = r'\s*private class InternalTarget[^{]*\{(?:[^{}]|\{(?:[^{}]|\{[^{}]*\})*\})*\}'

        content = re.sub(target_method_pattern, '', content, flags=re.DOTALL)
        content = re.sub(internal_target_pattern, '', content, flags=re.DOTALL)

    else:
        # For other spell types, just replace the TODO comment section
        todo_pattern = r'// Spell effect\s*\n\s*// TODO: Implement[^\n]+\n\s*//[^\n]*'

        if spell_type == 'damage' or spell_type == 'aoe_damage':
            min_dmg, max_dmg = get_damage_values(circle)
            replacement = f'''// Spell effect - Direct damage
                double damage = Utility.RandomMinMax({min_dmg}, {max_dmg});
                damage += Caster.Skills.Conjuration.Value * 0.5;

                SpellHelper.Damage(this, target, damage, {effects['damage_type']});

                if (target is Mobile mobile)
                    mobile.SendMessage("You are struck by magical energy!");'''

        elif spell_type == 'heal':
            min_heal, max_heal = get_damage_values(circle)
            replacement = f'''// Spell effect - Healing
                int toHeal = Utility.RandomMinMax({min_heal}, {max_heal});
                toHeal += (int)(Caster.Skills.Conjuration.Value * 0.3);

                if (target is Mobile mobile)
                {{
                    mobile.Heal(toHeal, Caster);
                    mobile.SendMessage("You feel healed!");
                }}'''

        elif spell_type == 'buff' or spell_type == 'debuff':
            bonus = get_buff_bonus(circle)
            duration = get_duration_minutes(circle)
            stat_type = 'Str' if spell_type == 'buff' else 'Dex'
            replacement = f'''// Spell effect - Buff
                int bonus = {bonus} + (int)(Caster.Skills.Conjuration.Value * 0.2);

                if (target is Mobile mobile)
                {{
                    mobile.AddStatMod(new StatMod(StatType.{stat_type}, "{spell_name.replace(' ', '_')}_{stat_type}", bonus, TimeSpan.FromMinutes({duration})));
                    mobile.SendMessage("You feel empowered!");
                }}'''

        elif spell_type == 'dot':
            replacement = f'''// Spell effect - Damage over time
                int ticks = 6;
                int damagePerTick = 5 + (int)(Caster.Skills.Conjuration.Value * 0.1);

                Timer.DelayCall(TimeSpan.Zero, TimeSpan.FromSeconds(5), ticks, () =>
                {{
                    if (target is Mobile mobile && !mobile.Deleted && mobile.Alive)
                    {{
                        mobile.Damage(damagePerTick, Caster);
                        mobile.FixedParticles({effects['particle']}, 10, 15, 5021, EffectLayer.Waist);
                    }}
                }});

                if (target is Mobile m)
                    m.SendMessage("You have been afflicted with a curse!");'''

        else:
            # Default generic effect
            replacement = f'''// Spell effect - Generic
                target.SendMessage("You are affected by magic!");'''

        content = re.sub(todo_pattern, replacement, content, flags=re.DOTALL)

    # Write updated file
    with open(file_path, 'w', encoding='utf-8') as f:
        f.write(content)

    return True

def main():
    """Process all spell files with TODOs"""
    import sys

    dry_run = '--dry-run' in sys.argv or '-n' in sys.argv
    school_filter = None

    # Check for school filter argument
    for arg in sys.argv[1:]:
        if not arg.startswith('-'):
            school_filter = arg
            break

    schools = [
        'Bard', 'Druid', 'Enchanter', 'IceMage', 'Illusionist',
        'Necromancer', 'Oracle', 'Shaman', 'Sorcerer', 'Summoner',
        'Warlock', 'Witch'
    ]

    if school_filter:
        schools = [s for s in schools if s.lower() == school_filter.lower()]
        if not schools:
            print(f"Unknown school: {school_filter}")
            return

    total_implemented = 0

    for school in schools:
        school_dir = os.path.join(SPELLS_DIR, school)
        if not os.path.exists(school_dir):
            continue

        print(f"\n{'=' * 60}")
        print(f"Processing {school} spells...")
        print(f"{'=' * 60}")

        school_count = 0
        for file in sorted(os.listdir(school_dir)):
            if file.endswith('Spell.cs'):
                file_path = os.path.join(school_dir, file)
                if implement_spell_file(file_path, school, dry_run):
                    school_count += 1

        print(f"\n{school}: {school_count} spells {'would be ' if dry_run else ''}implemented")
        total_implemented += school_count

    print(f"\n{'=' * 60}")
    print(f"Total: {total_implemented} spells {'would be ' if dry_run else ''}implemented")
    if dry_run:
        print("Run without --dry-run to apply changes")
    print(f"{'=' * 60}")

if __name__ == "__main__":
    main()
