# Vystia Zone Control System Design

## Overview

The Zone Control System defines different danger levels across Vystia regions, controlling PvP rules, death penalties, loot drops, and reward multipliers.

## Implementation Status: COMPLETE

**Location:** `ServUO/Scripts/Custom/VystiaClasses/Zones/`
**Files:** 1 (726 lines)

---

## Zone Types

| Zone | Color | PvP | Consent | Guards | Death Penalty | Loot Drop | XP/Gold Bonus |
|------|-------|-----|---------|--------|---------------|-----------|---------------|
| Sanctuary | Green (0x47) | No | N/A | Yes | 0% | 0% | +0% |
| Contested | Yellow (0x35) | Yes | Required | Yes | 25% | 10% | +25% |
| Lawless | Red (0x22) | Yes | No | No | 50% | 50% | +50% |
| Extreme | Black (0x455) | Yes | No | No | 100% | 100% | +100% |

---

## Zone Rules

### Sanctuary (Green)
- Complete protection from PvP
- Guards respond to criminals
- No death penalties
- Starting areas and major cities

### Contested (Yellow)
- PvP requires both players flagged
- Guards still respond
- 25% skill loss on death
- 10% item drop chance
- Frontier and wilderness areas

### Lawless (Red)
- Open PvP, no consent needed
- No guard protection
- 50% skill loss on death
- 50% item drop chance
- Dangerous dungeons and remote areas

### Extreme (Black)
- Full open PvP
- 100% skill loss on death
- Full loot on death
- **Permadeath risk** (repeated deaths)
- No self-resurrection allowed
- Boss areas and endgame content

---

## Default Region Mappings

### Sanctuary Zones
- Britain, Trinsic, Moonglow, Yew, Minoc, Vesper
- Skara Brae, Jhelom, Magincia, Nujel'm
- Serpent's Hold, Delucia, Papua
- Ironhaven, Frostholm, Verdantia, Crystalspire, Sunport

### Contested Zones
- Frosthold Wilds, Emberlands Border
- Verdantpeak Forest, Desert Outskirts
- Shadowfen Marshes

### Lawless Zones
- Shadowfen Deep, Emberlands Core
- Frosthold Peaks, Crystal Barrens
- Obsidian Wastes

### Extreme Zones
- ShadowVoid, Void Rift
- Eternal Frost, Molten Core

---

## Death Mechanics

### Skill Loss
```
totalLoss = sum(skill.Base * 0.005 * deathPenalty)
```

### Loot Drop
- Gold/bank checks are protected
- Blessed/newbied items are protected
- Random roll per item based on `LootDropRate`
- Items drop to corpse or ground

---

## GM Commands

| Command | Access | Description |
|---------|--------|-------------|
| `[ZoneInfo` / `[ZI` | Player | Current zone information |
| `[SetZone <region> <type>` / `[SZ` | GM | Configure zone type |
| `[ZoneList` / `[ZL` | GM | List all configured zones |
| `[TogglePvP` | Player | Toggle PvP flag in contested zones |

---

## Custom Regions

The `VystiaZoneRegion` class extends `BaseRegion`:
- Automatic zone announcements on enter/exit
- Housing restrictions in Extreme zones
- Darker ambient lighting in Extreme zones
- Full death handling integration

---

## Special Rules

### Extreme Zone Restrictions
- No housing placement allowed
- Self-resurrection disabled
- Darker lighting (global level 12)
- Permadeath warnings on death

---

*Last Updated: 2026-01-03*
