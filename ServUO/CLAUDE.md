# ServUO Technical Context

This document covers ServUO-specific technical systems. For general project overview, see `/CLAUDE.md`.

**Quick Reference:** See `Vystia/reference/` for class, spell, skill, and command tables.

---

## AI Sidekick Combat System

**Location:** `Scripts/Services/AISidekicks/`
**Status:** Fully functional

### Key Files
- `AI/SidekickAI.cs` - Base AI controller
- `AI/MageCombatAI.cs` - Mage combat logic (kiting, spell combos)
- `AI/TamerCombatAI.cs` - Tamer combat logic (pet commands, veterinary)
- `AI/WarriorCombatAI.cs` - Warrior combat logic (melee tank)
- `Archetypes/` - Sidekick class definitions

### Combat Simulation
**Location:** `Scripts/Services/AISidekicks/Simulation/`

```bash
cd Scripts/Services/AISidekicks/Simulation
python full_combat_simulator.py          # Single test
python parameter_optimizer.py dual       # Optimize for both enemies
```

### Optimizable Parameters (MageCombatAI.cs)
| Parameter | Current | Description |
|-----------|---------|-------------|
| `MIN_RETREAT_DISTANCE` | 4 | Start retreating when enemy within X tiles |
| `SPELL_RELEASE_RANGE_MAX` | 14 | Maximum casting distance |
| `LOW_HEALTH` | 0.70 | HP% threshold to start healing |
| `CRITICAL_HEALTH` | 0.29 | HP% for emergency actions |

### Archetypes
- **Mage** - Kiting, spell combos, self-healing
- **Tamer** - Pet commands, veterinary healing, stays near bonded pet
- **Warrior** - Melee tank, self-healing with bandages/potions

---

## Custom Dwarf Race System

**Location:** `Scripts/Mobiles/Vystia/Races/Dwarf.cs`
**Status:** Fully functional

### Body IDs
| ID | Description |
|----|-------------|
| 987 | Dwarf Male body (75% human) |
| 988 | Dwarf Female body (75% human) |
| 909-913 | Male plate armor + warhammer (75% scale) |
| 914-918 | Female leather armor (75% scale) |
| 919 | Shared fancy dress (75% scale) |

### Client Files Required
- `anim.mul` / `anim.idx` - Patched with dwarf animations (IDs 909-919)
- `equipconv.def` - Equipment animation conversion rules
- `bodyconv.def` - **CRITICAL:** Disable redirects for bodies 914-919
- `mobtypes.txt` - Body type definitions (987/988 = HUMAN with flag 10)

### ClassicUO Modification Required
Body IDs 987/988 must be added to the `IsHuman` check in `AnimationsLoader.cs`:
```csharp
public bool IsHuman(ushort g) => g == 400 || g == 401 || g == 987 || g == 988 || ...
```

### bodyconv.def Fix (CRITICAL)
Comment out or remove these lines to prevent ClassicUO from reading wrong animation data:
```
# Vystia: Disabled for custom dwarf equipment animations in anim.mul
# 914	-1	-1	-1	627	-1
# 915	-1	-1	-1	628	-1
# 916	-1	-1	-1	636	-1
# 917	-1	-1	-1	637	-1
# 918	-1	-1	-1	638	-1
# 919	-1	-1	-1	639	-1
```

### Sprite Tools
**Location:** `Vystia/tools/`
- `dwarf_sprite_creator.py` - Extracts human body 400/401, resizes to 75%
- `dwarf_sprite_writer.py` - Writes dwarf bodies to 987/988 in anim.mul
- `dwarf_equipment_creator.py` - Extracts equipment animations, resizes to 75%
- `dwarf_equipment_writer.py` - Writes equipment to 909-919 in anim.mul

---

## Regional Hue Themes

| Region | Hue | Used By |
|--------|-----|---------|
| Frosthold | 1150-1153 | Barbarian, Ice Mage, Beastmaster |
| Emberlands | 1358 | Sorcerer |
| Desert | 1719 | Ranger, Illusionist |
| Shadowfen | 2073 | Witch |
| ShadowVoid | 1109 | Warlock, Necromancer |
| Verdantpeak | 2010 | Druid, Alchemist |
| Crystal Barrens | 1154 | Wizard, Oracle |
| Ironclad | 2305 | Artificer, Fighter, Monk, Templar |
| Skyreach | 1281 | (reserved) |
| Underwater | 1365 | Summoner |
| Multi-Regional | 1153 | Cleric, Paladin, Bounty Hunter, Knight |

---

## Build Commands

```bash
cd D:\UO\ServUO
dotnet restore
dotnet build
```

**Current Status:** 0 errors, 0 warnings

---

## GM Commands Quick Reference

For complete command list, see `Vystia/reference/COMMANDS.md`

### Most Used
```
[spawnvystia          - Spawn creatures/items/vendors gump
[spellbook <type>     - Give spellbook (ice, druid, witch, etc.)
[svs <value>          - Set all Vystia skills to value
[st / [SpawnMage      - Spawn AI sidekick
[sd / [sdm / [sdf     - Spawn dwarf
```

---

*Technical context for ServUO systems. See `/CLAUDE.md` for project overview.*
*Last Updated: 2025-12-11*
