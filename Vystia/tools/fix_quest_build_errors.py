#!/usr/bin/env python3
"""
Fix all build errors in the quest waypoint system
"""

import re
from pathlib import Path

def fix_file(file_path, replacements):
    """Apply replacements to a file"""
    print(f"Fixing {file_path.name}...")

    with open(file_path, 'r', encoding='utf-8') as f:
        content = f.read()

    original = content

    for pattern, replacement in replacements:
        content = re.sub(pattern, replacement, content)

    if content != original:
        with open(file_path, 'w', encoding='utf-8') as f:
            f.write(content)
        print(f"  + Fixed {file_path.name}")
        return True
    else:
        print(f"  - No changes needed for {file_path.name}")
        return False

def main():
    base_path = Path(r'D:\UO\ServUO\Scripts\Custom\VystiaClasses')

    # Fix AddQuestNPCCommand.cs - missing using directive
    cmd_file = base_path / 'Commands' / 'AddQuestNPCCommand.cs'
    fix_file(cmd_file, [
        (r'using Server\.Gumps;',
         'using Server.Gumps;\nusing Server.Services.LLM;'),
    ])

    # Fix AddQuestNPCGump.cs - switch expressions and Serial.MinusOne
    gump_file = base_path / 'Gumps' / 'AddQuestNPCGump.cs'

    # Read the file first
    with open(gump_file, 'r', encoding='utf-8') as f:
        content = f.read()

    # Fix switch expression for waypoint type colors (line ~219)
    content = re.sub(
        r'int typeColor = waypoint\.Type switch\s*\{[^}]+\};',
        '''int typeColor;
                switch (waypoint.Type)
                {
                    case Server.Custom.VystiaClasses.Quests.WaypointType.Origin:
                        typeColor = 0x00FF00; // Green
                        break;
                    case Server.Custom.VystiaClasses.Quests.WaypointType.NPCCompletion:
                        typeColor = 0xFFD700; // Gold
                        break;
                    case Server.Custom.VystiaClasses.Quests.WaypointType.BossCompletion:
                        typeColor = 0xFF4500; // Red-orange
                        break;
                    default:
                        typeColor = LabelColor;
                        break;
                }''',
        content,
        flags=re.DOTALL
    )

    # Fix Serial.MinusOne references
    content = content.replace('waypoint.AssignedNPCSerial != Serial.MinusOne',
                              'waypoint.AssignedNPCSerial.Value != -1')
    content = content.replace('waypoint.AssignedNPCSerial == Serial.MinusOne',
                              'waypoint.AssignedNPCSerial.Value == -1')

    # Write back
    with open(gump_file, 'w', encoding='utf-8') as f:
        f.write(content)
    print(f"+ Fixed {gump_file.name}")

    # Fix VystiaQuestEditorGump.cs - switch expressions and Serial.MinusOne
    editor_file = base_path / 'Gumps' / 'VystiaQuestEditorGump.cs'

    # This file was already read in the previous session, but we need to fix it
    with open(editor_file, 'r', encoding='utf-8') as f:
        content = f.read()

    # Fix switch expression for waypoint type colors
    content = re.sub(
        r'int typeColor = waypoint\.Type switch\s*\{[^}]+\};',
        '''int typeColor;
                    switch (waypoint.Type)
                    {
                        case Server.Custom.VystiaClasses.Quests.WaypointType.Origin:
                            typeColor = 0x00FF00; // Green
                            break;
                        case Server.Custom.VystiaClasses.Quests.WaypointType.NPCCompletion:
                            typeColor = 0xFFD700; // Gold
                            break;
                        case Server.Custom.VystiaClasses.Quests.WaypointType.BossCompletion:
                            typeColor = 0xFF4500; // Red-orange
                            break;
                        default:
                            typeColor = LabelColor;
                            break;
                    }''',
        content,
        flags=re.DOTALL
    )

    # Fix Serial.MinusOne references
    content = content.replace('waypoint.AssignedNPCSerial != Serial.MinusOne',
                              'waypoint.AssignedNPCSerial.Value != -1')
    content = content.replace('waypoint.AssignedNPCSerial == Serial.MinusOne',
                              'waypoint.AssignedNPCSerial.Value == -1')

    # Fix WaypointType enum references without full namespace
    content = content.replace('WaypointType.Origin',
                              'Server.Custom.VystiaClasses.Quests.WaypointType.Origin')
    content = content.replace('WaypointType.Waypoint',
                              'Server.Custom.VystiaClasses.Quests.WaypointType.Waypoint')
    content = content.replace('WaypointType.NPCCompletion',
                              'Server.Custom.VystiaClasses.Quests.WaypointType.NPCCompletion')
    content = content.replace('WaypointType.BossCompletion',
                              'Server.Custom.VystiaClasses.Quests.WaypointType.BossCompletion')

    # Fix the cast in OnResponse
    content = content.replace('(WaypointType)(buttonID - 700)',
                              '(Server.Custom.VystiaClasses.Quests.WaypointType)(buttonID - 700)')

    # Write back
    with open(editor_file, 'w', encoding='utf-8') as f:
        f.write(content)
    print(f"+ Fixed {editor_file.name}")

    print("\n+ All files fixed!")

if __name__ == '__main__':
    main()
