#!/usr/bin/env python3
"""
Set all Vystia classes to have maxed starting stats (100/100/100)
"""

import os
import re

CLASSES_DIR = r"D:\UO\ServUO\Scripts\Custom\VystiaClasses\Classes"

def update_class_stats(file_path):
    """Update a class file to have 100/100/100 starting stats"""
    with open(file_path, 'r', encoding='utf-8') as f:
        content = f.read()

    # Update StartStr
    content = re.sub(
        r'public override int StartStr => \d+;',
        'public override int StartStr => 100;',
        content
    )

    # Update StartDex
    content = re.sub(
        r'public override int StartDex => \d+;',
        'public override int StartDex => 100;',
        content
    )

    # Update StartInt
    content = re.sub(
        r'public override int StartInt => \d+;',
        'public override int StartInt => 100;',
        content
    )

    # Update stats comment if it exists
    content = re.sub(
        r'// Stats \(Total: \d+\)',
        '// Stats (Total: 300)',
        content
    )

    with open(file_path, 'w', encoding='utf-8') as f:
        f.write(content)

    print(f"Updated: {os.path.basename(file_path)}")

def main():
    """Update all class files"""
    class_files = [
        f for f in os.listdir(CLASSES_DIR)
        if f.endswith('Class.cs')
    ]

    print(f"Found {len(class_files)} class files")

    for class_file in class_files:
        file_path = os.path.join(CLASSES_DIR, class_file)
        update_class_stats(file_path)

    print(f"\nUpdated {len(class_files)} classes to have 100/100/100 stats!")

if __name__ == "__main__":
    main()
