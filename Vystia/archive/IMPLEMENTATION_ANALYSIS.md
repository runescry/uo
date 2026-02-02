# Vystia Implementation Analysis Report
## Task 4: Current Systems Analysis

Generated: 2026-01-03

---

## Summary

This report documents all currently implemented Vystia systems in the ServUO codebase.
Analysis covers:
- VystiaClasses (18 subfolders)
- Items/Vystia
- Mobiles/Vystia
- Services/LLM
- Services/AISidekicks

---

## 1. New Gameplay Systems (Recently Added)

### 1.1 Pet System
**Location:** `VystiaClasses/Pets/` (5 files, 2,984 lines)
**Status:** FULLY IMPLEMENTED

| Component | File | Lines | Description |
|-----------|------|-------|-------------|
| Core System | VystiaPetSystem.cs | 522 | Base pet framework, summoning, stats |
| Beastmaster | BeastmasterPets.cs | 641 | Tamed beast companions |
| Artificer | ArtificerPets.cs | 671 | Mechanical constructs |
| Necromancer | NecromancerPets.cs | 554 | Undead minions |
| Summoner | SummonerPets.cs | 596 | Elemental summons |

**Pet Types:**
- TamedBeast (Beastmaster - permanent, bondable)
- SummonedElemental, SummonedCreature (Summoner - timed)
- UndeadMinion, UndeadServant (Necromancer - timed)
- MechanicalConstruct, ClockworkServant (Artificer - timed)

**Pet Tiers:** Lesser (1 slot) - Standard (2) - Greater (3) - Superior (4) - Legendary (5)

**GM Commands:**
- `[SummonPet <type> [tier]` - Summon test pet
- `[DismissPets [type]` - Dismiss pets
- `[PetInfo` - Show pet status

---

### 1.2 Housing System
**Location:** `VystiaClasses/Housing/` (2 files, 1,134 lines)
**Status:** FULLY IMPLEMENTED

| Component | File | Lines | Description |
|-----------|------|-------|-------------|
| Costs | VystiaHousingCosts.cs | 455 | Purchase prices, taxes |
| Taxes | VystiaPropertyTax.cs | 679 | Weekly tax collection |

**Housing Tiers:**
| Size | Dimensions | Purchase | Weekly Tax |
|------|------------|----------|------------|
| Small | 7x7 | 50,000g | 500g |
| Medium | 11x11 | 150,000g | 1,500g |
| Large | 15x15 | 400,000g | 4,000g |
| Keep | 18x18 | 1,000,000g | 10,000g |
| Castle | 31x31 | 3,000,000g | 30,000g |

**GM Commands:**
- `[HouseCosts` / `[HC` - Display all costs
- `[SetHousePrice <amount>` / `[SHP` - Override house price
- `[HouseInfo` / `[HI` - Show house details

---

### 1.3 Zone Control System
**Location:** `VystiaClasses/Zones/` (1 file, 726 lines)
**Status:** FULLY IMPLEMENTED

| Zone Type | Color | PvP | Consent | Guards | Loot Drop | XP Bonus |
|-----------|-------|-----|---------|--------|-----------|----------|
| Sanctuary | Green | No | N/A | Yes | 0% | +0% |
| Contested | Yellow | Yes | Required | Yes | 10% | +25% |
| Lawless | Red | Yes | No | No | 50% | +50% |
| Extreme | Black | Yes | No | No | 100% | +100% |

**Features:**
- Per-zone death penalties and skill loss
- Permadeath risk in Extreme zones
- Default region mappings (towns = Sanctuary, wilderness = Contested)
- Custom VystiaZoneRegion class

**GM Commands:**
- `[ZoneInfo` / `[ZI` - Current zone info
- `[SetZone <region> <type>` / `[SZ` - Configure zone
- `[ZoneList` / `[ZL` - List all zones
- `[TogglePvP` - Toggle PvP flag

---

### 1.4 Faction System
**Location:** `VystiaClasses/Factions/` (3 files, 1,384 lines)
**Status:** FULLY IMPLEMENTED

| Faction | Region | Enemy |
|---------|--------|-------|
| Frostguard | Frosthold | Flame Legion |
| Flame Legion | Emberlands | Frostguard |
| Greenward | Verdantpeak | Voidborn |
| Arcane Conclave | Crystal Barrens | - |
| Technoguild | Ironclad | - |
| Sandwalkers | Desert | - |
| Voidborn | ShadowVoid | Greenward |

**Reputation Tiers:**
| Tier | Threshold | Vendor Discount |
|------|-----------|-----------------|
| Hostile | <-1000 | None |
| Unfriendly | -1000 to 0 | None |
| Neutral | 0 to 3000 | None |
| Friendly | 3000 to 6000 | 5% |
| Honored | 6000 to 12000 | 8% |
| Revered | 12000 to 15000 | 12% |
| Exalted | 15000+ | 15% |

**GM Commands:**
- `[Factions` - Show all standings
- `[Faction <1-7>` - Detailed faction info
- `[SetReputation <faction> <amount>` - Set exact rep
- `[AddReputation <faction> <amount>` - Modify rep
- `[DonateFaction <faction> <gold>` - Donate for rep

---

### 1.5 Economy/Gold Sink System
**Location:** `VystiaClasses/Economy/` (2 files, 1,096 lines)
**Status:** FULLY IMPLEMENTED

**Service Fees (VystiaServiceFees.cs):**
| Service | Base Cost | Scaling |
|---------|-----------|---------|
| Resurrection | 50g | +10g per fame level |
| Short Travel | 100g | <500 tiles |
| Medium Travel | 175g | 500-1500 tiles |
| Long Travel | 250g | >1500 tiles |
| Pet Stabling | 30g/day | 150g/week discounted |

**NPCs:**
- VystiaHealer - Resurrection service (speech: "resurrect", "heal")
- VystiaMoongateAttendant - Travel fees (speech: "travel", "gate")

**Repair System (VystiaRepairService.cs):**
- Durability-based repair costs
- Gold sink for equipment maintenance

---

### 1.6 Religion System
**Location:** `VystiaClasses/Religion/` (2 files, 662 lines)
**Status:** FULLY IMPLEMENTED

**6 Religions:**
| Religion | Region | Passive Bonuses |
|----------|--------|-----------------|
| Frosthelm Faith | Frosthold | Cold Resist +5%, Cold Dmg +3% |
| Surya's Sandscript | Emberlands | Fire Resist +5%, Fire Dmg +3% |
| Lunara's Covenant | Verdantpeak | Poison Resist +5%, Healing +5% |
| Celestis Arcanum | Crystal Barrens | Energy Resist +5%, Mana Regen +2 |
| Oceana's Covenant | ShadowVoid | Physical Resist +3%, Stealth +5 |
| Cogsmith Creed | Ironclad | Armor +5, Crafting +5 |

**Piety Tiers:**
| Tier | Threshold | Unlocks |
|------|-----------|---------|
| None | 0-49 | - |
| Initiate | 50-199 | First passive bonus |
| Devoted | 200-499 | First devotion power |
| Faithful | 500-899 | Second devotion power |
| Exalted | 900-1000 | Third devotion power |

**Piety Actions:**
- Daily Prayer: +10 piety (once per 24h)
- Tithing: +1 piety per 100g donated
- Pilgrimage: +75 piety (shrine visit)

**GM Commands:**
- `[Religion` - Show status
- `[Convert <1-6>` - Convert to religion
- `[Pray` - Daily prayer
- `[Tithe <gold>` - Donate gold
- `[SetPiety <amount>` - Set exact piety

---

### 1.7 Crafting System
**Location:** `VystiaClasses/Crafting/` (3 files, 682 lines)
**Status:** FULLY IMPLEMENTED

| Component | File | Lines | Description |
|-----------|------|-------|-------------|
| Recipes | VystiaCraftingRecipes.cs | 220 | Regional recipes |
| Engineering | DefEngineering.cs | 260 | Mechanical crafting |
| Transmutation | DefTransmutation.cs | 202 | Alchemical crafting |

---

## 2. Core Systems (Previously Documented)

### 2.1 Character Classes v2.0
**Location:** `VystiaClasses/Classes/` (21 files)
**Status:** 100% COMPLETE (25/25 classes)

### 2.2 Secondary Resources
**Location:** `VystiaClasses/Core/` (2 files)
- 15 resource types (Soul Shards, Fury, Chi, etc.)

### 2.3 Combat Systems
**Location:** `VystiaClasses/Systems/` (7 files)
- Damage Pipeline
- Buff/Debuff System
- Crowd Control with DR
- Target Tracker

### 2.4 Abilities
**Location:** `VystiaClasses/Abilities/` (3 files)
- 512 data-driven abilities
- Python automation for generation

### 2.5 Spells
**Location:** `VystiaClasses/Spells/` (2+ files)
- 384 spells across 12 magic schools
- All spellbooks functional

---

## 3. Content Systems

### 3.1 Creatures
**Location:** `Mobiles/Vystia/` (138 creatures)
- 10 regions + bosses
- Regional resource drops
- Unique combat abilities

### 3.2 Items/Resources
**Location:** `Items/Vystia/`
- 96 magic reagents (8 per school)
- 384 spell scrolls
- Regional resources (ores, woods, leathers)
- 131 equipment pieces

### 3.3 Quests
**Location:** `VystiaClasses/Quests/` (6 files)
- Multi-chain quest system
- LLM-powered NPCs
- Waypoint system

---

## 4. Support Systems

### 4.1 LLM NPC System
**Location:** `Services/LLM/` (46 files)
- Context-aware NPC dialogue
- Personality types
- Speech patterns

### 4.2 AI Sidekicks
**Location:** `Services/AISidekicks/` (27 files)
- 3 archetypes: Mage, Warrior, Tamer
- Python combat simulation
- A* pathfinding

---

## 5. GM Tools

### 5.1 Gumps
**Location:** `VystiaClasses/Gumps/` (11 files)
- Class selection
- Quest editor
- Ability editor
- Resource management

### 5.2 Commands
**Location:** `VystiaClasses/Commands/` (4 files)
- 147+ GM commands documented

---

## Documentation Gaps Identified

1. **New systems lacking dedicated docs:**
   - Pet System (no design doc found)
   - Housing System (no design doc found)
   - Zone System (no design doc found)
   - Religion System (no design doc found)

2. **Outdated documentation:**
   - Reference docs may not include new commands
   - Implementation status needs update

3. **Duplicate documentation:**
   - Similar content in ServUO and Vystia folders
   - Need consolidation

---

*Analysis complete. Proceed to Task 5 for documentation reorganization.*

