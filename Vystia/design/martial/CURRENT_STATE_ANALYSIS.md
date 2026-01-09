# Martial Classes - Current State Analysis

**Date:** 2025-12-13  
**Purpose:** Document current implementation status of all 14 martial classes

---

## Overview

All 14 martial classes have **16 abilities each** (4 tiers × 4 abilities), for a total of **224 abilities**.

**Ability ID Range:** 2000-2223 (sequential, no conflicts)

---

## Current State by Class

| Class | Ability IDs | Status | Abilities | Notes |
|-------|------------|--------|-----------|-------|
| Fighter | 2000-2015 | ✅ Complete | 16 | Weapon master, stance system |
| Barbarian | 2016-2031 | ✅ Complete | 16 | Rage system, burst damage |
| Monk | 2032-2047 | ✅ Complete | 16 | Chi system, combo points |
| Rogue | 2048-2063 | ✅ Complete | 16 | Stealth, combo points |
| Ranger | 2064-2079 | ✅ Complete | 16 | Focus system, terrain mastery |
| Knight | 2080-2095 | ✅ Complete | 16 | Shield mechanics, tank |
| Paladin | 2096-2111 | ✅ Complete | 16 | Tithing, virtue system |
| Templar | 2112-2127 | ✅ Complete | 16 | Zeal stacks, judgment |
| Bounty Hunter | 2128-2143 | ✅ Complete | 16 | Marks, pursuit stacks |
| Beastmaster | 2144-2159 | ✅ Complete | 16 | Bond system, pack tactics |
| Artificer | 2160-2175 | ✅ Complete | 16 | Steam, charges, turrets |
| Alchemist | 2176-2191 | ✅ Complete | 16 | Reagents, mutagens |
| Cleric | 2192-2207 | ✅ Complete | 16 | Faith, prayer system |
| Wizard | 2208-2223 | ✅ Complete | 16 | Spell power, metamagic |

**Total:** 224 abilities across 14 classes

---

## Ability Distribution Analysis

### By Type (Estimated)

| Type | Count | Percentage | Notes |
|------|-------|-----------|-------|
| Damage | ~80 | 35.7% | Direct damage abilities |
| Utility | ~60 | 26.8% | Buffs, debuffs, mobility |
| Defense | ~40 | 17.9% | Shields, mitigation, healing |
| Mobility | ~20 | 8.9% | Movement, escape abilities |
| Control | ~24 | 10.7% | CC, interrupts, stuns |

**Note:** Exact distribution needs verification from implementation files.

---

## Implementation Files

**Server-Side:**
- `ServUO/Scripts/Custom/VystiaClasses/Abilities/` - Individual ability files
- `ServUO/Scripts/Custom/VystiaClasses/Abilities/*Abilities.cs` - Class ability registrations

**Client-Side:**
- `ClassicUO/src/ClassicUO.Client/Game/Data/Abilities.cs` - Client ability definitions

---

## Verification Status

- ✅ **ID Alignment:** All 224 abilities have unique, sequential IDs (2000-2223)
- ✅ **No Conflicts:** Verified no duplicate IDs
- ⏳ **Balance Analysis:** Pending archetype comparison
- ⏳ **Future State:** Pending industry standard comparison

---

**Last Updated:** 2025-12-13  
**Next Steps:** Create future state proposals based on archetype analysis

