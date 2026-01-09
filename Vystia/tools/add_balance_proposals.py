#!/usr/bin/env python3
"""
Add proposed balance changes sections to all magic school design files
"""

from pathlib import Path

ROOT = Path(r"D:\UO")
MAGIC_DIR = ROOT / "Vystia/design/magic"

# Proposed changes for each school (from ARCHETYPE_BALANCE_ANALYSIS.md)
PROPOSALS = {
    "DivinationMagic.md": {
        "current": "14 Damage, 0 Healing, 2 Defense, 1 Area, 1 Debuff",
        "target": "14 Damage, 1-2 Healing, 2 Defense, 3-4 Area, 3-4 Debuff",
        "spells": [
            "Circle 3-4: Temporal Heal - Rewind damage taken in last 10s, heals 15-30 HP",
            "Circle 4-5: Time Rewind - Undo last 5s of damage/healing, restore previous HP",
            "Circle 2-3: Haste - +30% movement speed, +20% attack speed for 20s",
            "Circle 3-4: Slow Time - -40% movement/attack speed for 15s",
            "Circle 4-5: Time Well - Area effect, slows enemies, speeds allies",
            "Circle 3-4: Temporal Sight - See future positions, +20% dodge for 10s"
        ]
    },
    "ShamanicMagic.md": {
        "current": "13 Damage, 1 Healing, 0 Defense, 0 Area, 0 Debuff",
        "target": "13 Damage, 2-3 Healing, 1 Defense, 3-4 Area, 2-3 Debuff",
        "spells": [
            "Circle 2-3: Healing Totem - 2-4 HP/tick to nearby allies, lasts 30s",
            "Circle 3-4: Chain Heal - Heals 3 targets, 20-35 HP each",
            "Circle 3-4: Earth Totem - +15 Physical Resist, taunts enemies",
            "Circle 2-3: Mana Spring Totem - +5 mana/tick to nearby allies",
            "Circle 3-4: Spirit Walk - Teleport 8 tiles, immune to slows during",
            "Circle 4-5: Totemic Recall - Instantly replace all totems at new location"
        ]
    },
    "BardicMagic.md": {
        "current": "14 Damage, 2 Healing, 2 Defense, 0 Area, 0 Debuff",
        "target": "14 Damage, 2 Healing, 2 Defense, 2-3 Area, 2-3 Debuff",
        "spells": [
            "Circle 2-3: Song of Speed - +25% movement speed to party, 30s",
            "Circle 3-4: Battle Hymn - +15% damage to party, 20s",
            "Circle 3-4: Song of Protection - +10 all resists to party, 25s",
            "Circle 4-5: Discordant Chorus - 4-tile radius, confuses enemies",
            "Circle 3-4: Marching Song - Party moves faster, immune to slows"
        ]
    },
    "EnchantingMagic.md": {
        "current": "18 Damage, 1 Healing, 3 Defense, 0 Area, 0 Debuff",
        "target": "12-15 Damage, 1-2 Healing, 5 Defense, 2-3 Area, 2-3 Debuff",
        "spells": [
            "Remove 3-6 damage spells (convert to utility/buffs)",
            "Circle 3-4: Rune of Fortitude - +20 all resists, 30s",
            "Circle 4-5: Rune of Warding - Absorbs 50 damage, reflects 25%",
            "Circle 2-3: Rune of Teleportation - Create teleport rune, 1 use",
            "Circle 3-4: Dispel Magic - Remove 1 buff from target",
            "Circle 4-5: Mass Dispel - Remove all buffs in 3-tile radius"
        ]
    },
    "IllusionMagic.md": {
        "current": "10 Damage, 0 Healing, 0 Defense, 1 Area, 0 Debuff",
        "target": "10 Damage, 0 Healing, 1 Defense, 3-4 Area, 3-4 Debuff",
        "spells": [
            "Circle 2-3: Mirror Image - Creates 1-2 clones, redirects attacks",
            "Circle 3-4: Invisibility - Stealth for 15s, next attack +50% damage",
            "Circle 2-3: Confuse - Target attacks random nearby for 10s",
            "Circle 3-4: Phantasmal Terrain - 4-tile radius, enemies see illusions",
            "Circle 4-5: Illusionary Teleport - Teleport, leaves decoy behind",
            "Circle 3-4: Mind Control - Control target for 5s (PvE only)"
        ]
    },
    "Necromancy.md": {
        "current": "11 Damage, 5 Healing, 0 Defense, 0 Area, 2 Debuff",
        "target": "11 Damage, 2-3 Healing, 1 Defense, 1-2 Area, 3-4 Debuff",
        "spells": [
            "Remove 2-3 direct healing spells (keep only life drain)",
            "Circle 3-4: Bone Armor - +20 Physical Resist, bone spikes reflect damage",
            "Circle 4-5: Corpse Explosion - Explode corpse, 30-50 damage in 3-tile radius",
            "Circle 3-4: Death Nova - 4-tile radius, 15-25 damage, spreads decay",
            "Circle 2-3: Detect Undead - See undead through walls, +20% damage vs undead",
            "Circle 3-4: Soul Bind - Link to corpse, can resurrect from it"
        ]
    },
    "SummoningMagic.md": {
        "current": "2 Damage, 0 Healing, 0 Defense, 1 Area, 0 Debuff",
        "target": "2 Damage, 1 Healing, 1 Defense, 2-3 Area, 2-3 Debuff",
        "spells": [
            "Circle 3-4: Mend Summon - Heals summoned creature 40-60 HP",
            "Circle 3-4: Summon Shield - Summoned creature takes damage for you",
            "Circle 2-3: Bond Teleport - Teleport to your summon's location",
            "Circle 3-4: Summon Swarm - Summon 3-5 weak creatures in area",
            "Circle 4-5: Summon Barrier - Summoned creature creates protective zone"
        ]
    },
    "DarkMagic.md": {
        "current": "10 Damage, 1 Healing, 0 Defense, 2 Area, 1 Debuff",
        "target": "10 Damage, 1-2 Healing, 1 Defense, 3-4 Area, 2-3 Debuff",
        "spells": [
            "Circle 3-4: Demon Skin - +20 all resists, +10% spell damage, 30s",
            "Circle 2-3: Shadow Step - Teleport 8 tiles through shadows",
            "Circle 3-4: Shadow Storm - 4-tile radius, 12-24 shadow damage",
            "Circle 2-3: Fear - Target flees for 5s, cannot attack",
            "Circle 3-4: Mass Fear - 3-tile radius, all enemies flee"
        ]
    },
    "NatureMagic.md": {
        "current": "9 Damage, 4 Healing, 5 Defense, 13 Area, 5 Debuff",
        "target": "9 Damage, 4 Healing, 5 Defense, 13 Area, 5 Debuff",
        "spells": [
            "✅ Well-balanced - No major changes needed",
            "Consider: Add 1 escape form (hawk form flight, bear form charge)"
        ]
    }
}

def add_proposal_section(file_path, school_name, proposal_data):
    """Add proposal section to a magic school file"""
    with open(file_path, 'r', encoding='utf-8') as f:
        content = f.read()
    
    # Check if section already exists
    if "## Proposed Balance Changes" in content:
        print(f"  [SKIP] {school_name}: Section already exists, skipping")
        return
    
    # Find the end of the file (before any existing "Last Updated" or status)
    end_markers = [
        "**Last Updated:**",
        "*Last Updated:*",
        "**Status:**",
        "*Status:*"
    ]
    
    insert_pos = len(content)
    for marker in end_markers:
        pos = content.rfind(marker)
        if pos != -1:
            insert_pos = min(insert_pos, pos)
    
    # Build proposal section
    section = f"""

---

## Proposed Balance Changes (Based on Archetype Analysis)

**Date:** 2025-12-13  
**Source:** `Vystia/admin/ARCHETYPE_BALANCE_ANALYSIS.md`

### Current State
- **Current:** {proposal_data['current']}

### Target State
- **Target:** {proposal_data['target']}

### Proposed New Spells

"""
    
    for spell in proposal_data['spells']:
        section += f"- **{spell}**\n"
    
    section += """
### Spell Replacements/Updates
- See individual spell proposals above

### Implementation Priority
- **Phase 1 (Critical):** Mobility/escape, defensive cooldowns
- **Phase 2 (Important):** Healing, utility spells
- **Phase 3 (Enhancement):** Area control, advanced utilities

"""
    
    # Insert before the last section
    new_content = content[:insert_pos] + section + content[insert_pos:]
    
    with open(file_path, 'w', encoding='utf-8') as f:
        f.write(new_content)
    
    print(f"  [OK] {school_name}: Added proposal section")

def main():
    """Main function"""
    print("Adding proposed balance changes to magic school files...\n")
    
    for filename, proposal_data in PROPOSALS.items():
        file_path = MAGIC_DIR / filename
        if file_path.exists():
            school_name = filename.replace(".md", "").replace("Magic", " Magic")
            add_proposal_section(file_path, school_name, proposal_data)
        else:
            print(f"  [ERROR] {filename}: File not found")
    
    print("\nDone!")

if __name__ == "__main__":
    main()

