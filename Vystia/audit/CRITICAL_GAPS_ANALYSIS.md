# Critical Gaps Analysis

**Date:** 2025-01-10  
**Status:** Comprehensive Gap Analysis  
**Purpose:** Identify and prioritize all missing features and incomplete implementations

---

## Executive Summary

This document provides a detailed analysis of all critical gaps in the Vystia implementation, prioritized by impact and implementation complexity. The analysis covers missing features, incomplete systems, and integration gaps.

**Overall Gap Status:**
- 🔴 **Critical Gaps:** 4 areas
- 🟡 **High Priority Gaps:** 3 areas
- 🟢 **Medium Priority Gaps:** 4 areas
- ⚪ **Low Priority Gaps:** 2 areas

---

## 🔴 Critical Missing Features

### 1. Missing Crafting Recipes (7 Disciplines)

**Priority:** CRITICAL  
**Impact:** High - Classes cannot craft their unique items  
**Estimated Effort:** 2-3 weeks  
**Dependencies:** None

**Missing Recipe Implementations:**

| Discipline | Primary Class | Status | Recipes Needed |
|------------|---------------|--------|----------------|
| Runecrafting | Enchanter | System exists | Enchantments, runes, protection items |
| Inscription | Oracle | System exists | Divination scrolls, prophecy items |
| Leathercraft | Ranger | System exists | Leather armor, quivers, ranger gear |
| Woodshaping | Druid | System exists | Staves, totems, bows, nature items |
| Clothcraft | Bard | System exists | Robes, cloaks, musical instruments |
| Necrocraft | Necromancer | System exists | Bone items, soul vessels, necromantic gear |
| Jewelcraft | Sorcerer | System exists | Jewelry, foci, elemental items |

**Required Components:**
- Recipe definitions for each discipline
- Material requirements
- Workstation items (if not exist)
- Vendor integration
- Quality tiers (Standard, Quality, Exceptional, Masterwork, Legendary)

**Files to Create/Update:**
- `ServUO/Scripts/Custom/VystiaClasses/Crafting/DefRunecrafting.cs` (add recipes)
- `ServUO/Scripts/Custom/VystiaClasses/Crafting/DefInscription.cs` (add recipes)
- `ServUO/Scripts/Custom/VystiaClasses/Crafting/DefLeathercraft.cs` (add recipes)
- `ServUO/Scripts/Custom/VystiaClasses/Crafting/DefWoodshaping.cs` (add recipes)
- `ServUO/Scripts/Custom/VystiaClasses/Crafting/DefClothcraft.cs` (add recipes)
- `ServUO/Scripts/Custom/VystiaClasses/Crafting/DefNecrocraft.cs` (add recipes)
- `ServUO/Scripts/Custom/VystiaClasses/Crafting/DefJewelcraft.cs` (add recipes)

**Implementation Steps:**
1. Define recipe list for each discipline (10-20 recipes per discipline)
2. Create material requirements
3. Define skill requirements
4. Add recipes to crafting system
5. Test crafting functionality
6. Integrate with vendors

---

### 2. Missing Transmutation Resource Potions

**Priority:** CRITICAL  
**Impact:** High - Secondary resource management incomplete  
**Estimated Effort:** 1 week  
**Dependencies:** Potion effect system

**Missing Recipes:**
- Fury Draught (+15 max Fury, 10 min)
- Berserker's Blood (Fury decay -35%, 5 min)
- Chi Elixir (+1 max Chi, 10 min)
- Focused Serum (Focus decay -35%, 5 min)
- Zealot's Tonic (+1 Zeal per kill, 5 min)
- Knight's Fortifier (+2 max Fortitude, 10 min)
- Hunter's Mark Oil (Mark duration +35%)
- Shard Catalyst (+5% Soul Shard gen)
- LifeForce Flask (Store 35 LifeForce)
- Chill Enhancer (+5% freeze duration)
- Crescendo Catalyst (Crescendo gen +35%)
- Faith Vessel (Store 35 Faith)
- Steam Concentrate (Portable Steam, 35 charges)
- Virtue Essence (+1 to one Virtue)

**Required Components:**
- Potion item classes (14 potion types)
- Potion effect system for secondary resources
- Recipe definitions in DefTransmutation.cs
- Material requirements

**Files to Create/Update:**
- `ServUO/Scripts/Items/Vystia/Consumables/ResourcePotions.cs` (new file)
- `ServUO/Scripts/Custom/VystiaClasses/Crafting/DefTransmutation.cs` (add recipes)
- `ServUO/Scripts/Custom/VystiaClasses/Systems/SecondaryResource.cs` (add potion effects)

**Implementation Steps:**
1. Create potion item classes
2. Implement potion effect system
3. Add recipes to DefTransmutation.cs
4. Test potion effects
5. Integrate with vendors

---

### 3. Missing Engineering Constructs

**Priority:** CRITICAL  
**Impact:** Medium - Artificer class incomplete  
**Estimated Effort:** 2 weeks  
**Dependencies:** Construct AI system

**Missing Constructs:**
- Clockwork Spider (50 HP, Scout, 1 slot)
- Repair Drone (Heals constructs, 1 slot)
- Steam Turret (100 HP, Ranged, 2 slots)
- Iron Golem (500 HP, Tank, 3 slots)
- Siege Engine (Territory warfare, 5 slots)

**Required Components:**
- Construct item classes (5 construct types)
- Construct summoning system
- Construct AI/behavior
- Recipe definitions in DefEngineering.cs
- Material requirements

**Files to Create/Update:**
- `ServUO/Scripts/Items/Vystia/Constructs/` (new directory)
- `ServUO/Scripts/Custom/VystiaClasses/Crafting/DefEngineering.cs` (add recipes)
- `ServUO/Scripts/Custom/VystiaClasses/Pets/ConstructAI.cs` (new file)

**Implementation Steps:**
1. Create construct item classes
2. Implement construct AI/behavior
3. Implement construct summoning system
4. Add recipes to DefEngineering.cs
5. Test construct functionality

---

### 4. Incomplete Devotion Powers

**Priority:** CRITICAL  
**Impact:** Medium - Some powers not fully functional  
**Estimated Effort:** 1 week  
**Dependencies:** None

**TODOs in Code:**
- **Endurance of Winter:** "Cannot die" flag not implemented
- **Nature's Sanctuary:** Zone-based healing bonus not implemented
- **Celestial Alignment:** Spell count tracking not fully implemented
- **Abyssal Call:** Water elemental summon not implemented

**Files to Update:**
- `ServUO/Scripts/Custom/VystiaClasses/Religion/VystiaDevotionPowers.cs`

**Implementation Steps:**
1. Implement "cannot die" flag for Endurance of Winter
2. Implement zone-based healing bonus for Nature's Sanctuary
3. Complete spell count tracking for Celestial Alignment
4. Implement water elemental summon for Abyssal Call
5. Test all devotion powers
6. Update documentation

---

## 🟡 High Priority Missing Features

### 5. Missing Portable Shrines

**Priority:** HIGH  
**Impact:** Low - Convenience feature  
**Estimated Effort:** 1 week  
**Dependencies:** None

**Missing Items:**
- Cogsmith's Portable Anvil (Cogsmith Creed)
- Moonstone Circle (Lunara's Covenant)
- Sun Dial (Surya's Sandscript)
- Tide Pool Basin (Oceana's Covenant)
- Star Chart (Celestis Arcanum)
- Frost Totem (Frosthelm Faith)

**Required Components:**
- Portable shrine item classes (6 items)
- Shrine functionality in portable form
- Crafting recipes
- Material requirements

**Files to Create:**
- `ServUO/Scripts/Items/Vystia/Religious/PortableShrines.cs` (new file)

**Implementation Steps:**
1. Create portable shrine item classes
2. Implement shrine functionality
3. Add crafting recipes
4. Test portable shrine functionality

---

### 6. Missing Major Temples

**Priority:** HIGH  
**Impact:** Low - Cosmetic/roleplay feature  
**Estimated Effort:** 2 weeks  
**Dependencies:** None

**Missing Temples:**
- The Grand Foundry (Cogsmith Creed, Ironclad Capital)
- Grove of the Moon (Lunara's Covenant, Verdantpeak Heart)
- Temple of the Sun (Surya's Sandscript, Desert Oasis)
- Abyssal Cathedral (Oceana's Covenant, Forgotten Depths)
- Astral Observatory (Celestis Arcanum, Crystal Barrens Peak)
- Frost Father's Sanctum (Frosthelm Faith, Frosthold Summit)

**Required Components:**
- Temple structures (6 temples)
- Special temple functions:
  - Engineering legendary blessing (Grand Foundry)
  - Moonlight healing pool (Grove of the Moon)
  - Solar prophecy chamber (Temple of the Sun)
  - Underwater resurrection (Abyssal Cathedral)
  - Starlight enchanting (Astral Observatory)
  - Endurance trials (Frost Father's Sanctum)

**Files to Create:**
- `ServUO/Scripts/Items/Vystia/Religious/Temples/` (new directory)

**Implementation Steps:**
1. Create temple structure items
2. Implement special temple functions
3. Place temples in world
4. Test temple functionality

---

### 7. Faction System Enhancements

**Priority:** HIGH  
**Impact:** Low - Quality of life features  
**Estimated Effort:** 2-3 weeks  
**Dependencies:** None

**Missing Features:**
- Faction tokens (currency system)
- Tier-gated recipes (recipes need tier assignment)
- Title system (Exalted tier)
- Unique items at Exalted tier

**Required Components:**
- Faction token items
- Token currency system
- Title system
- Unique item vendors
- Tier-gated recipe system

**Files to Create/Update:**
- `ServUO/Scripts/Items/Vystia/Factions/FactionTokens.cs` (new file)
- `ServUO/Scripts/Custom/VystiaClasses/Factions/VystiaTitleSystem.cs` (new file)
- `ServUO/Scripts/Custom/VystiaClasses/Crafting/` (add tier assignments)

**Implementation Steps:**
1. Create faction token system
2. Implement title system
3. Add tier assignments to recipes
4. Create unique item vendors
5. Test faction enhancements

---

## 🟢 Medium Priority Missing Features

### 8. Pilgrimage System

**Priority:** MEDIUM  
**Impact:** Low - Piety generation method  
**Estimated Effort:** 1 week  
**Dependencies:** None

**Missing:**
- Pilgrimage tracking system
- Pilgrimage cooldown (weekly)
- Pilgrimage rewards (+75 piety per shrine visit)

**Files to Create/Update:**
- `ServUO/Scripts/Custom/VystiaClasses/Religion/VystiaPilgrimageSystem.cs` (new file)
- `ServUO/Scripts/Items/Vystia/Religious/VystiaShrine.cs` (add pilgrimage)

**Implementation Steps:**
1. Create pilgrimage tracking system
2. Implement weekly cooldown
3. Add pilgrimage rewards
4. Test pilgrimage system

---

### 9. Boss Kill Integration

**Priority:** MEDIUM  
**Impact:** Medium - Reputation and piety rewards  
**Estimated Effort:** 1 week  
**Dependencies:** None

**Missing:**
- Boss death handlers call reputation rewards
- Boss death handlers call piety rewards
- Faction-aligned boss rewards
- Religion-aligned boss rewards

**Files to Update:**
- `ServUO/Scripts/Mobiles/Vystia/Bosses/BaseVystiaBoss.cs`
- `ServUO/Scripts/Mobiles/Vystia/Bosses/VystiaBossLootSystem.cs`

**Implementation Steps:**
1. Add reputation reward calls to boss death handlers
2. Add piety reward calls to boss death handlers
3. Test boss kill rewards
4. Verify faction/religion alignment

---

### 10. Donation System

**Priority:** MEDIUM  
**Impact:** Low - Reputation generation method  
**Estimated Effort:** 1 week  
**Dependencies:** None

**Missing:**
- Donation NPC/UI
- Gold donation system
- Reputation rewards (+50 per 1,000g, max 500/day)

**Files to Create:**
- `ServUO/Scripts/Mobiles/Vystia/NPCs/FactionDonationNPC.cs` (new file)
- `ServUO/Scripts/Custom/VystiaClasses/Factions/VystiaDonationSystem.cs` (new file)

**Implementation Steps:**
1. Create donation NPC
2. Implement donation UI
3. Add reputation rewards
4. Test donation system

---

### 11. PvP Kill Integration

**Priority:** MEDIUM  
**Impact:** Low - Faction PvP rewards  
**Estimated Effort:** 1 week  
**Dependencies:** PvP system

**Missing:**
- PvP kill handler
- Enemy faction kill detection
- Reputation rewards (+25 per kill)

**Files to Create/Update:**
- `ServUO/Scripts/Custom/VystiaClasses/Factions/VystiaPvPHandler.cs` (new file)

**Implementation Steps:**
1. Create PvP kill handler
2. Detect enemy faction kills
3. Award reputation rewards
4. Test PvP kill rewards

---

## ⚪ Low Priority Missing Features

### 12. Zone Control System

**Priority:** LOW  
**Impact:** Low - PvP zone rules  
**Estimated Effort:** 2 weeks  
**Dependencies:** None

**Missing:**
- Zone type system (Sanctuary, Contested, Lawless, Extreme)
- Zone visual indicators
- Zone rule enforcement
- Zone-specific services

**Status:** ⚠️ Zone system exists but needs verification

**Files to Verify/Update:**
- `ServUO/Scripts/Custom/VystiaClasses/Zones/`

---

### 13. Camping/Hiking System

**Priority:** LOW  
**Impact:** Low - Convenience feature  
**Estimated Effort:** 2 weeks  
**Dependencies:** None

**Missing:**
- Camping system (campfire creation, safe camp, outpost)
- Hiking system (travel to discovered locations)
- Location discovery system
- Wilderness damage bonuses

**Files to Create:**
- `ServUO/Scripts/Custom/VystiaClasses/Skills/CampingSystem.cs` (new file)
- `ServUO/Scripts/Custom/VystiaClasses/Skills/HikingSystem.cs` (new file)

---

## Integration Gaps

### Quest Integration ✅

**Status:** ✅ **FULLY INTEGRATED**

**Verification:**
- ✅ Quest → Piety rewards: Implemented
- ✅ Quest → Reputation rewards: Implemented
- ✅ Faction-specific quests: Supported
- ✅ Religion-specific quests: Supported

**Code Evidence:**
- `VystiaQuestSystem.cs` lines 221-236

---

### Boss Integration ⚠️

**Status:** ⚠️ **PARTIALLY INTEGRATED**

**Missing:**
- Boss kill → Reputation rewards: Framework exists, needs integration
- Boss kill → Piety rewards: Framework exists, needs integration

**Required Actions:**
- Add reputation reward calls to boss death handlers
- Add piety reward calls to boss death handlers

---

## Design Mismatches

### Faction Tier Thresholds

**Issue:** Implementation thresholds don't match design document

**Design:**
- Friendly: 1-1,500
- Allied: 1,501-4,500
- Honored: 4,501-9,000
- Revered: 9,001-15,000
- Exalted: 15,000+

**Implementation:**
- Friendly: 3,000+
- Honored: 6,000+
- Revered: 12,000+
- Exalted: 15,000+

**Recommendation:** Align thresholds with design or update design document

---

## Implementation Roadmap

### Phase 1: Critical Gaps (4-6 weeks)
1. Implement missing crafting recipes (7 disciplines) - 2-3 weeks
2. Implement transmutation resource potions - 1 week
3. Implement engineering constructs - 2 weeks
4. Complete devotion power TODOs - 1 week

### Phase 2: High Priority (3-4 weeks)
5. Implement portable shrines - 1 week
6. Implement major temples - 2 weeks
7. Faction system enhancements - 2-3 weeks

### Phase 3: Medium Priority (4-5 weeks)
8. Pilgrimage system - 1 week
9. Boss kill integration - 1 week
10. Donation system - 1 week
11. PvP kill integration - 1 week

### Phase 4: Low Priority (4+ weeks)
12. Zone control system - 2 weeks
13. Camping/hiking system - 2 weeks

**Total Estimated Effort:** 15-19 weeks for all gaps

---

## Recommendations

### Immediate Actions (Week 1-2)
1. Start with missing crafting recipes (highest impact)
2. Complete devotion power TODOs (quick wins)
3. Begin transmutation potions (enables resource management)

### Short-term Actions (Week 3-6)
4. Implement engineering constructs
5. Add portable shrines
6. Integrate boss kill rewards

### Long-term Actions (Week 7+)
7. Major temples
8. Faction enhancements
9. Zone control system
10. Camping/hiking system

---

**Document Status:** Complete  
**Last Updated:** 2025-01-10
