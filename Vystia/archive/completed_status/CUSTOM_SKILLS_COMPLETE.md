# Vystia Custom Skills System - IMPLEMENTATION COMPLETE

**Status:** FULLY IMPLEMENTED
**Date Completed:** 2025-12-11
**Implementation Session:** Custom Skills + Spell Skill Gains

---

## Overview

The Vystia shard now has 26 custom class-specific skills fully integrated into both ServUO server and ClassicUO client. These skills:
- Appear in the native skill window (both GM `[skills` command and player skill window)
- Gain through spell casting using standard UO skill gain mechanics
- Are integrated with the CheckFizzle pattern for proper fizzle/gain behavior

---

## Custom Skills (26 total)

### Magic Class Skills (12) - IDs 58-69

| ID | Skill Name | Class | Description |
|----|------------|-------|-------------|
| 58 | Cryomancy | Ice Mage | Ice magic mastery, chill stack effectiveness |
| 59 | Demonology | Warlock | Dark pact magic, soul shard generation |
| 60 | NecromancyArts | Necromancer | Undead control, life force manipulation |
| 61 | Druidism | Druid | Nature magic, shapeshifting effectiveness |
| 62 | Elementalism | Sorcerer | Elemental attunement, stance power |
| 63 | BardicLore | Bard | Song power, crescendo generation |
| 64 | Hexcraft | Witch | Curse potency, hex duration |
| 65 | Divination | Oracle | Foresight, time manipulation |
| 66 | Conjuration | Summoner | Summon power, bond strength |
| 67 | SpiritCalling | Shaman | Totem power, ancestor connection |
| 68 | Runeweaving | Enchanter | Enchantment power, rune crafting |
| 69 | IllusionMagic | Illusionist | Illusion duration, misdirection |

### Martial Class Skills (14) - IDs 70-83

| ID | Skill Name | Class | Description |
|----|------------|-------|-------------|
| 70 | Berserking | Barbarian | Fury generation, rage duration |
| 71 | Subterfuge | Rogue | Combo point generation, stealth power |
| 72 | MartialArts | Monk | Chi generation, stance effectiveness |
| 73 | ChivalricArts | Knight | Fortitude generation, defensive power |
| 74 | HolyDevotion | Paladin | Virtue power, tithing efficiency |
| 75 | Marksmanship | Ranger | Focus generation, aspect power |
| 76 | CombatMastery | Fighter | Stance effectiveness, weapon mastery |
| 77 | Zealotry | Templar | Zeal generation, holy strike power |
| 78 | Manhunting | Bounty Hunter | Pursuit generation, tracking power |
| 79 | BeastBonding | Beastmaster | Pet power, beast control |
| 80 | Engineering | Artificer | Steam/charge generation, construct power |
| 81 | Transmutation | Alchemist | Potion power, mutagen effectiveness |
| 82 | DivineGrace | Cleric | Faith generation, healing power |
| 83 | ArcaneStudies | Wizard | Spell mastery, arcane power |

---

## Server-Side Implementation

### Files Modified

**1. Server/Skills.cs**
- Extended `SkillName` enum with 26 new values (IDs 58-83)
- Updated `Skills` class with property getters for each skill

**2. Scripts/Misc/SkillCheck.cs**
- Added UseAntiMacro entries for skills 58-83 (all true)
- Added Vystia-filtered debug output (lines 272-305, 408-527)
- Shows `[VYSTIA CheckSkill]` and `[VYSTIA Gain]` for skill IDs 58-83 only

**3. Scripts/Misc/SkillInfo.cs**
- Added 26 SkillInfo entries to Table array (now 84 total)
- Each entry includes: name, stat scaling (INT for magic, STR/DEX for martial), title, gain factor

**4. Scripts/Custom/VystiaClasses/Spells/VystiaSpellBase.cs**
- `CheckFizzle()` override implements UO-standard skill gain pattern
- Single `CheckSkill()` call determines BOTH fizzle AND triggers gain
- Circle-based difficulty ranges:
  - Circle 1: minSkill=0, maxSkill=30
  - Circle 2: minSkill=0, maxSkill=40
  - Circle 3: minSkill=10, maxSkill=50
  - Circle 4: minSkill=20, maxSkill=60
  - Circle 5: minSkill=35, maxSkill=75
  - Circle 6: minSkill=50, maxSkill=90
  - Circle 7: minSkill=65, maxSkill=100
  - Circle 8: minSkill=80, maxSkill=120

**5. Scripts/Custom/VystiaClasses/Commands/VystiaSkillCommands.cs**
- `[rvs` / `[resetvystiaskills` - Resets all 26 Vystia skills to 0.0
- `[svs <value>` / `[setvystiaskills <value>` - Sets all to specified value (0-120)
- `[skillcap [value]` - Gets or sets Mobile.SkillsCap
- `[skillinfo` - Shows Skills.Total breakdown

**6. Scripts/Gumps/SkillsGump.cs**
- Added "Vystia Magic" group (skills 58-69) at lines 504-517
- Added "Vystia Martial" group (skills 70-83) at lines 519-535
- GMs can now see/edit all Vystia skills via `[skills` command

---

## Client-Side Implementation

### Files Modified (ClassicUO)

**1. Game/Data/SkillsLoader.cs**
- Extended skill count to 84
- Added Vystia skill names for IDs 58-83
- Skills appear in skill window with proper names

**2. Game/Managers/SkillsGroupManager.cs**
- Added "Vystia Magic" group containing skills 58-69
- Added "Vystia Martial" group containing skills 70-83
- Groups appear in player skill window
- `AddVystiaGroupsIfMissing()` helper ensures groups exist even when loading cached XML

**3. Game/UI/Gumps/SkillsGump.cs** (StandardSkillsGump)
- Modified to display Vystia skill groups
- Skills show current value and can be locked/up/down like standard UO skills

---

## Spell Casting Skill Gains

### How It Works

When a player casts a Vystia spell:

1. **VystiaSpellBase.CheckFizzle()** is called
2. Gets the school skill via `GetSchoolSkill()` (e.g., Druidism for Nature spells)
3. Looks up circle-based min/max skill range
4. Calls `Caster.CheckSkill(skill, minSkill, maxSkill)` - this SINGLE call:
   - Determines if the spell fizzles (fails) or succeeds
   - Triggers a skill gain check automatically
   - Follows standard UO gain mechanics (harder = more likely to gain)

### Skill Gain Mechanics

**Standard UO CheckSkill Pattern:**
- If skill < minSkill: Auto-fail (fizzle), no gain possible (too hard)
- If skill >= maxSkill: Auto-success, no gain possible (too easy)
- If skill in range: Random success/fail, gain possible during check
- Gain chance decreases as skill approaches cap

**Gain Chance Formula** (from SkillCheck.cs):
```
gc = (cap - base) / cap
If base < 10: guaranteed gain check
Else if gc > random roll: gain check triggered
```

---

## Configuration

### Skill Cap

**IMPORTANT:** Default UO skill cap (7000 points = 700.0 total) is too low for Vystia with 84 skills.

**Recommended Setting:**
```
[skillcap 84000
```
This allows 8400 total skill points (100 per skill × 84 skills).

### Config File Location

Skills cap can be set in ServUO config:
- `Config/Publish.cfg` or `Config/Custom.cfg`
- Setting: `PlayerCaps.TotalSkillCap=84000`

Or programmatically via `Mobile.SkillsCap` property.

---

## Testing & Debug Commands

### Skill Management Commands

| Command | Description |
|---------|-------------|
| `[rvs` | Reset all 26 Vystia skills to 0.0 |
| `[svs 50` | Set all 26 Vystia skills to 50.0 |
| `[skillcap` | Show current skill cap and total |
| `[skillcap 84000` | Set skill cap to 8400 points |
| `[skillinfo` | Show skill total breakdown (UO vs Vystia) |

### Debug Output

When casting Vystia spells, debug output appears:
```
[Spell Debug] VystiaSpell.CheckFizzle for NatureNaturesTouchSpell (School: Nature)
[Spell Debug] Skill=20.0, Circle=1, Range=0.0-30.0
[VYSTIA CheckSkill] Druidism(61): AllowGain=True, gc=0.45, Base=0.6
[VYSTIA CheckSkill] ShouldGain=True [Base<10=True]
[VYSTIA Gain] SUCCESS! Druidism: 0.6 -> 1.0
```

---

## Combat Skill Gains

Combat skill gains are triggered via `VystiaResourceManager.OnDamageDealt()`:

**Trigger Conditions:**
- 25% chance per damage event (prevents spam)
- Difficulty based on target: `(HitsMax + Fame/100 + Skills.Total/100) / 10`
- Crit bonus: +5 difficulty
- Damage bonus: `+damage/2` (max +25)

**Implementation:** `VystiaResourceManager.cs` lines 305-338

---

## Known Issues

**RESOLVED:**
1. Skills.Total exceeding Skills.Cap blocked ALL gains
   - **Fix:** Set `[skillcap 84000` for Vystia

2. Meditation/Focus gaining instead of class skill
   - **Fix:** CheckFizzle override now calls CheckSkill on correct Vystia skill

3. Skill gains triggered before spell completion
   - **Fix:** Changed from OnCast() to CheckFizzle() override

---

## Migration from Planning Document

The original `CUSTOM_SKILLS_PLAN.md` has been archived to `Vystia/archive/`.

**What Was Implemented vs Planned:**

| Planned Feature | Status |
|-----------------|--------|
| Real skills (not pseudo-skills) | IMPLEMENTED |
| Server-side skill registration | IMPLEMENTED |
| Client-side skill display | IMPLEMENTED |
| Skill gain via CheckSkill | IMPLEMENTED |
| Combat skill gains | IMPLEMENTED |
| GM testing commands | IMPLEMENTED |
| Anti-macro protection | IMPLEMENTED |
| Skill info definitions | IMPLEMENTED |

---

## File Locations Summary

**Server:**
- `ServUO/Server/Skills.cs` - SkillName enum, Skill class
- `ServUO/Scripts/Misc/SkillCheck.cs` - Gain mechanics + debug
- `ServUO/Scripts/Misc/SkillInfo.cs` - Skill definitions
- `ServUO/Scripts/Custom/VystiaClasses/Spells/VystiaSpellBase.cs` - CheckFizzle
- `ServUO/Scripts/Custom/VystiaClasses/Commands/VystiaSkillCommands.cs` - GM commands
- `ServUO/Scripts/Gumps/SkillsGump.cs` - GM skill editor groups

**Client:**
- `ClassicUO/src/ClassicUO.Client/Game/Data/SkillsLoader.cs` - Skill names
- `ClassicUO/src/ClassicUO.Client/Game/Managers/SkillsGroupManager.cs` - Skill groups
- `ClassicUO/src/ClassicUO.Client/Game/UI/Gumps/SkillsGump.cs` - Skill display

---

*Document Created: 2025-12-11*
*Status: COMPLETE*
