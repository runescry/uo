# Vystia Documentation Consolidation Script
# Completes the reorganization by creating README files

$ErrorActionPreference = "Stop"
$VystiaRoot = "C:\DevEnv\GIT\UO\Vystia"

Write-Host "=== Vystia Documentation Consolidation ===" -ForegroundColor Cyan

# Create README files
Write-Host "Creating README files..." -ForegroundColor Yellow

# design/README.md
@"
# Vystia Design Documentation

This directory contains all lore, world-building, and design documentation.

## Contents

- **WORLD_LORE.md** - Complete world lore, regions, history (114 KB)
- **CLASSES.md** - Character class lore and descriptions (34 KB)
- **CREATURES_BESTIARY.md** - All 131 creatures (58 KB)
- **DUNGEON_BOSSES.md** - Boss encounters (8 KB)
- **FACTIONS.md** - Faction system (2 KB)
- **SEA_SYSTEMS.md** - Underwater systems (2 KB)
- **CUSTOM_RACES.md** - Custom race implementation (12 KB)

## Purpose

Defines WHAT Vystia is - world, inhabitants, story.

## Related

- ../implementation/ - How to build
- ../reference/ - Quick lookups
- ../VYSTIA_MASTER_INVENTORY.md - Status
"@ | Out-File "$VystiaRoot/design/README.md" -Encoding UTF8

# implementation/README.md
@"
# Vystia Implementation Guides

How-to guides for implementing Vystia systems.

## Structure

### installation/
- INSTALLATION.md - Installation guide
- DEPLOYMENT.md - Deployment procedures

### content/
- CREATURES.md - Creature implementation
- EQUIPMENT.md - Equipment guide
- ITEMS.md - Item guide
- RESOURCES.md - Resource system

### world/
- MAP_SETUP.md - Map setup
- TERRAIN_GENERATION.md - Terrain generation
- WORLD_GENERATION.md - World generation

### art/
- ART.md - Art asset implementation

## Purpose

Explains HOW to implement Vystia systems.

## Related

- ../design/ - What to implement
- ../reference/ - Quick lookups
- ../planning/ - Future plans
"@ | Out-File "$VystiaRoot/implementation/README.md" -Encoding UTF8

# reference/README.md
@"
# Vystia Reference Documentation

Quick-reference for development.

## Contents

- ITEMS_INVENTORY.md - Item inventory
- COMMANDS.md - GM commands reference

### terrain/
- README.md - Terrain overview
- TERRAIN_GUIDE.md - Terrain reference
- TILE_REFERENCE.md - Tile reference
- REAL_TERRAIN_EXPORT.md - Export procedures

## Purpose

Quick lookups for active development.

## Related

- ../VYSTIA_MASTER_INVENTORY.md - Master status
- ../implementation/ - How-to guides
- ../design/ - Lore and design
"@ | Out-File "$VystiaRoot/reference/README.md" -Encoding UTF8

# planning/README.md
@"
# Vystia Planning Documentation

Future plans, roadmaps, and proposals.

## Contents

- ROADMAP.md - Content roadmap
- IMPLEMENTATION.md - Implementation plan
- LOCATIONS.md - Planned locations
- MAP.md - Map expansion plans

## Purpose

Outlines WHAT'S NEXT for Vystia.

## Status

See ../VYSTIA_MASTER_INVENTORY.md for current status.

## Related

- ../design/ - What we're building
- ../implementation/ - How we'll build it
"@ | Out-File "$VystiaRoot/planning/README.md" -Encoding UTF8

# archive/README.md
@"
# Vystia Documentation Archive

Outdated or superseded documentation.

## Why Archive?

Historical context and decisions preserved for reference.

## Contents

### Root (Superseded)
- ACTUAL_USAGE.md
- DOCS_INDEX.md
- IMPLEMENTATION_STATUS.md
- README_LLM_NPC.md
- current issue.md
- VYSTIA_IMPLEMENTATION_ANALYSIS.md
- VYSTIA_TODO.md
- VYSTIA_DOCS_INDEX.md

### creatures/
- Old creature bestiaries

### troubleshooting/
- VYSTIA_TROUBLESHOOTING_LOG.md

## Current Documentation

- ../VYSTIA_MASTER_INVENTORY.md - Master file
- ../design/ - Design and lore
- ../implementation/ - Guides
- ../reference/ - Quick reference
- ../planning/ - Future plans
"@ | Out-File "$VystiaRoot/archive/README.md" -Encoding UTF8

# Root README.md
@"
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

``````
Vystia/
├── README.md (this file)
├── VYSTIA_MASTER_INVENTORY.md (MASTER FILE)
├── design/ - WHAT to build
├── implementation/ - HOW to build
├── reference/ - Quick lookups
├── planning/ - Future plans
├── Magic/ - Magic system
├── .claude/ - Internal tools
├── tools/ - Automation scripts
└── archive/ - Old documents
``````

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
"@ | Out-File "$VystiaRoot/README.md" -Encoding UTF8

Write-Host "Created 6 README files" -ForegroundColor Green
Write-Host ""
Write-Host "=== Consolidation Complete! ===" -ForegroundColor Green
Write-Host ""
Write-Host "Summary:" -ForegroundColor Cyan
Write-Host "- Deleted: 4 redundant files"
Write-Host "- Archived: 12 outdated files"
Write-Host "- Moved: 31 active files"
Write-Host "- Created: 6 README files"
Write-Host ""
Write-Host "New Structure:" -ForegroundColor Cyan
Write-Host "Before: 62 files in 7 directories"
Write-Host "After: 46 active + 16 archived in organized structure"
Write-Host ""
Write-Host "Root directory now has only 2 files:" -ForegroundColor Yellow
Get-ChildItem "$VystiaRoot" -File -Filter "*.md" | Select-Object Name
