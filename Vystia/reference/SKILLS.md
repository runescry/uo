# Vystia Custom Skills Reference

**Total:** 26 custom skills (12 magic + 14 martial)
**Skill ID Range:** 58-83
**Status:** All implemented (server + client)

---

## Quick Reference

### Magic Skills (IDs 58-69)

| ID | Skill Name | Class | Stat | Description |
|----|------------|-------|------|-------------|
| 58 | Cryomancy | Ice Mage | INT | Ice magic mastery, chill stack effectiveness |
| 59 | Demonology | Warlock | INT | Dark pact magic, soul shard generation |
| 60 | NecromancyArts | Necromancer | INT | Undead control, life force manipulation |
| 61 | Druidism | Druid | INT | Nature magic, shapeshifting effectiveness |
| 62 | Elementalism | Sorcerer | INT | Elemental attunement, stance power |
| 63 | BardicLore | Bard | INT | Song power, crescendo generation |
| 64 | Hexcraft | Witch | INT | Curse potency, hex duration |
| 65 | Divination | Oracle | INT | Foresight, time manipulation |
| 66 | Conjuration | Summoner | INT | Summon power, bond strength |
| 67 | SpiritCalling | Shaman | INT | Totem power, ancestor connection |
| 68 | Runeweaving | Enchanter | INT | Enchantment power, rune crafting |
| 69 | IllusionMagic | Illusionist | INT | Illusion duration, misdirection |

### Martial Skills (IDs 70-83)

| ID | Skill Name | Class | Stat | Description |
|----|------------|-------|------|-------------|
| 70 | Berserking | Barbarian | STR | Fury generation, rage duration |
| 71 | Subterfuge | Rogue | DEX | Combo point generation, stealth power |
| 72 | MartialArts | Monk | DEX | Chi generation, stance effectiveness |
| 73 | ChivalricArts | Knight | STR | Fortitude generation, defensive power |
| 74 | HolyDevotion | Paladin | INT | Virtue power, tithing efficiency |
| 75 | Marksmanship | Ranger | DEX | Focus generation, aspect power |
| 76 | CombatMastery | Fighter | STR | Stance effectiveness, weapon mastery |
| 77 | Zealotry | Templar | STR | Zeal generation, holy strike power |
| 78 | Manhunting | Bounty Hunter | DEX | Pursuit generation, tracking power |
| 79 | BeastBonding | Beastmaster | INT | Pet power, beast control |
| 80 | Engineering | Artificer | INT | Steam/charge generation, construct power |
| 81 | Transmutation | Alchemist | INT | Potion power, mutagen effectiveness |
| 82 | DivineGrace | Cleric | INT | Faith generation, healing power |
| 83 | ArcaneStudies | Wizard | INT | Spell mastery, arcane power |

---

## Skill Gain Mechanics

### Spell Casting Gains

Vystia spells use the `CheckFizzle()` pattern for skill gains:

| Circle | Min Skill | Max Skill | Notes |
|--------|-----------|-----------|-------|
| 1 | 0 | 30 | Anyone can attempt, gains up to 30 |
| 2 | 0 | 40 | Gains up to 40 |
| 3 | 10 | 50 | Requires 10 skill to attempt |
| 4 | 20 | 60 | Requires 20 skill to attempt |
| 5 | 35 | 75 | Requires 35 skill to attempt |
| 6 | 50 | 90 | Requires 50 skill to attempt |
| 7 | 65 | 100 | Near mastery required |
| 8 | 80 | 100 | Grandmaster range (Vystia: 100 is GM, no power scrolls) |

**How Gains Work:**
- `Caster.CheckSkill(skill, minSkill, maxSkill)` determines both fizzle and gain
- If skill < minSkill: Auto-fail (fizzle), no gain possible
- If skill >= maxSkill: Auto-success, no gain possible (too easy)
- If skill in range: Random success/fail, gain chance during check

### Combat Gains

Combat skill gains trigger via `VystiaResourceManager.OnDamageDealt()`:
- 25% trigger rate per damage event
- Difficulty scales with target power
- Crit bonus: +5 difficulty
- Damage bonus: +damage/2 (max +25)

---

## GM Commands

| Command | Description |
|---------|-------------|
| `[rvs` | Reset all 26 Vystia skills to 0.0 |
| `[resetvystiaskills` | Same as above (full name) |
| `[svs <value>` | Set all Vystia skills to specified value (0-100) |
| `[setvystiaskills <value>` | Same as above (full name) |
| `[skillcap` | Show current skill cap and total |
| `[skillcap <value>` | Set skill cap (recommend 84000 for Vystia) |
| `[skillinfo` | Show skill total breakdown (UO vs Vystia) |

---

## Configuration

### Skill Cap

**Default UO:** 7000 (700.0 total skill points)
**Recommended Vystia:** 84000 (8400.0 total = 100 per skill × 84 skills)

**Set via command:**
```
[skillcap 84000
```

**Set via config:** `Config/Publish.cfg` or `Config/Custom.cfg`
```
PlayerCaps.TotalSkillCap=84000
```

---

## File Locations

| Category | Path |
|----------|------|
| SkillName Enum | `ServUO/Server/Skills.cs` |
| SkillInfo Definitions | `ServUO/Scripts/Misc/SkillInfo.cs` |
| Skill Gain Logic | `ServUO/Scripts/Misc/SkillCheck.cs` |
| Spell CheckFizzle | `ServUO/Scripts/Custom/VystiaClasses/Spells/VystiaSpellBase.cs` |
| GM Commands | `ServUO/Scripts/Custom/VystiaClasses/Commands/VystiaSkillCommands.cs` |
| GM Skill Editor | `ServUO/Scripts/Gumps/SkillsGump.cs` |
| Client Skill Names | `ClassicUO/src/ClassicUO.Client/Game/Data/SkillsLoader.cs` |
| Client Skill Groups | `ClassicUO/src/ClassicUO.Client/Game/Managers/SkillsGroupManager.cs` |

---

## Skill Groups (Client Display)

### Vystia Magic (Skills 58-69)
Displayed in player skill window as a group containing:
Cryomancy, Demonology, NecromancyArts, Druidism, Elementalism, BardicLore,
Hexcraft, Divination, Conjuration, SpiritCalling, Runeweaving, IllusionMagic

### Vystia Martial (Skills 70-83)
Displayed in player skill window as a group containing:
Berserking, Subterfuge, MartialArts, ChivalricArts, HolyDevotion, Marksmanship,
CombatMastery, Zealotry, Manhunting, BeastBonding, Engineering, Transmutation,
DivineGrace, ArcaneStudies

---

*Last Updated: 2025-12-11*
*Source of Truth: This file. Do not duplicate skill tables elsewhere.*
