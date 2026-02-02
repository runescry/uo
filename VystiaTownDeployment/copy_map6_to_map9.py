"""
Copy map6 from CentrED to map9 in UO directory
"""

import shutil
from pathlib import Path

# Source: CentrED map6 files
SOURCE_MAP = Path(r'D:\UO\centredsharp\Output-TerrainTest\editable_maps\map6.mul')
SOURCE_STATICS = Path(r'D:\UO\centredsharp\Output-TerrainTest\editable_maps\statics6.mul')
SOURCE_STAIDX = Path(r'D:\UO\centredsharp\Output-TerrainTest\editable_maps\staidx6.mul')

# Target: UO directory map9 files
TARGET_MAP = Path(r'C:\Program Files (x86)\Electronic Arts\Ultima Online Classic\map9.mul')
TARGET_STATICS = Path(r'C:\Program Files (x86)\Electronic Arts\Ultima Online Classic\statics9.mul')
TARGET_STAIDX = Path(r'C:\Program Files (x86)\Electronic Arts\Ultima Online Classic\staidx9.mul')

print("="*80)
print("COPY MAP6 TO MAP9")
print("="*80)

# Verify source files exist
print("\nChecking source files...")
if not SOURCE_MAP.exists():
    print(f"ERROR: Source map file not found: {SOURCE_MAP}")
    exit(1)
if not SOURCE_STATICS.exists():
    print(f"ERROR: Source statics file not found: {SOURCE_STATICS}")
    exit(1)
if not SOURCE_STAIDX.exists():
    print(f"ERROR: Source staidx file not found: {SOURCE_STAIDX}")
    exit(1)

print(f"  Source map: {SOURCE_MAP}")
print(f"  Source statics: {SOURCE_STATICS}")
print(f"  Source staidx: {SOURCE_STAIDX}")

# Verify target directory exists
target_dir = TARGET_MAP.parent
if not target_dir.exists():
    print(f"\nERROR: Target directory not found: {target_dir}")
    exit(1)

print(f"\nTarget directory: {target_dir}")

# Copy files
print("\nCopying files...")
try:
    shutil.copy2(SOURCE_MAP, TARGET_MAP)
    print(f"  Copied: map6.mul -> map9.mul")
    
    shutil.copy2(SOURCE_STATICS, TARGET_STATICS)
    print(f"  Copied: statics6.mul -> statics9.mul")
    
    shutil.copy2(SOURCE_STAIDX, TARGET_STAIDX)
    print(f"  Copied: staidx6.mul -> staidx9.mul")
    
    print("\n" + "="*80)
    print("COPY COMPLETE")
    print("="*80)
    print(f"Map6 files copied to map9 in: {target_dir}")
    print("\nNote: Restart UO client for map9 to be recognized")
    
except Exception as e:
    print(f"\nERROR: Failed to copy files: {e}")
    exit(1)
