"""
Clear statics files completely and place Multi 0x64
Run this AFTER closing CentrED and ServUO
"""

import shutil
from pathlib import Path
import sys

# Add current directory to path for imports
sys.path.append(str(Path(__file__).parent))
from place_multi_from_file import place_multi_centred_style

print("="*60)
print("CLEAN PLACEMENT OF MULTI 0x64")
print("="*60)
print()
print("WARNING: This will clear all statics!")
print("Make sure CentrED and ServUO are CLOSED")
print()

# Paths
clean_statics = Path(r'C:\DevEnv\GIT\UO\UOL 1.5\Statics0.mul')
clean_staidx = Path(r'C:\DevEnv\GIT\UO\UOL 1.5\StaIdx0.mul')

target_statics = Path(r'C:\Program Files (x86)\Electronic Arts\Ultima Online Classic\statics0.mul')
target_staidx = Path(r'C:\Program Files (x86)\Electronic Arts\Ultima Online Classic\staidx0.mul')

centred_statics = Path(r'C:\DevEnv\GIT\UO\ServUO-fresh\Data\Maps\statics0.mul')
centred_staidx = Path(r'C:\DevEnv\GIT\UO\ServUO-fresh\Data\Maps\staidx0.mul')

# Step 1: Copy clean files
print("Step 1: Copying clean statics files...")
try:
    shutil.copy(clean_statics, target_statics)
    shutil.copy(clean_staidx, target_staidx)
    print("  OK - Copied to ClassicUO location")
except Exception as e:
    print(f"  ERROR: {e}")
    print("  Make sure CentrED and ServUO are CLOSED!")
    exit(1)

try:
    shutil.copy(clean_statics, centred_statics)
    shutil.copy(clean_staidx, centred_staidx)
    print("  OK - Copied to CentrED location")
except Exception as e:
    print(f"  ERROR: {e}")
    exit(1)

print()

# Step 2: Place Multi 0x64
print("Step 2: Placing Multi 0x64...")
print()

place_multi_centred_style(
    multi_file=r'C:\DevEnv\GIT\UO\Vystia Town Generator\brick_house_7x7.txt',
    world_x=1750,
    world_y=800,
    world_z=0
)

# Step 3: Copy to CentrED
print()
print("Step 3: Copying to CentrED location...")
shutil.copy(target_statics, centred_statics)
shutil.copy(target_staidx, centred_staidx)
print("  OK - Copied")

print()
print("="*60)
print("COMPLETE!")
print("="*60)
print()
print("You can now start CentrED and ServUO")
print("Navigate to (1750, 800) to see Multi 0x64")
print()
