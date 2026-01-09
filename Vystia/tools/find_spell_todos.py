#!/usr/bin/env python3
"""
Find all spells with TODO comments
"""

import os
import re

SPELLS_DIR = r"D:\UO\ServUO\Scripts\Custom\VystiaClasses\Spells"

def find_todos_in_file(file_path):
    """Find TODO comments in a spell file and extract spell info"""
    with open(file_path, 'r', encoding='utf-8') as f:
        content = f.read()

    # Extract spell name from class definition
    class_match = re.search(r'public class (\w+Spell)', content)
    if not class_match:
        return None

    # Extract spell display name from SpellInfo
    name_match = re.search(r'new SpellInfo\(\s*"([^"]+)"', content)
    spell_name = name_match.group(1) if name_match else "Unknown"

    # Extract circle
    circle_match = re.search(r'public override SpellCircle Circle => SpellCircle\.(\w+);', content)
    circle = circle_match.group(1) if circle_match else "Unknown"

    # Check for TODO
    has_todo = 'TODO: Implement' in content

    if has_todo:
        return {
            'name': spell_name,
            'circle': circle,
            'file': os.path.basename(file_path)
        }

    return None

def main():
    """Generate report of all spells with TODOs"""
    schools = [
        'Bard', 'Druid', 'Enchanter', 'IceMage', 'Illusionist',
        'Necromancer', 'Oracle', 'Shaman', 'Sorcerer', 'Summoner',
        'Warlock', 'Witch'
    ]

    all_todos = {}
    total_todos = 0

    for school in schools:
        school_dir = os.path.join(SPELLS_DIR, school)
        if not os.path.exists(school_dir):
            continue

        school_todos = []

        for file in os.listdir(school_dir):
            if file.endswith('Spell.cs'):
                file_path = os.path.join(school_dir, file)
                result = find_todos_in_file(file_path)
                if result:
                    school_todos.append(result)

        if school_todos:
            all_todos[school] = sorted(school_todos, key=lambda x: x['circle'])
            total_todos += len(school_todos)

    # Generate markdown report
    print("# Vystia Spells with TODO Implementation\n")
    print(f"**Total Spells with TODOs:** {total_todos} out of 384 spells\n")
    print("---\n")

    for school in schools:
        if school not in all_todos:
            print(f"## {school} (0 TODOs)")
            print("All spells implemented!\n")
            continue

        todos = all_todos[school]
        print(f"## {school} ({len(todos)} TODOs)\n")
        print("| Circle | Spell Name | File |")
        print("|--------|------------|------|")

        for todo in todos:
            print(f"| {todo['circle']} | {todo['name']} | {todo['file']} |")

        print()

    # Summary by circle
    print("---\n")
    print("## Summary by Circle\n")

    circle_counts = {}
    for school_todos in all_todos.values():
        for todo in school_todos:
            circle = todo['circle']
            circle_counts[circle] = circle_counts.get(circle, 0) + 1

    print("| Circle | TODOs |")
    print("|--------|-------|")
    for circle in ['First', 'Second', 'Third', 'Fourth', 'Fifth', 'Sixth', 'Seventh', 'Eighth']:
        count = circle_counts.get(circle, 0)
        print(f"| {circle} | {count} |")

    print(f"\n**Total:** {total_todos} spells need implementation")

if __name__ == "__main__":
    main()
