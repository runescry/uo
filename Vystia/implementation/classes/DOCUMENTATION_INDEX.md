# Vystia Character Classes - Documentation Index

**Quick Start:** Read files in this order for fastest onboarding.

## Core Documentation (Start Here)

1. **[README.md](README.md)** - Start here! Complete overview, directory structure, class list, implementation status
2. **[IMPLEMENTATION_SUMMARY.md](IMPLEMENTATION_SUMMARY.md)** - Detailed timeline, automation scripts, implementation details
3. **[CLAUDE.md](CLAUDE.md)** - AI assistant context with code patterns and examples

## Reference Documentation

4. **[RESOURCE_CORRECTIONS.md](RESOURCE_CORRECTIONS.md)** - Vystia resource catalog and regional mappings
5. **[KNOWN_ISSUES.md](KNOWN_ISSUES.md)** - Current build errors (5 pre-existing, none from classes)
6. **DOCUMENTATION_INDEX.md** - This file

## External Documentation

### Project-Level
- `D:\UO\CLAUDE.md` - Main project overview
- `D:\UO\ServUO\CLAUDE.md` - ServUO-specific context
- `D:\UO\Vystia\VYSTIA_MASTER_INVENTORY.md` - Complete system inventory (MASTER FILE)

### Magic System
- `D:\UO\Vystia\Magic\README.md` - Magic system overview
- `D:\UO\Vystia\Magic\VYSTIA_MAGIC_REAGENTS.md` - Reagent documentation
- `D:\UO\Vystia\Magic\VYSTIA_SPELLBOOKS_IMPLEMENTATION.md` - Spellbook system
- `D:\UO\Vystia\Magic\{SchoolName}.md` - Individual magic school docs (12 files)

## Quick Reference

### Implementation Status
- **Classes:** 25/25 (100%) ✅
- **Spells:** 384/384 (100%) ✅
- **Magic Schools:** 12/12 (100%) ✅
- **Spellbooks:** 12/12 (100%) ✅
- **Special Items:** 16/16 (100%) ✅
- **Magic Reagents:** 96/96 (100%) ✅

### File Organization
```
VystiaClasses/
├── Classes/           - 25 class implementation files
├── Spells/            - 384 spell files across 12 schools
├── Items/
│   ├── AbilityItems/  - 5 complex items + ClassSpecialItems.cs (11 simple items)
│   ├── Spellbooks/    - VystiaSpellbooks.cs (all 12 consolidated)
│   └── Creatures/     - ClockworkScout.cs
├── Gumps/             - Class selection UI
├── Core/              - Base classes and applicator
└── *.md               - Documentation (you are here)
```

### Automation Tools
- `D:\UO\Vystia\tools\generate_all_classes.py` - Generated 15 classes
- `D:\UO\Vystia\tools\generate_spells.py` - Generated 384 spells

## Documentation Standards

### When to Update Each File

**README.md** - Update when:
- Directory structure changes
- New major features added
- Implementation status changes
- Build status changes

**IMPLEMENTATION_SUMMARY.md** - Update when:
- New classes implemented
- Automation scripts created/modified
- Implementation timeline changes
- Testing results available

**CLAUDE.md** - Update when:
- Code patterns change
- New implementation guidelines needed
- Common mistakes identified
- Working examples added

**KNOWN_ISSUES.md** - Update when:
- New bugs discovered
- Bugs fixed
- Workarounds found

**RESOURCE_CORRECTIONS.md** - Update when:
- New resources added
- Resource names corrected
- Regional mappings change

## Consolidated vs Individual Files

### Why Some Files Are Consolidated

**Consolidated Files:**
- `ClassSpecialItems.cs` - 11 simpler ability items
- `VystiaSpellbooks.cs` - All 12 magic school spellbooks

**Benefits:**
- Reduced file sprawl (28 potential files → 10 actual files)
- Easier maintenance
- Grouped by functionality
- Still maintains clear namespacing

**Individual Files:**
- Complex items with gumps (ShapeshiftTotem, ArtificerBlueprints)
- Items with complex logic (RageTotem, ConstructControlDevice, HolySymbol)
- Unique creatures (ClockworkScout)

## Version History

**v1.0 (2025-12-07)** - 100% Complete
- All 25 classes implemented
- All 384 spells implemented
- All 28 custom items implemented
- Complete documentation

**v0.4 (2025-12-06)**
- All 384 spells generated
- 12 spellbooks implemented
- 96 magic reagents created

**v0.3 (2025-12-05)**
- 6 ability items extracted
- Resource names corrected
- Initial documentation created

**v0.2 (2024-12)**
- First 10 classes implemented manually
- Initial spell implementations

**v0.1 (2024-11)**
- Base PlayerClass system
- Core architecture

## Contact & Contribution

When making changes:
1. Update the relevant documentation file
2. Update IMPLEMENTATION_SUMMARY.md timeline
3. Update README.md if structure changes
4. Run build to verify changes compile
5. Test in-game if possible

---

*Last Updated: 2025-12-07*
*Status: Production Ready*
