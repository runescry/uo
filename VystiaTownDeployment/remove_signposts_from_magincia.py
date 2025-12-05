"""
Remove Signposts from New Magincia Decoration Files
Removes all signpost entries from Magincia decoration config files as a test.
"""

import re
from pathlib import Path
import shutil
from datetime import datetime

# Signpost Static IDs to remove
SIGNPOST_IDS = {
    '0x0BA0',  # Metal signpost
    '0x0B9D',  # Metal signpost variant
    '0x0BA2',  # Metal signpost variant
    '2978',    # Signpost
    '2977',    # Signpost
    '2971',    # Signpost
    '2970',    # Signpost
}

def remove_signposts_from_file(file_path: Path, backup: bool = True):
    """
    Remove all signpost entries from a decoration config file.
    
    Args:
        file_path: Path to the .cfg file
        backup: Whether to create a backup before modifying
    """
    if not file_path.exists():
        print(f"ERROR: File not found: {file_path}")
        return False
    
    # Create backup
    if backup:
        backup_path = file_path.parent / f"{file_path.stem}_backup_{datetime.now().strftime('%Y%m%d_%H%M%S')}.cfg"
        shutil.copy2(file_path, backup_path)
        print(f"  Created backup: {backup_path.name}")
    
    # Read file
    with open(file_path, 'r', encoding='utf-8') as f:
        lines = f.readlines()
    
    # Process lines to remove signposts
    cleaned_lines = []
    skip_next_static = False
    removed_count = 0
    
    i = 0
    while i < len(lines):
        line = lines[i]
        stripped = line.strip().lower()
        
        # Check if this is a signpost comment
        if 'signpost' in stripped and stripped.startswith('#'):
            # Skip this comment line
            removed_count += 1
            i += 1
            
            # Check if next line is a Static entry
            if i < len(lines) and lines[i].strip().startswith('Static'):
                static_line = lines[i].strip()
                # Extract Static ID
                static_match = re.search(r'Static\s+(\S+)', static_line)
                if static_match:
                    static_id = static_match.group(1)
                    # Check if it's a signpost ID
                    if static_id in SIGNPOST_IDS or static_id.upper() in [s.upper() for s in SIGNPOST_IDS]:
                        # Skip this Static line and all coordinate lines that follow
                        removed_count += 1
                        i += 1
                        
                        # Skip coordinate lines (lines that are just numbers)
                        while i < len(lines):
                            coord_line = lines[i].strip()
                            # Check if it's a coordinate line (3 numbers or 2 numbers)
                            if re.match(r'^-?\d+\s+-?\d+(\s+-?\d+)?$', coord_line):
                                removed_count += 1
                                i += 1
                            elif coord_line == '':
                                # Empty line - keep it and break
                                break
                            else:
                                # Not a coordinate, break
                                break
                        continue
            
            # Continue to next iteration
            continue
        
        # Check if this is a Static line with a signpost ID
        if line.strip().startswith('Static'):
            static_match = re.search(r'Static\s+(\S+)', line.strip())
            if static_match:
                static_id = static_match.group(1)
                if static_id in SIGNPOST_IDS or static_id.upper() in [s.upper() for s in SIGNPOST_IDS]:
                    # Skip this Static line
                    removed_count += 1
                    i += 1
                    
                    # Skip coordinate lines that follow
                    while i < len(lines):
                        coord_line = lines[i].strip()
                        if re.match(r'^-?\d+\s+-?\d+(\s+-?\d+)?$', coord_line):
                            removed_count += 1
                            i += 1
                        elif coord_line == '':
                            break
                        else:
                            break
                    continue
        
        # Keep this line
        cleaned_lines.append(line)
        i += 1
    
    # Write cleaned file
    with open(file_path, 'w', encoding='utf-8') as f:
        f.writelines(cleaned_lines)
    
    print(f"  Removed {removed_count} signpost-related lines")
    return True

def main():
    print("="*80)
    print("REMOVE SIGNPOSTS FROM NEW MAGINCIA DECORATION FILES")
    print("="*80)
    
    # Paths to decoration files
    script_dir = Path(__file__).parent.parent
    trammel_file = script_dir / "ServUO" / "Data" / "Magincia" / "Trammel" / "Decoration.cfg"
    felucca_file = script_dir / "ServUO" / "Data" / "Magincia" / "Felucca" / "Decoration.cfg"
    
    print(f"\nTrammel file: {trammel_file}")
    print(f"Felucca file: {felucca_file}")
    
    success = True
    
    # Process Trammel file
    if trammel_file.exists():
        print(f"\n[1/2] Processing Trammel decoration file...")
        if not remove_signposts_from_file(trammel_file):
            success = False
    else:
        print(f"\n[1/2] ERROR: Trammel file not found!")
        success = False
    
    # Process Felucca file
    if felucca_file.exists():
        print(f"\n[2/2] Processing Felucca decoration file...")
        if not remove_signposts_from_file(felucca_file):
            success = False
    else:
        print(f"\n[2/2] ERROR: Felucca file not found!")
        success = False
    
    print("\n" + "="*80)
    if success:
        print("COMPLETE!")
        print("="*80)
        print("\nAll signpost entries have been removed from the decoration files.")
        print("Backups have been created with timestamps.")
        print("\nTo test:")
        print("1. Restart your server")
        print("2. Run: [DecorateOldMagincia")
        print("3. Check that signposts are no longer spawned")
    else:
        print("ERROR: Some files could not be processed!")
        print("="*80)

if __name__ == "__main__":
    main()

