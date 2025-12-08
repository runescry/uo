#!/usr/bin/env python3
import re
from pathlib import Path

initializer_path = Path("C:/DevEnv/GIT/UO/ServUO/Scripts/Custom/VystiaClasses/Spells/VystiaSpellInitializer.cs")

with open(initializer_path, 'r', encoding='utf-8') as f:
    content = f.read()

schools = ['IceMage', 'Druid', 'Witch', 'Sorcerer', 'Warlock', 'Oracle', 'Necromancer', 'Summoner', 'Shaman', 'Bard', 'Enchanter', 'Illusionist']

total = 0
for school in schools:
    pattern = f'private static void Register{school}Spells\\(\\).*?^        \\}}'
    match = re.search(pattern, content, re.DOTALL | re.MULTILINE)
    if match:
        count = len(re.findall(r'Register\(\d+', match.group(0)))
        print(f'{school}: {count} spells')
        total += count
    else:
        print(f'{school}: NOT FOUND')

print(f'\nTotal: {total} spells')
