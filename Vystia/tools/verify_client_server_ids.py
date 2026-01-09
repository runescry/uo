#!/usr/bin/env python3
"""
Verify client spell IDs match server spell IDs
"""

import re
from pathlib import Path

ROOT = Path(r"D:\UO")
CLIENT_DIR = ROOT / "ClassicUO/src/ClassicUO.Client/Game/Data"
SERVER_FILE = ROOT / "ServUO/Scripts/Custom/VystiaClasses/Spells/VystiaSpellInitializer.cs"

# Expected ID ranges
EXPECTED_RANGES = {
    "Ice": (1000, 1031),
    "Nature": (1032, 1063),
    "Hex": (1064, 1095),
    "Elemental": (1096, 1127),
    "Dark": (1128, 1159),
    "Divination": (1160, 1191),
    "Necromancy": (1192, 1223),
    "Summoning": (1224, 1255),
    "Shamanic": (1256, 1287),
    "Bardic": (1288, 1319),
    "Enchanting": (1320, 1351),
    "Illusion": (1352, 1383),
}

def extract_server_ids():
    """Extract spell IDs from server file"""
    server_ids = {}
    
    with open(SERVER_FILE, 'r', encoding='utf-8') as f:
        content = f.read()
    
    # Pattern: Register(ID, typeof(SpellName))
    pattern = r'Register\((\d+),\s*typeof\((\w+Spell)\)\)'
    
    for match in re.finditer(pattern, content):
        spell_id = int(match.group(1))
        spell_name = match.group(2)
        server_ids[spell_id] = spell_name
    
    return server_ids

def extract_client_ids(client_file):
    """Extract spell IDs from client file"""
    client_ids = {}
    
    if not client_file.exists():
        return client_ids
    
    with open(client_file, 'r', encoding='utf-8') as f:
        content = f.read()
    
    # Pattern: "Spell Name", ID, // Server ID
    pattern = r'"([^"]+)",\s*(\d+)\s*,\s*//\s*Server\s*ID\s*(\d+)'
    
    for match in re.finditer(pattern, content):
        spell_name = match.group(1)
        client_id = int(match.group(2))
        server_id = int(match.group(3))
        client_ids[client_id] = (spell_name, server_id)
    
    return client_ids

def main():
    """Main verification"""
    print("Verifying client-server spell ID alignment...\n")
    
    server_ids = extract_server_ids()
    print(f"Found {len(server_ids)} server spell registrations\n")
    
    all_match = True
    
    for school_name, (start_id, end_id) in EXPECTED_RANGES.items():
        # Find client file
        if school_name == "Ice":
            client_file = CLIENT_DIR / "SpellsVystiaIceMagic.cs"
        elif school_name == "Necromancy":
            client_file = CLIENT_DIR / "SpellsVystiaNecromancy.cs"
        else:
            client_file = CLIENT_DIR / f"SpellsVystia{school_name}.cs"
        
        if not client_file.exists():
            print(f"⚠️  {school_name}: Client file not found: {client_file.name}")
            all_match = False
            continue
        
        client_ids = extract_client_ids(client_file)
        
        # Check each ID in range
        issues = []
        for server_id in range(start_id, end_id + 1):
            if server_id not in server_ids:
                issues.append(f"  Server ID {server_id} not found in server file")
                all_match = False
            else:
                # Check if client has matching ID
                found = False
                for client_id, (name, mapped_server_id) in client_ids.items():
                    if mapped_server_id == server_id:
                        found = True
                        break
                
                if not found:
                    issues.append(f"  Server ID {server_id} ({server_ids[server_id]}) not found in client file")
                    all_match = False
        
        if issues:
            print(f"[X] {school_name} ({start_id}-{end_id}): {len(issues)} issues")
            for issue in issues[:5]:  # Show first 5
                print(issue)
            if len(issues) > 5:
                print(f"  ... and {len(issues) - 5} more")
        else:
            print(f"[OK] {school_name} ({start_id}-{end_id}): All IDs match")
    
    print("\n" + "="*60)
    if all_match:
        print("[OK] All client files match server IDs!")
    else:
        print("[WARN] Some mismatches found. Review above.")
    
    return all_match

if __name__ == "__main__":
    main()

