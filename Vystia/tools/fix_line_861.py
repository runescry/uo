#!/usr/bin/env python3
"""Fix line 861 in VystiaQuestEditorGump.cs"""

from pathlib import Path

file_path = Path(r'D:\UO\ServUO\Scripts\Custom\VystiaClasses\Gumps\VystiaQuestEditorGump.cs')

with open(file_path, 'r', encoding='utf-8') as f:
    content = f.read()

# Fix the specific line
content = content.replace(
    'waypoint.AssignedNPCSerial = Serial.MinusOne.Value;',
    'waypoint.AssignedNPCSerial = -1;'
)

with open(file_path, 'w', encoding='utf-8') as f:
    f.write(content)

print("+ Fixed line 861")
