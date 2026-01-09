#!/usr/bin/env python3
"""
Create martial class design files with current state vs future state
"""

from pathlib import Path

ROOT = Path(r"D:\UO")
MARTIAL_DIR = ROOT / "Vystia/design/martial"

# Class definitions with ability IDs and archetypes
CLASSES = [
    {"name": "Fighter", "ids": "2000-2015", "archetype": "Melee DPS", "theme": "Weapon master, stance dancing"},
    {"name": "Barbarian", "ids": "2016-2031", "archetype": "Burst Melee", "theme": "Rage-fueled berserker"},
    {"name": "Monk", "ids": "2032-2047", "archetype": "Combo DPS", "theme": "Disciplined martial artist"},
    {"name": "Rogue", "ids": "2048-2063", "archetype": "Burst/Utility", "theme": "Shadow operative"},
    {"name": "Ranger", "ids": "2064-2079", "archetype": "Ranged/Hybrid", "theme": "Wilderness hunter"},
    {"name": "Knight", "ids": "2080-2095", "archetype": "Tank", "theme": "Stalwart defender"},
    {"name": "Paladin", "ids": "2096-2111", "archetype": "Tank/Support", "theme": "Holy warrior"},
    {"name": "Templar", "ids": "2112-2127", "archetype": "Burst DPS", "theme": "Zealous inquisitor"},
    {"name": "BountyHunter", "ids": "2128-2143", "archetype": "Hunter/Debuff", "theme": "Relentless tracker"},
    {"name": "Beastmaster", "ids": "2144-2159", "archetype": "Pet DPS", "theme": "Pack alpha"},
    {"name": "Artificer", "ids": "2160-2175", "archetype": "Gadget DPS", "theme": "Combat engineer"},
    {"name": "Alchemist", "ids": "2176-2191", "archetype": "Burst/Utility", "theme": "Battle chemist"},
    {"name": "Cleric", "ids": "2192-2207", "archetype": "Healer/Support", "theme": "Divine healer"},
    {"name": "Wizard", "ids": "2208-2223", "archetype": "Flex Caster", "theme": "Arcane generalist"},
]

# Archetype-based recommendations
ARCHETYPE_NEEDS = {
    "Melee DPS": {
        "mobility": "Charge, leap, dash",
        "defense": "Second wind, parry stance, damage reduction",
        "utility": "Battle cry, rally, weapon swap"
    },
    "Burst Melee": {
        "mobility": "Furious charge, leap, dash",
        "defense": "Berserker's rage immunity, damage reduction",
        "utility": "Intimidating shout, battle cry"
    },
    "Combo DPS": {
        "mobility": "Flying kick, teleport, dash",
        "defense": "Meditation, dodge, chi shield",
        "utility": "Healing touch, chi burst"
    },
    "Burst/Utility": {
        "mobility": "Shadow step, roll, dash",
        "defense": "Evasion, vanish, dodge",
        "utility": "Detect traps, pick lock, stealth"
    },
    "Ranged/Hybrid": {
        "mobility": "Disengage, roll, dash",
        "defense": "Feign death, camouflage, dodge",
        "utility": "Tracking, trap detection, terrain mastery"
    },
    "Tank": {
        "mobility": "Shield charge, intercept, dash",
        "defense": "Shield wall, damage reduction, taunt",
        "utility": "Rallying cry, group support, threat generation"
    },
    "Tank/Support": {
        "mobility": "Divine charge, dash",
        "defense": "Divine protection, shield, aura",
        "utility": "Aura of protection, blessing, detect evil"
    },
    "Burst DPS": {
        "mobility": "Zeal charge, divine leap, dash",
        "defense": "Divine protection, judgment shield",
        "utility": "Detect heresy, smite, judgment"
    },
    "Hunter/Debuff": {
        "mobility": "Pursuit dash, track teleport, dash",
        "defense": "Evasion, dodge, mark defense",
        "utility": "Detect target, trap setting, tracking"
    },
    "Pet DPS": {
        "mobility": "Pack charge, beast form, dash",
        "defense": "Pet tank, pack defense, bond shield",
        "utility": "Pet teleport, pet heal, pack coordination"
    },
    "Gadget DPS": {
        "mobility": "Steam dash, teleport device, dash",
        "defense": "Shield generator, barrier, turret defense",
        "utility": "Detect traps, repair, gadget management"
    },
    "Burst/Utility": {
        "mobility": "Alchemical dash, potion of speed, dash",
        "defense": "Potion of protection, alchemical shield",
        "utility": "Detect poison, alchemical sight, mutagen"
    },
    "Healer/Support": {
        "mobility": "Divine step, teleport, dash",
        "defense": "Divine shield, protection, aura",
        "utility": "Mass heal, blessing, detect undead, purify"
    },
    "Flex Caster": {
        "mobility": "Arcane step, teleport, dash",
        "defense": "Arcane shield, counterspell, barrier",
        "utility": "Detect magic, identify, metamagic"
    }
}

def create_class_file(class_info):
    """Create a martial class design file"""
    filename = f"{class_info['name']}.md"
    file_path = MARTIAL_DIR / filename
    
    if file_path.exists():
        print(f"  [SKIP] {class_info['name']}: File already exists")
        return
    
    needs = ARCHETYPE_NEEDS.get(class_info['archetype'], {})
    
    content = f"""# {class_info['name']} - Current State vs Future State

**Class:** {class_info['name']}  
**Ability IDs:** {class_info['ids']}  
**Archetype:** {class_info['archetype']}  
**Theme:** {class_info['theme']}

---

## Current State

### Ability Distribution (16 abilities)
- **Damage:** ~X abilities (X%) - *Needs verification*
- **Utility:** ~X abilities (X%) - *Needs verification*
- **Defense:** ~X abilities (X%) - *Needs verification*
- **Mobility:** ~X abilities (X%) - *Needs verification*

### Core Mechanics
- *[To be documented from implementation files]*

### Strengths
- ✅ *[To be assessed]*

### Weaknesses
- ⚠️ *[To be assessed]*

---

## Future State Proposals

**Date:** 2025-12-13  
**Source:** Industry archetype standards (`Vystia/admin/ARCHETYPE_BALANCE_ANALYSIS.md`)

### Target Distribution
Based on **{class_info['archetype']}** archetype standards:
- **Damage:** Industry standard for {class_info['archetype']}
- **Utility:** Industry standard for {class_info['archetype']}
- **Defense:** Industry standard for {class_info['archetype']}
- **Mobility:** Industry standard for {class_info['archetype']}

### Proposed New Abilities

#### Mobility Needs
**Recommended:** {needs.get('mobility', 'TBD')}

#### Defense Needs
**Recommended:** {needs.get('defense', 'TBD')}

#### Utility Needs
**Recommended:** {needs.get('utility', 'TBD')}

### Ability Replacements/Updates
- *[To be determined after current state analysis]*

### Implementation Priority
- **Phase 1 (Critical):** Mobility and defense abilities
- **Phase 2 (Important):** Utility abilities
- **Phase 3 (Enhancement):** Advanced mechanics

---

## Detailed Analysis Needed

1. **Current State Audit:** Analyze all 16 abilities to determine exact distribution
2. **Gap Analysis:** Compare against industry standards for {class_info['archetype']}
3. **Specific Proposals:** Create detailed ability proposals
4. **Implementation Plan:** Prioritize changes by impact

---

**Last Updated:** 2025-12-13  
**Status:** Template created, detailed analysis pending

"""
    
    with open(file_path, 'w', encoding='utf-8') as f:
        f.write(content)
    
    print(f"  [OK] {class_info['name']}: Created file")

def main():
    """Main function"""
    print("Creating martial class design files...\n")
    
    for class_info in CLASSES:
        create_class_file(class_info)
    
    print("\nDone!")

if __name__ == "__main__":
    main()

