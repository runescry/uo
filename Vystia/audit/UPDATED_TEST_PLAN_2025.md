# Vystia Systems - Updated Comprehensive Test Plan

**Date:** 2025-01-10  
**Status:** Complete Test Plan for All Implemented Systems  
**Purpose:** Systematic testing of all 64 todo items and Vystia systems

---

## Executive Summary

This test plan covers all Vystia systems including:
- **Crafting System** (7 disciplines + recipes)
- **Resource Potions** (14 types)
- **Engineering Constructs** (5 types)
- **Devotion Powers** (4 fixes)
- **Portable Shrines** (6 religions)
- **Major Temples** (6 temples)
- **Faction System** (tokens, tier-gated recipes, titles, exalted items)
- **Pilgrimage System** (tracking, cooldown, rewards)
- **Boss Kill Integration** (reputation & piety)
- **Donation System** (NPC, UI, rewards)
- **PvP Kill Integration** (faction rewards)
- **Zone Control System** (verification)
- **Camping/Hiking System** (campfire, safe camp, outpost, location discovery)
- **Faction Threshold Fix** (verification)

---

## Test Categories

1. **Critical System Tests** - Core functionality that must work
2. **Integration Tests** - Cross-system interactions
3. **Feature Tests** - Individual feature validation
4. **Edge Case Tests** - Boundary conditions and error handling
5. **Performance Tests** - System load and responsiveness

---

## 1. CRITICAL SYSTEM TESTS

### Test 1.1: Crafting Disciplines (7 Disciplines)

**Purpose:** Verify all 7 missing crafting disciplines are implemented and functional

**Test Disciplines:**
1. Runecrafting (Enchanter)
2. Inscription (Oracle)
3. Leathercraft (Ranger)
4. Woodshaping (Druid)
5. Clothcraft (Bard)
6. Necrocraft (Necromancer)
7. Jewelcraft (Sorcerer)

**Prerequisites:**
- Character with appropriate class
- Crafting workstation available
- Materials gathered

**Steps (per discipline):**
1. Create character with appropriate class
2. Verify crafting discipline exists (`Def[Discipline].cs`)
3. Access workstation (Runic Altar, Scriptorium, Tanning Rack, etc.)
4. Verify crafting menu displays
5. Verify recipes are available
6. Gather required materials
7. Attempt to craft basic item
8. Verify item created successfully
9. Verify materials consumed
10. Verify skill gain occurs

**Expected Results:**
- All 7 disciplines exist and are accessible
- Workstations function correctly
- Recipes are available
- Crafting succeeds with proper materials
- Items created match recipe specifications
- Materials consumed correctly
- Skill gains occur

**Verification Files:**
- `ServUO/Scripts/Custom/VystiaClasses/Crafting/DefRunecrafting.cs`
- `ServUO/Scripts/Custom/VystiaClasses/Crafting/DefInscription.cs`
- `ServUO/Scripts/Custom/VystiaClasses/Crafting/DefLeathercraft.cs`
- `ServUO/Scripts/Custom/VystiaClasses/Crafting/DefWoodshaping.cs`
- `ServUO/Scripts/Custom/VystiaClasses/Crafting/DefClothcraft.cs`
- `ServUO/Scripts/Custom/VystiaClasses/Crafting/DefNecrocraft.cs`
- `ServUO/Scripts/Custom/VystiaClasses/Crafting/DefJewelcraft.cs`

**Test Status:** ⚠️ Requires verification of recipe completeness

---

### Test 1.2: Resource Potions (14 Types)

**Purpose:** Verify all 14 resource potions are implemented and functional

**Test Potions:**
1. Fury Draught (+15 max Fury)
2. Berserker's Blood (Fury decay -35%)
3. Chi Elixir (+1 max Chi)
4. Focused Serum (Focus decay -35%)
5. Zealot's Tonic (+1 Zeal per kill)
6. Knight's Fortifier (+2 max Fortitude)
7. Hunter's Mark Oil (+35% Mark duration)
8. Shard Catalyst (+5% Soul Shard gen)
9. LifeForce Flask (Store 35 LifeForce)
10. Chill Enhancer (+5% freeze duration)
11. Crescendo Catalyst (+35% Crescendo gen)
12. Faith Vessel (Store 35 Faith)
13. Steam Concentrate (Portable Steam, 35 charges)
14. Virtue Essence (+1 to one Virtue)

**Prerequisites:**
- Alchemist character
- Transmutation skill
- Required reagents
- Mortar and Pestle

**Steps (per potion):**
1. Create Alchemist character
2. Access Alchemist's Lab or use Mortar and Pestle
3. Verify potion recipe exists in DefTransmutation.cs
4. Gather required reagents
5. Craft potion
6. Verify potion created
7. Use potion on appropriate class character
8. Verify effect applies correctly
9. Verify duration/charges work
10. Verify stacking rules (if applicable)

**Expected Results:**
- All 14 potions craftable
- Recipes exist in DefTransmutation.cs
- Potions create successfully
- Effects apply correctly
- Durations/charges work
- Stacking rules enforced

**Verification Files:**
- `ServUO/Scripts/Custom/VystiaClasses/Crafting/DefTransmutation.cs`
- `ServUO/Scripts/Items/Vystia/Consumables/TransmutationPotions.cs`

**Test Status:** ✅ Implementation verified, requires functional testing

---

### Test 1.3: Engineering Constructs (5 Types)

**Purpose:** Verify all 5 engineering constructs are implemented and functional

**Test Constructs:**
1. Clockwork Spider (50 HP, Scout, 1 slot)
2. Repair Drone (Heals constructs, 1 slot)
3. Steam Turret (100 HP, Ranged, 2 slots)
4. Iron Golem (500 HP, Tank, 3 slots)
5. Siege Engine (Territory warfare, 5 slots)

**Prerequisites:**
- Artificer character
- Engineering skill
- Required materials (Clockwork Ingots, Gears, Springs, Steam Cores)
- Engineering Tool Kit

**Steps (per construct):**
1. Create Artificer character
2. Access Steam Forge
3. Verify construct recipe exists in DefEngineering.cs
4. Gather required materials
5. Craft construct core
6. Verify core created
7. Use core to summon construct
8. Verify construct summoned correctly
9. Verify construct stats (HP, slots, abilities)
10. Test construct functionality (scout, heal, attack, etc.)
11. Verify construct duration/charges
12. Verify control slot usage

**Expected Results:**
- All 5 constructs craftable
- Recipes exist in DefEngineering.cs
- Constructs summon successfully
- Stats match design specifications
- Functionality works correctly
- Control slots used correctly

**Verification Files:**
- `ServUO/Scripts/Custom/VystiaClasses/Crafting/DefEngineering.cs`
- `ServUO/Scripts/Items/Vystia/Constructs/EngineeringConstructs.cs`

**Test Status:** ✅ Implementation verified, requires functional testing

---

### Test 1.4: Devotion Power Fixes (4 Powers)

**Purpose:** Verify all 4 devotion power fixes are implemented correctly

**Test Powers:**
1. Endurance of Winter - "Cannot die" flag
2. Nature's Sanctuary - Zone-based healing bonus
3. Celestial Alignment - Spell count tracking
4. Abyssal Call - Water elemental summon

**Prerequisites:**
- Character with appropriate religion
- Required piety tier (Faithful/Exalted)
- Power not on cooldown

**Steps (per power):**

**Endurance of Winter:**
1. Join Frosthelm Faith
2. Reach Faithful tier (500 piety)
3. Activate Endurance of Winter
4. Take lethal damage
5. Verify HP stays at 1 (cannot die)
6. Verify duration (5 seconds)
7. Verify damage reduction (-50%) applies
8. Verify power expires after 5 seconds

**Nature's Sanctuary:**
1. Join Lunara's Covenant
2. Reach Faithful tier (500 piety)
3. Activate Nature's Sanctuary
4. Verify 4-tile zone created
5. Stand in zone
6. Take damage
7. Verify +25% healing bonus applies
8. Verify duration (20 seconds)
9. Verify zone expires correctly

**Celestial Alignment:**
1. Join Celestis Arcanum
2. Reach Exalted tier (900 piety)
3. Activate Celestial Alignment
4. Cast spell
5. Verify mana cost is 0
6. Verify spell count tracked (max 4)
7. Cast 4 spells
8. Verify 5th spell costs mana
9. Verify duration expires after 8 seconds

**Abyssal Call:**
1. Join Oceana's Covenant
2. Reach Faithful tier (500 piety)
3. Activate Abyssal Call
4. Verify water elemental summoned
5. Verify elemental stats (200 HP, 2 slots)
6. Verify elemental follows commands
7. Verify elemental expires after 2 minutes

**Expected Results:**
- All 4 powers activate correctly
- Effects match design specifications
- Durations work correctly
- Cooldowns apply correctly
- Boss interactions work (if applicable)

**Verification Files:**
- `ServUO/Scripts/Custom/VystiaClasses/Religion/VystiaDevotionPowers.cs`

**Test Status:** ✅ Implementation verified, requires functional testing

---

## 2. HIGH PRIORITY FEATURE TESTS

### Test 2.1: Portable Shrines (6 Religions)

**Purpose:** Verify all 6 portable shrines are implemented and functional

**Test Shrines:**
1. Cogsmith's Portable Anvil (Cogsmith Creed)
2. Moonstone Circle (Lunara's Covenant)
3. Sun Dial (Surya's Sandscript)
4. Tide Pool Basin (Oceana's Covenant)
5. Star Chart (Celestis Arcanum)
6. Frost Totem (Frosthelm Faith)

**Prerequisites:**
- Character with appropriate religion
- Required materials for crafting
- Crafting skill

**Steps (per shrine):**
1. Join appropriate religion
2. Gather required materials
3. Craft portable shrine (or obtain via GM command)
4. Place shrine in world
5. Verify shrine functions (prayer, tithe, etc.)
6. Verify shrine effects apply
7. Verify shrine uses decrement
8. Verify shrine expires after uses

**Expected Results:**
- All 6 shrines craftable/obtainable
- Shrines place correctly
- Shrine functions work
- Effects apply correctly
- Uses decrement correctly
- Shrines expire after uses

**Verification Files:**
- `ServUO/Scripts/Items/Vystia/Religious/ThematicPortableShrines.cs`
- `ServUO/Scripts/Custom/VystiaClasses/Religion/PortableShrine.cs`

**Test Status:** ✅ Implementation verified, requires functional testing

---

### Test 2.2: Major Temples (6 Temples)

**Purpose:** Verify all 6 major temples are implemented and functional

**Test Temples:**
1. The Grand Foundry (Cogsmith Creed, Ironclad Capital)
2. Grove of the Moon (Lunara's Covenant, Verdantpeak Heart)
3. Temple of the Sun (Surya's Sandscript, Desert Oasis)
4. Abyssal Cathedral (Oceana's Covenant, Forgotten Depths)
5. Astral Observatory (Celestis Arcanum, Crystal Barrens Peak)
6. Frost Father's Sanctum (Frosthelm Faith, Frosthold Summit)

**Prerequisites:**
- Character with appropriate religion
- Access to temple location

**Steps (per temple):**
1. Join appropriate religion
2. Travel to temple location
3. Verify temple exists and is accessible
4. Interact with temple
5. Verify temple functions available:
   - Prayer
   - Tithe
   - Pilgrimage
   - Temple Blessing
   - Group Ritual
   - Power Recharge
   - Item Blessing
   - Resurrection
6. Verify temple-specific bonuses apply
7. Verify temple aura effects

**Expected Results:**
- All 6 temples exist and are accessible
- Temple functions work correctly
- Temple-specific bonuses apply
- Aura effects work
- Group rituals function

**Verification Files:**
- `ServUO/Scripts/Items/Vystia/Religious/MajorTemples.cs`

**Test Status:** ✅ Implementation verified, requires functional testing

---

### Test 2.3: Faction Enhancements

**Purpose:** Verify faction token system, tier-gated recipes, titles, and exalted items

**Test Components:**
1. Faction Token currency system
2. Tier-gated recipe access system
3. Faction Title system (Exalted tier)
4. Unique Exalted-tier faction items

**Prerequisites:**
- Character with faction
- Reputation at various tiers
- Access to faction vendors

**Steps:**

**Faction Tokens:**
1. Join faction
2. Complete quest/kill boss
3. Verify tokens awarded
4. Verify tokens stored correctly
5. Access faction vendor
6. Verify tokens can be spent
7. Verify token costs are correct

**Tier-Gated Recipes:**
1. Join faction
2. Gain Friendly reputation (3,000+)
3. Access crafting system
4. Verify Friendly-tier recipes available
5. Gain Honored reputation (6,000+)
6. Verify Honored-tier recipes available
7. Gain Revered reputation (12,000+)
8. Verify Revered-tier recipes available
9. Gain Exalted reputation (15,000+)
10. Verify Exalted-tier recipes available

**Faction Titles:**
1. Join faction
2. Gain Exalted reputation (15,000+)
3. Verify title awarded
4. Verify title displayed
5. Verify title persists

**Exalted Items:**
1. Reach Exalted tier
2. Access faction vendor
3. Verify Exalted-tier items available
4. Purchase Exalted item
5. Verify item properties
6. Verify item bonuses

**Expected Results:**
- Token system works correctly
- Tier-gated recipes unlock at correct thresholds
- Titles awarded and displayed correctly
- Exalted items available and functional

**Verification Files:**
- `ServUO/Scripts/Items/Vystia/Faction/FactionTokens.cs`
- `ServUO/Scripts/Custom/VystiaClasses/Crafting/TierGatedRecipes.cs`
- `ServUO/Scripts/Custom/VystiaClasses/Factions/VystiaFactionTitles.cs`
- `ServUO/Scripts/Items/Vystia/Faction/ExaltedFactionItems.cs`

**Test Status:** ✅ Implementation verified, requires functional testing

---

### Test 2.4: Pilgrimage System

**Purpose:** Verify pilgrimage tracking, cooldown, and piety rewards

**Prerequisites:**
- Character with religion
- Access to shrine

**Steps:**
1. Join religion
2. Visit shrine
3. Perform pilgrimage
4. Verify +75 piety awarded
5. Verify pilgrimage tracked
6. Attempt second pilgrimage immediately
7. Verify weekly cooldown enforced
8. Wait 7 days (or use GM command to reset)
9. Perform second pilgrimage
10. Verify piety awarded again
11. Verify multiple shrines can be visited
12. Verify pilgrimage tracking persists

**Expected Results:**
- Pilgrimage awards +75 piety
- Weekly cooldown enforced
- Tracking persists
- Multiple shrines can be visited

**Verification Files:**
- `ServUO/Scripts/Custom/VystiaClasses/Religion/VystiaReligionSystem.cs`

**Test Status:** ✅ Implementation verified, requires functional testing

---

## 3. INTEGRATION TESTS

### Test 3.1: Boss Kill Integration

**Purpose:** Verify boss kills award reputation and piety correctly

**Prerequisites:**
- Boss spawned
- Character with faction and/or religion
- Party members (optional)

**Steps:**
1. Spawn regional boss
2. Character has aligned faction
3. Kill boss
4. Verify reputation awarded (+100)
5. Verify tokens awarded
6. Character has aligned religion
6. Kill boss again
7. Verify piety awarded (+25)
8. Test with party
9. Verify party members receive rewards
10. Verify group reward reduction (75%)

**Expected Results:**
- Boss kills award reputation
- Boss kills award piety (if religion aligned)
- Tokens awarded
- Party rewards work correctly
- Group reduction applies

**Verification Files:**
- `ServUO/Scripts/Custom/VystiaClasses/Factions/VystiaBossRewards.cs`
- `ServUO/Scripts/Mobiles/Vystia/Bosses/BaseVystiaBoss.cs`

**Test Status:** ✅ Implementation verified, requires functional testing

---

### Test 3.2: Donation System

**Purpose:** Verify faction donation NPC, UI, and reputation rewards

**Prerequisites:**
- Faction donation NPC spawned
- Character with gold
- Character with faction

**Steps:**
1. Access faction donation NPC
2. Verify donation UI displays
3. Donate 1,000 gold
4. Verify +50 reputation awarded
5. Donate 5,000 gold
6. Verify +250 reputation awarded
7. Donate 10,000 gold
8. Verify +500 reputation awarded
9. Test custom amount donation
10. Verify gold withdrawn correctly
11. Verify reputation calculation correct

**Expected Results:**
- Donation NPC accessible
- UI displays correctly
- Reputation awarded correctly (+50 per 1,000g)
- Gold withdrawn correctly
- Custom amounts work

**Verification Files:**
- `ServUO/Scripts/Custom/VystiaClasses/Factions/VystiaFactionDonationNPC.cs`
- `ServUO/Scripts/Custom/VystiaClasses/Factions/VystiaFactionSystem.cs`

**Test Status:** ✅ Implementation verified, requires functional testing

---

### Test 3.3: PvP Kill Integration

**Purpose:** Verify PvP kills award faction reputation correctly

**Prerequisites:**
- Two characters with different factions
- PvP zone (Contested/Lawless/Extreme)
- PvP flag enabled

**Steps:**
1. Character A joins faction 1
2. Character B joins faction 2 (enemy faction)
3. Both enter PvP zone
4. Both enable PvP flag
5. Character A kills Character B
6. Verify +25 reputation awarded to Character A
7. Verify tokens awarded
8. Test cooldown (30 minutes)
9. Kill same player again
10. Verify cooldown enforced
11. Test zone multipliers (Lawless 1.5x, Extreme 2.0x)
12. Test skill difference bonus

**Expected Results:**
- PvP kills award reputation (+25 base)
- Tokens awarded
- Cooldown enforced (30 minutes)
- Zone multipliers apply
- Skill difference bonus applies
- Enemy faction bonus applies

**Verification Files:**
- `ServUO/Scripts/Custom/VystiaClasses/Factions/VystiaPvPRewards.cs`
- `ServUO/Scripts/Mobiles/PlayerMobile.cs`

**Test Status:** ✅ Implementation verified, requires functional testing

---

### Test 3.4: Zone Control System Verification

**Purpose:** Verify zone control system functionality

**Prerequisites:**
- Characters in different zones
- Zone system enabled

**Steps:**
1. Enter Sanctuary zone
2. Verify PvP disabled
3. Verify guards respond
4. Enter Contested zone
5. Verify PvP requires consent
6. Verify guards respond
7. Enter Lawless zone
8. Verify open PvP
9. Verify no guards
10. Verify death penalties (50% skill loss, 50% loot drop)
11. Enter Extreme zone
12. Verify full PvP
13. Verify full death penalties (100% skill loss, 100% loot drop)
14. Test zone transitions
15. Verify zone colors display

**Expected Results:**
- Zone types work correctly
- PvP rules enforced
- Death penalties match zone
- Guards respond correctly
- Zone colors display

**Verification Files:**
- `ServUO/Scripts/Custom/VystiaClasses/Zones/VystiaZoneSystem.cs`

**Test Status:** ✅ Implementation verified, requires functional testing

---

## 4. FEATURE TESTS

### Test 4.1: Camping System

**Purpose:** Verify camping system (campfire, safe camp, outpost)

**Prerequisites:**
- Character with camping skill
- Camping kit or kindling
- Valid location

**Steps:**

**Campfire:**
1. Use kindling or camping kit
2. Create campfire
3. Verify campfire placed
4. Stand near campfire
5. Verify regen bonus applies
6. Verify campfire duration (30 minutes)
7. Verify campfire expires

**Safe Camp:**
1. Use camping kit (Safe Camp type)
2. Verify camping skill requirement (50)
3. Create safe camp
4. Verify safe camp placed
5. Verify monster aggro reduction
6. Verify safe camp duration
7. Verify safe camp expires

**Outpost:**
1. Use camping kit (Outpost type)
2. Verify camping skill requirement (80)
3. Create outpost
4. Verify outpost placed
5. Verify vendor access
6. Verify storage access
7. Verify outpost duration
8. Verify outpost expires

**Expected Results:**
- All camp types create successfully
- Skill requirements enforced
- Effects apply correctly
- Durations work
- Camps expire correctly

**Verification Files:**
- `ServUO/Scripts/Custom/VystiaClasses/Systems/VystiaCampingSystem.cs`

**Test Status:** ✅ Implementation verified, requires functional testing

---

### Test 4.2: Hiking System

**Purpose:** Verify hiking system (location discovery, travel)

**Prerequisites:**
- Character with camping skill
- Gold for travel costs

**Steps:**
1. Discover location (use command or manual discovery)
2. Verify waypoint created
3. Verify waypoint saved
4. Travel to waypoint
5. Verify travel cost calculated correctly
6. Verify gold deducted
7. Verify player teleported
8. Verify cooldown enforced (5 minutes)
9. Test camping skill discount (up to 50%)
10. Test multiple waypoints
11. Test waypoint removal
12. Verify waypoints persist after logout

**Expected Results:**
- Location discovery works
- Waypoints created and saved
- Travel works correctly
- Costs calculated correctly
- Cooldown enforced
- Skill discount applies
- Waypoints persist

**Verification Files:**
- `ServUO/Scripts/Custom/VystiaClasses/Systems/VystiaHikingSystem.cs`

**Test Status:** ✅ Implementation verified, requires functional testing

---

### Test 4.3: Faction Threshold Fix

**Purpose:** Verify faction reputation thresholds match design document

**Prerequisites:**
- Character with faction
- Ability to modify reputation (GM command)

**Steps:**
1. Check current threshold values in code
2. Compare with main design document (VYSTIA_COMPLETE_DESIGN_DOCUMENT.md):
   - Friendly: 1 to 1,500 (5% discount)
   - Allied: 1,501 to 4,500 (10% discount)
   - Honored: 4,501 to 9,000 (12% discount)
   - Exalted: 9,001 to 15,000 (15% discount)
3. Set reputation to 0
4. Verify tier is Neutral
5. Add +1 reputation
6. Verify tier changes to Friendly
7. Set reputation to 1,500
8. Verify tier is Friendly
9. Add +1 reputation
10. Verify tier changes to Allied
11. Repeat for each threshold (Allied→Honored→Exalted)
12. Verify discount percentages match tiers
13. Verify tier-gated recipes unlock at correct thresholds
14. Check FactionCommands.cs display text (may show outdated values)

**Expected Results:**
- Thresholds match main design document
- Tier transitions work correctly at: 1, 1,501, 4,501, 9,001
- Discounts match tiers (5%, 10%, 12%, 15%)
- Recipe unlocks match tiers
- Note: FactionCommands.cs display may show outdated thresholds

**Verification Files:**
- `ServUO/Scripts/Custom/VystiaClasses/Factions/VystiaFactionSystem.cs`
- `ServUO/Scripts/Custom/VystiaClasses/Factions/FactionCommands.cs` (check display text)

**Test Status:** ✅ Implementation verified - matches design doc. Display text needs update.

---

## 5. EDGE CASE TESTS

### Test 5.1: Resource Overflow

**Purpose:** Verify resources don't overflow beyond maximum

**Steps:**
1. Create character with resource (Fury, max 100)
2. Generate resource to maximum
3. Attempt to generate more
4. Verify resource caps at maximum
5. Use ability requiring resource
6. Verify resource decreases
7. Generate resource again
8. Verify resource increases correctly

**Expected Results:**
- Resources cap at maximum
- Resources decrease on use
- Resources increase correctly

---

### Test 5.2: Cooldown Edge Cases

**Purpose:** Verify cooldowns work correctly at boundaries

**Steps:**
1. Activate devotion power
2. Verify cooldown set
3. Attempt to activate again immediately
4. Verify cooldown enforced
5. Wait for cooldown to expire
6. Verify power can be activated again
7. Test power recharge (50% reduction)
8. Verify cooldown reduced correctly

**Expected Results:**
- Cooldowns enforced correctly
- Cooldowns expire correctly
- Power recharge works correctly

---

### Test 5.3: Multiple System Interactions

**Purpose:** Verify multiple systems work together correctly

**Steps:**
1. Character with class, religion, and faction
2. Use devotion power
3. Craft item with tier-gated recipe
4. Use resource potion
5. Kill boss
6. Verify all rewards apply correctly
7. Verify no conflicts between systems

**Expected Results:**
- All systems work together
- No conflicts
- Rewards apply correctly

---

## 6. PERFORMANCE TESTS

### Test 6.1: System Load

**Purpose:** Verify systems handle load correctly

**Steps:**
1. Spawn 50+ players
2. All perform crafting simultaneously
3. All use devotion powers simultaneously
4. All access faction vendors simultaneously
5. Monitor server performance
6. Verify no lag or crashes

**Expected Results:**
- Systems handle load
- No performance degradation
- No crashes

---

### Test 6.2: Data Persistence

**Purpose:** Verify data persists correctly

**Steps:**
1. Create character
2. Gain reputation, piety, waypoints
3. Logout
4. Server restart
5. Login
6. Verify all data persisted

**Expected Results:**
- All data persists
- No data loss
- Load times acceptable

---

## Test Execution Guidelines

### Test Environment Setup

1. **Server Configuration:**
   - ServUO server running
   - All systems loaded
   - Test database clean
   - GM access available

2. **Test Character Setup:**
   - GM character for spawning
   - Test characters for each class
   - Test characters for each religion
   - Test characters for each faction

3. **Test Data:**
   - Test items spawned
   - Test NPCs spawned
   - Test materials available
   - Test bosses spawned

### Test Documentation

**For Each Test:**
1. Document test name and purpose
2. Document prerequisites
3. Document steps taken
4. Document expected results
5. Document actual results
6. Document any issues found
7. Document resolution (if applicable)

### Test Reporting

**Test Report Format:**
- Test ID
- Test Name
- Status (Pass/Fail/Skip)
- Issues Found
- Resolution
- Notes

---

## Test Priority

### Critical Priority (Run First)
1. Crafting Disciplines (7 disciplines)
2. Resource Potions (14 types)
3. Engineering Constructs (5 types)
4. Devotion Power Fixes (4 powers)

### High Priority (Run Second)
5. Portable Shrines (6 religions)
6. Major Temples (6 temples)
7. Faction Enhancements (tokens, recipes, titles, items)
8. Pilgrimage System

### Medium Priority (Run Third)
9. Boss Kill Integration
10. Donation System
11. PvP Kill Integration
12. Zone Control Verification

### Low Priority (Run Fourth)
13. Camping/Hiking System
14. Faction Threshold Fix
15. Edge Case Tests
16. Performance Tests

---

## Known Issues

### Implementation Status

**Fully Implemented:**
- ✅ All 7 crafting disciplines
- ✅ All 14 resource potions
- ✅ All 5 engineering constructs
- ✅ All 4 devotion power fixes
- ✅ All 6 portable shrines
- ✅ All 6 major temples
- ✅ Faction enhancements (tokens, recipes, titles, items)
- ✅ Pilgrimage system
- ✅ Boss kill integration
- ✅ Donation system
- ✅ PvP kill integration
- ✅ Zone control system
- ✅ Camping/hiking system

**Needs Verification:**
- ⚠️ Recipe completeness for all 7 disciplines
- ⚠️ Wilderness damage bonuses (not found in search)
- ⚠️ FactionCommands.cs display text (shows outdated thresholds)

---

## Test Completion Checklist

- [ ] All 7 crafting disciplines tested
- [ ] All 14 resource potions tested
- [ ] All 5 engineering constructs tested
- [ ] All 4 devotion power fixes tested
- [ ] All 6 portable shrines tested
- [ ] All 6 major temples tested
- [ ] Faction enhancements tested
- [ ] Pilgrimage system tested
- [ ] Boss kill integration tested
- [ ] Donation system tested
- [ ] PvP kill integration tested
- [ ] Zone control system tested
- [ ] Camping/hiking system tested
- [ ] Faction threshold fix verified (✅ Implementation correct, display text needs update)
- [ ] Edge cases tested
- [ ] Performance tests completed

---

**Document Status:** Complete  
**Last Updated:** 2025-01-10  
**Next Review:** After test execution

