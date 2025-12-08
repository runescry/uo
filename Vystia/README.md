# Vystia - Custom Ultima Online Shard

Complete custom world built on ServUO.

## Quick Start

- **VYSTIA_MASTER_INVENTORY.md** - START HERE! Complete status
- **design/** - World lore, classes, creatures
- **implementation/** - How-to guides
- **reference/** - Quick reference
- **planning/** - Future roadmaps
- **Magic/** - Magic system (12 schools, 384 spells)

## Project Status

| System | Status | Count |
|--------|--------|-------|
| Character Classes | 100% | 25/25 |
| Custom Spells | 100% | 384/384 |
| Magic Schools | 100% | 12/12 |
| Creatures | 100% | 131/131 |
| Magic Reagents | 100% | 96/96 |

**Overall: ~65%** - Core systems complete!

## Structure

```
Vystia/
â”œâ”€â”€ README.md (this file)
â”œâ”€â”€ VYSTIA_MASTER_INVENTORY.md (MASTER FILE)
â”œâ”€â”€ design/ - WHAT to build
â”œâ”€â”€ implementation/ - HOW to build
â”œâ”€â”€ reference/ - Quick lookups
â”œâ”€â”€ planning/ - Future plans
â”œâ”€â”€ Magic/ - Magic system
â”œâ”€â”€ .claude/ - Internal tools
â”œâ”€â”€ tools/ - Automation scripts
â””â”€â”€ archive/ - Old documents
```

## Key Systems

### Character Classes (100%)
- 25 classes across 10 regions
- 12 magic schools
- 384 spells
- 96 custom reagents
- 28 custom items

### Creatures (100%)
- 131 custom creatures
- Regional bosses
- Unique loot

### World
- 10 unique regions
- Complete lore
- Custom races

### Magic (100%)
- 12 schools: Ice, Nature, Hex, Elemental, Dark, Divination, Necromancy, Summoning, Shamanic, Bardic, Enchanting, Illusion
- 32 spells per school
- Custom reagents
- Functional spellbooks

## Development Tools

In tools/:
- generate_all_classes.py
- generate_spells.py
- consolidate_documentation.ps1

## Installation

See implementation/installation/INSTALLATION.md

## For Developers

1. Read VYSTIA_MASTER_INVENTORY.md
2. Check design/ for specs
3. Follow implementation/ guides
4. Use reference/ for lookups
5. See planning/ for roadmaps

---

**Last Updated:** 2025-12-07
**Status:** Consolidated and organized
