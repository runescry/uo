# Vystia GM Documentation

This folder contains Game Master (GM) testing guides, command references, and content creation templates for the Vystia shard.

## File Index

| File | Purpose | Lines | Primary Use |
|------|---------|-------|-------------|
| [COMMANDS.md](COMMANDS.md) | Quick command reference | 243 | Quick lookup during testing |
| [SYSTEM_TEST_GUIDE.md](SYSTEM_TEST_GUIDE.md) | Comprehensive 19-section test checklist | 707 | Full system validation |
| [CLASS_TESTING_GUIDE.md](CLASS_TESTING_GUIDE.md) | All 26 classes with spell checklists | 1,269 | Class-specific testing |
| [GM_TESTING_GUIDE.md](GM_TESTING_GUIDE.md) | Detailed class testing procedures | 2,000+ | In-depth class validation |
| [NPC_TESTING_GUIDE.md](NPC_TESTING_GUIDE.md) | NPC, LLM, and new systems testing | 658 | NPC & system integration |
| [TESTING.md](TESTING.md) | General GM testing quick reference | 322 | Quick testing workflows |
| [CONTENT_CREATION.md](CONTENT_CREATION.md) | Templates for new content | 389 | Creating quests, NPCs, items |

---

## Recommended Workflow

### Quick Testing Session
1. Start with [TESTING.md](TESTING.md) for quick commands
2. Reference [COMMANDS.md](COMMANDS.md) for specific commands

### Full System Validation
1. Use [SYSTEM_TEST_GUIDE.md](SYSTEM_TEST_GUIDE.md) - covers all 19 systems
2. Check off each section as tested

### Class Testing
1. Use [CLASS_TESTING_GUIDE.md](CLASS_TESTING_GUIDE.md) for spell checklists
2. Reference [GM_TESTING_GUIDE.md](GM_TESTING_GUIDE.md) for detailed procedures

### NPC & New Systems
1. Use [NPC_TESTING_GUIDE.md](NPC_TESTING_GUIDE.md) for:
   - LLM dialogue testing
   - Faction leaders
   - Service fees, Religion, Pets, Housing, Zones

### Content Creation
1. Use [CONTENT_CREATION.md](CONTENT_CREATION.md) templates for:
   - Quests, NPCs, Creatures, Spells, Items

---

## Cross-References

| Topic | Primary Doc | Also See |
|-------|-------------|----------|
| Commands | [../reference/COMMANDS.md](../reference/COMMANDS.md) | COMMANDS.md (quick ref) |
| Classes | CLASS_TESTING_GUIDE.md | GM_TESTING_GUIDE.md |
| Spells | CLASS_TESTING_GUIDE.md | [../reference/SPELLS.md](../reference/SPELLS.md) |
| Skills | [../reference/SKILLS.md](../reference/SKILLS.md) | SYSTEM_TEST_GUIDE.md |
| Systems | SYSTEM_TEST_GUIDE.md | NPC_TESTING_GUIDE.md |

---

## Systems Covered

All documentation covers **currently implemented systems**:

### Core Systems
- 25 Character Classes (v2.0)
- 26 Custom Skills (IDs 58-83)
- 384 Spells (12 schools x 32 spells)
- 15 Secondary Resources
- Buff/Debuff System
- Crowd Control (with diminishing returns)
- Stance/Form System (28 stances)
- Ability Framework

### World Systems
- Pet System (4 classes, 5 tiers)
- Housing System (5 sizes, weekly taxes)
- Zone Control (4 zone types)
- Faction System (7 factions)
- Economy System (gold sinks, service fees)
- Religion System (6 religions, piety)

### Content Systems
- Quest System (multi-waypoint, LLM NPCs)
- 138 Creatures (10 regions)
- 14 Vendors (12 magic + 2 general)
- 25 Class Trainers

---

## Validation Status

All files validated 2026-01-03:
- No deprecated content
- All referenced systems are implemented
- Commands match current implementation
- Spell IDs verified (1000-1383)

---

*Last Updated: 2026-01-03*
