# Validation and Testing Plan

**Date:** 2025-01-10  
**Status:** Comprehensive Testing Plan  
**Purpose:** Provide systematic testing scenarios for all documented flows and integrations

---

## Overview

This document provides comprehensive testing scenarios for validating all Vystia systems, their integrations, and mechanic flows. Each test scenario includes prerequisites, steps, expected results, and verification methods.

---

## Test Categories

1. **System Integration Tests** - Test flows between systems
2. **Mechanic Flow Tests** - Test individual system flows
3. **Integration Point Tests** - Test specific integration points
4. **Edge Case Tests** - Test boundary conditions and error handling

---

## 1. System Integration Tests

### Test 1.1: Class → Crafting → Vendor → Quest Flow

**Purpose:** Verify complete flow from class selection through crafting to quest completion

**Prerequisites:**
- Artificer character created
- Faction reputation (Friendly+)
- Materials gathered

**Steps:**
1. Create Artificer character
2. Join Ironclad Alliance faction
3. Gain Friendly reputation (3,000+)
4. Gather materials (Gears, Springs, Steam Core)
5. Access faction vendor
6. Verify discount applied (5%)
7. Purchase materials if needed
8. Access Steam Forge
9. Craft construct (Clockwork Spider)
10. Verify construct created
11. Accept faction-aligned quest
12. Complete quest objectives
13. Verify reputation reward given
14. Verify vendor discount increased (if tier progressed)

**Expected Results:**
- Character created with Artificer class
- Faction reputation gained
- Vendor discount applied correctly
- Construct crafted successfully
- Quest completed
- Reputation reward given
- Vendor discount updated if tier progressed

**Verification:**
- Check character class
- Check faction reputation value
- Check vendor discount percentage
- Check construct in inventory
- Check quest completion status
- Check reputation value after quest

**Files to Verify:**
- `ServUO/Scripts/Custom/VystiaClasses/Classes/ArtificerClass.cs`
- `ServUO/Scripts/Custom/VystiaClasses/Crafting/DefEngineering.cs`
- `ServUO/Scripts/Custom/VystiaClasses/Factions/VystiaFactionVendor.cs`
- `ServUO/Scripts/Custom/VystiaClasses/Quests/VystiaQuestSystem.cs`

---

### Test 1.2: NPC → Quest → LLM → Lore Flow

**Purpose:** Verify LLM NPC can offer quests with lore-integrated dialogue

**Prerequisites:**
- Quest NPC spawned
- Quest generated
- Lore entries loaded

**Steps:**
1. Spawn quest NPC
2. Link quest to NPC
3. Player approaches NPC
4. Player speaks to NPC (asks about quest)
5. Verify LLM generates quest-aware dialogue
6. Verify relevant lore retrieved
7. Verify lore integrated into response
8. NPC offers quest
9. Player accepts quest
10. Verify quest active
11. Player asks NPC about quest progress
12. Verify LLM generates progress-aware dialogue

**Expected Results:**
- Quest NPC spawned successfully
- LLM dialogue is quest-aware
- Relevant lore retrieved and integrated
- Quest offered and accepted
- Progress-aware dialogue generated

**Verification:**
- Check NPC quest link
- Check LLM response content
- Check lore retrieval
- Check quest status
- Check dialogue context

**Files to Verify:**
- `ServUO/Scripts/Custom/VystiaClasses/Quests/QuestNPC.cs`
- `ServUO/Scripts/Services/LLM/LLMConversationHelper.cs`
- `ServUO/Scripts/Services/LLM/Data/SimpleLoreSystem.cs`

---

### Test 1.3: Religion → Faction → Quest → Reward Flow

**Purpose:** Verify religion and faction systems work together in quest rewards

**Prerequisites:**
- Player has religion (Frosthelm Faith)
- Player has faction (Polar Alliance)
- Religion-aligned quest available
- Faction-aligned quest available

**Steps:**
1. Player joins Frosthelm Faith religion
2. Player joins Polar Alliance faction
3. Player accepts religion-aligned quest
4. Complete quest
5. Verify piety reward given
6. Verify piety amount correct for tier
7. Player accepts faction-aligned quest
8. Complete quest
9. Verify reputation reward given
10. Verify reputation amount correct for tier
11. Player accepts quest aligned to both
12. Complete quest
13. Verify both piety and reputation rewards given

**Expected Results:**
- Religion joined successfully
- Faction joined successfully
- Piety reward given for religion quest
- Reputation reward given for faction quest
- Both rewards given for dual-aligned quest

**Verification:**
- Check religion status
- Check faction status
- Check piety value before/after quest
- Check reputation value before/after quest
- Check quest reward properties

**Files to Verify:**
- `ServUO/Scripts/Custom/VystiaClasses/Religion/VystiaPiety.cs`
- `ServUO/Scripts/Custom/VystiaClasses/Factions/VystiaReputation.cs`
- `ServUO/Scripts/Custom/VystiaClasses/Quests/VystiaQuestSystem.cs`

---

### Test 1.4: PVE → Boss → Reputation → Vendor Flow

**Purpose:** Verify boss kills award reputation and enable vendor access

**Prerequisites:**
- Boss spawned
- Player has faction
- Faction vendor exists

**Steps:**
1. Spawn regional boss (Frost Father)
2. Player has Polar Alliance faction
3. Player kills boss
4. Verify reputation reward given (+100)
5. Check reputation tier
6. Access faction vendor
7. Verify discount matches tier
8. Verify tier-gated recipes available (if applicable)

**Expected Results:**
- Boss killed successfully
- Reputation reward given
- Reputation tier updated
- Vendor discount matches tier
- Tier-gated recipes available

**Verification:**
- Check boss death handler
- Check reputation value before/after
- Check reputation tier
- Check vendor discount
- Check recipe availability

**Files to Verify:**
- `ServUO/Scripts/Mobiles/Vystia/Bosses/BaseVystiaBoss.cs`
- `ServUO/Scripts/Custom/VystiaClasses/Factions/VystiaFactionSystem.cs`
- `ServUO/Scripts/Custom/VystiaClasses/Factions/VystiaFactionVendor.cs`

**Note:** This test may fail if boss kill integration not yet implemented. Document as known issue.

---

## 2. Mechanic Flow Tests

### Test 2.1: Class System Flow

**Purpose:** Verify complete class system flow from selection to ability usage

**Prerequisites:**
- Character creation system
- Class selection available

**Steps:**
1. Create character
2. Select class (Barbarian)
3. Verify starting gear equipped
4. Verify starting skills set
5. Verify secondary resource initialized (Fury = 0)
6. Enter combat
7. Deal damage to target
8. Verify Fury increases (+8 on hit)
9. Verify Fury UI updated
10. Use ability requiring Fury (Savage Strike, 20 Fury)
11. Verify Fury cost paid
12. Verify ability executes
13. Leave combat
14. Verify Fury decays (-5/sec)

**Expected Results:**
- Class selected successfully
- Starting gear equipped
- Skills set correctly
- Resource initialized
- Resource generates in combat
- Ability uses resource
- Resource decays out of combat

**Verification:**
- Check character class
- Check equipped items
- Check skill values
- Check resource value
- Check resource generation
- Check ability execution
- Check resource decay

**Files to Verify:**
- `ServUO/Scripts/Custom/VystiaClasses/Classes/BarbarianClass.cs`
- `ServUO/Scripts/Custom/VystiaClasses/Systems/VystiaResourceManager.cs`
- `ServUO/Scripts/Custom/VystiaClasses/Systems/SecondaryResource.cs`

---

### Test 2.2: Crafting System Flow

**Purpose:** Verify complete crafting flow from material gathering to item creation

**Prerequisites:**
- Alchemist character
- Materials available
- Workstation accessible

**Steps:**
1. Create Alchemist character
2. Gather materials (reagents)
3. Access Alchemist's Lab
4. Select recipe (potion)
5. Verify skill check (Alchemy Mastery)
6. Verify material check
7. Attempt craft
8. Verify quality roll
9. Verify item created
10. Verify materials consumed
11. Verify item quality correct
12. Use crafted item
13. Verify item effect works

**Expected Results:**
- Materials gathered
- Skill check passes
- Material check passes
- Item crafted successfully
- Materials consumed
- Item quality correct
- Item effect works

**Verification:**
- Check material inventory
- Check skill level
- Check crafting attempt
- Check item created
- Check material consumption
- Check item quality
- Check item effect

**Files to Verify:**
- `ServUO/Scripts/Custom/VystiaClasses/Crafting/DefTransmutation.cs`
- `ServUO/Scripts/Services/Craft/Core/CraftSystem.cs`

---

### Test 2.3: Quest System Flow

**Purpose:** Verify complete quest flow from generation to completion

**Prerequisites:**
- Quest generation system
- Quest NPC spawning

**Steps:**
1. Generate quest via LLM
2. Verify quest created
3. Verify quest waypoints created
4. Spawn quest NPC
5. Link quest to NPC
6. Player interacts with NPC
7. NPC offers quest
8. Player accepts quest
9. Verify quest active
10. Player completes waypoint 1
11. Verify waypoint 1 complete
12. Player completes waypoint 2
13. Verify waypoint 2 complete
14. All waypoints complete
15. Quest completes
16. Verify rewards given

**Expected Results:**
- Quest generated
- Waypoints created
- NPC spawned and linked
- Quest offered and accepted
- Waypoints complete
- Quest completes
- Rewards given

**Verification:**
- Check quest existence
- Check waypoint count
- Check NPC quest link
- Check quest status
- Check waypoint completion
- Check quest completion
- Check rewards given

**Files to Verify:**
- `ServUO/Scripts/Custom/VystiaClasses/Quests/VystiaQuestSystem.cs`
- `ServUO/Scripts/Custom/VystiaClasses/Quests/QuestNPC.cs`
- `ServUO/Scripts/Custom/VystiaClasses/Quests/QuestWaypoint.cs`

---

### Test 2.4: NPC System Flow

**Purpose:** Verify LLM NPC interaction flow

**Prerequisites:**
- LLM service configured
- Lore system loaded
- NPC spawned

**Steps:**
1. Spawn LLM NPC
2. Set personality (Wise Sage)
3. Set speech pattern (Formal)
4. Load lore knowledge base
5. Player approaches NPC
6. Player speaks to NPC
7. Verify speech detected
8. Verify LLM context built
9. Verify relevant lore retrieved
10. Verify LLM response generated
11. Verify response displayed
12. Verify response is context-appropriate

**Expected Results:**
- NPC spawned
- Personality set
- Lore loaded
- Speech detected
- Context built
- Lore retrieved
- Response generated
- Response appropriate

**Verification:**
- Check NPC properties
- Check personality
- Check lore loading
- Check speech detection
- Check context building
- Check lore retrieval
- Check response quality

**Files to Verify:**
- `ServUO/Scripts/Services/LLM/Core/LLMNpc.cs`
- `ServUO/Scripts/Services/LLM/LLMConversationHelper.cs`
- `ServUO/Scripts/Services/LLM/Data/SimpleLoreSystem.cs`

---

### Test 2.5: Religion System Flow

**Purpose:** Verify complete religion flow from conversion to power usage

**Prerequisites:**
- Religion system active
- Shrine available

**Steps:**
1. Player converts to religion (Frosthelm Faith)
2. Verify piety initialized (0)
3. Player prays at shrine
4. Verify +10 piety
5. Player tithes 1,000g
6. Verify +10 piety (capped at 30/day)
7. Verify tier progression (Initiate at 50)
8. Verify passive bonus applied
9. Player reaches Devoted tier (200)
10. Verify Devotion Power 1 available
11. Player uses Devotion Power 1
12. Verify power effect applied
13. Verify cooldown triggered
14. Player recharges at shrine
15. Verify cooldown reset

**Expected Results:**
- Religion joined
- Piety initialized
- Piety increases from prayer/tithe
- Tier progression works
- Passive bonuses apply
- Devotion powers available
- Powers work correctly
- Cooldown system works

**Verification:**
- Check religion status
- Check piety value
- Check tier
- Check passive bonuses
- Check power availability
- Check power effects
- Check cooldown

**Files to Verify:**
- `ServUO/Scripts/Custom/VystiaClasses/Religion/VystiaReligionSystem.cs`
- `ServUO/Scripts/Custom/VystiaClasses/Religion/VystiaPiety.cs`
- `ServUO/Scripts/Custom/VystiaClasses/Religion/VystiaDevotionPowers.cs`

---

### Test 2.6: Faction System Flow

**Purpose:** Verify complete faction flow from reputation to vendor access

**Prerequisites:**
- Faction system active
- Faction vendor available

**Steps:**
1. Player joins faction (Polar Alliance)
2. Verify reputation initialized (0)
3. Player completes faction quest
4. Verify reputation awarded (+50-500 based on tier)
5. Verify tier progression (Friendly at 3,000)
6. Access faction vendor
7. Verify discount applied (5%)
8. Player gains more reputation (Honored at 6,000)
9. Verify discount increased (8%)
10. Verify tier-gated recipes available

**Expected Results:**
- Faction joined
- Reputation initialized
- Reputation increases from quests
- Tier progression works
- Vendor discount applies
- Discount updates with tier
- Tier-gated recipes available

**Verification:**
- Check faction status
- Check reputation value
- Check tier
- Check vendor discount
- Check recipe availability

**Files to Verify:**
- `ServUO/Scripts/Custom/VystiaClasses/Factions/VystiaFactionSystem.cs`
- `ServUO/Scripts/Custom/VystiaClasses/Factions/VystiaReputation.cs`
- `ServUO/Scripts/Custom/VystiaClasses/Factions/VystiaFactionVendor.cs`

---

### Test 2.7: Combat System Flow

**Purpose:** Verify complete combat flow from initiation to damage application

**Prerequisites:**
- Character with class
- Target available
- Ability available

**Steps:**
1. Player selects target
2. Player selects ability
3. Verify resource cost validation
4. Pay resource costs
5. Resolve targets
6. Calculate base damage
7. Check crit chance
8. Apply crit multiplier if crit
9. Apply damage multipliers
10. Calculate resistances
11. Apply target debuffs
12. Apply final damage
13. Process on-hit effects
14. Generate resources
15. Apply stacks
16. Apply crowd control

**Expected Results:**
- Target selected
- Ability selected
- Costs validated and paid
- Targets resolved
- Damage calculated correctly
- Crit system works
- Multipliers applied
- Resistances calculated
- Damage applied
- Effects processed
- Resources generated
- Stacks applied
- CC applied

**Verification:**
- Check target selection
- Check ability selection
- Check cost payment
- Check damage calculation
- Check crit application
- Check effect application
- Check resource generation

**Files to Verify:**
- `ServUO/Scripts/Custom/VystiaClasses/Abilities/AbilityExecutor.cs`
- `ServUO/Scripts/Custom/VystiaClasses/Systems/VystiaDamageSystem.cs`
- `ServUO/Scripts/Custom/VystiaClasses/Systems/VystiaBuffSystem.cs`
- `ServUO/Scripts/Custom/VystiaClasses/Systems/CrowdControlSystem.cs`

---

## 3. Integration Point Tests

### Test 3.1: Class-Crafting Integration

**Purpose:** Verify each class with crafting ability has complete crafting support

**Test Classes:**
- Artificer (Engineering)
- Alchemist (Transmutation)
- Enchanter (Runecrafting)
- Oracle (Inscription)
- Ranger (Leathercraft)
- Druid (Woodshaping)
- Bard (Clothcraft)
- Necromancer (Necrocraft)
- Sorcerer (Jewelcraft)
- Fighter (Smithing)

**Steps (per class):**
1. Create class character
2. Verify class has crafting discipline
3. Verify vendor exists for class
4. Verify reagents/resources available
5. Verify workstation exists
6. Verify recipes exist (if implemented)
7. Attempt crafting
8. Verify crafting works

**Expected Results:**
- Each class has crafting discipline
- Vendors exist
- Materials available
- Workstations exist
- Recipes exist (or documented as missing)
- Crafting works (if recipes exist)

**Verification:**
- Check class crafting discipline
- Check vendor existence
- Check material availability
- Check workstation existence
- Check recipe existence
- Test crafting functionality

---

### Test 3.2: NPC-Quest Integration

**Purpose:** Verify NPCs can offer quests with LLM dialogue

**Test NPCs:**
- Faction leaders
- Quest givers
- Ancient beings

**Steps (per NPC type):**
1. Spawn NPC
2. Link quest to NPC (if applicable)
3. Player interacts with NPC
4. Verify LLM dialogue works
5. Verify quest offering works (if applicable)
6. Verify quest-aware dialogue

**Expected Results:**
- NPCs spawn successfully
- LLM dialogue works
- Quest offering works
- Dialogue is quest-aware

**Verification:**
- Check NPC spawning
- Check LLM integration
- Check quest offering
- Check dialogue context

---

### Test 3.3: Religion-Faction Integration

**Purpose:** Verify religion and faction systems work together

**Steps:**
1. Player joins religion
2. Player joins aligned faction
3. Verify synergy bonuses apply
4. Player completes quest aligned to both
5. Verify both piety and reputation rewards given
6. Verify bonuses stack correctly

**Expected Results:**
- Religion and faction work together
- Synergy bonuses apply
- Dual rewards given
- Bonuses stack correctly

**Verification:**
- Check religion status
- Check faction status
- Check synergy bonuses
- Check reward application

---

### Test 3.4: PVE-PVP Integration

**Purpose:** Verify PvE and PvP systems work together

**Steps:**
1. Player in PvE zone
2. Verify PvE rules apply
3. Player moves to PvP zone
4. Verify PvP rules apply
5. Player engages in PvP
6. Verify religion PvP bonuses apply
7. Verify faction PvP rewards apply
8. Verify death penalties match zone

**Expected Results:**
- Zone rules apply correctly
- PvP rules enforced
- Religion bonuses apply
- Faction rewards apply
- Death penalties correct

**Verification:**
- Check zone type
- Check PvP rules
- Check religion bonuses
- Check faction rewards
- Check death penalties

---

## 4. Edge Case Tests

### Test 4.1: Resource Overflow

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
- Resource caps at maximum
- Resource decreases on use
- Resource increases correctly

---

### Test 4.2: Quest Waypoint Order

**Purpose:** Verify quest waypoints can be completed in any order (if applicable)

**Steps:**
1. Create quest with multiple waypoints
2. Verify waypoint order (if sequential)
3. Complete waypoints in order
4. Verify quest completes
5. Create quest with parallel waypoints
6. Complete waypoints in different order
7. Verify quest completes

**Expected Results:**
- Sequential waypoints require order
- Parallel waypoints can be completed in any order
- Quest completes correctly

---

### Test 4.3: Religion Change Penalties

**Purpose:** Verify religion change penalties work correctly

**Steps:**
1. Player joins religion 1
2. Gain piety (500)
3. Change to religion 2 (first change)
4. Verify 50% piety retained (250, capped at 200)
5. Gain piety in religion 2 (300, total 500)
6. Change to religion 3 (second change)
7. Verify 25% piety retained (125, capped at 100)
8. Change to religion 4 (third change)
9. Verify 0% piety retained (full reset)

**Expected Results:**
- First change: 50% retained, cap 200
- Second change: 25% retained, cap 100
- Third+ change: 0% retained

---

### Test 4.4: Faction Reputation Boundaries

**Purpose:** Verify faction reputation tier boundaries work correctly

**Steps:**
1. Player has 2,999 reputation (Neutral)
2. Complete quest (+50 reputation)
3. Verify tier changes to Friendly (3,000+)
4. Verify discount applies (5%)
5. Player has 5,999 reputation (Friendly)
6. Complete quest (+50 reputation)
7. Verify tier changes to Honored (6,000+)
8. Verify discount increases (8%)

**Expected Results:**
- Tier boundaries work correctly
- Discounts update at boundaries
- Tier progression smooth

---

## Test Execution Guidelines

### Test Environment Setup

1. **Server Configuration:**
   - ServUO server running
   - All systems loaded
   - LLM service configured (if testing LLM features)
   - Test database clean

2. **Test Character Setup:**
   - GM character for spawning
   - Test characters for each class
   - Test characters for each religion
   - Test characters for each faction

3. **Test Data:**
   - Test items spawned
   - Test NPCs spawned
   - Test quests generated
   - Test materials available

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

## Known Test Limitations

### LLM Service Dependency

**Issue:** Some tests require LLM service to be configured and running

**Workaround:** Mock LLM responses or skip LLM-dependent tests if service unavailable

### Boss Kill Integration

**Issue:** Boss kill → Reputation/Piety rewards may not be integrated yet

**Workaround:** Document as known issue, test framework only

### Zone System

**Issue:** Zone system may not be fully functional

**Workaround:** Verify zone system exists, test basic functionality only

---

## Test Priority

### High Priority Tests (Run First)
1. Class system flow
2. Quest system flow
3. Religion system flow
4. Faction system flow
5. Class-crafting integration
6. Quest-religion-faction integration

### Medium Priority Tests (Run Second)
7. Crafting system flow
8. NPC system flow
9. Combat system flow
10. Boss integration
11. PvP integration

### Low Priority Tests (Run Third)
12. Edge case tests
13. Zone system tests
14. Camping/hiking tests

---

**Document Status:** Complete  
**Last Updated:** 2025-01-10
