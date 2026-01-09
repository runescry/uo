#!/usr/bin/env python3
"""
Fix final build errors - namespace doubling and Serial.MinusOne
"""

from pathlib import Path

def main():
    file_path = Path(r'D:\UO\ServUO\Scripts\Custom\VystiaClasses\Gumps\VystiaQuestEditorGump.cs')

    with open(file_path, 'r', encoding='utf-8') as f:
        content = f.read()

    # Fix doubled namespace
    content = content.replace(
        'Server.Custom.VystiaClasses.Quests.Server.Custom.VystiaClasses.Quests.',
        'Server.Custom.VystiaClasses.Quests.'
    )

    # Fix any remaining Serial.MinusOne that isn't already .Value
    # But don't replace Serial.MinusOne.Value with Serial.MinusOne.Value.Value
    lines = content.split('\n')
    fixed_lines = []
    for line in lines:
        if 'Serial.MinusOne' in line and 'Serial.MinusOne.Value' not in line:
            line = line.replace('Serial.MinusOne', 'Serial.MinusOne.Value')
        fixed_lines.append(line)
    content = '\n'.join(fixed_lines)

    with open(file_path, 'w', encoding='utf-8') as f:
        f.write(content)

    print(f"+ Fixed {file_path.name}")

if __name__ == '__main__':
    main()
