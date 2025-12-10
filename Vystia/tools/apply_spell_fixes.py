"""
Apply corrected spell registrations to VystiaSpellInitializer.cs
"""

import re

INITIALIZER_PATH = r"C:\DevEnv\GIT\UO\ServUO\Scripts\Custom\VystiaClasses\Spells\VystiaSpellInitializer.cs"
CORRECTIONS_PATH = r"C:\DevEnv\GIT\UO\Vystia\CORRECTED_SPELL_REGISTRATIONS.txt"

# Methods to replace (in order they appear in file)
METHODS_TO_REPLACE = [
    "RegisterWitchSpells",
    "RegisterSorcererSpells",
    "RegisterWarlockSpells",
    "RegisterOracleSpells",
    "RegisterNecromancerSpells",
    "RegisterSummonerSpells",
    "RegisterShamanSpells",
    "RegisterBardSpells",
    "RegisterEnchanterSpells",
    "RegisterIllusionistSpells",
]

def extract_method_from_corrections(method_name, corrections_text):
    """Extract a single method's corrected code from the corrections file."""
    # Find the method in the corrections
    pattern = rf"(        private static void {method_name}\(\).*?\n        \}})"
    match = re.search(pattern, corrections_text, re.DOTALL)

    if match:
        return match.group(1)
    else:
        return None

def replace_method_in_file(file_content, method_name, new_method_code):
    """Replace a method in the file with new code."""
    # Find existing method
    pattern = rf"(        private static void {method_name}\(\).*?\n        \}})"
    match = re.search(pattern, file_content, re.DOTALL)

    if match:
        old_code = match.group(1)
        # Replace old with new
        new_content = file_content.replace(old_code, new_method_code)
        return new_content, True
    else:
        return file_content, False

def main():
    print("=" * 80)
    print("APPLYING SPELL REGISTRATION FIXES TO VystiaSpellInitializer.cs")
    print("=" * 80)
    print()

    # Read corrections file
    print("Reading corrections file...")
    with open(CORRECTIONS_PATH, 'r', encoding='utf-8') as f:
        corrections_text = f.read()
    print(f"  [OK] Loaded corrections ({len(corrections_text)} chars)")
    print()

    # Read original file
    print("Reading VystiaSpellInitializer.cs...")
    with open(INITIALIZER_PATH, 'r', encoding='utf-8') as f:
        file_content = f.read()
    print(f"  [OK] Loaded file ({len(file_content)} chars)")
    print()

    # Apply each fix
    fixes_applied = 0
    for i, method_name in enumerate(METHODS_TO_REPLACE, 1):
        print(f"{i}/10 Applying fix for {method_name}...")

        # Extract corrected method
        new_method = extract_method_from_corrections(method_name, corrections_text)
        if not new_method:
            print(f"  [ERROR] Could not find {method_name} in corrections file!")
            continue

        # Replace in file
        file_content, success = replace_method_in_file(file_content, method_name, new_method)
        if success:
            print(f"  [OK] Replaced {method_name}")
            fixes_applied += 1
        else:
            print(f"  [ERROR] Could not find {method_name} in original file!")

    print()

    # Write updated file
    if fixes_applied > 0:
        print(f"Writing updated file ({fixes_applied} methods fixed)...")
        with open(INITIALIZER_PATH, 'w', encoding='utf-8') as f:
            f.write(file_content)
        print(f"  [OK] File updated successfully!")
    else:
        print("[ERROR] No fixes applied - file not updated")

    print()
    print("=" * 80)
    print(f"COMPLETE! {fixes_applied}/10 methods fixed")
    print("=" * 80)
    print()
    print("NEXT STEPS:")
    print("1. Rebuild ServUO server: dotnet build")
    print("2. Test all 12 spellbooks in-game")

if __name__ == "__main__":
    main()
