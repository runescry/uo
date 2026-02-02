# VYSTIA SHARD OVERVIEW

Last Updated: 2026-01-23

This document is a single, consolidated snapshot of Vystia's world lore, systems, and technical pipeline. It is meant as a quick orientation for designers, builders, and implementers.

---

## 1) What Vystia Is
- Custom Ultima Online shard built on ServUO.
- Distinct fantasy world blending elemental magic with steampunk/industrial technology.
- Content is split between design/specs (Vystia/) and runtime code (ServUO/Scripts/).

Key entry points:
- Vystia/README.md
- Vystia/VYSTIA_MASTER_INVENTORY.md
- Vystia/design/README.md
- d:\UO\README.md

---

## 2) World & Lore Snapshot
- Core theme: elemental magic + mechanical innovation.
- Many regions with strong cultural identity (Ironclad Empire, Frosthold, Verdantpeak, Emberlands, Whispering Sands, Sunken Isles, Crystal Barrens, Shadowfen, Skyreach, etc.).
- Major political blocs drive conflict, trade, and diplomacy:
  - Ironclad Alliance
  - Sylvan Concord
  - League of Sands
  - Maritime Sovereignty
  - Highland Compact
  - Arcane Coalition
  - Polar Alliance
- Creatures are lore-gated: only region-appropriate monsters should spawn.

Primary references:
- Vystia/design/WORLD_LORE.md
- Vystia/design/CREATURES_BESTIARY.md
- Vystia/design/FACTIONS.md

---

## 3) Religions (Lore vs System)
Lore lists six religions tied to regions (Cogsmith Creed, Lunara's Covenant, Surya's Sandscript, Celestis Arcanum, Oceana's Covenant, Frosthelm Faith).
System implementation lists six religions with different names (Frosthelm Faith, Surya's Sandscript, Lunara's Covenant, Celestis Arcanum, Oceana's Covenant, Cogsmith Creed).

Action: reconcile naming and region mappings so lore, system, and UI are aligned.

Reference:
- Vystia/design/WORLD_LORE.md
- Vystia/design/systems/RELIGION_SYSTEM.md

---

## 4) Classes & Magic
- Design docs describe 26 classes.
- Inventory and README claim 25 implemented classes.
- Magic design targets 12 schools x 32 spells = 384 total.

Bard example (open design):
- Songweaving + Crescendo + Song Mastery + Virtuoso/Repertoire talents.
- Detailed combat loop, finales, and scaling systems.

Action: confirm class count and reconcile Bard spell implementation status vs design claims.

Reference:
- Vystia/design/CLASSES.md
- Vystia/VYSTIA_MASTER_INVENTORY.md
- Vystia/README.md
- Vystia/design/system_design/VYSTIA_BARD_SYSTEM.md
- Vystia/design/magic/BardicMagic.md

---

## 5) Core Gameplay Systems (Design Status)
- Zone Control: PvP consent, death penalties, loot drop rules by zone.
- Economy: service fees, travel costs, stabling, repairs, gold sinks.
- Housing: tiered purchase costs + weekly taxes and decay.
- Pet System: class-specific companions with tier scaling.
- Sea/Underwater Systems: breath/pressure, gear, salvage, underwater combat.
- Faction System: reputation tiers, tokens, vendors, rewards.
- Quest Systems (implementation split):
  - Vystia Dynamic Quests: LLM-generated plans -> DynamicQuest + QuestNPCs (Chronicler, Quest Wizard).
  - Mondain/BaseQuest: classic quest chain system (MondainQuester, BaseQuester, LLMQuester).

Reference:
- Vystia/design/systems/ZONE_SYSTEM.md
- Vystia/design/systems/ECONOMY_SYSTEM.md
- Vystia/design/systems/HOUSING_SYSTEM.md
- Vystia/design/systems/PET_SYSTEM.md
- Vystia/design/SEA_SYSTEMS.md
- Vystia/design/FACTIONS.md

---

## 6) Content Pipeline (How the World Is Built)
1. Terrain generation (VystiaTerrainGeneration) creates base map.
2. Town deployment (VystiaTownDeployment) places structures and statics.
3. Vystia design docs define creatures, loot, items, and lore.
4. ServUO/Scripts hosts runtime content.

Reference:
- d:\UO\README.md
- Vystia/README.md

---

## 7) Current Status Signals (From Inventory/README)
- Creatures: 131 implemented.
- Magic: 12 schools / 384 spells claimed complete.
- Equipment: 172 items claimed complete.
- NPCs/Quests: early phases complete; expansion planned.
  - Note: Vystia Dynamic Quests and Mondain/BaseQuest are parallel systems with different tools/NPCs.

Reference:
- Vystia/README.md
- Vystia/VYSTIA_MASTER_INVENTORY.md

---

## 8) Known Gaps / Mismatches (Need Reconciliation)
- Class count mismatch: 26 (design) vs 25 (inventory/README).
- Religion naming mismatch: lore vs system implementation.
- Bardic magic: design doc is deep and specific; BardicMagic.md says all spells are placeholder STR buffs.
- Some README claims may be outdated relative to design docs.
- Quest stack split: Vystia Dynamic Quests (LLM + QuestNPC) vs Mondain/BaseQuest (LLMQuester) are separate systems with different tooling.

---

## 9) Suggested Next Actions (If You Want a Follow-Up)
- Resolve lore/system naming for religions.
- Verify actual class count in code vs design docs.
- Audit Bardic magic implementation status against design spec.
- Create a short "source of truth" index for each system (design doc -> implementation folder -> status).

---

## Quick Navigation
- World lore: Vystia/design/WORLD_LORE.md
- Bestiary: Vystia/design/CREATURES_BESTIARY.md
- Classes: Vystia/design/CLASSES.md
- Systems index: Vystia/design/systems/README.md
- Full inventory: Vystia/VYSTIA_MASTER_INVENTORY.md
- Bard system: Vystia/design/system_design/VYSTIA_BARD_SYSTEM.md
- Bard magic status: Vystia/design/magic/BardicMagic.md

