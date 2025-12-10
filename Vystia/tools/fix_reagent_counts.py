#!/usr/bin/env python3
"""Fix reagent counts from 82 to 96 across all documentation."""

import re
import os

FIXES = [
    # Pattern, Replacement
    (r'82 unique Vystia reagents', '96 Vystia reagents (8 per school)'),
    (r'82 custom Vystia reagents', '96 custom Vystia reagents (8 per school)'),
    (r'All 82 custom reagents', 'All 96 custom reagents (8 per school)'),
    (r'All 82 reagents', 'All 96 reagents (8 per school)'),
    (r'Magic Reagents \(82\)', 'Magic Reagents (96)'),
    (r'\| \*\*Magic Reagents\*\* \| ✅ 100% \| 82/82 \|', '| **Magic Reagents** | ✅ 100% | 96/96 |'),
    (r'All 82 Vystia magic reagents', 'All 96 Vystia magic reagents'),
]

FILES_TO_FIX = [
    r'C:\DevEnv\GIT\UO\Vystia\Magic\README.md',
    r'C:\DevEnv\GIT\UO\Vystia\README.md',
    r'C:\DevEnv\GIT\UO\Vystia\SPELLBOOK_SYSTEM_COMPLETE.md',
    r'C:\DevEnv\GIT\UO\Vystia\VYSTIA_MASTER_INVENTORY.md',
    r'C:\DevEnv\GIT\UO\CLAUDE.md',
]

def fix_file(filepath):
    """Apply all fixes to a file."""
    if not os.path.exists(filepath):
        print(f"[SKIP] {filepath} not found")
        return False

    with open(filepath, 'r', encoding='utf-8') as f:
        content = f.read()

    original = content
    for pattern, replacement in FIXES:
        content = re.sub(pattern, replacement, content)

    if content != original:
        with open(filepath, 'w', encoding='utf-8') as f:
            f.write(content)
        print(f"[OK] Fixed {filepath}")
        return True
    else:
        print(f"[SKIP] No changes needed in {filepath}")
        return False

def main():
    print("Fixing reagent counts (82 -> 96)...")
    print("=" * 60)

    fixed = 0
    for filepath in FILES_TO_FIX:
        if fix_file(filepath):
            fixed += 1

    print("=" * 60)
    print(f"[DONE] Fixed {fixed}/{len(FILES_TO_FIX)} files")

if __name__ == "__main__":
    main()
