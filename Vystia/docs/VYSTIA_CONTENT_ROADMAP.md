# Vystia Missing Content Guide

## Overview
This document enumerates content described by the Vystia lore and guides that is not yet implemented, and proposes practical, UO:R-compatible implementations. Use this as a delivery backlog.

---

## 🛡️ Factions & Reputation

### Status
- No faction definitions, standings, tokens, or reputation rewards.
- No faction vendors or turn-in systems.

### Proposal
- Add faction system with tokens per alliance: Ironclad Alliance, Sylvan Concord, League of Sands, Maritime Sovereignty, Highland Compact, Arcane Coalition, Polar Alliance.
- Implement token drops from regional bosses and quest rewards.
- Add reputation thresholds unlocking vendors, cosmetics, regional recipes.

### Implementation Notes
- New items: `IroncladToken`, `SylvanToken`, etc.
- Vendors: `IroncladBlacksmith`, `SylvanHerbalist`, etc.
- Reputation storage: Player `XmlAttachment` or serialized profile.

---

## 📜 Questlines & Narrative Arcs

### Status
- No quests for artifact arcs: Heartwood Core, Magma Heart, Luminous Scepter, Mirror of Truth.
- No regional quest chains, bounties (e.g., Crypt Trackers), or achievements.

### Proposal
- Create 2–3 quest chains per region with escalating objectives and region-appropriate rewards.
- Artifact arcs culminate in boss dungeons with guaranteed drop roll items.
- Add bounty boards in island ports for Sunken Isles (treasure recovery, fugitive hunts).

### Implementation Notes
- Use ServUO `BaseQuest` or lightweight gump/dialogue scripts.
- Place quest givers in city `features` from `Vystia_World_Config.json`.

---

## 🧑‍🤝‍🧑 City Vendors & NPCs

### Status
- JSON features mention markets, potion shops, illusionists, curse vendors; not implemented.
- No notable NPCs with dialogue or rumors.

### Proposal
- Add vendor inventories per region theme (potions, illusions, curses, druidry, smithing).
- Seed each city with 3–6 themed NPCs, 1 quest giver, 1 vendor, 1 guard captain.

### Implementation Notes
- Vendor buy/sell lists tuned to regional resources from `./VYSTIA_ITEMS_GUIDE.md`.
- Dialogue scripts surface local rumors and quest hooks.

---

## ⚒️ Items, Crafting & Resources

### Status
- `./VYSTIA_ITEMS_GUIDE.md` defines items; no ServUO classes, recipes, or resource nodes.

### Proposal
- Implement artifact items, regional resources, crafting recipes, and region-locked stations (Magma Heart forge).
- Add gathering nodes: Crystal Veins, Heartwood Trees, Oasis Wells.

### Implementation Notes
- Add resources to skinning/mining/foraging tables and boss drops.
- Region-locked recipes enforced via map/region checks or reputation.

---

## 👑 Dungeon Bosses, Keys & Rewards

### Status
- Spawners exist; no bespoke boss AI, phases, key gates, or unique reward tables.

### Proposal
- Add boss mechanics and unique drops aligned with lore artifacts.
- Introduce progression keys (e.g., Sphinx Sigil, Frost Seal, Ember Core) for final chambers.

### Implementation Notes
- Guarantee one artifact component on boss kill (group roll).
- Boss AI examples: Frost Father's shield phases; Volcano Wyrm lava burst; Sphinx riddles.

---

## 🏙️ City/Region Simulation

### Status
- No guards, patrols, or law variations per city; no placed buildings matching features.

### Proposal
- Place city layouts with functional buildings and signage matching `features`.
- Add guard patrol routes, region crime rules, and ambient spawners.

### Implementation Notes
- Use XMLSpawner for ambient life; add sign items and deco kits per biome.

---

## ⚖️ Economy & Trade

### Status
- No regional tokens/currencies beyond gold; no trade route events.

### Proposal
- Add regional tokens (used by vendors), caravan events, and shipping contracts between islands.

### Implementation Notes
- Contract items with expiration; payout on delivery; piracy encounters in sea lanes.

---

## 🌊 Sea & Underwater Systems

### Status
- Abyssal Trench spawners exist; no underwater rules, breath timers, or treasure systems.

### Proposal
- Implement underwater movement penalties, breath meter, and diving gear.
- Add sunken chests with lockpicking/salvage mini-games; Crypt Tracker bounties.

### Implementation Notes
- Diving gear as equipable layer with breath duration; underwater-only spawns and deco.

---

## ✨ Magic Schools & Utilities

### Status
- No Nature/Light/Shadow/Solar magic augments or scrying systems.

### Proposal
- Add scrolls, staves, and trinkets that modify existing spells (damage type, utility effects).
- Implement scrying mirrors in Arcane Coalition cities for event hooks.

### Implementation Notes
- Use item `OnCast` hooks or spell overrides gated by items/reputation.

---

## 🏠 Housing & Decorations

### Status
- No placeable deco/furniture packs to reflect regional identity.

### Proposal
- Create craftable/lootable deco sets: Frosthold, Desert, Forest, Crystal, Industrial.

### Implementation Notes
- Recipes unlocked by regional reputation; use existing art assets and hues.

---

## 🗺️ World Integration & UX

### Status
- No map pins/markers, in-game guidebooks, or libraries surfacing the lore.

### Proposal
- Add cartography markers for cities/dungeons; place libraries with readable books summarizing lore.

### Implementation Notes
- Starter guidebook placed in new player backpacks; region-specific field guides sold by vendors.

---

## 📑 Rollout Plan (Backlog)

1. Artifacts + boss loot integration (Phase 1)
2. Resources + crafting recipes + nodes (Phase 1)
3. Faction tokens, vendors, and rep tracking (Phase 1)
4. City NPCs and vendor inventories (Phase 2)
5. Questlines for 4 priority regions (Phase 2)
6. Boss mechanics + key gates (Phase 2)
7. Underwater systems + sunken treasure (Phase 3)
8. Magic augments + scrying utilities (Phase 3)
9. Housing/deco sets + economy events (Phase 3)
10. World UX: map pins, books, libraries (Phase 3)

---

## Links
- Items & Artifacts: `./VYSTIA_ITEMS_GUIDE.md`
- World Generation: `../VYSTIA_GENERATION_GUIDE.md`
- Creatures & Spawners: `../VYSTIA_CREATURES_GUIDE.md`
- World Lore: `../Vystia World Lore.md`
