#!/usr/bin/env python3
"""
Spell Fixer Script for Vystia Magic System
Reads spell files, extracts XML summary, and generates proper implementations
based on spell type (damage, buff, debuff, shapeshift, summon, etc.)
"""

import os
import re
import glob

SPELLS_DIR = os.path.dirname(os.path.abspath(__file__))

# Pattern to match the broken spell implementation
BROKEN_PATTERN = re.compile(
    r'public override void OnCast\(\)\s*\{[^}]*?// Check fizzle.*?'
    r'Caster\.Target = new InternalTarget\(this\);.*?\}'
    r'.*?public void Target\(IDamageable target\).*?'
    r'// Spell effect - Buff.*?'
    r'mobile\.AddStatMod\(new StatMod\(StatType\.Str.*?'
    r'private class InternalTarget : Target.*?'
    r'TargetFlags\.Harmful.*?'
    r'OnTargetFinish\(Mobile from\)\s*\{[^}]*\}[^}]*\}',
    re.DOTALL
)

def extract_spell_info(content):
    """Extract spell info from XML summary."""
    # Extract summary
    summary_match = re.search(r'/// <summary>\s*/// (.+?)/// </summary>', content, re.DOTALL)
    if summary_match:
        summary = summary_match.group(1).strip()
        summary = re.sub(r'\s*///\s*', ' ', summary).strip()
    else:
        summary = ""

    # Extract spell name from class definition
    class_match = re.search(r'public class (\w+)Spell : VystiaSpell', content)
    spell_class = class_match.group(1) if class_match else ""

    # Extract circle
    circle_match = re.search(r'SpellCircle\.(\w+)', content)
    circle = circle_match.group(1) if circle_match else "First"

    return {
        'summary': summary,
        'class_name': spell_class,
        'circle': circle,
        'is_buff': any(x in summary.lower() for x in ['+', 'buff', 'increase', 'grant', 'heal', 'regen', 'resist', 'protect']),
        'is_debuff': any(x in summary.lower() for x in ['curse', 'weaken', 'slow', 'reduce', 'fear', 'confuse', 'silence', '-']),
        'is_damage': any(x in summary.lower() for x in ['damage', 'harm', 'attack', 'strike', 'burn', 'blast', 'bolt']),
        'is_shapeshift': 'form' in summary.lower() or 'transform' in summary.lower(),
        'is_summon': any(x in summary.lower() for x in ['summon', 'animate', 'conjure', 'call', 'army']),
        'is_aoe': any(x in summary.lower() for x in ['aoe', 'area', 'mass', 'all', 'radius', 'nearby']),
        'is_self': any(x in summary.lower() for x in ['self', 'caster']),
        'is_invis': 'invisib' in summary.lower() or 'hidden' in summary.lower(),
        'is_totem': 'totem' in summary.lower(),
    }

def generate_buff_implementation(info):
    """Generate a proper buff implementation."""
    stat_mod = "Str"
    amount = 10
    resist_type = None

    summary = info['summary'].lower()

    # Determine stat type
    if 'dex' in summary:
        stat_mod = "Dex"
    elif 'int' in summary:
        stat_mod = "Int"

    # Extract amount from summary
    amount_match = re.search(r'\+(\d+)', info['summary'])
    if amount_match:
        amount = int(amount_match.group(1))

    # Check for resistance
    for r in ['physical', 'fire', 'cold', 'poison', 'energy']:
        if r in summary:
            resist_type = r.capitalize()
            break

    code = f'''        public override void OnCast()
        {{
            Caster.Target = new InternalTarget(this);
        }}

        public void Target(Mobile target)
        {{
            if (!Caster.CanSee(target))
            {{
                Caster.SendLocalizedMessage(500237);
            }}
            else if (CheckBSequence(target))
            {{
                SpellHelper.Turn(Caster, target);

                // Visual effect
                target.FixedParticles(0x375A, 10, 30, 5013, 0x21, 0, EffectLayer.Waist);
                target.PlaySound(0x1F2);

                // Calculate duration (2-5 minutes)
                double duration = 2.0 + (Caster.Skills.Magery.Value / 40.0);

                // {info['summary']}
                target.AddStatMod(new StatMod(StatType.{stat_mod}, "{info['class_name']}_{stat_mod}", {amount}, TimeSpan.FromMinutes(duration)));
'''

    if resist_type:
        code += f'''
                ResistanceMod resistMod = new ResistanceMod(ResistanceType.{resist_type}, {amount});
                target.AddResistanceMod(resistMod);

                Timer.DelayCall(TimeSpan.FromMinutes(duration), () =>
                {{
                    if (target != null && !target.Deleted)
                    {{
                        target.RemoveResistanceMod(resistMod);
                        target.SendMessage("The spell effect fades.");
                    }}
                }});
'''

    code += f'''
                target.SendMessage(0x3B2, "You are empowered by {info['class_name'].replace('_', ' ')}!");
                if (target != Caster)
                    Caster.SendMessage(0x3B2, "You grant your ally a powerful buff!");
            }}

            FinishSequence();
        }}

        private class InternalTarget : Target
        {{
            private readonly {info['class_name']}Spell m_Owner;

            public InternalTarget({info['class_name']}Spell owner)
                : base(10, false, TargetFlags.Beneficial)
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

    return code

def generate_debuff_implementation(info):
    """Generate a proper debuff implementation."""
    stat_mod = "Str"
    amount = 10

    summary = info['summary'].lower()

    if 'dex' in summary or 'slow' in summary or 'speed' in summary:
        stat_mod = "Dex"
    elif 'int' in summary or 'mana' in summary or 'magic' in summary:
        stat_mod = "Int"

    # Extract amount
    amount_match = re.search(r'-?(\d+)', info['summary'])
    if amount_match:
        amount = int(amount_match.group(1))
    else:
        amount = 15

    code = f'''        public override void OnCast()
        {{
            Caster.Target = new InternalTarget(this);
        }}

        public void Target(Mobile target)
        {{
            if (!Caster.CanSee(target))
            {{
                Caster.SendLocalizedMessage(500237);
            }}
            else if (CheckHSequence(target))
            {{
                SpellHelper.Turn(Caster, target);

                // Visual effect
                target.FixedParticles(0x374A, 10, 30, 5013, 0x21, 0, EffectLayer.Waist);
                target.PlaySound(0x1FB);

                // Calculate duration (8-20 seconds)
                double duration = 8.0 + (Caster.Skills.Magery.Value / 10.0);

                // {info['summary']}
                target.AddStatMod(new StatMod(StatType.{stat_mod}, "{info['class_name']}_{stat_mod}", -{amount}, TimeSpan.FromSeconds(duration)));

                target.SendMessage(0x22, "You feel weakened by {info['class_name'].replace('_', ' ')}!");
                Caster.SendMessage(0x3B2, "You curse your enemy!");
            }}

            FinishSequence();
        }}

        private class InternalTarget : Target
        {{
            private readonly {info['class_name']}Spell m_Owner;

            public InternalTarget({info['class_name']}Spell owner)
                : base(12, false, TargetFlags.Harmful)
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

    return code

def generate_damage_implementation(info):
    """Generate a proper damage spell implementation."""
    min_damage = 10
    max_damage = 20
    damage_type = "Fire"

    summary = info['summary'].lower()

    # Extract damage values
    damage_match = re.search(r'(\d+)[- ]+(\d+)', info['summary'])
    if damage_match:
        min_damage = int(damage_match.group(1))
        max_damage = int(damage_match.group(2))
    else:
        # Base on circle
        circle_mult = {'First': 1, 'Second': 1.5, 'Third': 2, 'Fourth': 2.5,
                      'Fifth': 3, 'Sixth': 3.5, 'Seventh': 4, 'Eighth': 5}
        mult = circle_mult.get(info['circle'], 1)
        min_damage = int(10 * mult)
        max_damage = int(25 * mult)

    # Determine damage type
    if 'cold' in summary or 'ice' in summary or 'frost' in summary:
        damage_type = "Cold"
    elif 'fire' in summary or 'flame' in summary or 'burn' in summary:
        damage_type = "Fire"
    elif 'poison' in summary or 'toxic' in summary:
        damage_type = "Poison"
    elif 'energy' in summary or 'lightning' in summary or 'electric' in summary:
        damage_type = "Energy"
    else:
        damage_type = "Physical"

    # Damage distribution (100% of specified type)
    damage_dist = {'Physical': 0, 'Fire': 0, 'Cold': 0, 'Poison': 0, 'Energy': 0}
    damage_dist[damage_type] = 100

    if info['is_aoe']:
        return generate_aoe_damage_implementation(info, min_damage, max_damage, damage_dist)

    code = f'''        public override void OnCast()
        {{
            Caster.Target = new InternalTarget(this);
        }}

        public void Target(Mobile target)
        {{
            if (!Caster.CanSee(target))
            {{
                Caster.SendLocalizedMessage(500237);
            }}
            else if (CheckHSequence(target))
            {{
                SpellHelper.Turn(Caster, target);

                // Visual effect
                target.FixedParticles(0x36BD, 20, 10, 5044, 0x21, 0, EffectLayer.Head);
                target.PlaySound(0x307);

                // {info['summary']}
                double damage = Utility.RandomMinMax({min_damage}, {max_damage});
                damage += Caster.Skills.Magery.Value / 10.0;

                SpellHelper.Damage(this, target, damage, {damage_dist['Physical']}, {damage_dist['Fire']}, {damage_dist['Cold']}, {damage_dist['Poison']}, {damage_dist['Energy']});

                Caster.SendMessage(0x3B2, "Your spell strikes true!");
            }}

            FinishSequence();
        }}

        private class InternalTarget : Target
        {{
            private readonly {info['class_name']}Spell m_Owner;

            public InternalTarget({info['class_name']}Spell owner)
                : base(12, false, TargetFlags.Harmful)
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

    return code

def generate_aoe_damage_implementation(info, min_damage, max_damage, damage_dist):
    """Generate AoE damage spell implementation."""
    code = f'''        public override void OnCast()
        {{
            Caster.Target = new InternalTarget(this);
        }}

        public void Target(IPoint3D p)
        {{
            if (!Caster.CanSee(p))
            {{
                Caster.SendLocalizedMessage(500237);
            }}
            else if (CheckSequence())
            {{
                SpellHelper.Turn(Caster, p);

                Point3D loc = new Point3D(p);
                Map map = Caster.Map;

                // Visual effect at location
                Effects.SendLocationParticles(
                    EffectItem.Create(loc, map, EffectItem.DefaultDuration),
                    0x36BD, 20, 30, 0x21, 0, 5044, 0);
                Effects.PlaySound(loc, map, 0x307);

                Caster.SendMessage(0x3B2, "Your spell erupts!");

                // {info['summary']}
                System.Collections.Generic.List<Mobile> targets = new System.Collections.Generic.List<Mobile>();
                IPooledEnumerable eable = map.GetMobilesInRange(loc, 4);

                foreach (Mobile m in eable)
                {{
                    if (m != Caster && m.Alive && Caster.CanBeHarmful(m, false))
                        targets.Add(m);
                }}
                eable.Free();

                foreach (Mobile m in targets)
                {{
                    Caster.DoHarmful(m);

                    double damage = Utility.RandomMinMax({min_damage}, {max_damage});
                    damage += Caster.Skills.Magery.Value / 10.0;

                    SpellHelper.Damage(this, m, damage, {damage_dist['Physical']}, {damage_dist['Fire']}, {damage_dist['Cold']}, {damage_dist['Poison']}, {damage_dist['Energy']});

                    m.FixedParticles(0x36BD, 10, 20, 5044, 0x21, 0, EffectLayer.Waist);
                }}

                if (targets.Count > 0)
                    Caster.SendMessage(0x3B2, $"You strike {{targets.Count}} enemies!");
            }}

            FinishSequence();
        }}

        private class InternalTarget : Target
        {{
            private readonly {info['class_name']}Spell m_Owner;

            public InternalTarget({info['class_name']}Spell owner)
                : base(12, true, TargetFlags.None)
            {{
                m_Owner = owner;
            }}

            protected override void OnTarget(Mobile from, object o)
            {{
                if (o is IPoint3D)
                    m_Owner.Target((IPoint3D)o);
            }}

            protected override void OnTargetFinish(Mobile from)
            {{
                m_Owner.FinishSequence();
            }}
        }}'''

    return code

def generate_shapeshift_implementation(info):
    """Generate shapeshift spell implementation."""
    # Parse form name and stats from summary
    summary = info['summary'].lower()

    # Default body and stats
    body_id = 225  # Wolf default
    stat_type = "Str"
    stat_amount = 30

    if 'wolf' in summary:
        body_id = 225
        stat_type = "Dex"
        stat_amount = 25
    elif 'bear' in summary:
        body_id = 213
        stat_type = "Str"
        stat_amount = 30
    elif 'hawk' in summary or 'eagle' in summary or 'bird' in summary:
        body_id = 5
        stat_type = "Dex"
        stat_amount = 40
    elif 'treant' in summary or 'tree' in summary:
        body_id = 301
        stat_type = "Str"
        stat_amount = 50
    elif 'hydra' in summary or 'serpent' in summary:
        body_id = 33
        stat_type = "Str"
        stat_amount = 70
    elif 'lich' in summary:
        body_id = 24
        stat_type = "Int"
        stat_amount = 50
    elif 'demon' in summary:
        body_id = 9
        stat_type = "Str"
        stat_amount = 60
    elif 'dragon' in summary:
        body_id = 12
        stat_type = "Str"
        stat_amount = 80
    elif 'elemental' in summary:
        body_id = 16  # Fire elemental
        stat_type = "Int"
        stat_amount = 40
    elif 'ghost' in summary or 'spirit' in summary:
        body_id = 26
        stat_type = "Dex"
        stat_amount = 35
    elif 'titan' in summary:
        body_id = 189
        stat_type = "Str"
        stat_amount = 100

    # Extract stat amount from summary if present
    stat_match = re.search(r'\+(\d+)\s*(STR|DEX|INT)', info['summary'], re.IGNORECASE)
    if stat_match:
        stat_amount = int(stat_match.group(1))
        stat_type = stat_match.group(2).capitalize()
        if stat_type.upper() == "STR":
            stat_type = "Str"
        elif stat_type.upper() == "DEX":
            stat_type = "Dex"
        else:
            stat_type = "Int"

    form_name = info['class_name'].replace('Spell', '').replace('_', '')

    code = f'''        public override void OnCast()
        {{
            if (CheckSequence())
            {{
                // Toggle off if already in this form
                if (Caster.BodyMod == {body_id})
                {{
                    Remove{form_name}(Caster);
                    Caster.SendMessage(0x3B2, "You return to your normal form.");
                }}
                else
                {{
                    Apply{form_name}(Caster);
                }}
            }}

            FinishSequence();
        }}

        private void Apply{form_name}(Mobile m)
        {{
            // Visual effect
            m.FixedParticles(0x3728, 10, 15, 5038, 0x21, 0, EffectLayer.Waist);
            m.PlaySound(0x1FE);

            // Transform
            m.BodyMod = {body_id};
            m.HueMod = 0;

            // Calculate duration (3-6 minutes)
            double duration = 3.0 + (Caster.Skills.Magery.Value / 40.0);

            // {info['summary']}
            m.AddStatMod(new StatMod(StatType.{stat_type}, "{form_name}_{stat_type}", {stat_amount}, TimeSpan.FromMinutes(duration)));

            m.SendMessage(0x3B2, "You transform! (+{stat_amount} {stat_type.upper()})");
            m.SendMessage(0x22, "You cannot cast spells in this form!");

            // Auto-revert after duration
            Timer.DelayCall(TimeSpan.FromMinutes(duration), () =>
            {{
                if (m != null && !m.Deleted && m.BodyMod == {body_id})
                {{
                    Remove{form_name}(m);
                    m.SendMessage("Your transformation fades.");
                }}
            }});
        }}

        private static void Remove{form_name}(Mobile m)
        {{
            m.BodyMod = 0;
            m.HueMod = -1;
            m.RemoveStatMod("{form_name}_{stat_type}");

            m.FixedParticles(0x3728, 10, 15, 5038, 0x21, 0, EffectLayer.Waist);
            m.PlaySound(0x1FE);
        }}'''

    return code

def generate_summon_implementation(info):
    """Generate summon spell implementation."""
    code = f'''        public override void OnCast()
        {{
            if (CheckSequence())
            {{
                SpellHelper.Turn(Caster, Caster);

                // Visual effect
                Caster.FixedParticles(0x3728, 10, 15, 5038, 0x21, 0, EffectLayer.Waist);
                Caster.PlaySound(0x212);

                // {info['summary']}
                // Note: Summon implementation requires creature definitions
                Caster.SendMessage(0x3B2, "You summon a creature to aid you!");
                Caster.SendMessage(0x22, "(Summon creature not yet implemented - needs creature class)");
            }}

            FinishSequence();
        }}'''

    return code

def generate_invis_implementation(info):
    """Generate invisibility spell implementation."""
    code = f'''        public override void OnCast()
        {{
            Caster.Target = new InternalTarget(this);
        }}

        public void Target(Mobile target)
        {{
            if (!Caster.CanSee(target))
            {{
                Caster.SendLocalizedMessage(500237);
            }}
            else if (CheckBSequence(target))
            {{
                SpellHelper.Turn(Caster, target);

                // Visual effect
                target.FixedParticles(0x376A, 9, 32, 5007, EffectLayer.Waist);
                target.PlaySound(0x3C4);

                // Calculate duration (30 seconds - 2 minutes)
                double duration = 30.0 + (Caster.Skills.Magery.Value / 2.0);

                // {info['summary']}
                target.Hidden = true;

                target.SendMessage(0x3B2, "You fade from sight!");
                if (target != Caster)
                    Caster.SendMessage(0x3B2, "You grant invisibility!");

                // Note: Hidden status is broken by actions, no timer needed
            }}

            FinishSequence();
        }}

        private class InternalTarget : Target
        {{
            private readonly {info['class_name']}Spell m_Owner;

            public InternalTarget({info['class_name']}Spell owner)
                : base(10, false, TargetFlags.Beneficial)
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

    return code

def generate_totem_implementation(info):
    """Generate totem spell implementation."""
    code = f'''        public override void OnCast()
        {{
            if (CheckSequence())
            {{
                // Visual effect at caster
                Caster.FixedParticles(0x3728, 10, 15, 5038, 0x21, 0, EffectLayer.Waist);
                Caster.PlaySound(0x212);

                // {info['summary']}
                // Create totem item at caster's location
                Point3D loc = Caster.Location;
                Map map = Caster.Map;

                // Totem effect (simplified - aura buff)
                double duration = 1.0 + (Caster.Skills.Magery.Value / 60.0);

                Caster.SendMessage(0x3B2, "You plant a totem!");
                Caster.SendMessage(0x22, "(Totem effect active for " + duration.ToString("F1") + " minutes)");

                // Apply totem buff to nearby allies
                IPooledEnumerable eable = map.GetMobilesInRange(loc, 8);

                foreach (Mobile m in eable)
                {{
                    if (m == Caster || (m is PlayerMobile && m.Party == Caster.Party))
                    {{
                        m.AddStatMod(new StatMod(StatType.Str, "{info['class_name']}_Totem", 10, TimeSpan.FromMinutes(duration)));
                        m.FixedParticles(0x375A, 10, 15, 5013, 0x21, 0, EffectLayer.Waist);
                    }}
                }}
                eable.Free();
            }}

            FinishSequence();
        }}'''

    return code

def generate_self_buff_implementation(info):
    """Generate self-cast buff implementation."""
    stat_type = "Str"
    amount = 20

    summary = info['summary'].lower()

    if 'dex' in summary:
        stat_type = "Dex"
    elif 'int' in summary:
        stat_type = "Int"

    amount_match = re.search(r'\+(\d+)', info['summary'])
    if amount_match:
        amount = int(amount_match.group(1))

    code = f'''        public override void OnCast()
        {{
            if (CheckSequence())
            {{
                // Visual effect
                Caster.FixedParticles(0x375A, 10, 30, 5013, 0x21, 0, EffectLayer.Waist);
                Caster.PlaySound(0x1F2);

                // Calculate duration (2-5 minutes)
                double duration = 2.0 + (Caster.Skills.Magery.Value / 40.0);

                // {info['summary']}
                Caster.AddStatMod(new StatMod(StatType.{stat_type}, "{info['class_name']}_{stat_type}", {amount}, TimeSpan.FromMinutes(duration)));

                Caster.SendMessage(0x3B2, "You feel empowered! (+{amount} {stat_type.upper()})");
            }}

            FinishSequence();
        }}'''

    return code

def determine_spell_type_and_generate(info):
    """Determine spell type and generate appropriate implementation."""
    if info['is_shapeshift']:
        return generate_shapeshift_implementation(info)
    elif info['is_totem']:
        return generate_totem_implementation(info)
    elif info['is_summon']:
        return generate_summon_implementation(info)
    elif info['is_invis']:
        return generate_invis_implementation(info)
    elif info['is_self'] and info['is_buff']:
        return generate_self_buff_implementation(info)
    elif info['is_damage']:
        return generate_damage_implementation(info)
    elif info['is_debuff']:
        return generate_debuff_implementation(info)
    elif info['is_buff']:
        return generate_buff_implementation(info)
    else:
        # Default to buff
        return generate_buff_implementation(info)

def fix_spell_file(filepath):
    """Fix a single spell file."""
    with open(filepath, 'r', encoding='utf-8') as f:
        content = f.read()

    # Check if file has the broken pattern
    if '// Spell effect - Buff' not in content:
        return False  # Already fixed or different implementation

    info = extract_spell_info(content)
    if not info['class_name']:
        print(f"  Could not extract class name from {filepath}")
        return False

    print(f"  Fixing {info['class_name']}...")
    print(f"    Summary: {info['summary'][:80]}...")

    # Generate new implementation
    new_impl = determine_spell_type_and_generate(info)

    # Find and replace the broken implementation
    # Pattern to match from OnCast to end of InternalTarget class
    old_pattern = re.compile(
        r'        public override void OnCast\(\)\s*\{.*?'
        r'private class InternalTarget : Target.*?'
        r'OnTargetFinish\(Mobile from\)\s*\{[^}]*\}[^}]*\}',
        re.DOTALL
    )

    # Also need to handle self-cast spells without targeting
    old_pattern_self = re.compile(
        r'        public override void OnCast\(\)\s*\{.*?'
        r'FinishSequence\(\);\s*\}',
        re.DOTALL
    )

    if old_pattern.search(content):
        new_content = old_pattern.sub(new_impl, content)
    elif old_pattern_self.search(content) and not 'InternalTarget' in content:
        new_content = old_pattern_self.sub(new_impl, content)
    else:
        print(f"    Could not find pattern to replace in {filepath}")
        return False

    # Write fixed content
    with open(filepath, 'w', encoding='utf-8') as f:
        f.write(new_content)

    return True

def main():
    """Main function to fix all broken spells."""
    schools = ['Bard', 'Sorcerer', 'Warlock', 'Witch', 'Illusionist',
               'Necromancer', 'Enchanter', 'Oracle', 'Shaman', 'Summoner']

    total_fixed = 0
    total_failed = 0

    for school in schools:
        school_dir = os.path.join(SPELLS_DIR, school)
        if not os.path.exists(school_dir):
            print(f"School directory not found: {school_dir}")
            continue

        print(f"\n=== Fixing {school} spells ===")

        spell_files = glob.glob(os.path.join(school_dir, '*.cs'))
        fixed = 0

        for filepath in spell_files:
            try:
                if fix_spell_file(filepath):
                    fixed += 1
            except Exception as e:
                print(f"  Error fixing {filepath}: {e}")
                total_failed += 1

        print(f"  Fixed {fixed} spells in {school}")
        total_fixed += fixed

    print(f"\n=== Summary ===")
    print(f"Total spells fixed: {total_fixed}")
    print(f"Total failures: {total_failed}")

if __name__ == '__main__':
    main()
