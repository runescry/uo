"""
Shift all spell registration IDs down by 1 for schools 2-12.
Ice Magic (999-1030) is already correct, don't touch it.
"""

import re

FILE_PATH = r"C:\DevEnv\GIT\UO\ServUO\Scripts\Custom\VystiaClasses\Spells\VystiaSpellInitializer.cs"

def shift_ids(content):
    """Shift all Register() IDs from 1031-1382 down by 1."""
    def replace_id(match):
        old_id = int(match.group(1))
        if old_id >= 1031:  # Only shift IDs 1031 and above (all non-Ice schools)
            new_id = old_id - 1
            return f"Register({new_id},"
        return match.group(0)  # Keep Ice Magic IDs unchanged

    # Pattern: Register(ID, where ID is a number
    pattern = r'Register\((\d+),'
    new_content = re.sub(pattern, replace_id, content)

    return new_content

def main():
    print("Shifting spell registration IDs down by 1...")

    with open(FILE_PATH, 'r', encoding='utf-8') as f:
        content = f.read()

    new_content = shift_ids(content)

    with open(FILE_PATH, 'w', encoding='utf-8') as f:
        f.write(new_content)

    print("[OK] All IDs shifted successfully!")
    print("\nNext: Rebuild ServUO server with 'dotnet build'")

if __name__ == "__main__":
    main()
