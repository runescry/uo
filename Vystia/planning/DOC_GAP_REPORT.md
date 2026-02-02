# Documentation Gap Report

Generated: 2026-01-23
Scope: First 20 known mismatches (docs vs code/runtime or doc vs doc) prioritized for reconciliation.

Legend:
- Doc claim: statement or status in documentation.
- Observed: code, runtime data, or conflicting doc evidence.

| ID | Area | Doc claim (path) | Observed / conflicting evidence | Impact | Suggested fix |
|---|---|---|---|---|---|
| 1 | Class count | `Vystia/VYSTIA_MASTER_INVENTORY.md` ("All 25 character classes") and `Vystia/reference/VYSTIA_COMPLETE_REFERENCE.md` (Classes: 25) | `ServUO/Scripts/Custom/VystiaClasses/Classes/PlayerClassV2.cs` enum count = 26 (+ None) | Class roster mismatch across docs and code | Update class counts to 26 and list missing class explicitly (likely Rogue) |
| 2 | Rogue visibility | `Vystia/reference/VYSTIA_COMPLETE_REFERENCE.md` class list has 25 entries and omits Rogue | `ServUO/Scripts/Custom/VystiaClasses/Classes/RogueClass.cs` exists; `Vystia/gm/COMMANDS.md` lists Rogue | Players/admins see different class rosters | Add Rogue to class lists and summary tables |
| 3 | Class implementation status | `Vystia/README.md` claims "26/26 All implemented" | `Vystia/implementation/classes/VYSTIA_CLASS_SELECTION_SYSTEM.md` says 10 implemented, 15 stubs | Conflicting expectations on class completeness | Align README status with actual implementation detail or update class status docs |
| 4 | Class completeness (internal) | `Vystia/VYSTIA_MASTER_INVENTORY.md` says all 25 classes fully implemented | Same file lists 15 stub classes lacking custom abilities | Internal contradiction | Reword to distinguish base class wiring vs custom abilities |
| 5 | Class selection counts (internal) | `Vystia/implementation/classes/VYSTIA_CLASS_SELECTION_SYSTEM.md` says 26 classes (10 implemented, 15 stubs) | 10 + 15 = 25 | Numeric inconsistency | Fix counts and reconcile roster list |
| 6 | LLM class domain count | `Vystia/VYSTIA_MASTER_INVENTORY.md` says `class_domain.json` has 25 classes | `PlayerClassTypeV2` has 26 classes | LLM lore class roster is behind | Regenerate/expand class_domain.json and update count |
| 7 | Class generator count | `Vystia/README.md` says generate_all_classes.py generated 25 classes | `PlayerClassTypeV2` has 26 classes | Tooling doc behind class roster | Update README tooling summary and script output notes |
| 8 | Spell count | Docs commonly state 384 spells (12 schools x 32) | Spell folder has 386 `.cs` files (`ServUO/Scripts/Custom/VystiaClasses/Spells/`) | Spell counts inconsistent; may indicate extra files or duplicates | Audit spell file list and update counts or prune extras |
| 9 | Spellbook test status (internal) | `Vystia/README.md` says 12 spellbooks are tested and functional | Same file says only Ice & Druid tested, 10 ready for testing | Conflicting readiness claims | Clarify tested vs ready status; pick one source of truth |
| 10 | Spellbook testing vs checklist | `Vystia/README.md` says all spellbooks tested | `Vystia/SPELLBOOK_SYSTEM_COMPLETE.md` marks Bard pending test | Testing claims inconsistent | Update README or complete Bard test and note completion |
| 11 | Bard primary skill | `Vystia/design/system_design/VYSTIA_BARD_SYSTEM.md` uses Songweaving (ID 67) | Class/skill references use BardicLore (ID 63): `Vystia/reference/VYSTIA_COMPLETE_REFERENCE.md`, `Vystia/reference/SKILLS.md` | Core Bard skill identity unclear | Decide canonical Bard skill and update all doc tables |
| 12 | Crescendo cap | `Vystia/design/system_design/VYSTIA_BARD_SYSTEM.md` says 0-20 | `Vystia/player/PLAYERS_GUIDE.md` says max 10; `Vystia/implementation/classes/SYSTEMS_V2.md` says 100 | Conflicting resource caps | Align Bard resource design and update player/GM guides |
| 13 | Bard system direction | `Vystia/design/system_design/VYSTIA_BARD_SYSTEM.md` specifies Songweaving-first | GM/player docs (e.g. `Vystia/TESTING_CHECKLIST.md`, `Vystia/player/guides/BARD_GUIDE.md`) assume BardSpellbook-first | Design vs implementation split | Choose authoritative Bard system and update guides accordingly |
| 14 | Bardic magic status | `Vystia/VYSTIA_MASTER_INVENTORY.md` says Bardic Magic 32/32 fully implemented | `Vystia/design/magic/BardicMagic.md` states 0/32 correct effects (placeholders) | Major mismatch on implementation status | Update inventory status or implement Bardic spells per design |
| 15 | Bardic magic deprecation | `Vystia/design/magic/BardicMagic.md` is deprecated in favor of Songweaving | Reference/test docs still list Bardic spellbook spells (e.g. `Vystia/reference/SPELLS.md`, `Vystia/TESTING_CHECKLIST.md`) | Docs conflict on active system | Update references to match chosen system and mark deprecated content |
| 16 | Bard spellbook hue | `Vystia/design/magic/BardicMagic.md` lists hue 0x481 | `Vystia/VYSTIA_MASTER_INVENTORY.md` lists Bard spellbook hue 0x8A5 | Item data mismatch | Verify actual hue in `ServUO/Scripts/Items/Equipment/Spellbooks/VystiaSpellbooks.cs` and update docs |
| 17 | Bard system example | `Vystia/VYSTIA_SHARD_OVERVIEW.md` presents Songweaving examples | `Vystia/TESTING_CHECKLIST.md` and `Vystia/gm/GM_TESTING_GUIDE.md` test BardSpellbook usage | Inconsistent onboarding/testing | Align overview narrative with current implementation choice |
| 18 | Quest count vs runtime data | `Vystia/README.md` says 6 quests generated and ready | `ServUO/Data/VystiaQuests.xml` has 0 `<Quest>` nodes | Quest availability overstated | Generate/export quests to XML or update README status |
| 19 | Quest count vs inventory | `Vystia/VYSTIA_MASTER_INVENTORY.md` claims 6 Mondain quests active | `ServUO/Data/VystiaQuests.xml` empty | Inventory overstates quest state | Reconcile quest generation output with inventory |
| 20 | Class selection file list | `Vystia/implementation/classes/VYSTIA_CLASS_SELECTION_SYSTEM.md` references `PendingClasses.cs` | Only `ServUO/Scripts/Custom/VystiaClasses/Classes/PendingClasses.cs.bak` exists | Doc points to non-existent file | Update doc to current file(s) or restore missing class file |

## Notes
- Gaps above are based on current repo state as of 2026-01-23.
- Some mismatches are doc-vs-doc; resolving them requires picking a single source of truth and propagating.
