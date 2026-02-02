# System Audit Matrix (Top 10+)

Generated: 2026-01-23
Scope: Top 10 systems from DOCUMENTATION_AUDIT_PLAN.md

Columns:
- System
- Design Docs
- Implementation (code)
- Runtime Data
- Tests/Guides
- Status (claimed)
- Notes / Source of Truth (SoT)
- Verified By

| System | Design Docs | Implementation (code) | Runtime Data | Tests/Guides | Status (claimed) | Notes / SoT | Verified By |
|---|---|---|---|---|---|---|---|
| Classes (v2) | `Vystia/design/CLASSES.md`, `Vystia/reference/CLASSES.md` | `ServUO/Scripts/Custom/VystiaClasses/Classes/*`, `ServUO/Scripts/Custom/VystiaClasses/Core/*` | N/A | `Vystia/gm/CLASS_TESTING_GUIDE.md`, `Vystia/gm/GM_TESTING_GUIDE.md` | 26/26 | SoT should be `ServUO/Scripts/Custom/VystiaClasses/Classes/PlayerClassV2.cs` + `Vystia/reference/CLASSES.md` (verify counts). | code: enum count 27 in `PlayerClassTypeV2` (None + 26); test: `Vystia/gm/CLASS_TESTING_GUIDE.md`; 2026-01-23 |
| Magic (schools/spells) | `Vystia/design/magic/*`, `Vystia/Magic/` | `ServUO/Scripts/Custom/VystiaClasses/Spells/*` | N/A | `Vystia/reference/SPELLS.md`, `Vystia/gm/CLASS_TESTING_GUIDE.md` | 12 schools / 384 spells | SoT should be spell registration in `ServUO/Scripts/Custom/VystiaClasses/Spells/` + `Vystia/reference/SPELLS.md`. | code: spell folder has 386 .cs files; test: `Vystia/reference/SPELLS.md`; 2026-01-23 |
| Religions | `Vystia/design/WORLD_LORE.md`, `Vystia/design/systems/RELIGION_SYSTEM.md` | `ServUO/Scripts/Custom/VystiaClasses/Religion/*` | N/A | `Vystia/TESTING_GUIDE.md` | 6 religions | SoT should be `Vystia/design/systems/RELIGION_SYSTEM.md` + `VystiaClasses/Religion` enums (confirm lore naming alignment). | code: enum count 7 in `VystiaReligion` (None + 6); test: `Vystia/TESTING_GUIDE.md`; 2026-01-23 |
| Factions | `Vystia/design/FACTIONS.md`, `Vystia/design/systems/FACTION_SYSTEM.md` | `ServUO/Scripts/Custom/VystiaClasses/Factions/*` | N/A | `Vystia/gm/SYSTEM_TEST_GUIDE.md` | 7 factions | SoT should be `Vystia/design/FACTIONS.md` + `VystiaClasses/Factions` implementation. | code: enum count 8 in `VystiaFaction` (None + 7); test: `Vystia/gm/SYSTEM_TEST_GUIDE.md`; 2026-01-23 |
| Quest Systems (split) | `Vystia/design/CLASS_QUEST_SYSTEM_DESIGN.md`, `Vystia/implementation/LLM_DYNAMIC_QUEST_GENERATION.md` | Dynamic: `ServUO/Scripts/Custom/VystiaClasses/Quests/*`; Mondain: `ServUO/Scripts/Services/LLM/Core/LLMQuester.cs`, `ServUO/Scripts/Quests/*` | `ServUO/Data/VystiaQuests.xml` | `Vystia/TESTING_GUIDE.md`, `Vystia/reference/QUEST_GENERATION_QUICK_REF.md` | Phase 1 complete (dynamic + 6 Mondain quests) | SoT split: Dynamic via `VystiaQuestSystem`/`DynamicQuest`; Mondain via ServUO BaseQuest. | code: dynamic + Mondain paths present; runtime: VystiaQuests.xml exists (0 quests); test: `Vystia/TESTING_GUIDE.md`; 2026-01-23 |
| NPCs | `Vystia/design/VYSTIA_NPC_DESIGN.md`, `Vystia/reference/NPCS.md` | `ServUO/Scripts/Mobiles/Vystia/*`, `ServUO/Scripts/Custom/VystiaClasses/Quests/QuestNPC.cs` | `ServUO/Data/Vystia/npc_templates.json` | `Vystia/gm/NPC_TESTING_GUIDE.md` | 13 implemented / 400+ planned | SoT should be `ServUO/Scripts/Mobiles/Vystia/*` + `Vystia/reference/NPCS.md`. | code: 185 .cs files under `ServUO/Scripts/Mobiles/Vystia`; runtime: npc_templates.json exists; test: `Vystia/gm/NPC_TESTING_GUIDE.md`; 2026-01-23 |
| Economy | `Vystia/design/systems/ECONOMY_SYSTEM.md` | `ServUO/Scripts/Custom/VystiaClasses/Economy/*`, `ServUO/Scripts/Services/Economy/*`, `ServUO/Scripts/Custom/Commands/VystiaServiceNPCCommands.cs` | N/A | `Vystia/gm/SYSTEM_TEST_GUIDE.md` | Implemented (claimed) | SoT should be `VystiaClasses/Economy` + `Services/Economy` service layer. | code: 2 files in each Economy folder; test: `Vystia/gm/SYSTEM_TEST_GUIDE.md`; 2026-01-23 |
| Housing | `Vystia/design/systems/HOUSING_SYSTEM.md` | Core: `ServUO/Scripts/Multis/*`, `ServUO/Scripts/Items/Functional/House*`; Vystia: `ServUO/Scripts/Custom/VystiaClasses/Housing/*`, `ServUO/Scripts/Custom/VystiaClasses/Zones/VystiaZoneSystem.cs` | N/A | `Vystia/gm/SYSTEM_TEST_GUIDE.md` | Implemented (core ServUO + Vystia tax/price) | SoT is ServUO housing core + Vystia housing costs/tax overrides. | code: 2 Vystia housing files + zone AllowHousing override; test: `Vystia/gm/SYSTEM_TEST_GUIDE.md`; 2026-01-23 |
| LLM Lore/Memory | `Vystia/implementation/llm/*`, `Vystia/VYSTIA_MASTER_INVENTORY.md` | `ServUO/Scripts/Services/LLM/*` | `ServUO/Data/Lore/*`, `ServUO/Data/LLM/*` | `Vystia/TESTING_GUIDE.md` | 195 entries (claimed) | SoT should be `ServUO/Scripts/Services/LLM` + `ServUO/Data/Lore`. | runtime: Data/Lore folder exists; test: `Vystia/TESTING_GUIDE.md`; 2026-01-23 |
| Pets | `Vystia/design/systems/PET_SYSTEM.md` | `ServUO/Scripts/Custom/VystiaClasses/Pets/*` | N/A | `Vystia/TESTING_GUIDE.md` | Implemented (partial) | SoT should be `VystiaClasses/Pets`. | code: 5 pet system files; test: `Vystia/TESTING_GUIDE.md`; 2026-01-23 |
| Zones/PvP | `Vystia/design/systems/ZONE_SYSTEM.md` | `ServUO/Scripts/Custom/VystiaClasses/Zones/*` | N/A | `Vystia/gm/SYSTEM_TEST_GUIDE.md` | Implemented (claimed) | SoT should be zone system code + design doc. | pending |
| Sea/Underwater | `Vystia/design/SEA_SYSTEMS.md` | (verify) `ServUO/Scripts/Custom/VystiaClasses/Sea/*` | N/A | `Vystia/gm/SYSTEM_TEST_GUIDE.md` | Planned/partial | Confirm actual code paths. | pending |
| Crafting Tiers | `Vystia/design/systems/CRAFTING_SYSTEM.md` | `ServUO/Scripts/Custom/VystiaClasses/Crafting/*` | N/A | `Vystia/gm/SYSTEM_TEST_GUIDE.md` | Implemented (claimed) | SoT should be crafting tier gates + recipes. | pending |
| Combat/Abilities | `Vystia/design/systems/COMBAT_SYSTEM.md` | `ServUO/Scripts/Custom/VystiaClasses/Abilities/*`, `ServUO/Scripts/Custom/VystiaClasses/Systems/*` | N/A | `Vystia/gm/GM_TESTING_GUIDE.md` | Implemented (claimed) | SoT should be abilities + resource systems. | pending |
| Resources/Secondary | `Vystia/design/systems/RESOURCE_SYSTEM.md` | `ServUO/Scripts/Custom/VystiaClasses/Systems/VystiaResourceManager.cs` | N/A | `Vystia/gm/GM_TESTING_GUIDE.md` | Implemented (claimed) | SoT should be resource manager + per-class resource definitions. | pending |
| World Pipeline | `Vystia/README.md`, `d:\\UO\\README.md` | `VystiaTerrainGeneration/*`, `VystiaTownDeployment/*` | N/A | `Vystia/TESTING_GUIDE.md` | Tooling ready | SoT is pipeline repos + deployment scripts. | pending |
| AI Sidekicks | `Vystia/implementation/llm/*` | `ServUO/Scripts/Services/AISidekicks/*` | `ServUO/Data/LLM/*` | `Vystia/gm/NPC_TESTING_GUIDE.md` | Implemented (claimed) | SoT should be `Services/AISidekicks`. | pending |

## Follow-ups
- Populate Status with verified values during Phase 2 audit.
