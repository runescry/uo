"""
Extract actual spell class names from implemented spell files.
"""

import os
import re

SPELLS_PATH = r"C:\DevEnv\GIT\UO\ServUO\Scripts\Custom\VystiaClasses\Spells"

def extract_class_name(file_path):
    """Extract the class name from a spell .cs file."""
    with open(file_path, 'r', encoding='utf-8') as f:
        content = f.read()

    # Find public class declaration
    match = re.search(r'public class (\w+Spell)\s*:', content)
    if match:
        return match.group(1)
    return None

def main():
    spell_classes = {}  # {prefix: [class_names]}

    for school_dir in os.listdir(SPELLS_PATH):
        school_path = os.path.join(SPELLS_PATH, school_dir)

        if not os.path.isdir(school_path):
            continue

        print(f"Scanning {school_dir}/...")
        classes = []

        for filename in os.listdir(school_path):
            if not filename.endswith(".cs"):
                continue

            file_path = os.path.join(school_path, filename)
            class_name = extract_class_name(file_path)

            if class_name:
                classes.append(class_name)

        if classes:
            # Sort alphabetically
            classes.sort()
            spell_classes[school_dir] = classes
            print(f"  Found {len(classes)} spell classes")

    # Write output
    output_path = r"C:\DevEnv\GIT\UO\Vystia\ACTUAL_SPELL_CLASS_NAMES.txt"
    with open(output_path, 'w', encoding='utf-8') as f:
        f.write("# Actual Spell Class Names Extracted from Source Files\n\n")

        for school, classes in sorted(spell_classes.items()):
            f.write(f"## {school}\n")
            for class_name in classes:
                f.write(f"  {class_name}\n")
            f.write(f"\n")

    print(f"\nOutput written to: {output_path}")

if __name__ == "__main__":
    main()
