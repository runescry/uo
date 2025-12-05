# Vystia Documentation Index (docs/)

**Last Updated:** December 2024

---

## Status Documents (Current)

| Document | Purpose | Priority |
|----------|---------|----------|
| [VYSTIA_MASTER_STATUS.md](./VYSTIA_MASTER_STATUS.md) | **Overall implementation status** | ⭐ Start Here |
| [VYSTIA_IMPLEMENTATION_PLAN.md](./VYSTIA_IMPLEMENTATION_PLAN.md) | 7-phase implementation roadmap | ⭐ Reference |
| [VYSTIA_IMPLEMENTATION_ANALYSIS.md](./VYSTIA_IMPLEMENTATION_ANALYSIS.md) | Cross-reference & gap analysis | ⭐ Reference |

---

## Design Documents (Complete)

| Document | Purpose |
|----------|---------|
| [VYSTIA_WORLD_LORE.md](./VYSTIA_WORLD_LORE.md) | World background, 8 regions, history |
| [VYSTIA_CREATURES_BESTIARY_FULL.md](./VYSTIA_CREATURES_BESTIARY_FULL.md) | 150+ creatures with body IDs, stats |
| [VYSTIA_DUNGEON_BOSSES.md](./VYSTIA_DUNGEON_BOSSES.md) | 10 bosses with phase mechanics |
| [VYSTIA_ITEMS_GUIDE.md](./VYSTIA_ITEMS_GUIDE.md) | Artifacts, resources, consumables |
| [VYSTIA_FACTIONS_GUIDE.md](./VYSTIA_FACTIONS_GUIDE.md) | Political factions & conflicts |
| [VYSTIA_SEA_SYSTEMS.md](./VYSTIA_SEA_SYSTEMS.md) | Naval & underwater content |

---

## Reference Documents

| Document | Purpose |
|----------|---------|
| [VYSTIA_CONTENT_ROADMAP.md](./VYSTIA_CONTENT_ROADMAP.md) | Missing content guide |
| [VYSTIA_ART_IMPLEMENTATION.md](./VYSTIA_ART_IMPLEMENTATION.md) | Art & graphics notes |

---

## Archived Documents

*Moved to `docs/archive/` - superseded or historical only*

| Document | Reason Archived |
|----------|-----------------|
| VYSTIA_CREATURES_BESTIARY.md | Superseded by VYSTIA_CREATURES_BESTIARY_FULL.md |
| VYSTIA_CREATURES_BESTIARY_T2A.md | T2A-only variant, superseded |
| VYSTIA_TROUBLESHOOTING_LOG.md | Historical issues log, resolved |

---

## Implementation Guides (in guides/)

| Document | Purpose |
|----------|---------|
| [VYSTIA_EQUIPMENT_GUIDE.md](../guides/VYSTIA_EQUIPMENT_GUIDE.md) | 200+ weapons, armor, shields |
| [VYSTIA_RESOURCES_GUIDE.md](../guides/VYSTIA_RESOURCES_GUIDE.md) | 57+ ores, woods, reagents |
| [VYSTIA_CREATURES_IMPLEMENTATION.md](../guides/VYSTIA_CREATURES_IMPLEMENTATION.md) | Creature code templates |
| [VYSTIA_TERRAIN_GENERATION_GUIDE.md](../guides/VYSTIA_TERRAIN_GENERATION_GUIDE.md) | World terrain generation |
| [VYSTIA_DEPLOYMENT_GUIDE.md](../guides/VYSTIA_DEPLOYMENT_GUIDE.md) | Server deployment |

---

## Quick Links

### Currently Implemented in ServUO:
- ✅ **10 Regional Bosses** (ALL COMPLETE)
  - Frost Father, Volcano Wyrm, Sphinx of Surya, Coven Matriarch
  - Ancient Treant, Crystal Drake Alpha, Griffin Lord, Ancient Kraken
  - Forge Master, Timeworn Lich
- ✅ **5 Legendary Weapons** (ALL COMPLETE)
  - The Eternal Winter, Phoenix Ascension, The Cogmaster, Prismatic Edge, Voidcaller
- ✅ **61 Resources** (ALL COMPLETE)
  - 8 ores, 8 ingots, 14 woods, 6 leathers, 13 reagents, 3 components, 8 special
- ✅ **37 Regional Weapons** (4 per region)
- ✅ **48 Armor Pieces** (6 per region × 8 regions)
- ✅ **8 Regional Shields** (1 per region)
- ✅ **8 Regional Loot Packs** (ALL COMPLETE)

### Next to Implement:
- 140 non-boss creatures
- Crafting system integration

### GM Commands:
```
-- Bosses --
[add FrostFather         [add VolcanoWyrm
[add SphinxOfSurya       [add CovenMatriarch
[add AncientTreant       [add CrystalDrakeAlpha
[add GriffinLord         [add AncientKraken
[add ForgeMaster         [add TimewornLich

-- Legendary Weapons --
[add TheEternalWinter    [add PhoenixAscension
[add TheCogmaster        [add PrismaticEdge
[add Voidcaller

-- Loot Test --
[droploot                [clearloot
```

---

*See [DOCS_INDEX.md](../DOCS_INDEX.md) in root for full navigation*
